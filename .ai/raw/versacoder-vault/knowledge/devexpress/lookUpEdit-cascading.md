---
title: "lookUpEdit-cascading - VersaCoder"
type: knowledge
category: devexpress
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "LookUpEdit kademeli (cascading) baglanti: CascadingOwner, EditValueChanged ile Dapper yeniden yukleme ve grid icinde ShownEditor (v26.1)"
---

# lookUpEdit-cascading — Kademeli lookUpEdit (v26.1)

**Zorunlu Baglantilar:** [index](../index.md) · [devexpress-rules](../../.rules/devexpress-rules.md) · [ADR-004](../../.decisions/accepted/ADR-004-devexpress-universal-v261-office2010.md) · [grid-editing-modes](grid-editing-modes.md) · [datasource-binding-dapper](datasource-binding-dapper.md)

> **Tek cumlelik sorumluluk:** *Il-Ilce gibi bagli listelerde alt liste nasil filtelenir, veri ne zaman Dapper ile yeniden yuklenir* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

İki yol vardır: **(a) bildirimsel** — `CascadingOwner` ile ana editörü tanımla, DevExpress anahtar alanını kendisi bulur (`CascadingMember` ile elle verilir); **(b) elle** — ana editörün `EditValueChanged` olayında Dapper parametreli sorguyla alt listenin `DataSource`'unu tazeler. Kalıcı/çok büyük liste **(b)** (sorgu indeksli, tamamı çekme); küçük ve sabit ilişki **(a)**. Grid içinde ise kabuk `ShownEditor` olayında etkin editörün `Properties.DataSource`'ı satıra göre doldurulur.

---

## §1 Iki Yol

| Yol | Kurulum | Ne zaman |
|---|---|---|
| (a) Bildirimsel | `alt.CascadingOwner = ust;` | ilişki sabit, liste küçük |
| (a) Elle anahtar | `alt.CascadingMember = "IlId";` | otomatik eşleşme bulamazsa |
| (b) Elle yükleme | `EditValueChanged` → Dapper `@p1` | liste büyük / DB'den |
| (b) Popup'ta filtre | `PopupFilter` olayı | yalnız açılırken süzmek istenirse |
| Grid içi | `gridView1.ShownEditor` | satır bazlı kademeli liste |

```csharp
lookupIlce.CascadingOwner = lookupIl;          // alt -> ust baglantisi
lookupIlce.EditValue = null;                   // ust degisince alti bosalt
```

## §2 Elle Yukleme (Dapper, parametreli)

```csharp
private void lookupIl_EditValueChanged(object sender, EventArgs e)
{
    lookupIlce.EditValue = null;
    int ilId = Convert.ToInt32(lookupIl.EditValue);
    lookupIlce.Properties.DataSource = LoadIlceler(ilId);
}
```

```csharp
private static object LoadIlceler(int ilId)
{
    using (var conn = new SqlConnection(connString))
        return conn.Query("SELECT Id, Ad FROM dbo.Ilce WHERE IlId = @p1 ORDER BY Ad",
                          new { p1 = ilId }).ToList();
}
```

> **UYARI:** `EditValue = null` satırı **zorunlu** — üst değer değiştiğinde altta eski `Id` kalırsa `ValueMember` karşılıksız kalır ve editör boş görünür (kayıtta hata üretmez, sürpriz yapar).

## §3 Editör Ozellikleri

| Ozellik | Ne yapar | Ornek |
|---|---|---|
| `Properties.DataSource` | liste kaynağı | `List<T>` / `DataTable` |
| `Properties.DisplayMember` | görünen metin | `"Ad"` |
| `Properties.ValueMember` | seçilen değer (anahtar) | `"Id"` |
| `Properties.CascadePopupSize` | alt liste boyutu | Designer |
| `CascadingOwner` / `CascadingMember` | kademeli bağ | §1 |
| `EditValue` | seçili değer | `null` = boş |

```csharp
lookupIl.Properties.DisplayMember = "Ad";
lookupIl.Properties.ValueMember   = "Id";
lookupIl.Properties.DataSource    = LoadIller();
```

## §4 Grid Icinde (RepositoryItem)

Kabuk `ShownEditor` olayında, satırdaki üst değere göre alt lookup'ın kaynağını doldur:

```csharp
private void grid_ShownEditor(object sender, EventArgs e)
{
    if (gridView1.FocusedColumn.FieldName != "IlceId") return;
    var edit = gridView1.ActiveEditor as LookUpEdit;
    if (edit != null)
        edit.Properties.DataSource = LoadIlceler(SatirIlId());
}
```

| Adim | Sorumlu | Not |
|---|---|---|
| Üst kolon editörü | `RepositoryItemLookUpEdit` | Designer (DX-10) |
| Alt kolon kaynağı | `ShownEditor` + `ActiveEditor` | yalnız odaklanınca yüklenir |
| DB sorgusu | repository (`@p1`) | string-concat **yok** (G2) |
| Doğrulama | `ValidateRow` ([grid-editing-modes](grid-editing-modes.md)) | üst/alt tutarlılığı |
| Kaydetme | tek `Execute` + transaction | bkz. [datasource-binding-dapper](datasource-binding-dapper.md) |

## §5 net48 ve net10.0-windows'da fark

| Konu | net48 | net10.0-windows |
|---|---|---|
| Dil sürümü | **C# 7.3** — `using var` **yasak**, `as` + `if` (§4) zorunlu | C# 14 (paylaşılan dosya 7.3) |
| `Convert.ToInt32(EditValue)` | `EditValue` `DBNull` olabilir → önce `== null` kontrolü | aynı |
| DevExpress editor | `DevExpress.XtraEditors` 26.1 | aynı (aynı CPM satırı) |
| Binding | `App.config` redirect 26.1 | redirect gerekmez |
| Dapper | `Query<T>` + anonim parametre | aynı |

> **YASAK:** alt listeyi tamamını çekip istemcide süzmek (5.000+ satır) ve `CascadingOwner` yerine EF `Include` (G2).

## §6 Kardes Dosyalar

| Konu | Dosya |
|---|---|
| Grid düzenleme | [grid-editing-modes](grid-editing-modes.md) |
| Veri kaynağı (Dapper) | [datasource-binding-dapper](datasource-binding-dapper.md) |
| TreeList düzenleme | [treelist-inplace-editing](treelist-inplace-editing.md) |
| Arama/filtre kavramı | [grid-filtering-sorting](grid-filtering-sorting.md) |
