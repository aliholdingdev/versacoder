---
title: "theme-manager-office2010 - VersaCoder"
type: knowledge
category: devexpress
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "DevExpress v26.1 global gorunum (look & feel) yonetimi: UserLookAndFeel + SkinManager, Office2010 ribbon stili ile iliskisi, karar matrisi ve iki bacak farki"
---

# Global Tema ve Office2010

**Zorunlu Baglantilar:** [vault indeks](../index.md) · [devexpress-rules](../../.rules/devexpress-rules.md) · [ADR-004](../../.decisions/accepted/ADR-004-devexpress-universal-v261-office2010.md) · [ribbon-control-office2010](ribbon-control-office2010.md) · [nuget-references-cpm](nuget-references-cpm.md)

> **Tek cumlelik sorumluluk:** *Tema (skin) nereden tek noktada yonetilir ve ribbon stili neden temadan bagimsizdir* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

DevExpress v26.1'de "tema" uc ayri katmandir: **ribbon stili** (DX-03: yalniz `RibbonControlStyle.Office2010`), **skin** (renk/dosya paketi, `UserLookAndFeel`), **appearance** (tek kontrolun gorunumu). Global tema **tek noktadan** `UserLookAndFeel.Default` ile ayarlanir; tum kontroller ondan miras alir. Ribbon stili hicbir tema degisiminde degistirilemez — bu ADR-004 ve DX-03 kirmizi cizgisidir; skin adi ise `ui-designer` yetkisindedir.

```text
+--------------------------------------------------------------+
| UserLookAndFeel.Default        <- tek global kaynak (1 kez)  |
|    +--> RibbonControl.RibbonStyle  <- SADECE Office2010 (DX-03)|
|    |                                    -- temadan BAGIMSIZ   |
|    +--> Form/Control .LookAndFeel   <- miras: kontrol -> form |
|    +--> SkinManager.Default.Skins   <- kurulu skin adlari     |
|    +--> AppearanceObject'lar        <- en alt katman          |
+--------------------------------------------------------------+
```

---

## §1 Anahtar ozellikler

| API / ozellik | Ne is yarar | Varsayilan | Karar sahibi |
|---|---|---|---|
| `UserLookAndFeel.Default.SetSkinStyle(skin)` | tum uygulamaya tek skin | sistem temasi | `ui-designer` |
| `UserLookAndFeel.Default.Style` | klasik `LookAndFeelStyle` secimi | — | skin ile birlikte kullanilmaz |
| `form.LookAndFeel` | forma ozel override | `Default`'tan miras | `developer` (nadir) |
| `SkinManager.Default.Skins` | kurulu skin listesi (runtime kesif) | — | — |
| `ribbonControl1.RibbonStyle` | ribbon stili | `Office2010` | **sabit — kimse degistirmez** |

---

## §2 Kod (tek ayar noktasi)

```csharp
using DevExpress.LookAndFeel;

// Program.cs / MainForm kurucusu - TEK ayar noktasi (v26.1)
UserLookAndFeel.Default.SetSkinStyle("Office2010Blue");        // skin = renk paketi
ribbonControl1.RibbonStyle =
    DevExpress.XtraBars.Ribbon.RibbonControlStyle.Office2010;   // DX-03 - DEGISTIRILMEZ
this.LookAndFeel.SkinName = "Office2010Blue";                  // forma ozel gerekirse (nadir)
```

| Ayrin | Skin | Ribbon stili |
|---|---|---|
| API | `SetSkinStyle(string)` | `RibbonControlStyle` |
| Ne degisir | renk, glyph, dosya | ribbon duzeni/grafigi |
| Degisebilir mi | evet (`ui-designer`) | **hayir** (DX-03 / G3) |
| Ihlalde | gorsel tutarsizlik | **CRITICAL — build RED** |

Kural: ayar **bir kez**, uygulama baslangicinda yapilir; her formda tekrarlayan `SetSkinStyle` cagrisi daginikliktir (SRP).

---

## §3 Karar matrisi

| Soru | Karar sahibi | Kaynak |
|---|---|---|
| Ribbon hangi stil? | sabit: `Office2010` — pazarlik yok | [ADR-004](../../.decisions/accepted/ADR-004-devexpress-universal-v261-office2010.md) |
| Skin hangi ad? | `ui-designer` | [devexpress-rules](../../.rules/devexpress-rules.md) |
| Global mi, forma ozel mi? | `developer` | SRP — global varsayilan |
| Surum? | v26.1 tek surum | G3 / DX-01 |

---

## §4 Iki bacak (net48 / net10.0-windows)

| Konu | net48 | net10.0-windows |
|---|---|---|
| Paket kaynagi | CPM + `App.config` redirect **yalniz 26.1** | ayni CPM satiri, redirect gerekmez |
| `UserLookAndFeel` API | birebir ayni | birebir ayni |
| Dil surumu | **C# 7.3** — `using var`, target-typed `new` yasak | C# 14 yazilabilir; ortak dosya 7.3'te kalir |
| Tema + DPI | `AutoScaleMode` Designer degeri korunur | .NET 10 DPI davranisi farkli (DX-14) |

> **YASAK:** DX-03'teki satir disinda ribbon stili, `v26.1` disi surum, EF Core (G2 / G3 / DX-02).

---

## §5 Kontrol listesi

| # | Adim | Kanit |
|---|---|---|
| 1 | `SetSkinStyle` yalniz baslangic noktasinda 1 kez | grep |
| 2 | Her `RibbonControl`'de `RibbonStyle = Office2010` | grep |
| 3 | `.csproj`'de elle `Version=` yok (CPM) | grep |
| 4 | net48 `App.config` redirect yalniz 26.1 | dosya icerigi |
| 5 | Her iki bacakta `dotnet build` -> 0 hata | build ciktisi |

---

## §6 Kardes dosyalar

| Konu | Dosya |
|---|---|
| Ribbon stili kurali | [ribbon-control-office2010](ribbon-control-office2010.md) |
| Ribbon form kurulumu | [ribbon-form-basics](ribbon-form-basics.md) |
| Paket/CPM referansi | [nuget-references-cpm](nuget-references-cpm.md) |
| Designer edit kurallari | [designer-runtime-customization](designer-runtime-customization.md) |

---

## §9 Kanit

| Kaynak dosya | Ne alindi |
|---|---|
| `knowledge\devexpress\theme-manager-office2010.md` (kaynak vault) | gorunum katmanlari akisi, anahtar API tablosu, tek ayar noktasi kodu, karar matrisi |
| `.decisions\accepted\ADR-004-devexpress-universal-v261-office2010.md` | Office2010 + v26.1 sabiti |
| `.rules\devexpress-rules.md` | DX-03, DX-01, DX-14 |

> Kanitsiz iddia icin `DOĞRULAMA GEREKLI` yazilir; kurulu skin adlari kurulumu bagimlidir, bu dosyada listelenmez.
