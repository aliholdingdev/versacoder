---
title: "errorprovider-validation-flow - VersaCoder"
type: knowledge
category: winforms
status: active
version: 1.0.0
updated: 2026-09-30
authority: primary
scope: "ErrorProvider ile alan bazli dogrulamanin akisini, tek instance kuralini, hata temizleme zorunlulugunu ve uygulama katmani ile tekrar kullanimi tanimlar"
tags: [knowledge]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# ErrorProvider ile Dogrulama Akisi

**Zorunlu Baglantilar:** [validation-form-level](validation-form-level.md) · [data-binding-basics](data-binding-basics.md) · [error-handling-rules](../../.rules/error-handling-rules.md) · [ui-rules](../../.rules/ui-rules.md) · [designer-cs-trio-rule](designer-cs-trio-rule.md)
**Küme + Hub:** [knowledge index](../index.md) · [winforms index](index.md) · [index](../../index.md) · [CLAUDE](../../claude.md) · [AGENTS](../../definitions/agents.md) · [brain](../../.decisions/brain.md) · [glossary](../../definitions/glossary.md) · [keys](../../governance/keys.md)
**Kardeşler (winforms · 4/32):** [accessibility-tab-and-mnemonics](accessibility-tab-and-mnemonics.md) · [anchor-dock-layout-rules](anchor-dock-layout-rules.md) · [async-await-ui-thread](async-await-ui-thread.md) · [async-loading-patterns](async-loading-patterns.md) · [async-ui-thread-marshalling](async-ui-thread-marshalling.md) · [async-void-event-handlers](async-void-event-handlers.md) · [backgroundworker-vs-task](backgroundworker-vs-task.md) · [cancellation-in-ui](cancellation-in-ui.md) · [cancellation-token-in-ui](cancellation-token-in-ui.md) · [composition-root-host-startup](composition-root-host-startup.md) · [control-disposal-and-memory](control-disposal-and-memory.md) · [controls-lifecycle-events](controls-lifecycle-events.md) · [designer-cs-anatomy](designer-cs-anatomy.md) · [designer-editing-rules](designer-editing-rules.md) · [designer-hand-edit-policy](designer-hand-edit-policy.md) · [di-container-lifetimes](di-container-lifetimes.md) · [docking-panels-patterns](docking-panels-patterns.md) · [exception-boundary-ui](exception-boundary-ui.md) · [form-close-and-dispose](form-close-and-dispose.md) · [form-close-cancel-pattern](form-close-cancel-pattern.md) · [form-constructor-vs-load](form-constructor-vs-load.md) · [form-load-and-shown-events](form-load-and-shown-events.md) · [layout-table-panel](layout-table-panel.md) · [logging-integration](logging-integration.md) · [partial-class-and-file-naming](partial-class-and-file-naming.md) · [settings-user-config](settings-user-config.md) · [splitcontainer-and-sizing](splitcontainer-and-sizing.md) · [winforms-rendering-performance](winforms-rendering-performance.md)

> **Tek cumlelik sorumluluk:** *Bir formda ErrorProvider nerede kurulur, hata ne zaman yazilir ne zaman temizlenir ve is kurali nerede dogrulanir* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

`ErrorProvider` **bir formda tek instance** kullanilir; kurulum `.Designer.cs`'te, **hata atama/temizleme** `.Form.cs`'tedir. Dogrulama **iki katmandir**: (1) girdi bicimi alanda (`ErrorProvider` + `Validating`), (2) is kurali Application katmaninda. Kural ihlali geldiginde **once tum alan hatalari temizlenir, sonra yeni hata yazilir** - aksi halde eski hata ekranda kalir.

```text
+-----------------------------------------------------------+
| 1 _errorProvider = new ErrorProvider(this.components)     |
| 2 alan Validating  -> bicim kontrolu                      |
| 3 hata varsa e.Cancel = true -> odak alandan CIKMAZ       |
| 4 SetError(control, msg) -> kirmizi isaret + ipucu        |
| 5 temizlik: Clear() ya da SetError(control, null)         |
+-----------------------------------------------------------+
```

---

## §1 Akis adimlari

| # | Adim | Ne olur | Dosya |
|---|---|---|---|
| 1 | `_errorProvider = new ErrorProvider(this.components)` | tek instance | `.Designer.cs` |
| 2 | alan `Validating` olayi | bicim kontrolu | bag `.Designer.cs`, govde `.cs` |
| 3 | hata varsa `e.Cancel = true` | sekme ilerlemez | `.cs` |
| 4 | `_errorProvider.SetError(control, msg)` | kirmizi isaret + ipucu | `.cs` |
| 5 | temizlik | `Clear()` ya da `SetError(control, null)` | `.cs` |

