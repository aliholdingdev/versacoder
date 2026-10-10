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

**Zorunlu Baglantilar:** [[knowledge/winforms/designer-cs-trio-rule|designer-cs-trio-rule]] · [[knowledge/winforms/designer-cs-anatomy|designer-cs-anatomy]] · [[knowledge/winforms/designer-editing-rules|designer-editing-rules]] · [[.rules/ui-rules|ui-rules]] · [[checklists/winforms-designer-check|winforms-designer-check]]
**Küme + Hub:** [[knowledge/index|knowledge index]] · [[knowledge/winforms/index|winforms index]] · [[index|index]] · [[claude|CLAUDE]] · [[definitions/agents|AGENTS]] · [[.decisions/brain|brain]] · [[definitions/glossary|glossary]] · [[governance/keys|keys]]
**Kardeşler (winforms · 4/32):** [[knowledge/winforms/accessibility-tab-and-mnemonics|accessibility-tab-and-mnemonics]] · [[knowledge/winforms/anchor-dock-layout-rules|anchor-dock-layout-rules]] · [[knowledge/winforms/async-await-ui-thread|async-await-ui-thread]] · [[knowledge/winforms/async-loading-patterns|async-loading-patterns]] · [[knowledge/winforms/async-ui-thread-marshalling|async-ui-thread-marshalling]] · [[knowledge/winforms/async-void-event-handlers|async-void-event-handlers]] · [[knowledge/winforms/backgroundworker-vs-task|backgroundworker-vs-task]] · [[knowledge/winforms/cancellation-in-ui|cancellation-in-ui]] · [[knowledge/winforms/cancellation-token-in-ui|cancellation-token-in-ui]] · [[knowledge/winforms/composition-root-host-startup|composition-root-host-startup]] · [[knowledge/winforms/control-disposal-and-memory|control-disposal-and-memory]] · [[knowledge/winforms/controls-lifecycle-events|controls-lifecycle-events]] · [[knowledge/winforms/data-binding-basics|data-binding-basics]] · [[knowledge/winforms/di-container-lifetimes|di-container-lifetimes]] · [[knowledge/winforms/docking-panels-patterns|docking-panels-patterns]] · [[knowledge/winforms/errorprovider-validation-flow|errorprovider-validation-flow]] · [[knowledge/winforms/exception-boundary-ui|exception-boundary-ui]] · [[knowledge/winforms/form-close-and-dispose|form-close-and-dispose]] · [[knowledge/winforms/form-close-cancel-pattern|form-close-cancel-pattern]] · [[knowledge/winforms/form-constructor-vs-load|form-constructor-vs-load]] · [[knowledge/winforms/form-load-and-shown-events|form-load-and-shown-events]] · [[knowledge/winforms/layout-table-panel|layout-table-panel]] · [[knowledge/winforms/logging-integration|logging-integration]] · [[knowledge/winforms/partial-class-and-file-naming|partial-class-and-file-naming]] · [[knowledge/winforms/settings-user-config|settings-user-config]] · [[knowledge/winforms/splitcontainer-and-sizing|splitcontainer-and-sizing]] · [[knowledge/winforms/validation-form-level|validation-form-level]] · [[knowledge/winforms/winforms-rendering-performance|winforms-rendering-performance]]

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

Tasarimci dosyasi genellikle yuzlerce satirdir ve **tamami baglama yuklenmez** (bkz. [[knowledge/winforms/designer-cs-anatomy|designer-cs-anatomy]] §2).

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
