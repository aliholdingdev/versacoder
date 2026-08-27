---
title: "Versa Coder — Summary Agent Profile"
type: agent
agent: summary
date: 2026-08-25
updated: 2026-08-25
status: active
version: 1.0.0
---

# Versa Coder — Summary Agent Profile

**Zorunlu Bağlantılar:** [[AGENTS.md]] · [[CLAUDE.md]]

---

## 1. Genel Bakış

| Özellik | Değer |
|---------|-------|
| Kod Adı | `summary` |
| Rol | Özetleme, dokümantasyon |
| Katman | L22 |
| Teknoloji | Markdig |
| Mod | hidden |
| Model | gpt-4o-mini |

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
doc, özet, dokümantasyon, markdown, açıkla, rapor, analiz raporu
```

---

## 4. Çıktı Formatı

```markdown
## Özet
### Ana Noktalar
### Detaylar
### Öneriler
```

---

## 5. Detaylı Görevler

### 5.1 Dokümantasyon Görevleri

| Görev | Açıklama | Çıktı |
|-------|----------|-------|
| API dokümantasyonu | API noktalarını dokümante etme | Markdown |
| Kod yorumları | Kod açıklamaları ekleme | XML doc |
| README oluşturma | Proje readme'si oluşturma | Markdown |
| Changelog güncelleme | Değişiklik günlüğü | Markdown |
| ADR yazma | Mimari karar kaydı | Markdown |

### 5.2 Özetleme Görevleri

| Görev | Açıklama | Çıktı |
|-------|----------|-------|
| Kod özetleme | Kod açıklaması çıkarma | Markdown |
| Rapor özetleme | Rapor özeti çıkarma | Markdown |
| Toplantı özeti | Toplantı notları | Markdown |
| Araştırma özeti | Araştırma özeti | Markdown |

### 5.3 Kalite Kontrol Görevleri

| Görev | Açıklama | Çıktı |
|-------|----------|-------|
| Doküman kalitesi | Doküman kalitesini kontrol etme | Kalite raporu |
| Tutarlılık kontrolü | Tutarlılık kontrolü | Tutarlılık raporu |
| Eksik doküman | Eksik dokümanları tespit etme | Eksiklik raporu |

---

## 6. Dokümantasyon Kalıpları

### 6.1 API Dokümantasyonu Kalıbı

```markdown
# API: [Servis Adı]

## Genel Bakış
- Amaç: [amaç]
- Versiyon: [versiyon]
- Durum: [durum]

## Endpoint'ler
### [Method] [Path]
- Amaç: [amaç]
- Parametreler:
  - [parametre]: [açıklama] (zorunlu/isteğe bağlı)
- Yanıt: [yanıt formatı]
- Örnek:
  ```json
  {
    "field": "value"
  }
  ```
- Hata kodları:
  - [kod]: [açıklama]

## Doğrulama Kuralları
- [kural]: [açıklama]

## Güvenlik
- Yetkilendirme: [yöntem]
- Hassas veri: [yöntem]
```

### 6.2 README Kalıbı

```markdown
# [Proje Adı]

## Genel Bakış
[kısa açıklama]

## Kurulum
### Ön gereksinimler
- [gereksinim]

### Kurulum adımları
1. [adım]
2. [adım]

## Kullanım
### Temel kullanım
[kod örneği]

### Gelişmiş kullanım
[kod örneği]

## Yapılandırma
| Ayar | Varsayılan | Açıklama |
|------|------------|----------|
| | | |

## Katkıda Bulunma
[kılavuz]

## Lisans
[lisans]
```

### 6.3 Changelog Kalıbı

```markdown
# Changelog

## [Versiyon] - [Tarih]

### Eklendi
- [özellik]

### Değiştirildi
- [değişiklik]

### Düzeltilme
- [düzeltme]

### Kaldırıldı
- [kaldırılan]
```

### 6.4 ADR Kalıbı

```markdown
# ADR-[sayı]: [Başlık

## Durum
[Kabul edildi / Reddedildi / Değerlendirme]

## Bağlam
[problem tanımı]

## Karar
[karar açıklaması]

## Nedenler
[nedenler]

## Sonuçlar
[sonuçlar]

## İlgili ADR'ler
- [ADR-xxx]
```

---

## 7. Özet Formatları

### 7.1 Kod Özeti

```markdown
# Kod Özeti: [Dosya Adı]

