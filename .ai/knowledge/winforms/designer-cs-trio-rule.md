---
title: "designer-cs-trio-rule - VersaCoder"
type: knowledge
category: winforms
status: active
version: 1.0.0
updated: 2026-09-29
authority: primary
scope: "Her formun neden tam uc dosyadan olustugunu, hangi dosyanin hangi yetkiye sahip oldugunu ve .cs dosyasinda tasarim koduna neden izin verilmedigini tanimlar"
tags: [knowledge]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# Designer.cs + .resx + Kod-Behind Uclusu Kurali

**Zorunlu Baglantilar:** [[knowledge/index|vault indeks]] · [[.rules/coding-standards|coding-standards]] · [[.rules/ui-rules|ui-rules]] · [[knowledge/winforms/partial-class-and-file-naming|partial-class-and-file-naming]] · [[knowledge/winforms/anchor-dock-layout-rules|anchor-dock-layout-rules]]
**Küme + Hub:** [[knowledge/winforms/index|winforms index]] · [[index|index]] · [[claude|CLAUDE]] · [[definitions/agents|AGENTS]] · [[.decisions/brain|brain]] · [[definitions/glossary|glossary]] · [[governance/keys|keys]]
**Kardeşler (winforms · 3/32):** [[knowledge/winforms/accessibility-tab-and-mnemonics|accessibility-tab-and-mnemonics]] · [[knowledge/winforms/async-await-ui-thread|async-await-ui-thread]] · [[knowledge/winforms/async-loading-patterns|async-loading-patterns]] · [[knowledge/winforms/async-ui-thread-marshalling|async-ui-thread-marshalling]] · [[knowledge/winforms/async-void-event-handlers|async-void-event-handlers]] · [[knowledge/winforms/backgroundworker-vs-task|backgroundworker-vs-task]] · [[knowledge/winforms/cancellation-in-ui|cancellation-in-ui]] · [[knowledge/winforms/cancellation-token-in-ui|cancellation-token-in-ui]] · [[knowledge/winforms/composition-root-host-startup|composition-root-host-startup]] · [[knowledge/winforms/control-disposal-and-memory|control-disposal-and-memory]] · [[knowledge/winforms/controls-lifecycle-events|controls-lifecycle-events]] · [[knowledge/winforms/data-binding-basics|data-binding-basics]] · [[knowledge/winforms/designer-cs-anatomy|designer-cs-anatomy]] · [[knowledge/winforms/designer-editing-rules|designer-editing-rules]] · [[knowledge/winforms/designer-hand-edit-policy|designer-hand-edit-policy]] · [[knowledge/winforms/di-container-lifetimes|di-container-lifetimes]] · [[knowledge/winforms/docking-panels-patterns|docking-panels-patterns]] · [[knowledge/winforms/errorprovider-validation-flow|errorprovider-validation-flow]] · [[knowledge/winforms/exception-boundary-ui|exception-boundary-ui]] · [[knowledge/winforms/form-close-and-dispose|form-close-and-dispose]] · [[knowledge/winforms/form-close-cancel-pattern|form-close-cancel-pattern]] · [[knowledge/winforms/form-constructor-vs-load|form-constructor-vs-load]] · [[knowledge/winforms/form-load-and-shown-events|form-load-and-shown-events]] · [[knowledge/winforms/layout-table-panel|layout-table-panel]] · [[knowledge/winforms/logging-integration|logging-integration]] · [[knowledge/winforms/settings-user-config|settings-user-config]] · [[knowledge/winforms/splitcontainer-and-sizing|splitcontainer-and-sizing]] · [[knowledge/winforms/validation-form-level|validation-form-level]] · [[knowledge/winforms/winforms-rendering-performance|winforms-rendering-performance]]

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
