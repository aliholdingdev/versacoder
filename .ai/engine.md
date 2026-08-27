---
title: "Versa Coder — Orkestrasyon Motoru"
type: engine
category: orchestration
date: 2026-08-25
updated: 2026-08-25
status: active
version: 1.0.0
---

# Versa Coder — Orkestrasyon Motoru

**Zorunlu Bağlantılar:** [[CLAUDE.md]] · [[AGENTS.md]] · [[WORKFLOW.md]]

---

## 1. Amaç

Versa Coder'daki tüm agent'ların koordinasyonunu, görev dağıtımını ve akışını yöneten **orquestrasyon motorunun** teknik tanımıdır.

---

## 2. Motor Mimarisi

```
┌─────────────────────────────────────────────────────┐
│                   MASTER ORCHESTRATOR                │
│  ┌───────────┐  ┌───────────┐  ┌───────────┐       │
│  │  Analiz   │  │  Seçim    │  │  Dağıtım  │       │
│  │  Motoru   │→ │  Motoru   │→ │  Motoru   │       │
│  └───────────┘  └───────────┘  └───────────┘       │
│                      ↓                              │
│  ┌─────────────────────────────────────────────┐   │
│  │              AGENT POOL                      │   │
│  │  ┌─────┐ ┌─────┐ ┌─────┐ ┌─────┐ ┌─────┐  │   │
│  │  │Build│ │Plan │ │Expl.│ │Gen. │ │Summ.│  │   │
│  │  └─────┘ └─────┘ └─────┘ └─────┘ └─────┘  │   │
│  └─────────────────────────────────────────────┘   │
│                      ↓                              │
│  ┌─────────────────────────────────────────────┐   │
│  │              TOOL REGISTRY                    │   │
│  │  Read | Write | Edit | Glob | Grep | Bash   │   │
│  │  Git | Test | MCP | Session | Context        │   │
│  └─────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────┘
```

---

## 3. Akış Diyagramı

```
Kullanıcı Girdisi
    ↓
[1. Pre-flight] → Vault oku, context hazırla
    ↓
[2. Analiz] → Keyword çıkarma, domain eşleme
    ↓
[3. Seçim] → Doğru agent'ı belirle (§7.2 Seçim Algoritması)
    ↓
[4. Context Assembly] → Vault + Learning + Session bilgisini birleştir
    ↓
[5. Görev Ata] → Seçilen agent'a görevi ilet
    ↓
[6. Agent Çalıştır] → Agent tool'ları kullanarak görevi yürütür
    ↓
[7. Tool Çağrıları] → LLM → Tool Registry → Execute → Result → LLM
    ↓
[8. Doğrulama] → Çıktıyı kontrol et
    ↓
[9. Handover] → Gerekirse diğer ajana transfer et
    ↓
[10. Tamamla] → Sonucu kaydet, logla
```

---

## 4. Görev Durum Makinesi

```
CREATED → QUEUED → ASSIGNED → RUNNING → COMPLETED
                                    ↓
                                  FAILED → RETRY → RUNNING
                                    ↓
                                  ESCALATED → HUMAN_REVIEW
```

| Durum | Tanım | Geçiş |
|-------|-------|-------|
| CREATED | Görev oluşturuldu | → QUEUED |
| QUEUED | Kuyrukta bekliyor | → ASSIGNED |
| ASSIGNED | Agent'a atandı | → RUNNING |
| RUNNING | Çalışıyor | → COMPLETED / FAILED |
| COMPLETED | Başarıyla tamamlandı | — |
| FAILED | Başarısız oldu | → RETRY / ESCALATED |
| RETRY | Yeniden deneniyor | → RUNNING |
| ESCALATED | İnsan müdahalesine gerek var | → HUMAN_REVIEW |
| HUMAN_REVIEW | İnsan incelemesinde | → RUNNING / COMPLETED |

---

## 5. Context Assembly Motoru

