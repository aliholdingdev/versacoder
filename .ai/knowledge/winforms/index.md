---
title: "winforms — Bilgi Kümesi İndeksi"
type: index
category: knowledge
status: active
version: 1.0.0
updated: 2026-10-07
authority: SSOT
scope: "WinForms Form uclusu, layout, async, erisilebilirlik, Designer — 32 dosyanin tek satir ozeti ve tam baglanti listesi"
---

# Winforms — Bilgi Kümesi İndeksi

**Zorunlu Bağlantılar:** [knowledge index](../index.md) · [vault index](../../index.md) · [CLAUDE](../../CLAUDE.md) · [AGENTS](../../AGENTS.md) · [brain](../../brain.md) · [glossary](../../glossary.md) · [keys](../../keys.md)
**Küme + Hub:** [winforms index](index.md)

> **Tek cümlelik sorumluluk:** Bu klasördeki **32 bilgi dosyasının** hangisini
> ne zaman açacağını tek tabloda söyler; cevabın kendisi o dosyadadır.

## §0 Özet (Answer First)

- **32 dosya** · 8 alanlı frontmatter · 3–8 KB (ölçüm 2026-10-07)
- **Kullanım:** konuyu tablada bul → dosya adına tıkla → § bölümlerine in.
- **Küme içi tam bağlantı:** her dosya, aşağıdaki tabloda listelenen diğer 31 kardeşine linklidir.

## §1 Dosya Listesi (32)

