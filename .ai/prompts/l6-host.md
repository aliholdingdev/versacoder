---
title: "prompt - l6 Host (DI + Bootstrap, Tam Kod)"
type: prompt
category: prompts
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "l6 Host katmanini (CompositionRoot, DI kayitlari, lifecycle, CLI->Terminal->LLM baglantisi) TAM KOD olarak ureten prompt - 3 kanit zorunlu, uygulama ayaga kalkar (smoke)"
---

# prompt — l6 Host (DI + Bootstrap, Tam Kod)

**Zorunlu Baglantilar:** [prompts/index](index.md) · [AGENTS](../AGENTS.md) · [CLAUDE](../CLAUDE.md) · [TODOS/05](../TODOS/05-kod-ve-kalite.md) · [host-layer-spec](../spec/host-layer-spec.md) · [cli-spec](../spec/cli-spec.md) · [llm-provider-spec](../spec/llm-provider-spec.md) · [layer-boundary-check](../checklists/layer-boundary-check.md) · [ADR-028](../.decisions/accepted/ADR-028-layered-baseline-9-projects.md)

> **Tek cumlelik sorumluluk:** l6 `Host` katmanindaki composition root, DI kayitlari ve
> yasam dongusunu **tam kod** olarak ureten, **smoke test** + **3 kanitla** kapanan is emridir —
> **iş mantığı YAZMAZ** (yalnız kurulum + birleştirme).

---

## §0 Ozet (Answer First)

| Alan | Deger |
|---|---|
| Katman | `l6` — `VersaCoder.Host` |
| K bandı | `K0300-K0349` |
| TFM | `net48;net10.0-windows` |
| Gereksinim | `VC-CLI-013` (config) + `VC-LLM-014` (fallback) + `VC-PRD-040..042` (terminal) |
| Ajan | `architect` (DI tasarımı) → `developer` → `tester` (smoke) |
| Kapı | GATE-005 |
| Kanıt | 3 tane: build + smoke (uygulama ayağa kalkar) + NetArchTest |

**YASAK:** iş mantığı (yalnız kompozisyon + startup) · form/control/DevExpress tipi (R7) ·
EF Core · `static` servis locator.

---

## §1 Baglam

1. `TODOS/05 §0` satır 7: **l6 = DI + bootstrap**, kapı `uygulama başlıyor (smoke)`.
2. `ADR-028 R7`: `l6` form, control, DevExpress tipi **görmez**.
3. `cli-spec §6`: config okuma sırası env → `secrets.json` → default.
4. `llm-provider-spec §4 VC-LLM-014`: fallback OpenRouter → NIM.

---

## §2 Görev — TAM KOD

### §2.1 Composition Root (`K0300-K0314`)

```csharp
// src/VersaCoder.Host/Composition/AppHost.cs
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using VersaCoder.Abstractions.Ports;
using VersaCoder.Crosscutting.Caching;
using VersaCoder.Crosscutting.Configuration;
using VersaCoder.Crosscutting.Logging;
using VersaCoder.Crosscutting.Time;
using VersaCoder.Infrastructure.FileSystem;
using VersaCoder.Infrastructure.Http;
using VersaCoder.Protocol.Streaming;

namespace VersaCoder.Host.Composition;

/// <summary>Kompozisyon + startup — is mantigi YASAK (R7).</summary>
public sealed class AppHost : IAsyncDisposable
{
    private readonly Dictionary<Type, object> _services = new();
    public T Get<T>() where T : notnull => (T)_services[typeof(T)];
    public void Register<T>(T instance) where T : notnull => _services[typeof(T)] = instance;

    public static AppHost Create(string[] args)
    {
        var host = new AppHost();
        host.Register<IClock>(new SystemClock());
        host.Register<ILogger>(new ConsoleLogger());
        host.Register<IFileSystem>(new FileSystemAdapter());
        host.Register<ICache>(new InMemoryCache());
        host.Register<IConfig>(new EnvConfig());       // VC-CLI-013: env -> secrets.json -> default
        // LLM kayitlari (llm-provider-spec §6): OpenRouter + NIM + fallback
        host.Register<IAgentProvider>(new OpenRouterAdapter(host.Get<IConfig>()));
        host.Register<IAgentProvider>(new NvidiaNimAdapter(host.Get<IConfig>()));
        return host;
    }

    public async Task<int> RunAsync(string[] args, CancellationToken ct)
    {
        // VC-CLI-001..007: komut agaci cozumleme (cli-spec §3)
        var exit = CliRouter.RouteAsync(this, args, ct);   // 0=OK,1=usage,2=runtime,3=notfound,4=perm,5=cancelled
        return await exit;
    }

    public async ValueTask DisposeAsync() { await Task.CompletedTask; }
}

internal sealed class ConsoleLogger : ILogger { /* basit stdout */ }
internal sealed class EnvConfig : IConfig { /* VC-CLI-013 */ }
internal sealed class OpenRouterAdapter : IAgentProvider { /* llm-provider-spec §3.1 */ }
internal sealed class NvidiaNimAdapter : IAgentProvider { /* llm-provider-spec §3.2 */ }
```

