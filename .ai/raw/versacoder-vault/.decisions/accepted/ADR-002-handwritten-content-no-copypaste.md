---
title: "ADR-002 — Handwritten Content, No Copy-Paste"
type: adr
category: adr
status: accepted
adr_id: ADR-002
date: 2026-09-25
updated: 2026-09-25
version: 1.0.0
authority: SSOT
tags: [vault, content, copy-paste, ssot, process, handwritten, forbidden-sources]
---

# ADR-002 — Handwritten Content, No Copy-Paste

**Zorunlu Bağlantılar:** [ADR Defteri](../adr-index.md) · [Vault Ana Sayfa](../../index.md) · [Anayasa](../../CLAUDE.md) · [Anahtar Kelime Haritası](../../keys.md) · [Kapsam Analizi](../../reports/00-kapsam-analizi.md) · [ADR-001](ADR-001-dual-target-net48-net10.md) · [ADR-003](ADR-003-vault-structure-500md-frontmatter.md) · [ADR-004](ADR-004-devexpress-universal-v261-office2010.md) · [ADR-005](ADR-005-ado-net-dapper-no-ef-core.md)

> **Tek cümlelik sorumluluk:** *Vault içeriği hangi kaynaktan gelir, "yalnız referans" tam olarak neyi serbest bırakır, hangi kaynaklara asla yazılmaz* sorusunun tek cevabıdır.

---

## §1 Kapsam ve Kullanım

### §1.1 — Ne zaman kullanılır

1. `.ai/` altına **yeni bir MD dosyası** yazılırken (içerik nereden geliyor?).
2. Bir şablon (`.templates/`) doldurulurken — şablon **iskelet** midir, **içerik** midir?
3. Ekrana/wireframe'e metin yazılırken ([ADR-003](ADR-003-vault-structure-500md-frontmatter.md) kapsamı).
4. Yedek vault'tan "bilgi" okunurken — ne alınır, ne alınmaz?
5. `*.old.*` kopyaları veya başka bir `.ai` dizini看到 edildiğinde.
6. Review'da bir dosyada **yabancı dil/ton/ürün adı** görüldüğünde (tarama §6.2).

### §1.2 — Yasaklı kaynaklar (salt okunur, asla yazılmaz)

| # | Kaynak | Durum | Alınabilir | Alınamaz |
|---|---|---|---|---|
| 1 | `C:\www\coremusic.net\.ai` | 🔒 salt okunur | **YAPI**: klasör düzeni, dosya tipi, ekran listesi, gate seti | tek kelime cümle, paragraf, ton |
| 2 | `C:\www\versacoder\versacoder.ai` (+ `.ai.01..04`) | 🔒 salt okunur | **BİLGİ**: ne tartışılmış, gate tanımı, fikir | metin bloğu, başlık dizilimi |
| 3 | `*.old.*` kopyaları (her dizinde) | 🔒 salt okunur | yalnız dosya adı/format gözlemi | herhangi içerik |
| 4 | ai.01/ai.02 (eski plan) | ⚠️ yalnız **yapıbilgi** | ekran listesi, gates, mimari şema | **teknoloji sürümleri** (`.NET 8`, `24.2.x`, EF Core) |
| 5 | ai.03 bootstrap | ✅ birincil kaynak | 5 senior karar metni | — |

> **UYARI:** 1–3 numaralı kaynaklara **yazma yok** (`Copy-Item` dâhil). İhlalde: dosya silinir, sıfırdan yazılır (G1 · `CLAUDE.md` §1).

### §1.3 — "Yalnız referans" ne demektir

| Serbest (yapıbilgi) | Yasak (içerik) |
|---|---|
| Klasör/kategori düzeni, dosya adlandırma kalıbı | Cümle, paragraf, madde metni |
| Ekran/wireframe **listesi** ve gate adları | Ekran içi **açıklama metni**, buton copy'si |
| Mimari katman **sırası** ve ASCII şema iskeleti | ASCII şemanın birebir kopyası |
| "Bu dosya şunları içermeli" (içindekiler) | "Şunlar şöyle yapılır" (anlatım) |
| Doğrulanmış **olgu** (dosya 599 MD, 15 kök MD) | Onun **anlatımı** (kaynağın cümlesi) |
| Türkçe/İngilizce terim eşlemesi (`glossary.md`) | Kaynağın örnek cümleleri |

