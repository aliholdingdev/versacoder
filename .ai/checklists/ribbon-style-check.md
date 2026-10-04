---
title: "ribbon-style-check - VersaCoder"
type: checklist
category: checklists
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "Ribbon stili denetimi icin 12 kanitli soru: Office2010 tek stil, RibbonForm, sayfa/grup yapisi, QAT"
---

# ribbon-style-check — Ribbon Stil (`Office2010`) Kontrol Listesi

**Zorunlu Baglantilar:** [vault index](../index.md) · [CLAUDE.md](../CLAUDE.md) · [kayit defteri](index.md) · [coding-standards](../.rules/coding-standards.md)

> **Tek cumlelik sorumluluk:** *Ribbon stili yalniz `RibbonControlStyle.Office2010` mu, baska stil referansi sifir mi* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Bu vault'ta ribbon stili **tek satirdir**: `RibbonControlStyle.Office2010`. `Office2007`, `Office2010Blue`, `Office2010Silver`, `OfficeUniversal` ve baska her stil `G3 CRITICAL` sayilir. Ayrica ana pencere `RibbonForm` olmak zorundadir; `XtraForm`/`Form` ana pencere kabul edilmez. Kanit = tek bir grep ciktisi + form sinif tanimi.

| Alan | Deger |
|---|---|
| Kim doldurur | `ui-designer` (kurulum) · `reviewer` (tarama) |
| Tek stil | `RibbonControlStyle.Office2010` |
| Kanit paketi | 2 grep ciktisi + 1 sinif tanimi |
| Soru sayisi | 12 · hepsi `[ ]` + `Kanit:` satiri |

---

## §1 Sorular

- [ ] 1. Stil atamasi **yalniz** `RibbonControlStyle.Office2010` mi?
  **Kanit:** `Select-String -Path src\**\*.cs -Pattern 'RibbonControlStyle\.\w+'` ciktisi — tum satirlar `Office2010`.
- [ ] 2. `Office2007` / `Office2010Blue` / `Office2010Silver` / `OfficeUniversal` geciyor mu (0 satir olmali)?
  **Kanit:** `Select-String -Path src\**\*.cs,*.resx -Pattern 'Office2007|Office2010Blue|Office2010Silver|OfficeUniversal'` = 0 satir.
- [ ] 3. Ana pencere `RibbonForm` mu?
  **Kanit:** sinif tanimi satiri + `Select-String -Pattern ': RibbonForm'` sayisi ≥ 1.
- [ ] 4. `RibbonControl` formun `Controls` koleksiyonuna designer'da eklenmis mi?
  **Kanit:** `.Form.Designer.cs` icinde `((System.ComponentModel.ISupportInitialize)(this.ribbonControl1)).BeginInit()` blogu.
- [ ] 5. Her ribbon sayfasi (`RibbonPage`) en az bir gruba sahip mi (bos sayfa 0)?
  **Kanit:** `RibbonPage` sayisi ile `RibbonPageGroup` sayisi karsilastirmasi (grup ≥ sayfa).
- [ ] 6. Butonlar `ItemLinks.Add` ile mi baglandi (elle `Controls.Add` **0**)?
  **Kanit:** `Select-String -Path src\**\*.cs -Pattern 'ribbonControl1\.Controls\.Add'` = 0 satir.
- [ ] 7. Her `BarButtonItem`'in bir `ItemClick` hedefi var mi (hedefsiz buton 0)?
  **Kanit:** `BarButtonItem` sayisi ile `ItemClick` handler sayisi karsilastirmasi.
- [ ] 8. Ribbon metinleri `.resx`'ten mi geliyor (yerellestirilebilir)?
  **Kanit:** 1 ornek `Caption`/`Text` atamasi + yerellestirme notu (ya da "sabit, gerekce").
- [ ] 9. Ikonlar SVG/glyph olarak mi (raster `Image.FromFile` ile dosya yolu **0**)?
  **Kanit:** `Select-String -Path src\**\*.cs -Pattern 'Image\.FromFile'` = 0 satir.
- [ ] 10. Quick Access Toolbar (QAT) gizli mi ya da tanimli mi (rastgele durum 0)?
  **Kanit:** `ribbonControl1.ShowQuickAccessItem` satiri (degeriyle birlikte).
- [ ] 11. `MergeRibbon` / MDI birlestirme kullaniliyor mu (kullanilmiyorsa "0" yazilir)?
  **Kanit:** `Select-String -Path src\**\*.cs -Pattern 'MergeRibbon|RibbonMerge'` = 0 satir ya da gerekce.
- [ ] 12. Ekranda ilk gorsel dogrulama yapildi mi (ribbon aciliyor mu)?
  **Kanit:** calistirilmis `dotnet build` + ekran goruntusu/notu; kanit yoksa `NEEDS_VALIDATION`.

---

## §2 RED Kosullari

| # | Soru | RED kosulu (birisi tutarsa is durur) |
|---|---|---|
| 1 | Soru 1 veya 2 | `Office2010` disi herhangi bir stil → **G3 CRITICAL** |
| 2 | Soru 3 | `RibbonForm` degilse ana pencere kurali kirik → form yeniden kurulur |
| 3 | Soru 6 | Buton `Controls.Add` ile eklenmis → designer disi mudahale, RED |
| 4 | Soru 4 | `RibbonControl` kayitli degilse form acilmaz → `ui-designer`'a devir |
| 5 | Soru 12 | Kanitsiz "ribbon calisiyor" → **G5**, NEEDS_VALIDATION |

---

## §3 Kapsam Disi

| Bu liste NEYI KAPLAMAZ | Nerede yasar |
|---|---|
| Genel DevExpress kontrol secimi ve surumu | [devexpress-control-check.md](devexpress-control-check.md) |
| Form/Designer/resx ucyusu ve `ResourceManager` | [winforms-designer-check.md](winforms-designer-check.md) |
| GATE-003 wireframe onayi | [gate-003-ui-onay.md](gate-003-ui-onay.md) |
| Erisilebilirlik (klavye kisayollari, kontrast) | [accessibility-check.md](accessibility-check.md) |
| Stil kuralinin anayasasi | [../CLAUDE.md](../CLAUDE.md) §1 G3 · [../spec/test-strategy.md](../spec/test-strategy.md) |
