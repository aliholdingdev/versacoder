---
title: "Katalog Dugumu N09 - Domain Olaylari (l1)"
type: catalog
category: architecture
status: needs-validation
version: 1.0.0
updated: 2026-10-04
authority: derived
scope: "N09 domain-events dugumu: kimlik, src klasoru, aile yapisi, ornek kayit ve yon kurali (ADR-013 + ADR-028)"
---

# Katalog Dugumu N09 - Domain Olaylari

**Zorunlu Baglantilar:** [catalog index](index.md) · [node-inventory](node-inventory.md) · [mimari index](../index.md) · [ADR-028](../../.decisions/accepted/ADR-028-layered-baseline-9-projects.md) · [ADR-013](../../.decisions/accepted/ADR-013-layered-architecture-52-nodes.md)

> **Tek cumlelik sorumluluk:** *N09 domain-events hangi projede ve klasorde yasar, ne yapar, kac kayit hedefler* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

**N09 (domain-events), l1 Domain katmaninin alt katmanidir: `VersaCoder.Domain` projesinde `src/VersaCoder.Domain/Events/` klasorunde yasar.** SessionStarted, BudgetExceeded gibi domain olaylarini tanimlar. 10 aile x 12 rol = 120 kayit hedefi (ADR-013 asgari 112); durum `planli`.

Kayitlar ADR-013 2.1 biciminde 8 sutun tasir; bu dosya aile kalibini ve ornek satirlari gosterir, tam 120 satirlik liste bu dosyanin boyut sinirina sigmaz (DOĞRULAMA GEREKLI).

```text
+--------------------------------------------------+
| l1 Domain -> VersaCoder.Domain
| src/VersaCoder.Domain/Events/
| 10 aile x 12 rol = 120 kayit (esik 112)
+--------------------------------------------------+
```

| Alan | Deger |
|---|---|
| Dugum | N09 |
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
| 1 | SessionStarted, BudgetExceeded gibi domain olaylarini tanimlar | System.Data, Dapper, dosya veya git cagirmak |
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
| 1 | N09 | domain-events | `src/VersaCoder.Domain/Events/` |

**Kanit yolu (ADR-028 2.3/5):** tarama yolu gercek proje adini icerir; genis kalip (`src/**/Domain/**`) sahte PASS uretir.

```powershell
Test-Path 'C:\versacoder\src\VersaCoder.Domain\Events'
```

Dizin su an yok cikti `False` olur; PASS yazilmaz -> **DOĞRULAMA GEREKLI**.

---

## §3 Aile Yapisi

Rol kalibi (12): `I<F>` · `<F>` · `<F>Dto` · `<F>Mapper` · `<F>Validator` · `<F>Factory` · `<F>Options` · `<F>Provider` · `<F>Registry` · `<F>Converter` · `<F>Tests` · `<F>Builder`

| # | Aile | # | Aile |
|---|---|---|---|
| 1 | SessionStarted | 6 | ConfigChanged |
| 2 | SessionEnded | 7 | ErrorRaised |
| 3 | MessageAdded | 8 | AgentSpawned |
| 4 | ToolExecuted | 9 | BudgetExceeded |
| 5 | TaskCompleted | 10 | PlanUpdated |

Her aile ayni rol kalibini 12 kez tekrarlar; kayit sayisi = aile x rol.

---

## §4 Kayit Ornegi (8 sutun)

