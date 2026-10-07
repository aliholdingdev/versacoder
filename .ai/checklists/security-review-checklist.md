---
title: "Guvenlik Denetim Listesi"
type: checklist
category: checklists
status: active
version: 1.0.0
updated: 2026-10-07
authority: SSOT
scope: "ADR-018 — secret yonetimi + OWASP denetimi (CLAUDE G2/G8 baglanti)"
---

# Guvenlik Denetim Listesi

**Zorunlu Baglantilar:** [checklist index](index.md) · [vault index](../index.md) · [WORKFLOW](../governance/workflow.md) · [CLAUDE](../claude.md) · [AGENTS](../definitions/agents.md) · [brain](../.decisions/brain.md) · [TODOS index](../ops/todos/index.md)

> **Tek cumlelik sorumluluk:** Uygulama guvenligi kalemlerini OWASP odakli olarak denetler.

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

## §1 Kabul Kriterleri (8 madde)

- [ ] **1.** Giris dogrulama (input validation) tum girislerde
- [ ] **2.** Cikis kodlamasi (output encoding) — XSS onleyici
- [ ] **3.** Parametreli SQL (**concat = 0**, G2)
- [ ] **4.** CSRF/XSS/SQL Injection kalemleri gecti (OWASP Top 10)
- [ ] **5.** Secret yonetimi DPAPI uzerinden (ADR-018)
- [ ] **6.** Yetkilendirme (authorization) kontrol noktasi tanimli
- [ ] **7.** Rate limit / guvenlik header'lari degerlendirilmis
- [ ] **8.** Audit log kritik islemleri kaydediyor (secret hariç)

## §2 Yakalama (komut / tarama)

| # | Komut | Beklenen cikti |
|---|---|---|
| 1 | `Select-String -Path src/**/*.cs -Pattern "\"\s*\+\s*\w*(Id|Name)"` | 0 satir (concat SQL) |
| 2 | `Select-String -Path *.csproj,Directory.Packages.props -Pattern EntityFrameworkCore` | 0 satir (G2) |
| 3 | `secret taramasi (bkz. secret-leak-check)` | 0 |
| 4 | `OWASP kalemleri uzerinden inceleme` | her kalem PASS ya da bulgu + severity |

## §3 RED Nedenleri

| # | Neden | Sonuc |
|---|---|---|
| 1 | Concat SQL bulundu | G2 CRITICAL -> RED |
| 2 | Secret sizintisi | CRITICAL -> anahtar yenile |
| 3 | Yetkilendirme eksik | HIGH -> kod geri alinir |

## §4 Kanit Kutusu (WORKFLOW §2)

```text
KANIT   : <komut> -> <sayilar>
DOSYA   : <yazilan dosyalar>
EKSIK   : YOK ya da DOĞRULAMA GEREKLI
SONRAKI : <adim, ~ sure>
```

## §5 Kaynak (turetilme dayanagi)

- [brain](../.decisions/brain.md) §1.1 **ADR-018 yakalama: `security-review-checklist`**
- [CLAUDE](../claude.md) G2 · G8 · §2 (tespit komutlari)
- [AGENTS](../definitions/agents.md) §2 (`security-auditor` ajani)

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
