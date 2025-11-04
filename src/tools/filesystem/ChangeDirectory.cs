// src/tools/filesystem/ChangeDirectory.cs
namespace AISlop;

public class ChangeDirectory : ITool
{
    public string Name => "changedirectory";

    public Task<string> ExecuteAsync(Dictionary<string, string> args, ToolExecutionContext context)
    {
        string? dirname = args.GetValueOrDefault("dirname");
        if (string.IsNullOrWhiteSpace(dirname))
            return Task.FromResult("Error: 'dirname' is required.");
        return _ChangeDirectory(dirname, context);
    }

    private Task<string> _ChangeDirectory(string folderName, ToolExecutionContext cwd)
    {
        if (folderName == "/")
        {
            cwd.CurrentWorkingDirectory = "environment";
            return Task.FromResult($"Successfully changed to folder \"{cwd}\"");
        }

        if (cwd.CurrentWorkingDirectory.Contains(folderName))
            return Task.FromResult($"Already in a folder named \"{folderName}\"");

        string path = Path.Combine(cwd.CurrentWorkingDirectory, folderName);
        if (!Directory.Exists(path))
            return Task.FromResult($"Directory \"{folderName}\" does not exist");

        cwd.CurrentWorkingDirectory = path;
        return Task.FromResult($"Successfully changed to folder \"{folderName}\"");
    }
}
