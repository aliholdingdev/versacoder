---
title: "l6 Host — Katman Şartnamesi (VersaCoder)"
type: architecture
category: l6-host
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "l6 Host katmani: composition root, DI kayitlari, config/TFM koprusu, acilis sirasi, N38-N40 + N51, VersaCoder.Host, bagimlilik listesi ve test stratejisi"
---

# l6 Host — Katman Şartnamesi

**Zorunlu Baglantilar:** [mimari indeks](../index.md) · [katalog](../catalog/index.md) · [bagimlilik haritasi](../00-overview/dependency-map.md) · [system-overview](../00-overview/system-overview.md) · [ADR-028](../../.decisions/accepted/ADR-028-layered-baseline-9-projects.md) · [layer-boundary-check](../../checklists/layer-boundary-check.md) · [test stratejisi](../../spec/test-strategy.md)

> **Tek cumlelik sorumluluk:** *Hangi tipin kime baglandigi ve uygulamanin hangi sirada ayaga kalktiginin* tek cevabidir; baska hicbir katman `ServiceCollection`/container gormez (composition root tekeli).

## §0 Ozet (Answer First)

l6 **composition root'un tek sahibidir**: DI kaydi (N39), config/sir/TFM koprusu (N40) ve acilis boru hatti (N38) burada toplanir; l6 **l0..l5'in tamamina bakabilir**, **yalniz l7** ona bakar ve **l6 l7'ye bakamaz** (matris X); l0..l5'ten l6'ya referans **yasak** (NetArchTest = 0). `#if` yalniz N40 (`ITfmBridge`) icerisinde yasar (G9); acilis **fail-fast**: bir adim basarisizsa sonraki adim asla baslamaz, `StartupReport.FailedAt` hangi adimda duruldugunu bildirir. Klasordeki 5 dosya: bu sartname (`index.md`), N38..N40 alt dugumleri ve N51 makro dugumu.

## §1 Sorumluluk

**Tablo A — Bu katman NE YAPAR**

| # | Sorumluluk | Kanit | Cikti |
|---|---|---|---|
| H1 | DI kaydi: tek yerde `services.Add*` cagrilari | `ServiceCollectionExtensions.cs` | container |
| H2 | Config yukleme: `appsettings.json` + ortam + DPAPI sir | `ConfigurationBootstrapper` | `IBridgeOptions` |
| H3 | Acilis sirasi: config -> logger -> migration -> provider -> tools -> session -> UI | `StartupPipeline` (sirali adimlar) | `StartupReport` |
| H4 | TFM koprusu: `#if NETFRAMEWORK` / `#if NET` yalniz burada | `ITfmBridge` (N40) | platform uyumu |
| H5 | Migration tetikleme + circuit breaker / timeout ayarlari | `IMigrationRunner.Run()`, `ResilienceOptions` | guvenli varsayilanlar |

**Tablo B — Bu katman NE YAPMAZ**

| # | Yapmaz | Nerede | Neden |
|---|---|---|---|
| B1 | Is kurali / dogrulama | l1, l2 | composition != domain |
| B2 | SQL calistirma, dosya I/O | l4 | altyapi ayri katman |
| B3 | Mesaj semasi / tool tanimi | l5 | sema != baglama |
| B4 | Kontrol olusturma, Paint/rendeleme | l7 | gosterim ayri katman |
| B5 | Logger'i elle `new` etmek | l3 `ILogger` portu | bagimlilik ihlali |

## §2 GIRENLER

| # | Giren | Kaynak | Hangi kapidan gecer |
|---|---|---|---|
| 1 | `IConfiguration` (dosya + ortam) | N40 `IConfigBootstrapper.Load` | sir katmani DPAPI ile cozulur |
| 2 | DI cagrisi (`Program.Main`) | l7 Cli / Desktop | `AddVersaCoder(services, config)` |
| 3 | Butce/timeout/LLM uc noktasi ayarlari | config | `IBridgeOptions` -> l6/l4/l5 servisleri |
| 4 | `CancellationToken` (kapanis) | `IShutdownHook` | flush/dispose sirasi |

## §3 CIKANLAR

| # | Cikti | Alici | Not |
|---|---|---|---|
| 1 | Kurulu container | l6 ic akisi (N38) | her l0 portu `Probe` ile cozulur |
| 2 | `StartupReport` (adim + `FailedAt`) | l7 + log | basarisizsa UI acilmaz |
| 3 | `IBridgeOptions` | l4/l5 servisleri | sir log'a asla cikmaz |
| 4 | Kapanis temizligi (`IDisposalOrder`) | isletim sistemi | dispose sirasi zorunlu |

