---
title: "data-binding-basics - VersaCoder"
type: knowledge
category: winforms
status: active
version: 1.0.0
updated: 2026-09-30
authority: primary
scope: "WinForms veri baglamanin parcalarini (Binding, BindingSource, CurrencyManager), yon ve guncelleme kurallarini ve ihlal belirtilerini tanimlar"
tags: [knowledge]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# Data Binding Temelleri

**Zorunlu Baglantilar:** [[knowledge/winforms/validation-form-level|validation-form-level]] · [[.rules/data-access-rules|data-access-rules]] · [[.rules/coding-standards|coding-standards]] · [[knowledge/winforms/designer-cs-trio-rule|designer-cs-trio-rule]] · [[knowledge/index|vault indeks]]
**Küme + Hub:** [[knowledge/winforms/index|winforms index]] · [[index|index]] · [[claude|CLAUDE]] · [[definitions/agents|AGENTS]] · [[.decisions/brain|brain]] · [[definitions/glossary|glossary]] · [[governance/keys|keys]]
**Kardeşler (winforms 3/32):** [[knowledge/winforms/accessibility-tab-and-mnemonics|accessibility-tab-and-mnemonics]] [[knowledge/winforms/anchor-dock-layout-rules|anchor-dock-layout-rules]] [[knowledge/winforms/async-await-ui-thread|async-await-ui-thread]] [[knowledge/winforms/async-loading-patterns|async-loading-patterns]] [[knowledge/winforms/async-ui-thread-marshalling|async-ui-thread-marshalling]] [[knowledge/winforms/async-void-event-handlers|async-void-event-handlers]] [[knowledge/winforms/backgroundworker-vs-task|backgroundworker-vs-task]] [[knowledge/winforms/cancellation-in-ui|cancellation-in-ui]] [[knowledge/winforms/cancellation-token-in-ui|cancellation-token-in-ui]] [[knowledge/winforms/composition-root-host-startup|composition-root-host-startup]] [[knowledge/winforms/control-disposal-and-memory|control-disposal-and-memory]] [[knowledge/winforms/controls-lifecycle-events|controls-lifecycle-events]] [[knowledge/winforms/designer-cs-anatomy|designer-cs-anatomy]] [[knowledge/winforms/designer-editing-rules|designer-editing-rules]] [[knowledge/winforms/designer-hand-edit-policy|designer-hand-edit-policy]] [[knowledge/winforms/di-container-lifetimes|di-container-lifetimes]] [[knowledge/winforms/docking-panels-patterns|docking-panels-patterns]] [[knowledge/winforms/errorprovider-validation-flow|errorprovider-validation-flow]] [[knowledge/winforms/exception-boundary-ui|exception-boundary-ui]] [[knowledge/winforms/form-close-and-dispose|form-close-and-dispose]] [[knowledge/winforms/form-close-cancel-pattern|form-close-cancel-pattern]] [[knowledge/winforms/form-constructor-vs-load|form-constructor-vs-load]] [[knowledge/winforms/form-load-and-shown-events|form-load-and-shown-events]] [[knowledge/winforms/layout-table-panel|layout-table-panel]] [[knowledge/winforms/logging-integration|logging-integration]] [[knowledge/winforms/partial-class-and-file-naming|partial-class-and-file-naming]] [[knowledge/winforms/settings-user-config|settings-user-config]] [[knowledge/winforms/splitcontainer-and-sizing|splitcontainer-and-sizing]] [[knowledge/winforms/winforms-rendering-performance|winforms-rendering-performance]]

> **Tek cumlelik sorumluluk:** *Bir control'e deger nasil baglanir, kim guncel kaydi bilir, iki yonlu guncelleme ne zaman patlar* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

WinForms baglamasi uc parcadan olusur: **`Binding`** (control property <-> kaynak property), **`BindingSource`** (sira + filtre + koleksiyon uzerindeki tek muhatap), **`CurrencyManager`** (aktif satir: `Position`, `Current`). Kaynak **`INotifyPropertyChanged`** implemente etmiyorsa alan guncellemesi ekrana yansimaz; `DataSource`'a **yeni liste** vermek (`ResetBindings`) ile tek satir guncellemek (`ResetCurrentBinding`) ayri islerdir. Veri kaynagi bu vault'ta **ADO.NET + Dapper**'dir; baglama **ara katmanin DTO'su** uzerinden yapilir, `DataTable` yalnizca hizli liste icindir.

```text
control property  <--- Binding --->  DTO / IList  <--- BindingSource --->  Position (aktif satir)
                                          |
                                    INotifyPropertyChanged yoksa -> ekranda ESKI deger kalir
```

---

## §1 Baglama parcalari

| Parca | Ne yapar | Tipik kullanim |
|---|---|---|
| `Binding` | tek property eslesmesi | `nameBox.DataBindings.Add("Text", src, "Name")` |
| `BindingSource` | sira + filtre + `AddNew`/`Remove` | `Position`, grid besleme |
| `CurrencyManager` | aktif kayit | `bindingSource.CurrencyManager.Position` |
| `INotifyPropertyChanged` | alan degisimi ekrana yansir | DTO / view-model |
| `IList` / `DataTable` | koleksiyon kaynagi | liste, hizli tablo |
| `BindingNavigator` | ileri/geri/kaydet cubugu | klasik CRUD |

