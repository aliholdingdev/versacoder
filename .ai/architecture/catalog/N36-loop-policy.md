---
title: Katalog Dugumu N36 - Dongu Politikasi
type: catalog-node
category: l5-protocol
status: needs-validation
version: 1.0.0
updated: 2026-10-04
authority: derived
scope: ajanda dongu adimi, butce ve durma kurali
---

# Katalog Dugumu N36 - Dongu Politikasi

**Zorunlu Baglantilar:** index.md | node-inventory.md | ../index.md | ../../.decisions/accepted/ADR-028-layered-baseline-9-projects.md | ../../.decisions/accepted/ADR-013-layered-architecture-52-nodes.md

> Dugum, katman sozlesmesini cevaplar ve kanitini baglar.

## §0 Ozet (Answer First)

| Alan | Deger |
| --- | --- |
| Katman | l5 Protocol |
| Proje | VersaCoder.Protocol |
| Kapsam | ajanda dongu adimi, butce ve durma kurali |
| Kaynak | C:/www/versacoder/versacoder/.ai/architecture/catalog/l5-protocol/N36-loop-policy.md |
| Sorumluluk | Dugum, katman sozlesmesini cevaplar ve kanitini baglar. |

- Dongu politikasi adim butcesi, durma kosulu ve duraklatma sinyalini tanimlar.
- Derinlik limiti ve paralellik politikasi sonsuz donguyu kokten kirar.
- Ilerleme raporu ve dongu metrikleri her adimda uretilip disari bildirilir.
- Kesinti sinyali guvenli noktada durur, yarim is emri geri alinir.
- Kayitlar en az 6 aile 120 ornek ile kanitlanir, gerisi namludur (kayit yok).

## §1 YAPAR / YAPMAZ

| YAPAR | YAPMAZ |
| --- | --- |
| Dongu butcesi ve durma kuralini uretir | Donguyu kendisi calistirir (l6 is) |
| | l0 + l1 + l2 (l4 kabul) | - |
| | protokol kurallari, mesaj ve dongu sozlesmeleri | - |

## §2 Bagimlilik Yonu

| Yon | Durum | Aciklama |
| --- | --- | --- |
| l0 | EVET | asagidan tek yon |
| l1 | EVET | asagidan tek yon |
| l2 | EVET | asagidan tek yon |
| l4 | HAYIR | yon asagi dogru |

## §3 Aileler

| # | Aile | Sorumluluk |
| --- | --- | --- |
| 1 | LoopPolicy | Dongu politikasi tanimi |
| 2 | StepBudget | Adim butcesi sayaci |
| 3 | StopCondition | Durma kosulu kurallari |
| 4 | ParallelPolicy | Paralellik siniri |
| 5 | DepthLimit | Derinlik limiti |
| 6 | PauseSignal | Duraklatma sinyali |
| 7 | ProgressReport | Ilerleme raporu |
| 8 | LoopMetrics | Dongu metrikleri |
| 9 | Interruption | Kesinti isareti |
| 10 | LoopGuard | Koruyucu kisit |

## §4 Ornek Kayit (seri 120)

| Kayit | Aile | Alan1 | Alan2 |
| --- | --- | --- | --- |
| 1 | LoopPolicy | butce | 30 adim |
| 2 | LoopPolicy | durma | hedef tamam |
| 3 | StepBudget | derinlik 3 | limit |
| 4 | StepBudget | paralel 2 | sinir |
| 5 | StopCondition | duraklat | sinyal |
| 6 | ParallelPolicy | ilerleme | 50% |

## §5 Yon Ozeti

| Katman | Izin | Yon |
| --- | --- | --- |
| l0 | EVET | asagidan tek yon |
| l1 | EVET | asagidan tek yon |
| l2 | EVET | asagidan tek yon |
| l4 | HAYIR | yon asagi dogru |

## §6 Testler

