using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using HASS.Agent.Shared.Models.HomeAssistant;
using Vanara.PInvoke;
using static Vanara.PInvoke.DXGI;

namespace HASS.Agent.Shared.HomeAssistant.Sensors.GeneralSensors.SingleValue;

/// <summary>
/// Sensor indicating the current GPU load
/// </summary>
public class GpuLoadSensor : AbstractSingleValueSensor
{
    /// <summary>
    /// The default entity name and friendly name for this sensor
    /// </summary>
    private const string DefaultName = "gpuload";

    /// <summary>
    /// The special value for <see cref="GpuId"/> that indicates the sensor should average across all detected GPUs, rather than reporting on a specific adapter
    /// </summary>
    private const string AllGpus = "*";

    /// <summary>
    /// The regex used to extract the adapter luid from a GPU Engine counter instance name (eg. 'luid_0x00000000_0x00016e08_engtype_3D').
    /// </summary>
    private static readonly Regex AdapterLuidRegex = new(@"luid_(0x[0-9A-Fa-f]+_0x[0-9A-Fa-f]+)", RegexOptions.Compiled);

    /// <summary>
    /// The adapter luid (eg. '0x00000000_0x00016e08') to report on, or '*' to average across every detected gpu
    /// </summary>
    public string GpuId { get; protected set; }

    /// <summary>
    /// Indicates whether the sensor is reporting on a specific adapter (true) or averaging across all detected adapters (false)
    /// </summary>
    private readonly bool _useSpecificGpu;

    /// <summary>
    /// The cached 'GPU Engine' counters.
    /// </summary>
    private readonly Dictionary<string, PerformanceCounter> _engineCounterCache = new();

    /// <summary>
    /// Creates a new gpu load sensor
    /// </summary>
    /// <param name="gpuId">The adapter luid (eg. '0x00000000_0x00016e08') to report on, or '*' to average across every detected gpu</param>
    /// <param name="updateInterval">How often, in seconds, the sensor's value is refreshed</param>
    /// <param name="entityName">The entity's unique name, used in its mqtt topic</param>
    /// <param name="name">The entity's friendly (display) name</param>
    /// <param name="id">The entity's unique id</param>
    /// <param name="advancedSettings">Serialized advanced settings (device class, unit of measurement, state class) overriding the auto discovery config</param>
    public GpuLoadSensor(string gpuId = AllGpus, int? updateInterval = null, string entityName = DefaultName, string name = DefaultName, string id = default, string advancedSettings = default) : base(entityName ?? DefaultName, name ?? null, updateInterval ?? 30, id, advancedSettings: advancedSettings)
    {
        GpuId = string.IsNullOrEmpty(gpuId) ? AllGpus : gpuId;
        _useSpecificGpu = GpuId != AllGpus;
    }

    ///<inheritdoc/>
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

    ///<inheritdoc/>
    public override string GetState()
    {
        return GetGPUUsage().ToString("0.##", CultureInfo.InvariantCulture);
    }

    ///<inheritdoc/>
    public override string GetAttributes() => string.Empty;

    /// <summary>
    /// Returns the current GPU usage, either for a specific adapter (if <see cref="GpuId"/> is set to a luid) or averaged across all detected adapters (if <see cref="GpuId"/> is set to '*')
    /// </summary>
    /// <returns>The current GPU usage as a float value</returns>
    public float GetGPUUsage()
    {
        try
        {
            return SelectGpuUsage(GetPerGpuUsage(), GpuId, _useSpecificGpu);
        }
        catch
        {
            return 0;
        }
    }

    /// <summary>
    /// Pure selection logic: picks a single GPU's usage, or averages across all of them when none is specified
    /// </summary>
    internal static float SelectGpuUsage(IReadOnlyDictionary<string, float> perGpuUsage, string gpuId, bool useSpecificGpu)
    {
        if (perGpuUsage.Count == 0)
            return 0;

        if (useSpecificGpu)
            return perGpuUsage.TryGetValue(gpuId, out var usage) ? usage : 0;

        return perGpuUsage.Values.Average();
    }

    /// <summary>
    /// Reads the 'GPU Engine' 3D counters and sums them per physical adapter (identified by its luid).
    /// Counter objects are kept in <see cref="_engineCounterCache"/> across calls (instances come and go as processes start/stop using the gpu) so a still-running process's counter diffs against its previous real reading - only a process seen for the first time needs a one-off throwaway priming read
    /// </summary>
    private Dictionary<string, float> GetPerGpuUsage()
    {
        // Get the list of current GPU Engine counters
        var category = new PerformanceCounterCategory("GPU Engine");
        var instanceNames = FilterToKnownAdapters(
            category.GetInstanceNames().Where(name => name.EndsWith("engtype_3D")),
            GetAvailableGpus().Keys
        ).ToList();

        //Remove any stale counters from the cache (eg. a process that was using the GPU but has since exited)
        foreach (var staleInstanceName in _engineCounterCache.Keys.Except(instanceNames).ToList())
        {
            _engineCounterCache[staleInstanceName].Dispose();
            _engineCounterCache.Remove(staleInstanceName);
        }

        //Add any new counters to the cache and do a throwaway read to prime them (otherwise their first real reading will be 0)
        var newlySeenCounters = new List<PerformanceCounter>();
        foreach (var instanceName in instanceNames)
        {
            if (_engineCounterCache.ContainsKey(instanceName))
                continue;

            var counter = category.GetCounters(instanceName).FirstOrDefault(c => c.CounterName.Equals("Utilization Percentage"));
            if (counter == null)
                continue;

            _engineCounterCache[instanceName] = counter;
            newlySeenCounters.Add(counter);
        }

        newlySeenCounters.ForEach(x => { _ = x.NextValue(); });

        // Read the current values of all cached counters and aggregate them by adapter
        var samples = _engineCounterCache.Select(x => (InstanceName: x.Key, Value: x.Value.NextValue()));
        return AggregateUsageByAdapter(samples);
    }

