---
title: "Dugum Detayı N05 — Sabitler ve Sabit Adlar (l0)"
type: architecture
category: architecture
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "Sabit adlar (log/anahtar/cache/olay/bicim) ve sabit degerlerin tek kaynagi - siir/metin saklanmaz, literal kullanim yasaktir"
---

# Dugum Detayı N05 — Sabitler ve Sabit Adlar (l0)

**Zorunlu Baglantilar:** [Mimari index](../index.md) · [katman spec](index.md) · [keys](../../keys.md) · [ADR-028](../../.decisions/accepted/ADR-028-layered-baseline-9-projects.md) · [test-strategy](../../spec/test-strategy.md) · [N04-extensions](N04-extensions.md)

> **Tek cumlelik sorumluluk:** *Bir anahtar/olay/kategori adi nerede tanimlidir ve yanlis yazilirsa hangi test patlar* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Serbest metin adlari (log olayi `project.created`, config anahtari `log.level`, cache anahtari `l2:list:projects`) **l0'da tek sinifta** tanimlanir; dogrudan string literal kullanimi **yanlis yazilmis anahtar** demektir ve derleme zamaninda gorunmez. Bu dugum **sir/anahtar/metin icermez** (G2/G8): yalniz ad ve bicim sabitlenir; gercek deger (API anahtari, connection string) buraya **yazilmaz**.

| Alan | Deger |
|---|---|
| N kodu / alt katman | N05 · constants |
| Makro katman / proje | l0 Abstractions · `VersaCoder.Abstractions` |
| Giren | ad tanimi talebi (l1, l2, l3, l5, l6) |
| Cikti | `const string` sabitleri + bicim kaliplari |
| Yasak | connection string, API anahtari, sifre bu siniflarda duramaz |

---

## §1 Sorumluluk (NE YAPAR)

| # | Sabit grubu | Ornek ad | Kullanan katman | Neden sabit |
|---|---|---|---|---|
| 1 | Log olay adlari | `LogEvents.ProjectCreated = "project.created"` | l2, l3 | arama/analiz tutarliligi |
| 2 | Config anahtarlari | `ConfigKeys.LogLevel = "log.level"` | l3, l6 | yazim hatasi -> sessiz varsayilan |
| 3 | Cache anahtar kaliibi | `CacheKeys.ProjectList = "l2:list:projects"` | l2, l3 | TTL patlamasi onlenir |
| 4 | Olay tipi adlari | `EventTypes.TaskStarted = "task.started"` | l1, l5 | ABI uyumu (bkz [N03-events-abi](N03-events-abi.md)) |
| 5 | Bicim + limit sabitleri | `UtcFormat`, `MaxPageSize = 100` | l3, l5, l1, l2 | tek sinir kaynagi |

---

## §2 Bu Katmana GIRENLER / CIKANLAR

| Yon | Kim / ne | Tur | Kosul |
|---|---|---|---|
| GIREN | yeni anahtar/olay adi talebi | girdi | sabit sinifina eklenir |
| CIKAN | `const string` referansi | cikti | derleme zamanina gecer |
| CIKAN | ad degisikligi -> derleme hatasi | cikti | kullanim yerleri korunur |
| YASAK | literal kullanim (`"l2:list:..."`) | yasak | Arch tarama testi |
| YASAK | sifre/anahtar degeri sabite yazmak | yasak | guvenlik bulgusu (CRITICAL) |

---

## §3 Yasak Yonler (ADR-028 matrisi)

| # | Yasak yon | Kaynak | Bu dugumdeki karsiligi |
|---|---|---|---|
| 1 | `l0 -> l1..l7` | ADR-028 §2.1 | sabit sinifi katman adi tasmaz |
| 2 | `l2 -> l4` | ADR-028 §2.1 | cache anahtari veritabani bilmez |
| 3 | `l3 -> l1, l2, l4..l7` | ADR-028 §2.1 | l3 yalniz sabit adini okur |
| 4 | `l6 -> l7` | ADR-028 §2.1 | config kokusu UI'dan okunmaz |
| 5 | `l7 -> l1..l5` | ADR-028 §2.1 | ekran sabitleri l6 uzerinden alir |

