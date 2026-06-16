using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Management;
using System.Text.RegularExpressions;
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
    private const string AllGpus = "*";

    private static readonly Regex PhysicalGpuIndexRegex = new(@"_phys_(\d+)_", RegexOptions.Compiled);

    public string GpuId { get; protected set; }
    private readonly bool _useSpecificGpu;

    public GpuLoadSensor(string gpuId = AllGpus, int? updateInterval = null, string entityName = DefaultName, string name = DefaultName, string id = default, string advancedSettings = default) : base(entityName ?? DefaultName, name ?? null, updateInterval ?? 30, id, advancedSettings: advancedSettings)
    {
        GpuId = string.IsNullOrEmpty(gpuId) ? AllGpus : gpuId;
        _useSpecificGpu = GpuId != AllGpus;
    }

    public override DiscoveryConfigModel GetAutoDiscoveryConfig()
    {
        if (Variables.MqttManager == null)
            return null;

        var deviceConfig = Variables.MqttManager.GetDeviceConfigModel();
        if (deviceConfig == null)
            return null;

        return AutoDiscoveryConfigModel ?? SetAutoDiscoveryConfigModel(new SensorDiscoveryConfigModel(Domain)
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
            var perGpuUsage = GetPerGpuUsage();
            if (perGpuUsage.Count == 0)
                return 0;

            if (_useSpecificGpu)
                return perGpuUsage.TryGetValue(GpuId, out var usage) ? usage : 0;

            // 'all' selected: average across every detected gpu, instead of summing them together
            return perGpuUsage.Values.Average();
        }
        catch
        {
            return 0;
        }
    }

    /// <summary>
    /// Reads the 'GPU Engine' 3D counters and sums them per physical GPU (identified by its 'phys_n' index)
    /// </summary>
    private static Dictionary<string, float> GetPerGpuUsage()
    {
        var category = new PerformanceCounterCategory("GPU Engine");
        var gpuCounters = category.GetInstanceNames()
            .Where(name => name.EndsWith("engtype_3D"))
            .SelectMany(name => category.GetCounters(name))
            .Where(counter => counter.CounterName.Equals("Utilization Percentage"))
            .ToList();

        gpuCounters.ForEach(x => { _ = x.NextValue(); });
        Thread.Sleep(10); //TODO(Amadeo): fix this

        return gpuCounters
            .GroupBy(x => GetPhysicalGpuIndex(x.InstanceName))
            .ToDictionary(g => g.Key, g => g.Sum(x => x.NextValue()));
    }

    /// <summary>
    /// Extracts the physical adapter index (the 'n' in 'phys_n') from a GPU Engine counter instance name
    /// </summary>
    private static string GetPhysicalGpuIndex(string instanceName)
    {
        var match = PhysicalGpuIndexRegex.Match(instanceName);
        return match.Success ? match.Groups[1].Value : "0";
    }

    /// <summary>
    /// Enumerates the physical GPUs currently exposing a 'GPU Engine' performance counter, keyed by their physical adapter index
    /// </summary>
    public static Dictionary<string, string> GetAvailableGpus()
    {
        var gpus = new Dictionary<string, string>();

        try
        {
            var category = new PerformanceCounterCategory("GPU Engine");
            var physicalIndexes = category.GetInstanceNames()
                .Where(name => name.EndsWith("engtype_3D"))
                .Select(GetPhysicalGpuIndex)
                .Distinct()
                .OrderBy(x => int.TryParse(x, out var parsed) ? parsed : int.MaxValue);

            var gpuNames = GetGpuNamesByIndex();

            foreach (var physicalIndex in physicalIndexes)
            {
                gpus[physicalIndex] = gpuNames.TryGetValue(physicalIndex, out var gpuName) && !string.IsNullOrWhiteSpace(gpuName)
                    ? gpuName
                    : $"GPU {physicalIndex}";
            }
        }
        catch
        {
            // best effort, no gpu's found
        }

        return gpus;
    }

    /// <summary>
    /// Best-effort mapping of physical adapter index to a friendly GPU name, using WMI's video controller enumeration order
    /// </summary>
    private static Dictionary<string, string> GetGpuNamesByIndex()
    {
        var names = new Dictionary<string, string>();

        try
        {
            using var searcher = new ManagementObjectSearcher("SELECT Name, PNPDeviceID FROM Win32_VideoController");
            using var results = searcher.Get();

            var index = 0;
            foreach (var result in results)
            {
                using var videoController = (ManagementObject)result;

                var pnpDeviceId = videoController["PNPDeviceID"]?.ToString() ?? string.Empty;
                if (!pnpDeviceId.StartsWith("PCI", StringComparison.OrdinalIgnoreCase))
                    continue; // skip basic render/remote display virtual adapters

                names[index.ToString(CultureInfo.InvariantCulture)] = videoController["Name"]?.ToString();
                index++;
            }
        }
        catch
        {
            // best effort, no names found
        }

        return names;
    }
}
