using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Sensors;

namespace HASS.Agent.Managers.DeviceSensors
{
    internal class BarometerSensor : IInternalDeviceSensor
    {
        private readonly Barometer _barometer;

        public string MeasurementType { get; } = "pressure";
        public string UnitOfMeasurement { get; } = "hPa";

        public bool Available => _barometer != null;
        public InternalDeviceSensorType Type => InternalDeviceSensorType.Barometer;
        public string Measurement
        {
            get
            {
                if (!Available)
                    return null;

                var sensorReading = _barometer.GetCurrentReading();
                if (sensorReading == null)
                    return null;

                return sensorReading.StationPressureInHectopascals.ToString();
            }
        }

        public bool IsNumeric { get; } = true;

        public Dictionary<string, string> Attributes => InternalDeviceSensor.NoAttributes;

        public BarometerSensor(Barometer barometer)
        {
            _barometer = barometer;
        }
    }
}
