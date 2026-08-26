using MediatR;
using VersaCoder.Abstractions.Repositories;
using VersaCoder.Application.Commands;
using VersaCoder.Application.Common;
using VersaCoder.Application.DTOs;

namespace VersaCoder.Application.Handlers;

public class CompleteSessionHandler : IRequestHandler<CompleteSessionCommand, Result<SessionDto>>
{
    private readonly ISessionRepository _sessionRepository;

    public CompleteSessionHandler(ISessionRepository sessionRepository)
    {
        _sessionRepository = sessionRepository;
    }

    public async Task<Result<SessionDto>> Handle(CompleteSessionCommand request, CancellationToken cancellationToken)
    {
        var session = await _sessionRepository.GetByIdAsync(request.SessionId, cancellationToken);
        if (session == null)
            return Result<SessionDto>.Failure($"Session with id {request.SessionId} not found");

        session.Complete();
        await _sessionRepository.UpdateAsync(session, cancellationToken);
        await _sessionRepository.SaveChangesAsync(cancellationToken);

        var dto = new SessionDto
        {
            Id = session.Id,
            Name = session.Name,
            State = session.State.ToString(),
            CreatedAt = session.CreatedAt,
            UpdatedAt = session.UpdatedAt,
            CompletedAt = session.CompletedAt
        };

        return Result<SessionDto>.Success(dto);
    }
}
