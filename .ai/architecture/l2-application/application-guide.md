---
title: "Versa Coder — L2 Application Layer Guide"
type: architecture
category: layer
layer: L2
date: 2026-08-25
updated: 2026-08-25
status: active
version: 1.0.0
---

# Versa Coder — L2 Application Layer Guide

**Zorunlu Bağlantılar:** [[CLAUDE.md]] · [[architecture/l1-abstractions/abstractions-guide]] · [[brain.md]]

---

## 1. Amaç

Application katmanı, **use case'leri, command/query'leri ve DTO'ları** içerir. CQRS pattern'ı MediatR ile uygulanır.

---

## 2. Commands (CQRS Write)

| Command | Dosya | Tanım |
|---------|-------|-------|
| `CreateSessionCommand` | `VersaCoder.Application/Commands/CreateSessionCommand.cs` | Yeni oturum oluştur |
| `SendPromptCommand` | `VersaCoder.Application/Commands/SendPromptCommand.cs` | Prompt gönder |
| `BranchSessionCommand` | `VersaCoder.Application/Commands/BranchSessionCommand.cs` | Oturum dalı oluştur |
| `CompleteSessionCommand` | `VersaCoder.Application/Commands/CompleteSessionCommand.cs` | Oturumu tamamla |
| `CreateProjectCommand` | `VersaCoder.Application/Commands/CreateProjectCommand.cs` | Proje oluştur |
| `RecordLearningCommand` | `VersaCoder.Application/Commands/RecordLearningCommand.cs` | Öğrenme kaydet |

---

## 3. Queries (CQRS Read)

| Query | Dosya | Tanım |
|-------|-------|-------|
| `GetSessionQuery` | `VersaCoder.Application/Queries/GetSessionQuery.cs` | Tek oturum al |
| `GetSessionMessagesQuery` | `VersaCoder.Application/Queries/GetSessionMessagesQuery.cs` | Oturum mesajlarını al |
| `GetProjectQuery` | `VersaCoder.Application/Queries/GetProjectQuery.cs` | Tek proje al |
| `GetContextQuery` | `VersaCoder.Application/Queries/GetContextQuery.cs` | Context al |
| `GetAllSessionsQuery` | `VersaCoder.Application/Queries/GetAllSessionsQuery.cs` | Tüm oturumları al |
| `GetAllProjectsQuery` | `VersaCoder.Application/Queries/GetAllProjectsQuery.cs` | Tüm projeleri al |

---

## 4. Handlers

| Handler | Command/Query | Tanım |
|---------|---------------|-------|
| `CreateSessionHandler` | `CreateSessionCommand` | Session oluştur |
| `SendPromptHandler` | `SendPromptCommand` | Prompt işle |
| `BranchSessionHandler` | `BranchSessionCommand` | Dal oluştur |
| `CompleteSessionHandler` | `CompleteSessionCommand` | Tamamla |
| `CreateProjectHandler` | `CreateProjectCommand` | Proje oluştur |
| `RecordLearningHandler` | `RecordLearningCommand` | Öğrenme kaydet |
| `GetSessionHandler` | `GetSessionQuery` | Session oku |
| `GetSessionMessagesHandler` | `GetSessionMessagesQuery` | Mesajları oku |

---

## 5. Services

| Servis | Dosya | Tanım |
|--------|-------|-------|
| `AgentSelectorService` | `VersaCoder.Application/Services/AgentSelectorService.cs` | Agent seçimi |
| `ContextManagerService` | `VersaCoder.Application/Services/ContextManagerService.cs` | Context yönetimi |
| `DiagramTeacherService` | `VersaCoder.Application/Services/DiagramTeacherService.cs` | Diyagram öğretme |
| `GitService` | `VersaCoder.Application/Services/GitService.cs` | Git işlemleri |
| `LearningService` | `VersaCoder.Application/Services/LearningService.cs` | Öğrenme |
| `ProjectAnalyzerService` | `VersaCoder.Application/Services/ProjectAnalyzerService.cs` | Proje analizi |
| `SessionManagerService` | `VersaCoder.Application/Services/SessionManagerService.cs` | Session yönetimi |
| `TemplateService` | `VersaCoder.Application/Services/TemplateService.cs` | Şablon sistemi |

---

## 6. DTO'lar

