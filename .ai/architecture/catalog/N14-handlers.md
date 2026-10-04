---
title: "Katalog Dugumu N14 - Isleyiciler (l2)"
type: catalog
category: architecture
status: needs-validation
version: 1.0.0
updated: 2026-10-04
authority: derived
scope: "N14 handlers dugumu: kimlik, src klasoru, aile yapisi, ornek kayit ve yon kurali (ADR-013 + ADR-028)"
---

# Katalog Dugumu N14 - Isleyiciler

**Zorunlu Baglantilar:** [catalog index](index.md) · [node-inventory](node-inventory.md) · [mimari index](../index.md) · [ADR-028](../../.decisions/accepted/ADR-028-layered-baseline-9-projects.md) · [ADR-013](../../.decisions/accepted/ADR-013-layered-architecture-52-nodes.md)

> **Tek cumlelik sorumluluk:** *N14 handlers hangi projede ve klasorde yasar, ne yapar, kac kayit hedefler* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

**N14 (handlers), l2 Application katmaninin alt katmanidir: `VersaCoder.Application` projesinde `src/VersaCoder.Application/Handlers/` klasorunde yasar.** Command ve query isleyicilerini tek noktada orkestre eder. 10 aile x 12 rol = 120 kayit hedefi (ADR-013 asgari 112); durum `planli`.

Kayitlar ADR-013 2.1 biciminde 8 sutun tasir; bu dosya aile kalibini ve ornek satirlari gosterir, tam 120 satirlik liste bu dosyanin boyut sinirina sigmaz (DOĞRULAMA GEREKLI).

```text
+--------------------------------------------------+
| l2 Application -> VersaCoder.Application
| src/VersaCoder.Application/Handlers/
| 10 aile x 12 rol = 120 kayit (esik 112)
+--------------------------------------------------+
```

| Alan | Deger |
|---|---|
| Dugum | N14 |
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
| 1 | Command ve query isleyicilerini tek noktada orkestre eder | l4 katmanina dogrudan bakmak (repository portu l0 katmaninda, uygulamasi l4 uzerinde) - YOK |
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
| 1 | N14 | handlers | `src/VersaCoder.Application/Handlers/` |

**Kanit yolu (ADR-028 2.3/5):** tarama yolu gercek proje adini icerir; genis kalip (`src/**/Domain/**`) sahte PASS uretir.

```powershell
Test-Path 'C:\versacoder\src\VersaCoder.Application\Handlers'
```

Dizin su an yok cikti `False` olur; PASS yazilmaz -> **DOĞRULAMA GEREKLI**.

---

## §3 Aile Yapisi

Rol kalibi (12): `I<F>` · `<F>` · `<F>Dto` · `<F>Mapper` · `<F>Validator` · `<F>Factory` · `<F>Options` · `<F>Provider` · `<F>Registry` · `<F>Converter` · `<F>Tests` · `<F>Builder`

| # | Aile | # | Aile |
|---|---|---|---|
| 1 | SessionHandler | 6 | SettingHandler |
| 2 | PromptHandler | 7 | AgentHandler |
| 3 | ToolHandler | 8 | TaskHandler |
| 4 | FileHandler | 9 | BudgetHandler |
| 5 | ProjectHandler | 10 | AuditHandler |

Her aile ayni rol kalibini 12 kez tekrarlar; kayit sayisi = aile x rol.

---

## §4 Kayit Ornegi (8 sutun)

