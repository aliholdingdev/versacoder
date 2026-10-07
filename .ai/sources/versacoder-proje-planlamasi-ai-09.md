---
title: "VERSA CODER — Terminal TUI (Full-Screen) + Headless CLI Planlama Promptu (ai.09)"
raw_path: "raw/versacoder-proje-planlamasi-ai-09.md"
ingested: 2026-10-06 18:59
tags: [versacoder, tui, terminal-gui, cli, headless, dotnet, nuget, gate]
---

# Versa Coder Proje Planlaması ai.09 — Terminal TUI / Headless CLI

## Genel Özet
Bu dosya (`versacoder-proje-planlamasi-ai-09.md`), Versa Coder'ın "opencode benzeri" kod editörlü tam ekran TUI kabuğunu ve headless CLI modunu planlayan bir senior architect promptudur (ai.09, VERSION 1.0.0). Temel prensip: Versa Coder **tam ekran TUI**dır; sıralı çıktı yalnız `--headless` modda yaşar — ikisini karıştıran tasarım iki kez çizim katmanı yazdırır. Teknoloji kararları nuget.org üzerinden 2026-10-06 tarihinde doğrulanmıştır: Terminal.Gui 2.5.0, Terminal.Gui.Editor 2.5.7, PrettyPrompt 6.0.5, Markdig 1.4.0, Spectre.Console (yalnız headless). Kırmızı çizgi: `Terminal.Gui`'a iş mantığı girmez; TUI, CLI ve Blazor aynı çekirdeği (ADR-010 `l0..l6`) çağırır. `src/**` altına kod yazımı GATE-003 onayından önce yasaktır (G8 · G12).

## Ana Fikirler
- Hedef ekran haritası opencode referansıdır: TabView (sekme + "New session"), markdown akış alanı, isteğe bağlı bölünmüş kod editörü paneli, giriş kutusu + model satırı, durum satırı (repo yolu, shift+tab agents, ctrl+p commands, MCP/plugin sayacı, sürüm) (ai-09.md §1).
- Teknoloji kararları (nuget.org, 2026-10-06 doğrulanmış): Terminal.Gui 2.5.0 (çekirdek TUI), Terminal.Gui.Editor 2.5.7 (hazır editör), PrettyPrompt 6.0.5 (giriş satırı; CSharpRepl'in kullandığı kütüphane), Markdig 1.4.0 (markdown), Spectre.Console (`--headless`/CI), ADR-010 `l0..l6` çekirdek paylaşımı (ai-09.md §2).
- Kırmızı çizgi: `Terminal.Gui`'a iş mantığı girmez; aksi halde Blazor ve headless CLI aynı kodu iki kez yazar (ai-09.md §2).
- Zorluk/takvim: kabuk 2-3 hafta, markdown render ~2 hafta, streaming ~2 hafta, **kod editörü ZOR (bataklık) 1-2 ay**, keybindings sürekli, headless 1 hafta; TUI iskeleti ~2.5-3 ay, editör olgun ~4-5 ay (ai-09.md §3).
- Zorunlu sıralama: çekirdek + headless CLI önce (4-6 hafta) → TUI kabuk → editör paneli; TUI'dan başlarsan çekirdek sabit değilken ekranı iki kez yazarsın (ai-09.md §3).
- Kabul/RED kriterleri: tüm ekran öğeleri çalışıyor; 2000 token akarken zıplama yok; TUI projesinde iş mantığı yok; `--headless` çıktısı ANSI'sız düz metin/Markdown; editör paneli satır no + arama + diff; `dotnet build` 0 hata + ekran görüntüsü kanıtı (ai-09.md §4).
- Çıktı formatı: `BAŞLIK / KAPSAM / KARAR / RİSK / KANIT` ASCII kutusu (`+ - |`; `U+2500-U+257F` yasak); KANIT somut değilse çıktı işlenmez (G5) (ai-09.md §4).
- Kapsam dışı: `src/**` kodu (GATE-003 yasağı), ilk git commit (yalnız GATE-004; `git add -A` yasak), Blazor kabuğu (ai.10 adayı), model/sağlayıcı seçimi (ai-09.md §5). Başlangıç kanıtı `C:\temp\opencode\TuiProbe` geçici projesiyle paket kurulumu doğrular (ai-09.md §6).

## Önemli Alıntılar/Veriler
- "Versa Coder tam ekran TUI'dır; sıralı çıktı yalnız `--headless` modda yaşar." (ai-09.md §0).
- Karar tablosu: "TUI çerçevesi (ÇEKİRDEK) | Terminal.Gui 2.5.0 | Tam ekran, resize, mouse, Windows terminal; 2.1M indirme" (ai-09.md §2).
- "Kırmızı çizgi: `Terminal.Gui`'a iş mantığı girmez." (ai-09.md §2).
- "Toplam: çalışan TUI iskeleti ~2.5-3 ay; editör olgun ~4-5 ay." (ai-09.md §3).
- Kabul kriteri: "2000 token akarken alt satır/zıplama yok" (ai-09.md §4).
- "`src/**` altına kod yazımı — GATE-003 onayından önce yasak (G8 · G12)." (ai-09.md §5).
- Kanıt adımı: `dotnet new console -n TuiProbe -o C:\temp\opencode\TuiProbe` + Terminal.Gui/Terminal.Gui.Editor/PrettyPrompt/Markdig kurulumu; "sürüm uydurulmaz" (ai-09.md §6).
- Bağlantılar: VISION (7 ekran + CLI), AGENTS (ajan matrisi · 3-STRIKE), WORKFLOW (GATE-003/GATE-004), ai.08 katmanlı mimari (ai-09.md §7).

## Bu Kaynaktan Oluşan/Güncellenen Wiki Sayfaları
- [[versacoder-projesi]] — TUI/CLI katman planı, nuget paket kararları ve zorluk takvimi
- [[opencode]] — hedef ekran haritası opencode referansıdır; streaming stabilite hedefi
- [[agent-workflow]] — GATE-003/GATE-004 kapıları, `BAŞLIK/KAPSAM/KARAR/RİSK/KANIT` çıktı kutusu, G5 kanıt kuralı
- [[terminal-tui-kabugu]] — Terminal.Gui/PrettyPrompt/Markdig paket seçimleri ve çekirdek saflığı (yeni sayfa)
- [[prompt-engineering]] — ai.09'un RED/KABUL kriterli prompt yapısı (planlama promptu örneği)
