---
title: "VersaCoder — DevExpress Kuralları (v26.1)"
type: rules
category: rules
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
scope: "DevExpress Universal 2026 / v26.1 tek sürüm kanunu — ribbon stili, form üçlüsü, lisans/binding, sürüm sapması, kontrol kullanımı ve Designer dosyası edit kuralları (CLAUDE.md G3 derinliği)"
---

# VersaCoder — DevExpress Kuralları (`.rules/devexpress-rules.md`)

**Zorunlu Bağlantılar:** [index.md](../index.md) · [CLAUDE.md](../CLAUDE.md) · [keys.md](../keys.md) · [coding-standards.md](./coding-standards.md) · [solid-rules.md](./solid-rules.md) · [testing-rules.md](./testing-rules.md) · [../reports/00-kapsam-analizi.md](../reports/00-kapsam-analizi.md)

> **Tek cümlelik sorumluluk:** *Hangi DevExpress sürümü, hangi ribbon stili, hangi form kalıbı ve ne Designer dosyasında düzenlenir* sorusunun tek cevabıdır.
> Bu dosya `CLAUDE.md` **G3 (DevExpress Sürüm Kanunu)** guardrail'inin teknik derinliğidir; çelişirse `CLAUDE.md` kazanır.

---

## §1 Kapsam

**İçinde:** `VersaCoder.Desktop` UI katmanı — sürüm sabitleme, ribbon/form/grid kullanımı, `.Designer.cs`/`.resx` edit kuralları, binding/lisans, DPI.

**Dışında:** ekran wireframe tasarımı (GATE-003, `ui-design/`), SOLID yerleşimi (`solid-rules.md`), genel kod yazımı (`coding-standards.md`).

> **KURAL:** DevExpress **yalnız UI katmanında** yaşar; başka katman `DevExpress.*` referansı alamaz (DX-15).
> **YASAK:** `v26.1` dışına çıkan herhangi bir sürüm referansı (`24.2`, `23.x`, `25.x`) — derleme değil, **RED** (G3).

### §1.1 Sürüm Tek Kaynağı

```
Directory.Packages.props  (CPM - tek nokta)
  +---------------------------------------------+
  | <PackageVersion Include="DevExpress.*"      |
  |                  Version="26.1.<patch>" />  |  <-- tek sabitleme yeri
  +---------------------------------------------+
        |                          |
        v                          v
  VersaCoder.Desktop.csproj    (sürüm YAZMAZ - sadece PackageReference Include)
  App.config (net48 binding)   (yönlendirme -> 26.1 ayni)
```

> **UYARI:** `<patch>` değeri lisanslı kurulumdan gelir ve **PART 5'te** sabitlenir; bu dosyada uydurulmaz (`NEEDS_VALIDATION`). Değişen tek sayı **o** satırdır; başka yerde sürüm geçmesin.

---

## §2 Kurallar (DX-01 .. DX-15)

