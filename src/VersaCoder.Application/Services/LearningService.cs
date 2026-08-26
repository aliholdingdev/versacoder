using VersaCoder.Abstractions.Repositories;
using VersaCoder.Abstractions.Services;
using VersaCoder.Domain.Entities;
using VersaCoder.Domain.Enums;

namespace VersaCoder.Application.Services;

public class LearningService : ILearningService
{
    private readonly ILearningRepository _learningRepository;

    public LearningService(ILearningRepository learningRepository)
    {
        _learningRepository = learningRepository;
    }

    public async Task RecordPatternAsync(string key, string value, float confidence, string? source, CancellationToken cancellationToken = default)
    {
        var entry = new LearningEntry(LearningCategory.PATTERN, key, value)
        {
            Confidence = confidence
        };
        if (!string.IsNullOrEmpty(source))
            entry.Source = source;
        await _learningRepository.AddAsync(entry, cancellationToken);
        await _learningRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task RecordCorrectionAsync(string key, string value, string? source, CancellationToken cancellationToken = default)
    {
        var entry = new LearningEntry(LearningCategory.CORRECTION, key, value);
        if (!string.IsNullOrEmpty(source))
            entry.Source = source;
        await _learningRepository.AddAsync(entry, cancellationToken);
        await _learningRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task RecordKnowledgeAsync(string key, string value, float confidence, string? source, CancellationToken cancellationToken = default)
    {
        var entry = new LearningEntry(LearningCategory.KNOWLEDGE, key, value)
        {
            Confidence = confidence
        };
        if (!string.IsNullOrEmpty(source))
            entry.Source = source;
        await _learningRepository.AddAsync(entry, cancellationToken);
        await _learningRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task RecordRuleAsync(string key, string value, float confidence, string? source, CancellationToken cancellationToken = default)
    {
        var entry = new LearningEntry(LearningCategory.RULE, key, value)
        {
            Confidence = confidence
        };
        if (!string.IsNullOrEmpty(source))
            entry.Source = source;
        await _learningRepository.AddAsync(entry, cancellationToken);
        await _learningRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<LearningEntry>> GetRelevantPatternsAsync(Guid sessionId, CancellationToken cancellationToken = default)
    {
        return await _learningRepository.GetByCategoryAsync(LearningCategory.PATTERN, cancellationToken);
    }

    public async Task<List<LearningEntry>> GetRecentCorrectionsAsync(CancellationToken cancellationToken = default)
    {
        return await _learningRepository.GetByCategoryAsync(LearningCategory.CORRECTION, cancellationToken);
    }
}
