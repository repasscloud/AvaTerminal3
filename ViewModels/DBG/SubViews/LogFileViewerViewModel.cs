// LogFileViewerViewModel.cs
using System;
using System.IO;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.ApplicationModel;    // for Clipboard
using Microsoft.Maui.Controls;           // for Shell pop
using AvaTerminal3.Services.Interfaces;
using AvaTerminal3.Helpers;

namespace AvaTerminal3.ViewModels.DBG.SubViews;

public partial class LogFileViewerViewModel : ObservableObject
{
    public LogFileViewerViewModel()
    {
        LogPath = LogSinkService.GetLogPath();

        // kick off load
        LoadLogContentsCommand = new AsyncRelayCommand(LoadLogContentsAsync);

        // navigation / actions
        BackCommand        = new RelayCommand(() => _ = Shell.Current.GoToAsync(".."));
        DeleteFileCommand  = new AsyncRelayCommand(DeleteLogAsync);
        CopyContentCommand = new RelayCommand(CopyContent);
        LogTicketCommand   = new RelayCommand(LogTicketPlaceholder);
    }

    [ObservableProperty]
    string logPath;

    [ObservableProperty]
    string statusMessage = string.Empty;

    [ObservableProperty]
    string logFileContent = string.Empty;    // renamed from 'logContents'

    public bool HasStatusMessage 
        => !string.IsNullOrWhiteSpace(StatusMessage);

    // —— Commands ——
    public IAsyncRelayCommand LoadLogContentsCommand { get; }
    public IRelayCommand       BackCommand            { get; }
    public IAsyncRelayCommand  DeleteFileCommand      { get; }
    public IRelayCommand       CopyContentCommand     { get; }
    public IRelayCommand       LogTicketCommand       { get; }

    // —— Implementation ——

    private async Task LoadLogContentsAsync()
    {
        try
        {
            // READ ENTIRE FILE (includes line breaks)
            LogFileContent = await File.ReadAllTextAsync(LogPath);
            StatusMessage  = string.Empty;
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error loading log: {ex.Message}";
        }
    }

    private async Task DeleteLogAsync()
    {
        try
        {
            if (File.Exists(LogPath))
            {
                LogSinkService.DeleteLogFile();
                StatusMessage = "Log file deleted.";
                await Shell.Current.GoToAsync("..");
            }
            else
            {
                StatusMessage = "Log file not found.";
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Failed to delete log: {ex.Message}";
        }
    }

    private void CopyContent()
    {
        _ = Clipboard.SetTextAsync(LogFileContent);
        StatusMessage = "Log content copied to clipboard.";
    }

    private void LogTicketPlaceholder()
    {
        StatusMessage = "LogTicket action (not yet implemented).";
    }
}
