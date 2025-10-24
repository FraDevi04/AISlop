namespace AISlop;
public class CloseTerminalTool : ITool
{
    public string Name => "close_terminal";
    public Task<string> ExecuteAsync(Dictionary<string, string> args, ToolExecutionContext context)
    {
        if (!args.TryGetValue("session_id", out var sessionId) || string.IsNullOrWhiteSpace(sessionId))
        {
            return Task.FromResult("Error: 'session_id' argument is missing.");
        }

        if (TerminalManager.CloseSession(sessionId))
        {
            return Task.FromResult($"Terminal session '{sessionId}' closed successfully.");
        }
        else
        {
            return Task.FromResult($"Error: Could not find or close terminal session with ID '{sessionId}'.");
        }
    }
}
