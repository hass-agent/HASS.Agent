using HASS.Agent.Shared.HomeAssistant.Sensors.GeneralSensors.SingleValue;

namespace HASS.Agent.Shared.UnitTests.HomeAssistant.Sensors.GeneralSensors.SingleValue;

public class GpuLoadSensorTests
{
    [Test]
    public void DefaultConstructor_SelectsAllGpus()
    {
        var sensor = new GpuLoadSensor();

        Assert.That(sensor.GpuId, Is.EqualTo("*"));
    }

    [TestCase(null)]
    [TestCase("")]
    public void Constructor_NullOrEmptyGpuId_NormalizesToAllGpus(string? gpuId)
    {
        var sensor = new GpuLoadSensor(gpuId!);

        Assert.That(sensor.GpuId, Is.EqualTo("*"));
    }

    [TestCase("0")]
    [TestCase("1")]
    [TestCase("12")]
    public void Constructor_SpecificGpuId_IsPreserved(string gpuId)
    {
        var sensor = new GpuLoadSensor(gpuId);

        Assert.That(sensor.GpuId, Is.EqualTo(gpuId));
    }

    [Test]
    public void SelectGpuUsage_NoGpusDetected_ReturnsZero()
    {
        var result = GpuLoadSensor.SelectGpuUsage(new Dictionary<string, float>(), "*", useSpecificGpu: false);

        Assert.That(result, Is.EqualTo(0));
    }

    [Test]
    public void SelectGpuUsage_AllGpusSelected_AveragesInsteadOfSumming()
    {
        // this is the actual bug being fixed: two gpus at 20% and 80% load should report 50%, not 100%
        var perGpuUsage = new Dictionary<string, float> { ["0"] = 20f, ["1"] = 80f };

        var result = GpuLoadSensor.SelectGpuUsage(perGpuUsage, "*", useSpecificGpu: false);

        Assert.That(result, Is.EqualTo(50f));
    }

    [Test]
    public void SelectGpuUsage_AllGpusSelected_SingleGpuUnaffected()
    {
        var perGpuUsage = new Dictionary<string, float> { ["0"] = 42f };

        var result = GpuLoadSensor.SelectGpuUsage(perGpuUsage, "*", useSpecificGpu: false);

        Assert.That(result, Is.EqualTo(42f));
    }

    [Test]
    public void SelectGpuUsage_SpecificGpuSelected_ReturnsOnlyThatGpusValue()
    {
        var perGpuUsage = new Dictionary<string, float> { ["0"] = 20f, ["1"] = 80f };

        var result = GpuLoadSensor.SelectGpuUsage(perGpuUsage, "1", useSpecificGpu: true);

        Assert.That(result, Is.EqualTo(80f));
    }

    [Test]
    public void SelectGpuUsage_SpecificGpuNoLongerPresent_ReturnsZero()
    {
        var perGpuUsage = new Dictionary<string, float> { ["0"] = 20f };

        var result = GpuLoadSensor.SelectGpuUsage(perGpuUsage, "5", useSpecificGpu: true);

        Assert.That(result, Is.EqualTo(0));
    }

    [TestCase("pid_1234_luid_0x00000000_0x0000C5F2_phys_0_eng_0_engtype_3D", "0")]
    [TestCase("pid_4080_luid_0x00000000_0x0000C8F1_phys_1_eng_2_engtype_3D", "1")]
    [TestCase("pid_999_luid_0x00000000_0x00001234_phys_12_eng_3_engtype_3D", "12")]
    public void GetPhysicalGpuIndex_ParsesPhysIndexFromInstanceName(string instanceName, string expectedIndex)
    {
        var result = GpuLoadSensor.GetPhysicalGpuIndex(instanceName);

        Assert.That(result, Is.EqualTo(expectedIndex));
    }

    [Test]
    public void GetPhysicalGpuIndex_UnrecognizedFormat_FallsBackToZero()
    {
        var result = GpuLoadSensor.GetPhysicalGpuIndex("some_unexpected_counter_instance_name");

        Assert.That(result, Is.EqualTo("0"));
    }

    [Test]
    public void BuildGpuLabels_KnownGpu_UsesFriendlyName()
    {
        var names = new Dictionary<string, string> { ["0"] = "NVIDIA GeForce RTX 3070" };

        var result = GpuLoadSensor.BuildGpuLabels(new[] { "0" }, names);

        Assert.That(result["0"], Is.EqualTo("NVIDIA GeForce RTX 3070"));
    }

    [Test]
    public void BuildGpuLabels_UnknownGpu_FallsBackToGenericLabel()
    {
        var names = new Dictionary<string, string> { ["0"] = "NVIDIA GeForce RTX 3070" };

        var result = GpuLoadSensor.BuildGpuLabels(new[] { "0", "1" }, names);

        Assert.That(result["1"], Is.EqualTo("GPU 1"));
    }

    [Test]
    public void BuildGpuLabels_BlankName_FallsBackToGenericLabel()
    {
        var names = new Dictionary<string, string> { ["0"] = "   " };

        var result = GpuLoadSensor.BuildGpuLabels(new[] { "0" }, names);

        Assert.That(result["0"], Is.EqualTo("GPU 0"));
    }

    [Test]
    public void BuildGpuLabels_NoIndexes_ReturnsEmpty()
    {
        var result = GpuLoadSensor.BuildGpuLabels(System.Array.Empty<string>(), new Dictionary<string, string>());

        Assert.That(result, Is.Empty);
    }

    [Test]
    public void GetGPUUsage_NeverThrowsAndReturnsNonNegativeValue()
    {
        var sensor = new GpuLoadSensor();

        float result = 0;
        Assert.DoesNotThrow(() => result = sensor.GetGPUUsage());
        Assert.That(result, Is.GreaterThanOrEqualTo(0));
    }

    [Test]
    public void GetAvailableGpus_NeverThrowsAndReturnsADictionary()
    {
        Dictionary<string, string>? result = null;
        Assert.DoesNotThrow(() => result = GpuLoadSensor.GetAvailableGpus());
        Assert.That(result, Is.Not.Null);
    }
}
