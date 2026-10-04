---
title: "Katalog Dugumu N28 - Migrasyonlar (l4)"
type: catalog
category: architecture
status: needs-validation
version: 1.0.0
updated: 2026-10-04
authority: derived
scope: "N28 migrations dugumu: kimlik, src klasoru, aile yapisi, ornek kayit ve yon kurali (ADR-013 + ADR-028)"
---

# Katalog Dugumu N28 - Migrasyonlar

**Zorunlu Baglantilar:** [catalog index](index.md) · [node-inventory](node-inventory.md) · [mimari index](../index.md) · [ADR-028](../../.decisions/accepted/ADR-028-layered-baseline-9-projects.md) · [ADR-013](../../.decisions/accepted/ADR-013-layered-architecture-52-nodes.md)

> **Tek cumlelik sorumluluk:** *N28 migrations hangi projede ve klasorde yasar, ne yapar, kac kayit hedefler* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

**N28 (migrations), l4 Infrastructure katmaninin alt katmanidir: `VersaCoder.Infrastructure` projesinde `src/VersaCoder.Infrastructure/Migrations/` klasorunde yasar.** Saf SQL migration betiklerini surum tablosuyla sirayla uygular (EF Core migration yok, ADR-005). 10 aile x 12 rol = 120 kayit hedefi (ADR-013 asgari 112); durum `planli`.

Kayitlar ADR-013 2.1 biciminde 8 sutun tasir; bu dosya aile kalibini ve ornek satirlari gosterir, tam 120 satirlik liste bu dosyanin boyut sinirina sigmaz (DOĞRULAMA GEREKLI).

```text
+--------------------------------------------------+
| l4 Infrastructure -> VersaCoder.Infrastructure
| src/VersaCoder.Infrastructure/Migrations/
| 10 aile x 12 rol = 120 kayit (esik 112)
+--------------------------------------------------+
```

| Alan | Deger |
|---|---|
| Dugum | N28 |
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
| 1 | saf SQL migration betiklerini surum tablosuyla sirayla uygulamak | EF Core migration kullanmak veya migration ORM'e devretmek |
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
| 1 | N28 | migrations | `src/VersaCoder.Infrastructure/Migrations/` |

**Kanit yolu (ADR-028 2.3/5):** tarama yolu gercek proje adini icerir; genis kalip (`src/**/Domain/**`) sahte PASS uretir.

```powershell
Test-Path 'C:\versacoder\src\VersaCoder.Infrastructure\Migrations'
```

Dizin su an yok cikti `False` olur; PASS yazilmaz -> **DOĞRULAMA GEREKLI**.

---

## §3 Aile Yapisi

Rol kalibi (12): `I<F>` · `<F>` · `<F>Dto` · `<F>Mapper` · `<F>Validator` · `<F>Factory` · `<F>Options` · `<F>Provider` · `<F>Registry` · `<F>Converter` · `<F>Tests` · `<F>Builder`

| # | Aile | # | Aile |
|---|---|---|---|
| 1 | MigrationRunner | 6 | DownScript |
| 2 | MigrationScript | 7 | MigrationReport |
| 3 | MigrationTable | 8 | SchemaVersion |
| 4 | MigrationFolder | 9 | BaselineSeed |
| 5 | MigrationChecksum | 10 | MigrationLock |

Her aile ayni rol kalibini 12 kez tekrarlar; kayit sayisi = aile x rol.

---

## §4 Kayit Ornegi (8 sutun)

