---
title: "cancellation-token-in-ui - VersaCoder"
type: knowledge
category: winforms
status: active
version: 1.0.0
updated: 2026-09-30
authority: SSOT
scope: "Form kapanarken veya yeni arama baslarken eski isin iptal edilmesini, CancellationTokenSource omrini ve iptalin UI'a yansitilmasini tanimlar."
---

# UI'da `CancellationToken` Kullanimi

**Zorunlu Baglantilar:** [vault indeks](../index.md) · [cancellation-in-ui](cancellation-in-ui.md) · [async-ui-thread-marshalling](async-ui-thread-marshalling.md) · [async-rules](../../.rules/async-rules.md) · [error-handling-rules](../../.rules/error-handling-rules.md)
**Küme + Hub:** [winforms index](index.md) · [index](../../index.md) · [CLAUDE](../../CLAUDE.md) · [AGENTS](../../AGENTS.md) · [brain](../../brain.md) · [glossary](../../glossary.md) · [keys](../../keys.md)
**Kardeşler (winforms · 3/32):** [accessibility-tab-and-mnemonics](accessibility-tab-and-mnemonics.md) · [anchor-dock-layout-rules](anchor-dock-layout-rules.md) · [async-await-ui-thread](async-await-ui-thread.md) · [async-loading-patterns](async-loading-patterns.md) · [async-void-event-handlers](async-void-event-handlers.md) · [backgroundworker-vs-task](backgroundworker-vs-task.md) · [composition-root-host-startup](composition-root-host-startup.md) · [control-disposal-and-memory](control-disposal-and-memory.md) · [controls-lifecycle-events](controls-lifecycle-events.md) · [data-binding-basics](data-binding-basics.md) · [designer-cs-anatomy](designer-cs-anatomy.md) · [designer-cs-trio-rule](designer-cs-trio-rule.md) · [designer-editing-rules](designer-editing-rules.md) · [designer-hand-edit-policy](designer-hand-edit-policy.md) · [di-container-lifetimes](di-container-lifetimes.md) · [docking-panels-patterns](docking-panels-patterns.md) · [errorprovider-validation-flow](errorprovider-validation-flow.md) · [exception-boundary-ui](exception-boundary-ui.md) · [form-close-and-dispose](form-close-and-dispose.md) · [form-close-cancel-pattern](form-close-cancel-pattern.md) · [form-constructor-vs-load](form-constructor-vs-load.md) · [form-load-and-shown-events](form-load-and-shown-events.md) · [layout-table-panel](layout-table-panel.md) · [logging-integration](logging-integration.md) · [partial-class-and-file-naming](partial-class-and-file-naming.md) · [settings-user-config](settings-user-config.md) · [splitcontainer-and-sizing](splitcontainer-and-sizing.md) · [validation-form-level](validation-form-level.md) · [winforms-rendering-performance](winforms-rendering-performance.md)

> **Tek cumlelik sorumluluk:** *Bir `CancellationTokenSource` ne zaman dogar, ne zaman iptal edilir ve hangi olayda serbest birakilir* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Her forma ait `CancellationTokenSource` **form omrunedir**: yeni bir is baslarken eskisi iptal edilir (`Cancel` + `Dispose` + yenisi), form kapanirken (`FormClosed`) iptal edilir ve `Dispose` edilir. Iptal **hata degildir**: `OperationCanceledException` yakalanir, kullaniciya hata diyalogu **cikarilmaz**, durum cubugu guncellenir.

```text
+---------------------------------------------------------------+
| kurulurken   : _cts = new CancellationTokenSource()           |
| yeni is      : eski Cancel + Dispose + yenisi                 |
| is cagrisi   : token tum async zincirden ilerletilir          |
| iptal butonu : _cts.Cancel()                                  |
| kapanis      : FormClosed -> Cancel + Dispose + null          |
+---------------------------------------------------------------+
```

---

## §1 Omur tablosu

| # | Zaman | Ne yapilir | Nerede |
|---|---|---|---|
| 1 | Form kurulurken | `_cts = new CancellationTokenSource()` | kurucu (`.Form.cs`) |
| 2 | Yeni is baslarken | eski `Cancel()` + `Dispose()` + yenisi | olay isleyicisi |
| 3 | Is cagrisina | `token` ilerletilir | `.Form.cs` -> katmanlar |
| 4 | "Iptal" butonu | `_cts.Cancel()` | olay isleyicisi |
| 5 | Form kapanirken | `Cancel()` + `Dispose()` | `FormClosed` |

