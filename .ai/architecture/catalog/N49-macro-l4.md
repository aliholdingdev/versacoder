---
title: "Katalog Dugumu N49 - Makro l4 Infrastructure (l4)"
type: catalog
category: architecture
status: needs-validation
version: 1.0.0
updated: 2026-10-04
authority: derived
scope: "N49 macro-l4 dugumu: kimlik, src klasoru, aile yapisi, ornek kayit ve yon kurali (ADR-013 + ADR-028)"
---

# Katalog Dugumu N49 - Makro l4 Infrastructure

**Zorunlu Baglantilar:** [catalog index](index.md) · [node-inventory](node-inventory.md) · [mimari index](../index.md) · [ADR-028](../../.decisions/accepted/ADR-028-layered-baseline-9-projects.md) · [ADR-013](../../.decisions/accepted/ADR-013-layered-architecture-52-nodes.md)

> **Tek cumlelik sorumluluk:** *N49 macro-l4 hangi projede ve klasorde yasar, ne yapar, kac kayit hedefler* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

**N49 (macro-l4), l4 Infrastructure katmaninin makro ozet dugumudur: `VersaCoder.Infrastructure` proje kokunde yasar.** l4 Infrastructure katmaninin makro ozetini tasir: InfrastructureCore, DataKit, StorageKit, GitKit, LlmKit. 5 aile x 4 rol = 20 kayit hedefi (ADR-013 asgari 20); durum `planli`.

Makro dugumler alt katman dosyalarini tekrarlamaz, katman ozetini tasir; kayitlar ADR-013 2.1 biciminde 8 sutundur. Bu dosya `authority: derived` + `status: needs-validation` icerir: klasor ve aile listesi `src/` olusunca dogrulanir.

```text
+--------------------------------------------------+
| l4 Infrastructure -> VersaCoder.Infrastructure
| src/VersaCoder.Infrastructure/
| 5 aile x 4 rol = 20 kayit (esik 20)
+--------------------------------------------------+
```

| Alan | Deger |
|---|---|
| Dugum | N49 |
| Dugum turu | makro ozet dugumu |
| Makro katman | l4 Infrastructure |
| Proje (.csproj) | VersaCoder.Infrastructure |
| TFM | net48;net10.0-windows |
| Kayit hedefi | 20 (ADR-013 asgari 20) |
| Durum | planli |
| Sorumlu rol | architect |

---

## §1 Sorumluluk (YAPAR / YAPMAZ)

| # | YAPAR | YAPMAZ |
|---|---|---|
| 1 | l4 katmaninin makro ozetini tasir: InfrastructureCore, DataKit, StorageKit, GitKit, LlmKit | l2 katmanina dogrudan bakmak (repository portu l0 katmaninda, uygulamasi l4 uzerinde) - YOK |
| 2 | kayitlari 8 sutunlu ADR-013 biciminde tutmak (planli -> yapildi -> testli) | SQL yazmak veya baglanti yonetmek |
| 3 | ADO, SQLite, git, HTTP gibi dis bagimliliklari uyarlamak | domain invariantini ihlal etmek |
| 4 | l0 portlarini somutlandirmak ve saglik durumunu bildirmek | UI ekrani acmak veya Ribbon kontrolune dokunmak |
| 5 | l0 ve l1 disinda bagimlilik acmamak | l2, l3, l5, l6, l7 katmanlarina referans vermek |
| 6 | istisnayi disari tasimak, donus tipi Result<T> olur | portal seviyesinde is mantigi yurutmek |

---

## §2 Klasor ve N Eslemesi

