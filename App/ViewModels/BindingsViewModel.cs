using System.Collections.ObjectModel;
using App.Contracts;
using App.Extensions;
using App.Pages.Modals;
using App.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Binding = Client.Data.Binding;

namespace App.ViewModels;

public partial class BindingsViewModel : ObservableObject, IPageLifeTimeAware
{
    public BindingsViewModel(BindingsService bindingsService)
    {
        BindingsService = bindingsService;
    }
    
    public BindingsService BindingsService { get; }

    [ObservableProperty]
    public partial string Search { get; set; } = string.Empty;

    [ObservableProperty]
    public partial bool Editing { get; set; } = false;
    
    [ObservableProperty]
    public partial ObservableCollection<Binding> Bindings { get; set; } = [];
    
    public async void OnAppearing()
    {
        await this.Handle(async () =>
        {
            await BindingsService.LoadAsync();
            UpdateBindings(BindingsService.Bindings);
        });
    }

    partial void OnSearchChanged(string value)
    {
        var items = string.IsNullOrWhiteSpace(value) 
            ? BindingsService.Bindings.ToList()
            : BindingsService
                .Bindings
                .ToList()
                .Where(x => x.Display.Contains(value));

        UpdateBindings(items);
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
            var page = new EditBindingModalPage();
            
            page.OnConfirm += async binding =>
            {
                await BindingsService.InsertAsync(binding);
                UpdateBindings(BindingsService.Bindings);
            };

            await Shell.Current.CurrentPage.Navigation.PushModalAsync(page);
        });
    }
    
    [RelayCommand]
    public async Task Edit(Binding value)
    {
        await this.Handle(async () =>
        {
            var page = new EditBindingModalPage(value);
            page.OnConfirm += async binding =>
            {
                await BindingsService.InsertAsync(binding);
                UpdateBindings(BindingsService.Bindings);
            };

            await Shell.Current.CurrentPage.Navigation.PushModalAsync(page);
        });
    }

    [RelayCommand]
    public async Task Delete(Binding binding)
    {
        await BindingsService.DeleteAsync(binding);
        UpdateBindings(BindingsService.Bindings);
    }

    private void UpdateBindings(IEnumerable<Binding> items)
    {
        Bindings.Clear();
        
        foreach (var binding in items)
            Bindings.Add(binding);
    }
}