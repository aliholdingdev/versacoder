---
title: "Erisilebilirlik ve Klavye Navigasyonu - VersaCoder"
type: knowledge
category: devexpress
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "DevExpress v26.1 erisilebilirlik ve klavye navigasyonu: AccessibleName/Role, TabIndex/TabStop, BarItem.Shortcut, grid ok tuslari ve iki bacak farki"
---

# Erisilebilirlik ve Klavye Navigasyonu (v26.1)

**Zorunlu Baglantilar:** [bilgi indeksi](../index.md) · [vault index](../../index.md) · [kodlama standartlari](../../.rules/coding-standards.md) · [ADR-001 dual target](../../.decisions/accepted/ADR-001-dual-target-net48-net10.md) · [bar-status-items](bar-status-items.md) · [dx-validation-provider](dx-validation-provider.md)

> **Tek cumlelik sorumluluk:** *Ekran klavye ve ekran okuyucu ile uygulama kullanilabilir mi, etkilesimli noktalar nerede* sorusunun tek cevabidir.

## §0 Ozet (Answer First)

Erisilebilirlik uc parcadir: **metadate** (`AccessibleName` / `AccessibleRole`), **odak sirasi** (`TabIndex` / `TabStop`) ve **klavye yolu** (kisayol + ok tuslari). DevExpress kontrolleri WinForms erisilebilirlik altyapisini kullanir; ekrandaki her etkilesimli kontrolun okunabilir adi olmalidir. Ribbon'da eylemler `BarItem.Shortcut` ile, grid'de gezinme ok tuslariyla acilir; fare ile yapilabilen islem icin mutlaka klavye yolu da yazilir. Ribbon stili yalniz `RibbonControlStyle.Office2010`, surum yalniz DevExpress Universal 2026 / v26.1.

## §1 Erisilebilirlik katmanlari

