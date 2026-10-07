---
title: "accessibility-tab-and-mnemonics - VersaCoder"
type: knowledge
category: winforms
status: active
version: 1.0.0
updated: 2026-09-29
authority: SSOT
scope: "Klavye ile tam gezinme: TabIndex/TabStop ayari, Alt-tusu mnemonikleri, okunabilir ad ve AccessibleName/Description alanlarinin kurulumu"
---

# Erisilebilirlik: Tab Sirasi, Mnemonik ve Odak

**Zorunlu Baglantilar:** [vault indeks](../index.md) · [accessibility-rules](../../.rules/accessibility-rules.md) · [ui-rules](../../.rules/ui-rules.md) · [designer-cs-trio-rule](designer-cs-trio-rule.md) · [anchor-dock-layout-rules](anchor-dock-layout-rules.md)
**Küme + Hub:** [winforms index](index.md) · [index](../../index.md) · [CLAUDE](../../claude.md) · [AGENTS](../../definitions/agents.md) · [brain](../../.decisions/brain.md) · [glossary](../../definitions/glossary.md) · [keys](../../governance/keys.md)
**Kardeşler (winforms · 3/32):** [async-await-ui-thread](async-await-ui-thread.md) · [async-loading-patterns](async-loading-patterns.md) · [async-ui-thread-marshalling](async-ui-thread-marshalling.md) · [async-void-event-handlers](async-void-event-handlers.md) · [backgroundworker-vs-task](backgroundworker-vs-task.md) · [cancellation-in-ui](cancellation-in-ui.md) · [cancellation-token-in-ui](cancellation-token-in-ui.md) · [composition-root-host-startup](composition-root-host-startup.md) · [control-disposal-and-memory](control-disposal-and-memory.md) · [controls-lifecycle-events](controls-lifecycle-events.md) · [data-binding-basics](data-binding-basics.md) · [designer-cs-anatomy](designer-cs-anatomy.md) · [designer-editing-rules](designer-editing-rules.md) · [designer-hand-edit-policy](designer-hand-edit-policy.md) · [di-container-lifetimes](di-container-lifetimes.md) · [docking-panels-patterns](docking-panels-patterns.md) · [errorprovider-validation-flow](errorprovider-validation-flow.md) · [exception-boundary-ui](exception-boundary-ui.md) · [form-close-and-dispose](form-close-and-dispose.md) · [form-close-cancel-pattern](form-close-cancel-pattern.md) · [form-constructor-vs-load](form-constructor-vs-load.md) · [form-load-and-shown-events](form-load-and-shown-events.md) · [layout-table-panel](layout-table-panel.md) · [logging-integration](logging-integration.md) · [partial-class-and-file-naming](partial-class-and-file-naming.md) · [settings-user-config](settings-user-config.md) · [splitcontainer-and-sizing](splitcontainer-and-sizing.md) · [validation-form-level](validation-form-level.md) · [winforms-rendering-performance](winforms-rendering-performance.md)

> **Tek cumlelik sorumluluk:** *Tum is akisi fare olmadan nasil yurutulur, o alanlar nereye yazilir* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Tum is akisi **fare olmadan** yurutulebilmelidir: `Tab`/`Shift+Tab` ile gezinme, `Alt+harf` ile eylem, `Enter`/`Esc` ile onay/iptal. `TabIndex`, `TabStop`, okunabilir `Text`/`AccessibleName` ve `AccessibleDescription` **`.Designer.cs`'te** tanimlanir; `.Form.cs`'e yalniz **odak davranisi** (giris yapilan ilk alana `Focus()`) ait olabilir.

```text
+----------------------------------------------------------+
| Form acilir -> ilk alan odaklanir (Shown icinde Focus)   |
|     |                                                    |
|     +--> Tab / Shift+Tab : TabIndex sirasi (tasarim kodu)|
|     +--> Alt+K           : mnemonic (&K)                 |
|     +--> Enter / Esc     : onay / iptal                  |
|     +--> hata -> hatali alana odak + SelectAll           |
+----------------------------------------------------------+
```

