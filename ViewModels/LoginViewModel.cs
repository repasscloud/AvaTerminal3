using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using AvaTerminal3.Helpers;
using AvaTerminal3.Services.Interfaces;
using AvaTerminal3.Views;
using Microsoft.Maui.ApplicationModel;

namespace AvaTerminal3.ViewModels;

public class LoginViewModel : INotifyPropertyChanged
{
    private readonly IAuthService _authService;

    public LoginViewModel()
    {
        _authService = ServiceHelper.Get<IAuthService>();
        LoginCommand = new Command(async () => await LoginAsync());
        ForgotPasswordCommand = new Command(GoToForgotPassword);
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

    private string _logSinkPath = LogSinkService.GetLogPath();
    public string LogSinkPath
    {
        get => _logSinkPath;
        set { _logSinkPath = value; OnPropertyChanged(); }
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
    public ICommand ForgotPasswordCommand { get; }

    private async Task LoginAsync()
    {
        IsBusy = true;
        ErrorMessage = string.Empty;

        // log login
        LogSinkService.Write($"[Login] User '{Username}' attempted login.");

        try
        {
            var result = await _authService.LoginAsync(Username, Password);
            if (!result)
            {
                ErrorMessage = "Login failed. Please try again.";
                return;
            }

            // success login
            LogSinkService.Write($"[Login] Login successful.");

            await Task.Delay(500);
            AppShell.LoadShell();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Login failed: {ex.Message}";
            LogSinkService.Write($"[Login] {ErrorMessage}");
        }
        finally
        {
            IsBusy = false;
        }
    }
    private async void GoToForgotPassword()
    {
        try
        {
            var window = Application.Current?.Windows.FirstOrDefault();
            if (window?.Page is not null)
            {
                await window.Page.DisplayAlert(
                    "Reset Password",
                    "We'll open your browser to reset your password.",
                    "OK");

                Uri uri = new("https://yourwebsite.com/forgot-password");
                await Browser.Default.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to open browser: {ex.Message}";
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
