namespace HASS.Agent.Shared.Models.Config.Service
{
    public class ServiceMqttSettings
    {
        public ServiceMqttSettings()
        {
            //
        }

        public string MqttAddress { get; set; } = "homeassistant.local";
        public int MqttPort { get; set; } = 1883;
        public bool MqttUseWebSocket { get; set; } = false;
        public bool MqttUseTls { get; set; }
        public bool MqttAllowUntrustedCertificates { get; set; } = true;
        public string MqttUsername { get; set; } = string.Empty;
        public string MqttPassword { get; set; } = string.Empty;
        public string MqttDiscoveryPrefix { get; set; } = "homeassistant";
        public string MqttClientId { get; set; } = string.Empty;
        public bool MqttUseRetainFlag { get; set; } = true;
        public string MqttRootCertificate { get; set; } = string.Empty;
        public string MqttClientCertificate { get; set; } = string.Empty;
    }
}