| Katman | Ne kapsar | Nerede ayarlanir |
|---|---|---|
| Metadate | `AccessibleName`, `AccessibleDescription`, `AccessibleRole` | Designer + kod |
| Odak | `TabIndex`, `TabStop`, odak sirasi | Designer (Olaylar penceresi) |
| Mnemonik | baslikta `&` (Alt+harf) | `.resx` / Designer |
| Klavye yolu | `BarItem.Shortcut`, ok tuslari | Designer + kod |
| Gorsel geri bildirim | odak cercevesi (skin'den) | tema ayari |

```
Tab / Shift+Tab  ->  TabIndex sirasi  ->  odak gorunurlugu (skin)
     |
     +--> & (mnemonic) -> hizli erisim (Alt+K)
     +--> Ctrl+N        -> BarItem.Shortcut (ribbon eylemi)
     +--> Ok tuslari    -> grid gezinme (hucre -> hucre)
```

## §2 Anahtar ozellikler

| Ozellik | Ne is yarar | Ornek deger |
|---|---|---|
| `AccessibleName` | ekran okuyucunun okudugu ad | `"Agenti calistir"` |
| `AccessibleRole` | kontrol turu | `AccessibleRole.PushButton` |
| `TabIndex` | odak sirasi | `0`, `1`, `2` (Designer) |
| `TabStop` | odaklanip odaklanmama | `false` (yalniz gorsel) |
| `BarItem.Shortcut` | ribbon eylem kisayolu | `Ctrl+N` |
| `OptionsBehavior.Editable` | grid klavye duzenleme | raporda `false` |

## §3 Kod (metadate + kisayol)

```csharp
using System.Windows.Forms;
using DevExpress.XtraBars;

// 1) Okunabilir ad/rol - Designer'da ayarla, kodda test edilir
btnRun.AccessibleName = "Agenti calistir";
btnRun.AccessibleRole = AccessibleRole.PushButton;

// 2) Ribbon eylemi klavyeyle acilir (fare zorunlu akis YAZMAZ)
bsiNewSession.Shortcut = new BarShortcut(Keys.Control | Keys.N);
btnRun.TabStop = true;                      // odaklanabilir
```

| Kontrol | Klavye davranisi | Not |
|---|---|---|
| `GridControl` | ok tuslariyla hucre gezinme | `Editable=false` iken salt okunur |
| `RibbonControl` | `Ctrl+Tab` + kisayollar | `Office2010` duzeninde sabit |
| `TreeList` | `+` / `-` genisletme | agac gezinme |
| `MemoEdit` | `Enter` yeni satir (`AcceptsReturn`) | not/mesaj alani |

## §4 Inceleme listesi

- [ ] Her etkilesimli kontrolde `AccessibleName` bos degil
- [ ] `TabIndex` tasarimla uyumlu, `TabStop=false` olanlar bilincli
- [ ] Sadece fare ile yapilan islem yok (kisayol veya tus var)
- [ ] Odak gorunur (skin odak cercevesi kapatilmamis)
- [ ] Ribbon eylemlerinin `Shortcut` aksami yok

## §5 Iki bacak (net48 / net10.0-windows)

| Konu | net48 | net10.0-windows |
|---|---|---|
| `AccessibleName/Role` | WinForms standart | ayni (UIA koprusu daha guclu) |
| Ekran okuyucu testi | Windows + NVDA/IA2 | ayni arac, surucu farkli -> **iki bacakta ayri test** |
| Kisayol (`BarShortcut`) | v26.1 (net48) | v26.1 (net10) - ayni imza |
| Dil | **C# 7.3** - `using var`, target-typed `new` yasak | C# 14; ortak dosya 7.3 kalir |

## §6 Dogrulama ve katman siniri

| Kural | Yeri | Not |
|---|---|---|
| Anlik geri bildirim (hata ikonu) | `DXErrorProvider` + `Validating` | bkz. [dx-validation-provider](dx-validation-provider.md) |
| Durum metni (sayac/ad) | status bar ogeleri | bkz. [bar-status-items](bar-status-items.md) |
| Isleme kurali | ViewModel / servis | View'a gomulmez |
| Kalici veri | ADO.NET + Dapper, `@p1` parametreli | EF Core onerilmez (ADR-005) |

## §7 Yaygin hatalar

| # | Hata | Sonuc | Duzeltme |
|---|---|---|---|
| 1 | `AccessibleName` bos birakmak | okuyucu yalniz "dugme" der | her etkilesimli kontrole ad |
| 2 | Tek fare akisi yazmak | klavye ile ulasilamaz | `BarItem.Shortcut` veya ok tusu |
| 3 | `TabIndex` sifirlamak | odak atlama | Designer'da sirayla kur |
| 4 | Odak cercevesini gizlemek | odak gorunmez | skin ayarina dokunma |
| 5 | Kisayollari kod icinde dagitmak | aksama ve karmasa | tek kisayol listesi |

## §8 Kardes dosyalar

| Konu | Dosya |
|---|---|
| Durum cubugu ogeleri | [bar-status-items](bar-status-items.md) |
| Alan dogrulama + ikon | [dx-validation-provider](dx-validation-provider.md) |
| Ribbon formu ve stil siniri | [designer-runtime-customization](designer-runtime-customization.md) |
| Kosullu bicim ile geri bildirim | [grid-conditional-format-rules](grid-conditional-format-rules.md) |

## §9 Kanit

| Kaynak dosya | Alinan kanit | Durum |
|---|---|---|
| `accessibility-keyboard-nav.md` | katman tablosu, ozellik tablosu, `BarShortcut` ornegi, iki bacak tablosu | dogrulandi |
| `bar-status-items.md` | status bar ogeleri ile iliski | dogrulandi |
| `dx-validation-provider.md` | geri bildirim katmani siniri | dogrulandi |
| Ekran okuyucu surucu davranisi | kaynakta `NEEDS_VALIDATION` | **DOĞRULAMA GEREKLI** |
