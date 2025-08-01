using HASS.Agent.Shared.Models.Config.Service;

namespace HASS.Agent.Satellite.Service.Extensions
{
    public static class ServiceMqttSettingsExtensions
    {
        /// <summary>
        /// Returns whether the provided MQTT settings differ from the current settings
        /// </summary>
        /// <param name="newSettings"></param>
        /// <param name="currentSettings"></param>
        /// <returns></returns>
        public static bool SettingsChanged(this ServiceMqttSettings newSettings, ServiceMqttSettings currentSettings)
        {
            if (newSettings.MqttAddress != currentSettings.MqttAddress) return true;
            if (newSettings.MqttPort != currentSettings.MqttPort) return true;
            if (newSettings.MqttUseTls != currentSettings.MqttUseTls) return true;
            if (newSettings.MqttAllowUntrustedCertificates != currentSettings.MqttAllowUntrustedCertificates) return true;
            if (newSettings.MqttUsername != currentSettings.MqttUsername) return true;
            if (newSettings.MqttPassword != currentSettings.MqttPassword) return true;
            if (newSettings.MqttDiscoveryPrefix != currentSettings.MqttDiscoveryPrefix) return true;
            if (newSettings.MqttUseRetainFlag != currentSettings.MqttUseRetainFlag) return true;
            if (newSettings.MqttRootCertificate != currentSettings.MqttRootCertificate) return true;
            if (newSettings.MqttClientCertificate != currentSettings.MqttClientCertificate) return true;

            return false;
        }
    }
}
