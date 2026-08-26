using MediatR;
using VersaCoder.Abstractions.Repositories;
using VersaCoder.Application.Common;
using VersaCoder.Application.Queries;
using VersaCoder.Application.DTOs;

namespace VersaCoder.Application.Handlers;

public class GetSessionHandler : IRequestHandler<GetSessionQuery, Result<SessionDto>>
{
    private readonly ISessionRepository _sessionRepository;

    public GetSessionHandler(ISessionRepository sessionRepository)
    {
        _sessionRepository = sessionRepository;
    }

    public async Task<Result<SessionDto>> Handle(GetSessionQuery request, CancellationToken cancellationToken)
    {
        var session = await _sessionRepository.GetByIdAsync(request.SessionId, cancellationToken);
        if (session == null)
            return Result<SessionDto>.Failure($"Session with id {request.SessionId} not found");

        var dto = new SessionDto
        {
            Id = session.Id,
            Name = session.Name,
            State = session.State.ToString(),
            ParentId = session.ParentId,
            CreatedAt = session.CreatedAt,
            UpdatedAt = session.UpdatedAt,
            CompletedAt = session.CompletedAt,
            MessageCount = session.Messages.Count,
            Metadata = session.Metadata
        };

        return Result<SessionDto>.Success(dto);
    }
}
