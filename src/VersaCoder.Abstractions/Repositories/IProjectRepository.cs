using VersaCoder.Domain.Entities;
using VersaCoder.Domain.Interfaces;

namespace VersaCoder.Abstractions.Repositories;

public interface IProjectRepository : IRepository<Project>
{
    Task<Project?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<Project?> GetByRootPathAsync(string rootPath, CancellationToken cancellationToken = default);
    Task<Project?> GetWithFilesAsync(Guid id, CancellationToken cancellationToken = default);
}
