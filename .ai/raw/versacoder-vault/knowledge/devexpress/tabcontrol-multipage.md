---
title: "XtraTabControl Coklu Sayfa Duzeni - VersaCoder"
type: knowledge
category: devexpress
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "XtraTabControl + XtraTabPage ile sekme duzeni: baslik konumu, cok satir, gezinme butonlari ve secim olayi"
---

# XtraTabControl Coklu Sayfa Duzeni

**Zorunlu Baglantilar:** [vault index](../../index.md) · [devexpress-rules](../../.rules/devexpress-rules.md) · [ADR-004](../../.decisions/accepted/ADR-004-devexpress-universal-v261-office2010.md) · [bilgi index](../index.md) · [navigationframe-pages](navigationframe-pages.md) · [dockmanager-panels](dockmanager-panels.md)

> **Tek cumlelik sorumluluk:** *Bir formun icinde bircok sayfayi sekme olarak nasil gosterdiginiz ve secim olayini nereden yakaladiginiz* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

`XtraTabControl`, sayfalarini `TabPages` koleksiyonunda tutar; her `XtraTabPage` bir `Text` (sekme yazisi) ve icine dock'lanmis kontroller tasir. Basliklar sigmazsa `MultiLine = true` ile cok satira, `HeaderLocation`/`HeaderOrientation` ile konuma/orientasyona gorelenir; sigmayan basliklar icin `HeaderButtons = TabButtons.Prev | TabButtons.Next` gezinme butonlari eklenir. Secim olayi **tek** yerden yakalanir: `SelectedPageChanged` (`TabPageChangedEventArgs.Page` / `.PrevPage`). `PageVisible` ile sayfa gizlenir, `PageEnabled` ile pasiflestirilir.

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

## §3 net48 ve net10.0-windows'da fark

| Konu | `net48` | `net10.0-windows` |
|---|---|---|
| API | Ayni (`XtraTabControl` her iki bacakta da) | Ayni |
| C# surumu | 7.3: anonim delegate ile olay | 14: `(s, e) => ...` ayni calisir |
| Nullable | `SelectedPage` null dondurmeyebilir | `Nullable=enable` ise `e.Page?` dikkat |
| DPI | Baslik ikonlari sabit 16px | Otomatik olcek; `AutoScaleMode` sabit (DX-14) |
| `#if` | Yalniz DPI/platform farkinda | Ayni; sekme mantigi #if altina ALINMAZ |

---

## §4 Yaygin hatalar

| # | Hata | Sonuc | Duzeltme |
|---|---|---|---|
| 1 | Sayfa icerigini kodda `new` ile doldurmak | Designer ile carpisma | Designer'da ekle, kodda sadece olay |
| 2 | `SelectedTabPageIndex++` ile dongu | Sinir asimi (`Count`) | Index kontrolu ile |
| 3 | Her sekme icin ayri `PageChanged` handler | God form | Tek handler, `e.Page` ile dallan |
| 4 | `MultiLine=true` + dar panel | Basliklar 3 satira kayar | Once `HeaderButtons` dene |
| 5 | Sayfayi silmek yerine `Visible=false` (Control) | Yer kaplar | `PageVisible = false` |

---

## §5 Kanit ve dogrulama

| Kalem | Durum |
|---|---|
| `MultiLine`, `HeaderLocation`, `HeaderButtons`, `SelectedPageChanged` (`TabPageChangedEventArgs`) | v26.1 WinForms dokumantasyonu ile **dogrulandi** |
| `CloseButtonMode` / sayfa kapatma butonu | `NEEDS_VALIDATION` (yerine `CustomHeaderButtons` kullanildi) |
| `dotnet build` | Bu batch'te alinmadi → `NEEDS_VALIDATION` |
| Ilgili kural | DX-09 (Editor = `XtraTabControl` + editor) |

**Kardesler:** [navigationframe-pages](navigationframe-pages.md) · [dockmanager-panels](dockmanager-panels.md) · [layoutcontrol-grouping](layoutcontrol-grouping.md) · [ribbon-form-basics](ribbon-form-basics.md)
