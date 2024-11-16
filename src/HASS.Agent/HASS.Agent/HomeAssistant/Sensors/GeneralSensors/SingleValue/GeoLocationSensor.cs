using HASS.Agent.Shared.Extensions;
using HASS.Agent.Shared.Models.HomeAssistant;

namespace HASS.Agent.HomeAssistant.Sensors.GeneralSensors.SingleValue
{
    /// <summary>
    /// Sensor containing the coördinates of the device
    /// </summary>
    public class GeoLocationSensor : AbstractSingleValueSensor
    {
        private const string DefaultName = "geolocation";
        public GeoLocationSensor(int? updateInterval = 10, string entityName = DefaultName, string name = DefaultName, string id = default, string advancedSettings = default) : base(entityName ?? DefaultName, name ?? null, updateInterval ?? 30, id, advancedSettings: advancedSettings) { }

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
                Icon = "mdi:earth",
                Availability_topic = $"{Variables.MqttManager.MqttDiscoveryPrefix()}/{Domain}/{deviceConfig.Name}/availability"
            });
        }

        public override string GetState()
        {
            var locator = new Windows.Devices.Geolocation.Geolocator();
            var location = locator.GetGeopositionAsync().GetAwaiter().GetResult();
            var position = location.Coordinate.Point.Position;

            var lat = position.Latitude.ConvertToStringDotDecimalSeperator();
            var lon = position.Longitude.ConvertToStringDotDecimalSeperator();
            var alt = position.Altitude.ConvertToStringDotDecimalSeperator();

            return $"{lat},{lon},{alt}";
        }

        public override string GetAttributes() => string.Empty;
    }
}
