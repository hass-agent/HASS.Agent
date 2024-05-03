using System.Globalization;
using System.Linq;
using HASS.Agent.Shared.Managers;
using HASS.Agent.Shared.Models.HomeAssistant;
using LibreHardwareMonitor.Hardware;

namespace HASS.Agent.Shared.HomeAssistant.Sensors.GeneralSensors.SingleValue
{
    /// <summary>
    /// Sensor containing the current GPU temp
    /// </summary>
    public class GpuTemperatureSensor : AbstractSingleValueSensor
    {
        private const string DefaultName = "gputemperature";
        private readonly IHardware _gpu;

        public GpuTemperatureSensor(int? updateInterval = null, string entityName = DefaultName, string name = DefaultName, string id = default, string advancedSettings = default) : base(entityName ?? DefaultName, name ?? null, updateInterval ?? 30, id, advancedSettings: advancedSettings)
        {
			_gpu = HardwareManager.Hardware.FirstOrDefault(
				h => h.HardwareType == HardwareType.GpuAmd ||
				h.HardwareType == HardwareType.GpuNvidia ||
                h.HardwareType == HardwareType.GpuIntel
			);
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
                Device_class = "temperature",
                Unit_of_measurement = "°C",
                State_class = "measurement",
                Availability_topic = $"{Variables.MqttManager.MqttDiscoveryPrefix()}/{Domain}/{deviceConfig.Name}/availability"
            });
        }

        public override string GetState()
        {
            if (_gpu == null)
                return null;

            _gpu.Update();

            var sensor = _gpu.Sensors.FirstOrDefault(s => s.SensorType == SensorType.Temperature);

            if (sensor?.Value == null)
                return null;

            return sensor.Value.HasValue ? sensor.Value.Value.ToString("#.##", CultureInfo.InvariantCulture) : null;
        }

        public override string GetAttributes() => string.Empty;
    }
}
