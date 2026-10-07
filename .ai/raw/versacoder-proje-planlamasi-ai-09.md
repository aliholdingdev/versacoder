# VERSA CODER — TERMINAL TUI / CLI SHELL PLANNING PROMPT

####################################################################################
#                                                                                  #
#                 Versa Coder AI My Coding Assistant Agentic IDE                   #
#          Visual Studio IDE / Xcode / Cursor AI IDE / Antigravity AI IDE          #
#                                                                                  #
#                              VERSION 1.0.0                                       #
#                                                                                  #
#                    Designed & Coding by Bayram Ali Akşit                         #
#                                                                                  #
#                     Versa Coder TUI / CLI Katman Planı                           #
#                                                                                  #
####################################################################################

# VERSA CODER — 50+ YILLIK SENIOR SOFTWARE ARCHITECT
## TERMINAL TUI (FULL-SCREEN) + HEADLESS CLI PLANLAMA PROMPTU
### VERSION 1.0.0 — ai.09
#### Designed & Coding by Bayram Ali Akşit

---

# 0. AMAÇ

Sen Versa Coder projesinin **50+ yıllık deneyime sahip Senior Software Architect ve
Terminal UI Specialist** seviyesinde, **kod editörlü tam ekran TUI kabuğunu** planlayan
uzmansın.

Bu çalışma doğrudan kod yazmak için değil; Versa Coder'ın "opencode benzeri" terminal
arayüzünün **ekran haritasını, paket seçimini, katman sınırlarını, zorluk dağılımını ve
kabul kriterlerini** belirlemek içindir.

> **Temel prensip:** Öncelikle "normal terminal çıktısı" mı, "tam ekran TUI" mı
> karar ver. Versa Coder **tam ekran TUI**'dır; sıralı çıktı yalnız `--headless`
> modda yaşar. İkisini karıştıran tasarım iki kez çizim katmanı yazdırır.

---

# 1. HEDEF EKRAN HARİTASI (opencode referansı)

Hedef, aşağıdaki öğeleri **tümüyle** içeren tek tam ekran kabuktur:

```text
+=====================================================================+
| [sekreme 1] [sekreme 2] [ + New session ]                (TabView) |
+---------------------------------------------------------------------+
|                                                                      |
|                    Markdown akış alanı (scrollback)                  |
|                    - tablo / kod bloğu / renk / blok alıntı          |
|                    - streaming: token akarken satır ZIPLAMAZ          |
|                                                                      |
|                    Kod editörü paneli (isteğe bağlı bölünmüş)        |
|                    - satır no · seçim · arama · diff · katlama       |
|                                                                      |
+---------------------------------------------------------------------+
| | Ask anything... "Fix a TODO in the codebase"        (giriş kutusu) |
|   Build auto   MiMo-V2.6-Flash Free   OpenCode Zen    (model satırı)|
+---------------------------------------------------------------------+
| C:\versacoder:master        shift+tab agents   ctrl+p commands      |
| 3 MCP /mcps    2 plugins failed /plugins              2.0.9 (durum) |
+=====================================================================+
```

Her öğe için **zorunlu** çıktı: ekran adı · sahibi katman · çizim bileşeni · klavye
kısayolu · durum (boş/hata/akış).

---

# 2. TEKNOLOJİ KARARLARI (nuget.org üzerinden doğrulanmış — 2026-10-06)

| # | Karar | Paket / Sürüm | Gerekçe |
|---|---|---|---|
| 1 | TUI çerçevesi (ÇEKİRDEK) | **Terminal.Gui 2.5.0** | Tam ekran, resize, mouse, Windows terminal; 2.1M indirme |
| 2 | Hazır editör görünümü | **Terminal.Gui.Editor 2.5.7** | Kod editörü panelini sıfırdan yazmayı önler |
| 3 | Giriş satırı (geçmiş · ok tuşu · renk) | **PrettyPrompt 6.0.5** | CSharpRepl'in kullandığı kütüphane |
| 4 | Markdown parse | **Markdig 1.4.0** | Tablo/kod bloğu ayrıştırma olgun |
| 5 | Headless CLI çıktısı | **Spectre.Console** (sürüm kurulumda) | Yalnız `--headless` / CI modu — TUI'da KULLANILMAZ |
| 6 | Core paylaşımı | ADR-010 (`l0..l6`) | TUI, CLI ve Blazor **aynı çekirdeği** çağırır |

