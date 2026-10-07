---
title: "Performans Butcesi Kontrolu"
type: checklist
category: checklists
status: active
version: 1.0.0
updated: 2026-10-07
authority: reference
scope: "Performans olcumu — butce, latency, bellek; olculmemis iddia yasagi (G9/G5)"
tags: [checklist]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# Performans Butcesi Kontrolu

**Zorunlu Baglantilar:** [checklist index](index.md) · [vault index](../index.md) · [WORKFLOW](../governance/workflow.md) · [CLAUDE](../claude.md) · [AGENTS](../definitions/agents.md) · [brain](../.decisions/brain.md) · [TODOS index](../ops/todos/index.md)

> **Tek cumlelik sorumluluk:** Olculmus performans sinirlarinin asilmadigini ve olculmemis iddialarin kanitlandigini denetler.

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

- [ ] **1.** Butce kalemleri **olc** (tahmin degil): build suresi, test suresi, acilis
- [ ] **2.** Olculmemis performans iddiasi **yok** ("hizli" = kanitsiz)
- [ ] **3.** Benchmark sonucu varsa **calistirilmis ciktisi** ile yazilmis
- [ ] **4.** Butce asimi raporlanmis (kacinilmamis)
- [ ] **5.** Erken optimizasyon (premature) **yok** — olcum once
- [ ] **6.** Gate suresi asimi (20 dk) raporlanmis

## §2 Yakalama (komut / tarama)

| # | Komut | Beklenen cikti |
|---|---|---|
| 1 | `dotnet build (suresi olc)` | sure + 0 hata |
| 2 | `dotnet test (suresi olc)` | sure + 0 basarisiz |
| 3 | `olcum ciktisi (benchmark varsa)` | sayi + komut |

## §3 RED Nedenleri

| # | Neden | Sonuc |
|---|---|---|
| 1 | Kanitsiz performans iddiasi | G9 -> madde silinir |
| 2 | Butce asimi raporlanmamis | G5 -> kapi RED |
| 3 | Erken optimizasyon | olcum istenir |

## §4 Kanit Kutusu (WORKFLOW §2)

```text
KANIT   : <komut> -> <sayilar>
DOSYA   : <yazilan dosyalar>
EKSIK   : YOK ya da DOĞRULAMA GEREKLI
SONRAKI : <adim, ~ sure>
```

## §5 Kaynak (turetilme dayanagi)

- [CLAUDE](../claude.md) G5 · G9 (kanitsiz/olculmemis iddia yasağı)
- [AGENTS](../definitions/agents.md) §11 (butce tablosu, ~ ile kestirim)
- [ULTRA-THINKING](../definitions/ultra-thinking.md) §2.7 (dalga butcesi)
- **Turetme notu:** butce kalemleri vault butce tablosundan turetildi; kesin esikler `DOĞRULAMA GEREKLI` (olcum yapilmadi)

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
