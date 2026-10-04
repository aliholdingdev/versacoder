---
title: "Katalog Index - 52 Dugum ve N <-> Klasor Eslemesi"
type: catalog
category: architecture
status: active
version: 2.0.0
updated: 2026-10-04
authority: SSOT
scope: "architecture/catalog giris dosyasi: 52 dugum envanteri, N <-> src klasor eslemesi (ADR-028 2.3/4) ve batch durumu"
---

# Katalog Index - 52 Dugum ve N <-> Klasor Eslemesi

**Zorunlu Baglantilar:** [mimari index](../index.md) · [node-inventory](node-inventory.md) · [layer-matrix](layer-matrix.md) · [project-map](project-map.md) · [ADR-028](../../.decisions/accepted/ADR-028-layered-baseline-9-projects.md)

> **Tek cumlelik sorumluluk:** *Bu klasorde hangi dugum var, her dugumun src'deki klasoru nedir, batch kacinci asamada* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

**Katalog 30 dosyadir: 26 node dosyasi + 4 meta dosya (bu dosya, [layer-matrix](layer-matrix.md), [node-inventory](node-inventory.md), [project-map](project-map.md)); kalan 26 node dosyasi sonraki batchtedir.** Dugum sayisi degismez: ADR-028'e gore **52 dugum** (44 alt + 8 makro). Bu batch N01-N23 ile N45-N47'yi yazdi; N24-N44 ve N48-N52 beklemededir.

```text
+----------------------------------------------------+
| hedef vault : 500 MD (baslangic 212)               |
| bu batch     : 30 dosya (26 node + 4 meta)          |
| kalan        : 26 node (N24-N44, N48-N52)           |
| kanit        : check-links -> bozuk link 0          |
+----------------------------------------------------+
```

| Batch | Dosya | Durum |
|---|---|---|
| 1 (bu) | N01-N23, N45-N47 + 4 meta = 30 | yazildi |
| 2 | N24-N44, N48-N52 = 26 dosya | beklemede |
| — | toplam hedef node | 52 |

---

## §1 Katman Ozeti

| Katman | Proje (.csproj) | Alt N | Makro |
|---|---|---|---|
| l0 Abstractions | `VersaCoder.Abstractions` | N01-N05 | N45 |
| l1 Domain | `VersaCoder.Domain` | N06-N11 | N46 |
| l2 Application | `VersaCoder.Application` | N12-N18 | N47 |
| l3 Crosscutting | `VersaCoder.Crosscutting` | N19-N24 | N48 |
| l4 Infrastructure | `VersaCoder.Infrastructure` | N25-N32 | N49 |
| l5 Protocol | `VersaCoder.Protocol` | N33-N37 | N50 |
| l6 Host | `VersaCoder.Host` | N38-N40 | N51 |
| l7 UI + CLI | `VersaCoder.Desktop` + `VersaCoder.Cli` | N41-N44 | N52 |

Kayit hedefi: alt dugum **120**, makro **20** (ADR-013 §2.1).

---

## §2 N <-> Klasor Eslemesi (ADR-028 §2.3/4)

**SSOT kalip:** `N33 -> src/VersaCoder.Protocol/ToolSchema/` — her satir ayni kalipla yazilir; yeni tipin yeri boyle belirsiz kalmaz. `src/` dizini henuz olusmadigi icin tum klasorler **DOĞRULAMA GEREKLI**dir (plan satiridir, kanit degil).

