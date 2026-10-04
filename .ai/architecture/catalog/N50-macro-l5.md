---
title: "Katalog Dugumu N50 - Makro l5 Protocol (l5)"
type: catalog
category: architecture
status: needs-validation
version: 1.0.0
updated: 2026-10-04
authority: derived
scope: "N50 macro-l5 dugumu: kimlik, src klasoru, aile yapisi, ornek kayit ve yon kurali (ADR-013 + ADR-028)"
---

# Katalog Dugumu N50 - Makro l5 Protocol

**Zorunlu Baglantilar:** [catalog index](index.md) · [node-inventory](node-inventory.md) · [mimari index](../index.md) · [ADR-028](../../.decisions/accepted/ADR-028-layered-baseline-9-projects.md) · [ADR-013](../../.decisions/accepted/ADR-013-layered-architecture-52-nodes.md)

> **Tek cumlelik sorumluluk:** *N50 macro-l5 hangi projede ve klasorde yasar, ne yapar, kac kayit hedefler* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

**N50 (macro-l5), l5 Protocol katmaninin makro ozet dugumudur: `VersaCoder.Protocol` proje kokunde yasar.** l5 Protocol katmaninin makro ozetini tasir: ProtocolCore, ToolSchemaKit, MessageKit, SessionKit, LoopKit. 5 aile x 4 rol = 20 kayit hedefi (ADR-013 asgari 20); durum `planli`.

Makro dugumler alt katman dosyalarini tekrarlamaz, katman ozetini tasir; kayitlar ADR-013 2.1 biciminde 8 sutundur. Bu dosya `authority: derived` + `status: needs-validation` icerir: klasor ve aile listesi `src/` olusunca dogrulanir.

```text
+--------------------------------------------------+
| l5 Protocol -> VersaCoder.Protocol
| src/VersaCoder.Protocol/
| 5 aile x 4 rol = 20 kayit (esik 20)
+--------------------------------------------------+
```

| Alan | Deger |
|---|---|
| Dugum | N50 |
| Dugum turu | makro ozet dugumu |
| Makro katman | l5 Protocol |
| Proje (.csproj) | VersaCoder.Protocol |
| TFM | net48;net10.0-windows |
| Kayit hedefi | 20 (ADR-013 asgari 20) |
| Durum | planli |
| Sorumlu rol | architect |

---

## §1 Sorumluluk (YAPAR / YAPMAZ)

| # | YAPAR | YAPMAZ |
|---|---|---|
| 1 | l5 katmaninin makro ozetini tasir: ProtocolCore, ToolSchemaKit, MessageKit, SessionKit, LoopKit | arac cagrisini kendisi calistirmak (l4 is) - YOK |
| 2 | kayitlari 8 sutunlu ADR-013 biciminde tutmak (planli -> yapildi -> testli) | ag uzerinden istek gondermek |
| 3 | protokol kurallari, mesaj zarfi ve dongu sozlesmesini tanimlamak | domain invariantini ihlal etmek |
| 4 | surdurme ve hata katalogunu yurutmek | log, cache veya guvenlik sifresi uretmek |
| 5 | l0, l1, l2 ve l4 disinda bagimlilik acmamak | l3, l6 ve l7 katmanlarina referans vermek |
| 6 | l0 portlari uzerinden alt katmanlari cagirmak | istisnayi disari tasimak, donus tipi Result<T> olur |

---

## §2 Klasor ve N Eslemesi

