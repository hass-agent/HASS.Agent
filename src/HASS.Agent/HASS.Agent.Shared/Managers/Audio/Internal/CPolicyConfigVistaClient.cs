using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using CSCore.CoreAudioAPI;

namespace HASS.Agent.Shared.Managers.Audio.Internal;

// https://github.com/morphx666/CoreAudio
// https://github.com/File-New-Project/EarTrumpet

internal class CPolicyConfigVistaClient : IDisposable
{
    private IPolicyConfigVista policyConfigVistaClient;

    public CPolicyConfigVistaClient()
    {
        policyConfigVistaClient = (IPolicyConfigVista)new _CPolicyConfigVistaClient();
    }

    public void SetDefaultDevice(string deviceID)
    {

        policyConfigVistaClient.SetDefaultEndpoint(deviceID, Role.Console);
        policyConfigVistaClient.SetDefaultEndpoint(deviceID, Role.Multimedia);
        policyConfigVistaClient.SetDefaultEndpoint(deviceID, Role.Communications);
    }

    public void Dispose()
    {
        if (policyConfigVistaClient != null && Marshal.IsComObject(policyConfigVistaClient))
            Marshal.FinalReleaseComObject(policyConfigVistaClient);

        GC.SuppressFinalize(this);
    }

    ~CPolicyConfigVistaClient()
    {
        Dispose();
    }
}