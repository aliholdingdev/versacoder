---
title: "VersaCoder — README"
type: readme
category: docs
status: active
version: 1.0.0
updated: 2026-10-05
authority: SSOT
scope: "Depo kökü README: ne yapar, hızlı başlangıç, katmanlar, yapılandırma, testler, mimari not, referanslar"
---

# VersaCoder

> **Ajan/üretim SSOT'lu `.ai/` vault'u + henüz kurulmamış VersaCoder masaüstü kod tabanı.**
> Şu an depo **yalnızca belgedir**: `src/`, `tests/` ve `.sln` diskte **yok** (2026-10-05 ölçümü).

## 1. Ne Yapar (Answer First)

- **vault:** 553 Markdown dosyasıyla kural, ADR, katman spec ve şablon tutar.
- **kapı:** `GATE-003` kullanıcı onayından önce tek satır `.cs` yazılmaz.
- **kod:** 9 üretim projesi (K0..K8) + test projesi **henüz yazılmadı** — `DOĞRULAMA GEREKLİ`.

## 2. Hızlı Başlangıç

```powershell
# on (bu makinede dogrulanmis)
git status --short
dotnet --version                                   # 10.0.301
powershell -NoProfile -ExecutionPolicy Bypass -File .\.ai\scripts\check-links.ps1
powershell -NoProfile -ExecutionPolicy Bypass -File .\.ai\scripts\measure-vault.ps1
```

```powershell
# kod kismi - .sln yokken CALISMAZ (DOĞRULAMA GEREKLİ)
dotnet build                                       # .sln YOK -> "MSB1003"
dotnet test                                        # tests/ YOK -> "hedef bulunamadi"
```

**Sıralama:** `.ai/.templates/index.md` → görev şablonu → `.ai/.rules/` → kod → `rules-check`.

## 3. Katmanlar / Kullanım

| Katman | Sorumluluk | Referans | Diskte |
|---|---|---|---|
| `.ai/.templates/` | 38 şablon, 10 alt klasör | [templates](./.ai/.templates/index.md) | ✅ 38 MD |
| `.ai/.rules/` | kalıcı kural metni | [rules](./.ai/.rules/index.md) | ✅ 18 MD |
| `.ai/.decisions/` | ADR-001..030 (0 boşluk) | [adr-index](./.ai/.decisions/adr-index.md) | ✅ 32 MD |
| `.ai/architecture/` | katman spec + catalog | [architecture](./.ai/architecture/index.md) | ✅ 129 MD |
| `.ai/checklists/` + `.ai/.agents/` | gate listesi · 7 ajan profili | [index](./.ai/index.md) | ✅ 32 MD · 7 MD |
| `.ai/scripts/` | 14 PS1 ölçüm betiği | [check-links.ps1](./.ai/scripts/check-links.ps1) | ✅ 14 PS1 |
| `src/**` | 9 üretim projesi (K0..K8) | `CLAUDE.md` §B5 | ❌ **YOK** |
| `tests/**` | unit + integration, hedef %90 | — | ❌ **YOK** |

## 4. Yapılandırma

| Anahtar | Varsayılan | Not |
|---|---|---|
| `global.json` | SDK 10.0.301 | dosya **YOK** — `DOĞRULAMA GEREKLİ` |
| `Directory.Packages.props` | CPM kilidi | dosya **YOK** — `DOĞRULAMA GEREKLİ` |
| Frontmatter | 8 alan, UTF-8 (BOM'suz) | `.ai/**` 553/553 uyumlu |
| Token bütçesi | 50.000 / oturum | kök dosya payı ≤ %3 |
| DevExpress | **yalnız v26.1** (ADR) | paket henüz kurulmadı |
| Veri erişimi | ADO.NET + Dapper | EF Core **yok** (G2 · ADR-005) |

## 5. Testler

```powershell
# su an calisir (belge dogrulamasi)
powershell -NoProfile -ExecutionPolicy Bypass -File .\.ai\scripts\check-links.ps1
# kod testleri - tests/ YOK oldugu icin CALISMAZ (DOĞRULAMA GEREKLİ)
dotnet test --filter "FullyQualifiedName~VersaCoder"
```

## 6. Mimari Not

```text
C:\versacoder
+-- CLAUDE.md ............. kural + K0..K8 + G1..G12 (SSOT baglantisi)
+-- AGENTS.md ............. golge (20 satir, "Kaynak: CLAUDE.md")
+-- README.md ............. bu dosya
+-- WORKFLOW.md ........... kapi/kapı ozeti
+-- .ai/ .................. 553 MD vault
     +-- .templates/ ...... 38   sablon
     +-- .rules/ .......... 18   kural
     +-- .decisions/ ...... 32   (ADR-001..030)
     +-- architecture/ .... 129  katman spec
     +-- scripts/ ......... 14   ps1
+-- src/ .................. YOK (GATE-004 onayi bekliyor)
+-- tests/ ................ YOK
```

Yön: `K6 -> K7 -> {K5, K4} -> K3 -> K2 -> K1 -> K0`; yukarı bakan referans ihlaldir.

## 7. Referanslar

| # | Kaynak | Yol |
|---|---|---|
| 1 | Kök kural (SSOT) | [CLAUDE.md](./CLAUDE.md) |
| 2 | Gölge ajan dosyası | [AGENTS.md](./AGENTS.md) |
| 3 | Kapı özeti | [WORKFLOW.md](./WORKFLOW.md) |
| 4 | Vault navigasyonu | [.ai/index.md](./.ai/index.md) |
| 5 | Şablon envanteri | [.ai/.templates/index.md](./.ai/.templates/index.md) |
| 6 | Mimari indeks | [.ai/architecture/index.md](./.ai/architecture/index.md) |