---

## §1 Ozellik - gorev - yer

| # | Ozellik | Ne ise yarar | Nerede |
|---|---|---|---|
| 1 | `TabIndex` | `Tab` sirasi | `.Designer.cs` |
| 2 | `TabStop` | sekme duraklamasi (grid/dahili kontrol: `false`) | `.Designer.cs` |
| 3 | `Text = "&Kaydet"` | `Alt+K` mnemonigi | `.resx` / `.Designer.cs` |
| 4 | `AccessibleName` | ekran okuyucu adi | `.Designer.cs` |
| 5 | `AccessibleDescription` | ne yaptigi | `.Designer.cs` |

---

## §2 Kurallar

| # | Kural | Neden |
|---|---|---|
| 1 | `TabIndex` soldan saga, yukaridan asagi **sifirdan** baslar | karmasik siralar sessizce bozulur |
| 2 | Ribbon ici ogeler `TabStop = false` | sekme ribbon'u atlamali, icerige gitmeli |
| 3 | `GridControl` kendi gezinmesini yonetir (`TabStop = false`) | ic ok tuslari yeterli |
| 4 | Her gorunur metin `&` icerir | `Alt` erisimi |
| 5 | Yalniz ikon butonda `AccessibleName` **zorunlu** | okuyucu "buton" der, anlam kaybolur |

```csharp
this.btnKaydet.TabIndex = 3;
this.btnKaydet.TabStop = true;
this.btnKaydet.Text = "&Kaydet";
this.btnKaydet.AccessibleName = "Kaydet";
this.btnKaydet.AccessibleDescription = "Ayarlari kaydeder ve kapatir.";
this.gridResults.TabIndex = 5;
this.gridResults.TabStop = false;      // grid ic gezinme kendi icinde
```

---

## §3 Kod-behind'daki odak davranisi

| # | Durum | Ne yapilir | `.cs`'e yazilir mi |
|---|---|---|---|
| 1 | Ilk acilista odak | `Shown` icinde `Focus()` | evet |
| 2 | Dogrulama hatasi sonrasi | hatali alana `Focus()` + `Select()` | evet |
| 3 | Mnemonic eslesmesi | otomatik | hayir (ozellik) |
| 4 | Tab sirasi | `TabIndex` | hayir (`.Designer.cs`) |
| 5 | Klavye kisayolu baglama | `Shortcuts` / `ItemClick` | hayir (`.Designer.cs`) |

**Denetim komutu (kanit uretir):** `Select-String -Path "src\**\*.Designer.cs" -Pattern "AccessibleName"` ile sayim alinir; sayi 0 ise sade ikon butonlar denetimsizdir. TabIndex tutarliligi de ayni yolla taranir (ayni konteynerde tekrar eden degerler inceleme alinir).

---

## §4 Kontrol listesi

| # | Adim | Kanit |
|---|---|---|
| 1 | Form bastan sona `Tab` ile gezilebiliyor | manuel test |
| 2 | `Alt` mnemonikleri cakismiyor | manuel test |
| 3 | Sade ikon butonlarda `AccessibleName` var | §3 denetim ciktisi |
| 4 | Hata sonrasi odak hatali alana gidiyor | davranis testi |
| 5 | Iki bacakta `dotnet build` -> 0 hata | build ciktisi |

---

## §9 Kanit

| Kaynak dosya | Ne alindi |
|---|---|
| `knowledge\winforms\accessibility-tab-and-mnemonics.md` (kaynak vault) | ozellik tablosu, 5 kural, Designer kodu, odak tablosu, denetim komutu |
| `.rules\accessibility-rules.md` | erisilebilirlik kurali |
| `checklists\accessibility-check.md` | denetim maddeleri (duz metin, link degil) |

> Kanitsiz iddia icin `DOĞRULAMA GEREKLI` yazilir; bu dosyada kaynak disi bilgi yoktur.
