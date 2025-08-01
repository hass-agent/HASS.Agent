using System;
using System.Collections.Generic;
using System.Text;
using HASS.Agent.Shared.Functions;
using HASS.Agent.Shared.Resources.Localization;

namespace HASS.Agent.Shared.Enums
{
    /// <summary>
    /// Contains the possible state of a window
    /// </summary>
    public enum WindowState
    {
        [LocalizedDescription("WindowState_Hidden", typeof(Languages))]
        Hidden = 0,

        [LocalizedDescription("WindowState_Maximized", typeof(Languages))]
        Maximized = 3,

        [LocalizedDescription("WindowState_Minimized", typeof(Languages))]
        Minimized = 2,

        [LocalizedDescription("WindowState_Normal", typeof(Languages))]
        Normal = 1,

        [LocalizedDescription("WindowState_Unknown", typeof(Languages))]
        Unknown = -1
    }
}
