using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using AvaTerminal3.Helpers;
using AvaTerminal3.Services.Interfaces;

namespace AvaTerminal3.ViewModels;

public class LoginViewModel : INotifyPropertyChanged
{
    private readonly IAuthService _authService;

    public LoginViewModel()
    {
        _authService = ServiceHelper.Get<IAuthService>();
        LoginCommand = new Command(async () => await LoginAsync());
    }

    private string _username = string.Empty;
    public string Username
    {
        get => _username;
        set { _username = value; OnPropertyChanged(); }
    }

    private string _password = string.Empty;
    public string Password
    {
        get => _password;
        set { _password = value; OnPropertyChanged(); }
    }

    private bool _isBusy;
    public bool IsBusy
    {
        get => _isBusy;
        set { _isBusy = value; OnPropertyChanged(); }
    }

    private string _errorMessage = string.Empty;
    public string ErrorMessage
    {
        get => _errorMessage;
        set
        {
            _errorMessage = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(HasError));
        }
    }

    public bool HasError => !string.IsNullOrEmpty(ErrorMessage);

    public ICommand LoginCommand { get; }

    private async Task LoginAsync()
    {
        IsBusy = true;
        ErrorMessage = string.Empty;

        try
        {
            var result = await _authService.LoginAsync(Username, Password);
            if (!result)
            {
                ErrorMessage = "Login failed. Please try again.";
                return;
            }

            await Task.Delay(500);
            await Shell.Current.GoToAsync("//MainPage");
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Login failed: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    public ICommand ForgotPasswordCommand => new Command(() =>
    {
        // TODO: Navigation or show toast
    });

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
