---
title: "composition-root-host-startup - VersaCoder"
type: knowledge
category: winforms
status: active
version: 1.0.0
updated: 2026-09-29
authority: SSOT
scope: "Uygulamanin nerede kuruldugunu, Program.cs'in ne icerip ne iceremedigini, #if platform sinirini ve container yasam dongusunu tanimlar"
---

# Composition Root ve Program.cs Baslangic Akisi

**Zorunlu Baglantilar:** [vault indeks](../index.md) · [coding-standards](../../.rules/coding-standards.md) · [async-rules](../../.rules/async-rules.md) · [di-container-lifetimes](di-container-lifetimes.md) · [logging-integration](logging-integration.md)
**Küme + Hub:** [winforms index](index.md) · [index](../../index.md) · [CLAUDE](../../claude.md) · [AGENTS](../../definitions/agents.md) · [brain](../../.decisions/brain.md) · [glossary](../../definitions/glossary.md) · [keys](../../governance/keys.md)
**Kardeşler (winforms · 3/32):** [accessibility-tab-and-mnemonics](accessibility-tab-and-mnemonics.md) · [anchor-dock-layout-rules](anchor-dock-layout-rules.md) · [async-await-ui-thread](async-await-ui-thread.md) · [async-loading-patterns](async-loading-patterns.md) · [async-ui-thread-marshalling](async-ui-thread-marshalling.md) · [async-void-event-handlers](async-void-event-handlers.md) · [backgroundworker-vs-task](backgroundworker-vs-task.md) · [cancellation-in-ui](cancellation-in-ui.md) · [cancellation-token-in-ui](cancellation-token-in-ui.md) · [control-disposal-and-memory](control-disposal-and-memory.md) · [controls-lifecycle-events](controls-lifecycle-events.md) · [data-binding-basics](data-binding-basics.md) · [designer-cs-anatomy](designer-cs-anatomy.md) · [designer-cs-trio-rule](designer-cs-trio-rule.md) · [designer-editing-rules](designer-editing-rules.md) · [designer-hand-edit-policy](designer-hand-edit-policy.md) · [docking-panels-patterns](docking-panels-patterns.md) · [errorprovider-validation-flow](errorprovider-validation-flow.md) · [exception-boundary-ui](exception-boundary-ui.md) · [form-close-and-dispose](form-close-and-dispose.md) · [form-close-cancel-pattern](form-close-cancel-pattern.md) · [form-constructor-vs-load](form-constructor-vs-load.md) · [form-load-and-shown-events](form-load-and-shown-events.md) · [layout-table-panel](layout-table-panel.md) · [partial-class-and-file-naming](partial-class-and-file-naming.md) · [settings-user-config](settings-user-config.md) · [splitcontainer-and-sizing](splitcontainer-and-sizing.md) · [validation-form-level](validation-form-level.md) · [winforms-rendering-performance](winforms-rendering-performance.md)

> **Tek cumlelik sorumluluk:** *Program.cs ne yapar, nerede yapmaz* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

**Composition root** (`Program.cs` + Host katmani) tum bagimliliklari **tek yerde** birlestirir; baska hicbir katman `IServiceProvider` ya da somut servis `new`'lemez. `Program.cs` yalniz uc sey yapar: **(1)** platform baslatma, **(2)** servis kayitlari, **(3)** `Application.Run(mainForm)`. Host katmani is mantigi **icermez**; `#if` yalniz platform/API farkiyla sinirlidir.

```text
+------------------------------------------------------------+
| [1] Initialize / EnableVisualStyles   (platform)           |
| [2] BuildHost()                       (servis kayitlari)   |
| [3] RegisterGlobalHandlers()          (handler sonrada)     |
| [4] Application.Run(MainForm)         (anas dongu)          |
|      sira bozulursa: tema tutarsiz / handler erken koken   |
+------------------------------------------------------------+
```

---

## §1 Program.cs bolumleri

