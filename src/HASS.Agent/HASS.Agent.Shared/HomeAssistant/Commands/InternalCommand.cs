using HASS.Agent.Shared.Enums;
using HASS.Agent.Shared.Models.HomeAssistant;

namespace HASS.Agent.Shared.HomeAssistant.Commands
{
    /// <summary>
    /// Internal commands (executed within the HASS.Agent platform)
    /// </summary>
    public class InternalCommand : AbstractCommand
    {
        private const string DefaultName = "internal";

        public string State { get; protected set; }
        public string CommandConfig { get; protected set; }

        public InternalCommand(string entityName = DefaultName, string name = DefaultName, string commandConfig = "", CommandEntityType entityType = CommandEntityType.Switch, string id = default) : base(entityName ?? DefaultName, name ?? null, entityType, id)
        {
            State = "OFF";
            CommandConfig = commandConfig;
        }

        public override void TurnOn()
        {
            State = "ON";

            // to be overriden

            State = "OFF";
        }

        public override void TurnOnWithAction(string action)
        {
            State = "ON";

            // to be overriden

            State = "OFF";
        }

        public override DiscoveryConfigModel GetAutoDiscoveryConfig()
        {
            if (Variables.MqttManager == null) return null;

            var deviceConfig = Variables.MqttManager.GetDeviceConfigModel();
            if (deviceConfig == null) return null;

            return new CommandDiscoveryConfigModel()
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

        public override void TurnOff()
        {
            //
        }
    }
}
