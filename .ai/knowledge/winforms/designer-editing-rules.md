---
title: "designer-editing-rules - VersaCoder"
type: knowledge
category: winforms
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: ".Designer.cs ve .resx'in ne zaman elle duzenlenebilegini, bozulma belirtilerini ve git cakismasi cozum akisini tanimlar"
---

# Tasarim Dosyasi Elle Duzenleme Kurallari

**Zorunlu Baglantilar:** [designer-cs-trio-rule](designer-cs-trio-rule.md) · [designer-cs-anatomy](designer-cs-anatomy.md) · [designer-hand-edit-policy](designer-hand-edit-policy.md) · [ui-rules](../../.rules/ui-rules.md) · [coding-standards](../../.rules/coding-standards.md)

> **Tek cumlelik sorumluluk:** *`.Designer.cs` / `.resx` elle ne zaman ve kim tarafindan duzenlenebilir, bozuldugunda hangi belirtilerle anlasilir ve cakisma nasil cozulur* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

**`.Designer.cs` ve `.resx` elle duzenlenmez;** degisiklik Visual Studio tasarimcisinda yapilir ve bu iki dosyada son soz **`ui-designer`**'dadir. Elle mudahale **kacinilmazsa** (git cakismasi, merge) tek kural **dosyanin bir tarafi butun olarak alinir**, parcali duzeltme yapilmaz; ardindan form tasarimcida **acilip kaydedilerek** `.resx` ile yeniden eslestirilir. Parcali duzenleme en sik iki hatayi uretir: anahtar uyusmazligi (`MissingManifestResourceException`) ve kapanis blogunun kaybi ile **sessizce bozulan layout**.

```text
cakisma --> TAM dosyayi bir daldan al --> VS'te formu ac
         --> Ctrl+S (iki dosya birlikte yeniden yazilir)
         --> git diff + ekran dogrulama --> RED yoksa tamam
```

---

## §1 Yetki ve kural tablosu

| Durum | Kim yapar | Nasil |
|---|---|---|
| kontrol ekleme/degistirme, konum/boyut | `ui-designer` | VS tasarimcisi (Properties penceresi) |
| `.resx` metin/gorsel degisikligi | `ui-designer` | tasarimci properties -> Text/Icon alani |
| formun gorunum karari (DX kontrol secimi) | `ui-designer` | [devexpress-rules](../../.rules/devexpress-rules.md) - v26.1 + Office2010 |
| git merge cakismasi | `developer` **gecici** -> sonra tasarimci | dosyanin tek tarafi alinir, tasarimcida acilir |
| runtime `Text = veri` atamasi | `developer` | is mantigidir, tasarim dosyasina dokunmaz |

---

## §2 El ile mudahale belirtileri

| Belirti | Muhtemel neden | Duzeltme |
|---|---|---|
| `CS0102: ... zaten 'X' icin tanim iceriyor` | ayni uye iki kez (copy-paste) | tasarimciyi ac, kaydet; gerekirse dosyayi geri al |
| `MissingManifestResourceException` | resx anahtari != `resources.GetString(...)` adi | tasarimcida formu ac -> resx yeniden yazilir |
| form acilmiyor: "Exception when opening designer" | elle bozulmus `InitializeComponent` blogu | dosyayi bilinen iyi commite al, tasarimcida ac |
| layout bozuk ama kod derleniyor | `ResumeLayout(false)` / `EndInit()` satiri kaymis | elle ekleme - **dosyayi geri al** |
| `git conflict` icinde `.Designer.cs` | iki dal ayni dosyayi degistirmis | §3 akisini izle |
| resx icinde `<<<<<<<` | cakisma isareti metne sizmis | XML gecersiz -> dosyayi geri al |

> **NOT:** `ResumeLayout(false)` tek basina silinse bile derleme gecer - bu yuzden hata **derlemede degil, ekranda** cikar. Kanit zorunlu: formu **ac, calistir, gozle dogrula**.

---

## §3 Git cakismasi cozum akisi

| Adim | Yapilan | Kanit |
|---|---|---|
| 1 | cakisan dosyanin **TAMAMI** bir daldan al (theirs ya da ours - yarim YOK) | `git status` temiz |
| 2 | VS'te formu ac -> tasarimci dosyayi okur ve resx ile eslestirir | tasarimci hata vermiyor |
| 3 | formu KAYDET (Ctrl+S) -> `.Designer.cs` + `.resx` birlikte yeniden yazilir | iki dosya birlikte guncel |
| 4 | fark al (`git diff`) -> yalniz beklenen tasarimsal degisiklikler gorunmeli | elle eklenmis `SuspendLayout` satiri **yok** DOĞRULAMA GEREKLI |
| 5 | uygulamayi calistir -> ekran dogrula | ekran goruntusu ya da calisan uygulama |

---

## §4 Iki bacak farki (net48 / net10.0-windows)

| Konu | `net48` | `net10.0-windows` |
|---|---|---|
| tasarimcinin yazdigi dosya bicimi | **ayni** (UTF-8, `resources` blogu) | ayni |
| tasarim zamani host | .NET Framework designer host | .NET designer host - **dosya icerigi ayni** |
| elle `#if` ekleme | YASAK iki bacakta | YASAK |
| DPI anahtari (`$this.AutoScaleDimensions`) | tasarimci yazar | ayni anahtar gecerli (bkz. [anchor-dock-layout-rules](anchor-dock-layout-rules.md)) |

---

## §5 Kim neyi degistirir

| Kim | Bu dosyada yetkisi | Ihlalde sonuc |
|---|---|---|
| `ui-designer` | `.Designer.cs` + `.resx` son sozu | - |
| `developer` | yalniz `.Form.cs` + gerekceli gecici cakisma cozumu | yetkisiz degisiklik **RED**, geri alinir |
| `reviewer` | tarama: `git diff --name-only -- "*.Designer.cs" "*.resx"` | elle degisiklik bulgusu -> `ui-designer` |
| derleyici | iki `InitializeComponent` tanimi | build RED |

El ile mudahale taramasi (kanit uretir, beklenen sonuc **0** satir DOĞRULAMA GEREKLI):

```powershell
Select-String -Path "src\**\*.cs" -Pattern "SuspendLayout\(|ResumeLayout\(|BeginInit\(|Controls\.Add\(" |
  Where-Object { $_.Path -notlike "*.Designer.cs" }
```

---

## §9 Kanit

| Kaynak dosya | Ne alindi |
|---|---|
| `knowledge\winforms\designer-editing-rules.md` (kaynak vault) | yetki tablosu, bozulma belirtisi tablosu, 5 adimli cakisma akisi, iki bacak tablosu, tarama komutu |
| `.rules\ui-rules.md` | ekran/kod-behind yetki siniri |
| `checklists\winforms-designer-check.md` | denetim maddeleri (duz metin, link degil) |

> Kanitsiz iddia icin DOĞRULAMA GEREKLI yazilir; bu dosyada kaynak disi bilgi yoktur.
