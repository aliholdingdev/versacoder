using MediatR;
using VersaCoder.Application.Common;
using VersaCoder.Application.DTOs;

namespace VersaCoder.Application.Queries;

public class GetProjectQuery : IRequest<Result<ProjectDto>>
{
    public Guid ProjectId { get; set; }
}
