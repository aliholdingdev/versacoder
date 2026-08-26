using VersaCoder.Domain.Enums;

namespace VersaCoder.Abstractions.Services;

public interface IAgentRunner
{
    Task<AgentResponse> RunAsync(AgentRequest request, CancellationToken cancellationToken = default);
    IAsyncEnumerable<AgentStreamChunk> RunStreamingAsync(AgentRequest request, CancellationToken cancellationToken = default);
}

public class AgentRequest
{
    public string Prompt { get; set; } = string.Empty;
    public Guid SessionId { get; set; }
    public string? AgentName { get; set; }
    public AgentRole? PreferredRole { get; set; }
    public Dictionary<string, object> Context { get; set; } = new();
}

public class AgentResponse
{
    public string Content { get; set; } = string.Empty;
    public string AgentName { get; set; } = string.Empty;
    public bool HasToolCalls { get; set; }
    public List<ToolCallResult> ToolCalls { get; set; } = new();
    public TimeSpan Duration { get; set; }
}

public class AgentStreamChunk
{
    public string Content { get; set; } = string.Empty;
    public bool IsComplete { get; set; }
}

public class ToolCallResult
{
    public string ToolName { get; set; } = string.Empty;
    public Dictionary<string, object> Parameters { get; set; } = new();
    public string Result { get; set; } = string.Empty;
    public bool Success { get; set; }
}
