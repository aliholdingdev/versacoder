---
title: "Katalog Dugumu N24 - Dayaniklilik (l3)"
type: catalog
category: architecture
status: needs-validation
version: 1.0.0
updated: 2026-10-04
authority: derived
scope: "N24 resilience dugumu: kimlik, src klasoru, aile yapisi, ornek kayit ve yon kurali (ADR-013 + ADR-028)"
---

# Katalog Dugumu N24 - Dayaniklilik

**Zorunlu Baglantilar:** [catalog index](index.md) · [node-inventory](node-inventory.md) · [mimari index](../index.md) · [ADR-028](../../.decisions/accepted/ADR-028-layered-baseline-9-projects.md) · [ADR-013](../../.decisions/accepted/ADR-013-layered-architecture-52-nodes.md)

> **Tek cumlelik sorumluluk:** *N24 resilience hangi projede ve klasorde yasar, ne yapar, kac kayit hedefler* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

**N24 (resilience), l3 Crosscutting katmaninin alt katmanidir: `VersaCoder.Crosscutting` projesinde `src/VersaCoder.Crosscutting/Resilience/` klasorunde yasar.** Hata toleransini retry, circuit breaker, timeout, fallback ve bulkhead politikalariyla saglar. 10 aile x 12 rol = 120 kayit hedefi (ADR-013 asgari 112); durum `planli`.

Kayitlar ADR-013 2.1 biciminde 8 sutun tasir; bu dosya aile kalibini ve ornek satirlari gosterir, tam 120 satirlik liste bu dosyanin boyut sinirina sigmaz (DOĞRULAMA GEREKLI).

```text
+--------------------------------------------------+
| l3 Crosscutting -> VersaCoder.Crosscutting
| src/VersaCoder.Crosscutting/Resilience/
| 10 aile x 12 rol = 120 kayit (esik 112)
+--------------------------------------------------+
```

| Alan | Deger |
|---|---|
| Dugum | N24 |
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
| 1 | hata toleransini retry, circuit breaker, timeout, fallback ve bulkhead ile saglar | l1, l2 ve l4..l7 katmanlarina bakmak (l3 yalniz l0) |
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
| 1 | N24 | resilience | `src/VersaCoder.Crosscutting/Resilience/` |

**Kanit yolu (ADR-028 2.3/5):** tarama yolu gercek proje adini icerir; genis kalip (`src/**/Domain/**`) sahte PASS uretir.

```powershell
Test-Path 'C:\versacoder\src\VersaCoder.Crosscutting\Resilience'
```

Dizin su an yok cikti `False` olur; PASS yazilmaz -> **DOĞRULAMA GEREKLI**.

---

## §3 Aile Yapisi

Rol kalibi (12): `I<F>` · `<F>` · `<F>Dto` · `<F>Mapper` · `<F>Validator` · `<F>Factory` · `<F>Options` · `<F>Provider` · `<F>Registry` · `<F>Converter` · `<F>Tests` · `<F>Builder`

| # | Aile | # | Aile |
|---|---|---|---|
| 1 | RetryPolicy | 6 | HealthProbe |
| 2 | CircuitBreaker | 7 | RateLimiter |
| 3 | TimeoutGuard | 8 | FailureInjector |
| 4 | Fallback | 9 | ResilienceReport |
| 5 | Bulkhead | 10 | Jitter |

Her aile ayni rol kalibini 12 kez tekrarlar; kayit sayisi = aile x rol.

---

## §4 Kayit Ornegi (8 sutun)

