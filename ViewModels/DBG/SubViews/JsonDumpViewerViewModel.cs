// JsonDumpViewerViewModel.cs
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AvaTerminal3.Services.Interfaces;
using AvaTerminal3.Helpers;
using AvaTerminal3.Models.Kernel.SysVar;

namespace AvaTerminal3.ViewModels.DBG.SubViews;

public partial class JsonDumpViewerViewModel : ObservableObject
{
    private readonly IAvaApiService _avaApiService;
    private readonly IAuthService _authService;

    public JsonDumpViewerViewModel(IAvaApiService avaApiService, IAuthService authService)
    {
        _avaApiService = avaApiService;
        _authService = authService;

        JsonDumpPath = LogSinkService.GetDumpFilePath();

        // kick off load
        LoadJsonDumpContentsCommand = new AsyncRelayCommand(LoadJsonDumpContentsAsync);

        // navigation / actions
        BackCommand = new RelayCommand(async () =>
            await Shell.Current.Navigation.PopAsync(animated: true)
        );
        DeleteFileCommand = new AsyncRelayCommand(DeleteJsonDumpAsync);
        CopyContentCommand = new RelayCommand(CopyContent);
        LogTicketCommand = new AsyncRelayCommand(LogTicketPlaceholderAsync);
    }

    [ObservableProperty]
    string jsonDumpPath;

    [ObservableProperty]
    string statusMessage = string.Empty;

    [ObservableProperty]
    string jsonDumpFileContent = string.Empty;

    public bool HasStatusMessage 
        => !string.IsNullOrWhiteSpace(StatusMessage);

    // —— Commands ——
    public IAsyncRelayCommand LoadJsonDumpContentsCommand { get; }
    public IRelayCommand      BackCommand            { get; }
    public IAsyncRelayCommand DeleteFileCommand      { get; }
    public IRelayCommand      CopyContentCommand     { get; }
    public IRelayCommand      LogTicketCommand       { get; }

    // —— Implementation ——

    private async Task LoadJsonDumpContentsAsync()
    {
        try
        {
            // READ ENTIRE FILE (includes line breaks)
            JsonDumpFileContent = await File.ReadAllTextAsync(JsonDumpPath);
            StatusMessage  = string.Empty;
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error loading log: {ex.Message}";
        }
    }

    private async Task DeleteJsonDumpAsync()
    {
        try
        {
            if (File.Exists(JsonDumpPath))
            {
                LogSinkService.DeleteDumpFile();
                StatusMessage = "Dump file deleted.";
                await Shell.Current.GoToAsync("..");
            }
            else
            {
                StatusMessage = "Json dump file not found.";
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Failed to delete json dump file: {ex.Message}";
        }
    }

    private void CopyContent()
    {
        _ = Clipboard.SetTextAsync(JsonDumpFileContent);
        StatusMessage = "Json dump file content copied to clipboard.";
    }

    private async Task LogTicketPlaceholderAsync()
    {
        // get the current logged in user
        var _userId = await _authService.GetLoggedInUserAsync();

        // create internal ticket to be logged
        InternalSupportTicket supportTicket = new InternalSupportTicket
        {
            IssueId = "n/a",
            Subject = "JsonDumpFile Ticket Upload",
            Category = "Internal_JsonDumpFileUpload",
            UserId = _userId,
            Message = JsonDumpFileContent,
        };

        // upload it to api (api handles it)
        var ticketResponse = await _avaApiService.PostInternalSupportTicketAsync(supportTicket);

        if (ticketResponse)
        {
            StatusMessage = "Ticket logged successfully with IT.";
        }
        else
        {
            StatusMessage = "Ticket not logged. Consult LogFile.";
        }
    }
}
