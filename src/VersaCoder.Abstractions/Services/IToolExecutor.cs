namespace VersaCoder.Abstractions.Services;

public interface IToolExecutor
{
    Task<ToolResult> ExecuteAsync(ToolCallRequest request, CancellationToken cancellationToken = default);
    Task<List<ToolInfo>> GetAvailableToolsAsync(string? agentRole = null, CancellationToken cancellationToken = default);
}

public class ToolCallRequest
{
    public string ToolName { get; set; } = string.Empty;
    public Dictionary<string, object> Parameters { get; set; } = new();
    public string? AgentName { get; set; }
}

public class ToolResult
{
    public bool Success { get; set; }
    public string Output { get; set; } = string.Empty;
    public string? Error { get; set; }
    public TimeSpan Duration { get; set; }
    public Dictionary<string, object> Metadata { get; set; } = new();
}

public class ToolInfo
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public Dictionary<string, object> Schema { get; set; } = new();
}
