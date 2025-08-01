using HASS.Agent.Shared.Functions;
using HASS.Agent.Shared.Resources.Localization;

namespace HASS.Agent.Shared.Enums
{
    /// <summary>
    /// Contains the state changes that can occur in a system
    /// </summary>
    public enum SystemStateEvent
    {
        [LocalizedDescription("SystemStateEvent_ApplicationStarted", typeof(Languages))]
        ApplicationStarted,

        [LocalizedDescription("SystemStateEvent_Logoff", typeof(Languages))]
        Logoff,

        [LocalizedDescription("SystemStateEvent_SystemShutdown", typeof(Languages))]
        SystemShutdown,

        [LocalizedDescription("SystemStateEvent_Resume", typeof(Languages))]
        Resume,

        [LocalizedDescription("SystemStateEvent_Suspend", typeof(Languages))]
        Suspend,

        [LocalizedDescription("SystemStateEvent_ConsoleConnect", typeof(Languages))]
        ConsoleConnect,

        [LocalizedDescription("SystemStateEvent_ConsoleDisconnect", typeof(Languages))]
        ConsoleDisconnect,

        [LocalizedDescription("SystemStateEvent_RemoteConnect", typeof(Languages))]
        RemoteConnect,

        [LocalizedDescription("SystemStateEvent_RemoteDisconnect", typeof(Languages))]
        RemoteDisconnect,

        [LocalizedDescription("SystemStateEvent_SessionLock", typeof(Languages))]
        SessionLock,

        [LocalizedDescription("SystemStateEvent_SessionLogoff", typeof(Languages))]
        SessionLogoff,

        [LocalizedDescription("SystemStateEvent_SessionLogon", typeof(Languages))]
        SessionLogon,

        [LocalizedDescription("SystemStateEvent_SessionRemoteControl", typeof(Languages))]
        SessionRemoteControl,

        [LocalizedDescription("SystemStateEvent_SessionUnlock", typeof(Languages))]
        SessionUnlock
    }
}