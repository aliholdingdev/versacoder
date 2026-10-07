---
title: "lookUpEdit-cascading - VersaCoder"
type: knowledge
category: devexpress
status: active
version: 1.0.0
updated: 2026-09-30
authority: SSOT
scope: "LookUpEdit kademeli (cascading) baglanti: CascadingOwner, EditValueChanged ile Dapper yeniden yukleme ve grid icinde ShownEditor"
---

# Kademeli lookUpEdit (Cascading)

**Zorunlu Baglantilar:** [vault indeks](../index.md) · [devexpress-rules](../../.rules/devexpress-rules.md) · [data-access-rules](../../.rules/data-access-rules.md) · [grid-editing-modes](grid-editing-modes.md) · [datasource-binding-dapper](datasource-binding-dapper.md) · [grid-filtering-sorting](grid-filtering-sorting.md)
**Küme + Hub:** [devexpress index](index.md) · [index](../../index.md) · [CLAUDE](../../CLAUDE.md) · [AGENTS](../../AGENTS.md) · [brain](../../brain.md) · [glossary](../../glossary.md) · [keys](../../keys.md)
**Kardeşler (devexpress 4/35):** tam liste → [index](index.md) (kompakt kip — dosya 8 KB bandını korumak için)

> **Tek cumlelik sorumluluk:** *Il-Ilce gibi bagli listelerde alt liste nasil filtelenir, veri ne zaman Dapper ile yeniden yuklenir* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Iki yol vardir: **(a) bildirimsel** - `CascadingOwner` ile ana editörü tanimla, DevExpress anahtar alanini kendisi bulur (`CascadingMember` ile elle verilir); **(b) elle** - ana editörün `EditValueChanged` olayinda Dapper parametreli sorguyla alt listenin `DataSource`'unu tazeler. Kalici/ buyuk liste **(b)** (sorgu indeksli, tamami cekilmez); kucuk ve sabit iliski **(a)**. Grid icinde ise kabuk `ShownEditor` olayinda etkin editörün `Properties.DataSource`'i satira gore doldurulur.

```text
+------------------------------------------------------------------+
| (a) CascadingOwner / CascadingMember -> bildirimsel, kucuk liste  |
| (b) EditValueChanged -> Dapper @p1 -> alt liste yenilenir         |
| Grid ici: ShownEditor -> ActiveEditor.Properties.DataSource      |
| ZORUNLU: ust degisince alt EditValue = null                      |
+------------------------------------------------------------------+
```

---

## §1 Iki yol

| Yol | Kurulum | Ne zaman |
|---|---|---|
| (a) Bildirimsel | `alt.CascadingOwner = ust;` | iliski sabit, liste kucuk |
| (a) Elle anahtar | `alt.CascadingMember = "IlId";` | otomatik eslesme bulamazsa |
| (b) Elle yukleme | `EditValueChanged` -> Dapper `@p1` | liste buyuk / DB'den |
| (b) Popup'ta filtre | `PopupFilter` olayi | yalniz acilirken suremek istenirse |
| Grid ici | `gridView1.ShownEditor` | satir bazli kademeli liste |

```csharp
lookupIlce.CascadingOwner = lookupIl;          // alt -> ust baglantisi
lookupIlce.EditValue = null;                   // ust degisince alti bosalt
```

---

## §2 Elle yukleme (Dapper, parametreli)

```csharp
private void lookupIl_EditValueChanged(object sender, EventArgs e)
{
    lookupIlce.EditValue = null;
    int ilId = Convert.ToInt32(lookupIl.EditValue);
    lookupIlce.Properties.DataSource = LoadIlceler(ilId);
}

private static object LoadIlceler(int ilId)
{
    using (var conn = new SqlConnection(connString))
        return conn.Query("SELECT Id, Ad FROM dbo.Ilce WHERE IlId = @p1 ORDER BY Ad",
                          new { p1 = ilId }).ToList();
}
```

> **UYARI:** `EditValue = null` satiri **zorunlu** - ust deger degistiginde altta eski `Id` kalirsa `ValueMember` karsiliksiz kalir ve editör bos gorunur (kayitta hata uretmez, surpriz yapar).

---

## §3 Editör ozellikleri

