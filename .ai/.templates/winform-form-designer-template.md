---
title: "WinForms Form + Designer Sablonu (form tresligi + RibbonForm)"
type: template
category: templates
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "Her WinForms formun uretim kurali: form.cs / form.Designer.cs / form.resx tresligi, tum UI kodu Designer.cs'te, event handler form.cs'te"
---

# WinForms Form + Designer Sablonu

**Zorunlu Baglantilar:** [index](../index.md) · [CLAUDE](../CLAUDE.md) · [platform-arastirma](../reports/12-platform-arastirmasi.md) · [new-doc-contract](new-doc-contract.md)

> **Tek cumlelik sorumluluk:** *Yeni bir form hangi 3 dosyadan olusur, UI kodu
> nereye yazilir, Ribbon nasil kurulur* sorusunun web kanitli tek cevabidir.

---

## §0 Ozet (Answer First)

- **3 dosya zorunlu:** `<Ad>.cs` (mantık + event handler) · `<Ad>.Designer.cs`
  (**tüm UI design kodu**) · `<Ad>.resx` (kaynak) — `partial class` aynı isimde.
- **UI kodu tek yöntemde:** her kontrol `InitializeComponent()` içine yazılır;
  olay işleyicileri **asla** `.Designer.cs`'e yazılmaz (tasarımcı yeniden üretir, siler).
- **Anapencere = `RibbonForm`** (G3); `RibbonControl.RibbonStyle = Office2010`
  açıkça atanır (varsayılan değil, `Default = Office2013`).
- **net48 / net10.0-windows çift bacak** (G6): kod her iki TFM'de derlenir.

---

## §1 Amaç

Bu şablon tek bir soruyu yanıtlar: **bir WinForms formu hangi dosyalardan oluşur, UI kodu
tam olarak nereye yazılır ve bir şey bozulduğunda ne yapılır.** Şablonun varlık nedeni,
Visual Studio tasarımçısının (designer) üretim davranışının öngörülebilir olmasıdır:
`InitializeComponent()` her tasarımçı açılışında yeniden yazıldığı için, o gövdeye yazılan
el kodu silinir. Kaybı önleyen kural tek cümlede şudur:

> **Tasarımcının ürettiği dosyaya elle kod yazma; tasarlayıcının üretmediği dosyaya yalnızca
> davranış yaz.**

Amaç üç katmanlıdır; her katman bu belgenin bir bölümüne karşılık gelir:

| Katman | Ne kazandırır | Karşılığı |
|---|---|---|
| Sözleşme | dosya üçlüsü, sahiplik, sınır | §3.1 |
| Teknik | iskelet, yerleşim, event, kaynak, palet | §3.2 · §4.2 · §4.3 · §4.5 |
| Operasyon | adım sırası, doğrulama, hata modu, kanıt | §5 · §6 |

**Amaç olmayanlar.** Formun iş mantığı tasarımı, veri erişimi katmanı ve test stratejisi bu
şablonun dışındadır — bunlar `§2.2 Kapsam dışı`da listelenir. Şablon yalnızca *düzeni ve
dosya sözleşmesini* tanımlar; davranışın ne olduğu hakkında söz söylemez.

**Başarı ölçütü.** Bir form bu şablonla üretildiyse, tasarımçı dosyayı açıp kaydettiğinde
hiçbir el kodu kaybolmamalı ve `git diff` çıktısında yalnızca beklenen dosyalar değişmiş
olmalıdır. Beklenmedik bir dosyanın değişmesi, sınırın ihlal edildiğinin kanıtıdır (`§6.1`).

## §2 Kapsam

### §2.1 Kapsam içi

| # | Konu | Geçerli olduğu yer |
|---|---|---|
| 1 | Yeni form üçlüsü: `.cs` · `.Designer.cs` · `.resx` | her yeni form |
| 2 | Anapencere = `RibbonForm` | uygulamanın tek ana penceresi |
| 3 | `InitializeComponent()` gövde düzeni ve sırası | her form |
| 4 | Kontrol yerleşimi: `Anchor` · `Dock` · `Padding` | her form |
| 5 | Event bağlaması ile gövdesi ayrımı | her olay |
| 6 | `Dispose(bool)` ve tutulan kaynakların bırakılması | her form |
| 7 | Kaynak (`.resx`) kullanımı ve renk paleti | görsel üreten her form |
| 8 | Tasarımcı hata modları + salt-okunur denetim | her form |
| 9 | `net48` / `net10.0-windows` çift bacak | tüm UI projesi |

### §2.2 Kapsam dışı

| # | Konu | Neden dışarıda | Gideceği yer |
|---|---|---|---|
| 1 | İş mantığı sınıfları | forma değil ayrı katmana ait | proje mimarisi |
| 2 | Veri erişimi / EF Core | form yalnızca UI taşır | `.ai/architecture/` |
| 3 | Birim ve entegrasyon testleri | bu şablon kod değil düzen tanımlar | [session-log](session-log-template.md) §6 |
| 4 | DevExpress sürüm yükseltme kararı | G3 kararı vault düzeyindedir | `../keys.md` · [adr](adr/adr-index.md) |
| 5 | Ribbon dışı kütüphane stili | ayrı şablon konusu | [ribbon](frontend/ribbon-template.md) |
| 6 | Doküman/düz metin şablonları | bu belge yalnızca WinForms | [documentation](documentation/root-md-template.md) |

### §2.3 Uygulama önişlemi (precondition)

Şablon yalnızca üç koşul sağlandığında uygulanır:

1. `Directory.Packages.props` içinde **tek** bir DevExpress sürüm satırı vardır (G3).
2. UI projesi hem `net48` hem `net10.0-windows` hedeflerine sahiptir (G6).
3. Proje WinForms paketlerini referans alır (`DevExpress.Utils`, `DevExpress.XtraBars`,
   `DevExpress.XtraEditors`).

Bir koşul yoksa şablon uygulanmaz; önce ortam tamamlanır. Eksik koşulu şablonla kapatmaya
çalışmak, ileride "tasarımcı açılmıyor" hatasını üretir (`§6.3` → H1).

