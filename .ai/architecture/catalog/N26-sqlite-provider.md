---
title: "Katalog Dugumu N26 - Sqlite Saglayici (l4)"
type: catalog
category: architecture
status: needs-validation
version: 1.0.0
updated: 2026-10-04
authority: derived
scope: "N26 sqlite-provider dugumu: kimlik, src klasoru, aile yapisi, ornek kayit ve yon kurali (ADR-013 + ADR-028)"
---

# Katalog Dugumu N26 - Sqlite Saglayici

**Zorunlu Baglantilar:** [catalog index](index.md) · [node-inventory](node-inventory.md) · [mimari index](../index.md) · [ADR-028](../../.decisions/accepted/ADR-028-layered-baseline-9-projects.md) · [ADR-013](../../.decisions/accepted/ADR-013-layered-architecture-52-nodes.md)

> **Tek cumlelik sorumluluk:** *N26 sqlite-provider hangi projede ve klasorde yasar, ne yapar, kac kayit hedefler* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

**N26 (sqlite-provider), l4 Infrastructure katmaninin alt katmanidir: `VersaCoder.Infrastructure` projesinde `src/VersaCoder.Infrastructure/Data/Sqlite/` klasorunde yasar.** Yerel SQLite dosya veritabaninin baglanti, pragma ve journal katmanini yonetir. 10 aile x 12 rol = 120 kayit hedefi (ADR-013 asgari 112); durum `planli`.

Kayitlar ADR-013 2.1 biciminde 8 sutun tasir; bu dosya aile kalibini ve ornek satirlari gosterir, tam 120 satirlik liste bu dosyanin boyut sinirina sigmaz (DOĞRULAMA GEREKLI).

```text
+--------------------------------------------------+
| l4 Infrastructure -> VersaCoder.Infrastructure
| src/VersaCoder.Infrastructure/Data/Sqlite/
| 10 aile x 12 rol = 120 kayit (esik 112)
+--------------------------------------------------+
```

| Alan | Deger |
|---|---|
| Dugum | N26 |
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
| 1 | SQLite baglantisi, pragma ve journal katmanini yonetmek | sunucu taraf SQL yazmak (SqlServer saglayicisi N27 isidir) |
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
| 1 | N26 | sqlite-provider | `src/VersaCoder.Infrastructure/Data/Sqlite/` |

**Kanit yolu (ADR-028 2.3/5):** tarama yolu gercek proje adini icerir; genis kalip (`src/**/Domain/**`) sahte PASS uretir.

```powershell
Test-Path 'C:\versacoder\src\VersaCoder.Infrastructure\Data\Sqlite'
```

Dizin su an yok cikti `False` olur; PASS yazilmaz -> **DOĞRULAMA GEREKLI**.

---

## §3 Aile Yapisi

Rol kalibi (12): `I<F>` · `<F>` · `<F>Dto` · `<F>Mapper` · `<F>Validator` · `<F>Factory` · `<F>Options` · `<F>Provider` · `<F>Registry` · `<F>Converter` · `<F>Tests` · `<F>Builder`

| # | Aile | # | Aile |
|---|---|---|---|
| 1 | SqliteConnection | 6 | SqliteInitializer |
| 2 | SqliteDialect | 7 | SqlitePool |
| 3 | SqlitePragmas | 8 | SqliteBackup |
| 4 | SqliteJournal | 9 | SqliteLock |
| 5 | SqliteProbe | 10 | SqliteTrace |

Her aile ayni rol kalibini 12 kez tekrarlar; kayit sayisi = aile x rol.

---

## §4 Kayit Ornegi (8 sutun)

