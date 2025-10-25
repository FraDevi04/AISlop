namespace AISlop;

public class RunInTerminalTool : ITool
{
    public string Name => "run_in_terminal";
    public async Task<string> ExecuteAsync(Dictionary<string, string> args, ToolExecutionContext context)
    {
        if (!args.TryGetValue("session_id", out var sessionId) || string.IsNullOrWhiteSpace(sessionId))
        {
            return "Error: 'session_id' argument is missing.";
        }
        if (!args.TryGetValue("command", out var command) || string.IsNullOrWhiteSpace(command))
        {
            return "Error: 'command' argument is missing.";
        }

        var session = TerminalManager.GetSession(sessionId);
        if (session == null)
        {
            return $"Error: No active terminal session found with ID '{sessionId}'.";
        }

        string result = await session.RunCommandAsync(command);

        if (string.IsNullOrWhiteSpace(result.Trim()))
        {
            return "Command executed, no output produced.";
        }

        return result;
    }
}
