---
title: "Katalog Dugumu N07 - Deger Nesneleri (l1)"
type: catalog
category: architecture
status: needs-validation
version: 1.0.0
updated: 2026-10-04
authority: derived
scope: "N07 value-objects dugumu: kimlik, src klasoru, aile yapisi, ornek kayit ve yon kurali (ADR-013 + ADR-028)"
---

# Katalog Dugumu N07 - Deger Nesneleri

**Zorunlu Baglantilar:** [catalog index](index.md) · [node-inventory](node-inventory.md) · [mimari index](../index.md) · [ADR-028](../../.decisions/accepted/ADR-028-layered-baseline-9-projects.md) · [ADR-013](../../.decisions/accepted/ADR-013-layered-architecture-52-nodes.md)

> **Tek cumlelik sorumluluk:** *N07 value-objects hangi projede ve klasorde yasar, ne yapar, kac kayit hedefler* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

**N07 (value-objects), l1 Domain katmaninin alt katmanidir: `VersaCoder.Domain` projesinde `src/VersaCoder.Domain/ValueObjects/` klasorunde yasar.** SessionId, FilePath, TokenBudget gibi degeri olan nesneleri ve dogrulamalarini tanimlar. 10 aile x 12 rol = 120 kayit hedefi (ADR-013 asgari 112); durum `planli`.

Kayitlar ADR-013 2.1 biciminde 8 sutun tasir; bu dosya aile kalibini ve ornek satirlari gosterir, tam 120 satirlik liste bu dosyanin boyut sinirina sigmaz (DOĞRULAMA GEREKLI).

```text
+--------------------------------------------------+
| l1 Domain -> VersaCoder.Domain
| src/VersaCoder.Domain/ValueObjects/
| 10 aile x 12 rol = 120 kayit (esik 112)
+--------------------------------------------------+
```

| Alan | Deger |
|---|---|
| Dugum | N07 |
| Dugum turu | alt katman (klasor/namespace duzeyi) |
| Makro katman | l1 Domain |
| Proje (.csproj) | VersaCoder.Domain |
| TFM | net48;net10.0-windows |
| Kayit hedefi | 120 (ADR-013 asgari 112) |
| Durum | planli |
| Sorumlu rol | architect |

---

## §1 Sorumluluk (YAPAR / YAPMAZ)

| # | YAPAR | YAPMAZ |
|---|---|---|
| 1 | SessionId, FilePath, TokenBudget gibi degeri olan nesneleri ve dogrulamalarini tanimlar | System.Data, Dapper, dosya veya git cagirmak |
| 2 | kayitlari 8 sutunlu ADR-013 biciminde tutmak (planli -> yapildi -> testli) | kullanim senaryosu orkestre etmek, is l2 katmaninda |
| 3 | entity, value object ve aggregate tasarlamak | SQL veya migration yazmak, is l4 katmaninda |
| 4 | invariant ve is kuralini saf C# icinde kodlamak | UI ekranı veya mesaj kutusu acmak |
| 5 | domain event uretmek | l2..l7 katmanlarina referans vermek |
| 6 | l0 katmaninin sozlesmelerini kullanmak | framework bagimliligi getirmek (or. EntityFramework) |

---

## §2 Klasor ve N Eslemesi

