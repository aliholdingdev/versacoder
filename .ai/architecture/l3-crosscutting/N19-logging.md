---
title: "N19 - Logging (ILogSink, Sink Cesitleri, Yapilandirilmis Kayit)"
type: architecture
category: l3-crosscutting
status: active
version: 1.0
updated: 2026-10-04
authority: SSOT
scope: VersaCoder
---

# N19 - Logging (ILogSink, Sink Cesitleri, Yapilandirilmis Kayit)

**Zorunlu Baglantilar:** [index.md](index.md) | [N20-caching.md](N20-caching.md) | [N21-clock-time.md](N21-clock-time.md) | [N23-config-opts.md](N23-config-opts.md) | [keys.md](../../keys.md) | [ADR-028](../../.decisions/accepted/ADR-028-layered-baseline-9-projects.md) | [ADR-013](../../.decisions/accepted/ADR-013-layered-architecture-52-nodes.md) | [layer-boundary-check](../../checklists/layer-boundary-check.md) | [test-strategy](../../spec/test-strategy.md)

> **Tek cumlelik sorumluluk:** Bir olay logunun hangi bicimde, nereye yazildigini ve log yazmanın asla patlamadigini belirler.

## §0 Ozet (Answer First)

Log **yapilandirilmistir**: `Message` + `Fields` sozlugu + `Utc` (IClock) + opsiyonel `Exception`; serbest metin birlestirme **yoktur**, olay adi `kategori.olay` bicimindedir (`project.created`). Yazici `ILogSink`'tir ve **`Write` asla exception firlatmaz** — log hatasi is bitirmez. Uretimde `CompositeLogSink` (File + Debug), testte `NullLogSink`. Log'a **sifre/connection string/API anahtari yazilmaz**.

**Kunye:**

| Ozellik | Deger |
|---|---|
| Katman | l3-crosscutting |
| Proje | VersaCoder.Crosscutting |
| N kodu | N19, [ADR-013](../../.decisions/accepted/ADR-013-layered-architecture-52-nodes.md) |
| Arayuz | `ILogSink.Write(LogEntry)` |
| Varsayilan yon | yalniz l0 (`Result` hata dili) |

## §1 Sorumluluk

Tek sorumluluk: **tek yazi sozlesmesi + patlamama garantisi**.

| Tip | Imza / alan | Sorumluluk |
|---|---|---|
| `LogLevel` | `Trace, Debug, Info, Warn, Error` | seviye (5 kademe) |
| `LogEntry` | `Level`, `Message`, `Fields`, `Utc`, `Exception` | tek log kaydi |
| `ILogSink` | `void Write(LogEntry)` | tek yazi sozlesmesi; **firlatmaz** |
| `FileLogSink` | gunluk dosyasi, satir bazli atomik | uretim |
| `DebugLogSink` | `Debug` ciktisi | gelistirme |
| `CompositeLogSink` | 2/3 sink'e dagitir | masaustu varsayilani |
| `NullLogSink` | hicbir yere yazmaz | test |

```csharp
public interface ILogSink
{
    void Write(LogEntry entry);   // asla exception firlatmaz; hata sessizce atlanir
}
```

## §2 Girenler / Ciktilar

| Yon | Ne | Kaynak |
|---|---|---|
| GIRENLER | `LogEntry` (seviye, olay, alanlar) | l1, l2, l4, l5, l6, l7 |
| GIRENLER | `Utc` zaman damgasi | [N21](N21-clock-time.md) `IClock` |
| GIRENLER | Seviye filtresi | [N23](N23-config-opts.md) config |
| CIKANLAR | Gunluk dosyasi / Debug ciktisi | uretim |
| CIKANLAR | Hiclik (test) | `NullLogSink` |
| CIKANLAR | Yutulmus sink hatasi | ust katmana **yayilmaz** |

## §3 Yasak Yonler

| Yon | Deger | Neden |
|---|---|---|
| l3 -> l1/l2/l4..l7 | X | nötr kalmak (matris `l3 yalniz l0`) |
| Log icinde loglama | X | sonsuz dongu (B5) |
| Loga sifre/anahtar | X | sızinti (CRITICAL) |
| `DateTime.Now` ile `Utc` | X | iki bacak farkli zaman; `IClock` zorunlu |
| NuGet log framework'u | X | G8 |

Izinli: l3 -> l0 (`Result` hata dili, [index](../index.md)).

## §4 N Kodlari ve Proje

| N | Ad | Iliski | Proje |
|---|---|---|---|
| N19 | Logging | Sink ailesi | VersaCoder.Crosscutting |
| N21 | Clock-Time | `Utc` uretimi | VersaCoder.Crosscutting |
| N23 | Config-Opts | Seviye filtresi | VersaCoder.Crosscutting |
| N05 | Constants | `LogEvents` sabitleri (l0) | VersaCoder.Abstractions |
| N39 | DI registration | Sink kaydi | VersaCoder.Host |

## §5 Bagimlilik Listesi

| Bagimlilik | Yon | Not |
|---|---|---|
| l0: `Result`, `LogEvents` | asagi | tek izinli |
| l1..l7 | YOK | nötr |
| NuGet (Serilog/NLog) | YOK | kendi ince arayuzu (G8) |
| BCL (`System.Diagnostics`, `System.IO`) | ic | yeterli |

## §6 Test Stratejisi

| Katman | Ne test edilir | Hedef |
|---|---|---|
| Unit | `NullLogSink`/`FileLogSink` exception **firlatmaz** | once RED |
| Unit | `CompositeLogSink` iki sink'e de yazar | sayac |
| Unit | Olay adi `kategori.olay` biciminde | regex |
| Unit | Seviye filtresi (Info alti dosyaya gitmez) | sinir |
| Arch | Log metninde `Password`/`ApiKey` yok | arama testi |

## §7 Hata Durumlari

| Durum | Sonuc | Koruma |
|---|---|---|
| `Write` icinde istisna | **yutulur**, is devam eder | sink ici `try/catch` |
| Dosya yazma izni yok | log kaybi, uygulama calisir | yedek `DebugLogSink` |
| Log icinde loglama | sonsuz dongu | **yasak** (B5) |
| `Fields` icinde sirlanan deger | sızıntı | kod incelemesi + arama testi |
| Olay adi literal yazildi | arama yapilamaz | `LogEvents` sabitleri (l0) |

## §8 Kurallar (5 madde)

- Kaydi yapilandir: `Message` + `Fields` + `kategori.olay`; serbest metin yazma.
- `Write` icinde exception **yutulur**; is akisina yayma.
- `Utc`'yi `IClock`'tan al; `DateTime.Now` ile `Utc` kullanma.
- Sifre/anahtar/connection string loga **yazma**.
- Testte `NullLogSink`; uretimde `CompositeLogSink` (yasam suresi kaydi: [index.md](index.md) §1).

## §9 Kanit

| Kanit | Deger |
|---|---|
| Dosya sayisi (l3-crosscutting) | 6 (index + N19..N23) |
| Boyut araligi | 2600-8500 bayt |
| Baglanti kirigi | 0 |
| Format | ASCII tablo, emoji yok |
| Kaynak | Bilesim: hedef vault SSOT + kaynak vault detayi |
