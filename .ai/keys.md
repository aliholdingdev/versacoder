---
title: "Versa Coder — Keyword Haritası"
type: reference
category: navigation
date: 2026-08-25
updated: 2026-08-25
status: active
version: 1.0.0
---

# Versa Coder — Keyword Haritası

**Zorunlu Bağlantılar:** [[CLAUDE.md]] · [[AGENTS.md]] · [[index.md]]

---

## 1. Amaç

Bu dosya, AI ajanlarının hangi keyword'leri hangi vault dosyalarına yönlendireceğini gösteren **keyword → dosya eşleme haritasıdır**.

---

## 2. Keyword Kategorileri

### 2.1 Mimari & Yapı

| Keyword | Hedef Dosya | Açıklama |
|---------|-------------|----------|
| mimari, architecture, katman, layer | [[architecture/00-overview/architecture-master]] | Ana mimari plan |
| L0, domain, varlık, entity | [[architecture/l0-domain/domain-guide]] | Domain katmanı |
| L1, abstractions, arayüz | [[architecture/l1-abstractions/abstractions-guide]] | Abstractions katmanı |
| L2, application, use case | [[architecture/l2-application/application-guide]] | Application katmanı |
| L3, crosscutting | [[architecture/l3-crosscutting/crosscutting-guide]] | CrossCutting katmanı |
| L4, infrastructure | [[architecture/l4-infrastructure/infrastructure-guide]] | Infrastructure katmanı |
| L5, protocol, MCP | [[architecture/l5-protocol/protocol-guide]] | Protocol katmanı |
| L6, host, DI | [[architecture/l6-host/host-guide]] | Host katmanı |
| L7, UI, DevExpress | [[architecture/l7-ui/ui-guide]] | UI katmanı |

### 2.2 AI & Provider

| Keyword | Hedef Dosya | Açıklama |
|---------|-------------|----------|
| provider, LLM, OpenAI, Anthropic | [[architecture/l4-infrastructure/ai/provider-router]] | Provider routing |
| agent, runner, orkestrasyon | [[architecture/l4-infrastructure/ai/agent-runner]] | Agent runner |
| tool, araç, 45+ | [[architecture/l4-infrastructure/ai/tool-system]] | Tool sistemi |
| AI, yapay zeka, model | [[CLAUDE.md]] §6 | AI provider mimarisi |

### 2.3 Agent Sistemi

| Keyword | Hedef Dosya | Açıklama |
|---------|-------------|----------|
| build, kod, yaz, oluştur | [[.agents/build-agent]] | Build Agent |
| plan, planla, tasarla | [[.agents/plan-agent]] | Plan Agent |
| explore, analiz, tara | [[.agents/explore-agent]] | Explore Agent |
| general, genel | [[.agents/general-agent]] | General Agent |
| summary, özet | [[.agents/summary-agent]] | Summary Agent |
| title, başlık, isim | [[.agents/title-agent]] | Title Agent |
| MO, master, orkestratör | [[.agents/master-orchestrator]] | Master Orchestrator |

### 2.4 Veritabanı & Data

| Keyword | Hedef Dosya | Açıklama |
|---------|-------------|----------|
| database, veritabanı, SQLite | [[architecture/l4-infrastructure/data/database-schema]] | DB şeması |
| EF Core, entity, migration | [[architecture/l4-infrastructure/data/database-schema]] | EF config |
| repository, depo | [[architecture/l4-infrastructure/infrastructure-guide]] | Repository pattern |

### 2.5 Süreç & Workflow

| Keyword | Hedef Dosya | Açıklama |
|---------|-------------|----------|
| workflow, süreç, prosedür | [[WORKFLOW.md]] | Tüm süreçler |
| code review, inceleme | [[WORKFLOW.md]] §5.1 | Code review workflow |
| bug fix, hata düzeltme | [[WORKFLOW.md]] §5.2 | Bug fix workflow |
| feature, özellik | [[WORKFLOW.md]] §5.3 | New feature workflow |
| session, oturum | [[WORKFLOW.md]] §5.4 | Session init |
| vault sync, senkronizasyon | [[WORKFLOW.md]] §5.5 | Vault sync |

### 2.6 Güvenlik & Kural

