---
title: "prompt - l0 Abstractions Portlari (Tam Kod)"
type: prompt
category: prompts
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "l0 Abstractions katmanindaki tum portlari (IAgentProvider, ITool, Result<T>, IEditorService, IVaultReader, IAgentStreamSink) TAM KOD olarak ureten prompt - 3 kanit zorunlu, 7 ajan matrisi, spec gereksinim kimlikleri"
---

# prompt — l0 Abstractions Portlari (Tam Kod)

**Zorunlu Baglantilar:** [prompts/index](index.md) · [AGENTS](../AGENTS.md) · [CLAUDE](../CLAUDE.md) · [keys](../keys.md) · [TODOS/05](../TODOS/05-kod-ve-kalite.md) · [TODOS/06](../TODOS/06-5000-katman-kurulumu.md) · [layer-boundary-check](../checklists/layer-boundary-check.md) · [llm-provider-spec](../spec/llm-provider-spec.md) · [cli-spec](../spec/cli-spec.md) · [terminal-spec](../spec/terminal-spec.md) · [layer-rules](../.rules/layer-rules.md) · [ADR-028](../.decisions/accepted/ADR-028-layered-baseline-9-projects.md)

> **Tek cumlelik sorumluluk:** l0 `Abstractions` katmanindaki tum portlari **tam kod**
> olarak ureten, **3 kanitla** (build + test + NetArchTest) kapanan is emridir.

---

## §0 Ozet (Answer First)

| Alan | Deger |
|---|---|
| Katman | `l0` — `VersaCoder.Abstractions` (ADR-028 §5.1) |
| K bandi | `K0000-K0049` (50 slot) |
| TFM | `net48;net10.0-windows` (G6 cift bacak) |
| Gereksinim | `VC-LLM-001..014` (llm-provider-spec §2) + `VC-CLI-001..016` (cli-spec §2) |
| Ajan | `architect` (port tasarimi) → `developer` (kod) → `reviewer` (inceleme) → `tester` (test) |
| Kapı | GATE-003 (09-terminal onayi) → GATE-004 (iskelet + ilk commit) → GATE-005 (test) |
| Kanıt | 3 tane: `dotnet build` (cift bacak) + `dotnet test` + NetArchTest |

**YASAK:** somut sınıf · dis paket referansi · `DbConnection` · `DateTime.Now` ·
DevExpress · EF Core · `static` global durum (G2/G3/G8).

---

## §1 Baglam (vault'tan okunur, uydurulmaz)

1. `TODOS/05 §0` satır 1: **l0 = portlar (`Result`, `IAgentStreamSink`)**, kapı `build + 13 soru`.
2. `TODOS/06 §1` Blok K0: `.sln` + 45 `.csproj` + 2 test + 5000 kural — **önce** l0 yazılır.
3. `llm-provider-spec §2`: `IAgentProvider` portu l0'da, bağımlılığı **0**.
4. `cli-spec §2.1`: CLI komutları port üzerinden (`new/build/run/layer/agent/vault`).
5. `ADR-028 §5`: `l0` **hiçbir** `VersaCoder.*` namespace'ine `using` yapamaz (R1 CRITICAL).

---

## §2 Görev — TAM KOD

### §2.1 `Result<T>` + `Error` (N01, `K0000-K0009`)

