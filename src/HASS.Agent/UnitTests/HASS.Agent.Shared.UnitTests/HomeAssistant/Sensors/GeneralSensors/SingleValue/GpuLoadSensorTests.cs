using System.Threading;
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

    [TestCase("pid_1234_luid_0x00000000_0x0000C5F2_phys_0_eng_0_engtype_3D", "0x00000000_0x0000c5f2")]
    [TestCase("pid_4080_luid_0x00000000_0x0000C8F1_phys_1_eng_2_engtype_3D", "0x00000000_0x0000c8f1")]
    // the real-world bug: 'phys_0' for every instance regardless of which adapter it actually belongs to -
    // the luid is what's actually unique here, so it must be what gets extracted, not the phys segment
    [TestCase("pid_2568_luid_0x00000000_0x00016e08_phys_0_eng_0_engtype_3D", "0x00000000_0x00016e08")]
    [TestCase("pid_2568_luid_0x00000000_0x000183dd_phys_0_eng_0_engtype_3D", "0x00000000_0x000183dd")]
    public void GetAdapterLuid_ParsesLuidFromInstanceName(string instanceName, string expectedLuid)
    {
        var result = GpuLoadSensor.GetAdapterLuid(instanceName);

        Assert.That(result, Is.EqualTo(expectedLuid));
    }

    [Test]
    public void GetAdapterLuid_UnrecognizedFormat_FallsBackToFullInstanceName()
    {
        var result = GpuLoadSensor.GetAdapterLuid("some_unexpected_counter_instance_name");

        Assert.That(result, Is.EqualTo("some_unexpected_counter_instance_name"));
    }

    [Test]
    public void GetAdapterLuid_DifferentCasingForSameAdapter_NormalizesToSameValue()
    {
        // the actual bug this fixes: windows doesn't consistently capitalize the luid's hex digits across
        // different processes' counter instances, so two instances for the SAME real adapter could otherwise
        // be grouped as two different "adapters" - and fail to match the lowercase luid GetAvailableGpus() builds
        var lower = GpuLoadSensor.GetAdapterLuid("pid_1_luid_0x00000000_0x00016e08_phys_0_eng_0_engtype_3D");
        var upper = GpuLoadSensor.GetAdapterLuid("pid_2_luid_0x00000000_0x00016E08_phys_0_eng_0_engtype_3D");

        Assert.That(upper, Is.EqualTo(lower));
    }

    [Test]
    public void FilterToKnownAdapters_DropsInstancesFromUnknownAdapter()
    {
        // the actual bug this fixes: GetAvailableGpus() (DXGI) no longer lists a phantom/virtual adapter (eg.
        // WARP), but without this filter its counter instances would still flow into the 'all gpus' average
        var instanceNames = new[]
        {
            "pid_1_luid_0x00000000_0x00016e08_phys_0_eng_0_engtype_3D", // real, known
            "pid_2_luid_0x00000000_0x0001839e_phys_0_eng_0_engtype_3D", // phantom, unknown
        };
        var knownGpuLuids = new[] { "0x00000000_0x00016e08" };

        var result = GpuLoadSensor.FilterToKnownAdapters(instanceNames, knownGpuLuids);

        Assert.That(result, Is.EqualTo(new[] { "pid_1_luid_0x00000000_0x00016e08_phys_0_eng_0_engtype_3D" }));
    }

    [Test]
    public void FilterToKnownAdapters_KnownLuidCasingDiffersFromInstanceName_StillMatches()
    {
        var instanceNames = new[] { "pid_1_luid_0x00000000_0x00016E08_phys_0_eng_0_engtype_3D" };
        var knownGpuLuids = new[] { "0x00000000_0x00016e08" };

        var result = GpuLoadSensor.FilterToKnownAdapters(instanceNames, knownGpuLuids);

        Assert.That(result, Is.EqualTo(instanceNames));
    }

    [Test]
    public void FilterToKnownAdapters_NoKnownAdapters_ReturnsEmpty()
    {
        var instanceNames = new[] { "pid_1_luid_0x00000000_0x00016e08_phys_0_eng_0_engtype_3D" };

        var result = GpuLoadSensor.FilterToKnownAdapters(instanceNames, Enumerable.Empty<string>());

        Assert.That(result, Is.Empty);
    }

    [Test]
    public void AggregateUsageByAdapter_GroupsByLuidNotPhysIndex()
    {
        // two different processes on the SAME adapter (same luid, both 'phys_0') must be summed together,
        // while a process on a DIFFERENT adapter (different luid, also 'phys_0') must stay separate
        var samples = new[]
        {
            ("pid_1_luid_0x00000000_0x00016e08_phys_0_eng_0_engtype_3D", 10f),
            ("pid_2_luid_0x00000000_0x00016e08_phys_0_eng_0_engtype_3D", 5f),
            ("pid_3_luid_0x00000000_0x000183dd_phys_0_eng_0_engtype_3D", 20f),
        };

        var result = GpuLoadSensor.AggregateUsageByAdapter(samples);

        Assert.That(result.Count, Is.EqualTo(2));
        Assert.That(result.Values, Has.Member(15f)); // 10 + 5, same adapter
        Assert.That(result.Values, Has.Member(20f));
    }

    [Test]
    public void AggregateUsageByAdapter_KeysResultByRawLuid()
    {
        var samples = new[]
        {
            ("pid_1_luid_0x00000000_0x000183dd_phys_0_eng_0_engtype_3D", 99f),
            ("pid_2_luid_0x00000000_0x00016e08_phys_0_eng_0_engtype_3D", 11f),
        };

        var result = GpuLoadSensor.AggregateUsageByAdapter(samples);

        // keyed directly by the adapter's luid - the same id GetAvailableGpus() uses, so no separate
        // index-matching scheme is needed to look up which gpu a given sample's value belongs to
        Assert.That(result["0x00000000_0x00016e08"], Is.EqualTo(11f));
        Assert.That(result["0x00000000_0x000183dd"], Is.EqualTo(99f));
    }

    [Test]
    public void AggregateUsageByAdapter_NoSamples_ReturnsEmpty()
    {
        var result = GpuLoadSensor.AggregateUsageByAdapter(Enumerable.Empty<(string, float)>());

        Assert.That(result, Is.Empty);
    }

    [Test]
    public void GetState_SpecificGpuNotPresent_ReturnsZeroNotEmptyString()
    {
        // regression test: ToString("#.##") on exactly 0 produces an empty string in .NET, not "0" - this
        // exercises the real GetState()/GetGPUUsage() path with a gpu id that can never match real hardware,
        // so SelectGpuUsage deterministically falls through to 0 regardless of what's actually installed
        var sensor = new GpuLoadSensor("999");

        Assert.That(sensor.GetState(), Is.EqualTo("0"));
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

    // exercises the real 'GPU Engine' performance counters on whatever machine runs this test, rather than synthetic data - skips instead of failing on a machine with no gpu counters
    [Test]
    [Category("Hardware")]
    public void GetAvailableGpus_OnThisMachine_EachGpuReportsAPlausibleLoad()
    {
        var sleep = 1000;
        var gpus = GpuLoadSensor.GetAvailableGpus();
        if (gpus.Count == 0)
        {
            Assert.Ignore("No 'GPU Engine' performance counters were detected on this machine.");
            return;
        }

        TestContext.Out.WriteLine($"Detected {gpus.Count} GPU(s):");

        // GpuLoadSensor caches its counters across calls, so the first read on a freshly-created sensor only primes them and carries little meaning on its own.
        // Read once to prime, wait for a real time window to pass, then read again, since percentage-style performance counters need a meaningful delta between samples to be accurate.
        var sensors = gpus.Select(gpu => (gpu.Key, gpu.Value, Sensor: new GpuLoadSensor(gpu.Key))).ToList();
        foreach (var (_, _, sensor) in sensors)
            sensor.GetGPUUsage();

        Thread.Sleep(sleep);

        foreach (var (gpuId, gpuName, sensor) in sensors)
        {
            var usage = sensor.GetGPUUsage();
            var state = sensor.GetState();

            TestContext.Out.WriteLine($"  [{gpuId}] {gpuName}: {state}% (raw value: {usage})");

            Assert.That(usage, Is.GreaterThanOrEqualTo(0).And.LessThanOrEqualTo(100),
                $"GPU '{gpuName}' (id {gpuId}) reported an out-of-range load: {usage}");
        }

        var allGpus = new GpuLoadSensor();
        allGpus.GetGPUUsage();
        Thread.Sleep(sleep);
        var averageUsage = allGpus.GetGPUUsage();
        TestContext.Out.WriteLine($"  [*] All GPUs (average): {allGpus.GetState()}% (raw value: {averageUsage})");

        Assert.That(averageUsage, Is.GreaterThanOrEqualTo(0).And.LessThanOrEqualTo(100));
    }
}
