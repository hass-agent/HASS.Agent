using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Sensors;

namespace HASS.Agent.Managers.DeviceSensors
{
    internal class AccelerometerSensor : IInternalDeviceSensor
    {
        public const string AttributeAccelerationX = "AccelerationX";
        public const string AttributeAccelerationY = "AccelerationY";
        public const string AttributeAccelerationZ = "AccelerationZ";
        public const string AttributeLastShaken = "LastShaken";

        public string MeasurementType { get; } = string.Empty;
        public string UnitOfMeasurement { get; } = string.Empty;

        private readonly Accelerometer _accelerometer;

        public bool Available => _accelerometer != null;
        public InternalDeviceSensorType Type => InternalDeviceSensorType.Accelerometer;
        public string Measurement
        {
            get
            {
                if (!Available)
                    return null;

                var sensorReading = _accelerometer.GetCurrentReading();
                var accX = Math.Round((decimal)sensorReading.AccelerationX, 2);
                var accY = Math.Round((decimal)sensorReading.AccelerationX, 2);
                var accZ = Math.Round((decimal)sensorReading.AccelerationX, 2);

                _attributes[AttributeAccelerationX] = accX.ToString();
                _attributes[AttributeAccelerationY] = accY.ToString();
                _attributes[AttributeAccelerationZ] = accZ.ToString();

                return (
                    Math.Abs(accX) +
                    Math.Abs(accY) +
                    Math.Abs(accZ)
                ).ToString();
            }
        }

        public bool IsNumeric { get; } = true;

        private readonly Dictionary<string, string> _attributes = new();
        public Dictionary<string, string> Attributes => _attributes;

        public AccelerometerSensor(Accelerometer accelerometer)
        {
            _accelerometer = accelerometer;
            if (_accelerometer != null)
                _accelerometer.Shaken += OnAccelerometerShake;
        }

        private void OnAccelerometerShake(Accelerometer sender, AccelerometerShakenEventArgs args)
        {
            _attributes[AttributeLastShaken] = args.Timestamp.ToLocalTime().ToString();
        }
    }
}
