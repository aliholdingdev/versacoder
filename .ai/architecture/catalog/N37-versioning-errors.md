---
title: Katalog Dugumu N37 - Surumlendirme ve Hatalar
type: catalog-node
category: l5-protocol
status: needs-validation
version: 1.0.0
updated: 2026-10-04
authority: derived
scope: protokol surumu, hata katalogu ve uyum
---

# Katalog Dugumu N37 - Surumlendirme ve Hatalar

**Zorunlu Baglantilar:** index.md | node-inventory.md | ../index.md | ../../.decisions/accepted/ADR-028-layered-baseline-9-projects.md | ../../.decisions/accepted/ADR-013-layered-architecture-52-nodes.md

> Dugum, katman sozlesmesini cevaplar ve kanitini baglar.

## §0 Ozet (Answer First)

| Alan | Deger |
| --- | --- |
| Katman | l5 Protocol |
| Proje | VersaCoder.Protocol |
| Kapsam | protokol surumu, hata katalogu ve uyum |
| Kaynak | C:/www/versacoder/versacoder/.ai/architecture/catalog/l5-protocol/N37-versioning-errors.md |
| Sorumluluk | Dugum, katman sozlesmesini cevaplar ve kanitini baglar. |

- Protokol surumu semantik kuralla yazilir, uyum matrisi hangi surumlerin konusacagini soyler.
- Hata kodlari merkezi katalogdan uretilir, serbest metin hata yasaktir.
- Kirilan degisiklikler tanimlanir ve yeni surume gecis ipucu uretilir.
- Surum koruyucu eski paketleri reddedip kacinilabilir hata kodu dondurur.
- Kayitlar en az 6 aile 120 ornek ile kanitlanir, gerisi namludur (kayit yok).

## §1 YAPAR / YAPMAZ

| YAPAR | YAPMAZ |
| --- | --- |
| Surum anlasmasi ve hata katalogu uretir | Hati loglar (l3 ise) |
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
| 1 | ProtocolVersion | Surum tanimi ve karsilastirma |
| 2 | ProtocolErrorCode | Hata kodu enum uretimi |
| 3 | ProtocolViolation | Ihlal kaydi |
| 4 | ErrorCatalog | Merkezi hata katalogu |
| 5 | VersionNegotiation | Surum anlasmasi |
| 6 | CompatMatrix | Uyum matrisi |
| 7 | BreakingChange | Kirilan degisiklik kaydi |
| 8 | VersionGuard | Surum koruyucusu |
| 9 | MigrationHint | Gecis ipucu uretimi |
| 10 | ErrorSerializer | Hata paketi serilestirme |

## §4 Ornek Kayit (seri 120)

| Kayit | Aile | Alan1 | Alan2 |
| --- | --- | --- | --- |
| 1 | ProtocolVersion | v1.4 | semver |
| 2 | ProtocolVersion | ERR-012 | sema hatasi |
| 3 | ProtocolErrorCode | negotiate | v1-v2 |
| 4 | ProtocolErrorCode | compat | ok |
| 5 | ProtocolViolation | breaking | alan silindi |
| 6 | ErrorCatalog | guard | reddedilen |

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
| SurumKarsilastirmaTesti | ProtocolVersion | karsilastirilan |
| HataKataloguTesti | ProtocolVersion | kod bulunan |
| UyumMatrisiTesti | VersionNegotiation | eslesen |
| SurumKoruyucuTesti | VersionNegotiation | reddedilen |

## §7 Riskler

| Risk | Etki | Onlem |
| --- | --- | --- |
| Serbest metin hata | tutarsiz | katalog zorunlu |
| Sessiz kirilma | uyumsuzluk | uyum matrisi |
| Gecis eksikligi | kirilma | migration hint |

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
| Dosya | N37-versioning-errors.md |
| Klasor ornegi | C:\versacoder\src\VersaCoder.Protocol |
| Ornek kayit | 6/6 hedef (seri 120 icin) |
| Test | 4/4 hedef |

