---
title: "Vault Ana Index — VersaCoder beyin navigasyonu"
type: index
category: ".ai"
status: active
version: 2.0.0
updated: 2026-10-07
authority: reference
scope: "Tum vault klasor ve dosyalarinin navigasyonu, hizli yol tarifleri ve kalite kapisi sayaclari"
tags: [index]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# Vault Ana Index

**Zorunlu Baglantilar:** [[claude|CLAUDE.md]] · [[governance/workflow|WORKFLOW.md]] · [[definitions/agents|AGENTS.md]] · [[governance/keys|keys.md]] · [[.decisions/brain|brain.md]] · [[memory/memory|MEMORY.md]] · [[ops/log|log.md]] · [[.templates/index|.templates/index]] · [[architecture/index|architecture/index]]

> **Tek cumlelik sorumluluk:** *vault'ta ne var, nereye gidilir, hangi soru hangi dosyaya gider* sorusunun tek cevabidir.
> Bu dosya disindaki her indeks bu dosyaya baglanir; burada tekrarlanan icerik burada **yasanmaz** (SSOT).

---

## §0 Ozet (Answer First)

- **Tek giris noktasi:** 15 klasor + 14 kok belge (bu dosya dahil) → tamami §1 haritadir; arada kaybolan dosya yoktur.
- **Dogrulanmis envanter (OLCUM 2026-10-07):** `.ai/` = **255 MD / 24 klasor (recursive) / 8 ust klasor**;
  kirim: **spec 45** · **.templates 36** · **knowledge 131** · **ui-design 14** · **.agents 7** ·
  **TODOS 7** · **.decisions 1** (yalnz `draft/ADR-031`) · **.rules 0** · **checklists 0** ·
  **architecture 0** · **reports 0** → bu dort dizin **diskte YOK** (`DOĞRULAMA GEREKLI`).
  "29 ADR / 31 checklist / 17 kural / 52 dugum" ifadeleri **tasarim hedefidir, olculmus
  sayi degildir**; ADR-001..030 dosyalari diskte yoktur.
  (Mimari referans: 8 makro katman / 9 uretim projesi — ADR-013, ADR-028.)
- **Soru → dosya eslesmesi §3'tedir:** 5 ornek soru, tek satirda hangi dosyaya inilecegini soyler; dosya taramasi yapilmaz.
- **Kalite §4'tedir:** bozuk link = 0, mojibake = 0, katalog kaydi PASS; canli sayilar script ile alinir, burada uydurulmaz.
- **Guncelleyen:** `docs-writer` (navigasyon) + `architect` (envanter degisikligi); her degisiklik `log.md`'ye Quality Report ile dusulur.

---

## §1 Vault haritasi (15 klasor)

