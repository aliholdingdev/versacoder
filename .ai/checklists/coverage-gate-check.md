---
title: "Test Kapsam (Coverage) Kapisi"
type: checklist
category: checklists
status: active
version: 1.0.0
updated: 2026-10-07
authority: reference
scope: "GATE-005 kapsam kapisi — coverage bandi, atlanan test 0, gerekce zorunlulugu"
tags: [checklist]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# Test Kapsam (Coverage) Kapisi

**Zorunlu Baglantilar:** [[checklists/index|checklist index]] · [[index|vault index]] · [[governance/workflow|WORKFLOW]] · [[claude|CLAUDE]] · [[definitions/agents|AGENTS]] · [[.decisions/brain|brain]] · [[ops/todos/index|TODOS index]]

> **Tek cumlelik sorumluluk:** Coverage hedefinin karsilanip karsilanmadigini ve gerekceli istisnalarin yazili oldugunu denetler.

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

- [ ] **1.** Coverage hedefi `spec/test-strategy.md` bandidir (hedef %90+, min %80)
- [ ] **2.** Altindaysa **yazili gerekce** `EKSIK` satirinda; gerekcesiz yuzde kabul edilmez
- [ ] **3.** Atlanan test (`Skip` / `Assert.Ignore`) **0**
- [ ] **4.** Test edilmemis kod "verified" **olarak gecmemis** (G5)
- [ ] **5.** Iki bacakta (`net48` + `net10.0-windows`) da test kosulmus
- [ ] **6.** Mimari testler (NetArchTest) kapsam disi DEGIL, ayrica yesil

## §2 Yakalama (komut / tarama)

| # | Komut | Beklenen cikti |
|---|---|---|
| 1 | `dotnet test --framework net48` | Gecen=N · Basarisiz=0 |
| 2 | `dotnet test --framework net10.0-windows` | Gecen=N · Basarisiz=0 |
| 3 | `Select-String -Path tests/**/*.cs -Pattern "Skip|Assert.Ignore"` | 0 satir |
| 4 | `coverage ciktisi (rapor)` | band icinde ya da gerekce yazilmis |

## §3 RED Nedenleri

| # | Neden | Sonuc |
|---|---|---|
| 1 | Coverage bandin altinda, gerekcesiz | Kanitsiz yuzde (G5) -> kapi RED |
| 2 | Atlanan test var | RED -> test acilir ya da gerekce yazilir |
| 3 | Tek bacakta test | G6/ADR-019 -> kapi acik |

## §4 Kanit Kutusu (WORKFLOW §2)

```text
KANIT   : <komut> -> <sayilar>
DOSYA   : <yazilan dosyalar>
EKSIK   : YOK ya da DOĞRULAMA GEREKLI
SONRAKI : <adim, ~ sure>
```

## §5 Kaynak (turetilme dayanagi)

- [[governance/workflow|WORKFLOW]] §2.4 (GATE-005 kabul kriteri 4-5)
- [[.decisions/brain|brain]] §1.1 ADR-019 yakalama: `dual-tfm-build-check`, `gate-005-test-kapsami`
- [[claude|CLAUDE]] G5 (kanitsiz bitti yasagi)

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