| ID | Kural | Neden | İhlal belirtisi | Düzeltme |
|---|---|---|---|---|
| DX-01 | **Tek sürüm: DevExpress Universal 2026 / v26.1** — sabitleme yalnız `Directory.Packages.props` içinde | İki sürüm yan yana = binding conflict + beklenmedik render | `.csproj`'de elle `Version="..."`; iki farklı DevExpress sürümü | CPM'e taşı; `.csproj`'den sürümü sil |
| DX-02 | **Sürüm sapması yok (G3)**: kod, belge, config'te `24.2/23.x/25.x` referansı **0** | Eski sürüm referansı license/designer kırar; ai.01'den sızıntı riski | Taramada `DevExpress.* 24.` kalıbı | İlgili satırı v26.1'e çevir; kaynak neyse (eski vault) `CS-13` gereği yeniden yaz |
| DX-03 | **Ribbon yalnız `RibbonControlStyle.Office2010`** — tam satır: `this.ribbonControl1.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonControlStyle.Office2010;` | G3: diğer stiller (Office2007, Office2010Blue vb.) yasak | `.Designer.cs`'de `RibbonStyle =` başka bir değer | Designer'da `RibbonControlStyle.Office2010` seç; elle yazma (DX-10) |
| DX-04 | **Ana pencere `RibbonForm`**: `public partial class MainForm : RibbonForm` | G3: `XtraForm` ana pencere yasak; ribbon gövdeye gömülür | `: XtraForm` türeyen ana pencere | `RibbonForm`'a çevir; ribbon bağlantısını Designer'da kur |
| DX-05 | **Form üçlüsü zorunlu**: `FormName.cs` + `FormName.Designer.cs` + `FormName.resx` — üçü birlikte, aynı isimle | Tek başına `.cs` olan form Designer'da açılmaz; resx yoksa yerelleştirme/DPI kırılır | `.Designer.cs` olmayan `*Form.cs`; adı farklı üçlü | Üç dosyayı eşle; `resx` adı sınıf adıyla birebir |
| DX-06 | **Lisans/binding kuralları**: net48 `App.config` binding redirect **yalnız** 26.1'e (`AutoGenerateBindingRedirects=true`); lisans anahtarı/registration dosyası **repo'ya girmez** (CI/ortam değişkeni) | Yanlış redirect = `FileLoadException`; lisans dosyası sızıntısı = uyumsuzluk | Redirect 24.x/25.x gösteriyor; repo'da lisans dosyası | Redirect'i 26.1'e sabitle; lisansı ortamdan ver (`NEEDS_VALIDATION`: kesin lisans yöntemi PART 5'te) |
| DX-07 | **Ribbon yapısı**: `RibbonPage` (mantıksal sekme) → `RibbonPageGroup` (bölüm) → `BarItem` (eylem); eylem mantığı ayrı sınıfta (SOL-01) | Ribbon'u kodda doldurmak Designer'ı her açılışta siler | Handler içinde `ribbonPage1.Items.Add(...)` | Yapıyı Designer'da kur; kod yalnız `ItemClick` → handler çağrısı |
| DX-08 | **Grid standardı**: `GridControl` + `GridView`; kolon başlıkları/tipleri Designer'da; veri `DataSource` ile; biçim `DisplayFormat`/`FormatString`; elle hücre boyama yok | Elle çizim/elle kolon = DPI ve tema altında bozulma | `CustomDrawCell` ile "stil"; kodda `gridView1.Columns.Add` tekrarı | Designer + binding; özel görünüm `ColumnView` özellikleriyle |
| DX-09 | **Kontrol seti ekranlarla eşleşir** (kapsam analizi §4.1 aday listesi): Start=`TileControl`, IDE=`DockManager`+`StatusBarControl`, Solution=`TreeList`, Editor=`RichEditControl`/`MemoEdit`+`XtraTabControl`, Chat=`ListBoxControl`+`MemoEdit`, Agent=`GridControl`+`ProgressPanel`, Settings=`DataLayoutControl`+`PropertyGridControl` | Aşağıdaki isimlendirilmiş kontrol, ekranın reddedilmiş alternatifiyle değiştirilmez | Ekran kodunda liste dışı ana kontrol | Listeye dön; alternatif istek → `ui-design/` + kullanıcı onayı |
| DX-10 | **`.Designer.cs` elle edit kuralları**: elle **yazılmaz** — `InitializeComponent` gövdesi, `SuspendLayout/ResumeLayout`, `BeginInit/EndInit` çiftleri, `resources.ApplyResources(...)`, olay bağlantı satırları, `Name`/`TabIndex` alanları | Elle yazılan satır Designer yeniden üretilince **silinir**; çift `ResumeLayout` çökme üretir | Designer dosyasında iş mantığı; elle eklenmiş `this.Controls.Add` | Her yapısal değişiklik **tasarımcıdan**; elle olduysa Designer'ı aç-kapa ile doğrula ve diff'i temizle |
| DX-11 | **`.Designer.cs`'de olmaması gereken**: iş kuralı, ağ/DB çağrısı, `async`, gösterge (Toast/MessageBox), `TODO` yorumu | Designer kodu üretilir; buradaki mantık kaybolur veya incelemeden kaçar | `Form_Load` içinde servis çağrısı (`.Designer.cs` içinde) | Mantığı `FormName.cs`'e (event handler) veya handler/servise taşı |
| DX-12 | **Sahiplik/Dispose**: kontroller `IContainer` tarafından dispose edilir; form kodunda elle `control.Dispose()` **yasak** | Çift dispose = `ObjectDisposedException` | `Dispose()` elle çağrılmış | Sahipliği Designer'a bırak; yalnız **sahip olmadığınız** nesneyi dispose edin (`CS-15`) |
| DX-13 | **Olay bağlantısı**: kritik olaylar Designer'da; handler adı olayı anlatır (`OnRibbonFilePrintClick` benzeri), handler **ince** kalsın (tek çağrı) | Kalın handler = Form sınıfı god-class (SOL-01) | Tek handler'da 200 satır akış | Handler → handler/servis çağrısı (`solid-rules.md` SOL-02) |
| DX-14 | **DPI/otomatik ölçek**: `AutoScaleMode` Designer'ın değeri korunur; sabit `Size` yerine anchoring/docking; `#if` yalnız platform DPI API'sinde | Farklı DPI'de kesilmiş/taşmış kontroller | Elle yazılmış `AutoScaleMode = None` | Designer'a dön; `Anchor`/`Dock` kullan |
| DX-15 | **Katman izolasyonu**: `DevExpress.*` referansı **yalnız** `VersaCoder.Desktop` projesinde | DevExpress'i çekirdeğe taşımak testleri ve net10/Linux taşınabilirliğini kilitler | `.Domain`/`.Application`/`.Protocol` csproj'inde DevExpress paketi | Paketi UI'a taşı; arada DTO/abstraction (`solid-rules.md` SOL-05) |

