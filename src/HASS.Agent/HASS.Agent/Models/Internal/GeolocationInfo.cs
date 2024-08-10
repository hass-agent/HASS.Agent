using Windows.Devices.Geolocation;

namespace HASS.Agent.Models.Internal
{
    public class GeolocationInfo
    {
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string source_type { get; set; }
        public string gps_accuracy { get; set; }
        public string not_permitted { get; set; }

        public GeolocationInfo()
        {

        }

        public GeolocationInfo(string lon, string lat, PositionSource source)
        {
            longitude = lon;
            latitude = lat;
            source_type = "";
            gps_accuracy = "1.2";

            switch (source )
            {
                case PositionSource.Satellite:
                    source_type = "gps";
                    break;
                case PositionSource.WiFi:
                    source_type = "router";
                    break;
            }
        }
    }
}
    