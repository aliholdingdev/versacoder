using VersaCoder.Domain.Enums;

namespace VersaCoder.Domain.Events;

public class SessionCreatedEvent : DomainEvent
{
    public Guid SessionId { get; }
    public string Name { get; }

    public SessionCreatedEvent(Guid sessionId, string name)
    {
        SessionId = sessionId;
        Name = name;
    }
}
