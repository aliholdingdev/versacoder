using VersaCoder.Abstractions.Repositories;
using VersaCoder.Abstractions.Services;
using VersaCoder.Domain.Enums;

namespace VersaCoder.Application.Services;

public class ContextManagerService : IContextManager
{
    private readonly IMessageRepository _messageRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly ILearningRepository _learningRepository;

    public ContextManagerService(
        IMessageRepository messageRepository,
        IProjectRepository projectRepository,
        ILearningRepository learningRepository)
    {
        _messageRepository = messageRepository;
        _projectRepository = projectRepository;
        _learningRepository = learningRepository;
    }

    public async Task<AssembledContext> AssembleAsync(AgentRole agentRole, Guid sessionId, CancellationToken cancellationToken = default)
    {
        var context = new AssembledContext();

        var sessionMessages = await _messageRepository.GetBySessionIdPagedAsync(sessionId, 1, 50, cancellationToken);

        context.Sources.Add(new ContextData
        {
            Source = "session",
            Type = ContextType.SESSION,
            Content = string.Join("\n", sessionMessages.Select(m => $"{m.Role}: {m.Content}")),
            TokenCount = sessionMessages.Sum(m => m.Content.Length / 4),
            Priority = 10
        });

        context.TokenCount = context.Sources.Sum(s => s.TokenCount);

        return context;
    }

    public async Task<ContextData> GetContextAsync(Guid sessionId, ContextType type, CancellationToken cancellationToken = default)
    {
        var messages = await _messageRepository.GetBySessionIdPagedAsync(sessionId, 1, 50, cancellationToken);

        return new ContextData
        {
            Source = type.ToString().ToLower(),
            Type = type,
            Content = string.Join("\n", messages.Select(m => $"{m.Role}: {m.Content}")),
            TokenCount = messages.Sum(m => m.Content.Length / 4),
            Priority = 10
        };
    }

    public async Task UpdateContextAsync(Guid sessionId, ContextType type, string content, CancellationToken cancellationToken = default)
    {
        var message = new Domain.Entities.Message(sessionId, "system", content);
        await _messageRepository.AddAsync(message, cancellationToken);
        await _messageRepository.SaveChangesAsync(cancellationToken);
    }
}
