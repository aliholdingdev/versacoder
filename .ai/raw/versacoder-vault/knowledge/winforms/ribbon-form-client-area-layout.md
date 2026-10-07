---
title: "WinForms — RibbonForm İç Alanı ve Ofis-2010 Ribbon Yerleşimi"
type: knowledge
category: winforms
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
scope: "`RibbonForm`'da ribbon, sayfa ve içerik alanının nasıl yerleştirildiğini, `RibbonControlStyle.Office2010` kilidini ve ribbon altındaki içeriğin nasıl dock'landığını tanımlar"
---

# `RibbonForm` İç Alanı ve Ribbon Yerleşimi

**Bağlantılar:** [index.md](index.md) · [../../CLAUDE.md](../../CLAUDE.md)

## §0 Özet (Answer First)

Ana pencere **hep `RibbonForm`**'dur ve `RibbonControl.Dock = Fill` ile forma yapışır; uygulama içeriği ribbon'un **altında kalan alan**dır ve `Dock = Fill` ile oraya yerleşir. `RibbonControlStyle` **yalnız `Office2010`**'dur (`RibbonControlStyle.Office2010Silver`/`Office2007` **yasak**). Ribbon ile ilgili **hiçbir** kod `.Form.cs`'e yazılmaz — kontrol üretimi, sayfa/kategori ekleme ve stil ataması `.Designer.cs`'tedir.

| # | Öğe | Değer / konum | Dosya |
|---|---|---|---|
| 1 | Form sınıfı | `RibbonForm` | `.cs` (taban sınıf) |
| 2 | `RibbonControl` | `Dock = Fill` | `.Designer.cs` |
| 3 | `RibbonControlStyle` | **`Office2010`** (tek izinli) | `.Designer.cs` |
| 4 | Sayfalar / kategoriler | `RibbonPageCollection` | `.Designer.cs` |
| 5 | İçerik (`GridControl`, editörler) | ribbon altındaki `Fill` paneli | `.Designer.cs` |

### §1.1 Kalıp

```csharp
// XxxForm.Designer.cs
this.ribbonControl1.Dock = System.Windows.Forms.DockStyle.Fill;
this.ribbonControl1.Style = DevExpress.XtraBars.Ribbon.RibbonControlStyle.Office2010;
this.ribbonControl1.Pages.Add(this.pageHome);
this.pageHome.Groups.Add(this.groupFile);
```

```csharp
// XxxForm.cs — ribbon'a ilişkin 0 satır
public partial class MainForm : RibbonForm
{
    private readonly IProjectService _projects;

    public MainForm(IProjectService projects)
    {
        InitializeComponent();      // ribbon burada kurulur
        _projects = projects;
    }

    private async void btnNew_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
    {
        await _projects.CreateAsync();   // yalnız olay yönlendiricisi
    }
}
```

### §1.2 Stil kilidi tablosu

| Stil | Durum | Sonuç |
|---|---|---|
| `RibbonControlStyle.Office2010` | **tek izinli** | geçerli |
| `Office2010Silver` / `Office2010Colorful` | yasak | G3 → CRITICAL, RED |
| `Office2007` / `OfficeXP` | yasak | G3 → CRITICAL, RED |
| `Default` | yasak | stil belirsiz |
| Kod içinde ikinci bir stil ataması | yasak | çelişki → RED |

```powershell
# Stil kilidi taramasi — beklenen: yalniz "Office2010"
Select-String -Path "src\**\*.cs" -Pattern "RibbonControlStyle\.\w+"
```

### §1.3 İçerik yerleşimi

```
+--------------------------------------------------+
| RibbonControl  (Dock = Fill, Style = Office2010)  |
|  + tab: [Gorunum] [Proje] [Arac]                 |
|  + panel (Content) ------------------------------+
|  |  grid / editör / terminal   (Dock = Fill)     |
|  +-----------------------------------------------+
| StatusBarControl (Dock = Bottom)                 |
+--------------------------------------------------+
```

| # | İçerik | `Dock` | Not |
|---|---|---|---|
| 1 | `GridControl` | `Fill` | ana veri görünümü |
| 2 | `SplitContainer` | `Fill` | iki panelli ekran |
| 3 | `StatusBarControl` | `Bottom` | durum + ilerleme |
| 4 | Yükleme göstergesi | `Fill` + `Visible=false` | busy durumu |

### §1.4 `XtraForm` yasağı

`XtraForm` **ana pencere olarak kullanılmaz** (G3). Alt pencere/`ShowDialog` formları da aynı ribbon kalıbını izler ya da standart `Form` kullanır — **ana pencere her koşulda `RibbonForm`**.

## §2 Uygulama Kontrol Listesi

| # | Adım | Kanıt |
|---|---|---|
| 1 | Ana pencere `RibbonForm` | kod incelemesi |
| 2 | Stil taraması yalnız `Office2010` | §1.2 komutu |
| 3 | `.Form.cs` içinde ribbon kodu yok | grep `ribbon` `.Form.cs` |
| 4 | İçerik `Dock = Fill` ribbon altında | görsel kanıt |
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
| 2 | `RibbonControlStyle.Office2010` **dışı** herhangi bir ribbon stili (G3) | CRITICAL — RED |
| 3 | EF Core · `DbContext` · string-concat SQL (G2) | CRITICAL |
| 4 | `.Form.cs`'e ribbon/kontrol üretimi, `SuspendLayout`/`ResumeLayout`/`BeginInit` yazmak | form üçlüsü RED |
| 5 | Kanıtsız "bitti" (G5) · bozuk iç link (G4) | RED |
