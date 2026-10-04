---
title: "Katalog Dugumu N21 - Saat ve Zaman (l3)"
type: catalog
category: architecture
status: needs-validation
version: 1.0.0
updated: 2026-10-04
authority: derived
scope: "N21 clock-time dugumu: kimlik, src klasoru, aile yapisi, ornek kayit ve yon kurali (ADR-013 + ADR-028)"
---

# Katalog Dugumu N21 - Saat ve Zaman

**Zorunlu Baglantilar:** [catalog index](index.md) · [node-inventory](node-inventory.md) · [mimari index](../index.md) · [ADR-028](../../.decisions/accepted/ADR-028-layered-baseline-9-projects.md) · [ADR-013](../../.decisions/accepted/ADR-013-layered-architecture-52-nodes.md)

> **Tek cumlelik sorumluluk:** *N21 clock-time hangi projede ve klasorde yasar, ne yapar, kac kayit hedefler* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

**N21 (clock-time), l3 Crosscutting katmaninin alt katmanidir: `VersaCoder.Crosscutting` projesinde `src/VersaCoder.Crosscutting/Clock/` klasorunde yasar.** IClock, stopwatch, deadline ve TTL gibi zaman soyutlamalarini verir. 10 aile x 12 rol = 120 kayit hedefi (ADR-013 asgari 112); durum `planli`.

Kayitlar ADR-013 2.1 biciminde 8 sutun tasir; bu dosya aile kalibini ve ornek satirlari gosterir, tam 120 satirlik liste bu dosyanin boyut sinirina sigmaz (DOĞRULAMA GEREKLI).

```text
+--------------------------------------------------+
| l3 Crosscutting -> VersaCoder.Crosscutting
| src/VersaCoder.Crosscutting/Clock/
| 10 aile x 12 rol = 120 kayit (esik 112)
+--------------------------------------------------+
```

| Alan | Deger |
|---|---|
| Dugum | N21 |
| Dugum turu | alt katman (klasor/namespace duzeyi) |
| Makro katman | l3 Crosscutting |
| Proje (.csproj) | VersaCoder.Crosscutting |
| TFM | net48;net10.0-windows |
| Kayit hedefi | 120 (ADR-013 asgari 112) |
| Durum | planli |
| Sorumlu rol | architect |

---

## §1 Sorumluluk (YAPAR / YAPMAZ)

| # | YAPAR | YAPMAZ |
|---|---|---|
| 1 | IClock, stopwatch, deadline ve TTL gibi zaman soyutlamalarini verir | l0 disinda bir katmana dogrudan bakmak |
| 2 | kayitlari 8 sutunlu ADR-013 biciminde tutmak (planli -> yapildi -> testli) | is kurali veya invariant tasimak, is l1 katmaninda |
| 3 | ortak kesit servisini vermek: log, saat, onbek, kripto, yapilandirma | kullanim senaryosu orkestre etmek, is l2 katmaninda |
| 4 | test edilebilir saricilar sunmak (IClock, ILogSink) | SQL veya veri erisimi yazmak, is l4 katmaninda |
| 5 | yalniz l0 katmanina bakmak | UI tipine bakmak (Desktop / Cli) |
| 6 | katmanlari degil alt altyapiyi sarmak | secret veya anahtar barindirmak, keys.md tek kaynaktir |

---

## §2 Klasor ve N Eslemesi

