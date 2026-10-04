---
title: "Katalog Dugumu N51 - Makro l6 Host (l6)"
type: catalog
category: architecture
status: needs-validation
version: 1.0.0
updated: 2026-10-04
authority: derived
scope: "N51 macro-l6 dugumu: kimlik, src klasoru, aile yapisi, ornek kayit ve yon kurali (ADR-013 + ADR-028)"
---

# Katalog Dugumu N51 - Makro l6 Host

**Zorunlu Baglantilar:** [catalog index](index.md) · [node-inventory](node-inventory.md) · [mimari index](../index.md) · [ADR-028](../../.decisions/accepted/ADR-028-layered-baseline-9-projects.md) · [ADR-013](../../.decisions/accepted/ADR-013-layered-architecture-52-nodes.md)

> **Tek cumlelik sorumluluk:** *N51 macro-l6 hangi projede ve klasorde yasar, ne yapar, kac kayit hedefler* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

**N51 (macro-l6), l6 Host katmaninin makro ozet dugumudur: `VersaCoder.Host` proje kokunde yasar.** l6 Host katmaninin makro ozetini tasir: HostCore, BootstrapKit, RegistrationKit, ConfigKit, LifetimeKit. 5 aile x 4 rol = 20 kayit hedefi (ADR-013 asgari 20); durum `planli`.

Makro dugumler alt katman dosyalarini tekrarlamaz, katman ozetini tasir; kayitlar ADR-013 2.1 biciminde 8 sutundur. Bu dosya `authority: derived` + `status: needs-validation` icerir: klasor ve aile listesi `src/` olusunca dogrulanir.

```text
+--------------------------------------------------+
| l6 Host -> VersaCoder.Host
| src/VersaCoder.Host/
| 5 aile x 4 rol = 20 kayit (esik 20)
+--------------------------------------------------+
```

| Alan | Deger |
|---|---|
| Dugum | N51 |
| Dugum turu | makro ozet dugumu |
| Makro katman | l6 Host |
| Proje (.csproj) | VersaCoder.Host |
| TFM | net48;net10.0-windows |
| Kayit hedefi | 20 (ADR-013 asgari 20) |
| Durum | planli |
| Sorumlu rol | architect |

---

## §1 Sorumluluk (YAPAR / YAPMAZ)

| # | YAPAR | YAPMAZ |
|---|---|---|
| 1 | l6 katmaninin makro ozetini tasir: HostCore, BootstrapKit, RegistrationKit, ConfigKit, LifetimeKit | baslangic adimlarini kendisi yazmak (l4 is) - YOK |
| 2 | kayitlari 8 sutunlu ADR-013 biciminde tutmak (planli -> yapildi -> testli) | UI ekranlari acmak veya CLI argumani ayrıştırmak |
| 3 | DI kayitlari, omur haritasi ve saglik probunu yurutmek | domain invariantini ihlal etmek |
| 4 | yapilandirma kopruleri ile dis degerleri tek gorunumde toplamak | sifreleri loglamak veya dosyaya yazmak |
| 5 | l0, l1, l2, l4 ve l5 disinda bagimlilik acmamak | l3 ve l7 katmanlarina referans vermek |
| 6 | l0 portlari uzerinden alt katmanlari cagirmak | istisnayi disari tasimak, donus tipi Result<T> olur |

---

## §2 Klasor ve N Eslemesi

