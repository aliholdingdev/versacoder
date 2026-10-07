---
title: "ADR-004 — DevExpress Universal 2026 / v26.1 + RibbonControlStyle.Office2010"
type: adr
category: decisions
status: accepted
version: 1.0.0
updated: 2026-09-25
authority: SSOT
scope: "DevExpress sürüm kilidi, ribbon stili tekeli, iki bacak uyumluluğunun web ile doğrulanması"
---

# ADR-004 — DevExpress v26.1 (tek sürüm) + Office2010 (tek ribbon stili)

**Durum:** accepted · **Tarih:** 2026-09-25 · **Karar sahibi:** `architect` · **Hedef bacak:** `net48 + net10.0-windows`

**İlgili:** [[../../index.md]] · [[../../CLAUDE.md]] · [[../../reports/01-web-kaynaklari.md]] · [[ADR-001-dual-target-net48-net10.md]] · [[ADR-005-ado-net-dapper-no-ef-core.md]]

---

## Context

Prompt §3 sürümü kilitleyor: **DevExpress Universal 2026 / v26.1**, ribbon **yalnız `RibbonControlStyle.Office2010`**. ai.01'deki `DevExpress.Win.All 24.2.3` referansı **REDDEDİLDİ** (R3). İkili bacak için sürümün her iki TFM'i desteklediği WEB GATE ile doğrulandı.

## Decision

1. **Tek sürüm:** DevExpress **v26.1** — CPM'te `DevExpress.*` yalnız `26.1.*`; başka sürüm = `G3` ihlali.
2. **Tek ribbon stili:** `RibbonControlStyle.Office2010` (Office2007/Unified yasak).
3. **İki bacak doğrulandı:** v24.2–v26.1 → `.NET Framework 4.6.2+` (**net48 ✓**); v25.2–v26.1 → `.NET 8/9/10` (**net10.0-windows ✓**).
4. **Designer:** her form `Form.cs + Form.Designer.cs + Form.resx` üçlüsü; `ResourceManager` adlarına elle dokunulmaz (son söz `ui-designer`).
5. **v26.2 geçişi ayrı ADR** (min .NET sürümü .NET 10 olacak; .NET Framework 4.6.2 kalıyor).

## Alternatifler

| # | Alternatif | + | − | Neden reddedildi |
|---|---|---|---|---|
| A | **Seçilen:** v26.1 + Office2010 | Web kanıtlı 2 bacak; AI Chat + Agent Skills var | Lisans mevcut | — |
| B | v25.2 | Kararlı, bilinen | AI Chat/Agent Skills yok | Prompt v26.1 şart |
| C | v26.2+ | En yeni | min .NET → 10; net48 riski; stabil değil | 2 bacak kısıtı |
| D | Başka ribbon stili | Görsel çeşitlilik | G3 ihlali, tutarsız tasarım | Yasak |

## Consequences

- **(+)** Paket sürümü tek yerden (CPM); tasarım tutarlı; iki bacakta tek kod.
- **(−)** v26.2 geçişi = yeni ADR + regresyon turu; ribbon özelleştirmeleri Office2010 şablonuna bağlı.

## Web kanıtı (erişim 2026-09-25)

| Bulgu | Kaynak |
|---|---|
| v24.2–v26.1: .NET Framework 4.6.2+ → **4.8 ✓** | `docs.devexpress.com/WindowsForms/8092/prerequisites` |
| v25.2–v26.1: .NET 8/9/**10 ✓** | aynı sayfa |
| v25.2–v26.1.5 = Supported + security update | `devexpress.com/support/versions.xml` |
| v26.2'de min .NET sürümü .NET 10 olacak | `community.devexpress.com/.../winforms-year-end-roadmap-v26-2.aspx` |
| v26.1 = WinForms AI Chat + AI Agent Skills | `devexpress.com/subscriptions/whats-new/` |

## Kabul kriterleri

- [ ] `Directory.Packages.props`: DevExpress yalnız `26.1.*`
- [ ] Kod taraması: `RibbonControlStyle.` → yalnız `Office2010`
- [ ] `dotnet build` iki bacakta 0 hata (GATE-004)
