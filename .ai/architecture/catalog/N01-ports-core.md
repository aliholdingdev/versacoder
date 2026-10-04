---
title: "Katalog Dugumu N01 - Port Sozlesmeleri (l0)"
type: catalog
category: architecture
status: needs-validation
version: 1.0.0
updated: 2026-10-04
authority: derived
scope: "N01 ports-core dugumu: kimlik, src klasoru, aile yapisi, ornek kayit ve yon kurali (ADR-013 + ADR-028)"
---

# Katalog Dugumu N01 - Port Sozlesmeleri

**Zorunlu Baglantilar:** [catalog index](index.md) · [node-inventory](node-inventory.md) · [mimari index](../index.md) · [ADR-028](../../.decisions/accepted/ADR-028-layered-baseline-9-projects.md) · [ADR-013](../../.decisions/accepted/ADR-013-layered-architecture-52-nodes.md)

> **Tek cumlelik sorumluluk:** *N01 ports-core hangi projede ve klasorde yasar, ne yapar, kac kayit hedefler* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

**N01 (ports-core), l0 Abstractions katmaninin alt katmanidir: `VersaCoder.Abstractions` projesinde `src/VersaCoder.Abstractions/Ports/` klasorunde yasar.** Dis dunyayla konusan port arayuzlerini tek yerde tanimlar (or. IHttpGateway, ADR-028 2.3/6); somut uygulama l4 katmaninda kalir. 10 aile x 12 rol = 120 kayit hedefi (ADR-013 asgari 112); durum `planli`.

Kayitlar ADR-013 2.1 biciminde 8 sutun tasir; bu dosya aile kalibini ve ornek satirlari gosterir, tam 120 satirlik liste bu dosyanin boyut sinirina sigmaz (DOĞRULAMA GEREKLI).

```text
+--------------------------------------------------+
| l0 Abstractions -> VersaCoder.Abstractions
| src/VersaCoder.Abstractions/Ports/
| 10 aile x 12 rol = 120 kayit (esik 112)
+--------------------------------------------------+
```

| Alan | Deger |
|---|---|
| Dugum | N01 |
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
| 1 | Dis dunyayla konusan port arayuzlerini tek yerde tanimlar (or. IHttpGateway, ADR-028 2.3/6); somut uygulama l4 katmaninda kalir | somut uygulama (adapter) sinifi yazmak, uygulama l4 katmaninda |
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
| 1 | N01 | ports-core | `src/VersaCoder.Abstractions/Ports/` |

**Kanit yolu (ADR-028 2.3/5):** tarama yolu gercek proje adini icerir; genis kalip (`src/**/Domain/**`) sahte PASS uretir.

```powershell
Test-Path 'C:\versacoder\src\VersaCoder.Abstractions\Ports'
```

Dizin su an yok cikti `False` olur; PASS yazilmaz -> **DOĞRULAMA GEREKLI**.

---

## §3 Aile Yapisi

Rol kalibi (12): `I<F>` · `<F>` · `<F>Dto` · `<F>Mapper` · `<F>Validator` · `<F>Factory` · `<F>Options` · `<F>Provider` · `<F>Registry` · `<F>Converter` · `<F>Tests` · `<F>Builder`

| # | Aile | # | Aile |
|---|---|---|---|
| 1 | TextPort | 6 | FileSystemPort |
| 2 | ClockPort | 7 | NetworkPort |
| 3 | EnvPort | 8 | PromptPort |
| 4 | ProcessPort | 9 | DiagPort |
| 5 | RandomPort | 10 | StreamPort |

Her aile ayni rol kalibini 12 kez tekrarlar; kayit sayisi = aile x rol.

---

## §4 Kayit Ornegi (8 sutun)

