---
title: "GATE-005 — Test + Kapsam"
type: checklist
category: checklists
status: active
version: 1.0.0
updated: 2026-10-07
authority: SSOT
scope: "GATE-005 test green — dotnet test iki bacak, NetArchTest 0 ihlal, coverage gerekcesi"
---

# GATE-005 — Test + Kapsam

**Zorunlu Baglantilar:** [checklist index](index.md) · [vault index](../index.md) · [WORKFLOW](../WORKFLOW.md) · [CLAUDE](../CLAUDE.md) · [AGENTS](../AGENTS.md) · [brain](../brain.md) · [TODOS index](../TODOS/index.md)

> **Tek cumlelik sorumluluk:** Testlerin yesil oldugunu, mimari denetimin ihlalsiz oldugunu ve kapsamin kanitini denetler.

## §0 Ozet (Answer First)

- **Kullanim:** bu gecisten once yukaridan asagi tek tek isaretlenir; **1 madde bile bossa RED**.
- **Kapatan:** kullanici — bu liste yalniz kanit hazirlar (G12).
- **Kanit bicimi:** [WORKFLOW](../WORKFLOW.md) §2 4 satir (`KANIT/DOSYA/EKSIK/SONRAKI`).
- **Sira:** [WORKFLOW](../WORKFLOW.md) §1 gate sirasinda ilgili kapidan **once** calistirilir.

### §0.1 Calistirma zamanlamasi

| Alan | Deger |
|---|---|
| Ne zaman | ilgili is baslamadan **once** (on-kontrol) ya da kapi kapanisinda (kanit) |
| Kim hazirlar | isin ureticisi (matristeki ajan — [AGENTS](../AGENTS.md) §2) |
| Kim kapatir | **yalniz kullanici** (G12) |
| Suresi asilirsa | is durur + [log](../log.md) raporu (WORKFLOW §2, 20 dk esigi) |
| Kanit kutusu | §4 — 4 satir, `EKSIK` bos birakilmaz |

## §1 Kabul Kriterleri (7 madde)

- [ ] **1.** `dotnet test` — **0 basarisiz**; iki bacakta da calisir ve test sayilari esittir
- [ ] **2.** NetArchTest — yon + circular **0 ihlal**; matris ADR-028 §2.3 ile ayni kaynaktan
- [ ] **3.** layer-boundary-check 13 sorusu — **0 yanlis negatif**
- [ ] **4.** Coverage hedefi `spec/test-strategy.md` bandidir; altindaysa **yazili gerekce** zorunlu
- [ ] **5.** Atlama (`Skip` / `Assert.Ignore`) **0**; varsa gerekcesi `EKSIK` satirinda
- [ ] **6.** Test edilmemis sonuc "verified / production ready" **olarak yazilmamis** (G5)
- [ ] **7.** Test projesinde EF Core referansi **0** (G2)

## §2 Yakalama (calistirilacak komut)

| # | Komut / tarama | Beklenen cikti |
|---|---|---|
| 1 | `dotnet test --framework net48` | Gecen=N · Basarisiz=0 |
| 2 | `dotnet test --framework net10.0-windows` | Gecen=N · Basarisiz=0 |
| 3 | `NetArchTest LayerRulesTests` | 0 failure (yon + circular) |
| 4 | `Select-String -Path tests/**/*.cs -Pattern "Skip|Assert.Ignore"` | 0 satir |
| 5 | `Select-String -Path *.Tests.csproj -Pattern EntityFrameworkCore` | 0 satir |

## §3 RED Nedenleri

| # | Neden | Sonuc |
|---|---|---|
| 1 | Atlanan test var | RED — test geri acilir ya da gerekce yazilir |
| 2 | NetArchTest ihlali | Mimari RED — katman koduna donulur |
| 3 | Tek bacakta test | G6/ADR-019 — kapi acik |
| 4 | Coverage bandin altinda, gerekcesiz | Kanitsiz coverage (G5) — kapi acik |
| 5 | Yasak tarama temiz degil | G2/G3 — teslim yolunun onu kapanir |

## §4 Kanit Kutusu (WORKFLOW §2)

```text
KANIT   : <komut> -> <sayilar>
DOSYA   : <yazilan dosyalar>
EKSIK   : YOK ya da DOĞRULAMA GEREKLI
SONRAKI : <adim, ~ sure>
```

## §5 Kaynak (bu liste nereden turetilmistir)

- [WORKFLOW](../WORKFLOW.md) §2.4 (5 kabul kriteri + 6 kirmizi neden + NetArchTest sarti)
- [CLAUDE](../CLAUDE.md) G2 · G5 · §2 (tespit komutlari)
- [brain](../brain.md) §1.1 ADR-005/017/019 yakalama satirlari · §4.1
- **Turetme notu:** WORKFLOW "14 adim" der; adimlar yazilmamis -> kabul kriterlerinden 7 madde turetildi, fark `DOĞRULAMA GEREKLI`

> **Not:** Bu liste vault belgelerinden **turetilmistir**; kaynakta olmayan madde uydurulmaz (G9). Turetilmis sayi ile kaynak arasindaki fark varsa `DOĞRULAMA GEREKLI` isaretlenir.

---

## §6 Iliskili Dosyalar

[index](index.md) · [WORKFLOW](../WORKFLOW.md) · [CLAUDE](../CLAUDE.md) · [AGENTS](../AGENTS.md) · [brain](../brain.md) · [PROJECTS](../PROJECTS.md) · [VISION](../VISION.md) · [keys](../keys.md) · [glossary](../glossary.md) · [log](../log.md) · [TODOS index](../TODOS/index.md) · [ADR-031](../.decisions/draft/ADR-031-visual-production-provider.md)

### §6.1 Ayni gecisten gecen diger checklist'ler

Sert kapi on kontrolleri (hepsi kapi kapanisindan **once** calisir; birinde RED varsa siradaki kapi acilmaz):

- [gate-004a-vault-kapanis](gate-004a-vault-kapanis.md) — vault olcumu (GATE-004a)
- [gate-003-ui-onay](gate-003-ui-onay.md) — 7 wireframe + kullanici onayi (GATE-003, HARD STOP)
- [gate-004-ilk-commit](gate-004-ilk-commit.md) — cift bacak build + ilk commit (GATE-004)
- [gate-005-test-kapsami](gate-005-test-kapsami.md) — test + NetArchTest (GATE-005)
- [layer-boundary-check](layer-boundary-check.md) — katman siniri 13 soru

**Ortak kural:** bu checklist'teki bir madde digerinde de RED uretiyorsa **once en alttaki kok neden** kapatilir; yuzey duzeltmesi kapiyi acmaz (CLAUDE G5 — kanitsiz gecis yok).
