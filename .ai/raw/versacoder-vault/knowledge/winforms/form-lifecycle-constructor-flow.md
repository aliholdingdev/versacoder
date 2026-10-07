---
title: "WinForms — Form Yaşam Döngüsü: Kurucu ve InitializeComponent Akışı"
type: knowledge
category: winforms
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
scope: "Bir form nesnesi `new` anından `Dispose`'a kadar hangi evrelerden geçer, hangi kod hangi dosyada bulunur ve hangi olay sırası garanti edilir"
---

# Form Yaşam Döngüsü: Kurucu ve `InitializeComponent` Akışı

**Bağlantılar:** [index.md](index.md) · [../../CLAUDE.md](../../CLAUDE.md)

## §0 Özet (Answer First)

Bir formun yaşam döngüsü **sabit beş evreden** geçer: `new XxxForm()` → `InitializeComponent()` → `Load` → `Shown` → `Close`/`Dispose`. **Tasarım kodunun tamamı** (kontrol üretimi, `SuspendLayout`, `ResumeLayout`, `BeginInit`) `.Designer.cs` içindeki tek `InitializeComponent` gövdesindedir; `.Form.cs` dosyası yalnızca **olay yönlendirici**dir. Kural `MainForm` dahil **tüm** formlar için geçerlidir.

| # | Evre | Tetikleyen | Hangi dosya | Tipik iş |
|---|---|---|---|---|
| 1 | Alan başlatma + `InitializeComponent()` | `new XxxForm()` | `.Form.cs` kurucusu (yalnız çağrı) + `.Designer.cs` (gövde) | Kontrol üretimi, `resources.ApplyResources`, olay bağlama |
| 2 | Kurucudan sonraki kod | kurucunun devamı | `.Form.cs` | Saf alan atamaları, bağımlılık tutma (UI çizmeden) |
| 3 | `Load` | ilk `Show`/`ShowDialog` çağrısı hemen öncesi | `.Form.cs` | Veri yükleme, tema/dpi ayarı, `async` doldurma |
| 4 | `Shown` | pencere ilk kez görünür olduğunda | `.Form.cs` | Odak, ilk yükleme tetikleyicisi, animasyon |
| 5 | `FormClosing` → `FormClosed` → `Dispose` | kapatma / sahiplik bitişi | `.Form.cs` (onay) + `.Designer.cs` (`Dispose`) | Kaydetme sorusu, temizlik |

### §1.1 Kurucu kalıbı (iki bacak)

```csharp
public partial class SettingsForm : RibbonForm
{
    private readonly ISettingsService _settings;

    public SettingsForm(ISettingsService settings)
    {
        InitializeComponent();          // tek tasarım kaynağı = .Designer.cs
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
    }

    private async void SettingsForm_Load(object sender, EventArgs e)
    {
        await LoadSettingsAsync();      // iş mantığı Application katmanına gider
    }
}
```

> **KURAL:** `InitializeComponent()` çağrısı kurucunun **ilk** satırıdır; ondan önce hiçbir kontrole erişilemez (`this.Controls` henüz dolu değildir) ve `Controls.Add`/`SuspendLayout`/`BeginInit` `.Form.cs`'e yazılmaz.

### §1.2 Olay sırası (garantili)

| # | Olay | Ne zaman | Kod-behind yazılır mı |
|---|---|---|---|
| 1 | (kurucu gövdesi) | `new` anında | yalnız `InitializeComponent()` + alan ataması |
| 2 | `HandleCreated` | pencere tutamaçı oluştuğu an | hayır (tasarım ayrıntısı) |
| 3 | `Load` | görünmeden hemen önce, **her** `Show`/`ShowDialog`'ta | evet — veri yükleme |
| 4 | `Shown` | ilk kez görünür | evet — ilk odak/animasyon |
| 5 | `Activated`/`Deactivate` | odak değişimi | gerektiğinde |
| 6 | `FormClosing` → `FormClosed` | kapatma | evet — onay + temizlik |
| 7 | `Dispose` | `using` bloğu / sahiplik sonu | hayır — `.Designer.cs` içinde |

### §1.3 `Load` neden "ilk sefere mahsus" değildir?

`Load` her `ShowDialog()` çağrısında **yeniden tetiklenir**; `Shown` ise yalnızca handle ilk oluşturulduğunda. Formu tekrar tekrar gösteriyorsanız doldurma mantığını `Load`'a değil, açıkça çağrılan bir `InitializeAsync()` metoda taşıyın — aksi halde ikinci açılışta eski veri görünür.

### §1.4 İki bacak notu (`net48` + `net10.0-windows`)

| Konu | `net48` | `net10.0-windows` | `#if` gerekli mi |
|---|---|---|---|
| `SynchronizationContext` | `WindowsFormsSynchronizationContext` | aynı | hayır |
| `async void Load` | desteklenir | desteklenir | hayır |
| Yüksek-DPI varsayılanları | `app.config` `DpiAwareness` | `ApplicationConfiguration.Initialize()` | **evet — yalnız platform/API farkı** |
| Top-level statements / `ApplicationConfiguration` | yok | var | `#if` yalnız burada |

`#if` **yalnız** bu platform farkı için kullanılır; iş mantığı asla `#if` içine alınmaz (`CLAUDE.md` G6).

## §2 Uygulama Kontrol Listesi

| # | Adım | Kanıt |
|---|---|---|
| 1 | Kurucuda ilk satır `InitializeComponent()` | kod incelemesi |
| 2 | `.Form.cs` içinde `Controls.Add`/`SuspendLayout`/`ResumeLayout`/`BeginInit` = **0 satır** | `grep` = 0 |
| 3 | `Load` içindeki doldurma `async` ve `await` ile | kod incelemesi |
| 4 | Tekrarlı açılan formda doldurma `Load`'tan taşındı | davranış testi |
| 5 | Her iki bacakta `dotnet build` → 0 hata | build çıktısı |

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