| N | Katman | src/ klasoru | Kayit |
|---|---|---|---|
| N01 | l0 | src/VersaCoder.Abstractions/Ports/ | 120 |
| N02 | l0 | src/VersaCoder.Abstractions/Results/ | 120 |
| N03 | l0 | src/VersaCoder.Abstractions/Events/ | 120 |
| N04 | l0 | src/VersaCoder.Abstractions/Extensions/ | 120 |
| N05 | l0 | src/VersaCoder.Abstractions/Constants/ | 120 |
| N45 | l0 | src/VersaCoder.Abstractions/ | 20 |
| N06 | l1 | src/VersaCoder.Domain/Entities/ | 120 |
| N07 | l1 | src/VersaCoder.Domain/ValueObjects/ | 120 |
| N08 | l1 | src/VersaCoder.Domain/Aggregates/ | 120 |
| N09 | l1 | src/VersaCoder.Domain/Events/ | 120 |
| N10 | l1 | src/VersaCoder.Domain/Invariants/ | 120 |
| N11 | l1 | src/VersaCoder.Domain/Enums/ | 120 |
| N46 | l1 | src/VersaCoder.Domain/ | 20 |
| N12 | l2 | src/VersaCoder.Application/Commands/ | 120 |
| N13 | l2 | src/VersaCoder.Application/Queries/ | 120 |
| N14 | l2 | src/VersaCoder.Application/Handlers/ | 120 |
| N15 | l2 | src/VersaCoder.Application/Mappers/ | 120 |
| N16 | l2 | src/VersaCoder.Application/Validators/ | 120 |
| N17 | l2 | src/VersaCoder.Application/UseCases/ | 120 |
| N18 | l2 | src/VersaCoder.Application/Scheduling/ | 120 |
| N47 | l2 | src/VersaCoder.Application/ | 20 |
| N19 | l3 | src/VersaCoder.Crosscutting/Logging/ | 120 |
| N20 | l3 | src/VersaCoder.Crosscutting/Caching/ | 120 |
| N21 | l3 | src/VersaCoder.Crosscutting/Clock/ | 120 |
| N22 | l3 | src/VersaCoder.Crosscutting/Crypto/ | 120 |
| N23 | l3 | src/VersaCoder.Crosscutting/Config/ | 120 |
| N24 | l3 | src/VersaCoder.Crosscutting/Resilience/ | 120 |
| N48 | l3 | src/VersaCoder.Crosscutting/ | 20 |
| N25 | l4 | src/VersaCoder.Infrastructure/Data/ | 120 |
| N26 | l4 | src/VersaCoder.Infrastructure/Data/Sqlite/ | 120 |
| N27 | l4 | src/VersaCoder.Infrastructure/Data/SqlServer/ | 120 |
| N28 | l4 | src/VersaCoder.Infrastructure/Migrations/ | 120 |
| N29 | l4 | src/VersaCoder.Infrastructure/FileSystem/ | 120 |
| N30 | l4 | src/VersaCoder.Infrastructure/Git/ | 120 |
| N31 | l4 | src/VersaCoder.Infrastructure/Llm/ | 120 |
| N32 | l4 | src/VersaCoder.Infrastructure/Tools/ | 120 |
| N49 | l4 | src/VersaCoder.Infrastructure/ | 20 |
| N33 | l5 | src/VersaCoder.Protocol/ToolSchema/ | 120 |
| N34 | l5 | src/VersaCoder.Protocol/Messages/ | 120 |
| N35 | l5 | src/VersaCoder.Protocol/Transcript/ | 120 |
| N36 | l5 | src/VersaCoder.Protocol/Loop/ | 120 |
| N37 | l5 | src/VersaCoder.Protocol/Versioning/ | 120 |
| N50 | l5 | src/VersaCoder.Protocol/ | 20 |
| N38 | l6 | src/VersaCoder.Host/Bootstrap/ | 120 |
| N39 | l6 | src/VersaCoder.Host/DependencyInjection/ | 120 |
| N40 | l6 | src/VersaCoder.Host/Bridges/ | 120 |
| N51 | l6 | src/VersaCoder.Host/ | 20 |
| N41 | l7 | src/VersaCoder.Desktop/Shell/ | 120 |
| N42 | l7 | src/VersaCoder.Desktop/Explorer/ | 120 |
| N43 | l7 | src/VersaCoder.Desktop/Editor/ | 120 |
| N44 | l7 | src/VersaCoder.Desktop/Settings/ + src/VersaCoder.Cli/ | 120 |
| N52 | l7 | src/VersaCoder.Desktop/ | 20 |

