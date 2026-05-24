using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App.Extensions;
using App.Pages.Tabs;
using App.Services;
using Client;
using Domain;

namespace App.Pages;

public partial class ConnectionPage : ContentPage
{
    public PicoliClient Client { get; }
    public SignalsHandler SignalsHandler { get; }
    public Configuration Configuration { get; }

    public ConnectionPage(PicoliClient client, SignalsHandler signalsHandler, Configuration configuration)
    {
        Client = client;
        SignalsHandler = signalsHandler;
        Configuration = configuration;
        InitializeComponent();

        IpInput.Text = Configuration.DefaultIp;
        PortInput.Text = Configuration.DefaultPort.ToString();
    }

    protected override void OnAppearing()
    {
        Shell.SetFlyoutBehavior(this, FlyoutBehavior.Disabled);
    }

    protected override void OnDisappearing()
    {
        Shell.SetFlyoutBehavior(this, FlyoutBehavior.Locked);
    }

    private async void OnConnectClicked(object? sender, EventArgs e)
    {
        await this.Handle(async () =>
        {
            var ip = IpInput.Text;
            var port = int.Parse(PortInput.Text);

            if (!await Client.ConnectAsync(ip, port))
                throw new Exception("Failed to connect to server");

            await Client.SubscribeAsync(Constants.Messages.MESSAGE_TOPIC);

            Client.Name = Configuration.ClientName;
            SignalsHandler.ListenToMessages();

            await Shell.Current.GoToAsync("//" + nameof(SignalsTab));
        });
    }

    private async void OnDebugModeClicked(object? sender, EventArgs e)
    {
        await this.Handle(async () =>
        {
            Client.Name = Configuration.ClientName;
            await Shell.Current.GoToAsync("//" + nameof(SignalsTab));
        });
    }
}