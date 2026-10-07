---
title: "partial-class-and-file-naming - VersaCoder"
type: knowledge
category: winforms
status: active
version: 1.0.0
updated: 2026-09-29
authority: primary
scope: "Form dosya adi = sinif adi kurali, partial class butunlugu, tek sinif/dosya, #region yasagi ve namespace eslemesi"
tags: [knowledge]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# Dosya Adi, Sinif Adi, partial

**Zorunlu Baglantilar:** [vault indeks](../index.md) · [naming-conventions](../../.rules/naming-conventions.md) · [coding-standards](../../.rules/coding-standards.md) · [designer-cs-trio-rule](designer-cs-trio-rule.md) · [composition-root-host-startup](composition-root-host-startup.md)
**Küme + Hub:** [winforms index](index.md) · [index](../../index.md) · [CLAUDE](../../claude.md) · [AGENTS](../../definitions/agents.md) · [brain](../../.decisions/brain.md) · [glossary](../../definitions/glossary.md) · [keys](../../governance/keys.md)
**Kardeşler (winforms · 3/32):** [accessibility-tab-and-mnemonics](accessibility-tab-and-mnemonics.md) · [anchor-dock-layout-rules](anchor-dock-layout-rules.md) · [async-await-ui-thread](async-await-ui-thread.md) · [async-loading-patterns](async-loading-patterns.md) · [async-ui-thread-marshalling](async-ui-thread-marshalling.md) · [async-void-event-handlers](async-void-event-handlers.md) · [backgroundworker-vs-task](backgroundworker-vs-task.md) · [cancellation-in-ui](cancellation-in-ui.md) · [cancellation-token-in-ui](cancellation-token-in-ui.md) · [control-disposal-and-memory](control-disposal-and-memory.md) · [controls-lifecycle-events](controls-lifecycle-events.md) · [data-binding-basics](data-binding-basics.md) · [designer-cs-anatomy](designer-cs-anatomy.md) · [designer-editing-rules](designer-editing-rules.md) · [designer-hand-edit-policy](designer-hand-edit-policy.md) · [di-container-lifetimes](di-container-lifetimes.md) · [docking-panels-patterns](docking-panels-patterns.md) · [errorprovider-validation-flow](errorprovider-validation-flow.md) · [exception-boundary-ui](exception-boundary-ui.md) · [form-close-and-dispose](form-close-and-dispose.md) · [form-close-cancel-pattern](form-close-cancel-pattern.md) · [form-constructor-vs-load](form-constructor-vs-load.md) · [form-load-and-shown-events](form-load-and-shown-events.md) · [layout-table-panel](layout-table-panel.md) · [logging-integration](logging-integration.md) · [settings-user-config](settings-user-config.md) · [splitcontainer-and-sizing](splitcontainer-and-sizing.md) · [validation-form-level](validation-form-level.md) · [winforms-rendering-performance](winforms-rendering-performance.md)

> **Tek cumlelik sorumluluk:** *Form dosyalarinin adlandirmasi, `partial` butunlugu ve dosya ici duzen yasagi hangi kurallara baglidir* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Bir formun uc dosyasi da **ayni sinif adini** tasir: `SessionListForm.cs` + `SessionListForm.Designer.cs` + `SessionListForm.resx` ve ucude **ayni namespace**'dedir. `partial` anahtari iki dosyada da **yazilidir**; birinde eksikse derleme `CS0260` ile patlar. Dosya icinde **ikinci sinif tanimlanmaz**, `#region` **kullanilmaz**. Adlar ASCII ve PascalCase'dir; dosya sistemi adi ile sinif adi birebir ayni oldugu icin `typeof(X)` -> `X.resx` eslemesi kendiliginden dogru calisir.

---

## §1 Dosya -> sinif eslemesi

