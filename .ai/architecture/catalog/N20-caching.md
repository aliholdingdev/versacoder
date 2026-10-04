---
title: "Katalog Dugumu N20 - Onbek (l3)"
type: catalog
category: architecture
status: needs-validation
version: 1.0.0
updated: 2026-10-04
authority: derived
scope: "N20 caching dugumu: kimlik, src klasoru, aile yapisi, ornek kayit ve yon kurali (ADR-013 + ADR-028)"
---

# Katalog Dugumu N20 - Onbek

**Zorunlu Baglantilar:** [catalog index](index.md) · [node-inventory](node-inventory.md) · [mimari index](../index.md) · [ADR-028](../../.decisions/accepted/ADR-028-layered-baseline-9-projects.md) · [ADR-013](../../.decisions/accepted/ADR-013-layered-architecture-52-nodes.md)

> **Tek cumlelik sorumluluk:** *N20 caching hangi projede ve klasorde yasar, ne yapar, kac kayit hedefler* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

**N20 (caching), l3 Crosscutting katmaninin alt katmanidir: `VersaCoder.Crosscutting` projesinde `src/VersaCoder.Crosscutting/Caching/` klasorunde yasar.** Bellek ve dosya onbekini, anahtar ile gecersizlestirme politikalarini tanimlar. 10 aile x 12 rol = 120 kayit hedefi (ADR-013 asgari 112); durum `planli`.

Kayitlar ADR-013 2.1 biciminde 8 sutun tasir; bu dosya aile kalibini ve ornek satirlari gosterir, tam 120 satirlik liste bu dosyanin boyut sinirina sigmaz (DOĞRULAMA GEREKLI).

```text
+--------------------------------------------------+
| l3 Crosscutting -> VersaCoder.Crosscutting
| src/VersaCoder.Crosscutting/Caching/
| 10 aile x 12 rol = 120 kayit (esik 112)
+--------------------------------------------------+
```

| Alan | Deger |
|---|---|
| Dugum | N20 |
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
| 1 | Bellek ve dosya onbekini, anahtar ile gecersizlestirme politikalarini tanimlar | l0 disinda bir katmana dogrudan bakmak |
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
| 1 | N20 | caching | `src/VersaCoder.Crosscutting/Caching/` |

**Kanit yolu (ADR-028 2.3/5):** tarama yolu gercek proje adini icerir; genis kalip (`src/**/Domain/**`) sahte PASS uretir.

```powershell
Test-Path 'C:\versacoder\src\VersaCoder.Crosscutting\Caching'
```

Dizin su an yok cikti `False` olur; PASS yazilmaz -> **DOĞRULAMA GEREKLI**.

---

## §3 Aile Yapisi

Rol kalibi (12): `I<F>` · `<F>` · `<F>Dto` · `<F>Mapper` · `<F>Validator` · `<F>Factory` · `<F>Options` · `<F>Provider` · `<F>Registry` · `<F>Converter` · `<F>Tests` · `<F>Builder`

| # | Aile | # | Aile |
|---|---|---|---|
| 1 | MemoryCache | 6 | SlidingWindow |
| 2 | CacheKey | 7 | StampedeGuard |
| 3 | CacheEntry | 8 | CacheStats |
| 4 | CachePolicy | 9 | FileCache |
| 5 | CacheInvalidator | 10 | CacheSerializer |

Her aile ayni rol kalibini 12 kez tekrarlar; kayit sayisi = aile x rol.

---

## §4 Kayit Ornegi (8 sutun)

