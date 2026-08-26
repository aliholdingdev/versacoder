using VersaCoder.Abstractions.Repositories;
using VersaCoder.Abstractions.Services;
using VersaCoder.Domain.Entities;
using VersaCoder.Domain.Enums;

namespace VersaCoder.Application.Services;

public class SessionManagerService : ISessionManager
{
    private readonly ISessionRepository _sessionRepository;
    private readonly IMessageRepository _messageRepository;

    public SessionManagerService(
        ISessionRepository sessionRepository,
        IMessageRepository messageRepository)
    {
        _sessionRepository = sessionRepository;
        _messageRepository = messageRepository;
    }

    public async Task<Session> CreateSessionAsync(string name, CancellationToken cancellationToken = default)
    {
        var session = new Session(name);
        await _sessionRepository.AddAsync(session, cancellationToken);
        await _sessionRepository.SaveChangesAsync(cancellationToken);
        return session;
    }

    public async Task<Session?> GetSessionAsync(Guid sessionId, CancellationToken cancellationToken = default)
    {
        return await _sessionRepository.GetByIdAsync(sessionId, cancellationToken);
    }

    public async Task<List<Session>> GetAllSessionsAsync(CancellationToken cancellationToken = default)
    {
        return await _sessionRepository.GetAllAsync(cancellationToken);
    }

    public async Task<Session> BranchSessionAsync(Guid sourceSessionId, string branchName, string reason, CancellationToken cancellationToken = default)
    {
        var source = await _sessionRepository.GetByIdAsync(sourceSessionId, cancellationToken)
            ?? throw new InvalidOperationException($"Session {sourceSessionId} not found");

        var branch = new Session($"{source.Name} - {branchName}")
        {
            ParentId = sourceSessionId,
            State = SessionState.ACTIVE
        };

        await _sessionRepository.AddAsync(branch, cancellationToken);
        source.State = SessionState.BRANCHED;
        await _sessionRepository.UpdateAsync(source, cancellationToken);
        await _sessionRepository.SaveChangesAsync(cancellationToken);

        return branch;
    }

    public async Task<Session> ForkSessionAsync(Guid sourceSessionId, string newName, CancellationToken cancellationToken = default)
    {
        var source = await _sessionRepository.GetByIdAsync(sourceSessionId, cancellationToken)
            ?? throw new InvalidOperationException($"Session {sourceSessionId} not found");

        var fork = new Session(newName)
        {
            ParentId = sourceSessionId,
            State = SessionState.ACTIVE
        };

        await _sessionRepository.AddAsync(fork, cancellationToken);
        await _sessionRepository.SaveChangesAsync(cancellationToken);

        return fork;
    }

    public async Task<Session> MergeSessionsAsync(Guid sourceSessionId, Guid targetSessionId, string mergeStrategy, CancellationToken cancellationToken = default)
    {
        var source = await _sessionRepository.GetByIdAsync(sourceSessionId, cancellationToken)
            ?? throw new InvalidOperationException($"Source session {sourceSessionId} not found");

        var target = await _sessionRepository.GetByIdAsync(targetSessionId, cancellationToken)
            ?? throw new InvalidOperationException($"Target session {targetSessionId} not found");

        target.State = SessionState.ACTIVE;
        await _sessionRepository.UpdateAsync(target, cancellationToken);
        source.State = SessionState.COMPLETED;
        await _sessionRepository.UpdateAsync(source, cancellationToken);
        await _sessionRepository.SaveChangesAsync(cancellationToken);

        return target;
    }

    public async Task CompleteSessionAsync(Guid sessionId, CancellationToken cancellationToken = default)
    {
        var session = await _sessionRepository.GetByIdAsync(sessionId, cancellationToken)
            ?? throw new InvalidOperationException($"Session {sessionId} not found");

        session.Complete();
        await _sessionRepository.UpdateAsync(session, cancellationToken);
        await _sessionRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task PauseSessionAsync(Guid sessionId, CancellationToken cancellationToken = default)
    {
        var session = await _sessionRepository.GetByIdAsync(sessionId, cancellationToken)
            ?? throw new InvalidOperationException($"Session {sessionId} not found");

        session.State = SessionState.PAUSED;
        await _sessionRepository.UpdateAsync(session, cancellationToken);
        await _sessionRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task ResumeSessionAsync(Guid sessionId, CancellationToken cancellationToken = default)
    {
        var session = await _sessionRepository.GetByIdAsync(sessionId, cancellationToken)
            ?? throw new InvalidOperationException($"Session {sessionId} not found");

        session.State = SessionState.ACTIVE;
        await _sessionRepository.UpdateAsync(session, cancellationToken);
        await _sessionRepository.SaveChangesAsync(cancellationToken);
    }
}
