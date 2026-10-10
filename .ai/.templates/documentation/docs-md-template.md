---
title: "VersaCoder — Genel Doküman (docs-md) Şablonu"
type: template
category: template
status: active
version: 1.0.0
updated: 2026-09-25
authority: reference
scope: "genel docs-md sablonu: amac, bolumler ve belge turu alanlari"
tags: [template]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# VersaCoder — Genel Doküman (docs-md) Şablonu

**Zorunlu Bağlantılar:** [[.templates/index]] · [[.templates/index]] · [[./root-md-template]] · [[./readme-template]] · [[.templates/architecture/ascii-diagram-template]] · [[.templates/adr/adr-template]]

---

## §1 Amaç

Bu şablon, `.ai/` altında **içerik ağırlıklı her belgeyi** (`documentation/`, `reports/`, `.rules/`, `spec/`, `learning/`) tek biçimde yazdırır: frontmatter'lı, 7 bölümlü, çapraz bağlı, kanıt etiketli, bozuk linksiz. `T-DOK`'un genel belge ayağıdır; kök belge `[[root-md-template]]`, README `[[readme-template]]`, ADR `[[.templates/adr/adr-template]]` kapsamındadır.

| Alan | Değer |
|------|-------|
| Template Name / Path | `docs-md-template.md` · `.ai/.templates/documentation/docs-md-template.md` |
| Hedef Dosya | `.ai/documentation/*.md` · `.ai/reports/*.md` · `.ai/.rules/*.md` · `.ai/spec/*.md` · `.ai/learning/**.md` |
| Hedef Tür | `spec` \| `report` \| `guide` \| `rules` \| `index` \| `reference` |
| Zorunlu Yapı | 7 bölüm `§1…§7` + `§1.2 Disk Kanıtları` (index.md §3.7) |
| Üretilen Uzunluk | 120–450 satır (belge) · bu şablon 480–660 satır |
| Kodlama | **UTF-8 (BOM'suz)** — Latin-1/CP1254 YASAK |
| Dil | Türkçe (ç ğ ı İ ö ş ü) — dosya yolu, alan adı, komut İngilizce |
| Part · Versiyon · Authority | `C` · `1.0.0` · **SSOT** (bu dosya) |
| Bootstrap | 2026-09-25 |

### §1.1 Neden Genel Belge İçin Ayrı Şablon?

Belge en kolay "boş sayfadan" yazılır: kimse iskeleti bilmez, herkes farklı bölüm açar, kanıt ile tahmin aynı cümleye girer. Şablon yoksa 40+ belge 40 farklı kalitede olur ve `.ai/` aranamaz hâle gelir.

| Boşluk | Belirti | Bu Şablonun Kestiği Şey |
|--------|---------|-------------------------|
| Bölüm adları serbest | bir belgede `## Giriş`, başka birinde `## §1 Amaç` | §3.3 zorunlu 7 bölüm iskeleti |
| Kanıt ile tahmin karışıyor | "çalıştığı varsayılan" cümlesi | §3.5 etiket sistemi (`⚠️` · `NEEDS_VALIDATION`) |
| Bağlantılar kırık | `](../yok.md)` → 404 | §3.4 bağlantı dili + §5.3 komut 3 |
| Frontmatter'sız MD | arama/gezinme kırık | §3.1 8 alan zorunlu (G3) |
| Belge != tek gerçek | aynı kural iki belgede | §2.1 eşleme + SSUT beyanı |
| "bitti" kanıtsız | ölçüm yok, iddia var | §3.5 + §6.1 K-check |

### §1.2 Disk Kanıtları ve Boşluk (YAGNI)

İddia → kanıt. Kanıt yoksa uydurulmaz, `⚠️ VERIFICATION REQUIRED` yazılır. Ölçüm: `C:\versacoder`, 2026-09-25, `Test-Path` + `Get-ChildItem`.

| # | İddia | Durum | Kanıt |
|---|-------|-------|-------|
| 1 | `.ai/documentation/` (bu şablonun ana hedefi) | ❌ YOK | `Test-Path = False` — 0 MD |
| 2 | `C:\versacoder\README.md` (depo kökü) | ❌ YOK | `Test-Path = False` → `[[readme-template]]` ile üretilecek |
| 3 | `src/` · `tests/` · `global.json` · `.sln` | ❌ YOK | kökte yalnız `.ai`, `.git` |
| 4 | `.ai/reports/` = **1 MD** | ✅ DOĞRULANDI | `00-kapsam-analizi.md` = 179 satır / 9,5 KB |
| 5 | `.ai/.rules/` = **6 MD** · 72,9 KB | ✅ DOĞRULANDI | `Get-ChildItem *.md` = coding-standards · data-access · devexpress · security · solid · testing |
| 6 | `.ai/spec/` · `.ai/learning/` | ❌ YOK | `Get-ChildItem` = 0 dosya |
| 7 | `.ai/.templates/documentation/` = **4 MD** · 47,6 KB | ✅ DOĞRULANDI | claude-md (616) · docs-md (bu) · readme (120) · root-md (147) |
| 8 | `.ai/.templates/index.md` §3.7 kalite sözleşmesi | ✅ OKUNDU | 608 satır / 40.440 bayt |
| 9 | `.ai/.templates/CLAUDE.md` dizin kuralı (≥480, 7 bölüm) | ✅ OKUNDU | 132 satır / 7.047 bayt |
| 10 | `.ai/WORKFLOW.md` (gate kaynağı) · `keys.md` · `log.md` | ✅ DOĞRULANDI | 901 / 188 / 606 satır |
| 11 | `.ai/architecture/index.md` · `.ai/adr/` · `.ai/adr/ADR-004` | ❌ YOK | `.ai/architecture/` (4 MD) var; `.ai/adr/` yok → kararlar `.ai/.decisions/` altında |
| 12 | `.ai/ui-design/01…07` tel kafesleri + GATE-003 | ❌ YOK → ⚠️ | klasörde yalnız `README.md` (56 satır) |
| 13 | `dotnet --version` = **10.0.401** | ✅ ÇALIŞTI | `global.json` yok → SDK sürümü makineye bağlı |
| 14 | `.ai/scripts/check-links.md` · `measure-vault.md` | ✅ DOĞRULANDI | 174 · 159 satır — §5.3 komutlarının dayanağı |
| 15 | Harici vault'lar (`C:\www\…`) | 🔒 ERİŞİM YASAK | yalnız **yapı** referansı; tek kelime kopyalanmadı (G1) |

**Yorum yasağı:** Var olmayan dosya için "şurada tanımlı" denmez; `⚠️ VERIFICATION REQUIRED` yazılır, satır olduğu yerde durur. §1.2/1–3, 6, 11–12 satırları bu şablonun **kendi ürettiği** belgelerin henüz var olmadığını, kanıtsız "var" denemeyeceğini hatırlatır.

### §1.3 Bu Şablon Hangi Belge, Hangi Değil?

| Bu Şablon (docs-md) | Değil → Doğru Şablon |
|---------------------|----------------------|
| `.ai/documentation/<konu>.md` genel anlatım | `.ai/` kök omurgası → `[[root-md-template]]` |
| `.ai/reports/*-analizi.md` bulgu/rapor | Karar → `[[.templates/adr/adr-template]]` |
| `.ai/.rules/*.md` kural metni | Depo/katman README → `[[readme-template]]` |
| `.ai/spec/*.md` şartname | Tel kafes → `[[.templates/ui-design/wireframe-template]]` |
| `.ai/learning/**` rehber/öğrenme notu | API referansı → `api-doc-template` (Part D, planlı ⚠️) |
| `type: index` belge indeksi | Wiki sayfası → `WikiPage-Template` (Part D, planlı ⚠️) |

### §1.4 Belge Yaşam Döngüsü

```text
+----------------------------------------------------------------+
| [1] Taslak      iskelet §3.3, frontmatter §3.1                 |
|    v                                                           |
| [2] Kanıt        her iddia §3.5 etiketi alır (⚠️ / ✅ / ❌)      |
|    v                                                           |
| [3] Bağlantı     tüm ](.) hedefleri diskte var (§5.3/3)       |
|    v                                                           |
| [4] Onay         docs-writer -> reviewer (§4.2, ≈ 18 dk)        |
|    v                                                           |
| [5] Yayın        status: draft -> active                       |
|    v                                                           |
| [6] Güncelleme   içerik değişince updated + log.md satırı       |
|    <- geri dönüş: kanıt eskidiyse [2]'e, belge active kalır      |
+----------------------------------------------------------------+
```

**Ne anlatır:** Belgenin taslak→kanıt→onay→yayın döngüsü; eskiyen kanıt `[6]`'dan `[2]`'ye döner, `status: deprecated` yalnız içerik tamamen geçersizse kullanılır.

---

## §2 Kapsam

**İçinde:** genel belge frontmatter'i, 7 bölüm iskeleti, bağlantı dili, kanıt etiketleri, yazım biçimi, ASCII diyagram kullanımı, belgeye özgü doğrulama.
**Dışında:** kök MD iskeleti (`[[root-md-template]]`), README (`[[readme-template]]`), ADR (`[[.templates/adr/adr-template]]`), API doc / Wiki / katman README / güvenlik denetimi (Part D, planlı), tel kafes, kod.

| Kapsam | Kapsam Dışı |
|--------|-------------|
| `.ai/documentation/**` genel anlatım belgesi | `.ai/index.md` kök navigasyon → `root-md` |
| `.ai/reports/**` analiz/teslim raporu | `.ai/.decisions/**` karar → `adr-template` |
| `.ai/.rules/**` kural metni (içerikli) | `.ai/.templates/**` şablon → envanter §3.7 |
| `.ai/spec/**` şartname + `type: index` | `.ai/ui-design/**` tel kafes → `wireframe-template` |
| Kanıt etiketleri + §1.2 disk kanıtları | `.cs` / `.sql` üretimi → backend şablonları |

### §2.1 Dosya Tipi → Şablon Eşlemesi

| # | Üretilecek Dosya | `type` | Şablon | Part |
|---|------------------|--------|--------|------|
| 1 | `.ai/documentation/<konu>.md` (anlatım, rehber) | `spec`/`guide` | **bu dosya** | C |
| 2 | `.ai/reports/<konu>-*.md` (analiz, teslim, kalite) | `report` | **bu dosya** | C |
| 3 | `.ai/.rules/*.md` (kural metni, içerikli) | `rules` | **bu dosya** | C |
| 4 | `.ai/spec/*.md` (şartname) | `spec` | **bu dosya** | C |
| 5 | `.ai/{index,AGENTS,CLAUDE,WORKFLOW,…}.md` kök omurga | `index`/`registry` | `[[root-md-template]]` | C |
| 6 | `README.md` (depo/katman/klasör) | `readme` | `[[readme-template]]` | C |
| 7 | `.ai/adr/ADR-NNN-*.md` mimari/veri/güvenlik kararı | `adr` | `[[.templates/adr/adr-template]]` (+5 varyant) | A |
| 8 | `.ai/documentation/api/*.md` · `wiki/*.md` · güvenlik raporu | `api`/`wiki`/`security` | Part D şablonları ⚠️ PLANLI | D |

### §2.2 Kapsam Sınırı Testi

Üretilecek dosyaya sırayla sor; **ilk `EVET`** şablonu belirler:

1. **`.ai/` kökünde mi, 14 omurga dosyasından biri mi?** → `[[root-md-template]]`.
2. **`README.md` adını mı taşıyor?** → `[[readme-template]]`.
3. **`ADR-` ile mi başlıyor / geri alınabilir karar mı?** → `[[.templates/adr/adr-template]]`.
4. **`.ai/ui-design/` altındaki ekran/tel kafes mi?** → `[[.templates/ui-design/wireframe-template]]`.
5. **`.cs` / `.sql` üretiyor mu?** → backend/infrastructure şablonları.
6. **Yukarıdakilerin hiçbiri değilse ve içerik `.ai/**` içindeyse** → **bu şablon**.

`HAYIR` soruları: CoreMusic/versacoder.ai içeriği mi (G1)? · EF Core geçen satır var mı (G2)? · Frontmatter 11 alan dolu mu (G3)? · `{{…}}` §3.9 bloğu dışında mı (G4)? · `.ai/.rules/` ile çelişen kural mı (SSUT)?

---

## §3 Mimari

Bu bölüm hem **şablonun kendi yapısını** hem **üretilen belgenin yapısını** tanımlar.

### §3.1 Frontmatter (Değişmez 8 Alan)

| # | Alan | Zorunlu | Örnek | Eksikse |
|---|------|---------|-------|---------|
| 1 | `title` | Evet | `"Token Bütçesi — Sınır ve Aşım Raporu"` | RED (G3) |
| 2 | `type` | Evet | `spec` \| `report` \| `guide` \| `rules` \| `index` \| `reference` | RED |
| 3 | `category` | Evet | `documentation` \| `reports` \| `rules` \| `spec` | RED |
| 4 | `status` | Evet | `draft` \| `active` \| `deprecated` | RED |
| 5 | `version` | Evet | `1.0.0` (üç bölüm) | RED |
| 6 | `updated` | Evet | `2026-09-25` (ISO) | RED |
| 7 | `authority` | Evet | `SSOT` — bu belge kendi konusunun tek kaynağı | RED |
| 8 | `part` | Evet | `C` (documentation şablonları) | RED |

**Kurallar:** (a) Alan adı **İngilizce**, değer proje diline göre; (b) `updated` yalnız **içerik** değişince güncellenir, biçimsel düzeltmede bırakılır; (c) `authority: primary` çelişen ikinci bir belge varsa **tekrar olan satır silinir**; (d) Frontmatter'sız MD yasaktır (G3).

### §3.2 Dosya Adı, Yeri ve H1

| Öğe | Kural | Doğru | Yanlış |
|-----|-------|-------|--------|
| Dosya adı | kebab-case, uzantı `.md`, `.`/boşluk yok | `token-butce-asim-raporu.md` | `Token Bütçe.Rapor.md` |
| Yol | `.ai/` altına **kökten** yazılır | `.ai/reports/token-butce-asim-raporu.md` | `~/belgeler/…` |
| H1 | dosyada **tek**, `#` ile | `# Token Bütçesi — Aşım Raporu` | iki `#` başlığı |
| Bağlantı satırı | H1'den hemen sonra tek satır | `**Zorunlu Bağlantılar:** [[…]] · …` | yok / 10 bağlantı |
| İçindekiler | > 8 bölümse tablo | `\| Bölüm \| Alt Konular \|` | yuvarlak liste |

### §3.3 Üretilen Belgenin Zorunlu 7 Bölüm İskeleti

`index.md` §3.7'nin şablonlara koyduğu iskelet, **üretilen belgeye de** aynen uygulanır:

```markdown
---
title: "<Belge Adı> — <Alt Başlık>"
type: <spec|report|guide|rules|index|reference>
category: <documentation|reports|rules|spec>
status: <active|draft|deprecated>
version: <major.minor.patch>
updated: <YYYY-MM-DD>
authority: primary
part: C
---

# <Belge Adı>

**Zorunlu Bağlantılar:** [[.templates/index]] · [[governance/keys]] · [[<ilgili>]]

> **Tek cümlelik sorumluluk:** Bu dosya <X> sorusunun tek cevabıdır.

## §1 Amaç
### §1.1 Kapsam        (İçinde / Dışında — iki satır)
### §1.2 Disk Kanıtları (tablo: iddia · durum · kanıt)
### §1.3 Hedef Kitle   (rol -> bu belgeden ne alır)
### §1.4 Akış          (1 ASCII kutu, 66 sütun)

## §2 Kapsam
### §2.1 Kapsam Dışı   (neyin NEREDE olduğu — tablo)
### §2.2 Sınır Testi   (3–6 soru, ilk EVET belirler)

## §3 Mimari           (ana bilgi — §3.1…§3.N numaralı, 4–10 alt bölüm)

## §4 Kurallar
### §4.1 Zorunlu       (1–N, madde ≤ 5)
### §4.2 Yasak         (her madde "YASAK: …" ile)

## §5 Workflow
### §5.1 Adım          (tablo: # · adım · çıktı · süre)
### §5.2 Hata Modları  (tablo: belirti · kök neden · düzeltme)

## §6 Doğrulama
### §6.1 Checklist     (- [ ] ölçülebilir maddeler)
### §6.2 Red Sinyalleri (tablo: sinyal -> RED)

## §7 Referanslar
### §7.1 Tablo         (# · Kaynak · Yol · Not)
### §7.2 Red Team      (≥ 5 sık hata satırı)
```

**Zorunluluk tablosu (min):**

| Bölüm | Min satır | İçerik sözü |
|-------|-----------|-------------|
| `§1 Amaç` | 40 | + `§1.2 Disk Kanıtları` **her belgede zorunlu** |
| `§2 Kapsam` | 25 | Kapsam Dışı + Sınır Testi |
| `§3 Mimari` | 120 | numaralı alt bölümler, ana bilgi |
| `§4 Kurallar` | 30 | Zorunlu + Yasak |
| `§5 Workflow` | 30 | Adım + Hata Modları |
| `§6 Doğrulama` | 30 | Checklist + Red Sinyalleri |
| `§7 Referanslar` | 20 | Tablo + Red Team |
| **Toplam** | **≥ 295** | G6: `< 480` satır şablonda reddedilir, belgede **≥ 295** yeter |

> **İstisna:** `type: rules` belgelerinde `§5 Workflow` yerine `§5 Uygulama` yazılabilir; ad değişirse `index.md` §3.7'ye not düşülür.

### §3.4 Bağlantı Dili (İç Bağlantı Kuralları)

| # | Kural | Doğru | Yanlış |
|---|-------|-------|--------|
| 1 | Vault içi referans **wiki biçimi** | `[[.templates/index]]` · `[[.templates/.rules/coding-standards]]` | `[[.templates/yok|index]]` |
| 2 | Yol **dosyadan göreli**, `..` ile yüksel | `.ai/reports/x.md` → `[[.templates/index]]` | `[x](../../../….md)` |
| 3 | Bağlantı satırı **tek**, H1'den sonra | 4–6 bağlantı | her paragraf bağlantı |
| 4 | Hedef **diskte yoksa** bağlantı yazılmaz | yalnız ad + `⚠️` | kırık `](…)` |
| 5 | Kod içi yol **code span** | `` `.ai/.rules/security-rules.md` `` | bağlantı gibi yazmak |
| 6 | Bağlantı **açıklamalı** | `[[governance/keys]]` (dosya zaten biliniyor) | `[[.templates/documentation/…|buraya tıklayın]]` |

### §3.5 Kanıt ve Etiket Sistemi

Her iddia bir etiket taşır; etiketsiz iddia **RED**'dir (G5 benzeri).

| Etiket | Ne Zaman | Zorunlu Karşılık |
|--------|----------|------------------|
| ✅ `DOĞRULANDI` | dosya diskte ölçüldü | ölçüyolu + sayı (`179 satır / 9,5 KB`) |
| ❌ `YOK` | `Test-Path = False` | üretim planı (hangi şablon) |
| ⚠️ `VERIFICATION REQUIRED` | iddia var, kanıt henüz yok | satır **sonuna** yazılır, öne çekilmez |
| `NEEDS_VALIDATION` | doğrulanması gereken teknik varsayım | sahibi + ne zaman doğrulanacak |
| `UNKNOWN` | bilgi elimizde değil | uydurma **yasak** — "bilinmiyor" yazılır |
| `INFERRED` | çıkarım | açıkça "çıkarım:" diye başlar |
| 🔒 `ERİŞİM YASAK` | kopyalamama kapsamı (G1) | yalnız yapı referansı |

**Cümle kalıbı (zorunlu):** *"İddia → kanıt. Ölçüm: `C:\versacoder`, `<tarih>`."* — "`muhtemelen`, `genellikle`, `belki de`" yasaktır; yerine etiket konur.

### §3.6 Yazım Biçimi Kuralları

| # | Kural | Neden |
|---|-------|-------|
| 1 | **Önce cevap, sonra madde** — ilk paragraf tek cümlelik sorumluluktur | Okur cevabı aramasın |
| 2 | Her bölüm `§` numarası taşır; çapraz referans `§3.5` biçiminde | Link kesin olsun |
| 3 | 2+ karşılaştırma varsa **tablo > madde** | Taranabilirlik |
| 4 | Madde sayısı **≤ 5**; fazlası tablo veya alt bölüm | Bilişsel yük |
| 5 | Tahmin **somut** olur: "≈18 dk", "47,6 KB", "6 MD" — "biraz/çok" yasak | Ölçülebilirlik |
| 6 | Sayı her zaman **ölçümle** gelir; ölçülmeyen sayı `⚠️` alır | Kanıt disiplini |
| 7 | Türkçe yazımda `ç ğ ı İ ö ş ü` doğru; `?`, `\xc3\xbc` yasak (G5) | Mojibake |
| 8 | Emoji yalnız **durum hücrelerinde** (`✅ ❌ ⚠️ 🔒`) | Başlıkta emoji = RED |

### §3.7 ASCII Diyagram Kuralı

Belge bir **akış / hiyerarşi / döngü** anlatıyorsa `[[.templates/architecture/ascii-diagram-template]]` kuralıyla tek kutu çizilir:

| Kural | Değer | Denetim |
|-------|-------|---------|
| Karakter seti | `+ + + + | - + +` + `→ v` (saf ASCII değil) | §5.3/5 |
| Genişlik | **66 karakter** (`| ` + 63 gövde + `|`) | §5.3/5 |
| Dil | kutu içi Türkçe serbest | — |
| Adet | bölüm başına **≤ 1** kutu; 2 kutu = tabloya çevir | gözden geçirme |
| Blok | ` ```text ` ile | ` ``` ` (dil yok) = düz metin sanılır |

### §3.8 Dolu VersaCoder Örneği (kısa, gerçek veriyle)

```markdown
## §6 Doğrulama

### §6.1 Checklist

- [ ] `.ai/.rules/` = 6 MD · 72,9 KB (ölçüm 2026-09-25)
- [ ] `.ai/documentation/` üretimden önce 0 MD — `⚠️` ile işaretli
- [ ] `dotnet --version` = 10.0.401 (kanıt: §5.3/1 çıktısı)

### §6.2 Red Sinyalleri

| # | Sinyal | Aksiyon |
|---|--------|---------|
| R1 | `](…)` hedefi diskte yok | **RED** — §5.3/3 |
| R2 | `⚠️` satırı "üretilmiş gibi" yazılmış | **RED** — §1.2/15 |
```

