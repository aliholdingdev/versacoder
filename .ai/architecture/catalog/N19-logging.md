---
title: "Katalog Dugumu N19 - Loglama (l3)"
type: catalog
category: architecture
status: needs-validation
version: 1.0.0
updated: 2026-10-04
authority: derived
scope: "N19 logging dugumu: kimlik, src klasoru, aile yapisi, ornek kayit ve yon kurali (ADR-013 + ADR-028)"
---

# Katalog Dugumu N19 - Loglama

**Zorunlu Baglantilar:** [catalog index](index.md) · [node-inventory](node-inventory.md) · [mimari index](../index.md) · [ADR-028](../../.decisions/accepted/ADR-028-layered-baseline-9-projects.md) · [ADR-013](../../.decisions/accepted/ADR-013-layered-architecture-52-nodes.md)

> **Tek cumlelik sorumluluk:** *N19 logging hangi projede ve klasorde yasar, ne yapar, kac kayit hedefler* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

**N19 (logging), l3 Crosscutting katmaninin alt katmanidir: `VersaCoder.Crosscutting` projesinde `src/VersaCoder.Crosscutting/Logging/` klasorunde yasar.** Log seviyesi, kapsam, denetim ve iz kayitlarini tek noktada saglar. 10 aile x 12 rol = 120 kayit hedefi (ADR-013 asgari 112); durum `planli`.

Kayitlar ADR-013 2.1 biciminde 8 sutun tasir; bu dosya aile kalibini ve ornek satirlari gosterir, tam 120 satirlik liste bu dosyanin boyut sinirina sigmaz (DOĞRULAMA GEREKLI).

```text
+--------------------------------------------------+
| l3 Crosscutting -> VersaCoder.Crosscutting
| src/VersaCoder.Crosscutting/Logging/
| 10 aile x 12 rol = 120 kayit (esik 112)
+--------------------------------------------------+
```

| Alan | Deger |
|---|---|
| Dugum | N19 |
| Dugum turu | alt katman (klasor/namespace duzeyi) |
| Makro katman | l3 Crosscutting |
| Proje (.csproj) | VersaCoder.Crosscutting |
| TFM | net48;net10.0-windows |
| Kayit hedefi | 120 (ADR-013 asgari 112) |
| Durum | planli |
| Sorumlu rol | architect |

---

## §1 Sorumluluk (YAPAR / YAPMAZ)

| # | YAPAR | YAPMAZ |
|---|---|---|
| 1 | Log seviyesi, kapsam, denetim ve iz kayitlarini tek noktada saglar | l0 disinda bir katmana dogrudan bakmak |
| 2 | kayitlari 8 sutunlu ADR-013 biciminde tutmak (planli -> yapildi -> testli) | is kurali veya invariant tasimak, is l1 katmaninda |
| 3 | ortak kesit servisini vermek: log, saat, onbek, kripto, yapilandirma | kullanim senaryosu orkestre etmek, is l2 katmaninda |
| 4 | test edilebilir saricilar sunmak (IClock, ILogSink) | SQL veya veri erisimi yazmak, is l4 katmaninda |
| 5 | yalniz l0 katmanina bakmak | UI tipine bakmak (Desktop / Cli) |
| 6 | katmanlari degil alt altyapiyi sarmak | secret veya anahtar barindirmak, keys.md tek kaynaktir |

---

## §2 Klasor ve N Eslemesi

