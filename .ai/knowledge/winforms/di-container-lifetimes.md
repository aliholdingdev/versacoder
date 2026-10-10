---
title: "di-container-lifetimes - VersaCoder"
type: knowledge
category: winforms
status: active
version: 1.0.0
updated: 2026-09-29
authority: primary
scope: "Singleton/transient omurlerinin hangi servise verilecegi, WinForms'ta scope karsiligi (form omru) ve yanlis omurun yarattigi bellek sizintisi"
tags: [knowledge]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# DI Container Omurleri ve Form Kaydi

**Zorunlu Baglantilar:** [[knowledge/index|vault indeks]] · [[.rules/coding-standards|coding-standards]] · [[knowledge/winforms/composition-root-host-startup|composition-root-host-startup]] · [[knowledge/winforms/settings-user-config|settings-user-config]] · [[knowledge/winforms/logging-integration|logging-integration]]
**Küme + Hub:** [[knowledge/winforms/index|winforms index]] · [[index|index]] · [[claude|CLAUDE]] · [[definitions/agents|AGENTS]] · [[.decisions/brain|brain]] · [[definitions/glossary|glossary]] · [[governance/keys|keys]]
**Kardeşler (winforms · 4/32):** [[knowledge/winforms/accessibility-tab-and-mnemonics|accessibility-tab-and-mnemonics]] · [[knowledge/winforms/anchor-dock-layout-rules|anchor-dock-layout-rules]] · [[knowledge/winforms/async-await-ui-thread|async-await-ui-thread]] · [[knowledge/winforms/async-loading-patterns|async-loading-patterns]] · [[knowledge/winforms/async-ui-thread-marshalling|async-ui-thread-marshalling]] · [[knowledge/winforms/async-void-event-handlers|async-void-event-handlers]] · [[knowledge/winforms/backgroundworker-vs-task|backgroundworker-vs-task]] · [[knowledge/winforms/cancellation-in-ui|cancellation-in-ui]] · [[knowledge/winforms/cancellation-token-in-ui|cancellation-token-in-ui]] · [[knowledge/winforms/control-disposal-and-memory|control-disposal-and-memory]] · [[knowledge/winforms/controls-lifecycle-events|controls-lifecycle-events]] · [[knowledge/winforms/data-binding-basics|data-binding-basics]] · [[knowledge/winforms/designer-cs-anatomy|designer-cs-anatomy]] · [[knowledge/winforms/designer-cs-trio-rule|designer-cs-trio-rule]] · [[knowledge/winforms/designer-editing-rules|designer-editing-rules]] · [[knowledge/winforms/designer-hand-edit-policy|designer-hand-edit-policy]] · [[knowledge/winforms/docking-panels-patterns|docking-panels-patterns]] · [[knowledge/winforms/errorprovider-validation-flow|errorprovider-validation-flow]] · [[knowledge/winforms/exception-boundary-ui|exception-boundary-ui]] · [[knowledge/winforms/form-close-and-dispose|form-close-and-dispose]] · [[knowledge/winforms/form-close-cancel-pattern|form-close-cancel-pattern]] · [[knowledge/winforms/form-constructor-vs-load|form-constructor-vs-load]] · [[knowledge/winforms/form-load-and-shown-events|form-load-and-shown-events]] · [[knowledge/winforms/layout-table-panel|layout-table-panel]] · [[knowledge/winforms/partial-class-and-file-naming|partial-class-and-file-naming]] · [[knowledge/winforms/splitcontainer-and-sizing|splitcontainer-and-sizing]] · [[knowledge/winforms/validation-form-level|validation-form-level]] · [[knowledge/winforms/winforms-rendering-performance|winforms-rendering-performance]]

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
