---
title: "prompt - l2 Application Handlerlari (Tam Kod)"
type: prompt
category: prompts
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "l2 Application katmanindaki handler/use-case'leri (Command, Query, Handler, Validator, DTO) TAM KOD olarak ureten prompt - TDD (kirmizi-yesil), 3 kanit zorunlu"
---

# prompt — l2 Application Handlerları (Tam Kod)

**Zorunlu Baglantilar:** [prompts/index](index.md) · [AGENTS](../AGENTS.md) · [CLAUDE](../CLAUDE.md) · [TODOS/05](../TODOS/05-kod-ve-kalite.md) · [layer-boundary-check](../checklists/layer-boundary-check.md) · [solid-rules](../.rules/solid-rules.md) · [ADR-028](../.decisions/accepted/ADR-028-layered-baseline-9-projects.md)

> **Tek cumlelik sorumluluk:** l2 `Application` katmanindaki command/query handler'larini
> **tam kod** olarak ureten, **TDD** (kirmizi→yesil) ve **3 kanitla** kapanan is emridir.

---

## §0 Ozet (Answer First)

| Alan | Deger |
|---|---|
| Katman | `l2` — `VersaCoder.Application` |
| K bandi | `K0100-K0149` |
| TFM | `net48;net10.0-windows` |
| Gereksinim | `VC-CLI-001..007` (cli-spec §2.1) |
| Ajan | `developer` (kod) → `tester` (TDD) → `reviewer` |
| Kapı | GATE-005 |
| Kanıt | 3 tane: kirmizi→yesil + `dotnet build` (cift) + NetArchTest |

**YASAK:** `Infrastructure` tanımak · `DbConnection`/SQL · DevExpress tipi ·
`DateTime.Now` · UI tipini iş mantığında taşımak.

---

## §1 Baglam

1. `TODOS/05 §0` satır 3: **l2 = handler/use-case + validator**, kapı `test kırmızı→yeşil (TDD)`.
2. `cli-spec §2.1`: `VC-CLI-001..007` komutları l2 handler'lardan geçer.
3. `l0` portlar + `l1` varlıklar zaten yazıldı.

---

## §2 Görev — TAM KOD

### §2.1 Command/Query sözleşmesi (`K0100-K0104`)

```csharp
// src/VersaCoder.Application/Common/ICommand.cs
using VersaCoder.Abstractions.Results;

namespace VersaCoder.Application.Common;

public interface ICommand<TResponse> { }
public interface IQuery<TResponse> { }
public interface ICommandHandler<in TCommand, TResponse> where TCommand : ICommand<TResponse>
{
    Task<Result<TResponse>> HandleAsync(TCommand command, CancellationToken ct);
}
public interface IQueryHandler<in TQuery, TResponse> where TQuery : IQuery<TResponse>
{
    Task<Result<TResponse>> HandleAsync(TQuery query, CancellationToken ct);
}
```

### §2.2 `LayerListQuery` handler (`VC-CLI-004`, `K0105-K0114`)

```csharp
// src/VersaCoder.Application/Layers/LayerListQuery.cs
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using VersaCoder.Abstractions.Ports;
using VersaCoder.Abstractions.Results;
using VersaCoder.Application.Common;

namespace VersaCoder.Application.Layers;

public sealed record LayerListQuery(string? Band = null, string? Status = null) : IQuery<IReadOnlyList<LayerDto>>;
public sealed record LayerDto(string LayerId, string Band, string Dll, string Layer, string OwnerAgent, string Status);

public sealed class LayerListHandler : IQueryHandler<LayerListQuery, IReadOnlyList<LayerDto>>
{
    private readonly IVaultReader _vault;
    public LayerListHandler(IVaultReader vault) => _vault = vault;

    public async Task<Result<IReadOnlyList<LayerDto>>> HandleAsync(LayerListQuery q, CancellationToken ct)
    {
        var rows = await _vault.SearchAsync(q.Band ?? "K", ct);
        if (rows.Count == 0) return Result<IReadOnlyList<LayerDto>>.Failure("VC-E-0506", "Layer bulunamadi");
        return Result<IReadOnlyList<LayerDto>>.Success(Parse(rows, q));
    }
    private static IReadOnlyList<LayerDto> Parse(IReadOnlyList<string> rows, LayerListQuery q) =>
        System.Linq.Enumerable.ToArray(System.Linq.Enumerable.Empty<LayerDto>());   // CSV parse
}
```

### §2.3 Validator (`K0115-K0124`)

```csharp
// src/VersaCoder.Application/Common/Validator.cs
using System.Collections.Generic;
using VersaCoder.Abstractions.Results;

namespace VersaCoder.Application.Common;

public sealed class ValidationException : Exception
{
    public IReadOnlyDictionary<string, string[]> Errors { get; }
    public ValidationException(IReadOnlyDictionary<string, string[]> errors) : base("Dogrulama hatasi")
        => Errors = errors;
}

public static class Validator
{
    public static void NotNullOrWhiteSpace(string? v, string field, IDictionary<string, List<string>> errs)
    {
        if (string.IsNullOrWhiteSpace(v)) errs[field] = new List<string> { $"{field} bos olamaz." };
    }
}
```

---

## §3 Kabul Kriterleri

- [ ] `ICommandHandler` / `IQueryHandler` / `LayerListHandler` / `ValidationException` — 4+ tip
- [ ] **TDD**: test önce kırmızı, kod sonra yeşil
- [ ] `l2` → `Infrastructure` `using` = **0** (R4)
- [ ] Çift bacak build 0 hata
- [ ] `layer-boundary-check` 13 soru

---

## §4 Kanıt (3 tane zorunlu)

```text
KANIT 1: dotnet test -> ONCE kirmisi (test var, kod yok) SONRA yesil
KANIT 2: dotnet build -f net48 + -f net10.0-windows -> 0 hata
KANIT 3: NetArchTest LayerRulesTests -> 0 ihlal
```

**Çıktı kutusu:**

```text
+--------------------------------------------------+
| BASLIK : l2 Application handlerlari yazildi      |
| KAPSAM : src/VersaCoder.Application/** (4+ tip)  |
| KARAR  : TDD; l2 -> l4 oku YOK (R4)              |
| RISK   : LayerListHandler CSV parse TODO         |
| KANIT  : kirmizi->yesil + build x2 = 0 + 0 ihlal |
+--------------------------------------------------+
```

---

## §5 Ajan Ataması

| # | Ajan | İş |
|---|---|---|
| 1 | `developer` | Handler + validator |
| 2 | `tester` | TDD test (kırmızı→yeşil) |
| 3 | `reviewer` | R4 + SOLID |

---

**Olculen kanit:** `.ai/prompts/l2-application.md` kaydedildi; `prompts/` = 34 dosya.
