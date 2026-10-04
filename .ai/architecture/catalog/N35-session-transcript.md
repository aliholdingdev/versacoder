---
title: Katalog Dugumu N35 - Oturum ve Transkript
type: catalog-node
category: l5-protocol
status: needs-validation
version: 1.0.0
updated: 2026-10-04
authority: derived
scope: oturum durumu, transkript ve sikistirma
---

# Katalog Dugumu N35 - Oturum ve Transkript

**Zorunlu Baglantilar:** index.md | node-inventory.md | ../index.md | ../../.decisions/accepted/ADR-028-layered-baseline-9-projects.md | ../../.decisions/accepted/ADR-013-layered-architecture-52-nodes.md

> Dugum, katman sozlesmesini cevaplar ve kanitini baglar.

## §0 Ozet (Answer First)

| Alan | Deger |
| --- | --- |
| Katman | l5 Protocol |
| Proje | VersaCoder.Protocol |
| Kapsam | oturum durumu, transkript ve sikistirma |
| Kaynak | C:/www/versacoder/versacoder/.ai/architecture/catalog/l5-protocol/N35-session-transcript.md |
| Sorumluluk | Dugum, katman sozlesmesini cevaplar ve kanitini baglar. |

- Oturum; mesaj gecmisi, ekler ve ozet cerceveleriyle kalici transkript olarak saklanir.
- Baglam penceresi doldugunda sikistirma kurali en eski girisleri ozetleyip sikistirir.
- Transkript dizini arama ve yeniden oynatma icin indekslenir.
- Tutma politikasi eski kayitlari silmek yerine arsivler, silme yalnizca acik politikayla olur.
- Kayitlar en az 6 aile 120 ornek ile kanitlanir, gerisi namludur (kayit yok).

## §1 YAPAR / YAPMAZ

| YAPAR | YAPMAZ |
| --- | --- |
| Oturum durumu ve transkript kaydi uretir | Mesaji uretir (l5 protokol + l4 is) |
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
| 1 | SessionState | Oturum durum modeli |
| 2 | TranscriptEntry | Tek transkript girisi |
| 3 | TranscriptStore | Kalici yazma ve okuma |
| 4 | CompactionRule | Sikistirma kurali |
| 5 | ContextWindow | Baglam penceresi olcusu |
| 6 | TurnRecord | Tur kaydi (soru-cevap) |
| 7 | AttachmentRef | Ek referansi |
| 8 | SummaryFrame | Ozet cercevesi |
| 9 | RetentionPolicy | Tutma politikasi |
| 10 | TranscriptIndex | Arama indeksi |

## §4 Ornek Kayit (seri 120)

| Kayit | Aile | Alan1 | Alan2 |
| --- | --- | --- | --- |
| 1 | SessionState | session | aktif |
| 2 | SessionState | turn | 12 |
| 3 | TranscriptEntry | girdi 1 | mesaj |
| 4 | TranscriptEntry | sikistir | esik 80% |
| 5 | TranscriptStore | ek | dosya ref |
| 6 | CompactionRule | arsiv | 30 gun |

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
| TranskriptYazmaTesti | SessionState | yazilan |
| SikistirmaTesti | SessionState | sikisan |
| IndeksAramaTesti | ContextWindow | bulunan |
| TutmaPolitikasiTesti | ContextWindow | arsivlenen |

## §7 Riskler

| Risk | Etki | Onlem |
| --- | --- | --- |
| Baglam tasmasi | maliyet | sikistirma esigi |
| Veri kaybi | kalici silme | arsiv once |
| Indeks uyumsuzlugu | arama yanlis | surum indeksi |

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
| Dosya | N35-session-transcript.md |
| Klasor ornegi | C:\versacoder\src\VersaCoder.Protocol |
| Ornek kayit | 6/6 hedef (seri 120 icin) |
| Test | 4/4 hedef |

kayit tablosu §10'e port edildi (kaynak vault 53 dosya / 5440 kayit); esik 5000 karsilanir.


## §10 Kayit Tablosu (ADR-013 2.2 - kaynak vault port)

