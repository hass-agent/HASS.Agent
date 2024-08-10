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
    /// </summary>
    public class GeoLocationSensor : AbstractSingleValueSensor
    {
        private const string DefaultName = "geolocation";
        private string state = "OFF";

        public GeoLocationSensor(int? updateInterval = 10, string name = DefaultName, string friendlyName = DefaultName,
            string id = default) : base(name ?? DefaultName, friendlyName ?? null, updateInterval ?? 30, id)
        {
            Domain = "device_tracker";
            UseAttributes = true;

        }

        private string _attributes;

        public override DiscoveryConfigModel GetAutoDiscoveryConfig()
        {
            if (Variables.MqttManager == null) return null;

            var deviceConfig = Variables.MqttManager.GetDeviceConfigModel();
            if (deviceConfig == null) return null;
            //WIP
            Domain = "device_tracker";
            return AutoDiscoveryConfigModel ?? SetAutoDiscoveryConfigModel(new SensorDiscoveryConfigModel()
            {
                EntityName = Name,
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
            var state = "OFF";
            state = "ON";

            return state;
        }

        public void SetAttributes(string value) => _attributes = string.IsNullOrWhiteSpace(value) ? "{}" : value;
        //public override string GetAttributes() => _attributes;

        
        public override string? GetAttributes()
        {
            ObjectCache cache = MemoryCache.Default;
            GeolocationInfo fileContents = cache["location_" + this.Id] as GeolocationInfo;
            var state = "OFF";

            if (fileContents != null)
            {
                string jsonPayload = JsonConvert.SerializeObject(fileContents, Formatting.Indented);
                SetAttributes(jsonPayload);
                state = "ON";
                return jsonPayload;
            }
            else
            {
                Geolocator geolocator = null;
                Geoposition position;
                GeolocationInfo gli = new GeolocationInfo();

                var accessStatus = Geolocator.RequestAccessAsync().GetAwaiter().GetResult();
                switch (accessStatus)
                {
                    case GeolocationAccessStatus.Allowed:
                        // notify user: Waiting for update

                        //https://community.home-assistant.io/t/attributes-latitude-and-longitude-in-a-lovelace-map/318760
                        geolocator = new Geolocator();
                        position = geolocator.GetGeopositionAsync().GetAwaiter().GetResult();
                        var lat = position.Coordinate.Latitude.ConvertToStringDotDecimalSeperator();
                        var lon = position.Coordinate.Longitude.ConvertToStringDotDecimalSeperator();
                        var alt = position.Coordinate.Altitude?.ConvertToStringDotDecimalSeperator();
                        var accuracy = position.Coordinate.Accuracy.ConvertToStringDotDecimalSeperator();
                        var sourceType = position.Coordinate.PositionSource;

                        gli = new GeolocationInfo(lon, lat, sourceType);
                        gli.gps_accuracy = accuracy;
                        gli.not_permitted = "false";

                        cache.Set("location_" + this.Id, gli, DateTimeOffset.Now.AddSeconds(120));

                        state = "ON";

                        break;

                    case GeolocationAccessStatus.Denied:
                        // notify user: Access to location is denied
                        gli.not_permitted = "true";

                        break;

                    case GeolocationAccessStatus.Unspecified:
                        // notify user: Unspecified error
                        gli.not_permitted = "true";

                        break;
                }

                string json_payload = JsonConvert.SerializeObject(gli, Formatting.Indented);
                SetAttributes(json_payload);
                return json_payload;
            }
        }
    }
}
