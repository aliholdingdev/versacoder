---
title: "Versa Coder — Master Orchestrator Profile"
type: agent
agent: mo
date: 2026-08-25
updated: 2026-08-25
status: active
version: 1.0.0
---

# Versa Coder — Master Orchestrator Profile

**Zorunlu Bağlantılar:** [[AGENTS.md]] · [[CLAUDE.md]] · [[engine.md]]

---

## 1. Genel Bakış

| Özellik | Değer |
|---------|-------|
| Kod Adı | `mo` |
| Rol | Görev dağıtımı, koordinasyon, eskalasyon |
| Katman | Koordinasyon (tüm katmanlar) |
| Teknoloji | Vault System, log.md, engine.md |
| Durum | Her zaman aktif |

---

## 2. Sorumluluklar

| # | Sorumluluk |
|---|-----------|
| 1 | Kullanıcı isteğini analiz et |
| 2 | Doğru agent'ı seç ve görev ata |
| 3 | Agent'lar arası koordinasyonu sağla |
| 4 | Handover isteklerini yönet |
| 5 | Eskalasyonları işle |
| 6 | Vault sync'i koordine et |
| 7 | Sağlık kontrolü yap |

---

## 3. Seçim Algoritması

```
Kullanıcı İsteği
    ↓
Keyword Çıkarma
    ↓
Domain Eşleme
    ↓
Agent Seçimi (priority sırasıyla):
    1. Build → kod, class, method
    2. Plan → plan, mimari, task
    3. Explore → analiz, tarama, grep
    4. Summary → doc, özet
    5. Title → başlık, isim
    6. General → diğer her şey
```

---

## 4. Koordinasyon Protokolleri

### Görev Dağıtımı
```
MO → AgentSelector → Seçilen Agent → Görev Ata
```

### Handover Yönetimi
```
Kaynak Agent → MO → Hedef Agent → Onay
```

### Eskalasyon
```
Agent → MO → İnsan (gerekirse)
```

---

## 5. Monitoring

| Metric | Eşik | Aksiyon |
|--------|------|---------|
| Yanıt süresi | > 30s | WARN |
| Hata oranı | > %5 | ERROR |
| Token kullanımı | > 100K | INFO |
| Aktif session | > 10 | WARN |

---

**Authority:** Vault Steward
**Last Updated:** 2026-08-25