**Ölçüt (tek cümle testi):** Kaynağın dosyasını yanına açıp cümleyi **yan yana** koy; ≥8 kelime boyunca aynı sıra varsa → **kopyadır**, yeniden yaz. Olgu aynı, cümle farklıysa → serbest.

### §1.4 — Kapsam ve kapsam dışı

| Kapsam | Kapsam dışı |
|---|---|
| `.ai/**/*.md` üretim ve düzenleme anı | Ürün kaynak kodu (`src/`) — ayrı kural (`.rules/coding-standards.md`) |
| Şablon doldurma (iskelet serbest, metin elle) | Kullanıcı tarafından **verilen** prompt/metin (SOURCE_VERIFIED) |
| Görsel/ASCII diyagram (saf ASCII, elle çizim) | Açık kaynak **altyapı** kodu (lisanslı paket, NuGet) |
| Yedek vault'tan **bilgi** okuma biçimi | Kütüphane dokümantasyonundan doğrulanan olgular |

---

## §2 Bağlam (Context)

### §2.1 — Karar ortamı

| Boyut | VersaCoder gerçeği | Kaynak |
|---|---|---|
| Yapı referansı | coremusic `.ai` = **599 MD**, 15 kök MD (30–94 KB), 16 klasör | `reports/00-kapsam-analizi.md` §1.2 |
| Bilgi referansı | `versacoder.ai` + `.ai.01..04` ≈ 350+ MD, içerik **tutarsız** | aynı rapor (ör. brain: 80 KB vs 2 KB) |
| Yasak | `Copy-Item` ile `.ai` kopyalamak · blok yapıştırma · "aynısınıVersaCoder için" | `CLAUDE.md` G1 |
| Aday ADR'ler (yedek) | 11 ADR listesi vardı; ADR-001 "güncellenmeli", ADR-002 "İPTAL" | rapor §4.2 |
| Eski plan | ai.01 BÖLÜM 6 `.ai` yapısı **CoreMusic'e özgü** | rapor §2/2 |
| Hedef ölçek | ~500+ MD, elle yazılacak | [ADR-003](ADR-003-vault-structure-500md-frontmatter.md) |
| Bağlayıcı kural | **G1** — Kopyalama Yasak (SSUT) · G7 — CoreMusic'e yazma yasağı | `CLAUDE.md` §1 |

### §2.2 — Kararı tetikleyen tetikleyiciler

1. İki hazır `.ai` vardı (599 MD + 350 MD); **kopyalamak en hızlı yoldu** — ama prompt bu yolu açıkça yasakladı ve iki vault da VersaCoder'a ait değildi (yanlış ürün adı, yanlış domain).
2. Yedek vault **içerik kalitesizdi**: aynı dosya iki yerde 80 KB vs 2 KB (tutarsızlık) — kopyalansa **kötü kalite** devralınacaktı (rapor §2/2).
3. Eski plan (ai.01) `.ai` yapısını CoreMusic'e özgü tanımlıyordu; VersaCoder'ın kendi akışı (7 ekran, gate seti) farklıydı.
4. ~500 MD elle yazılacak — en yavaş yol; **süre riski R4** doğrudan bu kararın bedeli.
5. AI ile "toplu doldurma" (boilerplate üretimi) teklif edilebilirdi: hızlı ama **kanıtsız ve özdeş** içerik üretir (G5/G9 riski).

### §2.3 — Masadaki seçenekler (≥2 alternatif)

Masada dört seçenek vardı: **(A)** hepsini elle, sıfırdan yaz (SEÇİLDİ) · **(B)** CoreMusic vault'unu kopyala · **(C)** versacoder.ai yedeğini kopyala · **(D)** AI boilerplate ile üret. Tam eleme §3.2'dedir. Bağlam ayrımı önemli: B ve C birbirinden farklıdır — B **yapı** için cazip, C **bilgi** için cazip; ikisi de tek satır metin taşırsa G1 ihlali doğar.

### §2.4 — Kapsam dışı

1. Ürün kodunun yeniden yazımı (kod zaten yok — `src/` PART 5).
2. Doğrulanmış dış olgular (resmi dokümantasyon, NuGet sürümü) — `WEB_VERIFIED` etiketiyle serbest.
3. Kullanıcının bizzat verdiği metinler (prompt, onay cümleleri) — `SOURCE_VERIFIED`.
4. `.templates/` iskeletlerinin varlığı — iskelet serbesttir (§1.3), doldurma elle.

