---
title: "VersaCoder — Belge (Vault/MD) Şartnamesi"
type: spec
category: spec
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "Vault MD dosyasının zorunlu yapısı (frontmatter, §0, tablo, boyut), link izinleri, UTF-8/mojibake, kanıt ve kopyalama yasağı — GATE-004a'nın ölçüt kaynağı"
---

# VersaCoder — Belge (Vault/MD) Şartnamesi (`spec/docs-spec.md`)

**Zorunlu Bağlantılar:** [index](../index.md) · [CLAUDE.md](../CLAUDE.md) · [keys](../keys.md) · [WORKFLOW](../WORKFLOW.md) · [test-strategy](test-strategy.md) · [review-spec](review-spec.md) · [build-spec](build-spec.md)

> **Tek cümlelik sorumluluk:** *Vault'a yazılan her `.md` dosyası hangi yapıya uyar, hangi hedeflere link verir ve neyle kanıtlanır* sorusunun tek cevabıdır.
> Üretim şablonu `.ai/.templates/new-doc-contract.md` (batch kuralları); bu belge **şartnamedir** — ihlalini `review-spec` §2.1.6 üzerinden `reviewer` RED eder.

---

## §0 Özet (Answer First)

Her yeni/değişen vault `.md` dosyası: **UTF-8** · **ilk satır `---`** · **8 alanlı frontmatter** (`authority: SSOT`, `updated` güncel) · **`## §0 Özet (Answer First)`** · **≥3 tablo** (ya da 1 tablo + ASCII kutu diyagramı) · **≤5 madde** · **3–8 KB** · **yalnız izinli göreli hedeflere link** · **`check-links.ps1` = 0 bozuk** · **mojibake = 0**. Kanıtsız "bitti" (G5), kopya içerik (ADR-002) ve kod içi bilgi (EF Core / string-concat SQL) bu şartnamede geçersizdir.

---

## §1 Zorunlu Yapı

| # | Katman | Şart | İhlalde |
|---|---|---|---|
| 1 | Kodlama | UTF-8, LF/CR-LF, CR-only yok | `mojibake 0` ihlali → RED |
| 2 | Frontmatter | 8 alan, ilk satır `---`, `authority: SSOT` | `NoFrontmatter` / eksik alan → RED |
| 3 | Başlık | `# H1` = frontmatter `title` ile aynı anlam | LOW bulgu |
| 4 | Zorunlu bağlantılar | `**Zorunlu Bağlantılar:**` satırı | LOW bulgu |
| 5 | Tek cümle | `> **Tek cümlelik sorumluluk:**` | MEDIUM |
| 6 | Bölümler | `## §0` zorunlu, sonrası `## §N` | MEDIUM |
| 7 | Yoğunluk | ≥3 tablo **veya** 1 tablo + ASCII diyagram | MEDIUM |
| 8 | Madde | liste ≤5 madde (ADHD çıktısı) | LOW |
| 9 | Boyut | 3–8 KB | HIGH (aşım) / LOW (alt) |
| 10 | Dil | Türkçe; dosya adı ASCII `kebab-case.md` | LOW |

**Frontmatter (birebir):**

```yaml
---
title: "<Dosya adı — VersaCoder>"
type: knowledge | spec | rules | adr | checklist | prompt | playbook | report | architecture
category: <klasör adı>
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
owner: architect
scope: "<1 satır kapsam — ne cevaplar>"
---
```

---

## §2 Link İzinleri

| Dosyanın klasörü | İzinli göreli hedefler |
|---|---|
| `.ai/knowledge/*` | `../../index.md` · `../../CLAUDE.md` · `../../keys.md` · `../../VISION.md` · `../../.rules/{devexpress,data-access,security,coding-standards,testing}-rules.md` · `../../.decisions/accepted/ADR-004…` · `ADR-005…` · `ADR-012…` · `../index.md` · kardeş dosyalar |
| `.ai/spec` | `../index.md` · `../CLAUDE.md` · `../keys.md` · `../WORKFLOW.md` · `../.rules/testing-rules.md` · `../architecture/index.md` · kardeş dosyalar |
| `.ai/.rules` | `../index.md` · `../CLAUDE.md` · `../keys.md` · `../spec/test-strategy.md` · kardeş dosyalar |
| `.ai/.decisions/accepted` | `../index.md` · `../adr-index.md` · `../../CLAUDE.md` · `../../keys.md` · kardeş ADR'ler · `../../spec/test-strategy.md` · `../../reports/01-web-kaynaklari.md` |
| `.ai/architecture/l*` | `../index.md` · `../00-overview/{dependency-map,system-overview}.md` · `../../CLAUDE.md` · `../../keys.md` · `../catalog/<katman>/<node>.md` · `../../.decisions/accepted/ADR-013…` · `../../reports/00-kapsam-analizi.md` |
| `.ai/reports` · `.ai/checklists` · `.ai/prompts` · `.ai/playbooks` | `../index.md` · `../CLAUDE.md` · `../keys.md` · `../WORKFLOW.md` · `../VISION.md` · kardeş dosyalar |

