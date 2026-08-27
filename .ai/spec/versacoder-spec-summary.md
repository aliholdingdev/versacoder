---
title: "Versa Coder — ProjeSpec Özeti"
type: summary
category: project-spec-summary
date: 2026-08-26
updated: 2026-08-26
status: active
version: 1.0.0
authority: Single Source of Truth (SSOT)
---

# Versa Coder — ProjeSpec Özeti

**Detaylı şartname:** [[versacoder-spec]]

---

## 1. Proje Özeti

Versa Coder, C# .NET 8.0 ile yazılmış yapay zeka destekli bir IDE platformudur. Tamamen self-contained bir generatif-AI ve agentic framework'tür.

### 1.1 Temel Özellikler

| Özellik | Açıklama |
|---------|----------|
| Teknoloji | .NET 8.0, C# 12.0 |
| UI | DevExpress WinForms + MDI + Ribbon |
| ORM | Entity Framework Core (DbContext ONLY) |
| DB | SQLite WAL modu |
| AI | Çoklu LLM (OpenAI, Anthropic, Google, Ollama) |
| Protocol | Model Context Protocol (MCP) |
| Code Analysis | Roslyn, AST |
| Git | LibGit2Sharp |
| Loglama | Serilog |
| Test | xUnit |

### 1.2 Proje Boyutu

| Metrik | Değer |
|--------|-------|
| Toplam Proje | 36 |
| Çalışan Proje | 9 (~6,850 satır) |
| Stub Proje | 26 |
| UI Projesi | 1 (boş) |
| Test Projesi | 0 |

---

## 2. Mimari Özet

### 2.1 Katmanlar (L0-L7)

| Katman | Ad | Durum |
|--------|-----|-------|
| L0 | Domain | ✅ Çalışıyor (~800 satır) |
| L1 | Abstractions | ✅ Çalışıyor (~600 satır) |
| L2 | Application | ✅ Çalışıyor (~2500 satır) |
| L3 | CrossCutting | ✅ Çalışıyor (~200 satır) |
| L4 | Infrastructure | ✅ Çalışıyor (~2585 satır) |
| L5 | Protocol | ❌ Boş stub |
| L6 | Host | ✅ Çalışıyor (~65 satır) |
| L7 | UI | ❌ Boş form |

### 2.2 Bağımlılık Zinciri

```
L7 → L6 → L5 → L4 → L3 → L2 → L1 → L0
```

---

## 3. Agent Sistemi Özeti

| # | Agent | Görev | Durum |
|---|-------|-------|-------|
| 1 | Master Orchestrator | Koordinasyon | ✅ Aktif |
| 2 | Build Agent | Kod üretimi | ✅ Aktif |
| 3 | Plan Agent | Planlama | ✅ Aktif |
| 4 | Explore Agent | Analiz | ✅ Aktif |
| 5 | General Agent | Genel | ✅ Aktif |
| 6 | Summary Agent | Doküman | ✅ Aktif |
| 7 | Title Agent | İsimlendirme | ✅ Aktif |
| 8 | Resilience Agent | Dayanıklılık | 🔄 V11.0 |
| 9 | Human Agent | İnsan etkileşimi | 🔄 V11.0 |

---

## 4. Kritik Eksikler

| # | Eksik | Öncelik | Tahmini |
|---|-------|---------|---------|
| 1 | UI katmanı (DevExpress + MDI + Ribbon) | YÜKSEK | 3-4 hafta |
| 2 | MCP protokolü | YÜKSEK | 2-3 hafta |
| 3 | Context yönetimi | YÜKSEK | 1-2 hafta |
| 4 | Git entegrasyonu | YÜKSEK | 1-2 hafta |
| 5 | Configuration sistemi | YÜKSEK | 1 hafta |
| 6 | FileSystem servisleri | YÜKSEK | 1 hafta |

---

## 5. Yol Haritası Özeti

| Aşama | Kapsam | Süre |
|-------|--------|------|
| FAZ 1 | Altyapı servisleri | 2-3 hafta |
| FAZ 2 | UI katmanı | 3-4 hafta |
| FAZ 3 | Protokol & Entegrasyon | 2-3 hafta |
| FAZ 4 | Ek modüller | 2-3 hafta |
| FAZ 5 | Test & Optimizasyon | 1-2 hafta |

**Toplam Tahmini:** 10-15 hafta

---

## 6. Guardrails Özeti

| # | Kural |
|---|-------|
| 1 | Kod yazmadan önce plan yap |
| 2 | Vault'tan bilgi almadan kodlama yapma |
| 3 | Uydurma bilgi kullanma |
| 4 | Dosyaları yerinde değiştir |
| 5 | Tek Doğruluk Kaynağı kullan |
| 6 | Şablon kullanımı zorunlu |
| 7 | Session sürekliliği sağla |
| 8 | İnsan onayı al |
| 9 | Bağlam toplama önce yap |
| 10 | Öğrenme aktif tut |
| 11 | Diagram öğretme yap |
| 12 | Çelişki kapısı oluştur |
| 13 | EF Core DbContext ONLY |
| 14 | WinForms code-behind kullanma |
| 15 | DevExpress kullanımı zorunlu |
| 16 | SQLite WAL modu kullan |

---

**Authority:** Vault Steward
**Last Updated:** 2026-08-26
**Mode:** Red Team · Human Mode · Truth Mode
