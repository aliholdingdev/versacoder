---
title: "Katalog Dugumu N46 - Makro l1 Domain (l1)"
type: catalog
category: architecture
status: needs-validation
version: 1.0.0
updated: 2026-10-04
authority: derived
scope: "N46 macro-l1 dugumu: kimlik, src klasoru, aile yapisi, ornek kayit ve yon kurali (ADR-013 + ADR-028)"
---

# Katalog Dugumu N46 - Makro l1 Domain

**Zorunlu Baglantilar:** [catalog index](index.md) · [node-inventory](node-inventory.md) · [mimari index](../index.md) · [ADR-028](../../.decisions/accepted/ADR-028-layered-baseline-9-projects.md) · [ADR-013](../../.decisions/accepted/ADR-013-layered-architecture-52-nodes.md)

> **Tek cumlelik sorumluluk:** *N46 macro-l1 hangi projede ve klasorde yasar, ne yapar, kac kayit hedefler* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

**N46 (macro-l1), l1 Domain katmaninin makro ozet dugumudur: `VersaCoder.Domain` proje kokunde yasar.** l1 katmaninin makro ozetini tasir: DomainCore, EntityModel, ValueObjectModel, AggregateModel, InvariantModel. 5 aile x 4 rol = 20 kayit hedefi (ADR-013 asgari 20); durum `planli`.

Makro dugumler alt katman dosyalarini tekrarlamaz, katman ozetini tasir; kayitlar ADR-013 2.1 biciminde 8 sutundur. Bu dosya `authority: derived` + `status: needs-validation` icerir: klasor ve aile listesi `src/` olusunca dogrulanir.

```text
+--------------------------------------------------+
| l1 Domain -> VersaCoder.Domain
| src/VersaCoder.Domain/
| 5 aile x 4 rol = 20 kayit (esik 20)
+--------------------------------------------------+
```

| Alan | Deger |
|---|---|
| Dugum | N46 |
| Dugum turu | makro ozet dugumu |
| Makro katman | l1 Domain |
| Proje (.csproj) | VersaCoder.Domain |
| TFM | net48;net10.0-windows |
| Kayit hedefi | 20 (ADR-013 asgari 20) |
| Durum | planli |
| Sorumlu rol | architect |

---

## §1 Sorumluluk (YAPAR / YAPMAZ)

| # | YAPAR | YAPMAZ |
|---|---|---|
| 1 | l1 katmaninin makro ozetini tasir: DomainCore, EntityModel, ValueObjectModel, AggregateModel, InvariantModel | System.Data, Dapper, dosya veya git cagirmak |
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
| 1 | N46 | macro-l1 | `src/VersaCoder.Domain/` |

**Kanit yolu (ADR-028 2.3/5):** tarama yolu gercek proje adini icerir; genis kalip (`src/**/Domain/**`) sahte PASS uretir.

```powershell
Test-Path 'C:\versacoder\src\VersaCoder.Domain'
```

Dizin su an yok cikti `False` olur; PASS yazilmaz -> **DOĞRULAMA GEREKLI**.

---

## §3 Aile Yapisi

Rol kalibi (4): `I<F>` · `<F>Module` · `<F>Descriptor` · `<F>Tests`

| # | Aile |
|---|---|
| 1 | DomainCore |
| 2 | EntityModel |
| 3 | ValueObjectModel |
| 4 | AggregateModel |
| 5 | InvariantModel |

Her aile ayni rol kalibini 4 kez tekrarlar; kayit sayisi = aile x rol.

---

## §4 Kayit Ornegi (8 sutun)

| ID | Bilesen | Tip | Sorumluluk | Proje | TFM | Test | Durum |
|---|---|---|---|---|---|---|---|
| N46-001 | `DomainCore` | interface | DomainCore katman sozlesmesi | VersaCoder.Domain | net48;net10.0-windows | DomainCoreTests | planli |
| N46-002 | `DomainCoreModule` | class | DomainCore modul kaydi | VersaCoder.Domain | net48;net10.0-windows | DomainCoreTests | planli |
| N46-003 | `DomainCoreDescriptor` | record | DomainCore katman tanimi kaydi | VersaCoder.Domain | net48;net10.0-windows | DomainCoreTests | planli |
| N46-004 | `DomainCoreTests` | class | DomainCore mimari testleri | VersaCoder.Domain | net48;net10.0-windows | - | planli |

