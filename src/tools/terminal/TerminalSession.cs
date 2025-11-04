using System.Diagnostics;
using System.Text;

namespace AISlop;

public class TerminalSession : IDisposable
{
    private readonly Process _process;
    private readonly StreamWriter _inputWriter;
    private readonly StringBuilder _outputBuffer = new StringBuilder();
    private readonly StringBuilder _errorBuffer = new StringBuilder();
    private TaskCompletionSource<string>? _currentCommandCompletionSource;
    private readonly string _endOfCommandMarker = Guid.NewGuid().ToString();
    public TerminalSession(string workingDirectory)
    {
        _currentCommandCompletionSource = null;

        string shell;
        if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows))
            shell = "cmd.exe";
        else
            shell = "/bin/bash";

        // Use fallback working directory if doesn't exist:
        if (!Directory.Exists(workingDirectory))
            workingDirectory = "/home/fradevi";

        var processInfo = new ProcessStartInfo(shell)
        {
            WorkingDirectory = workingDirectory,
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            StandardOutputEncoding = Encoding.UTF8,
            StandardErrorEncoding = Encoding.UTF8
        };

        _process = new Process { StartInfo = processInfo, EnableRaisingEvents = true };

        _process.OutputDataReceived += OnOutputDataReceived;
        _process.ErrorDataReceived += OnErrorDataReceived;

        _process.Start();

        _process.BeginOutputReadLine();
        _process.BeginErrorReadLine();

        _inputWriter = _process.StandardInput;
        _inputWriter.AutoFlush = true;
    }

    private void OnOutputDataReceived(object sender, DataReceivedEventArgs e)
    {
        if (e.Data == null) return;

        if (e.Data.Contains(_endOfCommandMarker))
        {
            var finalOutput = _outputBuffer.ToString();
            var finalError = _errorBuffer.ToString();

            _outputBuffer.Clear();
            _errorBuffer.Clear();

            // Signal completion for the active command, if any
            _currentCommandCompletionSource?.TrySetResult(finalOutput + finalError);
        }
        else
        {
            _outputBuffer.AppendLine(e.Data);
        }
    }

    private void OnErrorDataReceived(object sender, DataReceivedEventArgs e)
    {
        if (e.Data != null)
        {
            _errorBuffer.AppendLine(e.Data);
        }
    }

    public async Task<string> RunCommandAsync(string command)
    {
        // Initialize a new completion source for this command
        var tcs = new TaskCompletionSource<string>();
        _currentCommandCompletionSource = tcs;

        await _inputWriter.WriteLineAsync(command);
        await _inputWriter.WriteLineAsync($"echo {_endOfCommandMarker}");

        return await tcs.Task;
    }

    public void Dispose()
    {
        _process.OutputDataReceived -= OnOutputDataReceived;
        _process.ErrorDataReceived -= OnErrorDataReceived;

        _inputWriter.WriteLine("exit");

        if (!_process.WaitForExit(10000)) // Wait up to 10 seconds
        {
            _process.Kill();
        }

        _inputWriter.Dispose();
        _process.Dispose();
    }
}
