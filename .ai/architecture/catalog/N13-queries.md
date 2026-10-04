---
title: "Katalog Dugumu N13 - Sorgular (l2)"
type: catalog
category: architecture
status: needs-validation
version: 1.0.0
updated: 2026-10-04
authority: derived
scope: "N13 queries dugumu: kimlik, src klasoru, aile yapisi, ornek kayit ve yon kurali (ADR-013 + ADR-028)"
---

# Katalog Dugumu N13 - Sorgular

**Zorunlu Baglantilar:** [catalog index](index.md) · [node-inventory](node-inventory.md) · [mimari index](../index.md) · [ADR-028](../../.decisions/accepted/ADR-028-layered-baseline-9-projects.md) · [ADR-013](../../.decisions/accepted/ADR-013-layered-architecture-52-nodes.md)

> **Tek cumlelik sorumluluk:** *N13 queries hangi projede ve klasorde yasar, ne yapar, kac kayit hedefler* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

**N13 (queries), l2 Application katmaninin alt katmanidir: `VersaCoder.Application` projesinde `src/VersaCoder.Application/Queries/` klasorunde yasar.** GetSession, ListSessions gibi okuma sorgularini ve sonuc DTOlarini tanimlar. 10 aile x 12 rol = 120 kayit hedefi (ADR-013 asgari 112); durum `planli`.

Kayitlar ADR-013 2.1 biciminde 8 sutun tasir; bu dosya aile kalibini ve ornek satirlari gosterir, tam 120 satirlik liste bu dosyanin boyut sinirina sigmaz (DOĞRULAMA GEREKLI).

```text
+--------------------------------------------------+
| l2 Application -> VersaCoder.Application
| src/VersaCoder.Application/Queries/
| 10 aile x 12 rol = 120 kayit (esik 112)
+--------------------------------------------------+
```

| Alan | Deger |
|---|---|
| Dugum | N13 |
| Dugum turu | alt katman (klasor/namespace duzeyi) |
| Makro katman | l2 Application |
| Proje (.csproj) | VersaCoder.Application |
| TFM | net48;net10.0-windows |
| Kayit hedefi | 120 (ADR-013 asgari 112) |
| Durum | planli |
| Sorumlu rol | architect |

---

## §1 Sorumluluk (YAPAR / YAPMAZ)

| # | YAPAR | YAPMAZ |
|---|---|---|
| 1 | GetSession, ListSessions gibi okuma sorgularini ve sonuc DTOlarini tanimlar | l4 katmanina dogrudan bakmak (repository portu l0 katmaninda, uygulamasi l4 uzerinde) - YOK |
| 2 | kayitlari 8 sutunlu ADR-013 biciminde tutmak (planli -> yapildi -> testli) | SQL yazmak veya baglanti yonetmek |
| 3 | command, query ve isleyici (handler) yazmak | domain invariantini ihlal etmek |
| 4 | DTO esleme ve girdi dogrulamayi yurutmek | UI ekrani acmak veya Ribbon kontrolune dokunmak |
| 5 | kullanim senaryosunu uctan uca orkestre etmek | l3, l5, l6 ve l7 katmanlarina referans vermek |
| 6 | l0 portlari uzerinden alt katmanlari cagirmak | istisnayi disari tasimak, donus tipi Result<T> olur |

---

## §2 Klasor ve N Eslemesi

