---
title: "VersaCoder — Kök CLAUDE.md / AGENTS.md Şablonu"
type: template
category: template
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
scope: "kucuk CLAUDE.md / AGENTS.md sablonu: baglam, kurallar ve komut kisayollari"
---

# VersaCoder — Kök CLAUDE.md / AGENTS.md Şablonu

**Zorunlu Bağlantılar:** [index](../../index.md) · [CLAUDE.md](../../CLAUDE.md) · [architecture-template](../architecture/architecture-template.md) · [ascii-diagram-template](../architecture/ascii-diagram-template.md) · [adr-template](../adr/adr-template.md) · [katman-readme-template](katman-readme-template.md)

---

## §1 Amaç

`C:\versacoder` depo kökündeki **`CLAUDE.md`** ve gölge dosyası **`AGENTS.md`**'nin tek üretim kaynağıdır. Her ajan oturumu bu dosyayı **ilk satırda** okur; yoksa vault görmez. Kök dosya belge değil **çalışma sözleşmesidir**: kim olduğum, vault nerede, hangi katman var, neyi asla yapmam.

SRP gereği kök dosya **kural metni üretmez**; kural `.ai/.rules/`, karar `.ai/adr/`, şablon `.ai/.templates/` içindedir, kök yalnız **bağlantı verir** (envanter G12).

