---
title: Katalog Dugumu N33 - Arac Semasi
type: catalog-node
category: l5-protocol
status: needs-validation
version: 1.0.0
updated: 2026-10-04
authority: derived
scope: protokol arac semasi ve kayit dogrulama
---

# Katalog Dugumu N33 - Arac Semasi

**Zorunlu Baglantilar:** index.md | node-inventory.md | ../index.md | ../../.decisions/accepted/ADR-028-layered-baseline-9-projects.md | ../../.decisions/accepted/ADR-013-layered-architecture-52-nodes.md

> Dugum, katman sozlesmesini cevaplar ve kanitini baglar.

## §0 Ozet (Answer First)

| Alan | Deger |
| --- | --- |
| Katman | l5 Protocol |
| Proje | VersaCoder.Protocol |
| Kapsam | protokol arac semasi ve kayit dogrulama |
| Kaynak | C:/www/versacoder/versacoder/.ai/architecture/catalog/l5-protocol/N33-tool-schema.md |
| Sorumluluk | Dugum, katman sozlesmesini cevaplar ve kanitini baglar. |

- Arac cagrisi sozlesmesi JSON-Sema ile tanimlanir ve calisma aninda dogrulanir.
- Sema; arac adi, parametre ve sonuc semasi icerir, bilinmeyen alanlar reddedilir.
- Uyum katmani bazi alanlari esler, kalani hata olarak bildirir.
- Sema surumle uyum matrisine baglidir, kirilan alan yeni surumde isim degistirir.
- Kayitlar en az 6 aile 120 ornek ile kanitlanir, gerisi namludur (kayit yok).

## §1 YAPAR / YAPMAZ

| YAPAR | YAPMAZ |
| --- | --- |
| Arac semasini ve parametre dogrulamayi uretir | Arac cagrisini calistirir (l4 is) |
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
| 1 | ToolDefinition | Arac tanimi ve meta veri |
| 2 | ToolParameterSchema | Parametre semasi kurallari |
| 3 | ToolRegistry | Arac kaydi ve arama |
| 4 | ToolJsonSchema | JSON-Sema uretimi |
| 5 | ToolCallSchema | Cagri paketi dogrulama |
| 6 | ToolResultSchema | Sonuc paketi dogrulama |
| 7 | SchemaValidator | Sema kontrol motoru |
| 8 | ToolPermissionMap | Arac izin eslemesi |
| 9 | ToolCatalog | Arac katalog listesi |
| 10 | ToolSignature | Imza karsilastirma |

## §4 Ornek Kayit (seri 120)

| Kayit | Aile | Alan1 | Alan2 |
| --- | --- | --- | --- |
| 1 | ToolDefinition | git.status | repo durumu |
| 2 | ToolDefinition | fs.readFile | dosya oku |
| 3 | ToolParameterSchema | parametre zorunlu | string |
| 4 | ToolParameterSchema | sonuc dondurur | boolean |
| 5 | ToolRegistry | cagri dogrulanir | ok |
| 6 | ToolJsonSchema | izin haritasi | ok |

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
| SemaDogrulamaOlumluTesti | ToolDefinition | gecerli |
| SemaDogrulamaOlumsuzTesti | ToolDefinition | reddedilen |
| AracKaydiAramaTesti | ToolCallSchema | bulunan |
| ImzaKarsilastirmaTesti | ToolCallSchema | eslesen |

## §7 Riskler

| Risk | Etki | Onlem |
| --- | --- | --- |
| Sema gevsekligi | yanlis cagri | strict mod |
| Sema uyumsuzlugu | kirilma | surum matrisi |
| Kayit eksikligi | kanit yok | 120 hedefi |

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
| Dosya | N33-tool-schema.md |
| Klasor ornegi | C:\versacoder\src\VersaCoder.Protocol |
| Ornek kayit | 6/6 hedef (seri 120 icin) |
| Test | 4/4 hedef |

kayit tablosu §10'e port edildi (kaynak vault 53 dosya / 5440 kayit); esik 5000 karsilanir.