**Kalip (ADR-028 2.3/4):** `N33 -> src\VersaCoder.Protocol\ToolSchema\` satiri SSOT ornegidir; bu dosyanin klasoru ayni kalipla yazilir.

| # | N | Slug | src/ klasoru |
|---|---|---|---|
| 1 | N13 | queries | `src/VersaCoder.Application/Queries/` |

**Kanit yolu (ADR-028 2.3/5):** tarama yolu gercek proje adini icerir; genis kalip (`src/**/Domain/**`) sahte PASS uretir.

```powershell
Test-Path 'C:\versacoder\src\VersaCoder.Application\Queries'
```

Dizin su an yok cikti `False` olur; PASS yazilmaz -> **DOĞRULAMA GEREKLI**.

---

## §3 Aile Yapisi

Rol kalibi (12): `I<F>` · `<F>` · `<F>Dto` · `<F>Mapper` · `<F>Validator` · `<F>Factory` · `<F>Options` · `<F>Provider` · `<F>Registry` · `<F>Converter` · `<F>Tests` · `<F>Builder`

| # | Aile | # | Aile |
|---|---|---|---|
| 1 | GetSession | 6 | GetTaskList |
| 2 | ListSessions | 7 | GetToolCatalog |
| 3 | GetMessages | 8 | GetProjectTree |
| 4 | SearchFiles | 9 | GetBudget |
| 5 | GetSettings | 10 | GetAudit |

Her aile ayni rol kalibini 12 kez tekrarlar; kayit sayisi = aile x rol.

---

## §4 Kayit Ornegi (8 sutun)

| ID | Bilesen | Tip | Sorumluluk | Proje | TFM | Test | Durum |
|---|---|---|---|---|---|---|---|
| N13-001 | `IGetSession` | interface | GetSession sozlesmesi (port) | VersaCoder.Application | net48;net10.0-windows | GetSessionTests | planli |
| N13-002 | `GetSession` | class | GetSession cekirdek uygulamasi | VersaCoder.Application | net48;net10.0-windows | GetSessionTests | planli |
| N13-003 | `GetSessionDto` | record | GetSession veri transferi modeli | VersaCoder.Application | net48;net10.0-windows | GetSessionTests | planli |
| N13-004 | `GetSessionMapper` | class | GetSession <-> DTO esleme | VersaCoder.Application | net48;net10.0-windows | GetSessionTests | planli |
| N13-005 | `GetSessionValidator` | class | GetSession dogrulama kurallari | VersaCoder.Application | net48;net10.0-windows | GetSessionTests | planli |
| N13-006 | `GetSessionFactory` | class | GetSession uretimi ve varsayilanlari | VersaCoder.Application | net48;net10.0-windows | GetSessionTests | planli |

Kalan aileler ayni kalibi 12 rolle tekrarlar (makro dugumlerde 4 rol); toplam hedef **120** kayit.

---

## §5 Yon Kurali

| # | Yon | Deger | Kaynak |
|---|---|---|---|
| 1 | l2 -> l1 ve l0 | OK | ADR-028 2.1 |
| 2 | l2 -> l4 | YOK (repository portu l0 katmaninda) | ADR-028 2.3/2 |
| 3 | l4 -> l2 | X | ADR-028 2.3/2 |
| 4 | l3, l5, l6, l7 -> l2 | X | ADR-028 2.1 |

Matrisin tamami [layer-matrix](layer-matrix.md) dosyasindadir; NetArchTest `LayerRulesTests` bu tabloya gore yazilir.

---

## §6 Test ve Durum

| # | Kontrol | Beklenen | Durum |
|---|---|---|---|
| 1 | Kayit satiri | 120 (asgari 112) | bu dosyada 6 ornek satir; tam liste DOĞRULAMA GEREKLI |
| 2 | Durum degeri | planli -> yapildi -> testli | planli |
| 3 | Test sinifi | GetSessionTests (xUnit) | planli |
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
(Select-String -Path 'C:\versacoder\.ai\architecture\catalog\N13-queries.md' -Pattern '^\|\s*N\d{2}-\d{3}\s*\|').Count
# 2) boyut bandi - beklenen: 2600-8500
(Get-Item 'C:\versacoder\.ai\architecture\catalog\N13-queries.md').Length
# 3) klasor kaniti - gercek proje adi (ADR-028 2.3/5)
(Test-Path 'C:\versacoder\src\VersaCoder.Application\Queries')
```

Katalog sayimi `powershell -NoProfile -ExecutionPolicy Bypass -File C:\versacoder\.ai\scripts\count-components.ps1` ile alinir; bu batch sonunda toplam esik (5000) henuz karsilanmaz - kalan 26 node dosyasi sonraki batchtir.

## §10 Kayit Tablosu (ADR-013 2.2 - kaynak vault port)

| N13-007 | `GetSessionOptions` | record | GetSession yapilandirmasi | VersaCoder.Application | net48;net10.0-windows | GetSessionTests | planli |
| N13-008 | `GetSessionProvider` | class | GetSession saglayici uygulamasi | VersaCoder.Application | net48;net10.0-windows | GetSessionTests | planli |
| N13-009 | `GetSessionRegistry` | class | GetSession kayit/katalog yonetimi | VersaCoder.Application | net48;net10.0-windows | GetSessionTests | planli |
| N13-010 | `GetSessionConverter` | class | GetSession bicim donusumu | VersaCoder.Application | net48;net10.0-windows | GetSessionTests | planli |
| N13-011 | `GetSessionTests` | class | GetSession testleri (xUnit) | VersaCoder.Application | net48;net10.0-windows | - | planli |
| N13-012 | `GetSessionBuilder` | class | GetSession uretici (builder) | VersaCoder.Application | net48;net10.0-windows | GetSessionTests | planli |
| N13-013 | `IListSessions` | interface | ListSessions sozlesmesi (port) | VersaCoder.Application | net48;net10.0-windows | ListSessionsTests | planli |
| N13-014 | `ListSessions` | class | ListSessions cekirdek uygulamasi | VersaCoder.Application | net48;net10.0-windows | ListSessionsTests | planli |
| N13-015 | `ListSessionsDto` | record | ListSessions veri transferi modeli | VersaCoder.Application | net48;net10.0-windows | ListSessionsTests | planli |
| N13-016 | `ListSessionsMapper` | class | ListSessions <-> DTO esleme | VersaCoder.Application | net48;net10.0-windows | ListSessionsTests | planli |
| N13-017 | `ListSessionsValidator` | class | ListSessions dogrulama kurallari | VersaCoder.Application | net48;net10.0-windows | ListSessionsTests | planli |
| N13-018 | `ListSessionsFactory` | class | ListSessions uretimi ve varsayilanlari | VersaCoder.Application | net48;net10.0-windows | ListSessionsTests | planli |
| N13-019 | `ListSessionsOptions` | record | ListSessions yapilandirmasi | VersaCoder.Application | net48;net10.0-windows | ListSessionsTests | planli |
| N13-020 | `ListSessionsProvider` | class | ListSessions saglayici uygulamasi | VersaCoder.Application | net48;net10.0-windows | ListSessionsTests | planli |
| N13-021 | `ListSessionsRegistry` | class | ListSessions kayit/katalog yonetimi | VersaCoder.Application | net48;net10.0-windows | ListSessionsTests | planli |
| N13-022 | `ListSessionsConverter` | class | ListSessions bicim donusumu | VersaCoder.Application | net48;net10.0-windows | ListSessionsTests | planli |
| N13-023 | `ListSessionsTests` | class | ListSessions testleri (xUnit) | VersaCoder.Application | net48;net10.0-windows | - | planli |
| N13-024 | `ListSessionsBuilder` | class | ListSessions uretici (builder) | VersaCoder.Application | net48;net10.0-windows | ListSessionsTests | planli |
| N13-025 | `IGetMessages` | interface | GetMessages sozlesmesi (port) | VersaCoder.Application | net48;net10.0-windows | GetMessagesTests | planli |
| N13-026 | `GetMessages` | class | GetMessages cekirdek uygulamasi | VersaCoder.Application | net48;net10.0-windows | GetMessagesTests | planli |
| N13-027 | `GetMessagesDto` | record | GetMessages veri transferi modeli | VersaCoder.Application | net48;net10.0-windows | GetMessagesTests | planli |
| N13-028 | `GetMessagesMapper` | class | GetMessages <-> DTO esleme | VersaCoder.Application | net48;net10.0-windows | GetMessagesTests | planli |
| N13-029 | `GetMessagesValidator` | class | GetMessages dogrulama kurallari | VersaCoder.Application | net48;net10.0-windows | GetMessagesTests | planli |
| N13-030 | `GetMessagesFactory` | class | GetMessages uretimi ve varsayilanlari | VersaCoder.Application | net48;net10.0-windows | GetMessagesTests | planli |
| N13-031 | `GetMessagesOptions` | record | GetMessages yapilandirmasi | VersaCoder.Application | net48;net10.0-windows | GetMessagesTests | planli |
| N13-032 | `GetMessagesProvider` | class | GetMessages saglayici uygulamasi | VersaCoder.Application | net48;net10.0-windows | GetMessagesTests | planli |
| N13-033 | `GetMessagesRegistry` | class | GetMessages kayit/katalog yonetimi | VersaCoder.Application | net48;net10.0-windows | GetMessagesTests | planli |
| N13-034 | `GetMessagesConverter` | class | GetMessages bicim donusumu | VersaCoder.Application | net48;net10.0-windows | GetMessagesTests | planli |
| N13-035 | `GetMessagesTests` | class | GetMessages testleri (xUnit) | VersaCoder.Application | net48;net10.0-windows | - | planli |
| N13-036 | `GetMessagesBuilder` | class | GetMessages uretici (builder) | VersaCoder.Application | net48;net10.0-windows | GetMessagesTests | planli |
| N13-037 | `ISearchFiles` | interface | SearchFiles sozlesmesi (port) | VersaCoder.Application | net48;net10.0-windows | SearchFilesTests | planli |
| N13-038 | `SearchFiles` | class | SearchFiles cekirdek uygulamasi | VersaCoder.Application | net48;net10.0-windows | SearchFilesTests | planli |
| N13-039 | `SearchFilesDto` | record | SearchFiles veri transferi modeli | VersaCoder.Application | net48;net10.0-windows | SearchFilesTests | planli |
| N13-040 | `SearchFilesMapper` | class | SearchFiles <-> DTO esleme | VersaCoder.Application | net48;net10.0-windows | SearchFilesTests | planli |
| N13-041 | `SearchFilesValidator` | class | SearchFiles dogrulama kurallari | VersaCoder.Application | net48;net10.0-windows | SearchFilesTests | planli |
| N13-042 | `SearchFilesFactory` | class | SearchFiles uretimi ve varsayilanlari | VersaCoder.Application | net48;net10.0-windows | SearchFilesTests | planli |
| N13-043 | `SearchFilesOptions` | record | SearchFiles yapilandirmasi | VersaCoder.Application | net48;net10.0-windows | SearchFilesTests | planli |
| N13-044 | `SearchFilesProvider` | class | SearchFiles saglayici uygulamasi | VersaCoder.Application | net48;net10.0-windows | SearchFilesTests | planli |
| N13-045 | `SearchFilesRegistry` | class | SearchFiles kayit/katalog yonetimi | VersaCoder.Application | net48;net10.0-windows | SearchFilesTests | planli |
| N13-046 | `SearchFilesConverter` | class | SearchFiles bicim donusumu | VersaCoder.Application | net48;net10.0-windows | SearchFilesTests | planli |
| N13-047 | `SearchFilesTests` | class | SearchFiles testleri (xUnit) | VersaCoder.Application | net48;net10.0-windows | - | planli |
| N13-048 | `SearchFilesBuilder` | class | SearchFiles uretici (builder) | VersaCoder.Application | net48;net10.0-windows | SearchFilesTests | planli |
| N13-049 | `IGetSettings` | interface | GetSettings sozlesmesi (port) | VersaCoder.Application | net48;net10.0-windows | GetSettingsTests | planli |
| N13-050 | `GetSettings` | class | GetSettings cekirdek uygulamasi | VersaCoder.Application | net48;net10.0-windows | GetSettingsTests | planli |
| N13-051 | `GetSettingsDto` | record | GetSettings veri transferi modeli | VersaCoder.Application | net48;net10.0-windows | GetSettingsTests | planli |
| N13-052 | `GetSettingsMapper` | class | GetSettings <-> DTO esleme | VersaCoder.Application | net48;net10.0-windows | GetSettingsTests | planli |
| N13-053 | `GetSettingsValidator` | class | GetSettings dogrulama kurallari | VersaCoder.Application | net48;net10.0-windows | GetSettingsTests | planli |
| N13-054 | `GetSettingsFactory` | class | GetSettings uretimi ve varsayilanlari | VersaCoder.Application | net48;net10.0-windows | GetSettingsTests | planli |
| N13-055 | `GetSettingsOptions` | record | GetSettings yapilandirmasi | VersaCoder.Application | net48;net10.0-windows | GetSettingsTests | planli |
| N13-056 | `GetSettingsProvider` | class | GetSettings saglayici uygulamasi | VersaCoder.Application | net48;net10.0-windows | GetSettingsTests | planli |
| N13-057 | `GetSettingsRegistry` | class | GetSettings kayit/katalog yonetimi | VersaCoder.Application | net48;net10.0-windows | GetSettingsTests | planli |
| N13-058 | `GetSettingsConverter` | class | GetSettings bicim donusumu | VersaCoder.Application | net48;net10.0-windows | GetSettingsTests | planli |
| N13-059 | `GetSettingsTests` | class | GetSettings testleri (xUnit) | VersaCoder.Application | net48;net10.0-windows | - | planli |
| N13-060 | `GetSettingsBuilder` | class | GetSettings uretici (builder) | VersaCoder.Application | net48;net10.0-windows | GetSettingsTests | planli |
| N13-061 | `IGetTaskList` | interface | GetTaskList sozlesmesi (port) | VersaCoder.Application | net48;net10.0-windows | GetTaskListTests | planli |
| N13-062 | `GetTaskList` | class | GetTaskList cekirdek uygulamasi | VersaCoder.Application | net48;net10.0-windows | GetTaskListTests | planli |
| N13-063 | `GetTaskListDto` | record | GetTaskList veri transferi modeli | VersaCoder.Application | net48;net10.0-windows | GetTaskListTests | planli |
| N13-064 | `GetTaskListMapper` | class | GetTaskList <-> DTO esleme | VersaCoder.Application | net48;net10.0-windows | GetTaskListTests | planli |
| N13-065 | `GetTaskListValidator` | class | GetTaskList dogrulama kurallari | VersaCoder.Application | net48;net10.0-windows | GetTaskListTests | planli |
| N13-066 | `GetTaskListFactory` | class | GetTaskList uretimi ve varsayilanlari | VersaCoder.Application | net48;net10.0-windows | GetTaskListTests | planli |
| N13-067 | `GetTaskListOptions` | record | GetTaskList yapilandirmasi | VersaCoder.Application | net48;net10.0-windows | GetTaskListTests | planli |
| N13-068 | `GetTaskListProvider` | class | GetTaskList saglayici uygulamasi | VersaCoder.Application | net48;net10.0-windows | GetTaskListTests | planli |
| N13-069 | `GetTaskListRegistry` | class | GetTaskList kayit/katalog yonetimi | VersaCoder.Application | net48;net10.0-windows | GetTaskListTests | planli |
| N13-070 | `GetTaskListConverter` | class | GetTaskList bicim donusumu | VersaCoder.Application | net48;net10.0-windows | GetTaskListTests | planli |
| N13-071 | `GetTaskListTests` | class | GetTaskList testleri (xUnit) | VersaCoder.Application | net48;net10.0-windows | - | planli |
| N13-072 | `GetTaskListBuilder` | class | GetTaskList uretici (builder) | VersaCoder.Application | net48;net10.0-windows | GetTaskListTests | planli |
| N13-073 | `IGetToolCatalog` | interface | GetToolCatalog sozlesmesi (port) | VersaCoder.Application | net48;net10.0-windows | GetToolCatalogTests | planli |
| N13-074 | `GetToolCatalog` | class | GetToolCatalog cekirdek uygulamasi | VersaCoder.Application | net48;net10.0-windows | GetToolCatalogTests | planli |
| N13-075 | `GetToolCatalogDto` | record | GetToolCatalog veri transferi modeli | VersaCoder.Application | net48;net10.0-windows | GetToolCatalogTests | planli |
| N13-076 | `GetToolCatalogMapper` | class | GetToolCatalog <-> DTO esleme | VersaCoder.Application | net48;net10.0-windows | GetToolCatalogTests | planli |
| N13-077 | `GetToolCatalogValidator` | class | GetToolCatalog dogrulama kurallari | VersaCoder.Application | net48;net10.0-windows | GetToolCatalogTests | planli |
| N13-078 | `GetToolCatalogFactory` | class | GetToolCatalog uretimi ve varsayilanlari | VersaCoder.Application | net48;net10.0-windows | GetToolCatalogTests | planli |
| N13-079 | `GetToolCatalogOptions` | record | GetToolCatalog yapilandirmasi | VersaCoder.Application | net48;net10.0-windows | GetToolCatalogTests | planli |
| N13-080 | `GetToolCatalogProvider` | class | GetToolCatalog saglayici uygulamasi | VersaCoder.Application | net48;net10.0-windows | GetToolCatalogTests | planli |
| N13-081 | `GetToolCatalogRegistry` | class | GetToolCatalog kayit/katalog yonetimi | VersaCoder.Application | net48;net10.0-windows | GetToolCatalogTests | planli |
| N13-082 | `GetToolCatalogConverter` | class | GetToolCatalog bicim donusumu | VersaCoder.Application | net48;net10.0-windows | GetToolCatalogTests | planli |
| N13-083 | `GetToolCatalogTests` | class | GetToolCatalog testleri (xUnit) | VersaCoder.Application | net48;net10.0-windows | - | planli |
| N13-084 | `GetToolCatalogBuilder` | class | GetToolCatalog uretici (builder) | VersaCoder.Application | net48;net10.0-windows | GetToolCatalogTests | planli |
| N13-085 | `IGetProjectTree` | interface | GetProjectTree sozlesmesi (port) | VersaCoder.Application | net48;net10.0-windows | GetProjectTreeTests | planli |
| N13-086 | `GetProjectTree` | class | GetProjectTree cekirdek uygulamasi | VersaCoder.Application | net48;net10.0-windows | GetProjectTreeTests | planli |
| N13-087 | `GetProjectTreeDto` | record | GetProjectTree veri transferi modeli | VersaCoder.Application | net48;net10.0-windows | GetProjectTreeTests | planli |
| N13-088 | `GetProjectTreeMapper` | class | GetProjectTree <-> DTO esleme | VersaCoder.Application | net48;net10.0-windows | GetProjectTreeTests | planli |
| N13-089 | `GetProjectTreeValidator` | class | GetProjectTree dogrulama kurallari | VersaCoder.Application | net48;net10.0-windows | GetProjectTreeTests | planli |
| N13-090 | `GetProjectTreeFactory` | class | GetProjectTree uretimi ve varsayilanlari | VersaCoder.Application | net48;net10.0-windows | GetProjectTreeTests | planli |
| N13-091 | `GetProjectTreeOptions` | record | GetProjectTree yapilandirmasi | VersaCoder.Application | net48;net10.0-windows | GetProjectTreeTests | planli |
| N13-092 | `GetProjectTreeProvider` | class | GetProjectTree saglayici uygulamasi | VersaCoder.Application | net48;net10.0-windows | GetProjectTreeTests | planli |
| N13-093 | `GetProjectTreeRegistry` | class | GetProjectTree kayit/katalog yonetimi | VersaCoder.Application | net48;net10.0-windows | GetProjectTreeTests | planli |
| N13-094 | `GetProjectTreeConverter` | class | GetProjectTree bicim donusumu | VersaCoder.Application | net48;net10.0-windows | GetProjectTreeTests | planli |
| N13-095 | `GetProjectTreeTests` | class | GetProjectTree testleri (xUnit) | VersaCoder.Application | net48;net10.0-windows | - | planli |
| N13-096 | `GetProjectTreeBuilder` | class | GetProjectTree uretici (builder) | VersaCoder.Application | net48;net10.0-windows | GetProjectTreeTests | planli |
| N13-097 | `IGetBudget` | interface | GetBudget sozlesmesi (port) | VersaCoder.Application | net48;net10.0-windows | GetBudgetTests | planli |
| N13-098 | `GetBudget` | class | GetBudget cekirdek uygulamasi | VersaCoder.Application | net48;net10.0-windows | GetBudgetTests | planli |
| N13-099 | `GetBudgetDto` | record | GetBudget veri transferi modeli | VersaCoder.Application | net48;net10.0-windows | GetBudgetTests | planli |
| N13-100 | `GetBudgetMapper` | class | GetBudget <-> DTO esleme | VersaCoder.Application | net48;net10.0-windows | GetBudgetTests | planli |
| N13-101 | `GetBudgetValidator` | class | GetBudget dogrulama kurallari | VersaCoder.Application | net48;net10.0-windows | GetBudgetTests | planli |
| N13-102 | `GetBudgetFactory` | class | GetBudget uretimi ve varsayilanlari | VersaCoder.Application | net48;net10.0-windows | GetBudgetTests | planli |
| N13-103 | `GetBudgetOptions` | record | GetBudget yapilandirmasi | VersaCoder.Application | net48;net10.0-windows | GetBudgetTests | planli |
| N13-104 | `GetBudgetProvider` | class | GetBudget saglayici uygulamasi | VersaCoder.Application | net48;net10.0-windows | GetBudgetTests | planli |
| N13-105 | `GetBudgetRegistry` | class | GetBudget kayit/katalog yonetimi | VersaCoder.Application | net48;net10.0-windows | GetBudgetTests | planli |
| N13-106 | `GetBudgetConverter` | class | GetBudget bicim donusumu | VersaCoder.Application | net48;net10.0-windows | GetBudgetTests | planli |
| N13-107 | `GetBudgetTests` | class | GetBudget testleri (xUnit) | VersaCoder.Application | net48;net10.0-windows | - | planli |
| N13-108 | `GetBudgetBuilder` | class | GetBudget uretici (builder) | VersaCoder.Application | net48;net10.0-windows | GetBudgetTests | planli |
| N13-109 | `IGetAudit` | interface | GetAudit sozlesmesi (port) | VersaCoder.Application | net48;net10.0-windows | GetAuditTests | planli |
| N13-110 | `GetAudit` | class | GetAudit cekirdek uygulamasi | VersaCoder.Application | net48;net10.0-windows | GetAuditTests | planli |
| N13-111 | `GetAuditDto` | record | GetAudit veri transferi modeli | VersaCoder.Application | net48;net10.0-windows | GetAuditTests | planli |
| N13-112 | `GetAuditMapper` | class | GetAudit <-> DTO esleme | VersaCoder.Application | net48;net10.0-windows | GetAuditTests | planli |
| N13-113 | `GetAuditValidator` | class | GetAudit dogrulama kurallari | VersaCoder.Application | net48;net10.0-windows | GetAuditTests | planli |
| N13-114 | `GetAuditFactory` | class | GetAudit uretimi ve varsayilanlari | VersaCoder.Application | net48;net10.0-windows | GetAuditTests | planli |
| N13-115 | `GetAuditOptions` | record | GetAudit yapilandirmasi | VersaCoder.Application | net48;net10.0-windows | GetAuditTests | planli |
| N13-116 | `GetAuditProvider` | class | GetAudit saglayici uygulamasi | VersaCoder.Application | net48;net10.0-windows | GetAuditTests | planli |
| N13-117 | `GetAuditRegistry` | class | GetAudit kayit/katalog yonetimi | VersaCoder.Application | net48;net10.0-windows | GetAuditTests | planli |
| N13-118 | `GetAuditConverter` | class | GetAudit bicim donusumu | VersaCoder.Application | net48;net10.0-windows | GetAuditTests | planli |
| N13-119 | `GetAuditTests` | class | GetAudit testleri (xUnit) | VersaCoder.Application | net48;net10.0-windows | - | planli |
| N13-120 | `GetAuditBuilder` | class | GetAudit uretici (builder) | VersaCoder.Application | net48;net10.0-windows | GetAuditTests | planli |
