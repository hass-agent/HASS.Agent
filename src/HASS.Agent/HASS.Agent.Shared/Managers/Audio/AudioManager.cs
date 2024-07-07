using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.CoreAudioApi;
using HASS.Agent.Shared.Managers.Audio.Exceptions;
using HASS.Agent.Shared.Managers.Audio.Internal;
using Serilog;
using NAudio.CoreAudioApi.Interfaces;
using HidSharp;

namespace HASS.Agent.Shared.Managers.Audio;
public static class AudioManager
{
    private static bool _initialized = false;

    private static MMDeviceEnumerator _enumerator = null;
    private static MMNotificationClient _notificationClient = null;

    private static readonly ConcurrentDictionary<string, string> _devices = new();
    private static readonly ConcurrentQueue<string> _devicesToBeRemoved = new();
    private static readonly ConcurrentQueue<string> _devicesToBeAdded = new();

    private static readonly Dictionary<int, string> _applicationNameCache = new();

    private static void InitializeDevices()
    {
        _enumerator = new MMDeviceEnumerator();

        foreach (var device in _enumerator.EnumerateAudioEndPoints(DataFlow.All, DeviceState.Active))
            _devices[device.ID] = device.FriendlyName;

        _notificationClient = new MMNotificationClient();
        _notificationClient.DeviceAdded += DeviceAdded;
        _notificationClient.DeviceRemoved += DeviceRemoved;
        _notificationClient.DeviceStateChanged += DeviceStateChanged;
        _enumerator.RegisterEndpointNotificationCallback(_notificationClient);

        _initialized = true;
    }

    private static void DeviceStateChanged(object sender, DeviceStateChangedEventArgs e)
    {
        switch (e.DeviceState)
        {
            case DeviceState.Active:
                if (_devices.ContainsKey(e.DeviceId))
                    return;

                _devices.Remove(e.DeviceId, out var _);
                //_devicesToBeAdded.Enqueue(e.DeviceId);
                break;

            case DeviceState.NotPresent:
            case DeviceState.Unplugged:
            case DeviceState.Disabled:
                {
                    using var device = _enumerator.GetDevice(e.DeviceId);
                    _devices[e.DeviceId] = device.FriendlyName;
                    //_devicesToBeRemoved.Enqueue(e.DeviceId);
                    break;
                }

            default:
                break;
        }
    }

    private static void DeviceRemoved(object sender, DeviceNotificationEventArgs e)
    {
        _devicesToBeRemoved.Enqueue(e.DeviceId);
    }

    private static void DeviceAdded(object sender, DeviceNotificationEventArgs e)
    {
        if (_devices.ContainsKey(e.DeviceId))
            return;

        _devicesToBeAdded.Enqueue(e.DeviceId);
    }

    private static bool CheckInitialization()
    {
        if (!_initialized)
        {
            Log.Warning("[AUDIOMGR] not yet initialized!");

            return false;
        }

        /*        while (_devicesToBeRemoved.TryDequeue(out var deviceId))
                {
                    if (_devices.Remove(deviceId, out var device))
                        device.Dispose();
                }

                while (_devicesToBeAdded.TryDequeue(out var deviceId))
                {
                    var device = _enumerator.GetDevice(deviceId);
                    _devices[deviceId] = new InternalAudioDevice(device);
                }*/

        return true;
    }

    private static string GetSessionDisplayName(InternalAudioSession session)
    {
        var procId = session.ProcessId;

        if (procId <= 0)
            return session.DisplayName;

        if (_applicationNameCache.TryGetValue(procId, out var cachedName))
            return cachedName;

        using var process = Process.GetProcessById(procId);
        _applicationNameCache[procId] = process.ProcessName;

        return process.ProcessName;
    }

    private static List<AudioSession> GetDeviceSessions(MMDevice mmDevice)
    {
        using var internalAudioSessionManager = new InternalAudioSessionManager(mmDevice.AudioSessionManager);
        return GetDeviceSessions(mmDevice.FriendlyName, internalAudioSessionManager);
    }

