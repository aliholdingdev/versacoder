---
title: "layer-5000-check - 5000+ Katman Kurulum Kontrol Listesi (4 Kademe, 3 Eksen)"
type: checklist
category: checklists
status: active
version: 2.0.0
updated: 2026-10-04
authority: SSOT
scope: "ADR-030 v2 (4 kademe, tip/dugum/paket/yon 4 eksen) uygulamasinin kabul listesi: sayim, dugum, kademe gecisi, yon grubu 8·12·16·20, iki sayac, cozum dosyasi senkronu ve gate baglantisi — 14 madde, her madde kanitsiz kapanmaz"
---

# layer-5000-check — 5000+ Katman Kurulum Kontrolu

**Zorunlu Baglantilar:** [index](index.md) · [WORKFLOW](../WORKFLOW.md) · [layering-5000-spec](../spec/layering-5000-spec.md) · [layer-rules](../.rules/layer-rules.md) · [ADR-030](../.decisions/accepted/ADR-030-5000-layers-class-level.md) · [coding-standards](../.rules/coding-standards.md) · [test-strategy](../spec/test-strategy.md)

> **Tek cumlelik sorumluluk:** *5000+ katman kurulumu kurala uygun mu — 4 eksen, adlandirma, iki sayac, yon grubu, cozum dosyasi ve gate kanitlari dolu mu* sorusunun tek cevabi.

---

## §0 Ozet (Answer First)

`architect` doldurur, kapanisi **kullanici** yapar (ajan gate kapatamaz, G12). Kanit olarak yalniz **calistirilmis komut ciktisi** kabul edilir (G5): sayac `PASS` (tip **ve** dugum), build `0 hata` x2, NetArchTest `0 ihlal`, cozum senkron `0 fark`. Tek bir "oldu" bile kanitsiz ise liste gecersizdir.

| Alan | Deger |
|---|---|
| Kim doldurur | `architect` (kanit toplayan) |
| Kim kapatir | **kullanici** (G12) |
| Zorunlu kanit | **iki sayac** + 2 build + `dotnet test` + cozum diff + `git status` |
| Soru sayisi | **14** — hepsi `[ ]` + `Kanit:` satiri |

---

## §1 Sorular

### A. Eksenler (kademe olcumu)

- [ ] **1. Tip ekseni >= 5000** ve `Hatali format 0` mi? (bugun `count-components` = **5440 PASS**)
  **Kanit:** `powershell -File .ai\scripts\count-components.ps1` -> `TOPLAM KAYIT 5440 (esik >= 5000)` + `SONUC PASS` (tarihle).
- [ ] **2. Dugum ekseni hedefe yakin mi?** (bugun `Kayit dosyasi: 52` — alt 44 + makro 8; hedef **5000**, eksik 4948 — Adim 28)
  **Kanit:** ayni sayac ciktisindaki `Kayit dosyasi : 44 (esik 52)` satiri + §2.3 kademe tablosu karsilastirmasi.
- [ ] **3. Fiziksel .dll ekseni faz sayisiyla eslesiyor mu?** `45 -> 500 -> 1500 -> 3000 -> 5000` (K0 + N1..N4)
  **Kanit:** `Get-ChildItem src,tests -Filter *.csproj` -> toplam; bugun **9 + 0 test + 0 `.sln`** = SCALE-100 eksik.
- [ ] **4. K bandi dolu mu?** `K0000..K5499` icinde **5000 dolu + 500 bos** (k5000-dll-list §1)
  **Kanit:** `dependency-map.md` matris boyutu (64 / 144 / 256 / 400 hucre) + satir sayimi.

### B. Adlandirma ve katalog

- [ ] **5. ID bicimi her satirda `K<4 hane>` mi?** (`k5000-layer-map.csv` `layerId` sutunu)
  **Kanit:** katalog uzerinde desen taramasi -> hatali ID = 0.
- [ ] **6. Her kayit 8 sutun dolu** (ID/Tip/Tur/Sorumluluk/Proje/TFM/Test/Durum)?
  **Kanit:** sayac format kontrolu -> eksik sutun 0.
