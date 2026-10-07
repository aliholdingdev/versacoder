---
title: "form-single-instance-reuse - VersaCoder"
type: knowledge
category: winforms
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "Aynı ekranın tekrar tekrar new'lenmesi yerine Show/Hide örüntüsü, Application.OpenForms denetimi, sahipli (owned) form kuralları"
---

# form-single-instance-reuse — Formu Yeniden Kullanma

**Zorunlu Baglantilar:** [vault index](../../index.md) · [coding-standards](../../.rules/coding-standards.md) · [winforms index](../index.md) · [form-showdialog-result](form-showdialog-result.md) · [di-form-factory-navigation](di-form-factory-navigation.md)

> **Tek cümlelik sorumluluk:** *Aynı ekran ikinci kez açıldığında yeni form mu üretilir yoksa mevcut instance mı gösterilir; karar ve doğru kod kalıbı nedir* sorusunun tek cevabıdır.

---

## §0 Ozet (Answer First)

Aynı ekranın (ayarlar, yardım, sonuç listesi) ikinci kez açılması çoğu yerde **yeni form yerine mevcut instance'ı göstermek** demektir: `Form.Active` gibi bir işaretçi ile instance bulunur, `Visible = true` + `WindowState` düzeltilir, yoksa yeni instance üretilip **referans tutulur**. Ancak veri her açılışta tazelenmeyecekse instance ömrü de tazeleme stratejisiyle birlikte düşünülmelidir: ya `Shown`/`Activated` içinde yenile ya da kapanışta instance'ı bırak. Instance **cache'leme ile `Dispose` edilmeyen** formlar olay abonelikleri ve handle bırakır; cache yalnız **sahipli** (owned) ve **kısa ömürlü değil, uzun ömürlü tekil** ekranlarda kullanılır.

---

## §1 Karar tablosu

| Ekran tipi | Örüntü | Neden |
|---|---|---|
| Modal onay/ayar diyaloğu | Her seferinde **yeni** + `using` | Kısa ömürlü, veri taşır, Dispose şart |
| Tekil yan panel (yardım, çıktı) | **Show/Hide** + tek instance | Yeniden kurulum maliyeti yüksek |
| Liste/sonuç ekranı (veri taze) | Yeni instance **ya da** `Activated` ile yenileme | Tazelik garantisi |
| Sahipli alt pencere (`Owner = ana`) | Sahip kapanınca kendiliğinden kapanır | Zincir yaşam döngüsü |
| MDI çocuk (varsa) | `MdiParent` + tek instance çocuk başına | MDI iskeleti kuralı |

---

## §2 Tekil instance kalibi

```csharp
// OutputToolWindow.cs — kullanici mantigi; kontrol yaratimi .Designer.cs'te
private static OutputToolWindow _tek;   // UZUN omurlu tekil ekran icin

public static OutputToolWindow Goster(IWin32Window owner)
{
    if (_tek == null || _tek.IsDisposed)
    {
        _tek = new OutputToolWindow();
        _tek.Owner = owner;                       // sahipli: ana kapaninca o de kapanir
        _tek.FormClosed += delegate { _tek = null; };   // olum -> isaretci temizle
    }
    if (!_tek.Visible)  _tek.Show(owner);
    if (_tek.WindowState == FormWindowState.Minimized)
        _tek.WindowState = FormWindowState.Normal;
    _tek.BringToFront();
    return _tek;
}
```

| Satır | Kural |
|---|---|
| `IsDisposed` kontrolü | Dispose edilmiş form **asla** tekrar gösterilmez (InvalidOperationException) |
| `FormClosed += ... _tek = null` | Referans sızıntısını engeller |
| `Show(owner)` | Sahipli gösterim; sahipsiz `Show()` pencereyi kaybedebilir |
| `BringToFront()` | Z-order'da öne alır, kullanıcı "açılmadı" sanmaz |

---

## §3 Application.OpenForms ile denetim

```csharp
// Ayni ekrandan birden fazla acilmis mi? (kalin caiz)
Form mevcut = null;
for (int i = 0; i < Application.OpenForms.Count; i++)
{
    if (Application.OpenForms[i] is SettingsForm s) { mevcut = s; break; }
}
if (mevcut != null) { mevcut.Activate(); return; }
```

| Yaklaşım | Ne zaman | Not |
|---|---|---|
| Statik işaretçi (`_tek`) | Tek sorumlu ekranlar | En hızlı, en kırılgan (testte temizle) |
| `Application.OpenForms` taraması | Yığınsal açılışlar | Her açılışta O(n), n küçük kalır |
| DI factory cache'i | Uzun ömürlü servis düzeyi | Cache sahibi `IDisposable` ise Dispose zorunlu ([di-container-lifetimes](di-container-lifetimes.md)) |

---

## §4 net48 / net10.0-windows farki

| Konu | `net48` | `net10.0-windows` |
|---|---|---|
| `Application.OpenForms` | **aynı** | aynı |
| `FormClosed` → `null` atama davranışı | aynı | aynı |
| Yeni formu DI'dan üretme | `ActivatorUtilities` (paket) | aynı kalıp — bkz. [di-form-factory-navigation](di-form-factory-navigation.md) |
| Dil | C# 7.3: `delegate { }` anonim metot | Aynı kod her iki bacakta |

> **NOT:** Instance cache'i statik alan tutuyorsa testlerde temizlenmelidir; aksi halde sıralı testler birbirini kirletir — bkz. [testing-rules](../../.rules/testing-rules.md).

---

## §5 Kardesler

[form-showdialog-result](form-showdialog-result.md) · [di-form-factory-navigation](di-form-factory-navigation.md) · [control-disposal-and-memory](control-disposal-and-memory.md) · [form-close-cancel-pattern](form-close-cancel-pattern.md)
