// src/tools/task/AskUser.cs
namespace AISlop;

public class AskUser : ITool
{
    public string Name => "askuser";
    public Task<string> ExecuteAsync(Dictionary<string, string> args, ToolExecutionContext context)
    {
        string? q = args.GetValueOrDefault("question");
        if (string.IsNullOrWhiteSpace(q))
            return Task.FromResult("Error: 'question' is required.");
        return Task.Run(() => _AskUser(q));
    }

    private string _AskUser(string message) // TODO: shouldn't be run as async, thats just fucked lol
    {
        // Your original blocking method
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"[Agent Asks]: {message}");
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.Write("Response: ");
        Console.ResetColor();
        return Console.ReadLine()!;
    }
}
