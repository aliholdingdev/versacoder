---
title: "ADR-020 — Diyagramlar UTF-8 box-drawing (D1) + mermaid düz metin karşılığı (D3)"
type: adr
category: ".decisions"
status: superseded
version: 1.0.0
updated: 2026-10-07
authority: primary
scope: "D1: UTF-8 box-drawing — ADR-029 ile superseded; D3: mermaid yanına düz metin — yürürlükte"
tags: [adr, vault, diagrams, superseded]
source: "brain.md §1/§1.1/§1.2 — 2026-10-07 yeniden inşa (orijinal tam metin kayıp; G9: kaynaksız detay yok)"
supersedes: ""
---

## §1 Bağlam

Diyagram düz metin karşılıksız kalsaydı komut çıktısı ve arka uç okuyucu
diyagramı göremezdi.

## §2 Karar

Bu ADR iki karar taşır:

- **D1 — Diyagram çizim karakterleri:** UTF-8 box-drawing (Orijinal karar).
  **DURUM: SUPERSEDED by [[.decisions/accepted/adr-029-ascii-box-drawing-only|ADR-029-ascii-box-drawing-only.md]]**
  (2026-10-01): ASCII `+ - |` kararı kazandı; U+2500-U+257F vault'ta yasak.
- **D3 — Mermaid kuralı:** Mermaid diyagramı varsa **yanında düz metin
  karşılığı da zorunludur.** **DURUM: yürürlükte.**

## §3 Alternatifler

- **Mermaid tek başına:** arka uç okuyucu göremez. ❌
- **Mermaid + düz metin (D3, seçilen):** iki okuyucu da görür. ✅

## §4 Sonuçlar

**Ters-senaryo:** Diyagram düz metin karşılıksız kalsaydı komut çıktısı ve arka
uç okuyucu diyagramı göremezdi.

**Yakalama:** `documentation-check`; D1 kapsamı ADR-029 ile ASCII'ye döndü
(brain §4.2 madde 5 kapandı).

## §5 Ölçülebilir çıktı

| Dosya | Çıktı |
|---|---|
| diyagram içeren tüm `.md` | diyagramın düz metin karşılığı var |

## §6 Kaynak

- brain.md §1 satır 020 · §1.1 · §1.2

## §7 İlgili

- [[.decisions/accepted/adr-029-ascii-box-drawing-only|ADR-029-ascii-box-drawing-only.md]] — superseder
- [[.decisions/adr-index|adr-index]] · [[.decisions/brain|brain]]
