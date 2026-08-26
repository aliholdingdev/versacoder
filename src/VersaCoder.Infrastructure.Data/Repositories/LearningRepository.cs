using Microsoft.EntityFrameworkCore;
using VersaCoder.Abstractions.Repositories;
using VersaCoder.Domain.Entities;
using VersaCoder.Domain.Enums;

namespace VersaCoder.Infrastructure.Data.Repositories;

public class LearningRepository : Repository<LearningEntry>, ILearningRepository
{
    public LearningRepository(VersaCoderDbContext context) : base(context)
    {
    }

    public async Task<List<LearningEntry>> GetByCategoryAsync(LearningCategory category, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(l => l.Category == category)
            .OrderByDescending(l => l.Confidence)
            .ToListAsync(cancellationToken);
    }

    public async Task<LearningEntry?> GetByKeyAsync(string key, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(l => l.Key == key, cancellationToken);
    }

    public async Task<List<LearningEntry>> GetByMinConfidenceAsync(float minConfidence, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(l => l.Confidence >= minConfidence)
            .OrderByDescending(l => l.Confidence)
            .ToListAsync(cancellationToken);
    }
}