| Keyword | Hedef Dosya | Açıklama |
|---------|-------------|----------|
| security, güvenlik | [[rules/security-architecture]] | Güvenlik mimarisi |
| coding standard, kod standartı | [[rules/coding-standards]] | Kod standartları |
| performance, performans | [[rules/performance-guidelines]] | Performans |
| deployment, dağıtım | [[rules/deployment-guide]] | Dağıtım rehberi |
| plugin, eklenti | [[rules/plugin-development]] | Plugin geliştirme |
| MCP, protocol | [[rules/mcp-integration]] | MCP entegrasyonu |

### 2.7 Skill & Şablon

| Keyword | Hedef Dosya | Açıklama |
|---------|-------------|----------|
| skill, beceri | [[skills/]] | Skill listesi |
| template, şablon | [[.templates/index]] | Template kataloğu |
|ADR, karar | [[decisions/adr-template]] | ADR şablonu |

---

## 3. Hızlı Erişim Tablosu

| İhtiyaç | Keyword Örnekleri | İlk Tıklama |
|---------|-------------------|-------------|
| Yeni dosya oluştur | "oluştur", "yaz", "class" | [[.agents/build-agent]] |
| Mimari planla | "plan", "mimari", "tasarım" | [[.agents/plan-agent]] |
| Kod analiz et | "analiz", "tara", "bul" | [[.agents/explore-agent]] |
| Doküman yaz | "doc", "özet", "markdown" | [[.agents/summary-agent]] |
| İsim bul | "isim", "naming", "başlık" | [[.agents/title-agent]] |
| Hata düzelt | "bug", "hata", "fix" | [[WORKFLOW.md]] §5.2 |
| Test yaz | "test", "xUnit" | [[skills/testing-skill]] |
| Güvenlik kontrol | "security", "güvenlik" | [[rules/security-architecture]] |

---

## 4. Token Optimizasyon Haritası

### 4.1 Dosya Boyutu ve Token Karşılıkları

| Dosya | Satır | Tahmini Token | Öncelik |
|-------|-------|---------------|---------|
| CLAUDE.md | ~700 | ~6000 | Zorunlu |
| AGENTS.md | ~600 | ~5000 | Zorunlu |
| WORKFLOW.md | ~600 | ~4000 | Zorunlu |
| brain.md | ~600 | ~4000 | Zorunlu |
| ROLE.md | ~550 | ~5000 | Zorunlu |
| index.md | ~600 | ~1500 | Opsiyonel |
| keys.md | ~150 | ~1000 | Opsiyonel |

### 4.2 Lazy Loading Stratejisi

| Senaryo | Yüklenen Dosyalar | Token |
|---------|-------------------|-------|
| Session Başlatma | CLAUDE.md, AGENTS.md | ~11,000 |
| Kod Yazma | + WORKFLOW.md, brain.md | ~19,000 |
| Mimari Planlama | + ROLE.md, index.md | ~26,500 |
| Tam Yükleme | Tüm dosyalar | ~26,500 |

### 4.3 Kategori Bazlı Yükleme

| Kategori | Dosyalar | Kullanım |
|----------|----------|----------|
| Core | CLAUDE.md, AGENTS.md | Her session |
| Workflow | WORKFLOW.md | Geliştirme |
| Architecture | brain.md | Mimari kararlar |
| Roles | ROLE.md | Görev dağıtımı |
| Navigation | index.md, keys.md | Arama |

---

## 5. Keyword → Agent Routing Detail

### 5.1 Build Agent Keywords

| Keyword | Kullanım | Öncelik |
|---------|----------|---------|
| kod, code | Kod yazma | Yüksek |
| class | Sınıf oluşturma | Yüksek |
| method | Method oluşturma | Yüksek |
| property | Property oluşturma | Yüksek |
| service | Servis oluşturma | Yüksek |
| repository | Repository oluşturma | Yüksek |
| handler | Handler oluşturma | Yüksek |
| test | Test yazma | Yüksek |
| bug | Hata düzeltme | Yüksek |
| fix | Düzeltme | Yüksek |
| refactor | Yeniden yapılandırma | Orta |
| optimize | Optimizasyon | Orta |

### 5.2 Plan Agent Keywords

| Keyword | Kullanım | Öncelik |
|---------|----------|---------|
| plan | Planlama | Yüksek |
| mimari | Mimari planlama | Yüksek |
| architecture | Mimari planlama | Yüksek |
| task | Görev dağıtımı | Yüksek |
| phase | Aşama planlama | Yüksek |
| milestone | Kilometre taşı | Yüksek |
| design | Tasarım | Yüksek |
| structure | Yapı tasarımı | Yüksek |
| module | Modül tasarımı | Yüksek |

