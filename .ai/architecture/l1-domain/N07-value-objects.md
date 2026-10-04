---
title: "N07 - Value Objects (Immutability, Equals, Validation)"
type: architecture
category: l1-domain
status: active
version: 1.0
updated: 2026-10-04
authority: SSOT
scope: VersaCoder
---

# N07 - Value Objects (Immutability, Equals, Validation)

**Zorunlu Baglantilar:** [index.md](index.md) | [N06-entities.md](N06-entities.md) | [N08-aggregates.md](N08-aggregates.md) | [N10-invariants.md](N10-invariants.md) | [N11-enums.md](N11-enums.md) | [N46-macro-l1.md](N46-macro-l1.md) | [ADR-028](../../.decisions/accepted/ADR-028-layered-baseline-9-projects.md) | [ADR-013](../../.decisions/accepted/ADR-013-layered-architecture-52-nodes.md) | [layer-boundary-check](../../checklists/layer-boundary-check.md) | [test-strategy](../../spec/test-strategy.md)

> **Tek cumlelik sorumluluk:** Basliksiz, degistirilemez ve esitligi deger uzerinden tanimlanan tipleri uretir; ham tip (string/int/decimal) yerine anlamli tip kullanmayi zorunlu kilar.

## §0 Ozet (Answer First)

N07, ayni ham tipten farkli anlam cikariyor: `Email`, `Money`, `Slug`, `Duration`. Iki `Money` degeri ayni tutarsa aynidir, kimlik degildir. Alanlar `init`/`readonly` ile uretilir, sonra degistirilemez. Dogrulama yapici (constructor/factory) tarafindan **kurulus aninda** yapilir; gecersiz deger hic var olmaz. L1'de bulunur: hem domain hem application tarafindan kullanilir, aslinda hicbir yone gitmez (diger katmanlari tanimaz).

**Kunye:**

| Ozellik | Deger |
|---|---|
| Katman | l1-domain |
| Proje | VersaCoder.Domain (ADR-028) |
| N kodu | N07, [ADR-013](../../.decisions/accepted/ADR-013-layered-architecture-52-nodes.md) |
| Kullanim | N06, N08, N10 ve L2 DTO/deger tasiyici olarak |
| Varsayilan yon | YOK (L1 hic katmana bagimli degil) |

## §1 Sorumluluk

Kapsadigi tek sey: **deger tasiyan tip**. Bir tip asagidaki dort ozellikten birini tasiyorsa N07 adayidir:

1. Esitlik deger uzerinden tanimlanir (referans == degil).
2. Nesne olustuktan sonra ic durumu degismez.
3. Kendi dogrulama kuralini tasir (invariant'i kendi icinde).
4. Dis identifier'i (Guid, Id) yoktur; birebir karsiligi yoktur.

Neler disaridir: kimlikli nesneler ([N06](N06-entities.md)), davranisli kume kokleri ([N08](N08-aggregates.md)), ham primitif sarmalayici olmayan anlamsiz wrapper'lar.

## §2 Girenler / Ciktilar

| Yon | Ne | Kaynak |
|---|---|---|
| GIRENLER | Ham primitif (string, int, decimal, DateTime) | L2 komut, L3 istek, L6 dosya |
| GIRENLER | Dogrulama kurali (format, aralik, bos olmama) | [N10](N10-invariants.md), uzman kurali |
| GIRENLER | Semaphore / kilit kurali (L1'de senkronizasyon yok) | Yok |
| CIKANLAR | Degistirilemez, esitligi deger bazli tip | L1 ic kullanim, L2 DTO |
| CIKANLAR | Kurulus aninda patlama (invariant ihlali) | Hata yonetimi: Result/Either |
| CIKANLAR | Kesisim testi icin sozlesme (ToString, Eslesme) | Testler |

Ozet: giris primitif + kural, cikis anlamsiz ve guvenli tip.

## §3 Yasak Yonler

| Yon | Deger | Neden |
|---|---|---|
| N07 -> l2-application | X | Ileride bakar (Yukariye Bakma) |
| N07 -> l3-crosscutting | X | Altyapi tipi tanimaz (ADR-028 matrisi) |
| N07 -> l4-infrastructure | X | Klasik dibl: domain, infra'ya bakmaz |
| N07 -> l5-presentation | X | UI modeli ile deger tipi karistirilmaz |
| N07 -> l6-database | X | Sutun tipi ile deger tipi ayni sey degil |
| N07 -> l7-testing | X | Test tipi uretilmez |

Izinli: L0'a bakma YOK (deger tipi alt katman kullanmaz); L0'dan L1'e bakma [N45](../index.md) uzerinden port bagimi olarak zaten var. Fiziksel dogrulama icin [layer-boundary-check](../../checklists/layer-boundary-check.md).

## §4 N Kodlari ve Proje

| N | Ad | Iliski | Proje |
|---|---|---|---|
| N07 | Value Objects | N06/N08 alan tipi | VersaCoder.Domain |
| N06 | Entities | N07 alanlarini kullanir | VersaCoder.Domain |
| N08 | Aggregates | N07 + N06 kokeyn | VersaCoder.Domain |
| N10 | Invariants | N07 ic dogrulama | VersaCoder.Domain |
| N11 | Enums | N07 ile kesisir (sinirli deger) | VersaCoder.Domain |

## §5 Bagimlilik Listesi

| Bagimlilik | Yon | Not |
|---|---|---|
| System (primitive'ler) | ic | Sadece BCL tipi |
| N10 invariants | ic | Kural ortakligi |
| N06 entities | ic | Tip iliskisi |
| L2, L3, L4 | YOK | Sifir bagimlilik |
| Harici paket | YOK | Cikti tasiyorsa dictionary/generic tercih edilir |

## §6 Test Stratejisi

| Katman | Ne test edilir | Arac |
|---|---|---|
| Unit | Degistirilemezlik, esitlik (== yerine Equals) | xUnit |
| Unit | Gecersiz giriste Result/Either hatasi | xUnit |
| Unit | Ayni deger, farkli referans => esit | xUnit |
| Integration | Veri katmaninda karsiligi var mi (gerekirse) | EF Core InMemory |
| E2E | L2 komutundan L1'e dogru tip gelir mi | Playwright |

## §7 Hata Durumlari

| Durum | Davranis |
|---|---|
| Gecersiz format | Kurulus firlatir, hic dondurulmez |
| Bos deger | Result/Either ile domain failure |
| Null ham deger | ArgumentNullException veya Result failure |
| Kesisim (iki tip ayni deger uretir) | Testle yakalanir, zamanlamada degil |

## §8 Kurallar

- Alanlari `init` veya `readonly` ile tanimla; `set` yok.
- Esitligi `Equals` + `GetHashCode` ile deger uzerinden ver.
- Dogrulamayi constructor/factory'de yap; sonradan "duzeltme" metodu yazma.
- Ham tipi disariya `ToString`/`Value` ile ac; her yerde acma.
- Kimlik iceren model [N06](N06-entities.md); deger iceren model N07.

## §9 Kanit

| Kanit | Deger |
|---|---|
| Dosya sayisi (l1-domain) | 8 (index + N06..N11 + N46) |
| Boyut araligi | 2600-8500 bayt |
| Baglanti kirigi | 0 |
| Format | ASCII tablo, emoji yok |
| Kaynak | Bilesim: hedef vault SSOT + kaynak vault detayi |
