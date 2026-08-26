---
title: "ADR-006 — Tool System Architecture"
type: decision
status: accepted
date: 2026-08-25
version: 1.0.0
---

# ADR-006 — Tool System Architecture

**Status:** Accepted  
**Date:** 2026-08-25  
**Category:** Infrastructure.AI  
**Sorumlu:** Build Agent

---

## 1. Karar

Versa Coder, **45+ araç** içeren, **plugin tabanlı** bir tool sistemi kullanacaktır.

## 2. Bağlam

AI ajanlarının çeşitli görevleri yerine getirmesi için araçlara ihtiyacı vardır:
- Dosya okuma/yazma/düzenleme
- Terminal komutları
- Git işlemleri
- Test çalıştırma
- AI sorguları
- MCP araçları
- Proje analizi
- Session yönetimi
- Context yönetimi

## 3. Seçenekler

| Seçenek | Artıları | Eksileri |
|---------|----------|----------|
| **Hardcoded Tools** | Basit | Genişletilemez |
| **Plugin System** | Esnek, genişletilebilir | Karmaşık |
| **Reflection-based** | Dinamik yükleme | Yavaş, hata riski |
| **Compiled Plugins** | Hızlı, güvenli | Build süreci gerekli |

## 4. Karar

**Compiled Plugin System** + **Built-in Tools** kombinasyonu seçildi.

## 5. Tool Kategorileri

| Kategori | Tool Sayısı | Örnekler |
|----------|-------------|----------|
| **Dosya** | 5 | Read, Write, Edit, Glob, Grep |
| **Terminal** | 2 | Bash, PowerShell |
| **Git** | 7 | Status, Diff, Commit, Push, Pull, Branch, Log |
| **Test** | 3 | Run Tests, Coverage, Generate |
| **AI** | 3 | LLM Query, Embedding, Summarize |
| **MCP** | 3 | Resource Read, Tool Call, List |
| **Proje** | 5 | Index, Analyze, Diagram, Search, Quality |
| **Session** | 5 | Save, Load, Branch, Fork, Merge |
| **Context** | 4 | Assemble, Update, Validate, Compress |
| **Editör** | 4 | Find, Replace, Format, Lint |
| **Güvenlik** | 3 | Scan Secrets, Validate Auth, Check Permissions |
| **Toplam** | 45+ | — |

## 6. ITool Arayüzü

```csharp
public interface ITool
{
    string Name { get; }
    string Description { get; }
    string Category { get; }
    ToolParameters Schema { get; }
    
    Task<ToolResult> ExecuteAsync(
        ToolRequest request,
        CancellationToken cancellationToken = default);
    
    Task<bool> CanExecuteAsync(
        ToolRequest request,
        CancellationToken cancellationToken = default);
}
```

## 7. ToolResult Tasarımı

```csharp
public class ToolResult
{
    public bool Success { get; set; }
    public string Output { get; set; }
    public string Error { get; set; }
    public List<ToolResult> SubResults { get; set; }
    public TimeSpan Duration { get; set; }
    public Dictionary<string, object> Metadata { get; set; }
}
```

## 8. ToolRegistry Tasarımı

```csharp
public class ToolRegistry
{
    private readonly Dictionary<string, ITool> _tools;
    private readonly Dictionary<string, ToolCategory> _categories;
    
    public void Register(ITool tool)
    {
        _tools[tool.Name] = tool;
        
        if (!_categories.ContainsKey(tool.Category))
            _categories[tool.Category] = new ToolCategory();
        
        _categories[tool.Category].Add(tool);
    }
    
    public ITool GetTool(string name)
    {
        return _tools.TryGetValue(name, out var tool) ? tool : null;
    }
    
    public List<ToolInfo> GetToolsForAgent(string agentRole)
    {
        return _tools.Values
            .Where(t => IsAllowedForAgent(t, agentRole))
            .Select(t => new ToolInfo(t))
            .ToList();
    }
}
```

## 9. ToolExecutor Tasarımı

