---
title: "Katalog Dugumu N47 - Makro l2 Application (l2)"
type: catalog
category: architecture
status: needs-validation
version: 1.0.0
updated: 2026-10-04
authority: derived
scope: "N47 macro-l2 dugumu: kimlik, src klasoru, aile yapisi, ornek kayit ve yon kurali (ADR-013 + ADR-028)"
---

# Katalog Dugumu N47 - Makro l2 Application

**Zorunlu Baglantilar:** [catalog index](index.md) · [node-inventory](node-inventory.md) · [mimari index](../index.md) · [ADR-028](../../.decisions/accepted/ADR-028-layered-baseline-9-projects.md) · [ADR-013](../../.decisions/accepted/ADR-013-layered-architecture-52-nodes.md)

> **Tek cumlelik sorumluluk:** *N47 macro-l2 hangi projede ve klasorde yasar, ne yapar, kac kayit hedefler* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

**N47 (macro-l2), l2 Application katmaninin makro ozet dugumudur: `VersaCoder.Application` proje kokunde yasar.** l2 katmaninin makro ozetini tasir: ApplicationCore, CommandFlow, QueryFlow, UseCaseFlow, ValidationFlow. 5 aile x 4 rol = 20 kayit hedefi (ADR-013 asgari 20); durum `planli`.

Makro dugumler alt katman dosyalarini tekrarlamaz, katman ozetini tasir; kayitlar ADR-013 2.1 biciminde 8 sutundur. Bu dosya `authority: derived` + `status: needs-validation` icerir: klasor ve aile listesi `src/` olusunca dogrulanir.

```text
+--------------------------------------------------+
| l2 Application -> VersaCoder.Application
| src/VersaCoder.Application/
| 5 aile x 4 rol = 20 kayit (esik 20)
+--------------------------------------------------+
```

| Alan | Deger |
|---|---|
| Dugum | N47 |
| Dugum turu | makro ozet dugumu |
| Makro katman | l2 Application |
| Proje (.csproj) | VersaCoder.Application |
| TFM | net48;net10.0-windows |
| Kayit hedefi | 20 (ADR-013 asgari 20) |
| Durum | planli |
| Sorumlu rol | architect |

---

## §1 Sorumluluk (YAPAR / YAPMAZ)

| # | YAPAR | YAPMAZ |
|---|---|---|
| 1 | l2 katmaninin makro ozetini tasir: ApplicationCore, CommandFlow, QueryFlow, UseCaseFlow, ValidationFlow | l4 katmanina dogrudan bakmak (repository portu l0 katmaninda, uygulamasi l4 uzerinde) - YOK |
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
| 1 | N47 | macro-l2 | `src/VersaCoder.Application/` |

**Kanit yolu (ADR-028 2.3/5):** tarama yolu gercek proje adini icerir; genis kalip (`src/**/Domain/**`) sahte PASS uretir.

```powershell
Test-Path 'C:\versacoder\src\VersaCoder.Application'
```

Dizin su an yok cikti `False` olur; PASS yazilmaz -> **DOĞRULAMA GEREKLI**.

---

## §3 Aile Yapisi

Rol kalibi (4): `I<F>` · `<F>Module` · `<F>Descriptor` · `<F>Tests`

| # | Aile |
|---|---|
| 1 | ApplicationCore |
| 2 | CommandFlow |
| 3 | QueryFlow |
| 4 | UseCaseFlow |
| 5 | ValidationFlow |

Her aile ayni rol kalibini 4 kez tekrarlar; kayit sayisi = aile x rol.

---

## §4 Kayit Ornegi (8 sutun)

| ID | Bilesen | Tip | Sorumluluk | Proje | TFM | Test | Durum |
|---|---|---|---|---|---|---|---|
| N47-001 | `ApplicationCore` | interface | ApplicationCore katman sozlesmesi | VersaCoder.Application | net48;net10.0-windows | ApplicationCoreTests | planli |
| N47-002 | `ApplicationCoreModule` | class | ApplicationCore modul kaydi | VersaCoder.Application | net48;net10.0-windows | ApplicationCoreTests | planli |
| N47-003 | `ApplicationCoreDescriptor` | record | ApplicationCore katman tanimi kaydi | VersaCoder.Application | net48;net10.0-windows | ApplicationCoreTests | planli |
| N47-004 | `ApplicationCoreTests` | class | ApplicationCore mimari testleri | VersaCoder.Application | net48;net10.0-windows | - | planli |

