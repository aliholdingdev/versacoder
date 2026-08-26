using VersaCoder.Domain.Entities;
using VersaCoder.Domain.Interfaces;

namespace VersaCoder.Abstractions.Repositories;

public interface IMessageRepository : IRepository<Message>
{
    Task<List<Message>> GetBySessionIdAsync(Guid sessionId, CancellationToken cancellationToken = default);
    Task<List<Message>> GetBySessionIdPagedAsync(Guid sessionId, int page, int pageSize, CancellationToken cancellationToken = default);
}
