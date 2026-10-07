---
title: "Vault Link Sagligi Kontrolu"
type: checklist
category: checklists
status: active
version: 1.0.0
updated: 2026-10-07
authority: reference
scope: "ADR-003 + ADR-021 — bozuk link 0, her dizinde en fazla bir liste, frontmatter 8 alan"
tags: [checklist]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# Vault Link Sagligi Kontrolu

**Zorunlu Baglantilar:** [checklist index](index.md) · [vault index](../index.md) · [WORKFLOW](../governance/workflow.md) · [CLAUDE](../claude.md) · [AGENTS](../definitions/agents.md) · [brain](../.decisions/brain.md) · [TODOS index](../ops/todos/index.md)

> **Tek cumlelik sorumluluk:** Vault linklerinin hedefe gittigini ve her dizinde tek indeks bulundugunu denetler.

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

- [ ] **1.** `check-links.ps1` -> **BOZUK = 0**
- [ ] **2.** Her dizinde **en fazla bir** indeks/liste dosyasi (ADR-021 SRP)
- [ ] **3.** Link hedefi envanterde (index §2) kayitli
- [ ] **4.** Frontmatter 11 alan, ilk satir `---`
- [ ] **5.** Bozuk hedefe link **silinmemis, duzeltilmis** (hedefe bagli kural)
- [ ] **6.** Mojibake 0 (bozuk byte link metnini de bozar)

## §2 Yakalama (komut / tarama)

| # | Komut | Beklenen cikti |
|---|---|---|
| 1 | `pwsh -File .ai/scripts/check-links.ps1` | BOZUK = 0 |
| 2 | `pwsh -File .ai/scripts/find-mojibake.ps1` | 0 dosya |
| 3 | `pwsh -File .ai/scripts/measure-vault.ps1` | NoFrontmatter = 0 |
| 4 | `Test-Path <her link hedefi>` | True |

## §3 RED Nedenleri

| # | Neden | Sonuc |
|---|---|---|
| 1 | BOZUK > 0 | G4 -> GATE-004a kapali kalir |
| 2 | Ayni dizinde 2 indeks | ADR-021 ihlali -> birlestirilir |
| 3 | Frontmatter eksik | dosya vaulta ait degil -> tamamlanir |

## §4 Kanit Kutusu (WORKFLOW §2)

```text
KANIT   : <komut> -> <sayilar>
DOSYA   : <yazilan dosyalar>
EKSIK   : YOK ya da DOĞRULAMA GEREKLI
SONRAKI : <adim, ~ sure>
```

## §5 Kaynak (turetilme dayanagi)

- [brain](../.decisions/brain.md) §1.1 **ADR-003 + ADR-021 yakalama: `vault-link-check`**
- [CLAUDE](../claude.md) G4 · §B10 (olcum komutlari)
- [WORKFLOW](../governance/workflow.md) §2.1 (GATE-004a kabul kriteri 2)

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
