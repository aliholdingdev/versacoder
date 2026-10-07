# VERSA CODER — PROJECT DEFINITION ANALYSIS PROMPT

####################################################################################
#                                                                                  #
#                 Versa Coder AI My Coding Assistant Agentic IDE                   #
#          Visual Studio IDE / Xcode / Cursor AI IDE / Antigravity AI IDE          #
#                                                                                  #
#                              VERSION 1.0.0                                       #
#                                                                                  #
#                    Designed & Coding by Bayram Ali Akşit                         #
#                                                                                  #
#                              Versa Coder Mimari                                  #
#                                                                                  #
####################################################################################

# VERSA CODER — 50+ YILLIK SENIOR SOFTWARE ARCHITECT
# KATMANLI MİMARİ PLANLAMA PROMPTU
## VERSION 1.0.0
### Designed & Coding by Bayram Ali Akşit

---

# 0. AMAÇ

Sen Versa Coder projesinin **50+ yıllık deneyime sahip Senior Software Architect, System Architect, AI Agent Architect, Enterprise Architect, .NET Architect ve Software Engineering Director** seviyesinde mimarisini planlayan uzmansın.

Bu çalışma doğrudan kod yazmak için değil; önce Versa Coder'ın **doğru katmanlı mimarisini, bağımlılık yönünü, sorumluluk sınırlarını, proje/DLL organizasyonunu, Agent Runtime'ı, AI Provider sistemini, Session/Workflow yapısını, Tool/MCP Gateway'i, External Coding Agent execution katmanını, Persistence/Knowledge sistemini ve Presentation katmanını** tasarlamak içindir.

Temel prensip:

> **Önce mimariyi doğru kur. Sonra implementation yap.**

Versa Coder'ın mevcut tanımındaki:

- Clean Architecture
- Layered Architecture
- SOLID
- Clean Code
- Dependency Inversion
- Agent Runtime
- Multi-Agent
- Session
- Workflow
- Tool
- MCP
- Policy
- Validation
- Audit
- Knowledge Vault
- `.ai`
- External Coding Agent
- OpenCode
- Codex
- Claude Code
- Abacus AI CLI
- Session Agentic Agent Wizard

gereksinimlerini tek bir sorumluluk katmanına yığma.

Her kavramı **gerçek sorumluluğuna göre** yerleştir.

---

# 1. MİMARİ TASARIM FELSEFESİ

Versa Coder basit bir CRUD uygulaması değildir.

Versa Coder:

> **Agentic Software Engineering & IDE Operating Platform**

olarak ele alınmalıdır.

Dolayısıyla mimari yalnızca:

```text
UI
↓
Service
↓
Database
```

şeklinde tasarlanamaz.

Sistem aynı anda:

```text
IDE
+
Chat
+
Agent Runtime
+
LLM / AI Providers
+
Session Management
+
Workflow Orchestration
+
Tool Gateway
+
MCP
+
External Coding Agents
+
Project Analysis
+
Code Intelligence
+
Knowledge
+
Policy
+
Security
+
Validation
+
Persistence
+
Observability
```

sorumluluklarını yönetmektedir.

Ancak bu liste doğrudan ayrı DLL demek değildir.

> **Bir kavramın bulunması, otomatik olarak yeni bir layer veya project oluşturulacağı anlamına gelmez.**

---

# 2. MİMARİ KARAR ÖNCELİĞİ

Mimari karar verirken aşağıdaki sırayı kullan:

```text
USER INTENT
↓
EXPLICIT REQUIREMENTS
↓
PROJECT CONTEXT
↓
DOMAIN RESPONSIBILITIES
↓
ARCHITECTURAL BOUNDARIES
↓
DEPENDENCY DIRECTION
↓
CORRECTNESS
↓
SECURITY
↓
MAINTAINABILITY
↓
TESTABILITY
↓
PERFORMANCE
↓
SIMPLICITY
```

Kullanıcı tarafından belirtilmeyen bir technology, framework, database, ORM, message broker, cloud service veya deployment modeli zorunlu kabul edilmemelidir.

---

# 3. TEMEL ARCHITECTURE MODELİ

Versa Coder için başlangıç mimari modeli:

```text
┌──────────────────────────────────────────────────────────────┐
│                    PRESENTATION                              │
│  IDE UI · Chat · Wizard · Editor · Panels · Visual Graph     │
└──────────────────────────────┬───────────────────────────────┘
                               │
                               ▼
┌──────────────────────────────────────────────────────────────┐
│                    APPLICATION                              │
│ Intent · Use Cases · Commands · Queries · Session · Workflow│
│ Orchestration · Agent Coordination · Application Services    │
└──────────────────────────────┬───────────────────────────────┘
                               │
                               ▼
┌──────────────────────────────────────────────────────────────┐
│                       AGENT RUNTIME                          │
│ Agent Lifecycle · Agent Execution · Task Decomposition       │
│ Swarm · Handoff · Context Assembly · Policy Enforcement      │
└──────────────────────────────┬───────────────────────────────┘
                               │
                               ▼
┌──────────────────────────────────────────────────────────────┐
│                         DOMAIN                               │
│ Agent · Session · Task · Workflow · Policy · Requirement     │
│ Execution · Validation · Project · Value Objects · Events    │
└──────────────────────────────┬───────────────────────────────┘
                               ▲
                               │ Dependency Inversion
┌──────────────────────────────┴───────────────────────────────┐
│                    ABSTRACTIONS / CONTRACTS                  │
│ AI · Tools · MCP · Storage · Git · Process · Project Analysis│
│ Security · Knowledge · Logging · External Agents             │
└──────────────────────────────┬───────────────────────────────┘
                               ▲
                               │ implements
┌──────────────────────────────┴───────────────────────────────┐
│                      INFRASTRUCTURE                          │
│ AI Providers · OpenCode · Codex · Claude Code · Abacus CLI   │
│ MCP · FileSystem · Git · Database · Process · Security       │
│ Roslyn · Persistence · Knowledge · Telemetry · Plugins       │
└──────────────────────────────────────────────────────────────┘
```

