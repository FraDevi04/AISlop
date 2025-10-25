// src/tools/terminal/TerminalManager.cs
using System.Collections.Concurrent;

namespace AISlop;
public static class TerminalManager
{
    private static readonly ConcurrentDictionary<string, TerminalSession> _sessions = new();

    public static string CreateSession(string workingDirectory)
    {
        var sessionId = Guid.NewGuid().ToString("N");
        var session = new TerminalSession(workingDirectory);
        _sessions.TryAdd(sessionId, session);
        return sessionId;
    }

    public static TerminalSession? GetSession(string sessionId)
    {
        _sessions.TryGetValue(sessionId, out var session);
        return session;
    }

    public static bool CloseSession(string sessionId)
    {
        if (_sessions.TryRemove(sessionId, out var session))
        {
            session.Dispose();
            return true;
        }
        return false;
    }
}
