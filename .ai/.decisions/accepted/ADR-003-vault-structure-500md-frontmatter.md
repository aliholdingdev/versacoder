---
title: "ADR-003 — Vault Yapısı: 500+ MD, Frontmatter Zorunlu, 30–88 KB"
type: adr
category: decisions
status: accepted
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: ".ai vault'unun dosya ölçeği, frontmatter standardı, kalite bandı ve link bütünlüğü kuralı"
---

# ADR-003 — Vault Yapısı: 500+ MD, Frontmatter Zorunlu

**Durum:** accepted · **Tarih:** 2026-09-25 · **Karar sahibi:** `architect` · **Hedef bacak:** vault dokümantasyon (TFM bağımsız)

**İlgili:** [[../../index.md]] · [[../../CLAUDE.md]] · [[../../WORKFLOW.md]] · [[ADR-001-dual-target-net48-net10.md]] · [[ADR-002-handwritten-content-no-copypaste.md]] · [[ADR-004-devexpress-universal-v261-office2010.md]]

---

## Context

CoreMusic vault'u (~600 MD, kök 31–94 KB) yalnız **yapı** referansıdır; VersaCoder vault'u `C:\versacoder\.ai` altında sıfırdan kuruluyor. 2026-09-25 denetiminde: **81 MD**, 14 kök belge (12'si 30–88 KB bandında, `index.md` 8,5 KB ve `keys.md` 8,1 KB band dışı) ve **120 bozuk iç link** tespit edildi. Dizinler noktalı adlandırıldı: `.agents · .templates · .decisions · .rules · .sql · .obsidian`.

## Decision

1. **Ölçek:** hedef **≥500 MD**; kök belgeler istisnasız **30–88 KB**.
2. **Frontmatter:** her `.md` = 8 alan (`title, type, category, status, version, updated, authority: SSOT, scope`); eksik alan = dosya vault'a ait değildir.
3. **Link:** göreli yol, hedef diskte yoksa link kaldırılır ya da hedef oluşturulur → **bozuk link = 0** (GATE-004a kanıtı).
4. **Noktalı dizinler:** `.agents/ .templates/ .decisions/ .rules/ .sql/ .obsidian/` — nokta kullanılmazsa link kırılır.
5. **Tarama:** `scripts/check-links.ps1` her vault part'inde + PART 8'de.

## Alternatifler

| # | Alternatif | + | − | Neden reddedildi |
|---|---|---|---|---|
| A | **Seçilen:** 500+ MD / 30–88 KB / 0 bozuk link | Aranabilir derinlik, SSOT, kanıtlanabilir kalite | ≈90+ dk/katalog | — |
| B | 80–120 MD kısa dosya | ≈20 dk | Yüzeysel, tekrarlı soru, GATE-004a bandı sağlanmaz | Gate şartı |
| C | Tek dev dosya (embed) | Link sorunu yok | 500 KB+ dosya okunamaz/editör kilitlenir | Bütçe kuralı (AGENTS.md §6) |

## Consequences

- **(+)** Kalite kanıtı otomatik (`check-links.ps1` sayısal çıktı); `index.md`/`keys.md` tek doğru cevabı verir; yeni dosyalar `.templates/documentation/` kalıbıyla üretilir.
- **(−)** Her büyük part ≈90 dk bütçeye eklenir; elle eklenen frontmatter'sız dosya GATE-004a'yı kırar.

## Web kanıtı

Dış kaynak gerekmiyor (iç tercih). Kalite bandı çıtası = CoreMusic yerel envanteri (2026-09-25).

## Kabul kriterleri

- [ ] `.ai` altında ≥500 `.md`
- [ ] 14 kök belge 30–88 KB
- [ ] `scripts/check-links.ps1` → 0 bozuk link
- [ ] Her dosyada 8 alanlı frontmatter
