---
title: "WinForms Designer / Form Uclusu Kontrolu"
type: checklist
category: checklists
status: active
version: 1.0.0
updated: 2026-10-07
authority: reference
scope: "ADR-015 designer/resx sozlesmesi — form uclusu, ui-designer sahipligi, GATE-003"
tags: [checklist]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# WinForms Designer / Form Uclusu Kontrolu

**Zorunlu Baglantilar:** [checklist index](index.md) · [vault index](../index.md) · [WORKFLOW](../governance/workflow.md) · [CLAUDE](../claude.md) · [AGENTS](../definitions/agents.md) · [brain](../.decisions/brain.md) · [TODOS index](../ops/todos/index.md)

> **Tek cumlelik sorumluluk:** .Form.cs + .Designer.cs + .resx ucullusunun bozulmadigini ve Designerin elle ellenmedigini denetler.

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

- [ ] **1.** Her ekran icin `.Form.cs` + `.Form.Designer.cs` + `.resx` **3 dosya da var**
- [ ] **2.** `.Designer.cs` **elle duzeltilmemis** (yalniz tasarimcida acilmis) — diff incelenir
- [ ] **3.** `.Form.cs` icinde `Controls.Add` / manuel layout **yok** (layout Designer'a ait)
- [ ] **4.** `InitializeComponent` icinde is mantigi **yok**
- [ ] **5.** Ekran adlari `ui-design/01..07` ile birebir eslesiyor
- [ ] **6.** Onay zinciri tam: GATE-003 onayi -> kod (F3 akisi: 05 -> onay -> 06 -> 04)

## §2 Yakalama (komut / tarama)

| # | Komut | Beklenen cikti |
|---|---|---|
| 1 | `Get-ChildItem src/**/*.Form.*` | her ekran icin 3 dosya |
| 2 | `Select-String -Path src/**/*.Designer.cs -Pattern "InitializeComponent"` | yalniz tasarimci uretimi |
| 3 | `Select-String -Path src/**/*.Form.cs -Pattern "Controls.Add"` | 0 satir |

## §3 RED Nedenleri

| # | Neden | Sonuc |
|---|---|---|
| 1 | Designer elle duzeltilmis | Kaynak urun bozulur -> is geri alinir, ui-designer'a devredilir |
| 2 | Form uclusu eksik | Ekran acilmaz -> kod geri alinir |
| 3 | Onaysiz kod | G12/GATE-003 -> geri alinir |

## §4 Kanit Kutusu (WORKFLOW §2)

```text
KANIT   : <komut> -> <sayilar>
DOSYA   : <yazilan dosyalar>
EKSIK   : YOK ya da DOĞRULAMA GEREKLI
SONRAKI : <adim, ~ sure>
```

## §5 Kaynak (turetilme dayanagi)

- [brain](../.decisions/brain.md) §1.1 **ADR-015 yakalama noktasi: `winforms-designer-check`**
- [CLAUDE](../claude.md) G12 (yetki) · §5 (l7 yasaklari)
- [WORKFLOW](../governance/workflow.md) §2.2 (GATE-003 kabul kriteri)

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
