---
title: "parameterized-queries — VersaCoder"
type: knowledge
category: data
status: active
version: 1.0.0
updated: 2026-09-29
authority: primary
scope: "Her sorguda @ad parametresi zorunlulugu, IN/LIKE/ORDER BY kaliplari, string-concat SQL yasagi (DA-03 / G2)"
tags: [knowledge]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# parameterized-queries — Parametreli Sorgu Kalıpları

**Zorunlu Baglantilar:** [[index|vault index]] · [[claude|CLAUDE.md (G2)]] · [[.rules/coding-standards|coding standards]] · [[knowledge/data/sql-injection-defense|sql-injection-defense]] · [[knowledge/data/ado-net-command|ado-net-command]] · [[knowledge/data/dapper-parameters-dynamic|dapper-parameters-dynamic]]
**Küme + Hub:** [[knowledge/index|knowledge index]] · [[knowledge/data/index|data index]] · [[definitions/agents|AGENTS]] · [[.decisions/brain|brain]] · [[definitions/glossary|glossary]] · [[governance/keys|keys]]
**Kardeşler (data · 6/25):** [[knowledge/data/ado-net-connection|ado-net-connection]] · [[knowledge/data/audit-columns-convention|audit-columns-convention]] · [[knowledge/data/command-timeout-policy|command-timeout-policy]] · [[knowledge/data/concurrency-optimistic|concurrency-optimistic]] · [[knowledge/data/connection-pooling-notes|connection-pooling-notes]] · [[knowledge/data/connection-string-secrets|connection-string-secrets]] · [[knowledge/data/dapper-async-patterns|dapper-async-patterns]] · [[knowledge/data/dapper-multi-map|dapper-multi-map]] · [[knowledge/data/dapper-procedures-stored|dapper-procedures-stored]] · [[knowledge/data/dual-tfm-provider-choice|dual-tfm-provider-choice]] · [[knowledge/data/migration-rollback|migration-rollback]] · [[knowledge/data/migration-script-writing|migration-script-writing]] · [[knowledge/data/migration-version-table|migration-version-table]] · [[knowledge/data/null-handling-db-mapping|null-handling-db-mapping]] · [[knowledge/data/repository-pattern-thin|repository-pattern-thin]] · [[knowledge/data/sqlite-provider-setup|sqlite-provider-setup]] · [[knowledge/data/sqlserver-provider-setup|sqlserver-provider-setup]] · [[knowledge/data/transaction-scope-local|transaction-scope-local]] · [[knowledge/data/unit-of-work-pattern|unit-of-work-pattern]]

> **Tek cumlelik sorumluluk:** *Değer sorgu metnine nasıl hiç girmeden yazılır; IN, LIKE, ORDER BY gibi dinamik parçalar parametrelenemezse ne yapılır* sorusunun tek cevabıdır.

---

## §0 Ozet (Answer First)

Kural tek satırda biter: **gövde sabit, değer `@ad`.** `+`, `$"..."` veya `string.Format` ile SQL kurmak G2 ihlalidir → build RED, satır ADO.NET + Dapper ile yeniden yazılır. Parametrelenemeyen tek şey **adres**dir (tablo/kolon adı, sıralama yönü); o da allowlist ile sabit dallara bölünür. Dapper `@Ids` listesini kendisi genişlettiği için `IN` için elle `@p1, @p2` üretmeye gerek yoktur.

## §1 Dogru kullanim

```csharp
// sabit govde + parametre: plan yeniden kullanilir, deger metne girmez
const string sql = @"
    SELECT Id, Title FROM Sessions
    WHERE Title LIKE @Pattern AND DeletedAt IS NULL
    ORDER BY CreatedAt DESC
    OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY";

var rows = await connection.QueryAsync<SessionRow>(sql,
    new { Pattern = "%" + term + "%", Skip = skip, Take = take },
    commandTimeout: DbTimeouts.Query, cancellationToken: cancellationToken);
```

`%` ve `_` işareti kullanıcının kendisinden geliyorsa `@Pattern` içine kaçış yazılır; gerekirse `ESCAPE '\'` eklenir.

## §2 Kalip tablosu

