---
title: "Bagimlilik / Paket Guncelleme Kontrolu"
type: checklist
category: checklists
status: active
version: 1.0.0
updated: 2026-10-07
authority: SSOT
scope: "ADR-012 + ADR-014 — CPM, .csproj icinde Version=0, iki bacakta ayni major"
---

# Bagimlilik / Paket Guncelleme Kontrolu

**Zorunlu Baglantilar:** [checklist index](index.md) · [vault index](../index.md) · [WORKFLOW](../governance/workflow.md) · [CLAUDE](../claude.md) · [AGENTS](../definitions/agents.md) · [brain](../.decisions/brain.md) · [TODOS index](../ops/todos/index.md)

> **Tek cumlelik sorumluluk:** Paket surumlerinin CPM uzerinden tek yerden yonetildigini ve cift bacak uyumlu oldugunu denetler.

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

- [ ] **1.** Surumler yalniz `Directory.Packages.props` (CPM) icinde
- [ ] **2.** .csproj icinde `Version` satiri **0** (ADR-014)
- [ ] **3.** Her paket **iki TFM** icinde de restore ediliyor
- [ ] **4.** Yeni paket = kullanici onayi (G8)
- [ ] **5.** DevExpress yalniz **26.1** (G3)
- [ ] **6.** Test paketleri CPM'e eklenmis (NU1008 riski kapali)

## §2 Yakalama (komut / tarama)

| # | Komut | Beklenen cikti |
|---|---|---|
| 1 | `Select-String -Path *.csproj -Pattern "<Version>"` | 0 satir |
| 2 | `Select-String -Path Directory.Packages.props -Pattern "DevExpress.\w+ \d+\.\d+"` | tek surum 26.1 |
| 3 | `dotnet restore` | 0 NU1008 |
| 4 | `dotnet build -f net48 + -f net10.0-windows` | 0 hata (iki bacak) |

## §3 RED Nedenleri

| # | Neden | Sonuc |
|---|---|---|
| 1 | .csproj icinde Version satiri | ADR-014 ihlali -> CPM'e tasinir |
| 2 | Sapma (26.1 disi / iki bacak farkli) | G3/G6 -> RED |
| 3 | Onaysiz yeni paket | G8 -> DECISION BLOCKED |

## §4 Kanit Kutusu (WORKFLOW §2)

```text
KANIT   : <komut> -> <sayilar>
DOSYA   : <yazilan dosyalar>
EKSIK   : YOK ya da DOĞRULAMA GEREKLI
SONRAKI : <adim, ~ sure>
```

## §5 Kaynak (turetilme dayanagi)

- [brain](../.decisions/brain.md) §1.1 **ADR-012 + ADR-014 yakalama: `dependency-update-check`**
- [PROJECTS](../definitions/projects.md) §2.2 (paket gerekceleri) · §3 (CPM duzeltmesi)
- [CLAUDE](../claude.md) G3 · G8

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