```
┌──────────────────────────────────────────┐
│           CONTEXT ASSEMBLY               │
│                                          │
│  1. Vault Oku                           │
│     ├── CLAUDE.md (guardrails)          │
│     ├── AGENTS.md (agent sınırları)     │
│     ├── WORKFLOW.md (süreçler)          │
│     └── brain.md (mimari kararlar)      │
│                                          │
│  2. Learning Yükle                       │
│     ├── Patterns (kalıplar)             │
│     ├── Corrections (düzeltmeler)       │
│     └── Knowledge (bilgi)               │
│                                          │
│  3. Session Yükle                        │
│     ├── Önceki mesajlar                 │
│     ├── Mevcut durum                    │
│     └── Branch geçmişi                  │
│                                          │
│  4. Proje Analiz                         │
│     ├── Dosya yapısı                    │
│     ├── Bağımlılıklar                   │
│     └── Mevcut kod kalıpları            │
│                                          │
│  5. Birleştir                            │
│     └── Tek context object oluştur      │
└──────────────────────────────────────────┘
```

---

## 6. Event Bus

| Event | Publisher | Subscriber | Amaç |
|-------|-----------|------------|------|
| `task.created` | MO | Agent Pool | Yeni görev |
| `task.assigned` | MO | Seçilen Agent | Görev atandı |
| `task.completed` | Agent | MO | Görev tamamlandı |
| `task.failed` | Agent | MO | Görev başarısız |
| `handover.requested` | Kaynak Agent | Hedef Agent | Transfer isteği |
| `escalation.triggered` | Agent | MO + İnsan | Eskalasyon |
| `session.started` | System | Tümü | Oturum başladı |
| `session.ended` | System | Tümü | Oturum bitti |

---

## 7. Koordinasyon Protokolleri

### 7.1 Paralel Görev Çalıştırma

```csharp
// Bağımsız görevler paralel çalıştırılabilir
var tasks = new[]
{
    AgentRunner.RunAsync(agentBuild, task1),
    AgentRunner.RunAsync(agentExplore, task2),
    AgentRunner.RunAsync(agentSummary, task3)
};
await Task.WhenAll(tasks);
```

### 7.2 Sıralı Görev Zinciri

```csharp
// Bağımlı görevler sıralı çalıştırılır
var result1 = await AgentRunner.RunAsync(agentPlan, planningTask);
var result2 = await AgentRunner.RunAsync(agentBuild, buildTask, result1);
var result3 = await AgentRunner.RunAsync(agentSummary, docTask, result2);
```

---

## 8. Hata Yönetimi

| Hata Tipi | Öncelik | Aksiyon |
|-----------|---------|---------|
| Provider hatası | HIGH | Fallback provider'a geç |
| Tool hatası | MEDIUM | Retry (max 3) |
| Timeout | MEDIUM | Agent'ı durdur, MO'ya bildir |
| Context overflow | LOW | Compaction agent'ı çağır |
| Vault hatası | CRITICAL | İşlemi durdur, insana bildir |

---

## 9. Monitoring & Metrics

| Metric | Açıklama | Eşik |
|--------|----------|------|
| Görev tamamlama oranı | Başarılı / Toplam | > %95 |
| Ortalama yanıt süresi | Görev başına | < 30s |
| Hata oranı | Başarısız / Toplam | < %5 |
| Agent kullanım dağılımı | Her agent için | Dengeli |
| Token kullanımı | Oturum başına | < 100K |

---

## 10. Quality Report

| Metrik | Değer |
|--------|-------|
| Version | 1.0.0 |
| Status | Active |
| Agent Pool | 7 |
| Tool Count | 45+ |
| Event Types | 8 |
| Error Types | 5 |
| Metrics | 5 |

---

## 11. Motor Detayları

### 11.1 Analiz Motoru

Analiz motoru, kullanıcı girişini işler ve uygun agent'ı belirler:

| Adım | Aksiyon | Çıktı |
|------|---------|-------|
| 1 | Girdiyi normalize et | Küçük harf, boşluk temizleme |
| 2 | Keyword'leri çıkar | Anahtar kelime listesi |
| 3 | Domain eşleme | Domain bazlı eşleme |
| 4 | Agent seçimi | Uygun agent listesi |
| 5 | Öncelik belirleme | Öncelik seviyesi |

#### Analiz Motoru Kodu

