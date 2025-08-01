using HASS.Agent.Shared.Functions;
using HASS.Agent.Shared.Resources.Localization;

namespace HASS.Agent.Shared.Enums
{
    /// <summary>
    /// Contains the user's Windows notification states
    /// </summary>
    public enum UserNotificationState
    {
        [LocalizedDescription("UserNotificationState_NotPresent", typeof(Languages))]
        NotPresent = 1,

        [LocalizedDescription("UserNotificationState_Busy", typeof(Languages))]
        Busy = 2,

        [LocalizedDescription("UserNotificationState_RunningDirect3dFullScreen", typeof(Languages))]
        RunningDirect3dFullScreen = 3,

        [LocalizedDescription("UserNotificationState_PresentationMode", typeof(Languages))]
        PresentationMode = 4,

        [LocalizedDescription("UserNotificationState_AcceptsNotifications", typeof(Languages))]
        AcceptsNotifications = 5,

        [LocalizedDescription("UserNotificationState_QuietTime", typeof(Languages))]
        QuietTime = 6,

        [LocalizedDescription("UserNotificationState_RunningWindowsStoreApp", typeof(Languages))]
        RunningWindowsStoreApp = 7
    }
}