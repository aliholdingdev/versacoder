---
title: "Dugum Detayı N45 — MAKRO l0 Abstractions"
type: architecture
category: architecture
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "l0 katmaninin makro sorumlulugu, bagimlilik yonu, disariya acilan imzalari ve N01-N05 alt dugumlerinin birlesik ozeti"
---

# Dugum Detayı N45 — MAKRO l0 Abstractions

**Zorunlu Baglantilar:** [Mimari index](../index.md) · [katman spec](index.md) · [dependency-map](../00-overview/dependency-map.md) · [ADR-028](../../.decisions/accepted/ADR-028-layered-baseline-9-projects.md) · [layer-boundary-check](../../checklists/layer-boundary-check.md) · [test-strategy](../../spec/test-strategy.md)

> **Tek cumlelik sorumluluk:** *l0 katmani olarak neye yarar, nereden nereye bakabilir ve disariya hangi imzalari acar* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

l0 **hicbir sey calistirmaz, yalniz imza yayimlar**: `Result`/`Error` hata dili, port sozlesmeleri, olay ABI'si, guard/extension ve sabit adlar. Bagimlilik yonu **tek yonludur `l1..l7 -> l0`**; l0'in `VersaCoder.*` ve NuGet referansi **0** olmalidir. Bu makro N45'tir ve altindaki bes alt dugumun (N01..N05) sorumluluklarini birlestirir; bilesen kayitlari [katalog](../catalog/index.md)'dadir.

| Alan | Deger |
|---|---|
| Makro kod | N45 · l0 Abstractions (inşa sırasi 1) |
| Proje / TFM | `VersaCoder.Abstractions` · `net48;net10.0-windows` |
| Alt dugumler | N01, N02, N03, N04, N05 |
| Buna bakabilen | l1, l2, l3, l4, l5, l6, l7 — tumu |
| Buna bakmasi yasak | l0'in kendisi hicbir `VersaCoder.*` tipine bakamaz |

---

## §1 Sorumluluk (NE YAPAR)

| # | Alt dugum | Disariya acilan yuzey | Tuketen katman |
|---|---|---|---|
| 1 | [N01-ports-core](N01-ports-core.md) | `IRepository<,>`, `IUnitOfWork`, `IDbConnectionFactory`, `ISqlDialect`, `IFileSystem`, `IProcessRunner`, `ILlmTransport`, `IDispatcher` | l2, l4, l5, l6, l7 |
| 2 | [N02-result-error](N02-result-error.md) | `Result`, `Result<T>`, `Error`, `ErrorCode` | **tumu** |
| 3 | [N03-events-abi](N03-events-abi.md) | `IDomainEvent`, `IAgentEvent` | l1 (uretim), l2, l5, l7 |
| 4 | [N04-extensions](N04-extensions.md) | `Guard.*`, `StringExtensions`, `CollectionExtensions` | l1..l7 |
| 5 | [N05-constants](N05-constants.md) | `LogEvents`, `ConfigKeys`, `CacheKeys`, `EventTypes` | l1, l2, l3, l5 |

---

## §2 Bu Katmana GIRENLER / CIKANLAR

| Yon | Kim / ne | Tur | Kosul |
|---|---|---|---|
| GIREN | l1, l2, l3, l4, l5, l6, l7 -> l0 | izinli ok | tumu bu katmana bakabilir |
| GIREN | test projesi -> l0 (unit + arch) | izinli ok | `VersaCoder.Tests` |
| CIKAN | tek hata dili (`Result<T>`) | cikti | tum katmanlar ayni sozlesmeyi okur |
| CIKAN | port imzalari (govde yok) | cikti | uygulayici l4 / l5 |
| YASAK | l0 -> `VersaCoder.*` / NuGet / `DateTime.Now` | yasak | Arch testi 0/0/0 |

---

## §3 Yasak Yonler (ADR-028 matrisi)

| # | Yasak yon | Kaynak | Anlami |
|---|---|---|---|
| 1 | `l0 -> l1..l7` | ADR-028 §2.1 | en alt sabit; tek istisna yok |
| 2 | `l2 -> l4` | ADR-028 §2.1 | repository portu l0'da, uygulamasi l4'te |
| 3 | `l4 -> l2` | ADR-028 §2.3/2 | sira degil kenar; port disi tip paylasimi yasak |
| 4 | `l5 -> l4` | ADR-028 §2.3/2 | protokol katmani altyapıya bakmaz |
| 5 | `l3 -> l1, l2, l4..l7` | ADR-028 §2.1 | l3 yalniz l0'a bakar |

