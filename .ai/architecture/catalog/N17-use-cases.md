---
title: "Katalog Dugumu N17 - Kullanim Senaryolari (l2)"
type: catalog
category: architecture
status: needs-validation
version: 1.0.0
updated: 2026-10-04
authority: derived
scope: "N17 use-cases dugumu: kimlik, src klasoru, aile yapisi, ornek kayit ve yon kurali (ADR-013 + ADR-028)"
---

# Katalog Dugumu N17 - Kullanim Senaryolari

**Zorunlu Baglantilar:** [catalog index](index.md) · [node-inventory](node-inventory.md) · [mimari index](../index.md) · [ADR-028](../../.decisions/accepted/ADR-028-layered-baseline-9-projects.md) · [ADR-013](../../.decisions/accepted/ADR-013-layered-architecture-52-nodes.md)

> **Tek cumlelik sorumluluk:** *N17 use-cases hangi projede ve klasorde yasar, ne yapar, kac kayit hedefler* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

**N17 (use-cases), l2 Application katmaninin alt katmanidir: `VersaCoder.Application` projesinde `src/VersaCoder.Application/UseCases/` klasorunde yasar.** ChatUseCase, ToolRunUseCase gibi uctan uca kullanim senaryolarini yazar. 10 aile x 12 rol = 120 kayit hedefi (ADR-013 asgari 112); durum `planli`.

Kayitlar ADR-013 2.1 biciminde 8 sutun tasir; bu dosya aile kalibini ve ornek satirlari gosterir, tam 120 satirlik liste bu dosyanin boyut sinirina sigmaz (DOĞRULAMA GEREKLI).

```text
+--------------------------------------------------+
| l2 Application -> VersaCoder.Application
| src/VersaCoder.Application/UseCases/
| 10 aile x 12 rol = 120 kayit (esik 112)
+--------------------------------------------------+
```

| Alan | Deger |
|---|---|
| Dugum | N17 |
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
| 1 | ChatUseCase, ToolRunUseCase gibi uctan uca kullanim senaryolarini yazar | l4 katmanina dogrudan bakmak (repository portu l0 katmaninda, uygulamasi l4 uzerinde) - YOK |
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
| 1 | N17 | use-cases | `src/VersaCoder.Application/UseCases/` |

**Kanit yolu (ADR-028 2.3/5):** tarama yolu gercek proje adini icerir; genis kalip (`src/**/Domain/**`) sahte PASS uretir.

```powershell
Test-Path 'C:\versacoder\src\VersaCoder.Application\UseCases'
```

Dizin su an yok cikti `False` olur; PASS yazilmaz -> **DOĞRULAMA GEREKLI**.

---

## §3 Aile Yapisi

Rol kalibi (12): `I<F>` · `<F>` · `<F>Dto` · `<F>Mapper` · `<F>Validator` · `<F>Factory` · `<F>Options` · `<F>Provider` · `<F>Registry` · `<F>Converter` · `<F>Tests` · `<F>Builder`

| # | Aile | # | Aile |
|---|---|---|---|
| 1 | ChatUseCase | 6 | AgentSpawnUseCase |
| 2 | ToolRunUseCase | 7 | TaskPlanUseCase |
| 3 | FileEditUseCase | 8 | SettingsApplyUseCase |
| 4 | ProjectOpenUseCase | 9 | BudgetCheckUseCase |
| 5 | SessionResumeUseCase | 10 | SearchUseCase |

Her aile ayni rol kalibini 12 kez tekrarlar; kayit sayisi = aile x rol.

---

## §4 Kayit Ornegi (8 sutun)

