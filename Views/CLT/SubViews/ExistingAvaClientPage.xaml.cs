// File: Views/CLT/SubViews/ExistingAvaClientPage.xaml.cs
using AvaTerminal3.ViewModels.CLT;

namespace AvaTerminal3.Views.CLT.SubViews
{
    public partial class ExistingAvaClientPage : ContentPage
    {
        // 1) Strongly-typed, non-nullable ViewModel property
        public ExistingAvaClientViewModel ViewModel { get; }

        public ExistingAvaClientPage(ExistingAvaClientViewModel vm)
        {
            InitializeComponent();

            // 2) Assign both the property and the BindingContext
            ViewModel      = vm;
            BindingContext = vm;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (ViewModel.Client is null)
            {
                // show bounce back message
                await Shell.Current.DisplayAlert(
                    "Malformed Client",
                    "Search results of client are malformed.",
                    "OK"
                );

                // go back
                await Shell.Current.GoToAsync("..");
            }
        }
    }
}
