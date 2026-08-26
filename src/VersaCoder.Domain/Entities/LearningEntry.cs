using VersaCoder.Domain.Enums;

namespace VersaCoder.Domain.Entities;

public class LearningEntry
{
    public Guid Id { get; set; }
    public LearningCategory Category { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public float Confidence { get; set; }
    public string? Source { get; set; }
    public int AppliedCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastAppliedAt { get; set; }

    protected LearningEntry() { }

    public LearningEntry(LearningCategory category, string key, string value)
    {
        Id = Guid.NewGuid();
        Category = category;
        Key = key ?? throw new ArgumentNullException(nameof(key));
        Value = value ?? throw new ArgumentNullException(nameof(value));
        Confidence = 0.5f;
        AppliedCount = 0;
        CreatedAt = DateTime.UtcNow;
        LastAppliedAt = DateTime.MinValue;
    }

    public void Apply()
    {
        AppliedCount++;
        LastAppliedAt = DateTime.UtcNow;
        Confidence = Math.Min(1.0f, Confidence + 0.05f);
    }
}
