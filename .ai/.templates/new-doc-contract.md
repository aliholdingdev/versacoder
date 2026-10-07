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
pwsh -File C:\versacoder\.ai\scripts\check-links.ps1    # BOZUK link : 0 hedefi
pwsh -File C:\versacoder\.ai\scripts\measure-vault.ps1  # NoFrontmatter = 0, Mojibake = 0
```
