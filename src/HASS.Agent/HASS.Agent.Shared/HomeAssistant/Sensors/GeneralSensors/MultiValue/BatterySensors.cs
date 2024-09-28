using System;
using System.Collections.Generic;
using System.Windows.Forms;
using HASS.Agent.Shared.Functions;
using HASS.Agent.Shared.HomeAssistant.Sensors.GeneralSensors.MultiValue.DataTypes;
using HASS.Agent.Shared.Models.HomeAssistant;

namespace HASS.Agent.Shared.HomeAssistant.Sensors.GeneralSensors.MultiValue;

/// <summary>
/// Multivalue sensor containing power and battery info
/// </summary>
public class BatterySensors : AbstractMultiValueSensor
{
    private const string DefaultName = "battery";
    private readonly int _updateInterval;

    public sealed override Dictionary<string, AbstractSingleValueSensor> Sensors { get; protected set; } = new Dictionary<string, AbstractSingleValueSensor>();

    public BatterySensors(int? updateInterval = null, string entityName = DefaultName, string name = DefaultName, string id = default) : base(entityName ?? DefaultName, name ?? null, updateInterval ?? 30, id)
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

    public sealed override void UpdateSensorValues()
    {
        var parentSensorSafeName = SharedHelperFunctions.GetSafeValue(EntityName);
        var deviceName = SharedHelperFunctions.GetSafeDeviceName();

        var powerStatus = SystemInformation.PowerStatus;
        var chargeStatus = powerStatus.BatteryChargeStatus.ToString();

        var chargeStatusEntityName = $"{parentSensorSafeName}_charge_status";
        var chargeStatusId = $"{Id}_charge_status";
        var chargeStatusSensor = new DataTypeStringSensor(_updateInterval, chargeStatusEntityName, "Charge Status", chargeStatusId, string.Empty, "mdi:battery-charging", string.Empty, EntityName);
        chargeStatusSensor.SetState(chargeStatus);
        AddUpdateSensor(chargeStatusId, chargeStatusSensor);

        var fullChargeLifetimeMinutes = powerStatus.BatteryFullLifetime;
        if (fullChargeLifetimeMinutes != -1)
            fullChargeLifetimeMinutes = Convert.ToInt32(Math.Round(TimeSpan.FromSeconds(fullChargeLifetimeMinutes).TotalMinutes));

        var fullChargeLifetimeEntityName = $"{parentSensorSafeName}_full_charge_lifetime";
        var fullChargeLifetimeId = $"{Id}_full_charge_lifetime";
        var fullChargeLifetimeSensor = new DataTypeIntSensor(_updateInterval, fullChargeLifetimeEntityName, "Full Charge Lifetime", fullChargeLifetimeId, string.Empty, "measurement", "mdi:battery-high", string.Empty, EntityName);
        fullChargeLifetimeSensor.SetState(fullChargeLifetimeMinutes);
        AddUpdateSensor(fullChargeLifetimeId, fullChargeLifetimeSensor);

        var chargeRemainingPercentage = Convert.ToInt32(powerStatus.BatteryLifePercent * 100);
        var chargeRemainingPercentageEntityName = $"{parentSensorSafeName}_charge_remaining_percentage";
        var chargeRemainingPercentageId = $"{Id}_charge_remaining_percentage";
        var chargeRemainingPercentageSensor = new DataTypeIntSensor(_updateInterval, chargeRemainingPercentageEntityName, "Charge Remaining Percentage", chargeRemainingPercentageId, string.Empty, "measurement", "mdi:battery-high", "%", EntityName);
        chargeRemainingPercentageSensor.SetState(chargeRemainingPercentage);
        AddUpdateSensor(chargeRemainingPercentageId, chargeRemainingPercentageSensor);

        var chargeRemainingMinutes = powerStatus.BatteryLifeRemaining;
        if (chargeRemainingMinutes != -1)
            chargeRemainingMinutes = Convert.ToInt32(Math.Round(TimeSpan.FromSeconds(chargeRemainingMinutes).TotalMinutes));

        var chargeRemainingMinutesEntityName = $"{parentSensorSafeName}_charge_remaining";
        var chargeRemainingMinutesId = $"{Id}_charge_remaining";
        var chargeRemainingMinutesSensor = new DataTypeIntSensor(_updateInterval, chargeRemainingMinutesEntityName, "Charge Remaining", chargeRemainingMinutesId, string.Empty, "measurement", "mdi:battery-high", string.Empty, EntityName);
        chargeRemainingMinutesSensor.SetState(chargeRemainingMinutes);
        AddUpdateSensor(chargeRemainingMinutesId, chargeRemainingMinutesSensor);

        var powerlineStatus = powerStatus.PowerLineStatus.ToString();
        var powerlineStatusEntityName = $"{parentSensorSafeName}_powerline_status";
        var powerlineStatusId = $"{Id}_powerline_status";
        var powerlineStatusSensor = new DataTypeStringSensor(_updateInterval, powerlineStatusEntityName, "Powerline Status", powerlineStatusId, string.Empty, "mdi:power-plug", string.Empty, EntityName);
        powerlineStatusSensor.SetState(powerlineStatus);
        AddUpdateSensor(powerlineStatusId, powerlineStatusSensor);
    }

    public override DiscoveryConfigModel GetAutoDiscoveryConfig() => null;
}
