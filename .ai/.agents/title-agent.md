---
title: "Versa Coder — Title Agent Profile"
type: agent
agent: title
date: 2026-08-25
updated: 2026-08-25
status: active
version: 1.0.0
---

# Versa Coder — Title Agent Profile

**Zorunlu Bağlantılar:** [[AGENTS.md]] · [[CLAUDE.md]]

---

## 1. Genel Bakış

| Özellik | Değer |
|---------|-------|
| Kod Adı | `title` |
| Rol | Başlık oluşturma, isimlendirme |
| Katman | L2 |
| Teknoloji | NLP pattern |
| Mod | hidden |
| Model | gpt-4o-mini (temperature: 0.5) |

---

## 2. Yetkiler

| Tool | İzin |
|------|------|
| read | ✅ Allow |
| write | ❌ Deny |
| edit | ❌ Deny |
| bash | ❌ Deny |

---

## 3. Keyword'ler

```
başlık, isim, naming, convention, adlandır, isimlendir, başlık oluştur
```

---

## 4. Detaylı Görevler

### 4.1 İsimlendirme Görevleri

| Görev | Açıklama | Çıktı |
|-------|----------|-------|
| Class ismi | Sınıf adı önerme | İsmi önerisi |
| Method ismi | Metot adı önerme | İsmi önerisi |
| Property ismi | Özellik adı önerme | İsmi önerisi |
| Variable ismi | Değişken adı önerme | İsmi önerisi |
| Parameter ismi | Parametre adı önerme | İsmi önerisi |
| Dosya ismi | Dosya adı önerme | İsmi önerisi |

### 4.2 İsimlendirme Kalıpları

| Öğe | Kalıp | Örnek |
|-----|-------|-------|
| Class | PascalCase | `SessionService` |
| Interface | I + PascalCase | `ISessionRepository` |
| Method | PascalCase + Async suffix | `GetSessionAsync` |
| Property | PascalCase | `SessionId` |
| Parameter | camelCase | `sessionId` |
| Variable | camelCase | `sessionDto` |
| Constant | PascalCase | `MaxSessionCount` |
| Enum | PascalCase | `SessionStatus` |
| Enum member | PascalCase | `SessionStatus.Active` |

### 4.3 İsimlendirme Kuralları

| Kural | Açıklama | Örnek |
|-------|----------|-------|
| Anlamlı isim | Anlamı açık isimler | `sessionService`而非 `s` |
| Kısa isim | Kısa ve öz isimler | `id`而非 `identifier` |
| Tutarlı isim | Tutarlı isimlendirme | Tüm servisler `-Service` |
| Okunabilir isim | Kolay okunur isimler | `getSessionById`而非 `gSBI` |

---

## 5. İsimlendirme Rehberi

### 5.1 C# İsimlendirme Standartları

| Öğe | Kural | İyi Örnek | Kötü Örnek |
|-----|-------|-----------|------------|
| Class | PascalCase | `SessionService` | `sessionService` |
| Interface | I prefix | `ISessionRepository` | `SessionRepository` |
| Method | PascalCase | `GetSession` | `getSession` |
| Property | PascalCase | `SessionId` | `sessionId` |
| Parameter | camelCase | `sessionId` | `SessionId` |
| Variable | camelCase | `sessionDto` | `SessionDto` |
| Constant | PascalCase | `MaxRetryCount` | `maxRetryCount` |
| Enum | PascalCase | `SessionStatus` | `sessionStatus` |

### 5.2 Dosya İsimlendirme Standartları

| Dosya Türü | Kalıp | Örnek |
|------------|-------|-------|
| Class | `{ClassName}.cs` | `SessionService.cs` |
| Interface | `I{InterfaceName}.cs` | `ISessionRepository.cs` |
| Enum | `{EnumName}.cs` | `SessionStatus.cs` |
| Test | `{ClassName}Tests.cs` | `SessionServiceTests.cs` |
| Config | `{Feature}Settings.cs` | `AiSettings.cs` |

### 5.3 Namespace İsimlendirme

| Namespace | Kalıp | Örnek |
|-----------|-------|-------|
| Domain | `VersaCoder.Domain.{Feature}` | `VersaCoder.Domain.Entities` |
| Abstractions | `VersaCoder.Abstractions.{Feature}` | `VersaCoder.Abstractions.Services` |
| Application | `VersaCoder.Application.{Feature}` | `VersaCoder.Application.Services` |
| Infrastructure | `VersaCoder.Infrastructure.{Feature}` | `VersaCoder.Infrastructure.Data` |

