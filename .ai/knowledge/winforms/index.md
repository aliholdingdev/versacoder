---
title: "winforms — Bilgi Kümesi İndeksi"
type: index
category: knowledge
status: active
version: 1.0.0
updated: 2026-10-07
authority: reference
scope: "WinForms Form uclusu, layout, async, erisilebilirlik, Designer — 32 dosyanin tek satir ozeti ve tam baglanti listesi"
tags: [knowledge]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# Winforms — Bilgi Kümesi İndeksi

**Zorunlu Bağlantılar:** [[knowledge/index|knowledge index]] · [[index|vault index]] · [[claude|CLAUDE]] · [[definitions/agents|AGENTS]] · [[.decisions/brain|brain]] · [[definitions/glossary|glossary]] · [[governance/keys|keys]]
**Küme + Hub:** [[knowledge/winforms/index|winforms index]]

> **Tek cümlelik sorumluluk:** Bu klasördeki **32 bilgi dosyasının** hangisini
> ne zaman açacağını tek tabloda söyler; cevabın kendisi o dosyadadır.

## §0 Özet (Answer First)

- **32 dosya** · 11 alanlı frontmatter · 3–8 KB (ölçüm 2026-10-07)
- **Kullanım:** konuyu tablada bul → dosya adına tıkla → § bölümlerine in.
- **Küme içi tam bağlantı:** her dosya, aşağıdaki tabloda listelenen diğer 31 kardeşine linklidir.

## §1 Dosya Listesi (32)

