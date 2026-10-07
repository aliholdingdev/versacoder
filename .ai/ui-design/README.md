---
title: "ui-design/ — Ekran Wireframe Klasörü (GATE-003)"
type: index
category: ui
status: draft
version: 1.2.0
updated: 2026-10-02
authority: SSOT
scope: "11 wireframe dosyasinin navigasyonu (01..10 + 08-terminal), ekran adi sozlesmesi (01-start-page..10-ai-chat-console), dosya uclusu kurali (cs/Designer.cs/resx), GATE-003 cikti paketi ve ONAY PAKETI (S7) - onaysiz tek satir .cs yazilmaz"
---

# `ui-design/` — Ekran Wireframe Klasörü

**Zorunlu Bağlantılar:** [index](../index.md) · [VISION](../VISION.md) · [reports/00-kapsam-analizi](../reports/00-kapsam-analizi.md) · [ADR-028](../.decisions/accepted/ADR-028-layered-baseline-9-projects.md) · [ui-rules](../.rules/ui-rules.md) · [devexpress-rules](../.rules/devexpress-rules.md)

> **Tek cümlelik sorumluluk:** Hangi ekranın hangi dosyada yaşadığı, ekran adlarının yazım sözleşmesi ve
> GATE-003'ün neyi beklediği sorusunun tek cevabıdır.

---

## §0 Özet (Answer First)

Bu klasörde **11 wireframe** tanımlıdır (01..07 · 08-ascii-art-viewer · 08-terminal · 09-cli-console · 10-ai-chat-console); ekran adları **dosya adıyla aynıdır** ve değiştirilemez
(`01-start-page` .. `10-ai-chat-console`). Kod `src/VersaCoder.Desktop/` içinde yazılır ama
**GATE-003 kapanmadan tek satır `.cs` yazılmaz** — bu klasör, o kapının girdisidir.

| # | Dosya | Ekran | DevExpress odağı | Besleyen katman |
|---|---|---|---|---|
| 1 | `01-start-page.md` | Karşılama + son projeler | `RibbonControl`, `TileControl`, `GridControl` | l7 + l6 |
| 2 | `02-main-ide.md` | Ana pencere (kabuk) | `DockLayoutManager`, `XtraTabControl` | l7 |
| 3 | `03-solution-explorer.md` | Proje ağacı | `TreeList`, `SearchControl` | l7 + l5 |
| 4 | `04-code-editor.md` | Kod editörü | `RichEditControl`, `TreeList` (outline) | l7 + l4 |
| 5 | `05-ai-chat.md` | Ajan sohbeti | `RichEditControl` (akış), `MemoEdit` | l7 + l5 |
| 6 | `06-agent-panel.md` | Görev paneli | `GridControl`, `ProgressBarControl` | l7 + l6 |
| 7 | `07-settings.md` | Ayarlar | `GridControl`, `ToggleSwitch` | l7 + l4 |
| 8 | `08-ascii-art-viewer.md` | ASCII/Unicode Art Viewer | `RibbonControl`, `PictureEdit`, `TreeList` | l7 |
| 9 | `09-terminal.md` | **Terminal (CLI dock, alt panel)** | `MemoEdit`, `TextEdit`, `StatusBarControl` | l7 + l6 |
| 10 | `09-cli-console.md` + [spec](../spec/cli-console-spec.md) | **CLI Konsolu (CLI sekmesi, kendi penceresi)** | `XtraTabControl`, `MarqueeProgressBarControl`, `LayoutControl` | l7 + l6 |
| 11 | `10-ai-chat-console.md` + [spec](../spec/ai-chat-console-spec.md) | **AI Chat Konsolu (oturum sekmesi, kendi penceresi)** | `XtraTabControl`, `RichEditControl`, `LayoutControl` | l7 + l5 |

> **Kaynak çelişkisi (stale README):** eski vault README'si ekran listesini
> `02-project-new / 03-editor / 04-agent-chat / 05-tasks / 06-terminal` olarak veriyordu; diskteki
> gerçek dosyalar yukarıdaki gibidir. **Karar: dosya adları esastır** (ADR-002 — doğrulanmış yerel
> kanıt > tekrarlanan metin). Eski liste `DOĞRULAMA GEREKLİ` olarak not edilmiştir.

---

## §1 Ekran Adı Sözleşmesi

| Kural | Değer | Neden |
|---|---|---|
| Biçim | `NN-kebab-case` (2 hane + tire) | sıralama + link stabilitesi |
| Aralıklar | `01` .. `10`, numara **atlanmaz/eklenmez** | 11 wireframe sabittir (7 ekran + 08-ascii + 08-terminal + 09-cli-console + 10-ai-chat-console, 2026-10-02 kullanıcı kararı) |
| Dosya ↔ ekran | dosya adı = ekran adı = koddaki bölge adı | tek isim üç yerde (SSOT) |
| Kod içi geçiş | `02-main-ide` ifadesi `.resx`/metinde **görünmez** | kullanıcı dili ≠ dosya adı |
| Yeni ekran | 12. wireframe isteği = **kapsam değişikliği**, onay ister | GATE-003 paketi genişler |

