---
title: "Versa Coder — Detailed Architecture"
type: architecture
category: detailed
date: 2026-08-25
version: 1.0.0
---

# Versa Coder — Detailed Architecture

**Zorunlu Bağlantılar:** [[CLAUDE.md]] · [[AGENTS.md]] · [[brain.md]] · [[architecture-master]]

---

## 1. Mimari Felsefe

Versa Coder, **Clean Architecture** + **DDD** + **MVVM** prensiplerini birleştiren, 30 katmanlı modüler bir yapıya sahiptir.

### 1.1 Temel Prensipler

| Prensip | Uygulama |
|---------|----------|
| **SOLID** | Her sınıf tek sorumluluk, açık-kapalı, yerine koyma, arayüz ayrımı, bağımlılık tersi |
| **Clean Architecture** | Bağımlılıklar içeriye doğru akar (Dependency Rule) |
| **DDD** | Domain katmanı bağımsız, Application katmanı Use Case'leri orkestra eder |
| **MVVM** | View↔ViewModel bağımsız, CommunityToolkit.Mvvm |
| **CQRS** | Komut ve sorgu ayrımı (MediatR) |

### 1.2 Bağımlılık Matrisi

```
L7 UI → L6 Host → L5 Protocol → L4 Infrastructure → L3 CrossCutting → L2 Application → L1 Abstractions → L0 Domain
```

**Kurallar:**
- ✅ Her katman sadece bir altındaki katmana bağımlı olabilir
- ❌ Aşağı katmanlar yukarı katmanları asla çağırılamaz
- ❌ Cross-cutting concerns (L3) hiçbir katmana bağımlı değildir

---

## 2. Solution Yapısı

```
VersaCoder.sln
│
├── src/
│   ├── VersaCoder.Domain/                    # L0 — Domain
│   ├── VersaCoder.Abstractions/              # L1 — Abstractions
│   ├── VersaCoder.Application/               # L2 — Application
│   ├── VersaCoder.CrossCutting/              # L3 — CrossCutting
│   │
│   ├── VersaCoder.Infrastructure.Data/       # L4.1 — Data
│   ├── VersaCoder.Infrastructure.AI/         # L4.2 — AI
│   ├── VersaCoder.Infrastructure.MCP/        # L4.3 — MCP
│   ├── VersaCoder.Infrastructure.Auth/       # L4.4 — Auth
│   ├── VersaCoder.Infrastructure.Config/     # L4.5 — Config
│   ├── VersaCoder.Infrastructure.Plugins/    # L4.6 — Plugins
│   ├── VersaCoder.Infrastructure.Services/   # L4.7 — Services
│   ├── VersaCoder.Infrastructure.Caching/    # L4.8 — Caching
│   ├── VersaCoder.Infrastructure.Messaging/  # L4.9 — Messaging
│   ├── VersaCoder.Infrastructure.FileSystem/ # L4.10 — FileSystem
│   ├── VersaCoder.Infrastructure.Network/    # L4.11 — Network
│   ├── VersaCoder.Infrastructure.Security/   # L4.12 — Security
│   ├── VersaCoder.Infrastructure.Observability/ # L4.13 — Observability
│   ├── VersaCoder.Infrastructure.Context/    # L4.14 — Context
│   ├── VersaCoder.Infrastructure.Learning/   # L4.15 — Learning
│   ├── VersaCoder.Infrastructure.Diagram/    # L4.16 — Diagram
│   ├── VersaCoder.Infrastructure.ProjectAnalysis/ # L4.17 — ProjectAnalysis
│   ├── VersaCoder.Infrastructure.Testing/    # L4.18 — Testing
│   ├── VersaCoder.Infrastructure.Documentation/ # L4.19 — Documentation
│   ├── VersaCoder.Infrastructure.Refactoring/ # L4.20 — Refactoring
│   ├── VersaCoder.Infrastructure.CodeAnalysis/ # L4.21 — CodeAnalysis
│   ├── VersaCoder.Infrastructure.Git/        # L4.22 — Git
│   ├── VersaCoder.Infrastructure.Integration/ # L4.23 — Integration
│   ├── VersaCoder.Infrastructure.Templating/ # L4.24 — Templating
│   ├── VersaCoder.Infrastructure.Deployment/ # L4.25 — Deployment
│   ├── VersaCoder.Infrastructure.Backup/     # L4.26 — Backup
│   ├── VersaCoder.Infrastructure.Versioning/ # L4.27 — Versioning
│   │
│   ├── VersaCoder.Protocol/                  # L5 — Protocol
│   ├── VersaCoder.Host/                      # L6 — Host
│   └── VersaCoder.UI/                        # L7 — UI
│
└── tests/
    ├── VersaCoder.Domain.Tests/
    ├── VersaCoder.Application.Tests/
    └── VersaCoder.Infrastructure.Tests/
```

