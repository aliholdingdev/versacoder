using MediatR;
using VersaCoder.Application.Common;
using VersaCoder.Application.DTOs;

namespace VersaCoder.Application.Queries;

public class GetAllSessionsQuery : IRequest<Result<List<SessionDto>>>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
