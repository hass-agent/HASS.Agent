using System;
using HASS.Agent.Shared.Enums;
using HASS.Agent.Shared.Models.Config.Old;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace HASS.Agent.Shared.Models.Config
{
    /// <summary>
    /// Storable version of sensor objects
    /// </summary>
    public class ConfiguredSensor
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public SensorType Type { get; set; }
        public Guid Id { get; set; } = Guid.Empty;
        public string Name { get; set; } = string.Empty;
        public int? UpdateInterval { get; set; }
        public string Query { get; set; } = string.Empty;
        public string Scope { get; set; }
        public string WindowName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Counter { get; set; } = string.Empty;
        public string Instance { get; set; } = string.Empty;
        public string EntityName { get; set; } = string.Empty;
        public bool IgnoreAvailability { get; set; } = false;
        public bool ApplyRounding { get; set; } = false;
        public int? Round { get; set; }
        public string AdvancedSettings { get; set; } = string.Empty;

        public static ConfiguredSensor FromLAB02(ConfiguredSensorLAB02 oldConfig)
        {
            return new ConfiguredSensor()
            {
                Type = oldConfig.Type,
                Id = oldConfig.Id,
                Name = oldConfig.Name,
                UpdateInterval = oldConfig.UpdateInterval,
                Query = oldConfig.Query,
                Scope = oldConfig.Scope,
                WindowName = oldConfig.WindowName,
                Category = oldConfig.Category,
                Counter = oldConfig.Counter,
                Instance = oldConfig.Instance,
                ApplyRounding = oldConfig.ApplyRounding,
                Round = oldConfig.Round,
                EntityName = oldConfig.Name,
            };
        }

        public static ConfiguredSensor From2023Beta(ConfiguredSensor2023Beta oldConfig)
        {
            return new ConfiguredSensor()
            {
                Type = oldConfig.Type,
                Id = oldConfig.Id,
                Name = oldConfig.Name,
                UpdateInterval = oldConfig.UpdateInterval,
                Query = oldConfig.Query,
                Scope = oldConfig.Scope,
                WindowName = oldConfig.WindowName,
                Category = oldConfig.Category,
                Counter = oldConfig.Counter,
                Instance = oldConfig.Instance,
                ApplyRounding = oldConfig.ApplyRounding,
                Round = oldConfig.Round,
                EntityName = oldConfig.EntityName,
            };
        }
    }


}