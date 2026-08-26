using MediatR;
using VersaCoder.Application.Common;
using VersaCoder.Application.DTOs;

namespace VersaCoder.Application.Commands;

public class BranchSessionCommand : IRequest<Result<SessionDto>>
{
    public Guid SourceSessionId { get; set; }
    public string BranchName { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
}
