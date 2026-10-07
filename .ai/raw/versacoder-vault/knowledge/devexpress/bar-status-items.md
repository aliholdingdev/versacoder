---
title: "bar-status-items - VersaCoder"
type: knowledge
category: devexpress
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "DevExpress v26.1 StatusBarControl + BarStaticItem/BarEditItem kullanimi, guncelleme noktalari, ribbon ile iliski ve iki bacak farki"
---

# bar-status-items — StatusBar ve Bar Öğeleri (v26.1)

**Zorunlu Baglantilar:** [index](../index.md) · [devexpress-rules](../../.rules/devexpress-rules.md) · [ADR-004](../../.decisions/accepted/ADR-004-devexpress-universal-v261-office2010.md) · [accessibility-keyboard-nav](accessibility-keyboard-nav.md) · [ribbon-auto-hide-scaling](ribbon-auto-hide-scaling.md)

> **Tek cumlelik sorumluluk:** *Durum çubuğu öğeleri nerede kurulur, ne zaman güncellenir* sorusunun tek cevabıdır.

---

## §0 Ozet (Answer First)

Durum çubuğu `StatusBarControl` + `BarStaticItem`/`BarEditItem` üçlüsüyle kurulur; **öğeler Designer'da** eklenir, **caption/durum kodda** güncellenir (DX-07/DX-13). Status bar, IDE ekranının (`DockManager` + `StatusBarControl`, DX-09) parçasıdır ve ribbon'a bağımlı değildir — yine de tema/stil `Office2010` ailesinden gelir. Güncelleme noktası **tek handler** olmalı; öğe başına ayrı ayrı `Caption` yazan kod tekrarı SRP ihlalidir.

---

## §1 Ogeler

| Öğe | Ne gösterir | Tıklanabilir mi | Tipik kullanım |
|---|---|---|---|
| `BarStaticItem` | sabit metin / sayaç | hayır | `"Oturum: X"`, `"12 satir"` |
| `BarEditItem` | editör (kombobox/spin) | evet | filtre, zoom |
| `BarButtonItem` | eylem | evet | `"Yenile"` (nadir) |
| `BarCheckItem` | aç/kapa | evet | `"Auto-fit"` |

```
StatusBarControl
   +-- BarStaticItem bsiSession   (sol: oturum adi)
   +-- BarStaticItem bsiRows      (sag: satir sayisi)
   +-- BarEditItem   beiZoom      (zoom editoru)
   +-- BarButtonItem bsiRefresh   (yenile eylemi)
```

---

## §2 Anahtar Ozellikler

| Ozellik | Ne is yarar | Not |
|---|---|---|
| `BarStaticItem.Caption` | görünen metin | **kodda güncellenen tek alan** |
| `BarStaticItem.Alignment` | sola/sağa hizalama | öğe bağlantısında |
| `BarEditItem.EditValue` | editör değeri | `DataSource` ile de beslenir |
| `BarEditItem.Edit` | bağlı editör (`RepositoryItem`) | Designer |
| `barManager.Items` | tüm bar öğeleri | paylaşım kaynağı |

> **KURAL:** Öğeler **tek bir** `BarManager` üzerinden tanımlanır; iki manager'da aynı ad = çakışma (DX-07).

---

## §3 Kod (tek güncelleme noktası)

```csharp
// Olay yonlendirici - tek yerde guncelleme (DX-13)
private void OnSessionLoaded(string name, int rowCount)
{
    bsiSession.Caption = "Oturum: " + name;   // BarStaticItem
    bsiRows.Caption    = rowCount + " satir";  // sag blok
    beiZoom.EditValue  = 100;                 // BarEditItem degeri
}

// Grid odagi degistiginde status bar'i besle
gridView1.FocusedRowObjectChanged += (s, e) => OnRowStatus();
```

| Yapılan | Yapılmayan |
|---|---|
| Designer'da öğe ekleme + adlandırma | kodda `new BarStaticItem()` ile kalıcı öğe kurma |
| Tek handler'da toplu güncelleme | her yerde ayrı `bsiX.Caption = ...` |
| Durum verisi servisten | status bar içinde sorgu çalıştırma |

---

## §4 Ribbon ile Iliski

| Soru | Cevap |
|---|---|
| Status bar ribbon'a bağlı mı? | Hayır — `RibbonControl` olmadan da çalışır |
| Stil ne? | global skin'den (bkz. [theme-manager-office2010](theme-manager-office2010.md)) |
| Ribbon stili değişir mi? | **Hayır** — `Office2010` sabit (DX-03) |
| Yerelleştirme? | metin `.resx` / kod — [localizing-ui-text](localizing-ui-text.md) |

---

## §5 Iki Bacak (net48 / net10.0-windows)

| Konu | net48 | net10.0-windows |
|---|---|---|
| `StatusBarControl` / `BarStaticItem` | v26.1 (net48) | v26.1 (net10) — aynı sınıf |
| `Caption` güncelleme | UI thread'de | UI thread'de (`Invoke` gerekirse aynı) |
| Yüksek DPI | `AutoScaleMode` korunur (DX-14) | .NET 10 DPI algısı farklı → [ribbon-auto-hide-scaling](ribbon-auto-hide-scaling.md) |
| Dil | **C# 7.3** — `using var` yasak | C# 14; ortak dosya 7.3 kalır |

---

## §6 Kardes Dosyalar

| Konu | Dosya |
|---|---|
| Klavye/erişilebilirlik | [accessibility-keyboard-nav](accessibility-keyboard-nav.md) |
| Docking (status bar'ın komşusu) | [docking-saved-layout-xml](docking-saved-layout-xml.md) |
| Ribbon yapısı + DPI | [ribbon-auto-hide-scaling](ribbon-auto-hide-scaling.md) |
| Grid odak olayı kaynağı | [gridcontrol-basics](gridcontrol-basics.md) |
