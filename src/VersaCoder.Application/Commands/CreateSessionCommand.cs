using MediatR;
using VersaCoder.Application.Common;
using VersaCoder.Application.DTOs;

namespace VersaCoder.Application.Commands;

public class CreateSessionCommand : IRequest<Result<SessionDto>>
{
    public string Name { get; set; } = string.Empty;
}
