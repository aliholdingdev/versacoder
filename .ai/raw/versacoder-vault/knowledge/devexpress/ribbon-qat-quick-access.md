---
title: "Quick Access Toolbar (QAT) - VersaCoder"
type: knowledge
category: devexpress
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "Ribbon hizli erisim cubugunun (QAT) konumu, icerigi, RibbonForm basligina gomulmesi ve kullanici ozellestirmesi"
---

# Quick Access Toolbar (QAT)

**Zorunlu Baglantilar:** [vault index](../../index.md) · [devexpress-rules](../../.rules/devexpress-rules.md) · [ADR-004](../../.decisions/accepted/ADR-004-devexpress-universal-v261-office2010.md) · [bilgi index](../index.md) · [ribbon-form-basics](ribbon-form-basics.md) · [ribbon-control-office2010](ribbon-control-office2010.md)

> **Tek cumlelik sorumluluk:** *En cok kullanilan komutlarin ribbon disinda nerede durdugu ve nasil eklendigi* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

QAT, ribbon'un ustunde (veya `RibbonForm`'da baslik barinda) duran kisa komut cubugudur; konum ve gorunurluk **tek** ozellikten yonetilir: `RibbonControl.ToolbarLocation`. Icerik `RibbonControl.Toolbar` / `QuickToolbarItemLinks` ile erisilen baglantilarla kurulur; `ShowItemCaptionsInQAT` acildiginda oge yazilari da gorunur. `RibbonForm` kullanildiginda QAT **basliga gomer**; `OfficeUniversal` stilinde tamamen kaybolur - biz zaten yalnizca `Office2010` kullandigimiz icin bu durumla karsilasmayiz. Kullanici sag tik ile ogeyi QAT'e ekleyip cikarabilir; `AllowCustomization` ile bu davranis kontrol edilir.

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
        ribbonControl1.ShowItemCaptionsInQAT = true;  // QAT'te yazi gorunsun
        ribbonControl1.AllowCustomization = true;     // kullanici QAT'i duzenleyebilsin
    }
}
```

```
RibbonForm basligi
+--------------------------------------------------+
| [QAT: kaydet | geri al]   Uygulama - Belge  - X  |
+--------------------------------------------------+
| (File) | Sayfa 1 | Sayfa 2 | ...                 |
+--------------------------------------------------+
```

---

## §2 Ozellik tablosu

| Ozellik | Tur | Ne is yapar | Not |
|---|---|---|---|
| `ToolbarLocation` | enum | QAT konumu **ve** gorunurlugu | Enum uyesi adlari `NEEDS_VALIDATION` |
| `Toolbar` | nesne | QAT'in kendisine erisim (icindekileri duzenle) | Tasarimcida ekle/kaldir tercih edilir |
| `QuickToolbarItemLinks` | koleksiyon | QAT'te gorunen baglantilar | Mevcut `BarItem` linkleridir, yeni oge degil |
| `ShowItemCaptionsInQAT` | `bool` | QAT oge yaninda yaziyi gosterir | Dar ekranda tiklanabilirlik artar |
| `AllowCustomization` | `bool` | Kullanici QAT'i duzenleyebilir | Kapaliysa sag tik menusu kisitli |
| `RibbonForm` entegrasyonu | — | QAT baslik barina gomulur | `XtraForm` kullanilmissa ayni davranis yok |
| `OfficeUniversal` stilinde | — | QAT **gosterilmez** | Bizde stil `Office2010` → gecersiz |

---

## §3 net48 ve net10.0-windows'da fark

| Konu | `net48` | `net10.0-windows` |
|---|---|---|
| QAT davranisi | Ayni (DevExpress derleme zamani ayni) | Ayni |
| Ozellik kaydi | `AutoSaveLayoutToXml` dosya yolu `Application.StartupPath` | `AppContext.BaseDirectory` |
| DPI | Kucuk ikonlar 16x16; manifest DPI ayari | Otomatik olcek; `AutoScaleMode` korunur (DX-14) |
| Nullable | QAT link koleksiyonu null kontrolu gerektirmez | `Nullable=enable` ise `Toolbar?` uyarisi olabilir |
| `#if` | Yalniz dosya yolu / DPI API farkinda | Ayni; QAT kodu #if altina ALINMAZ |

---

## §4 Yaygin hatalar

| # | Hata | Sonuc | Duzeltme |
|---|---|---|---|
| 1 | QAT'i kodda `new` ile olusturmak | Ribbon ile carpisma | `RibbonControl.Toolbar`/Designer |
| 2 | Ayni komutu hem sayfa hem QAT'te ayri oge yapmak | Iki farkli olay handler'i | Tek `BarItem`, iki `ItemLinks` |
| 3 | `ToolbarLocation`'i ayarlayip gizli kalmasini beklemek | Enum hatasi / gorunmeme | Degerleri designer'dan sec |
| 4 | QAT'e is mantigi yazmak | Test edilemez UI | Handler → `Application` katmani |
| 5 | `OfficeUniversal` stil denemesi | QAT kaybolur + G3 ihlali | `Office2010` (DX-03) |

---

## §5 Kanit ve dogrulama

| Kalem | Durum |
|---|---|
| `ToolbarLocation`, `Toolbar`, `QuickToolbarItemLinks`, `ShowItemCaptionsInQAT`, `AllowCustomization` | v26.1 ozellik listesinde dogrulandi |
| `ToolbarLocation` enum uyesi adlari | `NEEDS_VALIDATION` (property grid'ten dogrula) |
| `dotnet build` | Bu batch'te alinmadi → `NEEDS_VALIDATION` |
| Kural | DX-03 (tek stil), DX-04 (RibbonForm) |

**Kardesler:** [ribbon-form-basics](ribbon-form-basics.md) · [ribbon-control-office2010](ribbon-control-office2010.md) · [ribbon-pages-groups-items](ribbon-pages-groups-items.md) · [ribbon-application-menu](ribbon-application-menu.md)
