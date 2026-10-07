---
title: "grid-search-panel-find - VersaCoder"
type: knowledge
category: devexpress
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "GridControl arama paneli (Search Panel), OptionsFind ayarlari, kolon bazli filtre popup'i ve buyuk veri icin DB aramasi (v26.1)"
---

# grid-search-panel-find — Arama paneli ve find (v26.1)

**Zorunlu Baglantilar:** [index](../index.md) · [devexpress-rules](../../.rules/devexpress-rules.md) · [ADR-004](../../.decisions/accepted/ADR-004-devexpress-universal-v261-office2010.md) · [grid-filtering-sorting](grid-filtering-sorting.md) · [gridcontrol-basics](gridcontrol-basics.md)

> **Tek cumlelik sorumluluk:** *Arama paneli neyi arar, kolon filtresi neyi arar ve hangisini veritabanina birakmak gerekir* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Grid'de **iki ayrı arama** vardır: üstteki **Search Panel** (Ctrl+F, `OptionsFind` ile yönetilir, *tüm görünür sütunlarda* metin arar) ve sütun başlığındaki **Filter Popup** (tek sütunda kriter). İkisi de istemci tarafında çalışır; veri büyürse asıl arama Dapper sorgusuna (`WHERE ... LIKE @p1`) taşınmalı. Arama panelini aç/kapa tek çağrıdır: `gridControl1.ShowFindPanel()` / `HideFindPanel()`. Sürüm **yalnız v26.1** (ADR-004).

---

## §1 Secim Tablosu

| Arama | Nerede | Kapsam | Kac satir tarar |
|---|---|---|---|
| Search Panel (Ctrl+F) | grid üstü | görünür tüm sütunlar | ekrandaki `DataTable` |
| Filter Popup | sütun başlığı | tek sütun | ekrandaki `DataTable` |
| Kod içi filtre | `ActiveFilterString` | kriter diline göre | ekrandaki `DataTable` |
| DB araması | Dapper `@p1` | sorgu indeksine göre | **tüm tablo** (indeksli) |

## §2 OptionsFind Ozellikleri

| Ozellik | Varsayilan | Ne is yarar | Ornek |
|---|---|---|---|
| `OptionsFind.AlwaysVisible` | `false` | paneli sürekli açık tut | `true` (arama ekranı) |
| `OptionsFind.FindNullPrompt` | — | boşken ipucu metni | `"Ad, kod, tutar ara..."` |
| `OptionsFind.FindFilterColumns` | `"*"` | aranacak sütunlar | `"Ad;Sehir"` |
| `OptionsFind.SearchFilterCondition` | `AnyWord` | kelime eşleme kipi | `All` / `ExactMatch` |
| `OptionsFind.ShowClearButton` | `true` | temizle düğmesi | `true` |
| `OptionsFind.HighlightFindResults` | `true` | eşleşmeyi vurgula | `true` |

```csharp
gridView1.OptionsFind.FindFilterColumns = "Ad;Sehir";
gridView1.OptionsFind.FindNullPrompt = "Ad, sehir veya kod ara...";
gridControl1.ShowFindPanel();
```

## §3 Sonuclari Iyilestirme

| Sorun | Neden | Duzeltme |
|---|---|---|
| Türkçe `ç/ğ/ı/ö/ş/ü` eşleşmiyor | kültür `tr-TR` değil | `Thread.CurrentThread.CurrentCulture` = `tr-TR` |
| Panel aramıyor, hiçbir şey bulamıyor | sütun `Visible=false` | `FindFilterColumns`'a alan adını **açıkça** yaz |
| Yazım açıkken arama sonuç vermiyor | editör hâlâ açık | önce `CloseEditor()` |
| 100.000+ satırda yavaş | `SELECT *` + istemci süzme | DB'ye `LIKE @p1` yaz, grid'e sonucu bağla |
| Panel fazla yer kaplıyor | `AlwaysVisible = true` | `HideFindPanel()` + düğmeye bağla |

Arama sonucuyla gezinme: `Enter` = sonraki eşleşme, `Shift+Enter` = önceki, `Esc` = paneli kapat (varsayılan kısayollar, `OptionsFind` ile değiştirilebilir).

## §4 net48 ve net10.0-windows'da fark

| Konu | net48 | net10.0-windows |
|---|---|---|
| Dil sürümü | **C# 7.3** — `using var` **yasak** | C# 14 (paylaşılan dosya 7.3) |
| Kültür | `System.Globalization.CultureInfo("tr-TR")` | aynı; `CurrentCulture` varsayılanı OS'a bağlı |
| DevExpress | 26.1 + binding redirect | 26.1 (aynı CPM satırı) |
| Dapper `LIKE` | `@p1` + `N'%' + @p1 + '%'` yerine `LIKE @p1` (parametre tam değeri ver) | aynı |

```csharp
conn.ExecuteScalar<int>("SELECT COUNT(*) FROM dbo.Musteri WHERE Ad LIKE @p1",
                        new { p1 = "%" + arama + "%" });
```

> **YASAK:** arama metnini `sql + " WHERE Ad LIKE '%" + x + "%'"` biçiminde **birleştirme** (G2).

## §5 Kardes Dosyalar

| Konu | Dosya |
|---|---|
| Filtre + sıralama | [grid-filtering-sorting](grid-filtering-sorting.md) |
| Sayfalama + sanal mod | [grid-paging-virtual-mode](grid-paging-virtual-mode.md) |
| Veri kaynağı (Dapper) | [datasource-binding-dapper](datasource-binding-dapper.md) |
| Grid iskeleti | [gridcontrol-basics](gridcontrol-basics.md) |