---

## §3 Karar (Decision)

### §3.1 — Karar ifadesi (tek cümle)

**Tüm `.ai/` vault içeriği elle ve sıfırdan yazılır; yedek/CoreMusic/`*.old.*` kaynakları yalnızca yapı-bilgi için salt okunur okunur, hiçbir metin bloğu kopyalanmaz — karar verildi.**

### §3.2 — Alternatifler

| # | Alternatif | Artı | Eksi | Neden elendi |
|---|---|---|---|---|
| A — Elle, sıfırdan yazım (SEÇİLDİ) | ton tutarlı, her cümle sahipli, kanıtlanabilir (G5), VersaCoder'a özel | en yavaş yol; R4 süre riski | **elenmedi** — tek yol G1'i ve kalite çıtasını tutturuyor |
| B — CoreMusic vault'unu kopyala | 599 MD hazır; yapı + içerik | **G1 ihlali** (RED); yanlış ürün/domain; yasa ihlali | açık yasak → elendi |
| C — versacoder.ai yedeğini kopyala | VersaCoder konusuna yakın | iç tutarsızlık (80 KB vs 2 KB brain); blok kopya = G1 ihlali | "yalnız bilgi" sınırı ihlal → elendi |
| D — AI boilerplate ile top üretim | en hızlı, maliyet 0 | özdeş/kanıtsız metin; G5 (kanıtsız tamam) + G9 (uydurma) riski; 500 MD'de kalite dağılır | kanıt şartı → elendi |
| E — Karışım: iskelet kopyala + cümle yaz (kısmi) | hız + temizlik | "iskelet" ile "içerik" sınırı yoruma açık; drift riski | sınır zaten A'da tanımlı, ayrı yol gerekmiyor → elendi |

### §3.3 — Gerekçe

Kısıt: içerik VersaCoder'a ait olmalı, her cümle savunulabilir olmalı ve çelişen iki kaynaktan (ai.01 vs ai.03) biri G10 ile çözülmeliydi. Ölçülebilir etki: elle yazılan dosyada **tek yabancı blok yok**; `keys.md`/`index.md` tonu ile yeni dosya tonu aynıdır. Elenen B'nin bedeli: yasak ihlali + yanlış ürün adı ("CoreMusic") sızması → dosyanın silinip yeniden yazılması (iş iki kat). C'nin bedeli: tutarsız kalitenin (80 KB/2 KB) devralınması ve gizli kopya izinin kalıcı olması. D'nin bedeli: kanıtsız "tamam" (G5) — 500 MD'de hata yüzdesi 10 olsa 50 bozuk dosya demektir. E sınırı çizse de, A zaten §1.3'te aynı çizgiyi çizdiği için ayrı bir yol **YAGNI**'dır.

### §3.4 — Referans okuma protokolü (adım adım)

```text
1. Kaynagi AC  (salt okunur -- hicbir sey yazma)
2. Dosyanin YAPISINI incele: klasor, dosya adi, ici-kilikler, gate adlari
3. KAPAT / baska pencereye al
4. Bizim dosyayi SIFIRDAN yaz (kaynaga bakmadan cumle kurma)
5. Yalniz olgulari DOGRULA: dosya adi / sayi / tarih / komut ciktisi
6. Yan yana test: >= 8 kelimelik ayni sira var mi?  --EVET--> sil, yeniden yaz
7. Etiketle: REPOSITORY_VERIFIED | SOURCE_VERIFIED | INFERRED | NEEDS_VALIDATION
```

| Adım | Çıktı | Hata halinde |
|---|---|---|
| 2 | yapı notu (dosya listesi, içindekiler) | — |
| 4 | yeni metin | kaynağa bakıp cümle kurulmuşsa → adım 6'ya dön |
| 5 | olgu satırları | doğrulanamayan olgu → `⚠️ VERIFICATION REQUIRED` |
| 6 | 0 benzerlik | ≥8 kelime sıra = kopya → dosya yeniden yazılır |

### §3.5 — Kabul kriterleri (ölçülebilir)

