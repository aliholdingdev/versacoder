---
title: "N22 - Crypto / Security (IStringProtector, AES-256-GCM)"
type: architecture
category: l3-crosscutting
status: active
version: 1.0
updated: 2026-10-04
authority: SSOT
scope: VersaCoder
---

# N22 - Crypto / Security (IStringProtector, AES-256-GCM)

**Zorunlu Baglantilar:** [index.md](index.md) | [N19-logging.md](N19-logging.md) | [N23-config-opts.md](N23-config-opts.md) | [keys.md](../../keys.md) | [ADR-028](../../.decisions/accepted/ADR-028-layered-baseline-9-projects.md) | [ADR-013](../../.decisions/accepted/ADR-013-layered-architecture-52-nodes.md) | [layer-boundary-check](../../checklists/layer-boundary-check.md) | [test-strategy](../../spec/test-strategy.md)

> **Tek cumlelik sorumluluk:** Duz metnin neden diskte durmadigini, sifrelemenin nasil cagrilacagini ve anahtarin kimde oldugunu belirler.

## §0 Ozet (Answer First)

`IStringProtector`, baglanti dizesi gibi **duyarliligi olan metni** korur/cozer: algoritma **AES-256-GCM** (BCL `System.Security.Cryptography`, **paket yok**), sonuc **Base64(paket)**. **Anahtar bu katmanda barindirilmaz** — nereden gelecegi ayri bir ADR kararidir (`NEEDS_VALIDATION`). Diskte **duz metin sifre/connection string yazmak yasaktir**. Cozme basarisizsa hata `Result.Fail(Invalid)` biciminde duser, exception ust katmana sizmaz.

**Kunye:**

| Ozellik | Deger |
|---|---|
| Katman | l3-crosscutting |
| Proje | VersaCoder.Crosscutting |
| N kodu | N22, [ADR-013](../../.decisions/accepted/ADR-013-layered-architecture-52-nodes.md) |
| Arayuz | `IStringProtector` |
| Varsayilan yon | yalniz l0 (`Result` hata dili) |

## §1 Sorumluluk

Tek sorumluluk: **metni koru/coz sozlesmesi** (anahtar yonetimi disarida).

| Imza | Davranis | Not |
|---|---|---|
| `string Protect(string plainText)` | AES-256-GCM -> Base64 | `null` guard |
| `string Unprotect(string cipherText)` | Base64 -> coz, dogrula | MAC hataliysa reddet |

| Karar | Secilen | Reddedilen | Gerekce |
|---|---|---|---|
| Algoritma | **AES-256-GCM** (BCL) | duz metin / kendi sifresi | standart + butunluk (MAC) |
| Paket | **0** (BCL) | NuGet kripto kutuphanesi | G8 |
| Anahtar kaynagi | ADR ile **ayri karar** | koddaki sabit string | CRITICAL |
| Girdi bicimi | Base64 paket | hex/ikili dosya | tek metin yuzeyi |

```csharp
public interface IStringProtector
{
    string Protect(string plainText);       // sonuc: Base64(paket)
    string Unprotect(string cipherText);    // hata: Result.Fail(Invalid)
}
```

## §2 Girenler / Ciktilar

| Yon | Ne | Kaynak |
|---|---|---|
| GIRENLER | Duz metin (baglanti dizesi) | l4, l6 |
| GIRENLER | Anahtar (dis kaynak, ADR) | harici kasa kaydi |
| GIRENLER | Sifreli metin (config) | [N23](N23-config-opts.md) |
| CIKANLAR | Base64(paket) | config dosyasi |
| CIKANLAR | Cozulmus metin | tuketici |
| CIKANLAR | `Result.Fail(Invalid)` (MAC/base64 hatasi) | ust katmana exception **sizmaz** |

## §3 Yasak Yonler

| Yon | Deger | Neden |
|---|---|---|
| l3 -> l1/l2/l4..l7 | X | nötr kalmak (matris `l3 yalniz l0`) |
| Anahtari kodda/barindirmak | X | **CRITICAL**, ADR gerektirir |
| Duz metin diskte | X | **CRITICAL**, tarama testi |
| NuGet kripto paketi | X | G8 |
| Boylece cagri disinda exception | X | S5 kurali |

Izinli: l3 -> l0 (`Result`), l3 -> [keys.md](../../keys.md) (esleme referansi).

## §4 N Kodlari ve Proje

| N | Ad | Iliski | Proje |
|---|---|---|---|
| N22 | Crypto-Sec | `IStringProtector` | VersaCoder.Crosscutting |
| N23 | Config-Opts | Cozme sirasinda cagrir | VersaCoder.Crosscutting |
| N39 | DI registration | Singleton kaydi | VersaCoder.Host |
| N40 | config-bridges | Baglanti dizesi okuma | VersaCoder.Host |
| N30 | ado-dapper | Baglanti dizesi tuketir | VersaCoder.Infrastructure |

## §5 Bagimlilik Listesi

| Bagimlilik | Yon | Not |
|---|---|---|
| l0: `Result`, `Error` | asagi | tek izinli |
| l1..l7 | YOK | nötr |
| BCL `System.Security.Cryptography` | ic | AES-256-GCM |
| NuGet kripto | YOK | G8 |
| Anahtar kaynagi | ADR | `NEEDS_VALIDATION` |

## §6 Test Stratejisi

| Katman | Ne test edilir | Hedef |
|---|---|---|
| Unit | Koru -> coz turu orijinali verir | once RED |
| Unit | Bozuk paket -> `Invalid`, exception **yok** | tek byte degistir |
| Unit | Ayni girdi 2. kez -> **farkli** cikti (GCM nonce) | tekrar-saldiri |
| Arch | Kodda/config'de duz metin `Password`/`ConnectionString` yok | arama testi |
| Arch | NuGet kripto paketi yok | `.csproj` denetimi |

## §7 Hata Durumlari

| Durum | ErrorCode | Sonuc |
|---|---|---|
| Paket bozulmus / MAC hatali | `Invalid` | cozme basarisiz, ustte hata |
| Base64 gecersiz | `Invalid` | cozme reddedilir |
| `null`/bos girdi | `Invalid` / guard | reddet |
| Anahtar kayboldu | `Invalid` | sifreli metin okunamaz |
| Anahtar koddaki sabit | **CRITICAL** | sifreleme bosuna |
| Duz metin diskte bulundu | **CRITICAL** | sızıntı |

## §8 Kurallar (5 madde)

- Duyarliligi metni disari `Protect`/`Unprotect` ile gec; ham yazma.
- Anahtari bu katmanda **barindirma**; kaynagi ADR ile belirle.
- Hata donusunu `Result.Fail(Invalid)` ile ver; exception yukari **sizmaz**.
- `AesGcm` yapilandirma farkini (net48/net10) tek zarf sinifinda tut — `NEEDS_VALIDATION`.
- Duz metin taramasini testte arama testi ile denetle.

## §9 Kanit

| Kanit | Deger |
|---|---|
| Dosya sayisi (l3-crosscutting) | 6 (index + N19..N23) |
| Boyut araligi | 2600-8500 bayt |
| Baglanti kirigi | 0 |
| Format | ASCII tablo, emoji yok |
| Kaynak | Bilesim: hedef vault SSOT + kaynak vault detayi |
