using System.Diagnostics;

namespace Server;

public static class Terminal
{
    public static bool Run(string command)
    {
        using var process = new Process();
        process.StartInfo = GetProcessStartInfo("/bin/bash", $"-c \"{command}\"");
        process.WaitForExit();
        return process.ExitCode == 0;
    }

    public static ValueTask<bool> RunAsync(string command)
        => RunAsync("/bin/bash", $"-c \"{command}\"");
    
    public static async ValueTask<bool> RunAsync(string file, string arguments)
    {
        using var process = new Process();
        process.StartInfo = GetProcessStartInfo(file, arguments);
        process.Start();
        await process.WaitForExitAsync();
        
        var error = await process.StandardError.ReadToEndAsync();
        
        if (!string.IsNullOrWhiteSpace(error))
            throw new Exception(error);
        
        return process.ExitCode == 0;
    }

    private static ProcessStartInfo GetProcessStartInfo(string file, string arguments)
        => new()
        {
            FileName = file,
            Arguments = arguments,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };
}