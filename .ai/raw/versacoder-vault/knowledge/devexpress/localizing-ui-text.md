---
title: "localizing-ui-text - VersaCoder"
type: knowledge
category: devexpress
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "DevExpress v26.1 UI metni yerellestirmesi: kultur bazli resx, Localizer kademeleri, ribbon/grid metinleri ve iki bacak farki"
---

# localizing-ui-text — UI Metni Yerelleştirme (v26.1)

**Zorunlu Baglantilar:** [index](../index.md) · [devexpress-rules](../../.rules/devexpress-rules.md) · [ADR-004](../../.decisions/accepted/ADR-004-devexpress-universal-v261-office2010.md) · [designer-runtime-customization](designer-runtime-customization.md) · [bar-status-items](bar-status-items.md)

> **Tek cumlelik sorumluluk:** *Uygulama ve DevExpress kontrol metinleri hangi katmanda, hangi kültürle çevrilir* sorusunun tek cevabıdır.

---

## §0 Ozet (Answer First)

Metin üç kaynaktan gelir ve **her biri ayrı yerde** çevrilir: (1) form metinleri → kültür bazlı `.resx` (`FormName.tr.resx`), (2) DevExpress'in kendi metinleri (menü, buton, hata) → `Localizer` sınıf kademesi, (3) çalışma zamanı metinleri (caption/sayaç) → kod + kaynak sınıfı. Kültür **uygulama başlangıcında** sabitlenir, hiçbir yerde rastgele `new CultureInfo(...)` tekrarlanmaz. Yerelleştirme yalnız UI katmanındadır; `Application` katmanı metni bilmez (SOL-05).

---

## §1 Metin Kaynaklari

| Kaynak | Ne kapsar | Nerede | Kim yazar |
|---|---|---|---|
| `FormName.resx` | varsayılan (ingilizce/özgün) metin | proje kökü | `ui-designer` (Designer) |
| `FormName.tr.resx` | Türkçe karşılık | yanındaki klasor | `ui-designer` |
| DevExpress `Localizer` | kontrol içi metin (buton, sekme, uyarı) | UI assembly | `developer` |
| Kod içi literal | dinamik caption/sayaç | `.cs` | `developer` (minimum) |

```
Thread.CurrentThread.CurrentUICulture = "tr"
        |
        +--> ResourceManager  -> FormName.tr.resx  -> form metinleri
        |
        +--> Localizer        -> DevExpress metinleri (menu/buton/hata)
        |
        +--> kod + kaynak sinifi -> dinamik metin (sayaç, durum)
```

---

## §2 Anahtar Ozellikler

| Ozellik / API | Ne is yarar | Varsayilan |
|---|---|---|
| `Localizable` (form) | `true` ise metinler `.resx`'e taşınır | `false` (Designer) |
| `Thread.CurrentThread.CurrentUICulture` | kaynak seçimini belirler | sistem dili |
| `CultureInfo("tr-TR")` | Türkçe kültür nesnesi | — |
| `Localizer` (DevExpress) | kontrol metni kademeleri | İngilizce |
| `resources.GetString("name")` | kodda yerel metin okuma | — |

---

## §3 Kod (kültür + kaynak)

```csharp
using System.Globalization;
using System.Threading;

// Program.cs - kultur TEK yerde sabitlenir (her iki bacakta ayni)
Thread.CurrentThread.CurrentCulture      = new CultureInfo("tr-TR");
Thread.CurrentThread.CurrentUICulture   = new CultureInfo("tr-TR");
CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("tr-TR"); // arka plan thread'leri

Application.Run(new MainForm());   // form .resx'i bu kulture gore yukler
```

DevExpress metinleri için `Localizer` kademeleri (aynı anda **bir** tanesi aktif olur):

| Kademe | Ne kapsar | Not |
|---|---|---|
| `BarManagerLocalizer` | ribbon/buton/tooltip | `RibbonControl` bu kademeden okur |
| `EditorsLocalizer` | editör butonları (Clear, Popup) | `BaseEdit` |
| `GridLocalizer` | grid menü/filtre metinleri | `GridView` |
| `ExceptionLocalizer` | hata/uyarı metni | `DXErrorProvider` |

> **NEEDS_VALIDATION:** v26.1'de her kademenin **tam sınıf adı ve `GetLocalizedString` imzası** derleme ile doğrulanacak; kademeler **eşzamanlı** devre dışı bırakılmaz.

---

## §4 Kurallar (en fazla 5 madde)

1. Kültür başlangıçta **tek kez** set edilir; fonksiyon içinde değiştirilmez.
2. Ekran metni **kaynak sınıfından** okunur, literal yazılmaz (tutarlılık + denetim).
3. Yerelleştirme `.Designer.cs` içinde **elle** yapılmaz; `Localizable=true` ile Designer üretilir (DX-10).
4. Mesaj metni ile **veri** ayrıdır: çevirilecek metin `.resx`, veri repository (Dapper).
5. Ribbon stili yerelleştirme ile **ilgili değildir** — o hep `Office2010` (DX-03).

---

## §5 Iki Bacak (net48 / net10.0-windows)

| Konu | net48 | net10.0-windows |
|---|---|---|
| `CultureInfo` / `ResourceManager` | aynı API | aynı API (+ `AppContext` varsayılanları) |
| Uydu assembly | `tr` klasoru bin yanında | aynı, `tr/` altında |
| DevExpress `Localizer` | v26.1 (net48 paket) | v26.1 (net10 paket) — **aynı sınıf adları** |
| Dil sürümü | **C# 7.3** — `using var` yasak | C# 14; ortak dosya 7.3 kalır |

---

## §6 Kardes Dosyalar

| Konu | Dosya |
|---|---|
| Form üçlüsü / Designer | [designer-runtime-customization](designer-runtime-customization.md) |
| Status bar metni | [bar-status-items](bar-status-items.md) |
| Ribbon yapısı | [ribbon-auto-hide-scaling](ribbon-auto-hide-scaling.md) |
| Paket/CPM (yerelleştirme paketleri) | [nuget-references-cpm](nuget-references-cpm.md) |
