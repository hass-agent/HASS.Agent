using System.Diagnostics;
using System.IO;
using System.Linq;
using HASS.Agent.Shared.Enums;
using HASS.Agent.Shared.Functions;
using HASS.Agent.Shared.Models.HomeAssistant;

namespace HASS.Agent.Shared.HomeAssistant.Sensors.GeneralSensors.SingleValue
{
    /// <summary>
    /// Sensor indicating the current state of the provided application
    /// </summary>
    public class WindowStateSensor : AbstractSingleValueSensor
    {
        private const string DefaultName = "windowstate";
        public string ProcessName { get; protected set; }

        public WindowStateSensor(string processName, string entityName = DefaultName, string name = DefaultName, int? updateInterval = 10, string id = default, string advancedSettings = default) : base(entityName ?? DefaultName, name ?? null, updateInterval ?? 30, id, advancedSettings: advancedSettings)
        {
            ProcessName = processName;
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
                Availability_topic = $"{Variables.MqttManager.MqttDiscoveryPrefix()}/sensor/{deviceConfig.Name}/availability"
            });
        }

        public override string GetState()
        {
            // we don't need the extension
            if (ProcessName.Contains(".")) ProcessName = Path.GetFileNameWithoutExtension(ProcessName);

            // search for our process
            var procs = Process.GetProcessesByName(ProcessName);
            if (!procs.Any()) return WindowState.Unknown.ToString();

            // get the placement
            var windowPlacement = NativeMethods.GetPlacement(procs.First().MainWindowHandle);

            // dispose all objects
            foreach (var proc in procs) proc?.Dispose();

            // done
            return windowPlacement.showCmd.ToString();
        }

        public override string GetAttributes() => string.Empty;
    }
}
