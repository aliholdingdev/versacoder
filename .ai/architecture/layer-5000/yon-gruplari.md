---
title: "Yon Gruplari - 1 Grup = 1 Fiziksel .dll (Hedef 5000+)"
type: architecture
category: architecture
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "Yon grubunun ne oldugunu (fiziksel uretim .dll = .csproj = matris satiri), nasil turetildigini (6'dan fazla modul -> bolme + eski ağaç klasor adlari), bugun 9 uretim .dll ile hedef 5000+ arasindaki 4991'lik boslugu ve 25.000.000 hucrelik matrisin kural uretimini tanimlar"
---

# Yon Gruplari — 1 Grup = 1 Fiziksel `.dll`

**Zorunlu Baglantilar:** [layer-5000 index](index.md) · [ADR-030](../../.decisions/accepted/ADR-030-5000-layers-class-level.md) · [layer-rules](../../.rules/layer-rules.md) · [layering-5000-spec](../../spec/layering-5000-spec.md) · [dependency-map](../00-overview/dependency-map.md) · [catalog](../catalog/index.md) · [layer-5000-check](../../checklists/layer-5000-check.md)

> **Tek cumlelik sorumluluk:** *bir yon grubu tam olarak ne kadar `.dll` kapsar, yeni grup nereden turer ve bugun 9'dan 5000+`a nasil cikilir* sorusunun tek cevabi.

---

## §0 Ozet (Answer First)

**1 yon grubu = 1 fiziksel uretim `.dll` = 1 `.csproj` = baglilik matrisinin 1 satiri.** (kullanici karari 2026-10-02, G7 — ADR-030 v2.1.0 §2.3)

| Alan | Deger | Kaynak |
|---|---|---|
| Bugun uretim yon grubu | **9** | 2026-10-02 `src/**/*.csproj` sayimi |
| Bugun test projesi | **2** (matris satiri degil) | 2026-10-02 `tests/**/*.csproj` sayimi |
| Toplam `.csproj` | **11** | ayni sayim |
| Hedef | **>= 5000 uretim .dll** | ADR-030 v2.1.0 §2.1 |
| **Eksik** | **4991 uretim .dll** | 5000 - 9 |
| Ara kademe | **YOK** | ADR-030 §0 (100/500/1000 iptal) |

---

## §1 Yeni yon grubu nasil turetilir (4 adim, uydurulmaz)

| # | Adim | Kural | Kanit |
|---|---|---|---|
| 1 | **Bolme esigi** | Bir `.dll` icinde **6'dan fazla** sorumluluk (modul) birikirse **bolunmek zorundadir** | modul sayimi (katalog/node uzerinden) |
| 2 | **Ad nereden gelir** | Bolunen parcanin adi **eski ağaçlarin gercek klasor adlarindan** alinir: `C:\www\versacoder\versacoder.old.1` · `versacoder.old.2` · `versacoder.old.old old` (her ağaç **olculmus**: 36 klasor / 36 `.csproj`) | klasor listesi ciktisi |
| 3 | **Kayit** | Yeni grup bu dosyaya `l<N>` satiri olarak yazilir **VE** `.csproj` ancak ondan sonra acilir | bu dosya §2 + `log.md` GATE-004 kaydi |
| 4 | **Matris** | `dependency-map.md`'ye 1 satir + 1 sutun eklenir; NetArchTest kural seti **yeniden uretilir** | matris boyutu sayimi |

> **YASAK:** ad uydurmak (G9) · bu dosyaya satir yazmadan `.csproj` acmak (LAY-06) · bolme esigini yogunluk bahanesiyle dusurmek (G5).

---

## §2 Bugunun olculmus durumu (2026-10-02)

### §2.1 Uretim yon gruplari (9)

| Yon grubu | `.csproj` | Durum |
|---|---|---|
| `l0` Foundation | `src/VersaCoder.Abstractions` | diskte VAR |
| `l1` Domain | `src/VersaCoder.Domain` | diskte VAR |
| `l2` Application | `src/VersaCoder.Application` | diskte VAR |
| `l3` CrossCutting | `src/VersaCoder.Crosscutting` | diskte VAR |
| `l4` Infrastructure | `src/VersaCoder.Infrastructure` (semsiye) | diskte VAR — 29 alt paket **planli** |
| `l5` Protocol | `src/VersaCoder.Protocol` | diskte VAR |
| `l6` Host | `src/VersaCoder.Host` | diskte VAR |
| `l7` UI-Desktop | `src/VersaCoder.Desktop` | diskte VAR |
| `l8` UI-Cli | `src/VersaCoder.Cli` | diskte VAR |

