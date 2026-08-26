using VersaCoder.Domain.Entities;
using VersaCoder.Domain.Enums;
using VersaCoder.Domain.Interfaces;

namespace VersaCoder.Abstractions.Repositories;

/// <summary>
/// Task repository arayüzü — IRepository&lt;TaskItem&gt; üzerine inşa edilmiş
/// task'a özgü sorguları tanımlar.
/// </summary>
public interface ITaskRepository : IRepository<TaskItem>
{
    Task<List<TaskItem>> GetByStatusAsync(TaskItemStatus status, CancellationToken cancellationToken = default);
    Task<List<TaskItem>> GetByPriorityAsync(Priority priority, CancellationToken cancellationToken = default);
    Task<List<TaskItem>> GetByTaskListIdAsync(Guid? taskListId, CancellationToken cancellationToken = default);
    Task<List<TaskItem>> GetBySessionIdAsync(Guid? sessionId, CancellationToken cancellationToken = default);
    Task<List<TaskItem>> GetByParentTaskIdAsync(Guid? parentTaskId, CancellationToken cancellationToken = default);
    Task<List<TaskItem>> GetSubTasksAsync(Guid parentTaskId, CancellationToken cancellationToken = default);
    Task<List<TaskItem>> GetOverdueTasksAsync(CancellationToken cancellationToken = default);
    Task<List<TaskItem>> GetDueWithinAsync(DateTime dueDate, CancellationToken cancellationToken = default);
    Task<List<TaskItem>> GetByAssignedToAsync(string assignedTo, CancellationToken cancellationToken = default);
    Task<List<TaskItem>> GetByTagNameAsync(string tagName, CancellationToken cancellationToken = default);
    Task<List<TaskItem>> GetBlockedTasksAsync(CancellationToken cancellationToken = default);
    Task<List<TaskItem>> GetMilestoneTasksAsync(CancellationToken cancellationToken = default);
    Task<List<TaskItem>> GetWithDependenciesAsync(Guid taskId, CancellationToken cancellationToken = default);
    Task<List<TaskItem>> GetWithTagsAsync(CancellationToken cancellationToken = default);
    Task<List<TaskItem>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
    Task<List<TaskItem>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    Task<TaskItem?> GetWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<int> GetCountByStatusAsync(TaskItemStatus status, CancellationToken cancellationToken = default);
    Task<Dictionary<TaskItemStatus, int>> GetStatusCountsAsync(CancellationToken cancellationToken = default);
    Task<Dictionary<Priority, int>> GetPriorityCountsAsync(CancellationToken cancellationToken = default);
    Task<int> GetTotalCountAsync(CancellationToken cancellationToken = default);
}
