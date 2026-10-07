---
title: "designer-cs-trio-rule - VersaCoder"
type: knowledge
category: winforms
status: active
version: 1.0.0
updated: 2026-09-29
authority: SSOT
scope: "Her formun neden tam uc dosyadan olustugunu, hangi dosyanin hangi yetkiye sahip oldugunu ve .cs dosyasinda tasarim koduna neden izin verilmedigini tanimlar"
---

# Designer.cs + .resx + Kod-Behind Uclusu Kurali

**Zorunlu Baglantilar:** [vault indeks](../index.md) · [coding-standards](../../.rules/coding-standards.md) · [ui-rules](../../.rules/ui-rules.md) · [partial-class-and-file-naming](partial-class-and-file-naming.md) · [anchor-dock-layout-rules](anchor-dock-layout-rules.md)
**Küme + Hub:** [winforms index](index.md) · [index](../../index.md) · [CLAUDE](../../claude.md) · [AGENTS](../../definitions/agents.md) · [brain](../../.decisions/brain.md) · [glossary](../../definitions/glossary.md) · [keys](../../governance/keys.md)
**Kardeşler (winforms · 3/32):** [accessibility-tab-and-mnemonics](accessibility-tab-and-mnemonics.md) · [async-await-ui-thread](async-await-ui-thread.md) · [async-loading-patterns](async-loading-patterns.md) · [async-ui-thread-marshalling](async-ui-thread-marshalling.md) · [async-void-event-handlers](async-void-event-handlers.md) · [backgroundworker-vs-task](backgroundworker-vs-task.md) · [cancellation-in-ui](cancellation-in-ui.md) · [cancellation-token-in-ui](cancellation-token-in-ui.md) · [composition-root-host-startup](composition-root-host-startup.md) · [control-disposal-and-memory](control-disposal-and-memory.md) · [controls-lifecycle-events](controls-lifecycle-events.md) · [data-binding-basics](data-binding-basics.md) · [designer-cs-anatomy](designer-cs-anatomy.md) · [designer-editing-rules](designer-editing-rules.md) · [designer-hand-edit-policy](designer-hand-edit-policy.md) · [di-container-lifetimes](di-container-lifetimes.md) · [docking-panels-patterns](docking-panels-patterns.md) · [errorprovider-validation-flow](errorprovider-validation-flow.md) · [exception-boundary-ui](exception-boundary-ui.md) · [form-close-and-dispose](form-close-and-dispose.md) · [form-close-cancel-pattern](form-close-cancel-pattern.md) · [form-constructor-vs-load](form-constructor-vs-load.md) · [form-load-and-shown-events](form-load-and-shown-events.md) · [layout-table-panel](layout-table-panel.md) · [logging-integration](logging-integration.md) · [settings-user-config](settings-user-config.md) · [splitcontainer-and-sizing](splitcontainer-and-sizing.md) · [validation-form-level](validation-form-level.md) · [winforms-rendering-performance](winforms-rendering-performance.md)

> **Tek cumlelik sorumluluk:** *Her form hangi uc dosyadan olusur ve hangi dosya neye karar verir* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Her form **tam uc dosyadan** olusur ve her dosyanin **tek bir sorumlulugu** vardir: `XxxForm.cs` yalniz kullanici mantigi, `XxxForm.Designer.cs` **tek `InitializeComponent` kaynagi**, `XxxForm.resx` **`ResourceManager` verisi**. `MainForm` dahil **hicbir** formda kontrol uretme, `SuspendLayout`, `ResumeLayout` ya da `BeginInit` kodu `.cs` dosyasina yazilmaz - bu kodun tek yeri `.Designer.cs` govdesidir.

```text
+---------------------------------------------------------------+
| XxxForm.cs        : is mantigi + olay yonlendiricileri        |
| XxxForm.Designer.cs: TEK InitializeComponent + kontrol uretimi |
| XxxForm.resx      : metin/ikon/culture anahtarlari            |
|                                                                   |
| .Form.cs'  --> tasarim kodu YASAK (tasarimci siler / CS0111)   |
+---------------------------------------------------------------+
```

---

## §1 Uc dosya - uc yetki

| # | Dosya | Tek sorumluluk | Yetki | Bu dosyada YASAK |
|---|---|---|---|---|
| 1 | `XxxForm.cs` | kullanici mantigi + olay yonlendiricileri | developer | `new Label()`, `Controls.Add`, `SuspendLayout`, `ResumeLayout`, `BeginInit` |
| 2 | `XxxForm.Designer.cs` | tek `InitializeComponent`, kontrol uretimi, olay `+=` baglari, `Dispose` | ui-designer | is mantigi, `async`, servis cagrisi |
| 3 | `XxxForm.resx` | metin/ikon/culture kaynaklari | ui-designer | elle XML duzenleme, `ResourceManager` adini degistirmek |

---

## §2 Tarama tablosu (kanit uretir)

| Aranacak | `.Form.cs` beklenen | `.Designer.cs` beklenen |
|---|---|---|
| `SuspendLayout(` | **0** | var |
| `ResumeLayout(` | **0** | var |
| `BeginInit()` | **0** | var (DevExpress bilesenlerinde) |
| `Controls.Add(` | **0** | var |
| `new DevExpress.` | **0** | var |
| `InitializeComponent()` | yalniz **cagri** (1 satir) | yalniz **tanim** (1 govde) |

---

## §3 Bu kural neden zorunlu

| # | `.cs`'e tasarim kodu yazilirsa | Sonuc |
|---|---|---|
| 1 | Visual Studio tasarimcisi dosyayi yeniden uretir | elle yazilan kod **silinir** |
| 2 | Iki `InitializeComponent` tanimi | derleme hatasi (CS0111) |
| 3 | `.resx` anahtarlari kod ile celisir | `MissingManifestResourceException` |
| 4 | Yetki siniri kalkar | ekran sahipligi ihlali |
| 5 | Kod-behind 200+ satira siser | is mantigi UI'a gomulur, test edilemez |

**Kim neyi degistirir:** kontrol ekleme/boyut/tema ve olay **baglama** (`+=`) `.Designer.cs` icinde; olay **govdesi** `.Form.cs` icinde ince kalir. `developer` `.resx`'i elliyorsa bulgu bicimi sunlardir: `.Designer.cs` ile `.resx` tutarsiz olur, form acilmaz, is **RED** olur ve ekran isine gecilir.

---

## §4 Kontrol listesi

| # | Adim | Kanit |
|---|---|---|
| 1 | Her form icin 3 dosya var (`.cs` / `.Designer.cs` / `.resx`) | dosya listesi |
| 2 | `.Form.cs` taramasi: `SuspendLayout|ResumeLayout|BeginInit|Controls.Add` = 0 | grep ciktisi |
| 3 | `InitializeComponent` tanimi yalniz 1 dosyada | grep = 1 eslesme |
| 4 | `.resx` gecerli `<root>` XML | dosya okundu |
| 5 | Iki bacakta `dotnet build` -> 0 hata | build ciktisi |

---

## §9 Kanit

| Kaynak dosya | Ne alindi |
|---|---|
| `knowledge\winforms\designer-cs-trio-rule.md` (kaynak vault) | uc dosya tablosu, kod ornegi, tarama tablosu, 5 sonuc satiri |
| `.rules\ui-rules.md` | ekran/kod-behind kurali |
| `checklists\winforms-designer-check.md` | denetim maddeleri (duz metin, link degil) |

> Kanitsiz iddia icin `DOĞRULAMA GEREKLI` yazilir; bu dosyada kaynak disi bilgi yoktur.
