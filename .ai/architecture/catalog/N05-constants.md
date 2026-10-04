---
title: "Katalog Dugumu N05 - Sabitler (l0)"
type: catalog
category: architecture
status: needs-validation
version: 1.0.0
updated: 2026-10-04
authority: derived
scope: "N05 constants dugumu: kimlik, src klasoru, aile yapisi, ornek kayit ve yon kurali (ADR-013 + ADR-028)"
---

# Katalog Dugumu N05 - Sabitler

**Zorunlu Baglantilar:** [catalog index](index.md) · [node-inventory](node-inventory.md) · [mimari index](../index.md) · [ADR-028](../../.decisions/accepted/ADR-028-layered-baseline-9-projects.md) · [ADR-013](../../.decisions/accepted/ADR-013-layered-architecture-52-nodes.md)

> **Tek cumlelik sorumluluk:** *N05 constants hangi projede ve klasorde yasar, ne yapar, kac kayit hedefler* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

**N05 (constants), l0 Abstractions katmaninin alt katmanidir: `VersaCoder.Abstractions` projesinde `src/VersaCoder.Abstractions/Constants/` klasorunde yasar.** Tablo, kolon, komut ve olay adlari gibi sabitleri tek yerde toplar. 10 aile x 12 rol = 120 kayit hedefi (ADR-013 asgari 112); durum `planli`.

Kayitlar ADR-013 2.1 biciminde 8 sutun tasir; bu dosya aile kalibini ve ornek satirlari gosterir, tam 120 satirlik liste bu dosyanin boyut sinirina sigmaz (DOĞRULAMA GEREKLI).

```text
+--------------------------------------------------+
| l0 Abstractions -> VersaCoder.Abstractions
| src/VersaCoder.Abstractions/Constants/
| 10 aile x 12 rol = 120 kayit (esik 112)
+--------------------------------------------------+
```

| Alan | Deger |
|---|---|
| Dugum | N05 |
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
| 1 | Tablo, kolon, komut ve olay adlari gibi sabitleri tek yerde toplar | somut uygulama (adapter) sinifi yazmak, uygulama l4 katmaninda |
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
| 1 | N05 | constants | `src/VersaCoder.Abstractions/Constants/` |

**Kanit yolu (ADR-028 2.3/5):** tarama yolu gercek proje adini icerir; genis kalip (`src/**/Domain/**`) sahte PASS uretir.

```powershell
Test-Path 'C:\versacoder\src\VersaCoder.Abstractions\Constants'
```

Dizin su an yok cikti `False` olur; PASS yazilmaz -> **DOĞRULAMA GEREKLI**.

---

## §3 Aile Yapisi

Rol kalibi (12): `I<F>` · `<F>` · `<F>Dto` · `<F>Mapper` · `<F>Validator` · `<F>Factory` · `<F>Options` · `<F>Provider` · `<F>Registry` · `<F>Converter` · `<F>Tests` · `<F>Builder`

| # | Aile | # | Aile |
|---|---|---|---|
| 1 | AppConstants | 6 | CommandNames |
| 2 | ErrorCodes | 7 | EventNames |
| 3 | TableNames | 8 | StatusNames |
| 4 | ColumnNames | 9 | HeaderNames |
| 5 | RouteNames | 10 | PathConstants |

Her aile ayni rol kalibini 12 kez tekrarlar; kayit sayisi = aile x rol.

---

## §4 Kayit Ornegi (8 sutun)