**Ne anlatır:** Ölçü + etiket + kısaltma üçlüsü birlikte kullanıldığında belge nasıl görünür.

### §3.9 Tam Kalıp (yer tutuculu belge iskeleti)

> `{{…}}` yer tutucuları **yalnız bu blokta** yaşar; üretilen belgede adedi **0** olmalıdır (G4).

````markdown
```markdown
---
title: "{{BELGE_ADI}} — {{ALT_BASLIK}}"
type: {{spec|report|guide|rules}}
category: {{documentation|reports|rules|spec}}
status: draft
version: 0.1.0
updated: {{YYYY-MM-DD}}
authority: primary
part: C
---

# {{BELGE_ADI}}

**Zorunlu Bağlantılar:** [[.templates/index]] · [[governance/keys]] · {{ILGILI_BAGLANTI}}

> **Tek cümlelik sorumluluk:** Bu dosya {{TEK_CUMLE}}.

## §1 Amaç
### §1.1 Kapsam
**İçinde:** … **Dışında:** …
### §1.2 Disk Kanıtları
| # | İddia | Durum | Kanıt |  <- her belgede ZORUNLU
### §1.3 Hedef Kitle
| Rol | Bu belgeden ne alır |
### §1.4 Akış
```text
{{66 KARAKTERLIK ASCII KUTU}}
```

## §2 Kapsam / §3 Mimari / §4 Kurallar / §5 Workflow / §6 Doğrulama / §7 Referanslar
{{BOLUMLERI DOLDUR}}
```
````

