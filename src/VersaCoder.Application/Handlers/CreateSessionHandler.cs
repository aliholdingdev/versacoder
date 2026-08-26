using MediatR;
using VersaCoder.Abstractions.Repositories;
using VersaCoder.Application.Commands;
using VersaCoder.Application.Common;
using VersaCoder.Application.DTOs;
using VersaCoder.Domain.Entities;

namespace VersaCoder.Application.Handlers;

public class CreateSessionHandler : IRequestHandler<CreateSessionCommand, Result<SessionDto>>
{
    private readonly ISessionRepository _sessionRepository;

    public CreateSessionHandler(ISessionRepository sessionRepository)
    {
        _sessionRepository = sessionRepository;
    }

    public async Task<Result<SessionDto>> Handle(CreateSessionCommand request, CancellationToken cancellationToken)
    {
        var session = new Session(request.Name);
        await _sessionRepository.AddAsync(session, cancellationToken);
        await _sessionRepository.SaveChangesAsync(cancellationToken);

        var dto = new SessionDto
        {
            Id = session.Id,
            Name = session.Name,
            State = session.State.ToString(),
            CreatedAt = session.CreatedAt,
            UpdatedAt = session.UpdatedAt
        };

        return Result<SessionDto>.Success(dto);
    }
}
