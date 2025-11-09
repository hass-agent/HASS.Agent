using System.Runtime.Serialization;
using HASS.Agent.Shared.Functions;
using HASS.Agent.Shared.Resources.Localization;

namespace HASS.Agent.Shared.Enums
{
    /// <summary>
    /// Contains all possible command types
    /// </summary>
    public enum CommandType
    {
        [LocalizedDescription("CommandType_CustomCommand", typeof(Languages))]
        [EnumMember(Value = "CustomCommand")]
        CustomCommand,

        [LocalizedDescription("CommandType_CustomExecutorCommand", typeof(Languages))]
        [EnumMember(Value = "CustomExecutorCommand")]
        CustomExecutorCommand,

        [LocalizedDescription("CommandType_HibernateCommand", typeof(Languages))]
        [EnumMember(Value = "HibernateCommand")]
        HibernateCommand,

        [LocalizedDescription("CommandType_KeyCommand", typeof(Languages))]
        [EnumMember(Value = "KeyCommand")]
        KeyCommand,

        [LocalizedDescription("CommandType_LaunchUrlCommand", typeof(Languages))]
        [EnumMember(Value = "LaunchUrlCommand")]
        LaunchUrlCommand,

        [LocalizedDescription("CommandType_LockCommand", typeof(Languages))]
        [EnumMember(Value = "LockCommand")]
        LockCommand,

        [LocalizedDescription("CommandType_LogOffCommand", typeof(Languages))]
        [EnumMember(Value = "LogOffCommand")]
        LogOffCommand,

        [LocalizedDescription("CommandType_MediaMuteCommand", typeof(Languages))]
        [EnumMember(Value = "MediaMuteCommand")]
        MediaMuteCommand,

        [LocalizedDescription("CommandType_MediaNextCommand", typeof(Languages))]
        [EnumMember(Value = "MediaNextCommand")]
        MediaNextCommand,

        [LocalizedDescription("CommandType_MediaPlayPauseCommand", typeof(Languages))]
        [EnumMember(Value = "MediaPlayPauseCommand")]
        MediaPlayPauseCommand,

        [LocalizedDescription("CommandType_MediaPreviousCommand", typeof(Languages))]
        [EnumMember(Value = "MediaPreviousCommand")]
        MediaPreviousCommand,

        [LocalizedDescription("CommandType_MediaVolumeDownCommand", typeof(Languages))]
        [EnumMember(Value = "MediaVolumeDownCommand")]
        MediaVolumeDownCommand,

        [LocalizedDescription("CommandType_MediaVolumeUpCommand", typeof(Languages))]
        [EnumMember(Value = "MediaVolumeUpCommand")]
        MediaVolumeUpCommand,

        [LocalizedDescription("CommandType_MonitorSleepCommand", typeof(Languages))]
        [EnumMember(Value = "MonitorSleepCommand")]
        MonitorSleepCommand,

        [LocalizedDescription("CommandType_MonitorSleepPowerPlanCommand", typeof(Languages))]
        [EnumMember(Value = "MonitorSleepPowerPlanCommand")]
        MonitorSleepPowerPlanCommand,

        [LocalizedDescription("CommandType_MonitorWakeCommand", typeof(Languages))]
        [EnumMember(Value = "MonitorWakeCommand")]
        MonitorWakeCommand,

        [LocalizedDescription("CommandType_MultipleKeysCommand", typeof(Languages))]
        [EnumMember(Value = "MultipleKeysCommand")]
        MultipleKeysCommand,

        [LocalizedDescription("CommandType_PowershellCommand", typeof(Languages))]
        [EnumMember(Value = "PowershellCommand")]
        PowershellCommand,

        [LocalizedDescription("CommandType_PublishAllSensorsCommand", typeof(Languages))]
        [EnumMember(Value = "PublishAllSensorsCommand")]
        PublishAllSensorsCommand,

        [LocalizedDescription("CommandType_RestartCommand", typeof(Languages))]
        [EnumMember(Value = "RestartCommand")]
        RestartCommand,

        [LocalizedDescription("CommandType_SendWindowToFrontCommand", typeof(Languages))]
        [EnumMember(Value = "SendWindowToFrontCommand")]
        SendWindowToFrontCommand,

        [LocalizedDescription("CommandType_SwitchDesktopCommand", typeof(Languages))]
        [EnumMember(Value = "SwitchDesktopCommand")]
        SwitchDesktopCommand,

        [LocalizedDescription("CommandType_SetVolumeCommand", typeof(Languages))]
        [EnumMember(Value = "SetVolumeCommand")]
        SetVolumeCommand,

        [LocalizedDescription("CommandType_SetApplicationVolumeCommand", typeof(Languages))]
        [EnumMember(Value = "SetApplicationVolumeCommand")]
        SetApplicationVolumeCommand,

        [LocalizedDescription("CommandType_SetAudioOutputCommand", typeof(Languages))]
        [EnumMember(Value = "SetAudioOutputCommand")]
        SetAudioOutputCommand,

        [LocalizedDescription("CommandType_SetAudioInputCommand", typeof(Languages))]
        [EnumMember(Value = "SetAudioInputCommand")]
        SetAudioInputCommand,

        [LocalizedDescription("CommandType_ShutdownCommand", typeof(Languages))]
        [EnumMember(Value = "ShutdownCommand")]
        ShutdownCommand,

        [LocalizedDescription("CommandType_SleepCommand", typeof(Languages))]
        [EnumMember(Value = "SleepCommand")]
        SleepCommand,

        [LocalizedDescription("CommandType_WebViewCommand", typeof(Languages))]
        [EnumMember(Value = "WebViewCommand")]
        WebViewCommand,

        [LocalizedDescription("CommandType_TrayWebViewCommand", typeof(Languages))]
        [EnumMember(Value = "TrayWebViewCommand")]
        TrayWebViewCommand,

        [LocalizedDescription("CommandType_RadioCommand", typeof(Languages))]
        [EnumMember(Value = "RadioCommand")]
        RadioCommand,

        [LocalizedDescription("CommandType_WinformsSleepCommand", typeof(Languages))]
        [EnumMember(Value = "WinformsSleepCommand")]
        WinformsSleepCommand,

        [LocalizedDescription("CommandType_VoicemeeterCommand", typeof(Languages))]
        [EnumMember(Value = "VoicemeeterCommand")]
        VoicemeeterCommand
    }
}