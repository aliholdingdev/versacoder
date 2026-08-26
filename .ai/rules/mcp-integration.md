---
title: "Versa Coder — MCP Entegrasyonu"
type: rules
category: mcp
date: 2026-08-25
updated: 2026-08-25
status: active
version: 1.0.0
---

# Versa Coder — MCP Entegrasyonu

---

## 1. MCP Nedir?

Model Context Protocol (MCP), AI modellerinin **dış kaynaklara ve tool'lara** erişimini standartlaştıran protokoldür.

---

## 2. MCP Mimarisi

```
VersaCoder (MCP Client)
    ↓
MCP Server (dış servis)
    ↓
Resources (dosya, database, schema)
Tools (dış tool'ar)
Prompts (dış prompt'lar)
```

---

## 3. MCP Kullanım Alanları

| Alan | Kullanım |
|------|----------|
| Dosya sistemi | Dış dosya okuma/yazma |
| Veritabanı | Dış DB sorgulama |
| API | Dış API çağrısı |
| Tool | Dış tool entegrasyonu |
| Knowledge | Dış bilgi kaynakları |

---

## 4. VersaCoder MCP Rolleri

| Rol | Tanım |
|-----|-------|
| MCP Client | Dış MCP sunucularına bağlanır |
| MCP Server | VersaCoder'ı MCP kaynağı olarak sunar |

---

**Authority:** Vault Steward
**Last Updated:** 2026-08-25