| ID | Bilesen | Tip | Sorumluluk | Proje | TFM | Test | Durum |
|---|---|---|---|---|---|---|---|
| N20-001 | `IMemoryCache` | interface | MemoryCache sozlesmesi (port) | VersaCoder.Crosscutting | net48;net10.0-windows | MemoryCacheTests | planli |
| N20-002 | `MemoryCache` | class | MemoryCache cekirdek uygulamasi | VersaCoder.Crosscutting | net48;net10.0-windows | MemoryCacheTests | planli |
| N20-003 | `MemoryCacheDto` | record | MemoryCache veri transferi modeli | VersaCoder.Crosscutting | net48;net10.0-windows | MemoryCacheTests | planli |
| N20-004 | `MemoryCacheMapper` | class | MemoryCache <-> DTO esleme | VersaCoder.Crosscutting | net48;net10.0-windows | MemoryCacheTests | planli |
| N20-005 | `MemoryCacheValidator` | class | MemoryCache dogrulama kurallari | VersaCoder.Crosscutting | net48;net10.0-windows | MemoryCacheTests | planli |
| N20-006 | `MemoryCacheFactory` | class | MemoryCache uretimi ve varsayilanlari | VersaCoder.Crosscutting | net48;net10.0-windows | MemoryCacheTests | planli |

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
| 3 | Test sinifi | MemoryCacheTests (xUnit) | planli |
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
(Select-String -Path 'C:\versacoder\.ai\architecture\catalog\N20-caching.md' -Pattern '^\|\s*N\d{2}-\d{3}\s*\|').Count
# 2) boyut bandi - beklenen: 2600-8500
(Get-Item 'C:\versacoder\.ai\architecture\catalog\N20-caching.md').Length
# 3) klasor kaniti - gercek proje adi (ADR-028 2.3/5)
(Test-Path 'C:\versacoder\src\VersaCoder.Crosscutting\Caching')
```

Katalog sayimi `powershell -NoProfile -ExecutionPolicy Bypass -File C:\versacoder\.ai\scripts\count-components.ps1` ile alinir; bu batch sonunda toplam esik (5000) henuz karsilanmaz - kalan 26 node dosyasi sonraki batchtir.

## §10 Kayit Tablosu (ADR-013 2.2 - kaynak vault port)

| N20-007 | `MemoryCacheOptions` | record | MemoryCache yapilandirmasi | VersaCoder.Crosscutting | net48;net10.0-windows | MemoryCacheTests | planli |
| N20-008 | `MemoryCacheProvider` | class | MemoryCache saglayici uygulamasi | VersaCoder.Crosscutting | net48;net10.0-windows | MemoryCacheTests | planli |
| N20-009 | `MemoryCacheRegistry` | class | MemoryCache kayit/katalog yonetimi | VersaCoder.Crosscutting | net48;net10.0-windows | MemoryCacheTests | planli |
| N20-010 | `MemoryCacheConverter` | class | MemoryCache bicim donusumu | VersaCoder.Crosscutting | net48;net10.0-windows | MemoryCacheTests | planli |
| N20-011 | `MemoryCacheTests` | class | MemoryCache testleri (xUnit) | VersaCoder.Crosscutting | net48;net10.0-windows | - | planli |
| N20-012 | `MemoryCacheBuilder` | class | MemoryCache uretici (builder) | VersaCoder.Crosscutting | net48;net10.0-windows | MemoryCacheTests | planli |
| N20-013 | `ICacheKey` | interface | CacheKey sozlesmesi (port) | VersaCoder.Crosscutting | net48;net10.0-windows | CacheKeyTests | planli |
| N20-014 | `CacheKey` | class | CacheKey cekirdek uygulamasi | VersaCoder.Crosscutting | net48;net10.0-windows | CacheKeyTests | planli |
| N20-015 | `CacheKeyDto` | record | CacheKey veri transferi modeli | VersaCoder.Crosscutting | net48;net10.0-windows | CacheKeyTests | planli |
| N20-016 | `CacheKeyMapper` | class | CacheKey <-> DTO esleme | VersaCoder.Crosscutting | net48;net10.0-windows | CacheKeyTests | planli |
| N20-017 | `CacheKeyValidator` | class | CacheKey dogrulama kurallari | VersaCoder.Crosscutting | net48;net10.0-windows | CacheKeyTests | planli |
| N20-018 | `CacheKeyFactory` | class | CacheKey uretimi ve varsayilanlari | VersaCoder.Crosscutting | net48;net10.0-windows | CacheKeyTests | planli |
| N20-019 | `CacheKeyOptions` | record | CacheKey yapilandirmasi | VersaCoder.Crosscutting | net48;net10.0-windows | CacheKeyTests | planli |
| N20-020 | `CacheKeyProvider` | class | CacheKey saglayici uygulamasi | VersaCoder.Crosscutting | net48;net10.0-windows | CacheKeyTests | planli |
| N20-021 | `CacheKeyRegistry` | class | CacheKey kayit/katalog yonetimi | VersaCoder.Crosscutting | net48;net10.0-windows | CacheKeyTests | planli |
| N20-022 | `CacheKeyConverter` | class | CacheKey bicim donusumu | VersaCoder.Crosscutting | net48;net10.0-windows | CacheKeyTests | planli |
| N20-023 | `CacheKeyTests` | class | CacheKey testleri (xUnit) | VersaCoder.Crosscutting | net48;net10.0-windows | - | planli |
| N20-024 | `CacheKeyBuilder` | class | CacheKey uretici (builder) | VersaCoder.Crosscutting | net48;net10.0-windows | CacheKeyTests | planli |
| N20-025 | `ICacheEntry` | interface | CacheEntry sozlesmesi (port) | VersaCoder.Crosscutting | net48;net10.0-windows | CacheEntryTests | planli |
| N20-026 | `CacheEntry` | class | CacheEntry cekirdek uygulamasi | VersaCoder.Crosscutting | net48;net10.0-windows | CacheEntryTests | planli |
| N20-027 | `CacheEntryDto` | record | CacheEntry veri transferi modeli | VersaCoder.Crosscutting | net48;net10.0-windows | CacheEntryTests | planli |
| N20-028 | `CacheEntryMapper` | class | CacheEntry <-> DTO esleme | VersaCoder.Crosscutting | net48;net10.0-windows | CacheEntryTests | planli |
| N20-029 | `CacheEntryValidator` | class | CacheEntry dogrulama kurallari | VersaCoder.Crosscutting | net48;net10.0-windows | CacheEntryTests | planli |
| N20-030 | `CacheEntryFactory` | class | CacheEntry uretimi ve varsayilanlari | VersaCoder.Crosscutting | net48;net10.0-windows | CacheEntryTests | planli |
| N20-031 | `CacheEntryOptions` | record | CacheEntry yapilandirmasi | VersaCoder.Crosscutting | net48;net10.0-windows | CacheEntryTests | planli |
| N20-032 | `CacheEntryProvider` | class | CacheEntry saglayici uygulamasi | VersaCoder.Crosscutting | net48;net10.0-windows | CacheEntryTests | planli |
| N20-033 | `CacheEntryRegistry` | class | CacheEntry kayit/katalog yonetimi | VersaCoder.Crosscutting | net48;net10.0-windows | CacheEntryTests | planli |
| N20-034 | `CacheEntryConverter` | class | CacheEntry bicim donusumu | VersaCoder.Crosscutting | net48;net10.0-windows | CacheEntryTests | planli |
| N20-035 | `CacheEntryTests` | class | CacheEntry testleri (xUnit) | VersaCoder.Crosscutting | net48;net10.0-windows | - | planli |
| N20-036 | `CacheEntryBuilder` | class | CacheEntry uretici (builder) | VersaCoder.Crosscutting | net48;net10.0-windows | CacheEntryTests | planli |
| N20-037 | `ICachePolicy` | interface | CachePolicy sozlesmesi (port) | VersaCoder.Crosscutting | net48;net10.0-windows | CachePolicyTests | planli |
| N20-038 | `CachePolicy` | class | CachePolicy cekirdek uygulamasi | VersaCoder.Crosscutting | net48;net10.0-windows | CachePolicyTests | planli |
| N20-039 | `CachePolicyDto` | record | CachePolicy veri transferi modeli | VersaCoder.Crosscutting | net48;net10.0-windows | CachePolicyTests | planli |
| N20-040 | `CachePolicyMapper` | class | CachePolicy <-> DTO esleme | VersaCoder.Crosscutting | net48;net10.0-windows | CachePolicyTests | planli |
| N20-041 | `CachePolicyValidator` | class | CachePolicy dogrulama kurallari | VersaCoder.Crosscutting | net48;net10.0-windows | CachePolicyTests | planli |
| N20-042 | `CachePolicyFactory` | class | CachePolicy uretimi ve varsayilanlari | VersaCoder.Crosscutting | net48;net10.0-windows | CachePolicyTests | planli |
| N20-043 | `CachePolicyOptions` | record | CachePolicy yapilandirmasi | VersaCoder.Crosscutting | net48;net10.0-windows | CachePolicyTests | planli |
| N20-044 | `CachePolicyProvider` | class | CachePolicy saglayici uygulamasi | VersaCoder.Crosscutting | net48;net10.0-windows | CachePolicyTests | planli |
| N20-045 | `CachePolicyRegistry` | class | CachePolicy kayit/katalog yonetimi | VersaCoder.Crosscutting | net48;net10.0-windows | CachePolicyTests | planli |
| N20-046 | `CachePolicyConverter` | class | CachePolicy bicim donusumu | VersaCoder.Crosscutting | net48;net10.0-windows | CachePolicyTests | planli |
| N20-047 | `CachePolicyTests` | class | CachePolicy testleri (xUnit) | VersaCoder.Crosscutting | net48;net10.0-windows | - | planli |
| N20-048 | `CachePolicyBuilder` | class | CachePolicy uretici (builder) | VersaCoder.Crosscutting | net48;net10.0-windows | CachePolicyTests | planli |
| N20-049 | `ICacheInvalidator` | interface | CacheInvalidator sozlesmesi (port) | VersaCoder.Crosscutting | net48;net10.0-windows | CacheInvalidatorTests | planli |
| N20-050 | `CacheInvalidator` | class | CacheInvalidator cekirdek uygulamasi | VersaCoder.Crosscutting | net48;net10.0-windows | CacheInvalidatorTests | planli |
| N20-051 | `CacheInvalidatorDto` | record | CacheInvalidator veri transferi modeli | VersaCoder.Crosscutting | net48;net10.0-windows | CacheInvalidatorTests | planli |
| N20-052 | `CacheInvalidatorMapper` | class | CacheInvalidator <-> DTO esleme | VersaCoder.Crosscutting | net48;net10.0-windows | CacheInvalidatorTests | planli |
| N20-053 | `CacheInvalidatorValidator` | class | CacheInvalidator dogrulama kurallari | VersaCoder.Crosscutting | net48;net10.0-windows | CacheInvalidatorTests | planli |
| N20-054 | `CacheInvalidatorFactory` | class | CacheInvalidator uretimi ve varsayilanlari | VersaCoder.Crosscutting | net48;net10.0-windows | CacheInvalidatorTests | planli |
| N20-055 | `CacheInvalidatorOptions` | record | CacheInvalidator yapilandirmasi | VersaCoder.Crosscutting | net48;net10.0-windows | CacheInvalidatorTests | planli |
| N20-056 | `CacheInvalidatorProvider` | class | CacheInvalidator saglayici uygulamasi | VersaCoder.Crosscutting | net48;net10.0-windows | CacheInvalidatorTests | planli |
| N20-057 | `CacheInvalidatorRegistry` | class | CacheInvalidator kayit/katalog yonetimi | VersaCoder.Crosscutting | net48;net10.0-windows | CacheInvalidatorTests | planli |
| N20-058 | `CacheInvalidatorConverter` | class | CacheInvalidator bicim donusumu | VersaCoder.Crosscutting | net48;net10.0-windows | CacheInvalidatorTests | planli |
| N20-059 | `CacheInvalidatorTests` | class | CacheInvalidator testleri (xUnit) | VersaCoder.Crosscutting | net48;net10.0-windows | - | planli |
| N20-060 | `CacheInvalidatorBuilder` | class | CacheInvalidator uretici (builder) | VersaCoder.Crosscutting | net48;net10.0-windows | CacheInvalidatorTests | planli |
| N20-061 | `ISlidingWindow` | interface | SlidingWindow sozlesmesi (port) | VersaCoder.Crosscutting | net48;net10.0-windows | SlidingWindowTests | planli |
| N20-062 | `SlidingWindow` | class | SlidingWindow cekirdek uygulamasi | VersaCoder.Crosscutting | net48;net10.0-windows | SlidingWindowTests | planli |
| N20-063 | `SlidingWindowDto` | record | SlidingWindow veri transferi modeli | VersaCoder.Crosscutting | net48;net10.0-windows | SlidingWindowTests | planli |
| N20-064 | `SlidingWindowMapper` | class | SlidingWindow <-> DTO esleme | VersaCoder.Crosscutting | net48;net10.0-windows | SlidingWindowTests | planli |
| N20-065 | `SlidingWindowValidator` | class | SlidingWindow dogrulama kurallari | VersaCoder.Crosscutting | net48;net10.0-windows | SlidingWindowTests | planli |
| N20-066 | `SlidingWindowFactory` | class | SlidingWindow uretimi ve varsayilanlari | VersaCoder.Crosscutting | net48;net10.0-windows | SlidingWindowTests | planli |
| N20-067 | `SlidingWindowOptions` | record | SlidingWindow yapilandirmasi | VersaCoder.Crosscutting | net48;net10.0-windows | SlidingWindowTests | planli |
| N20-068 | `SlidingWindowProvider` | class | SlidingWindow saglayici uygulamasi | VersaCoder.Crosscutting | net48;net10.0-windows | SlidingWindowTests | planli |
| N20-069 | `SlidingWindowRegistry` | class | SlidingWindow kayit/katalog yonetimi | VersaCoder.Crosscutting | net48;net10.0-windows | SlidingWindowTests | planli |
| N20-070 | `SlidingWindowConverter` | class | SlidingWindow bicim donusumu | VersaCoder.Crosscutting | net48;net10.0-windows | SlidingWindowTests | planli |
| N20-071 | `SlidingWindowTests` | class | SlidingWindow testleri (xUnit) | VersaCoder.Crosscutting | net48;net10.0-windows | - | planli |
| N20-072 | `SlidingWindowBuilder` | class | SlidingWindow uretici (builder) | VersaCoder.Crosscutting | net48;net10.0-windows | SlidingWindowTests | planli |
| N20-073 | `IStampedeGuard` | interface | StampedeGuard sozlesmesi (port) | VersaCoder.Crosscutting | net48;net10.0-windows | StampedeGuardTests | planli |
| N20-074 | `StampedeGuard` | class | StampedeGuard cekirdek uygulamasi | VersaCoder.Crosscutting | net48;net10.0-windows | StampedeGuardTests | planli |
| N20-075 | `StampedeGuardDto` | record | StampedeGuard veri transferi modeli | VersaCoder.Crosscutting | net48;net10.0-windows | StampedeGuardTests | planli |
| N20-076 | `StampedeGuardMapper` | class | StampedeGuard <-> DTO esleme | VersaCoder.Crosscutting | net48;net10.0-windows | StampedeGuardTests | planli |
| N20-077 | `StampedeGuardValidator` | class | StampedeGuard dogrulama kurallari | VersaCoder.Crosscutting | net48;net10.0-windows | StampedeGuardTests | planli |
| N20-078 | `StampedeGuardFactory` | class | StampedeGuard uretimi ve varsayilanlari | VersaCoder.Crosscutting | net48;net10.0-windows | StampedeGuardTests | planli |
| N20-079 | `StampedeGuardOptions` | record | StampedeGuard yapilandirmasi | VersaCoder.Crosscutting | net48;net10.0-windows | StampedeGuardTests | planli |
| N20-080 | `StampedeGuardProvider` | class | StampedeGuard saglayici uygulamasi | VersaCoder.Crosscutting | net48;net10.0-windows | StampedeGuardTests | planli |
| N20-081 | `StampedeGuardRegistry` | class | StampedeGuard kayit/katalog yonetimi | VersaCoder.Crosscutting | net48;net10.0-windows | StampedeGuardTests | planli |
| N20-082 | `StampedeGuardConverter` | class | StampedeGuard bicim donusumu | VersaCoder.Crosscutting | net48;net10.0-windows | StampedeGuardTests | planli |
| N20-083 | `StampedeGuardTests` | class | StampedeGuard testleri (xUnit) | VersaCoder.Crosscutting | net48;net10.0-windows | - | planli |
| N20-084 | `StampedeGuardBuilder` | class | StampedeGuard uretici (builder) | VersaCoder.Crosscutting | net48;net10.0-windows | StampedeGuardTests | planli |
| N20-085 | `ICacheStats` | interface | CacheStats sozlesmesi (port) | VersaCoder.Crosscutting | net48;net10.0-windows | CacheStatsTests | planli |
| N20-086 | `CacheStats` | class | CacheStats cekirdek uygulamasi | VersaCoder.Crosscutting | net48;net10.0-windows | CacheStatsTests | planli |
| N20-087 | `CacheStatsDto` | record | CacheStats veri transferi modeli | VersaCoder.Crosscutting | net48;net10.0-windows | CacheStatsTests | planli |
| N20-088 | `CacheStatsMapper` | class | CacheStats <-> DTO esleme | VersaCoder.Crosscutting | net48;net10.0-windows | CacheStatsTests | planli |
| N20-089 | `CacheStatsValidator` | class | CacheStats dogrulama kurallari | VersaCoder.Crosscutting | net48;net10.0-windows | CacheStatsTests | planli |
| N20-090 | `CacheStatsFactory` | class | CacheStats uretimi ve varsayilanlari | VersaCoder.Crosscutting | net48;net10.0-windows | CacheStatsTests | planli |
| N20-091 | `CacheStatsOptions` | record | CacheStats yapilandirmasi | VersaCoder.Crosscutting | net48;net10.0-windows | CacheStatsTests | planli |
| N20-092 | `CacheStatsProvider` | class | CacheStats saglayici uygulamasi | VersaCoder.Crosscutting | net48;net10.0-windows | CacheStatsTests | planli |
| N20-093 | `CacheStatsRegistry` | class | CacheStats kayit/katalog yonetimi | VersaCoder.Crosscutting | net48;net10.0-windows | CacheStatsTests | planli |
| N20-094 | `CacheStatsConverter` | class | CacheStats bicim donusumu | VersaCoder.Crosscutting | net48;net10.0-windows | CacheStatsTests | planli |
| N20-095 | `CacheStatsTests` | class | CacheStats testleri (xUnit) | VersaCoder.Crosscutting | net48;net10.0-windows | - | planli |
| N20-096 | `CacheStatsBuilder` | class | CacheStats uretici (builder) | VersaCoder.Crosscutting | net48;net10.0-windows | CacheStatsTests | planli |
| N20-097 | `IFileCache` | interface | FileCache sozlesmesi (port) | VersaCoder.Crosscutting | net48;net10.0-windows | FileCacheTests | planli |
| N20-098 | `FileCache` | class | FileCache cekirdek uygulamasi | VersaCoder.Crosscutting | net48;net10.0-windows | FileCacheTests | planli |
| N20-099 | `FileCacheDto` | record | FileCache veri transferi modeli | VersaCoder.Crosscutting | net48;net10.0-windows | FileCacheTests | planli |
| N20-100 | `FileCacheMapper` | class | FileCache <-> DTO esleme | VersaCoder.Crosscutting | net48;net10.0-windows | FileCacheTests | planli |
| N20-101 | `FileCacheValidator` | class | FileCache dogrulama kurallari | VersaCoder.Crosscutting | net48;net10.0-windows | FileCacheTests | planli |
| N20-102 | `FileCacheFactory` | class | FileCache uretimi ve varsayilanlari | VersaCoder.Crosscutting | net48;net10.0-windows | FileCacheTests | planli |
| N20-103 | `FileCacheOptions` | record | FileCache yapilandirmasi | VersaCoder.Crosscutting | net48;net10.0-windows | FileCacheTests | planli |
| N20-104 | `FileCacheProvider` | class | FileCache saglayici uygulamasi | VersaCoder.Crosscutting | net48;net10.0-windows | FileCacheTests | planli |
| N20-105 | `FileCacheRegistry` | class | FileCache kayit/katalog yonetimi | VersaCoder.Crosscutting | net48;net10.0-windows | FileCacheTests | planli |
| N20-106 | `FileCacheConverter` | class | FileCache bicim donusumu | VersaCoder.Crosscutting | net48;net10.0-windows | FileCacheTests | planli |
| N20-107 | `FileCacheTests` | class | FileCache testleri (xUnit) | VersaCoder.Crosscutting | net48;net10.0-windows | - | planli |
| N20-108 | `FileCacheBuilder` | class | FileCache uretici (builder) | VersaCoder.Crosscutting | net48;net10.0-windows | FileCacheTests | planli |
| N20-109 | `ICacheSerializer` | interface | CacheSerializer sozlesmesi (port) | VersaCoder.Crosscutting | net48;net10.0-windows | CacheSerializerTests | planli |
| N20-110 | `CacheSerializer` | class | CacheSerializer cekirdek uygulamasi | VersaCoder.Crosscutting | net48;net10.0-windows | CacheSerializerTests | planli |
| N20-111 | `CacheSerializerDto` | record | CacheSerializer veri transferi modeli | VersaCoder.Crosscutting | net48;net10.0-windows | CacheSerializerTests | planli |
| N20-112 | `CacheSerializerMapper` | class | CacheSerializer <-> DTO esleme | VersaCoder.Crosscutting | net48;net10.0-windows | CacheSerializerTests | planli |
| N20-113 | `CacheSerializerValidator` | class | CacheSerializer dogrulama kurallari | VersaCoder.Crosscutting | net48;net10.0-windows | CacheSerializerTests | planli |
| N20-114 | `CacheSerializerFactory` | class | CacheSerializer uretimi ve varsayilanlari | VersaCoder.Crosscutting | net48;net10.0-windows | CacheSerializerTests | planli |
| N20-115 | `CacheSerializerOptions` | record | CacheSerializer yapilandirmasi | VersaCoder.Crosscutting | net48;net10.0-windows | CacheSerializerTests | planli |
| N20-116 | `CacheSerializerProvider` | class | CacheSerializer saglayici uygulamasi | VersaCoder.Crosscutting | net48;net10.0-windows | CacheSerializerTests | planli |
| N20-117 | `CacheSerializerRegistry` | class | CacheSerializer kayit/katalog yonetimi | VersaCoder.Crosscutting | net48;net10.0-windows | CacheSerializerTests | planli |
| N20-118 | `CacheSerializerConverter` | class | CacheSerializer bicim donusumu | VersaCoder.Crosscutting | net48;net10.0-windows | CacheSerializerTests | planli |
| N20-119 | `CacheSerializerTests` | class | CacheSerializer testleri (xUnit) | VersaCoder.Crosscutting | net48;net10.0-windows | - | planli |
| N20-120 | `CacheSerializerBuilder` | class | CacheSerializer uretici (builder) | VersaCoder.Crosscutting | net48;net10.0-windows | CacheSerializerTests | planli |
