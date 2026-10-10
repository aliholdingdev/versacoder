---
title: "ado-net-command - VersaCoder"
type: knowledge
category: data
status: active
version: 1.0.0
updated: 2026-09-29
authority: primary
scope: "DbCommand olusturma, Execute* yontemi secimi, parametre ekleme ve CommandTimeout - ham ADO.NET yolu"
tags: [knowledge]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# ado-net-command - ADO.NET Komut Calistirma

**Zorunlu Baglantilar:** [[knowledge/index|vault indeks]] · [[.decisions/accepted/adr-005-ado-net-dapper-no-ef-core|ADR-005]] · [[.decisions/accepted/adr-001-dual-target-net48-net10|ADR-001]] · [[.rules/coding-standards|coding-standards]] · [[.rules/error-handling-rules|error-handling-rules]] · [[knowledge/data/command-timeout-policy|command-timeout-policy]]
**Küme + Hub:** [[knowledge/data/index|data index]] · [[index|index]] · [[claude|CLAUDE]] · [[definitions/agents|AGENTS]] · [[.decisions/brain|brain]] · [[definitions/glossary|glossary]] · [[governance/keys|keys]]
**Kardeşler (data · 6/25):** [[knowledge/data/audit-columns-convention|audit-columns-convention]] · [[knowledge/data/concurrency-optimistic|concurrency-optimistic]] · [[knowledge/data/connection-pooling-notes|connection-pooling-notes]] · [[knowledge/data/connection-string-secrets|connection-string-secrets]] · [[knowledge/data/dapper-multi-map|dapper-multi-map]] · [[knowledge/data/dapper-query-mapping|dapper-query-mapping]] · [[knowledge/data/dual-tfm-provider-choice|dual-tfm-provider-choice]] · [[knowledge/data/migration-rollback|migration-rollback]] · [[knowledge/data/migration-script-writing|migration-script-writing]] · [[knowledge/data/migration-version-table|migration-version-table]] · [[knowledge/data/null-handling-db-mapping|null-handling-db-mapping]] · [[knowledge/data/pagination-offset-keyset|pagination-offset-keyset]] · [[knowledge/data/parameterized-queries|parameterized-queries]] · [[knowledge/data/repository-pattern-thin|repository-pattern-thin]] · [[knowledge/data/sql-injection-defense|sql-injection-defense]] · [[knowledge/data/sqlite-provider-setup|sqlite-provider-setup]] · [[knowledge/data/sqlserver-provider-setup|sqlserver-provider-setup]] · [[knowledge/data/transaction-scope-local|transaction-scope-local]] · [[knowledge/data/unit-of-work-pattern|unit-of-work-pattern]]

> **Tek cumlelik sorumluluk:** *Bir `DbCommand` nereden olusur, hangi `Execute*` hangi ise yarar, degerler metne mi parametreye mi girer* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Komut ya Dapper (`QueryAsync` / `ExecuteAsync`) ya da ham `DbCommand` ile calistirilir; **ikisinde de** govde sabit metin, degerler parametre, `CommandTimeout` acik. `DbCommand` sahibi olan baglantinin `CreateCommand()` cagrisindan gelir; repository icinde `new SqlCommand(...)` yazmak yasaktir. Iade degeri gerekmeyen yazmada `ExecuteNonQueryAsync`, tek skaler degerde `ExecuteScalarAsync`, satir akisinda `ExecuteReaderAsync` kullanilir. Dapper bu uc yontemi sarar, elle yazmak gereksizdir; ham yol yalniz Dapper'in kapsadigi ozel durumlar icin kalir.

| Ihtiyac | Ham ADO.NET | Dapper karsiligi |
|---|---|---|
| Satir donmeyen yazma | `ExecuteNonQueryAsync` | `ExecuteAsync` |
| Tek skaler deger (COUNT, MAX) | `ExecuteScalarAsync` | `ExecuteScalarAsync<T>` |
| Birden cok satir | `ExecuteReaderAsync` | `QueryAsync<T>` |
| Birden cok result-set | `NextResult` | `QueryMultiple` |
| Depolama proseduru | `CommandType.StoredProcedure` | `CommandDefinition` + `CommandType` |

---

## §1 Dogru kullanim

```csharp
using (var connection = _connectionFactory.Create())
{
    await connection.OpenAsync(cancellationToken);
    using (var cmd = connection.CreateCommand())
    {
        cmd.CommandText = "UPDATE Sessions SET Title = @Title WHERE Id = @Id";
        cmd.CommandTimeout = DbTimeouts.Query;              // 0 yasak (DA-09)

        var pTitle = cmd.CreateParameter();
        pTitle.ParameterName = "@Title";
        pTitle.Value = title;
        cmd.Parameters.Add(pTitle);

        var pId = cmd.CreateParameter();
        pId.ParameterName = "@Id";
        pId.Value = id;
        cmd.Parameters.Add(pId);

        var affected = await cmd.ExecuteNonQueryAsync(cancellationToken);
    }
}
```

---

## §2 Parametre ekleme kurallari

