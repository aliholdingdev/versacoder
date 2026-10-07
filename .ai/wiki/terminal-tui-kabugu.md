---
title: Terminal TUI Kabuğu
type: arac
created: 2026-10-06
updated: 2026-10-06
sources:
  - raw/versacoder-proje-planlamasi-ai-09.md
tags: [tui, terminal, cli, nuget]
---

# Terminal TUI Kabuğu

**Karar:** Versa Coder **tam ekran TUI**'dır; sıralı çıktı yalnız `--headless` modda verilir (ai-09 §0).

**Paketler** (nuget.org, 2026-10-06 doğrulanmış):
`Terminal.Gui 2.5.0` · `Terminal.Gui.Editor 2.5.7` · `PrettyPrompt 6.0.5` · `Markdig 1.4.0` · `Spectre.Console`

**Sınır:** `Terminal.Gui`'a iş mantığı girmez; TUI / CLI / Blazor aynı çekirdeği (ADR-010 `l0..l6`) çağırır (ai-09 §2).

**Takvim:** TUI iskeleti ~2.5-3 ay · editör olgun ~4-5 ay · kod editörü "ZOR (bataklık) 1-2 ay". **Sıralama zorunlu:** çekirdek + headless CLI önce → TUI kabuk → editör (ai-09 §3).

**Kabul kriterleri:**
- 2000 token akarken zıplama yok
- Çıktı ASCII kutu (`+ - |`); U+2500–U+257F ve emoji yasak
- Başlangıç kanıtı: `C:\temp\opencode\TuiProbe` ile paket kurulumu doğrulanır, sürüm uydurulmaz (ai-09 §4-6)

## İlgili Sayfalar
- [[versacoder-projesi]] — proje planındaki yeri
- [[opencode]] — hedef ekran haritası referansı
- [[agent-workflow]] — headless çıktı kutusu standardı
