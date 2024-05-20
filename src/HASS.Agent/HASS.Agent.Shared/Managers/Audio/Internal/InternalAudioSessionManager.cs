using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.CoreAudioApi;

namespace HASS.Agent.Shared.Managers.Audio.Internal;
internal class InternalAudioSessionManager : IDisposable
{
    public AudioSessionManager Manager { get; private set; }
    public ConcurrentDictionary<string, InternalAudioSession> Sessions { get; private set; } = new();
    public InternalAudioSessionManager(AudioSessionManager sessionManager2)
    {
        Manager = sessionManager2;

        var sessions = Manager.Sessions;
        for (var i = 0; i < sessions.Count; i++)
        {
            var session = sessions[i];
            var internalSession = new InternalAudioSession(session);
            Sessions[internalSession.Control.GetSessionInstanceIdentifier] = internalSession;
        }

        Manager.OnSessionCreated += Manager_OnSessionCreated;
    }

    private void Manager_OnSessionCreated(object sender, NAudio.CoreAudioApi.Interfaces.IAudioSessionControl newSession)
    {
        if (newSession != null)
        {
            var internalSession = new InternalAudioSession(new AudioSessionControl(newSession));
            Sessions[internalSession.Control.GetSessionInstanceIdentifier] = internalSession;
        }
    }

    public void RemoveDisconnectedSessions()
    {
        var expiredSessionsId = Sessions.Values.Where(s => s.Expired).Select(s => s.Control.GetSessionInstanceIdentifier);
        if (!expiredSessionsId.Any())
            return;

        foreach (var expiredSessionId in expiredSessionsId)
        {
            Sessions.Remove(expiredSessionId, out var expiredSession);
            expiredSession?.Dispose();
        }
    }

    public void Dispose()
    {
        if (Manager != null)
            Manager.OnSessionCreated -= Manager_OnSessionCreated;

        foreach (var session in Sessions.Values)
        {
            session?.Dispose();
        }

        Manager?.Dispose();

        GC.SuppressFinalize(this);
    }

    ~InternalAudioSessionManager()
    {
        Dispose();
    }
}
