---
title: "ribbonform-basics - VersaCoder"
type: knowledge
category: winforms
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "RibbonForm + RibbonControl + RibbonControlStyle.Office2010 ile ana pencere kurulumu, form uclusu ve iki bacakta (net48 + net10.0-windows) ribbon farklari"
---

# ribbonform-basics — RibbonForm + RibbonControl (v26.1)

**Zorunlu Baglantilar:** [index](../index.md) · [devexpress-rules](../../.rules/devexpress-rules.md) · [ADR-004](../../.decisions/accepted/ADR-004-devexpress-universal-v261-office2010.md) · [coding-standards](../../.rules/coding-standards.md) · [form-navigation-patterns](form-navigation-patterns.md)

> **Tek cumlelik sorumluluk:** *Ana pencere RibbonForm olarak nasil kurulur, ribbon hangi stil ve hangi sirayla baglanir* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Ana pencere bu vault'ta **`RibbonForm`**'dur; ribbon stili **yalniz `RibbonControlStyle.Office2010`**'dur (G3 / DX-03 / DX-04). Baska stil ya da `XtraForm` turevi ana pencere **RED**dir. Ribbon **Designer'da** kurulur: `RibbonPage` -> `RibbonPageGroup` -> `BarItem`; kod yalniz `ItemClick` handler'ini isletir (DX-07). Form uclusu `XxxForm.cs` + `XxxForm.Designer.cs` + `XxxForm.resx` ayni adla zorunludur (DX-05). Sürüm **DevExpress Universal 2026 / v26.1** tek kaynaktir (ADR-004).

---

## §1 Iskelet ve Form Uclusu

| Parca | Gorev | Nerede yazilir |
|---|---|---|
| `RibbonForm` | ana pencere govdesi; ribbon'u icine gomur | `XxxForm.cs` (sinif tanimi) |
| `RibbonControl` | ribbon barinin kendisi | `XxxForm.Designer.cs` |
| `RibbonPage` | mantiksal sekme (orn. "Dosya", "Gorunum") | Designer |
| `RibbonPageGroup` | sekme icindeki bolum | Designer |
| `BarItem` / `BarButtonItem` | tiklanabilir eylem | Designer + olay baglayici |

Uc dosya ayni adla ve birlikte olmalidir; yalniz `.cs` olan form Designer'da acilmaz.

## §2 Zorunlu ribbon satiri

`.Designer.cs` icinde tek ve birebir bu deger yazilir:

```csharp
this.ribbonControl1.RibbonStyle =
    DevExpress.XtraBars.Ribbon.RibbonControlStyle.Office2010;
```

Kurulum sirasi (4 adim):

1. `RibbonForm` tureven sinif olustur -> form uclusu otomatik eklenir.
2. Form'a `RibbonControl` surukle, `Dock = Fill`.
3. `RibbonPage` -> `RibbonPageGroup` -> `BarButtonItem` ekle.
4. Her `ItemClick` icin ince bir handler bagla; is mantigini `Application` katmanindaki servise gonder (SOL-02).

```csharp
public partial class MainForm : RibbonForm
{
    public MainForm()
    {
        InitializeComponent();
        this.ribbonControl1.ApplicationMenu = this.ribbonPopupMenu1;
    }

    private void barBtnYeni_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
    {
        // Is mantigi code-behind'da yasamaz: yalniz yonlendirme.
        _shell.NewSession();
    }
}
```

> **YASAK:** `ribbonPage1.Items.Add(...)` gibi kod icinde ribbon kurmak (her acilista Designer'i siler, DX-07) · `Office2007` / `Office2010Silver` gibi baska stil (G3) · `.resx`'i elle duzenlemek (DX-05).

## §3 Iki bacak: net48 vs net10.0-windows

| Konu | net48 | net10.0-windows |
|---|---|---|
| C# dili | **7.3** - `using var`, `??=`, target-typed `new` yok | C# 14 (ayni kaynak dosya 7.3'te kalir) |
| DevExpress paketi | CPM `Directory.Packages.props` 26.1 + `App.config` binding redirect (`AutoGenerateBindingRedirects=true`) | ayni CPM satiri; redirect gerekmez |
| DPI baslangici | `app.manifest` ile acilmazsa sistem olcekleme devreye girer | `ApplicationConfiguration.Initialize()` icinde high-DPI varsayilan acik |
| Ribbon API (`DevExpress.XtraBars`) | v26.1 - ayni | v26.1 - ayni; bacak farki **yalniz** paket + DPI |
| Lisans / registration | `App.config` yonlendirmesi + ortam degiskeni | ortam degiskeni (`NEEDS_VALIDATION`: kesin yontem PART 5) |

`#if` yalnizca DPI/paket satirindaki bu farkta kullanilir; ribbon kurulum kodu bacaklar ortaktir (G6).

## §4 Sik yapilan hatalar

| # | Hata | Sonuc | Duzeltme |
|---|---|---|---|
| 1 | Ana pencere `XtraForm` | G3 RED | `RibbonForm`'a cevir |
| 2 | `RibbonStyle` baska deger | G3 RED | `Office2010` satirina don |
| 3 | `.resx` elle duzenleme | `ResourceManager` adi bozulur, form acilmaz | Designer'dan uret (DX-05) |
| 4 | Handler icinde SQL kurma | G2 RED | ADO.NET + Dapper, parametreli sorgu (ADR-005) |
| 5 | Paket surumunu `.csproj`'ye yazma | iki surum = binding conflict | CPM'e tasi (DX-01) |

## §5 Kardes Dosyalar

| Konu | Dosya |
|---|---|
| Form acma / kapama akisi | [form-navigation-patterns](form-navigation-patterns.md) |
| Ribbon'dan komut yurutme | [command-pattern-mvvm](command-pattern-mvvm.md) |
| `Program.cs` acilis sirasi | [startup-pipeline-ui](startup-pipeline-ui.md) |
