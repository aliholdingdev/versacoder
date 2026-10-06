---
title: "VersaCoder — Ekran Şartnamesi (Screen Spec) Şablonu"
type: template
category: templates/ui-design
status: active
version: 1.0.0
updated: 2026-10-05
authority: SSOT
scope: "ekran şartnamesi: ekran kimliği, 1024x600 pixel-reference, layout agaci, kontrol envanteri, durum matrisi ve WCAG 2.2 AA"
---

# VersaCoder — Ekran Şartnamesi (Screen Spec) Şablonu

**Zorunlu Bağlantılar:** [index](../index.md) · [wireframe](./wireframe-template.md)

---

## §1 Amaç

Bu şablon `.ai/ui-design/screen/` altındaki **yedi ekran şartnamesini** tek biçimde yazdırır: tel kafesin (görsel iskelet) **teknik karşılığı** olan ekran başına bir `.md` dosyası; kimlik, viewport, layout ağacı, kontrol envanteri, durum matrisi, WCAG 2.2 AA, doğrulama kuralları, kabul kriterleri ve GATE-003 onay kutusu bu dosyada yaşar. Tel kafes "nasıl görünüyor" der, şartname "hangi kontrol, hangi binding, hangi olay, hangi durum" der.

| Alan | Değer |
|------|-------|
| Template Name / Path | `screen-spec-template.md` · `.ai/.templates/ui-design/` |
| Hedef Dosya | `.ai/ui-design/screen/01-start-page.md … 07-settings.md` (7 şartname) |
| Tür | `T-UX` (envanter Part C, satır #26) |
| Sadakat | L3 etkileşim — tel kafesin (L2) üstü, kodun altı |
| Viewport | **1024×600** pixel-reference (§3.2) |
| Kodlama | **UTF-8 (BOM'suz)** — Latin-1/CP1254 YASAK |
| Kapı | `GATE-003` — 7 şartname + 7 tel kafes birlikte onaylanır |
| Dil / Versiyon / Authority | Türkçe (ç ğ ı İ ö ş ü) · 1.0.0 · **SSOT** · 2026-10-05 |

### §1.1 Şartname Ne İşe Yarar / Ne Değildir?

| YAPILIR | YAPMAZ |
|---------|--------|
| Kontrol **kimliği**, tipi, binding yolu, olayı | Kontrolün **kodu** (`*.cs`) |
| 1024×600 üzerinde ölçü + yerleşim hiyerarşisi | Hex renk, font, kontrast (→ `reference`) |
| loading / empty / error / permission durumları | Sunucu tarafı hata metni kaynağı (→ handler) |
| WCAG 2.2 AA + sekme sırası + klavye kısayolu | Ekran görüntüsü / piksel birebir mockup |
| Doğrulama kuralı ve hata gösterim yeri | Domain invariant (→ `l1` katmanı) |
| Acceptance criteria + GATE-003 onay kutusu | Kullanım senaryosu tam akışı (→ `flow`) |

### §1.2 Disk Kanıtları ve Boşluk (YAGNI)

İddia → kanıt; kanıt yoksa uydurulmaz. Ölçüm: `C:\versacoder`, 2026-10-05.

| # | İddia | Durum | Kanıt |
|---|-------|-------|-------|
| 1 | `.ai/ui-design/screen/*.md` (7 şartname) | ❌ YOK | `Test-Path = False`, 0 dosya |
| 2 | `.ai/.templates/ui-design/screen-spec-template.md` | ❌ YOK | envanter #26 `PLANLI · 16,0` |
| 3 | `.ai/.templates/ui-design/wireframe-template.md` | ✅ DOĞRULANDI | 642 satır / 34,5 KB |
| 4 | `.ai/.templates/index.md` §3.4 satır 216 | ✅ OKUNDU | hedef `.ai/ui-design/screen/*.md` |
| 5 | `.ai/index.md` · `.ai/CLAUDE.md` · `.ai/WORKFLOW.md` | ❌ YOK | `.ai/` altında yalnız `.templates` var |
| 6 | `.ai/.rules/` · `.ai/.decisions/` · `.ai/architecture/` | ❌ YOK | envanter beyanı → ⚠️ VERIFICATION REQUIRED |
| 7 | `.ai/reports/00-kapsam-analizi.md` | ❌ YOK | ⚠️ — kapı tanımı ikincil kaynaklardan |
| 8 | `src/VersaCoder.Desktop/**` (ekran kodu) | ❌ YOK | `src/` = `Test-Path False` |
| 9 | `GATE-003` kayıt satırı | ❌ YOK → ⚠️ | kapı henüz alınmadı |
| 10 | Harici vault `C:\www\…` (envanter G1) | 🔒 ERİŞİM YASAK | yalnız **yapı** referansı, cümle kopyalanmaz |

**Yorum yasağı:** Var olmayan dosya için "şurada tanımlı" denmez; `⚠️ VERIFICATION REQUIRED` yazılır, satır olduğu yerde durur. `.ai/index.md` gibi kırık sayılan yollara **bağlantı verilmez**, yalnız metin olarak değinilir.

### §1.3 GATE-003 Kapsamı — Şablon Muafiyeti (kritik ayrım)

`GATE-003` yasağı **yalnız iki şeyi** kapsar: (1) gerçek ekran dosyası (`.ai/ui-design/screen/*.md`, tel kafes, prompt çıktısı), (2) `src/**` kodu. **Bu şablon bir şablondur; kapı alınmadan da yazılabilir ve yazılmalıdır** — tersi Part C'yi kilitler (kök `CLAUDE.md` G8: "ekran/kod üretilmez").

```text
+------------------------------------------------------------------+
| [1] bu dosya (şablon)      GATE-003'ten MUAF  -- yazilabilir     |
|     v                                                            |
| [2] .ai/ui-design/screen/  KAPI GEREKIR -- 7 sartname onayi      |
|     v                                                            |
| [3] .ai/ui-design/ 01..07  tel kafes + sartname birlikte onay    |
|     v                                                            |
| [4] src/**  *.cs           KAPI SONRASI -- onaysiz tek satir yok |
+------------------------------------------------------------------+
```

| Öğe | GATE-003 kapsamı | Bu şablon |
|-----|------------------|-----------|
| `.ai/.templates/ui-design/screen-spec-template.md` | ❌ kapsam dışı | ✅ serbest |
| `.ai/ui-design/screen/01-start-page.md` (gerçek dosya) | ✅ kapsar | ⛔ kapı öncesi yazılmaz |
| `src/VersaCoder.Desktop/**/*.cs` | ✅ kapsar | ⛔ kapı öncesi yazılmaz |
| `.ai/ui-design/prompts/*.md` (üretim promptu) | ❌ kapsam dışı | ✅ serbest |

**Kural:** Şablon ile üretim aynı kapıdan geçmez. Kapı alındıktan sonra şablon **dondurulur**; değişiklik talebi kapı üzerinden yapılır (§6.3).

### §1.4 Neden 1024×600 Pixel-Reference?

| İhtiyaç | 1024×600 | 1920×1080 |
|---------|----------|-----------|
| Masaüstü penceresi gerçekliği | ✅ minimum desteklenen ekranda taşmaz | ✗ çoğu kullanıcıda referans değil |
| Ölçek hesabı | ✅ 1 birim = 1 px, oran 16:9,5 | ✗ hi-DPI oranı belirsiz |
| Ribbon + grid sığdırma | ✅ dar alanda zorunlu | ✗ geniş alanda yanıltır |
| Ölçü tutarlılığı | ✅ tek reference, ±0 sapma | ✗ iki farklı referans = iki gerçek |
| DPI %150'e taşınabilirlik | ✅ 1024 → 1536 ölçeklenir | ✗ 1920 → 2880 sığmaz |

**Kural:** Şartnamedeki **her** px değeri 1024×600 üzerine yazılır; diğer çözünürlükler bu reference'dan **oranla** hesaplanır. `≈` ile verilen değer yaklaşık, `=` ile verilen değer kesindir (§4.3).

---

## §2 Kapsam

**İçinde:** 7 ekran şartnamesinin yazımı, ekran kimliği, viewport hesabı, layout ağacı, kontrol envanteri, durum matrisi, WCAG 2.2 AA ve sekme sırası, doğrulama kuralları, acceptance criteria, GATE-003 onay kutusu.
**Dışında:** tel kafes çizimi (`./wireframe-template.md`), renk/tipografi/ikon (`reference`), akış ve hata dalları (`flow`), üretim promptu (`prompt`), `*.cs` kodu, SQL.

| Kapsam | Kapsam Dışı |
|--------|-------------|
| Kontrol ID'si + tipi + binding yolu + olayı | Kontrolün kodu (→ `../frontend/form-template.md`) |
| loading/empty/error/permission matrisi | Akış adımları ve cancellation (→ `flow`) |
| WCAG 2.2 AA + sekme sırası + kısayollar | Renk, token, kontrast hesabı (→ `reference`) |
| Doğrulama kuralı (gösterim yeri dahil) | Domain invariant / iş kuralı (→ `l1` · `l2`) |
| Acceptance criteria + onay kutusu | Kullanıcı onayının kendisi (→ GATE-003) |
| Viewport 1024×600 ölçüleri | Paket/sürüm kilidi (→ ADR-001) |

### §2.1 Dosya Tipi → Sorumlu Eşlemesi

| Dosya Tipi | Hedef | Sorumlu | Bu Şablondan mı |
|------------|-------|---------|------------------|
| `.ai/ui-design/screen/NN-<ekran>.md` | 7 şartname | ui-designer | ✅ **bu şablon** |
| `.ai/ui-design/NN-<ekran>.md` (kök) | 7 tel kafes | ui-designer | ❌ → `./wireframe-template.md` |
| `.ai/ui-design/flow/*.md` | akış diyagramı | ui-designer | ❌ → `flow` |
| `.ai/ui-design/reference/*.md` | token/tipografi | ui-designer | ❌ → `reference` |
| `.ai/ui-design/prompts/*.md` | üretim promptu | ui-designer | ❌ → `prompt` |
| `src/VersaCoder.Desktop/**/*.cs` | ekran kodu | developer | ❌ **GATE-003 öncesi yasak** |
| `src/**/XForm.cs`, `XRibbonPage.cs` | form/ribbon | developer | ❌ → `../frontend/form-template.md` |
| `tests/VersaCoder.Tests/**` | ekran testi | testing | ❌ → `../testing/unit-test-template.md` |

### §2.2 Kapsam Sınırı Testi

Üretilecek şartnameye sırayla sor; **ilk `EVET`** şablonu belirler:

1. Ekranın **teknik dökümü** mi (kontrol, binding, durum, WCAG)? → **bu şablon**.
2. Ekranın **görsel iskeleti** mi (kutu, ölçü, sadakat)? → `./wireframe-template.md`.
3. **Olay → sonuç → geri dönüş** mü? → `flow`.
4. **Renk / tipografi / token / ikon** mu? → `reference`.
5. **Model veya agent komutu** mu? → `prompt`.
6. **`.cs`** mi → ❌ kapı geçmeden üretilmez (§1.3).

`HAYIR` soruları: yasaklı vault cümlesi mi (G1)? · EF Core veya yıldızlı sorgu var mı (G2)? · `{{…}}` yayımlanacak mı (G5)? · GATE-003 kaydı olmadan gerçek ekran dosyası mı (G7)?

---

## §3 Mimari

Bu bölüm hem **şablonun kendi yapısını** hem **üretilecek şartnamenin yapısını** tanımlar. Katman sözlüğü (`l0..l7`) bu bölümde sabittir; şartname yalnız `l7`'yi (UI+CLI) tarif eder.

### §3.1 Dosya ve Ekran Kimliği

Her şartname dosyası **tek ekranı** kapsar; dosya adı tel kafes dosyasıyla aynı gövdeyi kullanır.

| Alan | Kural | Doğru | Yanlış |
|------|-------|-------|--------|
| Dosya adı | `.ai/ui-design/screen/NN-<ekran>.md` | `screen/01-start-page.md` | `screen/screens.md` (toplu) |
| Ekran kodu | `SP-NN` (spec) — tel kafes `WF-NN` | `SP-03` | `S3` |
| Ekran adı | Türkçe, tel kafesi ile birebir | "Önizleme" | "Preview Ekranı" |
| Amaç | tek cümle: "Kullanıcı … yapar" | `{{AMAC_CUMLESI}}` | boş hücre |
| Ana binding | `l7 → l2` handler köprüsü | `{{UseCase}}Handler` | repository adı |
| Viewport | 1024×600 (§3.2) | `1024×600` | `1920×1080` |
| GATE | kapı durumu satırı | `GATE-003: beklemede` | satır yok |

### §3.2 Viewport — 1024×600 Pixel-Reference Kuralı

| Kural | Hesap | Örnek |
|-------|-------|-------|
| Reference çerçeve | ilk satırda `[1024×600]` | `[1024×600]` |
| Yatay toplam | bölge genişlikleri + kenar boşlukları = 1024 | 40 + 240 + 744 + 40 = 1064 ❌ → düzelt |
| Dikey toplam | ribbon + tab + gövde + durum = 600 | 96 + 32 + 440 + 32 = 600 ✅ |
| Kesin ölçü | `=` ile yazılır | `yan panel = 240 px` |
| Yaklaşık ölçü | `≈` zorunlu | `≈ 238 px` |
| Ölçekleme | başka çözünürlük = oran × reference | 1536 px = 1024 × 1,5 |
| Taşma | bölge daraltılır, viewport **değiştirilmez** | 800 px genişlikte kaydırma |

**Kesin ölçü yazan şartname otomatik red olur** (`=` yalnız zorunlu toplamlarda kullanılır, §4.5 G9).

### §3.3 Layout Ağacı (ASCII — en fazla 66 sütun)

```text
+------------------------------------------------------------------+
| SP-01 · 01-start-page   [1024×600]   VERSA CODER — Baslangic    |
+------------------------------------------------------------------+
| ribbon (Dock=Top)          | yukseklik = 96 px                   |
| tab satiri                | yukseklik = 32 px                   |
| +-------------------------+------------------------------------+ |
| | SOL  genislik = 240 px  | ANA  genislik = 744 px             | |
| | +-- proje agaci (Tree)  | +-- kart alani (TileControl)       | |
| | +-- son kullanilanlar   | +-- hizli islem butonlari          | |
| +-------------------------+------------------------------------+ |
| durum cubugu (Dock=Bottom) | yukseklik = 32 px                  |
+------------------------------------------------------------------+
```

| Kural | Test |
|-------|------|
| Tüm kutular ` ```text ` içinde | Markdown dönüştürücü bozmasın |
| Satır uzunluğu ≤ 66 karakter | §5.3/1 = 0 satır |
| Her satırda `=` ile ölçü | §3.2 tablosu |
| Ebeveyn → çocuk girintisi 2 boşluk | ağaç okunur |
| Unicode kutu çizim (`U+2500-U+257F`) | **YASAK** — `-` `+` `|` kullan |

### §3.4 Kontrol Envanteri (ID · tip · binding · olay)

Şartnamenin çekirdeği bu tablodur; **her satır bir kontroldür**, boş hücre kabul edilmez.

| # | Kontrol ID | Tip (DevExpress 26.1) | Binding | Olay | Hedef katman |
|---|-----------|----------------------|---------|------|--------------|
| 1 | `{{ctlAd}}` | `TextEdit` | `{{Alan}}` | `Validating` | `l7 → l2` |
| 2 | `{{ctlDurum}}` | `ComboBoxEdit` | `{{Durum}}` | `SelectedValueChanged` | `l7 → l2` |
| 3 | `{{ctlListe}}` | `GridControl` | `bs{{Veri}}` | `FocusedRowChanged` | `l7 → l2` |
| 4 | `{{ctlAgaç}}` | `TreeList` | `{{Hiyerarsi}}` | `FocusedNodeChanged` | `l7 → l2` |
| 5 | `{{ctlKaydet}}` | `SimpleButton` | — | `Click` | `l7 → l2` |
| 6 | `{{ctlIptal}}` | `SimpleButton` | — | `Click` | `l7` (yerel) |
| 7 | `{{ctlDurumCub}}` | `LabelControl` | `{{DurumMetni}}` | — | `l7` (yerel) |
| 8 | `{{ctlAra}}` | `SearchControl` | `{{Filtre}}` | `TextChanged` | `l7 → l2` |

**Kurallar:** (a) Binding sütunu yalnızca `BindingSource` veri yolu yazar — SQL, tablo adı, `DbContext` **yazılmaz** (G2). (b) Olay sütunu boş olan kontrol salt görseldir, §3.6'da gerekçesi istenir. (c) Kontrol tipi `26.1` bileşen listesi dışındaysa §6.2 R6 tetiklenir. (d) Her kontrolün bir `TabIndex` değeri vardır (§3.6).

### §3.5 State Matrix (loading · empty · error · permission)

Dört durum **her ekranda zorunludur**; biri bile atlanırsa şartname eksiktir (§6.1 K11).

| Durum | Tetikleyen | Görsel Davranış | Kullanıcı Aksiyonu | Kilitlenme |
|-------|-----------|-----------------|--------------------|-----------|
| **loading** | handler `HandleAsync` başladı | `ProgressBar` görünür, grid devre dışı | `Esc` → iptal (flow §3) | yalnız grid |
| **empty** | sonuç 0 kayıt | `lblBosDurum` metni + tek eylem | "Yeni oluştur" → form | hayır |
| **error** | handler `failure` | `lblHata` + kod, ekran **açık kalır** | "Tekrar dene" / "Kopyala" | hayır |
| **permission** | rol yetkisiz | alan `Enabled=false` + açıklama | — | hayır, ekran okunur |

| Kurallar | Test |
|----------|------|
| 4 satırın hepsi dolu | §5.3/5 |
| error durumunda ekran kapanmaz | §3.7/6 (form-template) |
| permission yalnız rengi değil **yazıyı** da kullanır | WCAG 1.4.1 |
| loading ≤ 400 ms sonra görünür (öncesi flicker yok) | §6.5 K4 |

### §3.6 WCAG 2.2 AA, Klavye ve Sekme Sırası

| # | Kural | WCAG | Denetim |
|---|-------|------|---------|
| 1 | Metin/kontrast ≥ **4,5:1** (büyük metin 3:1) | 1.4.3 AA | `reference` hesap tablosu |
| 2 | Görsel odak halkası görünür (kalınlık ≥ 2 px) | 2.4.11 AA | gözlem |
| 3 | Her kontrolün adı + amacı (tooltip tek başına yeterli değil) | 4.1.2 A | okuyucu testi |
| 4 | Durum **yazıyla** da verilir (`HATALI`, `YETKİSİZ`) | 1.4.1 A | gözlem |
| 5 | Sekme sırası sol→üst, görsel okuma sırasıyla **aynı** | 2.4.3 A | §5.3/3 |
| 6 | Odak kaydırılabilir alan içinde kalır | 2.4.11 AA | tab gezinme |
| 7 | Modal açılışta odak modalin ilk öğesine, `Esc` kapatır | 2.1.2 A | klavye |
| 8 | Hata metni alana **yakın** ve `aria-live` nitelikli | 3.3.1 A | gözlem |
| 9 | Klavye kısayolu `Alt+{{X}}`, çakışma yok | 2.1.4 C | §6.5 K5 |
| 10 | Hedef boyut ≥ 24×24 px (mobil dokunma hariç masaüstü) | 2.5.8 AA | ölçü |

**Sekme sırası örneği (SP-01):** `§1 sekme satırı → §2 ağaç → §3 arama → §4 kart alanı → §5 hızlı işlem → §6 durum çubuğu (salt okunur, odaklanmaz)`.

### §3.7 Doğrulama Kuralları (saf kontrol — attribute yasak)

Doğrulama `l7`'de **kontrolün kendisinde** yaşar; kalıcı kural `l1`'dedir. `DataAnnotations` (`[Required]`, `[Range]`) forma **girmez** (G2 kardeş kuralı — `../frontend/form-template.md` §3.6).

| # | Kural | Nerede | Hata Gösterimi | Bloke Eder |
|---|-------|--------|----------------|-----------|
| 1 | Zorunlu alan boş mu | `Validating` | alan yanındaki `lbl{{X}}Error` | Kaydet |
| 2 | Uzunlık ≤ `{{N}}` | `TextChanged` | gri `lblDurum` + kırpma | Kaydet |
| 3 | Sayı aralığı `{{min}}..{{max}}` | `Validating` | `ErrorProvider` ikonu | Kaydet |
| 4 | Tarih mantığı (bitiş ≥ başlangıç) | `Validating` | kırmızı metin + açıklama | Kaydet |
| 5 | Dosya yolu var mı | `Click` öncesi | modal mesaj | İşlem |
| 6 | Kopya anahtar | handler `failure` | modal + odak alana | İşlem (sunucu) |
| 7 | Onay (silme/geri alma) | aksiyon öncesi | `YesNo` diyalog | İşlem |

**Sıra:** 1–4 anlık, 5–6 handler içinde, 7 aksiyon öncesi. Şartname asla "hepsi geçti" diye **varsaymaz**; handler `failure`'ı her zaman beklenir.

### §3.8 Boş ve Dolu Örnek

**Boş kalıp (yalnız bu blokta `{{…}}` yaşar — üretilen şartnamede adedi 0 olmalıdır):**

````markdown
# Şartname — {{SP_NN}} · {{EKAN_KODU}} · {{EKAN_ADI}}

| Alan | Değer |
|---|---|
| Ekran kodu / dosya | {{SP_NN}} · `.ai/ui-design/screen/{{NN-ekran}}.md` |
| Viewport | [1024×600] |
| Ana binding | {{UseCase}}Handler |
| GATE-003 | {{beklemede / onayli}} |

## Layout Ağacı
```text
{{ASCII_AGAC — en fazla 66 sutun}}
```

## Kontrol Envanteri
| # | ID | Tip | Binding | Olay | TabIndex |
|---|----|-----|---------|------|----------|
| 1 | {{ctl_1}} | {{Tip}} | {{Yol}} | {{Olay}} | 0 |

## State Matrix — 4/4 dolu
{{loading · empty · error · permission}}

## WCAG 2.2 AA + Sekme Sırası
{{10 madde + §1..§6 sirasi}}

## Doğrulama — 7 kural
{{§3.7 tablosu}}

## Acceptance Criteria — AC-01..AC-10
{{§3.9}}

## GATE-003 Onay Kutusu
{{§3.10}}
````

**Dolu örnek (SP-01 · 01-start-page):**

```text
+------------------------------------------------------------------+
| SP-01 · 01-start-page   [1024×600]   Baslangic Sayfasi           |
+------------------------------------------------------------------+
| kontrol envanteri (ozet)                                         |
|   1 tblProjects   TreeList    bsProjects   FocusedNodeChanged    |
|   2 txtAra        SearchCtrl  Filtre       TextChanged           |
|   3 tileNew       TileControl -            ItemClick             |
|   4 btnBaslat     SimpleButton -           Click                 |
|   5 btnIceAktar   SimpleButton -           Click                 |
|   6 lblDurum      LabelControl DurumMetni  -                     |
+------------------------------------------------------------------+
| durum matrisi                                                    |
|   loading  : ProgressPanel + grid Enabled=false, Esc = iptal     |
|   empty    : "Kayit yok" + <>Yeni Olustur                        |
|   error    : lblHata + kod, ekran ACIK, <>Tekrar Dene            |
|   permission: btnIceAktar Enabled=false + "Yetki yok" yazisi     |
+------------------------------------------------------------------+
| sekme sirasi: sekme > agac > arama > kart > hizli islem          |
+------------------------------------------------------------------+
```

**Ne anlatır:** Altı kontrolün tamamı ID + tip + binding + olay ile listelenmiş, dört durum dolu, sekme sırası yazılı. Renk/tema **yoktur** (G3).

### §3.9 Acceptance Criteria

Her şartname **tam 10** kabul kriteri ile kapanır; `AC-01`…`AC-10` numaralandırması değişmez.

| # | Kalıp | Örnek (SP-01) |
|---|-------|---------------|
| AC-01 | Ekran `[1024×600]` içinde taşmasız açılır | ribbon + grid sığar |
| AC-02 | İlk odak §1'dedir | ağaç odakta |
| AC-03 | 4 durumun hepsi tetiklenebilir | §3.5 |
| AC-04 | Boş durumda tek anlamlı eylem görünür | "Yeni oluştur" |
| AC-05 | Hata durumunda ekran **kapanmaz** | `lblHata` + kod |
| AC-06 | Sekme sırası okuma sırasıyla eş | §5.3/3 = 0 |
| AC-07 | Kontrast ≥ 4,5:1 | `reference` hesabı |
| AC-08 | 7 doğrulama kuralının hepsi tetiklenir | §3.7 |
| AC-09 | Kaydet → handler `failure` yolu kodlanmıştır | §3.7/6 |
| AC-10 | Şartname ile tel kafes bölgeleri birebir | `WF-NN` ↔ `SP-NN` |

### §3.10 GATE-003 Onay Kutusu

Şartnamenin **son bloğu** budur; kutular onay öncesi ` [ ] `, onay sonrası ` [x] ` olur.

```text
+------------------------------------------------------------------+
| GATE-003 — SP-01 onay kutusu                                     |
| [ ] 7/7 sartname dosyasi mevcut (01..07)                          |
| [ ] 7/7 tel kafes ile bolge eslesmesi (AC-10)                     |
| [ ] State matrix 4/4 dolu (AC-03)                                |
| [ ] WCAG 2.2 AA 10 madde tam (AC-06, AC-07)                      |
| [ ] Yer tutucu adedi 0 (G5)                                      |
| [ ] KULLANICI onayi                                              |
| Karar: [ ] GECTI  [ ] RED   tarih: {{YYYY-MM-DD}}                |
+------------------------------------------------------------------+
```

### §3.11 Tam Kalıp İskeleti (kopyala-yapıştır)

Yer tutucular yalnız §3.8 ve bu blokta yaşar; üretilen şartnamede adedi **0** olmalıdır (G5).

````markdown
# Şartname — {{SP_NN}} · {{EKAN_ADI}}

**Zorunlu Bağlantılar:** tel kafes `./{{NN-ekran}}.md` (bağlantı, `.ai/ui-design/` kökü)

| Alan | Değer |
|---|---|
| Ekran | {{SP_NN}} · {{EKAN_ADI}} · {{AMAC_CUMLESI}} |
| Viewport | [1024×600] — toplam {{1024 / 600}} |
| Katman | yalnız `l7` · binding → `{{UseCase}}Handler` (`l2`) |
| GATE-003 | {{beklemede}} |

## 1. Layout Ağacı (≤66 sütun)
{{ASCII}}

## 2. Kontrol Envanteri
{{ID · tip · binding · olay · TabIndex}}

## 3. State Matrix
{{loading · empty · error · permission}}

## 4. WCAG 2.2 AA + Sekme Sırası
{{10 madde}}

## 5. Doğrulama
{{7 kural}}

## 6. Acceptance Criteria
{{AC-01..AC-10}}

## 7. GATE-003 Onay Kutusu
{{§3.10}}
````

---

## §4 Kurallar

### §4.1 Yazım Sırası ve Öncelik

| Öncelik | Şart | Yapılacak |
|---------|------|-----------|
| 1 | Tel kafes (`WF-NN`) yoksa | **DUR** — şartname iskeletsiz yazılmaz |
| 2 | Ekran listesi (7) eksikse | 7/7 tamamla (§6.5 K2) |
| 3 | Kimlik satırı (§3.1) | dolu, tek cümle amaç |
| 4 | Viewport (§3.2) | `[1024×600]` + yatay/dikey toplam |
| 5 | Layout ağacı (§3.3) | ` ```text `, ≤66 |
| 6 | Kontrol envanteri (§3.4) | her satır dolu |
| 7 | State matrix (§3.5) | 4/4 |
| 8 | WCAG + sekme (§3.6) | 10 madde |
| 9 | Doğrulama (§3.7) + AC (§3.9) | 7 + 10 |
| 10 | Onay kutusu (§3.10) + ölçüm | §5.3 = 0 satır |

### §4.2 Sorumluluk ve Onay

| Aşama | Rol | Ne Onaylar | Çıktı | Süre |
|-------|-----|-----------|-------|------|
| 1 | ui-designer | 7 şartname + §5.3 çıktısı | `.ai/ui-design/screen/*.md` | 42 dk |
| 2 | reviewer | §6.1 (20 madde) | ✅ / RED | 12 dk |
| 3 | architect | binding ↔ katman (`l7→l2`) | ters referans yok | 8 dk |
| 4 | **kullanıcı (GATE-003)** | 7 tel kafes + 7 şartname | kapı kaydı | 20 dk |
| 5 | master-orchestrator | §5.4 Quality Report | `log.md` satırı | 6 dk |
| | **Toplam** | | | **≈ 88 dk** |

**3 RED** sonrası iş durur, gerekçe `master-orchestrator`'a yükseltilir. Kullanıcı onayı yoksa aşama 5'e geçilmez, `src/**` yasağı kalkmaz (§1.3).

### §4.3 Ölçek / Standartlar

| Öğe | Değer | Sapma |
|-----|-------|-------|
| Satır sayısı | ≥ 480 | < 480 = **RED** (G6) |
| Dosya boyutu | ≥ 27,0 KB | < 27,0 KB = RED |
| Viewport | 1024×600 | başka = RED (G9) |
| Kontrol satırı | 8–20 / ekran | > 30 → ekrana böl |
| State matrix | 4 satır | < 4 = RED |
| Kutu satırı | ≤ 66 karakter | > 66 = düzelt |
| `{{…}}` | 0 (kalıp blokları hariç) | > 0 = RED (G5) |
| Kodlama | UTF-8 BOM'suz | Latin-1/CP1254 otomatik RED |

### §4.4 Değişiklik Kontrolü

| Olay | Aksiyon | `updated` | Kayıt |
|------|---------|-----------|-------|
| Yalnız yazım/tooltip düzeltmesi | dosya içinde | değişmez | §5.3/4 yeniden |
| Kontrol eklendi/çıkarıldı | §3.4 + §3.6 + AC | artar | `log.md` tek satır |
| Binding yolu değişti | §3.4 + mimari inceleme | minor sürüm | architect onayı |
| Durum matrisi değişti | §3.5 + AC-03/05 | minor sürüm | reviewer |
| Ekran eklendi/çıkarıldı | 7/7 kuralı → kapı | artar | **kapı yeniden alınır** |
| Kapı sonrası şartname değişikliği | yalnız kapı üzerinden | patch | GATE-003 yeniden |

### §4.5 Hard Guardrails (Otomatik DUR)

| # | Guardrail | İhlal Sonucu |
|---|-----------|--------------|
| **G1** | Yasaklı vault (`C:\www\…`) cümlesi kopyalamak | **DUR** — sil, özgün yaz; yalnız yapı referansı |
| **G2** | EF Core / yıldızlı sorgu / forma SQL sızmak | **DUR** — ADO.NET + Dapper, `@p1..@pN`; katman `l4` |
| **G3** | Şartnamede hex renk, font adı, kontrast değeri | **DUR** — `reference` şablonuna taşı |
| **G4** | `src/**` veya gerçek ekran dosyasında kapı öncesi kod | **DUR** — §1.3, iş geri alınır |
| **G5** | Üretilen şartnamede `{{…}}` | **DUR** — kalıp doldurulur |
| **G6** | Satır < 480 ya da KB < 27,0 | **DUR** — §4.3 |
| **G7** | Kırık bağlantı (`.ai/index.md`, `.ai/CLAUDE.md`, `.ai/WORKFLOW.md`, `.ai/.rules/`, `.ai/.decisions/`, `.ai/architecture/`, `.ai/keys.md`, `.ai/log.md`, `.ai/ui-design/`, `.ai/reports/`) | **DUR** — bağlantıyı kaldır, metin olarak yaz |
| **G8** | `l7` dışında katman tarifi (Domain/Infrastructure detayı) | **DUR** — katman sınırı §3.4 |
| **G9** | `≈`sız kesin px ya da viewport `1024×600` dışı | **DUR** — §3.2 |
| **G10** | Sekme sırası / WCAG maddesi eksik | **DUR** — §3.6 10 madde |
| **G11** | Durum matrisinde eksik satır (4 < 4) | **DUR** — §3.5 |
| **G12** | Kanıtsız "onaylı / uygun" iddiası | **DUR** — §3.10 kutusu + §5.3 çıktısı istenir |

---

## §5 Workflow

### §5.1 Adım Bazlı Şartname Akışı

| # | Adım | Girdi | Çıktı | Yapan | Süre |
|---|------|-------|-------|-------|------|
| 1 | Ekran listesini doğrula | envanter §3.4 + tel kafesler | 7 dosya adı | ui-designer | 4 dk |
| 2 | Kimlik + viewport | §3.1 · §3.2 | başlık tablosu | ui-designer | 5 dk |
| 3 | Layout ağacı | tel kafes bölgeleri | ASCII ≤66 | ui-designer | 7 dk |
| 4 | Kontrol envanteri | §3.4 | 8–20 satır | ui-designer | 9 dk |
| 5 | State matrix | §3.5 | 4/4 | ui-designer | 6 dk |
| 6 | WCAG + sekme sırası | §3.6 | 10 madde | ui-designer | 6 dk |
| 7 | Doğrulama + AC | §3.7 · §3.9 | 7 + 10 | ui-designer | 7 dk |
| 8 | Onay kutusu | §3.10 | 6 kutu | ui-designer | 3 dk |
| 9 | Ölçüm | §5.3/1–6 | 0 satır | ui-designer | 4 dk |
| 10 | Denetim | §6.1 (20 madde) | ✅ / RED | reviewer | 12 dk |
| 11 | Kullanıcı onayı | 7 tel kafes + 7 şartname | **GATE-003** | kullanıcı → orchestrator | 20 dk |
| | **Toplam (7 şartname)** | | | | **≈ 83 dk** |

**Bütçe:** 1–8 ≈ 7 dk/ekran, 9–10 ≈ 16 dk/ekran → **≈ 23 dk/ekran**. Tek şartname 35 dakikayı aşarsa §2'ye dön: akış (`flow`) ya da renk (`reference`) içeriği sızmıştır.

### §5.2 Hata Modları

| # | Hata Modu | Belirti | Kök Neden | Düzeltme |
|---|-----------|---------|-----------|----------|
| 1 | Viewport kayması | toplam ≠ 1024 / 600 | bölgeler elle toplanmış | §3.2 toplam tablosu |
| 2 | Kesin px ile yazım | `= 238 px` | `≈` unutulmuş | yalnız toplamlarda `=` |
| 3 | Eksik durum | matrix 3/4 | permission atlanmış | §3.5 4 satır |
| 4 | Binding sızıntısı | sütunda tablo/SQL adı | katman karışmış | G2, `l7 → l2` |
| 5 | Kontrol tipi uydurma | 26.1 listesinde yok | sözlük atlanmış | DevExpress 26.1 adları |
| 6 | WCAG maddesi boş | 7/10 | denetim atlanmış | §3.6 10 satır |
| 7 | Yer tutucu sızıntısı | `{{CTL}}` yayımlanmış | §3.8 doldurulmadı | G5, §5.3/4 |
| 8 | Tel kafesle çelişki | bölge ≠ `WF-NN` | iki dosya ayrı yazılmış | AC-10, birlikte güncelle |
| 9 | Toplu dosya | `screen/all.md` | 1 dosya = 1 ekran kırmızı | 7 ayrı dosya |
| 10 | Kapı atlanmış | gerçek ekran dosyası kapı öncesi | §1.3 unutulmuş | **DUR** (G4), geri dön |

### §5.3 Salt Okunur Doğrulama Komutları

Altı komut da **hiçbir dosyayı değiştirmez**; hedef dizin yoksa dürüstçe `DIZIN YOK` yazar.

```powershell
# 1) YAPI — frontmatter 8 alan (scope dahil) + 7 bolum + zorunlu baglanti
$need='title','type','category','status','version','updated','authority','scope'
Get-ChildItem 'C:\versacoder\.ai\.templates\ui-design\*.md' | ForEach-Object {
  $r=[IO.File]::ReadAllText($_.FullName)
  $m=$need | Where-Object { $r -notmatch ('(?m)^'+$_+':') }
  $s=([regex]::Matches($r,'(?m)^## \u00A7')).Count
  $b=([regex]::Match($r,'(?m)^\*\*Zorunlu Ba\u011Flant\u0131lar:\*\*(.*)$')).Groups[1].Value
  if ($m) { 'EKSIK ALAN -> '+$_.Name+': '+($m -join ',') }
  if ($s -ne 7) { 'BOLUM -> '+$_.Name+': '+$s+'/7' }
  if ($b -notmatch '\]\(\.\.\/index\.md\)' -or $b -notmatch '\]\(\.\/wireframe-template\.md\)') {
    'BAGLANTI -> '+$_.Name } }

# 2) BAGLANTI HEDEFLERI DISKTE VAR MI (kirik link = 0; G7)
$base='C:\versacoder\.ai\.templates\ui-design'
Get-ChildItem "$base\*.md" | Select-String -Pattern '\]\((\.\.?\/[^)#]+)\)' -AllMatches |
  ForEach-Object { $_.Matches } | ForEach-Object { $_.Groups[1].Value } | Sort-Object -Unique |
  ForEach-Object { if (-not (Test-Path (Join-Path $base $_))) { 'KIRIK -> '+$_ } }

# 3) OLCU — satir >= 480 ve KB >= 27,0 (G6)
Get-ChildItem 'C:\versacoder\.ai\.templates\ui-design\*.md' | ForEach-Object {
  $n=(Get-Content $_.FullName).Count; $kb=[math]::Round($_.Length/1KB,1)
  if ($n -lt 480) { 'KISA -> {0}: {1} satir' -f $_.Name,$n }
  if ($kb -lt 27.0) { 'KUCUK -> {0}: {1} KB' -f $_.Name,$kb } }

# 4) YER TUTUCU + KODLAMA — {{...}} disinda 0, BOM/mojibake 0 (G5)
Get-ChildItem 'C:\versacoder\.ai\.templates\ui-design\*.md' | ForEach-Object {
  $b=[IO.File]::ReadAllBytes($_.FullName)
  if ($b.Length -ge 3 -and $b[0] -eq 0xEF -and $b[1] -eq 0xBB -and $b[2] -eq 0xBF) { 'BOM -> '+$_.Name }
  $t=[Text.Encoding]::UTF8.GetString($b)
  if ($t -match '\u00C3[\u0080-\u00BF]\u00C3[\u0080-\u00BF]|\uFFFD') { 'MOJIBAKE -> '+$_.Name }
  $in=$false; $i=0
  foreach ($l in ($t -split "`n")) { $i++
    if ($l -match '^`{3,}md') { $in=$true; continue }
    if ($l -match '^`{3,}' -and $in) { $in=$false; continue }
    if (-not $in -and $l -match '\{\{[A-Z0-9_]+\}\}') { 'YER TUTUCU -> {0}:{1}' -f $_.Name,$i } } }