---

## 3. Katman Detayları

### 3.1 L0 — Domain (`VersaCoder.Domain`)

**Sorumluluk:** İş kuralları, varlıklar, değer objeleri, domain event'leri

**Bağımlılık:** Yok (en iç katman)

**İçerik:**

```
VersaCoder.Domain/
├── Entities/
│   ├── Session.cs                    # Oturum varlığı
│   ├── Prompt.cs                     # Prompt varlığı
│   ├── Response.cs                   # Yanıt varlığı
│   ├── Message.cs                    # Mesaj varlığı
│   ├── Conversation.cs               # Konuşma varlığı
│   ├── Project.cs                    # Proje varlığı
│   ├── FileEntry.cs                  # Dosya girişi
│   ├── Agent.cs                      # Agent varlığı
│   ├── Tool.cs                       # Tool varlığı
│   ├── Skill.cs                      # Skill varlığı
│   ├── Context.cs                    # Bağlam varlığı
│   ├── LearningEntry.cs              # Öğrenme girişi
│   ├── Diagram.cs                    # Diyagram varlığı
│   └── User.cs                       # Kullanıcı varlığı
│
├── ValueObjects/
│   ├── SessionId.cs                  # Session ID
│   ├── PromptText.cs                 # Prompt metni
│   ├── AgentType.cs                  # Agent türü
│   ├── ToolName.cs                   # Tool adı
│   ├── FilePath.cs                   # Dosya yolu
│   ├── ModelName.cs                  # Model adı
│   └── Timestamp.cs                  # Zaman damgası
│
├── Enums/
│   ├── SessionState.cs               # ACTIVE, PAUSED, COMPLETED
│   ├── AgentRole.cs                  # BUILD, PLAN, EXPLORE, etc.
│   ├── Priority.cs                   # CRITICAL, HIGH, MEDIUM, LOW
│   ├── FileType.cs                   # CS, MD, JSON, etc.
│   └── ContextType.cs                # PROJECT, FILE, SESSION, etc.
│
├── Events/
│   ├── SessionCreatedEvent.cs
│   ├── PromptSentEvent.cs
│   ├── ResponseReceivedEvent.cs
│   ├── ToolExecutedEvent.cs
│   ├── AgentHandoverEvent.cs
│   └── LearningRecordedEvent.cs
│
├── Interfaces/
│   ├── IRepository.cs                # Generic repository
│   ├── IUnitOfWork.cs                # Unit of work
│   └── IDomainEvent.cs               # Domain event interface
│
└── Constants/
    ├── AgentNames.cs                 # Agent sabitleri
    ├── ToolNames.cs                  # Tool sabitleri
    └── SystemConstants.cs            # Sistem sabitleri
```

**NuGet Paketleri:** Yok (bağımsız)

---

### 3.2 L1 — Abstractions (`VersaCoder.Abstractions`)

**Sorumluluk:** Arayüzler, kontratlar, port'lar

**Bağımlılık:** L0 (Domain)

**İçerik:**

```
VersaCoder.Abstractions/
├── Repositories/
│   ├── ISessionRepository.cs
│   ├── IPromptRepository.cs
│   ├── IConversationRepository.cs
│   ├── IProjectRepository.cs
│   ├── ILearningRepository.cs
│   └── IDiagramRepository.cs
│
├── Services/
│   ├── IAIService.cs                 # AI sağlayıcı arayüzü
│   ├── IAgentRunner.cs              # Agent çalıştırıcı
│   ├── IToolExecutor.cs             # Tool çalıştırıcı
│   ├── IContextManager.cs           # Context yöneticisi
│   ├── ISessionManager.cs           # Session yöneticisi
│   ├── IProjectAnalyzer.cs          # Proje analizi
│   ├── IDiagramTeacher.cs           # Diyagram öğretme
│   ├── ILearningService.cs          # Öğrenme servisi
│   ├── ITemplateService.cs          # Şablon servisi
│   └── IGitService.cs              # Git servisi
│
├── Providers/
│   ├── ILLMProvider.cs              # LLM sağlayıcı arayüzü
│   ├── IEmbeddingProvider.cs        # Embedding sağlayıcı
│   └── ITokenizer.cs               # Tokenizer
│
├── Plugins/
│   ├── IPlugin.cs                   # Plugin arayüzü
│   ├── IPluginManager.cs           # Plugin yöneticisi
│   └── ITool.cs                     # Tool arayüzü
│
└── Events/
    ├── IDomainEventBus.cs           # Event bus arayüzü
    └── IEventHandler.cs             # Event handler
```

