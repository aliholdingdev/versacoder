---
title: "UI Wireframe 01 — Start Page (Karşılama Sayfası)"
type: spec
category: ui
status: draft
version: 1.2.0
updated: 2026-10-04
authority: SSOT
scope: "01-start-page ekranının wireframe'i: bölge haritası, ölçü tablosu, QAT + Uygulama Dugmesi + tab-page ribbon (Office2010), dosya üçlüsü, etkileşim kuralları, TFM notu — GATE-003 onayı bekliyor"
---

# 01 — START PAGE

**Zorunlu Bağlantılar:** [ui-design/index](README.md) · [VISION](../VISION.md) · [02-main-ide](02-main-ide.md) · [ui-rules](../.rules/ui-rules.md) · [devexpress-rules](../.rules/devexpress-rules.md) · [WORKFLOW](../WORKFLOW.md)

> **Tek cümlelik sorumluluk:** Uygulama açılışta gösterdiği ilk ekranın hangi bölgelerden oluştuğu
> ve tıklamaların nereye gittiği sorusunun tek cevabıdır.

---

## §0 Özet (Answer First)

`01-start-page`, `MainForm` içine gömülü **ilk bölge**dir: sol menü (accordion), karşılama
kutucukları (tile), son projeler tablosu ve durum çubuğu. Çift tık → `02-main-ide`.
**Durum: GATE-003 onayı alındı (2026-10-01)** (kod izni: checklist 13/13 sonrası).

| Bölge | Kontrol | Yerleşim |
|---|---|---|
| Üst | `RibbonControl` + `RibbonControlStyle.Office2010` (Uygulama Dugmesi + QAT + tab-page ribbon) | Form üstü (ribbon ≈96 px) |
| Sol | `AccordionControl` | DockPanel Left |
| Merkez üst | `TileControl` (karşılama kutucukları) | Merkez üst yarısı |
| Merkez alt | `GridControl` / `GridView` (son projeler) | Merkez alt yarısı |
| Alt | `StatusBarControl` | Dock Bottom |

**Künye:** Hedef bacak `net48 + net10.0-windows` · DevExpress Universal **v26.1** ·
bu ekran `01-start-page` dosya adıyla aynı adı taşır (ekran adı sözleşmesi §1).

---

## §1 Wireframe

**Olculendirme (yaklasik, `≈` zorunlu):** pencere `1280×800 px` · ribbon `≈96 px`
(baslik `≈30` + QAT `≈24` + sekme `≈42`) · accordion sol `≈240 px` · statusbar `≈24 px` ·
bant birimi `≈1 satir ≈ 24 px` · sadakat L2 (bilesen adayi, piksel birebir degil).
Kutu `|` `-` `+` ile cizildi; Unicode kutu cizim (U+2500-U+257F) ve emoji YOK (G4).

```text
+----------------------------------------------------------------------+ <- 0
| [VersaCoder]  [~] [v] [o]  HIZLI ERISIM ARACLARI        -  []  X      | <- baslik ~30 px
+----------------------------------------------------------------------+ <- 30
| (Uygulama Dugmesi: [VersaCoder])   [Kaydet][Geri][Yinele]  [Ara...]   |
+----------------------------------------------------------------------+ <- 54
| [Dosya]  [Gorunum]  [Araclar]  [Yardim]              (sekme ~42 px)   |
| +------------------------------------------------------------------+  |
| | GRUP:Proje  <>Yeni <>Ac <>Yukle | GRUP:Gorunum <>Sol <>Alt <>Temizle| | <- ~96
| +------------------------------------------------------------------+  |
+----------------------------------------------------------------------+ <- 96
|   |                                                                  |
|   |  +--------------------+  +--------------------+                   |
| AC|  | Yeni Proje         |  | Proje Ac           |  ~240 px          |
| CO|  | bos bir versiyon   |  | .sln / klasor sec  |  TILES ~240 px    |
| RD|  +--------------------+  +--------------------+                   |
|   |  +--------------------+  +--------------------+                   |
| ~ |  | CLI Konsolu        |  | Ayarlar            |                   |
| 2 |  | versacoder cli ac  |  | LLM / gorunum      |                   |
| 4 |  +--------------------+  +--------------------+                   |
| 0 |                                                                 |
| p |  SON PROJELER (GridControl ~260 px)                              |
| x |  +-----------------------------------+-----------+------+--------+
|   |  | Ad        | Yol          | Tarih  | Durum    |      |        |
|   |  +-----------+--------------+--------+----------+------+--------+
|   |  | VersaCode | C:\versacoder| 29.09  | Acik     |      |        |
|   |  | Demo      | D:\demo      | 28.09  | Kapali   |      |        |
|   |  | Son proje yok (bos durum satiri)                    |        |
+---+----------------------------------------------------------+-------+ <- 776
| HAZIR | v26.1 | net48/net10 | 3 son proje        StatusBar ~24 px   |
+----------------------------------------------------------------------+ <- 800
   ^                        ^
   [1] Accordion            [2] Grid satiri cift tik -> 02-main-ide
```

**Olcu tablosu (bolog bazli):**

