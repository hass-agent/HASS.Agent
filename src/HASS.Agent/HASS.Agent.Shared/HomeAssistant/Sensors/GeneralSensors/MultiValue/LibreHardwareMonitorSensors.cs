using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using HASS.Agent.Shared.Functions;
using HASS.Agent.Shared.HomeAssistant.Sensors.GeneralSensors.MultiValue.DataTypes;
using HASS.Agent.Shared.Managers;
using HASS.Agent.Shared.Models.HomeAssistant;
using Newtonsoft.Json.Linq;
using Serilog;
#pragma warning disable CS1591

namespace HASS.Agent.Shared.HomeAssistant.Sensors.GeneralSensors.MultiValue;

/// <summary>
/// Multivalue sensor containing the sensors published by a LibreHardwareMonitor web server endpoint
/// </summary>
public class LibreHardwareMonitorSensors : AbstractMultiValueSensor
{
    private const string DefaultName = "librehardwaremonitor";
    private const string DefaultEndpointUrl = "http://localhost:8085/data.json";

    /// <summary>
    /// Suffix of the entity reporting whether the endpoint is reachable
    /// </summary>
    public const string StatusEntitySuffix = "_status";

    private const string StateClass = "measurement";
    private const string StatusOk = "ok";
    private const string StatusUnreachable = "unreachable";

    /// <summary>
    /// Maps a LibreHardwareMonitor sensor type onto its Home Assistant device class and icon.
    /// Throughput takes its value from 'RawValue' because 'Value' switches between KB/s and MB/s
    /// as the rate changes, while an entity's unit is fixed when it's announced to Home Assistant.
    /// </summary>
    private static readonly Dictionary<string, SensorTypeMapping> TypeMappings = new(StringComparer.OrdinalIgnoreCase)
    {
        { "Clock", new SensorTypeMapping("frequency", "mdi:speedometer") },
        { "Control", new SensorTypeMapping(string.Empty, "mdi:fan") },
        { "Current", new SensorTypeMapping("current", "mdi:current-dc") },
        { "Data", new SensorTypeMapping("data_size", "mdi:harddisk") },
        { "Factor", new SensorTypeMapping(string.Empty, "mdi:numeric") },
        { "Fan", new SensorTypeMapping(string.Empty, "mdi:fan") },
        { "Level", new SensorTypeMapping(string.Empty, "mdi:water-percent") },
        { "Load", new SensorTypeMapping(string.Empty, "mdi:gauge") },
        { "Power", new SensorTypeMapping("power", "mdi:flash") },
        { "SmallData", new SensorTypeMapping("data_size", "mdi:memory") },
        { "Temperature", new SensorTypeMapping("temperature", "mdi:thermometer") },
        { "Throughput", new SensorTypeMapping("data_rate", "mdi:swap-vertical", true) },
        { "Timing", new SensorTypeMapping(string.Empty, "mdi:timer-outline") },
        { "Voltage", new SensorTypeMapping("voltage", "mdi:sine-wave") }
    };

    private static readonly SensorTypeMapping UnknownTypeMapping = new(string.Empty, "mdi:chip");

    // matches a number followed by an optional unit, eg. '45.0 C', '1679 RPM' or '40.500'
    private static readonly Regex ValuePattern = new(@"^\s*(-?[0-9.,]+)\s*(.*?)\s*$", RegexOptions.Compiled);

    private readonly int _updateInterval;
    private readonly HashSet<string> _typeFilter;

    public string EndpointUrl { get; protected set; }
    public string SensorTypes { get; protected set; }

    public override sealed Dictionary<string, AbstractSingleValueSensor> Sensors { get; protected set; } = new Dictionary<string, AbstractSingleValueSensor>();

    public LibreHardwareMonitorSensors(int? updateInterval = null, string entityName = DefaultName, string name = DefaultName, string endpointUrl = DefaultEndpointUrl, string sensorTypes = "", string id = default) : base(entityName ?? DefaultName, name ?? null, updateInterval ?? 30, id)
    {
        _updateInterval = updateInterval ?? 30;

        EndpointUrl = string.IsNullOrWhiteSpace(endpointUrl) ? DefaultEndpointUrl : endpointUrl;
        SensorTypes = sensorTypes ?? string.Empty;
        _typeFilter = ParseTypeFilter(SensorTypes);

        UpdateSensorValues();
    }

