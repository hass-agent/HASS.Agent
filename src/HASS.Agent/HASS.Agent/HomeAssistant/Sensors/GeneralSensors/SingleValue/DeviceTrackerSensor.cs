#nullable enable
using System.Runtime.Caching;
using HASS.Agent.Shared.Extensions;
using HASS.Agent.Shared.Models.HomeAssistant;
using Windows.Devices.Geolocation;
using HASS.Agent.Models.Internal;
using Newtonsoft.Json;

namespace HASS.Agent.HomeAssistant.Sensors.GeneralSensors.SingleValue
{
    /// <summary>
    /// Sensor containing the coördinates of the device
    /// XXXX Track location change event, and make non breaking
    /// </summary>
    public class DeviceTrackerSensor : AbstractSingleValueSensor
    {
        private const string DefaultName = "devicetracksensor";

        public DeviceTrackerSensor(int? updateInterval = 10, string entityName = DefaultName, string name = DefaultName,
            string id = default, string advancedSettings = default) : base(entityName ?? DefaultName, name ?? null,
            updateInterval ?? 30, id, advancedSettings: advancedSettings)
        {
            UseAttributes = true;
        }

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
                Json_attributes_topic = $"{Variables.MqttManager.MqttDiscoveryPrefix()}/{Domain}/{deviceConfig.Name}/{ObjectId}/attributes",
            });
        }

        public override string GetState()
        {
            var state = "ON";
            return state;
        }

        public override string? GetAttributes()
        {
            ObjectCache cache = System.Runtime.Caching.MemoryCache.Default;
            GeolocationInfo? fileContents = cache["location_" + this.Id] as GeolocationInfo;
            string jsonPayload = "";

            if (fileContents != null)
            {
                jsonPayload = JsonConvert.SerializeObject(fileContents, Formatting.Indented);
                return jsonPayload;
            }

            GeolocationInfo gli = new GeolocationInfo();

            var accessStatus = Geolocator.RequestAccessAsync().GetAwaiter().GetResult();
            switch (accessStatus)
            {
                case GeolocationAccessStatus.Allowed:
                    // https://community.home-assistant.io/t/attributes-latitude-and-longitude-in-a-lovelace-map/318760
                    var geolocator = new Geolocator();

                    var position = geolocator.GetGeopositionAsync().GetAwaiter().GetResult();
                    var lat = position.Coordinate.Latitude.ConvertToStringDotDecimalSeperator();
                    var lon = position.Coordinate.Longitude.ConvertToStringDotDecimalSeperator();
                    var alt = position.Coordinate.Altitude?.ConvertToStringDotDecimalSeperator();
                    var accuracy = position.Coordinate.Accuracy.ConvertToStringDotDecimalSeperator();
                    var sourceType = position.Coordinate.PositionSource;

                    gli = new GeolocationInfo(lon, lat, sourceType)
                    {
                        altitude = alt,
                        gps_accuracy = accuracy,
                        not_permitted = "false"
                    };

                    cache.Set("location_" + Id, gli, DateTimeOffset.Now.AddSeconds(UpdateIntervalSeconds - 1));
                    break;

                case GeolocationAccessStatus.Denied:
                    // notify user: Access to location is denied
                    gli.not_permitted = "true - enable location access";
                    break;

                case GeolocationAccessStatus.Unspecified:
                    // notify user: Unspecified error
                    gli.not_permitted = "true";
                    break;
            }

            jsonPayload = JsonConvert.SerializeObject(gli, Formatting.Indented);
            return jsonPayload;
        }
    }
}
