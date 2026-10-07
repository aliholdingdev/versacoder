---
title: "clipboard-operations - VersaCoder"
type: knowledge
category: winforms
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "WinForms clipboard: STAThread zorunlulugu, metin formatlari, koparma denemeleri (retry), guvenlik ve net48 + net10.0-windows farklari"
---

# clipboard-operations — Clipboard okuma / yazma

**Zorunlu Baglantilar:** [index](../index.md) · [security-rules](../../.rules/security-rules.md) · [coding-standards](../../.rules/coding-standards.md) · [drag-drop-basics](drag-drop-basics.md) · [exception-boundary-ui](exception-boundary-ui.md)

> **Tek cumlelik sorumluluk:** *Clipboard'dan nasil okunur, nasil yazilir, meşgul oldugunda ne yapilir* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Clipboard **STA thread** ister: `Main` metodunda `[STAThread]` olmali (WinForms sablonu zaten koyar); nerede olusturuldugu belli olmayan bir thread'den cagirmak `ThreadStateException` verir. Metin icin **`UnicodeText`** formati esas alinir (UTF-16, Turkce karakter guvenli); okumada bos metin `null` degil `string.Empty` sayilir. Clipboard baska uygulama tarafindan **kisa sureli meşgul** olabilir -> `Clipboard` cagrilari kisa **deneme dongusu** (retry) ile sarilir. Guvenlik: clipboard'a gelen metin **ham veridir**, dogrulanmadan calistirilmaz/saklanmaz (security-rules).

---

## §1 Temel islemler

| Islem | Cagri | Not |
|---|---|---|
| Metin oku | `Clipboard.GetText()` | bos ise `string.Empty` |
| Metin yaz | `Clipboard.SetText(s)` | bos metin `ArgumentException` verir |
| Nesne yaz | `Clipboard.SetDataObject(obj, true)` | `copy=true` -> uygulama kapansin da kalir |
| Format kontrolu | `Clipboard.ContainsText()` | okumadan once bak |
| Temizle | `Clipboard.Clear()` | kullanici onayi ile |

```csharp
static string SafeGetText()
{
    for (var i = 0; i < 5; i++)
    {
        try { return Clipboard.GetText(TextDataFormat.UnicodeText) ?? string.Empty; }
        catch (ExternalException) { Thread.Sleep(20); }   // baska uygulama meşgul
    }
    return string.Empty;   // log WARN -> kullaniciya bilgi
}
```

> **KURAL:** Clipboard hatasi **uygulama hatasi degildir**; 5 deneme sonunda bos donulur ve `WARN` loglanir. Sessizce yutulmaz (G5).

## §2 Formatlar

| Format | Ne taşir | Ne zaman |
|---|---|---|
| `TextDataFormat.UnicodeText` | UTF-16 metin | varsayilan metin yolu |
| `DataFormats.Text` | ASCII/ANSI metin | eski uygulama uyumu |
| `DataFormats.FileDrop` | `string[]` yol | dosya kopyalama (bkz. drag-drop) |
| Ozel format | `DataObject` + kendi anahtari | uygulama ici kopyala-yapistir |

| Guvenlik | Uygulama |
|---|---|
| Gelen metni calistirma | yapistirilan metin veri olarak okunur, `Process.Start` degil |
| Boyut siniri | asiri buyuk paste = donme -> uzunluk kontrolu |
| Sirlari panoya koyma | anahtar/parola `SetText` edilmez |
| Panodan otomatik okuma | yalniz acikca kullanici eylemi ile |

## §3 Iki bacak: net48 vs net10.0-windows

| Konu | net48 | net10.0-windows |
|---|---|---|
| `[STAThread]` | **zorunlu** (`Main` uzerinde) | **zorunlu** - degismedi |
| `Clipboard.GetText` / `SetText` | `System.Windows.Forms` - ayni | ayni API |
| `SetDataObject(obj, copy)` | COM uzerinden, `copy=true` gerekir | ayni imza |
| Apartman modeli | STA tek thread | ayni (`[STAThread]`) |
| C# dili | **7.3** | C# 14 |

`#if` bu dosyada **gerekmez**; API iki bacakta aynidir.

## §4 Yapilmayanlar

| # | Yasak | Sonuc | Duzeltme |
|---|---|---|---|
| 1 | `[STAThread]`'siz `Main` | `ThreadStateException` | sablon atributunu silme |
| 2 | `SetText("")` | `ArgumentException` | bos ise `Clear()` ya da atlama |
| 3 | Tek cagri, retry yok | meşgulken hata | §1 dongusu |
| 4 | Panodan gelen metni isleme | guvenlik | dogrula -> sonra isle |
| 5 | Clipboard'i UI disi thread'den cagirmak | yakalanmaz hata | `Control.BeginInvoke` |

## §5 Kardes Dosyalar

| Konu | Dosya |
|---|---|
| Surukle-birak (dosya formati ayni) | [drag-drop-basics](drag-drop-basics.md) |
| Hata siniri | [exception-boundary-ui](exception-boundary-ui.md) |
| Geri-al / yeniden-isle komutlari | [command-pattern-mvvm](command-pattern-mvvm.md) |
| Acilis (STAThread nerede) | [startup-pipeline-ui](startup-pipeline-ui.md) |