---

## §4 Kurallar

### §4.1 Yazım Sırası (9 Adım)

| # | Adım | Çıktı | Hata |
|---|------|-------|------|
| 1 | Şablon var mı kontrolü (envantere bak) | doğru şablon satırı | şablon yoksa üretme, `PLANLI` aç |
| 2 | `§2.2` sınır testini koş | docs-md = evet | yanlış şablon = yeniden yazım |
| 3 | Frontmatter 11 alan | `§3.1` | G3 RED |
| 4 | H1 + Zorunlu Bağlantılar + tek cümlelik sorumluluk | üst blok | — |
| 5 | `§1.2` disk kanıtlarını **ölçerek** doldur | kanıt tablosu | tahmin = ⚠️ |
| 6 | `§3.3` iskeletiyle bölümleri aç | 7 bölüm | tek `## Giriş` = RED |
| 7 | İçeriği yaz (§3.6 biçim kuralları) | `§3` | madde > 5 = tabloya böl |
| 8 | `§5.3` 6 komutunu çalıştır | 0 kırık link / 0 placeholder | G4, G7 RED |
| 9 | `§6.1` checklist + Quality Report | onay | kanıtsız "bitti" (G5) |

### §4.2 Sorumluluk ve Onay

| Aşama | Rol | Ne Onaylar | Çıktı | Süre |
|-------|-----|-----------|-------|------|
| 1 | docs-writer | taslak + frontmatter + §1.2 kanıtları | belge + ölçümler | 25 dk |
| 2 | reviewer | §6.1 checklist (21 madde) | ✅ / RED | 8 dk |
| 3 | architect | `.rules/` ile çelişki, SSUT, katman adı | çelişki yok | 5 dk |
| 4 | master-orchestrator | §5.4 Quality Report | kayıt | 5 dk |
| | | | **Toplam** | **≈ 43 dk** |

