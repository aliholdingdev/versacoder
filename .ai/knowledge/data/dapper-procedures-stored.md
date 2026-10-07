---
title: "dapper-procedures-stored - VersaCoder"
type: knowledge
category: data
status: active
version: 1.0.0
updated: 2026-09-29
authority: SSOT
scope: "Depolama proseduru politikasi ve Dapper ile cagrim deseni; SQLite test bacaginda uyum notu"
---

# dapper-procedures-stored - Depolama Prosedurleri

**Zorunlu Baglantilar:** [vault indeks](../index.md) · [ADR-005](../../.decisions/accepted/adr-005-ado-net-dapper-no-ef-core.md) · [ADR-001](../../.decisions/accepted/adr-001-dual-target-net48-net10.md) · [ado-net-command](ado-net-command.md) · [dapper-parameters-dynamic](dapper-parameters-dynamic.md) · [dual-tfm-provider-choice](dual-tfm-provider-choice.md)
**Küme + Hub:** [data index](index.md) · [index](../../index.md) · [CLAUDE](../../claude.md) · [AGENTS](../../definitions/agents.md) · [brain](../../.decisions/brain.md) · [glossary](../../definitions/glossary.md) · [keys](../../governance/keys.md)
**Kardeşler (data · 6/25):** [ado-net-connection](ado-net-connection.md) · [audit-columns-convention](audit-columns-convention.md) · [concurrency-optimistic](concurrency-optimistic.md) · [connection-pooling-notes](connection-pooling-notes.md) · [connection-string-secrets](connection-string-secrets.md) · [dapper-async-patterns](dapper-async-patterns.md) · [dapper-query-mapping](dapper-query-mapping.md) · [migration-rollback](migration-rollback.md) · [migration-script-writing](migration-script-writing.md) · [migration-version-table](migration-version-table.md) · [null-handling-db-mapping](null-handling-db-mapping.md) · [pagination-offset-keyset](pagination-offset-keyset.md) · [parameterized-queries](parameterized-queries.md) · [repository-pattern-thin](repository-pattern-thin.md) · [sql-injection-defense](sql-injection-defense.md) · [sqlite-provider-setup](sqlite-provider-setup.md) · [sqlserver-provider-setup](sqlserver-provider-setup.md) · [transaction-scope-local](transaction-scope-local.md) · [unit-of-work-pattern](unit-of-work-pattern.md)

> **Tek cumlelik sorumluluk:** *VersaCoder'da depolama proseduru kullanilir mi; kullanilirsa Dapper ile nasil cagrilir ve iki saglayicida nasil tasınir* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

**Varsayilan: hayir.** Is mantigi prosedure tasınmaz; sorgu govdesi repository'de sabit, parametreli SQL olarak durur - cunku test saglayicisi **SQLite'tir ve SQLite'in depolama proseduru yoktur**. Prosedur zorunlu bir kurumsal istekse cagrim deseni `CommandType.StoredProcedure` + parametreli `DynamicParameters` + acik `CommandTimeout` olur ve bu desen, sahte ya da denetlenebilir bir **dialect koprusu** ile repository arkasinda gizlenir. Prosedurle esdeger SQLite dalinin nasil yazilacagi mimari karar ister: **DOĞRULAMA GEREKLI**.

```csharp
const string proc = "usp_Sessions_ListRecent";

var p = new DynamicParameters();
p.Add("Top", take, DbType.Int32);
p.Add("Since", sinceUtc, DbType.DateTime2);

var rows = await connection.QueryAsync<SessionRow>(
    new CommandDefinition(proc, p, commandType: CommandType.StoredProcedure,
        commandTimeout: DbTimeouts.Report, cancellationToken: cancellationToken));
```

---

## §1 Politika tablosu

| Durum | Karar | Gerekce |
|---|---|---|
| Yeni is | **parametreli metin SQL** | tasınabilir, denetlenebilir |
| Is kurali mantigi | repository / servis katmaninda | prosedurde is mantigi = gorunmeyen kod |
| Sema degisikligi | migration dosyasi | `ALTER PROCEDURE` kodda yasak |
| Kurumsal zorunluluk | `CommandType.StoredProcedure` + desen | tek noktada gizleme |
| SQLite entegrasyon testi | ayni repo imzasi, farkli govde | test dali farkli saglayici kullanir |

---

## §2 Cagrim kurallari

