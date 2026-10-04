---
title: "tabcontrol-multipage - VersaCoder"
type: knowledge
category: devexpress
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "XtraTabControl + XtraTabPage ile sekme duzeni: baslik konumu, cok satir, gezinme butonlari ve secim olayi"
---

# XtraTabControl Coklu Sayfa Duzeni

**Zorunlu Baglantilar:** [vault indeks](../index.md) · [devexpress-rules](../../.rules/devexpress-rules.md) · [dockmanager-panels](dockmanager-panels.md) · [layoutcontrol-grouping](layoutcontrol-grouping.md) · [ribbon-form-basics](ribbon-form-basics.md) · [designer-runtime-customization](designer-runtime-customization.md)

> **Tek cumlelik sorumluluk:** *Bir formun icinde bircok sayfayi sekme olarak nasil gosterdiginiz ve secim olayini nereden yakaladiginiz* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

`XtraTabControl`, sayfalarini `TabPages` koleksiyonunda tutar; her `XtraTabPage` bir `Text` (sekme yazisi) ve icine dock'lanmis kontroller tasir. Basliklar sigmazsa `MultiLine = true` ile cok satira, `HeaderLocation` / `HeaderOrientation` ile konuma/yone gorelenir; sigmayan basliklar icin `HeaderButtons = TabButtons.Prev | TabButtons.Next` gezinme butonlari eklenir. Secim olayi **tek** yerden yakalanir: `SelectedPageChanged` (`TabPageChangedEventArgs.Page` / `.PrevPage`). `PageVisible` ile sayfa gizlenir, `PageEnabled` ile pasiflestirilir.

```text
+------------------------------------------------------------------+
| TabPages koleksiyonu  -> sayfa sahibi (Designer'da eklenir)       |
| MultiLine / Header*   -> baslik sigmayinca konum/yon/gezinme      |
| SelectedPageChanged   -> TEK handler; e.Page / e.PrevPage         |
| PageVisible / Enabled -> gizle / pasiflestir (Control degil)      |
+------------------------------------------------------------------+
```

---

## §1 Temel kullanim

```csharp
using DevExpress.Utils;
using DevExpress.XtraTab;

xtraTabControl1.MultiLine = false;
xtraTabControl1.HeaderLocation = HeaderLocation.Top;
xtraTabControl1.HeaderButtons = TabButtons.Prev | TabButtons.Next;
xtraTabControl1.SelectedPageChanged += (s, e) => OnTabPageChanged(e.Page);

xtraTabControl1.SelectedTabPage = xtraTabControl1.TabPages["Editor"];
```

---

## §2 Ozellik tablosu

| Nesne / ozellik | Tur | Ne is yapar |
|---|---|---|
| `XtraTabControl.TabPages` | koleksiyon | Sayfa sahibi; `TabPages.Add(page)` |
| `XtraTabPage.Text` | `string` | Sekme basligi |
| `XtraTabPage.PageVisible` | `bool` | Sayfayi gizler |
| `XtraTabPage.PageEnabled` | `bool` | Sayfayi pasif yapar |
| `SelectedTabPage` / `SelectedTabPageIndex` | nesne / int | Aktif sayfa (index ile de secilir) |
| `SelectedPageChanged` | olay | `TabPageChangedEventArgs.Page`, `.PrevPage` |
| `MultiLine` | `bool` | Basliklar tek satirma sigmazsa cok satir |
| `HeaderLocation` / `HeaderOrientation` | enum (`DevExpress.Utils`) | Baslik konumu ve yonu |
| `TabPageWidth` | int | Baslik genisligini elle kontrol |
| `HeaderButtons` | `TabButtons` | Prev/Next gibi gezinme butonlari |
| `HeaderButtonsShowMode` | `TabButtonShowMode` | Butonlarin gorunme kosulu |
| `CustomHeaderButtons` | koleksiyon | Ozel buton (orn. kapat) ekleme |
| `AppearancePage` | appearance | Sayfa basligi gorunumu |

---

## §3 Yaygin hatalar

