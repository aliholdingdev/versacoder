---
title: "Spec - 5000+ Katman (Tek Hedef, 3 Eksen, Yon Grubu = Fiziksel .dll)"
type: spec
category: spec
status: active
version: 2.1.0
updated: 2026-10-02
authority: primary
scope: "5000+ katmanin (tip + dugum + uretim .dll) nasil uretildigi, adlandirildigi, hedefe ilerlemenin nasil olculdugu, yon grubunun (1 grup = 1 fiziksel .dll) nasil bolundugu ve kapilara nasil baglandigi — ADR-030 v2.1 spec karsiligi; gereksinim kimligi VC-LAY-NNN"
tags: [spec]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# Spec — 5000+ Katman (`spec/layering-5000-spec.md`)

**Zorunlu Baglantilar:** [[spec/index|index]] · [[claude|CLAUDE.md]] · [[governance/keys|keys]] · [[governance/workflow|WORKFLOW]] · [[spec/test-strategy|test-strategy]] · [[architecture/index|architecture/index]] · [[spec/build-spec|build-spec]] · [[.decisions/accepted/adr-030-5000-layers-class-level|ADR-030]] · [[.decisions/accepted/adr-014-slnx-and-cpm|ADR-014]] · [[.decisions/accepted/adr-028-layered-baseline-9-projects|ADR-028]] · [[.rules/layer-rules|layer-rules]]

> **Tek cumlelik sorumluluk:** *5000+ katman (tip **ve** dugum) hangi kuralla uretilir, kademe nasil olculur, yon grubu nasil buyur ve hangi gate'e baglanir* sorusunun tek cevabi.
> Gereksinim kimligi bicimi: **`VC-LAY-NNN`**. Her madde test edilebilir yazilir (G5).

---

## §0 Ozet (Answer First)

**Uc eksen, TEK hedef, tek komut seti.** Katman = tip kaydi (ADR-030 §2.1) **ve** dugum (namespace/klasor) **ve** uretim `.dll` (yon grubu) — ucsu da **ayri sutun**, hepsi **`>= 5000`** hedefine gider.

| # | Eksen | Bugun (2026-10-02) | Hedef | Durum |
|---|---|---|---|---|
| 1 | **Tip** (kayit satiri) | **5440** | `>= 5000` | **ASILDI** (marj 440) |
| 2 | **Dugum** (namespace/klasor) | **44 alt** (+8 makro = 52) | `>= 5000` | **EKSIK 4956** |
| 3 | **Yon grubu = uretim `.dll`** (matris satiri) | **9** (`l0..l8`) | `>= 5000` | **EKSIK 4991** |
| — | test projesi (yon grubu **degil**) | **2** | — | toplam `.csproj` = 11 |

**ARA KADEME YOKTUR** (100 / 500 / 1000 **silindi**, 2026-10-02 kullanici karari G7). **2 ekseni acik** (KADeME DRIFT); drift, TODOS/06 ile kapanmadan kaldirilamaz.

**Topoloji:** uretim `.csproj` = yon grubu (bugun **9**, hedef **5000+**); test **2** ayri sayilir. Eski `9 + N + 2` (11/16/23/40) **iptal**.
**Yon grubu:** `1 grup = 1 fiziksel .dll` (ADR-030 §2.3); bugun **9**, matris olculmus **8x8 = 64** (dokum -> **9x9**, DOĞRULAMA GEREKLI), hedef 5000x5000.
**Olculen iki sayac AYRI raporlanir:** 5440 tip + 5834 parca (fark 394) — birlestirilmez (G10).

---

## §1 Kapsam / Kapsam Disi

| # | Kapsam | Kapsam disi (nerede) |
|---|---|---|
| 1 | Tip + dugum kaydi uretimi, adlandirma, ID disiplini | Hangi tipin hangi isi yaptigi: ilgili katman spec'i |
| 2 | **Tek hedef** + uc ekseni olcumu (tip/dugum/yon grubu) | Sayacin kendisi/kodu: `scripts/count-components.ps1` |
| 3 | Yon grubu tanimi (1 grup = 1 uretim `.dll`) + bolme kurali | Paket ici baglilik detayi: `dependency-map.md` |
| 4 | Topoloji (uretim `.csproj` = yon grubu) + `.sln`/`.slnx` + CPM | Build sirasi + `#if`: [[spec/build-spec|build-spec]] |
| 5 | Yon matrisi + NetArchTest beklentisi | Test piramidi/kapsam: [[spec/test-strategy|test-strategy]] |

