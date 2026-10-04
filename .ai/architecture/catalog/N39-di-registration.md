---
title: Katalog Dugumu N39 - DI Kayitlari
type: catalog-node
category: l6-host
status: needs-validation
version: 1.0.0
updated: 2026-10-04
authority: derived
scope: bagimlilik kapsi kayitlari ve omur haritasi
---

# Katalog Dugumu N39 - DI Kayitlari

**Zorunlu Baglantilar:** index.md | node-inventory.md | ../index.md | ../../.decisions/accepted/ADR-028-layered-baseline-9-projects.md | ../../.decisions/accepted/ADR-013-layered-architecture-52-nodes.md

> Dugum, katman sozlesmesini cevaplar ve kanitini baglar.

## §0 Ozet (Answer First)

| Alan | Deger |
| --- | --- |
| Katman | l6 Host |
| Proje | VersaCoder.Host |
| Kapsam | bagimlilik kapsi kayitlari ve omur haritasi |
| Kaynak | C:/www/versacoder/versacoder/.ai/architecture/catalog/l6-host/N39-di-registration.md |
| Sorumluluk | Dugum, katman sozlesmesini cevaplar ve kanitini baglar. |

- Servis kayitlari modul sirasiyla yapilir, kaynak ve omur (singleton/scoped/transient) haritasi tek yerden okunur.
- Cozucu sonlari saglik probu ile denetlenir, eksik kayit acilista yakalanir.
- Kayit denetimi her modulden sonra rapor uretir, disariya tasinan bagimlilik listelenir.
- Imha sirasi tersine kurulur, kalan sureli kaynaklar guvenli kapatilir.
- Kayitlar en az 6 aile 120 ornek ile kanitlanir, gerisi namludur (kayit yok).

## §1 YAPAR / YAPMAZ

| YAPAR | YAPMAZ |
| --- | --- |
| DI kayitlarini ve omur haritasini uretir | Cozumlemeyi kendisi calistirir (l4 is) |
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
| 1 | ServiceCollectionExtensions | Kayit genisletmeleri |
| 2 | ContainerHealth | Kapsi saglik durumu |
| 3 | LifetimeMap | Omur haritasi |
| 4 | RegistrationSource | Kaynak modul kaydi |
| 5 | ScopeFactory | Kaps uretimi |
| 6 | ResolverProbe | Cozucu probu |
| 7 | RegistrationAudit | Kayit denetimi |
| 8 | DisposalOrder | Imha sirasi |
| 9 | ModuleRegistrar | Modul kayitcisi |
| 10 | TestDoubleRegistrar | Test cifti kayitcisi |

## §4 Ornek Kayit (seri 120)

| Kayit | Aile | Alan1 | Alan2 |
| --- | --- | --- | --- |
| 1 | ServiceCollectionExtensions | transient | servis |
| 2 | ServiceCollectionExtensions | singleton | kayit |
| 3 | ContainerHealth | scoped | istek |
| 4 | ContainerHealth | probe | ok |
| 5 | LifetimeMap | audit | modul A |
| 6 | RegistrationSource | disposal | ters sira |

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
| KayitDenetimTesti | ServiceCollectionExtensions | kayitli |
| CozucuProbTesti | ServiceCollectionExtensions | bulunan |
| ImhaSirasiTesti | ScopeFactory | ters sira |
| TestCiftiKaydiTesti | ScopeFactory | eslesen |

## §7 Riskler

| Risk | Etki | Onlem |
| --- | --- | --- |
| Eksik kayit | acilis hatasi | cozucu probu |
| Yanlis omur | kacak kaynak | omur haritasi |
| Imha hatasi | kilit | ters imha sirasi |

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
| Dosya | N39-di-registration.md |
| Klasor ornegi | C:\versacoder\src\VersaCoder.Host |
| Ornek kayit | 6/6 hedef (seri 120 icin) |
| Test | 4/4 hedef |

