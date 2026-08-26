using HASS.Agent.Shared.HomeAssistant.Sensors.MediaActivity;
using HASS.Agent.Shared.Models.HomeAssistant;

namespace HASS.Agent.Shared.HomeAssistant.Sensors.GeneralSensors.SingleValue
{
    /// <summary>
    /// Sensor indicating whether the mic is in use
    /// </summary>
    public class MicrophoneActiveSensor : AbstractSingleValueSensor
    {
        private const string DefaultName = "microphoneactive";
        private readonly IMediaActivityProvider _mediaActivityProvider;

        public MicrophoneActiveSensor(int? updateInterval = null, string entityName = DefaultName, string name = DefaultName, string id = default, string advancedSettings = default)
            : this(MediaActivityProvider.Instance, updateInterval, entityName, name, id, advancedSettings)
        {
        }

        internal MicrophoneActiveSensor(IMediaActivityProvider mediaActivityProvider, int? updateInterval = null, string entityName = DefaultName, string name = DefaultName, string id = default, string advancedSettings = default)
            : base(entityName ?? DefaultName, name ?? null, updateInterval ?? 10, id, advancedSettings: advancedSettings)
        {
            _mediaActivityProvider = mediaActivityProvider;
            Domain = "binary_sensor";
        }

        public override string GetState() => _mediaActivityProvider.GetActivity(MediaActivityKind.Microphone).IsActive ? "ON" : "OFF";

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
                Availability_topic = $"{Variables.MqttManager.MqttDiscoveryPrefix()}/sensor/{deviceConfig.Name}/availability",
                Icon = "mdi:microphone"
            });
        }

        public override string GetAttributes() => string.Empty;

    }
}