> **YASAK:** **kapi gecmeden** `.csproj` acmak (ADR-030 §2.4 + LAY-06 — 5000+ `.csproj` hedefi **karar**, ama her tekil acim GATE-004 + onay **ister**) · `yon-gruplari.md` satiri olmadan proje acmak · ad uydurmak (G9) · tek sayacla "5000+" demek (G5) · `l4 -> l2` dogrudan ok · katalog satirini elle uydurmak (G9) · dugum eksigini yogunluk bahanesiyle yok saymak · **ara kademe iddiasi** (G7 iptali) · **EF Core** (G2/ADR-005 — bu spec kapsam disi ama her yerde baglayici).

> **KAYIT (G5 — gizlenmez):** eski yasak "5000+ `.csproj` acmak" **kullanici karariyla kaldirildi** (ADR-030 §6 v2.1.0); ama olcum **hala karsit**: 575 csproj x 2 TFM restore **cukermisti** (ADR-028 §3 A3). Bu nedenle `dotnet restore` GATE-004'te **zorunlu kanittir** (ADR-030 §2.5/6).

---

## §2 Davranis

### §2.1 Yeni tip ekleme akisi (5 adim)

```text
tip lazim olur
    |
    +--> 1. hangi yon grubu?  architecture/layer-5000/yon-gruplari.md §2'den sec
    |                         (9 uretim .dll); yoksa §1 kurali ile bol + satiri ORAYA yaz
    +--> 2. hangi dugum (N01..N44, ya da yeni dugum)? -> architecture/catalog/N<NN>-*.md
    +--> 3. kayit satiri (8 sutun): ID | Tip | Tur | Sorumluluk | Proje | TFM | Test | Durum
    +--> 4. kodu yaz (o yon grubunun `.dll` klasorunde, cift TFM G6)
    +--> 5. olcu: count-components -> PASS (tip + dugum) + gen-layer-5000 -> PASS
                 + dotnet build x2 -> 0 hata + NetArchTest -> 0 ihlal
```

- **Adim 5'ten biri kirmizi ise tip `planli` kalir**; `planli -> yazildi` donusumu **kanitla** olur.
- **Proje acmak yasak:** yeni tip hicbir zaman `.csproj` olusturmaz (LAY-06).

### §2.2 Adlandirma ve ID kurallari

| # | Kural | Ornek | Ihlalde |
|---|---|---|---|
| 1 | ID `N<iki hane>-<uc hane>`, dugum bazli artan | `N01-001`, `N44-120` | sayac `Hatali format` -> RED |
| 2 | Yeni dugum `N45..` makro alanina **girmez**; makro `N45..N52` = mevcut yon grubu karsiligi, yeni dugumlar `N53..` ve yeni makro = **yeni yon grubu** (§2.4) | `N53-files-api.md` | makro/alt karisimi -> RED |
| 3 | Ayni tip adi iki kayitta **yok** | `TextPort` 1 kez | katalog-kod esme denetimi kirmizi |
| 4 | Her kayitta `TFM` sutunu `net48;net10.0-windows` | G6 cift bacak | tek bacak = G6 ihlali |
| 5 | `Proje` sutunu `yon-gruplari.md` §2'deki listeden biri (9 uretim `.dll` + 2 test) | `VersaCoder.Domain` · `VersaCoder.Tests` | listede olmayan proje -> LAY-05/06 RED |
| 6 | `Durum` yalniz `planli \| yazildi \| test-edildi` | serbest metin yasak | sayac format RED |

### §2.3 Hedefe ilerleme — uc eksen ayni anda olculur (ara kademe YOK)

