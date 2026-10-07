---
title: "audit-columns-convention - VersaCoder"
type: knowledge
category: data
status: active
version: 1.0.0
updated: 2026-09-29
authority: SSOT
scope: "Audit kolonlari: CreatedAt / UpdatedAt / DeletedAt UTC ISO-8601, soft delete, TimeProvider tek kaynagi (DA-15)"
---

# audit-columns-convention - Audit Kolon Sozlesmesi

**Zorunlu Baglantilar:** [vault indeks](../index.md) · [coding-standards](../../.rules/coding-standards.md) · [solid-rules](../../.rules/solid-rules.md) · [error-handling-rules](../../.rules/error-handling-rules.md) · [concurrency-optimistic](concurrency-optimistic.md) · [dapper-parameters-dynamic](dapper-parameters-dynamic.md)
**Küme + Hub:** [data index](index.md) · [index](../../index.md) · [CLAUDE](../../claude.md) · [AGENTS](../../definitions/agents.md) · [brain](../../.decisions/brain.md) · [glossary](../../definitions/glossary.md) · [keys](../../governance/keys.md)
**Kardeşler (data · 6/25):** [ado-net-connection](ado-net-connection.md) · [connection-pooling-notes](connection-pooling-notes.md) · [connection-string-secrets](connection-string-secrets.md) · [dapper-async-patterns](dapper-async-patterns.md) · [dapper-multi-map](dapper-multi-map.md) · [dapper-procedures-stored](dapper-procedures-stored.md) · [dual-tfm-provider-choice](dual-tfm-provider-choice.md) · [migration-rollback](migration-rollback.md) · [migration-script-writing](migration-script-writing.md) · [migration-version-table](migration-version-table.md) · [null-handling-db-mapping](null-handling-db-mapping.md) · [pagination-offset-keyset](pagination-offset-keyset.md) · [parameterized-queries](parameterized-queries.md) · [repository-pattern-thin](repository-pattern-thin.md) · [sql-injection-defense](sql-injection-defense.md) · [sqlite-provider-setup](sqlite-provider-setup.md) · [sqlserver-provider-setup](sqlserver-provider-setup.md) · [transaction-scope-local](transaction-scope-local.md) · [unit-of-work-pattern](unit-of-work-pattern.md)

> **Tek cumlelik sorumluluk:** *Hangi tabloda hangi denetim kolonu bulunur, zaman nereden gelir, soft delete nasil yazilir* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Kritik tablolarda uc kolon bulunur: **`CreatedAt`**, **`UpdatedAt`** ve gerekiyorsa **`DeletedAt`** - hepsi **UTC, ISO-8601** biciminde (`2026-09-29T10:30:00Z`). Zaman **tek kaynaktan** gelir: `TimeProvider` ya da vault'taki saat soyutlamasi; `DateTime.Now` / `DateTime.UtcNow` dogrudan cagrilamaz (DA-15). Silme yikici degildir: `DeletedAt` doldurulur ve tum sorgular `DeletedAt IS NULL kosulu tasir; fiziksel silme yalniz acik onayla yapilir (G7).

```csharp
var now = _timeProvider.GetUtcNow().UtcDateTime;      // TEK KAYNAK (DA-15)

const string insertSql = @"
    INSERT INTO Sessions (Id, Title, CreatedAt, UpdatedAt)
    VALUES (@Id, @Title, @CreatedAt, @UpdatedAt)";

await connection.ExecuteAsync(insertSql, new
{
    Id = id, Title = title, CreatedAt = now, UpdatedAt = now
}, commandTimeout: DbTimeouts.Query, cancellationToken: cancellationToken);
```

---

## §1 Kolon sozlesmesi

| Kolon | Tip | Zorunlu | Dolduran |
|---|---|---|---|
| `Id` | `TEXT` (GUID) / `uniqueidentifier` | evet | uygulama |
| `CreatedAt` | ISO-8601 UTC | evet | INSERT |
| `UpdatedAt` | ISO-8601 UTC | evet | her UPDATE |
| `DeletedAt` | ISO-8601 UTC, `NULL` | soft delete tablolarinda | DELETE yerine UPDATE |
| `CreatedBy` / `UpdatedBy` | `TEXT` / `UNIQUEIDENTIFIER` | gerekiyorsa | kimlik baglami |
| `Version` | `INTEGER` | cakisma korumali tablolar | UPDATE (`+1`) |

---

## §2 Kural tablosu (DA-15)

