---
title: "ADR-028 — Katman bazlı temel: 8 makro / 52 düğüm / 9 üretim + 1-2 test"
type: adr
category: ".decisions"
status: accepted
version: 1.0.0
updated: 2026-10-07
authority: primary
scope: "Bu vault'taki en çok atıf yapılan baz karar; §2.1 katman tablosu bağlayıcı"
tags: [adr, architecture, baseline, layers, projects]
source: "brain.md §1/§1.1/§1.2 — 2026-10-07 yeniden inşa (orijinal tam metin kayıp; G9: kaynaksız detay yok)"
supersedes: ""
---

## §1 Bağlam

K3'ün 9 Tier / 575 csproj senaryosu kabul edilseydi ~1150 hedef oluşur,
restore/IDE çökerdi. Katman adları ve proje sayısı belirsiz kalsaydı herkes
kendi bazını kurardı.

## §2 Karar

**Baz:** 8 makro katman (l0..l7) · 52 düğüm (N01..N44) · **9 üretim + 1-2 test
projesi** (en fazla 11 `.csproj`) · çift TFM `net48;net10.0-windows`.

**§2.1 Makro katman tablosu (bağlayıcı):**

| # | Makro | Proje (.csproj) | Tek satır sorumluluk |
|---|---|---|---|
| 1 | l0 Abstractions | VersaCoder.Abstractions | port arayüzleri, Result<T>, event sözleşmesi |
| 2 | l1 Domain | VersaCoder.Domain | entity, invariant, domain event |
| 3 | l2 Application | VersaCoder.Application | use case, CQRS handler, orkestrasyon |
| 4 | l3 Crosscutting | VersaCoder.Crosscutting | logging, IClock, dosya yardımcıları |
| 5 | l4 Infrastructure | VersaCoder.Infrastructure | ADO.NET + Dapper, dosya, git, LLM sağlayıcı |
| 6 | l5 Protocol | VersaCoder.Protocol | mesaj şeması, tool sözleşmeleri |
| 7 | l6 Host | VersaCoder.Host | composition root, DI, appsettings |
| 8 | l7 UI + CLI | VersaCoder.Desktop + VersaCoder.Cli | RibbonForm / CLI ayrıştırma |
| — | test | VersaCoder.Tests (+ ArchitectureTests) | kanıt üretir; %90 coverage hedefi |

**Yön:** `l7→l6→l5→l4→l2→l1→l0`; `l3` yalnız `l0`'a; **iki doğrudan ok yasağı**:
`l4→l2 X` · `l5→l4 X` (port `l0`'da, uygulama `l4`'te). Matris NetArchTest
`LayerRulesTests` ile kanıtlanır; elle çizilmiş matris kanıt değildir.

**§2.3 Düzeltmeler:** 6 zorunlu düzeltme + tek matris kaynağı (NetArchTest).
**§3:** 3 tur / 21 ajan oylaması (4/10, 4/5, 6/6). **§4:** build sırası
`(l0→l1→l2 seri) → (l3‖l4‖l5) → (l6→l7)`; eşik: 5440 kayıt (marj 0).

## §3 Alternatifler

- **575 csproj (K3):** restore çöker — RED (§3 A3). ❌
- **Tek proje monolit:** katman denetimi imkânsız. ❌
- **8 makro + 52 düğüm + 9+2 proje (seçilen):** denetlenebilir baz. ✅

## §4 Sonuçlar

**Ters-senaryo:** K3'ün 9 Tier / 575 csproj senaryosu kabul edilseydi ~1150
hedef oluşur, restore/IDE çökerdi.

**Yakalama:** `checklists/layer-boundary-check.md` + `PROJECTS.md` §3 altı düzeltme.

## §5 Ölçülebilir çıktı

| Dosya | Çıktı |
|---|---|
| `PROJECTS.md` §3 + `architecture/index.md` | 6 düzeltme = 6/6 kapandı |

## §6 Kaynak

- brain.md §1 satır 028 · §1.1 · §1.2
- .ai/CLAUDE.md §5 (Katman Yasakları — "ADR-028 bağlayıcı")
- log.md 2026-09-29 kaydı (§2.1 9 satır tablo, §2.3 6 düzeltme, §3 oylama)

## §7 İlgili

- [ADR-013-layered-architecture-52-nodes.md](ADR-013-layered-architecture-52-nodes.md)
- [ADR-030-5000-layers-class-level.md](ADR-030-5000-layers-class-level.md)
- [layer-boundary-check](../../checklists/layer-boundary-check.md)
- [adr-index](../adr-index.md) · [brain](../../brain.md)