**NuGet Paketleri:** Yok (sadece arayüzler)

---

### 3.3 L2 — Application (`VersaCoder.Application`)

**Sorumluluk:** Use case'ler, DTO'lar, handler'lar, servisler

**Bağımlılık:** L0, L1

**İçerik:**

```
VersaCoder.Application/
├── DTOs/
│   ├── SessionDto.cs
│   ├── PromptDto.cs
│   ├── ResponseDto.cs
│   ├── AgentDto.cs
│   ├── ToolDto.cs
│   ├── ContextDto.cs
│   └── ProjectAnalysisDto.cs
│
├── Commands/
│   ├── CreateSessionCommand.cs
│   ├── SendPromptCommand.cs
│   ├── ExecuteToolCommand.cs
│   ├── SwitchAgentCommand.cs
│   ├── SaveContextCommand.cs
│   └── RecordLearningCommand.cs
│
├── Queries/
│   ├── GetSessionQuery.cs
│   ├── GetConversationQuery.cs
│   ├── GetProjectAnalysisQuery.cs
│   ├── GetContextQuery.cs
│   └── GetLearningEntriesQuery.cs
│
├── Handlers/
│   ├── CreateSessionHandler.cs
│   ├── SendPromptHandler.cs
│   ├── ExecuteToolHandler.cs
│   ├── SwitchAgentHandler.cs
│   ├── SaveContextHandler.cs
│   ├── RecordLearningHandler.cs
│   ├── GetSessionHandler.cs
│   ├── GetConversationHandler.cs
│   ├── GetProjectAnalysisHandler.cs
│   ├── GetContextHandler.cs
│   └── GetLearningEntriesHandler.cs
│
├── Services/
│   ├── SessionService.cs
│   ├── PromptService.cs
│   ├── AgentService.cs
│   ├── ContextService.cs
│   ├── ProjectAnalysisService.cs
│   └── LearningService.cs
│
├── Validators/
│   ├── CreateSessionValidator.cs
│   ├── SendPromptValidator.cs
│   └── ExecuteToolValidator.cs
│
├── Mappings/
│   ├── SessionMapping.cs
│   ├── PromptMapping.cs
│   └── AgentMapping.cs
│
└── Common/
    ├── Result.cs                     # Result pattern
    ├── PaginatedList.cs              # Sayfalı liste
    └── ValidationError.cs            # Validasyon hatası
```

**NuGet Paketleri:**
- MediatR (CQRS)
- AutoMapper (Mapping)
- FluentValidation (Validation)

---

### 3.4 L3 — CrossCutting (`VersaCoder.CrossCutting`)

**Sorumluluk:** Logging, exception handling, validation, cross-cutting concerns

**Bağımlılık:** Yok (bağımsız)

**İçerik:**

```
VersaCoder.CrossCutting/
├── Logging/
│   ├── SerilogConfiguration.cs
│   ├── FileLogger.cs
│   └── ConsoleLogger.cs
│
├── ExceptionHandling/
│   ├── GlobalExceptionHandler.cs
│   ├── DomainException.cs
│   ├── ValidationException.cs
│   └── NotFoundException.cs
│
├── Validation/
│   ├── ValidationBehavior.cs        # MediatR pipeline
│   └── FluentValidationExtensions.cs
│
├── Behaviors/
│   ├── LoggingBehavior.cs           # MediatR pipeline
│   ├── PerformanceBehavior.cs       # MediatR pipeline
│   └── CachingBehavior.cs           # MediatR pipeline
│
├── Attributes/
│   ├── LogAttribute.cs
│   └── CacheAttribute.cs
│
└── Extensions/
    ├── ServiceCollectionExtensions.cs
    └── StringExtensions.cs
```

**NuGet Paketleri:**
- Serilog
- Serilog.Sinks.File
- FluentValidation

---

### 3.5 L4 — Infrastructure (27 Modül)