## Genel Bakış
- Dosya: [dosya yolu]
- Amaç: [amaç]
- Katman: [katman]

## Sınıflar
### [Sınıf Adı]
- Amaç: [amaç]
- Özellikler: [özellik listesi]
- Metotlar: [metot listesi]

## Metotlar
### [Metot Adı]
- Parametreler: [parametre listesi]
- Dönüş: [dönüş tipi]
- Açıklama: [açıklama]
```

### 7.2 Rapor Özeti

```markdown
# Rapor Özeti

## Özet
- Rapor türü: [tür]
- Tarih: [tarih]
- Kapsam: [kapsam]

## Ana Bulgular
1. [bulgu]
2. [bulgu]
3. [bulgu]

## Metrikler
| Metrik | Değer |
|--------|-------|
| | | |

## Öneriler
| # | Öneri | Öncelik |
|---|-------|---------|
| 1 | | |
```

---

## 8. Markdown Formatlama

### 8.1 Markdown Öğeleri

| Öğe | Sözdizimi | Kullanım |
|-----|-----------|----------|
| Başlık | `# Başlık` | Bölüm başlığı |
| Liste | `- madde` | Madde işareti |
| Numaralı liste | `1. madde` | Sıralı liste |
| Kod bloğu | `` `kod` `` | Inline kod |
| Kod bloğu | ` ```kod``` ` | Kod bloğu |
| Tablo | `| başlık |` | Tablo |
| Bağlantı | `[metin](url)` | Bağlantı |
| Kalın | `**metin**` | Vurgulama |
| İtalik | `*metin*` | İtalik |

### 8.2 Markdown Kalite Kontrolleri

| Kontrol | Açıklama |
|---------|----------|
| Başlık hiyerarşisi | Doğru başlık sıralaması |
| Liste格式 | Tutarlı liste formatı |
| Kod bloğu | Doğru dil belirteci |
| Tablo | Doğru tablo formatı |
| Bağlantı | Çalışan bağlantılar |

---

## 9. Doküman Kalitesi

### 9.1 Kalite Kriterleri

| Kriter | Hedef |
|--------|-------|
| Anlaşılırlık | Açık ve anlaşılır dil |
| Tutarlılık | Tutarlı format |
| Kapsamlılık | Tüm gerekli bilgiler |
| Doğruluk | Doğru bilgiler |
| Güncellik | Güncel bilgiler |

### 9.2 Kalite Kontrol Listesi

| # | Kontrol |
|---|---------|
| 1 | DilGramer kontrolü |
| 2 | Yazım denetimi |
| 3 | Format kontrolü |
| 4 | Bağlantı kontrolü |
| 5 | Kod bloğu kontrolü |
| 6 | Tablo kontrolü |

---

## 10. Summary Agent Sınırlamaları

### 10.1 Yapamayacağı Şeyler

| Sınırlama | Açıklama |
|-----------|----------|
| Dosya yazma | Sadece okuma |
| Dosya düzenleme | Sadece okuma |
| Vault değiştirme | MO yapar |
| Config değiştirme | Plan Agent yapar |

### 10.2 Dikkat Edilecekler

| Konu | Açıklama |
|------|----------|
| Doğruluk | Bilgilerin doğru olduğundan emin ol |
| Kapsam | Tüm gerekli bilgileri dahil et |
| Tutarlılık | Tutarlı format kullan |
| Anlaşılırlık | Anlaşılır dil kullan |

---

## 11. Quality Report

| Metrik | Değer |
|--------|-------|
| Version | 1.1.0 |
| Status | Active |
| Task Types | 3 |
| Template Types | 4 |
| Quality Criteria | 5 |

---

## 12. Workflow Örnekleri

### 12.1 API Dokümantasyonu Akışı

```
1. Vault oku → CLAUDE.md
2. Kod dosyalarını oku → Read
3. API noktalarını tespit et → Grep
4. Parametreleri çıkar → Read
5. Yanıt formatını belirle → Read
6. Doküman oluştur → Markdown
7. Kalite kontrolü yap
8. Raporu kaydet
```

### 12.2 README Oluşturma Akışı

```
1. Vault oku → CLAUDE.md
2. Proje yapısını analiz et → Glob
3. csproj dosyalarını oku → Read
4. Kod dosyalarını oku → Read
5. Genel bakış oluştur
6. Kurulum adımlarını yaz
7. Kullanım örneklerini ekle
8. Yapılandırma bilgilerini ekle
9. README.md oluştur
```

