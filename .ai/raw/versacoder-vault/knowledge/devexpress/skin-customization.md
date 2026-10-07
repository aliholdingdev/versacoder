---
title: "skin-customization - VersaCoder"
type: knowledge
category: devexpress
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "DevExpress v26.1 skin ozellestirmesi: AppearanceObject katmanlari, paylasilan gorunum nesneleri, DX-08/DPI sinirlari ve iki bacak farki"
---

# skin-customization — Skin ve Appearance Özelleştirme (v26.1)

**Zorunlu Baglantilar:** [index](../index.md) · [devexpress-rules](../../.rules/devexpress-rules.md) · [ADR-004](../../.decisions/accepted/ADR-004-devexpress-universal-v261-office2010.md) · [theme-manager-office2010](theme-manager-office2010.md) · [gridview-columns-format](gridview-columns-format.md)

> **Tek cumlelik sorumluluk:** *Skin üzerine appearance nasıl yazılır ve nerede durulur* sorusunun tek cevabıdır.

---

## §0 Ozet (Answer First)

DevExpress görünümü üç katmandan oluşur: **skin** (global renk paketi) → **appearance** (kontrol/görünüm bazlı üstü yazma) → **format** ( hücre biçimi, `DisplayFormat` ). Özelleştirme hep **en üstten başlar**: önce global skin, sonra gerekirse appearance, en son hücre biçimi. Elle hücre çizimi (`CustomDrawCell`) **yasaktır** (DX-08) — DPI ve tema altında kırılır. Renk/font kodu `.Designer.cs`'de değil, kod tarafında ve tek yerde toplanır.

---

## §1 Katman Hiyerarsisi

| Katman | Kapsam | Nerede değişir | Ait olduğu rol |
|---|---|---|---|
| Skin | tüm uygulama | `UserLookAndFeel.Default` | `ui-designer` |
| Control appearance | tek kontrol | `ctrl.Appearance.*` | `ui-designer` |
| View appearance | görünüm bölgesi (Header, Row, SelectedRow) | `gridView1.Appearance.*` | `ui-designer` |
| Cell format | tek hücre değeri | `GridColumn.DisplayFormat` | `developer` |

```
skin  -->  appearance  -->  format
 (genel)    (ustune yaz)    (hucre bazli)
   |              |               |
   +--- hicbiri elle cizim degil: CustomDrawCell YASAK (DX-08)
```

---

## §2 Anahtar Ozellikler

| Ozellik | Ne is yarar | Tipik deger |
|---|---|---|
| `AppearanceObject.Font` | yazı tipi | `new Font("Segoe UI", 9f)` |
| `AppearanceObject.ForeColor` | yazı rengi | `Color.FromArgb(40, 40, 40)` |
| `AppearanceObject.BackColor` | zemin | `Color.FromArgb(245, 245, 245)` |
| `AppearanceObject.Options` | hangi alan **aktif** (yoksa skin ezer) | `AppearanceOptions.UseFont` |
| `AppearanceObject.Assign(src)` | görünümü kopyalama/paylaşma | `EvenRow.Assign(OddRow)` |
| `gridView1.AppearancePrint.*` | yazdırma/export görünümü | ayrı set — unutulmasın |

> **UYARI:** `Options` **önce** açılır, değer **sonra** atanır; tersi yapılırsa skin değeri ezer ve değişiklik görünmez.

---

## §3 Kod (appearance yazımı)

```csharp
using DevExpress.Utils;

// 1) Once alan maskesini ac, sonra degeri ver (skin uzerine yazar)
gridView1.Appearance.HeaderPanel.Options = AppearanceOptions.UseFont | AppearanceOptions.UseForeColor;
gridView1.Appearance.HeaderPanel.Font  = new Font("Segoe UI", 9f, FontStyle.Bold);
gridView1.Appearance.HeaderPanel.ForeColor = Color.FromArgb(40, 40, 40);

// 2) Ayni gorunumu paylas - tekrar yok (SRP)
gridView1.Appearance.EvenRow.Assign(gridView1.Appearance.OddRow);
```

| Yapilmayan | Neden |
|---|---|
| `CustomDrawCell` ile "stil" | DX-08 ihlali — DPI/tema altında bozulur |
| Her satırda aynı renk ataması | tekrar, tema değişince unutulur |
| `.Designer.cs` içinde renk kodu | Designer yeniden üretince silinir (DX-10) |

---

## §4 Kurallar (en fazla 5 madde)

1. Önce **skin** dene; appearance yalnız skin karşılamadığında.
2. `Options` maskesi olmadan renk/font **yazılmaz**.
3. Renkler sabit `Color.FromArgb` yerine **tek sınıfda** sabitlenebilir (dark/light geçişine hazır olsun).
4. Print/export görünümü ayrıdır (`AppearancePrint`) — eksik bırakılırsa rapor farklı görünür.
5. Elle çizim (`CustomDrawCell`) yerine appearance + `ColumnView` özellikleri (DX-08).

---

## §5 Iki Bacak (net48 / net10.0-windows)

| Konu | net48 | net10.0-windows |
|---|---|---|
| `AppearanceObject` API | aynı | aynı |
| Font adı | `"Segoe UI"` kurulu — net48'te ölçü birimi Font (pt) | C#/GDI+ aynı, DPI algısı farklı |
| DPI | `AutoScaleMode` Designer değeri korunur (DX-14) | `net10.0-windows` varsayılan DPI algısı daha yüksek → `#if` yalnız DPI API'sinde |
| Dil | **C# 7.3** — `using var`, target-typed `new` yasak | C# 14 yazılabilir; ortak kaynak 7.3'te kalır |

---

## §6 Kardes Dosyalar

| Konu | Dosya |
|---|---|
| Global tema/look & feel | [theme-manager-office2010](theme-manager-office2010.md) |
| Kolon biçimi (format katmanı) | [gridview-columns-format](gridview-columns-format.md) |
| Koşullu biçim (format rule) | [grid-conditional-format-rules](grid-conditional-format-rules.md) |
| DPI / ribbon ölçek | [ribbon-auto-hide-scaling](ribbon-auto-hide-scaling.md) |
