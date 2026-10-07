---
title: "VersaCoder — Ürün Şartnamesi (Product Spec)"
type: spec
category: spec
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
scope: "VersaCoder ürün seviyesi gereksinimleri — 7 ekran, ana kullanıcı yolculukları, fonksiyonel gereksinimler (VC-PRD-NNN), fonksiyonel olmayanlar ve kapsam dışı"
---

# VersaCoder — Ürün Şartnamesi (`spec/versacoder-spec.md`)

**Zorunlu Bağlantılar:** [index.md](../index.md) · [CLAUDE.md](../claude.md) · [keys.md](../governance/keys.md) · [protocol-spec.md](protocol-spec.md) · [session-spec.md](session-spec.md) · [tool-spec.md](tool-spec.md) · [test-strategy.md](test-strategy.md) · [../reports/00-kapsam-analizi.md](../ops/log.md)

> **Tek cümlelik sorumluluk:** *VersaCoder ne yapar, hangi ekranlar vardır, hangi gereksinimler test edilebilir* sorusunun tek cevabıdır.
> Gereksinim kimliği biçimi: **`VC-PRD-NNN`**. Her madde **test edilebilir** yazılır; kabul kriteri olmadan gereksinim sayılmaz (G5).

---

## §1 Ürün Tanımı

**VersaCoder**, Windows masaüstünde çalışan bir **AI Coding Platform / Agentic IDE**'dir: proje açılır, kod editörü kullanılır, **agent görevleri** çalıştırılır, AI ile **sohbet** edilir, görevler izlenir, **terminal** kullanılır ve ayarlar düzenlenir.

| Alan | Karar |
|---|---|
| Uygulamalar | `VersaCoder.Desktop` (ana) + `VersaCoder.Cli` (başsız) |
| Teknoloji | WinForms + DevExpress Universal **v26.1** · .NET `net48` + `net10.0-windows` |
| Veri | Yerel SQLite (ADO.NET + Dapper) — **EF Core yasak** (G2) |
| Bağlantı | Opsiyonel: yerel/uzak LLM sağlayıcısı (çevrimdışı kullanılabilir — §5) |
| Referans | opencode mimarisi (v1.18.32) — `REFERENCE_VERIFIED`, PART 7 girdisi |

> **KURAL:** Bu şartname `ui-design/` wireframe'lerinin (GATE-003) ve `spec/` alt belgelerinin üstündedir; çelişirse ürün gereksinimi (bu dosya) kazanır, fark `log.md`'ye kaydedilir (G10).

---

## §2 Ekran Envanteri (7 Ekran)

| # | Ekran | Dosya | Tek sorumluluk | Ana DevExpress kontrolleri |
|---|---|---|---|---|
| 1 | Start Page | `ui-design/01-start-page.md` | proje aç/yeni oluştur, son oturumlar | RibbonControl, TileControl, XtraTabControl |
| 2 | Main IDE | `ui-design/02-main-ide.md` | ana kabuk: menü/ribbon, panel orkestrasyonu, durum çubuğu | RibbonForm, RibbonControl (Office2010), DockManager, StatusBarControl |
| 3 | Solution Explorer | `ui-design/03-solution-explorer.md` | proje ağacı, dosya işlemleri | TreeList, ContextMenuStrip |
| 4 | Code Editor | `ui-design/04-code-editor.md` | dosya düzenleme, sekmeler | RichEditControl/MemoEdit, XtraTabControl |
| 5 | AI Chat | `ui-design/05-ai-chat.md` | modele mesaj, bağlam, streaming yanıt | MemoEdit, ListBoxControl, ProgressBarControl |
| 6 | Agent Panel | `ui-design/06-agent-panel.md` | **görev listesi** + agent çalışma takibi | GridControl, ProgressPanel, BarButtonItem |
| 7 | Settings | `ui-design/07-settings.md` | sağlayıcı, model, izin, görünüm ayarları | DataLayoutControl, PropertyGridControl, LookUpEdit |

> **UYARI:** **Terminal** ve **görev listesi** ayrı ekran değil, Main IDE içinde *dock* görünümlerdir (Terminal = dock panel; görev listesi = Agent Panel içeriği). Bu karar açık soru **OQ-1** ile ilişkilidir (7 ekran ↔ 16 yedek wireframe farkı — `reports/00-kapsam-analizi.md` §4.1/R1).

```
VersaCoder.Desktop
+--------------------------------------------------------------+
| RibbonForm (Office2010)                        StatusBar     |
| +----------+---------------------------------------------+  |
| | Solution |  Code Editor (XtraTabControl sekmeleri)      |  |
| | Explorer |---------------------------------------------|  |
| | (TreeList) | AI Chat  |  Agent Panel (gorev listesi)    |  |
| |          |-----------+---------------------------------|  |
| |          |  Terminal (dock gorunum)                     |  |
| +----------+---------------------------------------------+  |
| Start Page (ilk acilis) · Settings (modal/dock)              |
+--------------------------------------------------------------+
```

