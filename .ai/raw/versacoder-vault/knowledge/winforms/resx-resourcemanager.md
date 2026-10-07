---
title: "Resx ve ResourceManager - VersaCoder"
type: knowledge
category: winforms
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: ".resx dosya bicimi, ResourceManager cozme zinciri, form metinleri/ic kaynaklari, culture/satellite kurallari ve iki bacak (net48 / net10.0-windows) farklari"
---

# `.resx` ve `ResourceManager`

**Zorunlu Baglantilar:** [knowledge index](../index.md) · [CLAUDE.md](../../CLAUDE.md) · [designer-cs-anatomy](designer-cs-anatomy.md) · [coding-standards](../../.rules/coding-standards.md) · [security-rules](../../.rules/security-rules.md)

> **Tek cumlelik sorumluluk:** *Bir `.resx` nasil XML'den `ResourceManager` ile metne donusur, hangi ad bozulursa form metinleri kaybolur* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

`.resx`, **XML anahtar-deger** deposudur; derleme sirasinda `GenerateResource` onu `.resources` bicimine cevirir ve assembly'e **gomulu kaynak** olarak gomer. Kod icinden okuma tek yoldur: `ResourceManager("DosyaAdi_KaynakSinifi", typeof(Form).Assembly).GetString("anahtar")`. `ResourceManager` adi (namespace + sinif adi) ile `.resx` dosya adi **birebir ayni** olmali; biri degisirse metinler `null` doner ve form bos gorunur. Bu vault'ta `.resx` **`ui-designer` yetkisindedir**; `developer` yalnizca `ResourceManager` sonucunu okur, adi degistirmez.

---

## §1 `.resx` -> `.resources` -> ekran zinciri

| # | Asama | Ureten | Girdi | Cikti |
|---|---|---|---|---|
| 1 | XML yazimi | `ui-designer` | `<data name="..."><value>...</value></data>` | `MainForm.resx` |
| 2 | Derleme | `GenerateResource` (MSBuild) | `.resx` | `.resources` |
| 3 | Gomme | `EmbeddedResource` | `.resources` | assembly ici kaynak |
| 4 | Okuma | `ResourceManager` | anahtar | `string` / `object` |
| 5 | Kultur | satellite assembly | `tr-TR.resx` | kulture ozel deger |

```
MainForm.resx --(GenerateResource)--> MainForm.resources --(embed)--> assembly
      |                                                              |
      +-- culture: MainForm.tr-TR.resources --> satellite dll        |
                                                                     v
Form.cs:  ResourceManager("...Forms.MainForm_Resources", asm).GetString("okButton_Text")
                                                                     |
                                                                     v
                                                          okButton.Text = "Tamam"
```

---

## §2 `ResourceManager` ornegi (bu vault'ta el ile yazilir)

```csharp
using System;
using System.Globalization;
using System.Resources;

namespace VersaCoder.App.Forms
{
    internal static class MainFormResources
    {
        // Ad kurali: <Namespace>.<DosyaAdi>  -> Designer.cs ile AYNI olmali
        private static readonly ResourceManager Rm = new ResourceManager(
            "VersaCoder.App.Forms.MainForm_Resources",
            typeof(MainForm).Assembly);

        public static string OkButtonText
        {
            get { return Rm.GetString("okButton_Text", CultureInfo.CurrentUICulture) ?? "OK"; }
        }

        public static string Greeting(string name)
        {
            string fmt = Rm.GetString("greeting_fmt", CultureInfo.CurrentUICulture) ?? "{0}";
            return string.Format(CultureInfo.CurrentCulture, fmt, name);
        }
    }
}
```

| Parca | Kural | Bozulursa |
|---|---|---|
| `ResourceManager` adi | `<Namespace>.<resx dosya adi>` | `MissingManifestResourceException` |
| `GetString(..., culture)` | UI kulturu, `CurrentCulture` degil | metin dili yanlis |
| `?? "OK"` (fallback) | bos metin asla ekrana gelmez | bos buton yazisi |
| `string.Format` yerine interpolasyon | format metni kaynakta oldugu icin **`string.Format` zorunlu** | kaynakta `{0}` varken interpolasyon calismaz |

