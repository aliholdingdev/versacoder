---
title: "printing-preview - VersaCoder"
type: knowledge
category: winforms
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "PrintDocument + PrintPreviewDialog sayfalama modeli, DevExpress grid yazdirma baglantisi, kaynak temizligi ve net48 + net10.0-windows farklari"
---

# printing-preview — Baski ve onizleme (PrintDocument)

**Zorunlu Baglantilar:** [index](../index.md) · [devexpress-rules](../../.rules/devexpress-rules.md) · [coding-standards](../../.rules/coding-standards.md) · [file-dialogs-io](file-dialogs-io.md) · [exception-boundary-ui](exception-boundary-ui.md)

> **Tek cumlelik sorumluluk:** *Bir rapor/ekran nasil sayfalanir, onizleme ve baski nasil tetiklenir* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Baski `PrintDocument` uzerinden calisir: `BeginPrint` (durum sifirla) -> `PrintPage` (tek sayfa ciz, `HasMorePages` ile devam) -> `EndPrint` (kaynak temizle). Sayfalama **uydurulmaz**: kacildigi yer degiskenleri `BeginPrint`'te sifirlanir, `PrintPage` icinde ilerletilir. Onizleme `PrintPreviewDialog` ile yapilir, gercek baski `Print()` ile. **`Graphics` / `Font` / `Brush` `PrintPage` icinde dispose edilir** - her sayfada yeni uretilir. DevExpress grid tarafinda yazdirma onizlemesi `GridControl` uzerindedir; kesin uye adi lisansli kurulumda dogrulanir (`NEEDS_VALIDATION`).

---

## §1 Sayfalama modeli

| Olay | Ne yapar | Durum |
|---|---|---|
| `BeginPrint` | sayfa sayaci, `y` ofseti sifirla | uye alanlarini hazirla |
| `PrintPage` | tek sayfa cizimi | `HasMorePages = true` -> devam |
| `EndPrint` | font/brush temizleme | yarim kalan kayit yok |
| `PrintProgress` | icerik onizlemesi (varsa) | `PrintPreviewDialog` icin |

```csharp
void doc_PrintPage(object sender, PrintPageEventArgs e)
{
    var font = new Font("Segoe UI", 10f);
    try
    {
        float y = _startY;                       // BeginPrint'te sifirlanir
        foreach (var line in _lines)
        {
            if (y + font.Height > e.MarginBounds.Bottom)
            {
                _startY = y;                     // kaldigi yerden devam
                e.HasMorePages = true;
                return;
            }
            e.Graphics.DrawString(line, font, Brushes.Black, e.MarginBounds.Left, y);
            y += font.Height;
        }
        _startY = 0;
        e.HasMorePages = false;
    }
    finally { font.Dispose(); }
}
```

> **KURAL:** `e.HasMorePages` `false` birakilirsa baski 1 sayfada biter ve hata da vermez - en sik gozden kacan.

## §2 Onizleme ve tetikleme

| Adim | Cagri | Not |
|---|---|---|
| Onizleme | `previewDlg.Document = doc; previewDlg.ShowDialog(this);` | modal, baski yapmaz |
| Doğrudan baski | `doc.Print()` | yazici secimi dialogsuz |
| Yazici secimi | `doc.PrinterSettings` + `PrintDialog` | kullanici tercihi |
| Kenar boşlugu | `PrintPageEventArgs.MarginBounds` | `DefaultPageSettings` |

DevExpress tarafinda grid raporu icin ayri bir yazdirma sistemi vardir (`DevExpress.XtraPrinting`); hangi kontrolun hangi uye ile tetikleneceği **NEEDS_VALIDATION** olarak isaretlenir - uydurma uye adi yazilmaz (G9).

## §3 Iki bacak: net48 vs net10.0-windows

| Konu | net48 | net10.0-windows |
|---|---|---|
| `System.Drawing.Printing` | kutuda (`System.Drawing`) | `System.Drawing.Common` - yalniz Windows (`-windows` TFM'i karsilar) |
| `PrintDocument` / `PrintPreviewDialog` | ayni API | ayni API |
| `PrintDialog` | ayni | ayni |
| Yazici surucusu / `PrinterSettings` | isletim sistemi bagimli | ayni - fark surucude, kodda degil |
| C# dili | **7.3** - `using` deyimi ile kaynak temizligi | C# 14 (ayni dosya 7.3'te kalir) |

`#if` gerekmez; fark yalniz paket adindadir ve `net10.0-windows` TFM'i zaten Windows-only bagimliligi isaretler.

## §4 Yapilmayanlar

| # | Yasak | Sonuc | Duzeltme |
|---|---|---|---|
| 1 | `HasMorePages` unutulmasi | tek sayfa baski | §1 sablonu |
| 2 | Sayfalama degiskenini `PrintPage`'de sifirlamak | sonsuz dongu | `BeginPrint` |
| 3 | Font'u dispose etmemek | handle sizintisi | `finally { font.Dispose(); }` |
| 4 | Uydurma DevExpress uye adi | derleme hatasi / G9 | `NEEDS_VALIDATION` + dogrulama |
| 5 | Baskiyi UI thread disinda tetiklemek | beklenmeyen hata | `BeginInvoke` ya da olay icinde |

## §5 Kardes Dosyalar

| Konu | Dosya |
|---|---|
| Dosya kaydi (PDF/rapor disari) | [file-dialogs-io](file-dialogs-io.md) |
| Grid veri kaynagi (Dapper) | [index](../index.md) |
| DevExpress kontrol kurallari | [ribbonform-basics](ribbonform-basics.md) |
| Baski hatasi yakalama | [exception-boundary-ui](exception-boundary-ui.md) |