**Not (olcum, G9):** eski 8x8 `l7 UI` satiri **2 `.dll` tasiyordu** (Desktop + Cli). "1 grup = 1 `.dll`" kuraliyla `l7` **ikiye ayrildi** -> bugun **9 grup**, matris **9x9 = 81 hucre** olur. `dependency-map.md` hala **8x8** yaziyor -> **DOĞRULAMA GEREKLI**: matris 9'a cevrilmeden NetArchTest kurali gecerli degildir.

### §2.2 Test projeleri (2 — matris disi)

| `.csproj` | Rol |
|---|---|
| `tests/VersaCoder.Tests` | birim test |
| `tests/VersaCoder.ArchitectureTests` | mimari / NetArchTest |

Test projeleri yon grubu **sayilmaz**; yalniz bagimli uctur ve her yon grubunu **referanslayabilir** (kural disi degil).

### §2.3 29 planli `Infrastructure.*` paketi nereye gider?

29 paket **29 yeni yon grubu** **degildir**; §1/1 kuralina gore **bolum bolum** acilir:

| Asama | Paket | Yeni yon grubu |
|---|---|---|
| Faz A | Data · Config · Logging · Security · FileSystem | 5 (bolme esigine takilanlar) |
| Faz B | AI · Auth · Caching · Context · Messaging · Network · Observability | 7 |
| Faz C | Git · MCP · Plugins · Services · Integration · Backup · Versioning | 7 |
| Faz D | CodeAnalysis · ProjectAnalysis · Refactoring · Testing · Templating · Documentation · Reporting · Diagram · Deployment · Learning | 10 |
| **Toplam** | **29 paket** | **acilim sirasi = §1 kurali; adet kurali belirler** |

> **Açik nokta:** 29 paketin kac ayri `.dll` olacagi **henuz olculmedi** (kural > 6 modul -> bol; 29 paket tek basina 29'dan az ya da fazla grup uretebilir). Bu sayi **yazilmaz**, Faz A-D sirasinda **olcülür** (G5/G9).

---

## §3 5000+ hedefine yol

| # | Madde | Bugun | Hedef | Eksik |
|---|---|---|---|---|
| 1 | Uretim yon grubu (`.dll`) | **9** | **>= 5000** | **4991** |
| 2 | Dugum (namespace/klasor) | **44 alt** | **>= 5000** | **4956** |
| 3 | Tip kaydi | **5440** | **>= 5000** | **0 (ASILDI)** |
| 4 | Matris | **8x8 = 64** (olculmus) | 5000x5000 = 25.000.000 | uretim yolu **olculmedi** |

**Siralama kurali:** `.dll` acmadan **once** dugum (klasor + katalog kaydi) acilir; tip kaydi `planli` kalir. Uct ucuna `planli -> yazildi` donusumu **katalog-kod diff = 0** ile kanitlanir (VC-LAY-009).

---

## §4 Matris ve kural uretimi (RISK — acik)

| # | Soru | Durum |
|---|---|---|
| 1 | 5000x5000 = **25.000.000 hucre** nasil denetlenir | **uretim kurali** gerekir (elle yazilmaz) — ornek calisma **YOK** |
| 2 | NetArchTest kural dosyasi boyutu / sure | **olculmedi** |
| 3 | Bugunun olculmus deseni (20 OK / 40 X) yeni gruplarda korunuyor mu | denetlenecek — **yeni grup deseni bozarsa RED** |
| 4 | `dependency-map.md` 8x8 mi 9x9 mu olacak | **DOĞRULAMA GEREKLI** (§2.1 notu) |

> Bu dosya kural **emerir**, calisan ornegi **iddia etmez** (G9). Olcum alinmadan "matris uretildi" denemez (G5).

---

## §5 Dogrulama

- [ ] Yeni `.csproj` acilmadan **once** bu dosyaya `l<N>` satiri eklendi mi (§1/3)
- [ ] Bolme adi eski ağaç klasor listesinden mi geldi (§1/2, G9)
- [ ] Test projeleri yon grubu sayisina **karistirilmadi** mi (§2.2)
- [ ] `.csproj` toplami = uretim yon grubu + 2 test mi
- [ ] `count-components.ps1` dugum sayisi >= 5000 mu yoksa eksik **yazildi** mi (LAY-09)
- [ ] `dependency-map.md` matris boyutu ile NetArchTest kural seti **eslesiyor** mu
- [ ] `dotnet restore` **0 hata** (ADR-030 §2.5/6 — K6 acigi)

**Degisiklik gunlugu:**
- 2026-10-02 — **v1.0.0** ilk yazim (ADR-030 v2.1.0 ile birlikte): 1 grup = 1 uretim `.dll` · 4 adimli bolme kurali · 9 grupluk olculmus durum · 4991 eksik · matris uretim riski.