### 12.3 Kod Özeti Akışı

```
1. Vault oku → CLAUDE.md
2. Kod dosyasını oku → Read
3. Sınıfları analiz et → Read
4. Metotları analiz et → Read
5. Özeti oluştur → Markdown
6. Kalite kontrolü yap
7. Raporu kaydet
```

### 12.4 ADR Yazma Akışı

```
1. Vault oku → CLAUDE.md
2. Karar konusunu belirle
3. Bağlamı analiz et → brain.md
4. Karar seçeneklerini değerlendir
5. Nedenleri listele
6. Sonuçları belirle
7. ADR oluştur → Markdown
8. Onay için sun
```

---

## 13. Dokümantasyon En İyi Uygulamaları

### 13.1 Yazım İpuçları

| İpucu | Açıklama |
|-------|----------|
| Açık dil | Basit ve anlaşılır dil |
| Tutarlı format | Tutarlı format kullan |
| Kısa cümleler | Kısa ve öz cümleler |
| Madde işareti | Madde işareti kullan |
| Kod örneği | Kod örnekleri ekle |

### 13.2 Format İpuçları

| İpucu | Açıklama |
|-------|----------|
| Başlık hiyerarşisi | Doğru başlık sıralaması |
| Tablo kullanımı | Bilgilendirici tablolar |
| Kod bloğu | Doğru dil belirteci |
| Bağlantı | Çalışan bağlantılar |
| Görsel | Ekran görüntüleri |

### 13.3 Kalite İpuçları

| İpucu | Açıklama |
|-------|----------|
| Doğrulama | Bilgilerin doğruluğunu kontrol et |
| Güncelleme | Güncel bilgileri kullan |
| Kapsam | Tüm gerekli bilgileri dahil et |
| Erişilebilirlik | Kolay erişilebilir yap |

---

## 14. Dokümantasyon Entegrasyonu

### 14.1 Agent Entegrasyonları

| Agent | Entegrasyon | Akış |
|-------|-------------|------|
| MO → Summary | Doküman isteği | MO summary'a doküman atar |
| Summary → Build | Kod dokümanı | Summary build'a doküman iletir |
| Summary → Plan | Plan dokümanı | Summary plan'a doküman iletir |

### 14.2 Tool Entegrasyonları

| Tool | Kullanım |
|------|----------|
| Read | Dosya okuma |

---

## 15. Dokümantasyon Sınırlamaları

### 15.1 Yapamayacağı Şeyler

| Sınırlama | Açıklama |
|-----------|----------|
| Dosya yazma | Sadece okuma |
| Dosya düzenleme | Sadece okuma |
| Vault değiştirme | MO yapar |

### 15.2 Dikkat Edilecekler

| Konu | Açıklama |
|------|----------|
| Doğruluk | Bilgilerin doğru olduğundan emin ol |
| Kapsam | Tüm gerekli bilgileri dahil et |
| Tutarlılık | Tutarlı format kullan |

---

## 16. Dokümantasyon Gelecek Planı

### 16.1 Kısa Vadeli (1-2 hafta)

| Görev | Öncelik |
|-------|---------|
| Doküman şablonları | Yüksek |
| Kalite kontrol araçları | Yüksek |
| Format standartları | Orta |

### 16.2 Orta Vadeli (1-2 ay)

| Görev | Öncelik |
|-------|---------|
| Otomatik doküman üretimi | Orta |
| Çeviri desteği | Düşük |
| Görsel ekleme | Düşük |

### 16.3 Uzun Vadeli (3-6 ay)

| Görev | Öncelik |
|-------|---------|
| AI destekli yazma | Düşük |
| Otomatik güncelleme | Orta |
| Akıllı özetleme | Düşük |

---

## 17. Quality Report

| Metrik | Değer |
|--------|-------|
| Version | 1.2.0 |
| Status | Active |
| Task Types | 3 |
| Template Types | 4 |
| Quality Criteria | 5 |
| Workflow Examples | 4 |
| Best Practices | 12 |
| Integration Points | 3 |

---

**Authority:** Vault Steward
**Last Updated:** 2026-08-26
**Mode:** Red Team · Human Mode · Truth Mode
