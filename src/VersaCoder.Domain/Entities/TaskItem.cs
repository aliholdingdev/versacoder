using VersaCoder.Domain.Enums;

namespace VersaCoder.Domain.Entities;

/// <summary>
/// Ana task entity'si — Tüm görev yönetim özelliklerini destekler.
/// Alt görevler (parent-child), bağımlılıklar, etiketler, hatırlatıcılar, notlar desteklenir.
/// </summary>
public class TaskItem
{
    private const int MaxTitleLength = 500;
    private const int MaxDescriptionLength = 5000;
    private const int MaxNotesLength = 10000;

    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TaskItemStatus Status { get; set; }
    public Priority Priority { get; set; }
    public string Notes { get; set; } = string.Empty;

    // Parent-child (subtask) support
    public Guid? ParentTaskId { get; set; }
    public Guid? TaskListId { get; set; }
    public Guid? SessionId { get; set; }
    public string? AssignedTo { get; set; }

    // Timestamps
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime? StartedAt { get; set; }

    // Estimated duration
    public decimal? EstimatedHours { get; set; }
    public decimal? ActualHours { get; set; }
    public DurationType DurationType { get; set; }

    // Reminder
    public DateTime? ReminderDate { get; set; }
    public string? ReminderMessage { get; set; }
    public bool ReminderSent { get; set; }

    // Token limits
    public int MaxTokenTitle { get; set; } = 200;
    public int MaxTokenNotes { get; set; } = 2000;

    // Ordering
    public int SortOrder { get; set; }

    // Milestone flag
    public bool IsMilestone { get; set; }

    // Navigation properties
    public TaskItem? ParentTask { get; set; }
    public List<TaskItem> SubTasks { get; set; } = new();
    public TaskList? TaskList { get; set; }
    public List<TaskTag> Tags { get; set; } = new();
    public List<TaskDependency> Dependencies { get; set; } = new();
    public List<TaskDependency> Dependents { get; set; } = new();
    public List<TaskReminder> Reminders { get; set; } = new();

    protected TaskItem() { }

    public TaskItem(string title, Priority priority = Priority.MEDIUM, Guid? taskListId = null, Guid? sessionId = null)
    {
        Id = Guid.NewGuid();
        Title = ValidateAndTruncateTitle(title);
        Status = TaskItemStatus.NEW;
        Priority = priority;
        TaskListId = taskListId;
        SessionId = sessionId;
        DurationType = DurationType.HOURS;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Start()
    {
        if (Status != TaskItemStatus.NEW && Status != TaskItemStatus.ON_HOLD && Status != TaskItemStatus.REVIEW)
            throw new InvalidOperationException($"Cannot start task in {Status} status.");

        Status = TaskItemStatus.IN_PROGRESS;
        StartedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Complete()
    {
        if (Status != TaskItemStatus.IN_PROGRESS && Status != TaskItemStatus.REVIEW)
            throw new InvalidOperationException($"Cannot complete task in {Status} status.");

        Status = TaskItemStatus.COMPLETED;
        CompletedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Hold()
    {
        if (Status != TaskItemStatus.IN_PROGRESS)
            throw new InvalidOperationException($"Cannot hold task in {Status} status.");

        Status = TaskItemStatus.ON_HOLD;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Cancel()
    {
        if (Status == TaskItemStatus.COMPLETED)
            throw new InvalidOperationException("Cannot cancel a completed task.");

        Status = TaskItemStatus.CANCELLED;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Fail()
    {
        Status = TaskItemStatus.FAILED;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SendToReview()
    {
        if (Status != TaskItemStatus.IN_PROGRESS && Status != TaskItemStatus.FAILED)
            throw new InvalidOperationException($"Cannot send to review from {Status} status.");

        Status = TaskItemStatus.REVIEW;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateTitle(string title)
    {
        Title = ValidateAndTruncateTitle(title);
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateDescription(string description)
    {
        Description = description?.Length > MaxDescriptionLength
            ? description[..MaxDescriptionLength]
            : description ?? string.Empty;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateNotes(string notes)
    {
        Notes = notes?.Length > MaxNotesLength
            ? notes[..MaxNotesLength]
            : notes ?? string.Empty;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetEstimatedDuration(decimal hours, DurationType type)
    {
        EstimatedHours = hours;
        DurationType = type;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetDueDate(DateTime dueDate)
    {
        DueDate = dueDate;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetReminder(DateTime reminderDate, string? message = null)
    {
        ReminderDate = reminderDate;
        ReminderMessage = message;
        ReminderSent = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkReminderSent()
    {
        ReminderSent = true;
    }

    public bool IsOverdue()
    {
        return DueDate.HasValue && DateTime.UtcNow > DueDate.Value && Status != TaskItemStatus.COMPLETED && Status != TaskItemStatus.CANCELLED;
    }

    public bool HasDependencies()
    {
        return Dependencies.Count > 0;
    }

    public bool AreDependenciesMet()
    {
        return Dependencies.All(d => d.DependsOnTask.Status == TaskItemStatus.COMPLETED);
    }

    public int GetDepth()
    {
        int depth = 0;
        var current = ParentTask;
        while (current != null)
        {
            depth++;
            current = current.ParentTask;
        }
        return depth;
    }

    private static string ValidateAndTruncateTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Task title cannot be null or empty.", nameof(title));

        return title.Length > MaxTitleLength ? title[..MaxTitleLength] : title;
    }
}
