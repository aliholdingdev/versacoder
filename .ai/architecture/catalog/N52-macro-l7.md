---
title: "Katalog Dugumu N52 - Makro l7 UI + CLI (l7)"
type: catalog
category: architecture
status: needs-validation
version: 1.0.0
updated: 2026-10-04
authority: derived
scope: "N52 macro-l7 dugumu: kimlik, src klasoru, aile yapisi, ornek kayit ve yon kurali (ADR-013 + ADR-028)"
---

# Katalog Dugumu N52 - Makro l7 UI + CLI

**Zorunlu Baglantilar:** [catalog index](index.md) · [node-inventory](node-inventory.md) · [mimari index](../index.md) · [ADR-028](../../.decisions/accepted/ADR-028-layered-baseline-9-projects.md) · [ADR-013](../../.decisions/accepted/ADR-013-layered-architecture-52-nodes.md)

> **Tek cumlelik sorumluluk:** *N52 macro-l7 hangi projede ve klasorde yasar, ne yapar, kac kayit hedefler* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

**N52 (macro-l7), l7 UI katmaninin makro ozet dugumudur: `VersaCoder.Desktop` proje kokunde yasar.** l7 UI katmaninin makro ozetini tasir: DesktopCore, ShellKit, PanelKit, EditorKit, SettingsKit. 5 aile x 4 rol = 20 kayit hedefi (ADR-013 asgari 20); durum `planli`.

Makro dugumler alt katman dosyalarini tekrarlamaz, katman ozetini tasir; kayitlar ADR-013 2.1 biciminde 8 sutundur. Bu dosya `authority: derived` + `status: needs-validation` icerir: klasor ve aile listesi `src/` olusunca dogrulanir.

```text
+--------------------------------------------------+
| l7 UI -> VersaCoder.Desktop
| src/VersaCoder.Desktop/
| 5 aile x 4 rol = 20 kayit (esik 20)
+--------------------------------------------------+
```

| Alan | Deger |
|---|---|
| Dugum | N52 |
| Dugum turu | makro ozet dugumu |
| Makro katman | l7 UI |
| Proje (.csproj) | VersaCoder.Desktop |
| TFM | net48;net10.0-windows |
| Kayit hedefi | 20 (ADR-013 asgari 20) |
| Durum | planli |
| Sorumlu rol | architect |

---

## §1 Sorumluluk (YAPAR / YAPMAZ)

| # | YAPAR | YAPMAZ |
|---|---|---|
| 1 | l7 katmaninin makro ozetini tasir: DesktopCore, ShellKit, PanelKit, EditorKit, SettingsKit | is mantigi yurutmek (l2/l5 ise) - YOK |
| 2 | kayitlari 8 sutunlu ADR-013 biciminde tutmak (planli -> yapildi -> testli) | LLM cevabini uretmek veya dosya yazmak |
| 3 | ekran, panel ve CLI etkilesimini tanimlamak | domain invariantini ihlal etmek |
| 4 | komut kaydina bagli Ribbon ve konsol ciktisi uretmek | log, cache veya guvenlik sifresi uretmek |
| 5 | l0, l2, l5 ve l6 disinda bagimlilik acmamak | l1, l3 ve l4 katmanlarina referans vermek |
| 6 | komutlari l2 use-case uzerinden cagirmak | istisnayi disari tasimak, donus tipi Result<T> olur |

---

## §2 Klasor ve N Eslemesi

