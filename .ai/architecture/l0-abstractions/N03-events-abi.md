---
title: "Dugum Detayı N03 — Olay ABI (l0)"
type: architecture
category: architecture
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "IDomainEvent ve IAgentEvent sozlesmeleri, yayim yonu, Sequence sirasi ve ABI uyumsuzluk durumlari"
---

# Dugum Detayı N03 — Olay ABI (l0)

**Zorunlu Baglantilar:** [Mimari index](../index.md) · [katman spec](index.md) · [dependency-map](../00-overview/dependency-map.md) · [ADR-028](../../.decisions/accepted/ADR-028-layered-baseline-9-projects.md) · [test-strategy](../../spec/test-strategy.md) · [N02-result-error](N02-result-error.md)

> **Tek cumlelik sorumluluk:** *Bir olay hangi alanlari tasir, kim uretir, kim tuketir ve ABI bozulursa ne olur* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

l0 iki olay sozlesmesi yayimlar: **`IDomainEvent`** (is olayi: ureten l1, tuketen l2) ve **`IAgentEvent`** (akis olayi: ureten l5, tuketen l2/l7). Her olay `EventId` + zaman + tip adi tasir; `IAgentEvent` uzerine **monotonik `Sequence`** eklenir ki transkript sirasi kaybolmasin. Olay **gecmistir, emir degildir**: adlandirma gecmis zamandir (`TaskStarted`).

| Alan | Deger |
|---|---|
| N kodu / alt katman | N03 · events-abi |
| Makro katman / proje | l0 Abstractions · `VersaCoder.Abstractions` |
| Ureten | l1 (entity/durum gecisi), l5 (agent dongusu) |
| Tuketen | l2 (commit sonrasi yayim), l7 (gorunum) |
| Zaman kaynagi | l3 `IClock` (`DateTime.Now` yasak) |

---

## §1 Sorumluluk (NE YAPAR)

| # | Tip | Alanlar | Ureten | Tuketen |
|---|---|---|---|---|
| 1 | `IDomainEvent` | `EventId`, `OccurredAtUtc`, `EventType` | l1 | l2 |
| 2 | `IAgentEvent` | `EventId`, `Sequence`, `Kind` | l5 | l2, l7 |
| 3 | `Kind` sabitleri | `delta`, `tool_call`, `tool_result`, `done`, `error` | l5 | l7 |
| 4 | Somut olaylar (`TaskStarted` vb.) | l1 alt siniflari | durum makinesi gecisi | l2, l5, l7 |
| 5 | ABI kaydi | ad + bicim | bu dugum | uyumsuzlukta log (l3) |

---

## §2 Bu Katmana GIRENLER / CIKANLAR

| Yon | Kim / ne | Tur | Kosul |
|---|---|---|---|
| GIREN | l1 uretimi (entity gecisi) | girdi | `OccurredAtUtc` l3 `IClock`'tan |
| GIREN | l5 uretimi (`Sequence` 1..N) | girdi | bosluk/tekrar kabul edilmez |
| CIKAN | olay ABI'si (imza) | cikti | l2 kayit, l7 ekran |
| CIKAN | uyumsuzluk kaydi | cikti | `ErrorCode.Protocol` + log |
| YASAK | l0 -> l1, l5 somut olay tipi | yasak | somut olay l1/l5'te tanimlanir |

---

## §3 Yasak Yonler (ADR-028 matrisi)

| # | Yasak yon | Kaynak | Bu dugumdeki karsiligi |
|---|---|---|---|
| 1 | `l0 -> l1` | ADR-028 §2.1 | `TaskStarted` l0 dosyasina yazilamaz |
| 2 | `l2 -> l4` | ADR-028 §2.1 | olay yayimi veritabani bilmeden calisir |
| 3 | `l5 -> l4` | ADR-028 §2.3/2 | agent dongusu altyapiya bakmaz |
| 4 | `l7 -> l5` | ADR-028 §2.1 | ekran olayi l6 uzerinden alir |
| 5 | `l3 -> l1, l2, l5` | ADR-028 §2.1 | log yalniz `Error`/olay kaydini okur |