| Eksen | Bugun (2026-10-02) | Hedef | Eksik | Kanit araci |
|---|---|---|---|---|
| **Tip (K)** | **5440** | `>= 5000` | **0 — ASILDI** | `count-components.ps1` -> `TOPLAM KAYIT` |
| **Dugum (N)** | **44 alt** | `>= 5000` | **4956** | `count-components.ps1` -> `Kayit dosyasi` |
| **Yon grubu (Y)** | **9 uretim `.dll`** | `>= 5000` | **4991** | `src/**/*.csproj` sayimi + `yon-gruplari.md` |
| — (test) | 2 | — | — | `tests/**/*.csproj` (ayri sayilir, matris satiri degil) |

- **"Hedef asildi"** yalnizca **uc eksen de `>= 5000`** iken yazilir; tek eksen eksikse **eksik sayi yazilir** ve `KADeME DRIFT` etiketi **dusmez** (G5).
- **Bugun:** K ✓ (5440) · N ✗ (44) · Y ✗ (9) -> **2 eksen eksik**.
- **ARA KADEME YOK** (100/500/1000 **silindi**, G7): arada olculen sayi "ust kademe" olarak **adlandirilmaz**.

### §2.4 Dugum acma akisi (yeni namespace/klasor)

```text
dugum lazim olur
  1. alt dugum mu, makro mu?   alt -> N53.. (§2.2/2) · makro -> yon-gruplari.md §1 kurali (YENI YON GRUBU)
  2. klasor adi: <alan>-<sorumluluk> (kebab-case)   ornek: data-access, security-auth
     YENI yon grubu ise ad eski ağaç klasorunden gelir (G9, yon-gruplari §1/2)
  3. index.md + kayit satirlari (8 sutun) + genel 5000'i yeniden calistir
  4. olcu: count-components "Kayit dosyasi" satiri -> hedef 5000'e karsi eksik
```

- **Yeni makro dugum acmak = yeni yon grubu = `yon-gruplari.md`'ye satir = LAY-04** -> `architect` + kullanici onayi + GATE-004.
- **Dugum artirirken yogunluk raporlanir** (44'te 124, 5000'de 1.1 kayit/dugum); yogunluk **esigi konmaz**.

### §2.5 Cozum dosyasi (`.slnx` + `.sln`) — ADR-014 + kullanici karari 2026-10-02

| # | Dosya | Rol | Kural |
|---|---|---|---|
| 1 | `VersaCoder.slnx` | **modern** (ADR-014, VS 2026 / dotnet CLI) | diskte **VAR** (937 B) |
| 2 | `VersaCoder.sln` | **eski uyumluluk** (eski Visual Studio / harici arac) | K0'da uretilir, **VAR degil** |
| 3 | `Directory.Packages.props` | CPM (ADR-014) | diskte **VAR** (`C:\versacoder\Directory.Packages.props`) |
| 4 | Senkron | iki cozum dosyasi **ayni proje listesini** tasir | her K/faz kapanisinda diff: 0 fark |

> **Gerekce (kullanici karari):** "hem `.slnx` hem `.sln`, cunku eski surum uyumlulugu". Iki dosya = **iki gercek riski** -> senkrol denetimi zorunlu (G10 azaltici).

---

## §3 Test / Kabul (gereksinimler — 14 madde)

