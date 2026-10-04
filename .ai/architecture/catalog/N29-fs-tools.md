---
title: "Katalog Dugumu N29 - Dosya Sistem Araclari (l4)"
type: catalog
category: architecture
status: needs-validation
version: 1.0.0
updated: 2026-10-04
authority: derived
scope: "N29 fs-tools dugumu: kimlik, src klasoru, aile yapisi, ornek kayit ve yon kurali (ADR-013 + ADR-028)"
---

# Katalog Dugumu N29 - Dosya Sistem Araclari

**Zorunlu Baglantilar:** [catalog index](index.md) · [node-inventory](node-inventory.md) · [mimari index](../index.md) · [ADR-028](../../.decisions/accepted/ADR-028-layered-baseline-9-projects.md) · [ADR-013](../../.decisions/accepted/ADR-013-layered-architecture-52-nodes.md)

> **Tek cumlelik sorumluluk:** *N29 fs-tools hangi projede ve klasorde yasar, ne yapar, kac kayit hedefler* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

**N29 (fs-tools), l4 Infrastructure katmaninin alt katmanidir: `VersaCoder.Infrastructure` projesinde `src/VersaCoder.Infrastructure/FileSystem/` klasorunde yasar.** Dosya okuma/yazma, izleme ve atomik yazma islerini guvenli sarar. 10 aile x 12 rol = 120 kayit hedefi (ADR-013 asgari 112); durum `planli`.

Kayitlar ADR-013 2.1 biciminde 8 sutun tasir; bu dosya aile kalibini ve ornek satirlari gosterir, tam 120 satirlik liste bu dosyanin boyut sinirina sigmaz (DOĞRULAMA GEREKLI).

```text
+--------------------------------------------------+
| l4 Infrastructure -> VersaCoder.Infrastructure
| src/VersaCoder.Infrastructure/FileSystem/
| 10 aile x 12 rol = 120 kayit (esik 112)
+--------------------------------------------------+
```

| Alan | Deger |
|---|---|
| Dugum | N29 |
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
| 1 | dosya okuma/yazma, izleme ve atomik yazmayi guvenli sarmak | protokol veya mesaj bicimi uretmek (l5 isidir) |
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
| 1 | N29 | fs-tools | `src/VersaCoder.Infrastructure/FileSystem/` |

**Kanit yolu (ADR-028 2.3/5):** tarama yolu gercek proje adini icerir; genis kalip (`src/**/Domain/**`) sahte PASS uretir.

```powershell
Test-Path 'C:\versacoder\src\VersaCoder.Infrastructure\FileSystem'
```

Dizin su an yok cikti `False` olur; PASS yazilmaz -> **DOĞRULAMA GEREKLI**.

---

## §3 Aile Yapisi

Rol kalibi (12): `I<F>` · `<F>` · `<F>Dto` · `<F>Mapper` · `<F>Validator` · `<F>Factory` · `<F>Options` · `<F>Provider` · `<F>Registry` · `<F>Converter` · `<F>Tests` · `<F>Builder`

| # | Aile | # | Aile |
|---|---|---|---|
| 1 | FileSystemStore | 6 | TextDecoder |
| 2 | PathResolver | 7 | BinaryGuard |
| 3 | FileWatcher | 8 | GlobScanner |
| 4 | AtomicWriter | 9 | DirWalker |
| 5 | TempScope | 10 | FileSizeGuard |

Her aile ayni rol kalibini 12 kez tekrarlar; kayit sayisi = aile x rol.

---

## §4 Kayit Ornegi (8 sutun)

