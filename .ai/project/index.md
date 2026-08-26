---
title: "Versa Coder — Project Index"
type: project-index
date: 2026-08-25
version: 1.0.0
---

# Versa Coder — Project Index

**Zorunlu Bağlantılar:** [[CLAUDE.md]] · [[brain.md]]

---

## 1. Solution Yapısı

```
VersaCoder.sln
├── src/
│   ├── VersaCoder.Domain/           # L0
│   ├── VersaCoder.Abstractions/     # L1
│   ├── VersaCoder.Application/      # L2
│   ├── VersaCoder.CrossCutting/     # L3
│   ├── VersaCoder.Infrastructure.*  # L4 (27 modül)
│   ├── VersaCoder.Protocol/         # L5
│   ├── VersaCoder.Host/             # L6
│   └── VersaCoder.UI/               # L7
└── tests/
    ├── VersaCoder.Domain.Tests/
    ├── VersaCoder.Application.Tests/
    └── VersaCoder.Infrastructure.Tests/
```

## 2. NuGet Paketleri

| Paket | Kullanım |
|-------|----------|
| DevExpress.Win.Design | WinForms UI |
| CommunityToolkit.Mvvm | MVVM |
| MediatR | CQRS |
| AutoMapper | Mapping |
| FluentValidation | Validation |
| Serilog | Logging |
| EF Core Sqlite | SQLite ORM |
| LibGit2Sharp | Git |
| Markdig | Markdown |
| Polly | Resilience |

## 3. Mimari Katmanlar

| Katman | Modül | Sorumluluk |
|--------|-------|------------|
| L0 | Domain | Varlıklar, değer objeleri, domain event |
| L1 | Abstractions | Arayüzler, kontratlar |
| L2 | Application | Use case, DTO, handler |
| L3 | CrossCutting | Logging, exception, validation |
| L4 | Infrastructure | 27 altyapı modülü |
| L5 | Protocol | AI protokolü, MCP |
| L6 | Host | Uygulama başlangıcı, DI |
| L7 | UI | DevExpress WinForms |

## 4. L4 Infrastructure Modülleri

| # | Modül | Amaç |
|---|-------|------|
| 1 | Infrastructure.Data | SQLite, EF Core, repository |
| 2 | Infrastructure.AI | LLM provider, agent runner |
| 3 | Infrastructure.MCP | MCP client/server |
| 4 | Infrastructure.Auth | API key, credential yönetimi |
| 5 | Infrastructure.Config | Uygulama ayarları |
| 6 | Infrastructure.Plugins | Plugin sistemi |
| 7 | Infrastructure.Services | Yardımcı servisler |
| 8 | Infrastructure.Caching | Önbellek yönetimi |
| 9 | Infrastructure.Messaging | Event bus, messaging |
| 10 | Infrastructure.FileSystem | Dosya sistemi |
| 11 | Infrastructure.Network | HTTP, WebSocket |
| 12 | Infrastructure.Security | Şifreleme, token |
| 13 | Infrastructure.Observability | Monitoring, metrics |
| 14 | Infrastructure.Context | Context assembly |
| 15 | Infrastructure.Learning | Pattern, düzeltme |
| 16 | Infrastructure.Diagram | Diyagram okuma |
| 17 | Infrastructure.ProjectAnalysis | Proje indeksleme |
| 18 | Infrastructure.Testing | Test altyapısı |
| 19 | Infrastructure.Documentation | Otomatik doc |
| 20 | Infrastructure.Refactoring | Refactoring araçları |
| 21 | Infrastructure.CodeAnalysis | Kod analizi |
| 22 | Infrastructure.Git | Git entegrasyonu |
| 23 | Infrastructure.Integration | Üçüncü parti |
| 24 | Infrastructure.Templating | Şablon sistemi |
| 25 | Infrastructure.Deployment | Dağıtım araçları |
| 26 | Infrastructure.Backup | Yedekleme |
| 27 | Infrastructure.Versioning | Versiyon yönetimi |

## 5. Uygulama Sırası

| Sıra | Katman | Tahmini Süre |
|------|--------|-------------|
| 1 | L0 Domain | 1 hafta |
| 2 | L1 Abstractions | 1 hafta |
| 3 | L3 CrossCutting | 1 hafta |
| 4 | L2 Application | 2 hafta |
| 5 | L4 Data + Config | 2.5 hafta |
| 6 | L4 AI + MCP | 5 hafta |
| 7 | L4 Diğer modüller | 5 hafta |
| 8 | L5 Protocol | 1 hafta |
| 9 | L6 Host | 1 hafta |
| 10 | L7 UI | 4 hafta |
| Toplam | — | ~20 hafta |

---

**Authority:** Vault Steward
**Last Updated:** 2026-08-25
