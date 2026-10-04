---
title: "N09 - Domain Events (Naming, Occurrence, Dispatch)"
type: architecture
category: l1-domain
status: active
version: 1.0
updated: 2026-10-04
authority: SSOT
scope: VersaCoder
---

# N09 - Domain Events (Naming, Occurrence, Dispatch)

**Zorunlu Baglantilar:** [index.md](index.md) | [N06-entities.md](N06-entities.md) | [N08-aggregates.md](N08-aggregates.md) | [N10-invariants.md](N10-invariants.md) | [N46-macro-l1.md](N46-macro-l1.md) | [N03-events-abi.md](../index.md) | [ADR-028](../../.decisions/accepted/ADR-028-layered-baseline-9-projects.md) | [ADR-013](../../.decisions/accepted/ADR-013-layered-architecture-52-nodes.md) | [layer-boundary-check](../../checklists/layer-boundary-check.md) | [test-strategy](../../spec/test-strategy.md)

> **Tek cumlelik sorumluluk:** "Ne oldu"yu (gecmis zaman) olculebilir ve yayilabilir kayit haline getirir; ne yapilacagini degil, olan seyi duyurur.

## §0 Ozet (Answer First)

Domain event = **gecmis zamanda olmus, geri alinamaz bir olay**. Adi `AggregateOnaylandi` gibi eylem-gecmis zamandir; `OnaylaEvent` gibi emir kipi yasak. Event bir nesnedir, metot degil: `Yayimla` komutu, `Olustu` olayi. Tanim L1'de (`VersaCoder.Domain`), yayim (dispatch) [N09-abstraction -> N45](../index.md) portu uzerinden L4'te gerceklesir; L1 handler tanimaz. Aggregate'ler arasi tutarlilik tek kaynagi budur.

**Kunye:**

| Ozellik | Deger |
|---|---|
| Katman | l1-domain (tanim), dispatch: L4 icretilir |
| Proje | VersaCoder.Domain |
| N kodu | N09 (tanim), N45 (port), [ADR-013](../../.decisions/accepted/ADR-013-layered-architecture-52-nodes.md) |
| ABI | [N03-events-abi.md](../index.md) |
| Varsayilan yon | YOK (event L1'de tanimli, hic katmana bakmaz) |

## §1 Sorumluluk

Tek sorumluluk: **olayi kayit altina almak**. Event'nin gorevi duyurmak; islemi tamamlamak degil.

- Alan: `Id`, `Tarih` (L0 [N45](../index.md) tipi), `KaynakId` ve olaya ozel veri.
- Degistirilemezdir; olustuktan sonra veri degismez.
- Sadece gerceklesen gercege gore yazilir: `Olustu`, `Guncellendi`, `IptalEdildi`.
- Yayim asagidir: L1 event'i yayar mi? Hayir. Toplar, L4 tasir.

## §2 Girenler / Ciktilar

| Yon | Ne | Kaynak |
|---|---|---|
| GIRENLER | Aggregate durum degisimi | [N08](N08-aggregates.md) ic islemi |
| GIRENLER | Kural dogrulandi mi | [N10](N10-invariants.md) |
| GIRENLER | Olay verisi (deger tipleri) | [N07](N07-value-objects.md) |
| CIKANLAR | Degistirilemez olay nesnesi | L4 yayici (in-box dispatcher) |
| CIKANLAR | Event ABI sozlesmesi | [N03](../index.md) |
| CIKANLAR | Domain failure | Result/Either |

## §3 Yasak Yonler

| Yon | Deger | Neden |
|---|---|---|
| N09 -> l2-application | X | Handler'i (yordam) tanimaz |
| N09 -> l3-crosscutting | X | ADR-028: L3 yalniz L0'a bakar |
| N09 -> l4-infrastructure | X | Dispatcher ici L4; event tanimi L1'de kalir |
| N09 -> l5-presentation | X | UI modeli ile olay karistirilmaz |
| N09 -> l6-database | X | Kuyruk/sutun bilgisi tanimlanmaz |
| N09 -> l7-testing | X | Test olayi uretilmez |

Izinli: event -> L0 (deger + [N03](../index.md) ABI). Ters yon: L4 -> L1 sadece tip okumak icin (port bagimi [N45](../index.md)).

## §4 N Kodlari ve Proje

| N | Ad | Iliski | Proje |
|---|---|---|---|
| N09 | Domain Events | Olay tipi | VersaCoder.Domain |
| N03 | Events ABI | Olay imzasi | VersaCoder.Abstractions |
| N45 | Port | Yayim portu | VersaCoder.Abstractions |
| N08 | Aggregates | Olay uretir | VersaCoder.Domain |
| N19 | Logging | Olay kaydi (L3) | VersaCoder.Crosscutting |

## §5 Bagimlilik Listesi

| Bagimlilik | Yon | Not |
|---|---|---|
| L0 deger + ABI | asagi | Sadece L0 |
| N07/N06 | ic | Olay verisi tipleri |
| L2..L7 | YOK | Tum yonler asagidan yurur |
| Harici bus (MediatR vb.) | YOK | Cikti tasiyorsa bagimlilik artar |

## §6 Test Stratejisi

| Katman | Ne test edilir | Arac |
|---|---|---|
| Unit | Aggregate islemi dogru olayi uretir | xUnit |
| Unit | Event degistirilemez, adi gecmis zamanda | xUnit |
| Unit | Ayni olay iki kez yayilmaz (in-box) | xUnit |
| Integration | Olay L4 dispatcher'a ulasir (kayit) | EF + test double |
| E2E | Olay sonrasi ikinci adim calisir | Playwright |

## §7 Hata Durumlari

| Durum | Davranis |
|---|---|
| Handler hata verdi | Olay yine yayildi; islem geri alinmaz (ayrilmis olay) |
| Ayni olaydan iki tanesi | Kaynak Id + olay tipi ile tekillestirilir |
| Olay verisi eksik | Result failure; olay hic olusturulmaz |
| L4 dispatcher yok | Port bos; testte double ile dogrulanir |

## §8 Kurallar

- Adi gecmis zaman: `Olustu`, `Onaylandi`; emir kipi yasak.
- Event degistirilemez; alanlari `init`/`readonly`.
- L1'de olay **tanimla**, yayimi L0 portu uzerinden L4'e birak.
- Iki aggregate arasindaki tutarlilik icin tek kaynak olaydir; ayni transaction'a sigdirma.
- Olay verisini ham tiple degil [N07](N07-value-objects.md) ile tasi.

## §9 Kanit

| Kanit | Deger |
|---|---|
| Dosya sayisi (l1-domain) | 8 (index + N06..N11 + N46) |
| Boyut araligi | 2600-8500 bayt |
| Baglanti kirigi | 0 (kendi dosyalarim) |
| Format | ASCII tablo, emoji yok |
| Kaynak | Bilesim: hedef vault SSOT + kaynak vault detayi |
