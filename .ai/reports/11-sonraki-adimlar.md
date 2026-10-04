---
title: "VersaCoder — Sonraki Adımlar"
type: report
category: reports
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "Siradaki islerin sirasi, sahibi ve somut sure tahmini - her adim tek adim, olculebilir ciktili"
---

# VersaCoder — Sonraki Adımlar

**Zorunlu bağlantılar:** [index](../index.md) · [WORKFLOW](../WORKFLOW.md) · [CLAUDE](../CLAUDE.md) · [keys](../keys.md) · [09-teslim-raporu.md](09-teslim-raporu.md) · [10-bilinen-eksikler.md](10-bilinen-eksikler.md)

> **Tek cümlelik sorumluluk:** *Sırada hangi iş var, kim yapacak, kaç dakika sürecek* sorusunun tek cevabıdır.

---

## §0 Özet (Answer First)

**Sıradaki iş: GATE-004a'yı kapat — 500 MD + 0 bozuk link + 0 mojibake (≈2 sa, batch B1/B2/B4/B8/B9/B10 + 9 dosya onarımı).** Ardından **GATE-003 HARD STOP**: 7 wireframe yaz + `ui-design/README.md` ekran listesini düzelt + kullanıcı onayı (**≈90 dk**) — onaysız tek satır `.cs` yazılmaz (G12). Sonrası `src/` iskeleti (≈90 dk → GATE-004) → kodlama + test (≈6 sa → GATE-005) → opencode ADR'leri (≈90 dk) → doğrulama + ilk commit + teslim (GATE-006). Kalan toplam **≈5 sa 55 dk**.

---

## §1 Adım Tablosu (sıra zorunlu)

| # | Adım | Sahip | Çıktı / kanıt | Süre |
|---|---|---|---|---|
| 1 | Mojibake 9 dosya + `ui-design/README.md` (frontmatter `scope` + ekran listesi) onarımı | docs + ui-designer | `find-mojibake` = 0 | **≈15 dk** |
| 2 | Eksik batch'ler: B8 checklists · B9 prompts · B10 playbooks · `knowledge/*/index` (REZERVE adlar) | batch yazarları | bozuk link ↓ | **≈60 dk** |
| 3 | Kalan `knowledge/` (B1/B2/B4) + toplam **500 MD** | batch yazarları | `measure-vault` TotalMD ≥ 500 | **≈60 dk** |
| 4 | Link taraması 0 + ölçüm + **GATE-004a sunumu** | architect → kullanıcı | `check-links` 0 · `measure` PASS | **≈15 dk** |
| 5 | **GATE-003:** 7 wireframe (`01-start-page` … `07-settings`) + `index/flows/component-inventory` | ui-designer | `ui-design/` = 10 dosya | **≈60 dk** |
| 6 | **GATE-003 kullanıcı onayı** (HARD STOP) | **kullanıcı** | onay kutusu işaretli | **≈10 dk** |
| 7 | `VersaCoder.slnx` + `Directory.Build.props` + `Directory.Packages.props` (CPM) + 13 proje | architect + developer | restore 0 hata | **≈75 dk** |
| 8 | `dotnet build` iki bacak + **ilk git commit** → **GATE-004** | developer | `[DOTNET_BUILD_CIKTISI]` | **≈15 dk** |

---

## §2 Devam Adımları (GATE-004 sonrası)

| # | Adım | Sahip | Kanıt | Süre |
|---|---|---|---|---|
| 9 | L0..L3 kod + birim test (TDD) | developer + tester | `dotnet test` yeşil | ≈2 sa |
| 10 | L4 Dapper + saf SQL migration + entegrasyon (SQLite) | developer + tester | 0 başarısız | ≈1 sa 30 dk |
| 11 | L5..L7 + formlar (onaylı wireframe üzerinden) | developer | build yeşil | ≈1 sa 30 dk |
| 12 | NetArchTest + **GATE-005** | tester + reviewer | 0 ihlal + PASS | ≈1 sa |
| 13 | ADR-006..011 + `ITool`/`ILLMProvider` (OpenRouter `/free` doğrulaması) | architect + developer | ADR `accepted` | ≈90 dk |
| 14 | Doğrulama kutularının doldurulması + **GATE-006 teslim** | reviewer | `07-dogrulama-raporu.md` dolu | ≈40 dk |

---

## §3 Kritik Yol ve Bağımlılıklar

```
adim 1-4  ->  GATE-004a  ->  adim 5  ->  GATE-003 (KULLANICI)  ->  adim 7-8  ->  GATE-004
                                                                      ->  adim 9-12 -> GATE-005
                                                                      ->  adim 13-14 -> GATE-006
```

| # | Kural | Yer |
|---|---|---|
| 1 | Bir part bitmeden diğeri başlamaz | WORKFLOW §1 |
| 2 | GATE-003 **öncesi** `.cs` yazılmaz (G12) | WORKFLOW §2.4 |
| 3 | Gate onayı yalnız **kullanıcıya** ait | AGENTS.md §3.2 #10 |
| 4 | Kanıtsız "bitti" yasak; kutu dolmazsa `[KANIT_BEKLIYOR]` | CLAUDE.md G5 |
| 5 | Toplam kalan bütçe ≈5 sa 55 dk (WORKFLOW toplamı ≈8 sa 10 dk) | WORKFLOW §2 |

---

## §4 Bugün İçin Tek Adım

**Adım 1 → ≈15 dk:** `scripts/find-mojibake.ps1` çıktısındaki 9 dosyayı UTF-8'e onar + `ui-design/README.md`'ye `scope:` alanı ekle. Sonra `find-mojibake` = 0 ölç.
