using Microsoft.EntityFrameworkCore;
using VersaCoder.Abstractions.Repositories;
using VersaCoder.Domain.Entities;

namespace VersaCoder.Infrastructure.Data.Repositories;

public class MessageRepository : Repository<Message>, IMessageRepository
{
    public MessageRepository(VersaCoderDbContext context) : base(context)
    {
    }

    public async Task<List<Message>> GetBySessionIdAsync(Guid sessionId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(m => m.SessionId == sessionId)
            .OrderBy(m => m.Timestamp)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Message>> GetBySessionIdPagedAsync(Guid sessionId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(m => m.SessionId == sessionId)
            .OrderBy(m => m.Timestamp)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }
}
