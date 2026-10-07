---
title: "pictureedit-upload - VersaCoder"
type: knowledge
category: devexpress
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "PictureEdit ile gorsel gosterimi, dosya secimi/onomasi (upload) ve gorsel dogrulamasi - SizeMode, MenuManager, boyut/format kontrolu"
---

# pictureedit-upload - PictureEdit ve Gorsel Yukleme

**Zorunlu Baglantilar:** [vault index](../../index.md) · [knowledge index](../index.md) · [devexpress-rules](../../.rules/devexpress-rules.md) · [ADR-004](../../.decisions/accepted/ADR-004-devexpress-universal-v261-office2010.md) · [memoedit-richedit](memoedit-richedit.md)

> **Tek cumlelik sorumluluk:** *Bir gorselin PictureEdit'te nasil gosterilecegi, dosya seciminin nasil yapilacagi ve gorselin nasil dogrulanip saklanacagi* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

`PictureEdit` gorseli `Properties.SizeMode` (`PictureSizeMode.Zoom` / `Squeeze`) ile gosterir; gorsel `Image` / `EditValue` uzerinden verilir. "Upload" asamasinda `OpenFileDialog` ile dosya secilir, **uzanti + boyut + boyut siniri** kontrol edilir, kaynak `Image.FromStream` ile (dosya kilidi birakilmadan) acilir. Gorsel veritabanina gömülmez: dosya diskte, veritabaninda **yolu** tutulur - veri erisimi ADO.NET + Dapper + `@p1` (EF Core YASAK). `MenuManager` ile sag tik menusu/yaklastirma cubugu baglanir.

---

## §1 Gorsel Gosterim Secimi

| Ihtiyac | Ayar | Not |
|---|---|---|
| Oran korunarak sigdirma | `PictureSizeMode.Zoom` | en yaygin |
| Alana sigdirma (orani bozmadan) | `PictureSizeMode.Squeeze` | simge/gorsel onizleme |
| Orijinal boyut + kaydirma | `PictureSizeMode.Normal` | buyuk gorsellerde kaydirma gerekir |
| Sabit kutuda rozet/imle | `Anchor` + `AutoSize` | DX-14: sabit `Size` yerine |
| Cok buyuk dosya onizleme | once kucuk kopya | bellek riski |

> **KURAL:** `SizeMode` yalnizca **gorunumdur**; dosya boyutu/sagligi kontrolu yukleme adiminda yapilir, gosterim adiminda degil.

---

## §2 Kod ve Ozellik Tablosu

```csharp
// FormName.cs - dosya sec + gorseli yukle
using (var dlg = new OpenFileDialog { Filter = "Png|*.png|Jpg|*.jpg;*.jpeg" })
{
    if (dlg.ShowDialog() != DialogResult.OK) return;
    using (var fs = System.IO.File.OpenRead(dlg.FileName))
    using (var img = System.Drawing.Image.FromStream(fs))
        this.pictureEditAvatar.Image = new System.Drawing.Bitmap(img);   // kilitsiz kopya
}
this.pictureEditAvatar.Properties.SizeMode =
    DevExpress.XtraEditors.Controls.PictureSizeMode.Zoom;
```

| Ozellik / metot | Ne yapar | Ornek |
|---|---|---|
| `Properties.SizeMode` | gorselin sigdirma bicimi | `PictureSizeMode.Zoom` |
| `Image` | gorsel (`System.Drawing.Image`) | `Bitmap` |
| `EditValue` | gorselin nesne hali (`BaseEdit`) | `Image` |
| `Properties.MenuManager` | sag tik menusu + zoom cubugu | `BarManager` |
| `OpenFileDialog.Filter` | uzanti siniri | `"Png|*.png"` |

**Diyagram ( yukleme hatti):**

```
 OpenFileDialog --> uzanti + boyut kontrolu --> Image.FromStream (kilitsiz)
                                                     |
                                        PictureEdit.SizeMode = Zoom
                                                     |
                                   dosya: diskte   |   yol: Dapper @p1
                                   (EF Core YASAK) |   (parameterized)
```

---

## §3 net48 ve net10.0-windows'da fark

| Konu | net48 | net10.0-windows |
|---|---|---|
| Dil surumu | C# 7.3 - `using` ifadesi donus degeri **yok** | C# 14 - `using` deger dondurur |
| `System.Drawing` | GAC / Framework | `System.Drawing.Common` (yalniz Windows) |
| Binding redirect | `App.config` zorunlu (DX-06) | gerekmez |
| DPI | `App.manifest` ile otomatik | `Application.SetHighDpiMode` ile |
| Varsayilan font | 8.25pt | 9pt - gorsel yanindaki etiket genisler |

```csharp
#if NETFRAMEWORK
    Application.EnableVisualStyles();                     // net48: yeterli
#else
    Application.SetHighDpiMode(HighDpiMode.SystemAware);  // net10: DPI acilir
#endif
```

> **KURAL:** `#if` yalnizca bu tip **platform API farki** icin kullanilir; gorsel yukleme kodu iki bacakta da aynidir.

---

## §4 Dogrulama (DataAnnotations + DevExpress Validation)

```csharp
public class AvatarDraft
{
    [StringLength(120, ErrorMessage = "Yol en fazla 120 karakter")]
    public string ImagePath { get; set; }     // net10: string?
}
```

```csharp
string[] izinli = { ".png", ".jpg", ".jpeg" };
bool gecerli = izinli.Contains(System.IO.Path.GetExtension(dlg.FileName).ToLowerInvariant())
            && new System.IO.FileInfo(dlg.FileName).Length <= 2 * 1024 * 1024;
this.dxErrorProvider1.SetError(this.pictureEditAvatar,
    gecerli ? string.Empty : "Yalnizca png/jpg, en fazla 2 MB");
```

| Katman | Kural | Arac |
|---|---|---|
| Dosya siniri | uzanti + 2 MB | kod icinde kontrol + `DXErrorProvider` |
| Metin/uzunluk | `[StringLength]` | DataAnnotations (DTO) |
| Boyut/indirme siniri | `[Range]` | DTO (sunucu tarafinda tekrar) |
| Kalici veri | yol kolonu, `@p1` | ADO.NET + Dapper |
| Yasak | gorseli byte kolonuna gommek | EF Core + string-concat yok (`G2`) |

---

## §5 Sik Yapilan Hatalar

| # | Hata | Sonuc | Duzeltme |
|---|---|---|---|
| 1 | `Image.FromFile` kullanmak | dosya kilitlenir, silinemez | `FromStream` + kopya |
| 2 | Uzanti kontrolunu atlamak | zararli dosya | izin listesi + boyut siniri |
| 3 | Dosyayi `byte[]` olarak DB'ye yazmak | buyuk tablo, yavas | dosya + yol |
| 4 | Sabit `Size` + buyuk gorsel | DPI'da kesilir | `SizeMode` + `Anchor` (DX-14) |
| 5 | Tasarim degisikligini kodda yapmak | DX-10 ihlali | `.Designer.cs` yerine `.Form.cs` |
