---
title: "VersaCoder — Sürüm ve Yayın Şartnamesi (Release)"
type: spec
category: spec
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "Sürümleme (semver + etiket), changelog üretimi ve paket/artifact çıkarma koşulları — GATE-006 girişi"
---

# VersaCoder — Sürüm ve Yayın Şartnamesi (`spec/release-spec.md`)

**Zorunlu Bağlantılar:** [index](../index.md) · [CLAUDE.md](../CLAUDE.md) · [keys](../keys.md) · [WORKFLOW](../WORKFLOW.md) · [architecture/index](../architecture/index.md) · [build-spec](build-spec.md) · [packaging-spec](packaging-spec.md) · [deployment-spec](deployment-spec.md)

> **Tek cümlelik sorumluluk:** *Sürüm numarası ne zaman artar, changelog nereden üretilir ve hangi koşullar sağlanınca paket çıkartılır* sorusunun tek cevabıdır.

---

## §0 Özet (Answer First)

Sürüm **`vMAJOR.MINOR.PATCH`** ve tek kaynağı git etiketidir; changelog **git geçmişinden** `Added / Changed / Fixed / Security / Removed` kategorileriyle üretilir; paket/artifact çıkarma yalnız **iki bacak build + test yeşil ve vault 0 bozuk link** koşuluyla yapılır — geri alma planı yazılmadan release **GATE-006 RED**'dir.

---

## §1 Kapsam / Kapsam Disi

| # | Kapsam | Kapsam dışı (nerede) |
|---|---|---|
| 1 | Semver kuralı ve etiketleme | Derleme komutları: [build-spec](build-spec.md) |
| 2 | Changelog kategorileri ve üretimi | Paket sürüm kilidi (NuGet): [packaging-spec](packaging-spec.md) |
| 3 | Artifact/çıktı listesi ve checksum | Makineye kurulum: [deployment-spec](deployment-spec.md) |
| 4 | Rollback planı zorunluluğu | DB geri alma: [migration-spec](migration-spec.md) |
| 5 | Release kanıtları (GATE-006) | Onay mercii: [WORKFLOW](../WORKFLOW.md) §3 |

> **YASAK:** Elle yazılmış/changelog kopyası · etiketsiz artifact · geri alma plansız yayın · sürümle protocol sürümünü karıştırmak (bkz. [protocol-layer-spec](protocol-layer-spec.md) §2.3).

---

## §2 Davranış

### §2.1 Sürüm kuralı

| Değişiklik | Ne artar | Örnek | Onay |
|---|---|---|---|
| Kırıcı davranış / protokol MAJOR | **MAJOR** | `v0.3.0` → `v1.0.0` | ADR + kullanıcı (G8) |
| Geriye uyumlu özellik / yeni tool | **MINOR** | `v0.3.0` → `v0.4.0` | `architect` |
| Hata düzeltme / belge | **PATCH** | `v0.3.1` → `v0.3.2` | `reviewer` |
| Vault-only değişiklik | PATCH (ya da etiket yok) | 0.3.x | `docs` notu |

### §2.2 Changelog kategorileri

| Kategori | Ne girer | Kaynak |
|---|---|---|
| `Added` | Yeni spec/katman/tool/ekran | commit + gate raporu |
| `Changed` | Davranış değişikliği | ADR + diff |
| `Fixed` | Hata düzeltmesi | RED → GREEN kanıtı |
| `Security` | Güvenlik düzeltmesi | `security-spec` bulgusu |
| `Removed` | Kapsam çıkarımı | kullanıcı onayı (kapsam yetkisi) |

### §2.3 Paket çıkarma koşulları

```
  [ ] iki bacak build 0 hata        (build-spec §2.1)
  [ ] iki bacak test 0 failed       (test-strategy §4)
  [ ] check-links 0 bozuk + mojibake 0  (docs-spec)
  [ ] coverage >= %90 ya da gerekce  (GATE-005)
  [ ] changelog uretilmis            (git log + CHANGELOG)
  [ ] geri alma plani yazilmis       (yoksa GATE-006 RED)
        |
        v
  dotnet publish (2 TFM) --▶ artifact + checksum --▶ git etiketi vMAJOR.MINOR.PATCH
```

| # | Çıktı | İçerik |
|---|---|---|
| 1 | `publish/net48/` | Uygulama + DevExpress v26.1 DLL'leri |
| 2 | `publish/net10.0-windows/` | Aynı, `win-x64` RID ile |
| 3 | `publish/<tfm>/cli/` | GUI'siz CLI çıktısı |
| 4 | `publish/sql/` | Migration dosyaları (uygulama içinde uygulanır) |
| 5 | `checksum` + sürüm notu | `reports/` altına yazılır |

---

## §3 TFM Notu

| Bacak | Artifact | Not |
|---|---|---|
| `net48` | Framework-dependent (.NET Framework 4.8 kurulu makine) | `self-contained` **değil** |
| `net10.0-windows` | `win-x64`, tercihen framework-dependent | RID yalnız bu bacakta |
| Etiket | **Tek** sürüm etiketi iki artifact'i kapsar | İki ayrı sürüm = RED |

---

## §4 Test / Kabul

| ID | Ne test edilir | Kanıt |
|---|---|---|
| RL-01 | Etiket ↔ changelog ↔ artifact eşleşmesi | Sürüm notunda üçü de aynı numara |
| RL-02 | İki bacak artifact mevcut | `publish/` listesi + checksum |
| RL-03 | Changelog otomatik/kurala uygun | Kategori satırları (`changelog-generate` kuralları) |
| RL-04 | Rollback planı yazıldı | Release notunda "geri alma" bölümü |
| RL-05 | Kurulu sürüm açılabilir | Açılış smoke testi ≤ 2 sn (`performance-spec` B7) |

**Değişiklik günlüğü:** 2026-09-25 — ilk yazım (architect, GATE-004a batch).
