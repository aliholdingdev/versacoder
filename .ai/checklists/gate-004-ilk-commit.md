---
title: "GATE-004 — Iskelet + Build + Ilk Commit"
type: checklist
category: checklists
status: active
version: 1.0.0
updated: 2026-10-07
authority: reference
scope: "GATE-004 build green + ilk commit — cift TFM, NoWarn=0, NU1008=0, dosya bazli git add"
tags: [checklist]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# GATE-004 — Iskelet + Build + Ilk Commit

**Zorunlu Baglantilar:** [[checklists/index|checklist index]] · [[index|vault index]] · [[governance/workflow|WORKFLOW]] · [[claude|CLAUDE]] · [[definitions/agents|AGENTS]] · [[.decisions/brain|brain]] · [[ops/todos/index|TODOS index]]

> **Tek cumlelik sorumluluk:** Cift bacakli buildin yesil oldugunu ve ilk commitin kurallara uygun atildigini kanitlar.

## §0 Ozet (Answer First)

- **Kullanim:** bu gecisten once yukaridan asagi tek tek isaretlenir; **1 madde bile bossa RED**.
- **Kapatan:** kullanici — bu liste yalniz kanit hazirlar (G12).
- **Kanit bicimi:** [[governance/workflow|WORKFLOW]] §2 4 satir (`KANIT/DOSYA/EKSIK/SONRAKI`).
- **Sira:** [[governance/workflow|WORKFLOW]] §1 gate sirasinda ilgili kapidan **once** calistirilir.

### §0.1 Calistirma zamanlamasi

| Alan | Deger |
|---|---|
| Ne zaman | ilgili is baslamadan **once** (on-kontrol) ya da kapi kapanisinda (kanit) |
| Kim hazirlar | isin ureticisi (matristeki ajan — [[definitions/agents|AGENTS]] §2) |
| Kim kapatir | **yalniz kullanici** (G12) |
| Suresi asilirsa | is durur + [[ops/log|log]] raporu (WORKFLOW §2, 20 dk esigi) |
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

- [[governance/workflow|WORKFLOW]] §2.3 (5 kabul kriteri + 4 commit kaniti + 6 kirmizi neden)
- [[claude|CLAUDE]] G2 · G3 · G6 · G7 · §B10
- [[.decisions/brain|brain]] §1.1 ADR-014/019/022 yakalama satirlari
- **Turetme notu:** WORKFLOW "13 adim" der; adimlar yazilmamis -> kabul kriterlerinden 7 madde turetildi, fark `DOĞRULAMA GEREKLI`

> **Not:** Bu liste vault belgelerinden **turetilmistir**; kaynakta olmayan madde uydurulmaz (G9). Turetilmis sayi ile kaynak arasindaki fark varsa `DOĞRULAMA GEREKLI` isaretlenir.

---

## §6 Iliskili Dosyalar

[[checklists/index|index]] · [[governance/workflow|WORKFLOW]] · [[claude|CLAUDE]] · [[definitions/agents|AGENTS]] · [[.decisions/brain|brain]] · [[definitions/projects|PROJECTS]] · [[definitions/vision|VISION]] · [[governance/keys|keys]] · [[definitions/glossary|glossary]] · [[ops/log|log]] · [[ops/todos/index|TODOS index]] · [[.decisions/draft/adr-031-visual-production-provider|ADR-031]]

### §6.1 Ayni gecisten gecen diger checklist'ler

Sert kapi on kontrolleri (hepsi kapi kapanisindan **once** calisir; birinde RED varsa siradaki kapi acilmaz):

- [[checklists/gate-004a-vault-kapanis|gate-004a-vault-kapanis]] — vault olcumu (GATE-004a)
- [[checklists/gate-003-ui-onay|gate-003-ui-onay]] — 7 wireframe + kullanici onayi (GATE-003, HARD STOP)
- [[checklists/gate-004-ilk-commit|gate-004-ilk-commit]] — cift bacak build + ilk commit (GATE-004)
- [[checklists/gate-005-test-kapsami|gate-005-test-kapsami]] — test + NetArchTest (GATE-005)
- [[checklists/layer-boundary-check|layer-boundary-check]] — katman siniri 13 soru

**Ortak kural:** bu checklist'teki bir madde digerinde de RED uretiyorsa **once en alttaki kok neden** kapatilir; yuzey duzeltmesi kapiyi acmaz (CLAUDE G5 — kanitsiz gecis yok).