---

## §3 Ana Kullanıcı Yolculukları

| ID | Yolculuk | Adımlar | Başarı |
|---|---|---|---|
| J1 | Yeni proje aç → kodla | Start Page → "New Project" → dizin seç → Solution Explorer dolu → dosya çift tıkla → editörde düzenle → kaydet | Dosya diskte UTF-8 güncellenir |
| J2 | Agent görevi çalıştır | Editörden/ribbon'dan "Run Agent Task" → bağlam + talimat → Agent Panel'de satır `running` → tool çağrıları görünür → `completed` | Görev `completed`; transcript kayıtlı (session-spec) |
| J3 | AI Chat | AI Chat paneli → mesaj gönder → streaming yanıt → duraklat/durdur | İptal edilebilir; yanıt transcript'e yazılır |
| J4 | Görev listesi izle | Agent Panel → sıralı görevler → filtre (durum) → tıkla → detay | Durum geçişleri §4.3 kurallarına uyar |
| J5 | Terminal kullan | Dock → Terminal görünümü → komut → çıktı | Süreç timeout'a uyar; iptal edilebilir (SEC-05) |
| J6 | Ayarları değiştir | Settings → sağlayıcı/model/izin → kaydet → etkili | Yeni ayar bir sonraki görevde geçerli; secret şifreli (SEC-02/07) |

---

## §4 Fonksiyonel Gereksinimler (`VC-PRD-NNN`)

### §4.1 Proje Yaşam Döngüsü

| ID | Gereksinim | Kabul kriteri |
|---|---|---|
| VC-PRD-001 | Kullanıcı, Start Page'den **yeni proje** oluşturabilir (ad + kök dizin) | Geçersiz/ad çakışması reddedilir; proje kaydı SQLite'a düşer; ağacı görünür |
| VC-PRD-002 | Kullanıcı **mevcut projeyi** açabilir (son projeler listesi + dizin seçimi) | Son projeler kalıcı; açılışta dosya ağacı gerçek diskle eşleşir |
| VC-PRD-003 | Proje **silme/yeniden adlandırma** yıkıcıdır ve **onaya** tabidir (G7) | Onay dialoğu yoksa işlem çalışmaz; iptal = hiçbir değişiklik yok |
| VC-PRD-004 | Proje kök dizini **workspace** sınırıdır: dosya işlemleri bu kökün dışına çıkamaz | `..`/mutlak yol escape'i reddedilir (SEC-11) ve olay loglanır |

### §4.2 Editör ve Çözüm Gezgini

| ID | Gereksinim | Kabul kriteri |
|---|---|---|
| VC-PRD-010 | Dosya açma: Solution Explorer'da çift tık editör sekmesi açar | Aynı dosya ikinci kez açılmaz (sekme odaklanır); UTF-8 korunur |
| VC-PRD-011 | Düzenleme + kaydetme (Ctrl+S) | Kaydetme sonrası dosya diskte güncellenir; kirli sekme göstergesi sıfırlanır |
| VC-PRD-012 | Kirli dosya kapanışta uyarır | Uyarı: kaydet / vazgeçme / iptal — kaydarma veri kaybı üretmez |
| VC-PRD-013 | Dosya/dizin oluşturma, taşıma, silme workspace içinde | Silme onaylı (G7); işlem sonrası ağaç yenilenir |
| VC-PRD-014 | Arama (dosya içi ve proje geneli) | Sonuç listesi dosya:satır gösterir, tıklayınca editör o satıra gider |

### §4.3 Agent Görevleri

| ID | Gereksinim | Kabul kriteri |
|---|---|---|
| VC-PRD-020 | Kullanıcı bir **agent görevi** başlatabilir (talimat + isteğe bağlı dosya bağlamı) | Görev `created` → `running`; Agent Panel'de görünür |
| VC-PRD-021 | Görev durum makinesi: `created → running → paused → completed/failed` (ayrıntı `session-spec.md`) | Geçersiz geçiş (ör. `completed → running`) reddedilir |
| VC-PRD-022 | Görev **iptal/duraklat** edilebilir | İptal 2 sn içinde `failed(cancelled)`/`paused`; yarım tool çağrısı kilitli kalmaz |
| VC-PRD-023 | Tool çağrıları **izlenebilir**: araç, parametre özeti, sonuç, süre | Her çağrı transcript'te; izin gerektiren çağrı UI'da onay ister (`tool-spec`) |
| VC-PRD-024 | Görev **yeniden başlatılabilir** (resume) — tamamlanan adım tekrarlanmaz | Resume'da `completed` adım sayısı değişmez; transcript tutarlı |
| VC-PRD-025 | Görev listesi filtre/sıralama (durum, tarih) | Filtre sürdürülebilir (uygulama yeniden açılınca korunur) |

