---
title: Katalog Dugumu N34 - Mesajlar
type: catalog-node
category: l5-protocol
status: needs-validation
version: 1.0.0
updated: 2026-10-04
authority: derived
scope: protokol mesaj zarfi ve akis parcasi
---

# Katalog Dugumu N34 - Mesajlar

**Zorunlu Baglantilar:** index.md | node-inventory.md | ../index.md | ../../.decisions/accepted/ADR-028-layered-baseline-9-projects.md | ../../.decisions/accepted/ADR-013-layered-architecture-52-nodes.md

> Dugum, katman sozlesmesini cevaplar ve kanitini baglar.

## §0 Ozet (Answer First)

| Alan | Deger |
| --- | --- |
| Katman | l5 Protocol |
| Proje | VersaCoder.Protocol |
| Kapsam | protokol mesaj zarfi ve akis parcasi |
| Kaynak | C:/www/versacoder/versacoder/.ai/architecture/catalog/l5-protocol/N34-messages.md |
| Sorumluluk | Dugum, katman sozlesmesini cevaplar ve kanitini baglar. |

- Kullanici, asistan, arac cagri ve arac sonucu mesajlari tek zarf modelinde toplanir.
- Zarf; rol, icerik parcasi, kimlik ve zaman damgasi tasiyip sira numarasi uretir.
- Akis delta parcasi bolunmus cevaplari birlestirip tam mesaja cevirir.
- Zarf serilestirme surum baglidir, okuyucu eski surumu de kabul eder.
- Kayitlar en az 6 aile 120 ornek ile kanitlanir, gerisi namludur (kayit yok).

## §1 YAPAR / YAPMAZ

| YAPAR | YAPMAZ |
| --- | --- |
| Mesaj zarfini ve icerik parcasi modelini uretir | Mesaji ag uzerinden gonderir (l4 is) |
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
| 1 | MessageEnvelope | Tek mesaj zarfi modeli |
| 2 | MessageRole | Rol eslemesi (user/assistant) |
| 3 | ContentPart | Icerik parcasi temel tipi |
| 4 | ToolCallPart | Arac cagri parcasi |
| 5 | ToolResultPart | Arac sonuc parcasi |
| 6 | StreamDelta | Akis delta parcasi |
| 7 | MessageJson | JSON karsiligi uretimi |
| 8 | MessageSerializer | Serilestirme ve cozme |
| 9 | MessageFactory | Zarf kurma yardimcisi |
| 10 | MessageSample | Ornek mesaj uretimi |

## §4 Ornek Kayit (seri 120)

| Kayit | Aile | Alan1 | Alan2 |
| --- | --- | --- | --- |
| 1 | MessageEnvelope | user | metin |
| 2 | MessageEnvelope | assistant | metin |
| 3 | MessageRole | toolCall | git.status |
| 4 | MessageRole | toolResult | json |
| 5 | ContentPart | delta | parca 1/3 |
| 6 | ToolCallPart | zarf sira 12 | ok |

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
| ZarfUretimTesti | MessageEnvelope | olusturulan |
| SerilestirmeTesti | MessageEnvelope | donen |
| AkisBirlestirmeTesti | ToolResultPart | tam mesaj |
| RolEslemeTesti | ToolResultPart | eslesen |

## §7 Riskler

| Risk | Etki | Onlem |
| --- | --- | --- |
| Sira kaybi | karisik akis | sira damgasi |
| Surum uyumsuzlugu | cozme hatasi | okuyucu eslestirme |
| Eksik parcacik | eksik cevap | birlestirme testi |

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
| Dosya | N34-messages.md |
| Klasor ornegi | C:\versacoder\src\VersaCoder.Protocol |
| Ornek kayit | 6/6 hedef (seri 120 icin) |
| Test | 4/4 hedef |

kayit tablosu §10'e port edildi (kaynak vault 53 dosya / 5440 kayit); esik 5000 karsilanir.


## §10 Kayit Tablosu (ADR-013 2.2 - kaynak vault port)