| DTO | Dosya | Tanım |
|-----|-------|-------|
| `SessionDto` | `VersaCoder.Application/DTOs/SessionDto.cs` | Session verisi |
| `MessageDto` | `VersaCoder.Application/DTOs/MessageDto.cs` | Mesaj verisi |
| `ProjectDto` | `VersaCoder.Application/DTOs/ProjectDto.cs` | Proje verisi |
| `ContextDto` | `VersaCoder.Application/DTOs/ContextDto.cs` | Context verisi |
| `AgentDto` | `VersaCoder.Application/DTOs/AgentDto.cs` | Agent verisi |

---

## 7. Ortak Yapılar

| Yapı | Dosya | Tanım |
|------|-------|-------|
| `Result<T>` | `VersaCoder.Application/Common/Result.cs` | Monad pattern (Success/Failure) |
| `PaginatedList<T>` | `VersaCoder.Application/Common/PaginatedList.cs` | Sayfalı liste |

---

## 8. Kurallar

| # | Kural |
|---|-------|
| 1 | Handler'lar MediatR `IRequestHandler` implemente eder |
| 2 | Validation FluentValidation ile |
| 3 | Business logic Domain'e ait — Application sadece orkestra eder |
| 4 | DTO'lar simple data transfer — behavior yok |

---

## 9. Command Detayları

### 9.1 CreateSessionCommand

```csharp
public record CreateSessionCommand(
    string Name,
    Guid ProjectId,
    string? BranchName = null) : IRequest<Result<Guid>>;

// Handler
public class CreateSessionHandler : IRequestHandler<CreateSessionCommand, Result<Guid>>
{
    private readonly ISessionRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateSessionHandler> _logger;
    
    public CreateSessionHandler(
        ISessionRepository repository,
        IUnitOfWork unitOfWork,
        ILogger<CreateSessionHandler> logger)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }
    
    public async Task<Result<Guid>> Handle(
        CreateSessionCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var session = new Session(request.Name, request.ProjectId);
            
            await _repository.AddAsync(session, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            _logger.LogInformation("Session created: {SessionId}", session.Id);
            
            return Result<Guid>.Success(session.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create session");
            return Result<Guid>.Failure(ex.Message);
        }
    }
}
```

### 9.2 SendPromptCommand

```csharp
public record SendPromptCommand(
    Guid SessionId,
    string Prompt,
    string ModelName) : IRequest<Result<MessageDto>>;

// Handler
public class SendPromptHandler : IRequestHandler<SendPromptCommand, Result<MessageDto>>
{
    private readonly ISessionRepository _sessionRepository;
    private readonly IMessageRepository _messageRepository;
    private readonly ILLMProvider _llmProvider;
    private readonly IContextManager _contextManager;
    private readonly IUnitOfWork _unitOfWork;
    
    public async Task<Result<MessageDto>> Handle(
        SendPromptCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            // Session'ı getir
            var session = await _sessionRepository.GetByIdAsync(
                request.SessionId, cancellationToken);
            
            if (session == null)
                return Result<MessageDto>.Failure("Session not found");
            
            // Context'i hazırla
            var context = await _contextManager.GetCurrentContextAsync();
            
            // User mesajını kaydet
            var userMessage = new Message(
                request.SessionId, "user", request.Prompt, 0);
            await _messageRepository.AddAsync(userMessage, cancellationToken);
            
            // LLM'den yanıt al
            var llmRequest = new LLMRequest
            {
                Messages = context.Sources.Select(s => s.Content).ToList(),
                Model = request.ModelName
            };
            
            var llmResponse = await _llmProvider.CompleteAsync(
                llmRequest, cancellationToken);
            
            // Assistant mesajını kaydet
            var assistantMessage = new Message(
                request.SessionId, "assistant", llmResponse.Content, llmResponse.TokenCount);
            await _messageRepository.AddAsync(assistantMessage, cancellationToken);
            
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            return Result<MessageDto>.Success(assistantMessage.ToDto());
        }
        catch (Exception ex)
        {
            return Result<MessageDto>.Failure(ex.Message);
        }
    }
}
```

---

## 10. Query Detayları

### 10.1 GetSessionQuery