> **YASAK:** DX-03'teki satır dışında bir ribbon stili; DX-04 dışında ana pencere tabanı; DX-10'da listelenen satırları elle değiştirmek.

---

## §3 Kritik Kuralların Ayrıntısı

### §3.1 DX-03 — Ribbon Stili (uygun tek satır)

```csharp
// FormName.Designer.cs -> InitializeComponent() icinde (tasarimcida uretilir)
this.ribbonControl1.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonControlStyle.Office2010;
```

```csharp
// FormName.cs (kod dosyasi) - taban tip
public partial class MainForm : RibbonForm
{
    public MainForm()
    {
        InitializeComponent();
    }
}
```

| | |
|---|---|
| **YASAK** | `RibbonControlStyle.Office2007` · `Office2010Blue` · `Office2010Silver` · `OfficeUniversal` · varsayılanı değiştiren başka atama |
| **İhlalde** | CRITICAL — build RED (G3) |
| **Doğrulama** | `.rules/devexpress-rules.md` taraması: `RibbonStyle =` geçen her satır `Office2010` |

### §3.2 DX-05 — Form Üçlüsü Yapısı

```
src/VersaCoder.Desktop/Features/Sessions/
  +-------------------------------+------------------+-----------------+
  | SessionListForm.cs            | olaylar + akış   | elde yazilir    |
  | SessionListForm.Designer.cs   | InitializeComponent | tasarimcida   |
  | SessionListForm.resx          | kaynak/metin/DPI | uretilir/duzenlenir |
  +-------------------------------+------------------+-----------------+
         |                |                  |
         |                |                  +--> elle duzenleme YASAK (DX-10)
         |                +--> elde is mantigi YASAK (DX-11)
         +--> partial ayni sinif, ayni klasor
```

> **KURAL:** Yeni form = Designer'da ekle (üç dosya otomatik oluşur). Elle `cs` dosyası açıp isim uydurmak yasak (ayrıntı `naming-conventions.md` NC-09).

### §3.3 DX-07 — Ribbon Katmanı

```csharp
// KISA - handler ince kalir (SOL-01 / DX-13)
private void OnBarItemNewSessionItemClick(object sender, ItemClickEventArgs e)
    => _newSessionHandler.Execute(new NewSessionRequest());
```

```
RibbonPage "Session"
  +-- RibbonPageGroup "New"
  |     +-- BarButtonItem bsiNewSession   -> OnBarItemNewSessionItemClick
  |     +-- BarButtonItem bsiOpenSession
  +-- RibbonPageGroup "Tools"
        +-- BarButtonItem bsiRunAgent
```

### §3.4 DX-10 — Elle Edit Matrisi

| Alan | Elle edit | Bunun yerine |
|---|---|---|
| `InitializeComponent` gövdesi | **YASAK** | Designer'da düzenle |
| `SuspendLayout()` / `ResumeLayout()` | **YASAK** | Designer üretir |
| `BeginInit()` / `EndInit()` (grid/kolon) | **YASAK** | Designer |
| `resources.ApplyResources(control, "name")` | **YASAK** | resx + Designer |
| Olay bağlantı satırları (`+= ...`) | **YASAK** | Olaylar penceresi |
| Kontrol `Name`, `TabIndex`, `Dock`, `Anchor` | Tercih: Designer | Olay/Docking araçları |
| `FormName.cs` içindeki handler gövdesi | serbest | — (asıl kod alanı) |
| `FormName.resx` içindeki *metin* değeri | Designer'da | Yerelleştirme penceresi |

