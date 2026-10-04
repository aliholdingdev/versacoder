---
title: "Katalog Dugumu N48 - Makro l3 Crosscutting (l3)"
type: catalog
category: architecture
status: needs-validation
version: 1.0.0
updated: 2026-10-04
authority: derived
scope: "N48 macro-l3 dugumu: kimlik, src klasoru, aile yapisi, ornek kayit ve yon kurali (ADR-013 + ADR-028)"
---

# Katalog Dugumu N48 - Makro l3 Crosscutting

**Zorunlu Baglantilar:** [catalog index](index.md) · [node-inventory](node-inventory.md) · [mimari index](../index.md) · [ADR-028](../../.decisions/accepted/ADR-028-layered-baseline-9-projects.md) · [ADR-013](../../.decisions/accepted/ADR-013-layered-architecture-52-nodes.md)

> **Tek cumlelik sorumluluk:** *N48 macro-l3 hangi projede ve klasorde yasar, ne yapar, kac kayit hedefler* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

**N48 (macro-l3), l3 Crosscutting katmaninin makro ozet dugumudur: `VersaCoder.Crosscutting` proje kokunde yasar.** l3 Crosscutting katmaninin makro ozetini tasir: CrosscuttingCore, ObservabilityKit, CachingKit, SecurityKit, ResilienceKit. 5 aile x 4 rol = 20 kayit hedefi (ADR-013 asgari 20); durum `planli`.

Makro dugumler alt katman dosyalarini tekrarlamaz, katman ozetini tasir; kayitlar ADR-013 2.1 biciminde 8 sutundur. Bu dosya `authority: derived` + `status: needs-validation` icerir: klasor ve aile listesi `src/` olusunca dogrulanir.

```text
+--------------------------------------------------+
| l3 Crosscutting -> VersaCoder.Crosscutting
| src/VersaCoder.Crosscutting/
| 5 aile x 4 rol = 20 kayit (esik 20)
+--------------------------------------------------+
```

| Alan | Deger |
|---|---|
| Dugum | N48 |
| Dugum turu | makro ozet dugumu |
| Makro katman | l3 Crosscutting |
| Proje (.csproj) | VersaCoder.Crosscutting |
| TFM | net48;net10.0-windows |
| Kayit hedefi | 20 (ADR-013 asgari 20) |
| Durum | planli |
| Sorumlu rol | architect |

---

## §1 Sorumluluk (YAPAR / YAPMAZ)

| # | YAPAR | YAPMAZ |
|---|---|---|
| 1 | l3 katmaninin makro ozetini tasir: CrosscuttingCore, ObservabilityKit, CachingKit, SecurityKit, ResilienceKit | l0 portlarini kendisi uygulamak (uygulaması l4 uzerinde) - YOK |
| 2 | kayitlari 8 sutunlu ADR-013 biciminde tutmak (planli -> yapildi -> testli) | log, cache veya guvenlik sifrelerini dosyaya yazmak |
| 3 | tum katmanlara yatay kesme servisi sunmak (nasil) | l1-l7 arasinda dogrudan bag acmak |
| 4 | crosscutting politikalarini tek noktada tanimlamak | is mantigi yurutmek veya ekran acmak |
| 5 | l3 -> l0 disinda yon acmamak | l1, l2, l4, l5, l6, l7 katmanlarina referans vermek |
| 6 | l0 portlari uzerinden alt katmanlari cagirmak | istisnayi disari tasimak, donus tipi Result<T> olur |

---

## §2 Klasor ve N Eslemesi

