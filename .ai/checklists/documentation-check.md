---
title: "Dokumantasyon ve Kopya Icerik Kontrolu"
type: checklist
category: checklists
status: active
version: 1.0.0
updated: 2026-10-07
authority: reference
scope: "ADR-002 + ADR-024 + ADR-029 — sifirdan yazim, CoreMusic vault dokunulmaz, ASCII kutu"
tags: [checklist]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# Dokumantasyon ve Kopya Icerik Kontrolu

**Zorunlu Baglantilar:** [checklist index](index.md) · [vault index](../index.md) · [WORKFLOW](../governance/workflow.md) · [CLAUDE](../claude.md) · [AGENTS](../definitions/agents.md) · [brain](../.decisions/brain.md) · [TODOS index](../ops/todos/index.md)

> **Tek cumlelik sorumluluk:** Yeni uretilen belgelerin sifirdan yazildigini (kopya yok) ve bicim sozlesmesine uygun oldugunu denetler.

## §0 Ozet (Answer First)

- **Kullanim:** ilgili is yapilmadan once tek tek isaretlenir; **1 madde bossa RED**.
- **Kapatan:** kullanici (G12); bu liste yalniz kanit hazirlar.
- **Sira:** bu liste [WORKFLOW](../governance/workflow.md) §1 gate sirasinda ilgili kapidan **once** calistirilir; sirasiz calistirma kapiyi beklemez.
- **Cikti:** isaretli liste + §4 kanit kutusu; kanitsiz "gecti" denemez (G5).

### §0.1 Calistirma zamanlamasi

| Alan | Deger |
|---|---|
| Ne zaman | ilgili is baslamadan **once** (on-kontrol) ya da kapi kapanisinda (kanit) |
| Kim hazirlar | isin ureticisi (matristeki ajan — [AGENTS](../definitions/agents.md) §2) |
| Kim kapatir | **yalniz kullanici** (G12) |
| Suresi asilirsa | is durur + [log](../ops/log.md) raporu (WORKFLOW §2, 20 dk esigi) |
| Kanit kutusu | §4 — 4 satir, `EKSIK` bos birakilmaz |

## §1 Kabul Kriterleri (6 madde)

- [ ] **1.** Icerik **sifirdan** yazilmis (eski vaulttan yapistirma yok, G1)
- [ ] **2.** Baskasiyla birebir ayni blok **0** (benzerlik taramasi)
- [ ] **3.** `C:\www\coremusic...\` vaultuna **yazma** (ADR-024 — yalnizca yapi referansi)
- [ ] **4.** ASCII kutu `U+2500-U+257F` **0 satir** (ADR-029)
- [ ] **5.** Frontmatter 11 alan + `authority: primary`
- [ ] **6.** Yer tutucu (WIP/TBD/~) **0** (G4)

## §2 Yakalama (komut / tarama)

| # | Komut | Beklenen cikti |
|---|---|---|
| 1 | `Select-String -Path .ai/**/*.md -Pattern "[U+2500..U+257F]"` | 0 satir |
| 2 | `pwsh -File .ai/scripts/measure-vault.ps1` | NoFrontmatter = 0 |
| 3 | `git status --porcelain` | yalniz beklenen .ai/ girdileri (G1) |
| 4 | `benzerlik taramasi (yabanci blok)` | 0 eslesme |

## §3 RED Nedenleri

| # | Neden | Sonuc |
|---|---|---|
| 1 | Yabanci/kopya blok bulundu | G1 -> dosya silinir, sifirdan yazilir |
| 2 | Unicode kutu cizim | ADR-029/G4 -> ASCII ile yeniden cizilir |
| 3 | CoreMusic vaultuna yazma | ADR-024 + G1 cift ihlal |

## §4 Kanit Kutusu (WORKFLOW §2)

```text
KANIT   : <komut> -> <sayilar>
DOSYA   : <yazilan dosyalar>
EKSIK   : YOK ya da DOĞRULAMA GEREKLI
SONRAKI : <adim, ~ sure>
```

## §5 Kaynak (turetilme dayanagi)

- [brain](../.decisions/brain.md) §1.1 **ADR-002 + ADR-024 + ADR-029 yakalama: `documentation-check`**
- [CLAUDE](../claude.md) G1 · G4 · G7

> **Not:** Icerik vault belgelerinden **turetilmistir** (G9); kaynakta olmayan madde uydurulmaz.

---

## §6 Iliskili Dosyalar

[index](index.md) · [WORKFLOW](../governance/workflow.md) · [CLAUDE](../claude.md) · [AGENTS](../definitions/agents.md) · [brain](../.decisions/brain.md) · [PROJECTS](../definitions/projects.md) · [keys](../governance/keys.md) · [glossary](../definitions/glossary.md) · [log](../ops/log.md) · [TODOS index](../ops/todos/index.md) · [ADR-031](../.decisions/draft/adr-031-visual-production-provider.md)

### §6.1 Ayni gecisten gecen diger checklist'ler

Sert kapi on kontrolleri (hepsi kapi kapanisindan **once** calisir; birinde RED varsa siradaki kapi acilmaz):

- [gate-004a-vault-kapanis](gate-004a-vault-kapanis.md) — vault olcumu (GATE-004a)
- [gate-003-ui-onay](gate-003-ui-onay.md) — 7 wireframe + kullanici onayi (GATE-003, HARD STOP)
- [gate-004-ilk-commit](gate-004-ilk-commit.md) — cift bacak build + ilk commit (GATE-004)
- [gate-005-test-kapsami](gate-005-test-kapsami.md) — test + NetArchTest (GATE-005)
- [layer-boundary-check](layer-boundary-check.md) — katman siniri 13 soru

**Ortak kural:** bu checklist'teki bir madde digerinde de RED uretiyorsa **once en alttaki kok neden** kapatilir; yuzey duzeltmesi kapiyi acmaz (CLAUDE G5 — kanitsiz gecis yok).