Bu diyagram başlangıç modelidir.

Son mimariyi analiz sonucunda doğrula.

---

# 4. EN ÖNEMLİ KURAL — DEPENDENCY DIRECTION

Bağımlılık yönü rastgele olmamalıdır.

Temel prensip:

```text
Presentation
      ↓
Application
      ↓
Agent Runtime
      ↓
Domain
      ↑
Abstractions
      ↑
Infrastructure
```

Daha doğru Clean Architecture yorumu:

```text
Outer Layers
      ↓
Inner Policies

Infrastructure → Contracts
Application → Domain
Presentation → Application
```

Domain, Infrastructure'ı bilmemelidir.

Domain:

- DevExpress
- HttpClient
- SQLite
- SQL Server
- OpenCode
- Claude Code
- Codex
- Abacus AI
- MCP SDK
- FileSystem
- Process API

gibi dış teknolojilere doğrudan bağımlı olmamalıdır.

---

# 5. DOMAIN LAYER

## Amaç

Domain katmanı Versa Coder'ın iş kavramlarını temsil eder.

Burada teknoloji değil, **iş anlamı** bulunmalıdır.

Örnek domain kavramları:

```text
Project
Repository
Requirement
Task
Agent
AgentRole
AgentCapability
Session
Workflow
WorkflowStep
Execution
ExecutionProvider
Tool
ToolPermission
Policy
Approval
Validation
Result
Artifact
Decision
KnowledgeItem
```

Bunların tamamı otomatik olarak Entity olmak zorunda değildir.

---

# 6. DOMAIN RESPONSIBILITIES

Domain içerisinde:

- Entity
- Value Object
- Domain Rule
- Domain Event
- Aggregate
- State
- Invariant

gerektiğinde kullanılabilir.

Ancak:

> **Her class Entity değildir.**

> **Her kavram Aggregate değildir.**

> **Her property Value Object değildir.**

Domain yalnızca gerçek iş kurallarını içermelidir.

---

# 7. DOMAIN'DE OLMAMASI GEREKENLER

Aşağıdakileri Domain'e koyma:

```text
DevExpress Control
HttpClient
DbContext
SQL Query
SQLite API
SQL Server API
Process.Start
File.ReadAllText
Roslyn implementation
MCP SDK implementation
OpenCode CLI command
Claude Code CLI command
Codex CLI command
JSON serializer implementation
Logging framework
UI state
WinForms event handler
```

Bunlar dış katman sorumluluklarıdır.

---

# 8. APPLICATION LAYER

Application katmanı Domain ile dış dünya arasındaki **use-case orchestration** katmanıdır.

Örnek Use Case'ler:

```text
StartChatSession
AnalyzeUserIntent
CreateSession
ConfigureAgentSession
BuildWorkflow
SelectAgent
SelectExecutionProvider
StartAgentExecution
ExecuteExternalAgent
RequestApproval
ValidateResult
CancelExecution
RetryExecution
CompleteSession
PersistSession
```

Application:

> **"Ne yapılmalı?"**

sorusunu yönetir.

---

# 9. APPLICATION ≠ DOMAIN

Domain:

> İş kuralları ve temel kavramlar.

Application:

> Bu kavramlarla hangi use-case'in nasıl yürütüleceği.

Örneğin:

```text
Domain:
Session

Application:
StartSessionUseCase
```

veya:

```text
Domain:
Execution

Application:
StartExecutionCommandHandler
```

---

# 10. AGENT RUNTIME LAYER

Versa Coder'ın en kritik katmanlarından biridir.

Agent Runtime:

```text
Agent Lifecycle
Agent Execution
Task Decomposition
Agent Selection
Agent Handoff
Context Assembly
Tool Invocation
Workflow Execution
Policy Enforcement
Execution State
Result Handling
```

gibi runtime sorumluluklarını yönetir.

Bu katman yalnızca:

> "LLM'e prompt gönder"

katmanı değildir.

---

# 11. AGENT RUNTIME SORUMLULUKLARI

Agent Runtime:

- Agent başlatır.
- Agent execution yönetir.
- Agent capability kontrol eder.
- Agent context oluşturur.
- Task'ları ayrıştırır.
- Agent'lar arasında handoff yönetir.
- Workflow step çalıştırır.
- Tool invocation başlatır.
- Policy kontrolü tetikler.
- Result alır.
- Validation sürecine aktarır.
- Execution lifecycle yönetir.

Ancak Domain kurallarını kendi içinde tekrar yazmamalıdır.

---

# 12. ORCHESTRATOR

Orchestrator, Agent Runtime içerisinde veya Application ile Agent Runtime arasında konumlandırılabilir.

Bu karar implementation detayına göre doğrulanmalıdır.

Orchestrator'ın temel görevi:

```text
Intent
↓
Task
↓
Plan
↓
Agent Selection
↓
Workflow
↓
Execution
↓
Validation
↓
Result
```