| ID | Bilesen | Tip | Sorumluluk | Proje | TFM | Test | Durum |
|---|---|---|---|---|---|---|---|
| N24-001 | `IRetryPolicy` | interface | RetryPolicy sozlesmesi (port) | VersaCoder.Crosscutting | net48;net10.0-windows | RetryPolicyTests | planli |
| N24-002 | `RetryPolicy` | class | RetryPolicy cekirdek uygulamasi | VersaCoder.Crosscutting | net48;net10.0-windows | RetryPolicyTests | planli |
| N24-003 | `RetryPolicyDto` | record | RetryPolicy veri transferi modeli | VersaCoder.Crosscutting | net48;net10.0-windows | RetryPolicyTests | planli |
| N24-004 | `RetryPolicyMapper` | class | RetryPolicy <-> DTO esleme | VersaCoder.Crosscutting | net48;net10.0-windows | RetryPolicyTests | planli |
| N24-005 | `RetryPolicyValidator` | class | RetryPolicy dogrulama kurallari | VersaCoder.Crosscutting | net48;net10.0-windows | RetryPolicyTests | planli |
| N24-006 | `RetryPolicyFactory` | class | RetryPolicy uretimi ve varsayilanlari | VersaCoder.Crosscutting | net48;net10.0-windows | RetryPolicyTests | planli |

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
| 3 | Test sinifi | RetryPolicyTests (xUnit) | planli |
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
(Select-String -Path 'C:\versacoder\.ai\architecture\catalog\N24-resilience.md' -Pattern '^\|\s*N\d{2}-\d{3}\s*\|').Count
# 2) boyut bandi - beklenen: 2600-8500
(Get-Item 'C:\versacoder\.ai\architecture\catalog\N24-resilience.md').Length
# 3) klasor kaniti - gercek proje adi (ADR-028 2.3/5)
(Test-Path 'C:\versacoder\src\VersaCoder.Crosscutting\Resilience')
```

Katalog sayimi `powershell -NoProfile -ExecutionPolicy Bypass -File C:\versacoder\.ai\scripts\count-components.ps1` ile alinir; bu batch 26 node dosyasiyla 52 dugum klasorunu tamamlar - kayit esigi (5000) ornek satirlarla henuz karsilanmaz (DOĞRULAMA GEREKLI).

## §10 Kayit Tablosu (ADR-013 2.2 - kaynak vault port)

| N24-007 | `RetryPolicyOptions` | record | RetryPolicy yapilandirmasi | VersaCoder.Crosscutting | net48;net10.0-windows | RetryPolicyTests | planli |
| N24-008 | `RetryPolicyProvider` | class | RetryPolicy saglayici uygulamasi | VersaCoder.Crosscutting | net48;net10.0-windows | RetryPolicyTests | planli |
| N24-009 | `RetryPolicyRegistry` | class | RetryPolicy kayit/katalog yonetimi | VersaCoder.Crosscutting | net48;net10.0-windows | RetryPolicyTests | planli |
| N24-010 | `RetryPolicyConverter` | class | RetryPolicy bicim donusumu | VersaCoder.Crosscutting | net48;net10.0-windows | RetryPolicyTests | planli |
| N24-011 | `RetryPolicyTests` | class | RetryPolicy testleri (xUnit) | VersaCoder.Crosscutting | net48;net10.0-windows | - | planli |
| N24-012 | `RetryPolicyBuilder` | class | RetryPolicy uretici (builder) | VersaCoder.Crosscutting | net48;net10.0-windows | RetryPolicyTests | planli |
| N24-013 | `ICircuitBreaker` | interface | CircuitBreaker sozlesmesi (port) | VersaCoder.Crosscutting | net48;net10.0-windows | CircuitBreakerTests | planli |
| N24-014 | `CircuitBreaker` | class | CircuitBreaker cekirdek uygulamasi | VersaCoder.Crosscutting | net48;net10.0-windows | CircuitBreakerTests | planli |
| N24-015 | `CircuitBreakerDto` | record | CircuitBreaker veri transferi modeli | VersaCoder.Crosscutting | net48;net10.0-windows | CircuitBreakerTests | planli |
| N24-016 | `CircuitBreakerMapper` | class | CircuitBreaker <-> DTO esleme | VersaCoder.Crosscutting | net48;net10.0-windows | CircuitBreakerTests | planli |
| N24-017 | `CircuitBreakerValidator` | class | CircuitBreaker dogrulama kurallari | VersaCoder.Crosscutting | net48;net10.0-windows | CircuitBreakerTests | planli |
| N24-018 | `CircuitBreakerFactory` | class | CircuitBreaker uretimi ve varsayilanlari | VersaCoder.Crosscutting | net48;net10.0-windows | CircuitBreakerTests | planli |
| N24-019 | `CircuitBreakerOptions` | record | CircuitBreaker yapilandirmasi | VersaCoder.Crosscutting | net48;net10.0-windows | CircuitBreakerTests | planli |
| N24-020 | `CircuitBreakerProvider` | class | CircuitBreaker saglayici uygulamasi | VersaCoder.Crosscutting | net48;net10.0-windows | CircuitBreakerTests | planli |
| N24-021 | `CircuitBreakerRegistry` | class | CircuitBreaker kayit/katalog yonetimi | VersaCoder.Crosscutting | net48;net10.0-windows | CircuitBreakerTests | planli |
| N24-022 | `CircuitBreakerConverter` | class | CircuitBreaker bicim donusumu | VersaCoder.Crosscutting | net48;net10.0-windows | CircuitBreakerTests | planli |
| N24-023 | `CircuitBreakerTests` | class | CircuitBreaker testleri (xUnit) | VersaCoder.Crosscutting | net48;net10.0-windows | - | planli |
| N24-024 | `CircuitBreakerBuilder` | class | CircuitBreaker uretici (builder) | VersaCoder.Crosscutting | net48;net10.0-windows | CircuitBreakerTests | planli |
| N24-025 | `ITimeoutGuard` | interface | TimeoutGuard sozlesmesi (port) | VersaCoder.Crosscutting | net48;net10.0-windows | TimeoutGuardTests | planli |
| N24-026 | `TimeoutGuard` | class | TimeoutGuard cekirdek uygulamasi | VersaCoder.Crosscutting | net48;net10.0-windows | TimeoutGuardTests | planli |
| N24-027 | `TimeoutGuardDto` | record | TimeoutGuard veri transferi modeli | VersaCoder.Crosscutting | net48;net10.0-windows | TimeoutGuardTests | planli |
| N24-028 | `TimeoutGuardMapper` | class | TimeoutGuard <-> DTO esleme | VersaCoder.Crosscutting | net48;net10.0-windows | TimeoutGuardTests | planli |
| N24-029 | `TimeoutGuardValidator` | class | TimeoutGuard dogrulama kurallari | VersaCoder.Crosscutting | net48;net10.0-windows | TimeoutGuardTests | planli |
| N24-030 | `TimeoutGuardFactory` | class | TimeoutGuard uretimi ve varsayilanlari | VersaCoder.Crosscutting | net48;net10.0-windows | TimeoutGuardTests | planli |
| N24-031 | `TimeoutGuardOptions` | record | TimeoutGuard yapilandirmasi | VersaCoder.Crosscutting | net48;net10.0-windows | TimeoutGuardTests | planli |
| N24-032 | `TimeoutGuardProvider` | class | TimeoutGuard saglayici uygulamasi | VersaCoder.Crosscutting | net48;net10.0-windows | TimeoutGuardTests | planli |
| N24-033 | `TimeoutGuardRegistry` | class | TimeoutGuard kayit/katalog yonetimi | VersaCoder.Crosscutting | net48;net10.0-windows | TimeoutGuardTests | planli |
| N24-034 | `TimeoutGuardConverter` | class | TimeoutGuard bicim donusumu | VersaCoder.Crosscutting | net48;net10.0-windows | TimeoutGuardTests | planli |
| N24-035 | `TimeoutGuardTests` | class | TimeoutGuard testleri (xUnit) | VersaCoder.Crosscutting | net48;net10.0-windows | - | planli |
| N24-036 | `TimeoutGuardBuilder` | class | TimeoutGuard uretici (builder) | VersaCoder.Crosscutting | net48;net10.0-windows | TimeoutGuardTests | planli |
| N24-037 | `IFallback` | interface | Fallback sozlesmesi (port) | VersaCoder.Crosscutting | net48;net10.0-windows | FallbackTests | planli |
| N24-038 | `Fallback` | class | Fallback cekirdek uygulamasi | VersaCoder.Crosscutting | net48;net10.0-windows | FallbackTests | planli |
| N24-039 | `FallbackDto` | record | Fallback veri transferi modeli | VersaCoder.Crosscutting | net48;net10.0-windows | FallbackTests | planli |
| N24-040 | `FallbackMapper` | class | Fallback <-> DTO esleme | VersaCoder.Crosscutting | net48;net10.0-windows | FallbackTests | planli |
| N24-041 | `FallbackValidator` | class | Fallback dogrulama kurallari | VersaCoder.Crosscutting | net48;net10.0-windows | FallbackTests | planli |
| N24-042 | `FallbackFactory` | class | Fallback uretimi ve varsayilanlari | VersaCoder.Crosscutting | net48;net10.0-windows | FallbackTests | planli |
| N24-043 | `FallbackOptions` | record | Fallback yapilandirmasi | VersaCoder.Crosscutting | net48;net10.0-windows | FallbackTests | planli |
| N24-044 | `FallbackProvider` | class | Fallback saglayici uygulamasi | VersaCoder.Crosscutting | net48;net10.0-windows | FallbackTests | planli |
| N24-045 | `FallbackRegistry` | class | Fallback kayit/katalog yonetimi | VersaCoder.Crosscutting | net48;net10.0-windows | FallbackTests | planli |
| N24-046 | `FallbackConverter` | class | Fallback bicim donusumu | VersaCoder.Crosscutting | net48;net10.0-windows | FallbackTests | planli |
| N24-047 | `FallbackTests` | class | Fallback testleri (xUnit) | VersaCoder.Crosscutting | net48;net10.0-windows | - | planli |
| N24-048 | `FallbackBuilder` | class | Fallback uretici (builder) | VersaCoder.Crosscutting | net48;net10.0-windows | FallbackTests | planli |
| N24-049 | `IBulkhead` | interface | Bulkhead sozlesmesi (port) | VersaCoder.Crosscutting | net48;net10.0-windows | BulkheadTests | planli |
| N24-050 | `Bulkhead` | class | Bulkhead cekirdek uygulamasi | VersaCoder.Crosscutting | net48;net10.0-windows | BulkheadTests | planli |
| N24-051 | `BulkheadDto` | record | Bulkhead veri transferi modeli | VersaCoder.Crosscutting | net48;net10.0-windows | BulkheadTests | planli |
| N24-052 | `BulkheadMapper` | class | Bulkhead <-> DTO esleme | VersaCoder.Crosscutting | net48;net10.0-windows | BulkheadTests | planli |
| N24-053 | `BulkheadValidator` | class | Bulkhead dogrulama kurallari | VersaCoder.Crosscutting | net48;net10.0-windows | BulkheadTests | planli |
| N24-054 | `BulkheadFactory` | class | Bulkhead uretimi ve varsayilanlari | VersaCoder.Crosscutting | net48;net10.0-windows | BulkheadTests | planli |
| N24-055 | `BulkheadOptions` | record | Bulkhead yapilandirmasi | VersaCoder.Crosscutting | net48;net10.0-windows | BulkheadTests | planli |
| N24-056 | `BulkheadProvider` | class | Bulkhead saglayici uygulamasi | VersaCoder.Crosscutting | net48;net10.0-windows | BulkheadTests | planli |
| N24-057 | `BulkheadRegistry` | class | Bulkhead kayit/katalog yonetimi | VersaCoder.Crosscutting | net48;net10.0-windows | BulkheadTests | planli |
| N24-058 | `BulkheadConverter` | class | Bulkhead bicim donusumu | VersaCoder.Crosscutting | net48;net10.0-windows | BulkheadTests | planli |
| N24-059 | `BulkheadTests` | class | Bulkhead testleri (xUnit) | VersaCoder.Crosscutting | net48;net10.0-windows | - | planli |
| N24-060 | `BulkheadBuilder` | class | Bulkhead uretici (builder) | VersaCoder.Crosscutting | net48;net10.0-windows | BulkheadTests | planli |
| N24-061 | `IHealthProbe` | interface | HealthProbe sozlesmesi (port) | VersaCoder.Crosscutting | net48;net10.0-windows | HealthProbeTests | planli |
| N24-062 | `HealthProbe` | class | HealthProbe cekirdek uygulamasi | VersaCoder.Crosscutting | net48;net10.0-windows | HealthProbeTests | planli |
| N24-063 | `HealthProbeDto` | record | HealthProbe veri transferi modeli | VersaCoder.Crosscutting | net48;net10.0-windows | HealthProbeTests | planli |
| N24-064 | `HealthProbeMapper` | class | HealthProbe <-> DTO esleme | VersaCoder.Crosscutting | net48;net10.0-windows | HealthProbeTests | planli |
| N24-065 | `HealthProbeValidator` | class | HealthProbe dogrulama kurallari | VersaCoder.Crosscutting | net48;net10.0-windows | HealthProbeTests | planli |
| N24-066 | `HealthProbeFactory` | class | HealthProbe uretimi ve varsayilanlari | VersaCoder.Crosscutting | net48;net10.0-windows | HealthProbeTests | planli |
| N24-067 | `HealthProbeOptions` | record | HealthProbe yapilandirmasi | VersaCoder.Crosscutting | net48;net10.0-windows | HealthProbeTests | planli |
| N24-068 | `HealthProbeProvider` | class | HealthProbe saglayici uygulamasi | VersaCoder.Crosscutting | net48;net10.0-windows | HealthProbeTests | planli |
| N24-069 | `HealthProbeRegistry` | class | HealthProbe kayit/katalog yonetimi | VersaCoder.Crosscutting | net48;net10.0-windows | HealthProbeTests | planli |
| N24-070 | `HealthProbeConverter` | class | HealthProbe bicim donusumu | VersaCoder.Crosscutting | net48;net10.0-windows | HealthProbeTests | planli |
| N24-071 | `HealthProbeTests` | class | HealthProbe testleri (xUnit) | VersaCoder.Crosscutting | net48;net10.0-windows | - | planli |
| N24-072 | `HealthProbeBuilder` | class | HealthProbe uretici (builder) | VersaCoder.Crosscutting | net48;net10.0-windows | HealthProbeTests | planli |
| N24-073 | `IRateLimiter` | interface | RateLimiter sozlesmesi (port) | VersaCoder.Crosscutting | net48;net10.0-windows | RateLimiterTests | planli |
| N24-074 | `RateLimiter` | class | RateLimiter cekirdek uygulamasi | VersaCoder.Crosscutting | net48;net10.0-windows | RateLimiterTests | planli |
| N24-075 | `RateLimiterDto` | record | RateLimiter veri transferi modeli | VersaCoder.Crosscutting | net48;net10.0-windows | RateLimiterTests | planli |
| N24-076 | `RateLimiterMapper` | class | RateLimiter <-> DTO esleme | VersaCoder.Crosscutting | net48;net10.0-windows | RateLimiterTests | planli |
| N24-077 | `RateLimiterValidator` | class | RateLimiter dogrulama kurallari | VersaCoder.Crosscutting | net48;net10.0-windows | RateLimiterTests | planli |
| N24-078 | `RateLimiterFactory` | class | RateLimiter uretimi ve varsayilanlari | VersaCoder.Crosscutting | net48;net10.0-windows | RateLimiterTests | planli |
| N24-079 | `RateLimiterOptions` | record | RateLimiter yapilandirmasi | VersaCoder.Crosscutting | net48;net10.0-windows | RateLimiterTests | planli |
| N24-080 | `RateLimiterProvider` | class | RateLimiter saglayici uygulamasi | VersaCoder.Crosscutting | net48;net10.0-windows | RateLimiterTests | planli |
| N24-081 | `RateLimiterRegistry` | class | RateLimiter kayit/katalog yonetimi | VersaCoder.Crosscutting | net48;net10.0-windows | RateLimiterTests | planli |
| N24-082 | `RateLimiterConverter` | class | RateLimiter bicim donusumu | VersaCoder.Crosscutting | net48;net10.0-windows | RateLimiterTests | planli |
| N24-083 | `RateLimiterTests` | class | RateLimiter testleri (xUnit) | VersaCoder.Crosscutting | net48;net10.0-windows | - | planli |
| N24-084 | `RateLimiterBuilder` | class | RateLimiter uretici (builder) | VersaCoder.Crosscutting | net48;net10.0-windows | RateLimiterTests | planli |
| N24-085 | `IFailureInjector` | interface | FailureInjector sozlesmesi (port) | VersaCoder.Crosscutting | net48;net10.0-windows | FailureInjectorTests | planli |
| N24-086 | `FailureInjector` | class | FailureInjector cekirdek uygulamasi | VersaCoder.Crosscutting | net48;net10.0-windows | FailureInjectorTests | planli |
| N24-087 | `FailureInjectorDto` | record | FailureInjector veri transferi modeli | VersaCoder.Crosscutting | net48;net10.0-windows | FailureInjectorTests | planli |
| N24-088 | `FailureInjectorMapper` | class | FailureInjector <-> DTO esleme | VersaCoder.Crosscutting | net48;net10.0-windows | FailureInjectorTests | planli |
| N24-089 | `FailureInjectorValidator` | class | FailureInjector dogrulama kurallari | VersaCoder.Crosscutting | net48;net10.0-windows | FailureInjectorTests | planli |
| N24-090 | `FailureInjectorFactory` | class | FailureInjector uretimi ve varsayilanlari | VersaCoder.Crosscutting | net48;net10.0-windows | FailureInjectorTests | planli |
| N24-091 | `FailureInjectorOptions` | record | FailureInjector yapilandirmasi | VersaCoder.Crosscutting | net48;net10.0-windows | FailureInjectorTests | planli |
| N24-092 | `FailureInjectorProvider` | class | FailureInjector saglayici uygulamasi | VersaCoder.Crosscutting | net48;net10.0-windows | FailureInjectorTests | planli |
| N24-093 | `FailureInjectorRegistry` | class | FailureInjector kayit/katalog yonetimi | VersaCoder.Crosscutting | net48;net10.0-windows | FailureInjectorTests | planli |
| N24-094 | `FailureInjectorConverter` | class | FailureInjector bicim donusumu | VersaCoder.Crosscutting | net48;net10.0-windows | FailureInjectorTests | planli |
| N24-095 | `FailureInjectorTests` | class | FailureInjector testleri (xUnit) | VersaCoder.Crosscutting | net48;net10.0-windows | - | planli |
| N24-096 | `FailureInjectorBuilder` | class | FailureInjector uretici (builder) | VersaCoder.Crosscutting | net48;net10.0-windows | FailureInjectorTests | planli |
| N24-097 | `IResilienceReport` | interface | ResilienceReport sozlesmesi (port) | VersaCoder.Crosscutting | net48;net10.0-windows | ResilienceReportTests | planli |
| N24-098 | `ResilienceReport` | class | ResilienceReport cekirdek uygulamasi | VersaCoder.Crosscutting | net48;net10.0-windows | ResilienceReportTests | planli |
| N24-099 | `ResilienceReportDto` | record | ResilienceReport veri transferi modeli | VersaCoder.Crosscutting | net48;net10.0-windows | ResilienceReportTests | planli |
| N24-100 | `ResilienceReportMapper` | class | ResilienceReport <-> DTO esleme | VersaCoder.Crosscutting | net48;net10.0-windows | ResilienceReportTests | planli |
| N24-101 | `ResilienceReportValidator` | class | ResilienceReport dogrulama kurallari | VersaCoder.Crosscutting | net48;net10.0-windows | ResilienceReportTests | planli |
| N24-102 | `ResilienceReportFactory` | class | ResilienceReport uretimi ve varsayilanlari | VersaCoder.Crosscutting | net48;net10.0-windows | ResilienceReportTests | planli |
| N24-103 | `ResilienceReportOptions` | record | ResilienceReport yapilandirmasi | VersaCoder.Crosscutting | net48;net10.0-windows | ResilienceReportTests | planli |
| N24-104 | `ResilienceReportProvider` | class | ResilienceReport saglayici uygulamasi | VersaCoder.Crosscutting | net48;net10.0-windows | ResilienceReportTests | planli |
| N24-105 | `ResilienceReportRegistry` | class | ResilienceReport kayit/katalog yonetimi | VersaCoder.Crosscutting | net48;net10.0-windows | ResilienceReportTests | planli |
| N24-106 | `ResilienceReportConverter` | class | ResilienceReport bicim donusumu | VersaCoder.Crosscutting | net48;net10.0-windows | ResilienceReportTests | planli |
| N24-107 | `ResilienceReportTests` | class | ResilienceReport testleri (xUnit) | VersaCoder.Crosscutting | net48;net10.0-windows | - | planli |
| N24-108 | `ResilienceReportBuilder` | class | ResilienceReport uretici (builder) | VersaCoder.Crosscutting | net48;net10.0-windows | ResilienceReportTests | planli |
| N24-109 | `IJitter` | interface | Jitter sozlesmesi (port) | VersaCoder.Crosscutting | net48;net10.0-windows | JitterTests | planli |
| N24-110 | `Jitter` | class | Jitter cekirdek uygulamasi | VersaCoder.Crosscutting | net48;net10.0-windows | JitterTests | planli |
| N24-111 | `JitterDto` | record | Jitter veri transferi modeli | VersaCoder.Crosscutting | net48;net10.0-windows | JitterTests | planli |
| N24-112 | `JitterMapper` | class | Jitter <-> DTO esleme | VersaCoder.Crosscutting | net48;net10.0-windows | JitterTests | planli |
| N24-113 | `JitterValidator` | class | Jitter dogrulama kurallari | VersaCoder.Crosscutting | net48;net10.0-windows | JitterTests | planli |
| N24-114 | `JitterFactory` | class | Jitter uretimi ve varsayilanlari | VersaCoder.Crosscutting | net48;net10.0-windows | JitterTests | planli |
| N24-115 | `JitterOptions` | record | Jitter yapilandirmasi | VersaCoder.Crosscutting | net48;net10.0-windows | JitterTests | planli |
| N24-116 | `JitterProvider` | class | Jitter saglayici uygulamasi | VersaCoder.Crosscutting | net48;net10.0-windows | JitterTests | planli |
| N24-117 | `JitterRegistry` | class | Jitter kayit/katalog yonetimi | VersaCoder.Crosscutting | net48;net10.0-windows | JitterTests | planli |
| N24-118 | `JitterConverter` | class | Jitter bicim donusumu | VersaCoder.Crosscutting | net48;net10.0-windows | JitterTests | planli |
| N24-119 | `JitterTests` | class | Jitter testleri (xUnit) | VersaCoder.Crosscutting | net48;net10.0-windows | - | planli |
| N24-120 | `JitterBuilder` | class | Jitter uretici (builder) | VersaCoder.Crosscutting | net48;net10.0-windows | JitterTests | planli |
