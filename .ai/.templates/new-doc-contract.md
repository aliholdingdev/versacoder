---
title: "Yeni Vault Dosyasi Sozlesmesi (batch uretim kurallari)"
type: template
category: templates
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
scope: "GATE-004a icin uretilen tum yeni MD dosyalarinin frontmatter, link, boyut ve yasak kurallari - ihlal GATE-004a'yi kapatir"
---

# Yeni Vault Dosyasi Sozlesmesi

**Zorunlu Baglantilar:** [index](../index.md) · [root-md-template](documentation/root-md-template.md) · [docs-md-template](documentation/docs-md-template.md) · [ADR-003](../.decisions/accepted/ADR-003-vault-structure-500md-frontmatter.md) · [CLAUDE.md](../CLAUDE.md)

> **Tek cumlelik sorumluluk:** *Yeni uretilen her MD dosyasi hangi kurala uyar* sorusunun tek cevabidir.
> Bu dosyayi okumayan batch uretimi RED edilir (`reviewer`).

---

## §0 Ozet (Answer First)

Her yeni dosya: **UTF-8** · **ilk satir `---`** · **YAML frontmatter 8 alan** · **3-8 KB** · **sadece listelenen hedeflere link** · **Turkce** · **frontmatter'siz / bozuk link / EF Core / string-concat SQL yok**.

---

## §1 Zorunlu Frontmatter (ilk satir `---`)

```yaml
---
title: "<Dosya adi - VersaCoder>"
type: knowledge | spec | rules | adr | checklist | prompt | playbook | report | architecture
category: <ayni klasor adi>
status: active
version: 1.0.0
updated: <YYYY-MM-DD - dosyanin uretilip guncellendigi tarih>
authority: SSOT
scope: "<1 satir kapsam - ne cevaplar>"
---
```

`type`, `category` ve `updated` disindaki alanlar **birebir** boyle yazilir. `updated`, dosyanin uretilip guncellendigi **gercek tarihtir** (eski bir tarih kopyalanmaz — kopyalanan tarih her yeni dosyayi yanlis gosterir).

## §2 Zorunlu govde iskeleti

```markdown
# <H1 - frontmatter title ile ayni anlama>

**Zorunlu Baglantilar:** [index](<goreli yol>) · [<ilgili>](<goreli yol>)

> **Tek cumlelik sorumluluk:** <bu dosyanin tek cevabi>

---

## §0 Ozet (Answer First)

<3-5 cumle: cevap once, madde sonra>

---

## §1 ...
```

- Bolum basliklari `## §N <ad>` seklinde (§0 zorunlu).
- En az 3 tablo **veya** 1 tablo + 1 ASCII kutu diyagrami (`|`, `-`, `+` ile — Unicode kutu cizimi `U+2500-U+257F` YASAKTIR, CLAUDE G4).
- Madde listeleri **en fazla 5 madde** (ADHD cikti kurali).

## §3 Link kurallari (BOZUK LINK = GATE-004a KIRMIZI)

**SADECE** asagidaki hedeflere goreli link yaz. Baska dosya/link URETME.

| Dosyanin bulundugu klasor | Izinli goreli hedefler |
|---|---|
| `.ai/knowledge/*` | `../../index.md` · `../../CLAUDE.md` · `../../keys.md` · `../../VISION.md` · `../../.rules/devexpress-rules.md` · `../../.rules/data-access-rules.md` · `../../.rules/security-rules.md` · `../../.rules/coding-standards.md` · `../../.rules/testing-rules.md` · `../../.decisions/accepted/ADR-004-devexpress-universal-v261-office2010.md` · `../../.decisions/accepted/ADR-005-ado-net-dapper-no-ef-core.md` · `../../.decisions/accepted/ADR-012-dual-tfm-data-provider.md` · `../index.md` · ayni klasordeki kardes dosyalar (adini tam yaz) |
| `.ai/checklists` · `.ai/prompts` · `.ai/playbooks` | `../index.md` · `../CLAUDE.md` · `../keys.md` · `../WORKFLOW.md` · `../spec/test-strategy.md` · `../.rules/coding-standards.md` |
| `.ai/spec` | `../index.md` · `../CLAUDE.md` · `../keys.md` · `../WORKFLOW.md` · `../.rules/testing-rules.md` · `../architecture/index.md` · ayni klasordeki kardes dosyalar |
| `.ai/.rules` | `../index.md` · `../CLAUDE.md` · `../keys.md` · `../spec/test-strategy.md` · ayni klasordeki kardes dosyalar |
| `.ai/.decisions/accepted` | `../index.md` · `../adr-index.md` · `../../CLAUDE.md` · `../../keys.md` · `accepted/` altindaki kardes ADR'ler · `../../spec/test-strategy.md` · `../../reports/01-web-kaynaklari.md` |
| `.ai/architecture/l*` | `../index.md` · `../00-overview/dependency-map.md` · `../00-overview/system-overview.md` · `../../CLAUDE.md` · `../../keys.md` · `../catalog/<katman>/<node>.md` · `../../.decisions/accepted/ADR-013-layered-architecture-52-nodes.md` · `../../reports/00-kapsam-analizi.md` |
| `.ai/reports` | `../index.md` · `../CLAUDE.md` · `../keys.md` · `../WORKFLOW.md` · `../VISION.md` · baska report dosyalari |

