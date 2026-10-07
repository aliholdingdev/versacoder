---
title: "docking-panels-patterns - VersaCoder"
type: knowledge
category: winforms
status: active
version: 1.0.0
updated: 2026-09-30
authority: primary
scope: "Dock sirasini, panel kalibi secimini, panel durum kaydini (user.config) ve DevExpress DockManager ile ayrimi tanimlar"
tags: [knowledge]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# Docking ve Panel Kalibi

**Zorunlu Baglantilar:** [layout-table-panel](layout-table-panel.md) · [splitcontainer-and-sizing](splitcontainer-and-sizing.md) · [settings-user-config](settings-user-config.md) · [devexpress-rules](../../.rules/devexpress-rules.md) · [anchor-dock-layout-rules](anchor-dock-layout-rules.md)
**Küme + Hub:** [knowledge index](../index.md) · [winforms index](index.md) · [index](../../index.md) · [CLAUDE](../../claude.md) · [AGENTS](../../definitions/agents.md) · [brain](../../.decisions/brain.md) · [glossary](../../definitions/glossary.md) · [keys](../../governance/keys.md)
**Kardeşler (winforms · 5/32):** [accessibility-tab-and-mnemonics](accessibility-tab-and-mnemonics.md) · [async-await-ui-thread](async-await-ui-thread.md) · [async-loading-patterns](async-loading-patterns.md) · [async-ui-thread-marshalling](async-ui-thread-marshalling.md) · [async-void-event-handlers](async-void-event-handlers.md) · [backgroundworker-vs-task](backgroundworker-vs-task.md) · [cancellation-in-ui](cancellation-in-ui.md) · [cancellation-token-in-ui](cancellation-token-in-ui.md) · [composition-root-host-startup](composition-root-host-startup.md) · [control-disposal-and-memory](control-disposal-and-memory.md) · [controls-lifecycle-events](controls-lifecycle-events.md) · [data-binding-basics](data-binding-basics.md) · [designer-cs-anatomy](designer-cs-anatomy.md) · [designer-cs-trio-rule](designer-cs-trio-rule.md) · [designer-editing-rules](designer-editing-rules.md) · [designer-hand-edit-policy](designer-hand-edit-policy.md) · [di-container-lifetimes](di-container-lifetimes.md) · [errorprovider-validation-flow](errorprovider-validation-flow.md) · [exception-boundary-ui](exception-boundary-ui.md) · [form-close-and-dispose](form-close-and-dispose.md) · [form-close-cancel-pattern](form-close-cancel-pattern.md) · [form-constructor-vs-load](form-constructor-vs-load.md) · [form-load-and-shown-events](form-load-and-shown-events.md) · [logging-integration](logging-integration.md) · [partial-class-and-file-naming](partial-class-and-file-naming.md) · [validation-form-level](validation-form-level.md) · [winforms-rendering-performance](winforms-rendering-performance.md)

> **Tek cumlelik sorumluluk:** *Yan panel/sekreter/arac penceresi hangi docking kalibiyla yerlestirilir, durumu nasil kaydedilir, `Dock` sirasi nasil bozulmaz* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Docking'te tek kaynak **`Controls.Add` sirasi + `DockStyle`** degeridir: `Fill` en son eklenir, `Left/Right/Top/Bottom` once eklenir; ters sira panelin birbirini yemesine yol acar. `SplitContainer` cozunur yan panel icin yerlesik kalipdir; `FixedPanel` verilmezse yeniden boyutlandirmada icerik ziplar. Panel durumu (gizli/acik, genislik) `user.config`'e yazilir; **gizli panel yer tutucusu** unutulursa kullanici paneli `View ->` menu'sunden geri acamaz. Bu vault'ta docking **DevExpress v26.1 `DockManager`/`XtraDocking`** ile kurulur; yalnizca vurgu/tasma icin `DockStyle` kullanilir.

