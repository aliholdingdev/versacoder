---
title: "Ana Kütük — .ai/log.md (append-only günlük)"
type: log
category: root
status: active
version: 1.0.0
updated: 2026-10-06
authority: SSOT
scope: "vault durumunun tek kaynağı: kapı durumu ve geçmiş olaylar bu dosyadan okunur; append-only, UTF-8 BOM'suz"
---

# Ana Kütük — `.ai/log.md`

**Zorunlu Bağlantılar:** [anayasa](../CLAUDE.md) · [kapı zinciri](../WORKFLOW.md) · [şablon dizini](.templates/index.md)

## §1 Amaç

Bu dosya **append-only günlük**tir; vault durumunun **tek kaynağı**dır.

- **Kapı durumu buradan okunur** (`§3`); başka dosyadaki durum kopyası geçersizdir.
- **Biçim:** `TARİH | ne yapıldı | kanıt | kapı` — 4 alan `|` ile ayrılır; tarih ISO `YYYY-MM-DD`.
- **Sıra:** en yeni en üstte (ters kronolojik).
- **Kanıt zorunlu:** kaynak belgede kanıtı olmayan olay yazılmaz; kanıt yoksa satır eklenmez.
- **Link kuralı (G7):** yalnız diskte var olan hedeflere link verilir; olmayan hedef düz metin yazılır.
- **Geçmiş silinmez (G11):** düzeltme, yeni kayıt satırı olarak eklenir.

## §2 Kayıtlar (en yeni en üstte)

**Kayıt sayısı:** 7

| # | Tarih | Ne yapıldı | Kanıt | Kapı |
|---|---|---|---|---|
| 7 | 2026-10-06 | **AŞAMA C 1. parti ölçümü** — `.ai/WORKFLOW.md` 416 sat/22,4 KB · `.ai/CLAUDE.md` 443 sat/23,9 KB · `log.md` 69 sat/4,3 KB · bayt `45,45,45` · placeholder 0 · kırık link **0** · mojibake 0 · `DOĞRULAMA GEREKLİ` toplam 33 (WORKFLOW 21 · CLAUDE 12) | bu oturum PowerShell ölçümü | `004a → 003 → 004 → 005 → 006` |
| 6 | 2026-10-06 | **AŞAMA C 1. parti başladı** — `.ai/CLAUDE.md`, `.ai/WORKFLOW.md`, `log.md` (bu dosya) üretildi; kullanıcı AŞAMA C kök dosyalarıyla başlamayı seçti | bu oturum · bu dosya | — |
| 5 | 2026-10-06 | Keşif: arşiv 12 dosya tarandı; master prompt arşivleri kayda alındı (Prompt A v2.0.0 · Prompt B v1.0.0) | [01-…v2](prompts/01-master-enterprise-engineering-v2.md) · [02-…v1](prompts/02-master-project-installation-v1.md) | — |
| 4 | 2026-10-06 | Adım 4 taxonomi raporu: Prompt A §6'nın 19 klasör adı metinde kayıp → **17 belgelenmiş + 2 KAYIP**; `.ai/` kök 0 MD, kök dosyalar AŞAMA C'ye ertelendi | [adim4-vault-skeleton-001.md](reports/adim4-vault-skeleton-001.md) | — |
| 3 | 2026-10-06 | Çakışma kaydı: **CF-01…CF-16** durum sütunu güncellendi (✅ düzeltildi · ⏳ karar · 📌 yalnız kayıt) | [conflict-register-001-master-prompts.md](reports/conflict-register-001-master-prompts.md) | — |
| 2 | 2026-10-06 | **§6 D1…D5 kararları kullanıcı onaylı** — D1 K15 → 26–44 ✅ · D2 22 phase / 5 gate / 6 etiket ✅ · D3 17+2 taxonomi ✅ · D4 G6 kapsamı `.templates/**` 🔄 üretimde · D5 K14 → 480–780, K15 → 26–46 ✅ | [conflict-register-001-master-prompts.md](reports/conflict-register-001-master-prompts.md) §6 | — |
| 1 | 2026-10-05 | **Kapı durumu ölçümü** (WORKFLOW §3): GATE-004a ÖLÇÜLEBİLİR · GATE-003 AÇIK (HARD STOP) · GATE-004/005/006 BLOKE | [WORKFLOW](../WORKFLOW.md) §3 | `004a → 003 → 004 → 005 → 006` |

## §3 Kapı Durumu (tek kaynak)

**Kaynak:** [WORKFLOW](../WORKFLOW.md) §3 — **ölçüm 2026-10-05**. Yeni ölçüm bu tabloya **ek**lenir, satır silinmez (G11).

| Kapı | Durum | Kanıt |
|---|---|---|
| GATE-004a | **ÖLÇÜLEBİLİR** | 553 MD · bozuk link 0 · mojibake 0 · frontmatter eksik 0 |
| GATE-003 | **AÇIK (HARD STOP)** | kullanıcı onayı yok — `src/**`a yazılmadı |
| GATE-004 | **BLOKE** | `src/`, `tests/`, `.sln`, `global.json` YOK → `dotnet build` `MSB1003` |
| GATE-005 | **BLOKE** | `tests/` YOK → `dotnet test` hedef bulamaz |
| GATE-006 | **BLOKE** | önceki kapılar kapalı |

## §4 Doğrulama (salt-okunur)

```powershell
$f='C:\versacoder\.ai\log.md'
(Get-Content $f -Encoding UTF8).Count                                   # 60–150 satır
([System.IO.File]::ReadAllBytes($f))[0..2] -join ','                    # 45,45,45 olmalı
Select-String -LiteralPath $f -Pattern '\]\(([^)#]+)\)' -AllMatches |
  %{ $_.Matches } | %{ $_.Groups[1].Value } | Sort-Object -Unique |
  ?{ -not (Test-Path (Join-Path 'C:\versacoder\.ai' $_)) }              # BOŞ olmalı
```

## Append Kuralları

- **Append-only:** yeni kayıt **en üste** eklenir; mevcut satır silinmez, üstüne yazılmaz.
- **Silme / overwrite** yalnız **kullanıcı onayı** ile yapılır (G7); onay yoksa işlem yapılmaz.
- **Düzeltme:** eski satır durur; yeni satır üstte, eski kaydı işaretleyerek eklenir (G11).
- **Kodlama:** UTF-8 **BOM'suz** (ilk 3 bayt `45,45,45`); mojibake yasak (G9) — Türkçe karakterler ç ğ ı İ ö ş ü doğru yazılır.
- **Kanıt ve link:** her kayıtta kanıt (dosya adı) bulunur; link yalnız var olan hedefe verilir (G7).
