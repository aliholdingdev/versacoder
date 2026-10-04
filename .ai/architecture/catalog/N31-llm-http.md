---
title: "Katalog Dugumu N31 - LLM HTTP (l4)"
type: catalog
category: architecture
status: needs-validation
version: 1.0.0
updated: 2026-10-04
authority: derived
scope: "N31 llm-http dugumu: kimlik, src klasoru, aile yapisi, ornek kayit ve yon kurali (ADR-013 + ADR-028)"
---

# Katalog Dugumu N31 - LLM HTTP

**Zorunlu Baglantilar:** [catalog index](index.md) · [node-inventory](node-inventory.md) · [mimari index](../index.md) · [ADR-028](../../.decisions/accepted/ADR-028-layered-baseline-9-projects.md) · [ADR-013](../../.decisions/accepted/ADR-013-layered-architecture-52-nodes.md)

> **Tek cumlelik sorumluluk:** *N31 llm-http hangi projede ve klasorde yasar, ne yapar, kac kayit hedefler* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

**N31 (llm-http), l4 Infrastructure katmaninin alt katmanidir: `VersaCoder.Infrastructure` projesinde `src/VersaCoder.Infrastructure/Llm/` klasorunde yasar.** LLM saglayicisina HTTP/SSE istemcisini token sayimi ve oran butcesiyle yurutur. 10 aile x 12 rol = 120 kayit hedefi (ADR-013 asgari 112); durum `planli`.

Kayitlar ADR-013 2.1 biciminde 8 sutun tasir; bu dosya aile kalibini ve ornek satirlari gosterir, tam 120 satirlik liste bu dosyanin boyut sinirina sigmaz (DOĞRULAMA GEREKLI).

```text
+--------------------------------------------------+
| l4 Infrastructure -> VersaCoder.Infrastructure
| src/VersaCoder.Infrastructure/Llm/
| 10 aile x 12 rol = 120 kayit (esik 112)
+--------------------------------------------------+
```

| Alan | Deger |
|---|---|
| Dugum | N31 |
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
| 1 | LLM HTTP/SSE istemcisi, token sayimi ve oran butcesini yurutmek | ekran acmak veya protokol bicimi uretmek (l5, l7) |
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
| 1 | N31 | llm-http | `src/VersaCoder.Infrastructure/Llm/` |

**Kanit yolu (ADR-028 2.3/5):** tarama yolu gercek proje adini icerir; genis kalip (`src/**/Domain/**`) sahte PASS uretir.

```powershell
Test-Path 'C:\versacoder\src\VersaCoder.Infrastructure\Llm'
```

Dizin su an yok cikti `False` olur; PASS yazilmaz -> **DOĞRULAMA GEREKLI**.

---

## §3 Aile Yapisi

Rol kalibi (12): `I<F>` · `<F>` · `<F>Dto` · `<F>Mapper` · `<F>Validator` · `<F>Factory` · `<F>Options` · `<F>Provider` · `<F>Registry` · `<F>Converter` · `<F>Tests` · `<F>Builder`

| # | Aile | # | Aile |
|---|---|---|---|
| 1 | LlmHttpClient | 6 | RateBudget |
| 2 | SseReader | 7 | HttpRetry |
| 3 | PromptSerializer | 8 | AuthHeader |
| 4 | TokenCounter | 9 | StreamAggregator |
| 5 | ModelCatalog | 10 | LlmResponseParser |

Her aile ayni rol kalibini 12 kez tekrarlar; kayit sayisi = aile x rol.

---

## §4 Kayit Ornegi (8 sutun)

