---
title: "Dugum Detayı N02 — Result ve Error Tipleri (l0)"
type: architecture
category: architecture
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "Result/Result<T>/Error/ErrorCode sozlesmesi, basari-basarisizlik kurallari, hata dili ve hata kodlarinin ekran karsiliklari"
---

# Dugum Detayı N02 — Result ve Error Tipleri (l0)

**Zorunlu Baglantilar:** [Mimari index](../index.md) · [katman spec](index.md) · [dependency-map](../00-overview/dependency-map.md) · [ADR-028](../../.decisions/accepted/ADR-028-layered-baseline-9-projects.md) · [test-strategy](../../spec/test-strategy.md) · [N01-ports-core](N01-ports-core.md)

> **Tek cumlelik sorumluluk:** *Bir islem basarili mi, degilse hangi hata koduyla neden basarisiz* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

VersaCoder'da **beklenen hata degerdir, istisna degildir**: her katman donusunu `Result` / `Result<T>` ile verir; hata `ErrorCode` + kullaniciya uygun Turkce `Message` + yalniz loga `Detail` icerir. Bu tip **bu projede tek tanimlidir**; ikinci bir sonuc tipi iki hata dili dogurur. Istisna yalnizca programlama hatasindadir (null arguman) — o da fail-fast ile patlar, `Result` ile gizlenmez.