    private void AddUpdateSensor(string sensorId, AbstractSingleValueSensor sensor)
    {
        if (!Sensors.ContainsKey(sensorId))
            Sensors.Add(sensorId, sensor);
        else
            Sensors[sensorId] = sensor;
    }

    public override sealed void UpdateSensorValues()
    {
        var parentSensorSafeName = SharedHelperFunctions.GetSafeValue(EntityName);

        if (!HttpJsonManager.TryFetch(EndpointUrl, out var document, out var error))
        {
            // the existing sensors are kept, so their last known values stay available while the endpoint is down
            Log.Warning("[LIBREHARDWAREMONITOR] [{name}] Endpoint '{url}' unavailable: {err}", EntityName, EndpointUrl, error);

            SetStatusSensor(parentSensorSafeName, StatusUnreachable);
            return;
        }

        var readings = new List<Reading>();
        CollectReadings(document, new List<string>(), readings);

        SetReadingNames(readings);

        foreach (var reading in readings)
        {
            var safeSensorId = SharedHelperFunctions.GetSafeValue(reading.SensorId.Trim('/').Replace('/', '_'));
            var sensorId = $"{Id}_{safeSensorId}";

            // reuse an existing sensor so its change-detection isn't reset on every update
            if (Sensors.TryGetValue(sensorId, out var knownSensor) && knownSensor is DataTypeDoubleSensor knownDoubleSensor)
            {
                knownDoubleSensor.SetState(reading.Value);
                continue;
            }

            var mapping = GetTypeMapping(reading.Type);
            var entityName = $"{parentSensorSafeName}_{safeSensorId}";

            var sensor = new DataTypeDoubleSensor(_updateInterval, entityName, reading.Name, sensorId, mapping.DeviceClass, StateClass, mapping.Icon, reading.Unit, EntityName);
            sensor.SetState(reading.Value);

            AddUpdateSensor(sensorId, sensor);
        }

        SetStatusSensor(parentSensorSafeName, StatusOk);
    }

    /// <summary>
    /// Walks the sensor tree, collecting every leaf that carries a usable value
    /// </summary>
    /// <param name="node"></param>
    /// <param name="ancestors"></param>
    /// <param name="readings"></param>
    private void CollectReadings(JToken node, List<string> ancestors, ICollection<Reading> readings)
    {
        var sensorId = node.Value<string>("SensorId");
        if (!string.IsNullOrWhiteSpace(sensorId))
        {
            var reading = CreateReading(node, sensorId, ancestors);
            if (reading != null)
                readings.Add(reading);
        }

        if (node["Children"] is not JArray children)
            return;

        ancestors.Add(node.Value<string>("Text") ?? string.Empty);

        foreach (var child in children)
            CollectReadings(child, ancestors, readings);

        ancestors.RemoveAt(ancestors.Count - 1);
    }

    private Reading CreateReading(JToken node, string sensorId, IReadOnlyList<string> ancestors)
    {
        var sensorType = node.Value<string>("Type") ?? string.Empty;

        if (_typeFilter.Count > 0 && !_typeFilter.Contains(sensorType))
            return null;

        var mapping = GetTypeMapping(sensorType);
        var rawValue = node.Value<string>(mapping.UseRawValue ? "RawValue" : "Value");

        // skips the sensors reporting 'NaN' or '-', which have no value to publish
        if (!TryParseValue(rawValue, out var value, out var unit))
            return null;

        return new Reading
        {
            SensorId = sensorId,
            Hardware = ancestors.Count >= 2 ? ancestors[ancestors.Count - 2] : string.Empty,
            Text = node.Value<string>("Text") ?? string.Empty,
            Type = sensorType,
            Value = value,
            Unit = unit
        };
    }

