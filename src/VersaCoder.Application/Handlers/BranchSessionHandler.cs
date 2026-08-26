using MediatR;
using VersaCoder.Abstractions.Repositories;
using VersaCoder.Application.Commands;
using VersaCoder.Application.Common;
using VersaCoder.Application.DTOs;
using VersaCoder.Domain.Enums;

namespace VersaCoder.Application.Handlers;

public class BranchSessionHandler : IRequestHandler<BranchSessionCommand, Result<SessionDto>>
{
    private readonly ISessionRepository _sessionRepository;

    public BranchSessionHandler(ISessionRepository sessionRepository)
    {
        _sessionRepository = sessionRepository;
    }

    public async Task<Result<SessionDto>> Handle(BranchSessionCommand request, CancellationToken cancellationToken)
    {
        var sourceSession = await _sessionRepository.GetByIdAsync(request.SourceSessionId, cancellationToken);
        if (sourceSession == null)
            return Result<SessionDto>.Failure($"Session with id {request.SourceSessionId} not found");

        var branchSession = new Domain.Entities.Session($"{sourceSession.Name} - {request.BranchName}")
        {
            ParentId = request.SourceSessionId,
            State = SessionState.ACTIVE
        };

        await _sessionRepository.AddAsync(branchSession, cancellationToken);
        sourceSession.State = SessionState.BRANCHED;
        await _sessionRepository.UpdateAsync(sourceSession, cancellationToken);
        await _sessionRepository.SaveChangesAsync(cancellationToken);

        var dto = new SessionDto
        {
            Id = branchSession.Id,
            Name = branchSession.Name,
            State = branchSession.State.ToString(),
            ParentId = branchSession.ParentId,
            CreatedAt = branchSession.CreatedAt,
            UpdatedAt = branchSession.UpdatedAt
        };

        return Result<SessionDto>.Success(dto);
    }
}
