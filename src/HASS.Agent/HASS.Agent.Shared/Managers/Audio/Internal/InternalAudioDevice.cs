using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HidSharp;
using NAudio.CoreAudioApi;

namespace HASS.Agent.Shared.Managers.Audio.Internal;
internal class InternalAudioDevice : IDisposable
{
    private readonly MMDeviceEnumerator _enumerator;

    //public MMDevice MMDevice { get; private set; }
    //public AudioEndpointVolume AudioEndpointVolume { get; private set; }
    //public InternalAudioSessionManager Manager { get; private set; }

    public string DeviceId { get; private set; }
    public string FriendlyName { get; private set; }
    public bool Reinitialized { get; private set; }

    public InternalAudioDevice(MMDeviceEnumerator enumerator, string deviceId)
    {
        _enumerator = enumerator;
        DeviceId = deviceId;
        //Manager = new InternalAudioSessionManager(device.AudioSessionManager);
        //AudioEndpointVolume = device.AudioEndpointVolume;

        //DeviceId = device.ID;

        using var device = GetMMDevice();
        FriendlyName = device.FriendlyName;
    }

    public void Activate()
    {
        using var configClient = new CPolicyConfigVistaClient();
        configClient.SetDefaultDevice(DeviceId);
    }

    public void Dispose()
    {
        //AudioEndpointVolume?.Dispose();
        //Manager?.Dispose();
        //MMDevice?.Dispose();

        GC.SuppressFinalize(this);
    }

    public MMDevice GetMMDevice()
    {
        return _enumerator.GetDevice(DeviceId);
    }

    ~InternalAudioDevice()
    {
        Dispose();
    }
}