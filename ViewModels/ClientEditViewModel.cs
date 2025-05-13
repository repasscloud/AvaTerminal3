// File: ViewModels/ClientEditViewModel.cs
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using AvaTerminal3.Models.Dto;
using AvaTerminal3.Services.Interfaces;

namespace AvaTerminal3.ViewModels;

public class ClientEditViewModel : INotifyPropertyChanged
{
    private readonly IAvaApiService _avaApiService;

    public event PropertyChangedEventHandler? PropertyChanged;

    private AvaClientDto _client = new();
    public AvaClientDto Client
    {
        get => _client;
        set => SetField(ref _client, value);
    }

    public ICommand SaveCommand { get; }

    public bool IsEditMode { get; private set; }

    private static readonly Dictionary<string, string> CurrencyToFlagCode = new()
    {
        { "AUD", "1f1e6-1f1fa" }, // 🇦🇺
        { "USD", "1f1fa-1f1f8" }, // 🇺🇸
        { "EUR", "1f1ea-1f1fa" }, // 🇪🇺
        { "GBP", "1f1ec-1f1e7" }, // 🇬🇧
        { "JPY", "1f1ef-1f1f5" }, // 🇯🇵
        { "CHF", "1f1e8-1f1ed" }, // 🇨🇭
        { "CAD", "1f1e8-1f1e6" }, // 🇨🇦
        { "CNY", "1f1e8-1f1f3" }, // 🇨🇳
        { "HKD", "1f1ed-1f1f0" }, // 🇭🇰
        { "SGD", "1f1f8-1f1ec" }, // 🇸🇬
        { "NZD", "1f1f3-1f1ff" }  // 🇳🇿
    };

    public List<string> CurrencyList { get; } = new()
    {
        "AUD 🇦🇺", "USD 🇺🇸", "EUR", "GBP", "JPY", "CHF", "CAD", "CNY", "HKD", "SGD", "NZD"
    };

    public string? SelectedCurrency
    {
        get => Client.DefaultCurrency;
        set
        {
            if (Client.DefaultCurrency != value)
            {
                Client.DefaultCurrency = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedCurrency)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Client)));
            }
        }
    }

    public ClientEditViewModel(IAvaApiService avaApiService)
    {
        _avaApiService = avaApiService;
        SaveCommand = new Command(async () => await SaveClientAsync());
    }

    public void NewClient()
    {
        Client = new AvaClientDto();
        IsEditMode = false;
    }

    public async Task LoadClientAsync(string clientId)
    {
        var result = await _avaApiService.GetClientByIdAsync(clientId);
        if (result is not null)
        {
            Client = result;
            IsEditMode = true;
        }
    }

    public async Task SaveClientAsync()
    {
        if (IsEditMode)
            await _avaApiService.UpdateClientAsync(Client.ClientId, Client);
        else
            await _avaApiService.CreateClientAsync(Client);
    }

    private void SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (!EqualityComparer<T>.Default.Equals(field, value))
        {
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
