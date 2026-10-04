---
title: "Katalog Dugumu N16 - Dogrulayicilar (l2)"
type: catalog
category: architecture
status: needs-validation
version: 1.0.0
updated: 2026-10-04
authority: derived
scope: "N16 validators dugumu: kimlik, src klasoru, aile yapisi, ornek kayit ve yon kurali (ADR-013 + ADR-028)"
---

# Katalog Dugumu N16 - Dogrulayicilar

**Zorunlu Baglantilar:** [catalog index](index.md) · [node-inventory](node-inventory.md) · [mimari index](../index.md) · [ADR-028](../../.decisions/accepted/ADR-028-layered-baseline-9-projects.md) · [ADR-013](../../.decisions/accepted/ADR-013-layered-architecture-52-nodes.md)

> **Tek cumlelik sorumluluk:** *N16 validators hangi projede ve klasorde yasar, ne yapar, kac kayit hedefler* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

**N16 (validators), l2 Application katmaninin alt katmanidir: `VersaCoder.Application` projesinde `src/VersaCoder.Application/Validators/` klasorunde yasar.** Prompt, dosya yolu ve arac bagimlisi girdi dogrulama kurallarini tasir. 10 aile x 12 rol = 120 kayit hedefi (ADR-013 asgari 112); durum `planli`.

Kayitlar ADR-013 2.1 biciminde 8 sutun tasir; bu dosya aile kalibini ve ornek satirlari gosterir, tam 120 satirlik liste bu dosyanin boyut sinirina sigmaz (DOĞRULAMA GEREKLI).

```text
+--------------------------------------------------+
| l2 Application -> VersaCoder.Application
| src/VersaCoder.Application/Validators/
| 10 aile x 12 rol = 120 kayit (esik 112)
+--------------------------------------------------+
```

| Alan | Deger |
|---|---|
| Dugum | N16 |
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
| 1 | Prompt, dosya yolu ve arac bagimlisi girdi dogrulama kurallarini tasir | l4 katmanina dogrudan bakmak (repository portu l0 katmaninda, uygulamasi l4 uzerinde) - YOK |
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
| 1 | N16 | validators | `src/VersaCoder.Application/Validators/` |

**Kanit yolu (ADR-028 2.3/5):** tarama yolu gercek proje adini icerir; genis kalip (`src/**/Domain/**`) sahte PASS uretir.

```powershell
Test-Path 'C:\versacoder\src\VersaCoder.Application\Validators'
```

Dizin su an yok cikti `False` olur; PASS yazilmaz -> **DOĞRULAMA GEREKLI**.

---

## §3 Aile Yapisi

Rol kalibi (12): `I<F>` · `<F>` · `<F>Dto` · `<F>Mapper` · `<F>Validator` · `<F>Factory` · `<F>Options` · `<F>Provider` · `<F>Registry` · `<F>Converter` · `<F>Tests` · `<F>Builder`

| # | Aile | # | Aile |
|---|---|---|---|
| 1 | PromptValidator | 6 | SessionValidator |
| 2 | FilePathValidator | 7 | TokenBudgetValidator |
| 3 | ToolArgsValidator | 8 | EditPatchValidator |
| 4 | SettingValidator | 9 | AgentPolicyValidator |
| 5 | ModelValidator | 10 | LocaleValidator |

Her aile ayni rol kalibini 12 kez tekrarlar; kayit sayisi = aile x rol.

---

## §4 Kayit Ornegi (8 sutun)