**Denetim:** `powershell -NoProfile -ExecutionPolicy Bypass -File .ai/scripts/check-links.ps1` → **0 bozuk** · `powershell -NoProfile -ExecutionPolicy Bypass -File .ai/scripts/measure-vault.ps1` → `NoFrontmatter = 0`, `Mojibake = 0`.

---

## §3 Kanıt ve Yasaklar

| # | Yasak | Karşılığı | Seviye |
|---|---|---|---|
| 1 | Kanıtsız "bitti/tamam" | G5 → build/test/tarama çıktısı zorunlu | **CRITICAL** |
| 2 | Başka vault'tan kopya metin (CoreMusic, versacoder.ai) | ADR-002 → elde yazım | **CRITICAL** |
| 3 | EF Core referansı / string-concat SQL | G2 · ADR-005 → ADO.NET + Dapper, parameterized | **CRITICAL** |
| 4 | DevExpress ≠ v26.1 · ribbon ≠ `Office2010` | G3 · ADR-004 | **HIGH** |
| 5 | Tek bacaklı anlatım / tek bacakta test | G6 · ADR-001 → `net48` + `net10.0-windows` | **HIGH** |
| 6 | Var olmayan dosyaya link | §2 → `check-links.ps1` | **HIGH** |
| 7 | `http(s)://` dış link (az) · `mailto:` · boş link | §2 | MEDIUM |
| 8 | Mermaid / Unicode kutu çizim | ADR-020 → saf ASCII `+ - | + +` | MEDIUM |
| 9 | `NEEDS_VALIDATION` / `UNKNOWN` etiketsiz tahmin | G9 | MEDIUM |
| 10 | Bilgiyi 2 dosyaya yazmak | SSOT → tek kaynak + link | MEDIUM |

---

## §4 Yaşam Döngüsü

```
yeni belge           frontmatter + H1 + §0 -> dogrulama (olcum)
   |                                           |
   v                                           +--> NoFrontmatter=0, Mojibake=0
edit / revize  ---->  link taramasi ------>  check-links = 0  ---->  review-spec uzerinden review
                                                     |
                                                     v
                                              GATE-004a (vault kapanisi)
                                                     |
                                                     v
                                              log.md + rapor (tarih, sayi)
```

| Aşama | Zorunlu çıktı | Sahip |
|---|---|---|
| Üretim | dosya + frontmatter | dosyanın sahibi rol |
| Doğrulama | `check-links` + `measure-vault` çıktısı (sayılarla) | `reviewer` |
| İnceleme | `review-spec` §2.1.6 ölçütü, PASS/RED | `reviewer` |
| Gate | GATE-004a: bozuk link **0**, eksik frontmatter **0** | kullanıcı |

---

## §5 Kabul Kriterleri

- [ ] Dosya UTF-8, ilk satır `---`, frontmatter **8/8** alan dolu
- [ ] `## §0 Özet (Answer First)` var ve 3–5 cümle
- [ ] ≥3 tablo (ya da 1 tablo + ASCII diyagram); liste ≤5 madde; boyut 3–8 KB
- [ ] `check-links.ps1` → **0**; `measure-vault.ps1` → `NoFrontmatter 0`, `Mojibake 0`
- [ ] Yeni/degisen dosya sayısı + link düzeltme sayısı raporda (`n/m`)

---

## §6 İlgili Belgeler

| Belge | Ne verir |
|---|---|
| [test-strategy](test-strategy.md) | kanıtın test tarafı (coverage, gate testleri) |
| [review-spec](review-spec.md) | bu şartnamenin RED/PINK karar biçimi |
| [build-spec](build-spec.md) | vault ön kontrollerinin build'e girişi |
| `.ai/.templates/new-doc-contract.md` | batch üretim kuralları (şablon) |
| `.ai/.decisions/accepted/ADR-003-vault-structure-500md-frontmatter.md` | 500 MD + bozuk link = 0 hedefi |

**Değişiklik günlüğü:** 2026-09-25 — ilk yazım (architect). Kaynak: `new-doc-contract.md` + ADR-003 + `scripts/check-links.ps1`.
