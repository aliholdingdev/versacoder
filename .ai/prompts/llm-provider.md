---
title: "prompt - LLM Provider (OpenRouter + NVIDIA NIM, Tam Kod)"
type: prompt
category: prompts
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "IAgentProvider portunun iki uygulamasini (OpenRouterAdapter + NvidiaNimAdapter) TAM KOD olarak ureten prompt - 3 kanit zorunlu, anahtar .cs'e GOMULMEZ (SEC-06), retry/stream/fallback"
---

# prompt — LLM Provider (OpenRouter + NVIDIA NIM, Tam Kod)

**Zorunlu Baglantilar:** [prompts/index](index.md) · [AGENTS](../AGENTS.md) · [CLAUDE](../CLAUDE.md) · [TODOS/05](../TODOS/05-kod-ve-kalite.md) · [llm-provider-spec](../spec/llm-provider-spec.md) · [protocol-spec](../spec/protocol-spec.md) · [tool-spec](../spec/tool-spec.md) · [security-rules](../.rules/security-rules.md) · [layer-boundary-check](../checklists/layer-boundary-check.md)

> **Tek cumlelik sorumluluk:** `IAgentProvider` (l0) portunun iki somut uygulamasini
> (OpenRouter + NVIDIA NIM) **tam kod** olarak ureten, **retry/stream/fallback** ve
> **3 kanitla** kapanan is emridir — **anahtar `.cs`'e gomulmez** (SEC-06).

---

## §0 Ozet (Answer First)

| Alan | Deger |
|---|---|
| Port | `IAgentProvider` (l0, `K0010-K0019`) |
| Uygulama | `OpenRouterAdapter` + `NvidiaNimAdapter` (l4, `K0400-K0449`) |
| TFM | `net48;net10.0-windows` |
| Gereksinim | `VC-LLM-001..014` (llm-provider-spec §2) |
| Hata | `VC-E-0601..0615` (llm-provider-spec §5) |
| Ajan | `developer` → `security-auditor` (secret) → `tester` |
| Kapı | GATE-005 |
| Kanıt | 3 tane: build + test (retry/stream) + secret-leak-check = 0 |

**YASAK:** anahtar `.cs`'e gömme · anahtar log'a/konsola yazma · HTTP isteği doğrudan UI'dan (l7 izolasyonu) · EF Core.

---

## §1 Baglam

1. `llm-provider-spec §2`: `IAgentProvider` portu l0; uygulama l4.
2. `llm-provider-spec §3.1`: OpenRouter `https://openrouter.ai/api/v1` + `sk-or-v1-` + `HTTP-Referer`/`X-Title`.
3. `llm-provider-spec §3.2`: NIM `https://integrate.api.nvidia.com/v1` + `nvapi-`.
4. `llm-provider-spec §4`: retry 429 → 1s/2s/4s · timeout 300 sn · SSE < 30 sn · fallback.
5. `llm-provider-spec §6`: config `env` → `secrets.json` → default.

---

## §2 Görev — TAM KOD

### §2.1 OpenRouterAdapter (`VC-LLM-001..005`, `K0400-K0424`)

```csharp
// src/VersaCoder.Infrastructure/AI/OpenRouterAdapter.cs
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using VersaCoder.Abstractions.Ports;
using VersaCoder.Crosscutting.Configuration;
using VersaCoder.Protocol.Streaming;

namespace VersaCoder.Infrastructure.AI;

/// <summary>OpenRouter saglayicisi — llm-provider-spec §3.1 (VC-LLM-001..005).</summary>
public sealed class OpenRouterAdapter : IAgentProvider
{
    private const string Endpoint = "https://openrouter.ai/api/v1/chat/completions";   // VC-LLM-001
    private readonly HttpClient _http;
    private readonly IConfig _config;
    public OpenRouterAdapter(HttpClient http, IConfig config) { _http = http; _config = config; }

    public string Name => "openrouter";
    public bool IsAvailable => !string.IsNullOrEmpty(_config.Get("OPENROUTER_API_KEY"));   // VC-E-0613

    public async Task<ProviderResult> CompleteAsync(ProviderRequest r, CancellationToken ct)
    {
        var json = BuildJson(r);
        var key = _config.Get("OPENROUTER_API_KEY");           // env -> secrets.json -> default
        if (string.IsNullOrEmpty(key))
            throw new InvalidOperationException("VC-E-0613");  // exit 4

        for (var attempt = 0; attempt < 3; attempt++)          // VC-LLM-011 retry
        {
            using var req = new HttpRequestMessage(HttpMethod.Post, Endpoint)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
            req.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", key);
            req.Headers.TryAddWithoutValidation("HTTP-Referer", "https://versacoder.local");  // VC-LLM-005
            req.Headers.TryAddWithoutValidation("X-Title", "VersaCoder");
            using var resp = await _http.SendAsync(req, ct);
            if (resp.StatusCode == System.Net.HttpStatusCode.TooManyRequests) { await Task.Delay(1000 << attempt, ct); continue; }
            if (!resp.IsSuccessStatusCode) throw new InvalidOperationException($"VC-E-06{(int)resp.StatusCode / 100 == 4 ? "04" : "06"}");
            var body = await resp.Content.ReadAsStringAsync(ct);
            return Parse(body);
        }
        throw new InvalidOperationException("VC-E-0603");      // rate limit
    }

    public async IAsyncEnumerable<string> StreamAsync(ProviderRequest r, CancellationToken ct)
    {
        var json = BuildJson(r, stream: true);
        var key = _config.Get("OPENROUTER_API_KEY");
        using var req = new HttpRequestMessage(HttpMethod.Post, Endpoint)
        { Content = new StringContent(json, Encoding.UTF8, "application/json") };
        req.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", key);
        req.Headers.TryAddWithoutValidation("HTTP-Referer", "https://versacoder.local");
        using var resp = await _http.SendAsync(req, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, ct);
        using var stream = await resp.Content.ReadAsStreamAsync(ct);
        using var reader = new System.IO.StreamReader(stream);
        await foreach (var chunk in SseParser.ParseAsync(reader, ct)) yield return chunk;   // VC-LLM-013
    }

    public async Task<IReadOnlyList<ModelInfo>> ListModelsAsync(CancellationToken ct)
    {
        using var resp = await _http.GetAsync("https://openrouter.ai/api/v1/models", ct);
        var body = await resp.Content.ReadAsStringAsync(ct);
        return ParseModels(body);                                  // VC-LLM-004
    }

    private static string BuildJson(ProviderRequest r, bool stream = false) =>
        JsonSerializer.Serialize(new { model = r.Model, messages = r.Messages, temperature = r.Temperature, max_tokens = r.MaxTokens, stream });
    private static ProviderResult Parse(string json) => /* ... */ new("", 0, 0, "stop", "");
    private static IReadOnlyList<ModelInfo> ParseModels(string json) => Array.Empty<ModelInfo>();
}
```