    private static List<AudioSession> GetDeviceSessions(string deviceName, InternalAudioSessionManager internalAudioSessionManager)
    {
        var audioSessions = new List<AudioSession>();

        foreach (var (sessionId, session) in internalAudioSessionManager.Sessions)
        {
            try
            {
                var displayName = string.IsNullOrWhiteSpace(session.DisplayName) ? GetSessionDisplayName(session) : session.DisplayName;
                if (displayName == "audiodg")
                    continue;

                if (displayName.Length > 30)
                    displayName = $"{displayName[..30]}..";

                var audioSession = new AudioSession
                {
                    Id = sessionId,
                    Application = displayName,
                    PlaybackDevice = deviceName,
                    Muted = session.Volume.Mute,
                    Active = session.Control.State == AudioSessionState.AudioSessionStateActive,
                    MasterVolume = Convert.ToInt32(session.Volume.Volume * 100),
                    PeakVolume = Convert.ToDouble(session.MeterInformation.MasterPeakValue * 100)
                };

                audioSessions.Add(audioSession);
            }
            catch (Exception ex)
            {
                throw new AudioSessionException($"error retrieving session information for {deviceName}", ex);
            }
        }

        return audioSessions;
    }

    private static string GetReadableState(DeviceState state)
    {
        return state switch
        {
            DeviceState.Active => "ACTIVE",
            DeviceState.Disabled => "DISABLED",
            DeviceState.NotPresent => "NOT PRESENT",
            DeviceState.Unplugged => "UNPLUGGED",
            DeviceState.All => "STATEMASK_ALL",
            _ => "UNKNOWN"
        };
    }

    public static void Initialize()
    {
        Log.Debug("[AUDIOMGR] initializing");

        InitializeDevices();

        Log.Information("[AUDIOMGR] initialized");
    }

    public static void ReInitialize()
    {
        if (_initialized)
            CleanupDevices();

        Log.Debug("[AUDIOMGR] re-initializing");

        InitializeDevices();

        Log.Information("[AUDIOMGR] re-initialized");
    }

    public static void CleanupDevices()
    {
        Log.Debug("[AUDIOMGR] starting cleanup");
        _initialized = false;

        /*        foreach (var device in _devices.Values)
                    device.Dispose();*/

        _notificationClient.DeviceAdded -= DeviceAdded;
        _notificationClient.DeviceRemoved -= DeviceRemoved;
        _notificationClient.DeviceStateChanged -= DeviceStateChanged;
        _enumerator.UnregisterEndpointNotificationCallback(_notificationClient);

        _enumerator.Dispose();

        _devices.Clear();
        Log.Debug("[AUDIOMGR] cleanup completed");
    }

    public static List<AudioDevice> GetDevices()
    {
        var audioDevices = new List<AudioDevice>();

        if (!CheckInitialization())
            return audioDevices;

        try
        {
            using var defaultInputDevice = _enumerator.GetDefaultAudioEndpoint(DataFlow.Capture, Role.Multimedia);
            using var defaultOutputDevice = _enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);

            var defaultInputDeviceId = defaultInputDevice.ID;
            var defaultOutputDeviceId = defaultOutputDevice.ID;

            foreach (var deviceId in _devices.Keys)
            {
                using var device = _enumerator.GetDevice(deviceId);

                var audioSessions = GetDeviceSessions(device);
                var loudestSession = audioSessions.MaxBy(s => s.PeakVolume);

                var audioDevice = new AudioDevice
                {
                    State = GetReadableState(device.State),
                    Type = device.DataFlow == DataFlow.Capture ? DeviceType.Input : DeviceType.Output,
                    Id = device.ID,
                    FriendlyName = device.FriendlyName,
                    Volume = Convert.ToInt32(Math.Round(device.AudioEndpointVolume.MasterVolumeLevelScalar * 100, 0)),
                    Muted = device.AudioEndpointVolume.Mute,
                    PeakVolume = loudestSession == null ? 0 : loudestSession.PeakVolume,
                    Sessions = audioSessions,
                    Default = device.ID == defaultInputDeviceId || device.ID == defaultOutputDeviceId
                };

                audioDevices.Add(audioDevice);
            }
        }
        catch (Exception ex)
        {
            Log.Debug(ex, "[AUDIOMGR] Failed to retrieve devices: {msg}", ex.Message);
        }

