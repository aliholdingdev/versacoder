---
title: "ui-design/component-inventory.md - 11 Wireframe Kontrol Envanteri (GATE-003)"
type: spec
category: ui
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "01-start-page..10-ai-chat-console wireframe'lerinde (11 dosya) gecen her kontrolun DevExpress Universal v26.1 karsiligi, DX-09 karsilastirmasi, DOĞRULAMA GEREKLİ listesi ve ad eslesme tablosu (gate-003-ui-onay soru 4/5/9 kaniti)"
---

# component-inventory.md - Kontrol Envanteri

**Zorunlu Baglantilar:** [ui-design/index](README.md) · [flows](flows.md) · [02-main-ide](02-main-ide.md) · [devexpress-rules](../.rules/devexpress-rules.md) · [ui-rules](../.rules/ui-rules.md) · [gate-003 checklist](../checklists/gate-003-ui-onay.md)

> **Tek cumlelik sorumluluk:** Sekiz wireframe'de cizilen her kontrolün DevExpress
> Universal **v26.1** karşılığı hangisi sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

**Tek surum: DevExpress Universal v26.1** (DX-01) - bu dosyada baska hicbir surum
referansi **yoktur** (checklist soru 4). Ana pencere kaydi: `02-main-ide` icin
**`MainForm : RibbonForm`** (DX-04) + **`RibbonControlStyle.Office2010`** (DX-03).

| Sutun | Anlam |
|---|---|
| Ekran | Wireframe adi (dosya adiyla ayni, 01..08) |
| Kontrol | Wireframe'de adi geçen bölge/kontrol ya da cizilen etkilesimli bilesen |
| DevExpress v26.1 karşılığı | v26.1 sinifi/kontrol adi |
| Wireframe dosyasi | Kaynak `.md` |

**Esleme politikasi (G9):** wireframe'de ad geciyorsa dogrudan kayit edilir.
Ad gecmiyorsa ama cizimden aciksa (düğme, acilir liste, radyo, onay kutusu, sayi
alanı) standart v26.1 karşılığı yazılır ve "cizim eslemesi" notu dusulur. Ad da
cizim de belirsizse hucre **`DOĞRULAMA GEREKLİ`** yazılır; tahmin uydurulmaz.

---

## §1 Envanter tablosu

