// src/interfaces/ITool.cs
namespace AISlop;

/// <summary>
/// TODO: Add a "description" method for dynamic system instructions.
/// Collect every ITool description, implement into the system instruction
/// Thus no more change in instructions when a new tool is getting added
/// </summary>
public interface ITool
{
    string Name { get; }
    Task<string> ExecuteAsync(Dictionary<string, string> args, ToolExecutionContext context);
}