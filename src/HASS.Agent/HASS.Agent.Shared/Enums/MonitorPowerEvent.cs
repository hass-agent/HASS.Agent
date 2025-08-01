using System;
using System.Collections.Generic;
using System.Text;
using HASS.Agent.Shared.Functions;
using HASS.Agent.Shared.Resources.Localization;

namespace HASS.Agent.Shared.Enums
{
    /// <summary>
    /// Contains the power changes for monitors
    /// </summary>
    public enum MonitorPowerEvent
    {
        [LocalizedDescription("MonitorPowerEvent_Dimmed", typeof(Languages))]
        Dimmed,

        [LocalizedDescription("MonitorPowerEvent_PowerOff", typeof(Languages))]
        PowerOff,

        [LocalizedDescription("MonitorPowerEvent_PowerOn", typeof(Languages))]
        PowerOn,

        [LocalizedDescription("MonitorPowerEvent_Unknown", typeof(Languages))]
        Unknown,
    }
}
