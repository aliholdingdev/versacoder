---
title: "form-constructor-vs-load - VersaCoder"
type: knowledge
category: winforms
status: active
version: 1.0.0
updated: 2026-09-30
authority: primary
scope: "Form constructor'nda ne yapilmadigini, Load / Shown / Activated ayrimini ve ilk veri yuklemenin dogru yerini tanimlar."
tags: [knowledge]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# Form Constructor mi, Load mi?

**Zorunlu Baglantilar:** [[knowledge/index|vault indeks]] · [[knowledge/winforms/designer-cs-trio-rule|designer-cs-trio-rule]] · [[knowledge/winforms/form-load-and-shown-events|form-load-and-shown-events]] · [[knowledge/winforms/async-void-event-handlers|async-void-event-handlers]] · [[knowledge/winforms/di-container-lifetimes|di-container-lifetimes]]
**Küme + Hub:** [[knowledge/winforms/index|winforms index]] · [[index|index]] · [[claude|CLAUDE]] · [[definitions/agents|AGENTS]] · [[.decisions/brain|brain]] · [[definitions/glossary|glossary]] · [[governance/keys|keys]]
**Kardeşler (winforms 5/32):** tam liste → [[knowledge/winforms/index|index]] (kompakt kip — dosya 8 KB bandını korumak için)

> **Tek cumlelik sorumluluk:** *Formun ilk isi (kayit acma, veri getirme, pencere konumu) constructor'da mi yoksa `Load` / `Shown`'da mi yapilir* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

**Constructor yalniz iki sey yapar:** `InitializeComponent()` cagrisi ve enjekte edilen servislerin alanlara yazimi. Handle henüz olusmadigi icin constructor'da UI'ya dokunmak (metin atama, boyut okuma, `ShowDialog` cagirma, mesaj kutusu) **yeniden boyutlandirmayi ve handle'i erken yaratmayi** gibi yan etkiler dogurur. Isi olusturan bilgi (veri yukleme, pencere konumu, odak) `Load` icinde, gorunur olduktan sonra gereken is ise `Shown` icinde yapilir. Async veri yukleme `Load`'ta `async void` olay tutamaci ile yapilir ve **her `async void` govdesinde `try/catch` zorunludur**.

```text
+---------------------------------------------------------------+
| ctor : InitializeComponent() + alan atama  -- UI'ya DOKUNMA    |
| Load : ilk veri, kayit okuma (Handle artik var)                |
| Shown: odak, konum, animasyon sonrasi is                       |
| Activated: her odaklanmada tekrar (sik cagri, agir is YASAK)   |
+---------------------------------------------------------------+
```

---

## §1 Olay sirasi (ilk gosterim)

```text
new SessionListForm()      ->  ctor (InitializeComponent + alan atama)
        |
        v
   Form.Load                ->  ilk veri, kayit okuma (Handle artik var)
        |
        v
   Form.Shown               ->  odak, konum, animasyon sonrasi is
        |
        v
   Form.Activated           ->  her odak kazaniminda tekrar (sik cagri)
        |
        v
   FormClosing -> FormClosed -> Dispose
```

| Olay | Ne zaman | Sik kullanim | Yapilmaz |
|---|---|---|---|
| Constructor | `new ...` aninda | `InitializeComponent()` + `_servis = x` | UI okuma/yazma, `MessageBox`, `Task` baslatma |
| `Load` | Ilk kez, handle sonrasi | Veri sorgusu, baslik / konum | Uzun senkron is (UI donar) |
| `Shown` | Gorunur olduktan sonra | Ilk kontrole odak, animasyon sonrasi is | Veritabani sorgusu (`Load` yeterli) |
| `Activated` | Her odaklanmada | Yenileme tetigi | Agir is (her odakta tekrar maliyetli) |

---

## §2 Constructor'da yapilmayanlar

| # | Yasak is | Neden | Dogru yer |
|---|---|---|---|
| 1 | `this.Text = db.Soru()` (senkron veri) | handle yokken senkron I/O | `Load` + `async` |
| 2 | `this.Handle` / `CreateControl()` cagrisi | handle'i erken yaratir, DPI/parent hesabi kayar | Ilk `Shown` / `Load` |
| 3 | Konum / boyut hesabi | ekran metrigi pencere gorunmeden stabil degil | `Load` (`CenterScreen` zaten yeterli) |
| 4 | `Form.ShowDialog()` / mesaj kutusu | constructor henüz sahneyi hazirlamamis | olay tutamaci |
| 5 | Tasarim kodu (kontrol yaratimi, `SuspendLayout`) | proje yasagi - `.Designer.cs` tek kaynaktir | `XxxForm.Designer.cs` |

