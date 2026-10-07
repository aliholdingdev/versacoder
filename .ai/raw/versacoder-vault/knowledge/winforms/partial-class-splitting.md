---
title: "WinForms — partial class ile Dosya Ayrımı ve Yeni Form Ekleme"
type: knowledge
category: winforms
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
scope: "Aynı sınıfın üç parçaya nasıl bölündüğünü, her parçanın hangi imzayı içerdiğini ve sıfırdan yeni form eklemenin adım sırasını"
---

# `partial class` ile Dosya Ayrımı

**Bağlantılar:** [index.md](index.md) · [../../CLAUDE.md](../../CLAUDE.md)

## §0 Özet (Answer First)

Her form `public partial class XxxForm : RibbonForm` sözcüğüyle **üç dosyaya** bölünür ve üçünde de **aynı sınıf adı, aynı taban sınıf, aynı namespace** yazılır; imza uyuşmazsa CS0262 (partial'da çelişen modifier) ya da CS0115 (override bulunamadı) çıkar. Yeni form ekleme sırası **sabitdir**: `.cs` → `.Designer.cs` → `.resx`, sonra derle.

| # | Dosya | Sınıf beyanı | İçerdiği |
|---|---|---|---|
| 1 | `XxxForm.cs` | `public partial class XxxForm : RibbonForm` | alanlar, kurucu, olay yönlendiricileri |
| 2 | `XxxForm.Designer.cs` | `partial class XxxForm` | `InitializeComponent`, alan tanımları, `Dispose` |
| 3 | `XxxForm.resx` | — | kaynak verisi |

> **Taban sınıf yalnız bir yerde yazılır** (`.cs` dosyasında); `.Designer.cs`'te `: RibbonForm` **tekrarlanmaz**. Tekrarlanırsa derleyici CS0262 verir.

### §1.1 Yeni form ekleme (adım sırası)

| # | Adım | Dosya | Sorumlu |
|---|---|---|---|
| 1 | Tasarımcıda "Add Windows Form" | 3 dosya birlikte üretilir | `ui-designer` |
| 2 | Taban sınıfı `RibbonForm` yap + ribbon stili | `.cs` + `.Designer.cs` | `ui-designer` |
| 3 | `RibbonControlStyle.Office2010` ata | `.Designer.cs` | `ui-designer` |
| 4 | Kontrol setini yerleştir | `.Designer.cs` + `.resx` | `ui-designer` |
| 5 | DI kaydını ekle (bkz. `constructor-injection-in-forms`) | `Program.cs` / host | `developer` |
| 6 | Olay gövdelerini yaz | `.cs` | `developer` |
| 7 | `dotnet build` iki bacak | — | `developer` |

### §1.2 Sınıf beyanı kalıbı

```csharp
// SettingsForm.cs
namespace VersaCoder.UI.Forms
{
    public partial class SettingsForm : RibbonForm
    {
        private readonly ISettingsService _settings;

        public SettingsForm(ISettingsService settings)
        {
            InitializeComponent();
            _settings = settings;
        }
    }
}
```

```csharp
// SettingsForm.Designer.cs
namespace VersaCoder.UI.Forms
{
    partial class SettingsForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing) { /* ... */ }

        private void InitializeComponent() { /* tek kaynak */ }
    }
}
```

### §1.3 Sık derleme hataları

| # | Hata | Neden | Düzeltme |
|---|---|---|---|
| 1 | CS0262 — partial'larda modifier çelişiyor | birinde `public`, diğerinde yok / taban sınıf iki yerde | taban sınıfı yalnız `.cs`'e yaz |
| 2 | CS0115 — override edilen üye bulunamadı | `Dispose(bool)` `.Form.cs`'e taşındı | geri al, `.Designer.cs`'te kalsın |
| 3 | CS0101 — aynı adla iki tanım | yanlışlıkla 2. dosya ekle | fazlayı sil (onay ile) |
| 4 | `InitializeComponent` bulunamadı | dosya `.csproj`'e dahil değil | `Compile` tanımını kontrol et |
| 5 | Namespace uyuşmazlık | dosya elle taşındı | üç dosyada da aynı namespace |

### §1.4 İki bacak notu

`partial` davranışı her iki bacakta **aynıdır**; `.csproj` dosya kapsama (glob) `net48` ve `net10.0-windows` için SDK varsayılanıyla aynıdır. `#if` **yalnız** platforma özgü API farkı için kullanılır (G6) — dosya kapsamını `#if` ile bölmek **yasaktır**.

## §2 Uygulama Kontrol Listesi

| # | Adım | Kanıt |
|---|---|---|
| 1 | Üç dosya var, hepsinde aynı namespace | dosya + grep |
| 2 | Taban sınıf yalnız `.cs`'te | grep = 1 eşleşme `: RibbonForm` |
| 3 | `InitializeComponent` tanımı yalnız 1 | grep |
| 4 | Yeni form DI kaydı eklendi | host kaydı |
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
