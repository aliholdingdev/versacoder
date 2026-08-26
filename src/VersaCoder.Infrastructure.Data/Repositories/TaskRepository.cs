using Microsoft.EntityFrameworkCore;
using VersaCoder.Abstractions.Repositories;
using VersaCoder.Domain.Entities;
using VersaCoder.Domain.Enums;

namespace VersaCoder.Infrastructure.Data.Repositories;

/// <summary>
/// Task repository implementasyonu — EF Core ile SQLite WAL desteği.
/// Gelişmiş sorgular: durum, öncelik, tarih araması, bağımlılık kontrolü.
/// </summary>
public class TaskRepository : Repository<TaskItem>, ITaskRepository
{
    public TaskRepository(VersaCoderDbContext context) : base(context)
    {
    }

    public async Task<List<TaskItem>> GetByStatusAsync(TaskItemStatus status, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(t => t.Status == status)
            .OrderByDescending(t => t.UpdatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<TaskItem>> GetByPriorityAsync(Priority priority, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(t => t.Priority == priority)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<TaskItem>> GetByTaskListIdAsync(Guid? taskListId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(t => t.TaskListId == taskListId)
            .OrderBy(t => t.SortOrder)
            .ThenByDescending(t => t.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<TaskItem>> GetBySessionIdAsync(Guid? sessionId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(t => t.SessionId == sessionId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<TaskItem>> GetByParentTaskIdAsync(Guid? parentTaskId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(t => t.ParentTaskId == parentTaskId)
            .OrderBy(t => t.SortOrder)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<TaskItem>> GetSubTasksAsync(Guid parentTaskId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(t => t.ParentTaskId == parentTaskId)
            .OrderBy(t => t.SortOrder)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<TaskItem>> GetOverdueTasksAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        return await _dbSet
            .Where(t => t.DueDate.HasValue && t.DueDate.Value < now
                && t.Status != TaskItemStatus.COMPLETED && t.Status != TaskItemStatus.CANCELLED)
            .OrderBy(t => t.DueDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<TaskItem>> GetDueWithinAsync(DateTime dueDate, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(t => t.DueDate.HasValue && t.DueDate.Value <= dueDate && t.DueDate.Value >= DateTime.UtcNow
                && t.Status != TaskItemStatus.COMPLETED && t.Status != TaskItemStatus.CANCELLED)
            .OrderBy(t => t.DueDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<TaskItem>> GetByAssignedToAsync(string assignedTo, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(t => t.AssignedTo == assignedTo)
            .OrderByDescending(t => t.UpdatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<TaskItem>> GetByTagNameAsync(string tagName, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(t => t.Tags.Any(tag => tag.Name == tagName))
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<TaskItem>> GetBlockedTasksAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(t => t.Dependencies.Any(d =>
                d.DependencyType == DependencyType.FINISH_TO_START && d.DependsOnTask.Status != TaskItemStatus.COMPLETED ||
                d.DependencyType == DependencyType.START_TO_START && d.DependsOnTask.Status == TaskItemStatus.NEW ||
                d.DependencyType == DependencyType.FINISH_TO_FINISH && d.DependsOnTask.Status != TaskItemStatus.COMPLETED ||
                d.DependencyType == DependencyType.START_TO_FINISH && d.DependsOnTask.Status == TaskItemStatus.NEW))
            .OrderByDescending(t => t.Priority)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<TaskItem>> GetMilestoneTasksAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(t => t.IsMilestone)
            .OrderBy(t => t.DueDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<TaskItem>> GetWithDependenciesAsync(Guid taskId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(t => t.Dependencies)
                .ThenInclude(d => d.DependsOnTask)
            .Include(t => t.Dependents)
                .ThenInclude(d => d.Task)
            .Where(t => t.Id == taskId)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<TaskItem>> GetWithTagsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(t => t.Tags)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<TaskItem>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        var term = searchTerm.ToLower();
        return await _dbSet
            .Where(t => t.Title.ToLower().Contains(term)
                || t.Description.ToLower().Contains(term)
                || t.Notes.ToLower().Contains(term))
            .OrderByDescending(t => t.UpdatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<TaskItem>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(t => t.CreatedAt >= startDate && t.CreatedAt <= endDate)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<TaskItem?> GetWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(t => t.ParentTask)
            .Include(t => t.SubTasks)
            .Include(t => t.TaskList)
            .Include(t => t.Tags)
            .Include(t => t.Dependencies)
                .ThenInclude(d => d.DependsOnTask)
            .Include(t => t.Dependents)
                .ThenInclude(d => d.Task)
            .Include(t => t.Reminders)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public async Task<int> GetCountByStatusAsync(TaskItemStatus status, CancellationToken cancellationToken = default)
    {
        return await _dbSet.CountAsync(t => t.Status == status, cancellationToken);
    }

    public async Task<Dictionary<TaskItemStatus, int>> GetStatusCountsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .GroupBy(t => t.Status)
            .ToDictionaryAsync(g => g.Key, g => g.Count(), cancellationToken);
    }

    public async Task<Dictionary<Priority, int>> GetPriorityCountsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .GroupBy(t => t.Priority)
            .ToDictionaryAsync(g => g.Key, g => g.Count(), cancellationToken);
    }

    public async Task<int> GetTotalCountAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.CountAsync(cancellationToken);
    }
}
