---
title: "exception-handling-patterns - VersaCoder knowledge/csharp"
type: knowledge
category: knowledge
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "catch sirasi ve when filtresi, throw vs throw ex, istisna-Result siniri ve iki bacakta arguman kontrol API'leri"
---

# exception-handling-patterns — İstisna kalıpları

**Zorunlu Baglantilar:** [knowledge-index](../index.md) · [csharp-index](index.md) · [error-handling-rules](../../.rules/error-handling-rules.md) · [coding-standards](../../.rules/coding-standards.md) · [result-type-design](result-type-design.md) · [retry-and-timeout-policy](retry-and-timeout-policy.md)

> **Tek cumlelik sorumluluk:** *Istisna nerede yakalanir, nerede yeniden atilir ve hangi hata `Result`'a, hangisi istisnaya gider* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Istisna **programlama hatasi ve beklenmeyen durum** icindir; dogrulanmis is hatasi `Result`/`ErrorCode` ile doner ([result-type-design](result-type-design.md), `SOL-07`). Yakalama uctur: **en ozelden genise** siralanir, `catch` dar tip + `when` filtresi ile yazilir, yeniden atis **`throw;`** ile olur — `throw ex;` stack trace'i siler ve **yasaktir**. Yutma yasaktir: bos `catch` bu vault'ta `reviewer` tarafindan HIGH olarak RED edilir (`error-handling-rules` E4, `CS-05`). Log **bir kez**, en dis katmanda alinir; mesajlar kullaniciya Turkce ve guvenli olur — baglanti dizesi, SQL, sifre gibi detay yalniz loga gider.

## §1 Catch sirasi ve filtre

| Kalip | Ne zaman | Sonuc |
|---|---|---|
| `catch (OperationCanceledException) when (ct.IsCancellationRequested)` | iptal | normal sonuc sayilir (E12) |
| `catch (TimeoutException)` | zaman asimi | `ErrorCode.Timeout` |
| `catch (SqlException ex) when (ex.Number == 2627)` | belirli DB hatasi | `ErrorCode.Conflict` |
| `catch (Exception ex)` en dis katmanda | son savunma hatti | log + `ErrorCode` / yeniden at |
| `catch` + bos blok | **hicbir yerde** | sessiz basarisizlik (E4) |

Tip hiyerarsisi `catch`'te one arkaya gore degil, **her istisnanin bir kez yakalanacagi** sekilde siralanir; yakalama yapilmiyorsa `catch` bloklanmaz.

## §2 Iki bacakta

| API / ozellik | net48 · C# 7.3 | net10.0-windows · C# 14 |
|---|---|---|
| `throw;` (stack korur) vs `throw ex;` | Var | Var |
| `when` filtresi (C# 6) | Var | Var |
| `ExceptionDispatchInfo.Capture(...).Throw()` | Var | Var |
| `AggregateException.Flatten()` | Var | Var |
| `ArgumentNullException.ThrowIfNull` | **Yok** (.NET 7+) | Var |
| `ObjectDisposedException.ThrowIf` | **Yok** (.NET 8+) | Var |
| `UnreachableException` | **Yok** (.NET 7+) | Var |
| `TaskScheduler.UnobservedTaskException` | Var | Var |

> `ThrowIfNull` gibi yardimcilar ortak katmanda **kendi `Guard` sarmalayicimiz** ile kullanilir; `#if` degil ortak sinif yazmak iki bacakli testi tek seferde saglar (G6).

## §3 Ornek: dar yakalama + korumali donus

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
| `ArgumentNullException` | programlama hatasi, `Result` ile **gizlenmez** |
| `when (ex.Number == ...)` | yalniz istenen DB hatasi yakalanir |
| `Message` (Turkce) vs `Detail` (log) | sızıntı yok (E3) |
| `commandTimeout` | zaman asimi somut (E7) |

## §4 Katman sorumlulugu

| Katman | Yakalar | Cikti | Yapmaz |
|---|---|---|---|
| Domain (l1) | is kurali ihlali | tip'li domain hata kodu | gerekcesiz exception |
| Application (l2) | eksik girdi, on kosul | `Result<T>` | log (Infrastructure yapar) |
| Infrastructure (l4) | `SqlException` · timeout · deadlock | cevrilmis `ErrorCode` | kullanici metni uretme |
| UI (l7) | beklenmeyen tek nokta | dialog + log | yutmak |

## §5 Kurallar

1. `catch` yalniz kendi sorumlulugunu sarar; gerisi **yukari tasınir** (E1).
2. Yeniden atis `throw;` ile olur; `throw ex;` kod taramasinda **0** satir.
3. Yeniden deneme gecici + idempotent hata icindir, ustel geri cekilme ile ([retry-and-timeout-policy](retry-and-timeout-policy.md)).
4. Kullaniciya `ex.Message`/stack **asla** gitmez; `Message` guvenli, `Detail` log.
5. 3 basarisiz duzeltmeden sonra kod yazmak yasaktir — bloke raporu yazilir.

## §6 Dogrulama

| Kontrol | Beklenen kanit |
|---|---|
| `throw ex;` | kod taramasinda 0 |
| Bos `catch` | 0 |
| Hata yolu testi | `dotnet test`: basarisiz yol -> dogru `ErrorCode` |
| UI'da stack gosteren pencere | 0 |
| Iki bacak | `net48` + `net10.0-windows` build 0 hata |

## §7 Ilgili dosyalar

| Konu | Dosya |
|---|---|
| hata sozlesmesi | [result-type-design](result-type-design.md) |
| yeniden deneme + timeout | [retry-and-timeout-policy](retry-and-timeout-policy.md) |
| iptal hukmu | [cancellation-token-policy](cancellation-token-policy.md) |
| guard clause yazimi | [guard-clauses-and-method-shape](guard-clauses-and-method-shape.md) |

## §9 Kanit

| # | Iddia | Kanit kaynagi |
|---|---|---|
| 1 | bos catch yasagi (HIGH) | `.rules/error-handling-rules.md` E4 + `.rules/coding-standards.md` CS-05 |
| 2 | kullaniciya stack gitmez | `.rules/error-handling-rules.md` E3 |
| 3 | katman bazli yakalama | `.rules/error-handling-rules.md` §2 |
| 4 | ustel geri cekilme 250-500-1000 ms | `.rules/error-handling-rules.md` §3 E6 |
| 5 | `ThrowIfNull` net48'de yok | ADR-001 ikili bacak + CS-12 |
