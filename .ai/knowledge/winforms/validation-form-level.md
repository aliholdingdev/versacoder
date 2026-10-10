---
title: "validation-form-level - VersaCoder"
type: knowledge
category: winforms
status: active
version: 1.0.0
updated: 2026-09-30
authority: primary
scope: "Form seviyesi dogrulama zincirini (Validating, AutoValidate, ValidateChildren, ErrorProvider, kaydet onayi) ve iki bacak farklarini tanimlar"
tags: [knowledge]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# Form Seviyesi Dogrulama (Validation)

**Zorunlu Baglantilar:** [[knowledge/winforms/errorprovider-validation-flow|errorprovider-validation-flow]] · [[knowledge/winforms/data-binding-basics|data-binding-basics]] · [[.rules/testing-rules|testing-rules]] · [[.rules/error-handling-rules|error-handling-rules]] · [[.rules/coding-standards|coding-standards]]
**Küme + Hub:** [[knowledge/index|knowledge index]] · [[knowledge/winforms/index|winforms index]] · [[index|index]] · [[claude|CLAUDE]] · [[definitions/agents|AGENTS]] · [[.decisions/brain|brain]] · [[definitions/glossary|glossary]] · [[governance/keys|keys]]
**Kardeşler (winforms · 4/32):** [[knowledge/winforms/accessibility-tab-and-mnemonics|accessibility-tab-and-mnemonics]] · [[knowledge/winforms/anchor-dock-layout-rules|anchor-dock-layout-rules]] · [[knowledge/winforms/async-await-ui-thread|async-await-ui-thread]] · [[knowledge/winforms/async-loading-patterns|async-loading-patterns]] · [[knowledge/winforms/async-ui-thread-marshalling|async-ui-thread-marshalling]] · [[knowledge/winforms/async-void-event-handlers|async-void-event-handlers]] · [[knowledge/winforms/backgroundworker-vs-task|backgroundworker-vs-task]] · [[knowledge/winforms/cancellation-in-ui|cancellation-in-ui]] · [[knowledge/winforms/cancellation-token-in-ui|cancellation-token-in-ui]] · [[knowledge/winforms/composition-root-host-startup|composition-root-host-startup]] · [[knowledge/winforms/control-disposal-and-memory|control-disposal-and-memory]] · [[knowledge/winforms/controls-lifecycle-events|controls-lifecycle-events]] · [[knowledge/winforms/designer-cs-anatomy|designer-cs-anatomy]] · [[knowledge/winforms/designer-editing-rules|designer-editing-rules]] · [[knowledge/winforms/designer-hand-edit-policy|designer-hand-edit-policy]] · [[knowledge/winforms/di-container-lifetimes|di-container-lifetimes]] · [[knowledge/winforms/docking-panels-patterns|docking-panels-patterns]] · [[knowledge/winforms/exception-boundary-ui|exception-boundary-ui]] · [[knowledge/winforms/form-close-and-dispose|form-close-and-dispose]] · [[knowledge/winforms/form-close-cancel-pattern|form-close-cancel-pattern]] · [[knowledge/winforms/form-constructor-vs-load|form-constructor-vs-load]] · [[knowledge/winforms/form-load-and-shown-events|form-load-and-shown-events]] · [[knowledge/winforms/layout-table-panel|layout-table-panel]] · [[knowledge/winforms/logging-integration|logging-integration]] · [[knowledge/winforms/partial-class-and-file-naming|partial-class-and-file-naming]] · [[knowledge/winforms/settings-user-config|settings-user-config]] · [[knowledge/winforms/splitcontainer-and-sizing|splitcontainer-and-sizing]] · [[knowledge/winforms/winforms-rendering-performance|winforms-rendering-performance]]

> **Tek cumlelik sorumluluk:** *Hatali alan nasil isaretlenir, odak neden cikamaz, kaydet butonu neyi kontrol eder* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Dogrulama **uc katmanli** calisir: (1) alan bazli `Validating` (`e.Cancel = true` odagi hapsediyor -> odak cikamiyor), (2) `ErrorProvider` gorsel isaret (metin + ikon), (3) **kaydet butonu oncesi** `ValidateChildren(...)` ile tum zorunlu alanlarin toplu kontrolu. `ContainerControl.AutoValidate` `EnablePreventFocusChange` iken `Validating` iptali odagi tutar; `AutoValidate.Disable` ise olaylari tamamen susturur - formsal hata mesaji yerine **once kural yaz**. Kurallar `test` ile kapatilir; "guzel gorunuyor" kanit degildir (G5).

```text
[odak cikmaya calisir]
        |
        v
  nameBox.Validating --(e.Cancel=true)--> odak KALIR + ErrorProvider ikon
        |
        v (gecerli)
  nameBox.Validated
        |
        v
  [Kaydet] --> ValidateChildren() --> hepsi gecerli? --> Application katmani
                                    |
                                    hayir --> ilk hataliya odak + ozet
```

---

## §1 Dogrulama zinciri

