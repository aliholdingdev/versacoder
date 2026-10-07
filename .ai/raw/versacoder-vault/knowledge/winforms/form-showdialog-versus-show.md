---
title: "WinForms — ShowDialog ile Show Arasındaki Fark ve Sahiplik Kuralı"
type: knowledge
category: winforms
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
scope: "Modal ve modalsız açılışın davranış farkı, sahiplik (owner) zinciri, `DialogResult` akışı ve hangi form türünün hangi yöntemle açılacağı"
---

# `ShowDialog` ile `Show` Arasındaki Fark

**Bağlantılar:** [index.md](index.md) · [../../CLAUDE.md](../../CLAUDE.md)

## §0 Özet (Answer First)

`ShowDialog()` **modaldir**: çağrı bloke olur, sahibi devre dışı kalır, dönüş değeri `DialogResult`'tur ve form **çağıranın sahibidir**. `Show()` **modalsızdır**: akış devam eder, form kendi başına yaşar ve **sahibi onu açan formdur** (kapanış zinciri `owner` üzerinden yürür). Ana pencere **her zaman** `Application.Run(mainForm)` ile `Show` düzeyinde açılır; `MainForm` bile `InitializeComponent` dışı tasarım kodu içermez.

| # | Konu | `ShowDialog()` | `Show()` |
|---|---|---|---|
| 1 | Akış | bloke eder | devam eder |
| 2 | Geri dönüş | `DialogResult` (int) | `void` |
| 3 | Sahiplik | `owner` verilirse modala sahip olur | `owner` kapanınca o da kapanır |
| 4 | Arkadaki form | **devre dışı** | etkin kalır |
| 5 | Tekil örnek | her çağrıda yeni nesne | genelde tek örnek (`singleton` mantık) |
| 6 | Tipik kullanım | Onay/Ayar/Seçici | Dock içi araç penceresi, chat paneli |

### §1.1 Modal açılış (kalıp)

```csharp
using var dlg = new SettingsDialogForm(_settingsService);
dlg.ShowDialog(this);                       // "this" = sahip
if (dlg.DialogResult != DialogResult.OK) return;
await ReloadAsync();
```

`DialogResult` ayarı `.Form.cs`'te, buton bağlama `.Designer.cs`'tedir:

```csharp
// SettingsDialogForm.Designer.cs
this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
```

### §1.2 Modalsız açılış ve sahiplik zinciri

```csharp
// MainForm içinde — araç penceresi sahipli açılır
var terminal = new TerminalDockForm(_processHost);
terminal.Owner = this;      // MainForm kapanınca terminal de kapanır
terminal.Show();
```

| # | Hata | Belirti | Düzeltme |
|---|---|---|---|
| 1 | `Owner` verilmeden `Show` | ana form kapanınca hayalet pencere kalır | `Owner = this` |
| 2 | `ShowDialog`'ta `using` yok | her tıklamada yeni pencere açılır | `using` + tek örnek koru |
| 3 | `ShowDialog` içinde `await` sonrası eski değer | form bayatlar | kapanınca yenile |
| 4 | Modalsız formda iş mantığı | taşınmaz test edilemez | Application katmanına taşı |
| 5 | `Show()` ile `DialogResult` beklemek | `void` | `ShowDialog` kullan |

### §1.3 Uygulama penceresi açılışı (iki bacak)

```csharp
// Program.cs
[STAThread]
static void Main()
{
#if NETFRAMEWORK            // yalnız platform/API farkı (G6)
    Application.EnableVisualStyles();
    Application.SetCompatibleTextRenderingDefault(false);
#else
    ApplicationConfiguration.Initialize();
#endif
    var services = BuildCompositionRoot();     // DI — bkz. composition-root
    Application.Run(services.GetRequiredService<MainForm>());
}
```

> **KURAL:** `#if` yalnız `net48` ile `net10.0-windows` arasındaki **API farkını** kapsar; kütüphane seçimi, servis kaydı veya iş mantığı `#if` içine **alınmaz** (G6).

### §1.4 Kim hangi formu açar?

| Ekran | Yöntem | Neden |
|---|---|---|
| `01-start-page` (ana pencere) | `Application.Run` | uygulama yaşam döngüsü sahibi |
| `02-project-new` | `ShowDialog` | karar bloklamalı |
| `07-settings` | `ShowDialog` | ayarlar uygulanana kadar devre dışı |
| `06-terminal` (dock paneli) | `Show()` + `Owner` | paralel kullanılmalı |
| Onay mesajları | `MessageBox.Show(this, …)` | sahipli modal |

## §2 Uygulama Kontrol Listesi

| # | Adım | Kanıt |
|---|---|---|
| 1 | Her modal çağrı `using` + `owner` içeriyor | kod incelemesi |
| 2 | Modalsız formlarda `Owner` atanıyor | kod incelemesi |
| 3 | `DialogResult` ataması `.Designer.cs`'te | grep |
| 4 | `Program.cs`'te `#if` yalnız platform farkı | kod incelemesi |
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
