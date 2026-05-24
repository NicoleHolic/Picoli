using System.Diagnostics;
using Client;
using Client.Data;
using Domain.Messages;

namespace Cli;

internal sealed class MessageHandler
{
    public MessageHandler(Bindings bindings)
    {
        Bindings = bindings;
    }
    
    public Bindings Bindings { get; }
    
    public async ValueTask HandleMessage(Message message)
    {
        var bindings = Bindings
            .Where(x => x.Name.Equals(message.Parameters));

        foreach (var binding in bindings)
        {
            switch (binding.Type)
            {
                case SignalType.Alert:
                    await HandleAlert(binding.Parameters);
                    break;
                case SignalType.App:
                    await HandleApp(binding.Parameters);
                    break;
                case SignalType.Command:
                    await HandleCommand(binding.Parameters);
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    private async ValueTask HandleAlert(string parameters)
    {
        Console.WriteLine($"ALERT: {parameters}");
    }
    
    private async ValueTask HandleApp(string parameters)
    {
        Process.Start(new ProcessStartInfo
        {
            FileName = parameters,
            UseShellExecute = true
        });
    } 
    
    private async ValueTask HandleCommand(string parameters)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = "cmd.exe",
            Arguments = $"/c {parameters}",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        
        using var process = new Process();
        process.StartInfo = startInfo;
        process.Start();
        await process.WaitForExitAsync();
    }
}