### §4.4 AI Chat

| ID | Gereksinim | Kabul kriteri |
|---|---|---|
| VC-PRD-030 | Kullanıcı mesaj gönderir, yanıt **streaming** akar | İlk token 2 sn içinde görünür (hedef §5.1); iptal butonu çalışır |
| VC-PRD-031 | Sohbet bağlamı: aktif proje + seçili dosya (kullanıcı görünür ve kaldırılabilir) | Bağlam ekleme/çıkarma UI'da görünür; çıkarılan içerik isteğe gitmez |
| VC-PRD-032 | Sohbet oturumda kalıcı, yeniden açılınca görünür | Transcript yapısı `session-spec.md`'ye uyar |
| VC-PRD-033 | Sağlayıcı/model Settings'ten seçilir; hata kullanıcıya anlaşılır döner | Sağlayıcı yok/bağlantı hatası → Türkçe hata + retry (protocol-spec timeout/retry) |

### §4.5 Terminal

| ID | Gereksinim | Kabul kriteri |
|---|---|---|
| VC-PRD-040 | Terminal görünümünde komut çalıştırılır (workspace kökünde) | `WorkingDirectory` = proje kökü; çıktı satır akışı görünür |
| VC-PRD-041 | Komut girdisi sanitization'a tabidir; shell birleşimi yok | `ArgumentList` (SEC-05); tehlikeli kalıp reddedilir |
| VC-PRD-042 | Süreç timeout + iptal | Timeout'ta süreç tamamen sonlanır; terminal "killed (timeout)" der |

### §4.6 Görev Listesi (Agent Panel) ve Ayarlar

| ID | Gereksinim | Kabul kriteri |
|---|---|---|
| VC-PRD-050 | Görev listesi: id, başlık, durum, başlangıç, süre | Liste gerçek veritabanı durumunu yansıtır; refresh ile eşleşir |
| VC-PRD-051 | Görev detayı: tool çağrıları ve hata nedeni | `failed` görevde hata kodu (`VC-E-NNNN`) + insanca mesaj görünür |
| VC-PRD-060 | Ayarlar: sağlayıcı, model, API erişimi (şifreli), tool izin varsayılanı, görünüm | Kaydet/kapat ayrımı; secret **asla** düz metin görünmez (SEC-08) |
| VC-PRD-061 | Ayarlar kalıcı ve uygulama yeniden açılınca korunur | Yeniden açılışta değerler değişmez |
| VC-PRD-062 | Ayar geçersizse reddedilir (ör. boş URL, geçersiz timeout) | Alan bazlı hata gösterimi; kaydetme engellenir |

### §4.7 CLI (`VersaCoder.Cli`)

| ID | Gereksinim | Kabul kriteri |
|---|---|---|
| VC-PRD-070 | CLI bir görevi başlatabilir ve sonucu döndürür | Çıkış kodu: `0` başarılı, `1` görev `failed`, `2` kullanım hatası |
| VC-PRD-071 | CLI, Desktop ile **aynı** veritabanı/oturum modelini paylaşır | Aynı görev her iki uygulamada da listelenir (`session-spec.md`) |

---

## §5 Fonksiyonel Olmayan Gereksinimler

| ID | Kategori | Gereksinim | Kabul kriteri |
|---|---|---|---|
| VC-PRD-080 | Performans | Uygulama açılışı (Start Page görünür) **≤ 5 sn** (soğuk, hedef) | Ölçüm `test-strategy.md` §performans smoke; iki bacakta raporlanır |
| VC-PRD-081 | Performans | Proje ağacı 1.000 dosyada **≤ 2 sn**; liste ekranları 1.000 satırda akıcı | Ölçüm + sorgu bütçesi (DA-16: ekran başına ≤ 10 sorgu) |
| VC-PRD-082 | Çevrimdışı | Temel işlevler (proje, editör, terminal, görev izleme) **internetsiz** çalışır | Ağ kapalıyken J1/J4/J5 tamamlanır; yalnız LLM çağrıları ağ ister |
| VC-PRD-083 | Çift hedef | `net48` **ve** `net10.0-windows` derlenir ve test edilir | Her iki bacakta build 0 hata + test yeşil (G6, GATE-004/005) |
| VC-PRD-084 | Erişilebilirlik | Klavye ile tüm ana işlevler (ribbon kısayolları, sekmeler, grid gezinme); okunabilir kontrast; DPI'ta kesilme yok | Klavye yolculuğu J1–J6 tamamlanabilir (DX-14, GATE-003 kontrolü) |
| VC-PRD-085 | Dayanıklılık | Uygulama kapanışta kirli iş/oturum durumunu **kurtarılabilir** yapar | Zorla kapanış sonrası yeniden açılışta oturumlar `paused` işaretlenir (`session-spec`) |
| VC-PRD-086 | Güvenlik | Secret repo'da/düz metinde yok; loglar kırmızıaltılmış | Tarama: `sk-`/`Password=` = 0; logda ham secret = 0 (SEC-01/08) |
| VC-PRD-087 | Yerelleştirme | UI metinleri resx'te; dil değiştirme **açık kapsam dışı** (bkz. §6) | Anahtarlar İngilizce, varsayılan değerler Türkçe (NC-18) |
| VC-PRD-088 | Güvenilirlik | Görev/task verisi yalnız transaction içinde değişir | Bozuk yarım kayıt testte üretilemez (DA-13) |