Kalan aileler ayni kalibi 12 rolle tekrarlar (makro dugumlerde 4 rol); toplam hedef **20** kayit.

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
| 1 | Kayit satiri | 20 (asgari 20) | bu dosyada 4 ornek satir; tam liste DOĞRULAMA GEREKLI |
| 2 | Durum degeri | planli -> yapildi -> testli | planli |
| 3 | Test sinifi | DomainCoreTests (xUnit) | planli |
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
(Select-String -Path 'C:\versacoder\.ai\architecture\catalog\N46-macro-l1.md' -Pattern '^\|\s*N\d{2}-\d{3}\s*\|').Count
# 2) boyut bandi - beklenen: 2600-8500
(Get-Item 'C:\versacoder\.ai\architecture\catalog\N46-macro-l1.md').Length
# 3) klasor kaniti - gercek proje adi (ADR-028 2.3/5)
(Test-Path 'C:\versacoder\src\VersaCoder.Domain')
```

Katalog sayimi `powershell -NoProfile -ExecutionPolicy Bypass -File C:\versacoder\.ai\scripts\count-components.ps1` ile alinir; bu batch sonunda toplam esik (5000) henuz karsilanmaz - kalan 26 node dosyasi sonraki batchtir.

## §10 Kayit Tablosu (ADR-013 2.2 - kaynak vault port)

| N46-005 | `IEntityModel` | interface | EntityModel katman sozlesmesi | VersaCoder.Domain | net48;net10.0-windows | EntityModelTests | planli |
| N46-006 | `EntityModelModule` | class | EntityModel modul kaydi | VersaCoder.Domain | net48;net10.0-windows | EntityModelTests | planli |
| N46-007 | `EntityModelDescriptor` | record | EntityModel katman tanimi kaydi | VersaCoder.Domain | net48;net10.0-windows | EntityModelTests | planli |
| N46-008 | `EntityModelTests` | class | EntityModel mimari testleri | VersaCoder.Domain | net48;net10.0-windows | - | planli |
| N46-009 | `IValueObjectModel` | interface | ValueObjectModel katman sozlesmesi | VersaCoder.Domain | net48;net10.0-windows | ValueObjectModelTests | planli |
| N46-010 | `ValueObjectModelModule` | class | ValueObjectModel modul kaydi | VersaCoder.Domain | net48;net10.0-windows | ValueObjectModelTests | planli |
| N46-011 | `ValueObjectModelDescriptor` | record | ValueObjectModel katman tanimi kaydi | VersaCoder.Domain | net48;net10.0-windows | ValueObjectModelTests | planli |
| N46-012 | `ValueObjectModelTests` | class | ValueObjectModel mimari testleri | VersaCoder.Domain | net48;net10.0-windows | - | planli |
| N46-013 | `IAggregateModel` | interface | AggregateModel katman sozlesmesi | VersaCoder.Domain | net48;net10.0-windows | AggregateModelTests | planli |
| N46-014 | `AggregateModelModule` | class | AggregateModel modul kaydi | VersaCoder.Domain | net48;net10.0-windows | AggregateModelTests | planli |
| N46-015 | `AggregateModelDescriptor` | record | AggregateModel katman tanimi kaydi | VersaCoder.Domain | net48;net10.0-windows | AggregateModelTests | planli |
| N46-016 | `AggregateModelTests` | class | AggregateModel mimari testleri | VersaCoder.Domain | net48;net10.0-windows | - | planli |
| N46-017 | `IInvariantModel` | interface | InvariantModel katman sozlesmesi | VersaCoder.Domain | net48;net10.0-windows | InvariantModelTests | planli |
| N46-018 | `InvariantModelModule` | class | InvariantModel modul kaydi | VersaCoder.Domain | net48;net10.0-windows | InvariantModelTests | planli |
| N46-019 | `InvariantModelDescriptor` | record | InvariantModel katman tanimi kaydi | VersaCoder.Domain | net48;net10.0-windows | InvariantModelTests | planli |
| N46-020 | `InvariantModelTests` | class | InvariantModel mimari testleri | VersaCoder.Domain | net48;net10.0-windows | - | planli |
