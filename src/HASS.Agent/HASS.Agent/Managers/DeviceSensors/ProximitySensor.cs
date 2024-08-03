using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Sensors;

namespace HASS.Agent.Managers.DeviceSensors
{
    internal class ProximitySensor : IInternalDeviceSensor
    {
        private Windows.Devices.Sensors.ProximitySensor _proximitySensor;

        public string MeasurementType { get; } = "distance";
        public string UnitOfMeasurement { get; } = "mm";

        public bool Available => _proximitySensor != null;
        public InternalDeviceSensorType Type => InternalDeviceSensorType.ProximitySensor;
        public string Measurement
        {
            get
            {
                if (!Available)
                    return null;

                var sensorReading = _proximitySensor.GetCurrentReading();
                if (sensorReading == null)
                    return null;

                return sensorReading.DistanceInMillimeters.ToString();
            }
        }

        public bool IsNumeric { get; } = true;

        public Dictionary<string, string> Attributes => InternalDeviceSensor.NoAttributes;

        public ProximitySensor(Windows.Devices.Sensors.ProximitySensor proximitySensor)
        {
            _proximitySensor = proximitySensor;
        }

        public void UpdateInternalSensor(Windows.Devices.Sensors.ProximitySensor proximitySensor)
        {
            _proximitySensor = proximitySensor;
        }
    }
}