| # | Kural | Yasak |
|---|---|---|
| 1 | Prosedur adi **sabit metin** (`const string`) | adi interpolasyonla kurmak |
| 2 | Tum argumanlar parametre (`DynamicParameters`) | degeri ad icine gommek |
| 3 | `CommandTimeout` acik (`DbTimeouts.Report` = 60) | `0` |
| 4 | Donus degeri / cikti parametresi `Get<T>` ile alinir | sessiz `DBNull` |
| 5 | Prosedur surumu migration'da (`CREATE OR ALTER`) | kod icinde `ALTER` |

---

## §3 Saglayici karsilastirmasi

| Konu | SQL Server (uretim) | SQLite (test) |
|---|---|---|
| Depolama proseduru | var | **yok** |
| Govde dili | T-SQL | SQL - SQL |
| Cagrim | `CommandType.StoredProcedure` | metin SQL |
| Sema dosyasi | migration icinde `CREATE OR ALTER` | migration icinde `CREATE TABLE` |
| Ayni repo imzasi | evet | evet - yalniz govde farklidir |

Kalin cizgi: prosedur, repository imzasini degistirmez; yalniz dialect kaydini degistirir.

---

## §4 net48 / net10.0-windows farki

| Konu | `net48` | `net10.0-windows` |
|---|---|---|
| `CommandType.StoredProcedure` | var (SQL Client 5.2.x) | var (ayni paket) |
| SQLite saglayicisi | **yok** - prosedur cagrisi imkansiz | prosedur **yok** -> metin SQL dali |
| Cift implementasyon | yalniz `#if` degil, **dialect secimi** ile | ayni |
| Parametre yonlendirme | `DbType` + `ParameterDirection` | ayni |

> **NOT:** Prosedur cagrisi ile metin-SQL dali arasindaki fark `#if` ile degil, **dialect / repository secimi** ile yonetilir; `#if` yalniz platform API farki icindir (ADR-001).

---

## §5 Kardesler

[ado-net-command](ado-net-command.md) · [dapper-parameters-dynamic](dapper-parameters-dynamic.md) · [dual-tfm-provider-choice](dual-tfm-provider-choice.md) · [command-timeout-policy](command-timeout-policy.md) · [dapper-multi-map](dapper-multi-map.md)

---

## §6 Kisa karar

Yeni kodda prosedur yok; govde sabit + parametreli metin SQL. Prosedur zorunluysa `StoredProcedure` + `DynamicParameters` + `DbTimeouts.Report`, ve bu cagri dialect arkasinda gizlenir ki SQLite test dali calisabilsin.

---

## §7 Teshis

| Belirti | Kok neden | Duzeltme |
|---|---|---|
| Test dali prosedurde patliyor | SQLite prosedur desteklemez | metin SQL dali yaz |
| Prosedur adi composition ile kurulmus | sabit degil | `const string` yap |
| Cagri donuyor, deger null | cikti parametresi okunmamis | `Get<T>` kullan |
| Uzun sure timeout | `Report` yerine `Query` | `DbTimeouts.Report` |
| Kodda `ALTER PROCEDURE` | sema kodda | migration'a tasi |

---

## §8 Doğrulama

| Kontrol | Kanit |
|---|---|
| Prosedur adi interpolasyonu = **0** | kod taramasi |
| `CommandTimeout = 0` prosedur cagrisinda = **0** | kod taramasi |
| Sema degisikligi migration dosyasinda | migration incelemesi |
| SQLite test dali ayni imzayla geciyor | entegrasyon testi |

> Prosedurle esdeger SQLite dalinin mimari sekli karar oncesidir: **DOĞRULAMA GEREKLI**.

---

## §9 Kanit

| Kaynak dosya | Ne alindi |
|---|---|
| `dapper-procedures-stored.md` | varsayilan "hayir", politika ve cagrim tablolari, SQLite prosedur kisiti, `#if` yerine dialect |
| `ado-net-command.md` | `CommandType.StoredProcedure` karsilastirmasi |
| `dapper-parameters-dynamic.md` | `DynamicParameters` ve cikti parametresi yazimi |
| `command-timeout-policy.md` | `DbTimeouts.Report` (60 sn) degeri |
| `dual-tfm-provider-choice.md` | SQLite yalniz net10 test, `#if` disiplini |

> Kanitsiz iddia icin `DOĞRULAMA GEREKLI` yazilir; bu dosyada kaynak disi bilgi yoktur.
