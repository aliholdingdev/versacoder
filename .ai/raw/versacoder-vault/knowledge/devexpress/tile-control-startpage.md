---
title: "tile-control-startpage - VersaCoder"
type: knowledge
category: devexpress
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "DevExpress v26.1 TileControl ile baslangic ekrani: TileGroup/TileItem/TileItemElement yapisi, olcu, tiklama ile navigasyon, erisilebilirlik ve yerelleştirme"
---

# tile-control-startpage — TileControl ile Başlangıç Ekranı

**Zorunlu Baglantilar:** [vault index](../../index.md) · [knowledge index](../index.md) · [CLAUDE.md](../../CLAUDE.md) · [devexpress-rules](../../.rules/devexpress-rules.md) · [ADR-004](../../.decisions/accepted/ADR-004-devexpress-universal-v261-office2010.md) · [navigationframe-pages](navigationframe-pages.md) · [accordion-navigation](accordion-navigation.md) · [ribbon-form-basics](ribbon-form-basics.md) · [accessibility-keyboard-nav](accessibility-keyboard-nav.md) · [localizing-ui-text](localizing-ui-text.md)

> **Tek cümlelik sorumluluk:** *Başlangıç ekranındaki karo (tile) ızgarası `TileControl` ile nasıl kurulur, karoya tıklayınca hangi ekrana gidilir* sorusunun tek cevabıdır.

---

## §0 Ozet (Answer First)

Başlangıç ekranı, DevExpress **v26.1** `TileControl` (namespace `DevExpress.XtraEditors`) ile kurulur: `TileControl.Groups` → `TileGroup` → `TileItem` → `TileItemElement` (metin + ikon) hiyerarşisi. Karolar **kart değil navigasyon öğesidir**: `TileControl.ItemClick` olayında karonun taşıdığı sayfa adı okunur ve `navigationframe-pages` altındaki sayfaya geçilir — navigasyon **karo içine gömülmez** (code-behind 200+ satır = SOL-02 ihlali). Kontrol bir `Form`'un (`StartPageForm`) içinde yaşadığı için **WinForms üçlüsü** (`StartPageForm.cs` / `.Designer.cs` / `.resx`) geçerlidir; karo üretimi `.cs`'e değil tasarımcıya aittir.

| # | Parça | Tip | Ne taşır |
|---|---|---|---|
| 1 | `TileControl` | kontrol | ızgara + kaydırma + olaylar |
| 2 | `TileGroup` | karo grubu | başlık/organizasyon (ör. "Geliştirme", "Araçlar") |
| 3 | `TileItem` | tek karo | tıklanabilir öğe, boyut, işaret durumu |
| 4 | `TileItemElement` | karo içeriği | metin, ikon/görsel, hizalama |
| 5 | `Form` + `.Designer.cs` + `.resx` | üçlü | tüm tasarım `.Designer.cs`'te, metinler `.resx`'te |

---

## §1 Yapı ve tasarım zamanı

```
StartPageForm.cs              -> olay yonlendirici (ItemClick handler + kabuga cagri)
StartPageForm.Designer.cs     -> tileControl1, Groups, Items, Elements (tasarim)
StartPageForm.resx            -> karo metinleri + ikon (kaynak adlari bozulmaz)
        |
        v
  TileControl1
     +-- TileGroup "Geliştirme"
     |      +-- TileItem "Çözüm Gezgini"  -> TileItemElement (Text + Image)
     |      +-- TileItem "Kod Düzenleyici"
     +-- TileGroup "Araçlar"
            +-- TileItem "Ayarlar"
```

| Özellik | Nerede | Kural |
|---|---|---|
| `Groups` / `Items` / `Elements` | `.Designer.cs` | elle `.cs`'e `new TileItem()` **yazılmaz** |
| `TileItem.ItemSize` · `TileControl.ItemSize` | `.Designer.cs` | boyut ölçeği iki yerde tutarlı olmalı |
| `TileControl.ItemCheckMode` | `.Designer.cs` | `None` ise karo yalnız tıklanabilir öğedir (başlangıç ekranında önerilen) |
| Karo metni | `.resx` | sabit metin kodda **yasak** (`localizing-ui-text`) |
| İkon / görsel | `.resx` (binary) | tasarımcı properties penceresiyle; elle XML düzenleme yasak |

```csharp
// StartPageForm.cs — yalnizca olay yonlendirici (designer kodu buraya YAZILMAZ)
private void tileControl1_ItemClick(object sender, TileItemEventArgs e)
{
    string page = e.Item.Tag as string;          // karo -> sayfa adi (designer'da atanir)
    if (string.IsNullOrEmpty(page))
        return;

    _navigation.ShowPage(page);                  // kabuk servisi (navigationframe-pages)
}
```