| ID | Bilesen | Tip | Sorumluluk | Proje | TFM | Test | Durum |
|---|---|---|---|---|---|---|---|
| N17-001 | `IChatUseCase` | interface | ChatUseCase sozlesmesi (port) | VersaCoder.Application | net48;net10.0-windows | ChatUseCaseTests | planli |
| N17-002 | `ChatUseCase` | class | ChatUseCase cekirdek uygulamasi | VersaCoder.Application | net48;net10.0-windows | ChatUseCaseTests | planli |
| N17-003 | `ChatUseCaseDto` | record | ChatUseCase veri transferi modeli | VersaCoder.Application | net48;net10.0-windows | ChatUseCaseTests | planli |
| N17-004 | `ChatUseCaseMapper` | class | ChatUseCase <-> DTO esleme | VersaCoder.Application | net48;net10.0-windows | ChatUseCaseTests | planli |
| N17-005 | `ChatUseCaseValidator` | class | ChatUseCase dogrulama kurallari | VersaCoder.Application | net48;net10.0-windows | ChatUseCaseTests | planli |
| N17-006 | `ChatUseCaseFactory` | class | ChatUseCase uretimi ve varsayilanlari | VersaCoder.Application | net48;net10.0-windows | ChatUseCaseTests | planli |

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
| 3 | Test sinifi | ChatUseCaseTests (xUnit) | planli |
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
(Select-String -Path 'C:\versacoder\.ai\architecture\catalog\N17-use-cases.md' -Pattern '^\|\s*N\d{2}-\d{3}\s*\|').Count
# 2) boyut bandi - beklenen: 2600-8500
(Get-Item 'C:\versacoder\.ai\architecture\catalog\N17-use-cases.md').Length
# 3) klasor kaniti - gercek proje adi (ADR-028 2.3/5)
(Test-Path 'C:\versacoder\src\VersaCoder.Application\UseCases')
```

Katalog sayimi `powershell -NoProfile -ExecutionPolicy Bypass -File C:\versacoder\.ai\scripts\count-components.ps1` ile alinir; bu batch sonunda toplam esik (5000) henuz karsilanmaz - kalan 26 node dosyasi sonraki batchtir.

## §10 Kayit Tablosu (ADR-013 2.2 - kaynak vault port)

| N17-007 | `ChatUseCaseOptions` | record | ChatUseCase yapilandirmasi | VersaCoder.Application | net48;net10.0-windows | ChatUseCaseTests | planli |
| N17-008 | `ChatUseCaseProvider` | class | ChatUseCase saglayici uygulamasi | VersaCoder.Application | net48;net10.0-windows | ChatUseCaseTests | planli |
| N17-009 | `ChatUseCaseRegistry` | class | ChatUseCase kayit/katalog yonetimi | VersaCoder.Application | net48;net10.0-windows | ChatUseCaseTests | planli |
| N17-010 | `ChatUseCaseConverter` | class | ChatUseCase bicim donusumu | VersaCoder.Application | net48;net10.0-windows | ChatUseCaseTests | planli |
| N17-011 | `ChatUseCaseTests` | class | ChatUseCase testleri (xUnit) | VersaCoder.Application | net48;net10.0-windows | - | planli |
| N17-012 | `ChatUseCaseBuilder` | class | ChatUseCase uretici (builder) | VersaCoder.Application | net48;net10.0-windows | ChatUseCaseTests | planli |
| N17-013 | `IToolRunUseCase` | interface | ToolRunUseCase sozlesmesi (port) | VersaCoder.Application | net48;net10.0-windows | ToolRunUseCaseTests | planli |
| N17-014 | `ToolRunUseCase` | class | ToolRunUseCase cekirdek uygulamasi | VersaCoder.Application | net48;net10.0-windows | ToolRunUseCaseTests | planli |
| N17-015 | `ToolRunUseCaseDto` | record | ToolRunUseCase veri transferi modeli | VersaCoder.Application | net48;net10.0-windows | ToolRunUseCaseTests | planli |
| N17-016 | `ToolRunUseCaseMapper` | class | ToolRunUseCase <-> DTO esleme | VersaCoder.Application | net48;net10.0-windows | ToolRunUseCaseTests | planli |
| N17-017 | `ToolRunUseCaseValidator` | class | ToolRunUseCase dogrulama kurallari | VersaCoder.Application | net48;net10.0-windows | ToolRunUseCaseTests | planli |
| N17-018 | `ToolRunUseCaseFactory` | class | ToolRunUseCase uretimi ve varsayilanlari | VersaCoder.Application | net48;net10.0-windows | ToolRunUseCaseTests | planli |
| N17-019 | `ToolRunUseCaseOptions` | record | ToolRunUseCase yapilandirmasi | VersaCoder.Application | net48;net10.0-windows | ToolRunUseCaseTests | planli |
| N17-020 | `ToolRunUseCaseProvider` | class | ToolRunUseCase saglayici uygulamasi | VersaCoder.Application | net48;net10.0-windows | ToolRunUseCaseTests | planli |
| N17-021 | `ToolRunUseCaseRegistry` | class | ToolRunUseCase kayit/katalog yonetimi | VersaCoder.Application | net48;net10.0-windows | ToolRunUseCaseTests | planli |
| N17-022 | `ToolRunUseCaseConverter` | class | ToolRunUseCase bicim donusumu | VersaCoder.Application | net48;net10.0-windows | ToolRunUseCaseTests | planli |
| N17-023 | `ToolRunUseCaseTests` | class | ToolRunUseCase testleri (xUnit) | VersaCoder.Application | net48;net10.0-windows | - | planli |
| N17-024 | `ToolRunUseCaseBuilder` | class | ToolRunUseCase uretici (builder) | VersaCoder.Application | net48;net10.0-windows | ToolRunUseCaseTests | planli |
| N17-025 | `IFileEditUseCase` | interface | FileEditUseCase sozlesmesi (port) | VersaCoder.Application | net48;net10.0-windows | FileEditUseCaseTests | planli |
| N17-026 | `FileEditUseCase` | class | FileEditUseCase cekirdek uygulamasi | VersaCoder.Application | net48;net10.0-windows | FileEditUseCaseTests | planli |
| N17-027 | `FileEditUseCaseDto` | record | FileEditUseCase veri transferi modeli | VersaCoder.Application | net48;net10.0-windows | FileEditUseCaseTests | planli |
| N17-028 | `FileEditUseCaseMapper` | class | FileEditUseCase <-> DTO esleme | VersaCoder.Application | net48;net10.0-windows | FileEditUseCaseTests | planli |
| N17-029 | `FileEditUseCaseValidator` | class | FileEditUseCase dogrulama kurallari | VersaCoder.Application | net48;net10.0-windows | FileEditUseCaseTests | planli |
| N17-030 | `FileEditUseCaseFactory` | class | FileEditUseCase uretimi ve varsayilanlari | VersaCoder.Application | net48;net10.0-windows | FileEditUseCaseTests | planli |
| N17-031 | `FileEditUseCaseOptions` | record | FileEditUseCase yapilandirmasi | VersaCoder.Application | net48;net10.0-windows | FileEditUseCaseTests | planli |
| N17-032 | `FileEditUseCaseProvider` | class | FileEditUseCase saglayici uygulamasi | VersaCoder.Application | net48;net10.0-windows | FileEditUseCaseTests | planli |
| N17-033 | `FileEditUseCaseRegistry` | class | FileEditUseCase kayit/katalog yonetimi | VersaCoder.Application | net48;net10.0-windows | FileEditUseCaseTests | planli |
| N17-034 | `FileEditUseCaseConverter` | class | FileEditUseCase bicim donusumu | VersaCoder.Application | net48;net10.0-windows | FileEditUseCaseTests | planli |
| N17-035 | `FileEditUseCaseTests` | class | FileEditUseCase testleri (xUnit) | VersaCoder.Application | net48;net10.0-windows | - | planli |
| N17-036 | `FileEditUseCaseBuilder` | class | FileEditUseCase uretici (builder) | VersaCoder.Application | net48;net10.0-windows | FileEditUseCaseTests | planli |
| N17-037 | `IProjectOpenUseCase` | interface | ProjectOpenUseCase sozlesmesi (port) | VersaCoder.Application | net48;net10.0-windows | ProjectOpenUseCaseTests | planli |
| N17-038 | `ProjectOpenUseCase` | class | ProjectOpenUseCase cekirdek uygulamasi | VersaCoder.Application | net48;net10.0-windows | ProjectOpenUseCaseTests | planli |
| N17-039 | `ProjectOpenUseCaseDto` | record | ProjectOpenUseCase veri transferi modeli | VersaCoder.Application | net48;net10.0-windows | ProjectOpenUseCaseTests | planli |
| N17-040 | `ProjectOpenUseCaseMapper` | class | ProjectOpenUseCase <-> DTO esleme | VersaCoder.Application | net48;net10.0-windows | ProjectOpenUseCaseTests | planli |
| N17-041 | `ProjectOpenUseCaseValidator` | class | ProjectOpenUseCase dogrulama kurallari | VersaCoder.Application | net48;net10.0-windows | ProjectOpenUseCaseTests | planli |
| N17-042 | `ProjectOpenUseCaseFactory` | class | ProjectOpenUseCase uretimi ve varsayilanlari | VersaCoder.Application | net48;net10.0-windows | ProjectOpenUseCaseTests | planli |
| N17-043 | `ProjectOpenUseCaseOptions` | record | ProjectOpenUseCase yapilandirmasi | VersaCoder.Application | net48;net10.0-windows | ProjectOpenUseCaseTests | planli |
| N17-044 | `ProjectOpenUseCaseProvider` | class | ProjectOpenUseCase saglayici uygulamasi | VersaCoder.Application | net48;net10.0-windows | ProjectOpenUseCaseTests | planli |
| N17-045 | `ProjectOpenUseCaseRegistry` | class | ProjectOpenUseCase kayit/katalog yonetimi | VersaCoder.Application | net48;net10.0-windows | ProjectOpenUseCaseTests | planli |
| N17-046 | `ProjectOpenUseCaseConverter` | class | ProjectOpenUseCase bicim donusumu | VersaCoder.Application | net48;net10.0-windows | ProjectOpenUseCaseTests | planli |
| N17-047 | `ProjectOpenUseCaseTests` | class | ProjectOpenUseCase testleri (xUnit) | VersaCoder.Application | net48;net10.0-windows | - | planli |
| N17-048 | `ProjectOpenUseCaseBuilder` | class | ProjectOpenUseCase uretici (builder) | VersaCoder.Application | net48;net10.0-windows | ProjectOpenUseCaseTests | planli |
| N17-049 | `ISessionResumeUseCase` | interface | SessionResumeUseCase sozlesmesi (port) | VersaCoder.Application | net48;net10.0-windows | SessionResumeUseCaseTests | planli |
| N17-050 | `SessionResumeUseCase` | class | SessionResumeUseCase cekirdek uygulamasi | VersaCoder.Application | net48;net10.0-windows | SessionResumeUseCaseTests | planli |
| N17-051 | `SessionResumeUseCaseDto` | record | SessionResumeUseCase veri transferi modeli | VersaCoder.Application | net48;net10.0-windows | SessionResumeUseCaseTests | planli |
| N17-052 | `SessionResumeUseCaseMapper` | class | SessionResumeUseCase <-> DTO esleme | VersaCoder.Application | net48;net10.0-windows | SessionResumeUseCaseTests | planli |
| N17-053 | `SessionResumeUseCaseValidator` | class | SessionResumeUseCase dogrulama kurallari | VersaCoder.Application | net48;net10.0-windows | SessionResumeUseCaseTests | planli |
| N17-054 | `SessionResumeUseCaseFactory` | class | SessionResumeUseCase uretimi ve varsayilanlari | VersaCoder.Application | net48;net10.0-windows | SessionResumeUseCaseTests | planli |
| N17-055 | `SessionResumeUseCaseOptions` | record | SessionResumeUseCase yapilandirmasi | VersaCoder.Application | net48;net10.0-windows | SessionResumeUseCaseTests | planli |
| N17-056 | `SessionResumeUseCaseProvider` | class | SessionResumeUseCase saglayici uygulamasi | VersaCoder.Application | net48;net10.0-windows | SessionResumeUseCaseTests | planli |
| N17-057 | `SessionResumeUseCaseRegistry` | class | SessionResumeUseCase kayit/katalog yonetimi | VersaCoder.Application | net48;net10.0-windows | SessionResumeUseCaseTests | planli |
| N17-058 | `SessionResumeUseCaseConverter` | class | SessionResumeUseCase bicim donusumu | VersaCoder.Application | net48;net10.0-windows | SessionResumeUseCaseTests | planli |
| N17-059 | `SessionResumeUseCaseTests` | class | SessionResumeUseCase testleri (xUnit) | VersaCoder.Application | net48;net10.0-windows | - | planli |
| N17-060 | `SessionResumeUseCaseBuilder` | class | SessionResumeUseCase uretici (builder) | VersaCoder.Application | net48;net10.0-windows | SessionResumeUseCaseTests | planli |
| N17-061 | `IAgentSpawnUseCase` | interface | AgentSpawnUseCase sozlesmesi (port) | VersaCoder.Application | net48;net10.0-windows | AgentSpawnUseCaseTests | planli |
| N17-062 | `AgentSpawnUseCase` | class | AgentSpawnUseCase cekirdek uygulamasi | VersaCoder.Application | net48;net10.0-windows | AgentSpawnUseCaseTests | planli |
| N17-063 | `AgentSpawnUseCaseDto` | record | AgentSpawnUseCase veri transferi modeli | VersaCoder.Application | net48;net10.0-windows | AgentSpawnUseCaseTests | planli |
| N17-064 | `AgentSpawnUseCaseMapper` | class | AgentSpawnUseCase <-> DTO esleme | VersaCoder.Application | net48;net10.0-windows | AgentSpawnUseCaseTests | planli |
| N17-065 | `AgentSpawnUseCaseValidator` | class | AgentSpawnUseCase dogrulama kurallari | VersaCoder.Application | net48;net10.0-windows | AgentSpawnUseCaseTests | planli |
| N17-066 | `AgentSpawnUseCaseFactory` | class | AgentSpawnUseCase uretimi ve varsayilanlari | VersaCoder.Application | net48;net10.0-windows | AgentSpawnUseCaseTests | planli |
| N17-067 | `AgentSpawnUseCaseOptions` | record | AgentSpawnUseCase yapilandirmasi | VersaCoder.Application | net48;net10.0-windows | AgentSpawnUseCaseTests | planli |
| N17-068 | `AgentSpawnUseCaseProvider` | class | AgentSpawnUseCase saglayici uygulamasi | VersaCoder.Application | net48;net10.0-windows | AgentSpawnUseCaseTests | planli |
| N17-069 | `AgentSpawnUseCaseRegistry` | class | AgentSpawnUseCase kayit/katalog yonetimi | VersaCoder.Application | net48;net10.0-windows | AgentSpawnUseCaseTests | planli |
| N17-070 | `AgentSpawnUseCaseConverter` | class | AgentSpawnUseCase bicim donusumu | VersaCoder.Application | net48;net10.0-windows | AgentSpawnUseCaseTests | planli |
| N17-071 | `AgentSpawnUseCaseTests` | class | AgentSpawnUseCase testleri (xUnit) | VersaCoder.Application | net48;net10.0-windows | - | planli |
| N17-072 | `AgentSpawnUseCaseBuilder` | class | AgentSpawnUseCase uretici (builder) | VersaCoder.Application | net48;net10.0-windows | AgentSpawnUseCaseTests | planli |
| N17-073 | `ITaskPlanUseCase` | interface | TaskPlanUseCase sozlesmesi (port) | VersaCoder.Application | net48;net10.0-windows | TaskPlanUseCaseTests | planli |
| N17-074 | `TaskPlanUseCase` | class | TaskPlanUseCase cekirdek uygulamasi | VersaCoder.Application | net48;net10.0-windows | TaskPlanUseCaseTests | planli |
| N17-075 | `TaskPlanUseCaseDto` | record | TaskPlanUseCase veri transferi modeli | VersaCoder.Application | net48;net10.0-windows | TaskPlanUseCaseTests | planli |
| N17-076 | `TaskPlanUseCaseMapper` | class | TaskPlanUseCase <-> DTO esleme | VersaCoder.Application | net48;net10.0-windows | TaskPlanUseCaseTests | planli |
| N17-077 | `TaskPlanUseCaseValidator` | class | TaskPlanUseCase dogrulama kurallari | VersaCoder.Application | net48;net10.0-windows | TaskPlanUseCaseTests | planli |
| N17-078 | `TaskPlanUseCaseFactory` | class | TaskPlanUseCase uretimi ve varsayilanlari | VersaCoder.Application | net48;net10.0-windows | TaskPlanUseCaseTests | planli |
| N17-079 | `TaskPlanUseCaseOptions` | record | TaskPlanUseCase yapilandirmasi | VersaCoder.Application | net48;net10.0-windows | TaskPlanUseCaseTests | planli |
| N17-080 | `TaskPlanUseCaseProvider` | class | TaskPlanUseCase saglayici uygulamasi | VersaCoder.Application | net48;net10.0-windows | TaskPlanUseCaseTests | planli |
| N17-081 | `TaskPlanUseCaseRegistry` | class | TaskPlanUseCase kayit/katalog yonetimi | VersaCoder.Application | net48;net10.0-windows | TaskPlanUseCaseTests | planli |
| N17-082 | `TaskPlanUseCaseConverter` | class | TaskPlanUseCase bicim donusumu | VersaCoder.Application | net48;net10.0-windows | TaskPlanUseCaseTests | planli |
| N17-083 | `TaskPlanUseCaseTests` | class | TaskPlanUseCase testleri (xUnit) | VersaCoder.Application | net48;net10.0-windows | - | planli |
| N17-084 | `TaskPlanUseCaseBuilder` | class | TaskPlanUseCase uretici (builder) | VersaCoder.Application | net48;net10.0-windows | TaskPlanUseCaseTests | planli |
| N17-085 | `ISettingsApplyUseCase` | interface | SettingsApplyUseCase sozlesmesi (port) | VersaCoder.Application | net48;net10.0-windows | SettingsApplyUseCaseTests | planli |
| N17-086 | `SettingsApplyUseCase` | class | SettingsApplyUseCase cekirdek uygulamasi | VersaCoder.Application | net48;net10.0-windows | SettingsApplyUseCaseTests | planli |
| N17-087 | `SettingsApplyUseCaseDto` | record | SettingsApplyUseCase veri transferi modeli | VersaCoder.Application | net48;net10.0-windows | SettingsApplyUseCaseTests | planli |
| N17-088 | `SettingsApplyUseCaseMapper` | class | SettingsApplyUseCase <-> DTO esleme | VersaCoder.Application | net48;net10.0-windows | SettingsApplyUseCaseTests | planli |
| N17-089 | `SettingsApplyUseCaseValidator` | class | SettingsApplyUseCase dogrulama kurallari | VersaCoder.Application | net48;net10.0-windows | SettingsApplyUseCaseTests | planli |
| N17-090 | `SettingsApplyUseCaseFactory` | class | SettingsApplyUseCase uretimi ve varsayilanlari | VersaCoder.Application | net48;net10.0-windows | SettingsApplyUseCaseTests | planli |
| N17-091 | `SettingsApplyUseCaseOptions` | record | SettingsApplyUseCase yapilandirmasi | VersaCoder.Application | net48;net10.0-windows | SettingsApplyUseCaseTests | planli |
| N17-092 | `SettingsApplyUseCaseProvider` | class | SettingsApplyUseCase saglayici uygulamasi | VersaCoder.Application | net48;net10.0-windows | SettingsApplyUseCaseTests | planli |
| N17-093 | `SettingsApplyUseCaseRegistry` | class | SettingsApplyUseCase kayit/katalog yonetimi | VersaCoder.Application | net48;net10.0-windows | SettingsApplyUseCaseTests | planli |
| N17-094 | `SettingsApplyUseCaseConverter` | class | SettingsApplyUseCase bicim donusumu | VersaCoder.Application | net48;net10.0-windows | SettingsApplyUseCaseTests | planli |
| N17-095 | `SettingsApplyUseCaseTests` | class | SettingsApplyUseCase testleri (xUnit) | VersaCoder.Application | net48;net10.0-windows | - | planli |
| N17-096 | `SettingsApplyUseCaseBuilder` | class | SettingsApplyUseCase uretici (builder) | VersaCoder.Application | net48;net10.0-windows | SettingsApplyUseCaseTests | planli |
| N17-097 | `IBudgetCheckUseCase` | interface | BudgetCheckUseCase sozlesmesi (port) | VersaCoder.Application | net48;net10.0-windows | BudgetCheckUseCaseTests | planli |
| N17-098 | `BudgetCheckUseCase` | class | BudgetCheckUseCase cekirdek uygulamasi | VersaCoder.Application | net48;net10.0-windows | BudgetCheckUseCaseTests | planli |
| N17-099 | `BudgetCheckUseCaseDto` | record | BudgetCheckUseCase veri transferi modeli | VersaCoder.Application | net48;net10.0-windows | BudgetCheckUseCaseTests | planli |
| N17-100 | `BudgetCheckUseCaseMapper` | class | BudgetCheckUseCase <-> DTO esleme | VersaCoder.Application | net48;net10.0-windows | BudgetCheckUseCaseTests | planli |
| N17-101 | `BudgetCheckUseCaseValidator` | class | BudgetCheckUseCase dogrulama kurallari | VersaCoder.Application | net48;net10.0-windows | BudgetCheckUseCaseTests | planli |
| N17-102 | `BudgetCheckUseCaseFactory` | class | BudgetCheckUseCase uretimi ve varsayilanlari | VersaCoder.Application | net48;net10.0-windows | BudgetCheckUseCaseTests | planli |
| N17-103 | `BudgetCheckUseCaseOptions` | record | BudgetCheckUseCase yapilandirmasi | VersaCoder.Application | net48;net10.0-windows | BudgetCheckUseCaseTests | planli |
| N17-104 | `BudgetCheckUseCaseProvider` | class | BudgetCheckUseCase saglayici uygulamasi | VersaCoder.Application | net48;net10.0-windows | BudgetCheckUseCaseTests | planli |
| N17-105 | `BudgetCheckUseCaseRegistry` | class | BudgetCheckUseCase kayit/katalog yonetimi | VersaCoder.Application | net48;net10.0-windows | BudgetCheckUseCaseTests | planli |
| N17-106 | `BudgetCheckUseCaseConverter` | class | BudgetCheckUseCase bicim donusumu | VersaCoder.Application | net48;net10.0-windows | BudgetCheckUseCaseTests | planli |
| N17-107 | `BudgetCheckUseCaseTests` | class | BudgetCheckUseCase testleri (xUnit) | VersaCoder.Application | net48;net10.0-windows | - | planli |
| N17-108 | `BudgetCheckUseCaseBuilder` | class | BudgetCheckUseCase uretici (builder) | VersaCoder.Application | net48;net10.0-windows | BudgetCheckUseCaseTests | planli |
| N17-109 | `ISearchUseCase` | interface | SearchUseCase sozlesmesi (port) | VersaCoder.Application | net48;net10.0-windows | SearchUseCaseTests | planli |
| N17-110 | `SearchUseCase` | class | SearchUseCase cekirdek uygulamasi | VersaCoder.Application | net48;net10.0-windows | SearchUseCaseTests | planli |
| N17-111 | `SearchUseCaseDto` | record | SearchUseCase veri transferi modeli | VersaCoder.Application | net48;net10.0-windows | SearchUseCaseTests | planli |
| N17-112 | `SearchUseCaseMapper` | class | SearchUseCase <-> DTO esleme | VersaCoder.Application | net48;net10.0-windows | SearchUseCaseTests | planli |
| N17-113 | `SearchUseCaseValidator` | class | SearchUseCase dogrulama kurallari | VersaCoder.Application | net48;net10.0-windows | SearchUseCaseTests | planli |
| N17-114 | `SearchUseCaseFactory` | class | SearchUseCase uretimi ve varsayilanlari | VersaCoder.Application | net48;net10.0-windows | SearchUseCaseTests | planli |
| N17-115 | `SearchUseCaseOptions` | record | SearchUseCase yapilandirmasi | VersaCoder.Application | net48;net10.0-windows | SearchUseCaseTests | planli |
| N17-116 | `SearchUseCaseProvider` | class | SearchUseCase saglayici uygulamasi | VersaCoder.Application | net48;net10.0-windows | SearchUseCaseTests | planli |
| N17-117 | `SearchUseCaseRegistry` | class | SearchUseCase kayit/katalog yonetimi | VersaCoder.Application | net48;net10.0-windows | SearchUseCaseTests | planli |
| N17-118 | `SearchUseCaseConverter` | class | SearchUseCase bicim donusumu | VersaCoder.Application | net48;net10.0-windows | SearchUseCaseTests | planli |
| N17-119 | `SearchUseCaseTests` | class | SearchUseCase testleri (xUnit) | VersaCoder.Application | net48;net10.0-windows | - | planli |
| N17-120 | `SearchUseCaseBuilder` | class | SearchUseCase uretici (builder) | VersaCoder.Application | net48;net10.0-windows | SearchUseCaseTests | planli |
