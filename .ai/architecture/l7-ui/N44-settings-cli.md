---
title: "N44 Ayarlar ve CLI"
type: architecture
category: l7-ui
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: N44 - 07-settings formu ve VersaCoder.Cli: ayni ayar modeli, kodlu cikis (exit map), arguman parser (kayit tablosu DEGIL)
---

# N44 Ayarlar ve CLI

**Zorunlu Baglantilar:**
- [index.md](index.md)
- [N41-shell-ribbon.md](N41-shell-ribbon.md)
- [N52-macro-l7.md](N52-macro-l7.md)
- [../../index.md](../index.md)
- [../../catalog/index.md](../catalog/index.md)
- [../../checklists/layer-boundary-check.md](../../checklists/layer-boundary-check.md)
- [../../.decisions/adr-index.md](../../.decisions/adr-index.md)

> **Tek cumlelik sorumluluk:** N44, ayarlarin **ekranla ayni modelle** duzenlendigini ve CLI'nin **ayni l2 handler'larini** cagirip **kodlu cikis** verdigini buyur.

## §0 Ozet (Answer First)

- 07-settings formu (`LayoutControl` + `LookUpEdit` + `GridControl`) ve `VersaCoder.Cli` ayni nodadir: GUI ve CLI **ayni ayar modelini ve ayni handler'lari** paylasir; "ayni ayar iki farkli yere yazilir" imkansizdir.
- CLI cikisi ikili: insan okunur (varsayilan) ve `--json`; cikis kodlari `IExitCodeMap` ile sabittir: `0` OK, `2` bilinmeyen verb, `3` dogrulama, `64` kullanici hatasi, `70` dahili, `74` IO.
- Bilinmeyen verb `Result.Fail` ile biter; istisna **degil** sonuc (G5 kanit usulu).
- Ayar degeri dogrudan dosyaya yazilmaz; yazim servistedir (DA-01 ruhu; `.rules/data-access-rules.md`); form/CLI yalniz model baglar.
- Wireframe karsiligi: [ui-design/07-settings.md](../../ui-design/07-settings.md) - GATE-003 onayi beklemede.

## §1 Sorumluluk

| Tip | Imza (ozet) | Sorumluluk |
|---|---|---|
| `interface` | `ISettingsForm.Bind(ISettingsModel m)` | form <-> model baglama |
| `interface` | `ISettingRow.Key` / `.Validate(v)` | satir bazli dogrulama |
| `interface` | `ILookUpSource.GetOptions(key)` | model/saglayici listesi |
| `interface` | `IArgParser.Parse(string[] argv)` | arguman -> `CliRequest` |
| `interface` | `ICommandRegistry.Resolve(verb)` | fiil -> l2 handler |
| `interface` | `IConsoleRenderer.Write(result, bool json)` | cikti bicimi |
| `interface` | `IExitCodeMap.From(Result r)` | `0/2/3/64/70/74` eslemesi |

```csharp
// GUI ve CLI AYNI modeli kullanir; ayri yazim yasak (SSOT).
public interface IArgParser
{
    Result<CliRequest> Parse(string[] argv);   // bilinmeyen verb -> Result.Fail, exception YOK
}
// cikis: 0 OK -> 2 bilinmeyen verb -> 3 dogrulama -> 64 kullanici -> 70 dahili -> 74 IO
```

## §2 GIRENLER

| Girdi | Kaynak | Nasil kullanilir |
|---|---|---|
| Ayar modeli (GUI) | `ISettingsForm` veri binds | `ISettingsService` (l2) |
| `argv` (CLI) | konsol satiri | `IArgParser.Parse` |
| `verb` | argv[0] | `ICommandRegistry.Resolve` |
| Ayar kategorileri | servis cevabi | TreeList + Grid filtrasyonu |

## §3 CIKANLAR

