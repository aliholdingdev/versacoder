---
title: "gate-004a-vault-kapanis - VersaCoder"
type: checklist
category: checklists
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "GATE-004a (vault green) cikis kriteri: 500 MD, 0 bozuk link, 0 frontmatter eksigi, 0 mojibake, rezerve adlarin uretilmesi"
---

# gate-004a-vault-kapanis — GATE-004a Vault Green Kapaniş Kontrolü

**Zorunlu Baglantilar:** [vault index](../index.md) · [WORKFLOW](../WORKFLOW.md) · [kayit defteri](index.md) · [coding-standards](../.rules/coding-standards.md)

> **Tek cumlelik sorumluluk:** *Vault 500 MD hedefine ulasti mi, bicim ve link kurallari sifir ihlalle duruyor mu* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

GATE-004a, P3 (vault kurulumu) cikis kapisi ve **P4 oncesindeki son kontrol**. Listeyi `architect` doldurur, kapanisi **kullanici** yapar; uclusu script ciktilari (measure-vault · check-links · count-components) disinda kanit kabul edilmez. Tek bir "evet" bile ciktisiz ise liste gecersizdir (CLAUDE.md G5).

| Alan | Deger |
|---|---|
| Kim doldurur | `architect` (kanit toplayan) |
| Kim kapatir | **kullanici** — ajan kapatamaz (WORKFLOW §3.1 #2) |
| Zorunlu kanit | 3 script ciktisi + `git status --porcelain` |
| Soru sayisi | 14 · hepsi `[ ]` + `Kanit:` satiri |

---

## §1 Sorular

- [x] 1. Toplam MD sayisi **≥ 500** mi?
  **Kanit:** `powershell -File .ai\scripts\measure-vault.ps1` ciktisindaki `TotalMD` satiri (rakamla). => SONUC: TotalMD = 500 (esik >= 500), measure exit=0
- [x] 2. Frontmatter'siz dosya sayisi **0** mi?
  **Kanit:** ayni ciktida `NoFrontmatter = 0` satiri. => SONUC: NoFrontmatter = 0
- [x] 3. Bozuk ic link sayisi **0** mi?
  **Kanit:** `powershell -File .ai\scripts\check-links.ps1` ciktisi: `BOZUK link : 0` + `exit 0`. => SONUC: BOZUK link : 0 / Iyi link : 5699, check-links exit=0
- [x] 4. Mojibake (bozuk encoding) dosyasi **0** mi?
  **Kanit:** `powershell -File .ai\scripts\find-mojibake.ps1` ciktisi: `Mojibake file count: 0`. => SONUC: Mojibake file count: 0, find-mojibake exit=0
- [x] 5. Bilesen kaydi **5440+** ve script `PASS` mi?
  **Kanit:** `powershell -File .ai\scripts\count-components.ps1` ciktisindaki `PASS` satiri. => SONUC: TOPLAM KAYIT 5440 (esik >= 5000), Hatali format 0, SONUC PASS, exit=0
- [x] 6. 14 koku belge (index, AGENTS, CLAUDE, WORKFLOW, ROLE, keys, glossary, brain, MEMORY, VISION, PROJECTS, log, engine, ULTRA-THINKING) diskte mi?
  **Kanit:** klasor listesi + her dosyanin ilk satirinin `---` oldugunun ekran alintisi. => SONUC: 14/14 dosya diskte, hepsinin ilk satiri ---
- [x] 7. Her MD'nin frontmatter'i 8 alan + `authority: SSOT` + `updated: 2026-09-25` mi?
  **Kanit:** rastgele secilen 5 dosyanin ilk 10 satiri (kopyalanmis alinti).
  **ISTISNA (kullanici karari):** olcum - 500/500 dosyada frontmatter var (alan 8-9), authority satiri eksik 0; dagilim: SSOT 426 | derived 55 (catalog - ADR-013ten turetilmis) | ".ai/AGENTS.md" 16 (prompts). `updated` literal 2026-09-25 yerine guncel tarih (2026-09-29/30). Secenek: A) bu istisnayi kabul et (dogru etiketler korunur) B) hepsini SSOT + 09-25 yap (uydurma olur, RED).
  **KARAR (2026-09-30):** kullanici istisnayi kabul etti - GATE-004a onaylandi (14/14).