### 5.3 Explore Agent Keywords

| Keyword | Kullanım | Öncelik |
|---------|----------|---------|
| analiz | Kod analizi | Orta |
| tarama | Dosya tarama | Orta |
| grep | İçerik arama | Orta |
| glob | Dosya arama | Orta |
| dosya bul | Dosya bulma | Orta |
| search | Arama | Orta |
| find | Bulma | Orta |
| scan | Tarama | Orta |
| review | İnceleme | Orta |

### 5.4 Summary Agent Keywords

| Keyword | Kullanım | Öncelik |
|---------|----------|---------|
| doc | Dokümantasyon | Orta |
| özet | Özetleme | Orta |
| dokümantasyon | Dokümantasyon | Orta |
| markdown | Markdown yazma | Orta |
| readme | README oluşturma | Orta |
| changelog | Changelog güncelleme | Orta |
| adr | ADR yazma | Orta |

### 5.5 Title Agent Keywords

| Keyword | Kullanım | Öncelik |
|---------|----------|---------|
| başlık | Başlık oluşturma | Düşük |
| isim | İsim bulma | Düşük |
| naming | İsimlendirme | Düşük |
| convention | Kural | Düşük |
| pattern | Kalıp | Düşük |

---

## 6. Arama Optimizasyonu

### 6.1 Hızlı Arama Yolları

| İhtiyaç | Arama Yöntemi | Hız |
|---------|---------------|-----|
| Dosya bulma | Glob pattern | ~10ms |
| İçerik arama | Grep regex | ~50ms |
| Keyword eşleme | keys.md | ~5ms |
| Semantic arama | AI embedding | ~500ms |

### 6.2 Glob Pattern Örnekleri

| Pattern | Amaç |
|---------|------|
| `**/*.cs` | Tüm C# dosyaları |
| `src/**/*.cs` | src altındaki C# dosyaları |
| `.ai/**/*.md` | Vault'taki tüm MD dosyaları |
| `tests/**/*.cs` | Tüm test dosyaları |

### 6.3 Grep Pattern Örnekleri

| Pattern | Amaç |
|---------|------|
| `class\s+\w+` | Tüm class tanımları |
| `interface\s+I\w+` | Tüm interface tanımları |
| `public\s+async\s+Task` | Tüm async methodlar |
| `TODO\|FIXME\|HACK` | Yapılacaklar |

---

## 7. Bağlam Toplama Rehberi

### 7.1 Prompt Analizi

```
Kullanıcı Prompt'u
  → [1. Keyword Çıkarma] — keys.md'den eşle
    → [2. Dosya Seçimi] — İlgili dosyaları bul
      → [3. Token Hesaplama] — Bütçe kontrolü
        → [4. Yükleme] — Dosyaları yükle
          → [5. Bağlam Oluşturma] — Context oluştur
```

### 7.2 Token Bütçesi Yönetimi

```csharp
public class TokenBudgetManager
{
    private const int MaxTokens = 26500;
    private const int ReservedTokens = 5000; // Response için
    
    public List<VaultFile> SelectFiles(string prompt, List<VaultFile> availableFiles)
    {
        var selectedFiles = new List<VaultFile>();
        var currentTokens = 0;
        
        // 1. Zorunlu dosyaları yükle
        foreach (var file in availableFiles.Where(f => f.IsMandatory))
        {
            if (currentTokens + file.EstimatedTokens <= MaxTokens - ReservedTokens)
            {
                selectedFiles.Add(file);
                currentTokens += file.EstimatedTokens;
            }
        }
        
        // 2. Prompt'a göre ilgili dosyaları ekle
        var relevantFiles = GetRelevantFiles(prompt, availableFiles);
        foreach (var file in relevantFiles)
        {
            if (currentTokens + file.EstimatedTokens <= MaxTokens - ReservedTokens)
            {
                selectedFiles.Add(file);
                currentTokens += file.EstimatedTokens;
            }
        }
        
        return selectedFiles;
    }
}
```

### 7.3 Bağlam Kalitesi Metrikleri

| Metrik | Hedef | Kritik Eşik |
|--------|-------|-------------|
| Dosya kapsama | > %80 | < %50 |
| Token verimliliği | > %70 | < %50 |
| Alakalılık | > %90 | < %70 |
| Tamlık | > %95 | < %80 |

---

**Authority:** Vault Steward
**Last Updated:** 2026-08-26
**Mode:** Red Team · Human Mode · Truth Mode