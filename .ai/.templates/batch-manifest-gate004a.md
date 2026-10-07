---
title: "GATE-004a Batch Manifestosu - 347 Dosya Kapsami"
type: index
category: templates
status: active
version: 1.0.0
updated: 2026-09-25
authority: reference
scope: "GATE-004a icin uretilecek tum yeni MD dosyalarinin klasor, sayi ve tema listesi - subagent gorev spec'i burasi"
tags: [template]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# GATE-004a Batch Manifestosu

**Zorunlu Baglantilar:** [vault index](../index.md) · [new-doc-contract](new-doc-contract.md) · [ADR-003](../.decisions/accepted/adr-003-vault-structure-500md-frontmatter.md) · [ADR-013](../.decisions/accepted/adr-013-layered-architecture-52-nodes.md) · [CLAUDE.md](../claude.md)

> **Tek cumlelik sorumluluk:** *Hangi batch hangi klasore kac dosya, hangi temada yazar* sorusunun tek cevabidir.
> Uretim bu dosyaya gore yapilir; sapma = link/kapsam riski.

---

## §0 Ozet (Answer First)

147 mevcut → **hedef ≥500 MD**. Kapsanan yeni dosya: **~347** (index/sozlesme dosyaları haric). Her batch `new-doc-contract.md`'ye uyar, batch sonunda `check-links.ps1` = **0 bozuk**.

---

## §1 Batch Tablosu

| # | Klasor | Adet | Tema | Durum |
|---|---|---|---|---|
| B1 | `knowledge/devexpress/` | **60** | v26.1 kontrol ailesi (Ribbon, Grid, TreeList, Docking, Editors, Wizard, Theme, Data-binding, TFM uyumu) | ⏳ |
| B2 | `knowledge/winforms/` | **30** | Form yasam dongusu, designer+resx, DI, async, layout, kaynak/olcu, erisilebilirlik | ⏳ |
| B3 | `knowledge/data/` | **25** | ADO.NET + Dapper, parameterized SQL, migration+version tablosu, provider (SqlClient 5.2.x / Sqlite), secret, timeout | ⏳ |
| B4 | `knowledge/csharp/` | **25** | C# 7.3 (net48) vs C# 14 (net10), nullable, async, generics, hata yonetimi, interop, DI container | ⏳ |
| B5 | `architecture/l0..l7/` | **52** | Her katalog dugumu (N01..N52) icin bir **dugum detay** dosyasi: arayuz, akis, test plani, TFM | ⏳ |
| B6 | `spec/` | **35** | Katman+konu şartnameleri (UI, editor, ajan, oturum, tool, guvenlik, performans, dagitim) | ⏳ |
| B7 | `.decisions/accepted/` | **20** | ADR-006..011 (planli) + ADR-014..027 (yeni) | ⏳ |
| B8 | `checklists/` | **30** | Gate + inceleme + sevk listeleri (her biri kanit bekleyen sorular) | ⏳ |
| B9 | `prompts/` | **30** | Rol bazli promptlar (architect, developer, reviewer, tester, devops, docs, security...) | ⏳ |
| B10 | `playbooks/` | **20** | Tekrar eden is akisleri (form, katman, migration, ADR, release, debug, refactor...) | ⏳ |
| B11 | `.rules/` | **15** | Ek kural dosyalari (ui, async, logging, caching, i18n, dosya sistemi, git, paket...) | ⏳ |
| B12 | `reports/` | **10** | Rapor 02..09 + 2 ek rapor (PART 4/5/6/7/8 kanitlari) | ⏳ |
| B13 | `ui-design/` | **10** | 7 wireframe + index + flows + component-inventory (**PART 4 - GATE-003**) | ⏳ (ayri gate) |

**Toplam yeni dosya ≈ 347** (+6 indeks/sozlesme zaten yazildi = 153 → **~500**).

---

## §2 Batch Kurallari (tum B1..B12 icin)

1. Her dosya `new-doc-contract.md` §1-§5 gecer: UTF-8, frontmatter 8 alan, §0 Ozet, en az 1 tablo, maks 5 madde.
2. **Link sadece** ilgili klasorun `new-doc-contract.md` §3 izinli hedefleri + kardes dosyalar (adlari tam yazilir).
3. Her batch **kendi klasorunun `index.md`'sini** gunceller/kayit satiri ekler (varsa).
4. Batch sonu: `check-links.ps1` → 0 bozuk. Bozuk varsa **o dosyayi duzelt**, raporla.
5. **Yasak:** EF Core · string-concat SQL · DevExpress v26.1 disi · Office2010 disi ribbon · CoreMusic/versacoder.ai'dan kopya metin (ADR-002).

---

## §3 Ayri Kural: B13 (PART 4 wireframe)

B13 **GATE-003** onayi ister: UTF-8 box-drawing wireframe, ekran listesi **master prompt SSOT**
(`01-start-page, 02-main-ide, 03-solution-explorer, 04-code-editor, 05-ai-chat, 06-agent-panel, 07-settings`),
format `.templates/ui-design/wireframe-template.md`. Koddan **once** kullanici onayi alinir.

---

## §4 REZERVE DOSYA ADLARI (zorunlu - bunlari uretmeyen batch link kirar)

Indeks dosyalari asagida adlari gecen dosyalara link atmistir; **bu adlar birebir** uretilmek zorundadir.

| Batch | Rezerve adlar (ilk sirada uretilmeli) |
|---|---|
| B1 `knowledge/devexpress/` | `index.md` (60 dosyanin kayit defteri) |
| B2 `knowledge/winforms/` | `index.md` |
| B3 `knowledge/data/` | `index.md` |
| B4 `knowledge/csharp/` | `index.md` |
| B8 `checklists/` | `gate-004a-vault-kapanis.md` · `gate-003-ui-onay.md` · `gate-004-ilk-commit.md` · `gate-005-test-kapsami.md` + 26 yeni |
| B9 `prompts/` | `architect-prompt.md` · `developer-prompt.md` · `reviewer-prompt.md` · `tester-prompt.md` + 26 yeni |
| B10 `playbooks/` | `yeni-katman-ekleme.md` · `yeni-form-tasarlama.md` · `migration-ekleme.md` · `adr-acma.md` + 16 yeni |
| B11 `.rules/` | `index.md` (9 mevcut + 15 yeni = 24 dosyanin kayit defteri) |

Kalan 3 link (`knowledge/index.md` → 4 alt index) B1-B4'in `index.md`'leri ile kapanir.

## §5 Dogrulama (GATE-004a kapanisi)

```powershell
pwsh -File C:\versacoder\.ai\scripts\check-links.ps1      # BOZUK link : 0
pwsh -File C:\versacoder\.ai\scripts\count-components.ps1 # PASS (5440+)
pwsh -File C:\versacoder\.ai\scripts\measure-vault.ps1    # TotalMD >= 500, NoFrontmatter 0, Mojibake 0
```
