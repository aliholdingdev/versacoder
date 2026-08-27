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

**Authority:** Vault Steward
**Last Updated:** 2026-08-26
