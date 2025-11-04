// src/tools/task/TaskDone.cs
using AISlop;

namespace AISlop;

public class TaskDone : ITool
{
    public string Name => "taskdone";
    public Task<string> ExecuteAsync(Dictionary<string, string> args, ToolExecutionContext context)
    {
        string? m = args.GetValueOrDefault("message");
        if (string.IsNullOrWhiteSpace(m))
            return Task.FromResult("Error: 'message' is required.");
        return Task.FromResult(_TaskDone(m));
    }

    private string _TaskDone(string message)
    {
        Logging.DisplayAgentThought(message, ConsoleColor.Yellow);
        return "Task completion message displayed.";
    }
}