| Kalıp | Çözüm | Yasak olan |
|---|---|---|
| Değer | `@ad` + anonim nesne / `DynamicParameters` | metin birleştirme |
| `IN` listesi | `WHERE Id IN @Ids` (Dapper genişletir) | `IN (1,2,3)` elle yazmak |
| `LIKE` | `@Pattern` parametresi + gerekirse `ESCAPE '\'` | girdiyle kalıp birleştirmek |
| Sıralama yönü | sabit kolon, yön **iki sabit dal** | `ORDER BY {kolon}` |
| Kolon/tablo adı | allowlist (`HashSet.Contains`) → sabit dal | adı parametre yapmak |
| Sayısal limit | `@Take` parametresi | `LIMIT " + n` |

## §3 Kural tablosu (DA-03 · DA-06)

| # | Kural | Kanıt |
|---|---|---|
| 1 | SQL birleştiren satır = **0** | tarama: `$"SELECT` · `+ "SELECT` · `string.Format` |
| 2 | Gövde `const string` ya da sabit alan | kod incelemesi |
| 3 | Dinamik adres allowlist'ten geçer | allowlist dışı isim → exception |
| 4 | Her sorguda `commandTimeout: DbTimeouts.Query` | DA-09 |
| 5 | Sorgu yalnız repository'de (DA-01) | form/serviste SQL görünmez |

## §4 net48 / net10.0-windows farki

| Konu | `net48` | `net10.0-windows` |
|---|---|---|
| Parametre API | `@ad` ayraçlı, `SqlParameter` 5.2.x | aynı — fark yok |
| Metin literal | `@"..."` (C# 2) | `"""..."""` (C# 11) yalnız net10 |
| Dapper liste genişletme | 2.1.x | 2.1.x — aynı davranış |
| Sonuç | **aynı SQL, aynı plan** | aynı |

Fark yalnız sayfa parçasındadır (bkz. [[knowledge/data/pagination-offset-keyset|pagination-offset-keyset]]); gövde iki bacakta değişmez.

## §5 Parametrelenemez parca

| Parça | Yöntem | Neden parametre değil |
|---|---|---|
| Tablo/kolon adı | allowlist → sabit SQL dalı | SQL sözdizimi katmanı |
| `ORDER BY` yönü | `ASC`/`DESC` sabit dalları | yön değer parametresi kabul etmez |
| Prosedür adı | `const string` + `CommandType.StoredProcedure` | ad çalıştırma anında sabitlenir |
| `LIMIT`/`OFFSET` | sayısal `@Take`/`@Skip` | sayıysa parametrelenir |

## §6 Sik hatalar

- Kolon adını parametre yapıp `ORDER BY @Col` yazmak (çalışmaz)
- `IN` için `string.Join(",", ids)` kullanmak
- Girdiyi önce `Trim`/`Replace` edip "temizlendi" sanmak
- Log için SQL'i string concat ile kurup sonra parametreliye çevirmek
- Sorguyu forma gömmek (DA-01 ihlali)

## §7 Dogrulama notlari

Allowlist'in ortak bir `ISqlDialect` yardımıyla mı, yoksarepository içinde sabit `HashSet` ile mi kurulacağı vault'ta sabitlenmemiştir — **DOĞRULAMA GEREKLI**. Hangi yol seçilirse seçilsin, sonuç allowlist dışı isimde exception'dır.

## §8 Kardesler

[[knowledge/data/sql-injection-defense|sql-injection-defense]] · [[knowledge/data/dapper-parameters-dynamic|dapper-parameters-dynamic]] · [[knowledge/data/ado-net-command|ado-net-command]] · [[knowledge/data/dapper-query-mapping|dapper-query-mapping]] · [[knowledge/data/pagination-offset-keyset|pagination-offset-keyset]]

## §9 Kanit

| Kaynak dosya | Bu dosyaya giren kanıt |
|---|---|
| `parameterized-queries.md` (kaynak) | sabit gövde örneği, kalıp tablosu, DA-03/DA-06 kuralları, TFM farkları |
| `sql-injection-defense.md` | G2 ihlali ve tarama testi tanımı |
| `pagination-offset-keyset.md` | `@Skip`/`@Take` parametresi ve dialect `PagingClause` |
| `ado-net-command.md` | `DbTimeouts.Query` ve `CancellationToken` kullanımı |
| `dapper-parameters-dynamic.md` | `DynamicParameters` ve liste genişletme |
