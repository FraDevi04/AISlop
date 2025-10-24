using System.Diagnostics;
using System.Text;

namespace AISlop;

public class TerminalSession : IDisposable
{
    private readonly Process _process;
    private readonly StreamWriter _inputWriter;
    private readonly StringBuilder _outputBuffer = new StringBuilder();
    private readonly StringBuilder _errorBuffer = new StringBuilder();
    private readonly TaskCompletionSource<string> _commandCompletionSource;
    private readonly string _endOfCommandMarker = Guid.NewGuid().ToString();
    public TerminalSession(string workingDirectory)
    {
        _commandCompletionSource = new TaskCompletionSource<string>();

        var processInfo = new ProcessStartInfo("cmd.exe")
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

            _commandCompletionSource.TrySetResult(finalOutput + finalError);
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
        await _inputWriter.WriteLineAsync(command);
        await _inputWriter.WriteLineAsync($"echo {_endOfCommandMarker}");

        return await _commandCompletionSource.Task;
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
