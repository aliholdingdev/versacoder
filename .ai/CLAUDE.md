# Versa Coder — AI Constitution & Boot Protocol

**Type:** Constitution | **Category:** core | **Status:** active | **Version:** 1.0.0
**Authority:** Single Source of Truth (SSOT)
**Governance:** Red Team · Human Mode · Truth Mode
**Token Budget:** ~6000 token

---

## 1. Preamble

Versa Coder, yapay zeka destekli bir IDE (Integrated Development Environment) platformudur. Bu belge, tüm yapay zeka ajanlarının çalışması için zorunlu olan anayasal kuralları, protokolleri ve standartları tanımlar.

### 1.1 Proje Tanımı

| Özellik | Değer |
|---------|-------|
| Proje Adı | Versa Coder |
| Tür | AI-Integrated IDE |
| Platform | DevExpress WinForms |
| Dil | C# .NET 8 |
| Veritabanı | SQLite (WAL) |
| AI | Çoklu provider (OpenAI, Anthropic, Google, Ollama) |

---

## 2. Boot Protocol

### 2.1 Başlangıç Sırası

Her AI session başlatıldığında aşağıdaki sırayla yüklenmelidir:

| Sıra | Dosya | Amaç | Token Limiti |
|------|-------|------|--------------|
| 1 | CLAUDE.md | AI anayasası | ~6000 |
| 2 | AGENTS.md | Ajan kayıt defteri | ~5000 |
| 3 | WORKFLOW.md | Mühendislik süreçleri | ~4000 |
| 4 | brain.md | Mimari kararlar | ~4000 |
| 5 | ROLE.md | Rol tanımları | ~5000 |
| 6 | index.md | Ana katalog | ~1500 |
| 7 | keys.md | Anahtar kelime eşleme | ~1000 |

**Toplam Token Bütçesi:** ~26,500 token

### 2.2 Session Başlatma Prosedürü

```
1. Vault yükle (CLAUDE.md → AGENTS.md → WORKFLOW.md → brain.md)
2. Son session log'unu oku
3. Proje durumunu kontrol et
4. Kullanıcı isteğini analiz et
5. Uygun agent'ı seç
6. Görevi başlat
```

### 2.3 Context Assembly

```
Session Başlatma
  → [1. Vault Yükle] — CLAUDE.md → AGENTS.md → WORKFLOW.md → brain.md
    → [2. Session Log] — Son session'ı oku
      → [3. Proje Durumu] — Mevcut durumu kontrol et
        → [4. İstek Analizi] — Kullanıcı isteğini analiz et
          → [5. Agent Seçimi] — Uygun agent'ı seç
            → [6. Görev Başlatma] — Görevi başlat
```

---

## 3. Temel İlkeler

### 3.1 Single Source of Truth (SSOT)

- Tüm mimari kararlar `.ai/` vault'unda saklanır
- Dış kaynaklar yalnızca referans için kullanılır
- Çelişki durumunda vault'taki bilgi geçerlidir
- Vault'un güncellenmesi yalnızca Master Orchestrator tarafından yapılabilir

### 3.2 Zero Code Before Plan

- Kod yazmadan önce plan oluşturulmalıdır
- Plan onay aldıktan sonra kodlama başlar
- Plansız kodlama yasaktır
- Plan değişiklikleri loglanmalıdır

### 3.3 Human Approval Gate

- Kritik kararlar için insan onayı gereklidir
- Mimari değişiklikler onay gerektirir
- Güvenlikle ilgili değişiklikler onay gerektirir
- Onay alınmadan işlem yapılmaz

### 3.4 Learn & Adapt

- Her görev sonrası öğrenme kaydı tutulur
- Hatalar tekrarlanmamalıdır
- Başarılı kalıplar kaydedilmelidir
- Knowledge base sürekli güncellenir

---

## 4. Guardrails (Koruyucu Kurallar)

### 4.1 Zorunlu Guardrails

| # | Kural | Sonuç | Kategori |
|---|-------|-------|----------|
| 1 | Kod yazmadan önce plan yap | Kod geri alınır | Process |
| 2 | Vault'tan bilgi almadan kodlama yapma | Kod geçersiz | Knowledge |
| 3 | Uydurma bilgi kullanma | İçerik silinir | Integrity |
| 4 | Dosyaları yerinde değiştir (refactoring) | Dosya geri yüklenir | Code |
| 5 | Tek Doğruluk Kaynağı kullan | Dış bilgi reddedilir | Knowledge |
| 6 | Şablon kullanımı zorunlu | Dosya geçersiz | Code |
| 7 | Session sürekliliği sağla | Bağlam kaybolur | Process |
| 8 | İnsan onayı al | Kod geri alınır | Process |
| 9 | Bağlam toplama önce yap | Yanlış çıktı | Knowledge |
| 10 | Öğrenme aktif tut | Tekrar hata | Learning |
| 11 | Diagram öğretme yap | Yanlış anlama | Knowledge |
| 12 | Çelişki kapısı oluştur | Süreç durur | Process |
| 13 | ORM kullanma (EF Core DbContext ONLY) | SQL enjeksiyonu | Security |
| 14 | WinForms code-behind kullanma | Bakımı zor kod | Code |
| 15 | DevExpress kullanımı zorunlu | Tutarsızlık | UI |
| 16 | SQLite WAL modu kullan | Performans düşüklüğü | Performance |

