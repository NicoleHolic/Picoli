using System.Diagnostics;

namespace Server;

public static class Command
{
    public static bool Run(string command)
    {
        var info = GetProcessStartInfo(command);
        var process = Process.Start(info);

        if (process is null)
            return false;

        process.WaitForExit();
        return process.ExitCode == 0;
    }

    public static async ValueTask<bool> RunAsync(string command)
    {
        var info = GetProcessStartInfo(command);
        var process = Process.Start(info);
        
        if (process is null)
            return false;

        await process.WaitForExitAsync();
        return process.ExitCode == 0;
    }

private static ProcessStartInfo GetProcessStartInfo(string command)
        => new ProcessStartInfo()
        {
            FileName = "/bin/bash",
            Arguments = $"-c \"{command}\"",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
        };
}