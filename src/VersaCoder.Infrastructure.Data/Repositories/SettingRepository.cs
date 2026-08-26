using Microsoft.EntityFrameworkCore;
using VersaCoder.Abstractions.Repositories;
using VersaCoder.Domain.Entities;

namespace VersaCoder.Infrastructure.Data.Repositories;

public class SettingRepository : Repository<Setting>, ISettingRepository
{
    public SettingRepository(VersaCoderDbContext context) : base(context)
    {
    }

    public async Task<Setting?> GetByKeyAsync(string key, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(s => s.Key == key, cancellationToken);
    }

    public async Task<List<Setting>> GetByCategoryAsync(string category, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(s => s.Category == category)
            .ToListAsync(cancellationToken);
    }

    public async Task<Setting?> GetByKeyOrDefaultAsync(string key, string defaultValue, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(s => s.Key == key, cancellationToken);
    }
}
