---
title: "WinForms Form + Designer Sablonu (form tresligi + RibbonForm)"
type: template
category: templates
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "Her WinForms formun uretim kurali: form.cs / form.Designer.cs / form.resx tresligi, tum UI kodu Designer.cs'te, event handler form.cs'te"
---

# WinForms Form + Designer Sablonu

**Zorunlu Baglantilar:** [index](../index.md) · [CLAUDE](../CLAUDE.md) · [platform-arastirma](../reports/12-platform-arastirmasi.md) · [new-doc-contract](new-doc-contract.md)

> **Tek cumlelik sorumluluk:** *Yeni bir form hangi 3 dosyadan olusur, UI kodu
> nereye yazilir, Ribbon nasil kurulur* sorusunun web kanitli tek cevabidir.

---

## §0 Ozet (Answer First)

- **3 dosya zorunlu:** `<Ad>.cs` (mantık + event handler) · `<Ad>.Designer.cs`
  (**tüm UI design kodu**) · `<Ad>.resx` (kaynak) — `partial class` aynı isimde.
- **UI kodu tek yöntemde:** her kontrol `InitializeComponent()` içine yazılır;
  olay işleyicileri **asla** `.Designer.cs`'e yazılmaz (tasarımcı yeniden üretir, siler).
- **Anapencere = `RibbonForm`** (G3); `RibbonControl.RibbonStyle = Office2010`
  açıkça atanır (varsayılan değil, `Default = Office2013`).
- **net48 / net10.0-windows çift bacak** (G6): kod her iki TFM'de derlenir.

---

## §1 Dosya üçlüsü

| # | Dosya | İçerik | Sahip |
|---|---|---|---|
| 1 | `<Ad>.cs` | sınıf, constructor, `InitializeComponent()` çağrısı, **tüm event handler** | developer |
| 2 | `<Ad>.Designer.cs` | **tüm UI design kodu**: kontrol alanları + `InitializeComponent()` + `Dispose()` | ui-designer |
| 3 | `<Ad>.resx` | ikon/metadata; VS tarafından yönetilir | (elle dokunma) |

> `WEB_VERIFIED — github.com/dotnet/winforms docs/designer + learn.microsoft.com (2026-10-04)`
> (`[12-platform-arastirmasi](../reports/12-platform-arastirmasi.md)` §4)

## §2 Designer.cs iskeleti

```text
namespace <Proje>.UI
{
    partial class <Ad>Form
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing) { /* VS uretimi - elle DOKUNMA */ }

        private void InitializeComponent()
        {
            // TUM UI DESIGN KODU BURADA: kontrol tanimi, konum, boyut, event BAGLAMA
            // olay IMPLEMENTASYONU bu dosyada DEGIL, <Ad>.cs icinde
        }
    }
}
```

| # | Kural | Neden |
|---|---|---|
| 1 | Event **bağlaması** (`this.btn.Click += ...`) Designer.cs'te olabilir | VS üretir |
| 2 | Event **gövdesi** `<Ad>.cs`'te olur | yeniden üretimde kaybolmaz |
| 3 | VS 17.5+ `this.` nitelikleyicisi varsayılan olarak kalkar | Roslyn codegen |
| 4 | Eski stil gerekiyorsa `.editorconfig` → `[*.designer.cs]` bloğu | tutarlılık |

## §3 RibbonForm sablonu (ana pencere)

```text
public partial class AnaForm : RibbonForm
{
    public AnaForm()
    {
        InitializeComponent();
    }
}

// Designer.cs icinde:
// ribbonControl1.RibbonStyle = RibbonControlStyle.Office2010;
// RibbonPage + RibbonPageGroup + RibbonButtonItem sirasi
```

| # | Öğe | Not |
|---|---|---|
| 1 | `RibbonForm` | Ribbon'ın en iyi taşıyıcısı (DevExpress resmi) |
| 2 | `RibbonControlStyle.Office2010` | enum geçerli; **açıkça** set edilir |
| 3 | `Default` | v17.1+ = Office2013 — istenen stil bu değilse sakat |
| 4 | Sürüm | vault G3 = **v26.1** (29.07.2026 yayında) — `CONFLICT DETECTED` için [keys](../keys.md) ve 12-rapor §6 |

## §4 RED sinyalleri

| # | Sinyal | Düzeltme |
|---|---|---|
| 1 | UI kodu `<Ad>.cs` içinde | Designer.cs'e taşı |
| 2 | Event gövdesi Designer.cs'te | `<Ad>.cs`'e taşı |
| 3 | `class Ad : Form` (anapencere) | `RibbonForm` + `RibbonControlStyle.Office2010` |
| 4 | Tek dosyalı form (Designer yok) | üçlüyü oluştur |
| 5 | Sürüm 26.1 (G3 tek sürüm; 25.x RED) | `Directory.Packages.props` tek DevExpress satırı |
