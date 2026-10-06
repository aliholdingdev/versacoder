---
title: "VersaCoder — DevExpress Ribbon Sayfası (XRibbonPage.cs) Şablonu"
type: template
category: templates/frontend
status: active
version: 1.0.0
updated: 2026-10-05
authority: SSOT
scope: "WinForms ribbon sayfasi: sayfa/grup/oge hiyerarsisi, SVG ikon, ItemClick baglari, enable matrisi ve kisayol tuslari"
---

# VersaCoder — DevExpress Ribbon Sayfası (XRibbonPage.cs) Şablonu

**Zorunlu Bağlantılar:** [index](../index.md) · [form-template](./form-template.md)

**Bölümler:** §1 Amaç · §2 Kapsam · §3 Mimari · §4 Kurallar · §5 Workflow · §6 Doğrulama · §7 Referanslar

---

## §1 Amaç

Bu şablon, VersaCoder masaüstü uygulamasındaki **her DevExpress ribbon sayfasının tek biçimde** üretilmesi için zorunludur: sayfa/grup/öğe hiyerarşisi, `ImageOptions` SVG ikon sözleşmesi, `ItemClick` bağlama biçimi, enable/disable durum matrisi, kısayol tuşları, erişilebilirlik, sekmeler arası akış ve MVVM/katman sınırı tek dosyada tanımlanır. `T-FE`'nin şerit ayağıdır; form ayağı `form-template`, kontrol ayağı `component-template`'tir.

