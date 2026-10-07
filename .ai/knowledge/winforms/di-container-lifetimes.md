---
title: "di-container-lifetimes - VersaCoder"
type: knowledge
category: winforms
status: active
version: 1.0.0
updated: 2026-09-29
authority: SSOT
scope: "Singleton/transient omurlerinin hangi servise verilecegi, WinForms'ta scope karsiligi (form omru) ve yanlis omurun yarattigi bellek sizintisi"
---

# DI Container Omurleri ve Form Kaydi

**Zorunlu Baglantilar:** [vault indeks](../index.md) · [coding-standards](../../.rules/coding-standards.md) · [composition-root-host-startup](composition-root-host-startup.md) · [settings-user-config](settings-user-config.md) · [logging-integration](logging-integration.md)
**Küme + Hub:** [winforms index](index.md) · [index](../../index.md) · [CLAUDE](../../CLAUDE.md) · [AGENTS](../../AGENTS.md) · [brain](../../brain.md) · [glossary](../../glossary.md) · [keys](../../keys.md)
**Kardeşler (winforms · 4/32):** [accessibility-tab-and-mnemonics](accessibility-tab-and-mnemonics.md) · [anchor-dock-layout-rules](anchor-dock-layout-rules.md) · [async-await-ui-thread](async-await-ui-thread.md) · [async-loading-patterns](async-loading-patterns.md) · [async-ui-thread-marshalling](async-ui-thread-marshalling.md) · [async-void-event-handlers](async-void-event-handlers.md) · [backgroundworker-vs-task](backgroundworker-vs-task.md) · [cancellation-in-ui](cancellation-in-ui.md) · [cancellation-token-in-ui](cancellation-token-in-ui.md) · [control-disposal-and-memory](control-disposal-and-memory.md) · [controls-lifecycle-events](controls-lifecycle-events.md) · [data-binding-basics](data-binding-basics.md) · [designer-cs-anatomy](designer-cs-anatomy.md) · [designer-cs-trio-rule](designer-cs-trio-rule.md) · [designer-editing-rules](designer-editing-rules.md) · [designer-hand-edit-policy](designer-hand-edit-policy.md) · [docking-panels-patterns](docking-panels-patterns.md) · [errorprovider-validation-flow](errorprovider-validation-flow.md) · [exception-boundary-ui](exception-boundary-ui.md) · [form-close-and-dispose](form-close-and-dispose.md) · [form-close-cancel-pattern](form-close-cancel-pattern.md) · [form-constructor-vs-load](form-constructor-vs-load.md) · [form-load-and-shown-events](form-load-and-shown-events.md) · [layout-table-panel](layout-table-panel.md) · [partial-class-and-file-naming](partial-class-and-file-naming.md) · [splitcontainer-and-sizing](splitcontainer-and-sizing.md) · [validation-form-level](validation-form-level.md) · [winforms-rendering-performance](winforms-rendering-performance.md)

> **Tek cumlelik sorumluluk:** *Hangi servis hangi omre kaydedilir, form omrusu nasil yonetulur* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

WinForms'ta **scope kavrami yoktur**; karsiligi **form omrudur**. `Singleton` uzun omurlu tekil servisler icindir (ayar, loglama, baglanti fabrikasi), `Transient` her acilista yeni istenen form ve durumlu servisler icindir. Bir servis `Singleton` olup da formu tutarsa **bellek sizintisi** olusur: form GC'ye gitmez, `FormClosed` temizligi calismayabilir. Container yalniz **composition root**'ta kurulur; hicbir yerde `static` servis locator olarak kullanilmaz.

```text
+------------------------------------------------------------+
| Program.cs -> BuildHost()  (composition root)              |
|      |                                                     |
|      +--> Singleton : logger, settings, connection factory |
|      +--> Transient : formlar, repository, durumlu servis  |
|      |                                                     |
|      +--> Application.Run(MainForm)                        |
|                 +-- ShowDialog(SettingsForm) -> Dispose    |
+------------------------------------------------------------+
```

---

## §1 Omur secim tablosu

| Servis | Omur | Neden | Sizinti riski |
|---|---|---|---|
| `ILogger` | Singleton | durumsuz | yok |
| `ISettingsStore` | Singleton | tek dosya kaynagi | yok |
| `IDbConnectionFactory` | Singleton | baglanti havuzu maliyeti | yok |
| `ISessionRepository` (Dapper) | **Transient** | kisa omurlu `DbConnection` | Singleton ise baglanti tasar |
| `SettingsForm` | **Transient** | her acilista yeni pencere | Singleton ise form yeniden kullanilamaz |
| `IUiState` | **Transient** | forma bagli durum | Singleton ise formlar birbirini kirletir |
| `IFormFactory` | Singleton | uretim tek yerde | yok |

---

## §2 Kayit ve cozumleme kalibi

```csharp
// Host katmani - composition root
public static IServiceCollection AddVersaCoderUi(this IServiceCollection services)
{
    services.AddSingleton<ILogger, FileLogger>();
    services.AddSingleton<ISettingsStore, JsonSettingsStore>();
    services.AddSingleton<IDbConnectionFactory, SqliteConnectionFactory>();
    services.AddTransient<ISessionRepository, SessionRepository>();   // ADO.NET + Dapper

    services.AddTransient<SettingsForm>();       // form transient
    services.AddTransient<MainForm>();
    return services;
}

// Formu her seferinde yeni acmak icin factory (asla tek ornek saklama)
using var dlg = _formFactory.Create<SettingsForm>();
dlg.ShowDialog(this);
```

Form **provider'dan cozumlenip `using` bloguna** alinir ya da `Dispose` edilir.

---

## §3 Yaygin sizinti desenleri

| # | Desen | Belirti | Duzeltme |
|---|---|---|---|
| 1 | `AddSingleton<SettingsForm>()` | ikinci acilista eski form | `AddTransient` |
| 2 | Singleton servis olayina form abone | form GC olmaz | `FormClosed`'da `-=` |
| 3 | `IDbConnection` singleton | "connection already open" | `Transient` + `using` |
| 4 | Container'i statik alanda tutmak | test edilemez | composition root'a tasi |
| 5 | Form icinde `new Service()` | bagimlilik gizlenir | constructor injection |

---

## §4 Kontrol listesi

| # | Adim | Kanit |
|---|---|---|
| 1 | Formlar `Transient` | host kaydi |
| 2 | Repository / `IDbConnection` `Transient` | host kaydi |
| 3 | Uzun omurlu servislerde forma abonelik yok | grep `-=` + `FormClosed` |
| 4 | `new Service()` elle cagrisi yok (yalniz arayuz) | grep |
| 5 | Iki bacakta `dotnet build` -> 0 hata | build ciktisi |

---

## §9 Kanit

| Kaynak dosya | Ne alindi |
|---|---|
| `knowledge\winforms\di-container-lifetimes.md` (kaynak vault) | omur secim tablosu, kayit kalibi, 5 sizinti deseni, ASCII omur semasi |
| `.rules\coding-standards.md` | arayuz uzerinden bagimlilik kurali |
| `knowledge\winforms\composition-root-host-startup.md` | container'in tek yerde kurulmasi |

> Kanitsiz iddia icin `DOĞRULAMA GEREKLI` yazilir; bu dosyada kaynak disi bilgi yoktur.
