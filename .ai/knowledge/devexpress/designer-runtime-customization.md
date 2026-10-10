---
title: "Designer Dosyasi ve Runtime Ozellestirme - VersaCoder"
type: knowledge
category: devexpress
status: active
version: 1.0.0
updated: 2026-09-29
authority: primary
scope: "DevExpress Form uclusu, .Designer.cs anatomiisi, nerede ne yazilir ve runtime ozellestirme siniri - iki bacak farki"
tags: [knowledge]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# Designer Dosyasi ve Runtime Ozellestirme (v26.1)

**Zorunlu Baglantilar:** [[knowledge/index|bilgi indeksi]] · [[index|vault index]] · [[.rules/coding-standards|kodlama standartlari]] · [[.decisions/accepted/adr-028-layered-baseline-9-projects|ADR-028 layered baseline]] · [[knowledge/devexpress/accessibility-keyboard-nav|accessibility-keyboard-nav]] · [[knowledge/devexpress/docking-saved-layout-xml|docking-saved-layout-xml]]
**Küme + Hub:** [[knowledge/devexpress/index|devexpress index]] · [[claude|CLAUDE]] · [[definitions/agents|AGENTS]] · [[.decisions/brain|brain]] · [[definitions/glossary|glossary]] · [[governance/keys|keys]]
**Kardeşler (devexpress · 5/35):** [[knowledge/devexpress/accordion-navigation|accordion-navigation]] · [[knowledge/devexpress/alertcontrol-notifications|alertcontrol-notifications]] · [[knowledge/devexpress/bar-status-items|bar-status-items]] · [[knowledge/devexpress/chartcontrol-basics|chartcontrol-basics]] · [[knowledge/devexpress/checkedcomboedit-values|checkedcomboedit-values]] · [[knowledge/devexpress/comboboxedit-autocomplete|comboboxedit-autocomplete]] · [[knowledge/devexpress/datasource-binding-dapper|datasource-binding-dapper]] · [[knowledge/devexpress/dateedit-timeedit|dateedit-timeedit]] · [[knowledge/devexpress/docklayout-persistence|docklayout-persistence]] · [[knowledge/devexpress/grid-conditional-format-rules|grid-conditional-format-rules]] · [[knowledge/devexpress/grid-editing-modes|grid-editing-modes]] · [[knowledge/devexpress/grid-export-excel-pdf|grid-export-excel-pdf]] · [[knowledge/devexpress/grid-filtering-sorting|grid-filtering-sorting]] · [[knowledge/devexpress/grid-grouping-summaries|grid-grouping-summaries]] · [[knowledge/devexpress/grid-paging-virtual-mode|grid-paging-virtual-mode]] · [[knowledge/devexpress/grid-search-panel-find|grid-search-panel-find]] · [[knowledge/devexpress/gridcontrol-basics|gridcontrol-basics]] · [[knowledge/devexpress/gridview-columns-format|gridview-columns-format]] · [[knowledge/devexpress/layoutcontrol-grouping|layoutcontrol-grouping]] · [[knowledge/devexpress/lookupedit-cascading|lookUpEdit-cascading]] · [[knowledge/devexpress/mvvm-pattern-devexpress|mvvm-pattern-devexpress]] · [[knowledge/devexpress/nuget-references-cpm|nuget-references-cpm]] · [[knowledge/devexpress/performance-grid-optimization|performance-grid-optimization]] · [[knowledge/devexpress/ribbon-control-office2010|ribbon-control-office2010]] · [[knowledge/devexpress/ribbon-form-basics|ribbon-form-basics]] · [[knowledge/devexpress/ribbon-pages-groups-items|ribbon-pages-groups-items]] · [[knowledge/devexpress/skin-customization|skin-customization]] · [[knowledge/devexpress/splashscreen-waitform|splashscreen-waitform]] · [[knowledge/devexpress/tabcontrol-multipage|tabcontrol-multipage]] · [[knowledge/devexpress/theme-manager-office2010|theme-manager-office2010]]

> **Tek cumlelik sorumluluk:** *Form uclusunde hangi satir nerede yasar, runtime'da ne ozellestirilebilir* sorusunun tek cevabidir.

## §0 Ozet (Answer First)

Her DevExpress formu **uc dosyadan** olusur: `.cs` (olay/akis), `.Designer.cs` (`InitializeComponent`), `.resx` (metin/DPI kaynagi). Yapilsal her degisiklik **tasarımcıdan** yapilir; `.Designer.cs` ye elle yazilan satir yeniden uretimde silinir, is mantigi oraya konursa kaybolur. Runtime ozellestirme **kod dosyasinda** ve **kontrol eklemek/ozellik atamak** duzeyinde kalir; kalici duzen `.resx` / layout XML'e gider. Ribbon stili kodda degistirilemez: **yalniz** `RibbonControlStyle.Office2010`.

## §1 Form uclusu anatomiisi

```
SessionForm.cs              | olaylar, akis, servis cagrisi      | elde yazilir
SessionForm.Designer.cs     | InitializeComponent + Suspend/     | tasarimcida uretilir
                            | Resume + BeginInit/EndInit + Name  | elle edit YASAK
SessionForm.resx            | metin, ikon, DPI kaynagi           | uretilir/duzenlenir
        |
        +--> ayni klasor + ayni sinif adi (partial)
```

