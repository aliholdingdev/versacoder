---
title: "Versa Coder — Audit Trail"
type: log
category: audit
date: 2026-08-25
updated: 2026-08-25
status: active
version: 1.0.0
---

# Versa Coder — Audit Trail (log.md)

**⚠️ APPEND-ONLY:** Bu dosyaya yalnızca ekleme yapılabilir, mevcut kayıtlar değiştirilemez veya silinemez.

---

## Format

```
[YYYY-MM-DD HH:mm:ss] [LEVEL] [AGENT] [ACTION] — Detay
```

| Alan | Tanım |
|------|-------|
| Timestamp | ISO 8601 formatında tarih/saat |
| Level | INFO, WARN, ERROR, CRITICAL |
| Agent | İşlemi yapan agent (MO, build, plan, vb.) |
| Action | Yapılan işlem |
| Action | Detaylı açıklama |

---

## Son Kayıtlar

[2026-08-25 18:00:00] [INFO] [MO] [SESSION_INIT] — Vault sistemi başlatıldı
[2026-08-25 18:00:01] [INFO] [MO] [VAULT_LOAD] — CLAUDE.md yüklendi
[2026-08-25 18:00:02] [INFO] [MO] [VAULT_LOAD] — AGENTS.md yüklendi
[2026-08-25 18:00:03] [INFO] [MO] [VAULT_LOAD] — WORKFLOW.md yüklendi
[2026-08-25 18:00:04] [INFO] [MO] [VAULT_LOAD] — brain.md yüklendi
[2026-08-25 18:00:05] [INFO] [MO] [CONTEXT_ASSEMBLY] — Context birleştirme tamamlandı

---

## Log Format Detayı

| Alan | Tanım | Format |
|------|-------|--------|
| Timestamp | ISO 8601 formatında tarih/saat | `[YYYY-MM-DD HH:mm:ss]` |
| Level | Log seviyesi | `[INFO]`, `[WARN]`, `[ERROR]`, `[CRITICAL]` |
| Agent | İşlemi yapan agent | `[MO]`, `[build]`, `[plan]`, vb. |
| Action | Yapılan işlem | `[SESSION_INIT]`, `[VAULT_LOAD]`, vb. |
| Detail | Detaylı açıklama | Serbest metin |

---

## Log Seviyeleri

| Seviye | Kullanım |
|--------|----------|
| INFO | Normal işlemler |
| WARN | Uyarı gerektiren durumlar |
| ERROR | Hatalı işlemler |
| CRITICAL | Kritik hatalar |

---

## Log Detayları

### 1. INFO Seviyesi Kullanım Alanları

| Action | Açıklama |
|--------|----------|
| SESSION_INIT | Oturum başlatıldı |
| VAULT_LOAD | Vault dosyası yüklendi |
| CONTEXT_ASSEMBLY | Context birleştirme |
| TASK_ASSIGNED | Görev atandı |
| TASK_COMPLETED | Görev tamamlandı |
| AGENT_STARTED | Agent başlatıldı |
| AGENT_COMPLETED | Agent tamamlandı |
| TOOL_CALL | Tool çağrısı |
| FILE_READ | Dosya okundu |
| FILE_WRITE | Dosya yazıldı |
| GIT_COMMIT | Git commit yapıldı |
| TEST_RUN | Test çalıştırıldı |
| LEARNING_SAVE | Öğrenme kaydedildi |

### 2. WARN Seviyesi Kullanım Alanları

| Action | Açıklama |
|--------|----------|
| SLOW_RESPONSE | Yavaş yanıt (>15s) |
| HIGH_TOKEN_USAGE | Yüksek token kullanımı |
| LOW_COVERAGE | Düşük test coverage |
| VAULT_SYNC_NEEDED | Vault senkronizasyonu gerekli |
| SESSION_TIMEOUT | Oturum zaman aşımı |
| RETRY_ATTEMPT | Yeniden deneme |
| CACHE_MISS | Önbellek isabetsizliği |

### 3. ERROR Seviyesi Kullanım Alanları

| Action | Açıklama |
|--------|----------|
| PROVIDER_ERROR | LLM sağlayıcı hatası |
| TOOL_ERROR | Tool hatası |
| SESSION_ERROR | Oturum hatası |
| VAULT_ERROR | Vault hatası |
| GIT_ERROR | Git hatası |
| TEST_ERROR | Test hatası |
| LEARNING_ERROR | Öğrenme hatası |

### 4. CRITICAL Seviyesi Kullanım Alanları

| Action | Açıklama |
|--------|----------|
| LAYER_VIOLATION | Katman ihlali |
| SECURITY_BREACH | Güvenlik açığı |
| DATA_CORRUPTION | Veri bozulması |
| SYSTEM_FAILURE | Sistem arızası |
| GATE_BYPASS | Gate ihlali |
| HALLUCINATION_SPREAD | Hallüsinasyon yayılımı |

---

## Log Kategorileri

### 1. Session Kategorisi

| Action | Seviye | Açıklama |
|--------|--------|----------|
| SESSION_INIT | INFO | Oturum başlatıldı |
| SESSION_END | INFO | Oturum sonlandırıldı |
| SESSION_PAUSE | INFO | Oturum duraklatıldı |
| SESSION_RESUME | INFO | Oturum devam ettirildi |
| SESSION_TIMEOUT | WARN | Oturum zaman aşımı |
| SESSION_ERROR | ERROR | Oturum hatası |