kayit tablosu §10'e port edildi (kaynak vault 53 dosya / 5440 kayit); esik 5000 karsilanir.


## §10 Kayit Tablosu (ADR-013 2.2 - kaynak vault port)

| N37-001 | `IProtocolVersion` | interface | ProtocolVersion sozlesmesi (port) | VersaCoder.Protocol | net48;net10.0-windows | ProtocolVersionTests | planli |
| N37-002 | `ProtocolVersion` | class | ProtocolVersion cekirdek uygulamasi | VersaCoder.Protocol | net48;net10.0-windows | ProtocolVersionTests | planli |
| N37-003 | `ProtocolVersionDto` | record | ProtocolVersion veri transferi modeli | VersaCoder.Protocol | net48;net10.0-windows | ProtocolVersionTests | planli |
| N37-004 | `ProtocolVersionMapper` | class | ProtocolVersion <-> DTO esleme | VersaCoder.Protocol | net48;net10.0-windows | ProtocolVersionTests | planli |
| N37-005 | `ProtocolVersionValidator` | class | ProtocolVersion dogrulama kurallari | VersaCoder.Protocol | net48;net10.0-windows | ProtocolVersionTests | planli |
| N37-006 | `ProtocolVersionFactory` | class | ProtocolVersion uretimi ve varsayilanlari | VersaCoder.Protocol | net48;net10.0-windows | ProtocolVersionTests | planli |
| N37-007 | `ProtocolVersionOptions` | record | ProtocolVersion yapilandirmasi | VersaCoder.Protocol | net48;net10.0-windows | ProtocolVersionTests | planli |
| N37-008 | `ProtocolVersionProvider` | class | ProtocolVersion saglayici uygulamasi | VersaCoder.Protocol | net48;net10.0-windows | ProtocolVersionTests | planli |
| N37-009 | `ProtocolVersionRegistry` | class | ProtocolVersion kayit/katalog yonetimi | VersaCoder.Protocol | net48;net10.0-windows | ProtocolVersionTests | planli |
| N37-010 | `ProtocolVersionConverter` | class | ProtocolVersion bicim donusumu | VersaCoder.Protocol | net48;net10.0-windows | ProtocolVersionTests | planli |
| N37-011 | `ProtocolVersionTests` | class | ProtocolVersion testleri (xUnit) | VersaCoder.Protocol | net48;net10.0-windows | - | planli |
| N37-012 | `ProtocolVersionBuilder` | class | ProtocolVersion uretici (builder) | VersaCoder.Protocol | net48;net10.0-windows | ProtocolVersionTests | planli |
| N37-013 | `IProtocolErrorCode` | interface | ProtocolErrorCode sozlesmesi (port) | VersaCoder.Protocol | net48;net10.0-windows | ProtocolErrorCodeTests | planli |
| N37-014 | `ProtocolErrorCode` | class | ProtocolErrorCode cekirdek uygulamasi | VersaCoder.Protocol | net48;net10.0-windows | ProtocolErrorCodeTests | planli |
| N37-015 | `ProtocolErrorCodeDto` | record | ProtocolErrorCode veri transferi modeli | VersaCoder.Protocol | net48;net10.0-windows | ProtocolErrorCodeTests | planli |
| N37-016 | `ProtocolErrorCodeMapper` | class | ProtocolErrorCode <-> DTO esleme | VersaCoder.Protocol | net48;net10.0-windows | ProtocolErrorCodeTests | planli |
| N37-017 | `ProtocolErrorCodeValidator` | class | ProtocolErrorCode dogrulama kurallari | VersaCoder.Protocol | net48;net10.0-windows | ProtocolErrorCodeTests | planli |
| N37-018 | `ProtocolErrorCodeFactory` | class | ProtocolErrorCode uretimi ve varsayilanlari | VersaCoder.Protocol | net48;net10.0-windows | ProtocolErrorCodeTests | planli |
| N37-019 | `ProtocolErrorCodeOptions` | record | ProtocolErrorCode yapilandirmasi | VersaCoder.Protocol | net48;net10.0-windows | ProtocolErrorCodeTests | planli |
| N37-020 | `ProtocolErrorCodeProvider` | class | ProtocolErrorCode saglayici uygulamasi | VersaCoder.Protocol | net48;net10.0-windows | ProtocolErrorCodeTests | planli |
| N37-021 | `ProtocolErrorCodeRegistry` | class | ProtocolErrorCode kayit/katalog yonetimi | VersaCoder.Protocol | net48;net10.0-windows | ProtocolErrorCodeTests | planli |
| N37-022 | `ProtocolErrorCodeConverter` | class | ProtocolErrorCode bicim donusumu | VersaCoder.Protocol | net48;net10.0-windows | ProtocolErrorCodeTests | planli |
| N37-023 | `ProtocolErrorCodeTests` | class | ProtocolErrorCode testleri (xUnit) | VersaCoder.Protocol | net48;net10.0-windows | - | planli |
| N37-024 | `ProtocolErrorCodeBuilder` | class | ProtocolErrorCode uretici (builder) | VersaCoder.Protocol | net48;net10.0-windows | ProtocolErrorCodeTests | planli |
| N37-025 | `IProtocolViolation` | interface | ProtocolViolation sozlesmesi (port) | VersaCoder.Protocol | net48;net10.0-windows | ProtocolViolationTests | planli |
| N37-026 | `ProtocolViolation` | class | ProtocolViolation cekirdek uygulamasi | VersaCoder.Protocol | net48;net10.0-windows | ProtocolViolationTests | planli |
| N37-027 | `ProtocolViolationDto` | record | ProtocolViolation veri transferi modeli | VersaCoder.Protocol | net48;net10.0-windows | ProtocolViolationTests | planli |
| N37-028 | `ProtocolViolationMapper` | class | ProtocolViolation <-> DTO esleme | VersaCoder.Protocol | net48;net10.0-windows | ProtocolViolationTests | planli |
| N37-029 | `ProtocolViolationValidator` | class | ProtocolViolation dogrulama kurallari | VersaCoder.Protocol | net48;net10.0-windows | ProtocolViolationTests | planli |
| N37-030 | `ProtocolViolationFactory` | class | ProtocolViolation uretimi ve varsayilanlari | VersaCoder.Protocol | net48;net10.0-windows | ProtocolViolationTests | planli |
| N37-031 | `ProtocolViolationOptions` | record | ProtocolViolation yapilandirmasi | VersaCoder.Protocol | net48;net10.0-windows | ProtocolViolationTests | planli |
| N37-032 | `ProtocolViolationProvider` | class | ProtocolViolation saglayici uygulamasi | VersaCoder.Protocol | net48;net10.0-windows | ProtocolViolationTests | planli |
| N37-033 | `ProtocolViolationRegistry` | class | ProtocolViolation kayit/katalog yonetimi | VersaCoder.Protocol | net48;net10.0-windows | ProtocolViolationTests | planli |
| N37-034 | `ProtocolViolationConverter` | class | ProtocolViolation bicim donusumu | VersaCoder.Protocol | net48;net10.0-windows | ProtocolViolationTests | planli |
| N37-035 | `ProtocolViolationTests` | class | ProtocolViolation testleri (xUnit) | VersaCoder.Protocol | net48;net10.0-windows | - | planli |
| N37-036 | `ProtocolViolationBuilder` | class | ProtocolViolation uretici (builder) | VersaCoder.Protocol | net48;net10.0-windows | ProtocolViolationTests | planli |
| N37-037 | `IErrorCatalog` | interface | ErrorCatalog sozlesmesi (port) | VersaCoder.Protocol | net48;net10.0-windows | ErrorCatalogTests | planli |
| N37-038 | `ErrorCatalog` | class | ErrorCatalog cekirdek uygulamasi | VersaCoder.Protocol | net48;net10.0-windows | ErrorCatalogTests | planli |
| N37-039 | `ErrorCatalogDto` | record | ErrorCatalog veri transferi modeli | VersaCoder.Protocol | net48;net10.0-windows | ErrorCatalogTests | planli |
| N37-040 | `ErrorCatalogMapper` | class | ErrorCatalog <-> DTO esleme | VersaCoder.Protocol | net48;net10.0-windows | ErrorCatalogTests | planli |
| N37-041 | `ErrorCatalogValidator` | class | ErrorCatalog dogrulama kurallari | VersaCoder.Protocol | net48;net10.0-windows | ErrorCatalogTests | planli |
| N37-042 | `ErrorCatalogFactory` | class | ErrorCatalog uretimi ve varsayilanlari | VersaCoder.Protocol | net48;net10.0-windows | ErrorCatalogTests | planli |
| N37-043 | `ErrorCatalogOptions` | record | ErrorCatalog yapilandirmasi | VersaCoder.Protocol | net48;net10.0-windows | ErrorCatalogTests | planli |
| N37-044 | `ErrorCatalogProvider` | class | ErrorCatalog saglayici uygulamasi | VersaCoder.Protocol | net48;net10.0-windows | ErrorCatalogTests | planli |
| N37-045 | `ErrorCatalogRegistry` | class | ErrorCatalog kayit/katalog yonetimi | VersaCoder.Protocol | net48;net10.0-windows | ErrorCatalogTests | planli |
| N37-046 | `ErrorCatalogConverter` | class | ErrorCatalog bicim donusumu | VersaCoder.Protocol | net48;net10.0-windows | ErrorCatalogTests | planli |
| N37-047 | `ErrorCatalogTests` | class | ErrorCatalog testleri (xUnit) | VersaCoder.Protocol | net48;net10.0-windows | - | planli |
| N37-048 | `ErrorCatalogBuilder` | class | ErrorCatalog uretici (builder) | VersaCoder.Protocol | net48;net10.0-windows | ErrorCatalogTests | planli |
| N37-049 | `IVersionNegotiation` | interface | VersionNegotiation sozlesmesi (port) | VersaCoder.Protocol | net48;net10.0-windows | VersionNegotiationTests | planli |
| N37-050 | `VersionNegotiation` | class | VersionNegotiation cekirdek uygulamasi | VersaCoder.Protocol | net48;net10.0-windows | VersionNegotiationTests | planli |
| N37-051 | `VersionNegotiationDto` | record | VersionNegotiation veri transferi modeli | VersaCoder.Protocol | net48;net10.0-windows | VersionNegotiationTests | planli |
| N37-052 | `VersionNegotiationMapper` | class | VersionNegotiation <-> DTO esleme | VersaCoder.Protocol | net48;net10.0-windows | VersionNegotiationTests | planli |
| N37-053 | `VersionNegotiationValidator` | class | VersionNegotiation dogrulama kurallari | VersaCoder.Protocol | net48;net10.0-windows | VersionNegotiationTests | planli |
| N37-054 | `VersionNegotiationFactory` | class | VersionNegotiation uretimi ve varsayilanlari | VersaCoder.Protocol | net48;net10.0-windows | VersionNegotiationTests | planli |
| N37-055 | `VersionNegotiationOptions` | record | VersionNegotiation yapilandirmasi | VersaCoder.Protocol | net48;net10.0-windows | VersionNegotiationTests | planli |
| N37-056 | `VersionNegotiationProvider` | class | VersionNegotiation saglayici uygulamasi | VersaCoder.Protocol | net48;net10.0-windows | VersionNegotiationTests | planli |
| N37-057 | `VersionNegotiationRegistry` | class | VersionNegotiation kayit/katalog yonetimi | VersaCoder.Protocol | net48;net10.0-windows | VersionNegotiationTests | planli |
| N37-058 | `VersionNegotiationConverter` | class | VersionNegotiation bicim donusumu | VersaCoder.Protocol | net48;net10.0-windows | VersionNegotiationTests | planli |
| N37-059 | `VersionNegotiationTests` | class | VersionNegotiation testleri (xUnit) | VersaCoder.Protocol | net48;net10.0-windows | - | planli |
| N37-060 | `VersionNegotiationBuilder` | class | VersionNegotiation uretici (builder) | VersaCoder.Protocol | net48;net10.0-windows | VersionNegotiationTests | planli |
| N37-061 | `ICompatMatrix` | interface | CompatMatrix sozlesmesi (port) | VersaCoder.Protocol | net48;net10.0-windows | CompatMatrixTests | planli |
| N37-062 | `CompatMatrix` | class | CompatMatrix cekirdek uygulamasi | VersaCoder.Protocol | net48;net10.0-windows | CompatMatrixTests | planli |
| N37-063 | `CompatMatrixDto` | record | CompatMatrix veri transferi modeli | VersaCoder.Protocol | net48;net10.0-windows | CompatMatrixTests | planli |
| N37-064 | `CompatMatrixMapper` | class | CompatMatrix <-> DTO esleme | VersaCoder.Protocol | net48;net10.0-windows | CompatMatrixTests | planli |
| N37-065 | `CompatMatrixValidator` | class | CompatMatrix dogrulama kurallari | VersaCoder.Protocol | net48;net10.0-windows | CompatMatrixTests | planli |
| N37-066 | `CompatMatrixFactory` | class | CompatMatrix uretimi ve varsayilanlari | VersaCoder.Protocol | net48;net10.0-windows | CompatMatrixTests | planli |
| N37-067 | `CompatMatrixOptions` | record | CompatMatrix yapilandirmasi | VersaCoder.Protocol | net48;net10.0-windows | CompatMatrixTests | planli |
| N37-068 | `CompatMatrixProvider` | class | CompatMatrix saglayici uygulamasi | VersaCoder.Protocol | net48;net10.0-windows | CompatMatrixTests | planli |
| N37-069 | `CompatMatrixRegistry` | class | CompatMatrix kayit/katalog yonetimi | VersaCoder.Protocol | net48;net10.0-windows | CompatMatrixTests | planli |
| N37-070 | `CompatMatrixConverter` | class | CompatMatrix bicim donusumu | VersaCoder.Protocol | net48;net10.0-windows | CompatMatrixTests | planli |
| N37-071 | `CompatMatrixTests` | class | CompatMatrix testleri (xUnit) | VersaCoder.Protocol | net48;net10.0-windows | - | planli |
| N37-072 | `CompatMatrixBuilder` | class | CompatMatrix uretici (builder) | VersaCoder.Protocol | net48;net10.0-windows | CompatMatrixTests | planli |
| N37-073 | `IBreakingChange` | interface | BreakingChange sozlesmesi (port) | VersaCoder.Protocol | net48;net10.0-windows | BreakingChangeTests | planli |
| N37-074 | `BreakingChange` | class | BreakingChange cekirdek uygulamasi | VersaCoder.Protocol | net48;net10.0-windows | BreakingChangeTests | planli |
| N37-075 | `BreakingChangeDto` | record | BreakingChange veri transferi modeli | VersaCoder.Protocol | net48;net10.0-windows | BreakingChangeTests | planli |
| N37-076 | `BreakingChangeMapper` | class | BreakingChange <-> DTO esleme | VersaCoder.Protocol | net48;net10.0-windows | BreakingChangeTests | planli |
| N37-077 | `BreakingChangeValidator` | class | BreakingChange dogrulama kurallari | VersaCoder.Protocol | net48;net10.0-windows | BreakingChangeTests | planli |
| N37-078 | `BreakingChangeFactory` | class | BreakingChange uretimi ve varsayilanlari | VersaCoder.Protocol | net48;net10.0-windows | BreakingChangeTests | planli |
| N37-079 | `BreakingChangeOptions` | record | BreakingChange yapilandirmasi | VersaCoder.Protocol | net48;net10.0-windows | BreakingChangeTests | planli |
| N37-080 | `BreakingChangeProvider` | class | BreakingChange saglayici uygulamasi | VersaCoder.Protocol | net48;net10.0-windows | BreakingChangeTests | planli |
| N37-081 | `BreakingChangeRegistry` | class | BreakingChange kayit/katalog yonetimi | VersaCoder.Protocol | net48;net10.0-windows | BreakingChangeTests | planli |
| N37-082 | `BreakingChangeConverter` | class | BreakingChange bicim donusumu | VersaCoder.Protocol | net48;net10.0-windows | BreakingChangeTests | planli |
| N37-083 | `BreakingChangeTests` | class | BreakingChange testleri (xUnit) | VersaCoder.Protocol | net48;net10.0-windows | - | planli |
| N37-084 | `BreakingChangeBuilder` | class | BreakingChange uretici (builder) | VersaCoder.Protocol | net48;net10.0-windows | BreakingChangeTests | planli |
| N37-085 | `IVersionGuard` | interface | VersionGuard sozlesmesi (port) | VersaCoder.Protocol | net48;net10.0-windows | VersionGuardTests | planli |
| N37-086 | `VersionGuard` | class | VersionGuard cekirdek uygulamasi | VersaCoder.Protocol | net48;net10.0-windows | VersionGuardTests | planli |
| N37-087 | `VersionGuardDto` | record | VersionGuard veri transferi modeli | VersaCoder.Protocol | net48;net10.0-windows | VersionGuardTests | planli |
| N37-088 | `VersionGuardMapper` | class | VersionGuard <-> DTO esleme | VersaCoder.Protocol | net48;net10.0-windows | VersionGuardTests | planli |
| N37-089 | `VersionGuardValidator` | class | VersionGuard dogrulama kurallari | VersaCoder.Protocol | net48;net10.0-windows | VersionGuardTests | planli |
| N37-090 | `VersionGuardFactory` | class | VersionGuard uretimi ve varsayilanlari | VersaCoder.Protocol | net48;net10.0-windows | VersionGuardTests | planli |
| N37-091 | `VersionGuardOptions` | record | VersionGuard yapilandirmasi | VersaCoder.Protocol | net48;net10.0-windows | VersionGuardTests | planli |
| N37-092 | `VersionGuardProvider` | class | VersionGuard saglayici uygulamasi | VersaCoder.Protocol | net48;net10.0-windows | VersionGuardTests | planli |
| N37-093 | `VersionGuardRegistry` | class | VersionGuard kayit/katalog yonetimi | VersaCoder.Protocol | net48;net10.0-windows | VersionGuardTests | planli |
| N37-094 | `VersionGuardConverter` | class | VersionGuard bicim donusumu | VersaCoder.Protocol | net48;net10.0-windows | VersionGuardTests | planli |
| N37-095 | `VersionGuardTests` | class | VersionGuard testleri (xUnit) | VersaCoder.Protocol | net48;net10.0-windows | - | planli |
| N37-096 | `VersionGuardBuilder` | class | VersionGuard uretici (builder) | VersaCoder.Protocol | net48;net10.0-windows | VersionGuardTests | planli |
| N37-097 | `IMigrationHint` | interface | MigrationHint sozlesmesi (port) | VersaCoder.Protocol | net48;net10.0-windows | MigrationHintTests | planli |
| N37-098 | `MigrationHint` | class | MigrationHint cekirdek uygulamasi | VersaCoder.Protocol | net48;net10.0-windows | MigrationHintTests | planli |
| N37-099 | `MigrationHintDto` | record | MigrationHint veri transferi modeli | VersaCoder.Protocol | net48;net10.0-windows | MigrationHintTests | planli |
| N37-100 | `MigrationHintMapper` | class | MigrationHint <-> DTO esleme | VersaCoder.Protocol | net48;net10.0-windows | MigrationHintTests | planli |
| N37-101 | `MigrationHintValidator` | class | MigrationHint dogrulama kurallari | VersaCoder.Protocol | net48;net10.0-windows | MigrationHintTests | planli |
| N37-102 | `MigrationHintFactory` | class | MigrationHint uretimi ve varsayilanlari | VersaCoder.Protocol | net48;net10.0-windows | MigrationHintTests | planli |
| N37-103 | `MigrationHintOptions` | record | MigrationHint yapilandirmasi | VersaCoder.Protocol | net48;net10.0-windows | MigrationHintTests | planli |
| N37-104 | `MigrationHintProvider` | class | MigrationHint saglayici uygulamasi | VersaCoder.Protocol | net48;net10.0-windows | MigrationHintTests | planli |
| N37-105 | `MigrationHintRegistry` | class | MigrationHint kayit/katalog yonetimi | VersaCoder.Protocol | net48;net10.0-windows | MigrationHintTests | planli |
| N37-106 | `MigrationHintConverter` | class | MigrationHint bicim donusumu | VersaCoder.Protocol | net48;net10.0-windows | MigrationHintTests | planli |
| N37-107 | `MigrationHintTests` | class | MigrationHint testleri (xUnit) | VersaCoder.Protocol | net48;net10.0-windows | - | planli |
| N37-108 | `MigrationHintBuilder` | class | MigrationHint uretici (builder) | VersaCoder.Protocol | net48;net10.0-windows | MigrationHintTests | planli |
| N37-109 | `IErrorSerializer` | interface | ErrorSerializer sozlesmesi (port) | VersaCoder.Protocol | net48;net10.0-windows | ErrorSerializerTests | planli |
| N37-110 | `ErrorSerializer` | class | ErrorSerializer cekirdek uygulamasi | VersaCoder.Protocol | net48;net10.0-windows | ErrorSerializerTests | planli |
| N37-111 | `ErrorSerializerDto` | record | ErrorSerializer veri transferi modeli | VersaCoder.Protocol | net48;net10.0-windows | ErrorSerializerTests | planli |
| N37-112 | `ErrorSerializerMapper` | class | ErrorSerializer <-> DTO esleme | VersaCoder.Protocol | net48;net10.0-windows | ErrorSerializerTests | planli |
| N37-113 | `ErrorSerializerValidator` | class | ErrorSerializer dogrulama kurallari | VersaCoder.Protocol | net48;net10.0-windows | ErrorSerializerTests | planli |
| N37-114 | `ErrorSerializerFactory` | class | ErrorSerializer uretimi ve varsayilanlari | VersaCoder.Protocol | net48;net10.0-windows | ErrorSerializerTests | planli |
| N37-115 | `ErrorSerializerOptions` | record | ErrorSerializer yapilandirmasi | VersaCoder.Protocol | net48;net10.0-windows | ErrorSerializerTests | planli |
| N37-116 | `ErrorSerializerProvider` | class | ErrorSerializer saglayici uygulamasi | VersaCoder.Protocol | net48;net10.0-windows | ErrorSerializerTests | planli |
| N37-117 | `ErrorSerializerRegistry` | class | ErrorSerializer kayit/katalog yonetimi | VersaCoder.Protocol | net48;net10.0-windows | ErrorSerializerTests | planli |
| N37-118 | `ErrorSerializerConverter` | class | ErrorSerializer bicim donusumu | VersaCoder.Protocol | net48;net10.0-windows | ErrorSerializerTests | planli |
| N37-119 | `ErrorSerializerTests` | class | ErrorSerializer testleri (xUnit) | VersaCoder.Protocol | net48;net10.0-windows | - | planli |
| N37-120 | `ErrorSerializerBuilder` | class | ErrorSerializer uretici (builder) | VersaCoder.Protocol | net48;net10.0-windows | ErrorSerializerTests | planli |
