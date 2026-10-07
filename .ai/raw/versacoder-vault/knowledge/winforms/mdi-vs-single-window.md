---
title: "MDI mi Tek Pencere mi - VersaCoder"
type: knowledge
category: winforms
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "MDI (IsMdiContainer/MdiParent) ile tek-pencere+sekme mimarisi karsilastirmasi, sahiplik ve kapatma kurallari, net48 vs net10.0-windows farklari"
---

# MDI mi, Tek Pencere mi?

**Zorunlu Baglantilar:** [knowledge index](../index.md) · [CLAUDE.md](../../CLAUDE.md) · [docking-panels-patterns](docking-panels-patterns.md) · [form-lifecycle](form-lifecycle.md) · [devexpress-rules](../../.rules/devexpress-rules.md)

> **Tek cumlelik sorumluluk:** *Uygulama birden fazla belgeyi MDI cocuk pencereyle mi, tek pencere + sekme/dock paneliyle mi gosterir, kapatma sahibi kimdir* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

**MDI (`IsMdiContainer` + `MdiParent`) bugun yeni uygulama icin onerilmez**: Windows 10/11'te MDI alt pencereleri modern pencere yonetimine (snap, taskbar, DPI) zayif uyar; `MdiParent` set edilmemis form sessizce kendi basina acilir. Bu vault'un tercihi **tek ana pencere (RibbonForm) + sekme/dock paneli** (bkz. [docking-panels-patterns](docking-panels-patterns.md)) - tek `Application.Run`, tek message loop, tek kapatma karari. MDI gerekirse: `IsMdiContainer = true`, `child.MdiParent = this`, kapatma `ActiveMdiChild` uzerinden ve **cocuklar parent'tan once** kapatilir.

---

## §1 Karsilastirma

| Konu | MDI (cocuk pencere) | Tek pencere + sekme/dock |
|---|---|---|
| Pencere sayisi | 1 parent + N child (taskbar'da kaybolur) | 1 pencere |
| Kapatma | parent kapaninca tum cocuklar | sekme/panel kapatilir, pencere kalir |
| DPI / snap | MDI child'lar snap edilmez | standart pencere davranisi |
| Kaynak | her child icin handle + menu merge | tek handle, panel basina control |
| Test edilebilirlik | `MdiParent` bagimlilik zorunlu | her panel bagimsiz `UserControl` |
| Tercih | miras kod | **bu vault** (G6: tek bacak, tek kod yolu) |

---

## §2 MDI zorunlu kalirsa (kalip)

```csharp
// Ana pencere
this.IsMdiContainer = true;

// Cocuk acma
var child = new DocumentForm();
child.MdiParent = this;      // MdiParent OLMADAN acilma!
child.WindowState = FormWindowState.Maximized;
child.Show();

// Kapatma sirasi: cocuk once, parent sonra
foreach (Form f in MdiChildren)
{
    f.Close();
}
this.Close();
```

| Kural | Neden |
|---|---|
| `MdiParent` **set edilmeden** `Show()` | form yalniz basina acilir, MDI davranisi yok |
| `LayoutMdi(MdiLayout.Cascade/TileHorizontal)` | MDI alt pencerelerinin tek gosterim yolu |
| Menu/toolbar `MergeRequired` | MDI child menu'su parent'a birlesir (Ribbon'da `RibbonForm` icin `MdiMergeStyle`) |
| `IsMdiContainer` form `Dispose` | parent dispose edilince child'lar da edilir |

---

## §3 TFM farki (net48 vs net10.0-windows)

| Konu | `net48` | `net10.0-windows` |
|---|---|---|
| `IsMdiContainer` / `MdiParent` | var | var (**API ayni, davranis ayni**) |
| Menu merge (`MainMenuStrip`) | MDI menu merge klasik | ayni |
| Taskbar davranisi | MDI child taskbar'da gorunmez | ayni (Win32 davranisi) |
| DPI | MDI child **parent'in DPI'sini paylasir** (PerMonitorV2'de ayri) | ayni; `ApplicationHighDpiMode` uygulama geneli |
| Test | `MdiParent` bagimli form testleri EDT ister | ayni |

> **Sonuc:** MDI API'si iki bacakta da **birebir aynidir**; karar teknoloji degil **tasarim** kararidir. Tercih edilen yol tek pencere + dock/sekme -> `#if` **gerekmez**.

---

## §4 C# 7.3 (net48) vs C# 14

```csharp
// --- C# 7.3 (net48) ---
private Form _active;

public void OpenDocument(string path)
{
    DocumentForm child = new DocumentForm();
    child.MdiParent = this;
    child.Show();
    _active = child;
}

public void CloseActive()
{
    Form f = this.ActiveMdiChild;
    if (f != null)
    {
        f.Close();
    }
}
```

```csharp
// --- C# 14 (net10.0-windows) ---
private Form? _active;

public void OpenDocument(string path)
{
    DocumentForm child = new() { MdiParent = this };
    child.Show();
    _active = child;
}

public void CloseActive() => ActiveMdiChild?.Close();
```

| Ozellik | C# 7.3 | C# 14 | Ortak kaynak |
|---|---|---|---|
| `MdiParent` atama | ayri satir | object initializer | initializer 7.3'te de var |
| Null guvenli cagri | `if (f != null)` | `?.` | `?.` 6.0+ -> **her iki bacakta da var** |
| Expression-bodied metot | `=>` (6.0+) | ayni | imza degisimi ortak kodu kirabilir -> ifade govdesi serbest |
| Nullable alan | `Form _active` | `Form? _active` | 7.3'te `Form _active` derlenir |

---

## §5 Karar ve ihlal

| Durum | Karar | Gerekce |
|---|---|---|
| Yeni ekran (7 ekran listesi) | **tek pencere + dock/sekme** | G6 tek kod yolu, DPI/test kolayligi |
| Klasik "belge pencereleri" | `TabbedMdiManager` benzeri sekle cevir | MDI dezavantajlari |
| MDI zorunlu | §2 kalibi + test senaryosu | API iki bacakta ayni |
| `MdiParent` olmadan `Show()` | **IHLAL** | MDI davranisi kaybolur |
| Parent kapanmadan child disposal | bellek/handle sizintisi | §2 sirasi zorunlu |

> **Iliskili:** panel/dock tasarimi icin [docking-panels-patterns](docking-panels-patterns.md) · kapatma sirasi icin [form-lifecycle](form-lifecycle.md) · kontrol seti icin [devexpress-rules](../../.rules/devexpress-rules.md).
