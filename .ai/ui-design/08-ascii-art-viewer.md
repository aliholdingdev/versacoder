---
title: "UI Wireframe 08 — ASCII/Unicode Art Viewer (Sanat İzleyici)"
type: spec
category: ui
status: active
version: 1.2.0
updated: 2026-10-01
authority: reference
scope: "08-ascii-art-viewer ekranının wireframe'i: Windows pencere kabuğu (başlık çubuğu, - o x düğmeleri, menü, durum çubuğu), galeri / tuval / özellikler bölgeleri, dışa aktar diyalogu, Unicode (UTF-8 + türkçe harf + dolgu) desteği ve GATE-003 onay alanı - onaysız tek satır .cs yazılmaz"
tags: [ui-design]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# 08 — ASCII/UNICODE ART VIEWER

**Zorunlu Bağlantılar:** [ui-design/index](readme.md) · [01-start-page](01-start-page.md) · [02-main-ide](02-main-ide.md) · [07-settings](07-settings.md) · [ui-rules](../.rules/ui-rules.md) · [devexpress-rules](../.rules/devexpress-rules.md) · [WORKFLOW](../governance/workflow.md)

> **Tek cümlelik sorumluluk:** ASCII **ve Unicode** sanat dosyalarını Windows penceresi
> kabuğu içinde gösteren izleyicinin pencere düğmelerini, bölgelerini ve tıklama akışını tanımlar.

---

## §0 Özet (Answer First)

`08-ascii-art-viewer` = **1 pencere, 3 sütun, 1 ribbon**: başlık `[-] [o] [x]` +
`RibbonControl` (Office2010), galeri (sol) / tuval (merkez) / özellikler (sağ) ve durum
çubuğu. Akış: dosya aç ->
tuvalde göster -> kodek/karakter seti -> dışa aktar.

**Karakter seti varsayılanı Unicode (UTF-8):** Türkçe `ü ö ç ş ğ ı` ve dolum `█ ░` tuvalde
doğrudan gösterilir; ASCII modu ikincildir.

**Durum: GATE-003 onayı alındı (2026-10-01, 7+1 ekran ONAYLI)** — kod izni
[checklist](../checklists/gate-003-ui-onay.md) 13/13 tamamlandığında açılır (G12).

| Bölge | Kontrol (teklif) | Yerleşim |
|---|---|---|
| Üst | Başlık çubuğu `[-] [o] [x]` + `RibbonControl` (Office2010) | Form üstü (ribbon) |
| Sol | `TreeList` (klasör + dosya ağacı) | Dock Left, ~%25 |
| Merkez | Tuval yüzeyi (kontrol: `PictureEdit` — **DOĞRULAMA GEREKLİ**) | Merkez, ~%50 |
| Sağ | Özellik paneli (boyut / kodek / karakter seti) | Dock Right, ~%25 |
| Alt | `StatusBarControl` (dosya, boyut, kodek, imleç) | Dock Bottom |

**Karakter seti (G4 uyumu):** kutu çizimi `|` `-` `+` (ASCII) · dolum `█` `░` (U+2588 /
U+2591 — *dolgu, kutu çizimi değildir*) · harf `ü ö ç ş ğ ı` · ok `->`. **Yasak kalan:**
`U+2500-U+257F` box-drawing — bkz. [wireframe-template](../.templates/ui-design/wireframe-template.md)
karakter seti tablosu.

