namespace AvaTerminal3.Models.Kernel.SysVar;

public class InternalSupportTicket
{
    public required string IssueId { get; set; }
    public required string Subject { get; set; }
    public required string Category { get; set; }
    public required string UserId { get; set; }
    public required string Message { get; set; }
}