---
title: "treelist-inplace-editing - VersaCoder"
type: knowledge
category: devexpress
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "TreeList hucre duzenleme (OptionsBehavior.Editable/OptionsColumn), ValidateNode/SetColumnError, CellValueChanged ve Dapper ile kayit (v26.1)"
---

# treelist-inplace-editing — Yerlesik duzenleme ve dogrulama (v26.1)

**Zorunlu Baglantilar:** [index](../index.md) · [devexpress-rules](../../.rules/devexpress-rules.md) · [ADR-004](../../.decisions/accepted/ADR-004-devexpress-universal-v261-office2010.md) · [treelist-hierarchical-data](treelist-hierarchical-data.md) · [grid-editing-modes](grid-editing-modes.md)

> **Tek cumlelik sorumluluk:** *TreeList'te hucre kim tarafindan acilir, hatali dugum nerede yakalanir ve deger nereye yazilir* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

TreeList'te editör üç kademede açılır: `OptionsBehavior.Editable` (tümü), `OptionsColumn.AllowEdit` (sütun), `ShowingEditor` + `e.Cancel` (koşullu hücre). Doğrulama `ValidateNode` olayındadır: `Valid = false` + `ErrorText` verilirse düğüm odağı bırakmaz, `SetColumnError` ile hata ikonu da konur. Değer değişikliği `CellValueChanged` ile yakalanır; **veritabanına yazma orada değil**, onay sonrası tek Dapper `UPDATE` ile yapılır (G2). Grid ile aynı `RepositoryItem` editörleri kullanılır.

---

## §1 Duzenleme Acma Kurallari

| Kademe | Ozellik / olay | Sonuc |
|---|---|---|
| Tümü | `treeList1.OptionsBehavior.Editable` | `false` → hiçbir hücre açılmaz |
| Sütun | `col.OptionsColumn.AllowEdit` | `false` → sütun tıklanamaz |
| Sütun (kopya açık) | `col.OptionsColumn.ReadOnly` | `true` → editör açılır ama yazılamaz |
| Koşullu hücre | `ShowingEditor` + `e.Cancel` | odak satıra göre kapat |
| Koşullu editör | `ShownEditor` → `ActiveEditor.Properties.ReadOnly` | kademeli ağaçta yalnız kök düzenlenir |

```csharp
private void treeList1_ShowingEditor(object sender, System.ComponentModel.CancelEventArgs e)
{
    if (treeList1.FocusedColumn.FieldName == "Kod" && treeList1.FocusedNode.Level > 0)
        e.Cancel = true;   // alt dugumlerde Kod degistirilemez
}
```

## §2 Editör Secimi (RepositoryItem)

| Sutun tipi | RepositoryItem | Ayar |
|---|---|---|
| metin | `RepositoryItemTextEdit` | `MaxLength`, `Mask` |
| liste | `RepositoryItemComboBoxEdit` | `Items` |
| ilgili kayıt | `RepositoryItemLookUpEdit` | `DataSource`, `ValueMember` (bkz. [lookUpEdit-cascading](lookUpEdit-cascading.md)) |
| evet/hayır | `RepositoryItemCheckEdit` | `ValueChecked` |
| sayı | `RepositoryItemSpinEdit` | `EditMask = "n2"` |

```csharp
treeList1.Columns["Tutar"].ColumnEdit = new RepositoryItemSpinEdit();
treeList1.OptionsBehavior.Editable = true;
```

## §3 Dogrulama ve Degisiklik

```csharp
private void treeList1_ValidateNode(object sender, ValidateNodeEventArgs e)
{
    string ad = Convert.ToString(e.Node[colAd]);
    if (string.IsNullOrWhiteSpace(ad))
    {
        e.Valid = false;
        e.ErrorText = "Ad zorunludur.";
        treeList1.SetColumnError(colAd, e.ErrorText);
    }
}
```

| Olay / yontem | Ne zaman | Yapilacak |
|---|---|---|
| `ValidateNode` | düğüm odağı bırakacak | `Valid` + `ErrorText` |
| `InvalidNodeException` | kural ihlali sonrası | kullanıcı mesajını özelleştir |
| `CellValueChanged` | editör kapandı, değer değişti | DB'ye **yazma**, yalnız UI hesabını tazele |
| `SetColumnError` | hata gösterimi | hücreye ikon + ipucu |
| `BeginUpdate` / `EndUpdate` | toplu değişiklik | yeniden çizimi topla |

Kaydetme (onaydan **sonra**, parametreli — G2):

```csharp
conn.Execute("UPDATE dbo.Kategori SET Ad = @p1, UstId = @p2 WHERE Id = @p3",
             new { ad, ustId, id }, commandTimeout: 30);
```

> **UYARI:** `CellValueChanged` her hücrede tetiklenir; içinde tek tek `UPDATE` çalıştırmak (N sorgu) yerine **tek** kaydet düğmesi + transaction kullan.

## §4 net48 ve net10.0-windows'da fark

| Konu | net48 | net10.0-windows |
|---|---|---|
| Dil sürümü | **C# 7.3** — `using var` **yasak**, `Convert.ToString` nullable'a karşı güvenli | C# 14 (paylaşılan dosya 7.3) |
| Olay imzası | `System.ComponentModel.CancelEventArgs` (aynı) | aynı |
| DevExpress | 26.1 + binding redirect | 26.1 (aynı CPM satırı) |
| Dapper | `Execute` + `commandTimeout` | aynı |

> **YASAK:** EF Core `SaveChanges`, `ChangeTracker` (G2) ve `v26.1` dışı editör paketi (DX-02).

## §5 Kardes Dosyalar

| Konu | Dosya |
|---|---|
| Ağaç veri bağlama | [treelist-hierarchical-data](treelist-hierarchical-data.md) |
| Grid düzenleme (karşılaştırma) | [grid-editing-modes](grid-editing-modes.md) |
| Kademeli lookUpEdit | [lookUpEdit-cascading](lookUpEdit-cascading.md) |
| Veri kaynağı (Dapper) | [datasource-binding-dapper](datasource-binding-dapper.md) |
