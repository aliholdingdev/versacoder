---
title: "Katalog Dugumu N08 - Aggregate Kokleri (l1)"
type: catalog
category: architecture
status: needs-validation
version: 1.0.0
updated: 2026-10-04
authority: derived
scope: "N08 aggregates dugumu: kimlik, src klasoru, aile yapisi, ornek kayit ve yon kurali (ADR-013 + ADR-028)"
---

# Katalog Dugumu N08 - Aggregate Kokleri

**Zorunlu Baglantilar:** [catalog index](index.md) · [node-inventory](node-inventory.md) · [mimari index](../index.md) · [ADR-028](../../.decisions/accepted/ADR-028-layered-baseline-9-projects.md) · [ADR-013](../../.decisions/accepted/ADR-013-layered-architecture-52-nodes.md)

> **Tek cumlelik sorumluluk:** *N08 aggregates hangi projede ve klasorde yasar, ne yapar, kac kayit hedefler* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

**N08 (aggregates), l1 Domain katmaninin alt katmanidir: `VersaCoder.Domain` projesinde `src/VersaCoder.Domain/Aggregates/` klasorunde yasar.** Tutarlilik siniri tasiyan aggregate koklerini (SessionAggregate, ProjectAggregate) tanimlar. 10 aile x 12 rol = 120 kayit hedefi (ADR-013 asgari 112); durum `planli`.

Kayitlar ADR-013 2.1 biciminde 8 sutun tasir; bu dosya aile kalibini ve ornek satirlari gosterir, tam 120 satirlik liste bu dosyanin boyut sinirina sigmaz (DOĞRULAMA GEREKLI).

```text
+--------------------------------------------------+
| l1 Domain -> VersaCoder.Domain
| src/VersaCoder.Domain/Aggregates/
| 10 aile x 12 rol = 120 kayit (esik 112)
+--------------------------------------------------+
```

| Alan | Deger |
|---|---|
| Dugum | N08 |
| Dugum turu | alt katman (klasor/namespace duzeyi) |
| Makro katman | l1 Domain |
| Proje (.csproj) | VersaCoder.Domain |
| TFM | net48;net10.0-windows |
| Kayit hedefi | 120 (ADR-013 asgari 112) |
| Durum | planli |
| Sorumlu rol | architect |

---

## §1 Sorumluluk (YAPAR / YAPMAZ)

| # | YAPAR | YAPMAZ |
|---|---|---|
| 1 | Tutarlilik siniri tasiyan aggregate koklerini (SessionAggregate, ProjectAggregate) tanimlar | System.Data, Dapper, dosya veya git cagirmak |
| 2 | kayitlari 8 sutunlu ADR-013 biciminde tutmak (planli -> yapildi -> testli) | kullanim senaryosu orkestre etmek, is l2 katmaninda |
| 3 | entity, value object ve aggregate tasarlamak | SQL veya migration yazmak, is l4 katmaninda |
| 4 | invariant ve is kuralini saf C# icinde kodlamak | UI ekranı veya mesaj kutusu acmak |
| 5 | domain event uretmek | l2..l7 katmanlarina referans vermek |
| 6 | l0 katmaninin sozlesmelerini kullanmak | framework bagimliligi getirmek (or. EntityFramework) |

---

## §2 Klasor ve N Eslemesi

