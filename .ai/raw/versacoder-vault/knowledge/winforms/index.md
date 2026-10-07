---
title: "WinForms Knowledge Index"
type: index
category: winforms
status: active
version: 1.0.0
updated: 2026-09-24
authority: SSOT
scope: winforms-index
---

# WinForms Knowledge Index

## §0 Özet (Answer First)

Bu dizin, `.ai/knowledge/winforms/` altındaki **30 teknik dokümanının** kayıt defteridir. Her satır bir dosya, bir sorumluluk, bir durum. Dosyalar arası geçiş kardeş isimle yapılır; izinli olmayan hedeflere link verilmez (bkz. §3).

## §1 Dosya Kayıt Defteri

| # | Dosya | Sorumluluk | Durum |
|---|---|---|---|
| 1 | `form-lifecycle-constructor-flow.md` | Form yaşam döngüsü: ctor → Initialize → show akışı | ✅ |
| 2 | `form-load-and-shown-events.md` | `Load`/`Shown`/`Activated` zamanlaması ve ilk veri yükleme | ✅ |
| 3 | `form-close-and-dispose.md` | `Closing`/`Closed`/`Dispose` sırası ve iptal kalıbı | ✅ |
| 4 | `form-showdialog-versus-show.md` | Modal (`ShowDialog`) vs modeless (`Show`) seçimi | ✅ |
| 5 | `designer-cs-trio-rule.md` | Üçlü şablon: `.cs` + `.Designer.cs` + `.resx` kuralı | ✅ |
| 6 | `code-behind-event-wiring.md` | Event aboneliği kod-behind'da; abonelik temizliği | ✅ |
| 7 | `resx-resource-structure.md` | `.resx` iç yapısı, kod-suz resx kuralı | ✅ |
| 8 | `resourcemanager-name-contract.md` | `ResourceManager` ad sözleşmesi ve lookup zinciri | ✅ |
| 9 | `partial-class-splitting.md` | Partial class bölme ve dosya sorumlulukları | ✅ |
| 10 | `designer-hand-edit-policy.md` | `Designer.cs` elle düzenlemesi politikası | ✅ |
| 11 | `di-container-lifetimes.md` | Scoped/Singleton/Transient yaşam döngüleri | ✅ |
| 12 | `composition-root-host-startup.md` | Composition root: `Program.cs` host başlangıcı | ✅ |
| 13 | `constructor-injection-in-forms.md` | Form'larda constructor injection kalıbı | ✅ |
| 14 | `service-locator-avoidance.md` | Service locator'dan kaçınma, anti-pattern | ✅ |
| 15 | `async-await-ui-thread.md` | `async/await` + UI thread (Invoke) etkileşimi | ✅ |
| 16 | `async-void-event-handlers.md` | `async void` event handler'ların güvenli kalıbı | ✅ |
| 17 | `cancellation-token-in-ui.md` | UI'da `CancellationToken` ile iptal akışı | ✅ |
| 18 | `async-loading-patterns.md` | Async veri yükleme: loading state, hata, tekrar | ✅ |
| 19 | `anchor-dock-layout-rules.md` | Anchor/Dock ile esnek düzen kuralları | ✅ |
| 20 | `autoscalemode-dpi-scaling.md` | `AutoScaleMode` DPI/ölçek uyumu | ✅ |
| 21 | `splitcontainer-and-sizing.md` | `SplitContainer` ve boyutlandırma davranışı | ✅ |
| 22 | `ribbon-form-client-area-layout.md` | Ribbon formunda client alanı yerleşimi | ✅ |
| 23 | `localized-string-resources.md` | Yerelleştirilmiş string kaynakları akışı | ✅ |
| 24 | `image-and-icon-resources.md` | Görsel/ikon kaynak yönetimi | ✅ |
| 25 | `accessibility-tab-and-mnemonics.md` | Erişilebilirlik: Tab sırası ve mnemonic'ler | ✅ |
| 26 | `keyboard-shortcuts-and-keyevents.md` | Kısayol tuşları ve `KeyDown`/`KeyPress` akışı | ✅ |
| 27 | `errorprovider-validation-flow.md` | `ErrorProvider` ile doğrulama akışı | ✅ |
| 28 | `global-exception-handling.md` | Global exception yakalama (UI + non-UI) | ✅ |
| 29 | `winforms-rendering-performance.md` | Render performansı: paint, buffer, ölçüm | ✅ |
| 30 | `control-disposal-and-memory.md` | Control disposal ve bellek/GDI yönetimi | ✅ |

## §2 Kullanım

| İhtiyaç | Gidilecek dosya |
|---|---|
| Form açılışında ne olur? | `form-lifecycle-constructor-flow.md` |
| Yeni form eklerken kurallar? | `designer-cs-trio-rule.md` |
| Servisleri forma nasıl veririm? | `constructor-injection-in-forms.md` |
| Ağrıyan kayıt asenkron yüklensin | `async-loading-patterns.md` |
| FPS düşüyor / GDI artıyor | `winforms-rendering-performance.md`, `control-disposal-and-memory.md` |

Adım adım iş akışı: `../../WORKFLOW.md` (varsa).

## §3 İzinli Link Hedefleri

| Hedef | Durum |
|---|---|
| `../../CLAUDE.md` | Var → link verilir |
| `../../index.md` | Yok → link verilmez (`NEEDS_VALIDATION`) |
| `../../keys.md` | Yok → link verilmez (`NEEDS_VALIDATION`) |
| `../../VISION.md` | Yok → link verilmez (`NEEDS_VALIDATION`) |
| `../../.rules/*.md` | Yok → link verilmez (`NEEDS_VALIDATION`) |
| `../index.md` | Yok → link verilmez (`NEEDS_VALIDATION`) |
| Tabloda listelenen 30 kardeş dosya | Var → link verilir |

## §4 Yasaklar

1. Kayıtdışı dosya eklemek (önce §1'e satır, sonra içerik).
2. §3'te `NEEDS_VALIDATION` işaretli hedefe link vermek.
3. Kardeş dosya adını yanlış yazmak (link kırılır).
4. Durum sütununu doğrulamadan ✅ işaretlemek.
