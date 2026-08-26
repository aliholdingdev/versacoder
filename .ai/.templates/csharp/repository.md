---
title: "Repository Template"
type: template
category: csharp
version: 1.0.0
---

# Repository Template

## Kullanım

Yeni bir repository oluştururken bu template'i kullanın.

## Interface Template (L1 — Abstractions)

```csharp
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using {DomainNamespace};

namespace {AbstractionsNamespace}
{
    /// <summary>
    /// {EntityName} repository arayüzü
    /// </summary>
    public interface I{EntityName}Repository
    {
        /// <summary>
        /// ID ile getir
        /// </summary>
        Task<{EntityName}?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Tümünü getir
        /// </summary>
        Task<List<{EntityName}>> GetAllAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Sayfalı getir
        /// </summary>
        Task<(List<{EntityName}> Items, int TotalCount)> GetPagedAsync(
            int page,
            int pageSize,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Ekle
        /// </summary>
        Task<{EntityName}> AddAsync(
            {EntityName} entity,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Güncelle
        /// </summary>
        Task UpdateAsync(
            {EntityName} entity,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Sil
        /// </summary>
        Task DeleteAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Kaydet
        /// </summary>
        Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default);
    }
}
```

## Implementation Template (L4 — Infrastructure.Data)

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using {DomainNamespace};
using {AbstractionsNamespace};

namespace {DataNamespace}
{
    /// <summary>
    /// {EntityName} repository implementasyonu
    /// </summary>
    public class {EntityName}Repository : I{EntityName}Repository
    {
        private readonly VersaCoderDbContext _context;

        public {EntityName}Repository(VersaCoderDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<{EntityName}?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            return await _context.Set<{EntityName}>()
                .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
        }

        public async Task<List<{EntityName}>> GetAllAsync(
            CancellationToken cancellationToken = default)
        {
            return await _context.Set<{EntityName}>()
                .ToListAsync(cancellationToken);
        }

        public async Task<(List<{EntityName}> Items, int TotalCount)> GetPagedAsync(
            int page,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            var query = _context.Set<{EntityName}>();

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return (items, totalCount);
        }

        public async Task<{EntityName}> AddAsync(
            {EntityName} entity,
            CancellationToken cancellationToken = default)
        {
            await _context.Set<{EntityName}>()
                .AddAsync(entity, cancellationToken);

            return entity;
        }

        public async Task UpdateAsync(
            {EntityName} entity,
            CancellationToken cancellationToken = default)
        {
            _context.Set<{EntityName}>()
                .Update(entity);

            await Task.CompletedTask;
        }

        public async Task DeleteAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var entity = await GetByIdAsync(id, cancellationToken);
            if (entity != null)
            {
                _context.Set<{EntityName}>()
                    .Remove(entity);
            }
        }

