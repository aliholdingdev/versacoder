---
title: "Versa Coder — Ultra Düşünme Protokolü"
type: protocol
category: thinking
date: 2026-08-25
updated: 2026-08-25
status: active
version: 1.0.0
---

# Versa Coder — Ultra Düşünme Protokolü

**⚠️ ZORUNLULUK:** Tüm agent'lar kod yazmadan önce bu protokolü uygulamak ZORUNDADIR.

---

## 1. Amaç

AI ajanlarının kod yazmadan önce sistematik düşünmesini sağlayan **5 adımlı düşünme protokolüdür**. Bu protokol, hataları önler, kod kalitesini artırır ve mimari tutarlılığı sağlar.

---

## 2. 5 Adımlı Düşünme Protokolü

### Adım 1: Vault Oku (Max 25s)

```
OKU → CLAUDE.md (guardrails) → AGENTS.md (agent sınırları) → WORKFLOW.md (süreçler)
  → brain.md (mimari kararlar) → ROLE.md (rol tanımı)
```

| Kontrol | Kaynak | Timeout |
|---------|--------|---------|
| Guardrails kontrolü | CLAUDE.md §9 | 5s |
| Agent sınırları | AGENTS.md §5 | 5s |
| Workflow kuralları | WORKFLOW.md §7 | 5s |
| Mimari kararlar | brain.md §6 | 5s |
| Rol tanımı | ROLE.md §2 | 5s |

### Adım 2: Bağlamı Anla

```
ANLA → Domain, katman, dosyalar, bağımlılıklar
```

| Kontrol | Soru |
|---------|------|
| Domain | Bu görev hangi domain'de çalışıyor? |
| Katman | Hangi katmanda (L0-L7)? |
| Dosyalar | Hangi dosyalar etkilenecek? |
| Bağımlılıklar | Hangi bağımlılıklar var? |
| Mevcut kod | Mevcut kod kalıpları neler? |

### Adım 3: Hata Kontrolü

```
KONTROL → Syntax, imports, types, style, security
```

| Kontrol | Araç |
|---------|------|
| Syntax | LSP, Roslyn |
| Imports | Namespace kontrolü |
| Types | Type safety |
| Style | C# coding standards |
| Security | OWASP kuralları |

### Adım 4: Sonuç Tahmini

```
TAHMİN ET → Etki alanı, edge cases, performance
```

| Kontrol | Soru |
|---------|------|
| Etki alanı | Bu değişiklik kimleri etkiler? |
| Edge cases | Olası边缘 durumlar neler? |
| Performance | Performans etkisi nedir? |
| Backward compat | Geriye uyumluluk korunuyor mu? |

### Adım 5: Doğrulama

```
DOĞRULA → LSP, typecheck, test, template uyumu
```

| Kontrol | Araç |
|---------|------|
| LSP | Language Server Protocol |
| TypeCheck | Derleyici kontrolü |
| Test | xUnit testleri |
| Template | Template uyumluluğu |

---

## 3. Düşünme Formatı

```markdown
## Düşünme Süreci

### 1. Vault Oku
- [x] CLAUDE.md okundu
- [x] AGENTS.md okundu
- [x] WORKFLOW.md okundu

### 2. Bağlam Analizi
- Domain: [domain adı]
- Katman: L[X] - [katman adı]
- Dosyalar: [liste]

### 3. Hata Kontrolü
- [ ] Syntax kontrolü
- [ ] Type safety
- [ ] Security check

### 4. Sonuç Tahmini
- Etki alanı: [açıklama]
- Edge cases: [liste]
- Performance: [etki]

### 5. Doğrulama
- [ ] LSP pass
- [ ] TypeCheck pass
- [ ] Test pass
```

---

## 4. Timeout Kuralları

| Aşamal | Max Süre | Aşım |
|--------|----------|------|
| Vault Oku | 25s | Devam et, log WARN |
| Bağlam Analizi | Değişken | Devam et |
| Hata Kontrolü | Anlık | Devam et |
| Sonuç Tahmini | Değişken | Devam et |
| Doğrulama | Anlık | DUR, düzelt |

---

## 5. Düşünme Detayı

### 5.1 Vault Oku Detayı

| Kontrol | Kaynak | Timeout | Amaç |
|---------|--------|---------|------|
| Guardrails kontrolü | CLAUDE.md §9 | 5s | Guardrails'ları kontrol et |
| Agent sınırları | AGENTS.md §5 | 5s | Agent sınırlarını kontrol et |
| Workflow kuralları | WORKFLOW.md §7 | 5s | Workflow kurallarını kontrol et |
| Mimari kararlar | brain.md §6 | 5s | Mimari kararları kontrol et |
| Rol tanımı | ROLE.md §2 | 5s | Rol tanımını kontrol et |

### 5.2 Bağlam Analizi Detayı

| Kontrol | Soru | Kaynak |
|---------|------|--------|
| Domain | Bu görev hangi domain'de çalışıyor? | Domain driven design |
| Katman | Hangi katmanda (L0-L7)? | Clean Architecture |
| Dosyalar | Hangi dosyalar etkilenecek? | File system |
| Bağımlılıklar | Hangi bağımlılıklar var? | Dependency graph |
| Mevcut kod | Mevcut kod kalıpları neler? | Code analysis |