### §2.4 Girdi / çıktı sözleşmesi

| Yön | Öğe | Biçim |
|---|---|---|
| Girdi | form adı | `<Ad>Form` sonekli, ASCII, boşluksuz |
| Girdi | taşımak istediği kontroller | kontrol listesi + öncelik |
| Girdi | ana pencere mi? | evet → `RibbonForm`, hayır → `UserControl`/`Form` |
| Çıktı | 3 dosya | `<Ad>Form.cs` · `<Ad>Form.Designer.cs` · `<Ad>Form.resx` |
| Çıktı | kanıt | `§6.1` salt-okunur denetim çıktısı, `PASS`/`FAIL` satırları |

## §3 Mimari

### §3.1 Dosya üçlüsü

| # | Dosya | İçerik | Sahip |
|---|---|---|---|
| 1 | `<Ad>.cs` | sınıf, constructor, `InitializeComponent()` çağrısı, **tüm event handler** | developer |
| 2 | `<Ad>.Designer.cs` | **tüm UI design kodu**: kontrol alanları + `InitializeComponent()` + `Dispose()` | ui-designer |
| 3 | `<Ad>.resx` | ikon/metadata; VS tarafından yönetilir | (elle dokunma) |

> `WEB_VERIFIED — github.com/dotnet/winforms docs/designer + learn.microsoft.com (2026-10-04)`
> (`[12-platform-arastirmasi](../reports/12-platform-arastirmasi.md)` §4)

### §3.2 Designer.cs iskeleti

```text
namespace <Proje>.UI
{
    partial class <Ad>Form
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing) { /* VS uretimi - elle DOKUNMA */ }

        private void InitializeComponent()
        {
            // TUM UI DESIGN KODU BURADA: kontrol tanimi, konum, boyut, event BAGLAMA
            // olay IMPLEMENTASYONU bu dosyada DEGIL, <Ad>.cs icinde
        }
    }
}
```

| # | Kural | Neden |
|---|---|---|
| 1 | Event **bağlaması** (`this.btn.Click += ...`) Designer.cs'te olabilir | VS üretir |
| 2 | Event **gövdesi** `<Ad>.cs`'te olur | yeniden üretimde kaybolmaz |
| 3 | VS 17.5+ `this.` nitelikleyicisi varsayılan olarak kalkar | Roslyn codegen |
| 4 | Eski stil gerekiyorsa `.editorconfig` → `[*.designer.cs]` bloğu | tutarlılık |

#### §3.2.1 Tam `InitializeComponent()` gövdesi (düzenli kalıp)

Gövde her formda **aynı sırayla** kurulur. Sıra rastgele değildir: VS tasarımcısı da benzer
sırada üretir, bu yüzden elle yazılan sıra ile tasarlayıcının ürettiği sıra çatışırsa `git diff`
her kayıttan sonra temizlenmez.

```text
private void InitializeComponent()
{
    // (1) KAYNAKLAR — her zaman ilk satır
    this.components = new System.ComponentModel.Container();
    this resources = new System.ComponentModel.ComponentResourceManager(typeof(<Ad>Form));

    // (2) ALANLAR — alt bileşenler dâhil, alfabetik değil tutarlılık sırası
    this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
    this.btnKaydet = new DevExpress.XtraEditors.SimpleButton();
    this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
    this.ribbonControl1 = new DevExpress.XtraBars.Ribbon.RibbonControl();
    this.barButtonKaydet = new DevExpress.XtraBars.BarButtonItem(this.components);

    // (3) TASARIM ZİNCİRİ — her kontrol için:uspendLayout() ... ResumeLayout(false)
    ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
    ((System.ComponentModel.ISupportInitialize)(this.ribbonControl1)).BeginInit();
    this.SuspendLayout();

    // (4) ÖZELLİKLER — kontrol başına tek blok, kendi içinde sabit sıralı
    //
    //    panelControl1
    //
    this.panelControl1.Controls.Add(this.btnKaydet);
    this.panelControl1.Controls.Add(this.labelControl1);
    this.panelControl1.Dock = System.Windows.Forms.DockStyle.Fill;
    this.panelControl1.Location = new System.Drawing.Point(0, 147);
    this.panelControl1.Name = "panelControl1";
    this.panelControl1.Size = new System.Drawing.Size(984, 414);
    this.panelControl1.TabIndex = 0;

    //
    //    btnKaydet
    //
    this.btnKaydet.Anchor = ((System.Windows.Forms.AnchorStyles)(
        ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right))));
    this.btnKaydet.Location = new System.Drawing.Point(873, 367);
    this.btnKaydet.Name = "btnKaydet";
    this.btnKaydet.Size = new System.Drawing.Size(98, 28);
    this.btnKaydet.TabIndex = 1;
    this.btnKaydet.Text = "Kaydet";
    this.btnKaydet.Click += new System.EventHandler(this.btnKaydet_Click);   // (5) BAĞLAMA

    // (5) OLAY BAĞLAMALARI — özellik bloğunun sonunda, tek satır, tek olay
    //
    // (6) ANAPENCERE ÖZELLİKLERİ
    this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
    this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
    this.ClientSize = new System.Drawing.Size(984, 561);
    this.Controls.Add(this.panelControl1);
    this.Name = "<Ad>Form";
    this.Text = "<Başlık>";
    this.ResumeLayout(false);       // (7) KAPANIŞ — her Begin'in karşılığı
}

// (8) ALAN TANIMLARI — sınıfın altı, InitializeComponent'dan SONRA değil, dosyanın sonunda
private DevExpress.XtraEditors.PanelControl panelControl1;
private DevExpress.XtraEditors.SimpleButton btnKaydet;
private DevExpress.XtraEditors.LabelControl labelControl1;
private DevExpress.XtraBars.Ribbon.RibbonControl ribbonControl1;
private DevExpress.XtraBars.BarButtonItem barButtonKaydet;
```

