using System.Collections.Generic;
using HASS.Agent.Shared.Functions;
using HASS.Agent.Shared.HomeAssistant.Sensors.GeneralSensors.MultiValue.DataTypes;
using HASS.Agent.Shared.Managers;
using HASS.Agent.Shared.Models.HomeAssistant;
using HASS.Agent.Shared.Models.Internal;
using Newtonsoft.Json;

namespace HASS.Agent.Shared.HomeAssistant.Sensors.GeneralSensors.MultiValue
{
    /// <summary>
    /// Multivalue sensor containing Windows Update info
    /// </summary>
    public class WindowsUpdatesSensors : AbstractMultiValueSensor
    {
        private const string DefaultName = "windowsupdates";
        private readonly int _updateInterval;

        public sealed override Dictionary<string, AbstractSingleValueSensor> Sensors { get; protected set; } = new Dictionary<string, AbstractSingleValueSensor>();

        public WindowsUpdatesSensors(int? updateInterval = null, string entityName = DefaultName, string name = DefaultName, string id = default) : base(entityName ?? DefaultName, name ?? null, updateInterval ?? 900, id)
        {
            _updateInterval = updateInterval ?? 900;

            UpdateSensorValues();
        }

        private void AddUpdateSensor(string sensorId, AbstractSingleValueSensor sensor)
        {
            if (!Sensors.ContainsKey(sensorId))
                Sensors.Add(sensorId, sensor);
            else
                Sensors[sensorId] = sensor;
        }

        public sealed override void UpdateSensorValues()
        {
            var parentSensorSafeName = SharedHelperFunctions.GetSafeValue(EntityName);

            var (driverUpdates, softwareUpdates) = WindowsUpdatesManager.GetAvailableUpdates();

            var driverUpdateCount = driverUpdates.Count;
            var driverUpdateCountId = $"{parentSensorSafeName}_driver_updates_pending";
            var driverUpdateCountSensor = new DataTypeIntSensor(_updateInterval, driverUpdateCountId, "Driver Updates Pending", driverUpdateCountId, string.Empty, "measurement", "mdi:microsoft-windows", string.Empty, EntityName);
            driverUpdateCountSensor.SetState(driverUpdateCount);
            AddUpdateSensor(driverUpdateCountId, driverUpdateCountSensor);

            var softwareUpdateCount = softwareUpdates.Count;
            var softwareUpdateCountId = $"{parentSensorSafeName}_software_updates_pending";
            var softwareUpdateCountSensor = new DataTypeIntSensor(_updateInterval, softwareUpdateCountId, "Software Updates Pending", softwareUpdateCountId, string.Empty, "measurement", "mdi:microsoft-windows", string.Empty, EntityName);
            softwareUpdateCountSensor.SetState(softwareUpdateCount);
            AddUpdateSensor(softwareUpdateCountId, softwareUpdateCountSensor);

            var driverUpdatesList = new WindowsUpdateInfoCollection(driverUpdates);
            var driverUpdatesStr = JsonConvert.SerializeObject(driverUpdatesList, Formatting.Indented);
            var driverUpdatesId = $"{parentSensorSafeName}_driver_updates";
            var driverUpdatesSensor = new DataTypeIntSensor(_updateInterval, driverUpdatesId, "Available Driver Updates", driverUpdatesId, string.Empty, "measurement", "mdi:microsoft-windows", string.Empty, EntityName, true);
            driverUpdatesSensor.SetState(driverUpdates.Count);
            driverUpdatesSensor.SetAttributes(driverUpdatesStr);
            AddUpdateSensor(driverUpdatesId, driverUpdatesSensor);

            var softwareUpdatesStr = JsonConvert.SerializeObject(new WindowsUpdateInfoCollection(softwareUpdates), Formatting.Indented);
            var softwareUpdatesId = $"{parentSensorSafeName}_software_updates";
            var softwareUpdatesSensor = new DataTypeIntSensor(_updateInterval, softwareUpdatesId, "Available Software Updates", softwareUpdatesId, string.Empty, "measurement", "mdi:microsoft-windows", string.Empty, EntityName, true);
            softwareUpdatesSensor.SetState(softwareUpdates.Count);
            softwareUpdatesSensor.SetAttributes(softwareUpdatesStr);
            AddUpdateSensor(softwareUpdatesId, softwareUpdatesSensor);
        }

        public override DiscoveryConfigModel GetAutoDiscoveryConfig() => null;
    }
}