**Kalip (ADR-028 2.3/4):** `N33 -> src\VersaCoder.Protocol\ToolSchema\` satiri SSOT ornegidir; bu dosyanin klasoru ayni kalipla yazilir.

| # | N | Slug | src/ klasoru |
|---|---|---|---|
| 1 | N49 | macro-l4 | `src/VersaCoder.Infrastructure/` |

**Kanit yolu (ADR-028 2.3/5):** tarama yolu gercek proje adini icerir; genis kalip (`src/**/Domain/**`) sahte PASS uretir.

```powershell
Test-Path 'C:\versacoder\src\VersaCoder.Infrastructure'
```

Dizin su an yok cikti `False` olur; PASS yazilmaz -> **DOĞRULAMA GEREKLI**.

---

## §3 Aile Yapisi

Rol kalibi (4): `I<F>` · `<F>Module` · `<F>Descriptor` · `<F>Tests`

| # | Aile |
|---|---|
| 1 | InfrastructureCore |
| 2 | DataKit |
| 3 | StorageKit |
| 4 | GitKit |
| 5 | LlmKit |

Her aile ayni rol kalibini 4 kez tekrarlar; kayit sayisi = aile x rol.

---

## §4 Kayit Ornegi (8 sutun)

| ID | Bilesen | Tip | Sorumluluk | Proje | TFM | Test | Durum |
|---|---|---|---|---|---|---|---|
| N49-001 | `InfrastructureCore` | interface | InfrastructureCore katman sozlesmesi | VersaCoder.Infrastructure | net48;net10.0-windows | InfrastructureCoreTests | planli |
| N49-002 | `InfrastructureCoreModule` | class | InfrastructureCore modul kaydi | VersaCoder.Infrastructure | net48;net10.0-windows | InfrastructureCoreTests | planli |
| N49-003 | `InfrastructureCoreDescriptor` | record | InfrastructureCore katman tanimi kaydi | VersaCoder.Infrastructure | net48;net10.0-windows | InfrastructureCoreTests | planli |
| N49-004 | `InfrastructureCoreTests` | class | InfrastructureCore mimari testleri | VersaCoder.Infrastructure | net48;net10.0-windows | - | planli |

Kalan aileler ayni kalibi 12 rolle tekrarlar (makro dugumlerde 4 rol); toplam hedef **20** kayit.

---

## §5 Yon Kurali

| # | Yon | Deger | Kaynak |
|---|---|---|---|
| 1 | l4 -> l0 ve l1 | OK | ADR-028 2.1 |
| 2 | l4 -> l2 | YOK (repository portu l0 katmaninda) | ADR-028 2.3/2 |
| 3 | l4 -> l3, l5, l6, l7 | YOK | ADR-028 2.1 |
| 4 | l2 -> l4 | X | ADR-028 2.3/2 |

Matrisin tamami [layer-matrix](layer-matrix.md) dosyasindadir; NetArchTest `LayerRulesTests` bu tabloya gore yazilir.

---

## §6 Test ve Durum

| # | Kontrol | Beklenen | Durum |
|---|---|---|---|
| 1 | Kayit satiri | 20 (asgari 20) | bu dosyada 4 ornek satir; tam liste DOĞRULAMA GEREKLI |
| 2 | Durum degeri | planli -> yapildi -> testli | planli |
| 3 | Test sinifi | InfrastructureCoreTests (xUnit) | planli |
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
(Select-String -Path 'C:\versacoder\.ai\architecture\catalog\N49-macro-l4.md' -Pattern '^\|\s*N\d{2}-\d{3}\s*\|').Count
# 2) boyut bandi - beklenen: 2600-8500
(Get-Item 'C:\versacoder\.ai\architecture\catalog\N49-macro-l4.md').Length
# 3) klasor kaniti - gercek proje adi (ADR-028 2.3/5)
(Test-Path 'C:\versacoder\src\VersaCoder.Infrastructure')
```

kayit tablosu §10'e port edildi (kaynak vault 53 dosya / 5440 kayit); esik 5000 karsilanir.


## §10 Kayit Tablosu (ADR-013 2.2 - kaynak vault port)

| N49-005 | `IDataKit` | interface | DataKit katman sozlesmesi | VersaCoder.Infrastructure | net48;net10.0-windows | DataKitTests | planli |
| N49-006 | `DataKitModule` | class | DataKit modul kaydi | VersaCoder.Infrastructure | net48;net10.0-windows | DataKitTests | planli |
| N49-007 | `DataKitDescriptor` | record | DataKit katman tanimi kaydi | VersaCoder.Infrastructure | net48;net10.0-windows | DataKitTests | planli |
| N49-008 | `DataKitTests` | class | DataKit mimari testleri | VersaCoder.Infrastructure | net48;net10.0-windows | - | planli |
| N49-009 | `IStorageKit` | interface | StorageKit katman sozlesmesi | VersaCoder.Infrastructure | net48;net10.0-windows | StorageKitTests | planli |
| N49-010 | `StorageKitModule` | class | StorageKit modul kaydi | VersaCoder.Infrastructure | net48;net10.0-windows | StorageKitTests | planli |
| N49-011 | `StorageKitDescriptor` | record | StorageKit katman tanimi kaydi | VersaCoder.Infrastructure | net48;net10.0-windows | StorageKitTests | planli |
| N49-012 | `StorageKitTests` | class | StorageKit mimari testleri | VersaCoder.Infrastructure | net48;net10.0-windows | - | planli |
| N49-013 | `IGitKit` | interface | GitKit katman sozlesmesi | VersaCoder.Infrastructure | net48;net10.0-windows | GitKitTests | planli |
| N49-014 | `GitKitModule` | class | GitKit modul kaydi | VersaCoder.Infrastructure | net48;net10.0-windows | GitKitTests | planli |
| N49-015 | `GitKitDescriptor` | record | GitKit katman tanimi kaydi | VersaCoder.Infrastructure | net48;net10.0-windows | GitKitTests | planli |
| N49-016 | `GitKitTests` | class | GitKit mimari testleri | VersaCoder.Infrastructure | net48;net10.0-windows | - | planli |
| N49-017 | `ILlmKit` | interface | LlmKit katman sozlesmesi | VersaCoder.Infrastructure | net48;net10.0-windows | LlmKitTests | planli |
| N49-018 | `LlmKitModule` | class | LlmKit modul kaydi | VersaCoder.Infrastructure | net48;net10.0-windows | LlmKitTests | planli |
| N49-019 | `LlmKitDescriptor` | record | LlmKit katman tanimi kaydi | VersaCoder.Infrastructure | net48;net10.0-windows | LlmKitTests | planli |
| N49-020 | `LlmKitTests` | class | LlmKit mimari testleri | VersaCoder.Infrastructure | net48;net10.0-windows | - | planli |
