---
title: "performance-grid-optimization - VersaCoder"
type: knowledge
category: devexpress
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "DevExpress v26.1 GridControl performansi: veri yükleme, gorunum ayarlari, sanal mod esigi ve olcum adimi - iki bacak farki"
---

# performance-grid-optimization — Grid Performans Optimizasyonu (v26.1)

**Zorunlu Baglantilar:** [index](../index.md) · [devexpress-rules](../../.rules/devexpress-rules.md) · [ADR-004](../../.decisions/accepted/ADR-004-devexpress-universal-v261-office2010.md) · [gridcontrol-basics](gridcontrol-basics.md) · [grid-paging-virtual-mode](grid-paging-virtual-mode.md)

> **Tek cumlelik sorumluluk:** *Grid neden yavaşlar ve v26.1'de hangi ayarlar hız kazandırır* sorusunun tek cevabıdır.

---

## §0 Ozet (Answer First)

Grid yavaşlığının %80'i **veri yolu**ndandır: her tuşta `DataSource` yenileme, gereksiz `PopulateColumns`, editör açıkken refresh. Kalan %20 **görünüm yükü**: grup paneli, otomatik filtre satırı, özet satırları, gereksiz format rule'lar. Önce **ölç**, sonra ayar değiştir — `BestFitColumns()` ve `RefreshData()` kod içinde tekrarlanmaz, tek noktaya toplanır. 100.000+ satırda sanal/sunucu moduna geçiş ayrı iş kalemidir (`NEEDS_VALIDATION`: v26.1'de etkinleştirme yolu [grid-paging-virtual-mode](grid-paging-virtual-mode.md) içinde doğrulanır).

---

## §1 Darboğaz Haritasi

```
Veri yolu (kaynak -> DataSource)
   |  tekrarli sorgu / her refresh'te Load   <-- en pahali
   v
Kolon uretimi (PopulateColumns)              <-- 1 kez, sonra kapali
   v
Gorunum (OptionsView / OptionsBehavior)      <-- ayar ile ucuzlastirilir
   v
Cizim (DPI / tema / appearance)              <-- elle cizim YASAK (DX-08)
```

| Belirti | Kök neden | Düzeltme |
|---|---|---|
| Açılış yavaş | her formda sorgu + `BestFitColumns` | tek yükleme, ölçülü `BestFit` |
| Yazarken kasma | `RefreshData()` her `CellValueChanged`'te | olay yerine `LayoutChanged`/tek refresh |
| Kaydırma kasma | görünüm yükü (grup/filtre/özet) | `Options*` kapat |
| Bellek yüksek | 100k+ satır tam yüklü | sanal/sunucu modu (`NEEDS_VALIDATION`) |

---

## §2 Anahtar Ozellikler

| Ozellik | Varsayilan | Performans etkisi | Öneri |
|---|---|---|---|
| `gridView1.OptionsBehavior.Editable` | `true` | editör yolu her hücrede | raporda `false` |
| `gridView1.OptionsView.ShowGroupPanel` | `true` | sürükleme altyapısı | grupp yoksa `false` |
| `gridView1.OptionsView.ShowAutoFilterRow` | `false` | satır başına ek editör | gerekmiyorsa kapalı |
| `gridView1.OptionsView.ShowFooter` | `false` | özet hesabı | yalnız özetli ekranda |
| `gridControl1.DataSource` yenileme | — | **en pahali** | yalnız veri değişince |

---

## §3 Kod (ölçülü yükleme)

```csharp
// 1) Tek yükleme - Dapper parameterized (G2): tekrar yok
gridControl1.DataSource = _sessions.LoadAll(new { p1 = "Aktif" });
gridView1.PopulateColumns();                      // 1 kez
gridView1.BestFitColumns();                       // 1 kez, elle

// 2) Gorunum yukunu kapat (rapor/IDE ekranlari)
gridView1.OptionsView.ShowGroupPanel   = false;
gridView1.OptionsBehavior.Editable     = false;
gridView1.OptionsView.ShowAutoFilterRow = false;

// 3) Veri degistiyse TEK refresh
gridView1.RefreshData();
```

---

## §4 Kural Tablosu (en fazla 5 madde)

1. **Ölç → değiştir → ölç**: `Stopwatch` ile açılış/refresh süresi kaydedilir (`NEEDS_VALIDATION`: hedef ms kullanıcıyla sabitlenir).
2. `PopulateColumns` / `BestFitColumns` **form başına 1 kez**.
3. `RefreshData()` yalnız **veri** değiştiğinde; hücre olaylarında değil.
4. Elle hücre çizimi / `CustomDrawCell` **yok** (DX-08) — çizim maliyeti + kırılganlık.
5. `Options*` ayarları **Designer'da**; kodda tekrarlanan atama yok (DX-10).

---

## §5 Iki Bacak (net48 / net10.0-windows)

| Konu | net48 | net10.0-windows |
|---|---|---|
| Grid API | v26.1 net48 | v26.1 net10 — aynı imza |
| Ölçüm (`Stopwatch`) | aynı | aynı (harmonik saat farkı — `TimeSpan` karşılaştır) |
| Bellek/GC | framework GC | .NET 10 GC farklı davranır → **iki bacakta ayrı ölç** |
| Dil | **C# 7.3** — `using var` yasak | C# 14; ortak dosya 7.3 kalır |

---

## §6 Kardes Dosyalar

| Konu | Dosya |
|---|---|
| Grid iskeleti/bağlama | [gridcontrol-basics](gridcontrol-basics.md) |
| Sanal/sayfalama modu | [grid-paging-virtual-mode](grid-paging-virtual-mode.md) |
| Kolon/biçim (görünüm yükü) | [gridview-columns-format](gridview-columns-format.md) |
| Arama paneli maliyeti | [grid-search-panel-find](grid-search-panel-find.md) |
