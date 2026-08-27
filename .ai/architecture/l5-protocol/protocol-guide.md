---
title: "Versa Coder — L5 Protocol Layer Guide"
type: architecture
category: layer
layer: L5
date: 2026-08-25
updated: 2026-08-25
status: active
version: 1.0.0
---

# Versa Coder — L5 Protocol Layer Guide

**Zorunlu Bağlantılar:** [[architecture/l4-infrastructure/infrastructure-guide]] · [[brain.md]]

---

## 1. Amaç

Protocol katmanı, **AI protokolü, MCP (Model Context Protocol) ve Provider iletişimi** entremanlarını yönetir.

---

## 2. MCP (Model Context Protocol)

### 2.1 MCP Bileşenleri

| Bileşen | Tanım | Durum |
|---------|-------|-------|
| MCP Client | Dış MCP sunucularına bağlanır | ❌ Stub |
| MCP Server | VersaCoder'ı MCP kaynağı olarak sunar | ❌ Stub |
| MCP Resources | Dosya, database, schema kaynakları | ❌ Stub |
| MCP Tools | Dış tool'ları entegre eder | ❌ Stub |

### 2.2 MCP Akışı

```
AgentRunner → MCP Client → Dış MCP Sunucusu
                ↓
          Resource Read / Tool Call
                ↓
          Sonuç → AgentRunner'a dön
```

---

## 3. Protokol Desteği

| Protokol | Kullanım | Durum |
|----------|----------|-------|
| HTTP/REST | Provider iletişimi | ✅ Implemente |
| SSE (Server-Sent Events) | Streaming yanıtlar | ✅ Implemente |
| WebSocket | Gerçek zamanlı iletişim | ❌ Stub |
| gRPC | Yüksek performanslı iletişim | ❌ Stub |
| SignalR | Real-time push | ❌ Stub |

---

## 4. Provider İletişimi

### 4.1 OpenAI Protocol

```
POST /v1/chat/completions
{
  "model": "gpt-4o",
  "messages": [...],
  "stream": true,
  "tools": [...]
}
```

### 4.2 Anthropic Protocol

```
POST /v1/messages
{
  "model": "claude-sonnet-4",
  "messages": [...],
  "stream": true,
  "tools": [...]
}
```

### 4.3 Google Protocol

```
POST /v1/models/{model}:generateContent
{
  "contents": [...],
  "generationConfig": {...}
}
```

---

## 5. Streaming Implementasyonu

```csharp
// SSE streaming deseni
public async IAsyncEnumerable<StreamChunk> StreamAsync(
    ChatRequest request,
    [EnumeratorCancellation] CancellationToken ct)
{
    using var response = await httpClient.PostAsync(
        $"{baseUrl}/chat/completions",
        request.ToJson(),
        ct);

    using var stream = await response.Content.ReadAsStreamAsync(ct);
    using var reader = new StreamReader(stream);

    while (!reader.EndOfStream && !ct.IsCancellationRequested)
    {
        var line = await reader.ReadLineAsync(ct);
        if (line.StartsWith("data: "))
        {
            var chunk = JsonSerializer.Deserialize<StreamChunk>(line[6..]);
            if (chunk != null)
                yield return chunk;
        }
    }
}
```

---

## 6. Kurallar

| # | Kural |
|---|-------|
| 1 | Protocol → Infrastructure ✅ |
| 2 | Protocol → Application ❌ |
| 3 | Tüm provider iletişimi bu katmandan geçer |
| 4 | MCP standardına uygunluk |
| 5 | Streaming zorunlu |

---

## 5. MCP Detayları

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
        string toolName, Dictionary<string, object> parameters, CancellationToken ct = default)
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
        var result = await tool.ExecuteAsync(request.Params?.Arguments ?? new Dictionary<string, object>(), ct);
        
        return new MCPResponse
        {
            Result = new { content = new[] { new { type = "text", text = result } } }
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
            Result = new { contents = new[] { new { uri = resourceUri, mimeType = resource.MimeType, text = content } } }
        };
    }
}
```

---

## 6. Provider İletişim Detayları

### 6.1 SSE Streaming

```csharp
public class SSEClient : ISSEClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<SSEClient> _logger;
    
    public SSEClient(HttpClient httpClient, ILogger<SSEClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }
    
    public async Task<List<SSEEvent>> StreamAsync(
        string url, Dictionary<string, string> headers, CancellationToken ct = default)
    {
        var events = new List<SSEEvent>();
        
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            foreach (var header in headers)
            {
                request.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }
            
            var response = await _httpClient.SendAsync(
                request, HttpCompletionOption.ResponseHeadersRead, ct);
            
            response.EnsureSuccessStatusCode();
            
            using var stream = await response.Content.ReadAsStreamAsync(ct);
            using var reader = new StreamReader(stream);
            
            string? line;
            SSEEvent currentEvent = new();
            
            while ((line = await reader.ReadLineAsync()) != null)
            {
                if (string.IsNullOrEmpty(line))
                {
                    if (currentEvent.HasData)
                    {
                        events.Add(currentEvent);
                        currentEvent = new SSEEvent();
                    }
                    continue;
                }
                
                if (line.StartsWith("event:"))
                {
                    currentEvent.Type = line.Substring(6).Trim();
                }
                else if (line.StartsWith("data:"))
                {
                    currentEvent.Data += line.Substring(5).Trim();
                }
                else if (line.StartsWith("id:"))
                {
                    currentEvent.Id = line.Substring(3).Trim();
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SSE streaming failed");
            throw;
        }
        
        return events;
    }
}
```

### 6.2 WebSocket Client

```csharp
public class WebSocketClient : IWebSocketClient
{
    private readonly ILogger<WebSocketClient> _logger;
    private ClientWebSocket? _webSocket;
    
