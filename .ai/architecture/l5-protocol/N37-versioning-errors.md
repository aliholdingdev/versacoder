---
title: "N37 Surum ve Hata"
type: architecture
category: l5-protocol
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: N37 - protocolVersion major.minor kurali, uyumluluk matrisi, tek hata katalogu IErrorCatalog (kayit tablosu DEGIL)
---

# N37 Surum ve Hata

**Zorunlu Baglantilar:**
- [index.md](index.md)
- [N33-tool-schema.md](N33-tool-schema.md)
- [N34-messages.md](N34-messages.md)
- [N50-macro-l5.md](N50-macro-l5.md)
- [../../index.md](../../index.md)
- [../../checklists/layer-boundary-check.md](../../checklists/layer-boundary-check.md)

> **Tek cumlelik sorumluluk:** N37, protokol surumunun ne zaman kirildigini ve hata kodlarinin tek katalogdan nasil dagitildigini (kod = enum, mesaj = tek yer) buyur.

## §0 Ozet (Answer First)

- `protocolVersion` `major.minor` olarak tutulur: **breaking change = major artisi** ve eski major ile **iletisim kurulamaz** (`PROTOCOL_VERSION_UNSUPPORTED`); minor kucuk => geriye donuk uyumlu.
- Hata kodlari tek `IErrorCatalog` icinde yasar: her kodun **bir sahibi ve bir mesaji** vardir; kod `string` olarak **yazilmaz**, `ErrorCode` enum'undan okunur - iki bacakta da ayni string.
- Sahip oldugu nuans: alan silindi ama minor artti => **RED** (review); `IBreakingChange` kaydi zorunludur - kayit yoksa kapi kapanir.
- Ilerleme kosulu: surum matrisi `(1.0 <-> 1.3 ok, 1.x <-> 2.0 yok)` + katalog SSOT testi (§9 P37-1, P37-3).

## §1 Sorumluluk

| Tip | Imza (ozet) | Sorumluluk |
|---|---|---|
| `interface` | `IProtocolVersion.Major` / `.Minor` / `.Parse(string)` | surum sabiti + ayristirma |
| `interface` | `IVersionNegotiation.Compatible(local, remote)` | uyumluluk karari |
| `interface` | `IVersionGuard.Envelope(MessageEnvelope m)` | gelen karede surum denetimi |
| `interface` | `IErrorCatalog.Get(ErrorCode c)` | kod -> sahip + mesaj |
| `interface` | `IBreakingChange` | hangi alan kirildi, hangi major gerekli |

```csharp
public interface IVersionNegotiation
{
    Result Compatible(VersionSpec local, VersionSpec remote);
}
// kural: major FARKLI -> uyumsuz (RED). minor kucuk -> geriye donuk uyumlu.
// hata kodu string olarak YAZILMAZ; ErrorCode enum'u + IErrorCatalog uzerinden okunur.
```

## §2 GIRENLER

| Girdi | Kaynak | Nasil kullanilir |
|---|---|---|
| Yerel surum (protokol) | Bu katman (N37 sabiti) | `Compatible` sol bacak |
| Uzak surum bilgisi | l4/l7 tarafindan gelen kare | `IVersionGuard.Envelope` |
| `MessageEnvelope` | N34 | Zorunlu `protocolVersion` alanindan ayristirma |
| Yeni hata kodu talebi | herhangi bir dugum | Once N37'ye eklenir, sonra kullanilir |

## §3 CIKANLAR

| Cikti | Alici | Not |
|---|---|---|
| `Result` (uyumlu) | N34 -> N33/N35 akisi | mesaj islenmeye devam eder |
| `Result.Fail(PROTOCOL_VERSION_UNSUPPORTED / VERSION_MISSING / UNKNOWN_ERROR_CODE)` | cagiran + N34 | fail-closed |
| Hata mesaji (katalog) | l7 gosterim / l4 log | sahip + mesaj tek yerden |
| `IMigrationHint` | l6 upgrade akisi | major atlamada ne yapilacagi |

## §4 Yasak Yonler

| Yon | ADR-028 matrisi | N37 notu |
|---|---|---|
| l5 -> l4 | **?** | DOĞRULAMA GEREKLİ - surum bilgisi port uzerinden gelir |
| l5 -> l6, l5 -> l7 | X | upgrade/gosterim paketi okunmaz |
| l5 -> l3 | X | Crosscutting tasinamaz |
| l5 -> l0, l1, l2 | . | gecisli; `Result`/`ErrorCode` abi l0'da |
| l4 -> l5, l7 -> l5 | YOK / OK | katalog **okunabilir**; katalog geri bakmaz |

## §5 N Kodlari

| Node | Rol | Ilişki |
|---|---|---|
| N37 | Surum ve hata | Bu dosya - tek katalog |
| N33 | Arac semasi | `ProtocolViolation` kodlari |
| N34 | Mesaj tipleri | `protocolVersion` zorunlu alan |
| N35/N36 | Oturum / dongu | `SESSION_*` / `LOOP_*` kodlari buradan |
| N50 | Makro l5 | `IProtocolCore` |

## §6 Projeler

| Proje | N37 katkisi |
|---|---|
| `VersaCoder.Protocol` | VersionSpec, negotiator, hata katalogu (tek uretim projesi) |
| `VersaCoder.Tests` | Surum matrisi + katalog SSOT testleri (`Category=Protocol`) |
| `VersaCoder.ArchitectureTests` | NetArchTest: katalog disinda hata string'i = 0 |

TFM: `net48;net10.0-windows` - `struct VersionSpec` (manuel `Equals`); `record` yalniz `#if NET`. Katalog mesaji iki bacakta **ayni string** (metin karsilastirma testi).

## §7 Bagimlilik Listesi

| Yon | Hedef | Tur |
|---|---|---|
| l5 -> l0 | `Result` / `Error` / `ErrorCode` abi | izinli |
| l5 -> l1, l2 | tipler (matriste `.`) | gecisli |
| l5 -> l3 | `VersaCoder.Crosscutting` | matris **X** - yasak |
| l5 -> l4 | `VersaCoder.Infrastructure` | **?** - DOĞRULAMA GEREKLİ |
| harici | `System.Text.Json` (NuGet) | surum sabit (G8) |

## §8 Test Stratejisi

| Test | Arac | Kapsam |
|---|---|---|
| Uyumluluk matrisi | `dotnet test --filter Category=Protocol` | `(1.0<->1.3 ok, 1.x<->2.0 yok)` |
| Zorunlu surum alani | Theory | protocolVersion sil -> reddet |
| Katalog SSOT | birim test | her ErrorCode icin tek mesaj |
| Kapsam disi kod | derleme (enum) | katalogda olmayan kod uretilemez |
| Layer siniri (13 soru) | [layer-boundary-check.md](../../checklists/layer-boundary-check.md) | N37 sinirlari |

## §9 Kanit

| Id | Kanit | Beklenen |
|---|---|---|
| P37-1 | Surum matrisi testi | 1<->1 gecti, 1<->2 reddedildi - **DOĞRULAMA GEREKLİ** |
| P37-2 | `dotnet build VersaCoder.Protocol` | 0 hata |
| P37-3 | Katalogda her ErrorCode icin tek mesaj (sayim) | 0 cift metin |
| P37-4 | iki bacak katalog metni karsilastirma testi | 0 fark |
| P37-5 | Dosya boyutu 2.600-8.500 bayt | aralik icinde |

`src/` henüz diskte olmadigindan kod bazli kanitlar `DOĞRULAMA GEREKLİ` kalir.
