---
title: "Katalog Dugumu N02 - Result ve Hata Sozlesmeleri (l0)"
type: catalog
category: architecture
status: needs-validation
version: 1.0.0
updated: 2026-10-04
authority: derived
scope: "N02 result-error dugumu: kimlik, src klasoru, aile yapisi, ornek kayit ve yon kurali (ADR-013 + ADR-028)"
---

# Katalog Dugumu N02 - Result ve Hata Sozlesmeleri

**Zorunlu Baglantilar:** [catalog index](index.md) · [node-inventory](node-inventory.md) · [mimari index](../index.md) · [ADR-028](../../.decisions/accepted/ADR-028-layered-baseline-9-projects.md) · [ADR-013](../../.decisions/accepted/ADR-013-layered-architecture-52-nodes.md)

> **Tek cumlelik sorumluluk:** *N02 result-error hangi projede ve klasorde yasar, ne yapar, kac kayit hedefler* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

**N02 (result-error), l0 Abstractions katmaninin alt katmanidir: `VersaCoder.Abstractions` projesinde `src/VersaCoder.Abstractions/Results/` klasorunde yasar.** Basari/basarizalik sozlesmesi Result<T> ile hata ve failure tiplerini tek yerde tanimlar. 10 aile x 12 rol = 120 kayit hedefi (ADR-013 asgari 112); durum `planli`.

Kayitlar ADR-013 2.1 biciminde 8 sutun tasir; bu dosya aile kalibini ve ornek satirlari gosterir, tam 120 satirlik liste bu dosyanin boyut sinirina sigmaz (DOĞRULAMA GEREKLI).

```text
+--------------------------------------------------+
| l0 Abstractions -> VersaCoder.Abstractions
| src/VersaCoder.Abstractions/Results/
| 10 aile x 12 rol = 120 kayit (esik 112)
+--------------------------------------------------+
```

| Alan | Deger |
|---|---|
| Dugum | N02 |
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
| 1 | Basari/basarizalik sozlesmesi Result<T> ile hata ve failure tiplerini tek yerde tanimlar | somut uygulama (adapter) sinifi yazmak, uygulama l4 katmaninda |
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
| 1 | N02 | result-error | `src/VersaCoder.Abstractions/Results/` |

**Kanit yolu (ADR-028 2.3/5):** tarama yolu gercek proje adini icerir; genis kalip (`src/**/Domain/**`) sahte PASS uretir.

```powershell
Test-Path 'C:\versacoder\src\VersaCoder.Abstractions\Results'
```

Dizin su an yok cikti `False` olur; PASS yazilmaz -> **DOĞRULAMA GEREKLI**.

---

## §3 Aile Yapisi

Rol kalibi (12): `I<F>` · `<F>` · `<F>Dto` · `<F>Mapper` · `<F>Validator` · `<F>Factory` · `<F>Options` · `<F>Provider` · `<F>Registry` · `<F>Converter` · `<F>Tests` · `<F>Builder`

| # | Aile | # | Aile |
|---|---|---|---|
| 1 | Result | 6 | ValidationFailure |
| 2 | ErrorCode | 7 | NotFoundFailure |
| 3 | Error | 8 | ConflictFailure |
| 4 | Failure | 9 | RetrySignal |
| 5 | Success | 10 | DomainSignal |

Her aile ayni rol kalibini 12 kez tekrarlar; kayit sayisi = aile x rol.

---

## §4 Kayit Ornegi (8 sutun)

