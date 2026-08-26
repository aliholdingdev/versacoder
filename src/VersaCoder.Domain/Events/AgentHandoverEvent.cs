namespace VersaCoder.Domain.Events;

public class AgentHandoverEvent : DomainEvent
{
    public string SourceAgent { get; }
    public string TargetAgent { get; }
    public string Reason { get; }

    public AgentHandoverEvent(string sourceAgent, string targetAgent, string reason)
    {
        SourceAgent = sourceAgent;
        TargetAgent = targetAgent;
        Reason = reason;
    }
}
