using System;
using HASS.Agent.Shared.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace HASS.Agent.Shared.Models.Config.Old
{
    /// <summary>
    /// Storable version of sensor objects for the HASS.Agent 2023 Beta
    /// </summary>
    public class ConfiguredSensor2023Beta
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public SensorType Type { get; set; }
        public Guid Id { get; set; } = Guid.Empty;
        [JsonProperty("FriendlyName")]
        public string Name { get; set; } = string.Empty;
        public int? UpdateInterval { get; set; }
        public string Query { get; set; } = string.Empty;
        public string Scope { get; set; }
        public string WindowName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Counter { get; set; } = string.Empty;
        public string Instance { get; set; } = string.Empty;
        [JsonProperty("Name")]
        public string EntityName { get; set; } = string.Empty;
        public bool IgnoreAvailability { get; set; } = false;
        public bool ApplyRounding { get; set; } = false;
        public int? Round { get; set; }

        public static bool InJsonData(string jsonData)
        {
            return jsonData.Contains("FriendlyName");
        }
    }
}