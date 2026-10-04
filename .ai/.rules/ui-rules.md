---
title: "UI Ekran ve Tasarım Kuralları - VersaCoder"
type: rules
category: rules
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "Ekran/form kuralları: RibbonForm + Office2010 zorunluluğu, DevExpress v26.1 kontrol seti, wireframe(GATE-003)→Form zinciri, code-behind sınırı, yerleşim ve yenileme kuralları"
---

# UI Ekran ve Tasarım Kuralları (`.rules/ui-rules.md`)

**Zorunlu Bağlantılar:** [index.md](../index.md) · [CLAUDE.md](../CLAUDE.md) · [keys.md](../keys.md) · [test-strategy](../spec/test-strategy.md) · [devexpress-rules.md](devexpress-rules.md) · [accessibility-rules.md](accessibility-rules.md) · [performance-guidelines.md](performance-guidelines.md) · [testing-rules.md](testing-rules.md)

> **Tek cümlelik sorumluluk:** *Bir ekran hangi form ve kontrol setiyle kurulur, tasarım kararı kime aittir, ihlalde kimi RED eder* sorusunun tek cevabıdır.
> Bu dosya `devexpress-rules.md`'nin **ekran yerleşimi** katmanıdır; sürüm/ribbon derinliği orada, klavye-ekran okuyucu kuralları [accessibility-rules.md](accessibility-rules.md)'ndadır (SSOT).

---

## §0 Özet (Answer First)

Her ekran **GATE-003 onaylı wireframe'ten** gelir, ana pencere `RibbonForm` + `RibbonControlStyle.Office2010`, kontrol seti `devexpress-rules.md` DX-09 listesiyle sabittir; `.Designer.cs`/`.resx`'e yalnız `ui-designer` dokunur, kod yalnızca event handler'da yaşar — ihlal `reviewer` RED, tasarım/`.resx` ihlali `ui-designer`'a devredilir.

---

## §1 Kurallar

| # | Kural | İyi örnek | Kötü örnek | İhlalde kimi RED |
|---|---|---|---|---|
| U1 | Ana pencere her zaman `RibbonForm` türevi | `class MainForm : RibbonForm` | `class MainForm : XtraForm` | `reviewer` |
| U2 | Ribbon stili tek ve değişmez (`ADR-004`) | `RibbonControlStyle.Office2010` | `Office2007` · `Office19` | `reviewer` |
| U3 | DevExpress **yalnız Universal 2026 / v26.1** | `.csproj` = tek sürüm | v25.2 karışımı | `architect` (G8) |
| U4 | Ekran kodu onaylı wireframe'ten gelir (G12) | `ui-design/02-main-ide.md` + onay | onaysız `MainForm` yazımı | `ui-designer` |
| U5 | `.Designer.cs` / `.resx` yalnız `ui-designer`'ın | Designer'da düzen | elle `Form.resx` metin editi | `ui-designer` |
| U6 | Code-behind yalnız olay yönlendiricisi | `async void ..._Click` → servis çağrısı | `Form_Load` içinde 200 satır iş mantığı | `reviewer` |
| U7 | Yerleşim sabit ölçü ve DPI ile dayanıklı | `TablePanel` + `Dock` + `Anchor` | sabit `Location(240,120)` | `ui-designer` |
| U8 | Büyük veri sanallaştırmalı (B3 bütçesi) | `GridControl` server mode | tüm listeyi `DataSource`e vermek | `reviewer` |
| U9 | Toplu yenileme, tek repaint | `BeginUpdate()` / `EndUpdate()` | her satırda `Refresh()` | `reviewer` |
| U10 | Uyarı/gösterge tek kanal, Designer'da yok | `ProgressPanel` + durum çubuğu | `.Designer.cs` içinde `MessageBox.Show` | `reviewer` |

---

## §2 İstisnalar (ADR gerekir mi?)

| # | İstisna durumu | ADR gerekir mi | Onay |
|---|---|---|---|
| E1 | DevExpress v26.1'de karşılığı olmayan kontrol | **Evet** (alternatif ≥ 2 + sonuç ±) | `architect` + kullanıcı (G8) |
| E2 | RibbonForm olmayan modal diyalog penceresi | Hayır — yeter ki ana pencere `RibbonForm` kalsın | `ui-designer` |
| E3 | Office2010 dışı ribbon stili talebi | Evet, ama G3 nedeniyle **kapatılamaz** (RED) | kullanıcı — fiilen imkânsız |
| E4 | Ekran listesi (7 ekran) dışı yeni ekran | **Evet** — kapsam değişikliği | kullanıcı (kapsam yetkisi) |
| E5 | Designer'ın bozduğu `.Designer.cs`'i elle onarma | Hayır — ama işlemi `ui-designer` yapar, `developer` değil | `ui-designer` |

---

## §3 Kanıt

| # | Kontrol | Komut / Dosya | Beklenen |
|---|---|---|---|
| K1 | Ribbon stili tek | `Select-String -Path src -Pattern "RibbonStyle ="` | her satır `Office2010` |
| K2 | Ana pencere tipi | `Select-String -Path src -Pattern ": RibbonForm"` | 7 ekran = 7 eşleşme |
| K3 | Sürüm tekeli | `src/**/*.csproj` + `Directory.Packages.props` | DevExpress v26.1 tek satır |
| K4 | Onay zinciri | `ui-design/*.md` + `log.md` GATE-003 kaydı | onay tarihli |
| K5 | Dosya bütünlüğü | `check-links.ps1` (`.ai/scripts`) | 0 bozuk link |

- Ekran ekleme/çıkarma **kullanıcı onayı** olmadan yapılmaz (AGENTS.md §3.2 #11).
- `developer` ile `ui-designer` yetkisi çatışırsa yetki matrisi kazanır: `.Designer.cs`/`.resx` → `ui-designer`.
- Kanıtsız "tasarım tamam" iddiası G5 ihlalidir; wireframe + onay kaydı birlikte sunulur.

**Değişiklik günlüğü:** 2026-09-25 — ilk yazım (architect).