Her modül bağımsız bir DLL olarak paketlenir.

#### L4.1 — Infrastructure.Data

```
VersaCoder.Infrastructure.Data/
├── Context/
│   └── VersaCoderDbContext.cs
├── Configurations/
│   ├── SessionConfiguration.cs
│   ├── PromptConfiguration.cs
│   ├── ConversationConfiguration.cs
│   └── LearningEntryConfiguration.cs
├── Repositories/
│   ├── SessionRepository.cs
│   ├── PromptRepository.cs
│   ├── ConversationRepository.cs
│   └── LearningRepository.cs
├── Migrations/
│   └── (EF Core migrations)
└── Extensions/
    └── DbContextExtensions.cs
```

**NuGet:** Microsoft.EntityFrameworkCore.Sqlite, Microsoft.EntityFrameworkCore.Design

#### L4.2 — Infrastructure.AI

```
VersaCoder.Infrastructure.AI/
├── Providers/
│   ├── OpenAIProvider.cs
│   ├── AnthropicProvider.cs
│   ├── GoogleProvider.cs
│   ├── OllamaProvider.cs
│   └── CustomProvider.cs
├── Router/
│   └── ProviderRouter.cs
├── Agents/
│   ├── AgentRunner.cs
│   ├── AgentSelector.cs
│   └── AgentFactory.cs
├── Tools/
│   ├── ToolRegistry.cs
│   ├── ToolExecutor.cs
│   └── BuiltInTools/
│       ├── ReadFileTool.cs
│       ├── WriteFileTool.cs
│       ├── EditFileTool.cs
│       ├── GlobTool.cs
│       ├── GrepTool.cs
│       ├── BashTool.cs
│       ├── GitTool.cs
│       └── (45+ tool)
└── Tokenizers/
    └── TokenCounter.cs
```

**NuGet:** Microsoft.SemanticKernel, OpenAI, Anthropic SDK

#### L4.3 — Infrastructure.MCP

```
VersaCoder.Infrastructure.MCP/
├── Client/
│   ├── McpClient.cs
│   ├── McpServer.cs
│   └── McpMessageHandler.cs
├── Resources/
│   ├── ResourceProvider.cs
│   └── ResourceManager.cs
└── Tools/
    ├── McpToolProvider.cs
    └── McpToolExecutor.cs
```

#### L4.4 — Infrastructure.Auth

```
VersaCoder.Infrastructure.Auth/
├── Services/
│   ├── ApiKeyManager.cs
│   ├── CredentialVault.cs
│   └── TokenService.cs
├── Models/
│   ├── ApiKey.cs
│   └── Credential.cs
└── Storage/
    └── CredentialStorage.cs
```

#### L4.5 — Infrastructure.Config

```
VersaCoder.Infrastructure.Config/
├── Settings/
│   ├── AppSettings.cs
│   ├── AiSettings.cs
│   ├── DatabaseSettings.cs
│   └── UiSettings.cs
├── Providers/
│   ├── SettingsProvider.cs
│   └── EnvironmentProvider.cs
└── Extensions/
    └── ConfigurationExtensions.cs
```

**NuGet:** Microsoft.Extensions.Configuration.Json

#### L4.6 — Infrastructure.Plugins

```
VersaCoder.Infrastructure.Plugins/
├── Loader/
│   ├── PluginLoader.cs
│   └── PluginResolver.cs
├── Manager/
│   ├── PluginManager.cs
│   └── PluginRegistry.cs
└── Base/
    ├── PluginBase.cs
    └── ToolPluginBase.cs
```

#### L4.7 — Infrastructure.Services

```
VersaCoder.Infrastructure.Services/
├── Markdown/
│   └── MarkdownProcessor.cs         # Markdig
├── Json/
│   └── JsonSerializationService.cs
├── Hash/
│   └── HashService.cs
└── FileWatcher/
    └── FileWatcherService.cs
```

#### L4.8 — Infrastructure.Caching

```
VersaCoder.Infrastructure.Caching/
├── MemoryCache/
│   └── MemoryCacheService.cs
├── FileCache/
│   └── FileCacheService.cs
└── Strategies/
    └── CacheStrategy.cs
```

#### L4.9 — Infrastructure.Messaging

```
VersaCoder.Infrastructure.Messaging/
├── EventBus/
│   ├── InMemoryEventBus.cs
│   └── EventSubscription.cs
├── Handlers/
│   └── DomainEventHandlers.cs
└── Models/
    └── EventMessage.cs
```

