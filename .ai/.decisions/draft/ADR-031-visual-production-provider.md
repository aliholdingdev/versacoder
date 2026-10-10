---
title: "ADR-031 — Görsel Üretim Sağlayıcı Entegrasyonu (ÖNERİ)"
type: adr
category: ".ai/.decisions"
status: proposed
version: 0.2.0
updated: 2026-10-07
authority: primary
scope: "VersaCoder Studio/IDE kapsamında görsel üretim (render) sağlayıcısının hangi katmana yerleşeceği, port/adapter modeli, desteklenecek üç sağlayıcı yolu (OpenRouter image · ayrı sağlayıcılar · harici CLI) ve doğrulanmamış açık konular"
tags: [adr]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# ADR-031 — Görsel Üretim Sağlayıcı Entegrasyonu (ÖNERİ)

**Zorunlu Bağlantılar:** [[index|index]] · [[claude|CLAUDE]] · [[definitions/agents|AGENTS]] · [[definitions/vision|VISION]] · [[governance/workflow|WORKFLOW]] · [[.decisions/accepted/adr-028-layered-baseline-9-projects|ADR-028]] · [[.decisions/brain|brain]] · [[.decisions/adr-index|adr-index]]

> **Tek cümlelik sorumluluk:** VersaCoder'ın görsel üretim yeteneğinin **hangi
> katmanda, hangi port/adapter ile** yerleşeceği ve **hangi üç sağlayıcı yolunun**
> destekleneceğinin kararını taşır.
> **Durum: `proposed`** — kapsam kararı kullanıcı tarafından verildi (2026-10-07,
> "hepsi"); `accepted` olmak için §5'teki U2–U8 teknik doğrulamaları kapanmalıdır (G12).

---

## §0 Özet (Answer First)

- **Karar (kapsam — kullanıcı onaylı 2026-10-07):** Görsel üretim için **üç
  sağlayıcı yolu birden** kapsamdadır: **A)** OpenRouter image endpoint ·
  **B)** ayrı sağlayıcılar (Midjourney / FLUX / SDAPI sınıfı) · **C)** harici CLI
  (mevcut External Agent deseni). Üçü de **aynı port/adapter iskeleti** üzerine
  kurulur; ikinci bir mimari **açılmaz**.
- **Mimari (öneri):** port `l0`, adapter `l4`, protokol `l5` — mevcut
  `IAiProvider` / `llm-http` (N31) deseninin **görsel varyantı**. Yeni katman,
  yeni `.csproj` **açılmaz** (DLL şişirme yasağı, ai.08 §51).
