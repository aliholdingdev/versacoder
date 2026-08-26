using VersaCoder.Domain.Entities;

namespace VersaCoder.Abstractions.Services;

public interface ISessionManager
{
    Task<Session> CreateSessionAsync(string name, CancellationToken cancellationToken = default);
    Task<Session?> GetSessionAsync(Guid sessionId, CancellationToken cancellationToken = default);
    Task<List<Session>> GetAllSessionsAsync(CancellationToken cancellationToken = default);
    Task<Session> BranchSessionAsync(Guid sourceSessionId, string branchName, string reason, CancellationToken cancellationToken = default);
    Task<Session> ForkSessionAsync(Guid sourceSessionId, string newName, CancellationToken cancellationToken = default);
    Task<Session> MergeSessionsAsync(Guid sourceSessionId, Guid targetSessionId, string mergeStrategy, CancellationToken cancellationToken = default);
    Task CompleteSessionAsync(Guid sessionId, CancellationToken cancellationToken = default);
    Task PauseSessionAsync(Guid sessionId, CancellationToken cancellationToken = default);
    Task ResumeSessionAsync(Guid sessionId, CancellationToken cancellationToken = default);
}
