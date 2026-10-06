---
title: "VersaCoder — Tasarım Referansı (Reference) Şablonu"
type: template
category: templates/ui-design
status: active
version: 1.0.0
updated: 2026-10-05
authority: SSOT
scope: "tasarim referansi: design token, tipografi, renk, spacing, ikon, kontrol varyantlari, responsive kirilimlar ve tekrar kalipalari"
---

# VersaCoder — Tasarım Referansı (Reference) Şablonu

**Zorunlu Bağlantılar:** [index](../index.md) · [wireframe](./wireframe-template.md)

---

## §1 Amaç

Bu şablon `.ai/ui-design/reference/` altındaki **tasarım referansı dosyalarını** tek biçimde yazdırır: design token sözlüğü, tipografi ölçeği, renk paleti + kontrast hesapları, spacing/ızgara, ikon seti, DevExpress kontrol varyantları, responsive kırılım noktaları, "şu ekran şuna benzer" eşlemesi ve dört tekrar kalıbı (master-detail · liste · form · onay). **Renk ve kontrastın tek kaynağı burasıdır** — tel kafeste, şartnamede ve prompt'ta hex yoktur (wireframe G3).

| Alan | Değer |
|------|-------|
| Template Name / Path | `reference-template.md` · `.ai/.templates/ui-design/` |
| Hedef Dosya | `.ai/ui-design/reference/*.md` |
| Tür | `T-UX` (envanter Part C, satır #28) |
| Token Adı | `--vc-<alan>-<rol>` (SCSS/CSS değişkeni) |
| Hex Bu Dosyada | ✅ **izinli** — başka hiçbir ui-design dosyasında yasak (§3.4) |
| Kodlama | **UTF-8 (BOM'suz)** — Latin-1/CP1254 YASAK |
| Kapı | `GATE-003` — referans **kapı muafidir** (§1.3) |
| Dil / Versiyon / Authority | Türkçe (ç ğ ı İ ö ş ü) · 1.0.0 · **SSOT** · 2026-10-05 |

### §1.1 Referans Ne İşe Yarar / Ne Değildir?

| YAPILIR | YAPMAZ |
|---------|--------|
| Token adı → değer → kullanım yeri eşlemesi | Ekranın yerleşimi (→ `wireframe`) |
| Tipografi ölçeği + satır yüksekliği + harf aralığı | Kontrol envanteri (→ `screen-spec`) |
| Renk paleti + **kontrast oranı** hesabı (AA) | Akış ve hata dalları (→ `flow`) |
| Spacing ölçeği + ızgara + responsive kırılım | Kod (`*.cs`, CSS dosyası) |
| Kontrol varyantları (DevExpress 26.1) | Paket sürümü kararı (→ ADR-001) |
| Tekrar kalıpları (master-detail, liste, form, onay) | Prompt metni (→ `prompt`) |

### §1.2 Disk Kanıtları ve Boşluk (YAGNI)

İddia → kanıt; kanıt yoksa uydurulmaz. Ölçüm: `C:\versacoder`, 2026-10-05.

| # | İddia | Durum | Kanıt |
|---|-------|-------|-------|
| 1 | `.ai/ui-design/reference/*.md` | ❌ YOK | `Test-Path = False`, 0 dosya |
| 2 | `.ai/.templates/ui-design/reference-template.md` | ❌ YOK | envanter #28 `PLANLI · 10,0` |
| 3 | `.ai/.templates/ui-design/wireframe-template.md` | ✅ DOĞRULANDI | 642 satır / 34,5 KB |
| 4 | `.ai/.templates/index.md` §3.4 satır 218 | ✅ OKUNDU | hedef `.ai/ui-design/reference/*.md` |
| 5 | `.ai/.templates/architecture/ascii-diagram-template.md` | ✅ DOĞRULANDI | kutu biçimi (bağlantı verilmez) |
| 6 | `.ai/keys.md` · `.ai/.rules/` · `.ai/log.md` | ❌ YOK | envanter beyanı → ⚠️ VERIFICATION REQUIRED |
| 7 | `.ai/ui-design/` (üretim hedefi) | ❌ YOK | 0 dosya |
| 8 | `src/**` (token kod karşılığı) | ❌ YOK | `src/` = `Test-Path False` |
| 9 | Mevcut tema/SCSS dosyası | ❌ YOK | token'lar **bu dosyada tanım kaynağı** |
| 10 | Harici vault `C:\www\…` (envanter G1) | 🔒 ERİŞİM YASAK | yalnız **yapı** referansı, palet kopyalanmaz |

**Yorum yasağı:** Var olmayan dosya için "şurada tanımlı" denmez; `⚠️ VERIFICATION REQUIRED` yazılır. `.ai/index.md`, `.ai/CLAUDE.md`, `.ai/WORKFLOW.md`, `.ai/.rules/`, `.ai/.decisions/`, `.ai/architecture/`, `.ai/keys.md`, `.ai/log.md`, `.ai/ui-design/`, `.ai/reports/` yolları **kırık sayılır**; metin olarak değinilir, bağlantı verilmez.

### §1.3 GATE-003 Kapsamı — Referans Muafiyeti

Kapı yasağı **yalnız gerçek ekran dosyasını ve `src/**` kodunu** kapsar. Şablon ve referans dosyaları bu kapsamın dışındadır; kök `CLAUDE.md` G8 "ekran/kod üretilmez" der — **renk/token tarifi ne ekran ne koddur.**

```text
+------------------------------------------------------------------+
| [1] bu dosya (Sablon)          KAPI MUAF     -- yazilabilir       |
|     v                                                            |
| [2] .ai/ui-design/reference/   KAPI MUAF     -- token/tarif       |
|     v                                                            |
| [3] .ai/ui-design/01..07 + screen/  KAPI GEREKIR -- onay         |
|     v                                                            |
| [4] src/** + CSS/SCSS kodu     KAPI SONRASI  -- onaysiz yasak     |
+------------------------------------------------------------------+
```

| Öğe | GATE-003 kapsamı | Bu şablon |
|-----|------------------|-----------|
| `.ai/.templates/ui-design/reference-template.md` | ❌ kapsam dışı | ✅ serbest |
| `.ai/ui-design/reference/<konu>.md` | ❌ kapsam dışı | ✅ serbest |
| `.ai/ui-design/01…07` tel kafes + `screen/*.md` | ✅ kapsar | ⛔ kapı öncesi yazılmaz |
| `src/**` · `*.css` · `*.scss` | ✅ kapsar | ⛔ kapı öncesi yazılmaz |

**Kural:** Referans kapıdan bağımsız yazılır; ama **değeri uygulanan** yüzey (ekran/kod) kapıya bağlıdır (§6.3).

### §1.4 Neden Tek Kaynaklı Referans?

| Boşluk | Belirti | Bu Şablonun Kestiği Şey |
|--------|---------|--------------------------|
| İki farklı gri | 4 yerde 4 ayrı `#777` | §3.4 tek palet + rol |
| Kontrast sonradan ölçülür | AA ihlali, geri iş | §3.4 kontrast sütunu |
| Font ölçeği keyfi | 13/14/15 px karışması | §3.3 ölçek tablosu |
| Her ekranda farklı boşluk | hizasızlık şikayeti | §3.5 spacing ölçeği |
| Aynı panel 3 farklı yerde | tekrar + sapma | §3.10 dört kalıp |

---

## §2 Kapsam

**İçinde:** token sözlüğü, tipografi, renk + kontrast, spacing/ızgara, ikon, kontrol varyantları, responsive kırılım noktaları, ekran benzerlik eşlemesi, tekrar kalıpları, salt-okunur doğrulama.
**Dışında:** tel kafes, ekran şartnamesi, akış, prompt, `*.cs`/`*.scss` kodu, paket sürümü, iş kuralı.

| Kapsam | Kapsam Dışı |
|--------|-------------|
| `--vc-*` token adı + değer + kullanım yeri | CSS/SCSS dosyası (→ `src/**`, kapı sonrası) |
| Kontrast oranı hesabı (≥ 4,5:1 / 3:1) | Ekran yerleşimi (→ `./wireframe-template.md`) |
| Tipografi ölçeği + satır yüksekliği | Kontrol ID'si + binding (→ `screen-spec`) |
| Spacing + grid + `1024×600` kırılımları | Zamanlama/timeout (→ `flow`) |
| DevExpress 26.1 kontrol varyantı | Sürüm kurulumu (→ [../adr/adr-devexpress-template.md](../adr/adr-devexpress-template.md)) |
| Benzerlik eşlemesi + 4 tekrar kalıbı | Prompt/metin üretimi (→ `prompt`) |

### §2.1 Dosya Tipi → Sorumlu Eşlemesi

| Dosya Tipi | Hedef | Sorumlu | Bu Şablondan mı |
|------------|-------|---------|------------------|
| `.ai/ui-design/reference/<konu>.md` | token/tipo/renk/ikon | ui-designer | ✅ **bu şablon** |
| `.ai/ui-design/01…07-*.md` | tel kafes (hex yasak) | ui-designer | ❌ → `./wireframe-template.md` |
| `.ai/ui-design/screen/*.md` | şartname (hex yasak) | ui-designer | ❌ → `screen-spec` |
| `.ai/ui-design/flow/*.md` | akış (hex yasak) | ui-designer | ❌ → `flow` |
| `.ai/ui-design/prompts/*.md` | üretim promptu | ui-designer | ❌ → `prompt` |
| `src/**/Styles/*.scss` | token'ın kodu | developer | ❌ GATE-003 sonrası |
| `src/**/XForm.cs` | kontrol varyantı uygulaması | developer | ❌ → [../frontend/form-template.md](../frontend/form-template.md) |
| `tests/**` | kontrast otomasyonu | testing | ❌ → [../testing/test-plan-template.md](../testing/test-plan-template.md) |

### §2.2 Kapsam Sınırı Testi

Üretilecek dosyaya sırayla sor; **ilk `EVET`** şablonu belirler:

1. **Renk / tipografi / token / ikon / boşluk / varyant** mı? → **bu şablon**.
2. Ekranın **yerleşimi** mi? → `./wireframe-template.md`.
3. Ekranın **kontrol + durum dökümü** mı? → `screen-spec`.
4. **Başlangıç → adım → bitiş** mi? → `flow`.
5. **Agent/model komutu** mu? → `prompt`.
6. **`.cs` / `.scss`** mi → ❌ kapı geçmeden üretilmez (§1.3).

`HAYIR` soruları: yasaklı vault paleti mi (G1)? · EF Core / yıldızlı sorgu var mı (G2)? · Kontrast hesabı **olmadan** renk verildi mi (G9)? · Aynı token'ın iki farklı değeri mi var (G10)?

---

## §3 Mimari

Bu bölüm hem **şablonun yapısını** hem **üretilecek referans dosyasının yapısını** tanımlar. Kutular ` ```text ` içinde ve **≤ 66 sütun** yazılır.

### §3.1 Dosya ve Referans Kimliği

| Alan | Kural | Doğru | Yanlış |
|------|-------|-------|--------|
| Dosya adı | `.ai/ui-design/reference/<konu>.md` | `reference/colors.md` | `reference/renkler-1.md` |
| Referans kodu | `RF-NN` | `RF-03` | `R3` |
| Konu | tek odak | "Renk ve kontrast" | "Genel tasarım" |
| Token sayısı | konu başına | `12–40` | 3 (eksik) / 120 (şişkin) |
| Sahip | tek rol | `ui-designer` | belirsiz |
| İlgili ekranlar | `SP-NN` listesi | `SP-01, SP-02` | — |
| Kapı | muafiyet satırı | `GATE-003: referans muaf (§1.3)` | satır yok |
| Son güncelleme | `updated` alanı | ISO tarih | — |

### §3.2 Design Token Sözlüğü

Token adı kalıbı: `--vc-<alan>-<rol>`; SCSS karşılığı `$vc-<alan>-<rol>`. **İsimlendirme değişmez**, değer değişebilir.

| # | Token | Değer | Rol / Kullanım Yeri | İlgili Ekran |
|---|-------|-------|---------------------|--------------|
| 1 | `--vc-color-bg-app` | `#F5F6F8` | uygulama zemini | tümü |
| 2 | `--vc-color-bg-surface` | `#FFFFFF` | panel/yüzey zemini | SP-01, SP-04 |
| 3 | `--vc-color-text-primary` | `#1B1F27` | birincil metin | tümü |
| 4 | `--vc-color-text-muted` | `#5B6472` | ikincil metin | SP-01 |
| 5 | `--vc-color-border` | `#D6DAE1` | ayraç/kenarlık | tümü |
| 6 | `--vc-color-accent` | `#2F6FED` | vurgu / odak | SP-01, SP-03 |
| 7 | `--vc-color-danger` | `#C0392B` | hata metni + ikon | SP-02 |
| 8 | `--vc-color-success` | `#1E7F4F` | başarı durumu | SP-03 |
| 9 | `--vc-color-warning` | `#B7791F` | uyarı durumu | SP-05 |
| 10 | `--vc-color-focus-ring` | `#2F6FED` | odak halkası (2 px) | tümü |
| 11 | `--vc-space-1 … --vc-space-6` | `4, 8, 12, 16, 24, 32` px | boşluk ölçeği | tümü |
| 12 | `--vc-radius-md` | `4 px` | köşe yarıçapı | tümü |

**Kurallar:** (a) Her token'ın **tek** değeri vardır (G10). (b) Değer `src/**` içinde **yeniden tanımlanmaz**. (c) Yeni token için önce bu tabloya satır eklenir, sonra kod yazılır. (d) Kullanım yeri sütunu boş bırakılamaz.

### §3.3 Tipografi

| # | Rol | Token / Aile | Boyut | Satır | Ağırlık | Kullanım |
|---|-----|--------------|-------|-------|---------|----------|
| T1 | Başlık H1 | `--vc-font-ui` | 20 px | 28 px | 600 | pencere başlığı |
| T2 | Başlık H2 | `--vc-font-ui` | 16 px | 24 px | 600 | panel başlığı |
| T3 | Gövde | `--vc-font-ui` | 13 px | 20 px | 400 | form/metin |
| T4 | Yardımcı | `--vc-font-ui` | 12 px | 16 px | 400 | ipucu, durum |
| T5 | Monospace | `--vc-font-mono` | 13 px | 20 px | 400 | kod, sorgu |
| T6 | Etiket | `--vc-font-ui` | 12 px | 16 px | 600 | alan etiketi |
| T7 | Sayı (tablo) | `--vc-font-mono` | 13 px | 20 px | 400 | sağa hizalı sayı |

| Kural | Değer |
|-------|-------|
| Yazı ailesi | `Segoe UI` (varsayılan) · `Consolas` (mono) — ikisi de sistemde var |
| Ölçek oranı | ≈ 1,22 (12 → 13 → 16 → 20) |
| Minimum boyut | **12 px** (daha küçüğü RED) |
| Satır yüksekliği | ≥ 1,35 × boyut |
| Harf aralığı | `0` (mono kod için `0`) |
| Vurgu | **kalınlık** ya da renk — **altı çizili** yalnız bağlantıda |

### §3.4 Renk ve Kontrast (WCAG 2.2 AA)

**Bu dosya hex'in tek evidir.** Tel kafes, şartname, flow ve prompt'ta hex **yasak** (wireframe G3).

| # | Ön | Arkış | Oran | Eşik | Kullanım |
|---|----|-------|------|------|----------|
| C1 | `#1B1F27` | `#FFFFFF` | 15,8:1 | 4,5:1 ✅ | birincil metin |
| C2 | `#5B6472` | `#FFFFFF` | 6,1:1 | 4,5:1 ✅ | ikincil metin |
| C3 | `#5B6472` | `#F5F6F8` | 5,6:1 | 4,5:1 ✅ | zemin üstü metin |
| C4 | `#FFFFFF` | `#2F6FED` | 4,7:1 | 4,5:1 ✅ | vurgu butonu |
| C5 | `#FFFFFF` | `#C0392B` | 5,3:1 | 4,5:1 ✅ | hata butonu |
| C6 | `#1B1F27` | `#F5F6F8` | 14,3:1 | 4,5:1 ✅ | gövde |
| C7 | `#2F6FED` | `#FFFFFF` | 4,7:1 | 3:1 ✅ | odak halkası (UI) |
| C8 | `#D6DAE1` | `#FFFFFF` | 1,4:1 | 3:1 ❌ | **sadece dekoratif ayraç** |

| Kural | Değer / Test |
|-------|--------------|
| Metin kontrastı | ≥ **4,5:1** (18 px bold / 24 px üstü: 3:1) |
| UI bileşeni + odak | ≥ **3:1** (WCAG 2.2 §1.4.11) |
| Renk tek başına bilgi vermez | durum **yazıyla** da (`HATALI`, `YETKİSİZ`) — §1.4.1 |
| Odak halkası | ≥ 2 px kalınlık, `--vc-color-focus-ring` |
| Hesap | oran elle yazılmaz; §5.3/5 ile **kanıtlanır** |
| C8 uyarısı | dekoratif ayraç metin taşımaz; metin taşıyan her çizgi ≥ 3:1 |

### §3.5 Spacing, Izgara ve Responsive Kırılımlar

**Spacing ölçeği:** `4 · 8 · 12 · 16 · 24 · 32` px — ara değer (`6 px`, `10 px`) **yasak**.

| Kırılım | Genişlik | Davranış | Etkilenen bölge |
|---------|----------|----------|-----------------|
| B1 — minimum | **1024 px** | reference: iki sütun | tüm ekranlar |
| B2 — dar | < 1024 px | sol panel **240 → 200 px** | SP-01, SP-02 |
| B3 — çok dar | < 900 px | sol panel gizlenir, butonla açılır | SP-01 |
| B4 — geniş | ≥ 1440 px | ana alan **maks. 1200 px**, ortalanır | SP-06 |
| B5 — DPI %150 | 1024 → 1536 | ölçek ×1,5, **orantı korunur** | tümü |

| Izgara | Değer |
|--------|-------|
| Sütun | 12 sütun · oluk `16 px` · kenar boşluğu `40 px` |
| Yatay toplam (B1) | `40 + 240 + 16 + 744 + 40 = 1080` → 1024'te `16 px` oluk düşer, toplam `1024` |
| Dikey toplam | `96 (ribbon) + 32 (tab) + 440 (gövde) + 32 (durum) = 600` |
| Min dokunma/hedef | `24 × 24 px` (WCAG 2.2 §2.5.8) |

### §3.6 İkon

| # | İkon | Kaynak | Boyut | Anlam | Renk |
|---|------|--------|-------|-------|------|
| I1 | kaydet | DevExpress SVG | 16 px | eylem | `--vc-color-accent` |
| I2 | ekle | DevExpress SVG | 16 px | eylem | `--vc-color-accent` |
| I3 | sil | DevExpress SVG | 16 px | yıkıcı | `--vc-color-danger` |
| I4 | uyarı | DevExpress SVG | 16 px | durum | `--vc-color-warning` |
| I5 | onay | DevExpress SVG | 16 px | durum | `--vc-color-success` |
| I6 | bilgi | DevExpress SVG | 16 px | durum | `--vc-color-text-muted` |

| Kural | Değer |
|-------|-------|
| Set | DevExpress **26.1** SVG seti (ADR-001 kilidi) — özel çizim yok |
| Boyut | 16 px (gövde) · 20 px (başlık) — iki boyut |
| Emoji | **YASAK** (2 code unit, hizalama bozar) |
| Metinli ikon | yalnız eylem ikonuna **yazı + tooltip** eşlik eder |
| Renk-körü | ikon tek başına anlam taşımaz; yanına **metin** (I6) |

### §3.7 Kontrol Varyantları (DevExpress 26.1)

| Kontrol | Varyant 1 | Varyant 2 | Varyant 3 | Kullanım Yeri |
|---------|-----------|-----------|-----------|---------------|
| `SimpleButton` | `Primary` (accent) | `Normal` | `Danger` (kırmızı) | Kaydet · İptal · Sil |
| `TextEdit` | normal | `ReadOnly` + gri zemin | `ErrorProvider` | form alanları |
| `ComboBoxEdit` | normal | `Disabled` | çoklu seçim | filtre |
| `GridControl` | normal | `Loading` (satır yok) | `Empty` (mesaj satırı) | liste |
| `CheckEdit` | işaretli | işaretsiz | `Indeterminate` | ayar |
| `ProgressBar` | belirsiz (loading) | belirli (%) | hata (duraklat) | işlem |
| `LabelControl` | normal | `Muted` | `Danger` | durum / hata |

| Kural | Değer |
|-------|-------|
| Sürüm | **26.1** tek sürüm (ADR-001) |
| LookAndFeel | tek noktadan; kontrol başına tema **DEĞİL** |
| Varyant seçimi | primary yalnız ekranda **1 adet** |
| `Disabled` görünümü | gri + açıklama metni (yalnız renk değil) |

### §3.8 "Şu Ekran Şuna Benzer" Eşlemesi

Yeni ekran yazılırken **önce** bu tabloya bakılır; benzerlik yoksa yeni kalıp §3.10'da tanımlanır.

| # | Ekran | Benzediği Ekran | Ortak Kalıp | Fark |
|---|-------|-----------------|-------------|------|
| 1 | `SP-01` başlangıç | — (kendisi kalıp) | **kart + ağaç** | — |
| 2 | `SP-02` düzenleyici | `SP-01` | ağaç + iki sütun | ortada editör |
| 3 | `SP-03` önizleme | `SP-06` doküman | sekme + içerik | önizleme motoru |
| 4 | `SP-04` ayarlar | `SP-05` temalar | **liste + ayrıntı** | alan sayısı |
| 5 | `SP-05` temalar | `SP-04` ayarlar | liste + ayrıntı | önizleme kutusu |
| 6 | `SP-06` dokümantasyon | `SP-02` | ağaç + sekme | arama alanı |
| 7 | `SP-07` hakkında | — (kendisi kalıp) | **modal + sekme** | — |

### §3.9 Tekrar Kalıpları

Dört kalıp; her yeni ekran bu dörtten birine oturur (oturmuyorsa §6.2 R4 tetiklenir).

**P1 — Master-Detail (liste + ayrıntı):**

```text
+------------------------------------------------------------------+
| P1 MASTER-DETAIL     [1024x600]                                   |
+-------------------------------+----------------------------------+
| LISTE (sol) genislik = 320 px | DETAY (sag) [AKISKAN]            |
| [x] satir 1                   | alan etiketi: deger              |
|     satir 2                   | alan etiketi: deger              |
|     satir 3                   | <Kaydet>  <Vazgec>               |
+-------------------------------+----------------------------------+
| odak: liste > (OK) > detay alanlari > butonlar                    |
+------------------------------------------------------------------+
```

**P2 — Liste (tek sütun tablo):** başlık satırı (sıralanabilir) · satır (24 px) · boş durum mesajı · sağ üstte arama · satır başına tek eylem.
**P3 — Form (gruplu alanlar):** grup başlığı (H2) · etiket (T6) + alan (T3) · hata metni alanı yanında · altta birincil + ikincil buton.
**P4 — Onay (modal + perde):** odak modal içinde, `Esc` = vazgeç, birincil buton **sağda**, yıkıcı işlem `Danger`, perde arkası etkileşimsiz.

| Kalıp | Zorunlu Öğe | Sıklık |
|-------|-------------|--------|
| P1 master-detail | ana liste + takip eden detay + senkron oku | 2 ekran |
| P2 liste | sıralama + boş durum + arama | 3 ekran |
| P3 form | etiket + alan + hata + 2 buton | 4 ekran |
| P4 onay | perde + odak tuzağı yok + `Esc` | her yıkıcı işlem |

### §3.10 Dolu Örnek ve Tam Kalıp

**Dolu örnek — `RF-02 · Tema paleti`:**

```text
+------------------------------------------------------------------+
| RF-02 · Tema Paleti ve Kontrast                                   |
+------------------------------------------------------------------+
| token                     deger     on   esik   durum            |
| --vc-color-text-primary   #1B1F27  15.8  4.5    OK               |
| --vc-color-text-muted     #5B6472   6.1  4.5    OK               |
| --vc-color-accent         #2F6FED   4.7  4.5    OK               |
| --vc-color-danger         #C0392B   5.3  4.5    OK               |
| --vc-color-border         #D6DAE1   1.4  3.0    DEKORATIF ONLY   |
+------------------------------------------------------------------+
| olcek: 12 > 13 > 16 > 20 px   |  spacing: 4 8 12 16 24 32        |
| kirilim: B1 1024 | B2 <1024 | B3 <900 | B4 >=1440 | B5 DPI 150    |
+------------------------------------------------------------------+
```

**Tam kalıp (yalnız bu blokta `{{…}}` yaşar; üretilen referansta adedi 0 olmalıdır — G5):**

````markdown
# Referans — {{RF_NN}} · {{KONU_ADI}}

| Alan | Değer |
|---|---|
| Referans / dosya | {{RF_NN}} · `.ai/ui-design/reference/{{konu}}.md` |
| İlgili ekranlar | {{SP_NN_LISTESI}} |
| Token adedi | {{N}} |
| Kapı | GATE-003: referans muaf (§1.3) |

## Token Sözlüğü (§3.2)
| # | Token | Değer | Rol | İlgili ekran |

## Tipografi (§3.3)
{{T1..T7}}

## Renk ve Kontrast (§3.4)
| # | Ön | Arkış | Oran | Eşik | Durum |

## Spacing + Kırılım (§3.5)
{{4 8 12 16 24 32 · B1..B5}}

## İkon (§3.6) · Varyantlar (§3.7)
{{I1..I6 · kontrol varyantları}}

## Benzerlik Eşlemesi (§3.8) + Kalıp (§3.9)
{{şu ekran şuna benzer + P1..P4}}
````

### §3.11 Bağımlılık ve Katman Sınırı

| Konu | Kural |
|------|-------|
| Katman | Referans **yalnız `l7` (UI+CLI)** içindir; `l0..l6` katmanlarında renk/token **yaşamaz** |
| Ok yönü | `l7 → l6 → l5 → l4 → l2 → l1 → l0`; `l3` yalnız `l0`'a bakar; `l2 ↔ l4` doğrudan ok **YOK** |
| Token akışı | token → `l7` bileşeni → **kod**; `l2`/`l1`'e token **sızmaz** |
| Veri | token tablosu veri **değildir**; sorgu/EF **yok** (G2) |
| DevExpress | varyant adları `26.1` bileşenleri (ADR-001) |
| Prompt | referans `prompt` şablonuna **girdi** olur; tersi yasak |

---

## §4 Kurallar

### §4.1 Yazım Sırası ve Öncelik

| Öncelik | Şart | Yapılacak |
|---------|------|-----------|
| 1 | Ekran listesi yoksa | §3.8'e `⚠️` satırı, referans yine yazılır |
| 2 | Kimlik (§3.1) | 8 alan dolu |
| 3 | Token sözlüğü (§3.2) | ad + değer + rol + ekran |
| 4 | Tipografi (§3.3) | T1..T7, min 12 px |
| 5 | Renk + kontrast (§3.4) | her satırda oran + eşik |
| 6 | Spacing + kırılım (§3.5) | 6 olçü + B1..B5 |
| 7 | İkon + varyant (§3.6–§3.7) | 6 ikon + 7 kontrol |
| 8 | Benzerlik + kalıp (§3.8–§3.9) | 7 satır + P1..P4 |
| 9 | Ölçüm | §5.3 = 0 satır |
| 10 | Denetim | §6.1 = 20/20 |

### §4.2 Sorumluluk ve Onay

| Aşama | Rol | Ne Onaylar | Çıktı | Süre |
|-------|-----|-----------|-------|------|
| 1 | ui-designer | referans dosyaları + §5.3 çıktısı | `.ai/ui-design/reference/*.md` | 30 dk |
| 2 | reviewer | §6.1 (20 madde) | ✅ / RED | 10 dk |
| 3 | architect | token ↔ `l7` sınırı + ADR-001 | katman tutarlılığı | 6 dk |
| 4 | **kullanıcı (GATE-003)** | 7 tel kafes + 7 şartname (ref muaf) | kapı kaydı | 20 dk |
| 5 | master-orchestrator | §5.4 Quality Report | `log.md` satırı | 6 dk |
| | **Toplam** | | | **≈ 72 dk** |

**3 RED** sonrası iş durur, gerekçe `master-orchestrator`'a yükseltilir.

### §4.3 Ölçek / Standartlar

| Öğe | Değer | Sapma |
|-----|-------|-------|
| Satır sayısı | ≥ 480 | < 480 = **RED** (G6) |
| Dosya boyutu | ≥ 27,0 KB | < 27,0 KB = RED |
| Token / konu | 12–40 | > 60 → iki konuya böl |
| Tipografi satırı | 7 (T1..T7) | < 6 = RED |
| Kontrast satırı | ≥ 6, her biri oranlı | oranızsız satır = RED |
| Spacing ara değer | **0** | `6 px` / `10 px` = RED |
| Kutu satırı | ≤ 66 karakter | > 66 = düzelt |
| Kodlama | UTF-8 BOM'suz | Latin-1/CP1254 otomatik RED |

### §4.4 Değişiklik Kontrolü

| Olay | Aksiyon | `updated` | Kayıt |
|------|---------|-----------|-------|
| Yalnız açıklama düzeltmesi | dosya içinde | değişmez | §5.3 yeniden |
| Token değeri değişti | §3.2 + §3.4 + ilgili ekranlar | artar | `log.md` + mimariye bildirim |
| Yeni token eklendi | §3.2 + rol + kullanım | minor sürüm | reviewer |
| Kontrast düşürüldü | §3.4 hesap yeniden | **minor + otomatik RED** | G9 |
| Kontrol varyantı değişti | §3.7 + ADR-001 kontrolü | minor sürüm | architect |
| Ekran eklendi/çıkarıldı | §3.8 eşlemesi | artar | kapı kaydı |

### §4.5 Hard Guardrails (Otomatik DUR)

| # | Guardrail | İhlal Sonucu |
|---|-----------|--------------|
| **G1** | Yasaklı vault (`C:\www\…`) palet/cümlesi kopyalamak | **DUR** — sil, özgün yaz; yalnız yapı referansı |
| **G2** | EF Core / yıldızlı sorgu / veritabanı bağlantısı | **DUR** — ADO.NET + Dapper; token tablosu veri değildir |
| **G3** | Hex'in bu dosya dışına taşması (tel kafes/şartname/flow/prompt) | **DUR** — hex yalnız burada |
| **G4** | Kapı öncesi `src/**` · `*.scss` kodu ya da gerçek ekran dosyası | **DUR** — §1.3 |
| **G5** | Üretilen referansta `{{…}}` | **DUR** — §3.10 kalbı doldurulur |
| **G6** | Satır < 480 ya da KB < 27,0 | **DUR** — §4.3 |
| **G7** | Kırık bağlantı (`.ai/index.md`, `.ai/CLAUDE.md`, `.ai/WORKFLOW.md`, `.ai/.rules/`, `.ai/.decisions/`, `.ai/architecture/`, `.ai/keys.md`, `.ai/log.md`, `.ai/ui-design/`, `.ai/reports/`) | **DUR** — bağlantı kaldırılır, metin yazılır |
| **G8** | Token'ın `l7` dışı katmanması ya da `l2↔l4` oku | **DUR** — §3.11 |
| **G9** | Kontrast oranı yazılmadan renk vermek / eşik altı değer | **DUR** — §3.4 + §5.3/5 |
| **G10** | Aynı token'ın iki değeri | **DUR** — §3.2 tek değer kuralı |
| **G11** | Emoji · emoji yerine geçen sembol · sistemde olmayan font | **DUR** — §3.3 · §3.6 |
| **G12** | Kanıtsız "AA uygun" iddiası | **DUR** — §5.3/5 çıktısı istenir |

---

## §5 Workflow

### §5.1 Adım Bazlı Referans Yazımı

| # | Adım | Girdi | Çıktı | Yapan | Süre |
|---|------|-------|-------|-------|------|
| 1 | Konu listesi | envanter §3.4 + ekranlar | 3–5 `RF-NN` | ui-designer | 4 dk |
| 2 | Kimlik | §3.1 | başlık tablosu | ui-designer | 3 dk |
| 3 | Token sözlüğü | §3.2 | 12–40 satır | ui-designer | 8 dk |
| 4 | Tipografi | §3.3 | T1..T7 | ui-designer | 5 dk |
| 5 | Renk + kontrast | §3.4 | ≥6 oran satırı | ui-designer | 9 dk |
| 6 | Spacing + kırılım | §3.5 | 6 ölçü + B1..B5 | ui-designer | 5 dk |
| 7 | İkon + varyant | §3.6–§3.7 | 6 + 7 satır | ui-designer | 5 dk |
| 8 | Benzerlik + kalıp | §3.8–§3.9 | 7 + P1..P4 | ui-designer | 6 dk |
| 9 | Ölçüm | §5.3/1–6 | 0 satır | ui-designer | 4 dk |
| 10 | Denetim | §6.1 (20 madde) | ✅ / RED | reviewer | 10 dk |
| | **Toplam** | | | | **≈ 59 dk** |

**Bütçe:** 1–8 ≈ 45 dk, 9–10 ≈ 14 dk. Tek konu 30 dakikayı aşarsa §2.2'ye dön: ekran yerleşimi ya da akış tartışması sızmıştır.

### §5.2 Hata Modları

| # | Hata Modu | Belirti | Kök Neden | Düzeltme |
|---|-----------|---------|-----------|----------|
| 1 | Kontratsız renk | oran sütunu boş | hesap atlanmış | §3.4 + §5.3/5 |
| 2 | İki gerçek değer | aynı token 2 hex | iki kişi yazmış | G10, tek satır |
| 3 | Ara değer spacing | `6 px`, `10 px` | göz kararı | §3.5 ölçeği |
| 4 | Hex sızması | tel kafes/şartnamede hex | G3 karıştırılmış | hex yalnız burada |
| 5 | Font uydurma | sistemde olmayan font | kurulum yok | §3.3 `Segoe UI`/`Consolas` |
| 6 | Emoji ikon | `✅` `🎨` | konfor eklentisi | DevExpress SVG (§3.6) |
| 7 | Varyant şişkinliği | primary 3 adet | hiyerarşi yok | §3.7 birincil = 1 |
| 8 | Kırılım eksik | yalnız 1024 | responsive yazılmamış | §3.5 B1..B5 |
| 9 | Yer tutucu sızıntısı | `{{TOKEN}}` | §3.10 doldurulmadı | G5 |
| 10 | Benzerlik yok | yeni ekran kalıpsız | §3.8 atlanmış | §3.9 + yeni kalıp kaydı |

### §5.3 Salt Okunur Doğrulama Komutları

Altı komut da **hiçbir dosyayı değiştirmez**; dizin yoksa dürüstçe `DIZIN YOK` yazar.

```powershell
# 1) YAPI — frontmatter 8 alan (scope) + 7 bolum + zorunlu baglanti
$need='title','type','category','status','version','updated','authority','scope'
Get-ChildItem 'C:\versacoder\.ai\.templates\ui-design\*.md' | ForEach-Object {
  $r=[IO.File]::ReadAllText($_.FullName)
  $m=$need | Where-Object { $r -notmatch ('(?m)^'+$_+':') }
  $s=([regex]::Matches($r,'(?m)^## \u00A7')).Count
  $b=([regex]::Match($r,'(?m)^\*\*Zorunlu Ba\u011Flant\u0131lar:\*\*(.*)$')).Groups[1].Value
  if ($m) { 'EKSIK ALAN -> '+$_.Name+': '+($m - join ',') }
  if ($s -ne 7) { 'BOLUM -> '+$_.Name+': '+$s+'/7' }
  if ($b -notmatch '\]\(\.\.\/index\.md\)' -or $b -notmatch '\]\(\.\/wireframe-template\.md\)') {
    'BAGLANTI -> '+$_.Name } }

# 2) BAGLANTI HEDEFLERI — kirik link = 0 (G7)
$base='C:\versacoder\.ai\.templates\ui-design'
Get-ChildItem "$base\*.md" | Select-String -Pattern '\]\((\.\.?\/[^)#]+)\)' -AllMatches |
  ForEach-Object { $_.Matches } | ForEach-Object { $_.Groups[1].Value } | Sort-Object -Unique |
  ForEach-Object { if (-not (Test-Path (Join-Path $base $_))) { 'KIRIK -> '+$_ } }

# 3) OLCU — satir >= 480, KB >= 27,0, kutu <= 66 (G6)
Get-ChildItem 'C:\versacoder\.ai\.templates\ui-design\*.md' | ForEach-Object {
  $n=(Get-Content $_.FullName).Count; $kb=[math]::Round($_.Length/1KB,1)
  if ($n -lt 480) { 'KISA -> {0}: {1} satir' -f $_.Name,$n }
  if ($kb -lt 27.0) { 'KUCUK -> {0}: {1} KB' -f $_.Name,$kb }
  $box=$false; $i=0
  foreach ($l in [IO.File]::ReadAllLines($_.FullName,[Text.Encoding]::UTF8)) { $i++
    if ($l -match '^```text') { $box=$true; continue }
    if ($l -match '^`{3,}' -and $box) { $box=$false; continue }
    if ($box -and $l.Length -gt 66) { '{0}:{1} kutu={2} (>66)' -f $_.Name,$i,$l.Length } } }

