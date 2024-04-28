using System;
using HASS.Agent.Shared.Mqtt;

namespace HASS.Agent.Shared
{
    internal class Variables
    {
        /// <summary>
        /// Device info
        /// </summary>
        internal static string DeviceName { get; set; } = string.Empty;

        /// <summary>
        /// public references
        /// </summary>
        internal static Random Rnd { get; } = new Random();

        /// <summary>
        /// MQTT
        /// </summary>
        internal static IMqttManager MqttManager { get; set; }

        /// <summary>
        /// Settings
        /// </summary>
        internal static string CustomExecutorBinary { get; set; } = string.Empty;
    }
}
