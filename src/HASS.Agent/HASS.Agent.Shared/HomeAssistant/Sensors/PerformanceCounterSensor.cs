using System;
using System.Diagnostics;
using System.Globalization;
using HASS.Agent.Shared.Functions;
using HASS.Agent.Shared.Models.HomeAssistant;

namespace HASS.Agent.Shared.HomeAssistant.Sensors
{
    /// <summary>
    /// Sensor containing the current value of the provided performance counter
    /// </summary>
    public class PerformanceCounterSensor : AbstractSingleValueSensor
    {
        private const string DefaultName = "performancecountersensor";

        protected PerformanceCounter Counter = null;

        public string CategoryName { get; private set; }
        public string CounterName { get; private set; }
        public string InstanceName { get; private set; }

        public bool ApplyRounding { get; private set; }
        public int? Round { get; private set; }

        public PerformanceCounterSensor(string categoryName, string counterName, string instanceName, bool applyRounding = false, int? round = null, int? updateInterval = null, string entityName = DefaultName, string name = DefaultName, string id = default, string advancedSettings = default) : base(entityName ?? DefaultName, name ?? null, updateInterval ?? 10, id, advancedSettings: advancedSettings)
        {
            CategoryName = categoryName;
            CounterName = counterName;
            InstanceName = instanceName;
            ApplyRounding = applyRounding;
            Round = round;

            Counter = PerformanceCounters.GetSingleInstanceCounter(categoryName, counterName);
            if (Counter == null) throw new Exception("PerformanceCounter not found");

            Counter.InstanceName = instanceName;

            Counter.NextValue();
        }

        public void Dispose() => Counter?.Dispose();

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
                State_topic = $"{Variables.MqttManager.MqttDiscoveryPrefix()}/{Domain}/{deviceConfig.Name}/{EntityName}/state",
                State_class = "measurement",
                Availability_topic = $"{Variables.MqttManager.MqttDiscoveryPrefix()}/{Domain}/{deviceConfig.Name}/availability"
            });
        }

        public override string GetState()
        {
            var nextVal = Counter.NextValue();

            // optionally apply rounding
            if (ApplyRounding && Round != null && double.TryParse(nextVal.ToString(CultureInfo.CurrentCulture), out var dblValue))
            {
                return Math.Round(dblValue, (int)Round).ToString(CultureInfo.CurrentCulture);
            }

            // done
            return Math.Round(nextVal).ToString(CultureInfo.CurrentCulture);
        }

        public override string GetAttributes() => string.Empty;
    }
}
