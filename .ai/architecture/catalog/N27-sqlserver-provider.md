---
title: "Katalog Dugumu N27 - SqlServer Saglayici (l4)"
type: catalog
category: architecture
status: needs-validation
version: 1.0.0
updated: 2026-10-04
authority: derived
scope: "N27 sqlserver-provider dugumu: kimlik, src klasoru, aile yapisi, ornek kayit ve yon kurali (ADR-013 + ADR-028)"
---

# Katalog Dugumu N27 - SqlServer Saglayici

**Zorunlu Baglantilar:** [catalog index](index.md) · [node-inventory](node-inventory.md) · [mimari index](../index.md) · [ADR-028](../../.decisions/accepted/ADR-028-layered-baseline-9-projects.md) · [ADR-013](../../.decisions/accepted/ADR-013-layered-architecture-52-nodes.md)

> **Tek cumlelik sorumluluk:** *N27 sqlserver-provider hangi projede ve klasorde yasar, ne yapar, kac kayit hedefler* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

**N27 (sqlserver-provider), l4 Infrastructure katmaninin alt katmanidir: `VersaCoder.Infrastructure` projesinde `src/VersaCoder.Infrastructure/Data/SqlServer/` klasorunde yasar.** Sunucu taraf SQL Server saglayicisini baglanti, havuz, tekrar ve izleme ile yonetir. 10 aile x 12 rol = 120 kayit hedefi (ADR-013 asgari 112); durum `planli`.

Kayitlar ADR-013 2.1 biciminde 8 sutun tasir; bu dosya aile kalibini ve ornek satirlari gosterir, tam 120 satirlik liste bu dosyanin boyut sinirina sigmaz (DOĞRULAMA GEREKLI).

```text
+--------------------------------------------------+
| l4 Infrastructure -> VersaCoder.Infrastructure
| src/VersaCoder.Infrastructure/Data/SqlServer/
| 10 aile x 12 rol = 120 kayit (esik 112)
+--------------------------------------------------+
```

| Alan | Deger |
|---|---|
| Dugum | N27 |
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
| 1 | SqlServer baglanti, havuz, tekrar ve izleme katmanini yonetmek | yerel SQLite dosya yonetimine girmek (N26 isidir) |
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
| 1 | N27 | sqlserver-provider | `src/VersaCoder.Infrastructure/Data/SqlServer/` |

**Kanit yolu (ADR-028 2.3/5):** tarama yolu gercek proje adini icerir; genis kalip (`src/**/Domain/**`) sahte PASS uretir.

```powershell
Test-Path 'C:\versacoder\src\VersaCoder.Infrastructure\Data\SqlServer'
```

Dizin su an yok cikti `False` olur; PASS yazilmaz -> **DOĞRULAMA GEREKLI**.

---

## §3 Aile Yapisi

Rol kalibi (12): `I<F>` · `<F>` · `<F>Dto` · `<F>Mapper` · `<F>Validator` · `<F>Factory` · `<F>Options` · `<F>Provider` · `<F>Registry` · `<F>Converter` · `<F>Tests` · `<F>Builder`

| # | Aile | # | Aile |
|---|---|---|---|
| 1 | SqlServerConnection | 6 | SqlServerProbe |
| 2 | SqlServerDialect | 7 | SqlServerInitializer |
| 3 | SqlServerRetry | 8 | SqlServerCatalog |
| 4 | SqlServerPool | 9 | SqlServerLock |
| 5 | SqlServerTimeout | 10 | SqlServerTrace |

Her aile ayni rol kalibini 12 kez tekrarlar; kayit sayisi = aile x rol.

---

## §4 Kayit Ornegi (8 sutun)

