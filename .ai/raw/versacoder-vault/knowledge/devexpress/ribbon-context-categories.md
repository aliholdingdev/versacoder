---
title: "Ribbon Baglam Kategorileri (Contextual Tabs) - VersaCoder"
type: knowledge
category: devexpress
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "RibbonPageCategory ile kosullu/sekmeler: varsayilan kategori, renkli baslik, gorunurluk yonetimi ve sayfa ekleme"
---

# Ribbon Baglam Kategorileri (Contextual Tabs)

**Zorunlu Baglantilar:** [vault index](../../index.md) · [devexpress-rules](../../.rules/devexpress-rules.md) · [ADR-004](../../.decisions/accepted/ADR-004-devexpress-universal-v261-office2010.md) · [bilgi index](../index.md) · [ribbon-pages-groups-items](ribbon-pages-groups-items.md) · [ribbon-control-office2010](ribbon-control-office2010.md)

> **Tek cumlelik sorumluluk:** *"Secim yapilmadan gizli duran, kosul saglaninca renkli baslikla beliren sekmeler" nasil kurulur* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Ribbon'daki tum sayfalar bir **kategoriye** aittir: sayfalariniz varsayilan kategoride (`RibbonControl.DefaultPageCategory`) yasar, kosullu sekmeler ise ozel bir `RibbonPageCategory` icinde. Ozel kategorinin **basligi renkli gorunur** ve yalnizca `Visible = true` oldugunda sekmeleri ribbon'a eklenir. Ornek: grid'de bir hucre secildiginde "Secim" kategorisi belirir, secim kaldirilinca kaybolur. Kategori icine birden fazla sayfa konur; sayfanin kategorisini `RibbonPage.Category` ile okursunuz.

---

## §1 Temel kullanim

```csharp
using System.Drawing;
using DevExpress.XtraBars.Ribbon;

RibbonPageCategory cat = new RibbonPageCategory("Secim", Color.Orange, false);
ribbonControl1.PageCategories.Add(cat);
cat.Pages.AddRange(new RibbonPage[] { pageBi, pageFormat });
cat.Visible = true;                       // kosul saglandi -> sekmeler gorunur

ribbonControl1.SelectedPage = cat.Pages[0];
```

```
Ribbon
  +-- DefaultPageCategory (baslik gorunmez)
  |      +-- RibbonPage "Ana Sayfa"
  |      +-- RibbonPage "Ayarlar"
  +-- RibbonPageCategory "Secim" (renkli baslik, Visible=false)
         +-- RibbonPage "Bi"        <-- kosul saglaninca belirir
         +-- RibbonPage "Biçim"
```

---

## §2 Ozellik tablosu

| Nesne / ozellik | Tur | Ne is yapar |
|---|---|---|
| `RibbonPageCategory(caption, color, ...)` | ctor | Renkli baslikli kategori; 3. parametre `NEEDS_VALIDATION` |
| `RibbonControl.PageCategories` | koleksiyon | Ozel kategoriler; `Add` / `Remove` |
| `RibbonControl.DefaultPageCategory` | nesne | Normal (baglam disi) sayfalarin sahibi; basligi gosterilmez |
| `RibbonPageCategory.Pages` | koleksiyon | Kategoriye ait sekmeler (`AddRange` ile toplu ekleme) |
| `RibbonPage.Category` | okuma | Sayfanin hangi kategoride oldugu |
| `RibbonPageCategory.Visible` | `bool` | Kategoriyi ve tum sekmelerini gizler/gosterir |
| `RibbonControl.PageCategoryAlignment` | enum | Kategoriyi sag/sol hizalama → `NEEDS_VALIDATION` |
| `SelectedPageChanging` | olay | Secimi iptal edebilir; kosul disi sekmeye gecisi engeller |

---

## §3 net48 ve net10.0-windows'da fark

| Konu | `net48` | `net10.0-windows` |
|---|---|---|
| Kategori kurulumu | Ayni API | Ayni API |
| `Color.Orange` | `System.Drawing.Color` (gomye referans) | `System.Drawing` ayni; `Primitives` paketi gerekebilir |
| C# surumu | 7.3: `AddRange(new RibbonPage[] { ... })` | 14: `[..]` koleksiyon ifadesi de kullanilabilir |
| Nullable | `cat.Pages[0]` null dondurmez | `Nullable=enable` uyarisa dikkat |
| `#if` | Yalniz `System.Drawing` farkinda | Ayni; gorunurluk mantigi #if altina ALINMAZ |

---

## §4 Yaygin hatalar

| # | Hata | Sonuc | Duzeltme |
|---|---|---|---|
| 1 | Sayfayi dogrudan `Pages`'e ekleyip kategori beklemek | Sekme her zaman acik | `PageCategories` + `Visible` |
| 2 | Kategoriyi her olayda `new` ile uretmek | Bellek sizintisi / tekrarli sekmeler | Tek nesne, sadece `Visible` degistir |
| 3 | `Visible=false` yapilmadan "kosullu" davranis | Sekmeler hep acik | Kategoriyi varsayilan **kapali** baslat |
| 4 | Kategori basligini koda yazmak | Yerellestirme/disarida kalmak | Caption Designer/resx |
| 5 | Gizli kategorideki sayfaya secim yapmak | Bos ribbon | Once `Visible=true`, sonra `SelectedPage` |

---

## §5 Kanit ve dogrulama

| Kalem | Durum |
|---|---|
| `RibbonPageCategory(...)` ctor, `PageCategories.Add`, `Pages.AddRange`, `Visible` | v26.1 WinForms ornegi ile dogrulandi |
| Ctor 3. parametrenin ani | `NEEDS_VALIDATION` ( IntelliSense ile adi dogrula) |
| `PageCategoryAlignment` WinForms varligi | `NEEDS_VALIDATION` |
| `dotnet build` | Bu batch'te alinmadi → `NEEDS_VALIDATION` |

**Kardesler:** [ribbon-pages-groups-items](ribbon-pages-groups-items.md) · [ribbon-form-basics](ribbon-form-basics.md) · [ribbon-control-office2010](ribbon-control-office2010.md) · [dockmanager-panels](dockmanager-panels.md)
