---
title: "WinForms — .resx Dosya Yapısı ve Dosya Üçlüsü Tutarlılığı"
type: knowledge
category: winforms
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
scope: "Her formun `.resx` dosyasının içinde ne olduğu, XML düzeninin nasıl korunduğu ve `.Designer.cs` ile nasıl senkron kaldığı"
---

# `.resx` Dosya Yapısı ve Üçlü Tutarlılığı

**Bağlantılar:** [index.md](index.md) · [../../CLAUDE.md](../../CLAUDE.md)

## §0 Özet (Answer First)

Her formun kendi adını taşıyan bir `.resx` dosyası vardır ve `.Designer.cs` içindeki `ComponentResourceManager(typeof(XxxForm))` bunu **tip adıyla** çözer. `.resx` **elle XML olarak düzenlenmez**; değişiklik Visual Studio tasarımcısı üzerinden yapılır. `ResourceManager` adı ya da tip adı bozulursa form `MissingManifestResourceException` ile açılmaz — bu `G3`/`G4` ihlali sayılır ve `.resx` son sözü `ui-designer`'dadır.

| # | Dosya | İçerik | Kim üretir |
|---|---|---|---|
| 1 | `SettingsForm.resx` | `$this.Text`, ikon, yerelleştirilmiş metinler, `Appearances` | tasarımcı |
| 2 | `SettingsForm.Designer.cs` | `resources.ApplyResources(this.$this, "$this")` çağrıları | tasarımcı |
| 3 | `SettingsForm.cs` | yalnız `InitializeComponent()` çağrısı | `developer` |
| 4 | `Properties/Resources.resx` | proje geneli ikon/metin (form ile karıştırılmaz) | tasarımcı |

### §1.1 `.resx` iskeleti

```xml
<?xml version="1.0" encoding="utf-8"?>
<root>
  <resheader name="resmimetype"><value>text/microsoft-resx</value></resheader>
  <resheader name="version"><value>2.0</value></resheader>
  <resheader name="reader"><value>System.Resources.ResXResourceReader, ...</value></resheader>
  <resheader name="writer"><value>System.Resources.ResXResourceWriter, ...</value></resheader>
  <data name="$this.Caption" xml:space="preserve">
    <value>Ayarlar</value>
  </data>
  <data name="btnSave.Caption" xml:space="preserve">
    <value>Kaydet</value>
  </data>
</root>
```

> **KURAL:** `<data name="...">` anahtarları, `.Designer.cs` içindeki `ApplyResources(this.btnSave, "btnSave")` çağrılarıyla **birebir** eşleşir. Anahtar değiştirilirse ilgili özellik sessizce varsayılana döner — hata yerine **yanlış görünüm** çıkar, bu da bulguyu zorlaştırır.

### §1.2 Tutarlılık denetimi

| # | Denetim | Beklenen | İhlal belirtisi |
|---|---|---|---|
| 1 | `.resx` dosyası var mı | form başına 1 adet | eksikse `ApplyResources` çalışmaz |
| 2 | `ComponentResourceManager(typeof(X))` tip adı | dosya adıyla aynı | tip ≠ dosya → `MissingManifestResourceException` |
| 3 | `InitializeComponent` tanımı | **1** adet | 2 tanım → CS0111 |
| 4 | `.resx` BOM/düzen | geçerli UTF-8 XML | bozuksa tasarımcı açılmaz |
| 5 | Elle düzenlenen satır | 0 | `ui-designer` dışındaki elle müdahale |

### §1.3 `.resx`'e elle dokunulduğunda ne olur

| Belirti | Neden | Düzeltme | Sorumlu |
|---|---|---|---|
| Tasarımcı dosyayı açmıyor | XML `&`/`<` kaçışı bozuk | dosyayı tasarımcıda yeniden kaydet | `ui-designer` |
| Metin görünmüyor | `<data name>` ≠ `ApplyResources` anahtarı | anahtarı geri al | `ui-designer` |
| Form açılmıyor | `typeof(X)` ≠ dosya adı | tip adını geri al | `ui-designer` |
| `developer` .resx'i ellemiş | yetki aşımı | RED → `ui-designer`'a devret | `reviewer` |

### §1.4 Proje geneli kaynak vs form kaynağı

| | Form `.resx` | `Properties/Resources.resx` |
|---|---|---|
| Kapsam | tek form | tüm proje |
| Erişim | `ComponentResourceManager` | `Properties.Resources.House` |
| Tipik içerik | başlık, buton metni, `Appearances` | ikon, splash görseli |
| Yetki | `ui-designer` | `ui-designer` |

## §2 Uygulama Kontrol Listesi

| # | Adım | Kanıt |
|---|---|---|
| 1 | Her formun `.resx`'i var ve geçerli XML | dosya + ilk satır |
| 2 | `typeof(XxxForm)` ↔ dosya adı eşleşiyor | grep |
| 3 | `.resx` üzerinde elle değişiklik yok (diff) | `git diff` |
| 4 | Form açılışında `MissingManifestResourceException` yok | çalıştırma kanıtı |
| 5 | İki bacakta `dotnet build` → 0 hata | build çıktısı |

## §3 İzinli Link Hedefleri

| # | Hedef | Bu dosyada |
|---|---|---|
| 1 | Kardeş dosya adları (bu dizindeki 30 dosya) | var |
| 2 | [index.md](index.md) — kayıt defteri | var |
| 3 | [../../CLAUDE.md](../../CLAUDE.md) — anayasa G1..G12 | var |
| 4 | `../../index.md` · `../../keys.md` · `../../VISION.md` · `../../.rules/*.md` · `../index.md` | **yok → link verilmez** (`NEEDS_VALIDATION`) |

## §4 Yasaklar

| # | Yasak | İhlalde |
|---|---|---|
| 1 | DevExpress **v26.1** dışı sürüm referansı (G3/G8) | iş RED |
| 2 | `RibbonControlStyle.Office2010` dışı ribbon stili (G3) | iş RED |
| 3 | EF Core · `DbContext` · string-concat SQL (G2) | CRITICAL |
| 4 | `.Form.cs`'e kontrol üretimi / `SuspendLayout` / `ResumeLayout` / `BeginInit` yazmak · `.resx`'i `ui-designer` dışında ellemek | form üçlüsü RED |
| 5 | Kanıtsız "bitti" (G5) · bozuk iç link (G4) | RED |