| ID | Bilesen | Tip | Sorumluluk | Proje | TFM | Test | Durum |
|---|---|---|---|---|---|---|---|
| N27-001 | `ISqlServerConnection` | interface | SqlServerConnection sozlesmesi (port) | VersaCoder.Infrastructure | net48;net10.0-windows | SqlServerConnectionTests | planli |
| N27-002 | `SqlServerConnection` | class | SqlServerConnection cekirdek uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | SqlServerConnectionTests | planli |
| N27-003 | `SqlServerConnectionDto` | record | SqlServerConnection veri transferi modeli | VersaCoder.Infrastructure | net48;net10.0-windows | SqlServerConnectionTests | planli |
| N27-004 | `SqlServerConnectionMapper` | class | SqlServerConnection <-> DTO esleme | VersaCoder.Infrastructure | net48;net10.0-windows | SqlServerConnectionTests | planli |
| N27-005 | `SqlServerConnectionValidator` | class | SqlServerConnection dogrulama kurallari | VersaCoder.Infrastructure | net48;net10.0-windows | SqlServerConnectionTests | planli |
| N27-006 | `SqlServerConnectionFactory` | class | SqlServerConnection uretimi ve varsayilanlari | VersaCoder.Infrastructure | net48;net10.0-windows | SqlServerConnectionTests | planli |

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
| 3 | Test sinifi | SqlServerConnectionTests (xUnit) | planli |
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
(Select-String -Path 'C:\versacoder\.ai\architecture\catalog\N27-sqlserver-provider.md' -Pattern '^\|\s*N\d{2}-\d{3}\s*\|').Count
# 2) boyut bandi - beklenen: 2600-8500
(Get-Item 'C:\versacoder\.ai\architecture\catalog\N27-sqlserver-provider.md').Length
# 3) klasor kaniti - gercek proje adi (ADR-028 2.3/5)
(Test-Path 'C:\versacoder\src\VersaCoder.Infrastructure\Data\SqlServer')
```

Katalog sayimi `powershell -NoProfile -ExecutionPolicy Bypass -File C:\versacoder\.ai\scripts\count-components.ps1` ile alinir; bu batch 26 node dosyasiyla 52 dugum klasorunu tamamlar - kayit esigi (5000) ornek satirlarla henuz karsilanmaz (DOĞRULAMA GEREKLI).

## §10 Kayit Tablosu (ADR-013 2.2 - kaynak vault port)

| N27-007 | `SqlServerConnectionOptions` | record | SqlServerConnection yapilandirmasi | VersaCoder.Infrastructure | net48;net10.0-windows | SqlServerConnectionTests | planli |
| N27-008 | `SqlServerConnectionProvider` | class | SqlServerConnection saglayici uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | SqlServerConnectionTests | planli |
| N27-009 | `SqlServerConnectionRegistry` | class | SqlServerConnection kayit/katalog yonetimi | VersaCoder.Infrastructure | net48;net10.0-windows | SqlServerConnectionTests | planli |
| N27-010 | `SqlServerConnectionConverter` | class | SqlServerConnection bicim donusumu | VersaCoder.Infrastructure | net48;net10.0-windows | SqlServerConnectionTests | planli |
| N27-011 | `SqlServerConnectionTests` | class | SqlServerConnection testleri (xUnit) | VersaCoder.Infrastructure | net48;net10.0-windows | - | planli |
| N27-012 | `SqlServerConnectionBuilder` | class | SqlServerConnection uretici (builder) | VersaCoder.Infrastructure | net48;net10.0-windows | SqlServerConnectionTests | planli |
| N27-013 | `ISqlServerDialect` | interface | SqlServerDialect sozlesmesi (port) | VersaCoder.Infrastructure | net48;net10.0-windows | SqlServerDialectTests | planli |
| N27-014 | `SqlServerDialect` | class | SqlServerDialect cekirdek uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | SqlServerDialectTests | planli |
| N27-015 | `SqlServerDialectDto` | record | SqlServerDialect veri transferi modeli | VersaCoder.Infrastructure | net48;net10.0-windows | SqlServerDialectTests | planli |
| N27-016 | `SqlServerDialectMapper` | class | SqlServerDialect <-> DTO esleme | VersaCoder.Infrastructure | net48;net10.0-windows | SqlServerDialectTests | planli |
| N27-017 | `SqlServerDialectValidator` | class | SqlServerDialect dogrulama kurallari | VersaCoder.Infrastructure | net48;net10.0-windows | SqlServerDialectTests | planli |
| N27-018 | `SqlServerDialectFactory` | class | SqlServerDialect uretimi ve varsayilanlari | VersaCoder.Infrastructure | net48;net10.0-windows | SqlServerDialectTests | planli |
| N27-019 | `SqlServerDialectOptions` | record | SqlServerDialect yapilandirmasi | VersaCoder.Infrastructure | net48;net10.0-windows | SqlServerDialectTests | planli |
| N27-020 | `SqlServerDialectProvider` | class | SqlServerDialect saglayici uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | SqlServerDialectTests | planli |
| N27-021 | `SqlServerDialectRegistry` | class | SqlServerDialect kayit/katalog yonetimi | VersaCoder.Infrastructure | net48;net10.0-windows | SqlServerDialectTests | planli |
| N27-022 | `SqlServerDialectConverter` | class | SqlServerDialect bicim donusumu | VersaCoder.Infrastructure | net48;net10.0-windows | SqlServerDialectTests | planli |
| N27-023 | `SqlServerDialectTests` | class | SqlServerDialect testleri (xUnit) | VersaCoder.Infrastructure | net48;net10.0-windows | - | planli |
| N27-024 | `SqlServerDialectBuilder` | class | SqlServerDialect uretici (builder) | VersaCoder.Infrastructure | net48;net10.0-windows | SqlServerDialectTests | planli |
| N27-025 | `ISqlServerRetry` | interface | SqlServerRetry sozlesmesi (port) | VersaCoder.Infrastructure | net48;net10.0-windows | SqlServerRetryTests | planli |
| N27-026 | `SqlServerRetry` | class | SqlServerRetry cekirdek uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | SqlServerRetryTests | planli |
| N27-027 | `SqlServerRetryDto` | record | SqlServerRetry veri transferi modeli | VersaCoder.Infrastructure | net48;net10.0-windows | SqlServerRetryTests | planli |
| N27-028 | `SqlServerRetryMapper` | class | SqlServerRetry <-> DTO esleme | VersaCoder.Infrastructure | net48;net10.0-windows | SqlServerRetryTests | planli |
| N27-029 | `SqlServerRetryValidator` | class | SqlServerRetry dogrulama kurallari | VersaCoder.Infrastructure | net48;net10.0-windows | SqlServerRetryTests | planli |
| N27-030 | `SqlServerRetryFactory` | class | SqlServerRetry uretimi ve varsayilanlari | VersaCoder.Infrastructure | net48;net10.0-windows | SqlServerRetryTests | planli |
| N27-031 | `SqlServerRetryOptions` | record | SqlServerRetry yapilandirmasi | VersaCoder.Infrastructure | net48;net10.0-windows | SqlServerRetryTests | planli |
| N27-032 | `SqlServerRetryProvider` | class | SqlServerRetry saglayici uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | SqlServerRetryTests | planli |
| N27-033 | `SqlServerRetryRegistry` | class | SqlServerRetry kayit/katalog yonetimi | VersaCoder.Infrastructure | net48;net10.0-windows | SqlServerRetryTests | planli |
| N27-034 | `SqlServerRetryConverter` | class | SqlServerRetry bicim donusumu | VersaCoder.Infrastructure | net48;net10.0-windows | SqlServerRetryTests | planli |
| N27-035 | `SqlServerRetryTests` | class | SqlServerRetry testleri (xUnit) | VersaCoder.Infrastructure | net48;net10.0-windows | - | planli |
| N27-036 | `SqlServerRetryBuilder` | class | SqlServerRetry uretici (builder) | VersaCoder.Infrastructure | net48;net10.0-windows | SqlServerRetryTests | planli |
| N27-037 | `ISqlServerPool` | interface | SqlServerPool sozlesmesi (port) | VersaCoder.Infrastructure | net48;net10.0-windows | SqlServerPoolTests | planli |
| N27-038 | `SqlServerPool` | class | SqlServerPool cekirdek uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | SqlServerPoolTests | planli |
| N27-039 | `SqlServerPoolDto` | record | SqlServerPool veri transferi modeli | VersaCoder.Infrastructure | net48;net10.0-windows | SqlServerPoolTests | planli |
| N27-040 | `SqlServerPoolMapper` | class | SqlServerPool <-> DTO esleme | VersaCoder.Infrastructure | net48;net10.0-windows | SqlServerPoolTests | planli |
| N27-041 | `SqlServerPoolValidator` | class | SqlServerPool dogrulama kurallari | VersaCoder.Infrastructure | net48;net10.0-windows | SqlServerPoolTests | planli |
| N27-042 | `SqlServerPoolFactory` | class | SqlServerPool uretimi ve varsayilanlari | VersaCoder.Infrastructure | net48;net10.0-windows | SqlServerPoolTests | planli |
| N27-043 | `SqlServerPoolOptions` | record | SqlServerPool yapilandirmasi | VersaCoder.Infrastructure | net48;net10.0-windows | SqlServerPoolTests | planli |
| N27-044 | `SqlServerPoolProvider` | class | SqlServerPool saglayici uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | SqlServerPoolTests | planli |
| N27-045 | `SqlServerPoolRegistry` | class | SqlServerPool kayit/katalog yonetimi | VersaCoder.Infrastructure | net48;net10.0-windows | SqlServerPoolTests | planli |
| N27-046 | `SqlServerPoolConverter` | class | SqlServerPool bicim donusumu | VersaCoder.Infrastructure | net48;net10.0-windows | SqlServerPoolTests | planli |
| N27-047 | `SqlServerPoolTests` | class | SqlServerPool testleri (xUnit) | VersaCoder.Infrastructure | net48;net10.0-windows | - | planli |
| N27-048 | `SqlServerPoolBuilder` | class | SqlServerPool uretici (builder) | VersaCoder.Infrastructure | net48;net10.0-windows | SqlServerPoolTests | planli |
| N27-049 | `ISqlServerTimeout` | interface | SqlServerTimeout sozlesmesi (port) | VersaCoder.Infrastructure | net48;net10.0-windows | SqlServerTimeoutTests | planli |
| N27-050 | `SqlServerTimeout` | class | SqlServerTimeout cekirdek uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | SqlServerTimeoutTests | planli |
| N27-051 | `SqlServerTimeoutDto` | record | SqlServerTimeout veri transferi modeli | VersaCoder.Infrastructure | net48;net10.0-windows | SqlServerTimeoutTests | planli |
| N27-052 | `SqlServerTimeoutMapper` | class | SqlServerTimeout <-> DTO esleme | VersaCoder.Infrastructure | net48;net10.0-windows | SqlServerTimeoutTests | planli |
| N27-053 | `SqlServerTimeoutValidator` | class | SqlServerTimeout dogrulama kurallari | VersaCoder.Infrastructure | net48;net10.0-windows | SqlServerTimeoutTests | planli |
| N27-054 | `SqlServerTimeoutFactory` | class | SqlServerTimeout uretimi ve varsayilanlari | VersaCoder.Infrastructure | net48;net10.0-windows | SqlServerTimeoutTests | planli |
| N27-055 | `SqlServerTimeoutOptions` | record | SqlServerTimeout yapilandirmasi | VersaCoder.Infrastructure | net48;net10.0-windows | SqlServerTimeoutTests | planli |
| N27-056 | `SqlServerTimeoutProvider` | class | SqlServerTimeout saglayici uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | SqlServerTimeoutTests | planli |
| N27-057 | `SqlServerTimeoutRegistry` | class | SqlServerTimeout kayit/katalog yonetimi | VersaCoder.Infrastructure | net48;net10.0-windows | SqlServerTimeoutTests | planli |
| N27-058 | `SqlServerTimeoutConverter` | class | SqlServerTimeout bicim donusumu | VersaCoder.Infrastructure | net48;net10.0-windows | SqlServerTimeoutTests | planli |
| N27-059 | `SqlServerTimeoutTests` | class | SqlServerTimeout testleri (xUnit) | VersaCoder.Infrastructure | net48;net10.0-windows | - | planli |
| N27-060 | `SqlServerTimeoutBuilder` | class | SqlServerTimeout uretici (builder) | VersaCoder.Infrastructure | net48;net10.0-windows | SqlServerTimeoutTests | planli |
| N27-061 | `ISqlServerProbe` | interface | SqlServerProbe sozlesmesi (port) | VersaCoder.Infrastructure | net48;net10.0-windows | SqlServerProbeTests | planli |
| N27-062 | `SqlServerProbe` | class | SqlServerProbe cekirdek uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | SqlServerProbeTests | planli |
| N27-063 | `SqlServerProbeDto` | record | SqlServerProbe veri transferi modeli | VersaCoder.Infrastructure | net48;net10.0-windows | SqlServerProbeTests | planli |
| N27-064 | `SqlServerProbeMapper` | class | SqlServerProbe <-> DTO esleme | VersaCoder.Infrastructure | net48;net10.0-windows | SqlServerProbeTests | planli |
| N27-065 | `SqlServerProbeValidator` | class | SqlServerProbe dogrulama kurallari | VersaCoder.Infrastructure | net48;net10.0-windows | SqlServerProbeTests | planli |
| N27-066 | `SqlServerProbeFactory` | class | SqlServerProbe uretimi ve varsayilanlari | VersaCoder.Infrastructure | net48;net10.0-windows | SqlServerProbeTests | planli |
| N27-067 | `SqlServerProbeOptions` | record | SqlServerProbe yapilandirmasi | VersaCoder.Infrastructure | net48;net10.0-windows | SqlServerProbeTests | planli |
| N27-068 | `SqlServerProbeProvider` | class | SqlServerProbe saglayici uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | SqlServerProbeTests | planli |
| N27-069 | `SqlServerProbeRegistry` | class | SqlServerProbe kayit/katalog yonetimi | VersaCoder.Infrastructure | net48;net10.0-windows | SqlServerProbeTests | planli |
| N27-070 | `SqlServerProbeConverter` | class | SqlServerProbe bicim donusumu | VersaCoder.Infrastructure | net48;net10.0-windows | SqlServerProbeTests | planli |
| N27-071 | `SqlServerProbeTests` | class | SqlServerProbe testleri (xUnit) | VersaCoder.Infrastructure | net48;net10.0-windows | - | planli |
| N27-072 | `SqlServerProbeBuilder` | class | SqlServerProbe uretici (builder) | VersaCoder.Infrastructure | net48;net10.0-windows | SqlServerProbeTests | planli |
| N27-073 | `ISqlServerInitializer` | interface | SqlServerInitializer sozlesmesi (port) | VersaCoder.Infrastructure | net48;net10.0-windows | SqlServerInitializerTests | planli |
| N27-074 | `SqlServerInitializer` | class | SqlServerInitializer cekirdek uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | SqlServerInitializerTests | planli |
| N27-075 | `SqlServerInitializerDto` | record | SqlServerInitializer veri transferi modeli | VersaCoder.Infrastructure | net48;net10.0-windows | SqlServerInitializerTests | planli |
| N27-076 | `SqlServerInitializerMapper` | class | SqlServerInitializer <-> DTO esleme | VersaCoder.Infrastructure | net48;net10.0-windows | SqlServerInitializerTests | planli |
| N27-077 | `SqlServerInitializerValidator` | class | SqlServerInitializer dogrulama kurallari | VersaCoder.Infrastructure | net48;net10.0-windows | SqlServerInitializerTests | planli |
| N27-078 | `SqlServerInitializerFactory` | class | SqlServerInitializer uretimi ve varsayilanlari | VersaCoder.Infrastructure | net48;net10.0-windows | SqlServerInitializerTests | planli |
| N27-079 | `SqlServerInitializerOptions` | record | SqlServerInitializer yapilandirmasi | VersaCoder.Infrastructure | net48;net10.0-windows | SqlServerInitializerTests | planli |
| N27-080 | `SqlServerInitializerProvider` | class | SqlServerInitializer saglayici uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | SqlServerInitializerTests | planli |
| N27-081 | `SqlServerInitializerRegistry` | class | SqlServerInitializer kayit/katalog yonetimi | VersaCoder.Infrastructure | net48;net10.0-windows | SqlServerInitializerTests | planli |
| N27-082 | `SqlServerInitializerConverter` | class | SqlServerInitializer bicim donusumu | VersaCoder.Infrastructure | net48;net10.0-windows | SqlServerInitializerTests | planli |
| N27-083 | `SqlServerInitializerTests` | class | SqlServerInitializer testleri (xUnit) | VersaCoder.Infrastructure | net48;net10.0-windows | - | planli |
| N27-084 | `SqlServerInitializerBuilder` | class | SqlServerInitializer uretici (builder) | VersaCoder.Infrastructure | net48;net10.0-windows | SqlServerInitializerTests | planli |
| N27-085 | `ISqlServerCatalog` | interface | SqlServerCatalog sozlesmesi (port) | VersaCoder.Infrastructure | net48;net10.0-windows | SqlServerCatalogTests | planli |
| N27-086 | `SqlServerCatalog` | class | SqlServerCatalog cekirdek uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | SqlServerCatalogTests | planli |
| N27-087 | `SqlServerCatalogDto` | record | SqlServerCatalog veri transferi modeli | VersaCoder.Infrastructure | net48;net10.0-windows | SqlServerCatalogTests | planli |
| N27-088 | `SqlServerCatalogMapper` | class | SqlServerCatalog <-> DTO esleme | VersaCoder.Infrastructure | net48;net10.0-windows | SqlServerCatalogTests | planli |
| N27-089 | `SqlServerCatalogValidator` | class | SqlServerCatalog dogrulama kurallari | VersaCoder.Infrastructure | net48;net10.0-windows | SqlServerCatalogTests | planli |
| N27-090 | `SqlServerCatalogFactory` | class | SqlServerCatalog uretimi ve varsayilanlari | VersaCoder.Infrastructure | net48;net10.0-windows | SqlServerCatalogTests | planli |
| N27-091 | `SqlServerCatalogOptions` | record | SqlServerCatalog yapilandirmasi | VersaCoder.Infrastructure | net48;net10.0-windows | SqlServerCatalogTests | planli |
| N27-092 | `SqlServerCatalogProvider` | class | SqlServerCatalog saglayici uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | SqlServerCatalogTests | planli |
| N27-093 | `SqlServerCatalogRegistry` | class | SqlServerCatalog kayit/katalog yonetimi | VersaCoder.Infrastructure | net48;net10.0-windows | SqlServerCatalogTests | planli |
| N27-094 | `SqlServerCatalogConverter` | class | SqlServerCatalog bicim donusumu | VersaCoder.Infrastructure | net48;net10.0-windows | SqlServerCatalogTests | planli |
| N27-095 | `SqlServerCatalogTests` | class | SqlServerCatalog testleri (xUnit) | VersaCoder.Infrastructure | net48;net10.0-windows | - | planli |
| N27-096 | `SqlServerCatalogBuilder` | class | SqlServerCatalog uretici (builder) | VersaCoder.Infrastructure | net48;net10.0-windows | SqlServerCatalogTests | planli |
| N27-097 | `ISqlServerLock` | interface | SqlServerLock sozlesmesi (port) | VersaCoder.Infrastructure | net48;net10.0-windows | SqlServerLockTests | planli |
| N27-098 | `SqlServerLock` | class | SqlServerLock cekirdek uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | SqlServerLockTests | planli |
| N27-099 | `SqlServerLockDto` | record | SqlServerLock veri transferi modeli | VersaCoder.Infrastructure | net48;net10.0-windows | SqlServerLockTests | planli |
| N27-100 | `SqlServerLockMapper` | class | SqlServerLock <-> DTO esleme | VersaCoder.Infrastructure | net48;net10.0-windows | SqlServerLockTests | planli |
| N27-101 | `SqlServerLockValidator` | class | SqlServerLock dogrulama kurallari | VersaCoder.Infrastructure | net48;net10.0-windows | SqlServerLockTests | planli |
| N27-102 | `SqlServerLockFactory` | class | SqlServerLock uretimi ve varsayilanlari | VersaCoder.Infrastructure | net48;net10.0-windows | SqlServerLockTests | planli |
| N27-103 | `SqlServerLockOptions` | record | SqlServerLock yapilandirmasi | VersaCoder.Infrastructure | net48;net10.0-windows | SqlServerLockTests | planli |
| N27-104 | `SqlServerLockProvider` | class | SqlServerLock saglayici uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | SqlServerLockTests | planli |
| N27-105 | `SqlServerLockRegistry` | class | SqlServerLock kayit/katalog yonetimi | VersaCoder.Infrastructure | net48;net10.0-windows | SqlServerLockTests | planli |
| N27-106 | `SqlServerLockConverter` | class | SqlServerLock bicim donusumu | VersaCoder.Infrastructure | net48;net10.0-windows | SqlServerLockTests | planli |
| N27-107 | `SqlServerLockTests` | class | SqlServerLock testleri (xUnit) | VersaCoder.Infrastructure | net48;net10.0-windows | - | planli |
| N27-108 | `SqlServerLockBuilder` | class | SqlServerLock uretici (builder) | VersaCoder.Infrastructure | net48;net10.0-windows | SqlServerLockTests | planli |
| N27-109 | `ISqlServerTrace` | interface | SqlServerTrace sozlesmesi (port) | VersaCoder.Infrastructure | net48;net10.0-windows | SqlServerTraceTests | planli |
| N27-110 | `SqlServerTrace` | class | SqlServerTrace cekirdek uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | SqlServerTraceTests | planli |
| N27-111 | `SqlServerTraceDto` | record | SqlServerTrace veri transferi modeli | VersaCoder.Infrastructure | net48;net10.0-windows | SqlServerTraceTests | planli |
| N27-112 | `SqlServerTraceMapper` | class | SqlServerTrace <-> DTO esleme | VersaCoder.Infrastructure | net48;net10.0-windows | SqlServerTraceTests | planli |
| N27-113 | `SqlServerTraceValidator` | class | SqlServerTrace dogrulama kurallari | VersaCoder.Infrastructure | net48;net10.0-windows | SqlServerTraceTests | planli |
| N27-114 | `SqlServerTraceFactory` | class | SqlServerTrace uretimi ve varsayilanlari | VersaCoder.Infrastructure | net48;net10.0-windows | SqlServerTraceTests | planli |
| N27-115 | `SqlServerTraceOptions` | record | SqlServerTrace yapilandirmasi | VersaCoder.Infrastructure | net48;net10.0-windows | SqlServerTraceTests | planli |
| N27-116 | `SqlServerTraceProvider` | class | SqlServerTrace saglayici uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | SqlServerTraceTests | planli |
| N27-117 | `SqlServerTraceRegistry` | class | SqlServerTrace kayit/katalog yonetimi | VersaCoder.Infrastructure | net48;net10.0-windows | SqlServerTraceTests | planli |
| N27-118 | `SqlServerTraceConverter` | class | SqlServerTrace bicim donusumu | VersaCoder.Infrastructure | net48;net10.0-windows | SqlServerTraceTests | planli |
| N27-119 | `SqlServerTraceTests` | class | SqlServerTrace testleri (xUnit) | VersaCoder.Infrastructure | net48;net10.0-windows | - | planli |
| N27-120 | `SqlServerTraceBuilder` | class | SqlServerTrace uretici (builder) | VersaCoder.Infrastructure | net48;net10.0-windows | SqlServerTraceTests | planli |