**Gövde sırası tablosu — bu sıra ihlal edilmez:**

| # | Blok | Zorunlu | İhlalin sonucu |
|---|---|---|---|
| 1 | `resources` + `components` | Evet | `ArgumentException: Palet/ikon bulunamadı` |
| 2 | `new` ile alanların kurulması | Evet | `NullReferenceException` ilk çizimde |
| 3 | `BeginInit()` / `SuspendLayout()` | Kontroller `ISupportInitialize` ise | `InvalidOperationException: tasarım zamanı modu` |
| 4 | `Controls.Add` → **ebeveyn → çocuk** sırası | Evet | kontrol çizilmez ama hata da vermez |
| 5 | Özellikler (kendi bloğunda) | Evet | VS tasarımcısı her kayıttan sonra sırayı değiştirir |
| 6 | `Location`/`Size` **her zaman** yazılır | Evet | farklı DPI'da konum kayar |
| 7 | `TabIndex` artan ve benzersiz | Evet | `Tab` tuşu kontrolleri yanlış sırayla gezer |
| 8 | `ResumeLayout(false)` + `PerformLayout()` | Evet | ilk boyutta Layout döngüsü çalışmaz |
| 9 | Alan tanımları | Evet | `CS0103: The name 'x' does not exist` |

> **DPI notu.** `AutoScaleDimensions` + `AutoScaleMode` ikilisi olmadan form %125/`%150`
> ekranlarda kırpılır. Bu iki satır hiçbir koşulda silinmez; `Designer`'ı elle düzenleyen
> herkes bunları en üste değil, `ClientSize` hemen öncesine koyar (VS üretimi de oradadır).

#### §3.2.2 `Dispose(bool)` temizliği

`Dispose(bool)` **VS üretimidir ve elle dokunulmaz.** El ile kaynak bırakmak istenirse,
eklenti nesneleri `components` kapsayıcısına verilerek onun `Dispose`'una devredilir.

```text
protected override void Dispose(bool disposing)
{
    if (disposing && (components != null))
    {
        components.Dispose();      // kapsayıcıya verilen TÜM eklentiler burada bırakılır
    }
    base.Dispose(disposing);
}
```

| # | Durum | Ne yapılır | Neden |
|---|---|---|---|
| 1 | Görsel/veri bileşeni (`Timer`, `ImageList`, `BindingSource`) | `new X(this.components)` ile kur | `components.Dispose()` onu da bırakır |
| 2 | IDisposable ama eklenti olmayan nesne | alan tut, `Dispose(bool)` **içine elle ekle** | aksi hâlde sızıntı |
| 3 | Event'e `+=` yapılmış harici nesne | form kapanışında `-=` | aksi hâlde unsubscribe olmaz, form yaşar |
| 4 | `components == null` ise | hiçbir şey yapma | tasarımcı henüz `InitializeComponent` çağırmamış |
| 5 | `Dispose` içine iş mantığı | **yazma** | temizlik ≠ davranış; davranış `<Ad>.cs`'te |

**Yaygın sızıntı deseni.** `ImageList`/`Timer` `new Timer()` ile kurulup `components`'a
verilmezse form her açılış-kapanışta bir timer bırakır; 50 açılıştan sonra çalışma zamanı
belleği yükselir ve çökme sessizce olur. Doğrusu: `this.timer1 = new Timer(this.components);`

#### §3.2.3 Alan tanımları ve adlandırma

| # | Kural | İyi | Kötü |
|---|---|---|---|
| 1 | Alanlar `private`, `InitializeComponent`'ten sonra | `private SimpleButton btnKaydet;` | `public` alan → dışarıya tasarım sırrı sızar |
| 2 | Tür adı + rol adı | `btnKaydet` · `txtMusteri` · `gridSiparis` | `button1` → 3 ay sonra okunmaz |
| 3 | Benzersizlik tek dosyada | `btnKaydet` bir kez | aynı ada iki alan → `CS0102` |
| 4 | `Name` = alan adı | `this.btnKaydet.Name = "btnKaydet"` | `Name` != alan adı → olay yolu şaşar |
| 5 | Alan adı **yeniden adlandırılmaz** (VS değilse) | elle değiştirme yok | VS tasarımcısı `Name`'i esas alır, yeniden üretir |

### §3.3 RibbonForm sablonu (ana pencere)

```text
public partial class AnaForm : RibbonForm
{
    public AnaForm()
    {
        InitializeComponent();
    }
}

// Designer.cs icinde:
// ribbonControl1.RibbonStyle = RibbonControlStyle.Office2010;
// RibbonPage + RibbonPageGroup + RibbonButtonItem sirasi
```

| # | Öğe | Not |
|---|---|---|
| 1 | `RibbonForm` | Ribbon'ın en iyi taşıyıcısı (DevExpress resmi) |
| 2 | `RibbonControlStyle.Office2010` | enum geçerli; **açıkça** set edilir |
| 3 | `Default` | v17.1+ = Office2013 — istenen stil bu değilse sakat |
| 4 | Sürüm | vault G3 = **v26.1** (29.07.2026 yayında) — `CONFLICT DETECTED` için [keys](../keys.md) ve 12-rapor §6 |

## §4 Kurallar

### §4.1 RED sinyalleri

| # | Sinyal | Düzeltme |
|---|---|---|
| 1 | UI kodu `<Ad>.cs` içinde | Designer.cs'e taşı |
| 2 | Event gövdesi Designer.cs'te | `<Ad>.cs`'e taşı |
| 3 | `class Ad : Form` (anapencere) | `RibbonForm` + `RibbonControlStyle.Office2010` |
| 4 | Tek dosyalı form (Designer yok) | üçlüyü oluştur |
| 5 | Sürüm 26.1 (G3 tek sürüm; 25.x RED) | `Directory.Packages.props` tek DevExpress satırı |

### §4.2 Kontrol yerleşim kuralları (Anchor · Dock · Padding)

