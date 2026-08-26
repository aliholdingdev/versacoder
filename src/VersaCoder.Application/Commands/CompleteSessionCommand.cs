using MediatR;
using VersaCoder.Application.Common;
using VersaCoder.Application.DTOs;

namespace VersaCoder.Application.Commands;

public class CompleteSessionCommand : IRequest<Result<SessionDto>>
{
    public Guid SessionId { get; set; }
}