    private static bool TryParseValue(string rawValue, out double value, out string unit)
    {
        value = 0d;
        unit = string.Empty;

        if (string.IsNullOrWhiteSpace(rawValue))
            return false;

        var match = ValuePattern.Match(rawValue);
        if (!match.Success)
            return false;

        // LibreHardwareMonitor formats its values using the culture it runs under, normally ours as well
        var number = match.Groups[1].Value;
        if (!double.TryParse(number, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.CurrentCulture, out value)
            && !double.TryParse(number, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out value))
            return false;

        if (!double.IsFinite(value))
            return false;

        unit = match.Groups[2].Value;
        return true;
    }

    /// <summary>
    /// Names every reading after the hardware it belongs to, keeping the names unique
    /// </summary>
    /// <param name="readings"></param>
    private static void SetReadingNames(IReadOnlyCollection<Reading> readings)
    {
        foreach (var reading in readings)
            reading.Name = BuildReadingName(reading, false);

        // identical hardware models produce identical names, so those get their instance from the sensor id
        var duplicates = readings.GroupBy(reading => reading.Name).Where(group => group.Count() > 1).SelectMany(group => group).ToList();

        foreach (var reading in duplicates)
            reading.Name = BuildReadingName(reading, true);
    }

    private static string BuildReadingName(Reading reading, bool includeInstance)
    {
        var hardware = includeInstance ? $"{reading.Hardware} ({GetHardwareInstance(reading.SensorId)})" : reading.Hardware;

        // the type isn't repeated when the sensor's own name already carries it, eg. 'Temperature #1'
        var typeSuffix = reading.Text.Contains(reading.Type, StringComparison.OrdinalIgnoreCase) ? string.Empty : $" {reading.Type}";

        return $"{hardware} {reading.Text}{typeSuffix}".Trim();
    }

    /// <summary>
    /// Returns the hardware instance of a sensor id, eg. '0' for '/hdd/0/data/31'
    /// </summary>
    /// <param name="sensorId"></param>
    /// <returns></returns>
    private static string GetHardwareInstance(string sensorId)
    {
        var parts = sensorId.Trim('/').Split('/');
        return parts.Length >= 3 ? parts[parts.Length - 3] : string.Empty;
    }

    private void SetStatusSensor(string parentSensorSafeName, string status)
    {
        var statusId = $"{Id}{StatusEntitySuffix}";

        if (Sensors.TryGetValue(statusId, out var knownSensor) && knownSensor is DataTypeStringSensor knownStringSensor)
        {
            knownStringSensor.SetState(status);
            return;
        }

        var statusEntityName = $"{parentSensorSafeName}{StatusEntitySuffix}";
        var statusSensor = new DataTypeStringSensor(_updateInterval, statusEntityName, "LibreHardwareMonitor Status", statusId, string.Empty, "mdi:connection", string.Empty, EntityName);
        statusSensor.SetState(status);

        AddUpdateSensor(statusId, statusSensor);
    }

    private static HashSet<string> ParseTypeFilter(string sensorTypes)
    {
        var typeFilter = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        if (string.IsNullOrWhiteSpace(sensorTypes))
            return typeFilter;

        foreach (var sensorType in sensorTypes.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            typeFilter.Add(sensorType);

        return typeFilter;
    }

    private static SensorTypeMapping GetTypeMapping(string sensorType) => TypeMappings.TryGetValue(sensorType, out var mapping) ? mapping : UnknownTypeMapping;

    public override DiscoveryConfigModel GetAutoDiscoveryConfig() => null;

    private sealed class SensorTypeMapping
    {
        internal string DeviceClass { get; }
        internal string Icon { get; }
        internal bool UseRawValue { get; }

        internal SensorTypeMapping(string deviceClass, string icon, bool useRawValue = false)
        {
            DeviceClass = deviceClass;
            Icon = icon;
            UseRawValue = useRawValue;
        }
    }

    private sealed class Reading
    {
        internal string SensorId { get; init; }
        internal string Hardware { get; init; }
        internal string Text { get; init; }
        internal string Type { get; init; }
        internal double Value { get; init; }
        internal string Unit { get; init; }
        internal string Name { get; set; }
    }
}
