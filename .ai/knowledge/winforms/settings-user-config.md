---
title: "settings-user-config - VersaCoder"
type: knowledge
category: winforms
status: active
version: 1.0.0
updated: 2026-09-29
authority: primary
scope: "Kullanici ayarlarinin saklanmasi: ISettingsStore soyutlamasi, atomik dosya yazimi, sirlari disarda tutma ve net48 + net10.0-windows ayar farklari"
tags: [knowledge]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# settings-user-config - Kullanici ayarlari saklama (iki bacak)

**Zorunlu Baglantilar:** [[knowledge/index|vault indeks]] · [[.rules/security-rules|security-rules]] · [[.rules/configuration-rules|configuration-rules]] · [[knowledge/winforms/logging-integration|logging-integration]] · [[knowledge/winforms/composition-root-host-startup|composition-root-host-startup]]
**Küme + Hub:** [[knowledge/winforms/index|winforms index]] · [[index|index]] · [[claude|CLAUDE]] · [[definitions/agents|AGENTS]] · [[.decisions/brain|brain]] · [[definitions/glossary|glossary]] · [[governance/keys|keys]]
**Kardeşler (winforms · 3/32):** [[knowledge/winforms/accessibility-tab-and-mnemonics|accessibility-tab-and-mnemonics]] · [[knowledge/winforms/anchor-dock-layout-rules|anchor-dock-layout-rules]] · [[knowledge/winforms/async-await-ui-thread|async-await-ui-thread]] · [[knowledge/winforms/async-loading-patterns|async-loading-patterns]] · [[knowledge/winforms/async-ui-thread-marshalling|async-ui-thread-marshalling]] · [[knowledge/winforms/async-void-event-handlers|async-void-event-handlers]] · [[knowledge/winforms/backgroundworker-vs-task|backgroundworker-vs-task]] · [[knowledge/winforms/cancellation-in-ui|cancellation-in-ui]] · [[knowledge/winforms/cancellation-token-in-ui|cancellation-token-in-ui]] · [[knowledge/winforms/control-disposal-and-memory|control-disposal-and-memory]] · [[knowledge/winforms/controls-lifecycle-events|controls-lifecycle-events]] · [[knowledge/winforms/data-binding-basics|data-binding-basics]] · [[knowledge/winforms/designer-cs-anatomy|designer-cs-anatomy]] · [[knowledge/winforms/designer-cs-trio-rule|designer-cs-trio-rule]] · [[knowledge/winforms/designer-editing-rules|designer-editing-rules]] · [[knowledge/winforms/designer-hand-edit-policy|designer-hand-edit-policy]] · [[knowledge/winforms/di-container-lifetimes|di-container-lifetimes]] · [[knowledge/winforms/docking-panels-patterns|docking-panels-patterns]] · [[knowledge/winforms/errorprovider-validation-flow|errorprovider-validation-flow]] · [[knowledge/winforms/exception-boundary-ui|exception-boundary-ui]] · [[knowledge/winforms/form-close-and-dispose|form-close-and-dispose]] · [[knowledge/winforms/form-close-cancel-pattern|form-close-cancel-pattern]] · [[knowledge/winforms/form-constructor-vs-load|form-constructor-vs-load]] · [[knowledge/winforms/form-load-and-shown-events|form-load-and-shown-events]] · [[knowledge/winforms/layout-table-panel|layout-table-panel]] · [[knowledge/winforms/partial-class-and-file-naming|partial-class-and-file-naming]] · [[knowledge/winforms/splitcontainer-and-sizing|splitcontainer-and-sizing]] · [[knowledge/winforms/validation-form-level|validation-form-level]] · [[knowledge/winforms/winforms-rendering-performance|winforms-rendering-performance]]

> **Tek cumlelik sorumluluk:** *Kullanici ayarlari nereye, hangi bicimde ve hangi soyutlama altinda yazilir* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Ayarlari **tek arayuz `ISettingsStore`** altinda topla; uygulama kodu yalniz bu arayuzu bilir, depolama bicimi bacak degisir. Dosya konumu kullaniciya ozeldir: `%LocalAppData%\VersaCoder\settings.json` - makine degil, kullanici bazli. Yazim **atomiktir**: once gecici dosya, sonra degistirme; yarim kalan dosya sonraki acilista sifirlanmis ayar demektir. **Sirlar (API anahtari, lisans, parola) buraya yazilmaz**; sirlar ortam degiskeni ya da kimlik dogrulayicidir ([[.rules/security-rules|security-rules]]).

