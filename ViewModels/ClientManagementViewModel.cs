using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using AvaTerminal3.Services.Interfaces;
using AvaTerminal3.Views.CLT.SubViews;

namespace AvaTerminal3.ViewModels;

public class ClientManagementViewModel : INotifyPropertyChanged
{
    private readonly IAvaApiService _avaApiService;

    private string _clientId = string.Empty;
    private string _clientData = string.Empty;

    public event PropertyChangedEventHandler? PropertyChanged;

    public string ClientId
    {
        get => _clientId;
        set => SetField(ref _clientId, value);
    }

    public string ClientData
    {
        get => _clientData;
        set => SetField(ref _clientData, value);
    }

    public ICommand SearchCommand { get; }
    public ICommand NewClientCommand { get; }

    public ClientManagementViewModel(IAvaApiService avaApiService)
    {
        _avaApiService = avaApiService;
        SearchCommand = new Command(async () => await SearchClientAsync());
        NewClientCommand = new Command(async () => await OnNewClient());
    }

    private async Task SearchClientAsync()
    {
        if (!string.IsNullOrWhiteSpace(ClientId))
        {
            try
            {
                ClientData = await _avaApiService.GetClientAsync(ClientId);
            }
            catch (Exception ex)
            {
                ClientData = $"Error: {ex.Message}";
            }
        }
    }

    private async Task OnNewClient()
    {
        var currentApp = Application.Current;
        var window = currentApp?.Windows.FirstOrDefault();

        if (window is not null && window.Page is not null)
        {
            bool confirmed = await window.Page.DisplayAlert(
                "Create New Client",
                "Before adding a new client, please search to ensure they don't already exist.",
                "Continue",
                "Search First");

            if (confirmed)
            {
                await Shell.Current.GoToAsync(nameof(NewAvaClientPage));
                // var vm = new NewAvaClientViewModel(_avaApiService);
                // var newClientPage = new NewAvaClientPage(vm);

                // window.Page = newClientPage;
            }
        }
    }


    protected void SetField<T>(ref T field, T value, [CallerMemberName] string? propName = null)
    {
        if (!EqualityComparer<T>.Default.Equals(field, value))
        {
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}
