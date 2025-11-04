using System;
using System.Globalization;
using System.Management;
using System.Runtime.InteropServices;
using HASS.Agent.Shared.Models.HomeAssistant;
using HASS.Agent.Shared.Models.Internal;
using Newtonsoft.Json;
using Serilog;
using static System.Formats.Asn1.AsnWriter;

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
        protected ManagementObjectSearcher Searcher;

        public WmiQuerySensor(string query, string scope = "", bool applyRounding = false, int? round = null, int? updateInterval = null, string entityName = DefaultName, string name = DefaultName, string id = default, string advancedSettings = default) : base(entityName ?? DefaultName, name ?? null, updateInterval ?? 10, id, false, advancedSettings)
        {
            Query = query;
            Scope = scope;
            ApplyRounding = applyRounding;
            Round = round;

            ObjectQuery = new ObjectQuery(Query);
            Searcher = CreateSearcher();
        }

        private ManagementObjectSearcher CreateSearcher()
        {
            Searcher?.Dispose();

            var managementScope = !string.IsNullOrWhiteSpace(Scope)
                ? new ManagementScope(Scope)
                : new ManagementScope(@"\\localhost\");

            return new ManagementObjectSearcher(managementScope, ObjectQuery);
        }

        public void Dispose() => Searcher?.Dispose();

        public override DiscoveryConfigModel GetAutoDiscoveryConfig()
        {
            if (Variables.MqttManager == null)
                return null;

            var deviceConfig = Variables.MqttManager.GetDeviceConfigModel();
            if (deviceConfig == null)
                return null;

            return AutoDiscoveryConfigModel ?? SetAutoDiscoveryConfigModel(new SensorDiscoveryConfigModel(Domain)
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
            try
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
            catch (COMException)
            {
                Log.Warning("[WMIQUERY] [{name}] Searcher is no longer valid, recreating", Name);
                Searcher = CreateSearcher();
                return null;
            }
        }

        public override string GetAttributes() => string.Empty;
    }
}
