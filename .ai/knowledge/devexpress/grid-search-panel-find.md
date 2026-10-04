---
title: "grid-search-panel-find - VersaCoder"
type: knowledge
category: devexpress
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "GridControl arama paneli (Search Panel), OptionsFind ayarlari, kolon bazli filtre popup'i ve veritabani aramasina gecis"
---

# Grid Arama Paneli ve Find

**Zorunlu Baglantilar:** [vault indeks](../index.md) · [devexpress-rules](../../.rules/devexpress-rules.md) · [data-access-rules](../../.rules/data-access-rules.md) · [grid-filtering-sorting](grid-filtering-sorting.md) · [gridcontrol-basics](gridcontrol-basics.md) · [datasource-binding-dapper](datasource-binding-dapper.md)

> **Tek cumlelik sorumluluk:** *Arama paneli neyi arar, kolon filtresi neyi arar ve hangisini veritabanina birakmak gerekir* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Grid'de **iki ayri arama** vardir: ustteki **Search Panel** (Ctrl+F, `OptionsFind` ile yonetilir, *tum gorunur sutunlarda* metin arar) ve sutun basligindaki **Filter Popup** (tek sutunda kriter). Ikisi de istemci tarafinda calisir; veri buyurse asil arama Dapper sorgusuna (`WHERE ... LIKE @p1`) tasinmalidir. Arama panelini ac/kapa tek cagiridir: `gridControl1.ShowFindPanel()` / `HideFindPanel()`.

```text
+------------------------------------------------------------------+
| Search Panel (Ctrl+F)  -> gorunur TUM sutunlar, istemcide         |
| Filter Popup           -> tek sutun, istemcide                    |
| ActiveFilterString     -> kod ici kriter, istemcide               |
| Dapper LIKE @p1        -> DB indeksi, TUM tablo                  |
+------------------------------------------------------------------+
```

---

## §1 Secim tablosu

| Arama | Nerede | Kapsam | Kac satir tarar |
|---|---|---|---|
| Search Panel (Ctrl+F) | grid ustu | gorunur tum sutunlar | ekrandaki `DataTable` |
| Filter Popup | sutun basligi | tek sutun | ekrandaki `DataTable` |
| Kod icinde filtre | `ActiveFilterString` | kriter diline gore | ekrandaki `DataTable` |
| DB aramasi | Dapper `@p1` | sorgu indeksine gore | **tum tablo** (indeksli) |

---

## §2 OptionsFind ozellikleri

| Ozellik | Varsayilan | Ne is yarar | Ornek |
|---|---|---|---|
| `OptionsFind.AlwaysVisible` | `false` | paneli surekli acik tut | `true` (arama ekrani) |
| `OptionsFind.FindNullPrompt` | - | bosken ipucu metni | `"Ad, kod, tutar ara..."` |
| `OptionsFind.FindFilterColumns` | `"*"` | aranacak sutunlar | `"Ad;Sehir"` |
| `OptionsFind.SearchFilterCondition` | `AnyWord` | kelime esleme kipi | `All` / `ExactMatch` |
| `OptionsFind.ShowClearButton` | `true` | temizle dugmesi | `true` |
| `OptionsFind.HighlightFindResults` | `true` | eslesmeyi vurgula | `true` |

```csharp
gridView1.OptionsFind.FindFilterColumns = "Ad;Sehir";
gridView1.OptionsFind.FindNullPrompt = "Ad, sehir veya kod ara...";
gridControl1.ShowFindPanel();
```

---

## §3 Sonuclari iyilestirme

