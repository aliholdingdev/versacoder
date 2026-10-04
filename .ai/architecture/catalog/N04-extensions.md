---
title: "Katalog Dugumu N04 - Uzanti ve Guard Yardimcilari (l0)"
type: catalog
category: architecture
status: needs-validation
version: 1.0.0
updated: 2026-10-04
authority: derived
scope: "N04 extensions dugumu: kimlik, src klasoru, aile yapisi, ornek kayit ve yon kurali (ADR-013 + ADR-028)"
---

# Katalog Dugumu N04 - Uzanti ve Guard Yardimcilari

**Zorunlu Baglantilar:** [catalog index](index.md) · [node-inventory](node-inventory.md) · [mimari index](../index.md) · [ADR-028](../../.decisions/accepted/ADR-028-layered-baseline-9-projects.md) · [ADR-013](../../.decisions/accepted/ADR-013-layered-architecture-52-nodes.md)

> **Tek cumlelik sorumluluk:** *N04 extensions hangi projede ve klasorde yasar, ne yapar, kac kayit hedefler* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

**N04 (extensions), l0 Abstractions katmaninin alt katmanidir: `VersaCoder.Abstractions` projesinde `src/VersaCoder.Abstractions/Extensions/` klasorunde yasar.** Guard ve extension yardimcilarini verir; hicbir somut bagimlilik getirmez. 10 aile x 12 rol = 120 kayit hedefi (ADR-013 asgari 112); durum `planli`.

Kayitlar ADR-013 2.1 biciminde 8 sutun tasir; bu dosya aile kalibini ve ornek satirlari gosterir, tam 120 satirlik liste bu dosyanin boyut sinirina sigmaz (DOĞRULAMA GEREKLI).

```text
+--------------------------------------------------+
| l0 Abstractions -> VersaCoder.Abstractions
| src/VersaCoder.Abstractions/Extensions/
| 10 aile x 12 rol = 120 kayit (esik 112)
+--------------------------------------------------+
```

| Alan | Deger |
|---|---|
| Dugum | N04 |
| Dugum turu | alt katman (klasor/namespace duzeyi) |
| Makro katman | l0 Abstractions |
| Proje (.csproj) | VersaCoder.Abstractions |
| TFM | net48;net10.0-windows |
| Kayit hedefi | 120 (ADR-013 asgari 112) |
| Durum | planli |
| Sorumlu rol | architect |

---

## §1 Sorumluluk (YAPAR / YAPMAZ)

| # | YAPAR | YAPMAZ |
|---|---|---|
| 1 | Guard ve extension yardimcilarini verir; hicbir somut bagimlilik getirmez | somut uygulama (adapter) sinifi yazmak, uygulama l4 katmaninda |
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
| 1 | N04 | extensions | `src/VersaCoder.Abstractions/Extensions/` |

**Kanit yolu (ADR-028 2.3/5):** tarama yolu gercek proje adini icerir; genis kalip (`src/**/Domain/**`) sahte PASS uretir.

```powershell
Test-Path 'C:\versacoder\src\VersaCoder.Abstractions\Extensions'
```

Dizin su an yok cikti `False` olur; PASS yazilmaz -> **DOĞRULAMA GEREKLI**.

---

## §3 Aile Yapisi

Rol kalibi (12): `I<F>` · `<F>` · `<F>Dto` · `<F>Mapper` · `<F>Validator` · `<F>Factory` · `<F>Options` · `<F>Provider` · `<F>Registry` · `<F>Converter` · `<F>Tests` · `<F>Builder`

| # | Aile | # | Aile |
|---|---|---|---|
| 1 | Guard | 6 | StringExt |
| 2 | ArgumentGuard | 7 | CollectionExt |
| 3 | Check | 8 | TypeExt |
| 4 | Verify | 9 | TaskExt |
| 5 | Ensure | 10 | EnumExt |

Her aile ayni rol kalibini 12 kez tekrarlar; kayit sayisi = aile x rol.

---

## §4 Kayit Ornegi (8 sutun)

