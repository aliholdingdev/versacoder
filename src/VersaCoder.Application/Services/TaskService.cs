using VersaCoder.Abstractions.Repositories;
using VersaCoder.Abstractions.Services;
using VersaCoder.Domain.Entities;
using VersaCoder.Domain.Enums;
using VersaCoder.Domain.Interfaces;

namespace VersaCoder.Application.Services;

/// <summary>
/// Task yönetimi servisi — ITaskManager ve ITaskListManager arayüzlerini implemente eder.
/// Task CRUD, durum makinesi, alt görev, bağımlılık, etiket, hatırlatıcı yönetimi.
/// </summary>
public class TaskService : ITaskManager, ITaskListManager, ITagManager
{
    private readonly ITaskRepository _taskRepository;
    private readonly ITaskListRepository _taskListRepository;
    private readonly ILogService _logService;

    public TaskService(
        ITaskRepository taskRepository,
        ITaskListRepository taskListRepository,
        ILogService logService)
    {
        _taskRepository = taskRepository;
        _taskListRepository = taskListRepository;
        _logService = logService;
    }

    #region Task CRUD Operations

    public async Task<TaskItem> CreateTaskAsync(string title, Priority priority = Priority.MEDIUM,
        Guid? taskListId = null, Guid? sessionId = null, CancellationToken cancellationToken = default)
    {
        var task = new TaskItem(title, priority, taskListId, sessionId);
        await _taskRepository.AddAsync(task, cancellationToken);
        await _taskRepository.SaveChangesAsync(cancellationToken);

        await _logService.LogWithContextAsync(
            AuditLogLevel.INFO, "task-manager", "TASK_CREATED",
            $"Task oluşturuldu: {title}",
            taskId: task.Id, cancellationToken: cancellationToken);

        return task;
    }

    public async Task<TaskItem?> GetTaskAsync(Guid taskId, CancellationToken cancellationToken = default)
    {
        return await _taskRepository.GetByIdAsync(taskId, cancellationToken);
    }

    public async Task<TaskItem?> GetTaskWithDetailsAsync(Guid taskId, CancellationToken cancellationToken = default)
    {
        return await _taskRepository.GetWithDetailsAsync(taskId, cancellationToken);
    }

    public async Task<List<TaskItem>> GetAllTasksAsync(CancellationToken cancellationToken = default)
    {
        return await _taskRepository.GetAllAsync(cancellationToken);
    }

    public async Task<TaskItem> UpdateTaskAsync(Guid taskId, string? title = null, string? description = null,
        Priority? priority = null, CancellationToken cancellationToken = default)
    {
        var task = await _taskRepository.GetByIdAsync(taskId, cancellationToken)
            ?? throw new InvalidOperationException($"Task {taskId} not found.");

        if (title != null) task.UpdateTitle(title);
        if (description != null) task.UpdateDescription(description);
        if (priority.HasValue) task.Priority = priority.Value;

        await _taskRepository.UpdateAsync(task, cancellationToken);
        await _taskRepository.SaveChangesAsync(cancellationToken);

        return task;
    }

    public async Task DeleteTaskAsync(Guid taskId, CancellationToken cancellationToken = default)
    {
        await _taskRepository.DeleteAsync(taskId, cancellationToken);
        await _taskRepository.SaveChangesAsync(cancellationToken);
    }

    #endregion

    #region Status Management (State Machine)

    public async Task StartTaskAsync(Guid taskId, CancellationToken cancellationToken = default)
    {
        var task = await _taskRepository.GetByIdAsync(taskId, cancellationToken)
            ?? throw new InvalidOperationException($"Task {taskId} not found.");

        task.Start();
        await _taskRepository.UpdateAsync(task, cancellationToken);
        await _taskRepository.SaveChangesAsync(cancellationToken);

        await _logService.LogWithContextAsync(
            AuditLogLevel.INFO, "task-manager", "TASK_STARTED",
            $"Task başlatıldı: {task.Title}",
            taskId: task.Id, cancellationToken: cancellationToken);
    }

    public async Task CompleteTaskAsync(Guid taskId, CancellationToken cancellationToken = default)
    {
        var task = await _taskRepository.GetByIdAsync(taskId, cancellationToken)
            ?? throw new InvalidOperationException($"Task {taskId} not found.");

        task.Complete();
        await _taskRepository.UpdateAsync(task, cancellationToken);
        await _taskRepository.SaveChangesAsync(cancellationToken);

        await _logService.LogWithContextAsync(
            AuditLogLevel.INFO, "task-manager", "TASK_COMPLETED",
            $"Task tamamlandı: {task.Title}",
            taskId: task.Id, cancellationToken: cancellationToken);
    }

