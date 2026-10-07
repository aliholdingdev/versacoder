---
title: "form-showdialog-result - VersaCoder"
type: knowledge
category: winforms
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "Modal (ShowDialog) vs modeless (Show) seçimi, DialogResult akışı, çağıran formun durumu ve Dispose kuralı"
---

# form-showdialog-result — Modal Gösterim ve DialogResult

**Zorunlu Baglantilar:** [vault index](../../index.md) · [coding-standards](../../.rules/coding-standards.md) · [winforms index](../index.md) · [form-close-cancel-pattern](form-close-cancel-pattern.md) · [di-form-factory-navigation](di-form-factory-navigation.md)

> **Tek cümlelik sorumluluk:** *Bir form `Show` ile mi `ShowDialog` ile mi açılır, dönen `DialogResult` çağıran kodda nasıl işlenir ve form ne zaman `Dispose` edilir* sorusunun tek cevabıdır.

---

## §0 Ozet (Answer First)

Kullanıcı bir seçim **bitirmeden** ana pencereye dönülmeyecekse → `ShowDialog()` (modal). Arka planda açık kalıp ana pencereyle etkileşim sürecekse → `Show()` (modeless). Modal form **`using` ile** açılır; kapanınca `Dispose` çağırır, aksi halde olay abonelikleri ve handle'lar bekler. `DialogResult`'u `OK`/`Yes` dalında **sadece form kendi içinde** atar; çağıran taraf sonucu okur, formun içine karışmaz. `ShowDialog` sahipsiz çağrılırsa sahipli (`IWin32Window owner`) çağırılmalıdır — sahipsiz modal, çağıranı modal yapmaz ve odak kayması doğurur.

---

## §1 Show vs ShowDialog

| Kriter | `Show()` | `ShowDialog()` |
|---|---|---|
| Çağıranla etkileşim | Devam eder (modeless) | **Kilitlenir** (modal) |
| Dönüş değeri | yok (`void`) | `DialogResult` |
| Kapanış zorunluluğu | Uygulama kapanışına kadar açık kalabilir | Kapanana kadar bekler |
| `Dispose` | Çağıran yapar | `using` bloğu yapar |
| Tipik kullanım | Yardım/panel, çoklu pencere | Onay, ayar, dosya seçimi |
| Sahiplik | `owner` opsiyonel | **`owner` ver (Modal)`** |

```csharp
// GOOD — modal + owner + using + sonucun okunmasi
using (var dlg = _forms.Create<SeriesEditForm>())
{
    dlg.Owner = this;
    if (dlg.ShowDialog(this) != DialogResult.OK)
        return;                                  // iptal: arayuze dokunma

    await _series.SaveAsync(dlg.Model, ct);      // veri islemi serviste, form disinda
}
```

---

## §2 DialogResult tablosu

| `DialogResult` | Klasik tetikleyici | Çağıran kod davranışı |
|---|---|---|
| `OK` | `Tamam` düğmesi (`IButtonControl.PerformClick`) | Modeli al, kaydet |
| `Cancel` | `İptal`, ESC (`CancelButton`), pencere X | Hiçbir şey yapma, sessizce çık |
| `Yes` / `No` | Onay diyaloğu | Dal kur |
| `Abort` / `Retry` / `Ignore` | Hata diyaloğu | Yeniden dene / geç |
| **`None`** | `Show()` ile açılan form | Karşılaştırma yapılmaz |

> **KURAL:** `dlg.DialogResult = DialogResult.OK` atandığı anda modal form **kapanır**. Kapanışı geciktirmek istiyorsan `FormClosing` iptalini kullan ([form-close-cancel-pattern](form-close-cancel-pattern.md)).

---

## §3 Sik hatalar

| # | Hata | Belirti | Düzeltme |
|---|---|---|---|
| 1 | `new Form().ShowDialog()` sahipsiz | Ana pencere altta kalır, odak kayar | `ShowDialog(this)` |
| 2 | Formu alan property'de tutup `Dispose` etmememek | Olay sızıntısı, handle birikimi | `using` ya da `Close()` sonrası `Dispose()` |
| 3 | Sonucu `Text`/tag ile taşımak | Kırılgan, tip güvenli değil | `DialogResult` + public `Model` |
| 4 | Modeless formu tek örnek sanıp `new`'lemek | Aynı formdan 5 tane açık | [form-single-instance-reuse](form-single-instance-reuse.md) |
| 5 | Modal formda `await`'siz senkron veri sorgusu | UI donması | `Load` içinde `async` ([async-void-event-handlers](async-void-event-handlers.md)) |

---

## §4 net48 / net10.0-windows farki

| Konu | `net48` | `net10.0-windows` |
|---|---|---|
| `ShowDialog` / `DialogResult` davranışı | **aynı** | aynı |
| `Form.StartPosition` varsayılanı | `WindowsDefaultLocation` | aynı |
| DevExpress formları (`RibbonForm`, dialog'lar) | v26.1 | v26.1 — kalıp aynı (ADR-004) |
| `using` deyimi | blok biçimi `using (...) { }` (C# 7.3) | satır içi `using var` **paylaşılan dosyada yazılmaz** |

---

## §5 Kardesler

[form-constructor-vs-load](form-constructor-vs-load.md) · [form-close-cancel-pattern](form-close-cancel-pattern.md) · [form-single-instance-reuse](form-single-instance-reuse.md) · [di-form-factory-navigation](di-form-factory-navigation.md)
