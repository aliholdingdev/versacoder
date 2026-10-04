---
title: "Katalog Dugumu N11 - Enum Tipleri (l1)"
type: catalog
category: architecture
status: needs-validation
version: 1.0.0
updated: 2026-10-04
authority: derived
scope: "N11 enums dugumu: kimlik, src klasoru, aile yapisi, ornek kayit ve yon kurali (ADR-013 + ADR-028)"
---

# Katalog Dugumu N11 - Enum Tipleri

**Zorunlu Baglantilar:** [catalog index](index.md) · [node-inventory](node-inventory.md) · [mimari index](../index.md) · [ADR-028](../../.decisions/accepted/ADR-028-layered-baseline-9-projects.md) · [ADR-013](../../.decisions/accepted/ADR-013-layered-architecture-52-nodes.md)

> **Tek cumlelik sorumluluk:** *N11 enums hangi projede ve klasorde yasar, ne yapar, kac kayit hedefler* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

**N11 (enums), l1 Domain katmaninin alt katmanidir: `VersaCoder.Domain` projesinde `src/VersaCoder.Domain/Enums/` klasorunde yasar.** Role, MessageType, ToolKind gibi alan sozluklerini tek yerde tanimlar. 10 aile x 12 rol = 120 kayit hedefi (ADR-013 asgari 112); durum `planli`.

Kayitlar ADR-013 2.1 biciminde 8 sutun tasir; bu dosya aile kalibini ve ornek satirlari gosterir, tam 120 satirlik liste bu dosyanin boyut sinirina sigmaz (DOĞRULAMA GEREKLI).

```text
+--------------------------------------------------+
| l1 Domain -> VersaCoder.Domain
| src/VersaCoder.Domain/Enums/
| 10 aile x 12 rol = 120 kayit (esik 112)
+--------------------------------------------------+
```

| Alan | Deger |
|---|---|
| Dugum | N11 |
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
| 1 | Role, MessageType, ToolKind gibi alan sozluklerini tek yerde tanimlar | System.Data, Dapper, dosya veya git cagirmak |
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
| 1 | N11 | enums | `src/VersaCoder.Domain/Enums/` |

**Kanit yolu (ADR-028 2.3/5):** tarama yolu gercek proje adini icerir; genis kalip (`src/**/Domain/**`) sahte PASS uretir.

```powershell
Test-Path 'C:\versacoder\src\VersaCoder.Domain\Enums'
```

Dizin su an yok cikti `False` olur; PASS yazilmaz -> **DOĞRULAMA GEREKLI**.

---

## §3 Aile Yapisi

Rol kalibi (12): `I<F>` · `<F>` · `<F>Dto` · `<F>Mapper` · `<F>Validator` · `<F>Factory` · `<F>Options` · `<F>Provider` · `<F>Registry` · `<F>Converter` · `<F>Tests` · `<F>Builder`

| # | Aile | # | Aile |
|---|---|---|---|
| 1 | Role | 6 | ErrorSeverity |
| 2 | MessageType | 7 | ModelProvider |
| 3 | ToolKind | 8 | Permission |
| 4 | SessionState | 9 | Locale |
| 5 | TaskState | 10 | RunMode |

Her aile ayni rol kalibini 12 kez tekrarlar; kayit sayisi = aile x rol.

---

## §4 Kayit Ornegi (8 sutun)