# 4) HEX MUAFIYETI — hex yalniz reference-template'de (G3)
Get-ChildItem 'C:\versacoder\.ai\.templates\ui-design\*.md' |
  Select-String -Pattern '#[0-9A-Fa-f]{6}\b' |
  Where-Object { $_.Filename -ne 'reference-template.md' } |
  ForEach-Object { 'HEX TASMASI -> {0}:{1}' -f $_.Filename,$_.LineNumber }

# 5) KONTRAST — her renk satirinda "oran" hucresi var mi (G9)
$r='C:\versacoder\.ai\.templates\ui-design\reference-template.md'
$renk=$false; $oran=0; $satir=0
foreach ($l in [IO.File]::ReadAllLines($r,[Text.Encoding]::UTF8)) {
  if ($l -match '^## \u00A73\.4') { $renk=$true; continue }
  if ($renk -and $l -match '^## ') { $renk=$false }
  if ($renk -and $l -match '^\| C\d' -and $l -match '\d+,\d+:1|\d+\.\d+:1') { $oran++ }
  if ($renk -and $l -match '^\| C\d') { $satir++ } }
if ($satir -gt 0 -and $oran -ne $satir) { 'KONTRAST EKSIK -> {0}/{1} satir' -f $oran,$satir }

# 6) YER TUTUCU + KODLAMA — BOM/mojibake 0 (G5)
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
```

**Geçme:** #1–#6 hepsi **0 satır uyarı** (#4'te `reference-template.md` muaf, #5'te oran = satır).

### §5.4 Quality Report

```markdown
## Quality Report — reference (YYYY-MM-DD HH:MM)

