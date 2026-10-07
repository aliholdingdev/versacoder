---
title: "WinForms — Global Exception Yakalama ve Hata Raporlama"
type: knowledge
category: winforms
status: active
version: 1.0.0
updated: 2026-0-25
authority: SSOT
scope: "Yakalanmayan istisnaların nereye bağlandığını, hangi durumda uygulamanın yaşamaya devam ettiğini ve hata raporunun hangi alanları içerdiğini tanımlar"
---

# Global Exception Yakalama ve Hata Raporlama

**Bağlantılar:** [index.md](index.md) · [../../CLAUDE.md](../../CLAUDE.md)

## §0 Özet (Answer First)

Üç yakalama yüzeyi **hepsi** bağlanır ve **hepsi** tek `IErrorReporter`'a gider: `Application.ThreadException` (UI thread), `AppDomain.CurrentDomain.UnhandledException` (diğer thread'ler), `TaskScheduler.UnobservedTaskException` (atlanmış task'ler). Kural: **uygulama mümkünse yaşamaya devam eder**, hata günlüğe ve kullanıcıya **tek sefer** yazılır; `MessageBox` her yerde ayrı ayrı **çıkarılmaz**.

| # | Yüzey | Yakalar | Sonuç |
|---|---|---|---|
| 1 | `Application.ThreadException` | UI thread olay hataları | rapor + form yaşamaya devam eder |
| 2 | `AppDomain.UnhandledException` | havuz thread'leri | rapor + uygulama **kapanabilir** (bilinmeyen durum) |
| 3 | `TaskScheduler.UnobservedTaskException` | `await` edilmemiş task | rapor + uygulama yaşar |
| 4 | Olay işleyici `try/catch` | bilinen hata | rapor + kullanıcıya göster (bkz. `async-void-event-handlers`) |
| 5 | `Application.SetUnhandledExceptionMode(CatchException)` | 1–3'ü etkinleştirir | başlangıçta |

### §1.1 Bağlama kalıbı (`Program.cs`)

```csharp
Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

var reporter = host.Services.GetRequiredService<IErrorReporter>();

Application.ThreadException += (s, e) => reporter.Report(e.Exception);
AppDomain.CurrentDomain.UnhandledException += (s, e) =>
    reporter.Report(e.ExceptionObject as Exception);
TaskScheduler.UnobservedTaskException += (s, e) =>
{
    reporter.Report(e.Exception);
    e.SetObserved();                 // tekrar GC patlamasi engellenir
};
```

> **KURAL:** Handler'lar **container kurulduktan sonra** ve **`Application.Run`'dan önce** bağlanır; `reporter`'ı erken çözmek `InvalidOperationException` üretir.

### §1.2 Raporda bulunması zorunlu alanlar

| # | Alan | Örnek |
|---|---|---|
| 1 | Zaman (`UTC`) | `2026-09-25T10:14:03Z` |
| 2 | Bacak | `net48` / `net10.0-windows` |
| 3 | Olay yığını (tam) | `System.InvalidOperationException at ...` |
| 4 | Kullanıcı bağlamı | ekran, form adı, işlem |
| 5 | Sürüm | uygulama + DevExpress **v26.1** |

### §1.3 Yapılmayanlar

| # | Yasak | Neden |
|---|---|---|
| 1 | `catch { }` boş kafes | neden gizlenir (G9) |
| 2 | Her yerde ayrı `MessageBox.Show` | kullanıcı 3 dialog görür |
| 3 | Hata mesajını `ex.ToString()` ile ham göstermek | güvenlik + anlaşılmazlık |
| 4 | Hatayı günlüğe yazmamak | teşhis edilemez |
| 5 | Handler'ı `Application.Run`'dan **sonra** bağlamak | ilk hata kaçar |

### §1.4 Akış (ASCII)

```
olay isleyicisi
   |
   +-- try/catch  --> IErrorReporter.Report(ex)
   |                        |
   +-- catch yok            +--> ILogger (dosya)
                            +--> GUI: durum cubugu / dialog (tek yuzey)
                            +--> rapor kaydi (dosya:zaman:stack)
```

## §2 Uygulama Kontrol Listesi

| # | Adım | Kanıt |
|---|---|---|
| 1 | Üç global handler bağlı | kod incelemesi |
| 2 | `SetUnhandledExceptionMode(CatchException)` mevcut | grep |
| 3 | Handler'lar `Run`'dan önce | kod sırası |
| 4 | Boş `catch { }` yok | grep = 0 |
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
| 5 | Kanıtsız "bitti" (G5) · bozuk iç link (G4) · boş `catch` (G9) | RED |
