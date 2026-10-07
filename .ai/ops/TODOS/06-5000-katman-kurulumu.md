---
title: "TODOS Faz 06 - 5000+ Katman Kurulumu (ADR-030 v2 Uygulama Sirasi)"
type: todo
category: todos
status: active
version: 2.0.0
updated: 2026-10-07
authority: SSOT
scope: "ADR-030 v2 (4 kademe, 3 ekseni, 8·12·16·20 yon grubu) uygulamasinin asama sirasi: dogrulama -> K0 -> Faz A..D -> K1; her asama tek is sinirli, kapi durumu ve bagimliligi yazili, kanitsiz kapanmaz"
---

# Faz 06 — 5000+ Katman Kurulumu

**Zorunlu Baglantilar:** [TODOS index](index.md) · [Faz 02](02-5000-plani-sorulari.md) · [Faz 05](05-kod-ve-kalite.md) · [ADR-030](../../.decisions/accepted/adr-030-5000-layers-class-level.md) · [layering-5000-spec](../../spec/layering-5000-spec.md) · [layer-rules](../../.rules/layer-rules.md) · [layer-5000-check](../../checklists/layer-5000-check.md) · [WORKFLOW](../../governance/workflow.md)

> **Tek cumlelik sorumluluk:** 3 ekseni (tip · dugum · paket) ve yon grubunu kademeli olarak hedefe tasiyan, **her adimda kapi + kanit + bagimlilik** yazili asama listesidir.

### §0.1 Güncel Durum (2026-10-07 — ölçülmüş)

**Durum: RED — ADR-030 dosyasi diskte YOK**

| Gösterge | Değer | Hedef | Durum |
|---|---|---|---|
| .decisions/accepted/ADR-030* | **YOK** | VAR | RED — kaynak yok |
| .decisions/ icindeki dosyalar | **1** (yalniz draft/ADR-031) | ADR-001..031 | RED |
| checklists/layer-5000-check.md | **VAR** (7 madde turetildi) | 14 madde | KISMEN — orijinal 14 metin yok |
| count-components.ps1 | VAR (scripts/) | VAR | OK — betik mevcut |

> KAPANIS KOSULU: ADR-030 dosyasi uretilmeli ya da bu faz kapsam disi ilan edilmeli (kullanici karari). Orijinal 14 maddenin metni vaultta yok -> DOĞRULAMA GEREKLI.

> **Kaynak:** bu blok 2026-10-07 disk ölçümünden gelir (os.walk + git + dotnet); eski tarihli ölçümler tarihleriyle korunmuştur (append-only).


**Durum: AKTIF** (ADR-030 v2 + spec v2 + layer-rules v2.0.1 yazildi 2026-10-02; tum **kod adimlari GATE-003 onayina baglidir**).

---

## §0 Ozet (Answer First)