| ID | Bilesen | Tip | Sorumluluk | Proje | TFM | Test | Durum |
|---|---|---|---|---|---|---|---|
| N28-001 | `IMigrationRunner` | interface | MigrationRunner sozlesmesi (port) | VersaCoder.Infrastructure | net48;net10.0-windows | MigrationRunnerTests | planli |
| N28-002 | `MigrationRunner` | class | MigrationRunner cekirdek uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | MigrationRunnerTests | planli |
| N28-003 | `MigrationRunnerDto` | record | MigrationRunner veri transferi modeli | VersaCoder.Infrastructure | net48;net10.0-windows | MigrationRunnerTests | planli |
| N28-004 | `MigrationRunnerMapper` | class | MigrationRunner <-> DTO esleme | VersaCoder.Infrastructure | net48;net10.0-windows | MigrationRunnerTests | planli |
| N28-005 | `MigrationRunnerValidator` | class | MigrationRunner dogrulama kurallari | VersaCoder.Infrastructure | net48;net10.0-windows | MigrationRunnerTests | planli |
| N28-006 | `MigrationRunnerFactory` | class | MigrationRunner uretimi ve varsayilanlari | VersaCoder.Infrastructure | net48;net10.0-windows | MigrationRunnerTests | planli |

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
| 3 | Test sinifi | MigrationRunnerTests (xUnit) | planli |
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
(Select-String -Path 'C:\versacoder\.ai\architecture\catalog\N28-migrations.md' -Pattern '^\|\s*N\d{2}-\d{3}\s*\|').Count
# 2) boyut bandi - beklenen: 2600-8500
(Get-Item 'C:\versacoder\.ai\architecture\catalog\N28-migrations.md').Length
# 3) klasor kaniti - gercek proje adi (ADR-028 2.3/5)
(Test-Path 'C:\versacoder\src\VersaCoder.Infrastructure\Migrations')
```

Katalog sayimi `powershell -NoProfile -ExecutionPolicy Bypass -File C:\versacoder\.ai\scripts\count-components.ps1` ile alinir; bu batch 26 node dosyasiyla 52 dugum klasorunu tamamlar - kayit esigi (5000) ornek satirlarla henuz karsilanmaz (DOĞRULAMA GEREKLI).

## §10 Kayit Tablosu (ADR-013 2.2 - kaynak vault port)

| N28-007 | `MigrationRunnerOptions` | record | MigrationRunner yapilandirmasi | VersaCoder.Infrastructure | net48;net10.0-windows | MigrationRunnerTests | planli |
| N28-008 | `MigrationRunnerProvider` | class | MigrationRunner saglayici uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | MigrationRunnerTests | planli |
| N28-009 | `MigrationRunnerRegistry` | class | MigrationRunner kayit/katalog yonetimi | VersaCoder.Infrastructure | net48;net10.0-windows | MigrationRunnerTests | planli |
| N28-010 | `MigrationRunnerConverter` | class | MigrationRunner bicim donusumu | VersaCoder.Infrastructure | net48;net10.0-windows | MigrationRunnerTests | planli |
| N28-011 | `MigrationRunnerTests` | class | MigrationRunner testleri (xUnit) | VersaCoder.Infrastructure | net48;net10.0-windows | - | planli |
| N28-012 | `MigrationRunnerBuilder` | class | MigrationRunner uretici (builder) | VersaCoder.Infrastructure | net48;net10.0-windows | MigrationRunnerTests | planli |
| N28-013 | `IMigrationScript` | interface | MigrationScript sozlesmesi (port) | VersaCoder.Infrastructure | net48;net10.0-windows | MigrationScriptTests | planli |
| N28-014 | `MigrationScript` | class | MigrationScript cekirdek uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | MigrationScriptTests | planli |
| N28-015 | `MigrationScriptDto` | record | MigrationScript veri transferi modeli | VersaCoder.Infrastructure | net48;net10.0-windows | MigrationScriptTests | planli |
| N28-016 | `MigrationScriptMapper` | class | MigrationScript <-> DTO esleme | VersaCoder.Infrastructure | net48;net10.0-windows | MigrationScriptTests | planli |
| N28-017 | `MigrationScriptValidator` | class | MigrationScript dogrulama kurallari | VersaCoder.Infrastructure | net48;net10.0-windows | MigrationScriptTests | planli |
| N28-018 | `MigrationScriptFactory` | class | MigrationScript uretimi ve varsayilanlari | VersaCoder.Infrastructure | net48;net10.0-windows | MigrationScriptTests | planli |
| N28-019 | `MigrationScriptOptions` | record | MigrationScript yapilandirmasi | VersaCoder.Infrastructure | net48;net10.0-windows | MigrationScriptTests | planli |
| N28-020 | `MigrationScriptProvider` | class | MigrationScript saglayici uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | MigrationScriptTests | planli |
| N28-021 | `MigrationScriptRegistry` | class | MigrationScript kayit/katalog yonetimi | VersaCoder.Infrastructure | net48;net10.0-windows | MigrationScriptTests | planli |
| N28-022 | `MigrationScriptConverter` | class | MigrationScript bicim donusumu | VersaCoder.Infrastructure | net48;net10.0-windows | MigrationScriptTests | planli |
| N28-023 | `MigrationScriptTests` | class | MigrationScript testleri (xUnit) | VersaCoder.Infrastructure | net48;net10.0-windows | - | planli |
| N28-024 | `MigrationScriptBuilder` | class | MigrationScript uretici (builder) | VersaCoder.Infrastructure | net48;net10.0-windows | MigrationScriptTests | planli |
| N28-025 | `IMigrationTable` | interface | MigrationTable sozlesmesi (port) | VersaCoder.Infrastructure | net48;net10.0-windows | MigrationTableTests | planli |
| N28-026 | `MigrationTable` | class | MigrationTable cekirdek uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | MigrationTableTests | planli |
| N28-027 | `MigrationTableDto` | record | MigrationTable veri transferi modeli | VersaCoder.Infrastructure | net48;net10.0-windows | MigrationTableTests | planli |
| N28-028 | `MigrationTableMapper` | class | MigrationTable <-> DTO esleme | VersaCoder.Infrastructure | net48;net10.0-windows | MigrationTableTests | planli |
| N28-029 | `MigrationTableValidator` | class | MigrationTable dogrulama kurallari | VersaCoder.Infrastructure | net48;net10.0-windows | MigrationTableTests | planli |
| N28-030 | `MigrationTableFactory` | class | MigrationTable uretimi ve varsayilanlari | VersaCoder.Infrastructure | net48;net10.0-windows | MigrationTableTests | planli |
| N28-031 | `MigrationTableOptions` | record | MigrationTable yapilandirmasi | VersaCoder.Infrastructure | net48;net10.0-windows | MigrationTableTests | planli |
| N28-032 | `MigrationTableProvider` | class | MigrationTable saglayici uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | MigrationTableTests | planli |
| N28-033 | `MigrationTableRegistry` | class | MigrationTable kayit/katalog yonetimi | VersaCoder.Infrastructure | net48;net10.0-windows | MigrationTableTests | planli |
| N28-034 | `MigrationTableConverter` | class | MigrationTable bicim donusumu | VersaCoder.Infrastructure | net48;net10.0-windows | MigrationTableTests | planli |
| N28-035 | `MigrationTableTests` | class | MigrationTable testleri (xUnit) | VersaCoder.Infrastructure | net48;net10.0-windows | - | planli |
| N28-036 | `MigrationTableBuilder` | class | MigrationTable uretici (builder) | VersaCoder.Infrastructure | net48;net10.0-windows | MigrationTableTests | planli |
| N28-037 | `IMigrationFolder` | interface | MigrationFolder sozlesmesi (port) | VersaCoder.Infrastructure | net48;net10.0-windows | MigrationFolderTests | planli |
| N28-038 | `MigrationFolder` | class | MigrationFolder cekirdek uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | MigrationFolderTests | planli |
| N28-039 | `MigrationFolderDto` | record | MigrationFolder veri transferi modeli | VersaCoder.Infrastructure | net48;net10.0-windows | MigrationFolderTests | planli |
| N28-040 | `MigrationFolderMapper` | class | MigrationFolder <-> DTO esleme | VersaCoder.Infrastructure | net48;net10.0-windows | MigrationFolderTests | planli |
| N28-041 | `MigrationFolderValidator` | class | MigrationFolder dogrulama kurallari | VersaCoder.Infrastructure | net48;net10.0-windows | MigrationFolderTests | planli |
| N28-042 | `MigrationFolderFactory` | class | MigrationFolder uretimi ve varsayilanlari | VersaCoder.Infrastructure | net48;net10.0-windows | MigrationFolderTests | planli |
| N28-043 | `MigrationFolderOptions` | record | MigrationFolder yapilandirmasi | VersaCoder.Infrastructure | net48;net10.0-windows | MigrationFolderTests | planli |
| N28-044 | `MigrationFolderProvider` | class | MigrationFolder saglayici uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | MigrationFolderTests | planli |
| N28-045 | `MigrationFolderRegistry` | class | MigrationFolder kayit/katalog yonetimi | VersaCoder.Infrastructure | net48;net10.0-windows | MigrationFolderTests | planli |
| N28-046 | `MigrationFolderConverter` | class | MigrationFolder bicim donusumu | VersaCoder.Infrastructure | net48;net10.0-windows | MigrationFolderTests | planli |
| N28-047 | `MigrationFolderTests` | class | MigrationFolder testleri (xUnit) | VersaCoder.Infrastructure | net48;net10.0-windows | - | planli |
| N28-048 | `MigrationFolderBuilder` | class | MigrationFolder uretici (builder) | VersaCoder.Infrastructure | net48;net10.0-windows | MigrationFolderTests | planli |
| N28-049 | `IMigrationChecksum` | interface | MigrationChecksum sozlesmesi (port) | VersaCoder.Infrastructure | net48;net10.0-windows | MigrationChecksumTests | planli |
| N28-050 | `MigrationChecksum` | class | MigrationChecksum cekirdek uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | MigrationChecksumTests | planli |
| N28-051 | `MigrationChecksumDto` | record | MigrationChecksum veri transferi modeli | VersaCoder.Infrastructure | net48;net10.0-windows | MigrationChecksumTests | planli |
| N28-052 | `MigrationChecksumMapper` | class | MigrationChecksum <-> DTO esleme | VersaCoder.Infrastructure | net48;net10.0-windows | MigrationChecksumTests | planli |
| N28-053 | `MigrationChecksumValidator` | class | MigrationChecksum dogrulama kurallari | VersaCoder.Infrastructure | net48;net10.0-windows | MigrationChecksumTests | planli |
| N28-054 | `MigrationChecksumFactory` | class | MigrationChecksum uretimi ve varsayilanlari | VersaCoder.Infrastructure | net48;net10.0-windows | MigrationChecksumTests | planli |
| N28-055 | `MigrationChecksumOptions` | record | MigrationChecksum yapilandirmasi | VersaCoder.Infrastructure | net48;net10.0-windows | MigrationChecksumTests | planli |
| N28-056 | `MigrationChecksumProvider` | class | MigrationChecksum saglayici uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | MigrationChecksumTests | planli |
| N28-057 | `MigrationChecksumRegistry` | class | MigrationChecksum kayit/katalog yonetimi | VersaCoder.Infrastructure | net48;net10.0-windows | MigrationChecksumTests | planli |
| N28-058 | `MigrationChecksumConverter` | class | MigrationChecksum bicim donusumu | VersaCoder.Infrastructure | net48;net10.0-windows | MigrationChecksumTests | planli |
| N28-059 | `MigrationChecksumTests` | class | MigrationChecksum testleri (xUnit) | VersaCoder.Infrastructure | net48;net10.0-windows | - | planli |
| N28-060 | `MigrationChecksumBuilder` | class | MigrationChecksum uretici (builder) | VersaCoder.Infrastructure | net48;net10.0-windows | MigrationChecksumTests | planli |
| N28-061 | `IDownScript` | interface | DownScript sozlesmesi (port) | VersaCoder.Infrastructure | net48;net10.0-windows | DownScriptTests | planli |
| N28-062 | `DownScript` | class | DownScript cekirdek uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | DownScriptTests | planli |
| N28-063 | `DownScriptDto` | record | DownScript veri transferi modeli | VersaCoder.Infrastructure | net48;net10.0-windows | DownScriptTests | planli |
| N28-064 | `DownScriptMapper` | class | DownScript <-> DTO esleme | VersaCoder.Infrastructure | net48;net10.0-windows | DownScriptTests | planli |
| N28-065 | `DownScriptValidator` | class | DownScript dogrulama kurallari | VersaCoder.Infrastructure | net48;net10.0-windows | DownScriptTests | planli |
| N28-066 | `DownScriptFactory` | class | DownScript uretimi ve varsayilanlari | VersaCoder.Infrastructure | net48;net10.0-windows | DownScriptTests | planli |
| N28-067 | `DownScriptOptions` | record | DownScript yapilandirmasi | VersaCoder.Infrastructure | net48;net10.0-windows | DownScriptTests | planli |
| N28-068 | `DownScriptProvider` | class | DownScript saglayici uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | DownScriptTests | planli |
| N28-069 | `DownScriptRegistry` | class | DownScript kayit/katalog yonetimi | VersaCoder.Infrastructure | net48;net10.0-windows | DownScriptTests | planli |
| N28-070 | `DownScriptConverter` | class | DownScript bicim donusumu | VersaCoder.Infrastructure | net48;net10.0-windows | DownScriptTests | planli |
| N28-071 | `DownScriptTests` | class | DownScript testleri (xUnit) | VersaCoder.Infrastructure | net48;net10.0-windows | - | planli |
| N28-072 | `DownScriptBuilder` | class | DownScript uretici (builder) | VersaCoder.Infrastructure | net48;net10.0-windows | DownScriptTests | planli |
| N28-073 | `IMigrationReport` | interface | MigrationReport sozlesmesi (port) | VersaCoder.Infrastructure | net48;net10.0-windows | MigrationReportTests | planli |
| N28-074 | `MigrationReport` | class | MigrationReport cekirdek uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | MigrationReportTests | planli |
| N28-075 | `MigrationReportDto` | record | MigrationReport veri transferi modeli | VersaCoder.Infrastructure | net48;net10.0-windows | MigrationReportTests | planli |
| N28-076 | `MigrationReportMapper` | class | MigrationReport <-> DTO esleme | VersaCoder.Infrastructure | net48;net10.0-windows | MigrationReportTests | planli |
| N28-077 | `MigrationReportValidator` | class | MigrationReport dogrulama kurallari | VersaCoder.Infrastructure | net48;net10.0-windows | MigrationReportTests | planli |
| N28-078 | `MigrationReportFactory` | class | MigrationReport uretimi ve varsayilanlari | VersaCoder.Infrastructure | net48;net10.0-windows | MigrationReportTests | planli |
| N28-079 | `MigrationReportOptions` | record | MigrationReport yapilandirmasi | VersaCoder.Infrastructure | net48;net10.0-windows | MigrationReportTests | planli |
| N28-080 | `MigrationReportProvider` | class | MigrationReport saglayici uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | MigrationReportTests | planli |
| N28-081 | `MigrationReportRegistry` | class | MigrationReport kayit/katalog yonetimi | VersaCoder.Infrastructure | net48;net10.0-windows | MigrationReportTests | planli |
| N28-082 | `MigrationReportConverter` | class | MigrationReport bicim donusumu | VersaCoder.Infrastructure | net48;net10.0-windows | MigrationReportTests | planli |
| N28-083 | `MigrationReportTests` | class | MigrationReport testleri (xUnit) | VersaCoder.Infrastructure | net48;net10.0-windows | - | planli |
| N28-084 | `MigrationReportBuilder` | class | MigrationReport uretici (builder) | VersaCoder.Infrastructure | net48;net10.0-windows | MigrationReportTests | planli |
| N28-085 | `ISchemaVersion` | interface | SchemaVersion sozlesmesi (port) | VersaCoder.Infrastructure | net48;net10.0-windows | SchemaVersionTests | planli |
| N28-086 | `SchemaVersion` | class | SchemaVersion cekirdek uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | SchemaVersionTests | planli |
| N28-087 | `SchemaVersionDto` | record | SchemaVersion veri transferi modeli | VersaCoder.Infrastructure | net48;net10.0-windows | SchemaVersionTests | planli |
| N28-088 | `SchemaVersionMapper` | class | SchemaVersion <-> DTO esleme | VersaCoder.Infrastructure | net48;net10.0-windows | SchemaVersionTests | planli |
| N28-089 | `SchemaVersionValidator` | class | SchemaVersion dogrulama kurallari | VersaCoder.Infrastructure | net48;net10.0-windows | SchemaVersionTests | planli |
| N28-090 | `SchemaVersionFactory` | class | SchemaVersion uretimi ve varsayilanlari | VersaCoder.Infrastructure | net48;net10.0-windows | SchemaVersionTests | planli |
| N28-091 | `SchemaVersionOptions` | record | SchemaVersion yapilandirmasi | VersaCoder.Infrastructure | net48;net10.0-windows | SchemaVersionTests | planli |
| N28-092 | `SchemaVersionProvider` | class | SchemaVersion saglayici uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | SchemaVersionTests | planli |
| N28-093 | `SchemaVersionRegistry` | class | SchemaVersion kayit/katalog yonetimi | VersaCoder.Infrastructure | net48;net10.0-windows | SchemaVersionTests | planli |
| N28-094 | `SchemaVersionConverter` | class | SchemaVersion bicim donusumu | VersaCoder.Infrastructure | net48;net10.0-windows | SchemaVersionTests | planli |
| N28-095 | `SchemaVersionTests` | class | SchemaVersion testleri (xUnit) | VersaCoder.Infrastructure | net48;net10.0-windows | - | planli |
| N28-096 | `SchemaVersionBuilder` | class | SchemaVersion uretici (builder) | VersaCoder.Infrastructure | net48;net10.0-windows | SchemaVersionTests | planli |
| N28-097 | `IBaselineSeed` | interface | BaselineSeed sozlesmesi (port) | VersaCoder.Infrastructure | net48;net10.0-windows | BaselineSeedTests | planli |
| N28-098 | `BaselineSeed` | class | BaselineSeed cekirdek uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | BaselineSeedTests | planli |
| N28-099 | `BaselineSeedDto` | record | BaselineSeed veri transferi modeli | VersaCoder.Infrastructure | net48;net10.0-windows | BaselineSeedTests | planli |
| N28-100 | `BaselineSeedMapper` | class | BaselineSeed <-> DTO esleme | VersaCoder.Infrastructure | net48;net10.0-windows | BaselineSeedTests | planli |
| N28-101 | `BaselineSeedValidator` | class | BaselineSeed dogrulama kurallari | VersaCoder.Infrastructure | net48;net10.0-windows | BaselineSeedTests | planli |
| N28-102 | `BaselineSeedFactory` | class | BaselineSeed uretimi ve varsayilanlari | VersaCoder.Infrastructure | net48;net10.0-windows | BaselineSeedTests | planli |
| N28-103 | `BaselineSeedOptions` | record | BaselineSeed yapilandirmasi | VersaCoder.Infrastructure | net48;net10.0-windows | BaselineSeedTests | planli |
| N28-104 | `BaselineSeedProvider` | class | BaselineSeed saglayici uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | BaselineSeedTests | planli |
| N28-105 | `BaselineSeedRegistry` | class | BaselineSeed kayit/katalog yonetimi | VersaCoder.Infrastructure | net48;net10.0-windows | BaselineSeedTests | planli |
| N28-106 | `BaselineSeedConverter` | class | BaselineSeed bicim donusumu | VersaCoder.Infrastructure | net48;net10.0-windows | BaselineSeedTests | planli |
| N28-107 | `BaselineSeedTests` | class | BaselineSeed testleri (xUnit) | VersaCoder.Infrastructure | net48;net10.0-windows | - | planli |
| N28-108 | `BaselineSeedBuilder` | class | BaselineSeed uretici (builder) | VersaCoder.Infrastructure | net48;net10.0-windows | BaselineSeedTests | planli |
| N28-109 | `IMigrationLock` | interface | MigrationLock sozlesmesi (port) | VersaCoder.Infrastructure | net48;net10.0-windows | MigrationLockTests | planli |
| N28-110 | `MigrationLock` | class | MigrationLock cekirdek uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | MigrationLockTests | planli |
| N28-111 | `MigrationLockDto` | record | MigrationLock veri transferi modeli | VersaCoder.Infrastructure | net48;net10.0-windows | MigrationLockTests | planli |
| N28-112 | `MigrationLockMapper` | class | MigrationLock <-> DTO esleme | VersaCoder.Infrastructure | net48;net10.0-windows | MigrationLockTests | planli |
| N28-113 | `MigrationLockValidator` | class | MigrationLock dogrulama kurallari | VersaCoder.Infrastructure | net48;net10.0-windows | MigrationLockTests | planli |
| N28-114 | `MigrationLockFactory` | class | MigrationLock uretimi ve varsayilanlari | VersaCoder.Infrastructure | net48;net10.0-windows | MigrationLockTests | planli |
| N28-115 | `MigrationLockOptions` | record | MigrationLock yapilandirmasi | VersaCoder.Infrastructure | net48;net10.0-windows | MigrationLockTests | planli |
| N28-116 | `MigrationLockProvider` | class | MigrationLock saglayici uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | MigrationLockTests | planli |
| N28-117 | `MigrationLockRegistry` | class | MigrationLock kayit/katalog yonetimi | VersaCoder.Infrastructure | net48;net10.0-windows | MigrationLockTests | planli |
| N28-118 | `MigrationLockConverter` | class | MigrationLock bicim donusumu | VersaCoder.Infrastructure | net48;net10.0-windows | MigrationLockTests | planli |
| N28-119 | `MigrationLockTests` | class | MigrationLock testleri (xUnit) | VersaCoder.Infrastructure | net48;net10.0-windows | - | planli |
| N28-120 | `MigrationLockBuilder` | class | MigrationLock uretici (builder) | VersaCoder.Infrastructure | net48;net10.0-windows | MigrationLockTests | planli |
