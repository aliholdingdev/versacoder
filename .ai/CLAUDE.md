# Versa Coder — AI Constitution & Boot Protocol

**Type:** Constitution | **Category:** core | **Status:** active | **Version:** 1.0.0
**Authority:** Single Source of Truth (SSOT)
**Governance:** Red Team · Human Mode · Truth Mode

---

## 1. Preamble

Versa Coder, yapay zeka destekli bir IDE (Integrated Development Environment) platformudur. Bu belge, tüm yapay zeka ajanlarının çalışması için zorunlu olan anayasal kuralları, protokolleri ve standartları tanımlar.

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

---

## 3. Temel İlkeler

### 3.1 Single Source of Truth (SSOT)

- Tüm mimari kararlar `.ai/` vault'unda saklanır
- Dış kaynaklar yalnızca referans için kullanılır
- Çelişki durumunda vault'taki bilgi geçerlidir

### 3.2 Zero Code Before Plan

- Kod yazmadan önce plan oluşturulmalıdır
- Plan onay aldıktan sonra kodlama başlar
- Plansız kodlama yasaktır

### 3.3 Human Approval Gate

- Kritik kararlar için insan onayı gereklidir
- Mimari değişiklikler onay gerektirir
- Güvenlikle ilgili değişiklikler onay gerektirir

---

## 4. Guardrails (Koruyucu Kurallar)

### 4.1 Zorunlu Guardrails

| # | Kural | Sonuç |
|---|-------|-------|
| 1 | Kod yazmadan önce plan yap | Kod geri alınır |
| 2 | Vault'tan bilgi almadan kodlama yapma | Kod geçersiz |
| 3 | Uydurma bilgi kullanma | İçerik silinir |
| 4 | Dosyaları yerinde değiştir (refactoring) | Dosya geri yüklenir |
| 5 | Tek Doğruluk Kaynağı kullan | Dış bilgi reddedilir |
| 6 | Şablon kullanımı zorunlu | Dosya geçersiz |
| 7 | Session sürekliliği sağla | Bağlam kaybolur |
| 8 | İnsan onayı al | Kod geri alınır |
| 9 | Bağlam toplama önce yap | Yanlış çıktı |
| 10 | Öğrenme aktif tut | Tekrar hata |
| 11 | Diagram öğretme yap | Yanlış anlama |
| 12 | Çelişki kapısı oluştur | Süreç durur |
| 13 | ORM kullanma (EF Core DbContext ONLY) | SQL enjeksiyonu |
| 14 | WinForms code-behind kullanma | Bakımı zor kod |
| 15 | DevExpress kullanımı zorunlu | Tutarsızlık |
| 16 | SQLite WAL modu kullan | Performans düşüklüğü |

### 4.2 Guardrail Uygulama

Her guardrail ihlali tespit edildiğinde:
1. Hata loglanır
2. İşlem durdurulur
3. İnsan onayı beklenir
4. Düzeltme yapılır
5. Devam edilir

---

## 5. Agent Kullanım Protokolü

### 5.1 Agent Seçim Kriterleri

| Durum | Kullanılacak Agent |
|-------|-------------------|
| Kod yazma/düzenleme | Build Agent |
| Mimari planlama | Plan Agent |
| Kod analizi/tarama | Explore Agent |
| Dokümantasyon | Summary Agent |
| İsimlendirme | Title Agent |
| Genel görevler | General Agent |
| Koordinasyon | Master Orchestrator |

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

---

## 6. Mimari Kurallar

### 6.1 Katmanlı Mimari (Clean Architecture)

| Katman | Ad | Sorumluluk |
|--------|-----|-----------|
| L0 | Domain | Varlıklar, Değer Nesneleri, Olaylar |
| L1 | Abstractions | Arayüzler, Sözleşmeler |
| L2 | Application | Use Case'ler, DTO'lar, Handler'lar |
| L3 | CrossCutting | Loglama, İstisna, Doğrulama |
| L4 | Infrastructure | Modüller, Servisler |
| L5 | Protocol | AI Protokol, MCP |
| L6 | Host | Başlatma, DI, Yapılandırma |
| L7 | UI | DevExpress WinForms |

### 6.2 Bağımlılık Kuralları

- Her katman yalnızca bir alt katmana bağımlı olabilir
- L0 hiçbir katmana bağımlı değildir
- L7 yalnızca L6'ya bağımlıdır
- Bağımlılık ihlali yasaktır

### 6.3 Teknoloji Yığını

| Katman | Teknoloji |
|--------|-----------|
| UI | DevExpress WinForms 2026 Universal |
| Backend | C# .NET 8 |
| ORM | Entity Framework Core 8 (DbContext ONLY) |
| Veritabanı | SQLite (WAL modu) |
| AI | Çoklu sağlayıcı (OpenAI, Anthropic, Google, Ollama) |
| MCP | Model Context Protocol |
| Git | LibGit2Sharp |
| Loglama | Serilog |
| Test | xUnit |
| IoC | Microsoft.Extensions.DependencyInjection |
| MVVM | CommunityToolkit.Mvvm |
| Doğrulama | FluentValidation |
| Dayanıklılık | Polly |
| Markdown | Markdig |
| CQRS | MediatR |

---

## 7. Güvenlik Kuralları

### 7.1 Hassas Veri Koruması

- API anahtarları vault'ta saklanır
- Veritabanı şifreleri şifrelenir
- Loglarda hassas veri bulunmaz
- Günlük erişim logları tutulur

### 7.2 Erişim Kontrolü

- Her ajan yalnızca kendi dosyalarına erişebilir
- Vault dosyaları yalnızca okunabilir
- Log dosyaları yalnızca eklenebilir
- Config dosyaları yalnızca Plan Agent tarafından değiştirilebilir

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

---

## 10. Performans Hedefleri

| Metrik | Hedef |
|--------|-------|
| Yanıt süresi | < 2 saniye |
| Agent geçiş süresi | < 500 ms |
| Dosya okuma | < 100 ms |
| Veritabanı sorgusu | < 50 ms |
| UI yanıt süresi | < 16 ms (60 FPS) |

---

## 11. Versions & Changelog

| Version | Tarih | Değişiklik |
|---------|-------|-----------|
| 1.0.0 | 2026-08-26 | İlk sürüm, tüm guardrails tanımlandı |

---

**Authority:** Vault Steward
**Last Updated:** 2026-08-26
**Mode:** Red Team · Human Mode · Truth Mode