| Bolum | Ne var | Elle edit |
|---|---|---|
| `InitializeComponent()` | kontrol yaratimi, ozellik atama, `resources.ApplyResources` | **YASAK** |
| `SuspendLayout/ResumeLayout` cifti | layout kilidi | **YASAK** |
| `BeginInit/EndInit` cifti | grid/kolon nesneleri | **YASAK** |
| Olay baglilari (`+= ...`) | handler kaydi | Olaylar penceresinden |
| `FormName.cs` handler govdesi | is akisi | **serbest** (ince kalmali) |

## §2 Kod (yasal runtime kodu)

```csharp
public partial class SessionForm : RibbonForm
{
    private readonly ISessionService _sessions;  // arayuz uzerinden

    public SessionForm(ISessionService sessions)
    {
        _sessions = sessions;
        InitializeComponent();                   // Designer'in urettigi tek cagri
    }

    private void OnBarNewItemClick(object sender, ItemClickEventArgs e)
        => _sessions.Create();                   // ince handler
}
```

| Runtime ozellestirme | Yapilabilir mi | Not |
|---|---|---|
| Kodda `Controls.Add(yeniControl)` | evet | sahiplik Designer'a birakilir |
| Kodda `gridView1.Columns[k].Visible = ...` | evet | **dizilim** Designer'da, durum kodda |
| Kodda `ribbonControl1.RibbonStyle = ...` | **hayir** | sabit `Office2010` |
| `.Designer.cs` ye is mantigi | **hayir** | silinir/kaybolur |
| Elle `control.Dispose()` | **hayir** | `ObjectDisposedException` |

## §3 Ozellik tablosu (Designer'in urettigi alanlar)

| Alan | Tipik deger | Kim korur |
|---|---|---|
| `Name` | `"ribbonControl1"` | tasarimci |
| `TabIndex` / `TabStop` | sirali | tasarimci (erisilebilirlik) |
| `Dock` / `Anchor` | `DockStyle.Fill` | tasarimci (olcu) |
| `Resources.ApplyResources(c, "name")` | `.resx` anahtari | tasarimci |
| `AutoScaleMode` | `Dpi` / `Font` (Designer degeri) | **dokunulmaz** |

> **KURAL:** `.Designer.cs` diff'i yalniz tasarimci kaynakli olmalidir; PR'da elle yazilmis satir gorulurse **RED**.

## §4 Runtime ozellestirme akisi

| Adim | Nerede | Kanit |
|---|---|---|
| Yeni kontrol/kolon ekleme | **Designer** | uc dosya birlikte degisti |
| Durum degistirme (`Visible`, `Caption`) | `.Form.cs` (handler/servis) | ince handler |
| Kalici duzen | `.resx` / layout XML | bkz. [[knowledge/devexpress/docking-saved-layout-xml|docking-saved-layout-xml]] |
| Metin cevirisi | `.resx` kultur dosyasi | `.resx` tek kaynak |
| Erisilebilirlik adlari | Designer + kod testi | bkz. [[knowledge/devexpress/accessibility-keyboard-nav|accessibility-keyboard-nav]] |

## §5 Iki bacak (net48 / net10.0-windows)

| Konu | net48 | net10.0-windows |
|---|---|---|
| `.Designer.cs` | **ayni dosya** iki bacakta da derlenir | ayni |
| Uretilen dil sozdizimi | **C# 7.3** ile sinirli tut (LangVersion 7.3) | C# 14 yutabilir ama ortak dosya 7.3 kalir |
| `#if` | yalniz platform farki (orn. DPI/interop) | ayni |
| Designer destegi | Visual Studio WinForms designer | surume bagli -> **DOĞRULAMA GEREKLI** |

## §6 Yaygin hatalar

| # | Hata | Sonuc | Duzeltme |
|---|---|---|---|
| 1 | `.Designer.cs` e elle satir yazmak | yeniden uretimde silinir | Designer'dan degistir |
| 2 | Is mantigini `InitializeComponent` icine koymak | kaybolur | `.Form.cs` handler |
| 3 | Kodda `Dispose()` cagirmak | `ObjectDisposedException` | Designer'a birak |
| 4 | Ribbon stilini runtime degistirmek | kural kirilimi | `Office2010` sabit |
| 5 | `AutoScaleMode` elle degistirmek | DPI bozulmasi | dokunulmaz |

## §7 Kodlandirma notu

- Handler govdesi ince kalir; is kurali servise gider.
- `InitializeComponent()` ctor'da bir kez cagrilir.
- `.resx` metni kaynak kullanilir, literal tekrar edilmez.
- Elle eklenen kontrolun `Name`'i benzersizdir.
- `.Designer.cs` degisikligi her zaman tasarimci aracindan gelir.

## §8 Kardes dosyalar

| Konu | Dosya |
|---|---|
| Duzeni XML'e kaydetme | [[knowledge/devexpress/docking-saved-layout-xml|docking-saved-layout-xml]] |
| Panel kurulumu | [[knowledge/devexpress/dockmanager-panels|dockmanager-panels]] |
| Erisilebilirlik alanlari | [[knowledge/devexpress/accessibility-keyboard-nav|accessibility-keyboard-nav]] |
| Alan dogrulama | [[knowledge/devexpress/dx-validation-provider|dx-validation-provider]] |

## §9 Kanit

| Kaynak dosya | Alinan kanit | Durum |
|---|---|---|
| `designer-runtime-customization.md` | uclus anatomiisi, `InitializeComponent` yasak tablosu, yapilabilir/yapilamaz tablosu, akis tablosu, iki bacak | dogrulandi |
| Designer destegi (VS surumune bagli) | kaynakta `NEEDS_VALIDATION` | **DOĞRULAMA GEREKLI** |
| `dotnet build` | bu uretimde alinmadi | **DOĞRULAMA GEREKLI** |
