---
title: "VersaCoder — Bilinen Eksikler"
type: report
category: reports
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "Vault ve proje genelinde acik eksikler: frontmatter'siz dosya, bozuk link, mojibake, NEEDS_VALIDATION maddeleri ve eksik batch'ler"
---

# VersaCoder — Bilinen Eksikler

**Zorunlu bağlantılar:** [index](../index.md) · [WORKFLOW](../WORKFLOW.md) · [CLAUDE](../CLAUDE.md) · [keys](../keys.md) · [02-vault-kurulum-raporu.md](02-vault-kurulum-raporu.md) · [08-risk-kayit-defteri.md](08-risk-kayit-defteri.md)

> **Tek cümlelik sorumluluk:** *Şu an bilinen ve gizlenmemesi gereken eksiklerin tam listesi nerede* sorusunun tek cevabıdır.

---

## §0 Özet (Answer First)

**Frontmatter'sız dosya YOK (0), CR-only YOK (0).** Açık eksikler: **46 bozuk link** (`knowledge/` 34 · `checklists/` 4 · `playbooks/` 2), **9 mojibake dosya** (8×`.templates/` + `scripts/measure-vault.md`), **216/500 MD**, eksik batch'ler (B8 checklists 4 · B9 prompts 30 · B10 playbooks 20 · B13 wireframe 7), **ADR-006..011** yazılmadı ve **3 `NEEDS_VALIDATION`** madde (Sqlite dual-TFM, `src/` yokluğu, wireframe varlıkları). Hiçbiri gizlenmedi; her madde sahip + kapanış kanıtıyla listeleniyor.

---

## §1 Frontmatter / Kodlama

| # | Kontrol | Ölçüm | Sonuç |
|---|---|---|---|
| 1 | İlk satır `---` (frontmatter) | `measure-vault-u8.ps1` → **NoFrontmatter 0** | ✅ eksik yok |
| 2 | CR-only satır sonu | **CROnly 0** | ✅ eksik yok |
| 3 | Mojibake (UTF-8 → ANSI bozulması) | **9 dosya** | 🔴 açık |
| 4 | Frontmatter 8 alan + `authority: SSOT` | `ui-design/README.md` → **7 alan (scope eksik)** | 🔴 açık |
| 5 | Kök MD 30–88 KB | 14/14 bandında | ✅ eksik yok |

**Mojibake dosyaları (9):** `.templates/index.md` · `.templates/session-log-template.md` · `.templates/agents/agents-template.md` · `.templates/agents/subagent-template.md` · `.templates/architecture/ascii-diagram-template.md` · `.templates/documentation/docs-md-template.md` · `.templates/infrastructure/configuration-template.md` · `.templates/infrastructure/provider-template.md` · `scripts/measure-vault.md`.

---

## §2 Bozuk Link (46)

| # | Kaynak klasör | Adet | Neden | Sahip |
|---|---|---|---|---|
| 1 | `knowledge/data/` + `knowledge/devexpress/` | 34 | hedef kardeş dosyalar henüz yazılmadı (B1/B3) | batch yazarı |
| 2 | `checklists/index.md` | 4 | `gate-004a-vault-kapanis.md` · `gate-003-ui-onay.md` · `gate-004-ilk-commit.md` · `gate-005-test-kapsami.md` **üretilmedi** (B8) | batch B8 |
| 3 | `playbooks/index.md` | 2 | `yeni-katman-ekleme.md` · `yeni-form-tasarlama.md` (B10) | batch B10 |
| 4 | `prompts/` · `knowledge/*/index` | rezerve adlar | `architect-prompt.md` · `developer-prompt.md` · `tester-prompt.md` · `reviewer-prompt.md` · alt index'ler | batch B4/B9 |
| — | **`reports/` (10 rapor)** | **0** | — | — |

---

## §3 NEEDS_VALIDATION Maddeleri

| # | Madde | Soru | Nerede çözülür | Sahip |
|---|---|---|---|---|
| 1 | `Microsoft.Data.Sqlite` | net48 + net10 ikili desteği var mı? (yalnız ikincil kaynak) | PART 5 NuGet teyidi → ADR-012 eki | developer |
| 2 | **`src/` yok** → build/test kanıtı üretilemiyor | — | PART 5 iskelet + PART 6 kodlama | developer |
| 3 | 7 wireframe varlığı | `ui-design/` = yalnız `README.md` | PART 4 (B13) + GATE-003 | ui-designer |
| 4 | ADR-006..011 | 6 ADR yazılmadı | PART 7 | architect |
| 5 | `scripts/measure-vault.ps1` | PS 5.1 ANSI okumasında parse hatası | `measure-vault-u8.ps1` geçici çözüm | architect |

---

## §4 Eksik Batch'ler (REZERVE adlar)

| Batch | Klasör | Beklenen | Yazılan (ölçüm anı) |
|---|---|---|---|
| B1 | `knowledge/devexpress/` | 60 | 18 |
| B3 | `knowledge/data/` | 25 | 26 ✅ |
| B2 | `knowledge/winforms/` | 30 | 3 |
| B4 | `knowledge/csharp/` | 25 | 1 |
| B8 | `checklists/` | 30 | 4 (index + 3) |
| B9 | `prompts/` | 30 | 1 (index) |
| B10 | `playbooks/` | 20 | 1 (index) |
| B13 | `ui-design/` | 7 wireframe + 3 ek | 1 (README) |

> Ölçüm anı **2026-09-25**; batch'ler paralel yazıldığı için sayılar artıyor → kapanışta yeniden ölç (`measure-vault-u8.ps1`).

---

## §5 Kapanış Sırası

| # | İş | Tahmin |
|---|---|---|
| 1 | Mojibake 9 + `ui-design/README.md` frontmatter/scope onarımı | ≈15 dk |
| 2 | Rezerve adların üretilmesi (B8/B9/B10 + `knowledge/*/index`) | ≈60 dk |
| 3 | Bozuk 46 linkin yeniden taranıp 0'a indirilmesi | ≈20 dk |
| 4 | `NEEDS_VALIDATION` 1 ve 4 numaralı maddeler | ≈40 dk |
| 5 | Toplam 500 MD hedefi | ≈2 sa |
