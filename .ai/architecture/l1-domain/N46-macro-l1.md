---
title: "N46 - Macro View: L1 Domain (8 Nodes)"
type: architecture
category: l1-domain
status: active
version: 1.0
updated: 2026-10-04
authority: SSOT
scope: VersaCoder
---

# N46 - Macro View: L1 Domain (8 Nodes)

**Zorunlu Baglantilar:** [index.md](index.md) | [N06-entities.md](N06-entities.md) | [N07-value-objects.md](N07-value-objects.md) | [N08-aggregates.md](N08-aggregates.md) | [N09-domain-events.md](N09-domain-events.md) | [N10-invariants.md](N10-invariants.md) | [N11-enums.md](N11-enums.md) | [ADR-028](../../.decisions/accepted/ADR-028-layered-baseline-9-projects.md) | [ADR-013](../../.decisions/accepted/ADR-013-layered-architecture-52-nodes.md) | [layer-boundary-check](../../checklists/layer-boundary-check.md) | [test-strategy](../../spec/test-strategy.md)

> **Tek cumlelik sorumluluk:** L1'in tamamini tek ekranda ozetler: 8 node, tek yon asagi, "is kurali nerede?" sorusunun tek cevabi.

## §0 Ozet (Answer First)

L1 = **is kurali katmani**. 8 node tek projede yasar: `VersaCoder.Domain`. Kural: L1 hic katmana bakmaz (L0'a dahi minumum); sadece L0'in deger/sonuc/olay sozlesmelerini kullanir. Tum invariant'lar burada biter; L2 komutu L1'e girer, kural burada patlar, durum burada degisir. L1'de framework, DB, log, saat, HTTP **yok** — bunlar L3/L4'tedir ve L1'e girmez.

**Kunye:**

| Ozellik | Deger |
|---|---|
| Katman | l1-domain |
| Proje | VersaCoder.Domain (ADR-028, 9 projeden biri) |
| Node | N06, N07, N08, N09, N10, N11 + bu makro (N46) + index |
| N kodlari | [ADR-013](../../.decisions/accepted/ADR-013-layered-architecture-52-nodes.md) |
| Varsayilan yon | YOK (asagidan yukari bakis: L0 -> L1 icin portlar) |

## §1 Sorumluluk

L1'in tek sorumlulugu: **kural + durum**. Diger katmanlar ne yapiyor:

| Katman | Ne yapar | L1'e ne sokar |
|---|---|---|
| L2 application | Yordam (handler) | Komut/DTO veri |
| L3 crosscutting | Altyapı | YOK (girmez) |
| L4 infrastructure | Repository, dispatcher | Port uzerinden kayit |
| L5..L7 | UI, dosya, host | YOK (dogrudan giris yok) |

## §2 Girenler / Ciktilar

| Yon | Ne | Kaynak |
|---|---|---|
| GIRENLER | Komut (ham veri) | L2 [index](../index.md) |
| GIRENLER | Port (repository, olay) | L0/N45 port sozlesmesi |
| GIRENLER | Kural (uzman tanimi) | Kures ici |
| CIKANLAR | Tutarli aggregate durumu | L4 repository |
| CIKANLAR | Domain event | L4 dispatcher |
| CIKANLAR | Result/Either sonuc | L2'ye donus |

## §3 Yasak Yonler

| Yon | Deger | Neden |
|---|---|---|
| L1 -> l2-application | X | Yukari bakma (ADR-028) |
| L1 -> l3-crosscutting | X | **MATRIS X**: L3 yalniz L0'a bakar |
| L1 -> l4-infrastructure | X | Klasik dibl; port bagimi ters |
| L1 -> l5-presentation | X | UI bilmez |
| L1 -> l6-database | X | Sutun bilgisi yok |
| L1 -> l7-testing | X | Test tipi yok |

Not: kaynak vault'ta `IClock` icin L1 -> L3 izni geciyordu; hedef vault [index.md](../index.md) matrisinde `X`. **DOGRULAMA GEREKLI** (bkz [index.md](index.md) §3/2 ve N46 notu).

## §4 N Kodlari ve Proje

| N | Ad | Sorumluluk |
|---|---|---|
| N06 | Entities | Kimlikli nesne |
| N07 | Value Objects | Degistirilemez deger |
| N08 | Aggregates | Kok + sinir |
| N09 | Domain Events | Gecmis zaman olayi |
| N10 | Invariants | Kural evi |
| N11 | Enums | Sinirli kures |
| N46 | Macro L1 | Bu dosya |

## §5 Bagimlilik Listesi

| Bagimlilik | Yon | Not |
|---|---|---|
| L0 (deger, sonuc, olay ABI) | asagi | Tek izinli asagi yon |
| Harici NuGet | YOK | Domain saf kalir |
| L2..L7 | YOK | Sifir yukari bagimlilik |
| Test | L7 | Saf tip, kolay test |

## §6 Test Stratejisi

| Katman | Ne test edilir | Arac |
|---|---|---|
| Unit | Tum invariant'lar (N10) | xUnit |
| Unit | Aggregate olay uretimi | xUnit |
| Unit | Enum gecis matrisi | xUnit |
| Integration | Repository portu uzerinden yazim (L4 double) | xUnit |
| E2E | Is kurali bozulmadan akis | Playwright |

## §7 Kurallar (5 madde)

- L1 hic katmana bakmaz; sadece L0 sozlesmelerini kullanir.
- Kurali handler'a degil kok/invariant'a yaz.
- Harici paket, framework, DB, log, saat L1'e giremez.
- Olayi tanimla, yayimi L4'e birak (port).
- Uc proje gecisini [layer-boundary-check](../../checklists/layer-boundary-check.md) ile kapat.

## §8 Sinir Kontrol Sorulari

1. Bu tip baska katmanin tipini biliyor mu?
2. Kural L2'ye sigdirildi mi? (yapma)
3. L1'de NuGet referansi var mi? (yapma)
4. Repository/db context L1'e girdi mi? (yapma)
5. Olay dispatch L1'de mi? (yapma; port L4)

Cevaplama ve kanit: [layer-boundary-check](../../checklists/layer-boundary-check.md), [test-strategy](../../spec/test-strategy.md).

## §9 Kanit

| Kanit | Deger |
|---|---|
| Dosya sayisi (l1-domain) | 8 (index + N06..N11 + N46) |
| Boyut araligi | 2600-8500 bayt |
| Baglanti kirigi | 0 (kendi dosyalarim) |
| Format | ASCII tablo, emoji yok |
| Kaynak | Bilesim: hedef vault SSOT + kaynak vault detayi |