---

## §1 Ayar katmanlari

| Katman | Ornek | Kim yazar | Nerde durur |
|---|---|---|---|
| Uygulama ayarlari | tema, son dosya, panel boyu | kullanici | `settings.json` (per-user) |
| Makine ayarlari | instalasyon yolu, lisans yolu | kurulum | registry / installer |
| Gizli bilgi | API anahtari, parola | sistem | **bu dosyada DEGIL** |
| Donem verisi | oturum, proje | kullanici | uygulamaya ozel veri dosyasi (ADR-005) |

---

## §2 Arayuz ve atomik yazim

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

| Bicim | `net48` | `net10.0-windows` |
|---|---|---|
| Kutudan cikan JSON | `DataContractJsonSerializer` (`System.Runtime.Serialization`) | `System.Text.Json` (in-box) |
| Paket gereken alternatif | `System.Text.Json` NuGet = onay gerekir | - |
| Klasik alternatif | `Properties.Settings` + `app.config` `userSettings` | ayni bolum, tasima maliyeti yuksek |

Ayni `settings.json` dosyasi iki bacakta da **ayni anahtarlarla** okunur; JSON uretim bicimi `#if` ile yalnizca o satirda ayrilir.

---

## §3 Guvenlik ve dayaniklilik

| Kurul | Neden | Uygulama |
|---|---|---|
| Sifre/anahtar dosyaya girmez | dosya okunabilir = siza | ortam degiskeni / kimlik dogrulayicisi |
| Atomik yazim | elek kesintisi = bozuk JSON | tmp + `File.Replace` |
| UTF-8, BOM'suz | iki bacak ayni bayti okur | `new UTF8Encoding(false)` |
| Surum alani | ileride goc icin | `"version": 1` anahtari |
| Hata sessiz yutulmaz | kayip fark edilmez | okuma hatasi -> log + varsayilan don |

---

## §4 Yapilmayanlar

| # | Yasak | Sonuc | Duzeltme |
|---|---|---|---|
| 1 | Sirlari `settings.json`'a yazmak | dosya siza | ortam degiskeni |
| 2 | Dogrudan `File.WriteAllText` (arayuzsuz) | bicim iki bacakta dagilir | `ISettingsStore` |
| 3 | Atomik olmayan yazim | yarim dosya | tmp + `File.Replace` |
| 4 | Okuma hatasini yutup varsayilana sessiz donmek | kayip fark edilmez | log + acik donus |
| 5 | Form ici `new SettingsStore()` | bagimlilik gizlenir | composition root ([[knowledge/winforms/composition-root-host-startup|composition-root-host-startup]]) |

---

## §5 Kontrol listesi

| # | Adim | Kanit |
|---|---|---|
| 1 | Tek `ISettingsStore` arayuzu var | grep `interface ISettingsStore` = 1 |
| 2 | Yazim atomik (tmp + Replace) | kod incelemesi |
| 3 | Sifre/anahtar icerigi grep ile yok | tarama = 0 eslesme |
| 4 | `#if` yalnizca JSON uretimi satiri | grep `#if` |
| 5 | Iki bacakta `dotnet build` -> 0 hata | build ciktisi |

---

## §9 Kanit

| Kaynak dosya | Ne alindi |
|---|---|
| `knowledge\winforms\settings-user-config.md` (kaynak vault) | `ISettingsStore`, atomik yazim kodu, katman tablosu, iki bacak JSON farki |
| `.rules\configuration-rules.md` | yapilandirma dosyasi kurali |
| `.rules\security-rules.md` | sirlarin dosyaya yazilmamasi |
| `.ai\.decisions\accepted\ADR-005-ado-net-dapper-no-ef-core.md` | donem verisi saklama yonu |

> Kanitsiz iddia icin `DOĞRULAMA GEREKLI` yazilir; bu dosyada kaynak disi bilgi yoktur.
