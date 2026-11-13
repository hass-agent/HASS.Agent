using System.Diagnostics;
using System.IO;
using System.Linq;
using HASS.Agent.Shared.Models.HomeAssistant;

namespace HASS.Agent.Shared.HomeAssistant.Sensors.GeneralSensors.SingleValue
{
    /// <summary>
    /// Sensor indicating how many (if any) instances of the provided process are active
    /// </summary>
    public class ProcessActiveSensor : AbstractSingleValueSensor
    {
        private const string DefaultName = "processactive";
        public string ProcessName { get; protected set; }

        public ProcessActiveSensor(string processName, int? updateInterval = null, string entityName = DefaultName, string name = DefaultName, string id = default, string advancedSettings = default) : base(entityName ?? DefaultName, name ?? null, updateInterval ?? 10, id, advancedSettings: advancedSettings)
        {
            ProcessName = processName;

            // remove common extensions
            var procLc = ProcessName.ToLower();
            if (procLc.EndsWith(".exe") || procLc.EndsWith(".cmd") || procLc.EndsWith(".dll")) ProcessName = ProcessName[..^4];
        }

        public override DiscoveryConfigModel GetAutoDiscoveryConfig()
        {
            if (Variables.MqttManager == null) return null;

            var deviceConfig = Variables.MqttManager.GetDeviceConfigModel();
            if (deviceConfig == null) return null;

            return AutoDiscoveryConfigModel ?? SetAutoDiscoveryConfigModel(new SensorDiscoveryConfigModel(Domain)
            {
                EntityName = EntityName,
                Name = Name,
                Unique_id = Id,
                Device = deviceConfig,
                State_topic = $"{Variables.MqttManager.MqttDiscoveryPrefix()}/{Domain}/{deviceConfig.Name}/{ObjectId}/state",
                State_class = "measurement",
                Icon = "mdi:file-eye-outline",
                Availability_topic = $"{Variables.MqttManager.MqttDiscoveryPrefix()}/sensor/{deviceConfig.Name}/availability"
            });
        }

        public override string GetState()
        {
            // search for our process
            var procs = Process.GetProcessesByName(ProcessName);
            var instanceCount = procs.Any() ? procs.Length : 0;

            // dispose all objects
            foreach (var proc in procs) proc?.Dispose();

            // done
            return instanceCount.ToString();
        }

        public override string GetAttributes() => string.Empty;
    }
}