| # | Kriter | Ölçüt | Kanıt |
|---|---|---|---|
| KR1 | Yasaklı kaynaklara **yazma** yok | `C:\www\...` dizinlerinde değişiklik 0 (git/`LastWriteTime` denetimi) | §5.3/1 |
| KR2 | Kopya blok yok | ≥8 kelimelik eşleşen sıra = 0 (spot check) | §5.3/2–3 |
| KR3 | Ürün adı sızıntısı yok | vault'ta `CoreMusic` yalnız "yasak" bağlamında | §5.3/4 |
| KR4 | Eski teknoloji sızıntısı yok | `.NET 8` / `24.2.x` / `EF Core 9` yalnız "elenen" bağlamında | §5.3/5 |
| KR5 | Ton tutarlılığı | yeni dosya `index.md`/`keys.md` tonu ile uyumlu | §6.1/7 manuel |
| KR6 | Kanıt etiketi | her olgusal iddia etiketli (G9) | §6.1/8 |

### §3.6 — Bağımlılıklar ve ön koşullar

| # | Bağımlılık | Durum |
|---|---|---|
| 1 | `CLAUDE.md` G1 (kopyalama yasağı) — bu ADR'nin dayanağı | ✅ okundu 2026-09-25 |
| 2 | `reports/00-kapsam-analizi.md` §1.2 (kaynak envanteri) | ✅ okundu 2026-09-25 |
| 3 | [ADR-003](ADR-003-vault-structure-500md-frontmatter.md) — 500 MD hedefi = bedelin kaynağı | ✅ kabul edildi |
| 4 | `scripts/link-scan.ps1` + diff raporu (G1 kanıtı) | ⚠️ VERIFICATION REQUIRED — PART 3/8 |
| 5 | `.templates/` hazır iskeletler (yapı referansı) | ✅ diskte (24+ dosya) |

### §3.7 — Riskler ve azaltıcılar

| Risk | Olasılık | Etki | Azaltıcı |
|---|---|---|---|
| **R4:** ~500 MD elle yazım → süre aşımı | yüksek | yüksek | şablon başına bir kez yaz, katmanlara dağıt (rapor §3.1) |
| Yavaşlık → "hızlı kopya" cazibesi | orta | yüksek | G1 otomatik + manuel denetim; yasak net (§1.2) |
| Bilgi okurken istemeden cümle taşıma | orta | orta | §3.4 yan yana testi (≥8 kelime) |
| Eski teknoloji sızıntısı (ai.01) | orta | yüksek | KR4 taraması; `.rules/` hard rule (R3) |
| Ürün adı sızıntısı | düşük | yüksek | KR3 taraması (§5.3/4) |
| AI ile üretilmiş özdeş içerik | orta | orta | KR2 + kanıt etiketi (G5/G9) |

### §3.8 — Geri alma (reversibility)

Karar **geri alınabilir** (içerik tek tek silinip yeniden yazılabilir), ancak **kopya tespit edilen dosya için geri alma = yeniden yazım**dır:

| # | Tetik | Geri alma | Süre |
|---|---|---|---|
| 1 | Bir dosyada kopya blok bulundu | dosya silinir, sıfırdan yeniden yazılır (G1 ihlalde zorunlu) | dosya başına ~1–3 saat |
| 2 | Yasaklı dizinde istemsen yazma | `git status`/diff ile geri al + kullanıcıya rapor | ~10 dk |
| 3 | Tone/ton uyuşmazlığı | yalnız o dosya yeniden yazılır | ~1 saat |

### §3.9 — Karar veren ve danışılanlar

