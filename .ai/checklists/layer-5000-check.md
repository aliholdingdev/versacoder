---
title: "5000+ Katman Kurulum Kontrolu (ADR-030)"
type: checklist
category: checklists
status: active
version: 1.0.0
updated: 2026-10-07
authority: reference
scope: "ADR-030 v2 — 4 kademe, 3 eksen, yon grubu, katalog ID + 8 sutun, esik 5000"
tags: [checklist]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# 5000+ Katman Kurulum Kontrolu (ADR-030)

**Zorunlu Baglantilar:** [[checklists/index|checklist index]] · [[index|vault index]] · [[governance/workflow|WORKFLOW]] · [[claude|CLAUDE]] · [[definitions/agents|AGENTS]] · [[.decisions/brain|brain]] · [[ops/todos/index|TODOS index]]

> **Tek cumlelik sorumluluk:** 5000+ tip/katman kurgusunun ADR-030 v2 kosullarina uyup uymadigini denetler.

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

## §1 Kabul Kriterleri (7 madde)

- [ ] **1.** Katalog kayit sayisi **>= 5000** ve hatali format **0** (`count-components.ps1`)
- [ ] **2.** Dugum sayisi 44 alt + 8 makro (52) ile tutarli; ikinci sayim birimi **uretilmemis**
- [ ] **3.** Yon grubu 8/12/16/20 (kademe) ile kayitli; `l_i -> l_j` izinli <=> `j <= i`
- [ ] **4.** Tip (K) / fiziksel dosya (Y) / dugum (N) **uc eksen ayri** raporlanir (birlestirilmez)
- [ ] **5.** K5000 DLL listesinde 7 bant (K0000..K5499) + 5 faz (K0 + N1..N4) tanimli
- [ ] **6.** Kritik acik konular `DOĞRULAMA GEREKLI` ile isaretli (N4 dagilimi, restore olcumu)
- [ ] **7.** GATE-003 onayi alinmadan `src/**` kodu YAZILMAMIS (G12)

## §2 Yakalama (komut / tarama)

| # | Komut | Beklenen cikti |
|---|---|---|
| 1 | `pwsh .ai/scripts/count-components.ps1` | PASS · >= 5000 · format 0 |
| 2 | `pwsh .ai/scripts/count-dll.ps1 (varsa)` | uretim .csproj sayaci |
| 3 | `Test-Path .ai/.decisions/accepted/ADR-030*` | ADR dosyasi var mi |
| 4 | `Get-ChildItem src -Filter *.csproj -Recurse | Measure-Object` | kod yoksa 0 (G12 temiz) |

## §3 RED Nedenleri

| # | Neden | Sonuc |
|---|---|---|
| 1 | Esik altinda (<5000) | GATE-004a RED |
| 2 | Ikinci sayim birimi uretilmis (Tier-DLL sutunu) | marj negatife duser -> RED |
| 3 | GATE-003 onaysiz kod yazilmis | G12 -> geri alinir |
| 4 | ADR-030 dosyasi diskte yok | kaynak yok -> `DOĞRULAMA GEREKLI`, kod baslamaz |

## §4 Kanit Kutusu (WORKFLOW §2)

```text
KANIT   : <komut> -> <sayilar>
DOSYA   : <yazilan dosyalar>
EKSIK   : YOK ya da DOĞRULAMA GEREKLI
SONRAKI : <adim, ~ sure>
```

## §5 Kaynak (turetilme dayanagi)

- log kaydi (2026-10-02): `layer-5000-check.md` = **14 madde + durum tablosu** yazildi
- [[.decisions/brain|brain]] §1.1 ADR-030 (varsa) · ADR-027 (count-components)
- **Turetme notu:** orijinal 14 maddenin metni vaultta yok (dosya kayip) -> 7 madde ADR-030/log kayitlarindan turetildi; 14 ile fark `DOĞRULAMA GEREKLI`

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