    public async Task HoldTaskAsync(Guid taskId, CancellationToken cancellationToken = default)
    {
        var task = await _taskRepository.GetByIdAsync(taskId, cancellationToken)
            ?? throw new InvalidOperationException($"Task {taskId} not found.");

        task.Hold();
        await _taskRepository.UpdateAsync(task, cancellationToken);
        await _taskRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task CancelTaskAsync(Guid taskId, CancellationToken cancellationToken = default)
    {
        var task = await _taskRepository.GetByIdAsync(taskId, cancellationToken)
            ?? throw new InvalidOperationException($"Task {taskId} not found.");

        task.Cancel();
        await _taskRepository.UpdateAsync(task, cancellationToken);
        await _taskRepository.SaveChangesAsync(cancellationToken);

        await _logService.LogWithContextAsync(
            AuditLogLevel.WARN, "task-manager", "TASK_CANCELLED",
            $"Task iptal edildi: {task.Title}",
            taskId: task.Id, cancellationToken: cancellationToken);
    }

    public async Task FailTaskAsync(Guid taskId, CancellationToken cancellationToken = default)
    {
        var task = await _taskRepository.GetByIdAsync(taskId, cancellationToken)
            ?? throw new InvalidOperationException($"Task {taskId} not found.");

        task.Fail();
        await _taskRepository.UpdateAsync(task, cancellationToken);
        await _taskRepository.SaveChangesAsync(cancellationToken);

        await _logService.LogWithContextAsync(
            AuditLogLevel.ERROR, "task-manager", "TASK_FAILED",
            $"Task başarısız oldu: {task.Title}",
            taskId: task.Id, cancellationToken: cancellationToken);
    }

    public async Task SendToReviewAsync(Guid taskId, CancellationToken cancellationToken = default)
    {
        var task = await _taskRepository.GetByIdAsync(taskId, cancellationToken)
            ?? throw new InvalidOperationException($"Task {taskId} not found.");

        task.SendToReview();
        await _taskRepository.UpdateAsync(task, cancellationToken);
        await _taskRepository.SaveChangesAsync(cancellationToken);
    }

    #endregion

    #region Subtask Operations

    public async Task<TaskItem> CreateSubTaskAsync(Guid parentTaskId, string title,
        Priority priority = Priority.MEDIUM, CancellationToken cancellationToken = default)
    {
        var parent = await _taskRepository.GetByIdAsync(parentTaskId, cancellationToken)
            ?? throw new InvalidOperationException($"Parent task {parentTaskId} not found.");

        var subTask = new TaskItem(title, priority, parent.TaskListId, parent.SessionId)
        {
            ParentTaskId = parentTaskId
        };

        await _taskRepository.AddAsync(subTask, cancellationToken);
        await _taskRepository.SaveChangesAsync(cancellationToken);

        return subTask;
    }

    public async Task<List<TaskItem>> GetSubTasksAsync(Guid parentTaskId, CancellationToken cancellationToken = default)
    {
        return await _taskRepository.GetSubTasksAsync(parentTaskId, cancellationToken);
    }

    public async Task<List<TaskItem>> GetTaskHierarchyAsync(Guid taskId, CancellationToken cancellationToken = default)
    {
        var task = await _taskRepository.GetWithDetailsAsync(taskId, cancellationToken)
            ?? throw new InvalidOperationException($"Task {taskId} not found.");

        var result = new List<TaskItem> { task };
        result.AddRange(task.SubTasks);
        return result;
    }

    #endregion

    #region Dependency Operations

    public async Task AddDependencyAsync(Guid taskId, Guid dependsOnTaskId,
        DependencyType dependencyType = DependencyType.FINISH_TO_START, CancellationToken cancellationToken = default)
    {
        if (taskId == dependsOnTaskId)
            throw new ArgumentException("A task cannot depend on itself.");

        var task = await _taskRepository.GetByIdAsync(taskId, cancellationToken)
            ?? throw new InvalidOperationException($"Task {taskId} not found.");

        var dependsOn = await _taskRepository.GetByIdAsync(dependsOnTaskId, cancellationToken)
            ?? throw new InvalidOperationException($"Dependency task {dependsOnTaskId} not found.");

        var dependency = new TaskDependency(taskId, dependsOnTaskId, dependencyType);
        task.Dependencies.Add(dependency);

        await _taskRepository.UpdateAsync(task, cancellationToken);
        await _taskRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveDependencyAsync(Guid taskId, Guid dependsOnTaskId, CancellationToken cancellationToken = default)
    {
        var task = await _taskRepository.GetWithDetailsAsync(taskId, cancellationToken)
            ?? throw new InvalidOperationException($"Task {taskId} not found.");

        var dependency = task.Dependencies.FirstOrDefault(d => d.DependsOnTaskId == dependsOnTaskId)
            ?? throw new InvalidOperationException($"Dependency not found.");

        task.Dependencies.Remove(dependency);
        await _taskRepository.UpdateAsync(task, cancellationToken);
        await _taskRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<TaskDependency>> GetDependenciesAsync(Guid taskId, CancellationToken cancellationToken = default)
    {
        var task = await _taskRepository.GetWithDetailsAsync(taskId, cancellationToken);
        return task?.Dependencies ?? new List<TaskDependency>();
    }

    public async Task<bool> AreDependenciesMetAsync(Guid taskId, CancellationToken cancellationToken = default)
    {
        var task = await _taskRepository.GetWithDetailsAsync(taskId, cancellationToken);
        return task?.AreDependenciesMet() ?? true;
    }

    public async Task<List<TaskItem>> GetBlockedTasksAsync(CancellationToken cancellationToken = default)
    {
        return await _taskRepository.GetBlockedTasksAsync(cancellationToken);
    }

    #endregion

    #region Tag Operations

    public async Task<TaskTag> CreateTagAsync(string name, string color = "#6B7280", CancellationToken cancellationToken = default)
    {
        var tag = new TaskTag(name, color);
        await _taskRepository.AddAsync(tag as TaskItem ?? throw new InvalidOperationException("Cannot add tag directly"), cancellationToken);
        await _taskRepository.SaveChangesAsync(cancellationToken);
        return tag;
    }

    public async Task AddTagToTaskAsync(Guid taskId, Guid tagId, CancellationToken cancellationToken = default)
    {
        var task = await _taskRepository.GetWithDetailsAsync(taskId, cancellationToken)
            ?? throw new InvalidOperationException($"Task {taskId} not found.");

        // Tag varsa ekle (pratikte ayrı bir TagRepository kullanılır)
        await _taskRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveTagFromTaskAsync(Guid taskId, Guid tagId, CancellationToken cancellationToken = default)
    {
        var task = await _taskRepository.GetWithDetailsAsync(taskId, cancellationToken)
            ?? throw new InvalidOperationException($"Task {taskId} not found.");

        await _taskRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<TaskTag>> GetTagsForTaskAsync(Guid taskId, CancellationToken cancellationToken = default)
    {
        var task = await _taskRepository.GetWithDetailsAsync(taskId, cancellationToken);
        return task?.Tags ?? new List<TaskTag>();
    }

    public async Task<List<TaskItem>> GetTasksByTagAsync(string tagName, CancellationToken cancellationToken = default)
    {
        return await _taskRepository.GetByTagNameAsync(tagName, cancellationToken);
    }

    #endregion

    #region Reminder Operations

    public async Task<TaskReminder> SetReminderAsync(Guid taskId, DateTime reminderDate,
        string? message = null, CancellationToken cancellationToken = default)
    {
        var task = await _taskRepository.GetByIdAsync(taskId, cancellationToken)
            ?? throw new InvalidOperationException($"Task {taskId} not found.");

        var reminder = new TaskReminder(taskId, reminderDate, message);
        task.Reminders.Add(reminder);

        await _taskRepository.UpdateAsync(task, cancellationToken);
        await _taskRepository.SaveChangesAsync(cancellationToken);

        return reminder;
    }

    public async Task<List<TaskReminder>> GetRemindersForTaskAsync(Guid taskId, CancellationToken cancellationToken = default)
    {
        var task = await _taskRepository.GetWithDetailsAsync(taskId, cancellationToken);
        return task?.Reminders ?? new List<TaskReminder>();
    }

    public async Task<List<TaskReminder>> GetDueRemindersAsync(CancellationToken cancellationToken = default)
    {
        var allTasks = await _taskRepository.GetAllAsync(cancellationToken);
        return allTasks
            .SelectMany(t => t.Reminders)
            .Where(r => r.IsDue())
            .OrderBy(r => r.ReminderDate)
            .ToList();
    }

    #endregion

    #region Notes & Duration & DueDate

    public async Task UpdateNotesAsync(Guid taskId, string notes, CancellationToken cancellationToken = default)
    {
        var task = await _taskRepository.GetByIdAsync(taskId, cancellationToken)
            ?? throw new InvalidOperationException($"Task {taskId} not found.");

        task.UpdateNotes(notes);
        await _taskRepository.UpdateAsync(task, cancellationToken);
        await _taskRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task SetEstimatedDurationAsync(Guid taskId, decimal hours, DurationType type,
        CancellationToken cancellationToken = default)
    {
        var task = await _taskRepository.GetByIdAsync(taskId, cancellationToken)
            ?? throw new InvalidOperationException($"Task {taskId} not found.");

        task.SetEstimatedDuration(hours, type);
        await _taskRepository.UpdateAsync(task, cancellationToken);
        await _taskRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task SetDueDateAsync(Guid taskId, DateTime dueDate, CancellationToken cancellationToken = default)
    {
        var task = await _taskRepository.GetByIdAsync(taskId, cancellationToken)
            ?? throw new InvalidOperationException($"Task {taskId} not found.");

        task.SetDueDate(dueDate);
        await _taskRepository.UpdateAsync(task, cancellationToken);
        await _taskRepository.SaveChangesAsync(cancellationToken);
    }

    #endregion

    #region Query Operations

    public async Task<List<TaskItem>> GetTasksByStatusAsync(TaskItemStatus status, CancellationToken cancellationToken = default)
    {
        return await _taskRepository.GetByStatusAsync(status, cancellationToken);
    }

    public async Task<List<TaskItem>> GetTasksByPriorityAsync(Priority priority, CancellationToken cancellationToken = default)
    {
        return await _taskRepository.GetByPriorityAsync(priority, cancellationToken);
    }

    public async Task<List<TaskItem>> GetTasksByTaskListAsync(Guid taskListId, CancellationToken cancellationToken = default)
    {
        return await _taskRepository.GetByTaskListIdAsync(taskListId, cancellationToken);
    }

    public async Task<List<TaskItem>> GetTasksBySessionAsync(Guid sessionId, CancellationToken cancellationToken = default)
    {
        return await _taskRepository.GetBySessionIdAsync(sessionId, cancellationToken);
    }

    public async Task<List<TaskItem>> GetOverdueTasksAsync(CancellationToken cancellationToken = default)
    {
        return await _taskRepository.GetOverdueTasksAsync(cancellationToken);
    }

    public async Task<List<TaskItem>> GetDueWithinAsync(DateTime dueDate, CancellationToken cancellationToken = default)
    {
        return await _taskRepository.GetDueWithinAsync(dueDate, cancellationToken);
    }

    public async Task<List<TaskItem>> GetMilestoneTasksAsync(CancellationToken cancellationToken = default)
    {
        return await _taskRepository.GetMilestoneTasksAsync(cancellationToken);
    }

    public async Task<List<TaskItem>> SearchTasksAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        return await _taskRepository.SearchAsync(searchTerm, cancellationToken);
    }

    public async Task<List<TaskItem>> GetTasksByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        return await _taskRepository.GetByDateRangeAsync(startDate, endDate, cancellationToken);
    }

    #endregion

    #region Statistics

    public async Task<int> GetTotalCountAsync(CancellationToken cancellationToken = default)
    {
        return await _taskRepository.GetTotalCountAsync(cancellationToken);
    }

    public async Task<Dictionary<TaskItemStatus, int>> GetStatusCountsAsync(CancellationToken cancellationToken = default)
    {
        return await _taskRepository.GetStatusCountsAsync(cancellationToken);
    }

    public async Task<Dictionary<Priority, int>> GetPriorityCountsAsync(CancellationToken cancellationToken = default)
    {
        return await _taskRepository.GetPriorityCountsAsync(cancellationToken);
    }

    #endregion

    #region ITaskListManager Implementation

    public async Task<TaskList> CreateTaskListAsync(string name, string description = "",
        string color = "#3B82F6", string icon = "list", CancellationToken cancellationToken = default)
    {
        var taskList = new TaskList(name, description)
        {
            Color = color,
            Icon = icon
        };

        await _taskListRepository.AddAsync(taskList, cancellationToken);
        await _taskListRepository.SaveChangesAsync(cancellationToken);

        await _logService.LogWithContextAsync(
            AuditLogLevel.INFO, "task-manager", "TASK_LIST_CREATED",
            $"Task listesi oluşturuldu: {name}",
            cancellationToken: cancellationToken);

        return taskList;
    }

    public async Task<TaskList?> GetTaskListAsync(Guid listId, CancellationToken cancellationToken = default)
    {
        return await _taskListRepository.GetByIdAsync(listId, cancellationToken);
    }

    public async Task<TaskList?> GetTaskListWithTasksAsync(Guid listId, CancellationToken cancellationToken = default)
    {
        return await _taskListRepository.GetWithTasksAsync(listId, cancellationToken);
    }

    public async Task<List<TaskList>> GetAllTaskListsAsync(CancellationToken cancellationToken = default)
    {
        return await _taskListRepository.GetAllAsync(cancellationToken);
    }

    public async Task<List<TaskList>> GetActiveTaskListsAsync(CancellationToken cancellationToken = default)
    {
        return await _taskListRepository.GetActiveListsAsync(cancellationToken);
    }

    public async Task<TaskList> UpdateTaskListAsync(Guid listId, string? name = null, string? description = null,
        string? color = null, string? icon = null, Priority? defaultPriority = null,
        int? autoArchiveDays = null, CancellationToken cancellationToken = default)
    {
        var taskList = await _taskListRepository.GetByIdAsync(listId, cancellationToken)
            ?? throw new InvalidOperationException($"Task list {listId} not found.");

        if (name != null) taskList.UpdateName(name);
        if (description != null) taskList.UpdateDescription(description);
        if (color != null) taskList.UpdateColor(color);
        if (icon != null) taskList.UpdateIcon(icon);
        if (defaultPriority.HasValue) taskList.DefaultPriority = defaultPriority.Value;
        if (autoArchiveDays.HasValue) taskList.AutoArchiveDays = autoArchiveDays.Value;

        await _taskListRepository.UpdateAsync(taskList, cancellationToken);
        await _taskListRepository.SaveChangesAsync(cancellationToken);

        return taskList;
    }

    public async Task DeleteTaskListAsync(Guid listId, CancellationToken cancellationToken = default)
    {
        await _taskListRepository.DeleteAsync(listId, cancellationToken);
        await _taskListRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task ArchiveTaskListAsync(Guid listId, CancellationToken cancellationToken = default)
    {
        var taskList = await _taskListRepository.GetByIdAsync(listId, cancellationToken)
            ?? throw new InvalidOperationException($"Task list {listId} not found.");

        taskList.Archive();
        await _taskListRepository.UpdateAsync(taskList, cancellationToken);
        await _taskListRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task UnarchiveTaskListAsync(Guid listId, CancellationToken cancellationToken = default)
    {
        var taskList = await _taskListRepository.GetByIdAsync(listId, cancellationToken)
            ?? throw new InvalidOperationException($"Task list {listId} not found.");

        taskList.Unarchive();
        await _taskListRepository.UpdateAsync(taskList, cancellationToken);
        await _taskListRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<TaskList>> SearchTaskListsAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        return await _taskListRepository.SearchAsync(searchTerm, cancellationToken);
    }

    #endregion

    #region ITagManager Implementation

    // Note: In production, use a dedicated ITagRepository
    // This is a simplified implementation

    public async Task<TaskTag?> GetTagAsync(Guid tagId, CancellationToken cancellationToken = default)
    {
        // Simplified - in production use dedicated tag repository
        return null;
    }

    public async Task<List<TaskTag>> GetAllTagsAsync(CancellationToken cancellationToken = default)
    {
        return new List<TaskTag>();
    }

    public async Task<TaskTag> UpdateTagAsync(Guid tagId, string? name = null, string? color = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("Use dedicated tag repository in production.");
    }

    public async Task DeleteTagAsync(Guid tagId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("Use dedicated tag repository in production.");
    }

    public async Task<Dictionary<string, int>> GetTagUsageCountsAsync(CancellationToken cancellationToken = default)
    {
        return new Dictionary<string, int>();
    }

    #endregion
}
