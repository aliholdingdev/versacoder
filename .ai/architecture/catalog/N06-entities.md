---
title: "Katalog Dugumu N06 - Varliklar (l1)"
type: catalog
category: architecture
status: needs-validation
version: 1.0.0
updated: 2026-10-04
authority: derived
scope: "N06 entities dugumu: kimlik, src klasoru, aile yapisi, ornek kayit ve yon kurali (ADR-013 + ADR-028)"
---

# Katalog Dugumu N06 - Varliklar

**Zorunlu Baglantilar:** [catalog index](index.md) · [node-inventory](node-inventory.md) · [mimari index](../index.md) · [ADR-028](../../.decisions/accepted/ADR-028-layered-baseline-9-projects.md) · [ADR-013](../../.decisions/accepted/ADR-013-layered-architecture-52-nodes.md)

> **Tek cumlelik sorumluluk:** *N06 entities hangi projede ve klasorde yasar, ne yapar, kac kayit hedefler* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

**N06 (entities), l1 Domain katmaninin alt katmanidir: `VersaCoder.Domain` projesinde `src/VersaCoder.Domain/Entities/` klasorunde yasar.** Session, Message, Project gibi varlik tanimlarini tasir ve saf C# icinde kalir. 10 aile x 12 rol = 120 kayit hedefi (ADR-013 asgari 112); durum `planli`.

Kayitlar ADR-013 2.1 biciminde 8 sutun tasir; bu dosya aile kalibini ve ornek satirlari gosterir, tam 120 satirlik liste bu dosyanin boyut sinirina sigmaz (DOĞRULAMA GEREKLI).

```text
+--------------------------------------------------+
| l1 Domain -> VersaCoder.Domain
| src/VersaCoder.Domain/Entities/
| 10 aile x 12 rol = 120 kayit (esik 112)
+--------------------------------------------------+
```

| Alan | Deger |
|---|---|
| Dugum | N06 |
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
| 1 | Session, Message, Project gibi varlik tanimlarini tasir ve saf C# icinde kalir | System.Data, Dapper, dosya veya git cagirmak |
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
| 1 | N06 | entities | `src/VersaCoder.Domain/Entities/` |

**Kanit yolu (ADR-028 2.3/5):** tarama yolu gercek proje adini icerir; genis kalip (`src/**/Domain/**`) sahte PASS uretir.

```powershell
Test-Path 'C:\versacoder\src\VersaCoder.Domain\Entities'
```

Dizin su an yok cikti `False` olur; PASS yazilmaz -> **DOĞRULAMA GEREKLI**.

---

## §3 Aile Yapisi

Rol kalibi (12): `I<F>` · `<F>` · `<F>Dto` · `<F>Mapper` · `<F>Validator` · `<F>Factory` · `<F>Options` · `<F>Provider` · `<F>Registry` · `<F>Converter` · `<F>Tests` · `<F>Builder`

| # | Aile | # | Aile |
|---|---|---|---|
| 1 | Session | 6 | Workspace |
| 2 | Conversation | 7 | FileNode |
| 3 | Message | 8 | TaskItem |
| 4 | ToolCall | 9 | AgentProfile |
| 5 | Project | 10 | Permission |

Her aile ayni rol kalibini 12 kez tekrarlar; kayit sayisi = aile x rol.

---

## §4 Kayit Ornegi (8 sutun)

