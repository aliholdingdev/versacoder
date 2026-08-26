using VersaCoder.Domain.Enums;

namespace VersaCoder.Abstractions.Services;

public interface IContextManager
{
    Task<AssembledContext> AssembleAsync(AgentRole agentRole, Guid sessionId, CancellationToken cancellationToken = default);
    Task<ContextData> GetContextAsync(Guid sessionId, ContextType type, CancellationToken cancellationToken = default);
    Task UpdateContextAsync(Guid sessionId, ContextType type, string content, CancellationToken cancellationToken = default);
}

public class AssembledContext
{
    public List<ContextData> Sources { get; set; } = new();
    public int TokenCount { get; set; }
    public Dictionary<string, object> Metadata { get; set; } = new();
}

public class ContextData
{
    public string Source { get; set; } = string.Empty;
    public ContextType Type { get; set; }
    public string Content { get; set; } = string.Empty;
    public int TokenCount { get; set; }
    public int Priority { get; set; }
}
