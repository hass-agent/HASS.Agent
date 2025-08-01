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
using Microsoft.VisualBasic.ApplicationServices;

namespace HASS.Agent.Shared.Managers.Audio;
public static class AudioManager
{
    private static bool _initialized = false;

    private static MMDeviceEnumerator _enumerator = null;
    private static MMNotificationClient _notificationClient = null;

    private static readonly ConcurrentDictionary<string, string> _devices = new();

    private static readonly Dictionary<int, string> _applicationNameCache = new();

    private static bool _noDefaultInputLogged = false;
    private static bool _noDefaultOutputLogged = false;

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

    private static void AddDevice(string deviceId)
    {
        if (_devices.ContainsKey(deviceId))
            return;

        using var device = _enumerator.GetDevice(deviceId);
        _devices[deviceId] = device.FriendlyName;
        Log.Debug($"[AUDIOMGR] added device: {_devices[deviceId]}");
    }

    private static void RemoveDevice(string deviceId)
    {
        _devices.Remove(deviceId, out var removedDeviceName);
        if (!string.IsNullOrWhiteSpace(removedDeviceName))
            Log.Debug($"[AUDIOMGR] removed device: {removedDeviceName}");
    }

    private static void DeviceRemoved(object sender, DeviceNotificationEventArgs e)
    {
        try
        {
            RemoveDevice(e.DeviceId);
        }
        catch
        {
            Log.Error($"[AUDIOMGR] failed to remove device: {e.DeviceId}");
        }
    }
    private static void DeviceAdded(object sender, DeviceNotificationEventArgs e)
    {
        try
        {
            AddDevice(e.DeviceId);
        }
        catch
        {
            Log.Error($"[AUDIOMGR] failed to add device: {e.DeviceId}");
        }
    }

    private static void DeviceStateChanged(object sender, DeviceStateChangedEventArgs e)
    {
        switch (e.DeviceState)
        {
            case DeviceState.Active:
                AddDevice(e.DeviceId);
                break;

            case DeviceState.NotPresent:
            case DeviceState.Unplugged:
            case DeviceState.Disabled:
                RemoveDevice(e.DeviceId);
                break;

            default:
                break;
        }
    }

    private static bool CheckInitialization()
    {
        if (!_initialized)
        {
            Log.Warning("[AUDIOMGR] not yet initialized!");
            return false;
        }

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
        return GetDeviceSessions(_devices[mmDevice.ID], internalAudioSessionManager);
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

    public static void CleanupDevices()
    {
        Log.Debug("[AUDIOMGR] starting cleanup");
        _initialized = false;

        _notificationClient.DeviceAdded -= DeviceAdded;
        _notificationClient.DeviceRemoved -= DeviceRemoved;
        _notificationClient.DeviceStateChanged -= DeviceStateChanged;
        _enumerator.UnregisterEndpointNotificationCallback(_notificationClient);

        _enumerator.Dispose();

        _devices.Clear();
        Log.Debug("[AUDIOMGR] cleanup completed");
    }

    private static MMDevice GetDefaultMultimediaDevice(DataFlow dataFlow)
    {
        MMDevice device = null;
        try
        {
            device = _enumerator.GetDefaultAudioEndpoint(dataFlow, Role.Multimedia);
        }
        catch { }

        return device;
    }

    public static List<AudioDevice> GetDevices()
    {
        var audioDevices = new List<AudioDevice>();

        if (!CheckInitialization())
            return audioDevices;

        try
        {
            using var defaultInputDevice = GetDefaultMultimediaDevice(DataFlow.Capture);
            if (defaultInputDevice == null && !_noDefaultInputLogged)
            {
                Log.Information("[AUDIOMGR] no default input device detected");
                _noDefaultInputLogged = true;
            }
            else if (defaultInputDevice != null)
            {
                _noDefaultInputLogged = false;
            }

            using var defaultOutputDevice = GetDefaultMultimediaDevice(DataFlow.Render);
            if (defaultOutputDevice == null && !_noDefaultOutputLogged)
            {
                Log.Information("[AUDIOMGR] no default output device detected");
                _noDefaultOutputLogged = true;
            }else if (defaultOutputDevice != null)
            {
                _noDefaultOutputLogged = false;
            }

            var defaultInputDeviceId = defaultInputDevice != null ? defaultInputDevice.ID : string.Empty;
            var defaultOutputDeviceId = defaultOutputDevice != null ? defaultOutputDevice.ID : string.Empty;

            foreach (var (deviceId, deviceName) in _devices)
            {
                using var device = _enumerator.GetDevice(deviceId);

                var audioSessions = GetDeviceSessions(device);
                var loudestSession = audioSessions.MaxBy(s => s.PeakVolume);

                var audioDevice = new AudioDevice
                {
                    State = GetReadableState(device.State),
                    Type = device.DataFlow == DataFlow.Capture ? DeviceType.Input : DeviceType.Output,
                    Id = device.ID,
                    FriendlyName = deviceName,
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
            Log.Error(ex, "[AUDIOMGR] failed to retrieve devices: {msg}", ex.Message);
        }

        return audioDevices;
    }

    public static string GetDefaultDeviceId(DeviceType deviceType, DeviceRole deviceRole)
    {
        if (!CheckInitialization())
            return string.Empty;

        var deviceId = string.Empty;

        try
        {
            var dataFlow = deviceType == DeviceType.Input ? DataFlow.Capture : DataFlow.Render;
            var role = (Role)deviceRole;

            using var defaultDevice = _enumerator.GetDefaultAudioEndpoint(dataFlow, role);

            deviceId = defaultDevice.ID;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "[AUDIOMGR] failed to retrieve default device id: {msg}", ex.Message);
        }

        return deviceId;
    }

    public static int GetDefaultDeviceVolume(DeviceType deviceType, DeviceRole deviceRole)
    {
        if (!CheckInitialization())
            return 0;

        var volume = 0;

        try
        {
            var dataFlow = deviceType == DeviceType.Input ? DataFlow.Capture : DataFlow.Render;
            var role = (Role)deviceRole;

            using var defaultDevice = _enumerator.GetDefaultAudioEndpoint(dataFlow, role);

            volume = Convert.ToInt32(Math.Round(defaultDevice.AudioEndpointVolume.MasterVolumeLevelScalar * 100, 0));
        }
        catch (Exception ex)
        {
            Log.Error(ex, "[AUDIOMGR] failed to retrieve default device volume: {msg}", ex.Message);
        }

        return volume;
    }

    public static bool GetDefaultDeviceMute(DeviceType deviceType, DeviceRole deviceRole)
    {
        if (!CheckInitialization())
            return false;

        var mute = false;

        try
        {
            var dataFlow = deviceType == DeviceType.Input ? DataFlow.Capture : DataFlow.Render;
            var role = (Role)deviceRole;

            using var defaultDevice = _enumerator.GetDefaultAudioEndpoint(dataFlow, role);

            mute = defaultDevice.AudioEndpointVolume.Mute;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "[AUDIOMGR] failed to retrieve default device mute: {msg}", ex.Message);
        }

        return mute;
    }

    public static void ActivateDevice(string deviceName)
    {
        if (!CheckInitialization())
            return;

        try
        {
            var deviceId = _devices.FirstOrDefault(v => v.Value == deviceName).Key;
            if (string.IsNullOrWhiteSpace(deviceId))
                return;

            using var configClient = new CPolicyConfigVistaClient();
            configClient.SetDefaultDevice(deviceId);

            Log.Debug($"[AUDIOMGR] device '{deviceName}' activated");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "[AUDIOMGR] failed to activate device '{deviceName}': {msg}", deviceName, ex.Message);
        }
    }

