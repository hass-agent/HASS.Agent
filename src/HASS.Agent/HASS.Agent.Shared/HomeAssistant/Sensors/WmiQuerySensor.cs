using System;
using System.Globalization;
using System.Management;
using HASS.Agent.Shared.Models.HomeAssistant;
using HASS.Agent.Shared.Models.Internal;
using Newtonsoft.Json;

namespace HASS.Agent.Shared.HomeAssistant.Sensors
{
    /// <summary>
    /// Sensor containing the result of the provided WMI query
    /// </summary>
    public class WmiQuerySensor : AbstractSingleValueSensor
    {
        private const string DefaultName = "wmiquerysensor";

        public string Query { get; private set; }
        public string Scope { get; private set; }
        public bool ApplyRounding { get; private set; }
        public int? Round { get; private set; }

        protected readonly ObjectQuery ObjectQuery;
        protected readonly ManagementObjectSearcher Searcher;

        public WmiQuerySensor(string query, string scope = "", bool applyRounding = false, int? round = null, int? updateInterval = null, string entityName = DefaultName, string name = DefaultName, string id = default, string advancedSettings = default) : base(entityName ?? DefaultName, name ?? null, updateInterval ?? 10, id, false, advancedSettings)
        {
            Query = query;
            Scope = scope;
            ApplyRounding = applyRounding;
            Round = round;

            // prepare query
            ObjectQuery = new ObjectQuery(Query);

            // use either default or provided scope
            var managementscope = !string.IsNullOrWhiteSpace(scope)
                ? new ManagementScope(scope)
                : new ManagementScope(@"\\localhost\");

            // prepare searcher
            Searcher = new ManagementObjectSearcher(managementscope, ObjectQuery);
        }

        public void Dispose() => Searcher?.Dispose();

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
                State_topic = $"{Variables.MqttManager.MqttDiscoveryPrefix()}/{Domain}/{deviceConfig.Name}/{EntityName}/state",
                Availability_topic = $"{Variables.MqttManager.MqttDiscoveryPrefix()}/{Domain}/{deviceConfig.Name}/availability"
            });
        }

        public override string GetState()
        {
            using var collection = Searcher.Get();
            var retValue = string.Empty;

            foreach (var managementBaseObject in collection)
            {
                try
                {
                    if (!string.IsNullOrEmpty(retValue))
                        continue;

                    using var managementObject = (ManagementObject)managementBaseObject;
                    foreach (var property in managementObject.Properties)
                    {
                        retValue = property?.Value?.ToString() ?? string.Empty;
                        break;
                    }
                }
                finally
                {
                    managementBaseObject?.Dispose();
                }
            }

            // optionally apply rounding
            if (ApplyRounding && Round != null && double.TryParse(retValue, out var dblValue))
            {
                retValue = Math.Round(dblValue, (int)Round).ToString(CultureInfo.CurrentCulture);
            }

            // done
            return retValue;
        }

        public override string GetAttributes() => string.Empty;
    }
}
