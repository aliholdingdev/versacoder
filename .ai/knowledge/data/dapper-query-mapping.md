---
title: "dapper-query-mapping - VersaCoder"
type: knowledge
category: data
status: active
version: 1.0.0
updated: 2026-09-29
authority: primary
scope: "Dapper ile satir esleme: QueryAsync<T>, satir tipleri, kolon - ozellik uyumu, custom TypeHandler (DA-05 / DA-08)"
tags: [knowledge]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# dapper-query-mapping - Dapper Satir Esleme

**Zorunlu Baglantilar:** [[knowledge/index|vault indeks]] · [[.decisions/accepted/adr-005-ado-net-dapper-no-ef-core|ADR-005]] · [[.rules/coding-standards|coding-standards]] · [[.rules/testing-rules|testing-rules]] · [[knowledge/data/dapper-parameters-dynamic|dapper-parameters-dynamic]] · [[knowledge/data/dapper-multi-map|dapper-multi-map]]
**Küme + Hub:** [[knowledge/data/index|data index]] · [[index|index]] · [[claude|CLAUDE]] · [[definitions/agents|AGENTS]] · [[.decisions/brain|brain]] · [[definitions/glossary|glossary]] · [[governance/keys|keys]]
**Kardeşler (data · 6/25):** [[knowledge/data/ado-net-command|ado-net-command]] · [[knowledge/data/command-timeout-policy|command-timeout-policy]] · [[knowledge/data/concurrency-optimistic|concurrency-optimistic]] · [[knowledge/data/connection-pooling-notes|connection-pooling-notes]] · [[knowledge/data/connection-string-secrets|connection-string-secrets]] · [[knowledge/data/dapper-async-patterns|dapper-async-patterns]] · [[knowledge/data/dual-tfm-provider-choice|dual-tfm-provider-choice]] · [[knowledge/data/migration-rollback|migration-rollback]] · [[knowledge/data/migration-script-writing|migration-script-writing]] · [[knowledge/data/migration-version-table|migration-version-table]] · [[knowledge/data/null-handling-db-mapping|null-handling-db-mapping]] · [[knowledge/data/pagination-offset-keyset|pagination-offset-keyset]] · [[knowledge/data/parameterized-queries|parameterized-queries]] · [[knowledge/data/repository-pattern-thin|repository-pattern-thin]] · [[knowledge/data/sql-injection-defense|sql-injection-defense]] · [[knowledge/data/sqlite-provider-setup|sqlite-provider-setup]] · [[knowledge/data/sqlserver-provider-setup|sqlserver-provider-setup]] · [[knowledge/data/transaction-scope-local|transaction-scope-local]] · [[knowledge/data/unit-of-work-pattern|unit-of-work-pattern]]

> **Tek cumlelik sorumluluk:** *Sorgu satiri hangi tipe, hangi kuralla eslenir; sessiz eslesme hatalari nasil onlenir* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

`QueryAsync<T>` cagrilir, kolon adi **buyuk / kucuk harf duyarsiz** olarak `T`'nin ozelligine baglanir. Eslesmeyen kolon sessizce **atlanir**, eslesmeyen ozellik **`default`** degerinde kalir - bu sessizlik, `default` degerinin anlami olan tiplerde (`int`, `DateTime`) gorunmez veri uretir ve onlem zorunludur (DA-08). Satir tipleri `dynamic` degil, tip satiri olarak yazilir (DA-05). Veritabani ile C# tipi arasindaki kopru (`DateTime` <-> ISO-8601 metin, `bool` <-> `int`) dialect / factory katmaninda ya da `TypeHandler` ile tek yerde cozulur.

```csharp
public sealed class SessionRow
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public bool IsArchived { get; set; }
}

const string sql =
    "SELECT Id, Title, CreatedAt, IsArchived FROM Sessions WHERE Id = @Id";

var row = await connection.QuerySingleOrDefaultAsync<SessionRow>(sql, new { Id = id },
    commandTimeout: DbTimeouts.Query, cancellationToken: cancellationToken);

if (row is null) return SessionNotFound.Instance;   // bos satir default ile gizlenmez
```

---

## §1 Esleme kurallari tablosu

| Durum | Dapper davranisi | Kural |
|---|---|---|
| `Id` kolonu -> `Id` ozelligi | buyuk / kucuk harf duyarsiz eslesir | kolon adini oldugu gibi yaz |
| Eslesmeyen kolon | sessince yok sayilir | SELECT listesi ile satir tipi hizali olmali |
| Eslesmeyen ozellik | `default` kalir | `default` anlami olan tipte dikkat |
| `NULL` kolon | `null` (nullable tipte) | `string?`, `DateTime?` kullan |
| Constructor'li tip | parametrelerle esler | parametre adlari kolonla ayni olmali |
| `enum` kolon | sayi okunur | `TypeHandler` ile donusum tek yerde |
| Coklu result-set | `QueryMultiple` | [[knowledge/data/dapper-multi-map|dapper-multi-map]] |