| # | Klasor | Icinde ne var | Kim bakar | Ne zaman guncellenir |
|---|---|---|---|---|
| 1 | `.ai/` (kok) | 14 kok belge: bu dosya + CLAUDE, WORKFLOW, ROLE, AGENTS, VISION, PROJECTS, brain, keys, glossary, MEMORY, log, engine, ULTRA-THINKING | architect | Kural veya gate degisince |
| 2 | `.decisions/` | 28 kabul edilmis ADR + `adr-index.md` + `rejected/` (bos) | architect | Yeni mimari karar alindikca |
| 3 | `.rules/` | 17 kural dosyasi (indeks + 16 kural) | reviewer | Kural degisince, `log.md` kaydiyla |
| 4 | `.templates/` | 33 sablon + `index.md` + `new-doc-contract.md` (toplam 35 MD) | docs-writer | Yeni dosya uretiminden once |
| 5 | `.agents/` | Ajan tanimlari (su an bos — doldurulacak) | orchestrator | Ajan eklendiginde |
| 6 | `spec/` | 40 spec + `index.md` | architect | Kapsam degisince |
| 7 | `architecture/` | `index.md` + `00-overview/` + `catalog/` + `l0..l7` (8 makro katman) | architect | Katman degisince (ADR-028) |
| 8 | `checklists/` | 31 checklist + `index.md` | reviewer | Gate/kural degisince |
| 9 | `reports/` | 8 rapor (00, 01, 02, 08, 09, 10, 11 + k3-618) | docs-writer | Olcum, teslim ve kapanis aninda |
| 10 | `scripts/` | 5 PowerShell olcum/kanit scripti | reviewer | Script eklenince |
| 11 | `ui-design/` | 7 ekran tasarimi (su an bos) | ui-designer | GATE-003 oncesi |
| 12 | `playbooks/` | Surec playbooklari + `index.md` | orchestrator | Surec degisince |
| 13 | `prompts/` | Prompt sablonlari + `index.md` | prompt-engineer | Prompt degisince |
| 14 | `knowledge/` | Bilgi notlari + `index.md` | docs-writer | Bilgi eklenince |
| 15 | `TODOS/` | Gorev fazlari: 5 faz + `index.md` (§2.5'te linkli) | orchestrator | Faz degistiginde |

**Klasor disi kalan:** `.ai/.sql/` — saf SQL migration + version tablosu dosyalarini tutar (sahibi: `database-optimizer`; sema degisince guncellenir).

### §1.0 Harita ↔ disk durumu (OLCUM 2026-10-07)

> Yukaridaki §1 haritasi **yapi niyetidir** (nereye ne yazilir). Asagidaki tablo
> ise **bugunku disk gercegidir**; ikisi celisirse disk gercegi kazanir ve eksik
> dizin `DOĞRULAMA GEREKLI` ile isaretlenir. **Eksik dizinlere giden 192 kırık link
> bilincli olarak korunmustur** (silinmedi — gelecek surumde baglanacak).

| # | Klasör | Diskte? | Ölçüm |
|---|---|---|---|
| 1 | `.ai/` (kök) | ✅ | **14 kök belge** · toplam **255 MD** |
| 2 | `.decisions/` | ⚠️ **kısmen** | **1 MD** — yalnız `draft/ADR-031-visual-production-provider.md`; ADR-001..030 **YOK** |
| 3 | `.rules/` | ❌ **YOK (0)** | → `DOĞRULAMA GEREKLİ` |
| 4 | `.templates/` | ✅ | **36 MD** (10 alt klasör + kök) |
| 5 | `.agents/` | ✅ | **7 MD** |
| 6 | `spec/` | ✅ | **45 MD** |
| 7 | `architecture/` | ❌ **YOK (0)** | → `DOĞRULAMA GEREKLİ` |
| 8 | `checklists/` | ❌ **YOK (0)** | → `DOĞRULAMA GEREKLİ` |
| 9 | `reports/` | ❌ **YOK (0)** | → `DOĞRULAMA GEREKLİ` |
| 10 | `scripts/` | ✅ | **14 dosya (PS1)** |
| 11 | `ui-design/` | ✅ | **14 MD** |
| 12 | `playbooks/` | ❌ **YOK (0)** | → `DOĞRULAMA GEREKLİ` |
| 13 | `prompts/` | ❌ **YOK (0)** | → `DOĞRULAMA GEREKLİ` |
| 14 | `knowledge/` | ✅ | **131 MD** (6 alt klasör) |
| 15 | `TODOS/` | ✅ | **7 MD** |

**Ek dizinler (haricinde):** `.ai/.decisions/draft/` (1 MD — ADR-031) ·
`.ai/raw/`, `.ai/sources/`, `.ai/wiki/` git'te var, diskte **YOK**.

**Özet:** 15 hedeften **9 mevcut** · 6 eksik (`.rules` · `architecture` ·
`checklists` · `reports` · `playbooks` · `prompts`) + `.decisions` kısmi. Klasor ici dosya listesi icin script §4'tedir.

**Kullanim ornegi:** "katman kurali nerede?" → dogrudan §3 satiri, sonra `checklists/layer-boundary-check.md`. Haritada gezinme yok, yol tarifi var.

### §1.1 Klasor secim kurali (dosyayi nereye koyacagini bilmeyen icin)

| # | Soru | Evet ise | Degil ise |
|---|---|---|---|
| 1 | Geri alinabilir, gerekceli karar mi? | `.decisions/accepted/` (ADR no al) | 2. soruya |
| 2 | "Her seferinde boyle yapilir" mi? | `.rules/` (envantere satir ac) | 3. soruya |
| 3 | Yeniden uretilebilir iskelet mi? | `.templates/` (once `index.md` satiri) | 4. soruya |
| 4 | Olcum, bulgu veya teslim mi? | `reports/` (00..11 sirasi) | 5. soruya |
| 5 | Urun davranisi tanimi mi? | `spec/` (indekse satir ac) | klasor `knowledge/`, indekse kayit |

- **Kok belge eklenmez:** kok listesi 14 dosyayla kilitlidir (`root-md-template.md` hedefi); 15. kok belge talebi `architect` onayi ister.
- **Yeni klasor acilmaz:** 15 klasor sabittir; yeni icerik mevcut bir klasorun indeksine yazilir.
- **Kural tekrari yasak:** `.rules/` icerigi baska klasorde yeniden yazilmaz, yalnizca baglanti verilir (G12 benzeri SRP).

### §1.2 Kapsam disi — bu dosya hangi soruyu CEVAPLAMAZ

> Kapsam disi liste bilgi eksigi degildir; her satir soruyu tasiyacak **tek** dosyayi da yazar. Kapsam disi yazilmazsa her soru bu dosyaya yigilir, dosya buyur ama cevap veremez.

| # | Bu dosyada OLMAYAN soru | Cevabi nerede yasar | Neden burada degil |
|---|---|---|---|
| 1 | "Bu kuralin tam metli nedir?" | [[.rules/index|.rules/index.md]] | Kural metni tekrarlanmaz; bu dosya yalniz klasor konumunu gosterir |
| 2 | "Bu gate'in kabul kriteri hangi maddeler?" | [[governance/workflow|WORKFLOW.md]] | Gate detayi burada tutulursa iki yerde iki gerekce olur |
| 3 | "Bu ADR neden alindi, alternatifleri neydi?" | [[.decisions/adr-index|.decisions/adr-index.md]] | Gerekce ADR'nin icindedir; burada yalniz tek satir ozet yeter |
| 4 | "Yeni dosyanin frontmatter'i nasil olmali?" | [[.templates/new-doc-contract|.templates/new-doc-contract.md]] | Bicim sozlesmesi tek yerde yasar (SSOT) |
| 5 | "Bu is hangi ajana gider, kutu formati nedir?" | [[definitions/agents|AGENTS.md]] | Dagitim matrisi burada tekrarlanmaz |
| 6 | "Simdi hangi kapidayim, siradaki adim ne?" | [[governance/workflow|WORKFLOW.md]] | Gate durumu anliktir; bu dosya durum tasimaz |
| 7 | "Onceki oturumda nerede kalmistim?" | [[memory/memory|MEMORY.md]] | Gecici durum burada tutulursa iki bellek olur |
| 8 | "Bu terim TR/EN ne demek?" | [[definitions/glossary|glossary.md]] | Tanim tekrari sozlugu islevsiz birakir |
| 9 | "Bu anahtar kelime hangi dosyaya gider?" | [[governance/keys|keys.md]] | Navigasyon hub'i burasi; anahtar haritasi orada |
| 10 | "Hangi dosya hangi sablonla uretilir?" | [[.templates/index|.templates/index.md]] | Sablon envanteri ayri envanterdir |
| 11 | "Davranis tanimi var mi, testi ne test eder?" | [[spec/index|spec/index.md]] | Davranis spec'te yasar; kod onun uygulamasidir |
| 12 | "Katman yonu hangi okla gosteriliyor?" | [[checklists/layer-boundary-check|checklists/layer-boundary-check.md]] | Yon matrisi tek kaynaktan gelir (ADR-028) |
| 13 | "Olculmus sonuc hangi raporada?" | [[ops/log|reports/index.md]] | Olcum rapora, navigasyon index'e yazar |
| 14 | "K3'in 618 girdisinin detayi nedir?" | [[architecture/layer-5000/index|reports/k3-618-ham-envanter.md]] | Kapsam disi oldugu gerekcesi ADR-028 §2.2'dedir |
| 15 | "Hangi anahtari hangi dosya cevaplar, terimler, surec adimlari?" | [[governance/keys|keys.md]] · [[definitions/glossary|glossary.md]] · [[ops/index|playbooks/index.md]] | Uc ayri navigasyon katmani, hicbiri bu dosyada degil |
| 16 | "Prompt sablonu / bilgi notu var mi?" | [[.templates/index|prompts/index.md]] · [[knowledge/index|knowledge/index.md]] | Uretim sablonu ile bilgi notu ayni sey degildir |
| 17 | "Roller kime bagli, kim karar verir?" | [[definitions/role|ROLE.md]] | Rol hiyerarsisi burada ozetlenmez |
| 18 | "Orkestrasyon matrisi gate ile nasil esleniyor?" | [[definitions/engine|engine.md]] | Yurutme matrisi ayri dosyadir |
| 19 | "Uzun is nasil bolumlenir, derin dusme protokolu ne?" | [[definitions/ultra-thinking|ULTRA-THINKING.md]] | Protokol burada tekrar yazilmaz |
| 20 | "Urun ne icin var, 7 ekran hangileri, proje siniri ne?" | [[definitions/vision|VISION.md]] · [[definitions/projects|PROJECTS.md]] | Vizyon "neden", kapsam "ne"; ikisi de burada degil |

**Kapsam disi listesini guncelleme:** satiri `[[claude|CLAUDE.md]]` G9 kuralina gore silmek yerine, cevabin tasindigi dosyayla birlikte guncellenir; hedef dosya silinmis olabilirse satir `DOĞRULAMA GEREKLI` etiketiyle isaretlenir.

---

## §2 Dosya indeksi

### §2.1 Kok belgeler (14)

| # | Dosya | Tek satir ozet |
|---|---|---|
| 1 | [[index|index.md]] | Bu dosya — vault navigasyonunun tek girisi (SSOT hub) |
| 2 | [[claude|CLAUDE.md]] | AI anasaydasi — hard guardrail G1..G12 |
| 3 | [[governance/workflow|WORKFLOW.md]] | Surec, fazlar ve kapilar GATE-000..005 |
| 4 | [[definitions/role|ROLE.md]] | Mutlak rol tanimi + roller arasi oncelik |
| 5 | [[definitions/agents|AGENTS.md]] | Ajan dagitim matrisi, cikti kutusu, 3-STRIKE, eszamanlilik |
| 6 | [[definitions/vision|VISION.md]] | Urun vizyonu, problem–cozum matrisi, 7 ekran |
| 7 | [[definitions/projects|PROJECTS.md]] | Proje kapsami, hedefler, bilesenler |
| 8 | [[.decisions/brain|brain.md]] | ADR ozetleri + muhendislik kisitlari |
| 9 | [[governance/keys|keys.md]] | Anahtar kelime → hedef dosya haritasi |
| 10 | [[definitions/glossary|glossary.md]] | Terimler sozlugu (TR/EN), tek satir/terim |
| 11 | [[memory/memory|MEMORY.md]] | Kalici bellek — oturumlar arasi durum |
| 12 | [[ops/log|log.md]] | Kural degisikligi + Quality Report gunlugu |
| 13 | [[definitions/engine|engine.md]] | Orkestrasyon yurutme matrisi (gate ↔ part) |
| 14 | [[definitions/ultra-thinking|ULTRA-THINKING.md]] | Derin dusme protokolu + uzun is sablonlari |

### §2.2 Kararlar (.decisions) — 29 ADR

| # | Dosya | Tek satir ozet |
|---|---|---|
| 1 | [[.decisions/adr-index|.decisions/adr-index.md]] | 29 ADR'nin tamamini listeleyen tek indeks (28 accepted + ADR-020 superseded) |
| 2 | [[.decisions/accepted/adr-028-layered-baseline-9-projects|.decisions/accepted/ADR-028-layered-baseline-9-projects.md]] | **EN ONEMLI karar:** katman bazli temel — 8 makro katman, 9 uretim + 1-2 test projesi |

> Kalan 26 ADR (`ADR-001` … `ADR-027`) `accepted/` altindadir; dosya adlari ve tek satir ozetleri [[.decisions/adr-index|adr-index.md]] uzerinden okunur — burada tek tek tekrarlanmaz (SRP).

### §2.3 Kurallar (.rules) — 17 dosya

| # | Dosya | Tek satir ozet |
|---|---|---|
| 1 | [[.rules/index|.rules/index.md]] | Kural envanteri — 16 kural dosyasinin indeksi |
| 2 | [[.rules/coding-standards|.rules/coding-standards.md]] | Genel C# kod yazim standartlari |
| 3 | [[.rules/solid-rules|.rules/solid-rules.md]] | SOLID ilkeleri ve ihlal sonuclari |
| 4 | [[.rules/security-rules|.rules/security-rules.md]] | Guvenlik kurallari (OWASP odakli) |
| 5 | [[.rules/testing-rules|.rules/testing-rules.md]] | Test yazim kurallari + coverage beklentisi |
| 6 | [[.rules/data-access-rules|.rules/data-access-rules.md]] | ADO.NET + Dapper, parametreli SQL, migration |
| 7 | [[.rules/performance-guidelines|.rules/performance-guidelines.md]] | Performans olcum ve optimizasyon rehberi |
| 8 | [[.rules/logging-rules|.rules/logging-rules.md]] | Yapilandirilmis loglama kurallari |
| 9 | [[.rules/error-handling-rules|.rules/error-handling-rules.md]] | Hata yakalama ve yonetimi kalibi |
| 10 | [[.rules/naming-conventions|.rules/naming-conventions.md]] | Adlandirma kurallari (tip, alan, dosya) |
| 11 | [[.rules/async-rules|.rules/async-rules.md]] | Async/await, iptal ve kilit kurallari |
| 12 | [[.rules/caching-rules|.rules/caching-rules.md]] | Onbellek stratejileri ve gecersiz kilma |
| 13 | [[.rules/configuration-rules|.rules/configuration-rules.md]] | Yapilandirma yonetimi ve sirasi |
| 14 | [[.rules/deployment-guide|.rules/deployment-guide.md]] | Derleme ve dagitim adimlari |
| 15 | [[.rules/devexpress-rules|.rules/devexpress-rules.md]] | DevExpress v26.1, Ribbon stili Office2010 |
| 16 | [[.rules/accessibility-rules|.rules/accessibility-rules.md]] | Erisilebilirlik kurallari |
| 17 | [[.rules/ui-rules|.rules/ui-rules.md]] | WinForms/arayuz kurallari ve ekran adlari |

### §2.4 Sablonlar (.templates) — 36 dosya

| # | Dosya | Tek satir ozet |
|---|---|---|
| 1 | [[.templates/index|.templates/index.md]] | Sablon envanteri — hangi sablon nerede, kim yazar |
| 2 | [[.templates/new-doc-contract|.templates/new-doc-contract.md]] | Yeni MD dosyasi sozlesmesi — frontmatter, link, boyut, yasaklar (GATE-004a) |

> Kalan 34 sablon `.templates/` altindaki 10 klasorundedir (30 sablon: `adr/`, `agents/`, `architecture/`, `backend/`, `documentation/`, `frontend/`, `infrastructure/`, `query/`, `testing/`, `ui-design/`); kalan 4 sablon kokedir (`batch-manifest-gate004a.md`, `session-log-template.md`, `ascii-box-drawing-prompt.md`, `unicode-box-drawing-prompt.md` [EMEKLI]); tam liste [[.templates/index|sablon envanterinde]] yasar. Bu dosyaya eklenecek her yeni MD once `new-doc-contract.md`'den gecer.

### §2.5 Klasor indeksleri (spec, architecture, checklists, playbook/prompt/knowledge)

| # | Dosya | Tek satir ozet |
|---|---|---|
| 1 | [[spec/index|spec/index.md]] | 40 spec kaydinin indeksi (test stratejisi dahil) |
| 2 | [[architecture/index|architecture/index.md]] | Mimari indeks — 00-overview, catalog ve l0..l7 katmanlari |
| 3 | [[checklists/index|checklists/index.md]] | 31 checklist'in indeksi |
| 4 | [[checklists/layer-boundary-check|checklists/layer-boundary-check.md]] | Katman sinir kontrolu — mimari her cikti buradan gecer |
| 5 | [[ops/index|playbooks/index.md]] | Playbook indeksi (bu gorevde guncelleniyor) |
| 6 | [[.templates/index|prompts/index.md]] | Prompt indeksi (bu gorevde guncelleniyor) |
| 7 | [[knowledge/index|knowledge/index.md]] | Bilgi notu indeksi (bu gorevde guncelleniyor) |
| 8 | [[ops/todos/index|TODOS/index.md]] | Adim adim gorev fazlari (5 faz + indeks); "su dosyayi yap" kurali burada |

### §2.6 Raporlar (reports) — 8 dosya

| # | Dosya | Tek satir ozet |
|---|---|---|
| 1 | [[ops/log|reports/00-kapsam-analizi.md]] | Vault kapsam analizi ve olcum kanitlari |
| 2 | [[ops/log|reports/01-web-kaynaklari.md]] | Web kaynak listesi ve degerlendirmesi |
| 3 | [[ops/log|reports/02-vault-kurulum-raporu.md]] | Vault kurulum asamasi raporu |
| 4 | [[definitions/PROJECTS|reports/08-risk-kayit-defteri.md]] | Acik risklerin kayit defteri |
| 5 | [[ops/log|reports/09-teslim-raporu.md]] | Teslim raporu — ne teslim edildi |
| 6 | [[ops/log|reports/10-bilinen-eksikler.md]] | Bilinen eksikler ve kabul edilmis bosluklar |
| 7 | [[ops/log|reports/11-sonraki-adimlar.md]] | Sonraki adim plani |
| 8 | [[architecture/layer-5000/index|reports/k3-618-ham-envanter.md]] | K3-618 ham envanter (bu gorevde yaziliyor) |

### §2.7 Olcum scriptleri (scripts) — 5 dosya

| # | Dosya | Ne yapar |
|---|---|---|
| 1 | [[scripts/check-links.ps1|scripts/check-links.ps1]] | Bozuk ic link taramasi — hedef 0 |
| 2 | [[scripts/measure-vault.ps1|scripts/measure-vault.ps1]] | Satir/KB/frontmatter/mojibake olcumu (canli sayac) |
| 3 | [[scripts/measure-vault-u8.ps1|scripts/measure-vault-u8.ps1]] | UTF-8 varyant olcum |
| 4 | [[scripts/count-components.ps1|scripts/count-components.ps1]] | Bilesen katalog sayaci — PASS kosulu |
| 5 | [[scripts/find-mojibake.ps1|scripts/find-mojibake.ps1]] | Bozuk byte taramasi — hedef 0 |

### §2.8 Envanter notlari — her satir "ne cevaplar / kim bakar / sik hata"

> §2.1–§2.7 satirlari dosyanin **kimligini** (nedir) soyler; bu tablo **kullanimini** (nasil sorulur) soyler. Iki tablo ayni bilgiyi tekrarlamaz: kimlik bir kez yazilir, kullanim her gun okunur.

| # | Dosya | Sorusu neye cevap verir | Kim bakar | Sik yapilan hata |
|---|---|---|---|---|
| 1 | [[index|index.md]] | "X nerede?" | architect + docs-writer | Bu dosyaya kural yazmak; kural [[.rules/index|.rules/index.md]] klasorundedir |
| 2 | [[claude|CLAUDE.md]] | "Bunu yapabilir miyim, kaniti nedir?" | architect (degisiklik = kullanici) | Cilesen baska bir dosya one cikarmak; bu dosya kazanir |
| 3 | [[governance/workflow|WORKFLOW.md]] | "Siradaki gate ne, kabul kriteri ne?" | architect + kullanici (kapanis) | Gate'i ajanin kapatmasi; kapanis yetkisi kullaniciya aittir |
| 4 | [[definitions/role|ROLE.md]] | "Kim kimin ustunde, karari kim verir?" | architect | Rol adlarini burada aramak; yetki matrisi AGENTS §1'dedir |
| 5 | [[definitions/agents|AGENTS.md]] | "Bu is hangi ajana gider, cikti kutusu nedir?" | orchestrator | Kutusuz cikti gondermek; cikti islenmez ve ayni kutuyla tekrar istenir |
| 6 | [[definitions/vision|VISION.md]] | "Urun ne icin var, 7 ekran hangileri?" | product + ui-designer | Kapsam genisletmek; yeni ekran = kapsam degisimi, gate yeniden acilir |
| 7 | [[definitions/projects|PROJECTS.md]] | "Proje siniri, hedefler, bilesenler neler?" | architect | Vizyon ile kapsami karistirmak; VISION "neden", PROJECTS "ne" der |
| 8 | [[.decisions/brain|brain.md]] | "Muhendislik kisiti ve ADR ozeti nedir?" | architect | Kisiti burada okuyup uygulamayi baska yerde aramak; uygulama kural dosyasindadir |
| 9 | [[governance/keys|keys.md]] | "Bu anahtar hangi dosyaya gider?" | docs-writer | Anahtari ezberlemek; harita orada yasar, hafiza degil |
| 10 | [[definitions/glossary|glossary.md]] | "Bu terim TR/EN ne demek?" | docs-writer | Terimi baska dosyada yeniden tanimlamak; sozluk tek kaynaktir |
| 11 | [[memory/memory|MEMORY.md]] | "Onceki oturum nerede kalmisti?" | orchestrator | Gecici notu buraya yazmak; gecici not MEMORY'de kalir, kalici durum da oradadir |
| 12 | [[ops/log|log.md]] | "Bu degisiklik ne zaman oldu, kaniti ne?" | reviewer | Kaniti hatirlamak; kanit hatirlanmaz, gunluge yazilir |
| 13 | [[definitions/engine|engine.md]] | "Hangi gate hangi part ile esleniyor?" | orchestrator | Gate tanimini burada aramak; tanim [[governance/workflow|WORKFLOW.md]] §2'dedir |
| 14 | [[definitions/ultra-thinking|ULTRA-THINKING.md]] | "Uzun is nasil bolumlenir, uzun is sablonu nedir?" | architect | Uzun isi tek adimda yutmak; bolumleme sablonu oradadir |
| 15 | [[.rules/index|.rules/index.md]] | "Kuralin tam metli hangi dosyada?" | reviewer | Kurali baska dosyaya kopyalamak; yalnizca baglanti verilir |
| 16 | [[.decisions/adr-index|.decisions/adr-index.md]] | "29 ADR'den hangisi hangi karari vermis?" | architect | ADR'yi okumadan karar yeniden tartismak; once indekse bakilir |
| 17 | [[.decisions/accepted/adr-028-layered-baseline-9-projects|.decisions/accepted/ADR-028-layered-baseline-9-projects.md]] | "Katman, proje, TFM ve K3 kapsam disiligi nedir?" | architect | Eski ADR-013'u tek kaynak sanmak; ADR-028 onayli ve baglayicidir |
| 18 | [[.templates/index|.templates/index.md]] | "Hangi dosya hangi sablonla uretilir?" | docs-writer | Sablonsuz dosya uretmek; once sablon secilir, sonra icerik yazilir |
| 19 | [[.templates/new-doc-contract|.templates/new-doc-contract.md]] | "Frontmatter, link ve boyut sozlesmesi nedir?" | docs-writer | Bu dosyayi okumadan toplu uretim yapmak; uretim RED edilir |
| 20 | [[spec/index|spec/index.md]] | "Davranis tanimi var mi, testi ne test eder?" | architect | Davranisi kodda aramak; davranis spec'te, kod yalnizca uygulamadir |
| 21 | [[checklists/layer-boundary-check|checklists/layer-boundary-check.md]] | "Katman siniri hangi sorularla kontrol edilir?" | reviewer | Listeyi calistirmeden "temiz" demek; liste calistirilmadan kanit yoktur |
| 22 | [[architecture/index|architecture/index.md]] | "Katman, dugum ve katalog haritasi nedir?" | architect | Klasor adindan katman cikarmak; N-kod eslesmesi katalogdadir |
| 23 | [[ops/log|reports/index.md]] | "Olculmus sonuc hangi raporada duruyor?" | docs-writer | Olcumu bu dosyaya yazmak; olcum rapora, navigasyon index'e yazilir |
| 24 | [[ops/index|playbooks/index.md]] | "Surec adimlari hangi sira ile yurutulur?" | orchestrator | Adimlari akilda tutmak; sira playbook'da yasar |
| 25 | [[.templates/index|prompts/index.md]] | "Prompt sablonu hangi dosyada?" | prompt-engineer | Prompt'u serbest yazmak; sablon once, varyasyon sonra |
| 26 | [[knowledge/index|knowledge/index.md]] | "Bu bilgi notu daha once yazildi mi?" | docs-writer | Ayni bilgiyi ikinci kez yazmak; once indekste aranir |

**Neden 26 satir tek tek yazildi:** "hangi dosya" sorusu en cok tekrar eden sorudur; cevabi tabloda olmayan satir, okuyucuyu §3'e ve oradan [[governance/keys|keys.md]]'e tasir. Tabloda olmayan bir dosya envantere eklenmeden linklenirse bozuk link uretir (G4).

---

## §3 Hizli yol tarifleri (12 ornek soru)

| # | Soru | Git | Ne bulursun |
|---|---|---|---|
| 1 | "EF Core yasak mi?" | [[.decisions/brain|brain.md]] | ADR ozetleri — kisa cevap: veri erisimi ADO.NET + Dapper'dir |
| 2 | "Hangi gate'den geciyorum?" | [[governance/workflow|WORKFLOW.md]] | GATE-000..005 kosullari ve cikti kutusu |
| 3 | "Bu is hangi ajana gider?" | [[definitions/agents|AGENTS.md]] | Is tipi → ajan matrisi + cikti kutusu zorunlulugu |
| 4 | "Katman baglanti kurali nedir?" | [[checklists/layer-boundary-check|checklists/layer-boundary-check.md]] | Katman sinir sorulari (ADR-028 baglaminda) |
| 5 | "ADR listesi nerede?" | [[.decisions/adr-index|.decisions/adr-index.md]] | 29 ADR'nin tamami, kabul durumu ve tek satir ozeti |
| 6 | "K3 envanteri neden katalogda degil?" | [[architecture/layer-5000/index|reports/k3-618-ham-envanter.md]] | 618 girdilik ham liste + kapsam disi kararinin kaniti |
| 7 | "CPM'te hangi paketler kilitli, test paketleri eksik mi?" | [[.decisions/accepted/adr-028-layered-baseline-9-projects|.decisions/accepted/ADR-028-layered-baseline-9-projects.md]] | §2.3 — test paketlerinin `PackageVersion` satirlari ve NU1008 riski |
| 8 | "Hangi TFM'lerle derliyorum?" | [[.decisions/accepted/adr-028-layered-baseline-9-projects|.decisions/accepted/ADR-028-layered-baseline-9-projects.md]] | §2.1 — `net48` + `net10.0-windows` iki bacagi ve proje listesi |
| 9 | "DevExpress hangi surum, hangi ribbon stili?" | [[.rules/index|.rules/index.md]] | Kural envanteri — surum ve ribbon stili oradan tek satirda iner |
| 10 | "Test stratejisi nerede?" | [[spec/index|spec/index.md]] | 40 spec icinde test stratejisi de var; kapsam ve coverage beklentisi orada |
| 11 | "Vault'ta kac MD var, bozuk linki nasil bulurum?" | [[index|index.md]] | Bu dosya §4 — uc olcum komutu ve hedefleri canli olarak orada durur |
| 12 | "Yeni ADR/dosya acmadan once hangi sozlesmeden geciyorum?" | [[.templates/new-doc-contract|.templates/new-doc-contract.md]] | Frontmatter 11 alan, izinli linkler, boyut bandi ve yasaklar |

**Kural:** soru listede yoksa [[governance/keys|keys.md]] aranir (anahtar kelime → dosya); o da cevap vermezse soru `log.md`'ye kayit acar ve `architect` karar verir.

### §3.1 Sira: bu dosyayla calisma akisi (5 adim)

| # | Adim | Yapilan is | Kanit |
|---|---|---|---|
| 1 | Ne aradigini soru olarak yaz | §3'te eslesme var mi? | Bulunan satir no |
| 2 | Yoksa [[governance/keys|keys.md]] ara | Anahtar kelime → dosya | Hedef dosya yolu |
| 3 | Hedefi oku, baglantiyi takip et | §1 haritasi disina cikma | Okunan dosya adi |
| 4 | Dosya uretilecekse | [[.templates/new-doc-contract|.templates/new-doc-contract.md]] + §1.1 | Frontmatter 11 alan |
| 5 | Bitti ise | §4 sayaclarini calistir | 4 komut ciktisi |

### §3.2 Okuma sirasi rehberi — vault'u ilk kez acan biri neyi ONCE okur

> Siranin degistirilemez gerekcesi: her asamanin ciktisi bir sonrakinin girdisidir. Asama atlansa cikti da atlanmis olur ve is sonradan geri alinir.

| # | Asama | Once oku | Neden sira bu | Bu asamadan neyle cikilir |
|---|---|---|---|---|
| 1 | Ne yasak, ne kanit | [[claude|CLAUDE.md]] | 12 guardrail'i okumadan yazilan is, sonradan heba olur; yasaklar bastan bilinmezse duzeltme pahaliya gelir | Yasak listesi + kanit kutusu bicimi |
| 2 | Siradaki is ne, kapidan gececek mi | [[governance/workflow|WORKFLOW.md]] | Guc bu dosyada degil; kapali gate'de yazilan satir geri alinir | Gate adi + kabul kriteri + kapanis sahibi |
| 3 | Kim ne yapar | [[definitions/role|ROLE.md]] + [[definitions/agents|AGENTS.md]] | Is tipine gore ajani bulmadan yapilan is yanlis uzmanlikta kalir ve yanlis kanit uretir | Is tipi -> ajan satiri + cikti kutusu |
| 4 | Mimari sinir nerede | [[architecture/index|architecture/index.md]] + [[checklists/layer-boundary-check|checklists/layer-boundary-check.md]] | Kod oncesi sinir belli olmali; katman siniri sonradan duzeltilmez, kod yeniden yazilir | Katman yonu + yasak oklar + N-kod eslesmesi |
| 5 | Nerede kalmistim, kaniti nereye yazayim | [[memory/memory|MEMORY.md]] + [[ops/log|log.md]] | Oturum bilgisi sizden onceki oturumdan gelir; kanit da oraya degil gunluge dusulur | Kalici durum + tarihli Quality Report |

**Sira neden degismez (3 gerekce):**

- **1. asama atlanirsa** 4. asamada yazilan kod veri erisimi kuralina takilir ve is en pahali yerde (kod sonrasi) geri alinir.
- **2. asama atlanirsa** ajan gate'i hazirlamadan is yapar; kapanis yetkisi zaten kullanicidadir, hazirliksiz is beklemede kalir.
- **5. asama atlanirsa** "bitti" diyen kaniti bulamaz; kanitsiz beyan uydurmadir ve madde silinir.

**Asama disi calisma:** asama atlama gerekiyorsa nedeni `log.md`'ye tek satir yazilir; gerekcesiz atlama [[claude|CLAUDE.md]] G5 ihlalidir.

### §3.3 Yapilan-isler defteri (2026-09-29 itibariyle)

> Bu tabloya yalniz **bitmis ve kaniti olan** is girer; uzerinde calisilan is girmez (calisan is [[memory/memory|MEMORY.md]]'de yasar). Kaniti bulunmayan satir [[ops/log|log.md]] uzerinden silinir.

| # | Yapilan is | Ne kazandirdi | Kanit / kaynak |
|---|---|---|---|
| 1 | 3 tur / 21 ajanlik mimari tartismasi (10 senior -> 5 senior -> 3 junior + 3 ajans) | Katman ve proje sayisi rakip taslaka karsi savunulabilir hale geldi | [[.decisions/adr-index|.decisions/adr-index.md]] — ADR-028 §3, tur tur oylama sonucu |
| 2 | ADR-028 kabul edildi | 8 makro katman / 52 dugum / 9 uretim + 1-2 test projesi kilitlendi | [[.decisions/accepted/adr-028-layered-baseline-9-projects|ADR-028 §2.1]] |
| 3 | Vault kurulumu tamamlandi | 14 klasor + 14 kok belge iskeleti oturdu, olcum scriptleri baglandi | [[ops/log|reports/index.md]] |
| 4 | 29 ADR port edildi | Karar gecmisi tek indekste toplandi | [[.decisions/adr-index|.decisions/adr-index.md]] |
| 5 | 40 spec port edildi | Davranis tanimlari koddan ayri yerde yasamaya basladi | [[spec/index|spec/index.md]] |
| 6 | 31 checklist port edildi | Gate kriterleri kisiye degil listeye baglandi | §1 haritasindaki `checklists/` satiri |
| 7 | 17 kural dosyasi port edildi | Kod kurallari tek kaynagda toplandi | [[.rules/index|.rules/index.md]] |
| 8 | 36 sablon port edildi | Uretim oncesi sozlesme zorunlu hale geldi | [[.templates/index|.templates/index.md]] |
| 9 | 8 dosyada 20 mojibake eslestirmesi ASCII kacisa cevrildi | Bozuk byte izi temizlendi, olcum hedefi sifirlandi | Tarama hedefi 0 — §4 komutu ile dogrulanir |
| 10 | K3-618 ham envanteri rapora tasinip kapsam disi ilan edildi | 618 DLL listesi katalog sayimina karismadi | [[architecture/layer-5000/index|reports/k3-618-ham-envanter.md]] + ADR-028 §2.2 |
| 11 | Katalog sayimi olculdu: 52 dosya / 5440 kayit / 0 bozuk format / PASS | Sayim birimi `N###` kaydina indi; ikinci sayim birimi reddedildi | ADR-028 §1 (C4) — calistirilan `count-components` ciktisi |

**Defter kurali:** "yapildi" yazilmadan once satirin kanit sutunu dolu olmalidir; dolu degilse sutun `DOĞRULAMA GEREKLI` ile isaretlenir ve satir kapanmaz.

---

## §4 Kalite kapisi kutusu

```text
+--------------------------------------------------+
| SAYAC          | HEDEF                | KANIT    |
+--------------------------------------------------+
| Toplam MD      | (sayi sabit degil)   | scripts/measure-vault.ps1 |
| Bozuk link     | 0                    | scripts/check-links.ps1   |
| Mojibake       | 0                    | scripts/find-mojibake.ps1 |
| Katalog kaydi  | count-components PASS| scripts/count-components.ps1 |
+--------------------------------------------------+
```

| # | Sayac | Hedef | Canli sayi nereden |
|---|---|---|---|
| 1 | Toplam MD dosyasi | sabit sayi degil — trend | `pwsh -File scripts/measure-vault.ps1` calistir |
| 2 | Bozuk link | **0 hedefi** | `pwsh -File scripts/check-links.ps1` |
| 3 | Mojibake | **0 hedefi** | `pwsh -File scripts/find-mojibake.ps1` |
| 4 | Katalog kaydi | **PASS** | `pwsh -File scripts/count-components.ps1` |

- **Canli sayi bu dosyaya yazilmaz:** sayac degerleri anliktir; tiklandiginda eski bir sayi gostermemek icin burada rakam yerine komut verilir. Guncel toplam her zaman script ciktisindan okunur.
- **Olculmemis iddia yasak:** `scripts/measure-vault-u8.ps1` (UTF-8 varyanti) disindaki hicbir kaynak "toplam dosya = X" diye iddia edilemez; eden rapor `reports/10-bilinen-eksikler.md` uzerinden isaretlenir.
- **Gecen kosul:** 4 sayacin 4'ü de hedefte ise GATE-004a kapanis onayi verilebilir; aksi halde kapanis `checklists/gate-004a-vault-kapanis.md` uzerinden bekletilir.

### §4.1 Canli olcum — calistirilacak uc komut ve okuma bicimi

> Bu uc komut calistirilmadan hicbir sayi bu dosyaya yazilmaz; ciktilari [[ops/log|log.md]]'e Quality Report olarak dusulur. Asagidaki `NNN` yer tutucudur, olcen degeri ekrandaki ciktidir.

```text
komut 1   pwsh -File scripts/measure-vault.ps1
  oku     : toplam MD, eksik frontmatter sayisi, mojibake sayisi
  hedef   : NoFrontmatter = 0   ve   Mojibake = 0
  kayit   : ekrandaki toplam deger yazilir; "toplam X dosya var" buradan okunmaz

komut 2   pwsh -File scripts/check-links.ps1
  oku     : BOZUK = <sayi>
  hedef   : BOZUK = 0
  kayit   : BOZUK > 0 ise hangi dosyada oldugu birlikte yazilir

komut 3   pwsh -File scripts/count-components.ps1
  oku     : kayit sayisi + PASS/RED durumu
  hedef   : PASS   (katalogdan tek satir silinirse RED uretir)
  kayit   : PASS/RED ve katalog dosya sayisi birlikte yazilir
```

**Dorduncu sayac (diyagram):** Unicode kutu cizimi (`U+2500-U+257F`) taramasi `measure-vault.ps1`'de bir sutun olarak cikmaz; tarama [[claude|CLAUDE]] §2 satir 8'deki komutla yapilir: `Select-String -Path .ai\**\*.md -Pattern ('[' + [char]0x2500 + '-' + [char]0x257F + ']')` — hedef **0 satir**. Ayrica `scripts/measure-vault-u8.ps1` (UTF-8 varyanti) ve `scripts/find-mojibake.ps1` ayni ailede olcum yapar; ikisi de ayni sonucu vermezse fark `log.md`'ye yazilir ve sonuca guvenilmez.

**Yorum kurali (3 satir):**

```text
sayi hedefteyse     -> "hedefte" yaz, gate kanitina ekle
sayi hedefte degilse-> farki yaz, nedenini arama, duzeltmeye gec
cikti yoksa         -> "olculmedi" yaz; olculmemis sayi girmez (G9)
```

### §4.2 Sayac -> gate eslesmesi (hangi sayac kapiyi besler)

| # | Sayac | Besledigi gate / kural | Kapatan |
|---|---|---|---|
| 1 | Toplam MD + eksik frontmatter = 0 + bozuk link = 0 + mojibake = 0 | GATE-004a (vault green) | kullanici |
| 2 | Diyagramlarda 0 Unicode kutu cizimi | GATE-004a ayni kabul kriteri icinde | kullanici |
| 3 | Katalog kaydi PASS (sifir marj) | ADR-028 §2.3 — katalog-kod eslesme denetimi | reviewer kanitiyla kullanici |
| 4 | Iki bacak build 0 hata (`net48` ve `net10.0-windows`) | GATE-004 (build green + ilk commit) | kullanici |
| 5 | Test + mimari testleri 0 ihlal | GATE-005 (test green) | kullanici |

**Gecis kurali:** 1–3 hedefte degilken 4–5'e gecilmez; olcumun arsiv hali [[ops/log|reports/index.md]] uzerindedir, bu dosyada kopyalanmaz. Sayac hedefte degilse kapi kapanmaz, is [[claude|CLAUDE.md]] §4.1'deki `dur/bekle` durumuna duser.

---

## §5 Dogrulama

- [ ] Frontmatter 11 alan tam, `authority: primary`, `updated: 2026-10-07`.
- [ ] Tek H1 + "Tek cumlelik sorumluluk" satiri + §0 Ozet (5 madde).
- [ ] §1 haritasi 15 klasoru de kapsiyor; `.sql/` notu dusulmus.
- [ ] §2'deki tum linkler yalniz bu envanterdeki hedeflere gidiyor; bozuk link = 0.
- [ ] §4'te uydurma rakam yok; canli sayi script komutuyla veriliyor.
- [ ] ASCII tablo (`| - +`), Unicode kutu cizimi yok; Turkce, mojibake yok.
- [ ] Boyut 30–88 KB bandinda — kok belge sozlesmesi (`root-md-template.md` §2 hedefi 30–45 KB; olcum: `(Get-Item .ai/index.md).Length/1KB`). (Onceki 14–18 KB esigi kok belge genisletmesiyle gecersiz kalmistir.)
- [ ] Prompt arsivi kaynaklari §8'e referans olarak bagli; hicbir link silinmemis (kirik olsa bile korunmus).

### §5.1 Red sinyalleri (bu dosya bozulursa)

| # | Sinyal | Neden | Duzeltme |
|---|---|---|---|
| 1 | Listede olmayan bir hedefe link | Bozuk link uretimi (G7) | Baglantiyi sil ya da envantere ekle |
| 2 | Sabit "toplam dosya = X" rakami | Olculmemis iddia | Komuta geri don, script ciktisini ver |
| 3 | Unicode kutu cizimi (`U+2500`..) | ASCII kutu/Tablo sozlesmesi | `\| - +` ile yeniden ciz |
| 4 | Envantersiz yeni klasor/filos satiri | §1 15 klasorle kilitli | `architect` onayi al, sonra ekle |
| 5 | §2 ozet satirinin uydurma sayi icermesi | DOĞRULAMA GEREKLIDIR | Sayiyi sil veya script ile olc |

### §5.2 Boyut bandi ve kopya denetimi (bu dosyanin kendi kapisi)

| # | Kontrol | Hedef | Nasil bakilir |
|---|---|---|---|
| 1 | Boyut | 30–88 KB bandinda (kok belge sozlesmesi) | `(Get-Item .ai/index.md).Length/1KB` — olcen deger yazilir, tahmin degil |
| 2 | Frontmatter | 8 alan, `authority: primary`, `updated: 2026-09-29` | Ilk satir `---`, son satir `---` |
| 3 | Kopya | Baska vault'tan metin yapistirmasi yok | Bilgiden faydalan, cumleyi kendin yaz (CLAUDE G1) |
| 4 | Link | Yalniz envanterde listelenen hedefler | `scripts/check-links.ps1` -> BOZUK = 0 |
| 5 | Kod | Emoji yok, Unicode kutu cizimi yok | Diyagramlar yalniz `|`, `-`, `+` ile kurulur |
| 6 | Sayi | Uydurma rakam yok | Kaynaksiz sayi `DOĞRULAMA GEREKLI` olur |

**Bandin neden 30 KB altinda RED:** kisa index iki is yapamaz — hem soruyu hangi dosyaya gonderecegini soyler, hem de o dosyanin konumunu dogrular. 30 KB altinda ikinci is genelde atlanir ve okuyucu dosya taramasina duser; bu da tam olarak bu dosyanin onlemek istedigi davranistir.

**Bandin neden 88 KB ustunde RED:** 88 KB ustu bir index, icinde yasayan bilgiye donusur; o an SSOT ihlali baslar (kural burada yasamaya baslarsa [[.rules/index|.rules/index.md]] anlamsizlasir). Bu yuzden derinlik §2.8, §3 ve §7 gibi **navigasyonu agirlastiran** tablolara, kural metnine degil eklenir.

---

## §6 SSS (6 soru)

**S1 — Aradigim bulamazsam ne yapayim?** Once [[governance/keys|keys.md]]'de anahtar kelimeyi ara; bulamazsan soruyu [[ops/log|log.md]]'ye kayit ac ve `architect` kararini bekle. Kendi tahmininle devam etme — etiketsiz iddia [[claude|CLAUDE.md]] G9'dur ve madde silinir.

**S2 — Yeni bir kok dosya eklemem gerekir mi?** Nerdeyse hep **hayir**. Kok listesi 14 dosyayla kilitlidir; yeni icerik mevcut bir klasorun indeksine yazilir. Istisna (`architect` onayi) §1.1'de tanimlidir.

**S3 — Bu dosyayi kim gunceller, degisiklik nasil kayda gecer?** Navigasyon `docs-writer`, envanter degisikligi `architect`; her degisiklik [[ops/log|log.md]]'e Quality Report ile dusulur (§0 son madde). Kayitsiz degisiklik geri alinir.

**S4 — Olcum scripti hata verirse sayiyi nereden alayim?** Hicbir yerden. Sonucu [[ops/log|reports/index.md]] uzerinden isaretleyip `DOĞRULAMA GEREKLI` yaz; sayiyi elle doldurma. Script hatasi raporlanir, dosya degil script duzeltilir.

**S5 — Bu dosya ile [[claude|CLAUDE.md]] cakusursa hangisi kazanir?** [[claude|CLAUDE.md]] kazanir; bu dosya yalnizca "nereye gidilir"u soyler, "ne yasak"i soylemez. Cakusma [[claude|CLAUDE.md]] G10 etiketiyle raporlanir.

**S6 — §2.8 tablosunda olmayan bir dosya gordum, linkleyebilir miyim?** Linkleme. Once §1 haritasina ve §2 envanterine ekle (`architect` onayiyla), sonra linkle; envantersiz link bozuk link sayilir (G4).

---

## §7 Kirmizi ekip — sik yapilan 10 hata ve aninda duzeltmesi

| # | Hata | Belirtisi | Aninda duzeltme |
|---|---|---|---|
| 1 | Olculmemis sabit sayi yazmak | "toplam 500 dosya var" (komut ciktisi yok) | §4.1'deki komutu calistir, ciktisini ver |
| 2 | Envantersiz dosyaya link vermek | link taramasi BOZUK > 0 | Baglantiyi sil ya da once §2 envanterine satir ac |
| 3 | Kurali ikinci dosyaya kopyalamak | ayni madde iki yerde ayni kelimeyle | Birinde tut, otekine yalnizca baglanti ver |
| 4 | Yeni klasor/filos acmak | §1'de olmayan klasor satiri | `architect` onayi al, sonra §1'e ekle |
| 5 | Diyagrami Unicode ile cizmek | `U+2500-U+257F` taramasinda eslesme | `|`, `-`, `+` ile yeniden ciz |
| 6 | Mojibake'yi gormemek | iki bytelik ASCII artik metinde | [[ops/log|log.md]]'e kayit + kaynagi ASCII kacisa cevir |
| 7 | Ozet yazip cevap vermemek | soru hala acik, "detayli" denmis | §3 satirina in, hedef dosyayi oku, satiri guncelle |
| 8 | Kimlik ile kullanimi karistirmak | kural bu dosyada araniyor | §1.2 kapsam disi tablosuna bak |
| 9 | Kanitsiz "bitti" | komut ciktisi olmayan kapanis | §4.1'deki uc komutu calistir, sonucu yaz |
| 10 | Okuma sirasini atlamak | 1. asama okunmadan kod denemesi | §3.2'deki 5 asamayi bastan uygula |

**Ortak kural:** on maddeden herhangi biri fark edilirse is durur, satir [[ops/log|log.md]]'ye not edilir ve duzeltme ayni oturumda yapilir; ertelenen madde [[claude|CLAUDE.md]] G5 ile cakusur.

---

## §8 Referanslar

| # | Referans | Neden |
|---|---|---|
| 1 | [[claude|CLAUDE.md]] | Hard guardrail'lar — bu dosyanin ust siniri |
| 2 | [[governance/workflow|WORKFLOW.md]] | Gate ve cikti kutusu tanimlari |
| 3 | [[.templates/new-doc-contract|.templates/new-doc-contract.md]] | Frontmatter, link ve boyut sozlesmesi |
| 4 | [[.decisions/adr-index|.decisions/adr-index.md]] | 29 ADR — envanter dogrulamasi |
| 5 | [[ops/log|reports/00-kapsam-analizi.md]] | Olcum kanitlari |
| 6 | Prompt arsivi (kaynak) | `G:/…/versacoder proje planlaması ai.00..09.md` · `proje-ai.00.md` · `coremusic-vault-template.md` |
| ADR-031 | [[.decisions/draft/adr-031-visual-production-provider|ADR-031]] | gorsel uretim saglayicisi ADR (status: proposed, kapsam A+B+C) |
| kok README | [kok README](../readme.md) | depo README — hizli baslangic, Studio/IDE kimligi |
| kok CLAUDE | [kok CLAUDE](../claude.md) | kok kural dosyasi (golge — kural metni degil, baglanti) |
| kok AGENTS | [kok AGENTS](../agents.md) | kok ajan golgesi (Chat!=Agent!=LLM baglantisi) |
| kok WORKFLOW | [kok WORKFLOW](../workflow.md) | kok kapi ozeti (gate sirasi tek satir) |