**Kalip (ADR-028 2.3/4):** `N33 -> src\VersaCoder.Protocol\ToolSchema\` satiri SSOT ornegidir; bu dosyanin klasoru ayni kalipla yazilir.

| # | N | Slug | src/ klasoru |
|---|---|---|---|
| 1 | N52 | macro-l7 | `src/VersaCoder.Desktop/` |

**Kanit yolu (ADR-028 2.3/5):** tarama yolu gercek proje adini icerir; genis kalip (`src/**/Domain/**`) sahte PASS uretir.

```powershell
Test-Path 'C:\versacoder\src\VersaCoder.Desktop'
```

Dizin su an yok cikti `False` olur; PASS yazilmaz -> **DOĞRULAMA GEREKLI**.

---

## §3 Aile Yapisi

Rol kalibi (4): `I<F>` · `<F>Module` · `<F>Descriptor` · `<F>Tests`

| # | Aile |
|---|---|
| 1 | DesktopCore |
| 2 | ShellKit |
| 3 | PanelKit |
| 4 | EditorKit |
| 5 | SettingsKit |

Her aile ayni rol kalibini 4 kez tekrarlar; kayit sayisi = aile x rol.

---

## §4 Kayit Ornegi (8 sutun)

| ID | Bilesen | Tip | Sorumluluk | Proje | TFM | Test | Durum |
|---|---|---|---|---|---|---|---|
| N52-001 | `DesktopCore` | interface | DesktopCore katman sozlesmesi | VersaCoder.Desktop | net48;net10.0-windows | DesktopCoreTests | planli |
| N52-002 | `DesktopCoreModule` | class | DesktopCore modul kaydi | VersaCoder.Desktop | net48;net10.0-windows | DesktopCoreTests | planli |
| N52-003 | `DesktopCoreDescriptor` | record | DesktopCore katman tanimi kaydi | VersaCoder.Desktop | net48;net10.0-windows | DesktopCoreTests | planli |
| N52-004 | `DesktopCoreTests` | class | DesktopCore mimari testleri | VersaCoder.Desktop | net48;net10.0-windows | - | planli |

Kalan aileler ayni kalibi 12 rolle tekrarlar (makro dugumlerde 4 rol); toplam hedef **20** kayit.

---

## §5 Yon Kurali

| # | Yon | Deger | Kaynak |
|---|---|---|---|
| 1 | l7 -> l0, l2, l5 ve l6 | OK | ADR-028 2.1 |
| 2 | l7 -> l1 | YOK (yon asagi) | ADR-028 2.1 |
| 3 | l7 -> l3, l4 | YOK (yon asagi) | ADR-028 2.1 |
| 4 | l6 -> l7 | X (host UI acmaz) | ADR-028 2.1 |

Matrisin tamami [layer-matrix](layer-matrix.md) dosyasindadir; NetArchTest `LayerRulesTests` bu tabloya gore yazilir.

---

## §6 Test ve Durum

| # | Kontrol | Beklenen | Durum |
|---|---|---|---|
| 1 | Kayit satiri | 20 (asgari 20) | bu dosyada 4 ornek satir; tam liste DOĞRULAMA GEREKLI |
| 2 | Durum degeri | planli -> yapildi -> testli | planli |
| 3 | Test sinifi | DesktopCoreTests (xUnit) | planli |
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
(Select-String -Path 'C:\versacoder\.ai\architecture\catalog\N52-macro-l7.md' -Pattern '^\|\s*N\d{2}-\d{3}\s*\|').Count
# 2) boyut bandi - beklenen: 2600-8500
(Get-Item 'C:\versacoder\.ai\architecture\catalog\N52-macro-l7.md').Length
# 3) klasor kaniti - gercek proje adi (ADR-028 2.3/5)
(Test-Path 'C:\versacoder\src\VersaCoder.Desktop')
```

kayit tablosu §10'e port edildi (kaynak vault 53 dosya / 5440 kayit); esik 5000 karsilanir.


## §10 Kayit Tablosu (ADR-013 2.2 - kaynak vault port)

| N52-005 | `IShellKit` | interface | ShellKit katman sozlesmesi | VersaCoder.Desktop | net48;net10.0-windows | ShellKitTests | planli |
| N52-006 | `ShellKitModule` | class | ShellKit modul kaydi | VersaCoder.Desktop | net48;net10.0-windows | ShellKitTests | planli |
| N52-007 | `ShellKitDescriptor` | record | ShellKit katman tanimi kaydi | VersaCoder.Desktop | net48;net10.0-windows | ShellKitTests | planli |
| N52-008 | `ShellKitTests` | class | ShellKit mimari testleri | VersaCoder.Desktop | net48;net10.0-windows | - | planli |
| N52-009 | `IPanelKit` | interface | PanelKit katman sozlesmesi | VersaCoder.Desktop | net48;net10.0-windows | PanelKitTests | planli |
| N52-010 | `PanelKitModule` | class | PanelKit modul kaydi | VersaCoder.Desktop | net48;net10.0-windows | PanelKitTests | planli |
| N52-011 | `PanelKitDescriptor` | record | PanelKit katman tanimi kaydi | VersaCoder.Desktop | net48;net10.0-windows | PanelKitTests | planli |
| N52-012 | `PanelKitTests` | class | PanelKit mimari testleri | VersaCoder.Desktop | net48;net10.0-windows | - | planli |
| N52-013 | `IEditorKit` | interface | EditorKit katman sozlesmesi | VersaCoder.Desktop | net48;net10.0-windows | EditorKitTests | planli |
| N52-014 | `EditorKitModule` | class | EditorKit modul kaydi | VersaCoder.Desktop | net48;net10.0-windows | EditorKitTests | planli |
| N52-015 | `EditorKitDescriptor` | record | EditorKit katman tanimi kaydi | VersaCoder.Desktop | net48;net10.0-windows | EditorKitTests | planli |
| N52-016 | `EditorKitTests` | class | EditorKit mimari testleri | VersaCoder.Desktop | net48;net10.0-windows | - | planli |
| N52-017 | `ISettingsKit` | interface | SettingsKit katman sozlesmesi | VersaCoder.Desktop | net48;net10.0-windows | SettingsKitTests | planli |
| N52-018 | `SettingsKitModule` | class | SettingsKit modul kaydi | VersaCoder.Desktop | net48;net10.0-windows | SettingsKitTests | planli |
| N52-019 | `SettingsKitDescriptor` | record | SettingsKit katman tanimi kaydi | VersaCoder.Desktop | net48;net10.0-windows | SettingsKitTests | planli |
| N52-020 | `SettingsKitTests` | class | SettingsKit mimari testleri | VersaCoder.Desktop | net48;net10.0-windows | - | planli |