---

## §4 N Kodu ve Proje

| Alan | Deger |
|---|---|
| Katalog kaydi | `../catalog/index.md` altinda `N05` satiri |
| Kod yolu | `src/VersaCoder.Abstractions/Constants/` (`LogEvents.cs`, `ConfigKeys.cs`, `CacheKeys.cs`, `EventTypes.cs`) |
| Proje | `VersaCoder.Abstractions` (tek .csproj, cift TFM) |

---

## §5 Bagimlilik Listesi

| # | Bagimlilik | Yon | Tur | Not |
|---|---|---|---|---|
| 1 | BCL (`const`, `static readonly`) | l0 -> System | **izinli** | derleme zamanina gecer |
| 2 | NuGet paketi | l0 -> paket | **yasak** | `.csproj` referansi 0 |
| 3 | l3 `IAppConfig` ile anahtar esitligi | l0 <-> l3 | **kosullu** | kopma testi tests/l3 |
| 4 | l1, l2, l5 -> sabit adlari | ust -> l0 | **izinli** | derleme zamaninda dogrulama |
| 5 | sifre/anahtar kasasi | - | **yasak** | `[keys](../../keys.md)` tek kaynak |

---

## §6 Test Stratejisi

| Test turu | Ne dogrular | Hedef |
|---|---|---|
| Unit | her sabit adi benzersizdir | 0 tekrar |
| Unit | log olay adlari `kategori.olay` biciminde | %100 |
| Unit | cache anahtari kalibi `katman:islem:id` (tests/l0 + tests/l3) | %100 |
| Arch | siser/metin sabiti kodda gecmiyor | 0 bulgu |
| Arch | config anahtarlari `IAppConfig` ile ayni | 0 kopuk |

---

## §7 Hata Durumlari

| Durum | Sonuc | `ErrorCode` | Koruma |
|---|---|---|---|
| Anahtar literal yazildi | sessizce calisir, deger dustu | - | literal yasak + tarama |
| Sabit adi degisti, kullanim unutuldu | derleme hatasi | - | derleyici zaten korur |
| Cache anahtari kirpildi/kesisti | yanlis veri donme riski | - | fabrika metodu + test |
| Olay tipi adi uyumsuz | olay atlanir | `Protocol` | N03 ile ortak test |
| Sifre/anahtar sabite yazildi | **CRITICAL** (G2/guvenlik) | - | kod incelemesi + tarama |

---

## §8 Kurallar

- Dogrudan string literal ile anahtar/olay adi kullanilmaz; sabit sinifi tek kaynaktir.
- Connection string, API anahtari, sifre bu siniflarda **duramaz** — gercek deger [keys](../../keys.md)'te yasar.
- `const` tercih edilir; yalnizca bicim birlestirmesinde `string.Concat` kullanilir (sonuc iki bacakta ayni).
- Yeni sabit = ayni gun kullanim yerinin guncellenmesi; ad degisikligi derleme hatasi uretir, sessiz kalmaz.
- Cache anahtari kaliibi `katman:islem:id` seklindedir; N20 (caching) sozlesmesiyle birlilikte test edilir.

---

## §9 Kanit

| # | Iddia | Kanit | Durum |
|---|---|---|---|
| 1 | 6 baglantinin hedefi diskte var | `check-links.ps1` -> bozuk link **0** | olcum |
| 2 | Dosya 3-8 KB | `Get-Item N05-constants.md).Length` | olcum |
| 3 | Gizli deger yok | Arch taramasi: sifre/anahtar deseni 0 | test |
| 4 | Yon kurali kaynagi | [ADR-028](../../.decisions/accepted/ADR-028-layered-baseline-9-projects.md) §2.1 | dogrulandi |
| 5 | Madde listesi <= 5 | §8 = 5 madde | sayim |
