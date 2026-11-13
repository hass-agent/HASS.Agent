using System.Globalization;
using System.Management;
using HASS.Agent.Shared.Models.HomeAssistant;

namespace HASS.Agent.Shared.HomeAssistant.Sensors.WmiSensors.SingleValue
{
    /// <summary>
    /// Sensor containing the amount of used memory in MB
    /// </summary>
    public class MemoryUsageSensor : WmiQuerySensor
    {
        private const string DefaultName = "memoryusage";

        public MemoryUsageSensor(int? updateInterval = null, string entityName = DefaultName, string name = DefaultName, string id = default, bool applyRounding = false, int? round = null, string advancedSettings = default) : base("SELECT FreePhysicalMemory,TotalVisibleMemorySize FROM Win32_OperatingSystem", string.Empty, applyRounding, round, updateInterval ?? 30, entityName ?? DefaultName, name ?? null, id, advancedSettings: advancedSettings) { }

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
                Icon = "mdi:memory",
                Unit_of_measurement = "%",
                Availability_topic = $"{Variables.MqttManager.MqttDiscoveryPrefix()}/{Domain}/{deviceConfig.Name}/availability"
            });
        }

        public override string GetState()
        {
            using var collection = Searcher.Get();
            ulong? totalMemory = null;
            ulong? freeMemory = null;

            foreach (var o in collection)
            {
                var mo = (ManagementObject)o;
                try
                {
                    totalMemory = (ulong)(mo.Properties["TotalVisibleMemorySize"].Value ?? 0);
                    freeMemory = (ulong)(mo.Properties["FreePhysicalMemory"].Value ?? 0);
                }
                finally
                {
                    mo?.Dispose();
                }
            }

            if (totalMemory == null) return string.Empty;

            decimal totalMemoryDec = totalMemory.Value;
            decimal freeMemoryDec = freeMemory.Value;

            var precentageUsed = 100 - (freeMemoryDec / totalMemoryDec) * 100;
            return precentageUsed.ToString("#.##", CultureInfo.InvariantCulture);
        }
    }
}
