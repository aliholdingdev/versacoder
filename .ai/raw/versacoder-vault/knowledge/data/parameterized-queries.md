---
title: "parameterized-queries - VersaCoder"
type: knowledge
category: data
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "Her sorguda @ad parametresi zorunluluğu, IN/LIKE/ORDER BY kalıpları, string-concat SQL yasağı (DA-03 / CLAUDE.md G2)"
---

# parameterized-queries — Parametreli Sorgu Kalıpları

**Zorunlu Baglantilar:** [vault index](../../index.md) · [data-access-rules](../../.rules/data-access-rules.md) · [security-rules](../../.rules/security-rules.md) · [data index](../index.md) · [sql-injection-defense](sql-injection-defense.md)

> **Tek cumlelik sorumluluk:** *Değer sorgu metnine nasıl hiç girmeden yazılır; IN, LIKE, ORDER BY gibi dinamik parçalar parametrelenemezse ne yapılır* sorusunun tek cevabıdır.

---

## §0 Ozet (Answer First)

Kural tek: **gövde sabit, değer `@ad`.** `+`, `$"..."`, `string.Format` ile SQL kurmak `G2` ihlalidir → build RED, kod ADO.NET + Dapper ile yeniden yazılır. Parametrelenemeyen tek şey **adres**dir (tablo / kolon adı); o da **allowlist** ile sabit dallara bölünür. Dapper `@Ids` listesini kendisi genişletir, `IN` için elle ad üretmeye gerek yoktur.

---

## §1 Dogru kullanim (BAD -> GOOD)

```csharp
// GOOD — sabit govde + parametre (plan yeniden kullanilir, injection imkansiz)
const string sql = @"
    SELECT Id, Title FROM Sessions
    WHERE Title LIKE @Pattern AND DeletedAt IS NULL
    ORDER BY CreatedAt DESC
    OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY";

var rows = await connection.QueryAsync<SessionRow>(sql,
    new { Pattern = "%" + term + "%", Skip = skip, Take = take },
    commandTimeout: DbTimeouts.Query, cancellationToken: cancellationToken);
```

---

## §2 Kalip tablosu

| Kalıp | Çözüm | Yasak olan |
|---|---|---|
| Değer | `@ad` + anonim nesne / `DynamicParameters` | metin birleştirme |
| `IN` listesi | `WHERE Id IN @Ids` (Dapper genişletir) | `IN (1,2,3)` elle yazmak |
| `LIKE` | `@Pattern = $"%{term}%"` + gerekirse `ESCAPE '\'` | kullanıcı girdisiyle kalıp birleştirme |
| Sıralama yönü | `ORDER BY CreatedAt` sabit, yön `@Desc` ile **iki sabit dal** | `ORDER BY {kolon}` |
| Kolon/tablo adı | **allowlist** (`Array.IndexOf` / `HashSet.Contains`) → sabit SQL dalı | adı parametre yapmak (yapılamaz) |
| Sayısal LIMIT | `@Take` parametresi | `LIMIT " + n` |

---

## §3 Kural tablosu (DA-03 · DA-06)

| # | Kural | Kanıt |
|---|---|---|
| 1 | SQL birleştiren satır = **0** (tarama: `$"SELECT` · `+ "SELECT` · `string.Format`) | `reviewer` kod taraması |
| 2 | Gövde `const string` ya da sabit alan (DA-06) | kod incelemesi |
| 3 | Dinamik adres allowlist'ten geçer | test: allowlist dışı isim → exception |
| 4 | Her sorguda `commandTimeout: DbTimeouts.Query` | DA-09 |
| 5 | Sorgu **yalnız** repository'de (DA-01) | form/serviste SQL görünmez |

---

## §4 net48 / net10.0-windows farkı

| Konu | `net48` | `net10.0-windows` |
|---|---|---|
| Parametre API | `@ad` ayraçlı, `SqlParameter` 5.2.x | **aynı** — fark yok |
| Metin literal sözdizimi | `@"..."` (C# 2) güvenli | `"""..."""` (C# 11) yalnız net10'da |
| Dapper liste genişletme | `2.1.x` | `2.1.x` — aynı davranış |
| Kod sonucu | **aynı SQL aynı plan** | aynı |

> **NOT:** İki bacak arasında sorgu metni değişmemelidir; farklılık yalnız `ISqlDialect.PagingClause` içindedir (bkz. [pagination-offset-keyset](pagination-offset-keyset.md)).

---

## §5 Kardesler

[sql-injection-defense](sql-injection-defense.md) · [dapper-parameters-dynamic](dapper-parameters-dynamic.md) · [ado-net-command](ado-net-command.md) · [dapper-query-mapping](dapper-query-mapping.md) · [pagination-offset-keyset](pagination-offset-keyset.md)
