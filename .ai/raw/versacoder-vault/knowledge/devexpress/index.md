---
title: "DevExpress Knowledge Index"
type: index
category: knowledge
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "knowledge/devexpress altindaki 53 teknik dokumanin kayit defteri: sorumluluk, durum ve izinli link hedefleri"
---

# DevExpress Knowledge Index

**Zorunlu Bağlantılar:** [../index.md](../index.md) · [../../index.md](../../index.md) · [../../CLAUDE.md](../../CLAUDE.md) · [../../keys.md](../../keys.md) · [devexpress-rules](../../.rules/devexpress-rules.md) · [ADR-004](../../.decisions/accepted/ADR-004-devexpress-universal-v261-office2010.md)

> **Tek cümlelik sorumluluk:** *DevExpress v26.1 hakkında hangi dosyada ne var, o dosyaya nasıl ulaşılır* sorusunun tek cevabıdır.
> Kural metni `.rules/devexpress-rules.md`'dedir; burada yalnız **kayıt + yönlendirme** vardır (SRP).

---

## §0 Özet (Answer First)

Bu dizin `.ai/knowledge/devexpress/` altındaki **53 teknik dokümanın** kayıt defteridir. Hepsi **DevExpress Universal 2026 / v26.1** ve **`RibbonControlStyle.Office2010`** kilidi içindedir; sürüm sapması (G3) taşıyan satır **RED**'dir. Dosya adları kod olarak yazılır (3–8 KB sınırı); link hedefleri §3'te sınırlıdır.

---

## §1 Dosya Kayıt Defteri (53)

| # | Dosya | Sorumluluk | Durum |
|---|---|---|---|
| 1 | `accessibility-keyboard-nav.md` | `AccessibleName`/`Role`, tab sırası, kısayollar | ✅ |
| 2 | `accordion-navigation.md` | AccordionControl grup/öge hiyerarşisi + arama | ✅ |
| 3 | `alertcontrol-notifications.md` | AlertControl uyarı penceresi, tıklama verisi, HTML şablonu | ✅ |
| 4 | `bar-status-items.md` | StatusBar + `BarStaticItem`/`BarEditItem`, ribbon ilişkisi | ✅ |
| 5 | `chartcontrol-basics.md` | ChartControl: Dapper `DataTable` bağlama, seri/eksen | ✅ |
| 6 | `checkedcomboedit-values.md` | CheckedComboBoxEdit çok seçim, `GetCheckedValues` | ✅ |
| 7 | `comboboxedit-autocomplete.md` | ComboBoxEdit otomatik tamamlama + editor seçimi | ✅ |
| 8 | `datasource-binding-dapper.md` | Kontrollere Dapper: `IDataReader` → `DataTable` | ✅ |
| 9 | `dateedit-timeedit.md` | DateEdit/TimeEdit format, `Min/MaxValue`, doğrulama | ✅ |
| 10 | `designer-runtime-customization.md` | Form üçlüsü, Designer anatomisi, runtime sınırı | ✅ |
| 11 | `docking-saved-layout-xml.md` | DockManager düzeninin XML kaydı/geri yüklenmesi | ✅ |
| 12 | `docklayout-persistence.md` | Layout serileştirme: XML/Stream/Registry, sürüm uyumu | ✅ |
| 13 | `dockmanager-panels.md` | Yan paneller, konum, AutoHide/Hidden, IDE benzeri düzen | ✅ |
| 14 | `dx-validation-provider.md` | DXErrorProvider: hata ikonu, Validating akışı | ✅ |
| 15 | `grid-conditional-format-rules.md` | GridFormatRule koşullu biçim; elle hücre boyama **yasak** | ✅ |
| 16 | `grid-editing-modes.md` | Düzenleme modları, RepositoryItem, satır doğrulama | ✅ |
| 17 | `grid-export-excel-pdf.md` | XLSX/CSV/PDF/HTML dışa aktarma, editör kapatma | ✅ |
| 18 | `grid-filtering-sorting.md` | `ActiveFilterString` + `SortInfo`; DB tarafı ayrımı | ✅ |
| 19 | `grid-grouping-summaries.md` | Gruplama + `GroupSummary`/`Summary` toplamları | ✅ |
| 20 | `grid-paging-virtual-mode.md` | OFFSET/FETCH sayfalama, sanal mod, `BeginUpdate` | ✅ |
| 21 | `grid-search-panel-find.md` | Arama paneli, `OptionsFind`, büyük veri araması | ✅ |
| 22 | `gridcontrol-basics.md` | GridControl/GridView iskeleti, bağlama sırası, olaylar | ✅ |
| 23 | `gridview-columns-format.md` | Kolon bağları, tip/biçim katmanları, unbound kolonlar | ✅ |
| 24 | `layoutcontrol-grouping.md` | LayoutControl gruplar, tablo/akış modu, boyut kısıtı | ✅ |
| 25 | `localizing-ui-text.md` | Yerelleştirme: kültür resx, Localizer katmanları | ✅ |
| 26 | `lookUpEdit-cascading.md` | Kademeli LookUpEdit, `EditValueChanged` ile yeniden yükleme | ✅ |
| 27 | `memoedit-richedit.md` | MemoEdit düz metin, RichEdit DOCX/RTF akışı | ✅ |
| 28 | `mvvm-pattern-devexpress.md` | ViewModelBase + DelegateCommand + Messenger sınırı | ✅ |
| 29 | `navigationframe-pages.md` | NavigationFrame/NavigationPage tek ekran geçişi | ✅ |
| 30 | `nuget-references-cpm.md` | NuGet + CPM, TFM `net48`/`net10`, binding redirect | ✅ |
| 31 | `pdf-image-viewer.md` | PdfViewer + PictureEdit paketleri ve `LoadDocument` | ✅ |
| 32 | `performance-grid-optimization.md` | Grid performansı: yükleme, sanal mod eşiği, ölçüm | ✅ |
| 33 | `pictureedit-upload.md` | PictureEdit görsel gösterimi, dosya seçimi, format kontrolü | ✅ |
| 34 | `pivotgrid-basics.md` | PivotGridField alanları, `Area`/`SummaryType` | ✅ |
| 35 | `ribbon-application-menu.md` | Office2010 uygulama butonu: ApplicationMenu vs Backstage | ✅ |
| 36 | `ribbon-auto-hide-scaling.md` | Ribbon auto-hide + DPI ölçekleme, anchoring | ✅ |
| 37 | `ribbon-context-categories.md` | RibbonPageCategory koşullu sekmeler, görünürlük | ✅ |
| 38 | `ribbon-control-office2010.md` | **`RibbonControlStyle.Office2010` tek stil kanunu** | ✅ |
| 39 | `ribbon-form-basics.md` | RibbonForm ana pencere kalıbı + RibbonStatusBar | ✅ |
| 40 | `ribbon-pages-groups-items.md` | Page → Group → BarItem zinciri, tek `ItemClick` | ✅ |
| 41 | `ribbon-qat-quick-access.md` | Hızlı erişim çubuğu (QAT) konumu ve özelleştirme | ✅ |
| 42 | `richedit-document-api.md` | RichEdit belge API: aç/kaydet, salt okunur | ✅ |
| 43 | `serialization-layout-persistence.md` | Grid/layout XML kaydet-yükle, `OptionsLayout` | ✅ |
| 44 | `skin-customization.md` | Skin özelleştirme, AppearanceObject, DPI | ✅ |
| 45 | `spinedit-calc-edit.md` | SpinEdit/CalcEdit aralık, precision, kültür farkı | ✅ |
| 46 | `splashscreen-waitform.md` | SplashScreenManager splash/wait/overlay seçimi | ✅ |
| 47 | `tabcontrol-multipage.md` | XtraTabControl başlık konumu, çok satır, gezinme | ✅ |
| 48 | `textboxedit-masks.md` | TextEdit mask (RegEx/Numeric/DateTime) + doğrulama | ✅ |
| 49 | `theme-manager-office2010.md` | UserLookAndFeel/SkinManager, Office2010 ilişkisi | ✅ |
| 50 | `tile-control-startpage.md` | TileControl başlangıç ekranı, navigasyon, erişilebilirlik | ✅ |
| 51 | `togglecheck-edit.md` | ToggleSwitch/CheckEdit ikili/üçlü seçim durumları | ✅ |
| 52 | `treelist-hierarchical-data.md` | TreeList Id/ÜstId hiyerarşi, `Key/ParentFieldName` | ✅ |
| 53 | `treelist-inplace-editing.md` | TreeList hücre düzenleme, `ValidateNode`, Dapper kaydetme | ✅ |

