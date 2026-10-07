---
title: "ADR-010 — Desktop + CLI aynı çekirdeği paylaşır (l0..l6)"
type: adr
category: ".decisions"
status: accepted
version: 1.0.0
updated: 2026-10-07
authority: primary
scope: "Ayrışma yalnız l7; iki ayrı çekirdek yazımı yasak"
tags: [adr, architecture, cli, desktop, shared-core]
source: "brain.md §1/§1.1/§1.2 — 2026-10-07 yeniden inşa (orijinal tam metin kayıp; G9: kaynaksız detay yok)"
supersedes: ""
---

## §1 Bağlam

İki ayrı çekirdek yazılsaydı her özellik iki kez implemente edilir, davranış
sapması ve 2x iş oluşurdu.

## §2 Karar

Desktop (`VersaCoder.Desktop`) ve CLI (`VersaCoder.Cli`) **aynı `l0..l6`
çekirdeğini birebir paylaşır**; ayrışma yalnız `l7` (sunum) katmanındadır.
TUI, CLI ve gelecekteki Blazor kabuğu aynı servis arayüzlerini çağırır;
`Terminal.Gui` gibi UI kütüphanelerine iş mantığı girmez.

## §3 Alternatifler

- **İki çekirdek:** 2x iş, davranış sapması. ❌
- **Ortak çekirdek + ince sunum (seçilen):** tek davranış kaynağı. ✅

## §4 Sonuçlar

**Ters-senaryo:** İki ayrı çekirdek yazılsaydı her özellik iki kez implemente
edilir, davranış sapması ve 2x iş oluşurdu.

**Yakalama:** Ortak çekirdek testi: "aynı iş, Desktop ve Cli'de aynı sonuç".

## §5 Ölçülebilir çıktı

| Dosya | Çıktı |
|---|---|
| `VersaCoder.Desktop` + `VersaCoder.Cli` | ortak çekirdek testi iki girişte de aynı sonucu verir |

## §6 Kaynak

- brain.md §1 satır 010 · §1.1 · §1.2
- G: prompt arşivi ai.09 §2 (çekirdek saflığı kırmızı çizgisi) — yapı referansı

## §7 İlgili

- [ADR-028-layered-baseline-9-projects.md](adr-028-layered-baseline-9-projects.md)
- [adr-index](../adr-index.md) · [brain](../brain.md)
