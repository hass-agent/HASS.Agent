using System;
using System.Collections.Generic;
using HASS.Agent.Shared.Enums;
using HASS.Agent.Shared.Models.Config.Old;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using static HASS.Agent.Shared.Functions.Inputs;

namespace HASS.Agent.Shared.Models.Config
{
    /// <summary>
    /// Storable version of command objects
    /// </summary>
    public class ConfiguredCommand
    {
        public Guid Id { get; set; } = Guid.Empty;
        public string Name { get; set; } = string.Empty;
        public string EntityName { get; set; } = string.Empty;

        [JsonConverter(typeof(StringEnumConverter))]
        public CommandType Type { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public CommandEntityType EntityType { get; set; } = CommandEntityType.Switch;

        public string Command { get; set; } = string.Empty;

        public VirtualKeyShort KeyCode { get; set; }
        public bool RunAsLowIntegrity { get; set; } = false;
        public List<string> Keys { get; set; } = new List<string>();

        public static ConfiguredCommand FromLAB02(ConfiguredCommandLAB02 oldConfig)
        {
            return new ConfiguredCommand()
            {
                Id = oldConfig.Id,
                Name = oldConfig.Name,
                EntityName = oldConfig.Name,
                Type = oldConfig.Type,
                EntityType = oldConfig.EntityType,
                Command = oldConfig.Command,
                KeyCode = (VirtualKeyShort)oldConfig.KeyCode,
                RunAsLowIntegrity = oldConfig.RunAsLowIntegrity,
                Keys = oldConfig.Keys
            };
        }

        public static ConfiguredCommand From2023Beta(ConfiguredCommand2023Beta oldConfig)
        {
            return new ConfiguredCommand()
            {
                Id = oldConfig.Id,
                Name = oldConfig.Name,
                EntityName = oldConfig.EntityName,
                Type = oldConfig.Type,
                EntityType = oldConfig.EntityType,
                Command = oldConfig.Command,
                KeyCode = oldConfig.KeyCode,
                RunAsLowIntegrity = oldConfig.RunAsLowIntegrity,
                Keys = oldConfig.Keys  
            };
        }
    }
}
