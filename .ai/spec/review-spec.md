---
title: "VersaCoder — Kod İnceleme Şartnamesi (RED / GREEN)"
type: spec
category: spec
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
scope: "İnceleme ölçütleri, bulgu biçimi ve seviyeleri, PASS (GREEN) / RED kararının kanıt koşulu — `reviewer` rolünün tek kaynağı"
---

# VersaCoder — Kod İnceleme Şartnamesi (`spec/review-spec.md`)

**Zorunlu Bağlantılar:** [index](../index.md) · [CLAUDE.md](../CLAUDE.md) · [keys](../keys.md) · [WORKFLOW](../WORKFLOW.md) · [architecture/index](../architecture/index.md) · [test-strategy](test-strategy.md) · [docs-spec](docs-spec.md)

> **Tek cümlelik sorumluluk:** *İnceleme hangi ölçütlerle yapılır, bulgu nasıl yazılır ve PASS/RED neye kanıtlanır* sorusunun tek cevabıdır.
> Rol/SLA/devre akışı `.ai/AGENTS.md` §2.3 ve [WORKFLOW](../WORKFLOW.md) §3'tedir (tekrar yok); bu belge **ölçüt + karar kanıtı**dır.

---

## §0 Özet (Answer First)

İnceleme **ölçüt listesi üzerinden** yürür (anayasa G1..G12 + ilgili spec maddeleri), bulgu **`SEVIYE · dosya:satır · neden · düzeltme · kanıt`** biçimindedir ve karar **ikilidir: GREEN (PASS) ya da RED** — **kanıt yoksa PASS geçersizdir** (G5): `dotnet build`/`dotnet test` çıktısı ya da tarama sonucu olmadan GREEN yazılmaz.

---

## §1 Kapsam / Kapsam Disi

| # | Kapsam | Kapsam dışı (nerede) |
|---|---|---|
| 1 | İnceleme ölçütleri (statik + mimari + iki bacak) | Test **üretimi**: [test-strategy](test-strategy.md) |
| 2 | Bulgu biçimi, seviye tanımı, RED dönüş yolu | Rol yetkisi/SLA: `.ai/AGENTS.md` §2.3 |
| 3 | GREEN için zorunlu kanıt seti | Gate çıkış kontrolü: [WORKFLOW](../WORKFLOW.md) §3 |
| 4 | Vault/MD inceleme ölçütları | Belge kuralları: [docs-spec](docs-spec.md) |
| 5 | Performans ölçümünün incelemeye girişi | Ölçüm yöntemi: [performance-spec](performance-spec.md) |

> **YASAK:** Kanıtsız PASS (G5) · incelemede kod düzeltmek (RED → `developer`) · seviyesiz bulgu · "bence temiz" gerekçesi · iki bacaktan yalnız birini incelemek (G6).

---

## §2 Davranış

### §2.1 Ölçüt listesi (her incelemede tek tek)

| # | Ölçüt | Kontrol | Bağlı spec |
|---|---|---|---|
| 1 | Anayasa | G1..G12 tek tek | `CLAUDE.md` (link) |
| 2 | SQL/veri | EF Core 0 · concat 0 · parameterized · timeout | [data-layer-spec](data-layer-spec.md) |
| 3 | İki bacak | net48 **ve** net10 build/test yeşil | [build-spec](build-spec.md) |
| 4 | Katman yönü | NetArchTest 0 ihlal · `#if` yalnız platform | [protocol-layer-spec](protocol-layer-spec.md) |
| 5 | Güvenlik | Secret/izin/rolmelenme (SEC-01..16) | [security-spec](security-spec.md) |
| 6 | Vault/MD | Frontmatter 8 alan · link 0 bozuk · mojibake 0 | [docs-spec](docs-spec.md) |
| 7 | Performans | Bütçe ölçümü var mı (p95) | [performance-spec](performance-spec.md) |

### §2.2 Bulgu biçimi ve seviyeler

```
[SEVIYE] dosya/yolu.cs:123
  NEDEN   : neden yanlis (kural: G2 / DA-03 / spec §x)
  DuzELTME: ne degistirilmeli (tek somut islem)
  KANIT   : test/build ciktisi  -  ya da UNKNOWN
```

| Seviye | Ne sayılır | Sonuç | Dönüş |
|---|---|---|---|
| **CRITICAL** | Güvenlik / veri kaybı / anayasa ihlali | İş durur | → `developer` (≈10 dk) |
| **HIGH** | Mimari sözleşme / iki bacak kırığı | İş durur | → `developer`, gerekirse `architect` |
| **MEDIUM** | Test açığı, tekrar, ihlal edilebilirlik | Aynı oturumda kapanır | → `developer` |
| **LOW** | İsim/yorum/stil | Kuyruk | Sonraki part |

### §2.3 GREEN (PASS) koşulları

| # | Koşul | Kanıt |
|---|---|---|
| 1 | Ölçüt listesi tamamlandı (§2.1) | İnceleme raporu satırları |
| 2 | CRITICAL/HIGH **0** | Bulgu listesi |
| 3 | Build + test çıktısı eklendi | Konsol çıktısı (iki bacak) |
| 4 | Vault kontrolleri 0 bozuk / 0 mojibake | `check-links.ps1`, `find-mojibake.ps1` |
| 5 | SONRAKI ADIM yazıldı | Devre paketinin 4. maddesi |

```
  inceleme baslar --▶ olcut listesi (§2.1)
        |
        +-- CRITICAL/HIGH var  --▶ RED --▶ developer'a 4 maddeyle don
        |
        +-- yalniz MEDIUM/LOW  --▶ developer kapatir, yeniden bakilir
        |
        +-- 0 kritik + kanit ekli  --▶ GREEN (PASS) --▶ tester (test-strategy)
                                                   |
                                          3 RED dongusu --▶ 3-STRIKE / kullanici
```

---

## §3 TFM Notu

| Bacak | İnceleme | Not |
|---|---|---|
| `net48` | Build/test çıktısı **ayrı** | Yalnız modern bacak = eksik inceleme (G6) |
| `net10.0-windows` | Build/test çıktısı **ayrı** | Aynı |
| Paylaşımlı kod | Tek inceleme, **iki kanıt** | `#if` varsa platform satırı ayrıca bakılır |

---

## §4 Test / Kabul

| ID | Ne test edilir | Kanıt |
|---|---|---|
| RV-01 | Bulgu biçimi | Rapor satırları `SEVIYE · konum · neden · düzeltme · kanıt` içeriyor |
| RV-02 | Kanıtsız PASS yok | Çıktısız GREEN → PASS iptal |
| RV-03 | Ölçüt listesi | 7 satırın tamamı işaretli |
| RV-04 | RED dönüşü | RED bulgusu `developer`'a 4 maddeyle iletildi |
| RV-05 | Vault inceleme | Link/mojibake taramaları raporda (0/0) |

**Değişiklik günlüğü:** 2026-09-25 — ilk yazım (architect, GATE-004a batch).
