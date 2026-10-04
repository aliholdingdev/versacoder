---
title: "VersaCoder — Katman Kodlama Raporu (PART 6 / GATE-005)"
type: report
category: reports
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "PART 6 L0..L7 kodlama/test durumu - kaynak vault kararlari aktarilir, bu vault'taki satirlar DOĞRULAMA GEREKLI / bekliyor olarak kalir"
---

# VersaCoder — Katman Kodlama Raporu (PART 6)

**Zorunlu Baglantilar:** [index](index.md) · [00-kapsam-analizi](00-kapsam-analizi.md) · [04-proje-iskeleti-raporu](04-proje-iskeleti-raporu.md) · [07-dogrulama-raporu](07-dogrulama-raporu.md) · [WORKFLOW](../WORKFLOW.md) · [spec/index](../spec/index.md)

> **Tek cumlelik sorumluluk:** *L0..L7 katmanlarinin kodlama/test ilerlemesi nedir, hangi kanit GATE-005'i acar* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

**Kodlama HENUZ UYGULANMADI: `C:\versacoder` icinde `src/` ve `tests/` klasorleri yok (2026-09-30 olcumu) → sekiz katmanin sekizi de 0 dosya, 0 test.** Kaynak vault'ta kararlastirilan sey asagiya aktarildi: kodlama sirasi `Abstractions > Domain > Application > CrossCutting > Infrastructure > Protocol > Host > UI`, bagimlilik yonu tek yonlu L7 > L0 (ADR-013/ADR-028), test piramidi `spec/test-strategy.md`'de (birim > SQLite entegrasyon > NetArchTest mimari). Bu vault'taki durum **`DOĞRULAMA GEREKLI` / bekliyor**: `[DOTNET_TEST_CIKTISI]` kutulari bos, **build/test gecti yazilmadi** (G5). **GATE-005 kapali.**

---

## §1 Katman Ilerleme Tablosu (durum: bekliyor)

| # | Katman | Proje | Kapsam | Kod | Test | Durum |
|---|---|---|---|---|---|---|
| L0 | Abstractions | `VersaCoder.Abstractions` | arayuzler, contract'lar | 0 dosya | 0 | **DOĞRULAMA GEREKLI** |
| L1 | Domain | `VersaCoder.Domain` | entity, value object, domain kurali | 0 | 0 | **DOĞRULAMA GEREKLI** |
| L2 | Application | `VersaCoder.Application` | use-case, orkestrasyon | 0 | 0 | **DOĞRULAMA GEREKLI** |
| L3 | CrossCutting | `VersaCoder.CrossCutting` | logging, cache, result, DI | 0 | 0 | **DOĞRULAMA GEREKLI** |
| L4 | Infrastructure | `VersaCoder.Infrastructure` | ADO.NET + Dapper, SqlClient 5.2.x, migration (saf SQL) | 0 | 0 | **DOĞRULAMA GEREKLI** |
| L5 | Protocol | `VersaCoder.Protocol` | tool/agent/LLM sozlesmesi | 0 | 0 | **DOĞRULAMA GEREKLI** |
| L6 | Host | `VersaCoder.Host` | DI kabugu, oturum, zamanlayici | 0 | 0 | **DOĞRULAMA GEREKLI** |
| L7 | UI | `VersaCoder.Desktop` (+`.Cli`) | RibbonForm, formlar, olay yonlendirici | 0 | 0 | **DOĞRULAMA GEREKLI** |

**Icinde okuma:** "0 dosya" bir basari/ basarisizlik sayisi degil, **envanter olcumudur** (2026-09-30); proje kurulmadigi icin satir kapanmaz, `bekliyor` olarak kalir.

---

## §2 Test Sonuclari (kanit kutulari bos)

| # | Koleksiyon | Komut | Beklenen | Cikti |
|---|---|---|---|---|
| 1 | Birim (`net48`) | `dotnet test -f net48` | 0 basarisiz | `[DOTNET_TEST_CIKTISI]` |
| 2 | Birim (`net10.0-windows`) | `dotnet test -f net10.0-windows` | 0 basarisiz | `[DOTNET_TEST_CIKTISI]` |
| 3 | Mimari (NetArchTest) | `dotnet test --filter "Category=Architecture"` | katman ihlali 0 | `[DOTNET_TEST_CIKTISI]` |
| 4 | Coverage | coverage raporu | hedef + kapsam disi gerekcesi yazili | `[KANIT_BEKLIYOR]` |

> Bu tabloda **tek bir gecis/ kalmis sayisi da uydurulmadi**; kutular `developer`/`tester` ciktisiyla doldurulur, `reviewer` onaylar.

---

## §3 TDD Notlari (kaynak vault karari)