kayit tablosu §10'e port edildi (kaynak vault 53 dosya / 5440 kayit); esik 5000 karsilanir.


## §10 Kayit Tablosu (ADR-013 2.2 - kaynak vault port)

| N39-001 | `IServiceCollectionExtensions` | interface | ServiceCollectionExtensions sozlesmesi (port) | VersaCoder.Host | net48;net10.0-windows | ServiceCollectionExtensionsTests | planli |
| N39-002 | `ServiceCollectionExtensions` | class | ServiceCollectionExtensions cekirdek uygulamasi | VersaCoder.Host | net48;net10.0-windows | ServiceCollectionExtensionsTests | planli |
| N39-003 | `ServiceCollectionExtensionsDto` | record | ServiceCollectionExtensions veri transferi modeli | VersaCoder.Host | net48;net10.0-windows | ServiceCollectionExtensionsTests | planli |
| N39-004 | `ServiceCollectionExtensionsMapper` | class | ServiceCollectionExtensions <-> DTO esleme | VersaCoder.Host | net48;net10.0-windows | ServiceCollectionExtensionsTests | planli |
| N39-005 | `ServiceCollectionExtensionsValidator` | class | ServiceCollectionExtensions dogrulama kurallari | VersaCoder.Host | net48;net10.0-windows | ServiceCollectionExtensionsTests | planli |
| N39-006 | `ServiceCollectionExtensionsFactory` | class | ServiceCollectionExtensions uretimi ve varsayilanlari | VersaCoder.Host | net48;net10.0-windows | ServiceCollectionExtensionsTests | planli |
| N39-007 | `ServiceCollectionExtensionsOptions` | record | ServiceCollectionExtensions yapilandirmasi | VersaCoder.Host | net48;net10.0-windows | ServiceCollectionExtensionsTests | planli |
| N39-008 | `ServiceCollectionExtensionsProvider` | class | ServiceCollectionExtensions saglayici uygulamasi | VersaCoder.Host | net48;net10.0-windows | ServiceCollectionExtensionsTests | planli |
| N39-009 | `ServiceCollectionExtensionsRegistry` | class | ServiceCollectionExtensions kayit/katalog yonetimi | VersaCoder.Host | net48;net10.0-windows | ServiceCollectionExtensionsTests | planli |
| N39-010 | `ServiceCollectionExtensionsConverter` | class | ServiceCollectionExtensions bicim donusumu | VersaCoder.Host | net48;net10.0-windows | ServiceCollectionExtensionsTests | planli |
| N39-011 | `ServiceCollectionExtensionsTests` | class | ServiceCollectionExtensions testleri (xUnit) | VersaCoder.Host | net48;net10.0-windows | - | planli |
| N39-012 | `ServiceCollectionExtensionsBuilder` | class | ServiceCollectionExtensions uretici (builder) | VersaCoder.Host | net48;net10.0-windows | ServiceCollectionExtensionsTests | planli |
| N39-013 | `IContainerHealth` | interface | ContainerHealth sozlesmesi (port) | VersaCoder.Host | net48;net10.0-windows | ContainerHealthTests | planli |
| N39-014 | `ContainerHealth` | class | ContainerHealth cekirdek uygulamasi | VersaCoder.Host | net48;net10.0-windows | ContainerHealthTests | planli |
| N39-015 | `ContainerHealthDto` | record | ContainerHealth veri transferi modeli | VersaCoder.Host | net48;net10.0-windows | ContainerHealthTests | planli |
| N39-016 | `ContainerHealthMapper` | class | ContainerHealth <-> DTO esleme | VersaCoder.Host | net48;net10.0-windows | ContainerHealthTests | planli |
| N39-017 | `ContainerHealthValidator` | class | ContainerHealth dogrulama kurallari | VersaCoder.Host | net48;net10.0-windows | ContainerHealthTests | planli |
| N39-018 | `ContainerHealthFactory` | class | ContainerHealth uretimi ve varsayilanlari | VersaCoder.Host | net48;net10.0-windows | ContainerHealthTests | planli |
| N39-019 | `ContainerHealthOptions` | record | ContainerHealth yapilandirmasi | VersaCoder.Host | net48;net10.0-windows | ContainerHealthTests | planli |
| N39-020 | `ContainerHealthProvider` | class | ContainerHealth saglayici uygulamasi | VersaCoder.Host | net48;net10.0-windows | ContainerHealthTests | planli |
| N39-021 | `ContainerHealthRegistry` | class | ContainerHealth kayit/katalog yonetimi | VersaCoder.Host | net48;net10.0-windows | ContainerHealthTests | planli |
| N39-022 | `ContainerHealthConverter` | class | ContainerHealth bicim donusumu | VersaCoder.Host | net48;net10.0-windows | ContainerHealthTests | planli |
| N39-023 | `ContainerHealthTests` | class | ContainerHealth testleri (xUnit) | VersaCoder.Host | net48;net10.0-windows | - | planli |
| N39-024 | `ContainerHealthBuilder` | class | ContainerHealth uretici (builder) | VersaCoder.Host | net48;net10.0-windows | ContainerHealthTests | planli |
| N39-025 | `ILifetimeMap` | interface | LifetimeMap sozlesmesi (port) | VersaCoder.Host | net48;net10.0-windows | LifetimeMapTests | planli |
| N39-026 | `LifetimeMap` | class | LifetimeMap cekirdek uygulamasi | VersaCoder.Host | net48;net10.0-windows | LifetimeMapTests | planli |
| N39-027 | `LifetimeMapDto` | record | LifetimeMap veri transferi modeli | VersaCoder.Host | net48;net10.0-windows | LifetimeMapTests | planli |
| N39-028 | `LifetimeMapMapper` | class | LifetimeMap <-> DTO esleme | VersaCoder.Host | net48;net10.0-windows | LifetimeMapTests | planli |
| N39-029 | `LifetimeMapValidator` | class | LifetimeMap dogrulama kurallari | VersaCoder.Host | net48;net10.0-windows | LifetimeMapTests | planli |
| N39-030 | `LifetimeMapFactory` | class | LifetimeMap uretimi ve varsayilanlari | VersaCoder.Host | net48;net10.0-windows | LifetimeMapTests | planli |
| N39-031 | `LifetimeMapOptions` | record | LifetimeMap yapilandirmasi | VersaCoder.Host | net48;net10.0-windows | LifetimeMapTests | planli |
| N39-032 | `LifetimeMapProvider` | class | LifetimeMap saglayici uygulamasi | VersaCoder.Host | net48;net10.0-windows | LifetimeMapTests | planli |
| N39-033 | `LifetimeMapRegistry` | class | LifetimeMap kayit/katalog yonetimi | VersaCoder.Host | net48;net10.0-windows | LifetimeMapTests | planli |
| N39-034 | `LifetimeMapConverter` | class | LifetimeMap bicim donusumu | VersaCoder.Host | net48;net10.0-windows | LifetimeMapTests | planli |
| N39-035 | `LifetimeMapTests` | class | LifetimeMap testleri (xUnit) | VersaCoder.Host | net48;net10.0-windows | - | planli |
| N39-036 | `LifetimeMapBuilder` | class | LifetimeMap uretici (builder) | VersaCoder.Host | net48;net10.0-windows | LifetimeMapTests | planli |
| N39-037 | `IRegistrationSource` | interface | RegistrationSource sozlesmesi (port) | VersaCoder.Host | net48;net10.0-windows | RegistrationSourceTests | planli |
| N39-038 | `RegistrationSource` | class | RegistrationSource cekirdek uygulamasi | VersaCoder.Host | net48;net10.0-windows | RegistrationSourceTests | planli |
| N39-039 | `RegistrationSourceDto` | record | RegistrationSource veri transferi modeli | VersaCoder.Host | net48;net10.0-windows | RegistrationSourceTests | planli |
| N39-040 | `RegistrationSourceMapper` | class | RegistrationSource <-> DTO esleme | VersaCoder.Host | net48;net10.0-windows | RegistrationSourceTests | planli |
| N39-041 | `RegistrationSourceValidator` | class | RegistrationSource dogrulama kurallari | VersaCoder.Host | net48;net10.0-windows | RegistrationSourceTests | planli |
| N39-042 | `RegistrationSourceFactory` | class | RegistrationSource uretimi ve varsayilanlari | VersaCoder.Host | net48;net10.0-windows | RegistrationSourceTests | planli |
| N39-043 | `RegistrationSourceOptions` | record | RegistrationSource yapilandirmasi | VersaCoder.Host | net48;net10.0-windows | RegistrationSourceTests | planli |
| N39-044 | `RegistrationSourceProvider` | class | RegistrationSource saglayici uygulamasi | VersaCoder.Host | net48;net10.0-windows | RegistrationSourceTests | planli |
| N39-045 | `RegistrationSourceRegistry` | class | RegistrationSource kayit/katalog yonetimi | VersaCoder.Host | net48;net10.0-windows | RegistrationSourceTests | planli |
| N39-046 | `RegistrationSourceConverter` | class | RegistrationSource bicim donusumu | VersaCoder.Host | net48;net10.0-windows | RegistrationSourceTests | planli |
| N39-047 | `RegistrationSourceTests` | class | RegistrationSource testleri (xUnit) | VersaCoder.Host | net48;net10.0-windows | - | planli |
| N39-048 | `RegistrationSourceBuilder` | class | RegistrationSource uretici (builder) | VersaCoder.Host | net48;net10.0-windows | RegistrationSourceTests | planli |
| N39-049 | `IScopeFactory` | interface | ScopeFactory sozlesmesi (port) | VersaCoder.Host | net48;net10.0-windows | ScopeFactoryTests | planli |
| N39-050 | `ScopeFactory` | class | ScopeFactory cekirdek uygulamasi | VersaCoder.Host | net48;net10.0-windows | ScopeFactoryTests | planli |
| N39-051 | `ScopeFactoryDto` | record | ScopeFactory veri transferi modeli | VersaCoder.Host | net48;net10.0-windows | ScopeFactoryTests | planli |
| N39-052 | `ScopeFactoryMapper` | class | ScopeFactory <-> DTO esleme | VersaCoder.Host | net48;net10.0-windows | ScopeFactoryTests | planli |
| N39-053 | `ScopeFactoryValidator` | class | ScopeFactory dogrulama kurallari | VersaCoder.Host | net48;net10.0-windows | ScopeFactoryTests | planli |
| N39-054 | `ScopeFactoryFactory` | class | ScopeFactory uretimi ve varsayilanlari | VersaCoder.Host | net48;net10.0-windows | ScopeFactoryTests | planli |
| N39-055 | `ScopeFactoryOptions` | record | ScopeFactory yapilandirmasi | VersaCoder.Host | net48;net10.0-windows | ScopeFactoryTests | planli |
| N39-056 | `ScopeFactoryProvider` | class | ScopeFactory saglayici uygulamasi | VersaCoder.Host | net48;net10.0-windows | ScopeFactoryTests | planli |
| N39-057 | `ScopeFactoryRegistry` | class | ScopeFactory kayit/katalog yonetimi | VersaCoder.Host | net48;net10.0-windows | ScopeFactoryTests | planli |
| N39-058 | `ScopeFactoryConverter` | class | ScopeFactory bicim donusumu | VersaCoder.Host | net48;net10.0-windows | ScopeFactoryTests | planli |
| N39-059 | `ScopeFactoryTests` | class | ScopeFactory testleri (xUnit) | VersaCoder.Host | net48;net10.0-windows | - | planli |
| N39-060 | `ScopeFactoryBuilder` | class | ScopeFactory uretici (builder) | VersaCoder.Host | net48;net10.0-windows | ScopeFactoryTests | planli |
| N39-061 | `IResolverProbe` | interface | ResolverProbe sozlesmesi (port) | VersaCoder.Host | net48;net10.0-windows | ResolverProbeTests | planli |
| N39-062 | `ResolverProbe` | class | ResolverProbe cekirdek uygulamasi | VersaCoder.Host | net48;net10.0-windows | ResolverProbeTests | planli |
| N39-063 | `ResolverProbeDto` | record | ResolverProbe veri transferi modeli | VersaCoder.Host | net48;net10.0-windows | ResolverProbeTests | planli |
| N39-064 | `ResolverProbeMapper` | class | ResolverProbe <-> DTO esleme | VersaCoder.Host | net48;net10.0-windows | ResolverProbeTests | planli |
| N39-065 | `ResolverProbeValidator` | class | ResolverProbe dogrulama kurallari | VersaCoder.Host | net48;net10.0-windows | ResolverProbeTests | planli |
| N39-066 | `ResolverProbeFactory` | class | ResolverProbe uretimi ve varsayilanlari | VersaCoder.Host | net48;net10.0-windows | ResolverProbeTests | planli |
| N39-067 | `ResolverProbeOptions` | record | ResolverProbe yapilandirmasi | VersaCoder.Host | net48;net10.0-windows | ResolverProbeTests | planli |
| N39-068 | `ResolverProbeProvider` | class | ResolverProbe saglayici uygulamasi | VersaCoder.Host | net48;net10.0-windows | ResolverProbeTests | planli |
| N39-069 | `ResolverProbeRegistry` | class | ResolverProbe kayit/katalog yonetimi | VersaCoder.Host | net48;net10.0-windows | ResolverProbeTests | planli |
| N39-070 | `ResolverProbeConverter` | class | ResolverProbe bicim donusumu | VersaCoder.Host | net48;net10.0-windows | ResolverProbeTests | planli |
| N39-071 | `ResolverProbeTests` | class | ResolverProbe testleri (xUnit) | VersaCoder.Host | net48;net10.0-windows | - | planli |
| N39-072 | `ResolverProbeBuilder` | class | ResolverProbe uretici (builder) | VersaCoder.Host | net48;net10.0-windows | ResolverProbeTests | planli |
| N39-073 | `IRegistrationAudit` | interface | RegistrationAudit sozlesmesi (port) | VersaCoder.Host | net48;net10.0-windows | RegistrationAuditTests | planli |
| N39-074 | `RegistrationAudit` | class | RegistrationAudit cekirdek uygulamasi | VersaCoder.Host | net48;net10.0-windows | RegistrationAuditTests | planli |
| N39-075 | `RegistrationAuditDto` | record | RegistrationAudit veri transferi modeli | VersaCoder.Host | net48;net10.0-windows | RegistrationAuditTests | planli |
| N39-076 | `RegistrationAuditMapper` | class | RegistrationAudit <-> DTO esleme | VersaCoder.Host | net48;net10.0-windows | RegistrationAuditTests | planli |
| N39-077 | `RegistrationAuditValidator` | class | RegistrationAudit dogrulama kurallari | VersaCoder.Host | net48;net10.0-windows | RegistrationAuditTests | planli |
| N39-078 | `RegistrationAuditFactory` | class | RegistrationAudit uretimi ve varsayilanlari | VersaCoder.Host | net48;net10.0-windows | RegistrationAuditTests | planli |
| N39-079 | `RegistrationAuditOptions` | record | RegistrationAudit yapilandirmasi | VersaCoder.Host | net48;net10.0-windows | RegistrationAuditTests | planli |
| N39-080 | `RegistrationAuditProvider` | class | RegistrationAudit saglayici uygulamasi | VersaCoder.Host | net48;net10.0-windows | RegistrationAuditTests | planli |
| N39-081 | `RegistrationAuditRegistry` | class | RegistrationAudit kayit/katalog yonetimi | VersaCoder.Host | net48;net10.0-windows | RegistrationAuditTests | planli |
| N39-082 | `RegistrationAuditConverter` | class | RegistrationAudit bicim donusumu | VersaCoder.Host | net48;net10.0-windows | RegistrationAuditTests | planli |
| N39-083 | `RegistrationAuditTests` | class | RegistrationAudit testleri (xUnit) | VersaCoder.Host | net48;net10.0-windows | - | planli |
| N39-084 | `RegistrationAuditBuilder` | class | RegistrationAudit uretici (builder) | VersaCoder.Host | net48;net10.0-windows | RegistrationAuditTests | planli |
| N39-085 | `IDisposalOrder` | interface | DisposalOrder sozlesmesi (port) | VersaCoder.Host | net48;net10.0-windows | DisposalOrderTests | planli |
| N39-086 | `DisposalOrder` | class | DisposalOrder cekirdek uygulamasi | VersaCoder.Host | net48;net10.0-windows | DisposalOrderTests | planli |
| N39-087 | `DisposalOrderDto` | record | DisposalOrder veri transferi modeli | VersaCoder.Host | net48;net10.0-windows | DisposalOrderTests | planli |
| N39-088 | `DisposalOrderMapper` | class | DisposalOrder <-> DTO esleme | VersaCoder.Host | net48;net10.0-windows | DisposalOrderTests | planli |
| N39-089 | `DisposalOrderValidator` | class | DisposalOrder dogrulama kurallari | VersaCoder.Host | net48;net10.0-windows | DisposalOrderTests | planli |
| N39-090 | `DisposalOrderFactory` | class | DisposalOrder uretimi ve varsayilanlari | VersaCoder.Host | net48;net10.0-windows | DisposalOrderTests | planli |
| N39-091 | `DisposalOrderOptions` | record | DisposalOrder yapilandirmasi | VersaCoder.Host | net48;net10.0-windows | DisposalOrderTests | planli |
| N39-092 | `DisposalOrderProvider` | class | DisposalOrder saglayici uygulamasi | VersaCoder.Host | net48;net10.0-windows | DisposalOrderTests | planli |
| N39-093 | `DisposalOrderRegistry` | class | DisposalOrder kayit/katalog yonetimi | VersaCoder.Host | net48;net10.0-windows | DisposalOrderTests | planli |
| N39-094 | `DisposalOrderConverter` | class | DisposalOrder bicim donusumu | VersaCoder.Host | net48;net10.0-windows | DisposalOrderTests | planli |
| N39-095 | `DisposalOrderTests` | class | DisposalOrder testleri (xUnit) | VersaCoder.Host | net48;net10.0-windows | - | planli |
| N39-096 | `DisposalOrderBuilder` | class | DisposalOrder uretici (builder) | VersaCoder.Host | net48;net10.0-windows | DisposalOrderTests | planli |
| N39-097 | `IModuleRegistrar` | interface | ModuleRegistrar sozlesmesi (port) | VersaCoder.Host | net48;net10.0-windows | ModuleRegistrarTests | planli |
| N39-098 | `ModuleRegistrar` | class | ModuleRegistrar cekirdek uygulamasi | VersaCoder.Host | net48;net10.0-windows | ModuleRegistrarTests | planli |
| N39-099 | `ModuleRegistrarDto` | record | ModuleRegistrar veri transferi modeli | VersaCoder.Host | net48;net10.0-windows | ModuleRegistrarTests | planli |
| N39-100 | `ModuleRegistrarMapper` | class | ModuleRegistrar <-> DTO esleme | VersaCoder.Host | net48;net10.0-windows | ModuleRegistrarTests | planli |
| N39-101 | `ModuleRegistrarValidator` | class | ModuleRegistrar dogrulama kurallari | VersaCoder.Host | net48;net10.0-windows | ModuleRegistrarTests | planli |
| N39-102 | `ModuleRegistrarFactory` | class | ModuleRegistrar uretimi ve varsayilanlari | VersaCoder.Host | net48;net10.0-windows | ModuleRegistrarTests | planli |
| N39-103 | `ModuleRegistrarOptions` | record | ModuleRegistrar yapilandirmasi | VersaCoder.Host | net48;net10.0-windows | ModuleRegistrarTests | planli |
| N39-104 | `ModuleRegistrarProvider` | class | ModuleRegistrar saglayici uygulamasi | VersaCoder.Host | net48;net10.0-windows | ModuleRegistrarTests | planli |
| N39-105 | `ModuleRegistrarRegistry` | class | ModuleRegistrar kayit/katalog yonetimi | VersaCoder.Host | net48;net10.0-windows | ModuleRegistrarTests | planli |
| N39-106 | `ModuleRegistrarConverter` | class | ModuleRegistrar bicim donusumu | VersaCoder.Host | net48;net10.0-windows | ModuleRegistrarTests | planli |
| N39-107 | `ModuleRegistrarTests` | class | ModuleRegistrar testleri (xUnit) | VersaCoder.Host | net48;net10.0-windows | - | planli |
| N39-108 | `ModuleRegistrarBuilder` | class | ModuleRegistrar uretici (builder) | VersaCoder.Host | net48;net10.0-windows | ModuleRegistrarTests | planli |
| N39-109 | `ITestDoubleRegistrar` | interface | TestDoubleRegistrar sozlesmesi (port) | VersaCoder.Host | net48;net10.0-windows | TestDoubleRegistrarTests | planli |
| N39-110 | `TestDoubleRegistrar` | class | TestDoubleRegistrar cekirdek uygulamasi | VersaCoder.Host | net48;net10.0-windows | TestDoubleRegistrarTests | planli |
| N39-111 | `TestDoubleRegistrarDto` | record | TestDoubleRegistrar veri transferi modeli | VersaCoder.Host | net48;net10.0-windows | TestDoubleRegistrarTests | planli |
| N39-112 | `TestDoubleRegistrarMapper` | class | TestDoubleRegistrar <-> DTO esleme | VersaCoder.Host | net48;net10.0-windows | TestDoubleRegistrarTests | planli |
| N39-113 | `TestDoubleRegistrarValidator` | class | TestDoubleRegistrar dogrulama kurallari | VersaCoder.Host | net48;net10.0-windows | TestDoubleRegistrarTests | planli |
| N39-114 | `TestDoubleRegistrarFactory` | class | TestDoubleRegistrar uretimi ve varsayilanlari | VersaCoder.Host | net48;net10.0-windows | TestDoubleRegistrarTests | planli |
| N39-115 | `TestDoubleRegistrarOptions` | record | TestDoubleRegistrar yapilandirmasi | VersaCoder.Host | net48;net10.0-windows | TestDoubleRegistrarTests | planli |
| N39-116 | `TestDoubleRegistrarProvider` | class | TestDoubleRegistrar saglayici uygulamasi | VersaCoder.Host | net48;net10.0-windows | TestDoubleRegistrarTests | planli |
| N39-117 | `TestDoubleRegistrarRegistry` | class | TestDoubleRegistrar kayit/katalog yonetimi | VersaCoder.Host | net48;net10.0-windows | TestDoubleRegistrarTests | planli |
| N39-118 | `TestDoubleRegistrarConverter` | class | TestDoubleRegistrar bicim donusumu | VersaCoder.Host | net48;net10.0-windows | TestDoubleRegistrarTests | planli |
| N39-119 | `TestDoubleRegistrarTests` | class | TestDoubleRegistrar testleri (xUnit) | VersaCoder.Host | net48;net10.0-windows | - | planli |
| N39-120 | `TestDoubleRegistrarBuilder` | class | TestDoubleRegistrar uretici (builder) | VersaCoder.Host | net48;net10.0-windows | TestDoubleRegistrarTests | planli |
