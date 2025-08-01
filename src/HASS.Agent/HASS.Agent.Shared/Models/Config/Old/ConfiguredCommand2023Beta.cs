using System;
using System.Collections.Generic;
using HASS.Agent.Shared.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using static HASS.Agent.Shared.Functions.Inputs;

namespace HASS.Agent.Shared.Models.Config.Old
{
    /// <summary>
    /// Storable version of command objects for the HASS.Agent 2023 Beta
    /// </summary>
    public class ConfiguredCommand2023Beta
    {
        public Guid Id { get; set; } = Guid.Empty;
        [JsonProperty("FriendlyName")]
        public string Name { get; set; } = string.Empty;
        [JsonProperty("Name")]
        public string EntityName { get; set; } = string.Empty;

        [JsonConverter(typeof(StringEnumConverter))]
        public CommandType Type { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public CommandEntityType EntityType { get; set; } = CommandEntityType.Switch;

        public string Command { get; set; } = string.Empty;

        public VirtualKeyShort KeyCode { get; set; }
        public bool RunAsLowIntegrity { get; set; } = false;
        public List<string> Keys { get; set; } = new List<string>();

        public static bool InJsonData(string jsonData)
        {
            return jsonData.Contains("FriendlyName");
        }
    }
}
