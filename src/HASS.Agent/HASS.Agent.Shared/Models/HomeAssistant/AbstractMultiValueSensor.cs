using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Serilog;

namespace HASS.Agent.Shared.Models.HomeAssistant
{
    /// <summary>
    /// Abstract multivalue-sensor from which all multivalue-sensors are derived
    /// </summary>
    public abstract class AbstractMultiValueSensor : AbstractDiscoverable
    {
        public int UpdateIntervalSeconds { get; protected set; }
        public DateTime? LastUpdated { get; protected set; }

        public abstract Dictionary<string, AbstractSingleValueSensor> Sensors { get; protected set; }

        protected AbstractMultiValueSensor(string entityName, string name, int updateIntervalSeconds = 10, string id = default)
        {
            Id = id == null || id == Guid.Empty.ToString() ? Guid.NewGuid().ToString() : id;
            EntityName = entityName;
            Name = name;
            UpdateIntervalSeconds = updateIntervalSeconds;
            Domain = "sensor";
        }

        public override void ClearAutoDiscoveryConfig()
        {
            foreach (var sensor in Sensors) sensor.Value.ClearAutoDiscoveryConfig();
        }

        public abstract void UpdateSensorValues();

        public void ResetChecks()
        {
            LastUpdated = DateTime.MinValue;
            foreach (var sensor in Sensors) sensor.Value.ResetChecks();
        }
        
        public async Task PublishStatesAsync(bool respectChecks = true)
        {
            try
            {
                if (respectChecks)
                {
                    if (LastUpdated.HasValue && LastUpdated.Value.AddSeconds(UpdateIntervalSeconds) > DateTime.Now) return;
                }

                if (!Sensors.Any()) return;

                // fetch new values for all sensors
                UpdateSensorValues();

                // update their values
                foreach (var sensor in Sensors) await sensor.Value.PublishStateAsync(respectChecks);

                LastUpdated = DateTime.Now;
            }
            catch (Exception ex)
            {
                Log.Fatal("[SENSOR] [{name}] Error publishing state: {err}", EntityName, ex.Message);
            }
        }

        public async Task PublishAutoDiscoveryConfigAsync()
        {
            foreach (var sensor in Sensors) await sensor.Value.PublishAutoDiscoveryConfigAsync();
        }

        public async Task UnPublishAutoDiscoveryConfigAsync(bool migration = false)
        {
            foreach (var sensor in Sensors) await sensor.Value.UnPublishAutoDiscoveryConfigAsync(migration);
        }
    }
}