| ID | Gereksinim (test edilebilir) | Kanit |
|---|---|---|
| **VC-LAY-001** | `count-components.ps1` -> `TOPLAM KAYIT >= 5000` ve `Hatali format 0` ise **PASS** | sayac ciktisi (GATE-004a) |
| **VC-LAY-002** | Her kayit **8 sutun** tasiir; bos sutun = RED | sayac format kontrolu |
| **VC-LAY-003** | Tip esigi asilirsa kademe K sutunu guncellenir; **tek sayac tek ekseni kanitlar** | sayac + §2.3 tablosu |
| **VC-LAY-004** | `l4 -> l2` ve `l5 -> l4` kodda **0 ihlal** | NetArchTest `LayerRulesTests` (GATE-005) |
| **VC-LAY-005** | Yeni tip iki bacakta **0 hata** derlenir | `dotnet build -f net48` + `-f net10.0-windows` (GATE-004) |
| **VC-LAY-006** | `yon-gruplari.md` satiri olmayan `.csproj` denemesi `DECISION BLOCKED` verir | 3 kapi kaydi (`log.md`) |
| **VC-LAY-007** | `durum` sutunu yalniz 3 degerden biri | dis deger taramasi -> 0 |
| **VC-LAY-008** | **Dugum ekseni:** `Kayit dosyasi` **5000'e ulasana kadar** "hedef asildi" **yazilmaz** (eksik sayi yazilir) | sayac `Kayit dosyasi: 44` satiri (2026-10-02) |
| **VC-LAY-009** | `planli -> yazildi` donusumu yalniz katalog-kod **diff = 0** ile olur | diff raporu (pilot dugum N01) |
| **VC-LAY-010** | **Yon grubu sayisi = uretim `.csproj` sayisi** (bugun **9**, hedef **5000**); matris boyutu sayimla eslesir | `yon-gruplari.md` + `.csproj` sayimi + matris dosyasi |
| **VC-LAY-011** | 29 paket bolme kuraliyla **olculerek** yerlesir; acilan grup sayisi `yon-gruplari.md`'ye yazilir (adet **kural belirler**, uydurulmaz) | `yon-gruplari.md` §2.3 + doluluk raporu |
| **VC-LAY-012** | **Iki sayac birlikte** raporlanir: 5440 tip + 5834 parca (fark 394 yazilir) | **iki** komut ciktisi ayni raporda |
| **VC-LAY-013** | `.slnx` ve `.sln` ayni proje listesini tasir (senkron diff = 0) | cozum dosyasi diff'i (K0'dan itibaren) |
| **VC-LAY-014** | CPM (`Directory.Packages.props`) her iki cozum dosyasi icin gecerli; paket surumu elle ezilmez | `dotnet restore` ciktisi + props dosyasi |

**Test turu dagilimi:**
- **sayac + script (vault, kod yok):** VC-LAY-001 · 002 · 003 · 007 · 008 · 010 · 011 · 012
- **NetArchTest:** VC-LAY-004
- **build:** VC-LAY-005 · 014
- **surec denetimi (log kaydi):** VC-LAY-006 · 009 · 013

Detay: [[spec/test-strategy|test-strategy]].

---

## §4 Gate Baglantisi

| Gate | Bu specten ne bekler | Kanit |
|---|---|---|
| **GATE-004a** | vault + katalog butunlugu: tip >= 5000, dugum eksigi **raporlanmis**, link 0, iki sayac **ayri** | 3 sayac ciktisi |
| **GATE-004** | iki bacak build 0 hata + **`dotnet restore` 0 hata** (K6 acigi) + `.slnx`/`.sln` senkron + K0 (2 test projesi) | 2 build + restore + diff |
| **GATE-005** | NetArchTest 0 ihlal + uc eksenin olculmus degeri + kapsam %90 | `dotnet test` + §2.3 tablosu |
| **GATE-006** | **hedef 5000+**: uretim `.csproj` >= 5000 + dugum >= 5000 + tip >= 5000 | 3 eksen kaniti (LAY-09) |

**Degisiklik gunlugu:**
- 2026-10-02 — **v1.0.0** ilk yazim (ADR-030 v1 ile birlikte, architect).
- 2026-10-02 — **v2.0.0** ADR-030 v2 ile birlikte: dugum ekseni (VC-LAY-008) · yon grubu acilimi + 29 esleme (VC-LAY-010/011) · iki sayac birlikte (VC-LAY-012) · `.slnx`+`.sln`+CPM (VC-LAY-013/014, §2.5) · 7 -> **14 gereksinim** · kademe gecisi 4 eksenli (§2.3).
- 2026-10-02 — **v2.1.0** ADR-030 v2.1 ile birlikte (kullanici G7): **tek hedef 5000+**, ara kademeler silindi · 4 -> **3 eksen** (P Y'ye merge) · §2.3 "kademe gecisi" -> "hedefe ilerleme" · VC-LAY-008/010/011 hedefe baglandi · §1 yasagi **tersine cevrildi** (5000+ `.csproj` karar, kapisi korunur) + K6 restore kaydi · GATE-004'e `dotnet restore` eklendi · `yon-gruplari.md` baglantisi eklendi.
