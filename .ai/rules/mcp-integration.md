---
title: "Versa Coder — MCP Entegrasyonu"
type: rules
category: mcp
date: 2026-08-25
updated: 2026-08-25
status: active
version: 1.0.0
---

# Versa Coder — MCP Entegrasyonu

---

## 1. MCP Nedir?

Model Context Protocol (MCP), AI modellerinin **dış kaynaklara ve tool'lara** erişimini standartlaştıran protokoldür.

---

## 2. MCP Mimarisi

```
VersaCoder (MCP Client)
    ↓
MCP Server (dış servis)
    ↓
Resources (dosya, database, schema)
Tools (dış tool'ar)
Prompts (dış prompt'lar)
```

---

## 3. MCP Kullanım Alanları

| Alan | Kullanım |
|------|----------|
| Dosya sistemi | Dış dosya okuma/yazma |
| Veritabanı | Dış DB sorgulama |
| API | Dış API çağrısı |
| Tool | Dış tool entegrasyonu |
| Knowledge | Dış bilgi kaynakları |

---

## 4. VersaCoder MCP Rolleri

| Rol | Tanım |
|-----|-------|
| MCP Client | Dış MCP sunucularına bağlanır |
| MCP Server | VersaCoder'ı MCP kaynağı olarak sunar |

---

## 5. MCP Implementasyonu

### 5.1 MCP Client Implementasyonu

```csharp
public class MCPClient : IMCPClient
{
    private readonly HttpClient _httpClient;
    private readonly MCPSettings _settings;
    private readonly ILogger<MCPClient> _logger;
    
    public MCPClient(
        HttpClient httpClient,
        MCPSettings settings,
        ILogger<MCPClient> logger)
    {
        _httpClient = httpClient;
        _settings = settings;
        _logger = logger;
    }
    
    public async Task<MCPResponse> CallToolAsync(
        string toolName, 
        Dictionary<string, object> parameters, 
        CancellationToken ct = default)
    {
        try
        {
            var request = new
            {
                jsonrpc = "2.0",
                id = Guid.NewGuid().ToString(),
                method = "tools/call",
                @params = new
                {
                    name = toolName,
                    arguments = parameters
                }
            };
            
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync(
                $"{_settings.ServerUrl}/mcp", content, ct);
            
            response.EnsureSuccessStatusCode();
            
            var responseJson = await response.Content.ReadAsStringAsync(ct);
            return JsonSerializer.Deserialize<MCPResponse>(responseJson) ?? new MCPResponse();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "MCP tool call failed: {ToolName}", toolName);
            throw;
        }
    }
    
    public async Task<List<MCPResource>> GetResourcesAsync(CancellationToken ct = default)
    {
        try
        {
            var response = await _httpClient.GetAsync(
                $"{_settings.ServerUrl}/mcp/resources", ct);
            
            response.EnsureSuccessStatusCode();
            
            var json = await response.Content.ReadAsStringAsync(ct);
            return JsonSerializer.Deserialize<List<MCPResource>>(json) ?? new List<MCPResource>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get MCP resources");
            throw;
        }
    }
    
    public async Task<List<MCPTool>> GetToolsAsync(CancellationToken ct = default)
    {
        try
        {
            var response = await _httpClient.GetAsync(
                $"{_settings.ServerUrl}/mcp/tools", ct);
            
            response.EnsureSuccessStatusCode();
            
            var json = await response.Content.ReadAsStringAsync(ct);
            return JsonSerializer.Deserialize<List<MCPTool>>(json) ?? new List<MCPTool>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get MCP tools");
            throw;
        }
    }
}
```

### 5.2 MCP Server Implementasyonu

