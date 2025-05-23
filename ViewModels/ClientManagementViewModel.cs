using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using AvaTerminal3.Services.Interfaces;
using AvaTerminal3.Views.CLT.SubViews;

namespace AvaTerminal3.ViewModels
{
    public partial class ClientManagementViewModel : ObservableObject
    {
        private readonly IAvaApiService _avaApiService;

        public ClientManagementViewModel(IAvaApiService avaApiService)
        {
            _avaApiService = avaApiService;
        }

        [ObservableProperty]
        private string clientId = string.Empty;

        [ObservableProperty]
        private string clientData = string.Empty;

        [RelayCommand]
        private async Task SearchClientAsync()
        {
            if (string.IsNullOrWhiteSpace(ClientId))
                return;

            try
            {
                var data = await _avaApiService.GetClientAsync(ClientId);
                ClientData = data;

                if (!string.IsNullOrWhiteSpace(ClientData))
                {
                    // Navigate to existing client page when a result is found
                    await Shell.Current.GoToAsync(nameof(ExistingAvaClientPage));
                }
            }
            catch (Exception ex)
            {
                ClientData = $"Error: {ex.Message}";
            }
        }

        [RelayCommand]
        private async Task NewClientAsync()
        {
            bool confirmed = await Shell.Current.DisplayAlert(
                "Create New Client",
                "Search before creating a new client.",
                "Continue",
                "Search First");

            if (confirmed)
            {
                await Shell.Current.GoToAsync(nameof(NewAvaClientPage));
            }
        }
    }
}
