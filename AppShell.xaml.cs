namespace AvaTerminal3;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
    }

    private void OnToggleThemeClicked(object sender, EventArgs e)
    {
        Application.Current.UserAppTheme = Application.Current.UserAppTheme == AppTheme.Dark
            ? AppTheme.Light
            : AppTheme.Dark;
    }
}
