using System;
using HASS.Agent.Shared.Models.HomeAssistant;
using Serilog;
using Windows.Foundation.Metadata;

namespace HASS.Agent.Shared.HomeAssistant.Sensors.GeneralSensors.MultiValue.DataTypes
{
    /// <summary>
    /// Generic string sensor
    /// </summary>
    public class DataTypeStringSensor : AbstractSingleValueSensor
    {
        private readonly string _deviceClass;
        private readonly string _unitOfMeasurement;
        private readonly string _icon;

        private string _value = string.Empty;
        private string _attributes = string.Empty;

        public DataTypeStringSensor(int? updateInterval, string entityName, string name, string id, string deviceClass, string icon, string unitOfMeasurement, string multiValueSensorName, bool useAttributes = false) : base(entityName, name, updateInterval ?? 30, id, useAttributes)
        {
            TopicName = multiValueSensorName;

            _deviceClass = deviceClass;
            _unitOfMeasurement = unitOfMeasurement;
            _icon = icon;

            ObjectId = id;
        }

        [Obsolete("Deprecated due to HA 2023.8 MQTT changes in favor of method specifying entityName")]
        public DataTypeStringSensor(int? updateInterval, string name, string id, string deviceClass, string icon, string unitOfMeasurement, string multiValueSensorName, bool useAttributes = false) : base(name, name, updateInterval ?? 30, id, useAttributes)
        {
            TopicName = multiValueSensorName;

            _deviceClass = deviceClass;
            _unitOfMeasurement = unitOfMeasurement;
            _icon = icon;

            ObjectId = id;
        }

        public override DiscoveryConfigModel GetAutoDiscoveryConfig()
        {
            if (AutoDiscoveryConfigModel != null) return AutoDiscoveryConfigModel;

            if (Variables.MqttManager == null) return null;

            var deviceConfig = Variables.MqttManager.GetDeviceConfigModel();
            if (deviceConfig == null) return null;

            var model = new SensorDiscoveryConfigModel()
            {
                EntityName = EntityName,
                Name = Name,
                Unique_id = Id,
                Device = deviceConfig,
                State_topic = $"{Variables.MqttManager.MqttDiscoveryPrefix()}/{Domain}/{deviceConfig.Name}/{TopicName}/{ObjectId}/state",
                Availability_topic = $"{Variables.MqttManager.MqttDiscoveryPrefix()}/{Domain}/{deviceConfig.Name}/availability"
            };

            if (UseAttributes) model.Json_attributes_topic = $"{Variables.MqttManager.MqttDiscoveryPrefix()}/{Domain}/{deviceConfig.Name}/{TopicName}/{ObjectId}/attributes";

            if (!string.IsNullOrWhiteSpace(_deviceClass)) model.Device_class = _deviceClass;
            if (!string.IsNullOrWhiteSpace(_unitOfMeasurement)) model.Unit_of_measurement = _unitOfMeasurement;
            if (!string.IsNullOrWhiteSpace(_icon)) model.Icon = _icon;

            return SetAutoDiscoveryConfigModel(model);
        }

        public void SetState(string value) => _value = value;
        public void SetAttributes(string value) => _attributes = string.IsNullOrWhiteSpace(value) ? "{}" : value;

        public override string GetState() => _value;
        public override string GetAttributes() => _attributes;
    }
}
