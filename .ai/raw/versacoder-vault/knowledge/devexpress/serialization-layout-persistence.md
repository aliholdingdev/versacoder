---
title: "serialization-layout-persistence - VersaCoder"
type: knowledge
category: devexpress
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "DevExpress v26.1 layout serialization: grid/layout XML kayit-geri-yukleme, OptionsLayout, dosya konumu stratejisi ve iki bacak farki"
---

# serialization-layout-persistence — Layout Serialization (v26.1)

**Zorunlu Baglantilar:** [index](../index.md) · [devexpress-rules](../../.rules/devexpress-rules.md) · [ADR-004](../../.decisions/accepted/ADR-004-devexpress-universal-v261-office2010.md) · [designer-runtime-customization](designer-runtime-customization.md) · [docking-saved-layout-xml](docking-saved-layout-xml.md)

> **Tek cumlelik sorumluluk:** *Grid/form düzeni XML'e nasıl kaydedilir, ne zaman geri yüklenir ve dosya nereye konur* sorusunun tek cevabıdır.

---

## §0 Ozet (Answer First)

DevExpress düzeni (kolon sırası/genişliği, gruplama, `LayoutControl` yerleşimi) **XML'e serileştirilir**: `SaveLayoutToXml` / `RestoreLayoutFromXml`. Kaydetme **form kapanışında**, geri yükleme **`InitializeComponent` sonrası** yapılır — tersi olursa Designer değeri XML'i ezer. Kayıt dosyası **repo'ya girmez**; kullanıcı profili klasörüne yazılır. Sürüm `v26.1`, ribbon stili `Office2010` sabit; layout dosyası sürüm değişiminde **geri yükleme riski** taşır (eski XML → `NEEDS_VALIDATION`).

---

## §1 Kayit-Geri Yukleme Akisi

```
Form acilir
  InitializeComponent()        <-- Designer degerleri
        |
  RestoreLayoutFromXml(path)   <-- XML, Designer degerini EZER (istenen bu)
        |
  (kullanici kolonlari tasir / boyutlandirir)
        |
Form kapanir
  SaveLayoutToXml(path)        <-- yeni duzen XML'e yazar
```

| Zaman | Ne yapılır | Yapılmazsa |
|---|---|---|
| `InitializeComponent` **sonrası** | `RestoreLayoutFromXml` | XML okunmaz, düzen eski kalır |
| `FormClosing` | `SaveLayoutToXml` | değişiklik kaybolur |
| Kolon şeması değiştiyse | eski XML **silinir** | eksik/kayık kolon hatası |

---

## §2 Anahtar API'ler

| API | Ne yapar | Not |
|---|---|---|
| `gridView1.SaveLayoutToXml(path)` | görünüm düzenini yazar | kolon + grup + filtre |
| `gridView1.RestoreLayoutFromXml(path)` | düzeni geri yükler | `InitializeComponent` sonrası |
| `gridView1.OptionsLayout.StoreAllOptions` | **tüm** seçenekleri sakla | yoksa kısmi kayıt |
| `layoutControl1.SaveLayoutToXml(path)` | `LayoutControl` yerleşimi | forma özel |
| `Name` alanı | kayıt anahtarı | benzersiz olmalı |

---

## §3 Kod (kayıt + geri yükleme)

```csharp
private const string GridLayout = "grid-layout.xml";   // kullanici profili klasoru - §4

// Form acilisinda - InitializeComponent'ten SONRA
if (File.Exists(GridLayout))
    gridView1.RestoreLayoutFromXml(GridLayout);

// Form kapanisinda
private void OnFormClosing(object sender, FormClosingEventArgs e)
{
    gridView1.OptionsLayout.StoreAllOptions = true;     // tam kayit
    gridView1.SaveLayoutToXml(GridLayout);
}
```

> **UYARI:** Kayıt dosyası çalışma dizinine **konmaz** (CI/Program Files yazma izni yok); profil klasörüne (§5) konur.

---

## §4 Dosya Konumu Stratejisi

| Amaç | net48 | net10.0-windows |
|---|---|---|
| Kullanıcı düzeni | `%APPDATA%\VersaCoder\*.xml` (`Environment.SpecialFolder.ApplicationData`) | aynı |
| Paylaşılan (repo) | `.ai/` altına **konmaz** — `.gitignore` | aynı |
| Geçici test | `Path.GetTempPath()` | aynı |

| `Environment` kullanımı | Not |
|---|---|
| `Environment.GetFolderPath(SpecialFolder.ApplicationData)` | her iki bacakta da aynı yol döner |
| Sabit `C:\...` yol | **yasak** — makineye bağlı (kural ihlali) |

---

## §5 Iki Bacak (net48 / net10.0-windows)

| Konu | net48 | net10.0-windows |
|---|---|---|
| `SaveLayoutToXml` / `RestoreLayoutFromXml` | v26.1 (net48) | v26.1 (net10) — **aynı imza** |
| XML şeması | kontrol sürümüne bağlı | aynı sürüm → aynı şema |
| Dosya yolu | `ApplicationData` | `ApplicationData` (.NET 10'da `AppContext` alternatifi) |
| Dil | **C# 7.3** — `using var` yasak | C# 14; ortak dosya 7.3 kalır |

> **YASAK:** string-concat ile yol/SQL üretimi (G2), EF Core, `v26.1` dışı sürüm (DX-01/DX-02).

---

## §6 Kardes Dosyalar

| Konu | Dosya |
|---|---|
| Docking panel düzeni (XML) | [docking-saved-layout-xml](docking-saved-layout-xml.md) |
| Grid kolonları (kayıt anahtarı `Name`) | [gridview-columns-format](gridview-columns-format.md) |
| Designer/runtime sınırı | [designer-runtime-customization](designer-runtime-customization.md) |
| Pager/sanal mod (veri değil düzen) | [grid-paging-virtual-mode](grid-paging-virtual-mode.md) |
