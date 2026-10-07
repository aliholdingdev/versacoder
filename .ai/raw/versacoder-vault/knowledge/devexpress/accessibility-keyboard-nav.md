---
title: "accessibility-keyboard-nav - VersaCoder"
type: knowledge
category: devexpress
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "DevExpress v26.1 erisilebilirlik ve klavye navigasyonu: AccessibleName/Role, tab sirasi, kisayollar, grid/ribbon klavyesi ve iki bacak farki"
---

# accessibility-keyboard-nav — Erişilebilirlik ve Klavye Navigasyonu (v26.1)

**Zorunlu Baglantilar:** [index](../index.md) · [devexpress-rules](../../.rules/devexpress-rules.md) · [ADR-004](../../.decisions/accepted/ADR-004-devexpress-universal-v261-office2010.md) · [bar-status-items](bar-status-items.md) · [ribbon-auto-hide-scaling](ribbon-auto-hide-scaling.md)

> **Tek cumlelik sorumluluk:** *Ekran klavye ve ekran okuyucu ile kullanılabilir mi, kontrol noktaları nerede* sorusunun tek cevabıdır.

---

## §0 Ozet (Answer First)

Erişilebilirlik üç parçadır: **metadate** (`AccessibleName`/`AccessibleRole`), **odak sırası** (`TabIndex`/`TabStop`), **klavye yolu** (kısayol + ok tuşları). DevExpress kontrolleri WinForms erişilebilirlik altyapısını kullanır; ekrandaki **her etkileşimli kontrolün** okunabilir adı olmalıdır. Ribbon'da eylemler `BarItem.Shortcut` ile, grid'de gezinme ok tuşlarıyla sağlanır; fare zorunlu akış **yazılmaz**. Ribbon stili sabit `Office2010`, sürüm `v26.1`.

---

## §1 Erisilebilirlik Katmanlari

| Katman | Ne kapsar | Nerede ayarlanır | Sahip |
|---|---|---|---|
| Metadate | `AccessibleName`, `AccessibleDescription`, `AccessibleRole` | Designer + kod | `ui-designer` |
| Odak | `TabIndex`, `TabStop`, `DefaultFocus` | Designer (Olaylar/Taşıma) | `ui-designer` |
| Mnemonik | başlıkta `&` (Alt+harf) | `.resx` / Designer | `ui-designer` |
| Klavye yolu | `BarItem.Shortcut`, ok tuşları | Designer + kod | `developer` |
| Görsel geri bildirim | odak çerçevesi (skin'den) | tema | `ui-designer` |

```
Tab / Shift+Tab  -->  TabIndex sirasi  -->  odak gorunurlugu (skin)
     |
     +--> & (mnemonic) -> hizli erisim (Alt+K)
     +--> Ctrl+X        -> BarItem.Shortcut (ribbon eylemi)
     +--> Ok tuslari    -> grid gezinme (hucre -> hucre)
```

---

## §2 Anahtar Ozellikler

| Ozellik | Ne is yarar | Tipik deger |
|---|---|---|
| `AccessibleName` | ekran okuyucunun okuduğu ad | `"Agenti calistir"` |
| `AccessibleRole` | kontrol türü | `AccessibleRole.PushButton` |
| `TabIndex` | odak sırası | 0,1,2... (Designer) |
| `TabStop` | odağa girip girmeme | `false` (sadece görüntü) |
| `BarItem.Shortcut` | ribbon eylem kısayolu | `Ctrl+N` |
| `OptionsBehavior.Editable` | grid klavye düzenleme | raporda `false` |

---

## §3 Kod (metadate + kısayol)

```csharp
using System.Windows.Forms;
using DevExpress.XtraBars;

// 1) Okunabilir ad/rol - Designer'da ayarla, kodda test edilir
btnRun.AccessibleName = "Agenti calistir";
btnRun.AccessibleRole = AccessibleRole.PushButton;

// 2) Ribbon eylemi klavyeyle acilir (fare YAZMAZ)
bsiNewSession.Shortcut = new BarShortcut(Keys.Control | Keys.N);
btnRun.TabStop = true;                      // odaklanabilir
```

| Kontrol | Klavye davranışı | Not |
|---|---|---|
| `GridControl` | ok tuşlarıyla hücre gezinme | `Editable=false` iken salt okunur |
| `RibbonControl` | `Ctrl+Tab` / kısayollar | `Office2010` düzeninde sabit |
| `TreeList` | `+`/`-` genişletme | [treelist-hierarchical-data](treelist-hierarchical-data.md) |
| `MemoEdit` | `Enter` yeni satır (`AcceptsReturn`) | chat ekranı |

---

## §4 Kontrol Listesi (incelemede)

- [ ] Her etkileşimli kontrolde `AccessibleName` **boş değil**
- [ ] `TabIndex` tasarımla uyumlu, `TabStop=false` olanlar bilinçli
- [ ] Sadece fare ile yapılan işlem **yok** (kısayol/tuş)
- [ ] Odak görünür (skin odak çerçevesi kapatılmamış)
- [ ] Ribbon eylemlerinin `Shortcut`'ı çakışmıyor

---

## §5 Iki Bacak (net48 / net10.0-windows)

| Konu | net48 | net10.0-windows |
|---|---|---|
| `AccessibleName/Role` | WinForms standart | aynı (UIA köprüsü daha güçlü) |
| Ekran okuyucu testi | Windows + NVDA/IA2 | aynı araç, sürücü davranışı farklı → **iki bacakta ayrı test** |
| Kısayol (`BarShortcut`) | v26.1 (net48) | v26.1 (net10) — aynı imza |
| Dil | **C# 7.3** — `using var`, target-typed `new` yasak | C# 14; ortak dosya 7.3 kalır |

> **YASAK:** `v26.1` dışı sürüm, `RibbonControlStyle.Office2010` dışı ribbon, EF Core (DX-02 / DX-03 / G2).

---

## §6 Kardes Dosyalar

| Konu | Dosya |
|---|---|
| Status bar sayaç/adları | [bar-status-items](bar-status-items.md) |
| DPI / odak ölçeği | [ribbon-auto-hide-scaling](ribbon-auto-hide-scaling.md) |
| TreeList klavye gezinme | [treelist-hierarchical-data](treelist-hierarchical-data.md) |
| Grid editör yolu | [grid-editing-modes](grid-editing-modes.md) |
