---
title: "anchor-dock-layout-rules - VersaCoder"
type: knowledge
category: winforms
status: active
version: 1.0.0
updated: 2026-09-29
authority: SSOT
scope: "Pencere boyutlandiginda kontrollerin davranisi: Anchor/Dock farki, AutoSize tuzagi, Resize olayinda yerlestirme yazmama kurali"
---

# Anchor ve Dock ile Layout Kurallari

**Zorunlu Baglantilar:** [vault indeks](../index.md) · [ui-rules](../../.rules/ui-rules.md) · [accessibility-rules](../../.rules/accessibility-rules.md) · [designer-cs-trio-rule](designer-cs-trio-rule.md) · [winforms-rendering-performance](winforms-rendering-performance.md)
**Küme + Hub:** [winforms index](index.md) · [index](../../index.md) · [CLAUDE](../../CLAUDE.md) · [AGENTS](../../AGENTS.md) · [brain](../../brain.md) · [glossary](../../glossary.md) · [keys](../../keys.md)
**Kardeşler (winforms · 3/32):** [accessibility-tab-and-mnemonics](accessibility-tab-and-mnemonics.md) · [async-await-ui-thread](async-await-ui-thread.md) · [async-loading-patterns](async-loading-patterns.md) · [async-ui-thread-marshalling](async-ui-thread-marshalling.md) · [async-void-event-handlers](async-void-event-handlers.md) · [backgroundworker-vs-task](backgroundworker-vs-task.md) · [cancellation-in-ui](cancellation-in-ui.md) · [cancellation-token-in-ui](cancellation-token-in-ui.md) · [composition-root-host-startup](composition-root-host-startup.md) · [control-disposal-and-memory](control-disposal-and-memory.md) · [controls-lifecycle-events](controls-lifecycle-events.md) · [data-binding-basics](data-binding-basics.md) · [designer-cs-anatomy](designer-cs-anatomy.md) · [designer-editing-rules](designer-editing-rules.md) · [designer-hand-edit-policy](designer-hand-edit-policy.md) · [di-container-lifetimes](di-container-lifetimes.md) · [docking-panels-patterns](docking-panels-patterns.md) · [errorprovider-validation-flow](errorprovider-validation-flow.md) · [exception-boundary-ui](exception-boundary-ui.md) · [form-close-and-dispose](form-close-and-dispose.md) · [form-close-cancel-pattern](form-close-cancel-pattern.md) · [form-constructor-vs-load](form-constructor-vs-load.md) · [form-load-and-shown-events](form-load-and-shown-events.md) · [layout-table-panel](layout-table-panel.md) · [logging-integration](logging-integration.md) · [partial-class-and-file-naming](partial-class-and-file-naming.md) · [settings-user-config](settings-user-config.md) · [splitcontainer-and-sizing](splitcontainer-and-sizing.md) · [validation-form-level](validation-form-level.md)

> **Tek cumlelik sorumluluk:** *Pencere boyutu degistiginde kontroller nasil yerlesir* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Boyut degisimini **yalniz `Anchor` ve `Dock` yonetir**; `Resize` olayinda elle `Location`/`Size` **yazilmaz** - o kod `.Form.cs`'e tasarim kodu sokar ve DPI'da bozulur. Yatay serit `Dock = Top/Bottom`, tum alani kaplayan panel `Dock = Fill`, sabit boyutlu etiket `Anchor = Top, Left`, genisleyen grid `Anchor = Top, Bottom, Left, Right` olur. Bu atamalar **yalniz `.Designer.cs`** icindedir (bkz. [designer-cs-trio-rule](designer-cs-trio-rule.md)).

---

## §1 Araclar

| # | Arac | Ne yapar | Ne zaman |
|---|---|---|---|
| 1 | `Anchor` | kaboxa gore **bagil** konum/boyut korur | grid/panel gibi serbest ogeler |
| 2 | `Dock` | kenara **yapisir**, sira ile yigilir | arac cubugu, alt durum, icerik alani |
| 3 | `AutoSize` | icerik kadar yer kaplar | buton/etiket |
| 4 | `TableLayoutPanel` | hucre hizasi | form alanlari |
| 5 | `SplitContainer` | kullanici boler | iki panelli ekran |

---

## §2 Karar tablosu

| Ogge | `Anchor` | `Dock` | Not |
|---|---|---|---|
| Alt durum cubugu | - | `Bottom` | sira ile |
| Ust ribbon / arac seridi | - | `Top` | `RibbonForm`'da ribbon zaten dock'ludur |
| Sol gezinme paneli | - | `Left` | `SplitContainer` ile degisken |
| Merkez grid | `Top, Bottom, Left, Right` | `Fill` | ikisi de olur; `Fill` tercih |
| Form uzerindeki buton | `Top, Right` | - | sag ustte sabit |
| Metin kutusu (tam genislik) | `Top, Left, Right` | - | |
| Etiket (sabit) | `Top, Left` | - | `AutoSize = true` |

```csharp
// XxxForm.Designer.cs - layout ayarlari tasarim kodudur
this.grid.Dock = System.Windows.Forms.DockStyle.Fill;
this.pnlLeft.Dock = System.Windows.Forms.DockStyle.Left;
this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)
    ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
```

---

## §3 AutoSize tuzagi

| # | Belirti | Neden | Duzeltme |
|---|---|---|---|
| 1 | Buton metni degisince arkadaki kutu kirpiliyor | `AutoSize` sadece butonda | kapsayici `TableLayoutPanel` |
| 2 | Grid buyuyunce buton alta kaciyor | `Anchor` eksik | `Top, Right` ata |
| 3 | Pencere kuculunce grid kayboluyor | `Anchor` yalniz `Top, Left` | `Fill` kullan |
| 4 | Elle `Resize` ile yerlesim | DPI'da bozulur | `Anchor`/`Dock`'a tasi |
| 5 | Serit sirasi ters | dock sirasi yanlis | `Controls.Add` sirasi `.Designer.cs`'te |

`Form.Resize` yalniz **gorsel efekt / durum gecisi** icindir (ornegin tasma menusunu gizleme); yerlesim hesabi oraya yazilmaz.

---

## §4 Kontrol listesi

| # | Adim | Kanit |
|---|---|---|
| 1 | Yerlesim `Anchor`/`Dock` ile; `Resize`'ta konum kodu yok | grep `.Resize` govdeleri |
| 2 | `Anchor`/`Dock` atamalari yalniz `.Designer.cs`'te | grep = 0 `.Form.cs` |
| 3 | %100 / %150 DPI'da form acilip kontrol edildi | gorsel kanit |
| 4 | Pencere kucultme/genisletme testi yapildi | gorsel kanit |
| 5 | Iki bacakta `dotnet build` -> 0 hata | build ciktisi |

---

## §9 Kanit

| Kaynak dosya | Ne alindi |
|---|---|
| `knowledge\winforms\anchor-dock-layout-rules.md` (kaynak vault) | arac tablosu, karar tablosu, AutoSize tuzaigi, `Resize` kurali |
| `.rules\ui-rules.md` | ekran/yerlesim kurali |
| `.rules\accessibility-rules.md` | klavye/DPI erisilebilirlik beklentisi |
| `checklists\winforms-designer-check.md` | `.Designer.cs` denetim maddeleri |

> Kanitsiz iddia icin `DOĞRULAMA GEREKLI` yazilir; bu dosyada kaynak disi bilgi yoktur.
