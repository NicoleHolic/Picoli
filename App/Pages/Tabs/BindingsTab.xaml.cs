using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App.Contracts;
using App.Services;
using App.ViewModels;
using Binding = Client.Data.Binding;

namespace App.Pages.Tabs;

public partial class BindingsTab : ContentPage
{
    public BindingsTab(BindingsViewModel viewModel)
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