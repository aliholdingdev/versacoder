---
title: "Erisilebilirlik (WCAG) Kontrolu"
type: checklist
category: checklists
status: active
version: 1.0.0
updated: 2026-10-07
authority: reference
scope: "Erisilebilirlik kurallari — semantic HTML/kontrol, klavye, odak, kontrast (rules/accessibility)"
tags: [checklist]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# Erisilebilirlik (WCAG) Kontrolu

**Zorunlu Baglantilar:** [[checklists/index|checklist index]] · [[index|vault index]] · [[governance/workflow|WORKFLOW]] · [[claude|CLAUDE]] · [[definitions/agents|AGENTS]] · [[.decisions/brain|brain]] · [[ops/todos/index|TODOS index]]

> **Tek cumlelik sorumluluk:** Ekranlarin klavye ve ekran okuyucu ile kullanilabilir oldugunu denetler.

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

- [ ] **1.** Semantic yapilandirma (kontrol adlari anlamli)
- [ ] **2.** Klavye ile tam gezinme (Tab / Shift+Tab / Enter)
- [ ] **3.** Odak (focus) durumu gorunur
- [ ] **4.** Erisilebilir etiket: `AccessibleName` / `Text` dolu
- [ ] **5.** Yalniz renge bagimli bilgi **yok** (yanina metin etiketi)
- [ ] **6.** Kontrast yeterli (metin okunur)
- [ ] **7.** Dokunma/hedef boyutlari yeterli (masaustunde fare hedefi)

## §2 Yakalama (komut / tarama)

| # | Komut | Beklenen cikti |
|---|---|---|
| 1 | `Select-String -Path src/**/*.cs -Pattern "AccessibleName|AccessibleDescription"` | etiketler dolu |
| 2 | `odak/kaydirma taramasi (klavye)` | Tab sirasi mantikli |
| 3 | `ekran okuyucu ile kontrol (elle)` | okunabilir |

## §3 RED Nedenleri

| # | Neden | Sonuc |
|---|---|---|
| 1 | Odak gorunmez | erisilebilirlik RED -> duzeltilir |
| 2 | AccessibleName bos | ekran okuyucu okuyamaz -> RED |
| 3 | Yalniz renk ile bilgi | WCAG ihlali -> metin etiketi eklenir |

## §4 Kanit Kutusu (WORKFLOW §2)

```text
KANIT   : <komut> -> <sayilar>
DOSYA   : <yazilan dosyalar>
EKSIK   : YOK ya da DOĞRULAMA GEREKLI
SONRAKI : <adim, ~ sure>
```

## §5 Kaynak (turetilme dayanagi)

- [[.decisions/brain|brain]] §1.1 yakalama noktalari (evrensel)
- [[definitions/vision|VISION]] §0.3 (erisilebilirlik notlari)
- **Turetme notu:** vaultta `accessibility-rules.md` icerigi `.rules/` dizinindedir (diskte YOK) -> maddeler genel WCAG + VISION notlarindan turetildi; orijinal kural metni icin `DOĞRULAMA GEREKLI`

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
