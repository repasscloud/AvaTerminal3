using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AvaTerminal3.Services.Interfaces;
using AvaTerminal3.Views.CLT.SubViews;
using AvaTerminal3.Models.Dto;
using AvaTerminal3.Helpers;

namespace AvaTerminal3.ViewModels
{
    public partial class ClientManagementViewModel : ObservableObject
    {
        private readonly IAvaApiService _avaApiService;
        private readonly ISharedStateService _sharedStateService;

        public ClientManagementViewModel(IAvaApiService avaApiService, ISharedStateService sharedStateService)
        {
            _avaApiService = avaApiService;
            _sharedStateService = sharedStateService;
        }

        [ObservableProperty]
        private string errorMessage = string.Empty;

        [ObservableProperty]
        private string clientId = string.Empty;

        [ObservableProperty]
        private AvaClientDto clientData = new();

        [RelayCommand]
        private async Task SearchClientAsync()
        {
            if (string.IsNullOrWhiteSpace(ClientId))
                return;

            try
            {
                var data = await _avaApiService.GetAvaClientBySearchEverythingAsync(ClientId);

                if (data is not null)
                {
                    // debug - save the data to json
                    await LogSinkService.DumpJsonAsync(LogLevel.Debug, data);

                    ClientData = data;
                    _sharedStateService.SaveAvaClientDto(ClientData);
                    
                    // Navigate to existing client page when a result is found
                    await Shell.Current.GoToAsync(nameof(ExistingAvaClientPage));
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error: {ex.Message}";
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
