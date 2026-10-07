---
title: "WinForms — Composition Root ve Program.cs Başlangıç Akışı"
type: knowledge
category: winforms
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
scope: "Uygulamanın nerede kurulduğunu, `Program.cs`'in ne içerip ne içeremeyeceğini, `#if` platform sınırını ve container'ın yaşam döngüsünü tanımlar"
---

# Composition Root ve `Program.cs` Başlangıç Akışı

**Bağlantılar:** [index.md](index.md) · [../../CLAUDE.md](../../CLAUDE.md)

## §0 Özet (Answer First)

**Composition root** (`Program.cs` + `Host` katmanı) tüm bağımlılıkları **tek yerde** birleştirir; başka hiçbir katman `IServiceProvider` ya da somut servis `new`'lemez. `Program.cs` yalnızca üç şey yapar: **(1)** platform başlatma, **(2)** servis kayıtları, **(3)** `Application.Run(mainForm)`. `Host` katmanı iş mantığı **içermez** (kompozisyon + startup); `#if` yalnız platform/API farkıyla sınırlıdır.

| # | Bölüm | İçerdiği | İçermesi yasak |
|---|---|---|---|
| 1 | Platform başlatma | `ApplicationConfiguration.Initialize()` / `EnableVisualStyles` | iş mantığı |
| 2 | Servis kayıtları | `IServiceCollection` kayıtları | SQL, dosya I/O |
| 3 | Ana form çözümleme | `GetRequiredService<MainForm>()` + `Application.Run` | hata günlüğü mantığı |
| 4 | Global exception hookup | `Application.ThreadException`, `AppDomain.UnhandledException` | try/catch ile gizleme |

### §1.1 `Program.cs` kalıbı (iki bacak)

```csharp
[STAThread]
static void Main()
{
#if NETFRAMEWORK              // yalniz platform/API farki (G6)
    Application.EnableVisualStyles();
    Application.SetCompatibleTextRenderingDefault(false);
#else
    ApplicationConfiguration.Initialize();
#endif

    using var host = BuildHost();                 // composition root
    RegisterGlobalHandlers(host.Services);

    Application.Run(host.Services.GetRequiredService<MainForm>());
}

static IServiceProvider BuildHost()
{
    var services = new ServiceCollection();
    services.AddVersaCoderCore();     // Application + Infrastructure (Dapper)
    services.AddVersaCoderUi();       // formlar (transient)
    return services.BuildServiceProvider();
}
```

### §1.2 Kurulum sırası — sıra kritik

| # | Adım | Sıra bozulursa |
|---|---|---|
| 1 | `Initialize` / `EnableVisualStyles` | tema tutarsız, DPI sıçraması |
| 2 | Container kur | form çözümlenemez |
| 3 | Global handler'ları **container kurulduktan sonra** bağla | handler `ILogger`'ı erken çözer |
| 4 | Ana formu çözümlenir | `Form` transient ise her çağrı yeni örnek |
| 5 | `Application.Run` | uygulama hiç başlamaz |

### §1.3 `Host` katmanı ne yapar — ne yapmaz

| Yapar ✗ | Yapmaz ✗ |
|---|---|
| Servis kayıtları | İş mantığı / use case |
| `ServiceProvider` kurulumu | Doğrudan `SqliteCommand` |
| Uygulama yaşam döngüsü (`Application.Run`) | UI kontrol erişimi |
| Global handler bağlama | Log formatı kararı (o `ILogger`'da) |

> **KURAL:** `Host` içinde tek bir iş mantığı satırı bile yoksa **kanıttır**: dosyanın satır sayısı ve `grep` çıktısı. `Host` şişerse kompozisyon test edilemez hale gelir.

### §1.4 Global exception hookup özeti (bkz. `global-exception-handling`)

```csharp
Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
Application.ThreadException += (s, e) => reporter.Report(e.Exception);
AppDomain.CurrentDomain.UnhandledException += (s, e) =>
    reporter.Report(e.ExceptionObject as Exception);
```

## §2 Uygulama Kontrol Listesi

| # | Adım | Kanıt |
|---|---|---|
| 1 | `Program.cs` yalnız 4 bölüm içeriyor | kod incelemesi |
| 2 | `#if` yalnız platform/API farkı | grep `#if` |
| 3 | Container tek yerde kuruluyor (`BuildHost`) | grep `BuildServiceProvider` = 1 |
| 4 | Global handler'lar `Application.Run`'dan önce bağlı | kod incelemesi |
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
| 3 | EF Core · `DbContext` · string-concat SQL (G2) | CRITICAL |
| 4 | `.Form.cs`'e kontrol üretimi / `SuspendLayout` / `ResumeLayout` / `BeginInit` yazmak | form üçlüsü RED |
| 5 | Kanıtsız "bitti" (G5) · bozuk iç link (G4) · `#if` ile iş mantığı (G6) | RED |
