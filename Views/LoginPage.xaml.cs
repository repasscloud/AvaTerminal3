using AvaTerminal3.ViewModels;

namespace AvaTerminal3.Views;

public partial class LoginPage : ContentPage
{
    public LoginPage()
    {
        InitializeComponent();
        BindingContext = new LoginViewModel(); // <- this was missing
    }
}
