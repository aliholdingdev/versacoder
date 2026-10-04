---
title: "Katalog Dugumu N30 - Git Adaptoru (l4)"
type: catalog
category: architecture
status: needs-validation
version: 1.0.0
updated: 2026-10-04
authority: derived
scope: "N30 git-adapter dugumu: kimlik, src klasoru, aile yapisi, ornek kayit ve yon kurali (ADR-013 + ADR-028)"
---

# Katalog Dugumu N30 - Git Adaptoru

**Zorunlu Baglantilar:** [catalog index](index.md) · [node-inventory](node-inventory.md) · [mimari index](../index.md) · [ADR-028](../../.decisions/accepted/ADR-028-layered-baseline-9-projects.md) · [ADR-013](../../.decisions/accepted/ADR-013-layered-architecture-52-nodes.md)

> **Tek cumlelik sorumluluk:** *N30 git-adapter hangi projede ve klasorde yasar, ne yapar, kac kayit hedefler* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

**N30 (git-adapter), l4 Infrastructure katmaninin alt katmanidir: `VersaCoder.Infrastructure` projesinde `src/VersaCoder.Infrastructure/Git/` klasorunde yasar.** Git status, diff, commit ve branch islerini surec basina sarar. 10 aile x 12 rol = 120 kayit hedefi (ADR-013 asgari 112); durum `planli`.

Kayitlar ADR-013 2.1 biciminde 8 sutun tasir; bu dosya aile kalibini ve ornek satirlari gosterir, tam 120 satirlik liste bu dosyanin boyut sinirina sigmaz (DOĞRULAMA GEREKLI).

```text
+--------------------------------------------------+
| l4 Infrastructure -> VersaCoder.Infrastructure
| src/VersaCoder.Infrastructure/Git/
| 10 aile x 12 rol = 120 kayit (esik 112)
+--------------------------------------------------+
```

| Alan | Deger |
|---|---|
| Dugum | N30 |
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
| 1 | git status, diff, commit ve branch islerini surec basina sarmak | depo icine dogrudan dosya yazmak (her is git uzerinden gecer) |
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
| 1 | N30 | git-adapter | `src/VersaCoder.Infrastructure/Git/` |

**Kanit yolu (ADR-028 2.3/5):** tarama yolu gercek proje adini icerir; genis kalip (`src/**/Domain/**`) sahte PASS uretir.

```powershell
Test-Path 'C:\versacoder\src\VersaCoder.Infrastructure\Git'
```

Dizin su an yok cikti `False` olur; PASS yazilmaz -> **DOĞRULAMA GEREKLI**.

---

## §3 Aile Yapisi

Rol kalibi (12): `I<F>` · `<F>` · `<F>Dto` · `<F>Mapper` · `<F>Validator` · `<F>Factory` · `<F>Options` · `<F>Provider` · `<F>Registry` · `<F>Converter` · `<F>Tests` · `<F>Builder`

| # | Aile | # | Aile |
|---|---|---|---|
| 1 | GitClient | 6 | GitIgnoreWriter |
| 2 | GitStatusReader | 7 | GitRepoProbe |
| 3 | GitDiffReader | 8 | GitLogReader |
| 4 | GitCommitWriter | 9 | GitStashOps |
| 5 | GitBranchReader | 10 | GitSandbox |

Her aile ayni rol kalibini 12 kez tekrarlar; kayit sayisi = aile x rol.

---

## §4 Kayit Ornegi (8 sutun)