- [x] 8. B8 batch'inin 4 REZERVE adi birebir uretilmis mi?
  **Kanit:** `gate-004a-vault-kapanis.md` · `gate-003-ui-onay.md` · `gate-004-ilk-commit.md` · `gate-005-test-kapsami.md` icin `Test-Path` ciktisi = 4 x `True`. => SONUC: 4 x True (Test-Path hepsi)
- [x] 9. `checklists/index.md` kayit tablosundaki 4 link artik **yesil** mi?
  **Kanit:** `check-links.ps1` ciktisinda bu 4 hedefin "HEDEF BAZLI GRUPLAMA" listesinde olmamasi. => SONUC: 4 hedef checklists/index.md icerinde; check-links 0 oldugundan kirik listesinde degiller
- [x] 10. Yasak icerik taramasi sifir mi (EF Core · DevExpress v26.1 disi · string-concat SQL)?
  **Kanit:** CLAUDE.md §10 G2 + G3 komutlarinin ucunun de `0 satir` veren ciktisi. => SONUC: 0 satir - src/ yok (0 csproj, EntityFrameworkCore 0 satir); kod yazilinca GATE-004 tekrar kosacak
- [x] 11. Kopyalama (G1) izi yok mu?
  **Kanit:** `git status --porcelain` ciktisi — yalniz `?? .ai/` (ya da beklenen dosya listesi); `C:\www\` altinda hicbir degisiklik yok. => SONUC: hedefte git status = "?? .ai/" (1 satir); C:\www altinda bizim yazdigimiz dosya yok (oradaki 5 untracked = eski proje artigi, onceden vardi)
- [x] 12. `authority: SSOT` disi ikinci bir "kaynak" dosya (tekrar) uretilmis mi?
  **Kanit:** ayni bilginin iki dosyada tekrarini aratan tarama sonucu = 0 kritik satir; bulgu varsa satir adi. => SONUC: 0 kritik tekrar - 37 tekrarli uzun satirin hepsi kalip: sablon esleri 2 cift, katalog kalip satiri 32 dosya, boundary-check satiri 2 dosya; veri/karar tekrari yok
- [x] 13. `log.md`'ye tarihli kapanis kaydi dustu mu?
  **Kanit:** `log.md` son kayit satiri (tarih + GATE-004a + 3 script sayisi). => SONUC: log.md satir 169 - "22 | 2026-09-30 | dort kapinin final kosusu | measure=0/links=0/mojibake=0/count=PASS | GATE-004a kullanici onayi bekliyor"
- [x] 14. Kapsam disi dosya (`.old.*`, CoreMusic kopyasi) vault'a girmis mi?
  **Kanit:** `Get-ChildItem C:\versacoder\.ai -Recurse -Filter *.old.*` ciktisi = 0 dosya. => SONUC: 0 dosya (Get-ChildItem *.old.* = 0)

---

## §2 RED Kosullari

| # | Soru | RED kosulu (birisi tutarsa is durur) |
|---|---|---|
| 1 | Soru 3 | `BOZUK link > 0` → G4 ihlali, GATE-004a acik kalir |
| 2 | Soru 1+2 | `TotalMD < 500` ya da `NoFrontmatter > 0` → P3 eksik |
| 3 | Soru 8 | REZERVE adlardan biri yoksa `checklists/index.md` linki kirik → batch B8 gecersiz |
| 4 | Soru 10 | EF Core / v26.1 disi / string-concat satiri > 0 → G2/G3 CRITICAL |
| 5 | Soru 4 | `Mojibake file count > 0` → dosyalar UTF-8 olarak yeniden yazilir |

---

## §3 Kapsam Disi

| Bu liste NEYI KAPLAMAZ | Nerede yasar |
|---|---|
| Kod derleme / test yeşilligi (`net48` + `net10.0-windows`) | [gate-004-ilk-commit.md](gate-004-ilk-commit.md) |
| Coverage ve test kapsami yuzdesi | [gate-005-test-kapsami.md](gate-005-test-kapsami.md) |
| Wireframe ve UI onayi | [gate-003-ui-onay.md](gate-003-ui-onay.md) |
| Vault icerik kalitesi (soru-kanit bicimi) | [documentation-check.md](documentation-check.md) · [vault-link-check.md](vault-link-check.md) |
| Aci/odul, gate sirasi, sure raporlamasi | [../WORKFLOW.md](../WORKFLOW.md) §3 · [../spec/test-strategy.md](../spec/test-strategy.md) |