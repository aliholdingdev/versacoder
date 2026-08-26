---
title: "Versa Coder — Master Architecture Overview"
type: architecture
category: overview
date: 2026-08-25
updated: 2026-08-25
status: active
version: 1.0.0
---

# Versa Coder — Master Architecture Overview

**Zorunlu Bağlantılar:** [[CLAUDE.md]] · [[brain.md]] · [[index.md]]

---

## 1. Amaç

Versa Coder platformunun **30 katmanlı mimarisinin** genel tanımı, katman bağımlılık kuralları ve her katman arasındaki ilişkilerin detaylı açıklaması.

---

## 2. Mimari Genel Bakış

```
┌─────────────────────────────────────────────────────────────┐
│  L7  UI              DevExpress WinForms, Ribbon, Tabbed MDI│
├─────────────────────────────────────────────────────────────┤
│  L6  Host            Uygulama başlangıcı, DI, Konfigürasyon│
├─────────────────────────────────────────────────────────────┤
│  L5  Protocol        AI protokolü, MCP, Provider İletişimi  │
├─────────────────────────────────────────────────────────────┤
│  L4  Infrastructure  28 Altyapı Modülü                      │
│  ┌──────────────────────────────────────────────────────┐  │
│  │ Data │ AI │ MCP │ Auth │ Config │ Plugins │ Services │  │
│  │ Caching │ Messaging │ FileSystem │ Network │ Security  │  │
│  │ Observability │ Context │ Learning │ Diagram │ P.Anal. │  │
│  │ Testing │ Docs │ Refactor │ CodeAnal. │ Git │ Integ.  │  │
│  │ Templating │ Deploy │ Backup │ Versioning             │  │
│  └──────────────────────────────────────────────────────┘  │
├─────────────────────────────────────────────────────────────┤
│  L3  CrossCutting    Logging, Exception, Validation         │
├─────────────────────────────────────────────────────────────┤
│  L2  Application     Use Case'ler, DTO'lar, Servisler       │
├─────────────────────────────────────────────────────────────┤
│  L1  Abstractions    Arayüzler, Kontratlar                  │
├─────────────────────────────────────────────────────────────┤
│  L0  Domain          Varlıklar, Değer Obje, Domain Event    │
└─────────────────────────────────────────────────────────────┘
```

---

## 3. Katman Bağımlılık Matrisi

| Kaynak → Hedef | İzinli mi? | Açıklama |
|-----------------|------------|----------|
| L7 → L6 | ✅ Evet | UI, Host'a bağımlı |
| L6 → L5 | ✅ Evet | Host, Protocol'e bağımlı |
| L5 → L4 | ✅ Evet | Protocol, Infrastructure'a bağımlı |
| L4 → L3 | ✅ Evet | Infrastructure, CrossCutting'e bağımlı |
| L3 → L2 | ✅ Evet | CrossCutting, Application'a bağımlı |
| L2 → L1 | ✅ Evet | Application, Abstractions'a bağımlı |
| L1 → L0 | ✅ Evet | Abstractions, Domain'e bağımlı |
| L0 → L2/L3 | ❌ HAYIR | Layer Violation |
| L1 → L3 | ❌ HAYIR | Layer Violation |
| L3 → L0 | ❌ HAYIR | Layer Violation |

---

## 4. Teknoloji Haritası

| Katman | Ana Teknoloji | Paket | Versiyon |
|--------|---------------|-------|----------|
| L7 | DevExpress WinForms | DevExpress.Win.Design | 24.2.3 |
| L6 | .NET Host | Microsoft.Extensions.Hosting | 8.0 |
| L5 | HTTP/gRPC | System.Net.Http | 8.0 |
| L4 | EF Core | Microsoft.EntityFrameworkCore | 8.0.11 |
| L3 | MediatR | MediatR | 12.4.1 |
| L2 | CQRS | MediatR | 12.4.1 |
| L1 | Interfaces | — | — |
| L0 | DDD | — | — |

---

## 5. Çapraz Referanslar

| Bölüm | Hedef |
|-------|-------|
| Domain detayları | [[architecture/l0-domain/domain-guide]] |
| Abstractions detayları | [[architecture/l1-abstractions/abstractions-guide]] |
| Application detayları | [[architecture/l2-application/application-guide]] |
| CrossCutting detayları | [[architecture/l3-crosscutting/crosscutting-guide]] |
| Infrastructure detayları | [[architecture/l4-infrastructure/infrastructure-guide]] |
| Protocol detayları | [[architecture/l5-protocol/protocol-guide]] |
| Host detayları | [[architecture/l6-host/host-guide]] |
| UI detayları | [[architecture/l7-ui/ui-guide]] |

---

**Authority:** Vault Steward
**Last Updated:** 2026-08-25