| # | Kural | Uygulama |
|---|---|---|
| 1 | Sira **RED > GREEN > REFACTOR**; refactor = davranis korunur | `tests/**` sahibi `tester` |
| 2 | Birim test I/O'suz, deterministik (seed'li, zaman bagimsiz) | saat/rastgelelik `TimeProvider` ile |
| 3 | Entegrasyon test **gercek sema** uzerinde SQLite (`Microsoft.Data.Sqlite`) | **DOĞRULAMA GEREKLI** → NuGet teyidi |
| 4 | Test uretim kodunu **duzeltmez**; kok neden `developer`'a doner | AGENTS §1 (kod = `developer`, test = `tester`) |
| 5 | EF Core ile test verisi **yasak**; migration = saf SQL + version tablosu | ADR-005 · ADR-017 |

---

## §4 Kapi Kontrolu (GATE-005)

| # | Kriter | Bu vault'taki gercek | Durum |
|---|---|---|---|
| 1 | `dotnet build` iki bacakta 0 hata (GATE-004) | build ciktisi yok → [04-proje-iskeleti-raporu](04-proje-iskeleti-raporu.md) §4 | **ACIK** |
| 2 | `dotnet test` iki bacakta yesil | `[DOTNET_TEST_CIKTISI]` bos | **ACIK** |
| 3 | NetArchTest 0 ihlal | test projesi yok | **ACIK** |
| 4 | EF Core 0 · string-concat SQL 0 · DevExpress yalniz v26.1 | `src/` yok → tarama hedefi yok | **DOĞRULAMA GEREKLI** |
| 5 | `reviewer` PASS (kanit zorunlu) | inceleme yapilmadi | **ACIK** |

---

## §5 Sira ve Bagimlilik

| # | Adim | Onceki halka | Neden sirada |
|---|---|---|---|
| 1 | GATE-003 kullanici onayi | [03-ui-design-raporu](03-ui-design-raporu.md) §4 | onaysiz `.cs` yazilmaz (G12) |
| 2 | Iskelet + CPM (PART 5) | [04-proje-iskeleti-raporu](04-proje-iskeleti-raporu.md) §3 | test projesi olmayan katman olcumlenemez |
| 3 | L0 > L3 kod + birim test | `spec/test-strategy.md` | bagimlilik yonu tek yonlu (L7 > L0) |
| 4 | L4 (Dapper + migration saf SQL) | ADR-005 / ADR-017 | EF Core yasak, parametreli sorgu zorunlu |
| 5 | L5 > L7 + NetArchTest + iki bacak test | [ADR-028](../.decisions/accepted/ADR-028-layered-baseline-9-projects.md) · [architecture/index](../architecture/index.md) | mimari test en sonda calisir |

---

## §6 Risk

| # | Risk | Etki | Kapanis kosulu |
|---|---|---|---|
| R1 | Onay/iskeletsiz kodlama | iki kez is (gri alanda yazilan kod silinir) | §5 sirasi bozulmaz |
| R2 | Kanitsiz "test gecti" iddiasi | G5 ihlali, gate sahte yeşil | her satir `[DOTNET_TEST_CIKTISI]` ile kapanir |

---

## §7 Sure

| Is | Sahip | Tahmin |
|---|---|---|
| L0..L3 kod + birim test | developer + tester | ≈2 sa |
| L4 (Dapper + migration) + entegrasyon test | developer + tester | ≈1 sa 30 dk |
| L5..L6 + L7 form kodlamasi (GATE-003 sonrasi) | developer | ≈1 sa 30 dk |
| NetArchTest + iki bacak test + PASS | tester + reviewer | ≈1 sa |
| **Toplam PART 6** | — | **≈6 sa** (WORKFLOW §2.6) |

---

## §8 Kapsam Disi

| Bu rapor NEYI KAPLAMAZ | Nerede yasar |
|---|---|
| Proje/agac ve paket kurulumu | [04-proje-iskeleti-raporu](04-proje-iskeleti-raporu.md) |
| Build + vault olcum ciktilari | [07-dogrulama-raporu](07-dogrulama-raporu.md) |
| Wireframe ve UI onayi | [03-ui-design-raporu](03-ui-design-raporu.md) |
| Test stratejisi tanimi | `spec/test-strategy.md` ([spec/index](../spec/index.md)) |
| Katman/proje karari | [ADR-028](../.decisions/accepted/ADR-028-layered-baseline-9-projects.md) |

---

## §9 Kanit

| # | Kanit | Kaynak | Olcum |
|---|---|---|---|
| 1 | `src/` ve `tests/` klasorleri yok | dizin listesi, 2026-09-30 | 0 klasor |
| 2 | `C:\versacoder` koku yalniz `.ai/` + `.git/` | dizin listesi, 2026-09-30 | 2 giris |
| 3 | Test ciktisi kutulari bos | bu dosya §2 | 0/4 doldurulmus |
| 4 | GATE-005 kapali | §4 kapi tablosu | 5/5 kriter acik veya DOĞRULAMA GEREKLI |
| 5 | Olcum araci mevcut | `.ai/scripts/` icinde `check-links.ps1`, `find-mojibake.ps1` | 5 `.ps1` |

Kaynak dosya adlari: `05-katman-kodlama-raporu.md` (bu dosya), `04-proje-iskeleti-raporu.md`, `07-dogrulama-raporu.md`, `spec/test-strategy.md`.
