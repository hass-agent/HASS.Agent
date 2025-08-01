using HASS.Agent.Shared.Functions;
using HASS.Agent.Shared.Resources.Localization;

namespace HASS.Agent.Shared.Enums
{
    public enum ComponentStatus
    {
        [LocalizedDescription("ComponentStatus_Connecting", typeof(Languages))]
        Connecting,

        [LocalizedDescription("ComponentStatus_Failed", typeof(Languages))]
        Failed,

        [LocalizedDescription("ComponentStatus_Loading", typeof(Languages))]
        Loading,

        [LocalizedDescription("ComponentStatus_Ok", typeof(Languages))]
        Ok,

        [LocalizedDescription("ComponentStatus_Stopped", typeof(Languages))]
        Stopped,

        [LocalizedDescription("ComponentStatus_Disabled", typeof(Languages))]
        Disabled
    }
}