akışını yönetmektir.

Orchestrator:

- UI logic
- Database logic
- CLI syntax
- Provider-specific API logic

içermemelidir.

---

# 13. MULTI-AGENT / SWARM

Multi-Agent sistemini Agent Runtime içerisinde yönet.

Örnek:

```text
Orchestrator
      ↓
Architect Agent
      ↓
Developer Agent
      ↓
Testing Agent
      ↓
Reviewer Agent
```

Ancak:

> **Her task Multi-Agent olmak zorunda değildir.**

Routing:

```text
Simple Task
→ Single Agent

Complex Task
→ Multi-Agent Workflow
```

şeklinde değerlendirilebilir.

---

# 14. SESSION LAYER / SESSION RESPONSIBILITY

Session sistemini Domain + Application + Runtime olarak ayır.

### Domain

Session'ın state ve invariant'larını tanımlar.

### Application

Session oluşturma/configuration use-case'lerini yönetir.

### Runtime

Session execution'ı yürütür.

Bu üç sorumluluğu tek bir `SessionManager` God Class içine koyma.

---

# 15. SESSION AGENTIC AGENT WIZARD

Wizard bir Presentation concern gibi görünse de configuration üretimi yalnızca UI concern değildir.

Ayrım:

```text
Presentation:
Wizard UI

Application:
Wizard Configuration Use Case

Domain:
SessionConfiguration

Runtime:
Execution

Infrastructure:
Persistence
```

---

# 16. WIZARD FLOW

```text
CHAT
 ↓
INTENT ANALYSIS
 ↓
WIZARD OPEN
 ↓
USER CONFIGURATION
 ↓
CONFIGURATION VALIDATION
 ↓
APPROVAL
 ↓
IMMUTABLE SESSION CONFIGURATION
 ↓
SESSION START
 ↓
AGENT RUNTIME
```

Wizard UI kendi başına execution başlatmamalıdır.

---

# 17. PROMPT SYNTHESIS

Dynamic System Prompt üretimi Application/Agent Runtime sorumluluğuna bağlanmalıdır.

Prompt synthesis girdileri:

```text
Global Rules
Project Rules
Session Rules
Agent Role
Task
Context
Policy
Provider Constraints
Tool Permissions
```

çıktı:

```text
Agent Execution Instruction
```

Prompt compiler UI katmanında bulunmamalıdır.

---

# 18. AI PROVIDER LAYER

AI Provider sistemini Domain'den ayır.

Domain:

```text
IAiModel
IAiProvider
```

gibi contract'ları tanımlayabilir.

Infrastructure:

```text
OpenAI
Claude
Gemini
OpenCode
Local LLM
Other Provider
```

implementasyonlarını sağlar.

Provider-specific API bilgisi Domain'e sızmamalıdır.

---

# 19. MODEL ROUTER

Smart Model Router:

```text
Task
↓
Required Capability
↓
Available Models
↓
User Preference
↓
Policy
↓
Cost
↓
Latency
↓
Provider
↓
Model
```

şeklinde karar verebilir.

Ancak:

> Kullanıcı açıkça provider/model seçtiyse, policy izin verdiği sürece sessizce değiştirme.

---

# 20. EXTERNAL CODING AGENT LAYER

OpenCode, Codex, Claude Code ve Abacus AI CLI gibi harici coding-agent sistemleri doğrudan Application içine gömülmemelidir.

Kavramsal yapı:

```text
Application / Agent Runtime
          ↓
IExternalAgentExecutor
          ↓
Infrastructure Adapter
          ├── OpenCode
          ├── Codex
          ├── Claude Code
          └── Abacus AI CLI
```

Her provider kendi adapter'ına sahip olmalıdır.

---

# 21. EXTERNAL AGENT ADAPTER

Adapter'ın görevi:

```text
Versa Coder Request
↓
Provider-specific execution
↓
Process / API
↓
Raw Result
↓
Normalized Result
```

olmalıdır.

OpenCode CLI syntax Application'a sızmamalıdır.

Claude Code CLI syntax Application'a sızmamalıdır.

Codex CLI syntax Application'a sızmamalıdır.

---

# 22. PROCESS EXECUTION

CLI execution için ayrı bir Infrastructure concern oluştur.

Sorumluluklar:

```text
Process Start
Process Stop
Cancellation
Timeout
stdout
stderr
Exit Code
Environment
Working Directory
Child Process
Resource Limits
```

Bu mekanizma Agent Runtime'ın business logic'i olmamalıdır.

---

# 23. TOOL GATEWAY

Tool kullanımı merkezi bir Gateway üzerinden yönetilebilir.

```text
Agent
 ↓
Tool Gateway
 ↓
Policy
 ↓
Permission
 ↓
Tool
 ↓
Result
```

Tool implementation Infrastructure'da olabilir.

Tool contract'ları abstraction layer'da tanımlanabilir.

---

# 24. MCP LAYER

MCP'yi Agent ile aynı kavram kabul etme.

```text
Agent
 ↓
Tool Gateway
 ↓
MCP Adapter / Client
 ↓
MCP Server
 ↓
External Tool
```

MCP:

> Agent değil.

MCP:

> Tool/context integration protocol boundary'dir.

Gerçek MCP implementation dependency'leri Infrastructure'da tutulmalıdır.

---

# 25. POLICY ENGINE

Policy sistemi kritik bir cross-cutting control mekanizmasıdır.

Kontrol ettiği alanlar:

```text
Agent Permission
Tool Permission
File Access
Repository Access
Process Execution
Network Access
External Provider
Approval
Sensitive Operation
```

Policy yalnızca UI'da checkbox olmamalıdır.

Runtime tarafından uygulanmalıdır.

---

# 26. APPROVAL SYSTEM

Approval:

```text
Request
↓
Policy Check
↓
Approval Required?
↓
User Approval
↓
Execution
```

şeklinde çalışmalıdır.

UI yalnızca approval interaction'ını gösterir.

Kararın kendisi Application/Policy/Runtime sınırında olmalıdır.

---

# 27. PROJECT ANALYSIS / CODE INTELLIGENCE

Versa Coder'ın Repository ve kod analizi ayrı bir subsystem olarak planlanmalıdır.

Sorumluluklar:

```text
Repository Discovery
Project Structure
Dependency Analysis
Roslyn Analysis
Symbol Analysis
AST
References
Call Graph
Diagnostics
Code Index
```

Bunları Agent Runtime'ın içine gömme.

Agent Runtime analiz servislerini kullanmalıdır.

---

# 28. CONTEXT ASSEMBLY

Context Assembly:

```text
User Message
+
Project Context
+
Repository Context
+
Relevant Files
+
Requirements
+
Rules
+
Session State
+
Knowledge
```

birleştirerek Agent için uygun execution context oluşturur.

Context assembly:

> Agent Runtime ile Project Analysis / Knowledge arasında orchestration concern'dir.

---

# 29. KNOWLEDGE / .AI VAULT

`.ai` klasörünü doğrudan Domain veya UI içine koyma.

`.ai`:

> Project Knowledge / Documentation / SSOT kaynağıdır.

Kavramsal yapı:

```text
Application
 ↓
IKnowledgeService
 ↓
Knowledge Infrastructure
 ↓
.ai / Markdown / Other Store
```

`.ai` içeriği Domain Entity değildir.

---

# 30. MEMORY

Memory'yi Knowledge ile tamamen aynı kabul etme.

### Working Memory

Aktif Session Context.

### Long-Term Knowledge

`.ai`, documentation, decisions, project knowledge.

### Session Memory

Session sırasında oluşan execution bilgileri.

Bu ayrımların persistence stratejisi implementation aşamasında netleştirilmelidir.

---

# 31. PERSISTENCE

Persistence sorumluluğunu Infrastructure altında tut.

Olası kaynaklar:

```text
Session Store
Execution Store
Audit Store
Knowledge Store
Configuration Store
Project Metadata Store
```

Database teknolojisi belirtilmediyse:

`[DATABASE BELİRTİLMELİ]`

kullan.

---

# 32. AUDIT / OBSERVABILITY

Audit ve Observability aynı şey değildir.

### Audit

> Ne oldu?

### Observability

> Sistem şu anda nasıl çalışıyor?

Audit:

```text
Session
Agent
Provider
Action
Approval
Result
Validation
Timestamp
```

Observability:

```text
Logs
Metrics
Traces
Latency
Errors
Execution State
```

Framework seçimini uydurma.

---

# 33. SECURITY LAYER

Security cross-cutting concern olarak ele alınmalıdır.

İlgili alanlar:

```text
Authentication
Authorization
Credential Protection
Secret Handling
Process Isolation
Tool Permission
File Permission
Network Permission
Audit Protection
Sensitive Data Handling
```

Security'nin bütün sistemi tekrar eden bir God Service'e dönüşmesine izin verme.

---

# 34. PLUGIN SYSTEM

Plugin Architecture ayrı bir extensibility boundary olmalıdır.

```text
Plugin Contract
↓
Plugin Loader
↓
Isolation Boundary
↓
Plugin
```

Plugin'in Domain'e doğrudan dependency eklemesine izin verme.

Mevcut kaynaklarda `AssemblyLoadContext` tabanlı isolation fikri bulunuyor; gerçek uygulama kararı implementation öncesinde doğrulanmalıdır.

---

# 35. GIT / REPOSITORY SERVICES

Git ve Repository erişimini Infrastructure adapter'ları üzerinden yönet.

Application:

```text
IGitService
IRepositoryService
```

gibi abstraction kullanabilir.

Infrastructure:

```text
Git CLI
Git Library
File System
```

implementation'larını sağlayabilir.

---

# 36. FILE SYSTEM

File System doğrudan Agent'a verilmemelidir.

Doğru yaklaşım:

```text
Agent
 ↓
Tool Gateway
 ↓
Policy
 ↓
File System Tool
 ↓
File System
```

Böylece:

- path restriction,
- read/write permission,
- delete permission,
- audit

uygulanabilir.

---

# 37. PRESENTATION LAYER

Presentation katmanı:

```text
Main IDE
Chat
Agent Console
Project Explorer
Code Editor
Session Wizard
Agent Wizard
Visual Workflow
Execution Graph
Logs
Settings
Dialogs
```

gibi kullanıcı interaction'larını yönetir.

Presentation:

> Business logic taşımaz.

---

# 38. DEVEXPRESS SINIRI

DevExpress yalnızca UI boundary'de kullanılmalıdır.

Domain:

```text
NO DevExpress
```

Application:

```text
NO DevExpress
```

Domain/Agent Runtime:

```text
NO DevExpress
```

Presentation:

```text
DevExpress
```

UI technology değiştiğinde Domain/Application'ın yeniden yazılması gerekmemelidir.

---

# 39. UI → APPLICATION

UI event handler:

