---
title: "Katman Siniri Kontrolu — 13 Soru"
type: checklist
category: checklists
status: active
version: 1.0.0
updated: 2026-10-07
authority: reference
scope: "Katman siniri 13 sorusu — ADR-028 8x8 matris, NetArchTest, yon yasaklari; GATE-005 kabul kriteri 3"
tags: [checklist]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# Katman Siniri Kontrolu — 13 Soru

**Zorunlu Baglantilar:** [[checklists/index|checklist index]] · [[index|vault index]] · [[governance/workflow|WORKFLOW]] · [[claude|CLAUDE]] · [[definitions/agents|AGENTS]] · [[.decisions/brain|brain]] · [[ops/todos/index|TODOS index]]

> **Tek cumlelik sorumluluk:** Mimari ciktinin ADR-028 yon matrisinden gecip gecmedigini 13 maddeyle denetler.

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

## §1 Kabul Kriterleri (13 madde)

- [ ] **1.** Izin verilen yon tek: `l7 -> l6 -> l5 -> l4 -> l2 -> l1 -> l0` ve `l3 -> yalniz l0`
- [ ] **2.** `l4 -> l2` dogrudan ok **VAR MI?** (repository portu `l0`da, uygulamasi `l4`te -> ok YOK)
- [ ] **3.** `l5 -> l4` dogrudan ok **VAR MI?** (ok YOK)
- [ ] **4.** `l3 Crosscutting` yalniz `l0`a bakiyor; `l1..l7` tanimiyor
- [ ] **5.** Iceeriye bagimlilik yalniz `interface` uzerinden (somut sinif `new`lenmemis)
- [ ] **6.** Yeni tip nereye yazildi: arayuz->`l0` · is kurali->`l1` · senaryo->`l2` · kesit->`l3` · somut dis dunya->`l4` · arac sozlesmesi->`l5` · kurulum->`l6` · ekran->`l7`
- [ ] **7.** Protokol (`l5`) yalniz `l0`a iniyor (atlamali inis); `l1..l4` tanimiyor
- [ ] **8.** Domain katmani Infrastructure/UI/Database/AI Provider `using` **icermiyor**
- [ ] **9.** Application katmani DevExpress / CLI / Provider SDK / Database API **biliyor mu?** (bilmemeli)
- [ ] **10.** Host (`l6`) yalniz kompozisyon + startup; is mantigi **yok**
- [ ] **11.** Test projesi uretim koduna referans vermiyor (uretim asla teste bakmaz)
- [ ] **12.** 8x8 yon matrisi NetArchTest `LayerRulesTests` ile **ayni kaynaktan** geliyor (elle cizilmis matris kanit degildir)
- [ ] **13.** Yeni kod eklenmeden once 13 soru **0 yanlis negatif**

## §2 Yakalama (calistirilacak komut)

| # | Komut / tarama | Beklenen cikti |
|---|---|---|
| 1 | `Select-String -Path src/**/*.cs -Pattern "using Microsoft.EntityFrameworkCore"` | 0 satir (G2) |
| 2 | `dotnet build -f net48 + -f net10.0-windows` | 0 hata (cift bacak) |
| 3 | `dotnet test --framework net48 + --framework net10.0-windows` | 0 basarisiz |
| 4 | `NetArchTest LayerRulesTests` | 0 ihlal (yon + circular) |
| 5 | `pwsh .ai/scripts/count-components.ps1` | PASS · 52 dosya / >=5000 kayit |

## §3 RED Nedenleri

| # | Neden | Sonuc |
|---|---|---|
| 1 | Ters ok bulundu (l4->l2 / l5->l4 / l3->l0 disi) | Mimari RED — kod geri alinir, yon matrisine donulur |
| 2 | Somut sinif ust katmanda `new`lenmis | DIP ihlali — port `l0`a tasinir |
| 3 | Tek bacakta yesil test ile "bitti" | G6/ADR-019 RED — ikinci bacak calistirilir |
| 4 | 13 sorudan biri cevapsiz | Kanitsiz — kapi acilmaz (G5) |

## §4 Kanit Kutusu (WORKFLOW §2)

```text
KANIT   : <komut> -> <sayilar>
DOSYA   : <yazilan dosyalar>
EKSIK   : YOK ya da DOĞRULAMA GEREKLI
SONRAKI : <adim, ~ sure>
```

## §5 Kaynak (bu liste nereden turetilmistir)

- [[governance/workflow|WORKFLOW]] §2.4 (GATE-005 kabul kriteri + 13 soru madde 3) · §3.3 (izinli yon tablosu)
- [[claude|CLAUDE]] §5 (katman yasaklari + 8x8 matris) · §2 (tespit komutlari)
- [[.decisions/brain|brain]] §1.1 ADR-013/028 yakalama satirlari · §4.1 (RED tablosu)
- ADR-028 §2.3 madde 2 (matris = NetArchTest) · §C6 duzeltme 2
- **Turetme notu:** orijinal "13 soru" metni vaultta yoktur; 13 madde yukaridaki kaynaklardaki yasak/kabul maddelerinden **turetilmistir** -> `DOĞRULAMA GEREKLI`

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
