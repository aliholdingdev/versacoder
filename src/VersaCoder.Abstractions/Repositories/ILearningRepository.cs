using VersaCoder.Domain.Entities;
using VersaCoder.Domain.Enums;
using VersaCoder.Domain.Interfaces;

namespace VersaCoder.Abstractions.Repositories;

public interface ILearningRepository : IRepository<LearningEntry>
{
    Task<List<LearningEntry>> GetByCategoryAsync(LearningCategory category, CancellationToken cancellationToken = default);
    Task<LearningEntry?> GetByKeyAsync(string key, CancellationToken cancellationToken = default);
    Task<List<LearningEntry>> GetByMinConfidenceAsync(float minConfidence, CancellationToken cancellationToken = default);
}
