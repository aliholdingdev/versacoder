---
title: "concurrency-optimistic - VersaCoder"
type: knowledge
category: data
status: active
version: 1.0.0
updated: 2026-09-29
authority: primary
scope: "Optimist eszamanlilik: Version kolonu, WHERE .. AND Version, etkilenen satir sayisi, cakismada hata ve sinirli retry"
tags: [knowledge]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# concurrency-optimistic - Optimist Eszamanlilik

**Zorunlu Baglantilar:** [[knowledge/index|vault indeks]] · [[.rules/error-handling-rules|error-handling-rules]] · [[.rules/async-rules|async-rules]] · [[.rules/testing-rules|testing-rules]] · [[knowledge/data/audit-columns-convention|audit-columns-convention]] · [[knowledge/data/ado-net-command|ado-net-command]]
**Küme + Hub:** [[knowledge/data/index|data index]] · [[index|index]] · [[claude|CLAUDE]] · [[definitions/agents|AGENTS]] · [[.decisions/brain|brain]] · [[definitions/glossary|glossary]] · [[governance/keys|keys]]
**Kardeşler (data · 6/25):** [[knowledge/data/ado-net-connection|ado-net-connection]] · [[knowledge/data/connection-pooling-notes|connection-pooling-notes]] · [[knowledge/data/connection-string-secrets|connection-string-secrets]] · [[knowledge/data/dapper-async-patterns|dapper-async-patterns]] · [[knowledge/data/dapper-multi-map|dapper-multi-map]] · [[knowledge/data/dapper-procedures-stored|dapper-procedures-stored]] · [[knowledge/data/dual-tfm-provider-choice|dual-tfm-provider-choice]] · [[knowledge/data/migration-rollback|migration-rollback]] · [[knowledge/data/migration-script-writing|migration-script-writing]] · [[knowledge/data/migration-version-table|migration-version-table]] · [[knowledge/data/null-handling-db-mapping|null-handling-db-mapping]] · [[knowledge/data/pagination-offset-keyset|pagination-offset-keyset]] · [[knowledge/data/parameterized-queries|parameterized-queries]] · [[knowledge/data/repository-pattern-thin|repository-pattern-thin]] · [[knowledge/data/sql-injection-defense|sql-injection-defense]] · [[knowledge/data/sqlite-provider-setup|sqlite-provider-setup]] · [[knowledge/data/sqlserver-provider-setup|sqlserver-provider-setup]] · [[knowledge/data/transaction-scope-local|transaction-scope-local]] · [[knowledge/data/unit-of-work-pattern|unit-of-work-pattern]]

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
| `CreatedAt` / `UpdatedAt` / `DeletedAt` | [[knowledge/data/audit-columns-convention|audit-columns-convention]] |
| `Version` arttiriminin parametreli yazimi | [[knowledge/data/dapper-parameters-dynamic|dapper-parameters-dynamic]] |
| `affected` sayisinin okunmasi | [[knowledge/data/ado-net-command|ado-net-command]] |
| `throw;` ile hata yaysimi | [[.rules/error-handling-rules|error-handling-rules]] |

---

## §6 Kardesler

[[knowledge/data/audit-columns-convention|audit-columns-convention]] · [[knowledge/data/ado-net-command|ado-net-command]] · [[knowledge/data/dapper-parameters-dynamic|dapper-parameters-dynamic]] · [[knowledge/data/dapper-query-mapping|dapper-query-mapping]] · [[knowledge/data/command-timeout-policy|command-timeout-policy]]

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