Yerleşim üç araçtan biriyle yapılır; **ikisi aynı kontrolde birlikte kullanılmaz.** `Dock` hiyerarşiyi
kurar, `Anchor` serbest ölçüyü bağlar, `Padding` iç boşluğu verir. Aynı kontrolde hem `Dock` hem
`Anchor` atanması, VS tasarımcısının her kayıttan sonra konumu geri almasına yol açar.

| # | Araç | Ne bağlar | Kullanım yeri | Kullanılmadığı yer |
|---|---|---|---|---|
| 1 | `Dock = Fill` | genişlik **ve** yükseklik ebeveyne | tablo/grid, ana içerik paneli | sabit boyutlu buton |
| 2 | `Dock = Top/Bottom/Left/Right` | tek eksen sabit, diğeri dolu | araç çubuğu, durum çubuğu, yan panel | düğme grubu |
| 3 | `Anchor = Top, Bottom, Right` | sağ kenara mesafe sabit | pencere yeniden boyutlandığında sağda kalacak buton | tam dolu panel |
| 4 | `Anchor = Top, Left` (varsayılan) | sol üstte sabit | etiket, sabit simge | — |
| 5 | `Anchor = None` | hiçbiri; ortalı | tam ortada logo/dönendirici | form içeriğinin tamamı |
| 6 | `Padding` (kontrol) | iç boşluk | `Dock=Fill` panelin çocuklarını kenardan koruma | dış boşluk |
| 7 | `Margin` (ebeveyn-çocuk) | kardeş kontroller arası | `TableLayoutPanel` içinde | mutlak konumlu form |
| 8 | `AutoScroll = true` | taşan içerikte kaydırma | değişken sayıda kontrol olan panel | sabit form |

**Karar ağacı — kontrol başına tek yol:**

1. Kontrol, ebeveyni yeniden boyutlandığında **tam dolacak** mı? → `Dock = Fill`. Durdu.
2. Değilse, **bir kenara yapışık** mı kalacak? → `Dock` + uygun `Anchor`. Durdu.
3. Hiçbiri değilse, **serbest** mi konumlanacak? → `Anchor` ile en az iki kenara bağla.
4. Sadece `Top, Left` kalıyorsa → pencere büyüdüğünde kontrol sol üstte asılı kalır; bu
   genellikle **istenen şey değildir**. Bunu bilerek yapıp yapmadığını satır notuyla yaz.

**Yerleşim sabitleri (varsayılanlar):**

| Değer | Standart | Not |
|---|---|---|
| Form iç kenar boşluğu | `Padding = 8, 8, 8, 8` | VS varsayılanı; 4'ten küçük okunmaz |
| Kontrol arası yatay | `Margin = 6, 3, 6, 3` | dikeyde 3, yatayda 6 |
| Standart buton boyu | `98 × 28` (DevExpress: `28`) | yükseklik 28'den küçükse tıklama alanı AA'yı geçemez |
| Sekme (Tab) yüksekliği | `24` (min) | dokunmatikte `32` önerilir |
| Grid satır yüksekliği | `22` (varsayılan) | `%125 DPI`'da `27,5` olur — elle sabitleme |
| Yazı tipi | `AutoScaleMode.Font` + `9F` | `AutoScaleMode.Dpi` yalnızca sabit DPI projesinde |

**Kırılma noktaları.** Şu üç durumda yerleşim elle değil, düzenleyiciyle çözülür:

- `MinimumSize` verilmiş bir formda `Dock=Fill` çocuk, küçük ekranda kırpılır → `AutoScroll`.
- `SplitContainer` içinde iki panel de `Dock=Fill` ise hiyerarşi çöker → biri `None`.
- `TableLayoutPanel` hücresinde `Anchor` vermek anlamsızdır; hücre zaten düzenler → `Anchor` yok.

### §4.3 Event bağlama kuralları

Event **bağlama** (subscribe) ile **gövde** (handler) farklı dosyalarda yaşar. Bu ayrım tek
kuralda özetlenir:

> `+=` satırı `Designer.cs`'te olur **çünkü VS onu üretir**; gövde `<Ad>.cs`'te olur
> **çünkü VS onu siler.**

| # | Kural | Doğru | Yanlış |
|---|---|---|---|
| 1 | Bağlama satırı | `this.btn.Click += new EventHandler(this.btn_Click);` | `<Ad>.cs` içinde `btn.Click += ...` |
| 2 | Gövde | `<Ad>.cs` → `private void btn_Click(object sender, EventArgs e) {}` | `Designer.cs` içinde gövde |
| 3 | Aynı olaya iki yerde bağlama | yasak — iki kez tetiklenir | `Form_Load` hem `Designer` hem `cs`'te |
| 4 | Koşullu bağlama | `<Ad>.cs` constructor'ında `if` ile | `Designer.cs`'te `if` → VS siler |
| 5 | `-=` (unsubscribe) | yalnız `<Ad>.cs`'te, `FormClosed`/`Dispose` yolunda | `Designer.cs`'te |
| 6 | Lambda / anonim metod | **yasak** — `-=` yapılamaz, yakalanamaz | `btn.Click += (s,e) => ...` |
| 7 | `async void` handler | yalnız UI olayında ve `try/catch` içerir | gövdesiz `async void` → çökme |
| 8 | Yeniden giriş | iş uzunsa `_busy` kilidi | ikinci tıklama ikinci işi başlatır |

**Zorunlu olay seti (her form):**

| Olay | Nerede bağlanır | Ne yapar |
|---|---|---|
| `Load` | `Designer.cs` (VS üretir) | ilk veri yükleme — **iş mantığı `<Ad>.cs` içinde** |
| `Shown` | `Designer.cs` | modal akış, odak verme |
| `FormClosing` | `Designer.cs` | kaydedilmemiş değişiklik uyarısı |
| `FormClosed` | `Designer.cs` | `-=` temizliği, kaynak bırakma |
| `Resize` | yalnız gerekiyorsa | sık tetiklenen → `SuspendLayout` içinde çalıştır |
| `KeyPress` / `TextChanged` | yalnız gerekiyorsa | debounce ekle, her tuşta sorgu çekme |

