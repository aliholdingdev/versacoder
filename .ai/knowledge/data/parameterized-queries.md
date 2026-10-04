---
title: "parameterized-queries — VersaCoder"
type: knowledge
category: data
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "Her sorguda @ad parametresi zorunlulugu, IN/LIKE/ORDER BY kaliplari, string-concat SQL yasagi (DA-03 / G2)"
---

# parameterized-queries — Parametreli Sorgu Kalıpları

**Zorunlu Baglantilar:** [vault index](../../index.md) · [CLAUDE.md (G2)](../../CLAUDE.md) · [coding standards](../../.rules/coding-standards.md) · [sql-injection-defense](sql-injection-defense.md) · [ado-net-command](ado-net-command.md) · [dapper-parameters-dynamic](dapper-parameters-dynamic.md)

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

Fark yalnız sayfa parçasındadır (bkz. [pagination-offset-keyset](pagination-offset-keyset.md)); gövde iki bacakta değişmez.

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

[sql-injection-defense](sql-injection-defense.md) · [dapper-parameters-dynamic](dapper-parameters-dynamic.md) · [ado-net-command](ado-net-command.md) · [dapper-query-mapping](dapper-query-mapping.md) · [pagination-offset-keyset](pagination-offset-keyset.md)

## §9 Kanit

| Kaynak dosya | Bu dosyaya giren kanıt |
|---|---|
| `parameterized-queries.md` (kaynak) | sabit gövde örneği, kalıp tablosu, DA-03/DA-06 kuralları, TFM farkları |
| `sql-injection-defense.md` | G2 ihlali ve tarama testi tanımı |
| `pagination-offset-keyset.md` | `@Skip`/`@Take` parametresi ve dialect `PagingClause` |
| `ado-net-command.md` | `DbTimeouts.Query` ve `CancellationToken` kullanımı |
| `dapper-parameters-dynamic.md` | `DynamicParameters` ve liste genişletme |