```csharp
// src/VersaCoder.Abstractions/Results/Result.cs
namespace VersaCoder.Abstractions.Results;

/// <summary>Basarili ya da basarisiz islem sonucu. Exception firlatmaz (G5, brain §3).</summary>
/// <typeparam name="T">Basari durumunda donen deger tipi.</typeparam>
public readonly struct Result<T>
{
    private readonly T? _value;
    private readonly Error? _error;

    private Result(T value) { _value = value; _error = null; IsSuccess = true; }
    private Result(Error error) { _value = default; _error = error; IsSuccess = false; }

    /// <summary>True ise <see cref="Value"/> gecerlidir; false ise <see cref="Error"/>.</summary>
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public T Value => IsSuccess ? _value! : throw new InvalidOperationException("Failure result has no value.");
    public Error Error => IsFailure ? _error! : throw new InvalidOperationException("Success result has no error.");

    public static Result<T> Success(T value) => new(value);
    public static Result<T> Failure(Error error) => new(error);
    public static Result<T> Failure(string code, string message) => new(new Error(code, message));

    public Result<TOut> Map<TOut>(Func<T, TOut> mapper) =>
        IsSuccess ? Result<TOut>.Success(mapper(Value)) : Result<TOut>.Failure(Error);

    public override string ToString() =>
        IsSuccess ? $"OK({Value})" : $"FAIL({Error.Code}: {Error.Message})";
}

/// <summary>Sifirlanabilir (nullable) deger icin ayri tip — T: struct degilse bu kullanilir.</summary>
public readonly struct Result
{
    private readonly Error? _error;
    private Result(Error error) { _error = error; IsSuccess = false; }
    private Result() { _error = null; IsSuccess = true; }

    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public Error Error => IsFailure ? _error! : throw new InvalidOperationException("Success result has no error.");

    public static Result Success() => new();
    public static Result Failure(Error error) => new(error);
    public static Result Failure(string code, string message) => new(new Error(code, message));
}

/// <summary>Hata modeli — VC-E-05xx (cli-spec §4) ve VC-E-06xx (llm-provider-spec §5) kodlarini tasiyabilir.</summary>
public sealed record Error(string Code, string Message, string? Detail = null)
{
    public static readonly Error Unknown = new("VC-E-9999", "Bilinmeyen hata");
}
```

### §2.2 `IAgentProvider` (llm-provider-spec §2, `K0010-K0019`)

```csharp
// src/VersaCoder.Abstractions/Ports/IAgentProvider.cs
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace VersaCoder.Abstractions.Ports;

/// <summary>LLM saglayici portu — l0. Uygulamasi l4 (AI.Engine) (llm-provider-spec §2).</summary>
public interface IAgentProvider
{
    /// <summary>"openrouter" | "nvidia-nim" (llm-provider-spec §0).</summary>
    string Name { get; }
    bool IsAvailable { get; }
    Task<ProviderResult> CompleteAsync(ProviderRequest request, CancellationToken ct);
    IAsyncEnumerable<string> StreamAsync(ProviderRequest request, CancellationToken ct);
    Task<IReadOnlyList<ModelInfo>> ListModelsAsync(CancellationToken ct);
}

public sealed record ProviderRequest(
    string Model,
    IReadOnlyList<ChatMessage> Messages,
    double Temperature = 0.2,
    int MaxTokens = 4096,
    bool Stream = false,
    IReadOnlyList<ToolDefinition>? Tools = null);

public sealed record ChatMessage(string Role, string Content);
public sealed record ProviderResult(string Text, int PromptTokens, int CompletionTokens, string FinishReason, string Model);
public sealed record ModelInfo(string Id, int ContextLength, string Pricing, IReadOnlyList<string> Capabilities);
public sealed record ToolDefinition(string Name, string Description, string ParametersJson);
public sealed record ToolCall(string Id, string Name, string ArgumentsJson);
```

### §2.3 `ITool` + `IToolRegistry` (tool-spec §2, `K0020-K0029`)

```csharp
// src/VersaCoder.Abstractions/Ports/ITool.cs
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace VersaCoder.Abstractions.Ports;

/// <summary>Arac sozlesmesi — tool-spec §2. Model dogrudan dosya/Process/DB erisimi sahip degildir (SEC-06).</summary>
public interface ITool
{
    string Name { get; }
    string Description { get; }
    string ParametersJson { get; }
    Task<ToolResult> ExecuteAsync(IDictionary<string, object?> arguments, CancellationToken ct);
}

public interface IToolRegistry
{
    IReadOnlyList<ITool> All { get; }
    ITool? Find(string name);
    void Register(ITool tool);
}

public sealed record ToolResult(bool IsSuccess, string Output, string? Error = null);
```

### §2.4 `IEditorService` + `IVaultReader` + `IVaultWriter` (`K0030-K0039`)

