---
title: "layout-table-panel - VersaCoder"
type: knowledge
category: winforms
status: active
version: 1.0.0
updated: 2026-09-30
authority: SSOT
scope: "TableLayoutPanel ve FlowLayoutPanel kullanimini, Dock-Anchor sirasini, AutoSize tuzaklarini ve DPI altindaki davranisi tanimlar"
---

# Layout: TableLayoutPanel ve Dock/Anchor Kalibi

**Zorunlu Baglantilar:** [anchor-dock-layout-rules](anchor-dock-layout-rules.md) · [splitcontainer-and-sizing](splitcontainer-and-sizing.md) · [docking-panels-patterns](docking-panels-patterns.md) · [coding-standards](../../.rules/coding-standards.md) · [ui-rules](../../.rules/ui-rules.md)
**Küme + Hub:** [knowledge index](../index.md) · [winforms index](index.md) · [index](../../index.md) · [CLAUDE](../../CLAUDE.md) · [AGENTS](../../AGENTS.md) · [brain](../../brain.md) · [glossary](../../glossary.md) · [keys](../../keys.md)
**Kardeşler (winforms · 4/32):** [accessibility-tab-and-mnemonics](accessibility-tab-and-mnemonics.md) · [async-await-ui-thread](async-await-ui-thread.md) · [async-loading-patterns](async-loading-patterns.md) · [async-ui-thread-marshalling](async-ui-thread-marshalling.md) · [async-void-event-handlers](async-void-event-handlers.md) · [backgroundworker-vs-task](backgroundworker-vs-task.md) · [cancellation-in-ui](cancellation-in-ui.md) · [cancellation-token-in-ui](cancellation-token-in-ui.md) · [composition-root-host-startup](composition-root-host-startup.md) · [control-disposal-and-memory](control-disposal-and-memory.md) · [controls-lifecycle-events](controls-lifecycle-events.md) · [data-binding-basics](data-binding-basics.md) · [designer-cs-anatomy](designer-cs-anatomy.md) · [designer-cs-trio-rule](designer-cs-trio-rule.md) · [designer-editing-rules](designer-editing-rules.md) · [designer-hand-edit-policy](designer-hand-edit-policy.md) · [di-container-lifetimes](di-container-lifetimes.md) · [errorprovider-validation-flow](errorprovider-validation-flow.md) · [exception-boundary-ui](exception-boundary-ui.md) · [form-close-and-dispose](form-close-and-dispose.md) · [form-close-cancel-pattern](form-close-cancel-pattern.md) · [form-constructor-vs-load](form-constructor-vs-load.md) · [form-load-and-shown-events](form-load-and-shown-events.md) · [logging-integration](logging-integration.md) · [partial-class-and-file-naming](partial-class-and-file-naming.md) · [settings-user-config](settings-user-config.md) · [validation-form-level](validation-form-level.md) · [winforms-rendering-performance](winforms-rendering-performance.md)

> **Tek cumlelik sorumluluk:** *Pencere boyutunca sikisan kontroller hangi layout motoru ile duzelir, `TableLayoutPanel` ne zaman, `Dock`/`Anchor` ne zaman kullanilir* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

WinForms'ta tek dogru duzen motoru **parent'in child'lari konumlandirma hiyerarsisidir**: `TableLayoutPanel` (satir/sutun, yuzde + mutlak), `FlowLayoutPanel` (etiket+girdi sirasi), `Dock` (kenara yapisan panel), `Anchor` (iki kose arasina gerilen control). **Once `Dock`, sonra `Anchor`, en son elle `Location`** - `Location` elle ayarlaniyorsa layout tasarimi kiriktir. `AutoSize = true` + `DockStyle.Fill` birlikte `PerformLayout` dongusu uretebilir; tablo hucresinde `AutoSize` yalnizca bir eksende birakilir. DPI artisinda boyutlar pixel uzerinden degil, `AutoScaleMode.Dpi`/`Font` ile olculur.

```text
+----------------------------------------------------------+
| SIRA: Dock -> Anchor -> AutoSize -> en son Location      |
| TLP  : Percent + Absolute + AutoSize (ayni hucrede DKKAT) |
| DPI  : sabit piksel yazma, yuzde / AutoSize kullan        |
+----------------------------------------------------------+
```

---

## §1 Hangi layout ne icin

| Kalip | Ne yapar | Tipik yer | Risk |
|---|---|---|---|
| `TableLayoutPanel` | satir/sutun; `Percent` + `Absolute` + `AutoSize` | form govdesi, ayar sayfalari | `Percent` + `AutoSize` ayni hucrede -> olcum dongusu |
| `FlowLayoutPanel` | sigmayan satirda yenisine gecer | etiket + girdi listesi, toolbar | `FlowDirection` tersse okuma sirasi bozulur |
| `Dock` | kenara yapistirir (`Top/Bottom/Left/Right/Fill`) | ust baslik, alt durum, yan panel | `Dock` sirasi **z-index** gibidir, `Controls.Add` sirasi onemli |
| `Anchor` | iki kose arasina gerer | tekil kontrol, buton grubu | 4 kose de anchor -> `Dock`'a benzeyen davranis |
| `SplitContainer` | iki panel + suruklenebilir ayirac | ana pencere (cozunur yan panel) | `FixedPanel` verilmezse yeniden boyutlandirmada ziplama |