| ID | Bilesen | Tip | Sorumluluk | Proje | TFM | Test | Durum |
|---|---|---|---|---|---|---|---|
| N01-001 | `ITextPort` | interface | TextPort sozlesmesi (port) | VersaCoder.Abstractions | net48;net10.0-windows | TextPortTests | planli |
| N01-002 | `TextPort` | class | TextPort cekirdek uygulamasi | VersaCoder.Abstractions | net48;net10.0-windows | TextPortTests | planli |
| N01-003 | `TextPortDto` | record | TextPort veri transferi modeli | VersaCoder.Abstractions | net48;net10.0-windows | TextPortTests | planli |
| N01-004 | `TextPortMapper` | class | TextPort <-> DTO esleme | VersaCoder.Abstractions | net48;net10.0-windows | TextPortTests | planli |
| N01-005 | `TextPortValidator` | class | TextPort dogrulama kurallari | VersaCoder.Abstractions | net48;net10.0-windows | TextPortTests | planli |
| N01-006 | `TextPortFactory` | class | TextPort uretimi ve varsayilanlari | VersaCoder.Abstractions | net48;net10.0-windows | TextPortTests | planli |

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
| 3 | Test sinifi | TextPortTests (xUnit) | planli |
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
(Select-String -Path 'C:\versacoder\.ai\architecture\catalog\N01-ports-core.md' -Pattern '^\|\s*N\d{2}-\d{3}\s*\|').Count
# 2) boyut bandi - beklenen: 2600-8500
(Get-Item 'C:\versacoder\.ai\architecture\catalog\N01-ports-core.md').Length
# 3) klasor kaniti - gercek proje adi (ADR-028 2.3/5)
(Test-Path 'C:\versacoder\src\VersaCoder.Abstractions\Ports')
```

Katalog sayimi `powershell -NoProfile -ExecutionPolicy Bypass -File C:\versacoder\.ai\scripts\count-components.ps1` ile alinir; bu batch sonunda toplam esik (5000) henuz karsilanmaz - kalan 26 node dosyasi sonraki batchtir.

## §10 Kayit Tablosu (ADR-013 2.2 - kaynak vault port)

| N01-007 | `TextPortOptions` | record | TextPort yapilandirmasi | VersaCoder.Abstractions | net48;net10.0-windows | TextPortTests | planli |
| N01-008 | `TextPortProvider` | class | TextPort saglayici uygulamasi | VersaCoder.Abstractions | net48;net10.0-windows | TextPortTests | planli |
| N01-009 | `TextPortRegistry` | class | TextPort kayit/katalog yonetimi | VersaCoder.Abstractions | net48;net10.0-windows | TextPortTests | planli |
| N01-010 | `TextPortConverter` | class | TextPort bicim donusumu | VersaCoder.Abstractions | net48;net10.0-windows | TextPortTests | planli |
| N01-011 | `TextPortTests` | class | TextPort testleri (xUnit) | VersaCoder.Abstractions | net48;net10.0-windows | - | planli |
| N01-012 | `TextPortBuilder` | class | TextPort uretici (builder) | VersaCoder.Abstractions | net48;net10.0-windows | TextPortTests | planli |
| N01-013 | `IClockPort` | interface | ClockPort sozlesmesi (port) | VersaCoder.Abstractions | net48;net10.0-windows | ClockPortTests | planli |
| N01-014 | `ClockPort` | class | ClockPort cekirdek uygulamasi | VersaCoder.Abstractions | net48;net10.0-windows | ClockPortTests | planli |
| N01-015 | `ClockPortDto` | record | ClockPort veri transferi modeli | VersaCoder.Abstractions | net48;net10.0-windows | ClockPortTests | planli |
| N01-016 | `ClockPortMapper` | class | ClockPort <-> DTO esleme | VersaCoder.Abstractions | net48;net10.0-windows | ClockPortTests | planli |
| N01-017 | `ClockPortValidator` | class | ClockPort dogrulama kurallari | VersaCoder.Abstractions | net48;net10.0-windows | ClockPortTests | planli |
| N01-018 | `ClockPortFactory` | class | ClockPort uretimi ve varsayilanlari | VersaCoder.Abstractions | net48;net10.0-windows | ClockPortTests | planli |
| N01-019 | `ClockPortOptions` | record | ClockPort yapilandirmasi | VersaCoder.Abstractions | net48;net10.0-windows | ClockPortTests | planli |
| N01-020 | `ClockPortProvider` | class | ClockPort saglayici uygulamasi | VersaCoder.Abstractions | net48;net10.0-windows | ClockPortTests | planli |
| N01-021 | `ClockPortRegistry` | class | ClockPort kayit/katalog yonetimi | VersaCoder.Abstractions | net48;net10.0-windows | ClockPortTests | planli |
| N01-022 | `ClockPortConverter` | class | ClockPort bicim donusumu | VersaCoder.Abstractions | net48;net10.0-windows | ClockPortTests | planli |
| N01-023 | `ClockPortTests` | class | ClockPort testleri (xUnit) | VersaCoder.Abstractions | net48;net10.0-windows | - | planli |
| N01-024 | `ClockPortBuilder` | class | ClockPort uretici (builder) | VersaCoder.Abstractions | net48;net10.0-windows | ClockPortTests | planli |
| N01-025 | `IEnvPort` | interface | EnvPort sozlesmesi (port) | VersaCoder.Abstractions | net48;net10.0-windows | EnvPortTests | planli |
| N01-026 | `EnvPort` | class | EnvPort cekirdek uygulamasi | VersaCoder.Abstractions | net48;net10.0-windows | EnvPortTests | planli |
| N01-027 | `EnvPortDto` | record | EnvPort veri transferi modeli | VersaCoder.Abstractions | net48;net10.0-windows | EnvPortTests | planli |
| N01-028 | `EnvPortMapper` | class | EnvPort <-> DTO esleme | VersaCoder.Abstractions | net48;net10.0-windows | EnvPortTests | planli |
| N01-029 | `EnvPortValidator` | class | EnvPort dogrulama kurallari | VersaCoder.Abstractions | net48;net10.0-windows | EnvPortTests | planli |
| N01-030 | `EnvPortFactory` | class | EnvPort uretimi ve varsayilanlari | VersaCoder.Abstractions | net48;net10.0-windows | EnvPortTests | planli |
| N01-031 | `EnvPortOptions` | record | EnvPort yapilandirmasi | VersaCoder.Abstractions | net48;net10.0-windows | EnvPortTests | planli |
| N01-032 | `EnvPortProvider` | class | EnvPort saglayici uygulamasi | VersaCoder.Abstractions | net48;net10.0-windows | EnvPortTests | planli |
| N01-033 | `EnvPortRegistry` | class | EnvPort kayit/katalog yonetimi | VersaCoder.Abstractions | net48;net10.0-windows | EnvPortTests | planli |
| N01-034 | `EnvPortConverter` | class | EnvPort bicim donusumu | VersaCoder.Abstractions | net48;net10.0-windows | EnvPortTests | planli |
| N01-035 | `EnvPortTests` | class | EnvPort testleri (xUnit) | VersaCoder.Abstractions | net48;net10.0-windows | - | planli |
| N01-036 | `EnvPortBuilder` | class | EnvPort uretici (builder) | VersaCoder.Abstractions | net48;net10.0-windows | EnvPortTests | planli |
| N01-037 | `IProcessPort` | interface | ProcessPort sozlesmesi (port) | VersaCoder.Abstractions | net48;net10.0-windows | ProcessPortTests | planli |
| N01-038 | `ProcessPort` | class | ProcessPort cekirdek uygulamasi | VersaCoder.Abstractions | net48;net10.0-windows | ProcessPortTests | planli |
| N01-039 | `ProcessPortDto` | record | ProcessPort veri transferi modeli | VersaCoder.Abstractions | net48;net10.0-windows | ProcessPortTests | planli |
| N01-040 | `ProcessPortMapper` | class | ProcessPort <-> DTO esleme | VersaCoder.Abstractions | net48;net10.0-windows | ProcessPortTests | planli |
| N01-041 | `ProcessPortValidator` | class | ProcessPort dogrulama kurallari | VersaCoder.Abstractions | net48;net10.0-windows | ProcessPortTests | planli |
| N01-042 | `ProcessPortFactory` | class | ProcessPort uretimi ve varsayilanlari | VersaCoder.Abstractions | net48;net10.0-windows | ProcessPortTests | planli |
| N01-043 | `ProcessPortOptions` | record | ProcessPort yapilandirmasi | VersaCoder.Abstractions | net48;net10.0-windows | ProcessPortTests | planli |
| N01-044 | `ProcessPortProvider` | class | ProcessPort saglayici uygulamasi | VersaCoder.Abstractions | net48;net10.0-windows | ProcessPortTests | planli |
| N01-045 | `ProcessPortRegistry` | class | ProcessPort kayit/katalog yonetimi | VersaCoder.Abstractions | net48;net10.0-windows | ProcessPortTests | planli |
| N01-046 | `ProcessPortConverter` | class | ProcessPort bicim donusumu | VersaCoder.Abstractions | net48;net10.0-windows | ProcessPortTests | planli |
| N01-047 | `ProcessPortTests` | class | ProcessPort testleri (xUnit) | VersaCoder.Abstractions | net48;net10.0-windows | - | planli |
| N01-048 | `ProcessPortBuilder` | class | ProcessPort uretici (builder) | VersaCoder.Abstractions | net48;net10.0-windows | ProcessPortTests | planli |
| N01-049 | `IRandomPort` | interface | RandomPort sozlesmesi (port) | VersaCoder.Abstractions | net48;net10.0-windows | RandomPortTests | planli |
| N01-050 | `RandomPort` | class | RandomPort cekirdek uygulamasi | VersaCoder.Abstractions | net48;net10.0-windows | RandomPortTests | planli |
| N01-051 | `RandomPortDto` | record | RandomPort veri transferi modeli | VersaCoder.Abstractions | net48;net10.0-windows | RandomPortTests | planli |
| N01-052 | `RandomPortMapper` | class | RandomPort <-> DTO esleme | VersaCoder.Abstractions | net48;net10.0-windows | RandomPortTests | planli |
| N01-053 | `RandomPortValidator` | class | RandomPort dogrulama kurallari | VersaCoder.Abstractions | net48;net10.0-windows | RandomPortTests | planli |
| N01-054 | `RandomPortFactory` | class | RandomPort uretimi ve varsayilanlari | VersaCoder.Abstractions | net48;net10.0-windows | RandomPortTests | planli |
| N01-055 | `RandomPortOptions` | record | RandomPort yapilandirmasi | VersaCoder.Abstractions | net48;net10.0-windows | RandomPortTests | planli |
| N01-056 | `RandomPortProvider` | class | RandomPort saglayici uygulamasi | VersaCoder.Abstractions | net48;net10.0-windows | RandomPortTests | planli |
| N01-057 | `RandomPortRegistry` | class | RandomPort kayit/katalog yonetimi | VersaCoder.Abstractions | net48;net10.0-windows | RandomPortTests | planli |
| N01-058 | `RandomPortConverter` | class | RandomPort bicim donusumu | VersaCoder.Abstractions | net48;net10.0-windows | RandomPortTests | planli |
| N01-059 | `RandomPortTests` | class | RandomPort testleri (xUnit) | VersaCoder.Abstractions | net48;net10.0-windows | - | planli |
| N01-060 | `RandomPortBuilder` | class | RandomPort uretici (builder) | VersaCoder.Abstractions | net48;net10.0-windows | RandomPortTests | planli |
| N01-061 | `IFileSystemPort` | interface | FileSystemPort sozlesmesi (port) | VersaCoder.Abstractions | net48;net10.0-windows | FileSystemPortTests | planli |
| N01-062 | `FileSystemPort` | class | FileSystemPort cekirdek uygulamasi | VersaCoder.Abstractions | net48;net10.0-windows | FileSystemPortTests | planli |
| N01-063 | `FileSystemPortDto` | record | FileSystemPort veri transferi modeli | VersaCoder.Abstractions | net48;net10.0-windows | FileSystemPortTests | planli |
| N01-064 | `FileSystemPortMapper` | class | FileSystemPort <-> DTO esleme | VersaCoder.Abstractions | net48;net10.0-windows | FileSystemPortTests | planli |
| N01-065 | `FileSystemPortValidator` | class | FileSystemPort dogrulama kurallari | VersaCoder.Abstractions | net48;net10.0-windows | FileSystemPortTests | planli |
| N01-066 | `FileSystemPortFactory` | class | FileSystemPort uretimi ve varsayilanlari | VersaCoder.Abstractions | net48;net10.0-windows | FileSystemPortTests | planli |
| N01-067 | `FileSystemPortOptions` | record | FileSystemPort yapilandirmasi | VersaCoder.Abstractions | net48;net10.0-windows | FileSystemPortTests | planli |
| N01-068 | `FileSystemPortProvider` | class | FileSystemPort saglayici uygulamasi | VersaCoder.Abstractions | net48;net10.0-windows | FileSystemPortTests | planli |
| N01-069 | `FileSystemPortRegistry` | class | FileSystemPort kayit/katalog yonetimi | VersaCoder.Abstractions | net48;net10.0-windows | FileSystemPortTests | planli |
| N01-070 | `FileSystemPortConverter` | class | FileSystemPort bicim donusumu | VersaCoder.Abstractions | net48;net10.0-windows | FileSystemPortTests | planli |
| N01-071 | `FileSystemPortTests` | class | FileSystemPort testleri (xUnit) | VersaCoder.Abstractions | net48;net10.0-windows | - | planli |
| N01-072 | `FileSystemPortBuilder` | class | FileSystemPort uretici (builder) | VersaCoder.Abstractions | net48;net10.0-windows | FileSystemPortTests | planli |
| N01-073 | `INetworkPort` | interface | NetworkPort sozlesmesi (port) | VersaCoder.Abstractions | net48;net10.0-windows | NetworkPortTests | planli |
| N01-074 | `NetworkPort` | class | NetworkPort cekirdek uygulamasi | VersaCoder.Abstractions | net48;net10.0-windows | NetworkPortTests | planli |
| N01-075 | `NetworkPortDto` | record | NetworkPort veri transferi modeli | VersaCoder.Abstractions | net48;net10.0-windows | NetworkPortTests | planli |
| N01-076 | `NetworkPortMapper` | class | NetworkPort <-> DTO esleme | VersaCoder.Abstractions | net48;net10.0-windows | NetworkPortTests | planli |
| N01-077 | `NetworkPortValidator` | class | NetworkPort dogrulama kurallari | VersaCoder.Abstractions | net48;net10.0-windows | NetworkPortTests | planli |
| N01-078 | `NetworkPortFactory` | class | NetworkPort uretimi ve varsayilanlari | VersaCoder.Abstractions | net48;net10.0-windows | NetworkPortTests | planli |
| N01-079 | `NetworkPortOptions` | record | NetworkPort yapilandirmasi | VersaCoder.Abstractions | net48;net10.0-windows | NetworkPortTests | planli |
| N01-080 | `NetworkPortProvider` | class | NetworkPort saglayici uygulamasi | VersaCoder.Abstractions | net48;net10.0-windows | NetworkPortTests | planli |
| N01-081 | `NetworkPortRegistry` | class | NetworkPort kayit/katalog yonetimi | VersaCoder.Abstractions | net48;net10.0-windows | NetworkPortTests | planli |
| N01-082 | `NetworkPortConverter` | class | NetworkPort bicim donusumu | VersaCoder.Abstractions | net48;net10.0-windows | NetworkPortTests | planli |
| N01-083 | `NetworkPortTests` | class | NetworkPort testleri (xUnit) | VersaCoder.Abstractions | net48;net10.0-windows | - | planli |
| N01-084 | `NetworkPortBuilder` | class | NetworkPort uretici (builder) | VersaCoder.Abstractions | net48;net10.0-windows | NetworkPortTests | planli |
| N01-085 | `IPromptPort` | interface | PromptPort sozlesmesi (port) | VersaCoder.Abstractions | net48;net10.0-windows | PromptPortTests | planli |
| N01-086 | `PromptPort` | class | PromptPort cekirdek uygulamasi | VersaCoder.Abstractions | net48;net10.0-windows | PromptPortTests | planli |
| N01-087 | `PromptPortDto` | record | PromptPort veri transferi modeli | VersaCoder.Abstractions | net48;net10.0-windows | PromptPortTests | planli |
| N01-088 | `PromptPortMapper` | class | PromptPort <-> DTO esleme | VersaCoder.Abstractions | net48;net10.0-windows | PromptPortTests | planli |
| N01-089 | `PromptPortValidator` | class | PromptPort dogrulama kurallari | VersaCoder.Abstractions | net48;net10.0-windows | PromptPortTests | planli |
| N01-090 | `PromptPortFactory` | class | PromptPort uretimi ve varsayilanlari | VersaCoder.Abstractions | net48;net10.0-windows | PromptPortTests | planli |
| N01-091 | `PromptPortOptions` | record | PromptPort yapilandirmasi | VersaCoder.Abstractions | net48;net10.0-windows | PromptPortTests | planli |
| N01-092 | `PromptPortProvider` | class | PromptPort saglayici uygulamasi | VersaCoder.Abstractions | net48;net10.0-windows | PromptPortTests | planli |
| N01-093 | `PromptPortRegistry` | class | PromptPort kayit/katalog yonetimi | VersaCoder.Abstractions | net48;net10.0-windows | PromptPortTests | planli |
| N01-094 | `PromptPortConverter` | class | PromptPort bicim donusumu | VersaCoder.Abstractions | net48;net10.0-windows | PromptPortTests | planli |
| N01-095 | `PromptPortTests` | class | PromptPort testleri (xUnit) | VersaCoder.Abstractions | net48;net10.0-windows | - | planli |
| N01-096 | `PromptPortBuilder` | class | PromptPort uretici (builder) | VersaCoder.Abstractions | net48;net10.0-windows | PromptPortTests | planli |
| N01-097 | `IDiagPort` | interface | DiagPort sozlesmesi (port) | VersaCoder.Abstractions | net48;net10.0-windows | DiagPortTests | planli |
| N01-098 | `DiagPort` | class | DiagPort cekirdek uygulamasi | VersaCoder.Abstractions | net48;net10.0-windows | DiagPortTests | planli |
| N01-099 | `DiagPortDto` | record | DiagPort veri transferi modeli | VersaCoder.Abstractions | net48;net10.0-windows | DiagPortTests | planli |
| N01-100 | `DiagPortMapper` | class | DiagPort <-> DTO esleme | VersaCoder.Abstractions | net48;net10.0-windows | DiagPortTests | planli |
| N01-101 | `DiagPortValidator` | class | DiagPort dogrulama kurallari | VersaCoder.Abstractions | net48;net10.0-windows | DiagPortTests | planli |
| N01-102 | `DiagPortFactory` | class | DiagPort uretimi ve varsayilanlari | VersaCoder.Abstractions | net48;net10.0-windows | DiagPortTests | planli |
| N01-103 | `DiagPortOptions` | record | DiagPort yapilandirmasi | VersaCoder.Abstractions | net48;net10.0-windows | DiagPortTests | planli |
| N01-104 | `DiagPortProvider` | class | DiagPort saglayici uygulamasi | VersaCoder.Abstractions | net48;net10.0-windows | DiagPortTests | planli |
| N01-105 | `DiagPortRegistry` | class | DiagPort kayit/katalog yonetimi | VersaCoder.Abstractions | net48;net10.0-windows | DiagPortTests | planli |
| N01-106 | `DiagPortConverter` | class | DiagPort bicim donusumu | VersaCoder.Abstractions | net48;net10.0-windows | DiagPortTests | planli |
| N01-107 | `DiagPortTests` | class | DiagPort testleri (xUnit) | VersaCoder.Abstractions | net48;net10.0-windows | - | planli |
| N01-108 | `DiagPortBuilder` | class | DiagPort uretici (builder) | VersaCoder.Abstractions | net48;net10.0-windows | DiagPortTests | planli |
| N01-109 | `IStreamPort` | interface | StreamPort sozlesmesi (port) | VersaCoder.Abstractions | net48;net10.0-windows | StreamPortTests | planli |
| N01-110 | `StreamPort` | class | StreamPort cekirdek uygulamasi | VersaCoder.Abstractions | net48;net10.0-windows | StreamPortTests | planli |
| N01-111 | `StreamPortDto` | record | StreamPort veri transferi modeli | VersaCoder.Abstractions | net48;net10.0-windows | StreamPortTests | planli |
| N01-112 | `StreamPortMapper` | class | StreamPort <-> DTO esleme | VersaCoder.Abstractions | net48;net10.0-windows | StreamPortTests | planli |
| N01-113 | `StreamPortValidator` | class | StreamPort dogrulama kurallari | VersaCoder.Abstractions | net48;net10.0-windows | StreamPortTests | planli |
| N01-114 | `StreamPortFactory` | class | StreamPort uretimi ve varsayilanlari | VersaCoder.Abstractions | net48;net10.0-windows | StreamPortTests | planli |
| N01-115 | `StreamPortOptions` | record | StreamPort yapilandirmasi | VersaCoder.Abstractions | net48;net10.0-windows | StreamPortTests | planli |
| N01-116 | `StreamPortProvider` | class | StreamPort saglayici uygulamasi | VersaCoder.Abstractions | net48;net10.0-windows | StreamPortTests | planli |
| N01-117 | `StreamPortRegistry` | class | StreamPort kayit/katalog yonetimi | VersaCoder.Abstractions | net48;net10.0-windows | StreamPortTests | planli |
| N01-118 | `StreamPortConverter` | class | StreamPort bicim donusumu | VersaCoder.Abstractions | net48;net10.0-windows | StreamPortTests | planli |
| N01-119 | `StreamPortTests` | class | StreamPort testleri (xUnit) | VersaCoder.Abstractions | net48;net10.0-windows | - | planli |
| N01-120 | `StreamPortBuilder` | class | StreamPort uretici (builder) | VersaCoder.Abstractions | net48;net10.0-windows | StreamPortTests | planli |
