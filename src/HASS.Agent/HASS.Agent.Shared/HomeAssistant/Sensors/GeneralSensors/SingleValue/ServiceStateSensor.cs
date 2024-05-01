using System;
using System.ServiceProcess;
using HASS.Agent.Shared.Models.HomeAssistant;

namespace HASS.Agent.Shared.HomeAssistant.Sensors.GeneralSensors.SingleValue
{
    /// <summary>
    /// Sensor indicating the current state of the provided service
    /// </summary>
    public class ServiceStateSensor : AbstractSingleValueSensor
    {
        private const string DefaultName = "servicestate";
        public string ServiceName { get; protected set; }

        public ServiceStateSensor(string serviceName, int? updateInterval = null, string entityName = DefaultName, string name = DefaultName, string id = default, string advancedSettings = default) : base(entityName ?? DefaultName, name ?? null, updateInterval ?? 10, id, advancedSettings: advancedSettings)
        {
            ServiceName = serviceName;
        }

        public override DiscoveryConfigModel GetAutoDiscoveryConfig()
        {
            if (Variables.MqttManager == null) return null;

            var deviceConfig = Variables.MqttManager.GetDeviceConfigModel();
            if (deviceConfig == null) return null;

            return AutoDiscoveryConfigModel ?? SetAutoDiscoveryConfigModel(new SensorDiscoveryConfigModel()
            {
                EntityName = EntityName,
                Name = Name,
                Unique_id = Id,
                Device = deviceConfig,
                State_topic = $"{Variables.MqttManager.MqttDiscoveryPrefix()}/{Domain}/{deviceConfig.Name}/{ObjectId}/state",
                Icon = "mdi:file-eye-outline",
                Availability_topic = $"{Variables.MqttManager.MqttDiscoveryPrefix()}/{Domain}/{deviceConfig.Name}/availability"
            });
        }

        public override string GetState()
        {
            try
            {
                using var svc = new ServiceController(ServiceName);
                return svc.Status.ToString();
            }
            catch (InvalidOperationException)
            {
                // service wasn't found
                return "NotFound";
            }
        }

        public override string GetAttributes() => string.Empty;
    }
}
