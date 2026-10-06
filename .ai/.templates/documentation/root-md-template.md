---
title: "Şablon — Kök MD Belgesi (root-md)"
type: template
category: templates/documentation
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
scope: "kok MD sablonu: tek ekran ozet, ust/alt baglantilar ve durum satiri"
---

# Şablon — Kök MD Belgesi (`root-md-template.md`)

**Zorunlu Bağlantılar:** [index.md](../index.md) · [CLAUDE.md](../../CLAUDE.md) · [docs-md-template.md](./docs-md-template.md)

> **Tek cümlelik sorumluluk:** `.ai/` kökündeki 14 zorunlu belgenin **30–88 KB** bandını, ortak iskeletini ve birbirinden farkını tanımlar.
> Şablon **kendi başına** bir kök belge değildir; kök belgeler bunu temel alır.

## İçindekiler

| Bölüm | Ne verir |
|---|---|
| §1 Amaç | Neden ayrı şablon + G12 bağlantılı yazım ilkesi |
| §2 Kapsam | 14 belge envanteri, KB bandı, kapsam dışı |
| §3 Mimari | 8 alanlı frontmatter, bağ satırı, B2–B8 blokları, okuma sırası, iskelet |
| §4 Kurallar | Ret gerekçeleri, yasak içerik, hata modları |
| §5 Workflow | Yazım/güncelleme akışı ve kapılar |
| §6 Doğrulama | Checklist, ölçüm komutları, rapor formatı |
| §7 Referanslar | Kaynaklar ve çapraz bağlantılar |

---

## §1 Amaç

### §1.1 Neden ayrı bir kök şablon

| # | Gerekçe | Bozulursa |
|---|---|---|
| 1 | Kök MD'ler vault'un omurgası; ortalama 30–88 KB | Her biri kendi başına yazılır → kalite tutarsız |
| 2 | 14 dosyanın her biri *tek sorumluluk* taşır (SRP) | `index.md` + `brain.md` aynı bilgiyi barındırır → SSUT ihlali |
| 3 | `keys.md` zorunlu anahtar kelime haritasıdır | Arama çalışmaz, vault ölü olur |
| 4 | Hepsi aynı frontmatter + bağ satırı kullanır | Navigasyon kırılır |
| 5 | Ölçüm tek yerden yapılır | 30–88 KB bandı tutmaz |

### §1.2 G12 — kural metni değil, bağlantı

Kök MD'nin yazım ilkesi tek satırla özetlenir (CLAUDE.md `scope` satırı, birebir):

> kural metni değil, bağlantı

Kök belge, kuralın **kendisini** kopyalamaz; kuralın yaşadığı yere işaret eder. Kural tek
yerde yaşar (`.ai/.rules/` · `.ai/CLAUDE.md` §4.5), kök belge onu **özetler ve bağlar**.

| Durum | Davranış | Sonuç |
|---|---|---|
| Doğru | Tek satır özet + `[göster]\(hedef\)` bağlantısı | Tek SSOT, güncelleme tek yerde |
| Yanlış | Kuralın tam metni köke kopyalanır | İki SSOT; biri güncellenince çelişki (G12 ihlali) |
| Yanlış | Bağlantı olmadan "bkz. kurallar" denmesi | Okur hedefi bulamaz, vault ölü bağlantı |
| Yanlış | Bağlantı verilen hedef diskte yok | Kırık link → GATE-004a RED |

**Örnek — doğru tek satır:**

```markdown
| G12 | Envantere içerik kopyalanamaz (SRP) | [templates]\(../.templates/index.md\) |
```

**Örnek — yanlış (yasak):** kural maddesinin 20 satırlık tam metninin kök belgeye gömülmesi.
O metin `.ai/.rules/` içinde yaşar; kökte yalnız özeti + bağlantısı kalır.

### §1.3 Bu şablon ne üretir, ne üretmez

| Üretir | Üretmez |
|---|---|
| Kök MD için tek doğrulanabilir iskelet | Kendisi bir kök belge (envanterde 15. satır değildir) |
| 8 alanlı frontmatter zorunluluğu | Kural metni kopyası |
| Ret gerekçeleri (§4.2) + ölçüm komutu (§6.2) | Uydurma istatistik, tahmini KB |
| 14 belgenin tek sorumluluk farkı | Her belgenin içeriği (o belgenin işi) |

---

## §2 Kapsam

### §2.1 14 kök belge envanteri

