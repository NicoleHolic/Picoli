using System.Diagnostics;
using Client;
using Client.Data;
using Domain.Messages;

namespace App.Services;

public sealed class SignalsHandler
{
    public SignalsHandler(PicoliClient client, BindingsService bindingsService)
    {
        Client = client;
        BindingsService = bindingsService;
    }

    public PicoliClient Client { get; }
    public BindingsService BindingsService { get; }

    public void ListenToMessages()
    {
        Client.MessageReceived += OnMessageReceived;
    }

    private async ValueTask OnMessageReceived(Message message)
    {
        var bindings = BindingsService
            .Bindings
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
        await MainThread.InvokeOnMainThreadAsync(async () =>
            await Shell.Current.DisplayAlertAsync("Alert", parameters, "Ok")
        );
    }
    
    private async ValueTask HandleApp(string parameters)
    {
#if WINDOWS
        Process.Start(new ProcessStartInfo
        {
            FileName = parameters,
            UseShellExecute = true
        });
#elif MACCATALYST || MACOS
        Process.Start("open", $"\"{parameters}\"");
#else
        await Launcher.Default.OpenAsync(new OpenFileRequest
        {
            File = new ReadOnlyFile(parameters)
        });
#endif
    } 
    
    private async ValueTask HandleCommand(string parameters)
    {
#if ANDROID || IOS
        await MainThread.InvokeOnMainThreadAsync(() =>
            Shell.Current.DisplayAlert("Error", "Not supported!", "Ok"));
        return;
#elif WINDOWS
        var fileName  = "cmd.exe";
        var arguments = $"/c {parameters}";
#elif MACCATALYST || MACOS
        var fileName  = "/bin/bash";
        var arguments = $"-c \"{parameters}\"";
#else
        throw new PlatformNotSupportedException();
#endif

        using var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        process.Start();
        await process.WaitForExitAsync();
    }
}