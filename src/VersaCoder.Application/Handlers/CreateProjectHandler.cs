using MediatR;
using VersaCoder.Abstractions.Repositories;
using VersaCoder.Application.Commands;
using VersaCoder.Application.Common;
using VersaCoder.Application.DTOs;
using VersaCoder.Domain.Entities;

namespace VersaCoder.Application.Handlers;

public class CreateProjectHandler : IRequestHandler<CreateProjectCommand, Result<ProjectDto>>
{
    private readonly IProjectRepository _projectRepository;

    public CreateProjectHandler(IProjectRepository projectRepository)
    {
        _projectRepository = projectRepository;
    }

    public async Task<Result<ProjectDto>> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
    {
        var existingProject = await _projectRepository.GetByNameAsync(request.Name, cancellationToken);
        if (existingProject != null)
            return Result<ProjectDto>.Failure($"Project with name {request.Name} already exists");

        var project = new Project(request.Name, request.RootPath)
        {
            Description = request.Description
        };

        await _projectRepository.AddAsync(project, cancellationToken);
        await _projectRepository.SaveChangesAsync(cancellationToken);

        var dto = new ProjectDto
        {
            Id = project.Id,
            Name = project.Name,
            RootPath = project.RootPath,
            Description = project.Description,
            CreatedAt = project.CreatedAt,
            LastAnalyzedAt = project.LastAnalyzedAt
        };

        return Result<ProjectDto>.Success(dto);
    }
}
