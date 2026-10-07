---
title: "async-await-ui-thread - VersaCoder"
type: knowledge
category: winforms
status: active
version: 1.0.0
updated: 2026-09-30
authority: SSOT
scope: "Olay isleyicilerinde async kullanim kurallarini, await sonrasi UI thread'e donusu, Invoke ihtiyacini ve donmus pencere belirtisini tanimlar."
---

# `async` / `await` ile UI Thread Kurali

**Zorunlu Baglantilar:** [vault indeks](../index.md) · [async-ui-thread-marshalling](async-ui-thread-marshalling.md) · [async-rules](../../.rules/async-rules.md) · [performance-guidelines](../../.rules/performance-guidelines.md) · [async-loading-patterns](async-loading-patterns.md)
**Küme + Hub:** [winforms index](index.md) · [index](../../index.md) · [CLAUDE](../../claude.md) · [AGENTS](../../definitions/agents.md) · [brain](../../.decisions/brain.md) · [glossary](../../definitions/glossary.md) · [keys](../../governance/keys.md)
**Kardeşler (winforms · 3/32):** [accessibility-tab-and-mnemonics](accessibility-tab-and-mnemonics.md) · [anchor-dock-layout-rules](anchor-dock-layout-rules.md) · [async-void-event-handlers](async-void-event-handlers.md) · [backgroundworker-vs-task](backgroundworker-vs-task.md) · [cancellation-in-ui](cancellation-in-ui.md) · [cancellation-token-in-ui](cancellation-token-in-ui.md) · [composition-root-host-startup](composition-root-host-startup.md) · [control-disposal-and-memory](control-disposal-and-memory.md) · [controls-lifecycle-events](controls-lifecycle-events.md) · [data-binding-basics](data-binding-basics.md) · [designer-cs-anatomy](designer-cs-anatomy.md) · [designer-cs-trio-rule](designer-cs-trio-rule.md) · [designer-editing-rules](designer-editing-rules.md) · [designer-hand-edit-policy](designer-hand-edit-policy.md) · [di-container-lifetimes](di-container-lifetimes.md) · [docking-panels-patterns](docking-panels-patterns.md) · [errorprovider-validation-flow](errorprovider-validation-flow.md) · [exception-boundary-ui](exception-boundary-ui.md) · [form-close-and-dispose](form-close-and-dispose.md) · [form-close-cancel-pattern](form-close-cancel-pattern.md) · [form-constructor-vs-load](form-constructor-vs-load.md) · [form-load-and-shown-events](form-load-and-shown-events.md) · [layout-table-panel](layout-table-panel.md) · [logging-integration](logging-integration.md) · [partial-class-and-file-naming](partial-class-and-file-naming.md) · [settings-user-config](settings-user-config.md) · [splitcontainer-and-sizing](splitcontainer-and-sizing.md) · [validation-form-level](validation-form-level.md) · [winforms-rendering-performance](winforms-rendering-performance.md)

> **Tek cumlelik sorumluluk:** *UI thread'de hangi kod donmus pencere uretir, `await` sonrasi devam hangi thread'de surer ve `Invoke` ne zaman zorunludur* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Olay isleyicisi (`Xxx_Click`, `Xxx_Load`) `async void` olabilir ve olmalidir; **diger her metot** `async Task` doner. UI thread'de `Thread.Sleep`, senkron I/O ve `Task.Result` / `.Wait()` **yasaktir** - hepsi pencereyi dondurur. `await` sonrasi devam, WinForms `SynchronizationContext` sayesinde **otomatik olarak UI thread'e** doner; ayrica bir `Invoke` gerekmez.

```text
+------------------------------------------------------------------+
| UI thread'de Sleep / .Result / .Wait()  -> PENCERE DONAR (yasak) |
| await sonrasi devam                      -> otomatik UI thread   |
| Task.Run BLOK ICINDE kontrole dokunma    -> BeginInvoke zorunlu  |
+------------------------------------------------------------------+
```

---

## §1 Desen karar tablosu

| # | Desen | Karar | Neden |
|---|---|---|---|
| 1 | `async void Xxx_Click(...)` | tek istisna | olay sistemi `Task` bekleyemez |
| 2 | `async Task XxxAsync(...)` servis metodu | zorunlu | test edilebilir, `await` edilebilir |
| 3 | `Thread.Sleep(...)` UI'da | yasak | thread'i bloklar, "Yanit Vermiyor" |
| 4 | `task.Result` / `task.Wait()` | yasak | deadlock riski (sync-over-async) |
| 5 | Arka plandan kontrole yazmak | `Invoke` / `BeginInvoke` gerekli | kontrol thread-affine |

