// Services/PopupService.cs
using CommunityToolkit.Maui.Views;
using AvaTerminal3.Services.Interfaces;
using AvaTerminal3.Views.Popups;

namespace AvaTerminal3.Services;

public class PopupService : IPopupService
{
    public Task ShowNoticeAsync(string title, string message)
    {
        var popup = new NoticePopup(title, message);
        Shell.Current.ShowPopup(popup);
        // we don’t need to await dismissal on a Notice
        return Task.CompletedTask;
    }

    public Task<bool> ShowChoiceAsync(string title, string message)
    {
        var popup = new ChoicePopup(title, message);
        Shell.Current.ShowPopup(popup);
        // now await the Task<bool> that ChoicePopup exposes
        return popup.Task;
    }

    public Task<string?> ShowEntryAsync(string title, string message, string? initialValue = null)
    {
        var popup = new EntryPopup(title, message, initialValue);
        Shell.Current.ShowPopup(popup);
        return popup.Task;
    }

    public Task<string?> ShowSelectAsync(string title, IEnumerable<string> items, string? selected = null)
    {
        var popup = new SelectPopup(title, items, selected);
        Shell.Current.ShowPopup(popup);
        return popup.Task;
    }
}
