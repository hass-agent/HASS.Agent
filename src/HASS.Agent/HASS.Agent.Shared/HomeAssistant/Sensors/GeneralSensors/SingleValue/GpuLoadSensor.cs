﻿using System.Globalization;
using System.Linq;
using HASS.Agent.Shared.Managers;
using HASS.Agent.Shared.Models.HomeAssistant;
using LibreHardwareMonitor.Hardware;

namespace HASS.Agent.Shared.HomeAssistant.Sensors.GeneralSensors.SingleValue;

/// <summary>
/// Sensor indicating the current GPU load
/// </summary>
public class GpuLoadSensor : AbstractSingleValueSensor
{
	private const string DefaultName = "gpuload";
	private readonly IHardware _gpu;

	public GpuLoadSensor(int? updateInterval = null, string name = DefaultName, string friendlyName = DefaultName, string id = default) : base(name ?? DefaultName, friendlyName ?? null, updateInterval ?? 30, id)
	{
		_gpu = HardwareManager.Hardware.FirstOrDefault(
			h => h.HardwareType == HardwareType.GpuAmd ||
			h.HardwareType == HardwareType.GpuNvidia ||
            h.HardwareType == HardwareType.GpuIntel
		);
	}

	public override DiscoveryConfigModel GetAutoDiscoveryConfig()
	{
		if (Variables.MqttManager == null)
			return null;

		var deviceConfig = Variables.MqttManager.GetDeviceConfigModel();
		if (deviceConfig == null)
			return null;

		return AutoDiscoveryConfigModel ?? SetAutoDiscoveryConfigModel(new SensorDiscoveryConfigModel()
		{
			Name = Name,
			FriendlyName = FriendlyName,
			Unique_id = Id,
			Device = deviceConfig,
			State_topic = $"{Variables.MqttManager.MqttDiscoveryPrefix()}/{Domain}/{deviceConfig.Name}/{ObjectId}/state",
			Unit_of_measurement = "%",
			Availability_topic = $"{Variables.MqttManager.MqttDiscoveryPrefix()}/{Domain}/{deviceConfig.Name}/availability"
		});
	}

	public override string GetState()
	{
		if (_gpu == null)
			return null;

		_gpu.Update();

		var sensor = _gpu.Sensors.FirstOrDefault(s => s.SensorType == SensorType.Load);

		if (sensor?.Value == null)
			return null;

		return sensor.Value.HasValue ? sensor.Value.Value.ToString("#.##", CultureInfo.InvariantCulture) : null;
	}

	public override string GetAttributes() => string.Empty;
}