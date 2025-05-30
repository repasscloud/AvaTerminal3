namespace AvaTerminal3.Helpers;

public enum LogLevel
{
    Debug, // Detailed developer-level info, used for tracing and diagnostics
    Info,  // General application events, high-level actions or state changes
    Warn,  // Something unexpected, but not breaking; may need attention
    Error, // A problem that caused a failure in the current operation
    Fatal  // A critical issue that caused the entire app/process to crash or shut down
}