> **KURAL:** Karo ile ekran arasındaki tek bağlantı `Tag` içindeki **sayfa adıdır**. Handler içinde `if (karo == "X") new XForm().Show()` **yazılmaz** — o karar `navigationframe-pages`'tedir.

---

## §2 Karar tablosu (TileControl mü, başka kontrol mü)

| İhtiyaç | Kontrol | Gerekçe |
|---|---|---|
| Başlangıç ekranı ızgarası (kısayollar) | **`TileControl`** | büyük dokunma/hedef alanı, ikon + metin |
| Dar alanda tek satır/kolon kısayollar | `TileBar` | alan tasarrufu |
| Kategori ağacı + liste | `AccordionControl` / `NavigationFrame` | hiyerarşik gezinme |
| Ribbon komutları | `RibbonControl` (**`RibbonControlStyle.Office2010`**) | komut modeli zorunlu (ADR-004) |
| Veri ızgarası | `GridControl` | satır/sütun verisi |

| Durum | Davranış |
|---|---|
| Karo sayısı az (< 6) ve sabit | `TileControl` + `TileGroup` tek grup |
| Karo sayısı kullanıcıya göre değişiyor | gruplar kodla doldurulur, metin `.resx`'ten okunur |
| Karo "seçilebilir" olacaksa | `ItemCheckMode` `None` **değil** — ama başlangıç ekranında seçim anlamsız, tıklama yeterli |
| Uzun işlem gerekiyorsa | karo handler `async void` olmaz; `async` metot + iptal (`cancellation-token-policy` kardeşleri) |

---

## §3 Erişilebilirlik ve yerelleştirme

| Kural | Uygulama | Kardeş |
|---|---|---|
| Karo adı okunabilir olmalı | `TileItemElement` metni kısa + anlamlı; karo yalnız ikonla anlatılmaz | [accessibility-keyboard-nav](accessibility-keyboard-nav.md) |
| `Tab` / klavye ile gezinme | başlangıç ekranı klavyeyle geçilebilmeli | [accessibility-keyboard-nav](accessibility-keyboard-nav.md) |
| Metinler kaynakta | karo metinleri `.resx`, kodda literal **yok** | [localizing-ui-text](localizing-ui-text.md) |
| Rozet/sayı gösterimi | kısa metin + `AccessibleName` benzeri açıklama | [accessibility-keyboard-nav](accessibility-keyboard-nav.md) |

---

## §4 net48 / net10.0-windows farki

| Konu | `net48` | `net10.0-windows` |
|---|---|---|
| `TileControl` sınıfı | DevExpress **v26.1 net48** derlemesi | DevExpress **v26.1 net10.0-windows** derlemesi |
| `ItemClick` olay imzası | **aynı** | aynı |
| DPI'da karo ölçüleri | `AutoScaleMode` ile ölçeklenir (`.Designer.cs`) | aynı — bkz. `autoscalemode-dpi-scaling` |
| C# | **7.3**: `as` + `string.IsNullOrEmpty` (target-typed `new` yok) | C# 14 |
| Sürüm/stil | v26.1 + `Office2010` | v26.1 + `Office2010` |

> **YASAK:** DevExpress v26.1 dışı sürüm (G3/G8) · `RibbonControlStyle.Office2010` dışı ribbon stili (ADR-004) · `.Form.cs`'e karo/kontrol üretimi yazmak (üçlü RED).

---

## §5 Kardesler

| Konu | Dosya |
|---|---|
| Sayfa navigasyonu | [navigationframe-pages](navigationframe-pages.md) |
| Yan panel gezinme | [accordion-navigation](accordion-navigation.md) |
| Kabuk / ribbon düzeni | [ribbon-form-basics](ribbon-form-basics.md) |
| Metin kaynakları | [localizing-ui-text](localizing-ui-text.md) |
| Klavye erişimi | [accessibility-keyboard-nav](accessibility-keyboard-nav.md) |

---

## §6 Yasaklar

1. `new TileItem()` / karo ekleme kodunu `StartPageForm.cs`'e yazmak (tasarım `.Designer.cs`'indir).
2. `.resx` elle XML düzenleme — `ResourceManager` adı bozulursa metinler kaybolur (G3/G4).
3. Handler içinde doğrudan form `new`'leyip göstermek (navigasyon `navigationframe-pages`'te).
4. EF Core / string-concat SQL — veri erişimi **ADO.NET + Dapper**, sorgu `@p1` parametreli.
5. Kanıtsız "ekran hazır" (G5) — kanıt: `dotnet build` (iki bacak) + ekran görüntüsü.
