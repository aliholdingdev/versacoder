using Microsoft.EntityFrameworkCore;
using VersaCoder.Abstractions.Repositories;
using VersaCoder.Domain.Entities;
using VersaCoder.Domain.Enums;

namespace VersaCoder.Infrastructure.Data.Repositories;

public class SessionRepository : Repository<Session>, ISessionRepository
{
    public SessionRepository(VersaCoderDbContext context) : base(context)
    {
    }

    public async Task<List<Session>> GetByStateAsync(SessionState state, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(s => s.State == state)
            .OrderByDescending(s => s.UpdatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<Session?> GetWithMessagesAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(s => s.Messages)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<List<Session>> GetByParentIdAsync(Guid? parentId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(s => s.ParentId == parentId)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}
