using HASS.Agent.Shared.Functions;
using HASS.Agent.Shared.Resources.Localization;

namespace HASS.Agent.Shared.Enums
{
    /// <summary>
    /// Contains the states in which a Windows session can exist
    /// </summary>
    public enum LockState
    {
        [LocalizedDescription("LockState_Locked", typeof(Languages))]
        Locked,

        [LocalizedDescription("LockState_Unknown", typeof(Languages))]
        Unknown,

        [LocalizedDescription("LockState_Unlocked", typeof(Languages))]
        Unlocked
    }
}