```csharp
public class AnalysisEngine
{
    private readonly Dictionary<string, AgentRole> _keywordMap;
    
    public AnalysisResult Analyze(string userInput)
    {
        var normalized = userInput.ToLowerInvariant();
        var keywords = ExtractKeywords(normalized);
        var domain = MatchDomain(keywords);
        var agent = SelectAgent(keywords, domain);
        var priority = DeterminePriority(keywords);
        
        return new AnalysisResult
        {
            Keywords = keywords,
            Domain = domain,
            Agent = agent,
            Priority = priority,
            Confidence = CalculateConfidence(keywords)
        };
    }
    
    private List<string> ExtractKeywords(string input)
    {
        // Keyword çıkarma mantığı
        return input.Split(' ')
            .Where(w => _keywordMap.ContainsKey(w))
            .ToList();
    }
}
```

### 11.2 Seçim Motoru

Seçim motoru, analiz sonuçlarına göre en uygun agent'ı seçer:

| Kriter | Ağırlık | Hesaplama |
|--------|---------|-----------|
| Keyword eşleşmesi | %40 | Doğrudan eşleşme |
| Domain uyumu | %30 | Domain bazlı uyum |
| Agent kullanılabilirliği | %20 | Durum kontrolü |
| Yük dengesi | %10 | Mevcut görev yükü |

#### Seçim Motoru Kodu

```csharp
public class SelectionEngine
{
    public AgentSelection SelectAgent(AnalysisResult analysis)
    {
        var candidates = GetCandidateAgents(analysis.Domain);
        
        var scored = candidates.Select(agent => new
        {
            Agent = agent,
            Score = CalculateScore(agent, analysis)
        })
        .OrderByDescending(x => x.Score)
        .ToList();
        
        return new AgentSelection
        {
            Primary = scored.First().Agent,
            Secondary = scored.ElementAtOrDefault(1)?.Agent,
            Confidence = scored.First().Score
        };
    }
    
    private double CalculateScore(AgentRole agent, AnalysisResult analysis)
    {
        var keywordScore = CalculateKeywordScore(agent, analysis.Keywords);
        var domainScore = CalculateDomainScore(agent, analysis.Domain);
        var availabilityScore = CalculateAvailabilityScore(agent);
        var loadScore = CalculateLoadScore(agent);
        
        return (keywordScore * 0.4) +
               (domainScore * 0.3) +
               (availabilityScore * 0.2) +
               (loadScore * 0.1);
    }
}
```

### 11.3 Dağıtım Motoru

Dağıtım motoru, seçilen agent'a görevi dağıtır ve izler:

| Adım | Aksiyon | Timeout |
|------|---------|---------|
| 1 | Görev oluştur | Anlık |
| 2 | Agent'a ata | Max 500ms |
| 3 | Bağımlılıkları kontrol et | Max 1s |
| 4 | Context hazırla | Max 5s |
| 5 | Görevi başlat | Max 10s |
| 6 | İlerlemeyi izle | Sürekli |
| 7 | Tamamlanmayı bekle | Max 300s |

#### Dağıtım Motoru Kodu

```csharp
public class DispatchEngine
{
    private readonly IAgentRunner _agentRunner;
    private readonly ITaskQueue _taskQueue;
    
    public async Task<TaskResult> DispatchAsync(TaskRequest request)
    {
        var task = new Task
        {
            Id = Guid.NewGuid(),
            Agent = request.Agent,
            Priority = request.Priority,
            Status = TaskStatus.Created,
            CreatedAt = DateTime.UtcNow
        };
        
        await _taskQueue.EnqueueAsync(task);
        
        var result = await _agentRunner.RunAsync(
            task.Agent,
            request.Context,
            request.CancellationToken);
        
        return result;
    }
}
```

---

## 12. Motor Performansı

### 12.1 Performans Metrikleri

| Metrik | Hedef | Ölçüm |
|--------|-------|-------|
| Analiz süresi | < 100ms | Ortalama |
| Seçim süresi | < 50ms | Ortalama |
| Dağıtım süresi | < 500ms | Ortalama |
| Toplam gecikme | < 1s | End-to-end |
| Throughput | > 10 görev/saniye | Saniye başına |

### 12.2 Optimizasyon Teknikleri

| Teknik | Açıklama | Kazanç |
|--------|----------|--------|
| Caching | Sık kullanılan analizleri önbellekleme | %30 hız artışı |
| Parallel processing | Bağımsız görevleri paralel çalıştırma | %50 hız artışı |
| Lazy loading | Context'i gerektiğinde yükleme | Bellek tasarrufu |
| Prefetching | Öngörülen verileri önceden çekme | Gecikme azaltma |

