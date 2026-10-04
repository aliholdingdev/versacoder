---
title: "Katalog Dugumu N10 - Invariantlar (l1)"
type: catalog
category: architecture
status: needs-validation
version: 1.0.0
updated: 2026-10-04
authority: derived
scope: "N10 invariants dugumu: kimlik, src klasoru, aile yapisi, ornek kayit ve yon kurali (ADR-013 + ADR-028)"
---

# Katalog Dugumu N10 - Invariantlar

**Zorunlu Baglantilar:** [catalog index](index.md) · [node-inventory](node-inventory.md) · [mimari index](../index.md) · [ADR-028](../../.decisions/accepted/ADR-028-layered-baseline-9-projects.md) · [ADR-013](../../.decisions/accepted/ADR-013-layered-architecture-52-nodes.md)

> **Tek cumlelik sorumluluk:** *N10 invariants hangi projede ve klasorde yasar, ne yapar, kac kayit hedefler* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

**N10 (invariants), l1 Domain katmaninin alt katmanidir: `VersaCoder.Domain` projesinde `src/VersaCoder.Domain/Invariants/` klasorunde yasar.** Ihlal edilemez kurallari (SessionInvariant, BudgetInvariant) kodlar. 10 aile x 12 rol = 120 kayit hedefi (ADR-013 asgari 112); durum `planli`.

Kayitlar ADR-013 2.1 biciminde 8 sutun tasir; bu dosya aile kalibini ve ornek satirlari gosterir, tam 120 satirlik liste bu dosyanin boyut sinirina sigmaz (DOĞRULAMA GEREKLI).

```text
+--------------------------------------------------+
| l1 Domain -> VersaCoder.Domain
| src/VersaCoder.Domain/Invariants/
| 10 aile x 12 rol = 120 kayit (esik 112)
+--------------------------------------------------+
```

| Alan | Deger |
|---|---|
| Dugum | N10 |
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
| 1 | Ihlal edilemez kurallari (SessionInvariant, BudgetInvariant) kodlar | System.Data, Dapper, dosya veya git cagirmak |
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
| 1 | N10 | invariants | `src/VersaCoder.Domain/Invariants/` |

**Kanit yolu (ADR-028 2.3/5):** tarama yolu gercek proje adini icerir; genis kalip (`src/**/Domain/**`) sahte PASS uretir.

```powershell
Test-Path 'C:\versacoder\src\VersaCoder.Domain\Invariants'
```

Dizin su an yok cikti `False` olur; PASS yazilmaz -> **DOĞRULAMA GEREKLI**.

---

## §3 Aile Yapisi

Rol kalibi (12): `I<F>` · `<F>` · `<F>Dto` · `<F>Mapper` · `<F>Validator` · `<F>Factory` · `<F>Options` · `<F>Provider` · `<F>Registry` · `<F>Converter` · `<F>Tests` · `<F>Builder`

| # | Aile | # | Aile |
|---|---|---|---|
| 1 | SessionInvariant | 6 | ModelInvariant |
| 2 | BudgetInvariant | 7 | VersionInvariant |
| 3 | ToolPermissionInvariant | 8 | RetryInvariant |
| 4 | MessageOrderInvariant | 9 | AuditInvariant |
| 5 | FilePathInvariant | 10 | LocaleInvariant |

Her aile ayni rol kalibini 12 kez tekrarlar; kayit sayisi = aile x rol.

---

## §4 Kayit Ornegi (8 sutun)

