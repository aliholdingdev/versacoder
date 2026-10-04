---
title: "winforms-designer-check - VersaCoder"
type: checklist
category: checklists
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "WinForms form + .Designer.cs + .resx ucyusu icin 13 kanitli soru: ResourceManager, dis elleme, kaynak, layout"
---

# winforms-designer-check — WinForms Form / Designer / resx Kontrolü

**Zorunlu Baglantilar:** [vault index](../index.md) · [CLAUDE.md](../CLAUDE.md) · [kayit defteri](index.md) · [coding-standards](../.rules/coding-standards.md)

> **Tek cumlelik sorumluluk:** *Form `.Form.cs` + `.Designer.cs` + `.resx` ucyusu tutarli mi, `ResourceManager` adlari bozulmamis mi* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Her ekran **uc dosyadan** olusur; `.Designer.cs` ve `.resx` son sozu `ui-designer`'da, `developer` yalnizca olay yonlendirici yazar. Elle `Designer.cs`/`.resx` mudahalesi `ResourceManager` adlarini bozar → form acilmaz. Kanit = dosya ucyusu + `ResourceManager` arama ciktisi + Designer'in acilir durumdasi.

| Alan | Deger |
|---|---|
| Kim doldurur | `ui-designer` (dosya) · `reviewer` (denetim) |
| Son soz (Designer/resx) | `ui-designer` — `developer` elleme yapamaz |
| Kanit paketi | dosya listesi + `ResourceManager` taramasi |
| Soru sayisi | 13 · hepsi `[ ]` + `Kanit:` satiri |

---

## §1 Sorular

- [ ] 1. Her ekran icin **3 dosya** da var mi (`.Form.cs`, `.Form.Designer.cs`, `.Form.resx`)?
  **Kanit:** `Get-ChildItem src\**\*.Form.*` listesi; eksik uc = 0.
- [ ] 2. Her dosyanin ilk satiri `---` (vault MD'leri) ya da gecerli C# headeri mi?
  **Kanit:** ornek 1 formun 3 dosyasinin ilk 5 satiri.
- [ ] 3. `InitializeComponent()` icinde **sadece** tasarim kodu mu var (is mantigi **0**)?
  **Kanit:** en buyuk `.Form.Designer.cs` satir sayisi + `if (`/`foreach` aramasi = 0 satir.
- [ ] 4. `.resx` dosyasi **designer uzerinden** mi uretilmis (elle XML duzenleme izi yok)?
  **Kanit:** `.resx` icinde elle eklenmis `data name` suphesi ya da "0 suphe" satiri.
- [ ] 5. `ResourceManager` / `.resx` ad eslesmesi saglam mi (eksik anahtar 0)?
  **Kanit:** `Select-String -Path src\**\*.cs -Pattern 'ResourceManager'` ciktisi + resx `data name` listesi karsilastirmasi.
- [ ] 6. Control adlari `ui-design/NN-<ekran>.md` envanteri ile **ayni** mi?
  **Kanit:** wireframe kontrol listesi ile `Designer.cs` control adlarinin karsilastirma tablosu (fark = 0).
- [ ] 7. Code-behind **≤ 100 satir** ve yalniz olay yonlendirici mi?
  **Kanit:** `.Form.cs` satir sayisi (rakamla) + en buyuk metot boyutu.
- [ ] 8. Is mantigi `.Form.cs`'de degil, Application katmaninda mi (SQL/servis cagrisi 0)?
  **Kanit:** `Select-String -Path src\**\*.Form.cs -Pattern 'SELECT |QueryAsync|HttpClient'` = 0 satir.
- [ ] 9. Form `RibbonForm` mu (`XtraForm` ana pencere kullanilmamis)?
  **Kanit:** `Select-String -Path src\**\*.cs -Pattern 'class \w+ : \w+' | ... XtraForm` = 0 satir + ornek sinif tanimi.
- [ ] 10. Layout sabit piksel mi, anchor/dock ile mi (ekran boyunca tasma yok mu)?
  **Kanit:** ornek formun `Anchor`/`Dock`/`AutoSize` satirlari + 1366x768 notu.
- [ ] 11. Localization icin metinler `.resx`'te mi (`"KAYDET"` sabit string 0 olmali, Onaylanan disinda)?
  **Kanit:** `Select-String -Path src\**\*.Form.cs -Pattern 'Text = "'` ciktisi; disarida kalan satir sayisi.
- [ ] 12. Form dispose/kapatma kaynaklari temizliyor mu (event handler var mi)?
  **Kanit:** `FormClosing`/`Dispose` handler ornegi ya da "0 kaynak" satiri.
- [ ] 13. `reviewer` denetim raporu `.ai/reports/**` altinda mi?
  **Kanit:** rapor dosya yolu + `0 CRITICAL · 0 HIGH` satiri.

---

## §2 RED Kosullari

| # | Soru | RED kosulu (birisi tutarsa is durur) |
|---|---|---|
| 1 | Soru 1 | Eksik dosya (ucden az) → form acilmaz, is RED |
| 2 | Soru 3 veya 8 | Designer'da / code-behind'ta is mantigi → katman + yetki ihlali |
| 3 | Soru 5 | `ResourceManager` adi kirildi → `ui-designer`'a devredilir (AGENTS.md §3.4) |
| 4 | Soru 9 | `XtraForm` ana pencere → **G3 CRITICAL** |
| 5 | Soru 6 | Control adi wireframe ile farkli → **GATE-003 yeniden acilir** (G12) |

---

## §3 Kapsam Disi

| Bu liste NEYI KAPLAMAZ | Nerede yasar |
|---|---|
| GATE-003 wireframe onayinin kendisi | [gate-003-ui-onay.md](gate-003-ui-onay.md) |
| DevExpress kontrol API secimi (v26.1) | [devexpress-control-check.md](devexpress-control-check.md) |
| Ribbon stili ve RibbonForm kalibi | [ribbon-style-check.md](ribbon-style-check.md) |
| Erisilebilirlik (klavye, kontrast) | [accessibility-check.md](accessibility-check.md) |
| Code-behind / katman siniri kurallari | [layer-boundary-check.md](layer-boundary-check.md) |
