---
title: Katalog Dugumu N38 - Baslangic Boru Hatti
type: catalog-node
category: l6-host
status: needs-validation
version: 1.0.0
updated: 2026-10-04
authority: derived
scope: calisma zamani baslangic sirasi ve on kontroller
---

# Katalog Dugumu N38 - Baslangic Boru Hatti

**Zorunlu Baglantilar:** index.md | node-inventory.md | ../index.md | ../../.decisions/accepted/ADR-028-layered-baseline-9-projects.md | ../../.decisions/accepted/ADR-013-layered-architecture-52-nodes.md

> Dugum, katman sozlesmesini cevaplar ve kanitini baglar.

## §0 Ozet (Answer First)

| Alan | Deger |
| --- | --- |
| Katman | l6 Host |
| Proje | VersaCoder.Host |
| Kapsam | calisma zamani baslangic sirasi ve on kontroller |
| Kaynak | C:/www/versacoder/versacoder/.ai/architecture/catalog/l6-host/N38-bootstrap-pipeline.md |
| Sorumluluk | Dugum, katman sozlesmesini cevaplar ve kanitini baglar. |

- Baslangic boru hatti adimlari onayli sirada calistirip calisma zamanini ayaga kaldirir.
- On kontroller (preflight) hizli ve bagimsiz basar, saglik kapisi gecilmeden is baslamaz.
- Isinma gorevleri arka planda calisir, kritik olanlar kapida bekler.
- Sessiz basarisizliklar yasaktir; her adim rapora yazilir, hata yukari tasinir.
- Kayitlar en az 6 aile 120 ornek ile kanitlanir, gerisi namludur (kayit yok).

## §1 YAPAR / YAPMAZ

| YAPAR | YAPMAZ |
| --- | --- |
| Baslangic sirasini ve on kontrolleri uretir | Adimlari kendisi calistirir (l4 is) |
| | l0 + l1 + l2 + l4 + l5 (l3 kabul) | - |
| | baslangic, DI kayitlari ve yapilandirma koprusu | - |

## §2 Bagimlilik Yonu

| Yon | Durum | Aciklama |
| --- | --- | --- |
| l0 | EVET | asagidan tek yon |
| l1 | EVET | asagidan tek yon |
| l2 | EVET | asagidan tek yon |
| l4 | EVET | asagidan tek yon |
| l5 | EVET | asagidan tek yon |
| l3 + l7 | HAYIR | yon asagi dogru |

## §3 Aileler

| # | Aile | Sorumluluk |
| --- | --- | --- |
| 1 | StartupPipeline | Baslangic boru hatti motoru |
| 2 | StartupStep | Tek baslangic adimi |
| 3 | StartupReport | Baslangic raporu |
| 4 | WarmupTask | Isinma gorevi |
| 5 | OrderValidator | Sira dogrulayici |
| 6 | HealthGate | Saglik kapisi |
| 7 | SilentFailure | Sessiz basarisizlik kaydi |
| 8 | BootLog | Acilis kaydi |
| 9 | Preflight | On kontrol paketi |
| 10 | ShutdownHook | Kapanis kancasi |

## §4 Ornek Kayit (seri 120)

| Kayit | Aile | Alan1 | Alan2 |
| --- | --- | --- | --- |
| 1 | StartupPipeline | pipeline | 5 adim |
| 2 | StartupPipeline | preflight | gecen |
| 3 | StartupStep | healthGate | acik |
| 4 | StartupStep | warmup | arka plan |
| 5 | StartupReport | bootLog | yazildi |
| 6 | WarmupTask | shutdownHook | kayitli |

## §5 Yon Ozeti

| Katman | Izin | Yon |
| --- | --- | --- |
| l0 | EVET | asagidan tek yon |
| l1 | EVET | asagidan tek yon |
| l2 | EVET | asagidan tek yon |
| l4 | EVET | asagidan tek yon |
| l5 | EVET | asagidan tek yon |
| l3 + l7 | HAYIR | yon asagi dogru |

## §6 Testler

| Test | Aile | Dogruluk |
| --- | --- | --- |
| SiraDogrulamaTesti | StartupPipeline | dogru sira |
| PreflightTesti | StartupPipeline | gecen |
| SaglikKapisiTesti | OrderValidator | kapali |
| SessizBasarisizlikTesti | OrderValidator | yakalanan |

## §7 Riskler

