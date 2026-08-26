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

**Authority:** Vault Steward
**Last Updated:** 2026-08-25