```csharp
// src/VersaCoder.Abstractions/Ports/IEditorService.cs
using System.Threading;
using System.Threading.Tasks;

namespace VersaCoder.Abstractions.Ports;

/// <summary>Kod editoru portu (cli-spec §2.1, l7 UI bunu cagirir).</summary>
public interface IEditorService
{
    Task<string> GetTextAsync(string filePath, CancellationToken ct);
    Task SaveAsync(string filePath, string content, CancellationToken ct);
    Task OpenAsync(string filePath, CancellationToken ct);
    Task<int> GetCaretLineAsync(CancellationToken ct);
}

/// <summary>Vault okuma portu — SSOT (cli-spec VC-CLI-007).</summary>
public interface IVaultReader
{
    Task<string?> ReadAsync(string relativePath, CancellationToken ct);
    Task<IReadOnlyList<string>> SearchAsync(string query, CancellationToken ct);
    Task<IReadOnlyList<string>> ListAsync(string directory, CancellationToken ct);
}

/// <summary>Vault yazma portu — ADR/doc/test-doc (VAULT WRITER).</summary>
public interface IVaultWriter
{
    Task WriteAsync(string relativePath, string content, CancellationToken ct);
}
```

### §2.5 `IAgentStreamSink` (TODOS/05 §0 satır 1, `K0040-K0049`)

```csharp
// src/VersaCoder.Abstractions/Ports/IAgentStreamSink.cs
using System.Threading;
using System.Threading.Tasks;

namespace VersaCoder.Abstractions.Ports;

/// <summary>Agent/stream ciktisinin UI'a (terminal MemoEdit) aktiligi — terminal-spec §2.1.</summary>
public interface IAgentStreamSink
{
    Task OnLineAsync(string line, CancellationToken ct);
    Task OnCompletedAsync(int exitCode, CancellationToken ct);
    Task OnErrorAsync(string message, CancellationToken ct);
}
```

### §2.6 `IClock` + `IFileSystem` (Crosscutting baglantisi, `K0045-K0049`)

```csharp
// src/VersaCoder.Abstractions/Ports/IClock.cs
using System;
using System.Threading;
using System.Threading.Tasks;

namespace VersaCoder.Abstractions.Ports;

/// <summary>Zaman portu — l1'de DateTime.Now YASAK (ADR-028 R3).</summary>
public interface IClock { DateTimeOffset UtcNow { get; } }

/// <summary>Dosya sistemi portu — l0'da somut IO yok.</summary>
public interface IFileSystem
{
    bool FileExists(string path);
    string ReadAllText(string path);
    void WriteAllText(string path, string content);
    Task<string> ReadAllTextAsync(string path, CancellationToken ct);
}
```

---

## §3 Kabul Kriterleri

- [ ] 6 port dosyasi `src/VersaCoder.Abstractions/` altında (`Results/`, `Ports/`)
- [ ] `IAgentProvider` bağımlılığı **0** (NetArchTest `LayerRulesTests`)
- [ ] `Result<T>` + `Error` + `Result` (non-generic) — 3 tip
- [ ] `l0` → hiçbir `VersaCoder.*` `using` (R1 CRITICAL)
- [ ] Çift bacak: `net48` + `net10.0-windows`
- [ ] `layer-boundary-check` 13 soru cevaplı
- [ ] XML doc her public tip/mototta

---

## §4 Kanıt (3 tane zorunlu)

```text
KANIT 1: dotnet build -f net48 --nologo        -> 0 hata
KANIT 2: dotnet build -f net10.0-windows --nologo -> 0 hata
KANIT 3: dotnet test + NetArchTest LayerRulesTests -> 0 ihlal
```

**Çıktı kutusu (AGENTS §2):**

```text
+--------------------------------------------------+
| BASLIK : l0 Abstractions portlari yazildi        |
| KAPSAM : src/VersaCoder.Abstractions/** (6 dosya)|
| KARAR  : port l0'da, somut yok; R1 ihlali 0      |
| RISK   : IAgentProvider ToolCall semasi protokol |
|          ile celisebilir (llm-provider-spec §9/3)|
| KANIT  : build x2 = 0 hata + NetArchTest 0 ihlal |
+--------------------------------------------------+
```

---

## §5 Ajan Ataması (.ai/AGENTS.md §1)

| # | Ajan | İş |
|---|---|---|
| 1 | `architect` | Port tasarımı + `layer-boundary-check` 13 soru |
| 2 | `developer` | 6 dosya kodu (`Result`, `IAgentProvider`, `ITool`, ...) |
| 3 | `reviewer` | R1 ihlali + SOLID + `RİSK` satırı |
| 4 | `tester` | `dotnet test` + NetArchTest |

---

**Olculen kanit (2026-10-02):** bu prompt `.ai/prompts/l0-portlar.md` olarak kaydedildi; `prompts/` = 31 → **32 dosya**.
