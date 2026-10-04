---
title: "Katalog Dugumu N03 - Olay ABI Sozlesmeleri (l0)"
type: catalog
category: architecture
status: needs-validation
version: 1.0.0
updated: 2026-10-04
authority: derived
scope: "N03 events-abi dugumu: kimlik, src klasoru, aile yapisi, ornek kayit ve yon kurali (ADR-013 + ADR-028)"
---

# Katalog Dugumu N03 - Olay ABI Sozlesmeleri

**Zorunlu Baglantilar:** [catalog index](index.md) · [node-inventory](node-inventory.md) · [mimari index](../index.md) · [ADR-028](../../.decisions/accepted/ADR-028-layered-baseline-9-projects.md) · [ADR-013](../../.decisions/accepted/ADR-013-layered-architecture-52-nodes.md)

> **Tek cumlelik sorumluluk:** *N03 events-abi hangi projede ve klasorde yasar, ne yapar, kac kayit hedefler* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

**N03 (events-abi), l0 Abstractions katmaninin alt katmanidir: `VersaCoder.Abstractions` projesinde `src/VersaCoder.Abstractions/Events/` klasorunde yasar.** Olay zarfi, bus, abone ve yayin sozlesmelerini (ABI) tanimlar. 10 aile x 12 rol = 120 kayit hedefi (ADR-013 asgari 112); durum `planli`.

Kayitlar ADR-013 2.1 biciminde 8 sutun tasir; bu dosya aile kalibini ve ornek satirlari gosterir, tam 120 satirlik liste bu dosyanin boyut sinirina sigmaz (DOĞRULAMA GEREKLI).

```text
+--------------------------------------------------+
| l0 Abstractions -> VersaCoder.Abstractions
| src/VersaCoder.Abstractions/Events/
| 10 aile x 12 rol = 120 kayit (esik 112)
+--------------------------------------------------+
```

| Alan | Deger |
|---|---|
| Dugum | N03 |
| Dugum turu | alt katman (klasor/namespace duzeyi) |
| Makro katman | l0 Abstractions |
| Proje (.csproj) | VersaCoder.Abstractions |
| TFM | net48;net10.0-windows |
| Kayit hedefi | 120 (ADR-013 asgari 112) |
| Durum | planli |
| Sorumlu rol | architect |

---

## §1 Sorumluluk (YAPAR / YAPMAZ)

| # | YAPAR | YAPMAZ |
|---|---|---|
| 1 | Olay zarfi, bus, abone ve yayin sozlesmelerini (ABI) tanimlar | somut uygulama (adapter) sinifi yazmak, uygulama l4 katmaninda |
| 2 | kayitlari 8 sutunlu ADR-013 biciminde tutmak (planli -> yapildi -> testli) | baska VersaCoder projesine referans vermek |
| 3 | sozlesme ve arayuz tanimlamak (interface, record, enum) | veri erisimi, SQL veya dosya I/O cagirmak |
| 4 | hicbir somut uygulama sinifi yazmamak | UI tipine bakmak (Desktop / Cli) |
| 5 | yalniz BCL ve Directory.Packages.props paketlerine bakmak | framework bagimliligi getirmek (or. EntityFramework) |
| 6 | geri donusu l0 interface uzerinden saglamak | gizli is kurali veya sabit olmayan deger uretmek |

---

## §2 Klasor ve N Eslemesi