**Handler gövdesi iskeleti:**

```text
private void btnKaydet_Click(object sender, EventArgs e)
{
    if (_busy) return;                       // yeniden giriş kilidi
    _busy = true;
    btnKaydet.Enabled = false;
    try
    {
        if (!Dogrula()) return;              // doğrulama önce
        Kaydet();                            // iş
    }
    catch (Exception ex)
    {
        XtraMessageBox.Show(ex.Message, "Kayıt hatası",
            MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
    finally
    {
        _busy = false;
        btnKaydet.Enabled = true;            // her yolda geri açılır
    }
}
```

### §4.4 Kaynak (`.resx`) kullanımı

`.resx` dosyasını **yalnızca Visual Studio üretir ve yönetir.** Elle `XML` düzenlemek, tasarım
zamanında `InvalidOperationException` üretir ve dosya bozulursa form hiç açılmaz.

| # | Ne koyulur | Nereye | Elle dokunma |
|---|---|---|---|
| 1 | Form ikonu | `<Ad>Form.resx` → `Icon` | Hayır |
| 2 | Resim / backgroundImage | `.resx` → `System.Drawing.Bitmap` | Hayır |
| 3 | Yerelleştirilmiş metin | `.resx` → `"$this.Text"` anahtarı | Hayır |
| 4 | Kültür varyantları | `.<kültür>.resx` (VS üretir) | Hayır |
| 5 | Küçük sabit renk/ölçü | kod içinde (palet sabiti) | Evet |
| 6 | Büyük dizi veri | `.resx` değil, veri katmanı | — |

**Kural.** `.resx` 0 bayt değilse içeriği **elle değiştirilmez**; değişiklik gerekiyorsa form
tasarımcısında yapılır ve kaydedilir. `resx` içinde `"$this.Icon"` anahtarının kaybolması,
formun varsayılan pencere simgesiyle açılması demektir — bu bir hata değil, bir ipucudur
(`§6.3` → H3).

### §4.5 Renk, gradyan ve palet kuralları

Renk sabittir ve **tek yerden** okunur. Form içinde dağılmış `Color.FromArgb(45,45,48)`
değeri, ikinci formda farklı gri üretir ve tema değiştiğinde 12 dosyayı bulmak gerekir.

| # | Rol | Değer | Kullanım |
|---|---|---|---|
| 1 | Zemin (açık tema) | `Color.FromArgb(250, 250, 252)` | form ve panel `BackColor` |
| 2 | Yüzey (karta benzeyen panel) | `Color.White` | `PanelControl` |
| 3 | Ayırıcı / kenar | `Color.FromArgb(224, 224, 228)` | `Border`, `Splitter` |
| 4 | Birincil metin | `Color.FromArgb(32, 32, 36)` | etiket, grid hücresi |
| 5 | İkincil metin | `Color.FromArgb(120, 120, 128)` | ipucu, birim, zaman damgası |
| 6 | Vurgu (accent) | `Color.FromArgb(0, 120, 215)` | seçili satır, odak halkası |
| 7 | Başarı | `Color.FromArgb(16, 124, 65)` | onay rozeti |
| 8 | Uyarı | `Color.FromArgb(157, 108, 0)` | sarı rozet |
| 9 | Hata | `Color.FromArgb(196, 43, 28)` | hata metni, hata çerçevesi |
| 10 | Koyu tema zemin | `Color.FromArgb(32, 32, 36)` | yalnız `Office2010` + koyu paletle |

**Kontrast zorunluluğu (WCAG 2.1 AA):**

| Eşleşen çift | Oran | Eşik |
|---|---|---|
| Zemin (1) + Birincil metin (4) | ≈ 15,8:1 | AA 4,5 ✓ · AAA 7 ✓ |
| Zemin (1) + İkincil metin (5) | ≈ 4,6:1 | AA 4,5 ✓ · AAA 7 ✗ |
| Vurgu (6) + Beyaz metin | ≈ 4,6:1 | AA ✓ (yalnız ≥14pt kalın değilse dikkat) |
| Hata (9) + Beyaz metin | ≈ 5,2:1 | AA ✓ |

İkincil metin oranı sınırda olduğu için **%100 opaklık** kullanılır; `%70` opak `ForeColor`
oranı düşürür ve AA'yı kırar. Opaklık ile soluklaştırma yapılmaz.

**Gradyan.** DevExpress tema motoru gradyanı kendisi üretir; elle gradyan yalnızca iki yerde
kabul edilir:

| # | Yer | Araç | Not |
|---|---|---|---|
| 1 | Başlık bandı | `DevExpress.Utils.Skinner` / `Appearance` | `ForeColor`+`BackColor` ikilisi |
| 2 | İlerleme çubuğu | `ProgressBarControl` `ProcessingFill` | `%0 → %100` geçişi tek yönlü |

Elle çizilen `LinearGradientBrush` **RED**dir; tema değiştiğinde kaybolur ve koyu temada
okunmaz kalır.

### §4.6 Guardrail eşlemesi (G1..G12)

Bu şablon hangi guardrail'i hangi bölümde uygular — tek tablo:

