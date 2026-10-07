---
title: "gridview-columns-format - VersaCoder"
type: knowledge
category: devexpress
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "GridView kolon baglari, tip ve bicim katmanlari, unbound kolonlar ve CustomColumnDisplayText (v26.1, Dapper IDataReader/DataTable)"
---

# gridview-columns-format — Kolon baglari ve bicim (v26.1)

**Zorunlu Baglantilar:** [index](../index.md) · [devexpress-rules](../../.rules/devexpress-rules.md) · [ADR-004](../../.decisions/accepted/ADR-004-devexpress-universal-v261-office2010.md) · [gridcontrol-basics](gridcontrol-basics.md) · [grid-conditional-format-rules](grid-conditional-format-rules.md)

> **Tek cumlelik sorumluluk:** *Bir kolon veriyi nereden alir, nasil bicimlenir ve ne zaman metni kodda degistirilir* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Kolonun görünümü **üç katmanda** üretilir: ham veri (`FieldName`) → `FormatString` → gerekirse `CustomColumnDisplayText`. Bağ ve biçim **Designer'da** tutulur; kod yalnız hesaplanan (unbound) kolonlar ve maskelenmiş metinler için devreye girer. Elle hücre boyama (`CustomDrawCell`) **yasak** (DX-08) — onun yerine `FormatString`, `AppearanceCell` ve format rule kullanılır. Sürüm **yalnız DevExpress Universal 2026 / v26.1** (ADR-004).

---

## §1 Kolon Ozellikleri

| Ozellik | Ne is yarar | Ornek |
|---|---|---|
| `FieldName` | veri kaynağındaki alan adı | `"Tutar"` |
| `Caption` | başlık metni | `"Tutar (TL)"` |
| `VisibleIndex` | sıralama ve görünürlük (`-1` = gizli) | `2` |
| `Width` | sütun genişliği (px) | `120` |
| `FormatType` + `FormatString` | hücre biçimi | `Numeric` + `{0:n2}` |
| `UnboundType` | hesaplanan sütun tipi | `UnboundColumnType.String` |
| `ColumnEdit` | hücre editörü (RepositoryItem) | `RepositoryItemTextEdit` |

`FieldName` değeri veri kaynağında yoksa kolon **boş görünür** ve hata da üretmez; ilk kontrol edilecek yer burasıdır.

## §2 Bicimleme Sırası (kod)

```csharp
var col = gridView1.Columns["Tutar"];
col.Caption = "Tutar (TL)";
col.FormatType = DevExpress.Utils.FormatType.Numeric;
col.FormatString = "{0:n2}";
col.VisibleIndex = 2;
```

Biçim eşleştirme tablosu:

| Veri tipi | FormatString | Sonuc |
|---|---|---|
| para / ondalık | `{0:n2}` | `1.234,56` |
| tarih | `{0:dd.MM.yyyy}` | `25.09.2026` |
| yüzde | `{0:p0}` | `%12` |
| metin maskesi | `CustomColumnDisplayText` olayı | `TR** **** **** 1234` |
| evet/hayır | `RepositoryItemCheckEdit` + `Caption` | `☑` |

## §3 Kodda Metin Uretimi ve Unbound Kolon

```csharp
private void grid_CustomColumnDisplayText(object sender, CustomColumnDisplayTextEventArgs e)
{
    if (e.Column.FieldName == "Iban")
        e.DisplayText = MaskIban(e.Value as string);
}
```

Hesaplanan sütun (veri kaynağında yok, üretiliyor):

```csharp
var col = gridView1.Columns["DurumText"];
col.UnboundType = DevExpress.Data.UnboundColumnType.String;
gridView1.CustomUnboundColumnData += (s, e) =>
{
    if (e.Column.FieldName == "DurumText")
        e.Value = Convert.ToInt32(e.GetListSourceRowValue("Durum")) == 1 ? "Aktif" : "Pasif";
};
```

`GetListSourceRowValue` satır verisine erişir; bu, `SELECT *` yerine yalnız gerekli alanları çekmeyi mümkün kılar.

## §4 Icerik Kurallari

- `FormatString` varsa `CustomColumnDisplayText` **gerekmez**; ikisi birden yazma (öncelik: olay kazanır).
- Kolon başlığı sabit metinse `Caption`, yerelleştirme gerekirse `.resx` (DX-05).
- Sütun sırası/genişliği `SaveLayoutToXml` ile kullanıcıya bırakılır; kodda `VisibleIndex` **zorunlu** olanlar için kullanılır.
- `BestFitColumns()` yalnız ilk açılışta çağrılır; her `DataSourceChanged`'te çağırmak kaydırma hissi yaratır.
- Maskelenen veri (`Iban`, `Telefon`) için `CustomColumnDisplayText` yeterli; veritabanına maske **yazma**.

## §5 net48 ve net10.0-windows'da fark

| Konu | net48 | net10.0-windows |
|---|---|---|
| Dil sürümü | **C# 7.3** — lambda body'de `if` bloğu, `var` ile tip çıkarımı serbest; `using var` **yasak** | C# 14 (paylaşılan dosya 7.3 kalır) |
| `FormatString` davranışı | `n2`/`dd.MM.yyyy` kültür'e bağlı | aynı; `CultureInfo` açık verilirse sonuç aynı |
| DevExpress paketi | 26.1 + binding redirect | aynı CPM satırı |
| `UnboundColumnType` | `DevExpress.Data` (aynı tip) | aynı tip |

> **YASAK:** EF Core, `v26.1` dışı sürüm ve elle hücre çizimi (G2 / DX-02 / DX-08).

## §6 Kardes Dosyalar

| Konu | Dosya |
|---|---|
| Grid iskeleti | [gridcontrol-basics](gridcontrol-basics.md) |
| Koşullu biçimlendirme | [grid-conditional-format-rules](grid-conditional-format-rules.md) |
| Veri kaynağı (Dapper) | [datasource-binding-dapper](datasource-binding-dapper.md) |
