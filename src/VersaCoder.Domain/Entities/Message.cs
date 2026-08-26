namespace VersaCoder.Domain.Entities;

public class Message
{
    public Guid Id { get; set; }
    public Guid SessionId { get; set; }
    public string Role { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? AgentName { get; set; }
    public DateTime Timestamp { get; set; }
    public Dictionary<string, object> Metadata { get; set; } = new();

    public Session Session { get; set; } = null!;

    protected Message() { }

    public Message(Guid sessionId, string role, string content)
    {
        Id = Guid.NewGuid();
        SessionId = sessionId;
        Role = role ?? throw new ArgumentNullException(nameof(role));
        Content = content ?? throw new ArgumentNullException(nameof(content));
        Timestamp = DateTime.UtcNow;
    }
}