**Kalip (ADR-028 2.3/4):** `N33 -> src\VersaCoder.Protocol\ToolSchema\` satiri SSOT ornegidir; bu dosyanin klasoru ayni kalipla yazilir.

| # | N | Slug | src/ klasoru |
|---|---|---|---|
| 1 | N50 | macro-l5 | `src/VersaCoder.Protocol/` |

**Kanit yolu (ADR-028 2.3/5):** tarama yolu gercek proje adini icerir; genis kalip (`src/**/Domain/**`) sahte PASS uretir.

```powershell
Test-Path 'C:\versacoder\src\VersaCoder.Protocol'
```

Dizin su an yok cikti `False` olur; PASS yazilmaz -> **DOĞRULAMA GEREKLI**.

---

## §3 Aile Yapisi

Rol kalibi (4): `I<F>` · `<F>Module` · `<F>Descriptor` · `<F>Tests`

| # | Aile |
|---|---|
| 1 | ProtocolCore |
| 2 | ToolSchemaKit |
| 3 | MessageKit |
| 4 | SessionKit |
| 5 | LoopKit |

Her aile ayni rol kalibini 4 kez tekrarlar; kayit sayisi = aile x rol.

---

## §4 Kayit Ornegi (8 sutun)

| ID | Bilesen | Tip | Sorumluluk | Proje | TFM | Test | Durum |
|---|---|---|---|---|---|---|---|
| N50-001 | `ProtocolCore` | interface | ProtocolCore katman sozlesmesi | VersaCoder.Protocol | net48;net10.0-windows | ProtocolCoreTests | planli |
| N50-002 | `ProtocolCoreModule` | class | ProtocolCore modul kaydi | VersaCoder.Protocol | net48;net10.0-windows | ProtocolCoreTests | planli |
| N50-003 | `ProtocolCoreDescriptor` | record | ProtocolCore katman tanimi kaydi | VersaCoder.Protocol | net48;net10.0-windows | ProtocolCoreTests | planli |
| N50-004 | `ProtocolCoreTests` | class | ProtocolCore mimari testleri | VersaCoder.Protocol | net48;net10.0-windows | - | planli |

Kalan aileler ayni kalibi 12 rolle tekrarlar (makro dugumlerde 4 rol); toplam hedef **20** kayit.

---

## §5 Yon Kurali

| # | Yon | Deger | Kaynak |
|---|---|---|---|
| 1 | l5 -> l0, l1, l2 ve l4 | OK | ADR-028 2.1 |
| 2 | l5 -> l3 | YOK (yatay kesme servisi) | ADR-028 2.1 |
| 3 | l5 -> l6, l7 | YOK (yon asagi) | ADR-028 2.1 |
| 4 | l6 -> l5 | OK (asagidan tek yon) | ADR-028 2.1 |

Matrisin tamami [layer-matrix](layer-matrix.md) dosyasindadir; NetArchTest `LayerRulesTests` bu tabloya gore yazilir.

---

## §6 Test ve Durum

| # | Kontrol | Beklenen | Durum |
|---|---|---|---|
| 1 | Kayit satiri | 20 (asgari 20) | bu dosyada 4 ornek satir; tam liste DOĞRULAMA GEREKLI |
| 2 | Durum degeri | planli -> yapildi -> testli | planli |
| 3 | Test sinifi | ProtocolCoreTests (xUnit) | planli |
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
(Select-String -Path 'C:\versacoder\.ai\architecture\catalog\N50-macro-l5.md' -Pattern '^\|\s*N\d{2}-\d{3}\s*\|').Count
# 2) boyut bandi - beklenen: 2600-8500
(Get-Item 'C:\versacoder\.ai\architecture\catalog\N50-macro-l5.md').Length
# 3) klasor kaniti - gercek proje adi (ADR-028 2.3/5)
(Test-Path 'C:\versacoder\src\VersaCoder.Protocol')
```

kayit tablosu §10'e port edildi (kaynak vault 53 dosya / 5440 kayit); esik 5000 karsilanir.


## §10 Kayit Tablosu (ADR-013 2.2 - kaynak vault port)

| N50-005 | `IToolSchemaKit` | interface | ToolSchemaKit katman sozlesmesi | VersaCoder.Protocol | net48;net10.0-windows | ToolSchemaKitTests | planli |
| N50-006 | `ToolSchemaKitModule` | class | ToolSchemaKit modul kaydi | VersaCoder.Protocol | net48;net10.0-windows | ToolSchemaKitTests | planli |
| N50-007 | `ToolSchemaKitDescriptor` | record | ToolSchemaKit katman tanimi kaydi | VersaCoder.Protocol | net48;net10.0-windows | ToolSchemaKitTests | planli |
| N50-008 | `ToolSchemaKitTests` | class | ToolSchemaKit mimari testleri | VersaCoder.Protocol | net48;net10.0-windows | - | planli |
| N50-009 | `IMessageKit` | interface | MessageKit katman sozlesmesi | VersaCoder.Protocol | net48;net10.0-windows | MessageKitTests | planli |
| N50-010 | `MessageKitModule` | class | MessageKit modul kaydi | VersaCoder.Protocol | net48;net10.0-windows | MessageKitTests | planli |
| N50-011 | `MessageKitDescriptor` | record | MessageKit katman tanimi kaydi | VersaCoder.Protocol | net48;net10.0-windows | MessageKitTests | planli |
| N50-012 | `MessageKitTests` | class | MessageKit mimari testleri | VersaCoder.Protocol | net48;net10.0-windows | - | planli |
| N50-013 | `ISessionKit` | interface | SessionKit katman sozlesmesi | VersaCoder.Protocol | net48;net10.0-windows | SessionKitTests | planli |
| N50-014 | `SessionKitModule` | class | SessionKit modul kaydi | VersaCoder.Protocol | net48;net10.0-windows | SessionKitTests | planli |
| N50-015 | `SessionKitDescriptor` | record | SessionKit katman tanimi kaydi | VersaCoder.Protocol | net48;net10.0-windows | SessionKitTests | planli |
| N50-016 | `SessionKitTests` | class | SessionKit mimari testleri | VersaCoder.Protocol | net48;net10.0-windows | - | planli |
| N50-017 | `ILoopKit` | interface | LoopKit katman sozlesmesi | VersaCoder.Protocol | net48;net10.0-windows | LoopKitTests | planli |
| N50-018 | `LoopKitModule` | class | LoopKit modul kaydi | VersaCoder.Protocol | net48;net10.0-windows | LoopKitTests | planli |
| N50-019 | `LoopKitDescriptor` | record | LoopKit katman tanimi kaydi | VersaCoder.Protocol | net48;net10.0-windows | LoopKitTests | planli |
| N50-020 | `LoopKitTests` | class | LoopKit mimari testleri | VersaCoder.Protocol | net48;net10.0-windows | - | planli |
