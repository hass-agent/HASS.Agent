using System.Runtime.Serialization;
using HASS.Agent.Shared.Functions;
using HASS.Agent.Shared.Resources.Localization;

namespace HASS.Agent.Shared.Enums
{
    /// <summary>
    /// Contains all possible command entity types
    /// </summary>
    public enum CommandEntityType
    {
        [LocalizedDescription("CommandEntityType_Button", typeof(Languages))]
        [EnumMember(Value = "button")]
        Button,

        [LocalizedDescription("CommandEntityType_Light", typeof(Languages))]
        [EnumMember(Value = "light")]
        Light,

        [LocalizedDescription("CommandEntityType_Lock", typeof(Languages))]
        [EnumMember(Value = "lock")]
        Lock,

        [LocalizedDescription("CommandEntityType_Siren", typeof(Languages))]
        [EnumMember(Value = "siren")]
        Siren,

        [LocalizedDescription("CommandEntityType_Switch", typeof(Languages))]
        [EnumMember(Value = "switch")]
        Switch,
    }
}