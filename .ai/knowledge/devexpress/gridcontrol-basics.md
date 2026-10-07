---
title: "gridcontrol-basics - VersaCoder"
type: knowledge
category: devexpress
status: active
version: 1.0.0
updated: 2026-09-29
authority: SSOT
scope: "GridControl + GridView iskeleti, baglama sirasi, veri doldurma (Dapper -> DataTable) ve olay noktalari (DevExpress Universal 2026 / v26.1)"
---

# GridControl + GridView Iskeleti

**Zorunlu Baglantilar:** [vault indeks](../index.md) · [devexpress-rules](../../.rules/devexpress-rules.md) · [data-access-rules](../../.rules/data-access-rules.md) · [datasource-binding-dapper](datasource-binding-dapper.md) · [grid-filtering-sorting](grid-filtering-sorting.md)
**Küme + Hub:** [devexpress index](index.md) · [index](../../index.md) · [CLAUDE](../../claude.md) · [AGENTS](../../definitions/agents.md) · [brain](../../.decisions/brain.md) · [glossary](../../definitions/glossary.md) · [keys](../../governance/keys.md)
**Kardeşler (devexpress · 3/35):** [accessibility-keyboard-nav](accessibility-keyboard-nav.md) · [accordion-navigation](accordion-navigation.md) · [alertcontrol-notifications](alertcontrol-notifications.md) · [bar-status-items](bar-status-items.md) · [chartcontrol-basics](chartcontrol-basics.md) · [checkedcomboedit-values](checkedcomboedit-values.md) · [comboboxedit-autocomplete](comboboxedit-autocomplete.md) · [dateedit-timeedit](dateedit-timeedit.md) · [designer-runtime-customization](designer-runtime-customization.md) · [docking-saved-layout-xml](docking-saved-layout-xml.md) · [docklayout-persistence](docklayout-persistence.md) · [dockmanager-panels](dockmanager-panels.md) · [dx-validation-provider](dx-validation-provider.md) · [grid-conditional-format-rules](grid-conditional-format-rules.md) · [grid-editing-modes](grid-editing-modes.md) · [grid-export-excel-pdf](grid-export-excel-pdf.md) · [grid-grouping-summaries](grid-grouping-summaries.md) · [grid-paging-virtual-mode](grid-paging-virtual-mode.md) · [grid-search-panel-find](grid-search-panel-find.md) · [gridview-columns-format](gridview-columns-format.md) · [layoutcontrol-grouping](layoutcontrol-grouping.md) · [lookUpEdit-cascading](lookupedit-cascading.md) · [mvvm-pattern-devexpress](mvvm-pattern-devexpress.md) · [nuget-references-cpm](nuget-references-cpm.md) · [performance-grid-optimization](performance-grid-optimization.md) · [ribbon-control-office2010](ribbon-control-office2010.md) · [ribbon-form-basics](ribbon-form-basics.md) · [ribbon-pages-groups-items](ribbon-pages-groups-items.md) · [skin-customization](skin-customization.md) · [splashscreen-waitform](splashscreen-waitform.md) · [tabcontrol-multipage](tabcontrol-multipage.md) · [theme-manager-office2010](theme-manager-office2010.md)

> **Tek cumlelik sorumluluk:** *GridControl hangi veriyle, hangi sirayla ve hangi olay noktalariyla kurulur* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Grid'de tek cift vardir: **`GridControl` veri tasiir, `GridView` yonetimi yurutur** (kolon, siralama, gruplama, editor). Bu vault'ta veri kaynagi **Dapper `ExecuteReader` -> `DataTable.Load`** yoludur; EF Core yasaktir. Baglama sirti sabittir: `DataSource` -> `PopulateColumns()` -> gorunum ayarlari; tersi kurulursa kolonlar bos kalir ve hata da vermez. Kolon/gorunum ayarlari **Designer'da**, veri doldurma ve olay mantigi **kodda** yasar.

---

## §1 Sorumluluk dagilimi

