---
title: "Versa Coder — Agent Kayıt Defteri & Koordinasyon Protokolü"
type: guide
category: agent-registry
date: 2026-08-25
updated: 2026-08-25
status: active
version: 1.0.0
authority: Single Source of Truth (SSOT)
governance: Red Team · Human Mode · Truth Mode
reference:
  authority: ".ai/AGENTS.md"
  source_of_truth: ".ai/CLAUDE.md · .ai/AGENTS.md · .ai/WORKFLOW.md · .ai/brain.md · .ai/index.md"
---

# Versa Coder — Agent Kayıt Defteri & Koordinasyon Protokolü

**Zorunlu Bağlantılar:** [[CLAUDE.md]] · [[WORKFLOW.md]] · [[index.md]] · [[keys.md]] · [[brain.md]] · [[MEMORY.md]] · [[log.md]] · [[.templates/index]] · [[.agents/AGENTS.md]]

**Skills:** `.ai/skills/` (6 skill — Guardrail #16 zorunlu)

---

## 1. Amaç

Versa Coder ekosistemindeki 7 yapay zeka ajanının (Master Orchestrator + 6 uzman) yetki sınırlarını, rollerini, iletişim protokollerini ve kalite standartlarını tanımlayan **Tek Doğruluk Kaynağıdır (SSOT)**.

---

## 2. Kapsam

| Kapsam | Kapsam Dışı |
|--------|-------------|
| Tüm agent'ların domain yetkileri ve kısıtlamaları | Teknik uygulama detayları |
| Görev dağıtımı algoritması (Task Dispatch) | İş mantığı |
| Ajanlar arası handover ve eskalasyon protokolü | Veritabanı işlemleri |
| Sağlık kontrolü ve context lock mekanizması | Güvenlik politikası |

---

## 3. Terminoloji

| Terim | Tanım |
|-------|-------|
| **Agent** | Versa Coder ekosisteminde belirli bir alanda uzmanlaşmış yapay zeka birimi |
| **Master Orchestrator (MO)** | Tüm ajanları koordine eden ana kontrol birimi |
| **Domain Boundary** | Her ajanın yalnızca kendi alanında çalışması kuralı |
| **Handover** | Bir ajanın görevi başka bir ajana transfer etmesi |
| **Eskalasyon** | Bir sorunun çözülemediği durumda daha üst seviyeye çıkması |
| **Context Lock** | Eşzamanlı dosya erişimini önlemek için kilitleme mekanizması |
| **Health Check** | Ajanların çalışma durumunu kontrol eden mekanizma |
| **Task Queue** | Görevlerin öncelik sırasıyla beklediği kuyruk |

---

## 4. Agent Genel Bakış

| # | Agent | Kod Adı | Domain | Katman | Teknoloji |
|---|-------|---------|--------|--------|-----------|
| 1 | **Master Orchestrator** | `mo` | Görev dağıtımı, koordinasyon | Koordinasyon | Vault System, log.md |
| 2 | **Build Agent** | `build` | Kod yazma, dosya oluşturma, düzenleme | L2-L4 | C# .NET 8, EF Core |
| 3 | **Plan Agent** | `plan` | Mimari planlama, task dağıtımı | L2 | MediatR, CQRS |
| 4 | **Explore Agent** | `explore` | Kod analizi, dosya tarama | L1-L4 | Roslyn, AST |
| 5 | **General Agent** | `general` | Genel amaçlı görevler | Tümü | Multi-domain |
| 6 | **Summary Agent** | `summary` | Özetleme, dokümantasyon | L22 | Markdig |
| 7 | **Title Agent** | `title` | Başlık oluşturma, isimlendirme | L2 | NLP pattern |

### 4.1 Agent Detayları

| Agent | Uzmanlık | Deneyim | Max Paralel Görev | Kullanılabilir Araçlar |
|-------|----------|---------|-------------------|------------------------|
| Master Orchestrator | Koordinasyon, görev dağıtımı | Expert | 10 | Tüm araçlar |
| Build Agent | Kod üretimi, dosya işlemleri | Expert | 3 | Read, Write, Edit, Bash |
| Plan Agent | Mimari planlama, task dağıtımı | Expert | 1 | Read, Write, Glob, Grep |
| Explore Agent | Kod analizi, tarama | Expert | 5 | Read, Glob, Grep, Bash |
| General Agent | Genel amaçlı görevler | Expert | 2 | Tüm araçlar |
| Summary Agent | Özetleme, dokümantasyon | Expert | 3 | Read, Write, Markdown |
| Title Agent | Başlık oluşturma, isimlendirme | Expert | 10 | Read, Write |

---

## 5. Domain Sınırları

| Dosya Tipi | Sorumlu Agent | Diğerleri Erişebilir mi? |
|------------|---------------|--------------------------|
| `*.cs` (Domain, Application, Infrastructure) | Build Agent | ❌ |
| `*.csproj` / `*.sln` | Plan Agent | ❌ |
| `*.md` (documentation) | Summary Agent | ❌ |
| `*.sql` (migration, schema) | Build Agent | ❌ |
| `*.xaml` / `*.resx` (UI) | Build Agent | ❌ |
| `*.json` (config) | Plan Agent | ❌ |
| `test/**/*.cs` | Build Agent | ❌ |
| `.ai/` vault | MO (koordinasyon) | ✅ Okuma serbest |
| `log.md` (audit trail) | Tüm ajanlar (append-only) | ✅ Sadece ekleme |

**Layer Violation:** L0 → L2/L3 veya L1 → L3 gibi kural ihlalleri tespit edilirse derhal revert + log ERROR.

### 5.1 Dosya Erişim Matrisi

| Agent | Domain | Application | Infrastructure | UI | Test | Config | Documentation |
|-------|--------|-------------|----------------|----|----|--------|---------------|
| Build | ✅ | ✅ | ✅ | ✅ | ✅ | ❌ | ❌ |
| Plan | ❌ | ❌ | ❌ | ❌ | ❌ | ✅ | ❌ |
| Explore | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| General | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| Summary | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ | ✅ |
| Title | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ | ✅ |

---

## 6. Keyword → Agent Yönlendirmesi

| Keyword Grubu | Birincil Agent | İkincil Agent |
|---------------|----------------|---------------|
| kod, class, method, property, service, repository | Build Agent | Plan Agent |
| plan, mimari, task, phase, milestone | Plan Agent | MO |
| analiz, tarama, grep, glob, dosya bul | Explore Agent | Build Agent |
| doc, özet, dokümantasyon, markdown | Summary Agent | — |
| başlık, isim, naming, convention | Title Agent | — |
| vault, brain, decision, ADR | MO (vault-updater) | — |
| template, şablon | Tüm ajanlar (guardrail #16) | MO |

### 6.1 Keyword Eşleme Detayı

| Keyword | Birincil Agent | İkincil Agent | Öncelik |
|---------|----------------|---------------|---------|
| kod | Build Agent | Plan Agent | Yüksek |
| class | Build Agent | Plan Agent | Yüksek |
| method | Build Agent | Plan Agent | Yüksek |
| property | Build Agent | Plan Agent | Yüksek |
| service | Build Agent | Plan Agent | Yüksek |
| repository | Build Agent | Plan Agent | Yüksek |
| plan | Plan Agent | MO | Yüksek |
| mimari | Plan Agent | MO | Yüksek |
| task | Plan Agent | MO | Yüksek |
| phase | Plan Agent | MO | Yüksek |
| milestone | Plan Agent | MO | Yüksek |
| analiz | Explore Agent | Build Agent | Orta |
| tarama | Explore Agent | Build Agent | Orta |
| grep | Explore Agent | Build Agent | Orta |
| glob | Explore Agent | Build Agent | Orta |
| dosya bul | Explore Agent | Build Agent | Orta |
| doc | Summary Agent | — | Orta |
| özet | Summary Agent | — | Orta |
| dokümantasyon | Summary Agent | — | Orta |
| markdown | Summary Agent | — | Orta |
| başlık | Title Agent | — | Düşük |
| isim | Title Agent | — | Düşük |
| naming | Title Agent | — | Düşük |
| convention | Title Agent | — | Düşük |
| vault | MO | — | Yüksek |
| brain | MO | — | Yüksek |
| decision | MO | — | Yüksek |
| ADR | MO | — | Yüksek |
| template | Tüm ajanlar | MO | Yüksek |

---

## 7. Görev Dağıtımı Algoritması

```
Kullanıcı İsteği
  → [1. Analiz] — Keyword çıkarma, domain eşleme
    → [2. Pre-flight Checks] — Bağımlılık, dosya kontrolü
      → [3. Task Assignment] — Doğru ajanı seç ve görev ata
        → [4. Execution] — Ajan görevi yürütür
          → [5. Handover] — Gerekirse diğer ajana transfer
            → [6. Validation] — Çıktıyı doğrula
              → [7. Completion] — Görevi tamamla ve logla
```

### 7.1 Seçim Algoritması (C# Pseudocode)

```csharp
public AgentRole SelectAgent(string userPrompt)
{
    var prompt = userPrompt.ToLowerInvariant();

    // Priority 1: Build Agent
    if (ContainsAny(prompt, BuildKeywords))
        return AgentRole.Build;

    // Priority 2: Plan Agent
    if (ContainsAny(prompt, PlanKeywords))
        return AgentRole.Plan;

    // Priority 3: Explore Agent
    if (ContainsAny(prompt, ExploreKeywords))
        return AgentRole.Explore;

    // Priority 4: Summary Agent
    if (ContainsAny(prompt, SummaryKeywords))
        return AgentRole.Summary;

    // Priority 5: Title Agent
    if (ContainsAny(prompt, TitleKeywords))
        return AgentRole.Title;

    // Default: General Agent
    return AgentRole.General;
}
```

### 7.2 Görev Dağıtım Akışı

| Adım | Aksiyon | Sorumlu |
|------|---------|---------|
| 1 | Kullanıcı isteğini analiz et | MO |
| 2 | Keyword'leri çıkar | MO |
| 3 | Domain eşleme yap | MO |
| 4 | Pre-flight checks yap | MO |
| 5 | Doğru agent'ı seç | MO |
| 6 | Görevi ata | MO |
| 7 | Agent'ı çalıştır | Seçilen Agent |
| 8 | Çıktıyı doğrula | MO |
| 9 | Gerekirse handover yap | MO |
| 10 | Görevi tamamla ve logla | MO |

---

## 8. Öncelik Seviyeleri

| Öncelik | Tanım | Timeout | Max Retry |
|---------|-------|---------|-----------|
| CRITICAL | Sistem durması, güvenlik açığı | 5s | 1 |
| HIGH | Kritik işlev kaybı | 15s | 3 |
| MEDIUM | Normal geliştirme görevi | 30s | 3 |
| LOW | İyileştirme, optimizasyon | 60s | 2 |

### 8.1 Öncelik Seçim Kriterleri

| Kriter | CRITICAL | HIGH | MEDIUM | LOW |
|--------|----------|------|--------|-----|
| Sistem durması | ✅ | ❌ | ❌ | ❌ |
| Güvenlik açığı | ✅ | ❌ | ❌ | ❌ |
| Kritik işlev kaybı | ❌ | ✅ | ❌ | ❌ |
| Veri kaybı | ❌ | ✅ | ❌ | ❌ |
| Normal geliştirme | ❌ | ❌ | ✅ | ❌ |
| İyileştirme | ❌ | ❌ | ❌ | ✅ |
| Optimizasyon | ❌ | ❌ | ❌ | ✅ |

---

## 9. Handover Protokolü

```
[Kaynak Agent] → [Handover Request] → [Hedef Agent] → [Onay/Red] → [Confirmation]
```

### 9.1 Handover Mesaj Formatı

```json
{
  "subject": "Görevin kısa açıklaması",
  "sourceAgent": "build",
  "targetAgent": "plan",
  "priority": "MEDIUM",
  "affectedFiles": ["src/VersaCoder.Domain/Entities/Session.cs"],
  "request": "Mimari planlama gerekiyor",
  "status": "PENDING",
  "timestamp": "2026-08-25T12:00:00Z"
}
```

### 9.2 Handover Senaryoları

| # | Senaryo | Kaynak → Hedef |
|---|---------|----------------|
| 1 | Kod → Plan | Build → Plan (mimari onay) |
| 2 | Plan → Kod | Plan → Build (onay sonrası) |
| 3 | Kod → Explore | Build → Explore (analiz) |
| 4 | Explore → Kod | Explore → Build (bulgu sonrası) |
| 5 | Kod → Summary | Build → Summary (doc üretimi) |
| 6 | Any → MO | Herhangi biri → MO (eskalasyon) |
| 7 | MO → Any | MO → Herhangi biri (görev dağıtımı) |
| 8 | Any → Title | Herhangi biri → Title (isimlendirme) |

### 9.3 Handover Prosedürü

| Adım | Aksiyon | Sorumlu |
|------|---------|---------|
| 1 | Handover request oluştur | Kaynak Agent |
| 2 | Hedef agent'ı bilgilendir | MO |
| 3 | Hedef agent onay ver veya red et | Hedef Agent |
| 4 | Onay ise context transfer et | Kaynak Agent |
| 5 | Red ise alternatif çöz | Kaynak Agent |
| 6 | Transfer tamamla | Hedef Agent |
| 7 | Log kaydı oluştur | MO |

---

## 10. Eskalasyon Protokolü

```
Level 1 (Domain Lead) → Level 2 (Tech Lead) → Level 3 (Arch Lead) → İnsan
```

| Level | Sorumlu | Yetki |
|-------|---------|-------|
| Level 1 | Domain Lead (Agent) | Kendi alanında çözüm |
| Level 2 | Tech Lead (MO) | Çapraz alan çözümü |
| Level 3 | Arch Lead (İnsan) | Mimari karar |
| Level 4 | İnsan (Proje Sahibi) | Nihai karar |

### 10.1 Eskalasyon Kriterleri

| Level | Kriter | Aksiyon |
|-------|--------|---------|
| Level 1 | Agent kendi alanında çözüm bulamadı | MO'ya bildir |
| Level 2 | MO çapraz alan çözümü bulamadı | İnsan'a bildir |
| Level 3 | İnsan mimari karar verdi | Uygula |
| Level 4 | Proje sahibi nihai karar verdi | Uygula |

---

## 11. Sağlık Kontrolü

| Durum | Kod | Açıklama |
|-------|-----|----------|
| Healthy | 200 | Görev tamamlandı |
| Degraded | 301 | Yavaş yanıt (>15s) |
| Retry | 408 | Timeout, yeniden deneniyor |
| Failed | 500 | 3 retry başarısız |
| Dead | 503 | Yanıt yok, escalation |

### 11.1 Sağlık Kontrolü Prosedürü

| Adım | Aksiyon | Sıklık |
|------|---------|--------|
| 1 | Agent health check | Her görev öncesi |
| 2 | Tool health check | Her tool çağrısı öncesi |
| 3 | Provider health check | Her LLM çağrısı öncesi |
| 4 | System health check | Her session başında |

---

## 12. Context Lock

| Kurallar | Değer |
|---------|-------|
| Kilitleme süresi | Max 30 saniye |
| Deadlock prevention | MO en eski kilidi kırar |
| Öncelik | CRITICAL > HIGH > MEDIUM > LOW |
| Logging | Lock acquire/release `log.md`'ye yazılır |

### 12.1 Context Lock Prosedürü

| Adım | Aksiyon | Sorumlu |
|------|---------|---------|
| 1 | Lock isteği oluştur | Agent |
| 2 | Lock durumunu kontrol et | MO |
| 3 | Lock ver veya bekle | MO |
| 4 | İşlemi yap | Agent |
| 5 | Lock'ı serbest bırak | Agent |
| 6 | Log kaydı oluştur | MO |

---

## 13. Agent Detayları

| # | Agent | Katman | Teknoloji | Profil |
|---|-------|--------|-----------|--------|
| 1 | Master Orchestrator | Koordinasyon | Vault System, log.md | [[.agents/master-orchestrator]] |
| 2 | Build Agent | L2-L4 | C# .NET 8, EF Core | [[.agents/build-agent]] |
| 3 | Plan Agent | L2 | MediatR, CQRS | [[.agents/plan-agent]] |
| 4 | Explore Agent | L1-L4 | Roslyn, AST | [[.agents/explore-agent]] |
| 5 | General Agent | Tümü | Multi-domain | [[.agents/general-agent]] |
| 6 | Summary Agent | L22 | Markdig | [[.agents/summary-agent]] |
| 7 | Title Agent | L2 | NLP pattern | [[.agents/title-agent]] |

### 13.1 Master Orchestrator Detayları

| Özellik | Değer |
|---------|-------|
| Görev | Tüm ajanları koordine etme |
| Yetki | Tüm araçlara erişim |
| Sorumluluk | Görev dağıtımı, handover, eskalasyon |
| Teknoloji | Vault System, log.md |
| Max Paralel Görev | 10 |
| Sağlık Kontrolü | Her görev öncesi |

### 13.2 Build Agent Detayları

| Özellik | Değer |
|---------|-------|
| Görev | Kod yazma, dosya oluşturma, düzenleme |
| Yetki | Read, Write, Edit, Bash |
| Sorumluluk | L2-L4 katmanlarında kod üretimi |
| Teknoloji | C# .NET 8, EF Core |
| Max Paralel Görev | 3 |
| Sağlık Kontrolü | Her görev öncesi |

### 13.3 Plan Agent Detayları

| Özellik | Değer |
|---------|-------|
| Görev | Mimari planlama, task dağıtımı |
| Yetki | Read, Write, Glob, Grep |
| Sorumluluk | L2 katmanında planlama |
| Teknoloji | MediatR, CQRS |
| Max Paralel Görev | 1 |
| Sağlık Kontrolü | Her görev öncesi |

### 13.4 Explore Agent Detayları

| Özellik | Değer |
|---------|-------|
| Görev | Kod analizi, dosya tarama |
| Yetki | Read, Glob, Grep, Bash |
| Sorumluluk | L1-L4 katmanlarında analiz |
| Teknoloji | Roslyn, AST |
| Max Paralel Görev | 5 |
| Sağlık Kontrolü | Her görev öncesi |

### 13.5 General Agent Detayları

| Özellik | Değer |
|---------|-------|
| Görev | Genel amaçlı görevler |
| Yetki | Tüm araçlar |
| Sorumluluk | Tüm katmanlarda görev |
| Teknoloji | Multi-domain |
| Max Paralel Görev | 2 |
| Sağlık Kontrolü | Her görev öncesi |

### 13.6 Summary Agent Detayları

| Özellik | Değer |
|---------|-------|
| Görev | Özetleme, dokümantasyon |
| Yetki | Read, Write, Markdown |
| Sorumluluk | L22 katmanında dokümantasyon |
| Teknoloji | Markdig |
| Max Paralel Görev | 3 |
| Sağlık Kontrolü | Her görev öncesi |

### 13.7 Title Agent Detayları

| Özellik | Değer |
|---------|-------|
| Görev | Başlık oluşturma, isimlendirme |
| Yetki | Read, Write |
| Sorumluluk | L2 katmanında isimlendirme |
| Teknoloji | NLP pattern |
| Max Paralel Görev | 10 |
| Sağlık Kontrolü | Her görev öncesi |

---

## 14. Ultra Düşünme Protokolü

**⚠️ ZORUNLULUK:** Tüm agent'lar kod yazmadan önce bu protokolü uygulamak ZORUNDADIR.

### 14.1 5 Adımlı Düşünme Protokolü

| Adım | Kontrol | Kaynak | Timeout |
|------|---------|--------|---------|
| 1. Vault Oku | CLAUDE.md → AGENTS.md → WORKFLOW.md → brain.md → ROLE.md | `.ai/` vault | Max 25s |
| 2. Bağlamı Anla | Domain, katman, dosyalar, bağımlılıklar | Mevcut kod | Değişken |
| 3. Hata Kontrolü | Syntax, imports, types, style, security | LSP + Manuel | Anlık |
| 4. Sonuç Tahmini | Etki alanı, edge cases, performance | Düşünce | Değişken |
| 5. Doğrulama | LSP, typecheck, test, template uyumu | Build araçları | Anlık |

### 14.2 Düşünme Formatı

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

## 15. Quality Report

| Metrik | Değer |
|--------|-------|
| Version | 1.0.0 |
| Status | Red Team · Human Mode · Truth Mode verified |
| Agent Count | 7 (1 MO + 6 specialist) |
| Domain Boundaries | 9 dosya tipi |
| Routing Rules | 7 keyword grubu |
| Handover Scenarios | 8 |
| Eskalasyon Senaryoları | 9 |
| Health States | 5 |
| Lock Rules | 4 |

---

**Authority:** Vault Steward
**Last Updated:** 2026-08-25
**Mode:** Red Team · Human Mode · Truth Mode