| # | Dosya | Tek satır (frontmatter scope) |
|---|---|---|
| 1 | [[knowledge/winforms/accessibility-tab-and-mnemonics|accessibility-tab-and-mnemonics.md]] | Klavye ile tam gezinme: TabIndex/TabStop ayari, Alt-tusu mnemonikleri, okunabilir ad ve … |
| 2 | [[knowledge/winforms/anchor-dock-layout-rules|anchor-dock-layout-rules.md]] | Pencere boyutlandiginda kontrollerin davranisi: Anchor/Dock farki, AutoSize tuzagi, Resize … |
| 3 | [[knowledge/winforms/async-await-ui-thread|async-await-ui-thread.md]] | Olay isleyicilerinde async kullanim kurallarini, await sonrasi UI thread'e donusu, Invoke … |
| 4 | [[knowledge/winforms/async-loading-patterns|async-loading-patterns.md]] | Form acilisinda ve sorgu degisiminde verinin nasil yuklenecegini, yuklenirken arayuzun … |
| 5 | [[knowledge/winforms/async-ui-thread-marshalling|async-ui-thread-marshalling.md]] | Arka planda calisan isin sonucunun UI thread'e donusu: SynchronizationContext, … |
| 6 | [[knowledge/winforms/async-void-event-handlers|async-void-event-handlers.md]] | async void metotlarinin yalniz nerede kullanildigini, kacinan istisnalarin nereye gittigini … |
| 7 | [[knowledge/winforms/backgroundworker-vs-task|backgroundworker-vs-task.md]] | Arka plan isi icin BackgroundWorker ile Task/async arasindaki secimi: olaylar, iptal, hata … |
| 8 | [[knowledge/winforms/cancellation-in-ui|cancellation-in-ui.md]] | WinForms'ta iptal akisini: CancellationToken zincirini, iptal butonu ve FormClosing … |
| 9 | [[knowledge/winforms/cancellation-token-in-ui|cancellation-token-in-ui.md]] | Form kapanarken veya yeni arama baslarken eski isin iptal edilmesini, … |
| 10 | [[knowledge/winforms/composition-root-host-startup|composition-root-host-startup.md]] | Uygulamanin nerede kuruldugunu, Program.cs'in ne icerip ne iceremedigini, #if platform … |
| 11 | [[knowledge/winforms/control-disposal-and-memory|control-disposal-and-memory.md]] | WinForms'ta GDI+ nesneleri, event abonelikleri, Form ve Control disposal sirasini ve bellek … |
| 12 | [[knowledge/winforms/controls-lifecycle-events|controls-lifecycle-events.md]] | Control baz sinifinin olaylarini, parent-child olusum ve dispose sirasini, odak/vurgu/desen … |
| 13 | [[knowledge/winforms/data-binding-basics|data-binding-basics.md]] | WinForms veri baglamanin parcalarini (Binding, BindingSource, CurrencyManager), yon ve … |
| 14 | [[knowledge/winforms/designer-cs-anatomy|designer-cs-anatomy.md]] | XxxForm.Designer.cs dosyasinin bolum bolum anatomisini, tek InitializeComponent kaynagini … |
| 15 | [[knowledge/winforms/designer-cs-trio-rule|designer-cs-trio-rule.md]] | Her formun neden tam uc dosyadan olustugunu, hangi dosyanin hangi yetkiye sahip oldugunu ve … |
| 16 | [[knowledge/winforms/designer-editing-rules|designer-editing-rules.md]] | .Designer.cs ve .resx'in ne zaman elle duzenlenebilegini, bozulma belirtilerini ve git … |
| 17 | [[knowledge/winforms/designer-hand-edit-policy|designer-hand-edit-policy.md]] | .Designer.cs ve .resx uzerindeki son soz hakkini, elle mudahalenin sartlarini, geri alinma … |
| 18 | [[knowledge/winforms/di-container-lifetimes|di-container-lifetimes.md]] | Singleton/transient omurlerinin hangi servise verilecegi, WinForms'ta scope karsiligi (form … |
| 19 | [[knowledge/winforms/docking-panels-patterns|docking-panels-patterns.md]] | Dock sirasini, panel kalibi secimini, panel durum kaydini (user.config) ve DevExpress … |
| 20 | [[knowledge/winforms/errorprovider-validation-flow|errorprovider-validation-flow.md]] | ErrorProvider ile alan bazli dogrulamanin akisini, tek instance kuralini, hata temizleme … |
| 21 | [[knowledge/winforms/exception-boundary-ui|exception-boundary-ui.md]] | Uygulamadaki uc kuresel hata yakalayicisini, kullaniciya gosterim politikasini ve iki bacak … |
| 22 | [[knowledge/winforms/form-close-and-dispose|form-close-and-dispose.md]] | Formun kapanis kararinin nerede verildigini, iptal edilebildigini ve tum … |
| 23 | [[knowledge/winforms/form-close-cancel-pattern|form-close-cancel-pattern.md]] | FormClosing iptalini, kaydet-sorma oruntusunu, CloseReason ayrimini ve FormClosed ile … |
| 24 | [[knowledge/winforms/form-constructor-vs-load|form-constructor-vs-load.md]] | Form constructor'nda ne yapilmadigini, Load / Shown / Activated ayrimini ve ilk veri … |
| 25 | [[knowledge/winforms/form-load-and-shown-events|form-load-and-shown-events.md]] | Form acilirken tetiklenen Load, Shown ve Activated olaylarinin farkini, hangi isin nereye … |
| 26 | [[knowledge/winforms/layout-table-panel|layout-table-panel.md]] | TableLayoutPanel ve FlowLayoutPanel kullanimini, Dock-Anchor sirasini, AutoSize tuzaklarini … |
| 27 | [[knowledge/winforms/logging-integration|logging-integration.md]] | WinForms uygulamasinda log yazimi: kutudan cikan Trace/TextWriterTraceListener hatti, dosya … |
| 28 | [[knowledge/winforms/partial-class-and-file-naming|partial-class-and-file-naming.md]] | Form dosya adi = sinif adi kurali, partial class butunlugu, tek sinif/dosya, #region yasagi … |
| 29 | [[knowledge/winforms/settings-user-config|settings-user-config.md]] | Kullanici ayarlarinin saklanmasi: ISettingsStore soyutlamasi, atomik dosya yazimi, sirlari … |
| 30 | [[knowledge/winforms/splitcontainer-and-sizing|splitcontainer-and-sizing.md]] | SplitContainer kullanim kosullarini, panel sirasinin dock yapisina etkisini ve … |
| 31 | [[knowledge/winforms/validation-form-level|validation-form-level.md]] | Form seviyesi dogrulama zincirini (Validating, AutoValidate, ValidateChildren, … |
| 32 | [[knowledge/winforms/winforms-rendering-performance|winforms-rendering-performance.md]] | WinForms render performansi: Paint maliyeti, DoubleBuffered/flicker, Invalidate ile Refresh … |

> **Hub:** [[knowledge/index|knowledge index]] diğer 4 kümeye (csharp · data · engine · winforms · devexpress) açılır.
