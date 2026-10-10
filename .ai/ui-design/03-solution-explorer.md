---
title: "UI Wireframe 03 — Solution Explorer (Proje Ağacı)"
type: spec
category: ui
status: draft
version: 1.1.0
updated: 2026-09-29
authority: reference
scope: "03-solution-explorer ekranının wireframe'i: TreeList kolonları, arama paneli, sağ tık menüsü, dosya üçlüsü, etkileşim kuralları — GATE-003 onayı bekliyor"
tags: [ui-design]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# 03 — SOLUTION EXPLORER

**Zorunlu Bağlantılar:** [[ui-design/readme|ui-design/index]] · [[ui-design/02-main-ide|02-main-ide]] · [[ui-design/04-code-editor|04-code-editor]] · [[.rules/ui-rules|ui-rules]] · [[.rules/data-access-rules|.rules/data-access-rules.md]] · [[.decisions/accepted/adr-028-layered-baseline-9-projects|ADR-028]]

> **Tek cümlelik sorumluluk:** Proje ağacının hangi alanlardan oluştuğu, arama/sağ-tık akışının
> nereye gittiği sorusunun tek cevabıdır.

---

## §0 Özet (Answer First)

`03-solution-explorer`, `02-main-ide`'nin **sol dock** bölgesidir: kendine referanslı `TreeList`
(dosya/klasör ağacı) + `SearchControl` (Ctrl+F) + sağ tık menüsü + onay diyaloğu.
Ağaç verisi **servisten** gelir (`IProjectTreeService`); UI'da SQL/dosya okuma **yok**.
**Durum: GATE-003 onayı alındı (2026-10-01).**

| Alan | Kaynak | Not |
|---|---|---|
| Ağaç | `TreeList.KeyFieldName = Id` · `ParentFieldName = ParentId` · `RootValue = -1` | kendine referanslı; DB değil servis |
| Ad | `Name` | dosya/klasör adı |
| Tür | `Kind` (C#/MD/SLN) | `ImageCollection` ikonu |
| Durum | `IsDirty` | yıldız = kaydedilmemiş |
| Arama | `SearchControl` | Find Panel (Ctrl+F), alt ağaç filtrelenir |

---

## §1 Wireframe

```text
+--------------------------------------------------+
| SOLUTION EXPLORER                    [->] [o]    |
+--------------------------------------------------+
| Ara: dosya / klasör ara... [x]                   |
+--------------------------------------------------+
| [v] Solution 'VersaCoder'                        |
|   [v] src                                        |
|     [v] VersaCoder.Desktop                       |
|       [-] Program.cs            (C#)             |
|       [-] MainForm.cs           (C#)  *kirli     |
|       [+] Properties                             |
|     [v] VersaCoder.Cli                           |
|       [-] Program.cs            (C#)             |
|   [v] tests                                      |
|     [-] VersaCoder.Domain.Tests   (C#)           |
+--------------------------------------------------+
| Durum: 6 dosya | 1 değişiklik                    |
+--------------------------------------------------+

Sağ tık menüsü (ayrı pencere):
+--------------------------+
| Aç                       |
| Yeni Dosya               |
| Yeni Klasör              |
| Değiştir     [>] Kaydet  |
| Kopyala Yolu   Sil       |
|--------------------------|
| Silme onay dialog:       |
|Silinsin mi? [Evet][Hayır]|
+--------------------------+
```

**Durumlar:** yükleniyor → ağaçta `ProgressBarControl` tek satır; kırık bağlantı (yol yok)
→ kırmızı ikon + tooltip (neden: "dosya bulunamadı"); boş çözüm → "Proje ekle" aksiyonu.

---

## §2 Dosya Üçlüsü

| Dosya | İçerik | Sahip |
|---|---|---|
| `SolutionExplorerPanel.cs` | ağaç yükleme (`IProjectTreeService`), sağ tık komut yönlendirme, silme onayı akışı | `developer` |
| `SolutionExplorerPanel.Designer.cs` | `TreeList` kolonları, `SearchControl`, `ContextMenu`, `ImageCollection` kurulumu | **`ui-designer`** |
| `SolutionExplorerPanel.resx` | menü metinleri, durum açıklamaları, ikon kaynakları | **`ui-designer`** |

---

## §3 Etkileşim Kuralları

| # | Kural | Sonuç |
|---|---|---|
| 1 | Çift tık `.cs` | `04-code-editor`'da yeni sekme (açıksa sekmeye gider) |
| 2 | Silme her zaman onay ister | `MessageBoxForm` metni `.resx`'ten gelir |
| 3 | Ağaç yenileme `F5` | servis çağrısı, paralel yükleme — UI **donmaz** |
| 4 | Klasör genişleme durumu | oturum boyunca korunur |
| 5 | Kırık bağlantı | kırmızı ikon + tooltip; düğüm silinmez, yalnız işaretlenir |

---

## §4 Hedef Bacak Notu

| Konu | Karar |
|---|---|
| `TreeList` kendine referanslı mod | her iki bacakta aynı (DevExpress.Win 26.1) |
| Veri erişimi | ADO.NET + Dapper **yalnız servis katmanında**; UI'a SQL sızmaz (DA-01/G2) |
| Arama | `TreeList.OptionsFind` üzerinden; ek paket gerekmez |
| Erişilebilirlik | ok tuşlarıyla gezinme zorunlu; sağ tık menüsü `F10`/menü tuşuyla açılabilir; silme onayı odak `Hayır`'da başlar |

---

## §5 GATE-003 Durumu

| # | Öğe | Durum |
|---|---|---|
| 1 | Wireframe yazıldı | evet (bu dosya) |
| 2 | Kullanıcı onayı | **bekliyor** |
| 3 | Kod izni | kapalı |

---

## §6 Kanıt

| # | Kaynak | Ne kanıtlar |
|---|---|---|
| 1 | Kaynak wireframe `ui-design/03-solution-explorer.md` (eski vault) | alan/ağaç/menü listesi |
| 2 | [[.rules/data-access-rules|.rules/data-access-rules.md]] DA-01 | formda SQL yok |
| 3 | [[.rules/ui-rules|ui-rules]] | klavye gezinme + kontrast |
| 4 | [[.decisions/accepted/adr-028-layered-baseline-9-projects|ADR-028]] | ağaç verisi l5/l4 yolu |

> `IProjectTreeService` arayüz adı taslaktır; kesinleştiği yer `l0` (`DOĞRULAMA GEREKLİ`).
