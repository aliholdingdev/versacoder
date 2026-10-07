---
title: "grid-editing-modes - VersaCoder"
type: knowledge
category: devexpress
status: active
version: 1.0.0
updated: 2026-09-30
authority: SSOT
scope: "GridView duzenleme modlari, RepositoryItem editörleri, hucre/satir dogrulamasi ve Dapper ile kayit yazma"
---

# Grid Duzenleme Modlari ve Dogrulama

**Zorunlu Baglantilar:** [vault indeks](../index.md) · [devexpress-rules](../../.rules/devexpress-rules.md) · [data-access-rules](../../.rules/data-access-rules.md) · [gridcontrol-basics](gridcontrol-basics.md) · [lookUpEdit-cascading](lookupedit-cascading.md) · [grid-paging-virtual-mode](grid-paging-virtual-mode.md)
**Küme + Hub:** [devexpress index](index.md) · [index](../../index.md) · [CLAUDE](../../claude.md) · [AGENTS](../../definitions/agents.md) · [brain](../../.decisions/brain.md) · [glossary](../../definitions/glossary.md) · [keys](../../governance/keys.md)
**Kardeşler (devexpress 6/35):** tam liste → [index](index.md) (kompakt kip — dosya 8 KB bandını korumak için)

> **Tek cumlelik sorumluluk:** *Hucre nasil duzenlenir, ne zaman kaydedilir ve hatali veri nerede yakalanir* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Duzenleme uc karardir: **acik/kapali** (`OptionsBehavior.Editable`), **editör tipi** (`col.ColumnEdit` = `RepositoryItem`) ve **kaydetme ani**. Grid'deki degisiklik once bellekteki `DataTable`'i yazar; veritabanina Dapper `UPDATE` ile sen gecersin - bu yuzden "grid degisti" ile "veritabanina yazildi" ayni sey degildir. Dogrulama iki katmandir: hucre kapanirken `ValidatingEditor`, satir tamamlanirken `ValidateRow`.

```text
+------------------------------------------------------------------+
| 1) OptionsBehavior.Editable  -> duzenleme acik / kapali           |
| 2) col.ColumnEdit            -> hucre editör tipi (Repository)    |
| 3) ValidatingEditor          -> tek hucre kurali (kirmizi satir)  |
| 4) ValidateRow               -> satir kurali                      |
| 5) Dapper Execute (@p1,@p2)  -> DB'ye yazma; string-concat YOK    |
+------------------------------------------------------------------+
```

---

## §1 Mod secimi

| Mod | Kurulum | Ne zaman kullanilir |
|---|---|---|
| Salt okunur | `gridView1.OptionsBehavior.Editable = false` | rapor / log ekrani |
| Hucre duzenlemeli | `Editable = true` + `EnterMoveNextColumn = true` | hizli giris (fis) |
| Satir duzenlemeli | `OptionsEdit` + `UpdateCurrentRow()` ile kilit | kart formu |
| Satir ekleme | `OptionsView.NewItemRowPosition = Top/Bottom` | yeni kayit satiri |
| Coklu secimde toplu | `OptionsSelection.MultiSelect` + repository sabit | toplu durum degisimi |

---

## §2 RepositoryItem editör eslesmesi

| Kolon tipi | RepositoryItem | Ayarlanan ozellik |
|---|---|---|
| metin | `RepositoryItemTextEdit` | `MaxLength`, `Mask` |
| secim listesi | `RepositoryItemComboBoxEdit` | `Items` / `DataSource` |
| ilgili kayit | `RepositoryItemLookUpEdit` | `DataSource`, `ValueMember` (bkz. [lookUpEdit-cascading](lookupedit-cascading.md)) |
| evet/hayir | `RepositoryItemCheckEdit` | `ValueChecked` / `ValueUnchecked` |
| sayi / tarih | `RepositoryItemSpinEdit` / `RepositoryItemDateEdit` | `MinValue`, `EditMask` |

```csharp
colTutar.ColumnEdit = new RepositoryItemSpinEdit();
((RepositoryItemSpinEdit)colTutar.ColumnEdit).EditMask = "n2";
gridView1.OptionsBehavior.Editable = true;
```

---

## §3 Dogrulama katmanlari

| Olay | Ne zaman | Sorumluluk |
|---|---|---|
| `ValidatingEditor` | hucre editöründen cikilirken | tek hucre kurali, `e.ErrorText` |
| `CellValueChanging` | deger degismeden once | iptal (`e.Cancel = true`) |
| `CellValueChanged` | deger degisti | UI hesabini tazele, DB'ye **yazma** |
| `ValidateRow` / `InvalidRowException` | satir tamamlanirken | satir kurali + hata mesaji |

