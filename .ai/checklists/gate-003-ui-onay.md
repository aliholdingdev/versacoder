---
title: "GATE-003 — UI Onayi (HARD STOP)"
type: checklist
category: checklists
status: active
version: 1.0.0
updated: 2026-10-07
authority: reference
scope: "GATE-003 UI approval — HARD STOP, 7 wireframe, DevExpress v26.1, RibbonForm, kullanici onayi"
tags: [checklist]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# GATE-003 — UI Onayi (HARD STOP)

**Zorunlu Baglantilar:** [checklist index](index.md) · [vault index](../index.md) · [WORKFLOW](../governance/workflow.md) · [CLAUDE](../claude.md) · [AGENTS](../definitions/agents.md) · [brain](../.decisions/brain.md) · [TODOS index](../ops/todos/index.md)

> **Tek cumlelik sorumluluk:** 7 wireframein onayli olup olmadigini ve onaysiz .cs yazilmadigini denetler.

## §0 Ozet (Answer First)

- **Kullanim:** bu gecisten once yukaridan asagi tek tek isaretlenir; **1 madde bile bossa RED**.
- **Kapatan:** kullanici — bu liste yalniz kanit hazirlar (G12).
- **Kanit bicimi:** [WORKFLOW](../governance/workflow.md) §2 4 satir (`KANIT/DOSYA/EKSIK/SONRAKI`).
- **Sira:** [WORKFLOW](../governance/workflow.md) §1 gate sirasinda ilgili kapidan **once** calistirilir.

### §0.1 Calistirma zamanlamasi

| Alan | Deger |
|---|---|
| Ne zaman | ilgili is baslamadan **once** (on-kontrol) ya da kapi kapanisinda (kanit) |
| Kim hazirlar | isin ureticisi (matristeki ajan — [AGENTS](../definitions/agents.md) §2) |
| Kim kapatir | **yalniz kullanici** (G12) |
| Suresi asilirsa | is durur + [log](../ops/log.md) raporu (WORKFLOW §2, 20 dk esigi) |
| Kanit kutusu | §4 — 4 satir, `EKSIK` bos birakilmaz |

## §1 Kabul Kriterleri (7 madde)

- [ ] **1.** `ui-design/01..07` — **7/7** dosya diskte, her biri 8 alanli frontmatterli
- [ ] **2.** Wireframe ve mimari diyagramlarda Unicode kutu cizim **0** satir
- [ ] **3.** Envanterdeki her kontrol yalniz DevExpress **v26.1**; surum disi satir 0
- [ ] **4.** Ana pencere `RibbonForm`, ribbon stili **yalniz** `RibbonControlStyle.Office2010`
- [ ] **5.** Kullanicinin **acik, yazili** onayi `log.md`ye tarihli satir olarak dustu
- [ ] **6.** Eslikci dosyalar var: `component-inventory.md` + `flows.md`
- [ ] **7.** Ajan onayi **kendi kendine vermemis** (G12)

## §2 Yakalama (calistirilacak komut)

| # | Komut / tarama | Beklenen cikti |
|---|---|---|
| 1 | `Get-ChildItem .ai/ui-design -Filter *.md` | 7/7 dosya (+ README, inventory, flows) |
| 2 | `Select-String -Path .ai/ui-design/*.md -Pattern "U+2500..U+257F"` | 0 satir |
| 3 | `Select-String -Path .ai/ui-design/*.md -Pattern "DevExpress|v26."` | yalniz 26.1 |
| 4 | `Select-String -Path src/**/*.cs -Pattern "RibbonForm|RibbonControlStyle"` | yalniz Office2010 |
| 5 | `Select-String -Path .ai/log.md -Pattern "GATE-003"` | tarihli ONAY satiri |

## §3 RED Nedenleri

| # | Neden | Sonuc |
|---|---|---|
| 1 | Onaysiz tek .cs yazilmis | G12 — dosya geri alinir, kapi acik sayilir |
| 2 | Wireframe eksik (7den az) | Kapi kapali kalir, kod istemi reddedilir |
| 3 | Ajan onayi kendi kendine vermis | Onay gecersiz -> GATE-OPEN ile kullaniciya gidilir |
| 4 | Onaydan sonra ekran degistirilmis | Ilgili kod geri alinir, kapi yeniden acilir |
| 5 | v26.1 disi kontrol / Office2010 disi stil | G3 CRITICAL -> RED |

## §4 Kanit Kutusu (WORKFLOW §2)

```text
KANIT   : <komut> -> <sayilar>
DOSYA   : <yazilan dosyalar>
EKSIK   : YOK ya da DOĞRULAMA GEREKLI
SONRAKI : <adim, ~ sure>
```

## §5 Kaynak (bu liste nereden turetilmistir)

- [WORKFLOW](../governance/workflow.md) §2.2 (5 kabul kriteri + 7 wireframe tablosu + HARD STOP gerekceleri)
- [CLAUDE](../claude.md) G3 (DevExpress v26.1/Office2010) · G12 (gate disiplini)
- [brain](../.decisions/brain.md) §1.1 ADR-004/015/025 yakalama satirlari
- [VISION](../definitions/vision.md) §2 (7 ekran) · §5 (Non-Goals)
- **Turetme notu:** WORKFLOW "13 adimlik" liste der; adimlarin kendisi yazilmamis -> 7 madde kabul kriterlerinden turetildi, fark `DOĞRULAMA GEREKLI`

> **Not:** Bu liste vault belgelerinden **turetilmistir**; kaynakta olmayan madde uydurulmaz (G9). Turetilmis sayi ile kaynak arasindaki fark varsa `DOĞRULAMA GEREKLI` isaretlenir.

---

## §6 Iliskili Dosyalar

[index](index.md) · [WORKFLOW](../governance/workflow.md) · [CLAUDE](../claude.md) · [AGENTS](../definitions/agents.md) · [brain](../.decisions/brain.md) · [PROJECTS](../definitions/projects.md) · [VISION](../definitions/vision.md) · [keys](../governance/keys.md) · [glossary](../definitions/glossary.md) · [log](../ops/log.md) · [TODOS index](../ops/todos/index.md) · [ADR-031](../.decisions/draft/adr-031-visual-production-provider.md)

### §6.1 Ayni gecisten gecen diger checklist'ler

Sert kapi on kontrolleri (hepsi kapi kapanisindan **once** calisir; birinde RED varsa siradaki kapi acilmaz):

- [gate-004a-vault-kapanis](gate-004a-vault-kapanis.md) — vault olcumu (GATE-004a)
- [gate-003-ui-onay](gate-003-ui-onay.md) — 7 wireframe + kullanici onayi (GATE-003, HARD STOP)
- [gate-004-ilk-commit](gate-004-ilk-commit.md) — cift bacak build + ilk commit (GATE-004)
- [gate-005-test-kapsami](gate-005-test-kapsami.md) — test + NetArchTest (GATE-005)
- [layer-boundary-check](layer-boundary-check.md) — katman siniri 13 soru

**Ortak kural:** bu checklist'teki bir madde digerinde de RED uretiyorsa **once en alttaki kok neden** kapatilir; yuzey duzeltmesi kapiyi acmaz (CLAUDE G5 — kanitsiz gecis yok).
