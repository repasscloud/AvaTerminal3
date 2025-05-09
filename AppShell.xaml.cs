namespace AvaTerminal3;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute("PageOne", typeof(Views.PageOne));
        Routing.RegisterRoute("PageTwo", typeof(Views.PageTwo));
    }

    private void OnToggleThemeClicked(object sender, EventArgs e)
    {
        Application.Current.UserAppTheme = Application.Current.UserAppTheme == AppTheme.Dark
            ? AppTheme.Light
            : AppTheme.Dark;
    }

    public static void LoadShell()
    {
        Application.Current.Windows[0].Page = new AppShell();
    }
}
