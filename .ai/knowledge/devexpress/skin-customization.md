---
title: "skin-customization - VersaCoder"
type: knowledge
category: devexpress
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "Skin, appearance ve format katmanlarinin yazilis sirasi, AppearanceObject ozellikleri, DX-08 ve DPI sinirlari"
---

# Skin ve Appearance Ozellestirme

**Zorunlu Baglantilar:** [vault indeks](../index.md) · [devexpress-rules](../../.rules/devexpress-rules.md) · [theme-manager-office2010](theme-manager-office2010.md) · [gridview-columns-format](gridview-columns-format.md) · [grid-conditional-format-rules](grid-conditional-format-rules.md) · [designer-runtime-customization](designer-runtime-customization.md)

> **Tek cumlelik sorumluluk:** *Skin uzerine appearance nasil yazilir ve nerede durulur* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

DevExpress gorunumu uc katmandan olusur: **skin** (global renk paketi) -> **appearance** (kontrol/gorunum bazli ustune yazma) -> **format** (hucre bicimi, `DisplayFormat`). Ozellestirme hep **en ustten baslar**: once global skin, sonra gerekirse appearance, en son hucre bicimi. Elle hucre cizimi (`CustomDrawCell`) **yasaktir** (DX-08) - DPI ve tema altinda kirilir; renk/font kodu `.Designer.cs`'de degil, kod tarafinda ve tek yerde toplanir.

```text
skin  -->  appearance  -->  format
(genel)    (ustune yaz)    (hucre bazli)
   |             |              |
   +--- hicbiri elle cizim degil: CustomDrawCell YASAK (DX-08)
```

---

## §1 Katman hiyerarsisi

| Katman | Kapsam | Nerede degisir | Ait oldugu rol |
|---|---|---|---|
| Skin | tum uygulama | `UserLookAndFeel.Default` | `ui-designer` |
| Control appearance | tek kontrol | `ctrl.Appearance.*` | `ui-designer` |
| View appearance | gorunum bolgesi (Header, Row, SelectedRow) | `gridView1.Appearance.*` | `ui-designer` |
| Cell format | tek hucre degeri | `GridColumn.DisplayFormat` | `developer` |

---

## §2 Anahtar ozellikler

| Ozellik | Ne is yarar | Tipik deger |
|---|---|---|
| `AppearanceObject.Font` | yazi tipi | `new Font("Segoe UI", 9f)` |
| `AppearanceObject.ForeColor` | yazi rengi | `Color.FromArgb(40, 40, 40)` |
| `AppearanceObject.BackColor` | zemin | `Color.FromArgb(245, 245, 245)` |
| `AppearanceObject.Options` | hangi alan **aktif** (yoksa skin ezer) | `AppearanceOptions.UseFont` |
| `AppearanceObject.Assign(src)` | gorunumu kopyalama/paylasma | `EvenRow.Assign(OddRow)` |
| `gridView1.AppearancePrint.*` | yazdirma/export gorunumu | ayri set - unutulmasin |

> **UYARI:** `Options` **once** acilir, deger **sonra** atanir; tersi yapilirsa skin degeri ezer ve degisiklik gorunmez.

---

## §3 Kod (appearance yazimi)

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
| `CustomDrawCell` ile "stil" | DX-08 ihlali - DPI/tema altinda bozulur |
| Her satirda ayni renk atamasi | tekrar; tema degince unutulur |
| `.Designer.cs` icinde renk kodu | Designer yeniden uretince silinir (DX-10) |

---

## §4 Kurallar

1. Once **skin** dene; appearance yalniz skin karsilamadiginda.
2. `Options` maskesi olmadan renk/font **yazilmaz**.
3. Renkler sabit `Color.FromArgb` yerine **tek sinifda** sabitlenir (dark/light gecisine hazir olsun).
4. Print/export gorunumu ayridir (`AppearancePrint`) - eksik birakilirsa rapor farkli gorunur.
5. Elle cizim (`CustomDrawCell`) yerine appearance + `ColumnView` ozellikleri (DX-08).

---

## §5 Kim neyi degistirir

| Degisiklik | Dokunulacak katman | Ornek |
|---|---|---|
| Tum uygulama rengi | skin (`UserLookAndFeel.Default`) | tema secimi |
| Tek kontrol stili | `ctrl.Appearance.*` | buton fontu |
| Grid baslik/ satir stili | `gridView1.Appearance.*` | `HeaderPanel` |
| Hucre deger bicimi | `GridColumn.DisplayFormat` | `{0:n2}` |
| Kosullu renk | format rule | [grid-conditional-format-rules](grid-conditional-format-rules.md) |
| Yazdirma gorunumu | `AppearancePrint.*` | export/rapor |

---

## §6 Iki bacak (net48 / net10.0-windows)

| Konu | net48 | net10.0-windows |
|---|---|---|
| `AppearanceObject` API | ayni | ayni |
| Font adi | `"Segoe UI"` kurulu - net48'te olcu birimi Font (pt) | C#/GDI+ ayni, DPI algisi farkli |
| DPI | `AutoScaleMode` Designer degeri korunur (DX-14) | DPI algisi daha yuksek -> `#if` yalniz DPI API'sinde |
| Dil | **C# 7.3** - `using var`, target-typed `new` yasak | C# 14 yazilabilir; ortak kaynak 7.3'te kalir |

---

## §7 Tarama tablosu

| Aranan | Istenen | Yorum |
|---|---|---|
| `CustomDrawCell` | 0 eslesme | DX-08 |
| `.Designer.cs` icinde `Color.FromArgb` | 0 eslesme | renk kodu kod tarafinda (DX-10) |
| `Appearance...Options` | deger atamadan **once** | yoksa skin ezer |
| `AppearancePrint` | rapor ekrani varsa dolu | export farkli gorunmesin |
| Sabit renk yayilimi | tek sinifda toplanmis | dark/light gecisine hazir |

---

## §8 Kardes dosyalar

| Konu | Dosya |
|---|---|
| Global tema/look & feel | [theme-manager-office2010](theme-manager-office2010.md) |
| Kolon bicimi (format katmani) | [gridview-columns-format](gridview-columns-format.md) |
| Kosullu bicim (format rule) | [grid-conditional-format-rules](grid-conditional-format-rules.md) |
| Designer/runtime siniri | [designer-runtime-customization](designer-runtime-customization.md) |

---

## §9 Kanit

| Kaynak dosya | Ne alindi |
|---|---|
| `knowledge\devexpress\skin-customization.md` (kaynak vault) | uc katman hiyerarsisi, AppearanceObject ozellik tablosu, yazma sirasi uyari, yapilmayanlar tablosu, TFM/DPI farki |
| `.rules\devexpress-rules.md` | DX-08 (elle cizim yasagi), DX-10 (Designer siniri), DX-14 (DPI) |

> Kanitsiz iddia icin DOĞRULAMA GEREKLI yazilir; bu dosyada kaynak disi bilgi yoktur.