| Alan | Değer |
|------|-------|
| Template Name | `ribbon-template.md` |
| Template Path | `.ai/.templates/frontend/ribbon-template.md` |
| Hedef Dosya | `src/VersaCoder.Desktop/**/XRibbonPage.cs` (+ `.Designer.cs`) |
| Hedef Kod | C# · `partial class` · WinForms · DevExpress WinForms **26.1** |
| Kalıtım | `XRibbonPage` (sayfa) · `XRibbonPageGroup` (grup) · `RibbonButtonItem` (öğe) |
| Kodlama | **UTF-8 (BOM'suz)** — Latin-1/CP1254 YASAK |
| Üretilen Uzunluk | 100–240 satır (kod) + bu şablonun 480–720 satırı |
| Dil | Türkçe (ç ğ ı İ ö ş ü doğru) — sayfa/grup/öğe adı İngilizce |
| Katman | `l7 UI + CLI` (Presentation sınırı) — `l0..l7` §3.9 |
| Versiyon | 1.0.0 |
| Authority | SSOT (bu dosya) |
| Bootstrap | 2026-10-05 |

### §1.1 Neden Ribbon Şablonu Ayrı Bir Kaynak?

Ribbon kodu en çabuk **dağılan** koddur: her geliştirici kendi gruplarını farklı sıraya koyar, ikonu bitmap gömer, olayı üç yerde bağlar, kısayolu sonradan unutur. Tek kaynak yoksa aynı ekranda menü iki sürümde iki farklı yerde yaşar.

| Boşluk | Belirti | Bu Şablonun Kestiği Şey |
|--------|---------|-------------------------|
| Grup/öğe sırası değişken | ekranlar arası tutarsız menü | §3.3 sabit 8 adım + §3.2 kimlik |
| Bitmap gömme | tema/dpi'de bulanık ikon | §3.5 SVG (`SvgImage`) tek yol |
| Üç yerde `ItemClick` | komut iki kez çalışıyor | §3.6 **tek abone** + §5.3/3 |
| Enable kuralı dağınık | `Enabled` her yerde | §3.7b tek `ApplyState` matrisi |
| Kısayol yok/çakışıyor | klavye yoksayılır | §3.8 kısayol tablosu |
| İş mantığı ribbon'a sızar | test edilemez | §3.9 MVVM/katman sınırı |

### §1.2 Disk Kanıtları ve Boşluk (YAGNI)

İddia → kanıt. Kanıt yoksa uydurulmaz, `⚠️ VERIFICATION REQUIRED` yazılır. Ölçüm: `C:\versacoder`, 2026-10-05.

| # | İddia | Durum | Kanıt |
|---|-------|-------|-------|
| 1 | `.ai/.templates/frontend/ribbon-template.md` (yazım öncesi) | ❌ YOK | `Test-Path = False` — bu dosya ile açılıyor |
| 2 | `frontend/` klasörü = 2 MD (ben + kardeşler) | ✅ DOĞRULANDI | `form-template.md` (590 satır · 32,6 KB) + `component-template.md` |
| 3 | Envanter satır 32: `PLANLI · 13,0 KB · developer` | ✅ DOĞRULANDI | `.ai/.templates/index.md` §3.4 Part C |
| 4 | `src/VersaCoder.Desktop/**/XRibbonPage.cs` (hedef kod) | ❌ YOK | `src/` = `Test-Path False` |
| 5 | `RibbonForm` taşıyan gerçek form | ❌ YOK | `src/` yok → §3.11 örnekleri **tasarım**dır |
| 6 | `.ai/ui-design/` + `GATE-003` tel kafes onayı | ❌ YOK → ⚠️ | G8: onaysız **ekran/kod** üretilmez |
| 7 | `.ai/.rules/` · `.ai/WORKFLOW.md` · `.ai/adr/` | ❌ YOK | envanter beyanı, diske işlenmemiş |
| 8 | `.svg` ikon kaynağı / `SvgImageCollection` | ❌ YOK | `Resources\` dizini diskte YOK → `⚠️` |
| 9 | DevExpress sürümü **26.1** (ADR-001) | ⚠️ VERIFICATION REQUIRED | `.ai/adr/` (gerçek ADR) dizini diskte YOK |
| 10 | Harici referans vault'ları (envanter G1) | 🔒 ERİŞİM YASAK | yalnız **yapı** referansı; tek kelime kopyalanmadı |

**Yorum yasağı:** Var olmayan dosya için "şurada tanımlı" denmez; `⚠️ VERIFICATION REQUIRED` yazılır, satır olduğu yerde durur. §1.2'deki ❌ satırları §3.11'deki örneklerin **tasarım** olduğunu, diskte henüz ribbon kodu bulunmadığını hatırlatır.

### §1.3 Ribbon Ne Değildir?

| Ribbon'dur | Ribbon Değildir |
|------------|-----------------|
| Kullanıcıya **komut sunan** sayfa/grup/öğe ağacı | Komutun kendisi (→ `l2` handler) |
| `e.Item.Name` → handler **yönlendirmesi** | İş kuralı kararı (→ `l2 Application`) |
| Durumun **gösterimi** (`Enabled`, `Caption`) | Durumun sahibi (→ form ViewState / use case) |
| Kısayol tuşunun **tanımı** | Kısayolun çözüm mantığı (→ form `ProcessCmdKey`) |
| SVG ikon **referansı** | İkon bitmap'i gömmek (§3.5 yasak) |

### §1.4 Ribbon Sayfası Yaşam Döngüsü

```text
+----------------------------------------------------------------+
| [1] new {{Sayfa}}Page()        alanlar, grup listesi boş       |
|    v                                                           |
| [2] InitializeComponent        8 adım (§3.3) · tek sefer       |
|    v                                                           |
| [3] RibbonControl'a ekle       Pages.Add(page) + kategori      |
|    v                                                           |
| [4] SVG yükle                  ImageOptions.SetSvgImage(...)   |
|    v                                                           |
| [5] ApplyState(durum)          Enabled matrisi (§3.7b)         |
|    v                                                           |
| [6] ItemClick → OnX            tek abone (§3.6) → l2 handler   |
|    v                                                           |
| [7] SelectedPageChanged        sekmeler arası akış (§3.8)      |
|    v                                                           |
| [8] Dispose                  abone -= · ikon koleksiyonu temiz |
|    ← hata: handler failure formu gösterir, sayfa KAPANMAZ      |
+----------------------------------------------------------------+
```

**Ne anlatır:** Sayfadan komut çıkışına ve yıkıma kadar sekiz durum; hata durumunda akış `[8]`'e değil `[6]`'ya geri döner (§3.7).

---

## §2 Kapsam

**İçinde:** sayfa/grup/öğe hiyerarşisi, SVG ikon, `ItemClick` bağlama, enable matrisi, kısayollar, erişilebilirlik, sekme akışı, katman sınırı, salt-okunur doğrulama.
**Dışında:** form iskeleti, özel kontrol kodu, iş kuralı, SQL/EF, ikon çizimi, paket sürümü kararı.

| Kapsam | Kapsam Dışı |
|--------|-------------|
| `XRibbonPage.cs` + `XRibbonPage.Designer.cs` ikilisi | Formun kendi iskeleti (→ `form-template`) |
| `XRibbonPageGroup` + `RibbonButtonItem`/`BarEditItem` | Özel kontrol (→ `component-template`) |
| `ItemClick` tek abone + dispatch tablosu | `l2` handler gövdesi (→ `backend/handler-template`) |
| `ImageOptions` / `SvgImage` referansı | `.svg` dosyası çizimi (→ `ui-design`) |
| `ApplyState` enable matrisi + kısayol tablosu | Kalıcı kural (→ `.ai/.rules/` — diskte YOK) |
| Sekmeler arası akış + erişilebilirlik | Sürüm kilidi kararı (→ ADR-001) |

### §2.1 Dosya Tipi → Sorumlu Eşlemesi

| Dosya Tipi | Sorumlu | Bu Şablondan mı |
|------------|---------|------------------|
| `src/VersaCoder.Desktop/**/XRibbonPage.cs` (şerit sayfası) | developer | ✅ **bu dosya** |
| `src/VersaCoder.Desktop/**/XForm.cs` (ana ekran formu) | developer | ❌ → `form-template` |
| `src/VersaCoder.Desktop/Controls/*.cs` (özel kontrol) | developer | ❌ → `component-template` |
| `src/VersaCoder.Desktop/**/XRibbonForm.cs` (taşıyıcı form) | developer | ❌ → `form-template` (RibbonForm kalıtımı) |
| `.ai/ui-design/01-…07-*.md` (ekran + menü tanımı) | ui-designer | ❌ ön koşul, GATE-003 |

### §2.2 Kapsam Sınırı Testi

Üretilecek `.cs` dosyasına sırayla sor; **ilk `EVET`** şablonu belirler:

1. **`Form` türevi mi** (`XtraForm`, `RibbonForm`)? → `form-template`.
2. **`XRibbonPage` / `XRibbonPageGroup` barındırıyor mu ve `Form` değil mi?** → **bu şablon**.
3. **`Control` türevi mi ama `Form`/`XRibbonPage` değil mi?** → `component-template`.
4. **Yalnız veri erişimi mi içeriyor** (SQL, Dapper)? → `backend/repository-template`.
5. **Yalnız iş kuralı mı içeriyor** (use case)? → `backend/service-template`.
6. **Hiçbiri değilse** → şablon yoktur; talep `index.md` §5.1 Adım 1'den açılır.

`HAYIR` soruları: CoreMusic/versacoder.ai içeriği mi (G1)? · `EntityFramework` geçiyor mu (G2)? · ribbon içinde `if` ile iş kuralı var mı (G5)? · `GATE-003` onaysız `src/**` kodu mu (G8)?

---

## §3 Mimari

Bu bölüm hem **şablonun kendi yapısını** hem **üretilecek sayfanın yapısını** tanımlar.

### §3.1 Dosya ve Sınıf Sözleşmesi

| Öğe | Kural | Doğru | Yanlış |
|-----|-------|-------|--------|
| Dosya ikilisi | `{{Sayfa}}Page.cs` + `{{Sayfa}}Page.Designer.cs` | ikisi de var | tek dosya |
| Sınıf bildirimi | `public partial class {{Sayfa}}Page : XRibbonPage` | `partial` zorunlu | `partial`'siz |
| `Designer.cs` sahipliği | yalnız Visual Studio/tasarım-süresi yazar | elle düzenleme yasak | elle `Edit & Continue` |
| Adlandırma | `{{Alan}}Page` — `-Page` eki zorunlu | `SettingsPage`, `RunPage` | `page1`, `RibbonTabX` |
| Grup adı | `grp{{Islem}}` öneki | `grpKaydet` | `groupControl1` |
| Öğe adı | `itm{{Islem}}` öneki | `itmKaydet` | `barButtonItem1` |
| Dosya yolu | `src/VersaCoder.Desktop/{{Alan}}/{{Sayfa}}Page.cs` | kökten yazılır | dağınık kök klasör |
| TFM | `net48;net10.0-windows` ikilisinde derlenir | `#if NET48` farkı bilinçli | tek TFM'e göre yazım |

### §3.2 Sayfa Kimliği ve Amacı

Her sayfa için şu alanlar **tek satırda** doldurulur; boş hücre kabul edilmez:

| Alan | Açıklama | Zorunlu Örnek Kalıp |
|------|----------|----------------------|
| `SayfaAdı` | sınıf adı | `{{EkranAdi}}Page` |
| `Dosya` | kökten yol | `src/VersaCoder.Desktop/{{Alan}}/{{EkranAdi}}Page.cs` |
| `Amaç` | tek cümle | `Kullanıcı {{AMAC}} komutlarını {{SONUC}} ile bitirir.` |
| `Kategori` | `RibbonPageCategory` | `{{Kategori}}` (`Ana` \| `Bağlamsal`) |
| `Gruplar` | soldan sağa sıra | `grp{{1}} · grp{{2}} · grp{{3}}` (en fazla 4) |
| `Bağlı Use Case` | `l2` handler | `{{UseCaseAdi}}Handler` |
| `Birincil Öğe` | kısayol alacak olan | `itm{{Kaydet}}` + `Ctrl+{{S}}` |
| `Onay Kapısı` | üretilmeden önce | `GATE-003` tel kafesi ile birebir eşleşme |

### §3.3 Kurulumun 8 Sabit Adımı

Sıra **değiştirilemez**; değiştirilen sayfa tasarım-süresinde kayar ve denetim komutu (§5.3/2) sırayı doğrulayamaz.

| # | Adım | Ne Yapılır | İhlal Belirtisi |
|---|------|-----------|-----------------|
| 1 | `SuspendLayout()` | ölçüm olayları bastırılır | arkada titreşim |
| 2 | `((ISupportInitialize)(...)).BeginInit()` | `RibbonControl`/`Page` için | tasarım-süresi çökme |
| 3 | Sayfayı oluştur | `new {{Sayfa}}Page()` + `Name`/`Text` | sayfa görünmez |
| 4 | Grupları **soldan sağa** ekle | `page.Groups.Add(grp)` | menü sırası bozuk |
| 5 | Öğeleri gruba ekle + `ImageOptions` | `grp.ItemLinks.Add(itm)` | ikonsuz öğe |
| 6 | Kısayol/`Description`/`Hint` ata | `Shortcut`, `Description` | erişilebilirlik boş |
| 7 | Olaylara **tek seferde** abone | `ribbonControl.ItemClick += ...` | çift tetikleme |
| 8 | `ResumeLayout(false)` + `PerformLayout()` | ölçüm yeniden açılır | DPI taşması |

**Yasak:** 7. adımda `+=` edilen olay için yerinde `-=` türü temizlik; abonelik **taşıyıcı formun** `Dispose`'una aittir (§4.5 G6). Aynı dosyada `ItemClick +=` **birden fazla kez yazılamaz** (§5.3/3).

### §3.4 Hiyerarşi (ASCII kutu — 66 sütun)

```text
+----------------------------------------------------------------+
| RibbonControl (taşıyıcı form · form-template)                  |
| +-- RibbonPageCategory {{Kategori}}                            |
| |   +-- {{Sayfa}}Page : XRibbonPage        Name={{Sayfa}}      |
| |       +-- grp{{1}} : XRibbonPageGroup    Text={{Grup1}}      |
| |       |   +-- itm{{1}}  RibbonButtonItem Caption=Yeni        |
| |       |   +-- itm{{2}}  RibbonButtonItem Caption=Sil · Ctrl+D|
| |       +-- grp{{2}} : XRibbonPageGroup    Text={{Grup2}}      |
| |           +-- itm{{3}}  RibbonButtonItem Caption=Kaydet      |
| |           +-- itmAyar  RibbonBarSubItem  Alt ogeler          |
+----------------------------------------------------------------+
```

**Ne anlatır:** İzin verilen hiyerarşi — kategori > sayfa > grup > öğe; her satır `{{…}}` yerine gerçek adla doldurulur.
**Doğrulama:** §5.3/6 — kutu satırları tam 66 karakter olmalı.

### §3.5 `ImageOptions` / SVG İkon Sözleşmesi

Ribbon'da ikon **yalnız SVG** ile gelir; bitmap gömmek (`.png`, `Resources.**`) RED'tir (§4.3).

| # | Kural | Doğru | Yanlış |
|---|-------|-------|--------|
| 1 | Kaynak | `.svg` dosyası → `SvgImageCollection` | `Image.FromStream(...)` |
| 2 | Atama | `ImageOptions.SetSvgImage(itm, "{{ikon}}")` | `itm.Glyph = Properties.x` |
| 3 | Boyut | `ImageOptions.Width/Height` sabit (16/24) | her öğede farklı boyut |
| 4 | Ad | `{{konu}}-{{eylem}}` (koşumlu) | `icon1`, `new-svg-3` |
| 5 | Yer | `Resources/svg/*.svg` tek klasör | dağınık `.svg` |
| 6 | Eşdeğerlik | ikon + **metin** birlikte (`Caption`) | yalnız ikon (erişilebilirlik) |
| 7 | Tema | SVG ikonları tema ile renklendirilir | sabit renkli bitmap |
| 8 | Bulunamazsa | `⚠️` + tasarım-süresi hata | sessiz boş ikon (G11) |

### §3.6 `ItemClick` Bağlama Kuralları

Ribbon olay **yönlendirir**, iş **yapmaz**. Bağlama biçimi **tek kalıptır** ve grep ile sayılabilir (§3.10).

| # | Kural | Doğru | Yanlış |
|---|-------|-------|--------|
| 1 | Tek abone | `ribbonControl.ItemClick += OnRibbonItemClick;` | her `Page_Load`'ta `+=` |
| 2 | Yönlendirme | `switch (e.Item.Name)` → `case "itmKaydet"` | her öğeye ayrı lambda |
| 3 | Bilinmeyen öğe | `default:` → günlüğe kod, **yutma yok** | `default: break;` sessiz |
| 4 | Lambda yasak | adlı metot | `+= (s,e) => ...` (grep kırılır) |
| 5 | `BarEditItem` | `EditValueChanged` **tek** abone | her değer değişiminde bağ |
| 6 | İş mantığı | yalnız `await _handler.HandleAsync(...)` | ribbon içinde `if` kuralı |
| 7 | Çözme | taşıyıcı form `Dispose`'ında `-=` | GC'ye bırakmak |
| 8 | Hata | `failure` → `lblDurum`/XtraMessageBox | sayfa **kapanmaz** (G11) |

**Bağlantı zinciri:**

```text
+----------------------------------------------------------------+
| Kullanıcı   Ribbon tıklaması / Ctrl+{{S}}                      |
|    v                                                           |
| RibbonControl  ItemClick → OnRibbonItemClick (tek abone)       |
|    v                                                           |
| Dispatch      switch(e.Item.Name) → "itm{{Kaydet}}"            |
|    v                                                           |
| Taşıyıcı Form  bs{{Veri}}.EndEdit() + ValidateChildren()       |
|    v                                                           |
| l2 Handler   I{{UseCase}}Handler.HandleAsync(komut)            |
|    v                                                           |
| Sonuç        ApplyState(...) · lblDurum = OK                   |
|    ← hata: failure → lblDurum + kod · sayfa açık kalır         |
+----------------------------------------------------------------+
```

### §3.7 Olay Akışı ve Hata Durumları

| # | Olay | Tetikleyen | Handler / Akış | Başarı | Hata Durumu |
|---|------|-----------|----------------|--------|-------------|
| 1 | `RibbonForm_Load` | ilk açılış | sayfaları ekle + `ApplyState` | menü hazır | `⚠️` + sayfa boş kalır |
| 2 | `ItemClick` | kullanıcı | §3.6 dispatch → `l2` handler | durum güncellenir | `lblDurum` + kod, **sayfa kapanmaz** |
| 3 | `Ctrl+{{S}}` | klavye | aynı dispatch (kısayol = aynı öğe) | §3.7 ile aynı | §3.7/2 ile aynı |
| 4 | `EditValueChanged` | `BarEditItem` | girdi → form ViewState | filtre uygulanır | eski değere dön + uyarı |
| 5 | `SelectedPageChanged` | sekme değişimi | §3.8 akışı | bağlam güncellenir | önceki sekme korunur |
| 6 | handler `failure` | Application | hata kodu → metin | — | ekran **kapanmaz** (G11) |
| 7 | `Enabled` değişimi | `ApplyState` | §3.7b matrisi | odak düşer | olay **tetiklenmez** |
| 8 | `Dispose(true)` | form kapanışı | `-=` + SVG koleksiyonu | temiz | `ObjectDisposedException` riski |

**Kural:** 6. satır **her zaman** yazılır; başarı yolu asla tek yol olarak kodlanmaz.

#### §3.7b Enable/Disable Durum Matrisi

`Enabled` atamaları **tek metotta** yaşar: `ApplyState(RibbonState state)`. Dağınık `Enabled =` atamaları RED'tir (§5.3/5).

| Durum | `itmYeni` | `itm{{Kaydet}}` | `itmSil` | `itm{{Yenile}}` | `itmAyar` |
|-------|-----------|------------------|----------|------------------|-----------|
| `Normal` | açık | açık | açık | açık | açık |
| `Yukleme` | kapalı | kapalı | kapalı | **dönen** | kapalı |
| `Kirli` (değişiklik var) | kapalı | **açık** | kapalı | kapalı | açık |
| `Hata` | açık | kapalı | kapalı | açık | açık |
| `SaltOkunur` | kapalı | kapalı | kapalı | açık | kapalı |

**Kural:** Her hücre **açık/kapalı** olarak kodda yazılır; "önceki durumu koru" davranışı **yoktur** (tahmin = hata). Kapalı öğe için `Caption` **metinle** de durumu anlatır (`Pasif`, `Bekleniyor`), yalnız renk/griye güvenilmez.

### §3.8 Kısayol Tuşları, Erişilebilirlik ve Sekmeler Arası Akış

| Alan | Kural | Denetim |
|------|-------|---------|
| Kısayol | birincil öğelere `Shortcut` (Ctrl+S, Ctrl+N, F5) | §5.3/4 |
| Çakışma | aynı `Keys` kombinasyonu iki öğede **olamaz** | §5.3/4 |
| Dağıtım | kısayol çözümü `ProcessCmdKey`'de **aynı dispatch**'e gider | elle denetim |
| KeyTip | `Alt` ile erişim; `Description` kısa metin | gözlem |
| Erişilebilir ad | `AccessibleName` + `AccessibleDescription` (Türkçe) | gözlem |
| Metin + ikon | `Caption` zorunlu — yalnız ikonla komut verilmez | §3.5/6 |
| Renk-körü | durum **yazıyla** da (`Pasif`) | §3.7b |
| Sekme akışı | `SelectedPageChanged` → bağlam/kategori güncellenir; **grid filtresi sayfa değişiminde kaybolmaz** | §6.5 K3 |
| Bağlamsal kategori | `RibbonPageCategory` görünür/gizli tek noktadan | gözlem |
| DPI 100–200% | ribbon boyutu `AutoScaleMode.Dpi`'ye bırakılır; sabit `Width` yasak | §5.3/6 |
| DevExpress 26.1 | sürüm **yükseltilmez** (ADR-001) | §6.5 K4 |

**Kısayol tablosu (üretilen sayfada doldurulur):**

| Öğe | Kısayol | Dispatch Etiketi | Karşıt Öğe |
|-----|---------|------------------|-------------|
| `itm{{Yeni}}` | `Ctrl+{{N}}` | `case "itm{{Yeni}}"` | — |
| `itm{{Kaydet}}` | `Ctrl+{{S}}` | `case "itm{{Kaydet}}"` | `itm{{Iptal}}` |
| `itm{{Sil}}` | `Ctrl+{{Shift}}+{{D}}` | `case "itm{{Sil}}"` | onay penceresi |
| `itm{{Yenile}}` | `F5` | `case "itm{{Yenile}}"` | `Yukleme` durumu |

### §3.9 Katman Sınırı (l0..l7) ve MVVM Yorumu

| Katman | Ad | Ribbon Ne Alır | Ribbon Ne Vermez |
|--------|----|----------------|------------------|
| `l0` | Abstractions | — | — |
| `l1` | Domain | invariant **bilgisi** (salt okunur `{{Tip}}`) | `XRibbonPage`/`Control` tipi |
| `l2` | Application | komut/use case **sonucu** (DTO) + handler | ribbon öğe referansı |
| `l3` | Crosscutting | sonuç kodu → metin (`{{HataKodu}}`) | menü sırası |
| `l4` | Infrastructure | **asla** (G2: EF/SQL yasak) | — |
| `l5` | Protocol | — | — |
| `l6` | Host | — | — |
| `l7` | **UI + CLI** | ribbon burada yaşar | iş kuralı, sorgu |

**Sınır (MVVM yorumu):** DevExpress Ribbon **yalnız `l7` Presentation sınırında** görünür; `l1`–`l6` hiçbirinde `DevExpress.*` `using`'i geçmez (§5.3/5). Ribbon, `l2`'ye **olay/komut** gönderir; `l2` sayfayı **hiç görmez**. `BindingSource`/`DataSource` yalnız **görüntüleme** içindir (`BarEditItem`, liste dolgusu); **komut** bağlaması (command binding) ribbon'da **yoktur** — komut yolu her zaman `ItemClick → form → handler`'dır.

### §3.10 Grep ile Denetlenebilir Kalıplar

Denetim, **sözcük/sayıma** dayanır; yorum gerekmez.

| # | Kalıp | Beklenen | Aşırılık Belirtisi |
|---|-------|----------|---------------------|
| P1 | `partial\s+class\s+\w+Page\s*:\s*XRibbonPage` | her dosyada ≥ 1 | kalıtım/partial yok |
| P2 | `ItemClick\s*\+=` | sayfa dosyasında **= 1** | 0 → bağlantı yok · >1 → çift tetik |
| P3 | `=>\s*\(?\s*\w+\s*,\s*\w+\s*\)\s*=>` (lambda olay) | 0 | lambda abonelik |
| P4 | `Image\.FromStream\|Properties\.\w+\.Bitmap` | 0 | bitmap gömme (§3.5) |
| P5 | `SvgImage\|SetSvgImage` | ≥ öğe sayısı | SVG yok |
| P6 | `Shortcut\s*=` | ≥ birincil öğe sayısı | kısayolsuz komut |
| P7 | `Enabled\s*=` (sayfa dosyasında) | yalnız `ApplyState` gövdesinde | dağınık enable |
| P8 | `EntityFramework\|DataAnnotations\|SELECT\s+\*` | 0 | katman/G2/G7 ihlali |

**Test edilebilirlik:** dispatch **saf** ve `internal` olur (`internal static bool TryResolve(string itemName, out Komut komut)`); `l2` handler ctor DI ile gelir; ribbon `EventArgs` dışında bilgi **taşımaz** → sayfa testi, dispatch tablosu üzerinden yapılır.

### §3.11 Dolu VersaCoder Örneği ve Tam Kalıp

**Dolu örnek (gerçek VersaCoder verisiyle — `SettingsPage`):**

```text
+----------------------------------------------------------------+
| RibbonControl (SettingsForm : RibbonForm)                      |
| +-- RibbonPageCategory Ana                                     |
| |   +-- SettingsPage : XRibbonPage       Name=SettingsPage     |
| |       +-- grpBaglanti : XRibbonPageGroup  Text=Baglanti      |
| |       |   +-- itmYol     BarEditItem    ButtonEdit · browse  |
| |       |   +-- itmYenile  RibbonButtonItem F5 · SVG yenile    |
| |       +-- grpKayit : XRibbonPageGroup     Text=Kayit         |
| |           +-- itmKaydet  RibbonButtonItem Ctrl+S · SVG kaydet|
| |           +-- itmKapat   RibbonButtonItem Ctrl+K             |
+----------------------------------------------------------------+
```

**Tam kalıp (yalnız bu blokta `{{…}}` yaşar; üretilen kodda adedi 0 olmalıdır):**

````markdown
```csharp
public partial class {{EkranAdi}}Page : XRibbonPage
{
    private readonly I{{UseCase}}Handler _handler;

    public {{EkranAdi}}Page(I{{UseCase}}Handler handler)
    {
        _handler = handler;                 // ctor DI (§3.10)
        InitializeComponent();              // §3.3 — 8 adım, tek sefer
        ApplyState(RibbonState.Yukleme);    // §3.7b — tek enable noktası
    }

    private void OnRibbonItemClick(object sender, ItemClickEventArgs e)
    {
        switch (e.Item.Name)                // §3.6 — tek dispatch
        {
            case "itm{{Kaydet}}":
                var r = await _handler.HandleAsync(new {{Komut}}(...));
                if (r.IsFailure) { lblDurum.Text = r.Error; return; }  // §3.7/6
                ApplyState(RibbonState.Normal);
                break;
            default:
                Debug.WriteLine("Bilinmeyen öğe → " + e.Item.Name);   // G11
                break;
        }
    }

    private void ApplyState(RibbonState state)   // §3.7b — tek matris
    {
        itm{{Kaydet}}.Enabled = state == RibbonState.Kirli;
        itm{{Sil}}.Enabled    = state == RibbonState.Normal;
        itm{{Yeni}}.Enabled   = state != RibbonState.SaltOkunur;
    }
}
```
````

---

## §4 Kurallar

### §4.1 Yazım Sırası ve Öncelik

| Öncelik | Şart | Yapılacak |
|---------|------|-----------|
| 1 | GATE-003 tel kafesi onayı yoksa | **DUR** — gerçek ekran kodu üretilmez (§4.5 G8) |
| 2 | Menü/ekran tanımı varsa | §3.2 kimlik satırını doldur |
| 3 | Katman sınırı | §3.9 — ribbon yalnız `l7`, komut `l2`'ye |
| 4 | Hiyerarşi | §3.4 kalıbını çiz, adlandır (≤ 4 grup) |
| 5 | Kurulum | §3.3 8 adım, değiştirilemez sıra |
| 6 | İkon | §3.5 — SVG tek yol, bitmap yasak |
| 7 | Bağlama + durum | §3.6 tek abone · §3.7b tek matris |
| 8 | Kısayol + erişilebilirlik | §3.8 tabloları dolu |
| 9 | Doğrulama | §5.3 komutları → 0 satır |

### §4.2 Sorumluluk ve Onay

| Aşama | Rol | Ne Onaylar | Çıktı | Süre |
|-------|-----|-----------|-------|------|
| 1 | ui-designer | tel kafes ↔ §3.2 menü sırası | GATE-003 onayı | 10 dk |
| 2 | developer | §3.1–§3.11 kodu | `{{Sayfa}}Page.cs` + `Designer.cs` | 30 dk |
| 3 | reviewer | §6.1 (21 madde) | ✅ / RED | 10 dk |
| 4 | architect | katman: `l7` → `l2` komut bağı, `DevExpress` yalnız `l7` | §3.9 çizimi | 8 dk |
| 5 | master-orchestrator | §5.3 çıktısı + §5.4 | Quality Report | 5 dk |

> **3 RED** sonrası iş durur; gerekçe `master-orchestrator`'a yükseltilir. Red → düzelt → aşama 2'ye dön.

### §4.3 Ölçek / Standartlar

| Öğe | Değer | Sapma |
|-----|-------|-------|
| Dosya satırı (kod) | 100–240 | < 70 iskelet · > 400 böl |
| Grup sayısı / sayfa | ≤ 4 | > 5 → yeni sayfa |
| Öğe sayısı / grup | ≤ 6 | > 8 → alt öğe (`RibbonBarSubItem`) |
| Sayfa başına kısayol | ≥ 1 birincil | 0 = RED |
| Bitmap ikon | 0 | `Image.FromStream` = RED |
| Kodlama | UTF-8 BOM'suz | Latin-1/CP1254 otomatik RED |
| DevExpress sürümü | 26.1 | başka iddia = RED (ADR-001) |

### §4.4 Değişiklik Kontrolü

| Olay | Aksiyon | Kayıt |
|------|---------|-------|
| Sadece metin/tooltip düzeltmesi | kod içinde | `version` patch |
| Öğe/grup eklendi-çıkarıldı | §3.4 ağacı + §3.7b matrisi yeniden | `updated` + `log.md` satırı |
| Kısayol değişti | §3.8 tablosu + çakışma taraması | `version` minor |
| Olay akışı değişti (§3.7) | tablo + §6.1 yeniden | `version` minor + inceleme |
| GATE-003 tel kafesi değişti | sayfa buna uydurulur, tersi **asla** | gate kaydı |

### §4.5 Hard Guardrails (Otomatik DUR)

| # | Guardrail | İhlal Sonucu |
|---|-----------|--------------|
| G1 | CoreMusic / versacoder.ai içeriğinden cümle | **DUR** — sil + özgün yaz (envanter G1) |
| G2 | Sayfada `EntityFramework`, `DbConnection`, SQL metni | **DUR** — erişim `l4 Infrastructure`'a taşınır |
| G3 | `partial class` yok / `InitializeComponent` çağırmıyor | **DUR** — §5.3/1 |
| G4 | `SuspendLayout`/`BeginInit`–`ResumeLayout` sırası bozuk | **DUR** — §5.3/2 |
| G5 | Sayfada `SELECT *` / SQL **veya** ribbon içinde iş kuralı `if`'i | **DUR** — `l2` handler'a taşınır |
| G6 | `ItemClick +=` ikinci kez / her `Load`'ta abonelik | **DUR** — çift tetiklenme, §5.3/3 |
| G7 | DataAnnotations attribute (`[Required]`, `[Range]`) | **DUR** — saf kontrole çevrilir (`form-template` §3.6) |
| G8 | `GATE-003` onaysız **ekran/kod** üretilmez (kök `CLAUDE.md` G8 = SSOT) — kapsam: gerçek ekran dosyası ve `src/**` altına yazılan `.cs` satırı; **bu şablon (MD) üretilmesi G8'e girmez**, giren şey onaysız ekran/koddur. `.ai/.templates/index.md` §3.4 kapıyı `form`/`ribbon` şablon üretimine de yayar (daha dar yorum) — çelişkide kök `CLAUDE.md` geçerlidir | **DUR** — `ui-design/` onayına dönülür; şablon yazımı durmaz |
| G9 | ADR'siz mimari karar / DevExpress sürümü `26.1` dışı iddia | **DUR** — ADR-001, `.ai/adr/` dışında karar belgesi yazılmaz |
| G10 | `GATE-003` onayı yokken `XRibbonPage.cs` kodu | **DUR** — `ui-design/`'e dön |
| G11 | Hata yolunda `catch { }` / sessiz `default: break;` yutma | **DUR** — §3.7/6 zorunlu |
| G12 | Mojibake / karışık dil / BOM | **DUR** — UTF-8 BOM'suz yeniden yaz |

---

## §5 Workflow

### §5.1 Adım Bazlı Akış

| # | Adım | Girdi | Çıktı | Yapan | Süre |
|---|------|-------|-------|-------|------|
| 1 | Ön koşul | GATE-003 kaydı | onay satırı | ui-designer | 3 dk |
| 2 | Kimlik | menü/ekran tanımı | §3.2 tablosu dolu | developer | 4 dk |
| 3 | Katman | §3.9 | `l7` sınırı çizimi | developer | 3 dk |
| 4 | Hiyerarşi | §3.4 kalıbı | dolu kutu (66 sütun) | developer | 7 dk |
| 5 | İskelet | §3.3 8 adım | `Designer.cs` | developer | 12 dk |
| 6 | İkon | §3.5 | SVG atamaları + adlandırma | developer | 6 dk |
| 7 | Bağlama + durum + kısayol | §3.6 · §3.7b · §3.8 | tek abone + matris + tablo | developer | 16 dk |
| 8 | Doğrulama + kabul | §5.3/1–6 · §6.1 | 0 satır · 21/21 | reviewer → orchestrator | 11 dk |
| | **Toplam/sayfa** | | | | **≈ 62 dk** |

**Zaman bütçesi:** 1–4 ≈ 17 dk · 5–7 ≈ 34 dk · 8 ≈ 11 dk. Sayfa **1 saatten uzun** yazılıyorsa §2.2'ye dön: iş kuralı veya veri erişimi ribbon'a sızmıştır.

### §5.2 Hata Modları

| # | Hata Modu | Belirti | Kök Neden | Düzeltme |
|---|-----------|---------|-----------|----------|
| 1 | Çift `InitializeComponent` | öğeler iki kez | ctor + `Load` içinde çağrı | yalnız ctor |
| 2 | Sıra bozulması | tasarım-süresi `NullReferenceException` | §3.3 adımı atlanmış | 8 adıma dön |
| 3 | Çift `ItemClick` | komut iki kez çalışıyor | `+=` iki yerde | tek abone (G6) |
| 4 | Lambda abonelik | grep denetimi kırılır | `+= (s,e) =>` | adlı metot (§3.6/4) |
| 5 | Bitmap ikon | bulanık, tema dışı | `Image.FromStream` | `SvgImage` (§3.5) |
| 6 | Dağınık `Enabled` | durum tutarsız | her yerde `Enabled =` | tek `ApplyState` (§3.7b) |
| 7 | Kısayol çakışması | iki komut aynı tuşta | tablo güncellenmedi | §3.8 çakışma taraması |
| 8 | Ribbon'da iş kuralı | test edilemez | dispatch içinde karar | `l2`'ye taşı (G5) |
| 9 | Sayfada SQL | `IDbConnection` import'u | katman karıştı | `l2`'ye taşı (G2) |
| 10 | Hata yutma | sessiz `default: break;` | G11 | `Debug.WriteLine` + `lblDurum` |
| 11 | Elle `Designer.cs` düzenlemesi | tasarım-süresi çökme | kod elle girilmiş | IDE üzerinden düzelt |

### §5.3 Salt Okunur Doğrulama Komutları

Altı komut da **hiçbir dosyayı değiştirmez**; `src/` yoksa dürüstçe `DIZIN YOK` yazar.

```powershell
# 1) partial class + XRibbonPage kalitimi — eksik dosya 0
$src = 'C:\versacoder\src\VersaCoder.Desktop'
if (-not (Test-Path $src)) { 'DIZIN YOK -> 0 sayfa (src henuz yok)' } else {
  Get-ChildItem $src -Recurse -Filter '*Page.cs' | Where-Object {
    $_.Name -notlike '*.Designer.cs' } | ForEach-Object {
      if ((Get-Content $_.FullName -Raw -Encoding UTF8) -notmatch
          'partial\s+class\s+\w+Page\s*:\s*XRibbonPage') {
        'EKSIK PARTIAL/KALITIM -> ' + $_.Name } } }

# 2) Baslangic/donus sirasi: SuspendLayout -> InitBas -> ... -> ResumeLayout
Get-ChildItem $src -Recurse -Filter '*.Designer.cs' -EA SilentlyContinue | ForEach-Object {
  $raw = Get-Content $_.FullName -Raw -Encoding UTF8
  if ($raw -notmatch 'SuspendLayout\s*\(\s*\)')        { 'SUSPEND YOK -> ' + $_.Name }
  if ($raw -notmatch 'ResumeLayout\s*\(\s*false\s*\)') { 'RESUME YOK -> ' + $_.Name }
  if ($raw -notmatch 'XRibbonPageGroup')               { 'GRUP YOK -> ' + $_.Name }
  $i = $raw.IndexOf('SuspendLayout()'); $j = $raw.IndexOf('ResumeLayout(false)')
  if ($i -ge 0 -and $j -ge 0 -and $i -gt $j) { 'SIRA BOZUK -> ' + $_.Name } }

# 3) ItemClick tek abone + lambda abonelik yasak
Get-ChildItem $src -Recurse -Filter '*Page.cs' -EA SilentlyContinue | ForEach-Object {
  $raw = Get-Content $_.FullName -Raw -Encoding UTF8
  $n = ([regex]::Matches($raw,'ItemClick\s*\+=')).Count
  if ($n -eq 0) { 'ITEMCLICK YOK -> ' + $_.Name }
  if ($n -gt 1) { 'ITEMCLICK CIFT -> {0}: {1}' -f $_.Name, $n }
  $l = ([regex]::Matches($raw,'\+=\s*\(?\s*\w+\s*,\s*\w+\s*\)\s*=>')).Count
  if ($l -gt 0) { 'LAMBDA ABONELIK -> {0}: {1}' -f $_.Name, $l } }

# 4) Kisayol + SVG ikon — bitmap yasak
Get-ChildItem $src -Recurse -Filter '*Page.cs' -EA SilentlyContinue | ForEach-Object {
  $raw = Get-Content $_.FullName -Raw -Encoding UTF8
  $s = ([regex]::Matches($raw,'Shortcut\s*=')).Count
  $v = ([regex]::Matches($raw,'SvgImage|SetSvgImage')).Count
  if ($s -eq 0) { 'KISAYOL YOK -> ' + $_.Name }
  if ($v -eq 0) { 'SVG IKON YOK -> ' + $_.Name }
  if ($raw -match 'Image\.FromStream|Properties\.\w+\.Bitmap') { 'BITMAP YASAK -> ' + $_.Name } }

# 5) Enabled matrisi tek noktada + yasak referans — 0 satir
Get-ChildItem $src -Recurse -Filter '*.cs' -EA SilentlyContinue | ForEach-Object {
  $raw = Get-Content $_.FullName -Raw -Encoding UTF8
  $e = ([regex]::Matches($raw,'\.Enabled\s*=')).Count
  $a = ([regex]::Matches($raw,'void\s+ApplyState\s*\(')).Count
  if ($e -gt 0 -and $a -eq 0) { 'DAGINIK ENABLED -> ' + $_.Name } }
Get-ChildItem $src -Recurse -Filter '*.cs' -EA SilentlyContinue |
  Select-String -Pattern 'DataAnnotations|EntityFramework|DbConnection|SELECT\s+\*' -Encoding UTF8 |
  ForEach-Object { '{0}:{1}  {2}' -f $_.Path, $_.LineNumber, $_.Line.Trim() }

# 6) Kutu 66 sutun (yalniz ```text bloklari) + kodlama (BOM / mojibake)
$md = 'C:\versacoder\.ai\.templates\frontend\ribbon-template.md'
$inText = $false
Get-Content $md -Encoding UTF8 | ForEach-Object {
  if ($_ -match '^(`{3,})(\w*)') { if ($Matches[1].Length -eq 3) {
      $inText = ($Matches[2] -eq 'text') }; return }
  if ($inText -and $_ -match '^[|+].*[|]$' -and $_.Length -ne 66) { 'KUTU 66 DISI -> ' + $_ } }
Get-ChildItem $src -Recurse -Filter '*.cs' -EA SilentlyContinue | ForEach-Object {
  $b = [System.IO.File]::ReadAllBytes($_.FullName)
  if ($b.Length -ge 3 -and $b[0] -eq 0xEF -and $b[1] -eq 0xBB) { 'BOM VAR -> ' + $_.Name }
  if ((Get-Content $_.FullName -Raw -Encoding UTF8) -match '\uFFFD') { 'MOJIBAKE -> ' + $_.Name } }
```

**Geçme kriteri:** #1–#6 hepsi **0 satır** (veya `src/` yoksa dürüst `DIZIN YOK`); #6 kutu ölçümü **0 uyuşmayan satır**.

### §5.4 Quality Report

```markdown
## Quality Report — ribbon-template (YYYY-MM-DD HH:MM)

| Metrik | Değer | Eşik | Durum |
|---|---|---|---|
| Ölçüm (şablon) | <satır> satır / <KB> KB | 480–720 · 27–45 | 🟢/🔴 |
| Checklist §6.1 | n/21 | 21/21 | 🟢/🔴 |
| Komut §5.3 (#1–#6) | n satır çıktı | 0 | 🟢/🔴 |
| Kutu satırı 66 sütun | n/66 | 0 uyuşmayan | 🟢/🔴 |
| Yer tutucu / mojibake / yasaklı ad | n/n/n | 0/0/0 | 🟢/🔴 |
| Guardrail G1..G12 | ihlal n | 0 | 🟢/🔴 |
| GATE-003 (gerçek ekran kodu) | kayıt var mı | var | 🟢/🔴 |

**Karar:** 🟢 ONAYLANDI / 🔴 RED (<gerekçe>)
```

---

## §6 Doğrulama

### §6.1 Kontrol Listesi (21 madde)

| # | Kontrol | Beklenen | İlgili |
|---|---------|----------|--------|
| K1 | Frontmatter 8 alan + `authority: SSOT` (8. alan `scope`) | 8/8 · `SSOT` | §5.3 |
| K2 | H1 + `**Zorunlu Bağlantılar:**` (2 bağlantı) | 1 satır | üst kısım |
| K3 | `§1.1–§1.4` dolu | 4/4 | §1 |
| K4 | §1.2 disk kanıtları + `⚠️` + yorum yasağı | var | §1.2 |
| K5 | `§2.1–§2.2` dolu | 2/2 | §2 |
| K6 | `§3.1–§3.11` + dolu VersaCoder örneği | 11/11 | §3 |
| K7 | Hiyerarşi + bağlama kutuları 66 sütun | 0 uyuşmayan | §3.4, §3.6 |
| K8 | Kurulum 8 adım tablosu | 8 satır | §3.3 |
| K9 | SVG ikon sözleşmesi 8 kural | 8 | §3.5 |
| K10 | Olay akışı 8 satır + enable matrisi (5 durum × 5 öğe) | 8 + 25 | §3.7, §3.7b |
| K11 | Kısayol tablosu + erişilebilirlik + sekme akışı | var | §3.8 |
| K12 | DevExpress `26.1` geçerli (ADR-001) | doğru sürüm | §3.8 |
| K13 | `§4.1–§4.5` dolu | 5/5 | §4 |
| K14 | Hard Guardrails 12 madde + G8 ayrımı | 12 + G8 | §4.5 |
| K15 | `§5.1` adım tablosu + zaman bütçesi | 8 adım + toplam | §5.1 |
| K16 | `§5.2` ≥ 8 hata modu | 11 | §5.2 |
| K17 | `§5.3` ≥ 5 salt-okunur komut | 6 | §5.3 |
| K18 | `§5.4` Quality Report bloğu | var | §5.4 |
| K19 | `§6.1` ≥ 17 madde | 21 | bu tablo |
| K20 | `§6.2–§6.5` dolu | 4/4 | §6 |
| K21 | `§7.2` ≥ 10 satır · yer tutucu/mojibake | ≥ 10 · 0/0 | §7 · G12 |

### §6.2 Red Sinyalleri (Otomatik DUR)

| # | Sinyal | Aksiyon |
|---|--------|---------|
| R1 | §5.3 komutlarından biri > 0 satır | **DUR** — gösterilen dosya düzeltilir |
| R2 | §3.4/§3.6 kutusunda satır ≠ 66 | **DUR** — ölçümle hizalanır |
| R3 | `partial` / `XRibbonPage` kalıtımı / `SuspendLayout` eksik | **DUR** — §5.3/1–2 |
| R4 | Sayfada `EntityFramework` geçen satır | **DUR** — ADO.NET + Dapper (`l4`) |
| R5 | `ItemClick +=` sayısı ≠ 1 | **DUR** — §3.6/1 |
| R6 | Hata yolunda sessiz `catch` / sessiz `default` | **DUR** — §3.7/6 |
| R7 | GATE-003 kaydı yokken `XRibbonPage.cs` kodu | **DUR** — `ui-design/`'e dönülür |
| R8 | Mojibake / BOM / yasaklı vault adı | **DUR** — UTF-8 BOM'suz yeniden yazılır |
| R9 | Bitmap ikon (`Image.FromStream`) | **DUR** — §3.5 SVG |
| R10 | `ApplyState` yokken `.Enabled =` | **DUR** — §3.7b tek matris |

### §6.3 Tür / Sürüm Geçiş Kuralları

| Değişiklik | Etki |
|-----------|------|
| Yalnız metin/tooltip düzeltmesi | `version` patch, `updated` değişir |
| Öğe/grup eklendi-çıkarıldı | §3.4 + §3.7b matrisi yeniden |
| Kısayol değişti | §3.8 tablosu + çakışma taraması + `version` minor |
| Olay akışı değişti | §3.7 + `version` minor + inceleme |
| DevExpress yükseltme talebi | **RED** — ADR-001, sürüm `26.1` kilitli |

### §6.4 İlgili Dosyalar

| Dosya | İlişki |
|-------|--------|
| `.ai/.templates/frontend/ribbon-template.md` | **bu dosya** — şerit ayağının tek kaynağı |
| `.ai/.templates/frontend/form-template.md` | taşıyıcı `RibbonForm`; öğelerin bağlandığı ekran |
| `.ai/.templates/frontend/component-template.md` | ribbon içine gömülebilecek özel kontrol |
| `.ai/.templates/index.md` | envanter + kalite sözleşmesi §3.7 (satır 222) |
| `.ai/.templates/architecture/ascii-diagram-template.md` | kutu biçimi ve 66 sütun ölçümü |
| `.ai/.templates/adr/adr-devexpress-template.md` | §3.8 sürüm kilidi (ADR-001) |
| `.ai/.templates/backend/handler-template.md` | §3.9'daki `l2` handler'ın kaynağı |
| `.ai/.templates/ui-design/wireframe-template.md` | menü/komut tanımının ön koşulu |

### §6.5 Envantere Özgü Kontroller

| # | Kontrol | Beklenen | Kanıt |
|---|---------|----------|-------|
| K1 | Bu şablonun KB aralığı (envanter satır 222: `13,0`) | 27–45 | `(Get-Item …).Length / 1KB` |
| K2 | Bu şablonun satır aralığı (G6 yalnız alt sınır koyar) | 480–720 | `Get-Content … Count` |
| K3 | Kutu satırları 66 karakter (§3.4 + §3.6) | 0 uyuşmayan | §5.3/6 ölçümü |
| K4 | DevExpress `26.1` + `frontend/` 3 şablon | 3 | `Get-ChildItem .templates\frontend` |
| K5 | Yer tutucu yalnız §3.11 kalıp bloğu | 0 (diğer yer) | `Select-String '\{\{[A-Z0-9_]+\}\}'` |

---

## §7 Referanslar

### §7.1 VersaCoder Bağlantıları

| Kaynak | Yol | Ne İçin | Durum (2026-10-05) |
|--------|-----|---------|---------------------|
| Şablon envanteri | `.ai/.templates/index.md` | §3.7 sözleşmesi · satır 222 | ✅ okundu |
| Kardeş şablon (altın örnek) | `.ai/.templates/frontend/form-template.md` | biçim + §3.3 kalıbı | ✅ 590 satır · 32,6 KB |
| Biçim altın örneği | `.ai/.templates/architecture/ascii-diagram-template.md` | kutu 66 sütun | ✅ var |
| DevExpress kararı | `.ai/.templates/adr/adr-devexpress-template.md` | §3.8 sürüm kilidi | ✅ var |
| Kök SSOT (G8) | `C:\versacoder\CLAUDE.md` | GATE-003 kapsamı | ✅ var |
| Ekran onayı | `.ai/ui-design/` + `GATE-003` | ön koşul (G8, G10) | ❌ YOK → `⚠️` |
| Hedef kod | `src/VersaCoder.Desktop/**/XRibbonPage.cs` | bu şablonun çıktısı | ❌ YOK → `⚠️` |
| Yasaklı vault'lar | `C:\www\…` (envanter G1) | yalnız **yapı** referansı | 🔒 kopyalama YASAK |

### §7.2 Red Team — Sık Yapılan Hatalar

| # | Hata | Neden Yanlış | Doğrusu |
|---|------|--------------|---------|
| 1 | Ribbon'a iş kuralı yazmak | test edilemez, iki gerçek kaynağı | mantık `l2` handler'da (§3.9) |
| 2 | Sayfada SQL / `EntityFramework` | katman ihlali, gizli bağımlılık | komut `l2`'ye gider (G2) |
| 3 | `e.Item.Name` if/else zinciri | her öğe eklendiğinde kırılır | tek `switch` + `default` günlüğü (§3.6) |
| 4 | Her `Load`'ta `ItemClick +=` | komut iki kez çalışıyor | ctor'da **tek** abone (G6) |
| 5 | `+= (s,e) => ...` lambda | grep denetimi kırılır, taşınamaz | adlı metot (§3.6/4) |
| 6 | Bitmap ikon gömmek | DPI/tema'da bulanık | `ImageOptions.SetSvgImage` (§3.5) |
| 7 | `Enabled` atamalarını dağıtmak | durum tutarsızlığı | tek `ApplyState` matrisi (§3.7b) |
| 8 | Kısayol koymamak | klavye kullanıcısı engelli | §3.8 tablosu + çakışma taraması |
| 9 | Yalnız ikonla komut vermek | erişilebilirlik + renk-körü | `Caption` zorunlu (§3.5/6) |
| 10 | Sekme değişince filtre/veri sıfırlanmak | kullanıcı veri kaybı | §3.8 sekme akışı |
| 11 | GATE-003'ten önce `XRibbonPage.cs` kodu | onaysız ekran, ~60 dk kaybı | tel kafes onayı koddan önce (G8/G10) |
| 12 | Şablonu yazmadan sayfa üretmek | ikinci gerçek kaynağı doğar | önce bu dosya (§4.1) |

**SSUT beyanı:** Bu dosya DevExpress ribbon sayfası üretiminin **tek** kaynağıdır; her sayfa, `.ai/ui-design/` onayı ve bu şablon üzerinden üretilir. Çoğaltılan her satır ikinci gerçek kaynağı doğurur (envanter G12).
