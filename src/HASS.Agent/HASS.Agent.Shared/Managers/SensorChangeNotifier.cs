using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using HASS.Agent.Shared.Models.HomeAssistant;

namespace HASS.Agent.Shared.Managers
{
    /// <summary>
    /// Lets sensors become 'reactive': instead of waiting for their update interval, they get published
    /// on the next sensor loop iteration after <see cref="NotifyChanged{T}"/> is called for their type.
    /// <para>
    /// Safe to call from any thread (eg. a device watcher or WMI event callback). The actual reset is done by the host's
    /// sensor loop through <see cref="ResetPending"/>, so the sensor lists are only touched by the thread that publishes them.
    /// </para>
    /// </summary>
    public static class SensorChangeNotifier
    {
        private static readonly ConcurrentDictionary<Type, byte> Pending = new();

        /// <summary>
        /// Requests that all sensors of type <typeparamref name="T"/> (or derived from it) are published right away
        /// </summary>
        public static void NotifyChanged<T>() where T : AbstractDiscoverable => NotifyChanged(typeof(T));

        /// <summary>
        /// Requests that all sensors of the provided type (or derived from it) are published right away
        /// </summary>
        public static void NotifyChanged(Type sensorType)
        {
            if (sensorType == null) return;
            Pending.TryAdd(sensorType, 0);
        }

        /// <summary>
        /// Resets the checks of all sensors whose type has been flagged through <see cref="NotifyChanged{T}"/>,
        /// so they get published on the upcoming publish pass. Should be called from the host's sensor loop.
        /// </summary>
        public static void ResetPending(IEnumerable<AbstractSingleValueSensor> singleValueSensors, IEnumerable<AbstractMultiValueSensor> multiValueSensors)
        {
            if (Pending.IsEmpty) return;

            // take the flags before resetting, so a change arriving during the reset gets flagged again
            var types = new List<Type>();
            foreach (var type in Pending.Keys)
            {
                if (Pending.TryRemove(type, out _)) types.Add(type);
            }

            if (types.Count == 0) return;

            if (singleValueSensors != null)
            {
                foreach (var sensor in singleValueSensors.Where(s => types.Any(t => t.IsInstanceOfType(s)))) sensor.ResetChecks();
            }

            if (multiValueSensors != null)
            {
                foreach (var sensor in multiValueSensors.Where(s => types.Any(t => t.IsInstanceOfType(s)))) sensor.ResetChecks();
            }
        }
    }
}
