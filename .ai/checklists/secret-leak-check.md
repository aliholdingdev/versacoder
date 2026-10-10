---
title: "Secret / Anahtar Sizintisi Kontrolu"
type: checklist
category: checklists
status: active
version: 1.0.0
updated: 2026-10-07
authority: reference
scope: "ADR-011 + ADR-018 — DPAPI sifreli depo, keys.md yalniz harita, loga secret yazilmaz"
tags: [checklist]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# Secret / Anahtar Sizintisi Kontrolu

**Zorunlu Baglantilar:** [[checklists/index|checklist index]] · [[index|vault index]] · [[governance/workflow|WORKFLOW]] · [[claude|CLAUDE]] · [[definitions/agents|AGENTS]] · [[.decisions/brain|brain]] · [[ops/todos/index|TODOS index]]

> **Tek cumlelik sorumluluk:** Anahtar, token ve parolalarin kodda, repoda veya logda bulunmadigini denetler.

## §0 Ozet (Answer First)

- **Kullanim:** ilgili is yapilmadan once tek tek isaretlenir; **1 madde bossa RED**.
- **Kapatan:** kullanici (G12); bu liste yalniz kanit hazirlar.
- **Sira:** bu liste [[governance/workflow|WORKFLOW]] §1 gate sirasinda ilgili kapidan **once** calistirilir; sirasiz calistirma kapiyi beklemez.
- **Cikti:** isaretli liste + §4 kanit kutusu; kanitsiz "gecti" denemez (G5).

### §0.1 Calistirma zamanlamasi

| Alan | Deger |
|---|---|
| Ne zaman | ilgili is baslamadan **once** (on-kontrol) ya da kapi kapanisinda (kanit) |
| Kim hazirlar | isin ureticisi (matristeki ajan — [[definitions/agents|AGENTS]] §2) |
| Kim kapatir | **yalniz kullanici** (G12) |
| Suresi asilirsa | is durur + [[ops/log|log]] raporu (WORKFLOW §2, 20 dk esigi) |
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

- [[.decisions/brain|brain]] §1.1 **ADR-011 + ADR-018 yakalama: `secret-leak-check`**
- [[claude|CLAUDE]] G8 (yigin/teknoloji) · G11 (gizli islem)
- [[definitions/agents|AGENTS]] §4 (External Agent ortam izolasyonu)

> **Not:** Icerik vault belgelerinden **turetilmistir** (G9); kaynakta olmayan madde uydurulmaz.

---

## §6 Iliskili Dosyalar

[[checklists/index|index]] · [[governance/workflow|WORKFLOW]] · [[claude|CLAUDE]] · [[definitions/agents|AGENTS]] · [[.decisions/brain|brain]] · [[definitions/projects|PROJECTS]] · [[governance/keys|keys]] · [[definitions/glossary|glossary]] · [[ops/log|log]] · [[ops/todos/index|TODOS index]] · [[.decisions/draft/adr-031-visual-production-provider|ADR-031]]

### §6.1 Ayni gecisten gecen diger checklist'ler

Sert kapi on kontrolleri (hepsi kapi kapanisindan **once** calisir; birinde RED varsa siradaki kapi acilmaz):

- [[checklists/gate-004a-vault-kapanis|gate-004a-vault-kapanis]] — vault olcumu (GATE-004a)
- [[checklists/gate-003-ui-onay|gate-003-ui-onay]] — 7 wireframe + kullanici onayi (GATE-003, HARD STOP)
- [[checklists/gate-004-ilk-commit|gate-004-ilk-commit]] — cift bacak build + ilk commit (GATE-004)
- [[checklists/gate-005-test-kapsami|gate-005-test-kapsami]] — test + NetArchTest (GATE-005)
- [[checklists/layer-boundary-check|layer-boundary-check]] — katman siniri 13 soru

**Ortak kural:** bu checklist'teki bir madde digerinde de RED uretiyorsa **once en alttaki kok neden** kapatilir; yuzey duzeltmesi kapiyi acmaz (CLAUDE G5 — kanitsiz gecis yok).