> **3 RED** sonrası iş durur; gerekçe `master-orchestrator`'a yükseltilir. Red → düzelt → aşama 2'ye dön.

### §4.3 Ölçek / Standartlar

| Öğe | Değer | Sapma |
|-----|-------|-------|
| Üretilen belge satırı | 120–450 | < 80 iskelet · > 600 ikiye böl |
| Bölüm sayısı | 7 (`§1…§7`) | 6 altı / 9 üstü = RED |
| Bölüm başına madde | ≤ 5 | fazlası tablo |
| Bağlantı satırı | 4–6 bağlantı | 0 eksik · 10+ dağınık |
| ASCII kutu | bölüm ≤ 1, 66 sütun | 2 kutu = tablo |
| Kodlama | UTF-8 BOM'suz | Latin-1/CP1254 = RED |

### §4.4 Değişiklik Kontrolü

| Olay | Aksiyon | Kayıt |
|------|---------|-------|
| Yazım hatası / typo | yerinde düzelt | `updated` **değişmez** |
| Yeni veri/ölçüm eklendi | `§1.2` + ilgili bölüm | `updated` + `log.md` satırı, `version` patch |
| Yeni `§x.y` alt bölüm | `§6.1` checklist'i yenile | `version` minor + Quality Report |
| Kural değişti (`.rules/`) | önce `.rules/`, sonra bu belge | `version` minor, SSUT |
| Belge geçersizleşti | `status: deprecated` + üstüne "→ yeni belge" | `version` major |