| Sorun | Neden | Duzeltme |
|---|---|---|
| Turkce `c/g/i/o/s/u` eslesmiyor | kultur `tr-TR` degil | `CurrentCulture` = `tr-TR` |
| Panel aramiyor | sutun `Visible = false` | `FindFilterColumns`'a alan adini **acikca** yaz |
| Yazic acikken sonuc yok | editör hala acik | once `CloseEditor()` |
| 100.000+ satirda yavas | `SELECT *` + istemci sureme | DB'ye `LIKE @p1` yaz, grid'e sonucu bagla - bu esik `DOĞRULAMA GEREKLI` |
| Panel fazla yer kapliyor | `AlwaysVisible = true` | `HideFindPanel()` + dugmeye bagla |

Arama sonucuyla gezinme: `Enter` = sonraki eslesme, `Shift+Enter` = onceki, `Esc` = paneli kapat (varsayilan kisayollar, `OptionsFind` ile degistirilebilir).

---

## §4 DB aramasina gecis (Dapper)

```csharp
conn.ExecuteScalar<int>("SELECT COUNT(*) FROM dbo.Musteri WHERE Ad LIKE @p1",
                        new { p1 = "%" + arama + "%" });
```

> **YASAK:** arama metnini `sql + " WHERE Ad LIKE '%" + x + "%'"` biciminde **birlestirme** (G2).

---

## §5 Kim neyi degistirir

| Degisiklik | Nereye dokunulur | Etki |
|---|---|---|
| Aranacak sutunlar | `OptionsFind.FindFilterColumns` | kapsam daralir/genisler |
| Esleme kipi | `SearchFilterCondition` | `AnyWord` / `All` / `ExactMatch` |
| Panel gorunurlugu | `AlwaysVisible` + `Show/HideFindPanel` | ekran yer kaplamasi |
| Kultur | `CultureInfo.CurrentCulture` (`tr-TR`) | buyuk/kucuk harf + aksan |
| Buyuk veri | Dapper `LIKE @p1` sorgusu | tarama DB'ye gecer |

---

## §6 net48 ve net10.0-windows'da fark

| Konu | net48 | net10.0-windows |
|---|---|---|
| Dil surumu | **C# 7.3** - `using var` **yasak** | C# 14 (paylasilan dosya 7.3) |
| Kultur | `System.Globalization.CultureInfo("tr-TR")` | ayni; `CurrentCulture` varsayilani OS'a bagli |
| DevExpress | 26.1 + binding redirect | 26.1 (ayni CPM satiri) |
| Dapper `LIKE` | `LIKE @p1` + deger anonim parametrede (`%...%`) | ayni |

---

## §7 Kardes dosyalar

| Konu | Dosya |
|---|---|
| Filtre + siralama | [grid-filtering-sorting](grid-filtering-sorting.md) |
| Sayfalama + sanal mod | [grid-paging-virtual-mode](grid-paging-virtual-mode.md) |
| Veri kaynagi (Dapper) | [datasource-binding-dapper](datasource-binding-dapper.md) |
| Grid iskeleti | [gridcontrol-basics](gridcontrol-basics.md) |

---

## §8 Denetim listesi

| # | Adim | Kanit |
|---|---|---|
| 1 | Panel kipleri (`FindFilterColumns`, `SearchFilterCondition`) ekrana gore | ozellik denetimi |
| 2 | Kultur `tr-TR` acik | kod grep |
| 3 | Buyuk veride arama SQL'de (`LIKE @p1`) | sorgu incelemesi |
| 4 | String-concat ile `LIKE` yok | kod taramasi |
| 5 | Gizli sutunlar arama disinda birakildi mi? soruldu | test senaryosu |

---

## §9 Kanit

| Kaynak dosya | Ne alindi |
|---|---|
| `knowledge\devexpress\grid-search-panel-find.md` (kaynak vault) | iki arama ayrimi, OptionsFind tablosu, sorun/duzeltme tablosu, Dapper `LIKE` ornegi |
| `.rules\data-access-rules.md` | parametreli `LIKE` (G2), string-concat yasagi |

> Kanitsiz iddia icin DOĞRULAMA GEREKLI yazilir; bu dosyada kaynak disi bilgi yoktur.
