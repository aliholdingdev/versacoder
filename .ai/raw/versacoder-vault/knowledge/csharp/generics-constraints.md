---
title: "generics-constraints - VersaCoder"
type: knowledge
category: csharp
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "where kilavuzlarinin sinifi, C# 7.3'te mevcut kisitlar ve C# 9-14'te eklenenlerin iki bacak dagilimi"
---

# generics-constraints — Generic kısıtlar

**Zorunlu Baglantilar:** [index](../index.md) · [coding-standards](../../.rules/coding-standards.md) · [language-versions-net48-net10](language-versions-net48-net10.md) · [interfaces-vs-abstract](interfaces-vs-abstract.md)

> **Tek cumlelik sorumluluk:** *Bir `where` kisiti hangi bacakta derlenir ve tip T ile ne yapabilecegimi sinirlar* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Kisit, **derleme zamaninda** "T ile neler yapabilirim"i belirler; kisitsiz generic kod yalnizca `object` uzerindeki islemleri (`==`, `ToString`) yapabilir. `net48` bacakta varsayilan dil **C# 7.3** oldugu icin 7.3'te var olan kisitlar (`class`, `struct`, `unmanaged`, `new()`, `Enum`, `Delegate`, arayuz + temel tip) **iki bacakta da** calisir; `notnull` (8), `allows ref struct` (13), statik soyut arayuz uyesi (11) yalniz `net10` bacakta kalir. Bu vault kurali: **ortak imza 7.3 kisitlarinda kalir**, daha yenisi lazimsa dosya iki bacakli mi diye once [language-versions-net48-net10](language-versions-net48-net10.md) tablosuna bakilir.

---

## §1 Kisit Katalogu (iki bacak)

| Kisit | Anlam | net48 · C# 7.3 | net10.0-windows · C# 14 |
|---|---|---|---|
| `where T : class` | Referans tipi, null olabilir | Var | Var |
| `where T : struct` | Value tipi | Var | Var |
| `where T : class, notnull` | Referans + null olamaz (NRT) | **Yok** (C# 8, NRT bagli) | Var |
| `where T : unmanaged` | Safe handle disi ham bellek | Var (7.3) | Var |
| `where T : new()` | Parametresiz kurucu zorunlu | Var | Var |
| `where T : Enum` | Bir enum tipi | **Var (7.3 ile geldi)** | Var |
| `where T : Delegate` | Bir delegate tipi | **Var (7.3 ile geldi)** | Var |
| `where T : IRepo` (arayuz) | Arayuz uyesi | Var | Var |
| `where T : TemelTip` | Kalitim + arayuzlar (tek temel) | Var | Var |
| `where T : allows ref struct` | `ref struct` T olarak kullanilsin | **Yok** (C# 13) | Var |
| `where T : IComparable<T>` gibi birlesik | Sart `&` ile | Var | Var |
| Statik soyut arayuz uyesi (`IParsable` benzeri) | Interface icinde `static abstract` | **Yok** (C# 11 + runtime) | Var |

> **KURAL:** Kisit **arttirilmaz**, *genisletilmez*; `new()` eklemek cagiranlari kirar. Imza degisikligi public API'de review'e girer (SRP: imza `developer`, sozlesme `architect`).

## §2 Ne Kazandirir

| Kisit eklenince | Yazilabilen kod | Etki |
|---|---|---|
| `class` | `is`, `== null`, referans karsilastirma | En yaygin |
| `struct` | `default(T)`, null yok, stack'te | Alloc dusturur |
| `unmanaged` | `Span<byte>` reinterpret, pointer | Dusuk seviye |
| `new()` | `new T()` (factory yerine) | Kurucu gorevi yukler |
| `Enum` / `Delegate` | `Enum.Parse`, `DynamicInvoke` guvenli siniflandirma | C# 7.3'te iki bacakta |
| Kisit **yok** | Yalniz `object` uyelikleri | En dar |

## §3 Ornek

Iki bacakta derlenen — yalniz 7.3 kisitlari:

```csharp
internal static class Donusturucu<TGirdi, TSonuc>
    where TGirdi : class, new()      // referans + parametresiz kurucu
    where TSonuc : struct            // value tipi
{
    internal static TSonuc Cevir(TGirdi girdi)
    {
        if (girdi is null) throw new ArgumentNullException(nameof(girdi));
        var ara = new TGirdi();
        return Donustur(ara, girdi);
    }

    private static TSonuc Donustur(TGirdi ara, TGirdi girdi) => default;
}
```

`net10` bacakta eklenen (ortak dosyaya **yazilmaz**, ayrildirsa `#if` yalniz platform farkinda):

```csharp
internal static T Olustur<T>() where T : allows ref struct => default;  // C# 13
```

## §4 Sik Hatalar

| # | Hata | Sonuc | Dogrusu |
|---|---|---|---|
| 1 | `notnull`/`allows ref struct` ortak dosyaya yazmak | net48 derlemesi kirilir | §1 tablosundan kontrol |
| 2 | `new()` ekleyip kurucuyu unutmak | `new T()` her yerde hata | Kisit + kurucu birlikte |
| 3 | Kisitla `struct` deyip `default` okumak | `T` null dondurulebilir sandi | Kisitin ne garantiledigini oku |
| 4 | Generic'i `object` ile yazip sonra generic yapmak | Tip guveni kaybi | En dar kisitle basla |
| 5 | Kisit imzasinin daraltilmasi (public) | Cagiran kirilir | Imza degisikligi = review |

## §5 Dogrulama

| Kontrol | Kanit |
|---|---|
| Her `where` | §1 tablosunda iki bacak da "Var" mu? |
| Build | `net48` + `net10.0-windows` 0 hata |
| Kural | [coding-standards](../../.rules/coding-standards.md) |
| Kardesler | [interfaces-vs-abstract](interfaces-vs-abstract.md) · [records-and-immutability](records-and-immutability.md) |

Kisit davranisiyla ilgili olcülmemis iddia varsa `NEEDS_VALIDATION` yazilir; tahminle public imza acilmaz.
