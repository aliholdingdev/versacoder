---
title: "NavigationFrame Sayfa Gecisleri - VersaCoder"
type: knowledge
category: devexpress
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "NavigationFrame + NavigationPage ile tek ekranli (SDI) sayfa gecisi, QueryControl ile icerik uretimi ve gecis animasyonu"
---

# NavigationFrame Sayfa Gecisleri

**Zorunlu Baglantilar:** [vault index](../../index.md) · [devexpress-rules](../../.rules/devexpress-rules.md) · [ADR-004](../../.decisions/accepted/ADR-004-devexpress-universal-v261-office2010.md) · [bilgi index](../index.md) · [accordion-navigation](accordion-navigation.md) · [tabcontrol-multipage](tabcontrol-multipage.md)

> **Tek cumlelik sorumluluk:** *Ayni anda tek sayfa gosteren, gecisli "ekranlar" kabini nasil kurulur* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

`NavigationFrame`, `NavigationPage`'leri barindiran ve **yalnizca birini** gosteren bir kapsayicidir; ustunde sekm/dugme gibi **hiçbir gezinme elemani yoktur** - gezinmeyi disaridan saglarsiniz (Accordion, TabPane veya kendi butonlariniz). Sayfa secerken `SelectedPage` (nesne) veya `SelectedPageIndex` (int) kullanilir; icerik uretimini tembel yapmak icin `QueryControl` olayi olayi yakalanir (sayfa gorunur olacaginda kontrol olusturulur). Gecis animasyonu `AllowTransitionAnimation` + `TransitionType` ile acilir. Alternatif olarak `TabPane`, ayni sayfalari **dugmeli basliklarla** gosterir.

---

## §1 Temel kullanim

```csharp
using DevExpress.XtraBars.Navigation;

navigationFrame1.Pages.AddRange(new NavigationPage[]
{
    new NavigationPage(), new NavigationPage()
});

navigationFrame1.AllowTransitionAnimation = true;
navigationFrame1.SelectedPage = navigationPageAyarlar;   // ya da SelectedPageIndex = 1;
```

```
Form
  +-------------+---------------------------------+
  | Accordion   | NavigationFrame (Dock=Fill)      |
  |  oturumlar  |   +---------------------------+ |
  |  ayarlar  -->|   | yalnizca tek NavigationPage  | |
  |  log        |   +---------------------------+ |
  +-------------+---------------------------------+
   gezinme disarida     QueryControl -> icerik uretimi
```

---

## §2 Ozellik tablosu

| Nesne / ozellik | Tur | Ne is yapar |
|---|---|---|
| `NavigationFrame.Pages` | koleksiyon | Sayfa sahibi (`AddRange`) |
| `NavigationPage` | tur | Icerik tasiyici; uzerine kontrol dock'lanir |
| `NavigationFrame.SelectedPage` | nesne | Aktif sayfayi okur/ayarlar |
| `NavigationFrame.SelectedPageIndex` | int | Index ile sayfa secimi |
| `NavigationFrame.QueryControl` | olay | Sayfa lazimken icerigi uret (tembel yukleme) |
| `AllowTransitionAnimation` | `bool` | Gecis animasyonunu acar |
| `TransitionType` | nesne | Animasyon tipi |
| `TabPane` + `TabNavigationPage` | tur | Ayni model + gorunur gezinme butonlari |
| `TabNavigationPage.Caption` | `string` | Dugme yazisi |

---

## §3 net48 ve net10.0-windows'da fark

| Konu | `net48` | `net10.0-windows` |
|---|---|---|
| Kontrol olusturma | Designer tercih; kod `new NavigationFrame()` serbest | Ayni |
| Sayfa icerigi | UserControl uctan olusur | Ayni (`partial` + Designer) |
| Animasyon surumu | `DevExpress.Utils` animasyon altyapisi | Ayni; `#if` yalniz platform farkinda |
| Nullable | `SelectedPage` null gelebilir (bos sayfa) | `Nullable=enable` ise `SelectedPage?` dikkat |
| Cikis/kayit | Sayfa durumu XML'e yazilabilir | Ayni; [docklayout-persistence](docklayout-persistence.md) |

---

## §4 Yaygin hatalar

| # | Hata | Sonuc | Duzeltme |
|---|---|---|---|
| 1 | Frame icine tablo/dugme ekleyip gezinme beklemek | Gezinme elemani yok | Accordion/TabPane disarida |
| 2 | Her gezinmede `new NavigationPage()` | Sayfa yigilisi, bellek sizintisi | Tek uretim, `QueryControl` |
| 3 | Sayfa icerigini `Form_Load`'ta tam doldurmak | Yavas acilis | `QueryControl` ile tembel uretim |
| 4 | Animasyonu kapatmadan test yazmak | Zaman bazli test kirilgan | `AllowTransitionAnimation=false` testte |
| 5 | `SelectedPageIndex`'i kontrol sayisindan buyuk yazmak | Exception | Index kontrolu |

---

## §5 Kanit ve dogrulama

| Kalem | Durum |
|---|---|
| `Pages`, `SelectedPage`, `SelectedPageIndex`, `QueryControl`, `AllowTransitionAnimation` | v26.1 WinForms dokumantasyonu ile **dogrulandi** |
| `TransitionType` concrete tipleri | `NEEDS_VALIDATION` (designer listesinden sec) |
| `dotnet build` | Bu batch'te alinmadi → `NEEDS_VALIDATION` |
| Ilgili kural | DX-09 (ekranlar arasi gecis kabini) |

**Kardesler:** [accordion-navigation](accordion-navigation.md) · [tabcontrol-multipage](tabcontrol-multipage.md) · [tile-control-startpage](tile-control-startpage.md) · [layoutcontrol-grouping](layoutcontrol-grouping.md)
