---
title: "span-interop-net10 - VersaCoder"
type: knowledge
category: csharp
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "Span/Memory ile kopyasiz byte arasi gecis, P/Invoke sinirinda marshalling ve System.Memory (net48) ile runtime (net10.0-windows) farki"
---

# span-interop-net10 — `Span<T>` ile Kopyasız Veri Geçişi

**Zorunlu Baglantilar:** [vault index](../../index.md) · [knowledge index](../index.md) · [CLAUDE.md](../../CLAUDE.md) · [coding-standards](../../.rules/coding-standards.md) · [security-rules](../../.rules/security-rules.md) · [collections-choice](collections-choice.md) · [language-versions-net48-net10](language-versions-net48-net10.md)

> **Tek cümlelik sorumluluk:** *Bellekten belleğe, diziden native sınır'a veri kopyalamadan nasıl geçilir, `Span<T>` hangi yerde kullanılamaz* sorusunun tek cevabıdır.

---

## §0 Ozet (Answer First)

`Span<T>` / `ReadOnlySpan<T>` **yığında (stack) ya da tek bir dizi üzerinde görünüm**dür: kopya üretmez, sınırda (interop) tamponu olduğu gibi geçirir. En sık kullanım: `byte[]` → `ReadOnlySpan<byte>` → `Encoding.GetString` / `Utf8` çözümü, ve P/Invoke çağrısında `fixed`/`MemoryMarshal` ile imleç (pointer) elde etme. `Span<T>` **`async` metotta, `yield` içinde ve referans alan olarak saklanamaz** — orada `Memory<T>` kullanılır. Kopya kaçınılmazsa (uzun ömürlü saklama, kuyruk) `Span` değil **dizi** tutulur.

| # | Araç | Ne yapar | Kopya |
|---|---|---|---|
| 1 | `Span<T>` / `ReadOnlySpan<T>` | dizi/yığın üzerinde kesit görünümü | yok |
| 2 | `Memory<T>` / `ReadOnlyMemory<T>` | `Span`'in kalıcı (GC korumalı) kardeşi | yok |
| 3 | `MemoryMarshal.GetReference` / `GetArraySegment` | kesitten referans/dizi elde etme | yok |
| 4 | `stackalloc T[n]` | yığında geçici tampon | yok |
| 5 | `Encoding.GetString(ReadOnlySpan<byte>)` | byte → string tek geçişte | dizi kopyası **yapmaz** |

---

## §1 Geçiş tablosu (hangi yol seçilir)

| Gidiş | Yol | Kod parçası | Kaçınılır |
|---|---|---|---|
| `string` → native tampon | `Encoding.UTF8.GetBytes(s, span)` | `Span<byte>` + `stackalloc` | `GetBytes()` dizi kopyası (gereksiz) |
| native tampon → `string` | `Encoding.UTF8.GetString(ReadOnlySpan<byte>)` | tek çağrı | `ToArray()` + `GetString(byte[])` |
| `byte[]` kesiti | `arr.AsSpan(5, 10)` | görünüm | `arr.Skip(5).Take(10)` (LINQ = alloc) |
| `Span` → P/Invoke (`byte*`) | `fixed (byte* p = arr)` ya da `MemoryMarshal.GetReference` | `unsafe` bloğu | `GCHandle` elle yönetmek |
| Uzun ömürlü saklama / `async` | `Memory<T>` ya da `byte[]` | alan olarak | **`Span<T>` alanı (derleme hatası)** |
| Kuyruğa / iş parçacığına geçiş | dizi kopyası (açıkça) | `ToArray()` | paylaşılan tampon (yarış) |

```csharp
// byte[] -> string: kopyasiz gecis (net48 + net10 ayni kod)
public static string Decode(ReadOnlySpan<byte> utf8)
{
    return Encoding.UTF8.GetString(utf8);
}

// gecici tampon: yiginda, GC baskisi yok
public static int HashOf(byte[] data)
{
    Span<byte> tmp = stackalloc byte[data.Length];
    data.CopyTo(tmp);
    return BitConverter.ToInt32(tmp);      // kopya yok, alloc yok
}
```

---

## §2 Interop (native sınır) kuralları