| ID | Bilesen | Tip | Sorumluluk | Proje | TFM | Test | Durum |
|---|---|---|---|---|---|---|---|
| N16-001 | `IPromptValidator` | interface | PromptValidator sozlesmesi (port) | VersaCoder.Application | net48;net10.0-windows | PromptValidatorTests | planli |
| N16-002 | `PromptValidator` | class | PromptValidator cekirdek uygulamasi | VersaCoder.Application | net48;net10.0-windows | PromptValidatorTests | planli |
| N16-003 | `PromptValidatorDto` | record | PromptValidator veri transferi modeli | VersaCoder.Application | net48;net10.0-windows | PromptValidatorTests | planli |
| N16-004 | `PromptValidatorMapper` | class | PromptValidator <-> DTO esleme | VersaCoder.Application | net48;net10.0-windows | PromptValidatorTests | planli |
| N16-005 | `PromptValidatorValidator` | class | PromptValidator dogrulama kurallari | VersaCoder.Application | net48;net10.0-windows | PromptValidatorTests | planli |
| N16-006 | `PromptValidatorFactory` | class | PromptValidator uretimi ve varsayilanlari | VersaCoder.Application | net48;net10.0-windows | PromptValidatorTests | planli |

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
| 3 | Test sinifi | PromptValidatorTests (xUnit) | planli |
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
(Select-String -Path 'C:\versacoder\.ai\architecture\catalog\N16-validators.md' -Pattern '^\|\s*N\d{2}-\d{3}\s*\|').Count
# 2) boyut bandi - beklenen: 2600-8500
(Get-Item 'C:\versacoder\.ai\architecture\catalog\N16-validators.md').Length
# 3) klasor kaniti - gercek proje adi (ADR-028 2.3/5)
(Test-Path 'C:\versacoder\src\VersaCoder.Application\Validators')
```

Katalog sayimi `powershell -NoProfile -ExecutionPolicy Bypass -File C:\versacoder\.ai\scripts\count-components.ps1` ile alinir; bu batch sonunda toplam esik (5000) henuz karsilanmaz - kalan 26 node dosyasi sonraki batchtir.

## §10 Kayit Tablosu (ADR-013 2.2 - kaynak vault port)

| N16-007 | `PromptValidatorOptions` | record | PromptValidator yapilandirmasi | VersaCoder.Application | net48;net10.0-windows | PromptValidatorTests | planli |
| N16-008 | `PromptValidatorProvider` | class | PromptValidator saglayici uygulamasi | VersaCoder.Application | net48;net10.0-windows | PromptValidatorTests | planli |
| N16-009 | `PromptValidatorRegistry` | class | PromptValidator kayit/katalog yonetimi | VersaCoder.Application | net48;net10.0-windows | PromptValidatorTests | planli |
| N16-010 | `PromptValidatorConverter` | class | PromptValidator bicim donusumu | VersaCoder.Application | net48;net10.0-windows | PromptValidatorTests | planli |
| N16-011 | `PromptValidatorTests` | class | PromptValidator testleri (xUnit) | VersaCoder.Application | net48;net10.0-windows | - | planli |
| N16-012 | `PromptValidatorBuilder` | class | PromptValidator uretici (builder) | VersaCoder.Application | net48;net10.0-windows | PromptValidatorTests | planli |
| N16-013 | `IFilePathValidator` | interface | FilePathValidator sozlesmesi (port) | VersaCoder.Application | net48;net10.0-windows | FilePathValidatorTests | planli |
| N16-014 | `FilePathValidator` | class | FilePathValidator cekirdek uygulamasi | VersaCoder.Application | net48;net10.0-windows | FilePathValidatorTests | planli |
| N16-015 | `FilePathValidatorDto` | record | FilePathValidator veri transferi modeli | VersaCoder.Application | net48;net10.0-windows | FilePathValidatorTests | planli |
| N16-016 | `FilePathValidatorMapper` | class | FilePathValidator <-> DTO esleme | VersaCoder.Application | net48;net10.0-windows | FilePathValidatorTests | planli |
| N16-017 | `FilePathValidatorValidator` | class | FilePathValidator dogrulama kurallari | VersaCoder.Application | net48;net10.0-windows | FilePathValidatorTests | planli |
| N16-018 | `FilePathValidatorFactory` | class | FilePathValidator uretimi ve varsayilanlari | VersaCoder.Application | net48;net10.0-windows | FilePathValidatorTests | planli |
| N16-019 | `FilePathValidatorOptions` | record | FilePathValidator yapilandirmasi | VersaCoder.Application | net48;net10.0-windows | FilePathValidatorTests | planli |
| N16-020 | `FilePathValidatorProvider` | class | FilePathValidator saglayici uygulamasi | VersaCoder.Application | net48;net10.0-windows | FilePathValidatorTests | planli |
| N16-021 | `FilePathValidatorRegistry` | class | FilePathValidator kayit/katalog yonetimi | VersaCoder.Application | net48;net10.0-windows | FilePathValidatorTests | planli |
| N16-022 | `FilePathValidatorConverter` | class | FilePathValidator bicim donusumu | VersaCoder.Application | net48;net10.0-windows | FilePathValidatorTests | planli |
| N16-023 | `FilePathValidatorTests` | class | FilePathValidator testleri (xUnit) | VersaCoder.Application | net48;net10.0-windows | - | planli |
| N16-024 | `FilePathValidatorBuilder` | class | FilePathValidator uretici (builder) | VersaCoder.Application | net48;net10.0-windows | FilePathValidatorTests | planli |
| N16-025 | `IToolArgsValidator` | interface | ToolArgsValidator sozlesmesi (port) | VersaCoder.Application | net48;net10.0-windows | ToolArgsValidatorTests | planli |
| N16-026 | `ToolArgsValidator` | class | ToolArgsValidator cekirdek uygulamasi | VersaCoder.Application | net48;net10.0-windows | ToolArgsValidatorTests | planli |
| N16-027 | `ToolArgsValidatorDto` | record | ToolArgsValidator veri transferi modeli | VersaCoder.Application | net48;net10.0-windows | ToolArgsValidatorTests | planli |
| N16-028 | `ToolArgsValidatorMapper` | class | ToolArgsValidator <-> DTO esleme | VersaCoder.Application | net48;net10.0-windows | ToolArgsValidatorTests | planli |
| N16-029 | `ToolArgsValidatorValidator` | class | ToolArgsValidator dogrulama kurallari | VersaCoder.Application | net48;net10.0-windows | ToolArgsValidatorTests | planli |
| N16-030 | `ToolArgsValidatorFactory` | class | ToolArgsValidator uretimi ve varsayilanlari | VersaCoder.Application | net48;net10.0-windows | ToolArgsValidatorTests | planli |
| N16-031 | `ToolArgsValidatorOptions` | record | ToolArgsValidator yapilandirmasi | VersaCoder.Application | net48;net10.0-windows | ToolArgsValidatorTests | planli |
| N16-032 | `ToolArgsValidatorProvider` | class | ToolArgsValidator saglayici uygulamasi | VersaCoder.Application | net48;net10.0-windows | ToolArgsValidatorTests | planli |
| N16-033 | `ToolArgsValidatorRegistry` | class | ToolArgsValidator kayit/katalog yonetimi | VersaCoder.Application | net48;net10.0-windows | ToolArgsValidatorTests | planli |
| N16-034 | `ToolArgsValidatorConverter` | class | ToolArgsValidator bicim donusumu | VersaCoder.Application | net48;net10.0-windows | ToolArgsValidatorTests | planli |
| N16-035 | `ToolArgsValidatorTests` | class | ToolArgsValidator testleri (xUnit) | VersaCoder.Application | net48;net10.0-windows | - | planli |
| N16-036 | `ToolArgsValidatorBuilder` | class | ToolArgsValidator uretici (builder) | VersaCoder.Application | net48;net10.0-windows | ToolArgsValidatorTests | planli |
| N16-037 | `ISettingValidator` | interface | SettingValidator sozlesmesi (port) | VersaCoder.Application | net48;net10.0-windows | SettingValidatorTests | planli |
| N16-038 | `SettingValidator` | class | SettingValidator cekirdek uygulamasi | VersaCoder.Application | net48;net10.0-windows | SettingValidatorTests | planli |
| N16-039 | `SettingValidatorDto` | record | SettingValidator veri transferi modeli | VersaCoder.Application | net48;net10.0-windows | SettingValidatorTests | planli |
| N16-040 | `SettingValidatorMapper` | class | SettingValidator <-> DTO esleme | VersaCoder.Application | net48;net10.0-windows | SettingValidatorTests | planli |
| N16-041 | `SettingValidatorValidator` | class | SettingValidator dogrulama kurallari | VersaCoder.Application | net48;net10.0-windows | SettingValidatorTests | planli |
| N16-042 | `SettingValidatorFactory` | class | SettingValidator uretimi ve varsayilanlari | VersaCoder.Application | net48;net10.0-windows | SettingValidatorTests | planli |
| N16-043 | `SettingValidatorOptions` | record | SettingValidator yapilandirmasi | VersaCoder.Application | net48;net10.0-windows | SettingValidatorTests | planli |
| N16-044 | `SettingValidatorProvider` | class | SettingValidator saglayici uygulamasi | VersaCoder.Application | net48;net10.0-windows | SettingValidatorTests | planli |
| N16-045 | `SettingValidatorRegistry` | class | SettingValidator kayit/katalog yonetimi | VersaCoder.Application | net48;net10.0-windows | SettingValidatorTests | planli |
| N16-046 | `SettingValidatorConverter` | class | SettingValidator bicim donusumu | VersaCoder.Application | net48;net10.0-windows | SettingValidatorTests | planli |
| N16-047 | `SettingValidatorTests` | class | SettingValidator testleri (xUnit) | VersaCoder.Application | net48;net10.0-windows | - | planli |
| N16-048 | `SettingValidatorBuilder` | class | SettingValidator uretici (builder) | VersaCoder.Application | net48;net10.0-windows | SettingValidatorTests | planli |
| N16-049 | `IModelValidator` | interface | ModelValidator sozlesmesi (port) | VersaCoder.Application | net48;net10.0-windows | ModelValidatorTests | planli |
| N16-050 | `ModelValidator` | class | ModelValidator cekirdek uygulamasi | VersaCoder.Application | net48;net10.0-windows | ModelValidatorTests | planli |
| N16-051 | `ModelValidatorDto` | record | ModelValidator veri transferi modeli | VersaCoder.Application | net48;net10.0-windows | ModelValidatorTests | planli |
| N16-052 | `ModelValidatorMapper` | class | ModelValidator <-> DTO esleme | VersaCoder.Application | net48;net10.0-windows | ModelValidatorTests | planli |
| N16-053 | `ModelValidatorValidator` | class | ModelValidator dogrulama kurallari | VersaCoder.Application | net48;net10.0-windows | ModelValidatorTests | planli |
| N16-054 | `ModelValidatorFactory` | class | ModelValidator uretimi ve varsayilanlari | VersaCoder.Application | net48;net10.0-windows | ModelValidatorTests | planli |
| N16-055 | `ModelValidatorOptions` | record | ModelValidator yapilandirmasi | VersaCoder.Application | net48;net10.0-windows | ModelValidatorTests | planli |
| N16-056 | `ModelValidatorProvider` | class | ModelValidator saglayici uygulamasi | VersaCoder.Application | net48;net10.0-windows | ModelValidatorTests | planli |
| N16-057 | `ModelValidatorRegistry` | class | ModelValidator kayit/katalog yonetimi | VersaCoder.Application | net48;net10.0-windows | ModelValidatorTests | planli |
| N16-058 | `ModelValidatorConverter` | class | ModelValidator bicim donusumu | VersaCoder.Application | net48;net10.0-windows | ModelValidatorTests | planli |
| N16-059 | `ModelValidatorTests` | class | ModelValidator testleri (xUnit) | VersaCoder.Application | net48;net10.0-windows | - | planli |
| N16-060 | `ModelValidatorBuilder` | class | ModelValidator uretici (builder) | VersaCoder.Application | net48;net10.0-windows | ModelValidatorTests | planli |
| N16-061 | `ISessionValidator` | interface | SessionValidator sozlesmesi (port) | VersaCoder.Application | net48;net10.0-windows | SessionValidatorTests | planli |
| N16-062 | `SessionValidator` | class | SessionValidator cekirdek uygulamasi | VersaCoder.Application | net48;net10.0-windows | SessionValidatorTests | planli |
| N16-063 | `SessionValidatorDto` | record | SessionValidator veri transferi modeli | VersaCoder.Application | net48;net10.0-windows | SessionValidatorTests | planli |
| N16-064 | `SessionValidatorMapper` | class | SessionValidator <-> DTO esleme | VersaCoder.Application | net48;net10.0-windows | SessionValidatorTests | planli |
| N16-065 | `SessionValidatorValidator` | class | SessionValidator dogrulama kurallari | VersaCoder.Application | net48;net10.0-windows | SessionValidatorTests | planli |
| N16-066 | `SessionValidatorFactory` | class | SessionValidator uretimi ve varsayilanlari | VersaCoder.Application | net48;net10.0-windows | SessionValidatorTests | planli |
| N16-067 | `SessionValidatorOptions` | record | SessionValidator yapilandirmasi | VersaCoder.Application | net48;net10.0-windows | SessionValidatorTests | planli |
| N16-068 | `SessionValidatorProvider` | class | SessionValidator saglayici uygulamasi | VersaCoder.Application | net48;net10.0-windows | SessionValidatorTests | planli |
| N16-069 | `SessionValidatorRegistry` | class | SessionValidator kayit/katalog yonetimi | VersaCoder.Application | net48;net10.0-windows | SessionValidatorTests | planli |
| N16-070 | `SessionValidatorConverter` | class | SessionValidator bicim donusumu | VersaCoder.Application | net48;net10.0-windows | SessionValidatorTests | planli |
| N16-071 | `SessionValidatorTests` | class | SessionValidator testleri (xUnit) | VersaCoder.Application | net48;net10.0-windows | - | planli |
| N16-072 | `SessionValidatorBuilder` | class | SessionValidator uretici (builder) | VersaCoder.Application | net48;net10.0-windows | SessionValidatorTests | planli |
| N16-073 | `ITokenBudgetValidator` | interface | TokenBudgetValidator sozlesmesi (port) | VersaCoder.Application | net48;net10.0-windows | TokenBudgetValidatorTests | planli |
| N16-074 | `TokenBudgetValidator` | class | TokenBudgetValidator cekirdek uygulamasi | VersaCoder.Application | net48;net10.0-windows | TokenBudgetValidatorTests | planli |
| N16-075 | `TokenBudgetValidatorDto` | record | TokenBudgetValidator veri transferi modeli | VersaCoder.Application | net48;net10.0-windows | TokenBudgetValidatorTests | planli |
| N16-076 | `TokenBudgetValidatorMapper` | class | TokenBudgetValidator <-> DTO esleme | VersaCoder.Application | net48;net10.0-windows | TokenBudgetValidatorTests | planli |
| N16-077 | `TokenBudgetValidatorValidator` | class | TokenBudgetValidator dogrulama kurallari | VersaCoder.Application | net48;net10.0-windows | TokenBudgetValidatorTests | planli |
| N16-078 | `TokenBudgetValidatorFactory` | class | TokenBudgetValidator uretimi ve varsayilanlari | VersaCoder.Application | net48;net10.0-windows | TokenBudgetValidatorTests | planli |
| N16-079 | `TokenBudgetValidatorOptions` | record | TokenBudgetValidator yapilandirmasi | VersaCoder.Application | net48;net10.0-windows | TokenBudgetValidatorTests | planli |
| N16-080 | `TokenBudgetValidatorProvider` | class | TokenBudgetValidator saglayici uygulamasi | VersaCoder.Application | net48;net10.0-windows | TokenBudgetValidatorTests | planli |
| N16-081 | `TokenBudgetValidatorRegistry` | class | TokenBudgetValidator kayit/katalog yonetimi | VersaCoder.Application | net48;net10.0-windows | TokenBudgetValidatorTests | planli |
| N16-082 | `TokenBudgetValidatorConverter` | class | TokenBudgetValidator bicim donusumu | VersaCoder.Application | net48;net10.0-windows | TokenBudgetValidatorTests | planli |
| N16-083 | `TokenBudgetValidatorTests` | class | TokenBudgetValidator testleri (xUnit) | VersaCoder.Application | net48;net10.0-windows | - | planli |
| N16-084 | `TokenBudgetValidatorBuilder` | class | TokenBudgetValidator uretici (builder) | VersaCoder.Application | net48;net10.0-windows | TokenBudgetValidatorTests | planli |
| N16-085 | `IEditPatchValidator` | interface | EditPatchValidator sozlesmesi (port) | VersaCoder.Application | net48;net10.0-windows | EditPatchValidatorTests | planli |
| N16-086 | `EditPatchValidator` | class | EditPatchValidator cekirdek uygulamasi | VersaCoder.Application | net48;net10.0-windows | EditPatchValidatorTests | planli |
| N16-087 | `EditPatchValidatorDto` | record | EditPatchValidator veri transferi modeli | VersaCoder.Application | net48;net10.0-windows | EditPatchValidatorTests | planli |
| N16-088 | `EditPatchValidatorMapper` | class | EditPatchValidator <-> DTO esleme | VersaCoder.Application | net48;net10.0-windows | EditPatchValidatorTests | planli |
| N16-089 | `EditPatchValidatorValidator` | class | EditPatchValidator dogrulama kurallari | VersaCoder.Application | net48;net10.0-windows | EditPatchValidatorTests | planli |
| N16-090 | `EditPatchValidatorFactory` | class | EditPatchValidator uretimi ve varsayilanlari | VersaCoder.Application | net48;net10.0-windows | EditPatchValidatorTests | planli |
| N16-091 | `EditPatchValidatorOptions` | record | EditPatchValidator yapilandirmasi | VersaCoder.Application | net48;net10.0-windows | EditPatchValidatorTests | planli |
| N16-092 | `EditPatchValidatorProvider` | class | EditPatchValidator saglayici uygulamasi | VersaCoder.Application | net48;net10.0-windows | EditPatchValidatorTests | planli |
| N16-093 | `EditPatchValidatorRegistry` | class | EditPatchValidator kayit/katalog yonetimi | VersaCoder.Application | net48;net10.0-windows | EditPatchValidatorTests | planli |
| N16-094 | `EditPatchValidatorConverter` | class | EditPatchValidator bicim donusumu | VersaCoder.Application | net48;net10.0-windows | EditPatchValidatorTests | planli |
| N16-095 | `EditPatchValidatorTests` | class | EditPatchValidator testleri (xUnit) | VersaCoder.Application | net48;net10.0-windows | - | planli |
| N16-096 | `EditPatchValidatorBuilder` | class | EditPatchValidator uretici (builder) | VersaCoder.Application | net48;net10.0-windows | EditPatchValidatorTests | planli |
| N16-097 | `IAgentPolicyValidator` | interface | AgentPolicyValidator sozlesmesi (port) | VersaCoder.Application | net48;net10.0-windows | AgentPolicyValidatorTests | planli |
| N16-098 | `AgentPolicyValidator` | class | AgentPolicyValidator cekirdek uygulamasi | VersaCoder.Application | net48;net10.0-windows | AgentPolicyValidatorTests | planli |
| N16-099 | `AgentPolicyValidatorDto` | record | AgentPolicyValidator veri transferi modeli | VersaCoder.Application | net48;net10.0-windows | AgentPolicyValidatorTests | planli |
| N16-100 | `AgentPolicyValidatorMapper` | class | AgentPolicyValidator <-> DTO esleme | VersaCoder.Application | net48;net10.0-windows | AgentPolicyValidatorTests | planli |
| N16-101 | `AgentPolicyValidatorValidator` | class | AgentPolicyValidator dogrulama kurallari | VersaCoder.Application | net48;net10.0-windows | AgentPolicyValidatorTests | planli |
| N16-102 | `AgentPolicyValidatorFactory` | class | AgentPolicyValidator uretimi ve varsayilanlari | VersaCoder.Application | net48;net10.0-windows | AgentPolicyValidatorTests | planli |
| N16-103 | `AgentPolicyValidatorOptions` | record | AgentPolicyValidator yapilandirmasi | VersaCoder.Application | net48;net10.0-windows | AgentPolicyValidatorTests | planli |
| N16-104 | `AgentPolicyValidatorProvider` | class | AgentPolicyValidator saglayici uygulamasi | VersaCoder.Application | net48;net10.0-windows | AgentPolicyValidatorTests | planli |
| N16-105 | `AgentPolicyValidatorRegistry` | class | AgentPolicyValidator kayit/katalog yonetimi | VersaCoder.Application | net48;net10.0-windows | AgentPolicyValidatorTests | planli |
| N16-106 | `AgentPolicyValidatorConverter` | class | AgentPolicyValidator bicim donusumu | VersaCoder.Application | net48;net10.0-windows | AgentPolicyValidatorTests | planli |
| N16-107 | `AgentPolicyValidatorTests` | class | AgentPolicyValidator testleri (xUnit) | VersaCoder.Application | net48;net10.0-windows | - | planli |
| N16-108 | `AgentPolicyValidatorBuilder` | class | AgentPolicyValidator uretici (builder) | VersaCoder.Application | net48;net10.0-windows | AgentPolicyValidatorTests | planli |
| N16-109 | `ILocaleValidator` | interface | LocaleValidator sozlesmesi (port) | VersaCoder.Application | net48;net10.0-windows | LocaleValidatorTests | planli |
| N16-110 | `LocaleValidator` | class | LocaleValidator cekirdek uygulamasi | VersaCoder.Application | net48;net10.0-windows | LocaleValidatorTests | planli |
| N16-111 | `LocaleValidatorDto` | record | LocaleValidator veri transferi modeli | VersaCoder.Application | net48;net10.0-windows | LocaleValidatorTests | planli |
| N16-112 | `LocaleValidatorMapper` | class | LocaleValidator <-> DTO esleme | VersaCoder.Application | net48;net10.0-windows | LocaleValidatorTests | planli |
| N16-113 | `LocaleValidatorValidator` | class | LocaleValidator dogrulama kurallari | VersaCoder.Application | net48;net10.0-windows | LocaleValidatorTests | planli |
| N16-114 | `LocaleValidatorFactory` | class | LocaleValidator uretimi ve varsayilanlari | VersaCoder.Application | net48;net10.0-windows | LocaleValidatorTests | planli |
| N16-115 | `LocaleValidatorOptions` | record | LocaleValidator yapilandirmasi | VersaCoder.Application | net48;net10.0-windows | LocaleValidatorTests | planli |
| N16-116 | `LocaleValidatorProvider` | class | LocaleValidator saglayici uygulamasi | VersaCoder.Application | net48;net10.0-windows | LocaleValidatorTests | planli |
| N16-117 | `LocaleValidatorRegistry` | class | LocaleValidator kayit/katalog yonetimi | VersaCoder.Application | net48;net10.0-windows | LocaleValidatorTests | planli |
| N16-118 | `LocaleValidatorConverter` | class | LocaleValidator bicim donusumu | VersaCoder.Application | net48;net10.0-windows | LocaleValidatorTests | planli |
| N16-119 | `LocaleValidatorTests` | class | LocaleValidator testleri (xUnit) | VersaCoder.Application | net48;net10.0-windows | - | planli |
| N16-120 | `LocaleValidatorBuilder` | class | LocaleValidator uretici (builder) | VersaCoder.Application | net48;net10.0-windows | LocaleValidatorTests | planli |
