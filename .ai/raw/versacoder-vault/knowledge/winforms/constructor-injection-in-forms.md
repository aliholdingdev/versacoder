---
title: "WinForms — Formlarda Constructor Injection Kalıbı"
type: knowledge
category: winforms
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
scope: "Bir formun bağımlılıklarını nasıl aldığını, tasarımcının neden parametresiz kurucu istediğini ve `null` yolunun nasıl kapandığını tanımlar"
---

# Formlarda Constructor Injection Kalıbı

**Bağlantılar:** [index.md](index.md) · [../../CLAUDE.md](../../CLAUDE.md)

## §0 Özet (Answer First)

Form bağımlılıklarını **constructor üzerinden `interface` olarak** alır; `new SomeService()` ve statik servis locatör **yasaktır**. Tek sorun: Visual Studio tasarımcısı **parametresiz kurucu** ister — bu yüzden `XxxForm()` tasarımcı için, `XxxForm(ISettingsService)` gerçek kullanım içindir ve **ikisi de `InitializeComponent` çağırır**. Tasarımcı kurucusu yalnızca `.cs`'te bulunur, `.Designer.cs`'e girmez.

| # | Kurucu | Kullanım | İçerdiği |
|---|---|---|---|
| 1 | `XxxForm()` | **yalnız** tasarımcı açılışı | `InitializeComponent()` |
| 2 | `XxxForm(IAlbum a, IErrorReporter r)` | çalışma zamanı (DI) | `InitializeComponent()` + alan atamaları |
| 3 | `XxxForm(IAlbum a) : this()` | opsiyonel zincirleme | `InitializeComponent()` bir kez |

### §1.1 İki kurucu kalıbı

**Kalıp A — parametresiz kurucu yok (tasarımcı açılmaz):**

```csharp
public SettingsForm(ISettingsService settings)
{
    InitializeComponent();
    _settings = settings;
}
// Tasarımcı: "SettingsForm could not be created because it has no parameterless constructor"
```

**Kalıp B — önerilen (tasarımcı + DI birlikte çalışır):**

```csharp
public partial class SettingsForm : RibbonForm
{
    private readonly ISettingsService _settings;

    public SettingsForm()                  // yalnız tasarımcı için
    {
        InitializeComponent();
    }

    public SettingsForm(ISettingsService settings) : this()
    {
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
    }

    private async void SettingsForm_Load(object sender, EventArgs e)
    {
        if (_settings is null)
        {
            // Tasarımcı açılışı: yüklemeyi atla, ekrana uyarı yaz
            _errorProvider.SetError(tabMain, "Bağımlılıklar çözümlenemedi.");
            return;
        }
        await LoadAsync();
    }
}
```

### §1.2 `null` yolu tablosu

| # | Durum | Davranış | Neden |
|---|---|---|---|
| 1 | DI ile açıldı | servis dolu, `Load` çalışır | normal yol |
| 2 | Tasarımcıda açıldı | servis `null` → `Load` erken döner | tasarımcı crash olmaz |
| 3 | Servis yanlış kayıtlı | `ArgumentNullException` | fail-fast, baştan |
| 4 | Opsiyonel bağımlılık | `IOptionalService?` + null kontrol | belirsizlik gizlenmez |

### §1.3 `.cs` sınırı (tekrar)

Tasarımcı kurucusu `.Form.cs`'te olsa bile **içinde tasarım kodu yoktur**: tek satır `InitializeComponent()`. Kontrol üretimi, `SuspendLayout`, `ResumeLayout`, `BeginInit` `.Designer.cs`'in tekelidir — bu hem `MainForm` hem alt formlar için geçerlidir.

### §1.4 `.csproj` iki bacak notu

Bağımlılık kayıtları `.csproj`'da değil **kodda** olduğu için iki bacakta aynıdır; yalnızca container kütüphanesi sürümü `Directory.Packages.props` (CPM) üzerinden **tek yerde** tanımlıdır — proje başına sürüm yazmak yasaktır (G8).

## §2 Uygulama Kontrol Listesi

| # | Adım | Kanıt |
|---|---|---|
| 1 | Her formda en az 1 DI kurucusu var | kod incelemesi |
| 2 | Tasarımcı için parametresiz kurucu var | tasarımcı açılışı |
| 3 | `?? throw new ArgumentNullException` ya da açık `null` kontrolü var | kod incelemesi |
| 4 | `.Form.cs`'te `new SomeService()` yok | grep = 0 |
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
| 5 | Kanıtsız "bitti" (G5) · bozuk iç link (G4) | RED |