| Metrik | Değer | Eşik | Durum |
|---|---|---|---|
| Ölçüm (bu şablon) | <satır> satır / <KB> KB | ≥ 480 · ≥ 27,0 | 🟢/🔴 |
| Üretilen referans dosyası | n | ≥ 3 | 🟢/🔴 |
| Frontmatter 8 alan (§5.3/1) | n/8 | 8 | 🟢/🔴 |
| Kırık bağlantı (§5.3/2) | n | 0 | 🟢/🔴 |
| Kutu > 66 (§5.3/3) | n | 0 | 🟢/🔴 |
| Hex taşıması (§5.3/4) | n | 0 | 🟢/🔴 |
| Kontrast oranı eksik (§5.3/5) | n | 0 | 🟢/🔴 |
| Yer tutucu / BOM / mojibake | n/n/n | 0/0/0 | 🟢/🔴 |
| Checklist §6.1 | n/20 | 20/20 | 🟢/🔴 |
| Guardrail G1..G12 | ihlal n | 0 | 🟢/🔴 |

**Karar:** 🟢 kapıya sunulur / 🔴 düzeltmeye gönderildi (<gerekçe>)
```

---

## §6 Doğrulama

### §6.1 Kontrol Listesi (20 madde)

| # | Kontrol | Beklenen | İlgili |
|---|---------|----------|--------|
| K1 | Frontmatter 8 alan (`title…scope`) | 8/8 | §5.3/1 |
| K2 | `authority: SSOT` birebir | `SSOT` | envanter §3.7 |
| K3 | H1 + `**Zorunlu Bağlantılar:**` (index · wireframe) | 1 satır | §5.3/1 |
| K4 | `## §1…§7` | 7/7 | §5.3/1 |
| K5 | `§1.1–§1.4` dolu | 4/4 | §1 |
| K6 | `§1.3` referans muafiyeti yazılı | var | G4 |
| K7 | `§2.1` eşleme + `§2.2` sınır testi | 2/2 | §2 |
| K8 | `§3.1–§3.11` tam (11 alt bölüm) | 11/11 | §3 |
| K9 | Token adı `--vc-<alan>-<rol>` kalıbında | 0 sapma | §3.2 |
| K10 | Tipografi T1..T7 + min 12 px | 7 | §3.3 |
| K11 | Kontrast satırları oranlı, ≥6 | 6 | §3.4 · §5.3/5 |
| K12 | Spacing ara değeri 0 | 0 | §3.5 |
| K13 | Kırılım B1..B5 yazılı | 5 | §3.5 |
| K14 | İkon 6 + emoji 0 | 6/0 | §3.6 |
| K15 | Kontrol varyantları 7 satır + `26.1` | 7 | §3.7 |
| K16 | Benzerlik eşlemesi 7 satır + P1..P4 | 7 + 4 | §3.8–§3.9 |
| K17 | `§4.1–§4.5` + G1..G12 | 5 + 12 | §4.5 |
| K18 | `§5.1–§5.4` (10 adım · ≥8 hata modu · 6 komut · report) | 4/4 | §5 |
| K19 | `§6.1–§6.5` | 5/5 | §6 |
| K20 | `§7.1–§7.2` (Red Team ≥ 10 satır) + ≥ 480 satır / ≥ 27 KB | 2/2 · 2/2 | G6 |

