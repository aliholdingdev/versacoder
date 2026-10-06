---
title: "VersaCoder — UI Üretim Promptu Şablonu"
type: template
category: templates/ui-design
status: active
version: 1.0.0
updated: 2026-10-05
authority: SSOT
scope: "UI uretim promptu: baglam blogu, kisitlar (DevExpress yalniz l7, ITCSS/BEM), kontrol listesi, cikti sozlesmesi ve yasaklar"
---

# VersaCoder — UI Üretim Promptu Şablonu

**Zorunlu Bağlantılar:** [index](../index.md) · [wireframe](./wireframe-template.md)

---

## §1 Amaç

Bu şablon `.ai/ui-design/prompts/` altındaki **UI üretim promptlarını** tek biçimde yazdırır: bağlam bloğu, kısıtlar (DevExpress **yalnız `l7`**, ITCSS/BEM), görev tarifi, kontrol listesi, çıktı sözleşmesi, doğrulama adımları ve yasaklar (UI'dan domain/servis sızması). Prompt bir **makineye verilen emir** değil, **insan + model ortak çalışmasının sözleşmesidir**: girdileri tarif eder, çıktıyı bağlar, reddetme ölçütünü önceden yazar.

| Alan | Değer |
|------|-------|
| Template Name / Path | `prompt-template.md` · `.ai/.templates/ui-design/` |
| Hedef Dosya | `.ai/ui-design/prompts/*.md` |
| Tür | `T-UX` (envanter Part C, satır #29) |
| Girdi Bağları | `screen-spec` · `wireframe` · `reference` · `flow` |
| Çıktı | kod **DEĞİL** — üretim tarifi + doğrulama listesi |
| Kodlama | **UTF-8 (BOM'suz)** — Latin-1/CP1254 YASAK |
| Kapı | `GATE-003` — prompt **kapı muafidir**; ürettirdiği **kod** kapıya bağlı (§1.3) |
| Dil / Versiyon / Authority | Türkçe (ç ğ ı İ ö ş ü) · 1.0.0 · **SSOT** · 2026-10-05 |

### §1.1 Prompt Ne İşe Yarar / Ne Değildir?

| YAPILIR | YAPMAZ |
|---------|--------|
| Hangi dosyaların girdi olduğunu **bağlantıyla** verir | Kendi başına doğruluk iddiası taşır |
| Kısıt listesi (katman, CSS yöntemi, sürüm) | Ürün adı/versiyon değiştirme yetkisi |
| Çıktı sözleşmesi (dosya adı, biçim, uzunluk) | Kaynak kodun kendisini içerir |
| Reddedilme ölçütü (ne olursa RED) | Gözleme dayalı "uygun" iddiası |
| Yasaklar (domain/servis/SQL sızması) | `l1`/`l2` içine müdahale |
| Doğrulama adımları (ölçüm + komut) | Kapı onayı yerine geçme |

### §1.2 Disk Kanıtları ve Boşluk (YAGNI)

İddia → kanıt; kanıt yoksa uydurulmaz. Ölçüm: `C:\versacoder`, 2026-10-05.

| # | İddia | Durum | Kanıt |
|---|-------|-------|-------|
| 1 | `.ai/ui-design/prompts/*.md` | ❌ YOK | `Test-Path = False`, 0 dosya |
| 2 | `.ai/.templates/ui-design/prompt-template.md` | ❌ YOK | envanter #29 `PLANLI · 9,0` |
| 3 | `.ai/.templates/ui-design/wireframe-template.md` | ✅ DOĞRULANDI | 642 satır / 34,5 KB |
| 4 | `.ai/.templates/index.md` §3.4 satır 219 | ✅ OKUNDU | hedef `.ai/ui-design/prompts/*.md` |
| 5 | `.ai/.templates/architecture/ascii-diagram-template.md` | ✅ DOĞRULANDI | kutu biçimi (bağlantı verilmez) |
| 6 | `.ai/.rules/` · `.ai/WORKFLOW.md` · `.ai/keys.md` | ❌ YOK | envanter beyanı → ⚠️ VERIFICATION REQUIRED |
| 7 | `.ai/ui-design/{screen,flow,reference}/` | ❌ YOK | prompt **girdileri henüz yok** → ⚠️ |
| 8 | `GATE-003` kayıt satırı | ❌ YOK → ⚠️ | kapı henüz alınmadı |
| 9 | `src/**` (prompt'un hedef kodu) | ❌ YOK | `src/` = `Test-Path False` |
| 10 | Harici vault `C:\www\…` (envanter G1) | 🔒 ERİŞİM YASAK | yalnız **yapı** referansı, prompt kopyalanmaz |

**Yorum yasağı:** Var olmayan dosya için "şurada tanımlı" denmez; `⚠️ VERIFICATION REQUIRED` yazılır. `.ai/index.md`, `.ai/CLAUDE.md`, `.ai/WORKFLOW.md`, `.ai/.rules/`, `.ai/.decisions/`, `.ai/architecture/`, `.ai/keys.md`, `.ai/log.md`, `.ai/ui-design/`, `.ai/reports/` yolları **kırık sayılır**; metin olarak değinilir, bağlantı verilmez.

### §1.3 GATE-003 Kapsamı — Prompt ve Üretim Ayrımı

Kapı yasağı **yalnız gerçek ekran dosyasını ve `src/**` kodunu** kapsar. Prompt dosyası **bu kapsamın dışındadır**. Ancak prompt'un **ürettirdiği** kod `src/**` olduğu için kapı öncesi **çalıştırılmaz** — yazılmaz demek değil, uygulanmaz demek.

```text
+------------------------------------------------------------------+
| [1] bu dosya (Sablon)          KAPI MUAF   -- yazilabilir         |
|     v                                                            |
| [2] .ai/ui-design/prompts/*.md KAPI MUAF   -- prompt yazilir      |
|     v                                                            |
| [3] girdi paketi: screen + flow + reference  KAPI GEREKIR         |
|     v                                                            |
| [4] prompt CALISTIRILIR        KAPI SONRASI -- urunen kod src/**  |
+------------------------------------------------------------------+
```

| Öğe | GATE-003 kapsamı | Bu şablon |
|-----|------------------|-----------|
| `.ai/.templates/ui-design/prompt-template.md` | ❌ kapsam dışı | ✅ serbest |
| `.ai/ui-design/prompts/<konu>.md` (metin) | ❌ kapsam dışı | ✅ serbest |
| Prompt'un **çalıştırılıp** `src/**` kodu üretmesi | ✅ kapsar | ⛔ kapı öncesi **yürütülmez** |
| `.ai/ui-design/01…07` + `screen/*.md` | ✅ kapsar | ⛔ kapı öncesi yazılmaz |

**Kural:** Prompt yazılır, **bekletilir**; kapı kaydı gelince girdi paketi tamamlanır ve çalıştırılır (§6.3). Ters sıra keşfedilirse iş durur (G4).

### §1.4 Neden Prompt'un Kendi Şablonu Var?

| Boşluk | Belirti | Bu Şablonun Kestiği Şey |
|--------|---------|--------------------------|
| Bağlam eksik | model uyduruyor, tutarsız çıktı | §3.2 bağlam bloğu — **yalnız diskteki dosyalar** |
| Kısıt yok | EF Core / UI'dan domain sızması | §3.3 + §3.8 |
| Çıktı belirsiz | "bir form yaz" → 3 farklı yapı | §3.6 çıktı sözleşmesi |
| Doğrulama yok | model "tamam" der, kırık kalır | §3.7 ölçüm komutları |
| Kapı atlanır | kapıdan önce kod üretilir | §1.3 [4] satırı |

---

## §2 Kapsam

**İçinde:** prompt dosyasının yazımı, bağlam bloğu, kısıtlar (DevExpress `l7`, ITCSS/BEM), görev tarifi, kontrol listesi, çıktı sözleşmesi, doğrulama, yasaklar, sürümlendirme, salt-okunur denetim.
**Dışında:** prompt'un **ürettiği** kod, tel kafes/şartname/akış kendisi, renk/token hesabı, kapı onayı, SQL.

| Kapsam | Kapsam Dışı |
|--------|-------------|
| Girdi dosyalarının **bağlantı listesi** | Girdilerin içeriği (→ kardeş şablonlar) |
| Kısıt + yasak listesi | İş kuralı tanımı (→ `l1` Domain) |
| Çıktı sözleşmesi (dosya/biçim/sınır) | Çıktının kendisi (→ `src/**`, kapı sonrası) |
| Doğrulama adımları + ölçüm komutu | Kapı onayı (→ GATE-003) |
| Reddedilme ölçütü (RED koşulları) | Paket sürümü kararı (→ ADR-001) |
| Prompt sürümü + geri besleme kaydı | Model seçimi/kurulumu (→ `../llm-provider-template.md`) |

### §2.1 Dosya Tipi → Sorumlu Eşlemesi

| Dosya Tipi | Hedef | Sorumlu | Bu Şablondan mı |
|------------|-------|---------|------------------|
| `.ai/ui-design/prompts/<konu>.md` | üretim promptu | ui-designer | ✅ **bu şablon** |
| `.ai/ui-design/screen/*.md` | şartname (girdi) | ui-designer | ❌ → `screen-spec` |
| `.ai/ui-design/flow/*.md` | akış (girdi) | ui-designer | ❌ → `flow` |
| `.ai/ui-design/reference/*.md` | token (girdi) | ui-designer | ❌ → `reference` |
| `.ai/ui-design/01…07-*.md` | tel kafes (girdi) | ui-designer | ❌ → [./wireframe-template.md](./wireframe-template.md) |
| `src/**` üretilen kod | developer + model | developer | ❌ GATE-003 sonrası |
| `src/**/Styles/*.scss` | ITCSS/BEM çıktısı | developer | ❌ kapı sonrası |
| `.ai/.templates/llm-provider-template.md` | model/üretici ayarı | architect | ❌ bu şablonun girdisi |

### §2.2 Kapsam Sınırı Testi

Üretilecek dosyaya sırayla sor; **ilk `EVET`** şablonu belirler:

1. İçinde **"şu dosyaları oku, şu kısıtlarla, şu çıktıyı ver"** mi var? → **bu şablon**.
2. Ekranın **yerleşimi** mi? → `./wireframe-template.md`.
3. **Kontrol/durum dökümü** mı? → `screen-spec`.
4. **Başlangıç → adım → bitiş** mi? → `flow`.
5. **Renk/token/ikon** mu? → `reference`.
6. **Doğrudan `.cs`** mi → ❌ kapı geçmeden üretilmez (§1.3).

`HAYIR` soruları: yasaklı vault prompt'u mu (G1)? · EF Core / yıldızlı sorgu / `SELECT *` geçen satır var mı (G2)? · Prompt'ta hex/renk değeri mi (G3)? · `{{…}}` yayımlanacak mı (G5)? · Bağlantılardan biri diskte yok mu (G7)?

---

## §3 Mimari

Bu bölüm hem **şablonun yapısını** hem **üretilecek prompt dosyasının yapısını** tanımlar. Kutular ` ```text ` içinde ve **≤ 66 sütun** yazılır.

### §3.1 Dosya ve Prompt Kimliği

| Alan | Kural | Doğru | Yanlış |
|------|-------|-------|--------|
| Dosya adı | `.ai/ui-design/prompts/<konu>.md` | `prompts/editor-panel.md` | `prompts/p1.md` |
| Prompt kodu | `PR-NN` | `PR-04` | `P4` |
| Konu | tek üretim hedefi | "Düzenleyici paneli" | "Tüm arayüz" |
| Girdi | en az 2 kardeş dosya | spec + reference | hiçbiri |
| Çıktı hedefi | kapı sonrası dosya yolu | `src/VersaCoder.Desktop/**` | — |
| Kapı | muafiyet + yürütme satırı | `GATE-003: prompt muaf, yürütme kapıda` | — |
| Sürüm | prompt `v1`, `v2`… | `v2` | — |
| Durum | `taslak / hazır / çalıştırıldı` | `hazır` | — |

### §3.2 Bağlam Bloğu (Context Block)

Bağlam **yalnız diskte var olan dosyalardan** kurulur; modelin "bildiğini varsaydığı" hiçbir şey yazılmaz.

| # | Öğe | Ne Verilir | Verilmez |
|---|-----|-----------|----------|
| C1 | Ürün | **VersaCoder** — masaüstü uygulaması | başka ürün adı (G1) |
| C2 | Ekran | `SP-NN` + `WF-NN` + `{{ekran_adi}}` | tahmini ekran |
| C3 | Amac | şartnamedeki tek cümle | pazarlama metni |
| C4 | Girdi dosyaları | **bağlantı** ile `screen-spec`, `flow`, `reference` | dosya içeriği kopyası |
| C5 | Katman haritası | `l0..l7` + ok yönü (§3.3) | katman dışı yer |
| C6 | Sürüm/kilit | DevExpress **26.1** (ADR-001) | "en son sürüm" |
| C7 | Kapı durumu | `GATE-003` açık/kapalı | kapı yerine geçen ifade |
| C8 | Dil | Türkçe arayüz metinleri, İngilizce tanım adları | karışık dil |

```text
+------------------------------------------------------------------+
| BAGLAM BLOGU — PR-04 · editor-panel                             |
+------------------------------------------------------------------+
| urun     : VersaCoder (masaustu)     katman: yalniz l7           |
| ekran    : SP-02 / WF-02  "Duzenleyici"                          |
| amac     : kullanici dosya duzenler, hata gorur, kaydeder        |
| girdi    : [screen-spec] [flow] [reference]  (baglanti ile)      |
| kilit    : DevExpress 26.1 (ADR-001)  |  CSS: ITCSS + BEM        |
| cikti    : src/VersaCoder.Desktop/**  (KAPI SONRASI)             |
| kapi     : GATE-003 muaf (prompt) / yurutme kapıda               |
+------------------------------------------------------------------+
```

**Yasak:** Bağlamda EF Core, veritabanı adı, harici vault cümlesi, hex renk **bulunmaz** (G1–G3).

### §3.3 Kısıtlar — DevExpress `l7` · ITCSS · BEM

| # | Kısıt | Kural | İhlal |
|---|-------|-------|-------|
| K1 | DevExpress | **yalnız `l7` (UI+CLI)** — `l0..l6`'da DevExpress tipi **YOK** | RED |
| K2 | Sürüm | `26.1` tek sürüm (ADR-001) | RED |
| K3 | CSS yöntemi | **ITCSS** sırası: reset → tokens → generic → elements → objects → components → utilities | RED |
| K4 | Sınıf adlandırma | **BEM**: `block__element--modifier` (`editor__tab--active`) | RED |
| K5 | Stil erişimi | satır içi stil (`style="…"`) **YASAK** — yalnız sınıf/token | RED |
| K6 | Katman oku | `l7→l6→l5→l4→l2→l1→l0` · `l3` yalnız `l0`'a bakar · `l2↔l4` **YOK** | RED |
| K7 | Binding | `l7` yalnız `BindingSource` / handler görür | RED |
| K8 | Doğrulama | saf kontrol (DataAnnotations **yok**) | RED |

**ITCSS sırası (zorunlu, bu sırayla):**

| # | Katman | İçerik | Örnek |
|---|--------|--------|-------|
| 1 | reset | tarayıcı sıfırlama | `box-sizing` |
| 2 | tokens | `--vc-*` değişkenleri | `--vc-color-accent` |
| 3 | generic | element dışı yardımcılar | `.u-visually-hidden` |
| 4 | elements | ham HTML stilleri | `body`, `h2` |
| 5 | objects | layout (`o-`) | `o-grid` |
| 6 | components | bileşen (`c-` / BEM blok) | `c-toolbar`, `editor__tab` |
| 7 | utilities | tek amaçlı (`u-`) | `u-mt-8` |

### §3.4 Görev Tarifi (Adımlar)

Prompt içindeki adımlar **numaralı ve tek yönlüdür**; model sırayı değiştirirse çıktı RED edilir.

| # | Adım | Girdi | Beklenen Çıktı |
|---|------|-------|----------------|
| 1 | Girdi dosyalarını oku | §3.2 bağlantı listesi | girdi özeti (dosya adı + madde) |
| 2 | Kısıtları uygula | §3.3 K1..K8 | kısıt listesi (tek tek) |
| 3 | Yapıyı kur | §3.6 çıktı sözleşmesi | dosya iskeleti |
| 4 | Bileşenleri doldur | `reference` token + varyant | sınıf/token kullanımı |
| 5 | Erişilebilirliği ekle | `screen-spec` §3.6 WCAG | odak + etiket + durum metni |
| 6 | Yasak taraması | §3.8 | ihlal = 0 beyanı |
| 7 | Doğrulama | §3.7 | ölçüm çıktısı |

### §3.5 Kontrol Listesi (prompt içinde)

Model **çıktısında** bu listeyi tek tek işaretler; işaretsiz satır = RED.

| # | Kontrol | Beklenen | Bağlı Kardeş |
|---|---------|----------|--------------|
| P1 | Dosya sayısı = sözleşmedeki | eşit | §3.6 |
| P2 | DevExpress tipleri yalnız `l7` | 0 ihlal | §3.3 K1 |
| P3 | CSS ITCSS sırasında | 7 katman | §3.3 K3 |
| P4 | Sınıf adları BEM | 0 ihlal | §3.3 K4 |
| P5 | Satır içi stil yok | 0 | §3.3 K5 |
| P6 | EF / SQL / `DbContext` yok | 0 | §3.8 |
| P7 | Domain/servis tipi UI'a sızmadı | 0 | §3.8 |
| P8 | Hex/renk değeri kodda yok | 0 | G3 |
| P9 | WCAG maddeleri uygulandı | 10/10 | `screen-spec` |
| P10 | `{{…}}` yer tutucu kalmadı | 0 | G5 |

### §3.6 Çıktı Sözleşmesi

| Alan | Kural | Değer |
|------|-------|-------|
| Dosya yolu | kapı **sonrası** | `src/VersaCoder.Desktop/{{Alan}}/{{Dosya}}.cs` |
| Stil dosyası | kapı **sonrası** | `src/VersaCoder.Desktop/Styles/{{block}}.scss` |
| Biçim | C# `partial class` + `.scss` | ikisi de |
| Uzunluk | kod dosyası | `180–320 satır` (form-template §1) |
| Sınıf adı | `<Ad>Form` | `EditorForm` |
| BEM bloğu | dosya = blok | `editor` |
| Yazma dili | metin Türkçe, tanım adı İngilizce | — |
| Marka | **VersaCoder** tek ürün adı | — |
| Şekil | yanıt **yalnız** sözleşme blokları | serbest sohbet YASAK |

**Yanıt kalıbı:** model yanıtını şu bloklarla verir: `## Girdi Özeti` → `## Dosyalar` → `## Kontrol Listesi (P1..P10)` → `## Doğrulama Çıktısı` → `## Açık Noktalar`. **Açık Noktalar** bölümü boş değilse iş `reviewer`'a gider (§4.2).

### §3.7 Doğrulama

Prompt'un **kendi** doğrulaması iki katmandır: (a) model çıktısının iç kontrolü, (b) insanın ölçümü.

| # | Doğrulama | Kim | Araç | Beklenen |
|---|-----------|----|------|----------|
| D1 | §3.5 P1..P10 işaretli mi | model | yanıt gövdesi | 10/10 |
| D2 | Kısıt ihlali taraması | model | `Select-String` | 0 satır |
| D3 | Satır/KB ölçümü | reviewer | `Get-Content` | 180–320 |
| D4 | BEM/ITCSS denetimi | reviewer | gözden geçirme | 0 ihlal |
| D5 | WCAG + sekme sırası | reviewer | `screen-spec` §3.6 | 10/10 |
| D6 | Kapı kaydı | orchestrator | `GATE-003` satırı | kayıt var |

**Kural:** Modelin "tamam, uygun" beyanı **kanıt sayılmaz** (G12); D2 ve D3 çıktısı olmadan iş kapanmaz.

### §3.8 Yasaklar — UI'dan Domain/Servis Sızması

| # | Sızma | Belirti | Doğrusu |
|---|-------|---------|---------|
| Y1 | Domain tipi UI'da | `l7` dosyasında `Entity` / `ValueObject` | yalnız DTO + `BindingSource` |
| Y2 | Servis çağrısı UI'dan | formda `IUseCaseService` doğrudan new'lenmesi | handler enjeksiyonu (DI) |
| Y3 | SQL / EF | `SELECT` metni, `DbContext`, `DbSet` | ADO.NET + Dapper, `@p1..@pN` (`l4`) |
| Y4 | İş kuralı kodu | formda `if (kural)` hesabı | `l1` invariant, `l2` handler |
| Y5 | Doğrulama attribute | `[Required]`, `[Range]` | saf kontrol (§3.3 K8) |
| Y6 | Katman tersi | `l7` → `l0` doğrudan ok | ara katmanlar (§3.3 K6) |
| Y7 | Konfigürasyon sızması | kodda sabit `localhost` / yol | `l4` configuration |
| Y8 | Hassas veri UI'da | token/parola ekranda + loglanır | yalnız `l4`/`l3` |

**Tespit:** §5.3/4 komutu `src/**` üzerinde çalışır ve `EntityFramework|DbContext|DbSet|SELECT\s+\*` avlar; 0 satır dışındaki her satır **RED**.

### §3.9 Sürümleme ve Geri Besleme

| Olay | Aksiyon | Prompt Sürümü | Kayıt |
|------|---------|---------------|-------|
| İlk yazım | taslak | `v1` | durum `taslak` |
| Girdi kardeş değişti (spec/flow/reference) | bağlam bloğu güncellenir | minor (`v2`) | `log.md` tek satır |
| Kısıt değişti (sürüm/CSS yöntemi) | §3.3 güncellenir | minor | architect onayı |
| Çıktı reddedildi (D2/D3) | yasak/girdi eklenir, **aynı sürümde** düzeltme | aynı | RED nedeni satırı |
| Kapı sonrası ilk çalıştırma | durum `çalıştırıldı` | patch | GATE-003 kaydı |
| Yanıt "Açık Noktalar" dolu | iş `reviewer`'a | — | §4.2 aşama 2 |

### §3.10 Dolu Örnek — `PR-04 · editor-panel`

```text
+------------------------------------------------------------------+
| PR-04 · editor-panel   v1   durum: HAZIR (yurutme KAPI SONRASI)  |
+------------------------------------------------------------------+
| BAGLAM                                                            |
|   urun : VersaCoder   ekran: SP-02 / WF-02 "Duzenleyici"          |
|   girdi: [screen-spec] [flow] [reference]  (baglanti ile)         |
|   kilit: DevExpress 26.1 | ITCSS + BEM | satir ici stil YASAK     |
+------------------------------------------------------------------+
| GOREV 1 oku -> 2 kisit -> 3 iskelet -> 4 doldur -> 5 WCAG         |
|         -> 6 yasak taramasi -> 7 olcum                             |
+------------------------------------------------------------------+
| CIKTI SOZLESMESI                                                   |
|   src/.../EditorForm.cs        180-320 satir   partial class      |
|   src/.../Styles/editor.scss   ITCSS katman 6  BEM: editor__*     |
|   yanit: Girdi Ozeti > Dosyalar > P1..P10 > Dogrulama > Acik      |
+------------------------------------------------------------------+
| YASAK  Y1..Y8  (domain/servis/SQL/sifirlama/attribute/ters ok)     |
| KAPI   prompt muaf · yurutme GATE-003 kaydindan sonra              |
+------------------------------------------------------------------+
```

**Ne anlatır:** Bağlam, kısıt, görev, sözleşme, yasak ve kapı durumu tek bakışta; her satır kardeş şablonlara bağlı.

### §3.11 Tam Kalıp İskeleti

Yer tutucular yalnız §3.2, §3.10 ve bu blokta yaşar; üretilen prompt'ta adedi **0** olmalıdır (G5).

````markdown
# Prompt — {{PR_NN}} · {{KONU_ADI}}

| Alan | Değer |
|---|---|
| Prompt / dosya | {{PR_NN}} · `.ai/ui-design/prompts/{{konu}}.md` |
| Sürüm / durum | v{{N}} · {{taslak / hazir / calistirildi}} |
| Ekran | {{SP_NN}} · {{WF_NN}} · {{EKAN_ADI}} |
| Kapı | GATE-003: prompt muaf (§1.3) — yürütme kapıda |

**§1 Bağlam Bloğu (§3.2)**
- Ürün: **VersaCoder** · katman: yalnız `l7`
- Girdi (bağlantı): [screen-spec] [flow] [reference] · kilit: DevExpress 26.1
- Kapı durumu: {{GATE_003}}

**§2 Kısıtlar (§3.3)**
K1..K8 · ITCSS 7 katman · BEM `{{block}}__{{el}}--{{mod}}`

**§3 Görev (§3.4)**
1 oku → 2 kısıt → 3 iskelet → 4 doldur → 5 WCAG → 6 yasak → 7 ölçüm

**§4 Çıktı Sözleşmesi (§3.6)**
{{dosya yolu}} · {{180–320}} satır · yanıt blokları: Girdi Özeti → Dosyalar → P1..P10 → Doğrulama → Açık Noktalar

**§5 Yasaklar (§3.8)**
Y1..Y8 — {{UI'dan domain/servis/SQL sızması yasak}}

**§6 Doğrulama (§3.7)**
D1..D6 — {{model iç kontrolü + insan ölçümü}}
````

---

## §4 Kurallar

### §4.1 Yazım Sırası ve Öncelik

| Öncelik | Şart | Yapılacak |
|---------|------|-----------|
| 1 | Girdi kardeşler yoksa | prompt **yazılır**, `⚠️ girdi yok` satırıyla çalıştırılmaz |
| 2 | Kimlik (§3.1) | 8 alan dolu |
| 3 | Bağlam bloğu (§3.2) | C1..C8, yalnız diskteki dosyalar |
| 4 | Kısıtlar (§3.3) | K1..K8 + ITCSS 7 |
| 5 | Görev (§3.4) | 7 adım tek yönlü |
| 6 | Kontrol listesi (§3.5) | P1..P10 |
| 7 | Çıktı sözleşmesi (§3.6) | yol + uzunluk + yanıt blokları |
| 8 | Yasaklar (§3.8) | Y1..Y8 |
| 9 | Doğrulama (§3.7) | D1..D6 |
| 10 | Ölçüm + denetim | §5.3 = 0 · §6.1 = 20/20 |

### §4.2 Sorumluluk ve Onay

| Aşama | Rol | Ne Onaylar | Çıktı | Süre |
|-------|-----|-----------|-------|------|
| 1 | ui-designer | prompt + §5.3 çıktısı | `.ai/ui-design/prompts/*.md` | 24 dk |
| 2 | reviewer | §6.1 (20 madde) + `Açık Noktalar` | ✅ / RED | 10 dk |
| 3 | architect | kısıt ↔ katman/ADR-001 | K1..K8 tutarlılığı | 7 dk |
| 4 | **kullanıcı (GATE-003)** | 7 tel kafes + 7 şartname | kapı kaydı | 20 dk |
| 5 | master-orchestrator | §5.4 Quality Report | `log.md` satırı | 6 dk |
| | **Toplam** | | | **≈ 67 dk** |

**3 RED** sonrası iş durur, gerekçe `master-orchestrator`'a yükseltilir. Prompt **yürütme yetkisi** alamaz — yalnız kapı sonrası çalıştırılır.

### §4.3 Ölçek / Standartlar

| Öğe | Değer | Sapma |
|-----|-------|-------|
| Satır sayısı | ≥ 480 | < 480 = **RED** (G6) |
| Dosya boyutu | ≥ 27,0 KB | < 27,0 KB = RED |
| Bağlam maddesi | 8 (C1..C8) | < 6 = RED |
| Kısıt | 8 (K1..K8) | < 8 = RED |
| Kontrol listesi | 10 (P1..P10) | < 10 = RED |
| Yasak | 8 (Y1..Y8) | < 8 = RED |
| Girdi bağlantısı | ≥ 2 kardeş | 0 = RED |
| Kutu satırı | ≤ 66 karakter | > 66 = düzelt |
| Kodlama | UTF-8 BOM'suz | Latin-1/CP1254 otomatik RED |

### §4.4 Değişiklik Kontrolü

| Olay | Aksiyon | `updated` | Kayıt |
|------|---------|-----------|-------|
| Yalnız yazım düzeltmesi | dosya içinde | değişmez | §5.3 yeniden |
| Girdi kardeş güncellendi | §3.2 bağlam + §3.5 | artar | prompt minor sürüm |
| Kısıt değişti | §3.3 + §3.8 | minor sürüm | architect |
| Çıktı RED edildi | yasak/girdi eklenir | aynı sürüm | RED nedeni satırı |
| Kapı kaydı alındı | durum `çalıştırıldı` | patch | GATE-003 |
| Prompt'ta kaynak kod bloğu | **RED** — kod kapı sonrası | — | §6.2 R7 |

### §4.5 Hard Guardrails (Otomatik DUR)

| # | Guardrail | İhlal Sonucu |
|---|-----------|--------------|
| **G1** | Yasaklı vault (`C:\www\…`) prompt'u/cümlesi | **DUR** — sil, özgün yaz; yalnız yapı referansı |
| **G2** | EF Core / yıldızlı sorgu / `DbContext` / UI'da SQL | **DUR** — ADO.NET + Dapper, yalnız `l4` |
| **G3** | Prompt'ta hex/renk değeri | **DUR** — `reference` şablonuna taşınır |
| **G4** | Kapı öncesi prompt **yürütme** ya da `src/**` kodu | **DUR** — §1.3 [4], iş geri alınır |
| **G5** | Üretilen prompt'ta `{{…}}` | **DUR** — §3.11 kalbı doldurulur |
| **G6** | Satır < 480 ya da KB < 27,0 | **DUR** — §4.3 |
| **G7** | Kırık bağlantı (`.ai/index.md`, `.ai/CLAUDE.md`, `.ai/WORKFLOW.md`, `.ai/.rules/`, `.ai/.decisions/`, `.ai/architecture/`, `.ai/keys.md`, `.ai/log.md`, `.ai/ui-design/`, `.ai/reports/`) | **DUR** — bağlantı kaldırılır, metin yazılır |
| **G8** | DevExpress'in `l7` dışı kullanımı / `l2↔l4` oku | **DUR** — §3.3 K1 · K6 |
| **G9** | Bağlamda diskte olmayan dosya "girdi" göstermek | **DUR** — §3.2 yalnız gerçek bağlantılar |
| **G10** | Çıktı sözleşmesi olmayan prompt ("bir şey yaz") | **DUR** — §3.6 |
| **G11** | Model beyanını kanıt saymak (D2/D3'siz kapanış) | **DUR** — G12, §3.7 |
| **G12** | Ürün adı dışında marka / yasaklı vault adı | **DUR** — yalnız **VersaCoder** |

---

## §5 Workflow

### §5.1 Adım Bazlı Prompt Yazımı

| # | Adım | Girdi | Çıktı | Yapan | Süre |
|---|------|-------|-------|-------|------|
| 1 | Girdi paketini kontrol | kardeş dosyalar | var/yok listesi | ui-designer | 3 dk |
| 2 | Kimlik | §3.1 | başlık tablosu | ui-designer | 3 dk |
| 3 | Bağlam bloğu | §3.2 | C1..C8 | ui-designer | 5 dk |
| 4 | Kısıtlar | §3.3 | K1..K8 + ITCSS | ui-designer | 6 dk |
| 5 | Görev + kontrol listesi | §3.4–§3.5 | 7 + 10 satır | ui-designer | 6 dk |
| 6 | Çıktı sözleşmesi | §3.6 | yol + bloklar | ui-designer | 4 dk |
| 7 | Yasaklar | §3.8 | Y1..Y8 | ui-designer | 5 dk |
| 8 | Doğrulama | §3.7 | D1..D6 | ui-designer | 4 dk |
| 9 | Ölçüm | §5.3/1–6 | 0 satır | ui-designer | 4 dk |
| 10 | Denetim | §6.1 (20 madde) | ✅ / RED | reviewer | 9 dk |
| | **Toplam** | | | | **≈ 49 dk** |

**Bütçe:** 1–8 ≈ 36 dk, 9–10 ≈ 13 dk. Tek prompt 25 dakikayı aşarsa §2.2'ye dön: kaynak kod ya da iş kuralı prompt'a sızmıştır.

### §5.2 Hata Modları

| # | Hata Modu | Belirti | Kök Neden | Düzeltme |
|---|-----------|---------|-----------|----------|
| 1 | Girdi yok | kardeş dosya YOK | erken yazılmış | `⚠️` satırı, beklet (§1.3) |
| 2 | Bağlam uydurma | diskte olmayan dosya | "bildiğini varsayma" | §3.2, G9 |
| 3 | Kısıt unutulmuş | ITCSS/BEM yok | §3.3 atlanmış | K3, K4 |
| 4 | Sızma | UI'da `Entity`/SQL | §3.8 okunmamış | Y1..Y3 |
| 5 | Sözleşmesiz yanıt | serbest sohbet | §3.6 yok | blok kalıbı |
| 6 | Kanıtsız kapanış | "tamam uygun" | D2/D3 yok | G11, ölçüm |
| 7 | Kapı atlandı | prompt erken çalıştırıldı | §1.3 unutulmuş | **DUR** (G4) |
| 8 | Hex prompt'a sızdı | renk değeri yazıldı | G3 karıştırılmış | `reference` |
| 9 | Yer tutucu sızıntısı | `{{BLOK}}` | §3.11 doldurulmadı | G5 |
| 10 | Marka sapması | yanlış ürün adı | G1 | yalnız **VersaCoder** |

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
  if ($m) { 'EKSIK ALAN -> '+$_.Name+': '+($m -join ',') }
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

# 4) SIZMA TARAMASI — yasak terimler sablon icinde 0 satir (Y1..Y3, G2)
Get-ChildItem 'C:\versacoder\.ai\.templates\ui-design\*.md' |
  Select-String -Pattern 'DbContext|DbSet|EntityFramework|SELECT\s+\*|DataAnnotations' |
  ForEach-Object { 'SIZMA -> {0}:{1}' -f $_.Filename,$_.LineNumber }

# 5) YER TUTUCU + KODLAMA — BOM/mojibake 0 (G5)
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

# 6) MARKA + KISIT — VersaCoder disinda urun adi yok, K/P/Y sayilari tam
Get-ChildItem 'C:\versacoder\.ai\.templates\ui-design\*.md' |
  Select-String -Pattern 'CoreMusic|versacoder\.ai' | ForEach-Object { 'MARKA -> {0}:{1}' -f $_.Filename,$_.LineNumber }
$r='C:\versacoder\.ai\.templates\ui-design\prompt-template.md'
$t=[IO.File]::ReadAllText($r)
foreach ($p in @(@('K',8),@('P',10),@('Y',8),@('D',6),@('C',8))) {
  $n=([regex]::Matches($t,'(?m)^\| '+$p[0]+'\d')).Count
  if ($n -lt $p[1]) { 'EKSIK SAYIM -> {0}: {1}/{2}' -f $p[0],$n,$p[1] } }
```

**Geçme:** #1–#6 hepsi **0 satır uyarı** (#6'da `C/P/K/Y/D` sayıları §4.3 eşiklerini karşılar).

### §5.4 Quality Report

```markdown
## Quality Report — prompt (YYYY-MM-DD HH:MM)

| Metrik | Değer | Eşik | Durum |
|---|---|---|---|
| Ölçüm (bu şablon) | <satır> satır / <KB> KB | ≥ 480 · ≥ 27,0 | 🟢/🔴 |
| Üretilen prompt dosyası | n | ≥ 3 | 🟢/🔴 |
| Frontmatter 8 alan (§5.3/1) | n/8 | 8 | 🟢/🔴 |
| Kırık bağlantı (§5.3/2) | n | 0 | 🟢/🔴 |
| Kutu > 66 (§5.3/3) | n | 0 | 🟢/🔴 |
| Sızma terimi (§5.3/4) | n | 0 | 🟢/🔴 |
| Yer tutucu / BOM / mojibake | n/n/n | 0/0/0 | 🟢/🔴 |
| Marka + sayım (§5.3/6) | n | 0 | 🟢/🔴 |
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
| K6 | `§1.3` prompt/yürütme ayrımı yazılı | var | G4 |
| K7 | `§2.1` eşleme + `§2.2` sınır testi | 2/2 | §2 |
| K8 | `§3.1–§3.11` tam (11 alt bölüm) | 11/11 | §3 |
| K9 | Bağlam C1..C8 dolu, yalnız gerçek bağlantı | 8 | §3.2 |
| K10 | Kısıt K1..K8 + ITCSS 7 katman | 8 + 7 | §3.3 |
| K11 | Görev 7 adım tek yönlü | 7 | §3.4 |
| K12 | Kontrol listesi P1..P10 | 10 | §3.5 |
| K13 | Çıktı sözleşmesi: yol + uzunluk + yanıt blokları | 3/3 | §3.6 |
| K14 | Doğrulama D1..D6 | 6 | §3.7 |
| K15 | Yasaklar Y1..Y8 (domain/servis sızması) | 8 | §3.8 |
| K16 | Sürüm/geri besleme tablosu dolu | 5 satır | §3.9 |
| K17 | Dolu örnek + tam kalıp birlikte | 2 | §3.10–§3.11 |
| K18 | `§4.1–§4.5` + G1..G12 | 5 + 12 | §4.5 |
| K19 | `§5.1–§5.4` (10 adım · ≥8 hata modu · 6 komut · report) | 4/4 | §5 |
| K20 | `§6.1–§6.5` + `§7.1–§7.2` (Red Team ≥ 10) + ≥ 480/≥ 27 KB | 6/6 · 2/2 | G6 |

### §6.2 Red Sinyalleri (Otomatik DUR)

| # | Sinyal | Aksiyon |
|---|--------|---------|
| R1 | §5.3 komutlarından biri > 0 satır | **DUR** — gösterilen dosya düzeltilir |
| R2 | Bağlamda diskte olmayan girdi dosyası | **DUR** — G9, bağlantı gerçek olmalı |
| R3 | Kısıt eksik (K < 8) | **DUR** — §3.3 |
| R4 | Çıktı sözleşmesi yok | **DUR** — §3.6 |
| R5 | `src/**` içinde sızma terimi | **DUR** — §3.8 Y1..Y3 |
| R6 | Prompt'ta hex / renk değeri | **DUR** — G3, `reference` |
| R7 | Prompt'ta kaynak kod bloğu | **DUR** — §4.4, kod kapı sonrası |
| R8 | Kapı öncesi yürütme | **DUR** — G4, §1.3 [4] |
| R9 | Yer tutucu / kırık bağlantı | **DUR** — G5 · G7 |
| R10 | "Uygun/tamam" beyanı kanıtsız | **DUR** — G11, D2 + D3 çıktısı |

### §6.3 Kapı ve Geçiş Kuralları

| Geçiş | Koşul (hepsi `EVET`) | Onaylayan | Etki |
|-------|----------------------|------------|------|
| taslak → inceleme | §5.3 = 0 · §6.1 20/20 | ui-designer → reviewer | mimariye sunulur |
| inceleme → **hazır** | §6.2 = 0 · `Açık Noktalar` boş | architect | prompt beklemeye alınır |
| hazır → **çalıştırıldı** | `GATE-003` kaydı mevcut | orchestrator | `src/**` üretimi serbest |
| çalıştırıldı → kabul | D1..D6 çıktısı + §6.1 | reviewer + kullanıcı | sonuç kabul edilir |
| RED → revizyon | yasak/girdi eklendi | ui-designer | §5.9 sürüm kaydı |

Prompt **kapı muafidir** ama **yürütme kapıya bağlıdır**: kapı alındıktan sonra çalıştırılır, üretilen kod `src/**` kurallarına tabidir.

### §6.4 İlgili Dosyalar

| Dosya | İlişki |
|-------|--------|
| [./wireframe-template.md](./wireframe-template.md) | prompt'un **girdisi** (yerleşim) |
| [../index.md](../index.md) | envanter satır 29 · §3.7 · §4.5 G1..G12 |
| [../frontend/form-template.md](../frontend/form-template.md) | üretilen kodun biçimsel kaynağı |
| [../adr/adr-devexpress-template.md](../adr/adr-devexpress-template.md) | `26.1` kilidi (K2) |
| [../adr/adr-architecture-template.md](../adr/adr-architecture-template.md) | katman `l0..l7` (K1, K6) |
| [../backend/handler-template.md](../backend/handler-template.md) | `l2` karşılığı (Y2) |
| [../query/Query-Template.md](../query/Query-Template.md) | sorgu kuralı `@p1..@pN` (Y3) |
| [../agents/subagent-template.md](../agents/subagent-template.md) | alt-agent prompt dağıtımı |
| `./screen-spec-template.md` · `./flow-template.md` · `./reference-template.md` | kardeş girdiler — metin |

### §6.5 Envantere Özgü Kontroller

| # | Kontrol | Beklenen | Kanıt |
|---|---------|----------|-------|
| K1 | Bu şablon satır aralığı | ≥ 480 | `Get-Content` + `Count` |
| K2 | Bu şablon KB (envanter: `9,0`) | ≥ 27,0 | `(Get-Item …).Length / 1KB` |
| K3 | Sızma terimi geçen satır (§5.3/4) | 0 | tarama |
| K4 | Marka dışı ürün adı (§5.3/6) | 0 | tarama |
| K5 | Girdi bağlantısı ≥ 2 kardeş | ≥ 2 | §3.2 |
| K6 | Yasaklı vault adı geçen satır | 0 | G1 |
| K7 | `.ai/ui-design/prompts/` dosya sayısı (üretim sonrası) | ≥ 3 | `Test-Path` |

---

## §7 Referanslar

### §7.1 VersaCoder Bağlantıları

| Kaynak | Yol / Bağlantı | Ne İçin | Durum (2026-10-05) |
|--------|----------------|---------|---------------------|
| Şablon envanteri | [../index.md](../index.md) | §3.4 satır 29 · §3.7 · G1..G12 | ✅ okundu |
| Kardeş (altın örneği) | [./wireframe-template.md](./wireframe-template.md) | biçim referansı + girdi | ✅ 642 satır / 34,5 KB |
| Form karşılığı | [../frontend/form-template.md](../frontend/form-template.md) | çıktı kodunun kaynağı | ✅ okundu |
| DevExpress kilidi | [../adr/adr-devexpress-template.md](../adr/adr-devexpress-template.md) | K1 · K2 (`26.1`) | ✅ |
| Mimari ADR | [../adr/adr-architecture-template.md](../adr/adr-architecture-template.md) | `l0..l7` ok yönü | ✅ |
| Handler | [../backend/handler-template.md](../backend/handler-template.md) | Y2 — DI çağrısı | ✅ |
| Repository | [../backend/repository-template.md](../backend/repository-template.md) | Y3 — yalnız `l4` | ✅ |
| Sorgu şablonu | [../query/Query-Template.md](../query/Query-Template.md) | `@p1..@pN` (Y3) | ✅ |
| Alt-agent | [../agents/subagent-template.md](../agents/subagent-template.md) | prompt dağıtımı | ✅ |
| LLM sağlayıcı | [../llm-provider-template.md](../llm-provider-template.md) | model/üretici ayarı | ✅ (kök `.templates/`) |
| Kök belgeler | [../../../CLAUDE.md](../../../CLAUDE.md) · [../../../AGENTS.md](../../../AGENTS.md) | G8 · ajan rolleri | ✅ diskte var |
| `.ai/index.md` · `.ai/WORKFLOW.md` · `.ai/.rules/` | **metin** (bağlantı yok) | kapı/kural referansı | ❌ YOK → ⚠️ |
| Yasaklı vault'lar | `C:\www\…` (envanter G1) | yalnız **yapı** referansı | 🔒 prompt kopyalanmaz |

### §7.2 Red Team — Sık Yapılan Hatalar

| # | Hata | Neden Yanlış | Doğrusu |
|---|------|--------------|---------|
| 1 | Kapıdan önce prompt'u çalıştırma | üretilen kod `src/**`, G4 ihlali | §1.3 [4] — yazılır, bekletilir |
| 2 | Bağlamda olmayan dosyayı "girdi" gösterme | model uydurur, G9 | §3.2 yalnız gerçek bağlantı |
| 3 | "İyi bir form yaz" demek | sözleşme yok, 3 farklı yapı | §3.6 çıktı sözleşmesi |
| 4 | Kısıt listesiz prompt | EF/UI sızması, katman bozulur | §3.3 K1..K8 |
| 5 | UI'dan domain/servis çağrısı istemek | Y1/Y2 ihlali | handler DI (Y2) |
| 6 | Prompt'a hex/renk koymak | G3, iki gerçek kaynağı | `reference` bağlantısı |
| 7 | Prompt'a kaynak kod bloğu koymak | kapı atlanır, kod tartışılmaz | §4.4 — kod kapı sonrası |
| 8 | Modelin "tamam" demesini onay saymak | kanıtsız iddia (G11) | D2 + D3 çıktısı |
| 9 | Çıktı uzunluğu yazmamak | 900 satırlık dosya | §3.6: 180–320 satır |
| 10 | `.ai/index.md`'ye bağlantı vermek | diskte yok, kırık link (G7) | metin olarak yaz |
| 11 | `{{BLOK}}` ile işe başlatmak | iskelet üretiliyor | G5, §5.3/5 = 0 |
| 12 | Başka ürün adı kullanmak | G12, marka sapması | yalnız **VersaCoder** |

**SSUT beyanı:** UI üretim promptunun **tek** kaynağı bu dosyadır; tel kafes, screen-spec, flow, reference ve frontend şablonları yalnız buna bağlanır. Kopyalanan her satır ikinci gerçek kaynağı doğurur (envanter G12).