### §4.5 Hard Guardrails (Otomatik DUR)

| # | Guardrail | İhlal Sonucu |
|---|-----------|--------------|
| G1 | CoreMusic / versacoder.ai içeriğinden cümle | **RED** — sil + özgün yaz (envanter G1) |
| G2 | Belgede EF Core / string-concat SQL önerisi | **RED** — ADO.NET + Dapper'e çevir |
| G3 | Frontmatter eksik (`< 8` alan) | **RED** — §3.1 |
| G4 | `{{…}}` §3.9 bloğu dışında | **RED** — temizle, yeniden ölç |
| G5 | Kanıtsız "bitti" / etiketsiz iddia | **RED** — §3.5 etiketi |
| G6 | `< 480` satır (şablon dosyası) | **RED** — bu dosya için 480–660 |
| G7 | Bozuk `](…)` bağlantısı | **RED** — §5.3/3 |
| G8 | `GATE-003` beklentisi varken ekran/kod iddiası | **DUR** — `.ai/ui-design/`'e dön |
| G9 | `.ai/adr/` dışında karar cümlesi ("karar: X") | **RED** — karar ADR'ye taşınır |
| G10 | SQL'de string concat / `SELECT *` telkini | **RED** — `@pN` + indeks notu |
| G11 | `TreatWarningsAsErrors` / %90 coverage iddiası kanıtsız | **RED** — test çıktısı ekle |
| G12 | Bu şablonun içeriği ikinci belgede tekrar | **RED** — tek kaynak, diğeri link |

