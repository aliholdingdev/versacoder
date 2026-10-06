---
title: "VersaCoder — WinForms Özel Kontrol (XControl) Şablonu"
type: template
category: templates/frontend
status: active
version: 1.0.0
updated: 2026-10-05
authority: SSOT
scope: "WinForms ozel kontrolu: partial class iskeleti, property/ViewState sozlesmesi, olay baglari, Dispose temizligi"
---

# VersaCoder — WinForms Özel Kontrol (XControl) Şablonu

**Zorunlu Bağlantılar:** [index](../index.md) · [form-template](./form-template.md)

**Bölümler:** §1 Amaç · §2 Kapsam · §3 Mimari · §4 Kurallar · §5 Workflow · §6 Doğrulama · §7 Referanslar

---

## §1 Amaç

Bu şablon, VersaCoder masaüstü uygulamasında **yeniden kullanılan her WinForms özel kontrolünün tek biçimde** üretilmesi için zorunludur: sınıf/parça sözleşmesi, `InitializeComponent` sırası, property–ViewState bağlaması, olay tetikleme biçimi, `Dispose` temizliği, tasarım-süresi (designer) desteği, klavye/sekme erişilebilirliği, yüksek DPI, tema token'ları ve test edilebilirlik tek dosyada tanımlanır. `T-FE`'nin kontrol ayağıdır; form ayağı `form-template`, şerit ayağı `ribbon-template`'tir.