- [ ] **7. `Proje` sutunu K bandindaki listeyle eslesiyor mu?** (K0 = 45 cekirdek, N1..N4 = scaffold; **giris Exe'leri (l7) hariç** - matris `l7 -> l6 = OK`, disari cikan Exe referansi kademe sayimina girmez)
  **Kanit:** katalogdaki farkli proje adi sayisi -> kademe esigi disi 0 (`DOĞRULAMA GEREKLI` - l7 Exe kenarlari NetArchTest ile henuz olculmedi).
- [ ] **8. `Durum` sutunu yalniz `planli | yazildi | test-edildi` mi?**
  **Kanit:** dis deger taramasi -> 0 satir.
- [ ] **9. `layer.json` 12 alan dolu mu + sayisi = dolu K slot mu?** (`gen-layer-json.ps1 -Check`)
  **Kanit:** pilot dugum (N01) icin diff -> eslesmeyen kayit 0 ya da liste (VC-LAY-009).

### C. Derleme, yon ve olcum

- [ ] **10. Cift bacak build 0 hata mi** (G6)?
  **Kanit:** `dotnet build -f net48` + `-f net10.0-windows` -> ikisi de `0 Hata` (tarih + sure).
- [ ] **11. Matris 0 ihlal mi?** (8x8 olcum = **18 OK · 38 X · 8 -** [dependency-map §2] korunur + 5000² uretilmis kural NetArchTest 0 ihlal)
  **Kanit:** `dotnet test --filter LayerRulesTests` -> 0 basarisiz test.
- [ ] **12. Iki sayac AYRI raporlandi mi?** 5440 tip **+** 5834 parca (fark **394** yazildi)
  **Kanit:** `count-components.ps1` + `gen-layer-5000.ps1` ciktisi **ayni raporda** (tek basina biri RED).

### D. Cozum dosyasi ve kapi

- [ ] **13. `.slnx` + `.sln` senkron mu?** (VC-LAY-013) + CPM gecerli mi (VC-LAY-014)?
  **Kanit:** iki dosyanin proje listesi diff'i -> **0 fark**; `dotnet restore` -> 0 hata; `Directory.Packages.props` VAR.
- [ ] **14. `count-dll.ps1` PASS mi?** (uretim >= 5000, hatali format 0, marj >= 0)
- [ ] **15. Kapi + kayit:** onaysiz `.csproj` yok, `log.md`'de tarihli kayit var, `git status` temiz?
  **Kanit:** `git status` + `log.md` son satir + GATE-003 onay satiri (kod fazina giris).

---

## §2 RED kosullari

| # | Soru | RED kosulu (birisi tutarsa is durur) |
|---|---|---|
| 1 | Soru 1 | tip < 5000 ya da hatali format > 0 -> GATE-004a acik kalir |
| 2 | Soru 2 | dugum eksigi **yazilmamis** -> LAY-09 ihlali (olcumsuz kademe iddiasi) |
| 3 | Soru 3 | `.csproj` toplami kademe disi -> LAY-05 + G7 |
| 4 | Soru 4 | yon grubu 8/12/16/20 disinda -> LAY-04 (keyfi `l21`+ dahil) |
| 5 | Soru 7 | kademe disi proje -> ADR-028/030 ihlali, `DECISION BLOCKED` |
| 6 | Soru 10 | bir bacakta bile hata -> G6 ihlali (HIGH) |
| 7 | Soru 11 | yon ihlali > 0 -> NetArchTest RED, GATE-005 acik |
| 8 | Soru 12 | tek sayacla "5000+" -> LAY-10 + G5, kutu RED |
| 9 | Soru 13 | senkron fark > 0 -> iki gercek (G10), `.sln` guncellenir |
| 10 | Soru 14 | onaysiz `.csproj` -> G7/G12 cift ihlal, is geri alinir |

---

## §3 Kapsam disi

| Bu liste NEYI KAPLAMAZ | Nerede yasar |
|---|---|
| Vault 500 MD / frontmatter / link on kontrolu | [gate-004a-vault-kapanis.md](gate-004a-vault-kapanis.md) |
| UI wireframe onayi (7 ekran) | [gate-003-ui-onay.md](gate-003-ui-onay.md) |
| Iskelet + ilk commit (K0 Adim 8) | [gate-004-ilk-commit.md](gate-004-ilk-commit.md) |
| Test kapsami %90 | [gate-005-test-kapsami.md](gate-005-test-kapsami.md) |
| Katman yonu 13 soru (ayrinti) | [layer-boundary-check.md](layer-boundary-check.md) |
| 29 paket-grup eslemesinin **dogrulugu** | [layer-rules §1.2](../.rules/layer-rules.md) (VC-LAY-011) |

---

## §4 RED durumunda

1. Soru numarasi + kirmizi cikti `log.md`'ye etiketle (`LAYER-5000-RED`).
2. Isi **ureten ajana** geri don; baska ajana devretme (AGENTS §4).
3. Duzeltme ayni oturumda; kanit yeniden alinmadan madde `[x]` olmaz (G5).
4. 3 ayni soru dusurse `3-STRIKE` + bloke raporu (AGENTS §4 sablonu).

---

## §5 Durum (2026-10-02 olcumu — doldurulmus hal)

| Soru | 1 | 2 | 3 | 4 | 5-9 | 10 | 11 | 12 | 13 | 14 |
|---|---|---|---|---|---|---|---|---|---|---|
| Durum | PASS | **EKSIK 56** | **EKSIK (9 vs 11)** | PASS (8/8) | PASS* | PASS* | **YOK** (test yok) | PASS | **`.sln` YOK** | KOD YOK (GATE-003) |
| Kanit | 5440 | 44 | 9 csproj | 64 hucre | format 0 | 2026-10-02 | 0 test projesi | 5440+5834 | slnx=1, sln=0 | 0/13 onay |

> *`layer-boundary-check` 13 sorusu ve NetArchTest **henuz calistirilmadi** (test projesi yok) -> `DOĞRULAMA GEREKLI`.
> **Sonuc:** 14 maddeden **5 PASS · 6 EKSIK · 2 YOK · 1 BEKLEME** -> **GATE-005 acik**, kod fazi **GATE-003 onayi** ile baslayabilir.

**Degisiklik gunlugu:**
- 2026-10-02 — **v1.0.0** ilk yazim (12 madde, ADR-030 v1).
- 2026-10-02 — **v2.0.0** ADR-030 v2 ile birlikte: 12 -> **14 madde** · A/B/C/D kategori bolumu · dugum ekseni (madde 2) · yon grubu 8·12·16·20 (madde 4) · iki sayac birlikte (madde 12) · `.slnx`+`.sln`+CPM senkron (madde 13) · §5 **doldurulmus durum tablosu** (bugunku olcumle) · RED kosulu 5 -> 10 madde.