| G | Vault özeti | Bu şablonda nerede | Denetimi |
|---|---|---|---|
| G1 | Harici vault'tan içerik kopyalanamaz, yalnız yapı referansı | §7 yalnız bağlantı verir, metin kopyalamaz | `§6.1` K6 |
| G2 | EF Core yasak; ADO.NET + Dapper, parametreli sorgu | §2.2 kapsam dışı #2 | `§6.1` K7 |
| G3 | Frontmatter'sız Markdown yasak (8 alan) | satır 1-10, `birebir` korunur | `§6.1` K1 |
| G4 | Üretilen dosyada yer tutucu değişken kalmaz (0 adet) | bu dosyada yer tutucu yok | `§6.1` K2 |
| G5 | Türkçe zorunlu; mojibake yasak | `§6.1` K3 | `§6.1` K3 |
| G6 | < 480 satırlık şablon reddedilir | §6.1 K4 — eşik 480 satır / 26 KB | `§6.1` K4 |
| G7 | Bozuk bağlantı yasak; silme/overwrite onay ister | §7 bağlantıları diskte doğrulanır | `§6.1` K5 |
| G8 | `GATE-003` onaysız `src/**` kodu üretilmez | §5.1 ön koşul — onay alınmadan `.cs` yazılmaz | `§6.1` K8 |
| G9 | ADR'siz mimari karar yok; kanıtsız sayı yazılmaz | §6.2 kanıt satırı zorunlu | `§6.1` K9 |
| G10 | Concat SQL yasak; `@p1..@pN` zorunlu | §2.2 kapsam dışı #1 — form SQL yazmaz | `§6.1` K10 |
| G11 | `TreatWarningsAsErrors=true` + %90 coverage | §5.7 derleme = 0 uyarı | `§6.1` K11 |
| G12 | Envantere içerik kopyalanamaz (SRP) | §7 komşu şablona yalnız referans | `§6.1` K12 |

Kaynak: [CLAUDE.md](../../CLAUDE.md) · [templates index](index.md).

### §4.7 Kısa yasaklar listesi

| # | Yasak | Karşılığı |
|---|---|---|
| 1 | `<Ad>.cs` içine `InitializeComponent` benzeri UI kodu yazmak | §3.1 |
| 2 | `Designer.cs` içine iş mantığı / `MessageBox` yazmak | §4.3 |
| 3 | `.resx`'i metin editörüyle düzenlemek | §4.4 |
| 4 | Aynı kontrolde `Dock` + `Anchor` birlikte | §4.2 |
| 5 | Lambda ile event bağlamak | §4.3 #6 |
| 6 | Renk sabitini form içinde dağıtmak | §4.5 |
| 7 | Elle `LinearGradientBrush` çizmek | §4.5 |
| 8 | `Dispose(bool)` içine iş mantığı koymak | §3.2.2 #5 |
| 9 | `components`'a verilmeyen `IDisposable` tutmak | §3.2.2 #2 |
| 10 | Tasarlayıcıyı açmadan `Designer.cs`'i elle değiştirmek | §5.3 |
| 11 | `async void` handler gövdesiz bırakmak | §4.3 #7 |
| 12 | `src/**` altına `GATE-003` onaysız satır yazmak | G8 |

## §5 Workflow

Beş fazlı ve **geri dönüşlü**dür; her fazın sonunda bir sonraki faza geçmeden önce
`§6.1` denetiminden en az ilgili satırlar `PASS` olmalıdır.

### §5.1 Faz 0 — ön kontroller (2 dk)

1. `GATE-003` onayı alındı mı? **Hayır** → dur (`G8`).
2. `Directory.Packages.props` içinde tek DevExpress satırı var mı? (`G3`)
3. UI projesi `net48` **ve** `net10.0-windows` içeriyor mu? (`G6`)
4. `TreatWarningsAsErrors` `true` mı? (`G11`)

Dört sorudan biri "hayır" ise şablon uygulanmaz; önce ortam düzeltilir (`§2.3`).

### §5.2 Faz 1 — dosya üçlüsünü oluştur (3 dk)

1. `<Ad>Form.cs` · `<Ad>Form.Designer.cs` · `<Ad>Form.resx` aynı anda oluşturulur.
2. `partial class <Ad>Form` her üç dosyada aynı ad ve aynı `namespace`.
3. `.cs` içinde constructor yalnız `InitializeComponent();` çağırır.
4. `.Designer.cs` içinde `Dispose(bool)` ve `InitializeComponent()` vardır.
5. `.cs` dosyası `System.Windows.Forms` / `DevExpress` `using` bloğunu içerir.

### §5.3 Faz 2 — yerleşim (8 dk)

1. Önce hiyerarşi: `Dock` sırası ebeveyn → çocuk, **outer → inner**.
2. Sonra ölçüler: `Location`/`Size` her kontrolde açıkça yazılır.
3. En son `Anchor`: karar ağacını (`§4.2`) kontrol başına bir kez işlet.
4. `TabIndex` 0'dan başlar, formdaki görsel okuma sırasıyla artar.
5. Tasarımcıyı **bir kez aç, kaydet**: VS sırayı kendi standardına çeker. Elle yazdığın
   sıra ile VS'nin ürettiği sıra arasındaki fark, `git diff`'te görünür kalmamalıdır.

### §5.4 Faz 3 — olaylar (6 dk)

1. `+=` satırları `Designer.cs`'e, gövdeler `<Ad>.cs`'e.
2. Zorunlu olay seti (`§4.3`): `Load` · `Shown` · `FormClosing` · `FormClosed`.
3. Her handler'da yeniden giriş kilidi ve `finally` ile geri açma.
4. `-=` temizliği `FormClosed` yolunda.

### §5.5 Faz 4 — Ribbon (5 dk)

1. Anapencere `RibbonForm` ise `RibbonControl.RibbonStyle = Office2010` **açıkça** yazılır.
2. Sıra: `RibbonControl` → `RibbonPage` → `RibbonPageGroup` → `RibbonButtonItem`.
3. `RibbonPage` sayısı 1-5 arası; 6. sayfa bilgi mimarisidir, Ribbon değil.
4. `ItemClick` olaylarının gövdeleri ana form `.cs`'inde.

### §5.6 Faz 5 — kaynak ve palet (4 dk)

1. İkon/görsel `.resx`'e VS üzerinden eklenir (`§4.4`).
2. Renkler palet sabitlerinden okunur (`§4.5`), forma hiç sabit yazılmaz.
3. Kontrast oranı AA'yı karşılıyor mu — tabloya bak, tahmin etme.

### §5.7 Faz 6 — derleme ve teslim (4 dk)

1. `dotnet build` → **0 uyarı**, 0 hata (`G11`).
2. Tasarımcıyı aç → kaydet → `git diff` beklenen dosyalar dışında **boş** olmalı.
3. `§6.1` denetimlerini çalıştır, `PASS` satırlarını `KANIT` olarak yaz (`G9`).
4. Dosya değişikliklerini `session-log` şablonundaki tabloya işle
   ([session-log](session-log-template.md)).