---

## §2 Kullanım (neyi arıyorsan)

| İhtiyaç | Gidilecek dosya |
|---|---|
| Pencere / ribbon iskeleti | `ribbon-form-basics.md`, `ribbon-pages-groups-items.md` |
| Sürüm / stil isteği (G3) | `ribbon-control-office2010.md` |
| Listeye veri (Dapper) | `datasource-binding-dapper.md`, `gridcontrol-basics.md` |
| Düzen + doğrulama | `layoutcontrol-grouping.md`, `dx-validation-provider.md` |
| Bellek / FPS | `performance-grid-optimization.md` |

---

## §3 İzinli Link Hedefleri

| Hedef | Durum |
|---|---|
| `../../index.md` · `../index.md` · `../../CLAUDE.md` · `../../keys.md` · `../../VISION.md` | Var → link verilir |
| `../../.rules/devexpress-rules.md` · `../../.rules/coding-standards.md` | Var → link verilir |
| `../../.rules/data-access-rules.md` · `security-rules.md` · `testing-rules.md` | Var → link verilir |
| `../../.decisions/accepted/ADR-004…` · `ADR-005…` · `ADR-012…` | Var → link verilir |
| §1'de adı geçen 53 kardeş dosya | Var → adıyla (dizin 3–8 KB sınırı) |
| Bunun dışındaki hedef | **Yasak** → link kırılır (GATE-004a) |

---

## §4 Yasaklar

1. §1'de kaydı olmayan dosya eklemek (önce satır, sonra içerik).
2. §3'te yasak yazan hedefe link vermek.
3. Kardeş dosya adını yanlış yazmak (ad = link).
4. Doğrulamadan ✅ işaretlemek ve sürüm (v26.1) dışı iddia üretmek.

**Değişiklik günlüğü:** 2026-09-25 — ilk yazım (architect). Kaynak: dosya frontmatter `scope:`.
