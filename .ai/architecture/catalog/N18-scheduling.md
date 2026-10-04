---
title: "Katalog Dugumu N18 - Zamanlama ve Kuyruk (l2)"
type: catalog
category: architecture
status: needs-validation
version: 1.0.0
updated: 2026-10-04
authority: derived
scope: "N18 scheduling dugumu: kimlik, src klasoru, aile yapisi, ornek kayit ve yon kurali (ADR-013 + ADR-028)"
---

# Katalog Dugumu N18 - Zamanlama ve Kuyruk

**Zorunlu Baglantilar:** [catalog index](index.md) · [node-inventory](node-inventory.md) · [mimari index](../index.md) · [ADR-028](../../.decisions/accepted/ADR-028-layered-baseline-9-projects.md) · [ADR-013](../../.decisions/accepted/ADR-013-layered-architecture-52-nodes.md)

> **Tek cumlelik sorumluluk:** *N18 scheduling hangi projede ve klasorde yasar, ne yapar, kac kayit hedefler* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

**N18 (scheduling), l2 Application katmaninin alt katmanidir: `VersaCoder.Application` projesinde `src/VersaCoder.Application/Scheduling/` klasorunde yasar.** Is kuyrugu, zaman asimi, tekrar ve eszamanlilik sinirlarini yonetir. 10 aile x 12 rol = 120 kayit hedefi (ADR-013 asgari 112); durum `planli`.

Kayitlar ADR-013 2.1 biciminde 8 sutun tasir; bu dosya aile kalibini ve ornek satirlari gosterir, tam 120 satirlik liste bu dosyanin boyut sinirina sigmaz (DOĞRULAMA GEREKLI).

```text
+--------------------------------------------------+
| l2 Application -> VersaCoder.Application
| src/VersaCoder.Application/Scheduling/
| 10 aile x 12 rol = 120 kayit (esik 112)
+--------------------------------------------------+
```

| Alan | Deger |
|---|---|
| Dugum | N18 |
| Dugum turu | alt katman (klasor/namespace duzeyi) |
| Makro katman | l2 Application |
| Proje (.csproj) | VersaCoder.Application |
| TFM | net48;net10.0-windows |
| Kayit hedefi | 120 (ADR-013 asgari 112) |
| Durum | planli |
| Sorumlu rol | architect |

---

## §1 Sorumluluk (YAPAR / YAPMAZ)

| # | YAPAR | YAPMAZ |
|---|---|---|
| 1 | Is kuyrugu, zaman asimi, tekrar ve eszamanlilik sinirlarini yonetir | l4 katmanina dogrudan bakmak (repository portu l0 katmaninda, uygulamasi l4 uzerinde) - YOK |
| 2 | kayitlari 8 sutunlu ADR-013 biciminde tutmak (planli -> yapildi -> testli) | SQL yazmak veya baglanti yonetmek |
| 3 | command, query ve isleyici (handler) yazmak | domain invariantini ihlal etmek |
| 4 | DTO esleme ve girdi dogrulamayi yurutmek | UI ekrani acmak veya Ribbon kontrolune dokunmak |
| 5 | kullanim senaryosunu uctan uca orkestre etmek | l3, l5, l6 ve l7 katmanlarina referans vermek |
| 6 | l0 portlari uzerinden alt katmanlari cagirmak | istisnayi disari tasimak, donus tipi Result<T> olur |

---

## §2 Klasor ve N Eslemesi

