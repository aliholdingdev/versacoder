---
title: "dapper-parameters-dynamic - VersaCoder"
type: knowledge
category: data
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "Dapper parametre yazimi: anonim nesne vs DynamicParameters, cikti parametresi, null / DBNull, IN listesi genisletme (DA-03)"
---

# dapper-parameters-dynamic - Dapper Parametre Yazimi

**Zorunlu Baglantilar:** [vault indeks](../index.md) · [ADR-005](../../.decisions/accepted/ADR-005-ado-net-dapper-no-ef-core.md) · [coding-standards](../../.rules/coding-standards.md) · [error-handling-rules](../../.rules/error-handling-rules.md) · [ado-net-command](ado-net-command.md) · [dapper-query-mapping](dapper-query-mapping.md)

> **Tek cumlelik sorumluluk:** *Deger hangi nesneyle, hangi kuralla parametrelenir; cikti parametresi ve `null` nasil yazilir* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Sabit parametre seti icin **anonim nesne** (`new { Ad = x }`), kosullu parametre ya da cikti parametresi icin **`DynamicParameters`** kullanilir. Parametre adinda `@` **yazilmaz** (Dapper onu ekler); ad ile SQL'deki `@Ad` birebir ayni olmalidir. `null` -> `DBNull.Value` donusumunu Dapper yapar. `IN` listesi tek parametre adiyla yazilir, Dapper `@Ids`'i satir satir acar. Deger hicbir zaman metne girmez (DA-03).

```csharp
var p = new DynamicParameters();
p.Add("Title", title);                            // @ eklenmez
p.Add("Id", id);
p.Add("Limit", 50, DbType.Int32);                 // acik tip
if (!string.IsNullOrWhiteSpace(tag))
    p.Add("Tag", tag);                            // kosullu parametre

const string sql = @"
    UPDATE Sessions SET Title = @Title, UpdatedAt = @UpdatedAt
    WHERE Id = @Id AND DeletedAt IS NULL";

var affected = await connection.ExecuteAsync(sql, p,
    commandTimeout: DbTimeouts.Query, cancellationToken: cancellationToken);
```

---

## §1 Parametre secim tablosu

| Durum | Arac | Not |
|---|---|---|
| Hepsi sabit (3-5 adet) | anonim nesne | en kisa, en okunur |
| Kosullu parametre | `DynamicParameters` | yalniz cagrida eklenir |
| Cikti / donus degeri | `Add(..., ParameterDirection.Output)` | `Get<T>("Ad")` |
| `IN` listesi | `p.Add("Ids", idList)` | Dapper `@Ids`'i genisletir |
| `NULL` olabilir deger | `p.Add("X", maybeNull)` | Dapper `DBNull` yazar |
| Tip / buyukluk zorunlu | `DbType` + `Size` (yalniz SQL Server) | SQLite dali icin gereksiz |

---

## §2 Kural tablosu (DA-03)

| # | Kural | Yasak |
|---|---|---|
| 1 | Ad = SQL'deki `@Ad` (noktalanmis `p.Ad` degil) | `@p1` ile `"Id"` arasindaki uyusmazlik |
| 2 | Deger **asla** metne girmez | parametreli degeri `"` + ile birlestirmek |
| 3 | Liste parametresi tek adla | dongude `"@Id0,@Id1"` yazmak |
| 4 | `dynamic` yerine tip satir (DA-05) | `row.Id` uzerinde calismak |
| 5 | `commandTimeout` her cagrida | timeoutsuz cagri |

SQL govdesi sabit metin olur; degerler yalnizca `@ad` uzerinden girer. Boylece plan yeniden kullanilir ve enjeksiyon yuzeyi kapanir.

---

## §3 net48 / net10.0-windows farki

| Konu | `net48` | `net10.0-windows` |
|---|---|---|
| `DynamicParameters` | Dapper **2.1.x** - ayni | ayni |
| `DbType` tanimi | `System.Data.DbType` | ayni |
| `record` parametre | `record` C# 9 - paylasilan kodda yok | yazilabilir |
| Parametre davranisi | **fark yok** - tek kod yolu | ayni |

> **NOT:** Parametre katmani saglayicidan bagimsizdir; `@` ayracli soz dizimi iki saglayicida da gecerlidir - bazi eski saglayicilarda oldugu gibi `?` kullanilmaz.

---

## §4 Cikti parametresi ornegi

| Adim | Kod | Not |
|---|---|---|
| 1 | `p.Add("YeniId", dbType: DbType.Guid, direction: ParameterDirection.Output)` | yon acikca yazilir |
| 2 | `CommandDefinition` ile prosedur cagrisi | `commandType: StoredProcedure` |
| 3 | Cagri sonrasi `p.Get<Guid>("YeniId")` | donus degeri tek noktadan |
| 4 | `null` donus | `Get<T?>` ile ele alinir |
| 5 | Hata | ham saglayici exception'i yaysilmaz (DA-04) |

---

## §5 Kardesler

[ado-net-command](ado-net-command.md) · [dapper-query-mapping](dapper-query-mapping.md) · [dapper-multi-map](dapper-multi-map.md) · [dapper-async-patterns](dapper-async-patterns.md) · [dapper-procedures-stored](dapper-procedures-stored.md)

---

## §6 Kisa karar

Kucuk ve sabit set -> anonim nesne; kosullu ya da cikti icin -> `DynamicParameters`. `@` adlarda, govde sabit, deger parametre. `dynamic` satir tipi bu dosyanin disinda, [dapper-query-mapping](dapper-query-mapping.md) konusudur.

---

## §7 Teshis

| Belirti | Kok neden | Duzeltme |
|---|---|---|
| "Must declare the scalar variable" | ad uyusmazligi (`@p1` vs `Id`) | SQL'deki ad birebir kullan |
| Deger gorunmuyor | `p.Add` cagrisi unutulmus | her parametre eklenmeli |
| `IN` listesi calismiyor | elle ad uretimi | tek ad ile ver, Dapper acsin |
| Tip hatasi | `DbType` yazilmamis | acik `DbType` ekle |
| Timeout patlamasi | `commandTimeout` yok | `DbTimeouts.Query` |

---

## §8 Doğrulama

| Kontrol | Kanit |
|---|---|
| Metin birlestirme (`+`) ile SQL kurma = **0** | kod taramasi |
| Dongude parametre adi uretimi = **0** | kod taramasi |
| Her cagrida `commandTimeout` | kod incelemesi |
| `null` deger senaryosu testte geciyor | entegrasyon testi |

> `IN` listesi genisletmesinin buyuk listelerde siniri proje olcumune baglidir: **DOĞRULAMA GEREKLI**.

---

## §9 Kanit

| Kaynak dosya | Ne alindi |
|---|---|
| `dapper-parameters-dynamic.md` | anonim nesne / `DynamicParameters` secimi, DA-03 kurallari, `IN` ve `NULL` davranisi, iki bacak farki |
| `ado-net-command.md` | ham `CreateParameter` karsilastirmasi, `@p1` uyusmazligi |
| `dapper-query-mapping.md` | `dynamic` yerine tip satir kurali (DA-05) |
| `dapper-procedures-stored.md` | `CommandType.StoredProcedure` ile cikti parametresi |
| `command-timeout-policy.md` | her cagirda `DbTimeouts` zorunlulugu |

> Kanitsiz iddia icin `DOĞRULAMA GEREKLI` yazilir; bu dosyada kaynak disi bilgi yoktur.
