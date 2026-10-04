---
title: "ADR-030 - 5000+ Katman: Tek Hedef (katman = tip, yon grubu = fiziksel .dll)"
type: adr
category: decisions
status: accepted
version: 2.1.0
updated: 2026-10-04
authority: SSOT
scope: "VersaCoder mimarisinin hedefini TEK esikte kilitler: katman = tip kaydi >= 5000, dugum >= 5000, yon grubu = fiziksel .dll = .csproj >= 5000; 100/500/1000 ara kademeleri, 8·12·16·20 yon grubu merdiveni ve 9+N+2 topoloji formulu bu revizyonda iptal edildi (G7); olcum iki sayaci ayri ayri raporlanir"
---

# ADR-030 — 5000+ Katman: Tek Hedef

**Zorunlu Baglantilar:** [adr-index](../adr-index.md) · [ADR-028](ADR-028-layered-baseline-9-projects.md) · [ADR-013](ADR-013-layered-architecture-52-nodes.md) · [ADR-027](ADR-027-component-catalog-5000.md) · [yon-gruplari](../../architecture/layer-5000/yon-gruplari.md) · [CLAUDE.md](../../CLAUDE.md) · [keys.md](../../keys.md) · [test-strategy](../../spec/test-strategy.md)

> **Tek cumlelik sorumluluk:** *"5000+ katman" tek bir hedeftir — tip, dugum ve yon grubu (fiziksel .dll) ucun de `>= 5000`'e gider; ara kademeler yoktur* sorusunun tek cevabi.

---

## §0 Ozet (Answer First)

**Karar (kullanici, 2026-10-02, G7): hedef TEK sayi = 5000+.** Ara kademeler (SCALE-100/500/1000) dokumanlardan **cikarildi**; "8 katman" ifadesi bundan boyle **katman sayisi olarak kullanilmaz**. Uc eksen vardir ve hepsi ayni hedefe gider:

| # | Karar | Deger | Kaynak |
|---|---|---|---|
| 1 | Katman birimi | tip kaydi = 1 katman | bu ADR §2.1 |
| 2 | Katman hedefi | **tek esik `>= 5000`** (ara kademe YOK) | kullanici 2026-10-02 (G7) |
| 3 | **Yon grubu = Paket** | **1 yon grubu = 1 fiziksel `.dll` = 1 `.csproj`**; hedef `>= 5000` | bu ADR §2.3 + kullanici karari |
| 4 | Dugum hedefi | **`>= 5000`** (bugun 44, eksik 4956) | bu ADR §2.4 |
| 5 | Olcum | **iki sayac, ikisi de ayri raporlanir**: 5440 tip + 5834 parca | bu ADR §5 |
| 6 | Cift bacak | her tip `net48` + `net10.0-windows` (G6) | CLAUDE.md G6 |

**Iptal edilen iki eski karar (G7 ile):**

| Eskiden | Yeni hali |
|---|---|
| Kademe merdiveni `100 / 500 / 1000 / 5000` | **tek hedef `5000+`** (ara kademeler silindi) |
| Yon grubu `8 · 12 · 16 · 20` (matris 64/144/256/400) | **yon grubu = fiziksel .dll, hedef 5000+** |
| Topoloji `9 + N + 2` (N = 0·5·12·29 -> 11/16/23/40 csproj) | **`.csproj` sayisi = yon grubu sayisi = 5000+** |

**Kisa okuma:** katman = tip kaydi (bugun 5440, **ASILDI**) -> dugum = klasor (bugun 44, **EKSIK 4956**) -> yon grubu = .dll (bugun 11, **EKSIK 4989**) -> ikisi de kapanmadan "5000+ katman" **denemez** (G5).

---

## §1 Baglam (Context)

