---
title: "devexpress-control-check - VersaCoder"
type: checklist
category: checklists
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "DevExpress v26.1 kontrol kullanimi icin 13 kanitli surum + API + data-binding + grid sorusu"
---

# devexpress-control-check — DevExpress v26.1 Kontrol Kullanim Kontrolü

**Zorunlu Baglantilar:** [vault index](../index.md) · [CLAUDE.md](../CLAUDE.md) · [kayit defteri](index.md) · [coding-standards](../.rules/coding-standards.md)

> **Tek cumlelik sorumluluk:** *Kullanilan her DevExpress kontrolu v26.1'de var mi, surum tek satir mi, data-binding null mu yönetiyor* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

DevExpress **Universal 2026 / v26.1** tek surumdur; baska surum referansi `G3/G8` CRITICAL'dir. Kontrol API'si hakkinda bilinmeyen her iddia `NEEDS_VALIDATION` ile isaretlenir (G9) — uydurma API yazilamaz. Kanit = CPM surum satiri + proje referansi + kontrol adinin gectigi dosya:satır.

| Alan | Deger |
|---|---|
| Kim doldurur | `ui-designer` (kullanim) · `reviewer` (denetim) |
| Tek surum | `DevExpress Universal 2026 / v26.1` |
| Kanit paketi | CPM satiri + grep cikti + dosya:satır |
| Soru sayisi | 13 · hepsi `[ ]` + `Kanit:` satiri |

---

## §1 Sorular

- [ ] 1. CPM'te DevExpress **tek satir** ve degeri `26.1` mi?
  **Kanit:** `Select-String -Path Directory.Packages.props -Pattern 'DevExpress'` = 1 satir, `26.1`.
- [ ] 2. `.csproj`'te ikinci bir DevExpress surumu var mi (0 satir olmali)?
  **Kanit:** `Select-String -Path **\*.csproj -Pattern 'DevExpress'` = 0 satir (CPM yonetir).
- [ ] 3. Kullanilan tum kontroller `component-inventory.md` listesinde mi?
  **Kanit:** kodda gecen kontrol adlari ile envanter karsilastirmasi (fark = 0).
- [ ] 4. `RibbonForm` ana pencere olarak kullaniliyor mu?
  **Kanit:** sinif tanimi satiri (`class MainForm : RibbonForm`).
- [ ] 5. Grid/TreeList veri kaynagi `BindingSource`/`DataSource` uzerinden mi (elle `Items.Add` **0**)?
  **Kanit:** `Select-String -Path src\**\*.cs -Pattern '\.Items\.Add\('` = 0 satir + baglama ornegi.
- [ ] 6. Grid kolonlari `FieldName` ile bagli mi (unbound kolon 0 olmali)?
  **Kanit:** `Select-String -Path src\**\*.cs -Pattern 'FieldName\s*='` sayisi + unbound aramasi = 0.
- [ ] 7. `DataSource` null/bose geldiginde kontroller kiriliyor mu (guard var mi)?
  **Kanit:** `if (rows is null)` benzeri guard ornegi + dosya:satır.
- [ ] 8. Event handler'larda `sender`/`e` cast'i guvenli mi (InvalidCastException riski)?
  **Kanit:** 1 handler ornegi + `as`/`is` kullanimi.
- [ ] 9. Bilesen dispose ediliyor mu (form kapatildiginda kaynak sızıntısı 0)?
  **Kanit:** `Dispose`/`using` kullanimi ya da designer'in otomatik dispose blogu.
- [ ] 10. Kontrol surumune bagli `NEEDS_VALIDATION` iddialari var mi (hepsi etiketli mi)?
  **Kanit:** `Select-String -Path src\**\*.cs -Pattern 'NEEDS_VALIDATION'` ciktisi; etiketsiz iddia = 0.
- [ ] 11. Tema/renk sabit kodlar mi, tema sistemi uzerinden mi (`Appearance` / `LookAndFeel`)?
  **Kanit:** 1 ornek `Appearance` kullanimi + `Color.FromArgb` sayisi (yorumla birlikte).
- [ ] 12. DevExpress disi UI kutuphanesi (WPF/web) var mi (0 olmali)?
  **Kanit:** `Select-String -Path Directory.Packages.props -Pattern 'Avalonia|Blazor|WPF|LiveCharts'` = 0 satir.
- [ ] 13. Bulgular `.ai/reports/**` altina yazildi mi?
  **Kanit:** rapor dosya yolu + seviye sayilari.

---

## §2 RED Kosullari

| # | Soru | RED kosulu (birisi tutarsa is durur) |
|---|---|---|
| 1 | Soru 1 veya 2 | Surum ≠ `26.1` / ikinci satir → **G3 CRITICAL** |
| 2 | Soru 4 | `XtraForm` ana pencere → **G3 CRITICAL** |
| 3 | Soru 3 | Envante disi kontrol → GATE-003 kapsam disi, onay gerekir |
| 4 | Soru 10 | Etiketsiz API iddiasi → **G9**, madde silinir |
| 5 | Soru 12 | DevExpress disi UI kutuphanesi → **G8**, kullanici onayi gerekir |

---

## §3 Kapsam Disi

| Bu liste NEYI KAPLAMAZ | Nerede yasar |
|---|---|
| Ribbon stilinin kendisi (`Office2010`) | [ribbon-style-check.md](ribbon-style-check.md) |
| Form/Designer/resx ucyusu | [winforms-designer-check.md](winforms-designer-check.md) |
| GATE-003 wireframe onayi | [gate-003-ui-onay.md](gate-003-ui-onay.md) |
| Performans (virtual mode, tick butcesi) | [performance-budget-check.md](performance-budget-check.md) |
| Paket surumu ve CPM denetimi | [nuget-package-check.md](nuget-package-check.md) · [../CLAUDE.md](../CLAUDE.md) §13 |
