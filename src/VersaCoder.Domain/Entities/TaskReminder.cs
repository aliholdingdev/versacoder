namespace VersaCoder.Domain.Entities;

/// <summary>
/// Task hatırlatıcı entity'si — Birden fazla hatırlatıcı destekler.
/// AI tarafından yazılır, zamanı geldiğinde gönderilir.
/// </summary>
public class TaskReminder
{
    public Guid Id { get; set; }
    public Guid TaskId { get; set; }
    public DateTime ReminderDate { get; set; }
    public string? Message { get; set; }
    public bool IsSent { get; set; }
    public DateTime? SentAt { get; set; }
    public DateTime CreatedAt { get; set; }

    // Navigation
    public TaskItem Task { get; set; } = null!;

    protected TaskReminder() { }

    public TaskReminder(Guid taskId, DateTime reminderDate, string? message = null)
    {
        Id = Guid.NewGuid();
        TaskId = taskId;
        ReminderDate = reminderDate;
        Message = message;
        IsSent = false;
        CreatedAt = DateTime.UtcNow;
    }

    public void MarkAsSent()
    {
        IsSent = true;
        SentAt = DateTime.UtcNow;
    }

    public bool IsDue()
    {
        return !IsSent && DateTime.UtcNow >= ReminderDate;
    }
}
