---
title: "Versa Coder — L5 Protocol Layer Guide"
type: architecture
category: layer
layer: L5
date: 2026-08-25
updated: 2026-08-25
status: active
version: 1.0.0
---

# Versa Coder — L5 Protocol Layer Guide

**Zorunlu Bağlantılar:** [[architecture/l4-infrastructure/infrastructure-guide]] · [[brain.md]]

---

## 1. Amaç

Protocol katmanı, **AI protokolü, MCP (Model Context Protocol) ve Provider iletişimi** entremanlarını yönetir.

---

## 2. MCP (Model Context Protocol)

| Bileşen | Tanım |
|---------|-------|
| MCP Client | Dış MCP sunucularına bağlanır |
| MCP Server | VersaCoder'ı MCP kaynağı olarak sunar |
| MCP Resources | Dosya, database, schema kaynakları |
| MCP Tools | Dış tool'ları entegre eder |

---

## 3. Protokol Desteği

| Protokol | Kullanım | Durum |
|----------|----------|-------|
| HTTP/REST | Provider iletişimi | ✅ |
| SSE (Server-Sent Events) | Streaming yanıtlar | ✅ |
| WebSocket | Gerçek zamanlı iletişim | 🔄 Planlanan |
| gRPC | Yüksek performanslı iletişim | 🔄 Planlanan |
| SignalR | Real-time push | 🔄 Planlanan |

---

## 4. Kurallar

| # | Kural |
|---|-------|
| 1 | Protocol → Infrastructure ✅ |
| 2 | Protocol → Application ❌ |
| 3 | Tüm provider iletişimi bu katmandan geçer |
| 4 | MCP standardına uygunluk |

---

**Authority:** Vault Steward
**Last Updated:** 2026-08-25