| Rol | Kişi/Varlık |
|---|---|
| Karar veren | architect (senior karar #2 — `reports/00-kapsam-analizi.md` §2) |
| Kaynak | prompt yasağı + ai.01 BÖLÜM 6 analizi |
| Danışılan | reviewer (ton ve benzerlik incelemesi) |
| Uygulayan | architect (vault üretimi) |
| Reviewer | reviewer (§5.3 taramaları) |

### §3.10 — Açık sorular ve izleme metrikleri

| # | Soru / Metrik | Hedef | Sıklık |
|---|---|---|---|
| Q1 | `link-scan.ps1` + **diff raporu** otomasyonu ne zaman hazır? (G1 kanıtı) | PART 3/8 | açılışta |
| M1 | Yasaklı dizin değişikliği | 0 | her oturum sonu |
| M2 | Benzerlik spot check (≥8 kelime) | 0 | her 10 dosyada 1 |
| M3 | Ürün adı / eski sürüm sızıntısı | 0 | her PR |
| M4 | `⚠️ VERIFICATION REQUIRED` açık madde | düşüş eğilimi | haftalık |

---

## §4 Korkular ve Guardrails

### §4.1 — Bu kararın korkuları

1. **En kötü korku:** yasaklı vault'a istemsen **yazma** (`Copy-Item`, editör kaydı) → geri alınamaz kirlilik (G7).
2. İkinci: "sadece yapıyı aldım" derken **cümle** de gelir — kayan içerik gözden kaçar.
3. Üçüncüsü: süre baskısı (R4) → biri "hızlıca yapıştırayım" der; G1 ihlali sessizce doğar.
4. Dördüncüsü: eski plandan **teknoloji** sızması (`.NET 8`, EF Core) → G8 ihlali.
5. Beşincisi: AI üretimi özdeş metnin 500 dosyaya yayılması → kanıtsız "tamam" (G5).

### §4.2 — Risk matrisi

| Risk | Olasılık | Etki | Skor |
|---|---|---|---|
| Yasaklı dizine yazma | düşük | çok yüksek | **HH** |
| İstemsen kopya kayması | orta | yüksek | **H** |
| R4 süre aşımı | yüksek | yüksek | **H** |
| Eski teknoloji sızıntısı | orta | yüksek | **H** |
| Ürün adı sızıntısı | düşük | yüksek | **H** |
| Özdeş AI içerik | orta | orta | M |

### §4.3 — Azaltma planı

1. **Yazma koruması:** `C:\www\...` yollarına yönelik her işlem **salt okunur**; gerektiğinde dosya attribute/izin ile korunur, denetim `LastWriteTime` ile (§5.3/1).
2. **Otomatik tarama:** `scripts/link-scan.ps1` + benzerlik/ürün adı taraması → Part 3 ve Part 8'de zorunlu (R6 ile birlikte).
3. **Yazım ritmi:** `.templates/` iskeleti bir kez kurulur, içerik dalga dalga yazılır (R4 azaltıcısı); "toplu üretim" kapısı kapatılır.
4. **İkinci göz:** reviewer her 10 dosyada 1 spot check (KR2/KR5).
5. **Cezai sonuç:** ihlalde dosya silinir ve yeniden yazılır — pazarlık yok (G1).

### §4.4 — Kabullenilemez kriterler (otomatik RED)

| # | Kriter | İhlalde |
|---|---|---|
| 1 | Yasaklı dizinde dosya değişikliği | CRITICAL — geri al + rapor (G7) |
| 2 | ≥8 kelimelik kopya blok | RED — dosya yeniden yazılır (G1) |
| 3 | Vault'ta "CoreMusic" ürün adı (yasak bağlamı dışında) | RED — sil |
| 4 | `.NET 8` / `24.2.x` / `EF Core` seçilen teknoloji olarak | RED — G8 + [ADR-004](ADR-004-devexpress-universal-v261-office2010.md)/[ADR-005](ADR-005-ado-net-dapper-no-ef-core.md) |
| 5 | Kanıtsız "tamam/üretildi" iddiası | RED — G5 |
| 6 | `Copy-Item` ile `.ai` üretimi | CRITICAL — G1 |

### §4.5 — Guardrail Tablosu (AG1–AG14)

| # | Guardrail | Kural | İlgili CLAUDE.md | İhlal Sonucu |
|---|---|---|---|---|
| AG1 | Frontmatter 10 alan eksiksiz | `authority: SSOT` | G4 | RED |
| AG2 | Slug: `handwritten-content-no-copypaste` | Türkçe gövde, İngilizce slug | G4 | RED |
| AG3 | Secret yok | bu dosyada bilgi/ambar yok | G9 | kritik RED |
| AG4 | Yasaklı kaynak tablosu dolu | §1.2 5 satır | **G1** | RED |
| AG5 | Kanıtsız iddia `⚠️` | §6.3 kanıt satırları | G9 | RED |
| AG6 | Referans ≠ kopya testi | §3.4 ≥8 kelime kuralı | **G1** | RED |
| AG7 | Numara tekilliği | `ADR-002` tek dosya | — | RED |
| AG8 | Slug tekrarı yok | defter ile aynı slug | — | RED |
| AG9 | Supersede zinciri | `superseded_by` boş (ezilmedi) | — | RED |
| AG10 | KR1–KR6 dolu | ölçülebilir | G5 | RED |
| AG11 | Link temiz | kırık link 0 | G4 | RED |
| AG12 | Defter senkronu | `adr-index.md` satırı var | G4 | RED |
| AG13 | Yeniden yazım kaydı | ihlalde `log.md` tek satır | G1 | RED |
| AG14 | Yalnız-oku denetimi | §5.3 komutları yazmaz | G7 | CRITICAL |

---

## §5 Uygulama ve Hata Modları

### §5.1 — Uygulama adımları (11 adım)

| # | Adım | # | Adım | # | Adım |
|---|---|---|---|---|---|
| 1 | Yasaklı kaynak listesi (§1.2) | 5 | Dosyayı sıfırdan yaz (Türkçe, `index.md` tonu) | 9 | `adr-index.md` kapsam eşlemesi |
| 2 | `.templates/` iskeleti = yalnız iskelet | 6 | Olgu satırlarını doğrula + etiketle | 10 | §6.1 18 madde kontrolü |
| 3 | Yapıbilgiyi topla (klasör, gate, ekran listesi) | 7 | Yan yana benzerlik testi (§3.4/6) | 11 | `log.md` tek satır (dosya sayısı + süre) |
| 4 | Kaynağı kapat / ikinci pencereye al | 8 | Ürün adı / eski sürüm taraması (§5.3/2–4) | | |

### §5.2 — Hata modları

| # | Hata modu | Belirti | Düzeltme |
|---|---|---|---|
| 1 | Yasaklı dizine yazma | `LastWriteTime` değişmiş | geri al + rapor (G7) |
| 2 | İstemsen kopya | ≥8 kelimelik sıra | dosyayı sıfırdan yaz |
| 3 | Ürün adı / eski teknoloji sızıntısı | `CoreMusic` · `.NET 8` · `24.2.x` | sil ya da "elenen" bağlamına taşı |
| 4 | Özdeş AI içerik | dosyalar birbirinin aynı | çeşitlendir + kanıt ekle |
| 5 | Süre aşımı (R4) | 500 MD yarım kaldı | dalga planı: katman başına |
| 6 | Kanıtsız olgu | sayı/tarih tutmuyor | `⚠️` veya doğrula (G9) |
| 7 | Kırık link / ton sapması | `Test-Path` = False · broşür tonu | yalnız var olana link; `index.md` tonu |

### §5.3 — Doğrulama komutları (yalnız-oku)

```powershell
# 1 -- yasakli dizinlere YAZMA OLMAMALI (cikti bos olmali)
Get-ChildItem 'C:\www\coremusic.net\.ai','C:\www\versacoder\versacoder.ai' -Recurse -File -ErrorAction SilentlyContinue |
  Where-Object { $_.LastWriteTime -gt (Get-Date).AddHours(-12) } | Select-Object FullName, LastWriteTime
# 2 -- urun adi sizintisi (yalniz "yasak" baglami serbest -- elle bak)
Select-String -Path .ai\*.md,.ai\decisions\*.md,.ai\decisions\accepted\*.md -Pattern 'coremusic' -CaseSensitive:$false
# 3 -- eski teknoloji sizintisi (secilen teknololi olarak GECMEMELI)
Select-String -Path .ai\*.md -Pattern '\.NET 8|24\.2\.[0-9]|EF Core 9'
# 4 -- eski TFM/ORM sizintisi (yalniz "elenen" baglami)
Select-String -Path .ai\**\*.md -Pattern 'net8\.0|EntityFrameworkCore' 
# 5 -- Copy-Item izi (vault uretiminde KULLANILMAMALI)
Select-String -Path .ai\*.md -Pattern 'Copy-Item'
# 6 -- kopya benzerlik spot ornegi (elle: 10 dosyada 1)
Get-ChildItem .ai -Recurse -Filter *.md | Sort-Object LastWriteTime -Descending | Select-Object -First 10 FullName
```

> **UYARI:** Komutlar **yalnız-oku**dur. 2–5 numaralı çıktılar **önce bağlama bakılarak** okunur: yasak/elenen bağlamındaki geçişler serbesttir; seçilen teknoloji/ürün adı olarak geçen her satır §4.4'e düşer.

---

## §6 Doğrulama ve Testler

### §6.1 — Kontrol listesi (18 madde)

- [ ] 1. Frontmatter 10 alan eksiksiz · 2. `authority: SSOT` birebir
- [ ] 3. H1 + **Zorunlu Bağlantılar** satırı (9 bağlantı, tümü diskte var)
- [ ] 4. §1–§7 iskeleti eksiksiz · 5. §3.1 karar tek cümle (`… karar verildi.`)
- [ ] 6. §3.2 alternatif ≥2 satır (5 satır), 4 sütun dolu
- [ ] 7. Ton `index.md`/`keys.md` ile uyumlu (manuel) · 8. Her olgusal iddia etiketli (G9)
- [ ] 9. §1.2 yasaklı kaynak tablosu 5 satır dolu · 10. §1.3 serbest/yasak sınırı net (≥8 kelime)
- [ ] 11. §3.4 referans okuma protokolü 7 adım · 12. KR1–KR6 ölçülebilir
- [ ] 13. §3.7 risklerde R4 + kopya cazibesi · 14. §3.8 geri alma tablosu dolu
- [ ] 15. AG1–AG14 "İhlal Sonucu" sütunlu
- [ ] 16. 11 uygulama adımı + ≥7 hata modu + 6 yalnız-oku komutu
- [ ] 17. §7'de **gerçek itiraz + cevap** var · 18. Mojibake yok; yasaklı ad yalnız yasak bağlamında

### §6.2 — Test stratejisi

| Tip | Kapsam | Yöntem | Beklenen |
|---|---|---|---|
| Content similarity | yeni dosyalar | §3.4 yan yana testi (≥8 kelime) | 0 eşleşme |
| Forbidden source | `C:\www\...` | §5.3 komut 1 | 0 yazma |
| Product name / legacy stack | tüm vault | komut 2–4 | yalnız yasak/elenen bağlamı |
| Link | tüm iç link | `Test-Path` döngüsü | 0 kırık |
| Tone + evidence (manual) | 10 dosya örneklemi | `index.md` tonu + olgu doğrulama | uyumlu · açık `⚠️` = 0 |

### §6.3 — Kanıt tablosu

| Kanıt | Konum | Durum |
|---|---|---|
| G1 metni (kopyalama yasağı) | `CLAUDE.md` §1/G1 | ✅ okundu 2026-09-25 |
| Kaynak envanteri (599 MD / ≈350 MD) | `reports/00-kapsam-analizi.md` §1.2 | ✅ SOURCE_VERIFIED |
| Yedek tutarsızlığı (80 KB vs 2 KB) | aynı rapor §2/2 | ✅ SOURCE_VERIFIED |
| Yasaklı dizinlere yazma 0 | `C:\www\...` | ⚠️ VERIFICATION REQUIRED — her oturum komut 1 ile |
| `scripts/link-scan.ps1` | `.ai/scripts/` | ⚠️ VERIFICATION REQUIRED — PART 3 |
| Benzerlik otomasyonu | — | ⚠️ VERIFICATION REQUIRED — manuel spot check ile telafi |

### §6.4 — Doğrulama komutları ve şablona özgü kontroller

§5.3 komutları (1–6): **1 = 0 satır**, **2–4 = yalnız yasak/elenen bağlamı**, 5–6 manuel okunur; sonuç `log.md`'ye tek satır.

1. **Tek cümle testi:** §1.3'teki ≥8 kelime kuralı bu ADR'nin **kalbidir**; test yoksa karar denetlenemez.
2. **Yapıbilgi/içerik ayrımı** her üretilen dosyada uygulanır (şablon doldurma dâhil); okuma sonrası kaynak kapatılır (§3.4/3).
3. **Ölçek:** ~500 MD hedefi ([ADR-003](ADR-003-vault-structure-500md-frontmatter.md)) ile R4 dalga planı bağlayıcıdır.
4. **Yasak vault adı:** `CoreMusic` yalnız bu ADR'nin ve `.rules/`/`CLAUDE.md`'nin yasak bağlamında geçer.
5. **Boyut bandı:** bu dosya 15–25 KB → `(Get-Item .ai\decisions\accepted\ADR-002-*.md).Length`.

---

## §7 Sonuç ve Red Team

### §7.1 — Sonuç özeti

| Alan | Sonuç |
|---|---|
| Karar | Tüm vault içeriği elle, sıfırdan; yedek/CoreMusic/`*.old.*` yalnız yapı-bilgi, salt okunur |
| Durum | `accepted` · Versiyon 1.0.0 · Tarih 2026-09-25 |
| Sahip | architect · Reviewer: reviewer |
| İlgili ADR'ler | [ADR-003](ADR-003-vault-structure-500md-frontmatter.md) (ölçek = bedel) · [ADR-001](ADR-001-dual-target-net48-net10.md) · [ADR-004](ADR-004-devexpress-universal-v261-office2010.md) · [ADR-005](ADR-005-ado-net-dapper-no-ef-core.md) · [Defter](../adr-index.md) |
| Etkilenen G'ler | **G1** · G5 · G7 · G8 · G9 |
| İlişkili risk | **R4** · R6 · varsayım A2 |
| Sonraki adım | `link-scan.ps1` + diff raporu (Q1) — PART 3/8 |

### §7.2 — Red Team (bu karar nasıl yanlış gider?) — gerçek itiraz + cevap

> **RED TEAM:** *"500+ MD'yi elle yazmak gerçekçi değil: ekip ilk 50 dosyada hız kazanmak için 'sadece yapısını alıyoruz' diyerek CoreMusic'ten blok taşıyacak, sonra bu durum normalleşecek ve kimse fark etmeyecek — çünkü denetim otomatik değil, insan gözüne bağlı. Yani bu karar ya süre aşımıyla (R4) ya da gizli kopyayla sonuçlanır; ikisi de G1'i kağıt üstünde tutar, pratikte öldürür. 'Yan yana 8 kelime' kuralı da kopyanın 9. kelimesinde başlamasını engellemez."*

**Cevap (rebuttal):** İtiraz üç noktada haklı ve bu yüzden savunma "dikkatli olun" değil, **ölçülebilir kapılar** kuruyor. (1) **Normalleşme korkusu:** haklı — bu yüzden §4.4/1'de yasaklı dizine yazma **CRITICAL** ve §5.3 komut 1 ile her oturum sonunda `LastWriteTime` taranır; gizli kopya eninde sonunda **dosyanın metni**nde değil, **kaynak dosyanın zaman damgasında** görünür. (2) **8 kelime kuralı zayıf:** kısmen haklı — bu yüzden kural tek başına değil, **üçlü**dür: benzerlik testi + ürün adı taraması (komut 2) + eski teknoloji taraması (komut 3–4). Kopya yapan bir blok, VersaCoder tonuyla ve doğru ürün adıyla uyuşmak zorunda; kırılma üç taramadan birinde görünür. (3) **Süre aşımı:** tamamen haklı — bu ADR'nin **bilinçli bedeliidr** ve R4 olarak §3.7'de kayıtlıdır; azaltma "kopyalamak" değil, `.templates/` iskeleti + dalga planıdır (rapor §3.1). Özet: itiraz kararın **zayıf noktasını doğru teşhis ediyor**, ama savunma onu "iyi niyete" değil, **otomatik ölçüme** bağladığı için senaryo gözetlenebilir kalır.

| # | İtiraz | Senaryo | Savunma |
|---|---|---|---|
| 1 | Elle yazım sürmez | 500 MD yarım kalır | R4 + dalga planı; şablon bir kez |
| 2 | Kopya normalleşir | "sadece yapı" derken cümle gelir | komut 1 (zaman damgası) + KR2 |
| 3 | 8 kelime kuralı atlatılır | 9. kelimeye bölünür | üçlü tarama (benzerlik + ad + sürüm) |
| 4 | Denetim insan gözüne bağlı | reviewer atlar | §5.3 komutları otomatik; göz yalnız bağlam kararı |
| 5 | AI üretimi durdurulamaz | özdeş 500 dosya | KR6 + ton testi + kanıt etiketi |
| 6 | Yedek vault "bilgi" diye çerçeve genişler | içerik taşır | §1.3 sınırı + ≥8 kelime testi |
| 7 | Ürün adı hatası müşteriye sızar | sunumda yasaklı ad | KR3 — kritik RED, sil |
| 8 | Referans okuma yavaşlatır | 7 adım çok | adım 2–4 ≈ 5 dk/dosya; otomasyon zaten kapalı |

### §7.3 — Onay ve dağıtım

| Rol | İsim | Tarih | İmza |
|---|---|---|---|
| Yazar | architect | 2026-09-25 | — |
| Uygulayan | architect | 2026-09-25 | — |
| Mimari sorumlu | architect | 2026-09-25 | — |
| Reviewer | reviewer | 2026-09-25 | — |

---

**Zorunlu Bağlantılar (tekrar):** [ADR Defteri](../adr-index.md) · [Vault Ana Sayfa](../../index.md) · [Anayasa](../../CLAUDE.md) · [Kapsam Analizi](../../reports/00-kapsam-analizi.md)
