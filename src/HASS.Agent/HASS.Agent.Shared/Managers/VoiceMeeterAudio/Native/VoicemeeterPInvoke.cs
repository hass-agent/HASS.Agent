using Serilog;
using System;
using System.Runtime.InteropServices;

namespace HASS.Agent.Shared.Managers.VoiceMeeterAudio.Native;

internal partial class VoicemeeterPInvoke : IVoicemeeterPInvoke
{
    private const string RemoteLibraryPath = @"C:\Program Files (x86)\VB\Voicemeeter\VoicemeeterRemote.dll";
    private bool disposedValue;
    private LoginResponse currentLoginStatus = LoginResponse.LoggedOff;

    public bool IsLoggedIn => currentLoginStatus >= 0;

    public VoicemeeterPInvoke()
    {
        Login();
    }

    public void Login()
    {
        currentLoginStatus = VBVMR_Login();
        if (!IsLoggedIn)
        {
            Log.Error($"[VMAUDIOMGR] Voicemeeter did not login correctly, statuscode: {currentLoginStatus}");
        }
    }

    public bool SetParameters(string paramScript)
    {
        if (IsLoggedIn)
        {
            var result = VBVMR_SetParameters(paramScript);
            if (result == 0)
            {
                return true;
            }
            if (result < 0)
            {
                Log.Error("[VMAUDIOMGR] Unexpected error running parameter script");
            }
            if (result > 0)
            {
                Log.Error($"[VMAUDIOMGR] Error in script detected at line nr: {result}");
            }
        }
        return false;
    }

    public int IsParametersDirtyRaw()
    {
        return VBVMR_IsParametersDirty();
    }

    public bool IsParametersDirty()
    {
        return IsLoggedIn && VBVMR_IsParametersDirty() != 0;
    }

    public float GetParameter(string param)
    {
        if (IsLoggedIn)
        {
            var result = VBVMR_GetParameter(param, out float val);
            if (result == 0)
            {
                return val;
            }
            if (result < 0)
            {
                /*  -1: error
                    -2: no server.
                    -3: unknown parameter
                    -5: structure mismatch */
                Log.Error("[VMAUDIOMGR] Unexpected error getting parameter {param}, code: {result}", param, result);
            }
        }
        return float.NaN;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            VBVMR_Logout();
            disposedValue = true;
        }
    }

    ~VoicemeeterPInvoke()
    {
        Dispose(disposing: false);
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    [LibraryImport(RemoteLibraryPath, EntryPoint = "VBVMR_Login")]
    private static partial LoginResponse VBVMR_Login();

    [LibraryImport(RemoteLibraryPath, EntryPoint = "VBVMR_Logout")]
    private static partial int VBVMR_Logout();

    [LibraryImport(RemoteLibraryPath, EntryPoint = "VBVMR_SetParametersW", StringMarshalling = StringMarshalling.Utf16)]
    private static partial int VBVMR_SetParameters(string paramScript);

    [LibraryImport(RemoteLibraryPath, EntryPoint = "VBVMR_IsParametersDirty")]
    private static partial int VBVMR_IsParametersDirty();

    [LibraryImport(RemoteLibraryPath, EntryPoint = "VBVMR_GetParameterFloat", StringMarshalling = StringMarshalling.Utf16)]
    private static partial int VBVMR_GetParameter([MarshalAs(UnmanagedType.LPStr)] string paramName, out float value);
}