### 5.3 Hata Kontrolü Detayı

| Kontrol | Araç | Amaç |
|---------|------|------|
| Syntax | LSP, Roslyn | Syntax hatalarını kontrol et |
| Imports | Namespace kontrolü | Import hatalarını kontrol et |
| Types | Type safety | Type hatalarını kontrol et |
| Style | C# coding standards | Style hatalarını kontrol et |
| Security | OWASP kuralları | Güvenlik hatalarını kontrol et |

### 5.4 Sonuç Tahmini Detayı

| Kontrol | Soru | Kaynak |
|---------|------|--------|
| Etki alanı | Bu değişiklik kimleri etkiler? | Impact analysis |
| Edge cases | Olası边缘 durumlar neler? | Edge case analysis |
| Performance | Performans etkisi nedir? | Performance analysis |
| Backward compat | Geriye uyumluluk korunuyor mu? | Compatibility analysis |

### 5.5 Doğrulama Detayı

| Kontrol | Araç | Amaç |
|---------|------|------|
| LSP | Language Server Protocol | LSP hatalarını kontrol et |
| TypeCheck | Derleyici kontrolü | TypeCheck hatalarını kontrol et |
| Test | xUnit testleri | Test hatalarını kontrol et |
| Template | Template uyumluluğu | Template hatalarını kontrol et |

---

## 6. Düşünme Senaryoları

### 6.1 Yeni Dosya Oluşturma

| Adım | Aksiyon | Kontrol |
|------|---------|---------|
| 1 | Vault oku | Guardrails |
| 2 | Template seç | Template mandatory |
| 3 | Kod yaz | Coding standards |
| 4 | Test yaz | Coverage |
| 5 | Doğrula | LSP + TypeCheck |

### 6.2 Mevcut Dosyayı Düzenleme

| Adım | Aksiyon | Kontrol |
|------|---------|---------|
| 1 | Vault oku | Guardrails |
| 2 | Dosyayı oku | Mevcut kod |
| 3 | Değişikliği yap | In-place modification |
| 4 | Test çalıştır | Regression |
| 5 | Doğrula | LSP + TypeCheck |

### 6.3 Mimari Değişiklik

| Adım | Aksiyon | Kontrol |
|------|---------|---------|
| 1 | Vault oku | Guardrails |
| 2 | ADR kontrol et | ADR compliance |
| 3 | Mimari planı hazırla | Hard Gate |
| 4 | Onay al | Human approval |
| 5 | Uygula | Layer rules |
| 6 | Doğrula | Full test suite |

### 6.4 Bug Düzeltme

| Adım | Aksiyon | Kontrol |
|------|---------|---------|
| 1 | Vault oku | Guardrails |
| 2 | Root cause analizi | Debug |
| 3 | Düzeltmeyi yap | Fix |
| 4 | Test çalıştır | Regression |
| 5 | Doğrula | LSP + TypeCheck |

---

## 7. Düşünme Kalite Metrikleri

### 7.1 Metrikler

| Metrik | Hedef |
|--------|-------|
| Vault okuma oranı | %100 |
| Hata tespit oranı | > %90 |
| İlk seferde doğru | > %80 |
| Geri alma oranı | < %5 |

### 7.2 Monitoring

| Metrik | Kaynak | Sıklık |
|--------|--------|--------|
| Vault okuma | Log system | Her görev |
| Hata tespit | Bug tracker | Her düzeltme |
| İlk seferde doğru | Test results | Her test |
| Geri alma | Git log | Her commit |

---

## 8. Düşünme Entegrasyonu

### 8.1 Agent Entegrasyonu

| Agent | Entegrasyon |
|-------|-------------|
| Build Agent | Her kod yazma öncesi |
| Plan Agent | Her planlama öncesi |
| Explore Agent | Her analiz öncesi |
| General Agent | Her görev öncesi |

### 8.2 Tool Entegrasyonu

| Tool | Entegrasyon |
|------|-------------|
| Read | Dosya okuma öncesi |
| Write | Dosya yazma öncesi |
| Edit | Dosya düzenleme öncesi |
| Bash | Komut çalıştırma öncesi |

---

## 9. Düşünme Eğitim Materyali

### 9.1 Eğitim Konuları

| Konu | Açıklama |
|------|----------|
| Vault kullanımı | Vault nasıl okunur |
| Bağlam analizi | Bağlam nasıl analiz edilir |
| Hata kontrolü | Hatalar nasıl kontrol edilir |
| Sonuç tahmini | Sonuçlar nasıl tahmin edilir |
| Doğrulama | Nasıl doğrulanır |

### 9.2 Eğitim Materyalleri

| Materyal | Format |
|----------|--------|
| Vault usage guide | Markdown |
| Context analysis guide | Markdown |
| Error checking guide | Markdown |
| Prediction guide | Markdown |
| Verification guide | Markdown |

---

## 10. Quality Report

| Metrik | Değer |
|--------|-------|
| Version | 1.0.0 |
| Status | Active |
| Steps | 5 |
| Timeout Rules | 5 |
| Scenarios | 4 |
| Metrics | 4 |

---

**Authority:** Vault Steward
**Last Updated:** 2026-08-25