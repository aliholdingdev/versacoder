---
title: "code-behind-zero-designer-code - VersaCoder"
type: knowledge
category: winforms
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "XxxForm.cs içinde 0 satır tasarım kodu kuralı (main form dahil): hangi kod nereye yazılır, denetim yöntemi, ihlal sonucu"
---

# code-behind-zero-designer-code — `.cs`'te Tasarım Kodu = 0 Satır

**Zorunlu Baglantilar:** [vault index](../../index.md) · [coding-standards](../../.rules/coding-standards.md) · [winforms index](../index.md) · [designer-cs-anatomy](designer-cs-anatomy.md) · [CLAUDE.md](../../CLAUDE.md)

> **Tek cümlelik sorumluluk:** *`XxxForm.cs` dosyasında hangi kodların (kontrol oluşturma, `SuspendLayout`, `ResumeLayout`, `BeginInit`) hiç bulunmayacağı ve bunun ana form dâhil nasıl denetlendiği* sorusunun tek cevabıdır.

---

## §0 Ozet (Answer First)

**Kural:** Bir formun kod-behind'u (`XxxForm.cs`) yalnız kullanıcı mantığı içerir — constructor'da alan atama, olay tutamaçları, doğrulama, servis çağrıları. **Kontrol oluşturma (`new Label()`), `SuspendLayout()`, `ResumeLayout()`, `BeginInit()`/`EndInit()` ve konum/boyut/anchor atamaları `.cs`'e yazılmaz;** bunların tek kaynağı `XxxForm.Designer.cs`'tir. Kural **ana form dâhil tüm formlar** için geçerlidir. İhlal, tasarımcı ile kod arasındaki farkı doğurur: tasarımcı dosyayı her açılışta yeniden üretir, elle yazılan tasarım kodu ya kaybolur ya da iki kez çalışır (çift `SuspendLayout` = kayıp layout).

---

## §1 Hangi kod nereye yazilir

| İş | Dosya | Not |
|---|---|---|
| `InitializeComponent()` çağrısı (tek satır) | `XxxForm.cs` ctor | Gövde `.Designer.cs`'te |
| `_servis = injected;` alan atama | `XxxForm.cs` ctor | DI ([constructor-injection-in-forms](constructor-injection-in-forms.md)) |
| `btnKaydet.Click += ...` olay bağlama | `.cs` **ya da** designer — **tek yer** | Çift bağlama yasak |
| `new GridControl()`, `new TableLayoutPanel()` | **YASAK** → `.Designer.cs` | Tasarımcı ekler |
| `SuspendLayout()` / `ResumeLayout()` | **YASAK** → `.Designer.cs` | Toplu güncelleme için `BeginUpdate` |
| `ctrl.Location/Size/Anchor/Dock` | **YASAK** → `.Designer.cs` | Layout kararı tasarım |
| `ctrl.Text = veri` (runtime) | `.cs` serbest | İş verisidir, tasarım kararı değil |
| `grid.DataSource = satirlar` | `.cs` serbest | Toplu atama = performans kalıbı |
| `AccessibleName` / `TabIndex` | **YASAK** → `.Designer.cs` | [accessibility-tab-and-mnemonics](accessibility-tab-and-mnemonics.md) |

---

## §2 Ornek: dogru ve yanlis

```csharp
// YANLIS  — SessionListForm.cs icinde tasarim kodu (GATE RED)
private void InitUi()
{
    var lbl = new Label { Text = "Oturum", Location = new Point(12, 12) };  // kontrol yaratimi YASAK
    this.SuspendLayout();                                                      // SuspendLayout YASAK
    panel1.Controls.Add(lbl);
    this.ResumeLayout(false);                                               // ResumeLayout YASAK
}
```

```csharp
// DOGRU — SessionListForm.cs yalniz kullanici mantigi
public SessionListForm(ISessionQuery sessions) : this()
{
    _sessions = sessions;                       // alan atama
}

private void SessionListForm_Load(object sender, EventArgs e)
{
    lblBaslik.Text = "Oturumlar";               // veri metni (tasarim degil)
    _ = YukleAsync();                           // is mantigi
}
```

| Denetim | Yöntem | Beklenen |
|---|---|---|
| `SuspendLayout` / `ResumeLayout` / `BeginInit` | Kod taraması (`.cs`, `.Designer.cs` hariç) | **0** eşleşme |
| `new Label(` / `new GridControl(` | Kod taraması (form `.cs`'i) | **0** eşleşme |
| `.Location =` / `.Size =` | Kod taraması (`.cs`) | **0** eşleşme (runtime konum ayarı istisnası gerekçeli) |
| Form açılışı | Çalıştırıp gözle kontrol | Layout designer'daki ile aynı |

---

## §3 Ihlalin bedeli

| İhlal | Belirti | Sonuç |
|---|---|---|
| `.cs`'e `SuspendLayout()` | Designer kaydettiğinde çakışan iki layout akışı | Tasarım dosyası ile kod farklılaşır |
| Runtime kontrol yaratımı | `Dispose` edilmeyen kontroller + olay sızıntısı | Bellek sızıntısı ([control-disposal-and-memory](control-disposal-and-memory.md)) |
| Konum/boyut kodda | Farklı DPI'da kayan ekran | [autoscalemode-dpi-scaling](autoscalemode-dpi-scaling.md) ihlali |
| `InitializeComponent` gövdesinin `.cs`'e taşınması | `CS0102` ya da tasarımcının geri alması | Tasarım dosyası çöp |
| Ana formda ihlal | Ribbon iskeleti tasarlanamaz (Office2010 kuralı) | [ADR-004](../../.decisions/accepted/ADR-004-devexpress-universal-v261-office2010.md) sapması |

> **KURAL:** `reviewer` bu taramayı **RED** sebebi sayar; `developer` dosyayı düzeltir, tasarım kararı gerekiyorsa `ui-designer`'a devreder.

---

## §4 net48 / net10.0-windows farki

| Konu | `net48` | `net10.0-windows` |
|---|---|---|
| Kural | **aynı** — platform bağımsız proje kuralı | aynı |
| `BeginUpdate` (toplu güncelleme) | DevExpress `BaseView` / `ListView` | aynı API |
| `SuspendLayout` kodu `.cs`'te | yasak | yasak |
| `#if` | **bu kural için gerekmez** | gerekmez |

---

## §5 Kardesler

[designer-cs-anatomy](designer-cs-anatomy.md) · [designer-editing-rules](designer-editing-rules.md) · [partial-class-and-file-naming](partial-class-and-file-naming.md) · [form-triple-consistency-check](form-triple-consistency-check.md)