| # | Gercek | Kanit |
|---|---|---|
| C1 | v1.0.0 (2026-10-02) tek esik `>= 5000` ve "8 makro katman degismez" diyordu | bu dosya §6 degisiklik gunlugu |
| C2 | Kullanici karari (2026-10-02, G11): *"8 katman degil; 100 / 500 / 1000 / 5000 katman olacak; karari sen ver"* | bu oturum kullanici karari |
| C3 | Diskte **9 `.csproj`** uretim + **2 test** = **11**; `.sln` **0**, `.slnx` **1** | 2026-10-02: `src/**/*.csproj`=9, `tests/**/*.csproj`=2, `.slnx`=937 B |
| C4 | Katalog **5440 tip kaydi** (PASS) ve layer-5000 **5834 namespace parcasi** (PASS) | §5 K1 + K2 (2026-10-02) |
| C5 | 5000+ tip, tek semsiye `Infrastructure` icinde duruyor; eski agaclar 29 `Infrastructure.*` gosteriyor | `aaaaa-ek-1` §0 (3 snapshot union'i = 29 paket) |
| C6 | 575 csproj x 2 TFM restore'u **cukermisti** (ADR-028 §3 A3 UNIVERSAL RED) | ADR-028 §3 — **olcum hala gecerli, asagida K6** |
| C7 | **Yeni karar (2026-10-02, G7):** ara kademeler unutulur, tek hedef `5000+`; yon grubu **fiziksel .dll**'dir | bu oturum kullanici karari (2 soru ile onaylandi) |

**Celiski kaydi (G10):**

1. **"4 kademe" (C2) vs "tek hedef" (C7)** — ayni oturumda iki karar; **C7 kazanir** (son karar), C2'nin 100/500/1000 kademeleri **SUPERSEDED**. C2'nin tek esigi (`>= 5000`) zaten C7 ile aynidir.
2. **`9+N+2` formulu (v2.0.0) vs `yon grubu = fiziksel .dll` (C7)** — birlesir: paket (P) ekseni artik **yon grubu (Y) ekseni ile ayni seydir**; eski 4 eksenden P **Y'ye merge** edildi, sayi 4 -> **3** (K · N · Y).
3. **"8 makro katman" (ADR-028)** — bu revizyonla **celismez**: ADR-028 bugunun **olculmus** 8 grupluk baslangic matrisini (20 OK / 40 X) kilitler; bu ADR **hedefi** kilitler. Iki ADR birbirini supersede etmez (supersede zinciri bos).
4. **`ai.02` EF Core onerisi** — G2/ADR-005'e aykiridir; tekrar **RED** edilir, metni vault'a tasinmaz.

---

## §2 Karar

### §2.1 Katman ekeseni (K) — tek esik

**Katman = katalogda kayitli tip.** Katalog `architecture/catalog/N<NN>-*.md`, 8 sutun (ID | Tip | Tur | Sorumluluk | Proje | TFM | Test | Durum).

| Eksen | Hedef | Anlam | Durum (2026-10-02) |
|---|---|---|---|
| **K — tip kaydi** | `>= 5000` | kod birimleri (class/interface/record/enum) | **ASILDI** (5440, marj 440) |
| **N — dugum** | `>= 5000` | namespace / klasor (katalog dosyasi) | **EKSIK 4956** (44 alt dugum) |
| **Y — yon grubu = .dll** | `>= 5000` | fiziksel **uretim** `.csproj` = matris satiri | **EKSIK 4991** (bugun **9** uretim .dll; 2 test projesi matris satiri **degildir**, toplam `.csproj` = 11) |

- **Kural:** uc eksenin **hepsi** `>= 5000` olmadan "5000+ katman" iddiasi **yazilamaz** (G5).
- **Esik dusurme yetkisi yok** (LAY-08 korunur): tip marji 440'tir, esik asagi cekilmez.
- **Ara kademe yok:** v2.0.0'daki SCALE-100/500/1000 satirlari **kaldirildi**; arada olculen her sayi artik yalnizca "eksik kalmistir" olarak yazilir, "ust kademe" olarak **yazilmaz**.

### §2.2 Dugum ekseni (N)

Dugum = namespace/klasor (kullanici karari). Bugun **44 alt dugum** (`N01..N44`) + 8 makro (`N45..N52`) = 52 dosya. Hedef `>= 5000` -> **+4956 alt dugum**.

- Yeni dugum acmak `.csproj` **acmaz** (AYRI eksenlerdir; paket acimi icin §2.3 gecer).
- **YoGunluk raporlanir, esiklenmez:** 44 dugumde 5440/44 = **124 kayit/dugum**; 5000 dugumde beklenen 5440/5000 = **1.1**. Seyrelme **bilinçli kabul** edilmistir; yogunluk esigi **konmaz** (G5).

### §2.3 Yon grubu = fiziksel .dll (Y) — bu ADR'nin kalbi

**Tek satir karar: 1 yon grubu = 1 fiziksel `.dll` = 1 `.csproj` = baglilik matrisinin 1 satiri.**

| Soru | Cevap |
|---|---|
| Yon grubunu ne doldurur? | **`.dll`** (kullanici karari 2026-10-02, G7) |
| Bugun kac uretim yon grubu? | **9** (`l0..l8` = Abstractions · Domain · Application · Crosscutting · Infrastructure · Protocol · Host · Desktop · Cli) |
| Test projeleri? | **2** — matris satiri **degildir** (yalniz bagimli uc); toplam `.csproj` = 11 |
| Hedef? | **`>= 5000` uretim .dll** |
| Eksik? | **4991 `.csproj`** |
| P ekseni? | **Y'ye merge** (ikisi de `.csproj` sayar) |

**Turetme kurali (uydurulmaz, G9):** her yeni yon grubu, **olcusulur bir ihtiyactan** turer —
1. bir `.dll` icinde **6'dan fazla** sorumluluk (modul) birikirse bolunur (LAY-04),
2. bolme adi eski agraclarin gercek klasor adlarindan alinir (`versacoder.old.*`, olculmus 36 proje/ağaç),
3. bolunen her parcaya ayri `.csproj` + matris satiri verilir,
4. ad/uolverim adimi `yon-gruplari.md`'ye yazilmadan `.csproj` **acilmaz** (LAY-06).

**Matris boyutu ve kural uretimi (RISK, açik):** 5000 yon grubu -> matris **5000 x 5000 = 25.000.000 hucre**. Elle kural **yazilmaz**; kural seti **uretilir** ve en az su iki bicimde denetlenir:
- **uretim kurali:** `l_i -> l_j` yalnizca izinli sirada (insa sirasi `l5000.. -> ... -> l0`), tek satirlik genel kural,
- **olculmus matris:** bugunun 8x8'i (20 OK / 40 X) **olcum olarak kalir** ve yeni gruplarin tamami bu deseni **bozmamalidir** (bozarsa NetArchTest RED, GATE-005).

> **DOĞRULAMA GEREKLI (açık madde):** 25.000.000 hucrelik matrisin uretim yolu ve NetArchTest kural dosyasi boyutu **henuz olculmedi**; bu ADR kural uretimini **emreder**, calisan ornegi **iddia etmez** (G9).

### §2.4 Topoloji (`.csproj` sayisi)

| Tarih | Formul | Sonuc |
|---|---|---|
| v1.0.0 | 9 uretim + 2 test sabit | 11 toplam (9 uretim) |
| v2.0.0 (SUPERSEDED) | `9 + N + 2`, N = 0·5·12·29 | 11 · 16 · 23 · 40 |
| **v2.1.0 (BU)** | **uretim `.csproj` = yon grubu = 5000+** | **bugun 9 uretim (+2 test = 11) -> hedef 5000+** |

- **Olculur:** `Get-ChildItem src -Recurse -Include *.csproj` -> uretim toplami (bugun **9**); `tests` **ayri** sayilir (bugun **2**).
- **Kapi:** her yeni `.csproj` = `architect` karari + kullanici onayi + GATE-004 (ADR-028 3 kapi; LAY-06). Bu ADR kapiyi **onceden onaylamaz**.

### §2.5 Olcum ve kapi

| # | Arac | Ne olcer | Beklenen | Kapi |
|---|---|---|---|---|
| 1 | `scripts/count-components.ps1` | tip kaydi + dugum dosyasi + hatali format | tip `>= 5000` **VE** dugum eksigi **yazilmis** | GATE-004a |
| 2 | `scripts/gen-layer-5000.ps1` | 9 Tier namespace parcasi | **PASS** (ayri rapor) | GATE-004a |
| 3 | dosya sayimi | `.csproj` toplami = yon grubu sayisi | eslesme | GATE-004 |
| 4 | NetArchTest `LayerRulesTests` | yon matrisi | **0 ihlal** | GATE-005 |
| 5 | `dotnet build` iki TFM | cift bacak (G6) | **0 hata** | GATE-004 |
| 6 | `dotnet restore` | **5000 csproj restore sagligi** | **0 hata** (K6 acigi burada kapanir) | GATE-004 |

**Iki sayac farki KAPANMAZ, yazilir:** `count-components` **tip kaydi** sayar (5440), `gen-layer-5000` **namespace parcasi** sayar (5834). Fark = 394; ikisi de **bagimsiz PASS**'tir; birlestirip tek "5000+" kaniti cikarmak **yanlistir** (G10). Birincil kanit **tip kaydi (5440)**dir.

---

## §3 Alternatifler

| # | Alternatif | Arti | Eksi | Sonuc |
|---|---|---|---|---|
| A1 | v1.0.0: tek esik 5000 + "8 makro katman degismez" | basit | "8 mi 5000 mi" belirsizligi (C1) | **SUPERSEDED** |
| A2 | v2.0.0: 4 kademe (100/500/1000/5000) + `9+N+2` | olcum araligi | ara kademeler isleri kilitler; 4 satir bakim | **SUPERSEDED (bu revizyon)** |
| A3 | **Tek hedef 5000+ + yon grubu = .dll (BU ADR)** | tek hedef, tek sayi, okuyucu karisikligi yok | 5000 `.csproj` build/restore riski (K6) | **KABUL** (kullanici G7) |
| A4 | Yon grubu = mantiksal klasor (dll sayisi 40'ta kalinir) | build ucuz | kullanici "yon grubu = .dll" kararina aykiri | **RED** (kullanici C7) |
| A5 | 5000 makro katman ama elle matris | tam gorunur | 25.000.000 hucre elle **yonetilemez** | **RED** — yalnizca **uretilmis** kural kabul (§2.3) |

**Olculmus karsit kanit (G5 — gizlenmez):**

| # | Olcum | Cikti | Ne anlama gelir |
|---|---|---|---|
| K6 | 575 csproj x 2 TFM `dotnet restore` | **cukermisti** (ADR-028 §3 A3, UNIVERSAL RED) | 5000 csproj bu noktanin **~8.7 kati**; risk **artmis** durumda |

**En zayif nokta (red-team):** 5440 kayit koddan **once** yazildi (ADR-027 `planli` durumu); sayac PASS derken kod eksik olabilir. Cevap: `planli -> yazildi` donusumu katalog-kod diff'i ile kanitlanir (spec VC-LAY-009).
**Ikinci zayiflik:** K6 — 5000 `.csproj`'de restore/build olcum yapilmadan hedef **kanitlanamaz**; bu yuzden §2.5/6'ya `dotnet restore` maddesi **zorunlu** kondu ve GATE-004 icinde **olculmus** olarak calistirilir. Olcum cikmazsa hedef "acik" kalir (G5).

---

## §4 Sonuc

| # | Sonuc | Kanit |
|---|---|---|
| R1 | Katman hedefi **tek esik `>= 5000`**; ara kademe yok | bu ADR §2.1 |
| R2 | Yon grubu = **fiziksel .dll** = `.csproj` = matris satiri; P ekseni Y'ye merge (3 eksen) | bu ADR §2.3 + kullanici C7 |
| R3 | Bugun: K = **ASILDI** (5440) · N = **EKSIK 4956** (44 dugum) · Y = **EKSIK 4991** (9 uretim .dll) -> **2 ekseni acik** | §5 K1 + K3 |
| R4 | ADR-028 ve ADR-013 **supersede edilmez**; supersede zinciri bostur | adr-index §3.4 |
| R5 | 5000 `.csproj` restore riski **kabul edilmis ama olculmemistir** -> GATE-004'te `dotnet restore` zorunlu | §3 K6 + §2.5/6 |

**Takip:** `.rules/layer-rules.md` (LAY-01..12) · `spec/layering-5000-spec.md` (VC-LAY-001..014) · `architecture/layer-5000/yon-gruplari.md` (t uretme kurali) · `TODOS/06` · `checklists/layer-5000-check.md` (14 madde). Sahip: `architect`. Review: `reviewer`.

---

## §5 Kanit (olcum — G9)

| # | Kanit | Cikti | Tarih |
|---|---|---|---|
| K1 | `powershell -File .ai\scripts\count-components.ps1` | `TOPLAM KAYIT 5440 (esik >= 5000), Hatali format 0, SONUC PASS` | **2026-10-02** |
| K2 | `powershell -File .ai\scripts\gen-layer-5000.ps1` | `TOPLAM 5834 (esik >= 5000), Hatali format 0, SONUC PASS` | **2026-10-02** |
| K3 | `Get-ChildItem src -Recurse -Filter *.csproj` + `tests` | **9** uretim + **2** test = **11** `.csproj` | 2026-10-02 |
| K4 | cozum dosyalari | `.slnx` **VAR** (937 B) · `.sln` **YOK** · `Directory.Packages.props` VAR | 2026-10-02 |
| K5 | `gen-layer-5000.ps1` Tier dagilimi | T0=566 T1=1120 T2=982 T3=1110 T4=381 T5=326 T6=284 T7=661 T8=404 | 2026-10-02 |
| K6 | **kaynak:** ADR-028 §3 A3 | 575 csproj x 2 TFM `dotnet restore` = **cukermisti** (UNIVERSAL RED) | 2026-10-02 (daha onceki oturum, bu dosyaya tasinan olcum) |

> **Duzeltme kaydi:** v2.0.0'daki K3 (`9`) ve K4 (`.sln`=0, test=0) satirlari **yanlisti** — root'ta `Get-ChildItem -Filter *.Tests.csproj` **recursive olmadigi** icin 2 test projesi gorunmemisti. Doğrusu K3 = **11** (9+2) ve K4'te test projesi **VAR**. Test eksiği **yok**; drift yalniz **dugum + yon grubu** eksenlerinde.

---

## §6 Degisiklik gunlugu

| Surum | Tarih | Ne degisti | Kaynak |
|---|---|---|---|
| 1.0.0 | 2026-10-02 | ilk yazim: tek esik 5000, "8 makro katman degismez", 9+2 proje sabit | kullanici C1 |
| 2.0.0 | 2026-10-02 | 4 kademe (100/500/1000/5000) · sozluk ayrildi · topoloji 9+N+2 · iki sayac ayri · KADeME DRIFT | kullanici C2 (G11) |
| **2.1.0** | **2026-10-02** | **tek hedef 5000+** (ara kademeler silindi) · **yon grubu = fiziksel .dll** · P ekseni Y'ye merge (4 -> 3 eksen) · `9+N+2` ve `8·12·16·20` **iptal** · K6 restore riski + `dotnet restore` kapi maddesi · K3/K4 olcum duzeltmesi (11 csproj, 2 test VAR) | kullanici **C7** (G7 onayli is emri) |

> **Not (G7):** eski surumlerin metni **silinmedi**; degisen bolumler §6 tablosunda ve git gecmisinde. Dosya adi/slug'i `5000-layers-class-level` olarak kalir (adr-index §3.5/3: slug degisimi defter kopuklugu dogurur).