### 2. Vault Kategorisi

| Action | Seviye | Açıklama |
|--------|--------|----------|
| VAULT_LOAD | INFO | Vault yüklendi |
| VAULT_SAVE | INFO | Vault kaydedildi |
| VAULT_SYNC | INFO | Vault senkronize edildi |
| VAULT_ERROR | ERROR | Vault hatası |
| VAULT_CORRUPTION | CRITICAL | Vault bozulması |

### 3. Agent Kategorisi

| Action | Seviye | Açıklama |
|--------|--------|----------|
| AGENT_STARTED | INFO | Agent başlatıldı |
| AGENT_COMPLETED | INFO | Agent tamamlandı |
| AGENT_FAILED | ERROR | Agent başarısız oldu |
| AGENT_TIMEOUT | WARN | Agent zaman aşımı |
| AGENT_ESCALATED | WARN | Agent eskale edildi |

### 4. Task Kategorisi

| Action | Seviye | Açıklama |
|--------|--------|----------|
| TASK_CREATED | INFO | Görev oluşturuldu |
| TASK_ASSIGNED | INFO | Görev atandı |
| TASK_COMPLETED | INFO | Görev tamamlandı |
| TASK_FAILED | ERROR | Görev başarısız oldu |
| TASK_CANCELLED | INFO | Görev iptal edildi |
| TASK_ESCALATED | WARN | Görev eskale edildi |

### 5. Tool Kategorisi

| Action | Seviye | Açıklama |
|--------|--------|----------|
| TOOL_CALL | INFO | Tool çağrıldı |
| TOOL_COMPLETED | INFO | Tool tamamlandı |
| TOOL_FAILED | ERROR | Tool başarısız oldu |
| TOOL_TIMEOUT | WARN | Tool zaman aşımı |

### 6. Git Kategorisi

| Action | Seviye | Açıklama |
|--------|--------|----------|
| GIT_COMMIT | INFO | Commit yapıldı |
| GIT_PUSH | INFO | Push yapıldı |
| GIT_PULL | INFO | Pull yapıldı |
| GIT_MERGE | INFO | Merge yapıldı |
| GIT_ERROR | ERROR | Git hatası |

### 7. Test Kategorisi

| Action | Seviye | Açıklama |
|--------|--------|----------|
| TEST_RUN | INFO | Test çalıştırıldı |
| TEST_PASSED | INFO | Test geçildi |
| TEST_FAILED | ERROR | Test başarısız oldu |
| TEST_COVERAGE | INFO | Coverage raporu |

### 8. Learning Kategorisi

| Action | Seviye | Açıklama |
|--------|--------|----------|
| LEARNING_SAVE | INFO | Öğrenme kaydedildi |
| LEARNING_LOAD | INFO | Öğrenme yüklendi |
| LEARNING_ERROR | ERROR | Öğrenme hatası |

---

## Log Analizi

### 1. Günlük İstatistikler

| Metrik | Hedef |
|--------|-------|
| Toplam log sayısı | Değişken |
| INFO oranı | > %80 |
| WARN oranı | < %15 |
| ERROR oranı | < %5 |
| CRITICAL oranı | < %1 |

### 2. Haftalık İstatistikler

| Metrik | Hedef |
|--------|-------|
| Toplam session | > 10 |
| Ortalama session süresi | < 2 saat |
| Toplam token kullanımı | < 1M |
| Hata oranı | < %5 |

### 3. Aylık İstatistikler

| Metrik | Hedef |
|--------|-------|
| Toplam geliştirme süresi | > 100 saat |
| Kod kalitesi | > %90 |
| Test coverage | > %90 |
| Deployment sayısı | > 4 |

---

## Log Saklama

### 1. Saklama Kuralları

| Süre | Eylem |
|------|-------|
| 0-7 gün | Aktif log |
| 7-30 gün | Sıkıştırılmış log |
| 30-90 gün | Arşivlenmiş log |
| 90+ gün | Silinmiş (yedekli) |

### 2. Arşivleme Prosedürü

| Adım | Aksiyon | Sorumlu |
|------|---------|---------|
| 1 | Eski logları tara | System |
| 2 | Sıkıştırılmış kopya oluştur | System |
| 3 | Arşiv dizinine taşı | System |
| 4 | Orijinali sil | System |
| 5 | Log kaydı oluştur | MO |

---

## Log Security

### 1. Güvenlik Kuralları

| Kural | Açıklama |
|-------|----------|
| Append-only | Sadece ekleme yapılabilir |
| Encryption | Hassas bilgiler şifreli |
| Access control | Yetkili erişim |
| Audit | Tüm erişimler loglanıyor |

### 2. Güvenlik Prosedürleri

| Prosedür | Açıklama |
|----------|----------|
| Log integrity | Bütünlük kontrolü |
| Tamper detection | Değişiklik tespiti |
| Secure storage | Güvenli depolama |
| Backup | Yedekleme |

---

## Quality Report

| Metrik | Değer |
|--------|-------|
| Version | 1.0.0 |
| Status | Active |
| Log Levels | 4 |
| Categories | 8 |
| Retention Rules | 4 |
| Security Rules | 4 |

---

**Authority:** Vault Steward
**Last Updated:** 2026-08-25
**Mode:** Red Team · Human Mode · Truth Mode