**Mutlak yasak:** dis URL `http(s)://` (harici link sayilmaz, bozuk link uretmez - ama az kullan), `mailto:`, var olmayan dosya adi, `#` anchor disinda bos link.

## §4 Icerik kurallari (yasaklar)

1. **EF Core yok.** Veri erisimi = **ADO.NET + Dapper**, her sorgu **parameterized** (`@p1`), asla string concat. Migration = saf SQL + version tablosu.
2. **DevExpress = Universal 2026 / v26.1** tek surum. Ribbon stili **yalniz** `RibbonControlStyle.Office2010`.
3. **Ikili bacak:** `net48` + `net10.0-windows`. TFM gerekceleri belirtilir; `#if` yalnizca platform farkinda.
4. **Kanit zorunlu:** "bitti/tamam" iddiasi build/test ciktisiz yasak. Bilinmeyen sey `NEEDS_VALIDATION` olarak isaretlenir.
5. **Kopyalama yok (ADR-002):** baska vault'tan (CoreMusic, versacoder.ai) metin kopyalanmaz; yalnizca bilgi olarak faydanilir.

## §5 Kodlama

- **UTF-8** yaz (bozuk encoding `C3 B6`-`C3 A7` gibi iki bytelik ASCII artiklarina dusmus metin YASAK - olcersen duzelt).
- Dosya adi `kebab-case.md`, ASCII (Turkce karakter yok dosya adinda).
- Satir sonu LF/CR-LF farketmez; yalniz CR-only yasak.

