using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Sensors;

namespace HASS.Agent.Managers.DeviceSensors
{
    internal class HumanPresenceSensor : IInternalDeviceSensor
    {
        public const string AttributeDistanceInMilimeters = "DistanceInMilimeters";
        public const string AttributeEngagement = "Engagement";

        private Windows.Devices.Sensors.HumanPresenceSensor _humanPresenceSensor;

        public string MeasurementType { get; } = string.Empty;
        public string UnitOfMeasurement { get; } = string.Empty;

        public bool Available => _humanPresenceSensor != null;
        public InternalDeviceSensorType Type => InternalDeviceSensorType.ProximitySensor;
        public string Measurement
        {
            get
            {
                if (!Available)
                    return null;

                var sensorReading = _humanPresenceSensor.GetCurrentReading();
                if (sensorReading == null)
                    return null;

                Attributes = new Dictionary<string, string>()
                {
                    [AttributeDistanceInMilimeters] = sensorReading.DistanceInMillimeters.ToString(),
                    [AttributeEngagement] = sensorReading.Engagement.ToString(),
                };

                return (sensorReading.Presence == HumanPresence.Present).ToString();
            }
        }

        public bool IsNumeric { get; } = false;

        public Dictionary<string, string> Attributes { get; private set; }

        public HumanPresenceSensor(Windows.Devices.Sensors.HumanPresenceSensor humanPresenceSensor)
        {
            _humanPresenceSensor = humanPresenceSensor;
        }

        public void UpdateInternalSensor(Windows.Devices.Sensors.HumanPresenceSensor humanPresenceSensor)
        {
            _humanPresenceSensor = humanPresenceSensor;
        }
    }
}
