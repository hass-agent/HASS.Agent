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
using HASS.Agent.Shared.Managers.VoiceMeeterAudio.Native;
using WinRT;

namespace HASS.Agent.Shared.Managers.VoiceMeeterAudio;
public static class VoiceMeeterAudioManager
{
    private static bool _initialized = false;
    static IVoicemeeterPInvoke nativeVoicemeeter = null;
    static bool parametersDirty = false;

    private static void Initialize()
    {
        try
        {
            if (_initialized)
                return;
            Log.Debug("[VMAUDIOMGR] initializing");
            if (Environment.Is64BitOperatingSystem)
            {
                nativeVoicemeeter = new Voicemeeter64PInvoke();
            }
            else
            {
                nativeVoicemeeter = new VoicemeeterPInvoke();
            }
            Log.Information("[VMAUDIOMGR] initialized");
            _initialized = true;
        }
        catch (Exception ex)
        {
            Log.Error("[VMAUDIOMGR] Exception during initialize: {error}", ex.ToString());
        }
    }

    private static bool CheckConnection()
    {
        Initialize();
        var result = nativeVoicemeeter.IsParametersDirtyRaw();
        if (result < 0) { return false; }
        if (result > 0) { parametersDirty = true; }
        return true;
    }

    private static bool Connect()
    {
        if (!CheckConnection())
        {
            Log.Information("[VMAUDIOMGR] Voicemeeter connection lost, reconnecting...");
            nativeVoicemeeter.Login();
            return nativeVoicemeeter.IsLoggedIn;
        }
        return true;
    }

    public static void Shutdown()
    {
        Log.Debug("[VMAUDIOMGR] shutting down");
        try
        {
            nativeVoicemeeter?.Dispose();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "[VMAUDIOMGR] shutdown fatal error: {ex}", ex.Message);
        }
        Log.Debug("[VMAUDIOMGR] shutdown completed");
    }

    public static void SetParameters(string script)
    {
        if (!Connect())
        {
            return;
        }
        Log.Debug("[VMAUDIOMGR] Executing script: {script}", script);
        nativeVoicemeeter.SetParameters(script);
    }

    public static float GetParameter(string param)
    {
        if (!Connect())
        {
            return float.NaN;
        }
        Log.Debug("[VMAUDIOMGR] Getting param: {param}", param);
        while (nativeVoicemeeter.IsParametersDirty())
        {
            //fix temp issue with api.
        }
        return nativeVoicemeeter.GetParameter(param);
    }
}
