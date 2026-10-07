---
title: "Secret / Anahtar Sizintisi Kontrolu"
type: checklist
category: checklists
status: active
version: 1.0.0
updated: 2026-10-07
authority: SSOT
scope: "ADR-011 + ADR-018 — DPAPI sifreli depo, keys.md yalniz harita, loga secret yazilmaz"
---

# Secret / Anahtar Sizintisi Kontrolu

**Zorunlu Baglantilar:** [checklist index](index.md) · [vault index](../index.md) · [WORKFLOW](../governance/workflow.md) · [CLAUDE](../claude.md) · [AGENTS](../definitions/agents.md) · [brain](../.decisions/brain.md) · [TODOS index](../ops/todos/index.md)

> **Tek cumlelik sorumluluk:** Anahtar, token ve parolalarin kodda, repoda veya logda bulunmadigini denetler.

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

- [ ] **1.** Kod/repoda `sk-`, `nvapi-`, `Bearer`, `api_key` gibi deger **0**
- [ ] **2.** Secret yalniz **DPAPI sifreli yerel depoda**; duz metin dosya yok
- [ ] **3.** `keys.md` **yalniz harita** (deger icermiyor)
- [ ] **4.** Log/audit kaydina secret **yazilmamis**
- [ ] **5.** Prompt icine secret **konulmamis** (external agent gonderiminde de)
- [ ] **6.** `.env` / config dosyasi repoya **girmemis**

## §2 Yakalama (komut / tarama)

| # | Komut | Beklenen cikti |
|---|---|---|
| 1 | `Select-String -Path src/** -Pattern "sk-|nvapi-|Bearer |api_key\s*="` | 0 satir |
| 2 | `Select-String -Path .ai/** -Pattern "sk-|nvapi-|Bearer [A-Za-z0-9]"` | 0 satir |
| 3 | `Get-Content .ai/keys.md | Select-String "=|:"` | deger yok, yalniz tanim |
| 4 | `git ls-files | Select-String "\.env"` | 0 satir |

## §3 RED Nedenleri

| # | Neden | Sonuc |
|---|---|---|
| 1 | Kod/repoda secret bulundu | CRITICAL -> derhal kaldir + anahtar yenilenir + log |
| 2 | Loga secret yazilmis | G8/G11 -> kayit temizlenir |
| 3 | Prompta secret konulmus | G7 -> onay + kaldirma |

## §4 Kanit Kutusu (WORKFLOW §2)

```text
KANIT   : <komut> -> <sayilar>
DOSYA   : <yazilan dosyalar>
EKSIK   : YOK ya da DOĞRULAMA GEREKLI
SONRAKI : <adim, ~ sure>
```

## §5 Kaynak (turetilme dayanagi)

- [brain](../.decisions/brain.md) §1.1 **ADR-011 + ADR-018 yakalama: `secret-leak-check`**
- [CLAUDE](../claude.md) G8 (yigin/teknoloji) · G11 (gizli islem)
- [AGENTS](../definitions/agents.md) §4 (External Agent ortam izolasyonu)

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
