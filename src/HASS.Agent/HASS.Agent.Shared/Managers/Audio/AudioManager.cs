using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSCore.CoreAudioAPI;
using HASS.Agent.Shared.Managers.Audio.Exceptions;
using HASS.Agent.Shared.Managers.Audio.Internal;
using Serilog;

namespace HASS.Agent.Shared.Managers.Audio;
public static class AudioManager
{
    private static bool _initialized = false;

    private static readonly MMDeviceEnumerator _enumerator = new();

    private static readonly ConcurrentDictionary<string, InternalAudioDevice> _devices = new();
    private static readonly ConcurrentQueue<string> _devicesToBeRemoved = new();
    private static readonly ConcurrentQueue<string> _devicesToBeAdded = new();

    private static readonly Dictionary<int, string> _applicationNameCache = new();

    public static void Initialize()
    {
        Log.Debug("[AUDIOMGR] Audio Manager initializing");

        foreach (var device in _enumerator.EnumAudioEndpoints(DataFlow.All, DeviceState.Active))
            _devices[device.DeviceID] = new InternalAudioDevice(device);

        var nc = new MMNotificationClient(_enumerator);
        nc.DeviceAdded += (sender, e) =>
        {
            if (_devices.ContainsKey(e.DeviceId))
                return;

            _devicesToBeAdded.Enqueue(e.DeviceId);
        };

        nc.DeviceRemoved += (sender, e) =>
        {
            _devicesToBeRemoved.Enqueue(e.DeviceId);
        };

        _initialized = true;

        nc.DeviceStateChanged += (sender, e) =>
        {
            switch (e.DeviceState)
            {
                case DeviceState.Active:
                    if (_devices.ContainsKey(e.DeviceId))
                        return;

                    _devicesToBeAdded.Enqueue(e.DeviceId);
                    break;

                case DeviceState.NotPresent:
                case DeviceState.UnPlugged:
                    _devicesToBeRemoved.Enqueue(e.DeviceId);
                    break;

                default:
                    break;
            }
        };

        Log.Information("[AUDIOMGR] Audio Manager initialized");
    }

    private static void CheckInitialization()
    {
        if (!_initialized)
            throw new InvalidOperationException("AudioManager is not initialized");

        while (_devicesToBeRemoved.TryDequeue(out var deviceId))
        {
            if (_devices.Remove(deviceId, out var device))
                device.Dispose();
        }

        while (_devicesToBeAdded.TryDequeue(out var deviceId))
        {
            var device = _enumerator.GetDevice(deviceId);
            _devices[deviceId] = new InternalAudioDevice(device);
        }
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

    private static List<AudioSession> GetDeviceSessions(InternalAudioDevice internalAudioDevice)
    {
        var audioSessions = new List<AudioSession>();

        internalAudioDevice.Manager.RemoveDisconnectedSessions();
        foreach (var (sessionId, session) in internalAudioDevice.Manager.Sessions)
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
                    PlaybackDevice = internalAudioDevice.FriendlyName,
                    Muted = session.Volume.IsMuted,
                    Active = session.Control.SessionState == AudioSessionState.AudioSessionStateActive,
                    MasterVolume = Convert.ToInt32(session.Volume.MasterVolume * 100),
                    PeakVolume = Convert.ToInt32(session.MeterInformation.PeakValue * 100)
                };

                audioSessions.Add(audioSession);
            }
            catch (Exception ex)
            {
                throw new AudioSessionException($"error retrieving session information for {internalAudioDevice.FriendlyName}", ex);
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
            DeviceState.UnPlugged => "UNPLUGGED",
            DeviceState.All => "STATEMASK_ALL",
            _ => "UNKNOWN"
        };
    }

    public static List<AudioDevice> GetDevices()
    {
        CheckInitialization();

        var audioDevices = new List<AudioDevice>();

        using var defaultInputDevice = _enumerator.GetDefaultAudioEndpoint(DataFlow.Capture, Role.Multimedia);
        using var defaultOutputDevice = _enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);

        var defaultInputDeviceId = defaultInputDevice.DeviceID;
        var defaultOutputDeviceId = defaultOutputDevice.DeviceID;

        foreach (var device in _devices.Values.Where(d => d.MMDevice.DeviceState == DeviceState.Active))
        {

            var audioSessions = GetDeviceSessions(device);
            var loudestSession = audioSessions.MaxBy(s => s.PeakVolume);

            var audioDevice = new AudioDevice
            {
                State = GetReadableState(device.MMDevice.DeviceState),
                Type = device.MMDevice.DataFlow == DataFlow.Capture ? DeviceType.Input : DeviceType.Output,
                Id = device.DeviceId,
                FriendlyName = device.FriendlyName,
                Volume = Convert.ToInt32(Math.Round(device.AudioEndpointVolume.GetMasterVolumeLevelScalar() * 100, 0)),
                PeakVolume = loudestSession == null ? 0 : loudestSession.PeakVolume,
                Sessions = audioSessions,
                Default = device.DeviceId == defaultInputDeviceId || device.DeviceId == defaultOutputDeviceId
            };

            audioDevices.Add(audioDevice);
        }

        return audioDevices;
    }

    public static string GetDefaultDeviceId(DeviceType deviceType, DeviceRole deviceRole)
    {
        var dataFlow = deviceType == DeviceType.Input ? DataFlow.Capture : DataFlow.Render;
        var role = (Role)deviceRole;

        using var defaultDevice = _enumerator.GetDefaultAudioEndpoint(dataFlow, role);

        return defaultDevice.DeviceID;
    }

    public static void Activate(AudioDevice audioDevice)
    {
        if (!_devices.TryGetValue(audioDevice.Id, out var device))
            return;

        device.Activate();
    }

    public static void SetVolume(AudioDevice audioDevice, int volume)
    {
        if (!_devices.TryGetValue(audioDevice.Id, out var device))
            return;

        var volumeScalar = volume / 100f;
        device.AudioEndpointVolume.SetMasterVolumeLevelScalar(volumeScalar, Guid.Empty);
    }

    public static void SetVolume(AudioSession audioSession, int volume)
    {
        var device = _devices.Values.Where(d => d.FriendlyName == audioSession.PlaybackDevice).FirstOrDefault();
        if (device == null)
            return;

        if (!device.Manager.Sessions.TryGetValue(audioSession.Id, out var session))
            return;

        var volumeScalar = volume / 100f;
        session.Volume.MasterVolume = volumeScalar;
    }

    public static void SetMute(AudioDevice audioDevice, bool mute)
    {
        if (!_devices.TryGetValue(audioDevice.Id, out var device))
            return;

        device.AudioEndpointVolume.SetMute(mute, Guid.Empty);
    }

    public static void SetMute(AudioSession audioSession, bool mute)
    {
        var device = _devices.Values.Where(d => d.FriendlyName == audioSession.PlaybackDevice).FirstOrDefault();
        if (device == null)
            return;

        if (!device.Manager.Sessions.TryGetValue(audioSession.Id, out var session))
            return;

        session.Volume.IsMuted = mute;
    }

    public static void Shutdown()
    {
        Log.Debug("[AUDIOMGR] Audio Manager shutting down");
        try
        {
            foreach(var device in _devices.Values)
                device.Dispose();

            _enumerator.Dispose();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "[AUDIOMGR] Audio Manager shutdown fatal error: {ex}", ex.Message);
        }
        Log.Debug("[AUDIOMGR] Audio Manager shutdown completed");
    }
}