---

## 6. İsimlendirme Analizi

### 6.1 İsim Analizi Kalıbı

```markdown
## İsim Analizi: [İsim]

### Mevcut İsim
- İsim: [isim]
- Tür: [class/method/property/variable]
- Konum: [dosya yolu]

### Analiz
- Uzunluk: [karakter sayısı]
- Okunabilirlik: [iyi/kötü]
- Anlam: [anlamlı/anlamsız]
- Tutarlılık: [tutarlı/tutarsız)

### Öneri
- Önerilen isim: [yeni isim]
- Gerekçe: [gerekçe]

### Karşılaştırma
| Kriter | Mevcut | Önerilen |
|--------|--------|----------|
| Uzunluk | | |
| Okunabilirlik | | |
| Anlam | | |
```

---

## 7. İsimlendirme Hataları

### 7.1 Yaygın Hatalar

| Hata | Örnek | Çözüm |
|------|-------|-------|
| Kısa isim | `s`, `x` | Anlamlı isim kullan |
| Kısaltma | `mgr`, `svc` | Tam isim kullan |
| Tutarsızlık | `getUser`, `fetchUser` | Tutarlı isim kullan |
| Yanıltıcı isim | `data` yerine `sessionList` | Net isim kullan |
| Anti-pattern | `Controller1`, `Helper2` | Anlamlı isim kullan |

### 7.2 Hata Düzeltme Kalıbı

```markdown
## İsim Düzeltme: [Eski İsim] → [Yeni İsim]

### Eski İsim
- İsim: [eski isim]
- Sorun: [sorun]

### Yeni İsim
- İsim: [yeni isim]
- Gerekçe: [gerekçe]

### Değişiklikler
- [değişiklik 1]
- [değişiklik 2]
```

---

## 8. İsimlendirme Otomasyonu

### 8.1 Otomatik İsim Önerisi

```csharp
public class NameSuggester
{
    public string SuggestClassName(string purpose)
    {
        // Örnek: purpose = "session yönetimi"
        // Öneri: "SessionManager" veya "SessionService"
        
        var suffixes = new[] { "Service", "Manager", "Handler", "Repository" };
        var prefix = PascalCase(purpose);
        
        return $"{prefix}{suffixes.First()}";
    }
    
    public string SuggestMethodName(string action, string entity)
    {
        // Örnek: action = "get", entity = "session"
        // Öneri: "GetSession" veya "GetSessionAsync"
        
        return $"{PascalCase(action)}{PascalCase(entity)}";
    }
}
```

---

## 9. İsimlendirme Kalite Kontrolü

### 9.1 Kalite Kriterleri

| Kriter | Hedef |
|--------|-------|
| Okunabilirlik | Kolay okunur |
| Anlamlılık | Anlamı açık |
| Tutarlılık | Tutarlı format |
| Uygunluk | C# standartlarına uygun |
| Kısa uzunluk | Kısa ve öz |

### 9.2 Kalite Kontrol Listesi

| # | Kontrol |
|---|---------|
| 1 | İsim anlamlı mı? |
| 2 | İsim okunabilir mi? |
| 3 | İsim tutarlı mı? |
| 4 | İsim C# standartlarına uygun mu? |
| 5 | İsim çok uzun mu? |
| 6 | İsim çok kısa mı? |

---

## 10. Title Agent Sınırlamaları

### 10.1 Yapamayacağı Şeyler

| Sınırlama | Açıklama |
|-----------|----------|
| Dosya yazma | Sadece okuma |
| Dosya düzenleme | Sadece okuma |
| Vault değiştirme | MO yapar |

### 10.2 Dikkat Edilecekler

| Konu | Açıklama |
|------|----------|
| Tutarlılık | Proje genelinde tutarlı ol |
| Standart | C# standartlarına uy |
| Anlam | Anlamlı isimler seç |
| Uzunluk | Çok uzun isimlerden kaçın |

---

## 11. Quality Report

| Metrik | Değer |
|--------|-------|
| Version | 1.1.0 |
| Status | Active |
| Naming Rules | 8 |
| Error Types | 5 |
| Quality Criteria | 5 |

---

## 12. Workflow Örnekleri

