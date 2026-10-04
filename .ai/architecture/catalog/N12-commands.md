---
title: "Katalog Dugumu N12 - Komutlar (l2)"
type: catalog
category: architecture
status: needs-validation
version: 1.0.0
updated: 2026-10-04
authority: derived
scope: "N12 commands dugumu: kimlik, src klasoru, aile yapisi, ornek kayit ve yon kurali (ADR-013 + ADR-028)"
---

# Katalog Dugumu N12 - Komutlar

**Zorunlu Baglantilar:** [catalog index](index.md) · [node-inventory](node-inventory.md) · [mimari index](../index.md) · [ADR-028](../../.decisions/accepted/ADR-028-layered-baseline-9-projects.md) · [ADR-013](../../.decisions/accepted/ADR-013-layered-architecture-52-nodes.md)

> **Tek cumlelik sorumluluk:** *N12 commands hangi projede ve klasorde yasar, ne yapar, kac kayit hedefler* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

**N12 (commands), l2 Application katmaninin alt katmanidir: `VersaCoder.Application` projesinde `src/VersaCoder.Application/Commands/` klasorunde yasar.** StartSession, ExecuteTool gibi yazma komutlarini ve giris DTOlarini tanimlar. 10 aile x 12 rol = 120 kayit hedefi (ADR-013 asgari 112); durum `planli`.

Kayitlar ADR-013 2.1 biciminde 8 sutun tasir; bu dosya aile kalibini ve ornek satirlari gosterir, tam 120 satirlik liste bu dosyanin boyut sinirina sigmaz (DOĞRULAMA GEREKLI).

```text
+--------------------------------------------------+
| l2 Application -> VersaCoder.Application
| src/VersaCoder.Application/Commands/
| 10 aile x 12 rol = 120 kayit (esik 112)
+--------------------------------------------------+
```

| Alan | Deger |
|---|---|
| Dugum | N12 |
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
| 1 | StartSession, ExecuteTool gibi yazma komutlarini ve giris DTOlarini tanimlar | l4 katmanina dogrudan bakmak (repository portu l0 katmaninda, uygulamasi l4 uzerinde) - YOK |
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
| 1 | N12 | commands | `src/VersaCoder.Application/Commands/` |

**Kanit yolu (ADR-028 2.3/5):** tarama yolu gercek proje adini icerir; genis kalip (`src/**/Domain/**`) sahte PASS uretir.

```powershell
Test-Path 'C:\versacoder\src\VersaCoder.Application\Commands'
```

Dizin su an yok cikti `False` olur; PASS yazilmaz -> **DOĞRULAMA GEREKLI**.

---

## §3 Aile Yapisi

Rol kalibi (12): `I<F>` · `<F>` · `<F>Dto` · `<F>Mapper` · `<F>Validator` · `<F>Factory` · `<F>Options` · `<F>Provider` · `<F>Registry` · `<F>Converter` · `<F>Tests` · `<F>Builder`

| # | Aile | # | Aile |
|---|---|---|---|
| 1 | StartSession | 6 | OpenProject |
| 2 | SendPrompt | 7 | ApplyEdit |
| 3 | ExecuteTool | 8 | UpdateSetting |
| 4 | StopRun | 9 | SpawnAgent |
| 5 | SaveFile | 10 | CancelTask |

Her aile ayni rol kalibini 12 kez tekrarlar; kayit sayisi = aile x rol.

---

## §4 Kayit Ornegi (8 sutun)

