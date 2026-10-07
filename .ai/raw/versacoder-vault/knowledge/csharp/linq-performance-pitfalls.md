---
title: "linq-performance-pitfalls.md - VersaCoder"
type: knowledge
category: csharp
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "LINQ tuzaklari: erisim erteleme, tekrar sayim, kapali ifadeler ve iki bacakta eksik operatorler"
---

# linq-performance-pitfalls — LINQ tuzakları

**Zorunlu Baglantilar:** [index](../index.md) · [coding-standards](../../.rules/coding-standards.md) · [collections-choice](collections-choice.md) · [data-access-rules](../../.rules/data-access-rules.md)

> **Tek cumlelik sorumluluk:** *LINQ nerede yavaslar, hangi operator hangi bacakta var ve sorgu neden veritabaniyla karisir* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

LINQ'nun uc tuzasi vardir: **ertelenmis sorgu birden fazla kez enumerate edilir**, **kapali ifade (closure) her elemanda alloc yapar**, **`Count()`/`ElementAt()` gibi indeksli erisim lineer kalir**. Bu vault'ta LINQ **hafizada** calisir: veri ADO.NET + Dapper ile **parameterized** olarak gelir, LINQ sorguya degil **sonuca** uygulanir; `IQueryable` provider (EF Core dahil) **kullanilmaz** (ADR-005). `net10` bacakta `Chunk`/`DistinctBy`/`MaxBy`/`Order` gibi operatorler varken `net48`'de yoktur — ortak yardimci kod ya **7.3'te yazilan kendi dongumuzdur** ya da dosya bacaga ayrilir.

---

## §1 Tuzak Tablosu

| Tuzak | Kotu | Iyi | Etki |
|---|---|---|---|
| Ertelenmis sorgu | `var q = list.Where(...);` 2. kez | `.ToList()` ile bir kez kilitle | Cift is + kaynak degisimi |
| `Count()` vs `Any()` | `list.Count() > 0` | `list.Any()` | Tum elemanlari gezer |
| Indeksli erim | `q.ElementAt(42)` | `ToList()[42]` (zaten liste ise) | O(n) her cagirida |
| Sarmalayici ifade | Dongude lambda + dis degisken | Sabit / `foreach` degiskeni | Alloc + GC baskisi |
| Kategori sonrasi sirlama | `GroupBy` + `OrderBy` tekrar | Tek `OrderBy` + `ThenBy` | Ek domin siralama |
| `Contains` buyuk koleksiyonda | `List.Contains` O(n) | `HashSet` icinde | O(n) -> O(1) |
| `string` uzerinde `Contains(char)` | Karsilastirma karma | net10 `SearchValues` | Allocation dusme |

> **KURAL:** Sorgu **bir kez** uretilir, **bir kez** enumerate edilir; `IQueryable` davranisi isteniyorsa bu bir **veritabani tasarimi** kararidir, LINQ yazisi degil.

## §2 Iki Bacakta Operatorler

| Operator | net48 · C# 7.3 (LINQ to Objects) | net10.0-windows · C# 14 |
|---|---|---|
| `Where` / `Select` / `OrderBy` / `GroupBy` | Var | Var |
| `Any` / `All` / `First(OrDefault)` / `Single(OrDefault)` | Var | Var |
| `Aggregate` / `Sum` / `Min` / `Max` | Var | Var |
| `Chunk` | **Yok** (.NET 6+) | Var |
| `DistinctBy` / `GroupBy` anahtar select | **Yok** (elle `GroupBy`) | Var |
| `MaxBy` / `MinBy` | **Yok** (.NET 6+) | Var |
| `Order` / `OrderDescending` | **Yok** (.NET 7+) | Var |
| `TryGetNonEnumeratedCount` | **Yok** (.NET 6+) | Var |
| `ElementAt` (index'li, indeksli kaynak icin hizli) | Kaynaga bagli | Kaynaga bagli |
| `SearchValues` ile `Contains(char)` | **Yok** (.NET 8+) | Var |
| `System.Linq.Async` (`ToListAsync`) | Yalniz paketle | Yerlesik `await foreach` |

`net48`'de gereken is icin **kendi kisa dongumuz** yazilir (YAGNI: ek LINQ paketi alinmaz); dongu test edilir, iki bacakta da ayni sonucu verir.

## §3 Ornek

Tuzak — iki kez enumerate + kapali ifade:

```csharp
var aktif = musteriler.Where(m => m.Durum == "Aktif");     // ertelenmis
Console.WriteLine(aktif.Count());                           // 1. gezm
Console.WriteLine(aktif.Count(m => m.Borc > 0));            // 2. gezm + alloc
```

Dogru — bir kez kilitle, sabit ifade, Dapper ile veritabani isini ayir:

```csharp
var aktif = musteriler.Where(m => m.Durum == "Aktif").ToList();   // tek enumerate
int borclu = aktif.Count(m => m.Borc > 0);                         // liste uzerinde

var satirlar = baglanti.Query<Kayit>(
    "SELECT Id, Ad FROM Kayit WHERE Durum = @p1 AND Borc > @p2",
    new { p1 = "Aktif", p2 = 0 }, commandTimeout: 15);             // parameterized (G2)
```

| Satir | Kural |
|---|---|
| `.ToList()` | Ertelenmis sorguyu bir kez kilitle |
| `@p1/@p2` | String-concat SQL **yasak** |
| `commandTimeout` | Zaman asimi somut |
| `musteriler` | Hafiza ici kaynak; SQL degil, sonuc |

## §4 Sik Hatalar

| # | Hata | Sonuc | Dogrusu |
|---|---|---|---|
| 1 | Dongude `QueryMultiple` + LINQ karisimi | Iki sorgu semasi | Ayri: SQL islevi, LINQ hafiza |
| 2 | `OrderBy` her adimda | Ek domin siralama | Tek seferde `ThenBy` |
| 3 | Buyuk toplulukta `List.Contains` | O(n^2) | `HashSet` |
| 4 | net10 operatorunu ortak dosyaya yazmak | net48 kirilir | §2 tablosu |
| 5 | Olcmeden "yavas" diye Linq'yi kaldirmak | Kaynak gorunmez | Olc (NEEDS_VALIDATION) |

## §5 Dogrulama

| Kontrol | Kanit |
|---|---|
| Cift enumerate | Review: sorgu degiskeni 2+ kez mi geziyor? |
| SQL | [data-access-rules](../../.rules/data-access-rules.md) — parameterized |
| Iki bacak | `net48` + `net10.0-windows` build 0 hata |
| Kardesler | [collections-choice](collections-choice.md) · [linq icin olcum](../../.rules/testing-rules.md) |

Buyuk veri uzerinde "hizli" iddiasi **olcusuz** yazilmaz: `NEEDS_VALIDATION` + kisa olcum notu.
