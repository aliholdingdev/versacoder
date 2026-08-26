namespace VersaCoder.Domain.Events;

public class ResponseReceivedEvent : DomainEvent
{
    public Guid SessionId { get; }
    public string AgentName { get; }
    public string Content { get; }

    public ResponseReceivedEvent(Guid sessionId, string agentName, string content)
    {
        SessionId = sessionId;
        AgentName = agentName;
        Content = content;
    }
}
