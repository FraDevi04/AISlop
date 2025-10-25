// src/tools/filesystem/WriteFile.cs
using System.Text;
using System.Text.RegularExpressions;

namespace AISlop;

public class WriteFile : ITool
{
    public string Name => "writefile";
    public Task<string> ExecuteAsync(Dictionary<string, string> args, ToolExecutionContext context)
    {
        return _OverwriteFile(
            args.GetValueOrDefault("filename"),
            args.GetValueOrDefault("content"),
            args.GetValueOrDefault("append"),
            context.CurrentWorkingDirectory
            );
    }

    private Task<string> _CreateFile(string filename, string content, bool shouldAppend, string cwd)
    {
        string filePath = Path.Combine(cwd, filename);

        string? dir = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(dir))
            Directory.CreateDirectory(dir);

        if (File.Exists(filePath) && !shouldAppend)
            return Task.FromResult($"A file with that name already exists in the workspace: {filename}");

        using StreamWriter sw = new(filePath, shouldAppend);

        content = Regex.Unescape(content);
        if (filename.Contains(".html"))
            content = XMLParser.UnescapeHtml(content);

        sw.Write(content);

        return Task.FromResult($"File has been written: \"{filename}\" and content written into it");
    }

    public Task<string> _OverwriteFile(string filename, string text, string? append, string cwd)
    {
        string filePath = Path.Combine(cwd, filename);

        bool shouldAppend = append is null ? false : bool.Parse(append);

        if (File.Exists(filePath) && !shouldAppend)
            File.Delete(filePath);

        return _CreateFile(filename, text, shouldAppend, cwd);
    }
}
