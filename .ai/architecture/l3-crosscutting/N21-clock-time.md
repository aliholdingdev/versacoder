---
title: "N21 - Clock / Time (IClock, FixedClock, DateTime.Now Yasagi)"
type: architecture
category: l3-crosscutting
status: active
version: 1.0
updated: 2026-10-04
authority: SSOT
scope: VersaCoder
---

# N21 - Clock / Time (IClock, FixedClock, DateTime.Now Yasagi)

**Zorunlu Baglantilar:** [index.md](index.md) | [N19-logging.md](N19-logging.md) | [N20-caching.md](N20-caching.md) | [N23-config-opts.md](N23-config-opts.md) | [ADR-028](../../.decisions/accepted/ADR-028-layered-baseline-9-projects.md) | [ADR-013](../../.decisions/accepted/ADR-013-layered-architecture-52-nodes.md) | [layer-boundary-check](../../checklists/layer-boundary-check.md) | [test-strategy](../../spec/test-strategy.md)

> **Tek cumlelik sorumluluk:** Zamanin nereden geldigini, testte nasil sabitlendigini ve `DateTime.Now`'in neden yasak oldugunu belirler.

## §0 Ozet (Answer First)

Tum katmanlar zamanı **`IClock.UtcNow`** uzerinden alir; `DateTime.Now`/`DateTime.UtcNow` **dogrudan cagrisi = 0** (yalniz `SystemClock` icinde). Uretimde `SystemClock`, testte **`FixedClock`** (ayarlanabilir + `Advance`) — boylece iki TFM de ayni zaman gorur, `CreatedUtc` karsilastirmalari dalgalanmaz. Zaman **her zaman UTC**; `DateTimeKind.Unspecified` kabul edilmez. Kayit zamani **uretim aninda** alinir, gorme aninda degil.

**Kunye:**

| Ozellik | Deger |
|---|---|
| Katman | l3-crosscutting |
| Proje | VersaCoder.Crosscutting |
| N kodu | N21, [ADR-013](../../.decisions/accepted/ADR-013-layered-architecture-52-nodes.md) |
| Arayuz | `IClock { DateTimeOffset UtcNow }` |
| Varsayilan yon | yalniz l0 |

## §1 Sorumluluk

Tek sorumluluk: **tek zaman kaynagi + deterministik test**.

| Tip | Imza | Kullanim | Not |
|---|---|---|---|
| `IClock` | `DateTimeOffset UtcNow { get; }` | **tumu** (l1, l2, l4, l5) | tek zaman kaynagi |
| `SystemClock` | `DateTimeOffset.UtcNow` | uretim (l6 kaydi) | tek `DateTime.UtcNow` cagrisi |
| `FixedClock` | ayarlanabilir + `Advance(...)` | test | deterministik |
| `DateTimeOffset` | `UtcDateTime`, `ToUnixTimeSeconds()` | kalicilik/protokol | `DateTime` degil |

```csharp
public interface IClock { DateTimeOffset UtcNow { get; } }
public sealed class FixedClock : IClock
{
    private DateTimeOffset _now;
    public FixedClock(DateTimeOffset start) { _now = start; }
    public DateTimeOffset UtcNow => _now;
    public void Advance(TimeSpan by) { _now = _now.Add(by); }
}
```

## §2 Girenler / Ciktilar

| Yon | Ne | Kaynak |
|---|---|---|
| GIRENLER | Zaman talebi | l1, l2, l4, l5 kodu |
| GIRENLER | Test baslangic degeri | test (`FixedClock`) |
| GIRENLER | Kayit (DI) | l6 composition |
| CIKANLAR | `DateTimeOffset` (UTC) | iki bacak ayni deger |
| CIKANLAR | Ileri sarilmis zaman (test) | `Advance` |
| CIKANLAR | `null` istisnasi (kayit yoksa) | DI hatasi |

## §3 Yasak Yonler

| Yon | Deger | Neden |
|---|---|---|
| l3 -> l1/l2/l4..l7 | X | nötr kalmak (matris `l3 yalniz l0`) |
| Kodda `DateTime.Now` | X | **CRITICAL**: test edilemez + iki bacak farki |
| `DateTimeKind.Local`/`Unspecified` | X | saat dilimi karisir |
| `Task.Delay` ile sure olcme | X | flaky test |
| NuGet saat paketi | X | G8 |

Izinli: l3 -> l0 (`Result`); tuketim ters ok ile (l1/l2/l4/l5 -> l3).

## §4 N Kodlari ve Proje

| N | Ad | Iliski | Proje |
|---|---|---|---|
| N21 | Clock-Time | `IClock` ailesi | VersaCoder.Crosscutting |
| N19 | Logging | `Utc` uretir | VersaCoder.Crosscutting |
| N39 | DI registration | `SystemClock` singleton kaydi | VersaCoder.Host |
| N10 | Invariants | `CreatedUtc` karsilastirmasi (l1) | VersaCoder.Domain |
| N05 | Constants | Saat ile ilgili sabitler (l0) | VersaCoder.Abstractions |

## §5 Bagimlilik Listesi

| Bagimlilik | Yon | Not |
|---|---|---|
| l0: `Result` | asagi | tek izinli |
| l1..l7 | YOK | nötr |
| BCL (`System`, `DateTimeOffset`) | ic | yeterli |
| NuGet | YOK | G8 |

## §6 Test Stratejisi

| Katman | Ne test edilir | Hedef |
|---|---|---|
| Unit | `FixedClock` verilen degeri birebir dondurur | once RED |
| Unit | `Advance` sonrasi deger 1dk artar | sure testleri |
| Unit | I6 (`CompletedUtc > CreatedUtc`) sabit saatle tutarli | invariant guvenligi |
| Arch | `DateTime.Now`/`UtcNow` **dogrudan** cagrisi = 0 | kod arama |
| Arch | `IClock` l6 kayit sozlugunde | DI kaydi denetimi |

## §7 Hata Durumlari

| Durum | Sonuc | Koruma |
|---|---|---|
| Kodda `DateTime.Now` | **CRITICAL** (test edilemez) | Arch testi = 0 |
| `DateTimeKind.Local` kayit | saat dilimi karisi | yalniz `DateTimeOffset` UTC |
| `FixedClock` ileri sarilmadi | sure testi yanlis gecer | test duzeni (`Advance`) |
| `UtcNow` asiri gelecekte/gecmiste | I6 ihlali riski | sabit saat + sinir testi |
| `IClock` kayitli degil | `null` istisna | l6 kayit sozlugu |

## §8 Kurallar (5 madde)

- Zamani yalniz `IClock`'tan al; `DateTime.Now`/`UtcNow` dogrudan cagirma.
- Kayit zamani uretim aninda alinir (fabrika parametresi), gorme aninda degil.
- Testte `FixedClock` + `Advance`; `Task.Delay` ile sure olcme.
- Kalicilikta `DateTimeOffset` (UTC) kullan; `DateTime` degil.
- `SystemClock` disindaki her dogrudan cagri Arch testinde kirmizidir.

## §9 Kanit

| Kanit | Deger |
|---|---|
| Dosya sayisi (l3-crosscutting) | 6 (index + N19..N23) |
| Boyut araligi | 2600-8500 bayt |
| Baglanti kirigi | 0 |
| Format | ASCII tablo, emoji yok |
| Kaynak | Bilesim: hedef vault SSOT + kaynak vault detayi |
