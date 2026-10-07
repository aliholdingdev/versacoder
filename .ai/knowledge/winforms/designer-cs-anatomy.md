---
title: "designer-cs-anatomy - VersaCoder"
type: knowledge
category: winforms
status: active
version: 1.0.0
updated: 2026-09-30
authority: SSOT
scope: "XxxForm.Designer.cs dosyasinin bolum bolum anatomisini, tek InitializeComponent kaynagini ve .cs ile sinirin nerede bittigini tanimlar"
---

# .Designer.cs Anatomisi

**Zorunlu Baglantilar:** [designer-cs-trio-rule](designer-cs-trio-rule.md) · [designer-editing-rules](designer-editing-rules.md) · [designer-hand-edit-policy](designer-hand-edit-policy.md) · [partial-class-and-file-naming](partial-class-and-file-naming.md) · [coding-standards](../../.rules/coding-standards.md)
**Küme + Hub:** [knowledge index](../index.md) · [winforms index](index.md) · [index](../../index.md) · [CLAUDE](../../CLAUDE.md) · [AGENTS](../../AGENTS.md) · [brain](../../brain.md) · [glossary](../../glossary.md) · [keys](../../keys.md)
**Kardeşler (winforms 5/32):** tam liste → [index](index.md) (kompakt kip — dosya 8 KB bandını korumak için)

> **Tek cumlelik sorumluluk:** *Bir `.Designer.cs` dosyasi hangi bloklari icerir, neden tek `InitializeComponent` kaynagidir ve sinir `.cs`'le tam olarak nerede biter* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

`.Designer.cs`, formun **tek** tasarim kaynagidir: uye tanimlari, kontrol uretimi, `SuspendLayout`/`ResumeLayout`, `BeginInit`/`EndInit`, `ComponentResourceManager` ve kaynak atamalari yalniz burada bulunur. Ayni form icin **ikinci bir `InitializeComponent`** olursa derleme hatasi verir; govde `.cs`'e tasinirsa tasarimci her acista dosyayi yeniden yazar ve celiski dogar. Tek satirlik kural: **kontrol uretimi / `SuspendLayout` / `ResumeLayout` / `BeginInit` hicbir `.cs` dosyasina (main form dahil) yazilmaz.**

```text
+--------------------------------------------------------------+
| XxxForm.Designer.cs  -> TEK InitializeComponent + uyeler     |
|                        + kontrol uretimi + Dispose           |
| XxxForm.cs           -> ctor'da 1 satir cagri + is mantigi   |
| XxxForm.resx         -> ResourceManager anahtarlari          |
| .cs'ye tasarim kodu YASAK (tasarimci siler / derleme hatasi)  |
+--------------------------------------------------------------+
```

---

## §1 Dosya siniri - ne var, ne yasak

| Dosya | Icerdigi | Icermesi yasak olan |
|---|---|---|
| `XxxForm.cs` | is mantigi, olay tutamaclari, servis alanlari | kontrol uretimi, `SuspendLayout`, `ResumeLayout`, `BeginInit`, konum/boyut atamasi |
| `XxxForm.Designer.cs` | `InitializeComponent()`, `components = new Container()`, uyeler | kullanici is mantigi, veritabani sorgusu, mesaj kutusu |
| `XxxForm.resx` | metin/gorsel/ikon anahtarlari, `$this.AutoScaleDimensions` | elle duzenlenmis bozuk base64, yinelenen `data name` |

---

## §2 Dosyanin bolumleri (sira onemlidir)

| # | Bolum | Tipik icerik | Ureten |
|---|---|---|---|
| 1 | `partial class ... : RibbonForm` | `IContainer components = null;` | tasarimci (`partial` iki tarafta) |
| 2 | `InitializeComponent()` basi | `ComponentResourceManager` + `SuspendLayout()` | tasarimci |
| 3 | uye/kontrol uretimi | `new GridControl()`, `Dock`, `Anchor`, `Name` | tasarimci |
| 4 | `BeginInit`/`EndInit`, panel `SuspendLayout`/`ResumeLayout` | kapsayici eslemeleri | tasarimci |
| 5 | `ResumeLayout(false)` + `PerformLayout()` | `resources.ApplyResources(...)` | tasarimci |
| 6 | kaynak atamalari | `this.lblBaslik.Text = resources.GetString(...)` | tasarimci |
| 7 | `Dispose(bool)` | `components?.Dispose()` | tasarimci |

