using HASS.Agent.Settings;
using HASS.Agent.Shared.Enums;
using HASS.Agent.Shared.HomeAssistant.Sensors.GeneralSensors.SingleValue;
using HASS.Agent.Shared.Models.Config;

namespace HASS.Agent.UnitTests.Settings;

public class StoredSensorsTests
{
    [TestCase("*")]
    [TestCase("0")]
    [TestCase("1")]
    public void ConvertConfiguredToAbstractSingleValue_GpuLoadSensor_PassesQueryThroughAsGpuId(string query)
    {
        var configured = new ConfiguredSensor
        {
            Type = SensorType.GpuLoadSensor,
            Id = Guid.NewGuid(),
            EntityName = "gpuload",
            Name = "GPU Load",
            UpdateInterval = 30,
            Query = query
        };

        var sensor = StoredSensors.ConvertConfiguredToAbstractSingleValue(configured);

        Assert.That(sensor, Is.InstanceOf<GpuLoadSensor>());
        Assert.That(((GpuLoadSensor)sensor).GpuId, Is.EqualTo(query));
    }

    [Test]
    public void ConvertConfiguredToAbstractSingleValue_GpuLoadSensor_EmptyQuery_DefaultsToAllGpus()
    {
        // sensors configured before per-gpu selection existed have an empty Query - they should keep working as 'all'
        var configured = new ConfiguredSensor
        {
            Type = SensorType.GpuLoadSensor,
            Id = Guid.NewGuid(),
            EntityName = "gpuload",
            Name = "GPU Load",
            UpdateInterval = 30,
            Query = string.Empty
        };

        var sensor = StoredSensors.ConvertConfiguredToAbstractSingleValue(configured);

        Assert.That(((GpuLoadSensor)sensor).GpuId, Is.EqualTo("*"));
    }

    [Test]
    public void ConvertConfiguredToAbstractSingleValue_GpuLoadSensor_PassesThroughOtherFields()
    {
        var id = Guid.NewGuid();
        var configured = new ConfiguredSensor
        {
            Type = SensorType.GpuLoadSensor,
            Id = id,
            EntityName = "my_gpu",
            Name = "My GPU",
            UpdateInterval = 45,
            Query = "0"
        };

        var sensor = StoredSensors.ConvertConfiguredToAbstractSingleValue(configured);

        Assert.That(sensor.Id, Is.EqualTo(id.ToString()));
        Assert.That(sensor.EntityName, Is.EqualTo("my_gpu"));
        Assert.That(sensor.Name, Is.EqualTo("My GPU"));
        Assert.That(sensor.UpdateIntervalSeconds, Is.EqualTo(45));
    }
}
