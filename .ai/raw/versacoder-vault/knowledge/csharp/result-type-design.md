---
title: "result-type-design - VersaCoder"
type: knowledge
category: csharp
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "Result/Result<T>/Error/ErrorCode sozlesmesine onizleme (l0-abstractions N02 ile tutarli) ve istisna-Result siniri"
---

# result-type-design — Result / ErrorCode tasarımı

**Zorunlu Baglantilar:** [index](../../index.md) · [knowledge index](../index.md) · [exception-handling-patterns](exception-handling-patterns.md) · [coding-standards](../../.rules/coding-standards.md)

> **Tek cumlelik sorumluluk:** *Bir islem nasil basari/basarisizlik doner, hata nasil siniflanir ve istisna nerede biter* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Bu dosya, vault'un **l0 katmanindaki `Result`/`ErrorCode` sozlesmesine ONIZLEME**dir; kesin ve baglayici kayit **ADR-013 ile uyumlu l0 `result-error` dugumudur** (proje: `VersaCoder.Abstractions`) — buradaki amac dili ve siniri ogrenmek, tekrar tanim kurmak degildir (SSOT). Temel hukum: **beklenen hata degerdir, istisna degildir**; her katman `Result`/`Result<T>` ile doner, hata `ErrorCode` + kullaniciya uygun Turkce `Message` + yalniz loga `Detail` tasir. Istisna yalnizca **programlama hatasinda** (null arguman, bozuk sozlesme) atilir ve asla katman sinirini cikmaz. Bu projede **tek sonuc tipi** vardir; ikinci bir sonuc tipi iki ayri hata dili dogurur.

---

## §1 Sozlesme Ozeti (l0 ile tutarli)

| Tip | Alan / imza | Sorumluluk |
|---|---|---|
| `ErrorCode` (enum) | `None, Invalid, NotFound, Conflict, Unauthorized, Timeout, Cancelled, Database, Protocol, NotImplemented` | Hata siniflandirmasi — ekran davranisini belirler |
| `Error` | `Code`, `Message`, `Detail`, `Fields` | Tek hata nesnesi; `Fields` = alan -> mesaj dogrulama hatasi |
| `Result` | `IsSuccess`, `Error`, `Ok()`, `Fail(Error)` | Degeri olmayan islem sonucu |
| `Result<T>` | `IsSuccess`, `Value`, `Error`, `Ok(T)`, `Fail(Error)` | Degeri olan islem sonucu |

| Kural | Madde |
|---|---|
| S1 | Beklenen hata **degerdir**, istisna degildir |
| S2 | `Value` basarisizlikta `default(T)`; cagiran once `IsSuccess` kontrol eder |
| S3 | Programlama hatasi istisnadir, `Result` ile gizlenmez |
| S4 | `Message` Turkce ve kullaniciya uygun; `Detail` yalniz loga |
| S5 | Istisna asla katman sinirini cikmaz; yakalayan `Result.Fail` donecektir |

> **KURAL:** l0 tipi **tek tanimlidir**; uygulama katmanlari kendi `MyResult`/`OperationOutcome` varyantini uretmez. Varyant lazimsa mimariye gidilir (ADR-013 uyumu, `architect` X).

## §2 Iki Bacakta

| Konu | net48 · C# 7.3 | net10.0-windows · C# 14 |
|---|---|---|
| `struct` ile `Result<T>` (alloc yok) | Var | Var |
| Nullable annotation (`Result<T>?`, `string?`) | Dil surumu 7.3'te **annotation yok** (bkz. [nullable-reference-types](nullable-reference-types.md)) | Var |
| `init` / `record` ile immutable `Error` | Yok (C# 9) | Var |
| `required` ile zorunlu alan | Yok (C# 11) | Var |
| `readonly struct` + `readonly` member | Var (7.2) | Var |
| Generic matematik ile `ErrorCode` donusumu | Yok (C# 11 + runtime) | Var |
| Ekran/dil farki | Ayni enum, ayni davranis | Ayni enum, ayni davranis |

Ortak sozlesme **en kucuk ortak paydada** yazilir: `readonly struct` + klasik property getter (C# 7.3 uyumlu), `record`/`required` yalniz `net10` bacakta kalan dosyada ve `#if` **yalniz platform farkinda** kullanilir.

## §3 Ornek: akis

```csharp
// Beklenen hata -> deger
var sonuc = _repo.Sorgula(id);
if (!sonuc.IsSuccess)
    return Result<Kayit>.Fail(sonuc.Error);     // hata koduyla gecer

// Dogrulama hatasi -> alan bazli
var hata = new Error(ErrorCode.Invalid, "form eksik", detail: "dogrulama",
                     fields: new Dictionary<string, string> { ["Ad"] = "zorunlu" });
return Result<Kayit>.Fail(hata);

// Programlama hatasi -> istisna (Result ile gizlenmez)
if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id));
```

| Katman | Donus | Istisna durumu |
|---|---|---|
| l1 domain | `Result<T>` | invariant kirildi -> fail-fast |
| l2 application | `Result<T>` | dogrulama -> `Invalid` + `Fields` |
| l4 infra (ADO.NET + Dapper) | `Result<T>` | SQL hatasi -> `Database`, **istisna yakalanip cevrilir** |
| l7 UI | hata kodu -> ekran mesaji | kullaniciya `Message`, loga `Detail` |

## §4 Sik Hatalar

| # | Hata | Sonuc | Dogrusu |
|---|---|---|---|
| 1 | Iki sonuc tipi (`Result` + `Exception` ile ayni is) | Cift hata dili | Tek tip: `Result` |
| 2 | `Value`'yu `IsSuccess` sormadan okumak | Yanlis veri / `default` | Once `IsSuccess` |
| 3 | `Message` icine SQL/detay koymak | Sızıntı | `Message` guvenli, `Detail` log |
| 4 | Her yerde `try/catch` ile `Result` uretmek | Mantik bulanik | Sinirla: en dis katman + alt katman imzasi `Result` |
| 5 | l0 tipini kopyalayip uygulamada tanimlamak | SSOT ihlali | l0'daki tek tip kullanilir |

## §5 Dogrulama

| Kontrol | Kanit |
|---|---|
| Tek sonuc tipi | Kod taramasinda ikinci sonuc tipi = 0 |
| Hata yolu testi | `dotnet test`: her `ErrorCode` icin en az bir yol |
| Mimari uyum | NetArchTest katman kurali + ADR-013 (katalog kaydi) |
| Kayit nerede | Vault [index](../../index.md) -> architecture `l0-abstractions` (N02 result-error) |

Kesin imza ve hukum icin bu dosya degil, **l0 `result-error` dugumu** kaynaktir; onizleme ile dugum celisirse dugum kazanir ve bu dosya guncellenir.
