---
title: "VersaCoder — ADR Kayıt Defteri (Karar Defteri)"
type: adr-index
category: adr
status: active
version: 1.0.2
updated: 2026-10-04
authority: SSOT
scope: "Tüm ADR'lerin tek kayıt noktası: numara, başlık, durum, tarih, supersede zinciri, risk/karar eşlemesi"
---

# VersaCoder — ADR Kayıt Defteri (`adr-index.md`)

**Zorunlu Bağlantılar:** [Vault Ana Sayfa](../index.md) · [Anayasa](../CLAUDE.md) · [Anahtar Kelime Haritası](../keys.md) · [Kapsam Analizi](../reports/00-kapsam-analizi.md) · [ADR-001](accepted/ADR-001-dual-target-net48-net10.md) · [ADR-002](accepted/ADR-002-handwritten-content-no-copypaste.md) · [ADR-003](accepted/ADR-003-vault-structure-500md-frontmatter.md) · [ADR-004](accepted/ADR-004-devexpress-universal-v261-office2010.md) · [ADR-005](accepted/ADR-005-ado-net-dapper-no-ef-core.md)

> **Tek cümlelik sorumluluk:** *Hangi ADR numarası açık, durumu ne, hangi riski kapatıyor* sorusunun tek cevabıdır.
> ADR gövdeleri durum konusunda bağlayıcı değildir; **bu dosya ADR durumunun SSOT'udur.**

---

## §1 Kapsam ve Kullanım

### §1.1 — Ne zaman kullanılır

Yeni ADR açılırken (numara + satır) · durum değişince (`draft` → `accepted` → `superseded`) · bir ADR başka bir ADR'yi ezerken · "en güncel karar hangisi?" sorusunda · bir risk (R1–R6) hangi kararla kapatıldı diye arandığında · `GATE-004` mimari onay kanıtı olarak.

### §1.2 — Kanonical dizin notu (şablon farkı)

> **KURAL:** Bu vault'un kanonical ADR dizini **`.ai/.decisions/`** dir. `.templates/adr/*` şablonlarındaki örnek yollar `.ai/adr/...` yazar; **o yollar bu vault'ta geçersizdir**. Yeni ADR `.decisions/accepted|draft|rejected/` altına açılır, defter satırı da bu dosyaya yazılır.

| Öğe | Kanonical | Şablonun eski örneği |
|---|---|---|
| Defter | `.decisions/adr-index.md` | `.ai/adr/index.md` ❌ |
| Dosya | `.decisions/accepted/ADR-NNN-<slug>.md` | `.ai/adr/ADR-NNN-*.md` ❌ |
| Taslak / reddedilmiş | `.decisions/draft/` · `.decisions/rejected/` | — |

### §1.3 — SSOT kuralı

1. Durum, supersede ve numara **yalnız bu dosyada** yaşar → iki defter = iki gerçek (SSUT).
2. ADR gövdesindeki `status` **yansımadır**; çelişirse bu dosya kazanır → denetim tek noktadan.
3. ADR gövdesi silinmez; durum değişimi **dosya üstü işaretle** yapılır → geçmiş kanıtdır.
4. Bu dosya karar **içeriği** barındırmaz (SRP) → gerekçe ADR gövdesinde.
5. Defterde satırı olmayan ADR = kayıpsız kayıt → G4/G12 ihlali.

---

## §2 Bağlam (Context)

### §2.1 — Karar ortamı

| Boyut | VersaCoder gerçeği | Kaynak |
|---|---|---|
| Vault kökü | `C:\versacoder\.ai` (kullanıcı onayı) | `reports/00-kapsam-analizi.md` §1.4 |
| Defter / klasör | `.decisions/adr-index.md` · `accepted/` (✅ diskte) · `draft/` `rejected/` (planlı) | disk ölçümü 2026-09-25 |
| Açılış / durum / frontmatter | 29 karar (ADR-001..029): 28 `accepted` + 1 `superseded` (ADR-020) · enum `accepted·draft·rejected·superseded` · 8 alan `authority: SSOT` | §2.3 · §3.4 · `CLAUDE.md` G4 |
| Slug / dil | İngilizce kebab-case 2–4 sözcük · gövde Türkçe | `keys.md` §6 |

