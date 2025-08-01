using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HASS.Agent.Managers.DeviceSensors
{
    public enum InternalDeviceSensorType
    {
        Accelerometer = 0,
        ActivitySensor,
        Altimeter,
        Barometer,
        Compass,
        Gyrometer,
        HingeAngleSensor,
        HumanPresenceSensor,
        Inclinometer,
        LightSensor,
        Magnetometer,
        OrientationSensor,
        Pedometer,
        ProximitySensor,
        SimpleOrientationSensor
    }

    internal class InternalDeviceSensor
    {
        internal static readonly Dictionary<string, string> NoAttributes = new();
    }

    internal interface IInternalDeviceSensor
    {
        public bool Available { get; }
        public InternalDeviceSensorType Type { get; }
        public string Measurement { get; }
        public string MeasurementType { get; }
        public string UnitOfMeasurement { get; }
        public bool IsNumeric { get; }
        public Dictionary<string, string> Attributes { get; }
    }
}
