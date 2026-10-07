---
title: "task-vs-value-task - VersaCoder"
type: knowledge
category: csharp
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "Task ile ValueTask secimi, tekrar await kurali, net48 paket gereksinimi ve kopyalama/alloc maliyeti"
---

# task-vs-value-task — Task mı, ValueTask mı

**Zorunlu Baglantilar:** [index](../index.md) · [coding-standards](../../.rules/coding-standards.md) · [async-await-basics](async-await-basics.md) · [linq-performance-pitfalls](linq-performance-pitfalls.md)

> **Tek cumlelik sorumluluk:** *Bir asenkron metot `Task` mi `ValueTask` mi donecek, tekrar await ne zaman yasak* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

**Varsayilan `Task`/`Task<T>`**; `ValueTask` yalnizca **sicak yolda (hot path) ve tek bekleyici** varsa kullanilir — cunku `ValueTask`'i 2. kez `await` etmek davranis tanim disidir ve `Task`'ten farkli olarak uzerinde biriktirilen sonuc **cache'lenmez**. Kural: *await'i bir kez, sonucu Task gibi paylasacaksan Task*. `net48` bacakta `ValueTask` icin `System.Threading.Tasks.Extensions` paketi gerekir, `net10.0-windows` bacakta yerlesiktir — yani ortak imzayla yazilan bir API, iki bacakta da ayni davranisi vermek zorundadir.

---

## §1 Secim Tablosu

| Durum | Secim | Gerekce |
|---|---|---|
| Public/katman API | `Task` / `Task<T>` | Kararli, her yerde test edilebilir |
| Sicak yolda, tek await, sync-completion sik | `ValueTask<T>` | Allocation dusurur |
| Sonuc cache'lenecek / 2. kez beklenecek | `Task<T>` | `ValueTask` tekrar beklenebilir degil |
| Hicbir sey donecekse (boş) | `Task` (veya sicak yolda `ValueTask`) | `Task.CompletedTask` her iki bacakta var |
| `IAsyncEnumerable` akisi | Akisin kendisi | `Task` dondurmez |

> **KURAL:** `ValueTask` bir *optimizasyon*dur, bir *stildir*. Optimize edilmemis kodda `Task` kullanmak hata degildir; `ValueTask`'i her yerde kullanmak ise hata olabilir.

## §2 Iki Bacakta

| Konu | net48 · C# 7.3 | net10.0-windows · C# 14 |
|---|---|---|
| `Task` / `Task<T>` | Yerlesik | Yerlesik |
| `ValueTask` / `ValueTask<T>` | **Paket**: `System.Threading.Tasks.Extensions` | Yerlesik |
| `IsCompletedSuccessfully` | Paket uzerinden | Var |
| `AsTask()` / `ValueTask.FromResult` | Paket uzerinden | Var |
| Async method builder (`AsyncMethodBuilderAttribute`) | Paket + derleyici destegi | Yerlesik |
| `IValueTaskSource<T>` (ileri duzey, sifirlama) | Paket | Var (AOT dostu) |

`net48` bacakta paket eklenmeden `ValueTask` yazildiginda derleme hatasi alinir; paket eklenince **iki bacak testi** zorunlu hale gelir (imza, davranis, allocation davranisi).

## §3 Ornekler

Dogru — tek bekleyici, sicak yol:

```csharp
internal async ValueTask<string?> OkuAsync(int id, CancellationToken ct)
{
    var onbellek = _onbellek.Get(id);
    if (onbellek is not null) return onbellek;      // sync tamamlanma: alloc yok

    var satir = await _repo.SorgulaAsync(id, ct).ConfigureAwait(false);
    return satir?.Icerik;
}
```

Yanlis — ayni `ValueTask` iki kez await + paylasim:

```csharp
var vt = CevirAsync(kayit);           // tek instance
var a = await vt;                     // 1. await
var b = await vt;                     // 2. await -> hata / tanim disi davranis

await Task.WhenAll(CevirAsync(x), CevirAsync(y));  // paylasilan ValueTask yasak
```

Dogru alternatif — paylasim gerekiyorsa `Task`:

```csharp
Task<string> CevirTaskAsync(Kayit kayit) => CevirAsync(kayit).AsTask();
// ya da basit: metodu zaten Task<T> donecek sekilde yaz
```

## §4 Sik Hatalar

| # | Hata | Sonuc | Dogrusu |
|---|---|---|---|
| 1 | `ValueTask`'i `Task.WhenAll` icinde paylasmak | Tanim disi davranis | Her cagri icin ayri, ya da `Task` |
| 2 | `ValueTask` sonucunu saklayip sonra beklemek | Cache yok, kayip sonuc | `Task<T>` dondur |
| 3 | Olcum yapmadan "ValueTask hizlandirir" demek | Gereksiz karmasiklik | Once olc, sonra karar (NEEDS_VALIDATION) |
| 4 | net48'de paket unutulmasi | Derleme hatasi | Iki bacak build |
| 5 | Async metot icinde gereksiz `ValueTask.FromResult` sarmalayicilari | Okunurluk kaybi | Sync donus yolu dogrudan dondur |

## §5 Dogrulama

| Kontrol | Kanit |
|---|---|
| Tek bekleyici kurali | Review: `ValueTask` uzerinde 2. `await` = 0 |
| Iki bacak | `net48` (paketli) + `net10.0-windows` build 0 hata |
| Kural bagi | [coding-standards](../../.rules/coding-standards.md) |
| Kardesler | [async-await-basics](async-await-basics.md) · [linq-performance-pitfalls](linq-performance-pitfalls.md) |

`ValueTask`'in olcusuz kullanildigi bir imza, olcum eklenene dek `NEEDS_VALIDATION` olarak isaretlenir.