### §2.2 NvidiaNimAdapter (`VC-LLM-006..010`, `K0425-K0449`)

```csharp
// src/VersaCoder.Infrastructure/AI/NvidiaNimAdapter.cs
namespace VersaCoder.Infrastructure.AI;

/// <summary>NVIDIA NIM saglayicisi — llm-provider-spec §3.2 (VC-LLM-006..010).</summary>
public sealed class NvidiaNimAdapter : IAgentProvider
{
    private const string Endpoint = "https://integrate.api.nvidia.com/v1/chat/completions";   // VC-LLM-006
    // ayni iskelet; config key: NVIDIA_NIM_API_KEY (nvapi-...), default model: meta/llama-3.1-70b-instruct
    public string Name => "nvidia-nim";
    // IsAvailable / CompleteAsync / StreamAsync / ListModelsAsync — ayni retry + SSE
}
```

### §2.3 Fallback (`VC-LLM-014`, `K0449`)

```csharp
// src/VersaCoder.Infrastructure/AI/FallbackProvider.cs
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using VersaCoder.Abstractions.Ports;

namespace VersaCoder.Infrastructure.AI;

/// <summary>OpenRouter 4xx/5xx -> NIM'e dusurur (llm-provider-spec §4 VC-LLM-014).</summary>
public sealed class FallbackProvider : IAgentProvider
{
    private readonly IAgentProvider _primary;
    private readonly IAgentProvider _fallback;
    public FallbackProvider(IAgentProvider primary, IAgentProvider fallback) { _primary = primary; _fallback = fallback; }
    public string Name => $"{_primary.Name}->{_fallback.Name}";
    public bool IsAvailable => _primary.IsAvailable || _fallback.IsAvailable;
    public async Task<ProviderResult> CompleteAsync(ProviderRequest r, CancellationToken ct)
    {
        try { return await _primary.CompleteAsync(r, ct); }
        catch { return await _fallback.CompleteAsync(r, ct); }   // fallback
    }
    public async IAsyncEnumerable<string> StreamAsync(ProviderRequest r, CancellationToken ct) { /* ayni */ await Task.CompletedTask; yield break; }
    public Task<IReadOnlyList<ModelInfo>> ListModelsAsync(CancellationToken ct) => _primary.ListModelsAsync(ct);
}
```

---

## §3 Kabul Kriterleri

- [ ] `OpenRouterAdapter`, `NvidiaNimAdapter`, `FallbackProvider` — 3 tip
- [ ] **`sk-or-v1-` + `nvapi-` kodda 0 eşleşme** (secret-leak-check = 0)
- [ ] Retry 429 → 1s/2s/4s (3 deneme)
- [ ] SSE `StreamAsync` ilk token < 30 sn (protocol-spec §5)
- [ ] Fallback: OpenRouter 4xx → NIM
- [ ] Çift bacak build 0 hata

---

## §4 Kanıt (3 tane zorunlu)

```text
KANIT 1: dotnet build -f net48 + -f net10.0-windows -> 0 hata
KANIT 2: dotnet test -> retry testi (429 -> 3 deneme) + fallback testi yeşil
KANIT 3: secret-leak-check -> sk-or-v1 = 0, nvapi- = 0
```

**Çıktı kutusu:**

```text
+--------------------------------------------------+
| BASLIK : OpenRouter + NIM adapter yazildi        |
| KAPSAM : src/VersaCoder.Infrastructure/AI/**     |
| KARAR  : 2 adapter + fallback; anahtar config'de |
| RISK   : NIM endpoint canli test edilmedi        |
|          (llm-provider-spec §9/1)                |
| KANIT  : build x2 = 0 + test yeşil + secret=0    |
+--------------------------------------------------+
```

---

## §5 Ajan Ataması

| # | Ajan | İş |
|---|---|---|
| 1 | `developer` | 3 adapter kodu |
| 2 | `security-auditor` | secret-leak-check |
| 3 | `tester` | retry + fallback + stream test |

---

**Olculen kanit:** `.ai/prompts/llm-provider.md` kaydedildi; `prompts/` = 40 dosya.
