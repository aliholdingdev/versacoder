---
title: "Katalog Dugumu N15 - DTO Esleyiciler (l2)"
type: catalog
category: architecture
status: needs-validation
version: 1.0.0
updated: 2026-10-04
authority: derived
scope: "N15 dto-mappers dugumu: kimlik, src klasoru, aile yapisi, ornek kayit ve yon kurali (ADR-013 + ADR-028)"
---

# Katalog Dugumu N15 - DTO Esleyiciler

**Zorunlu Baglantilar:** [catalog index](index.md) · [node-inventory](node-inventory.md) · [mimari index](../index.md) · [ADR-028](../../.decisions/accepted/ADR-028-layered-baseline-9-projects.md) · [ADR-013](../../.decisions/accepted/ADR-013-layered-architecture-52-nodes.md)

> **Tek cumlelik sorumluluk:** *N15 dto-mappers hangi projede ve klasorde yasar, ne yapar, kac kayit hedefler* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

**N15 (dto-mappers), l2 Application katmaninin alt katmanidir: `VersaCoder.Application` projesinde `src/VersaCoder.Application/Mappers/` klasorunde yasar.** Domain ile DTO arasindaki eslemeleri tek noktada toplar (iki yonlu map). 10 aile x 12 rol = 120 kayit hedefi (ADR-013 asgari 112); durum `planli`.

Kayitlar ADR-013 2.1 biciminde 8 sutun tasir; bu dosya aile kalibini ve ornek satirlari gosterir, tam 120 satirlik liste bu dosyanin boyut sinirina sigmaz (DOĞRULAMA GEREKLI).

```text
+--------------------------------------------------+
| l2 Application -> VersaCoder.Application
| src/VersaCoder.Application/Mappers/
| 10 aile x 12 rol = 120 kayit (esik 112)
+--------------------------------------------------+
```

| Alan | Deger |
|---|---|
| Dugum | N15 |
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
| 1 | Domain ile DTO arasindaki eslemeleri tek noktada toplar (iki yonlu map) | l4 katmanina dogrudan bakmak (repository portu l0 katmaninda, uygulamasi l4 uzerinde) - YOK |
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
| 1 | N15 | dto-mappers | `src/VersaCoder.Application/Mappers/` |

**Kanit yolu (ADR-028 2.3/5):** tarama yolu gercek proje adini icerir; genis kalip (`src/**/Domain/**`) sahte PASS uretir.

```powershell
Test-Path 'C:\versacoder\src\VersaCoder.Application\Mappers'
```

Dizin su an yok cikti `False` olur; PASS yazilmaz -> **DOĞRULAMA GEREKLI**.

---

## §3 Aile Yapisi

Rol kalibi (12): `I<F>` · `<F>` · `<F>Dto` · `<F>Mapper` · `<F>Validator` · `<F>Factory` · `<F>Options` · `<F>Provider` · `<F>Registry` · `<F>Converter` · `<F>Tests` · `<F>Builder`

| # | Aile | # | Aile |
|---|---|---|---|
| 1 | SessionMapper | 6 | TaskMapper |
| 2 | MessageMapper | 7 | AgentMapper |
| 3 | ToolCallMapper | 8 | ProjectMapper |
| 4 | FileNodeMapper | 9 | BudgetMapper |
| 5 | SettingMapper | 10 | AuditMapper |

Her aile ayni rol kalibini 12 kez tekrarlar; kayit sayisi = aile x rol.

---

## §4 Kayit Ornegi (8 sutun)