        public async Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
```

---

## 3. Generic Repository Template

### 3.1 Generic Interface

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace {Namespace}
{
    /// <summary>
    /// Genel repository arayüzü
    /// </summary>
    /// <typeparam name="T">Entity tipi</typeparam>
    public interface IRepository<T> where T : class
    {
        /// <summary>
        /// ID ile getir
        /// </summary>
        Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default);

        /// <summary>
        /// Tümünü getir
        /// </summary>
        Task<IReadOnlyList<T>> GetAllAsync(CancellationToken ct = default);

        /// <summary>
        /// Filtre ile getir
        /// </summary>
        Task<IReadOnlyList<T>> FindAsync(
            Expression<Func<T, bool>> predicate,
            CancellationToken ct = default);

        /// <summary>
        /// Tek kayıt getir
        /// </summary>
        Task<T?> FirstOrDefaultAsync(
            Expression<Func<T, bool>> predicate,
            CancellationToken ct = default);

        /// <summary>
        /// Sayfalı getir
        /// </summary>
        Task<(IReadOnlyList<T> Items, int TotalCount)> GetPagedAsync(
            int pageNumber,
            int pageSize,
            CancellationToken ct = default);

        /// <summary>
        /// Ekle
        /// </summary>
        Task<T> AddAsync(T entity, CancellationToken ct = default);

        /// <summary>
        /// Toplu ekle
        /// </summary>
        Task AddRangeAsync(IEnumerable<T> entities, CancellationToken ct = default);

        /// <summary>
        /// Güncelle
        /// </summary>
        void Update(T entity);

        /// <summary>
        /// Toplu güncelle
        /// </summary>
        void UpdateRange(IEnumerable<T> entities);

        /// <summary>
        /// Sil
        /// </summary>
        void Remove(T entity);

        /// <summary>
        /// Toplu sil
        /// </summary>
        void RemoveRange(IEnumerable<T> entities);

        /// <summary>
        /// Say
        /// </summary>
        Task<int> CountAsync(
            Expression<Func<T, bool>>? predicate = null,
            CancellationToken ct = default);

        /// <summary>
        /// Var mı kontrol et
        /// </summary>
        Task<bool> AnyAsync(
            Expression<Func<T, bool>> predicate,
            CancellationToken ct = default);
    }
}
```

### 3.2 Generic Implementation

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace {Namespace}
{
    /// <summary>
    /// Genel repository implementasyonu
    /// </summary>
    /// <typeparam name="T">Entity tipi</typeparam>
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly VersaCoderDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public Repository(VersaCoderDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _dbSet = context.Set<T>();
        }

        public virtual async Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            return await _dbSet.FindAsync(new object[] { id }, ct);
        }

        public virtual async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken ct = default)
        {
            return await _dbSet.ToListAsync(ct);
        }

        public virtual async Task<IReadOnlyList<T>> FindAsync(
            Expression<Func<T, bool>> predicate,
            CancellationToken ct = default)
        {
            return await _dbSet.Where(predicate).ToListAsync(ct);
        }

        public virtual async Task<T?> FirstOrDefaultAsync(
            Expression<Func<T, bool>> predicate,
            CancellationToken ct = default)
        {
            return await _dbSet.FirstOrDefaultAsync(predicate, ct);
        }

        public virtual async Task<(IReadOnlyList<T> Items, int TotalCount)> GetPagedAsync(
            int pageNumber,
            int pageSize,
            CancellationToken ct = default)
        {
            var query = _dbSet.AsQueryable();
            
            var totalCount = await query.CountAsync(ct);
            
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);
            
            return (items, totalCount);
        }

        public virtual async Task<T> AddAsync(T entity, CancellationToken ct = default)
        {
            await _dbSet.AddAsync(entity, ct);
            return entity;
        }

        public virtual async Task AddRangeAsync(IEnumerable<T> entities, CancellationToken ct = default)
        {
            await _dbSet.AddRangeAsync(entities, ct);
        }

        public virtual void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        public virtual void UpdateRange(IEnumerable<T> entities)
        {
            _dbSet.UpdateRange(entities);
        }

        public virtual void Remove(T entity)
        {
            _dbSet.Remove(entity);
        }

        public virtual void RemoveRange(IEnumerable<T> entities)
        {
            _dbSet.RemoveRange(entities);
        }

        public virtual async Task<int> CountAsync(
            Expression<Func<T, bool>>? predicate = null,
            CancellationToken ct = default)
        {
            return predicate == null
                ? await _dbSet.CountAsync(ct)
                : await _dbSet.CountAsync(predicate, ct);
        }

        public virtual async Task<bool> AnyAsync(
            Expression<Func<T, bool>> predicate,
            CancellationToken ct = default)
        {
            return await _dbSet.AnyAsync(predicate, ct);
        }
    }
}
```

---

## 4. Specific Repository Examples

### 4.1 Session Repository

```csharp
namespace VersaCoder.Infrastructure.Data.Repositories
{
    /// <summary>
    /// Session repository implementasyonu
    /// </summary>
    public class SessionRepository : Repository<Session>, ISessionRepository
    {
        public SessionRepository(VersaCoderDbContext context) : base(context) { }

