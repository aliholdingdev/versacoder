---
title: "N23 - Config / Options (Katmanli Okuma, Fallback, Sirlama)"
type: architecture
category: l3-crosscutting
status: active
version: 1.0
updated: 2026-10-04
authority: SSOT
scope: VersaCoder
---

# N23 - Config / Options (Katmanli Okuma, Fallback, Sirlama)

**Zorunlu Baglantilar:** [index.md](index.md) | [N19-logging.md](N19-logging.md) | [N21-clock-time.md](N21-clock-time.md) | [N22-crypto-sec.md](N22-crypto-sec.md) | [dependency-map](../00-overview/dependency-map.md) | [ADR-028](../../.decisions/accepted/ADR-028-layered-baseline-9-projects.md) | [ADR-013](../../.decisions/accepted/ADR-013-layered-architecture-52-nodes.md) | [layer-boundary-check](../../checklists/layer-boundary-check.md) | [test-strategy](../../spec/test-strategy.md)

> **Tek cumlelik sorumluluk:** Bir ayarin nereden okundugunu, hangi sira ile ezildigini ve tipi bozuksa ne oldugunu belirler.

## §0 Ozet (Answer First)

`IAppConfig` **tek okuma yuzeyidir** ve **katmanli** calisir: `varsayilan (kod)` -> `kullanici dosyasi (roaming)` -> `ortam degiskeni (PROCESS)`; **son okuyan kazanir**. Boylece CLI ve masaustu ayni kodu paylasir, makineye gore deger degisir. Tip donusumu bozuksa **fallback doner** (exception yok); sirlanan degerler (baglanti dizesi) okuma sirasinda `IStringProtector` ile cozulur ([N22](N22-crypto-sec.md)).

**Kunye:**

| Ozellik | Deger |
|---|---|
| Katman | l3-crosscutting |
| Proje | VersaCoder.Crosscutting |
| N kodu | N23, [ADR-013](../../.decisions/accepted/ADR-013-layered-architecture-52-nodes.md) |
| Arayuz | `IAppConfig` (singleton) |
| Varsayilan yon | yalniz l0 |

## §1 Sorumluluk

Tek sorumluluk: **tek okuma yuzeyi + katman onceligi**.

| Imza | Donus | Not |
|---|---|---|
| `string GetString(string key, string fallback)` | deger ya da fallback | fallback **zorunlu** |
| `int GetInt(string key, int fallback)` | sayi ya da fallback | parse hatasi -> fallback |
| `bool GetBool(string key, bool fallback)` | mantiksal | `true/false` disi -> fallback |
| `IReadOnlyDictionary<string,string> Section(string prefix)` | alt kume | `log.` gibi onek |

| # | Katman | Kaynak | Ornek |
|---|---|---|---|
| 1 | varsayilan | kod icinde | `log.level = Info` |
| 2 | kullanici dosyasi | roaming/uygulama klasoru | `config.json` (BCL okuyucu) |
| 3 | ortam degiskeni | `Environment.GetEnvironmentVariable` | `VERSACODER_LOG_LEVEL` |

**YASAK:** Sifreli deger duz metin dosyada duramaz; okuma sirasinda `Unprotect` gecilir. Anahtar metni kodda/dosyada **bulunmaz**.

## §2 Girenler / Ciktilar

| Yon | Ne | Kaynak |
|---|---|---|
| GIRENLER | Anahtar + fallback | tuketici (l6, l2, l3) |
| GIRENLER | Varsayilan (kod) | bu proje |
| GIRENLER | Kullanici dosyasi / ortam degiskeni | makine |
| CIKANLAR | Deger ya da fallback | tuketici |
| CIKANLAR | Cozulmus sirlı deger | [N22](N22-crypto-sec.md) |
| CIKANLAR | Log kaybi (dosya okunamadi) | fallback + log |

## §3 Yasak Yonler

| Yon | Deger | Neden |
|---|---|---|
| l3 -> l1/l2/l4..l7 | X | nötr kalmak (matris `l3 yalniz l0`) |
| Dosyada duz metin sifre | X | **CRITICAL**, tarama testi |
| Dosya hatasinda istisna | X | ust katman patlamaz |
| `SetEnvironmentVariable(..., User)` | X | kayit/izin sorunu; yalniz PROCESS |
| NuGet config paketi | X | G8 |

Izinli: l3 -> l0 (`Result`), l3 -> [N22](N22-crypto-sec.md) (ayni katman ici cozme).

## §4 N Kodlari ve Proje

| N | Ad | Iliski | Proje |
|---|---|---|---|
| N23 | Config-Opts | `LayeredConfig` | VersaCoder.Crosscutting |
| N22 | Crypto-Sec | Sirlı deger cozme | VersaCoder.Crosscutting |
| N19 | Logging | `log.level` okur | VersaCoder.Crosscutting |
| N40 | config-bridges | l6 uzerinden baglanti | VersaCoder.Host |
| N39 | DI registration | Singleton kaydi | VersaCoder.Host |

## §5 Bagimlilik Listesi

| Bagimlilik | Yon | Not |
|---|---|---|
| l0: `Result` | asagi | tek izinli |
| l1..l7 | YOK | nötr |
| BCL (`Environment`, JSON okuyucu) | ic | yeterli |
| NuGet config | YOK | G8 |

## §6 Test Stratejisi

| Katman | Ne test edilir | Hedef |
|---|---|---|
| Unit | Katman onceligi: ortam > dosya > varsayilan | 3/3 |
| Unit | Dosya yoksa varsayilan | dayaniklilik |
| Unit | Bozuk sayi -> fallback (`"abc"`) | tip donusumu |
| Unit | Bozuk dosya -> fallback + log | kirik dosya |
| Arch | `Environment.*` yalniz `LayeredConfig` icinde | kod arama |

## §7 Hata Durumlari

| Durum | Sonuc | Koruma |
|---|---|---|
| Anahtar hic tanimli degil | **fallback** (exception yok) | zorunlu fallback |
| Sayi metni bozuk (`"abc"`) | fallback | `TryParse` |
| Dosya okunamiyor (izin/bozuk) | ust katmanlar **patlamaz**; varsayilan | `try/catch` + log |
| Dosyada yinelenen anahtar | **son deger** okunur | acik kural |
| Sirlı metin anahtarsiz cozulemiyor | fallback / `Invalid` | N22 + ADR |

## §8 Kurallar (5 madde)

- Her okumada **fallback ver**; istisna firlatma.
- Sirlı degeri okuma sirasinda `Unprotect` ile coz; duz metin okuma.
- Ezilme sirasini bozma: varsayilan -> dosya -> ortam (son okuyan kazanir).
- `Environment.*` yalniz `LayeredConfig` icinde; Arch testi ile denetle.
- Ortam degiskenini yalniz `PROCESS` kapsaminda kullan (`User` yok).

## §9 Kanit

| Kanit | Deger |
|---|---|
| Dosya sayisi (l3-crosscutting) | 6 (index + N19..N23) |
| Boyut araligi | 2600-8500 bayt |
| Baglanti kirigi | 0 |
| Format | ASCII tablo, emoji yok |
| Kaynak | Bilesim: hedef vault SSOT + kaynak vault detayi |