#### L4.10 — Infrastructure.FileSystem

```
VersaCoder.Infrastructure.FileSystem/
├── Services/
│   ├── FileSystemService.cs
│   ├── DirectoryService.cs
│   └── PathService.cs
└── Watchers/
    └── FileSystemWatcher.cs
```

#### L4.11 — Infrastructure.Network

```
VersaCoder.Infrastructure.Network/
├── HttpClient/
│   ├── HttpService.cs
│   └── HttpClientFactory.cs
├── WebSocket/
│   ├── WebSocketClient.cs
│   └── WebSocketManager.cs
└── Retry/
    └── PollyRetryService.cs
```

**NuGet:** Polly, System.Net.WebSockets

#### L4.12 — Infrastructure.Security

```
VersaCoder.Infrastructure.Security/
├── Encryption/
│   ├── AesEncryptionService.cs
│   └── HashService.cs
├── Token/
│   ├── JwtTokenService.cs
│   └── TokenValidator.cs
└── Sanitization/
    └── InputSanitizer.cs
```

#### L4.13 — Infrastructure.Observability

```
VersaCoder.Infrastructure.Observability/
├── Metrics/
│   ├── MetricsCollector.cs
│   └── PerformanceCounter.cs
├── Health/
│   ├── HealthCheckService.cs
│   └── HealthReport.cs
└── Telemetry/
    └── TelemetryService.cs
```

#### L4.14 — Infrastructure.Context

```
VersaCoder.Infrastructure.Context/
├── Assembly/
│   ├── ContextAssembler.cs
│   ├── ContextPrioritizer.cs
│   └── ContextCompressor.cs
├── Sources/
│   ├── ProjectContextProvider.cs
│   ├── FileContextProvider.cs
│   ├── SessionContextProvider.cs
│   ├── LearningContextProvider.cs
│   └── DiagramContextProvider.cs
├── Epochs/
│   ├── EpochManager.cs
│   └── EpochHistory.cs
└── Storage/
    └── ContextStorage.cs
```

#### L4.15 — Infrastructure.Learning

```
VersaCoder.Infrastructure.Learning/
├── Patterns/
│   ├── PatternRecognizer.cs
│   ├── CodePatternStore.cs
│   └── ArchitecturePatternStore.cs
├── Corrections/
│   ├── CorrectionTracker.cs
│   └── CorrectionApplier.cs
├── Knowledge/
│   ├── KnowledgeBase.cs
│   └── KnowledgeIndexer.cs
└── Rules/
    ├── RuleEngine.cs
    └── RuleConflictResolver.cs
```

#### L4.16 — Infrastructure.Diagram

```
VersaCoder.Infrastructure.Diagram/
├── Parsers/
│   ├── MermaidParser.cs
│   ├── PlantUMLParser.cs
│   └── DrawIOParser.cs
├── Converters/
│   ├── DiagramToContextConverter.cs
│   └── DiagramToCodeConverter.cs
├── Teachers/
│   └── DiagramAITeacher.cs
└── Renderers/
    ├── MermaidRenderer.cs
    └── PlantUMLRenderer.cs
```

#### L4.17 — Infrastructure.ProjectAnalysis

```
VersaCoder.Infrastructure.ProjectAnalysis/
├── Indexer/
│   ├── ProjectIndexer.cs
│   ├── FileIndexer.cs
│   ├── ClassIndexer.cs
│   └── MethodIndexer.cs
├── Analyzers/
│   ├── CodeAnalyzer.cs
│   ├── ArchitectureAnalyzer.cs
│   ├── DependencyAnalyzer.cs
│   └── QualityAnalyzer.cs
└── Reports/
    ├── AnalysisReport.cs
    └── QualityReport.cs
```

#### L4.18 — Infrastructure.Testing

```
VersaCoder.Infrastructure.Testing/
├── Runner/
│   ├── TestRunner.cs
│   └── CoverageAnalyzer.cs
├── Generators/
│   ├── TestGenerator.cs
│   └── MockGenerator.cs
└── Reports/
    └── TestReport.cs
```

#### L4.19 — Infrastructure.Documentation

```
VersaCoder.Infrastructure.Documentation/
├── Generators/
│   ├── ReadmeGenerator.cs
│   ├── ApiDocGenerator.cs
│   └── ChangelogGenerator.cs
├── Templates/
│   └── DocumentationTemplates.cs
└── Parsers/
    └── MarkdownParser.cs
```