### §6.2 Red Sinyalleri (Otomatik DUR)

| # | Sinyal | Aksiyon |
|---|--------|---------|
| R1 | §5.3 komutlarından biri > 0 satır | **DUR** — gösterilen dosya düzeltilir |
| R2 | Kontrast satırında oran yok / eşik altı | **DUR** — G9, hesap yeniden |
| R3 | Aynı token iki değerle | **DUR** — G10 |
| R4 | Yeni ekran dört kalıptan birine oturmuyor | **DUR** — §3.9'a kalıp eklenir |
| R5 | Tel kafes/şartname/flow/prompt'ta hex | **DUR** — G3, hex buraya taşınır |
| R6 | Ara spacing (`6 px`, `10 px`) | **DUR** — §3.5 ölçeği |
| R7 | Emoji ikon / sistemde olmayan font | **DUR** — §3.6 · §3.3 |
| R8 | Yer tutucu / kırık bağlantı | **DUR** — G5 · G7 |
| R9 | `26.1` dışı kontrol/varyant adı | **DUR** — ADR-001 |
| R10 | Kapı öncisi `src/**` · `*.scss` kodu | **DUR** — G4, §1.3 |

### §6.3 Kapı ve Geçiş Kuralları

| Geçiş | Koşul (hepsi `EVET`) | Onaylayan | Etki |
|-------|----------------------|------------|------|
| taslak → inceleme | §5.3 = 0 · §6.1 20/20 | ui-designer → reviewer | mimariye sunulur |
| inceleme → onay | §6.2 = 0 · §5.4 `log.md`'de | architect | referans dondurulur |
| referans → GATE-003 paketi | 7 tel kafes + 7 şartname 7/7 | **kullanıcı** | kapı kaydı |
| kapı → kod | `GATE-003` kaydı mevcut | orchestrator | `src/**` + `.scss` serbest |
| onay → revizyon | token/renk değişikliği | ui-designer | kapı **yeniden** alınır |

