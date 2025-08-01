using HASS.Agent.Shared.Enums;
using HASS.Agent.Shared.Models.Config;

namespace HASS.Agent.Satellite.Service.Extensions
{
    /// <summary>
    /// Extensions for HASS.Agent sensor objects
    /// </summary>
    public static class SensorExtensions
    {
        /// <summary>
        /// Returns TRUE if the configured sensor is single-value
        /// </summary>
        /// <param name="configuredSensor"></param>
        /// <returns></returns>
        public static bool IsSingleValue(this ConfiguredSensor configuredSensor) => configuredSensor.Type.IsSingleValue();

        /// <summary>
        /// Returns TRUE if the sensor-type is single-value
        /// </summary>
        /// <param name="sensorType"></param>
        /// <returns></returns>
        public static bool IsSingleValue(this SensorType sensorType)
        {
            // todo: this should be done through the shared library, prone to be forgotten
            return sensorType != SensorType.StorageSensors
                   && sensorType != SensorType.NetworkSensors
                   && sensorType != SensorType.WindowsUpdatesSensors
                   && sensorType != SensorType.BatterySensors
                   && sensorType != SensorType.DisplaySensors
                   && sensorType != SensorType.AudioSensors;
        }
    }
}