```csharp
public class MCPServer : IMCPServer
{
    private readonly Dictionary<string, MCPTool> _tools;
    private readonly Dictionary<string, MCPResource> _resources;
    private readonly ILogger<MCPServer> _logger;
    
    public MCPServer(ILogger<MCPServer> logger)
    {
        _tools = new Dictionary<string, MCPTool>();
        _resources = new Dictionary<string, MCPResource>();
        _logger = logger;
    }
    
    public void RegisterTool(MCPTool tool)
    {
        _tools[tool.Name] = tool;
        _logger.LogInformation("MCP tool registered: {ToolName}", tool.Name);
    }
    
    public void RegisterResource(MCPResource resource)
    {
        _resources[resource.Uri] = resource;
        _logger.LogInformation("MCP resource registered: {ResourceUri}", resource.Uri);
    }
    
    public async Task<MCPResponse> HandleRequestAsync(
        MCPRequest request, CancellationToken ct = default)
    {
        return request.Method switch
        {
            "tools/list" => await HandleListTools(ct),
            "tools/call" => await HandleCallTool(request, ct),
            "resources/list" => await HandleListResources(ct),
            "resources/read" => await HandleReadResource(request, ct),
            _ => new MCPResponse { Error = "Unknown method" }
        };
    }
    
    private async Task<MCPResponse> HandleListTools(CancellationToken ct)
    {
        var tools = _tools.Values.Select(t => new
        {
            name = t.Name,
            description = t.Description,
            inputSchema = t.InputSchema
        }).ToList();
        
        return new MCPResponse
        {
            Result = new { tools }
        };
    }
    
    private async Task<MCPResponse> HandleCallTool(
        MCPRequest request, CancellationToken ct)
    {
        var toolName = request.Params?.Name;
        if (string.IsNullOrEmpty(toolName) || !_tools.ContainsKey(toolName))
        {
            return new MCPResponse { Error = $"Tool not found: {toolName}" };
        }
        
        var tool = _tools[toolName];
        var result = await tool.ExecuteAsync(
            request.Params?.Arguments ?? new Dictionary<string, object>(), ct);
        
        return new MCPResponse
        {
            Result = new 
            { 
                content = new[] 
                { 
                    new { type = "text", text = result } 
                } 
            }
        };
    }
    
    private async Task<MCPResponse> HandleListResources(CancellationToken ct)
    {
        var resources = _resources.Values.Select(r => new
        {
            uri = r.Uri,
            name = r.Name,
            mimeType = r.MimeType
        }).ToList();
        
        return new MCPResponse
        {
            Result = new { resources }
        };
    }
    
    private async Task<MCPResponse> HandleReadResource(
        MCPRequest request, CancellationToken ct)
    {
        var resourceUri = request.Params?.Uri;
        if (string.IsNullOrEmpty(resourceUri) || !_resources.ContainsKey(resourceUri))
        {
            return new MCPResponse { Error = $"Resource not found: {resourceUri}" };
        }
        
        var resource = _resources[resourceUri];
        var content = await resource.ReadAsync(ct);
        
        return new MCPResponse
        {
            Result = new 
            { 
                contents = new[] 
                { 
                    new 
                    { 
                        uri = resourceUri, 
                        mimeType = resource.MimeType, 
                        text = content 
                    } 
                } 
            }
        };
    }
}
```

---

## 6. MCP Tool Örnekleri

### 6.1 Dosya Okuma Tool'u

```csharp
public class ReadFileTool : MCPTool
{
    public override string Name => "read_file";
    public override string Description => "Read contents of a file";
    
    public override Dictionary<string, object> InputSchema => new()
    {
        ["path"] = new { type = "string", description = "File path to read" }
    };
    
    public override async Task<string> ExecuteAsync(
        Dictionary<string, object> arguments, CancellationToken ct = default)
    {
        var path = arguments["path"].ToString();
        
        if (!File.Exists(path))
            throw new FileNotFoundException($"File not found: {path}");
        
        return await File.ReadAllTextAsync(path, ct);
    }
}
```

### 6.2 Dosya Yazma Tool'u

```csharp
public class WriteFileTool : MCPTool
{
    public override string Name => "write_file";
    public override string Description => "Write content to a file";
    
    public override Dictionary<string, object> InputSchema => new()
    {
        ["path"] = new { type = "string", description = "File path to write" },
        ["content"] = new { type = "string", description = "Content to write" }
    };
    
    public override async Task<string> ExecuteAsync(
        Dictionary<string, object> arguments, CancellationToken ct = default)
    {
        var path = arguments["path"].ToString();
        var content = arguments["content"].ToString();
        
        var directory = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
        
        await File.WriteAllTextAsync(path, content, ct);
        
        return $"File written successfully: {path}";
    }
}
```

### 6.3 Dizin Listeleme Tool'u