**Erişilebilirlik tabanı (her ekran dosyasında tekrar eder):** klavye ile gezinme, `Tab` sırası
boşluk sırasıyla aynı, kontrast ≥ 4.5:1, `AutoScaleMode.Dpi` (yüksek DPI), durum yalnız renkle
**anlatılmaz** (ikon + metin). Ayrıntı [ui-rules](../.rules/ui-rules.md).

---

## §2 Dosya Üçlüsü Kuralı (U5 sahipliği)

Her ekran üç dosyadan oluşur; sahiplik bölünemez:

| Dosya | Ne içerir | Sahibi |
|---|---|---|
| `Xxx.cs` | olay handler + servis çağrısı (yönlendirme) | `developer` |
| `Xxx.Designer.cs` | kontrol kurulumu, kolon/panel tanımları | **`ui-designer`** |
| `Xxx.resx` | metin, ikon, ribbon başlıkları (kod içi sabit string **yasak**) | **`ui-designer`** |

- `.Designer.cs`'i `developer` eliyle değiştirmek **U5 ihlali**dir → onay zinciri kopar (ADR-025 gate).
- `.resx`'teki metin **yerelleştirilebilir** olmalı; sabit metin kodda tutulursa `resx` boş kalır ve
  incelemede RED gerekçesidir.

---

## §3 Kabuk Ortak Bölgeleri (9 ekranın ortası)

```
+----------------------------------------------------------------------+
| RibbonControl (RibbonControlStyle.Office2010) - tek stil, baska yok  |
+----------------------------------------------------------------------+
| SOL dock        |  MERKEZ (XtraTabControl / ekran govdesi)   | SAG   |
| (bölge adlari   |                                           | dock   |
|  sabittir)      |                                           |        |
+------------------------------------------------------------------+   |
| StatusBarControl: durum | kodlama | hedef bacak | surec              |
+----------------------------------------------------------------------+
```

| Bölge | Kontrol | Kural |
|---|---|---|
| Üst | `RibbonControl` | stil **yalnız** `Office2010` (G3) |
| Sol/sağ dock | `DockLayoutManager` | panel adları sabit = `.resx` anahtarları |
| Merkez | `XtraTabControl` | sekme = açık dosya; kirli = `*` |
| Alt | `StatusBarControl` | asenkron işte "yazılıyor..." durumu |
| Modal | ayarlar/işlem diyaloğu | `07-settings` ayrı diyalog |

---

## §4 GATE-003 Çıkış Paketi (onay paketi)

| # | Öğe | Durum |
|---|---|---|
| 1 | 9 wireframe dosyası (`01`..`09`) | klasörde |
| 2 | Ekran adı sözleşmesi (§1) + bu indeks | bu dosya |
| 3 | Kullanıcıdan **açık onay** | **alındı (2026-10-01, 7+1) + 09-terminal ONAY BEKLİYOR (2026-10-02)** |
| 4 | Onay sonrası kod izni (`src/VersaCoder.Desktop/`) | **09-terminal için HENÜZ AÇIK DEĞİL** (checklist 8/9, 2026-10-02) |

**HARD STOP:** onay yoksa `net48;net10.0-windows` çift bacakta tek satır `.cs` yazılmaz;
bu kural [WORKFLOW](../WORKFLOW.md) §2'deki gate tanımıyla aynıdır ve ihlali işi başa döndürür.

---

## §5 Doğrulama

| # | Kontrol | Beklenen |
|---|---|---|
| 1 | Dosya sayısı | 9 wireframe + 1 indeks (bu dosya) |
| 2 | Her dosya adı ekran adıyla aynı | `01-start-page` .. `09-terminal` |
| 3 | Unicode kutu çizimi / emoji | 0 (yalnız `+ - \|`) |
| 4 | Bozuk link | 0 |
| 5 | Onay durumu | GATE-003 **kısmen açık** (8 ONAYLI + 09-terminal ONAY BEKLİYOR, 2026-10-02) |

- [ ] Her wireframe'de: künye + ASCII + bölge tablosu + dosya üçlüsü + etkileşim + TFM notu
- [ ] Her wireframe'de `updated: 2026-09-29` frontmatter 8 alan
- [ ] Erişilebilirlik notu var (§1 tabanı)

---

## §6 Referanslar

