using Microsoft.Extensions.Logging;
using VersaCoder.Abstractions.Services;
using VersaCoder.Domain.Constants;

namespace VersaCoder.Infrastructure.AI;

public class ToolRegistry : IToolExecutor
{
    private readonly Dictionary<string, Func<ToolCallRequest, CancellationToken, Task<ToolResult>>> _tools;
    private readonly ILogger<ToolRegistry> _logger;

    public ToolRegistry(ILogger<ToolRegistry> logger)
    {
        _tools = new Dictionary<string, Func<ToolCallRequest, CancellationToken, Task<ToolResult>>>(StringComparer.OrdinalIgnoreCase);
        _logger = logger;
        RegisterBuiltInTools();
    }

    public void RegisterTool(string name, Func<ToolCallRequest, CancellationToken, Task<ToolResult>> handler)
    {
        _tools[name] = handler;
        _logger.LogDebug("Registered tool: {Tool}", name);
    }

    public async Task<ToolResult> ExecuteAsync(ToolCallRequest request, CancellationToken cancellationToken = default)
    {
        if (!_tools.TryGetValue(request.ToolName, out var handler))
        {
            return new ToolResult
            {
                Success = false,
                Error = $"Tool '{request.ToolName}' not found"
            };
        }

        try
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = await handler(request, cancellationToken);
            stopwatch.Stop();
            result.Duration = stopwatch.Elapsed;
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing tool {Tool}", request.ToolName);
            return new ToolResult
            {
                Success = false,
                Error = ex.Message
            };
        }
    }

    public Task<List<ToolInfo>> GetAvailableToolsAsync(string? agentRole = null, CancellationToken cancellationToken = default)
    {
        var tools = _tools.Keys.Select(name => new ToolInfo
        {
            Name = name,
            Description = GetToolDescription(name),
            Category = GetToolCategory(name)
        }).ToList();

        return Task.FromResult(tools);
    }

    private void RegisterBuiltInTools()
    {
        // File tools
        RegisterTool(ToolNames.READ_FILE, async (req, ct) =>
        {
            var path = req.Parameters.GetValueOrDefault("path")?.ToString() ?? string.Empty;
            if (!File.Exists(path))
                return new ToolResult { Success = false, Error = "File not found" };

            var content = await File.ReadAllTextAsync(path, ct);
            return new ToolResult { Success = true, Output = content };
        });

        RegisterTool(ToolNames.WRITE_FILE, async (req, ct) =>
        {
            var path = req.Parameters.GetValueOrDefault("path")?.ToString() ?? string.Empty;
            var content = req.Parameters.GetValueOrDefault("content")?.ToString() ?? string.Empty;
            var dir = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            await File.WriteAllTextAsync(path, content, ct);
            return new ToolResult { Success = true, Output = $"Written to {path}" };
        });

        RegisterTool(ToolNames.LIST, async (req, ct) =>
        {
            var path = req.Parameters.GetValueOrDefault("path")?.ToString() ?? ".";
            if (!Directory.Exists(path))
                return new ToolResult { Success = false, Error = "Directory not found" };

            var entries = Directory.GetFileSystemEntries(path);
            var output = string.Join("\n", entries.Select(e => Path.GetFileName(e)));
            return new ToolResult { Success = true, Output = output };
        });

        RegisterTool(ToolNames.GLOB, async (req, ct) =>
        {
            var pattern = req.Parameters.GetValueOrDefault("pattern")?.ToString() ?? "*";
            var root = req.Parameters.GetValueOrDefault("root")?.ToString() ?? ".";
            var files = Directory.GetFiles(root, pattern, SearchOption.AllDirectories);
            return new ToolResult { Success = true, Output = string.Join("\n", files) };
        });

        RegisterTool(ToolNames.GREP, async (req, ct) =>
        {
            var query = req.Parameters.GetValueOrDefault("query")?.ToString() ?? string.Empty;
            var root = req.Parameters.GetValueOrDefault("root")?.ToString() ?? ".";
            var results = new List<string>();

            foreach (var file in Directory.GetFiles(root, "*.cs", SearchOption.AllDirectories))
            {
                var lines = await File.ReadAllLinesAsync(file, ct);
                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i].Contains(query, StringComparison.OrdinalIgnoreCase))
                        results.Add($"{file}:{i + 1}: {lines[i].Trim()}");
                }
            }

            return new ToolResult { Success = true, Output = string.Join("\n", results) };
        });
    }

    private string GetToolDescription(string name) => name switch
    {
        ToolNames.READ_FILE => "Read file contents",
        ToolNames.WRITE_FILE => "Write content to file",
        ToolNames.LIST => "List directory contents",
        ToolNames.GLOB => "Find files by pattern",
        ToolNames.GREP => "Search file contents",
        _ => "Unknown tool"
    };

    private string GetToolCategory(string name) => name switch
    {
        ToolNames.READ_FILE or ToolNames.WRITE_FILE or ToolNames.LIST => "file",
        ToolNames.GLOB or ToolNames.GREP => "search",
        _ => "general"
    };
}
