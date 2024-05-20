using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.CoreAudioApi;
using NAudio.CoreAudioApi.Interfaces;

namespace HASS.Agent.Shared.Managers.Audio.Internal;
internal class InternalAudioSession : IDisposable, IAudioSessionEventsHandler
{
    public AudioSessionControl Control { get; private set; }
    public SimpleAudioVolume Volume { get; private set; }
    public AudioMeterInformation MeterInformation { get; private set; }

    public string DisplayName { get; private set; }
    public int ProcessId { get; private set; }

    public bool Expired { get; private set; } = false;

    public InternalAudioSession(AudioSessionControl audioSessionControl)
    {
        Control = audioSessionControl;
        Volume = audioSessionControl.SimpleAudioVolume;
        MeterInformation = audioSessionControl.AudioMeterInformation;

        DisplayName = Control.DisplayName;
        ProcessId = Convert.ToInt32(Control.GetProcessID);

        Control.RegisterEventClient(this);
    }

    public void Dispose()
    {
        Control.UnRegisterEventClient(this);

        //MeterInformation?.Dispose();
        Volume?.Dispose();
        //Control2?.Dispose();
        Control?.Dispose();

        GC.SuppressFinalize(this);
    }

    public void OnVolumeChanged(float volume, bool isMuted)
    {

    }

    public void OnDisplayNameChanged(string displayName)
    {

    }

    public void OnIconPathChanged(string iconPath)
    {

    }

    public void OnChannelVolumeChanged(uint channelCount, nint newVolumes, uint channelIndex)
    {

    }

    public void OnGroupingParamChanged(ref Guid groupingId)
    {

    }

    public void OnStateChanged(AudioSessionState state)
    {
        if (state == AudioSessionState.AudioSessionStateExpired)
            Expired = true;
    }

    public void OnSessionDisconnected(AudioSessionDisconnectReason disconnectReason)
    {

    }

    ~InternalAudioSession()
    {
        Dispose();
    }

}