| Test | Aile | Dogruluk |
| --- | --- | --- |
| ButceAsimiTesti | LoopPolicy | durulan |
| DurmaKosuluTesti | LoopPolicy | durulan |
| DerinlikLimitiTesti | DepthLimit | durulan |
| KesintiTesti | DepthLimit | guvenli durus |

## §7 Riskler

| Risk | Etki | Onlem |
| --- | --- | --- |
| Sonsuz dongu | tuketim | butce + derinlik |
| Yarim is | tutarsiz durum | guvenli durus |
| Rapor eksikligi | kore is | her adimda rapor |

## §8 Disbaglanti

| Dis Kaynak | Yon | Not |
| --- | --- | --- |
| ADR-028 | - | VersaCoder.Protocol baz projeleri |
| ADR-013 | - | 52 dugum |
| layer-matrix.md | referans | yon matrisi |
| N19-logging | - |

## §9 Kanit

| Kanit | Durum |
| --- | --- |
| Klasor | .ai/architecture/catalog/ (duz) |
| Dosya | N36-loop-policy.md |
| Klasor ornegi | C:\versacoder\src\VersaCoder.Protocol |
| Ornek kayit | 6/6 hedef (seri 120 icin) |
| Test | 4/4 hedef |

kayit tablosu §10'e port edildi (kaynak vault 53 dosya / 5440 kayit); esik 5000 karsilanir.


## §10 Kayit Tablosu (ADR-013 2.2 - kaynak vault port)

