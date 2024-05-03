using HASS.Agent.Managers;
using HASS.Agent.Managers.DeviceSensors;
using HASS.Agent.Shared.Extensions;
using HASS.Agent.Shared.Models.HomeAssistant;
using Newtonsoft.Json;

namespace HASS.Agent.HomeAssistant.Sensors.GeneralSensors.SingleValue
{
    /// <summary>
    /// Sensor containing the device's internal sensor data
    /// </summary>
    public class InternalDeviceSensor : AbstractSingleValueSensor
    {
        private const string DefaultName = "internaldevicesensor";

        public InternalDeviceSensorType SensorType { get; set; }

        private readonly IInternalDeviceSensor _internalDeviceSensor;

        public InternalDeviceSensor(string sensorType, int? updateInterval = 10, string entityName = DefaultName, string name = DefaultName, string id = default, string advancedSettings = default) : base(entityName ?? DefaultName, name ?? null, updateInterval ?? 30, id, advancedSettings: advancedSettings)
        {
            SensorType = Enum.Parse<InternalDeviceSensorType>(sensorType);
            _internalDeviceSensor = InternalDeviceSensorsManager.AvailableSensors.First(s => s.Type == SensorType);

            UseAttributes = _internalDeviceSensor.Attributes != Managers.DeviceSensors.InternalDeviceSensor.NoAttributes;
        }

        public override DiscoveryConfigModel GetAutoDiscoveryConfig()
        {
            if (Variables.MqttManager == null)
                return null;

            var deviceConfig = Variables.MqttManager.GetDeviceConfigModel();
            if (deviceConfig == null)
                return null;

            var sensorDiscoveryConfigModel = new SensorDiscoveryConfigModel()
            {
                EntityName = EntityName,
                Name = Name,
                Unique_id = Id,
                Device = deviceConfig,
                State_topic = $"{Variables.MqttManager.MqttDiscoveryPrefix()}/{Domain}/{deviceConfig.Name}/{ObjectId}/state",
                Icon = "mdi:information-box-outline",
                Availability_topic = $"{Variables.MqttManager.MqttDiscoveryPrefix()}/{Domain}/{deviceConfig.Name}/availability"
            };

            if (UseAttributes)
                sensorDiscoveryConfigModel.Json_attributes_topic = $"{Variables.MqttManager.MqttDiscoveryPrefix()}/{Domain}/{deviceConfig.Name}/{ObjectId}/attributes";

            if (!string.IsNullOrWhiteSpace(_internalDeviceSensor.MeasurementType))
                sensorDiscoveryConfigModel.Device_class = _internalDeviceSensor.MeasurementType;
            if (!string.IsNullOrWhiteSpace(_internalDeviceSensor.UnitOfMeasurement))
                sensorDiscoveryConfigModel.Unit_of_measurement = _internalDeviceSensor.UnitOfMeasurement;
            if (_internalDeviceSensor.IsNumeric)
                sensorDiscoveryConfigModel.State_class = "measurement";

            return AutoDiscoveryConfigModel ?? SetAutoDiscoveryConfigModel(sensorDiscoveryConfigModel);
        }

        public override string GetState() => _internalDeviceSensor.Measurement;

        public override string GetAttributes() => JsonConvert.SerializeObject(_internalDeviceSensor.Attributes);
    }
}
