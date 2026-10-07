---
title: "WinForms — Designer.cs + .resx + Kod-Behind Üçlüsü Kuralı"
type: knowledge
category: winforms
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
scope: "Her formun neden tam üç dosyadan oluştuğunu, hangi dosyanın hangi yetkiye sahip olduğunu ve `.cs` dosyasında tasarım koduna neden izin verilmediğini tanımlar"
---

# `Designer.cs` + `.resx` + Kod-Behind Üçlüsü Kuralı

**Bağlantılar:** [index.md](index.md) · [../../CLAUDE.md](../../CLAUDE.md)

## §0 Özet (Answer First)

Her form **tam üç dosyadan** oluşur ve her dosyanın **tek bir sorumluluğu** vardır: `XxxForm.cs` yalnız kullanıcı mantığı, `XxxForm.Designer.cs` **tek `InitializeComponent` kaynağı**, `XxxForm.resx` **`ResourceManager` verisi**. `MainForm` dahil **hiçbir** formda kontrol oluşturma, `SuspendLayout`, `ResumeLayout` veya `BeginInit` kodu `.cs` dosyasına yazılmaz — bu kodun tek yeri `.Designer.cs` gövdesidir.

| # | Dosya | Tek sorumluluk | Yetki (son söz) | Bu dosyada YASAK |
|---|---|---|---|---|
| 1 | `XxxForm.cs` | Kullanıcı mantığı + olay **yönlendiricileri** | `developer` | `new Label()`, `Controls.Add`, `SuspendLayout`, `ResumeLayout`, `BeginInit`, `InitializeComponent` gövdesi |
| 2 | `XxxForm.Designer.cs` | **Tek** `InitializeComponent` kaynağı, tüm kontrol üretimi, olay `+=` bağları, `Dispose` | `ui-designer` | iş mantığı, `async`, servis çağrısı |
| 3 | `XxxForm.resx` | Metin/ikon/culture kaynakları, `ResourceManager` anahtarları | `ui-designer` | elle XML düzenleme, `ResourceManager` adını değiştirmek |

### §1.1 Ne görünür, ne görünmez (kontrol listesi)

```csharp
// ========== XxxForm.Designer.cs  (tek InitializeComponent) ==========
private void InitializeComponent()
{
    this.components = new System.ComponentModel.Container();
    this.SuspendLayout();
    this.btnSave = new DevExpress.XtraEditors.SimpleButton();
    // ...
    this.Controls.Add(this.btnSave);
    this.ResumeLayout(false);
}

// ========== XxxForm.cs  (designer kodu 0 satır) ==========
public partial class XxxForm : RibbonForm
{
    private readonly IThingService _things;

    public XxxForm(IThingService things)
    {
        InitializeComponent();
        _things = things;
    }

    private async void btnSave_Click(object sender, EventArgs e)
    {
        await _things.SaveAsync();      // iş mantığı Application katmanına
    }
}
```

### §1.2 `.cs`'e yazılmayan kod — tarama tablosu

| Aranacak | `.Form.cs` beklenen | `.Designer.cs` beklenen |
|---|---|---|
| `SuspendLayout(` | **0** | var |
| `ResumeLayout(` | **0** | var |
| `BeginInit()` | **0** | var (DevExpress bileşenlerinde) |
| `Controls.Add(` | **0** | var |
| `new DevExpress.` | **0** | var |
| `InitializeComponent()` | yalnız **çağrı** (1 satır) | yalnız **tanım** (1 gövde) |

### §1.3 Bu kural neden zorunlu

| # | `.cs`'e tasarım kodu yazılırsa | Sonuç |
|---|---|---|
| 1 | Visual Studio tasarımcısı dosyayı yeniden üretir | el ile yazılan kod **silinir** |
| 2 | İki `InitializeComponent` tanımı | derleme hatası (CS0111) |
| 3 | `.resx` anahtarları ile kod çelişir | `MissingManifestResourceException` |
| 4 | Yetki sınırı kalkar | `ui-designer` X alanı ihlali (G3) |
| 5 | Kod-behind 200+ satıra şişer | iş mantığı UI'a gömülür, test edilemez |

### §1.4 Kim neyi değiştirir

| Değişiklik | Yapan | Not |
|---|---|---|
| Kontrol ekleme/boyut/tema | `ui-designer` | `.Designer.cs` + `.resx` birlikte |
| Olay **bağlama** (`+=`) | `ui-designer` | `.Designer.cs` içinde |
| Olay **gövdesi** | `developer` | `.Form.cs` içinde, ince |
| Elle `.resx` XML düzeltmesi | `ui-designer` | `developer` yaparsa RED |
| Yeni metin anahtarı | `ui-designer` | `ResourceManager` adı değişmez |

> **UYARI:** `developer`, `.Designer.cs` ya da `.resx`'i ellemişse bulgu biçimi şudur: `developer` `.resx`'i ellemiş → `.Designer.cs` ≠ `.resx` tutarsız → form açılmıyor → **RED → `ui-designer`'a devret**.

## §2 Uygulama Kontrol Listesi

| # | Adım | Kanıt |
|---|---|---|
| 1 | Her form için 3 dosya var (`.cs` / `.Designer.cs` / `.resx`) | dosya listesi |
| 2 | `.Form.cs` taraması: `SuspendLayout|ResumeLayout|BeginInit|Controls.Add` = 0 | grep çıktısı |
| 3 | `InitializeComponent` tanımı yalnız 1 dosyada | grep = 1 eşleşme |
| 4 | `XxxForm.resx` ilk satırı geçerli `<root>` XML | dosya okundu |
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