| Risk | Etki | Onlem |
| --- | --- | --- |
| Yanlis sira | basarisiz acilis | order validator |
| Sessiz hata | bilinmeyen durum | rapor zorunlu |
| Bekleme tasmasi | yavas acilis | kapi esigi |

## §8 Disbaglanti

| Dis Kaynak | Yon | Not |
| --- | --- | --- |
| ADR-028 | - | VersaCoder.Host baz projeleri |
| ADR-013 | - | 52 dugum |
| layer-matrix.md | referans | yon matrisi |
| N19-logging | - |

## §9 Kanit

| Kanit | Durum |
| --- | --- |
| Klasor | .ai/architecture/catalog/ (duz) |
| Dosya | N38-bootstrap-pipeline.md |
| Klasor ornegi | C:\versacoder\src\VersaCoder.Host |
| Ornek kayit | 6/6 hedef (seri 120 icin) |
| Test | 4/4 hedef |

kayit tablosu §10'e port edildi (kaynak vault 53 dosya / 5440 kayit); esik 5000 karsilanir.


## §10 Kayit Tablosu (ADR-013 2.2 - kaynak vault port)

| N38-001 | `IStartupPipeline` | interface | StartupPipeline sozlesmesi (port) | VersaCoder.Host | net48;net10.0-windows | StartupPipelineTests | planli |
| N38-002 | `StartupPipeline` | class | StartupPipeline cekirdek uygulamasi | VersaCoder.Host | net48;net10.0-windows | StartupPipelineTests | planli |
| N38-003 | `StartupPipelineDto` | record | StartupPipeline veri transferi modeli | VersaCoder.Host | net48;net10.0-windows | StartupPipelineTests | planli |
| N38-004 | `StartupPipelineMapper` | class | StartupPipeline <-> DTO esleme | VersaCoder.Host | net48;net10.0-windows | StartupPipelineTests | planli |
| N38-005 | `StartupPipelineValidator` | class | StartupPipeline dogrulama kurallari | VersaCoder.Host | net48;net10.0-windows | StartupPipelineTests | planli |
| N38-006 | `StartupPipelineFactory` | class | StartupPipeline uretimi ve varsayilanlari | VersaCoder.Host | net48;net10.0-windows | StartupPipelineTests | planli |
| N38-007 | `StartupPipelineOptions` | record | StartupPipeline yapilandirmasi | VersaCoder.Host | net48;net10.0-windows | StartupPipelineTests | planli |
| N38-008 | `StartupPipelineProvider` | class | StartupPipeline saglayici uygulamasi | VersaCoder.Host | net48;net10.0-windows | StartupPipelineTests | planli |
| N38-009 | `StartupPipelineRegistry` | class | StartupPipeline kayit/katalog yonetimi | VersaCoder.Host | net48;net10.0-windows | StartupPipelineTests | planli |
| N38-010 | `StartupPipelineConverter` | class | StartupPipeline bicim donusumu | VersaCoder.Host | net48;net10.0-windows | StartupPipelineTests | planli |
| N38-011 | `StartupPipelineTests` | class | StartupPipeline testleri (xUnit) | VersaCoder.Host | net48;net10.0-windows | - | planli |
| N38-012 | `StartupPipelineBuilder` | class | StartupPipeline uretici (builder) | VersaCoder.Host | net48;net10.0-windows | StartupPipelineTests | planli |
| N38-013 | `IStartupStep` | interface | StartupStep sozlesmesi (port) | VersaCoder.Host | net48;net10.0-windows | StartupStepTests | planli |
| N38-014 | `StartupStep` | class | StartupStep cekirdek uygulamasi | VersaCoder.Host | net48;net10.0-windows | StartupStepTests | planli |
| N38-015 | `StartupStepDto` | record | StartupStep veri transferi modeli | VersaCoder.Host | net48;net10.0-windows | StartupStepTests | planli |
| N38-016 | `StartupStepMapper` | class | StartupStep <-> DTO esleme | VersaCoder.Host | net48;net10.0-windows | StartupStepTests | planli |
| N38-017 | `StartupStepValidator` | class | StartupStep dogrulama kurallari | VersaCoder.Host | net48;net10.0-windows | StartupStepTests | planli |
| N38-018 | `StartupStepFactory` | class | StartupStep uretimi ve varsayilanlari | VersaCoder.Host | net48;net10.0-windows | StartupStepTests | planli |
| N38-019 | `StartupStepOptions` | record | StartupStep yapilandirmasi | VersaCoder.Host | net48;net10.0-windows | StartupStepTests | planli |
| N38-020 | `StartupStepProvider` | class | StartupStep saglayici uygulamasi | VersaCoder.Host | net48;net10.0-windows | StartupStepTests | planli |
| N38-021 | `StartupStepRegistry` | class | StartupStep kayit/katalog yonetimi | VersaCoder.Host | net48;net10.0-windows | StartupStepTests | planli |
| N38-022 | `StartupStepConverter` | class | StartupStep bicim donusumu | VersaCoder.Host | net48;net10.0-windows | StartupStepTests | planli |
| N38-023 | `StartupStepTests` | class | StartupStep testleri (xUnit) | VersaCoder.Host | net48;net10.0-windows | - | planli |
| N38-024 | `StartupStepBuilder` | class | StartupStep uretici (builder) | VersaCoder.Host | net48;net10.0-windows | StartupStepTests | planli |
| N38-025 | `IStartupReport` | interface | StartupReport sozlesmesi (port) | VersaCoder.Host | net48;net10.0-windows | StartupReportTests | planli |
| N38-026 | `StartupReport` | class | StartupReport cekirdek uygulamasi | VersaCoder.Host | net48;net10.0-windows | StartupReportTests | planli |
| N38-027 | `StartupReportDto` | record | StartupReport veri transferi modeli | VersaCoder.Host | net48;net10.0-windows | StartupReportTests | planli |
| N38-028 | `StartupReportMapper` | class | StartupReport <-> DTO esleme | VersaCoder.Host | net48;net10.0-windows | StartupReportTests | planli |
| N38-029 | `StartupReportValidator` | class | StartupReport dogrulama kurallari | VersaCoder.Host | net48;net10.0-windows | StartupReportTests | planli |
| N38-030 | `StartupReportFactory` | class | StartupReport uretimi ve varsayilanlari | VersaCoder.Host | net48;net10.0-windows | StartupReportTests | planli |
| N38-031 | `StartupReportOptions` | record | StartupReport yapilandirmasi | VersaCoder.Host | net48;net10.0-windows | StartupReportTests | planli |
| N38-032 | `StartupReportProvider` | class | StartupReport saglayici uygulamasi | VersaCoder.Host | net48;net10.0-windows | StartupReportTests | planli |
| N38-033 | `StartupReportRegistry` | class | StartupReport kayit/katalog yonetimi | VersaCoder.Host | net48;net10.0-windows | StartupReportTests | planli |
| N38-034 | `StartupReportConverter` | class | StartupReport bicim donusumu | VersaCoder.Host | net48;net10.0-windows | StartupReportTests | planli |
| N38-035 | `StartupReportTests` | class | StartupReport testleri (xUnit) | VersaCoder.Host | net48;net10.0-windows | - | planli |
| N38-036 | `StartupReportBuilder` | class | StartupReport uretici (builder) | VersaCoder.Host | net48;net10.0-windows | StartupReportTests | planli |
| N38-037 | `IWarmupTask` | interface | WarmupTask sozlesmesi (port) | VersaCoder.Host | net48;net10.0-windows | WarmupTaskTests | planli |
| N38-038 | `WarmupTask` | class | WarmupTask cekirdek uygulamasi | VersaCoder.Host | net48;net10.0-windows | WarmupTaskTests | planli |
| N38-039 | `WarmupTaskDto` | record | WarmupTask veri transferi modeli | VersaCoder.Host | net48;net10.0-windows | WarmupTaskTests | planli |
| N38-040 | `WarmupTaskMapper` | class | WarmupTask <-> DTO esleme | VersaCoder.Host | net48;net10.0-windows | WarmupTaskTests | planli |
| N38-041 | `WarmupTaskValidator` | class | WarmupTask dogrulama kurallari | VersaCoder.Host | net48;net10.0-windows | WarmupTaskTests | planli |
| N38-042 | `WarmupTaskFactory` | class | WarmupTask uretimi ve varsayilanlari | VersaCoder.Host | net48;net10.0-windows | WarmupTaskTests | planli |
| N38-043 | `WarmupTaskOptions` | record | WarmupTask yapilandirmasi | VersaCoder.Host | net48;net10.0-windows | WarmupTaskTests | planli |
| N38-044 | `WarmupTaskProvider` | class | WarmupTask saglayici uygulamasi | VersaCoder.Host | net48;net10.0-windows | WarmupTaskTests | planli |
| N38-045 | `WarmupTaskRegistry` | class | WarmupTask kayit/katalog yonetimi | VersaCoder.Host | net48;net10.0-windows | WarmupTaskTests | planli |
| N38-046 | `WarmupTaskConverter` | class | WarmupTask bicim donusumu | VersaCoder.Host | net48;net10.0-windows | WarmupTaskTests | planli |
| N38-047 | `WarmupTaskTests` | class | WarmupTask testleri (xUnit) | VersaCoder.Host | net48;net10.0-windows | - | planli |
| N38-048 | `WarmupTaskBuilder` | class | WarmupTask uretici (builder) | VersaCoder.Host | net48;net10.0-windows | WarmupTaskTests | planli |
| N38-049 | `IOrderValidator` | interface | OrderValidator sozlesmesi (port) | VersaCoder.Host | net48;net10.0-windows | OrderValidatorTests | planli |
| N38-050 | `OrderValidator` | class | OrderValidator cekirdek uygulamasi | VersaCoder.Host | net48;net10.0-windows | OrderValidatorTests | planli |
| N38-051 | `OrderValidatorDto` | record | OrderValidator veri transferi modeli | VersaCoder.Host | net48;net10.0-windows | OrderValidatorTests | planli |
| N38-052 | `OrderValidatorMapper` | class | OrderValidator <-> DTO esleme | VersaCoder.Host | net48;net10.0-windows | OrderValidatorTests | planli |
| N38-053 | `OrderValidatorValidator` | class | OrderValidator dogrulama kurallari | VersaCoder.Host | net48;net10.0-windows | OrderValidatorTests | planli |
| N38-054 | `OrderValidatorFactory` | class | OrderValidator uretimi ve varsayilanlari | VersaCoder.Host | net48;net10.0-windows | OrderValidatorTests | planli |
| N38-055 | `OrderValidatorOptions` | record | OrderValidator yapilandirmasi | VersaCoder.Host | net48;net10.0-windows | OrderValidatorTests | planli |
| N38-056 | `OrderValidatorProvider` | class | OrderValidator saglayici uygulamasi | VersaCoder.Host | net48;net10.0-windows | OrderValidatorTests | planli |
| N38-057 | `OrderValidatorRegistry` | class | OrderValidator kayit/katalog yonetimi | VersaCoder.Host | net48;net10.0-windows | OrderValidatorTests | planli |
| N38-058 | `OrderValidatorConverter` | class | OrderValidator bicim donusumu | VersaCoder.Host | net48;net10.0-windows | OrderValidatorTests | planli |
| N38-059 | `OrderValidatorTests` | class | OrderValidator testleri (xUnit) | VersaCoder.Host | net48;net10.0-windows | - | planli |
| N38-060 | `OrderValidatorBuilder` | class | OrderValidator uretici (builder) | VersaCoder.Host | net48;net10.0-windows | OrderValidatorTests | planli |
| N38-061 | `IHealthGate` | interface | HealthGate sozlesmesi (port) | VersaCoder.Host | net48;net10.0-windows | HealthGateTests | planli |
| N38-062 | `HealthGate` | class | HealthGate cekirdek uygulamasi | VersaCoder.Host | net48;net10.0-windows | HealthGateTests | planli |
| N38-063 | `HealthGateDto` | record | HealthGate veri transferi modeli | VersaCoder.Host | net48;net10.0-windows | HealthGateTests | planli |
| N38-064 | `HealthGateMapper` | class | HealthGate <-> DTO esleme | VersaCoder.Host | net48;net10.0-windows | HealthGateTests | planli |
| N38-065 | `HealthGateValidator` | class | HealthGate dogrulama kurallari | VersaCoder.Host | net48;net10.0-windows | HealthGateTests | planli |
| N38-066 | `HealthGateFactory` | class | HealthGate uretimi ve varsayilanlari | VersaCoder.Host | net48;net10.0-windows | HealthGateTests | planli |
| N38-067 | `HealthGateOptions` | record | HealthGate yapilandirmasi | VersaCoder.Host | net48;net10.0-windows | HealthGateTests | planli |
| N38-068 | `HealthGateProvider` | class | HealthGate saglayici uygulamasi | VersaCoder.Host | net48;net10.0-windows | HealthGateTests | planli |
| N38-069 | `HealthGateRegistry` | class | HealthGate kayit/katalog yonetimi | VersaCoder.Host | net48;net10.0-windows | HealthGateTests | planli |
| N38-070 | `HealthGateConverter` | class | HealthGate bicim donusumu | VersaCoder.Host | net48;net10.0-windows | HealthGateTests | planli |
| N38-071 | `HealthGateTests` | class | HealthGate testleri (xUnit) | VersaCoder.Host | net48;net10.0-windows | - | planli |
| N38-072 | `HealthGateBuilder` | class | HealthGate uretici (builder) | VersaCoder.Host | net48;net10.0-windows | HealthGateTests | planli |
| N38-073 | `ISilentFailure` | interface | SilentFailure sozlesmesi (port) | VersaCoder.Host | net48;net10.0-windows | SilentFailureTests | planli |
| N38-074 | `SilentFailure` | class | SilentFailure cekirdek uygulamasi | VersaCoder.Host | net48;net10.0-windows | SilentFailureTests | planli |
| N38-075 | `SilentFailureDto` | record | SilentFailure veri transferi modeli | VersaCoder.Host | net48;net10.0-windows | SilentFailureTests | planli |
| N38-076 | `SilentFailureMapper` | class | SilentFailure <-> DTO esleme | VersaCoder.Host | net48;net10.0-windows | SilentFailureTests | planli |
| N38-077 | `SilentFailureValidator` | class | SilentFailure dogrulama kurallari | VersaCoder.Host | net48;net10.0-windows | SilentFailureTests | planli |
| N38-078 | `SilentFailureFactory` | class | SilentFailure uretimi ve varsayilanlari | VersaCoder.Host | net48;net10.0-windows | SilentFailureTests | planli |
| N38-079 | `SilentFailureOptions` | record | SilentFailure yapilandirmasi | VersaCoder.Host | net48;net10.0-windows | SilentFailureTests | planli |
| N38-080 | `SilentFailureProvider` | class | SilentFailure saglayici uygulamasi | VersaCoder.Host | net48;net10.0-windows | SilentFailureTests | planli |
| N38-081 | `SilentFailureRegistry` | class | SilentFailure kayit/katalog yonetimi | VersaCoder.Host | net48;net10.0-windows | SilentFailureTests | planli |
| N38-082 | `SilentFailureConverter` | class | SilentFailure bicim donusumu | VersaCoder.Host | net48;net10.0-windows | SilentFailureTests | planli |
| N38-083 | `SilentFailureTests` | class | SilentFailure testleri (xUnit) | VersaCoder.Host | net48;net10.0-windows | - | planli |
| N38-084 | `SilentFailureBuilder` | class | SilentFailure uretici (builder) | VersaCoder.Host | net48;net10.0-windows | SilentFailureTests | planli |
| N38-085 | `IBootLog` | interface | BootLog sozlesmesi (port) | VersaCoder.Host | net48;net10.0-windows | BootLogTests | planli |
| N38-086 | `BootLog` | class | BootLog cekirdek uygulamasi | VersaCoder.Host | net48;net10.0-windows | BootLogTests | planli |
| N38-087 | `BootLogDto` | record | BootLog veri transferi modeli | VersaCoder.Host | net48;net10.0-windows | BootLogTests | planli |
| N38-088 | `BootLogMapper` | class | BootLog <-> DTO esleme | VersaCoder.Host | net48;net10.0-windows | BootLogTests | planli |
| N38-089 | `BootLogValidator` | class | BootLog dogrulama kurallari | VersaCoder.Host | net48;net10.0-windows | BootLogTests | planli |
| N38-090 | `BootLogFactory` | class | BootLog uretimi ve varsayilanlari | VersaCoder.Host | net48;net10.0-windows | BootLogTests | planli |
| N38-091 | `BootLogOptions` | record | BootLog yapilandirmasi | VersaCoder.Host | net48;net10.0-windows | BootLogTests | planli |
| N38-092 | `BootLogProvider` | class | BootLog saglayici uygulamasi | VersaCoder.Host | net48;net10.0-windows | BootLogTests | planli |
| N38-093 | `BootLogRegistry` | class | BootLog kayit/katalog yonetimi | VersaCoder.Host | net48;net10.0-windows | BootLogTests | planli |
| N38-094 | `BootLogConverter` | class | BootLog bicim donusumu | VersaCoder.Host | net48;net10.0-windows | BootLogTests | planli |
| N38-095 | `BootLogTests` | class | BootLog testleri (xUnit) | VersaCoder.Host | net48;net10.0-windows | - | planli |
| N38-096 | `BootLogBuilder` | class | BootLog uretici (builder) | VersaCoder.Host | net48;net10.0-windows | BootLogTests | planli |
| N38-097 | `IPreflight` | interface | Preflight sozlesmesi (port) | VersaCoder.Host | net48;net10.0-windows | PreflightTests | planli |
| N38-098 | `Preflight` | class | Preflight cekirdek uygulamasi | VersaCoder.Host | net48;net10.0-windows | PreflightTests | planli |
| N38-099 | `PreflightDto` | record | Preflight veri transferi modeli | VersaCoder.Host | net48;net10.0-windows | PreflightTests | planli |
| N38-100 | `PreflightMapper` | class | Preflight <-> DTO esleme | VersaCoder.Host | net48;net10.0-windows | PreflightTests | planli |
| N38-101 | `PreflightValidator` | class | Preflight dogrulama kurallari | VersaCoder.Host | net48;net10.0-windows | PreflightTests | planli |
| N38-102 | `PreflightFactory` | class | Preflight uretimi ve varsayilanlari | VersaCoder.Host | net48;net10.0-windows | PreflightTests | planli |
| N38-103 | `PreflightOptions` | record | Preflight yapilandirmasi | VersaCoder.Host | net48;net10.0-windows | PreflightTests | planli |
| N38-104 | `PreflightProvider` | class | Preflight saglayici uygulamasi | VersaCoder.Host | net48;net10.0-windows | PreflightTests | planli |
| N38-105 | `PreflightRegistry` | class | Preflight kayit/katalog yonetimi | VersaCoder.Host | net48;net10.0-windows | PreflightTests | planli |
| N38-106 | `PreflightConverter` | class | Preflight bicim donusumu | VersaCoder.Host | net48;net10.0-windows | PreflightTests | planli |
| N38-107 | `PreflightTests` | class | Preflight testleri (xUnit) | VersaCoder.Host | net48;net10.0-windows | - | planli |
| N38-108 | `PreflightBuilder` | class | Preflight uretici (builder) | VersaCoder.Host | net48;net10.0-windows | PreflightTests | planli |
| N38-109 | `IShutdownHook` | interface | ShutdownHook sozlesmesi (port) | VersaCoder.Host | net48;net10.0-windows | ShutdownHookTests | planli |
| N38-110 | `ShutdownHook` | class | ShutdownHook cekirdek uygulamasi | VersaCoder.Host | net48;net10.0-windows | ShutdownHookTests | planli |
| N38-111 | `ShutdownHookDto` | record | ShutdownHook veri transferi modeli | VersaCoder.Host | net48;net10.0-windows | ShutdownHookTests | planli |
| N38-112 | `ShutdownHookMapper` | class | ShutdownHook <-> DTO esleme | VersaCoder.Host | net48;net10.0-windows | ShutdownHookTests | planli |
| N38-113 | `ShutdownHookValidator` | class | ShutdownHook dogrulama kurallari | VersaCoder.Host | net48;net10.0-windows | ShutdownHookTests | planli |
| N38-114 | `ShutdownHookFactory` | class | ShutdownHook uretimi ve varsayilanlari | VersaCoder.Host | net48;net10.0-windows | ShutdownHookTests | planli |
| N38-115 | `ShutdownHookOptions` | record | ShutdownHook yapilandirmasi | VersaCoder.Host | net48;net10.0-windows | ShutdownHookTests | planli |
| N38-116 | `ShutdownHookProvider` | class | ShutdownHook saglayici uygulamasi | VersaCoder.Host | net48;net10.0-windows | ShutdownHookTests | planli |
| N38-117 | `ShutdownHookRegistry` | class | ShutdownHook kayit/katalog yonetimi | VersaCoder.Host | net48;net10.0-windows | ShutdownHookTests | planli |
| N38-118 | `ShutdownHookConverter` | class | ShutdownHook bicim donusumu | VersaCoder.Host | net48;net10.0-windows | ShutdownHookTests | planli |
| N38-119 | `ShutdownHookTests` | class | ShutdownHook testleri (xUnit) | VersaCoder.Host | net48;net10.0-windows | - | planli |
| N38-120 | `ShutdownHookBuilder` | class | ShutdownHook uretici (builder) | VersaCoder.Host | net48;net10.0-windows | ShutdownHookTests | planli |