| Alan | Değer |
|------|-------|
| Template Name | `claude-md-template.md` |
| Template Path | `.ai/.templates/documentation/claude-md-template.md` |
| Hedef Dosya | `C:\versacoder\CLAUDE.md` (asıl) · `AGENTS.md` (gölge, §1.3) |
| Hedef Dizin | depo kökü — `.ai/` DIŞINDA tek MD dizini |
| Üretilen Uzunluk | `CLAUDE.md` 120–220 satır · `AGENTS.md` 10–20 satır |
| Kodlama | **UTF-8 (BOM'suz)** — Latin-1/CP1254 YASAK |
| Dil | Türkçe (ç ğ ı İ ö ş ü doğru) — dosya yolu/katman adı İngilizce |
| Zorunlu Blok | 10 blok `B1..B10` (§3.1) |
| Bağlam Bütçesi | 50.000 token/oturum — kök dosya payı ≤ %3 (§1.4) |
| Versiyon · Authority | `1.0.0` · `SSOT` (bu dosya) |
| Bootstrap | 2026-09-25 |

### §1.1 Okuma Sırası — Zincirin İlk Halkası

Ajan zinciri tek yönlüdür: kök dosya → envanter → kural → iş. İlk halka eksikse sonrakiler **hiç okunmaz**; bu yüzden kök dosya doküman değil **önyükleme**dir.

| Sıra | Okunan | Ne verir | Tekrar Eder mi |
|------|--------|----------|----------------|
| 1 | `CLAUDE.md` (bu şablon) | rol, vault bağları, katman listesi, yasaklar | Hayır — yalnız bağlar |
| 2 | `.ai/.templates/index.md` | 44 şablon envanteri + §3.7 kalite sözleşmesi | Hayır |
| 3 | `.ai/.rules/*.md` (7 MD) | kalıcı kural metni | Hayır |
| 4 | `.ai/architecture/index.md` · `WORKFLOW.md` | katman gerçeği · kapı gerçeği | Hayır |
| 5 | Görevin kendi şablonu | üretilcek dosyanın iskeleti | Hayır |

**Boşluk riski:** 3–5. dosyalar diskte yok (§1.2/6–9). Bu durumda kök dosya **uydurma yapmaz**, olmayan hedefe `⚠️ VERIFICATION REQUIRED` yazar ve sırayı **planlanmış** işaretler.

### §1.2 Disk Kanıtları ve Boşluk (YAGNI)

İddia → kanıt. Kanıt yoksa uydurulmaz. Ölçüm: `C:\versacoder`, 2026-09-25, `Test-Path` + `Get-ChildItem`.

| # | İddia | Durum | Kanıt |
|---|-------|-------|-------|
| 1 | `C:\versacoder\CLAUDE.md` | ❌ YOK | `Test-Path = False` — bu şablonla üretilecek |
| 2 | `C:\versacoder\AGENTS.md` | ❌ YOK | `Test-Path = False` |
| 3 | `C:\versacoder\README.md` | ❌ YOK | `Test-Path = False` |
| 4 | `src/` (9 VersaCoder projesi) | ❌ YOK | `Test-Path = False` → §3.4 **⚠️ VERIFICATION REQUIRED** |
| 5 | `tests/` (K8 test projeleri) · `.sln` | ❌ YOK | `Test-Path = False` |
| 6 | `global.json` (SDK 10.0.401) + `Directory.Packages.props` (CPM) | ❌ YOK | kökte yalnız `.ai`, `.git` |
| 7 | `versacoder.db` (SQLite) | ❌ YOK | `Test-Path = False` |
| 8 | `.ai/` kök MD'leri (`index.md`, `architecture/index.md`, `WORKFLOW.md`, `keys.md`, `log.md`) | ❌ YOK | `.ai/` altında yalnız `.templates` + `reports` |
| 9 | `.ai/.rules/` (7 MD) · `.ai/adr/` (ADR-001…008) | ❌ DİZİN YOK | `Test-Path = False` |
| 10 | `.ai/.templates/index.md` (envanter + §3.7 sözleşme) | ✅ DOĞRULANDI | 40.440 bayt |
| 11 | `.ai/.templates/CLAUDE.md` (dizin kuralı) | ✅ DOĞRULANDI | 7.047 bayt |
| 12 | `ascii-diagram-template.md` (biçim altın örneği) | ✅ DOĞRULANDI | 38.118 bayt / 644 satır |
| 13 | `.ai/.templates/documentation/` (bu şablonun dizini) | ✅ DOĞRULANDI | 3 şablon (docs-md · readme · root-md) |
| 14 | `.ai/reports/00-kapsam-analizi.md` | ✅ DOĞRULANDI | 9.711 bayt |
| 15 | Kopyalama yasağı kapsamındaki harici vault'lar | 🔒 ERİŞİM YASAK | yalnız **yapı** referansı; tek kelime kopyalanmadı (G1) |
| 16 | Bağlantı hedefi `.ai/keys.md` (secret sözlüğü) | ❌ YOK → `⚠️ VERIFICATION REQUIRED` | değer asla kök dosyaya girmez |

**Yorum yasağı:** Var olmayan dosya için "şurada tanımlı" denmez; `⚠️ VERIFICATION REQUIRED` yazılır. §1.2/1–9 satırları **üretilen** `CLAUDE.md`'de de görünür: bağlantı verilir, varlık iddia edilmez.

### §1.3 CLAUDE.md ile AGENTS.md Arasındaki Fark

İki dosyanın da aynı kural metnini taşıması iki gerçek kaynağı doğurur. Bu şablon tek kaynağı seçer: `CLAUDE.md` yazar, `AGENTS.md` yalnız **gösterir**.

| Dosya | Rol | Üretilen İçerik | Güncelleme |
|-------|-----|-----------------|------------|
| `CLAUDE.md` | asıl — ilk okunan | B1..B10, 120–220 satır | kural değişince |
| `AGENTS.md` | gölge — aracıların beklediği ad | 10–20 satır: H1 + "Kaynak: CLAUDE.md" + 3 bağlantı | asla ayrı |
| `.ai/.templates/CLAUDE.md` | `.templates/` içi dizin kuralı | ayrı dosya, bu şablon **değil** | envanter değişince |
| `.ai/CLAUDE.md` | ikinci kök dosya **YASAK** | — | — |

**Kural:** `AGENTS.md`'ye tek satır kural **kopyalanmaz**; kopya görürsen dosya 10–20 satıra indirilir (G5).

### §1.4 Bağlam Bütçesi ve Yaşam Döngüsü

Kök dosya her oturumda bütçeden yer kaplar; uzarsa **iş değil okuma** yemiş olur.

| Öğe | Değer |
|-----|-------|
| Oturum bütçesi | 50.000 token |
| `CLAUDE.md` payı | ≤ 1.500 token (%3) — > 2.000 → §4.3 RED |
| Üretilen satır | 120–220 — < 100 iskelet · > 260 doküman |
| Bağlantı | 6 zorunlu — 5 altı eksik · 8 üstü dağınık |
| Katman satırı | 9 (K0..K8) — 8 altı eksik |
| Güncelleme tetiği | kural / katman / rol değişimi → `updated` + `log.md` |

```text
+----------------------------------------------------------------+
| [1] CLAUDE.md oku       rol + yasak + katman listesi           |
|    v                                                           |
| [2] index.md oku        44 şablon envanteri + §3.7 sözleşme    |
|    v                                                           |
| [3] .rules/*.md oku      7 kural dosyası (metin burada)         |
|    v                                                           |
| [4] ARCHITECTURE + WORKFLOW   katman gerçeği + kapı gerçeği    |
|    v                                                           |
| [5] işe başla   <== KAPI: bütçe 50.000 token / oturum          |
+----------------------------------------------------------------+
```

**Ne anlatır:** Ajan önyükleme sırası; `[5]` bütçe kapısı. **Durum:** `[3]`–`[4]` diskte YOK (§1.2/8–9) → planlanmış sıra, `⚠️ VERIFICATION REQUIRED`.

---

## §2 Kapsam

**İçinde:** kök `CLAUDE.md` iskeleti, gölge `AGENTS.md`, blok sırası, bağlantı dili, bağlam bütçesi, kök dosyaya özgü doğrulama.
**Dışında:** kural metni (→ `.ai/.rules/`), karar (→ `.ai/adr/`), envanter (→ `[[../index]]`), katman spec'i (→ `[[../architecture/architecture-template]]`), kod.

| Kapsam | Kapsam Dışı |
|--------|-------------|
| B1..B10 blok sırası + her bloğun zorunlu içeriği | Kuralın **kendisi** (yalnız bağlantısı) |
| 6 zorunlu bağlantı + wiki dili (`[[…]]`) | ADR karar metni (→ `adr-template`) |
| K0..K8 listesinin kök dosyadaki özeti | katman iç sınıf tasarımı (→ `layer-spec`) |
| Bağlam bütçesi + uzunluk eşiği | ajan rol dosyaları (→ `agents-template`) |
| Gölge `AGENTS.md` kuralı (tek gerçek) | `.ai/` içine ikinci kök dosya (**YASAK**) |
| 6 salt-okunur PowerShell komutu | üretilen dosyanın iş içeriği (göreve göre) |

### §2.1 Dosya Tipi → Sorumlu Eşlemesi

| Dosya Tipi | Sorumlu | Bu Şablondan mı |
|------------|---------|------------------|
| `C:\versacoder\CLAUDE.md` (kök) | master-orchestrator | ✅ |
| `C:\versacoder\AGENTS.md` (gölge) | master-orchestrator | ✅ (§1.3) |
| `C:\versacoder\README.md` | docs-writer | ❌ → `readme-template` |
| `.ai/.templates/CLAUDE.md` | master-orchestrator | ❌ ayrı şablon |
| `.ai/.rules/*.md` | security · developer | ❌ kural ≠ kök dosya |
| `.ai/documentation/**/*.md` | docs-writer | ❌ → `docs-md-template` |
| `src/**/README.md` (katman) | docs-writer | ❌ → `katman-readme-template` |

### §2.2 Kapsam Sınırı Testi

**İlk `EVET`** şablonu belirler:

1. Dosya **depo kökünde** mi, adı `CLAUDE.md` / `AGENTS.md` mı? → **EVET** (bu şablon)
2. `src/` içinde bir katmanı mı anlatıyor? → `katman-readme-template`
3. Bir API arayüzünün imzası mı? → `api-doc-template`
4. `wiki/` altında bir sayfa mı? → `WikiPage-Template`
5. `.ai/reports/security-audit-*` mı? → `security-audit-template`
6. Hiçbiri değilse bu şablon **kullanılmaz**; talep §5.1 Adım 1'den açılır.

`HAYIR` soruları: kural metni içeriyor mu (→ `.ai/.rules/`)? · harici vault'tan cümle var mı (G1)? · `{{…}}` dışarıda mı (G4)?

---

## §3 Mimari

### §3.1 Zorunlu Blok Sırası (B1..B10)

Üretilen `CLAUDE.md` on bloktur; sıra **değiştirilemez** (sıra = okuma önceliği). Eksik blok RED (§6.1 K5).

| # | Blok | İçerik | Min Satır |
|---|------|--------|-----------|
| B1 | `# H1` + **Zorunlu Bağlantılar** | 6 bağlantı, `·` ayracılı | 2 |
| B2 | Tek cümlelik sorumluluk | `>` ile 1–2 cümle | 2 |
| B3 | Rol tanımı | neyi üstlenir / üstlenmez | 6 |
| B4 | Vault bağları | dizin + okuma sırası | 8 |
| B5 | Katman listesi | K0..K8, 9 satır | 11 |
| B6 | Kurallar özeti | G1..G12 tek satır + bağlantı | 14 |
| B7 | Ne yap / Ne yapma | iki sütun ≥ 10 satır | 12 |
| B8 | Dosya yolları | yol + disk durumu + tarih | 12 |
| B9 | Bütçe + güncelleme | 50.000 token, tetikler | 6 |
| B10 | Doğrulama | 2 komut özeti + Quality Report | 8 |

### §3.2 Frontmatter, H1 ve Bağlantı Satırı

| Öğe | Kural | Doğru | Yanlış |
|-----|-------|-------|--------|
| Frontmatter | kök dosyada 7 alan, ilk 9 satır | `authority: SSOT` | alan eksik / yazım farkı |
| H1 | tek tane, `# ` | `# VERSA CODER — Kök Kural Dosyası` | iki H1 · `##` |
| Bağlantı | `**Zorunlu Bağlantılar:**` + 6 `[[…]]` | `·` ayracılı | 5 altı · markdown `](…)` |
| Dil | Türkçe; slug/yol İngilizce | `katman listesi` · `src/` | karışık dil |
| Ürün adı | `VERSACODER` / VersaCoder | — | **CoreMusic ASLA** (G1) |

Kök dosya `.ai/` dışında olduğu için bağlantı yolu `.ai/...` ile başlar, `/` kullanılır (`\` yazılmaz).

### §3.3 B3 — Rol Tanımı Bloğu

İki paragrafı geçmez; zorunlu dört öğe: görev, sınır, çıkış kriteri, kapı.

| Öğe | Zorunlu Kalıp | Yasak Kalıp |
|------|---------------|-------------|
| Görev | "Bu depoda … sorumluyum." | "… olabilirim." (belirsiz) |
| Sınır | "… yapmam; yalnız bağlantısını veririm." | kural metni tekrarı |
| Çıktı | "… çıktım … biçimindedir." | "hızlı", "temiz" (ölçülmemiş) |
| Kapı | "… geçmeden … üretilmez." | kapı adı uydurma |
| Bütçe | "Oturum bütçesi 50.000 token." | farklı sayı |

### §3.4 B4+B5 — Vault Bağları ve K0..K8 Katman Listesi

Katman listesi `architecture-template` §3.2 ile **birebir** aynıdır; kök dosyada yalnız **özet** satırı durur, sorumluluk metni katman şablonundadır (SRP). `src/…` yolları diskte yok → §1.2/4.

| Kod | Katman | Proje | Tek satır sorumluluk | Disk |
|-----|--------|-------|----------------------|------|
| K8 | Test | `tests/VersaCoder.Tests`, `tests/VersaCoder.IntegrationTests` | kanıt üretir, hedef %90 coverage | ❌ `⚠️` |
| K7 | Host | `VersaCoder.Host` | composition root + DI + `appsettings*.json` | ❌ `⚠️` |
| K6 | UI | `VersaCoder.Desktop`, `VersaCoder.Cli` | RibbonForm sunumu / CLI ayrıştırma | ❌ `⚠️` |
| K5 | Protocol | `VersaCoder.Protocol` | mesaj şeması + tool sözleşmeleri | ❌ `⚠️` |
| K4 | Infrastructure | `VersaCoder.Infrastructure` | ADO.NET + Dapper, dosya, git, LLM | ❌ `⚠️` |
| K3 | Application | `VersaCoder.Application` | use case + CQRS handler | ❌ `⚠️` |
| K2 | Domain | `VersaCoder.Domain` | entity + invariant + domain event | ❌ `⚠️` |
| K1 | Abstractions | `VersaCoder.Abstractions` | port arayüzleri + `Result<T>` | ❌ `⚠️` |
| K0 | CrossCutting | `VersaCoder.CrossCutting` | logging · `IClock` · yardımcılar | ❌ `⚠️` |

**Yön (tek satır):** `K6 → K7 → {K5, K4} → K3 → K2 → K1 → K0`; yukarı bakan referans **ihlaldir**. Bu cümle B5'nin son satırıdır.

### §3.5 B6 — Kurallar Özeti (bağlantıdır, metin değil)

Her kural **tek satır** özetlenir; ayrıntı `.ai/.rules/` ve `index.md` §4.5'tedir. `.rules/` diskte YOK (§1.2/9) → satır `⚠️` taşır.

| ID | Tek satır özet | Ayrıntı Bağlantısı |
|----|----------------|--------------------|
| G1 | Harici vault'tan içerik kopyalanamaz; yalnız yapı referans | `[[../index]]` §4.5 |
| G2 | **EF Core yasak** — ADO.NET + Dapper | `[[../adr/adr-template]]` ADR-004 |
| G3 | Frontmatter'sız Markdown yasak (7 alan) | `[[../index]]` §3.7 |
| G4 | Üretilen dosyada `{{…}}` = 0 | `[[../index]]` §3.9 |
| G5 | Türkçe zorunlu; mojibake yasak | `[[../index]]` §4.5 |
| G6 | < 480 satırlık şablon reddedilir | `[[../index]]` §6.1 K14 |
| G7 | Bozuk bağlantı yasak; `.ai/` içine ikinci kök yok | `[[../index]]` §5.3 K6 |
| G8 | `GATE-003` onaysız ekran/kod üretilmez | `[[../CLAUDE]]` |
| G9 | ADR'siz mimari karar yok | `[[../adr/adr-template]]` |
| G10 | SQL'de concat yasak; `@p1..@pN` zorunlu | `[[../query/Query-Template]]` |
| G11 | `TreatWarningsAsErrors=true` + %90 coverage | `[[../index]]` §4.5 |
| G12 | Envantere içerik kopyalanamaz (SRP) | `[[../index]]` §1.1 |

### §3.6 B7 — Ne Yap / Ne Yapma

| ✅ Yap | ❌ Yapma (ilk satırda DUR) |
|--------|----------------------------|
| Bağlantı ver, kuralı **okumaya** git | Kural metnini kök dosyaya kopyala |
| Katmanı K0..K8 koduyla söyle | Katman adı uydur / numarayı kaydır |
| Olmayana `⚠️ VERIFICATION REQUIRED` yaz | Olmayan dosyayı "şurada" diye göster |
| 6 bağlantıyı tek satırda `·` ile tut | 8+ bağlantı · markdown `](…)` karışık |
| Tarihi ISO yaz (`YYYY-MM-DD`) | Serbest tarih biçimi |
| Sürümü ADR'den al (`26.1`) | "en güncel sürüm" kanıtsız iddiası |
| `@p1..@pN` örneğini göster | String concat SQL örneği ver |
| Ölç, sonra "tamam" de (§5.3) | Kanıtsız "hizalı/doğru" iddiası |
| Ürün adı `VERSACODER` / VersaCoder | **CoreMusic**'i hiçbir yerde geçirmek |
| İş bitince `log.md`'ye tek satır düş | Kök dosyayı her oturumda yeniden yaz |

### §3.7 B8 — Dosya Yolları ve Disk Durumu

| # | Yol (depo köküne göre) | Ne İçin | Var (2026-09-25) |
|---|------------------------|---------|------------------|
| 1 | `CLAUDE.md` | bu şablonun çıktısı | ❌ üretilecek |
| 2 | `AGENTS.md` | gölge dosya (§1.3) | ❌ üretilecek |
| 3 | `.ai/.templates/index.md` | envanter + §3.7 sözleşme | ✅ 40.440 bayt |
| 4 | `.ai/.rules/*.md` | kural metni (7 MD) | ❌ dizin yok |
| 5 | `.ai/architecture/index.md` | katman gerçeği | ❌ YOK |
| 6 | `.ai/WORKFLOW.md` | kapı gerçeği (`GATE-004a -> 003 -> 004 -> 005 -> 006`) | ❌ YOK |
| 7 | `.ai/keys.md` | secret sözlüğü (değer kök dosyaya girmez) | ❌ YOK |
| 8 | `.ai/adr/ADR-NNN-*.md` | kararlar (ADR-001…008) | ❌ dizin yok |
| 9 | `.ai/log.md` | Quality Report defteri | ❌ YOK |
| 10 | `src/VersaCoder.<Katman>/README.md` | katman özeti | ❌ dizin yok |

### §3.8 B10 — Doğrulama Kalıbı (bağlantı + boyut)

**Bağlantı kırık mı, bütçe aşıldı mı?** Komutlar salt-okunurdur; özet üretilen dosyaya gömülür, tam metin burada yaşar (SRP).

```powershell
$doc = 'C:\versacoder\CLAUDE.md'
if (-not (Test-Path -LiteralPath $doc)) { 'CLAUDE.md YOK -> bu sablonla uretilecek'; return }
$t = Select-String -LiteralPath $doc -Pattern '\[\[([^\]]+)\]\]' -AllMatches |
  ForEach-Object { $_.Matches } | ForEach-Object { $_.Groups[1].Value } | Sort-Object -Unique
$kirik = $t | Where-Object { -not (Test-Path "C:\versacoder\$($_ -replace '/','\').md") }
'Baglanti {0} · kirik {1} (0 olmali) · satir {2} (120-220)' -f $t.Count, @($kirik).Count, (Get-Content $doc).Count
$kirik | ForEach-Object { '  KIRIK -> ' + $_ }
```

### §3.9 Dolu VersaCoder Örneği ve Tam Kalıp

> Üst blok üretim standardı, alt blok `{{…}}` taşıyan iskelettir. Yer tutucu **yalnız** alt blokta yaşar.

````markdown
# VERSA CODER — Kök Kural Dosyası
**Zorunlu Bağlantılar:** [index](.templates/index.md) · [CLAUDE](.templates/CLAUDE.md) · [index](architecture/index.md) · [WORKFLOW](WORKFLOW.md) · [coding-standards](.rules/coding-standards.md) · [adr-index](.templates/adr/adr-index.md)

> Bu depoda VersaCoder ürününün kodunu ve kanıtını üretirim; kuralı burada tekrarlamam, yalnız bağlantısını veririm.

## Kurallar Özeti (metin `.ai/.rules/`'tedir)
| ID | Tek satır özet | Bağlantı |
|----|----------------|----------|
| G1 | Harici vault'tan içerik kopyalanamaz | [index](.templates/index.md) |
| G10 | Concat SQL yasak — `@p1..@pN` | [Query-Template](.templates/query/Query-Template.md) |

**Bağlam bütçesi:** 50.000 token/oturum · bu dosya payı ≤ %3 · güncelleme: kural/katman/rol değişince.
````

**Tam kalıp (üretilecek kök dosyanın iskeleti):**

````markdown
---
title: "VERSA CODER — Kök Kural Dosyası"
type: doc
category: root
status: active
version: 1.0.0
updated: {{YYYY-MM-DD}}
authority: SSOT
---
# VERSA CODER — Kök Kural Dosyası
**Zorunlu Bağlantılar:** [{{BAG_1}}]({{BAG_1}}) · [{{BAG_2}}]({{BAG_2}}) · [{{BAG_3}}]({{BAG_3}}) · [{{BAG_4}}]({{BAG_4}}) · [{{BAG_5}}]({{BAG_5}}) · [{{BAG_6}}]({{BAG_6}})
## B2 Sorumluluk
{{TEK_PARAGRAF — ne yapıyorum, ne yapmıyorum}}
## B3 Rol Tanımı
{{GOREV}} · {{SINIR}} · {{CIKTI}} · {{KAPI}} · {{BUTCE}}
## B5 Katman Listesi
{{K0..K8 — 9 satır, architecture-template §3.2 ile birebir}}
## B6 Kurallar Özeti
| ID | Tek satır özet | Bağlantı |
|---|---|---|
| {{G_ID}} | {{OZET}} | [{{BAGLANTI}}]({{BAGLANTI}}) |
## B7 Ne Yap / Ne Yapma
{{IKI SUTUN — 10 satir}}
## B8 Dosya Yolları
{{YOL + VAR/YOK + TARİH}}
## B9 Bağlam Bütçesi
{{50.000 token · tetikler}}
## B10 Doğrulama
{{2 komut özeti + Quality Report}}
````

---

## §4 Kurallar

### §4.1 Yazım Sırası

| # | Adım | Kural | İhlal |
|---|------|-------|-------|
| 1 | Önce `.templates/` | şablon yoksa kök dosya üretilmez | RED |
| 2 | Blok sırası B1..B10 | blok yer değiştirmez (§3.1) | RED |
| 3 | Bağlantılar sonra | kural metni **asla** kopyalanmaz | RED (G12) |
| 4 | Disk durumu son | §1.2 üretim anında yeniden ölçülür | `⚠️` kalır |
| 5 | Ölçüm | §5.3/1 (bağlantı) + §5.3/2 (boyut) | RED |

### §4.2 Sorumluluk ve Onay

| Aşama | Rol | Ne Onaylar | Süre |
|-------|-----|-----------|------|
| 1 | docs-writer | iskelet + frontmatter (B1..B10) | 20 dk |
| 2 | reviewer | §6.1 20 madde | 10 dk |
| 3 | architect | K0..K8 `architecture-template` ile birebir | 6 dk |
| 4 | master-orchestrator | §5.3 çıktıları + §6.4 Quality Report | 8 dk |
| | **Toplam** | | **≈ 44 dk** |

> **3 RED** sonrası iş durur; gerekçe `master-orchestrator`'a yükseltilir.

### §4.3 Ölçek / Standartlar

| Öğe | Değer | Sapma |
|-----|-------|-------|
| Üretilen `CLAUDE.md` | 120–220 satır | < 100 iskelet · > 260 RED |
| Üretilen `AGENTS.md` | 10–20 satır (gölge) | kopya içerirse RED (§1.3) |
| Bağlantı | tam 6 | 5 altı / 8 üstü RED |
| Katman satırı | 9 (K0..K8) | 8 altı RED |
| Kurallar özeti | 12 (G1..G12) | eksik RED |
| Token payı | ≤ 1.500 (%3) | > 2.000 RED (§1.4) |
| Bu şablon | 480–660 satır · 25–34 KB | dışı §5.3/2 |
| Kodlama | UTF-8 BOM'suz | Latin-1/CP1254 otomatik RED |

### §4.4 Değişiklik Kontrolü

| Olay | Aksiyon | Etkilenen | Kayıt |
|------|---------|-----------|-------|
| Kural özeti değişti | B6 satırı + bağlantı | `CLAUDE.md` | `version` minor + `log.md` |
| Katman eklendi | önce `architecture-template`, sonra B5 | iki dosya | `log.md` + §6.5 K5 |
| Bağlanan dosya taşındı | bağlantı hedefi yeniden yazılır | `CLAUDE.md` | §5.3/1 = 0 |
| `.ai/` içine ikinci kök dosya | **DUR** — silinir | — | §4.5 G7 |

### §4.5 Hard Guardrails (Otomatik DUR)

| # | Guardrail | İhlal Sonucu |
|---|-----------|--------------|
| G1 | Harici vault'tan (`C:\www\…`) cümle kopyalanması | **DUR** — dosya silinir, özgün yazılır |
| G2 | `EntityFramework` / EF Core referansı (özet bile olsa) | **DUR** — ADR-004, ADO.NET + Dapper |
| G3 | Frontmatter 7 alan eksik / `authority: SSOT` bozuk | **DUR** — §5.3/5 |
| G4 | Üretilen kök dosyada `{{…}}` (kalıp bloğu hariç) | **DUR** — §5.3/5 |
| G5 | `AGENTS.md` içinde kural metni kopyası | **DUR** — 10–20 satırlık gölge |
| G6 | Üretilen dosya < 100 / > 260 satır | **DUR** — §4.3 |
| G7 | `.ai/` içine ikinci kök kural dosyası | **DUR** — tek kök `CLAUDE.md` |
| G8 | `GATE-003` onayı olmadan ekran/kod | **DUR** — `WORKFLOW.md` (YOK → `⚠️`) |
| G9 | ADR vermeden mimari/karar belgesi | **DUR** — `.ai/adr/` (YOK → önce açılır) |
| G10 | String concat SQL (`"… " + x`) | **DUR** — `@p1..@pN` |
| G11 | `TreatWarningsAsErrors` / %90 coverage özetinin kaybı | **DUR** — B6'da kalır |
| G12 | Kural metninin kök dosyaya taşınması | **DUR** — bağlantıya çevrilir |
| G13 | `CoreMusic` adının geçmesi | **DUR** — `VERSACODER` / VersaCoder |
| G14 | Kanıtsız "bağlantılar sağlam" beyanı | **DUR** — §5.3 çıktıları istenir |

---

## §5 Workflow

### §5.1 Adım Bazlı Akış

| # | Adım | Girdi | Çıktı | Yapan | Süre |
|---|------|-------|-------|-------|------|
| 1 | Disk ölçümü | `Test-Path` listesi (§1.2) | güncellenmiş kanıt tablosu | docs-writer | 5 dk |
| 2 | İskelet seç | §3.1 (B1..B10) | blok listesi | docs-writer | 3 dk |
| 3 | Bağlantıyı yaz | 6 zorunlu hedef | H1 altı satır | docs-writer | 4 dk |
| 4 | B5 katmanları | `architecture-template` §3.2 | 9 satır (K0..K8) | architect | 5 dk |
| 5 | B6 özetleri | `index.md` §4.5 | 12 satır | security | 8 dk |
| 6 | B7–B8 doldur | ne yap/ne yapma + yollar | 2 tablo | docs-writer | 8 dk |
| 7 | Gölge oluştur | `CLAUDE.md` | `AGENTS.md` 10–20 satır | docs-writer | 4 dk |
| 8 | Ölç | §5.3/1–§5.3/6 | 0 kırık · 0 sızan | reviewer | 6 dk |
| 9 | Onay + kayıt | §6.1 20/20 | `log.md` Quality Report | orchestrator | 6 dk |
| | **Toplam** | | | | **≈ 49 dk** |

**Zaman bütçesi:** 1–3 ≈ 12 dk · 4–7 ≈ 25 dk · 8–9 ≈ 12 dk. **1 saatten uzun** sürüyorsa bloklar aşırı doldurulmuştur → B6/B7'de metin değil bağlantı yazılır.

### §5.2 Hata Modları

| # | Hata Modu | Belirti | Kök Neden | Düzeltme |
|---|-----------|---------|-----------|----------|
| 1 | Kural kopyası | kök dosyada uzun G paragrafı | SRP unutulmuş | bağlantıya çevir (G12) |
| 2 | Eksik bağlantı | 5 veya daha az `[[…]]` | hedef unutulmuş | §3.2 altı hedefi tamamla |
| 3 | Olmayan dosya iddiası | `.ai/.rules/` "mevcut" | §1.2 ölçülmemiş | `⚠️ VERIFICATION REQUIRED` |
| 4 | Katman kayması | `K8 Host` gibi yanlış kod | başka şablondan taşıma | `architecture-template` §3.2 |
| 5 | Gölge kopyası | `AGENTS.md` 200 satır | iki gerçek kaynağı | 10–20 satıra indir (G5) |
| 6 | Uzunluk sapması | > 260 / < 100 satır | doküman ya da iskelet | §4.3 aralığına çek |
| 7 | Yanlış bütçe | "100K token" | varsayım | 50.000 (§1.4) |
| 8 | Mojibake + ürün adı | bozuk Türkçe · `CoreMusic` | ANSI kaydetme / kopya | UTF-8 BOM'suz · G13 |

### §5.3 Salt Okunur Doğrulama Komutları

Altı komut da **hiçbir dosyayı değiştirmez**; hedef yoksa dürüstçe söyler.

```powershell
# 1) BAĞLANTI — wiki hedeflerinin disk durumu (kırık listelenir)
$base = 'C:\versacoder\.ai\.templates\documentation'
$doc  = Join-Path $base 'claude-md-template.md'
$targets = Select-String -LiteralPath $doc -Pattern '\[\[([^\]]+)\]\]' -AllMatches |
           ForEach-Object { $_.Matches } | ForEach-Object { $_.Groups[1].Value } | Sort-Object -Unique
foreach ($t in $targets) { $p = Join-Path $base ($t.Replace('/','\'))
  if ($p -notlike '*.md') { $p = "$p.md" }; '{0,-8} {1}' -f (Test-Path -LiteralPath $p), $t }

# 2) ÖLÇÜM — Part D'nin 5 şablonu: 480-660 satır · 25-34 KB
$dir = 'C:\versacoder\.ai\.templates\documentation'
foreach ($f in 'claude-md-template','api-doc-template','katman-readme-template','WikiPage-Template','security-audit-template') {
  $p = Join-Path $dir "$f.md"
  if (-not (Test-Path -LiteralPath $p)) { 'EKSIK    ' + $f; continue }
  $n = (Get-Content -LiteralPath $p).Count; $kb = [math]::Round((Get-Item -LiteralPath $p).Length / 1KB, 1)
  $g = ($n -ge 480) -and ($n -le 660) -and ($kb -ge 25) -and ($kb -le 34)
  '{0,-26} {1,4} satir / {2,5} KB  {3}' -f $f, $n, $kb, $(if ($g) { 'GECTI' } else { 'KALDI' }) }

# 3) BÖLÜM — `## §` = 7 ve alt başlık sayısı
'ust bolum: {0} (7 olmali)' -f (Select-String -LiteralPath $doc -Pattern '^## §\d ' -Encoding UTF8).Count
'alt bolum: {0}' -f (Select-String -LiteralPath $doc -Pattern '^### §\d\.\d+ ' -Encoding UTF8).Count

# 4) KUTU HİZASI — ```text bloklarında 66 karakter (0 uyuşmayan)
$box = $false; $satir = Get-Content -LiteralPath $doc -Encoding UTF8; $kotu = 0
for ($i = 0; $i -lt $satir.Count; $i++) {
  $l = $satir[$i]
  if ($l -match '^```text') { $box = $true; continue }
  if ($l -match '^```' -and $box) { $box = $false; continue }
  if ($box -and $l -match '^[+|]' -and $l.Length -ne 66) { '{0}: {1}' -f ($i+1), $l.Trim(); $kotu++ }
}
'hizasiz satir: {0} (0 olmali)' -f $kotu

# 5) FRONTMATTER + YER TUTUCU — 7 alan tam; `{{…}}` yalnız §3.9
$need = 'title','type','category','status','version','updated','authority'
$head = (Get-Content -LiteralPath $doc -TotalCount 12) -join "`n"
$eksik = @($need | Where-Object { $head -notmatch ('(?m)^' + [regex]::Escape($_) + ':') })
$kalip = (Select-String -LiteralPath $doc -Pattern '^### §3\.9' -Encoding UTF8).LineNumber
$sizan = @(Select-String -LiteralPath $doc -Pattern '\{\{[A-Z0-9_]+\}\}' -Encoding UTF8 |
           Where-Object { $_.LineNumber -lt $kalip })
'eksik alan: {0} ({1}) · disari sizan yer tutucu: {2} (0 olmali)' -f $eksik.Count, ($eksik -join ','), $sizan.Count

# 6) KODLAMA — BOM yok, mojibake 0, Türkçe harf var
$bytes = [System.IO.File]::ReadAllBytes($doc)
$bom = ($bytes.Length -ge 3 -and $bytes[0] -eq 0xEF -and $bytes[1] -eq 0xBB -and $bytes[2] -eq 0xBF)
$text = [System.IO.File]::ReadAllText($doc, [System.Text.UTF8Encoding]::new($false))
$moj = ([regex]::Matches($text, "\uFFFD|Ã[\u0080-\u00BF]|Â[\u0080-\u00BF]")).Count
$tur = ([regex]::Matches($text, '[çğıöşüÇĞİÖŞÜ]')).Count
'BOM={0} · mojibake={1} (0 olmali) · Turkce harf={2} (>0)' -f $bom, $moj, $tur
```

**Geçme kriteri:** #1 üretilen kök dosyada kırık = 0 · #2 beşi de `GECTI` · #3 = `7` · #4 = `0` · #5 = `0 eksik · 0 sızan` · #6 `BOM=False · mojibake=0 · Turkce harf>0`.

### §5.4 Quality Report

```markdown
## Quality Report — kök CLAUDE.md (YYYY-MM-DD HH:MM)

| Metrik | Değer | Eşik | Durum |
|--------|-------|------|-------|
| Üretilen `CLAUDE.md` · `AGENTS.md` | n satır / n satır | 120–220 · 10–20 | 🟢/🔴 |
| Bağlantı / kırık | 6 / 0 | tam 6 · kırık 0 | 🟢/🔴 |
| B1..B10 blok · K0..K8 satır | 10/10 · 9/9 | 10 · 9 | 🟢/🔴 |
| Yer tutucu / mojibake / CoreMusic | n/n/n | 0/0/0 | 🟢/🔴 |
| Guardrail G1..G14 | ihlal n | 0 | 🟢/🔴 |
| Şablon ölçümü (§5.3/2) | <satır>/<KB> | 480–660 · 25–34 | 🟢/🔴 |

**Karar:** 🟢 ONAYLANDI (kök dosya yazılır, `log.md`'ye 1 satır) / 🔴 RED (<gerekçe>)
```

---

## §6 Doğrulama

### §6.1 Kontrol Listesi (20 madde)

| # | Kontrol | Beklenen | İlgili |
|---|---------|----------|--------|
| K1 | Frontmatter 7 alan + `authority: SSOT` birebir | 7/7 | §3.2 · §5.3/5 |
| K2 | H1 tek + `**Zorunlu Bağlantılar:**` satırı | 1 satır | §3.2 |
| K3 | Bağlantı sayısı | 6 | §4.3 |
| K4 | Blok haritası B1..B10 | 10/10 | §3.1 |
| K5 | §1.1–§1.4 dolu; §1.2 ≥ 15 kanıt satırı + `⚠️` taşıyan satır | 4/4 · ≥ 15 | §1 |
| K6 | §1.3 gölge `AGENTS.md` kuralı yazılı | var | §1.3 |
| K7 | §1.4 bütçe 50.000 + %3 payı + kutu | var | §1.4 |
| K8 | §2.1 + §2.2 dolu | 2/2 | §2 |
| K9 | §3.1 B1..B10 tablosu | 10 satır | §3.1 |
| K10 | §3.4 K0..K8 = 9 satır, `architecture-template` ile birebir | 9 = 9 | §3.4 |
| K11 | §3.5 G1..G12 özet tablosu | 12 | §3.5 |
| K12 | §3.6 Ne yap / Ne yapma ≥ 10 satır | ≥ 10 | §3.6 |
| K13 | §3.7 dosya yolları + `Var` sütunu | ≥ 10 | §3.7 |
| K14 | §3.9 dolu örnek + tam kalıp (`{{…}}` yalnız kalıpta) | 2 blok | §3.9 |
| K15 | §4.5 G1..G14 | 14 madde | §4.5 |
| K16 | §5.1 9 adım · §5.2 ≥ 8 hata modu · §5.4 rapor | 9 · 8 · var | §5 |
| K17 | §5.3 salt-okunur komut ≥ 5 | 6 | §5.3 |
| K18 | §6.2–§6.5 dolu | 4/4 | §6 |
| K19 | §7.1 kaynak tablosu + §7.2 Red Team ≥ 10 satır | ≥ 10 | §7 |
| K20 | Ölçüm: 480–660 satır · 25–34 KB · `## §` = 7 · mojibake 0 | 4/4 | §5.3/2·3·6 |

### §6.2 Red Sinyalleri (Otomatik DUR)

| # | Sinyal | Aksiyon |
|---|--------|---------|
| R1 | §5.3/1'de kırık bağlantı | **DUR** — hedef üretilir ya da bağlantı silinir |
| R2 | Üretilen kök dosyada `{{…}}` | **DUR** — doldurulur (G4) |
| R3 | K0..K8'de eksik/yanlış kod | **DUR** — `architecture-template` §3.2 |
| R4 | `AGENTS.md` > 20 satır | **DUR** — gölge kuralı (G5) |
| R5 | `EntityFramework` / concat SQL geçen satır | **DUR** — G2 · G10 |
| R6 | `.ai/` içinde ikinci kök kural dosyası | **DUR** — G7 |
| R7 | §1.2'deki ❌ satırın "mevcut" diye anılması | **DUR** — `⚠️ VERIFICATION REQUIRED` |
| R8 | `CoreMusic` / mojibake / BOM | **DUR** — G13 · §5.3/6 |
| R9 | Ölçüm dışı uzunluk | **DUR** — §4.3 |
| R10 | §5.3 çıktısı olmadan "tamam" beyanı | **DUR** — çıktı istenir (G14) |

### §6.3 Sürüm / Değişiklik Geçiş Kuralları

| Değişiklik | `version` | `updated` | Kayıt |
|------------|-----------|-----------|-------|
| Blok ekleme/çıkarma (B11 gibi) | `minor` | evet | `log.md` + §6.5 K1 |
| Yalnız bağlantı/ımle düzeltmesi | `patch` | evet | `log.md` tek satır |
| Katman listesi değişimi | `minor` | evet | önce `architecture-template` |
| Bölüm adı değişimi | `major` | evet | bağlantı veren tüm dosyalar |

### §6.4 İlgili Dosyalar

| Dosya | İlişki |
|-------|--------|
| `.ai/.templates/documentation/claude-md-template.md` | **bu dosya** — kök kural dosyasının tek kaynağı |
| `.ai/.templates/index.md` | envanter (#35) + §3.7 kalite sözleşmesi |
| `.ai/.templates/CLAUDE.md` | `.templates/` içi dizin kuralı — bu şablonun **hedefi değil** |
| `.ai/.templates/architecture/architecture-template.md` | K0..K8'ın tek kaynağı (§3.4) |
| `.ai/.templates/architecture/ascii-diagram-template.md` | kutu biçimi + 66 ölçer (§5.3/4) |

### §6.5 Envantere Özgü Kontroller

| # | Kontrol | Beklenen | Kanıt |
|---|---------|----------|-------|
| K1 | Bu şablonun satır aralığı | 480–660 | §5.3/2 |
| K2 | Bu şablonun KB aralığı | 25–34 | §5.3/2 |
| K3 | Envanter satırı #35 `Durum` | `PLANLI → YAZILDI` (bittikten) | `index.md` §3.5 |
| K4 | Hedef dosya yolu envantere birebir | `CLAUDE.md` / `AGENTS.md` | `index.md` §3.5 #35 |
| K5 | K0..K8 satır sayısı = `architecture-template` §3.2 | 9 = 9 | iki tablo karşılaştırması |
| K6 | Kutu satırları 66 karakter | 0 uyuşmayan | §5.3/4 |

---

## §7 Referanslar

### §7.1 VersaCoder Bağlantıları

| Kaynak | Yol | Ne İçin | Durum (2026-09-25) |
|--------|-----|---------|---------------------|
| Şablon envanteri + kalite sözleşmesi | `.ai/.templates/index.md` | §3.7 · #35 satırı | ✅ 40.440 bayt |
| Biçim altın örneği (kutu, § yapısı) | `.ai/.templates/architecture/ascii-diagram-template.md` | §5.3/4 · kutu 66 | ✅ 38.118 bayt |
| K0..K8 katman listesi | `.ai/.templates/architecture/architecture-template.md` | §3.4 birebir | ✅ okundu |
| `.ai/` kök gerçekleri + kural metni | `Test-Path` çıktısı (§1.2/8–9) | dürüstlük kanıtı | ❌ YOK → `⚠️` |
| Yasaklı vault'lar | `C:\www\coremusic.net\…` · `C:\www\versacoder\…` | yalnız **yapı** referansı | 🔒 kopyalama YASAK |

### §7.2 Red Team — Sık Yapılan Hatalar

| # | Hata | Neden Yanlış | Doğrusu | Yakalama |
|---|------|--------------|---------|----------|
| 1 | Kuralı kök dosyaya kopyalamak | iki gerçek kaynağı, biri çürür (G12) | tek satır özet + bağlantı (§3.5) | gözden geçirme |
| 2 | `AGENTS.md`'yi ayrı yazmak | iki kök güncellenmez, tutarsızlık | gölge 10–20 satır (§1.3) | §5.3/2 + sayım |
| 3 | Olmayan `.ai/.rules/`'u "mevcut" göstermek | kanıtsız iddia, ajan yanlış yere gider | `⚠️ VERIFICATION REQUIRED` (§1.2/9) | §1.2 tablosu |
| 4 | Katman adı/kodu uydurmak | tüm vault K0..K8'e bağlanır, tek sapma her belgeyi çürütür | `architecture-template` §3.2 | §6.1 K10 |
| 5 | 6 bağlantıyı 12'ye çıkarmak | okuma bütçesi şişer, odak kayar | tam 6, `·` ayracılı | §5.3/1 |
| 6 | Dosyayı 400 satıra büyütmek | oturum bütçesi %3 dışına taşar | 120–220 (§4.3) | §5.3/2 |
| 7 | EF Core / concat SQL örneği vermek | ADR-004 + G2/G10 ihlali | ADO.NET + Dapper, `@p1..@pN` | `Select-String` |
| 8 | `CoreMusic` adını geçirmek | yasaklı vault izi (G1) | `VERSACODER` / VersaCoder | `Select-String -Pattern 'coremusic'` |
| 9 | ANSI/CP1254 kaydetmek | bozuk Türkçe, okunmaz | UTF-8 **BOM'suz** | §5.3/6 |
| 10 | `.ai/` içine ikinci kural dosyası | hangisinin geçerli olduğu belirsiz | tek kök `CLAUDE.md` (G7) | dizin taraması |

**SSUT beyanı:** Kök `CLAUDE.md`/`AGENTS.md` üretiminin **tek** kaynağı bu dosyadır; `.ai/.templates/CLAUDE.md` ayrı bir kural dosyasıdır, `.ai/` içine ikinci kök yazılmaz (G7).
