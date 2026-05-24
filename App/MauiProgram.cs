using App.Services;
using App.ViewModels;
using Client;
using Microsoft.Extensions.Logging;

namespace App;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        builder.Services.AddSingleton<AppShellDesktop>();
        builder.Services.AddSingleton<AppShellMobile>();
        
        builder.Services.AddSingleton<BindingsService>();
        builder.Services.AddSingleton<SignalsService>();
        builder.Services.AddSingleton(Configuration.Load());
        
        builder.Services.AddTransient<BindingsViewModel>();
        builder.Services.AddTransient<SignalsViewModel>();
        builder.Services.AddTransient<SettingsViewModel>();
        
        builder.Services.AddSingleton<PicoliClient>();
        builder.Services.AddSingleton<SignalsHandler>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}