    /// <summary>
    /// Extracts the adapter luid (eg. '0x00000000_0x00016e08') from a GPU Engine counter instance name, falling back to the full instance name if it doesn't match the expected format.
    /// </summary>
    /// <remarks>
    /// Lowercased because Windows doesn't consistently capitalize the hex digits across different instance names for the same adapter, and this must match <see cref="FormatLuid"/>'s output exactly
    /// </remarks>
    internal static string GetAdapterLuid(string instanceName)
    {
        var match = AdapterLuidRegex.Match(instanceName);
        return match.Success ? match.Groups[1].Value.ToLowerInvariant() : instanceName;
    }

    /// <summary>
    /// Pure filter logic: keeps only the counter instances belonging to a known real adapter, dropping ones from a phantom/virtual adapter (eg. WARP, or an indirect display driver) that <see cref="GetAvailableGpus"/> doesn't know about. 
    /// Otherwise an unselectable phantom adapter could still silently skew the 'all gpus' average.
    /// </summary>
    internal static IEnumerable<string> FilterToKnownAdapters(IEnumerable<string> instanceNames, IEnumerable<string> knownGpuLuids)
    {
        var knownSet = knownGpuLuids is ISet<string> set ? set : new HashSet<string>(knownGpuLuids);
        return instanceNames.Where(name => knownSet.Contains(GetAdapterLuid(name)));
    }

    /// <summary>
    /// Pure aggregation logic: sums each sample's value per adapter, grouped by its luid (the same luid <see cref="GetAvailableGpus"/> uses as the GpuId, so no separate index-matching scheme is needed)
    /// </summary>
    internal static Dictionary<string, float> AggregateUsageByAdapter(IEnumerable<(string InstanceName, float Value)> samples)
    {
        return samples
            .GroupBy(s => GetAdapterLuid(s.InstanceName))
            .ToDictionary(g => g.Key, g => g.Sum(s => s.Value));
    }

    /// <summary>
    /// Enumerates the known physical GPUs (excluding the WARP/Microsoft Basic Render software rasterizer) via DXGI, keyed by their adapter luid. 
    /// DXGI lists every installed adapter unconditionally, active or idle, so no separate 'GPU Engine' counter pass is needed to make an unused GPU (eg. an idle iGPU) selectable
    /// </summary>
    public static Dictionary<string, string> GetAvailableGpus()
    {
        var gpus = new Dictionary<string, string>();

        IDXGIFactory1 factory = null;
        try
        {
            var factoryResult = CreateDXGIFactory1(typeof(IDXGIFactory1).GUID, out var factoryObj);
            if (factoryResult.Failed || factoryObj is not IDXGIFactory1 dxgiFactory)
                return gpus;

            factory = dxgiFactory;

            for (uint adapterIndex = 0; ; adapterIndex++)
            {
                IDXGIAdapter1 adapter = null;
                try
                {
                    if (factory.EnumAdapters1(adapterIndex, out adapter).Failed || adapter == null)
                        break;

                    var desc = adapter.GetDesc1();
                    if (desc.Flags.HasFlag(DXGI_ADAPTER_FLAG.DXGI_ADAPTER_FLAG_SOFTWARE))
                        continue;

                    var luid = FormatLuid(desc.AdapterLuid);
                    gpus[luid] = string.IsNullOrWhiteSpace(desc.Description) ? $"GPU {luid}" : desc.Description;
                }
                finally
                {
                    if (adapter != null)
                        Marshal.ReleaseComObject(adapter);
                }
            }
        }
        catch
        {
            // best effort, no gpus found
        }
        finally
        {
            if (factory != null)
                Marshal.ReleaseComObject(factory);
        }

        return gpus;
    }

    /// <summary>
    /// Formats a DXGI LUID exactly as it appears inside a 'GPU Engine' counter instance name (eg. '0x00000000_0x00016e08').
    /// Lowercase 'x8' to match <see cref="GetAdapterLuid"/>'s normalization - the two must always agree
    /// </summary>
    private static string FormatLuid(LUID luid) => $"0x{(uint)luid.HighPart:x8}_0x{luid.LowPart:x8}";
}
