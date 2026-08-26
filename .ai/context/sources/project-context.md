---
title: "Versa Coder — Proje Bağlamı"
type: context
category: sources
date: 2026-08-25
updated: 2026-08-25
status: active
version: 1.0.0
---

# Versa Coder — Proje Bağlamı

**Zorunlu Bağlantılar:** [[CLAUDE.md]] · [[architecture/00-overview/architecture-master]]

---

## 1. Proje Tanımı

| Özellik | Değer |
|---------|-------|
| Proje Adı | Versa Coder |
| Tür | AI Destekli Kod Geliştirme Platformu (IDE) |
| Teknoloji | C# .NET 8, DevExpress 2026 Universal |
| Database | SQLite (WAL mode) |
| AI | Multi-provider LLM |
| Architecture | 30 katmanlı Clean Architecture |

---

## 2. Dosya Yapısı

```
versacoder/
├── .ai/                    — Vault sistemi
│   ├── CLAUDE.md          — AI anayasası
│   ├── AGENTS.md          — Agent tanımları
│   ├── WORKFLOW.md        — Süreçler
│   ├── brain.md           — Mimari kararlar
│   ├── architecture/      — Mimari dokümanlar
│   ├── .agents/           — Agent profilleri
│   ├── context/           — Context yönetimi
│   ├── learning/          — Öğrenme sistemi
│   ├── rules/             — Kurallar
│   └── skills/            — Skill tanımları
├── src/
│   ├── VersaCoder.Domain/           — L0 Domain
│   ├── VersaCoder.Abstractions/     — L1 Abstractions
│   ├── VersaCoder.Application/      — L2 Application
│   ├── VersaCoder.CrossCutting/     — L3 CrossCutting
│   ├── VersaCoder.Infrastructure.*/ — L4-L30 Infrastructure
│   ├── VersaCoder.Protocol/         — L5 Protocol
│   ├── VersaCoder.Host/             — L6 Host
│   └── VersaCoder.UI/               — L7 UI
├── tests/
│   ├── VersaCoder.Domain.Tests/
│   ├── VersaCoder.Application.Tests/
│   └── VersaCoder.Infrastructure.Tests/
└── VersaCoder.slnx
```

---

## 3. Bağımlılıklar

| Paket | Versiyon | Amaç |
|-------|----------|------|
| DevExpress.Win.Design | 24.2.3 | UI |
| MediatR | 12.4.1 | CQRS |
| AutoMapper | 13.0.1 | Mapping |
| FluentValidation | 11.11.0 | Validation |
| Serilog | 4.1.0 | Logging |
| EF Core | 8.0.11 | ORM |
| CommunityToolkit.Mvvm | 8.4.0 | MVVM |
| Markdig | Latest | Markdown |
| xUnit | Latest | Testing |

---

**Authority:** Vault Steward
**Last Updated:** 2026-08-25
