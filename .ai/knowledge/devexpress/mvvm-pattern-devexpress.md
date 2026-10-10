---
title: "mvvm-pattern-devexpress - VersaCoder"
type: knowledge
category: devexpress
status: active
version: 1.0.0
updated: 2026-09-29
authority: primary
scope: "DevExpress v26.1 MVVM deseni: ViewModelBase + DelegateCommand + Messenger, ince handler siniri, DI siniri ve iki bacak farki"
tags: [knowledge]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# DevExpress MVVM Kalibi

**Zorunlu Baglantilar:** [[knowledge/index|vault indeks]] · [[.rules/devexpress-rules|devexpress-rules]] · [[.rules/solid-rules|solid-rules]] · [[knowledge/devexpress/dx-validation-provider|dx-validation-provider]] · [[knowledge/devexpress/gridcontrol-basics|gridcontrol-basics]]
**Küme + Hub:** [[knowledge/devexpress/index|devexpress index]] · [[index|index]] · [[claude|CLAUDE]] · [[definitions/agents|AGENTS]] · [[.decisions/brain|brain]] · [[definitions/glossary|glossary]] · [[governance/keys|keys]]
**Kardeşler (devexpress · 5/35):** [[knowledge/devexpress/accessibility-keyboard-nav|accessibility-keyboard-nav]] · [[knowledge/devexpress/accordion-navigation|accordion-navigation]] · [[knowledge/devexpress/alertcontrol-notifications|alertcontrol-notifications]] · [[knowledge/devexpress/bar-status-items|bar-status-items]] · [[knowledge/devexpress/chartcontrol-basics|chartcontrol-basics]] · [[knowledge/devexpress/checkedcomboedit-values|checkedcomboedit-values]] · [[knowledge/devexpress/comboboxedit-autocomplete|comboboxedit-autocomplete]] · [[knowledge/devexpress/datasource-binding-dapper|datasource-binding-dapper]] · [[knowledge/devexpress/dateedit-timeedit|dateedit-timeedit]] · [[knowledge/devexpress/docking-saved-layout-xml|docking-saved-layout-xml]] · [[knowledge/devexpress/docklayout-persistence|docklayout-persistence]] · [[knowledge/devexpress/dockmanager-panels|dockmanager-panels]] · [[knowledge/devexpress/grid-conditional-format-rules|grid-conditional-format-rules]] · [[knowledge/devexpress/grid-editing-modes|grid-editing-modes]] · [[knowledge/devexpress/grid-export-excel-pdf|grid-export-excel-pdf]] · [[knowledge/devexpress/grid-filtering-sorting|grid-filtering-sorting]] · [[knowledge/devexpress/grid-grouping-summaries|grid-grouping-summaries]] · [[knowledge/devexpress/grid-paging-virtual-mode|grid-paging-virtual-mode]] · [[knowledge/devexpress/grid-search-panel-find|grid-search-panel-find]] · [[knowledge/devexpress/gridview-columns-format|gridview-columns-format]] · [[knowledge/devexpress/layoutcontrol-grouping|layoutcontrol-grouping]] · [[knowledge/devexpress/lookupedit-cascading|lookUpEdit-cascading]] · [[knowledge/devexpress/performance-grid-optimization|performance-grid-optimization]] · [[knowledge/devexpress/ribbon-control-office2010|ribbon-control-office2010]] · [[knowledge/devexpress/ribbon-form-basics|ribbon-form-basics]] · [[knowledge/devexpress/ribbon-pages-groups-items|ribbon-pages-groups-items]] · [[knowledge/devexpress/skin-customization|skin-customization]] · [[knowledge/devexpress/splashscreen-waitform|splashscreen-waitform]] · [[knowledge/devexpress/tabcontrol-multipage|tabcontrol-multipage]] · [[knowledge/devexpress/theme-manager-office2010|theme-manager-office2010]]

> **Tek cumlelik sorumluluk:** *DevExpress MVVM'de View, ViewModel ve servis siniri nerede cizilir* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