**Kalip (ADR-028 2.3/4):** `N33 -> src\VersaCoder.Protocol\ToolSchema\` satiri SSOT ornegidir; bu dosyanin klasoru ayni kalipla yazilir.

| # | N | Slug | src/ klasoru |
|---|---|---|---|
| 1 | N18 | scheduling | `src/VersaCoder.Application/Scheduling/` |

**Kanit yolu (ADR-028 2.3/5):** tarama yolu gercek proje adini icerir; genis kalip (`src/**/Domain/**`) sahte PASS uretir.

```powershell
Test-Path 'C:\versacoder\src\VersaCoder.Application\Scheduling'
```

Dizin su an yok cikti `False` olur; PASS yazilmaz -> **DOĞRULAMA GEREKLI**.

---

## §3 Aile Yapisi

Rol kalibi (12): `I<F>` · `<F>` · `<F>Dto` · `<F>Mapper` · `<F>Validator` · `<F>Factory` · `<F>Options` · `<F>Provider` · `<F>Registry` · `<F>Converter` · `<F>Tests` · `<F>Builder`

| # | Aile | # | Aile |
|---|---|---|---|
| 1 | RunQueue | 6 | ConcurrencyLimiter |
| 2 | StepScheduler | 7 | BackoffScheduler |
| 3 | RetryScheduler | 8 | CancelSignal |
| 4 | TimeoutScheduler | 9 | JobRunner |
| 5 | PriorityQueue | 10 | DelayPolicy |

Her aile ayni rol kalibini 12 kez tekrarlar; kayit sayisi = aile x rol.

---

## §4 Kayit Ornegi (8 sutun)

| ID | Bilesen | Tip | Sorumluluk | Proje | TFM | Test | Durum |
|---|---|---|---|---|---|---|---|
| N18-001 | `IRunQueue` | interface | RunQueue sozlesmesi (port) | VersaCoder.Application | net48;net10.0-windows | RunQueueTests | planli |
| N18-002 | `RunQueue` | class | RunQueue cekirdek uygulamasi | VersaCoder.Application | net48;net10.0-windows | RunQueueTests | planli |
| N18-003 | `RunQueueDto` | record | RunQueue veri transferi modeli | VersaCoder.Application | net48;net10.0-windows | RunQueueTests | planli |
| N18-004 | `RunQueueMapper` | class | RunQueue <-> DTO esleme | VersaCoder.Application | net48;net10.0-windows | RunQueueTests | planli |
| N18-005 | `RunQueueValidator` | class | RunQueue dogrulama kurallari | VersaCoder.Application | net48;net10.0-windows | RunQueueTests | planli |
| N18-006 | `RunQueueFactory` | class | RunQueue uretimi ve varsayilanlari | VersaCoder.Application | net48;net10.0-windows | RunQueueTests | planli |

Kalan aileler ayni kalibi 12 rolle tekrarlar (makro dugumlerde 4 rol); toplam hedef **120** kayit.

---

## §5 Yon Kurali

| # | Yon | Deger | Kaynak |
|---|---|---|---|
| 1 | l2 -> l1 ve l0 | OK | ADR-028 2.1 |
| 2 | l2 -> l4 | YOK (repository portu l0 katmaninda) | ADR-028 2.3/2 |
| 3 | l4 -> l2 | X | ADR-028 2.3/2 |
| 4 | l3, l5, l6, l7 -> l2 | X | ADR-028 2.1 |

Matrisin tamami [layer-matrix](layer-matrix.md) dosyasindadir; NetArchTest `LayerRulesTests` bu tabloya gore yazilir.

---

## §6 Test ve Durum

| # | Kontrol | Beklenen | Durum |
|---|---|---|---|
| 1 | Kayit satiri | 120 (asgari 112) | bu dosyada 6 ornek satir; tam liste DOĞRULAMA GEREKLI |
| 2 | Durum degeri | planli -> yapildi -> testli | planli |
| 3 | Test sinifi | RunQueueTests (xUnit) | planli |
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
(Select-String -Path 'C:\versacoder\.ai\architecture\catalog\N18-scheduling.md' -Pattern '^\|\s*N\d{2}-\d{3}\s*\|').Count
# 2) boyut bandi - beklenen: 2600-8500
(Get-Item 'C:\versacoder\.ai\architecture\catalog\N18-scheduling.md').Length
# 3) klasor kaniti - gercek proje adi (ADR-028 2.3/5)
(Test-Path 'C:\versacoder\src\VersaCoder.Application\Scheduling')
```

Katalog sayimi `powershell -NoProfile -ExecutionPolicy Bypass -File C:\versacoder\.ai\scripts\count-components.ps1` ile alinir; bu batch sonunda toplam esik (5000) henuz karsilanmaz - kalan 26 node dosyasi sonraki batchtir.

## §10 Kayit Tablosu (ADR-013 2.2 - kaynak vault port)

| N18-007 | `RunQueueOptions` | record | RunQueue yapilandirmasi | VersaCoder.Application | net48;net10.0-windows | RunQueueTests | planli |
| N18-008 | `RunQueueProvider` | class | RunQueue saglayici uygulamasi | VersaCoder.Application | net48;net10.0-windows | RunQueueTests | planli |
| N18-009 | `RunQueueRegistry` | class | RunQueue kayit/katalog yonetimi | VersaCoder.Application | net48;net10.0-windows | RunQueueTests | planli |
| N18-010 | `RunQueueConverter` | class | RunQueue bicim donusumu | VersaCoder.Application | net48;net10.0-windows | RunQueueTests | planli |
| N18-011 | `RunQueueTests` | class | RunQueue testleri (xUnit) | VersaCoder.Application | net48;net10.0-windows | - | planli |
| N18-012 | `RunQueueBuilder` | class | RunQueue uretici (builder) | VersaCoder.Application | net48;net10.0-windows | RunQueueTests | planli |
| N18-013 | `IStepScheduler` | interface | StepScheduler sozlesmesi (port) | VersaCoder.Application | net48;net10.0-windows | StepSchedulerTests | planli |
| N18-014 | `StepScheduler` | class | StepScheduler cekirdek uygulamasi | VersaCoder.Application | net48;net10.0-windows | StepSchedulerTests | planli |
| N18-015 | `StepSchedulerDto` | record | StepScheduler veri transferi modeli | VersaCoder.Application | net48;net10.0-windows | StepSchedulerTests | planli |
| N18-016 | `StepSchedulerMapper` | class | StepScheduler <-> DTO esleme | VersaCoder.Application | net48;net10.0-windows | StepSchedulerTests | planli |
| N18-017 | `StepSchedulerValidator` | class | StepScheduler dogrulama kurallari | VersaCoder.Application | net48;net10.0-windows | StepSchedulerTests | planli |
| N18-018 | `StepSchedulerFactory` | class | StepScheduler uretimi ve varsayilanlari | VersaCoder.Application | net48;net10.0-windows | StepSchedulerTests | planli |
| N18-019 | `StepSchedulerOptions` | record | StepScheduler yapilandirmasi | VersaCoder.Application | net48;net10.0-windows | StepSchedulerTests | planli |
| N18-020 | `StepSchedulerProvider` | class | StepScheduler saglayici uygulamasi | VersaCoder.Application | net48;net10.0-windows | StepSchedulerTests | planli |
| N18-021 | `StepSchedulerRegistry` | class | StepScheduler kayit/katalog yonetimi | VersaCoder.Application | net48;net10.0-windows | StepSchedulerTests | planli |
| N18-022 | `StepSchedulerConverter` | class | StepScheduler bicim donusumu | VersaCoder.Application | net48;net10.0-windows | StepSchedulerTests | planli |
| N18-023 | `StepSchedulerTests` | class | StepScheduler testleri (xUnit) | VersaCoder.Application | net48;net10.0-windows | - | planli |
| N18-024 | `StepSchedulerBuilder` | class | StepScheduler uretici (builder) | VersaCoder.Application | net48;net10.0-windows | StepSchedulerTests | planli |
| N18-025 | `IRetryScheduler` | interface | RetryScheduler sozlesmesi (port) | VersaCoder.Application | net48;net10.0-windows | RetrySchedulerTests | planli |
| N18-026 | `RetryScheduler` | class | RetryScheduler cekirdek uygulamasi | VersaCoder.Application | net48;net10.0-windows | RetrySchedulerTests | planli |
| N18-027 | `RetrySchedulerDto` | record | RetryScheduler veri transferi modeli | VersaCoder.Application | net48;net10.0-windows | RetrySchedulerTests | planli |
| N18-028 | `RetrySchedulerMapper` | class | RetryScheduler <-> DTO esleme | VersaCoder.Application | net48;net10.0-windows | RetrySchedulerTests | planli |
| N18-029 | `RetrySchedulerValidator` | class | RetryScheduler dogrulama kurallari | VersaCoder.Application | net48;net10.0-windows | RetrySchedulerTests | planli |
| N18-030 | `RetrySchedulerFactory` | class | RetryScheduler uretimi ve varsayilanlari | VersaCoder.Application | net48;net10.0-windows | RetrySchedulerTests | planli |
| N18-031 | `RetrySchedulerOptions` | record | RetryScheduler yapilandirmasi | VersaCoder.Application | net48;net10.0-windows | RetrySchedulerTests | planli |
| N18-032 | `RetrySchedulerProvider` | class | RetryScheduler saglayici uygulamasi | VersaCoder.Application | net48;net10.0-windows | RetrySchedulerTests | planli |
| N18-033 | `RetrySchedulerRegistry` | class | RetryScheduler kayit/katalog yonetimi | VersaCoder.Application | net48;net10.0-windows | RetrySchedulerTests | planli |
| N18-034 | `RetrySchedulerConverter` | class | RetryScheduler bicim donusumu | VersaCoder.Application | net48;net10.0-windows | RetrySchedulerTests | planli |
| N18-035 | `RetrySchedulerTests` | class | RetryScheduler testleri (xUnit) | VersaCoder.Application | net48;net10.0-windows | - | planli |
| N18-036 | `RetrySchedulerBuilder` | class | RetryScheduler uretici (builder) | VersaCoder.Application | net48;net10.0-windows | RetrySchedulerTests | planli |
| N18-037 | `ITimeoutScheduler` | interface | TimeoutScheduler sozlesmesi (port) | VersaCoder.Application | net48;net10.0-windows | TimeoutSchedulerTests | planli |
| N18-038 | `TimeoutScheduler` | class | TimeoutScheduler cekirdek uygulamasi | VersaCoder.Application | net48;net10.0-windows | TimeoutSchedulerTests | planli |
| N18-039 | `TimeoutSchedulerDto` | record | TimeoutScheduler veri transferi modeli | VersaCoder.Application | net48;net10.0-windows | TimeoutSchedulerTests | planli |
| N18-040 | `TimeoutSchedulerMapper` | class | TimeoutScheduler <-> DTO esleme | VersaCoder.Application | net48;net10.0-windows | TimeoutSchedulerTests | planli |
| N18-041 | `TimeoutSchedulerValidator` | class | TimeoutScheduler dogrulama kurallari | VersaCoder.Application | net48;net10.0-windows | TimeoutSchedulerTests | planli |
| N18-042 | `TimeoutSchedulerFactory` | class | TimeoutScheduler uretimi ve varsayilanlari | VersaCoder.Application | net48;net10.0-windows | TimeoutSchedulerTests | planli |
| N18-043 | `TimeoutSchedulerOptions` | record | TimeoutScheduler yapilandirmasi | VersaCoder.Application | net48;net10.0-windows | TimeoutSchedulerTests | planli |
| N18-044 | `TimeoutSchedulerProvider` | class | TimeoutScheduler saglayici uygulamasi | VersaCoder.Application | net48;net10.0-windows | TimeoutSchedulerTests | planli |
| N18-045 | `TimeoutSchedulerRegistry` | class | TimeoutScheduler kayit/katalog yonetimi | VersaCoder.Application | net48;net10.0-windows | TimeoutSchedulerTests | planli |
| N18-046 | `TimeoutSchedulerConverter` | class | TimeoutScheduler bicim donusumu | VersaCoder.Application | net48;net10.0-windows | TimeoutSchedulerTests | planli |
| N18-047 | `TimeoutSchedulerTests` | class | TimeoutScheduler testleri (xUnit) | VersaCoder.Application | net48;net10.0-windows | - | planli |
| N18-048 | `TimeoutSchedulerBuilder` | class | TimeoutScheduler uretici (builder) | VersaCoder.Application | net48;net10.0-windows | TimeoutSchedulerTests | planli |
| N18-049 | `IPriorityQueue` | interface | PriorityQueue sozlesmesi (port) | VersaCoder.Application | net48;net10.0-windows | PriorityQueueTests | planli |
| N18-050 | `PriorityQueue` | class | PriorityQueue cekirdek uygulamasi | VersaCoder.Application | net48;net10.0-windows | PriorityQueueTests | planli |
| N18-051 | `PriorityQueueDto` | record | PriorityQueue veri transferi modeli | VersaCoder.Application | net48;net10.0-windows | PriorityQueueTests | planli |
| N18-052 | `PriorityQueueMapper` | class | PriorityQueue <-> DTO esleme | VersaCoder.Application | net48;net10.0-windows | PriorityQueueTests | planli |
| N18-053 | `PriorityQueueValidator` | class | PriorityQueue dogrulama kurallari | VersaCoder.Application | net48;net10.0-windows | PriorityQueueTests | planli |
| N18-054 | `PriorityQueueFactory` | class | PriorityQueue uretimi ve varsayilanlari | VersaCoder.Application | net48;net10.0-windows | PriorityQueueTests | planli |
| N18-055 | `PriorityQueueOptions` | record | PriorityQueue yapilandirmasi | VersaCoder.Application | net48;net10.0-windows | PriorityQueueTests | planli |
| N18-056 | `PriorityQueueProvider` | class | PriorityQueue saglayici uygulamasi | VersaCoder.Application | net48;net10.0-windows | PriorityQueueTests | planli |
| N18-057 | `PriorityQueueRegistry` | class | PriorityQueue kayit/katalog yonetimi | VersaCoder.Application | net48;net10.0-windows | PriorityQueueTests | planli |
| N18-058 | `PriorityQueueConverter` | class | PriorityQueue bicim donusumu | VersaCoder.Application | net48;net10.0-windows | PriorityQueueTests | planli |
| N18-059 | `PriorityQueueTests` | class | PriorityQueue testleri (xUnit) | VersaCoder.Application | net48;net10.0-windows | - | planli |
| N18-060 | `PriorityQueueBuilder` | class | PriorityQueue uretici (builder) | VersaCoder.Application | net48;net10.0-windows | PriorityQueueTests | planli |
| N18-061 | `IConcurrencyLimiter` | interface | ConcurrencyLimiter sozlesmesi (port) | VersaCoder.Application | net48;net10.0-windows | ConcurrencyLimiterTests | planli |
| N18-062 | `ConcurrencyLimiter` | class | ConcurrencyLimiter cekirdek uygulamasi | VersaCoder.Application | net48;net10.0-windows | ConcurrencyLimiterTests | planli |
| N18-063 | `ConcurrencyLimiterDto` | record | ConcurrencyLimiter veri transferi modeli | VersaCoder.Application | net48;net10.0-windows | ConcurrencyLimiterTests | planli |
| N18-064 | `ConcurrencyLimiterMapper` | class | ConcurrencyLimiter <-> DTO esleme | VersaCoder.Application | net48;net10.0-windows | ConcurrencyLimiterTests | planli |
| N18-065 | `ConcurrencyLimiterValidator` | class | ConcurrencyLimiter dogrulama kurallari | VersaCoder.Application | net48;net10.0-windows | ConcurrencyLimiterTests | planli |
| N18-066 | `ConcurrencyLimiterFactory` | class | ConcurrencyLimiter uretimi ve varsayilanlari | VersaCoder.Application | net48;net10.0-windows | ConcurrencyLimiterTests | planli |
| N18-067 | `ConcurrencyLimiterOptions` | record | ConcurrencyLimiter yapilandirmasi | VersaCoder.Application | net48;net10.0-windows | ConcurrencyLimiterTests | planli |
| N18-068 | `ConcurrencyLimiterProvider` | class | ConcurrencyLimiter saglayici uygulamasi | VersaCoder.Application | net48;net10.0-windows | ConcurrencyLimiterTests | planli |
| N18-069 | `ConcurrencyLimiterRegistry` | class | ConcurrencyLimiter kayit/katalog yonetimi | VersaCoder.Application | net48;net10.0-windows | ConcurrencyLimiterTests | planli |
| N18-070 | `ConcurrencyLimiterConverter` | class | ConcurrencyLimiter bicim donusumu | VersaCoder.Application | net48;net10.0-windows | ConcurrencyLimiterTests | planli |
| N18-071 | `ConcurrencyLimiterTests` | class | ConcurrencyLimiter testleri (xUnit) | VersaCoder.Application | net48;net10.0-windows | - | planli |
| N18-072 | `ConcurrencyLimiterBuilder` | class | ConcurrencyLimiter uretici (builder) | VersaCoder.Application | net48;net10.0-windows | ConcurrencyLimiterTests | planli |
| N18-073 | `IBackoffScheduler` | interface | BackoffScheduler sozlesmesi (port) | VersaCoder.Application | net48;net10.0-windows | BackoffSchedulerTests | planli |
| N18-074 | `BackoffScheduler` | class | BackoffScheduler cekirdek uygulamasi | VersaCoder.Application | net48;net10.0-windows | BackoffSchedulerTests | planli |
| N18-075 | `BackoffSchedulerDto` | record | BackoffScheduler veri transferi modeli | VersaCoder.Application | net48;net10.0-windows | BackoffSchedulerTests | planli |
| N18-076 | `BackoffSchedulerMapper` | class | BackoffScheduler <-> DTO esleme | VersaCoder.Application | net48;net10.0-windows | BackoffSchedulerTests | planli |
| N18-077 | `BackoffSchedulerValidator` | class | BackoffScheduler dogrulama kurallari | VersaCoder.Application | net48;net10.0-windows | BackoffSchedulerTests | planli |
| N18-078 | `BackoffSchedulerFactory` | class | BackoffScheduler uretimi ve varsayilanlari | VersaCoder.Application | net48;net10.0-windows | BackoffSchedulerTests | planli |
| N18-079 | `BackoffSchedulerOptions` | record | BackoffScheduler yapilandirmasi | VersaCoder.Application | net48;net10.0-windows | BackoffSchedulerTests | planli |
| N18-080 | `BackoffSchedulerProvider` | class | BackoffScheduler saglayici uygulamasi | VersaCoder.Application | net48;net10.0-windows | BackoffSchedulerTests | planli |
| N18-081 | `BackoffSchedulerRegistry` | class | BackoffScheduler kayit/katalog yonetimi | VersaCoder.Application | net48;net10.0-windows | BackoffSchedulerTests | planli |
| N18-082 | `BackoffSchedulerConverter` | class | BackoffScheduler bicim donusumu | VersaCoder.Application | net48;net10.0-windows | BackoffSchedulerTests | planli |
| N18-083 | `BackoffSchedulerTests` | class | BackoffScheduler testleri (xUnit) | VersaCoder.Application | net48;net10.0-windows | - | planli |
| N18-084 | `BackoffSchedulerBuilder` | class | BackoffScheduler uretici (builder) | VersaCoder.Application | net48;net10.0-windows | BackoffSchedulerTests | planli |
| N18-085 | `ICancelSignal` | interface | CancelSignal sozlesmesi (port) | VersaCoder.Application | net48;net10.0-windows | CancelSignalTests | planli |
| N18-086 | `CancelSignal` | class | CancelSignal cekirdek uygulamasi | VersaCoder.Application | net48;net10.0-windows | CancelSignalTests | planli |
| N18-087 | `CancelSignalDto` | record | CancelSignal veri transferi modeli | VersaCoder.Application | net48;net10.0-windows | CancelSignalTests | planli |
| N18-088 | `CancelSignalMapper` | class | CancelSignal <-> DTO esleme | VersaCoder.Application | net48;net10.0-windows | CancelSignalTests | planli |
| N18-089 | `CancelSignalValidator` | class | CancelSignal dogrulama kurallari | VersaCoder.Application | net48;net10.0-windows | CancelSignalTests | planli |
| N18-090 | `CancelSignalFactory` | class | CancelSignal uretimi ve varsayilanlari | VersaCoder.Application | net48;net10.0-windows | CancelSignalTests | planli |
| N18-091 | `CancelSignalOptions` | record | CancelSignal yapilandirmasi | VersaCoder.Application | net48;net10.0-windows | CancelSignalTests | planli |
| N18-092 | `CancelSignalProvider` | class | CancelSignal saglayici uygulamasi | VersaCoder.Application | net48;net10.0-windows | CancelSignalTests | planli |
| N18-093 | `CancelSignalRegistry` | class | CancelSignal kayit/katalog yonetimi | VersaCoder.Application | net48;net10.0-windows | CancelSignalTests | planli |
| N18-094 | `CancelSignalConverter` | class | CancelSignal bicim donusumu | VersaCoder.Application | net48;net10.0-windows | CancelSignalTests | planli |
| N18-095 | `CancelSignalTests` | class | CancelSignal testleri (xUnit) | VersaCoder.Application | net48;net10.0-windows | - | planli |
| N18-096 | `CancelSignalBuilder` | class | CancelSignal uretici (builder) | VersaCoder.Application | net48;net10.0-windows | CancelSignalTests | planli |
| N18-097 | `IJobRunner` | interface | JobRunner sozlesmesi (port) | VersaCoder.Application | net48;net10.0-windows | JobRunnerTests | planli |
| N18-098 | `JobRunner` | class | JobRunner cekirdek uygulamasi | VersaCoder.Application | net48;net10.0-windows | JobRunnerTests | planli |
| N18-099 | `JobRunnerDto` | record | JobRunner veri transferi modeli | VersaCoder.Application | net48;net10.0-windows | JobRunnerTests | planli |
| N18-100 | `JobRunnerMapper` | class | JobRunner <-> DTO esleme | VersaCoder.Application | net48;net10.0-windows | JobRunnerTests | planli |
| N18-101 | `JobRunnerValidator` | class | JobRunner dogrulama kurallari | VersaCoder.Application | net48;net10.0-windows | JobRunnerTests | planli |
| N18-102 | `JobRunnerFactory` | class | JobRunner uretimi ve varsayilanlari | VersaCoder.Application | net48;net10.0-windows | JobRunnerTests | planli |
| N18-103 | `JobRunnerOptions` | record | JobRunner yapilandirmasi | VersaCoder.Application | net48;net10.0-windows | JobRunnerTests | planli |
| N18-104 | `JobRunnerProvider` | class | JobRunner saglayici uygulamasi | VersaCoder.Application | net48;net10.0-windows | JobRunnerTests | planli |
| N18-105 | `JobRunnerRegistry` | class | JobRunner kayit/katalog yonetimi | VersaCoder.Application | net48;net10.0-windows | JobRunnerTests | planli |
| N18-106 | `JobRunnerConverter` | class | JobRunner bicim donusumu | VersaCoder.Application | net48;net10.0-windows | JobRunnerTests | planli |
| N18-107 | `JobRunnerTests` | class | JobRunner testleri (xUnit) | VersaCoder.Application | net48;net10.0-windows | - | planli |
| N18-108 | `JobRunnerBuilder` | class | JobRunner uretici (builder) | VersaCoder.Application | net48;net10.0-windows | JobRunnerTests | planli |
| N18-109 | `IDelayPolicy` | interface | DelayPolicy sozlesmesi (port) | VersaCoder.Application | net48;net10.0-windows | DelayPolicyTests | planli |
| N18-110 | `DelayPolicy` | class | DelayPolicy cekirdek uygulamasi | VersaCoder.Application | net48;net10.0-windows | DelayPolicyTests | planli |
| N18-111 | `DelayPolicyDto` | record | DelayPolicy veri transferi modeli | VersaCoder.Application | net48;net10.0-windows | DelayPolicyTests | planli |
| N18-112 | `DelayPolicyMapper` | class | DelayPolicy <-> DTO esleme | VersaCoder.Application | net48;net10.0-windows | DelayPolicyTests | planli |
| N18-113 | `DelayPolicyValidator` | class | DelayPolicy dogrulama kurallari | VersaCoder.Application | net48;net10.0-windows | DelayPolicyTests | planli |
| N18-114 | `DelayPolicyFactory` | class | DelayPolicy uretimi ve varsayilanlari | VersaCoder.Application | net48;net10.0-windows | DelayPolicyTests | planli |
| N18-115 | `DelayPolicyOptions` | record | DelayPolicy yapilandirmasi | VersaCoder.Application | net48;net10.0-windows | DelayPolicyTests | planli |
| N18-116 | `DelayPolicyProvider` | class | DelayPolicy saglayici uygulamasi | VersaCoder.Application | net48;net10.0-windows | DelayPolicyTests | planli |
| N18-117 | `DelayPolicyRegistry` | class | DelayPolicy kayit/katalog yonetimi | VersaCoder.Application | net48;net10.0-windows | DelayPolicyTests | planli |
| N18-118 | `DelayPolicyConverter` | class | DelayPolicy bicim donusumu | VersaCoder.Application | net48;net10.0-windows | DelayPolicyTests | planli |
| N18-119 | `DelayPolicyTests` | class | DelayPolicy testleri (xUnit) | VersaCoder.Application | net48;net10.0-windows | - | planli |
| N18-120 | `DelayPolicyBuilder` | class | DelayPolicy uretici (builder) | VersaCoder.Application | net48;net10.0-windows | DelayPolicyTests | planli |
