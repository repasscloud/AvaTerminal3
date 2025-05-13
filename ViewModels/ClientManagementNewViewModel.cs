using System.Windows.Input;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using AvaTerminal3.Models.Dto;

namespace AvaTerminal3.ViewModels;

public class ClientManagementNewViewModel : INotifyPropertyChanged
{
    private AvaClientDto _client = new();

    public AvaClientDto Client
    {
        get => _client;
        set
        {
            _client = value;
            OnPropertyChanged();
        }
    }

    public ICommand SaveCommand => new Command(Save);

    private void Save()
    {
        // Replace this with your actual service call or API submission
        Console.WriteLine($"Saving client: {Client.ClientId}, {Client.CompanyName}");
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