**Toplam:** ~32 dakika (form başı). Fazlar atlanamaz; sıçrama en sık H1/H2 hatalarını üretir.

## §6 Doğrulama

### §6.1 Salt-okunur PowerShell denetimleri

Hiçbiri dosyaya **yazmaz** — yalnız okur ve `PASS`/`FAIL` döndürür. Denetimler sırasıyla
çalıştırılır; çıktı doğrudan kanıt satırına yapıştırılır.

| # | Ne ölçülür | Komut (salt-okunur) | Beklenen |
|---|---|---|---|
| K1 | Frontmatter 8 alan (G3) | `(Get-Content $f -Encoding UTF8 \| Select-Object -First 10 \| Select-String -Pattern '^(title\|type\|category\|status\|version\|updated\|authority\|scope):').Count` | `8` |
| K2 | Yer tutucu yok (G4) | `([regex]::Matches((Get-Content $f -Raw -Encoding UTF8),'\{\{')).Count` | `0` |
| K3 | Mojibake yok (G5) | `([regex]::Matches((Get-Content $f -Raw -Encoding UTF8),'[Ç\u00C3].\|[Â\u00E2][\u20AC]')).Count` | `0` |
| K4 | Satır / boyut (G6) | `(Get-Content $f -Encoding UTF8).Count` ve `((Get-Item $f).Length/1KB)` | `≥ 480` · `≥ 26` |
| K5 | Bozuk bağlantı yok (G7) | her `](hedef)` için `Test-Path` | `0` kırık (yenileri) |
| K6 | Harici kopya yok (G1) | §7'de blok metin değil, yalnız bağlantı | `0` blok |
| K7 | EF Core yok (G2) | `Select-String -Path $f -Pattern 'EntityFramework\|DbContext'` | `0` |
| K8 | `GATE-003` (G8) | süreç kaydında onay satırı var mı | `VAR` |
| K9 | Kanıt satırı (G9) | `Select-String -Path $f -Pattern 'KANIT'` | `≥ 1` |
| K10 | Concat SQL yok (G10) | `Select-String -Path $f -Pattern '" \+ \|SELECT .*\$\{'` | `0` |
| K11 | Uyarı yok (G11) | `dotnet build` stderr satır sayısı | `0` |
| K12 | SRP (G12) | komşu şablona yalnız `](...)` referans | `0` gömülü blok |

**BOM denetimi** (UTF-8 BOM yasak):

```text
$b = [System.IO.File]::ReadAllBytes($f)[0..2] -join ','
if ($b -eq '239,187,191') { 'BOM=VAR  -> RED' } else { 'BOM=YOK  -> PASS' }
```

**Tek satırlık toplu denetim** (bu dosyanın tamamı için):

```text
$f = 'C:\versacoder\.ai\.templates\winform-form-designer-template.md'
$b = [System.IO.File]::ReadAllBytes($f)[0..2] -join ','
"satır={0}  KB={1:N1}  BOM={2}  mojibake={3}  yer-tutucu={4}" -f `
  (Get-Content $f -Encoding UTF8).Count, ((Get-Item $f).Length/1KB), `
  $(if ($b -eq '239,187,191') {'VAR'} else {'YOK'}), `
  ([regex]::Matches((Get-Content $f -Raw -Encoding UTF8), '[Ç\u00C3].|[Â\u00E2][\u20AC]')).Count, `
  ([regex]::Matches((Get-Content $f -Raw -Encoding UTF8), '\{\{')).Count
```

### §6.2 Kanıt satırı biçimi

Doğrulama sonucu, [session-log](session-log-template.md) §3.12'deki kanonik biçimle aynıdır;
bu dosyada da tek satır olarak yazılır:

```text
KANIT | kod-yazici | ekledigim-satir-sayisi=<n> | agirlik=orta | suresi=<s> | hata=0 |
kanit-dosya=".ai/.templates/winform-form-designer-template.md" | kanit-satir=<a>-<b>
```

`<a>-<b>` aralığı **gerçek** satır numaralarıdır; uydurulan aralık `G9` ihlalidir. İddia
okunmadıysa kanıt yerine `⚠️ VERIFICATION REQUIRED` + gerekçe yazılır.

### §6.3 Hata modları ve çözümleri

| # | Belirti | Kök neden | Çözüm |
|---|---|---|---|
| H1 | Tasarımcı açılmıyor: *"Failed to create component"* | `InitializeComponent` içinde çalışma zamanı çağırmak (`DateTime.Now`, dosya okuma) | yalnız `new` + özellik atama; davranışı `<Ad>.cs`'e taşı |
| H2 | Tasarımcı açılmıyor: *"class not found"* / `CS0246` | `partial` sınıf adı ya da `namespace` üç dosyada farklı | üç dosyayı birebir aynı `namespace` + `partial class <Ad>Form` yap |
| H3 | Form açılıyor ama ikon/metin yok | `.resx` bozulmuş ya da `resources` satırı silinmiş | `resources` satırını `InitializeComponent` başına geri koy; VS'de dosyayı yeniden kaydet |
| H4 | `InvalidOperationException` (*design-time*) | `ISupportInitialize` kontrol için `BeginInit`/`EndInit` eksik | `((ISupportInitialize)(this.x)).BeginInit()` çiftini tamamla |
| H5 | Tasarımcı her kayıttan sonra dosyayı değiştiriyor | elle yazılan sıra VS sırasıyla çelişiyor (`§3.2.1`) | bir kez tasarlayıcıyı aç-kaydet, sonra `git diff`'i temizle |
| H6 | Form %150 DPI'da kırpık | `AutoScaleDimensions`/`AutoScaleMode` silinmiş | iki satırı `ClientSize` öncesine geri koy |
| H7 | `NullReferenceException` ilk çizimde | alan tanımı `new` edilmemiş ya da `Controls.Add` sırası ters | `§3.2.1` blok 2 ve blok 4 sırasına dön |
| H8 | Kontrol hiç görünmüyor | `Dock=Fill` ebeveyni 0 yüksekliğinde ya da `Visible=false` | ebeveyn ölçüsünü ve `z-order`'ı kontrol et |
| H9 | `Tab` tuşu yanlış geziyor | `TabIndex` tekrarı ya da eksik | tüm `TabIndex`'leri 0'dan benzersiz kur |
| H10 | Olay iki kez tetikleniyor | aynı `+=` hem `Designer.cs` hem `<Ad>.cs`'te | yalnız `Designer.cs`'te bırak |
| H11 | Bellek form kapandıktan sonra düşmüyor | `components`'a verilmemiş `Timer`/`ImageList` | `new X(this.components)` (`§3.2.2`) |
| H12 | Koyu temada metin okunmuyor | elle atanmış sabit renk | palet sabitlerine dön (`§4.5`) |

