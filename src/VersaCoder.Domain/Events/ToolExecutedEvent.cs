namespace VersaCoder.Domain.Events;

public class ToolExecutedEvent : DomainEvent
{
    public string ToolName { get; }
    public bool Success { get; }
    public TimeSpan Duration { get; }

    public ToolExecutedEvent(string toolName, bool success, TimeSpan duration)
    {
        ToolName = toolName;
        Success = success;
        Duration = duration;
    }
}
