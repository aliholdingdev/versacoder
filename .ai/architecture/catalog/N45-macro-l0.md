---
title: "Katalog Dugumu N45 - Makro l0 Abstractions (l0)"
type: catalog
category: architecture
status: needs-validation
version: 1.0.0
updated: 2026-10-04
authority: derived
scope: "N45 macro-l0 dugumu: kimlik, src klasoru, aile yapisi, ornek kayit ve yon kurali (ADR-013 + ADR-028)"
---

# Katalog Dugumu N45 - Makro l0 Abstractions

**Zorunlu Baglantilar:** [catalog index](index.md) · [node-inventory](node-inventory.md) · [mimari index](../index.md) · [ADR-028](../../.decisions/accepted/ADR-028-layered-baseline-9-projects.md) · [ADR-013](../../.decisions/accepted/ADR-013-layered-architecture-52-nodes.md)

> **Tek cumlelik sorumluluk:** *N45 macro-l0 hangi projede ve klasorde yasar, ne yapar, kac kayit hedefler* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

**N45 (macro-l0), l0 Abstractions katmaninin makro ozet dugumudur: `VersaCoder.Abstractions` proje kokunde yasar.** l0 katmaninin makro ozetini tasir: LayerAbstractions, PortContracts, ResultContracts, EventContracts, ExtKit. 5 aile x 4 rol = 20 kayit hedefi (ADR-013 asgari 20); durum `planli`.

Makro dugumler alt katman dosyalarini tekrarlamaz, katman ozetini tasir; kayitlar ADR-013 2.1 biciminde 8 sutundur. Bu dosya `authority: derived` + `status: needs-validation` icerir: klasor ve aile listesi `src/` olusunca dogrulanir.

```text
+--------------------------------------------------+
| l0 Abstractions -> VersaCoder.Abstractions
| src/VersaCoder.Abstractions/
| 5 aile x 4 rol = 20 kayit (esik 20)
+--------------------------------------------------+
```

| Alan | Deger |
|---|---|
| Dugum | N45 |
| Dugum turu | makro ozet dugumu |
| Makro katman | l0 Abstractions |
| Proje (.csproj) | VersaCoder.Abstractions |
| TFM | net48;net10.0-windows |
| Kayit hedefi | 20 (ADR-013 asgari 20) |
| Durum | planli |
| Sorumlu rol | architect |

---

## §1 Sorumluluk (YAPAR / YAPMAZ)

| # | YAPAR | YAPMAZ |
|---|---|---|
| 1 | l0 katmaninin makro ozetini tasir: LayerAbstractions, PortContracts, ResultContracts, EventContracts, ExtKit | somut uygulama (adapter) sinifi yazmak, uygulama l4 katmaninda |
| 2 | kayitlari 8 sutunlu ADR-013 biciminde tutmak (planli -> yapildi -> testli) | baska VersaCoder projesine referans vermek |
| 3 | sozlesme ve arayuz tanimlamak (interface, record, enum) | veri erisimi, SQL veya dosya I/O cagirmak |
| 4 | hicbir somut uygulama sinifi yazmamak | UI tipine bakmak (Desktop / Cli) |
| 5 | yalniz BCL ve Directory.Packages.props paketlerine bakmak | framework bagimliligi getirmek (or. EntityFramework) |
| 6 | geri donusu l0 interface uzerinden saglamak | gizli is kurali veya sabit olmayan deger uretmek |

---

## §2 Klasor ve N Eslemesi

