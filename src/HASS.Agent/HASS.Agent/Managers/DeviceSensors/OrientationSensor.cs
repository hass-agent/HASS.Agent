using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Sensors;

namespace HASS.Agent.Managers.DeviceSensors
{
    internal class OrientationSensor : IInternalDeviceSensor
    {
        public const string AttributeRotationMatrix = "RotationMatrix";
        public const string AttributeYawAccuracy = "YawAccuracy";

        private readonly Windows.Devices.Sensors.OrientationSensor _orientationSensor;

        public string MeasurementType { get; } = string.Empty;
        public string UnitOfMeasurement { get; } = string.Empty;

        public bool Available => _orientationSensor != null;
        public InternalDeviceSensorType Type => InternalDeviceSensorType.OrientationSensor;
        public string Measurement
        {
            get
            {
                if (!Available)
                    return null;

                var sensorReading = _orientationSensor.GetCurrentReading();
                _attributes[AttributeRotationMatrix] = JsonConvert.SerializeObject(sensorReading.RotationMatrix);
                _attributes[AttributeYawAccuracy] = sensorReading.YawAccuracy.ToString();

                return JsonConvert.SerializeObject(sensorReading.Quaternion);
            }
        }

        public bool IsNumeric { get; } = false;

        private readonly Dictionary<string, string> _attributes = new();
        public Dictionary<string, string> Attributes => _attributes;

        public OrientationSensor(Windows.Devices.Sensors.OrientationSensor orientationSensor)
        {
            _orientationSensor = orientationSensor;
        }
    }
}
