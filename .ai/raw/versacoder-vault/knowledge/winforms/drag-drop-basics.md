---
title: "drag-drop-basics - VersaCoder"
type: knowledge
category: winforms
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "WinForms surukle-birak temeli: AllowDrop, DragEnter/DragOver/DragDrop, DataFormats.FileDrop guvenlik kontrolu ve net48 + net10.0-windows koordinat/DPI farklari"
---

# drag-drop-basics — Surukle-birak (dosya ve icerik)

**Zorunlu Baglantilar:** [index](../index.md) · [keys](../../keys.md) · [security-rules](../../.rules/security-rules.md) · [coding-standards](../../.rules/coding-standards.md) · [file-dialogs-io](file-dialogs-io.md)

> **Tek cumlelik sorumluluk:** *Bir kontrole dosya/icerik suruklendiginde hangi olay sirayla calisir ve ne dogrulanir* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Surukle-birak **uc olaydan** olusur ve sira degistirilemez: `DragEnter` (efekt secimi) -> `DragOver` (imlecp konumu, gerekirse) -> `Drop` (veri alinir). Hedef kontrolde `AllowDrop = true` **zorunlu**; yoksa hicbir olay tetiklenmez. Gelen veri **ham veridir**: dosya yolu, uzantisi ve boyutu **drop oncesinde** dogrulanir (security-rules) - dogrulanmamis drop = harici veri ile is yapma. Iki bacakta API aynidir; fark DPI koordinatlaridir.

---

## §1 Olay sirasi

| Olay | Ne ogrenilir | Ne yapilir |
|---|---|---|
| `DragEnter` | `e.Data` formatlari, `KeyState` | `e.Effect = DragDropEffects.Copy` (ya da `None`) |
| `DragOver` | guncel imlec konumu | suruklenen satirin gidecegi yeri isaretle (gerekirse) |
| `DragDrop` | **gercek veri** (`GetData`) | yolu dogrula -> kabuga gonder |

```csharp
public MainForm()
{
    InitializeComponent();
    this.AllowDrop = true;                       // 1) hedef izni
    this.DragEnter += OnDragEnter;
    this.DragDrop  += OnDragDrop;
}

static void OnDragEnter(object sender, DragEventArgs e)
{
    var ok = e.Data != null && e.Data.GetDataPresent(DataFormats.FileDrop);
    e.Effect = ok ? DragDropEffects.Copy : DragDropEffects.None;
}

void OnDragDrop(object sender, DragEventArgs e)
{
    var paths = (string[])e.Data.GetData(DataFormats.FileDrop);   // ham veri
    foreach (var p in paths)
    {
        var ext = Path.GetExtension(p);
        if (!IsAllowedExtension(ext)) continue;                   // beyaz liste
        _shell.OpenFile(Path.GetFullPath(p));
    }
}
```

## §2 Formatlar ve guvenlik

| Format | Icerik | Kullanim |
|---|---|---|
| `DataFormats.FileDrop` | `string[]` yol listesi | dosya acma |
| `DataFormats.Text` / `UnicodeText` | metin | editore birakma |
| Ozel format (uygulama ici) | `DataObject` + kendi anahtari | satir tasma (grid ici) |

| Kontrol | Neden |
|---|---|
| Uzantı beyaz listesi | calistirilabilir dosya geldi mi diye bak |
| `Path.GetFullPath` | `..\..\` ile disari tasma |
| Klasor mu dosya mu | `Directory.Exists` / `File.Exists` |
| Boyut siniri | cok buyuk dosya = donme |
| Drop -> **onay** | kapsam disi acma G7'ye gidebilir |

> **YASAK:** Gelen yolu dogrudan `Process.Start` ile acmak; ham veriyi dogrulamadan saklamak.

## §3 Iki bacak: net48 vs net10.0-windows

| Konu | net48 | net10.0-windows |
|---|---|---|
| `AllowDrop` / `DragEnter` / `Drop` | ayni API | ayni API |
| `DataFormats.FileDrop` | ayni | ayni |
| DPI farki | manifest yoksa isletim sistemi olcekmeleri devreye girer, `PointToClient` sonucu kayabilir | high-DPS varsayilan acik (kaydi yok) |
| `#if` geregi | yalniz DPI manifesti notu | — |
| C# dili | **7.3** | C# 14 |

Koordinat hesabi gerekirse (`DragEventArgs` `X`/`Y` -> ekran -> istemci):

```csharp
var screen = new Point(e.X, e.Y);
var client = this.PointToClient(screen);   // DPI farkinda tek dogru donusum
```

## §4 Yapilmayanlar

| # | Yasak | Sonuc | Duzeltme |
|---|---|---|---|
| 1 | `AllowDrop` unutulmasi | hicbir olay calismaz | hedef kontrolde ac |
| 2 | Veriyi `DragEnter`'de almak | erken / bos veri | orada yalniz format bak, `Drop`'ta al |
| 3 | `e.Effect = None` demeden is yapma | kullanici yanlis efekt gorur | efekt secimi `DragEnter`'de |
| 4 | Ham yol ile is | guvenlik | §2 tablosu |
| 5 | Drop sonrasi dispose edilmis form | gec hata | havuz (`form-navigation-patterns` §2) |

## §5 Kardes Dosyalar

| Konu | Dosya |
|---|---|
| Dosya acma dialogu + yol dogrulama | [file-dialogs-io](file-dialogs-io.md) |
| Paneller arasi gecis | [form-navigation-patterns](form-navigation-patterns.md) |
| Clipboard'a kopyalama | [clipboard-operations](clipboard-operations.md) |
| Hata siniri (drop hatasi) | [exception-boundary-ui](exception-boundary-ui.md) |
