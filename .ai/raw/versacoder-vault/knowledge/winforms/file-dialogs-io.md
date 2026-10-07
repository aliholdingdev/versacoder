---
title: "file-dialogs-io - VersaCoder"
type: knowledge
category: winforms
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "OpenFileDialog/SaveFileDialog/FolderBrowserDialog kullanimi, UTF-8 dosya yazimi, atomik kayit ve net48 + net10.0-windows dialog/API farklari"
---

# file-dialogs-io — Dosya diyaloglari ve guvenli dosya I/O

**Zorunlu Baglantilar:** [index](../index.md) · [security-rules](../../.rules/security-rules.md) · [coding-standards](../../.rules/coding-standards.md) · [settings-user-config](settings-user-config.md) · [logging-integration](logging-integration.md)

> **Tek cumlelik sorumluluk:** *Dosya secimi dialogu nasil kurulur, dosya nasil guvenli yazilir* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Dialog **her zaman `using` icinde** ve `ShowDialog(this)` ile acilir; secim sonrasi yol dogrulanir (genisletilmis yol disari tasma, klasor mu dosya mi, uzantı listesi). Yazim **UTF-8 + atomik** (once `.tmp`, sonra degistirme) - elek kesintisi yarim dosya birakmaz. Dizin olusturma `Directory.CreateDirectory` ile yapilir; var olan silme/yeniden adlandirma **onayli** islemdir (G7). Iki bacakta tek gercek API farki klasor secim dialogudur.

---

## §1 Dialog seti

| Dialog | Ne secer | Kritik ozellik |
|---|---|---|
| `OpenFileDialog` | acilacak dosya (coklu) | `Filter`, `Multiselect`, `CheckFileExists` |
| `SaveFileDialog` | kaydedilecek dosya | `Filter`, `OverwritePrompt`, `DefaultExt` |
| `FolderBrowserDialog` | klasor (net48 yolu) | `SelectedPath`, `ShowNewFolderButton` |
| `OpenFolderDialog` | klasor (net8+ / net10 yolu) | `Multiselect` destekler |

Ornek:

```csharp
using (var dlg = new OpenFileDialog())
{
    dlg.Filter = "VersaCoder projesi (*.vproj)|*.vproj|Tum dosyalar (*.*)|*.*";
    dlg.CheckFileExists = true;
    dlg.RestoreDirectory = true;          // is klasoru degismez
    if (dlg.ShowDialog(this) != DialogResult.OK) return;

    var full = Path.GetFullPath(dlg.FileName);   // yol dogrulama
    _shell.OpenProject(full);
}
```

> **KURAL:** `Filter` ilk girdi varsayilan yapilir; `CheckFileExists = true` yalniz acma icin, kaydetmede `OverwritePrompt = true`.

## §2 Guvenli yazim (kod)

```csharp
public static void WriteAllTextAtomic(string path, string content)
{
    var dir = Path.GetDirectoryName(path);
    if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);

    var tmp = path + ".tmp";
    File.WriteAllText(tmp, content, new UTF8Encoding(false));   // UTF-8, BOM'suz
    if (File.Exists(path)) File.Replace(tmp, path, path + ".bak");
    else File.Move(tmp, path);
}
```

| Kural | Neden | Uygulama |
|---|---|---|
| Yolu `Path.GetFullPath` ile dogrula | `..\` ile disari tasma | acmadan once |
| Uzanti beyaz listesi | makro/calistirilabilir dosya acma | filtre + kontrol |
| Atomik yazim | yarim dosya | tmp + `Replace` |
| `FileStream` `using` ile | handle sizintisi | her acma |
| Silme = onay | geri alinamaz (G7) | kurtarma `.bak` birak |

## §3 Iki bacak: net48 vs net10.0-windows

| Konu | net48 | net10.0-windows |
|---|---|---|
| Klasor secimi | `FolderBrowserDialog` (eski agac gorunumu) | `OpenFolderDialog` (.NET 8+) - **bu tek `#if` farki** |
| Dosya ac/kaydet | ayni API | ayni API |
| Dosya kodlamasi | `UTF8Encoding` - ayni | ayni |
| `File.Replace` | ayni - | ayni |
| C# dili | **7.3** - `using` deyimi zorunlu | C# 14 (ayni dosya 7.3'te kalir) |

```csharp
#if NET48
    using (var dlg = new FolderBrowserDialog()) { /* ... */ }
#else
    using (var dlg = new OpenFolderDialog()) { /* ... */ }
#endif
```

`#if` yalnizca bu API farkindadir (G6); is mantigi bacaklar ortaktir.

## §4 Yapilmayanlar

| # | Yasak | Sonuc | Duzeltme |
|---|---|---|---|
| 1 | Ham yol ile dosya acma | yol hatasi / tasma | `Path.GetFullPath` + dogrulama |
| 2 | Dosyayi dogrudan uzerine yazma | kesinti = veri kaybi | atomik yazim §2 |
| 3 | Log'a parola/anahtar yazma | siza | security-rules maskelemesi |
| 4 | Dialog'u `Show()` yapmak | kapali dialog ile devam | `ShowDialog(owner)` |
| 5 | Kodlama varsayilanina birakmak | iki bacak farkli dosya | UTF-8 BOM'suz |

## §5 Kardes Dosyalar

| Konu | Dosya |
|---|---|
| Ayar dosyasi yazimi | [settings-user-config](settings-user-config.md) |
| Log dosyasi konumu | [logging-integration](logging-integration.md) |
| Surukle-birak ile dosya alma | [drag-drop-basics](drag-drop-basics.md) |
| Ayar ekraninda klasor secimi | [viewmodel-inotify](viewmodel-inotify.md) |
