---
title: "concurrency-optimistic - VersaCoder"
type: knowledge
category: data
status: active
version: 1.0.0
updated: 2026-09-29
authority: SSOT
scope: "Optimist eszamanlilik: Version kolonu, WHERE .. AND Version, etkilenen satir sayisi, cakismada hata ve sinirli retry"
---

# concurrency-optimistic - Optimist Eszamanlilik

**Zorunlu Baglantilar:** [vault indeks](../index.md) · [error-handling-rules](../../.rules/error-handling-rules.md) · [async-rules](../../.rules/async-rules.md) · [testing-rules](../../.rules/testing-rules.md) · [audit-columns-convention](audit-columns-convention.md) · [ado-net-command](ado-net-command.md)
**Küme + Hub:** [data index](index.md) · [index](../../index.md) · [CLAUDE](../../claude.md) · [AGENTS](../../definitions/agents.md) · [brain](../../.decisions/brain.md) · [glossary](../../definitions/glossary.md) · [keys](../../governance/keys.md)
**Kardeşler (data · 6/25):** [ado-net-connection](ado-net-connection.md) · [connection-pooling-notes](connection-pooling-notes.md) · [connection-string-secrets](connection-string-secrets.md) · [dapper-async-patterns](dapper-async-patterns.md) · [dapper-multi-map](dapper-multi-map.md) · [dapper-procedures-stored](dapper-procedures-stored.md) · [dual-tfm-provider-choice](dual-tfm-provider-choice.md) · [migration-rollback](migration-rollback.md) · [migration-script-writing](migration-script-writing.md) · [migration-version-table](migration-version-table.md) · [null-handling-db-mapping](null-handling-db-mapping.md) · [pagination-offset-keyset](pagination-offset-keyset.md) · [parameterized-queries](parameterized-queries.md) · [repository-pattern-thin](repository-pattern-thin.md) · [sql-injection-defense](sql-injection-defense.md) · [sqlite-provider-setup](sqlite-provider-setup.md) · [sqlserver-provider-setup](sqlserver-provider-setup.md) · [transaction-scope-local](transaction-scope-local.md) · [unit-of-work-pattern](unit-of-work-pattern.md)

> **Tek cumlelik sorumluluk:** *Ayni satira iki istek ayni anda yazarsa veri kaybi nasil onlenir, cakisma nasil anlasilir* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Kilit **yok**; bunun yerine **optimist** yontem kullanilir: her degistirilebilir satirda `Version` (int) kolonu tasinir, `UPDATE ... WHERE Id = @Id AND Version = @Version` calistirilir ve **etkilenen satir sayisi 0 ise** baska bir kullanici yazmis demektir -> domain hatasi + kullaniciya "veri baska biri tarafindan degistirildi" mesaji. Kilitli satir (`SELECT ... FOR UPDATE`) **kullanilmaz**: uzun transaction ekrani ve havuzu baglar. Retry **otomatik degildir**, acik ve sinirlidir: en fazla 1 kez, ayni veri yeniden okunarak.

```csharp
const string sql = @"
    UPDATE Sessions
       SET Title = @Title, Version = Version + 1, UpdatedAt = @UpdatedAt
     WHERE Id = @Id AND Version = @Version AND DeletedAt IS NULL";

var affected = await connection.ExecuteAsync(sql,
    new { Id = id, Title = title, Version = expectedVersion, UpdatedAt = nowUtc },
    commandTimeout: DbTimeouts.Query, cancellationToken: cancellationToken);

if (affected == 0)
    throw new ConcurrencyConflictException(nameof(Session), id);   // baskasi yazdi
```

---

## §1 Yontem karsilastirmasi

| Yontem | Nasil | Arti | Eksi |
|---|---|---|---|
| **`Version` kolonu** (secilen) | `WHERE Version = @V` + sayac arttirimi | saf SQL, iki saglayicida ayni | her tabloda kolon |
| `UpdatedAt` imzasi | `WHERE UpdatedAt = @E` | kolon eklemeye gerek | hassas saat esitligi |
| `ROWVERSION` (SQL Server) | donanim uretimi | guvenilir | **SQLite'ta yok** -> saglayiciya bagimli |
| Pesimistik kilit | `FOR UPDATE` / transaction | cakisma yok | ekran bloklar, havuz riski |
| Son yazan kazanir | `WHERE` yok | - | **veri kaybi - yasak** |

---