| ID | Bilesen | Tip | Sorumluluk | Proje | TFM | Test | Durum |
|---|---|---|---|---|---|---|---|
| N04-001 | `IGuard` | interface | Guard sozlesmesi (port) | VersaCoder.Abstractions | net48;net10.0-windows | GuardTests | planli |
| N04-002 | `Guard` | class | Guard cekirdek uygulamasi | VersaCoder.Abstractions | net48;net10.0-windows | GuardTests | planli |
| N04-003 | `GuardDto` | record | Guard veri transferi modeli | VersaCoder.Abstractions | net48;net10.0-windows | GuardTests | planli |
| N04-004 | `GuardMapper` | class | Guard <-> DTO esleme | VersaCoder.Abstractions | net48;net10.0-windows | GuardTests | planli |
| N04-005 | `GuardValidator` | class | Guard dogrulama kurallari | VersaCoder.Abstractions | net48;net10.0-windows | GuardTests | planli |
| N04-006 | `GuardFactory` | class | Guard uretimi ve varsayilanlari | VersaCoder.Abstractions | net48;net10.0-windows | GuardTests | planli |

Kalan aileler ayni kalibi 12 rolle tekrarlar (makro dugumlerde 4 rol); toplam hedef **120** kayit.

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
| 1 | Kayit satiri | 120 (asgari 112) | bu dosyada 6 ornek satir; tam liste DOĞRULAMA GEREKLI |
| 2 | Durum degeri | planli -> yapildi -> testli | planli |
| 3 | Test sinifi | GuardTests (xUnit) | planli |
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
(Select-String -Path 'C:\versacoder\.ai\architecture\catalog\N04-extensions.md' -Pattern '^\|\s*N\d{2}-\d{3}\s*\|').Count
# 2) boyut bandi - beklenen: 2600-8500
(Get-Item 'C:\versacoder\.ai\architecture\catalog\N04-extensions.md').Length
# 3) klasor kaniti - gercek proje adi (ADR-028 2.3/5)
(Test-Path 'C:\versacoder\src\VersaCoder.Abstractions\Extensions')
```

Katalog sayimi `powershell -NoProfile -ExecutionPolicy Bypass -File C:\versacoder\.ai\scripts\count-components.ps1` ile alinir; bu batch sonunda toplam esik (5000) henuz karsilanmaz - kalan 26 node dosyasi sonraki batchtir.

## §10 Kayit Tablosu (ADR-013 2.2 - kaynak vault port)

| N04-007 | `GuardOptions` | record | Guard yapilandirmasi | VersaCoder.Abstractions | net48;net10.0-windows | GuardTests | planli |
| N04-008 | `GuardProvider` | class | Guard saglayici uygulamasi | VersaCoder.Abstractions | net48;net10.0-windows | GuardTests | planli |
| N04-009 | `GuardRegistry` | class | Guard kayit/katalog yonetimi | VersaCoder.Abstractions | net48;net10.0-windows | GuardTests | planli |
| N04-010 | `GuardConverter` | class | Guard bicim donusumu | VersaCoder.Abstractions | net48;net10.0-windows | GuardTests | planli |
| N04-011 | `GuardTests` | class | Guard testleri (xUnit) | VersaCoder.Abstractions | net48;net10.0-windows | - | planli |
| N04-012 | `GuardBuilder` | class | Guard uretici (builder) | VersaCoder.Abstractions | net48;net10.0-windows | GuardTests | planli |
| N04-013 | `IArgumentGuard` | interface | ArgumentGuard sozlesmesi (port) | VersaCoder.Abstractions | net48;net10.0-windows | ArgumentGuardTests | planli |
| N04-014 | `ArgumentGuard` | class | ArgumentGuard cekirdek uygulamasi | VersaCoder.Abstractions | net48;net10.0-windows | ArgumentGuardTests | planli |
| N04-015 | `ArgumentGuardDto` | record | ArgumentGuard veri transferi modeli | VersaCoder.Abstractions | net48;net10.0-windows | ArgumentGuardTests | planli |
| N04-016 | `ArgumentGuardMapper` | class | ArgumentGuard <-> DTO esleme | VersaCoder.Abstractions | net48;net10.0-windows | ArgumentGuardTests | planli |
| N04-017 | `ArgumentGuardValidator` | class | ArgumentGuard dogrulama kurallari | VersaCoder.Abstractions | net48;net10.0-windows | ArgumentGuardTests | planli |
| N04-018 | `ArgumentGuardFactory` | class | ArgumentGuard uretimi ve varsayilanlari | VersaCoder.Abstractions | net48;net10.0-windows | ArgumentGuardTests | planli |
| N04-019 | `ArgumentGuardOptions` | record | ArgumentGuard yapilandirmasi | VersaCoder.Abstractions | net48;net10.0-windows | ArgumentGuardTests | planli |
| N04-020 | `ArgumentGuardProvider` | class | ArgumentGuard saglayici uygulamasi | VersaCoder.Abstractions | net48;net10.0-windows | ArgumentGuardTests | planli |
| N04-021 | `ArgumentGuardRegistry` | class | ArgumentGuard kayit/katalog yonetimi | VersaCoder.Abstractions | net48;net10.0-windows | ArgumentGuardTests | planli |
| N04-022 | `ArgumentGuardConverter` | class | ArgumentGuard bicim donusumu | VersaCoder.Abstractions | net48;net10.0-windows | ArgumentGuardTests | planli |
| N04-023 | `ArgumentGuardTests` | class | ArgumentGuard testleri (xUnit) | VersaCoder.Abstractions | net48;net10.0-windows | - | planli |
| N04-024 | `ArgumentGuardBuilder` | class | ArgumentGuard uretici (builder) | VersaCoder.Abstractions | net48;net10.0-windows | ArgumentGuardTests | planli |
| N04-025 | `ICheck` | interface | Check sozlesmesi (port) | VersaCoder.Abstractions | net48;net10.0-windows | CheckTests | planli |
| N04-026 | `Check` | class | Check cekirdek uygulamasi | VersaCoder.Abstractions | net48;net10.0-windows | CheckTests | planli |
| N04-027 | `CheckDto` | record | Check veri transferi modeli | VersaCoder.Abstractions | net48;net10.0-windows | CheckTests | planli |
| N04-028 | `CheckMapper` | class | Check <-> DTO esleme | VersaCoder.Abstractions | net48;net10.0-windows | CheckTests | planli |
| N04-029 | `CheckValidator` | class | Check dogrulama kurallari | VersaCoder.Abstractions | net48;net10.0-windows | CheckTests | planli |
| N04-030 | `CheckFactory` | class | Check uretimi ve varsayilanlari | VersaCoder.Abstractions | net48;net10.0-windows | CheckTests | planli |
| N04-031 | `CheckOptions` | record | Check yapilandirmasi | VersaCoder.Abstractions | net48;net10.0-windows | CheckTests | planli |
| N04-032 | `CheckProvider` | class | Check saglayici uygulamasi | VersaCoder.Abstractions | net48;net10.0-windows | CheckTests | planli |
| N04-033 | `CheckRegistry` | class | Check kayit/katalog yonetimi | VersaCoder.Abstractions | net48;net10.0-windows | CheckTests | planli |
| N04-034 | `CheckConverter` | class | Check bicim donusumu | VersaCoder.Abstractions | net48;net10.0-windows | CheckTests | planli |
| N04-035 | `CheckTests` | class | Check testleri (xUnit) | VersaCoder.Abstractions | net48;net10.0-windows | - | planli |
| N04-036 | `CheckBuilder` | class | Check uretici (builder) | VersaCoder.Abstractions | net48;net10.0-windows | CheckTests | planli |
| N04-037 | `IVerify` | interface | Verify sozlesmesi (port) | VersaCoder.Abstractions | net48;net10.0-windows | VerifyTests | planli |
| N04-038 | `Verify` | class | Verify cekirdek uygulamasi | VersaCoder.Abstractions | net48;net10.0-windows | VerifyTests | planli |
| N04-039 | `VerifyDto` | record | Verify veri transferi modeli | VersaCoder.Abstractions | net48;net10.0-windows | VerifyTests | planli |
| N04-040 | `VerifyMapper` | class | Verify <-> DTO esleme | VersaCoder.Abstractions | net48;net10.0-windows | VerifyTests | planli |
| N04-041 | `VerifyValidator` | class | Verify dogrulama kurallari | VersaCoder.Abstractions | net48;net10.0-windows | VerifyTests | planli |
| N04-042 | `VerifyFactory` | class | Verify uretimi ve varsayilanlari | VersaCoder.Abstractions | net48;net10.0-windows | VerifyTests | planli |
| N04-043 | `VerifyOptions` | record | Verify yapilandirmasi | VersaCoder.Abstractions | net48;net10.0-windows | VerifyTests | planli |
| N04-044 | `VerifyProvider` | class | Verify saglayici uygulamasi | VersaCoder.Abstractions | net48;net10.0-windows | VerifyTests | planli |
| N04-045 | `VerifyRegistry` | class | Verify kayit/katalog yonetimi | VersaCoder.Abstractions | net48;net10.0-windows | VerifyTests | planli |
| N04-046 | `VerifyConverter` | class | Verify bicim donusumu | VersaCoder.Abstractions | net48;net10.0-windows | VerifyTests | planli |
| N04-047 | `VerifyTests` | class | Verify testleri (xUnit) | VersaCoder.Abstractions | net48;net10.0-windows | - | planli |
| N04-048 | `VerifyBuilder` | class | Verify uretici (builder) | VersaCoder.Abstractions | net48;net10.0-windows | VerifyTests | planli |
| N04-049 | `IEnsure` | interface | Ensure sozlesmesi (port) | VersaCoder.Abstractions | net48;net10.0-windows | EnsureTests | planli |
| N04-050 | `Ensure` | class | Ensure cekirdek uygulamasi | VersaCoder.Abstractions | net48;net10.0-windows | EnsureTests | planli |
| N04-051 | `EnsureDto` | record | Ensure veri transferi modeli | VersaCoder.Abstractions | net48;net10.0-windows | EnsureTests | planli |
| N04-052 | `EnsureMapper` | class | Ensure <-> DTO esleme | VersaCoder.Abstractions | net48;net10.0-windows | EnsureTests | planli |
| N04-053 | `EnsureValidator` | class | Ensure dogrulama kurallari | VersaCoder.Abstractions | net48;net10.0-windows | EnsureTests | planli |
| N04-054 | `EnsureFactory` | class | Ensure uretimi ve varsayilanlari | VersaCoder.Abstractions | net48;net10.0-windows | EnsureTests | planli |
| N04-055 | `EnsureOptions` | record | Ensure yapilandirmasi | VersaCoder.Abstractions | net48;net10.0-windows | EnsureTests | planli |
| N04-056 | `EnsureProvider` | class | Ensure saglayici uygulamasi | VersaCoder.Abstractions | net48;net10.0-windows | EnsureTests | planli |
| N04-057 | `EnsureRegistry` | class | Ensure kayit/katalog yonetimi | VersaCoder.Abstractions | net48;net10.0-windows | EnsureTests | planli |
| N04-058 | `EnsureConverter` | class | Ensure bicim donusumu | VersaCoder.Abstractions | net48;net10.0-windows | EnsureTests | planli |
| N04-059 | `EnsureTests` | class | Ensure testleri (xUnit) | VersaCoder.Abstractions | net48;net10.0-windows | - | planli |
| N04-060 | `EnsureBuilder` | class | Ensure uretici (builder) | VersaCoder.Abstractions | net48;net10.0-windows | EnsureTests | planli |
| N04-061 | `IStringExt` | interface | StringExt sozlesmesi (port) | VersaCoder.Abstractions | net48;net10.0-windows | StringExtTests | planli |
| N04-062 | `StringExt` | class | StringExt cekirdek uygulamasi | VersaCoder.Abstractions | net48;net10.0-windows | StringExtTests | planli |
| N04-063 | `StringExtDto` | record | StringExt veri transferi modeli | VersaCoder.Abstractions | net48;net10.0-windows | StringExtTests | planli |
| N04-064 | `StringExtMapper` | class | StringExt <-> DTO esleme | VersaCoder.Abstractions | net48;net10.0-windows | StringExtTests | planli |
| N04-065 | `StringExtValidator` | class | StringExt dogrulama kurallari | VersaCoder.Abstractions | net48;net10.0-windows | StringExtTests | planli |
| N04-066 | `StringExtFactory` | class | StringExt uretimi ve varsayilanlari | VersaCoder.Abstractions | net48;net10.0-windows | StringExtTests | planli |
| N04-067 | `StringExtOptions` | record | StringExt yapilandirmasi | VersaCoder.Abstractions | net48;net10.0-windows | StringExtTests | planli |
| N04-068 | `StringExtProvider` | class | StringExt saglayici uygulamasi | VersaCoder.Abstractions | net48;net10.0-windows | StringExtTests | planli |
| N04-069 | `StringExtRegistry` | class | StringExt kayit/katalog yonetimi | VersaCoder.Abstractions | net48;net10.0-windows | StringExtTests | planli |
| N04-070 | `StringExtConverter` | class | StringExt bicim donusumu | VersaCoder.Abstractions | net48;net10.0-windows | StringExtTests | planli |
| N04-071 | `StringExtTests` | class | StringExt testleri (xUnit) | VersaCoder.Abstractions | net48;net10.0-windows | - | planli |
| N04-072 | `StringExtBuilder` | class | StringExt uretici (builder) | VersaCoder.Abstractions | net48;net10.0-windows | StringExtTests | planli |
| N04-073 | `ICollectionExt` | interface | CollectionExt sozlesmesi (port) | VersaCoder.Abstractions | net48;net10.0-windows | CollectionExtTests | planli |
| N04-074 | `CollectionExt` | class | CollectionExt cekirdek uygulamasi | VersaCoder.Abstractions | net48;net10.0-windows | CollectionExtTests | planli |
| N04-075 | `CollectionExtDto` | record | CollectionExt veri transferi modeli | VersaCoder.Abstractions | net48;net10.0-windows | CollectionExtTests | planli |
| N04-076 | `CollectionExtMapper` | class | CollectionExt <-> DTO esleme | VersaCoder.Abstractions | net48;net10.0-windows | CollectionExtTests | planli |
| N04-077 | `CollectionExtValidator` | class | CollectionExt dogrulama kurallari | VersaCoder.Abstractions | net48;net10.0-windows | CollectionExtTests | planli |
| N04-078 | `CollectionExtFactory` | class | CollectionExt uretimi ve varsayilanlari | VersaCoder.Abstractions | net48;net10.0-windows | CollectionExtTests | planli |
| N04-079 | `CollectionExtOptions` | record | CollectionExt yapilandirmasi | VersaCoder.Abstractions | net48;net10.0-windows | CollectionExtTests | planli |
| N04-080 | `CollectionExtProvider` | class | CollectionExt saglayici uygulamasi | VersaCoder.Abstractions | net48;net10.0-windows | CollectionExtTests | planli |
| N04-081 | `CollectionExtRegistry` | class | CollectionExt kayit/katalog yonetimi | VersaCoder.Abstractions | net48;net10.0-windows | CollectionExtTests | planli |
| N04-082 | `CollectionExtConverter` | class | CollectionExt bicim donusumu | VersaCoder.Abstractions | net48;net10.0-windows | CollectionExtTests | planli |
| N04-083 | `CollectionExtTests` | class | CollectionExt testleri (xUnit) | VersaCoder.Abstractions | net48;net10.0-windows | - | planli |
| N04-084 | `CollectionExtBuilder` | class | CollectionExt uretici (builder) | VersaCoder.Abstractions | net48;net10.0-windows | CollectionExtTests | planli |
| N04-085 | `ITypeExt` | interface | TypeExt sozlesmesi (port) | VersaCoder.Abstractions | net48;net10.0-windows | TypeExtTests | planli |
| N04-086 | `TypeExt` | class | TypeExt cekirdek uygulamasi | VersaCoder.Abstractions | net48;net10.0-windows | TypeExtTests | planli |
| N04-087 | `TypeExtDto` | record | TypeExt veri transferi modeli | VersaCoder.Abstractions | net48;net10.0-windows | TypeExtTests | planli |
| N04-088 | `TypeExtMapper` | class | TypeExt <-> DTO esleme | VersaCoder.Abstractions | net48;net10.0-windows | TypeExtTests | planli |
| N04-089 | `TypeExtValidator` | class | TypeExt dogrulama kurallari | VersaCoder.Abstractions | net48;net10.0-windows | TypeExtTests | planli |
| N04-090 | `TypeExtFactory` | class | TypeExt uretimi ve varsayilanlari | VersaCoder.Abstractions | net48;net10.0-windows | TypeExtTests | planli |
| N04-091 | `TypeExtOptions` | record | TypeExt yapilandirmasi | VersaCoder.Abstractions | net48;net10.0-windows | TypeExtTests | planli |
| N04-092 | `TypeExtProvider` | class | TypeExt saglayici uygulamasi | VersaCoder.Abstractions | net48;net10.0-windows | TypeExtTests | planli |
| N04-093 | `TypeExtRegistry` | class | TypeExt kayit/katalog yonetimi | VersaCoder.Abstractions | net48;net10.0-windows | TypeExtTests | planli |
| N04-094 | `TypeExtConverter` | class | TypeExt bicim donusumu | VersaCoder.Abstractions | net48;net10.0-windows | TypeExtTests | planli |
| N04-095 | `TypeExtTests` | class | TypeExt testleri (xUnit) | VersaCoder.Abstractions | net48;net10.0-windows | - | planli |
| N04-096 | `TypeExtBuilder` | class | TypeExt uretici (builder) | VersaCoder.Abstractions | net48;net10.0-windows | TypeExtTests | planli |
| N04-097 | `ITaskExt` | interface | TaskExt sozlesmesi (port) | VersaCoder.Abstractions | net48;net10.0-windows | TaskExtTests | planli |
| N04-098 | `TaskExt` | class | TaskExt cekirdek uygulamasi | VersaCoder.Abstractions | net48;net10.0-windows | TaskExtTests | planli |
| N04-099 | `TaskExtDto` | record | TaskExt veri transferi modeli | VersaCoder.Abstractions | net48;net10.0-windows | TaskExtTests | planli |
| N04-100 | `TaskExtMapper` | class | TaskExt <-> DTO esleme | VersaCoder.Abstractions | net48;net10.0-windows | TaskExtTests | planli |
| N04-101 | `TaskExtValidator` | class | TaskExt dogrulama kurallari | VersaCoder.Abstractions | net48;net10.0-windows | TaskExtTests | planli |
| N04-102 | `TaskExtFactory` | class | TaskExt uretimi ve varsayilanlari | VersaCoder.Abstractions | net48;net10.0-windows | TaskExtTests | planli |
| N04-103 | `TaskExtOptions` | record | TaskExt yapilandirmasi | VersaCoder.Abstractions | net48;net10.0-windows | TaskExtTests | planli |
| N04-104 | `TaskExtProvider` | class | TaskExt saglayici uygulamasi | VersaCoder.Abstractions | net48;net10.0-windows | TaskExtTests | planli |
| N04-105 | `TaskExtRegistry` | class | TaskExt kayit/katalog yonetimi | VersaCoder.Abstractions | net48;net10.0-windows | TaskExtTests | planli |
| N04-106 | `TaskExtConverter` | class | TaskExt bicim donusumu | VersaCoder.Abstractions | net48;net10.0-windows | TaskExtTests | planli |
| N04-107 | `TaskExtTests` | class | TaskExt testleri (xUnit) | VersaCoder.Abstractions | net48;net10.0-windows | - | planli |
| N04-108 | `TaskExtBuilder` | class | TaskExt uretici (builder) | VersaCoder.Abstractions | net48;net10.0-windows | TaskExtTests | planli |
| N04-109 | `IEnumExt` | interface | EnumExt sozlesmesi (port) | VersaCoder.Abstractions | net48;net10.0-windows | EnumExtTests | planli |
| N04-110 | `EnumExt` | class | EnumExt cekirdek uygulamasi | VersaCoder.Abstractions | net48;net10.0-windows | EnumExtTests | planli |
| N04-111 | `EnumExtDto` | record | EnumExt veri transferi modeli | VersaCoder.Abstractions | net48;net10.0-windows | EnumExtTests | planli |
| N04-112 | `EnumExtMapper` | class | EnumExt <-> DTO esleme | VersaCoder.Abstractions | net48;net10.0-windows | EnumExtTests | planli |
| N04-113 | `EnumExtValidator` | class | EnumExt dogrulama kurallari | VersaCoder.Abstractions | net48;net10.0-windows | EnumExtTests | planli |
| N04-114 | `EnumExtFactory` | class | EnumExt uretimi ve varsayilanlari | VersaCoder.Abstractions | net48;net10.0-windows | EnumExtTests | planli |
| N04-115 | `EnumExtOptions` | record | EnumExt yapilandirmasi | VersaCoder.Abstractions | net48;net10.0-windows | EnumExtTests | planli |
| N04-116 | `EnumExtProvider` | class | EnumExt saglayici uygulamasi | VersaCoder.Abstractions | net48;net10.0-windows | EnumExtTests | planli |
| N04-117 | `EnumExtRegistry` | class | EnumExt kayit/katalog yonetimi | VersaCoder.Abstractions | net48;net10.0-windows | EnumExtTests | planli |
| N04-118 | `EnumExtConverter` | class | EnumExt bicim donusumu | VersaCoder.Abstractions | net48;net10.0-windows | EnumExtTests | planli |
| N04-119 | `EnumExtTests` | class | EnumExt testleri (xUnit) | VersaCoder.Abstractions | net48;net10.0-windows | - | planli |
| N04-120 | `EnumExtBuilder` | class | EnumExt uretici (builder) | VersaCoder.Abstractions | net48;net10.0-windows | EnumExtTests | planli |
