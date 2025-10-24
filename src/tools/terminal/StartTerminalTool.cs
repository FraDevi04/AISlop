// src/tools/terminal/StartTerminal.cs
namespace AISlop;
public class StartTerminal : ITool
{
    public string Name => "start_terminal";

    public Task<string> ExecuteAsync(Dictionary<string, string> args, ToolExecutionContext context)
    {
        var sessionId = TerminalManager.CreateSession(context.CurrentWorkingDirectory);
        return Task.FromResult($"Terminal session started successfully. Use this session ID for subsequent commands: {sessionId}");
    }
}
