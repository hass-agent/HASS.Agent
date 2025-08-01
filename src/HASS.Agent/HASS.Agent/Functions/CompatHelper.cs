using HASS.Agent.HomeAssistant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HASS.Agent.Functions
{
    internal static class CompatHelper
    {
        private static (int, int, int) SplitHAVerion(string haVersion)
        {
            var splitVersion = haVersion.Split('.');

            var major = 0;
            if (splitVersion.Length > 0)
            {
                _ = int.TryParse(splitVersion[0], out major);
            }

            var minor = 0;
            if (splitVersion.Length > 1)
            {
                _ = int.TryParse(splitVersion[1], out minor);
            }

            var patch = 0;
            if (splitVersion.Length > 2)
            {
                _ = int.TryParse(splitVersion[2], out patch);
            }

            return (major, minor, patch);
        }

        /// <summary>
        /// Function checks if the Home Assistant connected to HASS.Agent is greater or equal to <paramref name="haVersion"/>.
        /// </summary>
        /// <param name="haVersion"></param>
        /// <returns>
        /// True if version is greater or equal from <paramref name="haVersion"/>.
        /// False if version is lower of there is no connection to Home Assistant instance or it's version cannot be determined.
        /// </returns>
        internal static bool HassVersionEqualOrOver(string haVersion)
        {
            if (string.IsNullOrWhiteSpace(haVersion) || string.IsNullOrWhiteSpace(HassApiManager.HaVersion))
            {
                return false;
            }

            var (targetMajor, targetMinor, targetPatch) = SplitHAVerion(haVersion);
            var (major, minor, patch) = SplitHAVerion(HassApiManager.HaVersion);

            if(major == 0)
            {
                return false;
            }

            return major > targetMajor
                || major == targetMajor && minor > targetMinor
                || major == targetMajor && minor == targetMinor && patch >= targetPatch;
        }
    }
}