```csharp
private void txtPort_Validating(object sender, System.ComponentModel.CancelEventArgs e)
{
    if (int.TryParse(txtPort.Text, out var port) is false || port is < 1 or > 65535)
    {
        _errorProvider.SetError(txtPort, "Port 1-65535 araliginda olmali.");
        e.Cancel = true;                 // odak alandan CIKMAZ
        return;
    }
    _errorProvider.SetError(txtPort, null);
}
```

Kaydet tiklamasi sirasi: `_errorProvider.Clear()` (tum hatalari temizle) -> `ValidateChildren(ValidationConstraints.Enabled)` (alan dogrulamasi) -> `try { await _settingsService.SaveAsync(); }` (is kurali) -> `ValidationException` yakalanirsa **konteyner `Control`'e** `SetError` -> `DialogResult.OK` + `Close()`. Genel hata `_errors.Report(ex)` ile sinira gider.

---

## §2 Kurulum tablosu

| # | Ozellik | Deger | Neden |
|---|---|---|---|
| 1 | `ContainerControl` | `this` | form kapaniyor da isaretler kapanir |
| 2 | `BlinkStyle` | `NeverBlink` | dikkat dagitan yanip sonme yok |
| 3 | `ContainerControl` alani | `.Designer.cs` | tasarim kodu |
| 4 | `SetError` cagrilari | `.Form.cs` | davranis |
| 5 | kaynak metin | `.resx` | cevrilebilir |

---

## §3 Yaygin hatalar

| # | Hata | Belirti | Duzeltme |
|---|---|---|---|
| 1 | her alan icin ayri `ErrorProvider` | isaretler ust üste biner | tek instance |
| 2 | `SetError` ile hata, ama `Clear()` yok | eski hata kalir | tiklamada once `Clear()` |
| 3 | `Validating`'te `e.Cancel` yok | hatali alanla gecilir | `CancelEventArgs` kullan |
| 4 | `ValidateChildren` cagrilmiyor | buton tiklaninca alanlar dogrulanmiyor | `btnSave_Click` basinda |
| 5 | is kurali hatasi alanda gosterilmiyor | kullanici nedenini goremiyor | konteyner `Control`'e `SetError` |

---

## §4 Dogrulamanin iki katmani

| Katman | Ne kontrol eder | Nerede | Ornek |
|---|---|---|---|
| alansal | bicim, aralik, zorunlu | `.Form.cs` (`Validating`) | port 1-65535 |
| is kurali | tutarlilik, cakisma | **Application** katmani | "proje adi zaten var" |

> **KURAL:** Is kurali asla yalniz UI'da dogrulanmaz; UI **hizli geri bildirim** verir, **son soz** Application katmanindadir. Aksi halde CLI ayni kurali kacirir. Detay bicimi icin [validation-form-level](validation-form-level.md).

---

## §5 Kim neyi degistirir

| Kim | Neyi degistirir | Dosya |
|---|---|---|
| `ui-designer` | `ErrorProvider` kurulumu, `ContainerControl`, `BlinkStyle` | `.Designer.cs` |
| `developer` | `Validating` govdesi, `Clear()`/`SetError` davranisi | `.Form.cs` |
| `developer` | is kurali dogrulamasi (Application katmani) | servis / use-case |
| `developer` | hata metinleri | `.resx` (metin alani) |

Uygulama kontrol listesi: formda tek `ErrorProvider` var (grep `new ErrorProvider` = 1 - DOĞRULAMA GEREKLI), kaydet tiklamasi `Clear()` + `ValidateChildren()` iceriyor, `Validating` govdeleri `e.Cancel` kullaniyor, hata metinleri `.resx`'te, iki bacakta `dotnet build` -> 0 hata DOĞRULAMA GEREKLI.

---

## §6 Tarama tablosu (ihlal bicimi)

| Aranacak | Beklenen | Ihlalde |
|---|---|---|
| `new ErrorProvider` | formda **1** tanim DOĞRULAMA GEREKLI | isaretler ust üste biner |
| `.Form.cs` icinde `new ErrorProvider` | **0** DOĞRULAMA GEREKLI | form uclusu RED |
| `SetError(` sayisi >= `Clear()` kullanimi | temizlikle birlikte | eski hata ikonu kalir |
| `catch { }` (bosluk) | **0** DOĞRULAMA GEREKLI | sessiz veri kaybi (CS-05) |
| `e.Cancel` | her `Validating` icinde | hatali alanla gecilir |

---

## §9 Kanit

| Kaynak dosya | Ne alindi |
|---|---|
| `knowledge\winforms\errorprovider-validation-flow.md` (kaynak vault) | 5 adimli akis, kalip kodu, kurulum/yaygin hata/iki katman tablolari, kontrol listesi |
| `.rules\error-handling-rules.md` | hata siniri ve raporlama kurali |
| `checklists\winforms-designer-check.md` | denetim maddeleri (duz metin, link degil) |

> Kanitsiz iddia icin DOĞRULAMA GEREKLI yazilir; bu dosyada kaynak disi bilgi yoktur.