### 12.3 Kaynak Kullanımı

| Kaynak | Hedef | Maksimum |
|--------|-------|----------|
| CPU | < %20 | %50 |
| Bellek | < 100MB | 250MB |
| Ağ | < 1MB/s | 5MB/s |
| Disk I/O | < 10MB/s | 50MB/s |

---

## 13. Motor Güvenliği

### 13.1 Güvenlik Kuralları

| Kural | Açıklama |
|-------|----------|
| Input validation | Tüm girdiler doğrulanır |
| Sandboxing | Agent'lar izole çalışır |
| Rate limiting | Aşırı kullanım engellenir |
| Audit logging | Tüm işlemler loglanır |
| Error handling | Hatalar güvenli şekilde işlenir |

### 13.2 Güvenlik Katmanları

```
┌─────────────────────────────────────┐
│         INPUT VALIDATION            │
│  - Girdi temizleme                 │
│  - Format kontrolü                 │
│  - Sınır kontrolü                  │
├─────────────────────────────────────┤
│         AUTHORIZATION               │
│  - Agent yetkileri                 │
│  - Dosya erişimi                   │
│  - Tool kullanımı                  │
├─────────────────────────────────────┤
│         AUDIT LOGGING               │
│  - Tüm işlemler loglanır          │
│  - izlenebilirlik                  │
│  - Compliance                      │
├─────────────────────────────────────┤
│         ERROR HANDLING              │
│  - Güvenli hata mesajları          │
│  - Hassas veri sızıntısı yok      │
│  - Graceful degradation            │
└─────────────────────────────────────┘
```

---

## 14. Motor İzleme

### 14.1 İzleme Noktaları

| Nokta | Metrik | Alarm |
|-------|--------|-------|
| Analiz motoru | Gecikme, hata oranı | > 100ms, > %5 |
| Seçim motoru | Gecikme, doğruluk | > 50ms, < %90 |
| Dağıtım motoru | Gecikme, başarı oranı | > 500ms, < %95 |
| Agent havuzu | Kullanım oranı, hata | > %80, > %10 |
| Tool registry | Çağrı sayısı, hata | > 1000/dk, > %5 |

### 14.2 Dashboard Bileşenleri

| Bileşen | İçerik | Güncelleme |
|---------|--------|-----------|
| Motor Durumu | Genel sağlık | Gerçek zamanlı |
| Agent Kullanımı | Agent bazlı kullanım | 5s |
| Görev İlerlemesi | Aktif görevler | Gerçek zamanlı |
| Hata Günlüğü | Son hatalar | Gerçek zamanlı |
| Performans Grafiği | Metrik trendleri | 1min |

### 14.3 Uyarı Sistemi

| Uyarı | Seviye | Aksiyon |
|-------|--------|---------|
| Yüksek gecikme | WARNING | İzleme artır |
| Hata oranı yüksek | ERROR | Düzeltme başlat |
| Kaynak kullanımı | WARNING | Optimizasyon |
| Sistem çökmesi | CRITICAL | Acil durum |

---

## 15. Motor Testleri

### 15.1 Test Kategorileri

| Kategori | Amaç | Kapsam |
|----------|------|--------|
| Unit Test | Bileşen testleri | Motor sınıfları |
| Integration Test | Bileşenler arası | Motor + Agent |
| Performance Test | Performans doğrulama | Yük testi |
| Security Test | Güvenlik doğrulama | Saldırı testi |
| E2E Test | Uçtan uca | Tam akış |

### 15.2 Test Senaryoları

| Senaryo | Beklenen Sonuç |
|---------|----------------|
| Basit görev | Başarıyla tamamlanma |
| Karmaşık görev | Doğru agent seçimi |
| Hatalı girdi | Güvenli hata yönetimi |
| Yüksek yük | Performans koruma |
| Saldırı | Güvenlik engelleme |

---

## 16. Quality Report

| Metrik | Değer |
|--------|-------|
| Version | 1.1.0 |
| Status | Active |
| Agent Pool | 7 |
| Tool Count | 45+ |
| Event Types | 8 |
| Error Types | 5 |
| Metrics | 10+ |
| Security Layers | 4 |
| Test Categories | 5 |

---

**Authority:** Vault Steward
**Last Updated:** 2026-08-26
**Mode:** Red Team · Human Mode · Truth Mode