```csharp
public record GetSessionQuery(Guid SessionId) : IRequest<Result<SessionDto?>>;

// Handler
public class GetSessionHandler : IRequestHandler<GetSessionQuery, Result<SessionDto?>>
{
    private readonly ISessionRepository _repository;
    
    public GetSessionHandler(ISessionRepository repository)
    {
        _repository = repository;
    }
    
    public async Task<Result<SessionDto?>> Handle(
        GetSessionQuery request,
        CancellationToken cancellationToken)
    {
        var session = await _repository.GetByIdAsync(
            request.SessionId, cancellationToken);
        
        return Result<SessionDto?>.Success(session?.ToDto());
    }
}
```

### 10.2 GetSessionMessagesQuery

```csharp
public record GetSessionMessagesQuery(
    Guid SessionId,
    int PageNumber = 1,
    int PageSize = 50) : IRequest<Result<PaginatedList<MessageDto>>>;

// Handler
public class GetSessionMessagesHandler : IRequestHandler<
    GetSessionMessagesQuery, Result<PaginatedList<MessageDto>>>
{
    private readonly IMessageRepository _repository;
    
    public GetSessionMessagesHandler(IMessageRepository repository)
    {
        _repository = repository;
    }
    
    public async Task<Result<PaginatedList<MessageDto>>> Handle(
        GetSessionMessagesQuery request,
        CancellationToken cancellationToken)
    {
        var messages = await _repository.GetBySessionIdAsync(
            request.SessionId, cancellationToken);
        
        var pagedMessages = messages
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(m => m.ToDto())
            .ToList();
        
        return Result<PaginatedList<MessageDto>>.Success(
            new PaginatedList<MessageDto>(
                pagedMessages,
                messages.Count,
                request.PageNumber,
                request.PageSize));
    }
}
```

---

## 11. Service Detayları

### 11.1 AgentSelectorService

```csharp
public class AgentSelectorService : IAgentSelectorService
{
    private readonly Dictionary<AgentRole, List<string>> _agentKeywords;
    
    public AgentSelectorService()
    {
        _agentKeywords = new Dictionary<AgentRole, List<string>>
        {
            [AgentRole.Build] = new List<string>
            {
                "kod", "class", "method", "property", "service", "repository"
            },
            [AgentRole.Plan] = new List<string>
            {
                "plan", "mimari", "task", "phase", "milestone"
            },
            [AgentRole.Explore] = new List<string>
            {
                "analiz", "tarama", "grep", "glob", "dosya bul"
            },
            [AgentRole.Summary] = new List<string>
            {
                "doc", "özet", "dokümantasyon", "markdown"
            },
            [AgentRole.Title] = new List<string>
            {
                "başlık", "isim", "naming", "convention"
            }
        };
    }
    
    public AgentRole SelectAgent(string userPrompt)
    {
        var prompt = userPrompt.ToLowerInvariant();
        
        foreach (var agent in _agentKeywords)
        {
            if (agent.Value.Any(keyword => prompt.Contains(keyword)))
                return agent.Key;
        }
        
        return AgentRole.General;
    }
}
```

### 11.2 ContextManagerService

```csharp
public class ContextManagerService : IContextManager
{
    private readonly IContextRepository _repository;
    private readonly ISessionRepository _sessionRepository;
    
    public async Task<Context> GetCurrentContextAsync()
    {
        // Mevcut context'i yükle
        var context = await _repository.GetLatestAsync();
        return context ?? CreateDefaultContext();
    }
    
    public async Task UpdateContextAsync(ContextUpdate update)
    {
        var context = await GetCurrentContextAsync();
        
        // Context'i güncelle
        foreach (var source in update.Sources)
        {
            context.Sources.Add(source);
        }
        
        foreach (var file in update.ActiveFiles)
        {
            if (!context.ActiveFiles.Contains(file))
                context.ActiveFiles.Add(file);
        }
        
        await _repository.SaveAsync(context);
    }
    
    private Context CreateDefaultContext()
    {
        return new Context
        {
            SessionId = Guid.NewGuid(),
            Sources = new List<ContextSource>(),
            ActiveFiles = new List<string>(),
            CurrentTask = string.Empty,
            Metadata = new Dictionary<string, object>()
        };
    }
}
```

---

## 12. Validation Detayları

### 12.1 FluentValidation Kullanımı

