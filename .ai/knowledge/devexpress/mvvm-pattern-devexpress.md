---
title: "mvvm-pattern-devexpress - VersaCoder"
type: knowledge
category: devexpress
status: active
version: 1.0.0
updated: 2026-09-29
authority: SSOT
scope: "DevExpress v26.1 MVVM deseni: ViewModelBase + DelegateCommand + Messenger, ince handler siniri, DI siniri ve iki bacak farki"
---

# DevExpress MVVM Kalibi

**Zorunlu Baglantilar:** [vault indeks](../index.md) · [devexpress-rules](../../.rules/devexpress-rules.md) · [solid-rules](../../.rules/solid-rules.md) · [dx-validation-provider](dx-validation-provider.md) · [gridcontrol-basics](gridcontrol-basics.md)
**Küme + Hub:** [devexpress index](index.md) · [index](../../index.md) · [CLAUDE](../../CLAUDE.md) · [AGENTS](../../AGENTS.md) · [brain](../../brain.md) · [glossary](../../glossary.md) · [keys](../../keys.md)
**Kardeşler (devexpress · 5/35):** [accessibility-keyboard-nav](accessibility-keyboard-nav.md) · [accordion-navigation](accordion-navigation.md) · [alertcontrol-notifications](alertcontrol-notifications.md) · [bar-status-items](bar-status-items.md) · [chartcontrol-basics](chartcontrol-basics.md) · [checkedcomboedit-values](checkedcomboedit-values.md) · [comboboxedit-autocomplete](comboboxedit-autocomplete.md) · [datasource-binding-dapper](datasource-binding-dapper.md) · [dateedit-timeedit](dateedit-timeedit.md) · [docking-saved-layout-xml](docking-saved-layout-xml.md) · [docklayout-persistence](docklayout-persistence.md) · [dockmanager-panels](dockmanager-panels.md) · [grid-conditional-format-rules](grid-conditional-format-rules.md) · [grid-editing-modes](grid-editing-modes.md) · [grid-export-excel-pdf](grid-export-excel-pdf.md) · [grid-filtering-sorting](grid-filtering-sorting.md) · [grid-grouping-summaries](grid-grouping-summaries.md) · [grid-paging-virtual-mode](grid-paging-virtual-mode.md) · [grid-search-panel-find](grid-search-panel-find.md) · [gridview-columns-format](gridview-columns-format.md) · [layoutcontrol-grouping](layoutcontrol-grouping.md) · [lookUpEdit-cascading](lookUpEdit-cascading.md) · [performance-grid-optimization](performance-grid-optimization.md) · [ribbon-control-office2010](ribbon-control-office2010.md) · [ribbon-form-basics](ribbon-form-basics.md) · [ribbon-pages-groups-items](ribbon-pages-groups-items.md) · [skin-customization](skin-customization.md) · [splashscreen-waitform](splashscreen-waitform.md) · [tabcontrol-multipage](tabcontrol-multipage.md) · [theme-manager-office2010](theme-manager-office2010.md)

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
| Form/Designer siniri | [designer-runtime-customization](designer-runtime-customization.md) |
| Dogrulama (View katmani) | [dx-validation-provider](dx-validation-provider.md) |
| Veri kaynagi kalibi (Dapper) | [gridcontrol-basics](gridcontrol-basics.md) |
| Paket/CPM | [nuget-references-cpm](nuget-references-cpm.md) |

---

## §9 Kanit

| Kaynak dosya | Ne alindi |
|---|---|
| `knowledge\devexpress\mvvm-pattern-devexpress.md` (kaynak vault) | katman siniri, API tablosu, ViewModel ornegi, 5 kural, iki bacak |
| `.rules\devexpress-rules.md` | DX-13 (ince handler), DX-10 (Designer) |
| `.rules\solid-rules.md` | SOL-02 (servis siniri) |
| `.decisions\accepted\ADR-005-ado-net-dapper-no-ef-core.md` | EF Core yasagi |

> Kanitsiz iddia icin `DOĞRULAMA GEREKLI` yazilir; `AsyncCommand` tam sinif adi bu etiketle kalir.