---

## §2 Yanlis ve dogru ornek

```csharp
// YANLIS - pencere donar
private void btnRun_Click(object sender, EventArgs e)
{
    var data = _repository.LoadAll();   // senkron I/O
    Thread.Sleep(1000);
    grid.DataSource = data;
}
```

```csharp
// DOGRU
private async void btnRun_Click(object sender, EventArgs e)
{
    try
    {
        btnRun.Enabled = false;
        UseWaitCursor = true;
        var data = await _repository.LoadAllAsync();   // UI thread serbest
        grid.DataSource = data;
    }
    catch (Exception ex)
    {
        _errors.Report(ex);
    }
    finally
    {
        btnRun.Enabled = true;
        UseWaitCursor = false;
    }
}
```

---

## §3 Sira tablosu (hangi adim hangi thread'de)

| # | Adim | Thread | Not |
|---|---|---|---|
| 1 | Olay tetiklenir | UI | |
| 2 | `await` oncesi kod | UI | kontroller guvenli |
| 3 | I/O / `Task.Run` | havuz | UI thread birakilir |
| 4 | `await` sonrasi | **UI** (otomatik) | `SynchronizationContext` |
| 5 | Kontrol `DataSource` ataması | UI | guvensiz degil |

---

## §4 `Invoke` gereken tek durum

`Task.Run` blogu icinde **dogrudan** kontrole dokunmak yasastir; kontrole yalniz `BeginInvoke` ile donulur:

```csharp
await Task.Run(() =>
{
    var rows = ComputeRows();                                  // agir hesap, UI yok
    BeginInvoke(new Action(() => grid.DataSource = rows));     // UI'a donus
});
```

> **KURAL:** `await Task.Run(...)` sonrasi **otomatik** UI thread'e donersiniz; blog **icinde** dokunacaksaniz `BeginInvoke` zorunludur. Ikisini karistirmak "cross-thread operation" hatasi uretir.

---

## §5 Tarama tablosu (kim neyi degistirir)

| Aranacak | `.Form.cs` beklenen | Kim sahip |
|---|---|---|
| `Thread.Sleep(` | 0 eslesme | `developer` |
| `.Result` / `.Wait()` | 0 eslesme | `developer` |
| `private async void` | yalniz olay isleyicisi | `developer` |
| `BeginInvoke(` / `Invoke(` | yalniz `Task.Run` blogu icinde | `developer` |
| `async` mantiginin `#if` icine alinmasi | 0 eslesme (yalniz API farki) | `reviewer` |

---

## §6 Iki bacak notu (net48 / net10.0-windows)

`WindowsFormsSynchronizationContext` her iki bacakta da vardir. `net48`'de `async void` olay isleyicilerinde analiz kurali daha kati olabilir; `net10.0-windows` tarafinda ayni kod derlenir. `#if` **yalniz** API farki icin kullanilir - `async` mantigi asla `#if` icine alinmaz.

---

## §7 Kontrol listesi

| # | Adim | Kanit |
|---|---|---|
| 1 | UI'da `Thread.Sleep` / `.Result` / `.Wait()` yok | grep hedefi: 0 eslesme |
| 2 | `async void` yalniz olay isleyicilerinde | kod incelemesi |
| 3 | `Task.Run` blogu icinde kontrole dokunulmuyor | kod incelemesi |
| 4 | Her `async void` icinde `try/catch/finally` var | kod incelemesi |
| 5 | Iki bacakta `dotnet build` -> 0 hata | build ciktisi DOĞRULAMA GEREKLI |

---

## §9 Kanit

| Kaynak dosya | Ne alindi |
|---|---|
| `knowledge\winforms\async-await-ui-thread.md` (kaynak vault) | desen karari, yanlis/dogru ornek, sira tablosu, BeginInvoke kurali, iki bacak notu |
| `.rules\async-rules.md` | async/await ve sync-over-async yasagi |
| `.rules\performance-guidelines.md` | UI thread'de bloke eden islem kurali |

> Kanitsiz iddia icin `DOĞRULAMA GEREKLI` yazilir; bu dosyada kaynak disi bilgi yoktur.