**Kalip (ADR-028 2.3/4):** `N33 -> src\VersaCoder.Protocol\ToolSchema\` satiri SSOT ornegidir; bu dosyanin klasoru ayni kalipla yazilir.

| # | N | Slug | src/ klasoru |
|---|---|---|---|
| 1 | N03 | events-abi | `src/VersaCoder.Abstractions/Events/` |

**Kanit yolu (ADR-028 2.3/5):** tarama yolu gercek proje adini icerir; genis kalip (`src/**/Domain/**`) sahte PASS uretir.

```powershell
Test-Path 'C:\versacoder\src\VersaCoder.Abstractions\Events'
```

Dizin su an yok cikti `False` olur; PASS yazilmaz -> **DOĞRULAMA GEREKLI**.

---

## §3 Aile Yapisi

Rol kalibi (12): `I<F>` · `<F>` · `<F>Dto` · `<F>Mapper` · `<F>Validator` · `<F>Factory` · `<F>Options` · `<F>Provider` · `<F>Registry` · `<F>Converter` · `<F>Tests` · `<F>Builder`

| # | Aile | # | Aile |
|---|---|---|---|
| 1 | DomainEvent | 6 | EventSubscriber |
| 2 | EventEnvelope | 7 | EventPublisher |
| 3 | EventName | 8 | EventFilter |
| 4 | EventPayload | 9 | EventLog |
| 5 | EventBus | 10 | EventClock |

Her aile ayni rol kalibini 12 kez tekrarlar; kayit sayisi = aile x rol.

---

## §4 Kayit Ornegi (8 sutun)

| ID | Bilesen | Tip | Sorumluluk | Proje | TFM | Test | Durum |
|---|---|---|---|---|---|---|---|
| N03-001 | `IDomainEvent` | interface | DomainEvent sozlesmesi (port) | VersaCoder.Abstractions | net48;net10.0-windows | DomainEventTests | planli |
| N03-002 | `DomainEvent` | class | DomainEvent cekirdek uygulamasi | VersaCoder.Abstractions | net48;net10.0-windows | DomainEventTests | planli |
| N03-003 | `DomainEventDto` | record | DomainEvent veri transferi modeli | VersaCoder.Abstractions | net48;net10.0-windows | DomainEventTests | planli |
| N03-004 | `DomainEventMapper` | class | DomainEvent <-> DTO esleme | VersaCoder.Abstractions | net48;net10.0-windows | DomainEventTests | planli |
| N03-005 | `DomainEventValidator` | class | DomainEvent dogrulama kurallari | VersaCoder.Abstractions | net48;net10.0-windows | DomainEventTests | planli |
| N03-006 | `DomainEventFactory` | class | DomainEvent uretimi ve varsayilanlari | VersaCoder.Abstractions | net48;net10.0-windows | DomainEventTests | planli |

Kalan aileler ayni kalibi 12 rolle tekrarlar (makro dugumlerde 4 rol); toplam hedef **120** kayit.

---

## §5 Yon Kurali

| # | Yon | Deger | Kaynak |
|---|---|---|---|
| 1 | l0 -> l1..l7 | X | ADR-028 2.1 |
| 2 | l0 -> dis (BCL, CPM paketi) | izinli | ADR-028 2.1 |
| 3 | l1..l7 -> l0 | OK | ADR-028 2.1 |

Matrisin tamami [layer-matrix](layer-matrix.md) dosyasindadir; NetArchTest `LayerRulesTests` bu tabloya gore yazilir.

---

## §6 Test ve Durum

| # | Kontrol | Beklenen | Durum |
|---|---|---|---|
| 1 | Kayit satiri | 120 (asgari 112) | bu dosyada 6 ornek satir; tam liste DOĞRULAMA GEREKLI |
| 2 | Durum degeri | planli -> yapildi -> testli | planli |
| 3 | Test sinifi | DomainEventTests (xUnit) | planli |
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
(Select-String -Path 'C:\versacoder\.ai\architecture\catalog\N03-events-abi.md' -Pattern '^\|\s*N\d{2}-\d{3}\s*\|').Count
# 2) boyut bandi - beklenen: 2600-8500
(Get-Item 'C:\versacoder\.ai\architecture\catalog\N03-events-abi.md').Length
# 3) klasor kaniti - gercek proje adi (ADR-028 2.3/5)
(Test-Path 'C:\versacoder\src\VersaCoder.Abstractions\Events')
```

Katalog sayimi `powershell -NoProfile -ExecutionPolicy Bypass -File C:\versacoder\.ai\scripts\count-components.ps1` ile alinir; bu batch sonunda toplam esik (5000) henuz karsilanmaz - kalan 26 node dosyasi sonraki batchtir.

## §10 Kayit Tablosu (ADR-013 2.2 - kaynak vault port)

| N03-007 | `DomainEventOptions` | record | DomainEvent yapilandirmasi | VersaCoder.Abstractions | net48;net10.0-windows | DomainEventTests | planli |
| N03-008 | `DomainEventProvider` | class | DomainEvent saglayici uygulamasi | VersaCoder.Abstractions | net48;net10.0-windows | DomainEventTests | planli |
| N03-009 | `DomainEventRegistry` | class | DomainEvent kayit/katalog yonetimi | VersaCoder.Abstractions | net48;net10.0-windows | DomainEventTests | planli |
| N03-010 | `DomainEventConverter` | class | DomainEvent bicim donusumu | VersaCoder.Abstractions | net48;net10.0-windows | DomainEventTests | planli |
| N03-011 | `DomainEventTests` | class | DomainEvent testleri (xUnit) | VersaCoder.Abstractions | net48;net10.0-windows | - | planli |
| N03-012 | `DomainEventBuilder` | class | DomainEvent uretici (builder) | VersaCoder.Abstractions | net48;net10.0-windows | DomainEventTests | planli |
| N03-013 | `IEventEnvelope` | interface | EventEnvelope sozlesmesi (port) | VersaCoder.Abstractions | net48;net10.0-windows | EventEnvelopeTests | planli |
| N03-014 | `EventEnvelope` | class | EventEnvelope cekirdek uygulamasi | VersaCoder.Abstractions | net48;net10.0-windows | EventEnvelopeTests | planli |
| N03-015 | `EventEnvelopeDto` | record | EventEnvelope veri transferi modeli | VersaCoder.Abstractions | net48;net10.0-windows | EventEnvelopeTests | planli |
| N03-016 | `EventEnvelopeMapper` | class | EventEnvelope <-> DTO esleme | VersaCoder.Abstractions | net48;net10.0-windows | EventEnvelopeTests | planli |
| N03-017 | `EventEnvelopeValidator` | class | EventEnvelope dogrulama kurallari | VersaCoder.Abstractions | net48;net10.0-windows | EventEnvelopeTests | planli |
| N03-018 | `EventEnvelopeFactory` | class | EventEnvelope uretimi ve varsayilanlari | VersaCoder.Abstractions | net48;net10.0-windows | EventEnvelopeTests | planli |
| N03-019 | `EventEnvelopeOptions` | record | EventEnvelope yapilandirmasi | VersaCoder.Abstractions | net48;net10.0-windows | EventEnvelopeTests | planli |
| N03-020 | `EventEnvelopeProvider` | class | EventEnvelope saglayici uygulamasi | VersaCoder.Abstractions | net48;net10.0-windows | EventEnvelopeTests | planli |
| N03-021 | `EventEnvelopeRegistry` | class | EventEnvelope kayit/katalog yonetimi | VersaCoder.Abstractions | net48;net10.0-windows | EventEnvelopeTests | planli |
| N03-022 | `EventEnvelopeConverter` | class | EventEnvelope bicim donusumu | VersaCoder.Abstractions | net48;net10.0-windows | EventEnvelopeTests | planli |
| N03-023 | `EventEnvelopeTests` | class | EventEnvelope testleri (xUnit) | VersaCoder.Abstractions | net48;net10.0-windows | - | planli |
| N03-024 | `EventEnvelopeBuilder` | class | EventEnvelope uretici (builder) | VersaCoder.Abstractions | net48;net10.0-windows | EventEnvelopeTests | planli |
| N03-025 | `IEventName` | interface | EventName sozlesmesi (port) | VersaCoder.Abstractions | net48;net10.0-windows | EventNameTests | planli |
| N03-026 | `EventName` | class | EventName cekirdek uygulamasi | VersaCoder.Abstractions | net48;net10.0-windows | EventNameTests | planli |
| N03-027 | `EventNameDto` | record | EventName veri transferi modeli | VersaCoder.Abstractions | net48;net10.0-windows | EventNameTests | planli |
| N03-028 | `EventNameMapper` | class | EventName <-> DTO esleme | VersaCoder.Abstractions | net48;net10.0-windows | EventNameTests | planli |
| N03-029 | `EventNameValidator` | class | EventName dogrulama kurallari | VersaCoder.Abstractions | net48;net10.0-windows | EventNameTests | planli |
| N03-030 | `EventNameFactory` | class | EventName uretimi ve varsayilanlari | VersaCoder.Abstractions | net48;net10.0-windows | EventNameTests | planli |
| N03-031 | `EventNameOptions` | record | EventName yapilandirmasi | VersaCoder.Abstractions | net48;net10.0-windows | EventNameTests | planli |
| N03-032 | `EventNameProvider` | class | EventName saglayici uygulamasi | VersaCoder.Abstractions | net48;net10.0-windows | EventNameTests | planli |
| N03-033 | `EventNameRegistry` | class | EventName kayit/katalog yonetimi | VersaCoder.Abstractions | net48;net10.0-windows | EventNameTests | planli |
| N03-034 | `EventNameConverter` | class | EventName bicim donusumu | VersaCoder.Abstractions | net48;net10.0-windows | EventNameTests | planli |
| N03-035 | `EventNameTests` | class | EventName testleri (xUnit) | VersaCoder.Abstractions | net48;net10.0-windows | - | planli |
| N03-036 | `EventNameBuilder` | class | EventName uretici (builder) | VersaCoder.Abstractions | net48;net10.0-windows | EventNameTests | planli |
| N03-037 | `IEventPayload` | interface | EventPayload sozlesmesi (port) | VersaCoder.Abstractions | net48;net10.0-windows | EventPayloadTests | planli |
| N03-038 | `EventPayload` | class | EventPayload cekirdek uygulamasi | VersaCoder.Abstractions | net48;net10.0-windows | EventPayloadTests | planli |
| N03-039 | `EventPayloadDto` | record | EventPayload veri transferi modeli | VersaCoder.Abstractions | net48;net10.0-windows | EventPayloadTests | planli |
| N03-040 | `EventPayloadMapper` | class | EventPayload <-> DTO esleme | VersaCoder.Abstractions | net48;net10.0-windows | EventPayloadTests | planli |
| N03-041 | `EventPayloadValidator` | class | EventPayload dogrulama kurallari | VersaCoder.Abstractions | net48;net10.0-windows | EventPayloadTests | planli |
| N03-042 | `EventPayloadFactory` | class | EventPayload uretimi ve varsayilanlari | VersaCoder.Abstractions | net48;net10.0-windows | EventPayloadTests | planli |
| N03-043 | `EventPayloadOptions` | record | EventPayload yapilandirmasi | VersaCoder.Abstractions | net48;net10.0-windows | EventPayloadTests | planli |
| N03-044 | `EventPayloadProvider` | class | EventPayload saglayici uygulamasi | VersaCoder.Abstractions | net48;net10.0-windows | EventPayloadTests | planli |
| N03-045 | `EventPayloadRegistry` | class | EventPayload kayit/katalog yonetimi | VersaCoder.Abstractions | net48;net10.0-windows | EventPayloadTests | planli |
| N03-046 | `EventPayloadConverter` | class | EventPayload bicim donusumu | VersaCoder.Abstractions | net48;net10.0-windows | EventPayloadTests | planli |
| N03-047 | `EventPayloadTests` | class | EventPayload testleri (xUnit) | VersaCoder.Abstractions | net48;net10.0-windows | - | planli |
| N03-048 | `EventPayloadBuilder` | class | EventPayload uretici (builder) | VersaCoder.Abstractions | net48;net10.0-windows | EventPayloadTests | planli |
| N03-049 | `IEventBus` | interface | EventBus sozlesmesi (port) | VersaCoder.Abstractions | net48;net10.0-windows | EventBusTests | planli |
| N03-050 | `EventBus` | class | EventBus cekirdek uygulamasi | VersaCoder.Abstractions | net48;net10.0-windows | EventBusTests | planli |
| N03-051 | `EventBusDto` | record | EventBus veri transferi modeli | VersaCoder.Abstractions | net48;net10.0-windows | EventBusTests | planli |
| N03-052 | `EventBusMapper` | class | EventBus <-> DTO esleme | VersaCoder.Abstractions | net48;net10.0-windows | EventBusTests | planli |
| N03-053 | `EventBusValidator` | class | EventBus dogrulama kurallari | VersaCoder.Abstractions | net48;net10.0-windows | EventBusTests | planli |
| N03-054 | `EventBusFactory` | class | EventBus uretimi ve varsayilanlari | VersaCoder.Abstractions | net48;net10.0-windows | EventBusTests | planli |
| N03-055 | `EventBusOptions` | record | EventBus yapilandirmasi | VersaCoder.Abstractions | net48;net10.0-windows | EventBusTests | planli |
| N03-056 | `EventBusProvider` | class | EventBus saglayici uygulamasi | VersaCoder.Abstractions | net48;net10.0-windows | EventBusTests | planli |
| N03-057 | `EventBusRegistry` | class | EventBus kayit/katalog yonetimi | VersaCoder.Abstractions | net48;net10.0-windows | EventBusTests | planli |
| N03-058 | `EventBusConverter` | class | EventBus bicim donusumu | VersaCoder.Abstractions | net48;net10.0-windows | EventBusTests | planli |
| N03-059 | `EventBusTests` | class | EventBus testleri (xUnit) | VersaCoder.Abstractions | net48;net10.0-windows | - | planli |
| N03-060 | `EventBusBuilder` | class | EventBus uretici (builder) | VersaCoder.Abstractions | net48;net10.0-windows | EventBusTests | planli |
| N03-061 | `IEventSubscriber` | interface | EventSubscriber sozlesmesi (port) | VersaCoder.Abstractions | net48;net10.0-windows | EventSubscriberTests | planli |
| N03-062 | `EventSubscriber` | class | EventSubscriber cekirdek uygulamasi | VersaCoder.Abstractions | net48;net10.0-windows | EventSubscriberTests | planli |
| N03-063 | `EventSubscriberDto` | record | EventSubscriber veri transferi modeli | VersaCoder.Abstractions | net48;net10.0-windows | EventSubscriberTests | planli |
| N03-064 | `EventSubscriberMapper` | class | EventSubscriber <-> DTO esleme | VersaCoder.Abstractions | net48;net10.0-windows | EventSubscriberTests | planli |
| N03-065 | `EventSubscriberValidator` | class | EventSubscriber dogrulama kurallari | VersaCoder.Abstractions | net48;net10.0-windows | EventSubscriberTests | planli |
| N03-066 | `EventSubscriberFactory` | class | EventSubscriber uretimi ve varsayilanlari | VersaCoder.Abstractions | net48;net10.0-windows | EventSubscriberTests | planli |
| N03-067 | `EventSubscriberOptions` | record | EventSubscriber yapilandirmasi | VersaCoder.Abstractions | net48;net10.0-windows | EventSubscriberTests | planli |
| N03-068 | `EventSubscriberProvider` | class | EventSubscriber saglayici uygulamasi | VersaCoder.Abstractions | net48;net10.0-windows | EventSubscriberTests | planli |
| N03-069 | `EventSubscriberRegistry` | class | EventSubscriber kayit/katalog yonetimi | VersaCoder.Abstractions | net48;net10.0-windows | EventSubscriberTests | planli |
| N03-070 | `EventSubscriberConverter` | class | EventSubscriber bicim donusumu | VersaCoder.Abstractions | net48;net10.0-windows | EventSubscriberTests | planli |
| N03-071 | `EventSubscriberTests` | class | EventSubscriber testleri (xUnit) | VersaCoder.Abstractions | net48;net10.0-windows | - | planli |
| N03-072 | `EventSubscriberBuilder` | class | EventSubscriber uretici (builder) | VersaCoder.Abstractions | net48;net10.0-windows | EventSubscriberTests | planli |
| N03-073 | `IEventPublisher` | interface | EventPublisher sozlesmesi (port) | VersaCoder.Abstractions | net48;net10.0-windows | EventPublisherTests | planli |
| N03-074 | `EventPublisher` | class | EventPublisher cekirdek uygulamasi | VersaCoder.Abstractions | net48;net10.0-windows | EventPublisherTests | planli |
| N03-075 | `EventPublisherDto` | record | EventPublisher veri transferi modeli | VersaCoder.Abstractions | net48;net10.0-windows | EventPublisherTests | planli |
| N03-076 | `EventPublisherMapper` | class | EventPublisher <-> DTO esleme | VersaCoder.Abstractions | net48;net10.0-windows | EventPublisherTests | planli |
| N03-077 | `EventPublisherValidator` | class | EventPublisher dogrulama kurallari | VersaCoder.Abstractions | net48;net10.0-windows | EventPublisherTests | planli |
| N03-078 | `EventPublisherFactory` | class | EventPublisher uretimi ve varsayilanlari | VersaCoder.Abstractions | net48;net10.0-windows | EventPublisherTests | planli |
| N03-079 | `EventPublisherOptions` | record | EventPublisher yapilandirmasi | VersaCoder.Abstractions | net48;net10.0-windows | EventPublisherTests | planli |
| N03-080 | `EventPublisherProvider` | class | EventPublisher saglayici uygulamasi | VersaCoder.Abstractions | net48;net10.0-windows | EventPublisherTests | planli |
| N03-081 | `EventPublisherRegistry` | class | EventPublisher kayit/katalog yonetimi | VersaCoder.Abstractions | net48;net10.0-windows | EventPublisherTests | planli |
| N03-082 | `EventPublisherConverter` | class | EventPublisher bicim donusumu | VersaCoder.Abstractions | net48;net10.0-windows | EventPublisherTests | planli |
| N03-083 | `EventPublisherTests` | class | EventPublisher testleri (xUnit) | VersaCoder.Abstractions | net48;net10.0-windows | - | planli |
| N03-084 | `EventPublisherBuilder` | class | EventPublisher uretici (builder) | VersaCoder.Abstractions | net48;net10.0-windows | EventPublisherTests | planli |
| N03-085 | `IEventFilter` | interface | EventFilter sozlesmesi (port) | VersaCoder.Abstractions | net48;net10.0-windows | EventFilterTests | planli |
| N03-086 | `EventFilter` | class | EventFilter cekirdek uygulamasi | VersaCoder.Abstractions | net48;net10.0-windows | EventFilterTests | planli |
| N03-087 | `EventFilterDto` | record | EventFilter veri transferi modeli | VersaCoder.Abstractions | net48;net10.0-windows | EventFilterTests | planli |
| N03-088 | `EventFilterMapper` | class | EventFilter <-> DTO esleme | VersaCoder.Abstractions | net48;net10.0-windows | EventFilterTests | planli |
| N03-089 | `EventFilterValidator` | class | EventFilter dogrulama kurallari | VersaCoder.Abstractions | net48;net10.0-windows | EventFilterTests | planli |
| N03-090 | `EventFilterFactory` | class | EventFilter uretimi ve varsayilanlari | VersaCoder.Abstractions | net48;net10.0-windows | EventFilterTests | planli |
| N03-091 | `EventFilterOptions` | record | EventFilter yapilandirmasi | VersaCoder.Abstractions | net48;net10.0-windows | EventFilterTests | planli |
| N03-092 | `EventFilterProvider` | class | EventFilter saglayici uygulamasi | VersaCoder.Abstractions | net48;net10.0-windows | EventFilterTests | planli |
| N03-093 | `EventFilterRegistry` | class | EventFilter kayit/katalog yonetimi | VersaCoder.Abstractions | net48;net10.0-windows | EventFilterTests | planli |
| N03-094 | `EventFilterConverter` | class | EventFilter bicim donusumu | VersaCoder.Abstractions | net48;net10.0-windows | EventFilterTests | planli |
| N03-095 | `EventFilterTests` | class | EventFilter testleri (xUnit) | VersaCoder.Abstractions | net48;net10.0-windows | - | planli |
| N03-096 | `EventFilterBuilder` | class | EventFilter uretici (builder) | VersaCoder.Abstractions | net48;net10.0-windows | EventFilterTests | planli |
| N03-097 | `IEventLog` | interface | EventLog sozlesmesi (port) | VersaCoder.Abstractions | net48;net10.0-windows | EventLogTests | planli |
| N03-098 | `EventLog` | class | EventLog cekirdek uygulamasi | VersaCoder.Abstractions | net48;net10.0-windows | EventLogTests | planli |
| N03-099 | `EventLogDto` | record | EventLog veri transferi modeli | VersaCoder.Abstractions | net48;net10.0-windows | EventLogTests | planli |
| N03-100 | `EventLogMapper` | class | EventLog <-> DTO esleme | VersaCoder.Abstractions | net48;net10.0-windows | EventLogTests | planli |
| N03-101 | `EventLogValidator` | class | EventLog dogrulama kurallari | VersaCoder.Abstractions | net48;net10.0-windows | EventLogTests | planli |
| N03-102 | `EventLogFactory` | class | EventLog uretimi ve varsayilanlari | VersaCoder.Abstractions | net48;net10.0-windows | EventLogTests | planli |
| N03-103 | `EventLogOptions` | record | EventLog yapilandirmasi | VersaCoder.Abstractions | net48;net10.0-windows | EventLogTests | planli |
| N03-104 | `EventLogProvider` | class | EventLog saglayici uygulamasi | VersaCoder.Abstractions | net48;net10.0-windows | EventLogTests | planli |
| N03-105 | `EventLogRegistry` | class | EventLog kayit/katalog yonetimi | VersaCoder.Abstractions | net48;net10.0-windows | EventLogTests | planli |
| N03-106 | `EventLogConverter` | class | EventLog bicim donusumu | VersaCoder.Abstractions | net48;net10.0-windows | EventLogTests | planli |
| N03-107 | `EventLogTests` | class | EventLog testleri (xUnit) | VersaCoder.Abstractions | net48;net10.0-windows | - | planli |
| N03-108 | `EventLogBuilder` | class | EventLog uretici (builder) | VersaCoder.Abstractions | net48;net10.0-windows | EventLogTests | planli |
| N03-109 | `IEventClock` | interface | EventClock sozlesmesi (port) | VersaCoder.Abstractions | net48;net10.0-windows | EventClockTests | planli |
| N03-110 | `EventClock` | class | EventClock cekirdek uygulamasi | VersaCoder.Abstractions | net48;net10.0-windows | EventClockTests | planli |
| N03-111 | `EventClockDto` | record | EventClock veri transferi modeli | VersaCoder.Abstractions | net48;net10.0-windows | EventClockTests | planli |
| N03-112 | `EventClockMapper` | class | EventClock <-> DTO esleme | VersaCoder.Abstractions | net48;net10.0-windows | EventClockTests | planli |
| N03-113 | `EventClockValidator` | class | EventClock dogrulama kurallari | VersaCoder.Abstractions | net48;net10.0-windows | EventClockTests | planli |
| N03-114 | `EventClockFactory` | class | EventClock uretimi ve varsayilanlari | VersaCoder.Abstractions | net48;net10.0-windows | EventClockTests | planli |
| N03-115 | `EventClockOptions` | record | EventClock yapilandirmasi | VersaCoder.Abstractions | net48;net10.0-windows | EventClockTests | planli |
| N03-116 | `EventClockProvider` | class | EventClock saglayici uygulamasi | VersaCoder.Abstractions | net48;net10.0-windows | EventClockTests | planli |
| N03-117 | `EventClockRegistry` | class | EventClock kayit/katalog yonetimi | VersaCoder.Abstractions | net48;net10.0-windows | EventClockTests | planli |
| N03-118 | `EventClockConverter` | class | EventClock bicim donusumu | VersaCoder.Abstractions | net48;net10.0-windows | EventClockTests | planli |
| N03-119 | `EventClockTests` | class | EventClock testleri (xUnit) | VersaCoder.Abstractions | net48;net10.0-windows | - | planli |
| N03-120 | `EventClockBuilder` | class | EventClock uretici (builder) | VersaCoder.Abstractions | net48;net10.0-windows | EventClockTests | planli |
