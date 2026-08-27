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

## 16. Session Veri Modelleri

### 16.1 Session Entity

```csharp
public class Session
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public Guid ProjectId { get; private set; }
    public SessionStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public string? BranchName { get; private set; }
    public Guid? ParentSessionId { get; private set; }
    
    // Navigation
    public Project Project { get; private set; }
    public Session? ParentSession { get; private set; }
    public ICollection<Session> ChildSessions { get; private set; }
    public ICollection<Message> Messages { get; private set; }
}
```

### 16.2 Message Entity

```csharp
public class Message
{
    public Guid Id { get; private set; }
    public Guid SessionId { get; private set; }
    public MessageRole Role { get; private set; }
    public string Content { get; private set; }
    public DateTime Timestamp { get; private set; }
    public int TokenCount { get; private set; }
    public string? Metadata { get; private set; }
    
    // Navigation
    public Session Session { get; private set; }
}
```

### 16.3 Session DTO'ları

```csharp
// Read DTO
public record SessionDto(
    Guid Id,
    string Name,
    SessionStatus Status,
    DateTime CreatedAt,
    DateTime? CompletedAt,
    int MessageCount,
    int TotalTokens);

// Create Command
public record CreateSessionCommand(
    string Name,
    Guid ProjectId,
    string? BranchName = null);

// Update Command
public record UpdateSessionCommand(
    Guid Id,
    string? Name = null,
    SessionStatus? Status = null);
```

---

## 17. Session API Endpointleri

### 17.1 REST API

| Endpoint | Method | Amaç |
|----------|--------|------|
| `/api/sessions` | GET | Tüm session'ları listele |
| `/api/sessions/{id}` | GET | Belirli session'ı getir |
| `/api/sessions` | POST | Yeni session oluştur |
| `/api/sessions/{id}` | PUT | Session'ı güncelle |
| `/api/sessions/{id}` | DELETE | Session'ı sil |
| `/api/sessions/{id}/messages` | GET | Session mesajlarını getir |
| `/api/sessions/{id}/branch` | POST | Yeni dal oluştur |
| `/api/sessions/{id}/merge` | POST | Dal birleştir |
| `/api/sessions/{id}/revert` | POST | Önceki duruma dön |

### 17.2 CQRS Query/Command

```csharp
// Queries
public record GetSessionByIdQuery(Guid Id) : IRequest<SessionDto?>;
public record GetAllSessionsQuery() : IRequest<IReadOnlyList<SessionDto>>;
public record GetSessionMessagesQuery(Guid SessionId) : IRequest<IReadOnlyList<MessageDto>>;

// Commands
public record CreateSessionCommand(string Name, Guid ProjectId) : IRequest<Guid>;
public record UpdateSessionCommand(Guid Id, string? Name) : IRequest<Unit>;
public record DeleteSessionCommand(Guid Id) : IRequest<Unit>;
public record ArchiveSessionCommand(Guid Id) : IRequest<Unit>;
```

---

## 18. Session İş Akış Diyagramları

### 18.1 Session Oluşturma Akışı

```
Kullanıcı → "Yeni session oluştur"
    ↓
MO → Anahtar kelime analizi
    ↓
MO → Build Agent seç
    ↓
Build Agent → Vault oku
    ↓
Build Agent → Template yükle
    ↓
Build Agent → Entity oluştur
    ↓
Build Agent → Repository implement et
    ↓
Build Agent → Handler yaz
    ↓
Build Agent → Test yaz
    ↓
Build Agent → Build + Test çalıştır
    ↓
MO → Sonucu raporla
    ↓
Kullanıcı → Onay ver
```

### 18.2 Session Branching Akışı

```
Kullanıcı → "Bu session'dan dal oluştur"
    ↓
MO → Mevcut session'ı kaydet
    ↓
MO → Yeni dal ID'si oluştur
    ↓
MO → Geçmiş kopyasını oluştur
    ↓
MO → Yeni dal'ı aktif yap
    ↓
MO → Log kaydı oluştur
    ↓
Kullanıcı → Devam et
```

### 18.3 Session Merge Akışı

```
Kullanıcı → "Bu iki dal'ı birleştir"
    ↓
MO → Hedef dal'ı seç
    ↓
MO → Çakışmaları kontrol et
    ↓
MO → Çakışmaları çöz
    ↓
MO → Birleştirmeyi uygula
    ↓
MO → Log kaydı oluştur
    ↓
Kullanıcı → Onay ver
```

---

## 19. Session Hata Yönetimi

### 19.1 Hata Türleri

| Hata | Seviye | Aksiyon |
|------|--------|---------|
| Session bulunamadı | ERROR | 404 döndür |
| Session zaten aktif | WARNING | Uyarı göster |
| Branch çakışması | ERROR | Çakışma çöz |
| Merge başarısız | ERROR | Geri al |
| Revert başarısız | ERROR | Log + bildirim |
| Token limiti aşıldı | WARNING | Yeni session başlat |
| Memory overflow | ERROR | Session'ı arşivle |

### 19.2 Hata Kodları

| Kod | Açıklama |
|-----|----------|
| SES-001 | Session bulunamadı |
| SES-002 | Session zaten aktif |
| SES-003 | Branch çakışması |
| SES-004 | Merge başarısız |
| SES-005 | Revert başarısız |
| SES-006 | Token limiti aşıldı |
| SES-007 | Memory overflow |

---

## 20. Session Optimizasyonu

### 20.1 Performans Optimizasyonları

| Teknik | Açıklama | Kazanç |
|--------|----------|--------|
| Lazy loading | Mesajları gerektiğinde yükle | Bellek tasarrufu |
| Pagination | Büyük listeleri sayfala | Hız artışı |
| Caching | Sık erişilen verileri önbellekle | yanıt süresi |
| Indexing | Sorgu alanlarını indeksle | Sorgu hızı |
| Compression | Büyük mesajları sıkıştır | Depolama tasarrufu |

### 20.2 Depolama Optimizasyonları

| Teknik | Açıklama | Kazanç |
|--------|----------|--------|
| Archiving | Eski session'ları arşivle | Depolama tasarrufu |
| Cleanup | Gereksiz verileri temizle | Depolama tasarrufu |
| Deduplication | Tekrar eden verileri kaldır | Depolama tasarrufu |
| Partitioning | Verileri bölümle | Sorgu hızı |

---

## 21. Quality Report

| Metrik | Değer |
|--------|-------|
| Version | 1.1.0 |
| Status | Active |
| Session States | 5 |
| Memory Levels | 4 |
| Recovery Scenarios | 4 |
| Security Rules | 4 |
| Metrics | 5 |
| API Endpoints | 9 |
| Error Codes | 7 |

---

**Authority:** Vault Steward
**Last Updated:** 2026-08-26
**Mode:** Red Team · Human Mode · Truth Mode