    public static void SetDeviceProperties(string deviceName, int volume, bool? mute)
    {
        if (!CheckInitialization())
            return;

        try
        {
            var deviceId = _devices.FirstOrDefault(v => v.Value == deviceName).Key;
            if (string.IsNullOrWhiteSpace(deviceId))
                return;

            using var device = _enumerator.GetDevice(deviceId);
            SetDeviceProperties(device, volume, mute);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "[AUDIOMGR] failed to set device properties '{deviceName}': {msg}", deviceName, ex.Message);
        }
    }

    private static void SetDeviceProperties(MMDevice device, int volume, bool? mute)
    {
        if (mute != null)
        {
            device.AudioEndpointVolume.Mute = (bool)mute;
            Log.Debug($"[AUDIOMGR] mute for '{_devices[device.ID]}' set to '{mute}'");
        }

        if (volume != -1)
        {
            var volumeScalar = Math.Clamp(volume / 100f, 0, 1);
            device.AudioEndpointVolume.MasterVolumeLevelScalar = volumeScalar;
            Log.Debug($"[AUDIOMGR] volume for '{_devices[device.ID]}' set to '{volume}'/'{volumeScalar}'");
        }
    }

    public static void SetDefaultDeviceProperties(DeviceType type, DeviceRole deviceRole, int volume, bool? mute)
    {
        try
        {
            var flow = type == DeviceType.Output ? DataFlow.Render : DataFlow.Capture;
            var role = (Role)deviceRole;
            using var device = _enumerator.GetDefaultAudioEndpoint(flow, role);
            SetDeviceProperties(device, volume, mute);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "[AUDIOMGR] failed to set default device properties: {msg}", ex.Message);
        }
    }

    public static void SetApplicationProperties(string deviceName, string applicationName, string sessionId, int volume, bool mute)
    {
        if (!CheckInitialization())
            return;

        try
        {
            var deviceId = _devices.FirstOrDefault(v => v.Value == deviceName).Key;
            if (string.IsNullOrWhiteSpace(deviceId))
                return;

            using var device = _enumerator.GetDevice(deviceId);
            using var sessionManager = new InternalAudioSessionManager(device.AudioSessionManager);
            var sessions = GetDeviceSessions(deviceName, sessionManager);

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
                    Log.Debug("[AUDIOMGR] no session '{sessionId}' found for device '{device}'", sessionId, deviceName);
                    return;
                }

                SetSessionProperties(internalSession, volume, mute);
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "[AUDIOMGR] failed to set application properties '{appName}': {msg}", applicationName, ex.Message);
        }
    }

    private static void SetSessionProperties(InternalAudioSession internalAudioSession, int volume, bool mute)
    {
        var displayName = string.IsNullOrWhiteSpace(internalAudioSession.DisplayName) ? GetSessionDisplayName(internalAudioSession) : internalAudioSession.DisplayName;

        internalAudioSession.Volume.Mute = mute;
        Log.Debug("[AUDIOMGR] mute for '{sessionName}' ({sessionId}) set to '{mute}'", displayName, internalAudioSession.Control.GetSessionInstanceIdentifier, mute);

        if (volume == -1)
            return;

        var volumeScalar = Math.Clamp(volume / 100f, 0, 1);
        internalAudioSession.Volume.Volume = volumeScalar;
        Log.Debug("[AUDIOMGR] volume for '{sessionName}' ({sessionId}) set to '{vol}'/'{volScal}'", displayName, internalAudioSession.Control.GetSessionInstanceIdentifier, volume, volumeScalar);
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
