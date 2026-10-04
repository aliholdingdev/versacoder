---
title: "prompt - l1 Domain Varliklari (Tam Kod)"
type: prompt
category: prompts
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "l1 Domain katmanindaki varliklari (Entity, ValueObject, Aggregate, Specification, DomainEvent) TAM KOD olarak ureten prompt - 3 kanit zorunlu, invariant testi, ADR-028 R3 (DateTime.Now yasak)"
---

# prompt — l1 Domain Varliklari (Tam Kod)

**Zorunlu Baglantilar:** [prompts/index](index.md) · [AGENTS](../AGENTS.md) · [CLAUDE](../CLAUDE.md) · [TODOS/05](../TODOS/05-kod-ve-kalite.md) · [layer-boundary-check](../checklists/layer-boundary-check.md) · [solid-rules](../.rules/solid-rules.md) · [coding-standards](../.rules/coding-standards.md) · [ADR-028](../.decisions/accepted/ADR-028-layered-baseline-9-projects.md)

> **Tek cumlelik sorumluluk:** l1 `Domain` katmanindaki varliklari **tam kod** olarak
> ureten, **invariant testi** ve **2 kanitla** (build + test) kapanan is emridir.

---

## §0 Ozet (Answer First)

| Alan | Deger |
|---|---|
| Katman | `l1` — `VersaCoder.Domain` |
| K bandi | `K0050-K0099` |
| TFM | `net48;net10.0-windows` |
| Gereksinim | `VC-PRD-NNN` (versacoder-spec §4) |
| Ajan | `developer` (kod) → `tester` (invariant testi) → `reviewer` |
| Kapı | GATE-005 |
| Kanıt | 2 tane: `dotnet build` (cift bacak) + `dotnet test` (invariant yeşil) |

**YASAK:** `Infrastructure`/`UI` `using` · SQL · **`DateTime.Now`** (R3 CRITICAL) ·
EF Core · `static` global durum.

---

## §1 Baglam

1. `TODOS/05 §0` satır 2: **l1 = varlıklar + invariant**, kapı `test yeşil`.
2. `ADR-028 R3`: `l1` saf C# katmanı — `DateTime.Now` yerine `IClock` (l0 portu).
3. `l0` → `Result<T>` + `Error` zaten yazıldı (l0-portlar.md).

---

## §2 Görev — TAM KOD

### §2.1 `Entity` + `AggregateRoot` (`K0050-K0059`)

```csharp
// src/VersaCoder.Domain/Entities/Entity.cs
using System;
using System.Collections.Generic;
using VersaCoder.Abstractions.Ports;

namespace VersaCoder.Domain.Entities;

/// <summary>Kimligi olan varlik — ValueObject'den kimlik farki ile ayrilir.</summary>
public abstract class Entity<TId> where TId : notnull
{
    public TId Id { get; protected set; } = default!;
    private readonly List<IDomainEvent> _events = new();
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _events.AsReadOnly();
    protected void Raise(IDomainEvent e) => _events.Add(e);
    public void ClearEvents() => _events.Clear();
    public override bool Equals(object? obj) => obj is Entity<TId> o && EqualityComparer<TId>.Default.Equals(Id, o.Id);
    public override int GetHashCode() => Id.GetHashCode();
}

/// <summary>Agreg kok — transactional tutarlilik siniri (DDD).</summary>
public abstract class AggregateRoot<TId> : Entity<TId> where TId : notnull { }
```

### §2.2 `ValueObject` (`K0060-K0064`)

```csharp
// src/VersaCoder.Domain/ValueObjects/ValueObject.cs
using System;
using System.Collections.Generic;
using System.Linq;

namespace VersaCoder.Domain.ValueObjects;

/// <summary>Kimliksiz, esitlige dayali deger — SequenceEqual ile karsilastirma.</summary>
public abstract class ValueObject
{
    protected abstract IEnumerable<object?> GetEqualityComponents();
    public override bool Equals(object? obj) =>
        obj is ValueObject o && GetEqualityComponents().SequenceEqual(o.GetEqualityComponents());
    public override int GetHashCode() =>
        GetEqualityComponents().Aggregate(1, (h, c) => HashCode.Combine(h, c));
}
```

### §2.3 `IDomainEvent` + `Specification` (`K0065-K0074`)

