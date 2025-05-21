// File: ViewModels/ClientEditViewModel.cs
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using AvaTerminal3.Helpers;
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

    public List<string> CurrencyDisplayList { get; } = CurrencyFlagHelper.GetCurrencyDisplayList();

    public string? SelectedCurrencyDisplay
    {
        get => CurrencyDisplayList.FirstOrDefault(item =>
            item.StartsWith(Client.DefaultCurrency ?? "", StringComparison.OrdinalIgnoreCase));
        set
        {
            var currencyCode = CurrencyFlagHelper.GetCurrencyCodeFromDisplay(value ?? "");
            if (Client.DefaultCurrency != currencyCode)
            {
                Client.DefaultCurrency = currencyCode;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedCurrencyDisplay)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Client)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedCurrencyFlag)));
            }
        }
    }

    public string? SelectedCurrencyFlag =>
        CurrencyFlagHelper.GetFlagImage(Client.DefaultCurrency ?? "");


    public List<string> CountryDisplayList { get; } = CountryCodeHelper.GetCountryDisplayList();

    public string? SelectedCountryDisplay
    {
        get => CountryDisplayList.FirstOrDefault(item =>
            item.Contains($"(+{Client.ContactPersonCountryCode ?? ""})"));
        set
        {
            var dialCode = CountryCodeHelper.GetDialCodeFromDisplay(value ?? "");
            if (Client.ContactPersonCountryCode != dialCode)
            {
                Client.ContactPersonCountryCode = dialCode;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedCountryDisplay)));
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