- **Destek listesi (kaynaklı):** prompt üretimi tarafında 11 model ailesi
  (Nano Banana Pro 2 · FLUX · SDXL · Pony · Illustrious · Midjourney · GPT Image ·
  Imagen · Anime modelleri · LoRA pipeline · ComfyUI) — [§4](#4-doğrulanmış-kaynaklar).
- **Henüz kararlaştırılmayan (DOĞRULAMA GEREKLI):** üç yolun da **gerçek**
  endpoint/anahtar/lisans/fiyat/rate-limit değerleri — hiçbiri kaynakta yok (§5).
  Bu yüzden durum `proposed`, `accepted` **değil**.
- **Studio/IDE bağlamı:** VersaCoder = kod + dokümantasyon + görsel üretim
  ([[definitions/vision|VISION]] §0.3); bu ADR üçüncü yeteneğin altyapı kararını verir.
- **Kapı ilişkisi:** Bu ADR **kod üretmez**; `src/**` yazımı GATE-003 onayından
  sonra ve bu ADR `accepted` olduktan sonra başlar (G12 · G8).

---

## §1 Bağlam (Context)

### §1.1 Neden şimdi

| # | Tetikleyici | Kaynak |
|---|---|---|
| 1 | VersaCoder Studio/IDE olarak genişletildi (kod + docs + görsel) | [[definitions/vision|VISION]] §0.3 · [README](../../../readme.md) §1 |
| 2 | Görsel üretim hattı tanımlandı (8 adım) ama **sağlayıcı boş** | [[definitions/agents|AGENTS]] §8.5 |
| 3 | Prompt üretimi tarafı hazır (model listesi, kalite kapısı, çıktı iskeleti) | AGENTS §8.5 · ChatGpt görsel prompt arşivi |
| 4 | Mevcut AI sağlayıcı soyutlaması zaten var | ADR-028 §2.1 (`llm-http` N31) · ADR-011 (fallback sırası) |

### §1.2 Problem

Prompt üretilebiliyor ama **prompt nereye gönderilecek** belirsiz. Bu belirsizlik
üç şekilde borç üretir:

```text
  prompt uretilmis          -->  nereye gonderilecek?  -->  BILINMIYOR
        |                                                      |
        |  secim yapilmazsa                                    v
        |  her modul kendi yolunu uretir  -->  3 farkli HTTP istemcisi
        |                                       + 3 farkli hata yonetimi
        v                                       + lisans/fiyat surprisesi
  gorsel uretim "calisiyor" diyemez (G5)   =  kanitsiz kapanis
```

### §1.3 Kısıtlar (bağlayıcı)

| # | Kısıt | Kaynak |
|---|---|---|
| K1 | Yeni `.csproj` yalnız gerçek boundary gerektiriyorsa açılır | ai.08 §51 (DLL şişirme yasağı) |
| K2 | Port `l0`'da, adapter `l4`'te; `l2 → l4` doğrudan ok **yok** | [[.decisions/accepted/adr-028-layered-baseline-9-projects|ADR-028]] §2.1 |
| K3 | EF Core yok; veri erişimi ADO.NET + Dapper | [[claude|CLAUDE]] G2 |
| K4 | Secret düz metin değil; DPAPI şifreli depo | ADR-018 · [[claude|CLAUDE]] G8 |
| K5 | Bilinmeyen API/endpoint/sürüm **uydurulmaz** | [[claude|CLAUDE]] G9 |
| K6 | Kod yalnız GATE-003 onayı sonrası | [[governance/workflow|WORKFLOW]] §2.2 |

---

## §2 Karar (ÖNERİ — henüz kabul edilmedi)

### §2.1 Mimari yerleşim (ADR-028 katman modeli içinde — üç adapter ailesi)

```text
┌──────────────────────────────────────────────────────────────┐
│ l7  UI/CLI     05-ai-chat (prompt girdisi) · 06-agent-panel │
└────────────────────────────┬─────────────────────────────────┘
                             │
┌────────────────────────────▼─────────────────────────────────┐
│ l6  Host        DI kaydi: IImageProvider -> hangi adapter?    │
│                 (hepsi kayitli; politika/tercih secir)        │
└────────────────────────────┬─────────────────────────────────┘
                             │
┌────────────────────────────▼─────────────────────────────────┐
│ l5  Protocol    image semasi: request/response + streaming    │
│                 (tool-schema/mesaj ailesiyle ayni katman)     │
└────────────────────────────┬─────────────────────────────────┘
                             │
┌────────────────────────────▼─────────────────────────────────┐
│ l4  Infrastructure   IImageProvider adapter AILESI            │
│  ┌────────────────┬───────────────────┬────────────────────┐  │
│  │ A) OpenRouter  │ B) Ayrilmis       │ C) Harici CLI      │  │
│  │    Image Http  │    Saglayicilar   │    Executor        │  │
│  │    adapter     │    adapter ailesi │    (External Agent │  │
│  │                │  Midjourney/FLUX/ │     deseni, AGENTS │  │
│  │                │  SDAPI vb.        │     S4.3 tekrar    │  │
│  │                │                   │     kullanilir)    │  │
│  └────────────────┴───────────────────┴────────────────────┘  │
│     (N31 llm-http yaninda; prompt uretimi l4'te DEGIL)        │
└────────────────────────────┬─────────────────────────────────┘
                             │ implements
┌────────────────────────────▼─────────────────────────────────┐
│ l0  Abstractions   IImageProvider / ImageRequest /            │
│                    ImageResult  (port — tek sozlesme)          │
└──────────────────────────────────────────────────────────────┘

  NOT: prompt uretim (analiz -> prompt) l2 Application'da calisir;
       l4 yalnizca uretilmis promptu secili saglayiciya GONDERIR.
       A/B/C ayni portu uygular — yani biri digerinin yerine GECMEZ,
       birlikte yasar; kullanici/policy hangisini kullanacagini secer.
```

**Üç yol neden ayrı adapter, tek port?** Çünkü üçü de aynı sözleşmeyi
(`ImageRequest → ImageResult`) farklı bir **mekanizmayla** karşılar: A HTTP
endpoint, B farklı HTTP + farklı kimlik doğrulama, C süreç (process) yürütme.
Tek port = `l0`/`l2` hiç değişmez; yeni sağlayıcı eklemek yalnız `l4`'te yeni bir
adapter satırıdır (OCP, brain §2.2).

### §2.2 Önerilen sözleşme (kavramsal — implementasyon GATE-003 sonrası)

```text
ImageRequest
----------------------
Provider        : hangi saglayici (yalniz desteklenenlerden)
Mode            : TEXT_TO_IMAGE | IMAGE_TO_IMAGE | EDIT | RESTORE
Prompt          : uretilmis ana prompt (Ingilizce)
NegativePrompt  : ilgili negatif
ReferenceImage  : IMAGE_TO_IMAGE / identity preservation icin
Target          : cozunurluk (3840x2160 | 7680x4320) + aspect (16:9)
ModelHint       : kullanici belirttiyse model adi
Timeout / Cancel / WorkingDirectory

ImageResult
----------------------
Status · ExitCode/HttpStatus · ImageBytes/Path · Seed ·
Duration · Errors · Warnings · ValidationState · Metadata
```

> **[DOĞRULAMA GEREKLI]** Alanların **kesin adları ve tipleri** implementasyon
> sırasında sabitlenir; bu tablo kavramsaldir ve `src/**`'de karşılığı yoktur.

### §2.3 Üretim/prompt ayrımı (kritik sınır)

| Katman | Ne yapar | Ne YAPMAZ |
|---|---|---|
| `l2` Application | Görsel analiz → prompt üretimi (`visual-director` mantığı) | Sağlayıcıya HTTP atmaz |
| `l4` Infrastructure | `IImageProvider` adapter'ı: promptu sağlayıcıya gönderir | Prompt **üretmez**, analiz **yapmaz** |
| `l5` Protocol | Request/Response şeması, streaming/sse | İş mantığı taşımaz |
| `l0` Abstractions | `IImageProvider` portu + `ImageRequest/Result` | Somut sınıf **barındırmaz** |

**Neden:** prompt üretimi olgunlaşır (model listesi değişir), sağlayıcı değişir;
ikisini ayırmak OCP'yi korur — yeni sağlayıcı = yeni adapter, zincir değişmez
(brain §2.2 OCP).

---

## §3 Alternatifler

| Etiket | Alternatif | + | − | Maliyet |
|---|---|---|---|---|
| **A (seçildi)** | `IImageProvider` portu (`l0`) + adapter ailesi (`l4`) + şema (`l5`) | ADR-028 deseniyle birebir; test edilebilir; sağlayıcı değişimi tek adapter | İlk port tasarımı gerekir | ≈ 1 gün tasarım + GATE-003 sonrası kod |
| **B (kapsamda)** | Mevcut `llm-http` (N31) içine gömülü image endpoint | Sıfır yeni port | `llm-http` şişer; image ≠ chat sözleşmesi; OCP/ISP ihlali | Düşük başlangıç, **yüksek borç** |
| **C (kapsamda)** | Harici CLI/Studio süreci (`IExternalExecutor` üzerinden) | Mevcut External Agent altyapısını (AGENTS §4.3) **yeniden** kullanır | Prompt→CLI→dosya→geri okuma zinciri ek katman | Orta-yüksek |

**Kapsam kararı (kullanıcı, 2026-10-07 — "hepsi"):** Üç yol **da** kapsamdadır.
Ama bu, B'nin "gömme" tasarımının kabulü **değildir**:

```text
  KAPSAM = { A, B, C }          (hangi saglayicilar desteklenecek)
  TASARIM = A                   (port/adapter iskeleti — tek mimari)
      |
      +--> A'nin adapter'i : OpenRouter image endpoint   (yol A)
      +--> B'nin adapter'lari: Midjourney/FLUX/SDAPI...  (yol B)
      +--> C'nin adapter'i : harici CLI executor         (yol C)
      |
      NOT: "B = llm-http'a GOMME" hala ELENMISTIR (ISP ihlali).
           B kapsamda ama kendi adapter'iyla; gomme yok.
```

**Eleme gerekçeleri (değişmedi):**
- **"B = gömme" elendi:** image request/response chat completion'dan **farklı** bir
  sözleşmedir; tek istemciye yığmak brain §2.4 ISP ihlali üretir. Yol B **kapsamda**,
  ama kendi adapter'ıyla.
- **C artık ertelenmedi:** kullanıcı "hepsi" dedi; C, mevcut `IExternalAgentExecutor`
  deseninin (AGENTS §4.3) **yeniden kullanımı** olduğundan ek mimari maliyeti düşüktür —
  ama gerçek CLI/binary bilgisi yine `[VERIFY REQUIRED]`.
- **A kazandı:** üç yolu birden taşıyabilecek **tek** iskelet A'dır; B/C onun
  adapter'larıdır.

---

## §4 Doğrulanmış Kaynaklar

> Aşağıdaki her satır prompt arşivinde **okunarak** doğrulanmıştır; uydurma yok.

| # | Doğrulanmış bilgi | Kaynak dosya · bölüm |
|---|---|---|
| V1 | Prompt üretimi 11 model ailesine uyarlanabilir: Nano Banana Pro 2 · FLUX · SDXL · Pony · Illustrious · Midjourney · GPT Image · Imagen · Anime modelleri · LoRA pipeline · ComfyUI | `girl-image-analzye-promt.md` §15 |
| V2 | Model-specific çıktı 7 biçimde: Nano Banana (editing) · FLUX (natural-language) · SDXL (prompt+negative+weighting) · Pony/Illustrious (tag-based) · Midjourney (compact+params) · GPT Image (detailed instruction) · ComfyUI (workflow-oriented) | aynı dosya §16 |
| V3 | 8K generator model listesi: Midjourney · FLUX · SDXL · Stable Diffusion · DALL·E + kullanıcı belirttiği diğerleri | `girl-image-8k-generator...` §23 |
| V4 | Hedef çözünürlükler: **3840×2160 (4K)** ve **7680×4320 (8K)**, aspect **16:9**, landscape | aynı dosya §22 TARGET |
| V5 | İki mod: `QUICK MODE` (title/prompt/negative/resolution/camera) ve `PROFESSIONAL CINEMATIC MODE` (17 alan) | aynı dosya §21 |
| V6 | Çıktı iskeleti 9 bölüm: TITLE · TARGET · CAMERA · LIGHTING · COMPOSITION · MAIN PROMPT · NEGATIVE PROMPT · ENHANCEMENT · WALLPAPER NOTES | aynı dosya §22 |
| V7 | Final kalite kapısı **19 kontrol** (16:9, 4K/8K ayrımı, icon-safe/taskbar-safe, anatomi, negative çelişkisi…) | aynı dosya §24 |
| V8 | Reconstruction prompt sırası 18 alan: `SUBJECT → IDENTITY → … → FIDELITY`; prompt **İngilizce**, açıklama Türkçe olabilir | `girl-image-analzye` §13 |
| V9 | `VISIBLE INFORMATION ONLY` kuralı + `EXTREME IDENTITY PRESERVATION` seçeneği | aynı dosya §10, §11 |
| V10 | 8 adım hat: `INPUT → ANALYZE → UNDERSTAND → EXTRACT → PLAN → PROMPT → OPTIMIZE → FINAL` | `ai-vision-render-studio.md` PRIMARY MISSION |
| V11 | 9 üretim modu: TEXT_TO_IMAGE · IMAGE_TO_IMAGE · IMAGE_EDITING · IMAGE_RESTORATION · PRODUCT · ARCHITECTURAL · CHARACTER · UI_UX · PHOTOGRAPHIC_TRANSFORM | aynı dosya IMAGE GENERATION MODES |
| V12 | Referans görsel = **visual source of truth**; kimlik/yüz/oranlar korunur | aynı dosya VISUAL SOURCE OF TRUTH / IDENTITY PRESERVATION |

**Doğrulama etiketi:** `SOURCE_VERIFIED — prompt arşivi, 2026-10-07 okundu`.
Bu dosyadaki **hiçbir satır** sağlayıcı API'si, fiyat, lisans veya endpoint
içermaz — onlar §5'tedir.

---

## §5 Bilinmeyenler (DOĞRULAMA GEREKLI — tahmin yasak, G9)

| # | Bilinmeyen | Durum | Doğrulama yolu |
|---|---|---|---|
| U1 | Hangi sağlayıcı yolları kapsamda | ✅ **KAPANDI** (2026-10-07) — kullanıcı: **hepsi (A+B+C)** | — (karar §3) |
| U2 | API endpoint / authentication / SDK | 🟡 **A ✅ kapandı** · B kısmen (ikincil kaynak) · C açık | BFL/Stability **birincil** docs + CLI dokümanı |
| U3 | Fiyat / kota / rate-limit | 🟡 A yapısal ✅ (tutar çelişkili G10) · B `NEEDS_VALIDATION` · C açık | canlı `cost_usd` + bfl.ai/stability.ai |
| U4 | Lisans / ticari kullanım | 🟡 **SD Community License kısmen kapandı** (<$1M) · A/BFL/C açık | lisans metinleri |
| U5 | Model adlarının güncelliği | ✅ **A kapandı** (59 model canlı çekildi) · B/C açık | A: üretimde her oturumda `GET` |
| U6 | Seed/parametre destekleri | 🟡 A **model bazında canlı** (§5.1.1) · B/C açık | capability descriptor + docs |
| U7 | Çıktı formatı | ✅ **A kapandı** (`b64_json` + SSE) · B/C açık | API referansı |
| U8 | Görsel depolama yeri (vault mu, temp mi) | AÇIK — karar verilmedi | §8 soru |

> **KURAL:** U2–U8 çözülmeden bu ADR `accepted` **olamaz**; çözüm öncesi
> `src/**`'e görsel sağlayıcı kodu yazılmaz (G12 · G5). **U1 + yol A'nın
> U2/U5/U7'si kapandı** — bu, kod izni **değildir**, yalnız kanıtlanmış
> kapsam/teknik bilgi anlamına gelir.

**Üç yol için U2–U7 ayrı ayrı kapatılır** (tek bir doğrulama üçünü de kapatmaz):

| Yol | U2 (endpoint/auth) | U3 (fiyat) | U4 (lisans) | Not |
|---|---|---|---|---|
| A — OpenRouter image | ✅ §5.1 | ⚠️ yapı ✅ / tutar G10 | AÇIK | 59 model canlı (§5.1.1) |
| B1 — FLUX (BFL) | ✅ **bfl.ai** | ✅ **birincil $0.014–$0.08** | AÇIK | `docs.bfl.ai` (§5.2 B2) |
| B2 — Stability SD | ✅ **platform.stability.ai** | ✅ **birincil $0.009–$0.08** | 🟡 **Community License ✅** | U4 adayı |
| B3 — Midjourney | 🔴 **resmi public API yok** | abonelik (AÇIK) | 🔴 | **kapsam dışı bırakılabilir** |
| C — harici CLI | AÇIK | kendi anahtarı | AÇIK | binary + argüman |

> **Tur-2 kazancı:** yol B'nin FLUX ve SD kollarında **U2 + U3 kapandı** (birincil
> kaynak); Midjourney kolu (B3) hâlâ 🔴. Toplam kalan açık: yol A **U3-tutar + U4** ·
> yol B **U4-BFL + B3** · yol C **tamamı** · her yolda **U8** (depolama yolu).