---

## §5 Workflow

### §5.1 Adım Bazlı Akış

| # | Adım | Girdi | Çıktı | Yapan | Süre |
|---|------|-------|-------|-------|------|
| 1 | Ön koşul | envanter satırı (`PLANLI`/`YAZILDI`) | şablon seçimi | docs-writer | 2 dk |
| 2 | Sınır testi | `§2.2` 6 soru | docs-md = evet | docs-writer | 2 dk |
| 3 | Ölçüm | disk (`Test-Path`, `Get-ChildItem`) | `§1.2` tablosu | docs-writer | 6 dk |
| 4 | İskelet | `§3.3` | 7 bölüm + frontmatter | docs-writer | 8 dk |
| 5 | İçerik | konu + `.rules/` + `keys.md` | `§3` ana bilgi | docs-writer | 25 dk |
| 6 | Biçim | `§3.6` + `§3.7` | tablolar + ≤ 1 kutu | docs-writer | 7 dk |
| 7 | Doğrulama | `§5.3` 6 komut | 0 / 0 / 0 çıktı | docs-writer | 5 dk |
| 8 | Onay | `§6.1` 21 madde | ✅ / RED | reviewer | 8 dk |
| 9 | Kayıt | `§5.4` Quality Report | `log.md` satırı | orchestrator | 5 dk |
| | **Toplam/belge** | | | | **≈ 68 dk** |

**Zaman bütçesi:** 1–4 ≈ 18 dk · 5–6 ≈ 32 dk · 7–9 ≈ 18 dk. Bir belge **2 saatten uzun** sürüyorsa `§2.2`'ye dön: kapsam fazlası (kod/ADR/tel kafes) sızmıştır.

### §5.2 Hata Modları

| # | Hata Modu | Belirti | Kök Neden | Düzeltme |
|---|-----------|---------|-----------|----------|
| 1 | İskeletsiz belge | 10 farklı `##` başlığı | `§3.3` atlanmış | 7 bölümü yeniden aç |
| 2 | Kanıtsız iddia | "genellikle çalışır" | `§3.5` etiketi yok | ölç ya da `⚠️` koy |
| 3 | Kırık bağlantı | `](../yok.md)` | hedef üretilmedi | bağlantıyı sil, ad + `⚠️` bırak |
| 4 | Placeholder sızıntısı | üretilmiş belgede `{{…}}` | `§3.9` bloğu kopyalanmış | temizle (G4) |
| 5 | Çifte gerçek | belge `.rules/` tekrarı | SSUT ihlali | `.rules/` tek kaynak, belge link |
| 6 | Aşırı uzunluk | 900 satır tek belge | kapsam fazlası | `§2.1`'e göre ikiye böl |
| 7 | Mojibake | `?`, `\xc3\xbc` | Latin-1 kaydetme | UTF-8 BOM'suz yeniden kaydet |
| 8 | Yanlış şablon | ADR metni docs-md'de | `§2.2` koşulmadı | `adr-template`'e taşı |

### §5.3 Salt Okunur Doğrulama Komutları

Altı komut da **hiçbir dosyayı değiştirmez**; hedef yoksa dürüstçe `DIZIN YOK` yazar.

```powershell
# 1) Frontmatter 11 alan + tek H1 (eksik = 0 olmali)
$md = 'C:\versacoder\.ai\documentation\ornek.md'
if (-not (Test-Path $md)) { 'DOSYA YOK -> once uret' } else {
  $t = Get-Content $md -Raw
  @('title','type','category','status','version','updated','authority','part') |
    Where-Object { $t -notmatch "(?m)^$_\s*:" } | ForEach-Object { "EKSIK ALAN -> $_" }
  if ((Select-String -Path $md -Pattern '^# ' -AllMatches).Count -ne 1) { 'H1 TEKIL DEGIL' } }

# 2) 7 bolum zorunlu (§1..§7) — eksik bolum 0 olmali
$bols = 1..7 | ForEach-Object { if ($t -notmatch "(?m)^## §$_ ") { "EKSIK BOLUM §$_" } }
$bols; if (-not $bols) { '7/7 BOLUM TAM' }

# 3) Kirik baglanti = 0 (G7) — wiki baglantilari ayrica kontrol edilir
$base = Split-Path $md
Select-String -Path $md -Pattern '\]\((\.\.?\/[^)#]+)\)' -AllMatches | ForEach-Object { $_.Matches } |
  ForEach-Object { $_.Groups[1].Value } | Sort-Object -Unique |
  ForEach-Object { if (-not (Test-Path (Join-Path $base $_))) { "KIRIK -> $_" } }

# 4) Satir/KB + placeholder (G4) + durum etiketleri
"{0} satir / {1} KB" -f (Get-Content $md).Count, [math]::Round((Get-Item $md).Length/1KB,1)
(Select-String -Path $md -Pattern '\{\{[A-Z0-9_]+\}\}').Count          # hedef: yalniz §3.9'da
(Select-String -Path $md -Pattern 'VERIFICATION REQUIRED').Count        # hedef: kanitli satir

# 5) ASCII kutu 66 sütun — uymayan satir 0
$t = Get-Content $md -Raw
[regex]::Matches($t,'(?ms)^```text\r?\n(.*?)```') | ForEach-Object {
  $_.Groups[1].Value -split "`r?`n" | Where-Object { $_.Length -gt 0 -and $_.Length -ne 66 } } |
  ForEach-Object { '66 DISI -> ' + $_.Length + ' : ' + $_ }

# 6) Kodlama (BOM + mojibake) + yasak teknoloji = 0
$b = [System.IO.File]::ReadAllBytes($md)
if ($b.Length -ge 3 -and $b[0] -eq 0xEF -and $b[1] -eq 0xBB) { 'BOM VAR' }
if ($t -match '\xC3[\x80-\xBF]\xC3[\x80-\xBF]|\uFFFD') { 'MOJIBAKE' }
if ($t -match 'EntityFramework') { 'EF CORE GECIYOR (G2)' }
```

