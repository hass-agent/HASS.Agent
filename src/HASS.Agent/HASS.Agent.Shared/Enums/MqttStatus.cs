namespace HASS.Agent.Shared.Enums
{
    /// <summary>
    /// Contains various MQTT MAnager statusses
    /// </summary>
    public enum MqttStatus
    {
        ConfigMissing,
        Connected,
        Connecting,
        Disconnected,
        Error
    }
}