| ID | Bilesen | Tip | Sorumluluk | Proje | TFM | Test | Durum |
|---|---|---|---|---|---|---|---|
| N14-001 | `ISessionHandler` | interface | SessionHandler sozlesmesi (port) | VersaCoder.Application | net48;net10.0-windows | SessionHandlerTests | planli |
| N14-002 | `SessionHandler` | class | SessionHandler cekirdek uygulamasi | VersaCoder.Application | net48;net10.0-windows | SessionHandlerTests | planli |
| N14-003 | `SessionHandlerDto` | record | SessionHandler veri transferi modeli | VersaCoder.Application | net48;net10.0-windows | SessionHandlerTests | planli |
| N14-004 | `SessionHandlerMapper` | class | SessionHandler <-> DTO esleme | VersaCoder.Application | net48;net10.0-windows | SessionHandlerTests | planli |
| N14-005 | `SessionHandlerValidator` | class | SessionHandler dogrulama kurallari | VersaCoder.Application | net48;net10.0-windows | SessionHandlerTests | planli |
| N14-006 | `SessionHandlerFactory` | class | SessionHandler uretimi ve varsayilanlari | VersaCoder.Application | net48;net10.0-windows | SessionHandlerTests | planli |

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
| 3 | Test sinifi | SessionHandlerTests (xUnit) | planli |
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
(Select-String -Path 'C:\versacoder\.ai\architecture\catalog\N14-handlers.md' -Pattern '^\|\s*N\d{2}-\d{3}\s*\|').Count
# 2) boyut bandi - beklenen: 2600-8500
(Get-Item 'C:\versacoder\.ai\architecture\catalog\N14-handlers.md').Length
# 3) klasor kaniti - gercek proje adi (ADR-028 2.3/5)
(Test-Path 'C:\versacoder\src\VersaCoder.Application\Handlers')
```

Katalog sayimi `powershell -NoProfile -ExecutionPolicy Bypass -File C:\versacoder\.ai\scripts\count-components.ps1` ile alinir; bu batch sonunda toplam esik (5000) henuz karsilanmaz - kalan 26 node dosyasi sonraki batchtir.

## §10 Kayit Tablosu (ADR-013 2.2 - kaynak vault port)

| N14-007 | `SessionHandlerOptions` | record | SessionHandler yapilandirmasi | VersaCoder.Application | net48;net10.0-windows | SessionHandlerTests | planli |
| N14-008 | `SessionHandlerProvider` | class | SessionHandler saglayici uygulamasi | VersaCoder.Application | net48;net10.0-windows | SessionHandlerTests | planli |
| N14-009 | `SessionHandlerRegistry` | class | SessionHandler kayit/katalog yonetimi | VersaCoder.Application | net48;net10.0-windows | SessionHandlerTests | planli |
| N14-010 | `SessionHandlerConverter` | class | SessionHandler bicim donusumu | VersaCoder.Application | net48;net10.0-windows | SessionHandlerTests | planli |
| N14-011 | `SessionHandlerTests` | class | SessionHandler testleri (xUnit) | VersaCoder.Application | net48;net10.0-windows | - | planli |
| N14-012 | `SessionHandlerBuilder` | class | SessionHandler uretici (builder) | VersaCoder.Application | net48;net10.0-windows | SessionHandlerTests | planli |
| N14-013 | `IPromptHandler` | interface | PromptHandler sozlesmesi (port) | VersaCoder.Application | net48;net10.0-windows | PromptHandlerTests | planli |
| N14-014 | `PromptHandler` | class | PromptHandler cekirdek uygulamasi | VersaCoder.Application | net48;net10.0-windows | PromptHandlerTests | planli |
| N14-015 | `PromptHandlerDto` | record | PromptHandler veri transferi modeli | VersaCoder.Application | net48;net10.0-windows | PromptHandlerTests | planli |
| N14-016 | `PromptHandlerMapper` | class | PromptHandler <-> DTO esleme | VersaCoder.Application | net48;net10.0-windows | PromptHandlerTests | planli |
| N14-017 | `PromptHandlerValidator` | class | PromptHandler dogrulama kurallari | VersaCoder.Application | net48;net10.0-windows | PromptHandlerTests | planli |
| N14-018 | `PromptHandlerFactory` | class | PromptHandler uretimi ve varsayilanlari | VersaCoder.Application | net48;net10.0-windows | PromptHandlerTests | planli |
| N14-019 | `PromptHandlerOptions` | record | PromptHandler yapilandirmasi | VersaCoder.Application | net48;net10.0-windows | PromptHandlerTests | planli |
| N14-020 | `PromptHandlerProvider` | class | PromptHandler saglayici uygulamasi | VersaCoder.Application | net48;net10.0-windows | PromptHandlerTests | planli |
| N14-021 | `PromptHandlerRegistry` | class | PromptHandler kayit/katalog yonetimi | VersaCoder.Application | net48;net10.0-windows | PromptHandlerTests | planli |
| N14-022 | `PromptHandlerConverter` | class | PromptHandler bicim donusumu | VersaCoder.Application | net48;net10.0-windows | PromptHandlerTests | planli |
| N14-023 | `PromptHandlerTests` | class | PromptHandler testleri (xUnit) | VersaCoder.Application | net48;net10.0-windows | - | planli |
| N14-024 | `PromptHandlerBuilder` | class | PromptHandler uretici (builder) | VersaCoder.Application | net48;net10.0-windows | PromptHandlerTests | planli |
| N14-025 | `IToolHandler` | interface | ToolHandler sozlesmesi (port) | VersaCoder.Application | net48;net10.0-windows | ToolHandlerTests | planli |
| N14-026 | `ToolHandler` | class | ToolHandler cekirdek uygulamasi | VersaCoder.Application | net48;net10.0-windows | ToolHandlerTests | planli |
| N14-027 | `ToolHandlerDto` | record | ToolHandler veri transferi modeli | VersaCoder.Application | net48;net10.0-windows | ToolHandlerTests | planli |
| N14-028 | `ToolHandlerMapper` | class | ToolHandler <-> DTO esleme | VersaCoder.Application | net48;net10.0-windows | ToolHandlerTests | planli |
| N14-029 | `ToolHandlerValidator` | class | ToolHandler dogrulama kurallari | VersaCoder.Application | net48;net10.0-windows | ToolHandlerTests | planli |
| N14-030 | `ToolHandlerFactory` | class | ToolHandler uretimi ve varsayilanlari | VersaCoder.Application | net48;net10.0-windows | ToolHandlerTests | planli |
| N14-031 | `ToolHandlerOptions` | record | ToolHandler yapilandirmasi | VersaCoder.Application | net48;net10.0-windows | ToolHandlerTests | planli |
| N14-032 | `ToolHandlerProvider` | class | ToolHandler saglayici uygulamasi | VersaCoder.Application | net48;net10.0-windows | ToolHandlerTests | planli |
| N14-033 | `ToolHandlerRegistry` | class | ToolHandler kayit/katalog yonetimi | VersaCoder.Application | net48;net10.0-windows | ToolHandlerTests | planli |
| N14-034 | `ToolHandlerConverter` | class | ToolHandler bicim donusumu | VersaCoder.Application | net48;net10.0-windows | ToolHandlerTests | planli |
| N14-035 | `ToolHandlerTests` | class | ToolHandler testleri (xUnit) | VersaCoder.Application | net48;net10.0-windows | - | planli |
| N14-036 | `ToolHandlerBuilder` | class | ToolHandler uretici (builder) | VersaCoder.Application | net48;net10.0-windows | ToolHandlerTests | planli |
| N14-037 | `IFileHandler` | interface | FileHandler sozlesmesi (port) | VersaCoder.Application | net48;net10.0-windows | FileHandlerTests | planli |
| N14-038 | `FileHandler` | class | FileHandler cekirdek uygulamasi | VersaCoder.Application | net48;net10.0-windows | FileHandlerTests | planli |
| N14-039 | `FileHandlerDto` | record | FileHandler veri transferi modeli | VersaCoder.Application | net48;net10.0-windows | FileHandlerTests | planli |
| N14-040 | `FileHandlerMapper` | class | FileHandler <-> DTO esleme | VersaCoder.Application | net48;net10.0-windows | FileHandlerTests | planli |
| N14-041 | `FileHandlerValidator` | class | FileHandler dogrulama kurallari | VersaCoder.Application | net48;net10.0-windows | FileHandlerTests | planli |
| N14-042 | `FileHandlerFactory` | class | FileHandler uretimi ve varsayilanlari | VersaCoder.Application | net48;net10.0-windows | FileHandlerTests | planli |
| N14-043 | `FileHandlerOptions` | record | FileHandler yapilandirmasi | VersaCoder.Application | net48;net10.0-windows | FileHandlerTests | planli |
| N14-044 | `FileHandlerProvider` | class | FileHandler saglayici uygulamasi | VersaCoder.Application | net48;net10.0-windows | FileHandlerTests | planli |
| N14-045 | `FileHandlerRegistry` | class | FileHandler kayit/katalog yonetimi | VersaCoder.Application | net48;net10.0-windows | FileHandlerTests | planli |
| N14-046 | `FileHandlerConverter` | class | FileHandler bicim donusumu | VersaCoder.Application | net48;net10.0-windows | FileHandlerTests | planli |
| N14-047 | `FileHandlerTests` | class | FileHandler testleri (xUnit) | VersaCoder.Application | net48;net10.0-windows | - | planli |
| N14-048 | `FileHandlerBuilder` | class | FileHandler uretici (builder) | VersaCoder.Application | net48;net10.0-windows | FileHandlerTests | planli |
| N14-049 | `IProjectHandler` | interface | ProjectHandler sozlesmesi (port) | VersaCoder.Application | net48;net10.0-windows | ProjectHandlerTests | planli |
| N14-050 | `ProjectHandler` | class | ProjectHandler cekirdek uygulamasi | VersaCoder.Application | net48;net10.0-windows | ProjectHandlerTests | planli |
| N14-051 | `ProjectHandlerDto` | record | ProjectHandler veri transferi modeli | VersaCoder.Application | net48;net10.0-windows | ProjectHandlerTests | planli |
| N14-052 | `ProjectHandlerMapper` | class | ProjectHandler <-> DTO esleme | VersaCoder.Application | net48;net10.0-windows | ProjectHandlerTests | planli |
| N14-053 | `ProjectHandlerValidator` | class | ProjectHandler dogrulama kurallari | VersaCoder.Application | net48;net10.0-windows | ProjectHandlerTests | planli |
| N14-054 | `ProjectHandlerFactory` | class | ProjectHandler uretimi ve varsayilanlari | VersaCoder.Application | net48;net10.0-windows | ProjectHandlerTests | planli |
| N14-055 | `ProjectHandlerOptions` | record | ProjectHandler yapilandirmasi | VersaCoder.Application | net48;net10.0-windows | ProjectHandlerTests | planli |
| N14-056 | `ProjectHandlerProvider` | class | ProjectHandler saglayici uygulamasi | VersaCoder.Application | net48;net10.0-windows | ProjectHandlerTests | planli |
| N14-057 | `ProjectHandlerRegistry` | class | ProjectHandler kayit/katalog yonetimi | VersaCoder.Application | net48;net10.0-windows | ProjectHandlerTests | planli |
| N14-058 | `ProjectHandlerConverter` | class | ProjectHandler bicim donusumu | VersaCoder.Application | net48;net10.0-windows | ProjectHandlerTests | planli |
| N14-059 | `ProjectHandlerTests` | class | ProjectHandler testleri (xUnit) | VersaCoder.Application | net48;net10.0-windows | - | planli |
| N14-060 | `ProjectHandlerBuilder` | class | ProjectHandler uretici (builder) | VersaCoder.Application | net48;net10.0-windows | ProjectHandlerTests | planli |
| N14-061 | `ISettingHandler` | interface | SettingHandler sozlesmesi (port) | VersaCoder.Application | net48;net10.0-windows | SettingHandlerTests | planli |
| N14-062 | `SettingHandler` | class | SettingHandler cekirdek uygulamasi | VersaCoder.Application | net48;net10.0-windows | SettingHandlerTests | planli |
| N14-063 | `SettingHandlerDto` | record | SettingHandler veri transferi modeli | VersaCoder.Application | net48;net10.0-windows | SettingHandlerTests | planli |
| N14-064 | `SettingHandlerMapper` | class | SettingHandler <-> DTO esleme | VersaCoder.Application | net48;net10.0-windows | SettingHandlerTests | planli |
| N14-065 | `SettingHandlerValidator` | class | SettingHandler dogrulama kurallari | VersaCoder.Application | net48;net10.0-windows | SettingHandlerTests | planli |
| N14-066 | `SettingHandlerFactory` | class | SettingHandler uretimi ve varsayilanlari | VersaCoder.Application | net48;net10.0-windows | SettingHandlerTests | planli |
| N14-067 | `SettingHandlerOptions` | record | SettingHandler yapilandirmasi | VersaCoder.Application | net48;net10.0-windows | SettingHandlerTests | planli |
| N14-068 | `SettingHandlerProvider` | class | SettingHandler saglayici uygulamasi | VersaCoder.Application | net48;net10.0-windows | SettingHandlerTests | planli |
| N14-069 | `SettingHandlerRegistry` | class | SettingHandler kayit/katalog yonetimi | VersaCoder.Application | net48;net10.0-windows | SettingHandlerTests | planli |
| N14-070 | `SettingHandlerConverter` | class | SettingHandler bicim donusumu | VersaCoder.Application | net48;net10.0-windows | SettingHandlerTests | planli |
| N14-071 | `SettingHandlerTests` | class | SettingHandler testleri (xUnit) | VersaCoder.Application | net48;net10.0-windows | - | planli |
| N14-072 | `SettingHandlerBuilder` | class | SettingHandler uretici (builder) | VersaCoder.Application | net48;net10.0-windows | SettingHandlerTests | planli |
| N14-073 | `IAgentHandler` | interface | AgentHandler sozlesmesi (port) | VersaCoder.Application | net48;net10.0-windows | AgentHandlerTests | planli |
| N14-074 | `AgentHandler` | class | AgentHandler cekirdek uygulamasi | VersaCoder.Application | net48;net10.0-windows | AgentHandlerTests | planli |
| N14-075 | `AgentHandlerDto` | record | AgentHandler veri transferi modeli | VersaCoder.Application | net48;net10.0-windows | AgentHandlerTests | planli |
| N14-076 | `AgentHandlerMapper` | class | AgentHandler <-> DTO esleme | VersaCoder.Application | net48;net10.0-windows | AgentHandlerTests | planli |
| N14-077 | `AgentHandlerValidator` | class | AgentHandler dogrulama kurallari | VersaCoder.Application | net48;net10.0-windows | AgentHandlerTests | planli |
| N14-078 | `AgentHandlerFactory` | class | AgentHandler uretimi ve varsayilanlari | VersaCoder.Application | net48;net10.0-windows | AgentHandlerTests | planli |
| N14-079 | `AgentHandlerOptions` | record | AgentHandler yapilandirmasi | VersaCoder.Application | net48;net10.0-windows | AgentHandlerTests | planli |
| N14-080 | `AgentHandlerProvider` | class | AgentHandler saglayici uygulamasi | VersaCoder.Application | net48;net10.0-windows | AgentHandlerTests | planli |
| N14-081 | `AgentHandlerRegistry` | class | AgentHandler kayit/katalog yonetimi | VersaCoder.Application | net48;net10.0-windows | AgentHandlerTests | planli |
| N14-082 | `AgentHandlerConverter` | class | AgentHandler bicim donusumu | VersaCoder.Application | net48;net10.0-windows | AgentHandlerTests | planli |
| N14-083 | `AgentHandlerTests` | class | AgentHandler testleri (xUnit) | VersaCoder.Application | net48;net10.0-windows | - | planli |
| N14-084 | `AgentHandlerBuilder` | class | AgentHandler uretici (builder) | VersaCoder.Application | net48;net10.0-windows | AgentHandlerTests | planli |
| N14-085 | `ITaskHandler` | interface | TaskHandler sozlesmesi (port) | VersaCoder.Application | net48;net10.0-windows | TaskHandlerTests | planli |
| N14-086 | `TaskHandler` | class | TaskHandler cekirdek uygulamasi | VersaCoder.Application | net48;net10.0-windows | TaskHandlerTests | planli |
| N14-087 | `TaskHandlerDto` | record | TaskHandler veri transferi modeli | VersaCoder.Application | net48;net10.0-windows | TaskHandlerTests | planli |
| N14-088 | `TaskHandlerMapper` | class | TaskHandler <-> DTO esleme | VersaCoder.Application | net48;net10.0-windows | TaskHandlerTests | planli |
| N14-089 | `TaskHandlerValidator` | class | TaskHandler dogrulama kurallari | VersaCoder.Application | net48;net10.0-windows | TaskHandlerTests | planli |
| N14-090 | `TaskHandlerFactory` | class | TaskHandler uretimi ve varsayilanlari | VersaCoder.Application | net48;net10.0-windows | TaskHandlerTests | planli |
| N14-091 | `TaskHandlerOptions` | record | TaskHandler yapilandirmasi | VersaCoder.Application | net48;net10.0-windows | TaskHandlerTests | planli |
| N14-092 | `TaskHandlerProvider` | class | TaskHandler saglayici uygulamasi | VersaCoder.Application | net48;net10.0-windows | TaskHandlerTests | planli |
| N14-093 | `TaskHandlerRegistry` | class | TaskHandler kayit/katalog yonetimi | VersaCoder.Application | net48;net10.0-windows | TaskHandlerTests | planli |
| N14-094 | `TaskHandlerConverter` | class | TaskHandler bicim donusumu | VersaCoder.Application | net48;net10.0-windows | TaskHandlerTests | planli |
| N14-095 | `TaskHandlerTests` | class | TaskHandler testleri (xUnit) | VersaCoder.Application | net48;net10.0-windows | - | planli |
| N14-096 | `TaskHandlerBuilder` | class | TaskHandler uretici (builder) | VersaCoder.Application | net48;net10.0-windows | TaskHandlerTests | planli |
| N14-097 | `IBudgetHandler` | interface | BudgetHandler sozlesmesi (port) | VersaCoder.Application | net48;net10.0-windows | BudgetHandlerTests | planli |
| N14-098 | `BudgetHandler` | class | BudgetHandler cekirdek uygulamasi | VersaCoder.Application | net48;net10.0-windows | BudgetHandlerTests | planli |
| N14-099 | `BudgetHandlerDto` | record | BudgetHandler veri transferi modeli | VersaCoder.Application | net48;net10.0-windows | BudgetHandlerTests | planli |
| N14-100 | `BudgetHandlerMapper` | class | BudgetHandler <-> DTO esleme | VersaCoder.Application | net48;net10.0-windows | BudgetHandlerTests | planli |
| N14-101 | `BudgetHandlerValidator` | class | BudgetHandler dogrulama kurallari | VersaCoder.Application | net48;net10.0-windows | BudgetHandlerTests | planli |
| N14-102 | `BudgetHandlerFactory` | class | BudgetHandler uretimi ve varsayilanlari | VersaCoder.Application | net48;net10.0-windows | BudgetHandlerTests | planli |
| N14-103 | `BudgetHandlerOptions` | record | BudgetHandler yapilandirmasi | VersaCoder.Application | net48;net10.0-windows | BudgetHandlerTests | planli |
| N14-104 | `BudgetHandlerProvider` | class | BudgetHandler saglayici uygulamasi | VersaCoder.Application | net48;net10.0-windows | BudgetHandlerTests | planli |
| N14-105 | `BudgetHandlerRegistry` | class | BudgetHandler kayit/katalog yonetimi | VersaCoder.Application | net48;net10.0-windows | BudgetHandlerTests | planli |
| N14-106 | `BudgetHandlerConverter` | class | BudgetHandler bicim donusumu | VersaCoder.Application | net48;net10.0-windows | BudgetHandlerTests | planli |
| N14-107 | `BudgetHandlerTests` | class | BudgetHandler testleri (xUnit) | VersaCoder.Application | net48;net10.0-windows | - | planli |
| N14-108 | `BudgetHandlerBuilder` | class | BudgetHandler uretici (builder) | VersaCoder.Application | net48;net10.0-windows | BudgetHandlerTests | planli |
| N14-109 | `IAuditHandler` | interface | AuditHandler sozlesmesi (port) | VersaCoder.Application | net48;net10.0-windows | AuditHandlerTests | planli |
| N14-110 | `AuditHandler` | class | AuditHandler cekirdek uygulamasi | VersaCoder.Application | net48;net10.0-windows | AuditHandlerTests | planli |
| N14-111 | `AuditHandlerDto` | record | AuditHandler veri transferi modeli | VersaCoder.Application | net48;net10.0-windows | AuditHandlerTests | planli |
| N14-112 | `AuditHandlerMapper` | class | AuditHandler <-> DTO esleme | VersaCoder.Application | net48;net10.0-windows | AuditHandlerTests | planli |
| N14-113 | `AuditHandlerValidator` | class | AuditHandler dogrulama kurallari | VersaCoder.Application | net48;net10.0-windows | AuditHandlerTests | planli |
| N14-114 | `AuditHandlerFactory` | class | AuditHandler uretimi ve varsayilanlari | VersaCoder.Application | net48;net10.0-windows | AuditHandlerTests | planli |
| N14-115 | `AuditHandlerOptions` | record | AuditHandler yapilandirmasi | VersaCoder.Application | net48;net10.0-windows | AuditHandlerTests | planli |
| N14-116 | `AuditHandlerProvider` | class | AuditHandler saglayici uygulamasi | VersaCoder.Application | net48;net10.0-windows | AuditHandlerTests | planli |
| N14-117 | `AuditHandlerRegistry` | class | AuditHandler kayit/katalog yonetimi | VersaCoder.Application | net48;net10.0-windows | AuditHandlerTests | planli |
| N14-118 | `AuditHandlerConverter` | class | AuditHandler bicim donusumu | VersaCoder.Application | net48;net10.0-windows | AuditHandlerTests | planli |
| N14-119 | `AuditHandlerTests` | class | AuditHandler testleri (xUnit) | VersaCoder.Application | net48;net10.0-windows | - | planli |
| N14-120 | `AuditHandlerBuilder` | class | AuditHandler uretici (builder) | VersaCoder.Application | net48;net10.0-windows | AuditHandlerTests | planli |
