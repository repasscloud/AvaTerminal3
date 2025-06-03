// DBGViewModel.cs
using System;
using System.IO;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.ApplicationModel;    // for Clipboard
using AvaTerminal3.Services.Interfaces;
using AvaTerminal3.Helpers;

namespace AvaTerminal3.ViewModels.DBG;

public partial class DBGViewModel : ObservableObject
{
    readonly IAvaApiService _avaApiService;

    public DBGViewModel(IAvaApiService avaApiService)
    {
        _avaApiService = avaApiService;

        // seed initial paths
        LogPath  = LogSinkService.GetLogPath();
        DumpPath = LogSinkService.GetDumpFilePath();

        // commands
        CheckApiHealthCommand = new AsyncRelayCommand(CheckApiHealthAsync);
        // GetApiVersionCommand  = new AsyncRelayCommand(GetApiVersionAsync);
        // DeleteLogCommand      = new RelayCommand(DeleteLog);
        // DeleteDumpCommand     = new RelayCommand(DeleteDump);
        // ViewLogCommand        = new RelayCommand(ViewLog);
        // ViewDumpCommand       = new RelayCommand(ViewDump);
        // CopyLogPathCommand    = new RelayCommand(CopyLogPath);
        // CopyDumpPathCommand   = new RelayCommand(CopyDumpPath);
    }

    [ObservableProperty]
    string logPath;

    [ObservableProperty]
    string dumpPath;

    [ObservableProperty]
    bool isCheckingApi;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ApiStatusText))]
    bool apiHealthy;

    public string ApiStatusText
        => ApiHealthy ? "✅ API is healthy" : "❌ API is down or unhealthy";

    [ObservableProperty]
    string statusMessage = string.Empty;

    // public bool HasStatusMessage
    //     => !string.IsNullOrWhiteSpace(StatusMessage);

    // // —— Raw API version ⤵️
    // [ObservableProperty]
    // [NotifyPropertyChangedFor(nameof(ApiVersionText))]
    // string apiVersionRaw = string.Empty;

    // // Computed nicely formatted version ⤵️
    // public string ApiVersionText
    //     => string.IsNullOrWhiteSpace(ApiVersionRaw)
    //        ? "Unknown"
    //        : ApiVersionRaw;

    // —— Commands ——
    public IAsyncRelayCommand CheckApiHealthCommand { get; }
    // public IAsyncRelayCommand GetApiVersionCommand  { get; }
    // public IRelayCommand    DeleteLogCommand        { get; }
    // public IRelayCommand    DeleteDumpCommand       { get; }
    // public IRelayCommand    ViewLogCommand          { get; }
    // public IRelayCommand    ViewDumpCommand         { get; }
    // public IRelayCommand    CopyLogPathCommand      { get; }
    // public IRelayCommand    CopyDumpPathCommand     { get; }

    // // —— Implementations ——

    private async Task CheckApiHealthAsync()
    {
        try
        {
            IsCheckingApi = true;
            ApiHealthy    = await _avaApiService.IsApiHealthyAsync();
        }
        catch (Exception ex)
        {
            StatusMessage = $"Health check failed: {ex.Message}";
            ApiHealthy    = false;
        }
        finally
        {
            IsCheckingApi = false;
        }
    }

    // private async Task GetApiVersionAsync()
    // {
    //     try
    //     {
    //         // Setting the generated ApiVersionRaw property
    //         ApiVersionRaw = await _avaApiService.GetApiVersionStringAsync();
    //     }
    //     catch (Exception ex)
    //     {
    //         StatusMessage = $"API version check failed: {ex.Message}";
    //     }
    // }

    // private void DeleteLog()
    // {
    //     try
    //     {
    //         var path = LogSinkService.GetLogPath();
    //         if (File.Exists(path))
    //         {
    //             LogSinkService.DeleteLogFile();
    //             StatusMessage = "Log file deleted.";
    //         }
    //         else
    //         {
    //             StatusMessage = "Log file not found.";
    //         }
    //     }
    //     catch (Exception ex)
    //     {
    //         StatusMessage = $"Failed to delete log file: {ex.Message}";
    //     }
    // }

    // private void DeleteDump()
    // {
    //     try
    //     {
    //         var path = LogSinkService.GetDumpFilePath();
    //         if (File.Exists(path))
    //         {
    //             LogSinkService.DeleteDumpFile();
    //             StatusMessage = "Dump file deleted.";
    //         }
    //         else
    //         {
    //             StatusMessage = "Dump file not found.";
    //         }
    //     }
    //     catch (Exception ex)
    //     {
    //         StatusMessage = $"Failed to delete dump file: {ex.Message}";
    //     }
    // }

    // private void ViewLog()
    // {
    //     StatusMessage = "Log would have been viewed here.";
    // }

    // private void ViewDump()
    // {
    //     StatusMessage = "Dump would have been viewed here.";
    // }

    // private void CopyLogPath()
    // {
    //     Clipboard.SetTextAsync(LogPath);
    //     StatusMessage = "Log path copied to clipboard.";
    // }

    // private void CopyDumpPath()
    // {
    //     Clipboard.SetTextAsync(DumpPath);
    //     StatusMessage = "Dump path copied to clipboard.";
    // }
}