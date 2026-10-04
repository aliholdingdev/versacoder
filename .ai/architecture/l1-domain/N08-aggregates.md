---
title: "N08 - Aggregates (Boundaries, Roots, Transactional Consistency)"
type: architecture
category: l1-domain
status: active
version: 1.0
updated: 2026-10-04
authority: SSOT
scope: VersaCoder
---

# N08 - Aggregates (Boundaries, Roots, Transactional Consistency)

**Zorunlu Baglantilar:** [index.md](index.md) | [N06-entities.md](N06-entities.md) | [N07-value-objects.md](N07-value-objects.md) | [N09-domain-events.md](N09-domain-events.md) | [N10-invariants.md](N10-invariants.md) | [N46-macro-l1.md](N46-macro-l1.md) | [ADR-028](../../.decisions/accepted/ADR-028-layered-baseline-9-projects.md) | [ADR-013](../../.decisions/accepted/ADR-013-layered-architecture-52-nodes.md) | [layer-boundary-check](../../checklists/layer-boundary-check.md) | [test-strategy](../../spec/test-strategy.md)

> **Tek cumlelik sorumluluk:** Bir is kuralini tek kaynaktan yoneten kok nesne + ic kume sinirini tanimlar; ayni transaction icinde tutarli kalmasi gereken seyleri bir arada tutar.

## §0 Ozet (Answer First)

Aggregate = kok ([N06](N06-entities.md) veya kucuk bir deger tipi) + icindekiler + sinir. Disaridan sadece kok uzerinden islem yapilir; ic nesneye dogrudan ulasilamaz. Kural: **kucuk aggregate** (2-3 seviye), **tek transaction = tek aggregate**. Aggregate'ler arasi tutarlilik [N09](N09-domain-events.md) ile event uzerinden saglanir, ayni transaction icinde degil. Tum invariant'lar kokte toplanir ([N10](N10-invariants.md)).

**Kunye:**

| Ozellik | Deger |
|---|---|
| Katman | l1-domain |
| Proje | VersaCoder.Domain (ADR-028) |
| N kodu | N08, [ADR-013](../../.decisions/accepted/ADR-013-layered-architecture-52-nodes.md) |
| Kullanim | L2 handler (Yordam), L4 repository |
| Varsayilan yon | YOK (L1 hic katmana bagimli degil) |

## §1 Sorumluluk

Tek sorumluluk: **sinir + kok sorumlulugu**. Aggregate su sorulari cevaplar: ne birlikte yazilir, nerede transaction biter, hangi nesneye disaridan dokunulmaz.

- Kok her zaman disaridan erisilebilir tek kapidir.
- Ic koleksiyon disaridan `IEnumerable` olarak gorunur, ekleme/celeme kokte metinle olur.
- Aggregate icindeki referanslar zayif olabilir; disari reference `[N06](N06-entities.md)` Id olarak tasinir.
- Bir islemi tek seferde bittigi say; ikinci transaction'a yayma.

## §2 Girenler / Ciktilar

| Yon | Ne | Kaynak |
|---|---|---|
| GIRENLER | Komut + Id (kok bulma) | L2 command |
| GIRENLER | Kural (invariant) | [N10](N10-invariants.md) |
| GIRENLER | Ham kayit (koleksiyon/alt nesne) | L6/L4 repository |
| CIKANLAR | Guncellenmis kok (tutarli durum) | L4 repository |
| CIKANLAR | Yayilan domain event | [N09](N09-domain-events.md) |
| CIKANLAR | Domain failure (kural ihlali) | Result/Either |

## §3 Yasak Yonler

| Yon | Deger | Neden |
|---|---|---|
| N08 -> l2-application | X | Yordam'i (handler) tanimaz |
| N08 -> l3-crosscutting | X | ADR-028 matrisi: L3 yalniz L0'a bakar |
| N08 -> l4-infrastructure | X | Repository disariya port ile girer |
| N08 -> l5-presentation | X | UI modeli aggregate degil |
| N08 -> l6-database | X | Id tipi Id'yi cozmek zorunda degil |
| N08 -> l7-testing | X | Test tipi uretilmez |

Izinli: aggregate icinde [N07](N07-value-objects.md) + [N06](N06-entities.md) kullanmak; [N09](N09-domain-events.md) ile olay yaymak. Dis aggregate'e dogrudan referans YOK (Id ile).

## §4 N Kodlari ve Proje

| N | Ad | Iliski | Proje |
|---|---|---|---|
| N08 | Aggregates | Kok: N06, deger: N07 | VersaCoder.Domain |
| N06 | Entities | Kok olur | VersaCoder.Domain |
| N10 | Invariants | Kokta calisir | VersaCoder.Domain |
| N09 | Domain Events | Cikis olayi | VersaCoder.Domain |
| N17 | Use Cases | Cagirir (L2) | VersaCoder.Application |

## §5 Bagimlilik Listesi

| Bagimlilik | Yon | Not |
|---|---|---|
| N06/N07/N10 | ic | Kume ici tipler |
| N09 | ic | Olay yayimi |
| L4 repository | TERS (port) | [N45](../index.md) |
| L2 handler | TERS (cagirir) | Ust katman |
| L3, L5, L6 | YOK | Sifir |

## §6 Test Stratejisi

| Katman | Ne test edilir | Arac |
|---|---|---|
| Unit | Kok icine gecersiz islem => Result failure | xUnit |
| Unit | Koleksiyon disaridan degistirilemez | xUnit |
| Unit | Event yayildi mi (N09) | xUnit |
| Integration | Tek transaction = tek aggregate (EF) | EF InMemory |
| E2E | Is kurali bozulmadan akis gecti mi | Playwright |

## §7 Hata Durumlari

| Durum | Davranis |
|---|---|
| Kok bulunamadi | Result failure (not found) |
| Invariant ihlali | Result failure, yazim yok |
| Dis aggregate eksik | Event ile asenkron tamamla |
| Cok buyuk aggregate | [layer-boundary-check](../../checklists/layer-boundary-check.md) sorusu ile bol |
| Ayni transaction'da iki kok | Kural: yapma; event'e tas |

## §8 Kurallar

- Disaridan sadece kok uzerinden islem yap.
- Tek islem = tek aggregate = tek transaction.
- Aggregate'ler arasi baglanti Id ile, obje referansi ile degil.
- Koleksiyonu disariya ac; ekleme/celeme kok metninde yap.
- Buyuyen kume icin [layer-boundary-check](../../checklists/layer-boundary-check.md) ile sinir sinayla.

## §9 Kanit

| Kanit | Deger |
|---|---|
| Dosya sayisi (l1-domain) | 8 (index + N06..N11 + N46) |
| Boyut araligi | 2600-8500 bayt |
| Baglanti kirigi | 0 |
| Format | ASCII tablo, emoji yok |
| Kaynak | Bilesim: hedef vault SSOT + kaynak vault detayi |
