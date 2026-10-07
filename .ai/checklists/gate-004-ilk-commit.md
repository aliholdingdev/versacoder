---
title: "GATE-004 — Iskelet + Build + Ilk Commit"
type: checklist
category: checklists
status: active
version: 1.0.0
updated: 2026-10-07
authority: SSOT
scope: "GATE-004 build green + ilk commit — cift TFM, NoWarn=0, NU1008=0, dosya bazli git add"
---

# GATE-004 — Iskelet + Build + Ilk Commit

**Zorunlu Baglantilar:** [checklist index](index.md) · [vault index](../index.md) · [WORKFLOW](../governance/workflow.md) · [CLAUDE](../claude.md) · [AGENTS](../definitions/agents.md) · [brain](../.decisions/brain.md) · [TODOS index](../ops/todos/index.md)

> **Tek cumlelik sorumluluk:** Cift bacakli buildin yesil oldugunu ve ilk commitin kurallara uygun atildigini kanitlar.

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

- [ ] **1.** `dotnet build -f net48` ve `-f net10.0-windows` — ikisi de **0 hata**
- [ ] **2.** Proje/props dosyalarinda `NoWarn` aramasi = **0 satir** (hata gizleme yok)
- [ ] **3.** Yasak tarama ucu de 0: EF Core · DevExpress v26.1 disi · string-concat SQL
- [ ] **4.** `Directory.Packages.props` test paketlerini icerir -> `dotnet restore` **0 NU1008**
- [ ] **5.** Ilk commit yalniz bu kapi kapandiktan sonra, **dosya bazli** `git add` ile atilmis
- [ ] **6.** Commit message `<kapi/ tipi>: <kisa ozet>` (<=72 karakter) + govdede `KANIT:` satiri
- [ ] **7.** `git show --name-only` -> stage edilenler yalniz kasitli dosyalar (kapsam disi 0)

## §2 Yakalama (calistirilacak komut)

| # | Komut / tarama | Beklenen cikti |
|---|---|---|
| 1 | `dotnet build VersaCoder.slnx -f net48` | 0 hata |
| 2 | `dotnet build VersaCoder.slnx -f net10.0-windows` | 0 hata |
| 3 | `Select-String -Path *.csproj,Directory.Packages.props -Pattern NoWarn` | 0 satir |
| 4 | `Select-String -Path *.csproj,Directory.Packages.props -Pattern EntityFrameworkCore` | 0 satir |
| 5 | `dotnet restore` | 0 NU1008 |
| 6 | `git log -1 --format=%s + --format=%B` | 72 krk on ek + KANIT satiri |
| 7 | `git show --name-only --format=` | yalniz kasitli dosyalar |

## §3 RED Nedenleri

| # | Neden | Sonuc |
|---|---|---|
| 1 | Tek bacakta derleniyor | G6/ADR-019 — kapi acik |
| 2 | Hata NoWarn ile ortulmus | Hata geri acilir, build yeniden kosulur |
| 3 | Restore NU1008 veriyor | Test paketleri Directory.Packages.props ta degil |
| 4 | DevExpress surumu/stili sapmis | G3 CRITICAL -> RED |
| 5 | EF Core referansi girmis | G2 CRITICAL -> is durur |
| 6 | git add -A kullanilmis | G7 — yikici, kapi RED |
| 7 | Vault yesil degilken commit atilmis | Commit geri alinir (G7 onayi) |

## §4 Kanit Kutusu (WORKFLOW §2)

```text
KANIT   : <komut> -> <sayilar>
DOSYA   : <yazilan dosyalar>
EKSIK   : YOK ya da DOĞRULAMA GEREKLI
SONRAKI : <adim, ~ sure>
```

## §5 Kaynak (bu liste nereden turetilmistir)

- [WORKFLOW](../governance/workflow.md) §2.3 (5 kabul kriteri + 4 commit kaniti + 6 kirmizi neden)
- [CLAUDE](../claude.md) G2 · G3 · G6 · G7 · §B10
- [brain](../.decisions/brain.md) §1.1 ADR-014/019/022 yakalama satirlari
- **Turetme notu:** WORKFLOW "13 adim" der; adimlar yazilmamis -> kabul kriterlerinden 7 madde turetildi, fark `DOĞRULAMA GEREKLI`

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