#### L4.20 — Infrastructure.Refactoring

```
VersaCoder.Infrastructure.Refactoring/
├── Analyzers/
│   ├── CodeSmellAnalyzer.cs
│   └── DuplicateCodeAnalyzer.cs
├── Strategies/
│   ├── ExtractMethodStrategy.cs
│   ├── RenameStrategy.cs
│   └── MoveStrategy.cs
└── Executors/
    └── RefactoringExecutor.cs
```

#### L4.21 — Infrastructure.CodeAnalysis

```
VersaCoder.Infrastructure.CodeAnalysis/
├── Syntax/
│   ├── SyntaxAnalyzer.cs
│   └── SyntaxTreeWalker.cs
├── Metrics/
│   ├── ComplexityAnalyzer.cs
│   ├── CouplingAnalyzer.cs
│   └── CohesionAnalyzer.cs
└── Reports/
    └── CodeAnalysisReport.cs
```

#### L4.22 — Infrastructure.Git

```
VersaCoder.Infrastructure.Git/
├── Services/
│   ├── GitService.cs
│   ├── GitRepository.cs
│   └── GitDiffService.cs
├── Models/
│   ├── GitCommit.cs
│   ├── GitBranch.cs
│   └── GitDiff.cs
└── Extensions/
    └── LibGit2SharpExtensions.cs
```

**NuGet:** LibGit2Sharp

#### L4.23 — Infrastructure.Integration

```
VersaCoder.Infrastructure.Integration/
├── Providers/
│   ├── GitHubIntegration.cs
│   ├── NuGetIntegration.cs
│   └── AzureDevOpsIntegration.cs
└── Models/
    └── IntegrationConfig.cs
```

#### L4.24 — Infrastructure.Templating

```
VersaCoder.Infrastructure.Templating/
├── Engine/
│   ├── TemplateEngine.cs
│   ├── TemplateRenderer.cs
│   └── TemplateRegistry.cs
├── Templates/
│   ├── ClassTemplate.cs
│   ├── ServiceTemplate.cs
│   ├── RepositoryTemplate.cs
│   ├── ViewModelTemplate.cs
│   └── TestTemplate.cs
└── Storage/
    └── TemplateStorage.cs
```

#### L4.25 — Infrastructure.Deployment

```
VersaCoder.Infrastructure.Deployment/
├── Builders/
│   ├── BuildService.cs
│   └── PublishService.cs
├── Scripts/
│   └── DeploymentScripts.cs
└── Reports/
    └── DeploymentReport.cs
```

#### L4.26 — Infrastructure.Backup

```
VersaCoder.Infrastructure.Backup/
├── Services/
│   ├── BackupService.cs
│   ├── RestoreService.cs
│   └── BackupScheduler.cs
├── Strategies/
│   ├── FullBackupStrategy.cs
│   └── IncrementalBackupStrategy.cs
└── Storage/
    └── BackupStorage.cs
```

#### L4.27 — Infrastructure.Versioning

```
VersaCoder.Infrastructure.Versioning/
├── Services/
│   ├── VersionService.cs
│   ├── SemanticVersioning.cs
│   └── ChangelogGenerator.cs
└── Models/
    └── VersionInfo.cs
```

---

### 3.6 L5 — Protocol (`VersaCoder.Protocol`)

**Sorumluluk:** AI protokolü, MCP iletişimi, provider haberleşmesi

```
VersaCoder.Protocol/
├── Messages/
│   ├── ProtocolMessage.cs
│   ├── ChatMessage.cs
│   ├── ToolCallMessage.cs
│   └── ToolResultMessage.cs
├── Serialization/
│   └── MessageSerializer.cs
└── Handlers/
    └── ProtocolHandler.cs
```

---

### 3.7 L6 — Host (`VersaCoder.Host`)

**Sorumluluk:** Uygulama başlangıcı, DI, konfigürasyon

```
VersaCoder.Host/
├── Startup/
│   ├── ApplicationHost.cs
│   ├── ServiceConfiguration.cs
│   └── MiddlewareConfiguration.cs
├── DI/
│   ├── DependencyInjectionContainer.cs
│   └── ServiceRegistration.cs
└── Configuration/
    └── ConfigurationManager.cs
```

**NuGet:** Microsoft.Extensions.DependencyInjection, Microsoft.Extensions.Configuration

---