```text
Button_Click
```

içinde:

```text
Agent selection
Database access
CLI execution
Business rules
Policy logic
```

yapma.

Bunun yerine:

```text
UI
 ↓
Command / Use Case
 ↓
Application
```

---

# 40. CONFIGURATION LAYER

Configuration'ı her projeye dağılmış static class'lara dönüştürme.

Ayrım:

```text
Global Configuration
Project Configuration
Session Configuration
Agent Configuration
Provider Configuration
Security Configuration
```

Configuration contract'ları abstraction/application tarafında tanımlanabilir.

Provider-specific configuration Infrastructure'da kalmalıdır.

---

# 41. CROSS-CUTTING CONCERNS

Cross-cutting concerns:

```text
Logging
Telemetry
Caching
Security
Validation
Error Handling
Retry
Tracing
Configuration
```

için merkezi abstraction kullanılabilir.

Ancak:

> Her cross-cutting concern için yeni layer oluşturma.

---

# 42. ERROR HANDLING

Hatalar katmanlar arasında raw exception olarak rastgele dolaştırılmamalıdır.

Örnek:

```text
Infrastructure Exception
↓
Adapter
↓
Normalized Application Error
↓
Application Result
↓
Presentation
```

Root Cause kaybolmamalıdır.

---

# 43. RESULT MODEL

Agentic execution için standart bir Result modeli değerlendir:

```text
ExecutionResult
├── Status
├── Output
├── Errors
├── Warnings
├── Artifacts
├── ChangedFiles
├── Validation
├── Metadata
└── AuditReference
```

Bu bir başlangıç modelidir.

Gereksiz alan ekleme.

---

# 44. EVENT / MESSAGE MODEL

Agent Runtime içerisinde önemli olaylar event olarak modellenebilir:

```text
SessionStarted
AgentStarted
AgentCompleted
ToolInvoked
ApprovalRequested
ApprovalGranted
ExecutionStarted
ExecutionCompleted
ValidationStarted
ValidationCompleted
SessionCompleted
```

Ancak her action için Event üretme.

Event-driven architecture yalnızca gerçekten gerekli olduğu noktada kullanılmalıdır.

---

# 45. CQRS KARARI

CQRS kullanımı otomatik zorunluluk değildir.

Eğer:

- read/write model ayrımı,
- karmaşık query,
- audit,
- execution state,
- yüksek orchestration complexity

gerçek gereksinim oluşturuyorsa değerlendir.

Aksi durumda:

> `[CQRS GEREKLİLİĞİ DOĞRULANMALI]`

olarak bırak.

---

# 46. ASYNC / CONCURRENCY

Agentic sistem doğal olarak asynchronous olabilir.

Özellikle:

```text
Agent Execution
Tool Execution
External CLI
Streaming
Parallel Agents
Validation
Telemetry
```

için async/cancellation tasarımı gereklidir.

Ancak bütün sistemi gereksiz şekilde parallel hale getirme.

Concurrency yalnızca bağımlılık grafiği izin verdiğinde uygulanmalıdır.

---

# 47. WORKFLOW ENGINE

Workflow:

```text
Task
↓
Steps
↓
Dependencies
↓
Execution
↓
Conditions
↓
Retry
↓
Validation
```

sorumluluklarını yönetebilir.

Workflow engine'i UI içine koyma.

Workflow definition ile workflow execution'ı ayır.

---

# 48. DAG / TASK DECOMPOSITION

Complex task:

```text
Requirement
↓
Task Decomposition
↓
DAG
↓
Parallel / Sequential Tasks
```

şeklinde modellenebilir.

Ancak DAG yalnızca gerçekten dependency ilişkisi varsa kullanılmalıdır.

Basit task için:

```text
Single Task
```

yeterlidir.

---

# 49. ARCHITECTURAL BOUNDARY RULES

Aşağıdaki bağımlılıklar yasaklanmalıdır:

```text
Domain → UI                 ❌
Domain → DevExpress         ❌
Domain → Infrastructure    ❌
Domain → Database           ❌
Domain → AI Provider        ❌

Application → DevExpress    ❌
Application → CLI           ❌
Application → Database API ❌
Application → Provider SDK ❌

Agent Runtime → UI          ❌
Agent Runtime → DevExpress  ❌

UI → Infrastructure         ❌
UI → Database               ❌
UI → CLI                    ❌
```

UI, Application üzerinden işlem başlatmalıdır.

---

# 50. ÖNERİLEN PROJECT / DLL ORGANİZASYONU

Aşağıdaki yapı başlangıç önerisidir; nihai DLL sayısı gerçek dependency graph çıkarıldıktan sonra belirlenmelidir.

