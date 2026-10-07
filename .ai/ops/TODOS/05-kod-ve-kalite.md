---
title: "TODOS Faz 05 — Kod Uygulaması ve Kalite Kapıları (GATE-005)"
type: todo
category: todos
status: blocked
version: 2.0.0
updated: 2026-10-07
authority: reference
scope: "l0-l7 uygulama kodunun katman katman nasil yazilacagi, NetArchTest 8x8, test kapsami ve cikis kriterleri — GATE-003 sonrasi acilir"
tags: [ops]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# Faz 05 — Kod Uygulaması ve Kalite (GATE-005)

**Zorunlu Bağlantılar:** [TODOS index](index.md) · [Faz 03](03-faz-d-iskelet.md) · [Faz 04](04-gate-003-wireframe-onay.md) · [gate-005-test-kapsami](../../checklists/gate-005-test-kapsami.md) · [layer-boundary-check](../../checklists/layer-boundary-check.md) · [coverage-gate-check](../../checklists/coverage-gate-check.md) · [ADR-028](../../.decisions/accepted/adr-028-layered-baseline-9-projects.md)

> **Tek cümlelik sorumluluk:** Onaylı 7 ekran ve iskeletten sonra kodun **hangi

### §0.1 Güncel Durum (2026-10-07 — ölçülmüş)

**Durum: BLOKE — src/ YOK, test calistirilamiyor**

| Gösterge | Değer | Hedef | Durum |
|---|---|---|---|
| src/ + tests/ | **YOK** | VAR | BLOKE |
| dotnet test | calisamaz (hedef yok) | 0 basarisiz | BLOKE |
| NetArchTest LayerRulesTests | calisamaz | 0 ihlal | BLOKE |
| checklist hazir mi | **24 liste VAR** | VAR | OK — gate-005-test-kapsami.md mevcut |

> Faz 03 bittikten sonra acilir. Siradaki teknik engel: iskelet + cift bacak build (GATE-004).

> **Kaynak:** bu blok 2026-10-07 disk ölçümünden gelir (os.walk + git + dotnet); eski tarihli ölçümler tarihleriyle korunmuştur (append-only).

> sırayla**, hangi katmanda ve hangi kalite kapılarından geçerek yazılacağının tek listesidir.

**Durum: KAPALI** (04 GATE-003 onayı + 03 iskelet biterse açılır).

---

## §0 Özet (Answer First)

Kod sırası **katman sırası = alttan yukarı** (l0 → l7); ters sıralamak NetArchTest RED
üretir. Her adım `diff + build + test` kanıtsız kapanmaz (AGENTS §1 developer).

| # | Katman | Proje | İlk iş | Kapı |
|---|---|---|---|---|
| 1 | l0 | `Abstractions` | portlar (`Result`, `IAgentStreamSink`) | build + 13 soru |
| 2 | l1 | `Domain` | varlıklar + invariant | test yeşil |
| 3 | l2 | `Application` | handler/use-case + validator | test yeşil |
| 4 | l3 | `Crosscutting` | log/cache/saat/config | test yeşil |
| 5 | l4 | `Infrastructure` | ADO.NET+Dapper + migration | DA checklist |
| 6 | l5 | `Protocol` | tool/agent şeması | şema round-trip |
| 7 | l6 | `Host` | DI + bootstrap | uygulama ayağa kalkar |
| 8 | l7 | `Desktop` + `Cli` | 7 ekran (onaylı wireframe) | GATE-003 ✓ |
| 9 | test | `Tests` + `ArchitectureTests` | kapsam + 8×8 | GATE-005 |

---

## §1 Adımlar (her adımda 3 kanıt zorunlu)

| # | Adım | Kanıt (zorunlu üçlü) | Süre |
|---|---|---|---|
| 1 | l0 portları yaz | `dotnet build` 0 hata + [layer-boundary-check](../../checklists/layer-boundary-check.md) 13 soru | 1 gün |
| 2 | l1 domain | `dotnet test` yeşil + invariant testi | 1 gün |
| 3 | l2 handler | test kırmızı→yeşil (TDD) | 2 gün |
| 4 | l3 crosscutting | log/cache birim testi | 1 gün |
| 5 | l4 data | DA-01..16 + `data-access-check` tümü | 2 gün |
| 6 | l5 şema | round-trip testi | 1 gün |
| 7 | l6 DI | uygulama başlıyor (smoke) | 1 gün |
| 8 | l7 ekranlar | `.cs + .Designer.cs + .resx` üçlüsü + U5 | 3 gün |
| 9 | kapsam + mimari | `dotnet test` + NetArchTest 8×8 | 1 gün |

**Toplam ≈ 13 gün** (tek geliştirici; ajan paralelleştirmesiyle kısalır).

---

## §2 Kalite Kapıları (her adımda)

| Kapı | Ne ölçer | RED koşulu |
|---|---|---|
| Build | `dotnet build -m` | 0 olmayan hata |
| Test | `dotnet test` | herhangi bir kırmızı |
| NetArchTest | 8×8 yön matrisi | ihlal > 0 |
| [layer-boundary-check](../../checklists/layer-boundary-check.md) | 13 soru | cevapsız soru |
| [data-access-check](../../checklists/data-access-check.md) | DA-01..16 | string-concat SQL / EF Core |
| [coverage-gate-check](../../checklists/coverage-gate-check.md) | kapsam %90 | < %90 |
| [secret-leak-check](../../checklists/secret-leak-check.md) | açık anahtar | > 0 |
| [gate-005-test-kapsami](../../checklists/gate-005-test-kapsami.md) | test paketi | [ ] madde |

- **3-STRIKE** (AGENTS §4): aynı iş 3 kez düşerse DUR + bloke raporu (`ETIKET · KONUM · ...`).
- **Yasak:** EF Core · v26.1 dışı DevExpress · `l7→l1/l4` · kod içi secret · string-concat SQL.

---

## §3 Tamamlanma (proje teslimi)

| # | Öğe | Kanıt |
|---|---|---|
| 1 | 9+2 proje yeşil | build + test çıktıları |
| 2 | NetArchTest 8×8 = 0 | ArchitectureTests |
| 3 | Kapsam ≥ %90 | coverage raporu |
| 4 | 7 ekran wireframe'e sadık | gate-003 7/7 |
| 5 | Vault hâlâ temiz | `check-links` 0, `find-mojibake` 0 |
| 6 | Gate'ler işaretli | 003 ✓ 004 ✓ 004a ✓ 005 ✓ |

---

## §4 Kabul Kriterleri (GATE-005)

- [ ] [gate-005-test-kapsami](../../checklists/gate-005-test-kapsami.md) tümü [x]
- [ ] Kapsam ≥ %90 ([coverage-gate-check](../../checklists/coverage-gate-check.md))
- [ ] NetArchTest 0 ihlal
- [ ] `data-access-check` + `secret-leak-check` 0 bulgu
- [ ] [documentation-check](../../checklists/documentation-check.md) [x] (README/vault güncel)

---

## §5 Kanıt

| # | Kanıt | Beklenen |
|---|---|---|
| P5-1 | `dotnet build -m` | 0 hata |
| P5-2 | `dotnet test` | yeşil, ≥ %90 |
| P5-3 | NetArchTest | 0 ihlal |
| P5-4 | checklist'ler | 003/004/004a/005 [x] |
| P5-5 | `log.md` | teslim kaydı |
