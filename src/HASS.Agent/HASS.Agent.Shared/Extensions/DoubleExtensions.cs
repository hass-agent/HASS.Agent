using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace HASS.Agent.Shared.Extensions
{
    public static class DoubleExtensions
    {
        /// <summary>
        /// Converts the double into a string, forcing a dot as the decimal seperator
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ConvertToStringDotDecimalSeperator(this double value) => value.ToString(CultureInfo.InvariantCulture).Replace(",", ".");
    }
}