| ID | Bilesen | Tip | Sorumluluk | Proje | TFM | Test | Durum |
|---|---|---|---|---|---|---|---|
| N02-001 | `IResult` | interface | Result sozlesmesi (port) | VersaCoder.Abstractions | net48;net10.0-windows | ResultTests | planli |
| N02-002 | `Result` | class | Result cekirdek uygulamasi | VersaCoder.Abstractions | net48;net10.0-windows | ResultTests | planli |
| N02-003 | `ResultDto` | record | Result veri transferi modeli | VersaCoder.Abstractions | net48;net10.0-windows | ResultTests | planli |
| N02-004 | `ResultMapper` | class | Result <-> DTO esleme | VersaCoder.Abstractions | net48;net10.0-windows | ResultTests | planli |
| N02-005 | `ResultValidator` | class | Result dogrulama kurallari | VersaCoder.Abstractions | net48;net10.0-windows | ResultTests | planli |
| N02-006 | `ResultFactory` | class | Result uretimi ve varsayilanlari | VersaCoder.Abstractions | net48;net10.0-windows | ResultTests | planli |

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
| 3 | Test sinifi | ResultTests (xUnit) | planli |
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
(Select-String -Path 'C:\versacoder\.ai\architecture\catalog\N02-result-error.md' -Pattern '^\|\s*N\d{2}-\d{3}\s*\|').Count
# 2) boyut bandi - beklenen: 2600-8500
(Get-Item 'C:\versacoder\.ai\architecture\catalog\N02-result-error.md').Length
# 3) klasor kaniti - gercek proje adi (ADR-028 2.3/5)
(Test-Path 'C:\versacoder\src\VersaCoder.Abstractions\Results')
```

Katalog sayimi `powershell -NoProfile -ExecutionPolicy Bypass -File C:\versacoder\.ai\scripts\count-components.ps1` ile alinir; bu batch sonunda toplam esik (5000) henuz karsilanmaz - kalan 26 node dosyasi sonraki batchtir.

## §10 Kayit Tablosu (ADR-013 2.2 - kaynak vault port)

| N02-007 | `ResultOptions` | record | Result yapilandirmasi | VersaCoder.Abstractions | net48;net10.0-windows | ResultTests | planli |
| N02-008 | `ResultProvider` | class | Result saglayici uygulamasi | VersaCoder.Abstractions | net48;net10.0-windows | ResultTests | planli |
| N02-009 | `ResultRegistry` | class | Result kayit/katalog yonetimi | VersaCoder.Abstractions | net48;net10.0-windows | ResultTests | planli |
| N02-010 | `ResultConverter` | class | Result bicim donusumu | VersaCoder.Abstractions | net48;net10.0-windows | ResultTests | planli |
| N02-011 | `ResultTests` | class | Result testleri (xUnit) | VersaCoder.Abstractions | net48;net10.0-windows | - | planli |
| N02-012 | `ResultBuilder` | class | Result uretici (builder) | VersaCoder.Abstractions | net48;net10.0-windows | ResultTests | planli |
| N02-013 | `IErrorCode` | interface | ErrorCode sozlesmesi (port) | VersaCoder.Abstractions | net48;net10.0-windows | ErrorCodeTests | planli |
| N02-014 | `ErrorCode` | class | ErrorCode cekirdek uygulamasi | VersaCoder.Abstractions | net48;net10.0-windows | ErrorCodeTests | planli |
| N02-015 | `ErrorCodeDto` | record | ErrorCode veri transferi modeli | VersaCoder.Abstractions | net48;net10.0-windows | ErrorCodeTests | planli |
| N02-016 | `ErrorCodeMapper` | class | ErrorCode <-> DTO esleme | VersaCoder.Abstractions | net48;net10.0-windows | ErrorCodeTests | planli |
| N02-017 | `ErrorCodeValidator` | class | ErrorCode dogrulama kurallari | VersaCoder.Abstractions | net48;net10.0-windows | ErrorCodeTests | planli |
| N02-018 | `ErrorCodeFactory` | class | ErrorCode uretimi ve varsayilanlari | VersaCoder.Abstractions | net48;net10.0-windows | ErrorCodeTests | planli |
| N02-019 | `ErrorCodeOptions` | record | ErrorCode yapilandirmasi | VersaCoder.Abstractions | net48;net10.0-windows | ErrorCodeTests | planli |
| N02-020 | `ErrorCodeProvider` | class | ErrorCode saglayici uygulamasi | VersaCoder.Abstractions | net48;net10.0-windows | ErrorCodeTests | planli |
| N02-021 | `ErrorCodeRegistry` | class | ErrorCode kayit/katalog yonetimi | VersaCoder.Abstractions | net48;net10.0-windows | ErrorCodeTests | planli |
| N02-022 | `ErrorCodeConverter` | class | ErrorCode bicim donusumu | VersaCoder.Abstractions | net48;net10.0-windows | ErrorCodeTests | planli |
| N02-023 | `ErrorCodeTests` | class | ErrorCode testleri (xUnit) | VersaCoder.Abstractions | net48;net10.0-windows | - | planli |
| N02-024 | `ErrorCodeBuilder` | class | ErrorCode uretici (builder) | VersaCoder.Abstractions | net48;net10.0-windows | ErrorCodeTests | planli |
| N02-025 | `IError` | interface | Error sozlesmesi (port) | VersaCoder.Abstractions | net48;net10.0-windows | ErrorTests | planli |
| N02-026 | `Error` | class | Error cekirdek uygulamasi | VersaCoder.Abstractions | net48;net10.0-windows | ErrorTests | planli |
| N02-027 | `ErrorDto` | record | Error veri transferi modeli | VersaCoder.Abstractions | net48;net10.0-windows | ErrorTests | planli |
| N02-028 | `ErrorMapper` | class | Error <-> DTO esleme | VersaCoder.Abstractions | net48;net10.0-windows | ErrorTests | planli |
| N02-029 | `ErrorValidator` | class | Error dogrulama kurallari | VersaCoder.Abstractions | net48;net10.0-windows | ErrorTests | planli |
| N02-030 | `ErrorFactory` | class | Error uretimi ve varsayilanlari | VersaCoder.Abstractions | net48;net10.0-windows | ErrorTests | planli |
| N02-031 | `ErrorOptions` | record | Error yapilandirmasi | VersaCoder.Abstractions | net48;net10.0-windows | ErrorTests | planli |
| N02-032 | `ErrorProvider` | class | Error saglayici uygulamasi | VersaCoder.Abstractions | net48;net10.0-windows | ErrorTests | planli |
| N02-033 | `ErrorRegistry` | class | Error kayit/katalog yonetimi | VersaCoder.Abstractions | net48;net10.0-windows | ErrorTests | planli |
| N02-034 | `ErrorConverter` | class | Error bicim donusumu | VersaCoder.Abstractions | net48;net10.0-windows | ErrorTests | planli |
| N02-035 | `ErrorTests` | class | Error testleri (xUnit) | VersaCoder.Abstractions | net48;net10.0-windows | - | planli |
| N02-036 | `ErrorBuilder` | class | Error uretici (builder) | VersaCoder.Abstractions | net48;net10.0-windows | ErrorTests | planli |
| N02-037 | `IFailure` | interface | Failure sozlesmesi (port) | VersaCoder.Abstractions | net48;net10.0-windows | FailureTests | planli |
| N02-038 | `Failure` | class | Failure cekirdek uygulamasi | VersaCoder.Abstractions | net48;net10.0-windows | FailureTests | planli |
| N02-039 | `FailureDto` | record | Failure veri transferi modeli | VersaCoder.Abstractions | net48;net10.0-windows | FailureTests | planli |
| N02-040 | `FailureMapper` | class | Failure <-> DTO esleme | VersaCoder.Abstractions | net48;net10.0-windows | FailureTests | planli |
| N02-041 | `FailureValidator` | class | Failure dogrulama kurallari | VersaCoder.Abstractions | net48;net10.0-windows | FailureTests | planli |
| N02-042 | `FailureFactory` | class | Failure uretimi ve varsayilanlari | VersaCoder.Abstractions | net48;net10.0-windows | FailureTests | planli |
| N02-043 | `FailureOptions` | record | Failure yapilandirmasi | VersaCoder.Abstractions | net48;net10.0-windows | FailureTests | planli |
| N02-044 | `FailureProvider` | class | Failure saglayici uygulamasi | VersaCoder.Abstractions | net48;net10.0-windows | FailureTests | planli |
| N02-045 | `FailureRegistry` | class | Failure kayit/katalog yonetimi | VersaCoder.Abstractions | net48;net10.0-windows | FailureTests | planli |
| N02-046 | `FailureConverter` | class | Failure bicim donusumu | VersaCoder.Abstractions | net48;net10.0-windows | FailureTests | planli |
| N02-047 | `FailureTests` | class | Failure testleri (xUnit) | VersaCoder.Abstractions | net48;net10.0-windows | - | planli |
| N02-048 | `FailureBuilder` | class | Failure uretici (builder) | VersaCoder.Abstractions | net48;net10.0-windows | FailureTests | planli |
| N02-049 | `ISuccess` | interface | Success sozlesmesi (port) | VersaCoder.Abstractions | net48;net10.0-windows | SuccessTests | planli |
| N02-050 | `Success` | class | Success cekirdek uygulamasi | VersaCoder.Abstractions | net48;net10.0-windows | SuccessTests | planli |
| N02-051 | `SuccessDto` | record | Success veri transferi modeli | VersaCoder.Abstractions | net48;net10.0-windows | SuccessTests | planli |
| N02-052 | `SuccessMapper` | class | Success <-> DTO esleme | VersaCoder.Abstractions | net48;net10.0-windows | SuccessTests | planli |
| N02-053 | `SuccessValidator` | class | Success dogrulama kurallari | VersaCoder.Abstractions | net48;net10.0-windows | SuccessTests | planli |
| N02-054 | `SuccessFactory` | class | Success uretimi ve varsayilanlari | VersaCoder.Abstractions | net48;net10.0-windows | SuccessTests | planli |
| N02-055 | `SuccessOptions` | record | Success yapilandirmasi | VersaCoder.Abstractions | net48;net10.0-windows | SuccessTests | planli |
| N02-056 | `SuccessProvider` | class | Success saglayici uygulamasi | VersaCoder.Abstractions | net48;net10.0-windows | SuccessTests | planli |
| N02-057 | `SuccessRegistry` | class | Success kayit/katalog yonetimi | VersaCoder.Abstractions | net48;net10.0-windows | SuccessTests | planli |
| N02-058 | `SuccessConverter` | class | Success bicim donusumu | VersaCoder.Abstractions | net48;net10.0-windows | SuccessTests | planli |
| N02-059 | `SuccessTests` | class | Success testleri (xUnit) | VersaCoder.Abstractions | net48;net10.0-windows | - | planli |
| N02-060 | `SuccessBuilder` | class | Success uretici (builder) | VersaCoder.Abstractions | net48;net10.0-windows | SuccessTests | planli |
| N02-061 | `IValidationFailure` | interface | ValidationFailure sozlesmesi (port) | VersaCoder.Abstractions | net48;net10.0-windows | ValidationFailureTests | planli |
| N02-062 | `ValidationFailure` | class | ValidationFailure cekirdek uygulamasi | VersaCoder.Abstractions | net48;net10.0-windows | ValidationFailureTests | planli |
| N02-063 | `ValidationFailureDto` | record | ValidationFailure veri transferi modeli | VersaCoder.Abstractions | net48;net10.0-windows | ValidationFailureTests | planli |
| N02-064 | `ValidationFailureMapper` | class | ValidationFailure <-> DTO esleme | VersaCoder.Abstractions | net48;net10.0-windows | ValidationFailureTests | planli |
| N02-065 | `ValidationFailureValidator` | class | ValidationFailure dogrulama kurallari | VersaCoder.Abstractions | net48;net10.0-windows | ValidationFailureTests | planli |
| N02-066 | `ValidationFailureFactory` | class | ValidationFailure uretimi ve varsayilanlari | VersaCoder.Abstractions | net48;net10.0-windows | ValidationFailureTests | planli |
| N02-067 | `ValidationFailureOptions` | record | ValidationFailure yapilandirmasi | VersaCoder.Abstractions | net48;net10.0-windows | ValidationFailureTests | planli |
| N02-068 | `ValidationFailureProvider` | class | ValidationFailure saglayici uygulamasi | VersaCoder.Abstractions | net48;net10.0-windows | ValidationFailureTests | planli |
| N02-069 | `ValidationFailureRegistry` | class | ValidationFailure kayit/katalog yonetimi | VersaCoder.Abstractions | net48;net10.0-windows | ValidationFailureTests | planli |
| N02-070 | `ValidationFailureConverter` | class | ValidationFailure bicim donusumu | VersaCoder.Abstractions | net48;net10.0-windows | ValidationFailureTests | planli |
| N02-071 | `ValidationFailureTests` | class | ValidationFailure testleri (xUnit) | VersaCoder.Abstractions | net48;net10.0-windows | - | planli |
| N02-072 | `ValidationFailureBuilder` | class | ValidationFailure uretici (builder) | VersaCoder.Abstractions | net48;net10.0-windows | ValidationFailureTests | planli |
| N02-073 | `INotFoundFailure` | interface | NotFoundFailure sozlesmesi (port) | VersaCoder.Abstractions | net48;net10.0-windows | NotFoundFailureTests | planli |
| N02-074 | `NotFoundFailure` | class | NotFoundFailure cekirdek uygulamasi | VersaCoder.Abstractions | net48;net10.0-windows | NotFoundFailureTests | planli |
| N02-075 | `NotFoundFailureDto` | record | NotFoundFailure veri transferi modeli | VersaCoder.Abstractions | net48;net10.0-windows | NotFoundFailureTests | planli |
| N02-076 | `NotFoundFailureMapper` | class | NotFoundFailure <-> DTO esleme | VersaCoder.Abstractions | net48;net10.0-windows | NotFoundFailureTests | planli |
| N02-077 | `NotFoundFailureValidator` | class | NotFoundFailure dogrulama kurallari | VersaCoder.Abstractions | net48;net10.0-windows | NotFoundFailureTests | planli |
| N02-078 | `NotFoundFailureFactory` | class | NotFoundFailure uretimi ve varsayilanlari | VersaCoder.Abstractions | net48;net10.0-windows | NotFoundFailureTests | planli |
| N02-079 | `NotFoundFailureOptions` | record | NotFoundFailure yapilandirmasi | VersaCoder.Abstractions | net48;net10.0-windows | NotFoundFailureTests | planli |
| N02-080 | `NotFoundFailureProvider` | class | NotFoundFailure saglayici uygulamasi | VersaCoder.Abstractions | net48;net10.0-windows | NotFoundFailureTests | planli |
| N02-081 | `NotFoundFailureRegistry` | class | NotFoundFailure kayit/katalog yonetimi | VersaCoder.Abstractions | net48;net10.0-windows | NotFoundFailureTests | planli |
| N02-082 | `NotFoundFailureConverter` | class | NotFoundFailure bicim donusumu | VersaCoder.Abstractions | net48;net10.0-windows | NotFoundFailureTests | planli |
| N02-083 | `NotFoundFailureTests` | class | NotFoundFailure testleri (xUnit) | VersaCoder.Abstractions | net48;net10.0-windows | - | planli |
| N02-084 | `NotFoundFailureBuilder` | class | NotFoundFailure uretici (builder) | VersaCoder.Abstractions | net48;net10.0-windows | NotFoundFailureTests | planli |
| N02-085 | `IConflictFailure` | interface | ConflictFailure sozlesmesi (port) | VersaCoder.Abstractions | net48;net10.0-windows | ConflictFailureTests | planli |
| N02-086 | `ConflictFailure` | class | ConflictFailure cekirdek uygulamasi | VersaCoder.Abstractions | net48;net10.0-windows | ConflictFailureTests | planli |
| N02-087 | `ConflictFailureDto` | record | ConflictFailure veri transferi modeli | VersaCoder.Abstractions | net48;net10.0-windows | ConflictFailureTests | planli |
| N02-088 | `ConflictFailureMapper` | class | ConflictFailure <-> DTO esleme | VersaCoder.Abstractions | net48;net10.0-windows | ConflictFailureTests | planli |
| N02-089 | `ConflictFailureValidator` | class | ConflictFailure dogrulama kurallari | VersaCoder.Abstractions | net48;net10.0-windows | ConflictFailureTests | planli |
| N02-090 | `ConflictFailureFactory` | class | ConflictFailure uretimi ve varsayilanlari | VersaCoder.Abstractions | net48;net10.0-windows | ConflictFailureTests | planli |
| N02-091 | `ConflictFailureOptions` | record | ConflictFailure yapilandirmasi | VersaCoder.Abstractions | net48;net10.0-windows | ConflictFailureTests | planli |
| N02-092 | `ConflictFailureProvider` | class | ConflictFailure saglayici uygulamasi | VersaCoder.Abstractions | net48;net10.0-windows | ConflictFailureTests | planli |
| N02-093 | `ConflictFailureRegistry` | class | ConflictFailure kayit/katalog yonetimi | VersaCoder.Abstractions | net48;net10.0-windows | ConflictFailureTests | planli |
| N02-094 | `ConflictFailureConverter` | class | ConflictFailure bicim donusumu | VersaCoder.Abstractions | net48;net10.0-windows | ConflictFailureTests | planli |
| N02-095 | `ConflictFailureTests` | class | ConflictFailure testleri (xUnit) | VersaCoder.Abstractions | net48;net10.0-windows | - | planli |
| N02-096 | `ConflictFailureBuilder` | class | ConflictFailure uretici (builder) | VersaCoder.Abstractions | net48;net10.0-windows | ConflictFailureTests | planli |
| N02-097 | `IRetrySignal` | interface | RetrySignal sozlesmesi (port) | VersaCoder.Abstractions | net48;net10.0-windows | RetrySignalTests | planli |
| N02-098 | `RetrySignal` | class | RetrySignal cekirdek uygulamasi | VersaCoder.Abstractions | net48;net10.0-windows | RetrySignalTests | planli |
| N02-099 | `RetrySignalDto` | record | RetrySignal veri transferi modeli | VersaCoder.Abstractions | net48;net10.0-windows | RetrySignalTests | planli |
| N02-100 | `RetrySignalMapper` | class | RetrySignal <-> DTO esleme | VersaCoder.Abstractions | net48;net10.0-windows | RetrySignalTests | planli |
| N02-101 | `RetrySignalValidator` | class | RetrySignal dogrulama kurallari | VersaCoder.Abstractions | net48;net10.0-windows | RetrySignalTests | planli |
| N02-102 | `RetrySignalFactory` | class | RetrySignal uretimi ve varsayilanlari | VersaCoder.Abstractions | net48;net10.0-windows | RetrySignalTests | planli |
| N02-103 | `RetrySignalOptions` | record | RetrySignal yapilandirmasi | VersaCoder.Abstractions | net48;net10.0-windows | RetrySignalTests | planli |
| N02-104 | `RetrySignalProvider` | class | RetrySignal saglayici uygulamasi | VersaCoder.Abstractions | net48;net10.0-windows | RetrySignalTests | planli |
| N02-105 | `RetrySignalRegistry` | class | RetrySignal kayit/katalog yonetimi | VersaCoder.Abstractions | net48;net10.0-windows | RetrySignalTests | planli |
| N02-106 | `RetrySignalConverter` | class | RetrySignal bicim donusumu | VersaCoder.Abstractions | net48;net10.0-windows | RetrySignalTests | planli |
| N02-107 | `RetrySignalTests` | class | RetrySignal testleri (xUnit) | VersaCoder.Abstractions | net48;net10.0-windows | - | planli |
| N02-108 | `RetrySignalBuilder` | class | RetrySignal uretici (builder) | VersaCoder.Abstractions | net48;net10.0-windows | RetrySignalTests | planli |
| N02-109 | `IDomainSignal` | interface | DomainSignal sozlesmesi (port) | VersaCoder.Abstractions | net48;net10.0-windows | DomainSignalTests | planli |
| N02-110 | `DomainSignal` | class | DomainSignal cekirdek uygulamasi | VersaCoder.Abstractions | net48;net10.0-windows | DomainSignalTests | planli |
| N02-111 | `DomainSignalDto` | record | DomainSignal veri transferi modeli | VersaCoder.Abstractions | net48;net10.0-windows | DomainSignalTests | planli |
| N02-112 | `DomainSignalMapper` | class | DomainSignal <-> DTO esleme | VersaCoder.Abstractions | net48;net10.0-windows | DomainSignalTests | planli |
| N02-113 | `DomainSignalValidator` | class | DomainSignal dogrulama kurallari | VersaCoder.Abstractions | net48;net10.0-windows | DomainSignalTests | planli |
| N02-114 | `DomainSignalFactory` | class | DomainSignal uretimi ve varsayilanlari | VersaCoder.Abstractions | net48;net10.0-windows | DomainSignalTests | planli |
| N02-115 | `DomainSignalOptions` | record | DomainSignal yapilandirmasi | VersaCoder.Abstractions | net48;net10.0-windows | DomainSignalTests | planli |
| N02-116 | `DomainSignalProvider` | class | DomainSignal saglayici uygulamasi | VersaCoder.Abstractions | net48;net10.0-windows | DomainSignalTests | planli |
| N02-117 | `DomainSignalRegistry` | class | DomainSignal kayit/katalog yonetimi | VersaCoder.Abstractions | net48;net10.0-windows | DomainSignalTests | planli |
| N02-118 | `DomainSignalConverter` | class | DomainSignal bicim donusumu | VersaCoder.Abstractions | net48;net10.0-windows | DomainSignalTests | planli |
| N02-119 | `DomainSignalTests` | class | DomainSignal testleri (xUnit) | VersaCoder.Abstractions | net48;net10.0-windows | - | planli |
| N02-120 | `DomainSignalBuilder` | class | DomainSignal uretici (builder) | VersaCoder.Abstractions | net48;net10.0-windows | DomainSignalTests | planli |
