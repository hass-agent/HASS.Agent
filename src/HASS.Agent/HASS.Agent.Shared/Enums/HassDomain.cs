using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using HASS.Agent.Shared.Functions;
using HASS.Agent.Shared.Resources.Localization;

namespace HASS.Agent.Shared.Enums
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum HassDomain
    {
        [LocalizedDescription("HassDomain_Automation", typeof(Languages))]
        [Category("automation")]
        [EnumMember(Value = "Automation")]
        Automation,

        [LocalizedDescription("HassDomain_Climate", typeof(Languages))]
        [Category("climate")]
        [EnumMember(Value = "Climate")]
        Climate,

        [LocalizedDescription("HassDomain_Cover", typeof(Languages))]
        [Category("cover")]
        [EnumMember(Value = "Cover")]
        Cover,

        [LocalizedDescription("HassDomain_HASSAgentCommands", typeof(Languages))]
        [Category("hass_agent_commands")]
        [EnumMember(Value = "HASSAgentCommands")]
        HASSAgentCommands,

        [LocalizedDescription("HassDomain_InputBoolean", typeof(Languages))]
        [Category("input_boolean")]
        [EnumMember(Value = "InputBoolean")]
        InputBoolean,

        [LocalizedDescription("HassDomain_Light", typeof(Languages))]
        [Category("light")]
        [EnumMember(Value = "Light")]
        Light,

        [LocalizedDescription("HassDomain_MediaPlayer", typeof(Languages))]
        [Category("media_player")]
        [EnumMember(Value = "MediaPlayer")]
        MediaPlayer,

        [LocalizedDescription("HassDomain_Scene", typeof(Languages))]
        [Category("scene")]
        [EnumMember(Value = "Scene")]
        Scene,

        [LocalizedDescription("HassDomain_Script", typeof(Languages))]
        [Category("script")]
        [EnumMember(Value = "Script")]
        Script,

        [LocalizedDescription("HassDomain_Switch", typeof(Languages))]
        [Category("switch")]
        [EnumMember(Value = "Switch")]
        Switch,

        [LocalizedDescription("HassDomain_Button", typeof(Languages))]
        [Category("button")]
        [EnumMember(Value = "Button")]
        Button
    }
}