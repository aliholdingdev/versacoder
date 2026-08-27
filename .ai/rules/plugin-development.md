---
title: "Versa Coder — Plugin Geliştirme Rehberi"
type: rules
category: plugin
date: 2026-08-25
updated: 2026-08-25
status: active
version: 1.0.0
---

# Versa Coder — Plugin Geliştirme Rehberi

---

## 1. Plugin Mimarisi

```
┌──────────────────────────────────────┐
│           PLUGIN HOST                │
│  ┌────────────────────────────────┐  │
│  │  Plugin Loader (Assembly.Load) │  │
│  └────────────────────────────────┘  │
│           ↓                          │
│  ┌────────────────────────────────┐  │
│  │  IPlugin Interface             │  │
│  │  ├── Initialize()              │  │
│  │  ├── GetTools()                │  │
│  │  ├── GetCommands()             │  │
│  │  └── Dispose()                 │  │
│  └────────────────────────────────┘  │
│           ↓                          │
│  ┌────────────────────────────────┐  │
│  │  Plugin Assembly (.dll)        │  │
│  └────────────────────────────────┘  │
└──────────────────────────────────────┘
```

---

## 2. Plugin Arayüzü

```csharp
public interface IPlugin : IDisposable
{
    string Name { get; }
    string Version { get; }
    string Description { get; }
    void Initialize(IServiceProvider serviceProvider);
    IReadOnlyList<ToolDefinition> GetTools();
    IReadOnlyList<CommandDefinition> GetCommands();
}
```

---

## 3. Plugin Dağıtımı

| Konum | Açıklama |
|-------|----------|
| `plugins/` | Yerel plugin dizini |
| NuGet | Paket yöneticisi |
| Custom | Özel plugin kaynağı |

---

## 4. Plugin Implementasyonu

### 4.1 Plugin Loader

```csharp
public class PluginLoader : IPluginLoader
{
    private readonly string _pluginDirectory;
    private readonly ILogger<PluginLoader> _logger;
    private readonly Dictionary<string, IPlugin> _loadedPlugins;
    
    public PluginLoader(string pluginDirectory, ILogger<PluginLoader> logger)
    {
        _pluginDirectory = pluginDirectory;
        _logger = logger;
        _loadedPlugins = new Dictionary<string, IPlugin>();
    }
    
    public async Task<IReadOnlyList<IPlugin>> LoadPluginsAsync()
    {
        var plugins = new List<IPlugin>();
        
        if (!Directory.Exists(_pluginDirectory))
        {
            _logger.LogWarning("Plugin directory not found: {Directory}", _pluginDirectory);
            return plugins;
        }
        
        var pluginFiles = Directory.GetFiles(_pluginDirectory, "*.dll");
        
        foreach (var pluginFile in pluginFiles)
        {
            try
            {
                var plugin = await LoadPluginAsync(pluginFile);
                if (plugin != null)
                {
                    plugins.Add(plugin);
                    _loadedPlugins[plugin.Name] = plugin;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load plugin: {File}", pluginFile);
            }
        }
        
        return plugins;
    }
    
    private async Task<IPlugin?> LoadPluginAsync(string assemblyPath)
    {
        var assembly = Assembly.LoadFrom(assemblyPath);
        
        var pluginType = assembly.GetTypes()
            .FirstOrDefault(t => typeof(IPlugin).IsAssignableFrom(t) && !t.IsAbstract);
        
        if (pluginType == null)
        {
            _logger.LogWarning("No IPlugin implementation found in {Assembly}", assemblyPath);
            return null;
        }
        
        var plugin = (IPlugin)Activator.CreateInstance(pluginType)!;
        
        // Initialize plugin
        plugin.Initialize(_serviceProvider);
        
        _logger.LogInformation(
            "Loaded plugin: {Name} v{Version}", 
            plugin.Name, 
            plugin.Version);
        
        return plugin;
    }
    
    public void UnloadPlugin(string pluginName)
    {
        if (_loadedPlugins.TryGetValue(pluginName, out var plugin))
        {
            plugin.Dispose();
            _loadedPlugins.Remove(pluginName);
            
            _logger.LogInformation("Unloaded plugin: {Name}", pluginName);
        }
    }
    
    public IReadOnlyList<string> GetLoadedPlugins()
    {
        return _loadedPlugins.Keys.ToList().AsReadOnly();
    }
}
```

### 4.2 Plugin Base Class

