using Client.Data;

namespace App.Pages.Modals;

public partial class EditSignalModalPage : ContentPage
{
    public EditSignalModalPage(Signal? signal = null)
    {
        InitializeComponent();
        
        SignalId = signal?.Id ?? Guid.CreateVersion7();
        DisplayEntry.Text = signal?.Display ?? string.Empty;
        NameEntry.Text = signal?.Name ?? string.Empty;
        ReceiverEntry.Text = signal?.Receiver ?? string.Empty;
    }
    
    public Guid SignalId { get; set; }
    
    public event Func<Signal, ValueTask>? OnConfirm;

    private async void Button_ConfirmClicked(object? sender, EventArgs e)
    {
        var signal = new Signal()
        {
            Id = SignalId,
            Display = DisplayEntry.Text,
            Name = NameEntry.Text,
            Receiver = ReceiverEntry.Text,
        };
        
        if (OnConfirm is not null) 
            await OnConfirm.Invoke(signal);
        
        await Navigation.PopModalAsync();
    }

    private async void Button_CancelClicked(object? sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }
}