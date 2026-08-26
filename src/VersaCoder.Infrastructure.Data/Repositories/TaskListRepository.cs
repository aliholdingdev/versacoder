using Microsoft.EntityFrameworkCore;
using VersaCoder.Abstractions.Repositories;
using VersaCoder.Domain.Entities;

namespace VersaCoder.Infrastructure.Data.Repositories;

/// <summary>
/// TaskList repository implementasyonu — EF Core ile SQLite WAL desteği.
/// </summary>
public class TaskListRepository : Repository<TaskList>, ITaskListRepository
{
    public TaskListRepository(VersaCoderDbContext context) : base(context)
    {
    }

    public async Task<List<TaskList>> GetActiveListsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(tl => !tl.IsArchived)
            .OrderBy(tl => tl.SortOrder)
            .ThenBy(tl => tl.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<TaskList>> GetArchivedListsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(tl => tl.IsArchived)
            .OrderByDescending(tl => tl.UpdatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<TaskList>> GetWithTaskCountsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(tl => tl.Tasks)
            .OrderBy(tl => tl.SortOrder)
            .ToListAsync(cancellationToken);
    }

    public async Task<TaskList?> GetWithTasksAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(tl => tl.Tasks)
                .ThenInclude(t => t.Tags)
            .FirstOrDefaultAsync(tl => tl.Id == id, cancellationToken);
    }

    public async Task<List<TaskList>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        var term = searchTerm.ToLower();
        return await _dbSet
            .Where(tl => tl.Name.ToLower().Contains(term)
                || tl.Description.ToLower().Contains(term))
            .OrderBy(tl => tl.Name)
            .ToListAsync(cancellationToken);
    }
}
