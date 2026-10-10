---
title: "Migration (Saf SQL) Kontrolu"
type: checklist
category: checklists
status: active
version: 1.0.0
updated: 2026-10-07
authority: reference
scope: "ADR-005 — migration = saf SQL + schema_version; ORM migration yasak"
tags: [checklist]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# Migration (Saf SQL) Kontrolu

**Zorunlu Baglantilar:** [[checklists/index|checklist index]] · [[index|vault index]] · [[governance/workflow|WORKFLOW]] · [[claude|CLAUDE]] · [[definitions/agents|AGENTS]] · [[.decisions/brain|brain]] · [[ops/todos/index|TODOS index]]

> **Tek cumlelik sorumluluk:** Migration dosyalarinin saf SQL + version tablosu kuralina uygun oldugunu denetler.

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

- [ ] **1.** Migration dosyalari **saf SQL** (ORM migration degil)
- [ ] **2.** `schema_version` tablosu var ve her dosya kayitli
- [ ] **3.** Dosya sirasi belirli (numarali dosya adlari)
- [ ] **4.** Her migration icin **birim test** (dosya basina)
- [ ] **5.** Geri alinabilirlik degerlendirilmis
- [ ] **6.** Checksum / imza tutarli

## §2 Yakalama (komut / tarama)

| # | Komut | Beklenen cikti |
|---|---|---|
| 1 | `Select-String -Path .ai/.sql/** -Pattern "schema_version"` | var |
| 2 | `Select-String -Path **/*.cs -Pattern "Migrate\(\)"|Database.Migrate` | 0 satir (ORM migration yok) |
| 3 | `Get-ChildItem .ai/.sql/*.sql | Sort-Object Name` | sirali dosya adlari |

## §3 RED Nedenleri

| # | Neden | Sonuc |
|---|---|---|
| 1 | ORM migration kullanilmis | G2 -> saf SQL'e cevrilir |
| 2 | schema_version yok | surum takibi kaybolur -> RED |
| 3 | Sira belirsiz | migration sureci durur |

## §4 Kanit Kutusu (WORKFLOW §2)

```text
KANIT   : <komut> -> <sayilar>
DOSYA   : <yazilan dosyalar>
EKSIK   : YOK ya da DOĞRULAMA GEREKLI
SONRAKI : <adim, ~ sure>
```

## §5 Kaynak (turetilme dayanagi)

- [[.decisions/brain|brain]] §1.1 **ADR-005 yakalama: `data-access-check`, `migration-check`**
- [[claude|CLAUDE]] G2 (EF yasagi + saf SQL)

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