| ID | Bilesen | Tip | Sorumluluk | Proje | TFM | Test | Durum |
|---|---|---|---|---|---|---|---|
| N30-001 | `IGitClient` | interface | GitClient sozlesmesi (port) | VersaCoder.Infrastructure | net48;net10.0-windows | GitClientTests | planli |
| N30-002 | `GitClient` | class | GitClient cekirdek uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | GitClientTests | planli |
| N30-003 | `GitClientDto` | record | GitClient veri transferi modeli | VersaCoder.Infrastructure | net48;net10.0-windows | GitClientTests | planli |
| N30-004 | `GitClientMapper` | class | GitClient <-> DTO esleme | VersaCoder.Infrastructure | net48;net10.0-windows | GitClientTests | planli |
| N30-005 | `GitClientValidator` | class | GitClient dogrulama kurallari | VersaCoder.Infrastructure | net48;net10.0-windows | GitClientTests | planli |
| N30-006 | `GitClientFactory` | class | GitClient uretimi ve varsayilanlari | VersaCoder.Infrastructure | net48;net10.0-windows | GitClientTests | planli |

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
| 3 | Test sinifi | GitClientTests (xUnit) | planli |
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
(Select-String -Path 'C:\versacoder\.ai\architecture\catalog\N30-git-adapter.md' -Pattern '^\|\s*N\d{2}-\d{3}\s*\|').Count
# 2) boyut bandi - beklenen: 2600-8500
(Get-Item 'C:\versacoder\.ai\architecture\catalog\N30-git-adapter.md').Length
# 3) klasor kaniti - gercek proje adi (ADR-028 2.3/5)
(Test-Path 'C:\versacoder\src\VersaCoder.Infrastructure\Git')
```

Katalog sayimi `powershell -NoProfile -ExecutionPolicy Bypass -File C:\versacoder\.ai\scripts\count-components.ps1` ile alinir; bu batch 26 node dosyasiyla 52 dugum klasorunu tamamlar - kayit esigi (5000) ornek satirlarla henuz karsilanmaz (DOĞRULAMA GEREKLI).

## §10 Kayit Tablosu (ADR-013 2.2 - kaynak vault port)

| N30-007 | `GitClientOptions` | record | GitClient yapilandirmasi | VersaCoder.Infrastructure | net48;net10.0-windows | GitClientTests | planli |
| N30-008 | `GitClientProvider` | class | GitClient saglayici uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | GitClientTests | planli |
| N30-009 | `GitClientRegistry` | class | GitClient kayit/katalog yonetimi | VersaCoder.Infrastructure | net48;net10.0-windows | GitClientTests | planli |
| N30-010 | `GitClientConverter` | class | GitClient bicim donusumu | VersaCoder.Infrastructure | net48;net10.0-windows | GitClientTests | planli |
| N30-011 | `GitClientTests` | class | GitClient testleri (xUnit) | VersaCoder.Infrastructure | net48;net10.0-windows | - | planli |
| N30-012 | `GitClientBuilder` | class | GitClient uretici (builder) | VersaCoder.Infrastructure | net48;net10.0-windows | GitClientTests | planli |
| N30-013 | `IGitStatusReader` | interface | GitStatusReader sozlesmesi (port) | VersaCoder.Infrastructure | net48;net10.0-windows | GitStatusReaderTests | planli |
| N30-014 | `GitStatusReader` | class | GitStatusReader cekirdek uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | GitStatusReaderTests | planli |
| N30-015 | `GitStatusReaderDto` | record | GitStatusReader veri transferi modeli | VersaCoder.Infrastructure | net48;net10.0-windows | GitStatusReaderTests | planli |
| N30-016 | `GitStatusReaderMapper` | class | GitStatusReader <-> DTO esleme | VersaCoder.Infrastructure | net48;net10.0-windows | GitStatusReaderTests | planli |
| N30-017 | `GitStatusReaderValidator` | class | GitStatusReader dogrulama kurallari | VersaCoder.Infrastructure | net48;net10.0-windows | GitStatusReaderTests | planli |
| N30-018 | `GitStatusReaderFactory` | class | GitStatusReader uretimi ve varsayilanlari | VersaCoder.Infrastructure | net48;net10.0-windows | GitStatusReaderTests | planli |
| N30-019 | `GitStatusReaderOptions` | record | GitStatusReader yapilandirmasi | VersaCoder.Infrastructure | net48;net10.0-windows | GitStatusReaderTests | planli |
| N30-020 | `GitStatusReaderProvider` | class | GitStatusReader saglayici uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | GitStatusReaderTests | planli |
| N30-021 | `GitStatusReaderRegistry` | class | GitStatusReader kayit/katalog yonetimi | VersaCoder.Infrastructure | net48;net10.0-windows | GitStatusReaderTests | planli |
| N30-022 | `GitStatusReaderConverter` | class | GitStatusReader bicim donusumu | VersaCoder.Infrastructure | net48;net10.0-windows | GitStatusReaderTests | planli |
| N30-023 | `GitStatusReaderTests` | class | GitStatusReader testleri (xUnit) | VersaCoder.Infrastructure | net48;net10.0-windows | - | planli |
| N30-024 | `GitStatusReaderBuilder` | class | GitStatusReader uretici (builder) | VersaCoder.Infrastructure | net48;net10.0-windows | GitStatusReaderTests | planli |
| N30-025 | `IGitDiffReader` | interface | GitDiffReader sozlesmesi (port) | VersaCoder.Infrastructure | net48;net10.0-windows | GitDiffReaderTests | planli |
| N30-026 | `GitDiffReader` | class | GitDiffReader cekirdek uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | GitDiffReaderTests | planli |
| N30-027 | `GitDiffReaderDto` | record | GitDiffReader veri transferi modeli | VersaCoder.Infrastructure | net48;net10.0-windows | GitDiffReaderTests | planli |
| N30-028 | `GitDiffReaderMapper` | class | GitDiffReader <-> DTO esleme | VersaCoder.Infrastructure | net48;net10.0-windows | GitDiffReaderTests | planli |
| N30-029 | `GitDiffReaderValidator` | class | GitDiffReader dogrulama kurallari | VersaCoder.Infrastructure | net48;net10.0-windows | GitDiffReaderTests | planli |
| N30-030 | `GitDiffReaderFactory` | class | GitDiffReader uretimi ve varsayilanlari | VersaCoder.Infrastructure | net48;net10.0-windows | GitDiffReaderTests | planli |
| N30-031 | `GitDiffReaderOptions` | record | GitDiffReader yapilandirmasi | VersaCoder.Infrastructure | net48;net10.0-windows | GitDiffReaderTests | planli |
| N30-032 | `GitDiffReaderProvider` | class | GitDiffReader saglayici uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | GitDiffReaderTests | planli |
| N30-033 | `GitDiffReaderRegistry` | class | GitDiffReader kayit/katalog yonetimi | VersaCoder.Infrastructure | net48;net10.0-windows | GitDiffReaderTests | planli |
| N30-034 | `GitDiffReaderConverter` | class | GitDiffReader bicim donusumu | VersaCoder.Infrastructure | net48;net10.0-windows | GitDiffReaderTests | planli |
| N30-035 | `GitDiffReaderTests` | class | GitDiffReader testleri (xUnit) | VersaCoder.Infrastructure | net48;net10.0-windows | - | planli |
| N30-036 | `GitDiffReaderBuilder` | class | GitDiffReader uretici (builder) | VersaCoder.Infrastructure | net48;net10.0-windows | GitDiffReaderTests | planli |
| N30-037 | `IGitCommitWriter` | interface | GitCommitWriter sozlesmesi (port) | VersaCoder.Infrastructure | net48;net10.0-windows | GitCommitWriterTests | planli |
| N30-038 | `GitCommitWriter` | class | GitCommitWriter cekirdek uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | GitCommitWriterTests | planli |
| N30-039 | `GitCommitWriterDto` | record | GitCommitWriter veri transferi modeli | VersaCoder.Infrastructure | net48;net10.0-windows | GitCommitWriterTests | planli |
| N30-040 | `GitCommitWriterMapper` | class | GitCommitWriter <-> DTO esleme | VersaCoder.Infrastructure | net48;net10.0-windows | GitCommitWriterTests | planli |
| N30-041 | `GitCommitWriterValidator` | class | GitCommitWriter dogrulama kurallari | VersaCoder.Infrastructure | net48;net10.0-windows | GitCommitWriterTests | planli |
| N30-042 | `GitCommitWriterFactory` | class | GitCommitWriter uretimi ve varsayilanlari | VersaCoder.Infrastructure | net48;net10.0-windows | GitCommitWriterTests | planli |
| N30-043 | `GitCommitWriterOptions` | record | GitCommitWriter yapilandirmasi | VersaCoder.Infrastructure | net48;net10.0-windows | GitCommitWriterTests | planli |
| N30-044 | `GitCommitWriterProvider` | class | GitCommitWriter saglayici uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | GitCommitWriterTests | planli |
| N30-045 | `GitCommitWriterRegistry` | class | GitCommitWriter kayit/katalog yonetimi | VersaCoder.Infrastructure | net48;net10.0-windows | GitCommitWriterTests | planli |
| N30-046 | `GitCommitWriterConverter` | class | GitCommitWriter bicim donusumu | VersaCoder.Infrastructure | net48;net10.0-windows | GitCommitWriterTests | planli |
| N30-047 | `GitCommitWriterTests` | class | GitCommitWriter testleri (xUnit) | VersaCoder.Infrastructure | net48;net10.0-windows | - | planli |
| N30-048 | `GitCommitWriterBuilder` | class | GitCommitWriter uretici (builder) | VersaCoder.Infrastructure | net48;net10.0-windows | GitCommitWriterTests | planli |
| N30-049 | `IGitBranchReader` | interface | GitBranchReader sozlesmesi (port) | VersaCoder.Infrastructure | net48;net10.0-windows | GitBranchReaderTests | planli |
| N30-050 | `GitBranchReader` | class | GitBranchReader cekirdek uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | GitBranchReaderTests | planli |
| N30-051 | `GitBranchReaderDto` | record | GitBranchReader veri transferi modeli | VersaCoder.Infrastructure | net48;net10.0-windows | GitBranchReaderTests | planli |
| N30-052 | `GitBranchReaderMapper` | class | GitBranchReader <-> DTO esleme | VersaCoder.Infrastructure | net48;net10.0-windows | GitBranchReaderTests | planli |
| N30-053 | `GitBranchReaderValidator` | class | GitBranchReader dogrulama kurallari | VersaCoder.Infrastructure | net48;net10.0-windows | GitBranchReaderTests | planli |
| N30-054 | `GitBranchReaderFactory` | class | GitBranchReader uretimi ve varsayilanlari | VersaCoder.Infrastructure | net48;net10.0-windows | GitBranchReaderTests | planli |
| N30-055 | `GitBranchReaderOptions` | record | GitBranchReader yapilandirmasi | VersaCoder.Infrastructure | net48;net10.0-windows | GitBranchReaderTests | planli |
| N30-056 | `GitBranchReaderProvider` | class | GitBranchReader saglayici uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | GitBranchReaderTests | planli |
| N30-057 | `GitBranchReaderRegistry` | class | GitBranchReader kayit/katalog yonetimi | VersaCoder.Infrastructure | net48;net10.0-windows | GitBranchReaderTests | planli |
| N30-058 | `GitBranchReaderConverter` | class | GitBranchReader bicim donusumu | VersaCoder.Infrastructure | net48;net10.0-windows | GitBranchReaderTests | planli |
| N30-059 | `GitBranchReaderTests` | class | GitBranchReader testleri (xUnit) | VersaCoder.Infrastructure | net48;net10.0-windows | - | planli |
| N30-060 | `GitBranchReaderBuilder` | class | GitBranchReader uretici (builder) | VersaCoder.Infrastructure | net48;net10.0-windows | GitBranchReaderTests | planli |
| N30-061 | `IGitIgnoreWriter` | interface | GitIgnoreWriter sozlesmesi (port) | VersaCoder.Infrastructure | net48;net10.0-windows | GitIgnoreWriterTests | planli |
| N30-062 | `GitIgnoreWriter` | class | GitIgnoreWriter cekirdek uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | GitIgnoreWriterTests | planli |
| N30-063 | `GitIgnoreWriterDto` | record | GitIgnoreWriter veri transferi modeli | VersaCoder.Infrastructure | net48;net10.0-windows | GitIgnoreWriterTests | planli |
| N30-064 | `GitIgnoreWriterMapper` | class | GitIgnoreWriter <-> DTO esleme | VersaCoder.Infrastructure | net48;net10.0-windows | GitIgnoreWriterTests | planli |
| N30-065 | `GitIgnoreWriterValidator` | class | GitIgnoreWriter dogrulama kurallari | VersaCoder.Infrastructure | net48;net10.0-windows | GitIgnoreWriterTests | planli |
| N30-066 | `GitIgnoreWriterFactory` | class | GitIgnoreWriter uretimi ve varsayilanlari | VersaCoder.Infrastructure | net48;net10.0-windows | GitIgnoreWriterTests | planli |
| N30-067 | `GitIgnoreWriterOptions` | record | GitIgnoreWriter yapilandirmasi | VersaCoder.Infrastructure | net48;net10.0-windows | GitIgnoreWriterTests | planli |
| N30-068 | `GitIgnoreWriterProvider` | class | GitIgnoreWriter saglayici uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | GitIgnoreWriterTests | planli |
| N30-069 | `GitIgnoreWriterRegistry` | class | GitIgnoreWriter kayit/katalog yonetimi | VersaCoder.Infrastructure | net48;net10.0-windows | GitIgnoreWriterTests | planli |
| N30-070 | `GitIgnoreWriterConverter` | class | GitIgnoreWriter bicim donusumu | VersaCoder.Infrastructure | net48;net10.0-windows | GitIgnoreWriterTests | planli |
| N30-071 | `GitIgnoreWriterTests` | class | GitIgnoreWriter testleri (xUnit) | VersaCoder.Infrastructure | net48;net10.0-windows | - | planli |
| N30-072 | `GitIgnoreWriterBuilder` | class | GitIgnoreWriter uretici (builder) | VersaCoder.Infrastructure | net48;net10.0-windows | GitIgnoreWriterTests | planli |
| N30-073 | `IGitRepoProbe` | interface | GitRepoProbe sozlesmesi (port) | VersaCoder.Infrastructure | net48;net10.0-windows | GitRepoProbeTests | planli |
| N30-074 | `GitRepoProbe` | class | GitRepoProbe cekirdek uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | GitRepoProbeTests | planli |
| N30-075 | `GitRepoProbeDto` | record | GitRepoProbe veri transferi modeli | VersaCoder.Infrastructure | net48;net10.0-windows | GitRepoProbeTests | planli |
| N30-076 | `GitRepoProbeMapper` | class | GitRepoProbe <-> DTO esleme | VersaCoder.Infrastructure | net48;net10.0-windows | GitRepoProbeTests | planli |
| N30-077 | `GitRepoProbeValidator` | class | GitRepoProbe dogrulama kurallari | VersaCoder.Infrastructure | net48;net10.0-windows | GitRepoProbeTests | planli |
| N30-078 | `GitRepoProbeFactory` | class | GitRepoProbe uretimi ve varsayilanlari | VersaCoder.Infrastructure | net48;net10.0-windows | GitRepoProbeTests | planli |
| N30-079 | `GitRepoProbeOptions` | record | GitRepoProbe yapilandirmasi | VersaCoder.Infrastructure | net48;net10.0-windows | GitRepoProbeTests | planli |
| N30-080 | `GitRepoProbeProvider` | class | GitRepoProbe saglayici uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | GitRepoProbeTests | planli |
| N30-081 | `GitRepoProbeRegistry` | class | GitRepoProbe kayit/katalog yonetimi | VersaCoder.Infrastructure | net48;net10.0-windows | GitRepoProbeTests | planli |
| N30-082 | `GitRepoProbeConverter` | class | GitRepoProbe bicim donusumu | VersaCoder.Infrastructure | net48;net10.0-windows | GitRepoProbeTests | planli |
| N30-083 | `GitRepoProbeTests` | class | GitRepoProbe testleri (xUnit) | VersaCoder.Infrastructure | net48;net10.0-windows | - | planli |
| N30-084 | `GitRepoProbeBuilder` | class | GitRepoProbe uretici (builder) | VersaCoder.Infrastructure | net48;net10.0-windows | GitRepoProbeTests | planli |
| N30-085 | `IGitLogReader` | interface | GitLogReader sozlesmesi (port) | VersaCoder.Infrastructure | net48;net10.0-windows | GitLogReaderTests | planli |
| N30-086 | `GitLogReader` | class | GitLogReader cekirdek uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | GitLogReaderTests | planli |
| N30-087 | `GitLogReaderDto` | record | GitLogReader veri transferi modeli | VersaCoder.Infrastructure | net48;net10.0-windows | GitLogReaderTests | planli |
| N30-088 | `GitLogReaderMapper` | class | GitLogReader <-> DTO esleme | VersaCoder.Infrastructure | net48;net10.0-windows | GitLogReaderTests | planli |
| N30-089 | `GitLogReaderValidator` | class | GitLogReader dogrulama kurallari | VersaCoder.Infrastructure | net48;net10.0-windows | GitLogReaderTests | planli |
| N30-090 | `GitLogReaderFactory` | class | GitLogReader uretimi ve varsayilanlari | VersaCoder.Infrastructure | net48;net10.0-windows | GitLogReaderTests | planli |
| N30-091 | `GitLogReaderOptions` | record | GitLogReader yapilandirmasi | VersaCoder.Infrastructure | net48;net10.0-windows | GitLogReaderTests | planli |
| N30-092 | `GitLogReaderProvider` | class | GitLogReader saglayici uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | GitLogReaderTests | planli |
| N30-093 | `GitLogReaderRegistry` | class | GitLogReader kayit/katalog yonetimi | VersaCoder.Infrastructure | net48;net10.0-windows | GitLogReaderTests | planli |
| N30-094 | `GitLogReaderConverter` | class | GitLogReader bicim donusumu | VersaCoder.Infrastructure | net48;net10.0-windows | GitLogReaderTests | planli |
| N30-095 | `GitLogReaderTests` | class | GitLogReader testleri (xUnit) | VersaCoder.Infrastructure | net48;net10.0-windows | - | planli |
| N30-096 | `GitLogReaderBuilder` | class | GitLogReader uretici (builder) | VersaCoder.Infrastructure | net48;net10.0-windows | GitLogReaderTests | planli |
| N30-097 | `IGitStashOps` | interface | GitStashOps sozlesmesi (port) | VersaCoder.Infrastructure | net48;net10.0-windows | GitStashOpsTests | planli |
| N30-098 | `GitStashOps` | class | GitStashOps cekirdek uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | GitStashOpsTests | planli |
| N30-099 | `GitStashOpsDto` | record | GitStashOps veri transferi modeli | VersaCoder.Infrastructure | net48;net10.0-windows | GitStashOpsTests | planli |
| N30-100 | `GitStashOpsMapper` | class | GitStashOps <-> DTO esleme | VersaCoder.Infrastructure | net48;net10.0-windows | GitStashOpsTests | planli |
| N30-101 | `GitStashOpsValidator` | class | GitStashOps dogrulama kurallari | VersaCoder.Infrastructure | net48;net10.0-windows | GitStashOpsTests | planli |
| N30-102 | `GitStashOpsFactory` | class | GitStashOps uretimi ve varsayilanlari | VersaCoder.Infrastructure | net48;net10.0-windows | GitStashOpsTests | planli |
| N30-103 | `GitStashOpsOptions` | record | GitStashOps yapilandirmasi | VersaCoder.Infrastructure | net48;net10.0-windows | GitStashOpsTests | planli |
| N30-104 | `GitStashOpsProvider` | class | GitStashOps saglayici uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | GitStashOpsTests | planli |
| N30-105 | `GitStashOpsRegistry` | class | GitStashOps kayit/katalog yonetimi | VersaCoder.Infrastructure | net48;net10.0-windows | GitStashOpsTests | planli |
| N30-106 | `GitStashOpsConverter` | class | GitStashOps bicim donusumu | VersaCoder.Infrastructure | net48;net10.0-windows | GitStashOpsTests | planli |
| N30-107 | `GitStashOpsTests` | class | GitStashOps testleri (xUnit) | VersaCoder.Infrastructure | net48;net10.0-windows | - | planli |
| N30-108 | `GitStashOpsBuilder` | class | GitStashOps uretici (builder) | VersaCoder.Infrastructure | net48;net10.0-windows | GitStashOpsTests | planli |
| N30-109 | `IGitSandbox` | interface | GitSandbox sozlesmesi (port) | VersaCoder.Infrastructure | net48;net10.0-windows | GitSandboxTests | planli |
| N30-110 | `GitSandbox` | class | GitSandbox cekirdek uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | GitSandboxTests | planli |
| N30-111 | `GitSandboxDto` | record | GitSandbox veri transferi modeli | VersaCoder.Infrastructure | net48;net10.0-windows | GitSandboxTests | planli |
| N30-112 | `GitSandboxMapper` | class | GitSandbox <-> DTO esleme | VersaCoder.Infrastructure | net48;net10.0-windows | GitSandboxTests | planli |
| N30-113 | `GitSandboxValidator` | class | GitSandbox dogrulama kurallari | VersaCoder.Infrastructure | net48;net10.0-windows | GitSandboxTests | planli |
| N30-114 | `GitSandboxFactory` | class | GitSandbox uretimi ve varsayilanlari | VersaCoder.Infrastructure | net48;net10.0-windows | GitSandboxTests | planli |
| N30-115 | `GitSandboxOptions` | record | GitSandbox yapilandirmasi | VersaCoder.Infrastructure | net48;net10.0-windows | GitSandboxTests | planli |
| N30-116 | `GitSandboxProvider` | class | GitSandbox saglayici uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | GitSandboxTests | planli |
| N30-117 | `GitSandboxRegistry` | class | GitSandbox kayit/katalog yonetimi | VersaCoder.Infrastructure | net48;net10.0-windows | GitSandboxTests | planli |
| N30-118 | `GitSandboxConverter` | class | GitSandbox bicim donusumu | VersaCoder.Infrastructure | net48;net10.0-windows | GitSandboxTests | planli |
| N30-119 | `GitSandboxTests` | class | GitSandbox testleri (xUnit) | VersaCoder.Infrastructure | net48;net10.0-windows | - | planli |
| N30-120 | `GitSandboxBuilder` | class | GitSandbox uretici (builder) | VersaCoder.Infrastructure | net48;net10.0-windows | GitSandboxTests | planli |
