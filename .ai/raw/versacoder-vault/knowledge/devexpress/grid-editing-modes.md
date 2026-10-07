---
title: "grid-editing-modes - VersaCoder"
type: knowledge
category: devexpress
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "GridView duzenleme modlari, RepositoryItem editörleri, hucre/satir dogrulama ve Dapper ile kayit yazma (v26.1)"
---

# grid-editing-modes — Duzenleme modlari ve dogrulama (v26.1)

**Zorunlu Baglantilar:** [index](../index.md) · [devexpress-rules](../../.rules/devexpress-rules.md) · [ADR-004](../../.decisions/accepted/ADR-004-devexpress-universal-v261-office2010.md) · [gridcontrol-basics](gridcontrol-basics.md) · [lookUpEdit-cascading](lookUpEdit-cascading.md)

> **Tek cumlelik sorumluluk:** *Hucre nasil duzenlenir, ne zaman kaydedilir ve hatali veri nerede yakalanir* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Düzenleme üç karardır: **açık/kapalı** (`OptionsBehavior.Editable`), **editör tipi** (`col.ColumnEdit` = `RepositoryItem`) ve **kaydetme anı** (satır değişince mi, butonla mı). Grid'de değişiklik **önce bellekteki `DataTable`'ı** yazar; veritabanına Dapper `UPDATE` ile **sen** geçersin — bu yüzden "grid değişti" ile "veritabanı değişti" aynı değildir. Doğrulama iki katmandır: `ValidatingEditor` (hücre kapatılırken) ve `ValidateRow` (satır tamamlanırken).

---

## §1 Mod Secimi

| Mod | Kurulum | Ne zaman kullanilir |
|---|---|---|
| Salt okunur | `gridView1.OptionsBehavior.Editable = false` | rapor / log ekranı |
| Hücre düzenlemeli | `Editable = true` + `EnterMoveNextColumn = true` | hızlı giriş (fiş) |
| Satır düzenlemeli | `OptionsEdit` + `UpdateCurrentRow()` ile kilit | kart formu |
| Satır ekleme | `OptionsView.NewItemRowPosition = Top/Bottom` | yeni kayıt satırı |
| Çoklu seçimde toplu | `OptionsSelection.MultiSelect` + repository sabit | toplu durum değişimi |

## §2 RepositoryItem Editörleri

| Kolon tipi | RepositoryItem | Ayarlanan ozellik |
|---|---|---|
| metin | `RepositoryItemTextEdit` | `MaxLength`, `Mask` |
| seçim listesi | `RepositoryItemComboBoxEdit` | `Items` / `DataSource` |
| ilgili kayıt | `RepositoryItemLookUpEdit` | `DataSource`, `ValueMember` (bkz. [lookUpEdit-cascading](lookUpEdit-cascading.md)) |
| evet/hayır | `RepositoryItemCheckEdit` | `ValueChecked` / `ValueUnchecked` |
| sayı / tarih | `RepositoryItemSpinEdit` / `RepositoryItemDateEdit` | `MinValue`, `EditMask` |

```csharp
colTutar.ColumnEdit = new RepositoryItemSpinEdit();
((RepositoryItemSpinEdit)colTutar.ColumnEdit).EditMask = "n2";
gridView1.OptionsBehavior.Editable = true;
```

## §3 Dogrulama ve Kayit

```csharp
private void grid_ValidatingEditor(object sender, BaseContainerValidateEditorEventArgs e)
{
    if (gridView1.FocusedColumn.FieldName == "Tutar" && Convert.ToDecimal(e.Value) < 0)
        e.ErrorText = "Tutar 0 dan kucuk olamaz.";   // hucre kirmizi olur
}
```

Satır onayı ve yazma (Dapper — parametreli, string-concat **yok**, G2):

```csharp
private void grid_ValidateRow(object sender, ValidateRowEventArgs e) { e.Valid = KuralSagla(); }

conn.Execute("UPDATE dbo.Fatura SET Tutar = @p1 WHERE Id = @p2",
             new { p1 = tutar, p2 = id }, commandTimeout: 30);
gridControl1.RefreshDataSource();
```

| Olay | Ne zaman | Sorumluluk |
|---|---|---|
| `ValidatingEditor` | hücre editöründen çıkılırken | tek hücre kuralı |
| `CellValueChanging` | değer değişmeden önce | iptal (`e.Cancel = true`) |
| `CellValueChanged` | değer değişti | UI hesabını tazele, DB'ye **yazma** |
| `ValidateRow` / `InvalidRowException` | satır tamamlanırken | satır kuralı + hata mesajı |

> **UYARI:** `CellValueChanged` içinde DB yazma — kullanıcı satırdan daha çıkabilir; kaydet butonu + tek `Execute` (transaction) kullan.

## §4 net48 ve net10.0-windows'da fark

| Konu | net48 | net10.0-windows |
|---|---|---|
| Dil sürümü | **C# 7.3** — `out` değişken atama C# 7 ile var; `using var` **yasak** | C# 14 (paylaşılan dosya 7.3) |
| `Convert.ToDecimal` | kültüre bağlı — `tr-TR` girişte ondalık virgül | aynı; `CultureInfo.CurrentCulture` açık set et |
| DevExpress editörleri | 26.1 repository tipleri (aynı) | aynı |
| Veritabanı yazma | Dapper `Execute` + `commandTimeout` | aynı |

> **YASAK:** EF Core `SaveChanges`, `AddOrUpdate` gibi bir DbContext katmanı (G2); grid üzerinden yazma yalnız ADO.NET/Dapper.

## §5 Kardes Dosyalar

| Konu | Dosya |
|---|---|
| Editör bağlama (kademeli) | [lookUpEdit-cascading](lookUpEdit-cascading.md) |
| Kolon ve biçim | [gridview-columns-format](gridview-columns-format.md) |
| Sayfalama + sanal mod | [grid-paging-virtual-mode](grid-paging-virtual-mode.md) |
| Veri kaynağı (Dapper) | [datasource-binding-dapper](datasource-binding-dapper.md) |