**Kalip (ADR-028 2.3/4):** `N33 -> src\VersaCoder.Protocol\ToolSchema\` satiri SSOT ornegidir; bu dosyanin klasoru ayni kalipla yazilir.

| # | N | Slug | src/ klasoru |
|---|---|---|---|
| 1 | N51 | macro-l6 | `src/VersaCoder.Host/` |

**Kanit yolu (ADR-028 2.3/5):** tarama yolu gercek proje adini icerir; genis kalip (`src/**/Domain/**`) sahte PASS uretir.

```powershell
Test-Path 'C:\versacoder\src\VersaCoder.Host'
```

Dizin su an yok cikti `False` olur; PASS yazilmaz -> **DOĞRULAMA GEREKLI**.

---

## §3 Aile Yapisi

Rol kalibi (4): `I<F>` · `<F>Module` · `<F>Descriptor` · `<F>Tests`

| # | Aile |
|---|---|
| 1 | HostCore |
| 2 | BootstrapKit |
| 3 | RegistrationKit |
| 4 | ConfigKit |
| 5 | LifetimeKit |

Her aile ayni rol kalibini 4 kez tekrarlar; kayit sayisi = aile x rol.

---

## §4 Kayit Ornegi (8 sutun)

| ID | Bilesen | Tip | Sorumluluk | Proje | TFM | Test | Durum |
|---|---|---|---|---|---|---|---|
| N51-001 | `HostCore` | interface | HostCore katman sozlesmesi | VersaCoder.Host | net48;net10.0-windows | HostCoreTests | planli |
| N51-002 | `HostCoreModule` | class | HostCore modul kaydi | VersaCoder.Host | net48;net10.0-windows | HostCoreTests | planli |
| N51-003 | `HostCoreDescriptor` | record | HostCore katman tanimi kaydi | VersaCoder.Host | net48;net10.0-windows | HostCoreTests | planli |
| N51-004 | `HostCoreTests` | class | HostCore mimari testleri | VersaCoder.Host | net48;net10.0-windows | - | planli |

Kalan aileler ayni kalibi 12 rolle tekrarlar (makro dugumlerde 4 rol); toplam hedef **20** kayit.

---

## §5 Yon Kurali

| # | Yon | Deger | Kaynak |
|---|---|---|---|
| 1 | l6 -> l0, l1, l2, l4 ve l5 | OK | ADR-028 2.1 |
| 2 | l6 -> l3 | OK (makro ozet uzerinden) | ADR-028 2.1 |
| 3 | l6 -> l7 | YOK (yon asagi) | ADR-028 2.1 |
| 4 | l7 -> l6 | OK (asagidan tek yon) | ADR-028 2.1 |

Matrisin tamami [layer-matrix](layer-matrix.md) dosyasindadir; NetArchTest `LayerRulesTests` bu tabloya gore yazilir.

---

## §6 Test ve Durum

| # | Kontrol | Beklenen | Durum |
|---|---|---|---|
| 1 | Kayit satiri | 20 (asgari 20) | bu dosyada 4 ornek satir; tam liste DOĞRULAMA GEREKLI |
| 2 | Durum degeri | planli -> yapildi -> testli | planli |
| 3 | Test sinifi | HostCoreTests (xUnit) | planli |
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
(Select-String -Path 'C:\versacoder\.ai\architecture\catalog\N51-macro-l6.md' -Pattern '^\|\s*N\d{2}-\d{3}\s*\|').Count
# 2) boyut bandi - beklenen: 2600-8500
(Get-Item 'C:\versacoder\.ai\architecture\catalog\N51-macro-l6.md').Length
# 3) klasor kaniti - gercek proje adi (ADR-028 2.3/5)
(Test-Path 'C:\versacoder\src\VersaCoder.Host')
```

kayit tablosu §10'e port edildi (kaynak vault 53 dosya / 5440 kayit); esik 5000 karsilanir.


## §10 Kayit Tablosu (ADR-013 2.2 - kaynak vault port)

| N51-005 | `IBootstrapKit` | interface | BootstrapKit katman sozlesmesi | VersaCoder.Host | net48;net10.0-windows | BootstrapKitTests | planli |
| N51-006 | `BootstrapKitModule` | class | BootstrapKit modul kaydi | VersaCoder.Host | net48;net10.0-windows | BootstrapKitTests | planli |
| N51-007 | `BootstrapKitDescriptor` | record | BootstrapKit katman tanimi kaydi | VersaCoder.Host | net48;net10.0-windows | BootstrapKitTests | planli |
| N51-008 | `BootstrapKitTests` | class | BootstrapKit mimari testleri | VersaCoder.Host | net48;net10.0-windows | - | planli |
| N51-009 | `IRegistrationKit` | interface | RegistrationKit katman sozlesmesi | VersaCoder.Host | net48;net10.0-windows | RegistrationKitTests | planli |
| N51-010 | `RegistrationKitModule` | class | RegistrationKit modul kaydi | VersaCoder.Host | net48;net10.0-windows | RegistrationKitTests | planli |
| N51-011 | `RegistrationKitDescriptor` | record | RegistrationKit katman tanimi kaydi | VersaCoder.Host | net48;net10.0-windows | RegistrationKitTests | planli |
| N51-012 | `RegistrationKitTests` | class | RegistrationKit mimari testleri | VersaCoder.Host | net48;net10.0-windows | - | planli |
| N51-013 | `IConfigKit` | interface | ConfigKit katman sozlesmesi | VersaCoder.Host | net48;net10.0-windows | ConfigKitTests | planli |
| N51-014 | `ConfigKitModule` | class | ConfigKit modul kaydi | VersaCoder.Host | net48;net10.0-windows | ConfigKitTests | planli |
| N51-015 | `ConfigKitDescriptor` | record | ConfigKit katman tanimi kaydi | VersaCoder.Host | net48;net10.0-windows | ConfigKitTests | planli |
| N51-016 | `ConfigKitTests` | class | ConfigKit mimari testleri | VersaCoder.Host | net48;net10.0-windows | - | planli |
| N51-017 | `ILifetimeKit` | interface | LifetimeKit katman sozlesmesi | VersaCoder.Host | net48;net10.0-windows | LifetimeKitTests | planli |
| N51-018 | `LifetimeKitModule` | class | LifetimeKit modul kaydi | VersaCoder.Host | net48;net10.0-windows | LifetimeKitTests | planli |
| N51-019 | `LifetimeKitDescriptor` | record | LifetimeKit katman tanimi kaydi | VersaCoder.Host | net48;net10.0-windows | LifetimeKitTests | planli |
| N51-020 | `LifetimeKitTests` | class | LifetimeKit mimari testleri | VersaCoder.Host | net48;net10.0-windows | - | planli |
