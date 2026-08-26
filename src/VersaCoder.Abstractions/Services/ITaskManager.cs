using VersaCoder.Domain.Entities;
using VersaCoder.Domain.Enums;

namespace VersaCoder.Abstractions.Services;

/// <summary>
/// Task yönetimi servisi arayüzü — Task CRUD, durum makinesi, alt görev,
/// bağımlılık, etiket, hatırlatıcı ve not yönetimi.
/// </summary>
public interface ITaskManager
{
    // CRUD Operations
    Task<TaskItem> CreateTaskAsync(string title, Priority priority = Priority.MEDIUM,
        Guid? taskListId = null, Guid? sessionId = null, CancellationToken cancellationToken = default);
    Task<TaskItem?> GetTaskAsync(Guid taskId, CancellationToken cancellationToken = default);
    Task<TaskItem?> GetTaskWithDetailsAsync(Guid taskId, CancellationToken cancellationToken = default);
    Task<List<TaskItem>> GetAllTasksAsync(CancellationToken cancellationToken = default);
    Task<TaskItem> UpdateTaskAsync(Guid taskId, string? title = null, string? description = null,
        Priority? priority = null, CancellationToken cancellationToken = default);
    Task DeleteTaskAsync(Guid taskId, CancellationToken cancellationToken = default);

    // Status Management (State Machine)
    Task StartTaskAsync(Guid taskId, CancellationToken cancellationToken = default);
    Task CompleteTaskAsync(Guid taskId, CancellationToken cancellationToken = default);
    Task HoldTaskAsync(Guid taskId, CancellationToken cancellationToken = default);
    Task CancelTaskAsync(Guid taskId, CancellationToken cancellationToken = default);
    Task FailTaskAsync(Guid taskId, CancellationToken cancellationToken = default);
    Task SendToReviewAsync(Guid taskId, CancellationToken cancellationToken = default);

    // Subtask Operations
    Task<TaskItem> CreateSubTaskAsync(Guid parentTaskId, string title,
        Priority priority = Priority.MEDIUM, CancellationToken cancellationToken = default);
    Task<List<TaskItem>> GetSubTasksAsync(Guid parentTaskId, CancellationToken cancellationToken = default);
    Task<List<TaskItem>> GetTaskHierarchyAsync(Guid taskId, CancellationToken cancellationToken = default);

    // Dependency Operations
    Task AddDependencyAsync(Guid taskId, Guid dependsOnTaskId,
        DependencyType dependencyType = DependencyType.FINISH_TO_START, CancellationToken cancellationToken = default);
    Task RemoveDependencyAsync(Guid taskId, Guid dependsOnTaskId, CancellationToken cancellationToken = default);
    Task<List<TaskDependency>> GetDependenciesAsync(Guid taskId, CancellationToken cancellationToken = default);
    Task<bool> AreDependenciesMetAsync(Guid taskId, CancellationToken cancellationToken = default);
    Task<List<TaskItem>> GetBlockedTasksAsync(CancellationToken cancellationToken = default);

    // Tag Operations
    Task<TaskTag> CreateTagAsync(string name, string color = "#6B7280", CancellationToken cancellationToken = default);
    Task AddTagToTaskAsync(Guid taskId, Guid tagId, CancellationToken cancellationToken = default);
    Task RemoveTagFromTaskAsync(Guid taskId, Guid tagId, CancellationToken cancellationToken = default);
    Task<List<TaskTag>> GetTagsForTaskAsync(Guid taskId, CancellationToken cancellationToken = default);
    Task<List<TaskItem>> GetTasksByTagAsync(string tagName, CancellationToken cancellationToken = default);

    // Reminder Operations
    Task<TaskReminder> SetReminderAsync(Guid taskId, DateTime reminderDate,
        string? message = null, CancellationToken cancellationToken = default);
    Task<List<TaskReminder>> GetRemindersForTaskAsync(Guid taskId, CancellationToken cancellationToken = default);
    Task<List<TaskReminder>> GetDueRemindersAsync(CancellationToken cancellationToken = default);

    // Notes
    Task UpdateNotesAsync(Guid taskId, string notes, CancellationToken cancellationToken = default);

