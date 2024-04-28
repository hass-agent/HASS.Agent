using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSCore.CoreAudioAPI;

namespace HASS.Agent.Shared.Managers.Audio.Internal;
internal class InternalAudioSession : IDisposable, IAudioSessionEvents
{
    public AudioSessionControl Control { get; private set; }
    public AudioSessionControl2 Control2 { get; private set; }
    public SimpleAudioVolume Volume { get; private set; }
    public AudioMeterInformation MeterInformation { get; private set; }

    public string DisplayName { get; private set; }
    public int ProcessId { get; private set; }
    public bool Expired { get; private set; } = false;

    public InternalAudioSession(AudioSessionControl audioSessionControl)
    {
        Control = audioSessionControl;
        Control2 = audioSessionControl.QueryInterface<AudioSessionControl2>();
        Volume = audioSessionControl.QueryInterface<SimpleAudioVolume>();
        MeterInformation = audioSessionControl.QueryInterface<AudioMeterInformation>();

        DisplayName = Control2.DisplayName;
        ProcessId = Control2.ProcessID;

        Control.RegisterAudioSessionNotification(this);
    }

    public void Dispose()
    {
        Control.UnregisterAudioSessionNotification(this);

        MeterInformation?.Dispose();
        Volume?.Dispose();
        Control2?.Dispose();
        Control?.Dispose();

        GC.SuppressFinalize(this);
    }

    ~InternalAudioSession()
    {
        Dispose();
    }

    public void OnDisplayNameChanged(string newDisplayName, ref Guid eventContext)
    {

    }

    public void OnIconPathChanged(string newIconPath, ref Guid eventContext)
    {

    }

    public void OnSimpleVolumeChanged(float newVolume, bool newMute, ref Guid eventContext)
    {

    }

    public void OnChannelVolumeChanged(int channelCount, float[] newChannelVolumeArray, int changedChannel, ref Guid eventContext)
    {

    }

    public void OnGroupingParamChanged(ref Guid newGroupingParam, ref Guid eventContext)
    {

    }

    public void OnStateChanged(AudioSessionState newState)
    {
        if (newState == AudioSessionState.AudioSessionStateExpired)
            Expired = true;
    }

    public void OnSessionDisconnected(AudioSessionDisconnectReason disconnectReason)
    {

    }
}