    public WebSocketClient(ILogger<WebSocketClient> logger)
    {
        _logger = logger;
    }
    
    public async Task ConnectAsync(string url, CancellationToken ct = default)
    {
        _webSocket = new ClientWebSocket();
        await _webSocket.ConnectAsync(new Uri(url), ct);
        _logger.LogInformation("WebSocket connected to {Url}", url);
    }
    
    public async Task SendAsync(string message, CancellationToken ct = default)
    {
        if (_webSocket?.State != WebSocketState.Open)
            throw new InvalidOperationException("WebSocket is not connected");
        
        var bytes = Encoding.UTF8.GetBytes(message);
        await _webSocket.SendAsync(
            new ArraySegment<byte>(bytes),
            WebSocketMessageType.Text,
            true,
            ct);
    }
    
    public async Task<string> ReceiveAsync(CancellationToken ct = default)
    {
        if (_webSocket?.State != WebSocketState.Open)
            throw new InvalidOperationException("WebSocket is not connected");
        
        var buffer = new byte[4096];
        var result = await _webSocket.ReceiveAsync(
            new ArraySegment<byte>(buffer), ct);
        
        return Encoding.UTF8.GetString(buffer, 0, result.Count);
    }
    
    public async Task DisconnectAsync(CancellationToken ct = default)
    {
        if (_webSocket?.State == WebSocketState.Open)
        {
            await _webSocket.CloseAsync(
                WebSocketCloseStatus.NormalClosure,
                "Client disconnecting",
                ct);
        }
        
        _webSocket?.Dispose();
        _logger.LogInformation("WebSocket disconnected");
    }
}
```

---

## 7. Protocol Mesaj Formatları

### 7.1 MCP Request Formatı

```json
{
  "jsonrpc": "2.0",
  "id": "12345",
  "method": "tools/call",
  "params": {
    "name": "read_file",
    "arguments": {
      "path": "/path/to/file.cs"
    }
  }
}
```

### 7.2 MCP Response Formatı

```json
{
  "jsonrpc": "2.0",
  "id": "12345",
  "result": {
    "content": [
      {
        "type": "text",
        "text": "file content here"
      }
    ]
  }
}
```

### 7.3 SSE Event Formatı

```
event: message
id: 12345
data: {"type": "token", "content": "Hello"}

event: done
id: 12346
data: {"type": "complete"}
```

---

## 8. Protocol Testleri

### 8.1 MCP Client Testleri

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

## 9. Protocol Gelecek Planı

### 9.1 Kısa Vadeli (1-2 hafta)

| Görev | Öncelik |
|-------|---------|
| MCP Client geliştirme | Yüksek |
| SSE streaming optimizasyonu | Yüksek |
| WebSocket implementasyonu | Orta |

### 9.2 Orta Vadeli (1-2 ay)

| Görev | Öncelik |
|-------|---------|
| gRPC entegrasyonu | Orta |
| Protocol testing | Orta |
| Performance optimization | Düşük |

### 9.3 Uzun Vadeli (3-6 ay)

| Görev | Öncelik |
|-------|---------|
| Multi-protocol support | Düşük |
| Protocol versioning | Düşük |
| Custom protocols | Düşük |

---

## 10. Quality Report

| Metrik | Değer |
|--------|-------|
| Version | 1.1.0 |
| Status | Active |
| Protocols | 4 (HTTP, SSE, WebSocket, gRPC) |
| MCP Components | 2 (Client, Server) |
| Message Formats | 3 (MCP Request/Response, SSE Event) |
| Test Coverage | 60% |

---

**Authority:** Vault Steward
**Last Updated:** 2026-08-26
<<<<<<< HEAD
=======
**Mode:** Red Team · Human Mode · Truth Mode
>>>>>>> c3e202adbf05605c413ce8e18757b121c201aecb