**Hata modu okuma sırası.** Belirti önce `H1/H2` ile elenir (tasarımcı hiç açılmıyorsa),
sonra `H4/H7` (açılıyor ama çizilmiyorsa), en son `H5/H6` (açılıyor ama bozuksa). Sıra
tersine çevriliyorsa saatler `H5`'te kaybedilir.

### §6.4 Kabul listesi

Teslimden önce hepsi `PASS` olmalıdır:

- [ ] 3 dosya var, `partial class` adları birebir aynı.
- [ ] `InitializeComponent` blok sırası `§3.2.1` tablosuyla uyumlu (9/9).
- [ ] Gövdeler `<Ad>.cs`'te; `Designer.cs`'te gövde yok.
- [ ] `Dock` + `Anchor` aynı kontrolde birlikte yok.
- [ ] `.resx` elle değiştirilmedi; ikon VS üzerinden tanımlı.
- [ ] Renkler palet sabitlerinden geliyor; form içine sabit yazılmamış.
- [ ] Kontrast oranları `§4.5` tablosunda AA'yı karşılıyor.
- [ ] `dotnet build` → 0 uyarı, 0 hata.
- [ ] Tasarımcı aç-kaydet sonrası `git diff` yalnız beklenen dosyalarda.
- [ ] `§6.1` K1..K12 satırları `PASS`; kanıt satırı yazıldı.
- [ ] `BOM=YOK`, mojibake=`0`, yer tutucu=`0`.
- [ ] `GATE-003` onayı kayıtta görünüyor.

## §7 Referanslar

### §7.1 İç bağlantılar (diskte doğrulanmış)

| # | Bağlantı | Ne için |
|---|---|---|
| 1 | [index](../index.md) | şablon envanteri ve kalite sözleşmesi |
| 2 | [CLAUDE](../CLAUDE.md) | `.templates/` klasör kuralları |
| 3 | [templates CLAUDE](CLAUDE.md) | G1..G12 guardrail tablosu |
| 4 | [new-doc-contract](new-doc-contract.md) | yeni doküman sözleşmesi (8 alan frontmatter) |
| 5 | [platform-arastirma](../reports/12-platform-arastirmasi.md) | `WEB_VERIFIED` kanıt raporu |
| 6 | [session-log](session-log-template.md) | oturum kaydı, kanıt satırı biçimi |
| 7 | [adr-index](adr/adr-index.md) | mimari karar kayıtları |
| 8 | [winforms-form](other/winforms-form-template.md) | komşu: form şablonu |
| 9 | [ribbon](frontend/ribbon-template.md) | komşu: Ribbon şablonu |
| 10 | [form](frontend/form-template.md) | komşu: form şablonu |
| 11 | [root-md](documentation/root-md-template.md) | komşu: kök MD şablonu |
| 12 | [CLAUDE kök](../../CLAUDE.md) | G1..G12 tek satır özet (B6 tablosu) |
| 13 | [AGENTS](../../AGENTS.md) | ajan dosyası, kapı kuralı |
| 14 | [WORKFLOW](../../WORKFLOW.md) | kapı ve faz akışı |

### §7.2 Dış kaynaklar (WEB_VERIFIED)

| # | Kaynak | Kullanılan iddia | Tarih |
|---|---|---|---|
| 1 | `github.com/dotnet/winforms` — `docs/designer` | `InitializeComponent` VS tarafından yeniden üretilir; `Dispose(bool)` üretilir | 2026-10-04 |
| 2 | `learn.microsoft.com` — *Windows Forms Designer* | `ISupportInitialize` `BeginInit`/`EndInit` zorunluluğu | 2026-10-04 |
| 3 | `learn.microsoft.com` — *Localization with .resx* | `.resx` VS tarafından yönetilir | 2026-10-04 |
| 4 | `learn.microsoft.com` — *Improving Performance: Windows Forms* | `AutoScaleMode` ve DPI davranışı | 2026-10-04 |
| 5 | DevExpress dokümantasyonu — `RibbonForm` | Ribbon'ın en iyi taşıyıcısı `RibbonForm` | 2026-10-04 |
| 6 | DevExpress dokümantasyonu — `RibbonControlStyle` | `Default` = `Office2013` (v17.1+) | 2026-10-04 |
| 7 | `12-platform-arastirmasi` §4 ve §6 | sürüm 26.1 + `CONFLICT DETECTED` kuralı | 2026-10-04 |

### §7.3 Kanıt ve yetki

| # | Soru | Cevap |
|---|---|---|
| 1 | Bu şablonun yetkisi nedir? | `authority: SSOT` (frontmatter, `birebir`) |
| 2 | İddialar nereden geliyor? | §7.2 — hepsi `WEB_VERIFIED`, tarih damgalı |
| 3 | Çelişki çıkarsa ne olur? | `../keys.md` ve 12-rapor §6 kazanır; bu dosya güncellenir |
| 4 | Sürüm kaydı nerede? | frontmatter `version` + `updated` alanları |
| 5 | Bu dosya kimde yaşar? | `.ai/.templates/` — [index](../index.md) envanterinde |