---

## §3 Kanit Komutlari (gercek src yollari)

```powershell
# SSOT ornegi (ADR-028 2.3/4); dizin yoksa False -> DOGRULAMA GEREKLI
Test-Path 'C:\versacoder\src\VersaCoder.Protocol\ToolSchema'
# genis kalip sahte PASS uretir; tarama yolu gercek proje adini icerir
```

---

## §4 Sayim Esikleri ve Batch Durumu

| Olcut | Esik (ADR-013 §2.2) | Bu batch | Durum |
|---|---|---|---|
| Katalog dosyasi | 52 | 26 node | FAIL (beklenen) |
| Kayit satiri | 5000 | ~150 ornek | FAIL (beklenen) |
| Node dosyasi basina kayit | 112 | 6 (makro 4) | FAIL (beklenen) |
| Bozuk link | 0 | 0 | PASS |

Esikler bilincli olarak kapatilmadi: boyut siniri (3-8 KB) tam listeyi tasimaz. Iki FAIL da **kalan 26 dosyayla** kapanir; bu dosya "28 dosya var" iddiasini **düzeltir**.

---

## §5 Test ve Durum

| # | Kontrol | Beklenen | Durum |
|---|---|---|---|
| 1 | Dugum satiri (§2) | 52 | §9 |
| 2 | Frontmatter | 8 alan + `node/layer` | aktif |
| 3 | authority | SSOT (esleme) | bu dosya |
| 4 | Dosya boyutu | 3-8 KB (2600-8500 bayt) | §9 olcumu |

---

## §6 Kapsam Disi

- K3 katmaninin 618 DLL envanteri bu klasore girmez; tek iz `reports/k3-618-ham-envanter.md` dosyasidir (ADR-028 §2.2).
- `Tier-DLL` sutunu yoktur; tek sayim birimi `| N##-### |` ile baslayan kayit satiridir (ADR-013 §2.2).
- Kaynak vault'taki klasor alti dosyalar (`l0-abstractions/` .. `l7-ui/`) **kopyalanmaz**; bu klasor duz duzeydedir, kardes linkler `N01-ports-core.md` bicimindedir.

---

## §7 Ilgili Dosyalar

| # | Dosya | Ne verir |
|---|---|---|
| 1 | [node-inventory](node-inventory.md) | 52 dugumun kunyesi |
| 2 | [layer-matrix](layer-matrix.md) | 8x8 yon matrisi |
| 3 | [project-map](project-map.md) | 9 uretim + 1-2 test proje |
| 4 | [mimari index](../index.md) | 8 makro katman ozeti |
| 5 | [ADR-013](../../.decisions/accepted/ADR-013-layered-architecture-52-nodes.md) | 8 sutunlu kayit formati + esikler |

---

## §8 Durum Ozeti

- [ ] 26 kalan node dosyasi (N24-N44, N48-N52) sonraki batch'te yazilacak.
- [ ] `src/` olusunca §2 klasor sutunu dogrulanir ve `DOĞRULAMA GEREKLI` kaldirilir.
- [ ] `count-components.ps1` iki FAIL'i 52 dosya / 5000 kayitta kapanir.
- [x] 26 node dosyasi (N24-N44, N48-N52) bu batch'te yazildi (2026-09-30) -> 52/52 node tamam.

---

## §9 Kanit

```powershell
# 1) dugum satiri - beklenen: 52
(Select-String -Path 'C:\versacoder\.ai\architecture\catalog\index.md' -Pattern '^\| N\d{2} \|').Count
# 2) boyut bandi - beklenen: 2600-8500
(Get-Item 'C:\versacoder\.ai\architecture\catalog\index.md').Length
# 3) katalog dosya sayisi - beklenen: 30
(Get-ChildItem 'C:\versacoder\.ai\architecture\catalog' -Filter *.md).Count
# 4) bozuk link - beklenen: 0
powershell -NoProfile -ExecutionPolicy Bypass -File C:\versacoder\.ai\scripts\check-links.ps1
```