---

## §6 Kapsam Dışı (Out of Scope)

| # | Kapsam dışı | Not |
|---|---|---|
| 1 | Web/mobil/WPF/arayüz | WinForms + DevExpress tek UI (G8) |
| 2 | Windows dışı işletim sistemi | `net*-windows` TFM |
| 3 | EF Core / ORM alternatifi | G2 — yok |
| 4 | Çok kullanıcılı sunucu/ekip iş birliği | Masaüstü, tek kullanıcı; gelecek part |
| 5 | Eklenti marketi / üçüncü parti plugin yükleyici | MCP gateway planı ayrı (`keys.md` #21) |
| 6 | Kod derleme/çalıştırma IDE entegrasyonu (F5 debugger) | Yalnız terminal + agent çalıştırma |
| 7 | Kullanıcı arayüzü dili çevirisi (TR/EN/…) | Varsayılan TR metinler sabit |
| 8 | Otomatik güncelleme (self-update) | — |

---

## §7 Açık Sorular (OPEN QUESTIONS)

> **AÇIK SORU:** Aşağıdakiler çözülmeden ilgili ekran/gereksinim **kesinleşmez**; karar `log.md` + kullanıcı onayı ister (G10).

| ID | Soru | Etkilediği gereksinim | Karar zamanı |
|---|---|---|---|
| OQ-1 | Terminal ve görev listesi ayrı ekran mı, dock görünüm mü? (7 ekran ↔ 16 yedek wireframe farkı, R1) | §2, VC-PRD-040/050 | GATE-003 öncesi |
| OQ-2 | Yerel (Ollama) vs OpenAI-uyumlu sağlayıcı **öncelik sırası** (B4) | VC-PRD-033/060 | PART 7 |
| OQ-3 | Editor kontrolü: `RichEditControl` (syntax zengin) vs `MemoEdit` (hafif) — performans/özelleştirme denemesi gerekir | VC-PRD-010/011 | GATE-003 |
| OQ-4 | `net48` bacağında DevExpress v26.1 uyumluluk kanıtı (R5) | VC-PRD-083 | **PART 5 ilk build** |
| OQ-5 | Proje modeli: klasör tabanlı mı, çözüm dosyası (`.vsx`) mı saklar? | VC-PRD-001/002 | PART 5 |

---

## §8 Kabul Kriterleri (Şartname Geneli)

- [ ] 7 ekran §2'de tek tek; her biri GATE-003 wireframe'ine bağlı
- [ ] 6 yolculuk (J1–J6) §3'te, başarısı tanımlı
- [ ] Fonksiyonel gereksinimler `VC-PRD-NNN` + **kabul kriteri** ile test edilebilir
- [ ] NFR'ler ölçülebilir eşik içeriyor (≤ 5 sn, ≤ 10 sorgu, 0 hata)
- [ ] Kapsam dışı 8 madde; açık sorular açıkça işaretli
- [ ] Belge içi bağlantılar yalnız mevcut dosyalara
- [ ] EF Core yalnız "yasak" olarak anılıyor; DevExpress yalnız v26.1

---

## §9 Referanslar

| # | Kaynak | Yol |
|---|---|---|
| 1 | Kapsam analizi (7 ekran, R1–R6, B1–B4) | `../reports/00-kapsam-analizi.md` |
| 2 | Anayasa (teknoloji değişmezleri) | `../CLAUDE.md` |
| 3 | Oturum modeli | `session-spec.md` |
| 4 | Protokol/mesaj | `protocol-spec.md` |
| 5 | Tool sözleşmesi | `tool-spec.md` |
| 6 | Test/ölçüm | `test-strategy.md` |
| 7 | Ekran wireframe'leri | `.ai/ui-design/` (GATE-003 çıktısı) |
