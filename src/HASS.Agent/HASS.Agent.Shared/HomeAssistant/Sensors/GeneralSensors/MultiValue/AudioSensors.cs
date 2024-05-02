using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using CoreAudio;
using HASS.Agent.Shared.Functions;
using HASS.Agent.Shared.HomeAssistant.Sensors.GeneralSensors.MultiValue.DataTypes;
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
    private static readonly Dictionary<int, string> ApplicationNames = new();
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

    private List<string> GetAudioDevices(DataFlow type)
    {
        var audioDevices = new List<string>();
        foreach (var device in Variables.AudioDeviceEnumerator.EnumerateAudioEndPoints(type, DeviceState.Active))
        {
            audioDevices.Add(device.DeviceFriendlyName);
            device.Dispose();
        }

        return audioDevices;
    }

    private List<string> GetAudioOutputDevices() => GetAudioDevices(DataFlow.Render);
    private List<string> GetAudioInputDevices() => GetAudioDevices(DataFlow.Capture);

    private void HandleAudioOutputSensors(string parentSensorSafeName, string deviceName)
    {
        using var audioDevice = Variables.AudioDeviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);

        var defaultDeviceEntityName = $"{parentSensorSafeName}_default_device";
        var defaultDeviceId = $"{Id}_default_device";
        var defaultDeviceSensor = new DataTypeStringSensor(_updateInterval, defaultDeviceEntityName, $"Default Device", defaultDeviceId, string.Empty, "mdi:speaker", string.Empty, EntityName);
        defaultDeviceSensor.SetState(audioDevice.DeviceFriendlyName);
        AddUpdateSensor(defaultDeviceId, defaultDeviceSensor);

        var defaultDeviceStateEntityName = $"{parentSensorSafeName}_default_device_state";
        var defaultDeviceStateId = $"{Id}_default_device_state";
        var defaultDeviceStateSensor = new DataTypeStringSensor(_updateInterval, defaultDeviceStateEntityName, $"Default Device State", defaultDeviceStateId, string.Empty, "mdi:speaker", string.Empty, EntityName);
        defaultDeviceStateSensor.SetState(GetReadableState(audioDevice.State));
        AddUpdateSensor(defaultDeviceStateId, defaultDeviceStateSensor);

        var masterVolume = Convert.ToInt32(Math.Round(audioDevice.AudioEndpointVolume?.MasterVolumeLevelScalar * 100 ?? 0, 0));
        var defaultDeviceVolumeEntityName = $"{parentSensorSafeName}_default_device_volume";
        var defaultDeviceVolumeId = $"{Id}_default_device_volume";
        var defaultDeviceVolumeSensor = new DataTypeIntSensor(_updateInterval, defaultDeviceVolumeEntityName, $"Default Device Volume", defaultDeviceVolumeId, string.Empty, "measurement", "mdi:speaker", string.Empty, EntityName);
        defaultDeviceVolumeSensor.SetState(masterVolume);
        AddUpdateSensor(defaultDeviceVolumeId, defaultDeviceVolumeSensor);

        var defaultDeviceIsMuted = audioDevice.AudioEndpointVolume?.Mute ?? false;
        var defaultDeviceIsMutedEntityName = $"{parentSensorSafeName}_default_device_muted";
        var defaultDeviceIsMutedId = $"{Id}_default_device_muted";
        var defaultDeviceIsMutedSensor = new DataTypeBoolSensor(_updateInterval, defaultDeviceIsMutedEntityName, $"Default Device Muted", defaultDeviceIsMutedId, string.Empty, "mdi:speaker", EntityName);
        defaultDeviceIsMutedSensor.SetState(defaultDeviceIsMuted);
        AddUpdateSensor(defaultDeviceIsMutedId, defaultDeviceIsMutedSensor);

        // get session and volume info
        var sessionInfos = GetSessions(out var peakVolume);

        var peakVolumeEntityName = $"{parentSensorSafeName}_peak_volume";
        var peakVolumeId = $"{Id}_peak_volume";
        var peakVolumeSensor = new DataTypeStringSensor(_updateInterval, peakVolumeEntityName, $"Peak Volume", peakVolumeId, string.Empty, "mdi:volume-high", string.Empty, EntityName);
        peakVolumeSensor.SetState(peakVolume.ToString(CultureInfo.CurrentCulture));
        AddUpdateSensor(peakVolumeId, peakVolumeSensor);

        var sessionsEntityName = $"{parentSensorSafeName}_sessions";
        var sessionsId = $"{Id}_sessions";
        var sessionsSensor = new DataTypeIntSensor(_updateInterval, sessionsEntityName, $"Audio Sessions", sessionsId, string.Empty, "measurement", "mdi:music-box-multiple-outline", string.Empty, EntityName, true);
        sessionsSensor.SetState(sessionInfos.Count);
        sessionsSensor.SetAttributes(
            JsonConvert.SerializeObject(new
            {
                AudioSessions = sessionInfos
            }, Formatting.Indented)
        );
        AddUpdateSensor(sessionsId, sessionsSensor);

        var audioOutputDevices = GetAudioOutputDevices();
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

    private void HandleAudioInputSensors(string parentSensorSafeName, string deviceName)
    {
        using var inputDevice = Variables.AudioDeviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Capture, Role.Communications);

        var defaultInputDeviceEntityName = $"{parentSensorSafeName}_default_input_device";
        var defaultInputDeviceId = $"{Id}_default_input_device";
        var defaultInputDeviceSensor = new DataTypeStringSensor(_updateInterval, defaultInputDeviceEntityName, $"Default Input Device", defaultInputDeviceId, string.Empty, "mdi:microphone", string.Empty, EntityName);
        defaultInputDeviceSensor.SetState(inputDevice.DeviceFriendlyName);
        AddUpdateSensor(defaultInputDeviceId, defaultInputDeviceSensor);

        var defaultInputDeviceStateEntityName = $"{parentSensorSafeName}_default_input_device_state";
        var defaultInputDeviceStateId = $"{Id}_default_input_device_state";
        var defaultInputDeviceStateSensor = new DataTypeStringSensor(_updateInterval, defaultInputDeviceStateEntityName, $"Default Input Device State", defaultInputDeviceStateId, string.Empty, "mdi:microphone", string.Empty, EntityName);
        defaultInputDeviceStateSensor.SetState(GetReadableState(inputDevice.State));
        AddUpdateSensor(defaultInputDeviceStateId, defaultInputDeviceStateSensor);

        var defaultInputDeviceIsMuted = inputDevice.AudioEndpointVolume?.Mute ?? false;
        var defaultInputDeviceIsMutedEntityName = $"{parentSensorSafeName}_default_input_device_muted";
        var defaultInputDeviceIsMutedId = $"{Id}_default_input_device_muted";
        var defaultInputDeviceIsMutedSensor = new DataTypeBoolSensor(_updateInterval, defaultInputDeviceIsMutedEntityName, $"Default Input Device Muted", defaultInputDeviceIsMutedId, string.Empty, "mdi:microphone", EntityName);
        defaultInputDeviceIsMutedSensor.SetState(defaultInputDeviceIsMuted);
        AddUpdateSensor(defaultInputDeviceIsMutedId, defaultInputDeviceIsMutedSensor);

        var inputVolume = (int)GetDefaultInputDevicePeakVolume(inputDevice);
        var defaultInputDeviceVolumeEntityName = $"{parentSensorSafeName}_default_input_device_volume";
        var defaultInputDeviceVolumeId = $"{Id}_default_input_device_volume";
        var defaultInputDeviceVolumeSensor = new DataTypeIntSensor(_updateInterval, defaultInputDeviceVolumeEntityName, $"Default Input Device Volume", defaultInputDeviceVolumeId, string.Empty, "measurement", "mdi:microphone", string.Empty, EntityName);
        defaultInputDeviceVolumeSensor.SetState(inputVolume);
        AddUpdateSensor(defaultInputDeviceVolumeId, defaultInputDeviceVolumeSensor);

        var audioInputDevices = GetAudioInputDevices();
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
            var parentSensorSafeName = SharedHelperFunctions.GetSafeValue(EntityName);
            var deviceSafeName = SharedHelperFunctions.GetSafeDeviceName();

            HandleAudioOutputSensors(parentSensorSafeName, deviceSafeName);
            HandleAudioInputSensors(parentSensorSafeName, deviceSafeName);

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

    private string GetSessionDisplayName(AudioSessionControl2 session)
    {
        var procId = (int)session.ProcessID;

        if (procId <= 0)
            return session.DisplayName;

        if (ApplicationNames.ContainsKey(procId))
            return ApplicationNames[procId];

        // we don't know this app yet, get process info
        using var p = Process.GetProcessById(procId);
        ApplicationNames.Add(procId, p.ProcessName);

        return p.ProcessName;
    }

    private List<AudioSessionInfo> GetSessions(out float peakVolume)
    {
        var sessionInfos = new List<AudioSessionInfo>();
        peakVolume = 0f;

        try
        {
            var errors = false;

            foreach (var device in Variables.AudioDeviceEnumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active))
            {
                using (device)
                {
                    foreach (var session in device.AudioSessionManager2?.Sessions.Where(x => x != null))
                    {
                        if (session.ProcessID == 0)
                            continue;

                        try
                        {
                            var displayName = GetSessionDisplayName(session);

                            if (displayName == "audiodg")
                                continue;

                            if (displayName.Length > 30)
                                displayName = $"{displayName[..30]}..";

                            var sessionInfo = new AudioSessionInfo
                            {
                                Application = displayName,
                                PlaybackDevice = device.DeviceFriendlyName,
                                Muted = session.SimpleAudioVolume?.Mute ?? false,
                                Active = session.State == AudioSessionState.AudioSessionStateActive,
                                MasterVolume = session.SimpleAudioVolume?.MasterVolume * 100 ?? 0f,
                                PeakVolume = session.AudioMeterInformation?.MasterPeakValue * 100 ?? 0f
                            };

                            // new max?
                            if (sessionInfo.PeakVolume > peakVolume)
                                peakVolume = sessionInfo.PeakVolume;

                            sessionInfos.Add(sessionInfo);
                        }
                        catch (Exception ex)
                        {
                            if (!_errorPrinted)
                                Log.Fatal(ex, "[AUDIO] [{name}] [{app}] Exception while retrieving info: {err}", EntityName, session.DisplayName, ex.Message);

                            errors = true;
                        }
                        finally
                        {
                            session?.Dispose();
                        }
                    }
                }
            }

            // only print errors once
            if (errors && !_errorPrinted)
            {
                _errorPrinted = true;

                return sessionInfos;
            }

            // optionally reset error flag
            if (_errorPrinted)
                _errorPrinted = false;
        }
        catch (Exception ex)
        {
            // something went wrong, only print once
            if (_errorPrinted)
                return sessionInfos;

            _errorPrinted = true;

            Log.Fatal(ex, "[AUDIO] [{name}] Fatal exception while getting sessions: {err}", EntityName, ex.Message);
        }

        return sessionInfos;
    }

    private float GetDefaultInputDevicePeakVolume(MMDevice inputDevice)
    {
        if (inputDevice == null)
            return 0f;

        var peakVolume = 0f;

        try
        {
            var errors = false;

            // process sessions (and get peak volume)
            foreach (var session in inputDevice.AudioSessionManager2?.Sessions?.Where(x => x != null)!)
            {
                try
                {
                    // filter inactive sessions
                    if (session.State != AudioSessionState.AudioSessionStateActive)
                        continue;

                    // set peak volume
                    var sessionPeakVolume = session.AudioMeterInformation?.MasterPeakValue * 100 ?? 0f;

                    // new max?
                    if (sessionPeakVolume > peakVolume)
                        peakVolume = sessionPeakVolume;
                }
                catch (Exception ex)
                {
                    if (!_errorPrinted)
                        Log.Fatal(ex, "[AUDIO] [{name}] [{app}] Exception while retrieving input info: {err}", EntityName, session.DisplayName, ex.Message);

                    errors = true;
                }
                finally
                {
                    session?.Dispose();
                }
            }

            // only print errors once
            if (errors && !_errorPrinted)
            {
                _errorPrinted = true;

                return peakVolume;
            }

            // optionally reset error flag
            if (_errorPrinted)
                _errorPrinted = false;
        }
        catch (Exception ex)
        {
            // something went wrong, only print once
            if (_errorPrinted)
                return peakVolume;

            _errorPrinted = true;

            Log.Fatal(ex, "[AUDIO] [{name}] Fatal exception while getting input info: {err}", EntityName, ex.Message);
        }

        return peakVolume;
    }

    /// <summary>
    /// Converts the audio device's state to a better readable form
    /// </summary>
    /// <param name="state"></param>
    /// <returns></returns>
    private static string GetReadableState(DeviceState state)
    {
        return state switch
        {
            DeviceState.Active => "ACTIVE",
            DeviceState.Disabled => "DISABLED",
            DeviceState.NotPresent => "NOT PRESENT",
            DeviceState.Unplugged => "UNPLUGGED",
            DeviceState.MaskAll => "STATEMASK_ALL",
            _ => "UNKNOWN"
        };
    }

    public override DiscoveryConfigModel GetAutoDiscoveryConfig() => null;
}