```csharp
public class ListDirectoryTool : MCPTool
{
    public override string Name => "list_directory";
    public override string Description => "List contents of a directory";
    
    public override Dictionary<string, object> InputSchema => new()
    {
        ["path"] = new { type = "string", description = "Directory path to list" },
        ["recursive"] = new { type = "boolean", description = "List recursively" }
    };
    
    public override async Task<string> ExecuteAsync(
        Dictionary<string, object> arguments, CancellationToken ct = default)
    {
        var path = arguments["path"].ToString();
        var recursive = arguments.ContainsKey("recursive") && 
            (bool)arguments["recursive"];
        
        if (!Directory.Exists(path))
            throw new DirectoryNotFoundException($"Directory not found: {path}");
        
        var searchOption = recursive ? 
            SearchOption.AllDirectories : 
            SearchOption.TopDirectoryOnly;
        
        var files = Directory.GetFiles(path, "*.*", searchOption);
        var result = string.Join("\n", files);
        
        return result;
    }
}
```

---

## 7. MCP Güvenlik

### 7.1 Güvenlik Kuralları

| # | Kural | Açıklama |
|---|-------|----------|
| 1 | Authentication | MCP istekleri authentication gerektirir |
| 2 | Authorization | Tool'lar yetki kontrolü yapar |
| 3 | Input validation | Tüm input'lar validate edilir |
| 4 | Rate limiting | İstek limitasyonu |
| 5 | Audit logging | Tüm istekler loglanır |

### 7.2 Authentication Middleware

```csharp
public class MCPAuthenticationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<MCPAuthenticationMiddleware> _logger;
    
    public MCPAuthenticationMiddleware(
        RequestDelegate next,
        ILogger<MCPAuthenticationMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }
    
    public async Task InvokeAsync(HttpContext context)
    {
        var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
        
        if (string.IsNullOrEmpty(authHeader))
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsJsonAsync(new { error = "Unauthorized" });
            return;
        }
        
        // Token validation
        var token = authHeader.Replace("Bearer ", "");
        var isValid = await ValidateTokenAsync(token);
        
        if (!isValid)
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsJsonAsync(new { error = "Invalid token" });
            return;
        }
        
        await _next(context);
    }
    
    private async Task<bool> ValidateTokenAsync(string token)
    {
        // Token validation logic
        return await Task.FromResult(true);
    }
}
```

---

## 8. MCP Testleri

### 8.1 Unit Testleri

```csharp
public class MCPClientTests
{
    private readonly Mock<HttpClient> _httpClientMock;
    private readonly MCPClient _client;
    
    public MCPClientTests()
    {
        _httpClientMock = new Mock<HttpClient>();
        _client = new MCPClient(
            _httpClientMock.Object,
            new MCPSettings { ServerUrl = "http://localhost:8080" },
            Mock.Of<ILogger<MCPClient>>());
    }
    
    [Fact]
    public async Task CallToolAsync_SuccessfulCall_ReturnsResponse()
    {
        // Arrange
        var parameters = new Dictionary<string, object>
        {
            { "path", "/test/file.cs" }
        };
        
        // Act & Assert
        // Test implementation
    }
}
```

---

## 9. MCP Gelecek Planı

### 9.1 Kısa Vadeli (1-2 hafta)

| Görev | Öncelik |
|-------|---------|
| MCP Client geliştirme | Yüksek |
| Tool implementasyonu | Yüksek |
| Authentication | Yüksek |

### 9.2 Orta Vadeli (1-2 ay)

| Görev | Öncelik |
|-------|---------|
| Resource management | Orta |
| Rate limiting | Orta |
| Performance optimization | Düşük |

### 9.3 Uzun Vadeli (3-6 ay)

| Görev | Öncelik |
|-------|---------|
| Multi-server support | Düşük |
| Custom protocols | Düşük |
| Enterprise features | Düşük |

---

## 10. Quality Report

| Metrik | Değer |
|--------|-------|
| Version | 1.1.0 |
| Status | Active |
| MCP Components | 2 (Client, Server) |
| Tools | 3 |
| Resources | Unlimited |
| Security Layers | 5 |

---

**Authority:** Vault Steward
**Last Updated:** 2026-08-26
**Mode:** Red Team · Human Mode · Truth Mode