### 4.2 Guardrail Uygulama

Her guardrail ihlali tespit edildiğinde:
1. Hata loglanır
2. İşlem durdurulur
3. İnsan onayı beklenir
4. Düzeltme yapılır
5. Devam edilir

### 4.3 Guardrail Kategorileri

| Kategori | Guardrail Sayısı | Örnekler |
|----------|-----------------|----------|
| Process | 4 | #1, #7, #8, #12 |
| Knowledge | 3 | #2, #5, #9, #11 |
| Code | 2 | #4, #6, #14 |
| Security | 1 | #13 |
| Performance | 1 | #16 |
| UI | 1 | #15 |
| Learning | 1 | #10 |

---

## 5. Agent Kullanım Protokolü

### 5.1 Agent Seçim Kriterleri

| Durum | Kullanılacak Agent | Öncelik |
|-------|-------------------|---------|
| Kod yazma/düzenleme | Build Agent | Yüksek |
| Mimari planlama | Plan Agent | Yüksek |
| Kod analizi/tarama | Explore Agent | Orta |
| Dokümantasyon | Summary Agent | Orta |
| İsimlendirme | Title Agent | Düşük |
| Genel görevler | General Agent | Düşük |
| Koordinasyon | Master Orchestrator | Yüksek |

### 5.2 Agent Çalıştırma Prosedürü

```
1. Kullanıcı isteğini analiz et
2. Keyword'leri çıkar
3. Uygun agent'ı seç
4. Agent profilini yükle
5. Görevi tanımla
6. Agent'ı çalıştır
7. Çıktıyı doğrula
8. Gerekirse handover yap
```

### 5.3 Agent Seçim Algoritması

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

---

## 6. Mimari Kurallar

### 6.1 Katmanlı Mimari (Clean Architecture)

| Katman | Ad | Sorumluluk | Bağımlılık |
|--------|-----|-----------|-----------|
| L0 | Domain | Varlıklar, Değer Nesneleri, Olaylar | Hiçbiri |
| L1 | Abstractions | Arayüzler, Sözleşmeler | L0 |
| L2 | Application | Use Case'ler, DTO'lar, Handler'lar | L1 |
| L3 | CrossCutting | Loglama, İstisna, Doğrulama | L2 |
| L4 | Infrastructure | Modüller, Servisler | L3 |
| L5 | Protocol | AI Protokol, MCP | L4 |
| L6 | Host | Başlatma, DI, Yapılandırma | L5 |
| L7 | UI | DevExpress WinForms | L6 |

### 6.2 Bağımlılık Kuralları

```
L7 → L6 (İzin verilen)
L6 → L5 (İzin verilen)
L5 → L4 (İzin verilen)
L4 → L3 (İzin verilen)
L3 → L2 (İzin verilen)
L2 → L1 (İzin verilen)
L1 → L0 (İzin verilen)

L0 → L2 (YASAK)
L1 → L3 (YASAK)
L2 → L4 (YASAK)
L3 → L5 (YASAK)
L4 → L6 (YASAK)
L5 → L7 (YASAK)
```

### 6.3 Teknoloji Yığını

| Katman | Teknoloji | Versiyon |
|--------|-----------|----------|
| UI | DevExpress WinForms | 2026 Universal |
| Backend | C# .NET | 8.0 |
| ORM | Entity Framework Core | 8.0 |
| Veritabanı | SQLite | WAL modu |
| AI | Çoklu sağlayıcı | OpenAI, Anthropic, Google, Ollama |
| MCP | Model Context Protocol | Latest |
| Git | LibGit2Sharp | Latest |
| Loglama | Serilog | Latest |
| Test | xUnit | Latest |
| IoC | MS.Extensions.DI | Latest |
| MVVM | CommunityToolkit.Mvvm | Latest |
| Doğrulama | FluentValidation | Latest |
| Dayanıklılık | Polly | Latest |
| Markdown | Markdig | Latest |
| CQRS | MediatR | Latest |

