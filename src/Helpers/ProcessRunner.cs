namespace Loupedeck.DockerPlugin.Helpers;

using System.Diagnostics;

public interface IProcessRunner
{
    Int32 RunAndGetExitCode(String fileName, String arguments);
}

public sealed class ProcessRunner : IProcessRunner
{
    public Int32 RunAndGetExitCode(String fileName, String arguments)
    {
        using var process = new Process { StartInfo = CreateHiddenStartInfo(fileName, arguments) };
        process.Start();
        // Drain redirected streams so a chatty process cannot block on a full pipe buffer.
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
        process.WaitForExit();
        return process.ExitCode;
    }

    private static ProcessStartInfo CreateHiddenStartInfo(String fileName, String arguments) => new()
    {
        FileName = fileName,
        Arguments = arguments,
        RedirectStandardOutput = true,
        RedirectStandardError = true,
        UseShellExecute = false,
        CreateNoWindow = true,
    };
}