| ID | Bilesen | Tip | Sorumluluk | Proje | TFM | Test | Durum |
|---|---|---|---|---|---|---|---|
| N11-001 | `IRole` | interface | Role sozlesmesi (port) | VersaCoder.Domain | net48;net10.0-windows | RoleTests | planli |
| N11-002 | `Role` | class | Role cekirdek uygulamasi | VersaCoder.Domain | net48;net10.0-windows | RoleTests | planli |
| N11-003 | `RoleDto` | record | Role veri transferi modeli | VersaCoder.Domain | net48;net10.0-windows | RoleTests | planli |
| N11-004 | `RoleMapper` | class | Role <-> DTO esleme | VersaCoder.Domain | net48;net10.0-windows | RoleTests | planli |
| N11-005 | `RoleValidator` | class | Role dogrulama kurallari | VersaCoder.Domain | net48;net10.0-windows | RoleTests | planli |
| N11-006 | `RoleFactory` | class | Role uretimi ve varsayilanlari | VersaCoder.Domain | net48;net10.0-windows | RoleTests | planli |

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
| 3 | Test sinifi | RoleTests (xUnit) | planli |
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
(Select-String -Path 'C:\versacoder\.ai\architecture\catalog\N11-enums.md' -Pattern '^\|\s*N\d{2}-\d{3}\s*\|').Count
# 2) boyut bandi - beklenen: 2600-8500
(Get-Item 'C:\versacoder\.ai\architecture\catalog\N11-enums.md').Length
# 3) klasor kaniti - gercek proje adi (ADR-028 2.3/5)
(Test-Path 'C:\versacoder\src\VersaCoder.Domain\Enums')
```

Katalog sayimi `powershell -NoProfile -ExecutionPolicy Bypass -File C:\versacoder\.ai\scripts\count-components.ps1` ile alinir; bu batch sonunda toplam esik (5000) henuz karsilanmaz - kalan 26 node dosyasi sonraki batchtir.

## §10 Kayit Tablosu (ADR-013 2.2 - kaynak vault port)

| N11-007 | `RoleOptions` | record | Role yapilandirmasi | VersaCoder.Domain | net48;net10.0-windows | RoleTests | planli |
| N11-008 | `RoleProvider` | class | Role saglayici uygulamasi | VersaCoder.Domain | net48;net10.0-windows | RoleTests | planli |
| N11-009 | `RoleRegistry` | class | Role kayit/katalog yonetimi | VersaCoder.Domain | net48;net10.0-windows | RoleTests | planli |
| N11-010 | `RoleConverter` | class | Role bicim donusumu | VersaCoder.Domain | net48;net10.0-windows | RoleTests | planli |
| N11-011 | `RoleTests` | class | Role testleri (xUnit) | VersaCoder.Domain | net48;net10.0-windows | - | planli |
| N11-012 | `RoleBuilder` | class | Role uretici (builder) | VersaCoder.Domain | net48;net10.0-windows | RoleTests | planli |
| N11-013 | `IMessageType` | interface | MessageType sozlesmesi (port) | VersaCoder.Domain | net48;net10.0-windows | MessageTypeTests | planli |
| N11-014 | `MessageType` | class | MessageType cekirdek uygulamasi | VersaCoder.Domain | net48;net10.0-windows | MessageTypeTests | planli |
| N11-015 | `MessageTypeDto` | record | MessageType veri transferi modeli | VersaCoder.Domain | net48;net10.0-windows | MessageTypeTests | planli |
| N11-016 | `MessageTypeMapper` | class | MessageType <-> DTO esleme | VersaCoder.Domain | net48;net10.0-windows | MessageTypeTests | planli |
| N11-017 | `MessageTypeValidator` | class | MessageType dogrulama kurallari | VersaCoder.Domain | net48;net10.0-windows | MessageTypeTests | planli |
| N11-018 | `MessageTypeFactory` | class | MessageType uretimi ve varsayilanlari | VersaCoder.Domain | net48;net10.0-windows | MessageTypeTests | planli |
| N11-019 | `MessageTypeOptions` | record | MessageType yapilandirmasi | VersaCoder.Domain | net48;net10.0-windows | MessageTypeTests | planli |
| N11-020 | `MessageTypeProvider` | class | MessageType saglayici uygulamasi | VersaCoder.Domain | net48;net10.0-windows | MessageTypeTests | planli |
| N11-021 | `MessageTypeRegistry` | class | MessageType kayit/katalog yonetimi | VersaCoder.Domain | net48;net10.0-windows | MessageTypeTests | planli |
| N11-022 | `MessageTypeConverter` | class | MessageType bicim donusumu | VersaCoder.Domain | net48;net10.0-windows | MessageTypeTests | planli |
| N11-023 | `MessageTypeTests` | class | MessageType testleri (xUnit) | VersaCoder.Domain | net48;net10.0-windows | - | planli |
| N11-024 | `MessageTypeBuilder` | class | MessageType uretici (builder) | VersaCoder.Domain | net48;net10.0-windows | MessageTypeTests | planli |
| N11-025 | `IToolKind` | interface | ToolKind sozlesmesi (port) | VersaCoder.Domain | net48;net10.0-windows | ToolKindTests | planli |
| N11-026 | `ToolKind` | class | ToolKind cekirdek uygulamasi | VersaCoder.Domain | net48;net10.0-windows | ToolKindTests | planli |
| N11-027 | `ToolKindDto` | record | ToolKind veri transferi modeli | VersaCoder.Domain | net48;net10.0-windows | ToolKindTests | planli |
| N11-028 | `ToolKindMapper` | class | ToolKind <-> DTO esleme | VersaCoder.Domain | net48;net10.0-windows | ToolKindTests | planli |
| N11-029 | `ToolKindValidator` | class | ToolKind dogrulama kurallari | VersaCoder.Domain | net48;net10.0-windows | ToolKindTests | planli |
| N11-030 | `ToolKindFactory` | class | ToolKind uretimi ve varsayilanlari | VersaCoder.Domain | net48;net10.0-windows | ToolKindTests | planli |
| N11-031 | `ToolKindOptions` | record | ToolKind yapilandirmasi | VersaCoder.Domain | net48;net10.0-windows | ToolKindTests | planli |
| N11-032 | `ToolKindProvider` | class | ToolKind saglayici uygulamasi | VersaCoder.Domain | net48;net10.0-windows | ToolKindTests | planli |
| N11-033 | `ToolKindRegistry` | class | ToolKind kayit/katalog yonetimi | VersaCoder.Domain | net48;net10.0-windows | ToolKindTests | planli |
| N11-034 | `ToolKindConverter` | class | ToolKind bicim donusumu | VersaCoder.Domain | net48;net10.0-windows | ToolKindTests | planli |
| N11-035 | `ToolKindTests` | class | ToolKind testleri (xUnit) | VersaCoder.Domain | net48;net10.0-windows | - | planli |
| N11-036 | `ToolKindBuilder` | class | ToolKind uretici (builder) | VersaCoder.Domain | net48;net10.0-windows | ToolKindTests | planli |
| N11-037 | `ISessionState` | interface | SessionState sozlesmesi (port) | VersaCoder.Domain | net48;net10.0-windows | SessionStateTests | planli |
| N11-038 | `SessionState` | class | SessionState cekirdek uygulamasi | VersaCoder.Domain | net48;net10.0-windows | SessionStateTests | planli |
| N11-039 | `SessionStateDto` | record | SessionState veri transferi modeli | VersaCoder.Domain | net48;net10.0-windows | SessionStateTests | planli |
| N11-040 | `SessionStateMapper` | class | SessionState <-> DTO esleme | VersaCoder.Domain | net48;net10.0-windows | SessionStateTests | planli |
| N11-041 | `SessionStateValidator` | class | SessionState dogrulama kurallari | VersaCoder.Domain | net48;net10.0-windows | SessionStateTests | planli |
| N11-042 | `SessionStateFactory` | class | SessionState uretimi ve varsayilanlari | VersaCoder.Domain | net48;net10.0-windows | SessionStateTests | planli |
| N11-043 | `SessionStateOptions` | record | SessionState yapilandirmasi | VersaCoder.Domain | net48;net10.0-windows | SessionStateTests | planli |
| N11-044 | `SessionStateProvider` | class | SessionState saglayici uygulamasi | VersaCoder.Domain | net48;net10.0-windows | SessionStateTests | planli |
| N11-045 | `SessionStateRegistry` | class | SessionState kayit/katalog yonetimi | VersaCoder.Domain | net48;net10.0-windows | SessionStateTests | planli |
| N11-046 | `SessionStateConverter` | class | SessionState bicim donusumu | VersaCoder.Domain | net48;net10.0-windows | SessionStateTests | planli |
| N11-047 | `SessionStateTests` | class | SessionState testleri (xUnit) | VersaCoder.Domain | net48;net10.0-windows | - | planli |
| N11-048 | `SessionStateBuilder` | class | SessionState uretici (builder) | VersaCoder.Domain | net48;net10.0-windows | SessionStateTests | planli |
| N11-049 | `ITaskState` | interface | TaskState sozlesmesi (port) | VersaCoder.Domain | net48;net10.0-windows | TaskStateTests | planli |
| N11-050 | `TaskState` | class | TaskState cekirdek uygulamasi | VersaCoder.Domain | net48;net10.0-windows | TaskStateTests | planli |
| N11-051 | `TaskStateDto` | record | TaskState veri transferi modeli | VersaCoder.Domain | net48;net10.0-windows | TaskStateTests | planli |
| N11-052 | `TaskStateMapper` | class | TaskState <-> DTO esleme | VersaCoder.Domain | net48;net10.0-windows | TaskStateTests | planli |
| N11-053 | `TaskStateValidator` | class | TaskState dogrulama kurallari | VersaCoder.Domain | net48;net10.0-windows | TaskStateTests | planli |
| N11-054 | `TaskStateFactory` | class | TaskState uretimi ve varsayilanlari | VersaCoder.Domain | net48;net10.0-windows | TaskStateTests | planli |
| N11-055 | `TaskStateOptions` | record | TaskState yapilandirmasi | VersaCoder.Domain | net48;net10.0-windows | TaskStateTests | planli |
| N11-056 | `TaskStateProvider` | class | TaskState saglayici uygulamasi | VersaCoder.Domain | net48;net10.0-windows | TaskStateTests | planli |
| N11-057 | `TaskStateRegistry` | class | TaskState kayit/katalog yonetimi | VersaCoder.Domain | net48;net10.0-windows | TaskStateTests | planli |
| N11-058 | `TaskStateConverter` | class | TaskState bicim donusumu | VersaCoder.Domain | net48;net10.0-windows | TaskStateTests | planli |
| N11-059 | `TaskStateTests` | class | TaskState testleri (xUnit) | VersaCoder.Domain | net48;net10.0-windows | - | planli |
| N11-060 | `TaskStateBuilder` | class | TaskState uretici (builder) | VersaCoder.Domain | net48;net10.0-windows | TaskStateTests | planli |
| N11-061 | `IErrorSeverity` | interface | ErrorSeverity sozlesmesi (port) | VersaCoder.Domain | net48;net10.0-windows | ErrorSeverityTests | planli |
| N11-062 | `ErrorSeverity` | class | ErrorSeverity cekirdek uygulamasi | VersaCoder.Domain | net48;net10.0-windows | ErrorSeverityTests | planli |
| N11-063 | `ErrorSeverityDto` | record | ErrorSeverity veri transferi modeli | VersaCoder.Domain | net48;net10.0-windows | ErrorSeverityTests | planli |
| N11-064 | `ErrorSeverityMapper` | class | ErrorSeverity <-> DTO esleme | VersaCoder.Domain | net48;net10.0-windows | ErrorSeverityTests | planli |
| N11-065 | `ErrorSeverityValidator` | class | ErrorSeverity dogrulama kurallari | VersaCoder.Domain | net48;net10.0-windows | ErrorSeverityTests | planli |
| N11-066 | `ErrorSeverityFactory` | class | ErrorSeverity uretimi ve varsayilanlari | VersaCoder.Domain | net48;net10.0-windows | ErrorSeverityTests | planli |
| N11-067 | `ErrorSeverityOptions` | record | ErrorSeverity yapilandirmasi | VersaCoder.Domain | net48;net10.0-windows | ErrorSeverityTests | planli |
| N11-068 | `ErrorSeverityProvider` | class | ErrorSeverity saglayici uygulamasi | VersaCoder.Domain | net48;net10.0-windows | ErrorSeverityTests | planli |
| N11-069 | `ErrorSeverityRegistry` | class | ErrorSeverity kayit/katalog yonetimi | VersaCoder.Domain | net48;net10.0-windows | ErrorSeverityTests | planli |
| N11-070 | `ErrorSeverityConverter` | class | ErrorSeverity bicim donusumu | VersaCoder.Domain | net48;net10.0-windows | ErrorSeverityTests | planli |
| N11-071 | `ErrorSeverityTests` | class | ErrorSeverity testleri (xUnit) | VersaCoder.Domain | net48;net10.0-windows | - | planli |
| N11-072 | `ErrorSeverityBuilder` | class | ErrorSeverity uretici (builder) | VersaCoder.Domain | net48;net10.0-windows | ErrorSeverityTests | planli |
| N11-073 | `IModelProvider` | interface | ModelProvider sozlesmesi (port) | VersaCoder.Domain | net48;net10.0-windows | ModelProviderTests | planli |
| N11-074 | `ModelProvider` | class | ModelProvider cekirdek uygulamasi | VersaCoder.Domain | net48;net10.0-windows | ModelProviderTests | planli |
| N11-075 | `ModelProviderDto` | record | ModelProvider veri transferi modeli | VersaCoder.Domain | net48;net10.0-windows | ModelProviderTests | planli |
| N11-076 | `ModelProviderMapper` | class | ModelProvider <-> DTO esleme | VersaCoder.Domain | net48;net10.0-windows | ModelProviderTests | planli |
| N11-077 | `ModelProviderValidator` | class | ModelProvider dogrulama kurallari | VersaCoder.Domain | net48;net10.0-windows | ModelProviderTests | planli |
| N11-078 | `ModelProviderFactory` | class | ModelProvider uretimi ve varsayilanlari | VersaCoder.Domain | net48;net10.0-windows | ModelProviderTests | planli |
| N11-079 | `ModelProviderOptions` | record | ModelProvider yapilandirmasi | VersaCoder.Domain | net48;net10.0-windows | ModelProviderTests | planli |
| N11-080 | `ModelProviderProvider` | class | ModelProvider saglayici uygulamasi | VersaCoder.Domain | net48;net10.0-windows | ModelProviderTests | planli |
| N11-081 | `ModelProviderRegistry` | class | ModelProvider kayit/katalog yonetimi | VersaCoder.Domain | net48;net10.0-windows | ModelProviderTests | planli |
| N11-082 | `ModelProviderConverter` | class | ModelProvider bicim donusumu | VersaCoder.Domain | net48;net10.0-windows | ModelProviderTests | planli |
| N11-083 | `ModelProviderTests` | class | ModelProvider testleri (xUnit) | VersaCoder.Domain | net48;net10.0-windows | - | planli |
| N11-084 | `ModelProviderBuilder` | class | ModelProvider uretici (builder) | VersaCoder.Domain | net48;net10.0-windows | ModelProviderTests | planli |
| N11-085 | `IPermission` | interface | Permission sozlesmesi (port) | VersaCoder.Domain | net48;net10.0-windows | PermissionTests | planli |
| N11-086 | `Permission` | class | Permission cekirdek uygulamasi | VersaCoder.Domain | net48;net10.0-windows | PermissionTests | planli |
| N11-087 | `PermissionDto` | record | Permission veri transferi modeli | VersaCoder.Domain | net48;net10.0-windows | PermissionTests | planli |
| N11-088 | `PermissionMapper` | class | Permission <-> DTO esleme | VersaCoder.Domain | net48;net10.0-windows | PermissionTests | planli |
| N11-089 | `PermissionValidator` | class | Permission dogrulama kurallari | VersaCoder.Domain | net48;net10.0-windows | PermissionTests | planli |
| N11-090 | `PermissionFactory` | class | Permission uretimi ve varsayilanlari | VersaCoder.Domain | net48;net10.0-windows | PermissionTests | planli |
| N11-091 | `PermissionOptions` | record | Permission yapilandirmasi | VersaCoder.Domain | net48;net10.0-windows | PermissionTests | planli |
| N11-092 | `PermissionProvider` | class | Permission saglayici uygulamasi | VersaCoder.Domain | net48;net10.0-windows | PermissionTests | planli |
| N11-093 | `PermissionRegistry` | class | Permission kayit/katalog yonetimi | VersaCoder.Domain | net48;net10.0-windows | PermissionTests | planli |
| N11-094 | `PermissionConverter` | class | Permission bicim donusumu | VersaCoder.Domain | net48;net10.0-windows | PermissionTests | planli |
| N11-095 | `PermissionTests` | class | Permission testleri (xUnit) | VersaCoder.Domain | net48;net10.0-windows | - | planli |
| N11-096 | `PermissionBuilder` | class | Permission uretici (builder) | VersaCoder.Domain | net48;net10.0-windows | PermissionTests | planli |
| N11-097 | `ILocale` | interface | Locale sozlesmesi (port) | VersaCoder.Domain | net48;net10.0-windows | LocaleTests | planli |
| N11-098 | `Locale` | class | Locale cekirdek uygulamasi | VersaCoder.Domain | net48;net10.0-windows | LocaleTests | planli |
| N11-099 | `LocaleDto` | record | Locale veri transferi modeli | VersaCoder.Domain | net48;net10.0-windows | LocaleTests | planli |
| N11-100 | `LocaleMapper` | class | Locale <-> DTO esleme | VersaCoder.Domain | net48;net10.0-windows | LocaleTests | planli |
| N11-101 | `LocaleValidator` | class | Locale dogrulama kurallari | VersaCoder.Domain | net48;net10.0-windows | LocaleTests | planli |
| N11-102 | `LocaleFactory` | class | Locale uretimi ve varsayilanlari | VersaCoder.Domain | net48;net10.0-windows | LocaleTests | planli |
| N11-103 | `LocaleOptions` | record | Locale yapilandirmasi | VersaCoder.Domain | net48;net10.0-windows | LocaleTests | planli |
| N11-104 | `LocaleProvider` | class | Locale saglayici uygulamasi | VersaCoder.Domain | net48;net10.0-windows | LocaleTests | planli |
| N11-105 | `LocaleRegistry` | class | Locale kayit/katalog yonetimi | VersaCoder.Domain | net48;net10.0-windows | LocaleTests | planli |
| N11-106 | `LocaleConverter` | class | Locale bicim donusumu | VersaCoder.Domain | net48;net10.0-windows | LocaleTests | planli |
| N11-107 | `LocaleTests` | class | Locale testleri (xUnit) | VersaCoder.Domain | net48;net10.0-windows | - | planli |
| N11-108 | `LocaleBuilder` | class | Locale uretici (builder) | VersaCoder.Domain | net48;net10.0-windows | LocaleTests | planli |
| N11-109 | `IRunMode` | interface | RunMode sozlesmesi (port) | VersaCoder.Domain | net48;net10.0-windows | RunModeTests | planli |
| N11-110 | `RunMode` | class | RunMode cekirdek uygulamasi | VersaCoder.Domain | net48;net10.0-windows | RunModeTests | planli |
| N11-111 | `RunModeDto` | record | RunMode veri transferi modeli | VersaCoder.Domain | net48;net10.0-windows | RunModeTests | planli |
| N11-112 | `RunModeMapper` | class | RunMode <-> DTO esleme | VersaCoder.Domain | net48;net10.0-windows | RunModeTests | planli |
| N11-113 | `RunModeValidator` | class | RunMode dogrulama kurallari | VersaCoder.Domain | net48;net10.0-windows | RunModeTests | planli |
| N11-114 | `RunModeFactory` | class | RunMode uretimi ve varsayilanlari | VersaCoder.Domain | net48;net10.0-windows | RunModeTests | planli |
| N11-115 | `RunModeOptions` | record | RunMode yapilandirmasi | VersaCoder.Domain | net48;net10.0-windows | RunModeTests | planli |
| N11-116 | `RunModeProvider` | class | RunMode saglayici uygulamasi | VersaCoder.Domain | net48;net10.0-windows | RunModeTests | planli |
| N11-117 | `RunModeRegistry` | class | RunMode kayit/katalog yonetimi | VersaCoder.Domain | net48;net10.0-windows | RunModeTests | planli |
| N11-118 | `RunModeConverter` | class | RunMode bicim donusumu | VersaCoder.Domain | net48;net10.0-windows | RunModeTests | planli |
| N11-119 | `RunModeTests` | class | RunMode testleri (xUnit) | VersaCoder.Domain | net48;net10.0-windows | - | planli |
| N11-120 | `RunModeBuilder` | class | RunMode uretici (builder) | VersaCoder.Domain | net48;net10.0-windows | RunModeTests | planli |