| ID | Bilesen | Tip | Sorumluluk | Proje | TFM | Test | Durum |
|---|---|---|---|---|---|---|---|
| N09-001 | `ISessionStarted` | interface | SessionStarted sozlesmesi (port) | VersaCoder.Domain | net48;net10.0-windows | SessionStartedTests | planli |
| N09-002 | `SessionStarted` | class | SessionStarted cekirdek uygulamasi | VersaCoder.Domain | net48;net10.0-windows | SessionStartedTests | planli |
| N09-003 | `SessionStartedDto` | record | SessionStarted veri transferi modeli | VersaCoder.Domain | net48;net10.0-windows | SessionStartedTests | planli |
| N09-004 | `SessionStartedMapper` | class | SessionStarted <-> DTO esleme | VersaCoder.Domain | net48;net10.0-windows | SessionStartedTests | planli |
| N09-005 | `SessionStartedValidator` | class | SessionStarted dogrulama kurallari | VersaCoder.Domain | net48;net10.0-windows | SessionStartedTests | planli |
| N09-006 | `SessionStartedFactory` | class | SessionStarted uretimi ve varsayilanlari | VersaCoder.Domain | net48;net10.0-windows | SessionStartedTests | planli |

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
| 3 | Test sinifi | SessionStartedTests (xUnit) | planli |
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
(Select-String -Path 'C:\versacoder\.ai\architecture\catalog\N09-domain-events.md' -Pattern '^\|\s*N\d{2}-\d{3}\s*\|').Count
# 2) boyut bandi - beklenen: 2600-8500
(Get-Item 'C:\versacoder\.ai\architecture\catalog\N09-domain-events.md').Length
# 3) klasor kaniti - gercek proje adi (ADR-028 2.3/5)
(Test-Path 'C:\versacoder\src\VersaCoder.Domain\Events')
```

Katalog sayimi `powershell -NoProfile -ExecutionPolicy Bypass -File C:\versacoder\.ai\scripts\count-components.ps1` ile alinir; bu batch sonunda toplam esik (5000) henuz karsilanmaz - kalan 26 node dosyasi sonraki batchtir.

## §10 Kayit Tablosu (ADR-013 2.2 - kaynak vault port)

| N09-007 | `SessionStartedOptions` | record | SessionStarted yapilandirmasi | VersaCoder.Domain | net48;net10.0-windows | SessionStartedTests | planli |
| N09-008 | `SessionStartedProvider` | class | SessionStarted saglayici uygulamasi | VersaCoder.Domain | net48;net10.0-windows | SessionStartedTests | planli |
| N09-009 | `SessionStartedRegistry` | class | SessionStarted kayit/katalog yonetimi | VersaCoder.Domain | net48;net10.0-windows | SessionStartedTests | planli |
| N09-010 | `SessionStartedConverter` | class | SessionStarted bicim donusumu | VersaCoder.Domain | net48;net10.0-windows | SessionStartedTests | planli |
| N09-011 | `SessionStartedTests` | class | SessionStarted testleri (xUnit) | VersaCoder.Domain | net48;net10.0-windows | - | planli |
| N09-012 | `SessionStartedBuilder` | class | SessionStarted uretici (builder) | VersaCoder.Domain | net48;net10.0-windows | SessionStartedTests | planli |
| N09-013 | `ISessionEnded` | interface | SessionEnded sozlesmesi (port) | VersaCoder.Domain | net48;net10.0-windows | SessionEndedTests | planli |
| N09-014 | `SessionEnded` | class | SessionEnded cekirdek uygulamasi | VersaCoder.Domain | net48;net10.0-windows | SessionEndedTests | planli |
| N09-015 | `SessionEndedDto` | record | SessionEnded veri transferi modeli | VersaCoder.Domain | net48;net10.0-windows | SessionEndedTests | planli |
| N09-016 | `SessionEndedMapper` | class | SessionEnded <-> DTO esleme | VersaCoder.Domain | net48;net10.0-windows | SessionEndedTests | planli |
| N09-017 | `SessionEndedValidator` | class | SessionEnded dogrulama kurallari | VersaCoder.Domain | net48;net10.0-windows | SessionEndedTests | planli |
| N09-018 | `SessionEndedFactory` | class | SessionEnded uretimi ve varsayilanlari | VersaCoder.Domain | net48;net10.0-windows | SessionEndedTests | planli |
| N09-019 | `SessionEndedOptions` | record | SessionEnded yapilandirmasi | VersaCoder.Domain | net48;net10.0-windows | SessionEndedTests | planli |
| N09-020 | `SessionEndedProvider` | class | SessionEnded saglayici uygulamasi | VersaCoder.Domain | net48;net10.0-windows | SessionEndedTests | planli |
| N09-021 | `SessionEndedRegistry` | class | SessionEnded kayit/katalog yonetimi | VersaCoder.Domain | net48;net10.0-windows | SessionEndedTests | planli |
| N09-022 | `SessionEndedConverter` | class | SessionEnded bicim donusumu | VersaCoder.Domain | net48;net10.0-windows | SessionEndedTests | planli |
| N09-023 | `SessionEndedTests` | class | SessionEnded testleri (xUnit) | VersaCoder.Domain | net48;net10.0-windows | - | planli |
| N09-024 | `SessionEndedBuilder` | class | SessionEnded uretici (builder) | VersaCoder.Domain | net48;net10.0-windows | SessionEndedTests | planli |
| N09-025 | `IMessageAdded` | interface | MessageAdded sozlesmesi (port) | VersaCoder.Domain | net48;net10.0-windows | MessageAddedTests | planli |
| N09-026 | `MessageAdded` | class | MessageAdded cekirdek uygulamasi | VersaCoder.Domain | net48;net10.0-windows | MessageAddedTests | planli |
| N09-027 | `MessageAddedDto` | record | MessageAdded veri transferi modeli | VersaCoder.Domain | net48;net10.0-windows | MessageAddedTests | planli |
| N09-028 | `MessageAddedMapper` | class | MessageAdded <-> DTO esleme | VersaCoder.Domain | net48;net10.0-windows | MessageAddedTests | planli |
| N09-029 | `MessageAddedValidator` | class | MessageAdded dogrulama kurallari | VersaCoder.Domain | net48;net10.0-windows | MessageAddedTests | planli |
| N09-030 | `MessageAddedFactory` | class | MessageAdded uretimi ve varsayilanlari | VersaCoder.Domain | net48;net10.0-windows | MessageAddedTests | planli |
| N09-031 | `MessageAddedOptions` | record | MessageAdded yapilandirmasi | VersaCoder.Domain | net48;net10.0-windows | MessageAddedTests | planli |
| N09-032 | `MessageAddedProvider` | class | MessageAdded saglayici uygulamasi | VersaCoder.Domain | net48;net10.0-windows | MessageAddedTests | planli |
| N09-033 | `MessageAddedRegistry` | class | MessageAdded kayit/katalog yonetimi | VersaCoder.Domain | net48;net10.0-windows | MessageAddedTests | planli |
| N09-034 | `MessageAddedConverter` | class | MessageAdded bicim donusumu | VersaCoder.Domain | net48;net10.0-windows | MessageAddedTests | planli |
| N09-035 | `MessageAddedTests` | class | MessageAdded testleri (xUnit) | VersaCoder.Domain | net48;net10.0-windows | - | planli |
| N09-036 | `MessageAddedBuilder` | class | MessageAdded uretici (builder) | VersaCoder.Domain | net48;net10.0-windows | MessageAddedTests | planli |
| N09-037 | `IToolExecuted` | interface | ToolExecuted sozlesmesi (port) | VersaCoder.Domain | net48;net10.0-windows | ToolExecutedTests | planli |
| N09-038 | `ToolExecuted` | class | ToolExecuted cekirdek uygulamasi | VersaCoder.Domain | net48;net10.0-windows | ToolExecutedTests | planli |
| N09-039 | `ToolExecutedDto` | record | ToolExecuted veri transferi modeli | VersaCoder.Domain | net48;net10.0-windows | ToolExecutedTests | planli |
| N09-040 | `ToolExecutedMapper` | class | ToolExecuted <-> DTO esleme | VersaCoder.Domain | net48;net10.0-windows | ToolExecutedTests | planli |
| N09-041 | `ToolExecutedValidator` | class | ToolExecuted dogrulama kurallari | VersaCoder.Domain | net48;net10.0-windows | ToolExecutedTests | planli |
| N09-042 | `ToolExecutedFactory` | class | ToolExecuted uretimi ve varsayilanlari | VersaCoder.Domain | net48;net10.0-windows | ToolExecutedTests | planli |
| N09-043 | `ToolExecutedOptions` | record | ToolExecuted yapilandirmasi | VersaCoder.Domain | net48;net10.0-windows | ToolExecutedTests | planli |
| N09-044 | `ToolExecutedProvider` | class | ToolExecuted saglayici uygulamasi | VersaCoder.Domain | net48;net10.0-windows | ToolExecutedTests | planli |
| N09-045 | `ToolExecutedRegistry` | class | ToolExecuted kayit/katalog yonetimi | VersaCoder.Domain | net48;net10.0-windows | ToolExecutedTests | planli |
| N09-046 | `ToolExecutedConverter` | class | ToolExecuted bicim donusumu | VersaCoder.Domain | net48;net10.0-windows | ToolExecutedTests | planli |
| N09-047 | `ToolExecutedTests` | class | ToolExecuted testleri (xUnit) | VersaCoder.Domain | net48;net10.0-windows | - | planli |
| N09-048 | `ToolExecutedBuilder` | class | ToolExecuted uretici (builder) | VersaCoder.Domain | net48;net10.0-windows | ToolExecutedTests | planli |
| N09-049 | `ITaskCompleted` | interface | TaskCompleted sozlesmesi (port) | VersaCoder.Domain | net48;net10.0-windows | TaskCompletedTests | planli |
| N09-050 | `TaskCompleted` | class | TaskCompleted cekirdek uygulamasi | VersaCoder.Domain | net48;net10.0-windows | TaskCompletedTests | planli |
| N09-051 | `TaskCompletedDto` | record | TaskCompleted veri transferi modeli | VersaCoder.Domain | net48;net10.0-windows | TaskCompletedTests | planli |
| N09-052 | `TaskCompletedMapper` | class | TaskCompleted <-> DTO esleme | VersaCoder.Domain | net48;net10.0-windows | TaskCompletedTests | planli |
| N09-053 | `TaskCompletedValidator` | class | TaskCompleted dogrulama kurallari | VersaCoder.Domain | net48;net10.0-windows | TaskCompletedTests | planli |
| N09-054 | `TaskCompletedFactory` | class | TaskCompleted uretimi ve varsayilanlari | VersaCoder.Domain | net48;net10.0-windows | TaskCompletedTests | planli |
| N09-055 | `TaskCompletedOptions` | record | TaskCompleted yapilandirmasi | VersaCoder.Domain | net48;net10.0-windows | TaskCompletedTests | planli |
| N09-056 | `TaskCompletedProvider` | class | TaskCompleted saglayici uygulamasi | VersaCoder.Domain | net48;net10.0-windows | TaskCompletedTests | planli |
| N09-057 | `TaskCompletedRegistry` | class | TaskCompleted kayit/katalog yonetimi | VersaCoder.Domain | net48;net10.0-windows | TaskCompletedTests | planli |
| N09-058 | `TaskCompletedConverter` | class | TaskCompleted bicim donusumu | VersaCoder.Domain | net48;net10.0-windows | TaskCompletedTests | planli |
| N09-059 | `TaskCompletedTests` | class | TaskCompleted testleri (xUnit) | VersaCoder.Domain | net48;net10.0-windows | - | planli |
| N09-060 | `TaskCompletedBuilder` | class | TaskCompleted uretici (builder) | VersaCoder.Domain | net48;net10.0-windows | TaskCompletedTests | planli |
| N09-061 | `IConfigChanged` | interface | ConfigChanged sozlesmesi (port) | VersaCoder.Domain | net48;net10.0-windows | ConfigChangedTests | planli |
| N09-062 | `ConfigChanged` | class | ConfigChanged cekirdek uygulamasi | VersaCoder.Domain | net48;net10.0-windows | ConfigChangedTests | planli |
| N09-063 | `ConfigChangedDto` | record | ConfigChanged veri transferi modeli | VersaCoder.Domain | net48;net10.0-windows | ConfigChangedTests | planli |
| N09-064 | `ConfigChangedMapper` | class | ConfigChanged <-> DTO esleme | VersaCoder.Domain | net48;net10.0-windows | ConfigChangedTests | planli |
| N09-065 | `ConfigChangedValidator` | class | ConfigChanged dogrulama kurallari | VersaCoder.Domain | net48;net10.0-windows | ConfigChangedTests | planli |
| N09-066 | `ConfigChangedFactory` | class | ConfigChanged uretimi ve varsayilanlari | VersaCoder.Domain | net48;net10.0-windows | ConfigChangedTests | planli |
| N09-067 | `ConfigChangedOptions` | record | ConfigChanged yapilandirmasi | VersaCoder.Domain | net48;net10.0-windows | ConfigChangedTests | planli |
| N09-068 | `ConfigChangedProvider` | class | ConfigChanged saglayici uygulamasi | VersaCoder.Domain | net48;net10.0-windows | ConfigChangedTests | planli |
| N09-069 | `ConfigChangedRegistry` | class | ConfigChanged kayit/katalog yonetimi | VersaCoder.Domain | net48;net10.0-windows | ConfigChangedTests | planli |
| N09-070 | `ConfigChangedConverter` | class | ConfigChanged bicim donusumu | VersaCoder.Domain | net48;net10.0-windows | ConfigChangedTests | planli |
| N09-071 | `ConfigChangedTests` | class | ConfigChanged testleri (xUnit) | VersaCoder.Domain | net48;net10.0-windows | - | planli |
| N09-072 | `ConfigChangedBuilder` | class | ConfigChanged uretici (builder) | VersaCoder.Domain | net48;net10.0-windows | ConfigChangedTests | planli |
| N09-073 | `IErrorRaised` | interface | ErrorRaised sozlesmesi (port) | VersaCoder.Domain | net48;net10.0-windows | ErrorRaisedTests | planli |
| N09-074 | `ErrorRaised` | class | ErrorRaised cekirdek uygulamasi | VersaCoder.Domain | net48;net10.0-windows | ErrorRaisedTests | planli |
| N09-075 | `ErrorRaisedDto` | record | ErrorRaised veri transferi modeli | VersaCoder.Domain | net48;net10.0-windows | ErrorRaisedTests | planli |
| N09-076 | `ErrorRaisedMapper` | class | ErrorRaised <-> DTO esleme | VersaCoder.Domain | net48;net10.0-windows | ErrorRaisedTests | planli |
| N09-077 | `ErrorRaisedValidator` | class | ErrorRaised dogrulama kurallari | VersaCoder.Domain | net48;net10.0-windows | ErrorRaisedTests | planli |
| N09-078 | `ErrorRaisedFactory` | class | ErrorRaised uretimi ve varsayilanlari | VersaCoder.Domain | net48;net10.0-windows | ErrorRaisedTests | planli |
| N09-079 | `ErrorRaisedOptions` | record | ErrorRaised yapilandirmasi | VersaCoder.Domain | net48;net10.0-windows | ErrorRaisedTests | planli |
| N09-080 | `ErrorRaisedProvider` | class | ErrorRaised saglayici uygulamasi | VersaCoder.Domain | net48;net10.0-windows | ErrorRaisedTests | planli |
| N09-081 | `ErrorRaisedRegistry` | class | ErrorRaised kayit/katalog yonetimi | VersaCoder.Domain | net48;net10.0-windows | ErrorRaisedTests | planli |
| N09-082 | `ErrorRaisedConverter` | class | ErrorRaised bicim donusumu | VersaCoder.Domain | net48;net10.0-windows | ErrorRaisedTests | planli |
| N09-083 | `ErrorRaisedTests` | class | ErrorRaised testleri (xUnit) | VersaCoder.Domain | net48;net10.0-windows | - | planli |
| N09-084 | `ErrorRaisedBuilder` | class | ErrorRaised uretici (builder) | VersaCoder.Domain | net48;net10.0-windows | ErrorRaisedTests | planli |
| N09-085 | `IAgentSpawned` | interface | AgentSpawned sozlesmesi (port) | VersaCoder.Domain | net48;net10.0-windows | AgentSpawnedTests | planli |
| N09-086 | `AgentSpawned` | class | AgentSpawned cekirdek uygulamasi | VersaCoder.Domain | net48;net10.0-windows | AgentSpawnedTests | planli |
| N09-087 | `AgentSpawnedDto` | record | AgentSpawned veri transferi modeli | VersaCoder.Domain | net48;net10.0-windows | AgentSpawnedTests | planli |
| N09-088 | `AgentSpawnedMapper` | class | AgentSpawned <-> DTO esleme | VersaCoder.Domain | net48;net10.0-windows | AgentSpawnedTests | planli |
| N09-089 | `AgentSpawnedValidator` | class | AgentSpawned dogrulama kurallari | VersaCoder.Domain | net48;net10.0-windows | AgentSpawnedTests | planli |
| N09-090 | `AgentSpawnedFactory` | class | AgentSpawned uretimi ve varsayilanlari | VersaCoder.Domain | net48;net10.0-windows | AgentSpawnedTests | planli |
| N09-091 | `AgentSpawnedOptions` | record | AgentSpawned yapilandirmasi | VersaCoder.Domain | net48;net10.0-windows | AgentSpawnedTests | planli |
| N09-092 | `AgentSpawnedProvider` | class | AgentSpawned saglayici uygulamasi | VersaCoder.Domain | net48;net10.0-windows | AgentSpawnedTests | planli |
| N09-093 | `AgentSpawnedRegistry` | class | AgentSpawned kayit/katalog yonetimi | VersaCoder.Domain | net48;net10.0-windows | AgentSpawnedTests | planli |
| N09-094 | `AgentSpawnedConverter` | class | AgentSpawned bicim donusumu | VersaCoder.Domain | net48;net10.0-windows | AgentSpawnedTests | planli |
| N09-095 | `AgentSpawnedTests` | class | AgentSpawned testleri (xUnit) | VersaCoder.Domain | net48;net10.0-windows | - | planli |
| N09-096 | `AgentSpawnedBuilder` | class | AgentSpawned uretici (builder) | VersaCoder.Domain | net48;net10.0-windows | AgentSpawnedTests | planli |
| N09-097 | `IBudgetExceeded` | interface | BudgetExceeded sozlesmesi (port) | VersaCoder.Domain | net48;net10.0-windows | BudgetExceededTests | planli |
| N09-098 | `BudgetExceeded` | class | BudgetExceeded cekirdek uygulamasi | VersaCoder.Domain | net48;net10.0-windows | BudgetExceededTests | planli |
| N09-099 | `BudgetExceededDto` | record | BudgetExceeded veri transferi modeli | VersaCoder.Domain | net48;net10.0-windows | BudgetExceededTests | planli |
| N09-100 | `BudgetExceededMapper` | class | BudgetExceeded <-> DTO esleme | VersaCoder.Domain | net48;net10.0-windows | BudgetExceededTests | planli |
| N09-101 | `BudgetExceededValidator` | class | BudgetExceeded dogrulama kurallari | VersaCoder.Domain | net48;net10.0-windows | BudgetExceededTests | planli |
| N09-102 | `BudgetExceededFactory` | class | BudgetExceeded uretimi ve varsayilanlari | VersaCoder.Domain | net48;net10.0-windows | BudgetExceededTests | planli |
| N09-103 | `BudgetExceededOptions` | record | BudgetExceeded yapilandirmasi | VersaCoder.Domain | net48;net10.0-windows | BudgetExceededTests | planli |
| N09-104 | `BudgetExceededProvider` | class | BudgetExceeded saglayici uygulamasi | VersaCoder.Domain | net48;net10.0-windows | BudgetExceededTests | planli |
| N09-105 | `BudgetExceededRegistry` | class | BudgetExceeded kayit/katalog yonetimi | VersaCoder.Domain | net48;net10.0-windows | BudgetExceededTests | planli |
| N09-106 | `BudgetExceededConverter` | class | BudgetExceeded bicim donusumu | VersaCoder.Domain | net48;net10.0-windows | BudgetExceededTests | planli |
| N09-107 | `BudgetExceededTests` | class | BudgetExceeded testleri (xUnit) | VersaCoder.Domain | net48;net10.0-windows | - | planli |
| N09-108 | `BudgetExceededBuilder` | class | BudgetExceeded uretici (builder) | VersaCoder.Domain | net48;net10.0-windows | BudgetExceededTests | planli |
| N09-109 | `IPlanUpdated` | interface | PlanUpdated sozlesmesi (port) | VersaCoder.Domain | net48;net10.0-windows | PlanUpdatedTests | planli |
| N09-110 | `PlanUpdated` | class | PlanUpdated cekirdek uygulamasi | VersaCoder.Domain | net48;net10.0-windows | PlanUpdatedTests | planli |
| N09-111 | `PlanUpdatedDto` | record | PlanUpdated veri transferi modeli | VersaCoder.Domain | net48;net10.0-windows | PlanUpdatedTests | planli |
| N09-112 | `PlanUpdatedMapper` | class | PlanUpdated <-> DTO esleme | VersaCoder.Domain | net48;net10.0-windows | PlanUpdatedTests | planli |
| N09-113 | `PlanUpdatedValidator` | class | PlanUpdated dogrulama kurallari | VersaCoder.Domain | net48;net10.0-windows | PlanUpdatedTests | planli |
| N09-114 | `PlanUpdatedFactory` | class | PlanUpdated uretimi ve varsayilanlari | VersaCoder.Domain | net48;net10.0-windows | PlanUpdatedTests | planli |
| N09-115 | `PlanUpdatedOptions` | record | PlanUpdated yapilandirmasi | VersaCoder.Domain | net48;net10.0-windows | PlanUpdatedTests | planli |
| N09-116 | `PlanUpdatedProvider` | class | PlanUpdated saglayici uygulamasi | VersaCoder.Domain | net48;net10.0-windows | PlanUpdatedTests | planli |
| N09-117 | `PlanUpdatedRegistry` | class | PlanUpdated kayit/katalog yonetimi | VersaCoder.Domain | net48;net10.0-windows | PlanUpdatedTests | planli |
| N09-118 | `PlanUpdatedConverter` | class | PlanUpdated bicim donusumu | VersaCoder.Domain | net48;net10.0-windows | PlanUpdatedTests | planli |
| N09-119 | `PlanUpdatedTests` | class | PlanUpdated testleri (xUnit) | VersaCoder.Domain | net48;net10.0-windows | - | planli |
| N09-120 | `PlanUpdatedBuilder` | class | PlanUpdated uretici (builder) | VersaCoder.Domain | net48;net10.0-windows | PlanUpdatedTests | planli |
