using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using HASS.Agent.Shared.Models.HomeAssistant;
using Newtonsoft.Json;

namespace HASS.Agent.Shared.HomeAssistant.Sensors.GeneralSensors.SingleValue
{
    /// <summary>
    /// Sensor containing the title of the current active window
    /// </summary>
    public class ActiveWindowSensor : AbstractSingleValueSensor
    {
        private const string DefaultName = "activewindow";

        private string _processName = string.Empty;

        public ActiveWindowSensor(int? updateInterval = null, string entityName = DefaultName, string name = DefaultName, string id = default) : base(entityName ?? DefaultName, name ?? null, updateInterval ?? 15, id, true) { }

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
                Availability_topic = $"{Variables.MqttManager.MqttDiscoveryPrefix()}/{Domain}/{deviceConfig.Name}/availability",
                Json_attributes_topic = $"{Variables.MqttManager.MqttDiscoveryPrefix()}/{Domain}/{deviceConfig.Name}/{ObjectId}/attributes"
            });
        }

        public override string GetState()
        {
            var windowHandle = GetForegroundWindow();

            GetWindowThreadProcessId(windowHandle, out var processId);
            var process = Process.GetProcessById(Convert.ToInt32(processId));
            _processName = process.ProcessName ?? string.Empty;

            return GetWindowTitle(windowHandle);
        }

        public override string GetAttributes() => JsonConvert.SerializeObject(new
        {
           processName = _processName
        });

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder builder, int count);

        [DllImport("user32.dll", SetLastError = true)]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);

        private static string GetWindowTitle(IntPtr windowHandle)
        {
            var titleLength = GetWindowTextLength(windowHandle) + 1;
            var builder = new StringBuilder(titleLength);
            var windowTitle = GetWindowText(windowHandle, builder, titleLength) > 0 ? builder.ToString() : string.Empty;

            return windowTitle.Length > 255 ? windowTitle[..255]: windowTitle; //Note(Amadeo): to make sure we don't exceed HA limitation of 255 payload length
        }
    }
}
