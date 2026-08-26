---
title: "Versa Coder — Session Hafızası"
type: memory
category: session-state
date: 2026-08-25
updated: 2026-08-25
status: active
version: 1.0.0
---

# Versa Coder — Session Hafızası (MEMORY.md)

**Zorunlu Bağlantılar:** [[CLAUDE.md]] · [[WORKFLOW.md]] · [[log.md]]

---

## 1. Amaç

Bu dosya, Versa Coder'ın oturumlar arası persistent hafızasını yönetir. Her session başında okunur, sonunda güncellenir.

---

## 2. Session State Formatı

```markdown
## Aktif Session
- **Session ID:** [UUID]
- **Başlangıç:** [timestamp]
- **Son aktivite:** [timestamp]
- **Agent:** [agent adı]
- **Durum:** [active/paused/completed]

## Session Geçmişi
| # | Session ID | Tarih | Agent | Konu | Durum |
|---|------------|-------|-------|------|-------|
| 1 | ... | ... | ... | ... | ... |
```

---

## 3. Persistent State

| Alan | Tanım | Güncelleme |
|------|-------|-----------|
| `last_session_id` | Son oturum ID'si | Her session sonu |
| `last_agent` | Son kullanılan agent | Her task sonu |
| `total_sessions` | Toplam oturum sayısı | Her session başında |
| `total_tokens` | Toplam token kullanımı | Her LLM çağrısı |
| `active_branch` | Aktif git dalı | Dal değişikliğinde |

---

## 4. Session Branching

```
Session A (ana)
├── Session B (dal — farklı yaklaşım dene)
│   └── Session C (alt dal — derinlemesine analiz)
└── Session D (dal — alternatif çözüm)
```

| İşlem | Tanım |
|-------|-------|
| **Branch** | Yeni dal oluştur, mevcut geçmişi kopyala |
| **Fork** | Tamamen bağımsız dal oluştur |
| **Merge** | İki dalı birleştir |
| **Revert** | Önceki duruma geri dön |

---

## 5. Session Cleanup

| Kurallar | Değer |
|---------|-------|
| Max aktif session | 10 |
| Max bekleme süresi | 24 saat |
| Otomatik arşivleme | 7 gün |
| Max saklama süresi | 90 gün |

---

## 6. Session Detayları

### 6.1 Session State Yönetimi

| Durum | Tanım | Aksiyon |
|-------|-------|---------|
| Active | Oturum aktif | Devam et |
| Paused | Oturum duraklatıldı | Resume |
| Completed | Oturum tamamlandı | Arşivle |
| Archived | Oturum arşivlendi | Sil veya sakla |

### 6.2 Session Branching Detayı

| İşlem | Tanım | Kullanım |
|-------|-------|----------|
| Branch | Yeni dal oluştur, mevcut geçmişi kopyala | Farklı yaklaşım dene |
| Fork | Tamamen bağımsız dal oluştur | Bağımsız deneme |
| Merge | İki dalı birleştir | Sonuçları birleştir |
| Revert | Önceki duruma geri dön | Hatalı değişikliği geri al |

### 6.3 Session Cleanup Detayı

| Kurallar | Değer | Amaç |
|---------|-------|------|
| Max aktif session | 10 | Kaynak kullanımı |
| Max bekleme süresi | 24 saat | Otomatik temizlik |
| Otomatik arşivleme | 7 gün | Depolama optimizasyonu |
| Max saklama süresi | 90 gün | Uzun vadeli depolama |

---

## 7. Session Branching Prosedürü

### 7.1 Branch Oluşturma

| Adım | Aksiyon | Sorumlu |
|------|---------|---------|
| 1 | Mevcut session'ı kaydet | System |
| 2 | Yeni dal ID'si oluştur | System |
| 3 | Geçmiş kopyasını oluştur | System |
| 4 | Yeni dal'ı aktif yap | System |
| 5 | Log kaydı oluştur | MO |

### 7.2 Fork Oluşturma

| Adım | Aksiyon | Sorumlu |
|------|---------|---------|
| 1 | Mevcut session'ı kaydet | System |
| 2 | Yeni bağımsız dal ID'si oluştur | System |
| 3 | Temel bilgileri kopyala | System |
| 4 | Yeni dal'ı aktif yap | System |
| 5 | Log kaydı oluştur | MO |

### 7.3 Merge İşlemi

| Adım | Aksiyon | Sorumlu |
|------|---------|---------|
| 1 | Hedef dal'ı seç | Kullanıcı |
| 2 | Çakışmaları kontrol et | System |
| 3 | Çakışmaları çöz | Kullanıcı |
| 4 | Birleştirmeyi uygula | System |
| 5 | Log kaydı oluştur | MO |

### 7.4 Revert İşlemi

| Adım | Aksiyon | Sorumlu |
|------|---------|---------|
| 1 | Hedef noktayı seç | Kullanıcı |
| 2 | Geri alma planını oluştur | System |
| 3 | Değişiklikleri geri al | System |
| 4 | doğrulama yap | System |
| 5 | Log kaydı oluştur | MO |

---

## 8. Session State Makinesi

```
CREATED → ACTIVE → PAUSED → COMPLETED → ARCHIVED
   ↓        ↓        ↓          ↓
   ↓        ↓        ↓          ↓
   ↓        ↓        ↓          ↓
   ↓        ↓        ↓          ↓
   ↓        ↓        ↓          ↓
   ↓        ↓        ↓          ↓
   ↓        ↓        ↓          ↓
   ↓        ↓        ↓          ↓
   ↓        ↓        ↓          ↓
   ↓        ↓        ↓          ↓
```

