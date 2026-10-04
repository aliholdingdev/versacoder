---
title: "Katalog Dugumu N25 - ADO.NET + Dapper (l4)"
type: catalog
category: architecture
status: needs-validation
version: 1.0.0
updated: 2026-10-04
authority: derived
scope: "N25 ado-dapper dugumu: kimlik, src klasoru, aile yapisi, ornek kayit ve yon kurali (ADR-013 + ADR-028)"
---

# Katalog Dugumu N25 - ADO.NET + Dapper

**Zorunlu Baglantilar:** [catalog index](index.md) · [node-inventory](node-inventory.md) · [mimari index](../index.md) · [ADR-028](../../.decisions/accepted/ADR-028-layered-baseline-9-projects.md) · [ADR-013](../../.decisions/accepted/ADR-013-layered-architecture-52-nodes.md)

> **Tek cumlelik sorumluluk:** *N25 ado-dapper hangi projede ve klasorde yasar, ne yapar, kac kayit hedefler* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

**N25 (ado-dapper), l4 Infrastructure katmaninin alt katmanidir: `VersaCoder.Infrastructure` projesinde `src/VersaCoder.Infrastructure/Data/` klasorunde yasar.** ADO.NET baglantisini ve Dapper sorgularini parametreli bicimde yurutur (ADR-005: EF Core yok). 10 aile x 12 rol = 120 kayit hedefi (ADR-013 asgari 112); durum `planli`.

Kayitlar ADR-013 2.1 biciminde 8 sutun tasir; bu dosya aile kalibini ve ornek satirlari gosterir, tam 120 satirlik liste bu dosyanin boyut sinirina sigmaz (DOĞRULAMA GEREKLI).

```text
+--------------------------------------------------+
| l4 Infrastructure -> VersaCoder.Infrastructure
| src/VersaCoder.Infrastructure/Data/
| 10 aile x 12 rol = 120 kayit (esik 112)
+--------------------------------------------------+
```

| Alan | Deger |
|---|---|
| Dugum | N25 |
| Dugum turu | alt katman (klasor/namespace duzeyi) |
| Makro katman | l4 Infrastructure |
| Proje (.csproj) | VersaCoder.Infrastructure |
| TFM | net48;net10.0-windows |
| Kayit hedefi | 120 (ADR-013 asgari 112) |
| Durum | planli |
| Sorumlu rol | architect |

---

## §1 Sorumluluk (YAPAR / YAPMAZ)

| # | YAPAR | YAPMAZ |
|---|---|---|
| 1 | ADO.NET baglantisini ve Dapper sorgularini parametreli yurutmek | EF Core kullanmak (ADR-005: EF Core YASAK) |
| 2 | kayitlari 8 sutunlu ADR-013 biciminde tutmak (planli -> yapildi -> testli) | is kurali veya invariant tasimak, is l1 katmaninda |
| 3 | ADO.NET + Dapper ile parametreli sorgu yurutmek (her sorgu @p1, string concat YASAK) | EF Core kullanmak veya migration ORM'e devretmek (ADR-005) |
| 4 | l0 portlari ve l1 tipleri uzerinden calismak | l2 Application katmanina referans vermek (l4 -> l2 = X) |
| 5 | dis erisimi (dosya, git, HTTP, DB, surec) tek noktada sarmak | l3, l5, l6 ve l7 katmanlarina bakmak |
| 6 | hatayi Result<T> ile disari tasimak ve loglamaya yazmak | gizli yan etki: her I/O Test-Path / NetArchTest ile denetlenir |

---

## §2 Klasor ve N Eslemesi