Referans tek başına kapı **açmaz**; kapı paketindedir. Kapı öncesi referans **zorunludur** — renksiz şartname, renksiz prompt yazılamaz.

### §6.4 İlgili Dosyalar

| Dosya | İlişki |
|-------|--------|
| [./wireframe-template.md](./wireframe-template.md) | hex'in yasak olduğu kardeş; G3 bu dosyaya işaret eder |
| [../index.md](../index.md) | envanter satır 28 · §3.7 · §4.5 G1..G12 |
| [../frontend/form-template.md](../frontend/form-template.md) | varyant + odak kodda burada uygulanır |
| [../adr/adr-devexpress-template.md](../adr/adr-devexpress-template.md) | `26.1` kilidi (§3.6–§3.7) |
| [../adr/adr-architecture-template.md](../adr/adr-architecture-template.md) | katman `l0..l7` sınırı (§3.11) |
| [../agents/agents-template.md](../agents/agents-template.md) | rollerin kaynağı (§4.2) |
| [../documentation/root-md-template.md](../documentation/root-md-template.md) | kök belge biçimi (bu şablonun başlığı) |
| `./screen-spec-template.md` · `./flow-template.md` · `./prompt-template.md` | kardeşler — bağlantı verilmez, metin |

### §6.5 Envantere Özgü Kontroller

