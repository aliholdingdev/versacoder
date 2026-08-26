---
title: "ADR-007 — Session Management Architecture"
type: decision
status: accepted
date: 2026-08-25
version: 1.0.0
---

# ADR-007 — Session Management Architecture

**Status:** Accepted  
**Date:** 2026-08-25  
**Category:** Infrastructure.Context  
**Sorumlu:** Build Agent

---

## 1. Karar

Versa Coder, **branch, fork, merge** destekli, **timeline tabanlı** bir session yönetim sistemi kullanacaktır.

## 2. Bağlam

Kullanıcılar:
- Birden fazla oturum aynı anda yürütebilmeli
- Bir oturumun dalını (branch) oluşturabilmeli
- Bir oturumu başka bir oturuma dönüştürebilmeli (fork)
- İki oturumu birleştirebilmeli (merge)
- Oturum geçmişini zaman çizelgesi olarak görüntüleyebilmeli
- Eski oturumlara dönebilmeli (undo/redo)

## 3. Seçenekler

| Seçenek | Artıları | Eksileri |
|---------|----------|----------|
| **Flat List** | Basit | Geçmiş yönetimi zor |
| **Git-like Branching** | Esnek, güçlü | Karmaşık |
| **Tree Structure** | Doğal hiyerarşi | Merge zor |
| **Timeline** | Basit zaman çizelgesi | Branching yok |

## 4. Karar

**Git-like Branching** + **Timeline** kombinasyonu seçildi.

## 5. Session Modeli

```csharp
public class Session
{
    public SessionId Id { get; set; }
    public string Name { get; set; }
    public SessionState State { get; set; }
    public SessionId? ParentId { get; set; }
    public List<SessionBranch> Branches { get; set; }
    public List<Prompt> Prompts { get; set; }
    public List<Response> Responses { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public Dictionary<string, object> Metadata { get; set; }
}
```

## 6. SessionBranch Modeli

```csharp
public class SessionBranch
{
    public string Name { get; set; }
    public SessionId SourceSessionId { get; set; }
    public SessionId BranchSessionId { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Reason { get; set; }
}
```

## 7. SessionTimeline

```csharp
public class SessionTimeline
{
    public List<TimelineEvent> Events { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    
    public class TimelineEvent
    {
        public DateTime Timestamp { get; set; }
        public string EventType { get; set; }  // CREATED, PROMPT, RESPONSE, BRANCH, FORK, MERGE
        public string Description { get; set; }
        public SessionId? RelatedSessionId { get; set; }
    }
}
```

## 8. Session Operations

### 8.1 Branch (Dal Oluşturma)

```csharp
public async Task<Session> BranchSessionAsync(
    SessionId sourceSessionId,
    string branchName,
    string reason)
{
    // 1. Kaynak oturumu al
    var sourceSession = await _sessionRepo.GetByIdAsync(sourceSessionId);
    
    // 2. Yeni oturum oluştur (kopya)
    var branchSession = new Session
    {
        Id = SessionId.New(),
        Name = $"{sourceSession.Name} - {branchName}",
        ParentId = sourceSessionId,
        State = SessionState.ACTIVE,
        Prompts = new List<Prompt>(sourceSession.Prompts),
        Responses = new List<Response>(sourceSession.Responses)
    };
    
    // 3. Branch kaydı ekle
    sourceSession.Branches.Add(new SessionBranch
    {
        Name = branchName,
        SourceSessionId = sourceSessionId,
        BranchSessionId = branchSession.Id,
        CreatedAt = DateTime.UtcNow,
        Reason = reason
    });
    
    // 4. Timeline'a ekle
    await AddTimelineEventAsync(sourceSessionId, "BRANCH",
        $"Branched to {branchName}");
    
    // 5. Kaydet
    await _sessionRepo.AddAsync(branchSession);
    await _unitOfWork.SaveChangesAsync();
    
    return branchSession;
}
```

### 8.2 Fork (Oturum Dönüştürme)

