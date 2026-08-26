---
title: "Versa Coder — Güvenlik Mimarisi"
type: rules
category: security
date: 2026-08-25
updated: 2026-08-25
status: active
version: 1.0.0
---

# Versa Coder — Güvenlik Mimarisi

**Zorunlu Bağlantılar:** [[CLAUDE.md]] · [[brain.md]]

---

## 1. Güvenlik İlkeleri

| # | İlke | Açıklama |
|---|------|----------|
| 1 |最小Yetki | Minimum privilege principle |
| 2 | Derinlemesine Savunma | Çoklu güvenlik katmanı |
| 3 | Güvenli Varsayılanlar | Varsayılan olarak güvenli yapılandırma |
| 4 | Açık Tasarım | Güvenlik gizli olmamalı |
| 5 | Hata Güvenliği | Hatalar bilgi sızdırmamalı |

---

## 2. API Key Yönetimi

| Kural | Açıklama |
|-------|----------|
| Hardcoded key yasak | IConfiguration + .env |
| Key rotasyonu | Periyodik key değişimi |
| Key erosion | Üretimde key erosion |
| Logging yasak | Key'ler loglanmaz |

---

## 3. Veri Güvenliği

| Veri Türü | Koruma |
|-----------|--------|
| API Key'leri | Şifrelenmiş saklama |
| Session verileri | SQLite şifreleme |
| Kullanıcı girdisi | Input validation |
| Dosya yolları | Path traversal koruması |

---

## 4. OWASP Kontrolleri

| Kontrol | Durum |
|---------|-------|
| Input Validation | ✅ FluentValidation |
| Output Encoding | ✅ Markdig sanitization |
| Authentication | 🔄 Planlanan |
| Authorization | 🔄 Planlanan |
| Error Handling | ✅ GlobalExceptionHandler |
| Logging | ✅ Serilog |

---

**Authority:** Vault Steward
**Last Updated:** 2026-08-25
