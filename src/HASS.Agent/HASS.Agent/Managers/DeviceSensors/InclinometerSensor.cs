using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Sensors;

namespace HASS.Agent.Managers.DeviceSensors
{
    internal class InclinometerSensor : IInternalDeviceSensor
    {
        public const string AttributePitchDegrees = "PitchDegrees";
        public const string AttributeYawDegrees = "YawDegrees";
        public const string AttributeRollDegrees = "RollDegrees";

        public const string AttributeYawAccuracy = "YawAccuracy";

        private readonly Inclinometer _inclinometer;

        public string MeasurementType { get; } = string.Empty;
        public string UnitOfMeasurement { get; } = string.Empty;

        public bool Available => _inclinometer != null;
        public InternalDeviceSensorType Type => InternalDeviceSensorType.Inclinometer;
        public string Measurement
        {
            get
            {
                if (!Available)
                    return null;

                var sensorReading = _inclinometer.GetCurrentReading();
                if (sensorReading == null)
                    return null;

                _attributes[AttributePitchDegrees] = Math.Round((decimal)sensorReading.PitchDegrees, 2).ToString();
                _attributes[AttributeYawDegrees] = Math.Round((decimal)sensorReading.YawDegrees, 2).ToString();
                _attributes[AttributeRollDegrees] = Math.Round((decimal)sensorReading.RollDegrees, 2).ToString();

                _attributes[AttributeYawAccuracy] = sensorReading.YawAccuracy.ToString();

                return sensorReading.Timestamp.ToLocalTime().ToString();
            }
        }

        public bool IsNumeric { get; } = false;

        private readonly Dictionary<string, string> _attributes = new();
        public Dictionary<string, string> Attributes => _attributes;

        public InclinometerSensor(Inclinometer inclinometer)
        {
            _inclinometer = inclinometer;
        }
    }
}
