using System.ComponentModel;
using System.Runtime.Serialization;
using HASS.Agent.Shared.Functions;
using HASS.Agent.Shared.Resources.Localization;

namespace HASS.Agent.Shared.Enums
{
    public enum HassAction
    {
        [LocalizedDescription("HassAction_On", typeof(Languages))]
        [Category("turn_on")]
        [EnumMember(Value = "On")]
        On,

        [LocalizedDescription("HassAction_Off", typeof(Languages))]
        [Category("turn_off")]
        [EnumMember(Value = "Off")]
        Off,

        [LocalizedDescription("HassAction_Open", typeof(Languages))]
        [Category("open_cover")]
        [EnumMember(Value = "Open")]
        Open,

        [LocalizedDescription("HassAction_Close", typeof(Languages))]
        [Category("close_cover")]
        [EnumMember(Value = "Close")]
        Close,

        [LocalizedDescription("HassAction_Play", typeof(Languages))]
        [Category("media_play")]
        [EnumMember(Value = "Play")]
        Play,

        [LocalizedDescription("HassAction_Pause", typeof(Languages))]
        [Category("media_pause")]
        [EnumMember(Value = "Pause")]
        Pause,

        [LocalizedDescription("HassAction_Stop", typeof(Languages))]
        [Category("media_stop")]
        [EnumMember(Value = "Stop")]
        Stop,

        [LocalizedDescription("HassAction_Toggle", typeof(Languages))]
        [Category("toggle")]
        [EnumMember(Value = "Toggle")]
        Toggle,

        [LocalizedDescription("HassAction_Trigger", typeof(Languages))]
        [Category("trigger")]
        [EnumMember(Value = "Trigger")]
        Trigger
    }
}