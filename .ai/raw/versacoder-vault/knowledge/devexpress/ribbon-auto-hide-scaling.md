---
title: "ribbon-auto-hide-scaling - VersaCoder"
type: knowledge
category: devexpress
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "DevExpress v26.1 ribbon auto-hide (Minimized) ve DPI olcekleme: sabit boyut yerine anchoring, AutoScaleMode ve iki bacak farki"
---

# ribbon-auto-hide-scaling — Ribbon Auto-Hide ve DPI Ölçekleme (v26.1)

**Zorunlu Baglantilar:** [index](../index.md) · [devexpress-rules](../../.rules/devexpress-rules.md) · [ADR-004](../../.decisions/accepted/ADR-004-devexpress-universal-v261-office2010.md) · [theme-manager-office2010](theme-manager-office2010.md) · [accessibility-keyboard-nav](accessibility-keyboard-nav.md)

> **Tek cumlelik sorumluluk:** *Ribbon nasıl daraltılır ve form farklı DPI'da neden taşmaz* sorusunun tek cevabıdır.

---

## §0 Ozet (Answer First)

Ribbon daraltma `RibbonControl.Minimized` ile yönetilir: `true` iken yalnız **sayfa başlıkları** görünür, çift tıklama ile geri açılır. `RibbonForm` gövdeye ribbon'ı gömer; `RibbonStyle` her koşulda **yalnız `Office2010`** (DX-03). Ölçekleme ise iki şeyden gelir: **`AutoScaleMode`** (Designer değeri korunur, DX-14) ve **anchoring/docking** (sabit `Size`/konum yasak). `#if` yalnız platform DPI API'sinde kullanılır; layout kodu ortaktır.

---

## §1 Auto-Hide Akisi

```
ribbonControl1.Minimized = true      <-- yalniz sayfa basliklari
        |
Cift tikla sayfa basligi  ---------->  geri acilir (kullanici)
        |
ribbonControl1.Minimized = false     <-- kodda acik baslatma
```

| Durum | Görünen | Kullanım |
|---|---|---|
| `Minimized = true` | sayfa başlıkları (üst şerit) | dar ekran / editör odak |
| `Minimized = false` | tam ribbon + gruplar | IDE ana ekranı varsayılan |
| Çift tıklama | aç/kapa | `AllowMinimizeRibbonOnDoubleClick` ile kapatılabilir (`NEEDS_VALIDATION`: v26.1 tam üye adı) |

---

## §2 Anahtar Ozellikler

| Ozellik | Ne yapar | Varsayilan | Not |
|---|---|---|---|
| `ribbonControl1.Minimized` | ribbon'u daralt | `false` | runtime erişilir |
| `RibbonControlStyle` | ribbon stili | `Office2010` | **değiştirilemez (DX-03)** |
| `form.AutoScaleDimensions` | otomatik ölçek | Designer değeri | DX-14: dokunulmaz |
| `Control.Dock` / `Anchor` | yeniden boyutlanma | — | sabit `Size` yerine |
| `RibbonPage.HeaderDisplayMode` | başlık gösterimi | skin | `NEEDS_VALIDATION` (v26.1 üye adı) |

---

## §3 Kod (daralt + ölçek)

```csharp
// Acilista ribbon'u daralt (sadece sayfa basliklari)
ribbonControl1.Minimized = true;
ribbonControl1.RibbonStyle =
    DevExpress.XtraBars.Ribbon.RibbonControlStyle.Office2010;   // DX-03 sabit

// DPI guvenli yerlesim - sabit Size/Konum YASAK (DX-14)
mainPanel.Dock   = DockStyle.Fill;
statusBar1.Dock   = DockStyle.Bottom;
ribbonControl1.Dock = DockStyle.Fill;
```

---

## §4 DPI ve Olcek Kural Tablosu

| Kural | Doğru | Yanlış |
|---|---|---|
| Boyut | `Dock` / `Anchor` | sabit `Size = new Size(900, 600)` |
| Ölçek | `AutoScaleMode` Designer değeri | elle `AutoScaleMode = None` |
| Konum | parent layout | `Location = new Point(...)` sabit |
| `#if` | yalnız DPI API'sinde | tüm form kodunda |
| Tema | skin + appearance | sabit renk (bkz. [skin-customization](skin-customization.md)) |

Yüksek DPI testi: **%100, %150, %200** ölçeklerinde form açılıp kontrol taşması aranır.

---

## §5 Iki Bacak (net48 / net10.0-windows)

| Konu | net48 | net10.0-windows |
|---|---|---|
| `Minimized` / ribbon API | v26.1 (net48) | v26.1 (net10) — aynı |
| DPI varsayilani | uygulama manifesti / `AppContext` | .NET 10 **PerMonitorV2** davranışı — `NEEDS_VALIDATION`: manifest kontrolü |
| `AutoScaleMode` | `Dpi`/`Font` (Designer) | aynı dosya, farklı algı → **iki bacakta ayrı test** |
| Dil | **C# 7.3** — `using var` yasak | C# 14; ortak `.Designer.cs`/`.cs` 7.3 kalır |

> **YASAK:** `RibbonControlStyle.Office2010` dışındaki her stil (DX-03 / G3), `v26.1` dışı sürüm (DX-02).

---

## §6 Kardes Dosyalar

| Konu | Dosya |
|---|---|
| Global tema | [theme-manager-office2010](theme-manager-office2010.md) |
| Klavye/erişilebilirlik | [accessibility-keyboard-nav](accessibility-keyboard-nav.md) |
| Status bar dock | [bar-status-items](bar-status-items.md) |
| Docking panel ölçeği | [docking-saved-layout-xml](docking-saved-layout-xml.md) |