```csharp
private void grid_ValidatingEditor(object sender, BaseContainerValidateEditorEventArgs e)
{
    if (gridView1.FocusedColumn.FieldName == "Tutar" && Convert.ToDecimal(e.Value) < 0)
        e.ErrorText = "Tutar 0 dan kucuk olamaz.";   // hucre kirmizi olur
}

private void grid_ValidateRow(object sender, ValidateRowEventArgs e) { e.Valid = KuralSagla(); }
```

---

## §4 Kayit yazma akisi (Dapper)

```csharp
conn.Execute("UPDATE dbo.Fatura SET Tutar = @p1 WHERE Id = @p2",
             new { p1 = tutar, p2 = id }, commandTimeout: 30);
gridControl1.RefreshDataSource();
```

| Adim | Kural |
|---|---|
| SQL | parametreli (`@p1`, `@p2`); string-concat **yok** (G2) |
| Zaman | kaydet butonu + tek `Execute` (transaction); `CellValueChanged` icinde degil |
| Sonrasi | `RefreshDataSource()` ile grid tazelenir |
| Yasak | EF Core `SaveChanges` / `AddOrUpdate` gibi DbContext katmani (G2) |

> **UYARI:** `CellValueChanged` icinde DB yazma - kullanici satirdan daha cikabilir; olay yalniz UI hesabini tazeler.

---

## §5 Kim neyi degistirir

| Degisiklik | Nereye dokunulur | Etki |
|---|---|---|
| Duzenleme kapatilacaksa | `OptionsBehavior.Editable` | tum editorler kapanir |
| Hucre tipi degisecekse | `col.ColumnEdit` (Designer) | yalniz o sutun |
| Kural degisecekse | `ValidatingEditor` / `ValidateRow` | tek hucre / tek satir |
| Kayit ani degisecekse | kaydet butonu + repository metodu | veri butunlugu |
| Ondalik/kultur | `CultureInfo.CurrentCulture` (`tr-TR`) | `Convert.ToDecimal` sonucu |

---

## §6 net48 ve net10.0-windows'da fark

| Konu | net48 | net10.0-windows |
|---|---|---|
| Dil surumu | **C# 7.3** - `out` degisken atama C# 7 ile var, `using var` **yasak** | C# 14 (paylasilan dosya 7.3) |
| `Convert.ToDecimal` | kulture bagli - `tr-TR` giriste ondalik virgul | ayni; `CurrentCulture` acik set et |
| DevExpress editörleri | 26.1 repository tipleri (ayni) | ayni |
| Veritabani yazma | Dapper `Execute` + `commandTimeout` | ayni |

---

## §7 Kardes dosyalar

| Konu | Dosya |
|---|---|
| Grid iskeleti | [gridcontrol-basics](gridcontrol-basics.md) |
| Editör baglama (kademeli) | [lookUpEdit-cascading](lookupedit-cascading.md) |
| Kolon ve bicim | [gridview-columns-format](gridview-columns-format.md) |
| Sayfalama + sanal mod | [grid-paging-virtual-mode](grid-paging-virtual-mode.md) |
| Veri kaynagi (Dapper) | [datasource-binding-dapper](datasource-binding-dapper.md) |

---

## §8 Denetim listesi

| # | Adim | Kanit |
|---|---|---|
| 1 | Mod ekrana gore secilmis (`Editable` degeri) | kod incelemesi |
| 2 | Her kolonun `ColumnEdit`'i tipine uygun | Designer editör listesi |
| 3 | `CellValueChanged` icinde DB yazma yok | grep |
| 4 | SQL parametreli, string-concat yok | kod taramasi |
| 5 | Hucre + satir dogrulamasi ikisi de bagli | olay listesi |

---

## §9 Kanit

| Kaynak dosya | Ne alindi |
|---|---|
| `knowledge\devexpress\grid-editing-modes.md` (kaynak vault) | mod secimi, RepositoryItem tablosu, dogrulama olaylari, Dapper kayit ornegi, TFM farki |
| `.rules\data-access-rules.md` | parametreli sorgu (G2), EF Core yasagi |

> Kanitsiz iddia icin DOĞRULAMA GEREKLI yazilir; bu dosyada kaynak disi bilgi yoktur.
