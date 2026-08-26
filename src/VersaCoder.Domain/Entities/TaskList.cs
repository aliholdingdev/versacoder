using VersaCoder.Domain.Enums;

namespace VersaCoder.Domain.Entities;

/// <summary>
/// Task list entity'si — Birden fazla task listesi desteği.
/// Renk, simge, varsayılan öncelik ve otomatik arşivleme destekler.
/// Hem fiziksel tablo hem de gruplama mekanizması olarak kullanılır.
/// </summary>
public class TaskList
{
    private const int MaxNameLength = 200;
    private const int MaxDescriptionLength = 1000;

    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Color { get; set; } = "#3B82F6"; // Default blue
    public string Icon { get; set; } = "list"; // Default icon
    public Priority DefaultPriority { get; set; } = Priority.MEDIUM;
    public int AutoArchiveDays { get; set; } = 0; // 0 = no auto-archive
    public bool IsArchived { get; set; }
    public int SortOrder { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation
    public List<TaskItem> Tasks { get; set; } = new();

    protected TaskList() { }

    public TaskList(string name, string description = "")
    {
        Id = Guid.NewGuid();
        Name = ValidateAndTruncateName(name);
        Description = description?.Length > MaxDescriptionLength
            ? description[..MaxDescriptionLength]
            : description ?? string.Empty;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateName(string name)
    {
        Name = ValidateAndTruncateName(name);
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateDescription(string description)
    {
        Description = description?.Length > MaxDescriptionLength
            ? description[..MaxDescriptionLength]
            : description ?? string.Empty;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateColor(string color)
    {
        Color = color ?? "#3B82F6";
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateIcon(string icon)
    {
        Icon = icon ?? "list";
        UpdatedAt = DateTime.UtcNow;
    }

    public void Archive()
    {
        IsArchived = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Unarchive()
    {
        IsArchived = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public bool ShouldAutoArchive()
    {
        if (AutoArchiveDays <= 0 || IsArchived) return false;

        var lastActivity = Tasks.Any()
            ? Tasks.Max(t => t.UpdatedAt)
            : CreatedAt;

        return (DateTime.UtcNow - lastActivity).TotalDays > AutoArchiveDays;
    }

    public int GetTotalTaskCount() => Tasks.Count;

    public int GetCompletedTaskCount() => Tasks.Count(t => t.Status == TaskItemStatus.COMPLETED);

    public int GetInProgressTaskCount() => Tasks.Count(t => t.Status == TaskItemStatus.IN_PROGRESS);

    public double GetCompletionPercentage()
    {
        var total = GetTotalTaskCount();
        return total == 0 ? 0 : (double)GetCompletedTaskCount() / total * 100;
    }

    private static string ValidateAndTruncateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Task list name cannot be null or empty.", nameof(name));

        return name.Length > MaxNameLength ? name[..MaxNameLength] : name;
    }
}
