namespace VersaCoder.Abstractions.Plugins;

public interface IPlugin
{
    string Name { get; }
    string Version { get; }
    string Description { get; }
    
    Task InitializeAsync(IPluginContext context, CancellationToken cancellationToken = default);
    Task ShutdownAsync(CancellationToken cancellationToken = default);
    Task<List<ITool>> GetToolsAsync(CancellationToken cancellationToken = default);
}

public interface IPluginContext
{
    string PluginDirectory { get; }
    Dictionary<string, object> Configuration { get; }
    T GetService<T>() where T : class;
}

public interface ITool
{
    string Name { get; }
    string Description { get; }
    string Category { get; }
    
    Task<ToolResult> ExecuteAsync(ToolRequest request, CancellationToken cancellationToken = default);
}

public class ToolRequest
{
    public Dictionary<string, object> Parameters { get; set; } = new();
    public string? AgentName { get; set; }
}

public class ToolResult
{
    public bool Success { get; set; }
    public string Output { get; set; } = string.Empty;
    public string? Error { get; set; }
}
