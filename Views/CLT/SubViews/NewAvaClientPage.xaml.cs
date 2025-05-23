// File: Views/CLT/SubViews/NewAvaClientPage.xaml.cs
using Microsoft.Maui.Controls;
using AvaTerminal3.ViewModels.CLT;

namespace AvaTerminal3.Views.CLT.SubViews
{
    public partial class NewAvaClientPage : ContentPage
    {
        // 1) Strongly-typed, non-nullable ViewModel property
        public NewAvaClientViewModel ViewModel { get; }

        public NewAvaClientPage(NewAvaClientViewModel vm)
        {
            InitializeComponent();

            // 2) Assign both the property and the BindingContext
            ViewModel      = vm;
            BindingContext = vm;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            // You can now safely refer to ViewModel without null checks.
            // e.g. await ViewModel.LoadClientAsync(id);
        }
    }
}