```text
+--------------------------------------------------------------+
| ilk eklenen : Top / Bottom / Left / Right  (seritler)        |
| en son      : Fill                      (ana icerik)        |
| durum kaydi : user.config (gizli/acik + genislik)            |
| yer tutucu  : View menusunde KALIR, panel geri acilabilir    |
+--------------------------------------------------------------+
```

---

## §1 Dock sirasi (kural tablosu)

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
| `TabControl` | ayni bolgede cok belge | tek pencere tercihi |
| DevExpress `DockManager` (v26.1) | kayitli panel + auto-hide + koleksiyon | **bu vault'un varsayilani** (ADR-004) |
| MDI child | **onermiyoruz** | tek pencere tercihi |

---

## §3 Iki bacak farki (net48 vs net10.0-windows)

| Konu | `net48` | `net10.0-windows` |
|---|---|---|
| `DockStyle` / `SplitContainer` | var | var - **ayni LayoutEngine** |
| panel durumu kaydi | `ConfigurationManager` (framework icinde) | `System.Configuration.ConfigurationManager` **paket** gerekir |
| `user.config` yolu | `%LOCALAPPDATA%\<sirket>\<uyg>` | ayni sablon, surum klasoru |
| DevExpress docking | v26.1 (her iki bacak) | v26.1 (ADR-004) |
| DPI | panel genisligi sabit piksel -> DPI'da daralir | `ApplicationHighDpiMode` + `AutoScaleMode.Dpi` |

> **Not:** `ConfigurationManager` farki `#if` degil `.csproj` farkidir - iki bacakta da ayni `<PackageReference>` (CPM tek noktada) kullanilir (bkz. [settings-user-config](settings-user-config.md)).

---

## §4 C# 7.3 (net48) vs C# 14

| Ozellik | C# 7.3 | C# 14 | Ortak kaynak |
|---|---|---|---|
| kosullu atama | `p.Width = ... ? ... : ...` | ayni | **fark yok** |
| nullable | `Panel p = sidePanel` | dogrudan alan | 7.3'te de derlenir |
| `if` kortu | `if (x) { ... }` | tek satir | ortak kodda blok |
| Settings erisimi | `Properties.Settings.Default` | ayni | proje uretimi iki bacakta ayni |

Tipik toggle: `sidePanel.Visible = !sidePanel.Visible` + `Width` atamasi + `Properties.Settings.Default.SideWidth` kaydi -> `Save()`.

---

## §5 Kim neyi degistirir + ihlal tablosu

| Ihlal | Belirti | Duzeltme | Sahip |
|---|---|---|---|
| `Fill` once eklendi | yan panel ekranda gorunmuyor | Add sirasini §1'e cevir | `ui-designer` |
| `FixedPanel.None` | ayirac ziplama, icerik sigmiyor | `FixedPanel.Panel2` | `ui-designer` |
| gizli panel yer tutucusu yok | panel geri acilamiyor | `View` menu + kayitli durum | `developer` (kod) + `ui-designer` (menu) |
| panel genisligi sabit piksel | 150% DPI'da icerik daralir | `AutoScaleMode.Dpi` + yuzde | `ui-designer` |
| durum kaydi string-concat path | `DA-03` ihlali | `IAppPaths` + parametreli ayar yazimi | `developer` |

Son kontrol: iki bacakta `dotnet build` -> 0 hata DOĞRULAMA GEREKLI; ayirac sola/saga suruklenerek gorsel dogrulama yapilir.

---

## §9 Kanit

| Kaynak dosya | Ne alindi |
|---|---|
| `knowledge\winforms\docking-panels-patterns.md` (kaynak vault) | dock sirasi tablosu, BuildDock kodu, kalip secim tablosu, TFM + C# karsilastirmasi, ihlal belirtileri |
| `.rules\devexpress-rules.md` | v26.1 DockManager tercihi |
| `.rules\data-access-rules.md` | path/SQL yazim kurali (DA-03) |

> Kanitsiz iddia icin DOĞRULAMA GEREKLI yazilir; bu dosyada kaynak disi bilgi yoktur.
