using HASS.Agent.Managers;
using HASS.Agent.Models.Internal;
using HASS.Agent.Shared.Models.HomeAssistant;
using Newtonsoft.Json;

namespace HASS.Agent.HomeAssistant.Sensors.GeneralSensors.SingleValue
{
    /// <summary>
    /// Sensor containing the number of present serial (COM) ports, with the ports as attributes.
    /// Gets published immediately when ports are added or removed.
    /// </summary>
    public class SerialPortsSensor : AbstractSingleValueSensor
    {
        private const string DefaultName = "serialports";
        private string _attributes = "{}";

        public SerialPortsSensor(int? updateInterval = 300, string entityName = DefaultName, string name = DefaultName, string id = default, string advancedSettings = default) : base(entityName ?? DefaultName, name ?? null, updateInterval ?? 300, id, advancedSettings: advancedSettings)
        {
            UseAttributes = true;
        }

        public override DiscoveryConfigModel GetAutoDiscoveryConfig()
        {
            if (Variables.MqttManager == null) return null;

            var deviceConfig = Variables.MqttManager.GetDeviceConfigModel();
            if (deviceConfig == null) return null;

            return AutoDiscoveryConfigModel ?? SetAutoDiscoveryConfigModel(new SensorDiscoveryConfigModel(Domain)
            {
                EntityName = EntityName,
                Name = Name,
                Unique_id = Id,
                Device = deviceConfig,
                State_topic = $"{Variables.MqttManager.MqttDiscoveryPrefix()}/{Domain}/{deviceConfig.Name}/{ObjectId}/state",
                State_class = "measurement",
                Icon = "mdi:serial-port",
                Availability_topic = $"{Variables.MqttManager.MqttDiscoveryPrefix()}/{Domain}/{deviceConfig.Name}/availability",
                Json_attributes_topic = $"{Variables.MqttManager.MqttDiscoveryPrefix()}/{Domain}/{deviceConfig.Name}/{ObjectId}/attributes"
            });
        }

        public override string GetState()
        {
            var ports = SerialPortManager.GetPorts();

            _attributes = JsonConvert.SerializeObject(new SerialPortInfoCollection(ports), Formatting.Indented);
            return ports.Count.ToString();
        }

        public override string GetAttributes() => _attributes;
    }
}