### 3.8 L7 — UI (`VersaCoder.UI`)

**Sorumluluk:** DevExpress WinForms, Ribbon, Tabbed MDI, MVVM

```
VersaCoder.UI/
├── Forms/
│   ├── MainForm.cs                  # Ana form (Ribbon + MDI)
│   ├── MainForm.Designer.cs
│   └── SplashForm.cs               # Başlangıç ekranı
│
├── Views/
│   ├── ChatView.cs                  # AI sohbet görünümü
│   ├── CodeEditorView.cs            # Kod editörü
│   ├── TerminalView.cs              # Terminal görünümü
│   ├── FileExplorerView.cs          # Dosya gezgini
│   ├── SolutionExplorerView.cs      # Çözüm gezgini
│   ├── ContextView.cs              # Context görünümü
│   ├── SessionView.cs              # Session görünümü
│   ├── DiagramView.cs              # Diyagram görünümü
│   ├── SettingsView.cs             # Ayarlar görünümü
│   └── OutputView.cs               # Çıktı görünümü
│
├── ViewModels/
│   ├── MainViewModel.cs
│   ├── ChatViewModel.cs
│   ├── CodeEditorViewModel.cs
│   ├── TerminalViewModel.cs
│   ├── FileExplorerViewModel.cs
│   ├── SolutionExplorerViewModel.cs
│   ├── ContextViewModel.cs
│   ├── SessionViewModel.cs
│   ├── DiagramViewModel.cs
│   ├── SettingsViewModel.cs
│   └── OutputViewModel.cs
│
├── Controls/
│   ├── ChatBubbleControl.cs         # Sohbet baloncuğu
│   ├── CodeEditorControl.cs         # Kod editörü kontrolü
│   ├── MarkdownRenderer.cs          # Markdown görüntüleyici
│   ├── StreamingTextControl.cs      # Akışlı metin
│   └── SyntaxHighlighter.cs         # Sözdizimi vurgulama
│
├── Themes/
│   ├── ThemeManager.cs
│   ├── DarkTheme.cs
│   └── LightTheme.cs
│
├── Resources/
│   ├── Icons/
│   ├── Images/
│   └── Localization/
│
└── Extensions/
    ├── ControlExtensions.cs
    └── FormExtensions.cs
```

**NuGet:** DevExpress.Win.Design, CommunityToolkit.Mvvm

---

## 4. NuGet Paket Özeti

### Core Packages

| Paket | Versiyon | Amaç |
|-------|----------|------|
| DevExpress.Win.Design | 2026.x | WinForms UI |
| CommunityToolkit.Mvvm | 8.x | MVVM |
| MediatR | 12.x | CQRS |
| AutoMapper | 13.x | Mapping |
| FluentValidation | 11.x | Validation |
| Serilog | 4.x | Logging |
| Serilog.Sinks.File | 6.x | File logging |
| Microsoft.EntityFrameworkCore.Sqlite | 8.0 | SQLite ORM |
| LibGit2Sharp | 0.30.x | Git |
| Markdig | 0.37.x | Markdown |
| Polly | 8.x | Resilience |
| Microsoft.Extensions.DependencyInjection | 8.0 | DI |
| Microsoft.Extensions.Configuration.Json | 8.0 | Config |
| System.Text.Json | 8.0 | JSON |

### AI Packages

| Paket | Versiyon | Amaç |
|-------|----------|------|
| OpenAI | 2.x | OpenAI API |
| Anthropic | 0.x | Anthropic API |
| Microsoft.SemanticKernel | 1.x | AI orchestration |

### Test Packages

| Paket | Versiyon | Amaç |
|-------|----------|------|
| xunit | 2.x | Testing |
| xunit.runner.visualstudio | 2.x | Test runner |
| Moq | 4.x | Mocking |
| FluentAssertions | 7.x | Assertions |
|coverlet.collector | 6.x | Coverage |

---

## 5. Uygulama Sırası

