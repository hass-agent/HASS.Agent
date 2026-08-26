using System.Collections.Generic;
using HASS.Agent.Shared.HomeAssistant.Sensors.MediaActivity;
using HASS.Agent.Shared.Models.HomeAssistant;
using Newtonsoft.Json;

namespace HASS.Agent.Shared.HomeAssistant.Sensors.GeneralSensors.SingleValue
{
    /// <summary>
    /// Sensor indicating whether the webcam is in use
    /// </summary>
    public class WebcamProcessSensor : AbstractSingleValueSensor
    {
        private const string DefaultName = "webcamprocess";
        private readonly IMediaActivityProvider _mediaActivityProvider;

        public WebcamProcessSensor(int? updateInterval = null, string entityName = DefaultName, string name = DefaultName, string id = default, string advancedSettings = default)
            : this(MediaActivityProvider.Instance, updateInterval, entityName, name, id, advancedSettings)
        {
        }

        internal WebcamProcessSensor(IMediaActivityProvider mediaActivityProvider, int? updateInterval = null, string entityName = DefaultName, string name = DefaultName, string id = default, string advancedSettings = default)
            : base(entityName ?? DefaultName, name ?? null, updateInterval ?? 10, id, true, advancedSettings: advancedSettings)
        {
            _mediaActivityProvider = mediaActivityProvider;
        }

        private readonly Dictionary<string, string> _processes = new Dictionary<string, string>();

        private string _attributes = string.Empty;

        public override string GetState() => WebcamProcess();
        public void SetAttributes(string value) => _attributes = string.IsNullOrWhiteSpace(value) ? "{}" : value;
        public override string GetAttributes() => _attributes;

        public override DiscoveryConfigModel GetAutoDiscoveryConfig()
        {
            if (Variables.MqttManager == null) return null;

            var deviceConfig = Variables.MqttManager.GetDeviceConfigModel();
            if (deviceConfig == null) return null;

            var model = new SensorDiscoveryConfigModel(Domain)
            {
                EntityName = EntityName,
                Name = Name,
                Unique_id = Id,
                Device = deviceConfig,
                State_topic = $"{Variables.MqttManager.MqttDiscoveryPrefix()}/{Domain}/{deviceConfig.Name}/{ObjectId}/state",
                State_class = "measurement",
                Availability_topic = $"{Variables.MqttManager.MqttDiscoveryPrefix()}/sensor/{deviceConfig.Name}/availability",
                Icon = "mdi:webcam"
            };

            if (UseAttributes)
            {
                model.Json_attributes_topic = $"{Variables.MqttManager.MqttDiscoveryPrefix()}/{Domain}/{deviceConfig.Name}/{ObjectId}/attributes";
            }

            return AutoDiscoveryConfigModel ?? SetAutoDiscoveryConfigModel(model);
        }

        private string WebcamProcess()
        {
            var snapshot = _mediaActivityProvider.GetActivity(MediaActivityKind.Webcam);
            _processes.Clear();

            foreach (var process in snapshot.Processes)
            {
                _processes[process.Name] = "on";
            }

            _attributes = _processes.Count > 0
                ? JsonConvert.SerializeObject(_processes, Formatting.Indented)
                : "{}";
            return _processes.Count.ToString();
        }

    }
}
