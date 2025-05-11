using HASS.Agent.Shared.Enums;
using HASS.Agent.Shared.Models.Config;
using HASS.Agent.Satellite.Service.Extensions;
using HASS.Agent.Satellite.Service.Settings;
using Serilog;

namespace HASS.Agent.Satellite.Service.Sensors
{
    /// <summary>
    /// Continuously performs sensor autodiscovery and state publishing
    /// </summary>
    internal static class SensorsManager
    {
        private static bool _active = true;
        private static bool _pause;

        private static bool _discoveryPublished = false;

        private static DateTime _lastAutoDiscoPublish = DateTime.MinValue;

        /// <summary>
        /// Initializes the sensor manager
        /// </summary>
        internal static async void Initialize()
        {
            // wait while mqtt's connecting
            while (Variables.MqttManager.GetStatus() == MqttStatus.Connecting) await Task.Delay(250);

            // start background processing
            _ = Task.Run(Process);
        }

        /// <summary>
        /// Stop processing sensor states
        /// </summary>
        internal static void Stop() => _active = false;

        /// <summary>
        /// Pause processing sensor states
        /// </summary>
        internal static void Pause() => _pause = true;

        /// <summary>
        /// Resume processing sensor states
        /// </summary>
        internal static void Resume() => _pause = false;

        /// <summary>
        /// Unpublishes all single- and multivalue sensors
        /// </summary>
        /// <returns></returns>
        internal static async Task UnpublishAllSensors()
        {
            try
            {
                var singleCount = 0;
                var multiCount = 0;

                // unpublish the autodisco's
                if (SingleValueSensorsPresent())
                {
                    foreach (var sensor in Variables.SingleValueSensors)
                    {
                        await sensor.UnPublishAutoDiscoveryConfigAsync();
                        sensor.ClearAutoDiscoveryConfig();
                        singleCount++;
                    }
                }

                if (MultiValueSensorsPresent())
                {
                    foreach (var sensor in Variables.MultiValueSensors)
                    {
                        await sensor.UnPublishAutoDiscoveryConfigAsync();
                        sensor.ClearAutoDiscoveryConfig();
                        multiCount++;
                    }
                }

                Log.Information("[SENSORSMANAGER] Unpublished {count} single-value sensor(s)", singleCount);
                Log.Information("[SENSORSMANAGER] Unpublished {count} multi-value sensor(s)", multiCount);

                // reset last publish
                _lastAutoDiscoPublish = DateTime.MinValue;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "[SENSORSMANAGER] Error while unpublishing: {err}", ex.Message);
            }
        }