**Kalip (ADR-028 2.3/4):** `N33 -> src\VersaCoder.Protocol\ToolSchema\` satiri SSOT ornegidir; bu dosyanin klasoru ayni kalipla yazilir.

| # | N | Slug | src/ klasoru |
|---|---|---|---|
| 1 | N07 | value-objects | `src/VersaCoder.Domain/ValueObjects/` |

**Kanit yolu (ADR-028 2.3/5):** tarama yolu gercek proje adini icerir; genis kalip (`src/**/Domain/**`) sahte PASS uretir.

```powershell
Test-Path 'C:\versacoder\src\VersaCoder.Domain\ValueObjects'
```

Dizin su an yok cikti `False` olur; PASS yazilmaz -> **DOĞRULAMA GEREKLI**.

---

## §3 Aile Yapisi

Rol kalibi (12): `I<F>` · `<F>` · `<F>Dto` · `<F>Mapper` · `<F>Validator` · `<F>Factory` · `<F>Options` · `<F>Provider` · `<F>Registry` · `<F>Converter` · `<F>Tests` · `<F>Builder`

| # | Aile | # | Aile |
|---|---|---|---|
| 1 | SessionId | 6 | TimeStamp |
| 2 | MessageId | 7 | DurationMs |
| 3 | FilePath | 8 | ProtocolVer |
| 4 | ModelName | 9 | Slug |
| 5 | TokenBudget | 10 | Checksum |

Her aile ayni rol kalibini 12 kez tekrarlar; kayit sayisi = aile x rol.

---

## §4 Kayit Ornegi (8 sutun)

| ID | Bilesen | Tip | Sorumluluk | Proje | TFM | Test | Durum |
|---|---|---|---|---|---|---|---|
| N07-001 | `ISessionId` | interface | SessionId sozlesmesi (port) | VersaCoder.Domain | net48;net10.0-windows | SessionIdTests | planli |
| N07-002 | `SessionId` | class | SessionId cekirdek uygulamasi | VersaCoder.Domain | net48;net10.0-windows | SessionIdTests | planli |
| N07-003 | `SessionIdDto` | record | SessionId veri transferi modeli | VersaCoder.Domain | net48;net10.0-windows | SessionIdTests | planli |
| N07-004 | `SessionIdMapper` | class | SessionId <-> DTO esleme | VersaCoder.Domain | net48;net10.0-windows | SessionIdTests | planli |
| N07-005 | `SessionIdValidator` | class | SessionId dogrulama kurallari | VersaCoder.Domain | net48;net10.0-windows | SessionIdTests | planli |
| N07-006 | `SessionIdFactory` | class | SessionId uretimi ve varsayilanlari | VersaCoder.Domain | net48;net10.0-windows | SessionIdTests | planli |

Kalan aileler ayni kalibi 12 rolle tekrarlar (makro dugumlerde 4 rol); toplam hedef **120** kayit.

---

## §5 Yon Kurali

| # | Yon | Deger | Kaynak |
|---|---|---|---|
| 1 | l1 -> l0 | OK | ADR-028 2.1 |
| 2 | l1 -> l2..l7 | X | ADR-028 2.1 |
| 3 | l2 -> l1 | OK | ADR-028 2.1 |

Matrisin tamami [layer-matrix](layer-matrix.md) dosyasindadir; NetArchTest `LayerRulesTests` bu tabloya gore yazilir.

---

## §6 Test ve Durum

| # | Kontrol | Beklenen | Durum |
|---|---|---|---|
| 1 | Kayit satiri | 120 (asgari 112) | bu dosyada 6 ornek satir; tam liste DOĞRULAMA GEREKLI |
| 2 | Durum degeri | planli -> yapildi -> testli | planli |
| 3 | Test sinifi | SessionIdTests (xUnit) | planli |
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
(Select-String -Path 'C:\versacoder\.ai\architecture\catalog\N07-value-objects.md' -Pattern '^\|\s*N\d{2}-\d{3}\s*\|').Count
# 2) boyut bandi - beklenen: 2600-8500
(Get-Item 'C:\versacoder\.ai\architecture\catalog\N07-value-objects.md').Length
# 3) klasor kaniti - gercek proje adi (ADR-028 2.3/5)
(Test-Path 'C:\versacoder\src\VersaCoder.Domain\ValueObjects')
```

Katalog sayimi `powershell -NoProfile -ExecutionPolicy Bypass -File C:\versacoder\.ai\scripts\count-components.ps1` ile alinir; bu batch sonunda toplam esik (5000) henuz karsilanmaz - kalan 26 node dosyasi sonraki batchtir.

## §10 Kayit Tablosu (ADR-013 2.2 - kaynak vault port)

| N07-007 | `SessionIdOptions` | record | SessionId yapilandirmasi | VersaCoder.Domain | net48;net10.0-windows | SessionIdTests | planli |
| N07-008 | `SessionIdProvider` | class | SessionId saglayici uygulamasi | VersaCoder.Domain | net48;net10.0-windows | SessionIdTests | planli |
| N07-009 | `SessionIdRegistry` | class | SessionId kayit/katalog yonetimi | VersaCoder.Domain | net48;net10.0-windows | SessionIdTests | planli |
| N07-010 | `SessionIdConverter` | class | SessionId bicim donusumu | VersaCoder.Domain | net48;net10.0-windows | SessionIdTests | planli |
| N07-011 | `SessionIdTests` | class | SessionId testleri (xUnit) | VersaCoder.Domain | net48;net10.0-windows | - | planli |
| N07-012 | `SessionIdBuilder` | class | SessionId uretici (builder) | VersaCoder.Domain | net48;net10.0-windows | SessionIdTests | planli |
| N07-013 | `IMessageId` | interface | MessageId sozlesmesi (port) | VersaCoder.Domain | net48;net10.0-windows | MessageIdTests | planli |
| N07-014 | `MessageId` | class | MessageId cekirdek uygulamasi | VersaCoder.Domain | net48;net10.0-windows | MessageIdTests | planli |
| N07-015 | `MessageIdDto` | record | MessageId veri transferi modeli | VersaCoder.Domain | net48;net10.0-windows | MessageIdTests | planli |
| N07-016 | `MessageIdMapper` | class | MessageId <-> DTO esleme | VersaCoder.Domain | net48;net10.0-windows | MessageIdTests | planli |
| N07-017 | `MessageIdValidator` | class | MessageId dogrulama kurallari | VersaCoder.Domain | net48;net10.0-windows | MessageIdTests | planli |
| N07-018 | `MessageIdFactory` | class | MessageId uretimi ve varsayilanlari | VersaCoder.Domain | net48;net10.0-windows | MessageIdTests | planli |
| N07-019 | `MessageIdOptions` | record | MessageId yapilandirmasi | VersaCoder.Domain | net48;net10.0-windows | MessageIdTests | planli |
| N07-020 | `MessageIdProvider` | class | MessageId saglayici uygulamasi | VersaCoder.Domain | net48;net10.0-windows | MessageIdTests | planli |
| N07-021 | `MessageIdRegistry` | class | MessageId kayit/katalog yonetimi | VersaCoder.Domain | net48;net10.0-windows | MessageIdTests | planli |
| N07-022 | `MessageIdConverter` | class | MessageId bicim donusumu | VersaCoder.Domain | net48;net10.0-windows | MessageIdTests | planli |
| N07-023 | `MessageIdTests` | class | MessageId testleri (xUnit) | VersaCoder.Domain | net48;net10.0-windows | - | planli |
| N07-024 | `MessageIdBuilder` | class | MessageId uretici (builder) | VersaCoder.Domain | net48;net10.0-windows | MessageIdTests | planli |
| N07-025 | `IFilePath` | interface | FilePath sozlesmesi (port) | VersaCoder.Domain | net48;net10.0-windows | FilePathTests | planli |
| N07-026 | `FilePath` | class | FilePath cekirdek uygulamasi | VersaCoder.Domain | net48;net10.0-windows | FilePathTests | planli |
| N07-027 | `FilePathDto` | record | FilePath veri transferi modeli | VersaCoder.Domain | net48;net10.0-windows | FilePathTests | planli |
| N07-028 | `FilePathMapper` | class | FilePath <-> DTO esleme | VersaCoder.Domain | net48;net10.0-windows | FilePathTests | planli |
| N07-029 | `FilePathValidator` | class | FilePath dogrulama kurallari | VersaCoder.Domain | net48;net10.0-windows | FilePathTests | planli |
| N07-030 | `FilePathFactory` | class | FilePath uretimi ve varsayilanlari | VersaCoder.Domain | net48;net10.0-windows | FilePathTests | planli |
| N07-031 | `FilePathOptions` | record | FilePath yapilandirmasi | VersaCoder.Domain | net48;net10.0-windows | FilePathTests | planli |
| N07-032 | `FilePathProvider` | class | FilePath saglayici uygulamasi | VersaCoder.Domain | net48;net10.0-windows | FilePathTests | planli |
| N07-033 | `FilePathRegistry` | class | FilePath kayit/katalog yonetimi | VersaCoder.Domain | net48;net10.0-windows | FilePathTests | planli |
| N07-034 | `FilePathConverter` | class | FilePath bicim donusumu | VersaCoder.Domain | net48;net10.0-windows | FilePathTests | planli |
| N07-035 | `FilePathTests` | class | FilePath testleri (xUnit) | VersaCoder.Domain | net48;net10.0-windows | - | planli |
| N07-036 | `FilePathBuilder` | class | FilePath uretici (builder) | VersaCoder.Domain | net48;net10.0-windows | FilePathTests | planli |
| N07-037 | `IModelName` | interface | ModelName sozlesmesi (port) | VersaCoder.Domain | net48;net10.0-windows | ModelNameTests | planli |
| N07-038 | `ModelName` | class | ModelName cekirdek uygulamasi | VersaCoder.Domain | net48;net10.0-windows | ModelNameTests | planli |
| N07-039 | `ModelNameDto` | record | ModelName veri transferi modeli | VersaCoder.Domain | net48;net10.0-windows | ModelNameTests | planli |
| N07-040 | `ModelNameMapper` | class | ModelName <-> DTO esleme | VersaCoder.Domain | net48;net10.0-windows | ModelNameTests | planli |
| N07-041 | `ModelNameValidator` | class | ModelName dogrulama kurallari | VersaCoder.Domain | net48;net10.0-windows | ModelNameTests | planli |
| N07-042 | `ModelNameFactory` | class | ModelName uretimi ve varsayilanlari | VersaCoder.Domain | net48;net10.0-windows | ModelNameTests | planli |
| N07-043 | `ModelNameOptions` | record | ModelName yapilandirmasi | VersaCoder.Domain | net48;net10.0-windows | ModelNameTests | planli |
| N07-044 | `ModelNameProvider` | class | ModelName saglayici uygulamasi | VersaCoder.Domain | net48;net10.0-windows | ModelNameTests | planli |
| N07-045 | `ModelNameRegistry` | class | ModelName kayit/katalog yonetimi | VersaCoder.Domain | net48;net10.0-windows | ModelNameTests | planli |
| N07-046 | `ModelNameConverter` | class | ModelName bicim donusumu | VersaCoder.Domain | net48;net10.0-windows | ModelNameTests | planli |
| N07-047 | `ModelNameTests` | class | ModelName testleri (xUnit) | VersaCoder.Domain | net48;net10.0-windows | - | planli |
| N07-048 | `ModelNameBuilder` | class | ModelName uretici (builder) | VersaCoder.Domain | net48;net10.0-windows | ModelNameTests | planli |
| N07-049 | `ITokenBudget` | interface | TokenBudget sozlesmesi (port) | VersaCoder.Domain | net48;net10.0-windows | TokenBudgetTests | planli |
| N07-050 | `TokenBudget` | class | TokenBudget cekirdek uygulamasi | VersaCoder.Domain | net48;net10.0-windows | TokenBudgetTests | planli |
| N07-051 | `TokenBudgetDto` | record | TokenBudget veri transferi modeli | VersaCoder.Domain | net48;net10.0-windows | TokenBudgetTests | planli |
| N07-052 | `TokenBudgetMapper` | class | TokenBudget <-> DTO esleme | VersaCoder.Domain | net48;net10.0-windows | TokenBudgetTests | planli |
| N07-053 | `TokenBudgetValidator` | class | TokenBudget dogrulama kurallari | VersaCoder.Domain | net48;net10.0-windows | TokenBudgetTests | planli |
| N07-054 | `TokenBudgetFactory` | class | TokenBudget uretimi ve varsayilanlari | VersaCoder.Domain | net48;net10.0-windows | TokenBudgetTests | planli |
| N07-055 | `TokenBudgetOptions` | record | TokenBudget yapilandirmasi | VersaCoder.Domain | net48;net10.0-windows | TokenBudgetTests | planli |
| N07-056 | `TokenBudgetProvider` | class | TokenBudget saglayici uygulamasi | VersaCoder.Domain | net48;net10.0-windows | TokenBudgetTests | planli |
| N07-057 | `TokenBudgetRegistry` | class | TokenBudget kayit/katalog yonetimi | VersaCoder.Domain | net48;net10.0-windows | TokenBudgetTests | planli |
| N07-058 | `TokenBudgetConverter` | class | TokenBudget bicim donusumu | VersaCoder.Domain | net48;net10.0-windows | TokenBudgetTests | planli |
| N07-059 | `TokenBudgetTests` | class | TokenBudget testleri (xUnit) | VersaCoder.Domain | net48;net10.0-windows | - | planli |
| N07-060 | `TokenBudgetBuilder` | class | TokenBudget uretici (builder) | VersaCoder.Domain | net48;net10.0-windows | TokenBudgetTests | planli |
| N07-061 | `ITimeStamp` | interface | TimeStamp sozlesmesi (port) | VersaCoder.Domain | net48;net10.0-windows | TimeStampTests | planli |
| N07-062 | `TimeStamp` | class | TimeStamp cekirdek uygulamasi | VersaCoder.Domain | net48;net10.0-windows | TimeStampTests | planli |
| N07-063 | `TimeStampDto` | record | TimeStamp veri transferi modeli | VersaCoder.Domain | net48;net10.0-windows | TimeStampTests | planli |
| N07-064 | `TimeStampMapper` | class | TimeStamp <-> DTO esleme | VersaCoder.Domain | net48;net10.0-windows | TimeStampTests | planli |
| N07-065 | `TimeStampValidator` | class | TimeStamp dogrulama kurallari | VersaCoder.Domain | net48;net10.0-windows | TimeStampTests | planli |
| N07-066 | `TimeStampFactory` | class | TimeStamp uretimi ve varsayilanlari | VersaCoder.Domain | net48;net10.0-windows | TimeStampTests | planli |
| N07-067 | `TimeStampOptions` | record | TimeStamp yapilandirmasi | VersaCoder.Domain | net48;net10.0-windows | TimeStampTests | planli |
| N07-068 | `TimeStampProvider` | class | TimeStamp saglayici uygulamasi | VersaCoder.Domain | net48;net10.0-windows | TimeStampTests | planli |
| N07-069 | `TimeStampRegistry` | class | TimeStamp kayit/katalog yonetimi | VersaCoder.Domain | net48;net10.0-windows | TimeStampTests | planli |
| N07-070 | `TimeStampConverter` | class | TimeStamp bicim donusumu | VersaCoder.Domain | net48;net10.0-windows | TimeStampTests | planli |
| N07-071 | `TimeStampTests` | class | TimeStamp testleri (xUnit) | VersaCoder.Domain | net48;net10.0-windows | - | planli |
| N07-072 | `TimeStampBuilder` | class | TimeStamp uretici (builder) | VersaCoder.Domain | net48;net10.0-windows | TimeStampTests | planli |
| N07-073 | `IDurationMs` | interface | DurationMs sozlesmesi (port) | VersaCoder.Domain | net48;net10.0-windows | DurationMsTests | planli |
| N07-074 | `DurationMs` | class | DurationMs cekirdek uygulamasi | VersaCoder.Domain | net48;net10.0-windows | DurationMsTests | planli |
| N07-075 | `DurationMsDto` | record | DurationMs veri transferi modeli | VersaCoder.Domain | net48;net10.0-windows | DurationMsTests | planli |
| N07-076 | `DurationMsMapper` | class | DurationMs <-> DTO esleme | VersaCoder.Domain | net48;net10.0-windows | DurationMsTests | planli |
| N07-077 | `DurationMsValidator` | class | DurationMs dogrulama kurallari | VersaCoder.Domain | net48;net10.0-windows | DurationMsTests | planli |
| N07-078 | `DurationMsFactory` | class | DurationMs uretimi ve varsayilanlari | VersaCoder.Domain | net48;net10.0-windows | DurationMsTests | planli |
| N07-079 | `DurationMsOptions` | record | DurationMs yapilandirmasi | VersaCoder.Domain | net48;net10.0-windows | DurationMsTests | planli |
| N07-080 | `DurationMsProvider` | class | DurationMs saglayici uygulamasi | VersaCoder.Domain | net48;net10.0-windows | DurationMsTests | planli |
| N07-081 | `DurationMsRegistry` | class | DurationMs kayit/katalog yonetimi | VersaCoder.Domain | net48;net10.0-windows | DurationMsTests | planli |
| N07-082 | `DurationMsConverter` | class | DurationMs bicim donusumu | VersaCoder.Domain | net48;net10.0-windows | DurationMsTests | planli |
| N07-083 | `DurationMsTests` | class | DurationMs testleri (xUnit) | VersaCoder.Domain | net48;net10.0-windows | - | planli |
| N07-084 | `DurationMsBuilder` | class | DurationMs uretici (builder) | VersaCoder.Domain | net48;net10.0-windows | DurationMsTests | planli |
| N07-085 | `IProtocolVer` | interface | ProtocolVer sozlesmesi (port) | VersaCoder.Domain | net48;net10.0-windows | ProtocolVerTests | planli |
| N07-086 | `ProtocolVer` | class | ProtocolVer cekirdek uygulamasi | VersaCoder.Domain | net48;net10.0-windows | ProtocolVerTests | planli |
| N07-087 | `ProtocolVerDto` | record | ProtocolVer veri transferi modeli | VersaCoder.Domain | net48;net10.0-windows | ProtocolVerTests | planli |
| N07-088 | `ProtocolVerMapper` | class | ProtocolVer <-> DTO esleme | VersaCoder.Domain | net48;net10.0-windows | ProtocolVerTests | planli |
| N07-089 | `ProtocolVerValidator` | class | ProtocolVer dogrulama kurallari | VersaCoder.Domain | net48;net10.0-windows | ProtocolVerTests | planli |
| N07-090 | `ProtocolVerFactory` | class | ProtocolVer uretimi ve varsayilanlari | VersaCoder.Domain | net48;net10.0-windows | ProtocolVerTests | planli |
| N07-091 | `ProtocolVerOptions` | record | ProtocolVer yapilandirmasi | VersaCoder.Domain | net48;net10.0-windows | ProtocolVerTests | planli |
| N07-092 | `ProtocolVerProvider` | class | ProtocolVer saglayici uygulamasi | VersaCoder.Domain | net48;net10.0-windows | ProtocolVerTests | planli |
| N07-093 | `ProtocolVerRegistry` | class | ProtocolVer kayit/katalog yonetimi | VersaCoder.Domain | net48;net10.0-windows | ProtocolVerTests | planli |
| N07-094 | `ProtocolVerConverter` | class | ProtocolVer bicim donusumu | VersaCoder.Domain | net48;net10.0-windows | ProtocolVerTests | planli |
| N07-095 | `ProtocolVerTests` | class | ProtocolVer testleri (xUnit) | VersaCoder.Domain | net48;net10.0-windows | - | planli |
| N07-096 | `ProtocolVerBuilder` | class | ProtocolVer uretici (builder) | VersaCoder.Domain | net48;net10.0-windows | ProtocolVerTests | planli |
| N07-097 | `ISlug` | interface | Slug sozlesmesi (port) | VersaCoder.Domain | net48;net10.0-windows | SlugTests | planli |
| N07-098 | `Slug` | class | Slug cekirdek uygulamasi | VersaCoder.Domain | net48;net10.0-windows | SlugTests | planli |
| N07-099 | `SlugDto` | record | Slug veri transferi modeli | VersaCoder.Domain | net48;net10.0-windows | SlugTests | planli |
| N07-100 | `SlugMapper` | class | Slug <-> DTO esleme | VersaCoder.Domain | net48;net10.0-windows | SlugTests | planli |
| N07-101 | `SlugValidator` | class | Slug dogrulama kurallari | VersaCoder.Domain | net48;net10.0-windows | SlugTests | planli |
| N07-102 | `SlugFactory` | class | Slug uretimi ve varsayilanlari | VersaCoder.Domain | net48;net10.0-windows | SlugTests | planli |
| N07-103 | `SlugOptions` | record | Slug yapilandirmasi | VersaCoder.Domain | net48;net10.0-windows | SlugTests | planli |
| N07-104 | `SlugProvider` | class | Slug saglayici uygulamasi | VersaCoder.Domain | net48;net10.0-windows | SlugTests | planli |
| N07-105 | `SlugRegistry` | class | Slug kayit/katalog yonetimi | VersaCoder.Domain | net48;net10.0-windows | SlugTests | planli |
| N07-106 | `SlugConverter` | class | Slug bicim donusumu | VersaCoder.Domain | net48;net10.0-windows | SlugTests | planli |
| N07-107 | `SlugTests` | class | Slug testleri (xUnit) | VersaCoder.Domain | net48;net10.0-windows | - | planli |
| N07-108 | `SlugBuilder` | class | Slug uretici (builder) | VersaCoder.Domain | net48;net10.0-windows | SlugTests | planli |
| N07-109 | `IChecksum` | interface | Checksum sozlesmesi (port) | VersaCoder.Domain | net48;net10.0-windows | ChecksumTests | planli |
| N07-110 | `Checksum` | class | Checksum cekirdek uygulamasi | VersaCoder.Domain | net48;net10.0-windows | ChecksumTests | planli |
| N07-111 | `ChecksumDto` | record | Checksum veri transferi modeli | VersaCoder.Domain | net48;net10.0-windows | ChecksumTests | planli |
| N07-112 | `ChecksumMapper` | class | Checksum <-> DTO esleme | VersaCoder.Domain | net48;net10.0-windows | ChecksumTests | planli |
| N07-113 | `ChecksumValidator` | class | Checksum dogrulama kurallari | VersaCoder.Domain | net48;net10.0-windows | ChecksumTests | planli |
| N07-114 | `ChecksumFactory` | class | Checksum uretimi ve varsayilanlari | VersaCoder.Domain | net48;net10.0-windows | ChecksumTests | planli |
| N07-115 | `ChecksumOptions` | record | Checksum yapilandirmasi | VersaCoder.Domain | net48;net10.0-windows | ChecksumTests | planli |
| N07-116 | `ChecksumProvider` | class | Checksum saglayici uygulamasi | VersaCoder.Domain | net48;net10.0-windows | ChecksumTests | planli |
| N07-117 | `ChecksumRegistry` | class | Checksum kayit/katalog yonetimi | VersaCoder.Domain | net48;net10.0-windows | ChecksumTests | planli |
| N07-118 | `ChecksumConverter` | class | Checksum bicim donusumu | VersaCoder.Domain | net48;net10.0-windows | ChecksumTests | planli |
| N07-119 | `ChecksumTests` | class | Checksum testleri (xUnit) | VersaCoder.Domain | net48;net10.0-windows | - | planli |
| N07-120 | `ChecksumBuilder` | class | Checksum uretici (builder) | VersaCoder.Domain | net48;net10.0-windows | ChecksumTests | planli |
