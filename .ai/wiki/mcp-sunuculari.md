---
title: MCP Sunucuları
type: arac
created: 2026-10-06
updated: 2026-10-06
sources:
  - raw/versacoder-proje-planlamasi-ai-07.md
  - raw/versacoder-proje-planlamasi-ai-06.md
  - raw/coremusic-vault-template.md
tags: [mcp, protocol, entegrasyon, sunucu]
---

# MCP Sunucuları

**Kaynaklar:** resmi `modelcontextprotocol/servers` + `mctrinh/awesome-mcp-servers` (versacoder-proje-planlamasi-ai-07).

**Taşıma protokolleri:** MCP Gateway **stdio** ve **SSE** ile çalışır (ai-06 §5.4).

**Plugin izolasyonu:** Plugin'ler `AssemblyLoadContext` içinde izole edilir; bir plugin çökse bile ana IDE etkilenmez (ai-06 §5.4).

**OpenCode config'inde tanımlı 4 MCP:** chrome-devtools · browsermcp · playwright · filesystem (coremusic-vault-template).

**Versa Coder mimarisindeki yeri:** `Protocol-MCP` katmanı 6 katmanlı platform anatomisinin bir parçasıdır; tool çağrıları **Policy Engine**'den geçer, izinsiz dış erişim yasaktır (ai-02).

## İlgili Sayfalar
- [[opencode]] — MCP istemcisi olarak araç
- [[agent-skills-ekosistemi]] — birlikte incelenen ekosistem
- [[versacoder-projesi]] — Protocol-MCP katmanı
- [[agent-workflow]] — policy kontrolünden geçen tool çağrıları
