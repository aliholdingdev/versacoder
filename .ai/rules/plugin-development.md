---
title: "Versa Coder — Plugin Geliştirme Rehberi"
type: rules
category: plugin
date: 2026-08-25
updated: 2026-08-25
status: active
version: 1.0.0
---

# Versa Coder — Plugin Geliştirme Rehberi

---

## 1. Plugin Mimarisi

```
┌──────────────────────────────────────┐
│           PLUGIN HOST                │
│  ┌────────────────────────────────┐  │
│  │  Plugin Loader (Assembly.Load) │  │
│  └────────────────────────────────┘  │
│           ↓                          │
│  ┌────────────────────────────────┐  │
│  │  IPlugin Interface             │  │
│  │  ├── Initialize()              │  │
│  │  ├── GetTools()                │  │
│  │  ├── GetCommands()             │  │
│  │  └── Dispose()                 │  │
│  └────────────────────────────────┘  │
│           ↓                          │
│  ┌────────────────────────────────┐  │
│  │  Plugin Assembly (.dll)        │  │
│  └────────────────────────────────┘  │
└──────────────────────────────────────┘
```

---

## 2. Plugin Arayüzü

```csharp
public interface IPlugin : IDisposable
{
    string Name { get; }
    string Version { get; }
    string Description { get; }
    void Initialize(IServiceProvider serviceProvider);
    IReadOnlyList<ToolDefinition> GetTools();
    IReadOnlyList<CommandDefinition> GetCommands();
}
```

---

## 3. Plugin Dağıtımı

| Konum | Açıklama |
|-------|----------|
| `plugins/` | Yerel plugin dizini |
| NuGet | Paket yöneticisi |
| Custom | Özel plugin kaynağı |

---

**Authority:** Vault Steward
**Last Updated:** 2026-08-25
