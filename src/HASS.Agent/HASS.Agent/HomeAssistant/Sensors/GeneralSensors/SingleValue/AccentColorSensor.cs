using HASS.Agent.Managers;
using HASS.Agent.Shared.Extensions;
using HASS.Agent.Shared.Models.HomeAssistant;
using Newtonsoft.Json;
using Windows.UI.ViewManagement;
using static HASS.Agent.Shared.Functions.Inputs;

namespace HASS.Agent.HomeAssistant.Sensors.GeneralSensors.SingleValue
{
    /// <summary>
    /// Sensor containing the current monitor power state
    /// </summary>
    public class AccentColorSensor : AbstractSingleValueSensor
    {
        private const string DefaultName = "accentcolor";

        private readonly UISettings _uiSettings = new();

        private string _attributesJson = string.Empty;

        public AccentColorSensor(int? updateInterval = 120, string entityName = DefaultName, string name = DefaultName, string id = default, string advancedSettings = default) : base(entityName ?? DefaultName, name ?? null, updateInterval ?? 120, id, true, advancedSettings) { }

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
                Icon = "mdi:palette",
                Availability_topic = $"{Variables.MqttManager.MqttDiscoveryPrefix()}/{Domain}/{deviceConfig.Name}/availability",
                Json_attributes_topic = $"{Variables.MqttManager.MqttDiscoveryPrefix()}/{Domain}/{deviceConfig.Name}/{ObjectId}/attributes"
            });
        }

        public override string GetState()
        {
            var accent = TryGetColor(UIColorType.Accent);
            _attributesJson = JsonConvert.SerializeObject(new
            {
                accent,
                background = TryGetColor(UIColorType.Background),
                foreground = TryGetColor(UIColorType.Foreground),
                accentDark3 = TryGetColor(UIColorType.AccentDark3),
                accentDark2 = TryGetColor(UIColorType.AccentDark2),
                accentDark1 = TryGetColor(UIColorType.AccentDark1),
                accentLight3 = TryGetColor(UIColorType.AccentLight3),
                accentLight2 = TryGetColor(UIColorType.AccentLight2),
                accentLight1 = TryGetColor(UIColorType.AccentLight1),
                complement = TryGetColor(UIColorType.Complement),
            });

            return accent;
        }

        public override string GetAttributes() => _attributesJson;

        private string TryGetColor(UIColorType colorType)
        {
            var color = "";

            try
            {
                color = _uiSettings.GetColorValue(colorType).ToString().Replace("#FF", "#");
            }
            catch {}

            return color;
        }
    }
}