| Nesne | Ne yapar | Nerede yazilir |
|---|---|---|
| `GridControl` | `DataSource`, seviye (level), gomulu gorunum, export | Designer + kod (yalniz `DataSource`) |
| `GridView` | kolon, siralama, gruplama, filtre, editor, ozet | Designer + kod (ayarlar) |
| `GridColumn` | `FieldName`, `Caption`, `VisibleIndex`, bicim | Designer |
| `GridLevelDescriptor` | master-detail (cocuk gorunum) hiyerarsisi | Designer |

Kolon ekleme/silme kodda tekrarlanmaz; Designer'da kurulur, kod yalniz otomatik uretimi (`PopulateColumns`) cagirir.

---

## §2 Kurulum sirasi (kod)

```csharp
gridControl1.Dock = DockStyle.Fill;
gridControl1.DataSource = LoadMusteriler();   // Dapper -> DataTable
gridView1.PopulateColumns();                  // kolonlari uret
gridView1.BestFitColumns();
gridView1.OptionsBehavior.Editable = false;   // salt okunur ornek
```

Veri doldurma - Dapper `IDataReader` -> `DataTable` (string-concat SQL yok):

```csharp
using (var conn = new SqlConnection(connString))
{
    var table = new DataTable();
    using (var reader = conn.ExecuteReader(sql, new { p1 = "Aktif" }))
        table.Load(reader);
    gridControl1.DataSource = table;
}
```

---

## §3 Temel ozellikler

| Ozellik | Varsayilan | Ne is yarar | Ornek deger |
|---|---|---|---|
| `gridControl1.DataSource` | `null` | veri seti | `DataTable` / `List<T>` |
| `gridView1.Name` | - | layout kaydi anahtari | `"gridViewMusteri"` |
| `gridView1.OptionsView.ShowGroupPanel` | `true` | surukle-birak grup paneli | `false` |
| `gridView1.OptionsBehavior.Editable` | `true` | hucre duzenleme | `false` (rapor ekrani) |
| `gridView1.OptionsSelection.MultiSelect` | `false` | coklu secim | `true` (toplu islem) |
| `gridView1.OptionsView.ShowFooter` | `false` | ozet satiri | `true` (tutar kolonu varsa) |

`Name` alani benzersiz tutulmalidir; `SaveLayoutToXml` / `RestoreLayoutFromXml` bu adla calisir.

---

## §4 Olay noktalari

| Olay | Ne zaman tetiklenir | Tipik kullanim |
|---|---|---|
| `FocusedRowObjectChanged` | odak satiri degisir | sag paneli / detayi besle |
| `DoubleClick` | satira cift tiklama | detay formunu ac |
| `CustomColumnDisplayText` | hucre metni uretilir | maskeleme (IBAN, telefon) |
| `ShownEditor` | editor acildi | etkin editorun ozelligini degistir |
| `DataSourceChanged` | veri yenilendi | `BestFitColumns()` / sayac guncelle |

Handler ince kalir; is mantigi veri katmanindaki repository'ye gider.

---

## §5 Kontrol listesi

| # | Adim | Kanit |
|---|---|---|
| 1 | Baglama sirasi `DataSource` -> `PopulateColumns` | kod incelemesi |
| 2 | SQL parametreli (`@pN`), string-concat yok | kod taramasi |
| 3 | Kolonlar Designer'da, kodda `Columns.Add` yok | grep |
| 4 | Layout anahtari (`Name`) benzersiz | isim listesi |
| 5 | Iki bacakta `dotnet build` -> 0 hata | build ciktisi |

---

## §9 Kanit

| Kaynak dosya | Ne alindi |
|---|---|
| `knowledge\devexpress\gridcontrol-basics.md` (kaynak vault) | sorumluluk dagilimi, kurulum sirasi, Dapper -> DataTable kodu, ozellik + olay tablolari |
| `.rules\devexpress-rules.md` | DX-08 (grid kurulumu) |
| `.rules\data-access-rules.md` | parametreli sorgu, EF Core yasagi |

> Kanitsiz iddia icin `DOĞRULAMA GEREKLI` yazilir; bu dosyada kaynak disi bilgi yoktur.