**Geçme kriteri:** #1–#6 hepsi **0 satır uyarı** (dosya yoksa dürüst `DOSYA YOK`); `7/7 BOLUM TAM` yazısı görünür.

### §5.4 Quality Report

```markdown
## Quality Report — docs-md (YYYY-MM-DD HH:MM)

| Metrik | Değer | Eşik | Durum |
|---|---|---|---|
| Ölçüm (şablon) | <satır> satır / <KB> KB | 480–660 · 25–34 | 🟢/🔴 |
| Checklist §6.1 | n/21 | 21/21 | 🟢/🔴 |
| Komut §5.3 (#1–#6) | n satır uyarı | 0 | 🟢/🔴 |
| 7 bölüm + §1.2 kanıt tablosu | 7/7 · var | 7/7 | 🟢/🔴 |
| Yer tutucu / mojibake / kırık link | n/n/n | 0/0/0 | 🟢/🔴 |
| Guardrail G1..G12 | ihlal n | 0 | 🟢/🔴 |

**Karar:** 🟢 ONAYLANDI / 🔴 RED (<gerekçe>)
```

---

## §6 Doğrulama

### §6.1 Kontrol Listesi (21 madde)

| # | Kontrol | Beklenen | İlgili |
|---|---------|----------|--------|
| K1 | Frontmatter 11 alan + `authority: primary` + `part: C` | 8/8 | §3.1 |
| K2 | H1 tek + `**Zorunlu Bağlantılar:**` (4–6) | 1 satır | §3.2 |
| K3 | `§1.1–§1.4` dolu | 4/4 | §1 |
| K4 | `§1.2` disk kanıtları: iddia · durum · kanıt + yorum yasağı | var | §1.2 |
| K5 | `§2.1` eşleme + `§2.2` sınır testi (≥ 5 soru) | 2/2 | §2 |
| K6 | `§3.1–§3.9` dolu, `§3.9` = tek `{{…}}` yeri | 9/9 | §3 |
| K7 | Üretilen belge iskeleti 7 bölüm + min satır tablosu | var | §3.3 |
| K8 | Bağlantı dili 6 kuralı yazılı | 6 | §3.4 |
| K9 | Etiket sistemi 7 satır (`✅ ❌ ⚠️ NEEDS VALIDATION UNKNOWN INFERRED 🔒`) | 7 | §3.5 |
| K10 | Yazım biçimi ≥ 8 kural (madde ≤ 5, tablo > madde) | 8 | §3.6 |
| K11 | ASCII kutu kuralı 66 sütun + bölüm ≤ 1 | var | §3.7 |
| K12 | Dolu örnek + tam kalıp mevcut | 2/2 | §3.8–§3.9 |
| K13 | `§4.1–§4.5` dolu, 9 adım | 5/5 | §4 |
| K14 | Hard Guardrails = 12 madde (G1..G12) | 12 | §4.5 |
| K15 | `§5.1` adım tablosu + zaman bütçesi | 9 adım + toplam | §5.1 |
| K16 | `§5.2` ≥ 8 hata modu | 8 | §5.2 |
| K17 | `§5.3` ≥ 5 salt-okunur komut | 6 | §5.3 |
| K18 | `§5.4` Quality Report bloğu | var | §5.4 |
| K19 | `§6.1` = 21 madde | 21 | bu tablo |
| K20 | `§6.2–§6.5` dolu | 4/4 | §6 |
| K21 | `§7.2` ≥ 10 satır · yer tutucu/mojibake | ≥ 10 · 0/0 | §7 · G4, G5 |

### §6.2 Red Sinyalleri (Otomatik DUR)

| # | Sinyal | Aksiyon |
|---|--------|---------|
| R1 | `](…)` hedefi diskte yok | **RED** — bağlantıyı sil (G7) |
| R2 | `§1.2` olmadan "kanıt" cümlesi | **RED** — etiket koy ya da ölç |
| R3 | `{{…}}` §3.9 dışında | **RED** — temizle (G4) |
| R4 | `EntityFramework` geçen satır | **RED** — ADO.NET + Dapper (G2) |
| R5 | `.ai/adr/` dışında "karar:" cümlesi | **RED** — ADR'ye taşı (G9) |
| R6 | Başlıkta H1'den fazlası | **RED** — tek H1 (§3.2) |
| R7 | `⚠️` satırı "üretilmiş" gibi anlatılmış | **RED** — dürüst etikete dön |
| R8 | Mojibake / BOM / emoji başlık | **RED** — UTF-8 BOM'suz (G5) |

