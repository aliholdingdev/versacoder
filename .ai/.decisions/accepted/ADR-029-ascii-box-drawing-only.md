---
title: "ADR-029 — Diyagramlar saf ASCII + - | (U+2500-U+257F yasak)"
type: adr
category: ".decisions"
status: accepted
version: 1.0.0
updated: 2026-10-07
authority: primary
scope: "ADR-020 D1'i supersede eder; G4 taraması = 0"
tags: [adr, vault, ascii, diagrams, supersedes]
source: "brain.md §1/§1.1/§1.2 — 2026-10-07 yeniden inşa (orijinal tam metin kayıp; G9: kaynaksız detay yok)"
supersedes: "ADR-020-no-mermaid-only-box-drawing.md"
---

## §1 Bağlam

Unicode box-drawing kalsaydı her terminal/monospace olmayan ortamda kutu bozulur,
G4 taraması vault genelinde sürekli RED verirdi.

## §2 Karar

Diyagramlar **saf ASCII `+ - |`** ile çizilir. Unicode box-drawing
`U+2500-U+257F` vault'ta **yasaktır** (G4 taraması = 0).
**[ADR-020](adr-020-no-mermaid-only-box-drawing.md) D1'i supersede eder.**
İstisna: CLI/headless çıktı biçimi (D22 — CLI/headless çıktısı ASCII kalır;
vault içi diyagram tercihi 2026-10-07 kararı D22 ile Unicode'a açılmıştır:
kullanıcı kararı "Unicode kazanır" — GATE-004a ASCII kriteri revize edilmiştir.
Bu revizyon ADR-032'de kayıtlıdır).

## §3 Alternatifler

- **Unicode box-drawing:** darbe/monospace olmayan ortamda bozulma. ❌ (eskiden)
- **ASCII (2026-10-01 ADR-029):** evrensel okunabilirlik. ✅
- **D22 revizyonu (2026-10-07):** Unicode tercih edilir; GATE-004a kriteri
  revize; CLI/headless ASCII kalır. → **ADR-032'ye devredildi.**

## §4 Sonuçlar

**Ters-senaryo:** Unicode box-drawing kalsaydı her terminal/monapse olmayan
ortamda kutu bozulur, G4 taraması sürekli RED verirdi.

**Yakalama:** kutu taraması `U+2500-U+257F` + `documentation-check`.

## §5 Ölçülebilir çıktı

| Dosya | Çıktı |
|---|---|
| tüm vault `.md` diyagramları | çizim karakteri politikası = ADR-032 D22 |

## §6 Kaynak

- brain.md §1 satır 029 · §1.1 · §1.2 · §4.2 madde 5
- D22 kararı (2026-10-07, kullanıcı onaylı)

## §7 İlgili

- [ADR-020-no-mermaid-only-box-drawing.md](adr-020-no-mermaid-only-box-drawing.md) — superseded (D1)
- [ADR-032-ai-control-plane.md](adr-032-ai-control-plane.md) — D22 revizyonu
- [adr-index](../adr-index.md) · [brain](../brain.md)
