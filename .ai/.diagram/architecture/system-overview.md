---
title: "Versa Coder — System Architecture Diagram"
type: diagram
category: architecture
format: mermaid
version: 1.0.0
---

# System Architecture Diagram

## 1. High-Level Architecture

```mermaid
graph TB
    subgraph "L7 UI Layer"
        UI[DevExpress WinForms]
        Ribbon[Ribbon Menu]
        TabbedMDI[Tabbed MDI]
        CodeEditor[Code Editor]
        ChatPanel[Chat Panel]
    end

    subgraph "L6 Host Layer"
        AppHost[Application Host]
        DI[Dependency Injection]
        Config[Configuration]
    end

    subgraph "L5 Protocol Layer"
        Protocol[AI Protocol]
        MCP[MCP Client/Server]
        Provider[Provider Router]
    end

    subgraph "L4 Infrastructure Layer"
        AI[Infrastructure.AI]
        Data[Infrastructure.Data]
        Context[Infrastructure.Context]
        Learning[Infrastructure.Learning]
        Git[Infrastructure.Git]
        Diagram[Infrastructure.Diagram]
    end

    subgraph "L3 CrossCutting Layer"
        Logging[Logging]
        Validation[Validation]
        Exception[Exception Handling]
    end

    subgraph "L2 Application Layer"
        Commands[CQRS Commands]
        Queries[CQRS Queries]
        Handlers[Handlers]
        DTOs[DTOs]
    end

    subgraph "L1 Abstractions Layer"
        Interfaces[Interfaces]
        Contracts[Contracts]
    end

    subgraph "L0 Domain Layer"
        Entities[Entities]
        ValueObjects[Value Objects]
        Events[Domain Events]
    end

    UI --> AppHost
    AppHost --> Protocol
    Protocol --> AI
    AI --> Data
    AI --> Context
    AI --> Learning
    Data --> Interfaces
    Interfaces --> Entities
```

## 2. AI Provider Flow

```mermaid
sequenceDiagram
    participant User
    participant UI
    participant AgentRunner
    participant ProviderRouter
    participant LLMProvider
    participant ToolExecutor

    User->>UI: Send Prompt
    UI->>AgentRunner: Run Agent
    AgentRunner->>ProviderRouter: Get Provider
    ProviderRouter-->>AgentRunner: Return Provider
    AgentRunner->>LLMProvider: Send Message
    LLMProvider-->>AgentRunner: Return Response
    
    alt Has Tool Calls
        AgentRunner->>ToolExecutor: Execute Tools
        ToolExecutor-->>AgentRunner: Return Results
        AgentRunner->>LLMProvider: Send Tool Results
        LLMProvider-->>AgentRunner: Return Final Response
    end
    
    AgentRunner-->>UI: Return Response
    UI-->>User: Display Response
```

## 3. Session Flow

```mermaid
stateDiagram-v2
    [*] --> Created
    Created --> Active : Start Session
    Active --> Active : Send Prompt
    Active --> Paused : Pause
    Paused --> Active : Resume
    Active --> Completed : Complete
    Active --> Branched : Branch
    Branched --> Active : Switch Branch
    Completed --> [*]
```

## 4. Context Assembly Flow

```mermaid
graph LR
    A[Vault Read] --> B[Learning Load]
    B --> C[Session Load]
    C --> D[Project Analysis]
    D --> E[Context Assembly]
    E --> F[Prompt Input]
    F --> G[AI Output]
    
    style A fill:#f9f,stroke:#333
    style E fill:#bbf,stroke:#333
    style G fill:#bfb,stroke:#333
```

---

**Authority:** Vault Steward  
**Last Updated:** 2026-08-25