> **Dikkat:** Kaynak metni **guvenlik acigi** olabilir (hata mesaji icinde SQL/yonlendirme bilgisi) - `security-rules.md` SEC-*. Metinler **kullaniciya gosterilen** yerde kullanilir; log/SQL tarafina **asla** kaynak metni gecirilmez.

---

## §3 TFM farki (net48 vs net10.0-windows)

| Konu | `net48` | `net10.0-windows` |
|---|---|---|
| `ResourceManager` API | `System.Resources` (mscorlib/System) - ayni | ayni imza |
| Satellite assembly | `<kultur>.dll` yan klasorunde | ayni |
| `NeutralResourcesLanguage` | `AssemblyInfo.cs` elle | `AssemblyInfo`/csproj ozelligi |
| `ResXFileCodeGenerator` | VS classic designer | ayni; `GenerateResource` davranisi ayni |
| Binary kaynak (gorsel) | `System.Drawing` tipi ile gomulur | ayni (Windows Desktop framework) |
| Dosya yolu / kaynak JSON | `EmbeddedResource` + elle okuma | ayni |

> **Sonuc:** `.resx` bicimi ve `ResourceManager` **iki bacakta da birebir aynidir**; fark yalnizca derleyici varsayilanlarindadir. Kaynak kodunda `#if` gerekmez.

---

## §4 C# 7.3 (net48) vs C# 14

```csharp
// --- C# 7.3 (net48) ---
ResourceManager rm = new ResourceManager(
    "VersaCoder.App.Forms.MainForm_Resources",
    typeof(MainForm).Assembly);

string text = rm.GetString("okButton_Text");
if (text == null)
{
    text = "OK";
}

string caption;
switch (culture.Name)
{
    case "tr-TR": caption = "Bilgi"; break;
    default:      caption = "Info";  break;
}
```

```csharp
// --- C# 14 (net10.0-windows) ---
ResourceManager rm = new(
    "VersaCoder.App.Forms.MainForm_Resources",
    typeof(MainForm).Assembly);

string? text = rm.GetString("okButton_Text") ?? "OK";

string caption = culture.Name switch
{
    "tr-TR" => "Bilgi",
    _       => "Info",
};
```

| Ozellik | C# 7.3 | C# 14 | Ortak kaynak |
|---|---|---|---|
| `ResourceManager` cagrisi | tip elle yazilir | target-typed `new()` | imza **ayni**, yalniz yazim farkli |
| Nullable donus | `string` (null sakli) | `string?` | `?? "OK"` her iki bacakta da calisir |
| Switch | `switch` statement | switch expression | expression 8.0+ ozelligi -> ayni dosyada ortak kod olarak **statement** kalir |
| `using` | `using (var r = ...) { }` | `using var r = ...;` | 7.3'te blok zorunlu |

---

## §5 Sik ihlaller

| Ihlal | Belirti | Duzeltme |
|---|---|---|
| `ResourceManager` adi yanlis | `MissingManifestResourceException` | ad = `<Namespace>.<resx dosya adi>` |
| `.resx` elle duzenlenince karmasik XML bozuldu | VS designer acilmiyor | `.resx`'i designer uzerinden ac, elle XML yazma |
| Metin kodda string literal | `CS-07` + ceviri imkansiz | metni `.resx`'e tasi |
| Kultur `CurrentCulture` ile okundu | sayi/tarih metni UI dilinde | `CurrentUICulture` |
| Kaynak metni SQL'e gecti | `DA-03` / G2 | kaynak yalnizca gosterim; SQL parameterized (`@p1`) |

> **Iliskili:** dosya uclusu icin [designer-cs-anatomy](designer-cs-anatomy.md) · kod yazim kurallari icin [coding-standards](../../.rules/coding-standards.md).
