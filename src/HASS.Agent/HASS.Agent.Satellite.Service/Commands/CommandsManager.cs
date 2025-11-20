using HASS.Agent.Shared.Enums;
using HASS.Agent.Shared.Models.Config;
using HASS.Agent.Satellite.Service.Settings;
using Serilog;

namespace HASS.Agent.Satellite.Service.Commands
{
    /// <summary>
    /// Continuously performs command autodiscovery and state publishing
    /// </summary>
    internal static class CommandsManager
    {
        private static bool _subscribed;

        private static bool _active = true;
        private static bool _pause;

        private static bool _discoveryPublished = false;

        private static DateTime _lastAutoDiscoPublish = DateTime.MinValue;

        /// <summary>
        /// Initializes the command manager
        /// </summary>
        internal static async void Initialize()
        {
            // wait while mqtt's connecting
            while (Variables.MqttManager.GetStatus() == MqttStatus.Connecting) await Task.Delay(250);

            // start background processing
            _ = Task.Run(Process);
        }

        /// <summary>
        /// Stop processing commands
        /// </summary>
        internal static void Stop() => _active = false;

        /// <summary>
        /// Pause processing commands
        /// </summary>
        internal static void Pause() => _pause = true;

        /// <summary>
        /// Resume processing commands
        /// </summary>
        internal static void Resume() => _pause = false;

        /// <summary>
        /// Unpublishes all commands
        /// </summary>
        /// <returns></returns>
        internal static async Task UnpublishAllCommands(bool migration = false)
        {
            try
            {
                // unpublish the autodisco's
                if (!CommandsPresent()) return;

                var count = 0;
                foreach (var command in Variables.Commands)
                {
                    await command.UnPublishAutoDiscoveryConfigAsync(migration);
                    await Variables.MqttManager.UnsubscribeAsync(command);
                    command.ClearAutoDiscoveryConfig();
                    count++;
                }

                Log.Information("[COMMANDSMANAGER] Unpublished {count} command(s)", count);

                // reset last publish & subscribed
                _lastAutoDiscoPublish = DateTime.MinValue;
                _subscribed = false;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "[COMMANDSMANAGER] Error while unpublishing: {err}", ex.Message);
            }

            _discoveryPublished = false;
        }

        /// <summary>
        /// Publishes all commands
        /// </summary>
        /// <returns></returns>
        internal static async Task ForcePublishAllCommands()
        {
            if (!CommandsPresent())
                return;

            foreach (var command in Variables.Commands)
            {
                await command.PublishAutoDiscoveryConfigAsync();
            }

            _discoveryPublished = false;
        }

        /// <summary>
        /// Generates new ID's for all commands
        /// </summary>
        internal static void ResetCommandIds()
        {
            if (!CommandsPresent()) return;
            foreach (var command in Variables.Commands) command.Id = Guid.NewGuid().ToString();

            StoredCommands.Store();
        }

        /// <summary>
        /// Continuously processes commands (autodiscovery, state)
        /// </summary>
        private static async void Process()
        {
            var firstRun = true;

            while (_active)
            {
                try
                {
                    if (firstRun)
                    {
                        // on the first run, just wait 1 sec - this is to make sure we're announcing ourselves,
                        // when there are no sensors or when the sensor manager's still initialising
                        await Task.Delay(TimeSpan.FromSeconds(1));
                    }
                    else await Task.Delay(TimeSpan.FromSeconds(30));

                    // are we paused?
                    if (_pause) continue;

                    // is mqtt available?
                    if (Variables.MqttManager.GetStatus() != MqttStatus.Connected)
                    {
                        _discoveryPublished = false;
                        continue;
                    }

                    // we're starting the first real run
                    firstRun = false;

                    // do we have commands?
                    if (!CommandsPresent()) continue;

                    // publish availability & autodiscovery every 30 sec
                    if ((DateTime.Now - _lastAutoDiscoPublish).TotalSeconds > 30)
                    {
                        await Variables.MqttManager.AnnounceAvailabilityAsync();

                        if (!_discoveryPublished)
                        {
                            foreach (var command in Variables.Commands.TakeWhile(_ => !_pause).TakeWhile(_ => _active))
                            {
                                if (_pause || Variables.MqttManager.GetStatus() != MqttStatus.Connected) continue;
                                await command.PublishAutoDiscoveryConfigAsync();
                            }

                            _discoveryPublished = true;
                        }

                        if (!_subscribed)
                        {
                            foreach (var command in Variables.Commands.TakeWhile(_ => !_pause).TakeWhile(_ => _active))
                            {
                                if (_pause || Variables.MqttManager.GetStatus() != MqttStatus.Connected) continue;
                                await Variables.MqttManager.SubscribeAsync(command);
                            }
                            _subscribed = true;
                        }

                        _lastAutoDiscoPublish = DateTime.Now;
                    }

                    // publish command states (they have their own time-based scheduling)
                    foreach (var command in Variables.Commands.TakeWhile(_ => !_pause).TakeWhile(_ => _active))
                    {
                        if (_pause || Variables.MqttManager.GetStatus() != MqttStatus.Connected) continue;
                        await command.PublishStateAsync();
                    }
                }
                catch (Exception ex)
                {
                    Log.Fatal(ex, "[COMMANDSMANAGER] Error while publishing: {err}", ex.Message);
                }
            }
        }

