---
title: "resx-file-structure - VersaCoder"
type: knowledge
category: winforms
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "XxxForm.resx iç yapısı: resheader, data name anahtarları, $this.*, base64 görsel, manifest adı ve bozulma belirtileri"
---

# resx-file-structure — `.resx` Dosyasının Yapısı

**Zorunlu Baglantilar:** [vault index](../../index.md) · [coding-standards](../../.rules/coding-standards.md) · [winforms index](../index.md) · [designer-cs-anatomy](designer-cs-anatomy.md) · [resx-resourcemanager](resx-resourcemanager.md)

> **Tek cümlelik sorumluluk:** *Bir form `.resx` dosyasında hangi anahtarlar bulunur, `ComponentResourceManager` bunları nasıl bulur ve anahtar kayarsa hangi hata çıkar* sorusunun tek cevabıdır.

---

## §0 Ozet (Answer First)

`.resx` = XML anahtar-değer deposudur; formun **görünüm bilgisi** (başlık, etiket metinleri, ikon, `$this.AutoScaleDimensions`) burada durur ve `XxxForm.Designer.cs` içindeki `ComponentResourceManager(typeof(XxxForm))` ile okunur. **Manifest adı** klasör + sınıf adından türetilir (`VersaCoder.UI.SessionListForm.resources`); sınıf adı değişirse `.resx` adı ve `typeof(...)` da değişmezse `MissingManifestResourceException` çıkar. Anahtar biçimi **`<uyye>.<ozellik>`** örneğin `lblBaslik.Text`, `$this.Text`, `$this.Icon` — elle değiştirilmesi (özellikle `Edit` ile açılan binary bloklar) resx'i geçersiz kılar.

---

## §1 Dosya yapisi

| Bölüm | Ne taşır | Elle düzenleme |
|---|---|---|
| `<resheader>` × 4 | mimetype / version / reader / writer | Yasak (şablon sabit) |
| `<data name="lblBaslik.Text">` | Etiket metni (`<value>`) | Tasarımcı properties penceresi |
| `<data name="$this.Text">` | Form başlığı | Tasarımcı |
| `<data name="$this.Icon">` | Pencere ikonu (binary/base64) | Tasarımcı → Icon seçici |
| `<data name="$this.AutoScaleDimensions">` | DPI ölçekleme ölçüsü | [autoscalemode-dpi-scaling](autoscalemode-dpi-scaling.md) |
| `<metadata>` | Görsel mime bilgisi (`bytearray.base64`) | Yasak |

```xml
<!-- SessionListForm.resx (parca) -->
<data name="lblBaslik.Text" xml:space="preserve">
  <value>Oturumlar</value>
</data>
<data name="$this.AutoScaleDimensions" type="System.Drawing.SizeF, System.Drawing">
  <value>96, 96</value>   <!-- 96 DPI bazi -->
</data>
```

---

## §2 Iliski tablosu (uclu)

| Dosya | Bu dosyaya referans |
|---|---|
| `SessionListForm.Designer.cs` | `ComponentResourceManager(typeof(SessionListForm))` → `SessionListForm.resx` |
| `SessionListForm.resx` | ad = sınıf adı, **aynı klasörde** |
| Assembly manifesti | `<namespace>.<class>.resources` (kaynak derleme sırasında gömülür) |
| `SessionListForm.cs` | resx'e **doğrudan dokunmaz** (runtime metin değişikliği kod-behind'ta ayrı kaynakla yapılır) |

| Hata | Neden | Düzeltme |
|---|---|---|
| `MissingManifestResourceException` | sınıf adı ↔ resx adı uyuşmuyor ya da resx derlenmemiş | Dosya adını/`typeof`'u eşleştir, derlemeyi temizle |
| `NullReferenceException` içinde `resources.GetString` | anahtar adı kaymış (`lblBaslik.Text` → `label1.Text`) | Tasarımcıda formu aç, kaydet |
| Tasarımcı "resource not found" | `.resx` başka klasöre taşındı | Üçlüyü birlikte taşı ([form-triple-consistency-check](form-triple-consistency-check.md)) |
| XML hatası satır N | Elle `<<<<<<<` ya da bozuk escape | Dosyayı geri al |

---

## §3 Kurallar

1. `.resx` **yalnız tasarımcı üzerinden** değişir (elle düzenleme → [designer-editing-rules](designer-editing-rules.md)).
2. Görsel/ikon **binary** bloğunu text editor'de kesmek dosyayı bozar; değişiklik properties'ten yapılır.
3. Aynı anahtar iki kez (`<data name>` tekrarı) resx'i geçersiz kılar → XML geçersiz hatası.
4. Metinleri **form resx'inde** tutmak tasarım kararıdır; uygulama geneli metinleri ayrı kaynak dosyasında tutulur ([resx-resourcemanager](resx-resourcemanager.md)).
5. Dosya **UTF-8** yazılır; `ş`/`ğ`/`İ` karakterlerinin `C3 B6` gibi iki bytelik artığa düşmesi (mojibake) G4 ihlalidir.

---

## §4 net48 / net10.0-windows farki

| Konu | `net48` | `net10.0-windows` |
|---|---|---|
| `.resx` XML biçimi ve `ResXResourceReader` | **aynı** | aynı |
| Manifest adlandırma kuralı | **aynı** | aynı |
| Culture-resx (`XxxForm.tr.resx`) | desteklenir | desteklenir — [localized-string-resources](localized-string-resources.md) |
| Uyumsuzluk riski | Sattellite derleme `EmbeddedResource` eşiği | Aynı kayıt kuralı; `#if` gerekmez |

---

## §5 Kardesler

[designer-cs-anatomy](designer-cs-anatomy.md) · [form-triple-consistency-check](form-triple-consistency-check.md) · [resx-resourcemanager](resx-resourcemanager.md) · [image-and-icon-resources](image-and-icon-resources.md)
