---
title: "Dugum Detayı N04 — Guard ve Extension (l0)"
type: architecture
category: architecture
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "Guard (on kosul) ve extension yardimcilarinin imzalari, istisna/Result siniri, kulturel varsayilan riski ve iki TFM uyumu"
---

# Dugum Detayı N04 — Guard ve Extension (l0)

**Zorunlu Baglantilar:** [Mimari index](../index.md) · [katman spec](index.md) · [dependency-map](../00-overview/dependency-map.md) · [ADR-028](../../.decisions/accepted/ADR-028-layered-baseline-9-projects.md) · [test-strategy](../../spec/test-strategy.md) · [N05-constants](N05-constants.md)

> **Tek cumlelik sorumluluk:** *On kosul (guard) nerede patlar, nerede `Result` dondurur ve genisletme metodu ne yapabilir, ne yapamaz* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Guard, **programlama hatasini** (null / bos deger / aralik disi) istisnalarla **erken patlatir**; is kurali hatasi ise `Result.Fail(Invalid)` ile doner. Extension metotlar **durum degistirmez**, yalniz dogrulama ve bicim donusumu yapar — is mantigi yazildigi an bu dugum l1'e tasmistir. BCL disi hicbir paket kullanilmaz; `net48` ile `net10.0-windows` arasindaki kulturel fark icin karsilastirma `StringComparison.Ordinal` olarak yazilir.

| Alan | Deger |
|---|---|
| N kodu / alt katman | N04 · extensions |
| Makro katman / proje | l0 Abstractions · `VersaCoder.Abstractions` |
| Giren cagri | l1..l7 public API girisleri |
| Cikti | ayni deger (zincirleme) ya da fail-fast istisna |
| Risk | kulturel varsayilan (turkce I/istanbul) — tek gercek TFM riski |

---

## §1 Sorumluluk (NE YAPAR)

| # | Yardimci | Imza (ozet) | Sorumluluk | Donus |
|---|---|---|---|---|
| 1 | `Guard.NotNull` | `T NotNull<T>(T value, string name)` | null giris | istisna |
| 2 | `Guard.NotEmpty` | `string NotEmpty(string value, string name)` | bos/kirpilmis metin | istisna |
| 3 | `Guard.InRange` | `int InRange(int v, int min, int max, string name)` | aralik disi | istisna |
| 4 | `Guard.ValidState` | `void ValidState(bool condition, string message)` | on kosul (contract) | istisna |
| 5 | `StringExtensions` | `Normalize`, `IsBlank` | kucuk harf + kirpma (Ordinal) | deger |
| 6 | `CollectionExtensions` | `IsEmpty<T>` | bos koleksiyon | deger |

---

## §2 Bu Katmana GIRENLER / CIKANLAR

| Yon | Kim / ne | Tur | Kosul |
|---|---|---|---|
| GIREN | l1..l7 public API cagrilari | girdi | yalnizca API girisinde |
| GIREN | is kurali ihlali sorgusu | girdi | guard degil `Result.Fail` |
| CIKAN | ayni deger (zincirleme) | cikti | durum degisikligi yok |
| CIKAN | fail-fast istisna | cikti | cagiran yakalar, testte patlar |
| YASAK | guard govdesinde is mantigi | yasak | mantik l1'e tasmistir |

---

## §3 Yasak Yonler (ADR-028 matrisi)

| # | Yasak yon | Kaynak | Bu dugumdeki karsiligi |
|---|---|---|---|
| 1 | `l0 -> l1..l7` | ADR-028 §2.1 | guard `VersaCoder.Domain` gormez |
| 2 | `l2 -> l4` | ADR-028 §2.1 | dogrulama `DbConnection` gormez |
| 3 | `l4 -> l2` | ADR-028 §2.3/2 | adaptorden handler tipine bakis yok |
| 4 | `l7 -> l1..l5` | ADR-028 §2.1 | ekran guard'i l6 DI uzerinden alir |
| 5 | `l3 -> l1, l2, l4..l7` | ADR-028 §2.1 | log yalniz sonucu okur |

---

## §4 N Kodu ve Proje

| Alan | Deger |
|---|---|
| Katalog kaydi | `../catalog/index.md` altinda `N04` satiri |
| Kod yolu | `src/VersaCoder.Abstractions/Guard/` + `Extensions/` |
| Proje | `VersaCoder.Abstractions` (tek .csproj, cift TFM) |

---

## §5 Bagimlilik Listesi

| # | Bagimlilik | Yon | Tur | Not |
|---|---|---|---|---|
| 1 | BCL (`System`, `System.Globalization`) | l0 -> System | **izinli** | saf yardimci |
| 2 | NuGet paketi | l0 -> paket | **yasak** | `.csproj` referansi 0 |
| 3 | `VersaCoder.Domain/Application` | l0 -> l1/l2 | **yasak** | is mantigi tasimiyor |
| 4 | l1..l7 -> `Guard.*` | ust -> l0 | **izinli** | API girisinde |
| 5 | Test projesi -> guard testleri | test -> l0 | **kosullu** | istisna tipi assertion |

---

## §6 Test Stratejisi

| Test turu | Ne dogrular | Hedef |
|---|---|---|
| Unit | her guard icin gecersiz girdi -> dogru istisna tipi + parametre adi | %100 |
| Unit | guard gecerli girdide ayni degeri dondurur | %100 |
| Unit | `Normalize` kucuk harf + kirpma + turkce I/istanbul (Ordinal) | %100 |
| Arch | extension govdesinde `VersaCoder.Domain/Application` referansi yok | 0 |
| Arch | NuGet = 0 | `.csproj` denetimi |

---

## §7 Hata Durumlari

| Durum | Sonuc | `ErrorCode` | Kim yakalar |
|---|---|---|---|
| `null` arguman | `ArgumentNullException` | - | cagiran (testte patlar) |
| Bos metin / kirpilmis bosluk | `ArgumentException` | - | cagiran |
| Aralik disi deger | `ArgumentOutOfRangeException` | - | cagiran |
| Oncosul saglanmadi | `InvalidOperationException` | - | cagiran |
| **Is kurali** ihlali | guard degil `Result.Fail` | `Invalid` / `Conflict` | l1 invariant, l2 dogrulayici |

---

## §8 Kurallar

- Guard **yalnizca** API girisinde kullanilir; dongu ici her adimda cagrilmasi sicak yolda is israfidir.
- `Normalize` icin `StringComparison.Ordinal` **acikca** yazilir; kulturel varsayilan iki bacakta farkli cikti verir.
- Extension durum degistirmez; degistirdigi an bu dugum l1'e tasmistir (reviewer bulgusu: HIGH).
- Is kurali hatasi guard ile degil `Result.Fail(Invalid)` ile doner — iki yol karistirilmaz.
- C# 9+ ozellikleri (`record`, `init`, switch expression) ortak kaynakta kullanilmaz (net48 dili C# 7.3).

---

## §9 Kanit

| # | Iddia | Kanit | Durum |
|---|---|---|---|
| 1 | 6 baglantinin hedefi diskte var | `check-links.ps1` -> bozuk link **0** | olcum |
| 2 | Dosya 3-8 KB | `Get-Item N04-extensions.md).Length` | olcum |
| 3 | Ordinal karsilastirma testi | `Normalize` testi iki bacakta ayni cikti | test |
| 4 | Yon kurali kaynagi | [ADR-028](../../.decisions/accepted/ADR-028-layered-baseline-9-projects.md) §2.1 | dogrulandi |
| 5 | Madde listesi <= 5 | §8 = 5 madde | sayim |
