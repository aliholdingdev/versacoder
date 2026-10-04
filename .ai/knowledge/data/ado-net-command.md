---
title: "ado-net-command - VersaCoder"
type: knowledge
category: data
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "DbCommand olusturma, Execute* yontemi secimi, parametre ekleme ve CommandTimeout - ham ADO.NET yolu"
---

# ado-net-command - ADO.NET Komut Calistirma

**Zorunlu Baglantilar:** [vault indeks](../index.md) · [ADR-005](../../.decisions/accepted/ADR-005-ado-net-dapper-no-ef-core.md) · [ADR-001](../../.decisions/accepted/ADR-001-dual-target-net48-net10.md) · [coding-standards](../../.rules/coding-standards.md) · [error-handling-rules](../../.rules/error-handling-rules.md) · [command-timeout-policy](command-timeout-policy.md)

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

[ado-net-connection](ado-net-connection.md) · [dapper-parameters-dynamic](dapper-parameters-dynamic.md) · [command-timeout-policy](command-timeout-policy.md) · [dapper-async-patterns](dapper-async-patterns.md) · [dapper-procedures-stored](dapper-procedures-stored.md)

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
