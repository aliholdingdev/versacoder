using VersaCoder.Domain.Entities;
using VersaCoder.Domain.Enums;

namespace VersaCoder.Abstractions.Services;

public interface ILearningService
{
    Task RecordPatternAsync(string key, string value, float confidence, string? source, CancellationToken cancellationToken = default);
    Task RecordCorrectionAsync(string key, string value, string? source, CancellationToken cancellationToken = default);
    Task RecordKnowledgeAsync(string key, string value, float confidence, string? source, CancellationToken cancellationToken = default);
    Task RecordRuleAsync(string key, string value, float confidence, string? source, CancellationToken cancellationToken = default);
    Task<List<LearningEntry>> GetRelevantPatternsAsync(Guid sessionId, CancellationToken cancellationToken = default);
    Task<List<LearningEntry>> GetRecentCorrectionsAsync(CancellationToken cancellationToken = default);
}