### §2.2 `CliRouter` (`VC-CLI-001..016`, `K0315-K0329`)

```csharp
// src/VersaCoder.Host/Routing/CliRouter.cs
using System.Threading;
using System.Threading.Tasks;

namespace VersaCoder.Host.Routing;

/// <summary>CLI komut agaci — cli-spec §3 (new/build/run/layer/agent/vault).</summary>
public static class CliRouter
{
    public static async Task<int> RouteAsync(AppHost host, string[] args, CancellationToken ct)
    {
        if (args.Length == 0) return 1;                                  // usage
        var cmd = args[0].ToLowerInvariant();
        return cmd switch
        {
            "new"    => await Commands.NewAsync(host, args, ct),
            "build"  => await Commands.BuildAsync(host, args, ct),
            "run"    => await Commands.RunAsync(host, args, ct),
            "layer"  => await Commands.LayerAsync(host, args, ct),
            "agent"  => await Commands.AgentAsync(host, args, ct),
            "vault"  => await Commands.VaultAsync(host, args, ct),
            "--help" or "-h" => 0,
            "--version" or "-v" => 0,
            _ => 1,   // VC-E-0501
        };
    }
}
```

---

## §3 Kabul Kriterleri

- [ ] `AppHost` + `CliRouter` + 4 internal adapter — 6 tip
- [ ] **Smoke**: uygulama ayağa kalkar, `--version` = 0
- [ ] `l6` → DevExpress/form = **0** (R7)
- [ ] Config sırası: env → secrets.json → default (VC-CLI-013)
- [ ] Çift bacak build 0 hata

---

## §4 Kanıt (3 tane zorunlu)

```text
KANIT 1: dotnet build -f net48 + -f net10.0-windows -> 0 hata
KANIT 2: dotnet run -- --version -> exit 0 (smoke)
KANIT 3: NetArchTest LayerRulesTests -> 0 ihlal (R7 = 0)
```

**Çıktı kutusu:**

```text
+--------------------------------------------------+
| BASLIK : l6 Host DI + bootstrap yazildi          |
| KAPSAM : src/VersaCoder.Host/** (6 tip)          |
| KARAR  : composition only; is mantigi yok (R7)   |
| RISK   : 2 LLM adapter ayni IAgentProvider tipi  |
| KANIT  : build x2 = 0 + smoke --version=0 + 0 ihlal|
+--------------------------------------------------+
```

---

## §5 Ajan Ataması

| # | Ajan | İş |
|---|---|---|
| 1 | `architect` | DI tasarımı + katman 13 soru |
| 2 | `developer` | AppHost + CliRouter |
| 3 | `tester` | Smoke test |
| 4 | `reviewer` | R7 + SOLID |

---

**Olculen kanit:** `.ai/prompts/l6-host.md` kaydedildi; `prompts/` = 38 dosya.