## §6 Self-check (batch biterken)

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File C:\versacoder\.ai\scripts\check-links.ps1    # BOZUK link : 0 hedefi
powershell -NoProfile -ExecutionPolicy Bypass -File C:\versacoder\.ai\scripts\measure-vault.ps1  # NoFrontmatter = 0, Mojibake = 0
```

> `scripts/` (AŞAMA C) diskte yoksa §6 betikleri çalışmaz — §12'deki betiksiz bloklar aynı
> sayıyı üretir. Bu belgeye **eklenen** hiçbir bağlantı diskte olmayan hedefe gitmez (G7 · §8).

---

## §7 Hedef Klasörlere Göre Üretim Rehberi

§1–§6 her dosyaya uygulanır; buradaki tablolar **klasöre özgü** ek kuralları verir.
"Şablon" sütunundaki ad `.templates/` altındaki gerçek dosyadır; yeni klasör açmak
bu sözleşmenin dışındadır — klasör ADR ile açılır (G9).

### §7.1 Tüm Hedefler İçin Ortak Adım Tablosu

| # | Adım | Ne yapılır | Kanıt | İlgili |
|---|---|---|---|---|
| 1 | Şablon seç | §7.2–§7.6'daki şablon adı okunur | iskelet dosyası açık | §7 |
| 2 | Frontmatter aç | 8 alan, `updated` üretim gününün gerçek tarihi | §9 D1–D6 | §1 |
| 3 | İskeleti kur | H1 + Zorunlu Bağlantılar + tek cümle + §0 | §2 kalıbı | §2 |
| 4 | İçeriği yaz | en az 1 tablo · madde ≤ 5 · kanıt satırı | tablo/komut çıktısı | §2, §4.4 |
| 5 | Link yaz | yalnız §3 satırı + §8.2 beyaz listesi | kırık = 0 | §3, §8 |
| 6 | Ölç ve raporla | 3-8 KB (§10) + §12 komut çıktısı | Quality Report | §10, §12 |

- Kardeş dosyaya linkte **dosya adı tam yazılır**: `../index.md` doğru, `../inde…` kısaltma yasak.
- Aynı konu iki dosyada yaşarsa (kural metni + şablon tekrarı) **ikinci dosya red** edilir (G12, R14).

### §7.2 `.ai/.decisions/` — ADR Dosyaları

| # | Kural | Değer |
|---|---|---|
| 1 | Dosya adı | `ADR-NNN-<kebab-baslik>.md` — NNN 3 hane, boşluksuz, `ADR-005-ado-net-dapper-no-ef-core.md` gibi |
| 2 | Şablon | [adr/adr-template.md](adr/adr-template.md); varyantlar: nygard · architecture · data · security · devexpress (aynı klasörde) |
| 3 | `type` / `category` | `adr` / `decisions` |
| 4 | Zorunlu bölümler | Durum · Bağlam · Karar · Sonuçlar (olumlu / olumsuz / nötr) · Alternatifler |
| 5 | Boyut | 3-8 KB (§10.1 sınıf 1); karar metni kendi dosyasında yaşar, başka dosyaya **kopyalanmaz** |

- **Karar tekrarı yasak:** aynı mimari karar başka dosyada da anlatılırsa ikinci SSOT doğar →
  düzeltme kararın tek dosyada toplanmasıdır; diğer dosya yalnız ADR'ye **link verir**.
- ADR numarası boşluk üretmez: `001..030` planlı aralıktır; yeni numara diskteki adr kaydından
  sonra verilir — numara uydurmak `R15` (kanıtsız iddia) sayılır.
- `.ai/.decisions/` diskte henüz yok (§8.1 satır 4); hedef üretilmeden bu klasöre **link yazılmaz**,
  yol düz metin olarak kalır.

### §7.3 `.ai/architecture/l*` — Katman Dosyaları

| # | Kural | Değer |
|---|---|---|
| 1 | Dosya adı | `l<0..7>-<konu>.md` — katman kodu `l0..l7` birebir, uydurma numara yasak |
| 2 | Şablon | [architecture/layer-spec-template.md](architecture/layer-spec-template.md) · [architecture/dependency-rule-template.md](architecture/dependency-rule-template.md) · [architecture/ascii-diagram-template.md](architecture/ascii-diagram-template.md) |
| 3 | `type` / `category` | `architecture` / `architecture` |
| 4 | Zorunlu içerik | sorumluluk · giden bağımlılıklar · portlar · TFM notu · test kanıtı satırı |
| 5 | Diyagram | ASCII kutu (`|`, `-`, `+`); Unicode `U+2500-U+257F` yasak (§2) |

**Yön tablosu** her katman dosyasında aynen bulunur:

| Ok | Anlamı | İhlali |
|---|---|---|
| `l7 -> l6 -> l5 -> l4 -> l2 -> l1 -> l0` | izinli referans yönü | yukarı bakan ok = mimari ihlal |
| `l3` yalnız `l0`'a bakar | crosscutting serbest değil | `l3 -> l4` yazımı red |
| `l2` ile `l4` arasında doğrudan ok **yok** | port `l0`'da, uygulama `l4`'te | doğrudan bağlantı red |

- `src/` diskte yokken "kodda doğrulandı" iddiası yazılmaz; gerekli yer `NEEDS_VALIDATION`
  olarak işaretlenir (§4.4, G9).

### §7.4 `.ai/reports/` — Rapor Dosyaları

| # | Kural | Değer |
|---|---|---|
| 1 | Dosya adı | `NN-<konu>.md` (2 hane sırayla) veya konu öneki: `security-audit-*.md`, `test-plan-*.md` |
| 2 | Şablon | [documentation/security-audit-template.md](documentation/security-audit-template.md) · [testing/test-plan-template.md](testing/test-plan-template.md) |
| 3 | `type` / `category` | `report` / `reports` |
| 4 | Zorunlu içerik | kapsam · yöntem · ölçüm tablosu · bulgular · sonuç (§2 iskeleti) |
| 5 | Boyut | 3-8 KB (§10); seri kanıtları (PART 4/5/6/7/8) **her kanıt ayrı raporda** toplanır |

- **Sayı kanıtsız yazılmaz:** "553 MD" gibi bir sayı, onu üreten komut çıktısı ile birlikte
  yazılır; komut çalıştırılamadıysa sayı değil `NEEDS_VALIDATION` kalır (G9).
- Başka vault'tan rapor metni taşınmaz; yalnız **yöntem fikri** alınır (G1, R14).

### §7.5 `.ai/prompts/` — Prompt Dosyaları

| # | Kural | Değer |
|---|---|---|
| 1 | Dosya adı | `NN-<rol-veya-konu>-v<N>.md` — diskteki örnek adlar: `01-master-enterprise-engineering-v2.md`, `02-master-project-installation-v1.md` |
| 2 | Şablon | ekran promptu: [ui-design/prompt-template.md](ui-design/prompt-template.md); rol promptu: §2 iskeleti |
| 3 | `type` / `category` | `prompt` / `prompts` |
| 4 | Zorunlu içerik | rol · görev · çıktı biçimi · yasaklar · örnek çıktı satırı |
| 5 | Boyut | 3-8 KB (§10.1 sınıf 1) — G6'nın 480 satır eşiği `.templates/` dışına taşmaz |

- **Prompt ≠ kural:** kural `.ai/.rules/`'ta, prompt `.ai/prompts/`'ta yaşar; aynı cümle iki
  yerde = G12 ihlali (R14).
- Prompt içine **anahtar/sır yazılmaz**; yalnız ortam değişkeninin **adı** geçer — bkz
  [llm-provider-template.md](llm-provider-template.md) §2 kural 1 (G12 güvenlik).

### §7.6 `.ai/.templates/` — Şablon Dosyaları

| # | Kural | Değer |
|---|---|---|
| 1 | Dosya adı | `<konu>-template.md`, ASCII kebab-case; `Query-Template.md` mevcut istisnadır, yeniden adlandırılmaz |
| 2 | `type` / `category` | `template` / `templates` |
| 3 | İskelet | [documentation/root-md-template.md](documentation/root-md-template.md) · [documentation/docs-md-template.md](documentation/docs-md-template.md) |
| 4 | Boyut | **≥ 480 satır · ≥ 27 KB** (G6 · envanter §3.7 K14/K15) — §10.1 sınıf 2 |
| 5 | Tek muaf | `.templates/CLAUDE.md` → 90–140 satır / 5–8 KB (G6 tek muafu) |

- Şablon dosyasında iskelet gereği çift süslü parantezli yer tutucu **yaşayabilir**; bunu 0'a indirmek yalnız
  **şablondan üretilen gerçek dosyada** yapılır (G4 · envanter §4.5 G4 satırı).
- Şablon bittikten sonra envanter satırı **tek geçişte** `PLANLI` → `YAZILDI` olur; şablon
  yazılırken envanter değiştirilmez (çift kayıt = envanter hatalı sayım).

---

## §8 Link Beyaz Listesi Genişletme (§3'ün Açılımı)

§3'teki tablo **değiştirilmez**; bu bölüm yalnız hangi hedefin diskte gerçekten var olduğunu
ve `.templates/` içinden hangi **yeni** linkin eklenebileceğini söyler.

### §8.1 Disk Durumu (ölçüm 2026-10-06)

| # | Hedef sınıfı | Diskte | `.templates/` içinden yeni link |
|---|---|---|---|
| 1 | `.ai/.templates/**` (envantere göre 50 MD) | ✅ var | **serbest** — §8.2 |
| 2 | `.ai/prompts/` (2 MD) · `.ai/reports/` (1 MD) | ✅ var | ⛔ link değil, düz yol metni (dizin kuralı §3.11) |
| 3 | Depo kökü `CLAUDE.md` · `AGENTS.md` · `README.md` · `WORKFLOW.md` | ✅ var | **serbest** — `../../` öneki |
| 4 | `.ai/index.md` · `.ai/CLAUDE.md` · `.ai/.rules/` · `.ai/.decisions/` · `.ai/architecture/` · `.ai/checklists/` · `.ai/scripts/` | ❌ YOK (AŞAMA C) | ⛔ **yeni link yok** — hedef üretilmeden yazılmaz (G7) |

> **Mevcut link dokunulmazdır:** §1–§6'daki eski bağlantılar (bkz belge başlığı) değiştirilmez.
> Bunların bir kısmı hâlihazırda AŞAMA C hedefine gider; bu, **yeni** linkin de kırık olmasına
> gerekçe olmaz — yeni link yalnız ✅ satırlarına yazılır. Kasıtlı eski kırık hedef
> envanter [index.md](index.md) §1.2'de de tek tek listelenmiştir.

### §8.2 Genişletilmiş İzinli Bağlantı Biçimleri

| # | Biçim | Hedef (diskte doğrulanmış) | Ne için |
|---|---|---|---|
| 1 | `index.md` | `.ai/.templates/index.md` | envanter, kalite sözleşmesi §3.7 |
| 2 | `CLAUDE.md` | `.ai/.templates/CLAUDE.md` | dizin kuralı, okuma sırası |
| 3 | `<alt-klasor>/<sablon>.md` | `.ai/.templates/<alt>/…` (adr, architecture, backend, documentation, frontend, infrastructure, other, query, testing, ui-design) | görev iskeleti |
| 4 | `../../CLAUDE.md` · `../../AGENTS.md` · `../../README.md` · `../../WORKFLOW.md` | depo kökü 4 dosya | rol · kapı · tanım |
| 5 | kardeş dosya (tam ad) | `.ai/.templates/` kök 9 dosya | sözleşme, manifesto, şablon |
| 6 | `<hedef>#<çapa>` yerine ayrı satır | — | çapa, bağlantı **dışında** düz metin yazılır |

### §8.3 Bağlantı Yazım Doğrusu / Yanlışı

| # | Doğru | Yanlış | Neden |
|---|---|---|---|
| 1 | `](adr/adr-template.md)` | `](adr\adr-template.md)` | `/ ayracı taşınabilir |
| 2 | `](../index.md)` — tam ad | `](../index)` — uzantısız | hedef dosya `.md` |
| 3 | `](../../CLAUDE.md)` | `](../CLAUDE.md)` — `.ai/` içine | `.ai/CLAUDE.md` diskte yok (§8.1/4) |
| 4 | `[metin](index.md)` | `[](index.md)` — boş metin | okunabilirlik |
| 5 | `https://…` yalnız kaynak satırında | bağlantı satırının tamamı dış URL | iç navigasyon çöker |

### §8.4 Betiksiz Bağlantı Ayrıştırma Mantığı

`scripts/check-links.ps1` yoksa aynı sonucu üreten 5 adım (uygulama §12/3):

| # | Adım | Mantık |
|---|---|---|
| 1 | Metni çek | `](…)` kalıbıyla bütün hedefler |
| 2 | Ayıkla | `http(s)://`, `mailto:` ve `#` ile başlayan çapalar hariç |
| 3 | Benzersizleştir | aynı hedef iki kez tek sayılır |
| 4 | Sına | hedefin dosyadan göreli konumuna göre `Test-Path` |
| 5 | Raporla | kırık varsa dosya adı + hedef listelenir; yeni eklenen linkte kırık = 0 |

---

## §9 Frontmatter Alan Alan Tanım (§1'in Açılımı)

§1'deki blok **değiştirilmez**; bu tablo yalnız yorumlamayı netleştirir.

| # | Alan | Kural | Doğru örnek | Red örnek |
|---|---|---|---|---|
| 1 | `title` | `"` içinde Türkçe; H1 ile **aynı anlam** | `"Yeni Vault Dosyasi Sozlesmesi"` | `ADR 005` (tırnaksız) · `My Notes` |
| 2 | `type` | §1'deki 9 değerden biri — başka değer bu sözleşmede yok | `adr` | `note` · `doc` · boş |
| 3 | `category` | dosyanın bulunduğu klasörün adı | `templates` | `docs` (klasör adı değil) |
| 4 | `status` | `active`; `WIP` / `~` / boş yasak | `active` | `wip` · `taslak` |
| 5 | `version` | üç bölüm `major.minor.patch` | `1.0.0` | `1.0` · `v1` · `1.0.0.0` |
| 6 | `updated` | **gerçek üretim tarihi**, ISO `YYYY-MM-DD` | `2026-10-06` | eski dosyanın tarihinin kopyası · `06.10.2026` |
| 7 | `authority` | her zaman `SSOT` | `SSOT` | `primary` · `draft` |
| 8 | `scope` | `"` içinde tek satır "ne cevaplar"; H1'i tekrarlamaz | `"GATE-004a batch kapsamı"` | boş · 3 satırlık paragraf |

### §9.1 Denetim Kuralları (D1–D6)

| # | Denetim | Beklenen | İhlali |
|---|---|---|---|
| D1 | Dosyanın **ilk satırı** `---`, frontmatter ikinci `---` ile kapanır | 2 ayraç | R1 |
| D2 | Alan sayısı tam 8 — fazla alan da eksik alan da red | 8/8 | R1 |
| D3 | Sıra: `title,type,category,status,version,updated,authority,scope` | 8 sıra | R2 |
| D4 | `updated` üretim günüyle aynı gün | ISO bugün | R3 |
| D5 | `scope` tek satır, `"` içinde | 1 satır | R4 |
| D6 | Değişiklikte sürüm: bölüm `minor` · düzeltme `patch` | üç bölüm | R5 |

> **İstisna yok ve alan eklenmez:** `type`/`category`/`updated` dışındaki5 alan §1'deki değerle
> birebir yazılır. 9. alan (ör. `tags`) bu sözleşmeye göre redtir — envanter de 8 alan bekler.

---

## §10 Boyut Sözleşmesi: 3-8 KB ve İstisnaları

§0'daki **3-8 KB** kuralı **değiştirilmez**; yalnız kapsamı ve istisnaları burada netleşir.

### §10.1 Kapsam Matrisi

| # | Dosya sınıfı | Satır | KB | Kural kaynağı |
|---|---|---|---|---|
| 1 | Yeni vault MD (`knowledge/`, `spec/`, `checklists/`, `playbooks/`, `.rules/`, `reports/`, `prompts/` …) | — | **3-8** | bu sözleşme §0 |
| 2 | `.templates/**` şablon dosyaları | **≥ 480** | **≥ 27** | G6 · envanter §3.7 (K14/K15) |
| 3 | `.templates/CLAUDE.md` — tek muaf | 90–140 | 5–8 | G6 tek muaf satırı |
| 4 | `.templates/index.md` — envanter | 520–680 | 26–44 | envanterin kendi sözleşmesi |
| 5 | `.templates/` kök sözleşme/manifesto (bu 3 dosya dâhil) | **≥ 480** | **≥ 26** | G6 + K15 kalite sözleşmesi |

### §10.2 3 KB Altına Düşerse

| # | Durum | Aksiyon |
|---|---|---|
| 1 | Konu gerçekten dar (tek kural, tek komut) | kardeş konuyla **birleştir** — tek dosya 3 KB'ı geçsin |
| 2 | Konu geniş ama örnek az | §0–§6 iskeletine 2. dolu örnek + ölçüm tablosu ekle |
| 3 | Konu henüz kanıtsız | yine de 3 KB altı yazılmaz: kapsamı büyüt ve `NEEDS_VALIDATION` bölümü aç |
| 4 | Konu başka dosyada zaten var | yeni dosya **açma** — kardeş dosyaya bölüm ekle (G12) |

### §10.3 8 KB'ı Aşarsa

| # | Durum | Aksiyon |
|---|---|---|
| 1 | Dosya iki ayrı soru cevaplıyorsa | böl; her parçada §0 tek cümle + kardeşe link |
| 2 | Tek soru, seri kanıt (PART 4/5/6/7/8) | seriye böl: `NN-<konu>.md`, toplamı `reports` kaydında |
| 3 | Art arda 6+ tablo bloğu | en az 2 bloğu kardeş dosyaya taşı, kalan §'yi linkle |
| 4 | Şablon dosyası (sınıf 2–5) | 8 KB üstü **zorunlu**; buradaki bölme kuralı şablona uygulanmaz |

### §10.4 Ölçüm Yorumu

| # | Kural | Ayrıntı |
|---|---|---|
| 1 | KB = bayt / 1024 | `Get-Item .Length / 1KB` |
| 2 | Türkçe UTF-8 harf 2 bayt | 8 KB ≈ 4.500–5.000 karakter; "az görünüyor" yanılgısına düşülmez |
| 3 | Sınır dâhildir | tam 8,0 KB geçer · 8,1 KB §10.3 · tam 3,0 KB geçer · 2,9 KB §10.2 |
| 4 | Ölçüm yapıştırılmaz | KB değeri tek başına değil, satır + link + mojibake sayacıyla raporlanır |

---

## §11 Red Nedenleri (R1–R15)

`reviewer` sırayla bakar; **ilk tutan** neden red gerekçesidir — aynı dosyada ikinci neden aranmaz.

| # | Red nedeni | Yakalama | Düzeltme | Süre |
|---|---|---|---|---|
| R1 | Frontmatter yok / `---` kapanmıyor / alan ≠ 8 | §9.1 D1–D2 | bloğu §1'e göre yeniden yaz | 5 dk |
| R2 | Alan sırası bozuk ya da `authority` ≠ `SSOT` | §9.1 D3 | 5 alan birebir düzelt | 3 dk |
| R3 | `updated` kopyalanmış eski tarih | §9.1 D4 | bugünün ISO tarihini yaz | 1 dk |
| R4 | `scope` çok satırlı ya da H1'i tekrarlıyor | §9.1 D5 | tek cümle "ne cevaplar" | 3 dk |
| R5 | `version` iki ya da dört bölüm | §9.1 D6 | `major.minor.patch` | 1 dk |
| R6 | H1 yok · Zorunlu Bağlantılar yok · §0 yok | §2 iskeleti | iskeleti tamamla | 5 dk |
| R7 | Tablo yok ve ASCII kutu yok (0/0) | §2 | ölçüm tablosu ekle | 8 dk |
| R8 | Madde listesi 5 maddeyi aşıyor | §2 (ADHD kuralı) | 5 maddeye indir, kalanı tabloya | 5 dk |
| R9 | Kırık link > 0 (yeni eklenen) | §8.4 · §12/3 | hedefi üret ya da linki kaldır | 10 dk |
| R10 | Yeni link `.ai/*` YOK hedefe (§8.1 satır 4) | §8.1 | bağlantıyı kaldır, yol metnine çevir | 5 dk |
| R11 | EF Core · string-concat SQL · `@pN`'siz sorgu | §4.1 | ADO.NET + Dapper'a çevir | 20 dk |
| R12 | DevExpress ≠ 26.1 ya da ribbon ≠ Office2010 | §4.2 | karar dosyasına göre düzelt | 10 dk |
| R13 | Mojibake · BOM · CR-only · Türkçe dosya adı | §5 · §12/1 · §12/4 | UTF-8 yeniden yaz, adı kebab-case | 10 dk |
| R14 | Başka vault'tan kopya cümle / içerik tekrarı (G1 · G12) | §4.5 | sil, özgün yaz | 15 dk |
| R15 | "bitti" iddiası komut çıktısız / kanıtsız sayı | §4.4 (G9) | komut çıktısı ekle ya da `NEEDS_VALIDATION` bırak | 10 dk |

**Kırmızı akışı:** 3 red sonrası dosya sahibine döner ve iş durur; gerekçe tek satır olarak
`.ai/log.md`'ye yazılır (bu dosya diskte yoksa — AŞAMA C — not bekler, tahmin yazılmaz).

---

## §12 Doğrulama Komutları (§6 Tamamlayıcı, Betiksiz)

§6 betikleri `scripts/` ister; **betik yoksa** aşağıdaki salt okunur bloklar aynı sayıyı üretir.
Hiçbiri dosyayı değiştirmez; beklentiler bu belgenin kendi sınıfıdır (§10.1 satır 5).

### §12/1 Satır · KB · BOM

```powershell
$d = 'C:\versacoder\.ai\.templates\new-doc-contract.md'
$b = [System.IO.File]::ReadAllBytes($d)
'{0} satır · {1:N1} KB · BOM={2}' -f (Get-Content $d -Encoding UTF8).Count, `
  ((Get-Item $d).Length / 1KB), ($b[0] -eq 239 -and $b[1] -eq 187 -and $b[2] -eq 191)
# hedef (sınıf 5): satır >= 480 · KB >= 26 · BOM = False
```

### §12/2 Frontmatter 8 Alan

```powershell
$d = 'C:\versacoder\.ai\.templates\new-doc-contract.md'
$fm = Get-Content $d -Encoding UTF8 -TotalCount 12
$alan  = $fm | Where-Object { $_ -match '^(title|type|category|status|version|updated|authority|scope):' }
$ayrac = @($fm | Where-Object { $_ -eq '---' }).Count
'alan {0}/8 · ayraç {1}/2 · ilk satir --- = {2}' -f $alan.Count, $ayrac, ($fm[0] -eq '---')
# hedef: alan 8 · ayraç 2 · True
```

### §12/3 Kırık Bağlantı (betiksiz)

```powershell
$d = 'C:\versacoder\.ai\.templates\new-doc-contract.md'; $base = Split-Path $d
$hedefler = [regex]::Matches((Get-Content $d -Raw -Encoding UTF8), '\]\(([^)#]+)\)') |
  ForEach-Object { $_.Groups[1].Value } |
  Where-Object { $_ -notmatch '^(https?:|mailto:)' } | Sort-Object -Unique
$kirik = @($hedefler | Where-Object { -not (Test-Path (Join-Path $base $_)) })
'toplam {0} · kirik {1}' -f $hedefler.Count, $kirik.Count
$kirik
# hedef: bu belgede YENI eklenen linkte kirik = 0
# ilk çalıştırma eski (korunan) AŞAMA C hedeflerini gösterebilir — bkz §8.1 satır 4
```

### §12/4 Mojibake · Yer Tutucu · Türkçe Karakter Bütünlüğü

```powershell
$d = 'C:\versacoder\.ai\.templates\new-doc-contract.md'
$r = Get-Content $d -Raw -Encoding UTF8
$moji = [regex]::Matches($r, '\u00C3.|\u00E2\u20AC').Count      # G5 — bozuk çift bayt
$yer  = [regex]::Matches($r, '\x7B\x7B[^\x7D]+\x7D\x7D').Count  # G4 — kalinip disi yer tutucu
$tru  = [regex]::Matches($r, '[\u015E\u011E\u0130\u00C7\u00D6\u00DC]').Count  # dogru Turkce harf sayaci
'mojibake {0} · yer-tutucu {1} · Turkce-harf {2}' -f $moji, $yer, $tru
# hedef: mojibake 0 · yer-tutucu 0 · Turkce-harf > 0 (G5)
```

### §12/5 Bu Üç Dosyanın Toplu Geçişi

```powershell
$klasor = 'C:\versacoder\.ai\.templates'
@('new-doc-contract.md','batch-manifest-gate004a.md','llm-provider-template.md') |
  ForEach-Object {
    $p = Join-Path $klasor $_
    $b = [System.IO.File]::ReadAllBytes($p)
    $r = Get-Content $p -Raw -Encoding UTF8
    [PSCustomObject]@{
      Dosya    = $_
      Satir    = (Get-Content $p -Encoding UTF8).Count
      KB       = [math]::Round((Get-Item $p).Length / 1KB, 1)
      BOM      = ($b[0] -eq 239 -and $b[1] -eq 187 -and $b[2] -eq 191)
      Mojibake = [regex]::Matches($r, '\u00C3.|\u00E2\u20AC').Count
    }
  } | Format-Table -AutoSize
# hedef: her satir >= 480 · KB >= 26 · BOM False · Mojibake 0
```

### §12/6 Quality Report Kalıbı

```markdown
## Quality Report — .templates/<dosya> (YYYY-MM-DD)
Ölçüm: <satır> satır / <KB> KB (≥480 / ≥26) · frontmatter 8/8 · kırık link (yeni) 0
Mojibake 0 · BOM yok · yer tutucu 0 · §11 red sinyali 0 · G1..G12 ihlali yok
Karar: ONAY | RED (<gerekçe>) · Onay: docs-writer -> reviewer
```

---

## §13 Uçtan Uca Üretim Senaryoları

Üç tipik iş, aynı sözleşmenin farklı kapılarından geçer. Adımlar **sırayla** uygulanır;
atlanan adım §11'deki red nedeniyle geri döner.

### §13.1 Senaryo A — Yeni ADR (`.ai/.decisions/`)

| # | Adım | Çıktı | Kapı |
|---|---|---|---|
| 1 | Numara ve konu belirle | `ADR-NNN-<konu>` — aralık `001..030` | §7.2 |
| 2 | Şablon aç | [adr/adr-template.md](adr/adr-template.md) iskeleti | §7.2 satır 2 |
| 3 | Durum / Bağlam / Karar / Sonuçlar / Alternatifler doldur | 5 bölüm eksiksiz | §7.2 satır 4 |
| 4 | Frontmatter 8 alan | `type: adr`, `updated` bugün | §9 |
| 5 | Link denetimi | yalnız §3 + §8.2 hedefleri | §12/3 |
| 6 | Ölçüm + rapor | 3-8 KB · §12/6 Quality Report | §10.1 sınıf 1 |

### §13.2 Senaryo B — Yeni Rapor (`.ai/reports/`)

| # | Adım | Çıktı | Kapı |
|---|---|---|---|
| 1 | Sıra numarası ver | `NN-<konu>.md` — mevcut raporların boşluğundan devam | §7.4 satır 1 |
| 2 | Şablon seç | güvenlik → [documentation/security-audit-template.md](documentation/security-audit-template.md); test → [testing/test-plan-template.md](testing/test-plan-template.md) | §7.4 satır 2 |
| 3 | Ölçüm bloğu yaz | her sayı **komut çıktısıyla** | §7.4 madde 1 (G9) |
| 4 | Bulguları tabloya al | bulgu · şiddet · kanıt · düzeltme | §2 |
| 5 | 3-8 KB'ı koru | aşıyorsa §10.3 | §10 |
| 6 | Mojibake taraması | §12/4 çıktısı = 0 | G5 |

### §13.3 Senaryo C — Yeni Şablon (`.ai/.templates/`)

| # | Adım | Çıktı | Kapı |
|---|---|---|---|
| 1 | Envanterde satır aç | `PLANLI` — şablon yazılırken envanter değiştirilmez (tek geçiş kuralı) | [index.md](index.md) §4.1 |
| 2 | İskeleti kur | H1 + Zorunlu Bağlantılar + §0 | §2 |
| 3 | 7 bölüm yaz (envanter §3.7) | §1 amaç … §7 referanslar | §7.6 |
| 4 | Yer tutucuları **bırak** | şablon sınıfında serbest; üretim çıktısında 0 | G4 · §7.6 madde 1 |
| 5 | Eşiği geç | ≥ 480 satır · ≥ 27 KB (§10.1 sınıf 2) | §12/1 |
| 6 | `PLANLI → YAZILDI` | tek geçişte, ölçüm sonrası | envanter §1.1 M3 |

### §13.4 Ortak Sınama Soruları (her senaryoda)

| # | Soru | Evet ise | Hayır ise |
|---|---|---|---|
| 1 | Frontmatter tam 8 alan mı? | §9'a geç | R1 — durdur |
| 2 | Yeni link diskte var mı? | §8.2'ye uymuştur | R9/R10 — linki kaldır |
| 3 | Sayı komut çıktılı mı? | G9 geçti | `NEEDS_VALIDATION` yaz |
| 4 | Boyut sınıfı doğru mu? | §10.1 satırını işaretle | §10.2/§10.3'e uygula |
| 5 | Türkçe ve mojibake 0 mı? | §12/4 geçti | R13 — UTF-8 yeniden yaz |