| Dosya | Sinif | `partial` | Icerik sahibi |
|---|---|---|---|
| `SessionListForm.cs` | `SessionListForm` | evet | developer (is mantigi) |
| `SessionListForm.Designer.cs` | `SessionListForm` | evet | ui-designer (tek `InitializeComponent`) |
| `SessionListForm.resx` | - (sinif adina adanmis) | - | ui-designer (ResourceManager anahtarlari) |
| `SessionEditForm.cs` | `SessionEditForm` | evet | developer |
| `Program.cs` | `Program` | tek dosya | composition root ([composition-root-host-startup](composition-root-host-startup.md)) |

Dosya adi ile sinif adi ayni degilse `CS0260` ya da tasarimcida "class not found" alinir; duzeltme **dosya adini sinif adina getirmektir**, sinifi degistirmek degil.

---

## §2 Kural tablosu

| # | Kural | Yasak | Denetim |
|---|---|---|---|
| 1 | Dosya adi = sinif adi (PascalCase, ASCII) | `form-ana.cs`, `Form1.cs` | klasor listesi |
| 2 | `partial` iki dosyada da | tek tarafli `partial` | kod taramasi: `partial class` = 2 |
| 3 | Dosyada tek sinif | yardimci sinif gommek | kod incelemesi |
| 4 | `#region` yok | gorsel bolumleme (designer dosyasi dahil) | kod taramasi: `#region` = 0 |
| 5 | Namespace = klasor yolu | klasor disina suruklenme | kod incelemesi |

```csharp
// SessionListForm.cs
namespace VersaCoder.UI.Forms
{
    public partial class SessionListForm : RibbonForm          // partial YAZILI
    {
        private readonly ISessionQuery _sessions;
        public SessionListForm() { InitializeComponent(); }
        public SessionListForm(ISessionQuery s) : this() { _sessions = s; }
    }
}

// SessionListForm.Designer.cs (parca)
namespace VersaCoder.UI.Forms
{
    partial class SessionListForm                               // partial YAZILI
    {
        private System.ComponentModel.IContainer components = null;
        protected override void Dispose(bool disposing) { /* components?.Dispose(); */ }
        private void InitializeComponent() { /* TEK KAYNAK */ }
    }
}
```

---

## §3 Aykirilik sonuclari

| Aykirilik | Sonuc |
|---|---|
| Bir tarafta `partial` yok | `CS0260` - build 0 hata saglanamaz |
| `#region` eklendi | kod stili reddi |
| Ikinci sinif `.cs`'e gizlendi | tek sorumluluk + test edilebilirlik ihlali |
| Namespace sapmasi | `typeof` -> yanlis manifest adi, resx bulunamaz |
| C# 8+ sozdizimi ortak dosyada (`using var`, `??=`) | `net48` bacagi kirar |

---

## §4 net48 / net10.0-windows farki

| Konu | `net48` (C# 7.3) | `net10.0-windows` (C# 14) |
|---|---|---|
| `partial` davranisi | **ayni** | ayni |
| Partial metot kurallari | dar | genis - **paylasilan dosyada yeni ozellik kullanma** |
| `CS0260` / derleme hatalari | **ayni** | ayni |
| `#if` | yalniz platform API farki | yalniz platform API farki |

---

## §5 Kontrol listesi

| # | Adim | Kanit |
|---|---|---|
| 1 | Dosya adi = sinif adi, ASCII PascalCase | klasor listesi |
| 2 | `partial class` her formda 2 eslesme | grep |
| 3 | `#region` = 0 | grep |
| 4 | Namespace klasor yolu ile ayni | kod incelemesi |
| 5 | Iki bacakta `dotnet build` -> 0 hata | build ciktisi |

---

## §9 Kanit

| Kaynak dosya | Ne alindi |
|---|---|
| `knowledge\winforms\partial-class-and-file-naming.md` (kaynak vault) | esleme tablosu, 6 kural, kod ornegi, aykilik sonuclari |
| `.rules\naming-conventions.md` | adlandirma kurali |
| `.rules\coding-standards.md` | tek sinif/dosya ve `#region` yasagi |

> Kanitsiz iddia icin `DOĞRULAMA GEREKLI` yazilir; bu dosyada kaynak disi bilgi yoktur.