| Kural | Neden |
|---|---|
| Tampon **çağrı sırasında** yaşamalı (`fixed` / `Span` scope'u içinde) | native kod imleci kullanırken GC taşınabilir |
| Native'e giden veri **açıkça kopyalanıyorsa** bunu yaz (net:`_` / `in` / `ref`) | kopya maliyeti görünür olsun |
| Native'den dönen belleği **kim serbest bırakır** tablosu yazılır | sızıntı / çift free |
| `DllImport` yerine imzaya göre seçim (bkz. §4) | bacak farkı |
| Girdi **doğrulanmadan** native'e verilmez | tampon taşması (G2/CRITICAL) |

| İmza yolu | `net48` | `net10.0-windows` |
|---|---|---|
| `DllImport` + `MarshalAs` | **tek yol** | geçerli (yalnız `unsafe` gerekirse `fixed`) |
| `LibraryImport` (kaynak üretilmiş marshalling) | **yok** | var — tercih edilir |
| `Span<T>` parametreli `DllImport` | yok (dizi/koleksiyon ile) | `LibraryImport` + `Span` destekli |
| `unsafe` bloğu / `AllowUnsafeBlocks` | gerekirse `.csproj`'da açılır | aynı |

> **KURAL:** Paylaşılan dosyada `LibraryImport` **kullanılmaz**; ortak kod `DllImport` + dizi imzasıyla kalır, `LibraryImport` varyantı `#if !NETFRAMEWORK` içinde ayrı dosyada durur (yalnızca API farkı — G6).

---

## §3 `Span<T>` kullanılamaz (derleme hatası) — 5 madde

1. `async` metot gövdesi ve `await` sonrası (`ref struct` `async`'e girmez).
2. `yield return` içinde.
3. Alan (field) olarak sınıfta — `ref struct` referans tipi olamaz.
4. `lock`/`try-catch` içinde serbest bırakılması gereken uzun ömürlü saklama (kapsam dışına çıkamaz).
5. Generik argüman olarak (`Span<T>` `ref struct`, `T` yerine geçemez) — çözüm: dizi/`Memory<T>` üzerinden yaz.

| Belirti | Kök neden | Düzeltme |
|---|---|---|
| `CS4007` / `ref struct` hatası | `async` içinde `Span` | `Memory<T>` ya da işi `async` dışına al |
| `CS0213` (yardımcı metod kullanılamaz) | `fixed` yalnızca sabitboyutlu dizi/alan üzerinde | `GCHandle.Alloc(..., Pinned)` ya da dizi kullanımı |
| Çalışma zamanı `TypeInitializationException` | `stackalloc` ile dev tampon (yığın taşması) | büyük tamponlarda `ArrayPool<T>.Shared.Rent` |

---

## §4 net48 / net10.0-windows farki

| Konu | `net48` | `net10.0-windows` |
|---|---|---|
| `Span<T>` / `Memory<T>` | **NuGet: `System.Memory`** paketi gerekir | runtime ile gelir |
| `Encoding.GetString(ReadOnlySpan<byte>)` | paket üzerinden var | var |
| `ArrayPool<T>` | `System.Buffers` paketi | runtime |
| `LibraryImport` | **yok** | var (C# 9+ / .NET 7+ kaynağı) |
| `stackalloc` | var (C# 7.3 ile) | var |
| Dil | **C# 7.3**: `Span` API'si var, `using var`/`ref` dönüş yok | C# 14 |

---

## §5 Kardesler

| Konu | Dosya |
|---|---|
| Koleksiyon/alloc seçimi | [collections-choice](collections-choice.md) |
| Dil sürümü farkları | [language-versions-net48-net10](language-versions-net48-net10.md) |
| Elle byte/string işleme güvenliği | [security-rules](../../.rules/security-rules.md) |

---

## §6 Yasaklar

1. Byte veriyi **onaylamadan** (uzunluk/doğrulama) native'e geçirmek — tampon taşması (CRITICAL).
2. `Span<T>` ile **kuyruk/thread** arası veri paylaşımı (paylaşılan yığın tamponu yarıştırır).
3. Performans gerekçesiyle kopya **gizlemek**: kopya kaçınılmazsa `ToArray()` açıkça yazılır (G5 kanıt kuralı).
4. EF Core; ham veri erişimi **ADO.NET + Dapper**, sorgu `@p1` parametreli.
5. Kanıtsız "kopya yok" iddiası — kanıt: `BenchmarkDotNet` ölçümü ya da `GC.GetAllocatedBytesForCurrentThread()` çıktısı.
