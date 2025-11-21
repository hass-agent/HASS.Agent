using HASS.Agent.Shared.Enums;
using HASS.Agent.Shared.Managers.VoiceMeeterAudio;
using HASS.Agent.Shared.Models.HomeAssistant;
using Serilog;
using System.Diagnostics;

namespace HASS.Agent.Shared.HomeAssistant.Commands
{
    /// <summary>
    /// Command to perform an action or script through Voicemeeter
    /// </summary>
    public class VoicemeeterCommand : AbstractCommand
    {
        private const string DefaultName = "Voicemeeter";

        public string Command { get; protected set; }
        public string State { get; protected set; }
        public Process Process { get; set; }

        public VoicemeeterCommand(string command, string entityName = DefaultName, string name = DefaultName, CommandEntityType entityType = CommandEntityType.Switch, string id = default) : base(entityName ?? DefaultName, name ?? null, entityType, id)
        {
            Command = command;
            State = "OFF";
        }

        public override void TurnOn()
        {
            State = "ON";

            if (string.IsNullOrWhiteSpace(Command))
            {
                Log.Warning("[Voicemeeter] [{name}] Unable to execute, it's configured as action-only", EntityName);

                State = "OFF";
                return;
            }

            VoiceMeeterAudioManager.SetParameters(Command);

            State = "OFF";
        }

        public override void TurnOff()
        {
            State = "OFF";
        }

        public override void TurnOnWithAction(string action)
        {
            State = "ON";
            VoiceMeeterAudioManager.SetParameters(action);
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
    }
}