**Kırmızı çizgi:** `Terminal.Gui`'a iş mantığı **girmez**. Çekirdek yalnız servis
arayüzleri üzerinden çağrılır; aksi halde Blazor ve headless CLI aynı kodu iki kez
yazmak zorunda kalır.

---

# 3. ZORLUK VE TAKVİM (tek geliştirici, gerçekçi)

| # | Parça | Zorluk | Tahmin |
|---|---|---|---|
| 1 | Kabuk: TabView + StatusBar + splash + giriş kutusu | Kolay | 2-3 hafta |
| 2 | Markdown render (tablo, kod bloğu, blok alıntı) | Orta | ~2 hafta |
| 3 | Streaming (token akarken tek satırın titremeden yenilenmesi) | Orta | ~2 hafta |
| 4 | **Kod editörü: satır no · seçim · arama · diff · katlama** | **ZOR (bataklık)** | **1-2 ay** |
| 5 | Keybindings + resize + mouse + hata durumları | Orta | sürekli |
| 6 | Headless mod (`--headless`, CI raporu) | Kolay | 1 hafta |

**Toplam:** çalışan TUI iskeleti **~2.5-3 ay**; editör olgun **~4-5 ay**.
**Sıralama zorunlu:** çekirdek + headless CLI önce (4-6 hafta) → TUI kabuk → editör paneli.
TUI'dan başlarsan çekirdek sabit değilken ekranı iki kez yazarsın.

---

# 4. BAŞARI KRİTERLERİ (kabul / RED)

| # | Kriter | Kabul | RED |
|---|---|---|---|
| 1 | Ekran öğesi kapsama | §1'deki **tüm** öğeler çalışıyor | Eksik öğe var |
| 2 | Streaming stabilite | 2000 token akarken alt satır/zıplama yok | Titreme, kayıp satır |
| 3 | Çekirdek saflığı | TUI projesinde iş mantığı yok; yalnız `l0..l6` çağrısı | `Terminal.Gui` içinde servis kodu |
| 4 | Headless mod | `--headless` çıktısı düz metin/Markdown, ANSI yok | TUI çıktısının kopyası |
| 5 | Editör paneli | Satır no + arama + diff görünür | Yalnız `TextView` (düz metin) |
| 6 | Kanıt | `dotnet build` 0 hata + ekran görüntüsü | Sözlü "oldu" iddiası |

**Çıktı formatı:** `BAŞLIK / KAPSAM / KARAR / RİSK / KANIT` ASCII kutusu
(`+ - |` ile; `U+2500-U+257F` yasak). `KANIT` somut değilse çıktı işlenmez (G5).

---

# 5. KAPSAM DIŞI (bu promptta yapılmaz)

- `src/**` altına kod yazımı — **GATE-003 onayından önce yasak** (G8 · G12).
- İlk git commit — yalnız GATE-004'te; `git add -A` yasak.
- Blazor kabuğu tasarımı (ayrı prompt: `ai.10` adayı).
- Model/sağlayıcı seçimi — ayrı karar (MiMo günlük · Sonnet mimari).

---

# 6. BAŞLANGIÇ KANITI (ispat adımı, gatesiz)

`src/**` dışında, geçici dizinde paket kurulumu doğrulanır:

```powershell
dotnet new console -n TuiProbe -o C:\temp\opencode\TuiProbe
dotnet add C:\temp\opencode\TuiProbe package Terminal.Gui Terminal.Gui.Editor PrettyPrompt Markdig
dotnet run --project C:\temp\opencode\TuiProbe
```

**Beklenen kanıt:** splash + durum satırı + giriş kutusundan oluşan ekranın çalışması.
Kurulum başarısızsa paket sürümü yeniden `dotnet package search` ile doğrulanır —
sürüm **uydurulmaz**.

---

# 7. BAĞLANTILAR

- [VISION](C:\versacoder\.ai\VISION.md) — 7 ekran + CLI ürün haritası
- [AGENTS](C:\versacoder\.ai\AGENTS.md) — ajan matrisi · çıktı kutusu · 3-STRIKE
- [WORKFLOW](C:\versacoder\.ai\WORKFLOW.md) — kapılar (GATE-003 · GATE-004)
- [ai.08](./versacoder%20proje%20planlamas%C4%B1%20ai.08.md) — katmanlı mimari planlama
- [ai.07](./versacoder%20proje%20planlamas%C4%B1%20ai.07.md) — OpenCode / skills kaynakları
