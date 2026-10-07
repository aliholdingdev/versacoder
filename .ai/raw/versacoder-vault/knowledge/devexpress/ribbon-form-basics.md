---
title: "RibbonForm Temelleri - VersaCoder"
type: knowledge
category: devexpress
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "DevExpress Universal 2026 (v26.1) ana pencere kalibi: RibbonForm tabani, RibbonControl + RibbonStatusBar eslesmesi ve Office2010 stili satiri"
---

# RibbonForm Temelleri

**Zorunlu Baglantilar:** [vault index](../../index.md) · [devexpress-rules](../../.rules/devexpress-rules.md) · [ADR-004](../../.decisions/accepted/ADR-004-devexpress-universal-v261-office2010.md) · [bilgi index](../index.md) · [ribbon-control-office2010](ribbon-control-office2010.md) · [ribbon-pages-groups-items](ribbon-pages-groups-items.md)

> **Tek cumlelik sorumluluk:** *VersaCoder ana penceresi hangi taban siniftan turer ve ribbon hangi tek satirla kilitlenir* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Ana pencere `RibbonForm`'dan turer (kural DX-04), ribbon stili **yalniz** `RibbonControlStyle.Office2010`'dur (kural DX-03). Ribbon, formun ustune `Dock = Top` ile yerlesir; `RibbonStatusBar` altta `Ribbon` baglantisiyla eslesir. Kod dosyasi (`.cs`) yalnizca olay yonlendiricisi tutar, tum yapilandirma Designer'da uretilir (DX-10). Formun her zaman **uc dosyasi** birlikte olur: `.cs` + `.Designer.cs` + `.resx` (DX-05).

---

## §1 Temel kullanim

```csharp
using DevExpress.XtraBars.Ribbon;

public partial class MainForm : RibbonForm
{
    public MainForm()
    {
        InitializeComponent();
        ribbonControl1.RibbonStyle = RibbonControlStyle.Office2010;
        ribbonStatusBar1.Ribbon = ribbonControl1;   // durum cubugu ribbon'a baglanir
    }
}
```

```
MainForm : RibbonForm
  +----------------------------------------------+
  | RibbonControl      Dock=Top    RibbonStyle   |
  |   +--------------------------------------+  |   = Office2010 (DX-03)
  |   | QAT | sayfa basliklari | uygulama butonu  |
  |   +--------------------------------------+  |
  |   +--------------------------------------+  |
  |   | RibbonPage -> RibbonPageGroup -> item  |  |
  |   +--------------------------------------+  |
  +----------------------------------------------+
  | govde (NavigationFrame / XtraTabControl ...) |
  +----------------------------------------------+
  | RibbonStatusBar   Ribbon = ribbonControl1    |
  +----------------------------------------------+
```

---

## §2 Ozellik tablosu

| Nesne | Tur | Ne is yapar | Designer notu |
|---|---|---|---|
| `MainForm` | `RibbonForm` | Ana pencere; QAT basliga gomulur, iki baslik (uygulama + belge) tasinir | Formu **Ribbon Form** olarak ekle, uc dosya otomatik olur |
| `ribbonControl1` | `RibbonControl` | Sayfa/grup/oge sahibi, `RibbonStyle` burada | `RibbonStyle` = `Office2010` |
| `ribbonStatusBar1` | `RibbonStatusBar` | Alt durum cubugu; `Ribbon` ozelligi ile baglanir | `Ribbon` = `ribbonControl1` |
| `applicationMenu1` | `ApplicationMenu` | Uygulama butonunun menusu | [ribbon-application-menu](ribbon-application-menu.md) |
| `ribbonControl1.Manager` | `BarManager` | Oge kimlikleri (Id) uretimi | Elle `Id` yazma; `GetNewItemId` kullanilir |
| `controller1` | `BarAndDockingController` | Renk/font/look-and-feel ayari | Opsiyonel; `RibbonControl.Controller`'a atanir |

---

## §3 net48 ve net10.0-windows'da fark

| Konu | `net48` | `net10.0-windows` |
|---|---|---|
| Program baslangici | `Application.EnableVisualStyles()` + `SetCompatibleTextRenderingDefault(false)` | `ApplicationConfiguration.Initialize()` |
| Paket/versiyon | CPM (`Directory.Packages.props`) tek kaynak; `App.config` binding redirect **yalniz** 26.1 (DX-06) | CPM ayni; binding redirect **gerekmez** |
| Dil surumu | C# **7.3** (`var`/`=>` serbest, `using` dogrulanmali) | C# **14** (nullable acik olabilir) |
| Nullable | Proje genelinde kapaliysa `?` uyarisi vermez | `Nullable=enable` ise `Ribbon` null kontrolu ister |
| `#if` kullanimi | yalniz platform API farkinda | ayni; is mantigi #if altina ALINMAZ |

> **KURAL:** Iki bacakta da ayni `.Designer.cs` calisir; fark yalniz baslangic/kurulum katmanidir.

---

## §4 Yaygin hatalar

| # | Hata | Sonuc | Duzeltme |
|---|---|---|---|
| 1 | Ana pencere `XtraForm` | G3 / DX-04 ihlali, QAT baslik disinda kalir | `RibbonForm`'a cevir |
| 2 | `RibbonStyle = Office2013` vb. | G3 ihlali (build RED sayilir) | `RibbonControlStyle.Office2010` |
| 3 | Kodda `Pages.Add` dongusu ile sayfa kurmak | Designer her acilista yapiyi siler | Yapıyı Designer'da kur (DX-07) |
| 4 | `.cs` tek basina form olusturmak | Form Designer'da acilmaz | Uclu dosyayi tamamla (DX-05) |
| 5 | `barManager1` + `ribbonControl1` ayni formda | Iki yonetici carpisir, olaylar calismaz | Tek yonetici: RibbonControl |

---

## §5 Kanit ve dogrulama

| Kalem | Durum |
|---|---|
| API imzalari (`RibbonStyle`, `RibbonStatusBar.Ribbon`) | v26.1 dokumantasyonu ile dogrulandi |
| `dotnet build` (net48 + net10.0-windows) | Bu batch'te alinmadi → `NEEDS_VALIDATION` |
| Kural uyumu | DX-03 / DX-04 / DX-05 satirlari karsilanir |
| Ilgili karar | [ADR-004](../../.decisions/accepted/ADR-004-devexpress-universal-v261-office2010.md) |

**Kardesler:** [ribbon-control-office2010](ribbon-control-office2010.md) · [ribbon-pages-groups-items](ribbon-pages-groups-items.md) · [ribbon-qat-quick-access](ribbon-qat-quick-access.md) · [splashscreen-waitform](splashscreen-waitform.md)
