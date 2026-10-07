---
title: "WinForms — Service Locator Yasaklama ve Doğru Alternatif"
type: knowledge
category: winforms
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
scope: "Neden statik servis locatör kullanılmadığını, gizli bağımlılık yarattığını ve elle yazılmış locatörün kod içinde nasıl tespit edilip constructor injection'a çevrileceğini"
---

# Service Locator Yasaklama ve Doğru Alternatif

**Bağlantılar:** [index.md](index.md) · [../../CLAUDE.md](../../CLAUDE.md)

## §0 Özet (Answer First)

`ServiceLocator.Get<IX>()`, `HttpContext`/`ServiceProvider` **static** erişimi ve `SomeClass.Instance` deseni **gizli bağımlılık** üretir: sınıf hangi servislere ihtiyacı olduğunu **açıkça söylemez**, bu yüzden test etmek için önce locator'ı kurmak gerekir. Doğru alternatif **constructor injection**'dır: bağımlılık **imzada** görünür, `null` yolu baştan kapanır, test için sahte nesne geçilir.

| # | Yasaklı desen | Belirti | Doğrusu |
|---|---|---|---|
| 1 | `ServiceLocator.Get<T>()` | sınıfın imzasında bağımlılık yok | constructor injection |
| 2 | `static ServiceProvider` alanı | testte kurulum zorunlu | composition root + imza |
| 3 | `SomeService.Instance` | global mutable state | tekil servis `Singleton` + imza |
| 4 | `new SomeService()` kod içinde | sürüm/parametre değişince kırılır | `ISomeService` + DI |
| 5 | `Form.ActiveForm` üzerinden servis alma | sahiplik belirsiz | bağımlılığı kurucuda al |

### §1.1 Kötü → iyi (aynı form)

```csharp
// YASAKLI — gizli bağımlılık
private void btnSave_Click(object sender, EventArgs e)
{
    var settings = AppServices.Get<ISettingsService>();   // locator
    settings.Save();                                       // nereden geldi bilinmiyor
}
```

```csharp
// DOĞRU — bağımlılık imzada
public sealed class SettingsForm : RibbonForm
{
    private readonly ISettingsService _settings;

    public SettingsForm(ISettingsService settings)
    {
        InitializeComponent();
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
    }

    private async void btnSave_Click(object sender, EventArgs e)
        => await _settings.SaveAsync();
}
```

### §1.2 Tespit komutları (kanıt üretir)

```powershell
# Locator desenlerini tara — beklenen: 0 satır
Select-String -Path "src\**\*.cs" -Pattern "ServiceLocator|GetService<|AppServices\.|\.Instance\b"
Select-String -Path "src\**\*.cs" -Pattern "static\s+\w*ServiceProvider"
# Beklenen cikti: 0 satir  (G5 kaniti: cikti dosyaya yapistirilir)
```

### §1.3 Nesne grafiği neden gizlenmez

```
SettingsForm --(imza)--> ISettingsService
                              ^
                              |  kayit: Host katmaninda 1 satir
                     SettingsService(Dapper, parameterized SQL)
```

| # | Locator | Constructor injection |
|---|---|---|
| 1 | Bağımlılık gizli | Bağımlılık **imzada** |
| 2 | Test için static kurulum | Test için sahte nesne |
| 3 | `null` çalışma zamanında | `null` kurucuda (fail-fast) |
| 4 | Döngüsel bağımlılık geç fark edilir | derleyici/test erken yakalar |
| 5 | Uygulama başlangıcı yavaş | yalnız ihtiyaç kadar çözümlenir |

### §1.4 İstisna: composition root

Locator kalıbı **yalnızca** composition root'ta (`Host` katmanı, §1.2 `Program.cs`) kabul edilir; orası zaten bağımlılıkları kurmak için vardır. UI katmanında (`l7-ui`) locator = mimari RED.

## §2 Uygulama Kontrol Listesi

| # | Adım | Kanıt |
|---|---|---|
| 1 | Locator taraması = 0 satır | §1.2 çıktısı |
| 2 | Form kurucuları `interface` alıyor | kod incelemesi |
| 3 | `.Form.cs` içinde `new SomeService()` yok | grep = 0 |
| 4 | Sınıf içi `static` servis alanı yok | grep |
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
| 4 | `.Form.cs`'e kontrol üretimi / `SuspendLayout` / `ResumeLayout` / `BeginInit` yazmak · UI'da service locator | form üçlüsü / mimari RED |
| 5 | Kanıtsız "bitti" (G5) · bozuk iç link (G4) | RED |