| # | Katman | API | Ne yapar |
|---|---|---|---|
| 1 | alan | `Control.Validating` | `e.Cancel` -> odak cikamaz |
| 2 | alan | `Control.Validated` | odak cikti, alan gecerli |
| 3 | form | `ContainerControl.AutoValidate` | `EnablePreventFocusChange` / `Disable` |
| 4 | form | `ValidateChildren()` | tum zorunlu alanlari toplu dogrular |
| 5 | gorsel | `ErrorProvider.SetError` | ikon + tooltip metni |
| 6 | kaydet | buton `Click` | toplu dogrulama + Application katmanina gecis |

---

## §2 Kalip ve parca kurallari

```csharp
private void nameBox_Validating(object sender, System.ComponentModel.CancelEventArgs e)
{
    string text = nameBox.Text == null ? string.Empty : nameBox.Text.Trim();
    if (text.Length == 0)
    {
        e.Cancel = true;
        errorProvider.SetError(nameBox, "Ad bos olamaz");
        return;
    }
    e.Cancel = false;
    errorProvider.SetError(nameBox, string.Empty);
}

private void saveButton_Click(object sender, EventArgs e)
{
    if (!ValidateChildren(ValidationSelectors.Visible))
    {
        statusLabel.Text = "Dogrulama hatasi var";
        return;
    }
    _application.Save(nameBox.Text);   // ADO.NET + Dapper, parametreli
}
```

| Parca | Kural |
|---|---|
| `ValidateChildren(...Visible)` | gizli alanlar zorunlu tutulmaz |
| `SetError(control, "")` | hatayi temizlemeyi unutma (kalici ikon) |
| metin `.resx`'ten | kural mesaji kod icinde string literal degil (CS-07) |
| kaydet | dogrulama **sonrasi**, Application katmanina tek cagri |
| SQL | parametreli `@p1` + `CommandTimeout` (ADR-005, DA-03) |

---

## §3 TFM farki (net48 vs net10.0-windows)

| Konu | `net48` | `net10.0-windows` |
|---|---|---|
| `Validating` / `Validated` | var | var |
| `ContainerControl.AutoValidate` | var | var |
| `ValidateChildren(ValidationSelectors)` | var | var |
| `ErrorProvider` | var | var |
| `INotifyDataErrorInfo` | 4.5+ var | var (baglama ile `ErrorProvider` baglasa da) |
| nullable anotasyonu | yok | `object? sender` |

> **Sonuc:** dogrulama **API'si iki bacakta aynidir**; `#if` gerekmez. Yalnizca `sender` anotasyonu ve dil yazimi farklidir.

---

## §4 C# 7.3 (net48) vs C# 14

| Ozellik | C# 7.3 | C# 14 | Ortak kaynak |
|---|---|---|---|
| `sender` tipi | `object` | `object?` | 7.3'te `object` |
| kosullu metin | `if/else` | ternary | ternary 7.3'te de var |
| null kontrol | `== null \|\|` | `??` + `Trim()` | `??` 6.0+ -> ortak kodda da var |
| `switch` ifadesi | statement | expression | ortak kodda statement (8.0+) |

Ortak govde: gecerlilik hesapla -> `e.Cancel = invalid` -> `SetError` (bos metin = temizle). Hata metni `.resx` kaynagindan okunur (bkz. [[knowledge/winforms/designer-cs-trio-rule|designer-cs-trio-rule]] - resx sorumlulugu).

---

## §5 Ihlal belirtisi

| Ihlal | Belirti | Duzeltme |
|---|---|---|
| `AutoValidate = Disable` + hic kontrol yok | hatali veri veritabanina gidiyor | kaydet oncesi `ValidateChildren()` |
| `SetError` temizlenmiyor | gecmis hata ikonu duruyor | her `Validating` sonunda `SetError(..., "")` |
| dogrulama yalniz `Click`'te | odakle cikis hatali veri birakir | alan `Validating` + toplu `ValidateChildren` |
| mesaj kodda string literal | ceviri/tekrar imkansiz (CS-07) | `.resx`'e tasi |
| `catch { }` ile kaydet hatasi gizleme | CS-05, sessiz veri kaybi | tipine gore yakala, goster |

---

## §6 Kim neyi degistirir

| Kim | Neyi degistirir | Dosya |
|---|---|---|
| `ui-designer` | `ErrorProvider` kurulumu, `AutoValidate` tasarim degeri | `.Designer.cs` |
| `developer` | `Validating`/`Validated` govdeleri, `ValidateChildren` cagrisi | `.Form.cs` |
| `developer` | is kurali (Application katmani) | servis |
| `tester` | kural testleri | test projesi |

Tarama: `.Form.cs`'te `new ErrorProvider` **0** DOĞRULAMA GEREKLI; her `Validating` icinde `e.Cancel` var; kaydet tiklamasi `ValidateChildren` iceriyor; iki bacakta `dotnet build` -> 0 hata DOĞRULAMA GEREKLI.

---

## §9 Kanit

| Kaynak dosya | Ne alindi |
|---|---|
| `knowledge\winforms\validation-form-level.md` (kaynak vault) | 6 katmanli zincir, akis semasi, kalip kodu, parca kurallari, TFM + C# karsilastirmasi, ihlal belirtileri |
| `.rules\testing-rules.md` | kuralin test ile kapanmasi |
| `.rules\error-handling-rules.md` | yakalama ve gosterme kurali |

> Kanitsiz iddia icin DOĞRULAMA GEREKLI yazilir; bu dosyada kaynak disi bilgi yoktur.