| N34-001 | `IMessageEnvelope` | interface | MessageEnvelope sozlesmesi (port) | VersaCoder.Protocol | net48;net10.0-windows | MessageEnvelopeTests | planli |
| N34-002 | `MessageEnvelope` | class | MessageEnvelope cekirdek uygulamasi | VersaCoder.Protocol | net48;net10.0-windows | MessageEnvelopeTests | planli |
| N34-003 | `MessageEnvelopeDto` | record | MessageEnvelope veri transferi modeli | VersaCoder.Protocol | net48;net10.0-windows | MessageEnvelopeTests | planli |
| N34-004 | `MessageEnvelopeMapper` | class | MessageEnvelope <-> DTO esleme | VersaCoder.Protocol | net48;net10.0-windows | MessageEnvelopeTests | planli |
| N34-005 | `MessageEnvelopeValidator` | class | MessageEnvelope dogrulama kurallari | VersaCoder.Protocol | net48;net10.0-windows | MessageEnvelopeTests | planli |
| N34-006 | `MessageEnvelopeFactory` | class | MessageEnvelope uretimi ve varsayilanlari | VersaCoder.Protocol | net48;net10.0-windows | MessageEnvelopeTests | planli |
| N34-007 | `MessageEnvelopeOptions` | record | MessageEnvelope yapilandirmasi | VersaCoder.Protocol | net48;net10.0-windows | MessageEnvelopeTests | planli |
| N34-008 | `MessageEnvelopeProvider` | class | MessageEnvelope saglayici uygulamasi | VersaCoder.Protocol | net48;net10.0-windows | MessageEnvelopeTests | planli |
| N34-009 | `MessageEnvelopeRegistry` | class | MessageEnvelope kayit/katalog yonetimi | VersaCoder.Protocol | net48;net10.0-windows | MessageEnvelopeTests | planli |
| N34-010 | `MessageEnvelopeConverter` | class | MessageEnvelope bicim donusumu | VersaCoder.Protocol | net48;net10.0-windows | MessageEnvelopeTests | planli |
| N34-011 | `MessageEnvelopeTests` | class | MessageEnvelope testleri (xUnit) | VersaCoder.Protocol | net48;net10.0-windows | - | planli |
| N34-012 | `MessageEnvelopeBuilder` | class | MessageEnvelope uretici (builder) | VersaCoder.Protocol | net48;net10.0-windows | MessageEnvelopeTests | planli |
| N34-013 | `IMessageRole` | interface | MessageRole sozlesmesi (port) | VersaCoder.Protocol | net48;net10.0-windows | MessageRoleTests | planli |
| N34-014 | `MessageRole` | class | MessageRole cekirdek uygulamasi | VersaCoder.Protocol | net48;net10.0-windows | MessageRoleTests | planli |
| N34-015 | `MessageRoleDto` | record | MessageRole veri transferi modeli | VersaCoder.Protocol | net48;net10.0-windows | MessageRoleTests | planli |
| N34-016 | `MessageRoleMapper` | class | MessageRole <-> DTO esleme | VersaCoder.Protocol | net48;net10.0-windows | MessageRoleTests | planli |
| N34-017 | `MessageRoleValidator` | class | MessageRole dogrulama kurallari | VersaCoder.Protocol | net48;net10.0-windows | MessageRoleTests | planli |
| N34-018 | `MessageRoleFactory` | class | MessageRole uretimi ve varsayilanlari | VersaCoder.Protocol | net48;net10.0-windows | MessageRoleTests | planli |
| N34-019 | `MessageRoleOptions` | record | MessageRole yapilandirmasi | VersaCoder.Protocol | net48;net10.0-windows | MessageRoleTests | planli |
| N34-020 | `MessageRoleProvider` | class | MessageRole saglayici uygulamasi | VersaCoder.Protocol | net48;net10.0-windows | MessageRoleTests | planli |
| N34-021 | `MessageRoleRegistry` | class | MessageRole kayit/katalog yonetimi | VersaCoder.Protocol | net48;net10.0-windows | MessageRoleTests | planli |
| N34-022 | `MessageRoleConverter` | class | MessageRole bicim donusumu | VersaCoder.Protocol | net48;net10.0-windows | MessageRoleTests | planli |
| N34-023 | `MessageRoleTests` | class | MessageRole testleri (xUnit) | VersaCoder.Protocol | net48;net10.0-windows | - | planli |
| N34-024 | `MessageRoleBuilder` | class | MessageRole uretici (builder) | VersaCoder.Protocol | net48;net10.0-windows | MessageRoleTests | planli |
| N34-025 | `IContentPart` | interface | ContentPart sozlesmesi (port) | VersaCoder.Protocol | net48;net10.0-windows | ContentPartTests | planli |
| N34-026 | `ContentPart` | class | ContentPart cekirdek uygulamasi | VersaCoder.Protocol | net48;net10.0-windows | ContentPartTests | planli |
| N34-027 | `ContentPartDto` | record | ContentPart veri transferi modeli | VersaCoder.Protocol | net48;net10.0-windows | ContentPartTests | planli |
| N34-028 | `ContentPartMapper` | class | ContentPart <-> DTO esleme | VersaCoder.Protocol | net48;net10.0-windows | ContentPartTests | planli |
| N34-029 | `ContentPartValidator` | class | ContentPart dogrulama kurallari | VersaCoder.Protocol | net48;net10.0-windows | ContentPartTests | planli |
| N34-030 | `ContentPartFactory` | class | ContentPart uretimi ve varsayilanlari | VersaCoder.Protocol | net48;net10.0-windows | ContentPartTests | planli |
| N34-031 | `ContentPartOptions` | record | ContentPart yapilandirmasi | VersaCoder.Protocol | net48;net10.0-windows | ContentPartTests | planli |
| N34-032 | `ContentPartProvider` | class | ContentPart saglayici uygulamasi | VersaCoder.Protocol | net48;net10.0-windows | ContentPartTests | planli |
| N34-033 | `ContentPartRegistry` | class | ContentPart kayit/katalog yonetimi | VersaCoder.Protocol | net48;net10.0-windows | ContentPartTests | planli |
| N34-034 | `ContentPartConverter` | class | ContentPart bicim donusumu | VersaCoder.Protocol | net48;net10.0-windows | ContentPartTests | planli |
| N34-035 | `ContentPartTests` | class | ContentPart testleri (xUnit) | VersaCoder.Protocol | net48;net10.0-windows | - | planli |
| N34-036 | `ContentPartBuilder` | class | ContentPart uretici (builder) | VersaCoder.Protocol | net48;net10.0-windows | ContentPartTests | planli |
| N34-037 | `IToolCallPart` | interface | ToolCallPart sozlesmesi (port) | VersaCoder.Protocol | net48;net10.0-windows | ToolCallPartTests | planli |
| N34-038 | `ToolCallPart` | class | ToolCallPart cekirdek uygulamasi | VersaCoder.Protocol | net48;net10.0-windows | ToolCallPartTests | planli |
| N34-039 | `ToolCallPartDto` | record | ToolCallPart veri transferi modeli | VersaCoder.Protocol | net48;net10.0-windows | ToolCallPartTests | planli |
| N34-040 | `ToolCallPartMapper` | class | ToolCallPart <-> DTO esleme | VersaCoder.Protocol | net48;net10.0-windows | ToolCallPartTests | planli |
| N34-041 | `ToolCallPartValidator` | class | ToolCallPart dogrulama kurallari | VersaCoder.Protocol | net48;net10.0-windows | ToolCallPartTests | planli |
| N34-042 | `ToolCallPartFactory` | class | ToolCallPart uretimi ve varsayilanlari | VersaCoder.Protocol | net48;net10.0-windows | ToolCallPartTests | planli |
| N34-043 | `ToolCallPartOptions` | record | ToolCallPart yapilandirmasi | VersaCoder.Protocol | net48;net10.0-windows | ToolCallPartTests | planli |
| N34-044 | `ToolCallPartProvider` | class | ToolCallPart saglayici uygulamasi | VersaCoder.Protocol | net48;net10.0-windows | ToolCallPartTests | planli |
| N34-045 | `ToolCallPartRegistry` | class | ToolCallPart kayit/katalog yonetimi | VersaCoder.Protocol | net48;net10.0-windows | ToolCallPartTests | planli |
| N34-046 | `ToolCallPartConverter` | class | ToolCallPart bicim donusumu | VersaCoder.Protocol | net48;net10.0-windows | ToolCallPartTests | planli |
| N34-047 | `ToolCallPartTests` | class | ToolCallPart testleri (xUnit) | VersaCoder.Protocol | net48;net10.0-windows | - | planli |
| N34-048 | `ToolCallPartBuilder` | class | ToolCallPart uretici (builder) | VersaCoder.Protocol | net48;net10.0-windows | ToolCallPartTests | planli |
| N34-049 | `IToolResultPart` | interface | ToolResultPart sozlesmesi (port) | VersaCoder.Protocol | net48;net10.0-windows | ToolResultPartTests | planli |
| N34-050 | `ToolResultPart` | class | ToolResultPart cekirdek uygulamasi | VersaCoder.Protocol | net48;net10.0-windows | ToolResultPartTests | planli |
| N34-051 | `ToolResultPartDto` | record | ToolResultPart veri transferi modeli | VersaCoder.Protocol | net48;net10.0-windows | ToolResultPartTests | planli |
| N34-052 | `ToolResultPartMapper` | class | ToolResultPart <-> DTO esleme | VersaCoder.Protocol | net48;net10.0-windows | ToolResultPartTests | planli |
| N34-053 | `ToolResultPartValidator` | class | ToolResultPart dogrulama kurallari | VersaCoder.Protocol | net48;net10.0-windows | ToolResultPartTests | planli |
| N34-054 | `ToolResultPartFactory` | class | ToolResultPart uretimi ve varsayilanlari | VersaCoder.Protocol | net48;net10.0-windows | ToolResultPartTests | planli |
| N34-055 | `ToolResultPartOptions` | record | ToolResultPart yapilandirmasi | VersaCoder.Protocol | net48;net10.0-windows | ToolResultPartTests | planli |
| N34-056 | `ToolResultPartProvider` | class | ToolResultPart saglayici uygulamasi | VersaCoder.Protocol | net48;net10.0-windows | ToolResultPartTests | planli |
| N34-057 | `ToolResultPartRegistry` | class | ToolResultPart kayit/katalog yonetimi | VersaCoder.Protocol | net48;net10.0-windows | ToolResultPartTests | planli |
| N34-058 | `ToolResultPartConverter` | class | ToolResultPart bicim donusumu | VersaCoder.Protocol | net48;net10.0-windows | ToolResultPartTests | planli |
| N34-059 | `ToolResultPartTests` | class | ToolResultPart testleri (xUnit) | VersaCoder.Protocol | net48;net10.0-windows | - | planli |
| N34-060 | `ToolResultPartBuilder` | class | ToolResultPart uretici (builder) | VersaCoder.Protocol | net48;net10.0-windows | ToolResultPartTests | planli |
| N34-061 | `IStreamDelta` | interface | StreamDelta sozlesmesi (port) | VersaCoder.Protocol | net48;net10.0-windows | StreamDeltaTests | planli |
| N34-062 | `StreamDelta` | class | StreamDelta cekirdek uygulamasi | VersaCoder.Protocol | net48;net10.0-windows | StreamDeltaTests | planli |
| N34-063 | `StreamDeltaDto` | record | StreamDelta veri transferi modeli | VersaCoder.Protocol | net48;net10.0-windows | StreamDeltaTests | planli |
| N34-064 | `StreamDeltaMapper` | class | StreamDelta <-> DTO esleme | VersaCoder.Protocol | net48;net10.0-windows | StreamDeltaTests | planli |
| N34-065 | `StreamDeltaValidator` | class | StreamDelta dogrulama kurallari | VersaCoder.Protocol | net48;net10.0-windows | StreamDeltaTests | planli |
| N34-066 | `StreamDeltaFactory` | class | StreamDelta uretimi ve varsayilanlari | VersaCoder.Protocol | net48;net10.0-windows | StreamDeltaTests | planli |
| N34-067 | `StreamDeltaOptions` | record | StreamDelta yapilandirmasi | VersaCoder.Protocol | net48;net10.0-windows | StreamDeltaTests | planli |
| N34-068 | `StreamDeltaProvider` | class | StreamDelta saglayici uygulamasi | VersaCoder.Protocol | net48;net10.0-windows | StreamDeltaTests | planli |
| N34-069 | `StreamDeltaRegistry` | class | StreamDelta kayit/katalog yonetimi | VersaCoder.Protocol | net48;net10.0-windows | StreamDeltaTests | planli |
| N34-070 | `StreamDeltaConverter` | class | StreamDelta bicim donusumu | VersaCoder.Protocol | net48;net10.0-windows | StreamDeltaTests | planli |
| N34-071 | `StreamDeltaTests` | class | StreamDelta testleri (xUnit) | VersaCoder.Protocol | net48;net10.0-windows | - | planli |
| N34-072 | `StreamDeltaBuilder` | class | StreamDelta uretici (builder) | VersaCoder.Protocol | net48;net10.0-windows | StreamDeltaTests | planli |
| N34-073 | `IMessageJson` | interface | MessageJson sozlesmesi (port) | VersaCoder.Protocol | net48;net10.0-windows | MessageJsonTests | planli |
| N34-074 | `MessageJson` | class | MessageJson cekirdek uygulamasi | VersaCoder.Protocol | net48;net10.0-windows | MessageJsonTests | planli |
| N34-075 | `MessageJsonDto` | record | MessageJson veri transferi modeli | VersaCoder.Protocol | net48;net10.0-windows | MessageJsonTests | planli |
| N34-076 | `MessageJsonMapper` | class | MessageJson <-> DTO esleme | VersaCoder.Protocol | net48;net10.0-windows | MessageJsonTests | planli |
| N34-077 | `MessageJsonValidator` | class | MessageJson dogrulama kurallari | VersaCoder.Protocol | net48;net10.0-windows | MessageJsonTests | planli |
| N34-078 | `MessageJsonFactory` | class | MessageJson uretimi ve varsayilanlari | VersaCoder.Protocol | net48;net10.0-windows | MessageJsonTests | planli |
| N34-079 | `MessageJsonOptions` | record | MessageJson yapilandirmasi | VersaCoder.Protocol | net48;net10.0-windows | MessageJsonTests | planli |
| N34-080 | `MessageJsonProvider` | class | MessageJson saglayici uygulamasi | VersaCoder.Protocol | net48;net10.0-windows | MessageJsonTests | planli |
| N34-081 | `MessageJsonRegistry` | class | MessageJson kayit/katalog yonetimi | VersaCoder.Protocol | net48;net10.0-windows | MessageJsonTests | planli |
| N34-082 | `MessageJsonConverter` | class | MessageJson bicim donusumu | VersaCoder.Protocol | net48;net10.0-windows | MessageJsonTests | planli |
| N34-083 | `MessageJsonTests` | class | MessageJson testleri (xUnit) | VersaCoder.Protocol | net48;net10.0-windows | - | planli |
| N34-084 | `MessageJsonBuilder` | class | MessageJson uretici (builder) | VersaCoder.Protocol | net48;net10.0-windows | MessageJsonTests | planli |
| N34-085 | `IMessageSerializer` | interface | MessageSerializer sozlesmesi (port) | VersaCoder.Protocol | net48;net10.0-windows | MessageSerializerTests | planli |
| N34-086 | `MessageSerializer` | class | MessageSerializer cekirdek uygulamasi | VersaCoder.Protocol | net48;net10.0-windows | MessageSerializerTests | planli |
| N34-087 | `MessageSerializerDto` | record | MessageSerializer veri transferi modeli | VersaCoder.Protocol | net48;net10.0-windows | MessageSerializerTests | planli |
| N34-088 | `MessageSerializerMapper` | class | MessageSerializer <-> DTO esleme | VersaCoder.Protocol | net48;net10.0-windows | MessageSerializerTests | planli |
| N34-089 | `MessageSerializerValidator` | class | MessageSerializer dogrulama kurallari | VersaCoder.Protocol | net48;net10.0-windows | MessageSerializerTests | planli |
| N34-090 | `MessageSerializerFactory` | class | MessageSerializer uretimi ve varsayilanlari | VersaCoder.Protocol | net48;net10.0-windows | MessageSerializerTests | planli |
| N34-091 | `MessageSerializerOptions` | record | MessageSerializer yapilandirmasi | VersaCoder.Protocol | net48;net10.0-windows | MessageSerializerTests | planli |
| N34-092 | `MessageSerializerProvider` | class | MessageSerializer saglayici uygulamasi | VersaCoder.Protocol | net48;net10.0-windows | MessageSerializerTests | planli |
| N34-093 | `MessageSerializerRegistry` | class | MessageSerializer kayit/katalog yonetimi | VersaCoder.Protocol | net48;net10.0-windows | MessageSerializerTests | planli |
| N34-094 | `MessageSerializerConverter` | class | MessageSerializer bicim donusumu | VersaCoder.Protocol | net48;net10.0-windows | MessageSerializerTests | planli |
| N34-095 | `MessageSerializerTests` | class | MessageSerializer testleri (xUnit) | VersaCoder.Protocol | net48;net10.0-windows | - | planli |
| N34-096 | `MessageSerializerBuilder` | class | MessageSerializer uretici (builder) | VersaCoder.Protocol | net48;net10.0-windows | MessageSerializerTests | planli |
| N34-097 | `IMessageFactory` | interface | MessageFactory sozlesmesi (port) | VersaCoder.Protocol | net48;net10.0-windows | MessageFactoryTests | planli |
| N34-098 | `MessageFactory` | class | MessageFactory cekirdek uygulamasi | VersaCoder.Protocol | net48;net10.0-windows | MessageFactoryTests | planli |
| N34-099 | `MessageFactoryDto` | record | MessageFactory veri transferi modeli | VersaCoder.Protocol | net48;net10.0-windows | MessageFactoryTests | planli |
| N34-100 | `MessageFactoryMapper` | class | MessageFactory <-> DTO esleme | VersaCoder.Protocol | net48;net10.0-windows | MessageFactoryTests | planli |
| N34-101 | `MessageFactoryValidator` | class | MessageFactory dogrulama kurallari | VersaCoder.Protocol | net48;net10.0-windows | MessageFactoryTests | planli |
| N34-102 | `MessageFactoryFactory` | class | MessageFactory uretimi ve varsayilanlari | VersaCoder.Protocol | net48;net10.0-windows | MessageFactoryTests | planli |
| N34-103 | `MessageFactoryOptions` | record | MessageFactory yapilandirmasi | VersaCoder.Protocol | net48;net10.0-windows | MessageFactoryTests | planli |
| N34-104 | `MessageFactoryProvider` | class | MessageFactory saglayici uygulamasi | VersaCoder.Protocol | net48;net10.0-windows | MessageFactoryTests | planli |
| N34-105 | `MessageFactoryRegistry` | class | MessageFactory kayit/katalog yonetimi | VersaCoder.Protocol | net48;net10.0-windows | MessageFactoryTests | planli |
| N34-106 | `MessageFactoryConverter` | class | MessageFactory bicim donusumu | VersaCoder.Protocol | net48;net10.0-windows | MessageFactoryTests | planli |
| N34-107 | `MessageFactoryTests` | class | MessageFactory testleri (xUnit) | VersaCoder.Protocol | net48;net10.0-windows | - | planli |
| N34-108 | `MessageFactoryBuilder` | class | MessageFactory uretici (builder) | VersaCoder.Protocol | net48;net10.0-windows | MessageFactoryTests | planli |
| N34-109 | `IMessageSample` | interface | MessageSample sozlesmesi (port) | VersaCoder.Protocol | net48;net10.0-windows | MessageSampleTests | planli |
| N34-110 | `MessageSample` | class | MessageSample cekirdek uygulamasi | VersaCoder.Protocol | net48;net10.0-windows | MessageSampleTests | planli |
| N34-111 | `MessageSampleDto` | record | MessageSample veri transferi modeli | VersaCoder.Protocol | net48;net10.0-windows | MessageSampleTests | planli |
| N34-112 | `MessageSampleMapper` | class | MessageSample <-> DTO esleme | VersaCoder.Protocol | net48;net10.0-windows | MessageSampleTests | planli |
| N34-113 | `MessageSampleValidator` | class | MessageSample dogrulama kurallari | VersaCoder.Protocol | net48;net10.0-windows | MessageSampleTests | planli |
| N34-114 | `MessageSampleFactory` | class | MessageSample uretimi ve varsayilanlari | VersaCoder.Protocol | net48;net10.0-windows | MessageSampleTests | planli |
| N34-115 | `MessageSampleOptions` | record | MessageSample yapilandirmasi | VersaCoder.Protocol | net48;net10.0-windows | MessageSampleTests | planli |
| N34-116 | `MessageSampleProvider` | class | MessageSample saglayici uygulamasi | VersaCoder.Protocol | net48;net10.0-windows | MessageSampleTests | planli |
| N34-117 | `MessageSampleRegistry` | class | MessageSample kayit/katalog yonetimi | VersaCoder.Protocol | net48;net10.0-windows | MessageSampleTests | planli |
| N34-118 | `MessageSampleConverter` | class | MessageSample bicim donusumu | VersaCoder.Protocol | net48;net10.0-windows | MessageSampleTests | planli |
| N34-119 | `MessageSampleTests` | class | MessageSample testleri (xUnit) | VersaCoder.Protocol | net48;net10.0-windows | - | planli |
| N34-120 | `MessageSampleBuilder` | class | MessageSample uretici (builder) | VersaCoder.Protocol | net48;net10.0-windows | MessageSampleTests | planli |