| # | Bolge | Kontrol | Genislik x Yukseklik |
|---|---|---|---|
| 1 | Baslik + Uygulama Dugmesi | `RibbonForm` basligi + `[VersaCoder]` | `≈1280 x 30 px` |
| 2 | Uygulama Dugmesi + Hizli Erisim (QAT) | `[VersaCoder]` + `[~][v][o][Ara]` | `≈1280 x 24 px` |
| 3 | Ribbon sekme satiri | `[Dosya] [Gorunum] [Araclar] [Yardim]` | `≈1280 x 42 px` |
| 4 | Ribbon grup sayfasi | `RibbonPageGroup`: Yeni/Ac/Yukle ... | `≈1280 x 54 px` |
| 5 | Sol menu | `AccordionControl` (Dock Left) | `≈240 x 680 px` |
| 6 | Karsilama kutucuklari | `TileControl` (4 tile, 2x2) | `≈1040 x 240 px` |
| 7 | Son projeler | `GridControl` / `GridView` | `≈1040 x 260 px` |
| 8 | Durum cubugu | `StatusBarControl` (Dock Bottom) | `≈1280 x 24 px` |

**Ribbon notu (G3):** `RibbonControlStyle.Office2010` TEK stildir — wireframe'teki
tab-page ribbon, QAT ve Uygulama Dugmesi bu stilin gorunumudur; baska stil atanamaz.

**Odak sirasi (klavye, WCAG 2.2 2.4.3):** Uygulama Dugmesi -> QAT -> ribbon sekmeleri
-> Accordion -> tile'lar -> grid satirlari -> StatusBar; tab sirasi sol-merkez-alt
gorsel okuma sirasiyla ayni. Bilgi yalniz renkle anlatilmaz (durum kolonunda metin de var).

**Boş / yükleniyor / hata durumları:** grid `0 satır` ise "Son proje yok" satırı (boş ekran
**asla** boş beyaz alan değildir); servis okurken `ProgressBarControl` satır içinde;
hata metni StatusBar'da + neden (kod 401 vb.), dramatik diyalog yok.

---

## §2 Dosya Üçlüsü

| Dosya | İçerik | Sahip |
|---|---|---|
| `StartPage.cs` | Tile/Grid tıklama yönlendirmeleri, `IRecentProjectService` yüklemesi (yalnız handler + servis çağrısı) | `developer` |
| `StartPage.Designer.cs` | `TileControl`, `GridControl`, `AccordionControl`, `StatusBarControl` kurulumu + kolon tanımları | **`ui-designer`** |
| `StartPage.resx` | Başlık/metinler, durum ikonları, tile görselleri | **`ui-designer`** |

---

## §3 Etkileşim Kuralları

| # | Kural | Sonuç |
|---|---|---|
| 1 | Grid satırına **çift tık** | proje açılır → `02-main-ide` |
| 2 | "Yeni Proje" tile'ı | `03-solution-explorer` boş ağaçla açılır |
| 2b | "CLI Konsolu" tile'ı | `09-cli-console` tam ekran sekme olarak açılır (2026-10-02) |
| 3 | Son projeler her açılışta servisten okunur | UI'da dosya I/O **yok** (DA-01 ruhu: veri servisten) |
| 4 | Kapanan projeler gridde gri | "Durum" kolonu tek bakışta ayrım |
| 5 | `RibbonControlStyle` yalnız `Office2010` | başka stil atanmaz (G3 ihlali = RED) |

---

## §4 Hedef Bacak Notu

| Konu | Karar |
|---|---|
| Kontroller | DevExpress.Win 26.1 hem `net462` hem `net8.0` varlıklarında mevcut → `#if` gerekmez |
| DPI | `AutoScaleMode.Dpi` Designer'da; `StartPage.resx` ölçek anahtarları korunur |
| Veri | `IRecentProjectService` (l6/l4) — form SQL görmez (DA-01) |
| Erişilebilirlik | Tab sırası sol→merkez→alt; kontrast ≥4.5:1; durum yalnız renkle anlatılmaz |

---

## §5 GATE-003 Durumu

| # | Öğe | Durum |
|---|---|---|
| 1 | Wireframe yazıldı | evet (bu dosya) |
| 2 | Kullanıcı onayı | **bekliyor** |
| 3 | Kod izni (`StartPage.cs`) | kapalı |

---

## §6 Kanıt

| # | Kaynak | Ne kanıtlar |
|---|---|---|
| 1 | Kaynak wireframe `ui-design/01-start-page.md` (eski vault) | bölge/kontrol listesi |
| 2 | [devexpress-rules](../.rules/devexpress-rules.md) | sürüm + `Office2010` tek stil |
| 3 | [ui-rules](../.rules/ui-rules.md) | yerleşim/erişilebilirlik tabanı |
| 4 | [ADR-028](../.decisions/accepted/ADR-028-layered-baseline-9-projects.md) | l7 sorumluluğu |

> Ekran adı `01-start-page` ve `Son Gunc.` tarihleri örnektir — canlı veri servisten gelir (`DOĞRULAMA GEREKLİ`: servis arayüzü adı ilk derlemede doğrulanacak).
