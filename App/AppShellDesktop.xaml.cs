using App.Extensions;
using App.Pages;
using Client;

namespace App;

public partial class AppShellDesktop : Shell
{
    public PicoliClient Client { get; }

    public AppShellDesktop(PicoliClient client)
    {
        Client = client;
        InitializeComponent();
    }

    private async void OnLogoutClicked(object? sender, EventArgs e)
    {
        if (await Client.DisconnectAsync())
            await Shell.Current.GoToAsync($"//MainPage");
    }
}