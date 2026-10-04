---
title: "N16 - Validators (Alan Bazli Dogrulama, Fields Bicimi)"
type: architecture
category: l2-application
status: active
version: 1.0
updated: 2026-10-04
authority: SSOT
scope: VersaCoder
---

# N16 - Validators (Alan Bazli Dogrulama, Fields Bicimi)

**Zorunlu Baglantilar:** [index.md](index.md) | [N12-commands.md](N12-commands.md) | [N14-handlers.md](N14-handlers.md) | [N15-dto-mappers.md](N15-dto-mappers.md) | [N17-use-cases.md](N17-use-cases.md) | [N47-macro-l2.md](N47-macro-l2.md) | [ADR-028](../../.decisions/accepted/ADR-028-layered-baseline-9-projects.md) | [ADR-013](../../.decisions/accepted/ADR-013-layered-architecture-52-nodes.md) | [layer-boundary-check](../../checklists/layer-boundary-check.md) | [test-strategy](../../spec/test-strategy.md)

> **Tek cumlelik sorumluluk:** Hangi alanin hangi kurala gore dogrulanacagini, hatanin `Fields` icinde nasil donecegini ve nerede invariant'a birakilacagini belirler.

## §0 Ozet (Answer First)

Dogrulama **alan bazlidir** ve **handler'in ilk adimidir**: hatalar `Error.Invalid(fields)` ile `Fields` sozlugunde (`"Name" -> "Proje adi bos olamaz."`) doner; ekran her hatayi kendi alanina yazabilir ve hepsi **tek seferde** gosterilir. **Is kurali (invariant) burada tekrarlanmaz** — o l1'dedir; l2 yalniz girdi bicimi ve on kosullarina bakar. FluentValidation **paket olarak alinmaz** (G8): saf C# kosullari + `StringComparer.Ordinal` sozlugu yeterlidir. Mesajlar **TR tek dil**.

**Kunye:**

| Ozellik | Deger |
|---|---|
| Katman | l2-application |
| Proje | VersaCoder.Application |
| N kodu | N16, [ADR-013](../../.decisions/accepted/ADR-013-layered-architecture-52-nodes.md) |
| Envanter | 5 alan kurali (asagi) |
| Varsayilan yon | l1 (invariant'a birakir) |

## §1 Sorumluluk

Tek sorumluluk: **girdi bicimi + toplu hata donusu**.

| # | Alan | Kural | ErrorCode | Mesaj (TR) |
|---|---|---|---|---|
| 1 | `Name` | 1..100 karakter, bos olamaz | `Invalid` | "Proje adi bos olamaz." |
| 2 | `RootPath` | normalize, `..` yok, kok olamaz | `Invalid` | "Gecerli bir klasor yolu secin." |
| 3 | `Key` (setting) | tekil (Scope icinde) | `Conflict` | "Bu ayar zaten var." |
| 4 | `Take` | 1..100 | `Invalid` | "Sayfa boyutu 1 ile 100 arasinda olmali." |
| 5 | Dosya yolu | kok icinde (`PathPolicy`, l1) | `Invalid` | "Bu dizin disinda dosya islemi yapilamaz." |

```csharp
private static IReadOnlyDictionary<string, string> Validate(CreateProjectCommand cmd)
{
    var errors = new Dictionary<string, string>(StringComparer.Ordinal);
    if (string.IsNullOrWhiteSpace(cmd.Name) || cmd.Name.Trim().Length > 100)
        errors["Name"] = "Proje adi bos olamaz.";
    return errors;
}
```

## §2 Girenler / Ciktilar

| Yon | Ne | Kaynak |
|---|---|---|
| GIRENLER | Komut alanlari | [N12](N12-commands.md) |
| GIRENLER | Kural (1..100, tekil, yol) | uzman kurali |
| GIRENLER | Tekil kontrol on kosulu | port sorgusu (l0) |
| CIKANLAR | `Fields` sozlugu (bos ise gec) | handler devam eder |
| CIKANLAR | `Invalid` tek donus | l7 alan hatasi gosterir |
| CIKANLAR | TR mesaj | l7 |

## §3 Yasak Yonler

| Yon | Deger | Neden |
|---|---|---|
| Validator -> l4-infrastructure | X | tekrar sorgu SQL degil |
| Validator -> l5/l6/l7 | X | ekran/mevcudiyet gorulmez |
| Validator -> l3-crosscutting | X (matris) | **DOGRULAMA GEREKLİ** |
| Invariant tekrari | X | iki gercek kaynak (N10) |
| Ingilizce mesaj | X | TR tek dil |

Izinli: validator -> l1 (`PathPolicy` benzeri kural okumasi), validator -> l0 (`Result`/`Error`).

## §4 N Kodlari ve Proje

| N | Ad | Iliski | Proje |
|---|---|---|---|
| N16 | Validators | 5 alan + toplu donus | VersaCoder.Application |
| N14 | Handlers | 1. adim cagrisi | VersaCoder.Application |
| N10 | Invariants | Is kurali buraya **birakilir** | VersaCoder.Domain |
| N15 | DTO/Mappers | Hata DTO'ya gecmez | VersaCoder.Application |
| N44 | settings-cli | `Key` tekiligi | VersaCoder.Cli + Desktop |

## §5 Bagimlilik Listesi

| Bagimlilik | Yon | Not |
|---|---|---|
| l0: `Result`, `Error` | asagi | hata tipi |
| l1: `PathPolicy` benzeri kural | asagi | is kurali degil |
| l4/l5/l6/l7 | YOK | sifir |
| FluentValidation paketi | YOK | G8 |

## §6 Test Stratejisi

| Katman | Ne test edilir | Hedef |
|---|---|---|
| Unit | 5/5 alan: gecersiz -> hata, gecerli -> gec | once RED |
| Unit | Sinir degerleri (0/1/100/101, bos/bosluk/101 krk) | sinir +/- 1 |
| Unit | Ayni anda 2 hatali alan -> ikisi `Fields`te | toplu donus |
| Arch | Invariant tekrari yok (kod arama) | tek kaynak |
| Arch | Mesaj dili TR (Ingilizce kelime denetimi) | arama testi |

## §7 Hata Durumlari

| Durum | ErrorCode | Sonuc |
|---|---|---|
| Birden fazla alan hatali | `Invalid` (hepsi `Fields`te) | tek seferde hepsi gosterilir |
| Boyle kayit zaten var | `Conflict` | dogrulayicida **degil**, l1/port'ta |
| Mesaj bos/null | `Invalid` | "bos olamaz" mesaji |
| Kutuphane mesaji (Ingilizce) | - | **yasak** |
| Girdi `null` (tipin kendisi) | istisna | programlama hatasi (guard) |

## §8 Kurallar (5 madde)

- Dogrulama handler'in ilk adimi; port cagrisindan **once** bitir.
- Hatalari tek donuste topla (`Fields`); tek tek istek yapma.
- Is kuralini burada **yazma**; invariant l1'de (N10) kalsin.
- Mesajlari TR yaz; kutuphane mesaji gosterme.
- Paket alma (FluentValidation); saf kosul + sozluk yeterli (G8).

## §9 Kanit

| Kanit | Deger |
|---|---|
| Dosya sayisi (l2-application) | 9 (index + N12..N18 + N47) |
| Boyut araligi | 2600-8500 bayt |
| Baglanti kirigi | 0 |
| Format | ASCII tablo, emoji yok |
| Kaynak | Bilesim: hedef vault SSOT + kaynak vault detayi |