| # | Bolum | Icerdigi | Icermesi yasak |
|---|---|---|---|
| 1 | Platform baslatma | `ApplicationConfiguration.Initialize()` / `EnableVisualStyles` | is mantigi |
| 2 | Servis kayitlari | `IServiceCollection` kayitlari | SQL, dosya I/O |
| 3 | Ana form cozumleme | `GetRequiredService<MainForm>()` + `Application.Run` | hata gunlugu mantigi |
| 4 | Global exception hookup | `Application.ThreadException`, `AppDomain.UnhandledException` | try/catch ile gizleme |

```csharp
[STAThread]
static void Main()
{
#if NETFRAMEWORK              // yalniz platform/API farki
    Application.EnableVisualStyles();
    Application.SetCompatibleTextRenderingDefault(false);
#else
    ApplicationConfiguration.Initialize();
#endif

    using var host = BuildHost();                 // composition root
    RegisterGlobalHandlers(host.Services);

    Application.Run(host.Services.GetRequiredService<MainForm>());
}
```

---

## §2 Kurulum sirasi - sira kritik

| # | Adim | Sira bozulursa |
|---|---|---|
| 1 | `Initialize` / `EnableVisualStyles` | tema tutarsiz, DPI sicramasi |
| 2 | Container kur | form cozumlenemez |
| 3 | Global handler'lari **container kurulduktan sonra** bagla | handler `logger`'i erken koken |
| 4 | Ana formu cozumle | form transient ise her cagri yeni ornek |
| 5 | `Application.Run` | uygulama hic baslamaz |

---

## §3 Host katmani ne yapar - ne yapmaz

| Yapar | Yapmaz |
|---|---|
| Servis kayitlari | Is mantigi / use case |
| `ServiceProvider` kurulumu | Dogrudan `DbCommand` olusturma |
| Uygulama yasam dongusu (`Application.Run`) | UI kontrol erisimi |
| Global handler baglama | Log formati karari |

`#if` ile is mantigi yazmak yasaktir; Host' tasan dosya kompozisyonu test edilemez yapar.

---

## §4 Global exception hookup

```csharp
Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
Application.ThreadException += (s, e) => reporter.Report(e.Exception);
AppDomain.CurrentDomain.UnhandledException += (s, e) =>
    reporter.Report(e.ExceptionObject as Exception);
```

| # | Yasak | Sonuc |
|---|---|---|
| 1 | Handler'lari container kurulumundan once baglamak | erken koken hata |
| 2 | Hatalari sessizce yutmak | kanipsiz "calisiyor" |
| 3 | `Program.cs` icine is mantigi yazmak | test edilemez baslangic |
| 4 | Iki ayri `BuildServiceProvider` | iki ayri dogruluk kaynagi |
| 5 | Kanitsiz "bitti" | G5 ihlali |

---

## §5 Kontrol listesi

| # | Adim | Kanit |
|---|---|---|
| 1 | `Program.cs` yalniz 4 bolum iceriyor | kod incelemesi |
| 2 | `#if` yalniz platform/API farki | grep `#if` |
| 3 | Container tek yerde kuruluyor | grep `BuildServiceProvider` = 1 |
| 4 | Global handler'lar `Application.Run`'dan once bagli | kod incelemesi |
| 5 | Iki bacakta `dotnet build` -> 0 hata | build ciktisi |

---

## §9 Kanit

| Kaynak dosya | Ne alindi |
|---|---|
| `knowledge\winforms\composition-root-host-startup.md` (kaynak vault) | 4 bolum tablosu, kalip kodu, sira tablosu, global handler ornegi |
| `.rules\coding-standards.md` | tek dogruluk kaynagi ve `#if` siniri |
| `.rules\async-rules.md` | handler baglama sirasi ile ilgili asenkron kural |
| `knowledge\winforms\di-container-lifetimes.md` | omur kayitlari |

> Kanitsiz iddia icin `DOĞRULAMA GEREKLI` yazilir; bu dosyada kaynak disi bilgi yoktur.
