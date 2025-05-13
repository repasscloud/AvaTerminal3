using Microsoft.Maui.Controls;

namespace AvaTerminal3;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute("PageOne", typeof(Views.PageOne));
        Routing.RegisterRoute("PageTwo", typeof(Views.PageTwo));
        Routing.RegisterRoute("ClientManagementPage", typeof(Views.CLT.ClientManagementPage));
    }

    private void OnToggleThemeClicked(object sender, EventArgs e)
    {
        var app = Application.Current;
        if (app != null)
        {
            app.UserAppTheme = app.UserAppTheme == AppTheme.Dark
                ? AppTheme.Light
                : AppTheme.Dark;
        }
    }

    public static void LoadShell()
    {
        var window = Application.Current?.Windows.FirstOrDefault();
        if (window is not null)
        {
            window.Page = new AppShell();
        }
    }
}