| # | Dosya | Tek sorumluluk | Hedef KB | `type` | Doğrulama |
|---|---|---|---|---|---|
| 1 | `index.md` | Vault navigasyonu — tüm dosyaların indeksi | 30–45 | `index` | Her link hedefi mevcut |
| 2 | `AGENTS.md` | Ajan kaydı, yetkiler, handover protokolü | 30–40 | `registry` | Ajan sayısı `.agents/` ile eşit |
| 3 | `CLAUDE.md` | AI anayasası — hard guardrail'lar | 35–60 | `constitution` | G1..G12 tek tek tanımlı |
| 4 | `WORKFLOW.md` | Süreçler, fazlar, gate'ler | 35–65 | `workflow` | GATE-004a..006 tanımlı |
| 5 | `ROLE.md` | Mutlak rol tanımı + roller arası öncelik | 30–50 | `role` | Rol çatışması kuralı var |
| 6 | `keys.md` | **Anahtar kelime → hedef dosya haritası** | 35–70 | `navigation` | Zorlu anahtarlar §3'te |
| 7 | `glossary.md` | Terimler sözlüğü (TR/EN) | 30–45 | `reference` | Her terim tek satır |
| 8 | `brain.md` | ADR özetleri + mühendislik kısıtları | 40–88 | `decisions` | Her ADR linkli |
| 9 | `MEMORY.md` | Kalıcı bellek — oturumlar arası durum | 30–50 | `memory` | Silme/kalıcılık kuralı |
| 10 | `VISION.md` | Ürün vizyonu, problem–çözüm matrisi | 30–50 | `vision` | 7 ekranla hizalı |
| 11 | `PROJECTS.md` | Proje kapsamı, hedefler, bileşenler | 30–50 | `project` | Kapsam sınırı açık |
| 12 | `log.md` | Kural değişikliği + kalite raporu günlüğü | 30–60 | `log` | Tarihli kayıt zorunlu |
| 13 | `engine.md` | Orkestrasyon yürütme matrisi | 30–50 | `engine` | Gate ↔ part eşleşmesi |
| 14 | `ULTRA-THINKING.md` | Derin düşünme protokolü + şablonlar | 30–45 | `protocol` | Adım sayısı somut |

**Toplam hedef:** 14 dosya · **~500–800 KB** · hepsi YAML frontmatter'lı.

### §2.2 Bütçe ve band

| Ölçü | Alt sınır | Üst sınır | Aşarsa |
|---|---|---|---|
| Tek kök dosya | 30 KB | 88 KB | ±%10 tolerans dışı → RET |
| Satır sayısı | belgeye göre | — | Alt sınırı aşan boş satır = dolgu |
| Token bütçesi | — | 50.000 (B9) | Sıkıştırma tetiklenir |
| Frontmatter | 8 alan | 8 alan | Eksik/fazla alan → RET |

### §2.3 Kapsam dışı

Bu şablon şunları **kapsamaz** (her biri başka belgenin sorumluluğu):

1. Kural metni — `.ai/.rules/` · kural özeti + bağlantı: G12
2. Şablon envanteri — [index.md](../index.md) §3.7 kalite sözleşmesi
3. Kapı tanımı — [WORKFLOW.md](../../../WORKFLOW.md) GATE-004a..006
4. Anayasa metni — [CLAUDE.md](../../../CLAUDE.md) B1..B10 + G1..G12
5. Doküman şablonları — [docs-md-template.md](./docs-md-template.md)
6. Kod yazımı, refactor, test üretimi (kapı öncesi G8)
7. ADR'nin tam metni — `brain.md` özetler, `.ai/.decisions/` yaşar
8. Ekran görüntüsü üretimi (görsel yalnız ilgili belgede)
9. Sürüm/CPM kilidi — `Directory.Packages.props`
10. Görev dağıtım planı — `engine.md`
11. Anahtar kelime haritasının kendisi — `keys.md`
12. Örnek README içeriği — [readme-template.md](./readme-template.md)
---

## §3 Mimari

### §3.1 8 alanlı frontmatter (zorunlu, byte-byte aynı sıra)

Her kök MD **tam 8 alan** ile açılır; alan adları, sırası ve tırnak biçimi değişmez:

| # | Alan | Zorunlu değer | Format | Red gerekçesi |
|---|---|---|---|---|
| 1 | `title` | Belge adı + vault | çift tırnaklı metin | tırnaksız / çoklu satır |
| 2 | `type` | envanterdeki 14 değerden biri | düz metin | tanımsız `type` |
| 3 | `category` | `vault-root` (diğerleri: `templates/documentation` gibi) | düz metin | boş |
| 4 | `status` | `active` | düz metin | `draft` ile kalıcılaşma |
| 5 | `version` | semver (örn. `1.0.0`) | `MAJOR.MINOR.PATCH` | elle abartılı bump |
| 6 | `updated` | ISO tarih | `YYYY-MM-DD` | güncellemede taze lenmemiş |
| 7 | `authority` | **`SSOT`** — tek değer | düz metin | `draft` / `reference` / boş |
| 8 | `scope` | tek cümlelik kapsam | çift tırnaklı metin | çoklu cümle, kural metni |

**`authority: SSOT` kuralı:** kök MD, kapsadığı konunun **tek doğruluk kaynağıdır**. İki kök
belge aynı konuyu SSOT olarak iddia edemez — ikincisi `reference`/özet durumuna düşer ve
`scope` satırında bunu söyler. `authority` alanını `SSOT` dışı bir değerle görmek = RET
(§4.2/2).

**Alan sayısı kontrolü** (aşağıdaki komut 8 dışındaki sayıyı yakalar):

```powershell
$f = 'C:\versacoder\CLAUDE.md'   # ornek koku belge
$m = Get-Content -Encoding UTF8 $f | Select-String -Pattern '^[a-z]+:'
if ($m.Count -ne 8) { Write-Error "frontmatter alan sayisi = $($m.Count), beklenen 8" }
```

### §3.2 Bağ satırı formatı (tek satır, `·` ayracılı)

