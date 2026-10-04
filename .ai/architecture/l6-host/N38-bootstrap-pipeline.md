---
title: "N38 Acilis Boru Hatti"
type: architecture
category: l6-host
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: N38 - sirali ve fail-fast acilis, StartupReport, preflight, kapanis kanclari (kayit tablosu DEGIL)
---

# N38 Acilis Boru Hatti

**Zorunlu Baglantilar:**
- [index.md](index.md)
- [N39-di-registration.md](N39-di-registration.md)
- [N51-macro-l6.md](N51-macro-l6.md)
- [../../index.md](../../index.md)
- [../../checklists/layer-boundary-check.md](../../checklists/layer-boundary-check.md)
- [../../spec/test-strategy.md](../../spec/test-strategy.md)

> **Tek cumlelik sorumluluk:** N38, uygulamanin hangi adimla, hangi sirada ve ilk hatada nasil durarak ayaga kalktigini (fail-fast boru hatti) buyur.

## §0 Ozet (Answer First)

- Acilis **sirali ve fail-fast** boru hattidir: `config -> logger -> migration -> provider -> tools -> session -> UI`; bir adim basarisizsa sonraki adim **asla baslamaz**.
- `StartupReport.FailedAt` hangi adimda duruldugunu adıyla bildirir; **sessiz basari** (hic calismadan `OK` donen adim) da hata sayilir (`SilentFailure`), gate kapali kalir.
- Sira **N38'a aittir**: `IStartupStep.RunAsync` icinde adim kendi sirasini **degistiremez**; ayni `Order` iki adimda ise acilistan once `ORDER_CONFLICT`.
- Ilerleme kosulu: 3. adim basarisiz -> 4-7 hic calismaz (§9 P38-2).

## §1 Sorumluluk

| Tip | Imza (ozet) | Sorumluluk |
|---|---|---|
| `interface` | `IStartupStep.Name` / `.Order` / `.RunAsync(ct)` | tek adim sozlesmesi |
| `interface` | `IStartupPipeline.RunAsync(CancellationToken ct)` | sirayi yurutme |
| `interface` | `IOrderValidator.Validate(steps)` | bosluk / cakisma reddi |
| `interface` | `IStartupReport.Steps` / `.FailedAt` / `.SilentFailure` | ne yapildi, nerede duruldu |
| `interface` | `IPreflight.Check()` + `IShutdownHook.OnStopAsync(ct)` | disk/izin yoklamasi + kapanis |

```csharp
public interface IStartupStep
{
    string Name { get; }
    int    Order { get; }
    Task<Result> RunAsync(CancellationToken ct);
}
// kural: RunAsync ICINDE asla adim sira DEGISTIRMEZ; sira N38'a aittir.
// hata -> Result.Fail -> boru hatti DURUR; kalan adimlar "atlandi" olarak raporlanir.
```

## §2 GIRENLER

| Girdi | Kaynak | Nasil kullanilir |
|---|---|---|
| DI kayitlari (N39) | l7 Cli / Desktop `Program.Main` | `IStartupPipeline.RunAsync` |
| Config + sir | N40 `IConfigBootstrapper` | 1. adim |
| Migration (N28/l4) | 3. adim | hatasi yukari tasinir, uygulama baslamaz |
| `CancellationToken` | kapanis / kullanici | `IShutdownHook` sirasi |

## §3 CIKANLAR

| Cikti | Alici | Not |
|---|---|---|
| `StartupReport` (adimlar + `FailedAt`) | l7 + log | basarisizsa UI acilmaz |
| `ProtocolViolation(ORDER_CONFLICT)` | acilis oncesi dogrulama | ayni `Order` iki adimda |
| `Result.Fail(IO_ERROR)` (kapanis) | log | yine de kapanir |
| IHealthGate acildi | l7 | UI ancak kapidan sonra gosterilir |

## §4 Yasak Yonler

| Yon | ADR-028 matrisi | N38 notu |
|---|---|---|
| l6 -> l7 | **X** | UI'i kendisi acmaz; gate acilinca l7 gosterir |
| l0..l5 -> l6 | **X** | container'dan habersiz katmanlar |
| l6 -> l0..l5 | OK | adimlarin tamami bu katmandan baglanir |
| l6 -> l3 | OK | logger yalniz `ILogger` portu uzerunden |
| l6 -> l4 | OK | migration **tetiklenir**, SQL burada yazilmaz |

## §5 N Kodlari

| Node | Rol | Ilişki |
|---|---|---|
| N38 | Acilis boru hatti | Bu dosya |
| N39 | DI kayitlari | Pipeline oncesi container |
| N40 | Config/TFM koprusu | 1. adimin girdisi |
| N28 | Migration (l4) | 3. adim uyducusu |
| N51 | Makro l6 | `IBootstrapKit` |

## §6 Projeler

| Proje | N38 katkisi |
|---|---|
| `VersaCoder.Host` | StartupPipeline, StartupReport, preflight, shutdown hook |
| `VersaCoder.Tests` | Sira + fail-fast + sessiz basari testleri (`Category=Host`) |
| `VersaCoder.ArchitectureTests` | NetArchTest: `ServiceCollection` yalniz Host'ta |

TFM: `net48;net10.0-windows` - giris `Main` senkron + `GetAwaiter().GetResult()` (`#if NETFRAMEWORK`) karsiligi `async Task Main` (`#if NET`); adim **sirasi** iki bacakta ortak ve test edilir (G9).

## §7 Bagimlilik Listesi

| Yon | Hedef | Tur |
|---|---|---|
| l6 -> l0 | `Result` / port arayuzleri | izinli |
| l6 -> l1..l5 | tipler + N28 migration uyducusu | izinli (tek katman hepsine bakar) |
| l6 -> l7 | `VersaCoder.Desktop` / `VersaCoder.Cli` | matris **X** - yasak |
| l0..l5 -> l6 | `VersaCoder.Host` | **X** - NetArchTest = 0 |
| harici | `Microsoft.Extensions.*` (NuGet) | surum G8 (`NEEDS_VALIDATION`) |

## §8 Test Stratejisi

| Test | Arac | Kapsam |
|---|---|---|
| Adim sirasi | `dotnet test --filter Category=Host` | tanimli sira + kayit |
| Fail-fast | birim test | 3. adim basarisiz -> 4-7 hic calismaz |
| Sessiz basari | birim test | hic calismayan adim SilentFailure |
| ORDER_CONFLICT | birim test | ayni `Order` acilistan once reddedilir |
| Layer siniri (13 soru) | [layer-boundary-check.md](../../checklists/layer-boundary-check.md) | N38 sinirlari |

## §9 Kanit

| Id | Kanit | Beklenen |
|---|---|---|
| P38-1 | `dotnet build VersaCoder.Host` | 0 hata |
| P38-2 | Fail-fast testi (4-7 adim calismaz) | gecti - **DOĞRULAMA GEREKLİ** |
| P38-3 | Sessiz basari testi (SilentFailure) | gecti - **DOĞRULAMA GEREKLİ** |
| P38-4 | iki bacak acilis testi (GATE-004/005) | her iki TFM yesil |
| P38-5 | Dosya boyutu 2.600-8.500 bayt | aralik icinde |

`src/` henüz diskte olmadigindan kod bazli kanitlar `DOĞRULAMA GEREKLİ` kalir.
