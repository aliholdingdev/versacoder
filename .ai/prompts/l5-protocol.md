---
title: "prompt - l5 Protocol (JSON-RPC + SSE, Tam Kod)"
type: prompt
category: prompts
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "l5 Protocol katmanini (JSON-RPC sozlesmesi, SSE streaming, tool-call parse, hata modeli VC-E-05xx/06xx) TAM KOD olarak ureten prompt - 3 kanit zorunlu, round-trip testi"
---

# prompt — l5 Protocol (JSON-RPC + SSE, Tam Kod)

**Zorunlu Baglantilar:** [prompts/index](index.md) · [AGENTS](../AGENTS.md) · [CLAUDE](../CLAUDE.md) · [TODOS/05](../TODOS/05-kod-ve-kalite.md) · [protocol-spec](../spec/protocol-spec.md) · [tool-spec](../spec/tool-spec.md) · [llm-provider-spec](../spec/llm-provider-spec.md) · [layer-boundary-check](../checklists/layer-boundary-check.md) · [ADR-028](../.decisions/accepted/ADR-028-layered-baseline-9-projects.md)

> **Tek cumlelik sorumluluk:** l5 `Protocol` katmanindaki arac sozlesmesi, SSE akisi ve
> hata modelini **tam kod** olarak ureten, **round-trip testi** + **3 kanitla** kapanan is emridir.

---

## §0 Ozet (Answer First)

| Alan | Deger |
|---|---|
| Katman | `l5` — `VersaCoder.Protocol` |
| K bandı | `K0250-K0299` |
| TFM | `net48;net10.0-windows` |
| Gereksinim | `VC-PRC-001..NNN` (protocol-spec §6) + `VC-E-05xx` (cli-spec §4) + `VC-E-06xx` (llm-provider-spec §5) |
| Ajan | `developer` → `tester` (round-trip) → `reviewer` |
| Kapı | GATE-005 |
| Kanıt | 3 tane: build + round-trip test + NetArchTest |

**YASAK:** UI tipine referans · runtime üretim SQL'i · `l4` → `l5` ters ok (R6).

---

## §1 Baglam

1. `TODOS/05 §0` satır 6: **l5 = tool/agent sözleşmesi**, kapı `sözleşme round-trip`.
2. `protocol-spec §4`: JSON-RPC mesaj kabuğu.
3. `llm-provider-spec §5`: `VC-E-0601..0615` hata kodları.
4. `cli-spec §4.1`: exit code 0-5.

---

## §2 Görev — TAM KOD

### §2.1 JSON-RPC mesaj kabuğu (`K0250-K0259`)

```csharp
// src/VersaCoder.Protocol/JsonRpc/JsonRpcMessage.cs
using System.Text.Json;
using System.Text.Json.Serialization;

namespace VersaCoder.Protocol.JsonRpc;

public sealed class JsonRpcRequest
{
    [JsonPropertyName("jsonrpc")] public string JsonRpc { get; set; } = "2.0";
    [JsonPropertyName("id")] public long Id { get; set; }
    [JsonPropertyName("method")] public string Method { get; set; } = string.Empty;
    [JsonPropertyName("params")] public JsonElement? Params { get; set; }
}

public sealed class JsonRpcResponse
{
    [JsonPropertyName("jsonrpc")] public string JsonRpc { get; set; } = "2.0";
    [JsonPropertyName("id")] public long Id { get; set; }
    [JsonPropertyName("result")] public JsonElement? Result { get; set; }
    [JsonPropertyName("error")] public JsonRpcError? Error { get; set; }
}

public sealed class JsonRpcError
{
    [JsonPropertyName("code")] public int Code { get; set; }
    [JsonPropertyName("message")] public string Message { get; set; } = string.Empty;
    [JsonPropertyName("data")] public string? Data { get; set; }
}
```

### §2.2 SSE parser (`VC-LLM-013`, `K0260-K0269`)

```csharp
// src/VersaCoder.Protocol/Streaming/SseParser.cs
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;

namespace VersaCoder.Protocol.Streaming;

/// <summary>LLM SSE akisini satir satir parser (llm-provider-spec §4 VC-LLM-013).</summary>
public static class SseParser
{
    public static async IAsyncEnumerable<string> ParseAsync(
        System.IO.StreamReader reader,
        [EnumeratorCancellation] CancellationToken ct)
    {
        while (!reader.EndOfStream && !ct.IsCancellationRequested)
        {
            var line = await reader.ReadLineAsync(ct);
            if (line is null) yield break;
            if (!line.StartsWith("data:", System.StringComparison.Ordinal)) continue;
            var payload = line.Substring(5).Trim();
            if (payload == "[DONE]") yield break;
            yield return payload;   // JSON chunk — AI.Engine parse eder
        }
    }
}
```

### §2.3 Tool-call round-trip (`K0270-K0284`)

```csharp
// src/VersaCoder.Protocol/Tools/ToolCallRoundTrip.cs
using System.Text.Json;
using VersaCoder.Abstractions.Ports;

namespace VersaCoder.Protocol.Tools;

/// <summary>Provider tool_calls -> ITool argumanlari -> sonuc -> provider'a geri gonder.</summary>
public static class ToolCallRoundTrip
{
    public static IDictionary<string, object?> ToArguments(ToolCall call)
    {
        using var doc = JsonDocument.Parse(call.ArgumentsJson);
        var dict = new Dictionary<string, object?>();
        foreach (var p in doc.RootElement.EnumerateObject()) dict[p.Name] = p.Value.Clone();
        return dict;
    }

    public static string ToResultJson(ToolResult result) =>
        JsonSerializer.Serialize(new { isSuccess = result.IsSuccess, output = result.Output, error = result.Error });

    public static string ToChatMessage(ToolResult result) =>
        JsonSerializer.Serialize(new { role = "tool", content = ToResultJson(result) });
}
```

---

## §3 Kabul Kriterleri

- [ ] `JsonRpcRequest/Response/Error` + `SseParser` + `ToolCallRoundTrip` — 5 tip
- [ ] **Round-trip**: `ToolCall → arguments → ITool → result → chat message` (test)
- [ ] `l5` → `l4` ters ok = **0** (R6)
- [ ] `VC-E-0601..0615` kodları enum'da
- [ ] Çift bacak build 0 hata

---

## §4 Kanıt (3 tane zorunlu)

```text
KANIT 1: dotnet build -f net48 + -f net10.0-windows -> 0 hata
KANIT 2: dotnet test -> round-trip testi yeşil
KANIT 3: NetArchTest LayerRulesTests -> 0 ihlal (R6 = 0)
```

**Çıktı kutusu:**

```text
+--------------------------------------------------+
| BASLIK : l5 Protocol yazildi                     |
| KAPSAM : src/VersaCoder.Protocol/** (5 tip)      |
| KARAR  : JSON-RPC + SSE + tool round-trip        |
| RISK   : SSE chunk JSON parse hata (VC-E-0615)   |
| KANIT  : build x2 = 0 + round-trip yeşil + 0 ihlal|
+--------------------------------------------------+
```

---

## §5 Ajan Ataması

| # | Ajan | İş |
|---|---|---|
| 1 | `developer` | 5 tip kodu |
| 2 | `tester` | round-trip testi |
| 3 | `reviewer` | R6 + SOLID |

---

**Olculen kanit:** `.ai/prompts/l5-protocol.md` kaydedildi; `prompts/` = 37 dosya.