| ID | Bilesen | Tip | Sorumluluk | Proje | TFM | Test | Durum |
|---|---|---|---|---|---|---|---|
| N26-001 | `ISqliteConnection` | interface | SqliteConnection sozlesmesi (port) | VersaCoder.Infrastructure | net48;net10.0-windows | SqliteConnectionTests | planli |
| N26-002 | `SqliteConnection` | class | SqliteConnection cekirdek uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | SqliteConnectionTests | planli |
| N26-003 | `SqliteConnectionDto` | record | SqliteConnection veri transferi modeli | VersaCoder.Infrastructure | net48;net10.0-windows | SqliteConnectionTests | planli |
| N26-004 | `SqliteConnectionMapper` | class | SqliteConnection <-> DTO esleme | VersaCoder.Infrastructure | net48;net10.0-windows | SqliteConnectionTests | planli |
| N26-005 | `SqliteConnectionValidator` | class | SqliteConnection dogrulama kurallari | VersaCoder.Infrastructure | net48;net10.0-windows | SqliteConnectionTests | planli |
| N26-006 | `SqliteConnectionFactory` | class | SqliteConnection uretimi ve varsayilanlari | VersaCoder.Infrastructure | net48;net10.0-windows | SqliteConnectionTests | planli |

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
| 3 | Test sinifi | SqliteConnectionTests (xUnit) | planli |
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
(Select-String -Path 'C:\versacoder\.ai\architecture\catalog\N26-sqlite-provider.md' -Pattern '^\|\s*N\d{2}-\d{3}\s*\|').Count
# 2) boyut bandi - beklenen: 2600-8500
(Get-Item 'C:\versacoder\.ai\architecture\catalog\N26-sqlite-provider.md').Length
# 3) klasor kaniti - gercek proje adi (ADR-028 2.3/5)
(Test-Path 'C:\versacoder\src\VersaCoder.Infrastructure\Data\Sqlite')
```

Katalog sayimi `powershell -NoProfile -ExecutionPolicy Bypass -File C:\versacoder\.ai\scripts\count-components.ps1` ile alinir; bu batch 26 node dosyasiyla 52 dugum klasorunu tamamlar - kayit esigi (5000) ornek satirlarla henuz karsilanmaz (DOĞRULAMA GEREKLI).

## §10 Kayit Tablosu (ADR-013 2.2 - kaynak vault port)

| N26-007 | `SqliteConnectionOptions` | record | SqliteConnection yapilandirmasi | VersaCoder.Infrastructure | net48;net10.0-windows | SqliteConnectionTests | planli |
| N26-008 | `SqliteConnectionProvider` | class | SqliteConnection saglayici uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | SqliteConnectionTests | planli |
| N26-009 | `SqliteConnectionRegistry` | class | SqliteConnection kayit/katalog yonetimi | VersaCoder.Infrastructure | net48;net10.0-windows | SqliteConnectionTests | planli |
| N26-010 | `SqliteConnectionConverter` | class | SqliteConnection bicim donusumu | VersaCoder.Infrastructure | net48;net10.0-windows | SqliteConnectionTests | planli |
| N26-011 | `SqliteConnectionTests` | class | SqliteConnection testleri (xUnit) | VersaCoder.Infrastructure | net48;net10.0-windows | - | planli |
| N26-012 | `SqliteConnectionBuilder` | class | SqliteConnection uretici (builder) | VersaCoder.Infrastructure | net48;net10.0-windows | SqliteConnectionTests | planli |
| N26-013 | `ISqliteDialect` | interface | SqliteDialect sozlesmesi (port) | VersaCoder.Infrastructure | net48;net10.0-windows | SqliteDialectTests | planli |
| N26-014 | `SqliteDialect` | class | SqliteDialect cekirdek uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | SqliteDialectTests | planli |
| N26-015 | `SqliteDialectDto` | record | SqliteDialect veri transferi modeli | VersaCoder.Infrastructure | net48;net10.0-windows | SqliteDialectTests | planli |
| N26-016 | `SqliteDialectMapper` | class | SqliteDialect <-> DTO esleme | VersaCoder.Infrastructure | net48;net10.0-windows | SqliteDialectTests | planli |
| N26-017 | `SqliteDialectValidator` | class | SqliteDialect dogrulama kurallari | VersaCoder.Infrastructure | net48;net10.0-windows | SqliteDialectTests | planli |
| N26-018 | `SqliteDialectFactory` | class | SqliteDialect uretimi ve varsayilanlari | VersaCoder.Infrastructure | net48;net10.0-windows | SqliteDialectTests | planli |
| N26-019 | `SqliteDialectOptions` | record | SqliteDialect yapilandirmasi | VersaCoder.Infrastructure | net48;net10.0-windows | SqliteDialectTests | planli |
| N26-020 | `SqliteDialectProvider` | class | SqliteDialect saglayici uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | SqliteDialectTests | planli |
| N26-021 | `SqliteDialectRegistry` | class | SqliteDialect kayit/katalog yonetimi | VersaCoder.Infrastructure | net48;net10.0-windows | SqliteDialectTests | planli |
| N26-022 | `SqliteDialectConverter` | class | SqliteDialect bicim donusumu | VersaCoder.Infrastructure | net48;net10.0-windows | SqliteDialectTests | planli |
| N26-023 | `SqliteDialectTests` | class | SqliteDialect testleri (xUnit) | VersaCoder.Infrastructure | net48;net10.0-windows | - | planli |
| N26-024 | `SqliteDialectBuilder` | class | SqliteDialect uretici (builder) | VersaCoder.Infrastructure | net48;net10.0-windows | SqliteDialectTests | planli |
| N26-025 | `ISqlitePragmas` | interface | SqlitePragmas sozlesmesi (port) | VersaCoder.Infrastructure | net48;net10.0-windows | SqlitePragmasTests | planli |
| N26-026 | `SqlitePragmas` | class | SqlitePragmas cekirdek uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | SqlitePragmasTests | planli |
| N26-027 | `SqlitePragmasDto` | record | SqlitePragmas veri transferi modeli | VersaCoder.Infrastructure | net48;net10.0-windows | SqlitePragmasTests | planli |
| N26-028 | `SqlitePragmasMapper` | class | SqlitePragmas <-> DTO esleme | VersaCoder.Infrastructure | net48;net10.0-windows | SqlitePragmasTests | planli |
| N26-029 | `SqlitePragmasValidator` | class | SqlitePragmas dogrulama kurallari | VersaCoder.Infrastructure | net48;net10.0-windows | SqlitePragmasTests | planli |
| N26-030 | `SqlitePragmasFactory` | class | SqlitePragmas uretimi ve varsayilanlari | VersaCoder.Infrastructure | net48;net10.0-windows | SqlitePragmasTests | planli |
| N26-031 | `SqlitePragmasOptions` | record | SqlitePragmas yapilandirmasi | VersaCoder.Infrastructure | net48;net10.0-windows | SqlitePragmasTests | planli |
| N26-032 | `SqlitePragmasProvider` | class | SqlitePragmas saglayici uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | SqlitePragmasTests | planli |
| N26-033 | `SqlitePragmasRegistry` | class | SqlitePragmas kayit/katalog yonetimi | VersaCoder.Infrastructure | net48;net10.0-windows | SqlitePragmasTests | planli |
| N26-034 | `SqlitePragmasConverter` | class | SqlitePragmas bicim donusumu | VersaCoder.Infrastructure | net48;net10.0-windows | SqlitePragmasTests | planli |
| N26-035 | `SqlitePragmasTests` | class | SqlitePragmas testleri (xUnit) | VersaCoder.Infrastructure | net48;net10.0-windows | - | planli |
| N26-036 | `SqlitePragmasBuilder` | class | SqlitePragmas uretici (builder) | VersaCoder.Infrastructure | net48;net10.0-windows | SqlitePragmasTests | planli |
| N26-037 | `ISqliteJournal` | interface | SqliteJournal sozlesmesi (port) | VersaCoder.Infrastructure | net48;net10.0-windows | SqliteJournalTests | planli |
| N26-038 | `SqliteJournal` | class | SqliteJournal cekirdek uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | SqliteJournalTests | planli |
| N26-039 | `SqliteJournalDto` | record | SqliteJournal veri transferi modeli | VersaCoder.Infrastructure | net48;net10.0-windows | SqliteJournalTests | planli |
| N26-040 | `SqliteJournalMapper` | class | SqliteJournal <-> DTO esleme | VersaCoder.Infrastructure | net48;net10.0-windows | SqliteJournalTests | planli |
| N26-041 | `SqliteJournalValidator` | class | SqliteJournal dogrulama kurallari | VersaCoder.Infrastructure | net48;net10.0-windows | SqliteJournalTests | planli |
| N26-042 | `SqliteJournalFactory` | class | SqliteJournal uretimi ve varsayilanlari | VersaCoder.Infrastructure | net48;net10.0-windows | SqliteJournalTests | planli |
| N26-043 | `SqliteJournalOptions` | record | SqliteJournal yapilandirmasi | VersaCoder.Infrastructure | net48;net10.0-windows | SqliteJournalTests | planli |
| N26-044 | `SqliteJournalProvider` | class | SqliteJournal saglayici uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | SqliteJournalTests | planli |
| N26-045 | `SqliteJournalRegistry` | class | SqliteJournal kayit/katalog yonetimi | VersaCoder.Infrastructure | net48;net10.0-windows | SqliteJournalTests | planli |
| N26-046 | `SqliteJournalConverter` | class | SqliteJournal bicim donusumu | VersaCoder.Infrastructure | net48;net10.0-windows | SqliteJournalTests | planli |
| N26-047 | `SqliteJournalTests` | class | SqliteJournal testleri (xUnit) | VersaCoder.Infrastructure | net48;net10.0-windows | - | planli |
| N26-048 | `SqliteJournalBuilder` | class | SqliteJournal uretici (builder) | VersaCoder.Infrastructure | net48;net10.0-windows | SqliteJournalTests | planli |
| N26-049 | `ISqliteProbe` | interface | SqliteProbe sozlesmesi (port) | VersaCoder.Infrastructure | net48;net10.0-windows | SqliteProbeTests | planli |
| N26-050 | `SqliteProbe` | class | SqliteProbe cekirdek uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | SqliteProbeTests | planli |
| N26-051 | `SqliteProbeDto` | record | SqliteProbe veri transferi modeli | VersaCoder.Infrastructure | net48;net10.0-windows | SqliteProbeTests | planli |
| N26-052 | `SqliteProbeMapper` | class | SqliteProbe <-> DTO esleme | VersaCoder.Infrastructure | net48;net10.0-windows | SqliteProbeTests | planli |
| N26-053 | `SqliteProbeValidator` | class | SqliteProbe dogrulama kurallari | VersaCoder.Infrastructure | net48;net10.0-windows | SqliteProbeTests | planli |
| N26-054 | `SqliteProbeFactory` | class | SqliteProbe uretimi ve varsayilanlari | VersaCoder.Infrastructure | net48;net10.0-windows | SqliteProbeTests | planli |
| N26-055 | `SqliteProbeOptions` | record | SqliteProbe yapilandirmasi | VersaCoder.Infrastructure | net48;net10.0-windows | SqliteProbeTests | planli |
| N26-056 | `SqliteProbeProvider` | class | SqliteProbe saglayici uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | SqliteProbeTests | planli |
| N26-057 | `SqliteProbeRegistry` | class | SqliteProbe kayit/katalog yonetimi | VersaCoder.Infrastructure | net48;net10.0-windows | SqliteProbeTests | planli |
| N26-058 | `SqliteProbeConverter` | class | SqliteProbe bicim donusumu | VersaCoder.Infrastructure | net48;net10.0-windows | SqliteProbeTests | planli |
| N26-059 | `SqliteProbeTests` | class | SqliteProbe testleri (xUnit) | VersaCoder.Infrastructure | net48;net10.0-windows | - | planli |
| N26-060 | `SqliteProbeBuilder` | class | SqliteProbe uretici (builder) | VersaCoder.Infrastructure | net48;net10.0-windows | SqliteProbeTests | planli |
| N26-061 | `ISqliteInitializer` | interface | SqliteInitializer sozlesmesi (port) | VersaCoder.Infrastructure | net48;net10.0-windows | SqliteInitializerTests | planli |
| N26-062 | `SqliteInitializer` | class | SqliteInitializer cekirdek uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | SqliteInitializerTests | planli |
| N26-063 | `SqliteInitializerDto` | record | SqliteInitializer veri transferi modeli | VersaCoder.Infrastructure | net48;net10.0-windows | SqliteInitializerTests | planli |
| N26-064 | `SqliteInitializerMapper` | class | SqliteInitializer <-> DTO esleme | VersaCoder.Infrastructure | net48;net10.0-windows | SqliteInitializerTests | planli |
| N26-065 | `SqliteInitializerValidator` | class | SqliteInitializer dogrulama kurallari | VersaCoder.Infrastructure | net48;net10.0-windows | SqliteInitializerTests | planli |
| N26-066 | `SqliteInitializerFactory` | class | SqliteInitializer uretimi ve varsayilanlari | VersaCoder.Infrastructure | net48;net10.0-windows | SqliteInitializerTests | planli |
| N26-067 | `SqliteInitializerOptions` | record | SqliteInitializer yapilandirmasi | VersaCoder.Infrastructure | net48;net10.0-windows | SqliteInitializerTests | planli |
| N26-068 | `SqliteInitializerProvider` | class | SqliteInitializer saglayici uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | SqliteInitializerTests | planli |
| N26-069 | `SqliteInitializerRegistry` | class | SqliteInitializer kayit/katalog yonetimi | VersaCoder.Infrastructure | net48;net10.0-windows | SqliteInitializerTests | planli |
| N26-070 | `SqliteInitializerConverter` | class | SqliteInitializer bicim donusumu | VersaCoder.Infrastructure | net48;net10.0-windows | SqliteInitializerTests | planli |
| N26-071 | `SqliteInitializerTests` | class | SqliteInitializer testleri (xUnit) | VersaCoder.Infrastructure | net48;net10.0-windows | - | planli |
| N26-072 | `SqliteInitializerBuilder` | class | SqliteInitializer uretici (builder) | VersaCoder.Infrastructure | net48;net10.0-windows | SqliteInitializerTests | planli |
| N26-073 | `ISqlitePool` | interface | SqlitePool sozlesmesi (port) | VersaCoder.Infrastructure | net48;net10.0-windows | SqlitePoolTests | planli |
| N26-074 | `SqlitePool` | class | SqlitePool cekirdek uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | SqlitePoolTests | planli |
| N26-075 | `SqlitePoolDto` | record | SqlitePool veri transferi modeli | VersaCoder.Infrastructure | net48;net10.0-windows | SqlitePoolTests | planli |
| N26-076 | `SqlitePoolMapper` | class | SqlitePool <-> DTO esleme | VersaCoder.Infrastructure | net48;net10.0-windows | SqlitePoolTests | planli |
| N26-077 | `SqlitePoolValidator` | class | SqlitePool dogrulama kurallari | VersaCoder.Infrastructure | net48;net10.0-windows | SqlitePoolTests | planli |
| N26-078 | `SqlitePoolFactory` | class | SqlitePool uretimi ve varsayilanlari | VersaCoder.Infrastructure | net48;net10.0-windows | SqlitePoolTests | planli |
| N26-079 | `SqlitePoolOptions` | record | SqlitePool yapilandirmasi | VersaCoder.Infrastructure | net48;net10.0-windows | SqlitePoolTests | planli |
| N26-080 | `SqlitePoolProvider` | class | SqlitePool saglayici uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | SqlitePoolTests | planli |
| N26-081 | `SqlitePoolRegistry` | class | SqlitePool kayit/katalog yonetimi | VersaCoder.Infrastructure | net48;net10.0-windows | SqlitePoolTests | planli |
| N26-082 | `SqlitePoolConverter` | class | SqlitePool bicim donusumu | VersaCoder.Infrastructure | net48;net10.0-windows | SqlitePoolTests | planli |
| N26-083 | `SqlitePoolTests` | class | SqlitePool testleri (xUnit) | VersaCoder.Infrastructure | net48;net10.0-windows | - | planli |
| N26-084 | `SqlitePoolBuilder` | class | SqlitePool uretici (builder) | VersaCoder.Infrastructure | net48;net10.0-windows | SqlitePoolTests | planli |
| N26-085 | `ISqliteBackup` | interface | SqliteBackup sozlesmesi (port) | VersaCoder.Infrastructure | net48;net10.0-windows | SqliteBackupTests | planli |
| N26-086 | `SqliteBackup` | class | SqliteBackup cekirdek uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | SqliteBackupTests | planli |
| N26-087 | `SqliteBackupDto` | record | SqliteBackup veri transferi modeli | VersaCoder.Infrastructure | net48;net10.0-windows | SqliteBackupTests | planli |
| N26-088 | `SqliteBackupMapper` | class | SqliteBackup <-> DTO esleme | VersaCoder.Infrastructure | net48;net10.0-windows | SqliteBackupTests | planli |
| N26-089 | `SqliteBackupValidator` | class | SqliteBackup dogrulama kurallari | VersaCoder.Infrastructure | net48;net10.0-windows | SqliteBackupTests | planli |
| N26-090 | `SqliteBackupFactory` | class | SqliteBackup uretimi ve varsayilanlari | VersaCoder.Infrastructure | net48;net10.0-windows | SqliteBackupTests | planli |
| N26-091 | `SqliteBackupOptions` | record | SqliteBackup yapilandirmasi | VersaCoder.Infrastructure | net48;net10.0-windows | SqliteBackupTests | planli |
| N26-092 | `SqliteBackupProvider` | class | SqliteBackup saglayici uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | SqliteBackupTests | planli |
| N26-093 | `SqliteBackupRegistry` | class | SqliteBackup kayit/katalog yonetimi | VersaCoder.Infrastructure | net48;net10.0-windows | SqliteBackupTests | planli |
| N26-094 | `SqliteBackupConverter` | class | SqliteBackup bicim donusumu | VersaCoder.Infrastructure | net48;net10.0-windows | SqliteBackupTests | planli |
| N26-095 | `SqliteBackupTests` | class | SqliteBackup testleri (xUnit) | VersaCoder.Infrastructure | net48;net10.0-windows | - | planli |
| N26-096 | `SqliteBackupBuilder` | class | SqliteBackup uretici (builder) | VersaCoder.Infrastructure | net48;net10.0-windows | SqliteBackupTests | planli |
| N26-097 | `ISqliteLock` | interface | SqliteLock sozlesmesi (port) | VersaCoder.Infrastructure | net48;net10.0-windows | SqliteLockTests | planli |
| N26-098 | `SqliteLock` | class | SqliteLock cekirdek uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | SqliteLockTests | planli |
| N26-099 | `SqliteLockDto` | record | SqliteLock veri transferi modeli | VersaCoder.Infrastructure | net48;net10.0-windows | SqliteLockTests | planli |
| N26-100 | `SqliteLockMapper` | class | SqliteLock <-> DTO esleme | VersaCoder.Infrastructure | net48;net10.0-windows | SqliteLockTests | planli |
| N26-101 | `SqliteLockValidator` | class | SqliteLock dogrulama kurallari | VersaCoder.Infrastructure | net48;net10.0-windows | SqliteLockTests | planli |
| N26-102 | `SqliteLockFactory` | class | SqliteLock uretimi ve varsayilanlari | VersaCoder.Infrastructure | net48;net10.0-windows | SqliteLockTests | planli |
| N26-103 | `SqliteLockOptions` | record | SqliteLock yapilandirmasi | VersaCoder.Infrastructure | net48;net10.0-windows | SqliteLockTests | planli |
| N26-104 | `SqliteLockProvider` | class | SqliteLock saglayici uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | SqliteLockTests | planli |
| N26-105 | `SqliteLockRegistry` | class | SqliteLock kayit/katalog yonetimi | VersaCoder.Infrastructure | net48;net10.0-windows | SqliteLockTests | planli |
| N26-106 | `SqliteLockConverter` | class | SqliteLock bicim donusumu | VersaCoder.Infrastructure | net48;net10.0-windows | SqliteLockTests | planli |
| N26-107 | `SqliteLockTests` | class | SqliteLock testleri (xUnit) | VersaCoder.Infrastructure | net48;net10.0-windows | - | planli |
| N26-108 | `SqliteLockBuilder` | class | SqliteLock uretici (builder) | VersaCoder.Infrastructure | net48;net10.0-windows | SqliteLockTests | planli |
| N26-109 | `ISqliteTrace` | interface | SqliteTrace sozlesmesi (port) | VersaCoder.Infrastructure | net48;net10.0-windows | SqliteTraceTests | planli |
| N26-110 | `SqliteTrace` | class | SqliteTrace cekirdek uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | SqliteTraceTests | planli |
| N26-111 | `SqliteTraceDto` | record | SqliteTrace veri transferi modeli | VersaCoder.Infrastructure | net48;net10.0-windows | SqliteTraceTests | planli |
| N26-112 | `SqliteTraceMapper` | class | SqliteTrace <-> DTO esleme | VersaCoder.Infrastructure | net48;net10.0-windows | SqliteTraceTests | planli |
| N26-113 | `SqliteTraceValidator` | class | SqliteTrace dogrulama kurallari | VersaCoder.Infrastructure | net48;net10.0-windows | SqliteTraceTests | planli |
| N26-114 | `SqliteTraceFactory` | class | SqliteTrace uretimi ve varsayilanlari | VersaCoder.Infrastructure | net48;net10.0-windows | SqliteTraceTests | planli |
| N26-115 | `SqliteTraceOptions` | record | SqliteTrace yapilandirmasi | VersaCoder.Infrastructure | net48;net10.0-windows | SqliteTraceTests | planli |
| N26-116 | `SqliteTraceProvider` | class | SqliteTrace saglayici uygulamasi | VersaCoder.Infrastructure | net48;net10.0-windows | SqliteTraceTests | planli |
| N26-117 | `SqliteTraceRegistry` | class | SqliteTrace kayit/katalog yonetimi | VersaCoder.Infrastructure | net48;net10.0-windows | SqliteTraceTests | planli |
| N26-118 | `SqliteTraceConverter` | class | SqliteTrace bicim donusumu | VersaCoder.Infrastructure | net48;net10.0-windows | SqliteTraceTests | planli |
| N26-119 | `SqliteTraceTests` | class | SqliteTrace testleri (xUnit) | VersaCoder.Infrastructure | net48;net10.0-windows | - | planli |
| N26-120 | `SqliteTraceBuilder` | class | SqliteTrace uretici (builder) | VersaCoder.Infrastructure | net48;net10.0-windows | SqliteTraceTests | planli |
