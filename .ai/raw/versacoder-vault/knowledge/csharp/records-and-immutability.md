---
title: "records-and-immutability - VersaCoder"
type: knowledge
category: csharp
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "record ile degismezlik, value-equality ve with ifadesi; net48'de ayni sozlesmenin alternatif karsiligi"
---

# records-and-immutability — Record ve degismezlik

**Zorunlu Baglantilar:** [index](../index.md) · [coding-standards](../../.rules/coding-standards.md) · [pattern-matching](pattern-matching.md) · [interfaces-vs-abstract](interfaces-vs-abstract.md)

> **Tek cumlelik sorumluluk:** *Degismez bir veri tipi nasil tanimlanir, record hangi bacakta var ve net48 karsiligi nedir* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

`record` **C# 9 = yalniz `net10.0-windows` bacakta**; `net48` (C# 7.3) ayni sozlesmeyi **elle yazilmis sealed class + okunur property + `IEquatable<T>`** ile karsilar. Degismezlik bir *anahtar* degil, bir **sözlesmedir**: disariya acan tip set-only setter veya readonly alan kullanir, degisiklik `with`/yeni instance ile yapilir. Bu vault'ta DTO/deger tipi nerede `record` olacagi, nerede 7.3-uyumlu class olacagi **iki bacak tablosuna gore** karar verilir; "her yere record" bir surum sapmasidir (G6).

---

## §1 Iki Bacak Tablosu

| Ozellik | net48 · C# 7.3 | net10.0-windows · C# 14 |
|---|---|---|
| `record` / `record struct` (C# 9/10) | **Yok** | Var |
| `with` ifadesi (C# 9) | **Yok** — elde yeni instance | Var |
| Value-equality (alan bazli `Equals`) | Elle: `IEquatable<T>` + `GetHashCode` | Derleyici uretir |
| `init` setter (C# 9) | **Yok** — `private set` + kurucu | Var |
| `required` alan (C# 11) | **Yok** | Var |
| Primary constructor (C# 12) | **Yok** — klasik kurucu | Var |
| `readonly struct` + `readonly` property (7.2) | Var | Var |
| Immutable nesne ornegi (elle) | `sealed class` + set-only property | `record` |
| `Deconstruct` | Elle yazilir | Derleyici uretir |

> **KURAL:** Ortak dosyada `record` **yazilmaz**; ortak imza 7.3 kalir. `record` yalniz `net10`'a ozel dosyada kullanilir ve o dosyanin bacak sorumlulugu acikca yazilir.

## §2 Degismezlik Karsilastirmasi

| Yaklasim | Degismezlik | Esitlik | Allokasyon | Bacak |
|---|---|---|---|---|
| `record` | Dil garanti (`init`/kurucu) | Derleyici (alan bazli) | Yeni instance (`with`) | net10 |
| `sealed class` + `private set` | Elle | `Equals` elle yazilir | Yeni instance | **iki bacak** |
| `readonly struct` | Bellek duzeyi | `Equals` gerekir (yoksa boxing) | Kopya (deger tipi) | **iki bacak** |
| `ReadOnlyCollection<T>` | Sarma | Kaynak tipin esitligi | Sarma | **iki bacak** |

Kucuk ve sik gecen veri (koordinat, aralik, tek satir DTO) -> `readonly struct`; referans paylasilan veri -> `sealed class`; degistirilebilir topluluk -> yalniz ic tip olarak `List<T>` + disariya `IReadOnlyList<T>`.

## §3 Ornek

net10 bacak (record):

```csharp
internal sealed record SessionDto(int Id, string Ad, DateTime OlusturmaUtc)
{
    internal SessionDto YeniAd(string ad) => this with { Ad = ad };   // degisiklik = yeni ornek
}
```

Iki bacak (7.3 uyumlu ayni sozlesme):

```csharp
internal sealed class SessionDto
{
    internal SessionDto(int id, string ad)
    {
        Id = id;
        Ad = ad ?? throw new ArgumentNullException(nameof(ad));
    }

    internal int Id { get; }
    internal string Ad { get; }
    internal SessionDto YeniAd(string ad) => new SessionDto(Id, ad);
}
```

| Kontrol | record | Elle (iki bacak) |
|---|---|---|
| Degisiklik `with` | Evet | Hayir — yeni instance |
| `Equals` alan bazli | Evet | Elle yazilmali |
| `net48` derlenir | **Hayir** | Evet |

## §4 Sik Hatalar

| # | Hata | Sonuc | Dogrusu |
|---|---|---|---|
| 1 | `record`'u ortak dosyaya yazmak | net48 derleme hatasi | §1 tablosu |
| 2 | `with` ile kopya sanip `mutable` alan tasmak | Deger degismez gorunur, ic kayar | Derin degismezlik |
| 3 | `DateTime`'i `Local` tutmak | Zaman dilimi kaymasi | UTC + `TimeProvider`/tek saat (l3 clock) |
| 4 | Record'da `GetHashCode`'yu anahtar yapmak | Alan degisince hash kayar | Anahtar stabil alan |
| 5 | `record` = API sozlesmesi sanmak | Serilestirme degisir | Serilestirme ayri karar |

## §5 Dogrulama

| Kontrol | Kanit |
|---|---|
| Degismezlik | Degisiklik testi: yeni ornek, eski degismedi |
| Iki bacak | `net48` build 0 hata (record dosyasi ortak degilse) |
| Kural | [coding-standards](../../.rules/coding-standards.md) |
| Kardesler | [pattern-matching](pattern-matching.md) · [serialization-choices](serialization-choices.md) |

`record`'un serilestirme davranisi test edilmeden yayina alinmaz; `NEEDS_VALIDATION` olarak isaretlenir.