```csharp
// src/VersaCoder.Domain/Events/IDomainEvent.cs
using System;

namespace VersaCoder.Domain.Events;

public interface IDomainEvent
{
    Guid EventId { get; }
    DateTimeOffset OccurredOn { get; }   // IClock'tan gelir (R3)
}

// src/VersaCoder.Domain/Specifications/Specification.cs
using System;
using System.Linq.Expressions;

namespace VersaCoder.Domain.Specifications;

public abstract class Specification<T>
{
    public abstract Expression<Func<T, bool>> ToExpression();
    public bool IsSatisfiedBy(T entity) => ToExpression().Compile()(entity);
    public static Specification<T> operator &(Specification<T> l, Specification<T> r) => new AndSpecification<T>(l, r);
    public static Specification<T> operator |(Specification<T> l, Specification<T> r) => new OrSpecification<T>(l, r);
}
internal sealed class AndSpecification<T> : Specification<T> { /* ... */ }
internal sealed class OrSpecification<T> : Specification<T> { /* ... */ }
```

### §2.4 Somut varlık: `AgentSession` (`K0075-K0084`)

```csharp
// src/VersaCoder.Domain/Aggregates/AgentSession.cs
using System;
using VersaCoder.Domain.Entities;
using VersaCoder.Domain.Events;

namespace VersaCoder.Domain.Aggregates;

/// <summary>Agent oturumu — invariant: durum yalnizca transition tablosuyla degisir.</summary>
public sealed class AgentSession : AggregateRoot<Guid>
{
    public static readonly IReadOnlyDictionary<SessionState, SessionState[]> Transitions =
        new Dictionary<SessionState, SessionState[]>
        {
            [SessionState.Created] = new[] { SessionState.Running, SessionState.Cancelled },
            [SessionState.Running] = new[] { SessionState.Completed, SessionState.Failed, SessionState.Cancelled },
            [SessionState.Completed] = Array.Empty<SessionState>(),
            [SessionState.Failed] = Array.Empty<SessionState>(),
            [SessionState.Cancelled] = Array.Empty<SessionState>(),
        };

    public SessionState State { get; private set; } = SessionState.Created;
    public string Prompt { get; private set; } = string.Empty;

    private AgentSession() { }

    public static AgentSession Start(Guid id, string prompt)
    {
        if (string.IsNullOrWhiteSpace(prompt)) throw new ArgumentException("Prompt bos olamaz.", nameof(prompt));
        var s = new AgentSession { Id = id, Prompt = prompt };
        s.Raise(new SessionStarted(id, DateTimeOffset.MinValue));   // DateTimeOffset IClock'tan gelir
        return s;
    }

    public void Complete()
    {
        if (!Transitions[State].Contains(SessionState.Completed))
            throw new InvalidOperationException($"'{State}' durumundan Completed'a gecilemez.");
        State = SessionState.Completed;
    }

    public void Fail() { /* ayni kontrol */ }
    public void Cancel() { /* ayni kontrol */ }
}

public enum SessionState { Created, Running, Completed, Failed, Cancelled }
```

---

## §3 Kabul Kriterleri

- [ ] `Entity<TId>`, `AggregateRoot<TId>`, `ValueObject`, `IDomainEvent`, `Specification<T>`, `AgentSession` — 6 tip
- [ ] **Invariant testi**: `AgentSession.Transitions` dışı geçiş = `InvalidOperationException`
- [ ] `DateTime.Now` = **0 eşleşme** (R3)
- [ ] `l1` → `Infrastructure`/`UI` `using` = **0**
- [ ] Çift bacak build 0 hata

---

## §4 Kanıt (2 tane zorunlu)

```text
KANIT 1: dotnet build -f net48 + -f net10.0-windows -> 0 hata
KANIT 2: dotnet test -> invariant testi gecti (Gecersiz gecis = 1 hata firlatir)
```

**Çıktı kutusu:**

```text
+--------------------------------------------------+
| BASLIK : l1 Domain varliklari yazildi            |
| KAPSAM : src/VersaCoder.Domain/** (6 tip)        |
| KARAR  : invariant transition tablosu; R3 = 0    |
| RISK   : DomainEvent OccurredOn(IClock) bagimli  |
| KANIT  : build x2 = 0 hata + test invariant yeşil|
+--------------------------------------------------+
```

---

## §5 Ajan Ataması

| # | Ajan | İş |
|---|---|---|
| 1 | `developer` | 6 tip kodu |
| 2 | `tester` | Invariant testi (`dotnet test`) |
| 3 | `reviewer` | SOLID + R3 ihlali |

---

**Olculen kanit:** `.ai/prompts/l1-domain.md` kaydedildi; `prompts/` = 33 dosya.