| Durum | Tanım | Geçişler |
|-------|-------|----------|
| CREATED | Session oluşturuldu | → ACTIVE |
| ACTIVE | Oturum aktif | → PAUSED, → COMPLETED |
| PAUSED | Oturum duraklatıldı | → ACTIVE, → COMPLETED |
| COMPLETED | Oturum tamamlandı | → ARCHIVED |
| ARCHIVED | Oturum arşivlendi | — |

---

## 9. Session Veri Yapısı

```json
{
  "sessionId": "UUID",
  "startTime": "ISO8601",
  "lastActivity": "ISO8601",
  "agent": "agent-name",
  "status": "active|paused|completed|archived",
  "branch": "branch-name",
  "parentSessionId": "UUID|null",
  "messages": [
    {
      "role": "user|assistant|system",
      "content": "message content",
      "timestamp": "ISO8601",
      "tokenCount": 1234
    }
  ],
  "metadata": {
    "totalTokens": 12345,
    "totalMessages": 67,
    "toolCalls": 12,
    "filesModified": ["file1.cs", "file2.cs"]
  }
}
```

---

## 10. Session Memory Management

### 10.1 Memory Seviyeleri

| Seviye | Tanım | Saklama Süresi |
|--------|-------|----------------|
| Short-term | Son 10 mesaj | Aktif session |
| Medium-term | Son 100 mesaj | 7 gün |
| Long-term | Tüm mesajlar | 90 gün |
| Permanent | Öğrenilen bilgiler | Sonsuz |

### 10.2 Memory Optimizasyonu

| Teknik | Açıklama |
|--------|----------|
| Summarization | Uzun mesajları özetleme |
| Compression | Tekrar eden bilgileri sıkıştırma |
| Archiving | Eski session'ları arşivleme |
| Cleanup | Gereksiz bilgileri temizleme |

### 10.3 Memory Erişim Patternları

| Pattern | Kullanım | Örnek |
|---------|----------|-------|
| Sequential | Sıralı okuma | Session geçmişi |
| Random | Rastgele erişim | Belirli mesaj |
| Filtered | Filtrelenmiş erişim | Agent bazlı |
| Aggregated | Toplu erişim | İstatistikler |

---

## 11. Session Recovery

### 11.1 Recovery Senaryoları

| Senaryo | Çözüm |
|---------|-------|
| Session interruption | log.md'den resume |
| Data corruption | git checkout + son commit |
| Memory overflow | Chunked read + compression |
| Concurrent conflict | Context Lock + Queue |

### 11.2 Recovery Prosedürü

| Adım | Aksiyon | Sorumlu |
|------|---------|---------|
| 1 | Durumu kontrol et | System |
| 2 | Kaynakları analiz et | System |
| 3 | Recovery planını oluştur | System |
| 4 | Geri yükleme işlemini başlat | System |
| 5 | Doğrulama yap | System |
| 6 | Log kaydı oluştur | MO |

---

## 12. Session Metrics

### 12.1 Metrikler

| Metrik | Tanım | Hedef |
|--------|-------|-------|
| Session duration | Oturum süresi | < 4 saat |
| Message count | Mesaj sayısı | < 100 |
| Token usage | Token kullanımı | < 100K |
| Tool calls | Tool çağrısı sayısı | < 50 |
| Files modified | Değiştirilen dosya | < 20 |

### 12.2 Metrik Toplama

| Metrik | Kaynak | Sıklık |
|--------|--------|--------|
| Session duration | Session state | Her session sonu |
| Message count | Message history | Her mesaj |
| Token usage | LLM calls | Her çağrı |
| Tool calls | Tool registry | Her çağrı |
| Files modified | Git diff | Her session sonu |

---

## 13. Session Security

### 13.1 Güvenlik Kuralları

| Kural | Açıklama |
|-------|----------|
| Encryption | Session verileri şifreli |
| Authentication | Kullanıcı doğrulaması |
| Authorization | Yetki kontrolü |
| Audit | Tüm işlemler loglanıyor |

### 13.2 Güvenlik Prosedürleri

| Prosedür | Açıklama |
|----------|----------|
| Session lock | Eşzamanlı erişimi önleme |
| Data isolation | Session veri izolasyonu |
| Secure deletion | Güvenli silme |
| Backup | Yedekleme |

---

## 14. Session Integration

### 14.1 Entegrasyon Noktaları

| Entegrasyon | Açıklama |
|-------------|----------|
| Git | Version control entegrasyonu |
| Vault | .ai/ vault entegrasyonu |
| Learning | Öğrenme sistemi entegrasyonu |
| Context | Context assembly entegrasyonu |

### 14.2 Entegrasyon Prosedürleri

| Prosedür | Adımlar |
|----------|---------|
| Git sync | commit → push → pull |
| Vault sync | load → update → save |
| Learning sync | pattern → correction → knowledge |
| Context sync | sources → assembly → validation |

---

## 15. Quality Report

| Metrik | Değer |
|--------|-------|
| Version | 1.0.0 |
| Status | Active |
| Session States | 5 |
| Memory Levels | 4 |
| Recovery Scenarios | 4 |
| Security Rules | 4 |
| Metrics | 5 |

---

**Authority:** Vault Steward
**Last Updated:** 2026-08-25
**Mode:** Red Team · Human Mode · Truth Mode