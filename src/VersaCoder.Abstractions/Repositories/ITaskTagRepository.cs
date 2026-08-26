using VersaCoder.Domain.Entities;
using VersaCoder.Domain.Interfaces;

namespace VersaCoder.Abstractions.Repositories;

/// <summary>
/// TaskTag repository arayüzü — IRepository&lt;TaskTag&gt; üzerine inşa edilmiş
/// tag'a özgü sorguları tanımlar.
/// </summary>
public interface ITaskTagRepository : IRepository<TaskTag>
{
    Task<TaskTag?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<List<TaskTag>> GetWithTaskCountsAsync(CancellationToken cancellationToken = default);
    Task<List<TaskTag>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
    Task<Dictionary<string, int>> GetUsageCountsAsync(CancellationToken cancellationToken = default);
}
