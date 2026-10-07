---
title: "designer-editing-rules - VersaCoder"
type: knowledge
category: winforms
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: ".Designer.cs ve .resx'in elle düzenlenmesi, git merge çakışması çözümü, yetki sınırı (ui-designer) ve bozulma belirtileri"
---

# designer-editing-rules — Tasarım Dosyasını Elle Düzenleme Kuralları

**Zorunlu Baglantilar:** [vault index](../../index.md) · [coding-standards](../../.rules/coding-standards.md) · [winforms index](../index.md) · [designer-cs-anatomy](designer-cs-anatomy.md) · [form-triple-consistency-check](form-triple-consistency-check.md)

> **Tek cümlelik sorumluluk:** *`.Designer.cs` / `.resx` elle ne zaman ve kim tarafından düzenlenebilir, elle bir düzenleme bozulursa hangi belirtilerle anlaşılır ve çakışma nasıl çözülür* sorusunun tek cevabıdır.

---

## §0 Ozet (Answer First)

**`.Designer.cs` ve `.resx` elle düzenlenmez;** değişiklik Visual Studio tasarımcısında yapılır. Vault yetkisine göre bu iki dosyanın son sözü **`ui-designer`**'dadır; `developer` elle bir değişiklik girerse iş `RED` ile `ui-designer`'a devredilir. Elle müdahale **kaçınılmazsa** (git çakışması, merge) tek kural: **dosyanın bir tarafı bütünüyle alınır**, parçalı düzeltme yapılmaz, ardından form tasarımcıda **açılıp kaydedilerek** resx ile yeniden eşleştirilir. Parçalı düzenleme en sık iki hatayı üretir: üyede/kaynakta anahtar uyuşmazlığı (`MissingManifestResourceException`) ve kapanış bloğunun kaybı ile **sessizce bozulan layout**.

---

## §1 Yetki ve kural tablosu

| Durum | Kim yapar | Nasıl |
|---|---|---|
| Kontrol ekleme/değiştirme, konum/boyut | `ui-designer` | VS tasarımcısı (ChangeType/Properties penceresi) |
| `.resx` metin/görsel değişikliği | `ui-designer` | Tasarımcı properties → Text/Icon alanı |
| Formun görünüm kararı (DX kontrol seçimi) | `ui-designer` | [devexpress-rules](../../.rules/devexpress-rules.md) — v26.1 + Office2010 |
| Git merge çakışması | `developer` **geçici** → sonra tasarımçı | Dosyanın tek tarafı alınır, designer'da açılır |
| Runtime `Text = veri` ataması | `developer` | İş mantığıdır, tasarım dosyasına dokunmaz |

---

## §2 El ile mudahale belirtileri

| Belirti | Muhtemel neden | Düzeltme |
|---|---|---|
| `CS0102: ... zaten 'X' için tanım içeriyor` | Aynı üye iki kez (copy-paste) | Designer'ı aç, kaydet; gerekirse dosyayı geri al |
| `MissingManifestResourceException` | resx anahtarı ≠ `resources.GetString(...)` adı | Tasarımcıda formu aç → resx yeniden yazılır |
| Form açılmıyor: "Exception when opening designer" | Elle bozulmuş `InitializeComponent` bloğu | Dosyayı bilinen iyi commite al, tasarımcıda aç |
| Layout bozuk ama kod derleniyor | `ResumeLayout(false)` / `EndInit()` satırı kaymış | Elle ekleme — **dosyayı geri al** |
| `git conflict` içinde `.Designer.cs` | İki dal aynı dosyayı değiştirmiş | §3 akışını izle |
|resx içinde `<<<<<<<` | Çakışma işareti metne sızmış | XML geçersiz → dosyayı geri al |

> **NOT:** `ResumeLayout(false)` tek başına silinse bile derleme geçer — bu yüzden hata **derlemede değil, ekranda** çıkar. Kanıt zorunlu: formu **aç, çalıştır, gözle doğrula** (bkz. [CLAUDE.md](../../CLAUDE.md) G5).

---

## §3 Git cakismasi cozum akisi

```text
1. cakisan dosyanin TAMAMINI bir daldan al       (theirs  ya da ours - yarim YOK)
2. VS'te formu ac -> tasarimci dosyayi okur ve resx ile eslestirir
3. formu KAYDET (Ctrl+S) -> .Designer.cs + .resx birlikte yeniden yazilir
4. fark al (git diff) -> yalniz beklenen tasarimsal degisiklikler gorunmeli
5. uygulamayi calistir -> ekran dogrula (kanitsiz "bitti" YASAK)
```

| Adım | Kanıt |
|---|---|
| 1 | `git diff --stat` yalnız 2-3 tasarım dosyası gösteriyor |
| 2-3 | Tasarımcı hata vermiyor |
| 4 | Elle eklenmiş `SuspendLayout` satırı **yok** |
| 5 | Ekran görüntüsü ya da çalışan uygulama |

---

## §4 net48 / net10.0-windows farki

| Konu | `net48` | `net10.0-windows` |
|---|---|---|
| Tasarımcının yazdığı dosya biçimi | **aynı** (UTF-8, `resources` bloğu) | aynı |
| Tasarım zamanı host | .NET Framework designer host | .NET designer host — **dosya içeriği aynı** |
| Elle `#if` ekleme | YASAK her iki bacakta | YASAK |
| DPI/ölçekleme anahtarı (`$this.AutoScaleDimensions`) | Tasarımcı yazar (`autoscalemode-dpi-scaling`) | Aynı anahtar geçerli |

---

## §5 Kardesler

[designer-cs-anatomy](designer-cs-anatomy.md) · [form-triple-consistency-check](form-triple-consistency-check.md) · [partial-class-and-file-naming](partial-class-and-file-naming.md) · [resx-file-structure](resx-file-structure.md)