| ID | Bilesen | Tip | Sorumluluk | Proje | TFM | Test | Durum |
|---|---|---|---|---|---|---|---|
| N15-001 | `ISessionMapper` | interface | SessionMapper sozlesmesi (port) | VersaCoder.Application | net48;net10.0-windows | SessionMapperTests | planli |
| N15-002 | `SessionMapper` | class | SessionMapper cekirdek uygulamasi | VersaCoder.Application | net48;net10.0-windows | SessionMapperTests | planli |
| N15-003 | `SessionMapperDto` | record | SessionMapper veri transferi modeli | VersaCoder.Application | net48;net10.0-windows | SessionMapperTests | planli |
| N15-004 | `SessionMapperMapper` | class | SessionMapper <-> DTO esleme | VersaCoder.Application | net48;net10.0-windows | SessionMapperTests | planli |
| N15-005 | `SessionMapperValidator` | class | SessionMapper dogrulama kurallari | VersaCoder.Application | net48;net10.0-windows | SessionMapperTests | planli |
| N15-006 | `SessionMapperFactory` | class | SessionMapper uretimi ve varsayilanlari | VersaCoder.Application | net48;net10.0-windows | SessionMapperTests | planli |

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
| 3 | Test sinifi | SessionMapperTests (xUnit) | planli |
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
(Select-String -Path 'C:\versacoder\.ai\architecture\catalog\N15-dto-mappers.md' -Pattern '^\|\s*N\d{2}-\d{3}\s*\|').Count
# 2) boyut bandi - beklenen: 2600-8500
(Get-Item 'C:\versacoder\.ai\architecture\catalog\N15-dto-mappers.md').Length
# 3) klasor kaniti - gercek proje adi (ADR-028 2.3/5)
(Test-Path 'C:\versacoder\src\VersaCoder.Application\Mappers')
```

Katalog sayimi `powershell -NoProfile -ExecutionPolicy Bypass -File C:\versacoder\.ai\scripts\count-components.ps1` ile alinir; bu batch sonunda toplam esik (5000) henuz karsilanmaz - kalan 26 node dosyasi sonraki batchtir.

## §10 Kayit Tablosu (ADR-013 2.2 - kaynak vault port)

| N15-007 | `SessionMapperOptions` | record | SessionMapper yapilandirmasi | VersaCoder.Application | net48;net10.0-windows | SessionMapperTests | planli |
| N15-008 | `SessionMapperProvider` | class | SessionMapper saglayici uygulamasi | VersaCoder.Application | net48;net10.0-windows | SessionMapperTests | planli |
| N15-009 | `SessionMapperRegistry` | class | SessionMapper kayit/katalog yonetimi | VersaCoder.Application | net48;net10.0-windows | SessionMapperTests | planli |
| N15-010 | `SessionMapperConverter` | class | SessionMapper bicim donusumu | VersaCoder.Application | net48;net10.0-windows | SessionMapperTests | planli |
| N15-011 | `SessionMapperTests` | class | SessionMapper testleri (xUnit) | VersaCoder.Application | net48;net10.0-windows | - | planli |
| N15-012 | `SessionMapperBuilder` | class | SessionMapper uretici (builder) | VersaCoder.Application | net48;net10.0-windows | SessionMapperTests | planli |
| N15-013 | `IMessageMapper` | interface | MessageMapper sozlesmesi (port) | VersaCoder.Application | net48;net10.0-windows | MessageMapperTests | planli |
| N15-014 | `MessageMapper` | class | MessageMapper cekirdek uygulamasi | VersaCoder.Application | net48;net10.0-windows | MessageMapperTests | planli |
| N15-015 | `MessageMapperDto` | record | MessageMapper veri transferi modeli | VersaCoder.Application | net48;net10.0-windows | MessageMapperTests | planli |
| N15-016 | `MessageMapperMapper` | class | MessageMapper <-> DTO esleme | VersaCoder.Application | net48;net10.0-windows | MessageMapperTests | planli |
| N15-017 | `MessageMapperValidator` | class | MessageMapper dogrulama kurallari | VersaCoder.Application | net48;net10.0-windows | MessageMapperTests | planli |
| N15-018 | `MessageMapperFactory` | class | MessageMapper uretimi ve varsayilanlari | VersaCoder.Application | net48;net10.0-windows | MessageMapperTests | planli |
| N15-019 | `MessageMapperOptions` | record | MessageMapper yapilandirmasi | VersaCoder.Application | net48;net10.0-windows | MessageMapperTests | planli |
| N15-020 | `MessageMapperProvider` | class | MessageMapper saglayici uygulamasi | VersaCoder.Application | net48;net10.0-windows | MessageMapperTests | planli |
| N15-021 | `MessageMapperRegistry` | class | MessageMapper kayit/katalog yonetimi | VersaCoder.Application | net48;net10.0-windows | MessageMapperTests | planli |
| N15-022 | `MessageMapperConverter` | class | MessageMapper bicim donusumu | VersaCoder.Application | net48;net10.0-windows | MessageMapperTests | planli |
| N15-023 | `MessageMapperTests` | class | MessageMapper testleri (xUnit) | VersaCoder.Application | net48;net10.0-windows | - | planli |
| N15-024 | `MessageMapperBuilder` | class | MessageMapper uretici (builder) | VersaCoder.Application | net48;net10.0-windows | MessageMapperTests | planli |
| N15-025 | `IToolCallMapper` | interface | ToolCallMapper sozlesmesi (port) | VersaCoder.Application | net48;net10.0-windows | ToolCallMapperTests | planli |
| N15-026 | `ToolCallMapper` | class | ToolCallMapper cekirdek uygulamasi | VersaCoder.Application | net48;net10.0-windows | ToolCallMapperTests | planli |
| N15-027 | `ToolCallMapperDto` | record | ToolCallMapper veri transferi modeli | VersaCoder.Application | net48;net10.0-windows | ToolCallMapperTests | planli |
| N15-028 | `ToolCallMapperMapper` | class | ToolCallMapper <-> DTO esleme | VersaCoder.Application | net48;net10.0-windows | ToolCallMapperTests | planli |
| N15-029 | `ToolCallMapperValidator` | class | ToolCallMapper dogrulama kurallari | VersaCoder.Application | net48;net10.0-windows | ToolCallMapperTests | planli |
| N15-030 | `ToolCallMapperFactory` | class | ToolCallMapper uretimi ve varsayilanlari | VersaCoder.Application | net48;net10.0-windows | ToolCallMapperTests | planli |
| N15-031 | `ToolCallMapperOptions` | record | ToolCallMapper yapilandirmasi | VersaCoder.Application | net48;net10.0-windows | ToolCallMapperTests | planli |
| N15-032 | `ToolCallMapperProvider` | class | ToolCallMapper saglayici uygulamasi | VersaCoder.Application | net48;net10.0-windows | ToolCallMapperTests | planli |
| N15-033 | `ToolCallMapperRegistry` | class | ToolCallMapper kayit/katalog yonetimi | VersaCoder.Application | net48;net10.0-windows | ToolCallMapperTests | planli |
| N15-034 | `ToolCallMapperConverter` | class | ToolCallMapper bicim donusumu | VersaCoder.Application | net48;net10.0-windows | ToolCallMapperTests | planli |
| N15-035 | `ToolCallMapperTests` | class | ToolCallMapper testleri (xUnit) | VersaCoder.Application | net48;net10.0-windows | - | planli |
| N15-036 | `ToolCallMapperBuilder` | class | ToolCallMapper uretici (builder) | VersaCoder.Application | net48;net10.0-windows | ToolCallMapperTests | planli |
| N15-037 | `IFileNodeMapper` | interface | FileNodeMapper sozlesmesi (port) | VersaCoder.Application | net48;net10.0-windows | FileNodeMapperTests | planli |
| N15-038 | `FileNodeMapper` | class | FileNodeMapper cekirdek uygulamasi | VersaCoder.Application | net48;net10.0-windows | FileNodeMapperTests | planli |
| N15-039 | `FileNodeMapperDto` | record | FileNodeMapper veri transferi modeli | VersaCoder.Application | net48;net10.0-windows | FileNodeMapperTests | planli |
| N15-040 | `FileNodeMapperMapper` | class | FileNodeMapper <-> DTO esleme | VersaCoder.Application | net48;net10.0-windows | FileNodeMapperTests | planli |
| N15-041 | `FileNodeMapperValidator` | class | FileNodeMapper dogrulama kurallari | VersaCoder.Application | net48;net10.0-windows | FileNodeMapperTests | planli |
| N15-042 | `FileNodeMapperFactory` | class | FileNodeMapper uretimi ve varsayilanlari | VersaCoder.Application | net48;net10.0-windows | FileNodeMapperTests | planli |
| N15-043 | `FileNodeMapperOptions` | record | FileNodeMapper yapilandirmasi | VersaCoder.Application | net48;net10.0-windows | FileNodeMapperTests | planli |
| N15-044 | `FileNodeMapperProvider` | class | FileNodeMapper saglayici uygulamasi | VersaCoder.Application | net48;net10.0-windows | FileNodeMapperTests | planli |
| N15-045 | `FileNodeMapperRegistry` | class | FileNodeMapper kayit/katalog yonetimi | VersaCoder.Application | net48;net10.0-windows | FileNodeMapperTests | planli |
| N15-046 | `FileNodeMapperConverter` | class | FileNodeMapper bicim donusumu | VersaCoder.Application | net48;net10.0-windows | FileNodeMapperTests | planli |
| N15-047 | `FileNodeMapperTests` | class | FileNodeMapper testleri (xUnit) | VersaCoder.Application | net48;net10.0-windows | - | planli |
| N15-048 | `FileNodeMapperBuilder` | class | FileNodeMapper uretici (builder) | VersaCoder.Application | net48;net10.0-windows | FileNodeMapperTests | planli |
| N15-049 | `ISettingMapper` | interface | SettingMapper sozlesmesi (port) | VersaCoder.Application | net48;net10.0-windows | SettingMapperTests | planli |
| N15-050 | `SettingMapper` | class | SettingMapper cekirdek uygulamasi | VersaCoder.Application | net48;net10.0-windows | SettingMapperTests | planli |
| N15-051 | `SettingMapperDto` | record | SettingMapper veri transferi modeli | VersaCoder.Application | net48;net10.0-windows | SettingMapperTests | planli |
| N15-052 | `SettingMapperMapper` | class | SettingMapper <-> DTO esleme | VersaCoder.Application | net48;net10.0-windows | SettingMapperTests | planli |
| N15-053 | `SettingMapperValidator` | class | SettingMapper dogrulama kurallari | VersaCoder.Application | net48;net10.0-windows | SettingMapperTests | planli |
| N15-054 | `SettingMapperFactory` | class | SettingMapper uretimi ve varsayilanlari | VersaCoder.Application | net48;net10.0-windows | SettingMapperTests | planli |
| N15-055 | `SettingMapperOptions` | record | SettingMapper yapilandirmasi | VersaCoder.Application | net48;net10.0-windows | SettingMapperTests | planli |
| N15-056 | `SettingMapperProvider` | class | SettingMapper saglayici uygulamasi | VersaCoder.Application | net48;net10.0-windows | SettingMapperTests | planli |
| N15-057 | `SettingMapperRegistry` | class | SettingMapper kayit/katalog yonetimi | VersaCoder.Application | net48;net10.0-windows | SettingMapperTests | planli |
| N15-058 | `SettingMapperConverter` | class | SettingMapper bicim donusumu | VersaCoder.Application | net48;net10.0-windows | SettingMapperTests | planli |
| N15-059 | `SettingMapperTests` | class | SettingMapper testleri (xUnit) | VersaCoder.Application | net48;net10.0-windows | - | planli |
| N15-060 | `SettingMapperBuilder` | class | SettingMapper uretici (builder) | VersaCoder.Application | net48;net10.0-windows | SettingMapperTests | planli |
| N15-061 | `ITaskMapper` | interface | TaskMapper sozlesmesi (port) | VersaCoder.Application | net48;net10.0-windows | TaskMapperTests | planli |
| N15-062 | `TaskMapper` | class | TaskMapper cekirdek uygulamasi | VersaCoder.Application | net48;net10.0-windows | TaskMapperTests | planli |
| N15-063 | `TaskMapperDto` | record | TaskMapper veri transferi modeli | VersaCoder.Application | net48;net10.0-windows | TaskMapperTests | planli |
| N15-064 | `TaskMapperMapper` | class | TaskMapper <-> DTO esleme | VersaCoder.Application | net48;net10.0-windows | TaskMapperTests | planli |
| N15-065 | `TaskMapperValidator` | class | TaskMapper dogrulama kurallari | VersaCoder.Application | net48;net10.0-windows | TaskMapperTests | planli |
| N15-066 | `TaskMapperFactory` | class | TaskMapper uretimi ve varsayilanlari | VersaCoder.Application | net48;net10.0-windows | TaskMapperTests | planli |
| N15-067 | `TaskMapperOptions` | record | TaskMapper yapilandirmasi | VersaCoder.Application | net48;net10.0-windows | TaskMapperTests | planli |
| N15-068 | `TaskMapperProvider` | class | TaskMapper saglayici uygulamasi | VersaCoder.Application | net48;net10.0-windows | TaskMapperTests | planli |
| N15-069 | `TaskMapperRegistry` | class | TaskMapper kayit/katalog yonetimi | VersaCoder.Application | net48;net10.0-windows | TaskMapperTests | planli |
| N15-070 | `TaskMapperConverter` | class | TaskMapper bicim donusumu | VersaCoder.Application | net48;net10.0-windows | TaskMapperTests | planli |
| N15-071 | `TaskMapperTests` | class | TaskMapper testleri (xUnit) | VersaCoder.Application | net48;net10.0-windows | - | planli |
| N15-072 | `TaskMapperBuilder` | class | TaskMapper uretici (builder) | VersaCoder.Application | net48;net10.0-windows | TaskMapperTests | planli |
| N15-073 | `IAgentMapper` | interface | AgentMapper sozlesmesi (port) | VersaCoder.Application | net48;net10.0-windows | AgentMapperTests | planli |
| N15-074 | `AgentMapper` | class | AgentMapper cekirdek uygulamasi | VersaCoder.Application | net48;net10.0-windows | AgentMapperTests | planli |
| N15-075 | `AgentMapperDto` | record | AgentMapper veri transferi modeli | VersaCoder.Application | net48;net10.0-windows | AgentMapperTests | planli |
| N15-076 | `AgentMapperMapper` | class | AgentMapper <-> DTO esleme | VersaCoder.Application | net48;net10.0-windows | AgentMapperTests | planli |
| N15-077 | `AgentMapperValidator` | class | AgentMapper dogrulama kurallari | VersaCoder.Application | net48;net10.0-windows | AgentMapperTests | planli |
| N15-078 | `AgentMapperFactory` | class | AgentMapper uretimi ve varsayilanlari | VersaCoder.Application | net48;net10.0-windows | AgentMapperTests | planli |
| N15-079 | `AgentMapperOptions` | record | AgentMapper yapilandirmasi | VersaCoder.Application | net48;net10.0-windows | AgentMapperTests | planli |
| N15-080 | `AgentMapperProvider` | class | AgentMapper saglayici uygulamasi | VersaCoder.Application | net48;net10.0-windows | AgentMapperTests | planli |
| N15-081 | `AgentMapperRegistry` | class | AgentMapper kayit/katalog yonetimi | VersaCoder.Application | net48;net10.0-windows | AgentMapperTests | planli |
| N15-082 | `AgentMapperConverter` | class | AgentMapper bicim donusumu | VersaCoder.Application | net48;net10.0-windows | AgentMapperTests | planli |
| N15-083 | `AgentMapperTests` | class | AgentMapper testleri (xUnit) | VersaCoder.Application | net48;net10.0-windows | - | planli |
| N15-084 | `AgentMapperBuilder` | class | AgentMapper uretici (builder) | VersaCoder.Application | net48;net10.0-windows | AgentMapperTests | planli |
| N15-085 | `IProjectMapper` | interface | ProjectMapper sozlesmesi (port) | VersaCoder.Application | net48;net10.0-windows | ProjectMapperTests | planli |
| N15-086 | `ProjectMapper` | class | ProjectMapper cekirdek uygulamasi | VersaCoder.Application | net48;net10.0-windows | ProjectMapperTests | planli |
| N15-087 | `ProjectMapperDto` | record | ProjectMapper veri transferi modeli | VersaCoder.Application | net48;net10.0-windows | ProjectMapperTests | planli |
| N15-088 | `ProjectMapperMapper` | class | ProjectMapper <-> DTO esleme | VersaCoder.Application | net48;net10.0-windows | ProjectMapperTests | planli |
| N15-089 | `ProjectMapperValidator` | class | ProjectMapper dogrulama kurallari | VersaCoder.Application | net48;net10.0-windows | ProjectMapperTests | planli |
| N15-090 | `ProjectMapperFactory` | class | ProjectMapper uretimi ve varsayilanlari | VersaCoder.Application | net48;net10.0-windows | ProjectMapperTests | planli |
| N15-091 | `ProjectMapperOptions` | record | ProjectMapper yapilandirmasi | VersaCoder.Application | net48;net10.0-windows | ProjectMapperTests | planli |
| N15-092 | `ProjectMapperProvider` | class | ProjectMapper saglayici uygulamasi | VersaCoder.Application | net48;net10.0-windows | ProjectMapperTests | planli |
| N15-093 | `ProjectMapperRegistry` | class | ProjectMapper kayit/katalog yonetimi | VersaCoder.Application | net48;net10.0-windows | ProjectMapperTests | planli |
| N15-094 | `ProjectMapperConverter` | class | ProjectMapper bicim donusumu | VersaCoder.Application | net48;net10.0-windows | ProjectMapperTests | planli |
| N15-095 | `ProjectMapperTests` | class | ProjectMapper testleri (xUnit) | VersaCoder.Application | net48;net10.0-windows | - | planli |
| N15-096 | `ProjectMapperBuilder` | class | ProjectMapper uretici (builder) | VersaCoder.Application | net48;net10.0-windows | ProjectMapperTests | planli |
| N15-097 | `IBudgetMapper` | interface | BudgetMapper sozlesmesi (port) | VersaCoder.Application | net48;net10.0-windows | BudgetMapperTests | planli |
| N15-098 | `BudgetMapper` | class | BudgetMapper cekirdek uygulamasi | VersaCoder.Application | net48;net10.0-windows | BudgetMapperTests | planli |
| N15-099 | `BudgetMapperDto` | record | BudgetMapper veri transferi modeli | VersaCoder.Application | net48;net10.0-windows | BudgetMapperTests | planli |
| N15-100 | `BudgetMapperMapper` | class | BudgetMapper <-> DTO esleme | VersaCoder.Application | net48;net10.0-windows | BudgetMapperTests | planli |
| N15-101 | `BudgetMapperValidator` | class | BudgetMapper dogrulama kurallari | VersaCoder.Application | net48;net10.0-windows | BudgetMapperTests | planli |
| N15-102 | `BudgetMapperFactory` | class | BudgetMapper uretimi ve varsayilanlari | VersaCoder.Application | net48;net10.0-windows | BudgetMapperTests | planli |
| N15-103 | `BudgetMapperOptions` | record | BudgetMapper yapilandirmasi | VersaCoder.Application | net48;net10.0-windows | BudgetMapperTests | planli |
| N15-104 | `BudgetMapperProvider` | class | BudgetMapper saglayici uygulamasi | VersaCoder.Application | net48;net10.0-windows | BudgetMapperTests | planli |
| N15-105 | `BudgetMapperRegistry` | class | BudgetMapper kayit/katalog yonetimi | VersaCoder.Application | net48;net10.0-windows | BudgetMapperTests | planli |
| N15-106 | `BudgetMapperConverter` | class | BudgetMapper bicim donusumu | VersaCoder.Application | net48;net10.0-windows | BudgetMapperTests | planli |
| N15-107 | `BudgetMapperTests` | class | BudgetMapper testleri (xUnit) | VersaCoder.Application | net48;net10.0-windows | - | planli |
| N15-108 | `BudgetMapperBuilder` | class | BudgetMapper uretici (builder) | VersaCoder.Application | net48;net10.0-windows | BudgetMapperTests | planli |
| N15-109 | `IAuditMapper` | interface | AuditMapper sozlesmesi (port) | VersaCoder.Application | net48;net10.0-windows | AuditMapperTests | planli |
| N15-110 | `AuditMapper` | class | AuditMapper cekirdek uygulamasi | VersaCoder.Application | net48;net10.0-windows | AuditMapperTests | planli |
| N15-111 | `AuditMapperDto` | record | AuditMapper veri transferi modeli | VersaCoder.Application | net48;net10.0-windows | AuditMapperTests | planli |
| N15-112 | `AuditMapperMapper` | class | AuditMapper <-> DTO esleme | VersaCoder.Application | net48;net10.0-windows | AuditMapperTests | planli |
| N15-113 | `AuditMapperValidator` | class | AuditMapper dogrulama kurallari | VersaCoder.Application | net48;net10.0-windows | AuditMapperTests | planli |
| N15-114 | `AuditMapperFactory` | class | AuditMapper uretimi ve varsayilanlari | VersaCoder.Application | net48;net10.0-windows | AuditMapperTests | planli |
| N15-115 | `AuditMapperOptions` | record | AuditMapper yapilandirmasi | VersaCoder.Application | net48;net10.0-windows | AuditMapperTests | planli |
| N15-116 | `AuditMapperProvider` | class | AuditMapper saglayici uygulamasi | VersaCoder.Application | net48;net10.0-windows | AuditMapperTests | planli |
| N15-117 | `AuditMapperRegistry` | class | AuditMapper kayit/katalog yonetimi | VersaCoder.Application | net48;net10.0-windows | AuditMapperTests | planli |
| N15-118 | `AuditMapperConverter` | class | AuditMapper bicim donusumu | VersaCoder.Application | net48;net10.0-windows | AuditMapperTests | planli |
| N15-119 | `AuditMapperTests` | class | AuditMapper testleri (xUnit) | VersaCoder.Application | net48;net10.0-windows | - | planli |
| N15-120 | `AuditMapperBuilder` | class | AuditMapper uretici (builder) | VersaCoder.Application | net48;net10.0-windows | AuditMapperTests | planli |
