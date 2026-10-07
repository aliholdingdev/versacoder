---
title: "form-load-and-shown-events - VersaCoder"
type: knowledge
category: winforms
status: active
version: 1.0.0
updated: 2026-09-30
authority: primary
scope: "Form acilirken tetiklenen Load, Shown ve Activated olaylarinin farkini, hangi isin nereye konacagini ve ilk yukleme mantiginin nasil korunacagini tanimlar."
tags: [knowledge]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# `Load`, `Shown` ve `Activated` Olaylari

**Zorunlu Baglantilar:** [vault indeks](../index.md) · [form-constructor-vs-load](form-constructor-vs-load.md) · [designer-cs-trio-rule](designer-cs-trio-rule.md) · [async-void-event-handlers](async-void-event-handlers.md) · [ui-rules](../../.rules/ui-rules.md)
**Küme + Hub:** [winforms index](index.md) · [index](../../index.md) · [CLAUDE](../../claude.md) · [AGENTS](../../definitions/agents.md) · [brain](../../.decisions/brain.md) · [glossary](../../definitions/glossary.md) · [keys](../../governance/keys.md)
**Kardeşler (winforms · 4/32):** [accessibility-tab-and-mnemonics](accessibility-tab-and-mnemonics.md) · [anchor-dock-layout-rules](anchor-dock-layout-rules.md) · [async-await-ui-thread](async-await-ui-thread.md) · [async-loading-patterns](async-loading-patterns.md) · [async-ui-thread-marshalling](async-ui-thread-marshalling.md) · [backgroundworker-vs-task](backgroundworker-vs-task.md) · [cancellation-in-ui](cancellation-in-ui.md) · [cancellation-token-in-ui](cancellation-token-in-ui.md) · [composition-root-host-startup](composition-root-host-startup.md) · [control-disposal-and-memory](control-disposal-and-memory.md) · [controls-lifecycle-events](controls-lifecycle-events.md) · [data-binding-basics](data-binding-basics.md) · [designer-cs-anatomy](designer-cs-anatomy.md) · [designer-editing-rules](designer-editing-rules.md) · [designer-hand-edit-policy](designer-hand-edit-policy.md) · [di-container-lifetimes](di-container-lifetimes.md) · [docking-panels-patterns](docking-panels-patterns.md) · [errorprovider-validation-flow](errorprovider-validation-flow.md) · [exception-boundary-ui](exception-boundary-ui.md) · [form-close-and-dispose](form-close-and-dispose.md) · [form-close-cancel-pattern](form-close-cancel-pattern.md) · [layout-table-panel](layout-table-panel.md) · [logging-integration](logging-integration.md) · [partial-class-and-file-naming](partial-class-and-file-naming.md) · [settings-user-config](settings-user-config.md) · [splitcontainer-and-sizing](splitcontainer-and-sizing.md) · [validation-form-level](validation-form-level.md) · [winforms-rendering-performance](winforms-rendering-performance.md)

> **Tek cumlelik sorumluluk:** *`Load`, `Shown` ve `Activated` arasindaki fark nedir ve bir is hangisine konur* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

`Load` **gorunmeden hemen once** ve **her** `Show` / `ShowDialog`'ta tetiklenir; `Shown` yalnizca pencere **ilk kez** gorunur oldugunda; `Activated` **her odak** kazanisinda. Veri yukleme `Load`'a, ilk-kez isleri `Shown`'a, odak bagli tazelemeler `Activated`'e konur. Hepsinde de is mantigi **uygulama katmanina** cagrilir, kod-behind yalnizca yonlendirir.

```text
+----------------------------------------------------------------+
| Load     : her Show / ShowDialog - veri yukleme (async burada)  |
| Shown    : form omrunde 1 KEZ - odak, animasyon, ilk ekran     |
| Activated: her odak - tazeleme tetigi (agir is YASAK)          |
| Deactivate: odak kaybi - zamanlayici durdur, taslak kaydet     |
+----------------------------------------------------------------+
```

---

## §1 Karsilastirma tablosu

| # | Olay | Kac kez tetiklenir | Zaman | Kullanim |
|---|---|---|---|---|
| 1 | `Load` | her `Show` / `ShowDialog` | handle hazir, pencere gorunmeden once | veri yukleme, `async` doldurma |
| 2 | `Shown` | form omrunde **1 kez** | ilk kez gorunur | ilk odak, giris animasyonu, "bir daha sorma" kutusu |
| 3 | `Activated` | her odak kazanisi | pencere gorunurken | listeyi tazele, zamanlayici baslat |
| 4 | `Deactivate` | odak kaybi | pencere gorunurken | zamanlayici durdur, taslak kaydet |

---

## §2 Baglama bicimi (`.Designer.cs` icinde)