---

## 7. Güvenlik Kuralları

### 7.1 Hassas Veri Koruması

- API anahtarları vault'ta saklanır
- Veritabanı şifreleri şifrelenir
- Loglarda hassas veri bulunmaz
- Günlük erişim logları tutulur

### 7.2 Erişim Kontrolü

| Kaynak | Erişim | Sorumlu |
|--------|--------|---------|
| Kod dosyaları | Build Agent | Sadece o |
| Config dosyaları | Plan Agent | Sadece o |
| Vault dosyaları | Tümü (okuma) | MO (yazma) |
| Log dosyaları | Tümü (append) | — |
| Test dosyaları | Build Agent | Sadece o |

### 7.3 Güvenlik Seviyeleri

| Seviye | Tanım | Aksiyon |
|--------|-------|---------|
| Critical | Sistem açığı | Derhal düzelt |
| High | Veri sızıntısı | 24 saat içinde |
| Medium | Yetki hatası | 1 hafta içinde |
| Low | İyileştirme | Plan dahilinde |

---

## 8. Kalite Standartları

### 8.1 Kod Kalitesi

- SOLID prensiplerine uygunluk
- Temiz Kod (Clean Code) standartları
- Minimum %80 test kapsama oranı
- Statik analiz hataları = 0

### 8.2 Dokümantasyon

- Her public API için XML doc
- Her entity için açıklama
- Her use case için senaryo
- Her karar için ADR (Architecture Decision Record)

### 8.3 Quality Gates

| Gate | Koşul | Zorunlu |
|------|-------|---------|
| Build Pass | 0 hata | ✅ |
| Test Pass | %100 başarılı | ✅ |
| Coverage | ≥ %80 | ✅ |
| Code Review | ≥ 1 onay | ✅ |
| Security Scan | 0 kritik | ✅ |
| Style Check | Uyarı yok | ✅ |

---

## 9. Acil Durum Protokolleri

### 9.1 Sistem Hatası

```
1. Hata türünü belirle
2. Etkilenen ajanı tespit et
3. İnsan onayı al
4. Düzeltme yap
5. Test et
6. Devam et
```

### 9.2 Veri Kaybı Riski

```
1. İşlemi durdur
2. Mevcut durumu kaydet
3. İnsan onayı al
4. Kurtarma yap
5. Devam et
```

### 9.3 Acil Durum Kontakları

| Durum | Sorumlu | İletişim |
|-------|---------|----------|
| Sistem Hatası | Build Agent → MO → İnsan | log.md |
| Güvenlik Açığı | MO → İnsan | Dialog |
| Veri Kaybı | MO → İnsan | Dialog |
| Performans | Build Agent → MO | log.md |

---

## 10. Performans Hedefleri

| Metrik | Hedef | Kritik Eşik |
|--------|-------|-------------|
| Yanıt süresi | < 2 saniye | > 5 saniye |
| Agent geçiş süresi | < 500 ms | > 1 saniye |
| Dosya okuma | < 100 ms | > 500 ms |
| Veritabanı sorgusu | < 50 ms | > 200 ms |
| UI yanıt süresi | < 16 ms (60 FPS) | > 32 ms (30 FPS) |
| Memory kullanımı | < 500 MB | > 1 GB |
| CPU kullanımı | < %30 | > %80 |

---

## 11. Versions & Changelog

| Version | Tarih | Değişiklik |
|---------|-------|-----------|
| 1.0.0 | 2026-08-26 | İlk sürüm, tüm guardrails tanımlandı |

---

## 12. Ek Protokoller

### 12.1 Ultra Düşünme Protokolü

Tüm agent'lar kod yazmadan önce bu protokolü uygulamak ZORUNDADIR:

| Adım | Kontrol | Kaynak |
|------|---------|--------|
| 1 | Vault Oku | CLAUDE.md → AGENTS.md → WORKFLOW.md → brain.md |
| 2 | Bağlamı Anla | Domain, katman, dosyalar |
| 3 | Hata Kontrolü | Syntax, imports, types |
| 4 | Sonuç Tahmini | Etki alanı, edge cases |
| 5 | Doğrulama | LSP, typecheck, test |

### 12.2 Handover Protokolü

```
[Kaynak Agent] → [Handover Request] → [Hedef Agent] → [Onay/Red] → [Confirmation]
```

### 12.3 Eskalasyon Protokolü

```
Level 1 (Domain Lead) → Level 2 (Tech Lead) → Level 3 (Arch Lead) → İnsan
```

---

**Authority:** Vault Steward
**Last Updated:** 2026-08-26
**Mode:** Red Team · Human Mode · Truth Mode