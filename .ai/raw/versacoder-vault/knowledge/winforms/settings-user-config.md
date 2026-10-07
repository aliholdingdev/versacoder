---
title: "settings-user-config - VersaCoder"
type: knowledge
category: winforms
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "Kullanici ayarlarinin saklanmasi: ISettingsStore soyutlamasi, atomik dosya yazimi, secrets disinda tutma ve net48 + net10.0-windows ayar farklari"
---

# settings-user-config — Kullanici ayarlari saklama (iki bacak)

**Zorunlu Baglantilar:** [index](../index.md) · [security-rules](../../.rules/security-rules.md) · [coding-standards](../../.rules/coding-standards.md) · [file-dialogs-io](file-dialogs-io.md) · [startup-pipeline-ui](startup-pipeline-ui.md)

> **Tek cumlelik sorumluluk:** *Kullanici ayarlari nereye, hangi bicimde ve hangi soyutlama altinda yazilir* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Ayarlari **tek arayuz `ISettingsStore`** altinda topla; uygulama kodu bu arayuzu bilir, depolama bicimi bacak degisir. Dosya konumu kullaniciya ozeldir: `%LocalAppData%\VersaCoder\settings.json` (makine degil, kullanici bazli). Yazim **atomiktir**: once gecici dosya, sonra degistirme - yarim dosya = sonraki acilista sifirlanmis ayar. **Sirlar (API anahtari, lisans, parola) buraya yazilmaz**; sirlar ortam degiskeni/kimlik dogrulayicidir (security-rules).

---

## §1 Ayar katmanlari

| Katman | Ornek | Kim yazar | Nerde durur |
|---|---|---|---|
| Uygulama ayarlari | tema, son dosya, panel boyu | kullanici | `settings.json` (per-user) |
| Makine ayarlari | instalasyon yolu, lisans yolu | kurulum | registry / installer |
| Gizli bilgi | API anahtari, parola | sistem | **bu dosyada DEGIL** |
| Donem verisi | oturum, proje | kullanici | uygulamaya ozel veri dosyasi (ADR-005) |

## §2 Arayuz ve yazim (kod)

```csharp
public interface ISettingsStore
{
    AppSettings Load();            // dosya yoksa varsayilan doner
    void Save(AppSettings value);  // atomik yazim
}

// Atomik yazim: gecici dosya -> degistir
var tmp = path + ".tmp";
File.WriteAllText(tmp, json, new UTF8Encoding(false));
if (File.Exists(path)) File.Replace(tmp, path, path + ".bak");
else File.Move(tmp, path);
```

JSON uretimi bacak degisir:

| Bicim | net48 | net10.0-windows |
|---|---|---|
| Varsayilan ( kutudan cikar ) | `DataContractJsonSerializer` (`System.Runtime.Serialization`, **paket gerekmez**) | `System.Text.Json` (**kutuda**, proje referansi yeter) |
| Paket gereken alternatif | `System.Text.Json` NuGet = **G8** (kullanici onayi) | — |
| Klasik alternatif | `Properties.Settings` + `app.config` `userSettings` bolumu | ayni bolum, tasima maliyeti yuksek |

> **KURAL:** Ayni `settings.json` dosyasi iki bacakta da ayni anahtarlarla okunur; bicim degisikligi iki bacagi **kilitler** (G6).

## §3 Iki bacak: net48 vs net10.0-windows

| Konu | net48 | net10.0-windows |
|---|---|---|
| JSON uretimi | `DataContractJsonSerializer` ya da onayli paket | `System.Text.Json` (in-box) |
| Varsayilan klasor | `Environment.GetFolderPath(SpecialFolder.LocalApplicationData)` | ayni API |
| Dosya yolu ayrimi | `\` kurali, `Path.Combine` zorunlu | ayni API |
| Kod yazimi | **C# 7.3** - `using` deyimi; `using var` yok | C# 14 |
| Config okuma | `ConfigurationManager` (App.config) | genelde dosya tabanli; `ConfigurationManager` paket ister |

`#if` yalnizca JSON uretimi satirinda (platform/API farki, G6).

## §4 Guvenlik ve dayaniklilik

| Kurul | Neden | Uygulama |
|---|---|---|
| Sifre/anahtar dosyaya girmez | dosya okunabilir = siza | ortam degiskeni / kimlik dogrulayici |
| Atomik yazim | elek kesintisi = bozuk JSON | tmp + `File.Replace` |
| UTF-8, BOM'suz | iki bacak ayni bayti okur | `new UTF8Encoding(false)` |
| Sürüm alani | ileride goc icin | `"version": 1` anahtari, okurken goz onu |
| Hata sessiz yutulmaz | kayip fark edilmez | okuma hatasi -> log + varsayilan don |

## §5 Kardes Dosyalar

| Konu | Dosya |
|---|---|
| Dosya acma / yazma guvenligi | [file-dialogs-io](file-dialogs-io.md) |
| Acilista ayar yukleme | [startup-pipeline-ui](startup-pipeline-ui.md) |
| Ayar ekraninin MVVM'si | [viewmodel-inotify](viewmodel-inotify.md) |
| Hata siniri (okuma hatasi) | [exception-boundary-ui](exception-boundary-ui.md) |