Olay baglama `InitializeComponent` icindedir; `.Form.cs`'e `this.Load += ...` **yazilmaz**:

```csharp
// SettingsForm.Designer.cs - tek InitializeComponent kaynagi
this.Load += new System.EventHandler(this.SettingsForm_Load);
this.Shown += new System.EventHandler(this.SettingsForm_Shown);
this.Activated += new System.EventHandler(this.SettingsForm_Activated);
```

```csharp
// SettingsForm.cs - yalniz olay yonlendiriciler
private async void SettingsForm_Load(object sender, EventArgs e)
{
    await _mediator.SendAsync(new LoadSettingsQuery(), CancellationToken.None);
}

private void SettingsForm_Shown(object sender, EventArgs e)
{
    if (_firstShown) return;      // Shown yalniz 1 kez gelir; yine de koruyucu yaz
    _firstShown = true;
    gridSettings.Focus();
}

private void SettingsForm_Activated(object sender, EventArgs e)
{
    _clock.Start();               // odak geldi -> canli tazeleme
}
```

---

## §3 Sik kirilan davranislar

| # | Belirti | Kok neden | Duzeltme |
|---|---|---|---|
| 1 | Ikinci acilista eski veri | doldurma `Load`'ta, form yeniden kullanimda | doldurmayi `Load`'tan `public async Task ReloadAsync()`'a tasi |
| 2 | `Shown` iki kez calisiyor | form gizlenip yeniden gosteriliyor (`Hide`+`Show`) | korucuk bayrak + olay `.Designer.cs`'ten kaldirma |
| 3 | `Load` icinde `MessageBox` dongusu | olay yeniden girise izin veriyor | `Shown`'a tasi ya da bayrak koy |
| 4 | Form gorunmeden olcu hatasi | `Load`'ta `Width` okunuyor | `Shown`'a tasi |
| 5 | `async void Load` istisnasi uygulamayi dusuruyor | yakalama yok | `try/catch` + global yakalayici |

---

## §4 `async void Load` kalibi

```csharp
private async void SettingsForm_Load(object sender, EventArgs e)
{
    try
    {
        _loading.Visible = true;
        await _settingsService.RefreshAsync();
    }
    catch (Exception ex)
    {
        _errorReporter.Report(ex);          // hatayi yutma
        throw;                              // global yakalayici da gorsun
    }
    finally
    {
        _loading.Visible = false;
    }
}
```

> **KURAL:** `async void` **yalniz** olay isleyicisinde ve **yalniz** tam kapsamli `try/catch/finally` ile kullanilir; kutuphane metodu asla `async void` olmaz.

---

## §5 Tarama tablosu (kim neyi degistirir)

| Aranacak | Beklenen | Kim |
|---|---|---|
| `.Form.cs` icinde `this.Load +=` | 0 eslesme (baglama `.Designer.cs`'te) | `developer` |
| `async void ..._Load` | govdede tam `try/catch/finally` | `developer` |
| `Shown` govdesinde `if (_firstShown)` | korucuk bayrak var | `developer` |
| `Activated` govdesinde agir is / sorgu | 0 eslesme | `reviewer` |
| DPI varsayilanlari `#if` disinda is mantigi | `#if` yalniz platform farki | `developer` |

`Shown` / `Activated` davranisi her iki bacakta aynidir; yalnizca DPI varsayilanlari farklidir (`net48` -> `app.config`, `net10.0-windows` -> `ApplicationConfiguration.Initialize()`). Bu fark **platform farkidir**, `#if` ile izole edilir ve is mantigina girmez.

---

## §6 Kontrol listesi

| # | Adim | Kanit |
|---|---|---|
| 1 | Olay `+=` satirlari yalniz `.Designer.cs` icinde | grep hedefi: `.Form.cs` = 0 eslesme |
| 2 | `async void` isleyicide tam `try/catch/finally` var | kod incelemesi |
| 3 | Yeniden kullanilan formda doldurma `Load`'tan tasindi | davranis testi |
| 4 | Hata yolu test edildi (ag yok senaryosu) | test ciktisi |
| 5 | Iki bacakta `dotnet build` -> 0 hata | build ciktisi DOĞRULAMA GEREKLI |

---

## §9 Kanit

| Kaynak dosya | Ne alindi |
|---|---|
| `knowledge\winforms\form-load-and-shown-events.md` (kaynak vault) | kac kez tetiklenen olay tablosu, Designer baglama ornegi, 5 kirilan davranis, async Load kalibi, iki bacak notu |
| `.rules\ui-rules.md` | olay yonlendiricisi ve kod-behind siniri |
| `checklists\winforms-designer-check.md` | olay baglari yerinin denetimi (duz metin, link degil) |

> Kanitsiz iddia icin `DOĞRULAMA GEREKLI` yazilir; bu dosyada kaynak disi bilgi yoktur.
