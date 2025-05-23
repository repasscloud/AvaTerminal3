using AvaTerminal3.Helpers;
using CommunityToolkit.Maui.Views;

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
        // Your original placeholder
    }

   private async void ShowLoadingPopup_Clicked(object sender, EventArgs e)
{
    var popup = new LoadingPopup();

    this.ShowPopup(popup); // Do NOT await this — it blocks until popup is closed

    await Task.Delay(2000);

    popup.Close(); // This now closes properly without user input
}

    private void Button_Clicked1(object sender, EventArgs e)
    {
        LogSinkService.DeleteLogFile();
    }

}
