using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSCore.CoreAudioAPI;

namespace HASS.Agent.Shared.Managers.Audio.Internal;
internal class InternalAudioSessionManager : IDisposable
{
    public AudioSessionManager2 Manager { get; private set; }
    public ConcurrentDictionary<string, InternalAudioSession> Sessions { get; private set; } = new();
    public InternalAudioSessionManager(AudioSessionManager2 sessionManager2)
    {
        Manager = sessionManager2;

        using var sessionEnumerator = Manager.GetSessionEnumerator();
        foreach (var session in sessionEnumerator)
        {
            var internalSession = new InternalAudioSession(session);
            Sessions[internalSession.Control2.SessionInstanceIdentifier] = internalSession;
        }

        Manager.SessionCreated += Manager_SessionCreated;
    }

    private void Manager_SessionCreated(object? sender, SessionCreatedEventArgs e)
    {
        if (e.NewSession != null)
        {
            var internalSession = new InternalAudioSession(e.NewSession);
            Sessions[internalSession.Control2.SessionInstanceIdentifier] = internalSession;
        }
    }

    public void RemoveDisconnectedSessions()
    {
        var expiredSessionsId = Sessions.Values.Where(s => s.Expired).Select(s => s.Control2.SessionInstanceIdentifier);
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
            Manager.SessionCreated -= Manager_SessionCreated;

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
