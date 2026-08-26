using VersaCoder.Domain.Enums;

namespace VersaCoder.Domain.Entities;

/// <summary>
/// Task bağımlılık entity'si — Gantt chart desteği için 4 çeşit bağımlılık.
/// Finish-to-Start, Start-to-Start, Finish-to-Finish, Start-to-Finish.
/// </summary>
public class TaskDependency
{
    public Guid Id { get; set; }
    public Guid TaskId { get; set; }
    public Guid DependsOnTaskId { get; set; }
    public DependencyType DependencyType { get; set; }
    public DateTime CreatedAt { get; set; }

    // Navigation
    public TaskItem Task { get; set; } = null!;
    public TaskItem DependsOnTask { get; set; } = null!;

    protected TaskDependency() { }

    public TaskDependency(Guid taskId, Guid dependsOnTaskId, DependencyType dependencyType = DependencyType.FINISH_TO_START)
    {
        if (taskId == dependsOnTaskId)
            throw new ArgumentException("A task cannot depend on itself.");

        Id = Guid.NewGuid();
        TaskId = taskId;
        DependsOnTaskId = dependsOnTaskId;
        DependencyType = dependencyType;
        CreatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Bağımlılığın противни olebilirliğini kontrol eder.
    /// Döngüsel bağımlılık olup olmadığını belirler.
    /// </summary>
    public bool IsBlocked()
    {
        return DependencyType switch
        {
            DependencyType.FINISH_TO_START => DependsOnTask.Status != TaskItemStatus.COMPLETED,
            DependencyType.START_TO_START => DependsOnTask.Status == TaskItemStatus.NEW,
            DependencyType.FINISH_TO_FINISH => DependsOnTask.Status != TaskItemStatus.COMPLETED,
            DependencyType.START_TO_FINISH => DependsOnTask.Status == TaskItemStatus.NEW,
            _ => false
        };
    }

    /// <summary>
    /// Bağımlı task'ın başlayabileceği tarihi hesaplar.
    /// Gantt chart çizimi için kullanılır.
    /// </summary>
    public DateTime? GetEarliestStartDate()
    {
        return DependencyType switch
        {
            DependencyType.FINISH_TO_START => DependsOnTask.CompletedAt ?? DependsOnTask.DueDate,
            DependencyType.START_TO_START => DependsOnTask.StartedAt ?? DependsOnTask.CreatedAt,
            DependencyType.START_TO_FINISH => DependsOnTask.StartedAt ?? DependsOnTask.CreatedAt,
            _ => null
        };
    }
}
