---
title: "VersaCoder — Katman README Şablonu"
type: template
category: template
status: active
version: 1.0.0
updated: 2026-09-25
authority: reference
scope: "katman README sablonu: sorumluluk, icindekiler, yapi ve dogrulama satirlari"
tags: [template]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# VersaCoder — Katman README Şablonu

**Zorunlu Bağlantılar:** [[../index]] · [[../CLAUDE]] · [[../architecture/architecture-template]] · [[../architecture/layer-spec-template]] · [[../adr/adr-template]] · [[api-doc-template]]

---

## §1 Amaç

`C:\versacoder\src\VersaCoder.<Katman>\README.md` dosyasının **tek üretim kaynağıdır**. Katman README'si kodun özeti değil, katmanın **sözleşmesidir**: bu katman neye yarar, hangi katmana bakar, hangi katmandan bakılamaz, hangi tipi dışarıya açar.

SRP gereği bu şablon **kural metni üretmez**: kural `.ai/.rules/`, katman tanımı `architecture-template`, derin spec `layer-spec-template`, karar `adr-template` şeklindedir — README yalnız **tek katmanın yüzeyini** anlatır.

| Alan | Değer |
|------|-------|
| Template Name | `katman-readme-template.md` |
| Template Path | `.ai/.templates/documentation/katman-readme-template.md` |
| Hedef Dosya | `src/VersaCoder.<Katman>/README.md` |
| Hedef Dizin | `src/` — 9 katman = 9 README (K0..K8) |
| Üretilen Uzunluk | `100–260 satır` · `7–15 KB` (katman başına) |
| Kodlama | **UTF-8 (BOM'suz)** — Latin-1/CP1254 YASAK |
| Dil | Türkçe — proje adı, tip adı, dosya adı İngilizce |
| Zorunlu Blok | 9 blok `R1..R9` (§3.1) |
| Bağlam Bütçesi | 50.000 token/oturum — README payı ≤ %2 (§1.4) |
| Versiyon · Authority | `1.0.0` · `SSOT` (bu dosya) |
| Bootstrap | 2026-09-25 |

### §1.1 Okuma Sırası — Katman Nereden Tanımlanır

README beş kaynaktan beslenir; **katman adı ve kodu uydurulmaz**.

| Sıra | Okunan | Ne verir | Tahmin Olur mu |
|------|--------|----------|----------------|
| 1 | `../architecture/architecture-template` §3.2 | K0..K8 kodu + sorumluluk tek satırı | Hayır — SSOT |
| 2 | `src/**` proje yapısı (`.csproj`, klasörler) | gerçek dosya ağacı | Hayır |
| 3 | `../architecture/layer-spec-template` | katmanın derin spec'i | Hayır |
| 4 | `../.rules/*.md` (6 MD) | o katmanın kaçınılmaz kuralı | Hayır |
| 5 | `README.md` (hedef) | bu şablonun çıktısı | — |

**Boşluk riski:** `src/` diskte YOK (§1.2/4). Bu durumda README **uydurulmaz**: dosya ağacı ve tip listesi `⚠️ VERIFICATION REQUIRED` ile işaretlenir, README `DRAFT` kalır ve katman kodu yalnız `architecture-template`'ten alınır.

### §1.2 Disk Kanıtları ve Boşluk (YAGNI)

İddia → kanıt. Ölçüm: `C:\versacoder`, 2026-09-25, `Test-Path` + `Get-ChildItem`.

| # | İddia | Durum | Kanıt |
|---|-------|-------|-------|
| 1 | `src/` (9 VersaCoder projesi) | ❌ YOK | `Test-Path = False` → hedef dizin üretilir |
| 2 | `src/VersaCoder.Domain/README.md` | ❌ YOK | `Test-Path = False` → bu şablonla |
| 3 | `tests/` + `.sln` + `global.json` | ❌ YOK | `Test-Path = False` |
| 4 | `.slnf` / `Directory.Packages.props` (CPM) | ❌ YOK | kökte yalnız `.ai`, `.git` |
| 5 | `.ai/.templates/architecture/architecture-template.md` | ✅ DOĞRULANDI | 34.059 bayt — K0..K8 tek kaynağı |
| 6 | `.ai/.templates/architecture/layer-spec-template.md` | ✅ DOĞRULANDI | 34.783 bayt — derin spec |
| 7 | `.ai/.templates/architecture/ascii-diagram-template.md` | ✅ DOĞRULANDI | 38.118 bayt — kutu biçimi |
| 8 | `.ai/.templates/index.md` (envanter) | ✅ DOĞRULANDI | 40.440 bayt |
| 9 | `.ai/.rules/` (6 MD) | ✅ DOĞRULANDI | `solid` · `coding-standards` · `data-access` … |
| 10 | `.ai/architecture/index.md` | ✅ DOĞRULANDI | katman dizini kaydı |
| 11 | `.ai/.decisions/adr-index.md` | ✅ DOĞRULANDI | karar defteri |
| 12 | `C:\versacoder\CLAUDE.md` (kök) | ❌ YOK | `Test-Path = False` |
| 13 | `.ai/documentation/` (README'lerin atası) | ❌ YOK | `Test-Path = False` |
| 14 | Kopyalama yasağı kapsamındaki harici vault'lar | 🔒 ERİŞİM YASAK | yalnız **yapı** referansı (G1) |

**Yorum yasağı:** Olmayan `src/` dosyası için "şurada tanımlı" denmez; `⚠️ VERIFICATION REQUIRED` yazılır. §1.2/1–4 satırları **üretilen** README'de de görünür: bağlantı verilir, varlık iddia edilmez.

### §1.3 README ↔ layer-spec ↔ ADR Ayrımı

Aynı bilgi üç yere yazılırsa üçü de bayatlar; her şeyin **tek evi** vardır:

| Bilgi | Tek Ev | README'de | Tekrar Edilmez |
|-------|--------|-----------|----------------|
| Katman kodu + sorumluluk (K0..K8) | `architecture-template` §3.2 | tek satır özet | tam tablo kopyası |
| Sınıf/interior tasarım | `layer-spec-template` | — | derin spec'e bağlantı |
| Bağımlılık yönü kuralı | `architecture-template` | **ok** (README'nin özü) | kural metni |
| Mimari karar + gerekçe | `adr/ADR-NNN` | tek satır + bağlantı | karar metni |
| Guardrail (G-madde) | `.ai/.rules/*.md` | o katmana değen 2–4 madde özeti | kural paragrafı |
| Kod ağacı | `src/**` | dosya/liste (kanıtlı) | kod içeriği |

### §1.4 Bağlam Bütçesi ve Uzunluk

| Öğe | Değer | Aşım Belirtisi |
|-----|-------|----------------|
| Oturum bütçesi | 50.000 token | — |
| README payı | ≤ 1.000 token (%2) | > 1.500 → §4.3 RED |
| Üretilen satır | 100–260 | < 80 iskelet · > 300 RED |
| Dosya ağacı satırı | 8–30 | > 40 → katman bölünmeli (mimari sorun) |
| Tip listesi | 5–25 | > 30 → README değil, spek |
| Güncelleme tetiği | katman sorumluluğu / bağımlılık değişimi | `log.md` + `version` |

```text
+----------------------------------------------------------------+
| [1] katman README oku     R1..R9 zorunlu blok sirasi           |
|    v                                                           |
| [2] architecture-template  K kodu + sorumluluk + yon (SSOT)     |
|    v                                                           |
| [3] src/ agaci olc        dosya + tip listesi (YOK ise ⚠️)     |
|    v                                                           |
| [4] bagimlilik oku  <== KAPI: yone aykiri README RED            |
|    v                                                           |
| [5] olcum + rapor          100-260 satir · 7-15 KB · 0 yersiz  |
+----------------------------------------------------------------+
```

**Ne anlatır:** README üretim sırası; `[4]` bağımlılık yönü kapısı. **Durum:** `[3]` diskte YOK (§1.2/1) → sıra planlanmış, `⚠️ VERIFICATION REQUIRED`.

---

## §2 Kapsam

**İçinde:** katman README iskeleti, K kartı, dosya ağacı, tip yüzeyi, bağımlılık özeti, katman README'sine özgü ölçüm.
**Dışında:** kural metni (`.ai/.rules/`), katman spec'i (`layer-spec-template`), ADR (`adr-template`), kod, test dosyaları.

| Kapsam | Kapsam Dışı |
|--------|-------------|
| R1..R9 blok sırası + zorunlu tablolar | Sınıf içi tasarım (`layer-spec-template`) |
| K0..K8 kartı (tek satır sorumluluk) | Katman listesinin **tam** kopyası (SRP) |
| Bağımlılık yönü + ihlal sonucu | ADR gerekçesi |
| Dosya ağacı (kanıtlı) + tip yüzeyi | Kod içeriği / metot gövdesi |
| 6 salt-okunur PowerShell komutu | `src/` üretimi (developer işi) |
| README uzunluk/bütçe eşiği | Genel anlatım (`docs-md-template`) |

### §2.1 Dosya Tipi → Sorumlu Eşlemesi

| Dosya Tipi | Sorumlu | Bu Şablondan mı |
|------------|---------|------------------|
| `src/VersaCoder.<Katman>/README.md` | docs-writer | ✅ |
| `.ai/documentation/api/<servis>-api.md` | docs-writer | ❌ → `api-doc-template` |
| `C:\versacoder\README.md` (depo kökü) | docs-writer | ❌ → `readme-template` |
| `.ai/documentation/**` (genel anlatım) | docs-writer | ❌ → `docs-md-template` |
| `wiki/<konu>.md` | docs-writer | ❌ → `WikiPage-Template` |
| `architecture/**` katman tanımı | architect | ❌ → `architecture-template` |
| `src/**` kod dosyası | developer | ❌ → `backend/*` şablonları |

### §2.2 Kapsam Sınırı Testi

**İlk `EVET`** şablonu belirler:

1. Dosya **bir `src/` katmanı projesinin kökünde** mi, adı `README.md` mı? → **EVET** (bu şablon)
2. Depo kökü `README.md` mi? → `readme-template`
3. Bir HTTP arayüzünün sözleşmesi mi? → `api-doc-template`
4. Katmanın **derin** tasarımı (sınıf/etkileşim) mı? → `layer-spec-template`
5. `wiki/` altında bilgi sayfası mı? → `WikiPage-Template`
6. Hiçbiri değilse bu şablon **kullanılmaz**; talep §5.1 Adım 1'den açılır.

`HAYIR` soruları: kural paragrafı içeriyor mu (→ `.ai/.rules/`)? · `{{…}}` dışarıda mı (G4)? · kod kopyalanmış mı (G12)?

---

## §3 Mimari

### §3.1 Zorunlu Blok Sırası (R1..R9)

Üretilen `README.md` dokuz bloktur; sıra **değiştirilemez**. Eksik blok RED (§6.1 K5).

| # | Blok | İçerik | Min Satır |
|---|------|--------|-----------|
| R1 | `# H1` + **Zorunlu Bağlantılar** | 6 bağlantı, `·` ayracılı | 2 |
| R2 | Tek cümlelik sorumluluk | `>` ile 1–2 cümle | 2 |
| R3 | K kartı | kod · ad · sorumluluk · yön | 6 |
| R4 | Bağımlılıklar | yukarı / aşağı + yasak yön | 5 |
| R5 | Dosya ağacı | kanıtlı ağaç ya da `⚠️` | 8 |
| R6 | Tip yüzeyi | dışarıya açılan arayüz/enum | 6 |
| R7 | Kurallar özeti | o katmana değen G-maddeleri | 5 |
| R8 | Test + kanıt | hangi test, hangi coverage | 4 |
| R9 | Doğrulama | ölçüm özeti + Quality Report | 6 |

### §3.2 Frontmatter, H1 ve Bağlantı Satırı

| Öğe | Kural | Doğru | Yanlış |
|-----|-------|-------|--------|
| Frontmatter | 7 alan, ilk 9 satır | `authority: primary` | alan eksik / yazım farkı |
| H1 | tek tane | `# VersaCoder.Domain (K2)` | iki H1 · `##` |
| Bağlantı | `**Zorunlu Bağlantılar:**` + 6 `[[…]]` | `·` ayracılı | 5 altı · markdown `](…)` |
| Katman kodu | `K0`..`K8` birebir | `K2 Domain` | `K-2` · `katman2` · uydurma |
| Dil | Türkçe; tip/yol İngilizce | `IEntityRepository` | karışık dil |
| Ürün adı | `VersaCoder` | — | **CoreMusic ASLA** (G13) |

Katman kodu **yalnız** `architecture-template` §3.2'den gelir; README'de kod ile ad arasına boşluk konur (`K2 Domain`), tire kullanılmaz.

### §3.3 R3 — K Kartı Kuralları

| Alan | Kural | Yanlış |
|------|-------|--------|
| `Kod` | `K0`..`K8`, §3.2 ile birebir | uydurma kod |
| `Proje` | `src/VersaCoder.<Ad>/` gerçek adı | `Domain katmanı` (yol yok) |
| `Sorumluluk` | `architecture-template`'ten **tek satır** aynen | 3 cümlelik paragraf |
| `Yön` | `K6 → K7 → {K5, K4} → K3 → K2 → K1 → K0` içindeki yeri | ters yön |
| `Framework` | yalnız bu katmanda izinli olanlar | her katmanda `DevExpress` |
| `Durum` | `✅` (üretildi) · `⚠️ VERIFICATION REQUIRED` (diskte yok) | varsayılan `✅` |

### §3.4 R5 — Dosya Ağacı Kuralları

| Kural | Değer |
|-------|-------|
| Ağaç **gerçekten ölçülmüşse** yazılır | `Test-Path` çıktısı = kanıt |
| Biçim: ```` ```text ```` bloğu, `+--` / ``  `` girintisi | markdown madde listesi yasak |
| Klasör adı tam, kısaltma yok | `Repositories/` ≠ `Repo/` |
| Dosya sayısı tek satır özet | `14 dosya · 6 klasör` |
| `src/` yoksa ağaç **uydurulmaz** | `⚠️ VERIFICATION REQUIRED` yeterli |
| `.cs` dosya içeriği yazılmaz | yalnız adlar |

### §3.5 R6 — Tip Yüzeyi

```markdown
| Tip | Ad | Ne Açar | İzinli Kullanım |
|-----|----|---------|-----------------|
| interface | `IOrderRepository` | sorgu/güncelleme portu | yalnız K3 |
| record | `OrderLine` | değişmez veri taşıyıcı | her katman |
| enum | `OrderState` | durum makinesi | her katman |
```

**Yasak:** `internal` ayrıntı, `private` metot, somut `class` listesi (tip yüzeyi = dışarıya **açılan** tipler). Açılmayan tip README'ye girmez (SRP).

### §3.6 R7 — Kurallar Özeti (bağlantıdır, metin değil)

| Kural | Katmanda özeti | Ayrıntı |
|-------|----------------|---------|
| G2 | EF Core **yok** — ADO.NET + Dapper | `[[../.rules/data-access-rules]]` |
| G11 | `TreatWarningsAsErrors` + %90 coverage | `[[../.rules/testing-rules]]` |
| G10 | SQL yalnız `@p1..@pN` | `[[../query/Query-Template]]` |
| — | Katman yönü + `#if` yalnız platform farkı | `[[../architecture/architecture-template]]` |

Özet **2–4 maddeyi geçmez**; katmanla ilgisi olmayan madde README'ye girmez.

### §3.7 R9 — Doğrulama Kalıbı (ölçüm + yön)

**Ağaç kanıtlı mı, yön bozulmuş mu?** Komutlar salt-okunurdur; özet README'ye gömülür, tam metin §5.3'tedir (SRP).

```powershell
$readme = 'C:\versacoder\src\VersaCoder.Domain\README.md'
if (-not (Test-Path -LiteralPath $readme)) { 'README YOK -> bu sablonla uretilecek'; return }
$n = (Get-Content -LiteralPath $readme).Count
$kod = (Select-String -LiteralPath $readme -Pattern '^\| K[0-8] \|' -Encoding UTF8).Count
$yon = (Select-String -LiteralPath $readme -Pattern 'K6 → K7 → \{K5, K4\} → K3 → K2 → K1 → K0' -Encoding UTF8).Count
'giris {0} (100-260) · K karti {1} (1) · yon satiri {2} (>=1)' -f $n, $kod, $yon
```

### §3.8 Dolu VersaCoder Örneği ve Tam Kalıp

> Üst blok üretim standardı, alt blok yer tutucu taşıyan iskelettir. Yer tutucu **yalnız** alt blokta yaşar.

````markdown
# VersaCoder.Domain (K2)
**Zorunlu Bağlantılar:** [[../index]] · [[../CLAUDE]] · [[../architecture/architecture-template]] · [[../architecture/layer-spec-template]] · [[../adr/adr-template]] · [[../.rules/solid-rules]]

> Bu katman iş kurallarını ve invariantları taşır; dışarıya **asla** altyapı tipi açmaz.

## R3 K Kartı
| Alan | Değer |
|---|---|
| Kod · Ad | K2 · Domain |
| Sorumluluk | entity + invariant + domain event |
| Yöne izin | yalnız aşağı: `K1`, `K0` |
| Durum | ⚠️ VERIFICATION REQUIRED (`src/` diskte yok) |

## R8 Test + Kanıt
`tests/VersaCoder.Domain.Tests` — hedef %90 coverage · `dotnet test` çıktısı kanıttır.
````

**Tam kalıp (üretilecek README'nin iskeleti):**

````markdown
---
title: "VersaCoder — {{KATMAN}} README"
type: doc
category: layer-readme
status: draft
version: 1.0.0
updated: {{YYYY-MM-DD}}
authority: primary
---
# VersaCoder.{{KATMAN}} ({{K_KODU}})
**Zorunlu Bağlantılar:** [[{{BAG_1}}]] · [[{{BAG_2}}]] · [[{{BAG_3}}]] · [[{{BAG_4}}]] · [[{{BAG_5}}]] · [[{{BAG_6}}]]

> {{TEK_PARAGRAF — bu katman ne yapar, neyi asla yapmaz}}

## R3 K Kartı
| Alan | Değer |
|---|---|
| Kod · Ad | {{K_KODU}} · {{KATMAN}} |
| Sorumluluk | {{architecture-template tek satiri}} |
| Durum | {{✅ / ⚠️ VERIFICATION REQUIRED}} |
## R4 Bağımlılıklar
{{yukari · asagi · yasak yon}}
## R5 Dosya Ağacı
{{Test-Path kanitli agac ya da ⚠️}}
## R6 Tip Yüzeyi
{{disariya acilan tipler}}
## R7 Kurallar Özeti
{{2-4 G-maddesi + baglanti}}
## R8 Test + Kanıt
{{test projesi · coverage hedefi · kanit}}
## R9 Doğrulama
{{olcum ozeti + Quality Report}}
````

---

## §4 Kurallar

### §4.1 Yazım Sırası

| # | Adım | Kural | İhlal |
|---|------|-------|-------|
| 1 | Önce `architecture-template` | K kodu oradan gelir | uydurma kod RED |
| 2 | Blok sırası R1..R9 | blok yer değiştirmez (§3.1) | RED |
| 3 | `src/` ölç | yoksa `⚠️`, ağaç uydurulmaz | RED (G5) |
| 4 | Bağımlılık yaz | yön `§3.6` zinciriyle birebir | RED |
| 5 | Ölç | §5.3/1–§5.3/6 | RED |

### §4.2 Sorumluluk ve Onay

| Aşama | Rol | Ne Onaylar | Süre |
|-------|-----|-----------|------|
| 1 | docs-writer | iskelet + K kartı (R1..R9) | 20 dk |
| 2 | reviewer | §6.1 20 madde + ağaç kanıtı | 10 dk |
| 3 | architect | K kodu + yön `architecture-template` ile birebir | 6 dk |
| 4 | master-orchestrator | §5.3 çıktıları + §6.4 Quality Report | 6 dk |
| | **Toplam** | | **≈ 42 dk** |

> **3 RED** sonrası iş durur; gerekçe `master-orchestrator`'a yükseltilir.

### §4.3 Ölçek / Standartlar

| Öğe | Değer | Sapma |
|-----|-------|-------|
| Üretilen README | 100–260 satır | < 80 iskelet · > 300 RED |
| K kartı | 1 (tek katman) | 2+ RED — README katman değil vault olur |
| Dosya ağacı | 8–30 satır | > 40 RED (mimari bölünme sinyali) |
| Tip yüzeyi | 5–25 satır | > 30 RED → `layer-spec-template` |
| Kurallar özeti | 2–4 madde | 5+ RED (G12 tekrar) |
| Bağlantı | tam 6 | 5 altı / 8 üstü RED |
| Bu şablon | 480–660 satır · 25–34 KB | dışı §5.3/1 |
| Kodlama | UTF-8 BOM'suz | Latin-1/CP1254 otomatik RED |

### §4.4 Değişiklik Kontrolü

| Olay | Aksiyon | Etkilenen | Kayıt |
|------|---------|-----------|-------|
| Katman sorumluluğu değişti | önce `architecture-template`, sonra R3 | iki dosya | `log.md` + `version` minor |
| Yeni katman (K9) eklendi | önce mimari, sonra README | üç dosya | `version` major |
| Dosya ağacı değişti | R5 yeniden ölçülür | tek dosya | `version` patch |
| Bağlanan dosya taşındı | bağlantı hedefi yeniden yazılır | tek dosya | §5.3/6 = 0 |

### §4.5 Hard Guardrails (Otomatik DUR)

| # | Guardrail | İhlal Sonucu |
|---|-----------|--------------|
| G1 | Harici vault'tan (`C:\www\…`) cümle/dosya kopyalanması | **DUR** — silinir, özgün yazılır |
| G2 | `EntityFramework` / EF Core referansı | **DUR** — ADR-004, ADO.NET + Dapper |
| G3 | Frontmatter 7 alan eksik / `authority: primary` bozuk | **DUR** — §5.3/4 |
| G4 | Üretilen README'de `{{…}}` (kalıp bloğu hariç) | **DUR** — §5.3/4 |
| G5 | `src/` yokken dosya ağacı/tip listesi uydurmak | **DUR** — `⚠️ VERIFICATION REQUIRED` |
| G6 | K kodunu uydurmak/terste yazmak (`K8 Domain`) | **DUR** — `architecture-template` §3.2 |
| G7 | Bağımlılık yönüne aykırı iddia (`K2 → K6`) | **DUR** — §3.6 zinciri |
| G8 | README'ye kural paragrafı taşımak | **DUR** — 2–4 satır özet + bağlantı (G12) |
| G9 | ADR/karar gerekçesini README'ye yazmak | **DUR** — tek satır + bağlantı |
| G10 | Kod içeriği/metot gövdesi yapıştırmak | **DUR** — tip yüzeyi yeterli |
| G11 | `%90 coverage` / test kanıtı satırının kaybı | **DUR** — R8'de kalır |
| G12 | Katman listesinin tamamını README'ye kopyalamak | **DUR** — tek kart (SRP) |
| G13 | `CoreMusic` adının geçmesi | **DUR** — `VersaCoder` |
| G14 | Kanıtsız `✅` (üretildi işareti) | **DUR** — `Test-Path` yoksa `⚠️` |

---

## §5 Workflow

### §5.1 Adım Bazlı Akış

| # | Adım | Girdi | Çıktı | Yapan | Süre |
|---|------|-------|-------|-------|-------|
| 1 | Disk ölçümü | `Test-Path` listesi (§1.2) | güncel kanıt tablosu | docs-writer | 5 dk |
| 2 | İskelet seç | §3.1 (R1..R9) | blok listesi | docs-writer | 3 dk |
| 3 | K kartı | `architecture-template` §3.2 | R3 | architect | 5 dk |
| 4 | Bağımlılık | §3.6 zinciri | R4 | architect | 5 dk |
| 5 | Ağaç + tip yüzeyi | `src/` ölçümü | R5 + R6 | docs-writer | 8 dk |
| 6 | Kurallar özeti | `.ai/.rules/` (6 MD) | R7 (2–4 madde) | security | 6 dk |
| 7 | Test + kanıt | `tests/` durumu | R8 | tester | 6 dk |
| 8 | Ölç | §5.3/1–§5.3/6 | 6 çıktı | reviewer | 6 dk |
| 9 | Onay + kayıt | §6.1 20/20 | Quality Report | orchestrator | 5 dk |
| | **Toplam** | | | | **≈ 49 dk** |

**Zaman bütçesi:** 1–4 ≈ 18 dk · 5–7 ≈ 20 dk · 8–9 ≈ 11 dk. **1 saatten uzun** sürüyorsa tip yüzeyi şişirilmiş demektir → R6 yalnız dışarıya **açılan** tipleri tutar.

### §5.2 Hata Modları

| # | Hata Modu | Belirti | Kök Neden | Düzeltme |
|---|-----------|---------|-----------|----------|
| 1 | Uydurma K kodu | `K9` · `K2 UI` | `architecture-template` okunmamış | §3.2 tablosu |
| 2 | Ters yön | README'de `K2 → K6` | zincir ezberlenmiş | §3.6 |
| 3 | Uydurma ağaç | `src/` yokken dolu ağaç | §1.2 ölçülmemiş | `⚠️ VERIFICATION REQUIRED` |
| 4 | Kural kopyası | R7'de uzun paragraf | SRP unutulmuş | 2–4 satır + bağlantı |
| 5 | Eksik bağlantı | 5 veya daha az `[[…]]` | hedef unutulmuş | §3.2 altı hedef |
| 6 | Uzunluk sapması | > 300 / < 80 satır | doküman ya da iskelet | §4.3 aralığına çek |
| 7 | Tekrar eden katman listesi | README'de 9 satır K tablosu | G12 | tek kart |
| 8 | Mojibake + ürün adı | bozuk Türkçe · `CoreMusic` | ANSI kaydetme | UTF-8 BOM'suz · G13 |

### §5.3 Salt Okunur Doğrulama Komutları

Altı komut da **hiçbir dosyayı değiştirmez**; hedef yoksa dürüstçe söyler.

```powershell
# 1) YAPI — 7 ust bolum · 480-660 satir · 25-34 KB (bu sablon)
$doc = 'C:\versacoder\.ai\.templates\documentation\katman-readme-template.md'
$n = (Get-Content -LiteralPath $doc).Count
$kb = [math]::Round((Get-Item -LiteralPath $doc).Length / 1KB, 1)
$bol = (Select-String -LiteralPath $doc -Pattern '^## §\d ' -Encoding UTF8).Count
'boluH2 {0} (7 olmali) · satir {1} (480-660) · KB {2} (25-34) · {3}' -f $bol, $n, $kb,
  $(if ($bol -eq 7 -and $n -ge 480 -and $n -le 660 -and $kb -ge 25 -and $kb -le 34) { 'GECTI' } else { 'KALDI' })

# 2) YER TUTUCU — yalniz kalip blogundan sonrasi; -CaseSensitive zorunlu
#    (tr-TR + IgnoreCase, [A-Z] sinifinda I iceren kelimeleri iskalar)
$kalip = (Select-String -LiteralPath $doc -Pattern '^### §3\.8' -Encoding UTF8).LineNumber
$sizan = @(Select-String -LiteralPath $doc -Pattern '[{}]{2}[A-Z0-9_]+[}]{2}' -Encoding UTF8 -CaseSensitive |
           Where-Object { $_.LineNumber -lt $kalip })
'kalip satiri {0} · oncesinde yer tutucu {1} (0 olmali) · toplam {2}' -f $kalip, $sizan.Count,
  @(Select-String -LiteralPath $doc -Pattern '[{}]{2}[A-Z0-9_]+[}]{2}' -Encoding UTF8 -CaseSensitive).Count

# 3) ICERIK — K karti · yon zinciri · 6 baglanti · EF 0
$doc = 'C:\versacoder\.ai\.templates\documentation\katman-readme-template.md'
$katmanSatiri = @(Select-String -LiteralPath $doc -Pattern '^\| K[0-8] \| ' -Encoding UTF8).Count
$yon  = @(Select-String -LiteralPath $doc -Pattern 'K6 → K7 → \{K5, K4\} → K3 → K2 → K1 → K0' -Encoding UTF8).Count
$bag  = [regex]::Matches([System.IO.File]::ReadAllText($doc), '\[\[([^\]]+)\]\]') |
        ForEach-Object { $_.Groups[1].Value } | Sort-Object -Unique
$ef   = @(Select-String -LiteralPath $doc -Pattern 'EntityFrameworkCore' -Encoding UTF8).Count
'K satiri {0} (9) · yon {1} (>=1) · baglanti {2} (6) · EF referansi {3} (0 olmali)' -f $katmanSatiri, $yon, $bag.Count, $ef

# 4) FRONTMATTER + KUTU — 7 alan tam · ```text kutulari 66 karakter
$need = 'title','type','category','status','version','updated','authority'
$head = (Get-Content -LiteralPath $doc -TotalCount 12) -join "`n"
$eksik = @($need | Where-Object { $head -notmatch ('(?m)^' + [regex]::Escape($_) + ':') })
$box = $false; $satir = Get-Content -LiteralPath $doc -Encoding UTF8; $kotu = 0
for ($i = 0; $i -lt $satir.Count; $i++) { $l = $satir[$i]
  if ($l -match '^```text') { $box = $true; continue }
  if ($l -match '^```' -and $box) { $box = $false; continue }
  if ($box -and $l -match '^[+|]' -and $l.Length -ne 66) { $kotu++ } }
'eksik alan {0} (0 olmali) · kutu hizasiz {1} (0 olmali)' -f $eksik.Count, $kotu

# 5) UYDURMA — hedef dizinde olmayan dosya iddiasi var mi (⚠️ disinda)
$okunmamis = @(Select-String -LiteralPath $doc -Pattern '✅ 40.440 bayt|✅ 34.059 bayt' -Encoding UTF8).Count
'kanitli disk iddiasi sayisi {0} (varsa §1.2 ile birebir olmali)' -f $okunmamis
'src/ icin ⚠️ tasiyan satir {0} (>=1)' -f @(Select-String -LiteralPath $doc -Pattern '⚠️ VERIFICATION REQUIRED' -Encoding UTF8).Count

# 6) KODLAMA — BOM yok, mojibake 0, turkce var, kirik baglanti 0
$bytes = [System.IO.File]::ReadAllBytes($doc)
$bom = ($bytes.Length -ge 3 -and $bytes[0] -eq 0xEF -and $bytes[1] -eq 0xBB -and $bytes[2] -eq 0xBF)
$text = [System.IO.File]::ReadAllText($doc, [System.Text.UTF8Encoding]::new($false))
$moj = ([regex]::Matches($text, "\uFFFD|Ã[\u0080-\u00BF]|Â[\u0080-\u00BF]")).Count
$tur = ([regex]::Matches($text, '[çğıöşüÇĞİÖŞÜ]')).Count
$base = 'C:\versacoder\.ai\.templates\documentation'
$kirik = @([regex]::Matches($text, '\[\[([^\]]+)\]\]') | ForEach-Object { $_.Groups[1].Value } | Sort-Object -Unique |
  Where-Object { -not (Test-Path -LiteralPath (Join-Path $base ($_.Replace('/','\') + '.md'))) })
'BOM={0} · mojibake={1} (0 olmali) · turkce harf={2} (>0) · kirik baglanti={3} (0 olmali)' -f $bom, $moj, $tur, $kirik.Count
$kirik | ForEach-Object { '  KIRIK -> ' + $_ }
```

**Geçme kriteri:** #1 `GECTI` · #2 `0 sızan` · #3 `K satırı 9 · yön ≥ 1 · bağlantı 6 · EF 0` · #4 `0 eksik · 0 hizasız` · #5 `⚠ ≥ 1` · #6 `BOM=False · mojibake=0 · kirik=0`.

### §5.4 Quality Report

```markdown
## Quality Report — <Katman>/README.md (YYYY-MM-DD HH:MM)

| Metrik | Değer | Eşik | Durum |
|--------|-------|------|-------|
| Üretilen README | <n> satır / <KB> KB | 100–260 · 7–15 | 🟢/🔴 |
| R1..R9 blok | <n>/9 | 9 | 🟢/🔴 |
| K kartı · yön zinciri · tip yüzeyi | 1/1 · 1/1 · <n> | 1 · 1 · 5–25 | 🟢/🔴 |
| Bağlantı / kırık | 6 / 0 | tam 6 · 0 | 🟢/🔴 |
| Yer tutucu / uydurma ağaç / mojibake | n/n/n | 0/0/0 | 🟢/🔴 |
| Guardrail G1..G14 | ihlal n | 0 | 🟢/🔴 |
| Şablon ölçümü (§5.3/1) | <satır>/<KB> | 480–660 · 25–34 | 🟢/🔴 |

**Karar:** 🟢 ONAYLANDI (`src/` altına yazılır, `log.md`'ye 1 satır) / 🔴 RED (<gerekçe>)
```

---

## §6 Doğrulama

### §6.1 Kontrol Listesi (20 madde)

| # | Kontrol | Beklenen | İlgili |
|---|---------|----------|--------|
| K1 | Frontmatter 7 alan + `authority: primary` birebir | 7/7 | §3.2 · §5.3/4 |
| K2 | H1 tek + `**Zorunlu Bağlantılar:**` satırı | 1 satır | §3.2 |
| K3 | Bağlantı sayısı | 6 | §4.3 |
| K4 | Blok haritası R1..R9 | 9/9 | §3.1 |
| K5 | §1.1–§1.4 dolu; §1.2 ≥ 12 kanıt satırı | 4/4 · ≥ 12 | §1 |
| K6 | §1.3 README/spec/ADR ayrım tablosu | 6 satır | §1.3 |
| K7 | §1.4 bütçe 1.000 + kutu 66 | var · 0 uyuşmayan | §1.4 |
| K8 | §2.1 + §2.2 dolu | 2/2 | §2 |
| K9 | §3.1 R1..R9 tablosu | 9 satır | §3.1 |
| K10 | §3.3 K kartı kural tablosu | 6 satır | §3.3 |
| K11 | §3.6 kural özeti bağlantı tablosu | 4 satır | §3.6 |
| K12 | §3.8 dolu örnek + tam kalıp (`{{…}}` yalnız kalıpta) | 2 blok | §3.8 |
| K13 | §3.4/§3.5 ağaç ve tip yüzeyi kuralları | var | §3.4 · §3.5 |
| K14 | §4.5 G1..G14 | 14 madde | §4.5 |
| K15 | §5.1 9 adım · §5.2 ≥ 8 hata modu | 9 · 8 | §5 |
| K16 | §5.3 salt-okunur komut ≥ 5 | 6 | §5.3 |
| K17 | §5.4 Quality Report | var | §5.4 |
| K18 | §6.2–§6.5 dolu | 4/4 | §6 |
| K19 | §7.1 kaynak tablosu + §7.2 Red Team ≥ 10 satır | ≥ 10 | §7 |
| K20 | Ölçüm: 480–660 satır · 25–34 KB · `## §` = 7 · mojibake 0 | 4/4 | §5.3/1·6 |

### §6.2 Red Sinyalleri (Otomatik DUR)

| # | Sinyal | Aksiyon |
|---|--------|---------|
| R1 | §5.3/6'da kırık bağlantı | **DUR** — hedef üretilir ya da bağlantı silinir |
| R2 | Üretilen README'de `{{…}}` | **DUR** — doldurulur (G4) |
| R3 | Uydurma K kodu / ters yön | **DUR** — `architecture-template` §3.2 |
| R4 | `src/` yokken dolu dosya ağacı | **DUR** — G5 · `⚠️` |
| R5 | README'de 9 satırlık katman listesi | **DUR** — G12, tek karta indir |
| R6 | `EntityFramework` geçen satır | **DUR** — G2 |
| R7 | R7'de uzun kural paragrafı | **DUR** — bağlantıya çevir (G8) |
| R8 | `CoreMusic` / mojibake / BOM | **DUR** — G13 · §5.3/6 |
| R9 | Ölçüm dışı uzunluk | **DUR** — §4.3 |
| R10 | §5.3 çıktısı olmadan "tamam" beyanı | **DUR** — çıktı istenir |

### §6.3 Sürüm / Değişiklik Geçiş Kuralları

| Değişiklik | `version` | `updated` | Kayıt |
|------------|-----------|-----------|-------|
| K kodu / sorumluluk değişimi | `minor` | evet | önce `architecture-template`, sonra `log.md` |
| Yalnız düzeltme (ımle/bağlantı) | `patch` | evet | `log.md` tek satır |
| Dosya ağacı değişimi (ağaç ölçümü) | `patch` | evet | ölçüm çıktısı |
| Blok adı değişimi (`R5` gibi) | `major` | evet | bağlantı veren tüm dosyalar |

### §6.4 İlgili Dosyalar

| Dosya | İlişki |
|-------|--------|
| `.ai/.templates/documentation/katman-readme-template.md` | **bu dosya** — katman README'sinin tek kaynağı |
| `.ai/.templates/architecture/architecture-template.md` | K0..K8 tek kaynağı (R3 · §3.6) |
| `.ai/.templates/architecture/layer-spec-template.md` | derin spec — README'ye girmez (§1.3) |
| `.ai/.templates/architecture/ascii-diagram-template.md` | kutu biçimi + 66 ölçer (§5.3/4) |
| `.ai/.templates/documentation/api-doc-template.md` | katman HTTP yüzeyi bu şablondan |
| `.ai/.rules/solid-rules.md` | bağımlılık kuralının metni (G7) |

### §6.5 Envantere Özgü Kontroller

| # | Kontrol | Beklenen | Kanıt |
|---|---------|----------|-------|
| K1 | Bu şablonun satır aralığı | 480–660 | §5.3/1 |
| K2 | Bu şablonun KB aralığı | 25–34 | §5.3/1 |
| K3 | Hedef dosya yolu envantere birebir | `src/VersaCoder.<Katman>/README.md` | `index.md` |
| K4 | §1.2 disk tablosu üretim anında yeniden ölçülmüş | tarihli | §5.3/5 |
| K5 | Kutu satırları 66 karakter | 0 uyuşmayan | §5.3/4 |
| K6 | Yer tutucu yalnız §3.8 kalıp bloğunda | 0 sızan | §5.3/2 |

---

## §7 Referanslar

### §7.1 VersaCoder Bağlantıları

| Kaynak | Yol | Ne İçin | Durum (2026-09-25) |
|--------|-----|---------|---------------------|
| Şablon envanteri + kalite sözleşmesi | `.ai/.templates/index.md` | kayıt + §3.7 | ✅ 40.440 bayt |
| K0..K8 tek kaynağı | `.ai/.templates/architecture/architecture-template.md` | R3 · §3.6 | ✅ 34.059 bayt |
| Katman derin spec'i | `.ai/.templates/architecture/layer-spec-template.md` | §1.3 ayrım | ✅ 34.783 bayt |
| Kutu biçimi | `.ai/.templates/architecture/ascii-diagram-template.md` | §5.3/4 · 66 | ✅ 38.118 bayt |
| Kural metni (6 MD) | `.ai/.rules/*.md` | R7 · §5.3 | ✅ 13–17 KB/dosya |
| Karar defteri | `.ai/.decisions/adr-index.md` | G9 · tek satır | ✅ okundu |
| Kod ağacı gerçeği | `src/**` | R5 · §5.3/5 | ❌ YOK → `⚠️` |
| Yasaklı vault'lar | `C:\www\coremusic.net\…` · `C:\www\versacoder\…` | yalnız **yapı** referansı | 🔒 kopyalama YASAK |

### §7.2 Red Team — Sık Yapılan Hatalar

| # | Hata | Neden Yanlış | Doğrusu | Yakalama |
|---|------|--------------|---------|----------|
| 1 | K kodunu uydurmak (`K9`, `K2 UI`) | tüm vault K0..K8'e bağlanır | `architecture-template` §3.2 | §5.3/3 |
| 2 | Bağımlılık yönünü terste yazmak | README yanlış mimariyi onaylar | §3.6 zinciri | §5.3/3 |
| 3 | `src/` yokken ağaç uydurmak | kanıtsız iddia, ajan yanlış yere gider | `⚠️ VERIFICATION REQUIRED` | §1.2 |
| 4 | Katman listesini 9 satır kopyalamak | SRP + okuma bütçesi şişer | tek K kartı | G12 |
| 5 | Kural paragrafını README'ye taşımak | iki gerçek kaynağı, biri çürür | 2–4 satır + bağlantı | §3.6 |
| 6 | Derin spec'i README'ye yapıştırmak | 600 satırlık README, kimse okumaz | `layer-spec-template` bağlantısı | §4.3 |
| 7 | Her katmanda `DevExpress`/`ADO.NET` yazmak | katmanlar arası sorumluluk bulanıklığı | izinli framework tablosu (R3) | gözden geçirme |
| 8 | 6 bağlantıyı 12'ye çıkarmak | okuma bütçesi şişer | tam 6, `·` ayracılı | §5.3/3 |
| 9 | EF Core / `DbSet` geçirmek | ADR-004 + G2 ihlali | ADO.NET + Dapper | §5.3/3 |
| 10 | `CoreMusic` adını geçirmek | yasaklı vault izi (G1) | `VersaCoder` | §5.3/6 |
| 11 | ANSI/CP1254 kaydetmek | bozuk Türkçe, okunmaz | UTF-8 **BOM'suz** | §5.3/6 |
| 12 | Ölçümden "tamam" demek | kanıtsız beyan (G5) | 6 komutun çıktısı | §5.3/1–6 |

**SSUT beyanı:** `src/VersaCoder.<Katman>/README.md` üretiminin **tek** kaynağı bu dosyadır; katman tanımı `architecture-template`'te, derin spec `layer-spec-template`'tedir — ikisi de bu README'ye **bağlanır**, kopyalanmaz. `⚠️ VERIFICATION REQUIRED` yazan hiçbir satır "üretildi" diye anılmaz (G14).