| ID | Bilesen | Tip | Sorumluluk | Proje | TFM | Test | Durum |
|---|---|---|---|---|---|---|---|
| N31-001 | `ILlmHttpClient` | interface | LlmHttpClient sozlesmesi (port) | VersaCoder.Infrastructure | net48;net10.0-windows | LlmHttpClientTests | planli |
| N31-002 | `LlmHttpClient` | class | LlmHttpClient cekirdek uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | LlmHttpClientTests | planli |
| N31-003 | `LlmHttpClientDto` | record | LlmHttpClient veri transferi modeli | VersaCoder.Infrastructure | net48;net10.0-windows | LlmHttpClientTests | planli |
| N31-004 | `LlmHttpClientMapper` | class | LlmHttpClient <-> DTO esleme | VersaCoder.Infrastructure | net48;net10.0-windows | LlmHttpClientTests | planli |
| N31-005 | `LlmHttpClientValidator` | class | LlmHttpClient dogrulama kurallari | VersaCoder.Infrastructure | net48;net10.0-windows | LlmHttpClientTests | planli |
| N31-006 | `LlmHttpClientFactory` | class | LlmHttpClient uretimi ve varsayilanlari | VersaCoder.Infrastructure | net48;net10.0-windows | LlmHttpClientTests | planli |

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
| 3 | Test sinifi | LlmHttpClientTests (xUnit) | planli |
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
(Select-String -Path 'C:\versacoder\.ai\architecture\catalog\N31-llm-http.md' -Pattern '^\|\s*N\d{2}-\d{3}\s*\|').Count
# 2) boyut bandi - beklenen: 2600-8500
(Get-Item 'C:\versacoder\.ai\architecture\catalog\N31-llm-http.md').Length
# 3) klasor kaniti - gercek proje adi (ADR-028 2.3/5)
(Test-Path 'C:\versacoder\src\VersaCoder.Infrastructure\Llm')
```

Katalog sayimi `powershell -NoProfile -ExecutionPolicy Bypass -File C:\versacoder\.ai\scripts\count-components.ps1` ile alinir; bu batch 26 node dosyasiyla 52 dugum klasorunu tamamlar - kayit esigi (5000) ornek satirlarla henuz karsilanmaz (DOĞRULAMA GEREKLI).

## §10 Kayit Tablosu (ADR-013 2.2 - kaynak vault port)

| N31-007 | `LlmHttpClientOptions` | record | LlmHttpClient yapilandirmasi | VersaCoder.Infrastructure | net48;net10.0-windows | LlmHttpClientTests | planli |
| N31-008 | `LlmHttpClientProvider` | class | LlmHttpClient saglayici uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | LlmHttpClientTests | planli |
| N31-009 | `LlmHttpClientRegistry` | class | LlmHttpClient kayit/katalog yonetimi | VersaCoder.Infrastructure | net48;net10.0-windows | LlmHttpClientTests | planli |
| N31-010 | `LlmHttpClientConverter` | class | LlmHttpClient bicim donusumu | VersaCoder.Infrastructure | net48;net10.0-windows | LlmHttpClientTests | planli |
| N31-011 | `LlmHttpClientTests` | class | LlmHttpClient testleri (xUnit) | VersaCoder.Infrastructure | net48;net10.0-windows | - | planli |
| N31-012 | `LlmHttpClientBuilder` | class | LlmHttpClient uretici (builder) | VersaCoder.Infrastructure | net48;net10.0-windows | LlmHttpClientTests | planli |
| N31-013 | `ISseReader` | interface | SseReader sozlesmesi (port) | VersaCoder.Infrastructure | net48;net10.0-windows | SseReaderTests | planli |
| N31-014 | `SseReader` | class | SseReader cekirdek uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | SseReaderTests | planli |
| N31-015 | `SseReaderDto` | record | SseReader veri transferi modeli | VersaCoder.Infrastructure | net48;net10.0-windows | SseReaderTests | planli |
| N31-016 | `SseReaderMapper` | class | SseReader <-> DTO esleme | VersaCoder.Infrastructure | net48;net10.0-windows | SseReaderTests | planli |
| N31-017 | `SseReaderValidator` | class | SseReader dogrulama kurallari | VersaCoder.Infrastructure | net48;net10.0-windows | SseReaderTests | planli |
| N31-018 | `SseReaderFactory` | class | SseReader uretimi ve varsayilanlari | VersaCoder.Infrastructure | net48;net10.0-windows | SseReaderTests | planli |
| N31-019 | `SseReaderOptions` | record | SseReader yapilandirmasi | VersaCoder.Infrastructure | net48;net10.0-windows | SseReaderTests | planli |
| N31-020 | `SseReaderProvider` | class | SseReader saglayici uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | SseReaderTests | planli |
| N31-021 | `SseReaderRegistry` | class | SseReader kayit/katalog yonetimi | VersaCoder.Infrastructure | net48;net10.0-windows | SseReaderTests | planli |
| N31-022 | `SseReaderConverter` | class | SseReader bicim donusumu | VersaCoder.Infrastructure | net48;net10.0-windows | SseReaderTests | planli |
| N31-023 | `SseReaderTests` | class | SseReader testleri (xUnit) | VersaCoder.Infrastructure | net48;net10.0-windows | - | planli |
| N31-024 | `SseReaderBuilder` | class | SseReader uretici (builder) | VersaCoder.Infrastructure | net48;net10.0-windows | SseReaderTests | planli |
| N31-025 | `IPromptSerializer` | interface | PromptSerializer sozlesmesi (port) | VersaCoder.Infrastructure | net48;net10.0-windows | PromptSerializerTests | planli |
| N31-026 | `PromptSerializer` | class | PromptSerializer cekirdek uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | PromptSerializerTests | planli |
| N31-027 | `PromptSerializerDto` | record | PromptSerializer veri transferi modeli | VersaCoder.Infrastructure | net48;net10.0-windows | PromptSerializerTests | planli |
| N31-028 | `PromptSerializerMapper` | class | PromptSerializer <-> DTO esleme | VersaCoder.Infrastructure | net48;net10.0-windows | PromptSerializerTests | planli |
| N31-029 | `PromptSerializerValidator` | class | PromptSerializer dogrulama kurallari | VersaCoder.Infrastructure | net48;net10.0-windows | PromptSerializerTests | planli |
| N31-030 | `PromptSerializerFactory` | class | PromptSerializer uretimi ve varsayilanlari | VersaCoder.Infrastructure | net48;net10.0-windows | PromptSerializerTests | planli |
| N31-031 | `PromptSerializerOptions` | record | PromptSerializer yapilandirmasi | VersaCoder.Infrastructure | net48;net10.0-windows | PromptSerializerTests | planli |
| N31-032 | `PromptSerializerProvider` | class | PromptSerializer saglayici uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | PromptSerializerTests | planli |
| N31-033 | `PromptSerializerRegistry` | class | PromptSerializer kayit/katalog yonetimi | VersaCoder.Infrastructure | net48;net10.0-windows | PromptSerializerTests | planli |
| N31-034 | `PromptSerializerConverter` | class | PromptSerializer bicim donusumu | VersaCoder.Infrastructure | net48;net10.0-windows | PromptSerializerTests | planli |
| N31-035 | `PromptSerializerTests` | class | PromptSerializer testleri (xUnit) | VersaCoder.Infrastructure | net48;net10.0-windows | - | planli |
| N31-036 | `PromptSerializerBuilder` | class | PromptSerializer uretici (builder) | VersaCoder.Infrastructure | net48;net10.0-windows | PromptSerializerTests | planli |
| N31-037 | `ITokenCounter` | interface | TokenCounter sozlesmesi (port) | VersaCoder.Infrastructure | net48;net10.0-windows | TokenCounterTests | planli |
| N31-038 | `TokenCounter` | class | TokenCounter cekirdek uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | TokenCounterTests | planli |
| N31-039 | `TokenCounterDto` | record | TokenCounter veri transferi modeli | VersaCoder.Infrastructure | net48;net10.0-windows | TokenCounterTests | planli |
| N31-040 | `TokenCounterMapper` | class | TokenCounter <-> DTO esleme | VersaCoder.Infrastructure | net48;net10.0-windows | TokenCounterTests | planli |
| N31-041 | `TokenCounterValidator` | class | TokenCounter dogrulama kurallari | VersaCoder.Infrastructure | net48;net10.0-windows | TokenCounterTests | planli |
| N31-042 | `TokenCounterFactory` | class | TokenCounter uretimi ve varsayilanlari | VersaCoder.Infrastructure | net48;net10.0-windows | TokenCounterTests | planli |
| N31-043 | `TokenCounterOptions` | record | TokenCounter yapilandirmasi | VersaCoder.Infrastructure | net48;net10.0-windows | TokenCounterTests | planli |
| N31-044 | `TokenCounterProvider` | class | TokenCounter saglayici uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | TokenCounterTests | planli |
| N31-045 | `TokenCounterRegistry` | class | TokenCounter kayit/katalog yonetimi | VersaCoder.Infrastructure | net48;net10.0-windows | TokenCounterTests | planli |
| N31-046 | `TokenCounterConverter` | class | TokenCounter bicim donusumu | VersaCoder.Infrastructure | net48;net10.0-windows | TokenCounterTests | planli |
| N31-047 | `TokenCounterTests` | class | TokenCounter testleri (xUnit) | VersaCoder.Infrastructure | net48;net10.0-windows | - | planli |
| N31-048 | `TokenCounterBuilder` | class | TokenCounter uretici (builder) | VersaCoder.Infrastructure | net48;net10.0-windows | TokenCounterTests | planli |
| N31-049 | `IModelCatalog` | interface | ModelCatalog sozlesmesi (port) | VersaCoder.Infrastructure | net48;net10.0-windows | ModelCatalogTests | planli |
| N31-050 | `ModelCatalog` | class | ModelCatalog cekirdek uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | ModelCatalogTests | planli |
| N31-051 | `ModelCatalogDto` | record | ModelCatalog veri transferi modeli | VersaCoder.Infrastructure | net48;net10.0-windows | ModelCatalogTests | planli |
| N31-052 | `ModelCatalogMapper` | class | ModelCatalog <-> DTO esleme | VersaCoder.Infrastructure | net48;net10.0-windows | ModelCatalogTests | planli |
| N31-053 | `ModelCatalogValidator` | class | ModelCatalog dogrulama kurallari | VersaCoder.Infrastructure | net48;net10.0-windows | ModelCatalogTests | planli |
| N31-054 | `ModelCatalogFactory` | class | ModelCatalog uretimi ve varsayilanlari | VersaCoder.Infrastructure | net48;net10.0-windows | ModelCatalogTests | planli |
| N31-055 | `ModelCatalogOptions` | record | ModelCatalog yapilandirmasi | VersaCoder.Infrastructure | net48;net10.0-windows | ModelCatalogTests | planli |
| N31-056 | `ModelCatalogProvider` | class | ModelCatalog saglayici uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | ModelCatalogTests | planli |
| N31-057 | `ModelCatalogRegistry` | class | ModelCatalog kayit/katalog yonetimi | VersaCoder.Infrastructure | net48;net10.0-windows | ModelCatalogTests | planli |
| N31-058 | `ModelCatalogConverter` | class | ModelCatalog bicim donusumu | VersaCoder.Infrastructure | net48;net10.0-windows | ModelCatalogTests | planli |
| N31-059 | `ModelCatalogTests` | class | ModelCatalog testleri (xUnit) | VersaCoder.Infrastructure | net48;net10.0-windows | - | planli |
| N31-060 | `ModelCatalogBuilder` | class | ModelCatalog uretici (builder) | VersaCoder.Infrastructure | net48;net10.0-windows | ModelCatalogTests | planli |
| N31-061 | `IRateBudget` | interface | RateBudget sozlesmesi (port) | VersaCoder.Infrastructure | net48;net10.0-windows | RateBudgetTests | planli |
| N31-062 | `RateBudget` | class | RateBudget cekirdek uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | RateBudgetTests | planli |
| N31-063 | `RateBudgetDto` | record | RateBudget veri transferi modeli | VersaCoder.Infrastructure | net48;net10.0-windows | RateBudgetTests | planli |
| N31-064 | `RateBudgetMapper` | class | RateBudget <-> DTO esleme | VersaCoder.Infrastructure | net48;net10.0-windows | RateBudgetTests | planli |
| N31-065 | `RateBudgetValidator` | class | RateBudget dogrulama kurallari | VersaCoder.Infrastructure | net48;net10.0-windows | RateBudgetTests | planli |
| N31-066 | `RateBudgetFactory` | class | RateBudget uretimi ve varsayilanlari | VersaCoder.Infrastructure | net48;net10.0-windows | RateBudgetTests | planli |
| N31-067 | `RateBudgetOptions` | record | RateBudget yapilandirmasi | VersaCoder.Infrastructure | net48;net10.0-windows | RateBudgetTests | planli |
| N31-068 | `RateBudgetProvider` | class | RateBudget saglayici uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | RateBudgetTests | planli |
| N31-069 | `RateBudgetRegistry` | class | RateBudget kayit/katalog yonetimi | VersaCoder.Infrastructure | net48;net10.0-windows | RateBudgetTests | planli |
| N31-070 | `RateBudgetConverter` | class | RateBudget bicim donusumu | VersaCoder.Infrastructure | net48;net10.0-windows | RateBudgetTests | planli |
| N31-071 | `RateBudgetTests` | class | RateBudget testleri (xUnit) | VersaCoder.Infrastructure | net48;net10.0-windows | - | planli |
| N31-072 | `RateBudgetBuilder` | class | RateBudget uretici (builder) | VersaCoder.Infrastructure | net48;net10.0-windows | RateBudgetTests | planli |
| N31-073 | `IHttpRetry` | interface | HttpRetry sozlesmesi (port) | VersaCoder.Infrastructure | net48;net10.0-windows | HttpRetryTests | planli |
| N31-074 | `HttpRetry` | class | HttpRetry cekirdek uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | HttpRetryTests | planli |
| N31-075 | `HttpRetryDto` | record | HttpRetry veri transferi modeli | VersaCoder.Infrastructure | net48;net10.0-windows | HttpRetryTests | planli |
| N31-076 | `HttpRetryMapper` | class | HttpRetry <-> DTO esleme | VersaCoder.Infrastructure | net48;net10.0-windows | HttpRetryTests | planli |
| N31-077 | `HttpRetryValidator` | class | HttpRetry dogrulama kurallari | VersaCoder.Infrastructure | net48;net10.0-windows | HttpRetryTests | planli |
| N31-078 | `HttpRetryFactory` | class | HttpRetry uretimi ve varsayilanlari | VersaCoder.Infrastructure | net48;net10.0-windows | HttpRetryTests | planli |
| N31-079 | `HttpRetryOptions` | record | HttpRetry yapilandirmasi | VersaCoder.Infrastructure | net48;net10.0-windows | HttpRetryTests | planli |
| N31-080 | `HttpRetryProvider` | class | HttpRetry saglayici uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | HttpRetryTests | planli |
| N31-081 | `HttpRetryRegistry` | class | HttpRetry kayit/katalog yonetimi | VersaCoder.Infrastructure | net48;net10.0-windows | HttpRetryTests | planli |
| N31-082 | `HttpRetryConverter` | class | HttpRetry bicim donusumu | VersaCoder.Infrastructure | net48;net10.0-windows | HttpRetryTests | planli |
| N31-083 | `HttpRetryTests` | class | HttpRetry testleri (xUnit) | VersaCoder.Infrastructure | net48;net10.0-windows | - | planli |
| N31-084 | `HttpRetryBuilder` | class | HttpRetry uretici (builder) | VersaCoder.Infrastructure | net48;net10.0-windows | HttpRetryTests | planli |
| N31-085 | `IAuthHeader` | interface | AuthHeader sozlesmesi (port) | VersaCoder.Infrastructure | net48;net10.0-windows | AuthHeaderTests | planli |
| N31-086 | `AuthHeader` | class | AuthHeader cekirdek uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | AuthHeaderTests | planli |
| N31-087 | `AuthHeaderDto` | record | AuthHeader veri transferi modeli | VersaCoder.Infrastructure | net48;net10.0-windows | AuthHeaderTests | planli |
| N31-088 | `AuthHeaderMapper` | class | AuthHeader <-> DTO esleme | VersaCoder.Infrastructure | net48;net10.0-windows | AuthHeaderTests | planli |
| N31-089 | `AuthHeaderValidator` | class | AuthHeader dogrulama kurallari | VersaCoder.Infrastructure | net48;net10.0-windows | AuthHeaderTests | planli |
| N31-090 | `AuthHeaderFactory` | class | AuthHeader uretimi ve varsayilanlari | VersaCoder.Infrastructure | net48;net10.0-windows | AuthHeaderTests | planli |
| N31-091 | `AuthHeaderOptions` | record | AuthHeader yapilandirmasi | VersaCoder.Infrastructure | net48;net10.0-windows | AuthHeaderTests | planli |
| N31-092 | `AuthHeaderProvider` | class | AuthHeader saglayici uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | AuthHeaderTests | planli |
| N31-093 | `AuthHeaderRegistry` | class | AuthHeader kayit/katalog yonetimi | VersaCoder.Infrastructure | net48;net10.0-windows | AuthHeaderTests | planli |
| N31-094 | `AuthHeaderConverter` | class | AuthHeader bicim donusumu | VersaCoder.Infrastructure | net48;net10.0-windows | AuthHeaderTests | planli |
| N31-095 | `AuthHeaderTests` | class | AuthHeader testleri (xUnit) | VersaCoder.Infrastructure | net48;net10.0-windows | - | planli |
| N31-096 | `AuthHeaderBuilder` | class | AuthHeader uretici (builder) | VersaCoder.Infrastructure | net48;net10.0-windows | AuthHeaderTests | planli |
| N31-097 | `IStreamAggregator` | interface | StreamAggregator sozlesmesi (port) | VersaCoder.Infrastructure | net48;net10.0-windows | StreamAggregatorTests | planli |
| N31-098 | `StreamAggregator` | class | StreamAggregator cekirdek uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | StreamAggregatorTests | planli |
| N31-099 | `StreamAggregatorDto` | record | StreamAggregator veri transferi modeli | VersaCoder.Infrastructure | net48;net10.0-windows | StreamAggregatorTests | planli |
| N31-100 | `StreamAggregatorMapper` | class | StreamAggregator <-> DTO esleme | VersaCoder.Infrastructure | net48;net10.0-windows | StreamAggregatorTests | planli |
| N31-101 | `StreamAggregatorValidator` | class | StreamAggregator dogrulama kurallari | VersaCoder.Infrastructure | net48;net10.0-windows | StreamAggregatorTests | planli |
| N31-102 | `StreamAggregatorFactory` | class | StreamAggregator uretimi ve varsayilanlari | VersaCoder.Infrastructure | net48;net10.0-windows | StreamAggregatorTests | planli |
| N31-103 | `StreamAggregatorOptions` | record | StreamAggregator yapilandirmasi | VersaCoder.Infrastructure | net48;net10.0-windows | StreamAggregatorTests | planli |
| N31-104 | `StreamAggregatorProvider` | class | StreamAggregator saglayici uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | StreamAggregatorTests | planli |
| N31-105 | `StreamAggregatorRegistry` | class | StreamAggregator kayit/katalog yonetimi | VersaCoder.Infrastructure | net48;net10.0-windows | StreamAggregatorTests | planli |
| N31-106 | `StreamAggregatorConverter` | class | StreamAggregator bicim donusumu | VersaCoder.Infrastructure | net48;net10.0-windows | StreamAggregatorTests | planli |
| N31-107 | `StreamAggregatorTests` | class | StreamAggregator testleri (xUnit) | VersaCoder.Infrastructure | net48;net10.0-windows | - | planli |
| N31-108 | `StreamAggregatorBuilder` | class | StreamAggregator uretici (builder) | VersaCoder.Infrastructure | net48;net10.0-windows | StreamAggregatorTests | planli |
| N31-109 | `ILlmResponseParser` | interface | LlmResponseParser sozlesmesi (port) | VersaCoder.Infrastructure | net48;net10.0-windows | LlmResponseParserTests | planli |
| N31-110 | `LlmResponseParser` | class | LlmResponseParser cekirdek uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | LlmResponseParserTests | planli |
| N31-111 | `LlmResponseParserDto` | record | LlmResponseParser veri transferi modeli | VersaCoder.Infrastructure | net48;net10.0-windows | LlmResponseParserTests | planli |
| N31-112 | `LlmResponseParserMapper` | class | LlmResponseParser <-> DTO esleme | VersaCoder.Infrastructure | net48;net10.0-windows | LlmResponseParserTests | planli |
| N31-113 | `LlmResponseParserValidator` | class | LlmResponseParser dogrulama kurallari | VersaCoder.Infrastructure | net48;net10.0-windows | LlmResponseParserTests | planli |
| N31-114 | `LlmResponseParserFactory` | class | LlmResponseParser uretimi ve varsayilanlari | VersaCoder.Infrastructure | net48;net10.0-windows | LlmResponseParserTests | planli |
| N31-115 | `LlmResponseParserOptions` | record | LlmResponseParser yapilandirmasi | VersaCoder.Infrastructure | net48;net10.0-windows | LlmResponseParserTests | planli |
| N31-116 | `LlmResponseParserProvider` | class | LlmResponseParser saglayici uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | LlmResponseParserTests | planli |
| N31-117 | `LlmResponseParserRegistry` | class | LlmResponseParser kayit/katalog yonetimi | VersaCoder.Infrastructure | net48;net10.0-windows | LlmResponseParserTests | planli |
| N31-118 | `LlmResponseParserConverter` | class | LlmResponseParser bicim donusumu | VersaCoder.Infrastructure | net48;net10.0-windows | LlmResponseParserTests | planli |
| N31-119 | `LlmResponseParserTests` | class | LlmResponseParser testleri (xUnit) | VersaCoder.Infrastructure | net48;net10.0-windows | - | planli |
| N31-120 | `LlmResponseParserBuilder` | class | LlmResponseParser uretici (builder) | VersaCoder.Infrastructure | net48;net10.0-windows | LlmResponseParserTests | planli |
