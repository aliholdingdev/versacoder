---
title: "Versa Coder — ProjeSpec İndeksi"
type: catalog
category: project-spec
date: 2026-08-26
updated: 2026-08-26
status: active
version: 1.0.0
authority: Single Source of Truth (SSOT)
governance: Red Team · Human Mode · Truth Mode
reference:
  authority: ".ai/spec/index.md"
  source_of_truth: ".ai/CLAUDE.md · .ai/AGENTS.md · .ai/brain.md"
---

# Versa Coder — ProjeSpec İndeksi

**Zorunlu Bağlantılar:** [[CLAUDE.md]] · [[AGENTS.md]] · [[WORKFLOW.md]] · [[brain.md]]

---

## 1. Amaç

Versa Coder projesinin kapsamlı teknik şartnamesinin (ProjeSpec) indeksidir. Tüm mimari kararlar, özellik tanımları, yol haritası ve uygulama detayları bu belgede referans olarak verilir.

---

## 2. Spec Dosyaları

| # | Dosya | İçerik | Satır | Durum |
|---|-------|--------|-------|-------|
| 1 | `versacoder-spec.md` | Ana teknik şartname | ~3000+ | ✅ Aktif |
| 2 | `versacoder-spec-summary.md` | Öz贝壳 şartname | ~500 | ✅ Aktif |
| 3 | `versacoder-spec-supplement.md` | Ek notlar, güncelleme notları | ~200 | 🔄 Geliştirme |

---

## 3. Spec İçerik Haritası

### 3.1 Ana Şartname Bölümü

| Bölüm | İçerik | Önem |
|-------|--------|------|
| §1 Genel Bakış | Proje tanımı, vizyon, hedefler | ZORUNLU |
| §2 Teknoloji Yığını | Framework, kütüphaneler, araçlar | ZORUNLU |
| §3 Mimari Tasarım | Katmanlar, modüller, bağımlılıklar | ZORUNLU |
| §4 Agent Sistemi | 7 agent tanımı, roller, akışlar | YÜKSEK |
| §5 Özellik Kataloğu | Tüm özellikler ve öncelikler | YÜKSEK |
| §6 Veri Modeli | Entity, Value Object, Schéma | YÜKSEK |
| §7 API Tasarımı | Endpoint'ler, protocol, format | ORTA |
| §8 UI/UX Tasarımı | Ekranlar, akışlar, wireframe | ORTA |
| §9 Güvenlik | Yetkilendirme, şifreleme, audit | YÜKSEK |
| §10 Performans | Metrikler, hedefler, optimizasyon | ORTA |
| §11 Test | Strateji, kapsam, araçlar | YÜKSEK |
| §12 Dağıtım | CI/CD, ortamlar, versiyonlama | ORTA |
| §13 Yol Haritası | Aşamalar, zaman çizelgesi | YÜKSEK |
| §14 Kod Standartları | İsimlendirme, format, kural | ZORUNLU |

### 3.2 Ek Belgeler

| Dosya | İçerik | Kullanım |
|-------|--------|----------|
| ADR-001~008 | Mimari Karar Kayıtları | Karar gerekçeleri |
| brain.md | Mimari kararlar haritası | Referans |
| WORKFLOW.md | Geliştirme süreçleri | Referans |
| AGENTS.md | Agent tanımları | Referans |

---

## 4. Spec Versiyonlama

| Version | Tarih | Değişiklik |
|---------|-------|-----------|
| 1.0.0 | 2026-08-26 | İlk şartname yayınlandı |

---

## 5. Spec Kullanım Akışı

```
Kullanıcı İsteği
  → [1. Spec'yi oku] — İlgili bölümleri belirle
    → [2. Karar ver] — Uygun teknoloji/design seç
      → [3. Kod yaz] — Şartnamee uygun kod üret
        → [4. Doğrula] — Şartnamee uyumluluk kontrol
          → [5. Test et] — Kapsama ve kalite kontrol
            → [6. Logla] — Audit trail oluştur
```

---

## 6. Hızlı Erişim

| İhtiyaç | Bölüm | Dosya |
|---------|-------|-------|
| Teknoloji seçimi | §2 | `versacoder-spec.md` |
| Mimari karar | §3 | `brain.md` |
| Agent rolleri | §4 | `AGENTS.md` |
| Özellik listesi | §5 | `versacoder-spec.md` |
| Veri modeli | §6 | `brain.md` §9 |
| Kod standartları | §14 | `CLAUDE.md` §13 |
| Yol haritası | §13 | `versacoder-spec.md` |

---

**Authority:** Vault Steward
**Last Updated:** 2026-08-26
**Mode:** Red Team · Human Mode · Truth Mode