---

## §3 Dogru ornek (ctor + Load)

```csharp
// SessionListForm.cs - yalniz kullanici mantigi; designer kodu SIFIR
public partial class SessionListForm : RibbonForm
{
    private readonly ISessionQuery _sessions;      // DI alani

    public SessionListForm()                       { InitializeComponent(); }
    public SessionListForm(ISessionQuery sessions) : this()
    {
        _sessions = sessions;                      // alan atama serbest, UI'ya dokunma YOK
    }

    private async void SessionListForm_Load(object sender, EventArgs e)
    {
        try
        {
            var rows = await _sessions.ListAsync(CancellationToken.None);
            gridControl1.DataSource = rows;        // tek atama = toplu guncelleme
        }
        catch (Exception ex)                       // async void'de yakalama ZORUNLU
        {
            HataGoster(ex);                        // ErrorProvider / mesaj kutusu
        }
    }
}
```

### §3.1 Parca - yer - sahip

| Parca | Nerede | Sahip |
|---|---|---|
| `InitializeComponent()` tek satir cagri | `.cs` ctor | `.Designer.cs`'teki govde `ui-designer`'da |
| Servis alanina yazim | `.cs` ctor | `developer` |
| Kontrol yaratimi / `SuspendLayout` | `.Designer.cs` | `ui-designer` |
| `Load` govdesi (veri + yakalama) | `.cs` | `developer` |

---

## §4 Iki bacak farki

| Konu | `net48` | `net10.0-windows` |
|---|---|---|
| Olay sirasi (`Load` -> `Shown`) | **ayni** | ayni |
| Dil surumu | C# 7.3 -> `using var` **yok** (satir sonunda `using (...)`) | C# 14 yazilabilir ama **paylasilan dosya C# 7.3 kalir** |
| `async` veri yukleme | yerel destek, ek paket yok | ayni |
| `RibbonForm` + ribbon stili | DevExpress **v26.1** | DevExpress **v26.1** |

`#if` yalniz platform API farki icin kullanilir; olay sirasi farki yok -> `#if` bu dosyada gerekmez.

---

## §5 Kim neyi degistirir

| Kim | Ne yapar | Kanit |
|---|---|---|
| `developer` | ctor'da yalniz alan atamasi, veriyi `Load`'a | kod incelemesi |
| `ui-designer` | `Load` / `Shown` olay `+=` baglarini `.Designer.cs`'e | grep ciktisi |
| `reviewer` | ctor icinde `MessageBox`, `ShowDialog`, senkron I/O tarar | grep hedefi: 0 eslesme |
| `tester` | ikinci acilista bayat veri davranisini sinar | davranis testi |

---

## §6 Kontrol listesi

| # | Adim | Kanit |
|---|---|---|
| 1 | Constructor'da UI yazimi yok | kod incelemesi |
| 2 | Veri yukleme `Load`'ta `async` + kafes | kod incelemesi |
| 3 | `Shown` yalniz bir kez calisiyor (koruyucu bayrak) | davranis testi |
| 4 | `Activated` icinde agir is yok | kod incelemesi |
| 5 | Iki bacakta `dotnet build` -> 0 hata | build ciktisi DOĞRULAMA GEREKLI |

---

## §9 Kanit

| Kaynak dosya | Ne alindi |
|---|---|
| `knowledge\winforms\form-constructor-vs-load.md` (kaynak vault) | olay sirasi diyagrami, olay tablosu, constructor yasaklari, dogru ornek, sahip tablosu, iki bacak farki |
| `.rules\coding-standards.md` | kod-behind ve tasarim kodu siniri |
| `checklists\winforms-designer-check.md` | `.Designer.cs` tutarliliginin denetimi (duz metin, link degil) |

> Kanitsiz iddia icin `DOĞRULAMA GEREKLI` yazilir; bu dosyada kaynak disi bilgi yoktur.
