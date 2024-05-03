using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Sensors;

namespace HASS.Agent.Managers.DeviceSensors
{
    internal class GyrometerSensor : IInternalDeviceSensor
    {
        public const string AttributeAngularVelocityX = "AngularVelocityX";
        public const string AttributeAngularVelocityY = "AngularVelocityY";
        public const string AttributeAngularVelocityZ = "AngularVelocityZ";

        private readonly Gyrometer _gyrometer;

        public string MeasurementType { get; } = string.Empty;
        public string UnitOfMeasurement { get; } = string.Empty;

        public bool Available => _gyrometer != null;
        public InternalDeviceSensorType Type => InternalDeviceSensorType.Gyrometer;
        public string Measurement
        {
            get
            {
                if (!Available)
                    return null;

                var sensorReading = _gyrometer.GetCurrentReading();
                var angVelX = Math.Round((decimal)sensorReading.AngularVelocityX, 2);
                var angVelY = Math.Round((decimal)sensorReading.AngularVelocityY, 2);
                var angVelZ = Math.Round((decimal)sensorReading.AngularVelocityZ, 2);

                _attributes[AttributeAngularVelocityX] = angVelX.ToString();
                _attributes[AttributeAngularVelocityY] = angVelY.ToString();
                _attributes[AttributeAngularVelocityZ] = angVelZ.ToString();

                return (
                    Math.Abs(angVelX) +
                    Math.Abs(angVelY) +
                    Math.Abs(angVelZ)
                ).ToString();
            }
        }

        public bool IsNumeric { get; } = true;

        private readonly Dictionary<string, string> _attributes = new();
        public Dictionary<string, string> Attributes => _attributes;

        public GyrometerSensor(Gyrometer gyrometer)
        {
            _gyrometer = gyrometer;
        }
    }
}
