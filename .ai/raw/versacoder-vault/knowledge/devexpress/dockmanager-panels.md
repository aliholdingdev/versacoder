---
title: "DockManager ve Panel Yonetimi - VersaCoder"
type: knowledge
category: devexpress
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "DevExpress DockManager ile yan paneller olusturma, konumlandirma, AutoHide/Hidden durumlari ve IDE benzeri duzen"
---

# DockManager ve Panel Yonetimi

**Zorunlu Baglantilar:** [vault index](../../index.md) · [devexpress-rules](../../.rules/devexpress-rules.md) · [ADR-004](../../.decisions/accepted/ADR-004-devexpress-universal-v261-office2010.md) · [bilgi index](../index.md) · [docklayout-persistence](docklayout-persistence.md) · [tabcontrol-multipage](tabcontrol-multipage.md)

> **Tek cumlelik sorumluluk:** *IDE ekranindaki sol/sag/alt panellerin hangi kontrolle kuruldugu ve durumlarinin nasil degistirildigi* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

`DockManager` (`DevExpress.XtraBars.Docking`) formdaki tum yan panellerin sahibidir; her panel bir `DockPanel`'dir ve `DockingStyle.Left/Right/Bottom/Top` ile kenara yapışır. Panel durumu **uc degerle** oynar: `Visible` (acik), `Hidden` (kapali), `AutoHide` (kenarda sekmeli gizli). Paneller **sabit `Dock`'lanamaz**; koordinatlar `DockManager` tarafindan yonetilir, formda `Anchor/Dock` ile ugrasmak yanlistir. VersaCoder IDE ekraninda `DockManager` + `StatusBarControl` birlikte kullanilir (DX-09).

---

## §1 Temel kullanim

```csharp
using DevExpress.XtraBars.Docking;

DockPanel panel = dockManager1.AddPanel(DockingStyle.Left);
panel.Text = "Cozum Gezgini";
panel.Visibility = DockPanelVisibility.AutoHide;   // kenarda gizli sekme
```

```
Form (RibbonForm)
  +------------------------------------------+
  | Ribbon (Dock=Top)                        |
  +--------+------------------------+--------+
  | Sol    |                        | Sag    |
  | panel  |   merkez govde         | panel  |
  |(Auto-  |   (NavigationFrame /   |(Hidden)|
  | Hide)  |    XtraTabControl)     |        |
  +--------+------------------------+--------+
  | StatusBar (Dock=Bottom)                  |
  +------------------------------------------+
   DockManager tum panel konumlarini tutar
```

---

## §2 Ozellik tablosu

| Nesne / ozellik | Tur | Ne is yapar |
|---|---|---|
| `dockManager1.AddPanel(DockingStyle)` | method | Yeni panel olusturur ve kenara yerlestirir |
| `dockManager1.DockPanels` | koleksiyon | Mevcut tum paneller |
| `DockPanel.Text` | `string` | Panel basligi (Caption) |
| `DockPanel.Visibility` | enum | `Visible` / `Hidden` / `AutoHide` |
| `DockPanel.Dock` | `DockingStyle` | Panelin kenari (Left/Right/Top/Bottom) |
| `DockPanel.FloatLocation` / `FloatSize` | `Point`/`Size` | Yuzen panel konumu/olcusu → `NEEDS_VALIDATION` |
| `barManager1.DockManager` | baglanti | Bar + panel duzenini **ayni yerde** kaydeder |
| `dockManager1.SerializationOptions` | secenek | Kaydet/yukle davranisi (bkz. kalıcılık dosyasi) |
| `PanelContainer` | tur | Birbirine yapışik panel gruplari |

---

## §3 net48 ve net10.0-windows'da fark

| Konu | `net48` | `net10.0-windows` |
|---|---|---|
| Panel kurulumu | Ayni API | Ayni API |
| Panel icerigi (kullanici kontrol) | UserControl uctan olusur | Ayni; `partial` + Designer |
| Konum kaydi yolu | `Application.StartupPath` veya `%LOCALAPPDATA%` | `Environment.SpecialFolder.LocalApplicationData` |
| DPI | Panel basligi 16px ikon; manifest DPI | Otomatik olcek; `AutoScaleMode` sabit (DX-14) |
| `#if` | Yalniz dosya yolu/DPI farkinda | Ayni; panel mantigi #if altina ALINMAZ |

---

## §4 Yaygin hatalar

| # | Hata | Sonuc | Duzeltme |
|---|---|---|---|
| 1 | Panelleri `Dock`/`Anchor` ile sabitlemek | Boyutlandirma catirdar, DockManager ile carpisir | `AddPanel(DockingStyle)` |
| 2 | Paneli `new DockPanel()` ile eklemek | Yoneticiye kayit yok, duzen kaydedilmez | `dockManager1.AddPanel` |
| 3 | `Visibility` yerine `Visible=false` | Beklenen davranis degil (AutoHide ayni kalmaz) | `DockPanelVisibility` |
| 4 | Merkez govdeyi de panele koymak | IDE ortasi kaybolur | Merkez dogrudan forma Dock=Fill |
| 5 | Panel metnini kodda uretmek | Yerellestirme kaybi | Caption Designer/resx |

---

## §5 Kanit ve dogrulama

| Kalem | Durum |
|---|---|
| `AddPanel(DockingStyle)`, `DockPanel.Text` | v26.1 WinForms ornegi ile dogrulandi |
| `DockPanelVisibility` enum degerleri | `NEEDS_VALIDATION` (designer'dan dogrula) |
| `FloatLocation` / `FloatSize` | `NEEDS_VALIDATION` |
| `dotnet build` | Bu batch'te alinmadi → `NEEDS_VALIDATION` |
| Ilgili kural | DX-09 (IDE = DockManager + StatusBarControl) |

**Kardesler:** [docklayout-persistence](docklayout-persistence.md) · [tabcontrol-multipage](tabcontrol-multipage.md) · [layoutcontrol-grouping](layoutcontrol-grouping.md) · [splashscreen-waitform](splashscreen-waitform.md)
