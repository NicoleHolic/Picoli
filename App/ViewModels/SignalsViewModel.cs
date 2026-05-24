using System.Collections.ObjectModel;
using App.Contracts;
using App.Extensions;
using App.Pages.Modals;
using App.Services;
using Client;
using Client.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Domain;
using Domain.Messages;

namespace App.ViewModels;

public partial class SignalsViewModel : ObservableObject, IPageLifeTimeAware
{
    public SignalsViewModel(SignalsService signalsService, PicoliClient client)
    {
        SignalsService = signalsService;
        Client = client;
    }
    
    public SignalsService SignalsService { get; }
    public PicoliClient Client { get; }

    [ObservableProperty]
    public partial string Search { get; set; } = string.Empty;

    [ObservableProperty]
    public partial bool Editing { get; set; } = false;
    
    [ObservableProperty]
    public partial ObservableCollection<Signal> Signals { get; set; } = [];
    
    public async void OnAppearing()
    {
        await this.Handle(async () =>
        {
            await SignalsService.LoadAsync();
            UpdateSignals(SignalsService.Signals);
        });
    }

    partial void OnSearchChanged(string value)
    {
        var items = string.IsNullOrWhiteSpace(value) 
            ? SignalsService.Signals.ToList()
            : SignalsService
                .Signals
                .ToList()
                .Where(x => x.Display.Contains(value));

        UpdateSignals(items);
    }

    [RelayCommand]
    public void ToggleEditMode()
    {
        Editing = !Editing;
    }
    
    [RelayCommand]
    public async Task Create()
    {
        await this.Handle(async () =>
        {
            var page = new EditSignalModalPage();
            
            page.OnConfirm += async signal =>
            {
                await SignalsService.InsertAsync(signal);
                UpdateSignals(SignalsService.Signals);
            };

            await Shell.Current.CurrentPage.Navigation.PushModalAsync(page);
        });
    }
    
    [RelayCommand]
    public async Task Edit(Signal value)
    {
        await this.Handle(async () =>
        {
            var page = new EditSignalModalPage(value);
            page.OnConfirm += async signal =>
            {
                await SignalsService.InsertAsync(signal);
                UpdateSignals(SignalsService.Signals);
            };

            await Shell.Current.CurrentPage.Navigation.PushModalAsync(page);
        });
    }

    [RelayCommand]
    public async Task Invoke(Signal value)
    {
        await this.Handle(async () =>
        {
            var message = new Message
            {
                Id = Guid.CreateVersion7(),
                Topic = Constants.Messages.MESSAGE_TOPIC,
                Parameters = value.Name,
                Receiver = value.Receiver,
                Sender = Client.Name,
                PublishedAt = DateTime.Now,
            };
            
            await Client.PublishAsync(message);
        });
    }

    [RelayCommand]
    public async Task Delete(Signal signal)
    {
        await SignalsService.DeleteAsync(signal);
        UpdateSignals(SignalsService.Signals);
    }

    private void UpdateSignals(IEnumerable<Signal> items)
    {
        Signals.Clear();
        
        foreach (var signal in items)
            Signals.Add(signal);
    }
}