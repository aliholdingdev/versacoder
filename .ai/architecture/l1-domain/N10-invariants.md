---
title: "N10 - Invariants (Where Rules Live, When They Break)"
type: architecture
category: l1-domain
status: active
version: 1.0
updated: 2026-10-04
authority: SSOT
scope: VersaCoder
---

# N10 - Invariants (Where Rules Live, When They Break)

**Zorunlu Baglantilar:** [index.md](index.md) | [N06-entities.md](N06-entities.md) | [N07-value-objects.md](N07-value-objects.md) | [N08-aggregates.md](N08-aggregates.md) | [N11-enums.md](N11-enums.md) | [N46-macro-l1.md](N46-macro-l1.md) | [ADR-028](../../.decisions/accepted/ADR-028-layered-baseline-9-projects.md) | [ADR-013](../../.decisions/accepted/ADR-013-layered-architecture-52-nodes.md) | [layer-boundary-check](../../checklists/layer-boundary-check.md) | [test-strategy](../../spec/test-strategy.md)

> **Tek cumlelik sorumluluk:** Kacinilmaz is kuralini "nerede yasar" ve "ne zaman patlar" sorulariyla tek yere koyar; kural hep var olur, hicbir zaman atlanamaz.

## §0 Ozet (Answer First)

Invariant = durum **her zaman** dogru olmali kurali. Yeri: [N07](N07-value-objects.md)'de deger kurulusunda, [N06](N06-entities.md)/[N08](N08-aggregates.md)'de kok metninde, [N11](N11-enums.md)'de sinirli deger setinde. Zamani: **degisim aninda**, yazimdan once; sonra "dogrula" metodu yok. Basi bos invariant = kural yanlis yerde (L2 handler'a sigdirilmis = L1'de kayip). Sonuc: Result/Either ile domain failure; exception sadece programci hatasi icin. Kural L1'de yasar, dogrulama L2'de tekrarlanmaz (L2 sadece format kontrolu yapabilir, is kurali degil).

**Kunye:**

| Ozellik | Deger |
|---|---|
| Katman | l1-domain |
| Proje | VersaCoder.Domain |
| N kodu | N10, [ADR-013](../../.decisions/accepted/ADR-013-layered-architecture-52-nodes.md) |
| Kural sahibi | N06/N07/N08 (kuralin sahibi kok) |
| Varsayilan yon | YOK (kural alt katman tanimlamaz) |

## §1 Sorumluluk

Tek sorumluluk: **kuralin evi ve zamanlamasi**. Kim neyi cevaplar:

| Soru | Cevap |
|---|---|
| Kural nerede? | Kuralin sahibi aggregate/kok (N08/N06) |
| Ne zaman kontrol? | Degisim aninda, tek seferde |
| Patlarsa ne? | Result/Either domain failure |
| Kim tekrar dogrular? | Kimse; L2 format kontrolu yapar, is kurali degil |
| Kural nerede gecersiz? | Dis aggregate; orada olayla asenkron cozulur |

## §2 Girenler / Ciktilar

| Yon | Ne | Kaynak |
|---|---|---|
| GIRENLER | Degisim istegi (komut + veri) | L2 handler |
| GIRENLER | Mevcut durum | [N08](N08-aggregates.md) kok |
| GIRENLER | Kural tanimi (sayi, aralik, kosul) | Uzman kurali, [N11](N11-enums.md) |
| CIKANLAR | Onay veya domain failure | Result/Either |
| CIKANLAR | Guncellenmis, tutarli durum | L4 repository |
| CIKANLAR | Olay (kural saglandiginda) | [N09](N09-domain-events.md) |

## §3 Yasak Yonler

| Yon | Deger | Neden |
|---|---|---|
| Kural -> l2-application | X | Yordam'a sigdirilan kural kaybolur |
| Kural -> l3-crosscutting | X | ADR-028: L3 yalniz L0'a bakar |
| Kural -> l4-infrastructure | X | DB constraint'i is kurali degil |
| Kural -> l5-presentation | X | UI'da gizlenen kural hile |
| Kural -> l6-database | X | Saklama kurali tasimaz |
| Kural -> l7-testing | X | Test kurali uretilmez |

Izinli: L1 -> L0 (deger tipi ve sonuc sozlesmesi); N kodlari ve kesisimler [index.md](../index.md) uzerinden gorulur. Fiziksel kontrol: [layer-boundary-check](../../checklists/layer-boundary-check.md).

## §4 N Kodlari ve Proje

| N | Ad | Iliski | Proje |
|---|---|---|---|
| N10 | Invariants | N06/N07/N08 ic | VersaCoder.Domain |
| N11 | Enums | Sinirli kural | VersaCoder.Domain |
| N17 | Use Cases | Tekrar etmez (sadece cagirir) | VersaCoder.Application |
| N16 | Validators | Format; is kurali degil | VersaCoder.Application |
| N02 | Result/Error | Hata tasima | VersaCoder.Abstractions |

## §5 Bagimlilik Listesi

| Bagimlilik | Yon | Not |
|---|---|---|
| N06/N07/N08/N11 | ic | Kural sahipleri |
| Sonuc tipi (L0) | asagi | Sadece L0 |
| L2 validator | TERS | Format kontrolu L2'de |
| L4, L6 | YOK | Kural saklama aracina bagimli degil |
| Harici kural kutuphanesi | YOK | Cikti tasiyorsa degerlendirilir |

## §6 Test Stratejisi

| Katman | Ne test edilir | Arac |
|---|---|---|
| Unit | Kural saglandiginda islem gecti | xUnit |
| Unit | Kural ihlalinde Result failure dondu | xUnit |
| Unit | Kural tekrar testte yazilmamis (ortak fabrika) | xUnit |
| Integration | Kayit sonrasi kural bozulmadi (EF) | EF InMemory |
| E2E | Gecersiz giriste kullanici hatasi goruldu | Playwright |

## §7 Hata Durumlari

| Durum | Davranis |
|---|---|
| Kural ihlali | Result/Either; rollback yok, yazim zaten yok |
| Kural atlandi (kod hatasi) | Domain failure; istisna yayilmaz |
| Dis aggregate kurali | Olay ile asenkron cozulur |
| Belirsiz kural | [layer-boundary-check](../../checklists/layer-boundary-check.md) sorusu ile karara bagla |

## §8 Kurallar

- Kurali kuralin sahibi kokte yaz; handler'a sigdirma.
- Degisim aninda tek seferde dogrula; sonrasi icin "dogrula" metodu birakma.
- Basarisizlikta Result/Either don; istisna programci hatasi icin ayri.
- Kurali testte tekrar yazma; var olan testi kullan.
- Simetrik/deger kurali [N07](N07-value-objects.md), sinirli kural [N11](N11-enums.md) icinde bitir.

## §9 Kanit

| Kanit | Deger |
|---|---|
| Dosya sayisi (l1-domain) | 8 (index + N06..N11 + N46) |
| Boyut araligi | 2600-8500 bayt |
| Baglanti kirigi | 0 (kendi dosyalarim) |
| Format | ASCII tablo, emoji yok |
| Kaynak | Bilesim: hedef vault SSOT + kaynak vault detayi |