```text
VersaCoder.sln

01 — Core
├── VersaCoder.Domain
└── VersaCoder.Abstractions

02 — Application
├── VersaCoder.Application
├── VersaCoder.Application.Contracts
└── VersaCoder.Application.Orchestration

03 — Agent Runtime
├── VersaCoder.AgentRuntime
├── VersaCoder.AgentRuntime.Abstractions
├── VersaCoder.AgentRuntime.Workflows
└── VersaCoder.AgentRuntime.Policies

04 — AI
├── VersaCoder.AI.Contracts
├── VersaCoder.AI.Routing
└── VersaCoder.AI.Models

05 — Tools / MCP
├── VersaCoder.Tools
├── VersaCoder.Tools.Contracts
├── VersaCoder.MCP
└── VersaCoder.MCP.Contracts

06 — Project Intelligence
├── VersaCoder.ProjectAnalysis
├── VersaCoder.CodeAnalysis
└── VersaCoder.Repository

07 — External Agents
├── VersaCoder.ExternalAgents
├── VersaCoder.ExternalAgents.Contracts
├── VersaCoder.ExternalAgents.OpenCode
├── VersaCoder.ExternalAgents.Codex
├── VersaCoder.ExternalAgents.ClaudeCode
└── VersaCoder.ExternalAgents.Abacus

08 — Infrastructure
├── VersaCoder.Infrastructure
├── VersaCoder.Infrastructure.Persistence
├── VersaCoder.Infrastructure.FileSystem
├── VersaCoder.Infrastructure.Git
├── VersaCoder.Infrastructure.Process
├── VersaCoder.Infrastructure.Security
├── VersaCoder.Infrastructure.Logging
└── VersaCoder.Infrastructure.Observability

09 — Knowledge
├── VersaCoder.Knowledge
└── VersaCoder.Knowledge.Infrastructure

10 — Plugins
├── VersaCoder.Plugin.Contracts
└── VersaCoder.Plugin.Runtime

11 — Presentation
└── VersaCoder.Presentation.WinForms
```

Bu liste:

> **kesin DLL sayısı değildir.**

Dependency graph sonucuna göre birleştirilebilir veya ayrıştırılabilir.

---

# 51. DLL SAYISI ŞİŞİRME YASAĞI

50+ DLL oluşturmak tek başına iyi Architecture değildir.

Yanlış:

```text
Her class = DLL
Her feature = DLL
Her service = DLL
Her interface = DLL
```

Doğru:

```text
Boundary
+
Responsibility
+
Independent Change
+
Dependency Isolation
+
Testability
+
Deployment / Loading Need
```

gerçekten gerekiyorsa project/DLL oluştur.

---

# 52. MODÜL SINIRLARI

Her modül için aşağıdaki tabloyu üret:

| Module | Responsibility | Depends On | Used By | Technology | State | Risk |
|---|---|---|---|---|---|---|

Her module için:

- neden var?
- neden ayrı?
- hangi layer'a ait?
- hangi bağımlılıkları var?
- hangi bağımlılıkları olmamalı?
- test stratejisi nedir?
- değişim etkisi nedir?

açıkla.

---

# 53. DEPENDENCY GRAPH

Mimari planın sonunda gerçek dependency graph üret:

```text
Presentation
      ↓
Application
      ↓
Agent Runtime
      ↓
Domain
      ↑
Contracts
      ↑
Infrastructure
```

Buna ek olarak:

```text
Agent Runtime
 ├── AI Contracts
 ├── Tool Contracts
 ├── Knowledge Contracts
 ├── Project Analysis Contracts
 └── External Agent Contracts
```

gibi ilişkileri göster.

Circular dependency tespit et.

---

# 54. CIRCULAR DEPENDENCY YASAĞI

Özellikle:

```text
Domain ↔ Infrastructure
Application ↔ Presentation
AgentRuntime ↔ Presentation
AI ↔ Domain
Tool ↔ AgentRuntime
```

gibi circular dependency oluşmasına izin verme.

Cycle tespit edilirse:

```text
ROOT CAUSE
↓
DEPENDENCY CYCLE
↓
BOUNDARY VIOLATION
↓
ABSTRACTION / RELOCATION
↓
VERIFICATION
```

ile çöz.

---

# 55. 50+ YILLIK SENIOR ARCHITECT REVIEW

Mimar her katman için şu soruları sormalıdır:

1. Bu katman neden var?
2. Bu sorumluluk başka katmana neden konulmadı?
3. Bu katmanın değişmesi kimi etkiler?
4. Dependency yönü doğru mu?
5. Domain temiz mi?
6. UI bağımlılığı sızmış mı?
7. Infrastructure business rule taşıyor mu?
8. Application God Service'e dönüşmüş mü?
9. Agent Runtime aşırı büyümüş mü?
10. Orchestrator God Object olmuş mu?
11. External Agent provider coupling var mı?
12. Test edilebilir mi?
13. Mock edilebilir mi?
14. Replace edilebilir mi?
15. Security boundary doğru mu?
16. Failure boundary doğru mu?
17. Persistence sınırı doğru mu?
18. Async lifecycle kontrol ediliyor mu?
19. Cancellation uygulanabiliyor mu?
20. Logging/audit yeterli mi?
21. Gereksiz abstraction var mı?
22. Gereksiz DLL var mı?
23. Gereksiz complexity var mı?
24. Yarın DevExpress değişirse ne olur?
25. Yarın OpenCode değişirse ne olur?
26. Yarın Claude Code değişirse ne olur?
27. Yarın AI Provider değişirse ne olur?
28. Database değişirse ne olur?
29. External CLI kullanılmazsa ne olur?
30. Internal Agent tek başına çalışabilir mi?

---

# 56. DEĞİŞTİRİLEBİLİRLİK TESTİ

Architecture şu değişiklikleri minimum etkiyle karşılamalıdır:

```text
DevExpress değişti
↓
Domain etkilenmemeli

OpenCode değişti
↓
Core/Application etkilenmemeli

Claude Code değişti
↓
External Adapter değişmeli

Database değişti
↓
Persistence implementation değişmeli

LLM Provider değişti
↓
AI Provider adapter değişmeli

MCP implementation değişti
↓
MCP Infrastructure değişmeli

CLI execution değişti
↓
Process / External Agent adapter değişmeli
```

