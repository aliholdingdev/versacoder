using MediatR;
using VersaCoder.Application.Common;
using VersaCoder.Application.DTOs;

namespace VersaCoder.Application.Queries;

public class GetSessionQuery : IRequest<Result<SessionDto>>
{
    public Guid SessionId { get; set; }
}
