---
title: "Docking Panel Kalip - VersaCoder"
type: knowledge
category: winforms
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "WinForms docking: DockStyle sirasi, SplitContainer, panel durumu kaydi, DevExpress v26.1 Docking ile karsilastirma; net48 vs net10.0-windows farklari"
---

# Docking ve Panel Kalip

**Zorunlu Baglantilar:** [knowledge index](../index.md) · [CLAUDE.md](../../CLAUDE.md) · [layout-table-panel](layout-table-panel.md) · [mdi-vs-single-window](mdi-vs-single-window.md) · [devexpress-rules](../../.rules/devexpress-rules.md)

> **Tek cumlelik sorumluluk:** *Yan panel/sekreter/arac penceresi hangi docking kalibiyla yerlestirilir, durumu nasil kaydedilir, `Dock` sirasi nasil bozulmaz* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Docking'te tek kaynak **`Controls.Add` sirasi + `DockStyle`** degeridir: `Fill` en son eklenir, `Left/Right/Top/Bottom` once eklenir; ters sira panelin birbirini yemesine yol acar. `SplitContainer` cozunur yan panel icin yerlesik kalip; `FixedPanel = Panel2` verilmezse yeniden boyutlandirmada icerik ziplar. Panel durumu (gizli/acik, genislik) `user.config`'e yazilir; **gizli panel yer tutucu** unutulursa kullanici paneli `View ->` menu'sunden geri acamaz. Bu vault'ta docking **DevExpress v26.1 `DockManager`/`XtraDocking`** ile kurulur (ADR-004), yalnizca vurgu/tasma icin `DockStyle` kullanilir.

---

## §1 `Dock` sirasi (kural tablosu)

| # | Eklenis sirasi | `DockStyle` | Sonuc |
|---|---|---|---|
| 1 | ilk | `Top` (ust baslik/toolbar) | en ust serit |
| 2 | 2. | `Bottom` (durum cubugu) | en alt serit |
| 3 | 3. | `Left` / `Right` (yan panel) | dikey seritler |
| 4 | **son** | `Fill` (ana icerik) | kalan alani doldurur |

```csharp
private void BuildDock(Form host)
{
    var status = new StatusStrip(); status.Dock = DockStyle.Bottom;
    var side   = new Panel();       side.Dock   = DockStyle.Left; side.Width = 260;
    var body   = new Panel();       body.Dock   = DockStyle.Fill;

    host.Controls.Add(status);   // 1
    host.Controls.Add(side);     // 2
    host.Controls.Add(body);     // 3 - FILL EN SON
}
```

> **KURAL:** `Dock` sirasi ile `Anchor` ayni hucrede kullanilmaz; `Fill` + `AutoSize` birlikte olcum dongusu uretir (bkz. [layout-table-panel](layout-table-panel.md)).

---

## §2 Kalip secimi

| Kalip | Ne zaman | Dayanak |
|---|---|---|
| `SplitContainer` | 2 panel + suruklenebilir ayirac (klasik IDE) | `FixedPanel`, `SplitterDistance` |
| `DockPanel` + `DockStyle.Left/Right` | sabit yan panel | durum kaydi `user.config` |
| `TabControl` | ayni bolgede cok belge | `mdi-vs-single-window.md` -> tek pencere tercihi |
| DevExpress `DockManager` (v26.1) | kayitli panel + auto-hide + koleksiyon | **bu vault'un varsayilani** (ADR-004) |
| MDI child | **onermiyoruz** | `mdi-vs-single-window.md` §1 |

---

## §3 TFM farki (net48 vs net10.0-windows)

| Konu | `net48` | `net10.0-windows` |
|---|---|---|
| `DockStyle` / `SplitContainer` | var | var - **ayni LayoutEngine** |
| Panel durumu kaydi | `Application.UserConfigPath` + `ConfigurationManager` (yerlesik) | `System.Configuration.ConfigurationManager` **paket** gerekir |
| `user.config` yolu | `%LOCALAPPDATA%\<sirket>\<uyg>` | ayni sablon, surum klasoru |
| DevExpress docking | v26.1 (her iki bacak) | v26.1 (ADR-004) |
| DPI | panel genisligi sabit piksel -> DPI'da daralir | `ApplicationHighDpiMode` + `AutoScaleMode.Dpi` |

> **Not:** `ConfigurationManager` **`net48`'de framework icinde, `net10.0-windows`'ta ayri NuGet paketidir** - iki bacakta da ayni `<PackageReference>` (CPM tek noktada) kullanilir; `#if` **gerekmez**, yalnizca `.csproj` farkidir.

---

## §4 C# 7.3 (net48) vs C# 14

```csharp
// --- C# 7.3 (net48) ---
private bool sideVisible;

public void ToggleSidePanel()
{
    sideVisible = !sideVisible;
    sidePanel.Visible = sideVisible;

    Panel p = sidePanel;
    p.Width = sideVisible ? 260 : 0;

    Properties.Settings.Default.SideWidth = p.Width;
    Properties.Settings.Default.Save();
}
```

```csharp
// --- C# 14 (net10.0-windows) ---
private bool _sideVisible;

public void ToggleSidePanel()
{
    _sideVisible = !_sideVisible;
    sidePanel.Visible = _sideVisible;
    sidePanel.Width = _sideVisible ? 260 : 0;

    Properties.Settings.Default.SideWidth = sidePanel.Width;
    Properties.Settings.Default.Save();
}
```

| Ozellik | C# 7.3 | C# 14 | Ortak kaynak |
|---|---|---|---|
| Kosullu atama | `p.Width = ... ? ... : ...` | ayni | **fark yok** |
| Nullable | `Panel p = sidePanel` | dogrudan alan | 7.3'te de derlenir |
| `if` kortu | `if (x) { ... }` | `if (x) ...` (tek satir) | ortak kodda blok |
| Settings erisimi | `Properties.Settings.Default` | ayni | proje uretimi iki bacakta ayni |

---

## §5 Ihlal belirtisi

| Ihlal | Belirti | Duzeltme |
|---|---|---|
| `Fill` once eklendi | yan panel ekranda gorunmuyor | Add sirasini §1'e cevir |
| `FixedPanel.None` | ayirac ziplama, icerik sigmiyor | `FixedPanel.Panel2` |
| Gizli panel yer tutucusu yok | panel geri acilamiyor | `View` menu + kayitli durum |
| Panel genisligi sabit piksel | 150% DPI'da icerik daralir | `AutoScaleMode.Dpi` + yuzde |
| Durum kaydi string-concat path | `DA-03` ihlali | `IAppPaths` + parameterized ayar yazimi |

> **Iliskili:** olcum kurallari icin [layout-table-panel](layout-table-panel.md) · ekran mimarisi tercihi icin [mdi-vs-single-window](mdi-vs-single-window.md) · DPI icin [autoscalemode-dpi-scaling](autoscalemode-dpi-scaling.md).
