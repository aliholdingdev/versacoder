---
title: "N40 Config ve TFM Koprusu"
type: architecture
category: l6-host
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: N40 - appsettings + ortam + DPAPI sir katmanlari, IBridgeOptions, #if'in tek sahibi ITfmBridge (kayit tablosu DEGIL)
---

# N40 Config ve TFM Koprusu

**Zorunlu Baglantilar:**
- [index.md](index.md)
- [N39-di-registration.md](N39-di-registration.md)
- [N51-macro-l6.md](N51-macro-l6.md)
- [../../index.md](../../index.md)
- [../../checklists/layer-boundary-check.md](../../checklists/layer-boundary-check.md)
- [../../keys.md](../../keys.md)

> **Tek cumlelik sorumluluk:** N40, yapilandirmanin dosya + ortam + sifreli sir katmanlarinin ve `#if` farklarinin **tek merkezden** cozuldugunu buyur.

## §0 Ozet (Answer First)

- Oncelik sirasi tek akista: `appsettings.json` -> ortam degiskeni (`VERSACODER_*`, **ustun gelir**) -> DPAPI ile cozulmus sir; sonuc `IBridgeOptions` olur.
- `ISecretBridge` **duz metni asla dondurmez** (basarisiz cozum -> `SECRET_UNPROTECT`, fail-fast, `null` degil); cozulmus sir log'a, hata mesajina, `Result` icerigine **sizamaz**.
- `ITfmBridge` `#if NETFRAMEWORK` / `#if NET` farklarinin **tek sahibidir**; baska dosyada `#if` yazmak G9 ihlalidir (review RED).
- Ilerleme kosulu: ortam degiskeni dosyayi ezer + `#if` taramasi (§9 P40-1, P40-5).

## §1 Sorumluluk

| Tip | Imza (ozet) | Sorumluluk |
|---|---|---|
| `interface` | `IEnvBridge.Get(key)` / `.OrDefault(key, v)` | ortam degiskeni katmani |
| `interface` | `ISecretBridge.Unprotect(blob)` / `.Protect(plain)` | DPAPI sir koprusu |
| `interface` | `ITfmBridge.IsNetFramework` / `.Apply<T>(net48Val, netVal)` | platform secimi tek yerde |
| `interface` | `IPathBridge.AppData` / `.ConfigFile` | yol cozumu, klasor garantisi |
| `interface` | `IConfigBootstrapper.Load()` -> `IBridgeOptions` | dosya + ortam birlesimi |

```csharp
public interface ISecretBridge
{
    string Unprotect(string blob);   // DPAPI CurrentUser; basarisiz -> hata, null DEGIL
    string Protect(string plain);
}
// kural: COZULMUS sir yalniz kod icinde; log/yayin/Result mesaji disinda ASLA cikmaz.
// ITfmBridge disindaki dosyada #if yazmak G9 ihlalidir (review RED).
```

## §2 GIRENLER

| Girdi | Kaynak | Nasil kullanilir |
|---|---|---|
| `appsettings.json` | disk (repo/yerel) | temel katman |
| `VERSACODER_*` ortam degiskeni | ortam | dosyayi ezer (onecelik) |
| Sifreli blob | DPAPI (`CurrentUser`) | `Unprotect` - makine/kullaniciya bagli |
| Ayar anahtarlari | N39 kayitlari | `IBridgeOptions` -> timeout/havuz/LLM |

## §3 CIKANLAR

| Cikti | Alici | Not |
|---|---|---|
| `IBridgeOptions` | N39 DI -> N38 acilis | sir icermez, deger icerir |
| `Result.Fail(CONFIG_MISSING / CONFIG_INVALID / SECRET_UNPROTECT)` | acilis raporu | hangi anahtar + satir no |
| Platform secimi (`Apply`) | N38 giris/kapanis | yalniz tek noktada |
| `PATH` cozumu (`IPathBridge`) | l4/l7 | klasor garantisi |

## §4 Yasak Yonler

| Yon | ADR-028 matrisi | N40 notu |
|---|---|---|
| l6 -> l7 | **X** | UI yol/ayar okumaz, yalnizca baglanir |
| l0..l5 -> l6 | **X** | config disinda container gorunmez |
| l6 -> l0..l5 | OK | tek katman hepsine bakar |
| l6 -> l4 | OK | sir dosyasi/DPAPI uygulamasi l4'te; **tetikleme** burada |
| l6 -> l3 | OK | logger portu; sir asla loga gitmez |

## §5 N Kodlari

| Node | Rol | Ilişki |
|---|---|---|
| N40 | Config/TFM koprusu | Bu dosya - `#if` tek sahibi |
| N39 | DI kayitlari | `IBridgeOptions` tuketicisi |
| N38 | Acilis boru hatti | 1. adim girdisi |
| N51 | Makro l6 | `IConfigKit` |
| N26/N27 | l4 config/sifre | blob uretimi (ilk kurulum, yerel) |

## §6 Projeler

| Proje | N40 katkisi |
|---|---|
| `VersaCoder.Host` | ConfigBootstrapper, Env/Secret/Tfm/Path kopru siniflari |
| `VersaCoder.Tests` | Oncelik + sizma + JSON hata testleri (`Category=Host`) |
| `VersaCoder.ArchitectureTests` | NetArchTest: `#if` yalniz N40'ta (kod taramasi) |

TFM: `net48;net10.0-windows` - DPAPI `ProtectedData` net48'de yerlesik, net10'da NuGet ailesi (G8 - `NEEDS_VALIDATION`); baglam her iki bacakta `DataProtectionScope.CurrentUser`.

## §7 Bagimlilik Listesi

| Yon | Hedef | Tur |
|---|---|---|
| l6 -> l0..l5 | konfigurasyon tuketimi | izinli |
| l6 -> l7 | `VersaCoder.Desktop` / `VersaCoder.Cli` | matris **X** - yasak |
| l0..l5 -> l6 | `VersaCoder.Host` | **X** - NetArchTest = 0 |
| harici | `System.Security.Cryptography.ProtectedData` | net48 yerlesik / net10 NuGet |
| harici | `System.Text.Json` (appsettings okuma) | surum G8 |

## §8 Test Stratejisi

| Test | Arac | Kapsam |
|---|---|---|
| Oncelik sirasi | `dotnet test --filter Category=Host` | ortam degiskeni dosyayi ezer |
| Sizma testi (negatif) | birim test | sir log/hata/Result'a sizmaz |
| Bozuk JSON | birim test | `CONFIG_INVALID` + satir no |
| Yanlis DPAPI baglami | birim test | `SECRET_UNPROTECT`, uygulama durur |
| Layer siniri (13 soru) | [layer-boundary-check.md](../../checklists/layer-boundary-check.md) | N40 sinirlari |

## §9 Kanit

| Id | Kanit | Beklenen |
|---|---|---|
| P40-1 | Oncelik testi (env > dosya) | gecti - **DOĞRULAMA GEREKLİ** |
| P40-2 | `dotnet build VersaCoder.Host` | 0 hata |
| P40-3 | Sizma taramasi: cozulmus sir log ornegi yok | 0 eslesme |
| P40-4 | `#if` taramasi: yalniz N40 (`ITfmBridge`) | disarida 0 `#if` |
| P40-5 | Dosya boyutu 2.600-8.500 bayt | aralik icinde |

`src/` henüz diskte olmadigindan kod bazli kanitlar `DOĞRULAMA GEREKLİ` kalir.
