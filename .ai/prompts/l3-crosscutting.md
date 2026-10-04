---
title: "prompt - l3 Crosscutting (Tam Kod)"
type: prompt
category: prompts
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "l3 Crosscutting katmanini (Logging, Caching, Config, Clock, Result extensions) TAM KOD olarak ureten prompt - 3 kanit zorunlu, l3 yalniz l0'a baglanir (R2)"
---

# prompt — l3 Crosscutting (Tam Kod)

**Zorunlu Baglantilar:** [prompts/index](index.md) · [AGENTS](../AGENTS.md) · [CLAUDE](../CLAUDE.md) · [TODOS/05](../TODOS/05-kod-ve-kalite.md) · [logging-rules](../.rules/logging-rules.md) · [caching-rules](../.rules/caching-rules.md) · [configuration-rules](../.rules/configuration-rules.md) · [layer-boundary-check](../checklists/layer-boundary-check.md) · [ADR-028](../.decisions/accepted/ADR-028-layered-baseline-9-projects.md)

> **Tek cumlelik sorumluluk:** l3 `Crosscutting` katmanindaki kesit sorumluluklarini
> **tam kod** olarak ureten, **3 kanitla** kapanan is emridir — **l3 yalniz l0'a baglanir** (R2).

---

## §0 Ozet (Answer First)

| Alan | Deger |
|---|---|
| Katman | `l3` — `VersaCoder.Crosscutting` |
| K bandı | `K0150-K0199` |
| TFM | `net48;net10.0-windows` |
| Gereksinim | `VC-PRD-NNN` (performance-spec) |
| Ajan | `developer` → `tester` → `reviewer` |
| Kapı | GATE-005 |
| Kanıt | 3 tane: build + test + NetArchTest |

**YASAK:** kural iş mantığı · `l0` dışına bağımlılık · EF Core · `DateTime.Now`.

---

## §1 Baglam

1. `TODOS/05 §0` satır 4: **l3 = log/cache/saat/config**, kapı `test yeşil`.
2. `ADR-028 R2`: **`l3` yalnız `l0`'a bakar** (CRITICAL).
3. `logging-rules`: 5 seviye structured log.
4. `caching-rules`: TTL + invalidation.

---

## §2 Görev — TAM KOD

### §2.1 `ILogger` + seviyeler (`K0150-K0159`)

```csharp
// src/VersaCoder.Crosscutting/Logging/ILogger.cs
using System;
using System.Collections.Generic;

namespace VersaCoder.Crosscutting.Logging;

public enum LogLevel { Trace, Debug, Information, Warning, Error, Critical }

public interface ILogger
{
    void Log(LogLevel level, string message, IReadOnlyDictionary<string, object?>? properties = null, Exception? ex = null);
}
public static class LoggerExtensions
{
    public static void Info(this ILogger l, string m) => l.Log(LogLevel.Information, m);
    public static void Error(this ILogger l, string m, Exception? e = null) => l.Log(LogLevel.Error, m, null, e);
}
```

### §2.2 `ICache` + TTL (`K0160-K0169`)

```csharp
// src/VersaCoder.Crosscutting/Caching/ICache.cs
using System;
using System.Threading;
using System.Threading.Tasks;

namespace VersaCoder.Crosscutting.Caching;

public interface ICache
{
    Task<T?> GetAsync<T>(string key, CancellationToken ct) where T : class;
    Task SetAsync<T>(string key, T value, TimeSpan ttl, CancellationToken ct) where T : class;
    Task RemoveAsync(string key, CancellationToken ct);
}
public sealed class InMemoryCache : ICache
{
    private readonly System.Collections.Concurrent.ConcurrentDictionary<string, (object Value, DateTimeOffset Exp)> _map = new();
    // TTL kontrolü + eviction — INFERRED (caching-rules §3)
}
```

### §2.3 `IClock` uygulaması + `IConfig` (`K0170-K0184`)

```csharp
// src/VersaCoder.Crosscutting/Time/SystemClock.cs
using System;
using VersaCoder.Abstractions.Ports;

namespace VersaCoder.Crosscutting.Time;

/// <summary>l1'de DateTime.Now YASAK (R3); herkes bunu kullanir.</summary>
public sealed class SystemClock : IClock { public DateTimeOffset UtcNow => DateTimeOffset.UtcNow; }
public sealed class FixedClock : IClock { public FixedClock(DateTimeOffset t) => UtcNow = t; public DateTimeOffset UtcNow { get; } }

// src/VersaCoder.Crosscutting/Configuration/IConfig.cs
using System.Collections.Generic;

namespace VersaCoder.Crosscutting.Configuration;

public interface IConfig
{
    string Get(string key, string? fallback = null);
    IReadOnlyDictionary<string, string> Section(string prefix);
}
```

---

## §3 Kabul Kriterleri

- [ ] `ILogger` + `LoggerExtensions` + `LogLevel` + `ICache` + `InMemoryCache` + `SystemClock` + `FixedClock` + `IConfig` — 8 tip
- [ ] **`l3` → `l0` dışında bağımlılık = 0** (R2 CRITICAL)
- [ ] `DateTime.Now` = 0 eşleşme
- [ ] Çift bacak build 0 hata

---

## §4 Kanıt (3 tane zorunlu)

```text
KANIT 1: dotnet build -f net48 + -f net10.0-windows -> 0 hata
KANIT 2: dotnet test -> log/cache/clock birim testi yeşil
KANIT 3: NetArchTest LayerRulesTests -> 0 ihlal (R2 = 0)
```

**Çıktı kutusu:**

```text
+--------------------------------------------------+
| BASLIK : l3 Crosscutting yazildi                 |
| KAPSAM : src/VersaCoder.Crosscutting/** (8 tip)  |
| KARAR  : l3 yalniz l0'a bagli (R2) = 0 ihlal     |
| RISK   : InMemoryCache eviction INFERRED          |
| KANIT  : build x2 = 0 + test yeşil + 0 ihlal     |
+--------------------------------------------------+
```

---

## §5 Ajan Ataması

| # | Ajan | İş |
|---|---|---|
| 1 | `developer` | 8 tip kodu |
| 2 | `tester` | log/cache/clock test |
| 3 | `reviewer` | R2 + logging/caching kuralları |

---

**Olculen kanit:** `.ai/prompts/l3-crosscutting.md` kaydedildi; `prompts/` = 35 dosya.
