using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App.Contracts;
using App.ViewModels;

namespace App.Pages.Tabs;

public partial class SettingsTab : ContentPage
{
    public SettingsTab(SettingsViewModel viewModel)
    {
        BindingContext = viewModel;
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        if (BindingContext is IPageLifeTimeAware lifeTimeAware)
            lifeTimeAware.OnAppearing();
    }
}