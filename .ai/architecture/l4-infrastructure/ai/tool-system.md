---
title: "Versa Coder — Tool System Mimarisi"
type: architecture
category: ai
date: 2026-08-25
updated: 2026-08-25
status: active
version: 1.0.0
---

# Versa Coder — Tool System Mimarisi

**Zorunlu Bağlantılar:** [[architecture/l4-infrastructure/ai/agent-runner]] · [[architecture/l4-infrastructure/ai/provider-router]] · [[brain.md]]

---

## 1. Amaç

Versa Coder'daki **45+ aracın** tanımını, kayıt mekanizmasını, permission sistemini ve çalışma şeklini tanımlayan tool sistemi mimarisi.

---

## 2. Tool Kategorileri

### 2.1 Dosya Araçları

| Tool | Tanım | Permission |
|------|-------|------------|
| `read` | Dosya oku | allow |
| `write` | Dosya yaz | ask |
| `edit` | Dosya düzenle | ask |
| `glob` | Dosya pattern ara | allow |
| `grep` | İçerik ara | allow |

### 2.2 Terminal Araçları

| Tool | Tanım | Permission |
|------|-------|------------|
| `bash` | Shell komutu çalıştır | ask |
| `powershell` | PowerShell komutu | ask |

### 2.3 Git Araçları

| Tool | Tanım | Permission |
|------|-------|------------|
| `git_status` | Git durumu | allow |
| `git_diff` | Değişiklikleri göster | allow |
| `git_commit` | Commit oluştur | ask |
| `git_push` | Push yap | ask |
| `git_pull` | Pull yap | ask |

### 2.4 AI Araçları

| Tool | Tanım | Permission |
|------|-------|------------|
| `llm_query` | LLM sorgusu | allow |
| `embedding` | Embedding oluştur | allow |

### 2.5 MCP Araçları

| Tool | Tanım | Permission |
|------|-------|------------|
| `mcp_resource_read` | MCP kaynağı oku | allow |
| `mcp_tool_call` | MCP tool çağır | allow |

### 2.6 Proje Araçları

| Tool | Tanım | Permission |
|------|-------|------------|
| `project_index` | Proje indeksi | allow |
| `project_analyze` | Proje analizi | allow |
| `diagram_create` | Diyagram oluştur | ask |

### 2.7 Session Araçları

| Tool | Tanım | Permission |
|------|-------|------------|
| `session_save` | Oturumu kaydet | allow |
| `session_load` | Oturum yükle | allow |
| `session_branch` | Dal oluştur | allow |
| `session_fork` | Çatal oluştur | allow |

### 2.8 Context Araçları

| Tool | Tanım | Permission |
|------|-------|------------|
| `context_assemble` | Context birleştir | allow |
| `context_update` | Context güncelle | allow |
| `context_validate` | Context doğrula | allow |

---

## 3. Tool Registry

```csharp
public class ToolRegistry
{
    private readonly Dictionary<string, ToolDefinition> _tools = new();

    public void Register(ToolDefinition tool)
    {
        _tools[tool.Name] = tool;
    }

    public ToolDefinition? GetTool(string name)
    {
        return _tools.TryGetValue(name, out var tool) ? tool : null;
    }

    public IReadOnlyList<ToolDefinition> GetToolsForAgent(AgentRole role)
    {
        return _tools.Values
            .Where(t => t.IsAllowedForAgent(role))
            .ToList();
    }
}
```

---

## 4. Permission Sistemi

| Action | Tanım | Varsayılan |
|--------|-------|------------|
| `allow` | Otomatik izin ver | Dosya okuma, glob, grep |
| `ask` | Kullanıcıya sor | Dosya yazma, terminal, git |
| `deny` | Reddet | Yasak işlemler |

---

## 5. Tool Tanım Formatı

```csharp
public class ToolDefinition
{
    public string Name { get; set; }           // "read_file"
    public string Description { get; set; }    // "Dosya içeriğini okur"
    public JsonSchema InputSchema { get; set; } // Input parametreleri
    public Func<JsonElement, Task<ToolResult>> Execute { get; set; }
    public PermissionLevel Permission { get; set; }
    public AgentRole[] AllowedAgents { get; set; }
}
```

---

## 6. OpenCode Eşleştirme

| VersaCoder Tool | OpenCode Karşılığı |
|-----------------|-------------------|
| read | `packages/core/src/tool/builtins/read.ts` |
| write | `packages/core/src/tool/builtins/write.ts` |
| edit | `packages/core/src/tool/builtins/edit.ts` |
| glob | `packages/core/src/tool/builtins/glob.ts` |
| grep | `packages/core/src/tool/builtins/grep.ts` |
| bash | `packages/core/src/tool/builtins/bash.ts` |
| task | `packages/opencode/src/tool/task.ts` |
| question | `packages/core/src/tool/builtins/question.ts` |

---

**Authority:** Vault Steward
**Last Updated:** 2026-08-25