```csharp
// CreateSessionCommand validator
public class CreateSessionCommandValidator : AbstractValidator<CreateSessionCommand>
{
    public CreateSessionCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Session name is required")
            .MaximumLength(100).WithMessage("Session name cannot exceed 100 characters");
        
        RuleFor(x => x.ProjectId)
            .NotEmpty().WithMessage("Project ID is required");
    }
}

// SendPromptCommand validator
public class SendPromptCommandValidator : AbstractValidator<SendPromptCommand>
{
    public SendPromptCommandValidator()
    {
        RuleFor(x => x.SessionId)
            .NotEmpty().WithMessage("Session ID is required");
        
        RuleFor(x => x.Prompt)
            .NotEmpty().WithMessage("Prompt is required")
            .MaximumLength(10000).WithMessage("Prompt cannot exceed 10000 characters");
        
        RuleFor(x => x.ModelName)
            .NotEmpty().WithMessage("Model name is required");
    }
}
```

### 12.2 Validation Behavior

```csharp
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;
    
    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }
    
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (_validators.Any())
        {
            var context = new ValidationContext<TRequest>(request);
            var validationResults = await Task.WhenAll(
                _validators.Select(v => v.ValidateAsync(context, cancellationToken)));
            
            var failures = validationResults
                .SelectMany(r => r.Errors)
                .Where(f => f != null)
                .ToList();
            
            if (failures.Any())
                throw new ValidationException(failures);
        }
        
        return await next();
    }
}
```

---

## 13. Result Pattern

### 13.1 Result<T> Implementasyonu

```csharp
public class Result<T>
{
    public bool IsSuccess { get; }
    public T? Value { get; }
    public string? Error { get; }
    
    private Result(bool isSuccess, T? value, string? error)
    {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
    }
    
    public static Result<T> Success(T value) => new Result<T>(true, value, null);
    
    public static Result<T> Failure(string error) => new Result<T>(false, default, error);
    
    public Result<TNew> Map<TNew>(Func<T, TNew> map)
    {
        return IsSuccess
            ? Result<TNew>.Success(map(Value!))
            : Result<TNew>.Failure(Error!);
    }
    
    public async Task<Result<TNew>> MapAsync<TNew>(Func<T, Task<TNew>> map)
    {
        return IsSuccess
            ? Result<TNew>.Success(await map(Value!))
            : Result<TNew>.Failure(Error!);
    }
}
```

---

## 14. Application Testleri

### 14.1 Handler Testleri

```csharp
public class CreateSessionHandlerTests
{
    private readonly Mock<ISessionRepository> _repositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILogger<CreateSessionHandler>> _loggerMock;
    private readonly CreateSessionHandler _handler;
    
    public CreateSessionHandlerTests()
    {
        _repositoryMock = new Mock<ISessionRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<CreateSessionHandler>>();
        _handler = new CreateSessionHandler(
            _repositoryMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object);
    }
    
    [Fact]
    public async Task Handle_ValidCommand_ReturnsSuccess()
    {
        // Arrange
        var command = new CreateSessionCommand("Test Session", Guid.NewGuid());
        
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        
        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotEqual(Guid.Empty, result.Value);
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Session>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
```

---

## 15. Application Gelecek Planı

### 15.1 Kısa Vadeli (1-2 hafta)

| Görev | Öncelik |
|-------|---------|
| Yeni command'lar | Yüksek |
| Yeni query'ler | Yüksek |
| Validation güncellemeleri | Orta |

### 15.2 Orta Vadeli (1-2 ay)

| Görev | Öncelik |
|-------|---------|
| CQRS zenginleştirme | Orta |
| Event handler'lar | Orta |
| Integration tests | Düşük |

### 15.3 Uzun Vadeli (3-6 ay)

| Görev | Öncelik |
|-------|---------|
| Event sourcing | Düşük |
| MediatR v12 migration | Orta |
| Performance optimization | Düşük |

---

## 16. Quality Report

| Metrik | Değer |
|--------|-------|
| Version | 1.1.0 |
| Status | Active |
| Commands | 6 |
| Queries | 6 |
| Handlers | 8 |
| Services | 8 |
| DTOs | 5 |
| Validators | 2 |

---

**Authority:** Vault Steward
**Last Updated:** 2026-08-26
**Mode:** Red Team · Human Mode · Truth Mode
