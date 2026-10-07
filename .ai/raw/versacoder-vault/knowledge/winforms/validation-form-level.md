---
title: "Form Seviyesi Dogrulama - VersaCoder"
type: knowledge
category: winforms
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "WinForms form seviyesi dogrulama: Validating/Validated, AutoValidate, ValidateChildren, ErrorProvider, kaydet onayi; net48 vs net10.0-windows farklari"
---

# Form Seviyesi Dogrulama (Validation)

**Zorunlu Baglantilar:** [knowledge index](../index.md) · [CLAUDE.md](../../CLAUDE.md) · [controls-lifecycle-events](controls-lifecycle-events.md) · [data-binding-basics](data-binding-basics.md) · [testing-rules](../../.rules/testing-rules.md)

> **Tek cumlelik sorumluluk:** *Hatali alan nasil isaretlenir, odak neden cikamaz, kaydet butonu neyi kontrol eder* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Dogrulama **uc katmanli** calisir: (1) alan bazli `Validating` (`e.Cancel = true` odagi hapsediyor → odak cikamiyor), (2) `ErrorProvider` gorsel isaret (metin + ikon), (3) **kaydet butonu oncesi** `ValidateChildren(ValidationSelectors...)` ile tum zorunlu alanlarin toplu kontrolu. `ContainerControl.AutoValidate` `EnablePreventFocusChange` iken `Validating` iptali odagi tutar; `AutoValidate.Disable` ise olaylari tamamen susturur - formsal hata mesaji yerine **once kural yaz**. Kurallar `test` ile kapatilir (bkz. [testing-rules](../../.rules/testing-rules.md)); "guzel gorunuyor" kanit degildir (G5).

---

## §1 Dogrulama zinciri

| # | Katman | API | Ne yapar |
|---|---|---|---|
| 1 | Alan | `Control.Validating` | `e.Cancel` → odak cikamaz |
| 2 | Alan | `Control.Validated` | odak cikti, alan gecerli |
| 3 | Form | `ContainerControl.AutoValidate` | `EnablePreventFocusChange` / `Disable` |
| 4 | Form | `ValidateChildren()` | tum zorunlu alanlari toplu dogrular |
| 5 | Gorsel | `ErrorProvider.SetError` | ikon + tooltip metni |
| 6 | Kaydet | buton `Click` | toplu dogrulama + `Application` katmanina gecis |

```
[odak cikmaya calisir]
        |
        v
  nameBox.Validating --(e.Cancel=true)--> odak KALIR + ErrorProvider ikon
        |
        v (gecerli)
  nameBox.Validated
        |
        v
  [Kaydet] --> ValidateChildren() --> hepsi gecerli? --> Application katmani (Dapper, @p1)
                                    |
                                    hayir --> ilk hataliya odak + ozet
```

---

## §2 Kalip

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
    _application.Save(nameBox.Text);   // ADO.NET + Dapper, parameterized
}
```

| Parca | Kural |
|---|---|
| `ValidateChildren(...Visible)` | gizli alanlar zorunlu tutulmaz |
| `SetError(control, "")` | hatayi temizlemeyi unutma (kalıcı ikon) |
| Metin `.resx`'ten | kural mesaji kod icinde string literal degil (CS-07) |
| Kaydet | dogrulama **sonrasi**, `Application` katmanina tek cagri |
| SQL | parameterized `@p1` + `CommandTimeout` (ADR-005, DA-03) |

---

## §3 TFM farki (net48 vs net10.0-windows)

| Konu | `net48` | `net10.0-windows` |
|---|---|---|
| `Validating` / `Validated` | var | var |
| `ContainerControl.AutoValidate` | var | var |
| `ValidateChildren(ValidationSelectors)` | var | var |
| `ErrorProvider` | var | var |
| `INotifyDataErrorInfo` | 4.5+ var | var (baglama ile `ErrorProvider` baglasa da) |
| Nullable anotasyonu | yok | `object? sender` |

> **Sonuc:** dogrulama API'si **iki bacakta aynidir**; `#if` gerekmez. Yalnizca `sender` anotasyonu ve dil yazimi farklidir (§4).

---

## §4 C# 7.3 (net48) vs C# 14

```csharp
// --- C# 7.3 (net48) ---
private void nameBox_Validating(object sender, CancelEventArgs e)
{
    string value = nameBox.Text;
    bool invalid = value == null || value.Trim().Length == 0;

    e.Cancel = invalid;
    if (invalid)
    {
        errorProvider.SetError(nameBox, MainFormResources.RequiredText);
    }
    else
    {
        errorProvider.SetError(nameBox, "");
    }
}
```

```csharp
// --- C# 14 (net10.0-windows) ---
private void nameBox_Validating(object? sender, CancelEventArgs e)
{
    string value = nameBox.Text ?? string.Empty;
    bool invalid = value.Trim().Length == 0;

    e.Cancel = invalid;
    errorProvider.SetError(nameBox, invalid ? MainFormResources.RequiredText : string.Empty);
}
```

| Ozellik | C# 7.3 | C# 14 | Ortak kaynak |
|---|---|---|---|
| `sender` tipi | `object` | `object?` | 7.3'te `object` |
| Kosullu metin | `if/else` | ternary | ternary 7.3'te de var |
| Null kontrol | `== null \|\|` | `??` + `Trim()` | `??` 6.0+ -> ortak kodda da var |
| `switch` expression | statement | expression | ortak kodda statement (8.0+) |

---

## §5 Ihlal belirtisi

| Ihlal | Belirti | Duzeltme |
|---|---|---|
| `AutoValidate = Disable` + hic kontrol yok | hatali veri veritabanina gidiyor | kaydet oncesi `ValidateChildren()` |
| `SetError` temizlenmiyor | gecmis hata ikonu duruyor | her `Validating` sonunda `SetError(..., "")` |
| Dogrulama yalniz `Click`'te | odakla cikis hatali veri birakir | alan `Validating` + toplu `ValidateChildren` |
| Mesaj kodda string literal | ceviri/tekrar imkansiz (CS-07) | `.resx`'e tasi (bkz. [resx-resourcemanager](resx-resourcemanager.md)) |
| `catch { }` ile kaydet hatasi gizleme | CS-05, sessiz veri kaybi | tipine gore yakala, goster |

> **Iliskili:** odak sirasi icin [controls-lifecycle-events](controls-lifecycle-events.md) · baglama ile kullanim icin [data-binding-basics](data-binding-basics.md) · kaynak adlari icin [resx-resourcemanager](resx-resourcemanager.md).