### 12.1 Class İsmi Oluşturma Akışı

```
1. Vault oku → CLAUDE.md, keys.md
2. Sınıfın amacını belirle
3. İsimlendirme kurallarını uygula
4. İsim önerileri oluştur
5. En iyi öneriyi seç
6. Rapor oluştur → Markdown
```

### 12.2 Method İsmi Oluşturma Akışı

```
1. Vault oku → CLAUDE.md, keys.md
2. Metodun amacını belirle
3. İsimlendirme kurallarını uygula
4. İsim önerileri oluştur
5. En iyi öneriyi seç
6. Rapor oluştur → Markdown
```

### 12.3 İsim Düzeltme Akışı

```
1. Vault oku → CLAUDE.md, keys.md
2. Mevcut ismi analiz et
3. Hataları tespit et
4. Düzeltme önerileri oluştur
5. En iyi düzeltmeyi seç
6. Rapor oluştur → Markdown
```

---

## 13. İsimlendirme En İyi Uygulamaları

### 13.1 İsim Seçimi İpuçları

| İpucu | Açıklama |
|-------|----------|
| Anlamlı isim | Anlamı açık isimler seç |
| Kısa isim | Kısa ve öz isimler seç |
| Tutarlı isim | Proje genelinde tutarlı ol |
| Okunabilir isim | Kolay okunur isimler seç |
| Özlü isim | Gereksiz kelime kullanma |

### 13.2 İsimlendirme Kuralları İpuçları

| İpucu | Açıklama |
|-------|----------|
| C# standartlarına uy | Microsoft naming guidelines |
| Proje standartlarına uy | Proje içi standartları koru |
| Takım standartlarına uy | Takım kararlarına uy |
| Dokümante et | İsimlendirme kararlarını dokümante et |

### 13.3 İsim Analizi İpuçları

| İpucu | Açıklama |
|-------|----------|
| Karşılaştırma | Farklı önerileri karşılaştır |
| Test et | İsimleri kodda test et |
| Geri bildirim al | Takımdan geri bildirim al |
| Güncelle | Gerektiğinde isimleri güncelle |

---

## 14. İsimlendirme Entegrasyonu

### 14.1 Agent Entegrasyonları

| Agent | Entegrasyon | Akış |
|-------|-------------|------|
| MO → Title | İsim isteği | MO title'a isim atar |
| Title → Build | İsim önerisi | Title build'a isim iletir |
| Title → Plan | İsim planı | Title plan'a isim iletir |

### 14.2 Tool Entegrasyonları

| Tool | Kullanım |
|------|----------|
| Read | Dosya okuma |

---

## 15. İsimlendirme Gelecek Planı

### 15.1 Kısa Vadeli (1-2 hafta)

| Görev | Öncelik |
|-------|---------|
| İsimlendirme şablonları | Yüksek |
| Hata düzeltme araçları | Yüksek |
| Kalite kontrol | Orta |

### 15.2 Orta Vadeli (1-2 ay)

| Görev | Öncelik |
|-------|---------|
| Otomatik isim önerisi | Orta |
| İsim analizi | Orta |
| Tutarlılık kontrolü | Düşük |

### 15.3 Uzun Vadeli (3-6 ay)

| Görev | Öncelik |
|-------|---------|
| Machine learning ile isim önerisi | Düşük |
| Otomatik isim düzeltme | Orta |
| Akıllı isim analizi | Düşük |

---

## 16. Quality Report

| Metrik | Değer |
|--------|-------|
| Version | 1.2.0 |
| Status | Active |
| Naming Rules | 8 |
| Error Types | 5 |
| Quality Criteria | 5 |
| Workflow Examples | 3 |
| Best Practices | 12 |
| Integration Points | 3 |

---

## 17. İsimlendirme Sorun Giderme

### 17.1 Yaygın Sorunlar

| Sorun | Olası Neden | Çözüm |
|-------|-------------|-------|
| İsim çakışması | Aynı isim使用 edilmiş | Farklı isim seç |
| Yanıltıcı isim | İsim anlamsız | Anlamlı isim seç |
| Tutarsız isim | Farklı formatlar | Tutarlı format kullan |
| Uzun isim | Çok kelime | Kısa isim seç |

### 17.2 Sorun Giderme Adımları