```csharp
public class ToolExecutor
{
    private readonly ToolRegistry _registry;
    private readonly ILogger _logger;
    private readonly ISecurityService _security;
    
    public async Task<ToolResult> ExecuteAsync(
        ToolCall toolCall,
        CancellationToken cancellationToken = default)
    {
        // 1. Tool'u bul
        var tool = _registry.GetTool(toolCall.Name);
        if (tool == null)
            return ToolResult.Fail($"Tool not found: {toolCall.Name}");
        
        // 2. Güvenlik kontrolü
        if (!await _security.CanExecuteAsync(tool, toolCall.Parameters))
            return ToolResult.Fail("Permission denied");
        
        // 3. Execute
        var stopwatch = Stopwatch.StartNew();
        try
        {
            var result = await tool.ExecuteAsync(
                new ToolRequest(toolCall.Parameters),
                cancellationToken);
            
            stopwatch.Stop();
            result.Duration = stopwatch.Elapsed;
            
            // 4. Logla
            _logger.LogInformation(
                "Tool {ToolName} executed in {Duration}ms",
                toolCall.Name, stopwatch.ElapsedMilliseconds);
            
            return result;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex,
                "Tool {ToolName} failed after {Duration}ms",
                toolCall.Name, stopwatch.ElapsedMilliseconds);
            
            return ToolResult.Fail(ex.Message);
        }
    }
}
```

## 10. Built-in Tool Örneği: ReadFileTool

```csharp
public class ReadFileTool : ITool
{
    public string Name => "read_file";
    public string Description => "Read file contents";
    public string Category => "file";
    
    public ToolParameters Schema => new()
    {
        Properties = new Dictionary<string, ToolParameter>
        {
            ["path"] = new() { Type = "string", Required = true },
            ["offset"] = new() { Type = "integer", Required = false },
            ["limit"] = new() { Type = "integer", Required = false }
        }
    };
    
    public async Task<ToolResult> ExecuteAsync(
        ToolRequest request,
        CancellationToken cancellationToken = default)
    {
        var path = request.GetString("path");
        var offset = request.GetInt("offset") ?? 0;
        var limit = request.GetInt("limit") ?? 1000;
        
        if (!File.Exists(path))
            return ToolResult.Fail($"File not found: {path}");
        
        var lines = await File.ReadAllLinesAsync(path, cancellationToken);
        var selected = lines.Skip(offset).Take(limit);
        
        return ToolResult.Success(string.Join("\n", selected));
    }
}
```

## 11. Tool Registration (DI)

```csharp
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddToolSystem(
        this IServiceCollection services)
    {
        // Built-in tools
        services.AddSingleton<ITool, ReadFileTool>();
        services.AddSingleton<ITool, WriteFileTool>();
        services.AddSingleton<ITool, EditFileTool>();
        services.AddSingleton<ITool, GlobTool>();
        services.AddSingleton<ITool, GrepTool>();
        services.AddSingleton<ITool, BashTool>();
        services.AddSingleton<ITool, PowerShellTool>();
        services.AddSingleton<ITool, GitStatusTool>();
        services.AddSingleton<ITool, GitDiffTool>();
        services.AddSingleton<ITool, GitCommitTool>();
        // ... 35+ more tools
        
        // Registry and executor
        services.AddSingleton<ToolRegistry>();
        services.AddSingleton<ToolExecutor>();
        
        return services;
    }
}
```

## 12. Plugin Tool Yükleme

```csharp
public class PluginToolLoader
{
    private readonly string _pluginDirectory;
    
    public List<ITool> LoadPluginTools()
    {
        var tools = new List<ITool>();
        
        foreach (var pluginDir in Directory.GetDirectories(_pluginDirectory))
        {
            var assembly = LoadPluginAssembly(pluginDir);
            var toolTypes = assembly.GetTypes()
                .Where(t => typeof(ITool).IsAssignableFrom(t));
            
            foreach (var type in toolTypes)
            {
                var tool = (ITool)Activator.CreateInstance(type);
                tools.Add(tool);
            }
        }
        
        return tools;
    }
}
```

---

**Authority:** Vault Steward  
**Last Updated:** 2026-08-25