| Sıra | Katman | Modül | Tahmini Süre |
|------|--------|-------|-------------|
| 1 | L0 | Domain | 1 hafta |
| 2 | L1 | Abstractions | 1 hafta |
| 3 | L3 | CrossCutting | 1 hafta |
| 4 | L2 | Application | 2 hafta |
| 5 | L4.1 | Infrastructure.Data | 2 hafta |
| 6 | L4.5 | Infrastructure.Config | 1 gün |
| 7 | L4.7 | Infrastructure.Services | 1 hafta |
| 8 | L4.10 | Infrastructure.FileSystem | 3 gün |
| 9 | L4.11 | Infrastructure.Network | 1 hafta |
| 10 | L4.12 | Infrastructure.Security | 1 hafta |
| 11 | L4.22 | Infrastructure.Git | 1 hafta |
| 12 | L4.2 | Infrastructure.AI | 3 hafta |
| 13 | L4.3 | Infrastructure.MCP | 2 hafta |
| 14 | L4.4 | Infrastructure.Auth | 1 hafta |
| 15 | L4.14 | Infrastructure.Context | 2 hafta |
| 16 | L4.15 | Infrastructure.Learning | 2 hafta |
| 17 | L4.16 | Infrastructure.Diagram | 1 hafta |
| 18 | L4.17 | Infrastructure.ProjectAnalysis | 1 hafta |
| 19 | L4.24 | Infrastructure.Templating | 1 hafta |
| 20 | L4.13 | Infrastructure.Observability | 3 gün |
| 21 | L4.8 | Infrastructure.Caching | 2 gün |
| 22 | L4.9 | Infrastructure.Messaging | 3 gün |
| 23 | L4.6 | Infrastructure.Plugins | 1 hafta |
| 24 | L4.18 | Infrastructure.Testing | 1 hafta |
| 25 | L4.19 | Infrastructure.Documentation | 3 gün |
| 26 | L4.20 | Infrastructure.Refactoring | 1 hafta |
| 27 | L4.21 | Infrastructure.CodeAnalysis | 1 hafta |
| 28 | L4.23 | Infrastructure.Integration | 3 gün |
| 29 | L4.25 | Infrastructure.Deployment | 2 gün |
| 30 | L4.26 | Infrastructure.Backup | 2 gün |
| 31 | L4.27 | Infrastructure.Versioning | 2 gün |
| 32 | L5 | Protocol | 1 hafta |
| 33 | L6 | Host | 1 hafta |
| 34 | L7 | UI | 4 hafta |
| 35 | Tests | Test projeleri | Sürekli |

**Toplam Tahmini Süre:** ~20 hafta (5 ay)

---

## 6. Katman Bağımlılık Grafikleri

### 6.1 Temel Akış

```
Kullanıcı (UI) → Host → Protocol → Application → Abstractions → Domain
                                            ↓
                                    Infrastructure (27 modül)
                                            ↓
                                    CrossCutting (logging, validation)
```

### 6.2 AI Akışı

```
Kullanıcı Promptu
    → L7 UI (ChatViewModel)
        → L6 Host (ApplicationHost)
            → L2 Application (SendPromptHandler)
                → L5 Protocol (ProtocolHandler)
                    → L4 Infrastructure.AI (AgentRunner)
                        → L4 Infrastructure.AI (ProviderRouter)
                            → L4 Infrastructure.AI (OpenAI/Anthropic/etc.)
                        ← L4 Infrastructure.AI (ToolExecutor)
                    ← L5 Protocol (ProtocolMessage)
                ← L2 Application (ResponseDto)
            ← L6 Host
        ← L7 UI (StreamingTextControl)
```

### 6.3 Context Assembly Akışı

```
Session Başlangıcı
    → L4 Infrastructure.Context (ContextAssembler)
        → L4 Infrastructure.Context.Sources (ProjectContextProvider)
        → L4 Infrastructure.Context.Sources (FileContextProvider)
        → L4 Infrastructure.Context.Sources (SessionContextProvider)
        → L4 Infrastructure.Context.Sources (LearningContextProvider)
        → L4 Infrastructure.Context.Sources (DiagramContextProvider)
    ← L4 Infrastructure.Context (ContextPrioritizer)
    ← L4 Infrastructure.Context (ContextCompressor)
```

---

## 7. Quality Report

| Metrik | Değer |
|--------|-------|
| Version | 1.0.0 |
| Total Layers | 30 |
| Total DLLs | 30 |
| Total NuGet Packages | ~20 |
| Estimated Timeline | 20 weeks |
| Architecture Pattern | Clean Architecture + DDD + MVVM |
| CQRS Framework | MediatR |
| ORM | EF Core (DbContext ONLY) |
| UI Framework | DevExpress WinForms |
| Test Framework | xUnit |

---

**Authority:** Vault Steward
**Last Updated:** 2026-08-25
**Mode:** Red Team · Human Mode · Truth Mode
