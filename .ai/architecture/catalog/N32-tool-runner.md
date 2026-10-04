---
title: "Katalog Dugumu N32 - Arac Calistirici (l4)"
type: catalog
category: architecture
status: needs-validation
version: 1.0.0
updated: 2026-10-04
authority: derived
scope: "N32 tool-runner dugumu: kimlik, src klasoru, aile yapisi, ornek kayit ve yon kurali (ADR-013 + ADR-028)"
---

# Katalog Dugumu N32 - Arac Calistirici

**Zorunlu Baglantilar:** [catalog index](index.md) · [node-inventory](node-inventory.md) · [mimari index](../index.md) · [ADR-028](../../.decisions/accepted/ADR-028-layered-baseline-9-projects.md) · [ADR-013](../../.decisions/accepted/ADR-013-layered-architecture-52-nodes.md)

> **Tek cumlelik sorumluluk:** *N32 tool-runner hangi projede ve klasorde yasar, ne yapar, kac kayit hedefler* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

**N32 (tool-runner), l4 Infrastructure katmaninin alt katmanidir: `VersaCoder.Infrastructure` projesinde `src/VersaCoder.Infrastructure/Tools/` klasorunde yasar.** Arac cagrilirini surec basina calistirir, cikti ve hatalari Result<T> ile doner. 10 aile x 12 rol = 120 kayit hedefi (ADR-013 asgari 112); durum `planli`.

Kayitlar ADR-013 2.1 biciminde 8 sutun tasir; bu dosya aile kalibini ve ornek satirlari gosterir, tam 120 satirlik liste bu dosyanin boyut sinirina sigmaz (DOĞRULAMA GEREKLI).

```text
+--------------------------------------------------+
| l4 Infrastructure -> VersaCoder.Infrastructure
| src/VersaCoder.Infrastructure/Tools/
| 10 aile x 12 rol = 120 kayit (esik 112)
+--------------------------------------------------+
```

| Alan | Deger |
|---|---|
| Dugum | N32 |
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
| 1 | arac cagrilirini surec basina calistirip ciktiyi Result<T> ile donmek | arac semasini uretmek (N33) veya dongu politikasini yazmak (N36) |
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
| 1 | N32 | tool-runner | `src/VersaCoder.Infrastructure/Tools/` |

**Kanit yolu (ADR-028 2.3/5):** tarama yolu gercek proje adini icerir; genis kalip (`src/**/Domain/**`) sahte PASS uretir.

```powershell
Test-Path 'C:\versacoder\src\VersaCoder.Infrastructure\Tools'
```

Dizin su an yok cikti `False` olur; PASS yazilmaz -> **DOĞRULAMA GEREKLI**.

---

## §3 Aile Yapisi

Rol kalibi (12): `I<F>` · `<F>` · `<F>Dto` · `<F>Mapper` · `<F>Validator` · `<F>Factory` · `<F>Options` · `<F>Provider` · `<F>Registry` · `<F>Converter` · `<F>Tests` · `<F>Builder`

| # | Aile | # | Aile |
|---|---|---|---|
| 1 | BashRunner | 6 | GlobTool |
| 2 | ReadTool | 7 | ListTool |
| 3 | WriteTool | 8 | WebFetchTool |
| 4 | EditTool | 9 | TaskTool |
| 5 | GrepTool | 10 | TodoTool |

Her aile ayni rol kalibini 12 kez tekrarlar; kayit sayisi = aile x rol.

---

## §4 Kayit Ornegi (8 sutun)