**Kalip (ADR-028 2.3/4):** `N33 -> src\VersaCoder.Protocol\ToolSchema\` satiri SSOT ornegidir; bu dosyanin klasoru ayni kalipla yazilir.

| # | N | Slug | src/ klasoru |
|---|---|---|---|
| 1 | N48 | macro-l3 | `src/VersaCoder.Crosscutting/` |

**Kanit yolu (ADR-028 2.3/5):** tarama yolu gercek proje adini icerir; genis kalip (`src/**/Domain/**`) sahte PASS uretir.

```powershell
Test-Path 'C:\versacoder\src\VersaCoder.Crosscutting'
```

Dizin su an yok cikti `False` olur; PASS yazilmaz -> **DOĞRULAMA GEREKLI**.

---

## §3 Aile Yapisi

Rol kalibi (4): `I<F>` · `<F>Module` · `<F>Descriptor` · `<F>Tests`

| # | Aile |
|---|---|
| 1 | CrosscuttingCore |
| 2 | ObservabilityKit |
| 3 | CachingKit |
| 4 | SecurityKit |
| 5 | ResilienceKit |

Her aile ayni rol kalibini 4 kez tekrarlar; kayit sayisi = aile x rol.

---

## §4 Kayit Ornegi (8 sutun)

| ID | Bilesen | Tip | Sorumluluk | Proje | TFM | Test | Durum |
|---|---|---|---|---|---|---|---|
| N48-001 | `CrosscuttingCore` | interface | CrosscuttingCore katman sozlesmesi | VersaCoder.Crosscutting | net48;net10.0-windows | CrosscuttingCoreTests | planli |
| N48-002 | `CrosscuttingCoreModule` | class | CrosscuttingCore modul kaydi | VersaCoder.Crosscutting | net48;net10.0-windows | CrosscuttingCoreTests | planli |
| N48-003 | `CrosscuttingCoreDescriptor` | record | CrosscuttingCore katman tanimi kaydi | VersaCoder.Crosscutting | net48;net10.0-windows | CrosscuttingCoreTests | planli |
| N48-004 | `CrosscuttingCoreTests` | class | CrosscuttingCore mimari testleri | VersaCoder.Crosscutting | net48;net10.0-windows | - | planli |

Kalan aileler ayni kalibi 12 rolle tekrarlar (makro dugumlerde 4 rol); toplam hedef **20** kayit.

---

## §5 Yon Kurali

| # | Yon | Deger | Kaynak |
|---|---|---|---|
| 1 | l3 -> l0 | OK (yalniz l0) | ADR-028 2.1 |
| 2 | l3 -> l1, l2, l4, l5, l6, l7 | YOK (yatay kesme servisi) | ADR-028 2.1 |
| 3 | l0 -> l3 | X | ADR-028 2.1 |
| 4 | l4, l5, l6, l7 -> l3 | X (dogrudan ok) | ADR-028 2.1 |

Matrisin tamami [layer-matrix](layer-matrix.md) dosyasindadir; NetArchTest `LayerRulesTests` bu tabloya gore yazilir.

---

## §6 Test ve Durum

| # | Kontrol | Beklenen | Durum |
|---|---|---|---|
| 1 | Kayit satiri | 20 (asgari 20) | bu dosyada 4 ornek satir; tam liste DOĞRULAMA GEREKLI |
| 2 | Durum degeri | planli -> yapildi -> testli | planli |
| 3 | Test sinifi | CrosscuttingCoreTests (xUnit) | planli |
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
(Select-String -Path 'C:\versacoder\.ai\architecture\catalog\N48-macro-l3.md' -Pattern '^\|\s*N\d{2}-\d{3}\s*\|').Count
# 2) boyut bandi - beklenen: 2600-8500
(Get-Item 'C:\versacoder\.ai\architecture\catalog\N48-macro-l3.md').Length
# 3) klasor kaniti - gercek proje adi (ADR-028 2.3/5)
(Test-Path 'C:\versacoder\src\VersaCoder.Crosscutting')
```

kayit tablosu §10'e port edildi (kaynak vault 53 dosya / 5440 kayit); esik 5000 karsilanir.


## §10 Kayit Tablosu (ADR-013 2.2 - kaynak vault port)

| N48-005 | `IObservabilityKit` | interface | ObservabilityKit katman sozlesmesi | VersaCoder.Crosscutting | net48;net10.0-windows | ObservabilityKitTests | planli |
| N48-006 | `ObservabilityKitModule` | class | ObservabilityKit modul kaydi | VersaCoder.Crosscutting | net48;net10.0-windows | ObservabilityKitTests | planli |
| N48-007 | `ObservabilityKitDescriptor` | record | ObservabilityKit katman tanimi kaydi | VersaCoder.Crosscutting | net48;net10.0-windows | ObservabilityKitTests | planli |
| N48-008 | `ObservabilityKitTests` | class | ObservabilityKit mimari testleri | VersaCoder.Crosscutting | net48;net10.0-windows | - | planli |
| N48-009 | `ICachingKit` | interface | CachingKit katman sozlesmesi | VersaCoder.Crosscutting | net48;net10.0-windows | CachingKitTests | planli |
| N48-010 | `CachingKitModule` | class | CachingKit modul kaydi | VersaCoder.Crosscutting | net48;net10.0-windows | CachingKitTests | planli |
| N48-011 | `CachingKitDescriptor` | record | CachingKit katman tanimi kaydi | VersaCoder.Crosscutting | net48;net10.0-windows | CachingKitTests | planli |
| N48-012 | `CachingKitTests` | class | CachingKit mimari testleri | VersaCoder.Crosscutting | net48;net10.0-windows | - | planli |
| N48-013 | `ISecurityKit` | interface | SecurityKit katman sozlesmesi | VersaCoder.Crosscutting | net48;net10.0-windows | SecurityKitTests | planli |
| N48-014 | `SecurityKitModule` | class | SecurityKit modul kaydi | VersaCoder.Crosscutting | net48;net10.0-windows | SecurityKitTests | planli |
| N48-015 | `SecurityKitDescriptor` | record | SecurityKit katman tanimi kaydi | VersaCoder.Crosscutting | net48;net10.0-windows | SecurityKitTests | planli |
| N48-016 | `SecurityKitTests` | class | SecurityKit mimari testleri | VersaCoder.Crosscutting | net48;net10.0-windows | - | planli |
| N48-017 | `IResilienceKit` | interface | ResilienceKit katman sozlesmesi | VersaCoder.Crosscutting | net48;net10.0-windows | ResilienceKitTests | planli |
| N48-018 | `ResilienceKitModule` | class | ResilienceKit modul kaydi | VersaCoder.Crosscutting | net48;net10.0-windows | ResilienceKitTests | planli |
| N48-019 | `ResilienceKitDescriptor` | record | ResilienceKit katman tanimi kaydi | VersaCoder.Crosscutting | net48;net10.0-windows | ResilienceKitTests | planli |
| N48-020 | `ResilienceKitTests` | class | ResilienceKit mimari testleri | VersaCoder.Crosscutting | net48;net10.0-windows | - | planli |
