using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Sensors;

namespace HASS.Agent.Managers.DeviceSensors
{
    internal class MagnetometerSensor : IInternalDeviceSensor
    {
        public const string AttributeMagneticFieldX = "MagneticFieldX";
        public const string AttributeMagneticFieldY = "MagneticFieldY";
        public const string AttributeMagneticFieldZ = "MagneticFieldZ";

        private readonly Magnetometer _magnetometer;

        public string MeasurementType { get; } = string.Empty;
        public string UnitOfMeasurement { get; } = string.Empty;

        public bool Available => _magnetometer != null;
        public InternalDeviceSensorType Type => InternalDeviceSensorType.Magnetometer;
        public string Measurement
        {
            get
            {
                if (!Available)
                    return null;

                var sensorReading = _magnetometer.GetCurrentReading();
                if (sensorReading == null)
                    return null;

                var magFieldX = Math.Round((decimal)sensorReading.MagneticFieldX,2);
                var magFieldY = Math.Round((decimal)sensorReading.MagneticFieldY, 2);
                var magFieldZ = Math.Round((decimal)sensorReading.MagneticFieldZ, 2);

                _attributes[AttributeMagneticFieldX] = magFieldX.ToString();
                _attributes[AttributeMagneticFieldY] = magFieldY.ToString();
                _attributes[AttributeMagneticFieldZ] = magFieldZ.ToString();

                return (
                    Math.Abs(magFieldX) +
                    Math.Abs(magFieldY) +
                    Math.Abs(magFieldZ)
                ).ToString();
            }
        }

        public bool IsNumeric { get; } = true;

        private readonly Dictionary<string, string> _attributes = new();
        public Dictionary<string, string> Attributes => _attributes;

        public MagnetometerSensor(Magnetometer magnetometer)
        {
            _magnetometer = magnetometer;
        }
    }
}