| Ekran | Kontrol | DevExpress v26.1 karşılığı | Wireframe dosyasi |
|---|---|---|---|
| 01-start-page | Ribbon (üst: Dosya / Görünüm / Araçlar / Yardım) | `RibbonControl` + `RibbonControlStyle.Office2010` (DX-03) | 01-start-page.md |
| 01-start-page | Toolbar (Yeni Proje / Proje Aç / Son Projeler) | `BarItem` (`BarButtonItem`) - cizim eslemesi (DX-07) | 01-start-page.md |
| 01-start-page | Sol menu (accordion) | `AccordionControl` | 01-start-page.md |
| 01-start-page | Karsilama kutucuklari (tile) | `TileControl` (`TileItem`) | 01-start-page.md |
| 01-start-page | Son projeler tablosu | `GridControl` + `GridView` (DX-08) | 01-start-page.md |
| 01-start-page | Durum çubuğu (alt) | `StatusBarControl` | 01-start-page.md |
| 01-start-page | Yükleniyor satırı (grid içinde) | `ProgressBarControl` | 01-start-page.md |
| 01-start-page | Pencere denetimleri `[-] [o] [x]` | `RibbonForm` başlık çubuğu (DX-04; 01 MainForm içinde) | 01-start-page.md |
| 02-main-ide | **Ana pencere kabuğu (`MainForm`)** | **`MainForm : RibbonForm` (DX-04) + `RibbonControl` + `RibbonControlStyle.Office2010` (DX-03)** - kanit: DX-04, VISION §2.4, N41; `02-main-ide.md` satır 23 `MainForm`, satır 29/44/120 `Office2010` (literal `RibbonForm` wireframe'de gecmez - §4/1) | 02-main-ide.md |
| 02-main-ide | Ribbon sayfalari (Dosya / Düzen / Görünüm / Agent) | `RibbonPage` + `RibbonPageGroup` (DX-07) | 02-main-ide.md |
| 02-main-ide | Ribbon butonları (Yeni / Aç / Kaydet / Cik, Geri / Ileri) | `BarItem` (DX-07) | 02-main-ide.md |
| 02-main-ide | Sol / sağ dock (panel yerleşimi) | `DockLayoutManager` (wireframe + README §3) - DX-09 `DockManager` yaziyor → `DOĞRULAMA GEREKLİ` (§4/2) | 02-main-ide.md |
| 02-main-ide | Merkez üst: dosya sekmeleri | `XtraTabControl` | 02-main-ide.md |
| 02-main-ide | Merkez alt: Cikti / Terminal / Problemler | `XtraTabControl` | 02-main-ide.md |
| 02-main-ide | Sol dock: çözüm ağacı | `TreeList` + Find Panel (`SearchControl`) → 03 | 02-main-ide.md |
| 02-main-ide | Sağ dock üst: AI Chat | `RichEditControl` + giriş (`MemoEdit`) → 05 | 02-main-ide.md |
| 02-main-ide | Sağ dock alt: Agent Paneli | `GridControl` + görev ağacı (`TreeList`) → 06 | 02-main-ide.md |
| 02-main-ide | Alt: durum çubuğu | `StatusBarControl` | 02-main-ide.md |
| 02-main-ide | Modal: Ayarlar | `XtraTabControl` diyalogu (02 §0) - 07-settings `TreeList` + `GridControl` diyalogu → `DOĞRULAMA GEREKLİ` (§4/3) | 02-main-ide.md |
| 03-solution-explorer | Çözüm / dosya ağacı | `TreeList` (kendine referansli: `KeyFieldName` / `ParentFieldName` / `RootValue`) | 03-solution-explorer.md |
| 03-solution-explorer | Arama alanı (Ctrl+F) | `SearchControl` (§0) + `TreeList.OptionsFind` (§4) | 03-solution-explorer.md |
| 03-solution-explorer | Sağ tık menüsü | `ContextMenu` (§2 adi) - v26.1 sinifi `DOĞRULAMA GEREKLİ` (§4/4) | 03-solution-explorer.md |
| 03-solution-explorer | Ikon kaynaklari (C# / MD / SLN) | `ImageCollection` | 03-solution-explorer.md |
| 03-solution-explorer | Silme onay diyalogu | `MessageBoxForm` (§3/2) | 03-solution-explorer.md |
| 03-solution-explorer | Yükleniyor satırı | `ProgressBarControl` | 03-solution-explorer.md |
| 03-solution-explorer | Panel durum satırı (6 dosya / 1 değişiklik) | `LabelControl` - cizim eslemesi | 03-solution-explorer.md |
| 04-code-editor | Sekme şeridi (+ yeni sekme) | `XtraTabControl` | 04-code-editor.md |
| 04-code-editor | Kod alanı (satır no, secili satır vurgusu) | `RichEditControl` | 04-code-editor.md |
| 04-code-editor | Outline (sağ üst) | `TreeList` | 04-code-editor.md |
| 04-code-editor | Problemler (sağ alt) | `GridControl` | 04-code-editor.md |
| 04-code-editor | Alt bar: autosave / derleme sonucu | `ProgressBarControl` | 04-code-editor.md |
| 04-code-editor | [Ctrl+B derle] butonu | `SimpleButton` - cizim eslemesi | 04-code-editor.md |
| 04-code-editor | Kirli sekme kapanış onayı (kaydet / kapat / vazgeç) | onay diyalogu - `MessageBoxForm` kalibi (03 §3/2); 04 §3/2 ad vermiyor → `DOĞRULAMA GEREKLİ` (§4/5) | 04-code-editor.md |
| 05-ai-chat | Üst bar: oturum + model seçici | `LookUpEdit` x2 | 05-ai-chat.md |
| 05-ai-chat | Üst bar: temizle / sil | `SimpleButton` | 05-ai-chat.md |
| 05-ai-chat | Mesaj akışı (salt okunur, kod bloglari) | `RichEditControl` | 05-ai-chat.md |
| 05-ai-chat | Durum satırı (akış + token sayacı) | `ProgressBarControl` | 05-ai-chat.md |
| 05-ai-chat | Gir kutusu (Enter / Shift+Enter) | `MemoEdit` | 05-ai-chat.md |
| 05-ai-chat | Aksiyon butonları: Gönder / Baglam ekle / Durdur | `SimpleButton` x3 | 05-ai-chat.md |
| 05-ai-chat | Akış butonları: Dosyaya uygula / Kopyala | `SimpleButton` - cizim eslemesi | 05-ai-chat.md |
| 05-ai-chat | Hata durumu "retry" butonu | `SimpleButton` | 05-ai-chat.md |
| 06-agent-panel | Üst bar: Baslat / Durdur / Yenile | `SimpleButton` x3 | 06-agent-panel.md |
| 06-agent-panel | Üst bar durum etiketi (ÇALIŞIYOR) | `LabelControl` - cizim eslemesi | 06-agent-panel.md |
| 06-agent-panel | Aktif ajanlar tablosu (Ajan / Rol / Durum / Ilerleme) | `GridControl` + `GridView` (DX-08) | 06-agent-panel.md |
| 06-agent-panel | Görev ağacı (TASK-001..) | `TreeList` | 06-agent-panel.md |
| 06-agent-panel | Olay akışı (son 20 satır) | `MemoEdit` (salt okunur) | 06-agent-panel.md |
| 06-agent-panel | Alt: adim ilerleme | `ProgressBarControl` | 06-agent-panel.md |
| 06-agent-panel | "Çalışıyor" animasyonu | `MarqueeProgressBarControl` (§4) | 06-agent-panel.md |
| 07-settings | Kategori ağacı | `TreeList` | 07-settings.md |
| 07-settings | Ayar ızgarası (satır = ayar) | `GridControl` + `GridView` (DX-08) | 07-settings.md |
| 07-settings | Bolme / deger satirlari | `ToggleSwitch` (§4) | 07-settings.md |
| 07-settings | Seçim hücreleri (`[Start Page v]`, `[mimo-free v]`) | `LookUpEdit` / `ComboBoxEdit` - editör tipi sabitlenmemis → `DOĞRULAMA GEREKLİ` (§4/6) | 07-settings.md |
| 07-settings | Gizli alan (anahtar) maskesi + göster / gizle | gizli hucre editoru + göster/gizle eylemi (SEC-02); ikon adi verilmemiş → `DOĞRULAMA GEREKLİ` (§4/7) | 07-settings.md |
| 07-settings | Butonlar: Kaydet / Varsayınlara Dön / Vazgeç | `SimpleButton` x3 | 07-settings.md |
| 07-settings | Onay diyalogu (değişiklik varken kapanış / sifirlama) | `MessageBoxForm` kalibi (03 §3/2); 07 ad vermiyor → `DOĞRULAMA GEREKLİ` (§4/5) | 07-settings.md |
| 07-settings | Pencere denetimleri `[-] [o] [x]` | modal diyalog penceresi - taban sınıf `DOĞRULAMA GEREKLİ` (DX-04 yalniz ana pencere icin; §4/8) | 07-settings.md |
| 07-settings | Dogrulama isareti `!` + tooltip | grid hata gostergesi + tooltip - cizim eslemesi | 07-settings.md |
| 08-ascii-art-viewer | Pencere kabuğu: başlık `[-] [o] [x]`, kenarlık, min 900x600 | Windows pencere kabuğu; ribbon barindirdigi icin `RibbonForm` beklenir → `DOĞRULAMA GEREKLİ` (§4/9) | 08-ascii-art-viewer.md |
| 08-ascii-art-viewer | Ribbon (üst) | `RibbonControl` + `RibbonControlStyle.Office2010` (DX-03) | 08-ascii-art-viewer.md |
| 08-ascii-art-viewer | Ribbon sekmeleri: Dosya / Düzen / Görünüm / Kodekler / Araçlar / Yardım | `RibbonPage` + `RibbonPageGroup` (DX-07) | 08-ascii-art-viewer.md |
| 08-ascii-art-viewer | Sayfa "Dosya": Aç / Kaydet / Dışa Aktar | `BarItem` (DX-07) | 08-ascii-art-viewer.md |
| 08-ascii-art-viewer | Araç çubuğu `[-] %100 [+]` (Yakınlaştırma) | Yakınlaştırma denetimi - kontrol adi verilmemiş → `DOĞRULAMA GEREKLİ` (§4/10) | 08-ascii-art-viewer.md |
| 08-ascii-art-viewer | [Izgara] butonu | `BarItem` (degistirici) - cizim eslemesi | 08-ascii-art-viewer.md |
| 08-ascii-art-viewer | Galeri (sol): klasör + dosya ağacı | `TreeList` | 08-ascii-art-viewer.md |
| 08-ascii-art-viewer | Tuval (merkez) + surukle-birak | `PictureEdit` - wireframe kendisi `DOĞRULAMA GEREKLİ` diyor (§4/11) | 08-ascii-art-viewer.md |
| 08-ascii-art-viewer | Özellik paneli (sağ: Dosya / Boyut / Kodek / Karakter seti) | kontrol adi verilmemiş → `DOĞRULAMA GEREKLİ` (§4/12) | 08-ascii-art-viewer.md |
| 08-ascii-art-viewer | Durum çubuğu (dosya, boyut, kodek, imlec) | `StatusBarControl` | 08-ascii-art-viewer.md |
| 08-ascii-art-viewer | Dışa Aktar diyalogu: bicim / boyut / arka plan secicileri | `ComboBoxEdit` / `LookUpEdit` - cizim eslemesi | 08-ascii-art-viewer.md |
| 08-ascii-art-viewer | Dışa Aktar: karakter seti radyolari (ASCII / Unicode) | `RadioGroup` - cizim eslemesi | 08-ascii-art-viewer.md |
| 08-ascii-art-viewer | Dışa Aktar: kenar boslugu `[ 4 ]` | `SpinEdit` - cizim eslemesi | 08-ascii-art-viewer.md |
| 08-ascii-art-viewer | Dışa Aktar: `Seffaf` onay kutusu | `CheckEdit` - cizim eslemesi | 08-ascii-art-viewer.md |
| 08-ascii-art-viewer | Dışa Aktar: [Iptal] / [Dışa Aktar] | `SimpleButton` x2 - cizim eslemesi | 08-ascii-art-viewer.md |
| 09-cli-console | Üst sekme şeridi (CLI / Yeni oturum) | `XtraTabControl` - cizim eslemesi | 09-cli-console.md |
| 09-cli-console | ASCII logo (blok `versacoder`) | `LabelControl` + monospace font - cizim eslemesi | 09-cli-console.md |
| 09-cli-console | Prompt kutusu (metin + `Build`/`model`/`tools` satiri) | `LayoutControl` + `ButtonEdit` - cizim eslemesi | 09-cli-console.md |
| 09-cli-console | Alt ipuclari satiri (`shift+tab agents  ctrl+p commands`) | `LabelControl` | 09-cli-console.md |
| 09-cli-console | Alt durum satiri (`cd yol:branch` / `satir` / `exit`) | `StatusBarControl` | 09-cli-console.md |
| 09-cli-console | Boş durum: son komut özeti satırı | `MarqueeProgressBarControl` (calisiyor) + `LabelControl` | 09-cli-console.md |
| 10-ai-chat-console | Üst sekme şeridi (oturum sekmeleri + kapatma) | `XtraTabControl` - cizim eslemesi | 10-ai-chat-console.md |
| 10-ai-chat-console | Sohbet akışı (mesajlar, kod bloglari) | `RichEditControl` | 10-ai-chat-console.md |
| 10-ai-chat-console | Prompt kutusu (metin + model/agent satiri) | `LayoutControl` + `MemoEdit` - cizim eslemesi | 10-ai-chat-console.md |
| 10-ai-chat-console | Alt durum satiri (`cd yol:branch` / mesaj / model) | `StatusBarControl` | 10-ai-chat-console.md |

**Sayim:** 11 wireframe · 82 satir · §1'de 12 satır `DOĞRULAMA GEREKLİ` (tam liste §4,
15 madde) · surum ifadesi yalniz **v26.1**.

---

## §2 DX-09 ile karsilastirma (devexpress-rules §2)

DX-09 "kontrol seti ekranlarla eslesir" der; wireframe'lerle karsilastirma:

| Ekran | DX-09 aday listesi | Wireframe kontrolü | Sonuc |
|---|---|---|---|
| 01-start-page | `TileControl` | `TileControl` | eşleşme |
| 02-main-ide | `DockManager` + `StatusBarControl` | `DockLayoutManager` + `StatusBarControl` | ad farki → §4/2 |
| 03-solution-explorer | `TreeList` | `TreeList` | eşleşme |
| 04-code-editor | `RichEditControl` / `MemoEdit` + `XtraTabControl` | `RichEditControl` + `XtraTabControl` | eşleşme |
| 05-ai-chat | `ListBoxControl` + `MemoEdit` | `RichEditControl` (akış) + `MemoEdit` | **fark** → §4/13 |
| 06-agent-panel | `GridControl` + `ProgressPanel` | `GridControl` + `TreeList` + `MemoEdit` + `ProgressBarControl` | ana kontrol `GridControl` eslesir; ek kontroller wireframe'de → §4/14 |
| 07-settings | `DataLayoutControl` + `PropertyGridControl` | `TreeList` + `GridControl` (+ `ToggleSwitch`) | **fark** → §4/15 |
| 08-ascii-art-viewer | (DX-09'da yok - kapsam degisikligi) | `TreeList` + `PictureEdit` + `StatusBarControl` | DX-09 kapsami disi |

> DX-09 "alternatif istek → `ui-design/` + kullanici onayı" der; 05 ve 07
> wireframe'leri kullanici onayindan gecti (README §7.1, 2026-10-01). Iki kaynak
> arasindaki fark **gizlenmez**, `DOĞRULAMA GEREKLİ` olarak §4'te tasir.

---

## §3 Surum kaydi (DX-01)

| Madde | Deger |
|---|---|
| Tek surum | DevExpress Universal **v26.1** |
| Bu dosyadaki diger surum referansi | **0** (checklist soru 4 taramasi) |
| Ribbon stili | `RibbonControlStyle.Office2010` (DX-03) |
| Ana pencere tabani | `MainForm : RibbonForm` (DX-04) |

---

## §4 DOĞRULAMA GEREKLİ listesi (uydurma yok - G9)

| # | Nerede | Ne belirsiz | Neyle dogrulanacak |
|---|---|---|---|
| 1 | 02-main-ide | `02-main-ide.md` literal `RibbonForm` yazmıyor (yalniz `MainForm` + `Office2010`) | kayit DX-04 + VISION §2.4 + N41'de; wireframe'e eklenecekse kullanici onayı |
| 2 | 02-main-ide | `DockLayoutManager` (wireframe/README) vs `DockManager` (DX-09) | v26.1 sınıf adi ilk derlemede |
| 3 | 02-main-ide | Modal Ayarlar: 02 "XtraTabControl diyalogu", 07 `TreeList` + `GridControl` | iki wireframe arası uyum - kullanici karari |
| 4 | 03-solution-explorer | `ContextMenu` icin v26.1 sınıf adi | kodda (BarManager / PopupMenu) |
| 5 | 04-code-editor + 07-settings | Onay diyalogu sınıf adi (04 §3/2, 07 §3/3 ad vermiyor) | 03'teki `MessageBoxForm` kalibi gecerli mi |
| 6 | 07-settings | Seçim hücreleri icin editör tipi (`LookUpEdit` mu `ComboBoxEdit` mi) | Designer'da seçim |
| 7 | 07-settings | Gizli alan göster/gizle ikonunun kontrol adi | Designer'da seçim (SEC-02) |
| 8 | 07-settings | Modal pencere taban sinifi (DX-04 yalniz ana pencereyi baglar) | mimari karar |
| 9 | 08-ascii-art-viewer | Pencere form sinifi adi verilmemiş | ribbon barindirdigi icin `RibbonForm` mu |
| 10 | 08-ascii-art-viewer | Yakinlastirma `[-] %100 [+]` denetimi | Designer'da seçim |
| 11 | 08-ascii-art-viewer | Tuval kontrolu - wireframe'in kendisi yaziyor | `PictureEdit` dogrulamasi (08 §0) |
| 12 | 08-ascii-art-viewer | Özellik panelinin kontrol adi | Designer'da seçim |
| 13 | 05-ai-chat | DX-09 `ListBoxControl` derken wireframe `RichEditControl` kullaniyor | kullanici onayı zaten var (README §7.1); kural metni guncellenecek mi |
| 14 | 06-agent-panel | DX-09 `ProgressPanel` derken wireframe `MarqueeProgressBarControl` kullaniyor | ayni |
| 15 | 07-settings | DX-09 `DataLayoutControl` + `PropertyGridControl` derken wireframe `GridControl` kullaniyor | ayni |

---

## §5 Ad eşleşme tablosu (checklist soru 9 kaniti)

Yön: wireframe'de geçen kontrol adi -> `component-inventory.md`'de kayitli mi.

| # | Wireframe'de geçen ad | Kaynak wireframe | Envanterdeki ekran | Durum |
|---|---|---|---|---|
| 1 | `RibbonControl` | 01 §0 · 02 §0 · 08 §0 | 01 / 02 / 08 | var |
| 2 | `RibbonControlStyle.Office2010` | 01 §0 · 08 §1 | 01 / 02 / 08 | var |
| 3 | `AccordionControl` | 01 §0 | 01 | var |
| 4 | `TileControl` | 01 §0 | 01 | var |
| 5 | `GridControl` | 01 §0 · 02 §0 · 04 §0 · 06 §0 · 07 §0 | 01 / 02 / 04 / 06 / 07 | var |
| 6 | `GridView` | 01 §0 · 06 §0 | 01 / 06 | var |
| 7 | `StatusBarControl` | 01 §0 · 02 §0 · 08 §0 | 01 / 02 / 08 | var |
| 8 | `ProgressBarControl` | 01 §1 · 03 §1 · 04 §0 · 05 §0 · 06 §0 | 01 / 03 / 04 / 05 / 06 | var |
| 9 | `DockLayoutManager` | 02 §2 · 02 §3 | 02 | var |
| 10 | `XtraTabControl` | 02 §0 · 04 §0 | 02 / 04 | var |
| 11 | Find Panel | 02 §0 | 02 | var |
| 12 | `TreeList` | 02 · 03 · 04 · 06 · 07 · 08 | 02 / 03 / 04 / 06 / 07 / 08 | var |
| 13 | `SearchControl` | 03 §0 | 03 | var |
| 14 | `OptionsFind` | 03 §4 | 03 | var |
| 15 | `ContextMenu` | 03 §2 | 03 | var |
| 16 | `ImageCollection` | 03 §0 · 03 §2 | 03 | var |
| 17 | `MessageBoxForm` | 03 §3 | 03 (+ 04 / 07 notu §4/5) | var |
| 18 | `RichEditControl` | 02 §0 · 04 §0 · 05 §0 | 02 / 04 / 05 | var |
| 19 | `MemoEdit` | 05 §0 · 06 §0 | 05 / 06 | var |
| 20 | `LookUpEdit` | 05 §0 | 05 (+ 07 §4/6 notu) | var |
| 21 | `SimpleButton` | 05 §0 · 06 §0 | 05 / 06 (+ 01 · 04 · 07 · 08 cizim) | var |
| 22 | `ToggleSwitch` | 07 §4 | 07 | var |
| 23 | `MarqueeProgressBarControl` | 06 §0 · 06 §4 | 06 | var |
| 24 | `PictureEdit` | 08 §0 | 08 | var (kendisi `DOĞRULAMA GEREKLİ`) |
| 25 | `Toolbar` | 01 §1 | 01 | var |

**Sayim:** 25 ad · eslesmeyen **0** · `RibbonForm` wireframe'lerde gecmez, dis
kaynaklidir (DX-04 · VISION §2.4 · N41) - §4/1.

**09/10 notu (2026-10-02):** `09-cli-console` ve `10-ai-chat-console` §1'de kayitli,
§5 ad esleme tablosuna 2026-10-02'de eklenecek (bu iki wireframe yeni).

---

## §6 Kanit

| # | Kaynak | Ne kanitlar |
|---|---|---|
| 1 | [01..08 wireframe](README.md) bölge/kontrol tablolari | kontrol adlari |
| 2 | [devexpress-rules](../.rules/devexpress-rules.md) DX-01..DX-09 | surum + stil + DX-09 karşılaştırması |
| 3 | [architecture catalog](../architecture/index.md) N41 | `MainForm : RibbonForm` kaydi |
| 4 | [VISION](../VISION.md) §2.4 | `RibbonForm` + `Office2010` (02-main-ide) |
| 5 | [flows](flows.md) | ekran gecisleri (soru 8 esligi) |