| Alan | Deger |
|---|---|
| N kodu / alt katman | N02 · result-error |
| Makro katman / proje | l0 Abstractions · `VersaCoder.Abstractions` |
| Ureten | l1..l7 tum katmanlar (imza l0'da) |
| Tuketen | tum katmanlar + UI ekran davranisi |
| Gate | GATE-004a -> GATE-003 -> GATE-004 |

---

## §1 Sorumluluk (NE YAPAR)

| # | Tip | Imza (ozet) | Sorumluluk |
|---|---|---|---|
| 1 | `ErrorCode` (enum) | `None, Invalid, NotFound, Conflict, Unauthorized, Timeout, Cancelled, Database, Protocol, NotImplemented` | hata siniflandirmasi |
| 2 | `Error` | `Code`, `Message`, `Detail`, `Fields` | tek hata nesnesi; `Fields` alan bazli dogrulama |
| 3 | `Result` | `IsSuccess`, `Error`, `Ok()`, `Fail(Error)` | degeri olmayan islem sonucu |
| 4 | `Result<T>` | `IsSuccess`, `Value`, `Error`, `Ok(T)`, `Fail(Error)` | degeri olan islem sonucu |
| 5 | S1..S5 sozlesme kurallari | `Value` = `default(T)` (asla throw) | basarisizlikta cagiran dallanir |

---

## §2 Bu Katmana GIRENLER / CIKANLAR

| Yon | Kim / ne | Tur | Kosul |
|---|---|---|---|
| GIREN | l1 invariant ihlali (`Conflict`) | girdi | fail-fast degil, `Result.Fail` |
| GIREN | l2 dogrulayici (`Invalid` + `Fields`) | girdi | form alani hatasi |
| GIREN | l4 altyapi hatasi (`Database`) | girdi | mesaj temizlenmis olmali |
| CIKAN | `Result<T>` donusu | cikti | tum katmanlar ayni hata dilini okur |
| CIKAN | ekran davranisi maplemesi (Invalid -> alan alti metin) | cikti | l7 |
| YASAK | ikinci sonuc tipi (`MyResult`) | yasak | Arch testi: tek `Result<T>` |

---

## §3 Yasak Yonler (ADR-028 matrisi)

| # | Yasak yon | Kaynak | Bu dugumdeki karsiligi |
|---|---|---|---|
| 1 | `l0 -> l1..l7` | ADR-028 §2.1 | `ErrorCode` hicbir katman adini tasmaz |
| 2 | `l2 -> l4` | ADR-028 §2.1 | handler hata olusturur, `SqlException` gormez |
| 3 | `l4 -> l2` | ADR-028 §2.3/2 | adaptorden use case tipine geri bakis yok |
| 4 | `l3 -> l1, l2, l4..l7` | ADR-028 §2.1 | log (l3) yalniz `Error` nesnesini okur |
| 5 | `l7 -> l1..l5` | ADR-028 §2.1 | ekran yalniz `Result` + `ErrorCode` bilir |

---

## §4 N Kodu ve Proje

| Alan | Deger |
|---|---|
| Katalog kaydi | `../catalog/index.md` altinda `N02` satiri |
| Kod yolu | `src/VersaCoder.Abstractions/Results/` (`Result.cs`, `ResultT.cs`, `Error.cs`, `ErrorCode.cs`) |
| Proje | `VersaCoder.Abstractions` (tek .csproj, cift TFM) |

---

## §5 Bagimlilik Listesi

| # | Bagimlilik | Yon | Tur | Not |
|---|---|---|---|---|
| 1 | BCL (`System.Collections.Generic`) | l0 -> System | **izinli** | `IReadOnlyDictionary` icin |
| 2 | NuGet paketi | l0 -> paket | **yasak** | `.csproj` referansi 0 |
| 3 | Loglestirme kutuphanesi | l0 -> l3 | **yasak** | `Error` log yazmaz, tasiyici olur |
| 4 | l1..l7 -> `Result<T>` | ust -> l0 | **izinli** | tek hata dili |
| 5 | Yazisma/ceviri katmani | - | **yasak** | `Message` her zaman Turkce |

---

## §6 Test Stratejisi

| Test turu | Ne dogrular | Hedef |
|---|---|---|
| Unit | `Ok` yolunda `IsSuccess = true`, `Value` dogru | %100 satir |
| Unit | `Fail` yolunda `Value = default(T)` ve `Error` korunur (S2) | %100 |
| Unit | `Fields` doluysa alan bazli hata gosterimi | %90 |
| Arch | `Result<T>` yalniz tek projede tanimli | 0 ikinci tip |
| Arch | `ErrorCode` bilesen bazinda kopyalanmis enum yok | 0 kopya |

---

## §7 Hata Durumlari

| `ErrorCode` | Ne zaman | Kim uretir | Ekran karsiligi |
|---|---|---|---|
| `Invalid` | alan dogrulamasi basarisiz | l2 dogrulayici + l1 invariant | alan alti kirmizi metin |
| `NotFound` | id/kayit yok | l4 port | bilgi cubugu |
| `Conflict` | invariant / durum catismasi | l1 | uyari penceresi, islem durur |
| `Timeout` | `CommandTimeout` / ag | l4 | "tekrar dene" butonu |
| `Database` | SQL/IO (mesaj **temizlenmis**) | l4 | hata paneli + log no |

---

## §8 Kurallar

- `Message` her zaman Turkce ve kullaniciya uygundur; `Detail` yalniz loga gider — karistirilmaz.
- `Result` ile gizlenen istisna yoktur: null arguman fail-fast patlar (programlama hatasi).
- Istisna **asla** katman sinirini cikmaz; yakalayan l2/l4 `Result.Fail` donecektir.
- `ErrorCode` yeni deger eklendiginde ekran maplemesi (l7) de ayni gun guncellenir.
- Bu tip l0 disinda **yeniden tanimlanamaz** (G1/SSUT): ikinci sonuc tipi iki hata dilidir.

---

## §9 Kanit

| # | Iddia | Kanit | Durum |
|---|---|---|---|
| 1 | 6 baglantinin hedefi diskte var | `check-links.ps1` -> bozuk link **0** | olcum |
| 2 | Dosya 3-8 KB | `Get-Item N02-result-error.md).Length` | olcum |
| 3 | Tek sonuc tipi kurali | Arch testi: `Result<T>` aranir, baska sonuc tipi bulunmaz | test |
| 4 | Yon kurali kaynagi | [ADR-028](../../.decisions/accepted/ADR-028-layered-baseline-9-projects.md) §2.1 | dogrulandi |
| 5 | Madde listesi <= 5 | §8 = 5 madde | sayim |
