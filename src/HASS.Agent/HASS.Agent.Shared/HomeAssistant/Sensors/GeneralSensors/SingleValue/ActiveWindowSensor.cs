using System;
using System.Runtime.InteropServices;
using System.Text;
using HASS.Agent.Shared.Models.HomeAssistant;

namespace HASS.Agent.Shared.HomeAssistant.Sensors.GeneralSensors.SingleValue
{
    /// <summary>
    /// Sensor containing the title of the current active window
    /// </summary>
    public class ActiveWindowSensor : AbstractSingleValueSensor
    {
        private const string DefaultName = "activewindow";

        public ActiveWindowSensor(int? updateInterval = null, string entityName = DefaultName, string name = DefaultName, string id = default) : base(entityName ?? DefaultName, name ?? null, updateInterval ?? 15, id) { }

        public override DiscoveryConfigModel GetAutoDiscoveryConfig()
        {
            if (Variables.MqttManager == null)
                return null;

            var deviceConfig = Variables.MqttManager.GetDeviceConfigModel();
            if (deviceConfig == null)
                return null;

            return AutoDiscoveryConfigModel ?? SetAutoDiscoveryConfigModel(new SensorDiscoveryConfigModel()
            {
                EntityName = EntityName,
                Name = Name,
                Unique_id = Id,
                Device = deviceConfig,
                State_topic = $"{Variables.MqttManager.MqttDiscoveryPrefix()}/{Domain}/{deviceConfig.Name}/{ObjectId}/state",
                Icon = "mdi:window-maximize",
                Availability_topic = $"{Variables.MqttManager.MqttDiscoveryPrefix()}/{Domain}/{deviceConfig.Name}/availability"
            });
        }

        public override string GetState()
        {
            return GetActiveWindowTitle();
        }

        public override string GetAttributes() => string.Empty;

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder builder, int count);

        private static string GetActiveWindowTitle()
        {
            var windowHandle = GetForegroundWindow();
            var titleLength = GetWindowTextLength(windowHandle) + 1;
            var builder = new StringBuilder(titleLength);
            var windowTitle = GetWindowText(windowHandle, builder, titleLength) > 0 ? builder.ToString() : string.Empty;

            return windowTitle.Length > 255 ? windowTitle[..255]: windowTitle; //Note(Amadeo): to make sure we don't exceed HA limitation of 255 payload length
        }
    }
}
