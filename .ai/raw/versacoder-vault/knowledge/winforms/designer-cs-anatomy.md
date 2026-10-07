---
title: "designer-cs-anatomy - VersaCoder"
type: knowledge
category: winforms
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "XxxForm.Designer.cs dosyasının anatomisi: tek InitializeComponent kaynağı, SuspendLayout/ResumeLayout/BeginInit bloklarının yeri, ResourceManager bağlantısı"
---

# designer-cs-anatomy — `.Designer.cs` Anatomisi

**Zorunlu Baglantilar:** [vault index](../../index.md) · [coding-standards](../../.rules/coding-standards.md) · [winforms index](../index.md) · [code-behind-zero-designer-code](code-behind-zero-designer-code.md) · [resx-file-structure](resx-file-structure.md)

> **Tek cümlelik sorumluluk:** *Bir `.Designer.cs` dosyasının hangi blokları içerdiği, neden tek `InitializeComponent` kaynağı sayıldığı ve `.cs` ile arasındaki sınırın tam olarak nerede olduğu* sorusunun tek cevabıdır.

---

## §0 Ozet (Answer First)

`.Designer.cs`, formun **tek ve tek** tasarım kaynağıdır: tüm üye tanımları, kontrol yaratımı, `SuspendLayout`/`ResumeLayout`, `BeginInit`/`EndInit`, `ComponentResourceManager` ve kaynak atamaları **yalnız burada** bulunur. Bir kod tabanında aynı form için **ikinci bir `InitializeComponent`** varsa derleme `CS0102` ile patlar; `InitializeComponent` gövdesi `.cs`'e **taşınırsa** dosya her tasarımcı açılışında geri alınır ve çakışma doğar. Bu nedenle proje kuralı nettir: **kontrol oluşturma / `SuspendLayout` / `ResumeLayout` / `BeginInit` kodu hiçbir `.cs` dosyasına (main form dahil) yazılmaz.**

---

## §1 Uc dosya, tek sorumluluk

```
SessionListForm.cs            ->  kullanici mantigi (ctor, olay, dogrulama, servis cagrilari)
SessionListForm.Designer.cs   ->  TEK InitializeComponent + uyeler + kontrol yaratimi
SessionListForm.resx          ->  ResourceManager anahtarlari ($this.*, Label1.Text, gorseller)
```

| Dosya | İçerdiği | İçermesi yasak olan |
|---|---|---|
| `XxxForm.cs` | İş mantığı, olay tutamaçları, servis alanları | Kontrol yaratımı, `SuspendLayout`, `ResumeLayout`, `BeginInit`, konum/boyut ataması |
| `XxxForm.Designer.cs` | `InitializeComponent()`, alan/uyeler, `components = new Container()` | Kullanıcı iş mantığı, veritabanı sorgusu, mesaj kutusu |
| `XxxForm.resx` | Metin/görsel/ikon anahtarları, `$this.AutoScaleDimensions` | Elle düzenlenmiş bozuk base64, yinelenen `data name` |

---

## §2 Designer dosyasinin bolumleri

| Bölüm | Tipik içerik | Elle mi? |
|---|---|---|
| `partial class ... : RibbonForm` | `System.ComponentModel.IContainer components = null;` | Hayır (sadece `partial` satırı iki tarafta) |
| `InitializeComponent()` başı | `ComponentResourceManager` + `SuspendLayout()` | Tasarımcı |
| Üye/kontrol yaratımı | `new GridControl()`, `Dock`, `Anchor`, `Name` | Tasarımcı |
| `BeginInit/EndInit`, panel `SuspendLayout/ResumeLayout` | Kapsayıcı eşlemeleri | Tasarımcı |
| `ResumeLayout(false)` + `PerformLayout()` | Kaynak atamaları (`resources.ApplyResources`) | Tasarımcı |
| Kaynak atamaları | `this.lblBaslik.Text = resources.GetString("lblBaslik.Text")` | Tasarımcı |
| `Dispose(bool)` | `components?.Dispose()` (designer container) | Tasarımcı |

```csharp
// XXXForm.Designer.cs — parca (elle DEGISTIRILMEZ; tasarimcinin urettigi hali)
private void InitializeComponent()
{
    this.components = new System.ComponentModel.Container();
    System.ComponentModel.ComponentResourceManager resources =
        new System.ComponentModel.ComponentResourceManager(typeof(SessionListForm));  // -> SessionListForm.resx
    this.SuspendLayout();
    // ... kontrol yaratimi: Name / Location / Size / Anchor / Text
    this.ResumeLayout(false);
}
```

---

## §3 sinir: .cs'te ne kalir

| İş | `.cs` (developer) | `.Designer.cs` (ui-designer) |
|---|---|---|
| `InitializeComponent()` **çağrısı** (tek satır, ctor içinde) | Evet | Gövde burada |
| Servis enjeksiyonu (`_svc = x`) | Evet | Hayır |
| `btnKaydet.Click += btnKaydet_Click;` | **Tercihen `.cs`** (bağlama = kullanıcı kararı) | Tasarımcı da bağlayabilir — **tek yer** olmalı |
| `this.Text = "..."` sabit başlık | Hayır (resx/`$this.Text`) | Evet |
| `gridControl1.DataSource = rows` (veri) | Evet (iş verisi) | Hayır |
| `SuspendLayout()` / `BeginInit()` | **YASAK** | Tek kaynak |

> **KURAL:** Aynı olay iki yerde bağlanırsa çift tetikleme olur; bağlama noktası **tek** tutulur ve kod incelemesinde aranır.

---

## §4 net48 / net10.0-windows farki

| Konu | `net48` | `net10.0-windows` |
|---|---|---|
| Designer dosyasının ürettiği dil | C# 7.3 uyumlu (tür belirteçli `new`) | Aynı dosya her iki bacakta aynı |
| `ComponentResourceManager(typeof(X))` | **aynı** | aynı |
| Tasarımcının derlediği hedef | Visual Studio tasarım zamanı (Framework) | .NET designer host — dosya **değişmez** |
| `#if` | **YOK** (tasarım dosyası platform içermemeli) | YOK |

> **NOT:** Tasarım dosyasına `#if` koymak tasarım zamanında derleme hatası verir; iki bacak farkı yalnız `.cs`'te ve platform API'sinde aranır (ADR-012 bağlamı).

---

## §5 Kardesler

[designer-editing-rules](designer-editing-rules.md) · [code-behind-zero-designer-code](code-behind-zero-designer-code.md) · [resx-file-structure](resx-file-structure.md) · [form-triple-consistency-check](form-triple-consistency-check.md)
