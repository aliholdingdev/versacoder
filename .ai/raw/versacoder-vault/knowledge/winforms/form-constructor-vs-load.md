---
title: "form-constructor-vs-load - VersaCoder"
type: knowledge
category: winforms
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "Form constructor'nda ne yapılmaz, Load/Shown/Activated ayrımı, ilk veri yüklemenin doğru yeri (net48 + net10.0-windows)"
---

# form-constructor-vs-load — Form Constructor mı, Load mı?

**Zorunlu Baglantilar:** [vault index](../../index.md) · [coding-standards](../../.rules/coding-standards.md) · [winforms index](../index.md) · [async-void-event-handlers](async-void-event-handlers.md) · [code-behind-zero-designer-code](code-behind-zero-designer-code.md)

> **Tek cümlelik sorumluluk:** *Formun ilk işi (kayıt açma, veri getirme, pencere konumu) constructor'da mı yoksa `Load`/`Shown`'da mı yapılır* sorusunun tek cevabıdır.

---

## §0 Ozet (Answer First)

**Constructor yalnız iki şey yapar:** `InitializeComponent()` çağrısı ve enjekte edilen servislerin alanlara yazımı. Handle henüz oluşmadığı için constructor'da UI'ya dokunmak (metin atama, boyut okuma, `ShowDialog` çağırma, mesaj kutusu) **yeniden boyutlandırma ve handle'ı erken yaratma** gibi yan etkiler doğurur. İşi oluran bilgi (veri yükleme, pencere konumu, odak) `Load` içinde, görünür olduktan sonra gereken iş ise `Shown` içinde yapılır. Async veri yükleme `Load`'ta `async void` olay tutamacı ile yapılır ve **her `async void` gövdesi `try/catch` zorunludur**.

---

## §1 Olay sirasi (ilk gosterim)

```
new SessionListForm()      ->  ctor (InitializeComponent + alan atama)
        |
        v
   Form.Load                ->  ilk veri, kayit okuma (Handle artik var)
        |
        v
   Form.Shown               ->  odak, konum, animation sonrasi is
        |
        v
   Form.Activated           ->  her odak kazaniminda tekrar (dikkat: sik cagri)
        |
        v
   FormClosing -> FormClosed -> Dispose
```

| Olay | Ne zaman | Sık kullanım | Yapılmaz |
|---|---|---|---|
| Constructor | `new ...` anında | `InitializeComponent()` + `_servis = x` | UI okuma/yazma, `MessageBox`, `Task` başlatma |
| `Load` | İlk kez, handle sonrası | Veri sorgusu (Dapper), başlık/konum | Uzun senkron iş (UI donar) |
| `Shown` | Görünür olduktan sonra | İlk kontrole odak, animasyon sonrası iş | Veritabanı sorgusu (`Load` yeterli) |
| `Activated` | Her odaklanmada | Yenileme tetiği | Ağır iş (her odakta tekrar maliyetli) |

---

## §2 Constructor'da yapilmayanlar

| # | Yasak iş | Neden | Doğru yer |
|---|---|---|---|
| 1 | `this.Text = db.Soru()` (senkron veri) | Handle yokken/pencere görünmeden senkron I/O | `Load` + `async` |
| 2 | `this.Handle` / `CreateControl()` çağrısı | Handle'ı erken yaratır, DPI/parent hesabı kayar | İlk `Shown`/`Load` |
| 3 | Konum/boyut hesabı (`StartPosition` dışına düşme) | Ekran metriği pencere görünmeden stabil değil | `Load` (`CenterScreen` zaten yeterli) |
| 4 | `Form.ShowDialog()` / mesaj kutusu | Constructor henüz sahneyi hazırlamamış | Olay tutamacı |
| 5 | Tasarım kodu (kontrol yaratımı, `SuspendLayout`) | Proje yasağı — `.Designer.cs` tek kaynaktır | `XxxForm.Designer.cs` |

> **KURAL:** Tasarım kodu `.cs`'e yazılmaz (main form dahil) — ayrıntı: [code-behind-zero-designer-code](code-behind-zero-designer-code.md).

---

## §3 Dogru ornek (ctor + Load)

```csharp
// SessionListForm.cs — yalniz kullanici mantigi; 0 satir designer kodu
public partial class SessionListForm : RibbonForm
{
    private readonly ISessionQuery _sessions;      // DI alani

    public SessionListForm()                       { InitializeComponent(); } // tasarimci icin
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

| Parça | Nerede | Sahip |
|---|---|---|
| `InitializeComponent()` tek satır çağrı | `.cs` ctor | designer'ın oluşturduğu blok `.Designer.cs`'te |
| Servis alanına yazım | `.cs` ctor | developer |
| Kontrol yaratımı / `SuspendLayout` | **`.Designer.cs`** | ui-designer |
| `Load` gövdesi (veri + yakalama) | `.cs` | developer |

---

## §4 net48 / net10.0-windows farki

| Konu | `net48` | `net10.0-windows` |
|---|---|---|
| Olay sırası (`Load` → `Shown`) | **aynı** | aynı |
| Dil sürümü | C# 7.3 → `using var` **yok** (satır sonunda `using (...)`) | C# 14 yazılabilir ama **paylaşılan dosya C# 7.3 kalır** |
| `async` veri yükleme | yerel destek, ek paket yok | aynı |
| `RibbonForm` + `RibbonControlStyle.Office2010` | DevExpress **v26.1** | DevExpress **v26.1** (ADR-004) |

> **NOT:** `#if` yalnız platform API farkı için kullanılır; olay sırası/imalat farkı yok → `#if` bu dosyada gerekmez.

---

## §5 Kardesler

[form-showdialog-result](form-showdialog-result.md) · [form-close-cancel-pattern](form-close-cancel-pattern.md) · [async-void-event-handlers](async-void-event-handlers.md) · [constructor-injection-in-forms](constructor-injection-in-forms.md)