Kalan aileler ayni kalibi 12 rolle tekrarlar (makro dugumlerde 4 rol); toplam hedef **20** kayit.

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
| 1 | Kayit satiri | 20 (asgari 20) | bu dosyada 4 ornek satir; tam liste DOĞRULAMA GEREKLI |
| 2 | Durum degeri | planli -> yapildi -> testli | planli |
| 3 | Test sinifi | ApplicationCoreTests (xUnit) | planli |
| 4 | NetArchTest yon denetimi | izinsiz kenar = 0 | planli (LayerRulesTests) |
| 5 | Dosya boyutu | 3-8 KB (2600-8500 bayt) | §9 olcumu |

---

## §7 Kapsam Disi

- K3 katmaninin 618 DLL envanteri bu dugume girmez; tek iz `reports\k3-618-ham-envanter.md` dosyasidir (ADR-028 2.2).
- Tam kayit listesi (20 satir) bu dosyaya sasmaz; ornek satirlar aile kalibini gosterir -> **DOĞRULAMA GEREKLI**.
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
`# 1) kayit sayimi (ADR-013 2.2) - beklenen: 20
(Select-String -Path 'C:\versacoder\.ai\architecture\catalog\N47-macro-l2.md' -Pattern '^\|\s*N\d{2}-\d{3}\s*\|').Count
# 2) boyut bandi - beklenen: 2600-8500
(Get-Item 'C:\versacoder\.ai\architecture\catalog\N47-macro-l2.md').Length
# 3) klasor kaniti - gercek proje adi (ADR-028 2.3/5)
(Test-Path 'C:\versacoder\src\VersaCoder.Application')
```

Katalog sayimi `powershell -NoProfile -ExecutionPolicy Bypass -File C:\versacoder\.ai\scripts\count-components.ps1` ile alinir; bu batch sonunda toplam esik (5000) henuz karsilanmaz - kalan 26 node dosyasi sonraki batchtir.

## §10 Kayit Tablosu (ADR-013 2.2 - kaynak vault port)

| N47-005 | `ICommandFlow` | interface | CommandFlow katman sozlesmesi | VersaCoder.Application | net48;net10.0-windows | CommandFlowTests | planli |
| N47-006 | `CommandFlowModule` | class | CommandFlow modul kaydi | VersaCoder.Application | net48;net10.0-windows | CommandFlowTests | planli |
| N47-007 | `CommandFlowDescriptor` | record | CommandFlow katman tanimi kaydi | VersaCoder.Application | net48;net10.0-windows | CommandFlowTests | planli |
| N47-008 | `CommandFlowTests` | class | CommandFlow mimari testleri | VersaCoder.Application | net48;net10.0-windows | - | planli |
| N47-009 | `IQueryFlow` | interface | QueryFlow katman sozlesmesi | VersaCoder.Application | net48;net10.0-windows | QueryFlowTests | planli |
| N47-010 | `QueryFlowModule` | class | QueryFlow modul kaydi | VersaCoder.Application | net48;net10.0-windows | QueryFlowTests | planli |
| N47-011 | `QueryFlowDescriptor` | record | QueryFlow katman tanimi kaydi | VersaCoder.Application | net48;net10.0-windows | QueryFlowTests | planli |
| N47-012 | `QueryFlowTests` | class | QueryFlow mimari testleri | VersaCoder.Application | net48;net10.0-windows | - | planli |
| N47-013 | `IUseCaseFlow` | interface | UseCaseFlow katman sozlesmesi | VersaCoder.Application | net48;net10.0-windows | UseCaseFlowTests | planli |
| N47-014 | `UseCaseFlowModule` | class | UseCaseFlow modul kaydi | VersaCoder.Application | net48;net10.0-windows | UseCaseFlowTests | planli |
| N47-015 | `UseCaseFlowDescriptor` | record | UseCaseFlow katman tanimi kaydi | VersaCoder.Application | net48;net10.0-windows | UseCaseFlowTests | planli |
| N47-016 | `UseCaseFlowTests` | class | UseCaseFlow mimari testleri | VersaCoder.Application | net48;net10.0-windows | - | planli |
| N47-017 | `IValidationFlow` | interface | ValidationFlow katman sozlesmesi | VersaCoder.Application | net48;net10.0-windows | ValidationFlowTests | planli |
| N47-018 | `ValidationFlowModule` | class | ValidationFlow modul kaydi | VersaCoder.Application | net48;net10.0-windows | ValidationFlowTests | planli |
| N47-019 | `ValidationFlowDescriptor` | record | ValidationFlow katman tanimi kaydi | VersaCoder.Application | net48;net10.0-windows | ValidationFlowTests | planli |
| N47-020 | `ValidationFlowTests` | class | ValidationFlow mimari testleri | VersaCoder.Application | net48;net10.0-windows | - | planli |