| # | Dosya | Tek satır (frontmatter scope) |
|---|---|---|
| 1 | [accessibility-tab-and-mnemonics.md](accessibility-tab-and-mnemonics.md) | Klavye ile tam gezinme: TabIndex/TabStop ayari, Alt-tusu mnemonikleri, okunabilir ad ve … |
| 2 | [anchor-dock-layout-rules.md](anchor-dock-layout-rules.md) | Pencere boyutlandiginda kontrollerin davranisi: Anchor/Dock farki, AutoSize tuzagi, Resize … |
| 3 | [async-await-ui-thread.md](async-await-ui-thread.md) | Olay isleyicilerinde async kullanim kurallarini, await sonrasi UI thread'e donusu, Invoke … |
| 4 | [async-loading-patterns.md](async-loading-patterns.md) | Form acilisinda ve sorgu degisiminde verinin nasil yuklenecegini, yuklenirken arayuzun … |
| 5 | [async-ui-thread-marshalling.md](async-ui-thread-marshalling.md) | Arka planda calisan isin sonucunun UI thread'e donusu: SynchronizationContext, … |
| 6 | [async-void-event-handlers.md](async-void-event-handlers.md) | async void metotlarinin yalniz nerede kullanildigini, kacinan istisnalarin nereye gittigini … |
| 7 | [backgroundworker-vs-task.md](backgroundworker-vs-task.md) | Arka plan isi icin BackgroundWorker ile Task/async arasindaki secimi: olaylar, iptal, hata … |
| 8 | [cancellation-in-ui.md](cancellation-in-ui.md) | WinForms'ta iptal akisini: CancellationToken zincirini, iptal butonu ve FormClosing … |
| 9 | [cancellation-token-in-ui.md](cancellation-token-in-ui.md) | Form kapanarken veya yeni arama baslarken eski isin iptal edilmesini, … |
| 10 | [composition-root-host-startup.md](composition-root-host-startup.md) | Uygulamanin nerede kuruldugunu, Program.cs'in ne icerip ne iceremedigini, #if platform … |
| 11 | [control-disposal-and-memory.md](control-disposal-and-memory.md) | WinForms'ta GDI+ nesneleri, event abonelikleri, Form ve Control disposal sirasini ve bellek … |
| 12 | [controls-lifecycle-events.md](controls-lifecycle-events.md) | Control baz sinifinin olaylarini, parent-child olusum ve dispose sirasini, odak/vurgu/desen … |
| 13 | [data-binding-basics.md](data-binding-basics.md) | WinForms veri baglamanin parcalarini (Binding, BindingSource, CurrencyManager), yon ve … |
| 14 | [designer-cs-anatomy.md](designer-cs-anatomy.md) | XxxForm.Designer.cs dosyasinin bolum bolum anatomisini, tek InitializeComponent kaynagini … |
| 15 | [designer-cs-trio-rule.md](designer-cs-trio-rule.md) | Her formun neden tam uc dosyadan olustugunu, hangi dosyanin hangi yetkiye sahip oldugunu ve … |
| 16 | [designer-editing-rules.md](designer-editing-rules.md) | .Designer.cs ve .resx'in ne zaman elle duzenlenebilegini, bozulma belirtilerini ve git … |
| 17 | [designer-hand-edit-policy.md](designer-hand-edit-policy.md) | .Designer.cs ve .resx uzerindeki son soz hakkini, elle mudahalenin sartlarini, geri alinma … |
| 18 | [di-container-lifetimes.md](di-container-lifetimes.md) | Singleton/transient omurlerinin hangi servise verilecegi, WinForms'ta scope karsiligi (form … |
| 19 | [docking-panels-patterns.md](docking-panels-patterns.md) | Dock sirasini, panel kalibi secimini, panel durum kaydini (user.config) ve DevExpress … |
| 20 | [errorprovider-validation-flow.md](errorprovider-validation-flow.md) | ErrorProvider ile alan bazli dogrulamanin akisini, tek instance kuralini, hata temizleme … |
| 21 | [exception-boundary-ui.md](exception-boundary-ui.md) | Uygulamadaki uc kuresel hata yakalayicisini, kullaniciya gosterim politikasini ve iki bacak … |
| 22 | [form-close-and-dispose.md](form-close-and-dispose.md) | Formun kapanis kararinin nerede verildigini, iptal edilebildigini ve tum … |
| 23 | [form-close-cancel-pattern.md](form-close-cancel-pattern.md) | FormClosing iptalini, kaydet-sorma oruntusunu, CloseReason ayrimini ve FormClosed ile … |
| 24 | [form-constructor-vs-load.md](form-constructor-vs-load.md) | Form constructor'nda ne yapilmadigini, Load / Shown / Activated ayrimini ve ilk veri … |
| 25 | [form-load-and-shown-events.md](form-load-and-shown-events.md) | Form acilirken tetiklenen Load, Shown ve Activated olaylarinin farkini, hangi isin nereye … |
| 26 | [layout-table-panel.md](layout-table-panel.md) | TableLayoutPanel ve FlowLayoutPanel kullanimini, Dock-Anchor sirasini, AutoSize tuzaklarini … |
| 27 | [logging-integration.md](logging-integration.md) | WinForms uygulamasinda log yazimi: kutudan cikan Trace/TextWriterTraceListener hatti, dosya … |
| 28 | [partial-class-and-file-naming.md](partial-class-and-file-naming.md) | Form dosya adi = sinif adi kurali, partial class butunlugu, tek sinif/dosya, #region yasagi … |
| 29 | [settings-user-config.md](settings-user-config.md) | Kullanici ayarlarinin saklanmasi: ISettingsStore soyutlamasi, atomik dosya yazimi, sirlari … |
| 30 | [splitcontainer-and-sizing.md](splitcontainer-and-sizing.md) | SplitContainer kullanim kosullarini, panel sirasinin dock yapisina etkisini ve … |
| 31 | [validation-form-level.md](validation-form-level.md) | Form seviyesi dogrulama zincirini (Validating, AutoValidate, ValidateChildren, … |
| 32 | [winforms-rendering-performance.md](winforms-rendering-performance.md) | WinForms render performansi: Paint maliyeti, DoubleBuffered/flicker, Invalidate ile Refresh … |

> **Hub:** [knowledge index](../index.md) diğer 4 kümeye (csharp · data · engine · winforms · devexpress) açılır.