### §2.2 — Kararı tetikleyen tetikleyiciler

1. 5 senior karar (`reports/00-kapsam-analizi.md` §2) **kapanmıştı ama kaydı yoktu** — gerekçe altı ay sonra bulunamazdı.
2. R1–R6 / B1–B4 için "hangi karar kapatıyor?" sorusu yanıtsızdı.
3. Kopyalama yasağı (G1) nedeniyle yedek vault'taki ADR listesi **kullanılamazdı**; defter sıfırdan yazılmalıydı.
4. G8 sapma taraması tek satır listesi ister: v26.1 · `net48`+`net10.0-windows` · Dapper · EF Core yasağı.
5. Supersede zinciri yoksa "en güncel karar" bilinemez; `keys.md` §3/24 eşlemesinin hedefi (`.decisions/`) boştur.

### §2.3 — Yaşam döngüsü ve klasör yapısı

```text
.ai/.decisions/
|-- adr-index.md   # bu dosya -- ADR durumunun SSOT'u
|-- accepted/      # yururlukte: ADR-001 .. ADR-029 (29 dosya; 28 accepted + ADR-020 superseded)
|-- draft/         # planli -- status: draft
|-- rejected/      # planli -- status: rejected (gerekce zorunlu)

Durum gecisi (tek yonlu):
  draft --[review + 18/18]--> accepted --[yeni ADR]--> superseded
  draft --[gerekceli RED]--> rejected
  superseded --> silme = IMKANSIZ (dosya arsivde kalir)
```

> **UYARI:** `draft/` ve `rejected/` henüz diskte yoktur; ilk dosyada oluşturulur. Klasör ↔ defter satırı **her zaman eşleşir** (klasör ≠ status = RED).

### §2.4 — Kapsam dışı

ADR **gövde içeriği** · `.ai/.rules/` (kural ≠ karar) · `brain.md` ADR özetleri (bu defterin **özeti**dir, ikinci defter değil) · kod içi karar yorumları · gate geçişleri (`WORKFLOW.md`).

---

## §3 Karar (Decision)

### §3.1 — Karar ifadesi (tek cümle)

**VersaCoder `.ai` vault'unda tüm mimari/teknoloji kararları `.decisions/adr-index.md` defterine kayıtlı `ADR-NNN-<ingilizce-slug>.md` dosyalarında yaşar; defter ADR durumunun tek doğruluk kaynağıdır — karar verildi.**

### §3.2 — Alternatifler

| # | Alternatif | Artı | Eksi | Neden elendi |
|---|---|---|---|---|
| A — Defterli `.decisions/` (SEÇİLDİ) | tek bakışta durum, zincir, denetlenebilir | satır elle senkron | **elenmedi** |
| B — Deftersiz (dosyalar tek kaynak) | senkron derdi yok | "en güncel karar" listelenemez | tek bakış şart → elendi |
| C — Script ile otomatik defter | sayım hatası sıfır | ilişki/gerekçe notu otomasyona kurban | otomasyon yalnız **doğrulamada** → elendi |
| D — Kararları `brain.md`'ye gömme | dosya sayısı az | kök şişer, SRP ihlal, zincir kaybolur | ADR-003 kök 30–88 KB bandı → elendi |
| E — Yalnız Obsidian link bulutu | görsel | metin denetimi ve diff okunmaz | denetim komutu şart → elendi |

### §3.3 — Gerekçe

Defter, 5 kapanmış senior kararın ve R1–R6 risklerinin izini tek satırda taşır; iz olmadan `GATE-004` onayı kanıtsız kalır (G5). Elle yazım ≈40 dk/ADR, script maliyeti bunu aşar — YAGNI. B en güçlü rakipti: "hangi ADR hangi riski kapatıyor" sorusu 5 dosyalık tarama ister. D seçeneği [ADR-003](accepted/ADR-003-vault-structure-500md-frontmatter.md) ile çelişir: kök dosyalar SRP ile yükümlüdür.

### §3.4 — Karar defteri (tüm ADR'ler)