MVVM uc katmandir: **View** (`.Designer.cs` + ince code-behind), **ViewModel** (duran veri + komut), **servis** (Dapper repository, dosya, ag). DevExpress bunun icin `ViewModelBase` (degisiklik bildirimi), `DelegateCommand` (komut) ve `Messenger` (ViewModel'ler arasi haberlesme) verir. Kodda kalan tek sey **binding ve komut cagrisi**; is servis katmanina gider. Handler'lar tek cagri kalir (DX-13), `.resx`/Designer'e MVVM kodu yazilmaz (DX-10).

```text
View (.Form.cs + .Designer.cs + .resx)
   |   binding / Command
   v
ViewModel (ViewModelBase)  --- Messenger ---> diger ViewModel
   |
   |   arayuz uzerinden (I*)
   v
Servis / Repository (ADO.NET + Dapper - EF Core YASAK)
```

---

## §1 Katman siniri

| Katman | Ne tasir | Ne TASIMAZ |
|---|---|---|
| View | binding, komut cagrisi, olay yonlendirici | SQL, is kurali, ag |
| ViewModel | durum, komut, dogrulama mesaji | `Control` referansi, `.resx` |
| Servis | sorgu (parametreli `@p1`), dosya | UI nesnesi, tema |

---

## §2 Anahtar API'ler

| API | Ne is yarar | Not |
|---|---|---|
| `ViewModelBase.GetValue<T>() / SetValue(...)` | ozellik degisikligi + `PropertyChanged` | depolama hazir |
| `DelegateCommand` / `DelegateCommand<T>` | `ICommand` sarici | handler **ince** kalmali |
| `Messenger.Default` | ViewModel'ler arasi mesaj | abonelik `Dispose`'da |
| `RaisePropertyChanged(name)` | elle bildirim (yol gerekirse) | `CallerMemberName` |
| `ISupportSplashScreen` | uzun is bekleme gostergesi | `RibbonForm` ile |

> **DOĞRULAMA GEREKLI:** `AsyncCommand`/hazir async komut sinifinin v26.1 tam adi derleme ile dogrulanir; dogrulanana kadar async is `Task` + `await` icinde, `async void` **yok**.

---

## §3 Kod (ViewModel + ince handler)

```csharp
using DevExpress.Mvvm;
using System.Windows.Input;

public class SessionViewModel : ViewModelBase
{
    public string SelectedName
    {
        get => GetValue<string>();          // ViewModelBase depolama
        set => SetValue(value);             // PropertyChanged yayini
    }

    public ICommand NewCommand => new DelegateCommand(OnNew);

    private void OnNew() => _sessions.Create(SelectedName);  // tek cagri (DX-13)
}
```

| | |
|---|---|
| **GORUNUMDE** | `gridControl1.DataBindings.Add(...)`, `button1.Command = vm.NewCommand` |
| **GORUNUMDE YOK** | SQL, `SqlConnection`, is karari veren `MessageBox` |
| **VIEWMODEL'DE YOK** | `Control`, `GridControl`, tema kodu |

---

## §4 Kurallar

| # | Kural | Kanit |
|---|---|---|
| 1 | Binding tek yonlu: View -> ViewModel; ViewModel View'i cagirmaz | kod incelemesi |
| 2 | `DelegateCommand` handler'i tek satir cagri (DX-13); akis serviste | kod incelemesi |
| 3 | Repository cagrisi arayuz uzerinden; EF Core **0** (G2) | grep |
| 4 | Messenger abonelikleri `Dispose`'da kaldirilir | kod incelemesi |
| 5 | MVVM kodu `.Designer.cs`'e girmez (DX-10) | dosya incelemesi |

---

## §5 Iki bacak (net48 / net10.0-windows)

| Konu | net48 | net10.0-windows |
|---|---|---|
| MVVM assembly | v26.1 DevExpress paketi (net48 bacagi) | v26.1 ayni paketin net10 bacagi |
| C# | **7.3** — record, `using var`, target-typed `new` **yasak** | C# 14 yazilabilir; **ortak** `.cs` dosyalari 7.3'te kalir |
| Binding | WinForms `DataBindings` | ayni |
| DI | el ile kurulum (`Program.cs`) | `Host`/`IServiceCollection` — `#if` yalniz burada |

> **YASAK:** EF Core, `v26.1` disi surum, `RibbonControlStyle.Office2010` disi ribbon (G2 / DX-02 / DX-03).

---

## §6 Kardes dosyalar

| Konu | Dosya |
|---|---|
| Form/Designer siniri | [[knowledge/devexpress/designer-runtime-customization|designer-runtime-customization]] |
| Dogrulama (View katmani) | [[knowledge/devexpress/dx-validation-provider|dx-validation-provider]] |
| Veri kaynagi kalibi (Dapper) | [[knowledge/devexpress/gridcontrol-basics|gridcontrol-basics]] |
| Paket/CPM | [[knowledge/devexpress/nuget-references-cpm|nuget-references-cpm]] |

---

## §9 Kanit

| Kaynak dosya | Ne alindi |
|---|---|
| `knowledge\devexpress\mvvm-pattern-devexpress.md` (kaynak vault) | katman siniri, API tablosu, ViewModel ornegi, 5 kural, iki bacak |
| `.rules\devexpress-rules.md` | DX-13 (ince handler), DX-10 (Designer) |
| `.rules\solid-rules.md` | SOL-02 (servis siniri) |
| `.decisions\accepted\ADR-005-ado-net-dapper-no-ef-core.md` | EF Core yasagi |

> Kanitsiz iddia icin `DOĞRULAMA GEREKLI` yazilir; `AsyncCommand` tam sinif adi bu etiketle kalir.