| # | Kaynak | Yol | Ne için |
|---|---|---|---|
| 1 | 9 ekranın kullanıcı akışı | [VISION](../VISION.md) §2.3–§2.9 | ne gösterir |
| 2 | UI kuralları | [.rules/ui-rules.md](../.rules/ui-rules.md) | yerleşim/erişilebilirlik |
| 3 | DevExpress kuralları | [.rules/devexpress-rules.md](../.rules/devexpress-rules.md) | sürüm + stil sabitlemesi |
| 4 | Katman bağlamı | [ADR-028](../.decisions/accepted/ADR-028-layered-baseline-9-projects.md) | l7 sorumluluğu |
| 5 | Kapı tanımı | [WORKFLOW](../WORKFLOW.md) | GATE-003 sırası |

---

## §7 ONAY PAKETİ (kullanıcı doldurur) — GATE-003

> **Bu bölüm GATE-003'ün tek onay noktasıdır.** Her satır bir ekrandır: `Durum`
> hücresine `ONAYLI` ya da `RED`, `Revizyon isteği` hücresine değişiklik talebini
> yazın. Onaylanmayan ekranın kodu yazılmaz (G12) — onay **ekran bazlıdır**, tek
> ekran `RED` diğerlerini kilitlemez.

### §7.1 Ekran onay tablosu

| # | Ekran | Wireframe | Durum | Revizyon isteği (sen yaz) | Tarih |
|---|---|---|---|---|---|
| 1 | `01-start-page` | [oku](01-start-page.md) | `ONAYLI` | yok | 2026-10-01 |
| 2 | `02-main-ide` | [oku](02-main-ide.md) | `ONAYLI` | yok | 2026-10-01 |
| 3 | `03-solution-explorer` | [oku](03-solution-explorer.md) | `ONAYLI` | yok | 2026-10-01 |
| 4 | `04-code-editor` | [oku](04-code-editor.md) | `ONAYLI` | yok | 2026-10-01 |
| 5 | `05-ai-chat` | [oku](05-ai-chat.md) | `ONAYLI` | yok | 2026-10-01 |
| 6 | `06-agent-panel` | [oku](06-agent-panel.md) | `ONAYLI` | yok | 2026-10-01 |
| 7 | `07-settings` | [oku](07-settings.md) | `ONAYLI` | yok | 2026-10-01 |
| 8 | `08-ascii-art-viewer` (kapsam değişikliği, §0) | [oku](08-ascii-art-viewer.md) | `ONAYLI` | yok | 2026-10-01 |
| 9 | `08-terminal` (dock alt paneli) | [oku](09-terminal.md) | `BEKLEMEDE` | — | 2026-10-02 |
| 10 | `09-cli-console` (kapsam değişikliği, §0) | [oku](09-cli-console.md) | `BEKLEMEDE` | — | 2026-10-02 |
| 11 | `10-ai-chat-console` (kapsam değişikliği, §0) | [oku](10-ai-chat-console.md) | `BEKLEMEDE` | — | 2026-10-02 |

**Geçerli `Durum` değerleri:** `BEKLEMEDE` → `ONAYLI` · `RED` · `REVIZE`
(`REVIZE` = değişiklik uygulandı, tekrar inceleme bekliyor; `REVIZE` tek başına
kod izni vermez, `ONAYLI` olmalıdır).

### §7.2 Revizyon istekleri (detaylı)

> Satır satır yazın; her madde **ekran + ne değişecek + neden** içerir.
> Uygulanan her madde `REVIZE` → tekrar `ONAYLI` akışından geçer.

1. _(boş — yazınca `developer`'a değil, önce `ui-designer`'a gider)_
2.
3.

### §7.3 Kural

| Kural | Sonuç |
|---|---|
| `Durum` hücresi dolu olmayan ekran | **onaysız** → o ekranın `.cs` kodu yasak (G12) |
| Ekran `RED` ise | yalnızca **o** ekran durur; kalan6 ekran ilerleyebilir |
| `REVIZE` yazıldıysa | kod izni **yok**; tekrar `ONAYLI` gerekir |
| 7/7 `ONAYLI` + [checklist](../checklists/gate-003-ui-onay.md) 13 madde | GATE-003 açılır → `.cs` yazımına izin |
| Revizyon maddesi `developer`'a iletilirse | U5/ADR-025 zinciri kopar — tasarım `ui-designer`'da kalır |

### §7.4 İmza (zorunlu — metinsiz onay sayılmaz, soru 12)

| Alan | Değer |
|---|---|
| Onay metni | "GATE-003 kapsamındaki 11 wireframe'i inceledim, onaylıyorum." |
| Tarih | 2026-10-01 |
| Kullanıcı | kullanıcı (bu oturum, OpenCode) |
| Ek kapsam / istisna | 8. `08-ascii-art-viewer` + 9. `08-terminal` + 10. `09-cli-console` + 11. `10-ai-chat-console` dahil: her biri için ayrı onay cümlesi yazılır |

> Onay metni bu cümledeki **kelimelerle** yazılmalıdır; "ok / tamam" gibi kısa
> onaylar [checklist](../checklists/gate-003-ui-onay.md) soru 12'de **RED**
> gerekçesidir. Onay `log.md`'ye tarihli satır olarak da kopyalanır.
