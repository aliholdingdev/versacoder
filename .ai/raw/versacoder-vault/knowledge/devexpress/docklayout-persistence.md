---
title: "Dock ve Form Duzeni Kalıcılığı - VersaCoder"
type: knowledge
category: devexpress
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "DockManager / LayoutControl / Ribbon duzenini XML, Stream veya Registry'ye kaydetme ve geri yukleme sirasi, surum uyumu"
---

# Dock ve Form Duzeni Kalıcılığı

**Zorunlu Baglantilar:** [vault index](../../index.md) · [devexpress-rules](../../.rules/devexpress-rules.md) · [ADR-004](../../.decisions/accepted/ADR-004-devexpress-universal-v261-office2010.md) · [bilgi index](../index.md) · [dockmanager-panels](dockmanager-panels.md) · [layoutcontrol-grouping](layoutcontrol-grouping.md)

> **Tek cumlelik sorumluluk:** *Kullanicinin panel/sayfa duzeni kapat-acildiginda nasil geri gelir* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

DevExpress kontrolleri duzeni **`SaveLayoutToXml` → `RestoreLayoutFromXml`** ikilisiyle kaydeder (registry ve stream varyantlari da var; `LoadLayoutFromXml` **yoktur**, dogru ad `RestoreLayoutFromXml`). Kayit, kontroller **hazir olduktan** sonra (`FormClosing`) yapilir, yukleme `Form.Load` sirasinda kontrol olusturulduktan sonra. Ayni XML'e **birden fazla duzen** yazilmaz; coklu duzen icin Workspace Manager gerekir. Kaydedilecek alan `SerializationOptions` (DockManager) / `OptionsLayout` (Grid, Tile) / `OptionsSerialization` (LayoutControl) ile sinirlandirilir.

---

## §1 Temel kullanim

```csharp
using System;
using System.IO;
using DevExpress.XtraBars.Docking;

string file = Path.Combine(
    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
    "VersaCoder\\dock-layout.xml");

dockManager1.SaveLayoutToXml(file);          // FormClosing'de
if (File.Exists(file))
    dockManager1.RestoreLayoutFromXml(file); // Load sonunda, panel olusturulduktan sonra
```

---

## §2 Kontrol bazli kayit tablosu

| Kontrol | Kaydet | Geri yukle | Ayar nesnesi |
|---|---|---|---|
| `DockManager` | `SaveLayoutToXml/Stream/Registry` | `RestoreLayoutFromXml/Stream/Registry` | `SerializationOptions` |
| `LayoutControl` | `SaveLayoutToXml` | `RestoreLayoutFromXml` | `OptionsSerialization` |
| `TileControl` | `SaveLayoutToXml` | `RestoreLayoutFromXml` | `OptionsLayout` |
| `GridControl` / `TreeList` | `SaveLayoutToXml` | `RestoreLayoutFromXml` | `OptionsLayout` |
| `BarManager` (+ DockManager) | `SaveLayoutToRegistry` | `RestoreLayoutFromRegistry` | `RegistryPath` |
| `RibbonControl` | `AutoSaveLayoutToXml` | `AutoSaveLayoutToXmlPath` | otomatik |

> **SIRA KURALI:** Yukleme, o layout'i olusturan tum kontroller **olusturulduktan sonra** yapilmalidir; aksi halde kayip panel/id eslesmez.

---

## §3 net48 ve net10.0-windows'da fark

| Konu | `net48` | `net10.0-windows` |
|---|---|---|
| Kayit yeri | `%LOCALAPPDATA%` (registry de mumkun) | `%LOCALAPPDATA%`; registry ayni calisir |
| Dosya yolu uretimi | `Application.StartupPath` / `SpecialFolder` | `AppContext.BaseDirectory` + `SpecialFolder` |
| Registry yazma | `Microsoft.Win32.Registry` gomye | Ayni namespace, uyari seviyesi farkli olabilir |
| XML serializer | `System.Xml` framework | Ayni API (`System.Xml`) |
| `#if` | Yalniz yolu/registry farkinda | Ayni; kayit kodu #if altina ALINMAZ |

---

## §4 Yaygin hatalar

| # | Hata | Sonuc | Duzeltme |
|---|---|---|---|
| 1 | `LoadLayoutFromXml` yazmak | Derleme hatasi (boyle metod yok) | `RestoreLayoutFromXml` |
| 2 | Konstruktorun basinda yukleme | Panel henuz yok, duzen bos | Load sonunda, kontrollerden sonra |
| 3 | XML'i `Program.cs`'e yazmak (Program Files) | Yetki hatasi (UAC) | `LocalApplicationData` |
| 4 | Ayni dosyaya birden cok duzen | Son yazilan oncekini siler | Workspace Manager |
| 5 | Eski surum XML'ini yukleme | Sessiz bozuk duzen | Dosya adina surum/ek ekle (v26.1) |

---

## §5 Kanit ve dogrulama

| Kalem | Durum |
|---|---|
| `SaveLayoutToXml` / `RestoreLayoutFromXml` (DockManager) | v26.1 dokumantasyonu ile **dogrulandi** |
| `SerializationOptions` / `OptionsLayout` / `OptionsSerialization` | dokuman karsiligi ile dogrulandi |
| Dosya yolu izinleri (net48 UAC) | `NEEDS_VALIDATION` (paket testinde dogrula) |
| `dotnet build` | Bu batch'te alinmadi → `NEEDS_VALIDATION` |

**Kardesler:** [dockmanager-panels](dockmanager-panels.md) · [tabcontrol-multipage](tabcontrol-multipage.md) · [accordion-navigation](accordion-navigation.md) · [tile-control-startpage](tile-control-startpage.md)