| ADR | Başlık / slug | Status | Tarih | Supersedes | İlişkili risk / karar |
|---|---|---|---|---|---|
| [ADR-001](accepted/ADR-001-dual-target-net48-net10.md) | Dual target: net48 + net10.0-windows | `accepted` | 2026-09-25 | — | R5 · B3 · **G6** · karar #1 |
| [ADR-002](accepted/ADR-002-handwritten-content-no-copypaste.md) | Handwritten content — no copy-paste | `accepted` | 2026-09-25 | — | R4 · A2 · **G1** · karar #2 |
| [ADR-003](accepted/ADR-003-vault-structure-500md-frontmatter.md) | Vault: 500+ MD, 30–88 KB kök, zorunlu frontmatter | `accepted` | 2026-09-25 | — | R4 · R6 · **G4** · karar #3 |
| [ADR-004](accepted/ADR-004-devexpress-universal-v261-office2010.md) | DevExpress Universal 2026 / v26.1 + Office2010 | `accepted` | 2026-09-25 | — | R3 · B3 · **G3** · karar #4 |
| [ADR-005](accepted/ADR-005-ado-net-dapper-no-ef-core.md) | ADO.NET + Dapper — EF Core yasak | `accepted` | 2026-09-25 | — | R3 · **G2** · karar #5 |
| [ADR-006](accepted/ADR-006-opencode-tool-contract.md) | opencode Tool Seti ⇄ VersaCoder.Protocol Sözleşmeleri | `accepted` | 2026-09-25 | — | — |
| [ADR-007](accepted/ADR-007-agent-hierarchy-session.md) | Ana Ajan + Alt Ajan: Oturum Hiyerarşisi | `accepted` | 2026-09-25 | — | — |
| [ADR-008](accepted/ADR-008-transcript-compaction.md) | Transcript Compaction (8 MB eşiği) | `accepted` | 2026-09-25 | — | — |
| [ADR-009](accepted/ADR-009-tool-execution-sandbox.md) | Tool Çalıştırma: Timeout, CancellationToken, İzin Modeli | `accepted` | 2026-09-25 | — | — |
| [ADR-010](accepted/ADR-010-cli-desktop-shared-core.md) | Desktop ve CLI Aynı Çekirdeği Paylaşır (l0..l6) | `accepted` | 2026-09-25 | — | — |
| [ADR-011](accepted/ADR-011-llm-provider-order.md) | LLM Sağlayıcı Sırası ve Zorunlu Streaming | `accepted` | 2026-09-25 | — | — |
| [ADR-012](accepted/ADR-012-dual-tfm-data-provider.md) | İki Bacakta Veri Sağlayıcı | `accepted` | 2026-09-25 | — | — |
| [ADR-013](accepted/ADR-013-layered-architecture-52-nodes.md) | Katmanlı Mimari: 52 Düğüm / 5000+ Bileşen Kataloğu | `accepted` | 2026-09-25 | — | — |
| [ADR-014](accepted/ADR-014-slnx-and-cpm.md) | VersaCoder.slnx + Central Package Management | `accepted` | 2026-09-25 | — | — |
| [ADR-015](accepted/ADR-015-designer-resx-convention.md) | Form Şı: `.cs` + `.Designer.cs` + `.resx` | `accepted` | 2026-09-25 | — | — |
| [ADR-016](accepted/ADR-016-no-ef-core-restated.md) | Veri Erişimi Sınırları (ADR-005'i tamamlayan kapsam notu) | `accepted` | 2026-09-25 | — | — |
| [ADR-017](accepted/ADR-017-parameterized-sql-mandatory.md) | Parametreli SQL zorunlu; string-concat RED | `accepted` | 2026-09-25 | — | — |
| [ADR-018](accepted/ADR-018-secret-store.md) | Secret Store: anahtarlar şifreli depoda, `keys.md` yalnız harita | `accepted` | 2026-09-25 | — | — |
| [ADR-019](accepted/ADR-019-dual-tfm-test-matrix.md) | İki Bacak Build + Test Matrisi Zorunlu | `accepted` | 2026-09-25 | — | — |
| [ADR-020](accepted/ADR-020-no-mermaid-only-box-drawing.md) | Diyagramlar UTF-8 Box-Drawing, Mermaid varsa yanında karşılığı | `superseded` | 2026-10-01 | — | **ADR-029** (D1 ASCII kararı) |
| [ADR-021](accepted/ADR-021-vault-ssot-single-index.md) | Vault navigasyonu tek `index.md`; tekrar yasak (SRP) | `accepted` | 2026-09-25 | — | — |
| [ADR-022](accepted/ADR-022-git-commit-gate004.md) | İlk commit GATE-004'te; önce vault ve wireframe onayı | `accepted` | 2026-09-25 | — | — |
| [ADR-023](accepted/ADR-023-opencode-reference-readonly.md) | opencode referans kopyası salt okunur, kod kopyası yok | `accepted` | 2026-09-25 | — | — |
| [ADR-024](accepted/ADR-024-coremusic-vault-immutable.md) | CoreMusic Vault dokunulmaz: yapı referansı, içerik değil | `accepted` | 2026-09-25 | — | — |
| [ADR-025](accepted/ADR-025-gate003-ui-hard-stop.md) | UI Hard Stop: wireframe onayı olmadan kod yazılmaz | `accepted` | 2026-09-25 | — | — |
| [ADR-026](accepted/ADR-026-logging-structured.md) | Structured Log: seviye, şablon ve iki bacak | `accepted` | 2026-09-25 | — | — |
| [ADR-027](accepted/ADR-027-component-catalog-5000.md) | Bileşen Kataloğu 5000+: sayı aracı ile kanıt | `accepted` | 2026-09-25 | — | — |
| [ADR-028](accepted/ADR-028-layered-baseline-9-projects.md) | Katmanlı Mimari Baz Kararı: 8 makro katman / 52 düğüm / 9 proje | `accepted` | 2026-09-29 | — | — |
| [ADR-029](accepted/ADR-029-ascii-box-drawing-only.md) | Diyagramlar saf ASCII (`+ - |`), Unicode box-drawing yasak | `accepted` | 2026-10-01 | **ADR-020** | G4 · kullanıcı kararı 2026-10-01 |
| [ADR-030](accepted/ADR-030-5000-layers-class-level.md) | 5000+ katman: tek hedef + yon grubu = fiziksel .dll — v2.1.0 | `accepted` | 2026-10-02 | - | kullanıcı kararı 2026-10-02 (G7) · revizyon §6 |

> **ADR-030 revizyonu (2026-10-02, v2.0.0 → v2.1.0):** tek hedef `>= 5000` + **yon grubu = fiziksel `.dll` = `.csproj`** (P ekseni Y'ye merge, 3 eksen K/N/Y); ara kademeler silindi; 5000 `.csproj` icin **K bandi** (K0000..K5499) ve 5 faz (K0 + N1..N4) kuruldu; 8x8 matris **olcum olarak kalir**, 5000x5000 kural seti **uretilir** (dependency-map §6). Supersede zinciri **boş** (ADR-028/013 korunur). Ayrıntı: ADR-030 §6.

> Satırlar numara artan sıralıdır; `status`/`tarih` ADR frontmatter'ı ile birebir eşleşir (KR1–KR3). `Supersedes` sütunu ilk supersede ile **ADR-029** satırında doldu (ADR-020 → ADR-029; bkz. §3.6). `İlişkili risk` sütunu ADR-001..005 dışındaki gövdelerde `R1–R6` / `B1–B4` token'ı taşımadığı için `—`dır (2026-10-01 taraması); bu ADR'lerin risk bağını §3.8/§3.4 notları taşır.

### §3.5 — Numaralandırma kuralları

1. **Format `ADR-NNN`** (3 hane, artan): doğru `ADR-006`; yanlış `ADR-6` / `ADR-06`.
2. **Yeni numara = en yüksek + 1**, açık numara atlanmaz: 005 → 006; gerekçesiz 005 → 008 yasak.
3. **Slug İngilizce kebab-case, 2–4 sözcük** ve defter satırıyla **aynı** (`ado-net-dapper-no-ef-core`); `ef-yok` ❌.
4. **Numara asla yeniden kullanılmaz**; vazgeçilen karar `rejected/`'ta numarasıyla kalır.
5. **Klasör = status** ve gövde `adr_id` dosyadaki numarayla aynı (`adr_id: ADR-5` ❌).

### §3.6 — Supersede prosedürü (5 adım)

1. Yeni ADR `draft/`'ta; numara = max+1 → defterde satır `draft`.
2. Kabul (`status: accepted`) → satır `accepted`.
3. Eski ADR'ye `status: superseded` + H1 altındaki gövde notu (`SUPERSEDED by ADR-MMM`) → eski satır `superseded`. (Frontmatter `superseded_by` anahtarı **yok**: 8 alan kuralı G4 kazanır; ilişki hem gövde notunda hem bu defterin `Supersedes` sütununda yaşar.)
4. Yeni ADR'ye `supersedes: ADR-NNN` → defterde ilişki sütunu dolar.
5. Zincir doğrulanır (§5.3 komut 4) + `log.md`'ye tek satır.

> **KURAL:** Zincir **tek yönlü** (`ADR-NNN → ADR-MMM`, geriye link yok). **Silme yok**; defter büyür, geçmiş kaybolmaz. Kopuk zincir = RED (AG9).

### §3.7 — Yeni ADR ekleme kontrol listesi

1. **Eşik testi:** ≥2 dosya bağlıyor · ay sonra geçerli · yanlışsının düzeltme maliyeti yüksek · "neden böyle yaptık" cevabı gerekli → hepsi `HAYIR` ise ADR açılmaz.
2. Şablon (`.templates/adr/adr-template.md` §1–§7) · numara = max+1 (§3.5/2) · slug tekil (§3.5/3) · dosya `draft/`'ta, frontmatter 8 alan.
3. §3.2'de **≥2 alternatif**, §7'de **gerçek itiraz + cevap**; bağlantılar yalnız var olan dosyalara (`Test-Path`).
4. `status: accepted` → dosya `accepted/`'e taşınır · bu deftere **tek satır** eklenir (§3.4 kalıbı).
5. §5.3 komutları (1–6) çalıştırılır · `log.md` tek satır + `.rules/*.md` notu.

### §3.8 — 5 kapanmış senior karar → ADR eşlemesi

| # | Senior karar (`reports/00-kapsam-analizi.md` §2) | Gerekçe özeti | ADR |
|---|---|---|---|
| 1 | `net48` + `net10.0-windows` iki bacak | ai.03 birebir; net48 = legacy, net10 = modern | ADR-001 |
| 2 | Vault içeriği sıfırdan, elle yazım | kopya yasağı; yedek vault tutarsız (80 KB vs 2 KB) | ADR-002 |
| 3 | ~500+ MD · kök 30–88 KB · zorunlu frontmatter | 599 MD ölçek hedefi; yedek 1–16 KB çıpa değil | ADR-003 |
| 4 | DevExpress Universal 2026 / v26.1 + `Office2010` | ai.03; ai.01'deki 24.2.x **REDDEDİLDİ** | ADR-004 |
| 5 | ADO.NET + Dapper, EF Core **YOK** | net48'de EF Core yok; SQL görünür olmalı | ADR-005 |

### §3.9 — Kabul kriterleri, kapsam, geri alma

| # | Kabul kriteri | Ölçüt | Kanıt |
|---|---|---|---|
| KR1 | Defter satırı = diskteki ADR sayısı | 0 fark | §5.3 komut 1 |
| KR2 | Slug + status + tarih + supersede dolu | 0 boş hücre | komut 2 |
| KR3 | Numaralar tekil, artan, deliksiz | 0 tekrar / 0 delik | komut 3 |
| KR4 | Klasör ↔ status ↔ defter eşit | 0 uyuşmazlık | komut 5 |
| KR5 | Supersede zinciri kopuk değil | 0 kopuk | komut 4 |

**Kapsam:** `.decisions/adr-index.md` + `.decisions/{accepted,draft,rejected}/ADR-*.md`; kapsam dışı ADR gövdesi · `.ai/.rules/` · `brain.md` özeti · gate'ler. **Geri alma:** `git checkout -- .ai/.decisions/adr-index.md` (~2 dk, reversible); ADR dosyaları asla silinmez. **Karar veren:** architect · Reviewer: reviewer.

---

## §4 Korkular ve Guardrails

### §4.1 — Bu kararın korkuları

Defter elle güncellenir, unutulur → bayat defter, iki gerçek (SSUT) · iki ADR aynı numarayı alır · supersede unutulur → iki "en güncel" karar · klasör/status ayrışır → denetim komutu yanlış okur · defter ADR gövdesinden içerik kopyalar → ikinci kaynak doğar · slug Türkçe/boşluklu → link/arama kırılır (G4).

### §4.2 — Risk matrisi ve azaltma

| Risk | Olasılık | Etki | Skor | Azaltıcı |
|---|---|---|---|---|
| Bayat defter (senkron unutma) | orta | yüksek | **H** | komut 1–2 her PR'da |
| Numara çakışması | düşük | yüksek | **H** | max+1 + komut 3 |
| Supersede kopuğu | düşük | yüksek | **H** | AG9 + komut 4 |
| Klasör/status ayrışması | orta | orta | M | komut 5 + §2.3 notu |
| Gövde ↔ defter içerik kopyası | orta | orta | M | §1.3/4 SRP |

**Azaltma:** her ADR kabulünde §3.7 adımları (~40 dk) · her PR'da §5.3 komutları — **>0 satır = DUR** · slug regex `^ADR-\d{3}-[a-z0-9]+(-[a-z0-9]+)*\.md$` · yazım yalnız `.decisions/` altında, onaylı (G7).

### §4.3 — Kabullenilemez kriterler (otomatik RED)

| # | Kriter | İhlalde |
|---|---|---|
| 1 | Defter satırı olmayan ADR dosyası (ya da tersi) | RED — satır/dosya eşleştirilir |
| 2 | Status enum dışı (`approved`, `done`, `WIP`) | RED — enum'a döndürülür |
| 3 | Silinmiş eski ADR | CRITICAL — dosya geri konar (G7) |
| 4 | `authority: SSOT` olmayan defter · deftere kopyalanmış gerekçe | RED — frontmatter (G4) · satır silinir (SRP) |

### §4.4 — Guardrail Tablosu (AG1–AG14)

| # | Guardrail | CLAUDE.md | İhlal |
|---|---|---|---|
| AG1 | Frontmatter 8 alan + `authority: SSOT` | G4 | RED |
| AG2 | İngilizce slug, Türkçe gövde | G4 | RED |
| AG3 | Secret/API key yok | G9 | kritik RED |
| AG4 | Defter satırı zorunlu (KR1) | G5 | RED |
| AG5 | Kanıtsız iddia `⚠️` | G9 | RED |
| AG6 | Yasaklı vault salt okunur | G1 | RED |
| AG7/AG8 | Numara ve slug tekrarsız | — | RED |
| AG9 | Supersede zinciri kopuk değil | — | RED |
| AG10 | §6.1 18/18 tam | G5 | RED |
| AG11/AG12 | Kırık link 0 · defter her ADR'de güncellenir | G4 | RED |
| AG13 | Supersede zinciri güncellenir | — | RED |
| AG14 | §5.3 komutları yazmaz | G7 | CRITICAL |

---

## §5 Uygulama ve Hata Modları

### §5.1 — Uygulama adımları (11 adım)

1. Diskteki ADR dosyalarını listele · 2. Mevcut defteri oku · 3. En büyük `ADR-NNN` bul · 4. Slug yaz + tekillik denetle.
5. Satırı hazırla (başlık, status, tarih, ilişki) · 6. Defteri güncelle (SSOT) · 7. Klasör ↔ status eşleştir · 8. Supersede varsa iki dosyayı birlikte işaretle.
9. §5.3 komutlarını çalıştır · 10. AG1–AG14 denetimi · 11. `log.md` tek satır + PR notu.

### §5.2 — Hata modları

| # | Hata modu | Belirti | Düzeltme |
|---|---|---|---|
| 1 | Defter unutuldu | diskte dosya, defterde yok | satır ekle (~2 dk) |
| 2 | Numara çakışması | iki dosya `ADR-00N` | biri yeniden numaralanır |
| 3 | Status tutarsız | klasör ≠ status ≠ defter | üçü eşleştir (KR4) |
| 4 | Supersede kopuk | eski dosya hâlâ `accepted` | `superseded` + gövde notu (§3.6/3) |
| 5 | Slug çakışması / Türkçe | `kararlar-01.md` | İngilizce slug + ayır |
| 6 | Çift defter / ölü link / kopya içerik | `brain.md` ≠ defter · `Test-Path` False · yabancı cümle | tek SSOT · ad düzelt · sıfırdan yaz (AG6) |

### §5.3 — Doğrulama komutları (yalnız-oku)

```powershell
# 1 ADR sayisi = defter satiri sayisi (ESIT olmali)
(Get-ChildItem .ai\.decisions -Recurse -Filter 'ADR-*.md').Count
# 2 defter satirlari -- slug + status + tarih DOLU olmali
Select-String -Path .ai\.decisions\adr-index.md -Pattern 'ADR-\d{3}'
# 3 numaralar tekil ve artan (tekrar/delik CIKMAMALI)
Get-ChildItem .ai\.decisions -Recurse -Filter 'ADR-*.md' |
  ForEach-Object { if ($_.Name -match '^ADR-(\d{3})-') { $Matches[1] } } | Sort-Object -Unique
# 4 supersede zinciri KOPUK olmamali
Select-String -Path .ai\.decisions\adr-index.md -Pattern 'supersed'
# 5 klasor <-> status ESIT olmali
Get-ChildItem .ai\.decisions -Recurse -Filter 'ADR-*.md' | ForEach-Object {
  '{0} => {1}' -f $_.Directory.Name, ([regex]::Match((Get-Content $_.FullName -Raw),'(?m)^status:\s*(\S+)').Groups[1].Value) }
# 6 authority: SSOT sapmasi 0 olmali
Get-ChildItem .ai\.decisions -Recurse -Filter '*.md' |
  Where-Object { (Get-Content $_.FullName -Raw) -notmatch '(?m)^authority:\s*SSOT\s*$' } |
  ForEach-Object { 'SSOT YOK -> ' + $_.Name }
```

> **UYARI:** Altı komut da **hiçbir dosyayı değiştirmez**. Yazma yalnız §5.1 adım 6–8'de, G7 onayıyla.

---

## §6 Doğrulama ve Testler

### §6.1 — Kontrol listesi (18 madde)

- [ ] 1. Frontmatter 8 alan (`title·type·category·status·version·updated·authority·scope`) · 2. H1 + **Zorunlu Bağlantılar** · 3. §1–§7 iskeleti eksiksiz
- [ ] 4. §3.1 karar tek cümle (`… karar verildi.`) · 5. §3.2 alternatif ≥2 satır, 4 sütun dolu
- [ ] 6. §3.4 defter tablosu = disk (KR1) · 7. Slug tekil/İngilizce (KR3) · 8. Status enum 4 değer
- [ ] 9. Klasör ↔ status ↔ defter eşit (KR4) · 10. Supersede zinciri tek yönlü (KR5) · 11. §3.8 eşleme dolu
- [ ] 12. §3.4 risk eşlemesi (R1–R6) satır bazlı · 13. §3.7 ekleme adımları · 14. §5.3 6 yalnız-oku komutu
- [ ] 15. Kanıtsız iddia `⚠️` işaretli (AG5) · 16. Mojibake yok; ı ğ ş İ ö ç ü doğru (AG2) · 17. `Test-Path` = True
- [ ] 18. Boyut bandı 10–18 KB (`(Get-Item).Length`)

### §6.2 — Test stratejisi

| Tip | Kapsam | Yöntem | Beklenen |
|---|---|---|---|
| Consistency / Uniqueness | defter ↔ disk, numara | §5.3 komut 1 + 3 | 0 fark / 0 tekrar |
| Status / Chain | klasör ↔ status, zincir | komut 5 + 4 | 0 uyuşmazlık / 0 kopuk |
| Encoding / Link | UTF-8 BOM'suz, CR-only 0, iç link | `Format-Hex` · `Test-Path` | BOM 0 · CR-only 0 · 0 kırık |
| Content | kopya içerik | cümle-bazlı spot check (ADR-002 §6) | 0 yabancı blok |

### §6.3 — Kanıt tablosu

| Kanıt | Konum | Durum |
|---|---|---|
| 5 ADR + 1 defter diskte → **bugün 29 ADR + 1 defter** (ADR-029 dahil, 2026-10-01) | `.decisions/` | ✅ yazıldı + `(Get-Item).Length` ile ölçüldü · §3.4 = 29 satır |
| 5 senior karar · R1–R6 · G1–G12 · §1–§7 iskeleti | `reports/00-kapsam-analizi.md` · `CLAUDE.md` · `.templates/adr/adr-template.md` | ✅ okundu 2026-09-25 |
| `draft/` · `rejected/` | `.decisions/` | ⚠️ VERIFICATION REQUIRED (ilk dosya / PART 5) |

### §6.4 — Şablona özgü ek kontroller

**Tek SSOT:** ikinci defter yok (komut 6; `brain.md` yalnız özet) · **numara artan:** max+1 (komut 3) · **kanonical yol:** `.ai/adr/` yoluna **link** yok, yalnız §1.2 karşılaştırma tablosu · **risk izi:** her R1–R6 satırı en az bir ADR'ye bağlı · **şablon:** örnek bloklar kopyalanmadı (ADR-002) · **boyut bandı:** 10–18 KB.

---

## §7 Sonuç ve Red Team

### §7.1 — Sonuç özeti

| Alan | Sonuç |
|---|---|
| Karar | ADR kaydı `.decisions/` altında; bu defter durumun SSOT'u |
| Durum / sürüm / tarih | `accepted` · 1.0.0 · 2026-09-25 |
| Sahip / review | architect · reviewer |
| İlgili ADR'ler | ADR-001 … ADR-029 (tam linkler §3.4 defterinde) |
| Etkilenen G'ler | G1 · G4 · G5 · G7 · G9 |
| Sonraki adım | İlk yeni ADR (ADR-006) → §3.7 adımları · ≈40 dk |

### §7.2 — Red Team (bu karar nasıl yanlış gider?) — gerçek itiraz + cevap

> **RED TEAM:** *"Elle senkronlanan defter eninde sonunda yalan söyler: biri dosyayı `accepted` yapar, satırı unutur; altı ay sonra geliştirici deftere güvenir, bayat kararı uygular. Dosyayı kendisi okusa bunu görecektir — defter araya giren ikinci bir gerçektir ve G1'in (tek gerçek) ruhuna aykırıdır."*

**Cevap (rebuttal):** İtiraz doğru tespit, yanlış sonuç. Risk "defterin yalan söylemesi" değil **senkronun unutulmasıdır** ve ölçülebilir: §5.3 komut 1, disk ile defter arasındaki **her 0 olmayan farkı** raporlar; bu fark §6.2 gereği her PR'da kapıdır. 29 ADR'de dosya dosya okumak yerine tarama maliyeti defter maliyetini geçer. Burada SSOT olan **durum**dur, **içerik** değil — gerekçe ADR gövdesinde yaşar (§1.3/4), ikinci içerik gerçeği doğmaz. Ölçüm: KR1 = 0 fark; düzeltme ≈2 dk (§5.2/1).

**En sık görülen üç itiraz ve savunması:** (1) *defter bayatlar* → komut 1 her PR'da; KR1 kapısı · (2) *aynı numara iki kez alınır* → max+1 + komut 3 + review · (3) *supersede unutulur* → AG9 + komut 4 + §3.6. Diğerleri: klasör/status ayrışması (KR4 + komut 5) · çift defter `brain.md` (§6.4) · şablonun `.ai/adr/` yolu (§1.2).

### §7.3 — Onay ve dağıtım

Yazar: architect · Vault/Mimari sorumlu: architect · Reviewer: reviewer — hepsi **2026-09-25**, imza satırı bu dosyada saklanır (kayıt `log.md`).