| ID | Bilesen | Tip | Sorumluluk | Proje | TFM | Test | Durum |
|---|---|---|---|---|---|---|---|
| N32-001 | `IBashRunner` | interface | BashRunner sozlesmesi (port) | VersaCoder.Infrastructure | net48;net10.0-windows | BashRunnerTests | planli |
| N32-002 | `BashRunner` | class | BashRunner cekirdek uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | BashRunnerTests | planli |
| N32-003 | `BashRunnerDto` | record | BashRunner veri transferi modeli | VersaCoder.Infrastructure | net48;net10.0-windows | BashRunnerTests | planli |
| N32-004 | `BashRunnerMapper` | class | BashRunner <-> DTO esleme | VersaCoder.Infrastructure | net48;net10.0-windows | BashRunnerTests | planli |
| N32-005 | `BashRunnerValidator` | class | BashRunner dogrulama kurallari | VersaCoder.Infrastructure | net48;net10.0-windows | BashRunnerTests | planli |
| N32-006 | `BashRunnerFactory` | class | BashRunner uretimi ve varsayilanlari | VersaCoder.Infrastructure | net48;net10.0-windows | BashRunnerTests | planli |

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
| 3 | Test sinifi | BashRunnerTests (xUnit) | planli |
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
(Select-String -Path 'C:\versacoder\.ai\architecture\catalog\N32-tool-runner.md' -Pattern '^\|\s*N\d{2}-\d{3}\s*\|').Count
# 2) boyut bandi - beklenen: 2600-8500
(Get-Item 'C:\versacoder\.ai\architecture\catalog\N32-tool-runner.md').Length
# 3) klasor kaniti - gercek proje adi (ADR-028 2.3/5)
(Test-Path 'C:\versacoder\src\VersaCoder.Infrastructure\Tools')
```

Katalog sayimi `powershell -NoProfile -ExecutionPolicy Bypass -File C:\versacoder\.ai\scripts\count-components.ps1` ile alinir; bu batch 26 node dosyasiyla 52 dugum klasorunu tamamlar - kayit esigi (5000) ornek satirlarla henuz karsilanmaz (DOĞRULAMA GEREKLI).

## §10 Kayit Tablosu (ADR-013 2.2 - kaynak vault port)

| N32-007 | `BashRunnerOptions` | record | BashRunner yapilandirmasi | VersaCoder.Infrastructure | net48;net10.0-windows | BashRunnerTests | planli |
| N32-008 | `BashRunnerProvider` | class | BashRunner saglayici uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | BashRunnerTests | planli |
| N32-009 | `BashRunnerRegistry` | class | BashRunner kayit/katalog yonetimi | VersaCoder.Infrastructure | net48;net10.0-windows | BashRunnerTests | planli |
| N32-010 | `BashRunnerConverter` | class | BashRunner bicim donusumu | VersaCoder.Infrastructure | net48;net10.0-windows | BashRunnerTests | planli |
| N32-011 | `BashRunnerTests` | class | BashRunner testleri (xUnit) | VersaCoder.Infrastructure | net48;net10.0-windows | - | planli |
| N32-012 | `BashRunnerBuilder` | class | BashRunner uretici (builder) | VersaCoder.Infrastructure | net48;net10.0-windows | BashRunnerTests | planli |
| N32-013 | `IReadTool` | interface | ReadTool sozlesmesi (port) | VersaCoder.Infrastructure | net48;net10.0-windows | ReadToolTests | planli |
| N32-014 | `ReadTool` | class | ReadTool cekirdek uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | ReadToolTests | planli |
| N32-015 | `ReadToolDto` | record | ReadTool veri transferi modeli | VersaCoder.Infrastructure | net48;net10.0-windows | ReadToolTests | planli |
| N32-016 | `ReadToolMapper` | class | ReadTool <-> DTO esleme | VersaCoder.Infrastructure | net48;net10.0-windows | ReadToolTests | planli |
| N32-017 | `ReadToolValidator` | class | ReadTool dogrulama kurallari | VersaCoder.Infrastructure | net48;net10.0-windows | ReadToolTests | planli |
| N32-018 | `ReadToolFactory` | class | ReadTool uretimi ve varsayilanlari | VersaCoder.Infrastructure | net48;net10.0-windows | ReadToolTests | planli |
| N32-019 | `ReadToolOptions` | record | ReadTool yapilandirmasi | VersaCoder.Infrastructure | net48;net10.0-windows | ReadToolTests | planli |
| N32-020 | `ReadToolProvider` | class | ReadTool saglayici uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | ReadToolTests | planli |
| N32-021 | `ReadToolRegistry` | class | ReadTool kayit/katalog yonetimi | VersaCoder.Infrastructure | net48;net10.0-windows | ReadToolTests | planli |
| N32-022 | `ReadToolConverter` | class | ReadTool bicim donusumu | VersaCoder.Infrastructure | net48;net10.0-windows | ReadToolTests | planli |
| N32-023 | `ReadToolTests` | class | ReadTool testleri (xUnit) | VersaCoder.Infrastructure | net48;net10.0-windows | - | planli |
| N32-024 | `ReadToolBuilder` | class | ReadTool uretici (builder) | VersaCoder.Infrastructure | net48;net10.0-windows | ReadToolTests | planli |
| N32-025 | `IWriteTool` | interface | WriteTool sozlesmesi (port) | VersaCoder.Infrastructure | net48;net10.0-windows | WriteToolTests | planli |
| N32-026 | `WriteTool` | class | WriteTool cekirdek uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | WriteToolTests | planli |
| N32-027 | `WriteToolDto` | record | WriteTool veri transferi modeli | VersaCoder.Infrastructure | net48;net10.0-windows | WriteToolTests | planli |
| N32-028 | `WriteToolMapper` | class | WriteTool <-> DTO esleme | VersaCoder.Infrastructure | net48;net10.0-windows | WriteToolTests | planli |
| N32-029 | `WriteToolValidator` | class | WriteTool dogrulama kurallari | VersaCoder.Infrastructure | net48;net10.0-windows | WriteToolTests | planli |
| N32-030 | `WriteToolFactory` | class | WriteTool uretimi ve varsayilanlari | VersaCoder.Infrastructure | net48;net10.0-windows | WriteToolTests | planli |
| N32-031 | `WriteToolOptions` | record | WriteTool yapilandirmasi | VersaCoder.Infrastructure | net48;net10.0-windows | WriteToolTests | planli |
| N32-032 | `WriteToolProvider` | class | WriteTool saglayici uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | WriteToolTests | planli |
| N32-033 | `WriteToolRegistry` | class | WriteTool kayit/katalog yonetimi | VersaCoder.Infrastructure | net48;net10.0-windows | WriteToolTests | planli |
| N32-034 | `WriteToolConverter` | class | WriteTool bicim donusumu | VersaCoder.Infrastructure | net48;net10.0-windows | WriteToolTests | planli |
| N32-035 | `WriteToolTests` | class | WriteTool testleri (xUnit) | VersaCoder.Infrastructure | net48;net10.0-windows | - | planli |
| N32-036 | `WriteToolBuilder` | class | WriteTool uretici (builder) | VersaCoder.Infrastructure | net48;net10.0-windows | WriteToolTests | planli |
| N32-037 | `IEditTool` | interface | EditTool sozlesmesi (port) | VersaCoder.Infrastructure | net48;net10.0-windows | EditToolTests | planli |
| N32-038 | `EditTool` | class | EditTool cekirdek uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | EditToolTests | planli |
| N32-039 | `EditToolDto` | record | EditTool veri transferi modeli | VersaCoder.Infrastructure | net48;net10.0-windows | EditToolTests | planli |
| N32-040 | `EditToolMapper` | class | EditTool <-> DTO esleme | VersaCoder.Infrastructure | net48;net10.0-windows | EditToolTests | planli |
| N32-041 | `EditToolValidator` | class | EditTool dogrulama kurallari | VersaCoder.Infrastructure | net48;net10.0-windows | EditToolTests | planli |
| N32-042 | `EditToolFactory` | class | EditTool uretimi ve varsayilanlari | VersaCoder.Infrastructure | net48;net10.0-windows | EditToolTests | planli |
| N32-043 | `EditToolOptions` | record | EditTool yapilandirmasi | VersaCoder.Infrastructure | net48;net10.0-windows | EditToolTests | planli |
| N32-044 | `EditToolProvider` | class | EditTool saglayici uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | EditToolTests | planli |
| N32-045 | `EditToolRegistry` | class | EditTool kayit/katalog yonetimi | VersaCoder.Infrastructure | net48;net10.0-windows | EditToolTests | planli |
| N32-046 | `EditToolConverter` | class | EditTool bicim donusumu | VersaCoder.Infrastructure | net48;net10.0-windows | EditToolTests | planli |
| N32-047 | `EditToolTests` | class | EditTool testleri (xUnit) | VersaCoder.Infrastructure | net48;net10.0-windows | - | planli |
| N32-048 | `EditToolBuilder` | class | EditTool uretici (builder) | VersaCoder.Infrastructure | net48;net10.0-windows | EditToolTests | planli |
| N32-049 | `IGrepTool` | interface | GrepTool sozlesmesi (port) | VersaCoder.Infrastructure | net48;net10.0-windows | GrepToolTests | planli |
| N32-050 | `GrepTool` | class | GrepTool cekirdek uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | GrepToolTests | planli |
| N32-051 | `GrepToolDto` | record | GrepTool veri transferi modeli | VersaCoder.Infrastructure | net48;net10.0-windows | GrepToolTests | planli |
| N32-052 | `GrepToolMapper` | class | GrepTool <-> DTO esleme | VersaCoder.Infrastructure | net48;net10.0-windows | GrepToolTests | planli |
| N32-053 | `GrepToolValidator` | class | GrepTool dogrulama kurallari | VersaCoder.Infrastructure | net48;net10.0-windows | GrepToolTests | planli |
| N32-054 | `GrepToolFactory` | class | GrepTool uretimi ve varsayilanlari | VersaCoder.Infrastructure | net48;net10.0-windows | GrepToolTests | planli |
| N32-055 | `GrepToolOptions` | record | GrepTool yapilandirmasi | VersaCoder.Infrastructure | net48;net10.0-windows | GrepToolTests | planli |
| N32-056 | `GrepToolProvider` | class | GrepTool saglayici uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | GrepToolTests | planli |
| N32-057 | `GrepToolRegistry` | class | GrepTool kayit/katalog yonetimi | VersaCoder.Infrastructure | net48;net10.0-windows | GrepToolTests | planli |
| N32-058 | `GrepToolConverter` | class | GrepTool bicim donusumu | VersaCoder.Infrastructure | net48;net10.0-windows | GrepToolTests | planli |
| N32-059 | `GrepToolTests` | class | GrepTool testleri (xUnit) | VersaCoder.Infrastructure | net48;net10.0-windows | - | planli |
| N32-060 | `GrepToolBuilder` | class | GrepTool uretici (builder) | VersaCoder.Infrastructure | net48;net10.0-windows | GrepToolTests | planli |
| N32-061 | `IGlobTool` | interface | GlobTool sozlesmesi (port) | VersaCoder.Infrastructure | net48;net10.0-windows | GlobToolTests | planli |
| N32-062 | `GlobTool` | class | GlobTool cekirdek uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | GlobToolTests | planli |
| N32-063 | `GlobToolDto` | record | GlobTool veri transferi modeli | VersaCoder.Infrastructure | net48;net10.0-windows | GlobToolTests | planli |
| N32-064 | `GlobToolMapper` | class | GlobTool <-> DTO esleme | VersaCoder.Infrastructure | net48;net10.0-windows | GlobToolTests | planli |
| N32-065 | `GlobToolValidator` | class | GlobTool dogrulama kurallari | VersaCoder.Infrastructure | net48;net10.0-windows | GlobToolTests | planli |
| N32-066 | `GlobToolFactory` | class | GlobTool uretimi ve varsayilanlari | VersaCoder.Infrastructure | net48;net10.0-windows | GlobToolTests | planli |
| N32-067 | `GlobToolOptions` | record | GlobTool yapilandirmasi | VersaCoder.Infrastructure | net48;net10.0-windows | GlobToolTests | planli |
| N32-068 | `GlobToolProvider` | class | GlobTool saglayici uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | GlobToolTests | planli |
| N32-069 | `GlobToolRegistry` | class | GlobTool kayit/katalog yonetimi | VersaCoder.Infrastructure | net48;net10.0-windows | GlobToolTests | planli |
| N32-070 | `GlobToolConverter` | class | GlobTool bicim donusumu | VersaCoder.Infrastructure | net48;net10.0-windows | GlobToolTests | planli |
| N32-071 | `GlobToolTests` | class | GlobTool testleri (xUnit) | VersaCoder.Infrastructure | net48;net10.0-windows | - | planli |
| N32-072 | `GlobToolBuilder` | class | GlobTool uretici (builder) | VersaCoder.Infrastructure | net48;net10.0-windows | GlobToolTests | planli |
| N32-073 | `IListTool` | interface | ListTool sozlesmesi (port) | VersaCoder.Infrastructure | net48;net10.0-windows | ListToolTests | planli |
| N32-074 | `ListTool` | class | ListTool cekirdek uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | ListToolTests | planli |
| N32-075 | `ListToolDto` | record | ListTool veri transferi modeli | VersaCoder.Infrastructure | net48;net10.0-windows | ListToolTests | planli |
| N32-076 | `ListToolMapper` | class | ListTool <-> DTO esleme | VersaCoder.Infrastructure | net48;net10.0-windows | ListToolTests | planli |
| N32-077 | `ListToolValidator` | class | ListTool dogrulama kurallari | VersaCoder.Infrastructure | net48;net10.0-windows | ListToolTests | planli |
| N32-078 | `ListToolFactory` | class | ListTool uretimi ve varsayilanlari | VersaCoder.Infrastructure | net48;net10.0-windows | ListToolTests | planli |
| N32-079 | `ListToolOptions` | record | ListTool yapilandirmasi | VersaCoder.Infrastructure | net48;net10.0-windows | ListToolTests | planli |
| N32-080 | `ListToolProvider` | class | ListTool saglayici uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | ListToolTests | planli |
| N32-081 | `ListToolRegistry` | class | ListTool kayit/katalog yonetimi | VersaCoder.Infrastructure | net48;net10.0-windows | ListToolTests | planli |
| N32-082 | `ListToolConverter` | class | ListTool bicim donusumu | VersaCoder.Infrastructure | net48;net10.0-windows | ListToolTests | planli |
| N32-083 | `ListToolTests` | class | ListTool testleri (xUnit) | VersaCoder.Infrastructure | net48;net10.0-windows | - | planli |
| N32-084 | `ListToolBuilder` | class | ListTool uretici (builder) | VersaCoder.Infrastructure | net48;net10.0-windows | ListToolTests | planli |
| N32-085 | `IWebFetchTool` | interface | WebFetchTool sozlesmesi (port) | VersaCoder.Infrastructure | net48;net10.0-windows | WebFetchToolTests | planli |
| N32-086 | `WebFetchTool` | class | WebFetchTool cekirdek uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | WebFetchToolTests | planli |
| N32-087 | `WebFetchToolDto` | record | WebFetchTool veri transferi modeli | VersaCoder.Infrastructure | net48;net10.0-windows | WebFetchToolTests | planli |
| N32-088 | `WebFetchToolMapper` | class | WebFetchTool <-> DTO esleme | VersaCoder.Infrastructure | net48;net10.0-windows | WebFetchToolTests | planli |
| N32-089 | `WebFetchToolValidator` | class | WebFetchTool dogrulama kurallari | VersaCoder.Infrastructure | net48;net10.0-windows | WebFetchToolTests | planli |
| N32-090 | `WebFetchToolFactory` | class | WebFetchTool uretimi ve varsayilanlari | VersaCoder.Infrastructure | net48;net10.0-windows | WebFetchToolTests | planli |
| N32-091 | `WebFetchToolOptions` | record | WebFetchTool yapilandirmasi | VersaCoder.Infrastructure | net48;net10.0-windows | WebFetchToolTests | planli |
| N32-092 | `WebFetchToolProvider` | class | WebFetchTool saglayici uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | WebFetchToolTests | planli |
| N32-093 | `WebFetchToolRegistry` | class | WebFetchTool kayit/katalog yonetimi | VersaCoder.Infrastructure | net48;net10.0-windows | WebFetchToolTests | planli |
| N32-094 | `WebFetchToolConverter` | class | WebFetchTool bicim donusumu | VersaCoder.Infrastructure | net48;net10.0-windows | WebFetchToolTests | planli |
| N32-095 | `WebFetchToolTests` | class | WebFetchTool testleri (xUnit) | VersaCoder.Infrastructure | net48;net10.0-windows | - | planli |
| N32-096 | `WebFetchToolBuilder` | class | WebFetchTool uretici (builder) | VersaCoder.Infrastructure | net48;net10.0-windows | WebFetchToolTests | planli |
| N32-097 | `ITaskTool` | interface | TaskTool sozlesmesi (port) | VersaCoder.Infrastructure | net48;net10.0-windows | TaskToolTests | planli |
| N32-098 | `TaskTool` | class | TaskTool cekirdek uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | TaskToolTests | planli |
| N32-099 | `TaskToolDto` | record | TaskTool veri transferi modeli | VersaCoder.Infrastructure | net48;net10.0-windows | TaskToolTests | planli |
| N32-100 | `TaskToolMapper` | class | TaskTool <-> DTO esleme | VersaCoder.Infrastructure | net48;net10.0-windows | TaskToolTests | planli |
| N32-101 | `TaskToolValidator` | class | TaskTool dogrulama kurallari | VersaCoder.Infrastructure | net48;net10.0-windows | TaskToolTests | planli |
| N32-102 | `TaskToolFactory` | class | TaskTool uretimi ve varsayilanlari | VersaCoder.Infrastructure | net48;net10.0-windows | TaskToolTests | planli |
| N32-103 | `TaskToolOptions` | record | TaskTool yapilandirmasi | VersaCoder.Infrastructure | net48;net10.0-windows | TaskToolTests | planli |
| N32-104 | `TaskToolProvider` | class | TaskTool saglayici uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | TaskToolTests | planli |
| N32-105 | `TaskToolRegistry` | class | TaskTool kayit/katalog yonetimi | VersaCoder.Infrastructure | net48;net10.0-windows | TaskToolTests | planli |
| N32-106 | `TaskToolConverter` | class | TaskTool bicim donusumu | VersaCoder.Infrastructure | net48;net10.0-windows | TaskToolTests | planli |
| N32-107 | `TaskToolTests` | class | TaskTool testleri (xUnit) | VersaCoder.Infrastructure | net48;net10.0-windows | - | planli |
| N32-108 | `TaskToolBuilder` | class | TaskTool uretici (builder) | VersaCoder.Infrastructure | net48;net10.0-windows | TaskToolTests | planli |
| N32-109 | `ITodoTool` | interface | TodoTool sozlesmesi (port) | VersaCoder.Infrastructure | net48;net10.0-windows | TodoToolTests | planli |
| N32-110 | `TodoTool` | class | TodoTool cekirdek uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | TodoToolTests | planli |
| N32-111 | `TodoToolDto` | record | TodoTool veri transferi modeli | VersaCoder.Infrastructure | net48;net10.0-windows | TodoToolTests | planli |
| N32-112 | `TodoToolMapper` | class | TodoTool <-> DTO esleme | VersaCoder.Infrastructure | net48;net10.0-windows | TodoToolTests | planli |
| N32-113 | `TodoToolValidator` | class | TodoTool dogrulama kurallari | VersaCoder.Infrastructure | net48;net10.0-windows | TodoToolTests | planli |
| N32-114 | `TodoToolFactory` | class | TodoTool uretimi ve varsayilanlari | VersaCoder.Infrastructure | net48;net10.0-windows | TodoToolTests | planli |
| N32-115 | `TodoToolOptions` | record | TodoTool yapilandirmasi | VersaCoder.Infrastructure | net48;net10.0-windows | TodoToolTests | planli |
| N32-116 | `TodoToolProvider` | class | TodoTool saglayici uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | TodoToolTests | planli |
| N32-117 | `TodoToolRegistry` | class | TodoTool kayit/katalog yonetimi | VersaCoder.Infrastructure | net48;net10.0-windows | TodoToolTests | planli |
| N32-118 | `TodoToolConverter` | class | TodoTool bicim donusumu | VersaCoder.Infrastructure | net48;net10.0-windows | TodoToolTests | planli |
| N32-119 | `TodoToolTests` | class | TodoTool testleri (xUnit) | VersaCoder.Infrastructure | net48;net10.0-windows | - | planli |
| N32-120 | `TodoToolBuilder` | class | TodoTool uretici (builder) | VersaCoder.Infrastructure | net48;net10.0-windows | TodoToolTests | planli |