        return audioDevices;
    }

    public static string GetDefaultDeviceId(DeviceType deviceType, DeviceRole deviceRole)
    {
        if (!CheckInitialization())
            return string.Empty;

        var dataFlow = deviceType == DeviceType.Input ? DataFlow.Capture : DataFlow.Render;
        var role = (Role)deviceRole;

        using var defaultDevice = _enumerator.GetDefaultAudioEndpoint(dataFlow, role);

        return defaultDevice.ID;
    }

    public static void ActivateDevice(string deviceName)
    {
        if (!CheckInitialization())
            return;

        var deviceId = _devices.FirstOrDefault(v => v.Value == deviceName).Key;
        if (string.IsNullOrWhiteSpace(deviceId))
            return;

        using var configClient = new CPolicyConfigVistaClient();
        configClient.SetDefaultDevice(deviceId);
    }

    public static void SetDeviceProperties(string deviceName, int volume, bool? mute)
    {
        if (!CheckInitialization())
            return;

        var deviceId = _devices.FirstOrDefault(v => v.Value == deviceName).Key;
        if (string.IsNullOrWhiteSpace(deviceId))
            return;

        using var device = _enumerator.GetDevice(deviceId);
        SetDeviceProperties(device, volume, mute);
    }

    private static void SetDeviceProperties(MMDevice device, int volume, bool? mute)
    {
        if (mute != null)
        {
            device.AudioEndpointVolume.Mute = (bool)mute;
        }

        if (volume != -1)
        {
            var volumeScalar = volume / 100f;
            device.AudioEndpointVolume.MasterVolumeLevelScalar = volumeScalar;
        }
    }

    public static void SetDefaultDeviceProperties(DeviceType type, DeviceRole deviceRole, int volume, bool? mute)
    {
        var flow = type == DeviceType.Output ? DataFlow.Render : DataFlow.Capture;
        var role = (Role)deviceRole;
        using var device = _enumerator.GetDefaultAudioEndpoint(flow, role);
        SetDeviceProperties(device, volume, mute);
    }

    public static void SetApplicationProperties(string deviceName, string applicationName, string sessionId, int volume, bool mute)
    {
        if (!CheckInitialization())
            return;

        var deviceId = _devices.FirstOrDefault(v => v.Value == deviceName).Key;
        if (string.IsNullOrWhiteSpace(deviceId))
            return;

        using var device = _enumerator.GetDevice(deviceId);
        using var sessionManager = new InternalAudioSessionManager(device.AudioSessionManager);
        var sessions = GetDeviceSessions(device.FriendlyName, sessionManager);

        var applicationAudioSessions = sessions.Where(s =>
            s.Application == applicationName
        );

        if (string.IsNullOrWhiteSpace(sessionId))
        {
            foreach (var session in applicationAudioSessions)
            {
                if (!sessionManager.Sessions.TryGetValue(session.Id, out var internalSession))
                    return;

                SetSessionProperties(internalSession, volume, mute);
            }
        }
        else
        {
            if (!sessionManager.Sessions.TryGetValue(sessionId, out var internalSession))
            {
                Log.Debug("[SETAPPVOLUME] No session {actionData.SessionId} found for device {device}", applicationName, deviceName);
                return;
            }

            SetSessionProperties(internalSession, volume, mute);
        }
    }

    private static void SetSessionProperties(InternalAudioSession internalAudioSession, int volume, bool mute)
    {
        internalAudioSession.Volume.Mute = mute;

        if (volume == -1)
            return;

        var volumeScalar = volume / 100f;
        internalAudioSession.Volume.Volume = volumeScalar;
    }

    public static void Shutdown()
    {
        Log.Debug("[AUDIOMGR] shutting down");
        try
        {
            CleanupDevices();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "[AUDIOMGR] shutdown fatal error: {ex}", ex.Message);
        }
        Log.Debug("[AUDIOMGR] shutdown completed");
    }
}