**Kalip (ADR-028 2.3/4):** `N33 -> src\VersaCoder.Protocol\ToolSchema\` satiri SSOT ornegidir; bu dosyanin klasoru ayni kalipla yazilir.

| # | N | Slug | src/ klasoru |
|---|---|---|---|
| 1 | N21 | clock-time | `src/VersaCoder.Crosscutting/Clock/` |

**Kanit yolu (ADR-028 2.3/5):** tarama yolu gercek proje adini icerir; genis kalip (`src/**/Domain/**`) sahte PASS uretir.

```powershell
Test-Path 'C:\versacoder\src\VersaCoder.Crosscutting\Clock'
```

Dizin su an yok cikti `False` olur; PASS yazilmaz -> **DOĞRULAMA GEREKLI**.

---

## §3 Aile Yapisi

Rol kalibi (12): `I<F>` · `<F>` · `<F>Dto` · `<F>Mapper` · `<F>Validator` · `<F>Factory` · `<F>Options` · `<F>Provider` · `<F>Registry` · `<F>Converter` · `<F>Tests` · `<F>Builder`

| # | Aile | # | Aile |
|---|---|---|---|
| 1 | Clock | 6 | DateProvider |
| 2 | UtcClock | 7 | TimezoneProvider |
| 3 | Stopwatch | 8 | Interval |
| 4 | Deadline | 9 | Debouncer |
| 5 | Ttl | 10 | Throttler |

Her aile ayni rol kalibini 12 kez tekrarlar; kayit sayisi = aile x rol.

---

## §4 Kayit Ornegi (8 sutun)

| ID | Bilesen | Tip | Sorumluluk | Proje | TFM | Test | Durum |
|---|---|---|---|---|---|---|---|
| N21-001 | `IClock` | interface | Clock sozlesmesi (port) | VersaCoder.Crosscutting | net48;net10.0-windows | ClockTests | planli |
| N21-002 | `Clock` | class | Clock cekirdek uygulamasi | VersaCoder.Crosscutting | net48;net10.0-windows | ClockTests | planli |
| N21-003 | `ClockDto` | record | Clock veri transferi modeli | VersaCoder.Crosscutting | net48;net10.0-windows | ClockTests | planli |
| N21-004 | `ClockMapper` | class | Clock <-> DTO esleme | VersaCoder.Crosscutting | net48;net10.0-windows | ClockTests | planli |
| N21-005 | `ClockValidator` | class | Clock dogrulama kurallari | VersaCoder.Crosscutting | net48;net10.0-windows | ClockTests | planli |
| N21-006 | `ClockFactory` | class | Clock uretimi ve varsayilanlari | VersaCoder.Crosscutting | net48;net10.0-windows | ClockTests | planli |

Kalan aileler ayni kalibi 12 rolle tekrarlar (makro dugumlerde 4 rol); toplam hedef **120** kayit.

---

## §5 Yon Kurali

| # | Yon | Deger | Kaynak |
|---|---|---|---|
| 1 | l3 -> l0 | OK | ADR-028 2.1 |
| 2 | l3 -> l1, l2, l4..l7 | X | ADR-028 2.1 |
| 3 | l5 -> l4 | X | ADR-028 2.3/2 |

Matrisin tamami [layer-matrix](layer-matrix.md) dosyasindadir; NetArchTest `LayerRulesTests` bu tabloya gore yazilir.

---

## §6 Test ve Durum

| # | Kontrol | Beklenen | Durum |
|---|---|---|---|
| 1 | Kayit satiri | 120 (asgari 112) | bu dosyada 6 ornek satir; tam liste DOĞRULAMA GEREKLI |
| 2 | Durum degeri | planli -> yapildi -> testli | planli |
| 3 | Test sinifi | ClockTests (xUnit) | planli |
| 4 | NetArchTest yon denetimi | izinsiz kenar = 0 | planli (LayerRulesTests) |
| 5 | Dosya boyutu | 3-8 KB (2600-8500 bayt) | §9 olcumu |

---

## §7 Kapsam Disi

- K3 katmaninin 618 DLL envanteri bu dugume girmez; tek iz `reports\k3-618-ham-envanter.md` dosyasidir (ADR-028 2.2).
- Tam kayit listesi (120 satir) bu dosyaya sasmaz; ornek satirlar aile kalibini gosterir -> **DOĞRULAMA GEREKLI**.
- Kod yazimi `src/` icinde yapilir; bu dosya yalniz katalog kartidir, bir `.csproj` degildir.

---

## §8 Ilgili Dosyalar

| # | Dosya | Ne verir |
|---|---|---|
| 1 | [catalog index](index.md) | 52 dugum envanteri + N <-> klasor eslemesi |
| 2 | [node-inventory](node-inventory.md) | 52 dugumun tek tek kunyesi |
| 3 | [layer-matrix](layer-matrix.md) | 8x8 yon matrisi (ADR-028 2.3/2) |
| 4 | [project-map](project-map.md) | 9 uretim + 1-2 test proje, N araliklari |
| 5 | [mimari index](../index.md) | 8 makro katman ozeti |
| 6 | [ADR-028](../../.decisions/accepted/ADR-028-layered-baseline-9-projects.md) | baz karar: 8 katman / 52 dugum / 9 proje |

---

## §9 Kanit

```powershell
`# 1) kayit sayimi (ADR-013 2.2) - beklenen: 120
(Select-String -Path 'C:\versacoder\.ai\architecture\catalog\N21-clock-time.md' -Pattern '^\|\s*N\d{2}-\d{3}\s*\|').Count
# 2) boyut bandi - beklenen: 2600-8500
(Get-Item 'C:\versacoder\.ai\architecture\catalog\N21-clock-time.md').Length
# 3) klasor kaniti - gercek proje adi (ADR-028 2.3/5)
(Test-Path 'C:\versacoder\src\VersaCoder.Crosscutting\Clock')
```

Katalog sayimi `powershell -NoProfile -ExecutionPolicy Bypass -File C:\versacoder\.ai\scripts\count-components.ps1` ile alinir; bu batch sonunda toplam esik (5000) henuz karsilanmaz - kalan 26 node dosyasi sonraki batchtir.

## §10 Kayit Tablosu (ADR-013 2.2 - kaynak vault port)

| N21-007 | `ClockOptions` | record | Clock yapilandirmasi | VersaCoder.Crosscutting | net48;net10.0-windows | ClockTests | planli |
| N21-008 | `ClockProvider` | class | Clock saglayici uygulamasi | VersaCoder.Crosscutting | net48;net10.0-windows | ClockTests | planli |
| N21-009 | `ClockRegistry` | class | Clock kayit/katalog yonetimi | VersaCoder.Crosscutting | net48;net10.0-windows | ClockTests | planli |
| N21-010 | `ClockConverter` | class | Clock bicim donusumu | VersaCoder.Crosscutting | net48;net10.0-windows | ClockTests | planli |
| N21-011 | `ClockTests` | class | Clock testleri (xUnit) | VersaCoder.Crosscutting | net48;net10.0-windows | - | planli |
| N21-012 | `ClockBuilder` | class | Clock uretici (builder) | VersaCoder.Crosscutting | net48;net10.0-windows | ClockTests | planli |
| N21-013 | `IUtcClock` | interface | UtcClock sozlesmesi (port) | VersaCoder.Crosscutting | net48;net10.0-windows | UtcClockTests | planli |
| N21-014 | `UtcClock` | class | UtcClock cekirdek uygulamasi | VersaCoder.Crosscutting | net48;net10.0-windows | UtcClockTests | planli |
| N21-015 | `UtcClockDto` | record | UtcClock veri transferi modeli | VersaCoder.Crosscutting | net48;net10.0-windows | UtcClockTests | planli |
| N21-016 | `UtcClockMapper` | class | UtcClock <-> DTO esleme | VersaCoder.Crosscutting | net48;net10.0-windows | UtcClockTests | planli |
| N21-017 | `UtcClockValidator` | class | UtcClock dogrulama kurallari | VersaCoder.Crosscutting | net48;net10.0-windows | UtcClockTests | planli |
| N21-018 | `UtcClockFactory` | class | UtcClock uretimi ve varsayilanlari | VersaCoder.Crosscutting | net48;net10.0-windows | UtcClockTests | planli |
| N21-019 | `UtcClockOptions` | record | UtcClock yapilandirmasi | VersaCoder.Crosscutting | net48;net10.0-windows | UtcClockTests | planli |
| N21-020 | `UtcClockProvider` | class | UtcClock saglayici uygulamasi | VersaCoder.Crosscutting | net48;net10.0-windows | UtcClockTests | planli |
| N21-021 | `UtcClockRegistry` | class | UtcClock kayit/katalog yonetimi | VersaCoder.Crosscutting | net48;net10.0-windows | UtcClockTests | planli |
| N21-022 | `UtcClockConverter` | class | UtcClock bicim donusumu | VersaCoder.Crosscutting | net48;net10.0-windows | UtcClockTests | planli |
| N21-023 | `UtcClockTests` | class | UtcClock testleri (xUnit) | VersaCoder.Crosscutting | net48;net10.0-windows | - | planli |
| N21-024 | `UtcClockBuilder` | class | UtcClock uretici (builder) | VersaCoder.Crosscutting | net48;net10.0-windows | UtcClockTests | planli |
| N21-025 | `IStopwatch` | interface | Stopwatch sozlesmesi (port) | VersaCoder.Crosscutting | net48;net10.0-windows | StopwatchTests | planli |
| N21-026 | `Stopwatch` | class | Stopwatch cekirdek uygulamasi | VersaCoder.Crosscutting | net48;net10.0-windows | StopwatchTests | planli |
| N21-027 | `StopwatchDto` | record | Stopwatch veri transferi modeli | VersaCoder.Crosscutting | net48;net10.0-windows | StopwatchTests | planli |
| N21-028 | `StopwatchMapper` | class | Stopwatch <-> DTO esleme | VersaCoder.Crosscutting | net48;net10.0-windows | StopwatchTests | planli |
| N21-029 | `StopwatchValidator` | class | Stopwatch dogrulama kurallari | VersaCoder.Crosscutting | net48;net10.0-windows | StopwatchTests | planli |
| N21-030 | `StopwatchFactory` | class | Stopwatch uretimi ve varsayilanlari | VersaCoder.Crosscutting | net48;net10.0-windows | StopwatchTests | planli |
| N21-031 | `StopwatchOptions` | record | Stopwatch yapilandirmasi | VersaCoder.Crosscutting | net48;net10.0-windows | StopwatchTests | planli |
| N21-032 | `StopwatchProvider` | class | Stopwatch saglayici uygulamasi | VersaCoder.Crosscutting | net48;net10.0-windows | StopwatchTests | planli |
| N21-033 | `StopwatchRegistry` | class | Stopwatch kayit/katalog yonetimi | VersaCoder.Crosscutting | net48;net10.0-windows | StopwatchTests | planli |
| N21-034 | `StopwatchConverter` | class | Stopwatch bicim donusumu | VersaCoder.Crosscutting | net48;net10.0-windows | StopwatchTests | planli |
| N21-035 | `StopwatchTests` | class | Stopwatch testleri (xUnit) | VersaCoder.Crosscutting | net48;net10.0-windows | - | planli |
| N21-036 | `StopwatchBuilder` | class | Stopwatch uretici (builder) | VersaCoder.Crosscutting | net48;net10.0-windows | StopwatchTests | planli |
| N21-037 | `IDeadline` | interface | Deadline sozlesmesi (port) | VersaCoder.Crosscutting | net48;net10.0-windows | DeadlineTests | planli |
| N21-038 | `Deadline` | class | Deadline cekirdek uygulamasi | VersaCoder.Crosscutting | net48;net10.0-windows | DeadlineTests | planli |
| N21-039 | `DeadlineDto` | record | Deadline veri transferi modeli | VersaCoder.Crosscutting | net48;net10.0-windows | DeadlineTests | planli |
| N21-040 | `DeadlineMapper` | class | Deadline <-> DTO esleme | VersaCoder.Crosscutting | net48;net10.0-windows | DeadlineTests | planli |
| N21-041 | `DeadlineValidator` | class | Deadline dogrulama kurallari | VersaCoder.Crosscutting | net48;net10.0-windows | DeadlineTests | planli |
| N21-042 | `DeadlineFactory` | class | Deadline uretimi ve varsayilanlari | VersaCoder.Crosscutting | net48;net10.0-windows | DeadlineTests | planli |
| N21-043 | `DeadlineOptions` | record | Deadline yapilandirmasi | VersaCoder.Crosscutting | net48;net10.0-windows | DeadlineTests | planli |
| N21-044 | `DeadlineProvider` | class | Deadline saglayici uygulamasi | VersaCoder.Crosscutting | net48;net10.0-windows | DeadlineTests | planli |
| N21-045 | `DeadlineRegistry` | class | Deadline kayit/katalog yonetimi | VersaCoder.Crosscutting | net48;net10.0-windows | DeadlineTests | planli |
| N21-046 | `DeadlineConverter` | class | Deadline bicim donusumu | VersaCoder.Crosscutting | net48;net10.0-windows | DeadlineTests | planli |
| N21-047 | `DeadlineTests` | class | Deadline testleri (xUnit) | VersaCoder.Crosscutting | net48;net10.0-windows | - | planli |
| N21-048 | `DeadlineBuilder` | class | Deadline uretici (builder) | VersaCoder.Crosscutting | net48;net10.0-windows | DeadlineTests | planli |
| N21-049 | `ITtl` | interface | Ttl sozlesmesi (port) | VersaCoder.Crosscutting | net48;net10.0-windows | TtlTests | planli |
| N21-050 | `Ttl` | class | Ttl cekirdek uygulamasi | VersaCoder.Crosscutting | net48;net10.0-windows | TtlTests | planli |
| N21-051 | `TtlDto` | record | Ttl veri transferi modeli | VersaCoder.Crosscutting | net48;net10.0-windows | TtlTests | planli |
| N21-052 | `TtlMapper` | class | Ttl <-> DTO esleme | VersaCoder.Crosscutting | net48;net10.0-windows | TtlTests | planli |
| N21-053 | `TtlValidator` | class | Ttl dogrulama kurallari | VersaCoder.Crosscutting | net48;net10.0-windows | TtlTests | planli |
| N21-054 | `TtlFactory` | class | Ttl uretimi ve varsayilanlari | VersaCoder.Crosscutting | net48;net10.0-windows | TtlTests | planli |
| N21-055 | `TtlOptions` | record | Ttl yapilandirmasi | VersaCoder.Crosscutting | net48;net10.0-windows | TtlTests | planli |
| N21-056 | `TtlProvider` | class | Ttl saglayici uygulamasi | VersaCoder.Crosscutting | net48;net10.0-windows | TtlTests | planli |
| N21-057 | `TtlRegistry` | class | Ttl kayit/katalog yonetimi | VersaCoder.Crosscutting | net48;net10.0-windows | TtlTests | planli |
| N21-058 | `TtlConverter` | class | Ttl bicim donusumu | VersaCoder.Crosscutting | net48;net10.0-windows | TtlTests | planli |
| N21-059 | `TtlTests` | class | Ttl testleri (xUnit) | VersaCoder.Crosscutting | net48;net10.0-windows | - | planli |
| N21-060 | `TtlBuilder` | class | Ttl uretici (builder) | VersaCoder.Crosscutting | net48;net10.0-windows | TtlTests | planli |
| N21-061 | `IDateProvider` | interface | DateProvider sozlesmesi (port) | VersaCoder.Crosscutting | net48;net10.0-windows | DateProviderTests | planli |
| N21-062 | `DateProvider` | class | DateProvider cekirdek uygulamasi | VersaCoder.Crosscutting | net48;net10.0-windows | DateProviderTests | planli |
| N21-063 | `DateProviderDto` | record | DateProvider veri transferi modeli | VersaCoder.Crosscutting | net48;net10.0-windows | DateProviderTests | planli |
| N21-064 | `DateProviderMapper` | class | DateProvider <-> DTO esleme | VersaCoder.Crosscutting | net48;net10.0-windows | DateProviderTests | planli |
| N21-065 | `DateProviderValidator` | class | DateProvider dogrulama kurallari | VersaCoder.Crosscutting | net48;net10.0-windows | DateProviderTests | planli |
| N21-066 | `DateProviderFactory` | class | DateProvider uretimi ve varsayilanlari | VersaCoder.Crosscutting | net48;net10.0-windows | DateProviderTests | planli |
| N21-067 | `DateProviderOptions` | record | DateProvider yapilandirmasi | VersaCoder.Crosscutting | net48;net10.0-windows | DateProviderTests | planli |
| N21-068 | `DateProviderProvider` | class | DateProvider saglayici uygulamasi | VersaCoder.Crosscutting | net48;net10.0-windows | DateProviderTests | planli |
| N21-069 | `DateProviderRegistry` | class | DateProvider kayit/katalog yonetimi | VersaCoder.Crosscutting | net48;net10.0-windows | DateProviderTests | planli |
| N21-070 | `DateProviderConverter` | class | DateProvider bicim donusumu | VersaCoder.Crosscutting | net48;net10.0-windows | DateProviderTests | planli |
| N21-071 | `DateProviderTests` | class | DateProvider testleri (xUnit) | VersaCoder.Crosscutting | net48;net10.0-windows | - | planli |
| N21-072 | `DateProviderBuilder` | class | DateProvider uretici (builder) | VersaCoder.Crosscutting | net48;net10.0-windows | DateProviderTests | planli |
| N21-073 | `ITimezoneProvider` | interface | TimezoneProvider sozlesmesi (port) | VersaCoder.Crosscutting | net48;net10.0-windows | TimezoneProviderTests | planli |
| N21-074 | `TimezoneProvider` | class | TimezoneProvider cekirdek uygulamasi | VersaCoder.Crosscutting | net48;net10.0-windows | TimezoneProviderTests | planli |
| N21-075 | `TimezoneProviderDto` | record | TimezoneProvider veri transferi modeli | VersaCoder.Crosscutting | net48;net10.0-windows | TimezoneProviderTests | planli |
| N21-076 | `TimezoneProviderMapper` | class | TimezoneProvider <-> DTO esleme | VersaCoder.Crosscutting | net48;net10.0-windows | TimezoneProviderTests | planli |
| N21-077 | `TimezoneProviderValidator` | class | TimezoneProvider dogrulama kurallari | VersaCoder.Crosscutting | net48;net10.0-windows | TimezoneProviderTests | planli |
| N21-078 | `TimezoneProviderFactory` | class | TimezoneProvider uretimi ve varsayilanlari | VersaCoder.Crosscutting | net48;net10.0-windows | TimezoneProviderTests | planli |
| N21-079 | `TimezoneProviderOptions` | record | TimezoneProvider yapilandirmasi | VersaCoder.Crosscutting | net48;net10.0-windows | TimezoneProviderTests | planli |
| N21-080 | `TimezoneProviderProvider` | class | TimezoneProvider saglayici uygulamasi | VersaCoder.Crosscutting | net48;net10.0-windows | TimezoneProviderTests | planli |
| N21-081 | `TimezoneProviderRegistry` | class | TimezoneProvider kayit/katalog yonetimi | VersaCoder.Crosscutting | net48;net10.0-windows | TimezoneProviderTests | planli |
| N21-082 | `TimezoneProviderConverter` | class | TimezoneProvider bicim donusumu | VersaCoder.Crosscutting | net48;net10.0-windows | TimezoneProviderTests | planli |
| N21-083 | `TimezoneProviderTests` | class | TimezoneProvider testleri (xUnit) | VersaCoder.Crosscutting | net48;net10.0-windows | - | planli |
| N21-084 | `TimezoneProviderBuilder` | class | TimezoneProvider uretici (builder) | VersaCoder.Crosscutting | net48;net10.0-windows | TimezoneProviderTests | planli |
| N21-085 | `IInterval` | interface | Interval sozlesmesi (port) | VersaCoder.Crosscutting | net48;net10.0-windows | IntervalTests | planli |
| N21-086 | `Interval` | class | Interval cekirdek uygulamasi | VersaCoder.Crosscutting | net48;net10.0-windows | IntervalTests | planli |
| N21-087 | `IntervalDto` | record | Interval veri transferi modeli | VersaCoder.Crosscutting | net48;net10.0-windows | IntervalTests | planli |
| N21-088 | `IntervalMapper` | class | Interval <-> DTO esleme | VersaCoder.Crosscutting | net48;net10.0-windows | IntervalTests | planli |
| N21-089 | `IntervalValidator` | class | Interval dogrulama kurallari | VersaCoder.Crosscutting | net48;net10.0-windows | IntervalTests | planli |
| N21-090 | `IntervalFactory` | class | Interval uretimi ve varsayilanlari | VersaCoder.Crosscutting | net48;net10.0-windows | IntervalTests | planli |
| N21-091 | `IntervalOptions` | record | Interval yapilandirmasi | VersaCoder.Crosscutting | net48;net10.0-windows | IntervalTests | planli |
| N21-092 | `IntervalProvider` | class | Interval saglayici uygulamasi | VersaCoder.Crosscutting | net48;net10.0-windows | IntervalTests | planli |
| N21-093 | `IntervalRegistry` | class | Interval kayit/katalog yonetimi | VersaCoder.Crosscutting | net48;net10.0-windows | IntervalTests | planli |
| N21-094 | `IntervalConverter` | class | Interval bicim donusumu | VersaCoder.Crosscutting | net48;net10.0-windows | IntervalTests | planli |
| N21-095 | `IntervalTests` | class | Interval testleri (xUnit) | VersaCoder.Crosscutting | net48;net10.0-windows | - | planli |
| N21-096 | `IntervalBuilder` | class | Interval uretici (builder) | VersaCoder.Crosscutting | net48;net10.0-windows | IntervalTests | planli |
| N21-097 | `IDebouncer` | interface | Debouncer sozlesmesi (port) | VersaCoder.Crosscutting | net48;net10.0-windows | DebouncerTests | planli |
| N21-098 | `Debouncer` | class | Debouncer cekirdek uygulamasi | VersaCoder.Crosscutting | net48;net10.0-windows | DebouncerTests | planli |
| N21-099 | `DebouncerDto` | record | Debouncer veri transferi modeli | VersaCoder.Crosscutting | net48;net10.0-windows | DebouncerTests | planli |
| N21-100 | `DebouncerMapper` | class | Debouncer <-> DTO esleme | VersaCoder.Crosscutting | net48;net10.0-windows | DebouncerTests | planli |
| N21-101 | `DebouncerValidator` | class | Debouncer dogrulama kurallari | VersaCoder.Crosscutting | net48;net10.0-windows | DebouncerTests | planli |
| N21-102 | `DebouncerFactory` | class | Debouncer uretimi ve varsayilanlari | VersaCoder.Crosscutting | net48;net10.0-windows | DebouncerTests | planli |
| N21-103 | `DebouncerOptions` | record | Debouncer yapilandirmasi | VersaCoder.Crosscutting | net48;net10.0-windows | DebouncerTests | planli |
| N21-104 | `DebouncerProvider` | class | Debouncer saglayici uygulamasi | VersaCoder.Crosscutting | net48;net10.0-windows | DebouncerTests | planli |
| N21-105 | `DebouncerRegistry` | class | Debouncer kayit/katalog yonetimi | VersaCoder.Crosscutting | net48;net10.0-windows | DebouncerTests | planli |
| N21-106 | `DebouncerConverter` | class | Debouncer bicim donusumu | VersaCoder.Crosscutting | net48;net10.0-windows | DebouncerTests | planli |
| N21-107 | `DebouncerTests` | class | Debouncer testleri (xUnit) | VersaCoder.Crosscutting | net48;net10.0-windows | - | planli |
| N21-108 | `DebouncerBuilder` | class | Debouncer uretici (builder) | VersaCoder.Crosscutting | net48;net10.0-windows | DebouncerTests | planli |
| N21-109 | `IThrottler` | interface | Throttler sozlesmesi (port) | VersaCoder.Crosscutting | net48;net10.0-windows | ThrottlerTests | planli |
| N21-110 | `Throttler` | class | Throttler cekirdek uygulamasi | VersaCoder.Crosscutting | net48;net10.0-windows | ThrottlerTests | planli |
| N21-111 | `ThrottlerDto` | record | Throttler veri transferi modeli | VersaCoder.Crosscutting | net48;net10.0-windows | ThrottlerTests | planli |
| N21-112 | `ThrottlerMapper` | class | Throttler <-> DTO esleme | VersaCoder.Crosscutting | net48;net10.0-windows | ThrottlerTests | planli |
| N21-113 | `ThrottlerValidator` | class | Throttler dogrulama kurallari | VersaCoder.Crosscutting | net48;net10.0-windows | ThrottlerTests | planli |
| N21-114 | `ThrottlerFactory` | class | Throttler uretimi ve varsayilanlari | VersaCoder.Crosscutting | net48;net10.0-windows | ThrottlerTests | planli |
| N21-115 | `ThrottlerOptions` | record | Throttler yapilandirmasi | VersaCoder.Crosscutting | net48;net10.0-windows | ThrottlerTests | planli |
| N21-116 | `ThrottlerProvider` | class | Throttler saglayici uygulamasi | VersaCoder.Crosscutting | net48;net10.0-windows | ThrottlerTests | planli |
| N21-117 | `ThrottlerRegistry` | class | Throttler kayit/katalog yonetimi | VersaCoder.Crosscutting | net48;net10.0-windows | ThrottlerTests | planli |
| N21-118 | `ThrottlerConverter` | class | Throttler bicim donusumu | VersaCoder.Crosscutting | net48;net10.0-windows | ThrottlerTests | planli |
| N21-119 | `ThrottlerTests` | class | Throttler testleri (xUnit) | VersaCoder.Crosscutting | net48;net10.0-windows | - | planli |
| N21-120 | `ThrottlerBuilder` | class | Throttler uretici (builder) | VersaCoder.Crosscutting | net48;net10.0-windows | ThrottlerTests | planli |