---

## §4 Review Checklist (incelemede işaretlenir)

- [ ] DX-01: sürüm yalnız `Directory.Packages.props`'ta; `.csproj`'de elle sürüm yok
- [ ] DX-02: `24.`/`23.`/`25.` DevExpress referansı = **0**
- [ ] DX-03: `RibbonStyle =` geçen her satır `Office2010`
- [ ] DX-04: ana pencere `RibbonForm` türevi
- [ ] DX-05: her form üçlüsü eksiksiz ve aynı ada sahip
- [ ] DX-06: binding redirect 26.1; repo'da lisans dosyası yok
- [ ] DX-07: Page → Group → Item hiyerarşisi; eylem mantığı Designer dışı
- [ ] DX-08: grid `DataSource` bağlı; elle hücre çizimi yok
- [ ] DX-09: ekran kontrol seti kapsam analizi §4.1 ile uyumlu
- [ ] DX-10: `.Designer.cs` diff'i yalnız tasarımcı kaynaklı (iş mantığı 0 satır)
- [ ] DX-11: `Designer.cs` içinde ağ/DB/async/yok = 0
- [ ] DX-12: elle `control.Dispose()` = 0
- [ ] DX-13: handler ≤ ~15 satır; tek sorumluluk
- [ ] DX-14: `AutoScaleMode` korunmuş; sabit konum yerine anchoring
- [ ] DX-15: DevExpress referansı yalnız `VersaCoder.Desktop`'ta (NetArchTest ile otomatik)

---

## §5 Sık Yapılan İhlaller (Common Violations)

| # | İhlal | Tip | Semptom | Düzeltme |
|---|---|---|---|---|
| 1 | Başka ribbon stili ataması | DX-03 | G3 RED | `Office2010` |
| 2 | Eski vault'tan `24.2` paket satırı kopyası | DX-02 | binding çakışması | CPM + sıfırdan yazım (CS-13) |
| 3 | Elle eklenen `this.Controls.Add` satırı | DX-10 | Designer yeniden yazınca kontrol kaybolur | tasarımı Designer'da kur |
| 4 | `.Designer.cs` içinde servis çağrısı | DX-11 | Designer'da kayıp/çökme | handler'a taşı |
| 5 | `.cs` tek başına form açmak (resx yok) | DX-05 | Designer hata verir | üçlüyü tamamla |
| 6 | Ana pencere `XtraForm` | DX-04 | G3 RED | `RibbonForm` |
| 7 | `.Domain`'de DevExpress paketi | DX-15 | test edilemez çekirdek | UI'a taşı (SOL-05) |
| 8 | Elle `Dispose()` çağrısı | DX-12 | `ObjectDisposedException` | sahipliği bırak |
| 9 | Kodda `Columns.Add` tekrarı | DX-08 | DPI/tema altında bozuk grid | Designer + binding |
| 10 | 500 satırlık `Form_Load` | DX-13/DX-11 | god form | handler'a böl (SOL-01) |

---

## §6 Doğrulama

- [ ] 15 kural `ID | Kural | Neden | İhlal belirtisi | Düzeltme` biçiminde
- [ ] DX-03'te **tam kod satırı** yazılı
- [ ] DX-09 kontrol listesi `reports/00-kapsam-analizi.md` §4.1 ile çelişmiyor
- [ ] Sürüm için uydurma patch sayısı yok (`NEEDS_VALIDATION` işaretli)
- [ ] Diyagramlar saf ASCII; frontmatter 8 alan, `authority: SSOT`

---

## §7 Referanslar

| # | Kaynak | Yol |
|---|---|---|
| 1 | Anayasa (G3) | `../CLAUDE.md` |
| 2 | Anahtar: ribbon, resx, v26.1 | `../keys.md` |
| 3 | 7 ekran aday kontrol listesi | `../reports/00-kapsam-analizi.md` |
| 4 | SOLID yerleşimi | `solid-rules.md` |
| 5 | Form adlandırma | `naming-conventions.md` |
| 6 | UI katmanı testi | `testing-rules.md` |