**Kalip (ADR-028 2.3/4):** `N33 -> src\VersaCoder.Protocol\ToolSchema\` satiri SSOT ornegidir; bu dosyanin klasoru ayni kalipla yazilir.

| # | N | Slug | src/ klasoru |
|---|---|---|---|
| 1 | N08 | aggregates | `src/VersaCoder.Domain/Aggregates/` |

**Kanit yolu (ADR-028 2.3/5):** tarama yolu gercek proje adini icerir; genis kalip (`src/**/Domain/**`) sahte PASS uretir.

```powershell
Test-Path 'C:\versacoder\src\VersaCoder.Domain\Aggregates'
```

Dizin su an yok cikti `False` olur; PASS yazilmaz -> **DOĞRULAMA GEREKLI**.

---

## §3 Aile Yapisi

Rol kalibi (12): `I<F>` · `<F>` · `<F>Dto` · `<F>Mapper` · `<F>Validator` · `<F>Factory` · `<F>Options` · `<F>Provider` · `<F>Registry` · `<F>Converter` · `<F>Tests` · `<F>Builder`

| # | Aile | # | Aile |
|---|---|---|---|
| 1 | SessionAggregate | 6 | ConversationAggregate |
| 2 | ProjectAggregate | 7 | WorkspaceAggregate |
| 3 | AgentAggregate | 8 | ToolchainAggregate |
| 4 | TaskAggregate | 9 | PolicyAggregate |
| 5 | SettingsAggregate | 10 | BudgetAggregate |

Her aile ayni rol kalibini 12 kez tekrarlar; kayit sayisi = aile x rol.

---

## §4 Kayit Ornegi (8 sutun)

| ID | Bilesen | Tip | Sorumluluk | Proje | TFM | Test | Durum |
|---|---|---|---|---|---|---|---|
| N08-001 | `ISessionAggregate` | interface | SessionAggregate sozlesmesi (port) | VersaCoder.Domain | net48;net10.0-windows | SessionAggregateTests | planli |
| N08-002 | `SessionAggregate` | class | SessionAggregate cekirdek uygulamasi | VersaCoder.Domain | net48;net10.0-windows | SessionAggregateTests | planli |
| N08-003 | `SessionAggregateDto` | record | SessionAggregate veri transferi modeli | VersaCoder.Domain | net48;net10.0-windows | SessionAggregateTests | planli |
| N08-004 | `SessionAggregateMapper` | class | SessionAggregate <-> DTO esleme | VersaCoder.Domain | net48;net10.0-windows | SessionAggregateTests | planli |
| N08-005 | `SessionAggregateValidator` | class | SessionAggregate dogrulama kurallari | VersaCoder.Domain | net48;net10.0-windows | SessionAggregateTests | planli |
| N08-006 | `SessionAggregateFactory` | class | SessionAggregate uretimi ve varsayilanlari | VersaCoder.Domain | net48;net10.0-windows | SessionAggregateTests | planli |

Kalan aileler ayni kalibi 12 rolle tekrarlar (makro dugumlerde 4 rol); toplam hedef **120** kayit.

---

## §5 Yon Kurali

| # | Yon | Deger | Kaynak |
|---|---|---|---|
| 1 | l1 -> l0 | OK | ADR-028 2.1 |
| 2 | l1 -> l2..l7 | X | ADR-028 2.1 |
| 3 | l2 -> l1 | OK | ADR-028 2.1 |

Matrisin tamami [layer-matrix](layer-matrix.md) dosyasindadir; NetArchTest `LayerRulesTests` bu tabloya gore yazilir.

---

## §6 Test ve Durum

| # | Kontrol | Beklenen | Durum |
|---|---|---|---|
| 1 | Kayit satiri | 120 (asgari 112) | bu dosyada 6 ornek satir; tam liste DOĞRULAMA GEREKLI |
| 2 | Durum degeri | planli -> yapildi -> testli | planli |
| 3 | Test sinifi | SessionAggregateTests (xUnit) | planli |
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
(Select-String -Path 'C:\versacoder\.ai\architecture\catalog\N08-aggregates.md' -Pattern '^\|\s*N\d{2}-\d{3}\s*\|').Count
# 2) boyut bandi - beklenen: 2600-8500
(Get-Item 'C:\versacoder\.ai\architecture\catalog\N08-aggregates.md').Length
# 3) klasor kaniti - gercek proje adi (ADR-028 2.3/5)
(Test-Path 'C:\versacoder\src\VersaCoder.Domain\Aggregates')
```

Katalog sayimi `powershell -NoProfile -ExecutionPolicy Bypass -File C:\versacoder\.ai\scripts\count-components.ps1` ile alinir; bu batch sonunda toplam esik (5000) henuz karsilanmaz - kalan 26 node dosyasi sonraki batchtir.

## §10 Kayit Tablosu (ADR-013 2.2 - kaynak vault port)

| N08-007 | `SessionAggregateOptions` | record | SessionAggregate yapilandirmasi | VersaCoder.Domain | net48;net10.0-windows | SessionAggregateTests | planli |
| N08-008 | `SessionAggregateProvider` | class | SessionAggregate saglayici uygulamasi | VersaCoder.Domain | net48;net10.0-windows | SessionAggregateTests | planli |
| N08-009 | `SessionAggregateRegistry` | class | SessionAggregate kayit/katalog yonetimi | VersaCoder.Domain | net48;net10.0-windows | SessionAggregateTests | planli |
| N08-010 | `SessionAggregateConverter` | class | SessionAggregate bicim donusumu | VersaCoder.Domain | net48;net10.0-windows | SessionAggregateTests | planli |
| N08-011 | `SessionAggregateTests` | class | SessionAggregate testleri (xUnit) | VersaCoder.Domain | net48;net10.0-windows | - | planli |
| N08-012 | `SessionAggregateBuilder` | class | SessionAggregate uretici (builder) | VersaCoder.Domain | net48;net10.0-windows | SessionAggregateTests | planli |
| N08-013 | `IProjectAggregate` | interface | ProjectAggregate sozlesmesi (port) | VersaCoder.Domain | net48;net10.0-windows | ProjectAggregateTests | planli |
| N08-014 | `ProjectAggregate` | class | ProjectAggregate cekirdek uygulamasi | VersaCoder.Domain | net48;net10.0-windows | ProjectAggregateTests | planli |
| N08-015 | `ProjectAggregateDto` | record | ProjectAggregate veri transferi modeli | VersaCoder.Domain | net48;net10.0-windows | ProjectAggregateTests | planli |
| N08-016 | `ProjectAggregateMapper` | class | ProjectAggregate <-> DTO esleme | VersaCoder.Domain | net48;net10.0-windows | ProjectAggregateTests | planli |
| N08-017 | `ProjectAggregateValidator` | class | ProjectAggregate dogrulama kurallari | VersaCoder.Domain | net48;net10.0-windows | ProjectAggregateTests | planli |
| N08-018 | `ProjectAggregateFactory` | class | ProjectAggregate uretimi ve varsayilanlari | VersaCoder.Domain | net48;net10.0-windows | ProjectAggregateTests | planli |
| N08-019 | `ProjectAggregateOptions` | record | ProjectAggregate yapilandirmasi | VersaCoder.Domain | net48;net10.0-windows | ProjectAggregateTests | planli |
| N08-020 | `ProjectAggregateProvider` | class | ProjectAggregate saglayici uygulamasi | VersaCoder.Domain | net48;net10.0-windows | ProjectAggregateTests | planli |
| N08-021 | `ProjectAggregateRegistry` | class | ProjectAggregate kayit/katalog yonetimi | VersaCoder.Domain | net48;net10.0-windows | ProjectAggregateTests | planli |
| N08-022 | `ProjectAggregateConverter` | class | ProjectAggregate bicim donusumu | VersaCoder.Domain | net48;net10.0-windows | ProjectAggregateTests | planli |
| N08-023 | `ProjectAggregateTests` | class | ProjectAggregate testleri (xUnit) | VersaCoder.Domain | net48;net10.0-windows | - | planli |
| N08-024 | `ProjectAggregateBuilder` | class | ProjectAggregate uretici (builder) | VersaCoder.Domain | net48;net10.0-windows | ProjectAggregateTests | planli |
| N08-025 | `IAgentAggregate` | interface | AgentAggregate sozlesmesi (port) | VersaCoder.Domain | net48;net10.0-windows | AgentAggregateTests | planli |
| N08-026 | `AgentAggregate` | class | AgentAggregate cekirdek uygulamasi | VersaCoder.Domain | net48;net10.0-windows | AgentAggregateTests | planli |
| N08-027 | `AgentAggregateDto` | record | AgentAggregate veri transferi modeli | VersaCoder.Domain | net48;net10.0-windows | AgentAggregateTests | planli |
| N08-028 | `AgentAggregateMapper` | class | AgentAggregate <-> DTO esleme | VersaCoder.Domain | net48;net10.0-windows | AgentAggregateTests | planli |
| N08-029 | `AgentAggregateValidator` | class | AgentAggregate dogrulama kurallari | VersaCoder.Domain | net48;net10.0-windows | AgentAggregateTests | planli |
| N08-030 | `AgentAggregateFactory` | class | AgentAggregate uretimi ve varsayilanlari | VersaCoder.Domain | net48;net10.0-windows | AgentAggregateTests | planli |
| N08-031 | `AgentAggregateOptions` | record | AgentAggregate yapilandirmasi | VersaCoder.Domain | net48;net10.0-windows | AgentAggregateTests | planli |
| N08-032 | `AgentAggregateProvider` | class | AgentAggregate saglayici uygulamasi | VersaCoder.Domain | net48;net10.0-windows | AgentAggregateTests | planli |
| N08-033 | `AgentAggregateRegistry` | class | AgentAggregate kayit/katalog yonetimi | VersaCoder.Domain | net48;net10.0-windows | AgentAggregateTests | planli |
| N08-034 | `AgentAggregateConverter` | class | AgentAggregate bicim donusumu | VersaCoder.Domain | net48;net10.0-windows | AgentAggregateTests | planli |
| N08-035 | `AgentAggregateTests` | class | AgentAggregate testleri (xUnit) | VersaCoder.Domain | net48;net10.0-windows | - | planli |
| N08-036 | `AgentAggregateBuilder` | class | AgentAggregate uretici (builder) | VersaCoder.Domain | net48;net10.0-windows | AgentAggregateTests | planli |
| N08-037 | `ITaskAggregate` | interface | TaskAggregate sozlesmesi (port) | VersaCoder.Domain | net48;net10.0-windows | TaskAggregateTests | planli |
| N08-038 | `TaskAggregate` | class | TaskAggregate cekirdek uygulamasi | VersaCoder.Domain | net48;net10.0-windows | TaskAggregateTests | planli |
| N08-039 | `TaskAggregateDto` | record | TaskAggregate veri transferi modeli | VersaCoder.Domain | net48;net10.0-windows | TaskAggregateTests | planli |
| N08-040 | `TaskAggregateMapper` | class | TaskAggregate <-> DTO esleme | VersaCoder.Domain | net48;net10.0-windows | TaskAggregateTests | planli |
| N08-041 | `TaskAggregateValidator` | class | TaskAggregate dogrulama kurallari | VersaCoder.Domain | net48;net10.0-windows | TaskAggregateTests | planli |
| N08-042 | `TaskAggregateFactory` | class | TaskAggregate uretimi ve varsayilanlari | VersaCoder.Domain | net48;net10.0-windows | TaskAggregateTests | planli |
| N08-043 | `TaskAggregateOptions` | record | TaskAggregate yapilandirmasi | VersaCoder.Domain | net48;net10.0-windows | TaskAggregateTests | planli |
| N08-044 | `TaskAggregateProvider` | class | TaskAggregate saglayici uygulamasi | VersaCoder.Domain | net48;net10.0-windows | TaskAggregateTests | planli |
| N08-045 | `TaskAggregateRegistry` | class | TaskAggregate kayit/katalog yonetimi | VersaCoder.Domain | net48;net10.0-windows | TaskAggregateTests | planli |
| N08-046 | `TaskAggregateConverter` | class | TaskAggregate bicim donusumu | VersaCoder.Domain | net48;net10.0-windows | TaskAggregateTests | planli |
| N08-047 | `TaskAggregateTests` | class | TaskAggregate testleri (xUnit) | VersaCoder.Domain | net48;net10.0-windows | - | planli |
| N08-048 | `TaskAggregateBuilder` | class | TaskAggregate uretici (builder) | VersaCoder.Domain | net48;net10.0-windows | TaskAggregateTests | planli |
| N08-049 | `ISettingsAggregate` | interface | SettingsAggregate sozlesmesi (port) | VersaCoder.Domain | net48;net10.0-windows | SettingsAggregateTests | planli |
| N08-050 | `SettingsAggregate` | class | SettingsAggregate cekirdek uygulamasi | VersaCoder.Domain | net48;net10.0-windows | SettingsAggregateTests | planli |
| N08-051 | `SettingsAggregateDto` | record | SettingsAggregate veri transferi modeli | VersaCoder.Domain | net48;net10.0-windows | SettingsAggregateTests | planli |
| N08-052 | `SettingsAggregateMapper` | class | SettingsAggregate <-> DTO esleme | VersaCoder.Domain | net48;net10.0-windows | SettingsAggregateTests | planli |
| N08-053 | `SettingsAggregateValidator` | class | SettingsAggregate dogrulama kurallari | VersaCoder.Domain | net48;net10.0-windows | SettingsAggregateTests | planli |
| N08-054 | `SettingsAggregateFactory` | class | SettingsAggregate uretimi ve varsayilanlari | VersaCoder.Domain | net48;net10.0-windows | SettingsAggregateTests | planli |
| N08-055 | `SettingsAggregateOptions` | record | SettingsAggregate yapilandirmasi | VersaCoder.Domain | net48;net10.0-windows | SettingsAggregateTests | planli |
| N08-056 | `SettingsAggregateProvider` | class | SettingsAggregate saglayici uygulamasi | VersaCoder.Domain | net48;net10.0-windows | SettingsAggregateTests | planli |
| N08-057 | `SettingsAggregateRegistry` | class | SettingsAggregate kayit/katalog yonetimi | VersaCoder.Domain | net48;net10.0-windows | SettingsAggregateTests | planli |
| N08-058 | `SettingsAggregateConverter` | class | SettingsAggregate bicim donusumu | VersaCoder.Domain | net48;net10.0-windows | SettingsAggregateTests | planli |
| N08-059 | `SettingsAggregateTests` | class | SettingsAggregate testleri (xUnit) | VersaCoder.Domain | net48;net10.0-windows | - | planli |
| N08-060 | `SettingsAggregateBuilder` | class | SettingsAggregate uretici (builder) | VersaCoder.Domain | net48;net10.0-windows | SettingsAggregateTests | planli |
| N08-061 | `IConversationAggregate` | interface | ConversationAggregate sozlesmesi (port) | VersaCoder.Domain | net48;net10.0-windows | ConversationAggregateTests | planli |
| N08-062 | `ConversationAggregate` | class | ConversationAggregate cekirdek uygulamasi | VersaCoder.Domain | net48;net10.0-windows | ConversationAggregateTests | planli |
| N08-063 | `ConversationAggregateDto` | record | ConversationAggregate veri transferi modeli | VersaCoder.Domain | net48;net10.0-windows | ConversationAggregateTests | planli |
| N08-064 | `ConversationAggregateMapper` | class | ConversationAggregate <-> DTO esleme | VersaCoder.Domain | net48;net10.0-windows | ConversationAggregateTests | planli |
| N08-065 | `ConversationAggregateValidator` | class | ConversationAggregate dogrulama kurallari | VersaCoder.Domain | net48;net10.0-windows | ConversationAggregateTests | planli |
| N08-066 | `ConversationAggregateFactory` | class | ConversationAggregate uretimi ve varsayilanlari | VersaCoder.Domain | net48;net10.0-windows | ConversationAggregateTests | planli |
| N08-067 | `ConversationAggregateOptions` | record | ConversationAggregate yapilandirmasi | VersaCoder.Domain | net48;net10.0-windows | ConversationAggregateTests | planli |
| N08-068 | `ConversationAggregateProvider` | class | ConversationAggregate saglayici uygulamasi | VersaCoder.Domain | net48;net10.0-windows | ConversationAggregateTests | planli |
| N08-069 | `ConversationAggregateRegistry` | class | ConversationAggregate kayit/katalog yonetimi | VersaCoder.Domain | net48;net10.0-windows | ConversationAggregateTests | planli |
| N08-070 | `ConversationAggregateConverter` | class | ConversationAggregate bicim donusumu | VersaCoder.Domain | net48;net10.0-windows | ConversationAggregateTests | planli |
| N08-071 | `ConversationAggregateTests` | class | ConversationAggregate testleri (xUnit) | VersaCoder.Domain | net48;net10.0-windows | - | planli |
| N08-072 | `ConversationAggregateBuilder` | class | ConversationAggregate uretici (builder) | VersaCoder.Domain | net48;net10.0-windows | ConversationAggregateTests | planli |
| N08-073 | `IWorkspaceAggregate` | interface | WorkspaceAggregate sozlesmesi (port) | VersaCoder.Domain | net48;net10.0-windows | WorkspaceAggregateTests | planli |
| N08-074 | `WorkspaceAggregate` | class | WorkspaceAggregate cekirdek uygulamasi | VersaCoder.Domain | net48;net10.0-windows | WorkspaceAggregateTests | planli |
| N08-075 | `WorkspaceAggregateDto` | record | WorkspaceAggregate veri transferi modeli | VersaCoder.Domain | net48;net10.0-windows | WorkspaceAggregateTests | planli |
| N08-076 | `WorkspaceAggregateMapper` | class | WorkspaceAggregate <-> DTO esleme | VersaCoder.Domain | net48;net10.0-windows | WorkspaceAggregateTests | planli |
| N08-077 | `WorkspaceAggregateValidator` | class | WorkspaceAggregate dogrulama kurallari | VersaCoder.Domain | net48;net10.0-windows | WorkspaceAggregateTests | planli |
| N08-078 | `WorkspaceAggregateFactory` | class | WorkspaceAggregate uretimi ve varsayilanlari | VersaCoder.Domain | net48;net10.0-windows | WorkspaceAggregateTests | planli |
| N08-079 | `WorkspaceAggregateOptions` | record | WorkspaceAggregate yapilandirmasi | VersaCoder.Domain | net48;net10.0-windows | WorkspaceAggregateTests | planli |
| N08-080 | `WorkspaceAggregateProvider` | class | WorkspaceAggregate saglayici uygulamasi | VersaCoder.Domain | net48;net10.0-windows | WorkspaceAggregateTests | planli |
| N08-081 | `WorkspaceAggregateRegistry` | class | WorkspaceAggregate kayit/katalog yonetimi | VersaCoder.Domain | net48;net10.0-windows | WorkspaceAggregateTests | planli |
| N08-082 | `WorkspaceAggregateConverter` | class | WorkspaceAggregate bicim donusumu | VersaCoder.Domain | net48;net10.0-windows | WorkspaceAggregateTests | planli |
| N08-083 | `WorkspaceAggregateTests` | class | WorkspaceAggregate testleri (xUnit) | VersaCoder.Domain | net48;net10.0-windows | - | planli |
| N08-084 | `WorkspaceAggregateBuilder` | class | WorkspaceAggregate uretici (builder) | VersaCoder.Domain | net48;net10.0-windows | WorkspaceAggregateTests | planli |
| N08-085 | `IToolchainAggregate` | interface | ToolchainAggregate sozlesmesi (port) | VersaCoder.Domain | net48;net10.0-windows | ToolchainAggregateTests | planli |
| N08-086 | `ToolchainAggregate` | class | ToolchainAggregate cekirdek uygulamasi | VersaCoder.Domain | net48;net10.0-windows | ToolchainAggregateTests | planli |
| N08-087 | `ToolchainAggregateDto` | record | ToolchainAggregate veri transferi modeli | VersaCoder.Domain | net48;net10.0-windows | ToolchainAggregateTests | planli |
| N08-088 | `ToolchainAggregateMapper` | class | ToolchainAggregate <-> DTO esleme | VersaCoder.Domain | net48;net10.0-windows | ToolchainAggregateTests | planli |
| N08-089 | `ToolchainAggregateValidator` | class | ToolchainAggregate dogrulama kurallari | VersaCoder.Domain | net48;net10.0-windows | ToolchainAggregateTests | planli |
| N08-090 | `ToolchainAggregateFactory` | class | ToolchainAggregate uretimi ve varsayilanlari | VersaCoder.Domain | net48;net10.0-windows | ToolchainAggregateTests | planli |
| N08-091 | `ToolchainAggregateOptions` | record | ToolchainAggregate yapilandirmasi | VersaCoder.Domain | net48;net10.0-windows | ToolchainAggregateTests | planli |
| N08-092 | `ToolchainAggregateProvider` | class | ToolchainAggregate saglayici uygulamasi | VersaCoder.Domain | net48;net10.0-windows | ToolchainAggregateTests | planli |
| N08-093 | `ToolchainAggregateRegistry` | class | ToolchainAggregate kayit/katalog yonetimi | VersaCoder.Domain | net48;net10.0-windows | ToolchainAggregateTests | planli |
| N08-094 | `ToolchainAggregateConverter` | class | ToolchainAggregate bicim donusumu | VersaCoder.Domain | net48;net10.0-windows | ToolchainAggregateTests | planli |
| N08-095 | `ToolchainAggregateTests` | class | ToolchainAggregate testleri (xUnit) | VersaCoder.Domain | net48;net10.0-windows | - | planli |
| N08-096 | `ToolchainAggregateBuilder` | class | ToolchainAggregate uretici (builder) | VersaCoder.Domain | net48;net10.0-windows | ToolchainAggregateTests | planli |
| N08-097 | `IPolicyAggregate` | interface | PolicyAggregate sozlesmesi (port) | VersaCoder.Domain | net48;net10.0-windows | PolicyAggregateTests | planli |
| N08-098 | `PolicyAggregate` | class | PolicyAggregate cekirdek uygulamasi | VersaCoder.Domain | net48;net10.0-windows | PolicyAggregateTests | planli |
| N08-099 | `PolicyAggregateDto` | record | PolicyAggregate veri transferi modeli | VersaCoder.Domain | net48;net10.0-windows | PolicyAggregateTests | planli |
| N08-100 | `PolicyAggregateMapper` | class | PolicyAggregate <-> DTO esleme | VersaCoder.Domain | net48;net10.0-windows | PolicyAggregateTests | planli |
| N08-101 | `PolicyAggregateValidator` | class | PolicyAggregate dogrulama kurallari | VersaCoder.Domain | net48;net10.0-windows | PolicyAggregateTests | planli |
| N08-102 | `PolicyAggregateFactory` | class | PolicyAggregate uretimi ve varsayilanlari | VersaCoder.Domain | net48;net10.0-windows | PolicyAggregateTests | planli |
| N08-103 | `PolicyAggregateOptions` | record | PolicyAggregate yapilandirmasi | VersaCoder.Domain | net48;net10.0-windows | PolicyAggregateTests | planli |
| N08-104 | `PolicyAggregateProvider` | class | PolicyAggregate saglayici uygulamasi | VersaCoder.Domain | net48;net10.0-windows | PolicyAggregateTests | planli |
| N08-105 | `PolicyAggregateRegistry` | class | PolicyAggregate kayit/katalog yonetimi | VersaCoder.Domain | net48;net10.0-windows | PolicyAggregateTests | planli |
| N08-106 | `PolicyAggregateConverter` | class | PolicyAggregate bicim donusumu | VersaCoder.Domain | net48;net10.0-windows | PolicyAggregateTests | planli |
| N08-107 | `PolicyAggregateTests` | class | PolicyAggregate testleri (xUnit) | VersaCoder.Domain | net48;net10.0-windows | - | planli |
| N08-108 | `PolicyAggregateBuilder` | class | PolicyAggregate uretici (builder) | VersaCoder.Domain | net48;net10.0-windows | PolicyAggregateTests | planli |
| N08-109 | `IBudgetAggregate` | interface | BudgetAggregate sozlesmesi (port) | VersaCoder.Domain | net48;net10.0-windows | BudgetAggregateTests | planli |
| N08-110 | `BudgetAggregate` | class | BudgetAggregate cekirdek uygulamasi | VersaCoder.Domain | net48;net10.0-windows | BudgetAggregateTests | planli |
| N08-111 | `BudgetAggregateDto` | record | BudgetAggregate veri transferi modeli | VersaCoder.Domain | net48;net10.0-windows | BudgetAggregateTests | planli |
| N08-112 | `BudgetAggregateMapper` | class | BudgetAggregate <-> DTO esleme | VersaCoder.Domain | net48;net10.0-windows | BudgetAggregateTests | planli |
| N08-113 | `BudgetAggregateValidator` | class | BudgetAggregate dogrulama kurallari | VersaCoder.Domain | net48;net10.0-windows | BudgetAggregateTests | planli |
| N08-114 | `BudgetAggregateFactory` | class | BudgetAggregate uretimi ve varsayilanlari | VersaCoder.Domain | net48;net10.0-windows | BudgetAggregateTests | planli |
| N08-115 | `BudgetAggregateOptions` | record | BudgetAggregate yapilandirmasi | VersaCoder.Domain | net48;net10.0-windows | BudgetAggregateTests | planli |
| N08-116 | `BudgetAggregateProvider` | class | BudgetAggregate saglayici uygulamasi | VersaCoder.Domain | net48;net10.0-windows | BudgetAggregateTests | planli |
| N08-117 | `BudgetAggregateRegistry` | class | BudgetAggregate kayit/katalog yonetimi | VersaCoder.Domain | net48;net10.0-windows | BudgetAggregateTests | planli |
| N08-118 | `BudgetAggregateConverter` | class | BudgetAggregate bicim donusumu | VersaCoder.Domain | net48;net10.0-windows | BudgetAggregateTests | planli |
| N08-119 | `BudgetAggregateTests` | class | BudgetAggregate testleri (xUnit) | VersaCoder.Domain | net48;net10.0-windows | - | planli |
| N08-120 | `BudgetAggregateBuilder` | class | BudgetAggregate uretici (builder) | VersaCoder.Domain | net48;net10.0-windows | BudgetAggregateTests | planli |
