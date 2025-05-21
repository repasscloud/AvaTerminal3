using AvaTerminal3.Helpers;

namespace AvaTerminal3.Views;

public partial class PageOne : ContentPage
{
    public string LogPath { get; set; }

    public PageOne()
    {
        InitializeComponent();

        LogPath = LogSinkService.GetLogPath();
        BindingContext = this;
    }

    private void Button_Clicked(object sender, EventArgs e)
    {

    }
}