| # | Kontrol | Beklenen | Kanıt |
|---|---------|----------|-------|
| K1 | Bu şablon satır aralığı | ≥ 480 | `Get-Content` + `Count` |
| K2 | Bu şablon KB (envanter: `10,0`) | ≥ 27,0 | `(Get-Item …).Length / 1KB` |
| K3 | Hex yalnız `reference-template.md` (§5.3/4) | 0 taşıma | tarama |
| K4 | Kontrast oranı eksik satır | 0 | §5.3/5 |
| K5 | Token tekrarı (aynı ad 2 satır) | 0 | elle / tarama |
| K6 | Yasaklı vault adı/paleti geçen satır | 0 | G1 |
| K7 | `.ai/ui-design/reference/` dosya sayısı (üretim sonrası) | ≥ 3 | `Test-Path` |

---

## §7 Referanslar

### §7.1 VersaCoder Bağlantıları

| Kaynak | Yol / Bağlantı | Ne İçin | Durum (2026-10-05) |
|--------|----------------|---------|---------------------|
| Şablon envanteri | [../index.md](../index.md) | §3.4 satır 28 · §3.7 · G1..G12 | ✅ okundu |
| Kardeş (altın örneği) | [./wireframe-template.md](./wireframe-template.md) | biçim referansı + G3 kaynağı | ✅ 642 satır / 34,5 KB |
| Form karşılığı | [../frontend/form-template.md](../frontend/form-template.md) | varyant/odak kodu | ✅ okundu |
| DevExpress kilidi | [../adr/adr-devexpress-template.md](../adr/adr-devexpress-template.md) | `26.1` + SVG ikon seti | ✅ |
| Mimari ADR | [../adr/adr-architecture-template.md](../adr/adr-architecture-template.md) | `l0..l7` sınırı | ✅ |
| Mimari şablon | [../architecture/architecture-template.md](../architecture/architecture-template.md) | katman tanımı | ✅ |
| Ajan rolleri | [../agents/agents-template.md](../agents/agents-template.md) | sorumlu roller | ✅ |
| Test planı | [../testing/test-plan-template.md](../testing/test-plan-template.md) | kontrast otomasyonu | ✅ |
| Kök belgeler | [../../../CLAUDE.md](../../../CLAUDE.md) · [../../../README.md](../../../README.md) | G8 · ürün adı | ✅ diskte var |
| `.ai/index.md` · `.ai/keys.md` · `.ai/.rules/` | **metin** (bağlantı yok) | kural referansı | ❌ YOK → ⚠️ |
| Yasaklı vault'lar | `C:\www\…` (envanter G1) | yalnız **yapı** referansı | 🔒 palet kopyalanmaz |