| # | Asama | Ne biter | Eksen etkisi (tip/dugum/fiziksel .dll) | Kapı |
|---|---|---|---|---|
| 0 | **K0** — cozum + test iskeleti + scaffold 45 | `.sln` uretilir, 45 cekirdek `.csproj` + 2 test acilir, 5000 kural uretilir | 5440/44/**45** | GATE-004 |
| A | **Faz A / N1** — 5 paket + scaffold 455 | Data · Config · Logging · Security · FileSystem + K0850-K1299 | 5440/44/**500** | GATE-005 |
| B | **Faz B / N2** — 7 paket + scaffold 1000 | AI · Auth · Caching · Context · Messaging · Network · Observability + K1300-K2299 | 5440/44/**1500** | GATE-005 |
| C | **Faz C / N3** — 7 paket + scaffold 1500 | Git · MCP · Plugins · Services · Integration · Backup · Versioning + K2300-K3799 | 5440/44/**3000** | GATE-005 |
| D | **Faz D / N4** — 10 paket + scaffold 2000 | CodeAnalysis · ProjectAnalysis · Refactoring · Testing · Templating · Documentation · Reporting · Diagram · Deployment · Learning + K3800-K5499 | 5440/44/**5000** | GATE-006 |
| K1 | **K1** — semsiye kapanisi | `VersaCoder.Infrastructure` bosaltilip **silinir** | 5440/44/**4999** | GATE-006 + **G7** |

> **3 eksenli drift'in kapanis sirasi:** fiziksel `.dll` (Y) eksigi K0 + A..D ile kapanir
> (45 → 5000); **dugum (N)** eksigi (44 → 5000) **ayri bir is** olarak Adim 28'de baslar
> ve katalog isidir (kod degil). Tip (K) ekseni **bugun 5440 PASS**, dokunulmaz.

**Sirali kural:** tek seferde **yalniz bir** asama; bir asamanin 3 kaniti (build · NetArchTest · sayac) yesil olmeden siradaki baslamaz.

---

## §1 Adimlar (her adim tek is sinirli)

### Blok 0 — On hazirlik (kod YOK, vault)

| # | Adim | Ne yapilir | Kanit | Bagimlilik | Sure |
|---|---|---|---|---|---|
| 1 | Olcum yenile | `count-components.ps1` + `gen-layer-5000.ps1` bugunun sayisi | **iki** PASS (5440 + 5834) | - | 2 dk |
| 2 | **GATE-003 onayi** | kullanici kod fazina onay verir | `log.md` "onay: ... tarih" satiri | - | - |
| 3 | **Dugum baslangici** | 44 -> 100 icin 56 yeni dugum dosyasi (`N53..N100`) + index | `Kayit dosyasi: 100` | Adim 1 | ~60 dk |

> **Adim 3 disindaki tum asamalarda kod yazilir -> Adim 2 onceden zorunludur (G12).**

### Blok K0 — Cozum + test iskeleti + scaffold 45 (ilk commit burada)

| # | Adim | Ne yapilir | Kanit | Bagimlilik |
|---|---|---|---|---|
| 4 | `.sln` uret | `dotnet new sln` + **45 cekirdek `.csproj`** `sln add` (`.slnx` zaten var) | `.sln` diskte + **senkron diff = 0** (VC-LAY-013) | Adim 2 |
| 5 | Test projeleri | `tests/VersaCoder.Tests.Unit` + `tests/VersaCoder.Tests.Architecture` | 2 `.Tests.csproj` diskte (bugun 0) | Adim 4 |
| 6 | NetArchTest bagla | `LayerRulesTests` **uretilmis 5000 kural** (dependency-map §6) + 8x8 olcum | `dotnet test` **0 ihlal** | Adim 5 |
| 7 | CPM dogrula | `Directory.Packages.props` iki cozumde de gecerli | `dotnet restore` 0 hata (VC-LAY-014) | Adim 4 |
| 8 | **Scaffold 45** | `tools\gen-5000-csproj.ps1 -Phase K0` (45 `.csproj` + `layer.json`) | `tools\count-dll.ps1` = **45 uretim + 2 test** | Adim 4 |
| 9 | **K matrix uret** | `tools\gen-matrix-5000.ps1` (5000 satir JSON) + 8x8 dogrulama | ilkk 8 satir §2 ile birebir | Adim 6 |
| 10 | **Ilk commit** | dosya bazli `git add` (tek dosya `add`; `git add -A` YASAK) | `git log` 1 satir | 4-9 hepsi yeşil |

### Blok A — 5 paket (Faz N1: +455 = 500 `.csproj` · K0850-K1299)

| # | Adim | Ne yapilir | Kanit | Bagimlilik |
|---|---|---|---|---|
| 11 | Tasinif | `Infrastructure` icindeki her sinifi **hangi pakete** gidecegine dair etiketle (29 paket hedefi) | sinif -> paket tablosu | Adim 10 |
| 12 | 5 `.csproj` ac | `Infrastructure.Data` · `.Config` · `.Logging` · `.Security` · `.FileSystem` | toplam **50** csproj + GATE-004 kaydi | Adim 11 |
| 13 | **N1 scaffold** | `tools\gen-5000-csproj.ps1 -Phase N1` (455 klasor, K0850-K1299) | `count-dll.ps1` = **500** uretim | Adim 12 |
| 14 | Yon grubu ekle | 8x8 -> **500x500** (`gen-matrix-5000.ps1` yeniden uret) | dependency-map §6 kural seti = 500 satir | Adim 13 |
| 15 | Sinif tasi | Adim 11'daki siniflari ilgili pakete tasi (namespace guncelle) | build 0 hata **x2** + NetArchTest **0 ihlal** + sayac PASS | Adim 14 |

### Blok B — 7 paket (Faz N2: +1000 = 1500 `.csproj` · K1300-K2299)

| # | Adim | Ne yapilir | Kanit | Bagimlilik |
|---|---|---|---|---|
| 16 | 7 `.csproj` ac | AI · Auth · Caching · Context · Messaging · Network · Observability | toplam **57** + GATE-004 kaydi | Blok A yeşil |
| 17 | **N2 scaffold** | `gen-5000-csproj.ps1 -Phase N2` (1000 klasor, K1300-K2299) | `count-dll.ps1` = **1500** | Adim 16 |
| 18 | Logging tasi | `Logging` `l11 Platform` -> `l14 Observability` | namespace + katalog guncel, 0 hata | Adim 17 |
| 19 | Sinif tasi + kanit | Adim 11'daki kalan siniflardan B grubu | 3 kanit (build x2 · NetArchTest · sayac) | Adim 18 |

### Blok C — 7 paket (Faz N3: +1500 = 3000 `.csproj` · K2300-K3799)

| # | Adim | Ne yapilir | Kanit | Bagimlilik |
|---|---|---|---|---|
| 20 | 7 `.csproj` ac | Git · MCP · Plugins · Services · Integration · Backup · Versioning | toplam **64** | Blok B yeşil |
| 21 | **N3 scaffold** | `gen-5000-csproj.ps1 -Phase N3` (1500 klasor, K2300-K3799) | `count-dll.ps1` = **3000** | Adim 20 |
| 22 | Esleme guncelle | `l11` +Git · `l16 Integration` (+MCP·Plugins·Services·Integration) · `l8` +Backup · `l19` +Versioning | §1.2 eslemesi = 29 icinde 19'u dagildi | Adim 21 |
| 23 | Sinif tasi + kanit | C grubu siniflari | 3 kanit | Adim 22 |

### Blok D — 10 paket (Faz N4: +2000 = 5000 `.csproj` · K3800-K5499)

| # | Adim | Ne yapilir | Kanit | Bagimlilik |
|---|---|---|---|---|
| 24 | 10 `.csproj` ac | CodeAnalysis · ProjectAnalysis · Refactoring · Testing · Templating · Documentation · Reporting · Diagram · Deployment · Learning | toplam **74** | Blok C yeşil |
| 25 | **N4 scaffold** | `gen-5000-csproj.ps1 -Phase N4` (2000 klasor, K3800-K5499) | `count-dll.ps1` = **5000** uretim (marj 0) | Adim 24 |
| 26 | **Matris 5000²** | `gen-matrix-5000.ps1` son uretim (5000 satir) | NetArchTest 0 ihlal (25.000.000 hucre uretilmis) | Adim 25 |
| 27 | Esleme kapa | 29 paketin tamami 7 banta dagildi (K0000-K0849) | §1.2 toplami = **29** | Adim 26 |
| 28 | Dugum 5000 | 44 -> 5000 dugum (`N53..`) — **katalog isidir, kod degil** | `Kayit dosyasi: 5000` | istege bagli, paralel |
| 29 | Sinif tasi + kanit | D grubu siniflari | 3 kanit | Adim 27 |

### Blok K1 — Semsiye kapanisi (G7 onayi AYRI ister)

| # | Adim | Ne yapilir | Kanit | Bagimlilik |
|---|---|---|---|---|
| 25 | Bosaltilma dogrula | `VersaCoder.Infrastructure` icinde sinif = **0** | dosya taramasi 0 | Blok D yeşil |
| 26 | **G7 ONAYI** | kullanici silmeye onay verir | `log.md` onay satiri | Adim 25 |
| 27 | `.csproj` sil | semsiye projeyi sil + cozum dosyalarindan cikar (`.sln` **ve** `.slnx`) | toplam **39** + senkron diff = 0 | Adim 26 |

---

## §2 Kabul kriterleri (kapanisa gore)

- [ ] **K0:** `.sln` + `.slnx` senkron (0 fark) · 45 cekirdek `.csproj` + 2 test · 5000 kural uretilmis · NetArchTest 0 ihlal · restore 0 hata · 1 commit
- [ ] **A/B/C/D:** her blokta **3 kanit** — `dotnet build` x2 = 0 hata · NetArchTest 0 ihlal · `count-dll.ps1` PASS (500/1500/3000/**5000**)
- [ ] **Kademe:** her blok sonunda §0 tablosundaki 3 eksen degeri ile olculmus olur (45/500/1500/3000/5000)
- [ ] **K bandi:** `k5000-layer-map.csv` ile `gen-5000-csproj.ps1` esleme = 1:1 (LAY-06)
- [ ] **Matris:** 8x8 olcum korunur (20 OK / 40 X) + 5000² uretilmis kural NetArchTest 0 ihlal
- [ ] **Iki sayac ayri** raporlandi: 5440 tip + 5834 parca (VC-LAY-012)
- [ ] **G7:** semsiye silme onayi `log.md`'de yazili

---

## §3 Acik sorular

| # | Soru | Durum | Sahip |
|---|---|---|---|
| 1 | Faz 02'nin 4 sorusu (plan birimi/yeri/kaynak/kapı) | **KAPANDI** 2026-10-02 (`.ai/plans/index.md`, P2-1..3 PASS) | - |
| 2 | Tier (T0..T8) tanimi iki kaynakta farkli | **DOĞRULAMA GEREKLI** (ikisi de kalir, kullanici karari) | architect |
| 3 | DX1001 lisans uyarisi test/CI'da tolerans mi, RED mi | test-strategy §7'ye not gerekli | tester |
| 4 | `.sln` uretim araci: `dotnet new sln` mi, `.slnx`'ten export mu | K0 Adim 4'te karar | architect |
| 5 | Dugum 5000 (Adim 23) hedef mi, yoksa SCALE-500'de durmak yeterli mi | kullaniciya 1 soru | kullanici |

**Degisiklik gunlugu:**
- 2026-10-02 — **v1.0.0** ilk yazim (4 adim, ADR-030 v1).
- 2026-10-02 — **v2.0.0** ADR-030 v2 ile birlikte: **6 asama blok (K0 · A · B · C · D · K1)** + 27 adim · her adimda **kapi + bagimlilik** · yon grubu acilim adimlari (11/14/21) · dugum ekseni (Adim 3/23) · `.slnx`+`.sln` senkron (Adim 4/27) · G7 onayi K1'de ayristirildi · acik soru 1 kapandi, 4 eklendi.