| Alan | Değer |
|------|-------|
| Template Name | `component-template.md` |
| Template Path | `.ai/.templates/frontend/component-template.md` |
| Hedef Dosya | `src/VersaCoder.Desktop/Controls/*.cs` (+ `*.Designer.cs`) |
| Hedef Kod | C# · `partial class` · WinForms · DevExpress WinForms **26.1** |
| Kalıtım | `UserControl` / `Control` / `Panel` türevi — DevExpress kontrolü **türetilmez, sarmalanır** |
| Kodlama | **UTF-8 (BOM'suz)** — Latin-1/CP1254 YASAK |
| Üretilen Uzunluk | 120–260 satır (kod) + bu şablonun 480–720 satırı |
| Dil | Türkçe (ç ğ ı İ ö ş ü doğru) — sınıf/alan/property adı İngilizce |
| Katman | `l7 UI + CLI` (Presentation sınırı) — `l0..l7` §3.9 |
| Versiyon | 1.0.0 |
| Authority | SSOT (bu dosya) |
| Bootstrap | 2026-10-05 |

### §1.1 Neden Kontrol Şablonu Ayrı Bir Kaynak?

Kontrol kodu, form kodundan **daha sessiz** bozulur: property eklendiğinde olay tetiklenmez, `Dispose` unutulduğunda timer sızar, tasarım-süresi ataması çalışma zamanını kirletir. Tek kaynak yoksa aynı kontrol iki yerde iki farklı davranış gösterir.

| Boşluk | Belirti | Bu Şablonun Kestiği Şey |
|--------|---------|-------------------------|
| Property sessiz değişir | arayüz güncellenmez | §3.5 `On{{Prop}}Changed` zorunluluğu |
| Çift olay aboneliği | buton iki kez çalışıyor | §3.6 tek abone + §5.3/5 |
| `Dispose` yarım | timer/GC sızıntısı | §3.9 yıkım temizliği |
| Tasarım-süresi kodu çalışma zamanına sızar | `DesignMode` dalgalanır | §3.5 `Site`/`DesignerSerializationVisibility` |
| Sekme ve klavye yok | fare zorunlu | §3.8 `TabStop` + ok tuşu akışı |
| Tema her dosyada tekrar | renk tutarsızlığı | §3.8 tek noktadan design-token |

### §1.2 Disk Kanıtları ve Boşluk (YAGNI)

İddia → kanıt. Kanıt yoksa uydurulmaz, `⚠️ VERIFICATION REQUIRED` yazılır. Ölçüm: `C:\versacoder`, 2026-10-05.

| # | İddia | Durum | Kanıt |
|---|-------|-------|-------|
| 1 | `.ai/.templates/frontend/component-template.md` (yazım öncesi) | ❌ YOK | `Test-Path = False` — bu dosya ile açılıyor |
| 2 | `.ai/.templates/frontend/` = **1 MD** (yalnız kardeş) | ✅ DOĞRULANDI | `Get-ChildItem` = `form-template.md` · 590 satır · 32,6 KB |
| 3 | `frontend/ribbon-template.md` (sıradaki şablon) | ❌ YOK | `Test-Path = False` — Part C #32 |
| 4 | Envanter satır 31: `PLANLI · 14,0 KB · developer` | ✅ DOĞRULANDI | `.ai/.templates/index.md` §3.4 Part C |
| 5 | `src/VersaCoder.Desktop/Controls/*.cs` (hedef kod) | ❌ YOK | `src/` = `Test-Path False` |
| 6 | `.ai/ui-design/` + `GATE-003` tel kafes onayı | ❌ YOK → ⚠️ | G8: onaysız **ekran/kod** üretilmez |
| 7 | `.ai/.rules/` · `.ai/WORKFLOW.md` · `.ai/keys.md` · `.ai/log.md` | ❌ YOK | envanter beyanı, diske işlenmemiş |
| 8 | `global.json` · `Directory.Packages.props` | ❌ YOK | `Test-Path = False` |
| 9 | Kök `README.md` · `CLAUDE.md` · `AGENTS.md` · `WORKFLOW.md` | ✅ VAR | `Test-Path = True` (4 dosya) |
| 10 | DevExpress sürümü **26.1** (ADR-001) | ⚠️ VERIFICATION REQUIRED | `.ai/adr/` (gerçek ADR) dizini diskte YOK |
| 11 | Harici referans vault'ları (envanter G1) | 🔒 ERİŞİM YASAK | yalnız **yapı** referansı; tek kelime kopyalanmadı |

**Yorum yasağı:** Var olmayan dosya için "şurada tanımlı" denmez; `⚠️ VERIFICATION REQUIRED` yazılır, satır olduğu yerde durur. §1.2'deki ❌ satırları §3.11'deki örneklerin **tasarım** olduğunu, diskte henüz kontrol kodu bulunmadığını hatırlatır.

### §1.3 Kontrol Ne Değildir?

| Kontroldür | Kontrol Değildir |
|------------|------------------|
| Bir **girdi/görüntüleme parçası**, formun çocuğu | Ekranın kendisi (→ `form-template`) |
| Property + olay + çizim sözleşmesi | İş kuralı kararı (→ `l2 Application` handler) |
| Yerel, saf **ViewState** (bellekte) | Kalıcı veri (→ `l4 Infrastructure`, Dapper) |
| Tasarım-süresi yeniden kullanılabilirlik | Ribbon sayfa tanımı (→ `ribbon-template`) |
| `On{{Prop}}Changed` ile **duyuru** | Duyurunun alıcısı (→ form, §3.6) |

### §1.4 Kontrol Yaşam Döngüsü

```text
+----------------------------------------------------------------+
| [1] new XKontrol()      alanlar + varsayılan (tasarım yok)     |
|    v                                                           |
| [2] InitializeComponent  8 adım (§3.3) · tek sefer             |
|    v                                                           |
| [3] Sahip forma eklen   Controls.Add → TabIndex 0..N (§3.8)    |
|    v                                                           |
| [4] Property ata        OnChanged → Invalidate/PerformLayout   |
|    v                                                           |
| [5] Kullanıcı girişi    Click/TextChanged → On{{Olay}} → form  |
|    v                                                           |
| [6] Dispose(disposing)  abone -= · timer stop · handle serbest |
|    ← geri dönüş: [5]'e hata döner; kontrol YIKILMAZ (§3.7/6)   |
+----------------------------------------------------------------+
```

**Ne anlatır:** Kontrolün kurulum→duyuru→yıkım döngüsü; hata durumunda akış `[6]`'ya değil `[5]`'e geri döner (§3.7).

---

## §2 Kapsam

**İçinde:** kontrol sınıfı yapısı, `InitializeComponent` sırası, property/ViewState sözleşmesi, olay tetikleme, `Dispose` temizliği, tasarım-süresi desteği, erişilebilirlik, DPI, tema token'ı, salt-okunur doğrulama.
**Dışında:** form iskeleti, ribbon sayfası, iş kuralı, SQL/EF, paket sürümü kararı, tel kafes onayı.

| Kapsam | Kapsam Dışı |
|--------|-------------|
| `Controls/XKontrol.cs` + `XKontrol.Designer.cs` ikilisi | Formun kendi iskeleti (→ `form-template`) |
| `partial class` · `SuspendLayout` · `Dispose(bool)` | Ribbon sayfa/grup/öğe (→ `ribbon-template`) |
| Property + `On{{Prop}}Changed` + `EventArgs` | Kullanıcı sözleşmesi (→ `REQUIREMENTS.md`) |
| Olay tetikleme + tek abone kuralı | Kalıcı kural (→ `.ai/.rules/` — diskte YOK) |
| `TabIndex`/`TabStop` + DPI + tema token | Sürüm kilidi kararı (→ ADR-001) |
| Salt-okunur denetim komutları | Sorgu/şema (→ ADR-004) |

### §2.1 Dosya Tipi → Sorumlu Eşlemesi

| Dosya Tipi | Sorumlu | Bu Şablondan mı |
|------------|---------|------------------|
| `src/VersaCoder.Desktop/Controls/*.cs` (özel kontrol) | developer | ✅ **bu dosya** |
| `src/VersaCoder.Desktop/**/XForm.cs` (ana ekran formu) | developer | ❌ → `form-template` |
| `src/VersaCoder.Desktop/**/XRibbonPage.cs` (şerit sayfası) | developer | ❌ → `ribbon-template` |
| `src/VersaCoder.Desktop/Forms/*.cs` (Part D #43) | developer | ❌ → Part D `winforms-form-template` |
| `.ai/ui-design/01-…07-*.md` (ekran tanımı) | ui-designer | ❌ ön koşul, GATE-003 |

### §2.2 Kapsam Sınırı Testi

Üretilecek `.cs` dosyasına sırayla sor; **ilk `EVET`** şablonu belirler:

1. **`Form` türevi mi** (`XtraForm`, `RibbonForm`)? → `form-template`.
2. **`Control` türevi mi ve `Form` değil mi?** → **bu şablon**.
3. **`XRibbonPage` / `XRibbonPageGroup` barındırıyor mu?** → `ribbon-template`.
4. **Yalnız veri erişimi mi içeriyor** (SQL, Dapper)? → `backend/repository-template`.
5. **Yalnız iş kuralı mı içeriyor** (use case)? → `backend/service-template`.
6. **Hiçbiri değilse** → şablon yoktur; talep `index.md` §5.1 Adım 1'den açılır.

`HAYIR` soruları: CoreMusic/versacoder.ai içeriği mi (G1)? · `EntityFramework` geçiyor mu (G2)? · `[Required]`/DataAnnotations var mı (G7)? · `GATE-003` onaysız `src/**` kodu mu (G8)?

---

## §3 Mimari

Bu bölüm hem **şablonun kendi yapısını** hem **üretilecek kontrolün yapısını** tanımlar.

### §3.1 Dosya ve Sınıf Sözleşmesi

| Öğe | Kural | Doğru | Yanlış |
|-----|-------|-------|--------|
| Dosya ikilisi | `XKontrol.cs` + `XKontrol.Designer.cs` | ikisi de var | tek dosya (tasarım-süresi kaybı) |
| Sınıf bildirimi | `public partial class XKontrol : UserControl` | `partial` zorunlu | `partial`'siz |
| `Designer.cs` sahipliği | yalnız Visual Studio/tasarım-süresi yazar | elle düzenleme yasak | elle `Edit & Continue` |
| Adlandırma | `X` öneki + İngilizce rol adı | `XBudgetMeter`, `XFilterBar` | `Kontrol1`, `MyThing` |
| Dosya yolu | `src/VersaCoder.Desktop/Controls/{{KontrolAdi}}.cs` | kökten yazılır | dağınık kök klasör |
| Kalıtım | kendi kontrolümüz `UserControl`/`Control` | sarmalama | DevExpress sınıfından **türetme** |
| TFM | `net48;net10.0-windows` ikilisinde derlenir | `#if NET48` farkı bilinçli | tek TFM'e göre yazım |

### §3.2 Kontrol Kimliği ve Amacı

Her kontrol için şu alanlar **tek satırda** doldurulur; boş hücre kabul edilmez:

| Alan | Açıklama | Zorunlu Örnek Kalıp |
|------|----------|----------------------|
| `KontrolAdı` | sınıf adı | `X{{Rol}}` |
| `Dosya` | kökten yol | `src/VersaCoder.Desktop/Controls/X{{Rol}}.cs` |
| `Amaç` | tek cümle, "ne yapar" | `{{AMAC}} bilgisini {{SONUC}} biçiminde gösterir.` |
| `Kullanan Form` | sahip ekran | `{{EkranAdi}}Form` |
| `Dışa Açtığı Olay` | en fazla 3 | `{{Olay}}Changed`, `{{Olay}}Requested` |
| `Bağlı Use Case` | `l2` handler | `{{UseCaseAdi}}Handler` |
| `Tema Token'ı` | §3.8 | `Token.{{Alan}}` |
| `Onay Kapısı` | üretilmeden önce | `GATE-003` tel kafesi ile birebir eşleşme |

### §3.3 `InitializeComponent` Yapısı (8 Sabit Adım)

Sıra **değiştirilemez**; değiştirilen kontrol tasarım-süresinde kayar ve denetim komutu (§5.3/2) sırayı doğrulayamaz.

| # | Adım | Ne Yapılır | İhlal Belirtisi |
|---|------|-----------|-----------------|
| 1 | `SuspendLayout()` | ölçüm olayları bastırılır | arkada titreşim |
| 2 | `resources.ApplyResources(...)` | yerelleştirme/boyut | metin kayması |
| 3 | Alt kontrolleri **dip→yukarı** oluştur | iç kontroller önce | `NullReferenceException` |
| 4 | `((ISupportInitialize)(...)).BeginInit()` | Grid/Editor gibi | tasarım-süresi çökme |
| 5 | Özellikleri ata (`Name`, `TabIndex`, `TabStop`, `Dock`/`Anchor`, tema token) | sekme + görünüm | sekme sırası bozuk |
| 6 | Olaylara **tek seferde** abone | `Click += ...` | çift tetikleme |
| 7 | `Controls.Add(...)` zinciri | ebeveyn→çocuk | alt kontrol görünmez |
| 8 | `ResumeLayout(false)` + `PerformLayout()` | ölçüm yeniden açılır | DPI taşması |

**Yasak:** 6. adımda abone edilen her olay için `-= null` türü temizlik; abonelik **kontrolün kendi abonelikleri için** `Dispose`'a, **sahip formun abonelikleri için** forma aittir (§3.9, §4.5 G6).

### §3.4 Bileşen Ağacı (ASCII kutu — 66 sütun)

```text
+----------------------------------------------------------------+
| {{KontrolSinifi}} : UserControl                                |
| +-- {{Icerik}}         Panel        Dock=Fill · SuspendLayout  |
| |   +-- {{Etiket}}     Label        Token.{{Rol}} · AutoSize   |
| |   +-- {{Alan_1}}     {{Tur}}      Bind: {{Alan}} · TabIndex 0|
| |   +-- {{Alan_2}}    {{Tur}}      Bind: {{Durum}} · TabIndex 1|
| +-- {{Dugme_1}}        SimpleButton Click → {{Olay}} · İndeks 2|
| +-- {{Dugme_2}}        SimpleButton Ctrl+{{K}} · İndeks 3      |
| +-- {{DurumEtiketi}}   Label        HATALI yazıyla da (§3.8)   |
+----------------------------------------------------------------+
```

**Ne anlatır:** Bir kontrolün izin verilen alt kümesi ve hiyerarşisi; her satır `{{…}}` yerine gerçek kontrol adıyla doldurulur.
**Doğrulama:** §5.3/6 — kutu satırları tam 66 karakter olmalı.

### §3.5 Property / ViewState Sözleşmesi

Kontrolün **tek gerçek kaynağı**, public property'lerin arkasındaki `private` ViewState'idir. Otomatik (`auto`) property **kabul edilmez**: değişiklik duyurusu kopar, arayüz sessiz kalır.

| # | Kural | Doğru | Yanlış |
|---|-------|-------|--------|
| 1 | Alan `private` + tam property | `private int _deger;` + `public int Deger` | `public int Deger { get; set; }` |
| 2 | Set yolunda fark yoksa **erken dön** | `if (_deger == v) return;` | her sette tam yeniden çizim |
| 3 | Fark varsa duyuru | `OnDegerChanged(e); Invalidate();` | yalnız `Invalidate()` |
| 4 | Duyuru sanal metot | `protected virtual void OnDegerChanged(...)` | doğrudan `event?.Invoke` set içinde |
| 5 | Tasarım-süresi metaveri | `[Category]` `[Description]` `[DefaultValue]` | öznitelik yok |
| 6 | Serileştirme | `[DesignerSerializationVisibility(Visible)]` | `Hidden` (tasarım kaybı) |
| 7 | Tasarım-süresi değeri | `Site?.GetService(...)` / `DesignMode` | `if (DesignMode)` tek başına (dalgalanır) |
| 8 | Çocuk etkisi | `PerformLayout()` yalnız child boyutu değiştiğinde | her sette layout |

**ViewState tablosu (üretilen kontrolde doldurulur):**

| Property | Tip | Varsayılan | Child Etkisi | Olay Tetikler | Tasarım-Süresi |
|----------|-----|-----------|--------------|----------------|-----------------|
| `{{Prop_1}}` | `{{Tip}}` | `{{Deger}}` | evet → `PerformLayout` | `{{Prop}}Changed` | `true` |
| `{{Prop_2}}` | `bool` | `false` | hayır → `Invalidate` | `{{Prop}}Changed` | `true` |
| `{{Prop_3}}` | `string` | `""` | hayır → `Invalidate` | `{{Prop}}Changed` | `true` |

### §3.6 Olay Bağlama Kuralları

Kontrol olay **yaratır**, iş **yapmaz**. Tetikleme biçimi tek kalıptır; sahip formda abonelik **birden fazla kez olmaz** (G6).

| # | Kural | Doğru | Yanlış |
|---|-------|-------|--------|
| 1 | Olay imzası | `event EventHandler<{{Args}}EventArgs> {{Olay}}` | `event Action<object>` |
| 2 | Tetikleyici | `protected virtual On{{Olay}}(e)` → null kontrolü | set içinde satır arası `Invoke` |
| 3 | `e` asla `null` | `e ?? EventArgs.Empty` uygun `EventArgs` tipi | `null` göndermek |
| 4 | Koşullu tetikleme | koşul açıkça yazılır (`if (_kullanilabilir)`) | koşulsuz her harekette |
| 5 | Sahip formda abonelik | ctor'da **bir kez** `+=` | her `Load`'ta `+=` |
| 6 | Çözme | sahip `Dispose`'ında `-=` | GC'ye bırakmak |
| 7 | Adlandırma | `…Changed` / `…Requested` / `…Validating` | `OnClick2`, `Event1` |

**Bağlantı zinciri:**

```text
+----------------------------------------------------------------+
| Kullanıcı    Click / TextChanged / SelectedIndexChanged        |
|    v                                                           |
| XKontrol     On{{Olay}}(e)   (protected virtual, §3.6/2)       |
|    v                                                           |
| Sahip Form   _xKontrol.{{Olay}} += OnX (ctor'da tek kez)       |
|    v                                                           |
| l2 Handler   I{{UseCase}}Handler.HandleAsync(komut)            |
|    v                                                           |
| Sonuç        form durumu güncellenir · kontrol YIKILMAZ        |
|    ← hata: form lblDurum'a yazar, kontrole mesaj YAZMAZ        |
+----------------------------------------------------------------+
```

### §3.7 Olay Akışı ve Hata Durumları

| # | Olay | Tetikleyen | Handler / Akış | Başarı | Hata Durumu |
|---|------|-----------|----------------|--------|-------------|
| 1 | Kontrol ctor | sahip forma eklenme | alanlar + varsayılan | hazır | `ObjectDisposedException` riski → §3.9 |
| 2 | `ThemeChanged` | tekil tema okuma | token tazele + `Invalidate` | yeniden çizim | varsayana düş, uygulama **durmaz** |
| 3 | `{{Prop}}Changed` | property set | `On{{Prop}}Changed` → duyuru | sahip güncellenir | ayar geri alınır, ekranda uyarı |
| 4 | `Validating` | odak kaybı | saf kontrol kuralı | odak ilerler | `CancelEventArgs.Cancel = true` |
| 5 | `{{Olay}}` tetiği | kullanıcı niyeti | sahip form → `l2` handler | form durumu güncellenir | kontrol durumu **değişmez** |
| 6 | handler `failure` | Application | hata kodu → metin | — | sahip form gösterir; kontrol **kapanmaz/yıkılmaz** |
| 7 | `Enabled = false` | sahip / kural | child'lar devre dışı | odak düşer | olay **tetiklenmez** (matris §3.7b) |
| 8 | `Dispose(true)` | sahip kaldırma | abone `-=` · timer stop | temiz | yarım `Dispose` → sızıntı |

**Kural:** 6. satır **her zaman** yazılır; başarı yolu asla tek yol olarak kodlanmaz (G11).

#### §3.7b Enable/Disable Durum Matrisi

| Durum | `{{Prop_1}}` | Girdi | Olay Tetikler mi | Odak | Görsel |
|-------|--------------|-------|------------------|------|--------|
| Normal | `{{Deger_A}}` | açık | evet | alabilir | tam renk |
| Devre dışı | `{{Deger_B}}` | kapalı | **hayır** | alamaz | `%50 + yazıyla `PASİF`` |
| Yükleme | — | kapalı | hayır | alamaz | `HATALI`/`BEKLİYOR` yazısı |
| Hata | `{{Deger_C}}` | açık | evet | alabilir | `ErrorIcon` + kırmızı **ve** metin |

### §3.8 Erişilebilirlik, Sekme Sırası, Yüksek DPI ve Tema

| Alan | Kural | Denetim |
|------|-------|---------|
| `TabIndex` | sahip formun içinde 0..N **kesintisiz**, okuma sırasıyla eş | §5.3/3 |
| `TabStop` | girdi `true`, yalnız görsel `false` | §5.3/3 |
| Klavye | `Enter`/`Space` tetikler; ok tuşları alt kontrol arasında dolaşır (`ProcessDialogKey`) | §6.5 K3 |
| Erişilebilir ad | `AccessibleName` + `AccessibleDescription` (Türkçe) | gözlem |
| Renk-körü | durumu **yazıyla** da ver (`HATALI`, `PASİF`), yalnız renkle değil | §3.7b |
| DPI 100–200% | `AutoScaleMode.Dpi` · `Anchor`/`Dock` · sabit `Width`/`Height` yasak · çizimde `DeviceDpi` | §5.3/4 |
| Tema | renk/boyut **yalnız** `Tokens.cs` (design-token) · `LookAndFeel` tek noktadan | §5.3/5 |
| DevExpress 26.1 | sürüm **yükseltilmez** (ADR-001) · kontrol türetilmez, sarmalanır | §6.5 K4 |

**DPI notu (26.1):** `DpiChanged`'de **kontrol boyutu elle ayarlanmaz**; `AutoScaleDimensions` tasarlandığı DPI'a sabit kalır, `AutoScaleMode.Dpi` devreye girer. `net48` tarafında DPI-işaretleme anahtarı **açılmaz**; ikili TFM'deki davranış farkı `#if NET48` ile belgelenir. Temele sabit `Color.FromArgb(...)` **yazım anında** RED; karşılığı `Token.{{Alan}}` olmalıdır.

### §3.9 Katman Sınırı (l0..l7) ve DevExpress Yeri

| Katman | Ad | Bu Kontrol Ne Alır | Bu Kontrol Ne Vermez |
|--------|----|--------------------|----------------------|
| `l0` | Abstractions | — | — |
| `l1` | Domain | invariant **bilgisi** (salt okunur `{{Tip}}`) | nesne tutma, güncelleme |
| `l2` | Application | komut/use case **sonucu** (DTO) | kontrol tipi, `Control` referansı |
| `l3` | Crosscutting | sonuç kodu → metin (`{{HataKodu}}`) | — |
| `l4` | Infrastructure | **asla** (G2: EF/SQL yasak) | — |
| `l5` | Protocol | — | — |
| `l6` | Host | — | — |
| `l7` | **UI + CLI** | kontrol burada yaşar | iş kuralı, sorgu |

**Kural:** DevExpress WinForms **yalnız `l7` Presentation sınırında** görünür; `l1`–`l6` hiçbirinde `DevExpress.*` `using`'i geçmez (§5.3/5). Kontrol, `l2`'ye **tip** değil **olay** gönderir; `l2` kontrolü **hiç görmez**.

### §3.10 Test Edilebilirlik (ctor DI · `internal`)

| # | Kural | Neden | Doğru |
|---|-------|-------|-------|
| 1 | Bağımlılık **ctor** ile gelir | gizli `new` test edilemez | `internal XKontrol(I{{Bağımlılık}} bagimlilik)` |
| 2 | `internal` erişim + `InternalsVisibleTo` | public API kirletilmez | test projesi `Controls`'a ulaşır |
| 3 | `On{{Olay}}` **`virtual`** | test override ile gözlemler | `sealed` metot |
| 4 | Saf çekirdek ayrı | çizim/diyalog testi dışarıda | `{{Cekirdek}}` hesabı ayrı sınıf |
| 5 | `DesignMode` dalgalanması | ctor'da güvenilmez | `Site` üzerinden karar |
| 6 | Timer/Task testte durdurulur | yarış koşulu | `StopAsync`/`Dispose` zorunlu |

**Test tablosu (`tests/VersaCoder.Tests/**` — diskte YOK, plan):**

| # | Test | Arrange | Assert |
|---|------|---------|--------|
| T1 | Varsayılan ViewState | `new X{{Rol}}()` | §3.5 varsayılanlar birebir |
| T2 | Farklı set olayı **bir kez** tetikler | `p = yeni` | olay sayısı `== 1` |
| T3 | Aynı değer seti tetiklemez | `p = aynı` | olay sayısı `== 0` |
| T4 | `Enabled=false` iken olay yok | `Enabled = false; tıkla` | olay `0` (§3.7b) |
| T5 | Yıkım temizliği | `Dispose()` sonra olay | `ObjectDisposedException` yok, abone `0` |
| T6 | Saf çekirdek hesabı | girdi kümesi | §6.1 K9 beklenen değer |

### §3.11 Dolu VersaCoder Örneği ve Tam Kalıp

**Dolu örnek (gerçek VersaCoder verisiyle — `XBudgetMeter`):**

```text
+----------------------------------------------------------------+
| XBudgetMeter : UserControl                                     |
| +-- pnlIcerik        Panel       Dock=Fill                     |
| |   +-- lblBaslik    Label       Token.Heading · AutoSize      |
| |   +-- prgUsage    ProgressBar Value={{Kullanilan}} · Indeks 0|
| |   +-- lblOran      Label       "%{0:N0} · yazı zorunlu       |
| +-- btnAyristir      SimpleButton Ctrl+D · İndeks 1 · Enabled  |
| +-- lblDurum         Label       HATALI / PASİF yazılı (§3.7b) |
+----------------------------------------------------------------+
```

**Tam kalıp (yalnız bu blokta `{{…}}` yaşar; üretilen kodda adedi 0 olmalıdır):**

````markdown
```csharp
public partial class X{{Rol}} : UserControl
{
    private {{Tip}} _{{alan}} = {{Varsayilan}};

    public X{{Rol}}(I{{Bağımlılık}} bagimlilik)
    {
        bagimlilik = bagimlilik ?? throw new ArgumentNullException(nameof(bagimlilik));
        InitializeComponent();                 // §3.3 — 8 adım, tek sefer
        AutoScaleMode = AutoScaleMode.Dpi;     // §3.8
    }

    [Category("Veri"), Description("{{Aciklama}}"),
     DefaultValue(typeof({{Tip}}), "{{Varsayilan}}")]
    public {{Tip}} {{Prop}}
    {
        get => _{{alan}};
        set
        {
            if (Equals(_{{alan}}, value)) return;   // §3.5/2 — fark yoksa erken dön
            _{{alan}} = value;
            On{{Prop}}Changed(EventArgs.Empty);      // §3.5/4 — tek duyuru noktası
            Invalidate();
        }
    }

    public event EventHandler {{Prop}}Changed;

    protected virtual void On{{Prop}}Changed(EventArgs e)
        => {{Prop}}Changed?.Invoke(this, e);

    protected override bool ProcessDialogKey(Keys keyData)   // §3.8 — klavye
    {
        if (keyData == (Keys.Control | Keys.{{Tus}})) { On{{Olay}}(); return true; }
        return base.ProcessDialogKey(keyData);
    }

    protected override void Dispose(bool disposing)          // §3.9 — yıkım
    {
        if (disposing) { _{{abone}}?.Dispose(); _{{abone}} = null; }
        base.Dispose(disposing);
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
| 2 | Ekran/kullanım tanımı varsa | §3.2 kimlik satırını doldur |
| 3 | Katman sınırı | §3.9 — DevExpress yalnız `l7` |
| 4 | Ağaç + property | §3.4 · §3.5 — auto-property yasak |
| 5 | `InitializeComponent` | §3.3 8 adım, değiştirilemez sıra |
| 6 | Olay bağlama | §3.6 — tek abone, `On{{Olay}}` sanal |
| 7 | Olay + hata + durum | §3.7 + §3.7b — 8 satır dolu |
| 8 | Erişilebilirlik + DPI + tema | §3.8 — `TabIndex`/`TabStop` |
| 9 | Dispose + test | §3.9 · §3.10 |
| 10 | Doğrulama | §5.3 komutları → 0 satır |

### §4.2 Sorumluluk ve Onay

| Aşama | Rol | Ne Onaylar | Çıktı | Süre |
|-------|-----|-----------|-------|------|
| 1 | ui-designer | tel kafes ↔ §3.2 amacı | GATE-003 onayı | 10 dk |
| 2 | developer | §3.1–§3.11 kodu | `Controls/X{{Rol}}.cs` + `Designer.cs` | 35 dk |
| 3 | reviewer | §6.1 (21 madde) | ✅ / RED | 10 dk |
| 4 | architect | katman: `l7` → `l2` olay bağı, ters referans yok | §3.9 çizimi | 8 dk |
| 5 | master-orchestrator | §5.3 çıktısı + §5.4 | Quality Report | 5 dk |

> **3 RED** sonrası iş durur; gerekçe `master-orchestrator`'a yükseltilir. Red → düzelt → aşama 2'ye dön.

### §4.3 Ölçek / Standartlar

| Öğe | Değer | Sapma |
|-----|-------|-------|
| Dosya satırı (kod) | 120–260 | < 90 iskelet · > 400 forma böl (SRP) |
| Alt kontrol sayısı | ≤ 12 | > 16 → ikiye böl |
| Dışa açılan olay | ≤ 3 | > 5 → ara nesne (SRP) |
| `TabIndex` | sahip formda 0..N kesintisiz | delik = RED |
| Sabit renk/boyut | 0 (`Token.*` kullanılır) | `Color.FromArgb` = RED |
| Kodlama | UTF-8 BOM'suz | Latin-1/CP1254 otomatik RED |
| DevExpress sürümü | 26.1 | başka iddia = RED (ADR-001) |

### §4.4 Değişiklik Kontrolü

| Olay | Aksiyon | Kayıt |
|------|---------|-------|
| Sadece metin/tooltip düzeltmesi | kod içinde | `version` patch |
| Property eklendi/çıkarıldı | §3.5 tablosu + §6.5 K3 yeniden | `updated` + `log.md` satırı |
| Olay akışı değişti (§3.7) | tablo + §6.1 yeniden | `version` minor |
| Yeni alt kontrol | §3.4 ağacı + §3.3 yeniden kontrol | `updated` |
| GATE-003 tel kafesi değişti | kontrol buna uydurulur, tersi **asla** | gate kaydı |

### §4.5 Hard Guardrails (Otomatik DUR)

| # | Guardrail | İhlal Sonucu |
|---|-----------|--------------|
| G1 | CoreMusic / versacoder.ai içeriğinden cümle | **DUR** — sil + özgün yaz (envanter G1) |
| G2 | Kontrolde `EntityFramework`, `DbConnection`, SQL metni | **DUR** — erişim `l4 Infrastructure`'a taşınır |
| G3 | `partial class` yok / `InitializeComponent` çağırmıyor | **DUR** — §5.3/1 |
| G4 | `SuspendLayout`–`ResumeLayout(false)` çifti eksik | **DUR** — §5.3/2 |
| G5 | Kontrolde `SELECT *` / SQL | **DUR** — `l2` handler'a taşınır |
| G6 | Olay aboneliği `+=` ikinci kez (kontrol içi veya sahip formda) | **DUR** — çift tetiklenme, §5.3/5 |
| G7 | DataAnnotations attribute (`[Required]`, `[Range]`, `[Key]`) | **DUR** — saf kontrol kuralına çevrilir (§3.5) |
| G8 | `GATE-003` onaysız **ekran/kod** üretilmez (kök `CLAUDE.md` G8 = SSOT) — kapsam: gerçek ekran dosyası ve `src/**` altına yazılan `.cs` satırı; **bu şablon (MD) üretilmesi G8'e girmez**, giren şey onaysız ekran/koddur. `.ai/.templates/index.md` §3.4 kapıyı `form`/`ribbon` şablon üretimine de yayar (daha dar yorum) — çelişkide kök `CLAUDE.md` geçerlidir | **DUR** — `ui-design/` onayına dönülür; şablon yazımı durmaz |
| G9 | ADR'siz mimari karar / DevExpress sürümü `26.1` dışı iddia | **DUR** — ADR-001, `.ai/adr/` dışında karar belgesi yazılmaz |
| G10 | `GATE-003` onayı yokken `Controls/*.cs` kodu | **DUR** — `ui-design/`'e dön |
| G11 | Hata yolunda `catch { }` sessiz yutma | **DUR** — §3.7/6 zorunlu |
| G12 | Mojibake / karışık dil / BOM | **DUR** — UTF-8 BOM'suz yeniden yaz |

---

## §5 Workflow

### §5.1 Adım Bazlı Akış

| # | Adım | Girdi | Çıktı | Yapan | Süre |
|---|------|-------|-------|-------|------|
| 1 | Ön koşul | GATE-003 kaydı | onay satırı | ui-designer | 3 dk |
| 2 | Kimlik | ekran tanımı | §3.2 tablosu dolu | developer | 4 dk |
| 3 | Katman | §3.9 | `l7` sınırı çizimi | developer | 3 dk |
| 4 | Ağaç + property | §3.4 · §3.5 | dolu kutu (66 sütun) + tablo | developer | 8 dk |
| 5 | İskelet | §3.3 8 adım | `Designer.cs` | developer | 10 dk |
| 6 | Olay + durum | §3.6 · §3.7 | 7 kural + 8 satır kod | developer | 14 dk |
| 7 | Erişilebilirlik + DPI + tema | §3.8 | `TabIndex`/`TabStop` + `Token.*` | developer | 7 dk |
| 8 | Dispose + test | §3.9 · §3.10 | `Dispose(bool)` + 6 test | developer | 12 dk |
| 9 | Doğrulama + kabul | §5.3/1–6 · §6.1 | 0 satır · 21/21 | reviewer → orchestrator | 12 dk |
| | **Toplam/kontrol** | | | | **≈ 73 dk** |

**Zaman bütçesi:** 1–4 ≈ 18 dk · 5–7 ≈ 31 dk · 8–9 ≈ 24 dk. Kontrol **1,5 saatten uzun** yazılıyorsa §2.2'ye dön: iş kuralı veya veri erişimi kontrole sızmıştır.

### §5.2 Hata Modları

| # | Hata Modu | Belirti | Kök Neden | Düzeltme |
|---|-----------|---------|-----------|----------|
| 1 | Çift `InitializeComponent` | alt kontroller iki kez | ctor + `Load` içinde çağrı | yalnız ctor |
| 2 | Sıra bozulması | tasarım-süresi `NullReferenceException` | §3.3 adımı atlanmış | 8 adıma dön |
| 3 | Auto-property | değişiklik arayüze yansımıyor | §3.5/1 yok | alan + `OnChanged` ekle |
| 4 | Çift olay | buton iki kez çalışıyor | `+=` iki yerde | tek abonelik (G6) |
| 5 | Yarım `Dispose` | timer/GC sızıntısı | `Dispose(bool)` unutulmuş | §3.9 deseni |
| 6 | Formda/kontrolde SQL | `IDbConnection` import'u | katman karıştı | `l2`'ye taşı (G2/G5) |
| 7 | `TabIndex`/`TabStop` deliği | klavye takılıyor | elle büyük numara | 0..N yeniden numarala |
| 8 | DPI taşması | %150'de kesilen alan | sabit `Width` | `Anchor`/`Dock` + `DeviceDpi` |
| 9 | Sabit renk | temada kalıyor | `Color.FromArgb` | `Token.{{Alan}}` |
| 10 | Hata yutma | boş ekran, konsol sessiz | `catch { }` | sahip form `lblDurum` (G11) |
| 11 | Elle `Designer.cs` düzenlemesi | tasarım-süresi çökme | kod elle girilmiş | IDE üzerinden düzelt |

### §5.3 Salt Okunur Doğrulama Komutları

Altı komut da **hiçbir dosyayı değiştirmez**; `src/` yoksa dürüstçe `DIZIN YOK` yazar.

```powershell
# 1) partial class + kalitim — Control/UserControl turevi olmayan dosya 0
$src = 'C:\versacoder\src\VersaCoder.Desktop'
if (-not (Test-Path $src)) { 'DIZIN YOK -> 0 kontrol (src henuz yok)' } else {
  Get-ChildItem "$src\Controls" -Recurse -Filter '*.cs' -EA SilentlyContinue |
    Where-Object { $_.Name -notlike '*.Designer.cs' } | ForEach-Object {
      if ((Get-Content $_.FullName -Raw) -notmatch
          'partial\s+class\s+X\w+\s*:\s*(UserControl|Control|Panel|XtraScrollableControl)') {
        'EKSIK PARTIAL/KALITIM -> ' + $_.Name } } }

# 2) SuspendLayout / ResumeLayout + sira (tasarim-suresi)
Get-ChildItem $src -Recurse -Filter '*.Designer.cs' -EA SilentlyContinue | ForEach-Object {
  $raw = Get-Content $_.FullName -Raw
  if ($raw -notmatch 'SuspendLayout\s*\(\s*\)')       { 'SUSPEND YOK -> ' + $_.Name }
  if ($raw -notmatch 'ResumeLayout\s*\(\s*false\s*\)'){ 'RESUME YOK -> ' + $_.Name }
  if ($raw -notmatch 'InitializeComponent\s*\(\s*\)') { 'INIT YOK -> ' + $_.Name }
  $i = $raw.IndexOf('SuspendLayout()'); $j = $raw.IndexOf('ResumeLayout(false)')
  if ($i -ge 0 -and $j -ge 0 -and $i -gt $j) { 'SIRA BOZUK -> ' + $_.Name } }

# 3) TabIndex deligi + TabStop gozden gecirilmis mi
Get-ChildItem $src -Recurse -Filter '*.Designer.cs' -EA SilentlyContinue | ForEach-Object {
  $raw = Get-Content $_.FullName -Raw
  $n = [regex]::Matches($raw,'TabIndex\s*=\s*(\d+)') |
       ForEach-Object { [int]$_.Groups[1].Value } | Sort-Object -Unique
  if ($n -and $n[0] -ne 0) { 'TABINDEX 0 YOK -> ' + $_.Name }
  if ($n.Count -gt 1) { $g = $n[0]..$n[-1] | Where-Object { $_ -notin $n }
    if ($g) { 'TABINDEX DELIK -> {0}: {1}' -f $_.Name, ($g -join ',') } }
  if ($raw -notmatch 'TabStop') { 'TABSTOP TANIMSIZ -> ' + $_.Name } }

# 4) Yikim temizligi — Dispose(bool) + components?.Dispose + abone cozme
Get-ChildItem $src -Recurse -Filter '*.cs' -EA SilentlyContinue | ForEach-Object {
  $raw = Get-Content $_.FullName -Raw
  if ($raw -notmatch 'override\s+void\s+Dispose\s*\(\s*bool\s+disposing\s*\)') {
    'DISPOSE YOK -> ' + $_.Name }
  if ($raw -match 'new\s+Timer\s*\(' -and $raw -notmatch '\.Dispose\s*\(') {
    'TIMER SERBEST -> ' + $_.Name } }

# 5) Yasak referans: DataAnnotations / EF / DbConnection / SELECT * — 0 satir
Get-ChildItem $src -Recurse -Filter '*.cs' -EA SilentlyContinue |
  Select-String -Pattern 'DataAnnotations|EntityFramework|DbConnection|SELECT\s+\*|Color\.FromArgb' |
  ForEach-Object { '{0}:{1}  {2}' -f $_.Path, $_.LineNumber, $_.Line.Trim() }

# 6) Kutu 66 sutun (yalniz ```text bloklari) + kodlama (BOM / mojibake) — 0 satir
$box = 'C:\versacoder\.ai\.templates\frontend\component-template.md'
$inText = $false
Get-Content $box -Encoding UTF8 | ForEach-Object {
  if ($_ -match '^(`{3,})(\w*)') { if ($Matches[1].Length -eq 3) {
      $inText = ($Matches[2] -eq 'text') }; return }
  if ($inText -and $_ -match '^[|+].*[|]$' -and $_.Length -ne 66) { 'KUTU 66 DISI -> ' + $_ } }
Get-ChildItem $src -Recurse -Filter '*.cs' -EA SilentlyContinue | ForEach-Object {
  $b = [System.IO.File]::ReadAllBytes($_.FullName)
  if ($b.Length -ge 3 -and $b[0] -eq 0xEF -and $b[1] -eq 0xBB) { 'BOM VAR -> ' + $_.Name }
  if ((Get-Content $_.FullName -Raw) -match '\xC3[\x80-\xBF]\xC3[\x80-\xBF]|\uFFFD') {
    'MOJIBAKE -> ' + $_.Name } }
```

**Geçme kriteri:** #1–#6 hepsi **0 satır** (veya `src/` yoksa dürüst `DIZIN YOK`); #6 kutu ölçümü **0 uyuşmayan satır**.

### §5.4 Quality Report

```markdown
## Quality Report — component-template (YYYY-MM-DD HH:MM)

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
| K7 | Bileşen ağacı + bağlama kutuları 66 sütun | 0 uyuşmayan | §3.4, §3.6 |
| K8 | `InitializeComponent` 8 adım tablosu | 8 satır | §3.3 |
| K9 | Property/ViewState sözleşmesi 8 kural + tablo | 8 + tablo | §3.5 |
| K10 | Olay akışı 8 satır + durum matrisi | 8 + 4 | §3.7, §3.7b |
| K11 | Erişilebilirlik + DPI + tema kuralları yazılı | var | §3.8 |
| K12 | DevExpress `26.1` geçerli (ADR-001) | doğru sürüm | §3.8 |
| K13 | `§4.1–§4.5` dolu | 5/5 | §4 |
| K14 | Hard Guardrails 12 madde + G8 ayrımı | 12 + G8 | §4.5 |
| K15 | `§5.1` adım tablosu + zaman bütçesi | 9 adım + toplam | §5.1 |
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
| R3 | `partial` / `SuspendLayout` / `Dispose(bool)` eksik | **DUR** — §3.3 · §3.9 |
| R4 | Kodda `EntityFramework` geçen satır | **DUR** — ADO.NET + Dapper (`l4`) |
| R5 | `TabIndex`/`TabStop` deliği | **DUR** — 0..N yeniden numaralanır |
| R6 | Hata yolunda sessiz `catch` | **DUR** — §3.7/6 |
| R7 | GATE-003 kaydı yokken `Controls/*.cs` kodu | **DUR** — `ui-design/`'e dönülür |
| R8 | Mojibake / BOM / yasaklı vault adı | **DUR** — UTF-8 BOM'suz yeniden yazılır |
| R9 | Auto-property (`{ get; set; }`) render tarafında | **DUR** — §3.5/1 |
| R10 | Sabit `Color.FromArgb` / sabit `Width` | **DUR** — `Token.*` + `Anchor`/`Dock` |

### §6.3 Tür / Sürüm Geçiş Kuralları

| Değişiklik | Etki |
|-----------|------|
| Yalnız metin/tooltip düzeltmesi | `version` patch, `updated` değişir |
| Property eklendi/çıkarıldı | §3.5 tablosu + §6.5 K3 yeniden |
| Olay akışı değişti | §3.7 + `version` minor + inceleme |
| Yeni alt kontrol / sınıf değişti | §3.4 ağacı + §5.3/2–3 yeniden |
| DevExpress yükseltme talebi | **RED** — ADR-001, sürüm `26.1` kilitli |

### §6.4 İlgili Dosyalar

| Dosya | İlişki |
|-------|--------|
| `.ai/.templates/frontend/component-template.md` | **bu dosya** — kontrol ayağının tek kaynağı |
| `.ai/.templates/frontend/form-template.md` | sahip form; kontrolün bağlandığı ikinci ayak |
| `.ai/.templates/frontend/ribbon-template.md` | üçüncü T-FE ayağı (Part C #32) |
| `.ai/.templates/index.md` | envanter + kalite sözleşmesi §3.7 (satır 221) |
| `.ai/.templates/architecture/ascii-diagram-template.md` | kutu biçimi ve 66 sütun ölçümü |
| `.ai/.templates/adr/adr-devexpress-template.md` | §3.8 sürüm kilidi (ADR-001) |
| `.ai/.templates/backend/handler-template.md` | §3.9'daki `l2` handler'ın kaynağı |
| `.ai/.templates/testing/unit-test-template.md` | §3.10 test tablosunun biçimi |

### §6.5 Envantere Özgü Kontroller

| # | Kontrol | Beklenen | Kanıt |
|---|---------|----------|-------|
| K1 | Bu şablonun KB aralığı (envanter satır 221: `14,0`) | 27–45 | `(Get-Item …).Length / 1KB` |
| K2 | Bu şablonun satır aralığı (G6 yalnız alt sınır koyar) | 480–720 | `Get-Content … Count` |
| K3 | Kutu satırları 66 karakter (§3.4 + §3.6) | 0 uyuşmayan | §5.3/6 ölçümü |
| K4 | DevExpress `26.1` + `frontend/` 3 şablon | 3 | `Get-ChildItem .templates\frontend` |
| K5 | Yer tutucu yalnız §3.11 kalıp bloğu | 0 (diğer yer) | `Select-String '\{\{[A-Z0-9_]+\}\}'` |

---

## §7 Referanslar

### §7.1 VersaCoder Bağlantıları

| Kaynak | Yol | Ne İçin | Durum (2026-10-05) |
|--------|-----|---------|---------------------|
| Şablon envanteri | `.ai/.templates/index.md` | §3.7 sözleşmesi · satır 221 | ✅ okundu |
| Kardeş şablon (altın örnek) | `.ai/.templates/frontend/form-template.md` | biçim + §3.3 kalıbı | ✅ 590 satır · 32,6 KB |
| Biçim altın örneği | `.ai/.templates/architecture/ascii-diagram-template.md` | kutu 66 sütun | ✅ var |
| DevExpress kararı | `.ai/.templates/adr/adr-devexpress-template.md` | §3.8 sürüm kilidi | ✅ var |
| Kök SSOT (G8) | `C:\versacoder\CLAUDE.md` | GATE-003 kapsamı | ✅ var |
| Ekran onayı | `.ai/ui-design/` + `GATE-003` | ön koşul (G8, G10) | ❌ YOK → `⚠️` |
| Hedef kod | `src/VersaCoder.Desktop/Controls/*.cs` | bu şablonun çıktısı | ❌ YOK → `⚠️` |
| Yasaklı vault'lar | `C:\www\…` (envanter G1) | yalnız **yapı** referansı | 🔒 kopyalama YASAK |

### §7.2 Red Team — Sık Yapılan Hatalar

| # | Hata | Neden Yanlış | Doğrusu |
|---|------|--------------|---------|
| 1 | Kontrole iş kuralı yazmak | test edilemez, iki gerçek kaynağı | mantık `l2` handler'da (§3.9) |
| 2 | Kontrolde SQL / `EntityFramework` | katman ihlali, gizli bağımlılık | sonuç `l2`'den gelir (G2) |
| 3 | Auto-property kullanmak | değişiklik duyurusu kopar | alan + `On{{Prop}}Changed` (§3.5) |
| 4 | `InitializeComponent` sırasını değiştirmek | tasarım-süresinde kayar | §3.3 8 adım sabit |
| 5 | `Dispose(bool)` yazmamak | timer/handle sızıntısı | §3.9 deseni |
| 6 | Olayı set içinden doğrudan `Invoke` | test edilemez, tutarsız | `protected virtual On{{Olay}}` (§3.6) |
| 7 | Sahip formda her `Load`'ta `+=` | çift tetikleme | ctor'da tek abone (G6) |
| 8 | `TabIndex`/`TabStop`'u sonradan atlamak | klavye kullanılamaz | §3.8 0..N kesintisiz |
| 9 | Sabit `Width` + sabit renk | %150'de taşar, tema kırılır | `Anchor`/`Dock` + `Token.*` (§3.8) |
| 10 | DevExpress sınıfından türetmek | sürüm kilitleri kırılır | sarmalama (§3.1) |
| 11 | GATE-003'ten önce `Controls/*.cs` kodu | onaysız ekran, ~70 dk kaybı | tel kafes onayı koddan önce (G8/G10) |
| 12 | Şablonu yazmadan kontrol üretmek | ikinci gerçek kaynağı doğar | önce bu dosya (§4.1) |

**SSUT beyanı:** Bu dosya WinForms özel kontrol üretiminin **tek** kaynağıdır; her kontrol, `.ai/ui-design/` onayı ve bu şablon üzerinden üretilir. Çoğaltılan her satır ikinci gerçek kaynağı doğurur (envanter G12).
