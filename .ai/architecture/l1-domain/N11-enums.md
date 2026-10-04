---
title: "N11 - Enums (Boundaries of Named Values)"
type: architecture
category: l1-domain
status: active
version: 1.0
updated: 2026-10-04
authority: SSOT
scope: VersaCoder
---

# N11 - Enums (Boundaries of Named Values)

**Zorunlu Baglantilar:** [index.md](index.md) | [N06-entities.md](N06-entities.md) | [N07-value-objects.md](N07-value-objects.md) | [N10-invariants.md](N10-invariants.md) | [N46-macro-l1.md](N46-macro-l1.md) | [ADR-028](../../.decisions/accepted/ADR-028-layered-baseline-9-projects.md) | [ADR-013](../../.decisions/accepted/ADR-013-layered-architecture-52-nodes.md) | [layer-boundary-check](../../checklists/layer-boundary-check.md) | [test-strategy](../../spec/test-strategy.md)

> **Tek cumlelik sorumluluk:** "Secilebilir degerler" kumesini tek yerde adlandirir; sayi, string ve serbest metin yerine sinirli ve okunur secenegi zorunlu kilar.

## §0 Ozet (Answer First)

Enum, deger kumesinin **siniri**dir: `Durum { Taslak, Yayinda, Arsiv }`. Yeni deger eklemek = L1'de tek dosya degisikligi; "4 mean ne?" sorusunu bitirir. Enum [N07](N07-value-objects.md)-gibi degistirilemez ve [N10](N10-invariants.md)-un konusudur (hangi gecise izin var), ama kimliksizdir. Kural: enum'u **ham int gibi kullanma** (donusum, karsilastirma) — hep enum tipi uzerinden. Enum'a eklenemeyecek/kurala ihtiyac duyan degerler enum degil value object olur.

**Kunye:**

| Ozellik | Deger |
|---|---|
| Katman | l1-domain |
| Proje | VersaCoder.Domain |
| N kodu | N11, [ADR-013](../../.decisions/accepted/ADR-013-layered-architecture-52-nodes.md) |
| Kullanim | N06/N07 alanlari, L2 DTO, L6 saklama (int) |
| Varsayilan yon | YOK (L1 hic katmana bakmaz) |

## §1 Sorumluluk

Tek sorumluluk: **adlandirilmis, sinirli deger kumesi**. Enum su cevaplari verir:

| Soru | Cevap |
|---|---|
| Hangi degerler mumkun? | Enum taniminda birebir listelenir |
| Yeni deger nereden eklenir? | L1 tek dosya |
| Kesisim (gecis) kurali kimde? | [N10](N10-invariants.md) + sahibi aggregate |
| Saklama nasil? | Int (L6); ad metni her yerde ayni olmali |
| UI'da ne gosterilir? | L5 kendi metnini tasiyabilir; kural L1'de |

## §2 Girenler / Ciktilar

| Yon | Ne | Kaynak |
|---|---|---|
| GIRENLER | Is kuresi tanimi | Uzman, [N10](N10-invariants.md) |
| GIRENLER | Mevcut deger adlari | Kod gecmisi (uyumluluk) |
| GIRENLER | Saklama bicimi (int) | [ADR-028](../../.decisions/accepted/ADR-028-layered-baseline-9-projects.md) |
| CIKANLAR | Enum tipi (degistirilemez) | Domain |
| CIKANLAR | Gecis matrisi (ne => ne) | Testler |
| CIKANLAR | Okunur ad (ToString) | L5, log (L3) |

## §3 Yasak Yonler

| Yon | Deger | Neden |
|---|---|---|
| N11 -> l2-application | X | Yordam tipi tanimlamaz |
| N11 -> l3-crosscutting | X | ADR-028 matrisi |
| N11 -> l4-infrastructure | X | DB enum'u is kuresi degil |
| N11 -> l5-presentation | X | UI etiketi kural degil |
| N11 -> l6-database | X | Sutun listesi ile kures karistirilmaz |
| N11 -> l7-testing | X | Test enum'u uretilmez |

Izinli: L1 -> L0 (yok sayilacak kadar az; enum BCL `System.Enum` kullanir). Ters: L2/L5 enum'u **okur**, yenisini **eklemez** (ekleme L1'de).

## §4 N Kodlari ve Proje

| N | Ad | Iliski | Proje |
|---|---|---|---|
| N11 | Enums | Kures sahibi | VersaCoder.Domain |
| N06 | Entities | Alan tipi | VersaCoder.Domain |
| N07 | Value Objects | Sinirli deger benzeri | VersaCoder.Domain |
| N10 | Invariants | Gecis kurali | VersaCoder.Domain |
| N15 | DTO/Mappers | L2 karsiligi (int) | VersaCoder.Application |

## §5 Bagimlilik Listesi

| Bagimlilik | Yon | Not |
|---|---|---|
| System (BCL enum) | ic | Harici paket yok |
| N06/N07/N10 | ic | Tip ve kural iliskisi |
| L2..L7 | YOK | Okuyanlar ters bagimli |
| Harici paket | YOK | — |

## §6 Test Stratejisi

| Katman | Ne test edilir | Arac |
|---|---|---|
| Unit | Tum enum degerleri tek tek okunur | xUnit |
| Unit | Gecis matrisi (izin verilen yonler) | xUnit |
| Unit | Bilinmeyen int gecersizdir (donusum) | xUnit |
| Integration | Saklama int olarak kaydedilir | EF InMemory |
| E2E | Secim dogru akisa yonlendirir | Playwright |

## §7 Hata Durumlari

| Durum | Davranis |
|---|---|
| Bilinmeyen deger (eski veri) | Gecersiz/arsiv degeri olarak isimlendir |
| Eksik deger | L1'de eklenir; L2'de magic number yok |
| Iki ayni anlama gelen enum | Birlestir; kesisim testi kirmizi |
| Geersiz gecis | [N10](N10-invariants.md) Result failure |

## §8 Kurallar

- Enum'u hep enum tipiyle tasi; int/string donusumunu sinirli tut.
- Yeni deger = L1 tek dosya; baska yerde ekleme.
- Gecis kuralini (hangi durum => hangi durum) [N10](N10-invariants.md) icinde yaz.
- Enum'a veri/parametre gerekiyorsa enum degil [N07](N07-value-objects.md) yap.
- UI metni kural degil; kuralin adi L1'de tek kaynak.

## §9 Kanit

| Kanit | Deger |
|---|---|
| Dosya sayisi (l1-domain) | 8 (index + N06..N11 + N46) |
| Boyut araligi | 2600-8500 bayt |
| Baglanti kirigi | 0 (kendi dosyalarim) |
| Format | ASCII tablo, emoji yok |
| Kaynak | Bilesim: hedef vault SSOT + kaynak vault detayi |
