using Client;
using Microsoft.Extensions.DependencyInjection;

namespace App;

public partial class App : Application
{
    public AppShellMobile ShellMobile { get; }
    public AppShellDesktop ShellDesktop { get; }

    public App(AppShellMobile shellMobile, AppShellDesktop shellDesktop)
    {
        ShellDesktop = shellDesktop;
        ShellMobile = shellMobile;
        InitializeComponent();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        Shell shell = PreferFlyoutOverTabBar()
            ? ShellDesktop
            : ShellMobile;
        
        return new Window(shell);
    }
    
    private static bool PreferFlyoutOverTabBar()
    {
        if (DeviceInfo.Idiom == DeviceIdiom.Desktop) 
            return true;

        var width = DeviceDisplay.MainDisplayInfo.Width
                    / DeviceDisplay.MainDisplayInfo.Density;
        
        return width >= 720;
    }
}