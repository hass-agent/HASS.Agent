using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using HASS.Agent.Shared.Functions;
using HASS.Agent.Shared.HomeAssistant.Sensors.GeneralSensors.MultiValue.DataTypes;
using HASS.Agent.Shared.Managers;
using HASS.Agent.Shared.Managers.Audio;
using HASS.Agent.Shared.Models.HomeAssistant;
using HASS.Agent.Shared.Models.Internal;
using HidSharp;
using Newtonsoft.Json;
using Serilog;

namespace HASS.Agent.Shared.HomeAssistant.Sensors.GeneralSensors.MultiValue;

/// <summary>
/// Multivalue sensor containing audio-related info
/// </summary>
public class AudioSensors : AbstractMultiValueSensor
{
    private const string DefaultName = "audio";
    private bool _errorPrinted = false;

    private readonly int _updateInterval;

    public override sealed Dictionary<string, AbstractSingleValueSensor> Sensors { get; protected set; } = new Dictionary<string, AbstractSingleValueSensor>();

    public AudioSensors(int? updateInterval = null, string entityName = DefaultName, string name = DefaultName, string id = default) : base(entityName ?? DefaultName, name ?? null, updateInterval ?? 20, id)
    {
        _updateInterval = updateInterval ?? 20;

        UpdateSensorValues();
    }

    private void AddUpdateSensor(string sensorId, AbstractSingleValueSensor sensor)
    {
        if (!Sensors.ContainsKey(sensorId))
            Sensors.Add(sensorId, sensor);
        else
            Sensors[sensorId] = sensor;
    }

    private void HandleAudioOutputSensors(IEnumerable<AudioDevice> outputDevices, string parentSensorSafeName)
    {
        var outputDevice = outputDevices.Where(d => d.Default).FirstOrDefault();
        if (outputDevice == null)
            return;

        var defaultDeviceEntityName = $"{parentSensorSafeName}_default_device";
        var defaultDeviceId = $"{Id}_default_device";
        var defaultDeviceSensor = new DataTypeStringSensor(_updateInterval, defaultDeviceEntityName, $"Default Device", defaultDeviceId, string.Empty, "mdi:speaker", string.Empty, EntityName);
        defaultDeviceSensor.SetState(outputDevice.FriendlyName);
        AddUpdateSensor(defaultDeviceId, defaultDeviceSensor);

        var defaultDeviceStateEntityName = $"{parentSensorSafeName}_default_device_state";
        var defaultDeviceStateId = $"{Id}_default_device_state";
        var defaultDeviceStateSensor = new DataTypeStringSensor(_updateInterval, defaultDeviceStateEntityName, $"Default Device State", defaultDeviceStateId, string.Empty, "mdi:speaker", string.Empty, EntityName);
        defaultDeviceStateSensor.SetState(outputDevice.State);
        AddUpdateSensor(defaultDeviceStateId, defaultDeviceStateSensor);

        var defaultDeviceVolumeEntityName = $"{parentSensorSafeName}_default_device_volume";
        var defaultDeviceVolumeId = $"{Id}_default_device_volume";
        var defaultDeviceVolumeSensor = new DataTypeIntSensor(_updateInterval, defaultDeviceVolumeEntityName, $"Default Device Volume", defaultDeviceVolumeId, string.Empty, "measurement", "mdi:speaker", string.Empty, EntityName);
        defaultDeviceVolumeSensor.SetState(outputDevice.Volume);
        AddUpdateSensor(defaultDeviceVolumeId, defaultDeviceVolumeSensor);

        var defaultDeviceIsMutedEntityName = $"{parentSensorSafeName}_default_device_muted";
        var defaultDeviceIsMutedId = $"{Id}_default_device_muted";
        var defaultDeviceIsMutedSensor = new DataTypeBoolSensor(_updateInterval, defaultDeviceIsMutedEntityName, $"Default Device Muted", defaultDeviceIsMutedId, string.Empty, "mdi:speaker", EntityName);
        defaultDeviceIsMutedSensor.SetState(outputDevice.Muted);
        AddUpdateSensor(defaultDeviceIsMutedId, defaultDeviceIsMutedSensor);

        var peakVolumeEntityName = $"{parentSensorSafeName}_peak_volume";
        var peakVolumeId = $"{Id}_peak_volume";
        var peakVolumeSensor = new DataTypeDoubleSensor(_updateInterval, peakVolumeEntityName, $"Peak Volume", peakVolumeId, string.Empty, "measurement", "mdi:volume-high", string.Empty, EntityName);
        peakVolumeSensor.SetState(outputDevice.PeakVolume);
        AddUpdateSensor(peakVolumeId, peakVolumeSensor);

        var sessionsEntityName = $"{parentSensorSafeName}_sessions";
        var sessionsId = $"{Id}_sessions";
        var sessionsSensor = new DataTypeIntSensor(_updateInterval, sessionsEntityName, $"Audio Sessions", sessionsId, string.Empty, "measurement", "mdi:music-box-multiple-outline", string.Empty, EntityName, true);
        sessionsSensor.SetState(outputDevice.Sessions.Count);
        sessionsSensor.SetAttributes(
            JsonConvert.SerializeObject(new
            {
                AudioSessions = outputDevice.Sessions
            }, Formatting.Indented)
        );
        AddUpdateSensor(sessionsId, sessionsSensor);

        var audioOutputDevices = outputDevices.Select(d => d.FriendlyName).ToList();
        var audioOutputDevicesEntityName = $"{parentSensorSafeName}_output_devices";
        var audioOutputDevicesId = $"{Id}_output_devices";
        var audioOutputDevicesSensor = new DataTypeIntSensor(_updateInterval, audioOutputDevicesEntityName, $"Audio Output Devices", audioOutputDevicesId, string.Empty, "measurement", "mdi:music-box-multiple-outline", string.Empty, EntityName, true);
        audioOutputDevicesSensor.SetState(audioOutputDevices.Count);
        audioOutputDevicesSensor.SetAttributes(
            JsonConvert.SerializeObject(new
            {
                OutputDevices = audioOutputDevices
            }, Formatting.Indented)
        );
        AddUpdateSensor(audioOutputDevicesId, audioOutputDevicesSensor);
    }