**Kalip (ADR-028 2.3/4):** `N33 -> src\VersaCoder.Protocol\ToolSchema\` satiri SSOT ornegidir; bu dosyanin klasoru ayni kalipla yazilir.

| # | N | Slug | src/ klasoru |
|---|---|---|---|
| 1 | N19 | logging | `src/VersaCoder.Crosscutting/Logging/` |

**Kanit yolu (ADR-028 2.3/5):** tarama yolu gercek proje adini icerir; genis kalip (`src/**/Domain/**`) sahte PASS uretir.

```powershell
Test-Path 'C:\versacoder\src\VersaCoder.Crosscutting\Logging'
```

Dizin su an yok cikti `False` olur; PASS yazilmaz -> **DOĞRULAMA GEREKLI**.

---

## §3 Aile Yapisi

Rol kalibi (12): `I<F>` · `<F>` · `<F>Dto` · `<F>Mapper` · `<F>Validator` · `<F>Factory` · `<F>Options` · `<F>Provider` · `<F>Registry` · `<F>Converter` · `<F>Tests` · `<F>Builder`

| # | Aile | # | Aile |
|---|---|---|---|
| 1 | Logger | 6 | CorrelationId |
| 2 | LogScope | 7 | AuditLog |
| 3 | LogFormatter | 8 | MetricCounter |
| 4 | LogSink | 9 | TraceSpan |
| 5 | LogLevelPolicy | 10 | LogRedactor |

Her aile ayni rol kalibini 12 kez tekrarlar; kayit sayisi = aile x rol.

---

## §4 Kayit Ornegi (8 sutun)

| ID | Bilesen | Tip | Sorumluluk | Proje | TFM | Test | Durum |
|---|---|---|---|---|---|---|---|
| N19-001 | `ILogger` | interface | Logger sozlesmesi (port) | VersaCoder.Crosscutting | net48;net10.0-windows | LoggerTests | planli |
| N19-002 | `Logger` | class | Logger cekirdek uygulamasi | VersaCoder.Crosscutting | net48;net10.0-windows | LoggerTests | planli |
| N19-003 | `LoggerDto` | record | Logger veri transferi modeli | VersaCoder.Crosscutting | net48;net10.0-windows | LoggerTests | planli |
| N19-004 | `LoggerMapper` | class | Logger <-> DTO esleme | VersaCoder.Crosscutting | net48;net10.0-windows | LoggerTests | planli |
| N19-005 | `LoggerValidator` | class | Logger dogrulama kurallari | VersaCoder.Crosscutting | net48;net10.0-windows | LoggerTests | planli |
| N19-006 | `LoggerFactory` | class | Logger uretimi ve varsayilanlari | VersaCoder.Crosscutting | net48;net10.0-windows | LoggerTests | planli |

Kalan aileler ayni kalibi 12 rolle tekrarlar (makro dugumlerde 4 rol); toplam hedef **120** kayit.

---

## §5 Yon Kurali

| # | Yon | Deger | Kaynak |
|---|---|---|---|
| 1 | l3 -> l0 | OK | ADR-028 2.1 |
| 2 | l3 -> l1, l2, l4..l7 | X | ADR-028 2.1 |
| 3 | l5 -> l4 | X | ADR-028 2.3/2 |

Matrisin tamami [layer-matrix](layer-matrix.md) dosyasindadir; NetArchTest `LayerRulesTests` bu tabloya gore yazilir.

---

## §6 Test ve Durum

| # | Kontrol | Beklenen | Durum |
|---|---|---|---|
| 1 | Kayit satiri | 120 (asgari 112) | bu dosyada 6 ornek satir; tam liste DOĞRULAMA GEREKLI |
| 2 | Durum degeri | planli -> yapildi -> testli | planli |
| 3 | Test sinifi | LoggerTests (xUnit) | planli |
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
(Select-String -Path 'C:\versacoder\.ai\architecture\catalog\N19-logging.md' -Pattern '^\|\s*N\d{2}-\d{3}\s*\|').Count
# 2) boyut bandi - beklenen: 2600-8500
(Get-Item 'C:\versacoder\.ai\architecture\catalog\N19-logging.md').Length
# 3) klasor kaniti - gercek proje adi (ADR-028 2.3/5)
(Test-Path 'C:\versacoder\src\VersaCoder.Crosscutting\Logging')
```

Katalog sayimi `powershell -NoProfile -ExecutionPolicy Bypass -File C:\versacoder\.ai\scripts\count-components.ps1` ile alinir; bu batch sonunda toplam esik (5000) henuz karsilanmaz - kalan 26 node dosyasi sonraki batchtir.

## §10 Kayit Tablosu (ADR-013 2.2 - kaynak vault port)

| N19-007 | `LoggerOptions` | record | Logger yapilandirmasi | VersaCoder.Crosscutting | net48;net10.0-windows | LoggerTests | planli |
| N19-008 | `LoggerProvider` | class | Logger saglayici uygulamasi | VersaCoder.Crosscutting | net48;net10.0-windows | LoggerTests | planli |
| N19-009 | `LoggerRegistry` | class | Logger kayit/katalog yonetimi | VersaCoder.Crosscutting | net48;net10.0-windows | LoggerTests | planli |
| N19-010 | `LoggerConverter` | class | Logger bicim donusumu | VersaCoder.Crosscutting | net48;net10.0-windows | LoggerTests | planli |
| N19-011 | `LoggerTests` | class | Logger testleri (xUnit) | VersaCoder.Crosscutting | net48;net10.0-windows | - | planli |
| N19-012 | `LoggerBuilder` | class | Logger uretici (builder) | VersaCoder.Crosscutting | net48;net10.0-windows | LoggerTests | planli |
| N19-013 | `ILogScope` | interface | LogScope sozlesmesi (port) | VersaCoder.Crosscutting | net48;net10.0-windows | LogScopeTests | planli |
| N19-014 | `LogScope` | class | LogScope cekirdek uygulamasi | VersaCoder.Crosscutting | net48;net10.0-windows | LogScopeTests | planli |
| N19-015 | `LogScopeDto` | record | LogScope veri transferi modeli | VersaCoder.Crosscutting | net48;net10.0-windows | LogScopeTests | planli |
| N19-016 | `LogScopeMapper` | class | LogScope <-> DTO esleme | VersaCoder.Crosscutting | net48;net10.0-windows | LogScopeTests | planli |
| N19-017 | `LogScopeValidator` | class | LogScope dogrulama kurallari | VersaCoder.Crosscutting | net48;net10.0-windows | LogScopeTests | planli |
| N19-018 | `LogScopeFactory` | class | LogScope uretimi ve varsayilanlari | VersaCoder.Crosscutting | net48;net10.0-windows | LogScopeTests | planli |
| N19-019 | `LogScopeOptions` | record | LogScope yapilandirmasi | VersaCoder.Crosscutting | net48;net10.0-windows | LogScopeTests | planli |
| N19-020 | `LogScopeProvider` | class | LogScope saglayici uygulamasi | VersaCoder.Crosscutting | net48;net10.0-windows | LogScopeTests | planli |
| N19-021 | `LogScopeRegistry` | class | LogScope kayit/katalog yonetimi | VersaCoder.Crosscutting | net48;net10.0-windows | LogScopeTests | planli |
| N19-022 | `LogScopeConverter` | class | LogScope bicim donusumu | VersaCoder.Crosscutting | net48;net10.0-windows | LogScopeTests | planli |
| N19-023 | `LogScopeTests` | class | LogScope testleri (xUnit) | VersaCoder.Crosscutting | net48;net10.0-windows | - | planli |
| N19-024 | `LogScopeBuilder` | class | LogScope uretici (builder) | VersaCoder.Crosscutting | net48;net10.0-windows | LogScopeTests | planli |
| N19-025 | `ILogFormatter` | interface | LogFormatter sozlesmesi (port) | VersaCoder.Crosscutting | net48;net10.0-windows | LogFormatterTests | planli |
| N19-026 | `LogFormatter` | class | LogFormatter cekirdek uygulamasi | VersaCoder.Crosscutting | net48;net10.0-windows | LogFormatterTests | planli |
| N19-027 | `LogFormatterDto` | record | LogFormatter veri transferi modeli | VersaCoder.Crosscutting | net48;net10.0-windows | LogFormatterTests | planli |
| N19-028 | `LogFormatterMapper` | class | LogFormatter <-> DTO esleme | VersaCoder.Crosscutting | net48;net10.0-windows | LogFormatterTests | planli |
| N19-029 | `LogFormatterValidator` | class | LogFormatter dogrulama kurallari | VersaCoder.Crosscutting | net48;net10.0-windows | LogFormatterTests | planli |
| N19-030 | `LogFormatterFactory` | class | LogFormatter uretimi ve varsayilanlari | VersaCoder.Crosscutting | net48;net10.0-windows | LogFormatterTests | planli |
| N19-031 | `LogFormatterOptions` | record | LogFormatter yapilandirmasi | VersaCoder.Crosscutting | net48;net10.0-windows | LogFormatterTests | planli |
| N19-032 | `LogFormatterProvider` | class | LogFormatter saglayici uygulamasi | VersaCoder.Crosscutting | net48;net10.0-windows | LogFormatterTests | planli |
| N19-033 | `LogFormatterRegistry` | class | LogFormatter kayit/katalog yonetimi | VersaCoder.Crosscutting | net48;net10.0-windows | LogFormatterTests | planli |
| N19-034 | `LogFormatterConverter` | class | LogFormatter bicim donusumu | VersaCoder.Crosscutting | net48;net10.0-windows | LogFormatterTests | planli |
| N19-035 | `LogFormatterTests` | class | LogFormatter testleri (xUnit) | VersaCoder.Crosscutting | net48;net10.0-windows | - | planli |
| N19-036 | `LogFormatterBuilder` | class | LogFormatter uretici (builder) | VersaCoder.Crosscutting | net48;net10.0-windows | LogFormatterTests | planli |
| N19-037 | `ILogSink` | interface | LogSink sozlesmesi (port) | VersaCoder.Crosscutting | net48;net10.0-windows | LogSinkTests | planli |
| N19-038 | `LogSink` | class | LogSink cekirdek uygulamasi | VersaCoder.Crosscutting | net48;net10.0-windows | LogSinkTests | planli |
| N19-039 | `LogSinkDto` | record | LogSink veri transferi modeli | VersaCoder.Crosscutting | net48;net10.0-windows | LogSinkTests | planli |
| N19-040 | `LogSinkMapper` | class | LogSink <-> DTO esleme | VersaCoder.Crosscutting | net48;net10.0-windows | LogSinkTests | planli |
| N19-041 | `LogSinkValidator` | class | LogSink dogrulama kurallari | VersaCoder.Crosscutting | net48;net10.0-windows | LogSinkTests | planli |
| N19-042 | `LogSinkFactory` | class | LogSink uretimi ve varsayilanlari | VersaCoder.Crosscutting | net48;net10.0-windows | LogSinkTests | planli |
| N19-043 | `LogSinkOptions` | record | LogSink yapilandirmasi | VersaCoder.Crosscutting | net48;net10.0-windows | LogSinkTests | planli |
| N19-044 | `LogSinkProvider` | class | LogSink saglayici uygulamasi | VersaCoder.Crosscutting | net48;net10.0-windows | LogSinkTests | planli |
| N19-045 | `LogSinkRegistry` | class | LogSink kayit/katalog yonetimi | VersaCoder.Crosscutting | net48;net10.0-windows | LogSinkTests | planli |
| N19-046 | `LogSinkConverter` | class | LogSink bicim donusumu | VersaCoder.Crosscutting | net48;net10.0-windows | LogSinkTests | planli |
| N19-047 | `LogSinkTests` | class | LogSink testleri (xUnit) | VersaCoder.Crosscutting | net48;net10.0-windows | - | planli |
| N19-048 | `LogSinkBuilder` | class | LogSink uretici (builder) | VersaCoder.Crosscutting | net48;net10.0-windows | LogSinkTests | planli |
| N19-049 | `ILogLevelPolicy` | interface | LogLevelPolicy sozlesmesi (port) | VersaCoder.Crosscutting | net48;net10.0-windows | LogLevelPolicyTests | planli |
| N19-050 | `LogLevelPolicy` | class | LogLevelPolicy cekirdek uygulamasi | VersaCoder.Crosscutting | net48;net10.0-windows | LogLevelPolicyTests | planli |
| N19-051 | `LogLevelPolicyDto` | record | LogLevelPolicy veri transferi modeli | VersaCoder.Crosscutting | net48;net10.0-windows | LogLevelPolicyTests | planli |
| N19-052 | `LogLevelPolicyMapper` | class | LogLevelPolicy <-> DTO esleme | VersaCoder.Crosscutting | net48;net10.0-windows | LogLevelPolicyTests | planli |
| N19-053 | `LogLevelPolicyValidator` | class | LogLevelPolicy dogrulama kurallari | VersaCoder.Crosscutting | net48;net10.0-windows | LogLevelPolicyTests | planli |
| N19-054 | `LogLevelPolicyFactory` | class | LogLevelPolicy uretimi ve varsayilanlari | VersaCoder.Crosscutting | net48;net10.0-windows | LogLevelPolicyTests | planli |
| N19-055 | `LogLevelPolicyOptions` | record | LogLevelPolicy yapilandirmasi | VersaCoder.Crosscutting | net48;net10.0-windows | LogLevelPolicyTests | planli |
| N19-056 | `LogLevelPolicyProvider` | class | LogLevelPolicy saglayici uygulamasi | VersaCoder.Crosscutting | net48;net10.0-windows | LogLevelPolicyTests | planli |
| N19-057 | `LogLevelPolicyRegistry` | class | LogLevelPolicy kayit/katalog yonetimi | VersaCoder.Crosscutting | net48;net10.0-windows | LogLevelPolicyTests | planli |
| N19-058 | `LogLevelPolicyConverter` | class | LogLevelPolicy bicim donusumu | VersaCoder.Crosscutting | net48;net10.0-windows | LogLevelPolicyTests | planli |
| N19-059 | `LogLevelPolicyTests` | class | LogLevelPolicy testleri (xUnit) | VersaCoder.Crosscutting | net48;net10.0-windows | - | planli |
| N19-060 | `LogLevelPolicyBuilder` | class | LogLevelPolicy uretici (builder) | VersaCoder.Crosscutting | net48;net10.0-windows | LogLevelPolicyTests | planli |
| N19-061 | `ICorrelationId` | interface | CorrelationId sozlesmesi (port) | VersaCoder.Crosscutting | net48;net10.0-windows | CorrelationIdTests | planli |
| N19-062 | `CorrelationId` | class | CorrelationId cekirdek uygulamasi | VersaCoder.Crosscutting | net48;net10.0-windows | CorrelationIdTests | planli |
| N19-063 | `CorrelationIdDto` | record | CorrelationId veri transferi modeli | VersaCoder.Crosscutting | net48;net10.0-windows | CorrelationIdTests | planli |
| N19-064 | `CorrelationIdMapper` | class | CorrelationId <-> DTO esleme | VersaCoder.Crosscutting | net48;net10.0-windows | CorrelationIdTests | planli |
| N19-065 | `CorrelationIdValidator` | class | CorrelationId dogrulama kurallari | VersaCoder.Crosscutting | net48;net10.0-windows | CorrelationIdTests | planli |
| N19-066 | `CorrelationIdFactory` | class | CorrelationId uretimi ve varsayilanlari | VersaCoder.Crosscutting | net48;net10.0-windows | CorrelationIdTests | planli |
| N19-067 | `CorrelationIdOptions` | record | CorrelationId yapilandirmasi | VersaCoder.Crosscutting | net48;net10.0-windows | CorrelationIdTests | planli |
| N19-068 | `CorrelationIdProvider` | class | CorrelationId saglayici uygulamasi | VersaCoder.Crosscutting | net48;net10.0-windows | CorrelationIdTests | planli |
| N19-069 | `CorrelationIdRegistry` | class | CorrelationId kayit/katalog yonetimi | VersaCoder.Crosscutting | net48;net10.0-windows | CorrelationIdTests | planli |
| N19-070 | `CorrelationIdConverter` | class | CorrelationId bicim donusumu | VersaCoder.Crosscutting | net48;net10.0-windows | CorrelationIdTests | planli |
| N19-071 | `CorrelationIdTests` | class | CorrelationId testleri (xUnit) | VersaCoder.Crosscutting | net48;net10.0-windows | - | planli |
| N19-072 | `CorrelationIdBuilder` | class | CorrelationId uretici (builder) | VersaCoder.Crosscutting | net48;net10.0-windows | CorrelationIdTests | planli |
| N19-073 | `IAuditLog` | interface | AuditLog sozlesmesi (port) | VersaCoder.Crosscutting | net48;net10.0-windows | AuditLogTests | planli |
| N19-074 | `AuditLog` | class | AuditLog cekirdek uygulamasi | VersaCoder.Crosscutting | net48;net10.0-windows | AuditLogTests | planli |
| N19-075 | `AuditLogDto` | record | AuditLog veri transferi modeli | VersaCoder.Crosscutting | net48;net10.0-windows | AuditLogTests | planli |
| N19-076 | `AuditLogMapper` | class | AuditLog <-> DTO esleme | VersaCoder.Crosscutting | net48;net10.0-windows | AuditLogTests | planli |
| N19-077 | `AuditLogValidator` | class | AuditLog dogrulama kurallari | VersaCoder.Crosscutting | net48;net10.0-windows | AuditLogTests | planli |
| N19-078 | `AuditLogFactory` | class | AuditLog uretimi ve varsayilanlari | VersaCoder.Crosscutting | net48;net10.0-windows | AuditLogTests | planli |
| N19-079 | `AuditLogOptions` | record | AuditLog yapilandirmasi | VersaCoder.Crosscutting | net48;net10.0-windows | AuditLogTests | planli |
| N19-080 | `AuditLogProvider` | class | AuditLog saglayici uygulamasi | VersaCoder.Crosscutting | net48;net10.0-windows | AuditLogTests | planli |
| N19-081 | `AuditLogRegistry` | class | AuditLog kayit/katalog yonetimi | VersaCoder.Crosscutting | net48;net10.0-windows | AuditLogTests | planli |
| N19-082 | `AuditLogConverter` | class | AuditLog bicim donusumu | VersaCoder.Crosscutting | net48;net10.0-windows | AuditLogTests | planli |
| N19-083 | `AuditLogTests` | class | AuditLog testleri (xUnit) | VersaCoder.Crosscutting | net48;net10.0-windows | - | planli |
| N19-084 | `AuditLogBuilder` | class | AuditLog uretici (builder) | VersaCoder.Crosscutting | net48;net10.0-windows | AuditLogTests | planli |
| N19-085 | `IMetricCounter` | interface | MetricCounter sozlesmesi (port) | VersaCoder.Crosscutting | net48;net10.0-windows | MetricCounterTests | planli |
| N19-086 | `MetricCounter` | class | MetricCounter cekirdek uygulamasi | VersaCoder.Crosscutting | net48;net10.0-windows | MetricCounterTests | planli |
| N19-087 | `MetricCounterDto` | record | MetricCounter veri transferi modeli | VersaCoder.Crosscutting | net48;net10.0-windows | MetricCounterTests | planli |
| N19-088 | `MetricCounterMapper` | class | MetricCounter <-> DTO esleme | VersaCoder.Crosscutting | net48;net10.0-windows | MetricCounterTests | planli |
| N19-089 | `MetricCounterValidator` | class | MetricCounter dogrulama kurallari | VersaCoder.Crosscutting | net48;net10.0-windows | MetricCounterTests | planli |
| N19-090 | `MetricCounterFactory` | class | MetricCounter uretimi ve varsayilanlari | VersaCoder.Crosscutting | net48;net10.0-windows | MetricCounterTests | planli |
| N19-091 | `MetricCounterOptions` | record | MetricCounter yapilandirmasi | VersaCoder.Crosscutting | net48;net10.0-windows | MetricCounterTests | planli |
| N19-092 | `MetricCounterProvider` | class | MetricCounter saglayici uygulamasi | VersaCoder.Crosscutting | net48;net10.0-windows | MetricCounterTests | planli |
| N19-093 | `MetricCounterRegistry` | class | MetricCounter kayit/katalog yonetimi | VersaCoder.Crosscutting | net48;net10.0-windows | MetricCounterTests | planli |
| N19-094 | `MetricCounterConverter` | class | MetricCounter bicim donusumu | VersaCoder.Crosscutting | net48;net10.0-windows | MetricCounterTests | planli |
| N19-095 | `MetricCounterTests` | class | MetricCounter testleri (xUnit) | VersaCoder.Crosscutting | net48;net10.0-windows | - | planli |
| N19-096 | `MetricCounterBuilder` | class | MetricCounter uretici (builder) | VersaCoder.Crosscutting | net48;net10.0-windows | MetricCounterTests | planli |
| N19-097 | `ITraceSpan` | interface | TraceSpan sozlesmesi (port) | VersaCoder.Crosscutting | net48;net10.0-windows | TraceSpanTests | planli |
| N19-098 | `TraceSpan` | class | TraceSpan cekirdek uygulamasi | VersaCoder.Crosscutting | net48;net10.0-windows | TraceSpanTests | planli |
| N19-099 | `TraceSpanDto` | record | TraceSpan veri transferi modeli | VersaCoder.Crosscutting | net48;net10.0-windows | TraceSpanTests | planli |
| N19-100 | `TraceSpanMapper` | class | TraceSpan <-> DTO esleme | VersaCoder.Crosscutting | net48;net10.0-windows | TraceSpanTests | planli |
| N19-101 | `TraceSpanValidator` | class | TraceSpan dogrulama kurallari | VersaCoder.Crosscutting | net48;net10.0-windows | TraceSpanTests | planli |
| N19-102 | `TraceSpanFactory` | class | TraceSpan uretimi ve varsayilanlari | VersaCoder.Crosscutting | net48;net10.0-windows | TraceSpanTests | planli |
| N19-103 | `TraceSpanOptions` | record | TraceSpan yapilandirmasi | VersaCoder.Crosscutting | net48;net10.0-windows | TraceSpanTests | planli |
| N19-104 | `TraceSpanProvider` | class | TraceSpan saglayici uygulamasi | VersaCoder.Crosscutting | net48;net10.0-windows | TraceSpanTests | planli |
| N19-105 | `TraceSpanRegistry` | class | TraceSpan kayit/katalog yonetimi | VersaCoder.Crosscutting | net48;net10.0-windows | TraceSpanTests | planli |
| N19-106 | `TraceSpanConverter` | class | TraceSpan bicim donusumu | VersaCoder.Crosscutting | net48;net10.0-windows | TraceSpanTests | planli |
| N19-107 | `TraceSpanTests` | class | TraceSpan testleri (xUnit) | VersaCoder.Crosscutting | net48;net10.0-windows | - | planli |
| N19-108 | `TraceSpanBuilder` | class | TraceSpan uretici (builder) | VersaCoder.Crosscutting | net48;net10.0-windows | TraceSpanTests | planli |
| N19-109 | `ILogRedactor` | interface | LogRedactor sozlesmesi (port) | VersaCoder.Crosscutting | net48;net10.0-windows | LogRedactorTests | planli |
| N19-110 | `LogRedactor` | class | LogRedactor cekirdek uygulamasi | VersaCoder.Crosscutting | net48;net10.0-windows | LogRedactorTests | planli |
| N19-111 | `LogRedactorDto` | record | LogRedactor veri transferi modeli | VersaCoder.Crosscutting | net48;net10.0-windows | LogRedactorTests | planli |
| N19-112 | `LogRedactorMapper` | class | LogRedactor <-> DTO esleme | VersaCoder.Crosscutting | net48;net10.0-windows | LogRedactorTests | planli |
| N19-113 | `LogRedactorValidator` | class | LogRedactor dogrulama kurallari | VersaCoder.Crosscutting | net48;net10.0-windows | LogRedactorTests | planli |
| N19-114 | `LogRedactorFactory` | class | LogRedactor uretimi ve varsayilanlari | VersaCoder.Crosscutting | net48;net10.0-windows | LogRedactorTests | planli |
| N19-115 | `LogRedactorOptions` | record | LogRedactor yapilandirmasi | VersaCoder.Crosscutting | net48;net10.0-windows | LogRedactorTests | planli |
| N19-116 | `LogRedactorProvider` | class | LogRedactor saglayici uygulamasi | VersaCoder.Crosscutting | net48;net10.0-windows | LogRedactorTests | planli |
| N19-117 | `LogRedactorRegistry` | class | LogRedactor kayit/katalog yonetimi | VersaCoder.Crosscutting | net48;net10.0-windows | LogRedactorTests | planli |
| N19-118 | `LogRedactorConverter` | class | LogRedactor bicim donusumu | VersaCoder.Crosscutting | net48;net10.0-windows | LogRedactorTests | planli |
| N19-119 | `LogRedactorTests` | class | LogRedactor testleri (xUnit) | VersaCoder.Crosscutting | net48;net10.0-windows | - | planli |
| N19-120 | `LogRedactorBuilder` | class | LogRedactor uretici (builder) | VersaCoder.Crosscutting | net48;net10.0-windows | LogRedactorTests | planli |