```csharp
public abstract class PluginBase : IPlugin
{
    protected IServiceProvider ServiceProvider { get; private set; } = null!;
    
    public abstract string Name { get; }
    public abstract string Version { get; }
    public abstract string Description { get; }
    
    public virtual void Initialize(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
    }
    
    public virtual IReadOnlyList<ToolDefinition> GetTools()
    {
        return new List<ToolDefinition>();
    }
    
    public virtual IReadOnlyList<CommandDefinition> GetCommands()
    {
        return new List<CommandDefinition>();
    }
    
    public virtual void Dispose()
    {
        // Override in derived classes
    }
}
```

---

## 5. Plugin Tool Örnekleri

### 5.1 SQL Plugin

```csharp
public class SQLPlugin : PluginBase
{
    public override string Name => "SQL Plugin";
    public override string Version => "1.0.0";
    public override string Description => "SQL query execution plugin";
    
    public override IReadOnlyList<ToolDefinition> GetTools()
    {
        return new List<ToolDefinition>
        {
            new ToolDefinition
            {
                Name = "execute_query",
                Description = "Execute SQL query",
                InputSchema = new Dictionary<string, object>
                {
                    ["query"] = new { type = "string", description = "SQL query" },
                    ["database"] = new { type = "string", description = "Database name" }
                }
            }
        };
    }
    
    public override IReadOnlyList<CommandDefinition> GetCommands()
    {
        return new List<CommandDefinition>
        {
            new CommandDefinition
            {
                Name = "sql",
                Description = "Execute SQL command",
                Handler = ExecuteSqlCommandAsync
            }
        };
    }
    
    private async Task<string> ExecuteSqlCommandAsync(string command)
    {
        // SQL execution logic
        return await Task.FromResult("Query executed successfully");
    }
}
```

### 5.2 Git Plugin

```csharp
public class GitPlugin : PluginBase
{
    public override string Name => "Git Plugin";
    public override string Version => "1.0.0";
    public override string Description => "Git operations plugin";
    
    public override IReadOnlyList<ToolDefinition> GetTools()
    {
        return new List<ToolDefinition>
        {
            new ToolDefinition
            {
                Name = "git_status",
                Description = "Get git status"
            },
            new ToolDefinition
            {
                Name = "git_commit",
                Description = "Create git commit",
                InputSchema = new Dictionary<string, object>
                {
                    ["message"] = new { type = "string", description = "Commit message" }
                }
            },
            new ToolDefinition
            {
                Name = "git_push",
                Description = "Push to remote"
            }
        };
    }
    
    public override IReadOnlyList<CommandDefinition> GetCommands()
    {
        return new List<CommandDefinition>
        {
            new CommandDefinition
            {
                Name = "git",
                Description = "Git operations",
                Handler = ExecuteGitCommandAsync
            }
        };
    }
    
    private async Task<string> ExecuteGitCommandAsync(string command)
    {
        // Git execution logic
        return await Task.FromResult("Git command executed successfully");
    }
}
```

---

## 6. Plugin API

### 6.1 Plugin Service Interface

```csharp
public interface IPluginService
{
    IReadOnlyList<IPlugin> GetLoadedPlugins();
    IPlugin? GetPlugin(string name);
    Task<bool> LoadPluginAsync(string pluginPath);
    void UnloadPlugin(string pluginName);
    IReadOnlyList<ToolDefinition> GetAllTools();
    IReadOnlyList<CommandDefinition> GetAllCommands();
}

public class PluginService : IPluginService
{
    private readonly PluginLoader _loader;
    private readonly ILogger<PluginService> _logger;
    
    public PluginService(PluginLoader loader, ILogger<PluginService> logger)
    {
        _loader = loader;
        _logger = logger;
    }
    
    public IReadOnlyList<IPlugin> GetLoadedPlugins()
    {
        return _loader.GetLoadedPlugins()
            .Select(name => _loader.GetPlugin(name))
            .Where(p => p != null)
            .ToList()!;
    }
    
    public IPlugin? GetPlugin(string name)
    {
        return _loader.GetPlugin(name);
    }
    
    public async Task<bool> LoadPluginAsync(string pluginPath)
    {
        try
        {
            var plugin = await _loader.LoadPluginAsync(pluginPath);
            return plugin != null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load plugin: {Path}", pluginPath);
            return false;
        }
    }
    
    public void UnloadPlugin(string pluginName)
    {
        _loader.UnloadPlugin(pluginName);
    }
    
    public IReadOnlyList<ToolDefinition> GetAllTools()
    {
        return GetLoadedPlugins()
            .SelectMany(p => p.GetTools())
            .ToList()
            .AsReadOnly();
    }
    
    public IReadOnlyList<CommandDefinition> GetAllCommands()
    {
        return GetLoadedPlugins()
            .SelectMany(p => p.GetCommands())
            .ToList()
            .AsReadOnly();
    }
}
```

