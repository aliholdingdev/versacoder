---
title: "Adım 4 — .ai/ Omurga Taxonomisi (17/19 belgelenmiş)"
type: report
category: reports
status: active
version: 1.0.0
updated: 2026-10-06
authority: SSOT
scope: "Prompt A §6 kaybından sonra CoreMusic §4 + index §3.10 birleşiminden türetilen .ai/ klasör taxonomisi; kök dosyalar AŞAMA C'ye ertelendi"
---

# Adım 4 — `.ai/` Omurga Taxonomisi

**Zorunlu Bağlantılar:** [templates index](../.templates/index.md) · [arşiv A](../prompts/01-master-enterprise-engineering-v2.md) · [arşiv B](../prompts/02-master-project-installation-v1.md) · [çakışma kaydı](./conflict-register-001-master-prompts.md)

## §1 Kaynak sorunu (kayıp itirafı)

**D3**'te `Prompt A §6 = 19 klasör` esas alınmıştı ve bu karar CoreMusic 14 · B §66 15 · vault §3.10 12'yi *reddediyordu*. 2026-10-06 taraması:

| Kaynak | Aranan | Sonuç |
|---|---|---|
| Prompt Arşivi `versacoder proje planlaması ai.00–09` (22–120 KB) | `VAULT FIRST` · `19 klasör` · `MASTER ENTERPRISE ENGINEERING` | **0 eşleşme** |
| [`01-…v2.md`](../prompts/01-master-enterprise-engineering-v2.md) §1 | `Diskte orijinali` | **YOK** (kendi beyanı) |
| [`02-…v1.md`](../prompts/02-master-project-installation-v1.md) §2 | `§66` başlığı | yalnız `FINAL OUTPUT DOCUMENT SET` — klasör adı yok |

→ **19 klasörün adı hiçbir yerde saklanmamıştır.** Bu rapor o adları **uydurmadı**; iki mevcut belgeden birleşim türetti ve iki adlık boşluğu açıkça işaretledi (sıfır halüsinasyon · [`conflict-register`](./conflict-register-001-master-prompts.md) §6 **D3**).

## §2 Dayanak

1. **CoreMusic yapı referansı §4.2** — 14 dizin, `Test-Path ile doğrulandı` denetimiyle listelenmiş (yalnız yapı referansı, içerik kopyalanmadı — G1).
2. **[`index.md`](../.templates/index.md) §3.10 Dizin Ağacı** — envanterin kendi ölçümü: 3 VAR + 8 `YOK (AŞAMA C)` + `csharp` stoğu.

CoreMusic §4.2'nin 14'ü ile §3.10'un YOK listesinin kesişimi **`adr` ve `checklists`** ile birleşince **17** kayıt çıkar; `Prompt A §6`nın 19'undan **2 adlık fark** kalan kısımda kaynaktır.

## §3 Taxonomi (17 kayıt)

| # | Klasör | Dayanak | Durum |
|---|---|---|---|
| 1 | `.templates/` | §4.2 · §3.10 | **VAR** (51 MD) |
| 2 | `prompts/` | §4.2 · §3.10 | **VAR** (2 MD) |
| 3 | `reports/` | §4.2 · §3.10 | **VAR** (1 MD) |
| 4 | `.rules/` | §4.2 · §3.10 | AŞAMA C — üretilmez |
| 5 | `.decisions/` | §4.2 · §3.10 | AŞAMA C — üretilmez |
| 6 | `.agents/` | §4.2 · §3.10 | AŞAMA C — üretilmez |
| 7 | `architecture/` | §4.2 · §3.10 | AŞAMA C — üretilmez |
| 8 | `ui-design/` | §4.2 · §3.10 | AŞAMA C — üretilmez |
| 9 | `scripts/` | §4.2 · §4.4 · §3.10 | AŞAMA C — üretilmez |
| 10 | `checklists/` | §3.10 YOK | AŞAMA C — üretilmez |
| 11 | `adr/` | §3.10 YOK | AŞAMA C — üretilmez |
| 12 | `.sql/` | §4.2 | ertelenmiş — veri dizini |
| 13 | `.png/` | §4.2 (PNG = source of truth) | ertelenmiş — varlık dizini |
| 14 | `ecosystem/` | §4.2 | ertelenmiş — içerik yok |
| 15 | `servers/` | §4.2 | ertelenmiş — içerik yok |
| 16 | `archives/` | §4.2 | ertelenmiş — içerik yok |
| 17 | `.workflows/` | §5 (9 dosya) | ertelenmiş — içerik yok |
| **18–19** | **(bilinmiyor)** | Prompt A §6 | **KAYIP** — kullanıcıdan §6 istenmeli |

**Klasör oluşturmadım:** boş dizin git tarafından izlenmez ve AŞAMA C hedeflerine dokunur; iskelet bu raporun §3 tablosudur.

## §4 Kök hedef dosyalar — AŞAMA C

`.ai/` köğünde **0 MD** (ölçüm 2026-10-06). Üretilmeyecek, **ama linkler silinmeyecek** (G7 · [`index.md`](../.templates/index.md) L14 notu):

`index.md` · `CLAUDE.md` · `WORKFLOW.md` · `keys.md` · `log.md` · `.rules/*` · `.decisions/*` · `architecture/*` · `checklists/*` · `scripts/*`

`log.md` olmadığı için bu adımın oturum kaydı **`mem-save`** ile tutuldu.

## §5 Doğrulama

```powershell
$ai='C:\versacoder\.ai'
"kok MD = " + (Get-ChildItem $ai -File -Filter *.md -Force).Count      # beklenen 0
"alt dizin = " + (Get-ChildItem $ai -Directory -Force).Count            # beklenen 3
(Get-Content "$ai\reports\adim4-vault-skeleton-001.md" -Encoding UTF8).Count
([System.IO.File]::ReadAllBytes("$ai\reports\adim4-vault-skeleton-001.md"))[0..2] -join ','
```

Beklenen: kök `0` · dizin `3` · ilk 3 bayt `45,45,45` · 3–8 KB · kırık link `0`.

## §6 Referanslar

Yapı referansı: `coremusic-vault-template.md` §4 (salt okunur, G1). Taxonomi SSOT: [`index.md`](../.templates/index.md) §3.10. Karar kaydı: [`conflict-register`](./conflict-register-001-master-prompts.md) §6 **D3**. Sözleşme: [`new-doc-contract`](../.templates/new-doc-contract.md).
