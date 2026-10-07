---
title: "Veri Baglama Temelleri - VersaCoder"
type: knowledge
category: winforms
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "WinForms data binding: Binding/BindingSource/CurrencyManager, INotifyPropertyChanged, DTM vsliste, hata gosterimi; net48 vs net10.0-windows farklari ve C# 7.3/14 yazim"
---

# Data Binding Temelleri

**Zorunlu Baglantilar:** [knowledge index](../index.md) · [CLAUDE.md](../../CLAUDE.md) · [validation-form-level](validation-form-level.md) · [data-access-rules](../../.rules/data-access-rules.md) · [ADR-005](../../.decisions/accepted/ADR-005-ado-net-dapper-no-ef-core.md)

> **Tek cumlelik sorumluluk:** *Bir control'e deger nasil baglanir, kim guncel kaydi bilir, iki yonlu guncelleme ne zaman patlar* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

WinForms'ta baglama uc parcadan olusur: **`Binding`** (control property <-> kaynak property), **`BindingSource`** (sira + filtre + koleksiyon uzerindeki tek muhatap), **`CurrencyManager`** (aktif satir, `Position`, `Current`). Kaynak **`INotifyPropertyChanged`** implemente etmiyorsa alan guncellemesi ekrana yansimaz; `DataSource`'a **yeni liste** vermek (`ResetBindings`) ile tek satir guncellemek (`ResetCurrentBinding`) ayri islerdir. Veri kaynagi bu vault'ta **ADO.NET + Dapper**'dir (ADR-005), baglama **ara katmanin DTO'su** uzerinden yapilir - `DataTable` yalnizca hizli liste icin.

---

## §1 Baglama parcalari

| Parca | Ne yapar | Tipik kullanim |
|---|---|---|
| `Binding` | tek property eslesmesi | `nameBox.DataBindings.Add("Text", src, "Name")` |
| `BindingSource` | sira + filtre + `AddNew`/`Remove` | `grid.BindingSource`, `Position` |
| `CurrencyManager` | aktif kayit | `bindingSource.CurrencyManager.Position` |
| `INotifyPropertyChanged` | alan degisimi ekrana yansir | DTO / view-model |
| `IList` / `DataTable` | koleksiyon kaynagi | liste, `DataTable` |
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
| Kaynak `INotifyPropertyChanged` yayiyor | alan → ekran **canli** | DTO'da `OnPropertyChanged` |
| Kaynak yalniz `IList` | yalnizca satir degisimi yansir | `ResetBindings()` |
| `DataSource` = yeni liste | sifirdan yukleme | `ResetBindings(bindingSourceReset...)` |
| Control `Text` bosaltildi | `FormattingEnabled=false` ise bosaltma yazilir | gerekliyse `FormattingEnabled=true` |
| Bir alan baska alani dogruluyor | `Validating` zinciri (bkz. [validation-form-level](validation-form-level.md)) | `AutoValidate` |
| Kaynakta SQL | **YASAK** (G2 / DA-03) | Dapper + `@p1` parametreli sorgu (ADR-005) |

> **KURAL:** Control'e **dogrudan** bagli DTO, **kaydetme icin kullanilmaz**; kaydetme `Application` katmanina gider ve **parameterized** SQL ile yapilir (`CommandTimeout` setli).

---

## §3 TFM farki (net48 vs net10.0-windows)

| Konu | `net48` | `net10.0-windows` |
|---|---|---|
| `Binding` / `BindingSource` / `CurrencyManager` | var | var - **ayni API** |
| `INotifyPropertyChanged` | var | var |
| `DataTable` / `DataView` | `System.Data` yerlesik | `System.Data.Common` + DataTable yerlesik (Windows Desktop) |
| Nullable anotasyonu | yok | `string?` / `T?` anotasyonlari |
| `System.ComponentModel.Annotations` | yerlesik | yerlesik (DataAnnotations dogrulama) |
| `IEnumerable` + `await foreach` | paket gerekir | `AsyncEnumerable` (10.0+) |

> **Sonuc:** data binding **API'si iki bacakta birebir aynidir**; fark yalnizca NRT anotasyonu ve asenkron koleksiyon yardimcilari. Kod `#if` **icermemeli**.

---

## §4 C# 7.3 (net48) vs C# 14

```csharp
// --- C# 7.3 (net48) ---
public class SessionDto : INotifyPropertyChanged
{
    private string name;
    public event PropertyChangedEventHandler PropertyChanged;

    public string Name
    {
        get { return name; }
        set
        {
            if (value == name) { return; }
            name = value;
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs("Name"));
            }
        }
    }
}
```

```csharp
// --- C# 14 (net10.0-windows) ---
public class SessionDto : INotifyPropertyChanged
{
    private string _name = string.Empty;

    public event PropertyChangedEventHandler? PropertyChanged;

    public string Name
    {
        get => _name;
        set
        {
            if (value == _name) { return; }
            _name = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Name)));
        }
    }
}
```

| Ozellik | C# 7.3 | C# 14 | Ortak kaynak |
|---|---|---|---|
| Property bildirimi | blok `get/set` | expression body (`=>`) | expression body 7.3'te de var (getter icin) |
| Event yayimi | `var h = Ev; if (h != null)` | `Ev?.Invoke(...)` | `?.` 6.0+ -> ortak kodda da calisir |
| `nameof` | var (6.0+) | ayni | **her iki bacakta ayni** |
| Nullable event | `PropertyChangedEventHandler` | `?` | 7.3'te `?` yok |

---

## §5 Ihlal belirtisi

| Ihlal | Belirti | Duzeltme |
|---|---|---|
| DTO `INotifyPropertyChanged` yok | alanim guncellenince ekranda eski deger | DTO'ya INPC ekle ya da `ResetBindings` |
| Her tus vurusunda tam liste reset | ekran titremesi, kayit konumu kaybi | yalniz satir guncellemesi / `Position` |
| Control'e dogrudan DB nesnesi | UI katmani veri erisimi biliyor (G2 riski) | DTO + `Application` katmani |
| `grid.DataSource = null` ile temizleme | abonelikler kopmaz | `Dispose` + `DataSource = null` sirasi |
| SQL'i baglama kodunda yazmak | string-concat SQL (DA-03) | Dapper parameterized (ADR-005) |

> **Iliskili:** dogrulama zinciri icin [validation-form-level](validation-form-level.md) · veri erisimi kurallari icin [data-access-rules](../../.rules/data-access-rules.md) · iptal ile birlikte kullanim icin [cancellation-in-ui](cancellation-in-ui.md).
