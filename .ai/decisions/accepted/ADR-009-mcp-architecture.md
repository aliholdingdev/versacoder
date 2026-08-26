---
title: "ADR-009 — MCP (Model Context Protocol) Architecture"
type: decision
status: accepted
date: 2026-08-25
version: 1.0.0
---

# ADR-009 — MCP Architecture

**Status:** Accepted  
**Date:** 2026-08-25  
**Category:** Infrastructure.MCP  
**Sorumlu:** Build Agent

---

## 1. Karar

Versa Coder, hem **MCP Client** hem de **MCP Server** olarak çalışacak, **stdio** ve **SSE** transport'ları destekleyen bir MCP implementasyonu kullanacaktır.

## 2. Bağlam

Model Context Protocol (MCP), AI modellerinin harici araçlara ve kaynaklara erişmesini sağlayan açık bir protokoldür. Versa Coder:
- MCP Client olarak: Harici MCP server'lara bağlanmalı
- MCP Server olarak: Kendi araçlarını ve kaynaklarını sunmalı

## 3. MCP Server (Versa Coder'ın sunduğu)

```csharp
public class VersaCoderMcpServer
{
    private readonly ToolRegistry _toolRegistry;
    private readonly ContextStorage _contextStorage;
    
    // Resources: Versa Coder'ın sunduğu kaynaklar
    public List<McpResource> GetResources()
    {
        return new List<McpResource>
        {
            new("versacoder://project/structure", "Project Structure"),
            new("versacoder://session/current", "Current Session"),
            new("versacoder://context/active", "Active Context"),
            new("versacoder://vault/rules", "Vault Rules")
        };
    }
    
    // Tools: Versa Coder'ın sunduğu araçlar
    public List<McpTool> GetTools()
    {
        return new List<McpTool>
        {
            new("read_file", "Read file contents"),
            new("write_file", "Write file contents"),
            new("search_code", "Search codebase"),
            new("run_tests", "Run test suite"),
            new("get_context", "Get assembled context")
        };
    }
}
```

## 4. MCP Client (Versa Coder'ın bağlandığı)

```csharp
public class VersaCoderMcpClient
{
    private readonly Dictionary<string, IMcpTransport> _transports;
    
    public async Task<McpResponse> CallToolAsync(
        string serverName,
        string toolName,
        Dictionary<string, object> parameters)
    {
        var transport = _transports[serverName];
        var request = new McpToolCallRequest
        {
            Tool = toolName,
            Arguments = parameters
        };
        
        return await transport.SendRequestAsync(request);
    }
    
    public async Task<List<McpResource>> ListResourcesAsync(
        string serverName)
    {
        var transport = _transports[serverName];
        return await transport.ListResourcesAsync();
    }
}
```

## 5. Transport Seçenekleri

| Transport | Kullanım | Avantaj |
|-----------|----------|---------|
| **stdio** | Yerel process | Hızlı, basit |
| **SSE** | HTTP over network | Uzak erişim |
| **WebSocket** | Gerçek zamanlı | Düşük gecikme |

## 6. Konfigürasyon

```json
{
  "MCP": {
    "Server": {
      "Enabled": true,
      "Name": "versacoder",
      "Transport": "stdio"
    },
    "Clients": [
      {
        "Name": "filesystem",
        "Transport": "stdio",
        "Command": "npx",
        "Args": ["-y", "@anthropic/mcp-filesystem"]
      },
      {
        "Name": "github",
        "Transport": "sse",
        "Url": "http://localhost:3001/sse"
      }
    ]
  }
}
```

---

**Authority:** Vault Steward  
**Last Updated:** 2026-08-25
