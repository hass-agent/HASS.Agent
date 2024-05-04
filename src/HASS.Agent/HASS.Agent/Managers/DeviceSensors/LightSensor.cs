using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Sensors;

namespace HASS.Agent.Managers.DeviceSensors
{
    internal class LightSensor : IInternalDeviceSensor
    {
        private readonly Windows.Devices.Sensors.LightSensor _lightSensor;

        public string MeasurementType { get; } = "illuminance";
        public string UnitOfMeasurement { get; } = "lx";

        public bool Available => _lightSensor != null;
        public InternalDeviceSensorType Type => InternalDeviceSensorType.LightSensor;
        public string Measurement
        {
            get
            {
                if (!Available)
                    return null;

                return Math.Round((decimal)_lightSensor.GetCurrentReading().IlluminanceInLux, 2).ToString();
            }
        }

        public bool IsNumeric { get; } = true;

        public Dictionary<string, string> Attributes => InternalDeviceSensor.NoAttributes;

        public LightSensor(Windows.Devices.Sensors.LightSensor lightSensor)
        {
            _lightSensor = lightSensor;
        }
    }
}