| N36-001 | `ILoopPolicy` | interface | LoopPolicy sozlesmesi (port) | VersaCoder.Protocol | net48;net10.0-windows | LoopPolicyTests | planli |
| N36-002 | `LoopPolicy` | class | LoopPolicy cekirdek uygulamasi | VersaCoder.Protocol | net48;net10.0-windows | LoopPolicyTests | planli |
| N36-003 | `LoopPolicyDto` | record | LoopPolicy veri transferi modeli | VersaCoder.Protocol | net48;net10.0-windows | LoopPolicyTests | planli |
| N36-004 | `LoopPolicyMapper` | class | LoopPolicy <-> DTO esleme | VersaCoder.Protocol | net48;net10.0-windows | LoopPolicyTests | planli |
| N36-005 | `LoopPolicyValidator` | class | LoopPolicy dogrulama kurallari | VersaCoder.Protocol | net48;net10.0-windows | LoopPolicyTests | planli |
| N36-006 | `LoopPolicyFactory` | class | LoopPolicy uretimi ve varsayilanlari | VersaCoder.Protocol | net48;net10.0-windows | LoopPolicyTests | planli |
| N36-007 | `LoopPolicyOptions` | record | LoopPolicy yapilandirmasi | VersaCoder.Protocol | net48;net10.0-windows | LoopPolicyTests | planli |
| N36-008 | `LoopPolicyProvider` | class | LoopPolicy saglayici uygulamasi | VersaCoder.Protocol | net48;net10.0-windows | LoopPolicyTests | planli |
| N36-009 | `LoopPolicyRegistry` | class | LoopPolicy kayit/katalog yonetimi | VersaCoder.Protocol | net48;net10.0-windows | LoopPolicyTests | planli |
| N36-010 | `LoopPolicyConverter` | class | LoopPolicy bicim donusumu | VersaCoder.Protocol | net48;net10.0-windows | LoopPolicyTests | planli |
| N36-011 | `LoopPolicyTests` | class | LoopPolicy testleri (xUnit) | VersaCoder.Protocol | net48;net10.0-windows | - | planli |
| N36-012 | `LoopPolicyBuilder` | class | LoopPolicy uretici (builder) | VersaCoder.Protocol | net48;net10.0-windows | LoopPolicyTests | planli |
| N36-013 | `IStepBudget` | interface | StepBudget sozlesmesi (port) | VersaCoder.Protocol | net48;net10.0-windows | StepBudgetTests | planli |
| N36-014 | `StepBudget` | class | StepBudget cekirdek uygulamasi | VersaCoder.Protocol | net48;net10.0-windows | StepBudgetTests | planli |
| N36-015 | `StepBudgetDto` | record | StepBudget veri transferi modeli | VersaCoder.Protocol | net48;net10.0-windows | StepBudgetTests | planli |
| N36-016 | `StepBudgetMapper` | class | StepBudget <-> DTO esleme | VersaCoder.Protocol | net48;net10.0-windows | StepBudgetTests | planli |
| N36-017 | `StepBudgetValidator` | class | StepBudget dogrulama kurallari | VersaCoder.Protocol | net48;net10.0-windows | StepBudgetTests | planli |
| N36-018 | `StepBudgetFactory` | class | StepBudget uretimi ve varsayilanlari | VersaCoder.Protocol | net48;net10.0-windows | StepBudgetTests | planli |
| N36-019 | `StepBudgetOptions` | record | StepBudget yapilandirmasi | VersaCoder.Protocol | net48;net10.0-windows | StepBudgetTests | planli |
| N36-020 | `StepBudgetProvider` | class | StepBudget saglayici uygulamasi | VersaCoder.Protocol | net48;net10.0-windows | StepBudgetTests | planli |
| N36-021 | `StepBudgetRegistry` | class | StepBudget kayit/katalog yonetimi | VersaCoder.Protocol | net48;net10.0-windows | StepBudgetTests | planli |
| N36-022 | `StepBudgetConverter` | class | StepBudget bicim donusumu | VersaCoder.Protocol | net48;net10.0-windows | StepBudgetTests | planli |
| N36-023 | `StepBudgetTests` | class | StepBudget testleri (xUnit) | VersaCoder.Protocol | net48;net10.0-windows | - | planli |
| N36-024 | `StepBudgetBuilder` | class | StepBudget uretici (builder) | VersaCoder.Protocol | net48;net10.0-windows | StepBudgetTests | planli |
| N36-025 | `IStopCondition` | interface | StopCondition sozlesmesi (port) | VersaCoder.Protocol | net48;net10.0-windows | StopConditionTests | planli |
| N36-026 | `StopCondition` | class | StopCondition cekirdek uygulamasi | VersaCoder.Protocol | net48;net10.0-windows | StopConditionTests | planli |
| N36-027 | `StopConditionDto` | record | StopCondition veri transferi modeli | VersaCoder.Protocol | net48;net10.0-windows | StopConditionTests | planli |
| N36-028 | `StopConditionMapper` | class | StopCondition <-> DTO esleme | VersaCoder.Protocol | net48;net10.0-windows | StopConditionTests | planli |
| N36-029 | `StopConditionValidator` | class | StopCondition dogrulama kurallari | VersaCoder.Protocol | net48;net10.0-windows | StopConditionTests | planli |
| N36-030 | `StopConditionFactory` | class | StopCondition uretimi ve varsayilanlari | VersaCoder.Protocol | net48;net10.0-windows | StopConditionTests | planli |
| N36-031 | `StopConditionOptions` | record | StopCondition yapilandirmasi | VersaCoder.Protocol | net48;net10.0-windows | StopConditionTests | planli |
| N36-032 | `StopConditionProvider` | class | StopCondition saglayici uygulamasi | VersaCoder.Protocol | net48;net10.0-windows | StopConditionTests | planli |
| N36-033 | `StopConditionRegistry` | class | StopCondition kayit/katalog yonetimi | VersaCoder.Protocol | net48;net10.0-windows | StopConditionTests | planli |
| N36-034 | `StopConditionConverter` | class | StopCondition bicim donusumu | VersaCoder.Protocol | net48;net10.0-windows | StopConditionTests | planli |
| N36-035 | `StopConditionTests` | class | StopCondition testleri (xUnit) | VersaCoder.Protocol | net48;net10.0-windows | - | planli |
| N36-036 | `StopConditionBuilder` | class | StopCondition uretici (builder) | VersaCoder.Protocol | net48;net10.0-windows | StopConditionTests | planli |
| N36-037 | `IParallelPolicy` | interface | ParallelPolicy sozlesmesi (port) | VersaCoder.Protocol | net48;net10.0-windows | ParallelPolicyTests | planli |
| N36-038 | `ParallelPolicy` | class | ParallelPolicy cekirdek uygulamasi | VersaCoder.Protocol | net48;net10.0-windows | ParallelPolicyTests | planli |
| N36-039 | `ParallelPolicyDto` | record | ParallelPolicy veri transferi modeli | VersaCoder.Protocol | net48;net10.0-windows | ParallelPolicyTests | planli |
| N36-040 | `ParallelPolicyMapper` | class | ParallelPolicy <-> DTO esleme | VersaCoder.Protocol | net48;net10.0-windows | ParallelPolicyTests | planli |
| N36-041 | `ParallelPolicyValidator` | class | ParallelPolicy dogrulama kurallari | VersaCoder.Protocol | net48;net10.0-windows | ParallelPolicyTests | planli |
| N36-042 | `ParallelPolicyFactory` | class | ParallelPolicy uretimi ve varsayilanlari | VersaCoder.Protocol | net48;net10.0-windows | ParallelPolicyTests | planli |
| N36-043 | `ParallelPolicyOptions` | record | ParallelPolicy yapilandirmasi | VersaCoder.Protocol | net48;net10.0-windows | ParallelPolicyTests | planli |
| N36-044 | `ParallelPolicyProvider` | class | ParallelPolicy saglayici uygulamasi | VersaCoder.Protocol | net48;net10.0-windows | ParallelPolicyTests | planli |
| N36-045 | `ParallelPolicyRegistry` | class | ParallelPolicy kayit/katalog yonetimi | VersaCoder.Protocol | net48;net10.0-windows | ParallelPolicyTests | planli |
| N36-046 | `ParallelPolicyConverter` | class | ParallelPolicy bicim donusumu | VersaCoder.Protocol | net48;net10.0-windows | ParallelPolicyTests | planli |
| N36-047 | `ParallelPolicyTests` | class | ParallelPolicy testleri (xUnit) | VersaCoder.Protocol | net48;net10.0-windows | - | planli |
| N36-048 | `ParallelPolicyBuilder` | class | ParallelPolicy uretici (builder) | VersaCoder.Protocol | net48;net10.0-windows | ParallelPolicyTests | planli |
| N36-049 | `IDepthLimit` | interface | DepthLimit sozlesmesi (port) | VersaCoder.Protocol | net48;net10.0-windows | DepthLimitTests | planli |
| N36-050 | `DepthLimit` | class | DepthLimit cekirdek uygulamasi | VersaCoder.Protocol | net48;net10.0-windows | DepthLimitTests | planli |
| N36-051 | `DepthLimitDto` | record | DepthLimit veri transferi modeli | VersaCoder.Protocol | net48;net10.0-windows | DepthLimitTests | planli |
| N36-052 | `DepthLimitMapper` | class | DepthLimit <-> DTO esleme | VersaCoder.Protocol | net48;net10.0-windows | DepthLimitTests | planli |
| N36-053 | `DepthLimitValidator` | class | DepthLimit dogrulama kurallari | VersaCoder.Protocol | net48;net10.0-windows | DepthLimitTests | planli |
| N36-054 | `DepthLimitFactory` | class | DepthLimit uretimi ve varsayilanlari | VersaCoder.Protocol | net48;net10.0-windows | DepthLimitTests | planli |
| N36-055 | `DepthLimitOptions` | record | DepthLimit yapilandirmasi | VersaCoder.Protocol | net48;net10.0-windows | DepthLimitTests | planli |
| N36-056 | `DepthLimitProvider` | class | DepthLimit saglayici uygulamasi | VersaCoder.Protocol | net48;net10.0-windows | DepthLimitTests | planli |
| N36-057 | `DepthLimitRegistry` | class | DepthLimit kayit/katalog yonetimi | VersaCoder.Protocol | net48;net10.0-windows | DepthLimitTests | planli |
| N36-058 | `DepthLimitConverter` | class | DepthLimit bicim donusumu | VersaCoder.Protocol | net48;net10.0-windows | DepthLimitTests | planli |
| N36-059 | `DepthLimitTests` | class | DepthLimit testleri (xUnit) | VersaCoder.Protocol | net48;net10.0-windows | - | planli |
| N36-060 | `DepthLimitBuilder` | class | DepthLimit uretici (builder) | VersaCoder.Protocol | net48;net10.0-windows | DepthLimitTests | planli |
| N36-061 | `IPauseSignal` | interface | PauseSignal sozlesmesi (port) | VersaCoder.Protocol | net48;net10.0-windows | PauseSignalTests | planli |
| N36-062 | `PauseSignal` | class | PauseSignal cekirdek uygulamasi | VersaCoder.Protocol | net48;net10.0-windows | PauseSignalTests | planli |
| N36-063 | `PauseSignalDto` | record | PauseSignal veri transferi modeli | VersaCoder.Protocol | net48;net10.0-windows | PauseSignalTests | planli |
| N36-064 | `PauseSignalMapper` | class | PauseSignal <-> DTO esleme | VersaCoder.Protocol | net48;net10.0-windows | PauseSignalTests | planli |
| N36-065 | `PauseSignalValidator` | class | PauseSignal dogrulama kurallari | VersaCoder.Protocol | net48;net10.0-windows | PauseSignalTests | planli |
| N36-066 | `PauseSignalFactory` | class | PauseSignal uretimi ve varsayilanlari | VersaCoder.Protocol | net48;net10.0-windows | PauseSignalTests | planli |
| N36-067 | `PauseSignalOptions` | record | PauseSignal yapilandirmasi | VersaCoder.Protocol | net48;net10.0-windows | PauseSignalTests | planli |
| N36-068 | `PauseSignalProvider` | class | PauseSignal saglayici uygulamasi | VersaCoder.Protocol | net48;net10.0-windows | PauseSignalTests | planli |
| N36-069 | `PauseSignalRegistry` | class | PauseSignal kayit/katalog yonetimi | VersaCoder.Protocol | net48;net10.0-windows | PauseSignalTests | planli |
| N36-070 | `PauseSignalConverter` | class | PauseSignal bicim donusumu | VersaCoder.Protocol | net48;net10.0-windows | PauseSignalTests | planli |
| N36-071 | `PauseSignalTests` | class | PauseSignal testleri (xUnit) | VersaCoder.Protocol | net48;net10.0-windows | - | planli |
| N36-072 | `PauseSignalBuilder` | class | PauseSignal uretici (builder) | VersaCoder.Protocol | net48;net10.0-windows | PauseSignalTests | planli |
| N36-073 | `IProgressReport` | interface | ProgressReport sozlesmesi (port) | VersaCoder.Protocol | net48;net10.0-windows | ProgressReportTests | planli |
| N36-074 | `ProgressReport` | class | ProgressReport cekirdek uygulamasi | VersaCoder.Protocol | net48;net10.0-windows | ProgressReportTests | planli |
| N36-075 | `ProgressReportDto` | record | ProgressReport veri transferi modeli | VersaCoder.Protocol | net48;net10.0-windows | ProgressReportTests | planli |
| N36-076 | `ProgressReportMapper` | class | ProgressReport <-> DTO esleme | VersaCoder.Protocol | net48;net10.0-windows | ProgressReportTests | planli |
| N36-077 | `ProgressReportValidator` | class | ProgressReport dogrulama kurallari | VersaCoder.Protocol | net48;net10.0-windows | ProgressReportTests | planli |
| N36-078 | `ProgressReportFactory` | class | ProgressReport uretimi ve varsayilanlari | VersaCoder.Protocol | net48;net10.0-windows | ProgressReportTests | planli |
| N36-079 | `ProgressReportOptions` | record | ProgressReport yapilandirmasi | VersaCoder.Protocol | net48;net10.0-windows | ProgressReportTests | planli |
| N36-080 | `ProgressReportProvider` | class | ProgressReport saglayici uygulamasi | VersaCoder.Protocol | net48;net10.0-windows | ProgressReportTests | planli |
| N36-081 | `ProgressReportRegistry` | class | ProgressReport kayit/katalog yonetimi | VersaCoder.Protocol | net48;net10.0-windows | ProgressReportTests | planli |
| N36-082 | `ProgressReportConverter` | class | ProgressReport bicim donusumu | VersaCoder.Protocol | net48;net10.0-windows | ProgressReportTests | planli |
| N36-083 | `ProgressReportTests` | class | ProgressReport testleri (xUnit) | VersaCoder.Protocol | net48;net10.0-windows | - | planli |
| N36-084 | `ProgressReportBuilder` | class | ProgressReport uretici (builder) | VersaCoder.Protocol | net48;net10.0-windows | ProgressReportTests | planli |
| N36-085 | `ILoopMetrics` | interface | LoopMetrics sozlesmesi (port) | VersaCoder.Protocol | net48;net10.0-windows | LoopMetricsTests | planli |
| N36-086 | `LoopMetrics` | class | LoopMetrics cekirdek uygulamasi | VersaCoder.Protocol | net48;net10.0-windows | LoopMetricsTests | planli |
| N36-087 | `LoopMetricsDto` | record | LoopMetrics veri transferi modeli | VersaCoder.Protocol | net48;net10.0-windows | LoopMetricsTests | planli |
| N36-088 | `LoopMetricsMapper` | class | LoopMetrics <-> DTO esleme | VersaCoder.Protocol | net48;net10.0-windows | LoopMetricsTests | planli |
| N36-089 | `LoopMetricsValidator` | class | LoopMetrics dogrulama kurallari | VersaCoder.Protocol | net48;net10.0-windows | LoopMetricsTests | planli |
| N36-090 | `LoopMetricsFactory` | class | LoopMetrics uretimi ve varsayilanlari | VersaCoder.Protocol | net48;net10.0-windows | LoopMetricsTests | planli |
| N36-091 | `LoopMetricsOptions` | record | LoopMetrics yapilandirmasi | VersaCoder.Protocol | net48;net10.0-windows | LoopMetricsTests | planli |
| N36-092 | `LoopMetricsProvider` | class | LoopMetrics saglayici uygulamasi | VersaCoder.Protocol | net48;net10.0-windows | LoopMetricsTests | planli |
| N36-093 | `LoopMetricsRegistry` | class | LoopMetrics kayit/katalog yonetimi | VersaCoder.Protocol | net48;net10.0-windows | LoopMetricsTests | planli |
| N36-094 | `LoopMetricsConverter` | class | LoopMetrics bicim donusumu | VersaCoder.Protocol | net48;net10.0-windows | LoopMetricsTests | planli |
| N36-095 | `LoopMetricsTests` | class | LoopMetrics testleri (xUnit) | VersaCoder.Protocol | net48;net10.0-windows | - | planli |
| N36-096 | `LoopMetricsBuilder` | class | LoopMetrics uretici (builder) | VersaCoder.Protocol | net48;net10.0-windows | LoopMetricsTests | planli |
| N36-097 | `IInterruption` | interface | Interruption sozlesmesi (port) | VersaCoder.Protocol | net48;net10.0-windows | InterruptionTests | planli |
| N36-098 | `Interruption` | class | Interruption cekirdek uygulamasi | VersaCoder.Protocol | net48;net10.0-windows | InterruptionTests | planli |
| N36-099 | `InterruptionDto` | record | Interruption veri transferi modeli | VersaCoder.Protocol | net48;net10.0-windows | InterruptionTests | planli |
| N36-100 | `InterruptionMapper` | class | Interruption <-> DTO esleme | VersaCoder.Protocol | net48;net10.0-windows | InterruptionTests | planli |
| N36-101 | `InterruptionValidator` | class | Interruption dogrulama kurallari | VersaCoder.Protocol | net48;net10.0-windows | InterruptionTests | planli |
| N36-102 | `InterruptionFactory` | class | Interruption uretimi ve varsayilanlari | VersaCoder.Protocol | net48;net10.0-windows | InterruptionTests | planli |
| N36-103 | `InterruptionOptions` | record | Interruption yapilandirmasi | VersaCoder.Protocol | net48;net10.0-windows | InterruptionTests | planli |
| N36-104 | `InterruptionProvider` | class | Interruption saglayici uygulamasi | VersaCoder.Protocol | net48;net10.0-windows | InterruptionTests | planli |
| N36-105 | `InterruptionRegistry` | class | Interruption kayit/katalog yonetimi | VersaCoder.Protocol | net48;net10.0-windows | InterruptionTests | planli |
| N36-106 | `InterruptionConverter` | class | Interruption bicim donusumu | VersaCoder.Protocol | net48;net10.0-windows | InterruptionTests | planli |
| N36-107 | `InterruptionTests` | class | Interruption testleri (xUnit) | VersaCoder.Protocol | net48;net10.0-windows | - | planli |
| N36-108 | `InterruptionBuilder` | class | Interruption uretici (builder) | VersaCoder.Protocol | net48;net10.0-windows | InterruptionTests | planli |
| N36-109 | `ILoopGuard` | interface | LoopGuard sozlesmesi (port) | VersaCoder.Protocol | net48;net10.0-windows | LoopGuardTests | planli |
| N36-110 | `LoopGuard` | class | LoopGuard cekirdek uygulamasi | VersaCoder.Protocol | net48;net10.0-windows | LoopGuardTests | planli |
| N36-111 | `LoopGuardDto` | record | LoopGuard veri transferi modeli | VersaCoder.Protocol | net48;net10.0-windows | LoopGuardTests | planli |
| N36-112 | `LoopGuardMapper` | class | LoopGuard <-> DTO esleme | VersaCoder.Protocol | net48;net10.0-windows | LoopGuardTests | planli |
| N36-113 | `LoopGuardValidator` | class | LoopGuard dogrulama kurallari | VersaCoder.Protocol | net48;net10.0-windows | LoopGuardTests | planli |
| N36-114 | `LoopGuardFactory` | class | LoopGuard uretimi ve varsayilanlari | VersaCoder.Protocol | net48;net10.0-windows | LoopGuardTests | planli |
| N36-115 | `LoopGuardOptions` | record | LoopGuard yapilandirmasi | VersaCoder.Protocol | net48;net10.0-windows | LoopGuardTests | planli |
| N36-116 | `LoopGuardProvider` | class | LoopGuard saglayici uygulamasi | VersaCoder.Protocol | net48;net10.0-windows | LoopGuardTests | planli |
| N36-117 | `LoopGuardRegistry` | class | LoopGuard kayit/katalog yonetimi | VersaCoder.Protocol | net48;net10.0-windows | LoopGuardTests | planli |
| N36-118 | `LoopGuardConverter` | class | LoopGuard bicim donusumu | VersaCoder.Protocol | net48;net10.0-windows | LoopGuardTests | planli |
| N36-119 | `LoopGuardTests` | class | LoopGuard testleri (xUnit) | VersaCoder.Protocol | net48;net10.0-windows | - | planli |
| N36-120 | `LoopGuardBuilder` | class | LoopGuard uretici (builder) | VersaCoder.Protocol | net48;net10.0-windows | LoopGuardTests | planli |