Bu test mimarinin gerçek abstraction kalitesini ölçmek için kullanılmalıdır.

---

# 57. TEST ARCHITECTURE

Her katman için uygun test seviyesi belirle:

```text
Domain
→ Unit Test

Application
→ Unit + Integration Test

Agent Runtime
→ Unit + Integration + Scenario Test

External Agents
→ Contract + Integration Test

MCP
→ Integration Test

Project Analysis
→ Integration / Regression Test

Infrastructure
→ Integration Test

Presentation
→ UI / E2E / Smoke Test
```

Test türlerini teknoloji verilmeden zorunlu framework'e bağlama.

---

# 58. PERFORMANCE

Performance planında önce ölçüm yap.

Ölçülecek alanlar:

```text
Startup
Project Discovery
AST Analysis
Context Assembly
Agent Scheduling
LLM Latency
Tool Latency
CLI Startup
External Agent Execution
Persistence
UI Responsiveness
Memory Usage
Parallel Agent Execution
```

Premature optimization yapma.

---

# 59. SECURITY ARCHITECTURE REVIEW

Özellikle External Agent nedeniyle:

```text
Process
↓
Working Directory
↓
Environment
↓
Credential
↓
Network
↓
Tool
↓
File
↓
Repository
```

boundary'lerini incele.

External CLI:

> Versa Coder'ın güvenlik politikasını bypass etmemelidir.

---

# 60. IMPLEMENTATION ÖNCESİ PLAN

Kodlamaya başlamadan önce şu sırayı uygula:

```text
1. PROJECT DISCOVERY
2. REFERENCE ANALYSIS
3. REQUIREMENT EXTRACTION
4. ARCHITECTURE DECISIONS
5. DOMAIN MODEL
6. LAYER DEFINITIONS
7. DEPENDENCY RULES
8. PROJECT / DLL PLAN
9. AGENT RUNTIME PLAN
10. AI PROVIDER PLAN
11. EXTERNAL AGENT PLAN
12. TOOL / MCP PLAN
13. SESSION / WORKFLOW PLAN
14. SECURITY PLAN
15. PERSISTENCE PLAN
16. OBSERVABILITY PLAN
17. TEST PLAN
18. IMPLEMENTATION PLAN
19. VALIDATION
20. USER APPROVAL
21. ONLY THEN CODE
```

---

# 61. ARCHITECTURE DECISION RECORD

Her önemli mimari karar için ADR oluştur.

Format:

```text
ADR-ID
TITLE
STATUS
CONTEXT
PROBLEM
OPTIONS
DECISION
RATIONALE
TRADE-OFFS
DEPENDENCIES
CONSEQUENCES
VALIDATION
```

---

# 62. KARARLARDA VARSAYIM YASAĞI

Aşağıdakileri kullanıcı belirtmediyse kesinleştirme:

```text
Database
ORM
Message Broker
Cloud
Container
Kubernetes
Microservices
Event Bus
CQRS
Event Sourcing
Vector Database
Redis
RabbitMQ
Kafka
Specific AI Provider
Specific Model
Specific CLI Syntax
Specific Sandbox
Specific Deployment
```

Bunlar gerekiyorsa:

`[BELİRTİLMELİ]`

veya:

`[DOĞRULAMA GEREKLİ]`

olarak bırak.

---

# 63. MONOLITHIC MODULARITY ÖNCELİĞİ

Versa Coder başlangıçta modüler bir desktop/IDE platformu olarak tasarlanıyorsa sırf "Enterprise" görünmek için Microservices oluşturma.

Öncelik:

```text
Modular Monolith / Modular Desktop Architecture
```

gerçek gereksinimler farklı bir deployment modelini zorunlu kılana kadar değerlendirilebilir.

Bu bir nihai karar değildir.

---

# 64. ARCHITECTURE QUALITY GATE

Mimari onaylanmadan önce:

### Boundary

- [ ] Katmanlar net.
- [ ] Dependency direction doğru.
- [ ] Circular dependency yok.
- [ ] Domain dış teknolojiye bağlı değil.

### Agent

- [ ] Agent Runtime ayrı sorumluluk.
- [ ] Orchestrator sınırları net.
- [ ] Session/Workflow ayrımı net.
- [ ] Multi-Agent gereksiz yere kullanılmıyor.

### AI

- [ ] Provider abstraction var.
- [ ] Model routing ayrı.
- [ ] Provider coupling sınırlı.

### External Agent

- [ ] OpenCode adapter ayrı.
- [ ] Codex adapter ayrı.
- [ ] Claude Code adapter ayrı.
- [ ] Abacus adapter ayrı.
- [ ] CLI process boundary var.
- [ ] Result normalization var.

### Tool / MCP

- [ ] Tool Gateway var.
- [ ] Policy kontrolü var.
- [ ] MCP boundary net.

### Security

- [ ] Credentials korunuyor.
- [ ] Working Directory kontrol ediliyor.
- [ ] Process permission kontrol ediliyor.
- [ ] Approval mekanizması var.

### Persistence