        /// <summary>
        /// Continously processes sensors (autodiscovery, states)
        /// </summary>
        private static async void Process()
        {
            // we use the firstrun flag to publish our state without respecting the time elapsed/value change check
            // otherwise, the state might stay in 'unknown' in HA until the value changes
            var firstRun = true;
            var firstRunDone = false;

            while (_active)
            {
                try
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(750));

                    // are we paused?
                    if (_pause) continue;

                    // is mqtt available?
                    if (Variables.MqttManager.GetStatus() != MqttStatus.Connected)
                    {
                        _discoveryPublished = false;
                        continue;
                    }

                    // optionally flag as the first real run
                    if (!firstRunDone) firstRunDone = true;

                    // publish availability & autodiscovery every 30 sec
                    if ((DateTime.Now - _lastAutoDiscoPublish).TotalSeconds > 30)
                    {
                        await Variables.MqttManager.AnnounceAvailabilityAsync();

                        if (!_discoveryPublished)
                        {
                            if (SingleValueSensorsPresent())
                            {
                                foreach (var sensor in Variables.SingleValueSensors.TakeWhile(_ => !_pause).TakeWhile(_ => _active))
                                {
                                    if (_pause || Variables.MqttManager.GetStatus() != MqttStatus.Connected) continue;
                                    await sensor.PublishAutoDiscoveryConfigAsync();
                                }
                            }

                            if (MultiValueSensorsPresent())
                            {
                                foreach (var sensor in Variables.MultiValueSensors.TakeWhile(_ => !_pause).TakeWhile(_ => _active))
                                {
                                    if (_pause || Variables.MqttManager.GetStatus() != MqttStatus.Connected) continue;
                                    await sensor.PublishAutoDiscoveryConfigAsync();
                                }
                            }

                            _discoveryPublished = true;
                        }

                        _lastAutoDiscoPublish = DateTime.Now;
                    }

                    if (_pause) continue;

                    // publish sensor states (they have their own time-based scheduling)
                    if (SingleValueSensorsPresent())
                    {
                        foreach (var sensor in Variables.SingleValueSensors.TakeWhile(_ => !_pause).TakeWhile(_ => _active))
                        {
                            if (_pause || Variables.MqttManager.GetStatus() != MqttStatus.Connected) continue;
                            await sensor.PublishStateAsync(!firstRun);
                        }
                    }

                    // check if there are multivalue sensors
                    if (!MultiValueSensorsPresent()) continue;

                    foreach (var sensor in Variables.MultiValueSensors.TakeWhile(_ => !_pause).TakeWhile(_ => _active))
                    {
                        if (_pause || Variables.MqttManager.GetStatus() != MqttStatus.Connected) continue;
                        await sensor.PublishStatesAsync(!firstRun);
                    }
                }
                catch (Exception ex)
                {
                    Log.Fatal(ex, "[SENSORSMANAGER] Error while publishing: {err}", ex.Message);
                }
                finally
                {
                    // check if we need to take down the 'first run' flag
                    if (firstRunDone && firstRun) firstRun = false;
                }
            }
        }

        /// <summary>
        /// Resets all sensor checks (last sent and previous value), so they'll all be published again
        /// </summary>
        internal static void ResetAllSensorChecks()
        {
            try
            {
                // pause processing
                Pause();

                // reset single-value sensors
                if (SingleValueSensorsPresent())
                {
                    foreach (var sensor in Variables.SingleValueSensors) sensor.ResetChecks();
                }

                // reset multi-value sensors
                if (MultiValueSensorsPresent())
                {
                    foreach (var sensor in Variables.MultiValueSensors) sensor.ResetChecks();
                }

                // done
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "[SENSORSMANAGER] Error while resetting all sensor checks: {err}", ex.Message);
            }
            finally
            {
                // resume processing
                Resume();
            }
        }

        /// <summary>
        /// Stores the provided sensors, and (re)publishes them
        /// </summary>
        /// <param name="sensors"></param>
        /// <param name="toBeDeletedSensors"></param>
        /// <returns></returns>
        internal static async Task<bool> StoreAsync(List<ConfiguredSensor> sensors, List<ConfiguredSensor>? toBeDeletedSensors = null)
        {
            toBeDeletedSensors ??= new List<ConfiguredSensor>();

            try
            {
                // pause processing
                Pause();

                // process the to-be-removed
                if (toBeDeletedSensors.Any())
                {
                    foreach (var sensor in toBeDeletedSensors)
                    {
                        if (sensor.IsSingleValue())
                        {
                            var abstractSensor = StoredSensors.ConvertConfiguredToAbstractSingleValue(sensor);
                            if (abstractSensor == null)
                                continue;

                            var sensorIndex = Variables.SingleValueSensors.FindIndex(x => x.Id == abstractSensor.Id);
                            if (sensorIndex != -1)
                            {

                                // remove and unregister
                                await abstractSensor.UnPublishAutoDiscoveryConfigAsync();
                                Variables.SingleValueSensors.RemoveAt(sensorIndex);

                                Log.Information("[SENSORS] Removed single-value sensor: {sensor}", abstractSensor.EntityName);
                            }
                            else
                            {
                                Log.Information("[SENSORS] Single-value sensor not removed, not activated: {sensor}", abstractSensor.EntityName);
                            }
                        }
                        else
                        {
                            var abstractSensor = StoredSensors.ConvertConfiguredToAbstractMultiValue(sensor);
                            if (abstractSensor == null)
                                continue;

                            var sensorIndex = Variables.MultiValueSensors.FindIndex(x => x.Id == abstractSensor.Id);
                            if (sensorIndex != -1)
                            {
                                // remove and unregister
                                await abstractSensor.UnPublishAutoDiscoveryConfigAsync();
                                Variables.MultiValueSensors.RemoveAt(sensorIndex);

                                Log.Information("[SENSORS] Removed multi-value sensor: {sensor}", abstractSensor.EntityName);
                            }
                            else
                            {
                                Log.Information("[SENSORS] Multi-value sensor not removed, not activated: {sensor}", abstractSensor.EntityName);
                            }
                        }
                    }
                }

                // copy our list to the main one
                foreach (var sensor in sensors)
                {
                    if (sensor.IsSingleValue())
                    {
                        var abstractSensor = StoredSensors.ConvertConfiguredToAbstractSingleValue(sensor);
                        if (abstractSensor == null) continue;

                        if (Variables.SingleValueSensors.All(x => x.Id != abstractSensor.Id))
                        {
                            // new, add and register
                            Variables.SingleValueSensors.Add(abstractSensor);
                            await abstractSensor.PublishAutoDiscoveryConfigAsync();
                            await abstractSensor.PublishStateAsync(false);

                            Log.Information("[SENSORS] Added single-value sensor: {sensor}", abstractSensor.EntityName);
                            continue;
                        }

                        // existing, update and re-register
                        var currentSensorIndex = Variables.SingleValueSensors.FindIndex(x => x.Id == abstractSensor.Id);
                        if (Variables.SingleValueSensors[currentSensorIndex].EntityName != abstractSensor.EntityName)
                        {
                            // name changed, unregister
                            Log.Information("[SENSORS] Single-value sensor changed name, re-registering as new entity: {old} to {new}", Variables.SingleValueSensors[currentSensorIndex].EntityName, abstractSensor.EntityName);

                            await Variables.SingleValueSensors[currentSensorIndex].UnPublishAutoDiscoveryConfigAsync();
                        }

                        Variables.SingleValueSensors[currentSensorIndex] = abstractSensor;
                        await abstractSensor.PublishAutoDiscoveryConfigAsync();
                        await abstractSensor.PublishStateAsync(false);

                        Log.Information("[SENSORS] Modified single-value sensor: {sensor}", abstractSensor.EntityName);
                    }
                    else
                    {
                        var abstractSensor = StoredSensors.ConvertConfiguredToAbstractMultiValue(sensor);
                        if (abstractSensor == null) continue;

                        if (Variables.MultiValueSensors.All(x => x.Id != abstractSensor.Id))
                        {
                            // new, add and register
                            Variables.MultiValueSensors.Add(abstractSensor);
                            await abstractSensor.PublishAutoDiscoveryConfigAsync();
                            await abstractSensor.PublishStatesAsync(false);

                            Log.Information("[SENSORS] Added multi-value sensor: {sensor}", abstractSensor.EntityName);
                            continue;
                        }

                        // existing, update and re-register
                        var currentSensorIndex = Variables.MultiValueSensors.FindIndex(x => x.Id == abstractSensor.Id);
                        if (Variables.MultiValueSensors[currentSensorIndex].EntityName != abstractSensor.EntityName)
                        {
                            // name changed, unregister
                            Log.Information("[SENSORS] Multi-value sensor changed name, re-registering as new entity: {old} to {new}", Variables.MultiValueSensors[currentSensorIndex].EntityName, abstractSensor.EntityName);

                            await Variables.MultiValueSensors[currentSensorIndex].UnPublishAutoDiscoveryConfigAsync();
                        }

                        Variables.MultiValueSensors[currentSensorIndex] = abstractSensor;
                        await abstractSensor.PublishAutoDiscoveryConfigAsync();
                        await abstractSensor.PublishStatesAsync(false);

                        Log.Information("[SENSORS] Modified multi-value sensor: {sensor}", abstractSensor.EntityName);
                    }
                }

                // annouce ourselves
                await Variables.MqttManager.AnnounceAvailabilityAsync();

                // store to file
                StoredSensors.Store();

                // done
                return true;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "[SENSORSMANAGER] Error while storing: {err}", ex.Message);
                return false;
            }
            finally
            {
                // resume processing
                Resume();
            }
        }

        /// <summary>
        /// Processes, stores and activates the received sensors
        /// </summary>
        /// <param name="sensors"></param>
        internal static void ProcessReceivedSensors(List<ConfiguredSensor> sensors)
        {
            try
            {
                if (!sensors.Any())
                {
                    Log.Warning("[SENSORSMANAGER] Received empty list, clearing all sensors");
                    _ = SettingsManager.ClearAllSensorsAsync();
                    return;
                }

                // collect all current sensors (to determine which are deleted)
                var currentSensors = Variables.SingleValueSensors.Select(StoredSensors.ConvertAbstractSingleValueToConfigured).ToList();
                currentSensors.AddRange(Variables.MultiValueSensors.Select(StoredSensors.ConvertAbstractMultiValueToConfigured).Where(mvSensor => mvSensor != null)!);

                // create the to-be-deleted list
                var toBeDeletedSensors = currentSensors.Where(sensor => sensors.All(x => x.Id != sensor.Id)).ToList();

                // log what we're doing
                Log.Information("[SENSORSMANAGER] Processing {count} received sensor(s), deleting {del} sensor(s) ..", sensors.Count, toBeDeletedSensors.Count);

                // process both lists
                _ = StoreAsync(sensors, toBeDeletedSensors);
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "[SENSORSMANAGER] Error while processing received: {err}", ex.Message);
            }
        }

        private static bool SingleValueSensorsPresent() => Variables.SingleValueSensors.Any();
        private static bool MultiValueSensorsPresent() => Variables.MultiValueSensors.Any();
    }
}