    private void HandleAudioInputSensors(IEnumerable<AudioDevice> inputDevices, string parentSensorSafeName)
    {
        var inputDevice = inputDevices.Where(d => d.Default).FirstOrDefault();
        if (inputDevice == null)
            return;

        var defaultInputDeviceEntityName = $"{parentSensorSafeName}_default_input_device";
        var defaultInputDeviceId = $"{Id}_default_input_device";
        var defaultInputDeviceSensor = new DataTypeStringSensor(_updateInterval, defaultInputDeviceEntityName, $"Default Input Device", defaultInputDeviceId, string.Empty, "mdi:microphone", string.Empty, EntityName);
        defaultInputDeviceSensor.SetState(inputDevice.FriendlyName);
        AddUpdateSensor(defaultInputDeviceId, defaultInputDeviceSensor);

        var defaultInputDeviceStateEntityName = $"{parentSensorSafeName}_default_input_device_state";
        var defaultInputDeviceStateId = $"{Id}_default_input_device_state";
        var defaultInputDeviceStateSensor = new DataTypeStringSensor(_updateInterval, defaultInputDeviceStateEntityName, $"Default Input Device State", defaultInputDeviceStateId, string.Empty, "mdi:microphone", string.Empty, EntityName);
        defaultInputDeviceStateSensor.SetState(inputDevice.State);
        AddUpdateSensor(defaultInputDeviceStateId, defaultInputDeviceStateSensor);

        var defaultInputDeviceIsMutedEntityName = $"{parentSensorSafeName}_default_input_device_muted";
        var defaultInputDeviceIsMutedId = $"{Id}_default_input_device_muted";
        var defaultInputDeviceIsMutedSensor = new DataTypeBoolSensor(_updateInterval, defaultInputDeviceIsMutedEntityName, $"Default Input Device Muted", defaultInputDeviceIsMutedId, string.Empty, "mdi:microphone", EntityName);
        defaultInputDeviceIsMutedSensor.SetState(inputDevice.Muted);
        AddUpdateSensor(defaultInputDeviceIsMutedId, defaultInputDeviceIsMutedSensor);

        var defaultInputDeviceVolumeEntityName = $"{parentSensorSafeName}_default_input_device_volume";
        var defaultInputDeviceVolumeId = $"{Id}_default_input_device_volume";
        var defaultInputDeviceVolumeSensor = new DataTypeIntSensor(_updateInterval, defaultInputDeviceVolumeEntityName, $"Default Input Device Volume", defaultInputDeviceVolumeId, string.Empty, "measurement", "mdi:microphone", string.Empty, EntityName);
        defaultInputDeviceVolumeSensor.SetState(Convert.ToInt32(inputDevice.PeakVolume));
        AddUpdateSensor(defaultInputDeviceVolumeId, defaultInputDeviceVolumeSensor);

        var audioInputDevices = inputDevices.Select(d => d.FriendlyName).ToList();
        var audioInputDevicesEntityName = $"{parentSensorSafeName}_input_devices";
        var audioInputDevicesId = $"{Id}_input_devices";
        var audioInputDevicesSensor = new DataTypeIntSensor(_updateInterval, audioInputDevicesEntityName, $"Audio Input Devices", audioInputDevicesId, string.Empty, "measurement", "mdi:microphone", string.Empty, EntityName, true);
        audioInputDevicesSensor.SetState(audioInputDevices.Count);
        audioInputDevicesSensor.SetAttributes(
            JsonConvert.SerializeObject(new
            {
                InputDevices = audioInputDevices
            }, Formatting.Indented)
        );
        AddUpdateSensor(audioInputDevicesId, audioInputDevicesSensor);
    }

    public override sealed void UpdateSensorValues()
    {
        try
        {
            var audioDevices = AudioManager.GetDevices();
            var outputDevices = audioDevices.Where(d => d.Type == DeviceType.Output);
            var inputDevices = audioDevices.Where(d => d.Type == DeviceType.Input);

            var parentSensorSafeName = SharedHelperFunctions.GetSafeValue(EntityName);

            HandleAudioOutputSensors(outputDevices, parentSensorSafeName);
            HandleAudioInputSensors(inputDevices, parentSensorSafeName);

            if (_errorPrinted)
                _errorPrinted = false;
        }
        catch (Exception ex)
        {
            if (_errorPrinted)
                return;

            _errorPrinted = true;

            Log.Fatal(ex, "[AUDIO] [{name}] Error while fetching audio info: {err}", EntityName, ex.Message);
        }
    }

    public override DiscoveryConfigModel GetAutoDiscoveryConfig() => null;
}