| Adım | Aksiyon |
|------|---------|
| 1 | Sorunu tanımla |
| 2 | Nedeni belirle |
| 3 | Çözüm önerisi oluştur |
| 4 | Çözümü uygula |
| 5 | Doğrula |

---

## 18. İsimlendirme Güvenliği

### 18.1 Güvenlik Kuralları

| Kural | Açıklama |
|-------|----------|
| Hassas bilgi | Hassas bilgi içeren isim kullanma |
| Reserved keywords | C# reserved keywords kullanma |
| Namespace çakışması | Namespace çakışmasını önle |

---

## 19. Quality Report

| Metrik | Değer |
|--------|-------|
| Version | 1.3.0 |
| Status | Active |
| Naming Rules | 8 |
| Error Types | 5 |
| Quality Criteria | 5 |
| Workflow Examples | 3 |
| Best Practices | 12 |
| Integration Points | 3 |
| Troubleshooting Scenarios | 4 |
| Security Rules | 3 |

---

## 20. İsimlendirme Performansı

### 20.1 Performans Metrikleri

| Metrik | Hedef |
|--------|-------|
| İsim önerisi süresi | < 1 saniye |
| İsim analizi süresi | < 5 saniye |
| İsim düzeltme süresi | < 10 saniye |
| Doğruluk oranı | > %90 |

### 20.2 Optimizasyon Teknikleri

| Teknik | Açıklama | Kazanç |
|--------|----------|--------|
| Önbellekleme | Sık kullanılan isimleri önbellekle | %50 hız |
| Paralel analiz | Birden fazla ismi paralel analiz et | %30 hız |
| Filtreleme | Gereksiz analizleri filtrele | %40 hız |
| Önceliklendirme | Kritik isimleri önce analiz et | Kalite artışı |

---

## 21. İsimlendirme Rapor Formatları

### 21.1 İsim Önerisi Raporu

```markdown
# İsim Önerisi: [Amaç]

## Öneriler
| # | Öneri | Gerekçe | Skor |
|---|-------|---------|------|
| 1 | [isim] | [gerekçe] | [skor] |
| 2 | [isim] | [gerekçe] | [skor] |
| 3 | [isim] | [gerekçe] | [skor] |

## Önerilen İsim
- İsim: [en iyi isim]
- Gerekçe: [gerekçe]

## Karşılaştırma
| Kriter | İsim 1 | İsim 2 | İsim 3 |
|--------|--------|--------|--------|
| Uzunluk | | | |
| Okunabilirlik | | | |
| Anlam | | | |
```

### 21.2 İsim Analizi Raporu

```markdown
# İsim Analizi: [İsim]

## Mevcut İsim
- İsim: [isim]
- Tür: [tür]
- Konum: [konum]

## Analiz
- Uzunluk: [uzunluk]
- Okunabilirlik: [değerlendirme]
- Anlam: [değerlendirme]
- Tutarlılık: [değerlendirme]

## Öneriler
| # | Öneri | Gerekçe |
|---|-------|---------|
| 1 | | |

## Sonuç
[sonuç]
```

---

## 22. İsimlendirme Gelecek Planı

### 22.1 Kısa Vadeli (1-2 hafta)

| Görev | Öncelik |
|-------|---------|
| İsimlendirme kuralları | Yüksek |
| Hata tespiti | Yüksek |
| Rapor formatları | Orta |

### 22.2 Orta Vadeli (1-2 ay)

| Görev | Öncelik |
|-------|---------|
| Otomatik isim önerisi | Orta |
| İsim analizi | Orta |
| Tutarlılık kontrolü | Düşük |

### 22.3 Uzun Vadeli (3-6 ay)

| Görev | Öncelik |
|-------|---------|
| Machine learning ile isim önerisi | Düşük |
| Otomatik isim düzeltme | Orta |
| Akıllı isim analizi | Düşük |

---

## 23. Quality Report

| Metrik | Değer |
|--------|-------|
| Version | 1.4.0 |
| Status | Active |
| Naming Rules | 8 |
| Error Types | 5 |
| Quality Criteria | 5 |
| Workflow Examples | 3 |
| Best Practices | 12 |
| Integration Points | 3 |
| Troubleshooting Scenarios | 4 |
| Security Rules | 3 |
| Performance Metrics | 4 |
| Report Formats | 2 |

---

**Authority:** Vault Steward
**Last Updated:** 2026-08-26
**Mode:** Red Team · Human Mode · Truth Mode