| ID | Bilesen | Tip | Sorumluluk | Proje | TFM | Test | Durum |
|---|---|---|---|---|---|---|---|
| N06-001 | `ISession` | interface | Session sozlesmesi (port) | VersaCoder.Domain | net48;net10.0-windows | SessionTests | planli |
| N06-002 | `Session` | class | Session cekirdek uygulamasi | VersaCoder.Domain | net48;net10.0-windows | SessionTests | planli |
| N06-003 | `SessionDto` | record | Session veri transferi modeli | VersaCoder.Domain | net48;net10.0-windows | SessionTests | planli |
| N06-004 | `SessionMapper` | class | Session <-> DTO esleme | VersaCoder.Domain | net48;net10.0-windows | SessionTests | planli |
| N06-005 | `SessionValidator` | class | Session dogrulama kurallari | VersaCoder.Domain | net48;net10.0-windows | SessionTests | planli |
| N06-006 | `SessionFactory` | class | Session uretimi ve varsayilanlari | VersaCoder.Domain | net48;net10.0-windows | SessionTests | planli |

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
| 3 | Test sinifi | SessionTests (xUnit) | planli |
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
(Select-String -Path 'C:\versacoder\.ai\architecture\catalog\N06-entities.md' -Pattern '^\|\s*N\d{2}-\d{3}\s*\|').Count
# 2) boyut bandi - beklenen: 2600-8500
(Get-Item 'C:\versacoder\.ai\architecture\catalog\N06-entities.md').Length
# 3) klasor kaniti - gercek proje adi (ADR-028 2.3/5)
(Test-Path 'C:\versacoder\src\VersaCoder.Domain\Entities')
```

Katalog sayimi `powershell -NoProfile -ExecutionPolicy Bypass -File C:\versacoder\.ai\scripts\count-components.ps1` ile alinir; bu batch sonunda toplam esik (5000) henuz karsilanmaz - kalan 26 node dosyasi sonraki batchtir.

## §10 Kayit Tablosu (ADR-013 2.2 - kaynak vault port)

| N06-007 | `SessionOptions` | record | Session yapilandirmasi | VersaCoder.Domain | net48;net10.0-windows | SessionTests | planli |
| N06-008 | `SessionProvider` | class | Session saglayici uygulamasi | VersaCoder.Domain | net48;net10.0-windows | SessionTests | planli |
| N06-009 | `SessionRegistry` | class | Session kayit/katalog yonetimi | VersaCoder.Domain | net48;net10.0-windows | SessionTests | planli |
| N06-010 | `SessionConverter` | class | Session bicim donusumu | VersaCoder.Domain | net48;net10.0-windows | SessionTests | planli |
| N06-011 | `SessionTests` | class | Session testleri (xUnit) | VersaCoder.Domain | net48;net10.0-windows | - | planli |
| N06-012 | `SessionBuilder` | class | Session uretici (builder) | VersaCoder.Domain | net48;net10.0-windows | SessionTests | planli |
| N06-013 | `IConversation` | interface | Conversation sozlesmesi (port) | VersaCoder.Domain | net48;net10.0-windows | ConversationTests | planli |
| N06-014 | `Conversation` | class | Conversation cekirdek uygulamasi | VersaCoder.Domain | net48;net10.0-windows | ConversationTests | planli |
| N06-015 | `ConversationDto` | record | Conversation veri transferi modeli | VersaCoder.Domain | net48;net10.0-windows | ConversationTests | planli |
| N06-016 | `ConversationMapper` | class | Conversation <-> DTO esleme | VersaCoder.Domain | net48;net10.0-windows | ConversationTests | planli |
| N06-017 | `ConversationValidator` | class | Conversation dogrulama kurallari | VersaCoder.Domain | net48;net10.0-windows | ConversationTests | planli |
| N06-018 | `ConversationFactory` | class | Conversation uretimi ve varsayilanlari | VersaCoder.Domain | net48;net10.0-windows | ConversationTests | planli |
| N06-019 | `ConversationOptions` | record | Conversation yapilandirmasi | VersaCoder.Domain | net48;net10.0-windows | ConversationTests | planli |
| N06-020 | `ConversationProvider` | class | Conversation saglayici uygulamasi | VersaCoder.Domain | net48;net10.0-windows | ConversationTests | planli |
| N06-021 | `ConversationRegistry` | class | Conversation kayit/katalog yonetimi | VersaCoder.Domain | net48;net10.0-windows | ConversationTests | planli |
| N06-022 | `ConversationConverter` | class | Conversation bicim donusumu | VersaCoder.Domain | net48;net10.0-windows | ConversationTests | planli |
| N06-023 | `ConversationTests` | class | Conversation testleri (xUnit) | VersaCoder.Domain | net48;net10.0-windows | - | planli |
| N06-024 | `ConversationBuilder` | class | Conversation uretici (builder) | VersaCoder.Domain | net48;net10.0-windows | ConversationTests | planli |
| N06-025 | `IMessage` | interface | Message sozlesmesi (port) | VersaCoder.Domain | net48;net10.0-windows | MessageTests | planli |
| N06-026 | `Message` | class | Message cekirdek uygulamasi | VersaCoder.Domain | net48;net10.0-windows | MessageTests | planli |
| N06-027 | `MessageDto` | record | Message veri transferi modeli | VersaCoder.Domain | net48;net10.0-windows | MessageTests | planli |
| N06-028 | `MessageMapper` | class | Message <-> DTO esleme | VersaCoder.Domain | net48;net10.0-windows | MessageTests | planli |
| N06-029 | `MessageValidator` | class | Message dogrulama kurallari | VersaCoder.Domain | net48;net10.0-windows | MessageTests | planli |
| N06-030 | `MessageFactory` | class | Message uretimi ve varsayilanlari | VersaCoder.Domain | net48;net10.0-windows | MessageTests | planli |
| N06-031 | `MessageOptions` | record | Message yapilandirmasi | VersaCoder.Domain | net48;net10.0-windows | MessageTests | planli |
| N06-032 | `MessageProvider` | class | Message saglayici uygulamasi | VersaCoder.Domain | net48;net10.0-windows | MessageTests | planli |
| N06-033 | `MessageRegistry` | class | Message kayit/katalog yonetimi | VersaCoder.Domain | net48;net10.0-windows | MessageTests | planli |
| N06-034 | `MessageConverter` | class | Message bicim donusumu | VersaCoder.Domain | net48;net10.0-windows | MessageTests | planli |
| N06-035 | `MessageTests` | class | Message testleri (xUnit) | VersaCoder.Domain | net48;net10.0-windows | - | planli |
| N06-036 | `MessageBuilder` | class | Message uretici (builder) | VersaCoder.Domain | net48;net10.0-windows | MessageTests | planli |
| N06-037 | `IToolCall` | interface | ToolCall sozlesmesi (port) | VersaCoder.Domain | net48;net10.0-windows | ToolCallTests | planli |
| N06-038 | `ToolCall` | class | ToolCall cekirdek uygulamasi | VersaCoder.Domain | net48;net10.0-windows | ToolCallTests | planli |
| N06-039 | `ToolCallDto` | record | ToolCall veri transferi modeli | VersaCoder.Domain | net48;net10.0-windows | ToolCallTests | planli |
| N06-040 | `ToolCallMapper` | class | ToolCall <-> DTO esleme | VersaCoder.Domain | net48;net10.0-windows | ToolCallTests | planli |
| N06-041 | `ToolCallValidator` | class | ToolCall dogrulama kurallari | VersaCoder.Domain | net48;net10.0-windows | ToolCallTests | planli |
| N06-042 | `ToolCallFactory` | class | ToolCall uretimi ve varsayilanlari | VersaCoder.Domain | net48;net10.0-windows | ToolCallTests | planli |
| N06-043 | `ToolCallOptions` | record | ToolCall yapilandirmasi | VersaCoder.Domain | net48;net10.0-windows | ToolCallTests | planli |
| N06-044 | `ToolCallProvider` | class | ToolCall saglayici uygulamasi | VersaCoder.Domain | net48;net10.0-windows | ToolCallTests | planli |
| N06-045 | `ToolCallRegistry` | class | ToolCall kayit/katalog yonetimi | VersaCoder.Domain | net48;net10.0-windows | ToolCallTests | planli |
| N06-046 | `ToolCallConverter` | class | ToolCall bicim donusumu | VersaCoder.Domain | net48;net10.0-windows | ToolCallTests | planli |
| N06-047 | `ToolCallTests` | class | ToolCall testleri (xUnit) | VersaCoder.Domain | net48;net10.0-windows | - | planli |
| N06-048 | `ToolCallBuilder` | class | ToolCall uretici (builder) | VersaCoder.Domain | net48;net10.0-windows | ToolCallTests | planli |
| N06-049 | `IProject` | interface | Project sozlesmesi (port) | VersaCoder.Domain | net48;net10.0-windows | ProjectTests | planli |
| N06-050 | `Project` | class | Project cekirdek uygulamasi | VersaCoder.Domain | net48;net10.0-windows | ProjectTests | planli |
| N06-051 | `ProjectDto` | record | Project veri transferi modeli | VersaCoder.Domain | net48;net10.0-windows | ProjectTests | planli |
| N06-052 | `ProjectMapper` | class | Project <-> DTO esleme | VersaCoder.Domain | net48;net10.0-windows | ProjectTests | planli |
| N06-053 | `ProjectValidator` | class | Project dogrulama kurallari | VersaCoder.Domain | net48;net10.0-windows | ProjectTests | planli |
| N06-054 | `ProjectFactory` | class | Project uretimi ve varsayilanlari | VersaCoder.Domain | net48;net10.0-windows | ProjectTests | planli |
| N06-055 | `ProjectOptions` | record | Project yapilandirmasi | VersaCoder.Domain | net48;net10.0-windows | ProjectTests | planli |
| N06-056 | `ProjectProvider` | class | Project saglayici uygulamasi | VersaCoder.Domain | net48;net10.0-windows | ProjectTests | planli |
| N06-057 | `ProjectRegistry` | class | Project kayit/katalog yonetimi | VersaCoder.Domain | net48;net10.0-windows | ProjectTests | planli |
| N06-058 | `ProjectConverter` | class | Project bicim donusumu | VersaCoder.Domain | net48;net10.0-windows | ProjectTests | planli |
| N06-059 | `ProjectTests` | class | Project testleri (xUnit) | VersaCoder.Domain | net48;net10.0-windows | - | planli |
| N06-060 | `ProjectBuilder` | class | Project uretici (builder) | VersaCoder.Domain | net48;net10.0-windows | ProjectTests | planli |
| N06-061 | `IWorkspace` | interface | Workspace sozlesmesi (port) | VersaCoder.Domain | net48;net10.0-windows | WorkspaceTests | planli |
| N06-062 | `Workspace` | class | Workspace cekirdek uygulamasi | VersaCoder.Domain | net48;net10.0-windows | WorkspaceTests | planli |
| N06-063 | `WorkspaceDto` | record | Workspace veri transferi modeli | VersaCoder.Domain | net48;net10.0-windows | WorkspaceTests | planli |
| N06-064 | `WorkspaceMapper` | class | Workspace <-> DTO esleme | VersaCoder.Domain | net48;net10.0-windows | WorkspaceTests | planli |
| N06-065 | `WorkspaceValidator` | class | Workspace dogrulama kurallari | VersaCoder.Domain | net48;net10.0-windows | WorkspaceTests | planli |
| N06-066 | `WorkspaceFactory` | class | Workspace uretimi ve varsayilanlari | VersaCoder.Domain | net48;net10.0-windows | WorkspaceTests | planli |
| N06-067 | `WorkspaceOptions` | record | Workspace yapilandirmasi | VersaCoder.Domain | net48;net10.0-windows | WorkspaceTests | planli |
| N06-068 | `WorkspaceProvider` | class | Workspace saglayici uygulamasi | VersaCoder.Domain | net48;net10.0-windows | WorkspaceTests | planli |
| N06-069 | `WorkspaceRegistry` | class | Workspace kayit/katalog yonetimi | VersaCoder.Domain | net48;net10.0-windows | WorkspaceTests | planli |
| N06-070 | `WorkspaceConverter` | class | Workspace bicim donusumu | VersaCoder.Domain | net48;net10.0-windows | WorkspaceTests | planli |
| N06-071 | `WorkspaceTests` | class | Workspace testleri (xUnit) | VersaCoder.Domain | net48;net10.0-windows | - | planli |
| N06-072 | `WorkspaceBuilder` | class | Workspace uretici (builder) | VersaCoder.Domain | net48;net10.0-windows | WorkspaceTests | planli |
| N06-073 | `IFileNode` | interface | FileNode sozlesmesi (port) | VersaCoder.Domain | net48;net10.0-windows | FileNodeTests | planli |
| N06-074 | `FileNode` | class | FileNode cekirdek uygulamasi | VersaCoder.Domain | net48;net10.0-windows | FileNodeTests | planli |
| N06-075 | `FileNodeDto` | record | FileNode veri transferi modeli | VersaCoder.Domain | net48;net10.0-windows | FileNodeTests | planli |
| N06-076 | `FileNodeMapper` | class | FileNode <-> DTO esleme | VersaCoder.Domain | net48;net10.0-windows | FileNodeTests | planli |
| N06-077 | `FileNodeValidator` | class | FileNode dogrulama kurallari | VersaCoder.Domain | net48;net10.0-windows | FileNodeTests | planli |
| N06-078 | `FileNodeFactory` | class | FileNode uretimi ve varsayilanlari | VersaCoder.Domain | net48;net10.0-windows | FileNodeTests | planli |
| N06-079 | `FileNodeOptions` | record | FileNode yapilandirmasi | VersaCoder.Domain | net48;net10.0-windows | FileNodeTests | planli |
| N06-080 | `FileNodeProvider` | class | FileNode saglayici uygulamasi | VersaCoder.Domain | net48;net10.0-windows | FileNodeTests | planli |
| N06-081 | `FileNodeRegistry` | class | FileNode kayit/katalog yonetimi | VersaCoder.Domain | net48;net10.0-windows | FileNodeTests | planli |
| N06-082 | `FileNodeConverter` | class | FileNode bicim donusumu | VersaCoder.Domain | net48;net10.0-windows | FileNodeTests | planli |
| N06-083 | `FileNodeTests` | class | FileNode testleri (xUnit) | VersaCoder.Domain | net48;net10.0-windows | - | planli |
| N06-084 | `FileNodeBuilder` | class | FileNode uretici (builder) | VersaCoder.Domain | net48;net10.0-windows | FileNodeTests | planli |
| N06-085 | `ITaskItem` | interface | TaskItem sozlesmesi (port) | VersaCoder.Domain | net48;net10.0-windows | TaskItemTests | planli |
| N06-086 | `TaskItem` | class | TaskItem cekirdek uygulamasi | VersaCoder.Domain | net48;net10.0-windows | TaskItemTests | planli |
| N06-087 | `TaskItemDto` | record | TaskItem veri transferi modeli | VersaCoder.Domain | net48;net10.0-windows | TaskItemTests | planli |
| N06-088 | `TaskItemMapper` | class | TaskItem <-> DTO esleme | VersaCoder.Domain | net48;net10.0-windows | TaskItemTests | planli |
| N06-089 | `TaskItemValidator` | class | TaskItem dogrulama kurallari | VersaCoder.Domain | net48;net10.0-windows | TaskItemTests | planli |
| N06-090 | `TaskItemFactory` | class | TaskItem uretimi ve varsayilanlari | VersaCoder.Domain | net48;net10.0-windows | TaskItemTests | planli |
| N06-091 | `TaskItemOptions` | record | TaskItem yapilandirmasi | VersaCoder.Domain | net48;net10.0-windows | TaskItemTests | planli |
| N06-092 | `TaskItemProvider` | class | TaskItem saglayici uygulamasi | VersaCoder.Domain | net48;net10.0-windows | TaskItemTests | planli |
| N06-093 | `TaskItemRegistry` | class | TaskItem kayit/katalog yonetimi | VersaCoder.Domain | net48;net10.0-windows | TaskItemTests | planli |
| N06-094 | `TaskItemConverter` | class | TaskItem bicim donusumu | VersaCoder.Domain | net48;net10.0-windows | TaskItemTests | planli |
| N06-095 | `TaskItemTests` | class | TaskItem testleri (xUnit) | VersaCoder.Domain | net48;net10.0-windows | - | planli |
| N06-096 | `TaskItemBuilder` | class | TaskItem uretici (builder) | VersaCoder.Domain | net48;net10.0-windows | TaskItemTests | planli |
| N06-097 | `IAgentProfile` | interface | AgentProfile sozlesmesi (port) | VersaCoder.Domain | net48;net10.0-windows | AgentProfileTests | planli |
| N06-098 | `AgentProfile` | class | AgentProfile cekirdek uygulamasi | VersaCoder.Domain | net48;net10.0-windows | AgentProfileTests | planli |
| N06-099 | `AgentProfileDto` | record | AgentProfile veri transferi modeli | VersaCoder.Domain | net48;net10.0-windows | AgentProfileTests | planli |
| N06-100 | `AgentProfileMapper` | class | AgentProfile <-> DTO esleme | VersaCoder.Domain | net48;net10.0-windows | AgentProfileTests | planli |
| N06-101 | `AgentProfileValidator` | class | AgentProfile dogrulama kurallari | VersaCoder.Domain | net48;net10.0-windows | AgentProfileTests | planli |
| N06-102 | `AgentProfileFactory` | class | AgentProfile uretimi ve varsayilanlari | VersaCoder.Domain | net48;net10.0-windows | AgentProfileTests | planli |
| N06-103 | `AgentProfileOptions` | record | AgentProfile yapilandirmasi | VersaCoder.Domain | net48;net10.0-windows | AgentProfileTests | planli |
| N06-104 | `AgentProfileProvider` | class | AgentProfile saglayici uygulamasi | VersaCoder.Domain | net48;net10.0-windows | AgentProfileTests | planli |
| N06-105 | `AgentProfileRegistry` | class | AgentProfile kayit/katalog yonetimi | VersaCoder.Domain | net48;net10.0-windows | AgentProfileTests | planli |
| N06-106 | `AgentProfileConverter` | class | AgentProfile bicim donusumu | VersaCoder.Domain | net48;net10.0-windows | AgentProfileTests | planli |
| N06-107 | `AgentProfileTests` | class | AgentProfile testleri (xUnit) | VersaCoder.Domain | net48;net10.0-windows | - | planli |
| N06-108 | `AgentProfileBuilder` | class | AgentProfile uretici (builder) | VersaCoder.Domain | net48;net10.0-windows | AgentProfileTests | planli |
| N06-109 | `IPermission` | interface | Permission sozlesmesi (port) | VersaCoder.Domain | net48;net10.0-windows | PermissionTests | planli |
| N06-110 | `Permission` | class | Permission cekirdek uygulamasi | VersaCoder.Domain | net48;net10.0-windows | PermissionTests | planli |
| N06-111 | `PermissionDto` | record | Permission veri transferi modeli | VersaCoder.Domain | net48;net10.0-windows | PermissionTests | planli |
| N06-112 | `PermissionMapper` | class | Permission <-> DTO esleme | VersaCoder.Domain | net48;net10.0-windows | PermissionTests | planli |
| N06-113 | `PermissionValidator` | class | Permission dogrulama kurallari | VersaCoder.Domain | net48;net10.0-windows | PermissionTests | planli |
| N06-114 | `PermissionFactory` | class | Permission uretimi ve varsayilanlari | VersaCoder.Domain | net48;net10.0-windows | PermissionTests | planli |
| N06-115 | `PermissionOptions` | record | Permission yapilandirmasi | VersaCoder.Domain | net48;net10.0-windows | PermissionTests | planli |
| N06-116 | `PermissionProvider` | class | Permission saglayici uygulamasi | VersaCoder.Domain | net48;net10.0-windows | PermissionTests | planli |
| N06-117 | `PermissionRegistry` | class | Permission kayit/katalog yonetimi | VersaCoder.Domain | net48;net10.0-windows | PermissionTests | planli |
| N06-118 | `PermissionConverter` | class | Permission bicim donusumu | VersaCoder.Domain | net48;net10.0-windows | PermissionTests | planli |
| N06-119 | `PermissionTests` | class | Permission testleri (xUnit) | VersaCoder.Domain | net48;net10.0-windows | - | planli |
| N06-120 | `PermissionBuilder` | class | Permission uretici (builder) | VersaCoder.Domain | net48;net10.0-windows | PermissionTests | planli |
