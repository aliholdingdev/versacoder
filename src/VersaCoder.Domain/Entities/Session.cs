using VersaCoder.Domain.Enums;

namespace VersaCoder.Domain.Entities;

public class Session
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public SessionState State { get; set; }
    public Guid? ParentId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public Dictionary<string, object> Metadata { get; set; } = new();

    public Session? Parent { get; set; }
    public List<Session> Branches { get; set; } = new();
    public List<Message> Messages { get; set; } = new();

    protected Session() { }

    public Session(string name)
    {
        Id = Guid.NewGuid();
        Name = name ?? throw new ArgumentNullException(nameof(name));
        State = SessionState.ACTIVE;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateName(string newName)
    {
        Name = newName ?? throw new ArgumentNullException(nameof(newName));
        UpdatedAt = DateTime.UtcNow;
    }

    public void Complete()
    {
        State = SessionState.COMPLETED;
        CompletedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Pause()
    {
        State = SessionState.PAUSED;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Resume()
    {
        State = SessionState.ACTIVE;
        UpdatedAt = DateTime.UtcNow;
    }
}
