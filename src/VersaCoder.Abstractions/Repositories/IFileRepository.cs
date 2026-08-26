using VersaCoder.Domain.Entities;
using VersaCoder.Domain.Interfaces;

namespace VersaCoder.Abstractions.Repositories;

public interface IFileRepository : IRepository<FileEntry>
{
    Task<List<FileEntry>> GetByProjectIdAsync(Guid projectId, CancellationToken cancellationToken = default);
    Task<FileEntry?> GetByRelativePathAsync(Guid projectId, string relativePath, CancellationToken cancellationToken = default);
    Task<List<FileEntry>> SearchByNameAsync(Guid projectId, string fileName, CancellationToken cancellationToken = default);
}
