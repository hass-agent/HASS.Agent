using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Sensors;

namespace HASS.Agent.Managers.DeviceSensors
{
    internal class CompassSensor : IInternalDeviceSensor
    {
        public const string AttributeMagneticNorth = "HadingMagneticNorth";

        private readonly Compass _compass;

        public string MeasurementType { get; } = string.Empty;
        public string UnitOfMeasurement { get; } = string.Empty;

        public bool Available => _compass != null;
        public InternalDeviceSensorType Type => InternalDeviceSensorType.Compass;
        public string Measurement
        {
            get
            {
                if (!Available)
                    return null;

                var sensorReading = _compass.GetCurrentReading();
                if (sensorReading == null)
                    return null;

                _attributes[AttributeMagneticNorth] = Math.Round((decimal)sensorReading.HeadingMagneticNorth, 2).ToString();
                return Math.Round((decimal)sensorReading.HeadingTrueNorth, 2).ToString();
            }
        }

        public bool IsNumeric { get; } = true;

        private readonly Dictionary<string, string> _attributes = new();
        public Dictionary<string, string> Attributes => _attributes;

        public CompassSensor(Compass compass)
        {
            _compass = compass;
        }
    }
}