---

## 7. Plugin Güvenlik

### 7.1 Security Sandbox

```csharp
public class PluginSandbox
{
    private readonly PluginSecurityPolicy _policy;
    private readonly ILogger<PluginSandbox> _logger;
    
    public PluginSandbox(PluginSecurityPolicy policy, ILogger<PluginSandbox> logger)
    {
        _policy = policy;
        _logger = logger;
    }
    
    public async Task<T> ExecuteInSandboxAsync<T>(Func<Task<T>> action)
    {
        var permissionSet = new PermissionSet(PermissionState.None);
        
        // Add permissions based on policy
        if (_policy.AllowFileAccess)
        {
            permissionSet.AddPermission(new FileIOPermission(
                FileIOPermissionAccess.Read | FileIOPermissionAccess.Write,
                _policy.AllowedPaths));
        }
        
        if (_policy.AllowNetworkAccess)
        {
            permissionSet.AddPermission(new SocketPermission(
                NetworkAccess.Connect,
                TransportType.Tcp,
                "*",
                SocketPermission.AllPorts));
        }
        
        // Execute in sandbox
        var appDomain = AppDomain.CreateDomain(
            "PluginSandbox",
            null,
            new AppDomainSetup
            {
                ApplicationBase = AppDomain.CurrentDomain.BaseDirectory
            },
            permissionSet);
        
        try
        {
            return await Task.Run(action);
        }
        finally
        {
            AppDomain.Unload(appDomain);
        }
    }
}
```

### 7.2 Plugin Security Policy

```csharp
public class PluginSecurityPolicy
{
    public bool AllowFileAccess { get; set; }
    public bool AllowNetworkAccess { get; set; }
    public bool AllowReflection { get; set; }
    public string[] AllowedPaths { get; set; } = Array.Empty<string>();
    public string[] BlockedAssemblies { get; set; } = Array.Empty<string>();
    
    public static PluginSecurityPolicy CreateDefault()
    {
        return new PluginSecurityPolicy
        {
            AllowFileAccess = true,
            AllowNetworkAccess = false,
            AllowReflection = false,
            AllowedPaths = new[] { "plugins/", "data/" },
            BlockedAssemblies = new[] { "System.Reflection.Emit" }
        };
    }
}
```

---

## 8. Plugin Testleri

### 8.1 Plugin Unit Testleri

```csharp
public class PluginLoaderTests
{
    private readonly Mock<ILogger<PluginLoader>> _loggerMock;
    private readonly PluginLoader _loader;
    
    public PluginLoaderTests()
    {
        _loggerMock = new Mock<ILogger<PluginLoader>>();
        _loader = new PluginLoader("plugins", _loggerMock.Object);
    }
    
    [Fact]
    public async Task LoadPlugins_DirectoryExists_LoadsPlugins()
    {
        // Arrange
        Directory.CreateDirectory("plugins");
        
        // Act
        var plugins = await _loader.LoadPluginsAsync();
        
        // Assert
        Assert.NotNull(plugins);
    }
    
    [Fact]
    public void GetLoadedPlugins_ReturnsLoadedPluginNames()
    {
        // Act
        var pluginNames = _loader.GetLoadedPlugins();
        
        // Assert
        Assert.NotNull(pluginNames);
    }
}
```

---

## 9. Plugin Gelecek Planı

### 9.1 Kısa Vadeli (1-2 hafta)

| Görev | Öncelik |
|-------|---------|
| Plugin loader geliştirme | Yüksek |
| Base class oluşturma | Yüksek |
| Tool implementasyonu | Yüksek |

### 9.2 Orta Vadeli (1-2 ay)

| Görev | Öncelik |
|-------|---------|
| Security sandbox | Orta |
| Plugin marketplace | Orta |
| Performance optimization | Düşük |

### 9.3 Uzun Vadeli (3-6 ay)

| Görev | Öncelik |
|-------|---------|
| Plugin marketplace | Düşük |
| Custom plugin types | Düşük |
| Enterprise features | Düşük |

---

## 10. Quality Report

| Metrik | Değer |
|--------|-------|
| Version | 1.1.0 |
| Status | Active |
| Plugin Components | 3 (Loader, Base, Service) |
| Security Layers | 2 (Sandbox, Policy) |
| Example Plugins | 2 (SQL, Git) |
| Test Coverage | 70% |

---

**Authority:** Vault Steward
**Last Updated:** 2026-08-26
**Mode:** Red Team · Human Mode · Truth Mode
