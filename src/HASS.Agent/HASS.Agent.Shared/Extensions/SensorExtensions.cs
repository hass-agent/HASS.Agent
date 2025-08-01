using HASS.Agent.Shared.Enums;
using HASS.Agent.Shared.Functions;

namespace HASS.Agent.Shared.Extensions
{
    /// <summary>
    /// Extensions for HASS.Agent sensor objects
    /// </summary>
    public static class SensorExtensions
    {
        /// <summary>
        /// Returns the name of the sensortype
        /// </summary>
        /// <param name="sensorType"></param>
        /// <returns></returns>
        public static string GetSensorName(this SensorType sensorType)
        {
            var (_, name) = sensorType.GetLocalizedDescriptionAndKey();
            return name.ToLower();
        }

        //TODO: remove after tests

        /// <summary>
        /// Returns the name of the sensortype, based on the provided devicename
        /// </summary>
        /// <param name="sensorType"></param>
        /// <param name="deviceName"></param>
        /// <returns></returns>
/*        public static string GetSensorName(this SensorType sensorType, string deviceName) 
        {
            var (_, name) = sensorType.GetLocalizedDescriptionAndKey();
            var sensorName = name.ToLower();
            
            return $"{SharedHelperFunctions.GetSafeValue(deviceName)}_{sensorName}";
        }*/
    }
}