| # | Konu | Dogru | Yanlis |
|---|---|---|---|
| 1 | Parametre olusturma | `cmd.CreateParameter()` | saglayiciya bagli tip elle `new SqlParameter()` |
| 2 | Ad | `"@Title"` - SQL govdesindeki `@Title` ile birebir ayni | `"@p1"` / `"@Id0"` gibi farkli ad |
| 3 | Deger | `pValue.Value = deger` | degeri metne `+` ile birlestirmek |
| 4 | Ekleme | `cmd.Parameters.Add(p)` | parametreyi kurup eklemeyi unutmak |
| 5 | `DBNull` | `null` icin `DBNull.Value` (Dapper yapar, ham yolda elle) | `Value = null` ile calistirmak |

Govdedeki tum degerler `@ad` ile temsil edilir; SQL govdesi hicbir yerde string concat ile kurulmaz. Bu, plan yeniden kullanimi ve denetim acisindan zorunludur (DA-03).

---

## §3 Kural tablosu (DA-03 - DA-06 - DA-09)

| # | Kural | Deger | Ihlal sonucu |
|---|---|---|---|
| 1 | `CommandText` | `const string` ya da sabit alan (DA-06) | plan yeniden kullanilmaz, denetim zorlasir |
| 2 | Deger | hic metne girmez, `@ad` parametresi (DA-03) | G2 ihlali -> `reviewer` RED |
| 3 | `CommandTimeout` | `DbTimeouts.Query` 15 - `Report` 60 - `Migration` 300 | `0` sinirsiz demektir, yasak |
| 4 | `CommandType` | `Text` varsayilan; `StoredProcedure` yalniz kasitli | karisik kullanim |
| 5 | Omur | `using` icinde; baglanti kapaninca yok olur | siza kalan `DbCommand` |

---

## §4 net48 / net10.0-windows farki

| Konu | `net48` (C# 7.3) | `net10.0-windows` |
|---|---|---|
| Async API | `ExecuteReaderAsync` vb. 4.5+ beraberinde geldi | ayni imza |
| `await using` | C# 8 sozdizimi - paylasilan koda yazilmaz, klasik `using ( ...) { }` | yazilabilir |
| Parametre tipi | `Microsoft.Data.SqlClient` 5.2.x | ayni paket, ayni surum |
| `CreateParameter()` | saglayici tipine baglilmadan guvenli yol | ayni |

> **NOT:** `SqlParameter.Size` ve `SqlDbType` gibi saglayiciya ozel ayarlar yalniz SQL Server dalinda calisir; SQLite test dalinin da ayni repository kodunu calistirmasi gerekir (DA-04). Iki bacak farki `#if` ile degil, factory ve dialect katmaniyla cozulur (ADR-001).

---

## §5 Ihlal belirtileri

| Belirti | Kok neden | Duzeltme |
|---|---|---|
| Her sorgu farkli plan uretiyor | `CommandText` interpolasyonla kuruluyor | sabit govde + parametre |
| Rapor ekrani donuyor | `CommandTimeout = 0` | `DbTimeouts.Report` (60) |
| `InvalidCastException` | deger metne karistirilmis | `CreateParameter` + `Value` |
| Tasan `DbCommand` | `using` yok | `using (var cmd = ...)` |
| Saglayici tipi repository'e sizi | `new SqlCommand(...)` | `connection.CreateCommand()` |

---

## §6 Dapper ile fark

| Davranis | Ham yol | Dapper |
|---|---|---|
| Parametre yazimi | elle `CreateParameter` | anonim nesne / `DynamicParameters` |
| Iptal | `Execute*Async(ct)` | `CommandDefinition.CancellationToken` |
| Timeout | `cmd.CommandTimeout` | `commandTimeout:` argumani |
| Satir tipi | `reader` + elle okuma | `QueryAsync<T>` |
| Coklu esleme | `NextResult` | `splitOn` / `QueryMultiple` |

---

## §7 Kardesler

[[knowledge/data/ado-net-connection|ado-net-connection]] · [[knowledge/data/dapper-parameters-dynamic|dapper-parameters-dynamic]] · [[knowledge/data/command-timeout-policy|command-timeout-policy]] · [[knowledge/data/dapper-async-patterns|dapper-async-patterns]] · [[knowledge/data/dapper-procedures-stored|dapper-procedures-stored]]

---

## §8 Kisa karar

`CreateCommand()` + sabit govde + `@ad` parametresi + acik `CommandTimeout` + `using`. Besi bir arada saglanmadan kod incelemesinden gecmez; detay kardes dosyalarda, kural kaynagi `data-access-rules` vault dosyasidir.

---

## §9 Kanit

| Kaynak dosya | Ne alindi |
|---|---|
| `ado-net-command.md` | Execute* yontem tablosu, `CreateCommand` kurali, DA-03/DA-06/DA-09 degerleri, iki bacak farklari |
| `dapper-parameters-dynamic.md` | `@` adlandirma ve `@p1` uyusmazligi notu |
| `command-timeout-policy.md` | `DbTimeouts` 15/60/300 ve `0` yasagi |
| `dapper-procedures-stored.md` | `CommandType.StoredProcedure` cagrim deseni |
| `dual-tfm-provider-choice.md` | SqlClient 5.2.x ortak surum, `#if` disiplini |

> Kanitsiz iddia icin `DOĞRULAMA GEREKLI` yazilir; bu dosyada kaynak disi bilgi yoktur.
