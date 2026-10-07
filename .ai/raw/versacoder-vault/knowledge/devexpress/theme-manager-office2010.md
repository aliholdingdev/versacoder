---
title: "theme-manager-office2010 - VersaCoder"
type: knowledge
category: devexpress
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "DevExpress v26.1 global gorunum (look & feel) yonetimi: UserLookAndFeel + SkinManager, Office2010 ribbon stili ile iliskisi, karar matrisi ve iki bacak farki"
---

# theme-manager-office2010 — Global Tema ve Office2010 (v26.1)

**Zorunlu Baglantilar:** [index](../index.md) · [devexpress-rules](../../.rules/devexpress-rules.md) · [ADR-004](../../.decisions/accepted/ADR-004-devexpress-universal-v261-office2010.md) · [skin-customization](skin-customization.md) · [ribbon-auto-hide-scaling](ribbon-auto-hide-scaling.md)

> **Tek cumlelik sorumluluk:** *Tema (skin) nereden tek noktada yönetilir ve ribbon stili neden temadan bağımsızdır* sorusunun tek cevabıdır.

---

## §0 Ozet (Answer First)

DevExpress v26.1'de "tema" üç ayrı katmandır: **ribbon stili** (DX-03: yalnız `RibbonControlStyle.Office2010`), **skin** (renk/dosya paketi, `UserLookAndFeel`), **appearance** (tek kontrolün görünümü). Global tema **tek noktadan** `UserLookAndFeel.Default` ile ayarlanır; tüm kontroller ondan miras alır. Ribbon stili hiçbir tema değişiminde değiştirilemez — bu [ADR-004](../../.decisions/accepted/ADR-004-devexpress-universal-v261-office2010.md) ve DX-03 kırmızı çizgisidir; skin adı ise `ui-designer` yetkisindedir. Sürüm **yalnız DevExpress Universal 2026 / v26.1**.

---

## §1 Gorunum Katmanlari (akıs)

```
UserLookAndFeel.Default              <-- tek global kaynak (kodda 1 kez ayarla)
      |
      +--> RibbonControl.RibbonStyle  <-- SADECE Office2010 (DX-03) -- temadan BAGIMSIZ
      |
      +--> Form/Control .LookAndFeel  <-- miras: kontrol -> form -> global
      |
      +--> SkinManager.Default.Skins  <-- kurulu skin adlari (kesif)
      |
      +--> AppearanceObject'lar       <-- en alt katman (bkz. skin-customization)
```

> **KURAL:** Ayar **bir kez**, uygulama başlangıcında yapılır; her formda tekrarlayan `SetSkinStyle` çağrısı dağınıklıktır (SRP).

---

## §2 Anahtar Ozellikler

| API / Ozellik | Ne is yarar | Varsayilan | Karar sahibi |
|---|---|---|---|
| `UserLookAndFeel.Default.SetSkinStyle(skin)` | tüm uygulamaya tek skin | sistem teması | `ui-designer` |
| `UserLookAndFeel.Default.Style` | klasik `LookAndFeelStyle` seçimi | — | skin ile birlikte kullanılmaz |
| `form.LookAndFeel` | forma özel override | `Default`'tan miras | `developer` (nadir) |
| `SkinManager.Default.Skins` | kurulu skin listesi (runtime keşif) | — | — |
| `ribbonControl1.RibbonStyle` | ribbon stili | `Office2010` | **sabit — kimse değiştirmez** |

---

## §3 Kod (tek ayar noktası)

```csharp
using DevExpress.LookAndFeel;

// Program.cs / MainForm kurucusu - TEK ayar noktasi (v26.1)
UserLookAndFeel.Default.SetSkinStyle("Office2010Blue");        // skin = renk paketi
ribbonControl1.RibbonStyle =
    DevExpress.XtraBars.Ribbon.RibbonControlStyle.Office2010;   // DX-03 - DEGISTIRILMEZ
this.LookAndFeel.SkinName = "Office2010Blue";                  // forma ozel gerekirse (nadir)
```

| Ayrım | Skin | Ribbon stili |
|---|---|---|
| API | `SetSkinStyle(string)` | `RibbonControlStyle` |
| Ne değişir | renk, glyph, dosya | ribbon düzeni/grafiği |
| Değişebilir mi | evet (`ui-designer`) | **hayır** (DX-03 / G3) |
| İhlalde | görsel tutarsızlık | **CRITICAL — build RED** |

---

## §4 Karar Matrisi

| Soru | Karar sahibi | Kaynak |
|---|---|---|
| Ribbon hangi stil? | sabit: `Office2010` — pazarlık yok | [ADR-004](../../.decisions/accepted/ADR-004-devexpress-universal-v261-office2010.md) |
| Skin hangi ad? | `ui-designer` | [devexpress-rules](../../.rules/devexpress-rules.md) |
| Global mi, forma özel mi? | `developer` | SRP — global varsayilan |
| Sürüm? | v26.1 tek sürüm | G3 / DX-01 |

---

## §5 Iki Bacak (net48 / net10.0-windows)

| Konu | net48 | net10.0-windows |
|---|---|---|
| Paket kaynagi | CPM `Directory.Packages.props` + `App.config` binding redirect **yalnız 26.1** | aynı CPM satırı, redirect gerekmez |
| `UserLookAndFeel` API | birebir aynı | birebir aynı |
| Dil sürümü | **C# 7.3** — `using var`, target-typed `new` yasak | C# 14 yazılabilir; ortak dosya 7.3'te kalır |
| Tema + DPI | `AutoScaleMode` Designer değeri korunur | .NET 10 DPI davranışı farklı → [ribbon-auto-hide-scaling](ribbon-auto-hide-scaling.md) |

> **YASAK:** DX-03'teki satır dışında ribbon stili, `v26.1` dışı sürüm, EF Core (G2 / G3 / DX-02).

---

## §6 Kardes Dosyalar

| Konu | Dosya |
|---|---|
| Appearance (üst katman) | [skin-customization](skin-customization.md) |
| Ribbon daraltma + DPI ölçek | [ribbon-auto-hide-scaling](ribbon-auto-hide-scaling.md) |
| Paket/CPM referansı | [nuget-references-cpm](nuget-references-cpm.md) |
| Designer edit kuralları | [designer-runtime-customization](designer-runtime-customization.md) |