**Sira kurali (tek satir):** `Dock` -> `Anchor` -> `AutoSize` -> en son `Location` (yalniz layout disi serbest kalip).

---

## §2 TableLayoutPanel ornegi ve ayarlari

```csharp
private void BuildLayout(TableLayoutPanel grid)
{
    grid.ColumnCount = 2;
    grid.RowCount = 3;
    grid.Dock = DockStyle.Fill;
    grid.AutoSize = true;

    grid.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 140F));
    grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
    grid.RowStyles.Add(new RowStyle(SizeType.AutoSize));
    grid.RowStyles.Add(new RowStyle(SizeType.AutoSize));
    grid.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

    grid.Controls.Add(nameLabel, 0, 0);
    grid.Controls.Add(nameBox,   1, 0);
    grid.Controls.Add(infoBox,   0, 2);
    grid.SetColumnSpan(infoBox, 2);
}
```

| Ayar | Anlam | Ne zaman |
|---|---|---|
| `SizeType.Absolute` | sabit piksel | etiket kolonu (140F gibi) |
| `SizeType.Percent` | kalan bosluga paylasir | doluluk sutunu |
| `SizeType.AutoSize` | icerik kadar | satir yuksekligi |
| `SetColumnSpan` | hucreyi genislet | mesaj alani, grid |

> **KURAL:** `Percent` degerleri toplami 100'den buyukse hedef payin disina tasar; `Absolute` piksel degeri **DPI'da buyur** - high-DPI acikken `AutoScaleMode.Dpi` ile tasarla, sabit piksel yazma.

---

## §3 Iki bacak farki (net48 vs net10.0-windows)

| Konu | `net48` | `net10.0-windows` |
|---|---|---|
| layout motoru (`LayoutEngine`) | ayni (`DefaultLayout`) | ayni - **davranis farki yok** |
| `TableLayoutPanel`/`FlowLayoutPanel` | var | var |
| DPI varsayilani | manifest / app.config `DpiAwareness` | MSBuild `<ApplicationHighDpiMode>` |
| `AutoScaleMode` | `Dpi`/`Font` (tasarimci `Dpi` yazar) | ayni; `ApplicationConfiguration.Initialize()` onceden kurar |
| tasarim pikseli | 96 DPI reference | ayni reference |

> **Sonuc:** layout kodu **iki bacakta da aynidir**; tek fark formun **hangi DPI modunda** acildigidir. `#if` gerekmez.

---

## §4 C# 7.3 (net48) vs C# 14

| Ozellik | C# 7.3 | C# 14 | Ortak kaynak |
|---|---|---|---|
| koleksiyon baslatma | `new List<Control>()` | `[]` | 7.3'te `new` zorunlu |
| nesne baslatma | `new Label()` + satir satir | `new() { ... }` | object initializer 7.3'te de var |
| tip yazma | her satirda | `var` / hedef tip | ortak dosyada **tip acik** yaz (CS-01) |
| `switch` ifadesi | statement | expression | ortak kodda statement (8.0+) |

---

## §5 Kim neyi degistirir + tarama tablosu

| Kim | Neyi degistirir | Dosya | Tarama |
|---|---|---|---|
| `ui-designer` | satir/sutun stili, `Dock`/`Anchor` degerleri | `.Designer.cs` | degerler tasarimcida |
| `developer` | icerik kontrolu ekleme, kod icinde kurulan tablo | `.Form.cs` | `Controls.Add` sira kontrolu |
| herkes | elle `Location` atamasi | yasak | `.Form.cs`'te `Location =` **0** DOĞRULAMA GEREKLI |
| herkes | elle `PerformLayout()` | yasak | dongu uretir |

Ihlal belirtileri: elle `Location` ile konum -> pencere boyutunda tasma; `AutoSize` + `DockStyle.Fill` -> `LayoutException`/sonsuz olcum; yanlis `Dock` sirasi -> panel birbirini yiyor; sabit piksel -> 150% DPI'da kirik ekran. Son kontrol: iki bacakta `dotnet build` -> 0 hata DOĞRULAMA GEREKLI.

---

## §9 Kanit

| Kaynak dosya | Ne alindi |
|---|---|
| `knowledge\winforms\layout-table-panel.md` (kaynak vault) | kalip tablosu, sira kurali, TLP kodu + ayar tablosu, TFM ve C# karsilastirmasi, ihlal belirtileri |
| `.rules\coding-standards.md` | tip yazim ve dosya kurali |
| `.rules\ui-rules.md` | ekran duzen kurali |

> Kanitsiz iddia icin DOĞRULAMA GEREKLI yazilir; bu dosyada kaynak disi bilgi yoktur.
