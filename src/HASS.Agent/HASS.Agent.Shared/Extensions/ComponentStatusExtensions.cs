using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using HASS.Agent.Shared.Enums;

namespace HASS.Agent.Shared.Extensions
{
    public static class ComponentStatusExtensions
    {
        /// <summary>
        /// Gets the corresponding color for the status
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Color GetColor(this ComponentStatus value)
        {
            return value switch
            {
                ComponentStatus.Loading => Color.DodgerBlue,
                ComponentStatus.Connecting => Color.DodgerBlue,
                ComponentStatus.Ok => Color.LimeGreen,
                ComponentStatus.Failed => Color.OrangeRed,
                ComponentStatus.Stopped => Color.Yellow,
                _ => Color.Gray
            };
        }
    }
}
