using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using HASS.Agent.Shared.Managers;
using HASS.Agent.Shared.Models.HomeAssistant;

namespace HASS.Agent.Shared.HomeAssistant.Sensors.GeneralSensors.SingleValue;

/// <summary>
/// Sensor indicating the current GPU load
/// </summary>
public class GpuLoadSensor : AbstractSingleValueSensor
{
    private const string DefaultName = "gpuload";

    public GpuLoadSensor(int? updateInterval = null, string entityName = DefaultName, string name = DefaultName, string id = default, string advancedSettings = default) : base(entityName ?? DefaultName, name ?? null, updateInterval ?? 30, id, advancedSettings: advancedSettings)
    {

    }

    public override DiscoveryConfigModel GetAutoDiscoveryConfig()
    {
        if (Variables.MqttManager == null)
            return null;

        var deviceConfig = Variables.MqttManager.GetDeviceConfigModel();
        if (deviceConfig == null)
            return null;

        return AutoDiscoveryConfigModel ?? SetAutoDiscoveryConfigModel(new SensorDiscoveryConfigModel()
        {
            EntityName = EntityName,
            Name = Name,
            Unique_id = Id,
            Device = deviceConfig,
            State_topic = $"{Variables.MqttManager.MqttDiscoveryPrefix()}/{Domain}/{deviceConfig.Name}/{ObjectId}/state",
            Unit_of_measurement = "%",
            State_class = "measurement",
            Availability_topic = $"{Variables.MqttManager.MqttDiscoveryPrefix()}/{Domain}/{deviceConfig.Name}/availability"
        });
    }

    public override string GetState()
    {
        return GetGPUUsage().ToString("#.##", CultureInfo.InvariantCulture);
    }

    public override string GetAttributes() => string.Empty;

    public float GetGPUUsage()
    {
        try
        {
            var category = new PerformanceCounterCategory("GPU Engine");
            var gpuCounters = category.GetInstanceNames()
                .Where(name => name.EndsWith("engtype_3D"))
                .SelectMany(name => category.GetCounters(name))
                .Where(counter => counter.CounterName.Equals("Utilization Percentage"))
                .ToList();

            gpuCounters.ForEach(x => { _ = x.NextValue(); });
            Thread.Sleep(10);

            return gpuCounters.Sum(x => x.NextValue());
        }
        catch
        {
            return 0;
        }
    }
}
