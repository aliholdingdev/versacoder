---
title: "Structured Log ve Gozlemlenebilirlik Kontrolu"
type: checklist
category: checklists
status: active
version: 1.0.0
updated: 2026-10-07
authority: SSOT
scope: "ADR-026 — structured log, 5 seviye, iki bacakta ayni alan adlari"
---

# Structured Log ve Gozlemlenebilirlik Kontrolu

**Zorunlu Baglantilar:** [checklist index](index.md) · [vault index](../index.md) · [WORKFLOW](../WORKFLOW.md) · [CLAUDE](../CLAUDE.md) · [AGENTS](../AGENTS.md) · [brain](../brain.md) · [TODOS index](../TODOS/index.md)

> **Tek cumlelik sorumluluk:** Loglarin tek sozlesmede, 5 seviyede ve iki bacakta ayni alan adlariyla yazildigini denetler.

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

- [ ] **1.** Tek structured sozlesme; serbest metin log **0**
- [ ] **2.** Seviyeler yalniz `Trace/Debug/Info/Warn/Error` (disi uydurulmaz)
- [ ] **3.** Iki bacakta **ayni alan adlari**
- [ ] **4.** Log kaydi `log.md` uzerinde ve dosyada kalici
- [ ] **5.** Kritik islemler audit'e dusuyor (secret hariç)
- [ ] **6.** Gozlemlenebilirlik: log + olcum + iz birlikte degerlendirilmis

## §2 Yakalama (komut / tarama)

| # | Komut | Beklenen cikti |
|---|---|---|
| 1 | `Select-String -Path src/**/*.cs -Pattern "LogInformation\(\"[^\{]*\""` | serbest metin log 0 |
| 2 | `Select-String -Path src/**/*.cs -Pattern "Log(Trace|Debug|Information|Warning|Error)"` | yalniz 5 seviye |
| 3 | `log seviye adi taramasi` | Trace/Debug/Info/Warn/Error disinda 0 |
| 4 | `pwsh -File .ai/scripts/check-links.ps1 (log referanslari)` | BOZUK = 0 |

## §3 RED Nedenleri

| # | Neden | Sonuc |
|---|---|---|
| 1 | Serbest metin log | ADR-026 RED -> sozlesmeye cevrilir |
| 2 | 5 seviye disi seviye adi | uydurma -> RED |
| 3 | Iki bacakta farkli alan adi | G6 -> ayni sozlesme zorunlu |

## §4 Kanit Kutusu (WORKFLOW §2)

```text
KANIT   : <komut> -> <sayilar>
DOSYA   : <yazilan dosyalar>
EKSIK   : YOK ya da DOĞRULAMA GEREKLI
SONRAKI : <adim, ~ sure>
```

## §5 Kaynak (turetilme dayanagi)

- [brain](../brain.md) §1.1 **ADR-026 yakalama: `logging-observability-check`**
- [CLAUDE](../CLAUDE.md) G5 · G6

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