    // Estimated Duration
    Task SetEstimatedDurationAsync(Guid taskId, decimal hours, DurationType type,
        CancellationToken cancellationToken = default);

    // Due Date
    Task SetDueDateAsync(Guid taskId, DateTime dueDate, CancellationToken cancellationToken = default);

    // Query Operations
    Task<List<TaskItem>> GetTasksByStatusAsync(TaskItemStatus status, CancellationToken cancellationToken = default);
    Task<List<TaskItem>> GetTasksByPriorityAsync(Priority priority, CancellationToken cancellationToken = default);
    Task<List<TaskItem>> GetTasksByTaskListAsync(Guid taskListId, CancellationToken cancellationToken = default);
    Task<List<TaskItem>> GetTasksBySessionAsync(Guid sessionId, CancellationToken cancellationToken = default);
    Task<List<TaskItem>> GetOverdueTasksAsync(CancellationToken cancellationToken = default);
    Task<List<TaskItem>> GetDueWithinAsync(DateTime dueDate, CancellationToken cancellationToken = default);
    Task<List<TaskItem>> GetMilestoneTasksAsync(CancellationToken cancellationToken = default);
    Task<List<TaskItem>> SearchTasksAsync(string searchTerm, CancellationToken cancellationToken = default);
    Task<List<TaskItem>> GetTasksByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);

    // Statistics
    Task<int> GetTotalCountAsync(CancellationToken cancellationToken = default);
    Task<Dictionary<TaskItemStatus, int>> GetStatusCountsAsync(CancellationToken cancellationToken = default);
    Task<Dictionary<Priority, int>> GetPriorityCountsAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Task list yönetimi servisi arayüzü — Birden fazla task listesi oluşturma,
/// düzenleme, arşivleme ve istatistik işlemleri.
/// </summary>
public interface ITaskListManager
{
    Task<TaskList> CreateTaskListAsync(string name, string description = "",
        string color = "#3B82F6", string icon = "list", CancellationToken cancellationToken = default);
    Task<TaskList?> GetTaskListAsync(Guid listId, CancellationToken cancellationToken = default);
    Task<TaskList?> GetTaskListWithTasksAsync(Guid listId, CancellationToken cancellationToken = default);
    Task<List<TaskList>> GetAllTaskListsAsync(CancellationToken cancellationToken = default);
    Task<List<TaskList>> GetActiveTaskListsAsync(CancellationToken cancellationToken = default);
    Task<TaskList> UpdateTaskListAsync(Guid listId, string? name = null, string? description = null,
        string? color = null, string? icon = null, Priority? defaultPriority = null,
        int? autoArchiveDays = null, CancellationToken cancellationToken = default);
    Task DeleteTaskListAsync(Guid listId, CancellationToken cancellationToken = default);
    Task ArchiveTaskListAsync(Guid listId, CancellationToken cancellationToken = default);
    Task UnarchiveTaskListAsync(Guid listId, CancellationToken cancellationToken = default);
    Task<List<TaskList>> SearchTaskListsAsync(string searchTerm, CancellationToken cancellationToken = default);
}

/// <summary>
/// Tag yönetimi servisi arayüzü — Etiket oluşturma, düzenleme ve gruplama.
/// </summary>
public interface ITagManager
{
    Task<TaskTag> CreateTagAsync(string name, string color = "#6B7280", CancellationToken cancellationToken = default);
    Task<TaskTag?> GetTagAsync(Guid tagId, CancellationToken cancellationToken = default);
    Task<List<TaskTag>> GetAllTagsAsync(CancellationToken cancellationToken = default);
    Task<TaskTag> UpdateTagAsync(Guid tagId, string? name = null, string? color = null, CancellationToken cancellationToken = default);
    Task DeleteTagAsync(Guid tagId, CancellationToken cancellationToken = default);
    Task<List<TaskItem>> GetTasksByTagAsync(string tagName, CancellationToken cancellationToken = default);
    Task<Dictionary<string, int>> GetTagUsageCountsAsync(CancellationToken cancellationToken = default);
}
