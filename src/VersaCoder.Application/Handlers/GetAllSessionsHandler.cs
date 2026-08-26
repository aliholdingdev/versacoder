using MediatR;
using VersaCoder.Abstractions.Repositories;
using VersaCoder.Application.Common;
using VersaCoder.Application.Queries;
using VersaCoder.Application.DTOs;

namespace VersaCoder.Application.Handlers;

public class GetAllSessionsHandler : IRequestHandler<GetAllSessionsQuery, Result<List<SessionDto>>>
{
    private readonly ISessionRepository _sessionRepository;

    public GetAllSessionsHandler(ISessionRepository sessionRepository)
    {
        _sessionRepository = sessionRepository;
    }

    public async Task<Result<List<SessionDto>>> Handle(GetAllSessionsQuery request, CancellationToken cancellationToken)
    {
        var sessions = await _sessionRepository.GetAllAsync(cancellationToken);

        var dtos = sessions.Select(s => new SessionDto
        {
            Id = s.Id,
            Name = s.Name,
            State = s.State.ToString(),
            ParentId = s.ParentId,
            CreatedAt = s.CreatedAt,
            UpdatedAt = s.UpdatedAt,
            CompletedAt = s.CompletedAt,
            MessageCount = s.Messages.Count
        }).ToList();

        return Result<List<SessionDto>>.Success(dtos);
    }
}