| ID | Bilesen | Tip | Sorumluluk | Proje | TFM | Test | Durum |
|---|---|---|---|---|---|---|---|
| N10-001 | `ISessionInvariant` | interface | SessionInvariant sozlesmesi (port) | VersaCoder.Domain | net48;net10.0-windows | SessionInvariantTests | planli |
| N10-002 | `SessionInvariant` | class | SessionInvariant cekirdek uygulamasi | VersaCoder.Domain | net48;net10.0-windows | SessionInvariantTests | planli |
| N10-003 | `SessionInvariantDto` | record | SessionInvariant veri transferi modeli | VersaCoder.Domain | net48;net10.0-windows | SessionInvariantTests | planli |
| N10-004 | `SessionInvariantMapper` | class | SessionInvariant <-> DTO esleme | VersaCoder.Domain | net48;net10.0-windows | SessionInvariantTests | planli |
| N10-005 | `SessionInvariantValidator` | class | SessionInvariant dogrulama kurallari | VersaCoder.Domain | net48;net10.0-windows | SessionInvariantTests | planli |
| N10-006 | `SessionInvariantFactory` | class | SessionInvariant uretimi ve varsayilanlari | VersaCoder.Domain | net48;net10.0-windows | SessionInvariantTests | planli |

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
| 3 | Test sinifi | SessionInvariantTests (xUnit) | planli |
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
(Select-String -Path 'C:\versacoder\.ai\architecture\catalog\N10-invariants.md' -Pattern '^\|\s*N\d{2}-\d{3}\s*\|').Count
# 2) boyut bandi - beklenen: 2600-8500
(Get-Item 'C:\versacoder\.ai\architecture\catalog\N10-invariants.md').Length
# 3) klasor kaniti - gercek proje adi (ADR-028 2.3/5)
(Test-Path 'C:\versacoder\src\VersaCoder.Domain\Invariants')
```

Katalog sayimi `powershell -NoProfile -ExecutionPolicy Bypass -File C:\versacoder\.ai\scripts\count-components.ps1` ile alinir; bu batch sonunda toplam esik (5000) henuz karsilanmaz - kalan 26 node dosyasi sonraki batchtir.

## §10 Kayit Tablosu (ADR-013 2.2 - kaynak vault port)

| N10-007 | `SessionInvariantOptions` | record | SessionInvariant yapilandirmasi | VersaCoder.Domain | net48;net10.0-windows | SessionInvariantTests | planli |
| N10-008 | `SessionInvariantProvider` | class | SessionInvariant saglayici uygulamasi | VersaCoder.Domain | net48;net10.0-windows | SessionInvariantTests | planli |
| N10-009 | `SessionInvariantRegistry` | class | SessionInvariant kayit/katalog yonetimi | VersaCoder.Domain | net48;net10.0-windows | SessionInvariantTests | planli |
| N10-010 | `SessionInvariantConverter` | class | SessionInvariant bicim donusumu | VersaCoder.Domain | net48;net10.0-windows | SessionInvariantTests | planli |
| N10-011 | `SessionInvariantTests` | class | SessionInvariant testleri (xUnit) | VersaCoder.Domain | net48;net10.0-windows | - | planli |
| N10-012 | `SessionInvariantBuilder` | class | SessionInvariant uretici (builder) | VersaCoder.Domain | net48;net10.0-windows | SessionInvariantTests | planli |
| N10-013 | `IBudgetInvariant` | interface | BudgetInvariant sozlesmesi (port) | VersaCoder.Domain | net48;net10.0-windows | BudgetInvariantTests | planli |
| N10-014 | `BudgetInvariant` | class | BudgetInvariant cekirdek uygulamasi | VersaCoder.Domain | net48;net10.0-windows | BudgetInvariantTests | planli |
| N10-015 | `BudgetInvariantDto` | record | BudgetInvariant veri transferi modeli | VersaCoder.Domain | net48;net10.0-windows | BudgetInvariantTests | planli |
| N10-016 | `BudgetInvariantMapper` | class | BudgetInvariant <-> DTO esleme | VersaCoder.Domain | net48;net10.0-windows | BudgetInvariantTests | planli |
| N10-017 | `BudgetInvariantValidator` | class | BudgetInvariant dogrulama kurallari | VersaCoder.Domain | net48;net10.0-windows | BudgetInvariantTests | planli |
| N10-018 | `BudgetInvariantFactory` | class | BudgetInvariant uretimi ve varsayilanlari | VersaCoder.Domain | net48;net10.0-windows | BudgetInvariantTests | planli |
| N10-019 | `BudgetInvariantOptions` | record | BudgetInvariant yapilandirmasi | VersaCoder.Domain | net48;net10.0-windows | BudgetInvariantTests | planli |
| N10-020 | `BudgetInvariantProvider` | class | BudgetInvariant saglayici uygulamasi | VersaCoder.Domain | net48;net10.0-windows | BudgetInvariantTests | planli |
| N10-021 | `BudgetInvariantRegistry` | class | BudgetInvariant kayit/katalog yonetimi | VersaCoder.Domain | net48;net10.0-windows | BudgetInvariantTests | planli |
| N10-022 | `BudgetInvariantConverter` | class | BudgetInvariant bicim donusumu | VersaCoder.Domain | net48;net10.0-windows | BudgetInvariantTests | planli |
| N10-023 | `BudgetInvariantTests` | class | BudgetInvariant testleri (xUnit) | VersaCoder.Domain | net48;net10.0-windows | - | planli |
| N10-024 | `BudgetInvariantBuilder` | class | BudgetInvariant uretici (builder) | VersaCoder.Domain | net48;net10.0-windows | BudgetInvariantTests | planli |
| N10-025 | `IToolPermissionInvariant` | interface | ToolPermissionInvariant sozlesmesi (port) | VersaCoder.Domain | net48;net10.0-windows | ToolPermissionInvariantTests | planli |
| N10-026 | `ToolPermissionInvariant` | class | ToolPermissionInvariant cekirdek uygulamasi | VersaCoder.Domain | net48;net10.0-windows | ToolPermissionInvariantTests | planli |
| N10-027 | `ToolPermissionInvariantDto` | record | ToolPermissionInvariant veri transferi modeli | VersaCoder.Domain | net48;net10.0-windows | ToolPermissionInvariantTests | planli |
| N10-028 | `ToolPermissionInvariantMapper` | class | ToolPermissionInvariant <-> DTO esleme | VersaCoder.Domain | net48;net10.0-windows | ToolPermissionInvariantTests | planli |
| N10-029 | `ToolPermissionInvariantValidator` | class | ToolPermissionInvariant dogrulama kurallari | VersaCoder.Domain | net48;net10.0-windows | ToolPermissionInvariantTests | planli |
| N10-030 | `ToolPermissionInvariantFactory` | class | ToolPermissionInvariant uretimi ve varsayilanlari | VersaCoder.Domain | net48;net10.0-windows | ToolPermissionInvariantTests | planli |
| N10-031 | `ToolPermissionInvariantOptions` | record | ToolPermissionInvariant yapilandirmasi | VersaCoder.Domain | net48;net10.0-windows | ToolPermissionInvariantTests | planli |
| N10-032 | `ToolPermissionInvariantProvider` | class | ToolPermissionInvariant saglayici uygulamasi | VersaCoder.Domain | net48;net10.0-windows | ToolPermissionInvariantTests | planli |
| N10-033 | `ToolPermissionInvariantRegistry` | class | ToolPermissionInvariant kayit/katalog yonetimi | VersaCoder.Domain | net48;net10.0-windows | ToolPermissionInvariantTests | planli |
| N10-034 | `ToolPermissionInvariantConverter` | class | ToolPermissionInvariant bicim donusumu | VersaCoder.Domain | net48;net10.0-windows | ToolPermissionInvariantTests | planli |
| N10-035 | `ToolPermissionInvariantTests` | class | ToolPermissionInvariant testleri (xUnit) | VersaCoder.Domain | net48;net10.0-windows | - | planli |
| N10-036 | `ToolPermissionInvariantBuilder` | class | ToolPermissionInvariant uretici (builder) | VersaCoder.Domain | net48;net10.0-windows | ToolPermissionInvariantTests | planli |
| N10-037 | `IMessageOrderInvariant` | interface | MessageOrderInvariant sozlesmesi (port) | VersaCoder.Domain | net48;net10.0-windows | MessageOrderInvariantTests | planli |
| N10-038 | `MessageOrderInvariant` | class | MessageOrderInvariant cekirdek uygulamasi | VersaCoder.Domain | net48;net10.0-windows | MessageOrderInvariantTests | planli |
| N10-039 | `MessageOrderInvariantDto` | record | MessageOrderInvariant veri transferi modeli | VersaCoder.Domain | net48;net10.0-windows | MessageOrderInvariantTests | planli |
| N10-040 | `MessageOrderInvariantMapper` | class | MessageOrderInvariant <-> DTO esleme | VersaCoder.Domain | net48;net10.0-windows | MessageOrderInvariantTests | planli |
| N10-041 | `MessageOrderInvariantValidator` | class | MessageOrderInvariant dogrulama kurallari | VersaCoder.Domain | net48;net10.0-windows | MessageOrderInvariantTests | planli |
| N10-042 | `MessageOrderInvariantFactory` | class | MessageOrderInvariant uretimi ve varsayilanlari | VersaCoder.Domain | net48;net10.0-windows | MessageOrderInvariantTests | planli |
| N10-043 | `MessageOrderInvariantOptions` | record | MessageOrderInvariant yapilandirmasi | VersaCoder.Domain | net48;net10.0-windows | MessageOrderInvariantTests | planli |
| N10-044 | `MessageOrderInvariantProvider` | class | MessageOrderInvariant saglayici uygulamasi | VersaCoder.Domain | net48;net10.0-windows | MessageOrderInvariantTests | planli |
| N10-045 | `MessageOrderInvariantRegistry` | class | MessageOrderInvariant kayit/katalog yonetimi | VersaCoder.Domain | net48;net10.0-windows | MessageOrderInvariantTests | planli |
| N10-046 | `MessageOrderInvariantConverter` | class | MessageOrderInvariant bicim donusumu | VersaCoder.Domain | net48;net10.0-windows | MessageOrderInvariantTests | planli |
| N10-047 | `MessageOrderInvariantTests` | class | MessageOrderInvariant testleri (xUnit) | VersaCoder.Domain | net48;net10.0-windows | - | planli |
| N10-048 | `MessageOrderInvariantBuilder` | class | MessageOrderInvariant uretici (builder) | VersaCoder.Domain | net48;net10.0-windows | MessageOrderInvariantTests | planli |
| N10-049 | `IFilePathInvariant` | interface | FilePathInvariant sozlesmesi (port) | VersaCoder.Domain | net48;net10.0-windows | FilePathInvariantTests | planli |
| N10-050 | `FilePathInvariant` | class | FilePathInvariant cekirdek uygulamasi | VersaCoder.Domain | net48;net10.0-windows | FilePathInvariantTests | planli |
| N10-051 | `FilePathInvariantDto` | record | FilePathInvariant veri transferi modeli | VersaCoder.Domain | net48;net10.0-windows | FilePathInvariantTests | planli |
| N10-052 | `FilePathInvariantMapper` | class | FilePathInvariant <-> DTO esleme | VersaCoder.Domain | net48;net10.0-windows | FilePathInvariantTests | planli |
| N10-053 | `FilePathInvariantValidator` | class | FilePathInvariant dogrulama kurallari | VersaCoder.Domain | net48;net10.0-windows | FilePathInvariantTests | planli |
| N10-054 | `FilePathInvariantFactory` | class | FilePathInvariant uretimi ve varsayilanlari | VersaCoder.Domain | net48;net10.0-windows | FilePathInvariantTests | planli |
| N10-055 | `FilePathInvariantOptions` | record | FilePathInvariant yapilandirmasi | VersaCoder.Domain | net48;net10.0-windows | FilePathInvariantTests | planli |
| N10-056 | `FilePathInvariantProvider` | class | FilePathInvariant saglayici uygulamasi | VersaCoder.Domain | net48;net10.0-windows | FilePathInvariantTests | planli |
| N10-057 | `FilePathInvariantRegistry` | class | FilePathInvariant kayit/katalog yonetimi | VersaCoder.Domain | net48;net10.0-windows | FilePathInvariantTests | planli |
| N10-058 | `FilePathInvariantConverter` | class | FilePathInvariant bicim donusumu | VersaCoder.Domain | net48;net10.0-windows | FilePathInvariantTests | planli |
| N10-059 | `FilePathInvariantTests` | class | FilePathInvariant testleri (xUnit) | VersaCoder.Domain | net48;net10.0-windows | - | planli |
| N10-060 | `FilePathInvariantBuilder` | class | FilePathInvariant uretici (builder) | VersaCoder.Domain | net48;net10.0-windows | FilePathInvariantTests | planli |
| N10-061 | `IModelInvariant` | interface | ModelInvariant sozlesmesi (port) | VersaCoder.Domain | net48;net10.0-windows | ModelInvariantTests | planli |
| N10-062 | `ModelInvariant` | class | ModelInvariant cekirdek uygulamasi | VersaCoder.Domain | net48;net10.0-windows | ModelInvariantTests | planli |
| N10-063 | `ModelInvariantDto` | record | ModelInvariant veri transferi modeli | VersaCoder.Domain | net48;net10.0-windows | ModelInvariantTests | planli |
| N10-064 | `ModelInvariantMapper` | class | ModelInvariant <-> DTO esleme | VersaCoder.Domain | net48;net10.0-windows | ModelInvariantTests | planli |
| N10-065 | `ModelInvariantValidator` | class | ModelInvariant dogrulama kurallari | VersaCoder.Domain | net48;net10.0-windows | ModelInvariantTests | planli |
| N10-066 | `ModelInvariantFactory` | class | ModelInvariant uretimi ve varsayilanlari | VersaCoder.Domain | net48;net10.0-windows | ModelInvariantTests | planli |
| N10-067 | `ModelInvariantOptions` | record | ModelInvariant yapilandirmasi | VersaCoder.Domain | net48;net10.0-windows | ModelInvariantTests | planli |
| N10-068 | `ModelInvariantProvider` | class | ModelInvariant saglayici uygulamasi | VersaCoder.Domain | net48;net10.0-windows | ModelInvariantTests | planli |
| N10-069 | `ModelInvariantRegistry` | class | ModelInvariant kayit/katalog yonetimi | VersaCoder.Domain | net48;net10.0-windows | ModelInvariantTests | planli |
| N10-070 | `ModelInvariantConverter` | class | ModelInvariant bicim donusumu | VersaCoder.Domain | net48;net10.0-windows | ModelInvariantTests | planli |
| N10-071 | `ModelInvariantTests` | class | ModelInvariant testleri (xUnit) | VersaCoder.Domain | net48;net10.0-windows | - | planli |
| N10-072 | `ModelInvariantBuilder` | class | ModelInvariant uretici (builder) | VersaCoder.Domain | net48;net10.0-windows | ModelInvariantTests | planli |
| N10-073 | `IVersionInvariant` | interface | VersionInvariant sozlesmesi (port) | VersaCoder.Domain | net48;net10.0-windows | VersionInvariantTests | planli |
| N10-074 | `VersionInvariant` | class | VersionInvariant cekirdek uygulamasi | VersaCoder.Domain | net48;net10.0-windows | VersionInvariantTests | planli |
| N10-075 | `VersionInvariantDto` | record | VersionInvariant veri transferi modeli | VersaCoder.Domain | net48;net10.0-windows | VersionInvariantTests | planli |
| N10-076 | `VersionInvariantMapper` | class | VersionInvariant <-> DTO esleme | VersaCoder.Domain | net48;net10.0-windows | VersionInvariantTests | planli |
| N10-077 | `VersionInvariantValidator` | class | VersionInvariant dogrulama kurallari | VersaCoder.Domain | net48;net10.0-windows | VersionInvariantTests | planli |
| N10-078 | `VersionInvariantFactory` | class | VersionInvariant uretimi ve varsayilanlari | VersaCoder.Domain | net48;net10.0-windows | VersionInvariantTests | planli |
| N10-079 | `VersionInvariantOptions` | record | VersionInvariant yapilandirmasi | VersaCoder.Domain | net48;net10.0-windows | VersionInvariantTests | planli |
| N10-080 | `VersionInvariantProvider` | class | VersionInvariant saglayici uygulamasi | VersaCoder.Domain | net48;net10.0-windows | VersionInvariantTests | planli |
| N10-081 | `VersionInvariantRegistry` | class | VersionInvariant kayit/katalog yonetimi | VersaCoder.Domain | net48;net10.0-windows | VersionInvariantTests | planli |
| N10-082 | `VersionInvariantConverter` | class | VersionInvariant bicim donusumu | VersaCoder.Domain | net48;net10.0-windows | VersionInvariantTests | planli |
| N10-083 | `VersionInvariantTests` | class | VersionInvariant testleri (xUnit) | VersaCoder.Domain | net48;net10.0-windows | - | planli |
| N10-084 | `VersionInvariantBuilder` | class | VersionInvariant uretici (builder) | VersaCoder.Domain | net48;net10.0-windows | VersionInvariantTests | planli |
| N10-085 | `IRetryInvariant` | interface | RetryInvariant sozlesmesi (port) | VersaCoder.Domain | net48;net10.0-windows | RetryInvariantTests | planli |
| N10-086 | `RetryInvariant` | class | RetryInvariant cekirdek uygulamasi | VersaCoder.Domain | net48;net10.0-windows | RetryInvariantTests | planli |
| N10-087 | `RetryInvariantDto` | record | RetryInvariant veri transferi modeli | VersaCoder.Domain | net48;net10.0-windows | RetryInvariantTests | planli |
| N10-088 | `RetryInvariantMapper` | class | RetryInvariant <-> DTO esleme | VersaCoder.Domain | net48;net10.0-windows | RetryInvariantTests | planli |
| N10-089 | `RetryInvariantValidator` | class | RetryInvariant dogrulama kurallari | VersaCoder.Domain | net48;net10.0-windows | RetryInvariantTests | planli |
| N10-090 | `RetryInvariantFactory` | class | RetryInvariant uretimi ve varsayilanlari | VersaCoder.Domain | net48;net10.0-windows | RetryInvariantTests | planli |
| N10-091 | `RetryInvariantOptions` | record | RetryInvariant yapilandirmasi | VersaCoder.Domain | net48;net10.0-windows | RetryInvariantTests | planli |
| N10-092 | `RetryInvariantProvider` | class | RetryInvariant saglayici uygulamasi | VersaCoder.Domain | net48;net10.0-windows | RetryInvariantTests | planli |
| N10-093 | `RetryInvariantRegistry` | class | RetryInvariant kayit/katalog yonetimi | VersaCoder.Domain | net48;net10.0-windows | RetryInvariantTests | planli |
| N10-094 | `RetryInvariantConverter` | class | RetryInvariant bicim donusumu | VersaCoder.Domain | net48;net10.0-windows | RetryInvariantTests | planli |
| N10-095 | `RetryInvariantTests` | class | RetryInvariant testleri (xUnit) | VersaCoder.Domain | net48;net10.0-windows | - | planli |
| N10-096 | `RetryInvariantBuilder` | class | RetryInvariant uretici (builder) | VersaCoder.Domain | net48;net10.0-windows | RetryInvariantTests | planli |
| N10-097 | `IAuditInvariant` | interface | AuditInvariant sozlesmesi (port) | VersaCoder.Domain | net48;net10.0-windows | AuditInvariantTests | planli |
| N10-098 | `AuditInvariant` | class | AuditInvariant cekirdek uygulamasi | VersaCoder.Domain | net48;net10.0-windows | AuditInvariantTests | planli |
| N10-099 | `AuditInvariantDto` | record | AuditInvariant veri transferi modeli | VersaCoder.Domain | net48;net10.0-windows | AuditInvariantTests | planli |
| N10-100 | `AuditInvariantMapper` | class | AuditInvariant <-> DTO esleme | VersaCoder.Domain | net48;net10.0-windows | AuditInvariantTests | planli |
| N10-101 | `AuditInvariantValidator` | class | AuditInvariant dogrulama kurallari | VersaCoder.Domain | net48;net10.0-windows | AuditInvariantTests | planli |
| N10-102 | `AuditInvariantFactory` | class | AuditInvariant uretimi ve varsayilanlari | VersaCoder.Domain | net48;net10.0-windows | AuditInvariantTests | planli |
| N10-103 | `AuditInvariantOptions` | record | AuditInvariant yapilandirmasi | VersaCoder.Domain | net48;net10.0-windows | AuditInvariantTests | planli |
| N10-104 | `AuditInvariantProvider` | class | AuditInvariant saglayici uygulamasi | VersaCoder.Domain | net48;net10.0-windows | AuditInvariantTests | planli |
| N10-105 | `AuditInvariantRegistry` | class | AuditInvariant kayit/katalog yonetimi | VersaCoder.Domain | net48;net10.0-windows | AuditInvariantTests | planli |
| N10-106 | `AuditInvariantConverter` | class | AuditInvariant bicim donusumu | VersaCoder.Domain | net48;net10.0-windows | AuditInvariantTests | planli |
| N10-107 | `AuditInvariantTests` | class | AuditInvariant testleri (xUnit) | VersaCoder.Domain | net48;net10.0-windows | - | planli |
| N10-108 | `AuditInvariantBuilder` | class | AuditInvariant uretici (builder) | VersaCoder.Domain | net48;net10.0-windows | AuditInvariantTests | planli |
| N10-109 | `ILocaleInvariant` | interface | LocaleInvariant sozlesmesi (port) | VersaCoder.Domain | net48;net10.0-windows | LocaleInvariantTests | planli |
| N10-110 | `LocaleInvariant` | class | LocaleInvariant cekirdek uygulamasi | VersaCoder.Domain | net48;net10.0-windows | LocaleInvariantTests | planli |
| N10-111 | `LocaleInvariantDto` | record | LocaleInvariant veri transferi modeli | VersaCoder.Domain | net48;net10.0-windows | LocaleInvariantTests | planli |
| N10-112 | `LocaleInvariantMapper` | class | LocaleInvariant <-> DTO esleme | VersaCoder.Domain | net48;net10.0-windows | LocaleInvariantTests | planli |
| N10-113 | `LocaleInvariantValidator` | class | LocaleInvariant dogrulama kurallari | VersaCoder.Domain | net48;net10.0-windows | LocaleInvariantTests | planli |
| N10-114 | `LocaleInvariantFactory` | class | LocaleInvariant uretimi ve varsayilanlari | VersaCoder.Domain | net48;net10.0-windows | LocaleInvariantTests | planli |
| N10-115 | `LocaleInvariantOptions` | record | LocaleInvariant yapilandirmasi | VersaCoder.Domain | net48;net10.0-windows | LocaleInvariantTests | planli |
| N10-116 | `LocaleInvariantProvider` | class | LocaleInvariant saglayici uygulamasi | VersaCoder.Domain | net48;net10.0-windows | LocaleInvariantTests | planli |
| N10-117 | `LocaleInvariantRegistry` | class | LocaleInvariant kayit/katalog yonetimi | VersaCoder.Domain | net48;net10.0-windows | LocaleInvariantTests | planli |
| N10-118 | `LocaleInvariantConverter` | class | LocaleInvariant bicim donusumu | VersaCoder.Domain | net48;net10.0-windows | LocaleInvariantTests | planli |
| N10-119 | `LocaleInvariantTests` | class | LocaleInvariant testleri (xUnit) | VersaCoder.Domain | net48;net10.0-windows | - | planli |
| N10-120 | `LocaleInvariantBuilder` | class | LocaleInvariant uretici (builder) | VersaCoder.Domain | net48;net10.0-windows | LocaleInvariantTests | planli |
