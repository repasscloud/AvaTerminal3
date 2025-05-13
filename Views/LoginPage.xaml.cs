using AvaTerminal3.ViewModels;

namespace AvaTerminal3.Views;

public partial class LoginPage : ContentPage
{
    public LoginPage()
    {
        InitializeComponent();
        BindingContext = new LoginViewModel();
    }

    private void PasswordEntry_Completed(object sender, EventArgs e)
    {
        if (BindingContext is LoginViewModel vm && vm.LoginCommand.CanExecute(null))
        {
            vm.LoginCommand.Execute(null);
        }
    }
}
