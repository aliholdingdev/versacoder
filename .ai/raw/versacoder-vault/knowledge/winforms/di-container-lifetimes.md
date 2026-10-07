---
title: "WinForms — DI Container Ömürleri ve Form Kaydı"
type: knowledge
category: winforms
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
scope: "Masaüstü uygulamasında singleton/transient/scoped ömürlerinin hangi servise verileceği, WinForms'ta scope'un nasıl kurulacağı ve yanlış ömür belirtmenin yarattığı sızıntı"
---

# DI Container Ömürleri ve Form Kaydı

**Bağlantılar:** [index.md](index.md) · [../../CLAUDE.md](../../CLAUDE.md)

## §0 Özet (Answer First)

WinForms'ta **scope kavramı yoktur**; karşılığı **form ömrüdür**. `Singleton` uzun ömürlü tekil servisler (ayar, loglama, bağlantı fabrikası), `Transient` her açılışta yeni istenen form ve durumlu servisler içindir. Bir servis `Singleton` olup da formu tutarsa → **bellek sızıntısı**: form GC'ye gitmez. Container **composition root**'ta (`Host` katmanı) kurulur, hiçbir yerde `static` servis locatör olarak kullanılmaz.

| # | Ömür | Ne zaman yeni örnek | Tipik kullanım |
|---|---|---|---|
| 1 | `Singleton` | uygulama boyunca 1 kez | `ILogger`, `ISettingsStore`, `IConnectionFactory`, `IHttpHandler` |
| 2 | `Transient` | her `GetRequiredService` | form nesneleri, durumlu servisler, `IErrorReporter` |
| 3 | `Scoped` *(isteğe bağlı)* | her kapsam sonunda | WinForms'ta yok → form başına `IDisposable` servis |
| 4 | `Factory` | her çağrı | `IFormFactory` → `SettingsForm` üretimi |

### §1.1 Ömür seçim tablosu

| Servis | Ömür | Neden | Sızıntı riski |
|---|---|---|---|
| `ILogger<T>` | Singleton | durumsuz | yok |
| `ISettingsStore` | Singleton | tek dosya kaynağı | — |
| `IDbConnectionFactory` | Singleton | bağlantı havuzu maliyeti | — |
| `ISessionRepository` (Dapper) | **Transient** | kısa ömürlü `DbConnection` | `Singleton` yapılırsa bağlantı taşar |
| `SettingsForm` | **Transient** | her açılışta yeni pencere | `Singleton` yapılırsa form yeniden kullanılamaz |
| `IUiState` | **Transient** | forma bağlı durum | `Singleton` yapılırsa formlar birbirini kirletir |

### §1.2 Kayıt ve çözümleme kalıbı

```csharp
// Host katmanı — composition root
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
```

```csharp
// Formu her seferinde yeni açmak için factory (asla tek örnek saklama)
using var dlg = _formFactory.Create<SettingsForm>();
dlg.ShowDialog(this);
```

> **KURAL:** Form **provider'dan çözümlenip `using` bloğuna** alınır ya da `Dispose` edilir. Container'a `Singleton` kaydedilmiş bir form, kapandıktan sonra bellekte kalır ve `FormClosed` temizliği **hiç çalışmayabilir**.

### §1.3 Yaygın sızıntı desenleri

| # | Desen | Belirti | Düzeltme |
|---|---|---|---|
| 1 | `AddSingleton<SettingsForm>()` | ikinci açılışta eski form | `AddTransient` |
| 2 | Singleton servis, olayına form abone | form GC olmaz | `FormClosed`'da `-=` |
| 3 | `IDbConnection` singleton | "connection already open" | `Transient` + `using` |
| 4 | Container'ı statik alanda tutmak | test edilemez | composition root'a taşı |
| 5 | Form içinde `new Service()` | bağımlılık gizlenir | constructor injection |

### §1.4 Yaşam döngüsü şeması (ASCII)

```
Program.Main
  |
  +-- BuildContainer()        (composition root, Host katmani)
  |        +-- Singleton'lar uretilir (1 kez)
  |
  +-- Application.Run(MainForm)
             |
             +-- MainForm (transient)  -- ShowDialog --> SettingsForm (transient)
             |                                              |
             |                                    Close -> Dispose
             |
  +-- container as IDisposable -> Dispose() (Singleton'lar burada)
```

## §2 Uygulama Kontrol Listesi

| # | Adım | Kanıt |
|---|---|---|
| 1 | Formlar `Transient` | host kaydı |
| 2 | `IDbConnection` / repo `Transient` | host kaydı |
| 3 | Uzun ömürlü servislerde forma abonelik yok | grep `-=` + `FormClosed` |
| 4 | `new Service()` elle çağrı yok (yalnız `I` interface) | grep |
| 5 | İki bacakta `dotnet build` → 0 hata | build çıktısı |

## §3 İzinli Link Hedefleri

| # | Hedef | Bu dosyada |
|---|---|---|
| 1 | Kardeş dosya adları (bu dizindeki 30 dosya) | var |
| 2 | [index.md](index.md) — kayıt defteri | var |
| 3 | [../../CLAUDE.md](../../CLAUDE.md) — anayasa G1..G12 | var |
| 4 | `../../index.md` · `../../keys.md` · `../../VISION.md` · `../../.rules/*.md` · `../index.md` | **yok → link verilmez** (`NEEDS_VALIDATION`) |

## §4 Yasaklar

| # | Yasak | İhlalde |
|---|---|---|
| 1 | DevExpress **v26.1** dışı sürüm referansı (G3/G8) | iş RED |
| 2 | `RibbonControlStyle.Office2010` dışı ribbon stili (G3) | iş RED |
| 3 | EF Core · `DbContext` · string-concat SQL (G2) · magic connection string | CRITICAL |
| 4 | `.Form.cs`'e kontrol üretimi / `SuspendLayout` / `ResumeLayout` / `BeginInit` yazmak | form üçlüsü RED |
| 5 | Kanıtsız "bitti" (G5) · bozuk iç link (G4) | RED |
