---
title: "exception-handling-patterns - VersaCoder"
type: knowledge
category: csharp
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "catch sirasi, throw vs throw ex, when filtresi, istisna siniflandirma ve iki bacakta arguman kontrol API'leri"
---

# exception-handling-patterns — İstisna kalıpları

**Zorunlu Baglantilar:** [index](../index.md) · [security-rules](../../.rules/security-rules.md) · [result-type-design](result-type-design.md) · [cancellation-token-policy](cancellation-token-policy.md)

> **Tek cumlelik sorumluluk:** *Istisna nerede yakalanir, nerede yeniden atilir ve hangi hata `Result`'a, hangisi istisnaya gider* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Istisna **programlama hatasi ve beklenmeyen durum** icindir; dogrulanmis is hatasi `Result`/`ErrorCode` ile doner (bkz. [result-type-design](result-type-design.md)). Yakalama kurali uctur: **en ozelden genise** siralanir, `catch` mumkun oldugunca dar tip + `when` filtresi ile yazilir, ve **`throw;`** ile yeniden atilir (`throw ex;` stack trace'i siler, yasak). Log **bir kez**, en dis katmanda yapilir; ara katmanda yakalayip sessizce gecmek kayip uretir. Mesajlar kullaniciya Turkce ve guvenli olur — baglanti dizesi, SQL, sifre gibi detaylar yalniz loga gider (`Detail`), ekrana asla cikmaz.

---

## §1 Catch Sirasi ve Filtr

| Kalip | Ne zaman | Sonuc |
|---|---|---|
| `catch (OperationCanceledException) when (ct.IsCancellationRequested)` | Iptal | Normal sonuc sayilir |
| `catch (TimeoutException)` | Zaman asimi | `ErrorCode.Timeout` |
| `catch (SqlException ex) when (ex.Number == 2627)` | Belirli DB hatasi | `ErrorCode.Conflict` |
| `catch (Exception ex) en dis katmanda` | Son savunma hatti | Log + `ErrorCode` / yeniden at |
| `catch` + yut (`{ }`) | **Hiçbir yerde** | sessiz basarisizlik |

Genelden ozel disi istisna: tip hiyerarsisi `catch`'te one arkaya gore degil, **oyle siralanir ki her istisna bir kez yakalanir**; yakalama yapilmiyorsa `catch` bloklanmaz.

## §2 Iki Bacakta

| API / ozellik | net48 · C# 7.3 | net10.0-windows · C# 14 |
|---|---|---|
| `throw;` (stack korur) vs `throw ex;` | Var | Var |
| `when` filtresi (C# 6) | Var | Var |
| `ExceptionDispatchInfo.Capture(...).Throw()` | Var (.NET 4.5+) | Var |
| `AggregateException.Flatten()` | Var | Var |
| `ArgumentNullException.ThrowIfNull` | **Yok** (.NET 7+) | Var |
| `ObjectDisposedException.ThrowIf` | **Yok** (.NET 8+) | Var |
| `UnreachableException` (ic tutarlılik) | **Yok** (.NET 7+) | Var |
| `AppDomain.CurrentDomain.UnhandledException` | Var | Var |
| `TaskScheduler.UnobservedTaskException` | Var | Var (.NET Core'da ayarlanabilir davranis) |
| AOT/trim: reflection ile tip uyan istisna | Mesajlar kayabilir | Trim uyarilari (IL2xxx) acik |

> **UYARI:** `ThrowIfNull` gibi yardimcilar ortak katmanda **yalnizca kendi sarmalayicimiz** ile kullanilir; cunku `net48`'de yoktur. `#if` degil, ortak bir `Guard` sinifi yazmak iki bacakli testi tek seferde saglar.

## §3 Ornek: dar yakalama + korumali yeniden atis

```csharp
internal Result<Kayit> Kaydet(Kayit kayit)
{
    if (kayit is null)
        throw new ArgumentNullException(nameof(kayit));   // programlama hatasi -> istisna

    try
    {
        _baglanti.Execute(sql, new { Id = kayit.Id, Ad = kayit.Ad },
                           commandTimeout: 15);           // parameterized, G2
        return Result<Kayit>.Ok(kayit);
    }
    catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601)
    {
        return Result<Kayit>.Fail(Error.Of(ErrorCode.Conflict, "kayit zaten var", ex.Message));
    }
    catch (SqlException ex)
    {
        return Result<Kayit>.Fail(Error.Of(ErrorCode.Database, "kayit kaydedilemedi", ex.Message));
    }
}
```

| Satir | Kural |
|---|---|
| `ArgumentNullException` | Programlama hatasi -> `Result` ile **gizlenmez** |
| `when (ex.Number == ...)` | Yalniz istenen DB hatasi yakalanir |
| `Message` (Turkce) vs `Detail` (log) | Sızıntı yok — [security-rules](../../.rules/security-rules.md) |
| `commandTimeout` | Zaman asimi somut, string-concat yok |

## §4 Sik Hatalar

| # | Hata | Sonuc | Dogrusu |
|---|---|---|---|
| 1 | `throw ex;` | Stack trace silinir, kok neden kayip | `throw;` |
| 2 | Ara katmanda yakalayip yutmak | Olay hic olmamis gibi gorunur | Dis katmanda log + dondur |
| 3 | `catch (Exception)` + devam | Bozuk veriyle is surer | Dar tip ya da yeniden at |
| 4 | Istisna mesajini UI'a oldugu gibi vermek | SQL/ connection sızıntısı | `Message` (guvenli) / `Detail` (log) |
| 5 | `finally` icinde `Dispose` yerine tek `using` | Yinelenen temizlik kodu | `using` / `await using` (bacağa gore) |

## §5 Dogrulama

| Kontrol | Kanit |
|---|---|
| `throw ex;` | Kod taramasinda 0 |
| Bos `catch` | 0 |
| Hata yolu testi | `dotnet test`: basarisiz yol -> dogru `ErrorCode` |
| Iki bacak | `net48` + `net10.0-windows` build 0 hata |

Ilgili: [result-type-design](result-type-design.md) · [cancellation-token-policy](cancellation-token-policy.md) · [testing-rules](../../.rules/testing-rules.md).
