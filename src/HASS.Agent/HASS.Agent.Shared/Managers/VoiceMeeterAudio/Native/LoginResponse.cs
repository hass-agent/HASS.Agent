namespace HASS.Agent.Shared.Managers.VoiceMeeterAudio.Native;
public enum LoginResponse
{
    AlreadyLoggedIn = -2,
    NoClient = -1,
    Ok = 0,
    VoiceMeeterNotRunning = 1,
    LoggedOff
}