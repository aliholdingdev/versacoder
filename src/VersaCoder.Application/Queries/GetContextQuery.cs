using MediatR;
using VersaCoder.Application.Common;
using VersaCoder.Application.DTOs;

namespace VersaCoder.Application.Queries;

public class GetContextQuery : IRequest<Result<ContextDto>>
{
    public Guid SessionId { get; set; }
    public string ContextType { get; set; } = string.Empty;
}
