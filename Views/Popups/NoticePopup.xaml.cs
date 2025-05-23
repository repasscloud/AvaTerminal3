using CommunityToolkit.Maui.Views;

namespace AvaTerminal3.Views.Popups;

public partial class NoticePopup : Popup
{
    public NoticePopup(string title, string message)
    {
        InitializeComponent();
        TitleLabel.Text = title;
        MessageLabel.Text = message;
    }

    private void OnOkClicked(object sender, EventArgs e)
    {
        Close(); // no return value
    }
}