| # | Hata | Sonuc | Duzeltme |
|---|---|---|---|
| 1 | Sayfa icerigini kodda `new` ile doldurmak | Designer ile carpisma | Designer'da ekle, kodda sadece olay |
| 2 | `SelectedTabPageIndex++` ile dongu | Sinir asimi (`Count`) | Index kontrolu ile |
| 3 | Her sekme icin ayri `PageChanged` handler | God form | Tek handler, `e.Page` ile dallan |
| 4 | `MultiLine = true` + dar panel | Basliklar 3 satira kayar | Once `HeaderButtons` dene |
| 5 | Sayfayi silmek yerine `Visible = false` (Control) | Yer kaplar | `PageVisible = false` |

---

## §4 Kim neyi degistirir

| Istenen | Dokunulacak yer | Dikkat |
|---|---|---|
| Sekme sayisi/icerigi | `TabPages` (Designer) | kodda `new` yok |
| Aktif sayfa | `SelectedTabPage` / `SelectedTabPageIndex` | index kontrolsuz artmasin |
| Baslik konumu | `HeaderLocation` / `HeaderOrientation` | enum `DevExpress.Utils` |
| Sigmayan baslik | `HeaderButtons` -> `MultiLine` | once gezinme butonu |
| Secim olayi | tek `SelectedPageChanged` | handler sayisi 1 |

---

## §5 net48 ve net10.0-windows'da fark

| Konu | `net48` | `net10.0-windows` |
|---|---|---|
| API | Ayni (`XtraTabControl` her iki bacakta da) | Ayni |
| C# surumu | 7.3: anonim delegate ile olay | 14: `(s, e) => ...` ayni calisir |
| Nullable | `SelectedPage` null dondurmeyebilir | `Nullable=enable` ise `e.Page?` dikkat |
| DPI | Baslik ikonlari sabit 16px | Otomatik olcek; `AutoScaleMode` sabit (DX-14) |
| `#if` | Yalniz DPI/platform farkinda | Ayni; sekme mantigi `#if` altina ALINMAZ |

---

## §6 Kanit ve dogrulama durumu

| Kalem | Durum |
|---|---|
| `MultiLine`, `HeaderLocation`, `HeaderButtons`, `SelectedPageChanged` (`TabPageChangedEventArgs`) | v26.1 WinForms dokumantasyonu ile **dogrulandi** (kaynak vault) |
| `CloseButtonMode` / sayfa kapatma butonu | `DOĞRULAMA GEREKLI` (yerine `CustomHeaderButtons` kullanildi) |
| `dotnet build` | Bu batch'te alinmadi -> `DOĞRULAMA GEREKLI` |
| Ilgili kural | DX-09 (Editor = `XtraTabControl` + editor) |

---

## §7 Tarama tablosu

| Aranan | Istenen | Yorum |
|---|---|---|
| `new XtraTabPage` kodda | 0 eslesme | Designer sahipligi (DX-10) |
| `SelectedPageChanged` handler | 1 adet | god form onlenir |
| `SelectedTabPageIndex++` | sinir kontrolu ile | `Count` asimi |
| `Visible = false` (sayfa) | `PageVisible = false` | yer tasarrufu |
| `PageVisible` / `PageEnabled` | ikisi de kullanildiginda farkli anlam | gizle vs. pasif |

---

## §8 Kardes dosyalar

| Konu | Dosya |
|---|---|
| Panel tabanli duzen | [dockmanager-panels](dockmanager-panels.md) |
| Gruplu alan duzeni | [layoutcontrol-grouping](layoutcontrol-grouping.md) |
| Form + ribbon host | [ribbon-form-basics](ribbon-form-basics.md) |
| Designer/runtime siniri | [designer-runtime-customization](designer-runtime-customization.md) |

---

## §9 Kanit

| Kaynak dosya | Ne alindi |
|---|---|
| `knowledge\devexpress\tabcontrol-multipage.md` (kaynak vault) | ozellik tablosu (13 kalem), secim olayi, 5 yaygin hata, TFM/DPI farki |
| `.rules\devexpress-rules.md` | DX-09 (editor yeri), DX-10 (Designer), DX-14 (DPI) |

> Kanitsiz iddia icin DOĞRULAMA GEREKLI yazilir; bu dosyada kaynak disi bilgi yoktur.