---

## §4 N Kodu ve Proje

| Alan | Deger |
|---|---|
| Katalog kaydi | `../catalog/index.md` altinda `N03` satiri |
| Kod yolu | `src/VersaCoder.Abstractions/Events/` (`IDomainEvent.cs`, `IAgentEvent.cs`) |
| Proje | `VersaCoder.Abstractions` (tek .csproj, cift TFM) |

---

## §5 Bagimlilik Listesi

| # | Bagimlilik | Yon | Tur | Not |
|---|---|---|---|---|
| 1 | BCL (`Guid`, `DateTimeOffset`) | l0 -> System | **izinli** | `DateTimeOffset` tercih edildi |
| 2 | NuGet paketi | l0 -> paket | **yasak** | `.csproj` referansi 0 |
| 3 | l3 `IClock` | ABI -> l3 | **kosullu** | l0 tipi bilmez, degeri l1/l5 doldurur |
| 4 | l1, l5 -> ABI | ureten -> l0 | **izinli** | somut olay kendi katmaninda |
| 5 | l2, l7 -> ABI | tuketen -> l0 | **izinli** | `ProjectReference` |

---

## §6 Test Stratejisi

| Test turu | Ne dogrular | Hedef |
|---|---|---|
| Unit | her somut event `IDomainEvent`'i saglar | **once RED** |
| Unit | `Sequence` monotonik artar (100 olaylik seri) | 0 geri adim |
| Unit | `Kind` yalniz izinli degerlerden biri | 0 enum disi etiket |
| Unit | zaman kaynagi `IClock` (FixedClock) | deterministik |
| Arch | l0 olay kodu hicbir `VersaCoder.*` tipine bakmaz | 0 |

---

## §7 Hata Durumlari

| Durum | `ErrorCode` | Sonuc | Duzeltme |
|---|---|---|---|
| `EventType` / `Kind` bilinmiyor | `Protocol` | olay atlanir + log | ABI kaydina ekle |
| `Sequence` geriye gitti | `Protocol` | transkript bozuldu sayilir | l5 uretimini durdur |
| `OccurredAtUtc` gelecekte | - | `NEEDS_VALIDATION` | `IClock` sabitlenmis mi bak |
| Event'i tuketici yok | - | **hata degildir** | persist yoksa kasitli |
| Uretim throw firlatti | `Database` | akis durmaz | `Result.Fail` ile sarmalanir |

---

## §8 Kurallar

- `OccurredAtUtc` `DateTime.Now` ile **doldurulmaz**; zaman l3 `IClock`'tan gelir (aksi halde iki TFM farkli zaman gorur).
- `Sequence` bosluk kabul etmez; tekrar/geri adim ekrana yansitilmaz, loglanir.
- Olay adlandirmasi gecmis zamandir: `TaskStarted`, `TaskStartedCmd` degil.
- `IAgentEvent.Kind` yeni degeri ABI'ye eklendiginde l7 etiket tablosu da ayni gun guncellenir.
- Olay uretimi istisna firlatamaz; firlatildi ise `Result.Fail`'e cevrilir, akis durmaz.

---

## §9 Kanit

| # | Iddia | Kanit | Durum |
|---|---|---|---|
| 1 | 6 baglantinin hedefi diskte var | `check-links.ps1` -> bozuk link **0** | olcum |
| 2 | Dosya 3-8 KB | `Get-Item N03-events-abi.md).Length` | olcum |
| 3 | Sira testi | 100 olaylik seri testi: `Sequence` monotonik | test |
| 4 | Yon kurali kaynagi | [ADR-028](../../.decisions/accepted/ADR-028-layered-baseline-9-projects.md) §2.1 | dogrulandi |
| 5 | Madde listesi <= 5 | §8 = 5 madde | sayim |