---

## §2 Kural tablosu (DA-05 - DA-08)

| # | Kural | Ihlal belirtisi |
|---|---|---|
| 1 | `Query<dynamic>` kod boyunca **0 adet** | her yerde `(string)row.Title` |
| 2 | Satir tipi Infrastructure katmaninda tanimli tip | anonim tip servise tasinmak |
| 3 | `SELECT` listesi = satir tipi ozellikleri | sessiz `default` veri |
| 4 | `NULL` gercek `?` tipi ile (`DateTime?`) | raporunda `0001-01-01` tarihleri |
| 5 | Esleme **testle** kanitlanir | gercek sema uzerinde entegrasyon testi |

---

## §3 Tip koprusu

| C# tipi | Saklama (SQL Server) | Saklama (SQLite test) | Not |
|---|---|---|---|
| `DateTime` | `datetime2` | `TEXT` (ISO-8601) | UTC, `Z` eki |
| `bool` | `bit` | `INTEGER` (0/1) | tek yerde donusum |
| `Guid` | `uniqueidentifier` | `TEXT` | ayni semantik |
| `int` | `int` | `INTEGER` | dogrudan |
| `string` | `nvarchar` | `TEXT` | `NULL` icin `string?` |

Kopru tek yerde tanimlanir; her repository'de ayri donusum yazilmaz.

---

## §4 net48 / net10.0-windows farki

| Konu | `net48` (C# 7.3) | `net10.0-windows` |
|---|---|---|
| `record` satir tipi | C# 9 - paylasilan kodda **`sealed class` + constructor** yazilir | `record` yazilabilir |
| `init` / `required` | yok | var |
| Nullable anotasyonu | proje kapali olabilir | `#nullable enable` |
| Dapper surumu | **2.1.x** | **2.1.x** - ayni esleme motoru |

> **KURAL:** Satir tipi sozdizimi iki bacakta da derlenir olmali; `record` kullanilip kullanilmayacagi ortak dil surumu baglidir ve mimari karardir: **DOĞRULAMA GEREKLI**.

---

## §5 Kardesler

[[knowledge/data/dapper-parameters-dynamic|dapper-parameters-dynamic]] · [[knowledge/data/dapper-multi-map|dapper-multi-map]] · [[knowledge/data/dapper-procedures-stored|dapper-procedures-stored]] · [[knowledge/data/audit-columns-convention|audit-columns-convention]] · [[knowledge/data/ado-net-connection|ado-net-connection]]

---

## §6 Kisa karar

`QueryAsync<T>` + tip satiri + SELECT listesi ile ozelliklerin hizasi + gercek `?` tipleri. `dynamic` ve anonim satir tipi kod boyunca sifirdir; esleme, gercek sema uzerinde bir entegrasyon testiyle kanitlanir.

---

## §7 Teshis

| Belirti | Kok neden | Duzeltme |
|---|---|---|
| Ozellik hep `default` | kolon adi farkli (tire / alt çizgi) | SELECT adini ozellik adina hizala |
| `0001-01-01` gorunuyor | `DateTime` null kabul ediyor | `DateTime?` kullan |
| Her yerde donusum var | `dynamic` kullanilmis | tip satiri gec |
| Enum yanlis okunuyor | sayi - isim farki | `TypeHandler` ekle |
| Test geciyor, uretim farkli | iki saglayicinin tip sozlugu farkli | dialect katmaninda tek kopru |

---

## §8 Doğrulama

| Kontrol | Kanit |
|---|---|
| `Query<dynamic>` aramasi = **0** | kod taramasi |
| SELECT listesi ile satir tipi hizali | sorgu incelemesi |
| `NULL` kolonlar nullable tipte | kod incelemesi |
| Gercek sema uzerinde esleme testi gecti | entegrasyon testi |
| `TypeHandler` kayitlari uygulama baslangicinda yapildi | calistirma testi |

---

## §9 Kanit

| Kaynak dosya | Ne alindi |
|---|---|
| `dapper-query-mapping.md` | esleme davranisi tablosu, DA-05 / DA-08 kurallari, `record` / `sealed class` iki bacak notu |
| `dapper-multi-map.md` | `splitOn` ve `QueryMultiple` ile sinir |
| `dapper-parameters-dynamic.md` | anonim nesne parametre yazimi |
| `audit-columns-convention.md` | UTC ISO-8601 tarih bicimi |
| `ado-net-connection.md` | `using` + `OpenAsync` catisi |

> Kanitsiz iddia icin `DOĞRULAMA GEREKLI` yazilir; bu dosyada kaynak disi bilgi yoktur.