### §6.3 Tür / Durum Geçiş Kuralları

| Değişiklik | Etki |
|-----------|------|
| Yalnız typo/dilbilgisi | `updated` değişmez, `version` sabit |
| Yeni ölçüm/kanıt | `updated` + `version` patch + `log.md` |
| Yeni bölüm `§x.y` | `version` minor + `§6.1` yeniden |
| Kural `.rules/`'ta değişti | önce `.rules/`, sonra bu belge; `version` minor |
| İçerik tamamen geçersiz | `status: deprecated` + yönlendirme, `version` major |
| Şablonun kendisi (bu dosya) | §6.5 K1/K2 ölçümü + Quality Report |

### §6.4 İlgili Dosyalar

| Dosya | İlişki |
|-------|--------|
| `.ai/.templates/documentation/docs-md-template.md` | **bu dosya** — genel belgenin tek kaynağı |
| `.ai/.templates/documentation/root-md-template.md` | kök omurga belgeleri (ayrı şablon) |
| `.ai/.templates/documentation/readme-template.md` | README (ayrı şablon) |
| `.ai/.templates/architecture/ascii-diagram-template.md` | `§3.7` kutu biçimi + 66 sütun |
| `.ai/.templates/adr/adr-template.md` | karar belgesi (docs-md değil) |
| `.ai/.templates/index.md` | envanter + §3.7 kalite sözleşmesi |

### §6.5 Envantere Özgü Kontroller

| # | Kontrol | Beklenen | Kanıt |
|---|---------|----------|-------|
| K1 | Bu şablonun KB aralığı (envanter satır 222: `11,0` tahmin) | 25–34 | `(Get-Item …).Length/1KB` |
| K2 | Bu şablonun satır aralığı | 480–660 | `(Get-Content …).Count` |
| K3 | `documentation/` = 4–9 MD (Part C + D üretimi) | ≥ 4 | `Get-ChildItem .templates\documentation` |
| K4 | Yer tutucu yalnız `§3.9` bloğu | 0 (diğer yer) | `Select-String '\{\{[A-Z0-9_]+\}\}'` |
| K5 | `.ai/documentation/` hedef dizin durumu | 0 MD → üretimde | `Test-Path` + sayım |

---

## §7 Referanslar

### §7.1 VersaCoder Bağlantıları

| Kaynak | Yol | Ne İçin | Durum (2026-09-25) |
|--------|-----|---------|---------------------|
| Şablon envanteri + §3.7 sözleşme | `.ai/.templates/index.md` | 7 bölüm + 8 alan kuralı | ✅ 608 satır / 39,5 KB |
| Dizin kuralı (≥480) | `.ai/.templates/CLAUDE.md` | asgari uzunluk + yasaklar | ✅ 132 satır / 6,9 KB |
| Kök belge şablonu | `.ai/.templates/documentation/root-md-template.md` | kök omurga ayrımı | ✅ 147 satır |
| README şablonu | `.ai/.templates/documentation/readme-template.md` | `§2.1` #6 ayrımı | ✅ ölçüldü |
| ASCII kutu biçimi | `.ai/.templates/architecture/ascii-diagram-template.md` | `§3.7` | ✅ 644 satır / 37,2 KB |
| Kurallar (6 MD) | `.ai/.rules/*.md` | `§4.5` G-madde dayanağı | ✅ 72,9 KB |
| Bağlantı denetimi | `.ai/scripts/check-links.md` · `measure-vault.md` | `§5.3` | ✅ 174 · 159 satır |
| Harici vault'lar | `C:\www\…` (envanter G1) | yalnız **yapı** referansı | 🔒 kopyalama YASAK |
| `.ai/documentation/` hedefi | — | üretim bekliyor | ❌ YOK → `⚠️` |

### §7.2 Red Team — Sık Yapılan Hatalar

| # | Hata | Neden Yanlış | Doğrusu |
|---|------|--------------|---------|
| 1 | `§1.2` olmadan "diskte mevcut" demek | kanıtsız iddia (G5) | ölç + `✅/❌/⚠️` etiketi |
| 2 | Belgeyi `## Giriş`/`## Özet` ile açmak | iskelet dağılır, arama kırılır | `§1 Amaç` + tek cümlelik sorumluluk |
| 3 | `.rules/` kuralını belgeye kopyalamak | iki gerçek kaynağı (SSUT) | `.rules/` tek kaynak, belge link verir |
| 4 | `](../yok.md)` bırakmak | G7 ihlali, tarama kırmızı | hedef yoksa ad + `⚠️` yaz |
| 5 | `{{…}}` ile yayınlamak | G4 ihlali, okur yer tutucu görür | yalnız `§3.9` bloğu, üretimde 0 |
| 6 | "belki de ~30 dakika" gibi belirsiz tahmin | ölçülemez | `≈30 dk` · `47,6 KB` · `6 MD` |
| 7 | ADR kararını genel belgeye yazmak | G9 — karar ikiye bölünür | `.decisions/` altına ADR, belge link |
| 8 | EF Core / `SELECT *` önermek | G2/G10 — yığın sapması | ADO.NET + Dapper + `@pN` |
| 9 | Belgeyi 900 satırda tek dosyada tutmak | okunamaz | `§2.1` eşlemesine göre ikiye böl |
| 10 | `status: active` ile kanıtsız yayımlamak | G5 | `draft` → `§6.1` 21/21 → `active` |

**SSUT beyanı:** Bu dosya `.ai/**` genel belgelerinin **tek** üretim kaynağıdır; her belge bu şablon + `.rules/` üzerinden üretilir. Çoğaltılan her satır ikinci gerçek kaynağı doğurur (envanter G12).
