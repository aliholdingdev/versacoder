using MediatR;
using VersaCoder.Application.Common;
using VersaCoder.Application.DTOs;

namespace VersaCoder.Application.Queries;

public class GetAllProjectsQuery : IRequest<Result<List<ProjectDto>>>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
