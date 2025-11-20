using System.Threading.Tasks;
using HASS.Agent.Shared.Enums;
using HASS.Agent.Shared.HomeAssistant;
using HASS.Agent.Shared.HomeAssistant.Commands;
using HASS.Agent.Shared.Models.HomeAssistant;
using MQTTnet;

namespace HASS.Agent.Shared.Mqtt
{
    /// <summary>
    /// HASS.Agent MQTT Managers interface
    /// </summary>
    public interface IMqttManager
    {
        bool IsConnected();
        bool IsReady();
        void Initialize();
        void CreateDeviceConfigModel();
        Task<bool> PublishAsync(MqttApplicationMessage message);
        Task AnnounceAutoDiscoveryConfigAsync(AbstractDiscoverable discoverable, string domain, bool clearConfig = false, bool migration = false);
        MqttStatus GetStatus();
        Task AnnounceAvailabilityAsync(bool offline = false);
        Task ClearDeviceConfigAsync();
        void Disconnect();
        Task SubscribeAsync(AbstractCommand command);
        Task UnsubscribeAsync(AbstractCommand command);

        Task SubscribeNotificationsAsync();
        
        string MqttDiscoveryPrefix();
        DeviceConfigModel GetDeviceConfigModel();
        void ReloadConfiguration();
        bool UseRetainFlag();
        Task SubscribeMediaCommandsAsync();
    }
}