        public async Task<Session?> GetByNameAsync(string name, CancellationToken ct = default)
        {
            return await _dbSet
                .FirstOrDefaultAsync(s => s.Name == name, ct);
        }

        public async Task<IReadOnlyList<Session>> GetByProjectIdAsync(
            Guid projectId, CancellationToken ct = default)
        {
            return await _dbSet
                .Where(s => s.ProjectId == projectId)
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync(ct);
        }

        public override async Task<Session?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            return await _dbSet
                .Include(s => s.Messages)
                .FirstOrDefaultAsync(s => s.Id == id, ct);
        }
    }
}
```

### 4.2 Message Repository

```csharp
namespace VersaCoder.Infrastructure.Data.Repositories
{
    /// <summary>
    /// Message repository implementasyonu
    /// </summary>
    public class MessageRepository : Repository<Message>, IMessageRepository
    {
        public MessageRepository(VersaCoderDbContext context) : base(context) { }

        public async Task<IReadOnlyList<Message>> GetBySessionIdAsync(
            Guid sessionId, CancellationToken ct = default)
        {
            return await _dbSet
                .Where(m => m.SessionId == sessionId)
                .OrderBy(m => m.CreatedAt)
                .ToListAsync(ct);
        }

        public async Task<IReadOnlyList<Message>> GetByRoleAsync(
            string role, CancellationToken ct = default)
        {
            return await _dbSet
                .Where(m => m.Role == role)
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync(ct);
        }
    }
}
```

---

## 5. Unit of Work Template

### 5.1 Unit of Work Interface

```csharp
namespace {Namespace}
{
    /// <summary>
    /// Unit of work arayüzü
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// Session repository
        /// </summary>
        ISessionRepository Sessions { get; }

        /// <summary>
        /// Message repository
        /// </summary>
        IMessageRepository Messages { get; }

        /// <summary>
        /// Değişiklikleri kaydet
        /// </summary>
        Task<int> SaveChangesAsync(CancellationToken ct = default);

        /// <summary>
        /// İşlem başla
        /// </summary>
        Task BeginTransactionAsync(CancellationToken ct = default);

        /// <summary>
        /// İşlem onayla
        /// </summary>
        Task CommitTransactionAsync(CancellationToken ct = default);

        /// <summary>
        /// İşlem geri al
        /// </summary>
        Task RollbackTransactionAsync(CancellationToken ct = default);
    }
}
```

### 5.2 Unit of Work Implementation

```csharp
namespace VersaCoder.Infrastructure.Data
{
    /// <summary>
    /// Unit of work implementasyonu
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly VersaCoderDbContext _context;
        private IDbContextTransaction? _transaction;

        private ISessionRepository? _sessions;
        private IMessageRepository? _messages;

        public UnitOfWork(VersaCoderDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public ISessionRepository Sessions =>
            _sessions ??= new SessionRepository(_context);

        public IMessageRepository Messages =>
            _messages ??= new MessageRepository(_context);

        public async Task<int> SaveChangesAsync(CancellationToken ct = default)
        {
            return await _context.SaveChangesAsync(ct);
        }

        public async Task BeginTransactionAsync(CancellationToken ct = default)
        {
            _transaction = await _context.Database.BeginTransactionAsync(ct);
        }

        public async Task CommitTransactionAsync(CancellationToken ct = default)
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync(ct);
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task RollbackTransactionAsync(CancellationToken ct = default)
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync(ct);
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }
}
```

---

## 6. Quality Report

| Metrik | Değer |
|--------|-------|
| Version | 1.1.0 |
| Status | Active |
| Repository Types | 2 (Generic, Specific) |
| Unit of Work | 1 |
| Examples | 3 (Session, Message, Generic) |

---

**Authority:** Vault Steward
**Last Updated:** 2026-08-26
**Mode:** Red Team · Human Mode · Truth Mode
