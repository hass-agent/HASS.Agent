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

        public override sealed Dictionary<string, AbstractSingleValueSensor> Sensors { get; protected set; } = new Dictionary<string, AbstractSingleValueSensor>();

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

        public override sealed void UpdateSensorValues()
        {
            var parentSensorSafeName = SharedHelperFunctions.GetSafeValue(EntityName);

            var (driverUpdates, softwareUpdates) = WindowsUpdatesManager.GetAvailableUpdates();

            var driverUpdateCountEntityName = $"{parentSensorSafeName}_driver_updates_pending";
            var driverUpdateCountId = $"{Id}_driver_updates_pending";
            var driverUpdateCountSensor = new DataTypeIntSensor(_updateInterval, driverUpdateCountEntityName, "Driver Updates Pending", driverUpdateCountId, string.Empty, "measurement", "mdi:microsoft-windows", string.Empty, EntityName);
            driverUpdateCountSensor.SetState(driverUpdates.Count);
            AddUpdateSensor(driverUpdateCountId, driverUpdateCountSensor);

            var softwareUpdateCountEntityName = $"{parentSensorSafeName}_software_updates_pending";
            var softwareUpdateCountId = $"{Id}_software_updates_pending";
            var softwareUpdateCountSensor = new DataTypeIntSensor(_updateInterval, softwareUpdateCountEntityName, "Software Updates Pending", softwareUpdateCountId, string.Empty, "measurement", "mdi:microsoft-windows", string.Empty, EntityName);
            softwareUpdateCountSensor.SetState(softwareUpdates.Count);
            AddUpdateSensor(softwareUpdateCountId, softwareUpdateCountSensor);

            var driverUpdatesEntityName = $"{parentSensorSafeName}_driver_updates";
            var driverUpdatesId = $"{Id}_driver_updates";
            var driverUpdatesSensor = new DataTypeIntSensor(_updateInterval, driverUpdatesEntityName, "Available Driver Updates", driverUpdatesId, string.Empty, "measurement", "mdi:microsoft-windows", string.Empty, EntityName, true);
            driverUpdatesSensor.SetState(driverUpdates.Count);
            driverUpdatesSensor.SetAttributes(
                JsonConvert.SerializeObject(
                    new WindowsUpdateInfoCollection(driverUpdates)
                , Formatting.Indented)
            );
            AddUpdateSensor(driverUpdatesId, driverUpdatesSensor);

            var softwareUpdatesEntityName = $"{parentSensorSafeName}_software_updates";
            var softwareUpdatesId = $"{Id}_software_updates";
            var softwareUpdatesSensor = new DataTypeIntSensor(_updateInterval, softwareUpdatesEntityName, "Available Software Updates", softwareUpdatesId, string.Empty, "measurement", "mdi:microsoft-windows", string.Empty, EntityName, true);
            softwareUpdatesSensor.SetState(softwareUpdates.Count);
            softwareUpdatesSensor.SetAttributes(
                JsonConvert.SerializeObject(
                    new WindowsUpdateInfoCollection(softwareUpdates)
                , Formatting.Indented)
            );
            AddUpdateSensor(softwareUpdatesId, softwareUpdatesSensor);
        }

        public override DiscoveryConfigModel GetAutoDiscoveryConfig() => null;
    }
}