### §7.2 Red Team — Sık Yapılan Hatalar

| # | Hata | Neden Yanlış | Doğrusu |
|---|------|--------------|---------|
| 1 | Tel kafese hex koymak | G3 ihlali, onay yavaşlar | hex yalnız bu şablonda (§3.4) |
| 2 | Kontrast hesaplamadan renk vermek | AA ihlali, geri iş | §3.4 + §5.3/5 |
| 3 | Aynı gri için 2 farklı hex | iki gerçek kaynağı | G10, tek token |
| 4 | `6 px` / `10 px` ara değer | ölçek bozulur | §3.5: 4 8 12 16 24 32 |
| 5 | Emoji ile durum göstermek | 2 code unit, kırılgan | DevExpress SVG + metin |
| 6 | Font olarak `Roboto` yazmak | sistemde yok, DPI kayması | `Segoe UI` / `Consolas` |
| 7 | Primary butonu 3 yere koymak | hiyerarşi yok | §3.7 birincil = 1 |
| 8 | Responsive kırılım yazmamak | 1024 altında taşar | §3.5 B1..B5 |
| 9 | Yeni ekranı kalıpsız çizmek | her ekranda farklı yapı | §3.8 + §3.9 P1..P4 |
| 10 | Token'ı `l2`'ye koymak | katman ihlali, kod sızması | §3.11 yalnız `l7` |
| 11 | `.ai/index.md`'ye bağlantı vermek | diskte yok, kırık link (G7) | metin olarak yaz |
| 12 | Kapı bekleyip referansı yazmamak | şartname/prompt renksiz kalır | §1.3 muafiyet tablosu |

**SSUT beyanı:** Tasarım referansının **tek** kaynağı bu dosyadır; tel kafes, screen-spec, flow, prompt ve frontend şablonları yalnız buna bağlanır. Kopyalanan her satır ikinci gerçek kaynağı doğurur (envanter G12).
