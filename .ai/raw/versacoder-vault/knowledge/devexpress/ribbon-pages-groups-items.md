---
title: "Ribbon Sayfa, Grup ve Oge Hiyerarsisi - VersaCoder"
type: knowledge
category: devexpress
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "RibbonPage -> RibbonPageGroup -> BarItem zinciri, oge kimligi (Id) ve tek elden ItemClick olayi yonlendirmesi"
---

# Ribbon Sayfa, Grup ve Oge Hiyerarsisi

**Zorunlu Baglantilar:** [vault index](../../index.md) · [devexpress-rules](../../.rules/devexpress-rules.md) · [ADR-004](../../.decisions/accepted/ADR-004-devexpress-universal-v261-office2010.md) · [bilgi index](../index.md) · [ribbon-form-basics](ribbon-form-basics.md) · [ribbon-context-categories](ribbon-context-categories.md)

> **Tek cumlelik sorumluluk:** *Ribbon'da bir komutun nereye ait oldugu ve olayin nereden yakalanacagi* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Zincir uc katmandir: `RibbonControl.Pages` (sekmeler) → `RibbonPage.Groups` (bolumler) → `RibbonPageGroup.ItemLinks` (butonlar). Butonlar aslinda `RibbonControl.Items` koleksiyonunda yasar; grup yalnizca onlara **link** ekler, ayni oge iki grupta gorunebilir. Her `BarItem.Id` benzersiz olmalidir (serilestirme bozulur). Olay yonlendirmesi **tek yerden**: `RibbonControl.ItemClick` handler'i kalin kalmaz, is mantigi ayri sinifa gonderir (DX-07 / DX-13).

---

## §1 Temel kullanim

```csharp
using DevExpress.XtraBars;
using DevExpress.XtraBars.Ribbon;

RibbonPage page = new RibbonPage("Oturumlar");
page.Groups.Add(new RibbonPageGroup("Yeni") { AllowTextClipping = false });
ribbonControl1.Pages.Add(page);

ribbonControl1.SelectedPage = ribbonControl1.Pages["Oturumlar"];
ribbonControl1.ItemClick += (s, e) => OnRibbonItemClick(e.Item);   // tek giris noktasi
```

```
RibbonControl
  +-- Pages ---------------- RibbonPage "Oturumlar"
  |      +-- Groups -------- RibbonPageGroup "Yeni"
  |             +-- ItemLinks -- BarButtonItem bsiNewSession (Id: benzersiz)
  |                             BarButtonItem bsiOpenSession
  +-- Items ---------------- tum BarItem'larin gercek sahibi (Id burada)
  +-- ItemClick ------------- TEK handler -> uygulama katmanina cagri
```

---

## §2 Ozellik tablosu

| Nesne / ozellik | Tur | Ne is yapar |
|---|---|---|
| `RibbonControl.Pages` | koleksiyon | Sekmeler; `rc.Pages.Add(page)` |
| `RibbonPage("ad")` | ctor | Yeni sayfa; baslik ctor'da verilir |
| `RibbonPage.Groups` | koleksiyon | Sayfa icindeki bolumler |
| `RibbonPageGroup("ad") { AllowTextClipping = false }` | ctor + property | Bolum; basligi kirpmadan goster |
| `RibbonPage.Visible` | `bool` | Sayfayi gizler (kullanici gorunurlugu) |
| `RibbonPageGroup.Visible` | `bool` | Bolumu gizler |
| `BarItem.Visibility` | `BarItemVisibility` | `Never` ile oge tamamen gizlenir |
| `BarItem.RibbonStyle` | `RibbonItemStyles` | Buyuk (Large) / kucuk gosterim |
| `BarItem.Id` | `string` | Benzersiz olmali; `Manager.GetNewItemId()` |
| `RibbonControl.SelectedPage` | `RibbonPage` | Aktif sayfa; `Pages["ad"]` ile indekslenir |
| `SelectedPageChanging` | olay | Secimden once iptal edilebilir |
| `AutoHideEmptyItems` / `AutoSizeItems` | `bool` | Bos otemizligi / ayni genislik |

---

## §3 net48 ve net10.0-windows'da fark

| Konu | `net48` | `net10.0-windows` |
|---|---|---|
| Lambda handler (`(s, e) => ...`) | C# 7.3 ile serbest | C# 14 ile serbest |
| `e.Item` tipi | `ItemClickEventArgs.Item` (`BarItem`) | Ayni |
| Olay `+=` satiri | Designer'da baglanir; kodda tekrar **YASAK** (DX-10) | Ayni |
| Nullable | `e.Item` null kontrolu genelde gereksiz | `Nullable=enable` ise `e.Item?` uyarisi olabilir |
| Derleme | C# 7.3 sinirlari (yerel fonksiyon yok, `??=` yok) | C# 14; ayni API, daha kisa yazim |

> **KURAL:** Oge kurulumu Designer'da; kodda yalnizca `ItemClick` → handler. Kodda `page.Groups.Add` tekrari Designer'i siler (DX-07).

---

## §4 Yaygin hatalar

| # | Hata | Sonuc | Duzeltme |
|---|---|---|---|
| 1 | Ayni `BarItem`'i iki `Items` eklemek | Id cakismasi, kayip oge | Tek eklem, coklu `ItemLinks` |
| 2 | Her buton icin ayri handler (30+ `+=`) | God form (SOL-01) | Tek `RibbonControl.ItemClick` |
| 3 | `Id`'yi elle yazmak | Kaydet/yukle bozulur | `Manager.GetNewItemId()` |
| 4 | Sayfayi kodla `Pages.Add` ile uretmek | Designer aciliminda silinir | Designer |
| 5 | `RibbonPageGroup`'u tam genislik zorlamak | Dar pencerede kirilan baslik | `AllowTextClipping = false` |

---

## §5 Kanit ve dogrulama

| Kalem | Durum |
|---|---|
| API ornekleri (`new RibbonPage("...")`, `SelectedPage = Pages["..."]`, `ItemClick`) | v26.1 dokumantasyonu ile dogrulandi |
| `RibbonItemStyles` enum degerleri (Large/Small) | `NEEDS_VALIDATION` (projede designer'dan dogrula) |
| `dotnet build` | Bu batch'te alinmadi → `NEEDS_VALIDATION` |
| Kural | DX-07 (Page → Group → Item), DX-13 (ince handler) |

**Kardesler:** [ribbon-form-basics](ribbon-form-basics.md) · [ribbon-control-office2010](ribbon-control-office2010.md) · [ribbon-context-categories](ribbon-context-categories.md) · [ribbon-qat-quick-access](ribbon-qat-quick-access.md)