```csharp
var source = new BindingSource();
source.DataSource = _sessions.LoadAll();       // IList<SessionDto>

nameBox.DataBindings.Add("Text", source, "Name");
idLabel.DataBindings.Add("Text", source, "Id");
grid.DataSource = source;
```

---

## §2 Yon ve guncelleme kurallari

| Durum | Davranis | Cozum |
|---|---|---|
| kaynak `INotifyPropertyChanged` yayiyor | alan -> ekran **canli** | DTO'da `OnPropertyChanged` |
| kaynak yalniz `IList` | yalnizca satir degisimi yansir | `ResetBindings()` |
| `DataSource` = yeni liste | sifirdan yukleme | `ResetBindings(...)` |
| control `Text` bosaltildi, `FormattingEnabled=false` | bosaltma yazilmaz | gerekliyse `FormattingEnabled=true` |
| bir alan baska alani dogruluyor | `Validating` zinciri | `AutoValidate` (bkz. [[knowledge/winforms/validation-form-level|validation-form-level]]) |
| kaynakta SQL | **YASAK** (G2 / DA-03) | Dapper + `@p1` parametreli sorgu |

> **KURAL:** Control'e **dogrudan** bagli DTO **kaydetme icin kullanilmaz**; kaydetme `Application` katmanina gider ve parametreli SQL ile yapilir.

---

## §3 TFM farki (net48 vs net10.0-windows)

| Konu | `net48` | `net10.0-windows` |
|---|---|---|
| `Binding` / `BindingSource` / `CurrencyManager` | var | var - **ayni API** |
| `INotifyPropertyChanged` | var | var |
| `DataTable` / `DataView` | `System.Data` yerlesik | Windows Desktop paketinde yerlesik |
| nullable anotasyonu | yok | `string?` / `T?` |
| `System.ComponentModel.Annotations` | yerlesik | yerlesik (DataAnnotations) |
| `await foreach` | paket gerekir | `AsyncEnumerable` (10.0+) |

> **Sonuc:** baglama **API'si iki bacakta birebir aynidir**; fark yalnizca NRT anotasyonu ve asenkron koleksiyon yardimcilari. Kod `#if` **icermemeli**.

---

## §4 C# 7.3 (net48) vs C# 14 - ortak kaynak tablosu

| Ozellik | C# 7.3 | C# 14 | Ortak kaynak |
|---|---|---|---|
| property bildirimi | blok `get/set` | expression body (`=>`) | expression body 7.3'te de var (getter icin) |
| event yayimi | `var h = Ev; if (h != null)` | `Ev?.Invoke(...)` | `?.` 6.0+ -> ortak kodda da calisir |
| `nameof` | var (6.0+) | ayni | **her iki bacakta ayni** |
| nullable event | `PropertyChangedEventHandler` | `?` | 7.3'te `?` yok |

INPC kalibi her iki bacakta da ayni isi gorur: `if (value == x) return;` kisa devre, sonra alan yazimi, sonra `PropertyChanged` yayimi (7.3: handler kopyala + `!= null`; 14: `?.Invoke` + `nameof`).

---

## §5 Ihlal belirtisi

| Ihlal | Belirti | Duzeltme |
|---|---|---|
| DTO `INotifyPropertyChanged` yok | alan guncellenince ekranda eski deger | DTO'ya INPC ekle ya da `ResetBindings` |
| her tus vurusunda tam liste reset | ekran titremesi, kayit konumu kaybi | yalniz satir guncellemesi / `Position` |
| control'e dogrudan DB nesnesi | UI katmani veri erisimi biliyor (G2 riski) | DTO + `Application` katmani |
| `grid.DataSource = null` ile temizleme | abonelikler kopmaz | `Dispose` + `DataSource = null` sirasi |
| SQL'i baglama kodunda yazmak | string-concat SQL (DA-03) | Dapper parametreli (ADR-005) |

---

## §6 Tarama tablosu (kim neyi degistirir)

| Aranacak | `.Form.cs` | `.Designer.cs` | Beklenen |
|---|---|---|---|
| `DataBindings.Add` | veri baglama kodu | - | DTO uzerinden |
| `new BindingSource()` | kodda kurulur | - | tek muhatap |
| `SuspendLayout(` | **0** DOĞRULAMA GEREKLI | var | - |
| string-concat SQL | **0** DOĞRULAMA GEREKLI | **0** DOĞRULAMA GEREKLI | parametreli sorgu |
| `INotifyPropertyChanged` | DTO tanimi | - | her guncelleyen DTO'da |

Kontrol listesi: baglama sirasi `DataSource` -> kolonlar, SQL parametreli, kolonlar tasarimcida, layout anahtari benzersiz, iki bacakta `dotnet build` -> 0 hata DOĞRULAMA GEREKLI.

---

## §9 Kanit

| Kaynak dosya | Ne alindi |
|---|---|
| `knowledge\winforms\data-binding-basics.md` (kaynak vault) | parca tablosu, yon/guncelleme tablosu, TFM tablosu, C# 7.3/14 karsilastirmasi, ihlal belirtileri |
| `.rules\data-access-rules.md` | parametreli sorgu, EF Core yasagi |
| `.rules\coding-standards.md` | DTO/katman yazim kurali |

> Kanitsiz iddia icin DOĞRULAMA GEREKLI yazilir; bu dosyada kaynak disi bilgi yoktur.
