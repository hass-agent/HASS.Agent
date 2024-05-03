using System;
using System.Globalization;
using CoreAudio;
using HASS.Agent.Shared.Models.HomeAssistant;

namespace HASS.Agent.Shared.HomeAssistant.Sensors.GeneralSensors.SingleValue
{
    /// <summary>
    /// Sensor containing the volume level of the default audio endpoint
    /// </summary>
    public class CurrentVolumeSensor : AbstractSingleValueSensor
    {
        private const string DefaultName = "currentvolume";

        public CurrentVolumeSensor(int? updateInterval = null, string entityName = DefaultName, string name = DefaultName, string id = default, string advancedSettings = default) : base(entityName ?? DefaultName, name ?? null, updateInterval ?? 15, id, advancedSettings: advancedSettings) { }

        public override DiscoveryConfigModel GetAutoDiscoveryConfig()
        {
            if (Variables.MqttManager == null) return null;

            var deviceConfig = Variables.MqttManager.GetDeviceConfigModel();
            if (deviceConfig == null) return null;

            return AutoDiscoveryConfigModel ?? SetAutoDiscoveryConfigModel(new SensorDiscoveryConfigModel()
            {
                EntityName = EntityName,
                Name = Name,
                Unique_id = Id,
                Device = deviceConfig,
                State_topic = $"{Variables.MqttManager.MqttDiscoveryPrefix()}/{Domain}/{deviceConfig.Name}/{ObjectId}/state",
                Icon = "mdi:volume-medium",
                Unit_of_measurement = "%",
                State_class = "measurement",
                Availability_topic = $"{Variables.MqttManager.MqttDiscoveryPrefix()}/{Domain}/{deviceConfig.Name}/availability"
            });
        }

        public override string GetState()
        {
            using var audioDevice = Variables.AudioDeviceEnumerator?.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
            // check for null & mute
            if (audioDevice?.AudioEndpointVolume == null) return "0";
            if (audioDevice.AudioEndpointVolume.Mute) return "0";

            // return as percentage
            return Math.Round(audioDevice.AudioEndpointVolume.MasterVolumeLevelScalar * 100, 0).ToString(CultureInfo.InvariantCulture);
        }

        public override string GetAttributes() => string.Empty;
    }
}