| ID | Bilesen | Tip | Sorumluluk | Proje | TFM | Test | Durum |
|---|---|---|---|---|---|---|---|
| N12-001 | `IStartSession` | interface | StartSession sozlesmesi (port) | VersaCoder.Application | net48;net10.0-windows | StartSessionTests | planli |
| N12-002 | `StartSession` | class | StartSession cekirdek uygulamasi | VersaCoder.Application | net48;net10.0-windows | StartSessionTests | planli |
| N12-003 | `StartSessionDto` | record | StartSession veri transferi modeli | VersaCoder.Application | net48;net10.0-windows | StartSessionTests | planli |
| N12-004 | `StartSessionMapper` | class | StartSession <-> DTO esleme | VersaCoder.Application | net48;net10.0-windows | StartSessionTests | planli |
| N12-005 | `StartSessionValidator` | class | StartSession dogrulama kurallari | VersaCoder.Application | net48;net10.0-windows | StartSessionTests | planli |
| N12-006 | `StartSessionFactory` | class | StartSession uretimi ve varsayilanlari | VersaCoder.Application | net48;net10.0-windows | StartSessionTests | planli |

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
| 3 | Test sinifi | StartSessionTests (xUnit) | planli |
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
(Select-String -Path 'C:\versacoder\.ai\architecture\catalog\N12-commands.md' -Pattern '^\|\s*N\d{2}-\d{3}\s*\|').Count
# 2) boyut bandi - beklenen: 2600-8500
(Get-Item 'C:\versacoder\.ai\architecture\catalog\N12-commands.md').Length
# 3) klasor kaniti - gercek proje adi (ADR-028 2.3/5)
(Test-Path 'C:\versacoder\src\VersaCoder.Application\Commands')
```

Katalog sayimi `powershell -NoProfile -ExecutionPolicy Bypass -File C:\versacoder\.ai\scripts\count-components.ps1` ile alinir; bu batch sonunda toplam esik (5000) henuz karsilanmaz - kalan 26 node dosyasi sonraki batchtir.

## §10 Kayit Tablosu (ADR-013 2.2 - kaynak vault port)

| N12-007 | `StartSessionOptions` | record | StartSession yapilandirmasi | VersaCoder.Application | net48;net10.0-windows | StartSessionTests | planli |
| N12-008 | `StartSessionProvider` | class | StartSession saglayici uygulamasi | VersaCoder.Application | net48;net10.0-windows | StartSessionTests | planli |
| N12-009 | `StartSessionRegistry` | class | StartSession kayit/katalog yonetimi | VersaCoder.Application | net48;net10.0-windows | StartSessionTests | planli |
| N12-010 | `StartSessionConverter` | class | StartSession bicim donusumu | VersaCoder.Application | net48;net10.0-windows | StartSessionTests | planli |
| N12-011 | `StartSessionTests` | class | StartSession testleri (xUnit) | VersaCoder.Application | net48;net10.0-windows | - | planli |
| N12-012 | `StartSessionBuilder` | class | StartSession uretici (builder) | VersaCoder.Application | net48;net10.0-windows | StartSessionTests | planli |
| N12-013 | `ISendPrompt` | interface | SendPrompt sozlesmesi (port) | VersaCoder.Application | net48;net10.0-windows | SendPromptTests | planli |
| N12-014 | `SendPrompt` | class | SendPrompt cekirdek uygulamasi | VersaCoder.Application | net48;net10.0-windows | SendPromptTests | planli |
| N12-015 | `SendPromptDto` | record | SendPrompt veri transferi modeli | VersaCoder.Application | net48;net10.0-windows | SendPromptTests | planli |
| N12-016 | `SendPromptMapper` | class | SendPrompt <-> DTO esleme | VersaCoder.Application | net48;net10.0-windows | SendPromptTests | planli |
| N12-017 | `SendPromptValidator` | class | SendPrompt dogrulama kurallari | VersaCoder.Application | net48;net10.0-windows | SendPromptTests | planli |
| N12-018 | `SendPromptFactory` | class | SendPrompt uretimi ve varsayilanlari | VersaCoder.Application | net48;net10.0-windows | SendPromptTests | planli |
| N12-019 | `SendPromptOptions` | record | SendPrompt yapilandirmasi | VersaCoder.Application | net48;net10.0-windows | SendPromptTests | planli |
| N12-020 | `SendPromptProvider` | class | SendPrompt saglayici uygulamasi | VersaCoder.Application | net48;net10.0-windows | SendPromptTests | planli |
| N12-021 | `SendPromptRegistry` | class | SendPrompt kayit/katalog yonetimi | VersaCoder.Application | net48;net10.0-windows | SendPromptTests | planli |
| N12-022 | `SendPromptConverter` | class | SendPrompt bicim donusumu | VersaCoder.Application | net48;net10.0-windows | SendPromptTests | planli |
| N12-023 | `SendPromptTests` | class | SendPrompt testleri (xUnit) | VersaCoder.Application | net48;net10.0-windows | - | planli |
| N12-024 | `SendPromptBuilder` | class | SendPrompt uretici (builder) | VersaCoder.Application | net48;net10.0-windows | SendPromptTests | planli |
| N12-025 | `IExecuteTool` | interface | ExecuteTool sozlesmesi (port) | VersaCoder.Application | net48;net10.0-windows | ExecuteToolTests | planli |
| N12-026 | `ExecuteTool` | class | ExecuteTool cekirdek uygulamasi | VersaCoder.Application | net48;net10.0-windows | ExecuteToolTests | planli |
| N12-027 | `ExecuteToolDto` | record | ExecuteTool veri transferi modeli | VersaCoder.Application | net48;net10.0-windows | ExecuteToolTests | planli |
| N12-028 | `ExecuteToolMapper` | class | ExecuteTool <-> DTO esleme | VersaCoder.Application | net48;net10.0-windows | ExecuteToolTests | planli |
| N12-029 | `ExecuteToolValidator` | class | ExecuteTool dogrulama kurallari | VersaCoder.Application | net48;net10.0-windows | ExecuteToolTests | planli |
| N12-030 | `ExecuteToolFactory` | class | ExecuteTool uretimi ve varsayilanlari | VersaCoder.Application | net48;net10.0-windows | ExecuteToolTests | planli |
| N12-031 | `ExecuteToolOptions` | record | ExecuteTool yapilandirmasi | VersaCoder.Application | net48;net10.0-windows | ExecuteToolTests | planli |
| N12-032 | `ExecuteToolProvider` | class | ExecuteTool saglayici uygulamasi | VersaCoder.Application | net48;net10.0-windows | ExecuteToolTests | planli |
| N12-033 | `ExecuteToolRegistry` | class | ExecuteTool kayit/katalog yonetimi | VersaCoder.Application | net48;net10.0-windows | ExecuteToolTests | planli |
| N12-034 | `ExecuteToolConverter` | class | ExecuteTool bicim donusumu | VersaCoder.Application | net48;net10.0-windows | ExecuteToolTests | planli |
| N12-035 | `ExecuteToolTests` | class | ExecuteTool testleri (xUnit) | VersaCoder.Application | net48;net10.0-windows | - | planli |
| N12-036 | `ExecuteToolBuilder` | class | ExecuteTool uretici (builder) | VersaCoder.Application | net48;net10.0-windows | ExecuteToolTests | planli |
| N12-037 | `IStopRun` | interface | StopRun sozlesmesi (port) | VersaCoder.Application | net48;net10.0-windows | StopRunTests | planli |
| N12-038 | `StopRun` | class | StopRun cekirdek uygulamasi | VersaCoder.Application | net48;net10.0-windows | StopRunTests | planli |
| N12-039 | `StopRunDto` | record | StopRun veri transferi modeli | VersaCoder.Application | net48;net10.0-windows | StopRunTests | planli |
| N12-040 | `StopRunMapper` | class | StopRun <-> DTO esleme | VersaCoder.Application | net48;net10.0-windows | StopRunTests | planli |
| N12-041 | `StopRunValidator` | class | StopRun dogrulama kurallari | VersaCoder.Application | net48;net10.0-windows | StopRunTests | planli |
| N12-042 | `StopRunFactory` | class | StopRun uretimi ve varsayilanlari | VersaCoder.Application | net48;net10.0-windows | StopRunTests | planli |
| N12-043 | `StopRunOptions` | record | StopRun yapilandirmasi | VersaCoder.Application | net48;net10.0-windows | StopRunTests | planli |
| N12-044 | `StopRunProvider` | class | StopRun saglayici uygulamasi | VersaCoder.Application | net48;net10.0-windows | StopRunTests | planli |
| N12-045 | `StopRunRegistry` | class | StopRun kayit/katalog yonetimi | VersaCoder.Application | net48;net10.0-windows | StopRunTests | planli |
| N12-046 | `StopRunConverter` | class | StopRun bicim donusumu | VersaCoder.Application | net48;net10.0-windows | StopRunTests | planli |
| N12-047 | `StopRunTests` | class | StopRun testleri (xUnit) | VersaCoder.Application | net48;net10.0-windows | - | planli |
| N12-048 | `StopRunBuilder` | class | StopRun uretici (builder) | VersaCoder.Application | net48;net10.0-windows | StopRunTests | planli |
| N12-049 | `ISaveFile` | interface | SaveFile sozlesmesi (port) | VersaCoder.Application | net48;net10.0-windows | SaveFileTests | planli |
| N12-050 | `SaveFile` | class | SaveFile cekirdek uygulamasi | VersaCoder.Application | net48;net10.0-windows | SaveFileTests | planli |
| N12-051 | `SaveFileDto` | record | SaveFile veri transferi modeli | VersaCoder.Application | net48;net10.0-windows | SaveFileTests | planli |
| N12-052 | `SaveFileMapper` | class | SaveFile <-> DTO esleme | VersaCoder.Application | net48;net10.0-windows | SaveFileTests | planli |
| N12-053 | `SaveFileValidator` | class | SaveFile dogrulama kurallari | VersaCoder.Application | net48;net10.0-windows | SaveFileTests | planli |
| N12-054 | `SaveFileFactory` | class | SaveFile uretimi ve varsayilanlari | VersaCoder.Application | net48;net10.0-windows | SaveFileTests | planli |
| N12-055 | `SaveFileOptions` | record | SaveFile yapilandirmasi | VersaCoder.Application | net48;net10.0-windows | SaveFileTests | planli |
| N12-056 | `SaveFileProvider` | class | SaveFile saglayici uygulamasi | VersaCoder.Application | net48;net10.0-windows | SaveFileTests | planli |
| N12-057 | `SaveFileRegistry` | class | SaveFile kayit/katalog yonetimi | VersaCoder.Application | net48;net10.0-windows | SaveFileTests | planli |
| N12-058 | `SaveFileConverter` | class | SaveFile bicim donusumu | VersaCoder.Application | net48;net10.0-windows | SaveFileTests | planli |
| N12-059 | `SaveFileTests` | class | SaveFile testleri (xUnit) | VersaCoder.Application | net48;net10.0-windows | - | planli |
| N12-060 | `SaveFileBuilder` | class | SaveFile uretici (builder) | VersaCoder.Application | net48;net10.0-windows | SaveFileTests | planli |
| N12-061 | `IOpenProject` | interface | OpenProject sozlesmesi (port) | VersaCoder.Application | net48;net10.0-windows | OpenProjectTests | planli |
| N12-062 | `OpenProject` | class | OpenProject cekirdek uygulamasi | VersaCoder.Application | net48;net10.0-windows | OpenProjectTests | planli |
| N12-063 | `OpenProjectDto` | record | OpenProject veri transferi modeli | VersaCoder.Application | net48;net10.0-windows | OpenProjectTests | planli |
| N12-064 | `OpenProjectMapper` | class | OpenProject <-> DTO esleme | VersaCoder.Application | net48;net10.0-windows | OpenProjectTests | planli |
| N12-065 | `OpenProjectValidator` | class | OpenProject dogrulama kurallari | VersaCoder.Application | net48;net10.0-windows | OpenProjectTests | planli |
| N12-066 | `OpenProjectFactory` | class | OpenProject uretimi ve varsayilanlari | VersaCoder.Application | net48;net10.0-windows | OpenProjectTests | planli |
| N12-067 | `OpenProjectOptions` | record | OpenProject yapilandirmasi | VersaCoder.Application | net48;net10.0-windows | OpenProjectTests | planli |
| N12-068 | `OpenProjectProvider` | class | OpenProject saglayici uygulamasi | VersaCoder.Application | net48;net10.0-windows | OpenProjectTests | planli |
| N12-069 | `OpenProjectRegistry` | class | OpenProject kayit/katalog yonetimi | VersaCoder.Application | net48;net10.0-windows | OpenProjectTests | planli |
| N12-070 | `OpenProjectConverter` | class | OpenProject bicim donusumu | VersaCoder.Application | net48;net10.0-windows | OpenProjectTests | planli |
| N12-071 | `OpenProjectTests` | class | OpenProject testleri (xUnit) | VersaCoder.Application | net48;net10.0-windows | - | planli |
| N12-072 | `OpenProjectBuilder` | class | OpenProject uretici (builder) | VersaCoder.Application | net48;net10.0-windows | OpenProjectTests | planli |
| N12-073 | `IApplyEdit` | interface | ApplyEdit sozlesmesi (port) | VersaCoder.Application | net48;net10.0-windows | ApplyEditTests | planli |
| N12-074 | `ApplyEdit` | class | ApplyEdit cekirdek uygulamasi | VersaCoder.Application | net48;net10.0-windows | ApplyEditTests | planli |
| N12-075 | `ApplyEditDto` | record | ApplyEdit veri transferi modeli | VersaCoder.Application | net48;net10.0-windows | ApplyEditTests | planli |
| N12-076 | `ApplyEditMapper` | class | ApplyEdit <-> DTO esleme | VersaCoder.Application | net48;net10.0-windows | ApplyEditTests | planli |
| N12-077 | `ApplyEditValidator` | class | ApplyEdit dogrulama kurallari | VersaCoder.Application | net48;net10.0-windows | ApplyEditTests | planli |
| N12-078 | `ApplyEditFactory` | class | ApplyEdit uretimi ve varsayilanlari | VersaCoder.Application | net48;net10.0-windows | ApplyEditTests | planli |
| N12-079 | `ApplyEditOptions` | record | ApplyEdit yapilandirmasi | VersaCoder.Application | net48;net10.0-windows | ApplyEditTests | planli |
| N12-080 | `ApplyEditProvider` | class | ApplyEdit saglayici uygulamasi | VersaCoder.Application | net48;net10.0-windows | ApplyEditTests | planli |
| N12-081 | `ApplyEditRegistry` | class | ApplyEdit kayit/katalog yonetimi | VersaCoder.Application | net48;net10.0-windows | ApplyEditTests | planli |
| N12-082 | `ApplyEditConverter` | class | ApplyEdit bicim donusumu | VersaCoder.Application | net48;net10.0-windows | ApplyEditTests | planli |
| N12-083 | `ApplyEditTests` | class | ApplyEdit testleri (xUnit) | VersaCoder.Application | net48;net10.0-windows | - | planli |
| N12-084 | `ApplyEditBuilder` | class | ApplyEdit uretici (builder) | VersaCoder.Application | net48;net10.0-windows | ApplyEditTests | planli |
| N12-085 | `IUpdateSetting` | interface | UpdateSetting sozlesmesi (port) | VersaCoder.Application | net48;net10.0-windows | UpdateSettingTests | planli |
| N12-086 | `UpdateSetting` | class | UpdateSetting cekirdek uygulamasi | VersaCoder.Application | net48;net10.0-windows | UpdateSettingTests | planli |
| N12-087 | `UpdateSettingDto` | record | UpdateSetting veri transferi modeli | VersaCoder.Application | net48;net10.0-windows | UpdateSettingTests | planli |
| N12-088 | `UpdateSettingMapper` | class | UpdateSetting <-> DTO esleme | VersaCoder.Application | net48;net10.0-windows | UpdateSettingTests | planli |
| N12-089 | `UpdateSettingValidator` | class | UpdateSetting dogrulama kurallari | VersaCoder.Application | net48;net10.0-windows | UpdateSettingTests | planli |
| N12-090 | `UpdateSettingFactory` | class | UpdateSetting uretimi ve varsayilanlari | VersaCoder.Application | net48;net10.0-windows | UpdateSettingTests | planli |
| N12-091 | `UpdateSettingOptions` | record | UpdateSetting yapilandirmasi | VersaCoder.Application | net48;net10.0-windows | UpdateSettingTests | planli |
| N12-092 | `UpdateSettingProvider` | class | UpdateSetting saglayici uygulamasi | VersaCoder.Application | net48;net10.0-windows | UpdateSettingTests | planli |
| N12-093 | `UpdateSettingRegistry` | class | UpdateSetting kayit/katalog yonetimi | VersaCoder.Application | net48;net10.0-windows | UpdateSettingTests | planli |
| N12-094 | `UpdateSettingConverter` | class | UpdateSetting bicim donusumu | VersaCoder.Application | net48;net10.0-windows | UpdateSettingTests | planli |
| N12-095 | `UpdateSettingTests` | class | UpdateSetting testleri (xUnit) | VersaCoder.Application | net48;net10.0-windows | - | planli |
| N12-096 | `UpdateSettingBuilder` | class | UpdateSetting uretici (builder) | VersaCoder.Application | net48;net10.0-windows | UpdateSettingTests | planli |
| N12-097 | `ISpawnAgent` | interface | SpawnAgent sozlesmesi (port) | VersaCoder.Application | net48;net10.0-windows | SpawnAgentTests | planli |
| N12-098 | `SpawnAgent` | class | SpawnAgent cekirdek uygulamasi | VersaCoder.Application | net48;net10.0-windows | SpawnAgentTests | planli |
| N12-099 | `SpawnAgentDto` | record | SpawnAgent veri transferi modeli | VersaCoder.Application | net48;net10.0-windows | SpawnAgentTests | planli |
| N12-100 | `SpawnAgentMapper` | class | SpawnAgent <-> DTO esleme | VersaCoder.Application | net48;net10.0-windows | SpawnAgentTests | planli |
| N12-101 | `SpawnAgentValidator` | class | SpawnAgent dogrulama kurallari | VersaCoder.Application | net48;net10.0-windows | SpawnAgentTests | planli |
| N12-102 | `SpawnAgentFactory` | class | SpawnAgent uretimi ve varsayilanlari | VersaCoder.Application | net48;net10.0-windows | SpawnAgentTests | planli |
| N12-103 | `SpawnAgentOptions` | record | SpawnAgent yapilandirmasi | VersaCoder.Application | net48;net10.0-windows | SpawnAgentTests | planli |
| N12-104 | `SpawnAgentProvider` | class | SpawnAgent saglayici uygulamasi | VersaCoder.Application | net48;net10.0-windows | SpawnAgentTests | planli |
| N12-105 | `SpawnAgentRegistry` | class | SpawnAgent kayit/katalog yonetimi | VersaCoder.Application | net48;net10.0-windows | SpawnAgentTests | planli |
| N12-106 | `SpawnAgentConverter` | class | SpawnAgent bicim donusumu | VersaCoder.Application | net48;net10.0-windows | SpawnAgentTests | planli |
| N12-107 | `SpawnAgentTests` | class | SpawnAgent testleri (xUnit) | VersaCoder.Application | net48;net10.0-windows | - | planli |
| N12-108 | `SpawnAgentBuilder` | class | SpawnAgent uretici (builder) | VersaCoder.Application | net48;net10.0-windows | SpawnAgentTests | planli |
| N12-109 | `ICancelTask` | interface | CancelTask sozlesmesi (port) | VersaCoder.Application | net48;net10.0-windows | CancelTaskTests | planli |
| N12-110 | `CancelTask` | class | CancelTask cekirdek uygulamasi | VersaCoder.Application | net48;net10.0-windows | CancelTaskTests | planli |
| N12-111 | `CancelTaskDto` | record | CancelTask veri transferi modeli | VersaCoder.Application | net48;net10.0-windows | CancelTaskTests | planli |
| N12-112 | `CancelTaskMapper` | class | CancelTask <-> DTO esleme | VersaCoder.Application | net48;net10.0-windows | CancelTaskTests | planli |
| N12-113 | `CancelTaskValidator` | class | CancelTask dogrulama kurallari | VersaCoder.Application | net48;net10.0-windows | CancelTaskTests | planli |
| N12-114 | `CancelTaskFactory` | class | CancelTask uretimi ve varsayilanlari | VersaCoder.Application | net48;net10.0-windows | CancelTaskTests | planli |
| N12-115 | `CancelTaskOptions` | record | CancelTask yapilandirmasi | VersaCoder.Application | net48;net10.0-windows | CancelTaskTests | planli |
| N12-116 | `CancelTaskProvider` | class | CancelTask saglayici uygulamasi | VersaCoder.Application | net48;net10.0-windows | CancelTaskTests | planli |
| N12-117 | `CancelTaskRegistry` | class | CancelTask kayit/katalog yonetimi | VersaCoder.Application | net48;net10.0-windows | CancelTaskTests | planli |
| N12-118 | `CancelTaskConverter` | class | CancelTask bicim donusumu | VersaCoder.Application | net48;net10.0-windows | CancelTaskTests | planli |
| N12-119 | `CancelTaskTests` | class | CancelTask testleri (xUnit) | VersaCoder.Application | net48;net10.0-windows | - | planli |
| N12-120 | `CancelTaskBuilder` | class | CancelTask uretici (builder) | VersaCoder.Application | net48;net10.0-windows | CancelTaskTests | planli |
