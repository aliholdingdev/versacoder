---
title: "VersaCoder — Tel Kafes (Wireframe) Şablonu"
type: template
category: template
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
scope: "tel kafes (wireframe) sablonu: ekran kutulari, alan etiketleri ve gezinme notlari"
---

# VersaCoder — Tel Kafes (Wireframe) Şablonu

**Zorunlu Bağlantılar:** [index](../../index.md) · [CLAUDE.md](../../CLAUDE.md) · [ascii-diagram-template](../architecture/ascii-diagram-template.md) · `screen-spec-template` · `flow-template` · [adr-devexpress-template](../adr/adr-devexpress-template.md)

---

## §1 Amaç

Bu şablon `.ai/ui-design/` altındaki **yedi zorunlu tel kafesi** tek biçimde yazdırır: düşük sadakat, birim tabanlı, gri tonlamalı, **66 karakterlik ASCII kutu içinde** (`|` `-` `+`; Unicode kutu çizim `U+2500-U+257F` yasak). Tel kafes `GATE-003'ün kanıtıdır`; onaysız tel kafes yoksa ekran kodu yoktur.

| Alan | Değer |
|------|-------|
| Template Name / Path | `wireframe-template.md` · `.ai/.templates/ui-design/` |
| Hedef Dosya | `.ai/ui-design/01-start-page.md … 07-settings.md` (7 ekran) |
| Karakter Seti | ASCII `|` `-` `+` (kutu) + `→ v [ ]` — Unicode kutu çizim yasak |
| Kodlama | **UTF-8 (BOM'suz)** — Latin-1/CP1254 YASAK |
| Standart Genişlik | **66 karakter** (`| ` + 63 gövde + `|`) |
| Sadakat | L2 — bileşen (§3.4); piksel birebir DEĞİL |
| Kapı | `GATE-003` — 7 tel kafes onayı, **onsuz ekran kodu yazılmaz** |
| Dil / Versiyon / Authority | Türkçe (ç ğ ı İ ö ş ü) · 1.0.0 · **SSOT** · 2026-09-25 |

### §1.1 Tel Kafes Nedir / Ne Değildir

| YAPILIR | YAPMAZ |
|---------|--------|
| Düşük sadakat, yaklaşık ölçüler (`≈240 px`) | Piksel birebir tasarım çıktısı |
| Bölge + birim gösterimi (alan, satır, sekme) | Gerçek veri / üretim içeriği |
| Gri tonlama + sembol dolgu (`#` `░` `·`) | Hex renk, tema, font (→ reference-template) |
| DevExpress **26.1** bileşen **adayı** | Sürüm/paket kurulumu (ADR-001) |
| Akış + kapı (GATE-003) kanıtı | Kod, `*.cs`, `Designer.cs` |
| Odak sırası + WCAG notu | Kaynak kod etiketi, betik |

### §1.2 Disk Kanıtları ve Boşluk (YAGNI)

İddia → kanıt; kanıt yoksa uydurulmaz. Ölçüm: `C:\versacoder`, 2026-09-25.

| # | İddia | Durum | Kanıt |
|---|-------|-------|-------|
| 1 | `.ai/ui-design/` + `01…07` tel kafesleri | ❌ YOK | `Test-Path = False`, 0 dosya |
| 2 | `.ai/.templates/ui-design/` klasörü | ✅ DOĞRULANDI | Part C paketi (5 MD), ölçüm §6.5 K1 |
| 3 | `.ai/.templates/index.md` §3.4 satır 214–218 | ✅ OKUNDU | Part C #25–29 |
| 4 | `reports/00-kapsam-analizi.md` §4.1 yedi ekran | ✅ OKUNDU | orta adlar çelişiyor → ⚠️ (§3.6) |
| 5 | `.ai/WORKFLOW.md` — GATE kaynağı | ❌ YOK | gate bilgisi `index.md` §3.6/§4.5 G8 + rapor §4.3 |
| 6 | `.ai/architecture/index.md` · `keys.md` · `.rules/` (7 MD) | ❌ YOK | envanter beyanı → ⚠️ VERIFICATION REQUIRED |
| 7 | `.ai/adr/ADR-001` — DevExpress `26.1` kilidi | ❌ `.ai/adr/` YOK | brief/envanter beyanı → ⚠️ |
| 8 | `src/VersaCoder.Desktop/**` (RibbonForm) | ❌ YOK | — |
| 9 | `ascii-diagram-template.md` (biçim altın örneği) | ✅ DOĞRULANDI | 644 satır / 37,2 KB |
| 10 | Harici vault `C:\www\…` | 🔒 ERİŞİM YASAK | yalnız **yapı** referansı, kopyalama yok |

**Yorum yasağı:** Var olmayan dosya için "şurada tanımlı" denmez; `⚠️ VERIFICATION REQUIRED` yazılır, satır olduğu yerde durur.

### §1.3 GATE-003 Kapısı ve Yedi Zorunlu Ekran

`.ai/WORKFLOW.md` diskte yok; kapı tanımı `index.md` §3.6 ve `reports/00-kapsam-analizi.md` §4.3'ten okundu (`GATE-003 = UI Approval, kullanıcı onayı ZORUNLU`).

```text
+----------------------------------------------------------------+
| GATE-002  requirements   spec + ADR taslağı                    |
|    v                                                           |
| PART 4    bu şablonla 7 tel kafes yazılır                      |
|    v                                                           |
| GATE-003  ui  TEL KAFES ONAYI <== KAPI: onaysiz ekran YASAK    |
|    v                                                           |
| PART 5    form/ribbon şablonları ve kod yalnız bundan sonra    |
+----------------------------------------------------------------+
```

Kural: `GATE-003` kaydı yoksa `src/VersaCoder.Desktop/**` üzerinde tek satır kod yazılmaz (envanter G8). Ters sıra keşfedilirse iş durur.

### §1.4 Neden Kutu + Ölçü Zorunlu?

| İhtiyaç | Kutu + ölçü | Kutusuz metin |
|---------|-------------|----------------|
| Bölge sınırı (panel mi, boşluk mu) | ✅ sınır fiziksel | ✗ "sol tarafta" belirsiz |
| Ölçek (240 px vs 400 px) | ✅ etiket + oran | ✗ tartışmaya açık |
| Bileşen adayı | ✅ §3.5 sözlüğü | ✗ kod yazarken değişir |
| Kapı nerede duruyor | ✅ `<== KAPI:` satırı | ✗ süreç gizlenir |
| Onay kanıtı | ✅ §5.3/1 ölçümü | ✗ kanıtsız "hizalı" |

---

## §2 Kapsam

**İçinde:** yedi tel kafesin yazımı, kutu alfabesi, 66 hizası, ölçülendirme, bileşen sözlüğü, GATE-003 kanıtı.
**Dışında:** renk/tema/ikon (`[[./reference-template]]`), ekran şartnamesi (`[[./screen-spec-template]]`), akış (`[[./flow-template]]`), prompt (`[[./prompt-template]]`), kod (`*.cs`).

### §2.1 Dosya Tipi → Sorumlu Eşlemesi

| Dosya Tipi | Hedef | Sorumlu | Bu Şablondan mı |
|------------|-------|---------|------------------|
| `.ai/ui-design/NN-<ekran>.md` | 7 tel kafes | ui-designer | ✅ **bu şablon** |
| `.ai/ui-design/screen/*.md` | Form/View şartnamesi | ui-designer | ❌ screen-spec |
| `.ai/ui-design/flow/*.md` | akış diyagramı | ui-designer | ❌ flow |
| `.ai/ui-design/reference/*.md` | renk/tipo/ikon | ui-designer | ❌ reference |
| `.ai/ui-design/prompts/*.md` | üretim promptu | ui-designer | ❌ prompt |
| `src/VersaCoder.Desktop/**/*.cs` | ekran kodu | developer | ❌ **GATE-003 öncesi yasak** |
| `src/**/XForm.cs`, `XRibbonPage.cs` | form/ribbon | developer | ❌ Part C #30–32, tel kafes SONRASI |
| Hex renk, font, kontrast kararı | tasarım sistemi | ui-designer | ❌ reference-template |

### §2.2 Kapsam Sınırı Testi

İlk `EVET` şablonu belirler: (1) ekranın **görsel iskeleti** mi → bu şablon · (2) **renk/tema/kontrast** mı → reference · (3) **olay → sonuç** mı → screen-spec · (4) **başlangıç → adımlar → son durum** mu → flow · (5) **model/agent komutu** mu → prompt · (6) **`.cs`** mi → ❌ kapı geçmeden üretilmez.

`HAYIR` soruları: yasaklı vault içeriği mi (G11)? · EF Core içeriyor mu (G10)? · hex renk var mı (G3)? · `{{…}}` yayımlanacak mı (G8)?

---

## §3 Mimari

Çizim alfabesi `[[../architecture/ascii-diagram-template]]` ile aynıdır; **tek fark genişliktir: diyagram 64, tel kafes 66 karakter.**

### §3.1 Çizim Alfabesi (tek karakter seti)

| Amaç | Karakter | Tel Kafeste |
|------|----------|-------------|
| Yatay çizgi | `-` | bant ayracı — Unicode `U+2500` yasak |
| Dikey | `|` | kenar — Unicode `U+2502` yasak |
| Köşe | `+ + + +` | pencere/panel köşesi |
| Ayraç | `+ +` | bant ayrımı |
| Sütun ayracı | `+ + +` | panel kenarı/kesişimi |
| Dolgu | `░ ▒ █` | boş · katman · vurgulu (dolgu, kutu çizimi değil) |
| Metin dolgusu | `#` | yazı bloğu |
| Dolgu/leader | `·` | satır içi doldurma (U+00B7, kutu çizimi değil) |
| Ok | `→` `v` | odak ve devam yönü (U+2192, kutu çizimi değil) |
| Kontrol adayı | `[ ]` `<>` `≡` | kutu · buton · ribbon |

**Yasaklar:** Unicode kutu çizim (`U+2500-U+257F`), emoji, hex renk, Latin-1 `Ä±`.

### §3.2 Hizalama Kuralı (tek kural — 66)

| Kural | Hesap |
|-------|-------|
| Tek sütun | `| ` + içerik + dolgu + `|` = **66**; içerik + dolgu = 63 |
| Dolgu | `dolgu = 63 − uzunluk(içerik)` |
| İki sütun (panel split) | gövde 64 = sol 31 + ayraç 1 + sağ 32 |
| Ayraçlı çizgi | `+` + 31×`-` + `+` + 32×`-` + `+` |
| Ölçüm birimi | karakter (UTF-16), Türkçe harf = 1, emoji = 2 → yasak |
| Taşma | içerik kısaltılır, kutu **genişletilmez** |
| Blok | tüm kutular ` ```text ` içinde (MD dönüştürücü bozmasın) |

Hizalama elle tahmin edilmez; §5.3/1 ile **kanıtlanır**. Ölçüm geçmeden tel kafes onaya girmez (G12).

### §3.3 Ölçülendirme ve Piksel Notasyonu

| Öğe | Gösterim | Örnek |
|-----|----------|-------|
| Ekran çerçevesi | ilk satırda köşeli parantez | `[1280×800]` |
| Konsol genişliği | gövde notu | `konsol 66 karakter` |
| Panel ölçüleri | `≈` + px | `SOL 240 px` |
| Yükseklik | bant satırı | `ribbon ≈3 satır` |
| Birim ölçeği | kabul | `1 bant ≈ 1 satır ≈ 24 px` |
| Sabit / akışan | etiket | `[sabit]` · `[akışan]` |
| Taşan içerik | kırpma | `···` |

px değerleri **yaklaşık**dır; `≈`sız kesin ölçüm RED (G13). Kesin ölçü `screen-spec`'te yaşar.

**Klavye ve erişilebilirlik notu (zorunlu):** her tel kafesin son satırında odak sırası `§1 → §2 → §3` yazılır; klavye ile gezinme sırası görsel okuma sırasıyla **aynı** olmalı (WCAG 2.2 §2.4.3). Modal açıldığında odak modal içinde kalır (§3.5 B6), Esc ile geri dönüş `flow` şablonunda tanımlanır.

### §3.4 Sadakat Seviyeleri ve Dolgu Sembolleri

| Seviye | Gösterilir | Gösterilmez | Kullanım |
|--------|------------|-------------|----------|
| **L1 iskelet** | bantlar, bölgeler | bileşen ayrıntısı | ilk taslak |
| **L2 bileşen** | bileşen adayı, yerleşim | renk, tipografi | **GATE-003 standartı** |
| **L3 etkileşim** | odak, boş/hata ipucu | kod, veri | screen-spec köprüsü |

| Sembol | Anlam | Sembol | Anlam |
|--------|-------|--------|-------|
| `█` | vurgulu/aktif | `░` | boş yüzey, pasif |
| `▒` | üst katman (modal perdesi) | `#` | metin bloğu |
| `·` | satır içi doldurma | `···` | taşan içerik kırpımı |
| `[x]` / `[ ]` | işaretli / işaretsiz kutu | `<>` `≡` | buton · ribbon |

### §3.5 Bileşen Sözlüğü (her biri kutulu örnek)

Bileşen adayları `reports/00-kapsam-analizi.md` §4.1'den gelir; **aday = kesin değil**, GATE-003'te onaylanır.

**B1 — Ribbon (`RibbonControl`, Office2010):**

```text
+----------------------------------------------------------------+
| ≡ RIBBON   Dosya   Düzenle   Görünüm   Araçlar   Yardım        |
| [Grup: Yeni]  <>Yeni  <>Aç  <>Kaydet    [Grup: Görünüm]  ≡     |
+----------------------------------------------------------------+
```

**B2 — Tablo (`GridControl`):**

```text
+----------------------------------------------------------------+
| TABLO   Ad · Durum · Tarih · İşlem                         ▼   |
| # ##########   ░░░   ······   ▼                                |
| # ##########   ░░░   ······   ▼                                |
+----------------------------------------------------------------+
```

**B3 — Ağaç (`TreeList`, Çözüm Gezgini):**

```text
|----------------------------------------------------------------|
|AĞAÇ   v Proje                                                  |
|      +-- v src                                                 |
|          +-- <>VersaCoder.Desktop                              |
|          +-- <>Program.cs                                      |
|----------------------------------------------------------------|
```

**B4 — Yan Panel (`DockManager`) + ana alan:**

```text
+-------------------------------+--------------------------------+
| YAN PANEL  ≈240 px [sabit]    | ANA ALAN  [akışan]             |
| ≡ Başlık  [×]   ···           | # #####  # #####  # #####      |
| ░ boş bölge                   | ░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░|
+-------------------------------+--------------------------------+
```

**B5 — Durum Çubuğu (`StatusBarControl`):**

```text
+----------------------------------------------------------------+
| DURUM  Hazır · sürüm 1.0.0 · GATE-003: beklemede · odak §1     |
+----------------------------------------------------------------+
```

**B6 — Modal (`XtraDialog` + perde):**

```text
|----------------------------------------------------------------|
| ▒▒ PERDE (ana alan devre dışı) ▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒|
| ▒ + MODAL  Kaydet? ----------------------------[×] +▒▒▒▒▒▒▒▒▒▒▒|
| ▒ |# kaydedilmemiş değişiklikler var               |▒▒▒▒▒▒▒▒▒▒▒|
| ▒ |              <>Vazgeç  <>Kaydet                |▒▒▒▒▒▒▒▒▒▒▒|
| ▒ +------------------------------------------------+▒▒▒▒▒▒▒▒▒▒▒|
|----------------------------------------------------------------|
```

**B7 — Toast / Bildirim (sağ alt):**

```text
|----------------------------------------------------------------|
|                             + BILGI -------------[×] +         |
|                             | # derleme 0 hata       |         |
|                             +------------------------+         |
|----------------------------------------------------------------|
```

### §3.6 Yedi Zorunlu Ekran Envanteri

| # | Dosya Adı | Ekran | Kod | DevExpress Adayı |
|---|-----------|-------|-----|------------------|
| 01 | `01-start-page.md` | Başlangıç Sayfası | WF-01 | RibbonControl, XtraTabControl, TileControl |
| 02 | `02-editor.md` | Düzenleyici | WF-02 | RibbonForm, DockManager, TreeList, MemoEdit |
| 03 | `03-preview.md` | Önizleme | WF-03 | XtraTabControl, PictureEdit, ProgressBar |
| 04 | `04-settings.md` | Ayarlar | WF-04 | DataLayoutControl, PropertyGrid, LookUpEdit |
| 05 | `05-themes.md` | Temalar | WF-05 | GridControl, SkinPalette, CheckedListBox |
| 06 | `06-docs.md` | Dokümantasyon | WF-06 | TreeList, RichEditControl, SearchControl |
| 07 | `07-about.md` | Hakkında | WF-07 | XtraTabControl, LabelControl, PictureEdit |

> **⚠️ VERIFICATION REQUIRED (dosya adları):** envanter `index.md` §3.4 yalnız **uçları** sabitler: `01-start-page.md … 07-settings.md`. `reports/00-kapsam-analizi.md` §4.1 orta adları `02-main-ide … 07-settings` verir. İki liste **çelişiyor** (rapor R1). Bu şablon Part C brief'indeki adlarla yazıldı; **04–07 satırları GATE-003'te kullanıcı onayıyla kesinleşir**, onay öncesi `⚠️` satırı silinmez.

**7/7 kuralı:** yedi dosyanın tamamı yoksa GATE-003 **geçemez** (§6.5 K2); eksik ekran = kapı RED.

### §3.7 Dolu Örnek W1 — `01-start-page` (L2, 66 hizalı)

```text
+----------------------------------------------------------------+
| [1280×800]  VERSA CODER — Başlangıç Sayfası   WF-01 · L2       |
+----------------------------------------------------------------+
| ≡ RIBBON   Dosya   Düzenle   Görünüm   Araçlar   Yardım        |
+----------------------------------------------------------------+
| TAB  [Yeni Proje]  [Son Kullanılanlar]  [Şablonlar]  [Ayarlar] |
+-------------------------------+--------------------------------+
| SOL ≈240 px [sabit]           | ANA ALAN  [akışan]             |
| ≡ Proje Ağacı                 | █ Yeni Proje kartı             |
|   ░ (boş — kayıt yok)         |   # kısa açıklama satırı       |
| ≡ Son Kullanılanlar (5)       |   <> Başlat    <> İçe Aktar    |
|   ···                         | ░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░|
+-------------------------------+--------------------------------+
| DURUM  Hazır · GATE-003: beklemede · odak §1 kart → §2 ağaç    |
+----------------------------------------------------------------+
```

**Ne anlatır:** dört bantlı iskelet (ribbon → tab → iki sütun → durum), sol panel sabit / ana alan akışkan, ilk odak `§1` başlangıç kartı. Renk/tema **yoktur** (G3).

### §3.8 Dolu Örnek W2 — `02-editor` (L2, 66 hizalı)

```text
|----------------------------------------------------------------|
|[1280×800]  VERSA CODER — Düzenleyici   WF-02 · L2              |
|----------------------------------------------------------------|
|≡ RIBBON   Dosya   Düzenle   Hata Ayıkla   Görünüm   Yardım     |
|----------------------------------------------------------------|
|HIZLI  <>Kaydet  <>Geri Al  <>Yinele        konsol ≈24 satır    |
|-------------------------------+--------------------------------|
|ÇÖZÜM GEZGİNİ ≈280 px          |SEKMELER  [Program.cs] [App.cs] |
|v src                          | + editör ---------------------+|
|  +-- v VersaCoder.Desktop     | | 1 # #####  ···              ||
|      +-- <>Form1.cs           | | 2 # #####                   ||
|      +-- <>Program.cs         | | 3 # #####  ···  ··· taşan   ||
|≡ ARAÇLAR  [AI Panel]          | +-----------------------------+|
|-------------------------------+--------------------------------|
|DURUM  düzenlemekte · odak §1 ağaç → §2 editör → §3 AI paneli   |
|----------------------------------------------------------------|
```

**Ne anlatır:** sol ağaç / sağ sekme+editör düzeni, AI paneli tetikleyicisi, üç adımlı odak sırası. Tek dosya = tek ekran; aynı anda iki ekran çizilmez.

### §3.9 Tam Kalıp İskeleti (kopyala-yapıştır)

Yer tutucular yalnız bu blokta yaşar; üretilen tel kafesde adedi **0** olmalıdır (G8).

````markdown
# Tel Kafes — {{EKAN_KODU}} · {{EKAN_ADI}}

| Alan | Değer |
|---|---|
| Ekran kodu / dosya | {{WF_NN}} · `.ai/ui-design/{{NN-ekran}}.md` |
| Çerçeve / sadakat | [{{1280×800}}] · L2 |
| Bileşen adayları | {{DEVEXPRESS_25_2_8_ADLARI}} |
| GATE | GATE-003 — {{onay_durumu}} |

```text
+----------------------------------------------------------------+
| [{{1280×800}}]  VERSA CODER — {{EKAN_ADI}}   {{WF_NN}} · L2    |
+----------------------------------------------------------------+
| ≡ RIBBON   {{MENU_SATIRI}}                                     |
+-------------------------------+--------------------------------+
| {{SOL_BOLGE_240PX}}           | {{SAG_BOLGE_AKISKAN}}          |
+-------------------------------+--------------------------------+
| DURUM  {{DURUM_CUBUGU}} · odak §1 → §2                         |
+----------------------------------------------------------------+
```

**Bölgeler:** {{BOLGE_LISTESI}} · **Ölçüler:** {{YAKLASIK_PX}}
**Erişilebilirlik:** {{ODAK_SIRASI + WCAG_NOTU}}
**Doğrulama:** §5.3/1 — {{YYYY-MM-DD}} (66/66 hizalı)
````

---

## §4 Kurallar

### §4.1 Tel Kafes Kalite Kuralları

| # | Kural | Test | İhlal |
|---|-------|------|-------|
| 1 | Her kutu satırı tam 66 karakter | §5.3/1 = 0 satır | **DUR** (G2) |
| 2 | Çerçeve `[1280×800]` ilk satırda, px `≈` ile | gözden geçirme | RED (G13) |
| 3 | Her bölgenin bileşen adayı yazılı | §3.5 eşleşmesi | RED |
| 4 | GATE-003 durumu dosyada görünür | `GATE-003:` satırı | RED |
| 5 | Renk/hex/tema yok | §5.3/4 = 0 | RED (G3) |
| 6 | Tek dosya = tek ekran | 1 dosyada 2 ekran yok | RED |
| 7 | Odak sırası `§1 → §2` yazılı | §3.3 zorunlu satır | RED |
| 8 | Bileşen adayı yalnız `26.1` bileşenleri | §5.3/4 | RED (G9) |

### §4.2 Sorumluluk ve Onay

| Aşama | Rol | Ne Onaylar | Çıktı | Süre |
|-------|-----|-----------|-------|------|
| 1 | ui-designer | 7 tel kafes + §5.3 ölçümü | `.ai/ui-design/NN-*.md` | 56 dk |
| 2 | reviewer | §6.1 checklist (22 madde) | ✅ / RED | 15 dk |
| 3 | architect | bileşen adayı ↔ kısıt | tutarlılık | 8 dk |
| 4 | **kullanıcı (GATE-003)** | 7 ekran onayı | kapı kaydı | 20 dk |
| 5 | master-orchestrator | §5.4 Quality Report | `log.md` satırı | 6 dk |
| | **Toplam** | | | **≈ 105 dk** |

**3 RED** sonrası iş durur, gerekçe `master-orchestrator`'a yükseltilir. Kullanıcı onayı yoksa aşama 5'e geçilmez, kod yasağı kalkmaz.

### §4.3 Ölçek / Standartlar

| Öğe | Değer | Sapma |
|-----|-------|-------|
| Satır genişliği | **66** sabit | ±1 = **DUR** |
| Maks. kutu satırı | 24 | aşımda ekran 2 bloğa bölünür |
| Maks. bant | 8 | aşımda alt ekrana inilir |
| Ekran sayısı | 7 (01–07) | 6 = kapı RED |
| Kod bloğu | ` ```text ` | boş ` ``` ` = RED |
| Kodlama | UTF-8 BOM'suz | Latin-1/CP1254 otomatik RED |

### §4.4 Değişiklik Kontrolü

| Olay | Aksiyon | `updated` | Kayıt |
|------|---------|-----------|-------|
| Yalnız hizalama düzeltmesi | dosya içinde | değişmez | §5.3/1 yeniden alınır |
| Bölge/ölçü değişti | tel kafes + açıklama | artar | `log.md` tek satır |
| Bileşen adayı değişti | §3.5 ile birlikte | minor sürüm | GATE-003'e bildirim |
| Ekran eklendi/çıkarıldı | 7/7 kuralı → kapı yeniden | artar | **kapı yeniden alınır** |
| Onay sonrası kod yazıldı | tel kafes dondurulur | patch | talep kapı üzerinden |

### §4.5 Hard Guardrails (Otomatik DUR)

| # | Guardrail | İhlal Sonucu |
|---|-----------|--------------|
| G1 | `-` veya `\|` ile çizim | **DUR** — §3.1 alfabesiyle yeniden çiz |
| G2 | Kutu satırı ≠ 66 (§5.3/1 > 0) | **DUR** — ölçüm geçene kadar |
| G3 | Tel kafeste hex renk / tema / font | **DUR** — reference şablonuna taşı |
| G4 | Emoji veya ASCII art kutu içinde | **DUR** — kaldır |
| G5 | `.ai/ui-design/` altında `.cs`/`Designer.cs` | **DUR** — kod ayrı part |
| G6 | 7 ekran eksik (§3.6) | **DUR** — GATE-003 geçemez |
| G7 | ` ```text ` olmayan kutu bloğu | **DUR** — blok türü düzeltilir |
| G8 | Üretilen tel kafeste `{{…}}` | **DUR** — §5.3/5 = 0 olana kadar |
| G9 | DevExpress sürümü `26.1` dışı | **DUR** — ADR-001 (⚠️ §1.2/7) |
| G10 | EF Core / concat SQL geçen satır | **DUR** — ADR-004, ADO.NET + Dapper |
| G11 | Yasaklı vault adı geçen satır | **DUR** — sil, özgün yaz |
| G12 | Kanıtsız "hizalı/onaylı" iddiası | §5.3/1 + GATE-003 kaydı istenir |
| G13 | `≈`sız kesin px ya da çerçeve satırı yok | **DUR** — §3.3 |
| G14 | GATE-003 alınmadan ekran kodu başlamış | **DUR** — iş geri alınır (G8) |

---

## §5 Workflow

### §5.1 Adım Bazlı Tel Kafes Akışı

| # | Adım | Girdi | Çıktı | Yapan | Süre |
|---|------|-------|-------|-------|------|
| 1 | Ekran listesini doğrula | §3.6 + envanter §3.4 | 7 dosya adı | ui-designer | 5 dk |
| 2 | Çerçeve + bantlar | §3.9 kalıp | L1 iskelet | ui-designer | 4 dk |
| 3 | Bileşen adayları | §3.5 + rapor §4.1 | L2 tel kafes | ui-designer | 6 dk |
| 4 | Ölçü + odak sırası | §3.3 + WCAG | `≈` + `§n` | ui-designer | 3 dk |
| 5 | Ölç | §5.3/1 | 66/66 | ui-designer | 1 dk |
| 6 | Düzelt | uyuşmayan satırlar | hizalı kutu | ui-designer | 3 dk |
| 7 | Kapı satırı | GATE-003 | `GATE-003:` | ui-designer | 1 dk |
| 8 | Denetim | §6.1 (22 madde) | ✅ / RED | reviewer | 10 dk |
| 9 | Kullanıcı onayı | 7 ekran | **GATE-003** | kullanıcı → orchestrator | 20 dk |
| 10 | Senkron | `.ai/log.md` | kayıt | docs-writer | 4 dk |
| | **Toplam (7 ekran)** | | | | **≈ 57 dk** |

Bütçe: 1–4 ≈ 18 dk/ekran, 5–8 ≈ 15 dk/ekran → **≈ 33 dk/ekran**. 33 dakikayı aşan tek tel kafes §1.1'e sapmıştır: renk/kod tartışması başlamış, L2'ye dönülür.

### §5.2 Hata Modları

| # | Hata Modu | Belirti | Kök Neden | Düzeltme |
|---|-----------|---------|-----------|----------|
| 1 | Hizasız kutu | §5.3/1 > 0 | elle dolgu | kısalt/pad, yeniden ölç |
| 2 | Tire/boru çizim | `- - -`, `\|` | ASCII alışkanlığı | §3.1 alfabesi |
| 3 | Renk sızması | hex, renk adı | tema şablonu karışmış | reference şablonu |
| 4 | Piksel takıntısı | 247 px | L2 sınırı unutulmuş | `≈` birime indir |
| 5 | Emoji kutuda | `✅` `🎨` | konfor eklentisi | `█`/`░`/düz metin |
| 6 | Yer tutucu sızıntısı | `{{BOLGE}}` | §3.9 doldurulmadı | temizle, §5.3/5 |
| 7 | Eksik ekran | 6/7 dosya | kapı atlanmış | 7/7 tamamla (G6) |
| 8 | Ters sıra | kod var, kafes yok | GATE-003 atlanmış | **DUR** (G14), geri dön |
| 9 | Ad çelişkisi | `07-about`/`07-settings` | §3.6 ⚠️ satırı | onaya kadar `⚠️` kalır |
| 10 | Yanlış bileşen | sözlükte olmayan kontrol | sözlük atlanmış | §3.5 + rapor §4.1 |

### §5.3 Salt Okunur Doğrulama Komutları

Altı komut da **hiçbir dosyayı değiştirmez** (kutu hizalama betiği ayrı araçtır, `-Fix` ile çalışır ve onaysız çağrılmaz).

```powershell
# 1) HIZALAMA ÖLÇER — çıkan satır sayısı 0 OLMALI (66 kuralı)
Get-ChildItem 'C:\versacoder\.ai\ui-design' -Filter *.md -ErrorAction SilentlyContinue |
  ForEach-Object { $box=$false; $i=0
    foreach ($l in [IO.File]::ReadAllLines($_.FullName, [Text.Encoding]::UTF8)) { $i++
      if ($l -match '^```text') { $box=$true; continue }
      if ($l -match '^`{3,}' -and $box) { $box=$false; continue }
      if ($box -and $l.Length -gt 0 -and [int]$l[0] -ge 0x2500 -and [int]$l[0] -le 0x257F -and $l.Length -ne 66) {
        '{0}:{1} uzunluk={2} (66 olmali)' -f $_.Name,$i,$l.Length } } }

# 2) YEDI EKRAN VARLIĞI — eksik dosya 0 (G6)
$w='01-start-page','02-editor','03-preview','04-settings','05-themes','06-docs','07-about'
$d='C:\versacoder\.ai\ui-design'
if (-not (Test-Path $d)) { 'DIZIN YOK -> 0/7 ekran (GATE-003 gecemez)' }
else { $w | ForEach-Object { if (-not (Test-Path (Join-Path $d "$_.md"))) { 'EKSIK -> '+$_ } } }

# 3) KODLAMA — BOM ve mojibake = 0
Get-ChildItem 'C:\versacoder\.ai\.templates\ui-design\*.md' | ForEach-Object {
  $b=[IO.File]::ReadAllBytes($_.FullName)
  if ($b.Length -ge 3 -and $b[0] -eq 0xEF -and $b[1] -eq 0xBB -and $b[2] -eq 0xBF) { 'BOM -> '+$_.Name }
  if ([Text.Encoding]::UTF8.GetString($b) -match '\u00C3[\u0080-\u00BF]|\uFFFD') { 'MOJIBAKE -> '+$_.Name } }

# 4) YASAK İÇERİK — hex renk, emoji, tire/boru çizim = 0 (G1, G3, G4)
Get-ChildItem 'C:\versacoder\.ai\.templates\ui-design\*.md','C:\versacoder\.ai\ui-design\*.md' -ErrorAction SilentlyContinue |
  Select-String -Pattern '#[0-9A-Fa-f]{6}\b' | ForEach-Object { 'HEX -> '+$_.Filename }
Get-ChildItem 'C:\versacoder\.ai\.templates\ui-design\*.md' |
  Select-String -Pattern '[\uD83C-\uDBFF][\uDC00-\uDFFF]' | ForEach-Object { 'EMOJI -> '+$_.Filename }

# 5) YER TUTUCU — üretilen tel kafes dosyalarında 0 (G8)
Get-ChildItem 'C:\versacoder\.ai\ui-design\*.md' -ErrorAction SilentlyContinue |
  Select-String -Pattern '\{\{[A-Z0-9_]+\}\}' | ForEach-Object { 'YER TUTUCU -> '+$_.Filename }

# 6) YAPI — frontmatter 8 alan + ## § = 7 + Zorunlu Bağlantılar = 6
$need='title','type','category','status','version','updated','authority','part'
Get-ChildItem 'C:\versacoder\.ai\.templates\ui-design\*.md' | ForEach-Object {
  $r=[IO.File]::ReadAllText($_.FullName)
  $m=$need | Where-Object { $r -notmatch ('(?m)^'+$_+':') }
  $s=([regex]::Matches($r,'(?m)^## \u00A7')).Count
  $b=([regex]::Match($r,'(?m)^\*\*Zorunlu Ba\u011Flant\u0131lar:\*\*(.*)$')).Groups[1].Value
  $c=([regex]::Matches($b,'\[\[')).Count
  if ($m) { 'EKSIK ALAN -> '+$_.Name+': '+($m -join ',') }
  if ($s -ne 7) { 'BOLUM -> '+$_.Name+': '+$s+'/7' }
  if ($c -ne 6) { 'BAGLANTI -> '+$_.Name+': '+$c+'/6' } }
```

**Geçme:** #1 = 0 satır · #2 = 0 eksik · #3–#6 = 0 satır uyarı.

### §5.4 Quality Report

```markdown
## Quality Report — Tel Kafes (YYYY-MM-DD HH:MM)

| Metrik | Değer | Eşik | Durum |
|---|---|---|---|
| Ölçüm (bu şablon) | <satır> satır / <KB> KB | 480–660 · 25–34 | 🟢/🔴 |
| Üretilen ekran dosyası | n / 7 | 7 | 🟢/🔴 |
| Hizasız kutu (§5.3/1) | n | 0 | 🟢/🔴 |
| Hex / emoji / tire-boru | n/n/n | 0/0/0 | 🟢/🔴 |
| Yer tutucu (§5.3/5) | n | 0 | 🟢/🔴 |
| Checklist §6.1 | n/22 | 22/22 | 🟢/🔴 |
| Guardrail G1..G14 | ihlal n | 0 | 🟢/🔴 |
| GATE-003 | açık / onaylı | kullanıcı onayı | 🟢/🔴 |

**Karar:** 🟢 kapıya sunulur / 🔴 düzeltmeye gönderildi (<gerekçe>)
```

---

## §6 Doğrulama

### §6.1 Kontrol Listesi (22 madde)

| # | Kontrol | Beklenen | İlgili |
|---|---------|----------|--------|
| K1 | Frontmatter 8 alan (`title…part`) | 8/8 | §5.3/6 |
| K2 | `authority: SSOT` birebir | `SSOT` | §5.3/6 |
| K3 | H1 + `**Zorunlu Bağlantılar:**` (6 bağlantı) | 1 satır | §5.3/6 |
| K4 | `## §1…§7` | 7/7 | §5.3/6 |
| K5 | `§1.1–§1.4` dolu | 4/4 | §1 |
| K6 | `§2.1` eşleme + `§2.2` sınır testi | 2/2 | §2 |
| K7 | `§3.1` alfabesi ≥10 satır | ≥10 | G1 |
| K8 | `§3.2` 66 hesabı + split 31/32 yazılı | var | G2 |
| K9 | `§3.3` ölçülendirme + WCAG/klavye notu | var | G13 |
| K10 | `§3.4` sadakat + dolgu tabloları | 2 dolu | §1.4 |
| K11 | `§3.5` yedi bileşen, her biri kutulu | 7 kutu | §3.1 |
| K12 | `§3.6` yedi ekran + ⚠️ notu | 7 + ⚠️ | §1.2/4 |
| K13 | `§3.7`–`§3.8` iki dolu ekran örneği | 2 | §4.3 |
| K14 | `§3.9` kalıp `{{…}}` dolu | evet | G8 |
| K15 | `§4.1` kalite kuralları ≥8 | 8 | — |
| K16 | `§4.2` 5 aşama + kullanıcı onayı | 5 | GATE-003 |
| K17 | `§4.5` G1..G14 dolu | 14 | — |
| K18 | `§5.1` 10 adım + bütçe | 10 | — |
| K19 | `§5.2` ≥8 hata modu | 10 | — |
| K20 | `§5.3` 6 salt-okunur komut | 6 | §6.5 |
| K21 | `§6.2` red + `§7.2` Red Team ≥10 satır | 9 + 11 | — |
| K22 | 480–660 satır · 25–34 KB · mojibake 0 | 3/3 | §6.5 |

### §6.2 Red Sinyalleri (Otomatik DUR)

| # | Sinyal | Aksiyon |
|---|--------|---------|
| R1 | §5.3/1 çıktısı > 0 satır | **DUR** — hizalama düzeltilir |
| R2 | Kutu içinde `-`/`\|` çizim | **DUR** — `-`/`|` ile yeniden çiz |
| R3 | Tel kafeste hex renk/tema | **DUR** — reference şablonuna taşı |
| R4 | 7 dosyadan eksik olan var | **DUR** — GATE-003 açılamaz |
| R5 | Üretilen dosyada `{{…}}` | **DUR** — kalıp doldurulur |
| R6 | `.cs` / `Designer.cs` üretimi | **DUR** — kod ayrı part (G14) |
| R7 | Mojibake / BOM / karışık dil | **DUR** — UTF-8 BOM'suz yeniden yaz |
| R8 | Ölçüm çıktısı olmadan "hizalı" | **DUR** — §5.3/1 çıktısı istenir |
| R9 | `26.1` dışı sürüm ya da EF Core | **DUR** — ADR-001 / ADR-004 |

### §6.3 GATE-003 Geçiş Kuralları

| Geçiş | Koşul (hepsi `EVET`) | Onaylayan | Etki |
|-------|----------------------|------------|------|
| taslak → inceleme | 7/7 dosya · §5.3/1 = 0 · §6.1 22/22 | ui-designer → reviewer | kapıya sunulur |
| inceleme → **onay** | §6.2 = 0 · §5.4 `log.md`'de | **kullanıcı** | `GATE-003` kaydı |
| onay → kod | kapı kaydı mevcut | orchestrator | form/ribbon şablonları serbest |
| onay → revizyon | bölgesel değişiklik talebi | ui-designer | kapı **yeniden** alınır |
| taslak → geri dönüş | B/C'de kusur | architect | Part C içinde kalır, ileri sıçrama yasak |

Kapı yalnız ileri açılır; onay sonrası değişiklik kapı üzerinden yapılır, kod içinden tel kafes düzeltilemez (G14).

### §6.4 İlgili Dosyalar

| Dosya | İlişki |
|-------|--------|
| `.ai/.templates/ui-design/wireframe-template.md` | **bu dosya** — tel kafesin tek kaynağı |
| `.ai/.templates/ui-design/screen-spec-template.md` | tel kafesin teknik karşılığı |
| `.ai/.templates/ui-design/flow-template.md` | akış + hata dalları |
| `.ai/.templates/ui-design/reference-template.md` | renk/tipografi (bu şablonda yasak) |
| `.ai/.templates/ui-design/prompt-template.md` | üretim promptu (kapı SONRASI) |
| `.ai/.templates/architecture/ascii-diagram-template.md` | alfabenin kaynağı (genişlik 64) |
| `.ai/.templates/index.md` | envanter satır 214 + §4.5 G8 |

### §6.5 Envantere Özgü Kontroller

| # | Kontrol | Beklenen | Kanıt |
|---|---------|----------|-------|
| K1 | Bu şablon satır aralığı | 480–660 | `Get-Content` + `Measure-Object -Line` |
| K2 | `.ai/ui-design/` dosya sayısı (üretim sonrası) | 7 | §5.3/2 |
| K3 | Bu şablon KB aralığı (envanter: `14,0`) | 25–34 (brief) | `(Get-Item …).Length/1KB` |
| K4 | Kutu satırları 66 karakter | 0 sapma | §5.3/1 |
| K5 | `§3.6` adları ↔ envanter `index.md` §3.4 ucu | `01-start-page` + `…07-*` | okuma + ⚠️ notu |
| K6 | `26.1` tek sürüm (ADR-001 beyanı) | tek değer | §5.3/4 + G9 |
| K7 | Yasaklı vault adı geçen satır | 0 | G11 |

---

## §7 Referanslar

### §7.1 VersaCoder Bağlantıları

| Kaynak | Yol | Ne İçin | Durum (2026-09-25) |
|--------|-----|---------|---------------------|
| Şablon envanteri | `.ai/.templates/index.md` | §3.4 Part C · G8 | ✅ okundu |
| Biçim referansı | `.templates/architecture/ascii-diagram-template.md` | alfabenin kaynağı | ✅ 644 satır / 37,2 KB |
| Ekran/kapı envanteri | `.ai/reports/00-kapsam-analizi.md` §4.1, §4.3 | 7 ekran + GATE-003 | ✅ okundu (ad çelişkisi §3.6) |
| Akış/kapı gerçeği | `.ai/WORKFLOW.md` | GATE-003 tanımı | ✅ Dosya var, §0 zinciri 2026-10-04 |
| Katman gerçeği | `.ai/architecture/index.md` | bileşen katmanı | ❌ YOK → ⚠️ |
| DevExpress kilidi | `.ai/adr/ADR-001` | `26.1` | ❌ `.ai/adr/` YOK → ⚠️ |
| Kardeş şablonlar | `./screen-spec`, `./flow`, `./reference`, `./prompt` | iş bölümü | ✅ Part C paketi |
| Yasaklı vault'lar | `C:\www\…` (envanter G1) | yalnız **yapı** referansı | 🔒 kopyalama YASAK |

### §7.2 Red Team — Sık Yapılan Hatalar

| # | Hata | Neden Yanlış | Doğrusu |
|---|------|--------------|---------|
| 1 | `-`/`\|` ile kutu çizmek | monospace'de kayar, dönüşmez | §3.1 (`-|+++++++++`) |
| 2 | Satırları elle hizalamak | mutlaka 1 karakter kayar | §5.3/1 ile ölç, sonra onayla |
| 3 | Piksel birebir tel kafes (247 px) | L2 ihlali, onay yavaşlar | `≈` birim + `[1280×800]` çerçeve |
| 4 | Tel kafese renk/hex koymak | tema başka şablonda (G3) | `█░▒` + reference şablonu |
| 5 | Emoji ile durum göstermek | 2 code unit, hizalama bozulur | düz metin / `█` |
| 6 | 7. ekranı atlayıp koda başlamak | GATE-003 ihlali, iş geri alınır | 7/7 + kapı kaydı (G6, G14) |
| 7 | Ekran adlarını uydurmak | envanter §3.4 ile çelişir | §3.6 + `⚠️` satırı korunur |
| 8 | `{{BOLGE}}` ile onaya sunmak | iskelet onaylanıyor | G8, §5.3/5 = 0 |
| 9 | Tek dosyada iki ekran | denetim ve kapı takibi kırılır | 1 dosya = 1 ekran |
| 10 | Kod yazıp kafesi sonradan çizmek | onay, üretilenin peşine takılır | sıra: kafes → GATE-003 → kod |
| 11 | Yasaklı vault cümlesi yapıştırmak | G1 ihlali, ikinci gerçek kaynak | yalnız yapı fikri, cümle özgün |

**SSUT beyanı:** Tel kafes biçiminin **tek** kaynağı bu dosyadır; `screen-spec`, `flow`, `prompt` ve `frontend` şablonları yalnız buna bağlanır. Kopyalanan her satır ikinci gerçek kaynağı doğurur (envanter G12).
