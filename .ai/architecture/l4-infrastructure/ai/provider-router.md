---
title: "Versa Coder — Provider Router Mimarisi"
type: architecture
category: ai
date: 2026-08-25
updated: 2026-08-25
status: active
version: 1.0.0
---

# Versa Coder — Provider Router Mimarisi

**Zorunlu Bağlantılar:** [[architecture/l4-infrastructure/ai/agent-runner]] · [[architecture/l4-infrastructure/ai/tool-system]] · [[brain.md]]

---

## 1. Amaç

Tüm LLM sağlayıcılarını (**OpenAI, Anthropic, Google, Ollama, Özel**) tek bir arayüz altında birleştiren, **çoklu provider routing, fallback ve streaming** destekleyen provider yönlendirme sistemi.

---

## 2. Provider Akışı

```
AgentRunner → ProviderRouter → Sağlayıcı Seçimi → LLM Provider → Yanıt
                                    ↓
                              ┌─────────────┐
                              │ OpenAI      │ ← Birincil
                              │ Anthropic   │ ← İkincil
                              │ Google      │ ← Üçüncül
                              │ Ollama      │ ← Yerel
                              │ Özel        │ ← Özel endpoint
                              └─────────────┘
                                    ↓
                              Fallback (hata durumunda)
```

---

## 3. Provider Konfigürasyonu

```json
{
  "AI": {
    "Providers": {
      "OpenAI": {
        "ApiKey": "${OPENAI_API_KEY}",
        "BaseUrl": "https://api.openai.com/v1",
        "Models": ["gpt-4o", "gpt-4.1", "o3"],
        "DefaultModel": "gpt-4o",
        "Timeout": 30,
        "MaxRetries": 3
      },
      "Anthropic": {
        "ApiKey": "${ANTHROPIC_API_KEY}",
        "BaseUrl": "https://api.anthropic.com/v1",
        "Models": ["claude-opus-4", "claude-sonnet-4"],
        "DefaultModel": "claude-sonnet-4"
      },
      "Google": {
        "ApiKey": "${GOOGLE_AI_API_KEY}",
        "Models": ["gemini-2.5-pro", "gemini-2.5-flash"]
      },
      "Ollama": {
        "BaseUrl": "http://localhost:11434",
        "Models": ["llama3.1", "qwen2.5", "codellama"]
      }
    }
  }
}
```

---

## 4. Routing Kuralları

| Kural | Öncelik | Açıklama |
|-------|---------|----------|
| Cost optimization | 1 | Basit görevler için ucuz model |
| Task-based routing | 2 | Görev türüne göre model seçimi |
| Fallback | 3 | Provider hatasında yedek sağlayıcı |
| Cache | 4 | Semantic cache ile tekrar sorguları önbelleğe alma |
| Rate limiting | 5 | Provider bazlı istek sınırlandırma |

---

## 5. Streaming SSE

```csharp
// Streaming deseni
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

## 6. Fallback Stratejisi

```
OpenAI (Birincil)
    ↓ Hata
Anthropic (İkincil)
    ↓ Hata
Google (Üçüncül)
    ↓ Hata
Ollama (Yerel — son çare)
```

| Hata Tipi | Aksiyon |
|-----------|---------|
| 429 Rate Limit | 5s bekle, retry |
| 500 Server Error | Fallback provider'a geç |
| 408 Timeout | Fallback provider'a geç |
| 401 Unauthorized | Log hatası, fallback |

---

**Authority:** Vault Steward
**Last Updated:** 2026-08-25