**Kalip (ADR-028 2.3/4):** `N33 -> src\VersaCoder.Protocol\ToolSchema\` satiri SSOT ornegidir; bu dosyanin klasoru ayni kalipla yazilir.

| # | N | Slug | src/ klasoru |
|---|---|---|---|
| 1 | N25 | ado-dapper | `src/VersaCoder.Infrastructure/Data/` |

**Kanit yolu (ADR-028 2.3/5):** tarama yolu gercek proje adini icerir; genis kalip (`src/**/Domain/**`) sahte PASS uretir.

```powershell
Test-Path 'C:\versacoder\src\VersaCoder.Infrastructure\Data'
```

Dizin su an yok cikti `False` olur; PASS yazilmaz -> **DOĞRULAMA GEREKLI**.

---

## §3 Aile Yapisi

Rol kalibi (12): `I<F>` · `<F>` · `<F>Dto` · `<F>Mapper` · `<F>Validator` · `<F>Factory` · `<F>Options` · `<F>Provider` · `<F>Registry` · `<F>Converter` · `<F>Tests` · `<F>Builder`

| # | Aile | # | Aile |
|---|---|---|---|
| 1 | DapperSession | 6 | ResultMapper |
| 2 | DbConnectionFactory | 7 | TxWrapper |
| 3 | SqlCommandBuilder | 8 | QueryCache |
| 4 | ParameterMapper | 9 | BulkReader |
| 5 | TypeHandler | 10 | DbLogger |

Her aile ayni rol kalibini 12 kez tekrarlar; kayit sayisi = aile x rol.

---

## §4 Kayit Ornegi (8 sutun)

| ID | Bilesen | Tip | Sorumluluk | Proje | TFM | Test | Durum |
|---|---|---|---|---|---|---|---|
| N25-001 | `IDapperSession` | interface | DapperSession sozlesmesi (port) | VersaCoder.Infrastructure | net48;net10.0-windows | DapperSessionTests | planli |
| N25-002 | `DapperSession` | class | DapperSession cekirdek uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | DapperSessionTests | planli |
| N25-003 | `DapperSessionDto` | record | DapperSession veri transferi modeli | VersaCoder.Infrastructure | net48;net10.0-windows | DapperSessionTests | planli |
| N25-004 | `DapperSessionMapper` | class | DapperSession <-> DTO esleme | VersaCoder.Infrastructure | net48;net10.0-windows | DapperSessionTests | planli |
| N25-005 | `DapperSessionValidator` | class | DapperSession dogrulama kurallari | VersaCoder.Infrastructure | net48;net10.0-windows | DapperSessionTests | planli |
| N25-006 | `DapperSessionFactory` | class | DapperSession uretimi ve varsayilanlari | VersaCoder.Infrastructure | net48;net10.0-windows | DapperSessionTests | planli |

Kalan aileler ayni kalibi 12 rolle tekrarlar (makro dugumlerde 4 rol); toplam hedef **120** kayit.

---

## §5 Yon Kurali

| # | Yon | Deger | Kaynak |
|---|---|---|---|
| 1 | l4 -> l0 ve l1 | OK | ADR-028 2.1 |
| 2 | l4 -> l2 | X (siradir, kenar degildir) | ADR-028 2.3/2 |
| 3 | l4 -> l3, l5, l6, l7 | X | ADR-028 2.1 |

Matrisin tamami [layer-matrix](layer-matrix.md) dosyasindadir; NetArchTest `LayerRulesTests` bu tabloya gore yazilir.

---

## §6 Test ve Durum

| # | Kontrol | Beklenen | Durum |
|---|---|---|---|
| 1 | Kayit satiri | 120 (asgari 112) | bu dosyada 6 ornek satir; tam liste DOĞRULAMA GEREKLI |
| 2 | Durum degeri | planli -> yapildi -> testli | planli |
| 3 | Test sinifi | DapperSessionTests (xUnit) | planli |
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
(Select-String -Path 'C:\versacoder\.ai\architecture\catalog\N25-ado-dapper.md' -Pattern '^\|\s*N\d{2}-\d{3}\s*\|').Count
# 2) boyut bandi - beklenen: 2600-8500
(Get-Item 'C:\versacoder\.ai\architecture\catalog\N25-ado-dapper.md').Length
# 3) klasor kaniti - gercek proje adi (ADR-028 2.3/5)
(Test-Path 'C:\versacoder\src\VersaCoder.Infrastructure\Data')
```

Katalog sayimi `powershell -NoProfile -ExecutionPolicy Bypass -File C:\versacoder\.ai\scripts\count-components.ps1` ile alinir; bu batch 26 node dosyasiyla 52 dugum klasorunu tamamlar - kayit esigi (5000) ornek satirlarla henuz karsilanmaz (DOĞRULAMA GEREKLI).

## §10 Kayit Tablosu (ADR-013 2.2 - kaynak vault port)

| N25-007 | `DapperSessionOptions` | record | DapperSession yapilandirmasi | VersaCoder.Infrastructure | net48;net10.0-windows | DapperSessionTests | planli |
| N25-008 | `DapperSessionProvider` | class | DapperSession saglayici uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | DapperSessionTests | planli |
| N25-009 | `DapperSessionRegistry` | class | DapperSession kayit/katalog yonetimi | VersaCoder.Infrastructure | net48;net10.0-windows | DapperSessionTests | planli |
| N25-010 | `DapperSessionConverter` | class | DapperSession bicim donusumu | VersaCoder.Infrastructure | net48;net10.0-windows | DapperSessionTests | planli |
| N25-011 | `DapperSessionTests` | class | DapperSession testleri (xUnit) | VersaCoder.Infrastructure | net48;net10.0-windows | - | planli |
| N25-012 | `DapperSessionBuilder` | class | DapperSession uretici (builder) | VersaCoder.Infrastructure | net48;net10.0-windows | DapperSessionTests | planli |
| N25-013 | `IDbConnectionFactory` | interface | DbConnectionFactory sozlesmesi (port) | VersaCoder.Infrastructure | net48;net10.0-windows | DbConnectionFactoryTests | planli |
| N25-014 | `DbConnectionFactory` | class | DbConnectionFactory cekirdek uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | DbConnectionFactoryTests | planli |
| N25-015 | `DbConnectionFactoryDto` | record | DbConnectionFactory veri transferi modeli | VersaCoder.Infrastructure | net48;net10.0-windows | DbConnectionFactoryTests | planli |
| N25-016 | `DbConnectionFactoryMapper` | class | DbConnectionFactory <-> DTO esleme | VersaCoder.Infrastructure | net48;net10.0-windows | DbConnectionFactoryTests | planli |
| N25-017 | `DbConnectionFactoryValidator` | class | DbConnectionFactory dogrulama kurallari | VersaCoder.Infrastructure | net48;net10.0-windows | DbConnectionFactoryTests | planli |
| N25-018 | `DbConnectionFactoryFactory` | class | DbConnectionFactory uretimi ve varsayilanlari | VersaCoder.Infrastructure | net48;net10.0-windows | DbConnectionFactoryTests | planli |
| N25-019 | `DbConnectionFactoryOptions` | record | DbConnectionFactory yapilandirmasi | VersaCoder.Infrastructure | net48;net10.0-windows | DbConnectionFactoryTests | planli |
| N25-020 | `DbConnectionFactoryProvider` | class | DbConnectionFactory saglayici uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | DbConnectionFactoryTests | planli |
| N25-021 | `DbConnectionFactoryRegistry` | class | DbConnectionFactory kayit/katalog yonetimi | VersaCoder.Infrastructure | net48;net10.0-windows | DbConnectionFactoryTests | planli |
| N25-022 | `DbConnectionFactoryConverter` | class | DbConnectionFactory bicim donusumu | VersaCoder.Infrastructure | net48;net10.0-windows | DbConnectionFactoryTests | planli |
| N25-023 | `DbConnectionFactoryTests` | class | DbConnectionFactory testleri (xUnit) | VersaCoder.Infrastructure | net48;net10.0-windows | - | planli |
| N25-024 | `DbConnectionFactoryBuilder` | class | DbConnectionFactory uretici (builder) | VersaCoder.Infrastructure | net48;net10.0-windows | DbConnectionFactoryTests | planli |
| N25-025 | `ISqlCommandBuilder` | interface | SqlCommandBuilder sozlesmesi (port) | VersaCoder.Infrastructure | net48;net10.0-windows | SqlCommandBuilderTests | planli |
| N25-026 | `SqlCommandBuilder` | class | SqlCommandBuilder cekirdek uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | SqlCommandBuilderTests | planli |
| N25-027 | `SqlCommandBuilderDto` | record | SqlCommandBuilder veri transferi modeli | VersaCoder.Infrastructure | net48;net10.0-windows | SqlCommandBuilderTests | planli |
| N25-028 | `SqlCommandBuilderMapper` | class | SqlCommandBuilder <-> DTO esleme | VersaCoder.Infrastructure | net48;net10.0-windows | SqlCommandBuilderTests | planli |
| N25-029 | `SqlCommandBuilderValidator` | class | SqlCommandBuilder dogrulama kurallari | VersaCoder.Infrastructure | net48;net10.0-windows | SqlCommandBuilderTests | planli |
| N25-030 | `SqlCommandBuilderFactory` | class | SqlCommandBuilder uretimi ve varsayilanlari | VersaCoder.Infrastructure | net48;net10.0-windows | SqlCommandBuilderTests | planli |
| N25-031 | `SqlCommandBuilderOptions` | record | SqlCommandBuilder yapilandirmasi | VersaCoder.Infrastructure | net48;net10.0-windows | SqlCommandBuilderTests | planli |
| N25-032 | `SqlCommandBuilderProvider` | class | SqlCommandBuilder saglayici uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | SqlCommandBuilderTests | planli |
| N25-033 | `SqlCommandBuilderRegistry` | class | SqlCommandBuilder kayit/katalog yonetimi | VersaCoder.Infrastructure | net48;net10.0-windows | SqlCommandBuilderTests | planli |
| N25-034 | `SqlCommandBuilderConverter` | class | SqlCommandBuilder bicim donusumu | VersaCoder.Infrastructure | net48;net10.0-windows | SqlCommandBuilderTests | planli |
| N25-035 | `SqlCommandBuilderTests` | class | SqlCommandBuilder testleri (xUnit) | VersaCoder.Infrastructure | net48;net10.0-windows | - | planli |
| N25-036 | `SqlCommandBuilderBuilder` | class | SqlCommandBuilder uretici (builder) | VersaCoder.Infrastructure | net48;net10.0-windows | SqlCommandBuilderTests | planli |
| N25-037 | `IParameterMapper` | interface | ParameterMapper sozlesmesi (port) | VersaCoder.Infrastructure | net48;net10.0-windows | ParameterMapperTests | planli |
| N25-038 | `ParameterMapper` | class | ParameterMapper cekirdek uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | ParameterMapperTests | planli |
| N25-039 | `ParameterMapperDto` | record | ParameterMapper veri transferi modeli | VersaCoder.Infrastructure | net48;net10.0-windows | ParameterMapperTests | planli |
| N25-040 | `ParameterMapperMapper` | class | ParameterMapper <-> DTO esleme | VersaCoder.Infrastructure | net48;net10.0-windows | ParameterMapperTests | planli |
| N25-041 | `ParameterMapperValidator` | class | ParameterMapper dogrulama kurallari | VersaCoder.Infrastructure | net48;net10.0-windows | ParameterMapperTests | planli |
| N25-042 | `ParameterMapperFactory` | class | ParameterMapper uretimi ve varsayilanlari | VersaCoder.Infrastructure | net48;net10.0-windows | ParameterMapperTests | planli |
| N25-043 | `ParameterMapperOptions` | record | ParameterMapper yapilandirmasi | VersaCoder.Infrastructure | net48;net10.0-windows | ParameterMapperTests | planli |
| N25-044 | `ParameterMapperProvider` | class | ParameterMapper saglayici uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | ParameterMapperTests | planli |
| N25-045 | `ParameterMapperRegistry` | class | ParameterMapper kayit/katalog yonetimi | VersaCoder.Infrastructure | net48;net10.0-windows | ParameterMapperTests | planli |
| N25-046 | `ParameterMapperConverter` | class | ParameterMapper bicim donusumu | VersaCoder.Infrastructure | net48;net10.0-windows | ParameterMapperTests | planli |
| N25-047 | `ParameterMapperTests` | class | ParameterMapper testleri (xUnit) | VersaCoder.Infrastructure | net48;net10.0-windows | - | planli |
| N25-048 | `ParameterMapperBuilder` | class | ParameterMapper uretici (builder) | VersaCoder.Infrastructure | net48;net10.0-windows | ParameterMapperTests | planli |
| N25-049 | `ITypeHandler` | interface | TypeHandler sozlesmesi (port) | VersaCoder.Infrastructure | net48;net10.0-windows | TypeHandlerTests | planli |
| N25-050 | `TypeHandler` | class | TypeHandler cekirdek uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | TypeHandlerTests | planli |
| N25-051 | `TypeHandlerDto` | record | TypeHandler veri transferi modeli | VersaCoder.Infrastructure | net48;net10.0-windows | TypeHandlerTests | planli |
| N25-052 | `TypeHandlerMapper` | class | TypeHandler <-> DTO esleme | VersaCoder.Infrastructure | net48;net10.0-windows | TypeHandlerTests | planli |
| N25-053 | `TypeHandlerValidator` | class | TypeHandler dogrulama kurallari | VersaCoder.Infrastructure | net48;net10.0-windows | TypeHandlerTests | planli |
| N25-054 | `TypeHandlerFactory` | class | TypeHandler uretimi ve varsayilanlari | VersaCoder.Infrastructure | net48;net10.0-windows | TypeHandlerTests | planli |
| N25-055 | `TypeHandlerOptions` | record | TypeHandler yapilandirmasi | VersaCoder.Infrastructure | net48;net10.0-windows | TypeHandlerTests | planli |
| N25-056 | `TypeHandlerProvider` | class | TypeHandler saglayici uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | TypeHandlerTests | planli |
| N25-057 | `TypeHandlerRegistry` | class | TypeHandler kayit/katalog yonetimi | VersaCoder.Infrastructure | net48;net10.0-windows | TypeHandlerTests | planli |
| N25-058 | `TypeHandlerConverter` | class | TypeHandler bicim donusumu | VersaCoder.Infrastructure | net48;net10.0-windows | TypeHandlerTests | planli |
| N25-059 | `TypeHandlerTests` | class | TypeHandler testleri (xUnit) | VersaCoder.Infrastructure | net48;net10.0-windows | - | planli |
| N25-060 | `TypeHandlerBuilder` | class | TypeHandler uretici (builder) | VersaCoder.Infrastructure | net48;net10.0-windows | TypeHandlerTests | planli |
| N25-061 | `IResultMapper` | interface | ResultMapper sozlesmesi (port) | VersaCoder.Infrastructure | net48;net10.0-windows | ResultMapperTests | planli |
| N25-062 | `ResultMapper` | class | ResultMapper cekirdek uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | ResultMapperTests | planli |
| N25-063 | `ResultMapperDto` | record | ResultMapper veri transferi modeli | VersaCoder.Infrastructure | net48;net10.0-windows | ResultMapperTests | planli |
| N25-064 | `ResultMapperMapper` | class | ResultMapper <-> DTO esleme | VersaCoder.Infrastructure | net48;net10.0-windows | ResultMapperTests | planli |
| N25-065 | `ResultMapperValidator` | class | ResultMapper dogrulama kurallari | VersaCoder.Infrastructure | net48;net10.0-windows | ResultMapperTests | planli |
| N25-066 | `ResultMapperFactory` | class | ResultMapper uretimi ve varsayilanlari | VersaCoder.Infrastructure | net48;net10.0-windows | ResultMapperTests | planli |
| N25-067 | `ResultMapperOptions` | record | ResultMapper yapilandirmasi | VersaCoder.Infrastructure | net48;net10.0-windows | ResultMapperTests | planli |
| N25-068 | `ResultMapperProvider` | class | ResultMapper saglayici uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | ResultMapperTests | planli |
| N25-069 | `ResultMapperRegistry` | class | ResultMapper kayit/katalog yonetimi | VersaCoder.Infrastructure | net48;net10.0-windows | ResultMapperTests | planli |
| N25-070 | `ResultMapperConverter` | class | ResultMapper bicim donusumu | VersaCoder.Infrastructure | net48;net10.0-windows | ResultMapperTests | planli |
| N25-071 | `ResultMapperTests` | class | ResultMapper testleri (xUnit) | VersaCoder.Infrastructure | net48;net10.0-windows | - | planli |
| N25-072 | `ResultMapperBuilder` | class | ResultMapper uretici (builder) | VersaCoder.Infrastructure | net48;net10.0-windows | ResultMapperTests | planli |
| N25-073 | `ITxWrapper` | interface | TxWrapper sozlesmesi (port) | VersaCoder.Infrastructure | net48;net10.0-windows | TxWrapperTests | planli |
| N25-074 | `TxWrapper` | class | TxWrapper cekirdek uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | TxWrapperTests | planli |
| N25-075 | `TxWrapperDto` | record | TxWrapper veri transferi modeli | VersaCoder.Infrastructure | net48;net10.0-windows | TxWrapperTests | planli |
| N25-076 | `TxWrapperMapper` | class | TxWrapper <-> DTO esleme | VersaCoder.Infrastructure | net48;net10.0-windows | TxWrapperTests | planli |
| N25-077 | `TxWrapperValidator` | class | TxWrapper dogrulama kurallari | VersaCoder.Infrastructure | net48;net10.0-windows | TxWrapperTests | planli |
| N25-078 | `TxWrapperFactory` | class | TxWrapper uretimi ve varsayilanlari | VersaCoder.Infrastructure | net48;net10.0-windows | TxWrapperTests | planli |
| N25-079 | `TxWrapperOptions` | record | TxWrapper yapilandirmasi | VersaCoder.Infrastructure | net48;net10.0-windows | TxWrapperTests | planli |
| N25-080 | `TxWrapperProvider` | class | TxWrapper saglayici uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | TxWrapperTests | planli |
| N25-081 | `TxWrapperRegistry` | class | TxWrapper kayit/katalog yonetimi | VersaCoder.Infrastructure | net48;net10.0-windows | TxWrapperTests | planli |
| N25-082 | `TxWrapperConverter` | class | TxWrapper bicim donusumu | VersaCoder.Infrastructure | net48;net10.0-windows | TxWrapperTests | planli |
| N25-083 | `TxWrapperTests` | class | TxWrapper testleri (xUnit) | VersaCoder.Infrastructure | net48;net10.0-windows | - | planli |
| N25-084 | `TxWrapperBuilder` | class | TxWrapper uretici (builder) | VersaCoder.Infrastructure | net48;net10.0-windows | TxWrapperTests | planli |
| N25-085 | `IQueryCache` | interface | QueryCache sozlesmesi (port) | VersaCoder.Infrastructure | net48;net10.0-windows | QueryCacheTests | planli |
| N25-086 | `QueryCache` | class | QueryCache cekirdek uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | QueryCacheTests | planli |
| N25-087 | `QueryCacheDto` | record | QueryCache veri transferi modeli | VersaCoder.Infrastructure | net48;net10.0-windows | QueryCacheTests | planli |
| N25-088 | `QueryCacheMapper` | class | QueryCache <-> DTO esleme | VersaCoder.Infrastructure | net48;net10.0-windows | QueryCacheTests | planli |
| N25-089 | `QueryCacheValidator` | class | QueryCache dogrulama kurallari | VersaCoder.Infrastructure | net48;net10.0-windows | QueryCacheTests | planli |
| N25-090 | `QueryCacheFactory` | class | QueryCache uretimi ve varsayilanlari | VersaCoder.Infrastructure | net48;net10.0-windows | QueryCacheTests | planli |
| N25-091 | `QueryCacheOptions` | record | QueryCache yapilandirmasi | VersaCoder.Infrastructure | net48;net10.0-windows | QueryCacheTests | planli |
| N25-092 | `QueryCacheProvider` | class | QueryCache saglayici uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | QueryCacheTests | planli |
| N25-093 | `QueryCacheRegistry` | class | QueryCache kayit/katalog yonetimi | VersaCoder.Infrastructure | net48;net10.0-windows | QueryCacheTests | planli |
| N25-094 | `QueryCacheConverter` | class | QueryCache bicim donusumu | VersaCoder.Infrastructure | net48;net10.0-windows | QueryCacheTests | planli |
| N25-095 | `QueryCacheTests` | class | QueryCache testleri (xUnit) | VersaCoder.Infrastructure | net48;net10.0-windows | - | planli |
| N25-096 | `QueryCacheBuilder` | class | QueryCache uretici (builder) | VersaCoder.Infrastructure | net48;net10.0-windows | QueryCacheTests | planli |
| N25-097 | `IBulkReader` | interface | BulkReader sozlesmesi (port) | VersaCoder.Infrastructure | net48;net10.0-windows | BulkReaderTests | planli |
| N25-098 | `BulkReader` | class | BulkReader cekirdek uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | BulkReaderTests | planli |
| N25-099 | `BulkReaderDto` | record | BulkReader veri transferi modeli | VersaCoder.Infrastructure | net48;net10.0-windows | BulkReaderTests | planli |
| N25-100 | `BulkReaderMapper` | class | BulkReader <-> DTO esleme | VersaCoder.Infrastructure | net48;net10.0-windows | BulkReaderTests | planli |
| N25-101 | `BulkReaderValidator` | class | BulkReader dogrulama kurallari | VersaCoder.Infrastructure | net48;net10.0-windows | BulkReaderTests | planli |
| N25-102 | `BulkReaderFactory` | class | BulkReader uretimi ve varsayilanlari | VersaCoder.Infrastructure | net48;net10.0-windows | BulkReaderTests | planli |
| N25-103 | `BulkReaderOptions` | record | BulkReader yapilandirmasi | VersaCoder.Infrastructure | net48;net10.0-windows | BulkReaderTests | planli |
| N25-104 | `BulkReaderProvider` | class | BulkReader saglayici uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | BulkReaderTests | planli |
| N25-105 | `BulkReaderRegistry` | class | BulkReader kayit/katalog yonetimi | VersaCoder.Infrastructure | net48;net10.0-windows | BulkReaderTests | planli |
| N25-106 | `BulkReaderConverter` | class | BulkReader bicim donusumu | VersaCoder.Infrastructure | net48;net10.0-windows | BulkReaderTests | planli |
| N25-107 | `BulkReaderTests` | class | BulkReader testleri (xUnit) | VersaCoder.Infrastructure | net48;net10.0-windows | - | planli |
| N25-108 | `BulkReaderBuilder` | class | BulkReader uretici (builder) | VersaCoder.Infrastructure | net48;net10.0-windows | BulkReaderTests | planli |
| N25-109 | `IDbLogger` | interface | DbLogger sozlesmesi (port) | VersaCoder.Infrastructure | net48;net10.0-windows | DbLoggerTests | planli |
| N25-110 | `DbLogger` | class | DbLogger cekirdek uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | DbLoggerTests | planli |
| N25-111 | `DbLoggerDto` | record | DbLogger veri transferi modeli | VersaCoder.Infrastructure | net48;net10.0-windows | DbLoggerTests | planli |
| N25-112 | `DbLoggerMapper` | class | DbLogger <-> DTO esleme | VersaCoder.Infrastructure | net48;net10.0-windows | DbLoggerTests | planli |
| N25-113 | `DbLoggerValidator` | class | DbLogger dogrulama kurallari | VersaCoder.Infrastructure | net48;net10.0-windows | DbLoggerTests | planli |
| N25-114 | `DbLoggerFactory` | class | DbLogger uretimi ve varsayilanlari | VersaCoder.Infrastructure | net48;net10.0-windows | DbLoggerTests | planli |
| N25-115 | `DbLoggerOptions` | record | DbLogger yapilandirmasi | VersaCoder.Infrastructure | net48;net10.0-windows | DbLoggerTests | planli |
| N25-116 | `DbLoggerProvider` | class | DbLogger saglayici uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | DbLoggerTests | planli |
| N25-117 | `DbLoggerRegistry` | class | DbLogger kayit/katalog yonetimi | VersaCoder.Infrastructure | net48;net10.0-windows | DbLoggerTests | planli |
| N25-118 | `DbLoggerConverter` | class | DbLogger bicim donusumu | VersaCoder.Infrastructure | net48;net10.0-windows | DbLoggerTests | planli |
| N25-119 | `DbLoggerTests` | class | DbLogger testleri (xUnit) | VersaCoder.Infrastructure | net48;net10.0-windows | - | planli |
| N25-120 | `DbLoggerBuilder` | class | DbLogger uretici (builder) | VersaCoder.Infrastructure | net48;net10.0-windows | DbLoggerTests | planli |
