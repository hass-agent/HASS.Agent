using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AudioSwitcher.AudioApi;
using AudioSwitcher.AudioApi.CoreAudio;
using AudioSwitcher.AudioApi.Session;
using HASS.Agent.Shared.Models.Internal;
using Serilog;
using HidSharp;

namespace HASS.Agent.Shared.Managers;
public static class AudioManager
{
    private static CoreAudioController _coreAudioController;
    private static bool _initialized = false;

    private static readonly Dictionary<int, string> _applicationNames = new();
    private static readonly Dictionary<string, int> _devicesPeakVolume = new();
    private static readonly Dictionary<string, int> _sessionsPeakVolume = new();

    private static bool _errorPrinted = false;

    public static void Initialize()
    {
        Log.Debug("[AUDIOMGR] Initializing");
        _coreAudioController = new CoreAudioController();

        _coreAudioController.AudioDeviceChanged.Subscribe(d =>
        {
            switch (d.ChangedType)
            {
                case DeviceChangedType.DeviceAdded:
                    SubscribeToDevicePeakValue(d.Device);

                    break;
                case DeviceChangedType.DeviceRemoved:
                    if (_devicesPeakVolume.ContainsKey(d.Device.FullName))
                        _devicesPeakVolume.Remove(d.Device.FullName);
                    break;
            }
        });

        foreach(var audioDevice in _coreAudioController.GetDevices())
        {
            SubscribeToDevicePeakValue(audioDevice);
            var audioSessionController = audioDevice.GetCapability<IAudioSessionController>();
            if(audioSessionController == null)
                continue;
            
            foreach(var session in audioSessionController.ActiveSessions())
            {
                SubscribeToSessionPeakValue(session);
            }
        }

        _initialized = true;
        Log.Information("[AUDIOMGR] Initialized");
    }

    private static void SubscribeToSessionPeakValue(IAudioSession audioSession)
    {
/*        audioSession.PeakValueChanged.Subscribe(v =>
        {
            _sessionsPeakVolume[v.Session.Id] = (int)v.PeakValue;
        });*/
    }

    private static void SubscribeToDevicePeakValue(IDevice audioDevice)
    {
        if (_devicesPeakVolume.ContainsKey(audioDevice.FullName))
            return;

        audioDevice.PeakValueChanged.Subscribe(valueChangedArgs =>
        {
            _devicesPeakVolume[audioDevice.FullName] = (int)valueChangedArgs.PeakValue;
        });

        var audioSessionController = audioDevice.GetCapability<IAudioSessionController>();
        audioSessionController?.SessionCreated.Subscribe(session =>
        {
            SubscribeToSessionPeakValue(session);
        });
    }

    private static void CheckInitialization()
    {
        if (!_initialized)
            throw new InvalidOperationException("AudioManager is not initialized");
    }

    public static IEnumerable<CoreAudioDevice> GetPlaybackDevices(DeviceState deviceState = DeviceState.Active)
    {
        CheckInitialization();

        return _coreAudioController.GetDevices(DeviceType.Playback, deviceState);
    }

    public static IEnumerable<CoreAudioDevice> GetCaptureDevices(DeviceState deviceState = DeviceState.Active)
    {
        CheckInitialization();

        return _coreAudioController.GetDevices(DeviceType.Capture, deviceState);
    }

    public static CoreAudioDevice GetDefaultDevice(DeviceType deviceType, Role deviceRole)
    {
        CheckInitialization();

        return _coreAudioController.GetDefaultDevice(deviceType, deviceRole);
    }

    private static string GetSessionDisplayName(IAudioSession session)
    {
        var procId = session.ProcessId;

        if (procId <= 0)
            return session.DisplayName;

        if (_applicationNames.ContainsKey(procId))
            return _applicationNames[procId];

        using var process = Process.GetProcessById(procId);
        _applicationNames.Add(procId, process.ProcessName);

        return process.ProcessName;
    }

    public static List<AudioSessionInfo> GetActiveAudioSessionsInformation()
    {
        var sessionsInformation = new List<AudioSessionInfo>();

        try
        {
            var errors = false;

            foreach (var device in GetPlaybackDevices())
            {
                var audioSessionController = device.GetCapability<IAudioSessionController>();
                if(audioSessionController == null)
                    continue;

                foreach (var session in audioSessionController.ActiveSessions())
                {
                    if (session.ProcessId == 0)
                        continue;

                    try
                    {
                        var displayName = session.DisplayName ?? GetSessionDisplayName(session);
                        if (displayName == "audiodg")
                            continue;

                        if (displayName.Length > 30)
                            displayName = $"{displayName[..30]}..";

                        var sessionInfo = new AudioSessionInfo
                        {
                            Application = displayName,
                            PlaybackDevice = device.FullName,
                            Muted = session.IsMuted,
                            Active = session.SessionState == AudioSessionState.Active,
                            MasterVolume = (float)session.Volume,
                            PeakVolume = _sessionsPeakVolume.ContainsKey(session.Id) ? _sessionsPeakVolume[session.Id] : 0
                        };

                        sessionsInformation.Add(sessionInfo);
                    }
                    catch (Exception ex)
                    {
                        if (!_errorPrinted)
                            Log.Debug("[AUDIOMGR] [{app}] Exception while retrieving info: {err}", session.DisplayName, ex.Message);

                        errors = true;
                    }
                }

            }

            // only print errors once
            if (errors && !_errorPrinted)
            {
                _errorPrinted = true;

                return sessionsInformation;
            }

            // optionally reset error flag
            if (_errorPrinted)
                _errorPrinted = false;
        }
        catch (Exception ex)
        {
            // something went wrong, only print once
            if (_errorPrinted)
                return sessionsInformation;

            _errorPrinted = true;

            Log.Fatal(ex, "[AUDIO] Fatal exception while getting sessions: {err}", ex.Message);
        }

        return sessionsInformation;
    }

    public static int GetDevicePeakVolume(string deviceFullName)
    {
        return _devicesPeakVolume.ContainsKey(deviceFullName) ? _devicesPeakVolume[deviceFullName] : 0;
    }

    public static void Shutdown()
    {
        _initialized = false;
        _coreAudioController.Dispose();
    }
}
