---
title: "Veri Erisim (ADO.NET + Dapper) Kontrolu"
type: checklist
category: checklists
status: active
version: 1.0.0
updated: 2026-10-07
authority: SSOT
scope: "ADR-005 + ADR-016 + ADR-017 — EF yok, DbConnection yalniz l4, parametreli SQL"
---

# Veri Erisim (ADO.NET + Dapper) Kontrolu

**Zorunlu Baglantilar:** [checklist index](index.md) · [vault index](../index.md) · [WORKFLOW](../WORKFLOW.md) · [CLAUDE](../CLAUDE.md) · [AGENTS](../AGENTS.md) · [brain](../brain.md) · [TODOS index](../TODOS/index.md)

> **Tek cumlelik sorumluluk:** Veri erisinin EF Core disi, parametreli ve katman kurallarina uygun oldugunu denetler.

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

- [ ] **1.** EF Core referansi **0** (paket + using + DbContext)
- [ ] **2.** Her sorgu **parametreli** (`@p1..@pN`); concat **0**
- [ ] **3.** `DbConnection` yalniz `l4 ado-dapper` icinde (baska katmanda baglanti yok)
- [ ] **4.** Migration = **saf SQL + `schema_version`** tablosu (ORM migration degil)
- [ ] **5.** Her komutta `CommandTimeout` acik set
- [ ] **6.** Test projesinde EF referansi **0**

## §2 Yakalama (komut / tarama)

| # | Komut | Beklenen cikti |
|---|---|---|
| 1 | `Select-String -Path *.csproj,Directory.Packages.props -Pattern EntityFrameworkCore` | 0 satir |
| 2 | `Select-String -Path src/**/*.cs -Pattern "\"\s*\+\s*\w*(Id|Name)"` | 0 satir |
| 3 | `Select-String -Path src/**/*.cs -Pattern "new SqlConnection|new SqliteConnection"` | yalniz l4 ado-dapper |
| 4 | `Select-String -Path .ai/.sql/** -Pattern "schema_version"` | migration dosyalarinda var |

## §3 RED Nedenleri

| # | Neden | Sonuc |
|---|---|---|
| 1 | EF Core referansi | G2 CRITICAL -> build RED, kod durur |
| 2 | Concat SQL | G2 -> otomatik RED |
| 3 | Baglanti ust katmanda | ADR-016 -> NetArchTest RED |

## §4 Kanit Kutusu (WORKFLOW §2)

```text
KANIT   : <komut> -> <sayilar>
DOSYA   : <yazilan dosyalar>
EKSIK   : YOK ya da DOĞRULAMA GEREKLI
SONRAKI : <adim, ~ sure>
```

## §5 Kaynak (turetilme dayanagi)

- [brain](../brain.md) §1.1 **ADR-005 + ADR-016 yakalama: `data-access-check`**
- [CLAUDE](../CLAUDE.md) G2 · §2 (EF + concat tarama komutlari)
- [PROJECTS](../PROJECTS.md) §2 (dis bagimliliklar)

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
