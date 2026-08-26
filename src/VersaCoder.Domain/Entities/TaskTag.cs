namespace VersaCoder.Domain.Entities;

/// <summary>
/// Task tag entity'si — Etiket destekli gruplama ve filtreleme.
/// Birden fazla task'a atanabilir, birden fazla task listesinde kullanılabilir.
/// </summary>
public class TaskTag
{
    private const int MaxNameLength = 100;

    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Color { get; set; } = "#6B7280"; // Default gray
    public DateTime CreatedAt { get; set; }

    // Navigation
    public List<TaskItem> Tasks { get; set; } = new();

    protected TaskTag() { }

    public TaskTag(string name, string color = "#6B7280")
    {
        Id = Guid.NewGuid();
        Name = ValidateAndTruncateName(name);
        Color = color ?? "#6B7280";
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdateName(string name)
    {
        Name = ValidateAndTruncateName(name);
    }

    public void UpdateColor(string color)
    {
        Color = color ?? "#6B7280";
    }

    private static string ValidateAndTruncateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Tag name cannot be null or empty.", nameof(name));

        return name.Length > MaxNameLength ? name[..MaxNameLength] : name;
    }
}