        /// <summary>
        /// Stores the provided commands, and (re)publishes them
        /// </summary>
        /// <param name="commands"></param>
        /// <param name="toBeDeletedCommands"></param>
        /// <returns></returns>
        internal static async Task<bool> StoreAsync(List<ConfiguredCommand> commands, List<ConfiguredCommand>? toBeDeletedCommands = null)
        {
            toBeDeletedCommands ??= new List<ConfiguredCommand>();

            try
            {
                // pause processing
                Pause();

                // process the to-be-removed
                if (toBeDeletedCommands.Any())
                {
                    foreach (var abstractCommand in toBeDeletedCommands.Select(StoredCommands.ConvertConfiguredToAbstract))
                    {
                        if (abstractCommand == null)
                            continue;

                        var commandIndex = Variables.Commands.FindIndex(x => x.Id == abstractCommand.Id);
                        if (commandIndex == -1)
                        {
                            await abstractCommand.UnPublishAutoDiscoveryConfigAsync();
                            await Variables.MqttManager.UnsubscribeAsync(abstractCommand);
                            Variables.Commands.RemoveAt(commandIndex);

                            Log.Information("[COMMANDS] Removed command: {command}", abstractCommand.EntityName);
                        }
                        else
                        {
                            Log.Information("[COMMANDS] Command not removed, not activated: {command}", abstractCommand.EntityName);
                        }
                    }
                }

                // copy our list to the main one
                foreach (var abstractCommand in commands.Select(StoredCommands.ConvertConfiguredToAbstract))
                {
                    if (abstractCommand == null)
                        continue;

                    if (Variables.Commands.All(x => x.Id != abstractCommand.Id))
                    {
                        // new, add and register
                        Variables.Commands.Add(abstractCommand);
                        await Variables.MqttManager.SubscribeAsync(abstractCommand);
                        await abstractCommand.PublishAutoDiscoveryConfigAsync();
                        await abstractCommand.PublishStateAsync(false);

                        Log.Information("[COMMANDS] Added command: {command}", abstractCommand.EntityName);
                        continue;
                    }

                    // existing, update and re-register
                    var currentCommandIndex = Variables.Commands.FindIndex(x => x.Id == abstractCommand.Id);
                    if (Variables.Commands[currentCommandIndex].EntityName != abstractCommand.EntityName || Variables.Commands[currentCommandIndex].EntityType != abstractCommand.EntityType)
                    {
                        // command changed, unregister and resubscribe on new mqtt channel
                        Log.Information("[COMMANDS] Command changed, re-registering as new entity: {old} to {new}", Variables.Commands[currentCommandIndex].EntityName, abstractCommand.EntityName);

                        await Variables.Commands[currentCommandIndex].UnPublishAutoDiscoveryConfigAsync();
                        await Variables.MqttManager.UnsubscribeAsync(Variables.Commands[currentCommandIndex]);
                        await Variables.MqttManager.SubscribeAsync(abstractCommand);
                    }

                    Variables.Commands[currentCommandIndex] = abstractCommand;
                    await abstractCommand.PublishAutoDiscoveryConfigAsync();
                    await abstractCommand.PublishStateAsync(false);

                    Log.Information("[COMMANDS] Modified command: {command}", abstractCommand.EntityName);
                }

                // annouce ourselves
                await Variables.MqttManager.AnnounceAvailabilityAsync();

                // store to file
                StoredCommands.Store();

                // done
                return true;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "[COMMANDSMANAGER] Error while storing: {err}", ex.Message);
                return false;
            }
            finally
            {
                // resume processing
                Resume();
            }
        }

        /// <summary>
        /// Processes, stores and activates the received commands
        /// </summary>
        /// <param name="commands"></param>
        internal static void ProcessReceivedCommands(List<ConfiguredCommand> commands)
        {
            try
            {
                if (!commands.Any())
                {
                    Log.Warning("[COMMANDSMANAGER] Received empty list, clearing all commands");
                    _ = SettingsManager.ClearAllCommandsAsync();
                    return;
                }

                // collect all current commands (to determine which are deleted)
                var currentCommands = Variables.Commands.Select(StoredCommands.ConvertAbstractToConfigured).ToList();

                // create the to-be-deleted list
                var toBeDeletedCommands = currentCommands.Where(command => command != null).Where(command => currentCommands.Any(x => x?.Id == command?.Id)).ToList();

                // log what we're doing
                Log.Information("[COMMANDSMANAGER] Processing {count} received command(s), deleting {del} command(s) ..", commands.Count, toBeDeletedCommands.Count);

                // process both lists
                _ = StoreAsync(commands, toBeDeletedCommands!);
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "[COMMANDSMANAGER] Error while processing received: {err}", ex.Message);
            }
        }

        private static bool CommandsPresent() => Variables.Commands.Any();
    }
}