---

## §4 N Kodlari ve Projeler

| N | Alt katman | Makro | Proje |
|---|---|---|---|
| N01 | ports-core | l0 | `VersaCoder.Abstractions` |
| N02 | result-error | l0 | `VersaCoder.Abstractions` |
| N03 | events-abi | l0 | `VersaCoder.Abstractions` |
| N04 | extensions | l0 | `VersaCoder.Abstractions` |
| N05 | constants | l0 | `VersaCoder.Abstractions` |
| N45 | makro ozet | l0 | proje degil, katman ozeti |

---

## §5 Bagimlilik Listesi

| # | Bagimlilik | Yon | Tur | Not |
|---|---|---|---|---|
| 1 | BCL | l0 -> System | **izinli** | iki bacakta ayni |
| 2 | NuGet / `Directory.Packages.props` | l0 -> paket | **yasak** | `.csproj` referansi 0 |
| 3 | `VersaCoder.*` | l0 -> diger katmanlar | **yasak** | hizli duzeltme yok |
| 4 | l1..l7 -> l0 | ust -> l0 | **izinli** | herkes okuyabilir |
| 5 | test projesi -> l0 | test -> l0 | **kosullu** | urun `.csproj`'ina girmez |

---

## §6 Test Stratejisi

| Test turu | Ne dogrular | Hedef |
|---|---|---|
| Arch (makro kabul kriteri) | `VersaCoder.Abstractions -> VersaCoder.*` = **0** | 0 |
| Arch (makro kabul kriteri) | NuGet referansi = **0** | 0 |
| Unit | `Result` + guard + sabit adlari (N02/N04/N05) | **%100** satir |
| Sozlesme | her `public interface` XML yorumu | 0 eksik |
| Dokuman-kod | imza listesi [N01-ports-core](N01-ports-core.md) §1 ile ayni | 0 fark |

---

## §7 Katman Geneli Riskler

| # | Risk | Sonuc | Koruma |
|---|---|---|---|
| 1 | l0 dosyasinda `using VersaCoder.*` | **CRITICAL** (dongu) | Arch testi: referans 0 |
| 2 | l0'da NuGet paketi | **CRITICAL** (G8) | Arch testi: `.csproj` 0 |
| 3 | l0'da `DateTime.Now` / `File.*` / `Console.*` | gizli bagimlilik | Arch testi: arama 0 |
| 4 | Ikinci sonuc tipi (`MyResult`) | iki hata dili | Arch testi: tek `Result<T>` |
| 5 | Port govdesine is mantigi yazmak | l0 artik l1'dir | kod incelemesi (reviewer) |

---

## §8 Kurallar

- Yeni port = [N01-ports-core](N01-ports-core.md) tablosuna ayni gun satir; tablosuz port mimari olarak yoktur.
- `Result<T>` bu katman disinda yeniden tanimlanamaz (tek hata dili).
- `#if` hedefi bu katmanda **0**'dir; iki bacak ayni imzayi kullanir.
- C# 7.3 disi ozellik (`record`, `init`) ortak kaynakta kullanilmaz — tek bacak derlenir.
- Port/guard/sabit ekleyen is, [layer-boundary-check](../../checklists/layer-boundary-check.md) 13 sorusundan 1, 2, 9'u tekrar isaretler.

---

## §9 Kanit

| # | Iddia | Kanit | Durum |
|---|---|---|---|
| 1 | 6 baglantinin hedefi diskte var | `check-links.ps1` -> bozuk link **0** | olcum |
| 2 | Dosya 3-8 KB | `Get-Item N45-macro-l0.md).Length` | olcum |
| 3 | 5 alt dugum baglantisi dogru | kardes dosyalar bu klasorde | olcum |
| 4 | Yon kurali kaynagi | [ADR-028](../../.decisions/accepted/ADR-028-layered-baseline-9-projects.md) §2.1 | dogrulandi |
| 5 | Madde listesi <= 5 | §8 = 5 madde | sayim |
