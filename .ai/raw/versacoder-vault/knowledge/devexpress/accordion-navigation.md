---
title: "AccordionControl Yan Navigasyonu - VersaCoder"
type: knowledge
category: devexpress
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "AccordionControl ile outlook benzeri yan menu: grup/oge hiyerarsisi, secim, arama kutusu ve NavigationFrame'e baglanti"
---

# AccordionControl Yan Navigasyonu

**Zorunlu Baglantilar:** [vault index](../../index.md) · [devexpress-rules](../../.rules/devexpress-rules.md) · [ADR-004](../../.decisions/accepted/ADR-004-devexpress-universal-v261-office2010.md) · [bilgi index](../index.md) · [navigationframe-pages](navigationframe-pages.md) · [tile-control-startpage](tile-control-startpage.md)

> **Tek cumlelik sorumluluk:** *Sol taraftaki acilir-kapanir menu hangi kontrolle kurulur ve bir ogeye tiklaninca ne olur* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

`AccordionControl`, NavBar'in modern halefi olarak **grup + oge** hiyerarsisi tutar: kok gruplar `accordionControl1.Elements`'te, alt ogeler `AccordionControlElement.Elements`'tedir. Bir oge `ElementStyle.Item`, bir bolum `ElementStyle.Group` olur; tiklama `ElementClick` olayiyla tek noktadan yakalanir ve genelde `NavigationFrame` sayfasini degistirir. Dahili arama kutusu (`ShowFilterControl`) ve tek/cok acilir mod (`ExpandElementMode`) ile kullanici deneyimi ayarlanir; `ViewType` hamburger menu gorunumune gecis saglar.

---

## §1 Temel kullanim

```csharp
using DevExpress.XtraBars.Navigation;

AccordionControlElement group = new AccordionControlElement { Style = ElementStyle.Group, Text = "Oturumlar" };
group.Elements.Add(new AccordionControlElement { Style = ElementStyle.Item, Text = "Yeni Oturum" });
accordionControl1.Elements.Add(group);

accordionControl1.ElementClick += (s, e) => NavigateTo(e.Element);
```

```
AccordionControl
  +-- Elements (kok)
  |     +-- Element "Oturumlar"   (Style = Group)
  |     |     +-- Elements
  |     |           +-- "Yeni Oturum"   (Style = Item)  --> ElementClick
  |     |           +-- "Liste"         (Style = Item)
  |     +-- Element "Ayarlar"     (Style = Group)
  +-- ShowFilterControl (arama kutusu)
  +-- ExpandElementMode / AnimationType / ViewType
```

---

## §2 Ozellik tablosu

| Üye | Tur | Ne is yapar |
|---|---|---|
| `AccordionControl.Elements` | koleksiyon | Kok grup/ogeler |
| `AccordionControlElement.Elements` | koleksiyon | Alt seviye (sinirsiz derinlik) |
| `AccordionControlElement.Style` | `ElementStyle` | `Group` / `Item` / `Header` |
| `AccordionControlElementBase.Text` / `.Image` | `string` / `Image` | Baslik ve ikon |
| `ElementClick` | olay | Tiklama; arguman uyesi `e.Element` (`NEEDS_VALIDATION`) |
| `AllowItemSelection` | `bool` | Tiklanan ogeyi vurgular |
| `ExpandElementMode` | enum | Ayni anda tek mi/cok oge acik kalsin |
| `ShowGroupExpandButtons` / `ShowItemExpandButtons` | `bool` | Ac/kapa butonlarini gizler |
| `ShowFilterControl` | `bool` | Dahili arama kutusu |
| `ViewType` + `AccordionOptionsHamburgerMenu` | enum/nesne | Hamburger menu gorunumu |
| `ContentContainer` | ozellik | Oge altina acilan ozel icerik alani |
| `AnimationType` | enum | Acilir/kapanir animasyonu |

---

## §3 net48 ve net10.0-windows'da fark

| Konu | `net48` | `net10.0-windows` |
|---|---|---|
| Paket | `DevExpress.Win.Navigation` (DevExpress.XtraBars.v26.1) | Ayni paket |
| Oge kurulumu | Designer "Elements" sekmesi | Ayni |
| C# surumu | 7.3: object initializer serbest | 14: ayni yazim, `[..]` da mumkun |
| Nullable | `e.Element` null gelmez | `Nullable=enable` ise `e.Element?` dikkat |
| `#if` | Yalniz platform farkinda | Ayni; menu mantigi #if altina ALINMAZ |

---

## §4 Yaygin hatalar

| # | Hata | Sonuc | Duzeltme |
|---|---|---|---|
| 1 | Her ogeye ayri `Click` handler | God form (SOL-01) | Tek `ElementClick` |
| 2 | Navigasyonu oge metni ile `switch` | Metin degisince kirilir | Sabit `Tag`/kimlik kullan |
| 3 | Ogeyi kodda `Elements.Add` ile tekrar | Her acilimda kopyalanir | Designer'da bir kez ekle |
| 4 | `ShowFilterControl` acikken metni kodda gizlemek | Arama sonucu bos kalir | Filtre sonucunu da isle |
| 5 | Accordion'u `Dock=Fill` yapip govdeyi ustune almak | Govde gorunmez | Sol `Dock=Left`, govde `Fill` |

---

## §5 Kanit ve dogrulama

| Kalem | Durum |
|---|---|
| `Elements`, `Style`, `ElementClick`, `ShowFilterControl`, `ExpandElementMode`, `ViewType` | v26.1 WinForms dokumantasyonu ile **dogrulandi** |
| `ElementClick` arguman tipi | `NEEDS_VALIDATION` (IntelliSense ile adlandir) |
| `dotnet build` | Bu batch'te alinmadi → `NEEDS_VALIDATION` |
| Ilgili kural | DX-09 (navigasyon kabini), DX-13 (ince handler) |

**Kardesler:** [navigationframe-pages](navigationframe-pages.md) · [tile-control-startpage](tile-control-startpage.md) · [dockmanager-panels](dockmanager-panels.md) · [ribbon-form-basics](ribbon-form-basics.md)