| Ozellik | Ne yapar | Ornek |
|---|---|---|
| `Properties.DataSource` | liste kaynagi | `List<T>` / `DataTable` |
| `Properties.DisplayMember` | gorunen metin | `"Ad"` |
| `Properties.ValueMember` | secilen deger (anahtar) | `"Id"` |
| `Properties.CascadePopupSize` | alt liste boyutu | Designer |
| `CascadingOwner` / `CascadingMember` | kademeli bag | §1 |
| `EditValue` | secili deger | `null` = bos |

```csharp
lookupIl.Properties.DisplayMember = "Ad";
lookupIl.Properties.ValueMember   = "Id";
lookupIl.Properties.DataSource    = LoadIller();
```

---

## §4 Grid icinde (RepositoryItem)

Kabuk `ShownEditor` olayinda, satirdaki ust deger gore alt lookup'in kaynagini doldur:

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
| Ust kolon editörü | `RepositoryItemLookUpEdit` | Designer (DX-10) |
| Alt kolon kaynagi | `ShownEditor` + `ActiveEditor` | yalniz odaklaninca yuklenir |
| DB sorgusu | repository (`@p1`) | string-concat **yok** (G2) |
| Dogrulama | `ValidateRow` (bkz. [grid-editing-modes](grid-editing-modes.md)) | ust/alt tutarliligi |
| Kaydetme | tek `Execute` + transaction | bkz. [datasource-binding-dapper](datasource-binding-dapper.md) |

---

## §5 Kim neyi degistirir

| Degisiklik | Nereye dokunulur | Sonuc |
|---|---|---|
| Bildirimsel bag | `CascadingOwner` / `CascadingMember` | DevExpress kendi eslestirir |
| Yeniden yukleme | `EditValueChanged` handler | alt liste DB'den tazelenir |
| Grid ici bag | `ShownEditor` + `ActiveEditor` | satir bazli alt liste |
| Anahtar alanlar | `DisplayMember` / `ValueMember` | gorunen metin vs. anahtar |
| Buyuk liste | SQL `WHERE IlId = @p1` | istemciye tamami inmez |

> **YASAK:** alt listeyi tamamini cekip istemcide suremek (5.000+ satir - `DOĞRULAMA GEREKLI`) ve `CascadingOwner` yerine EF `Include` (G2).

---

## §6 net48 ve net10.0-windows'da fark

| Konu | net48 | net10.0-windows |
|---|---|---|
| Dil surumu | **C# 7.3** - `using var` **yasak**, `as` + `if` (§4) zorunlu | C# 14 (paylasilan dosya 7.3) |
| `Convert.ToInt32(EditValue)` | `EditValue` `DBNull` olabilir -> once `== null` kontrolu | ayni |
| DevExpress editör | `DevExpress.XtraEditors` 26.1 | ayni (ayni CPM satiri) |
| Binding | `App.config` redirect 26.1 | redirect gerekmez |
| Dapper | `Query<T>` + anonim parametre | ayni |

---

## §7 Kardes dosyalar

| Konu | Dosya |
|---|---|
| Grid duzenleme | [grid-editing-modes](grid-editing-modes.md) |
| Veri kaynagi (Dapper) | [datasource-binding-dapper](datasource-binding-dapper.md) |
| TreeList duzenleme | `treelist-hierarchical-data` (kardes dosya yok - G7) |
| Arama/filtre kavrami | [grid-filtering-sorting](grid-filtering-sorting.md) |

---

## §8 Denetim listesi

| # | Adim | Kanit |
|---|---|---|
| 1 | Ust degisince alt `EditValue = null` | kod grep |
| 2 | Alt liste sorgusu parametreli (`@p1`) | kod incelemesi |
| 3 | Tam liste cekme yok (istemci sureme) | sorgu + boyut denetimi |
| 4 | `DisplayMember`/`ValueMember` kaynak alanlarla ayni | Designer denetimi |
| 5 | Grid ici `ShownEditor` sadece ilgili kolonda | olay incelemesi |

---

## §9 Kanit

| Kaynak dosya | Ne alindi |
|---|---|
| `knowledge\devexpress\lookUpEdit-cascading.md` (kaynak vault) | iki yol karsilastirmasi, Dapper yukleme ornegi, editör ozellik tablosu, grid ici adim tablosu, TFM farki |
| `.rules\data-access-rules.md` | parametreli sorgu (G2), EF `Include` yasagi |

> Kanitsiz iddia icin DOĞRULAMA GEREKLI yazilir; bu dosyada kaynak disi bilgi yoktur.
