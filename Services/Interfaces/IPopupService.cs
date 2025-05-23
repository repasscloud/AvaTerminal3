// Services/Interfaces/IPopupService.cs
namespace AvaTerminal3.Services.Interfaces;
public interface IPopupService
{
    Task ShowNoticeAsync(string title, string message);
    Task<bool> ShowChoiceAsync(string title, string message);
    Task<string?> ShowEntryAsync(string title, string message, string? initialValue = null);
    Task<string?> ShowSelectAsync(string title, IEnumerable<string> items, string? selected = null);
}
