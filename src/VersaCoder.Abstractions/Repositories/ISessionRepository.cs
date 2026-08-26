using VersaCoder.Domain.Entities;
using VersaCoder.Domain.Interfaces;

namespace VersaCoder.Abstractions.Repositories;

public interface ISessionRepository : IRepository<Session>
{
    Task<List<Session>> GetByStateAsync(Domain.Enums.SessionState state, CancellationToken cancellationToken = default);
    Task<Session?> GetWithMessagesAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<Session>> GetByParentIdAsync(Guid? parentId, CancellationToken cancellationToken = default);
}