```csharp
// XxxForm.Designer.cs - parca (elle DEGISTIRILMEZ)
private void InitializeComponent()
{
    this.components = new System.ComponentModel.Container();
    System.ComponentModel.ComponentResourceManager resources =
        new System.ComponentModel.ComponentResourceManager(typeof(SessionListForm)); // -> .resx
    this.SuspendLayout();
    // ... kontrol uretimi: Name / Location / Size / Anchor / Text
    this.ResumeLayout(false);
}
```

---

## §3 Sinir: her is hangi dosyada

| Is | `.cs` (developer) | `.Designer.cs` (ui-designer) |
|---|---|---|
| `InitializeComponent()` **cagrisi** (ctor icinde tek satir) | evet | govde burada |
| servis enjeksiyonu (`_svc = x`) | evet | hayir |
| `btnKaydet.Click += btnKaydet_Click;` | **tercihen `.cs`** | tasarimci da baglayabilir - **tek yer** olmali |
| `this.Text = "..."` sabit baslik | hayir (`$this.Text` / resx) | evet |
| `gridControl1.DataSource = rows` (veri) | evet (is verisi) | hayir |
| `SuspendLayout()` / `BeginInit()` | **YASAK** | tek kaynak |

> **KURAL:** Ayni olay iki yerde baglanirsa cift tetikleme olur; baglama noktasi **tek** tutulur ve incelemede aranir.

---

## §4 Iki bacak farki (net48 / net10.0-windows)

| Konu | `net48` | `net10.0-windows` |
|---|---|---|
| Dosyanin urettigi dil | C# 7.3 uyumlu (tur belirtir `new`) | ayni dosya iki bacakta da ayni |
| `ComponentResourceManager(typeof(X))` | **ayni** | ayni |
| Tasarimci host'u | VS tasarim zamani (Framework) | .NET designer host - dosya **degismez** |
| `#if` | **YOK** | YOK |

> **NOT:** Tasarim dosyasina `#if` koymak tasarim zamani derleme hatasi verir; iki bacak farki yalniz `.cs`'te ve platform API'sinde aranir.

---

## §5 Kim neyi degistirir

| Kim | Neyi degistirir | Nerede | Sonrasi |
|---|---|---|---|
| `ui-designer` | kontrol ekleme/kaldirma, konum, boyut, olay `+=` bagi | VS tasarimcisi | Ctrl+S ile `.Designer.cs` + `.resx` birlikte yazilir |
| `developer` | olay govdesi, servis, veri | `.Form.cs` | build + inceleme |
| `developer` (gecici) | git merge cakismasi | tek taraf butun alinir | tasarimcida ac + kaydet (bkz. [designer-editing-rules](designer-editing-rules.md)) |
| kimse | `InitializeComponent` govdesini elle yazmak | - | elle mudahale geri alinir (bkz. [designer-hand-edit-policy](designer-hand-edit-policy.md)) |

---

## §6 Tarama tablosu (kanit uretir)

| Aranacak | `.Form.cs` beklenen | `.Designer.cs` beklenen |
|---|---|---|
| `SuspendLayout(` | **0** DOĞRULAMA GEREKLI | var |
| `ResumeLayout(` | **0** DOĞRULAMA GEREKLI | var |
| `BeginInit()` | **0** DOĞRULAMA GEREKLI | var (DevExpress bilesenlerinde) |
| `Controls.Add(` | **0** DOĞRULAMA GEREKLI | var |
| `InitializeComponent()` | yalniz **cagri** (1 satir) | yalniz **tanim** (1 govde) |
| `#if` | `.cs`'te izinli | **0** DOĞRULAMA GEREKLI |

Ihlal belirtisi: `CS0102`/`CS0111` (iki tanim), tasarimcinin acilmamasi, kod derlenirken layoutun bozuk olmasi. Son kontrol: iki bacakta `dotnet build` -> 0 hata DOĞRULAMA GEREKLI.

---

## §9 Kanit

| Kaynak dosya | Ne alindi |
|---|---|
| `knowledge\winforms\designer-cs-anatomy.md` (kaynak vault) | uc dosya siniri, bolum tablosu, `InitializeComponent` parcasi, iki bacak tablosu, tarama satirlari |
| `.rules\coding-standards.md` | dosya/uye yazim kurali |
| `.rules\ui-rules.md` | kod-behind'ta tasarim kodu yasagi (bkz. [designer-cs-trio-rule](designer-cs-trio-rule.md)) |

> Kanitsiz iddia icin DOĞRULAMA GEREKLI yazilir; bu dosyada kaynak disi bilgi yoktur.