**Kalip (ADR-028 2.3/4):** `N33 -> src\VersaCoder.Protocol\ToolSchema\` satiri SSOT ornegidir; bu dosyanin klasoru ayni kalipla yazilir.

| # | N | Slug | src/ klasoru |
|---|---|---|---|
| 1 | N45 | macro-l0 | `src/VersaCoder.Abstractions/` |

**Kanit yolu (ADR-028 2.3/5):** tarama yolu gercek proje adini icerir; genis kalip (`src/**/Domain/**`) sahte PASS uretir.

```powershell
Test-Path 'C:\versacoder\src\VersaCoder.Abstractions'
```

Dizin su an yok cikti `False` olur; PASS yazilmaz -> **DOĞRULAMA GEREKLI**.

---

## §3 Aile Yapisi

Rol kalibi (4): `I<F>` · `<F>Module` · `<F>Descriptor` · `<F>Tests`

| # | Aile |
|---|---|
| 1 | LayerAbstractions |
| 2 | PortContracts |
| 3 | ResultContracts |
| 4 | EventContracts |
| 5 | ExtKit |

Her aile ayni rol kalibini 4 kez tekrarlar; kayit sayisi = aile x rol.

---

## §4 Kayit Ornegi (8 sutun)

| ID | Bilesen | Tip | Sorumluluk | Proje | TFM | Test | Durum |
|---|---|---|---|---|---|---|---|
| N45-001 | `LayerAbstractions` | interface | LayerAbstractions katman sozlesmesi | VersaCoder.Abstractions | net48;net10.0-windows | LayerAbstractionsTests | planli |
| N45-002 | `LayerAbstractionsModule` | class | LayerAbstractions modul kaydi | VersaCoder.Abstractions | net48;net10.0-windows | LayerAbstractionsTests | planli |
| N45-003 | `LayerAbstractionsDescriptor` | record | LayerAbstractions katman tanimi kaydi | VersaCoder.Abstractions | net48;net10.0-windows | LayerAbstractionsTests | planli |
| N45-004 | `LayerAbstractionsTests` | class | LayerAbstractions mimari testleri | VersaCoder.Abstractions | net48;net10.0-windows | - | planli |

Kalan aileler ayni kalibi 12 rolle tekrarlar (makro dugumlerde 4 rol); toplam hedef **20** kayit.

---

## §5 Yon Kurali

| # | Yon | Deger | Kaynak |
|---|---|---|---|
| 1 | l0 -> l1..l7 | X | ADR-028 2.1 |
| 2 | l0 -> dis (BCL, CPM paketi) | izinli | ADR-028 2.1 |
| 3 | l1..l7 -> l0 | OK | ADR-028 2.1 |

Matrisin tamami [layer-matrix](layer-matrix.md) dosyasindadir; NetArchTest `LayerRulesTests` bu tabloya gore yazilir.

---

## §6 Test ve Durum

| # | Kontrol | Beklenen | Durum |
|---|---|---|---|
| 1 | Kayit satiri | 20 (asgari 20) | bu dosyada 4 ornek satir; tam liste DOĞRULAMA GEREKLI |
| 2 | Durum degeri | planli -> yapildi -> testli | planli |
| 3 | Test sinifi | LayerAbstractionsTests (xUnit) | planli |
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
(Select-String -Path 'C:\versacoder\.ai\architecture\catalog\N45-macro-l0.md' -Pattern '^\|\s*N\d{2}-\d{3}\s*\|').Count
# 2) boyut bandi - beklenen: 2600-8500
(Get-Item 'C:\versacoder\.ai\architecture\catalog\N45-macro-l0.md').Length
# 3) klasor kaniti - gercek proje adi (ADR-028 2.3/5)
(Test-Path 'C:\versacoder\src\VersaCoder.Abstractions')
```

Katalog sayimi `powershell -NoProfile -ExecutionPolicy Bypass -File C:\versacoder\.ai\scripts\count-components.ps1` ile alinir; bu batch sonunda toplam esik (5000) henuz karsilanmaz - kalan 26 node dosyasi sonraki batchtir.

## §10 Kayit Tablosu (ADR-013 2.2 - kaynak vault port)

| N45-005 | `IPortContracts` | interface | PortContracts katman sozlesmesi | VersaCoder.Abstractions | net48;net10.0-windows | PortContractsTests | planli |
| N45-006 | `PortContractsModule` | class | PortContracts modul kaydi | VersaCoder.Abstractions | net48;net10.0-windows | PortContractsTests | planli |
| N45-007 | `PortContractsDescriptor` | record | PortContracts katman tanimi kaydi | VersaCoder.Abstractions | net48;net10.0-windows | PortContractsTests | planli |
| N45-008 | `PortContractsTests` | class | PortContracts mimari testleri | VersaCoder.Abstractions | net48;net10.0-windows | - | planli |
| N45-009 | `IResultContracts` | interface | ResultContracts katman sozlesmesi | VersaCoder.Abstractions | net48;net10.0-windows | ResultContractsTests | planli |
| N45-010 | `ResultContractsModule` | class | ResultContracts modul kaydi | VersaCoder.Abstractions | net48;net10.0-windows | ResultContractsTests | planli |
| N45-011 | `ResultContractsDescriptor` | record | ResultContracts katman tanimi kaydi | VersaCoder.Abstractions | net48;net10.0-windows | ResultContractsTests | planli |
| N45-012 | `ResultContractsTests` | class | ResultContracts mimari testleri | VersaCoder.Abstractions | net48;net10.0-windows | - | planli |
| N45-013 | `IEventContracts` | interface | EventContracts katman sozlesmesi | VersaCoder.Abstractions | net48;net10.0-windows | EventContractsTests | planli |
| N45-014 | `EventContractsModule` | class | EventContracts modul kaydi | VersaCoder.Abstractions | net48;net10.0-windows | EventContractsTests | planli |
| N45-015 | `EventContractsDescriptor` | record | EventContracts katman tanimi kaydi | VersaCoder.Abstractions | net48;net10.0-windows | EventContractsTests | planli |
| N45-016 | `EventContractsTests` | class | EventContracts mimari testleri | VersaCoder.Abstractions | net48;net10.0-windows | - | planli |
| N45-017 | `IExtKit` | interface | ExtKit katman sozlesmesi | VersaCoder.Abstractions | net48;net10.0-windows | ExtKitTests | planli |
| N45-018 | `ExtKitModule` | class | ExtKit modul kaydi | VersaCoder.Abstractions | net48;net10.0-windows | ExtKitTests | planli |
| N45-019 | `ExtKitDescriptor` | record | ExtKit katman tanimi kaydi | VersaCoder.Abstractions | net48;net10.0-windows | ExtKitTests | planli |
| N45-020 | `ExtKitTests` | class | ExtKit mimari testleri | VersaCoder.Abstractions | net48;net10.0-windows | - | planli |
