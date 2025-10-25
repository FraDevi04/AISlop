
using System.IO.Compression;
using System.Xml.Linq;

namespace AISlop;

public class ZipDirectory : ITool
{
    public string Name => "zipdirectory";

    public Task<string> ExecuteAsync(Dictionary<string, string> args, ToolExecutionContext context)
    {
        return _ZipDirectory(
            args.GetValueOrDefault("dirname"),
            context.CurrentWorkingDirectory
        );
    }

    private Task<string> _ZipDirectory(string name, string cwd)
    {
        string folder = Path.Combine(cwd, name);
        if (!Directory.Exists(folder))
            return Task.FromResult($"Directory: \"{name}\" does not exist.");

        ZipFile.CreateFromDirectory(folder, $"{folder}.zip");
        return Task.FromResult( $"Zip file \"{name}.zip\" has been created" );
    }
}

