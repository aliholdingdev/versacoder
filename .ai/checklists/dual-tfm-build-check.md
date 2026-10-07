---
title: "Cift Bacak (Dual TFM) Build Kontrolu"
type: checklist
category: checklists
status: active
version: 1.0.0
updated: 2026-10-07
authority: SSOT
scope: "ADR-001 + ADR-019 — cift TFM, #if yalniz platform, iki bacakta ayni test"
---

# Cift Bacak (Dual TFM) Build Kontrolu

**Zorunlu Baglantilar:** [checklist index](index.md) · [vault index](../index.md) · [WORKFLOW](../WORKFLOW.md) · [CLAUDE](../CLAUDE.md) · [AGENTS](../AGENTS.md) · [brain](../brain.md) · [TODOS index](../TODOS/index.md)

> **Tek cumlelik sorumluluk:** Ayni kodun hem net48 hem net10.0-windows bacaklarinda 0 hata ile derlendigini denetler.

## §0 Ozet (Answer First)

- **Kullanim:** ilgili is yapilmadan once tek tek isaretlenir; **1 madde bossa RED**.
- **Kapatan:** kullanici (G12); bu liste yalniz kanit hazirlar.
- **Sira:** bu liste [WORKFLOW](../WORKFLOW.md) §1 gate sirasinda ilgili kapidan **once** calistirilir; sirasiz calistirma kapiyi beklemez.
- **Cikti:** isaretli liste + §4 kanit kutusu; kanitsiz "gecti" denemez (G5).

### §0.1 Calistirma zamanlamasi

| Alan | Deger |
|---|---|
| Ne zaman | ilgili is baslamadan **once** (on-kontrol) ya da kapi kapanisinda (kanit) |
| Kim hazirlar | isin ureticisi (matristeki ajan — [AGENTS](../AGENTS.md) §2) |
| Kim kapatir | **yalniz kullanici** (G12) |
| Suresi asilirsa | is durur + [log](../log.md) raporu (WORKFLOW §2, 20 dk esigi) |
| Kanit kutusu | §4 — 4 satir, `EKSIK` bos birakilmaz |

## §1 Kabul Kriterleri (6 madde)

- [ ] **1.** `dotnet build -f net48` -> **0 hata**
- [ ] **2.** `dotnet build -f net10.0-windows` -> **0 hata**
- [ ] **3.** `#if` yalniz platform/API farkinda; **is mantigi bolunmemis** (G6)
- [ ] **4.** `dotnet test` iki bacakta da kosulmus, test sayilari esit
- [ ] **5.** Yeni paket **her iki TFM** icinde restore ediliyor (NU1202 riski yok)
- [ ] **6.** Tanimlanmis tek TFM ile "bitti" **denmemis** (ADR-019)

## §2 Yakalama (komut / tarama)

| # | Komut | Beklenen cikti |
|---|---|---|
| 1 | `dotnet build -f net48` | 0 hata |
| 2 | `dotnet build -f net10.0-windows` | 0 hata |
| 3 | `dotnet test --framework net48 + --framework net10.0-windows` | 0 basarisiz, sayilar esit |
| 4 | `Select-String -Path src/**/*.cs -Pattern "#if"` | yalniz platform/API farki |

## §3 RED Nedenleri

| # | Neden | Sonuc |
|---|---|---|
| 1 | Tek bacakta yesil | ADR-019/G6 RED -> ikinci bacak calistirilir |
| 2 | #if icinde is mantigi | HIGH — mimari RED |
| 3 | NU1202 / uyumsuz paket | paket iki bacakta da eklenmeli |

## §4 Kanit Kutusu (WORKFLOW §2)

```text
KANIT   : <komut> -> <sayilar>
DOSYA   : <yazilan dosyalar>
EKSIK   : YOK ya da DOĞRULAMA GEREKLI
SONRAKI : <adim, ~ sure>
```

## §5 Kaynak (turetilme dayanagi)

- [brain](../brain.md) §1.1 **ADR-001 + ADR-019 yakalama: `dual-tfm-build-check`**
- [CLAUDE](../CLAUDE.md) G6 (tek bacakli tasarim yasagi) · §B10
- [PROJECTS](../PROJECTS.md) §2.1 (iki bacak matrisi)

> **Not:** Icerik vault belgelerinden **turetilmistir** (G9); kaynakta olmayan madde uydurulmaz.

---

## §6 Iliskili Dosyalar

[index](index.md) · [WORKFLOW](../WORKFLOW.md) · [CLAUDE](../CLAUDE.md) · [AGENTS](../AGENTS.md) · [brain](../brain.md) · [PROJECTS](../PROJECTS.md) · [keys](../keys.md) · [glossary](../glossary.md) · [log](../log.md) · [TODOS index](../TODOS/index.md) · [ADR-031](../.decisions/draft/ADR-031-visual-production-provider.md)

### §6.1 Ayni gecisten gecen diger checklist'ler

Sert kapi on kontrolleri (hepsi kapi kapanisindan **once** calisir; birinde RED varsa siradaki kapi acilmaz):

- [gate-004a-vault-kapanis](gate-004a-vault-kapanis.md) — vault olcumu (GATE-004a)
- [gate-003-ui-onay](gate-003-ui-onay.md) — 7 wireframe + kullanici onayi (GATE-003, HARD STOP)
- [gate-004-ilk-commit](gate-004-ilk-commit.md) — cift bacak build + ilk commit (GATE-004)
- [gate-005-test-kapsami](gate-005-test-kapsami.md) — test + NetArchTest (GATE-005)
- [layer-boundary-check](layer-boundary-check.md) — katman siniri 13 soru

**Ortak kural:** bu checklist'teki bir madde digerinde de RED uretiyorsa **once en alttaki kok neden** kapatilir; yuzey duzeltmesi kapiyi acmaz (CLAUDE G5 — kanitsiz gecis yok).
