using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using System.Windows.Threading;
using HASS.Agent.Managers;
using HASS.Agent.Resources.Localization;
using HASS.Agent.Shared.Models.HomeAssistant;
using Microsoft.Win32;
using Newtonsoft.Json;
using Windows.Foundation.Metadata;
using WindowsDesktop;
using static HASS.Agent.Functions.NativeMethods;

namespace HASS.Agent.Shared.HomeAssistant.Sensors.GeneralSensors.SingleValue
{
    /// <summary>
    /// Sensor containing the ID of the currently active virtual desktop
    /// Additionally returns all available virtual desktops and their names (if named)
    /// </summary>
    public class ActiveDesktopSensor : AbstractSingleValueSensor
    {
        private const string _defaultName = "activedesktop";

        private string _desktopId = string.Empty;
        private string _attributes = string.Empty;

        public ActiveDesktopSensor(int? updateInterval = null, string name = _defaultName, string friendlyName = _defaultName, string id = default) : base(name ?? _defaultName, friendlyName ?? null, updateInterval ?? 15, id)
        {
            UseAttributes = true;
        }

        public override DiscoveryConfigModel GetAutoDiscoveryConfig()
        {
            if (Variables.MqttManager == null)
            {
                return null;
            }

            var deviceConfig = Variables.MqttManager.GetDeviceConfigModel();
            if (deviceConfig == null)
            {
                return null;
            }

            return AutoDiscoveryConfigModel ?? SetAutoDiscoveryConfigModel(new SensorDiscoveryConfigModel()
            {
                Name = Name,
                FriendlyName = FriendlyName,
                Unique_id = Id,
                Device = deviceConfig,
                State_topic = $"{Variables.MqttManager.MqttDiscoveryPrefix()}/{Domain}/{deviceConfig.Name}/{ObjectId}/state",
                Icon = "mdi:monitor",
                Availability_topic = $"{Variables.MqttManager.MqttDiscoveryPrefix()}/{Domain}/{deviceConfig.Name}/availability",
                Json_attributes_topic = $"{Variables.MqttManager.MqttDiscoveryPrefix()}/{Domain}/{deviceConfig.Name}/{ObjectId}/attributes"
            });
        }

        public override string GetState()
        {
            var currentDesktop = VirtualDesktopManager.GetCurrentDesktop();
            _desktopId = currentDesktop.Id.ToString();

            _attributes = JsonConvert.SerializeObject(new
            {
                desktopName = currentDesktop.Name,
                availableDesktops = VirtualDesktopManager.GetAllDesktopsInfo()
            }, Formatting.Indented);

            return _desktopId;
        }

        public override string GetAttributes() => _attributes;


    }
}
