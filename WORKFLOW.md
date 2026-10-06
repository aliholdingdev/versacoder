---
title: WORKFLOW - Kapı zinciri özeti (boot)
type: boot-instruction
category: root
status: active
version: 2.0.0
updated: 2026-10-05
authority: SSOT
scope: "Repo kökü; kapı adları ve sırası - kapı kriterleri .ai/WORKFLOW.md'de, durum kanıtı .ai/log.md'dedir"
---

# WORKFLOW — Kapı zinciri (özet)

**Zorunlu Bağlantılar:** [index](.ai/index.md) · [templates](.ai/.templates/index.md) · [WORKFLOW](.ai/WORKFLOW.md) · [checklists](.ai/checklists/index.md) · [log](.ai/log.md) · [AGENTS](.ai/AGENTS.md)

> **Tek cümlelik sorumluluk:** kapı adlarını ve sırasını tek satırda verir; kapı
> kriterleri, kanıt kutusu biçimi ve durum SSOT dosyalardadır — burada tekrarlanmaz.

## §1 Zincir

```text
GATE-004a (vault) -> GATE-003 (UI onayi, HARD STOP) -> GATE-004 (iskelet + ilk commit) -> GATE-005 (test) -> GATE-006 (teslim)
```

| Kapı | Ne kapatır | Checklist | Kanıt nerede aranır |
|---|---|---|---|
| GATE-004a | vault yeşil: ölçüm sayaçları sıfırlarda (link/mojibake/frontmatter) | `gate-004a-vault-kapanis.md` | `.ai/scripts/*.ps1` çıktısı |
| GATE-003 | ekran onayı — onaysız `src/**` kodu yasak (HARD STOP) | `gate-003-ui-onay.md` | kullanıcı onayı (tarihli, `.ai/log.md`) |
| GATE-004 | iskelet + 2 bacak build + dosya bazlı ilk commit | `gate-004-ilk-commit.md` | `dotnet build` + `dotnet test` çıktısı |
| GATE-005 | test + NetArchTest 0 ihlal | `gate-005-test-kapsami.md` | `dotnet test` çıktısı |
| GATE-006 | teslim | — (bu bandda tanımlı liste yok) | `WORKFLOW.md` §5 raporu |

## §2 Kural (tek satır)

- **Kapıyı yalnız kullanıcı açar**; RED veren kapı işi üreten ajana döndürür,
  başka ajana devredilmez — [`AGENTS`](.ai/AGENTS.md) §1.3.
- **Sıra atlanmaz:** `GATE-003` onayından **önce** `src/**` altına tek satır `.cs` yazılmaz.
- **Kapı RED'i strike sayılmaz** (gate kuralı, ajan yöntemi değil) — [`AGENTS`](.ai/AGENTS.md) §4.3.
- **Tam metin** (kapı kriterleri, kanıt kutusu `KANIT/DOSYA/EKSIK/SONRAKI`, süreler):
  [`WORKFLOW`](.ai/WORKFLOW.md) §2.
- **Günlük durum** (hangi kapı açık/kapalı, son onay ne zaman): [`log`](.ai/log.md)
  son kaydı — burada tahmin yazılmaz.
- **Kapalı soru:** "kapı kriteri tam nedir?" → `.ai/WORKFLOW.md`;
  "şimdi durum ne?" → `.ai/log.md`.

## §3 Şu anki durum (ölçüm 2026-10-05)

| Kapı | Durum | Kanıt |
|---|---|---|
| GATE-004a | **ÖLÇÜLEBİLİR** | 553 MD · BOZUK link 0 · MOJIBAKE 0 · frontmatter eksik 0 |
| GATE-003 | **AÇIK (HARD STOP)** | kullanıcı onayı yok — `src/**`a yazılmadı |
| GATE-004 | **BLOKE** | `src/`, `tests/`, `.sln`, `global.json` **YOK** → `dotnet build` `MSB1003` |
| GATE-005 | **BLOKE** | `tests/` YOK → `dotnet test` hedef bulamaz |
| GATE-006 | **BLOKE** | önceki kapılar kapalı |

## §4 Doğrulama

```powershell
# GATE-004a kaniti (bu makinede calisir)
powershell -NoProfile -ExecutionPolicy Bypass -File .\.ai\scripts\check-links.ps1
powershell -NoProfile -ExecutionPolicy Bypass -File .\.ai\scripts\find-mojibake.ps1
# durum kaniti (tahmin degil)
Get-Content .\.ai\log.md -Tail 5
```