**Künye:** `net48 + net10.0-windows` · DevExpress Universal **v26.1** · dosya adı sözleşmesi
`08-ascii-art-viewer` (README §1 01..07 aralığını aşar; satır GATE-003'te güncellenir).

---

## §1 Wireframe — Ana Pencere (Windows penceresi şeklinde)

```text
+----------------------------------------------------------------------+
| 08 ASCII Art Viewer - kediler.txt                        [-] [o] [x] |
+----------------------------------------------------------------------+
| Ribbon (Office2010): [Dosya] [Düzen] [Görünüm] [Kodekler] [Araçlar]  |
+----------------------------------------------------------------------+
| Sayfa "Dosya": [Aç] [Kaydet] [Dışa Aktar] | [-] %100 [+] | [Izgara]  |
+-------------------+--------------------------------+-----------------+
| GALERİ            | TUVAL                          | ÖZELLİKLER      |
| (TreeList)        |                                |                 |
|                   |                                | Dosya           |
| v Sanatlar        |  +------------------+          |   kediler.txt   |
|   kediler.txt  <  |  | Türkçe: üöçşığ  |          |                  |
|   logo.ans        |  |████░░██░░████░░|          | Boyut             |
|   banner.ans      |  |  >^_^< ~ ~ ~ ~ |          |   80 x 24         |
|   cornelia.nfo    |  +------------------+          |                 |
|                   |                                | Kodek           |
| v Dizinim         |    meow ~ ~ ~ (ü)              |   UTF-8         |
|   + import        |                                |                 |
|                   |                                | Karakter seti   |
|                   |                                |   Unicode       |
+-------------------+--------------------------------+-----------------+
| Hazır | 80 x 24 | UTF-8 | 2/12 dosya | Satır 1, Sütun 1              |
+----------------------------------------------------------------------+
```

**Pencere kabuğu (Windows) notları:**

| # | Öğe | Kural |
|---|---|---|
| 1 | Başlık çubuğu | `[küçült] [büyüt/kapat] [kapat]` = `[-] [o] [x]`; çift tık = büyüt/küçült |
| 2 | Kenarlık | Sürüklenebilir, yeniden boyutlanabilir; min. 900x600 |
| 3 | Ribbon | `RibbonControlStyle.Office2010` — sekme: Dosya / Düzen / Görünüm / Kodekler / Araçlar / Yardım (sürüm `v26.1` sabit; bkz. devexpress-rules) |
| 4 | Durum çubuğu | Dosya + `80 x 24` + kodek + dosya sayacı + imleç (`Satır`, `Sütun`) |
| 5 | DPI | %100 / %150 / %200'te piksel kaymaz (ui-rules) |
| 6 | Yazı tipi | Monospace; Unicode modunda `ü` 1 hücre kaplar (hizalama bozulmaz) |

---

## §2 Wireframe — Dışa Aktar Diyaloğu (modal pencere)

```text
+--------------------------------------------------------------+
| Dışa Aktar                                          [-] [x]  |
+--------------------------------------------------------------+
| Çıktı biçimi: [PNG v]    Boyut: [Otomatik v]                 |
|                                                              |
| Karakter seti:  ( ) ASCII   (o) Unicode                      |
| Kenar boşluğu:  [ 4 ] piksel                                 |
| Arka plan:      [ Beyaz v]  [ ] Şeffaf                       |
|                                                              |
|              [ İptal ]        [ Dışa Aktar ]                 |
+--------------------------------------------------------------+
```

---

## §3 Etkileşim Kuralları

| # | Tetik | Sonuç |
|---|---|---|
| 1 | Galeride **çift tık** | Dosya tuvale yüklenir; durum çubuğu güncellenir |
| 2 | Tuvale **sürükle-bırak** | Uzantılar: `.txt .ans .asc .nfo .diz` |
| 3 | `Ctrl+O` / `Ctrl+S` / `Ctrl+E` | Aç / Kaydet / Dışa Aktar (§2) |
| 4 | Araç çubuğu `[-] %100 [+]` | Yakınlaştırma; `%` elle yazılabilir |
| 5 | Kodekler ribbon sekmesi (grup: Kodlama) | `UTF-8` / `CP437` / `ANSI` -> tuval yeniden çizilir |
| 6 | Karakter seti (varsayılan **Unicode**) | `ü ö ç ş ğ ı` + `█ ░` gösterilir; ASCII'ye geçişte **kaynak dosya değişmez** |
| 7 | Desteklenmeyen uzantı | Uyarı: `Bu dosya tipi desteklenmiyor` — dosya açılmaz |

---

## §4 Beklenti Dışı Durumlar

| # | Durum | Davranış |
|---|---|---|
| 1 | Dosya boş | Tuvalde `Dosya boş` + durum `0 x 0` |
| 2 | Bozuk bayt (UTF-8 değil) | Kodek `CP437`'ye düşer, çubukta bilgi verilir |
| 3 | Dosya > 2 MB | Onay: `Büyük dosya (2.4 MB) açılsın mı?` |
| 4 | Dışa aktarım başarısız | `İptal` durur; **kaynak dosya değiştirilmez** |
| 5 | Pencere < 900x600 | Galeri + özellik paneli gizlenir, tuval korunur |

---

## §5 GATE-003 Durumu

| # | Madde | Durum |
|---|---|---|
| 1 | Wireframe tamam | **hazır** |
| 2 | Kullanıcı onayı | **alındı** (2026-10-01, §6) |
| 3 | `.cs` yazım izni | **checklist 13/13 bekliyor** — o ana kadar `src/**` altına tek satır yazılmaz (G12) |

---

## §6 ONAY ALANI (kullanıcı doldurur) — GATE-003

> Bu alanı **sen** dolduruyorsun: `Durum` hücresini değiştir, revizyon isteklerini yaz,
> imzalı onayı ver. Onaylanmadan bu ekranın kodu yazılmaz.

### §6.1 Onay tablosu

| Alan | Değer |
|---|---|
| Ekran | `08-ascii-art-viewer` |
| Durum | `ONAYLI` |
| Revizyon isteği (kısa) | yok |
| Tarih | 2026-10-01 |
| Onay metni | `"GATE-003 kapsamındaki 08-ascii-art-viewer wireframe'ini inceledim, onaylıyorum."` |
| Kullanıcı | kullanıcı (bu oturum, OpenCode) |

### §6.2 Revizyon isteklerim (satır satır)

1. _(boş — yazınca iş `ui-designer`'a döner, `developer`'a değil)_
2.
3.
4.
5.

### §6.3 Kural

| Kural | Sonuç |
|---|---|
| `Durum` `BEKLEMEDE` | kod yasak (G12) |
| `Durum` `RED` | yalnız bu ekran durur; 01..07 etkilenmez |
| `Durum` `REVİZE` | değişiklik uygulanır, tekrar `ONAYLI` gerekir |
| `Durum` `ONAYLI` + [checklist](../checklists/gate-003-ui-onay.md) 13 madde | GATE-003 açılır |
