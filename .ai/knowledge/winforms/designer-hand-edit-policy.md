---
title: "designer-hand-edit-policy - VersaCoder"
type: knowledge
category: winforms
status: active
version: 1.0.0
updated: 2026-09-30
authority: primary
scope: ".Designer.cs ve .resx uzerindeki son soz hakkini, elle mudahalenin sartlarini, geri alinma bicimini ve dosya okuma kuralini tanimlar"
tags: [knowledge]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# .Designer.cs / .resx Elle Duzenleme Politikasi

**Zorunlu Baglantilar:** [designer-cs-trio-rule](designer-cs-trio-rule.md) · [designer-cs-anatomy](designer-cs-anatomy.md) · [designer-editing-rules](designer-editing-rules.md) · [ui-rules](../../.rules/ui-rules.md) · [winforms-designer-check](../../checklists/winforms-designer-check.md)
**Küme + Hub:** [knowledge index](../index.md) · [winforms index](index.md) · [index](../../index.md) · [CLAUDE](../../claude.md) · [AGENTS](../../definitions/agents.md) · [brain](../../.decisions/brain.md) · [glossary](../../definitions/glossary.md) · [keys](../../governance/keys.md)
**Kardeşler (winforms · 4/32):** [accessibility-tab-and-mnemonics](accessibility-tab-and-mnemonics.md) · [anchor-dock-layout-rules](anchor-dock-layout-rules.md) · [async-await-ui-thread](async-await-ui-thread.md) · [async-loading-patterns](async-loading-patterns.md) · [async-ui-thread-marshalling](async-ui-thread-marshalling.md) · [async-void-event-handlers](async-void-event-handlers.md) · [backgroundworker-vs-task](backgroundworker-vs-task.md) · [cancellation-in-ui](cancellation-in-ui.md) · [cancellation-token-in-ui](cancellation-token-in-ui.md) · [composition-root-host-startup](composition-root-host-startup.md) · [control-disposal-and-memory](control-disposal-and-memory.md) · [controls-lifecycle-events](controls-lifecycle-events.md) · [data-binding-basics](data-binding-basics.md) · [di-container-lifetimes](di-container-lifetimes.md) · [docking-panels-patterns](docking-panels-patterns.md) · [errorprovider-validation-flow](errorprovider-validation-flow.md) · [exception-boundary-ui](exception-boundary-ui.md) · [form-close-and-dispose](form-close-and-dispose.md) · [form-close-cancel-pattern](form-close-cancel-pattern.md) · [form-constructor-vs-load](form-constructor-vs-load.md) · [form-load-and-shown-events](form-load-and-shown-events.md) · [layout-table-panel](layout-table-panel.md) · [logging-integration](logging-integration.md) · [partial-class-and-file-naming](partial-class-and-file-naming.md) · [settings-user-config](settings-user-config.md) · [splitcontainer-and-sizing](splitcontainer-and-sizing.md) · [validation-form-level](validation-form-level.md) · [winforms-rendering-performance](winforms-rendering-performance.md)

> **Tek cumlelik sorumluluk:** *`.Designer.cs` / `.resx` kimin malidir, elle mudahale ne zaman yapilir ve yapilirsa sonrasi nasil toparlanir* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

`.Designer.cs` ve `.resx` uzerinde **son soz `ui-designer`'indadir**; `developer`, `architect`, `tester` bu dosyalari degistirirse is **RED** edilir ve degisiklik geri alinir. Elle mudahale **yalniz** tasarimcinin cozemedigi durumda ve **yalniz** `ui-designer` tarafindan yapilir; sonraki tasarimci kaydi bu mudahaleyi **silecegi** icin tesadufi duzeltmeler para kaybidir. Tasarimci dosyasi okunurken **tamami baglanmaz**, parca parca okunur (§4).

```text
+----------------------------------------------------------+
| sahip: ui-designer  -> .Designer.cs + .resx + Resources  |
| sahip: developer    -> .Form.cs (SRP icinde serbest)      |
| ihlal  -> diff gorunur -> RED -> geri alma (onay + log)  |
+----------------------------------------------------------+
```

---

## §1 X sahipligi tablosu

| # | Dosya | X sahibi | Elle mudahale | Degisiklik sonrasi |
|---|---|---|---|---|
| 1 | `XxxForm.Designer.cs` | `ui-designer` | yalniz `ui-designer`, gerekceli | form tasarimcida yeniden acilmali |
| 2 | `XxxForm.resx` | `ui-designer` | yalniz `ui-designer`, anahtar korunarak | `.Designer.cs` ile birlikte |
| 3 | `XxxForm.cs` | `developer` | serbest (SRP icinde) | build + inceleme |
| 4 | `Properties/Resources.resx` | `ui-designer` | `ui-designer` | proje geneli etki -> iki bacak kontrol |

