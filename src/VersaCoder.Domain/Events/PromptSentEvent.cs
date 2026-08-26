namespace VersaCoder.Domain.Events;

public class PromptSentEvent : DomainEvent
{
    public Guid SessionId { get; }
    public string Content { get; }

    public PromptSentEvent(Guid sessionId, string content)
    {
        SessionId = sessionId;
        Content = content;
    }
}