| ID | Bilesen | Tip | Sorumluluk | Proje | TFM | Test | Durum |
|---|---|---|---|---|---|---|---|
| N05-001 | `IAppConstants` | interface | AppConstants sozlesmesi (port) | VersaCoder.Abstractions | net48;net10.0-windows | AppConstantsTests | planli |
| N05-002 | `AppConstants` | class | AppConstants cekirdek uygulamasi | VersaCoder.Abstractions | net48;net10.0-windows | AppConstantsTests | planli |
| N05-003 | `AppConstantsDto` | record | AppConstants veri transferi modeli | VersaCoder.Abstractions | net48;net10.0-windows | AppConstantsTests | planli |
| N05-004 | `AppConstantsMapper` | class | AppConstants <-> DTO esleme | VersaCoder.Abstractions | net48;net10.0-windows | AppConstantsTests | planli |
| N05-005 | `AppConstantsValidator` | class | AppConstants dogrulama kurallari | VersaCoder.Abstractions | net48;net10.0-windows | AppConstantsTests | planli |
| N05-006 | `AppConstantsFactory` | class | AppConstants uretimi ve varsayilanlari | VersaCoder.Abstractions | net48;net10.0-windows | AppConstantsTests | planli |

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
| 3 | Test sinifi | AppConstantsTests (xUnit) | planli |
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
(Select-String -Path 'C:\versacoder\.ai\architecture\catalog\N05-constants.md' -Pattern '^\|\s*N\d{2}-\d{3}\s*\|').Count
# 2) boyut bandi - beklenen: 2600-8500
(Get-Item 'C:\versacoder\.ai\architecture\catalog\N05-constants.md').Length
# 3) klasor kaniti - gercek proje adi (ADR-028 2.3/5)
(Test-Path 'C:\versacoder\src\VersaCoder.Abstractions\Constants')
```

Katalog sayimi `powershell -NoProfile -ExecutionPolicy Bypass -File C:\versacoder\.ai\scripts\count-components.ps1` ile alinir; bu batch sonunda toplam esik (5000) henuz karsilanmaz - kalan 26 node dosyasi sonraki batchtir.

## §10 Kayit Tablosu (ADR-013 2.2 - kaynak vault port)

| N05-007 | `AppConstantsOptions` | record | AppConstants yapilandirmasi | VersaCoder.Abstractions | net48;net10.0-windows | AppConstantsTests | planli |
| N05-008 | `AppConstantsProvider` | class | AppConstants saglayici uygulamasi | VersaCoder.Abstractions | net48;net10.0-windows | AppConstantsTests | planli |
| N05-009 | `AppConstantsRegistry` | class | AppConstants kayit/katalog yonetimi | VersaCoder.Abstractions | net48;net10.0-windows | AppConstantsTests | planli |
| N05-010 | `AppConstantsConverter` | class | AppConstants bicim donusumu | VersaCoder.Abstractions | net48;net10.0-windows | AppConstantsTests | planli |
| N05-011 | `AppConstantsTests` | class | AppConstants testleri (xUnit) | VersaCoder.Abstractions | net48;net10.0-windows | - | planli |
| N05-012 | `AppConstantsBuilder` | class | AppConstants uretici (builder) | VersaCoder.Abstractions | net48;net10.0-windows | AppConstantsTests | planli |
| N05-013 | `IErrorCodes` | interface | ErrorCodes sozlesmesi (port) | VersaCoder.Abstractions | net48;net10.0-windows | ErrorCodesTests | planli |
| N05-014 | `ErrorCodes` | class | ErrorCodes cekirdek uygulamasi | VersaCoder.Abstractions | net48;net10.0-windows | ErrorCodesTests | planli |
| N05-015 | `ErrorCodesDto` | record | ErrorCodes veri transferi modeli | VersaCoder.Abstractions | net48;net10.0-windows | ErrorCodesTests | planli |
| N05-016 | `ErrorCodesMapper` | class | ErrorCodes <-> DTO esleme | VersaCoder.Abstractions | net48;net10.0-windows | ErrorCodesTests | planli |
| N05-017 | `ErrorCodesValidator` | class | ErrorCodes dogrulama kurallari | VersaCoder.Abstractions | net48;net10.0-windows | ErrorCodesTests | planli |
| N05-018 | `ErrorCodesFactory` | class | ErrorCodes uretimi ve varsayilanlari | VersaCoder.Abstractions | net48;net10.0-windows | ErrorCodesTests | planli |
| N05-019 | `ErrorCodesOptions` | record | ErrorCodes yapilandirmasi | VersaCoder.Abstractions | net48;net10.0-windows | ErrorCodesTests | planli |
| N05-020 | `ErrorCodesProvider` | class | ErrorCodes saglayici uygulamasi | VersaCoder.Abstractions | net48;net10.0-windows | ErrorCodesTests | planli |
| N05-021 | `ErrorCodesRegistry` | class | ErrorCodes kayit/katalog yonetimi | VersaCoder.Abstractions | net48;net10.0-windows | ErrorCodesTests | planli |
| N05-022 | `ErrorCodesConverter` | class | ErrorCodes bicim donusumu | VersaCoder.Abstractions | net48;net10.0-windows | ErrorCodesTests | planli |
| N05-023 | `ErrorCodesTests` | class | ErrorCodes testleri (xUnit) | VersaCoder.Abstractions | net48;net10.0-windows | - | planli |
| N05-024 | `ErrorCodesBuilder` | class | ErrorCodes uretici (builder) | VersaCoder.Abstractions | net48;net10.0-windows | ErrorCodesTests | planli |
| N05-025 | `ITableNames` | interface | TableNames sozlesmesi (port) | VersaCoder.Abstractions | net48;net10.0-windows | TableNamesTests | planli |
| N05-026 | `TableNames` | class | TableNames cekirdek uygulamasi | VersaCoder.Abstractions | net48;net10.0-windows | TableNamesTests | planli |
| N05-027 | `TableNamesDto` | record | TableNames veri transferi modeli | VersaCoder.Abstractions | net48;net10.0-windows | TableNamesTests | planli |
| N05-028 | `TableNamesMapper` | class | TableNames <-> DTO esleme | VersaCoder.Abstractions | net48;net10.0-windows | TableNamesTests | planli |
| N05-029 | `TableNamesValidator` | class | TableNames dogrulama kurallari | VersaCoder.Abstractions | net48;net10.0-windows | TableNamesTests | planli |
| N05-030 | `TableNamesFactory` | class | TableNames uretimi ve varsayilanlari | VersaCoder.Abstractions | net48;net10.0-windows | TableNamesTests | planli |
| N05-031 | `TableNamesOptions` | record | TableNames yapilandirmasi | VersaCoder.Abstractions | net48;net10.0-windows | TableNamesTests | planli |
| N05-032 | `TableNamesProvider` | class | TableNames saglayici uygulamasi | VersaCoder.Abstractions | net48;net10.0-windows | TableNamesTests | planli |
| N05-033 | `TableNamesRegistry` | class | TableNames kayit/katalog yonetimi | VersaCoder.Abstractions | net48;net10.0-windows | TableNamesTests | planli |
| N05-034 | `TableNamesConverter` | class | TableNames bicim donusumu | VersaCoder.Abstractions | net48;net10.0-windows | TableNamesTests | planli |
| N05-035 | `TableNamesTests` | class | TableNames testleri (xUnit) | VersaCoder.Abstractions | net48;net10.0-windows | - | planli |
| N05-036 | `TableNamesBuilder` | class | TableNames uretici (builder) | VersaCoder.Abstractions | net48;net10.0-windows | TableNamesTests | planli |
| N05-037 | `IColumnNames` | interface | ColumnNames sozlesmesi (port) | VersaCoder.Abstractions | net48;net10.0-windows | ColumnNamesTests | planli |
| N05-038 | `ColumnNames` | class | ColumnNames cekirdek uygulamasi | VersaCoder.Abstractions | net48;net10.0-windows | ColumnNamesTests | planli |
| N05-039 | `ColumnNamesDto` | record | ColumnNames veri transferi modeli | VersaCoder.Abstractions | net48;net10.0-windows | ColumnNamesTests | planli |
| N05-040 | `ColumnNamesMapper` | class | ColumnNames <-> DTO esleme | VersaCoder.Abstractions | net48;net10.0-windows | ColumnNamesTests | planli |
| N05-041 | `ColumnNamesValidator` | class | ColumnNames dogrulama kurallari | VersaCoder.Abstractions | net48;net10.0-windows | ColumnNamesTests | planli |
| N05-042 | `ColumnNamesFactory` | class | ColumnNames uretimi ve varsayilanlari | VersaCoder.Abstractions | net48;net10.0-windows | ColumnNamesTests | planli |
| N05-043 | `ColumnNamesOptions` | record | ColumnNames yapilandirmasi | VersaCoder.Abstractions | net48;net10.0-windows | ColumnNamesTests | planli |
| N05-044 | `ColumnNamesProvider` | class | ColumnNames saglayici uygulamasi | VersaCoder.Abstractions | net48;net10.0-windows | ColumnNamesTests | planli |
| N05-045 | `ColumnNamesRegistry` | class | ColumnNames kayit/katalog yonetimi | VersaCoder.Abstractions | net48;net10.0-windows | ColumnNamesTests | planli |
| N05-046 | `ColumnNamesConverter` | class | ColumnNames bicim donusumu | VersaCoder.Abstractions | net48;net10.0-windows | ColumnNamesTests | planli |
| N05-047 | `ColumnNamesTests` | class | ColumnNames testleri (xUnit) | VersaCoder.Abstractions | net48;net10.0-windows | - | planli |
| N05-048 | `ColumnNamesBuilder` | class | ColumnNames uretici (builder) | VersaCoder.Abstractions | net48;net10.0-windows | ColumnNamesTests | planli |
| N05-049 | `IRouteNames` | interface | RouteNames sozlesmesi (port) | VersaCoder.Abstractions | net48;net10.0-windows | RouteNamesTests | planli |
| N05-050 | `RouteNames` | class | RouteNames cekirdek uygulamasi | VersaCoder.Abstractions | net48;net10.0-windows | RouteNamesTests | planli |
| N05-051 | `RouteNamesDto` | record | RouteNames veri transferi modeli | VersaCoder.Abstractions | net48;net10.0-windows | RouteNamesTests | planli |
| N05-052 | `RouteNamesMapper` | class | RouteNames <-> DTO esleme | VersaCoder.Abstractions | net48;net10.0-windows | RouteNamesTests | planli |
| N05-053 | `RouteNamesValidator` | class | RouteNames dogrulama kurallari | VersaCoder.Abstractions | net48;net10.0-windows | RouteNamesTests | planli |
| N05-054 | `RouteNamesFactory` | class | RouteNames uretimi ve varsayilanlari | VersaCoder.Abstractions | net48;net10.0-windows | RouteNamesTests | planli |
| N05-055 | `RouteNamesOptions` | record | RouteNames yapilandirmasi | VersaCoder.Abstractions | net48;net10.0-windows | RouteNamesTests | planli |
| N05-056 | `RouteNamesProvider` | class | RouteNames saglayici uygulamasi | VersaCoder.Abstractions | net48;net10.0-windows | RouteNamesTests | planli |
| N05-057 | `RouteNamesRegistry` | class | RouteNames kayit/katalog yonetimi | VersaCoder.Abstractions | net48;net10.0-windows | RouteNamesTests | planli |
| N05-058 | `RouteNamesConverter` | class | RouteNames bicim donusumu | VersaCoder.Abstractions | net48;net10.0-windows | RouteNamesTests | planli |
| N05-059 | `RouteNamesTests` | class | RouteNames testleri (xUnit) | VersaCoder.Abstractions | net48;net10.0-windows | - | planli |
| N05-060 | `RouteNamesBuilder` | class | RouteNames uretici (builder) | VersaCoder.Abstractions | net48;net10.0-windows | RouteNamesTests | planli |
| N05-061 | `ICommandNames` | interface | CommandNames sozlesmesi (port) | VersaCoder.Abstractions | net48;net10.0-windows | CommandNamesTests | planli |
| N05-062 | `CommandNames` | class | CommandNames cekirdek uygulamasi | VersaCoder.Abstractions | net48;net10.0-windows | CommandNamesTests | planli |
| N05-063 | `CommandNamesDto` | record | CommandNames veri transferi modeli | VersaCoder.Abstractions | net48;net10.0-windows | CommandNamesTests | planli |
| N05-064 | `CommandNamesMapper` | class | CommandNames <-> DTO esleme | VersaCoder.Abstractions | net48;net10.0-windows | CommandNamesTests | planli |
| N05-065 | `CommandNamesValidator` | class | CommandNames dogrulama kurallari | VersaCoder.Abstractions | net48;net10.0-windows | CommandNamesTests | planli |
| N05-066 | `CommandNamesFactory` | class | CommandNames uretimi ve varsayilanlari | VersaCoder.Abstractions | net48;net10.0-windows | CommandNamesTests | planli |
| N05-067 | `CommandNamesOptions` | record | CommandNames yapilandirmasi | VersaCoder.Abstractions | net48;net10.0-windows | CommandNamesTests | planli |
| N05-068 | `CommandNamesProvider` | class | CommandNames saglayici uygulamasi | VersaCoder.Abstractions | net48;net10.0-windows | CommandNamesTests | planli |
| N05-069 | `CommandNamesRegistry` | class | CommandNames kayit/katalog yonetimi | VersaCoder.Abstractions | net48;net10.0-windows | CommandNamesTests | planli |
| N05-070 | `CommandNamesConverter` | class | CommandNames bicim donusumu | VersaCoder.Abstractions | net48;net10.0-windows | CommandNamesTests | planli |
| N05-071 | `CommandNamesTests` | class | CommandNames testleri (xUnit) | VersaCoder.Abstractions | net48;net10.0-windows | - | planli |
| N05-072 | `CommandNamesBuilder` | class | CommandNames uretici (builder) | VersaCoder.Abstractions | net48;net10.0-windows | CommandNamesTests | planli |
| N05-073 | `IEventNames` | interface | EventNames sozlesmesi (port) | VersaCoder.Abstractions | net48;net10.0-windows | EventNamesTests | planli |
| N05-074 | `EventNames` | class | EventNames cekirdek uygulamasi | VersaCoder.Abstractions | net48;net10.0-windows | EventNamesTests | planli |
| N05-075 | `EventNamesDto` | record | EventNames veri transferi modeli | VersaCoder.Abstractions | net48;net10.0-windows | EventNamesTests | planli |
| N05-076 | `EventNamesMapper` | class | EventNames <-> DTO esleme | VersaCoder.Abstractions | net48;net10.0-windows | EventNamesTests | planli |
| N05-077 | `EventNamesValidator` | class | EventNames dogrulama kurallari | VersaCoder.Abstractions | net48;net10.0-windows | EventNamesTests | planli |
| N05-078 | `EventNamesFactory` | class | EventNames uretimi ve varsayilanlari | VersaCoder.Abstractions | net48;net10.0-windows | EventNamesTests | planli |
| N05-079 | `EventNamesOptions` | record | EventNames yapilandirmasi | VersaCoder.Abstractions | net48;net10.0-windows | EventNamesTests | planli |
| N05-080 | `EventNamesProvider` | class | EventNames saglayici uygulamasi | VersaCoder.Abstractions | net48;net10.0-windows | EventNamesTests | planli |
| N05-081 | `EventNamesRegistry` | class | EventNames kayit/katalog yonetimi | VersaCoder.Abstractions | net48;net10.0-windows | EventNamesTests | planli |
| N05-082 | `EventNamesConverter` | class | EventNames bicim donusumu | VersaCoder.Abstractions | net48;net10.0-windows | EventNamesTests | planli |
| N05-083 | `EventNamesTests` | class | EventNames testleri (xUnit) | VersaCoder.Abstractions | net48;net10.0-windows | - | planli |
| N05-084 | `EventNamesBuilder` | class | EventNames uretici (builder) | VersaCoder.Abstractions | net48;net10.0-windows | EventNamesTests | planli |
| N05-085 | `IStatusNames` | interface | StatusNames sozlesmesi (port) | VersaCoder.Abstractions | net48;net10.0-windows | StatusNamesTests | planli |
| N05-086 | `StatusNames` | class | StatusNames cekirdek uygulamasi | VersaCoder.Abstractions | net48;net10.0-windows | StatusNamesTests | planli |
| N05-087 | `StatusNamesDto` | record | StatusNames veri transferi modeli | VersaCoder.Abstractions | net48;net10.0-windows | StatusNamesTests | planli |
| N05-088 | `StatusNamesMapper` | class | StatusNames <-> DTO esleme | VersaCoder.Abstractions | net48;net10.0-windows | StatusNamesTests | planli |
| N05-089 | `StatusNamesValidator` | class | StatusNames dogrulama kurallari | VersaCoder.Abstractions | net48;net10.0-windows | StatusNamesTests | planli |
| N05-090 | `StatusNamesFactory` | class | StatusNames uretimi ve varsayilanlari | VersaCoder.Abstractions | net48;net10.0-windows | StatusNamesTests | planli |
| N05-091 | `StatusNamesOptions` | record | StatusNames yapilandirmasi | VersaCoder.Abstractions | net48;net10.0-windows | StatusNamesTests | planli |
| N05-092 | `StatusNamesProvider` | class | StatusNames saglayici uygulamasi | VersaCoder.Abstractions | net48;net10.0-windows | StatusNamesTests | planli |
| N05-093 | `StatusNamesRegistry` | class | StatusNames kayit/katalog yonetimi | VersaCoder.Abstractions | net48;net10.0-windows | StatusNamesTests | planli |
| N05-094 | `StatusNamesConverter` | class | StatusNames bicim donusumu | VersaCoder.Abstractions | net48;net10.0-windows | StatusNamesTests | planli |
| N05-095 | `StatusNamesTests` | class | StatusNames testleri (xUnit) | VersaCoder.Abstractions | net48;net10.0-windows | - | planli |
| N05-096 | `StatusNamesBuilder` | class | StatusNames uretici (builder) | VersaCoder.Abstractions | net48;net10.0-windows | StatusNamesTests | planli |
| N05-097 | `IHeaderNames` | interface | HeaderNames sozlesmesi (port) | VersaCoder.Abstractions | net48;net10.0-windows | HeaderNamesTests | planli |
| N05-098 | `HeaderNames` | class | HeaderNames cekirdek uygulamasi | VersaCoder.Abstractions | net48;net10.0-windows | HeaderNamesTests | planli |
| N05-099 | `HeaderNamesDto` | record | HeaderNames veri transferi modeli | VersaCoder.Abstractions | net48;net10.0-windows | HeaderNamesTests | planli |
| N05-100 | `HeaderNamesMapper` | class | HeaderNames <-> DTO esleme | VersaCoder.Abstractions | net48;net10.0-windows | HeaderNamesTests | planli |
| N05-101 | `HeaderNamesValidator` | class | HeaderNames dogrulama kurallari | VersaCoder.Abstractions | net48;net10.0-windows | HeaderNamesTests | planli |
| N05-102 | `HeaderNamesFactory` | class | HeaderNames uretimi ve varsayilanlari | VersaCoder.Abstractions | net48;net10.0-windows | HeaderNamesTests | planli |
| N05-103 | `HeaderNamesOptions` | record | HeaderNames yapilandirmasi | VersaCoder.Abstractions | net48;net10.0-windows | HeaderNamesTests | planli |
| N05-104 | `HeaderNamesProvider` | class | HeaderNames saglayici uygulamasi | VersaCoder.Abstractions | net48;net10.0-windows | HeaderNamesTests | planli |
| N05-105 | `HeaderNamesRegistry` | class | HeaderNames kayit/katalog yonetimi | VersaCoder.Abstractions | net48;net10.0-windows | HeaderNamesTests | planli |
| N05-106 | `HeaderNamesConverter` | class | HeaderNames bicim donusumu | VersaCoder.Abstractions | net48;net10.0-windows | HeaderNamesTests | planli |
| N05-107 | `HeaderNamesTests` | class | HeaderNames testleri (xUnit) | VersaCoder.Abstractions | net48;net10.0-windows | - | planli |
| N05-108 | `HeaderNamesBuilder` | class | HeaderNames uretici (builder) | VersaCoder.Abstractions | net48;net10.0-windows | HeaderNamesTests | planli |
| N05-109 | `IPathConstants` | interface | PathConstants sozlesmesi (port) | VersaCoder.Abstractions | net48;net10.0-windows | PathConstantsTests | planli |
| N05-110 | `PathConstants` | class | PathConstants cekirdek uygulamasi | VersaCoder.Abstractions | net48;net10.0-windows | PathConstantsTests | planli |
| N05-111 | `PathConstantsDto` | record | PathConstants veri transferi modeli | VersaCoder.Abstractions | net48;net10.0-windows | PathConstantsTests | planli |
| N05-112 | `PathConstantsMapper` | class | PathConstants <-> DTO esleme | VersaCoder.Abstractions | net48;net10.0-windows | PathConstantsTests | planli |
| N05-113 | `PathConstantsValidator` | class | PathConstants dogrulama kurallari | VersaCoder.Abstractions | net48;net10.0-windows | PathConstantsTests | planli |
| N05-114 | `PathConstantsFactory` | class | PathConstants uretimi ve varsayilanlari | VersaCoder.Abstractions | net48;net10.0-windows | PathConstantsTests | planli |
| N05-115 | `PathConstantsOptions` | record | PathConstants yapilandirmasi | VersaCoder.Abstractions | net48;net10.0-windows | PathConstantsTests | planli |
| N05-116 | `PathConstantsProvider` | class | PathConstants saglayici uygulamasi | VersaCoder.Abstractions | net48;net10.0-windows | PathConstantsTests | planli |
| N05-117 | `PathConstantsRegistry` | class | PathConstants kayit/katalog yonetimi | VersaCoder.Abstractions | net48;net10.0-windows | PathConstantsTests | planli |
| N05-118 | `PathConstantsConverter` | class | PathConstants bicim donusumu | VersaCoder.Abstractions | net48;net10.0-windows | PathConstantsTests | planli |
| N05-119 | `PathConstantsTests` | class | PathConstants testleri (xUnit) | VersaCoder.Abstractions | net48;net10.0-windows | - | planli |
| N05-120 | `PathConstantsBuilder` | class | PathConstants uretici (builder) | VersaCoder.Abstractions | net48;net10.0-windows | PathConstantsTests | planli |
