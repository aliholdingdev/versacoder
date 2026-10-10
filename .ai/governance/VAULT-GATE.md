---
title: "VAULT-GATE — .ai değişiklik kapısı"
type: gate
category: "governance"
status: active
version: 1.0.0
updated: 2026-10-07
authority: primary
scope: "Zincirin başı: 10 kontrol + oturum protokolü; commit öncesi yeşil"
tags: [gate, vault-gate, validation, d13, d24]
source: "D13 + D21 + D24 kararları (2026-10-07 onaylı plan, ADR-032 §2.7-2.8)"
supersedes: ""
---

**Zorunlu Bağlantılar:** [[governance/WORKFLOW|WORKFLOW]] · [[CLAUDE|CLAUDE]] · [[.decisions/accepted/ADR-032-ai-control-plane|ADR-032]] · [[governance/audit|audit]] · [[checklists/vault-link-check|vault-link-check]]

# VAULT-GATE — `.ai` Değişiklik Kapısı

> **Tek cümlelik sorumluluk:** `.ai` vault'a her yazmadan **önce** ve commit
> **öncesi** 10 kontrolü koşar; yeşil olmadan vault değişikliği commit edilmez.

## §1 Zincir (D13/D21)

```text
VAULT-GATE -> GATE-004a -> GATE-003 -> GATE-004 -> GATE-005 -> GATE-006
```

VAULT-GATE yalnız `.ai/**` değişikliklerini kapsar; `src/**` kapıları (003..006)
aynı kalır. Ajan kapıyı **hazırlar**, kapatmaz (kullanıcı kapatır — G12).

## §2 Oturum Protokolü (D24 — 5 adım)

1. **Ön kontrol:** 10 betik koş (`scripts/check-*.ps1` — D40).
2. **Rol-paketi yükle:** allowed/forbidden context (D11/D16).
3. **İş:** değişiklik tasarımı/uygulaması.
4. **Onay:** 3 kademe granularity (D18) — düşük: paket · orta: dosya · yüksek: etki + tek tek.
5. **Audit + log:** `governance/audit.md` onay kaydı + `ops/log.md` append satırı.

**Commit kuralı:** VAULT-GATE yeşil + onay kaydı olmadan commit yok.

## §3 10 Kontrol (D10/D40)

| # | Betik | Ne yakalar |
|---|---|---|
| 1 | `check-links.ps1` | kırık link (relative MD + wikilink) |
| 2 | `check-frontmatter.ps1` | 11 alan eksik/illegal FM |
| 3 | `check-mojibake.ps1` | encoding bozulması |
| 4 | `check-claims.ps1` | kanıtsız sayısal iddia |
| 5 | `check-authority.ps1` | iki primary / authority değeri illegal |
| 6 | `check-circular.ps1` | katman bağımlılık döngüsü (L4→L0 ters ok) |
| 7 | `check-orphan.ps1` | hiç linklenmeyen dosya + indeks kapsama |
| 8 | `check-duplicate.ps1` | duplicate authority (aynı kavram iki primary) |
| 9 | `check-stale.ps1` | bayat iddia/tarih/supersedes zinciri |
| 10 | `check-gate.ps1` | gate bypass + onay kaydı bütünlüğü |

Durum: **betikler Faz 4'te üretilecek** — bu dosya şimdiden hedefi ve
çagri arayuzunu sabitler (G9: betik yoksa "üretilecek" yazılır).

## §4 Kabul (D44 — VAULT-GATE yeşil = 7 madde)

1. 10 betik yeşil · 2. kırık link = 0 · 3. `.rules/` + `architecture/` + ADR-001..032 diskte ·
4. authority tek kaynaktan (kök SSOT etiketi yok) · 5. G-çakışma = 0 ·
6. FM 11 alan uyum + authority değeri geçerli · 7. audit + log kaydı.

## §5 İlgili

- [[governance/audit|audit]] · [[governance/versioning|versioning]] · [[governance/WORKFLOW|WORKFLOW]]
- [[.decisions/accepted/ADR-032-ai-control-plane|ADR-032]]
