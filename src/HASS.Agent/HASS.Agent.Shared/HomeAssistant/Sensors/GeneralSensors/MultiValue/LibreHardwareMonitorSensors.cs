using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
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

    /// <summary>
    /// Decimals kept from a value read as a bare number, matching the most precise format
    /// LibreHardwareMonitor displays
    /// </summary>
    private const int RawValueDecimals = 3;

    private const string StateClassMeasurement = "measurement";
    private const string StateClassTotalIncreasing = "total_increasing";
    private const string StatusOk = "ok";
    private const string StatusUnreachable = "unreachable";
    private const string StatusInvalid = "invalid";

    /// <summary>
    /// Maps every LibreHardwareMonitor sensor type onto its Home Assistant device class, icon
    /// and state class.
    /// </summary>
    /// <remarks>
    /// A type whose displayed unit isn't stable, or which is published without one, carries a
    /// fixed unit here and reads its number from 'RawValue', which is always in base units.
    /// A device class is only assigned where the unit LibreHardwareMonitor reports is a valid
    /// one for it: energy is published in mWh and conductivity in µS/cm, neither of which Home
    /// Assistant accepts for its matching device class, so those are left without one.
    /// Data and small data are binary multiples, 2^30 and 2^20 bytes, which LibreHardwareMonitor
    /// labels GB and MB; they are published as GiB and MiB so Home Assistant reads them as the
    /// binary units they are.
    /// </remarks>
    private static readonly Dictionary<string, SensorTypeMapping> TypeMappings = new(StringComparer.OrdinalIgnoreCase)
    {
        { "Clock", new SensorTypeMapping("frequency", "mdi:speedometer") },
        { "Conductivity", new SensorTypeMapping(string.Empty, "mdi:water-opacity") },
        { "Control", new SensorTypeMapping(string.Empty, "mdi:fan") },
        { "Current", new SensorTypeMapping("current", "mdi:current-dc") },
        { "Data", new SensorTypeMapping("data_size", "mdi:harddisk", "GiB") },
        { "Energy", new SensorTypeMapping(string.Empty, "mdi:lightning-bolt", stateClass: StateClassTotalIncreasing) },
        { "Factor", new SensorTypeMapping(string.Empty, "mdi:numeric") },
        { "Fan", new SensorTypeMapping(string.Empty, "mdi:fan") },
        { "Flow", new SensorTypeMapping("volume_flow_rate", "mdi:water-pump") },
        { "Frequency", new SensorTypeMapping("frequency", "mdi:pulse") },
        { "Humidity", new SensorTypeMapping("humidity", "mdi:water-percent") },
        { "Level", new SensorTypeMapping(string.Empty, "mdi:gauge-low") },
        { "Load", new SensorTypeMapping(string.Empty, "mdi:gauge") },
        { "Noise", new SensorTypeMapping("sound_pressure", "mdi:volume-high") },
        { "Power", new SensorTypeMapping("power", "mdi:flash") },
        { "SmallData", new SensorTypeMapping("data_size", "mdi:memory", "MiB") },
        { "Temperature", new SensorTypeMapping("temperature", "mdi:thermometer") },
        { "Throughput", new SensorTypeMapping("data_rate", "mdi:swap-vertical", "B/s") },
        { "TimeSpan", new SensorTypeMapping("duration", "mdi:timer-sand", "s") },
        { "Timing", new SensorTypeMapping(string.Empty, "mdi:timer-outline") },
        { "Voltage", new SensorTypeMapping("voltage", "mdi:sine-wave") }
    };

    private static readonly SensorTypeMapping UnknownTypeMapping = new(string.Empty, "mdi:chip");

    /// <summary>
    /// Connection speed is reported as a throughput sensor, but its value is a bit rate rather
    /// than a byte rate, and its displayed unit scales through bps, Kbps, Mbps and Gbps.
    /// </summary>
    private const string ThroughputSensorType = "Throughput";
    private const string ConnectionSpeedSensorName = "Connection Speed";
    private static readonly SensorTypeMapping ConnectionSpeedMapping = new("data_rate", "mdi:ethernet", "bit/s");

    // matches a number followed by an optional unit, eg. '45.0 C', '1679 RPM' or '40.500'
    private static readonly Regex ValuePattern = new(@"^\s*(-?[0-9.,]+)\s*(.*?)\s*$", RegexOptions.Compiled);

    private readonly int _updateInterval;
    private readonly HashSet<string> _typeFilter;

    private bool _initialAnnouncementDone;

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

        _initialAnnouncementDone = true;
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

        try
        {
            var newSensors = new List<AbstractSingleValueSensor>();

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

                var entityName = $"{parentSensorSafeName}_{safeSensorId}";

                var sensor = new DataTypeDoubleSensor(_updateInterval, entityName, reading.Name, sensorId, reading.Mapping.DeviceClass, reading.Mapping.StateClass, reading.Mapping.Icon, reading.Unit, EntityName);
                sensor.SetState(reading.Value);

                AddUpdateSensor(sensorId, sensor);

                if (_initialAnnouncementDone)
                    newSensors.Add(sensor);
            }

            SetStatusSensor(parentSensorSafeName, StatusOk);

            if (newSensors.Count > 0)
                AnnounceSensors(newSensors);
        }
        catch (Exception ex)
        {
            // the endpoint answered with something we can't read, the existing sensors keep their last known values
            Log.Error(ex, "[LIBREHARDWAREMONITOR] [{name}] Error reading the response from '{url}': {err}", EntityName, EndpointUrl, ex.Message);

            SetStatusSensor(parentSensorSafeName, StatusInvalid);
        }
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

        var text = node.Value<string>("Text") ?? string.Empty;
        var mapping = GetTypeMapping(sensorType, text);

        // skips the sensors reporting 'NaN' or '-', which have no value to publish
        if (!TryReadValue(node, mapping, out var value, out var unit))
            return null;

        return new Reading
        {
            SensorId = sensorId,
            Hardware = ancestors.Count >= 2 ? ancestors[ancestors.Count - 2] : string.Empty,
            Text = text,
            Type = sensorType,
            Mapping = mapping,
            Value = value,
            Unit = unit
        };
    }

    /// <summary>
    /// Reads a sensor's value and unit.
    /// </summary>
    /// <remarks>
    /// 'Value' is a formatted string in every LibreHardwareMonitor version, so it's the reliable
    /// source for the unit, and the only source that reflects a display preference such as
    /// Fahrenheit. A type whose displayed unit isn't stable instead takes its number from
    /// 'RawValue' and its unit from the mapping: 'RawValue' is a formatted string in older
    /// versions and a bare number in newer ones, so it's read as a number where possible.
    /// </remarks>
    /// <param name="node"></param>
    /// <param name="mapping"></param>
    /// <param name="value"></param>
    /// <param name="unit"></param>
    /// <returns></returns>
    private static bool TryReadValue(JToken node, SensorTypeMapping mapping, out double value, out string unit)
    {
        if (string.IsNullOrEmpty(mapping.Unit))
            return TryParseValue(node.Value<string>("Value"), out value, out unit);

        unit = mapping.Unit;
        return TryReadNumber(node["RawValue"], out value);
    }

    private static bool TryReadNumber(JToken token, out double value)
    {
        value = 0d;

        if (token == null)
            return false;

        // newer versions publish a bare number, which is read directly so no culture is involved
        if (token.Type is JTokenType.Float or JTokenType.Integer)
        {
            value = token.Value<double>();

            if (!double.IsFinite(value))
                return false;

            // a bare number carries the sensor's full precision, eg. 62.30000305175781 for a value
            // LibreHardwareMonitor itself displays as 62.3
            value = Math.Round(value, RawValueDecimals);
            return true;
        }

        return TryParseValue(token.Value<string>(), out value, out _);
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

    private static SensorTypeMapping GetTypeMapping(string sensorType, string text)
    {
        if (string.Equals(sensorType, ThroughputSensorType, StringComparison.OrdinalIgnoreCase)
            && string.Equals(text, ConnectionSpeedSensorName, StringComparison.OrdinalIgnoreCase))
            return ConnectionSpeedMapping;

        return TypeMappings.TryGetValue(sensorType, out var mapping) ? mapping : UnknownTypeMapping;
    }



    /// <summary>
    /// Announces sensors that appeared after the initial autodiscovery round, which would
    /// otherwise publish their state without Home Assistant having a configuration for them
    /// </summary>
    /// <param name="sensors"></param>
    private void AnnounceSensors(IReadOnlyList<AbstractSingleValueSensor> sensors)
    {
        _ = Task.Run(async () =>
        {
            foreach (var sensor in sensors)
            {
                try
                {
                    await sensor.PublishAutoDiscoveryConfigAsync();
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "[LIBREHARDWAREMONITOR] [{name}] Error announcing sensor {sensor}: {err}", EntityName, sensor.EntityName, ex.Message);
                }
            }

            Log.Information("[LIBREHARDWAREMONITOR] [{name}] Announced {count} newly discovered sensor(s)", EntityName, sensors.Count);
        });
    }

    public override DiscoveryConfigModel GetAutoDiscoveryConfig() => null;

    private sealed class SensorTypeMapping
    {
        internal string DeviceClass { get; }
        internal string Icon { get; }

        /// <summary>
        /// Fixed unit for a type whose displayed unit varies with its value. Empty means the unit
        /// is taken from the sensor's own formatted value.
        /// </summary>
        internal string Unit { get; }

        internal string StateClass { get; }

        internal SensorTypeMapping(string deviceClass, string icon, string unit = "", string stateClass = StateClassMeasurement)
        {
            DeviceClass = deviceClass;
            Icon = icon;
            Unit = unit;
            StateClass = stateClass;
        }
    }


    private sealed class Reading
    {
        internal string SensorId { get; init; }
        internal string Hardware { get; init; }
        internal string Text { get; init; }
        internal string Type { get; init; }
        internal SensorTypeMapping Mapping { get; init; }
        internal double Value { get; init; }
        internal string Unit { get; init; }
        internal string Name { get; set; }
    }
}