## §10 Kayit Tablosu (ADR-013 2.2 - kaynak vault port)

| N33-001 | `IToolDefinition` | interface | ToolDefinition sozlesmesi (port) | VersaCoder.Protocol | net48;net10.0-windows | ToolDefinitionTests | planli |
| N33-002 | `ToolDefinition` | class | ToolDefinition cekirdek uygulamasi | VersaCoder.Protocol | net48;net10.0-windows | ToolDefinitionTests | planli |
| N33-003 | `ToolDefinitionDto` | record | ToolDefinition veri transferi modeli | VersaCoder.Protocol | net48;net10.0-windows | ToolDefinitionTests | planli |
| N33-004 | `ToolDefinitionMapper` | class | ToolDefinition <-> DTO esleme | VersaCoder.Protocol | net48;net10.0-windows | ToolDefinitionTests | planli |
| N33-005 | `ToolDefinitionValidator` | class | ToolDefinition dogrulama kurallari | VersaCoder.Protocol | net48;net10.0-windows | ToolDefinitionTests | planli |
| N33-006 | `ToolDefinitionFactory` | class | ToolDefinition uretimi ve varsayilanlari | VersaCoder.Protocol | net48;net10.0-windows | ToolDefinitionTests | planli |
| N33-007 | `ToolDefinitionOptions` | record | ToolDefinition yapilandirmasi | VersaCoder.Protocol | net48;net10.0-windows | ToolDefinitionTests | planli |
| N33-008 | `ToolDefinitionProvider` | class | ToolDefinition saglayici uygulamasi | VersaCoder.Protocol | net48;net10.0-windows | ToolDefinitionTests | planli |
| N33-009 | `ToolDefinitionRegistry` | class | ToolDefinition kayit/katalog yonetimi | VersaCoder.Protocol | net48;net10.0-windows | ToolDefinitionTests | planli |
| N33-010 | `ToolDefinitionConverter` | class | ToolDefinition bicim donusumu | VersaCoder.Protocol | net48;net10.0-windows | ToolDefinitionTests | planli |
| N33-011 | `ToolDefinitionTests` | class | ToolDefinition testleri (xUnit) | VersaCoder.Protocol | net48;net10.0-windows | - | planli |
| N33-012 | `ToolDefinitionBuilder` | class | ToolDefinition uretici (builder) | VersaCoder.Protocol | net48;net10.0-windows | ToolDefinitionTests | planli |
| N33-013 | `IToolParameterSchema` | interface | ToolParameterSchema sozlesmesi (port) | VersaCoder.Protocol | net48;net10.0-windows | ToolParameterSchemaTests | planli |
| N33-014 | `ToolParameterSchema` | class | ToolParameterSchema cekirdek uygulamasi | VersaCoder.Protocol | net48;net10.0-windows | ToolParameterSchemaTests | planli |
| N33-015 | `ToolParameterSchemaDto` | record | ToolParameterSchema veri transferi modeli | VersaCoder.Protocol | net48;net10.0-windows | ToolParameterSchemaTests | planli |
| N33-016 | `ToolParameterSchemaMapper` | class | ToolParameterSchema <-> DTO esleme | VersaCoder.Protocol | net48;net10.0-windows | ToolParameterSchemaTests | planli |
| N33-017 | `ToolParameterSchemaValidator` | class | ToolParameterSchema dogrulama kurallari | VersaCoder.Protocol | net48;net10.0-windows | ToolParameterSchemaTests | planli |
| N33-018 | `ToolParameterSchemaFactory` | class | ToolParameterSchema uretimi ve varsayilanlari | VersaCoder.Protocol | net48;net10.0-windows | ToolParameterSchemaTests | planli |
| N33-019 | `ToolParameterSchemaOptions` | record | ToolParameterSchema yapilandirmasi | VersaCoder.Protocol | net48;net10.0-windows | ToolParameterSchemaTests | planli |
| N33-020 | `ToolParameterSchemaProvider` | class | ToolParameterSchema saglayici uygulamasi | VersaCoder.Protocol | net48;net10.0-windows | ToolParameterSchemaTests | planli |
| N33-021 | `ToolParameterSchemaRegistry` | class | ToolParameterSchema kayit/katalog yonetimi | VersaCoder.Protocol | net48;net10.0-windows | ToolParameterSchemaTests | planli |
| N33-022 | `ToolParameterSchemaConverter` | class | ToolParameterSchema bicim donusumu | VersaCoder.Protocol | net48;net10.0-windows | ToolParameterSchemaTests | planli |
| N33-023 | `ToolParameterSchemaTests` | class | ToolParameterSchema testleri (xUnit) | VersaCoder.Protocol | net48;net10.0-windows | - | planli |
| N33-024 | `ToolParameterSchemaBuilder` | class | ToolParameterSchema uretici (builder) | VersaCoder.Protocol | net48;net10.0-windows | ToolParameterSchemaTests | planli |
| N33-025 | `IToolRegistry` | interface | ToolRegistry sozlesmesi (port) | VersaCoder.Protocol | net48;net10.0-windows | ToolRegistryTests | planli |
| N33-026 | `ToolRegistry` | class | ToolRegistry cekirdek uygulamasi | VersaCoder.Protocol | net48;net10.0-windows | ToolRegistryTests | planli |
| N33-027 | `ToolRegistryDto` | record | ToolRegistry veri transferi modeli | VersaCoder.Protocol | net48;net10.0-windows | ToolRegistryTests | planli |
| N33-028 | `ToolRegistryMapper` | class | ToolRegistry <-> DTO esleme | VersaCoder.Protocol | net48;net10.0-windows | ToolRegistryTests | planli |
| N33-029 | `ToolRegistryValidator` | class | ToolRegistry dogrulama kurallari | VersaCoder.Protocol | net48;net10.0-windows | ToolRegistryTests | planli |
| N33-030 | `ToolRegistryFactory` | class | ToolRegistry uretimi ve varsayilanlari | VersaCoder.Protocol | net48;net10.0-windows | ToolRegistryTests | planli |
| N33-031 | `ToolRegistryOptions` | record | ToolRegistry yapilandirmasi | VersaCoder.Protocol | net48;net10.0-windows | ToolRegistryTests | planli |
| N33-032 | `ToolRegistryProvider` | class | ToolRegistry saglayici uygulamasi | VersaCoder.Protocol | net48;net10.0-windows | ToolRegistryTests | planli |
| N33-033 | `ToolRegistryRegistry` | class | ToolRegistry kayit/katalog yonetimi | VersaCoder.Protocol | net48;net10.0-windows | ToolRegistryTests | planli |
| N33-034 | `ToolRegistryConverter` | class | ToolRegistry bicim donusumu | VersaCoder.Protocol | net48;net10.0-windows | ToolRegistryTests | planli |
| N33-035 | `ToolRegistryTests` | class | ToolRegistry testleri (xUnit) | VersaCoder.Protocol | net48;net10.0-windows | - | planli |
| N33-036 | `ToolRegistryBuilder` | class | ToolRegistry uretici (builder) | VersaCoder.Protocol | net48;net10.0-windows | ToolRegistryTests | planli |
| N33-037 | `IToolJsonSchema` | interface | ToolJsonSchema sozlesmesi (port) | VersaCoder.Protocol | net48;net10.0-windows | ToolJsonSchemaTests | planli |
| N33-038 | `ToolJsonSchema` | class | ToolJsonSchema cekirdek uygulamasi | VersaCoder.Protocol | net48;net10.0-windows | ToolJsonSchemaTests | planli |
| N33-039 | `ToolJsonSchemaDto` | record | ToolJsonSchema veri transferi modeli | VersaCoder.Protocol | net48;net10.0-windows | ToolJsonSchemaTests | planli |
| N33-040 | `ToolJsonSchemaMapper` | class | ToolJsonSchema <-> DTO esleme | VersaCoder.Protocol | net48;net10.0-windows | ToolJsonSchemaTests | planli |
| N33-041 | `ToolJsonSchemaValidator` | class | ToolJsonSchema dogrulama kurallari | VersaCoder.Protocol | net48;net10.0-windows | ToolJsonSchemaTests | planli |
| N33-042 | `ToolJsonSchemaFactory` | class | ToolJsonSchema uretimi ve varsayilanlari | VersaCoder.Protocol | net48;net10.0-windows | ToolJsonSchemaTests | planli |
| N33-043 | `ToolJsonSchemaOptions` | record | ToolJsonSchema yapilandirmasi | VersaCoder.Protocol | net48;net10.0-windows | ToolJsonSchemaTests | planli |
| N33-044 | `ToolJsonSchemaProvider` | class | ToolJsonSchema saglayici uygulamasi | VersaCoder.Protocol | net48;net10.0-windows | ToolJsonSchemaTests | planli |
| N33-045 | `ToolJsonSchemaRegistry` | class | ToolJsonSchema kayit/katalog yonetimi | VersaCoder.Protocol | net48;net10.0-windows | ToolJsonSchemaTests | planli |
| N33-046 | `ToolJsonSchemaConverter` | class | ToolJsonSchema bicim donusumu | VersaCoder.Protocol | net48;net10.0-windows | ToolJsonSchemaTests | planli |
| N33-047 | `ToolJsonSchemaTests` | class | ToolJsonSchema testleri (xUnit) | VersaCoder.Protocol | net48;net10.0-windows | - | planli |
| N33-048 | `ToolJsonSchemaBuilder` | class | ToolJsonSchema uretici (builder) | VersaCoder.Protocol | net48;net10.0-windows | ToolJsonSchemaTests | planli |
| N33-049 | `IToolCallSchema` | interface | ToolCallSchema sozlesmesi (port) | VersaCoder.Protocol | net48;net10.0-windows | ToolCallSchemaTests | planli |
| N33-050 | `ToolCallSchema` | class | ToolCallSchema cekirdek uygulamasi | VersaCoder.Protocol | net48;net10.0-windows | ToolCallSchemaTests | planli |
| N33-051 | `ToolCallSchemaDto` | record | ToolCallSchema veri transferi modeli | VersaCoder.Protocol | net48;net10.0-windows | ToolCallSchemaTests | planli |
| N33-052 | `ToolCallSchemaMapper` | class | ToolCallSchema <-> DTO esleme | VersaCoder.Protocol | net48;net10.0-windows | ToolCallSchemaTests | planli |
| N33-053 | `ToolCallSchemaValidator` | class | ToolCallSchema dogrulama kurallari | VersaCoder.Protocol | net48;net10.0-windows | ToolCallSchemaTests | planli |
| N33-054 | `ToolCallSchemaFactory` | class | ToolCallSchema uretimi ve varsayilanlari | VersaCoder.Protocol | net48;net10.0-windows | ToolCallSchemaTests | planli |
| N33-055 | `ToolCallSchemaOptions` | record | ToolCallSchema yapilandirmasi | VersaCoder.Protocol | net48;net10.0-windows | ToolCallSchemaTests | planli |
| N33-056 | `ToolCallSchemaProvider` | class | ToolCallSchema saglayici uygulamasi | VersaCoder.Protocol | net48;net10.0-windows | ToolCallSchemaTests | planli |
| N33-057 | `ToolCallSchemaRegistry` | class | ToolCallSchema kayit/katalog yonetimi | VersaCoder.Protocol | net48;net10.0-windows | ToolCallSchemaTests | planli |
| N33-058 | `ToolCallSchemaConverter` | class | ToolCallSchema bicim donusumu | VersaCoder.Protocol | net48;net10.0-windows | ToolCallSchemaTests | planli |
| N33-059 | `ToolCallSchemaTests` | class | ToolCallSchema testleri (xUnit) | VersaCoder.Protocol | net48;net10.0-windows | - | planli |
| N33-060 | `ToolCallSchemaBuilder` | class | ToolCallSchema uretici (builder) | VersaCoder.Protocol | net48;net10.0-windows | ToolCallSchemaTests | planli |
| N33-061 | `IToolResultSchema` | interface | ToolResultSchema sozlesmesi (port) | VersaCoder.Protocol | net48;net10.0-windows | ToolResultSchemaTests | planli |
| N33-062 | `ToolResultSchema` | class | ToolResultSchema cekirdek uygulamasi | VersaCoder.Protocol | net48;net10.0-windows | ToolResultSchemaTests | planli |
| N33-063 | `ToolResultSchemaDto` | record | ToolResultSchema veri transferi modeli | VersaCoder.Protocol | net48;net10.0-windows | ToolResultSchemaTests | planli |
| N33-064 | `ToolResultSchemaMapper` | class | ToolResultSchema <-> DTO esleme | VersaCoder.Protocol | net48;net10.0-windows | ToolResultSchemaTests | planli |
| N33-065 | `ToolResultSchemaValidator` | class | ToolResultSchema dogrulama kurallari | VersaCoder.Protocol | net48;net10.0-windows | ToolResultSchemaTests | planli |
| N33-066 | `ToolResultSchemaFactory` | class | ToolResultSchema uretimi ve varsayilanlari | VersaCoder.Protocol | net48;net10.0-windows | ToolResultSchemaTests | planli |
| N33-067 | `ToolResultSchemaOptions` | record | ToolResultSchema yapilandirmasi | VersaCoder.Protocol | net48;net10.0-windows | ToolResultSchemaTests | planli |
| N33-068 | `ToolResultSchemaProvider` | class | ToolResultSchema saglayici uygulamasi | VersaCoder.Protocol | net48;net10.0-windows | ToolResultSchemaTests | planli |
| N33-069 | `ToolResultSchemaRegistry` | class | ToolResultSchema kayit/katalog yonetimi | VersaCoder.Protocol | net48;net10.0-windows | ToolResultSchemaTests | planli |
| N33-070 | `ToolResultSchemaConverter` | class | ToolResultSchema bicim donusumu | VersaCoder.Protocol | net48;net10.0-windows | ToolResultSchemaTests | planli |
| N33-071 | `ToolResultSchemaTests` | class | ToolResultSchema testleri (xUnit) | VersaCoder.Protocol | net48;net10.0-windows | - | planli |
| N33-072 | `ToolResultSchemaBuilder` | class | ToolResultSchema uretici (builder) | VersaCoder.Protocol | net48;net10.0-windows | ToolResultSchemaTests | planli |
| N33-073 | `ISchemaValidator` | interface | SchemaValidator sozlesmesi (port) | VersaCoder.Protocol | net48;net10.0-windows | SchemaValidatorTests | planli |
| N33-074 | `SchemaValidator` | class | SchemaValidator cekirdek uygulamasi | VersaCoder.Protocol | net48;net10.0-windows | SchemaValidatorTests | planli |
| N33-075 | `SchemaValidatorDto` | record | SchemaValidator veri transferi modeli | VersaCoder.Protocol | net48;net10.0-windows | SchemaValidatorTests | planli |
| N33-076 | `SchemaValidatorMapper` | class | SchemaValidator <-> DTO esleme | VersaCoder.Protocol | net48;net10.0-windows | SchemaValidatorTests | planli |
| N33-077 | `SchemaValidatorValidator` | class | SchemaValidator dogrulama kurallari | VersaCoder.Protocol | net48;net10.0-windows | SchemaValidatorTests | planli |
| N33-078 | `SchemaValidatorFactory` | class | SchemaValidator uretimi ve varsayilanlari | VersaCoder.Protocol | net48;net10.0-windows | SchemaValidatorTests | planli |
| N33-079 | `SchemaValidatorOptions` | record | SchemaValidator yapilandirmasi | VersaCoder.Protocol | net48;net10.0-windows | SchemaValidatorTests | planli |
| N33-080 | `SchemaValidatorProvider` | class | SchemaValidator saglayici uygulamasi | VersaCoder.Protocol | net48;net10.0-windows | SchemaValidatorTests | planli |
| N33-081 | `SchemaValidatorRegistry` | class | SchemaValidator kayit/katalog yonetimi | VersaCoder.Protocol | net48;net10.0-windows | SchemaValidatorTests | planli |
| N33-082 | `SchemaValidatorConverter` | class | SchemaValidator bicim donusumu | VersaCoder.Protocol | net48;net10.0-windows | SchemaValidatorTests | planli |
| N33-083 | `SchemaValidatorTests` | class | SchemaValidator testleri (xUnit) | VersaCoder.Protocol | net48;net10.0-windows | - | planli |
| N33-084 | `SchemaValidatorBuilder` | class | SchemaValidator uretici (builder) | VersaCoder.Protocol | net48;net10.0-windows | SchemaValidatorTests | planli |
| N33-085 | `IToolPermissionMap` | interface | ToolPermissionMap sozlesmesi (port) | VersaCoder.Protocol | net48;net10.0-windows | ToolPermissionMapTests | planli |
| N33-086 | `ToolPermissionMap` | class | ToolPermissionMap cekirdek uygulamasi | VersaCoder.Protocol | net48;net10.0-windows | ToolPermissionMapTests | planli |
| N33-087 | `ToolPermissionMapDto` | record | ToolPermissionMap veri transferi modeli | VersaCoder.Protocol | net48;net10.0-windows | ToolPermissionMapTests | planli |
| N33-088 | `ToolPermissionMapMapper` | class | ToolPermissionMap <-> DTO esleme | VersaCoder.Protocol | net48;net10.0-windows | ToolPermissionMapTests | planli |
| N33-089 | `ToolPermissionMapValidator` | class | ToolPermissionMap dogrulama kurallari | VersaCoder.Protocol | net48;net10.0-windows | ToolPermissionMapTests | planli |
| N33-090 | `ToolPermissionMapFactory` | class | ToolPermissionMap uretimi ve varsayilanlari | VersaCoder.Protocol | net48;net10.0-windows | ToolPermissionMapTests | planli |
| N33-091 | `ToolPermissionMapOptions` | record | ToolPermissionMap yapilandirmasi | VersaCoder.Protocol | net48;net10.0-windows | ToolPermissionMapTests | planli |
| N33-092 | `ToolPermissionMapProvider` | class | ToolPermissionMap saglayici uygulamasi | VersaCoder.Protocol | net48;net10.0-windows | ToolPermissionMapTests | planli |
| N33-093 | `ToolPermissionMapRegistry` | class | ToolPermissionMap kayit/katalog yonetimi | VersaCoder.Protocol | net48;net10.0-windows | ToolPermissionMapTests | planli |
| N33-094 | `ToolPermissionMapConverter` | class | ToolPermissionMap bicim donusumu | VersaCoder.Protocol | net48;net10.0-windows | ToolPermissionMapTests | planli |
| N33-095 | `ToolPermissionMapTests` | class | ToolPermissionMap testleri (xUnit) | VersaCoder.Protocol | net48;net10.0-windows | - | planli |
| N33-096 | `ToolPermissionMapBuilder` | class | ToolPermissionMap uretici (builder) | VersaCoder.Protocol | net48;net10.0-windows | ToolPermissionMapTests | planli |
| N33-097 | `IToolCatalog` | interface | ToolCatalog sozlesmesi (port) | VersaCoder.Protocol | net48;net10.0-windows | ToolCatalogTests | planli |
| N33-098 | `ToolCatalog` | class | ToolCatalog cekirdek uygulamasi | VersaCoder.Protocol | net48;net10.0-windows | ToolCatalogTests | planli |
| N33-099 | `ToolCatalogDto` | record | ToolCatalog veri transferi modeli | VersaCoder.Protocol | net48;net10.0-windows | ToolCatalogTests | planli |
| N33-100 | `ToolCatalogMapper` | class | ToolCatalog <-> DTO esleme | VersaCoder.Protocol | net48;net10.0-windows | ToolCatalogTests | planli |
| N33-101 | `ToolCatalogValidator` | class | ToolCatalog dogrulama kurallari | VersaCoder.Protocol | net48;net10.0-windows | ToolCatalogTests | planli |
| N33-102 | `ToolCatalogFactory` | class | ToolCatalog uretimi ve varsayilanlari | VersaCoder.Protocol | net48;net10.0-windows | ToolCatalogTests | planli |
| N33-103 | `ToolCatalogOptions` | record | ToolCatalog yapilandirmasi | VersaCoder.Protocol | net48;net10.0-windows | ToolCatalogTests | planli |
| N33-104 | `ToolCatalogProvider` | class | ToolCatalog saglayici uygulamasi | VersaCoder.Protocol | net48;net10.0-windows | ToolCatalogTests | planli |
| N33-105 | `ToolCatalogRegistry` | class | ToolCatalog kayit/katalog yonetimi | VersaCoder.Protocol | net48;net10.0-windows | ToolCatalogTests | planli |
| N33-106 | `ToolCatalogConverter` | class | ToolCatalog bicim donusumu | VersaCoder.Protocol | net48;net10.0-windows | ToolCatalogTests | planli |
| N33-107 | `ToolCatalogTests` | class | ToolCatalog testleri (xUnit) | VersaCoder.Protocol | net48;net10.0-windows | - | planli |
| N33-108 | `ToolCatalogBuilder` | class | ToolCatalog uretici (builder) | VersaCoder.Protocol | net48;net10.0-windows | ToolCatalogTests | planli |
| N33-109 | `IToolSignature` | interface | ToolSignature sozlesmesi (port) | VersaCoder.Protocol | net48;net10.0-windows | ToolSignatureTests | planli |
| N33-110 | `ToolSignature` | class | ToolSignature cekirdek uygulamasi | VersaCoder.Protocol | net48;net10.0-windows | ToolSignatureTests | planli |
| N33-111 | `ToolSignatureDto` | record | ToolSignature veri transferi modeli | VersaCoder.Protocol | net48;net10.0-windows | ToolSignatureTests | planli |
| N33-112 | `ToolSignatureMapper` | class | ToolSignature <-> DTO esleme | VersaCoder.Protocol | net48;net10.0-windows | ToolSignatureTests | planli |
| N33-113 | `ToolSignatureValidator` | class | ToolSignature dogrulama kurallari | VersaCoder.Protocol | net48;net10.0-windows | ToolSignatureTests | planli |
| N33-114 | `ToolSignatureFactory` | class | ToolSignature uretimi ve varsayilanlari | VersaCoder.Protocol | net48;net10.0-windows | ToolSignatureTests | planli |
| N33-115 | `ToolSignatureOptions` | record | ToolSignature yapilandirmasi | VersaCoder.Protocol | net48;net10.0-windows | ToolSignatureTests | planli |
| N33-116 | `ToolSignatureProvider` | class | ToolSignature saglayici uygulamasi | VersaCoder.Protocol | net48;net10.0-windows | ToolSignatureTests | planli |
| N33-117 | `ToolSignatureRegistry` | class | ToolSignature kayit/katalog yonetimi | VersaCoder.Protocol | net48;net10.0-windows | ToolSignatureTests | planli |
| N33-118 | `ToolSignatureConverter` | class | ToolSignature bicim donusumu | VersaCoder.Protocol | net48;net10.0-windows | ToolSignatureTests | planli |
| N33-119 | `ToolSignatureTests` | class | ToolSignature testleri (xUnit) | VersaCoder.Protocol | net48;net10.0-windows | - | planli |
| N33-120 | `ToolSignatureBuilder` | class | ToolSignature uretici (builder) | VersaCoder.Protocol | net48;net10.0-windows | ToolSignatureTests | planli |
