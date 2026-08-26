using VersaCoder.Domain.Entities;
using VersaCoder.Domain.Interfaces;

namespace VersaCoder.Abstractions.Repositories;

/// <summary>
/// TaskList repository arayüzü — IRepository&lt;TaskList&gt; üzerine inşa edilmiş
/// task listesi-specific sorguları tanımlar.
/// </summary>
public interface ITaskListRepository : IRepository<TaskList>
{
    Task<List<TaskList>> GetActiveListsAsync(CancellationToken cancellationToken = default);
    Task<List<TaskList>> GetArchivedListsAsync(CancellationToken cancellationToken = default);
    Task<List<TaskList>> GetWithTaskCountsAsync(CancellationToken cancellationToken = default);
    Task<TaskList?> GetWithTasksAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<TaskList>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
}