| Cikti | Alici | Not |
|---|---|---|
| Kaydedilmis ayar | `ISettingsService` (l2) | dosya yazimi serviste |
| `--json` cikti | CI / betik | round-trip ayrilabilir |
| `exit` kodu | kabuk | `IExitCodeMap` matrisi |
| Dogrulama hatasi | satir + ayni GUI metni | iki yerde farkli mesaj yasak |

## §4 Yasak Yonler

| Yon | ADR-028 matrisi | N44 notu |
|---|---|---|
| l7 -> l1 | **X** | domain'e dogrudan referans yok |
| l7 -> l4 | **X** | dosya/HTTP yazimi yok (serviste) |
| l7 -> l2, l5, l6 | OK | handler (l2), sema (l5), DI (l6) |
| GUI -> CLI ayri model | **X** | SSOT ihlali; tek modele indirgenir |
| -> l7 | **X** | hicbir katman kabuga bakmaz |

## §5 N Kodlari

| Node | Rol | Ilişki |
|---|---|---|
| N41 | Ribbon kabuk | ayar butonu -> acilis |
| N42 | Cozum gezgini | ayar gosterimi farkli bolge |
| N43 | Editor + AI Chat | LLM ayarlari buradan |
| N44 | Ayarlar + CLI | Bu dosya - 07-settings |
| N52 | Makro l7 | `ISettingsKit` |

## §6 Projeler

| Proje | N44 katkisi |
|---|---|
| `VersaCoder.Desktop` | `SettingsDialog` form uclusu (**ui-designer** sahipligi U5) |
| `VersaCoder.Cli` | `IArgParser` + `IConsoleRenderer` + `IExitCodeMap` |
| `VersaCoder.Tests` | arguman vaka tablosu + exit kod matrisi (`Category=Cli`) |
| `VersaCoder.ArchitectureTests` | NetArchTest: `l7 -> l1/l4` = 0 |

TFM: `net48;net10.0-windows` - `Environment.ExitCode` iki bacakta ayni; konsol kodlamasi (`UTF-8`) `#if NETFRAMEWORK` icinde yalnizca; arguman kurali ve exit eslemesi ortak (G9).

## §7 Bagimlilik Listesi

| Yon | Hedef | Tur |
|---|---|---|
| l7 -> l2 | `ISettingsService` / handler | izinli |
| l7 -> l6 | DI + config kokusu | izinli |
| l7 -> l1, l4 | `VersaCoder.Domain` / `VersaCoder.Infrastructure` | **X** - yasak |
| l7 -> l0, l3, l5 | port/tip/sema | izinli |
| harici | DevExpress Universal v26.1 (NuGet) | surum G8/G3 - paket kilitli |

## §8 Test Stratejisi

| Test | Arac | Kapsam |
|---|---|---|
| 30 arguman vaka tablosu | `dotnet test --filter Category=Cli` | gecerli/gecersiz ciftleri |
| Exit kod eslemesi | `IExitCodeMap` matrisi | 0/2/3/64/70/74 |
| Esdegerlik | GUI ve CLI ayni ayari yazar -> ayni sonuc | SSOT kaniti |
| `--json` round-trip | ayrilabilirlik | cikti sozlesmesi |
| Layer siniri (13 soru) | [layer-boundary-check.md](../../checklists/layer-boundary-check.md) | N44 sinirlari |

## §9 Kanit

| Id | Kanit | Beklenen |
|---|---|---|
| P44-1 | `dotnet build VersaCoder.Cli` | 0 hata |
| P44-2 | Arguman vaka tablosu + exit matrisi | gecti - **DOĞRULAMA GEREKLİ** |
| P44-3 | GUI/CLI esdegerlik testi | ayni dosya sonucu |
| P44-4 | NetArchTest: `l7 -> l1/l4` = 0 | 0 ihlal |
| P44-5 | Dosya boyutu 2.600-8.500 bayt | aralik icinde |

`src/` henüz diskte olmadigindan kod bazli kanitlar `DOĞRULAMA GEREKLİ` kalir. Katalog kaydi (`catalog/N44-settings-cli.md`) henuz yazilmadi - [catalog/index.md](../catalog/index.md) uzerinden izlenir.