### §5.1 Yol A doğrulaması — OpenRouter Image API (`WEB_VERIFIED`, 2026-10-07)

> İki bağımsız kaynakla doğrulandı: [Image API duyurusu](https://openrouter.ai/blog/announcements/image-api/)
> ve [Image Generation dokümanı](https://openrouter.ai/docs/guides/overview/multimodal/image-generation).

| # | Doğrulanmış | Değer | U etkisi |
|---|---|---|---|
| W1 | Üretim endpoint'i | `POST https://openrouter.ai/api/v1/images` | **U2 kapandı** (yol A) |
| W2 | Model listesi | `GET https://openrouter.ai/api/v1/images/models` | U5 kısmen |
| W3 | Endpoint/fiyat detayı | `GET https://openrouter.ai/api/v1/images/models/{author}/{slug}/endpoints` | U3 kısmen |
| W4 | Alternatif keşif | `GET https://openrouter.ai/api/v1/models?output_modalities=image` | U5 kısmen |
| W5 | Kimlik doğrulama | `Authorization: Bearer <OPENROUTER_API_KEY>` | **U2 kapandı** (yol A) |
| W6 | Normalize parametreler | `model` · `prompt` · `n` (1–10) · `resolution` (`512/768/1K/1.5K/2K/4K`) · `aspect_ratio` (`1:1,16:9,9:16,4:3,3:4,1:4,4:1,auto`) · `size` · `quality` (`auto/low/medium/high`) · `output_format` (`png/jpeg/webp/svg`) · `background` · `output_compression` (0–100) · `seed` · `stream` · `input_references` · `provider.*` · `user` | U6/U7 kısmen |
| W7 | Fiyat birimleri | endpoint kaydında `pricing[]`: `billable` (`output_image/input_image/input_reference`) · `unit` (`image/megapixel/token`) · `cost_usd` · `variant` | **U3 yapısal olarak kapandı** (tutarlar uç nokta bazında) |
| W8 | Fatura güvenliği | Başarısızlık/iptal → **502, ücret yok**; stream partial'ları kısmi faturalandırılmaz | risk azaltıcı |
| W9 | Streaming | `stream: true` (yalnız `supports_streaming: true` ise); SSE olayları: `image_generation.partial_image` · `image_generation.completed` (`b64_json`, `media_type`, `usage`, `cost`) · `error` · `data: [DONE]` | **U7 kapandı** (çıktı `b64_json`) |
| W10 | Seed | yetenek tanımında `seed: {type: boolean}`; yalnız destekleyen uç noktada deterministik | U6 kısmen |
| W11 | Model kapsamı | **30+** görsel modeli (Google · OpenAI · Black Forest Labs · Recraft · ByteDance · Sourceful · Microsoft · xAI); Image API duyurusu **2026-06-23** | U5 kısmen |

**⚠️ Çelişki (G10 — çözülmedi, iki kaynak farklı):** Seedream 4.5 birim fiyatı
kaynak 1'de **~$0.04/image**, kaynak 2'de **0.05 USD/image**. Bu ADR'de tek rakam
**seçilmez**; kesin fiyat `GET .../endpoints` yanıtından **canlı** okunmalıdır
(`cost_usd` alanı) → U3 hâlâ `DOĞRULAMA GEREKLI` (tutar düzeyinde).

**Bu bulgunun mimari etkisi:**

```text
  ImageRequest alanlari (S2.2)  <-->  OpenRouter parametreleri (W6)
  --------------------------------     -------------------------------
  Target: 3840x2160 / 16:9       <-->  resolution: "4K" + aspect_ratio: "16:9"
  NegativePrompt                 <-->  (bu endpoint'te AYRI alan YOK ->
                                        prompt icine gomme VEYA model bazli)
  ReferenceImage                 <-->  input_references[]
  Seed                           <-->  seed (yetenek tanimina bagli)
  Stream                         <-->  stream + SSE olaylari
```

> **Önemli boşluk:** OpenRouter Image API'de **negative prompt için ayrı
> parametre görünmüyor** (W6 listesinde yok). 8K generator'ın zorunlu
> `NEGATIVE PROMPT` bölümü (§4 V6/V7) bu endpoint'te **ya prompt metnine
> gömülür ya da model bazlı `provider.options` ile taşınır** → `DOĞRULAMA GEREKLI`.

**Kalan açık (yol A):** U4 (lisans/ticari kullanım) · U3'ün tutar düzeyi
(canlı `cost_usd`) · negative prompt taşıma yolu.

### §5.1.1 Yol A — canlı model envanteri (U5 ✅ KAPANDI, 2026-10-07)

`GET https://openrouter.ai/api/v1/images/models` canlı yanıtından **59 model**
çekildi (duyurudaki "30+" ifadesinden büyüktür — büyüme normal). Her model için
`supported_parameters` ayrı ayrı dönüyor:

| Aile | Örnek model ID'leri | Dikkat çeken yetenek |
|---|---|---|
| Black Forest Labs | `flux-3-image` · `flux.2-pro` · `flux.2-flex` · `flux.2-max` · `flux.2-klein-4b` | `seed` · `output_format` · `input_references` |
| ByteDance Seed | `seedream-4.5` · `seedream-5-0-flash/lite/pro` | `resolution 1K/2K/4K` · `seed` · 20 aspect_ratio |
| OpenAI | `gpt-image-1/1-mini/2/2.5-sunburst/2.5-flare` · `gpt-5-image(-mini)` · `gpt-5.4-image-2` | `quality` 4–6 kademe · `n` 1–10 · `output_compression` |
| Google | `gemini-3-pro-image(-preview)` · `gemini-3.1-flash-image(-preview/-lite)` · `gemini-2.5-flash-image` · `gemini-nano-banana-2.1` | `input_references` 0–14 |
| Recraft | `recraft-v3` · `v4(-pro/-vector/-styles/-utility)` · `v4.1(-flash/-pro/-vector)` | **`output_format: svg`** (vektör!) · `n` 1–6 |
| Microsoft | `mai-image-2.5(-pro)` · `mai-image-2.6(-flash)` | `aspect_ratio` 8 |
| xAI | `grok-imagine-image-2.0` · `-quality` | `resolution` · `quality low/medium` |
| Qwen / Krea / Sourceful / Meta / Tencent / InclusionAI | `qwen-image-3(-pro)` · `krea-2-*` · `riverflow-v2(-fast/-pro)/v2.5-*` · `muse-image` · `hy-image-3.5-preview` · `ming-image-0.1(-design-layer)` | karışık |

**Kritik bulgu (negatif prompt boşluğu teyidi):** 59 modelin **hiçbirinde**
`negative_prompt` / `negative` adlı bir `supported_parameters` anahtarı **YOK**.
→ §5.1'deki boşluk **tüm modeller için** geçerli; 8K generator'ın zorunlu
`NEGATIVE PROMPT` bölümü için tek yol **prompt metnine gömme** veya model bazlı
`provider.options` passthrough'u → hâlâ `DOĞRULAMA GEREKLI`.

**U5 sonucu:** `✅ KAPANDI` — envanter canlı çekildi (2026-10-07); ama model
listesi **değişkendir**, üretimde her oturumda `GET` ile tazelenmeli (bayatlık
riski, MEMORY L-06 mantığı).

### §5.2 Yol B doğrulaması — FLUX / Stable Diffusion / Midjourney (`WEB`, 2026-10-07)

> ⚠️ **Kaynak güveni:** Bu bulgular karşılaştırma/satış sitelerinden geldi;
> doğrulayan kaynak **kendisi** "fiyatları doğrudan bfl.ai ve stability.ai'den
> doğrulayın" uyarısı yapıyor. Bu yüzden fiyatlar **`NEEDS_VALIDATION`**,
> API varlığı iddiaları **`WEB (ikincil kaynak)`** etiketidir.

| # | Bulgu | Etiket | U etkisi |
|---|---|---|---|
| B1 | **FLUX (Black Forest Labs):** kendi API'si + aggregatorlar (fal.ai, Replicate); FLUX.2 · FLUX.2 Klein · FLUX.1 Kontext · FLUX 3 Image · Outpainting/Eraser/VTO | `WEB_VERIFIED` (bfl.ai) | **U2 kapandı (yol B1)** |
| B2 | **FLUX fiyat (birincil, `docs.bfl.ai/quick_start/pricing`):** kredi tabanlı **1 kredi = $0.01**; FLUX.2 [klein] 4B **$0.014 + $0.001/MP** · klein 9B **$0.015 + $0.002/MP** · FLUX.2 [pro] **$0.03** (edit $0.045) · [max] **$0.07** · [flex] **$0.05**; FLUX.1 Kontext [pro] **$0.04** / [max] **$0.08** · FLUX1.1 [pro] **$0.04** / Ultra **$0.06** / Raw **$0.06** · FLUX.1 Fill [pro] **$0.05**; çözünürlük **her zaman yukarı yuvarlanır** (1920×1080 = 2.07 MP → 3 MP); FLUX.2 [dev] **yalnız lokal** (barındırılmış API yok) | `WEB_VERIFIED` (birincil) | **U3 kapandı (yol B1)** |
| B3 | **Stability AI fiyat (birincil, `platform.stability.ai/pricing`):** **1 kredi = $0.01**; Stable Image **Ultra $0.08** (8 kredi) · **Core $0.03** (3 kredi) · SD 3.5 Large **$0.065** · Large Turbo **$0.04** · Medium **$0.035** · Flash **$0.025** · SDXL 1.0 **from $0.009** | `WEB_VERIFIED` (birincil) | **U3 kapandı (yol B2)** |
| B4 | **SD Community License:** yıllık geliri **<$1M** olan kuruluşlar için ücretsiz ticari kullanım + self-host/fine-tune | `WEB (ikincil)` | **U4 kısmen kapandı (yalnız SD)** |
| B5 | **Midjourney: resmi genel self-serve API YOK** — Discord veya kısıtlı kurumsal katman; erişim üçüncü taraf üzerinden | `WEB (ikincil)` | **U2 AÇIK/KISITLI (yol B'nin Midjourney kolu)** |
| B6 | Midjourney fiyat modeli: API başına değil **abonelik** ($10–120/ay) + üçüncü taraf maliyeti; gecikme ~30–60 sn | `NEEDS_VALIDATION` | U3 açık |
| B7 | Açık ağırlık/self-host: FLUX.2 [dev] + SD → **lisans U4'ün en net olduğu iki seçenek** | `WEB (bfl.ai)` | U4 için aday |

> **İkincil kaynak düzeltmesi:** önceki notta "~$0.03–0.07 **per megapixel**" ifadesi
> birincil kaynakla **birebir doğrulanmadı** — BFL'in FLUX.2 fiyatları büyük ölçüde
> **"from" (başlangıç) fiyatıdır**, yalnız **klein** ailesi açıkça `/MP` ek ücret taşır
> (`$0.001`/`$0.002` ilave MP başına). Bu yüzden fiyatlar **model bazında** yazıldı.

**Yol B mimari etkisi (önemli):**

```text
  YOL B = tek saglayici DEGIL, uc farkli durum:
  +----------------+---------------------------+------------------------+
  | FLUX (BFL)     | resmi API VAR             | adapter B-1            |
  +----------------+---------------------------+------------------------+
  | Stability SD   | resmi API VAR + lisans    | adapter B-2            |
  |                | NET (Community License)   | (U4 adayi)             |
  +----------------+---------------------------+------------------------+
  | Midjourney     | resmi public API YOK      | adapter B-3 = TEHLIKELI |
  |                | -> kullanilabilirlik      | kapsam disi BIRAKILABILIR|
  +----------------+---------------------------+------------------------+

  SONUC: "Midjourney destekle" ifadesi, resmi API olmadan
         UYGUNLANAMAZ -> ya kapsamdan cikarilir ya da
         [VERIFY REQUIRED] ile uc taraf entegrasyonu arastirilir.
```

**Kalan açık (yol B):** U2 (BFL/Stability resmi endpoint + auth, bfl.ai ve
stability.ai'den **birincil** kaynakla) · U3 (canlı fiyat) · U4 (BFL lisansı —
SD hariç) · **Midjourney kolunun kaderi** (kapsamda mı, değil mi → §8 Soru 3).

---

## §6 Sonuçlar (Kanıtlanmış / Tahmin)

### §6.1 Kanıtlanmış (`WEB_VERIFIED` — 2026-10-07)

| # | Sonuç | Kaynak |
|---|---|---|
| W-A1 | Yol A'nın endpoint'i, auth'u ve normalize parametre seti **mevcut ve doğrulanmış** | §5.1 W1–W6 |
| W-A2 | Fiyat **uç nokta bazlı** (`cost_usd`) okunabilir; kısmi faturalandırma yok | §5.1 W7–W8 |
| W-A3 | Çıktı biçimi `b64_json` + SSE streaming destekli (capability'ye bağlı) | §5.1 W9 |
| W-A4 | `input_references[]` ile `IMAGE_TO_IMAGE` / identity preservation **doğrudan destekli** | §5.1 W6 |
| W-A5 | `resolution`/`aspect_ratio` 8K generator hedefiyle (4K, 16:9) **örtüşüyor** | §5.1 W6 |
| W-A6 | **59 model canlı çekildi** — aile aile envanter §5.1.1'de (U5 kapandı) | `GET /api/v1/images/models` |
| W-A7 | **59 modelin hiçbirinde `negative_prompt` anahtarı yok** — boşluk tüm modeller için teyitli | §5.1.1 capability taraması |
| W-B1 | **Midjourney resmi public self-serve API'si yok** → yol B'nin Midjourney kolu uygulanamaz durumda | §5.2 B5 (`WEB ikincil`) |
| W-B2 | **SD Community License** yıllık gelir <$1M için ücretsiz ticari kullanım → U4'ün en net adayı | §5.2 B4 (`WEB ikincil`) |
| W-B3 | FLUX ve SD'nin **kendi resmi API'leri var**; FLUX dev + SD **self-host açık ağırlık** | §5.2 B1, B7 |

### §6.2 İkincil kaynak / doğrulanacak (`NEEDS_VALIDATION`)

| # | İddia | Durum (2026-10-07) |
|---|---|---|
| N1 | FLUX ve SD fiyatları | ✅ **KAPANDI** — birincil kaynak: `docs.bfl.ai/quick_start/pricing` + `platform.stability.ai/pricing` (§5.2 B2, B3). Eski "per-megapixel" çerçevesi düzeltildi |
| N2 | Midjourney abonelik $10–120/ay, gecikme 30–60 sn | ⚠️ **AÇIK** — ikincil kaynak; resmi fiyat sayfası doğrulanmadı |
| N3 | FLUX.1 Pro Ultra ~$0.06/image (fal.ai/Replicate) | ✅ **BİREBİR DOĞRULANDI** — BFL birincil `FLUX1.1 [pro] Ultra = $0.06` (aynı rakam) |

### §6.3 Tahmin (`INFERRED`)

| # | Sonuç | Güven |
|---|---|---|
| C1 | Port/adapter deseni ADR-028 ile uyumlu → ek `.csproj` gerekmez | Yüksek |
| C2 | Prompt üretimi ile sağlayıcı çağrısı ayrıldığından model listesi değişse bile `l0`/`l2` sabit kalır | Yüksek |
| C3 | Üç yol tek iskelette toplandığından B/C eklemesi yalnız `l4` adapter satırıdır | Orta-yüksek |
| C4 | Negative prompt'un ayrı alanı olmaması prompt tasarımını etkiler (59/59 modelde teyitli) | Yüksek (§5.1.1) |
| C5 | Model envanteri değişkendir → üretimde her oturumda `GET` ile tazelenmeli (bayatlık, MEMORY L-06) | Yüksek |
| C6 | Midjourney kolu kapsamdan çıkarılabilir; B1+B2 (FLUX+SD) yol B'yi tek başına taşır | Orta (§8 Soru 3 bekliyor) |

---

## §7 Riskler

| # | Risk | Etki | Tetikleyici | Erken belirti | Müdahale |
|---|---|---|---|---|---|
| R1 | Sağlayıcı kilitlenmesi (tek sağlayıcı kodu `l2`'ye sızmak) | Değişim pahalı | İlk adapter yazımı | `l2` içinde HTTP/URL | Port `l0`'a zorla, NetArchTest |
| R2 | Uydurma endpoint/fiyat | G9 ihlali, RED | §5 çözülmeden kod | Etiketsiz iddia | `DOĞRULAMA GEREKLI` + web doğrulama — **yol A için §5.1 ile büyük ölçüde kapandı**; yol B/C hâlâ açık |
| R3 | Lisans ihlali (ücretsiz sanıp ticari kullanım) | Hukuki risk | "ücretsiz" varsayımı | Kaynaksız "free" iddiası | U4 kapatılmadan yayına yok |
| R4 | Prompt↔sağlayıcı sözleşmesi tutmazsa | Prompt çöp olur | Model-specific syntax farkı | 7 farklı çıktı biçimi (V2) | Şema `l5`'te, adapter testleri |
| R5 | `visual-director` ajanı sağlayıcı bilgisine gömülür | Ajan değişimi kırılır | AGENTS §8.5'e endpoint yazmak | §8.5'te URL/anahtar | §8.5 yalnız **hat** taşır (öyledir) |

---

## §8 Açık Sorular (kullanıcıya — tek tur, A/B/C)

> **Tek soru kuralı** ([[definitions/role|ROLE]] §4): her tur **1 soru**, A/B/C, süreli.

**Soru 1 — Sağlayıcı stratejisi ✅ CEVAPLANDI (2026-10-07):**

```text
B1 - Gorsel uretim saglayicisi:  A) OpenRouter image endpoint
                                 B) Ayrilmis saglayici (Midjourney/FLUX/SDAPI)
                                 C) Harici CLI (External Agent deseni)
     KULLANICI CEVABI: "hepsi"  ->  kapsam = {A, B, C}   (U1 KAPANDI)
     Tasarim = A iskeleti (port/adapter); B ve C kendi
     adapter'lariyla eklenir — "gomme" hala elenmistir (S3).
```

**Soru 2 — Sıradaki tek soru (B2):**

```text
B2 - Simdi hangisini ONCE dogrulayayim?  A) YOL A once (OpenRouter image)
                                         B) YOL B once (Midjourney/FLUX/SDAPI)
                                         C) YOL C once (harici CLI)
      ~10 dk; secim U2/U3/U4 dogrulama sirasini belirler,
      ADR'yi accepted'a tasiyan ilk teknik adim budur.
```

**Soru 3 — Midjourney kolu (B5 bulgusundan doğdu, B2'den BAĞIMSIZ):**

```text
B3 - Midjourney resmi public API YOK. Kolu ne yapalim?
     A) Kapsamdan CIKAR (sadece FLUX + SD -> yol B tamamlanir)
     B) [VERIFY REQUIRED] ile uc taraf entegrasyonu arastir (sonra karar)
     C) Kapsamda TUT ama kodunu yazma (beklet, DOGRULAMA GEREKLI)
     ~5 dk; bu karar yol B'nin adapter sayisini belirler.
```

**Soru 4–9:** B2 cevabına göre sırayla — U2 endpoint/auth (birincil kaynak) ·
U3 fiyat/kota · U4 lisans (BFL) · U6 seed/parametre · U7 çıktı formatı ·
U8 görsel depolama yolu.

**Karar sırası:** `User Intent → Explicit Requirements → Constraints → Structure →
Correctness → Security → Maintainability → Performance → Simplicity`
(ai-agentic-promt-maker §19 ile aynı; ROLE §3 basamaklarıyla uyumlu).

### §8.1 Doğrulama sırası (hangi U hangi yolla kapanır — plan)

```text
  B2 cevabi (hangi yol ONCE)
        |
        v
  +-----+---------------------------------------------+
  | YOL A: OpenRouter image                          |
  |   U2 = endpoint + auth   -> resmi dokumantasyon x2 |
  |   U3 = fiyat/kota        -> WEB + tarih           |
  |   U4 = lisans            -> lisans metni          |
  |   U5 = models endpoint   -> GET /models           |
  +-----+---------------------------------------------+
        |  (her adim: WEB_VERIFIED - <URL> (2026-10-07))
        v
  ADR-031:  draft -> proposed (SIMDI OLDU) -> accepted (U2-U8 kapaninca)
        |
        v
  GATE-003 onayi -> src/** kodu (adapter A once)
```

---

## §9 ADR Durumu ve Kayıt

| Alan | Değer |
|---|---|
| **Durum** | **`proposed`** (2026-10-07 — kapsam kararı verildi) → `accepted` yalnız U2–U8 kapandıktan sonra |
| **Kapsam kararı** | **Üç yol birden** (A OpenRouter image · B ayrı sağlayıcılar · C harici CLI) — kullanıcı: "hepsi" |
| **Numara** | ADR-031 (sonraki boş numara; ADR-001..030 dolu) |
| **Supersede** | Yok — yeni karar, mevcut ADR'leri **değiştirmez** |
| **İlgili ADR'ler** | ADR-028 (katman) · ADR-011 (fallback/seq) · ADR-018 (secret) · ADR-010 (paylaşılan çekirdek) |
| **Kabul önkoşulu** | §5'teki U2–U8'in **üç yol için de** kapatılmış olması (U1 zaten kapandı) |
| **Kayıt yeri** | `.ai/.decisions/draft/` — `accepted` olunca `accepted/`'a taşınır + `adr-index.md`'ye satır |
| **adr-index kaydı** | **YAPILMADI** — `adr-index.md` diskte mevcut değil → `DOĞRULAMA GEREKLI` |

---

## §10 Doğrulama

- [ ] Frontmatter 11 alan · `status: proposed` · `version: 0.2.0` · `updated: 2026-10-07`
- [ ] §4'te 12 satır `SOURCE_VERIFIED` (dosya + § numarası ile)
- [ ] §5.1 2 kaynak `WEB_VERIFIED` + §5.1.1 **canlı** 59 model + §5.2 yol B `WEB (ikincil)` + `NEEDS_VALIDATION` ayrı işaretli
- [ ] G10 fiyat çelişkisi tek rakam seçilmeden kayıtlı (§5.1)
- [ ] §5 tablosunda U1/A-U2/U5/U7 ✅, B/C ve kalanlar açık; Midjourney 🔴 işaretli
- [ ] §6 üç katmanlı: `WEB_VERIFIED` / `NEEDS_VALIDATION` / `INFERRED` — karıştırılmamış
- [ ] §2/§3'te onaysız fiyat/lisans **kesin karar olarak** yazılmadı (G9)
- [ ] Kapsam {A,B,C} + tasarım tek iskelet ayrımı açık (§3)
- [ ] Diyagramlar Unicode box-drawing (`┌─┐│└┘`) — kullanıcı talimatı
- [ ] `src/**`'e bu ADR yüzünden **tek satır** yazılmadı (G12)
- [ ] §8 tek soru kuralı; Soru 1 ✅, Soru 2 (B2) + Soru 3 (B3) açık
- [ ] Kırık bağlantılar korunmuş (silme yok)

---

## §11 Referanslar

| # | Kaynak | Yol |
|---|---|---|
| 1 | Katman/proje baz kararı (bağlayıcı) | `.decisions/accepted/ADR-028-layered-baseline-9-projects.md` |
| 2 | Ajan matrisi · görsel üretim hattı | `AGENTS.md` §2, §8.5 |
| 3 | Studio/IDE kimliği | `VISION.md` §0.3 · `README.md` §1 |
| 4 | Anayasa (G5 · G8 · G9 · G12) | `CLAUDE.md` |
| 5 | Kapılar | `WORKFLOW.md` §2 |
| 6 | Karar hiyerarşisi · tek soru | `ROLE.md` §3–§4 |
| 7 | Prompt kaynağı — görsel analiz | `G:/…/Chatgpt/girl-image-analzye-promt.md` §10–§17, §20–§29 |
| 8 | Prompt kaynağı — 8K generator | `G:/…/Chatgpt/girl-image-8k-generator-wallpeprs-promt.md` §21–§24 |
| 9 | Prompt kaynağı — render studio | `G:/…/Chatgpt/ai-vision-render-studio.md` |
| 10 | Prompt kaynağı — mimari/sınırlar | `G:/…/versacoder proje planlaması ai.08.md` §49–§51 |
| 11 | Prompt kaynağı — adaptif soru/öncelik | `G:/…/Chatgpt/ai-agentic-promt-maker.md` §19 |
| 12 | **Web** — OpenRouter Image API duyurusu (2026-06-23) | https://openrouter.ai/blog/announcements/image-api/ |
| 13 | **Web** — OpenRouter Image Generation dokümanı | https://openrouter.ai/docs/guides/overview/multimodal/image-generation |
| 14 | **Web (canlı API)** — görsel model envanteri (59 model) | `GET https://openrouter.ai/api/v1/images/models` (2026-10-07) |
| 15 | **Web (ikincil)** — FLUX/SD/Midjourney API karşılaştırması | apiframe.ai + apiscout.dev 2026 rehberi |
| 16 | **Web (birincil)** — BFL FLUX API fiyatı (kredi + model bazlı) | https://docs.bfl.ai/quick_start/pricing |
| 17 | **Web (birincil)** — BFL fiyat hesaplayıcı | https://bfl.ai/pricing |
| 18 | **Web (birincil)** — Stability AI API fiyatı (kredi sistemi) | https://platform.stability.ai/pricing |

**Doğrulama etiketleri:** §4 = `SOURCE_VERIFIED` (prompt arşivi, 2026-10-07) ·
**§5.1 + §5.1.1 = `WEB_VERIFIED — <URL> (2026-10-07)` (2 doküman + canlı API)** ·
**§5.2 = `WEB (ikincil kaynak)` + fiyatlar `NEEDS_VALIDATION`** ·
§6.1 = `WEB_VERIFIED` · §6.2 = `NEEDS_VALIDATION` · §6.3 = `INFERRED` ·
§2.2 sözleşme alan adları = `INFERRED` (kavramsal) ·
§5 kalan U'lar (A: U3-tutar/U4/negative; B: U2/U3/U4-BFL; C: hepsi) =
`DOĞRULAMA GEREKLI` · §9 adr-index kaydı = `DOĞRULAMA GEREKLI` (dosya diskte yok).
| engine | [[definitions/engine|engine]] | orkestrasyon motoru — giris -> hazirlik -> dagitim -> kontrol -> teslim |
| PROJECTS | [[definitions/projects|PROJECTS]] | proje envanteri — 9+1-2 proje, dis bagimliliklar, risk defteri |
| glossary | [[definitions/glossary|glossary]] | terim sozlugu (136 terim) TR / EN / ELI10 |
| keys | [[governance/keys|keys]] | anahtar kelime -> hedef dosya hiz haritasi (101 satir) |
| MEMORY | [[memory/memory|MEMORY]] | kalici dersler, karar desenleri, kara liste, kayit defteri |
| log | [[ops/log|log]] | append-only is gunlugu + Quality Report |
| ULTRA | [[definitions/ultra-thinking|ULTRA]] | uzun dusunme protokolu — 5 adim, kanit hiyerarsisi, red-team |
| kok CLAUDE | [kok CLAUDE](../../../claude.md) | kok kural dosyasi (golge — kural metni degil, baglanti) |
| kok AGENTS | [kok AGENTS](../../../agents.md) | kok ajan golgesi (Chat!=Agent!=LLM baglantisi) |
| kok WORKFLOW | [kok WORKFLOW](../../../workflow.md) | kok kapi ozeti (gate sirasi tek satir) |
