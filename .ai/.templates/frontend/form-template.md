---
title: "VersaCoder — WinForms Form (XForm.cs) Şablonu"
type: template
category: template
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
scope: "WinForms form sablonu: XForm.cs iskeleti, kontroller ve olay baglamalari"
---

# VersaCoder — WinForms Form (XForm.cs) Şablonu

**Zorunlu Bağlantılar:** [[../index]] · [[../CLAUDE.md]] · [[../architecture/ascii-diagram-template]] · [[component-template]] · [[ribbon-template]] · [[../adr/adr-devexpress-template]]

---

## §1 Amaç

Bu şablon, VersaCoder masaüstü uygulamasındaki **her WinForms formunun tek biçimde** üretilmesi için zorunludur: kimlik, `InitializeComponent` sırası, bileşen ağacı, bağlama zinciri, saf kontrol doy­rulaması, olay akışı, erişilebilirlik ve DPI kuralları tek dosyada tanımlanır. `T-FE`'nin form ayağıdır; kontrol ayağı `[[component-template]]`, şerit ayağı `[[ribbon-template]]`'tir.

| Alan | Değer |
|------|-------|
| Template Name | `form-template.md` |
| Template Path | `.ai/.templates/frontend/form-template.md` |
| Hedef Dosya | `src/VersaCoder.Desktop/**/XForm.cs` (+ `XForm.Designer.cs`) |
| Hedef Kod | C# · `partial class` · WinForms · DevExpress WinForms **25.2.8** |
| Kalıtım | `XtraForm` / `RibbonForm` (ADR-001 kilidi) |
| Kodlama | **UTF-8 (BOM'suz)** — Latin-1/CP1254 YASAK |
| Üretilen Uzunluk | 180–320 satır (kod) + bu şablonun 480–660 satırı |
| Dil | Türkçe (ç ğ ı İ ö ş ü doğru) — sınıf/alan adı İngilizce |
| Versiyon | 1.0.0 |
| Authority | SSOT (bu dosya) |
| Bootstrap | 2026-09-25 |

### §1.1 Neden Form Şablonu Ayrı Bir Kaynak?

Form kodu en kolay yerel hâle gelen koddur: her geliştirici kendi `InitializeComponent` sırasını koyar, doğrulamayı DataAnnotations'a, veri erişimini `DbContext`'e kaçırır. Tek kaynak yoksa form bir daha taşınmaz.

| Boşluk | Belirti | Bu Şablonun Kestiği Şey |
|--------|---------|-------------------------|
| İki `InitializeComponent` sırası | tasarım-süresi kaymaları, çift abone | §3.3 sabit 8 adımlık sıra |
| Doğrulama katmanı karışması | `[Required]` attribute'u forma sızar | §3.6 **saf kontrol** kuralı |
| Veri erişimi forma girer | formda `SELECT` yazımı | §4.5 G5 — form yalnız `BindingSource` görür |
| Erişilebilirlik sona bırakılır | `TabOrder` rastgele | §3.8 zorunlu sekme haritası |
| DPI 100%’de yazılır | %150’de taşan kontroller | §3.8 DPI + DevExpress 25.2.8 notları |

### §1.2 Disk Kanıtları ve Boşluk (YAGNI)

İddia → kanıt. Kanıt yoksa uydurulmaz, `⚠️ VERIFICATION REQUIRED` yazılır. Ölçüm: `C:\versacoder`, 2026-09-25.

| # | İddia | Durum | Kanıt |
|---|-------|-------|-------|
| 1 | `.ai/.templates/frontend/` klasörü (yazım öncesi) | ❌ YOK | `Test-Path = False` — bu dosya ile açılıyor |
| 2 | `.ai/.templates/documentation/` klasörü (yazım öncesi) | ❌ YOK | `Test-Path = False` |
| 3 | `.ai/.templates/` = **23 MD · 6 klasör + 2 kök** | ✅ DOĞRULANDI | `Get-ChildItem -Recurse -Filter *.md` = 23 |
| 4 | `frontend/form-template.md` = envantere göre `PLANLI · 15,0 KB` | ✅ BU DOSYA | ölçüm §6.5 K1 |
| 5 | `src/VersaCoder.Desktop/**/XForm.cs` (hedef kod) | ❌ YOK | `src/` = `Test-Path False` |
| 6 | `.ai/ui-design/` + GATE-003 tel kafes onayı | ❌ YOK → ⚠️ | envanter G8: onaysız form üretilmez |
| 7 | `.ai/.rules/` (7 MD) · `architecture/index.md` · `WORKFLOW.md` · `keys.md` | ❌ YOK | envanter beyanı, diske işlenmemiş |
| 8 | `global.json` · `Directory.Packages.props` · `README.md` · `versacoder.db` | ❌ YOK | `Test-Path = False` |
| 9 | DevExpress sürümü **25.2.8** (ADR-001) | ⚠️ VERIFICATION REQUIRED | `.ai/adr/` dizini diskte YOK |
| 10 | Harici referans vault'ları (envanter G1) | 🔒 ERİŞİM YASAK | yalnız yapı referansı; tek kelime kopyalanmadı |

**Yorum yasağı:** Var olmayan dosya için "şurada tanımlı" denmez; `⚠️ VERIFICATION REQUIRED` yazılır, satır olduğu yerde durur. §1.2'deki ❌ satırları §3.9'daki örneklerin **tasarım** olduğunu, diskte henüz form kodu bulunmadığını hatırlatır.

### §1.3 Form Ne Değildir?

| Formdur | Form Değildir |
|---------|---------------|
| Ekranın **görsel iskeleti** + olay bağları | İş kuralı (→ Application use case) |
| `BindingSource` üzerinden veri **görüntüleme** | SQL / Dapper sorgusu (→ Infrastructure) |
| Saf kontrol bazlı girdi doğrulama | Kalıcı doğrulama kuralları (→ Domain invariant) |
| Ribbon menüsünün **taşıyıcısı** | Ribbon sayfa tanımı (→ `[[ribbon-template]]`) |
| Kullanıcı niyetini handler'a **ileten** | Handler'ın kendisi (→ Application handler) |

### §1.4 Form Yaşam Döngüsü

```text
+----------------------------------------------------------------+
| [1] new XForm()          alan başlatıcı, tasarım yok           |
|    v                                                           |
| [2] InitializeComponent  kontroller kurulur (§3.3)             |
|    v                                                           |
| [3] BindingSource bağ   veri zinciri kurulur (§3.5)            |
|    v                                                           |
| [4] Form_Load            tek seferlik kaynak okuma             |
|    v                                                           |
| [5] Kullanıcı olayı      handler → Application use case        |
|    v                                                           |
| [6] Kaydet / İptal       Flush → Close → Dispose               |
|    ← geri dönüş: hata durumunda [5]'e, ekran kapanmaz          |
+----------------------------------------------------------------+
```

**Ne anlatır:** Formun kurulum→kullanım→kapanış döngüsü; hata durumunda akış `[6]`'ya değil `[5]`'e geri döner (§3.7).

---

## §2 Kapsam

**İçinde:** form sınıfı yapısı, `InitializeComponent` sırası, bileşen ağacı, veri bağlama, saf kontrol doy­rulaması, olay akışı, hata durumları, erişilebilirlik, yüksek DPI ve salt-okunur doğrulama.
**Dışında:** özel kontrol kodu, ribbon sayfa tanımı, tel kafes onayı, iş kuralı, SQL, paket sürümü kararı.

| Kapsam | Kapsam Dışı |
|--------|-------------|
| `XForm.cs` + `XForm.Designer.cs` ikilisi | Kullanıcı sözleşmesi (→ `REQUIREMENTS.md`) |
| `partial class` · `SuspendLayout` · `Dispose` | `BindingSource` altındaki sorgu (→ Dapper) |
| Bileşen ağacı + `TabIndex` haritası | Ribbon sayfa/grup/öğe tanımı (→ ribbon) |
| Saf kontrol doy­rulama + hata mesajı | Kalıcı kural (→ `.ai/.rules/`) |
| Olay akışı tablosu + hata durumları | Sürüm kilidi kararı (→ ADR-001) |
| DPI + DevExpress 25.2.8 davranış notları | Şema/migration (→ ADR-004) |

### §2.1 Dosya Tipi → Sorumlu Eşlemesi

| Dosya Tipi | Sorumlu | Bu Şablondan mı |
|------------|---------|------------------|
| `src/VersaCoder.Desktop/**/XForm.cs` (ana ekran formu) | developer | ✅ |
| `src/VersaCoder.Desktop/**/XDialog.cs` (modal diyalog) | developer | ✅ |
| `src/VersaCoder.Desktop/Controls/*.cs` (özel kontrol) | developer | ❌ → `[[component-template]]` |
| `src/VersaCoder.Desktop/**/XRibbonPage.cs` (şerit sayfası) | developer | ❌ → `[[ribbon-template]]` |
| `.ai/ui-design/01-…07-*.md` (ekran tanımı) | ui-designer | ❌ ön koşul, GATE-003 |

### §2.2 Kapsam Sınırı Testi

Üretilecek `.cs` dosyasına sırayla sor; **ilk `EVET`** şablonu belirler:

1. **`Form` türevi mi** (`XtraForm`, `RibbonForm`)? → **bu şablon**.
2. **`Control` türevi mi ama `Form` değil mi?** → `[[component-template]]`.
3. **`XRibbonPage` / `XRibbonPageGroup` barındırıyor mu?** → `[[ribbon-template]]`.
4. **Yalnız veri erişimi mi içeriyor** (SQL, Dapper)? → `backend/repository-template`.
5. **Yalnız iş kuralı mı içeriyor** (use case)? → `backend/service-template`.
6. **Hiçbiri değilse** → şablon yoktur; talep `index.md` §5.1 Adım 1'den açılır.

`HAYIR` soruları: CoreMusic/versacoder.ai içeriği mi (G1)? · `EntityFramework` geçiyor mu (G2)? · `[Required]`/DataAnnotations var mı (G7)? · `GATE-003` onayı alınmış mı (G10)?

---

## §3 Mimari

Bu bölüm hem **şablonun kendi yapısını** hem **üretilecek formun yapısını** tanımlar.

### §3.1 Dosya ve Sınıf Sözleşmesi

| Öğe | Kural | Doğru | Yanlış |
|-----|-------|-------|--------|
| Dosya ikilisi | `XForm.cs` + `XForm.Designer.cs` | ikisi de var | tek dosya (tasarım-süresi kaybı) |
| Sınıf bildirimi | `public partial class XForm : XtraForm` | `partial` zorunlu | `partial`'siz |
| `Designer.cs` sahipliği | yalnız Visual Studio/tasarım-süresi yazar | elle düzenleme yasak | elle `Edit & Continue` |
| Adlandırma | `<EkranAdıForm>` — `-Form` eki zorunlu | `SettingsForm` | `Settings`, `frmSettings` |
| Dosya yolu | `src/VersaCoder.Desktop/<Alan>/<Ad>Form.cs` | kökten yazılır | dağınık kök klasör |
| TFM | `net48;net10.0-windows` ikilisinde derlenir | `#if NET48` farkı bilinçli | tek TFM'e göre yazım |

### §3.2 Form Kimliği ve Amacı

Her form için şu alanlar **tek satırda** doldurulur; boş hücre kabul edilmez:

| Alan | Açıklama | Zorunlu Örnek Kalıp |
|------|----------|----------------------|
| `FormAdı` | sınıf adı | `{{EkranAdi}}Form` |
| `Dosya` | kökten yol | `src/VersaCoder.Desktop/{{Alan}}/{{EkranAdi}}Form.cs` |
| `Amaç` | tek cümle, "ne yapar" | `Kullanıcı {{AMAC}} işlemini {{SONUC}} ile bitirir.` |
| `Bağlı Use Case` | Application katmanı | `{{UseCaseAdi}}Handler` |
| `BindingSource` | ana kaynak | `bs{{VeriAdi}}` |
| `Onay Kapısı` | kaydetmeden önce | `GATE-003` tel kafesi ile birebir eşleşme |

### §3.3 `InitializeComponent` Yapısı (8 Sabit Adım)

Sıra **değiştirilemez**; değiştirilen form tasarım-süresinde kayar ve denetim komutu (§5.3/2) sırayı doğrulayamaz.

| # | Adım | Ne Yapılır | İhlal Belirtisi |
|---|------|-----------|-----------------|
| 1 | `SuspendLayout()` | ölçüm olayları bastırılır | arkada titreşim |
| 2 | `resources.ApplyResources(...)` | yerelleştirme/boyut | metin kayması |
| 3 | Kontrolleri **dip→yukarı** oluştur | iç kontroller önce | `NullReferenceException` |
| 4 | `((ISupportInitialize)(...)).BeginInit()` | Grid/Ribbon gibi | tasarım-süresi çökme |
| 5 | Özellikleri ata | `Name`, `TabIndex`, `Dock` | sekme sırası bozuk |
| 6 | Olaylara **tek seferde** abone | `Click += ...` | çift tetikleme |
| 7 | `Controls.Add(...)` zinciri | ebeveyn→çocuk | kontrol görünmez |
| 8 | `ResumeLayout(false)` + `PerformLayout()` | ölçüm yeniden açılır | DPI taşması |

**Yasak:** 6. adımda abone edilen her olay için `-= null` türü temizlik; abonelik `Dispose`'a aittir (§4.5 G6).

### §3.4 Bileşen Ağacı (ASCII kutu — 66 sütun)

```text
+----------------------------------------------------------------+
| {{FormSinifi}} : XtraForm                                      |
| +-- {{Menu_Ogesi}}       XRibbonForm menü öğesi                |
| |   +-- {{Buton_1}}      Click → {{UseCase}}Handler            |
| |   +-- {{Buton_2}}      Ctrl+{{X}}, BeginInvoke               |
| +-- {{AnaPanel}}         XtraScrollableControl (Dock=Fill)     |
| |   +-- {{Grup_1}}       GroupControl                          |
| |   |   +-- {{Alan_1}}   TextEdit     Bind: {{Alan}}           |
| |   |   +-- {{Alan_2}}   ComboBoxEdit Bind: {{Durum}}          |
| |   +-- {{Tablo_1}}      GridControl ← bs{{Veri}}              |
| |       +-- {{Kolonlar}} GridView cols (OptionsView)           |
| +-- {{Dugmeler}}         SimpleButton Kaydet | İptal           |
| +-- {{Durum}}            lblDurum (sağ hizalı, AutoSize)       |
+----------------------------------------------------------------+
```

**Ne anlatır:** Bir formun izin verilen bileşen kümesi ve hiyerarşisi; her satır `{{…}}` yerine gerçek kontrol adıyla doldurulur.
**Doğrulama:** §5.3/1 — kutu satırları tam 66 karakter olmalı.

### §3.5 Veri Bağlama Zinciri (BindingSource)

Form **asla** repository/DB görmez; gördüğü tek şey `BindingSource`'tur. Zincir tek yönlüdür:

```text
+----------------------------------------------------------------+
| Application  {{UseCase}}Handler  →  result (nesne/liste)       |
|    v                                                           |
| Desktop      bs{{Veri}}.DataSource = result                    |
|    v                                                           |
| Kontrol      {{Alan_1}}.EditValue  ⇄  bs.{{Alan}} (DataMember) |
|    v                                                           |
| Kullanıcı    Kaydet → bs.EndEdit() → Flush → Handler           |
|    v                                                           |
| Hata         handler failure → lblDurum, ekran KAPANMAZ        |
+----------------------------------------------------------------+
```

| Kural | Doğru | Yanlış |
|-------|-------|--------|
| Kaynak | `bs.DataSource` tek atama noktası | her kontrolde ayrı doldurma |
| Yazma | `bs.EndEdit()` **önce**, handler **sonra** | handler içinde `EditValue` okumak |
| Yenileme | `bs.ResetBindings(false)` | `Invalidate` ile umut etmek |
| Sıfır-kayıt | `lblDurum` + Kaydet `Enabled=false` | sessizce hiçbir şey yapmamak |
| İlişkili liste | `BindingList<T>` | `DataTable` (ADR-004'te yok) |

### §3.6 Doğrulama Kuralları — Saf Kontrol (DataAnnotations YASAK)

Form doğrulaması **kontrolün kendisinde** yaşar. `[Required]`, `[Range]` vb. attribute'lar forma **girmez** (§4.5 G7): formda model yoktur, kalıcı kural ise Domain'dedir.

| # | Kural | Nerede | Hata Gösterimi | Bloke Eder |
|---|-------|--------|----------------|-----------|
| 1 | Zorunlu alan boş mu | `EditValue == null \|\| IsNullOrWhiteSpace` | `lbl{{Alan}}Error` görünür | Kaydet |
| 2 | Uzunluk sınırı | `text.Length > {{N}}` → kırp + uyarı | gri `lblDurum` metni | Kaydet |
| 3 | Sayı aralığı | `decimal.TryParse` + `min..max` | alanda `ErrorIcon` | Kaydet |
| 4 | Tarih mantığı | `bitiş >= başlangıç` | kırmızı `lbl{{Tarih}}Error` | Kaydet |
| 5 | Kopya anahtar | handler `failure(duplicate)` | modal mesaj + odak | — (sunucu tarafı) |
| 6 | Onay | silme/geri alırdan önce `XtraMessageBox` | `YesNo` | işlem |

**Sıra:** 1–4 **anlık** (`Validating`), 5 **handler** içinde, 6 **aksiyon öncesi**. Form asla "hepsi geçti" diye **varsaymaz**; handler'ın `failure`'ı her zaman beklenir (§3.7 satır 6).

### §3.7 Olay Akışı ve Hata Durumları

| # | Olay | Tetikleyen | Handler / Akış | Başarı | Hata Durumu |
|---|------|-----------|----------------|--------|-------------|
| 1 | `Form_Load` | ilk açılış | use case `HandleAsync` | `bs` dolar | `lblDurum` = hata, form açık kalır |
| 2 | `Validating` | alan odak kaybı | §3.6 kuralları 1–4 | odak ilerler | `CancelEventArgs.Cancel=true` |
| 3 | `Click` (Kaydet) | buton | `EndEdit` → handler | `lblDurum` = OK | mesaj + odak hatalı alana |
| 4 | `Click` (İptal) | buton | `bs.CancelEdit` → `Close` | kapanır | değişiklik varsa onay |
| 5 | `CellDoubleClick` | grid | ayrıntı formu modal | yeni form | — |
| 6 | handler `failure` | Application | hata kodu → metin | — | ekran **kapanmaz** |
| 7 | `FormClosing` | X / ESC | kirli `bs` var mı → onay | kapanır | `e.Cancel = true` |
| 8 | `Dispose(true)` | `Close` sonrası | `bs.Dispose`, olay `-= ` | temiz | `ObjectDisposedException` riski |

**Kural:** 6. satır **her zaman** yazılır; başarı yolu asla tek yol olarak kodlanmaz.

### §3.8 Erişilebilirlik, Sekme Sırası ve Yüksek DPI

| Alan | Kural | Denetim |
|------|-------|---------|
| `TabOrder` | `TabIndex` 0..N **kesintisiz** ve okuma sırasıyla eş | §5.3/3 |
| `AccessKey` | `&` öneki (`&Kaydet`) + `ShowAccelerators` | elle denetim |
| Odak | form açılınca ilk anlamlı alana `Focus()` | gözlem |
| Klavye | `Enter` → bir sonraki alan; `Esc` → İptal | §6.5 K3 |
| Renk-körü | durumu **yazıyla** da ver (`HATALI`), yalnız renkle değil | gözlem |
| DPI 100–200% | `Anchor`/`Dock` ile akış; sabit `Width` yasak | §5.3/4 |
| DevExpress 25.2.8 | `LookAndFeel` tek noktadan; sürüm **yükseltilmez** (ADR-001) | §6.5 K4 |

**DPI notu (25.2.8):** `DpiChanged`'de **kontrol boyutu elle ayarlanmaz**; `AutoScaleDimensions` tasarlandığı DPI'a sabit kalır, `AutoScaleMode.Dpi` devreye girer. `net48` tarafında DPI-işaretleme anahtarı **açılmaz**; ikili TFM'deki davranış farkı `#if NET48` ile belgelenir.

### §3.9 Dolu VersaCoder Örneği ve Tam Kalıp

**Dolu örnek (gerçek VersaCoder verisiyle — `SettingsForm`):**

```text
+----------------------------------------------------------------+
| SettingsForm : RibbonForm                                      |
| +-- mnuSettings        RibbonPageItem → SettingsPage           |
| +-- grpConnection      GroupControl                            |
| |   +-- txtDbPath       ButtonEdit   Bind: DbPath              |
| |   +-- cmbProvider     ComboBoxEdit Bind: Provider            |
| |   +-- lblError        ErrorProvider hedefi (§3.6/1)          |
| +-- grpLimits          GroupControl                            |
| |   +-- spnBudget       SpinEdit     Bind: TokenBudget         |
| |   +-- spnCoverage     SpinEdit     Bind: Coverage (0..100)   |
| +-- btnSave | btnCancel SimpleButton  tabIndex 10 | 11         |
| +-- lblDurum           StatusBar, sağ hizalı                   |
+----------------------------------------------------------------+
```

**Tam kalıp (yalnız bu blokta `{{…}}` yaşar; üretilen kodda adedi 0 olmalıdır):**

````markdown
```csharp
public partial class {{EkranAdi}}Form : RibbonForm
{
    private readonly I{{UseCase}}Handler _handler;

    public {{EkranAdi}}Form(I{{UseCase}}Handler handler)
    {
        _handler = handler;                 // bağımlılık DI'dan gelir
        InitializeComponent();              // §3.3 sırası değişmez
        AutoScaleMode = AutoScaleMode.Dpi;  // §3.8
    }

    private async void OnSave(object s, EventArgs e)
    {
        bs{{Veri}}.EndEdit();               // §3.5 zinciri
        if (!ValidateChildren()) return;    // §3.6 saf kontrol
        var r = await _handler.HandleAsync(new {{Komut}}(...));
        if (r.IsFailure) { lblDurum.Text = r.Error; return; }  // §3.7/6
        lblDurum.Text = "OK";
    }
}
```
````

---

## §4 Kurallar

### §4.1 Yazım Sırası ve Öncelik

| Öncelik | Şart | Yapılacak |
|---------|------|-----------|
| 1 | GATE-003 tel kafesi onayı yoksa | **DUR** — form üretilmez (§4.5 G10) |
| 2 | Ekran tanımı varsa | §3.2 kimlik satırını doldur |
| 3 | Bileşen ağacı | §3.4 kalıbını çiz, adlandır |
| 4 | `InitializeComponent` | §3.3 8 adım, değiştirilemez sıra |
| 5 | Bağlama | §3.5 — form repository görmez |
| 6 | Doğrulama | §3.6 — saf kontrol, attribute yasak |
| 7 | Olay + hata | §3.7 — 8 satırın hepsi dolu |
| 8 | Erişilebilirlik + DPI | §3.8 — `TabIndex` kesintisiz |
| 9 | Doğrulama | §5.3 komutları → 0 satır |

### §4.2 Sorumluluk ve Onay

| Aşama | Rol | Ne Onaylar | Çıktı | Süre |
|-------|-----|-----------|-------|------|
| 1 | ui-designer | tel kafes ↔ §3.2 amacı | GATE-003 onayı | 10 dk |
| 2 | developer | §3.1–§3.9 kodu | `XForm.cs` + `Designer.cs` | 40 dk |
| 3 | reviewer | §6.1 (21 madde) | ✅ / RED | 12 dk |
| 4 | architect | katman: form → Application bağı | ters referans yok | 8 dk |
| 5 | master-orchestrator | §5.3 çıktısı + §5.4 | Quality Report | 5 dk |

> **3 RED** sonrası iş durur; gerekçe `master-orchestrator`'a yükseltilir. Red → düzelt → aşama 2'ye dön.

### §4.3 Ölçek / Standartlar

| Öğe | Değer | Sapma |
|-----|-------|-------|
| Dosya satırı (kod) | 180–320 | < 100 iskelet · > 450 dokümana dönüşür |
| Kontrol sayısı | ≤ 40 | > 55 → forma böl |
| `TabIndex` aralığı | 0..N kesintisiz | delik = RED |
| Kodlama | UTF-8 BOM'suz | Latin-1/CP1254 otomatik RED |
| DevExpress sürümü | 25.2.8 | başka iddia = RED (ADR-001) |

### §4.4 Değişiklik Kontrolü

| Olay | Aksiyon | Kayıt |
|------|---------|-------|
| Sadece metin/tooltip düzeltmesi | kod içinde | `version` patch |
| Kontrol eklendi/çıkarıldı | §3.4 ağacı + §3.3 yeniden kontrol | `updated` + `log.md` satırı |
| Olay akışı değişti (§3.7) | tablo + §6.5 yeniden | `version` minor |
| GATE-003 tel kafesi değişti | form buna uydurulur, tersi **asla** | gate kaydı |

### §4.5 Hard Guardrails (Otomatik DUR)

| # | Guardrail | İhlal Sonucu |
|---|-----------|--------------|
| G1 | CoreMusic / versacoder.ai içeriğinden cümle | **DUR** — sil + özgün yaz (envanter G1) |
| G2 | Formda SQL, Dapper, `versacoder.db` erişimi | **DUR** — erişim Infrastructure'a taşınır |
| G3 | `partial class` yok / `InitializeComponent` çağırmıyor | **DUR** — §5.3/1 |
| G4 | `SuspendLayout`–`ResumeLayout` çifti eksik | **DUR** — §5.3/2 |
| G5 | Formda `SELECT`/`INSERT` metni | **DUR** — handler'a taşınır |
| G6 | Olay aboneliği `+=` ikinci kez | **DUR** — çift tetiklenme |
| G7 | DataAnnotations attribute (`[Required]`, `[Range]`) | **DUR** — saf kontrole çevrilir (§3.6) |
| G8 | `TabOrder` deliği / odak sırası bozuk | **DUR** — §3.8 |
| G9 | DevExpress sürümü `25.2.8` dışında iddia | **DUR** — ADR-001 |
| G10 | GATE-003 onayı olmadan form kodu | **DUR** — `ui-design/`'e dön |
| G11 | Hata yolunda `catch { }` sessiz yutma | **DUR** — §3.7/6 zorunlu |
| G12 | Mojibake / karışık dil / emoji (kutu dışında yasaklı listeler hariç) | **DUR** — UTF-8 BOM'suz yeniden yaz |

---

## §5 Workflow

### §5.1 Adım Bazlı Akış

| # | Adım | Girdi | Çıktı | Yapan | Süre |
|---|------|-------|-------|-------|------|
| 1 | Ön koşul | GATE-003 kaydı | onay satırı | ui-designer | 3 dk |
| 2 | Kimlik | ekran tanımı | §3.2 tablosu dolu | developer | 5 dk |
| 3 | Ağaç | §3.4 kalıbı | dolu kutu (66 sütun) | developer | 8 dk |
| 4 | İskelet | §3.3 8 adım | `Designer.cs` | developer | 15 dk |
| 5 | Bağlama | use case çıktısı | `BindingSource` zinciri | developer | 10 dk |
| 6 | Doğrulama + olay | §3.6 · §3.7 | 6 kural + 8 satır kod | developer | 22 dk |
| 7 | Erişilebilirlik + DPI | §3.8 | `TabIndex` + `AutoScale` | developer | 7 dk |
| 8 | Doğrulama + kabul | §5.3/1–6 · §6.1 | 0 satır · 21/21 | reviewer → orchestrator | 15 dk |
| | **Toplam/form** | | | | **≈ 85 dk** |

**Zaman bütçesi:** 1–4 ≈ 31 dk · 5–7 ≈ 39 dk · 8 ≈ 15 dk. Form **2 saatten uzun** yazılıyorsa §2.2'ye dön: iş kuralı veya veri erişimi forma sızmıştır.

### §5.2 Hata Modları

| # | Hata Modu | Belirti | Kök Neden | Düzeltme |
|---|-----------|---------|-----------|----------|
| 1 | Çift `InitializeComponent` | kontroller iki kez görünür | ctor + `Load` içinde çağrı | yalnız ctor |
| 2 | Sıra bozulması | `NullReferenceException` (tasarım) | §3.3 adımı atlanmış | 8 adıma dön |
| 3 | Çift olay | buton iki kez çalışıyor | `+=` iki yerde | tek abonelik |
| 4 | Formda SQL | `IDbConnection` import'u | katman karıştı | handler'a taşı (G2/G5) |
| 5 | TabOrder deliği | klavye takılıyor | `TabIndex` elle büyük yazılmış | 0..N yeniden numarala |
| 6 | DPI taşması | %150'de kesilen alan | sabit `Width` | `Anchor`/`Dock` |
| 7 | Hata yutma | boş ekran, konsol sessiz | `catch { }` | `lblDurum` + kod |
| 8 | Elle `Designer.cs` düzenlemesi | tasarım-süresi çökme | kod elle girilmiş | IDE üzerinden düzelt |

### §5.3 Salt Okunur Doğrulama Komutları

Altı komut da **hiçbir dosyayı değiştirmez**; `src/` yoksa dürüstçe `DIZIN YOK` yazar.

```powershell
# 1) partial class + Form kalitimi — eksik dosya 0
$src = 'C:\versacoder\src\VersaCoder.Desktop'
if (-not (Test-Path $src)) { 'DIZIN YOK -> 0 form (src henuz yok)' } else {
  Get-ChildItem $src -Recurse -Filter '*Form.cs' | Where-Object {
    $_.Name -notlike '*.Designer.cs' } | ForEach-Object {
      if ((Get-Content $_.FullName -Raw) -notmatch
          'partial\s+class\s+\w+Form\s*:\s*(XtraForm|RibbonForm)') {
        'EKSIK PARTIAL/KALITIM -> ' + $_.Name } } }

# 2) SuspendLayout / ResumeLayout + sira
Get-ChildItem $src -Recurse -Filter '*.Designer.cs' -EA SilentlyContinue | ForEach-Object {
  $raw = Get-Content $_.FullName -Raw
  if ($raw -notmatch 'SuspendLayout\s*\(\s*\)')   { 'SUSPEND YOK -> ' + $_.Name }
  if ($raw -notmatch 'ResumeLayout\s*\(\s*false\s*\)') { 'RESUME YOK -> ' + $_.Name }
  $i = $raw.IndexOf('SuspendLayout()'); $j = $raw.IndexOf('ResumeLayout(false)')
  if ($i -ge 0 -and $j -ge 0 -and $i -gt $j) { 'SIRA BOZUK -> ' + $_.Name } }

# 3) TabIndex deligi — 0'dan baslamayan / atlanan deger listelenir
Get-ChildItem $src -Recurse -Filter '*.Designer.cs' -EA SilentlyContinue | ForEach-Object {
  $n = [regex]::Matches((Get-Content $_.FullName -Raw),'TabIndex\s*=\s*(\d+)') |
       ForEach-Object { [int]$_.Groups[1].Value } | Sort-Object -Unique
  if ($n -and $n[0] -ne 0) { 'TABINDEX 0 YOK -> ' + $_.Name }
  if ($n.Count -gt 1) { $g = $n[0]..$n[-1] | Where-Object { $_ -notin $n }
    if ($g) { 'TABINDEX DELIK -> {0}: {1}' -f $_.Name, ($g -join ',') } } }

# 4) DPI / AutoScaleMode — eksik form 0
Get-ChildItem $src -Recurse -Filter '*.Designer.cs' -EA SilentlyContinue | ForEach-Object {
  if ((Get-Content $_.FullName -Raw) -notmatch 'AutoScaleMode') { 'AUTOSCALE YOK -> ' + $_.Name } }

# 5) Yasak referans: DataAnnotations / EF / form ici SQL — 0 satir
Get-ChildItem $src -Recurse -Filter '*.cs' -EA SilentlyContinue |
  Select-String -Pattern 'DataAnnotations|EntityFramework|DbConnection|SELECT\s+\*' |
  ForEach-Object { '{0}:{1}  {2}' -f $_.Path, $_.LineNumber, $_.Line.Trim() }

# 6) Kodlama — BOM + gercek mojibake 0
Get-ChildItem $src -Recurse -Filter '*.cs' -EA SilentlyContinue | ForEach-Object {
  $b = [System.IO.File]::ReadAllBytes($_.FullName)
  if ($b.Length -ge 3 -and $b[0] -eq 0xEF -and $b[1] -eq 0xBB) { 'BOM VAR -> ' + $_.Name }
  if ((Get-Content $_.FullName -Raw) -match '\xC3[\x80-\xBF]\xC3[\x80-\xBF]|\uFFFD') {
    'MOJIBAKE -> ' + $_.Name } }
```

**Geçme kriteri:** #1–#6 hepsi **0 satır** (veya `src/` yoksa dürüst `DIZIN YOK`); §5.3/1 kutu ölçümü **0 uyuşmayan satır**.

### §5.4 Quality Report

```markdown
## Quality Report — form-template (YYYY-MM-DD HH:MM)

| Metrik | Değer | Eşik | Durum |
|---|---|---|---|
| Ölçüm (şablon) | <satır> satır / <KB> KB | 480–660 · 25–34 | 🟢/🔴 |
| Checklist §6.1 | n/21 | 21/21 | 🟢/🔴 |
| Komut §5.3 (#1–#6) | n satır çıktı | 0 | 🟢/🔴 |
| Kutu satırı 66 sütun | n/66 | 0 uyuşmayan | 🟢/🔴 |
| Yer tutucu / mojibake / yasaklı ad | n/n/n | 0/0/0 | 🟢/🔴 |
| Guardrail G1..G12 | ihlal n | 0 | 🟢/🔴 |
| GATE-003 | kayıt var mı | var | 🟢/🔴 |

**Karar:** 🟢 ONAYLANDI / 🔴 RED (<gerekçe>)
```

---

## §6 Doğrulama

### §6.1 Kontrol Listesi (21 madde)

| # | Kontrol | Beklenen | İlgili |
|---|---------|----------|--------|
| K1 | Frontmatter 7 alan + `authority: SSOT` | 7/7 · `SSOT` | §5.3 |
| K2 | H1 + `**Zorunlu Bağlantılar:**` (6 bağlantı) | 1 satır | üst kısım |
| K3 | `§1.1–§1.4` dolu | 4/4 | §1 |
| K4 | §1.2 disk kanıtları + `⚠️` + yorum yasağı | var | §1.2 |
| K5 | `§2.1–§2.2` dolu | 2/2 | §2 |
| K6 | `§3.1–§3.9` + dolu VersaCoder örneği | 9/9 | §3 |
| K7 | Bileşen ağacı + bağlama kutuları 66 sütun | 0 uyuşmayan | §3.4, §3.5 |
| K8 | `InitializeComponent` 8 adım tablosu | 8 satır | §3.3 |
| K9 | Doğrulama 6 kural, DataAnnotations yasak | 6 + yasak | §3.6 |
| K10 | Olay akışı 8 satır dolu | 8 | §3.7 |
| K11 | Erişilebilirlik + DPI kuralları yazılı | var | §3.8 |
| K12 | DevExpress `25.2.8` geçerli (ADR-001) | doğru sürüm | §3.8 |
| K13 | `§4.1–§4.5` dolu | 5/5 | §4 |
| K14 | Hard Guardrails ≥ 10 madde | 12 | §4.5 |
| K15 | `§5.1` adım tablosu + zaman bütçesi | 8 adım + toplam | §5.1 |
| K16 | `§5.2` ≥ 8 hata modu | 8 | §5.2 |
| K17 | `§5.3` ≥ 5 salt-okunur komut | 6 | §5.3 |
| K18 | `§5.4` Quality Report bloğu | var | §5.4 |
| K19 | `§6.1` ≥ 20 madde | 21 | bu tablo |
| K20 | `§6.2–§6.5` dolu | 4/4 | §6 |
| K21 | `§7.2` ≥ 10 satır · yer tutucu/mojibake | ≥ 10 · 0/0 | §7 · G4 |

### §6.2 Red Sinyalleri (Otomatik DUR)

| # | Sinyal | Aksiyon |
|---|--------|---------|
| R1 | §5.3 komutlarından biri > 0 satır | **DUR** — gösterilen dosya düzeltilir |
| R2 | §3.4/§3.5 kutusunda satır ≠ 66 | **DUR** — ölçümle hizalanır |
| R3 | `partial` / `SuspendLayout` eksik | **DUR** — §3.3 |
| R4 | Formda `EntityFramework` geçen satır | **DUR** — ADO.NET + Dapper'e taşınır |
| R5 | `TabIndex` deliği | **DUR** — 0..N yeniden numaralanır |
| R6 | Hata yolunda sessiz `catch` | **DUR** — §3.7/6 |
| R7 | GATE-003 kaydı yokken kod | **DUR** — `ui-design/`'e dönülür |
| R8 | Mojibake / BOM / yasaklı vault adı | **DUR** — UTF-8 BOM'suz yeniden yazılır |

### §6.3 Tür / Sürüm Geçiş Kuralları

| Değişiklik | Etki |
|-----------|------|
| Yalnız metin/tooltip düzeltmesi | `version` patch, `updated` değişir |
| Kontrol eklendi/çıkarıldı | §3.4 ağacı + §6.5 K3 yeniden |
| Olay akışı değişti | §3.7 + `version` minor + inceleme |
| DevExpress yükseltme talebi | **RED** — ADR-001, sürüm `25.2.8` kilitli |

### §6.4 İlgili Dosyalar

| Dosya | İlişki |
|-------|--------|
| `.ai/.templates/frontend/form-template.md` | **bu dosya** — form ayağının tek kaynağı |
| `.ai/.templates/frontend/component-template.md` | özel kontrol (formun çocuğu olabilir) |
| `.ai/.templates/frontend/ribbon-template.md` | formun taşıdığı şerit sayfası |
| `.ai/.templates/index.md` | envanter + kalite sözleşmesi §3.7 (satır 219) |
| `.ai/.templates/architecture/ascii-diagram-template.md` | kutu biçimi ve 66 sütun ölçümü |

### §6.5 Envantere Özgü Kontroller

| # | Kontrol | Beklenen | Kanıt |
|---|---------|----------|-------|
| K1 | Bu şablonun KB aralığı (envanter satır 219: `15,0`) | 25–34 | `(Get-Item …).Length / 1KB` |
| K2 | Bu şablonun satır aralığı | 480–660 | `Get-Content … Count` |
| K3 | Kutu satırları 66 karakter (§3.4 + §3.5) | 0 uyuşmayan | §5.3/1 benzeri ölçüm |
| K4 | Yer tutucu yalnız kalıp bloğu | 0 (diğer yer) | `Select-String '\{\{[A-Z0-9_]+\}\}'` |
| K5 | `frontend/` klasöründe 3 şablon | 3 | `Get-ChildItem .templates\frontend` |

---

## §7 Referanslar

### §7.1 VersaCoder Bağlantıları

| Kaynak | Yol | Ne İçin | Durum (2026-09-25) |
|--------|-----|---------|---------------------|
| Şablon envanteri | `.ai/.templates/index.md` | §3.7 sözleşmesi · satır 219 | ✅ 39,5 KB |
| Biçim altın örneği | `.ai/.templates/architecture/ascii-diagram-template.md` | kutu 66 sütun | ✅ 37,2 KB / 644 satır |
| DevExpress kararı | `.ai/.templates/adr/adr-devexpress-template.md` | §3.8 sürüm kilidi | ✅ 21,4 KB |
| Ekran onayı | `.ai/ui-design/` + `GATE-003` | ön koşul (G10) | ❌ YOK → `⚠️` |
| Yasaklı vault'lar | `C:\www\…` (envanter G1) | yalnız **yapı** referansı | 🔒 kopyalama YASAK |

### §7.2 Red Team — Sık Yapılan Hatalar

| # | Hata | Neden Yanlış | Doğrusu |
|---|------|--------------|---------|
| 1 | Formda iş kuralı yazmak | iki gerçek kaynağı, test edilemez | mantık Application handler'da (§2) |
| 2 | Formda SQL/Dapper | katman ihlali, gizli bağımlılık | form yalnız `BindingSource` görür (§3.5) |
| 3 | `[Required]` attribute koymak | formda model yok, çalışmaz | saf kontrol + hata etiketi (§3.6) |
| 4 | `InitializeComponent` sırasını değiştirmek | tasarım-süresinde kayar | §3.3 8 adım sabit |
| 5 | `Designer.cs`'i elle düzenlemek | tasarımcı çöker | yalnız IDE ile |
| 6 | Hata yolunu `catch {}` ile kapatmak | kullanıcı ne olduğunu görmez | §3.7/6 `lblDurum` + kod |
| 7 | `TabOrder`'ı sonradan atlamak | klavye kullanılamaz | §3.8 0..N kesintisiz |
| 8 | Sabit `Width` ile DPI'a yazılmak | %150'de taşar | `Anchor`/`Dock` + `AutoScaleMode.Dpi` |
| 9 | DevExpress sürümünü güncellemek | ADR-001 ihlali | `25.2.8` sabit, yükseltme YASAK |
| 10 | GATE-003'ten önce kod yazmak | onaysız ekran, 120 dk kaybı | tel kafes onayı koddan önce (G10) |

**SSUT beyanı:** Bu dosya WinForms form üretiminin **tek** kaynağıdır; her form, `.ai/ui-design/` onayı ve bu şablon üzerinden üretilir. Çoğaltılan her satır ikinci gerçek kaynağı doğurur (envanter G12).