## §4 Yasak Yonler (ADR-028 matrisi)

| Yon hucresi | Deger | Anlam |
|---|---|---|
| `l6 -> l7` | **X** | UI'a bakis yok; UI baglayici degildir |
| `l0..l5 -> l6` | **X** | container'dan habersiz katmanlar (NetArchTest = 0) |
| `l6 -> l0..l5` | OK | tek katman ki hepsine bakabilir |
| `l7 -> l6` | OK | **tek bakan** katman |
| `l6 -> l3` | OK | `ILogger` portu uzerunden (elle `new` yok) |

```text
  l0 l1 l2 l3 l4 l5  (hepsine bakabilir)
            |
            v
   +---------------------+
   |  l6 HOST (N51)      |  composition root: tek yazim, tek sira
   |  N39 DI -> N40 cfg  |
   |       -> N38 acilis |
   +----------+----------+
              ^  yasak: l6 -> l7 (X) · l0..l5 -> l6 (X)
              |  yalniz l7 bakabilir
   +----------+----------+
   | l7 Desktop / Cli    |
   +---------------------+
```

## §5 N Kodlari

| N | Alt katman | N | Alt katman |
|---|---|---|---|
| N38 | bootstrap-pipeline | N40 | config-bridges |
| N39 | di-registration | N51 | makro l6 (5 kit) |

Seviye kurali: N kodlari klasor/namespace duzeyindedir, birer `.csproj` degildir (ADR-028 §2.1).

## §6 Projeler

| Proje | TFM | Not |
|---|---|---|
| `VersaCoder.Host` | `net48;net10.0-windows` | katmanin tek uretim projesi (9 uretimden biri) |
| `VersaCoder.Tests` | cift bacak | acilis sirasi + DI saglik testleri (`Category=Host`) |
| `VersaCoder.ArchitectureTests` | cift bacak | NetArchTest: `ServiceCollection` yalniz `VersaCoder.Host` icinde |

`net48` C# 7.3 alt kumesi, `net10.0-windows` C# 14; container `Microsoft.Extensions.DependencyInjection` (surum `Directory.Packages.props` G8 - `NEEDS_VALIDATION`).

## §7 Bagimlilik Listesi

| Kaynak | Giden | Tur |
|---|---|---|
| l6 | `VersaCoder.Abstractions` (l0) + l1, l2, l3, l4, l5 tipleri | izinli (tek katman hepsine bakar) |
| l6 | `VersaCoder.Crosscutting` (l3) `ILogger` portu | izinli; elle `new` yasak |
| l6 | `VersaCoder.Desktop` / `VersaCoder.Cli` (l7) | matris **X** -> yasak |
| l6 | `Microsoft.Extensions.DependencyInjection` (NuGet) | surum G8 (`NEEDS_VALIDATION`) |
| l6 | `System.Security.Cryptography.ProtectedData` (DPAPI) | `#if` yalniz N40'ta |

## §8 Test Stratejisi

| Test | Ne kapsar | Hedef |
|---|---|---|
| Unit (`VersaCoder.Tests`) | adim sirasi, `ORDER_CONFLICT`, `SilentFailure` | **%90** satir |
| Container saglik | her l0 portu `Probe` ile cozulur | 0 eksik port |
| Arch (NetArchTest) | `ServiceCollection` yalniz Host'ta; `l0..l5 -> l6` = 0 | **0 ihlal** |
| Gate | iki bacakta acilis testi (GATE-004 / GATE-005) | her iki TFM yesil |

Kosum: `dotnet test --filter Category=Host` · 13 soru icin [layer-boundary-check](../../checklists/layer-boundary-check.md).

## §9 Kanit

| # | Iddia | Kanit komutu | Beklenen |
|---|---|---|---|
| 1 | Dosya boyutu 2.600-8.500 bayt | `Get-Item` ile `.Length` | aralik icinde |
| 2 | Bozuk link yok | `powershell -NoProfile -ExecutionPolicy Bypass -File C:\versacoder\.ai\scripts\check-links.ps1` | `0` |
| 3 | Frontmatter 9 alan, ilk satir `---` | gozden gecirme | 9/9 |
| 4 | N38..N40 + N51 + index = 5 dosya | klasor listesi | 5 |
| 5 | Emoji / Unicode kutu / mojibake yok | `measure-vault.ps1` | `0` |

`src/` henüz diskte olmadigindan kod bazli ihlal sayilari `DOĞRULAMA GEREKLİ` kalir; ilk build sonrasi NetArchTest ciktisi bu tabloya islenir.
