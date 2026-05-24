using System.ComponentModel;
using App.Contracts;
using App.Extensions;
using App.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace App.ViewModels;

public partial class SettingsViewModel : ObservableObject, IPageLifeTimeAware
{
    public SettingsViewModel(Configuration configuration)
    {
        Configuration = configuration;
        
        ClientName = Configuration.ClientName;
        MessageChannel = Configuration.MessageChannel;
        DefaultIp = Configuration.DefaultIp;
        DefaultPort = Configuration.DefaultPort;
    }
    
    private Configuration Configuration { get; set; }
    
    [ObservableProperty]
    public partial string ClientName { get; set; }
    
    [ObservableProperty]
    public partial string MessageChannel { get; set; }
    
    [ObservableProperty]
    public partial string DefaultIp { get; set; }
    
    [ObservableProperty]
    public partial int DefaultPort { get; set; }

    [RelayCommand]
    public async Task Save()
    {
        Configuration.ClientName = ClientName;
        Configuration.MessageChannel = MessageChannel;
        Configuration.DefaultIp = DefaultIp;
        Configuration.DefaultPort = DefaultPort;
        Configuration.Save();
        
        await Shell.Current.DisplayAlertAsync("Settings", "Settings Saved", "OK");
    }
}