| N35-001 | `ISessionState` | interface | SessionState sozlesmesi (port) | VersaCoder.Protocol | net48;net10.0-windows | SessionStateTests | planli |
| N35-002 | `SessionState` | class | SessionState cekirdek uygulamasi | VersaCoder.Protocol | net48;net10.0-windows | SessionStateTests | planli |
| N35-003 | `SessionStateDto` | record | SessionState veri transferi modeli | VersaCoder.Protocol | net48;net10.0-windows | SessionStateTests | planli |
| N35-004 | `SessionStateMapper` | class | SessionState <-> DTO esleme | VersaCoder.Protocol | net48;net10.0-windows | SessionStateTests | planli |
| N35-005 | `SessionStateValidator` | class | SessionState dogrulama kurallari | VersaCoder.Protocol | net48;net10.0-windows | SessionStateTests | planli |
| N35-006 | `SessionStateFactory` | class | SessionState uretimi ve varsayilanlari | VersaCoder.Protocol | net48;net10.0-windows | SessionStateTests | planli |
| N35-007 | `SessionStateOptions` | record | SessionState yapilandirmasi | VersaCoder.Protocol | net48;net10.0-windows | SessionStateTests | planli |
| N35-008 | `SessionStateProvider` | class | SessionState saglayici uygulamasi | VersaCoder.Protocol | net48;net10.0-windows | SessionStateTests | planli |
| N35-009 | `SessionStateRegistry` | class | SessionState kayit/katalog yonetimi | VersaCoder.Protocol | net48;net10.0-windows | SessionStateTests | planli |
| N35-010 | `SessionStateConverter` | class | SessionState bicim donusumu | VersaCoder.Protocol | net48;net10.0-windows | SessionStateTests | planli |
| N35-011 | `SessionStateTests` | class | SessionState testleri (xUnit) | VersaCoder.Protocol | net48;net10.0-windows | - | planli |
| N35-012 | `SessionStateBuilder` | class | SessionState uretici (builder) | VersaCoder.Protocol | net48;net10.0-windows | SessionStateTests | planli |
| N35-013 | `ITranscriptEntry` | interface | TranscriptEntry sozlesmesi (port) | VersaCoder.Protocol | net48;net10.0-windows | TranscriptEntryTests | planli |
| N35-014 | `TranscriptEntry` | class | TranscriptEntry cekirdek uygulamasi | VersaCoder.Protocol | net48;net10.0-windows | TranscriptEntryTests | planli |
| N35-015 | `TranscriptEntryDto` | record | TranscriptEntry veri transferi modeli | VersaCoder.Protocol | net48;net10.0-windows | TranscriptEntryTests | planli |
| N35-016 | `TranscriptEntryMapper` | class | TranscriptEntry <-> DTO esleme | VersaCoder.Protocol | net48;net10.0-windows | TranscriptEntryTests | planli |
| N35-017 | `TranscriptEntryValidator` | class | TranscriptEntry dogrulama kurallari | VersaCoder.Protocol | net48;net10.0-windows | TranscriptEntryTests | planli |
| N35-018 | `TranscriptEntryFactory` | class | TranscriptEntry uretimi ve varsayilanlari | VersaCoder.Protocol | net48;net10.0-windows | TranscriptEntryTests | planli |
| N35-019 | `TranscriptEntryOptions` | record | TranscriptEntry yapilandirmasi | VersaCoder.Protocol | net48;net10.0-windows | TranscriptEntryTests | planli |
| N35-020 | `TranscriptEntryProvider` | class | TranscriptEntry saglayici uygulamasi | VersaCoder.Protocol | net48;net10.0-windows | TranscriptEntryTests | planli |
| N35-021 | `TranscriptEntryRegistry` | class | TranscriptEntry kayit/katalog yonetimi | VersaCoder.Protocol | net48;net10.0-windows | TranscriptEntryTests | planli |
| N35-022 | `TranscriptEntryConverter` | class | TranscriptEntry bicim donusumu | VersaCoder.Protocol | net48;net10.0-windows | TranscriptEntryTests | planli |
| N35-023 | `TranscriptEntryTests` | class | TranscriptEntry testleri (xUnit) | VersaCoder.Protocol | net48;net10.0-windows | - | planli |
| N35-024 | `TranscriptEntryBuilder` | class | TranscriptEntry uretici (builder) | VersaCoder.Protocol | net48;net10.0-windows | TranscriptEntryTests | planli |
| N35-025 | `ITranscriptStore` | interface | TranscriptStore sozlesmesi (port) | VersaCoder.Protocol | net48;net10.0-windows | TranscriptStoreTests | planli |
| N35-026 | `TranscriptStore` | class | TranscriptStore cekirdek uygulamasi | VersaCoder.Protocol | net48;net10.0-windows | TranscriptStoreTests | planli |
| N35-027 | `TranscriptStoreDto` | record | TranscriptStore veri transferi modeli | VersaCoder.Protocol | net48;net10.0-windows | TranscriptStoreTests | planli |
| N35-028 | `TranscriptStoreMapper` | class | TranscriptStore <-> DTO esleme | VersaCoder.Protocol | net48;net10.0-windows | TranscriptStoreTests | planli |
| N35-029 | `TranscriptStoreValidator` | class | TranscriptStore dogrulama kurallari | VersaCoder.Protocol | net48;net10.0-windows | TranscriptStoreTests | planli |
| N35-030 | `TranscriptStoreFactory` | class | TranscriptStore uretimi ve varsayilanlari | VersaCoder.Protocol | net48;net10.0-windows | TranscriptStoreTests | planli |
| N35-031 | `TranscriptStoreOptions` | record | TranscriptStore yapilandirmasi | VersaCoder.Protocol | net48;net10.0-windows | TranscriptStoreTests | planli |
| N35-032 | `TranscriptStoreProvider` | class | TranscriptStore saglayici uygulamasi | VersaCoder.Protocol | net48;net10.0-windows | TranscriptStoreTests | planli |
| N35-033 | `TranscriptStoreRegistry` | class | TranscriptStore kayit/katalog yonetimi | VersaCoder.Protocol | net48;net10.0-windows | TranscriptStoreTests | planli |
| N35-034 | `TranscriptStoreConverter` | class | TranscriptStore bicim donusumu | VersaCoder.Protocol | net48;net10.0-windows | TranscriptStoreTests | planli |
| N35-035 | `TranscriptStoreTests` | class | TranscriptStore testleri (xUnit) | VersaCoder.Protocol | net48;net10.0-windows | - | planli |
| N35-036 | `TranscriptStoreBuilder` | class | TranscriptStore uretici (builder) | VersaCoder.Protocol | net48;net10.0-windows | TranscriptStoreTests | planli |
| N35-037 | `ICompactionRule` | interface | CompactionRule sozlesmesi (port) | VersaCoder.Protocol | net48;net10.0-windows | CompactionRuleTests | planli |
| N35-038 | `CompactionRule` | class | CompactionRule cekirdek uygulamasi | VersaCoder.Protocol | net48;net10.0-windows | CompactionRuleTests | planli |
| N35-039 | `CompactionRuleDto` | record | CompactionRule veri transferi modeli | VersaCoder.Protocol | net48;net10.0-windows | CompactionRuleTests | planli |
| N35-040 | `CompactionRuleMapper` | class | CompactionRule <-> DTO esleme | VersaCoder.Protocol | net48;net10.0-windows | CompactionRuleTests | planli |
| N35-041 | `CompactionRuleValidator` | class | CompactionRule dogrulama kurallari | VersaCoder.Protocol | net48;net10.0-windows | CompactionRuleTests | planli |
| N35-042 | `CompactionRuleFactory` | class | CompactionRule uretimi ve varsayilanlari | VersaCoder.Protocol | net48;net10.0-windows | CompactionRuleTests | planli |
| N35-043 | `CompactionRuleOptions` | record | CompactionRule yapilandirmasi | VersaCoder.Protocol | net48;net10.0-windows | CompactionRuleTests | planli |
| N35-044 | `CompactionRuleProvider` | class | CompactionRule saglayici uygulamasi | VersaCoder.Protocol | net48;net10.0-windows | CompactionRuleTests | planli |
| N35-045 | `CompactionRuleRegistry` | class | CompactionRule kayit/katalog yonetimi | VersaCoder.Protocol | net48;net10.0-windows | CompactionRuleTests | planli |
| N35-046 | `CompactionRuleConverter` | class | CompactionRule bicim donusumu | VersaCoder.Protocol | net48;net10.0-windows | CompactionRuleTests | planli |
| N35-047 | `CompactionRuleTests` | class | CompactionRule testleri (xUnit) | VersaCoder.Protocol | net48;net10.0-windows | - | planli |
| N35-048 | `CompactionRuleBuilder` | class | CompactionRule uretici (builder) | VersaCoder.Protocol | net48;net10.0-windows | CompactionRuleTests | planli |
| N35-049 | `IContextWindow` | interface | ContextWindow sozlesmesi (port) | VersaCoder.Protocol | net48;net10.0-windows | ContextWindowTests | planli |
| N35-050 | `ContextWindow` | class | ContextWindow cekirdek uygulamasi | VersaCoder.Protocol | net48;net10.0-windows | ContextWindowTests | planli |
| N35-051 | `ContextWindowDto` | record | ContextWindow veri transferi modeli | VersaCoder.Protocol | net48;net10.0-windows | ContextWindowTests | planli |
| N35-052 | `ContextWindowMapper` | class | ContextWindow <-> DTO esleme | VersaCoder.Protocol | net48;net10.0-windows | ContextWindowTests | planli |
| N35-053 | `ContextWindowValidator` | class | ContextWindow dogrulama kurallari | VersaCoder.Protocol | net48;net10.0-windows | ContextWindowTests | planli |
| N35-054 | `ContextWindowFactory` | class | ContextWindow uretimi ve varsayilanlari | VersaCoder.Protocol | net48;net10.0-windows | ContextWindowTests | planli |
| N35-055 | `ContextWindowOptions` | record | ContextWindow yapilandirmasi | VersaCoder.Protocol | net48;net10.0-windows | ContextWindowTests | planli |
| N35-056 | `ContextWindowProvider` | class | ContextWindow saglayici uygulamasi | VersaCoder.Protocol | net48;net10.0-windows | ContextWindowTests | planli |
| N35-057 | `ContextWindowRegistry` | class | ContextWindow kayit/katalog yonetimi | VersaCoder.Protocol | net48;net10.0-windows | ContextWindowTests | planli |
| N35-058 | `ContextWindowConverter` | class | ContextWindow bicim donusumu | VersaCoder.Protocol | net48;net10.0-windows | ContextWindowTests | planli |
| N35-059 | `ContextWindowTests` | class | ContextWindow testleri (xUnit) | VersaCoder.Protocol | net48;net10.0-windows | - | planli |
| N35-060 | `ContextWindowBuilder` | class | ContextWindow uretici (builder) | VersaCoder.Protocol | net48;net10.0-windows | ContextWindowTests | planli |
| N35-061 | `ITurnRecord` | interface | TurnRecord sozlesmesi (port) | VersaCoder.Protocol | net48;net10.0-windows | TurnRecordTests | planli |
| N35-062 | `TurnRecord` | class | TurnRecord cekirdek uygulamasi | VersaCoder.Protocol | net48;net10.0-windows | TurnRecordTests | planli |
| N35-063 | `TurnRecordDto` | record | TurnRecord veri transferi modeli | VersaCoder.Protocol | net48;net10.0-windows | TurnRecordTests | planli |
| N35-064 | `TurnRecordMapper` | class | TurnRecord <-> DTO esleme | VersaCoder.Protocol | net48;net10.0-windows | TurnRecordTests | planli |
| N35-065 | `TurnRecordValidator` | class | TurnRecord dogrulama kurallari | VersaCoder.Protocol | net48;net10.0-windows | TurnRecordTests | planli |
| N35-066 | `TurnRecordFactory` | class | TurnRecord uretimi ve varsayilanlari | VersaCoder.Protocol | net48;net10.0-windows | TurnRecordTests | planli |
| N35-067 | `TurnRecordOptions` | record | TurnRecord yapilandirmasi | VersaCoder.Protocol | net48;net10.0-windows | TurnRecordTests | planli |
| N35-068 | `TurnRecordProvider` | class | TurnRecord saglayici uygulamasi | VersaCoder.Protocol | net48;net10.0-windows | TurnRecordTests | planli |
| N35-069 | `TurnRecordRegistry` | class | TurnRecord kayit/katalog yonetimi | VersaCoder.Protocol | net48;net10.0-windows | TurnRecordTests | planli |
| N35-070 | `TurnRecordConverter` | class | TurnRecord bicim donusumu | VersaCoder.Protocol | net48;net10.0-windows | TurnRecordTests | planli |
| N35-071 | `TurnRecordTests` | class | TurnRecord testleri (xUnit) | VersaCoder.Protocol | net48;net10.0-windows | - | planli |
| N35-072 | `TurnRecordBuilder` | class | TurnRecord uretici (builder) | VersaCoder.Protocol | net48;net10.0-windows | TurnRecordTests | planli |
| N35-073 | `IAttachmentRef` | interface | AttachmentRef sozlesmesi (port) | VersaCoder.Protocol | net48;net10.0-windows | AttachmentRefTests | planli |
| N35-074 | `AttachmentRef` | class | AttachmentRef cekirdek uygulamasi | VersaCoder.Protocol | net48;net10.0-windows | AttachmentRefTests | planli |
| N35-075 | `AttachmentRefDto` | record | AttachmentRef veri transferi modeli | VersaCoder.Protocol | net48;net10.0-windows | AttachmentRefTests | planli |
| N35-076 | `AttachmentRefMapper` | class | AttachmentRef <-> DTO esleme | VersaCoder.Protocol | net48;net10.0-windows | AttachmentRefTests | planli |
| N35-077 | `AttachmentRefValidator` | class | AttachmentRef dogrulama kurallari | VersaCoder.Protocol | net48;net10.0-windows | AttachmentRefTests | planli |
| N35-078 | `AttachmentRefFactory` | class | AttachmentRef uretimi ve varsayilanlari | VersaCoder.Protocol | net48;net10.0-windows | AttachmentRefTests | planli |
| N35-079 | `AttachmentRefOptions` | record | AttachmentRef yapilandirmasi | VersaCoder.Protocol | net48;net10.0-windows | AttachmentRefTests | planli |
| N35-080 | `AttachmentRefProvider` | class | AttachmentRef saglayici uygulamasi | VersaCoder.Protocol | net48;net10.0-windows | AttachmentRefTests | planli |
| N35-081 | `AttachmentRefRegistry` | class | AttachmentRef kayit/katalog yonetimi | VersaCoder.Protocol | net48;net10.0-windows | AttachmentRefTests | planli |
| N35-082 | `AttachmentRefConverter` | class | AttachmentRef bicim donusumu | VersaCoder.Protocol | net48;net10.0-windows | AttachmentRefTests | planli |
| N35-083 | `AttachmentRefTests` | class | AttachmentRef testleri (xUnit) | VersaCoder.Protocol | net48;net10.0-windows | - | planli |
| N35-084 | `AttachmentRefBuilder` | class | AttachmentRef uretici (builder) | VersaCoder.Protocol | net48;net10.0-windows | AttachmentRefTests | planli |
| N35-085 | `ISummaryFrame` | interface | SummaryFrame sozlesmesi (port) | VersaCoder.Protocol | net48;net10.0-windows | SummaryFrameTests | planli |
| N35-086 | `SummaryFrame` | class | SummaryFrame cekirdek uygulamasi | VersaCoder.Protocol | net48;net10.0-windows | SummaryFrameTests | planli |
| N35-087 | `SummaryFrameDto` | record | SummaryFrame veri transferi modeli | VersaCoder.Protocol | net48;net10.0-windows | SummaryFrameTests | planli |
| N35-088 | `SummaryFrameMapper` | class | SummaryFrame <-> DTO esleme | VersaCoder.Protocol | net48;net10.0-windows | SummaryFrameTests | planli |
| N35-089 | `SummaryFrameValidator` | class | SummaryFrame dogrulama kurallari | VersaCoder.Protocol | net48;net10.0-windows | SummaryFrameTests | planli |
| N35-090 | `SummaryFrameFactory` | class | SummaryFrame uretimi ve varsayilanlari | VersaCoder.Protocol | net48;net10.0-windows | SummaryFrameTests | planli |
| N35-091 | `SummaryFrameOptions` | record | SummaryFrame yapilandirmasi | VersaCoder.Protocol | net48;net10.0-windows | SummaryFrameTests | planli |
| N35-092 | `SummaryFrameProvider` | class | SummaryFrame saglayici uygulamasi | VersaCoder.Protocol | net48;net10.0-windows | SummaryFrameTests | planli |
| N35-093 | `SummaryFrameRegistry` | class | SummaryFrame kayit/katalog yonetimi | VersaCoder.Protocol | net48;net10.0-windows | SummaryFrameTests | planli |
| N35-094 | `SummaryFrameConverter` | class | SummaryFrame bicim donusumu | VersaCoder.Protocol | net48;net10.0-windows | SummaryFrameTests | planli |
| N35-095 | `SummaryFrameTests` | class | SummaryFrame testleri (xUnit) | VersaCoder.Protocol | net48;net10.0-windows | - | planli |
| N35-096 | `SummaryFrameBuilder` | class | SummaryFrame uretici (builder) | VersaCoder.Protocol | net48;net10.0-windows | SummaryFrameTests | planli |
| N35-097 | `IRetentionPolicy` | interface | RetentionPolicy sozlesmesi (port) | VersaCoder.Protocol | net48;net10.0-windows | RetentionPolicyTests | planli |
| N35-098 | `RetentionPolicy` | class | RetentionPolicy cekirdek uygulamasi | VersaCoder.Protocol | net48;net10.0-windows | RetentionPolicyTests | planli |
| N35-099 | `RetentionPolicyDto` | record | RetentionPolicy veri transferi modeli | VersaCoder.Protocol | net48;net10.0-windows | RetentionPolicyTests | planli |
| N35-100 | `RetentionPolicyMapper` | class | RetentionPolicy <-> DTO esleme | VersaCoder.Protocol | net48;net10.0-windows | RetentionPolicyTests | planli |
| N35-101 | `RetentionPolicyValidator` | class | RetentionPolicy dogrulama kurallari | VersaCoder.Protocol | net48;net10.0-windows | RetentionPolicyTests | planli |
| N35-102 | `RetentionPolicyFactory` | class | RetentionPolicy uretimi ve varsayilanlari | VersaCoder.Protocol | net48;net10.0-windows | RetentionPolicyTests | planli |
| N35-103 | `RetentionPolicyOptions` | record | RetentionPolicy yapilandirmasi | VersaCoder.Protocol | net48;net10.0-windows | RetentionPolicyTests | planli |
| N35-104 | `RetentionPolicyProvider` | class | RetentionPolicy saglayici uygulamasi | VersaCoder.Protocol | net48;net10.0-windows | RetentionPolicyTests | planli |
| N35-105 | `RetentionPolicyRegistry` | class | RetentionPolicy kayit/katalog yonetimi | VersaCoder.Protocol | net48;net10.0-windows | RetentionPolicyTests | planli |
| N35-106 | `RetentionPolicyConverter` | class | RetentionPolicy bicim donusumu | VersaCoder.Protocol | net48;net10.0-windows | RetentionPolicyTests | planli |
| N35-107 | `RetentionPolicyTests` | class | RetentionPolicy testleri (xUnit) | VersaCoder.Protocol | net48;net10.0-windows | - | planli |
| N35-108 | `RetentionPolicyBuilder` | class | RetentionPolicy uretici (builder) | VersaCoder.Protocol | net48;net10.0-windows | RetentionPolicyTests | planli |
| N35-109 | `ITranscriptIndex` | interface | TranscriptIndex sozlesmesi (port) | VersaCoder.Protocol | net48;net10.0-windows | TranscriptIndexTests | planli |
| N35-110 | `TranscriptIndex` | class | TranscriptIndex cekirdek uygulamasi | VersaCoder.Protocol | net48;net10.0-windows | TranscriptIndexTests | planli |
| N35-111 | `TranscriptIndexDto` | record | TranscriptIndex veri transferi modeli | VersaCoder.Protocol | net48;net10.0-windows | TranscriptIndexTests | planli |
| N35-112 | `TranscriptIndexMapper` | class | TranscriptIndex <-> DTO esleme | VersaCoder.Protocol | net48;net10.0-windows | TranscriptIndexTests | planli |
| N35-113 | `TranscriptIndexValidator` | class | TranscriptIndex dogrulama kurallari | VersaCoder.Protocol | net48;net10.0-windows | TranscriptIndexTests | planli |
| N35-114 | `TranscriptIndexFactory` | class | TranscriptIndex uretimi ve varsayilanlari | VersaCoder.Protocol | net48;net10.0-windows | TranscriptIndexTests | planli |
| N35-115 | `TranscriptIndexOptions` | record | TranscriptIndex yapilandirmasi | VersaCoder.Protocol | net48;net10.0-windows | TranscriptIndexTests | planli |
| N35-116 | `TranscriptIndexProvider` | class | TranscriptIndex saglayici uygulamasi | VersaCoder.Protocol | net48;net10.0-windows | TranscriptIndexTests | planli |
| N35-117 | `TranscriptIndexRegistry` | class | TranscriptIndex kayit/katalog yonetimi | VersaCoder.Protocol | net48;net10.0-windows | TranscriptIndexTests | planli |
| N35-118 | `TranscriptIndexConverter` | class | TranscriptIndex bicim donusumu | VersaCoder.Protocol | net48;net10.0-windows | TranscriptIndexTests | planli |
| N35-119 | `TranscriptIndexTests` | class | TranscriptIndex testleri (xUnit) | VersaCoder.Protocol | net48;net10.0-windows | - | planli |
| N35-120 | `TranscriptIndexBuilder` | class | TranscriptIndex uretici (builder) | VersaCoder.Protocol | net48;net10.0-windows | TranscriptIndexTests | planli |
