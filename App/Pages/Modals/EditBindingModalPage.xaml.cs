using Client.Data;
using Binding = Client.Data.Binding;

namespace App.Pages.Modals;

public partial class EditBindingModalPage : ContentPage
{
    public EditBindingModalPage(Binding? binding = null)
    {
        InitializeComponent();

        var types = Enum.GetValues(typeof(SignalType)).Cast<SignalType>().ToList();
        
        BindingId = binding?.Id ?? Guid.CreateVersion7();
        DisplayEntry.Text = binding?.Display ?? string.Empty;
        NameEntry.Text = binding?.Name ?? string.Empty;
        TypePicker.ItemsSource = types;
        TypePicker.SelectedIndex = binding is not null ? types.IndexOf(binding.Type) : 0;
        //TypePicker.SelectedItem = binding?.Type.ToString() ?? nameof(SignalType.Alert);
        ParametersEntry.Text = binding?.Parameters ?? string.Empty;
    }
    
    public Guid BindingId { get; set; }
    
    public event Func<Binding, ValueTask>? OnConfirm;

    private async void Button_ConfirmClicked(object? sender, EventArgs e)
    {
        var binding = new Binding()
        {
            Id = BindingId,
            Display = DisplayEntry.Text,
            Name = NameEntry.Text,
            Type = (SignalType) TypePicker.SelectedItem,
            Parameters = ParametersEntry.Text
        };
        
        if (OnConfirm is not null) 
            await OnConfirm.Invoke(binding);
        
        await Navigation.PopModalAsync();
    }

    private async void Button_CancelClicked(object? sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }
}