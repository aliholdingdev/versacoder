namespace VersaCoder.Application.DTOs;

public class SessionDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public Guid? ParentId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public int MessageCount { get; set; }
    public Dictionary<string, object> Metadata { get; set; } = new();
}