| ID | Bilesen | Tip | Sorumluluk | Proje | TFM | Test | Durum |
|---|---|---|---|---|---|---|---|
| N29-001 | `IFileSystemStore` | interface | FileSystemStore sozlesmesi (port) | VersaCoder.Infrastructure | net48;net10.0-windows | FileSystemStoreTests | planli |
| N29-002 | `FileSystemStore` | class | FileSystemStore cekirdek uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | FileSystemStoreTests | planli |
| N29-003 | `FileSystemStoreDto` | record | FileSystemStore veri transferi modeli | VersaCoder.Infrastructure | net48;net10.0-windows | FileSystemStoreTests | planli |
| N29-004 | `FileSystemStoreMapper` | class | FileSystemStore <-> DTO esleme | VersaCoder.Infrastructure | net48;net10.0-windows | FileSystemStoreTests | planli |
| N29-005 | `FileSystemStoreValidator` | class | FileSystemStore dogrulama kurallari | VersaCoder.Infrastructure | net48;net10.0-windows | FileSystemStoreTests | planli |
| N29-006 | `FileSystemStoreFactory` | class | FileSystemStore uretimi ve varsayilanlari | VersaCoder.Infrastructure | net48;net10.0-windows | FileSystemStoreTests | planli |

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
| 3 | Test sinifi | FileSystemStoreTests (xUnit) | planli |
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
(Select-String -Path 'C:\versacoder\.ai\architecture\catalog\N29-fs-tools.md' -Pattern '^\|\s*N\d{2}-\d{3}\s*\|').Count
# 2) boyut bandi - beklenen: 2600-8500
(Get-Item 'C:\versacoder\.ai\architecture\catalog\N29-fs-tools.md').Length
# 3) klasor kaniti - gercek proje adi (ADR-028 2.3/5)
(Test-Path 'C:\versacoder\src\VersaCoder.Infrastructure\FileSystem')
```

Katalog sayimi `powershell -NoProfile -ExecutionPolicy Bypass -File C:\versacoder\.ai\scripts\count-components.ps1` ile alinir; bu batch 26 node dosyasiyla 52 dugum klasorunu tamamlar - kayit esigi (5000) ornek satirlarla henuz karsilanmaz (DOĞRULAMA GEREKLI).

## §10 Kayit Tablosu (ADR-013 2.2 - kaynak vault port)

| N29-007 | `FileSystemStoreOptions` | record | FileSystemStore yapilandirmasi | VersaCoder.Infrastructure | net48;net10.0-windows | FileSystemStoreTests | planli |
| N29-008 | `FileSystemStoreProvider` | class | FileSystemStore saglayici uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | FileSystemStoreTests | planli |
| N29-009 | `FileSystemStoreRegistry` | class | FileSystemStore kayit/katalog yonetimi | VersaCoder.Infrastructure | net48;net10.0-windows | FileSystemStoreTests | planli |
| N29-010 | `FileSystemStoreConverter` | class | FileSystemStore bicim donusumu | VersaCoder.Infrastructure | net48;net10.0-windows | FileSystemStoreTests | planli |
| N29-011 | `FileSystemStoreTests` | class | FileSystemStore testleri (xUnit) | VersaCoder.Infrastructure | net48;net10.0-windows | - | planli |
| N29-012 | `FileSystemStoreBuilder` | class | FileSystemStore uretici (builder) | VersaCoder.Infrastructure | net48;net10.0-windows | FileSystemStoreTests | planli |
| N29-013 | `IPathResolver` | interface | PathResolver sozlesmesi (port) | VersaCoder.Infrastructure | net48;net10.0-windows | PathResolverTests | planli |
| N29-014 | `PathResolver` | class | PathResolver cekirdek uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | PathResolverTests | planli |
| N29-015 | `PathResolverDto` | record | PathResolver veri transferi modeli | VersaCoder.Infrastructure | net48;net10.0-windows | PathResolverTests | planli |
| N29-016 | `PathResolverMapper` | class | PathResolver <-> DTO esleme | VersaCoder.Infrastructure | net48;net10.0-windows | PathResolverTests | planli |
| N29-017 | `PathResolverValidator` | class | PathResolver dogrulama kurallari | VersaCoder.Infrastructure | net48;net10.0-windows | PathResolverTests | planli |
| N29-018 | `PathResolverFactory` | class | PathResolver uretimi ve varsayilanlari | VersaCoder.Infrastructure | net48;net10.0-windows | PathResolverTests | planli |
| N29-019 | `PathResolverOptions` | record | PathResolver yapilandirmasi | VersaCoder.Infrastructure | net48;net10.0-windows | PathResolverTests | planli |
| N29-020 | `PathResolverProvider` | class | PathResolver saglayici uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | PathResolverTests | planli |
| N29-021 | `PathResolverRegistry` | class | PathResolver kayit/katalog yonetimi | VersaCoder.Infrastructure | net48;net10.0-windows | PathResolverTests | planli |
| N29-022 | `PathResolverConverter` | class | PathResolver bicim donusumu | VersaCoder.Infrastructure | net48;net10.0-windows | PathResolverTests | planli |
| N29-023 | `PathResolverTests` | class | PathResolver testleri (xUnit) | VersaCoder.Infrastructure | net48;net10.0-windows | - | planli |
| N29-024 | `PathResolverBuilder` | class | PathResolver uretici (builder) | VersaCoder.Infrastructure | net48;net10.0-windows | PathResolverTests | planli |
| N29-025 | `IFileWatcher` | interface | FileWatcher sozlesmesi (port) | VersaCoder.Infrastructure | net48;net10.0-windows | FileWatcherTests | planli |
| N29-026 | `FileWatcher` | class | FileWatcher cekirdek uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | FileWatcherTests | planli |
| N29-027 | `FileWatcherDto` | record | FileWatcher veri transferi modeli | VersaCoder.Infrastructure | net48;net10.0-windows | FileWatcherTests | planli |
| N29-028 | `FileWatcherMapper` | class | FileWatcher <-> DTO esleme | VersaCoder.Infrastructure | net48;net10.0-windows | FileWatcherTests | planli |
| N29-029 | `FileWatcherValidator` | class | FileWatcher dogrulama kurallari | VersaCoder.Infrastructure | net48;net10.0-windows | FileWatcherTests | planli |
| N29-030 | `FileWatcherFactory` | class | FileWatcher uretimi ve varsayilanlari | VersaCoder.Infrastructure | net48;net10.0-windows | FileWatcherTests | planli |
| N29-031 | `FileWatcherOptions` | record | FileWatcher yapilandirmasi | VersaCoder.Infrastructure | net48;net10.0-windows | FileWatcherTests | planli |
| N29-032 | `FileWatcherProvider` | class | FileWatcher saglayici uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | FileWatcherTests | planli |
| N29-033 | `FileWatcherRegistry` | class | FileWatcher kayit/katalog yonetimi | VersaCoder.Infrastructure | net48;net10.0-windows | FileWatcherTests | planli |
| N29-034 | `FileWatcherConverter` | class | FileWatcher bicim donusumu | VersaCoder.Infrastructure | net48;net10.0-windows | FileWatcherTests | planli |
| N29-035 | `FileWatcherTests` | class | FileWatcher testleri (xUnit) | VersaCoder.Infrastructure | net48;net10.0-windows | - | planli |
| N29-036 | `FileWatcherBuilder` | class | FileWatcher uretici (builder) | VersaCoder.Infrastructure | net48;net10.0-windows | FileWatcherTests | planli |
| N29-037 | `IAtomicWriter` | interface | AtomicWriter sozlesmesi (port) | VersaCoder.Infrastructure | net48;net10.0-windows | AtomicWriterTests | planli |
| N29-038 | `AtomicWriter` | class | AtomicWriter cekirdek uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | AtomicWriterTests | planli |
| N29-039 | `AtomicWriterDto` | record | AtomicWriter veri transferi modeli | VersaCoder.Infrastructure | net48;net10.0-windows | AtomicWriterTests | planli |
| N29-040 | `AtomicWriterMapper` | class | AtomicWriter <-> DTO esleme | VersaCoder.Infrastructure | net48;net10.0-windows | AtomicWriterTests | planli |
| N29-041 | `AtomicWriterValidator` | class | AtomicWriter dogrulama kurallari | VersaCoder.Infrastructure | net48;net10.0-windows | AtomicWriterTests | planli |
| N29-042 | `AtomicWriterFactory` | class | AtomicWriter uretimi ve varsayilanlari | VersaCoder.Infrastructure | net48;net10.0-windows | AtomicWriterTests | planli |
| N29-043 | `AtomicWriterOptions` | record | AtomicWriter yapilandirmasi | VersaCoder.Infrastructure | net48;net10.0-windows | AtomicWriterTests | planli |
| N29-044 | `AtomicWriterProvider` | class | AtomicWriter saglayici uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | AtomicWriterTests | planli |
| N29-045 | `AtomicWriterRegistry` | class | AtomicWriter kayit/katalog yonetimi | VersaCoder.Infrastructure | net48;net10.0-windows | AtomicWriterTests | planli |
| N29-046 | `AtomicWriterConverter` | class | AtomicWriter bicim donusumu | VersaCoder.Infrastructure | net48;net10.0-windows | AtomicWriterTests | planli |
| N29-047 | `AtomicWriterTests` | class | AtomicWriter testleri (xUnit) | VersaCoder.Infrastructure | net48;net10.0-windows | - | planli |
| N29-048 | `AtomicWriterBuilder` | class | AtomicWriter uretici (builder) | VersaCoder.Infrastructure | net48;net10.0-windows | AtomicWriterTests | planli |
| N29-049 | `ITempScope` | interface | TempScope sozlesmesi (port) | VersaCoder.Infrastructure | net48;net10.0-windows | TempScopeTests | planli |
| N29-050 | `TempScope` | class | TempScope cekirdek uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | TempScopeTests | planli |
| N29-051 | `TempScopeDto` | record | TempScope veri transferi modeli | VersaCoder.Infrastructure | net48;net10.0-windows | TempScopeTests | planli |
| N29-052 | `TempScopeMapper` | class | TempScope <-> DTO esleme | VersaCoder.Infrastructure | net48;net10.0-windows | TempScopeTests | planli |
| N29-053 | `TempScopeValidator` | class | TempScope dogrulama kurallari | VersaCoder.Infrastructure | net48;net10.0-windows | TempScopeTests | planli |
| N29-054 | `TempScopeFactory` | class | TempScope uretimi ve varsayilanlari | VersaCoder.Infrastructure | net48;net10.0-windows | TempScopeTests | planli |
| N29-055 | `TempScopeOptions` | record | TempScope yapilandirmasi | VersaCoder.Infrastructure | net48;net10.0-windows | TempScopeTests | planli |
| N29-056 | `TempScopeProvider` | class | TempScope saglayici uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | TempScopeTests | planli |
| N29-057 | `TempScopeRegistry` | class | TempScope kayit/katalog yonetimi | VersaCoder.Infrastructure | net48;net10.0-windows | TempScopeTests | planli |
| N29-058 | `TempScopeConverter` | class | TempScope bicim donusumu | VersaCoder.Infrastructure | net48;net10.0-windows | TempScopeTests | planli |
| N29-059 | `TempScopeTests` | class | TempScope testleri (xUnit) | VersaCoder.Infrastructure | net48;net10.0-windows | - | planli |
| N29-060 | `TempScopeBuilder` | class | TempScope uretici (builder) | VersaCoder.Infrastructure | net48;net10.0-windows | TempScopeTests | planli |
| N29-061 | `ITextDecoder` | interface | TextDecoder sozlesmesi (port) | VersaCoder.Infrastructure | net48;net10.0-windows | TextDecoderTests | planli |
| N29-062 | `TextDecoder` | class | TextDecoder cekirdek uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | TextDecoderTests | planli |
| N29-063 | `TextDecoderDto` | record | TextDecoder veri transferi modeli | VersaCoder.Infrastructure | net48;net10.0-windows | TextDecoderTests | planli |
| N29-064 | `TextDecoderMapper` | class | TextDecoder <-> DTO esleme | VersaCoder.Infrastructure | net48;net10.0-windows | TextDecoderTests | planli |
| N29-065 | `TextDecoderValidator` | class | TextDecoder dogrulama kurallari | VersaCoder.Infrastructure | net48;net10.0-windows | TextDecoderTests | planli |
| N29-066 | `TextDecoderFactory` | class | TextDecoder uretimi ve varsayilanlari | VersaCoder.Infrastructure | net48;net10.0-windows | TextDecoderTests | planli |
| N29-067 | `TextDecoderOptions` | record | TextDecoder yapilandirmasi | VersaCoder.Infrastructure | net48;net10.0-windows | TextDecoderTests | planli |
| N29-068 | `TextDecoderProvider` | class | TextDecoder saglayici uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | TextDecoderTests | planli |
| N29-069 | `TextDecoderRegistry` | class | TextDecoder kayit/katalog yonetimi | VersaCoder.Infrastructure | net48;net10.0-windows | TextDecoderTests | planli |
| N29-070 | `TextDecoderConverter` | class | TextDecoder bicim donusumu | VersaCoder.Infrastructure | net48;net10.0-windows | TextDecoderTests | planli |
| N29-071 | `TextDecoderTests` | class | TextDecoder testleri (xUnit) | VersaCoder.Infrastructure | net48;net10.0-windows | - | planli |
| N29-072 | `TextDecoderBuilder` | class | TextDecoder uretici (builder) | VersaCoder.Infrastructure | net48;net10.0-windows | TextDecoderTests | planli |
| N29-073 | `IBinaryGuard` | interface | BinaryGuard sozlesmesi (port) | VersaCoder.Infrastructure | net48;net10.0-windows | BinaryGuardTests | planli |
| N29-074 | `BinaryGuard` | class | BinaryGuard cekirdek uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | BinaryGuardTests | planli |
| N29-075 | `BinaryGuardDto` | record | BinaryGuard veri transferi modeli | VersaCoder.Infrastructure | net48;net10.0-windows | BinaryGuardTests | planli |
| N29-076 | `BinaryGuardMapper` | class | BinaryGuard <-> DTO esleme | VersaCoder.Infrastructure | net48;net10.0-windows | BinaryGuardTests | planli |
| N29-077 | `BinaryGuardValidator` | class | BinaryGuard dogrulama kurallari | VersaCoder.Infrastructure | net48;net10.0-windows | BinaryGuardTests | planli |
| N29-078 | `BinaryGuardFactory` | class | BinaryGuard uretimi ve varsayilanlari | VersaCoder.Infrastructure | net48;net10.0-windows | BinaryGuardTests | planli |
| N29-079 | `BinaryGuardOptions` | record | BinaryGuard yapilandirmasi | VersaCoder.Infrastructure | net48;net10.0-windows | BinaryGuardTests | planli |
| N29-080 | `BinaryGuardProvider` | class | BinaryGuard saglayici uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | BinaryGuardTests | planli |
| N29-081 | `BinaryGuardRegistry` | class | BinaryGuard kayit/katalog yonetimi | VersaCoder.Infrastructure | net48;net10.0-windows | BinaryGuardTests | planli |
| N29-082 | `BinaryGuardConverter` | class | BinaryGuard bicim donusumu | VersaCoder.Infrastructure | net48;net10.0-windows | BinaryGuardTests | planli |
| N29-083 | `BinaryGuardTests` | class | BinaryGuard testleri (xUnit) | VersaCoder.Infrastructure | net48;net10.0-windows | - | planli |
| N29-084 | `BinaryGuardBuilder` | class | BinaryGuard uretici (builder) | VersaCoder.Infrastructure | net48;net10.0-windows | BinaryGuardTests | planli |
| N29-085 | `IGlobScanner` | interface | GlobScanner sozlesmesi (port) | VersaCoder.Infrastructure | net48;net10.0-windows | GlobScannerTests | planli |
| N29-086 | `GlobScanner` | class | GlobScanner cekirdek uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | GlobScannerTests | planli |
| N29-087 | `GlobScannerDto` | record | GlobScanner veri transferi modeli | VersaCoder.Infrastructure | net48;net10.0-windows | GlobScannerTests | planli |
| N29-088 | `GlobScannerMapper` | class | GlobScanner <-> DTO esleme | VersaCoder.Infrastructure | net48;net10.0-windows | GlobScannerTests | planli |
| N29-089 | `GlobScannerValidator` | class | GlobScanner dogrulama kurallari | VersaCoder.Infrastructure | net48;net10.0-windows | GlobScannerTests | planli |
| N29-090 | `GlobScannerFactory` | class | GlobScanner uretimi ve varsayilanlari | VersaCoder.Infrastructure | net48;net10.0-windows | GlobScannerTests | planli |
| N29-091 | `GlobScannerOptions` | record | GlobScanner yapilandirmasi | VersaCoder.Infrastructure | net48;net10.0-windows | GlobScannerTests | planli |
| N29-092 | `GlobScannerProvider` | class | GlobScanner saglayici uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | GlobScannerTests | planli |
| N29-093 | `GlobScannerRegistry` | class | GlobScanner kayit/katalog yonetimi | VersaCoder.Infrastructure | net48;net10.0-windows | GlobScannerTests | planli |
| N29-094 | `GlobScannerConverter` | class | GlobScanner bicim donusumu | VersaCoder.Infrastructure | net48;net10.0-windows | GlobScannerTests | planli |
| N29-095 | `GlobScannerTests` | class | GlobScanner testleri (xUnit) | VersaCoder.Infrastructure | net48;net10.0-windows | - | planli |
| N29-096 | `GlobScannerBuilder` | class | GlobScanner uretici (builder) | VersaCoder.Infrastructure | net48;net10.0-windows | GlobScannerTests | planli |
| N29-097 | `IDirWalker` | interface | DirWalker sozlesmesi (port) | VersaCoder.Infrastructure | net48;net10.0-windows | DirWalkerTests | planli |
| N29-098 | `DirWalker` | class | DirWalker cekirdek uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | DirWalkerTests | planli |
| N29-099 | `DirWalkerDto` | record | DirWalker veri transferi modeli | VersaCoder.Infrastructure | net48;net10.0-windows | DirWalkerTests | planli |
| N29-100 | `DirWalkerMapper` | class | DirWalker <-> DTO esleme | VersaCoder.Infrastructure | net48;net10.0-windows | DirWalkerTests | planli |
| N29-101 | `DirWalkerValidator` | class | DirWalker dogrulama kurallari | VersaCoder.Infrastructure | net48;net10.0-windows | DirWalkerTests | planli |
| N29-102 | `DirWalkerFactory` | class | DirWalker uretimi ve varsayilanlari | VersaCoder.Infrastructure | net48;net10.0-windows | DirWalkerTests | planli |
| N29-103 | `DirWalkerOptions` | record | DirWalker yapilandirmasi | VersaCoder.Infrastructure | net48;net10.0-windows | DirWalkerTests | planli |
| N29-104 | `DirWalkerProvider` | class | DirWalker saglayici uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | DirWalkerTests | planli |
| N29-105 | `DirWalkerRegistry` | class | DirWalker kayit/katalog yonetimi | VersaCoder.Infrastructure | net48;net10.0-windows | DirWalkerTests | planli |
| N29-106 | `DirWalkerConverter` | class | DirWalker bicim donusumu | VersaCoder.Infrastructure | net48;net10.0-windows | DirWalkerTests | planli |
| N29-107 | `DirWalkerTests` | class | DirWalker testleri (xUnit) | VersaCoder.Infrastructure | net48;net10.0-windows | - | planli |
| N29-108 | `DirWalkerBuilder` | class | DirWalker uretici (builder) | VersaCoder.Infrastructure | net48;net10.0-windows | DirWalkerTests | planli |
| N29-109 | `IFileSizeGuard` | interface | FileSizeGuard sozlesmesi (port) | VersaCoder.Infrastructure | net48;net10.0-windows | FileSizeGuardTests | planli |
| N29-110 | `FileSizeGuard` | class | FileSizeGuard cekirdek uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | FileSizeGuardTests | planli |
| N29-111 | `FileSizeGuardDto` | record | FileSizeGuard veri transferi modeli | VersaCoder.Infrastructure | net48;net10.0-windows | FileSizeGuardTests | planli |
| N29-112 | `FileSizeGuardMapper` | class | FileSizeGuard <-> DTO esleme | VersaCoder.Infrastructure | net48;net10.0-windows | FileSizeGuardTests | planli |
| N29-113 | `FileSizeGuardValidator` | class | FileSizeGuard dogrulama kurallari | VersaCoder.Infrastructure | net48;net10.0-windows | FileSizeGuardTests | planli |
| N29-114 | `FileSizeGuardFactory` | class | FileSizeGuard uretimi ve varsayilanlari | VersaCoder.Infrastructure | net48;net10.0-windows | FileSizeGuardTests | planli |
| N29-115 | `FileSizeGuardOptions` | record | FileSizeGuard yapilandirmasi | VersaCoder.Infrastructure | net48;net10.0-windows | FileSizeGuardTests | planli |
| N29-116 | `FileSizeGuardProvider` | class | FileSizeGuard saglayici uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | FileSizeGuardTests | planli |
| N29-117 | `FileSizeGuardRegistry` | class | FileSizeGuard kayit/katalog yonetimi | VersaCoder.Infrastructure | net48;net10.0-windows | FileSizeGuardTests | planli |
| N29-118 | `FileSizeGuardConverter` | class | FileSizeGuard bicim donusumu | VersaCoder.Infrastructure | net48;net10.0-windows | FileSizeGuardTests | planli |
| N29-119 | `FileSizeGuardTests` | class | FileSizeGuard testleri (xUnit) | VersaCoder.Infrastructure | net48;net10.0-windows | - | planli |
| N29-120 | `FileSizeGuardBuilder` | class | FileSizeGuard uretici (builder) | VersaCoder.Infrastructure | net48;net10.0-windows | FileSizeGuardTests | planli |
