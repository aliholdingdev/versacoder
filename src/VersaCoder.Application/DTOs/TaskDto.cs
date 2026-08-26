using VersaCoder.Domain.Enums;

namespace VersaCoder.Application.DTOs;

/// <summary>
/// Task DTO'su — UI ve API arası veri transferi için.
/// Domain entity'den ayrıştırılmış, безопасный view model.
/// </summary>
public record TaskDto
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public TaskItemStatus Status { get; init; }
    public Priority Priority { get; init; }
    public string Notes { get; init; } = string.Empty;

    // Hierarchy
    public Guid? ParentTaskId { get; init; }
    public string? ParentTaskTitle { get; init; }
    public int SubTaskCount { get; init; }
    public int CompletedSubTaskCount { get; init; }
    public int Depth { get; init; }

    // List & Session
    public Guid? TaskListId { get; init; }
    public string? TaskListName { get; init; }
    public Guid? SessionId { get; init; }
    public string? AssignedTo { get; init; }

    // Timestamps
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
    public DateTime? DueDate { get; init; }
    public DateTime? CompletedAt { get; init; }
    public DateTime? StartedAt { get; init; }

    // Duration
    public decimal? EstimatedHours { get; init; }
    public decimal? ActualHours { get; init; }
    public DurationType DurationType { get; init; }

    // Reminder
    public DateTime? ReminderDate { get; init; }
    public string? ReminderMessage { get; init; }
    public bool ReminderSent { get; init; }

    // Token limits
    public int MaxTokenTitle { get; init; }
    public int MaxTokenNotes { get; init; }

    // Ordering & Flags
    public int SortOrder { get; init; }
    public bool IsMilestone { get; init; }
    public bool IsOverdue { get; init; }

    // Dependencies
    public int DependencyCount { get; init; }
    public bool AreDependenciesMet { get; init; }

    // Tags
    public List<TaskTagDto> Tags { get; init; } = new();
    public List<TaskDto> SubTasks { get; init; } = new();

    // Computed
    public double CompletionPercentage =>
        SubTaskCount == 0 ? (Status == TaskItemStatus.COMPLETED ? 100 : 0) :
        (double)CompletedSubTaskCount / SubTaskCount * 100;
}

/// <summary>
/// Task oluşturma/güncelleme DTO'su — Input validation için.
/// </summary>
public record CreateTaskDto
{
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public Priority Priority { get; init; } = Priority.MEDIUM;
    public Guid? ParentTaskId { get; init; }
    public Guid? TaskListId { get; init; }
    public Guid? SessionId { get; init; }
    public string? AssignedTo { get; init; }
    public DateTime? DueDate { get; init; }
    public decimal? EstimatedHours { get; init; }
    public DurationType DurationType { get; init; } = DurationType.HOURS;
    public bool IsMilestone { get; init; }
    public int MaxTokenTitle { get; init; } = 200;
    public int MaxTokenNotes { get; init; } = 2000;
    public List<string> TagNames { get; init; } = new();
}

/// <summary>
/// Task güncelleme DTO'su — Partial update için.
/// </summary>
public record UpdateTaskDto
{
    public string? Title { get; init; }
    public string? Description { get; init; }
    public Priority? Priority { get; init; }
    public string? Notes { get; init; }
    public Guid? TaskListId { get; init; }
    public string? AssignedTo { get; init; }
    public DateTime? DueDate { get; init; }
    public decimal? EstimatedHours { get; init; }
    public DurationType? DurationType { get; init; }
    public bool? IsMilestone { get; init; }
    public int? MaxTokenTitle { get; init; }
    public int? MaxTokenNotes { get; init; }
    public int? SortOrder { get; init; }
}