- [ ] Domain persistence bilmiyor.
- [ ] Session persistence ayrılmış.
- [ ] Audit ayrılmış.
- [Knowledge ayrılmış.

### UI

- [ ] Presentation business logic içermiyor.
- [ ] DevExpress dış katmanlara sızmıyor.
- [ ] Wizard execution logic içermiyor.

### Quality

- [ ] Test stratejisi var.
- [ ] Observability var.
- [ ] Audit var.
- [ ] Validation var.
- [ ] Error handling var.
- [ ] Cancellation var.

---

# 65. OUTPUT FORMAT

Bu promptu uygulayan AI'ın çıktısı yalnızca mimari plan olmamalıdır.

Aşağıdaki sırayla üret:

```text
# VERSA CODER — LAYERED ARCHITECTURE PLAN

## 1. Architecture Summary

## 2. Architecture Principles

## 3. Layer Map

## 4. Dependency Direction

## 5. Domain Layer

## 6. Application Layer

## 7. Agent Runtime Layer

## 8. AI Provider Layer

## 9. Tool / MCP Layer

## 10. Project Intelligence Layer

## 11. External Coding Agent Layer

## 12. Infrastructure Layer

## 13. Knowledge / Memory Layer

## 14. Persistence Layer

## 15. Security / Policy

## 16. Presentation Layer

## 17. Session Agentic Agent Wizard

## 18. Workflow / DAG

## 19. Project / DLL Structure

## 20. Dependency Graph

## 21. Data Flow

## 22. Execution Flow

## 23. Security Boundaries

## 24. Error / Failure Boundaries

## 25. Testing Architecture

## 26. Performance Considerations

## 27. ADR Decisions

## 28. Open Questions

## 29. Risks

## 30. Architecture Validation

## 31. Implementation Plan

## 32. Final Architecture Summary
```

---

# 66. HER KATMAN İÇİN ZORUNLU ÇIKTI

Her katmanı şu formatta açıkla:

```text
Layer:
Purpose:
Responsibilities:
Does:
Does Not:
Depends On:
Used By:
Contracts:
Main Components:
Persistence:
Security:
Testing:
Failure Boundary:
Change Impact:
```

---

# 67. HER PROJECT / DLL İÇİN ZORUNLU ÇIKTI

```text
Project:
Layer:
Responsibility:
Why Exists:
Depends On:
Must Not Depend On:
Public Contracts:
Internal Components:
Test Strategy:
Change Impact:
Status:
```

Status:

```text
REQUIRED
OPTIONAL
NOT REQUIRED
VERIFY REQUIRED
```

---

# 68. FINAL ARCHITECTURE DIAGRAM

Final çıktıda en az şu diyagramları üret:

### Layer Diagram

```text
Presentation
↓
Application
↓
Agent Runtime
↓
Domain
↑
Contracts
↑
Infrastructure
```

### Agent Execution

```text
Chat
↓
Intent
↓
Wizard
↓
Session
↓
Orchestrator
↓
Agent
↓
Tool / AI / External Agent
↓
Validation
↓
Result
```

### External Agent

```text
Versa Coder
↓
External Agent Contract
↓
Adapter
↓
CLI / API
↓
External Agent
↓
Result
↓
Normalizer
↓
Versa Coder
```

### Security

```text
Request
↓
Policy
↓
Permission
↓
Approval
↓
Execution
↓
Audit
```

---

# 69. SON MİMARİ İLKESİ

Versa Coder'ın amacı:

> **çok sayıda DLL üretmek değildir.**

Amaç:

> **her sorumluluğun doğru yerde bulunduğu, dependency direction'ın korunduğu, Agentic runtime'ın kontrol edilebilir olduğu, AI Provider ve External Coding Agent'ların değiştirilebilir olduğu, UI'ın core business logic'ten ayrıldığı, test edilebilir ve sürdürülebilir bir Software Architecture oluşturmaktır.**

---

# 70. 50+ YILLIK SENIOR ARCHITECT SON KONTROLÜ

Final mimariyi onaylamadan önce şu soruya cevap ver:

> "Bu sistemi ben bugün tasarlayıp yarın 10 yıl boyunca geliştirmek zorunda olsaydım, hangi karar beni ileride en fazla teknik borca sokardı?"

Ardından:

```text
Root Cause
↓
Architectural Risk
↓
Impact
↓
Decision
↓
Trade-off
↓
Mitigation
↓
Verification
```

şeklinde değerlendir.

---

# 71. MUTLAK KURAL

Kod yazma.

Class üretme.

Interface implementation üretme.

Database oluşturma.

CLI command uydurma.

Provider API uydurma.

Framework/version uydurma.

Önce:

```text
UNDERSTAND
↓
ANALYZE
↓
MODEL
↓
SEPARATE
↓
VALIDATE
↓
PLAN
↓
APPROVE
↓
IMPLEMENT
```

---

# 72. FINAL CORE RULE

Versa Coder'ın katmanlı mimarisi:

```text
PRESENTATION
        ↓
APPLICATION
        ↓
AGENT RUNTIME
        ↓
DOMAIN
        ↑
ABSTRACTIONS / CONTRACTS
        ↑
INFRASTRUCTURE
```

çevresinde kurulmalı; ancak AI, Tool/MCP, Project Intelligence, External Agent, Knowledge, Persistence ve Security gibi büyük bounded responsibility'ler gerçek dependency graph'a göre modülerleştirilmelidir.

**Layer sayısı hedef değildir.**

**Doğru boundary hedeftir.**

**DLL sayısı hedef değildir.**

**Bağımsız değiştirilebilirlik hedeftir.**

**Agent sayısı hedef değildir.**

**Doğru orchestration hedeftir.**

**LLM sayısı hedef değildir.**

**Doğru engineering outcome hedeftir.**

Final mimari ancak bu prensipler doğrulandıktan sonra implementation aşamasına geçirilmelidir.
