---
title: OpenCode
type: arac
created: 2026-10-06
updated: 2026-10-06
sources:
  - raw/versacoder-proje-planlamasi-ai-04.md
  - raw/versacoder-proje-planlamasi-ai-05.md
  - raw/versacoder-proje-planlamasi-ai-07.md
  - raw/versacoder-proje-planlamasi-ai-08.md
  - raw/coremusic-vault-template.md
tags: [opencode, cli, agent, zen-free]
---

# OpenCode

**Rol:** Agentic coding CLI/agent'ı. Versa Coder'da **Infrastructure adapter**'ıdır (`VersaCoder.ExternalAgents.OpenCode`), execution plane değil control plane Versa Coder'dır (ai-08, ai-05).

**Zen free model uç noktası (ai-04):**
- Endpoint: `https://opencode.ai/zen/v1/chat/completions` (`@ai-sdk/openai-compatible`)
- Canlı liste: `https://opencode.ai/zen/v1/models`; katalog senkronu: `https://models.opencode.ai/api.json`
- Başlıklar: `HTTP-Referer`, `X-Title`, `X-Source`, `X-BILLING-INVOKE-ORIGIN`, User-Agent `opencode/1.0.0/client`
- 7 free model: deepseek-v4-flash-free, mimo-v2.5-free, hy3-free, laguna-s-2.1-free, nemotron-3-ultra-free, nemotron-3.5-lightning-free, big-pickle

**Yapılandırma yolu:** `C:\Users\Bayram Ali\.config\opencode` (ai-07).

**Çekirdek config (coremusic-vault-template):** `default_agent = master-orchestrator`, 16 komut, ~85 `skills.force`, diskte 8 SKILL.md, 4 MCP (chrome-devtools, browsermcp, playwright, filesystem).

**Uyarı:** OpenCodeExecutor kavramsal adapter'dır; gerçek CLI syntax doğrulanmadan yazılmaz → `[VERIFY REQUIRED]` (ai-05 §8).

**Hedef ekran haritası:** TUI'da referans TabView, markdown akış, editör paneli, model satırı, durum satırı (ai-09 §1).

## İlgili Sayfalar
- [[multi-llm-bagimsizlik]] — fallback zincirindeki yeri
- [[ucretsiz-model-havuzu]] — Zen free modelleri
- [[agent-skills-ekosistemi]] — skill ekosistemi
- [[mcp-sunuculari]] — kullandığı MCP sunucuları
- [[terminal-tui-kabugu]] — ekran haritası referansı
- [[referans-proje-analizi]] — analiz edilen referans
- [[chatgpt]] — bağımsız çalışan alternatif model ailesi
