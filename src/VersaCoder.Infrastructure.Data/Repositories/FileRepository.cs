using Microsoft.EntityFrameworkCore;
using VersaCoder.Abstractions.Repositories;
using VersaCoder.Domain.Entities;

namespace VersaCoder.Infrastructure.Data.Repositories;

public class FileRepository : Repository<FileEntry>, IFileRepository
{
    public FileRepository(VersaCoderDbContext context) : base(context)
    {
    }

    public async Task<List<FileEntry>> GetByProjectIdAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(f => f.ProjectId == projectId)
            .OrderBy(f => f.RelativePath)
            .ToListAsync(cancellationToken);
    }

    public async Task<FileEntry?> GetByRelativePathAsync(Guid projectId, string relativePath, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(f => f.ProjectId == projectId && f.RelativePath == relativePath, cancellationToken);
    }

    public async Task<List<FileEntry>> SearchByNameAsync(Guid projectId, string fileName, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(f => f.ProjectId == projectId && f.FileName.Contains(fileName))
            .ToListAsync(cancellationToken);
    }
}
