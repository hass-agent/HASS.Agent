using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Sensors;

namespace HASS.Agent.Managers.DeviceSensors
{
    internal class HingeAngleSensor : IInternalDeviceSensor
    {
        private readonly Windows.Devices.Sensors.HingeAngleSensor _hingeAngelSensor;

        public string MeasurementType { get; } = string.Empty;
        public string UnitOfMeasurement { get; } = string.Empty;

        public bool Available => _hingeAngelSensor != null;
        public InternalDeviceSensorType Type => InternalDeviceSensorType.HingeAngleSensor;
        public string Measurement
        {
            get
            {
                if (!Available)
                    return null;

                var sensorReading = _hingeAngelSensor.GetCurrentReadingAsync().AsTask().Result.AngleInDegrees;
                return Math.Round((decimal)sensorReading, 2).ToString();
            }
        }

        public bool IsNumeric { get; } = true;

        public Dictionary<string, string> Attributes => InternalDeviceSensor.NoAttributes;

        public HingeAngleSensor(Windows.Devices.Sensors.HingeAngleSensor hingeAngleSensor)
        {
            _hingeAngelSensor = hingeAngleSensor;
        }
    }
}