```csharp
public async Task<Session> ForkSessionAsync(
    SessionId sourceSessionId,
    string newName)
{
    // 1. Kaynak oturumu al
    var sourceSession = await _sessionRepo.GetByIdAsync(sourceSessionId);
    
    // 2. Yeni oturum oluştur (tam kopya)
    var forkSession = new Session
    {
        Id = SessionId.New(),
        Name = newName,
        State = SessionState.ACTIVE,
        Prompts = new List<Prompt>(sourceSession.Prompts),
        Responses = new List<Response>(sourceSession.Responses),
        Metadata = new Dictionary<string, object>(sourceSession.Metadata)
    };
    
    // 3. Timeline'a ekle
    await AddTimelineEventAsync(sourceSessionId, "FORK",
        $"Forked to {newName}");
    
    // 4. Kaydet
    await _sessionRepo.AddAsync(forkSession);
    await _unitOfWork.SaveChangesAsync();
    
    return forkSession;
}
```

### 8.3 Merge (Birleştirme)

```csharp
public async Task<Session> MergeSessionsAsync(
    SessionId sourceSessionId,
    SessionId targetSessionId,
    string mergeStrategy = "ours")
{
    // 1. Oturumları al
    var sourceSession = await _sessionRepo.GetByIdAsync(sourceSessionId);
    var targetSession = await _sessionRepo.GetByIdAsync(targetSessionId);
    
    // 2. Merge stratejisi uygula
    var mergedSession = mergeStrategy switch
    {
        "ours" => MergeOurs(sourceSession, targetSession),
        "theirs" => MergeTheirs(sourceSession, targetSession),
        "combine" => MergeCombine(sourceSession, targetSession),
        _ => throw new ArgumentException($"Unknown strategy: {mergeStrategy}")
    };
    
    // 3. Timeline'a ekle
    await AddTimelineEventAsync(sourceSessionId, "MERGE",
        $"Merged with {targetSessionId}");
    
    // 4. Kaydet
    await _sessionRepo.UpdateAsync(mergedSession);
    await _unitOfWork.SaveChangesAsync();
    
    return mergedSession;
}
```

## 9. Undo/Redo

```csharp
public class SessionUndoRedo
{
    private readonly Stack<SessionSnapshot> _undoStack;
    private readonly Stack<SessionSnapshot> _redoStack;
    
    public void TakeSnapshot(Session session)
    {
        var snapshot = new SessionSnapshot
        {
            SessionId = session.Id,
            State = session.State,
            Prompts = new List<Prompt>(session.Prompts),
            Responses = new List<Response>(session.Responses),
            Timestamp = DateTime.UtcNow
        };
        
        _undoStack.Push(snapshot);
        _redoStack.Clear();
    }
    
    public Session? Undo(Session currentSession)
    {
        if (_undoStack.Count == 0) return null;
        
        var snapshot = _undoStack.Pop();
        _redoStack.Push(CreateSnapshot(currentSession));
        
        return RestoreFromSnapshot(snapshot);
    }
    
    public Session? Redo(Session currentSession)
    {
        if (_redoStack.Count == 0) return null;
        
        var snapshot = _redoStack.Pop();
        _undoStack.Push(CreateSnapshot(currentSession));
        
        return RestoreFromSnapshot(snapshot);
    }
}
```

## 10. Session Storage

```csharp
public class SessionRepository : ISessionRepository
{
    private readonly VersaCoderDbContext _context;
    
    public async Task<Session> GetByIdAsync(SessionId id)
    {
        return await _context.Sessions
            .Include(s => s.Prompts)
            .Include(s => s.Responses)
            .Include(s => s.Branches)
            .FirstOrDefaultAsync(s => s.Id == id);
    }
    
    public async Task<List<Session>> GetByTimelineAsync(
        DateTime startDate,
        DateTime endDate)
    {
        return await _context.Sessions
            .Where(s => s.CreatedAt >= startDate && s.CreatedAt <= endDate)
            .OrderBy(s => s.CreatedAt)
            .ToListAsync();
    }
}
```

---

**Authority:** Vault Steward  
**Last Updated:** 2026-08-25
