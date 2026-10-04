---
title: "Kalıcılık Şartnamesi - VersaCoder"
type: spec
category: spec
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "Tercih/oturum saklama: nereye yazılır, hangi veri hangi biçimde, şifreleme ve yazma atomizmi"
---

# Kalıcılık Şartnamesi — VersaCoder

**Zorunlu Bağlantılar:** [index](../index.md) · [CLAUDE.md](../CLAUDE.md) · [keys](../keys.md) · [WORKFLOW](../WORKFLOW.md) · [testing-rules](../.rules/testing-rules.md) · [architecture](../architecture/index.md) · [versacoder-spec](versacoder-spec.md) · [project-model](project-model-spec.md) · [settings](settings-spec.md) · [session-spec](session-spec.md)

> **Tek cümlelik sorumluluk:** *Hangi veri nereye, hangi biçimle ve hangi güvenle yazılır* sorusunun tek cevabıdır.
> Veri erişimi **ADO.NET + Dapper** ve **parametreli** sorgulardır; **EF Core ve string-concat SQL yok** (G2 / ADR-005).

---

## §0 Özet (Answer First)

Üç katman vardır: **(1) tercih/oturum JSON** (roaming `%AppData%\VersaCoder\`), **(2) SQLite veritabanı** (aynı kök, görev/oturum/proje kayıtları), **(3) günlük dosyaları** (`.log`, döner). Secret alanları `IStringProtector` ile **korunmuş** olarak saklanır; düz metin diskte yazılmaz (SEC-02). Tüm yazımlar atomiktir (geçici dosya + `File.Replace`), bağlantı dizesi ortam değişkeni/üretimde farklıdır ve **repo içinde düz metin yoktur**.

---

## §1 Kapsam / Kapsam Dışı

| # | Kapsam içi | Kapsam dışı | Nerede |
|---|---|---|---|
| 1 | Dosya yolları, biçimler (JSON/SQLite/log) | Varlık modeli (alanlar) | [project-model](project-model-spec.md) |
| 2 | Yazma atomizmi, kilit, kurtarma | Şema/migration SQL'inin kendisi (bkz. §2.3) | [persistence](persistence-spec.md) §2.3 |
| 3 | Secret koruma ve anahtar kullanımı | Şifreleme algoritması kararı | [error-handling](error-handling-spec.md) hata yolu |
| 4 | Tercihlerin okunma sırası (katmanlı config) | Ayar ekranı doğrulaması | [settings](settings-spec.md) |
| 5 | Oturum/kayıt yaşam döngüsü | Transcript/compaction | [session-spec](session-spec.md) |

---

## §2 Davranış

### §2.1 Yerleşim

```
 %AppData%\VersaCoder\
 +-- settings.json        tercihler (secret alanlari KORUNMUS)
 +-- state.json           son projeler, acik sekmeler, panel durumu
 +-- versacoder.db        SQLite (gorev, oturum, proje kayitlari)
 +-- logs\                app.log  (donen dosya, gunluk siniri)
```

### §2.2 Veri ↔ depo matrisi

| # | Veri | Depo | Biçim | Not |
|---|---|---|---|---|
| P1 | Tercihler (sağlayıcı, model, skin, dil) | `settings.json` | JSON | secret **korumalı** |
| P2 | Son projeler, açık sekmeler, panel durumu | `state.json` | JSON | atomik yazım |
| P3 | Görevler, oturum/transcript, proje kayıtları | `versacoder.db` | SQLite + Dapper | parametreli sorgu |
| P4 | Günlük / metrik | `logs\*.log` | metin (UTF-8) | dönen dosya |
| P5 | Dosya **içeriği** (kod) | projenin kendisi | UTF-8 | kopyalanmaz |

### §2.3 Kurallar

| # | Kural | Kabul |
|---|---|---|
| K1 | JSON yazımı **atomiktir** (`.tmp` → `File.Replace`) | yarım dosya okunamaz |
| K2 | DB erişimi yalnız **parametreli** sorgular (`@p1`); SQL kodda sabit | concat SQL = 0, EF Core = 0 |
| K3 | Migration = saf SQL + **version tablosu** (`schema_version`) | uygulama açılışında sürüm kontrolü |
| K4 | Secret yalnız `Protect` ile yazılır, okununca UI'a **dönmez** | diskte düz metin = 0 |
| K5 | Okuma sırası: `varsayılan (kod)` → `dosya` → `ortam değişkeni` | son okuyan kazanır (l3 `IAppConfig`) |
| K6 | Bozuk JSON/DB → yedek kopya + varsayana düş, **uygulama açılır** | kilitli açılış = 0 |

---

## §3 TFM Notu

| Konu | `net48` | `net10.0-windows` | Karar |
|---|---|---|---|
| JSON | `System.Text.Json` (mevcut paket sürümü) | aynı | **fark yok** (paket G8 ile kilitli) |
| SQLite sağlayıcısı | `Microsoft.Data.Sqlite` (aynı sürüm) | aynı | fark yok |
| Dosya yolu | `%AppData%` (BCL `Environment`) | aynı | fark yok |
| Kripto | BCL AES (l3 `IStringProtector`) | aynı | fark yok |

> `#if` yalnız dosya yolu/platform farkı gerekirse (hedef 0 — G6).

---

## §4 Test / Kabul

| # | Ne test edilir | Kanıt | Şart |
|---|---|---|---|
| 1 | Atomik yazım (çökme simülasyonu) | birim test | eski dosya okunabilir |
| 2 | Parametreli sorgu denetimi | statik tarama / NetArchTest | concat = 0 |
| 3 | Secret disk turu | tarama: `sk-`/`Password=` = 0 | düz metin = 0 (SEC-01/08) |
| 4 | Kurtarma (bozuk JSON/DB) | entegrasyon | açılış başarılı + uyarı |
| 5 | Bozuk link | `check-links.ps1` | 0 |

**Değişiklik günlüğü:** 2026-09-25 — ilk yazım (GATE-004a batch).