# 5) YASAK ICERIK — EF/yildizli sorgu/renk hex'i = 0 (G2, G3)
Get-ChildItem 'C:\versacoder\.ai\.templates\ui-design\*.md' |
  Select-String -Pattern 'EntityFramework|DbConnection|SELECT\s+\*' |
  ForEach-Object { 'YASAK SQL/EF -> {0}:{1}' -f $_.Filename,$_.LineNumber }
Get-ChildItem 'C:\versacoder\.ai\.templates\ui-design\*.md' |
  Select-String -Pattern '#[0-9A-Fa-f]{6}\b' | ForEach-Object { 'HEX -> '+$_.Filename }

# 6) KUTU GENISLIGI — ```text bloklarinda 66 karakteri asan satir = 0
Get-ChildItem 'C:\versacoder\.ai\.templates\ui-design\*.md' | ForEach-Object {
  $box=$false; $i=0
  foreach ($l in [IO.File]::ReadAllLines($_.FullName,[Text.Encoding]::UTF8)) { $i++
    if ($l -match '^```text') { $box=$true; continue }
    if ($l -match '^`{3,}' -and $box) { $box=$false; continue }
    if ($box -and $l.Length -gt 66) { '{0}:{1} uzunluk={2} (>66)' -f $_.Name,$i,$l.Length } } }