| # | Kural | Yasak |
|---|---|---|
| 1 | Saat = `TimeProvider` (tek kaynak) | `DateTime.Now` / `DateTime.UtcNow` |
| 2 | Bicim ISO-8601, **UTC** (`Z` eki) | yerel saat, `dd.MM.yyyy` bicimi |
| 3 | `UpdatedAt` her UPDATE'te guncellenir | guncellenmeyen satir |
| 4 | Soft delete + `DeletedAt IS NULL` filtresi | dogrudan `DELETE` |
| 5 | Fiziksel silme onayli (G7) + yedek | onaysiz `DELETE FROM` |

---

## §3 net48 / net10.0-windows farki

| Konu | `net48` | `net10.0-windows` |
|---|---|---|
| `TimeProvider` | BCL'de **yok** -> vault'ta ortak arayuz (`IClock`) ya da paket | .NET 8+ **yerlesik** `TimeProvider` |
| Ortak kod | arayuz iki bacakta ortak, `#if` gerekmez | ayni arayuz |
| Tarih saklama | SQLite `TEXT` (test) / SQL Server `datetime2` (uretim) | ayni sozluk |
| `DateTimeKind` | `Utc` korunur, `ToUniversalTime` ile cevrilmez | ayni |

> **NOT:** `net48`'de `TimeProvider` bulunmadigi icin tek ortak arayuz tanimlanir (`IClock.GetUtcNow()`); tek uygulama, `#if` yok (ADR-001).

---

## §4 Soft delete akisi

| Adim | Islem | Not |
|---|---|---|
| 1 | Silme istegi gelir | YIKICI `DELETE` calistirilmaz |
| 2 | `DeletedAt = @Now` yazilir | `UpdatedAt` da guncellenir |
| 3 | Tum okumalar `DeletedAt IS NULL` tasir | filtre tek yerde, helper ile |
| 4 | Geri alma | `DeletedAt = NULL` yazilir |
| 5 | Kalici silme | acik onay + yedek (G7) |

Fiziksel silme disindaki her adim parametreli yazilir; tablo ve kolon adlari sabit metin, degerler `@ad` parametresidir (DA-03).

---

## §5 Cakisma ile iliski

| Durum | Davranis |
|---|---|
| Ayni satira es zamanli iki guncelleme | `Version` kontrolu ile yakalanir |
| `UpdatedAt` tek basina imza olarak kullanilir | hassas saat esitligi gerekir, tercih edilmez |
| Audit kolonu guncellenmeden guncelleme | veri butunlugu kirilir, `reviewer` RED |

Ayranti kardes dosyada: [concurrency-optimistic](concurrency-optimistic.md).

---

## §6 Kardesler

[concurrency-optimistic](concurrency-optimistic.md) · [dapper-parameters-dynamic](dapper-parameters-dynamic.md) · [dapper-query-mapping](dapper-query-mapping.md) · [ado-net-command](ado-net-command.md) · [command-timeout-policy](command-timeout-policy.md)

---

## §7 Denetim ve test

| Kontrol | Kanit |
|---|---|
| `DateTime.Now` aramasi repoda = **0** | kod taramasi |
| `DeletedAt IS NULL` filtresi her okumada | sorgu incelemesi |
| INSERT ve UPDATE audit kolonlari dolduruyor | entegrasyon testi |
| Biçim ISO-8601 UTC (`Z` eki) | kayit ornegi |
| Fiziksel silme onay akisi gecmis | G7 kaydi |

---

## §8 Kisa karar

UC kolon + tek saat kaynagi + soft delete. `DateTime.Now` yazan kod, olcum edilemeyen ve iki bacakta farkli davranan koddur; `TimeProvider` disindaki her zaman cagrisi incelemede gerekcelendirilmek zorundadir.

---

## §9 Kanit

| Kaynak dosya | Ne alindi |
|---|---|
| `audit-columns-convention.md` | kolon sozlesmesi, DA-15 kural tablosu, TimeProvider iki bacak notu, soft delete adimlari |
| `concurrency-optimistic.md` | `Version` kolonu ve cakisma yakalama |
| `dapper-parameters-dynamic.md` | anonim nesne ile parametreleme ornegi |
| `ado-net-command.md` | sabit `CommandText` + `@ad` kurali (DA-03 / DA-06) |
| `dual-tfm-provider-choice.md` | `#if` disiplini ve ortak arayuz yaklasimi |

> Kanitsiz iddia icin `DOĞRULAMA GEREKLI` yazilir; bu dosyada kaynak disi bilgi yoktur.
