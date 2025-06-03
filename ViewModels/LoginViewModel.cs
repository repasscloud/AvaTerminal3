using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using AvaTerminal3.Helpers;
using AvaTerminal3.Models.Dto;
using AvaTerminal3.Services.Interfaces;

namespace AvaTerminal3.ViewModels;

public class LoginViewModel : INotifyPropertyChanged
{
    private readonly IAuthService _authService;
    private readonly IEnvironmentService _envService;
    private bool _useDev;

    public LoginViewModel()
    {
        _authService = ServiceHelper.Get<IAuthService>();
        _envService  = ServiceHelper.Get<IEnvironmentService>();
        LoginCommand = new Command(async () => await LoginAsync());
        ForgotPasswordCommand = new Command(GoToForgotPassword);
    }

    public bool UseDev
    {
        get => _useDev;
        set
        {
            if (_useDev == value) return;
            _useDev = value;
            OnPropertyChanged();
            _envService.IsDev = _useDev;
        }
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
        await LogSinkService.WriteAsync(LogLevel.Debug, $"[Login.PreAuth] User '{Username}' attempted login.");

        var loginDto = new AvaEmployeeLoginDto
        {
            Username = Username,
            Password = Password,
        };

        try
        {
            var result = await _authService.LoginAvaUserAsync(loginDto);
            if (result is null)
            {
                ErrorMessage = "Login failed. Please try again.";
                return;
            }

            var authToken = result.Token;
            await _authService.SaveTokenAsync(authToken);
            await LogSinkService.WriteAsync(LogLevel.Info, $"[Login.Auth] Token saved from login process.");

            var jwtClaims = await _authService.GetClaimsAsync();
            if (jwtClaims is not null)
            {
                var loggedInUserRole = jwtClaims.Role;
                var loggedInUserId = jwtClaims.UniqueName;

                await LogSinkService.WriteAsync(LogLevel.Info, $"[Login.Auth] Logged in user '{loggedInUserId}' with role '{loggedInUserRole}'.");
            }
            else
            {
                await LogSinkService.WriteAsync(LogLevel.Error, $"[Login.Error] User logged in, but not JWT token stored.");
            }

            // success login
            await LogSinkService.WriteAsync(LogLevel.Debug, $"[Login.Auth] Login successful.");

            await Task.Delay(500);
            AppShell.LoadShell();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Login failed: {ex.Message}";
            await LogSinkService.WriteAsync(LogLevel.Error, $"[Login.Error] {ErrorMessage}");
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