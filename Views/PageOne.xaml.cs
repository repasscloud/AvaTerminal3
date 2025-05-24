using AvaTerminal3.Helpers;
using AvaTerminal3.Models.Static;
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

    protected async override void OnAppearing()
    {
        base.OnAppearing();
        await CheckVersionAsync();
    }

    private async Task CheckVersionAsync()
    {
        try
        {
            // 1) fetch the “version” string from your API
            var apiValue = await AttributeApi.GetCurrentSupportedClientVersionAsync();

            // 2) read your local version
            var localVersion = AppVersion.VersionInfo;

            // 3) compare and give feedback
            if (apiValue == localVersion)
            {
                await DisplayAlert(
                    "Version Check",
                    $"✅ Up to date ({localVersion})",
                    "OK");
            }
            else
            {
                await DisplayAlert(
                    "Version Mismatch",
                    $"⚠️ API: {apiValue}\nLocal: {localVersion}",
                    "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert(
                "Error",
                $"Failed to check version:\n{ex.Message}",
                "OK");
        }
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
