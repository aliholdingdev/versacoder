---
title: "Katman Spec - l3 Crosscutting (VersaCoder)"
type: architecture
category: l3-crosscutting
status: active
version: 1.0
updated: 2026-10-04
authority: SSOT
scope: VersaCoder
---

# Katman Spec - l3 Crosscutting

**Zorunlu Baglantilar:** [index.md](../index.md) | [dependency-map](../00-overview/dependency-map.md) | [N19-logging.md](N19-logging.md) | [N20-caching.md](N20-caching.md) | [N21-clock-time.md](N21-clock-time.md) | [N22-crypto-sec.md](N22-crypto-sec.md) | [N23-config-opts.md](N23-config-opts.md) | [ADR-028](../../.decisions/accepted/ADR-028-layered-baseline-9-projects.md) | [ADR-013](../../.decisions/accepted/ADR-013-layered-architecture-52-nodes.md) | [layer-boundary-check](../../checklists/layer-boundary-check.md) | [keys.md](../../keys.md) | [test-strategy](../../spec/test-strategy.md)

> **Tek cumlelik sorumluluk:** Her katmanin ihtiyac duydugu ama hicbir katmanin tek basina kurmak istemedigi alti yetenek (log, cache, zaman, sifre, config, retry) nerede yasar sorusunun tek cevabidir; **kimsenin isini yapmaz**, yalniz ortak altyapiyi tasir.

## §0 Ozet (Answer First)

