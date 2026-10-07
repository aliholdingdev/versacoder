---
title: "VersaCoder — Build Şartnamesi (İki Bacak + CI)"
type: spec
category: spec
status: active
version: 1.0.0
updated: 2026-09-25
authority: primary
scope: "İki bacak (`net48` + `net10.0-windows`) derleme sırası, koşullu derleme (`#if`) kuralı ve CI adımları — GATE-004 kanıtının tanımı"
tags: [spec]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# VersaCoder — Build Şartnamesi (`spec/build-spec.md`)

**Zorunlu Bağlantılar:** [index](../index.md) · [CLAUDE.md](../claude.md) · [keys](../governance/keys.md) · [WORKFLOW](../governance/workflow.md) · [architecture/index](../architecture/index.md) · [packaging-spec](packaging-spec.md) · [test-strategy](test-strategy.md) · [release-spec](release-spec.md)

> **Tek cümlelik sorumluluk:** *Hangi komutla, hangi sırayla, hangi koşullu derleme kuralıyla derlenir ve yeşil build kanıtı nedir* sorusunun tek cevabıdır.

---

## §0 Özet (Answer First)

Build = **restore → build (2×TFM) → test (2×TFM) → vault kontrolleri** sırasıdır; her iki bacakta da **0 hata / 0 yeni uyarı** (`TreatWarningsAsErrors`) şarttır ve **`#if` yalnız platform API farkındadır** — koşullu derleme ile iş mantığı bölünemez (G6, CS-12). Kanıt = konsol çıktısı, "geçti" yazımı değil (G5).

---

## §1 Kapsam / Kapsam Disi

| # | Kapsam | Kapsam dışı (nerede) |
|---|---|---|
| 1 | Yerel build sırası + iki bacak komutları | Paket sürüm kilidi: [packaging-spec](packaging-spec.md) |
| 2 | `#if` / koşullu derleme kuralı | Katman yönü denetimi: [protocol-layer-spec](protocol-layer-spec.md) |
| 3 | CI adımları (4 hücre) ve kanıt dosyaları | Yayın adımları/artifact: [release-spec](release-spec.md) |
| 4 | Vault ön kontrolleri (link/frontmatter) | Vault kuralı: [docs-spec](docs-spec.md) |
| 5 | Build hata modları ve sahipliği | Test yorumu/RED seviyesi: [review-spec](review-spec.md) |

> **YASAK:** Tek bacaklı build ile "yeşil" demek (G6) · uyarıyı gizlemek (CS-11) · `#if` içinde iş mantığı (CS-12) · kanıtsız "bitti" (G5).

---

## §2 Davranış

### §2.1 Yerel build sırası

| # | Adım | Komut | Şart |
|---|---|---|---|
| 1 | Restore | `dotnet restore VersaCoder.slnx` | 0 hata |
| 2 | Build (legacy) | `dotnet build -c Release -f net48` | **0 hata, 0 yeni uyarı** |
| 3 | Build (modern) | `dotnet build -c Release -f net10.0-windows` | **0 hata, 0 yeni uyarı** |
| 4 | Test | `dotnet test -f net48` ve `-f net10.0-windows` | 0 failed (GATE-005) |
| 5 | Vault | `scripts/check-links.ps1` · `find-mojibake.ps1` | 0 bozuk / 0 mojibake (GATE-004a) |

### §2.2 Koşullu derleme kuralı

| Durum | Karar | Örnek |
|---|---|---|
| Fark yalnız **platform API'sinde** | `#if` **kabul** — tek satır/tek metot | Ayrık dosya API'si, `ApplicationConfiguration` |
| Fark **iş mantığında** | `#if` **yasak** — ortak kod yaz | Farklı kural → iki kod tabanı, iki test yükü |
| Fark **paket/kütüphane** seviyesinde | `Directory.Packages.props` + proje referansı ile çöz | `#if` değil, sürüm kilidi (packaging-spec) |
| Belirsiz | `NEEDS_VALIDATION` → `architect` | Uydurma `#if` ile kapatmak yasak (G9) |

```
  dogru:                          yanlis:
  #if NET48                       #if NET48
    LegacyApi.Calistir()            if (siparisTipi == Eski) { ... }
  #else                          #else
    ModernApi.CalistirAsync()       if (siparisTipi == Yeni) { ... }
  #endif                         #endif
     |                                 |
  platform API farki             AYNI is kurali iki kez  -> RED (G6)
```

### §2.3 CI adımları (4 hücre)

| # | Hücre | Adım | Kanıt |
|---|---|---|---|
| 1 | `net48` build | restore → build `-f net48` | build log'u |
| 2 | `net10.0-windows` build | build `-f net10.0-windows` | build log'u |
| 3 | test × 2 bacak | `dotnet test` (2×) + NetArchTest | TRX / konsol |
| 4 | vault | link + mojibake + frontmatter | konsol çıktısı |

> Dört hücre **yeşil olmadan** publish yok; adımlar `.rules/deployment-guide.md` §2 ile aynıdır (tekrar yok).

---

## §3 TFM Notu

| Bacak | Derleme | Yaygın hata |
|---|---|---|
| `net48` | C# sürümü projeye göre sabit; BCL farkları `#if` ile | Modern API'nin gelmemesi |
| `net10.0-windows` | En yeni dil özellikleri | Ortak kodun modern sözdizimine kaçması (net48'de kırılır) |
| Ortak | `Nullable` açık, file-scoped namespace, `async` sonuna kadar (CS-01..04) | İki bacakta farklı uyarı seti = RED |

---

## §4 Test / Kabul

| ID | Ne test edilir | Kanıt |
|---|---|---|
| BD-01 | İki bacakta 0 hata | 2 ayrı `dotnet build` çıktısı (GATE-004) |
| BD-02 | `#if` yalnız platform API'si | `Select-String '#if (NET48\|NET10)'` satırları + gözden geçirme |
| BD-03 | Uyarı yok | `TreatWarningsAsErrors` aktif, konsol temiz |
| BD-04 | Vault kontrolleri | `check-links.ps1` 0 bozuk · `find-mojibake.ps1` 0 |
| BD-05 | CI 4 hücre | Pipeline çıktısı (adım bazlı log) |

**Değişiklik günlüğü:** 2026-09-25 — ilk yazım (architect, GATE-004a batch).
