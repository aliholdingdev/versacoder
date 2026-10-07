---
title: "docking-saved-layout-xml - VersaCoder"
type: knowledge
category: devexpress
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "DevExpress v26.1 DockManager panel duzeninin XML'e kaydi/geri yuklenmesi: SaveLayoutToXml zamanlamasi, dosya konumu ve iki bacak farki"
---

# docking-saved-layout-xml — Docking Düzeni XML (v26.1)

**Zorunlu Baglantilar:** [index](../index.md) · [devexpress-rules](../../.rules/devexpress-rules.md) · [ADR-004](../../.decisions/accepted/ADR-004-devexpress-universal-v261-office2010.md) · [serialization-layout-persistence](serialization-layout-persistence.md) · [ribbon-auto-hide-scaling](ribbon-auto-hide-scaling.md)

> **Tek cumlelik sorumluluk:** *DockManager panel düzeni ne zaman XML'e yazılır, ne zaman geri yüklenir* sorusunun tek cevabıdır.

---

## §0 Ozet (Answer First)

`DockManager.SaveLayoutToXml(path)` düzeni yazar, `RestoreLayoutFromXml(path)` geri yükler. **Zamanlama kritik:** geri yükleme `InitializeComponent`'ten **sonra**, form görünmeden **önce**; kaydetme `FormClosing`'te. Panel adları (`Name`) sabit tutulmazsa XML'deki panel bulunamaz ve **sessizce yok sayılır**. IDE ekranı DX-09 gereği `DockManager` + `StatusBarControl` üzerine kurulur; düzen dosyası kullanıcı profiline yazılır, repo'ya girmez.

---

## §1 Zamanlama Tablosu

| An | Ne | Kod |
|---|---|---|
| Form ctor | Designer düzeni | `InitializeComponent()` |
| ctor sonu / `OnLoad` | XML'i uygula | `dockManager1.RestoreLayoutFromXml(path)` |
| Kullanıcı panel taşır | — | (runtime durum) |
| `FormClosing` | XML'e yaz | `dockManager1.SaveLayoutToXml(path)` |
| Panel adı değişti | **eski XML'i sil** | yoksa panel kaybolur |

```
InitializeComponent()  -->  RestoreLayoutFromXml()  -->  form GOSTER
                                   |
                     (kullanici panel dizer)
                                   |
                            FormClosing -> SaveLayoutToXml()
```

---

## §2 Anahtar API'ler

| API / Ozellik | Ne yapar | Not |
|---|---|---|
| `SaveLayoutToXml(string)` | tüm panel düzenini yazar | dock/float/auto-hide |
| `RestoreLayoutFromXml(string)` | düzeni geri yükler | dosya yoksa sessizce geç |
| `DockPanel.Name` | XML'deki anahtar | **sabit** tutulmalı |
| `DockPanel.Visibility` | `Visible` / `Hidden` / auto-hide | durum XML'e yazılır |
| `DockPanel.FloatSize` | yüzen panel boyutu | DPI'a göre değişir |

---

## §3 Kod (kaydet + geri yükle)

```csharp
private const string DockLayout = "dock-layout.xml";   // profil klasoru - §4

// Form acilisinda - InitializeComponent'ten SONRA
if (File.Exists(DockLayout))
    dockManager1.RestoreLayoutFromXml(DockLayout);

// Form kapanisinda - tek cagri (DX-13)
private void OnFormClosing(object sender, FormClosingEventArgs e)
    => dockManager1.SaveLayoutToXml(DockLayout);
```

| Durum | Sonuç | Önlem |
|---|---|---|
| Dosya yok | ilk açılışta Designer düzeni | normal |
| Panel `Name` değişti | panel kaybolur (eski XML) | açılışta XML'yi sil |
| Sürüm değişimi (26.1 → 26.2) | şema farkı riski | `NEEDS_VALIDATION`: şema uyumu test edilir |

---

## §4 Dosya Konumu

| Kaynak | Yol (net48 ve net10 ortak) |
|---|---|
| Kullanıcı düzeni | `Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "VersaCoder", "dock-layout.xml")` |
| Geçici test | `Path.GetTempPath()` |
| Repo içi | **yok** (`.gitignore`) |

> **YASAK:** sabit `C:\...` yol; string-concat ile yol/SQL üretimi (G2).

---

## §5 Iki Bacak (net48 / net10.0-windows)

| Konu | net48 | net10.0-windows |
|---|---|---|
| `SaveLayoutToXml` / `RestoreLayoutFromXml` | v26.1 (net48) | v26.1 (net10) — aynı imza |
| XML şeması | sürümle eşleşmeli | aynı sürüm → aynı şema |
| `ApplicationData` yolu | `%APPDATA%\VersaCoder` | aynı (API ortak) |
| Dil | **C# 7.3** — `using var` yasak | C# 14; ortak dosya 7.3 kalır |

---

## §6 Kardes Dosyalar

| Konu | Dosya |
|---|---|
| Grid + LayoutControl düzeni | [serialization-layout-persistence](serialization-layout-persistence.md) |
| Status bar (komşu öğe) | [bar-status-items](bar-status-items.md) |
| Ribbon daraltma (DPI dahil) | [ribbon-auto-hide-scaling](ribbon-auto-hide-scaling.md) |
| Form üçlüsü | [designer-runtime-customization](designer-runtime-customization.md) |
