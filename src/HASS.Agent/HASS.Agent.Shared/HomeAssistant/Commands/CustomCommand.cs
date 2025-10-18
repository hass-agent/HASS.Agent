using System.Diagnostics;
using HASS.Agent.Shared.Enums;
using HASS.Agent.Shared.Managers;
using HASS.Agent.Shared.Models.HomeAssistant;
using Serilog;

namespace HASS.Agent.Shared.HomeAssistant.Commands
{
    /// <summary>
    /// Command to perform an action through a console, either normal or with low integrity
    /// </summary>
    public class CustomCommand : AbstractCommand
    {
        private const string DefaultName = "custom";
        
        public string Command { get; protected set; }
        public string State { get; protected set; }
        public bool RunAsLowIntegrity { get; protected set; }
        public Process Process { get; set; } = null;

        public CustomCommand(string command, bool runAsLowIntegrity, string entityName = DefaultName, string name = DefaultName, CommandEntityType entityType = CommandEntityType.Switch, string id = default) : base(entityName ?? DefaultName, name ?? null, entityType, id)
        {
            Command = command;
            RunAsLowIntegrity = runAsLowIntegrity;
            State = "OFF";
        }

        public override void TurnOn()
        {
            State = "ON";

            if (string.IsNullOrWhiteSpace(Command))
            {
                Log.Warning("[CUSTOMCOMMAND] [{name}] Unable to launch command, it's configured as action-only", EntityName);
                State = "OFF";
                return;
            }

            if (RunAsLowIntegrity) CommandLineManager.LaunchAsLowIntegrity(Command);
            else
            {
                var executed = CommandLineManager.ExecuteHeadless(Command);

                if (!executed) Log.Error("[CUSTOMCOMMAND] [{name}] Launching command failed", EntityName);
            }

            State = "OFF";
        }

        public override void TurnOnWithAction(string action)
        {
            State = "ON";

            // prepare command
            var command = string.IsNullOrWhiteSpace(Command) ? action : $"{Command} {action}";

            if (RunAsLowIntegrity) CommandLineManager.LaunchAsLowIntegrity(command);
            else
            {
                var executed = !string.IsNullOrWhiteSpace(Command)
                    ? CommandLineManager.Execute(Command, action)
                    : CommandLineManager.ExecuteHeadless(action);

                if (!executed) Log.Error("[CUSTOMCOMMAND] [{name}] Launching command with action '{action}' failed", EntityName, action);
            }

            State = "OFF";
        }

        public override DiscoveryConfigModel GetAutoDiscoveryConfig()
        {
            if (Variables.MqttManager == null) return null;

            var deviceConfig = Variables.MqttManager.GetDeviceConfigModel();
            if (deviceConfig == null) return null;

            return new CommandDiscoveryConfigModel(Domain)
            {
                EntityName = EntityName,
                Name = Name,
                Unique_id = Id,
                Availability_topic = $"{Variables.MqttManager.MqttDiscoveryPrefix()}/sensor/{deviceConfig.Name}/availability",
                Command_topic = $"{Variables.MqttManager.MqttDiscoveryPrefix()}/{Domain}/{deviceConfig.Name}/{ObjectId}/set",
                Action_topic = $"{Variables.MqttManager.MqttDiscoveryPrefix()}/{Domain}/{deviceConfig.Name}/{ObjectId}/action",
                State_topic = $"{Variables.MqttManager.MqttDiscoveryPrefix()}/{Domain}/{deviceConfig.Name}/{ObjectId}/state",
                Device = deviceConfig,
            };
        }

        public override string GetState() => State;

        public override void TurnOff() => Process?.Kill();
    }
}