## §2 Kural tablosu

| # | Kural | Yasak |
|---|---|---|
| 1 | Guncelleme ve silme her zaman `WHERE` + durum kosulu | kosulsuz `UPDATE` |
| 2 | Etkilenen satir 0 -> domain hatasi (sessiz gecis yok) | `affected` sayisini yok saymak |
| 3 | Ekranda acik cakisma mesaji | veriyi kore ezme |
| 4 | Retry: en fazla 1, veriyi yeniden okuyarak | kore dongu retry |
| 5 | `Version` degeri okumadan gelir | istemcinin uydurdugu surum |

---

## §3 net48 / net10.0-windows farki

| Konu | `net48` | `net10.0-windows` |
|---|---|---|
| Etkilenen satir (`ExecuteAsync`) | `int` - ayni | ayni |
| `ROWVERSION` | yalniz SQL Server dali | ayni - **dialect** icinde |
| SQLite versiyon kolonu | saglayici kullanilmaz | `INTEGER` sayac ile ayni desen |
| Hata tipi | `Exception` alt sinifi - iki bacakta ortak | ayni |
| Retry | elle (kod) | ayni - ek paket **yok** (YAGNI) |

---

## §4 Cakisma yasam dongusu

| Adim | Islem |
|---|---|
| 1 | Kayit okunur, `Version` istemciye gonderilir |
| 2 | Kullanici duzenler, form `Version`'i geri gonderir |
| 3 | UPDATE `WHERE Version = @beklenen` ile calisir |
| 4 | `affected == 0` -> cakisma; ekranda mesaj, veri yeniden okunur |
| 5 | Retry en fazla 1 kez; tekrar cakisma -> kullanici karari |

---

## §5 Ilgili kolonlar

Audit kolonlari bu desenin parcasidir; `UpdatedAt` her guncellemede yazilir, `DeletedAt IS NULL` sorgulara eslik eder. Ayranti kardes dosyadadir:

| Konu | Dosya |
|---|---|
| `CreatedAt` / `UpdatedAt` / `DeletedAt` | [audit-columns-convention](audit-columns-convention.md) |
| `Version` arttiriminin parametreli yazimi | [dapper-parameters-dynamic](dapper-parameters-dynamic.md) |
| `affected` sayisinin okunmasi | [ado-net-command](ado-net-command.md) |
| `throw;` ile hata yaysimi | [error-handling-rules](../../.rules/error-handling-rules.md) |

---

## §6 Kardesler

[audit-columns-convention](audit-columns-convention.md) · [ado-net-command](ado-net-command.md) · [dapper-parameters-dynamic](dapper-parameters-dynamic.md) · [dapper-query-mapping](dapper-query-mapping.md) · [command-timeout-policy](command-timeout-policy.md)

---

## §7 Kisa karar

`WHERE` + `Version` + etkilenen satir sayisi + tek seferlik retry. Kilit yok, kore retry yok, sessiz gecis yok. Cakisma hata degil, normal bir kullanici durumudur; ekranda acikca gosterilir.

---

## §8 Doğrulama

| Kontrol | Kanit |
|---|---|
| Kosulsuz `UPDATE` aramasi = **0** | kod taramasi |
| `affected == 0` her guncellemede ele aliniyor | birim testi |
| `Version` kolonu degistirilebilir tablolarda var | sema incelemesi |
| Cakisma mesaji ekranda goruluyor | UI testi |
| Retry sayisi kodda 1 ile sinirli | kod incelemesi |

> `ROWVERSION`'in SQLite test dalinda karsiligi mimari karar ister: **DOĞRULAMA GEREKLI**.

---

## §9 Kanit

| Kaynak dosya | Ne alindi |
|---|---|
| `concurrency-optimistic.md` | `Version` + `WHERE` deseni, yontem karsilastirmasi, retry kurali, iki bacak farki |
| `audit-columns-convention.md` | `UpdatedAt` / `DeletedAt` ve `TimeProvider` bagi |
| `ado-net-command.md` | etkilenen satir sayisi okumasi (`ExecuteNonQueryAsync` / `ExecuteAsync`) |
| `dapper-parameters-dynamic.md` | anonim nesne ile parametreleme |
| `command-timeout-policy.md` | her cagrida acik timeout |

> Kanitsiz iddia icin `DOĞRULAMA GEREKLI` yazilir; bu dosyada kaynak disi bilgi yoktur.