l3 = **ortak servis katmani**, insa sirasi 2 (l0'dan hemen sonra). **Izinli referans yalniz l0** (`Result<T>`, sozlesme tipleri); `l3 -> l1..l2, l4..l7` hepsi **X** (matris: `l3 yalniz l0'a bakar`). Dogrusal zincirde `l0 -> l3 -> l1` sirasi oldugu icin **l1'e asla bakamaz** — tek bir geri kenar `l1 -> l3 -> l1` dongusu dogurur ve insa sirasini curutur. Tum katmanlar l3'u **tuketir**.

**Kunye:**

| Ozellik | Deger |
|---|---|
| Katman | l3 Crosscutting (insa sirasi 2) |
| Proje | VersaCoder.Crosscutting (ADR-028) |
| N kodlari | N19..N24 (resilience katalogda) + index |
| Izinli referans | **yalniz l0** |
| Yasak referans | l1, l2, l4, l5, l6, l7 — hepsi |

## §1 Sorumluluk

| # | Yetenek | Temel arayuz | Karar |
|---|---|---|---|
| A1 | Loglama (seviye, Fields, coklu sink) | `ILogSink` | kendi ince arayuzu, paket yok |
| A2 | Cache (TTL'li, sinirli, thread-safe) | `ICache` | `ConcurrentDictionary` + TTL |
| A3 | Zaman (`UtcNow` + testte sabit saat) | `IClock` | `SystemClock`/`FixedClock` |
| A4 | Sifreleme (baglanti dizesi) | `IStringProtector` | AES-256-GCM (BCL) |
| A5 | Yapilandirma (varsayilan -> dosya -> ortam) | `IAppConfig` | katmanli, son okuyan kazanir |
| A6 | Retry (transient icin **tek** deneme) | `IRetryPolicy` | en fazla 1 tekrar |

| # | NE YAPMAZ | Neden |
|---|---|---|
| B1 | **l1'e** (entity/invariant) bakmaz | bu katman l1'den **once** derlenir |
| B2 | l2/l4/l5/l6/l7 tiplerine bakmaz | nötr kalmazsa dongu dogar |
| B3 | SQL, dosya yolu, protokol, form **bilmez** | is kurali l1, erisim l4 |
| B4 | Sir/anahtar/metin **barindirmez** | anahtar yonetimi ayri ADR |
| B5 | Kendi logunu **loglamaz** | sonsuz dongu |

## §2 Girenler / Ciktilar

| Yon | Ne | Kaynak |
|---|---|---|
| GIRENLER | Log kaydi (`LogEntry`) | l1, l2, l4, l5, l6, l7 |
| GIRENLER | Anahtar/deger + TTL | tuketici katman |
| GIRENLER | Ayardan okunan katman sirasi | dosya + ortam |
| CIKANLAR | `Result<T>` hata dili | l0 (tek bagimlilik) |
| CIKANLAR | Uretimde dosya logu / testte Null | sink secimi |
| CIKANLAR | Kayit sozlugu (yasam suresi) | l6 uygular |

## §3 Yasak Yonler

| Yon | Deger | Neden |
|---|---|---|
| l3 -> l1-domain | X | dongu: l1 -> l3 -> l1 |
| l3 -> l2-application | X | matris `l3 yalniz l0` |
| l3 -> l4..l7 | X | nötr kalmak sarti |
| l0 -> l3 | X | l0 en alttadir, yukari ok yasak |
| l3 -> NuGet | X | yalniz BCL (G8) |

Izinli: l3 -> l0 (`Result`, `Error`). Tuketim (ters ok) `l1, l2, l4..l7 -> l3` serbest. Not: kaynak vault metni `l2 -> l3 ✓` yazar; hedef matris X — **DOGRULAMA GEREKLİ** (bkz [index.md](../index.md) §4.1).

## §4 N Kodlari ve Proje

| N | Ad | Yetenek |
|---|---|---|
| N19 | logging | `ILogSink`, sink cesitleri |
| N20 | caching | `ICache`, TTL/boyut |
| N21 | clock-time | `IClock`, `FixedClock` |
| N22 | crypto-sec | `IStringProtector` |
| N23 | config-opts | `IAppConfig`, katmanli okuma |
| N24 | resilience | `IRetryPolicy` (kayit dosyasi: [catalog](../catalog/index.md)) |

## §5 Bagimlilik Listesi

| Bagimlilik | Yon | Not |
|---|---|---|
| l0: `Result`, `Error` | asagi | tek izinli |
| l1..l2, l4..l7 | YOK | hepsi CRITICAL |
| NuGet | YOK | yalniz BCL (G8) |
| Tuketici katmanlar | TERS | hepsi l3'u kullanir |
| Anahtar kaynagi | ADR | `NEEDS_VALIDATION` (N22) |

## §6 Test Stratejisi

| Katman | Ne test edilir | Hedef |
|---|---|---|
| Unit | `IClock`/`FixedClock` deterministik zaman | 0 dalgalanma |
| Unit | `ICache` TTL bitisi, ust sinir, eszamanlilik | 3/3 |
| Unit | `IRetryPolicy` 1 tekrar + non-transient ayrimi | 5/5 kural |
| Unit | `LayeredConfig` katman onceligi | 3/3 |
| Arch | l3 -> l1/l2/l4/l5/l6/l7 bagimliligi | **0** |

## §7 Boundary Check Sorulari

1. Bu dosyada `using VersaCoder.Domain|Application|...` var mi? (CRITICAL)
2. Dogrudan `DateTime.Now`/`UtcNow` cagrisi var mi? (yalniz `SystemClock`)
3. NuGet paketi var mi? (G8, hedef 0)
4. Retry'da 2. deneme var mi? (yasak)
5. Log'a sifre/anahtar yazildi mi? (CRITICAL)

Kanit: [layer-boundary-check](../../checklists/layer-boundary-check.md) 13 soru.

## §8 Kurallar (5 madde)

- Yeni ortak yetenek once buraya sorulur: "Bu l3'te var mi?" Varsa yeni servis **kurulmaz**.
- Log yapilandirilmistir (`Message` + `Fields` + `kategori.olay`); serbest metin birlestirme yok.
- `IStringProtector` anahtar **barindirmaz**; diskte duz metin yasak.
- Retry'da **2. deneme yasak**; surekli basarisizlik `Result.Fail` ile yukari tasinir.
- `#if` yalniz platform farki icin (log dosyasi yolu); hedef 0.

## §9 Kanit

| Kanit | Deger |
|---|---|
| Dosya sayisi (l3-crosscutting) | 6 (index + N19..N23) |
| Boyut araligi | 2600-8500 bayt |
| Baglanti kirigi | 0 |
| Format | ASCII tablo, emoji yok |
| Kaynak | Bilesim: hedef vault SSOT + kaynak vault detayi |
