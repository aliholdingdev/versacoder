---
title: "GATE-004a — Vault Kapanisi"
type: checklist
category: checklists
status: active
version: 1.0.0
updated: 2026-10-07
authority: SSOT
scope: "GATE-004a vault green — measure/check-links/find-mojibake/count-components, 500 MD esigi, frontmatter 8 alan"
---

# GATE-004a — Vault Kapanisi

**Zorunlu Baglantilar:** [checklist index](index.md) · [vault index](../index.md) · [WORKFLOW](../governance/workflow.md) · [CLAUDE](../claude.md) · [AGENTS](../definitions/agents.md) · [brain](../.decisions/brain.md) · [TODOS index](../ops/todos/index.md)

> **Tek cumlelik sorumluluk:** Vaultun yesil olup olmadigi (olcum sayaclari) gecmeden once kanitlar.

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

- [ ] **1.** `measure-vault.ps1` -> `TotalMD` hedefin ustunde, `NoFrontmatter = 0`
- [ ] **2.** `check-links.ps1` -> `BOZUK link = 0` ve surec 0 cikis koduyla biter
- [ ] **3.** `find-mojibake.ps1` -> `Mojibake file count: 0`
- [ ] **4.** `count-components.ps1` -> `PASS`; bilesen kaydi ADR-028 §3 olcumune esittir (marj 0)
- [ ] **5.** Kok belgeler disktedir; her birinin ilk satiri `---`, `authority` alani doludur
- [ ] **6.** Diyagramlarda `U+2500-U+257F` taramasi = 0 (ASCII kutu karari)
- [ ] **7.** Ciktisiz "vault hazir" beyani YOK (G5)

## §2 Yakalama (calistirilacak komut)

| # | Komut / tarama | Beklenen cikti |
|---|---|---|
| 1 | `pwsh -File .ai/scripts/measure-vault.ps1` | TotalMD / NoFrontmatter / Mojibake |
| 2 | `pwsh -File .ai/scripts/check-links.ps1` | BOZUK = 0 |
| 3 | `pwsh -File .ai/scripts/find-mojibake.ps1` | 0 dosya |
| 4 | `pwsh -File .ai/scripts/count-components.ps1` | PASS (>=5000, format 0) |
| 5 | `Select-String -Path .ai/**/*.md -Pattern "[U+2500..U+257F]"` | 0 satir |

## §3 RED Nedenleri

| # | Neden | Sonuc |
|---|---|---|
| 1 | Link verilen hedef diskte yok | BOZUK > 0 -> G4 ihlali, kapi acik kalir |
| 2 | Frontmatter eksik / 8 alan degil | NoFrontmatter > 0 -> dosya tamamlanir, sayac yeniden |
| 3 | Kayit CP1254/latin karisimi | Mojibake > 0 -> UTF-8 ile yeniden yazilir |
| 4 | Eski vaulttan kopya izi (G1) | git status icinde .ai/ disi degisiklik -> dosya silinir |
| 5 | Ciktisiz "vault hazir" | Beyan gecersiz, kapi RED (G5) |

## §4 Kanit Kutusu (WORKFLOW §2)

```text
KANIT   : <komut> -> <sayilar>
DOSYA   : <yazilan dosyalar>
EKSIK   : YOK ya da DOĞRULAMA GEREKLI
SONRAKI : <adim, ~ sure>
```

## §5 Kaynak (bu liste nereden turetilmistir)

- [WORKFLOW](../governance/workflow.md) §2.1 (5 kabul kriteri + 5 kirmizi neden + 4 satir kanit kutusu)
- [CLAUDE](../claude.md) §B10 (olcum komutlari) · G4 · G5 · G1
- [brain](../.decisions/brain.md) §1.1 ADR-003/022/027 yakalama satirlari
- **Turetme notu:** WORKFLOW "14 maddelik soru listesi" der ama maddeleri yazmaz -> 7 madde kabul kriterlerinden turetildi; 14 ile fark `DOĞRULAMA GEREKLI`

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
