using System;
using System.Collections.Generic;
using System.IO;
using System.Security;
using ByteSizeLib;
using HASS.Agent.Shared.Functions;
using HASS.Agent.Shared.HomeAssistant.Sensors.GeneralSensors.MultiValue.DataTypes;
using HASS.Agent.Shared.Models.HomeAssistant;
using HASS.Agent.Shared.Models.Internal;
using Newtonsoft.Json;
using Serilog;
#pragma warning disable CS1591

namespace HASS.Agent.Shared.HomeAssistant.Sensors.GeneralSensors.MultiValue;

/// <summary>
/// Multivalue sensor containing local storage info
/// </summary>
public class StorageSensors : AbstractMultiValueSensor
{
    private const string DefaultName = "storage";
    private readonly int _updateInterval;

    public override sealed Dictionary<string, AbstractSingleValueSensor> Sensors { get; protected set; } = new Dictionary<string, AbstractSingleValueSensor>();

    public StorageSensors(int? updateInterval = null, string entityName = DefaultName, string name = DefaultName, string id = default) : base(entityName ?? DefaultName, name ?? null, updateInterval ?? 30, id)
    {
        _updateInterval = updateInterval ?? 30;

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
        var driveCount = 0;

        var parentSensorSafeName = SharedHelperFunctions.GetSafeValue(EntityName);

        foreach (var drive in DriveInfo.GetDrives())
        {
            try
            {
                if (!(drive is { IsReady: true, DriveType: DriveType.Fixed }))
                    continue;

                if (string.IsNullOrWhiteSpace(drive.Name))
                    continue;

                var driveName = $"{drive.Name[..1].ToUpper()}";
                var driveNameLower = driveName.ToLower();

                var driveLabel = string.IsNullOrEmpty(drive.VolumeLabel) ? "-" : drive.VolumeLabel;

                var sensorValue = string.IsNullOrEmpty(drive.VolumeLabel) ? driveName : drive.VolumeLabel;

                var storageInfo = new StorageInfo
                {
                    Name = driveName,
                    Label = driveLabel,
                    FileSystem = drive.DriveFormat
                };

                var totalSizeMb = Math.Round(ByteSize.FromBytes(drive.TotalSize).MegaBytes);
                storageInfo.TotalSizeMB = totalSizeMb;

                var availableSpaceMb = Math.Round(ByteSize.FromBytes(drive.AvailableFreeSpace).MegaBytes);
                storageInfo.AvailableSpaceMB = availableSpaceMb;

                var usedSpaceMb = totalSizeMb - availableSpaceMb;
                storageInfo.UsedSpaceMB = usedSpaceMb;

                var availableSpacePercentage = (int)Math.Round((availableSpaceMb / totalSizeMb) * 100);
                storageInfo.AvailableSpacePercentage = availableSpacePercentage;

                var usedSpacePercentage = (int)Math.Round((usedSpaceMb / totalSizeMb) * 100);
                storageInfo.UsedSpacePercentage = usedSpacePercentage;

                var info = JsonConvert.SerializeObject(storageInfo, Formatting.Indented);
                var driveInfoEntityName = $"{parentSensorSafeName}_{driveNameLower}";
                var driveInfoId = $"{Id}_{driveNameLower}";
                var driveInfoSensor = new DataTypeStringSensor(_updateInterval, driveInfoEntityName, driveName, driveInfoId, string.Empty, "mdi:harddisk", string.Empty, EntityName, true);

                driveInfoSensor.SetState(sensorValue);
                driveInfoSensor.SetAttributes(info);

                AddUpdateSensor(driveInfoId, driveInfoSensor);

                driveCount++;
            }
            catch (Exception ex)
            {
                switch (ex)
                {
                    case UnauthorizedAccessException _:
                    case SecurityException _:
                        Log.Fatal(ex, "[STORAGE] [{name}] Disk access denied: {msg}", EntityName, ex.Message);
                        continue;
                    case DriveNotFoundException _:
                        Log.Fatal(ex, "[STORAGE] [{name}] Disk not found: {msg}", EntityName, ex.Message);
                        continue;
                    case IOException _:
                        Log.Fatal(ex, "[STORAGE] [{name}] Disk IO error: {msg}", EntityName, ex.Message);
                        continue;
                }

                Log.Fatal(ex, "[STORAGE] [{name}] Error querying disk: {msg}", EntityName, ex.Message);
            }
        }
         
        var driveCountEntityName = $"{parentSensorSafeName}_total_disk_count";
        var driveCountId = $"{Id}_total_disk_count";
        var driveCountSensor = new DataTypeIntSensor(_updateInterval, driveCountEntityName, "Total Disk Count", driveCountId, string.Empty, "measurement", "mdi:harddisk", string.Empty, EntityName);
        driveCountSensor.SetState(driveCount);

        AddUpdateSensor(driveCountId, driveCountSensor);
    }

    public override DiscoveryConfigModel GetAutoDiscoveryConfig() => null;
}
