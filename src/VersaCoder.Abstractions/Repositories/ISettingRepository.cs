using VersaCoder.Domain.Entities;
using VersaCoder.Domain.Interfaces;

namespace VersaCoder.Abstractions.Repositories;

public interface ISettingRepository : IRepository<Setting>
{
    Task<Setting?> GetByKeyAsync(string key, CancellationToken cancellationToken = default);
    Task<List<Setting>> GetByCategoryAsync(string category, CancellationToken cancellationToken = default);
    Task<Setting?> GetByKeyOrDefaultAsync(string key, string defaultValue, CancellationToken cancellationToken = default);
}