H1'den hemen sonra **tek** bağlantı satırı gelir:

```markdown
**Zorunlu Bağlantılar:** [index.md]\(index.md\) · [AGENTS.md]\(AGENTS.md\) · [CLAUDE.md]\(CLAUDE.md\) · [WORKFLOW.md]\(WORKFLOW.md\) · [keys.md]\(keys.md\) · [brain.md]\(brain.md\) · [MEMORY.md]\(MEMORY.md\) · [log.md]\(log.md\) · [index]\(.templates/index.md\)
```

| Kural | Ayrıntı | Red |
|---|---|---|
| Ayraç | yalnız `·` (boşluk + nokta + boşluk) | `,` / `|` ayraç |
| Hedef | göreli yol, diskte mevcut | `./ksimli` mutlak yol |
| Ad | bağlantı metni = dosya adı | metin ≠ hedef |
| Kapsam | en az index · AGENTS · CLAUDE · WORKFLOW · keys · brain · MEMORY · log · şablon index | eksik hedef |
| Sayı | 9 hedef (bu şablonun kendi satırı 3'tir — şablon dosyaları hariç) | kalabalık 20+ link |

**Şablon dosyaları** (`*-template.md`) kendi satırlarında yalnız **mevcut** hedeflere bağlanır;
envanterdeki kök dosyaların adları düz metin (kod içi) geçer. Aksi halde şablon kendi
kuralını ihlal eder.

### §3.3 B2–B8 blok yapısı (kök MD gövdesi)

`CLAUDE.md`-başlatan blok sırası B1..B10'dur; bu şablonun dayattığı **gövde çekirdeği
B2–B8**'dir:

| Blok | Ad | Zorunlu içerik | Min satır |
|---|---|---|---|
| B1 | H1 + bağ satırı | başlık, `·` ayracılı bağlantılar | 2 |
| B2 | Tek cümlelik sorumluluk | `>` ile 1–2 cümle — dosyanın tek cevabı | 2 |
| B3 | Rol tanımı | neyi üstlenir / üstlenmez (iki sütun) | 6 |
| B4 | Vault bağları + okuma sırası | §3.4 tablosu | 8 |
| B5 | Katman listesi | l0..l7 + test, 9 satır | 11 |
| B6 | Kurallar özeti | G1..G12 tek satır + bağlantı (G12) | 14 |
| B7 | Ne yap / Ne yapma | iki sütun ≥ 10 satır | 12 |
| B8 | Dosya yolları ve disk durumu | yol + ✅/❌ + ölçüm tarihi | 12 |

**Sıra değişmez** (B2 → B8); bloklar atlanamaz, yeniden adlandırılamaz. Bir blok boş kalırsa
satırı silinmez — içeriği üretilir. Eksik blok = §4.2/6 ret gerekçesi.

### §3.4 B4 — Vault bağları ve okuma sırası

| Sıra | Okunan | Ne verir | Tekrar eder mi |
|---|---|---|---|
| 1 | [CLAUDE.md](../../../CLAUDE.md) | rol, vault bağları, katman özeti, yasaklar | Hayır — yalnız bağlantı |
| 2 | [index.md](../index.md) | şablon envanteri + §3.7 kalite sözleşmesi | Hayır |
| 3 | rules dizini (`.ai/.rules/`) | kalıcı kural metni | Hayır |
| 4 | mimari + [WORKFLOW.md](../../../WORKFLOW.md) | katman gerçeği · kapı gerçeği | Hayır |
| 5 | Görevin kendi şablonu | üretilcek dosyanın iskeleti | Hayır |

**İlke:** hiçbir satır "tekrar eder mi" sütununda *Evet* olamaz — *Evet* = kopya = G12 ihlali.

### §3.5 B5 — Katman listesi (kök MD gövdesine birebir girer)

| # | Makro | Proje | Tek satır sorumluluk |
|---|---|---|---|
| 1 | `l0` Abstractions | `VersaCoder.Abstractions` | port arayüzleri, `Result<T>`, event sözleşmesi |
| 2 | `l1` Domain | `VersaCoder.Domain` | entity, invariant, domain event |
| 3 | `l2` Application | `VersaCoder.Application` | use case, CQRS handler, orkestrasyon |
| 4 | `l3` Crosscutting | `VersaCoder.Crosscutting` | logging, `IClock`, dosya/yardımcılar |
| 5 | `l4` Infrastructure | `VersaCoder.Infrastructure` | ADO.NET + Dapper, dosya, git, LLM sağlayıcı |
| 6 | `l5` Protocol | `VersaCoder.Protocol` | mesaj şeması, tool sözleşmeleri |
| 7 | `l6` Host | `VersaCoder.Host` | composition root, DI, `appsettings*.json` |
| 8 | `l7` UI + CLI | `VersaCoder.Desktop` + `VersaCoder.Cli` | RibbonForm sunumu / CLI ayrıştırma |
| — | test | `VersaCoder.Tests` | kanıt üretir; hedef %90 coverage |

**Yön:** `l7 -> l6 -> l5 -> l4 -> l2 -> l1 -> l0`; `l3` yalnız `l0`'a bakar; `l2` ile `l4`
arasında **doğrudan ok YOK**. Test katman değildir — ayrı projedir.

### §3.6 İskelet (kök MD şablonu)

```markdown
---
title: "<Belge Adı> — VersaCoder Vault"
type: <index|registry|constitution|workflow|role|navigation|reference|decisions|memory|vision|project|log|engine|protocol>
category: vault-root
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
scope: "<tek cümle>"
---

# <Belge Adı>

**Zorunlu Bağlantılar:** [index.md](index.md) · [AGENTS.md](AGENTS.md) · [CLAUDE.md](CLAUDE.md) · [WORKFLOW.md](WORKFLOW.md) · [keys.md](keys.md) · [brain.md](brain.md) · [MEMORY.md](MEMORY.md) · [log.md](log.md) · [index](.templates/index.md)

> **Tek cümlelik sorumluluk:** <bu dosyanın tek cevabı olduğu soru>

## İçindekiler
| Bölüm | Alt Konular |
|---|---|

## §1 Amaç
## §2 Kapsam
## §3 İçerik (ana gövde — 30 KB'lık kısım burada)
## §4 Kurallar
## §5 Doğrulama
## §6 Referanslar
```

**Not:** iskeletteki § başlıkları bu şablonun **kendi** §1..§7 sırasıyla çakışıyorsa, iç belge
kendi konu başlığını kullanır; değişmez olan **sıra** ve **numaralandırmadır** (§6.1/3).

### §3.7 Zorunlu anahtar kelimeler (`keys.md` hariç hepsinde geçmeli)

```
VersaCoder · VERSA CODER · agentic IDE · AI coding platform
RibbonControl · RibbonForm · RibbonControlStyle.Office2010 · DevExpress v26.1
Designer.cs · .resx · ResourceManager · WinForms
net48 · net10.0-windows · #if · iki bacak
ADO.NET · Dapper · parameterized · migration · version tablosu
Clean Architecture · SOLID · CQRS · DDD · DI
Protocol · Host · Abstractions · Domain · Application · Infrastructure
opencode · tool · agent · session · MCP · skill · streaming
Desktop · Cli · GATE · ADR · SSOT · frontmatter
```

### §3.8 KB bandını doldurma yöntemi (kopyalamadan)

30–88 KB'ı **kopyala-yapıştır olmadan** doldurmak için:

| # | Teknik | Örnek | Ek KB |
|---|---|---|---|
| 1 | **Karar tablosu** — her satır gerekçe + alternatif + sonuç | ADR özeti 25 satır | +3–6 |
| 2 | **Kapsam dışı listesi** — ne yapmadığını yaz | 10–15 madde | +2–4 |
| 3 | **Kontrol checklist'i** — ölçülebilir `[ ]` maddeleri | 20–40 madde | +3–7 |
| 4 | **Hata modları tablosu** | 8–12 satır | +2–5 |
| 5 | **ASCII diyagram** (teknik belgelerde) | 30–60 satır | +2–5 |
| 6 | **Örnek/girişim** — gerçek VersaCoder senaryosu | 5–10 blok | +4–10 |
| 7 | **Red team** — sık yapılan hatalar | 8–12 satır | +2–4 |

**Yasak dolgu:** CoreMusic içeriği kopyası · tekrar eden tablo · uydurma istatistik · "detaylıdır"
lafı · aynı cümlenin farklı sözcüklerle tekrarı · dolgu amaçlı boş satır yığını.

**Denge tablosu:** teknik 1 satırın alt sınırı aşmaz; bir tekniğin eklediği KB, o belgenin
eksik kalan bölümüne yazılır (ölçüm → bölge → teknik, §6.2 akışı).


### §3.9 Dolu örnek — ilk 12 satır (birebir kopyalanabilir)

```markdown
---
title: "CLAUDE.md — AI Anayasası"
type: constitution
category: vault-root
status: active
version: 1.0.0
updated: 2026-10-05
authority: SSOT
scope: "Depo kökü CLAUDE.md: rol, vault bağları, katman özeti, G1..G12, bütçe ve doğrulama"
---

# CLAUDE.md

**Zorunlu Bağlantılar:** [index.md]\(index.md\) · [AGENTS.md]\(AGENTS.md\) · [WORKFLOW.md]\(WORKFLOW.md\)
```

**Neden doğru:** 8 alan tam · sıra sabit · `authority: SSOT` · tek cümlelik scope · bağ satırı
H1'den hemen sonra ve **kaçışlı örnek** (§4.3/7). **Neden yanlış olurdu:** 9. alan eklenmesi,
`updated`'in bugünün tarihiyle elle yazılması, bağ satırının blok arasına düşmesi, örnekteki
linkin gerçek (kaçışsız) link olması — tarama onu kırık sayar.

**Uygulama sırası:** iskeleti kopyala (§3.6) → alanları doldur (§3.1) → bağ satırını yaz
(§3.2) → B2..B8'e geç (§3.3). Bu sıra ters çevrilirse §4.2/6 RED doğar.

---

## §4 Kurallar

### §4.1 Temel kurallar

| # | Kural | Gerekçe | İhlalde |
|---|---|---|---|
| 1 | Frontmatter tam **8 alan**, sabit sıra | Otomatik ayrıştırıcılar sıraya güvenir | §4.2/1 |
| 2 | `authority: SSOT` | Konunun tek doğruluk kaynağı | §4.2/2 |
| 3 | Bağ satırı H1'den hemen sonra, `·` ayraçlı | Navigasyon tek satırda taranır | §4.2/3 |
| 4 | B2..B8 blok sırası değişmez | Okuma sırası alışkanlığı + denetim | §4.2/6 |
| 5 | Kural metni değil, **bağlantı** (G12) | İki SSOT doğuran kopya çelişkisi | §4.2/5 |
| 6 | 30–88 KB bandı (±%10) | Aşırı kısa = eksik, aşırı uzun = dolgu | §4.2/7 |
| 7 | Tek H1, § numaralı bölümler | İçindekiler + derin bağlantı | §4.2/8 |
| 8 | Bağlantılar göreli ve diskte mevcut | Kırık link vault'u ölü yapar | §4.2/4 |
| 9 | UTF-8 (BOM yok), mojibake yok | Git farkı ve arama bozulur | §4.2/9 |
| 10 | DevExpress **yalnız v26.1**, EF Core **yok** | Sürüm sapması · mimari yasak | §4.2/11 |

### §4.2 Ret gerekçeleri (RED — biri bile yeter)

| # | Ret gerekçesi | Yakalama yöntemi |
|---|---|---|
| 1 | Frontmatter 8 alan değil | §3.1 alan sayma komutu |
| 2 | `authority` alanı `SSOT` değil ya da eksik | ilk 10 satır kontrolü |
| 3 | Bağ satırı yok / ayraç `·` değil / H1'den sonra boşluklu | satır başı okuma |
| 4 | Bağlantı hedefi diskte yok | her `]\(…\)` için `Test-Path` |
| 5 | Kural metni köke kopyalanmış (G12) | çapraz okuma: `.ai/.rules/` ile aynı blok |
| 6 | B2..B8 bloklarından biri yok ya da sıra bozuk | §3.3 tablosuyla satır satır karşılaştırma |
| 7 | Boyut 30 KB altı / 88 KB üstü (±%10 dışı) | §6.2 KB ölçümü |
| 8 | Çift H1 ya da `§` numarasız başlık | `^# ` ve `^## §` taraması |
| 9 | BOM var ya da mojibake var | ilk 3 bayt + U+00C3 deseni (§6.2) |
| 10 | Uydurma istatistik / "detaylıdır" dolgusu | Red Team okuması (§6.3) |
| 11 | EF Core geçiyor / DevExpress sürümü sapmış | `Directory.Packages.props` kilidiyle karşılaştırma |
| 12 | `keys.md` gerektiren zorlu anahtar bu belgede yok | §3.7 kelime listesiyle kesişim |
| 13 | Kanıtsız "tamamlandı" / "geçti" | her iddia = komut çıktısı ya da `?` |
| 14 | Ölçüm sayısı tahmin edilmiş (uydurulmuş) | §6.2 çıktısı raporla satır satır eşleşmeli |

**Sıra numaraları sabittir** — §3.1 ve §3.3 bu numaralara atıf yapar; ekleme yapılırken mevcut
numaralar kaydırılmaz, yeni madde **14'ten sonra** eklenir.

### §4.3 Yasak içerik

| # | Yasak | Neden |
|---|---|---|
| 1 | Kural maddesinin tam metni | G12 — kural `.ai/.rules/` içinde yaşar |
| 2 | CoreMusic içeriği kopyası | Konu dışı + telif |
| 3 | Tekrar eden tablo (aynı tablo iki bölümde) | SRP ihlali, ölçüm şişer |
| 4 | Uydurma istatistik ("%80 verimlilik") | Kanıtsız iddia = §4.2/10 |
| 5 | Boş satır yığını / dolgu paragrafı | KB bandını sahte doldurur |
| 6 | Mutlak yol (`C:…`) bağlantı | Taşınabilirlik kırılır |
| 7 | Kod içinde link gösterimi | Tarama örneği kırık link sanır (§6.2 kaçışı) |
| 8 | Tahmini sürüm / elle yazılmış tarih | Kilit dosya + `updated` çelişir |

### §4.4 Hata modları

| # | Belirti | Kök neden | Düzeltme |
|---|---|---|---|
| 1 | § başlıkları yanlış sırada (§3, §1'den önce) | Bölüm dosya sonuna eklenmiş | § sırası yeniden kurulur; içerik silinmez |
| 2 | Tarama "kırık link" diyor, hedef var | Kod bloğu içindeki örnek link | §4.3/7: örnekler kaçışlı yazılır |
| 3 | Frontmatter 9 satır | Yanlışlıkla eklenmiş alan (örn. `author`) | fazla alan silinir, 8 korunur |
| 4 | Git farkı her satırda | BOM ya da satır sonu karışması | UTF-8 BOM yok, tek satır sonu biçimi |
| 5 | Arama bozuk karakter buluyor | Kaydetme yanlış kodlama | dosya UTF-8 olarak yeniden kaydedilir |
| 6 | Boyut sınırda dalgalanma | Dolgu ile şişirme | §3.8 tekniklerinden eksik bölgeye yazılır |
| 7 | Blok başlıkları kod bloğu içinde de geçiyor | İskelet örneği gerçek başlık sanılır | Ölçüm blok durumunu izleyerek yapar (§6.1/3) |
| 8 | Bağ satırı kaymış (H1'den sonra iki boşluk) | Şablon kopyasında satır unutulmuş | §3.2 formatı yeniden uygulanır |
| 9 | Kod bloğu içinde gerçek başlık var | İskelet örneği düz metne açılmış | Örnek blok fence içinde tutulur |
| 10 | §3.7 anahtarlarının yarısı eksik | Kelime listesi okunmadan yazılmış | §3.7 kesişimi kontrol edilir |
| 11 | Rapor satırında `?` yerine tahmin yazıyor | Ölçüm komutu çalıştırılmamış | §6.2/1–4 çalıştırılır |
| 12 | Bölgesel ölçümde tek bölge kırmızı | Toplam yeşil diye geçildi | §6.4 tablosuna bakılır, eksik bölge tamamlanır |

---

## §5 Workflow

### §5.1 Kapılar (sıra değişmez)

| # | Kapı | Ne olur | Kanıt | Çıkışta |
|---|---|---|---|---|
| 1 | GATE-004a | Belge açılır/üretilir, ölçüm alınır | `dosya | satır | KB | BOM | mojibake` | Sayaçlar yeşil |
| 2 | GATE-003 | Kullanıcı onayı | Tarihli onay metni | **HARD STOP** — onaysız devam yok |
| 3 | GATE-004 | Checklist + çapraz kontrol | §6.1 işaretli madde | Kalite raporu |
| 4 | GATE-005 | K-A/K-B/K-D komutları | Konsol çıktısı | Bozuk link = 0 |
| 5 | GATE-006 | Red Team | Açık RED = 0 | Teslim |

### §5.2 Yazım adımları (yeni kök belge)

| # | Adım | Yapan | Çıktı | Süre |
|---|---|---|---|---|
| 1 | İskeleti §3.6'dan kopyala | docs-writer | 8 alanlı taslak | 2 dk |
| 2 | Bağ satırını §3.2 formatına göre yaz | docs-writer | 9 hedefli tek satır | 2 dk |
| 3 | B2..B8 bloklarını doldur (§3.3) | docs-writer | gövde çekirdeği | 40 dk |
| 4 | KB bandını §3.8 teknikleriyle tamamla | docs-writer | 30–88 KB | 30 dk |
| 5 | §4.2 ret listesini kendin uygula | docs-writer | RED = 0 beyanı | 10 dk |
| 6 | §6.2 ölçümünü çalıştır, raporu yapıştır | reviewer | sayaç satırı | 3 dk |
| 7 | Kapıları §5.1 sırasıyla geçir | reviewer + kullanıcı | onaylı belge | — |

### §5.3 Örnek akış — `AGENTS.md` elden geçirme (disk durumu güncellemesi)

Gerçek tetikleyici: `.agents/` dizinine ajan eklendi/çıkarıldı. Belgenin **içeriği** değil,
**ölçüm iddiaları** değişir.

| # | Ne değişir | Belgedeki iş | Kanıt | Sorumlu |
|---|---|---|---|---|
| 1 | Ajan sayısı | B8 dosya yolları satırındaki sayı | `(Get-ChildItem).Count` | docs-writer |
| 2 | `updated` | ISO tarih, minor sürüm | ilk 8 satır | docs-writer |
| 3 | §2.1 envanter satırı | "Ajan sayısı .agents/ ile eşit" doğrulaması | dosya farkı | reviewer |
| 4 | Bağlantılar | yeni ajan dosyasına link var mı, hedef var mı | `Test-Path` | reviewer |
| 5 | Ölçüm | K-B taraması yeniden: bozuk link 0 | konsol çıktısı | reviewer |

**Yapılmayan iş = RED tetikleyicileri:**

| # | Yapılmayan | Yakalama |
|---|---|---|
| 1 | Sayaç elle yazılmış | yeniden ölçümde çakışır (§4.2/14) |
| 2 | `updated` tazelenmemiş | ISO tarih eskisi (§4.2/2) |
| 3 | Yeni dosyaya link atlanmamış | §2.1 doğrulama satırı RED |
| 4 | Kullanıcı onayı alınmadan GATE-004 | §5.1/2 ihlali |

**Süre:** ≈ 18 dk. **Onay:** reviewer §6.2 çıktısını Quality Report'a yapıştırır.


### §5.4 Sıkıştırma ve düzeltme döngüsü (kapı kırmızıysa)

| # | Kırmızı bulgu | Düzeltme | Yeniden ölçen komut | Süre |
|---|---|---|---|---|
| 1 | KB 30 altı | §3.8/1 karar tablosu eksik bölgeye yazılır | §6.2/1 | 15 dk |
| 2 | KB 88 üstü | §4.3/3 tekrar eden tablolar birleştirilir | §6.2/1 | 20 dk |
| 3 | Bozuk link > 0 | hedef düzeltilir ya da bağlantı kaldırılır | §6.2/4 | 5 dk |
| 4 | Mojibake > 0 | dosya UTF-8 (BOM'suz) yeniden kaydedilir | §6.2/3 | 3 dk |
| 5 | Frontmatter ≠ 8 alan | fazla/eksik alan §3.1'e göre düzeltilir | §3.1 | 2 dk |
| 6 | B2..B8 bloğu eksik | blok içeriği üretilir, sıra korunur | §3.3 | 10 dk |
| 7 | §4.2/5 kopya RED | metin silinir; §1.2'ye göre özet + bağlantı yazılır | gözlem | 10 dk |

**Döngü kuralı:** düzeltmeden sonra **yalnız bozulan ölçümler** yeniden alınır; kapı sırası
(§5.1) baştan çalışmaz — GATE-003 onayı bozulmadıysa korunur. Aynı bulgu üç düzeltmeden sonra
hâlâ kırmızıysa dosya RED ile iade edilir; hiçbir sayı tahmin edilmez (§4.2/14).

**Toplam:** ≈ 65 dk worst-case. Kırmızı bulgu sayısı Quality Report'a `RED = n` satırıyla
yapıştırılır.

**İzleme notu:** her düzeltme satırı Quality Report'ta `madde → komut → yeni sayı` üçlüsüyle
kaydedilir; böylece GATE-005'te K-B çıktısı raporla satır satır eşleşir. Düzeltme yapılan
bölge §6.4'te ayrıca işaretlenir, çünkü yalnız o bölge tekrar ölçülür.

**Kırmızı-tekrar sınırı:** aynı bulgu üç düzeltmeden sonra hâlâ kırmızıysa dosya RED ile
geri döner; dördüncü deneme kapı atlamak anlamına gelir ve GATE-004 kaydı geçersiz sayılır.

---

## §6 Doğrulama

### §6.1 Kontrol checklist'i

- [ ] 1 · Frontmatter **8 alan** + `authority: SSOT` (§3.1 komutu)
- [ ] 2 · Bağ satırı H1'den hemen sonra, `·` ayracılı, hedefler göreli (§3.2)
- [ ] 3 · § başlıkları **§1..§7 sırasıyla**; ölçüm blok içi örnek başlıkları saymaz (kod bloğu durumu izlenerek)
- [ ] 4 · Boyut **30–88 KB** bandında (±%10)
- [ ] 5 · Tek H1; `§` numarasız ara başlık yok
- [ ] 6 · Bozuk iç link yok — her `]\(…\)` hedefi `Test-Path` ile var
- [ ] 7 · EF Core geçmiyor; DevExpress yalnız **v26.1**
- [ ] 8 · `keys.md` §3.7'deki zorlu anahtarların **tamamını** içeriyor
- [ ] 9 · Kanıtsız "tamamlandı" yok — her iddia komut çıktısı ya da `?`
- [ ] 10 · BOM yok (ilk 3 bayt `239,187,191` değil) · mojibake 0
- [ ] 11 · G12: kural metni kopyalanmamış, yalnız özet + bağlantı (§1.2)
- [ ] 12 · B2..B8 blokları eksiksiz ve doğru sırada (§3.3)
- [ ] 13 · Red Team'de açık RED = 0 (§6.3)
- [ ] 14 · `updated` ISO, `version` semver; ikisi de ilk 8 satırda

### §6.2 Ölçüm komutları (salt-okunur, GATE-004a kanıtı)

**1 · Satır ve KB:**

```powershell
$f = 'C:\versacoder\.ai\.templates\documentation\root-md-template.md'
$t = Get-Content -Raw -Encoding UTF8 $f
$lines = ([regex]::Matches($t, [char]10)).Count + 1
$kb = [math]::Round((Get-Item $f).Length / 1KB, 1)
"satır = $lines · KB = $kb"
```

**2 · BOM (ilk üç bayt):**

```powershell
$b = [IO.File]::ReadAllBytes($f)
if ($b[0] -eq 239 -and $b[1] -eq 187 -and $b[2] -eq 191) { 'BOM=VAR -> RET' } else { 'BOM=YOK' }
```

**3 · Mojibake (bozuk kodlama deseni):**

```powershell
$m = ([regex]::Matches($t, '\u00C3.|\u00E2\u20AC')).Count
"mojibake = $m"
```

**4 · Bozuk link (hedef diskte var mı):**

```powershell
$dir = Split-Path $f
$links = [regex]::Matches($t, '\]\(([^)]+)\)') | ForEach-Object { $_.Groups[1].Value }
$bad = $links | Where-Object { $_ -notmatch '^https?://' -and -not (Test-Path (Join-Path $dir $_)) }
"toplam link = $($links.Count) · bozuk = $($bad.Count)"
$bad
```

**5 · Frontmatter alan sayısı** — §3.1'deki komut.

**Rapor formatı** (Quality Report'a yapıştırılacak satır — `?` = hesaplanamıyor, asla
uydurulmaz):

```text
dosya | satır | KB | BOM | mojibake
root-md-template.md | 632 | 30.5 | YOK | 0
```

### §6.3 Red Team

| # | Saldırı | Soru | RED ise |
|---|---|---|---|
| R1 | "KB bandını doldurayım" | Bu bölüm dolgu mu? | §4.3/5 |
| R2 | "Kuralı buraya da yazayım" | Metin `.ai/.rules/` ile çakışıyor mu? | §4.2/5 |
| R3 | "Ölçmeden yeşil yazayım" | Sayı komut çıktısından mı? | §4.2/14 |
| R4 | "Örnek link taramaya girer" | Kaçış kuralı uygulanmış mı? | §4.3/7 |
| R5 | "Blok sırası esnek" | B2..B8 eksiksiz mi? | §4.2/6 |
| R6 | "authority şablonda kalsın" | 8 alan sabit mi, `SSOT` duruyor mu? | §4.2/1–2 |
| R7 | "Eski tarih sorun değil" | `updated` tazelendi mi? | §4.2/2 |
| R8 | "Kırık link sonra bulunur" | `Test-Path` koşuldu mu? | §4.2/4 |

### §6.4 Bölgesel ölçüm (hangi bölüm eksik?)

Toplam yeşilken bile tek bölge kırmızı olabilir; komut § başlıklarını blok durumu izleyerek
böler (§6.1/3):

```powershell
$sec = $null; $cnt = @{}
foreach ($ln in ($t -split [char]10)) {
  if ($ln -match '^## §(\d)') { $sec = $Matches[1]; $cnt[$sec] = 0 }
  elseif ($sec -and $cnt.ContainsKey($sec)) { $cnt[$sec]++ }
}
$cnt.GetEnumerator() | Sort-Object Name | Format-Table -AutoSize
```

**Bölge alt sınırları** (özgün ve bağlayıcı tablo [index.md](../index.md) §3.7'dedir; burası
uygulama özeti):

| Bölge | Min satır | Min KB | Eksikse |
|---|---|---|---|
| §1 Amaç | 40 | 2.0 | Amaç + G12 özeti yetersiz |
| §2 Kapsam | 45 | 2.0 | Envanter/kapsam dışı eksik |
| §3 Mimari | 150 | 9.0 | İskelet/B blokları noksan |
| §4 Kurallar | 70 | 4.0 | Ret gerekçeleri eksik |
| §5 Workflow | 60 | 4.0 | Kapı akışı eksik |
| §6 Doğrulama | 70 | 4.0 | Komut/checklist eksik |
| §7 Referanslar | 45 | 2.0 | Kaynak/çapraz link eksik |
| **Toplam** | **480** | **26.0** | GATE-004a RED |

---

## §7 Referanslar

### §7.1 Kaynaklar

| # | Kaynak | Yol |
|---|---|---|
| 1 | Genel doküman şablonu | `.ai/.templates/documentation/docs-md-template.md` |
| 2 | Şablon envanteri | `.ai/.templates/index.md` |
| 3 | Ölçüm kanıtı | `.ai/reports/00-kapsam-analizi.md` |
| 4 | Bootstrap kaynağı | `G:\…\Versacoder\versacoder proje planaması ai.03.md` |
| 5 | Anayasa (B1..B10, G1..G12) | `CLAUDE.md` |
| 6 | Kapılar (GATE-004a..006) | `WORKFLOW.md` |
| 7 | Kardeş şablon (README) | `.ai/.templates/documentation/readme-template.md` |
| 8 | Blok tanımları (B2–B8 ayrıntısı) | `.ai/.templates/documentation/claude-md-template.md` |

### §7.2 Çapraz bağlantılar (hepsi diskte mevcut)

| # | Bağlantı | Ne için |
|---|---|---|
| 1 | [index.md](../index.md) | Şablon envanteri + §3.7 kalite sözleşmesi |
| 2 | [CLAUDE.md](../../../CLAUDE.md) | B1..B10 blokları, G1..G12, okuma sırası |
| 3 | [docs-md-template.md](./docs-md-template.md) | Gövde belgeleri için genel şablon |
| 4 | [readme-template.md](./readme-template.md) | Proje README iskeleti (§1..§7) |
| 5 | [claude-md-template.md](./claude-md-template.md) | Anayasa şablonu, blok sırası |
| 6 | [WORKFLOW.md](../../../WORKFLOW.md) | Kapı tanımları ve kanıt sütunu |
| 7 | [README.md](../../../README.md) | Depo kökü girişi |
| 8 | [api-doc-template.md](./api-doc-template.md) | API belgesi şablonu |
| 9 | [katman-readme-template.md](./katman-readme-template.md) | Katman README'leri |

### §7.3 Kullanım notu

Bu şablonu **kopyalayarak** yeni kök belge üret: §3.6 iskeletini al, §3.1–§3.2'yi byte-byte
uygula, B2–B8'i (§3.3) doldur, KB bandını §3.8 ile tamamla, §4.2 listesinden geç, §6.2
komutlarını çalıştır, kapıları §5.1 sırasıyla geçir. Şablonun **kendi** metni hiçbir kök
belgeye gömülmez — o da G12'dir.


### §7.4 Ölçüm kaydı (bu dosya · GATE-004a çıktısı)

| dosya | satır | KB | BOM | mojibake |
|---|---|---|---|---|
| root-md-template.md | 632 | 30.5 | YOK | 0 |
| readme-template.md | 682 | 32.0 | YOK | 0 |

**Kayıt kuralı:** sayılar §6.2 komutundan gelir; hesaplanamayan hücre `?` yazılır, tahmin
edilmez. Bu tablo her güncellemeden sonra yeniden ölçülür; değişiklik varsa §6.1/10 maddesi
yeniden işaretlenir. Bölgesel alt sınırlar §6.4 tablosunda, bağlayıcı özgün tablo
[index.md](../index.md) §3.7'dedir.
