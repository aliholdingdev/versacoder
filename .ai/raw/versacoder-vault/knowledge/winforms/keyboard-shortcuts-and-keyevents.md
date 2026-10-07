---
title: "WinForms — Klavye Kısayolları, KeyPreview ve Ribbon Shortcut"
type: knowledge
category: winforms
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
scope: "Genel kısayolların (`Ctrl+S`, `F5`) nerede bağlandığını, `KeyPreview` avantajını, çakışma çözümünü ve DevExpress ribbon kısayollarının kod-behind'a taşınmamasını"
---

# Klavye Kısayolları, `KeyPreview` ve Ribbon Shortcut

**Bağlantılar:** [index.md](index.md) · [../../CLAUDE.md](../../CLAUDE.md)

## §0 Özet (Answer First)

Genel kısayollar (`Ctrl+S`, `Ctrl+N`, `F5`) **tek noktadan** bağlanır: formun `KeyPreview = true` + `KeyDown` olayı ya da ribbon `ItemShortcut`. Kod-behind'da **her tuş için ayrı** `if` bloğu yığmak yerine tek bir yönlendirici kullanılır; bağlama `.Designer.cs`'te yapılır, **gövde** `.Form.cs`'tedir.

| # | Mekanizma | Ne için | Dosya |
|---|---|---|---|
| 1 | `KeyPreview = true` + `KeyDown` | form geneli kısayol tek yerde | bağ `.Designer.cs`, gövde `.cs` |
| 2 | `ItemShortcut` (DevExpress) | ribbon butonuna kısayol | `.Designer.cs` |
| 3 | `ProcessCmdKey` | `Enter`/`Esc`/ok tuşu gibi sistem tuşları | `.cs` (override) |
| 4 | `ToolStripMenuItem.ShortcutKeys` | menü öğesi | `.Designer.cs` |
| 5 | Buton `Text = "&Kaydet"` | `Alt+K` mnemonic | `.resx` |

### §1.1 Tek yönlendirici kalıbı

```csharp
// XxxForm.cs
public MainForm()
{
    InitializeComponent();
    KeyPreview = true;                 // form, kontrol ÖNCE görür
}

protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
{
    switch (keyData)
    {
        case Keys.Control | Keys.S:
            btnKaydet.PerformClick();  // tek satir yonlendirme
            return true;
        case Keys.Escape:
            Close();
            return true;
    }
    return base.ProcessCmdKey(ref msg, keyData);
}
```

### §1.2 Çakışma tablosu

| # | Çakışma | Belirti | Çözüm |
|---|---|---|---|
| 1 | Grid içi `Ctrl+S` ile form `Ctrl+S` | grid önce yakalar | grid `ProcessCmdKey`'i devre dışı ya da `KeyPreview` |
| 2 | Aynı kısayol 2 ribbon öğesinde | ikisi de tetiklenir | kısayolları dağıt, `.Designer.cs`'te denetle |
| 3 | `Enter` onaylarken beklenmedik satır atlama | grid davranışı | `ProcessCmdKey` ile yakala |
| 4 | Türkçe klavyede `Alt+K` çalışmıyor | mnemonic çakışması | `&` konumunu değiştir |
| 5 | Kısayol `.cs`'e `KeyDown` içinde `if` yığını | 100+ satır | tek `switch` + tek kaynak |

```powershell
# Kisayol carpanlari — ayni Keys.Deger iki kez atanmis mi?
Select-String -Path "src\**\*.Designer.cs" -Pattern "ShortcutKeys = " |
  ForEach-Object { $_.Line.Trim() } | Group-Object | Where-Object { $_.Count -gt 1 }
```

### §1.3 Ribbon kısayolu (`.Designer.cs`)

```csharp
this.btnNew.ItemShortcut = new DevExpress.XtraBars.BarShortcut(
    ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N))));
this.btnNew.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnNew_ItemClick);
```

> **KURAL:** Kısayol **tanımı** ve `ItemClick` **bağlantısı** `.Designer.cs`'tedir; tıklanınca ne olacağı `.Form.cs`'teki **ince** olay gövdesindedir. Bu ayrım form üçlüsü kuralının parçasıdır.

### §1.4 Erişilebilirlik ile ilişki

Kısayollar, `accessibility-tab-and-mnemonics` dosyasındaki `Alt` mnemonikleriyle **çakışmamalıdır**; `Alt+harf` erişilebilirlik, `Ctrl+harf` güç kullanıcısı içindir. İki ayrı katmandır ve biri diğerinin yerine geçmez.

## §2 Uygulama Kontrol Listesi

| # | Adım | Kanıt |
|---|---|---|
| 1 | Form geneli kısayol tek yönlendiriciden geçiyor | kod incelemesi |
| 2 | Kısayol tanımları `.Designer.cs`'te, çakışma yok | §1.2 komutu |
| 3 | `KeyPreview = true` ilgili formlarda | grep |
| 4 | `Ctrl+S` / `Esc` / `F5` elle test edildi | manuel test |
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
| 4 | `.Form.cs`'e kısayol tanımı / kontrol üretimi / `SuspendLayout` / `ResumeLayout` / `BeginInit` yazmak | form üçlüsü RED |
| 5 | Kanıtsız "bitti" (G5) · bozuk iç link (G4) | RED |
