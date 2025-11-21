using System;

namespace HASS.Agent.Shared.Managers.VoiceMeeterAudio.Native;

internal interface IVoicemeeterPInvoke : IDisposable
{
    bool IsLoggedIn { get; }
    void Login();
    bool SetParameters(string paramScript);
    int IsParametersDirtyRaw();
    bool IsParametersDirty();
    float GetParameter(string param);
}