---

## §2 Ihlal tespiti (ne zaman RED)

| # | Belirti | Bulgusu yapan | Sonuc |
|---|---|---|---|
| 1 | tasarimci dosyayi acmiyor | `ui-designer` | diff alinir, geri alinir |
| 2 | `InitializeComponent` icinde `// TODO` / is mantigi | `reviewer` | `developer` RED |
| 3 | `.Form.cs` icinde `Controls.Add` | `reviewer` | form uclusu RED |
| 4 | `.resx` elle degistirilmis (`git diff` gorunur) | `reviewer` | yetki ihlali -> `ui-designer` |
| 5 | iki `InitializeComponent` tanimi | derleyici (CS0111) | build RED |

Tarama komutu - beklenen sonuc **0** satir DOĞRULAMA GEREKLI:

```powershell
git diff --name-only -- "*.Designer.cs" "*.resx"
Select-String -Path "src\**\*.cs" -Pattern "SuspendLayout\(|ResumeLayout\(|BeginInit\(|Controls\.Add\(" |
  Where-Object { $_.Path -notlike "*.Designer.cs" }
```

---

## §3 Elle mudahale izi: ne zaman gerekir

| # | Durum | Dogru davranis |
|---|---|---|
| 1 | DevExpress surum yukseltmesi `.Designer.cs`'i bozdu | `ui-designer` dosyayi tasarimcida yeniden uretir |
| 2 | `.resx` bozuk XML | `ui-designer` dosyayi yeniden kaydeder |
| 3 | olay `+=` yanlis baglanmis | `ui-designer` tasarimcidan baglar |
| 4 | `developer` "kucuk duzeltme" yapmak istiyor | **yapmaz**, `ui-designer`'a devreder |
| 5 | acil hotfix, `ui-designer` yok | `DECISION BLOCKED` -> kullaniciya sor |

---

## §4 Dosyayi okuma kurali

| # | Yontem | Ne icin |
|---|---|---|
| 1 | `grep` ile `this.btnXxx` ara | tek kontrolun tanimi |
| 2 | `offset`/`limit` ile parca parca oku | `InitializeComponent` baslangici |
| 3 | `Dispose` blogunu ayri oku | temizlik dogrulamasi |
| 4 | `.resx`'i `Select-String name=` ile tara | anahtar envanteri |

Tasarimci dosyasi genellikle yuzlerce satirdir ve **tamami baglama yuklenmez** (bkz. [designer-cs-anatomy](designer-cs-anatomy.md) §2).

---

## §5 Geri alma ve onay

Elle yapilmis ve kabul edilmeyen bir degisiklik `git checkout -- <dosya>` ile geri alinir; bu **yikici islem** sayilir ve geri alma **onay** gerektirir. Onay kaydi `log.md`'ye yazilir. Geri alma sonrasi form tasarimcida bir kez acilip kaydedilir, boylece `.Designer.cs` + `.resx` eslesmesi tazelenir.

---

## §6 Kim neyi degistirir

| Kim | Neyi yapar | Yapamaz |
|---|---|---|
| `ui-designer` | tasarimciyla duzeltme, resx anahtari, gerekceli elle mudahale | is mantigi yazmak |
| `developer` | `.Form.cs`, cakismada tek taraf alma, geri alma onayi isteme | `.Designer.cs`/`.resx` kalici degisiklik |
| `reviewer` | §2 taramasi + diff denetimi | dosyayi kendigi duzeltmek |
| `tester` | ekran uzerinden gozle dogrulama | tasarim dosyasina yazmak |

Uygulama kontrol listesi: diff'i bos ya da `ui-designer` imzali (git diff), `.Form.cs` taramasi 0 eslesme (DOĞRULAMA GEREKLI), tasarimci dosyayi aciyor (tasarimci kaniti), geri alma varsa onay kaydi (`log.md`), iki bacakta `dotnet build` -> 0 hata DOĞRULAMA GEREKLI.

---

## §9 Kanit

| Kaynak dosya | Ne alindi |
|---|---|
| `knowledge\winforms\designer-hand-edit-policy.md` (kaynak vault) | X sahipligi tablosu, ihlal tespiti, mudahale izi, okuma kurali, geri alma onayi, tarama komutu |
| `.rules\ui-rules.md` | tasarim dosyasi yetki siniri |
| `checklists\winforms-designer-check.md` | denetim maddeleri (duz metin, link degil) |

> Kanitsiz iddia icin DOĞRULAMA GEREKLI yazilir; bu dosyada kaynak disi bilgi yoktur.
