using VersaCoder.Domain.Enums;

namespace VersaCoder.Application.DTOs;

/// <summary>
/// TaskList DTO'su — UI ve API arası veri transferi için.
/// </summary>
public record TaskListDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string Color { get; init; } = "#3B82F6";
    public string Icon { get; init; } = "list";
    public Priority DefaultPriority { get; init; }
    public int AutoArchiveDays { get; init; }
    public bool IsArchived { get; init; }
    public int SortOrder { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }

    // Computed
    public int TotalTaskCount { get; init; }
    public int CompletedTaskCount { get; init; }
    public int InProgressTaskCount { get; init; }
    public double CompletionPercentage { get; init; }
}

/// <summary>
/// TaskList oluşturma DTO'su.
/// </summary>
public record CreateTaskListDto
{
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string Color { get; init; } = "#3B82F6";
    public string Icon { get; init; } = "list";
    public Priority DefaultPriority { get; init; } = Priority.MEDIUM;
    public int AutoArchiveDays { get; init; } = 0;
}

/// <summary>
/// TaskList güncelleme DTO'su.
/// </summary>
public record UpdateTaskListDto
{
    public string? Name { get; init; }
    public string? Description { get; init; }
    public string? Color { get; init; }
    public string? Icon { get; init; }
    public Priority? DefaultPriority { get; init; }
    public int? AutoArchiveDays { get; init; }
    public int? SortOrder { get; init; }
}

/// <summary>
/// TaskTag DTO'su.
/// </summary>
public record TaskTagDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Color { get; init; } = "#6B7280";
    public int TaskCount { get; init; }
    public DateTime CreatedAt { get; init; }
}