```

**Geçme:** #1–#6 hepsi **0 satır uyarı** (dizin yoksa dürüst `DIZIN YOK`).

### §5.4 Quality Report

```markdown
## Quality Report — screen-spec (YYYY-MM-DD HH:MM)

| Metrik | Değer | Eşik | Durum |
|---|---|---|---|
| Ölçüm (bu şablon) | <satır> satır / <KB> KB | ≥ 480 · ≥ 27,0 | 🟢/🔴 |
| Üretilen şartname | n / 7 | 7 | 🟢/🔴 |
| Frontmatter 8 alan (§5.3/1) | n/8 | 8 | 🟢/🔴 |
| Kırık bağlantı (§5.3/2) | n | 0 | 🟢/🔴 |
| Yer tutucu / BOM / mojibake | n/n/n | 0/0/0 | 🟢/🔴 |
| EF / yıldızlı sorgu / hex | n/n/n | 0/0/0 | 🟢/🔴 |
| Checklist §6.1 | n/20 | 20/20 | 🟢/🔴 |
| Guardrail G1..G12 | ihlal n | 0 | 🟢/🔴 |
| GATE-003 | açık / onaylı | kullanıcı onayı | 🟢/🔴 |

**Karar:** 🟢 kapıya sunulur / 🔴 düzeltmeye gönderildi (<gerekçe>)
```

---

## §6 Doğrulama

### §6.1 Kontrol Listesi (20 madde)

| # | Kontrol | Beklenen | İlgili |
|---|---------|----------|--------|
| K1 | Frontmatter 8 alan (`title…scope`) | 8/8 | §5.3/1 |
| K2 | `authority: SSOT` birebir | `SSOT` | §3.7 (envanter) |
| K3 | H1 + `**Zorunlu Bağlantılar:**` (index · wireframe) | 1 satır | §5.3/1 |
| K4 | `## §1…§7` | 7/7 | §5.3/1 |
| K5 | `§1.1–§1.4` dolu | 4/4 | §1 |
| K6 | `§1.3` şablon muafiyeti ayrımı yazılı | var | G4 |
| K7 | `§2.1` eşleme + `§2.2` sınır testi | 2/2 | §2 |
| K8 | `§3.1–§3.11` tam (11 alt bölüm) | 11/11 | §3 |
| K9 | Viewport `1024×600` + toplam hesabı | 2 toplam | §3.2 |
| K10 | Layout ağacı ` ```text ` + ≤66 | 0 sapma | §3.3 · §5.3/6 |
| K11 | State matrix 4/4 dolu | 4 | §3.5 |
| K12 | WCAG 10 madde + sekme sırası | 10 | §3.6 |
| K13 | Doğrulama 7 kural + DataAnnotations yasak | 7 | §3.7 |
| K14 | Boş kalıp + dolu örnek birlikte | 2 | §3.8 |
| K15 | Acceptance criteria AC-01..AC-10 | 10 | §3.9 |
| K16 | GATE-003 onay kutusu bloğu | 6 kutu | §3.10 |
| K17 | `§4.1–§4.5` + G1..G12 | 5 + 12 | §4.5 |
| K18 | `§5.1–§5.4` (11 adım · ≥8 hata modu · 6 komut · report) | 4/4 | §5 |
| K19 | `§6.1–§6.5` | 5/5 | §6 |
| K20 | `§7.1–§7.2` (Red Team ≥ 10 satır) + ≥ 480 satır / ≥ 27 KB | 2/2 · 2/2 | G6 |

### §6.2 Red Sinyalleri (Otomatik DUR)

| # | Sinyal | Aksiyon |
|---|--------|---------|
| R1 | §5.3 komutlarından biri > 0 satır | **DUR** — gösterilen dosya düzeltilir |
| R2 | Viewport ≠ `1024×600` ya da toplam tutmuyor | **DUR** — §3.2 |
| R3 | State matrix < 4 satır | **DUR** — §3.5 |
| R4 | Binding sütununda SQL / tablo / `DbContext` | **DUR** — G2, `l7 → l2` |
| R5 | Şartnamede hex renk / font adı | **DUR** — `reference` şablonuna taşı |
| R6 | DevExpress `26.1` dışı bileşen adı | **DUR** — ADR-001 |
| R7 | Yer tutucu yayımlanmış | **DUR** — G5 |
| R8 | Kırık bağlantı | **DUR** — G7, bağlantı kaldırılır |
| R9 | Kapı öncesi gerçek ekran dosyası / `src/**` kodu | **DUR** — G4, §1.3 |
| R10 | Mojibake / BOM / karışık dil | **DUR** — UTF-8 BOM'suz yeniden yaz |

### §6.3 Kapı ve Geçiş Kuralları

| Geçiş | Koşul (hepsi `EVET`) | Onaylayan | Etki |
|-------|----------------------|------------|------|
| taslak → inceleme | 7/7 şartname · §5.3 = 0 · §6.1 20/20 | ui-designer → reviewer | kapıya sunulur |
| inceleme → **onay** | §6.2 = 0 · §5.4 `log.md`'de | **kullanıcı** | `GATE-003` kaydı |
| onay → kod | kapı kaydı mevcut | orchestrator | `src/**` ve `../frontend/form-template.md` serbest |
| onay → revizyon | bölgesel değişiklik talebi | ui-designer | kapı **yeniden** alınır |
| taslak → geri dönüş | §1.1'e sızma (renk/akış/kod) | architect | Part C içinde kalır |

Kapı yalnız ileri açılır; onay sonrası değişiklik kapı üzerinden yapılır, kod içinden şartname düzeltilemez (G4).

### §6.4 İlgili Dosyalar

| Dosya | İlişki |
|-------|--------|
| [./wireframe-template.md](./wireframe-template.md) | **ön koşul** — şartname bu iskeletin teknik karşılığı |
| [../index.md](../index.md) | envanter satır 26 · §3.7 kalite sözleşmesi · §4.5 G1..G12 |
| [../frontend/form-template.md](../frontend/form-template.md) | şartnamedeki binding/olay kodda burada üretilir |
| `./flow-template.md` | olay → sonuç dalları bu şablonda değil, `flow`'da |
| `./reference-template.md` | renk/kontrast/token burada yasak (G3) |
| `./prompt-template.md` | şartnameden prompt üretilir (kapı muafiyeti §1.3) |
| `.ai/.templates/architecture/ascii-diagram-template.md` | kutu alfabesi (bağlantı verilmez, yalnız metin) |

### §6.5 Envantere Özgü Kontroller

| # | Kontrol | Beklenen | Kanıt |
|---|---------|----------|-------|
| K1 | Bu şablon satır aralığı | ≥ 480 | `Get-Content` + `Count` |
| K2 | `.ai/ui-design/screen/` dosya sayısı (üretim sonrası) | 7 | §5.3 benzeri `Test-Path` |
| K3 | Bu şablon KB (envanter: `16,0`) | ≥ 27,0 | `(Get-Item …).Length / 1KB` |
| K4 | Loading gecikmesi ≥ 400 ms kuralı yazılı | var | §3.5 |
| K5 | Klavye kısayolu çakışma kontrolü yazılı | var | §3.6/9 |
| K6 | Yasaklı vault adı geçen satır | 0 | G1 |
| K7 | `SELECT *` geçen satır (yalnız tarama kalıbında) | 1 (§5.3/5) | G2 |

---

## §7 Referanslar

### §7.1 VersaCoder Bağlantıları

| Kaynak | Yol / Bağlantı | Ne İçin | Durum (2026-10-05) |
|--------|----------------|---------|---------------------|
| Şablon envanteri | [../index.md](../index.md) | §3.4 satır 26 · §3.7 · §4.5 | ✅ okundu |
| Kardeş (altın örneği) | [./wireframe-template.md](./wireframe-template.md) | biçim referansı | ✅ 642 satır / 34,5 KB |
| Form karşılığı | [../frontend/form-template.md](../frontend/form-template.md) | binding + doğrulama kodu | ✅ okundu |
| DevExpress kilidi | [../adr/adr-devexpress-template.md](../adr/adr-devexpress-template.md) | sürüm `26.1` | ✅ ADR şablonu |
| Mimari ADR | [../adr/adr-architecture-template.md](../adr/adr-architecture-template.md) | katman `l0..l7` | ✅ |
| Veri ADR | [../adr/adr-data-template.md](../adr/adr-data-template.md) | ADO.NET + Dapper | ✅ G2 |
| Sorgu şablonu | [../query/Query-Template.md](../query/Query-Template.md) | `@p1..@pN` kuralı | ✅ |
| Kök belgeler | [../../../CLAUDE.md](../../../CLAUDE.md) · [../../../WORKFLOW.md](../../../WORKFLOW.md) | G8 · kapı zinciri | ✅ diskte var |
| `.ai/index.md` · `.ai/WORKFLOW.md` · `.ai/ui-design/` | **metin** (bağlantı yok) | kapı/bölüm referansı | ❌ YOK → ⚠️ |
| Yasaklı vault'lar | `C:\www\…` (envanter G1) | yalnız **yapı** referansı | 🔒 kopyalama YASAK |

### §7.2 Red Team — Sık Yapılan Hatalar

| # | Hata | Neden Yanlış | Doğrusu |
|---|------|--------------|---------|
| 1 | Şablonu yazmak için GATE-003 beklemek | şablon kapı muafidir, Part C kilitlenir | §1.3 tablosu |
| 2 | Kapıdan önce `.ai/ui-design/screen/*.md` yazmak | G4 ihlali, iş geri alınır | §1.3 `[2]` satırı |
| 3 | Viewport'u 1920×1080 yapmak | iki reference = iki gerçek | §3.2 `1024×600` |
| 4 | `≈` yazmadan kesin px vermek | ölçüm tartışması açılır | yalnız toplamlarda `=` |
| 5 | Binding sütununa SQL koymak | katman ihlali, gizli bağımlılık | `l7 → l2`, G2 |
| 6 | Durum matrisini 3 satıra indirmek | permission/empty kaçar | §3.5 4/4 |
| 7 | Renk/kontrast hesabını şartnameye koymak | iki gerçek kaynağı | `reference` şablonu |
| 8 | Akış adımlarını şartnameye yazmak | SRP ihlali, iki şablon aynı işi yapar | `flow` şablonu |
| 9 | `.ai/index.md`'ye bağlantı vermek | diskte yok, kırık link (G7) | metin olarak yaz |
| 10 | `{{CTL}}` ile onaya sunmak | iskelet onaylanıyor | G5, §5.3/4 = 0 |
| 11 | 7 ekranı tek dosyada toplamak | kapı takibi kırılır | 1 dosya = 1 ekran |
| 12 | "Uygun" diye kanıtsız onay yazmak | kanıt yoksa iddia yok | §3.10 kutusu + §5.3 |

**SSUT beyanı:** Ekran şartnamesi biçiminin **tek** kaynağı bu dosyadır; tel kafes, flow, reference, prompt ve frontend şablonları yalnız buna bağlanır. Kopyalanan her satır ikinci gerçek kaynağı doğurur (envanter G12).