---

## §2 Kalip

```csharp
public partial class ChatForm : RibbonForm
{
    private CancellationTokenSource? _searchCts;

    private async void txtQuery_TextChanged(object sender, EventArgs e)
    {
        _searchCts?.Cancel();                 // 1) eski isi iptal et
        _searchCts?.Dispose();
        _searchCts = new CancellationTokenSource();
        var token = _searchCts.Token;         // 2) yenisini al

        try
        {
            var hits = await _searcher.SearchAsync(txtQuery.Text, token);
            grid.DataSource = hits;
        }
        catch (OperationCanceledException)
        {
            status.Caption = "Arama iptal edildi.";   // hata diyalogu YOK
        }
        catch (Exception ex)
        {
            _errors.Report(ex);
        }
    }

    private void ChatForm_FormClosed(object sender, FormClosedEventArgs e)
    {
        _searchCts?.Cancel();
        _searchCts?.Dispose();
        _searchCts = null;
    }
}
```

---

## §3 Iptal mi, hata mi?

| Olay | Kullaniciya ne gorunur | Kod |
|---|---|---|
| `OperationCanceledException` | durum cubugu: "Iptal" | `catch (OperationCanceledException)` |
| Ag hatasi | hata diyalogu + rapor | `catch (Exception ex)` |
| Zaman asimi (`TimeoutException`) | "Zaman asimi" ayri satir | ayri `catch` |
| Form kapandiktan sonra donus | **hicbirsey** (kontrol disposed) | `token.IsCancellationRequested` kontrolu |

---

## §4 Yaygin hatalar

| # | Hata | Belirti | Duzeltme |
|---|---|---|---|
| 1 | `_cts` hic `Dispose` edilmiyor | GC bekcisi artar | her `Cancel` sonrasi `Dispose` |
| 2 | Ayni `_cts` iki is paylasiliyor | biri digerini iptal eder | is basina yeni `CancellationTokenSource` |
| 3 | Iptal istisnasi hata diyalogu gosteriyor | kullanici sasar | ayri `catch` |
| 4 | `FormClosed`'da iptal yok | form kapandiktan sonra is devam | `FormClosed`'da iptal |
| 5 | `token` hic ilerletilmiyor | iptal calismaz | tum async zincire `token` gecir |

---

## §5 Tarama tablosu (kim neyi degistirir)

| Aranacak | Beklenen | Kim |
|---|---|---|
| `new CancellationTokenSource()` | is basinda yenisi, tekrar kullanim yok | `developer` |
| `_cts.Dispose()` | her `Cancel` ile birlikte | `developer` |
| `catch (OperationCanceledException)` | ayri ve en ustte | `reviewer` |
| `FormClosed` icinde `Cancel()` | her forma | `reviewer` |
| `#if` icine alinmis iptal mantigi | 0 eslesme (yalniz API farki serbest) | `developer` |

`CancellationTokenSource` her iki bacakta da ayni API'yi kullanir. `#if` yalniz platforma ozgu API farkina (orn. `WaitAsync` yardimci metodu yoklugu) izin verir; iptal mantigi asla `#if` icine alinmaz.

---

## §6 Kontrol listesi

| # | Adim | Kanit |
|---|---|---|
| 1 | Her `_cts` `Cancel` + `Dispose` ediliyor | kod incelemesi |
| 2 | `FormClosed` icinde iptal var | kod incelemesi |
| 3 | `OperationCanceledException` ayri yakalaniyor | grep ciktisi |
| 4 | Token async zincirin tamaminda ilerletiliyor | kod incelemesi |
| 5 | Iki bacakta `dotnet build` -> 0 hata | build ciktisi DOĞRULAMA GEREKLI |

---

## §9 Kanit

| Kaynak dosya | Ne alindi |
|---|---|
| `knowledge\winforms\cancellation-token-in-ui.md` (kaynak vault) | omur tablosu, kalip, iptal/hata ayrimi, 5 yaygin hata, iki bacak notu |
| `.rules\async-rules.md` | token imzalama ve zincir ilerletme kurali |
| `.rules\error-handling-rules.md` | iptal istisnasinin hata sayilmamasi |

> Kanitsiz iddia icin `DOĞRULAMA GEREKLI` yazilir; bu dosyada kaynak disi bilgi yoktur.
